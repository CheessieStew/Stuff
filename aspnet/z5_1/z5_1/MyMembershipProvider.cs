using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace z5_1
{
    public class MyMembershipProvider : MembershipProvider
    {
        private static readonly string hashKey = "a3b1c6ff16";
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private string applicationName;
        private static RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();


        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MyMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Best membership provider evah");
            }

            base.Initialize(name, config);

            applicationName = config["applicationName"];
            if (String.IsNullOrEmpty(applicationName))
                applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

        }

        public override string ApplicationName
        {
            get
            { return applicationName; }

            set
            { applicationName = value; }
        }



        public override bool EnablePasswordReset
        {
            get { return false; }

        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 4; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 1; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }

        }

        public override string PasswordStrengthRegularExpression
        {
            get
            { return ""; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public EntityPassword MakePassword(Guid userID, string pwd)
        {
            EntityPassword ent = new EntityPassword();
            byte[] randomStuff = new byte[11];
            rand.GetBytes(randomStuff);
            char[] saltChars = new char[10];
            for (int i = 0; i < 10; i++)
                saltChars[i] = chars[randomStuff[i] % chars.Length];

            ent.Salt = new String(saltChars);
            ent.Rounds = randomStuff[10] % 9 + 1;
            ent.Date = DateTime.Now;
            ent.Id = userID;
            string encrypted = pwd + ent.Salt;
            HMACSHA1 hash = new HMACSHA1();
            hash.Key = HexToByte(hashKey);
            for (int i = 0; i < ent.Rounds; i++)
            {
                //encrypted = Convert.ToBase64String();
                encrypted = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(encrypted)));
            }
            ent.Short = encrypted;
            return ent;
        }

        public bool CheckPassword(EntityPassword realPwd, string pwd)
        {

            string encrypted = pwd + realPwd.Salt;
            HMACSHA1 hash = new HMACSHA1();
            hash.Key = HexToByte(hashKey);
            for (int i = 0; i < realPwd.Rounds; i++)
            {
                //encrypted = Convert.ToBase64String();
                encrypted = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(encrypted)));
            }
            return realPwd.Short == encrypted;
        }
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;
            ValidatePasswordEventArgs args =
                new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            EntityUser user = Utils.Connection.GetTable<EntityUser>().First(u => u.Name == username);
            if (user == null)
                return false;
            Guid id = user.Id;
            EntityPassword pwd = MakePassword(id, newPassword);
            var q = from p in Utils.Connection.GetTable<EntityPassword>()
                    where p.Id == id
                    select p;
            if (q.Count() != 1)
                throw new Exception("this should not happen");
            foreach (EntityPassword p in q)
            {
                p.Date = DateTime.Now;
                p.Rounds = pwd.Rounds;
                p.Salt = pwd.Salt;
                p.Short = pwd.Short;
            }

            Utils.Connection.SubmitChanges();
            return true;
        }



        public override MembershipUser CreateUser(string username,
            string password, string email, string passwordQuestion,
            string passwordAnswer, bool isApproved,
            object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
                new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            MembershipUser u = GetUser(username, false);
            if (u != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            EntityUser user = new EntityUser();
            user.Id = Guid.NewGuid();
            user.Name = username;
            user.Email = email;
            EntityPassword pwd = MakePassword(user.Id, password);
            Utils.Connection.GetTable<EntityUser>().InsertOnSubmit(user);
            Utils.Connection.GetTable<EntityPassword>().InsertOnSubmit(pwd);
            Utils.Connection.SubmitChanges();
            status = MembershipCreateStatus.Success;

            return GetUser(username, false);
        }



        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();
            var q = from u in Utils.Connection.GetTable<EntityUser>()
                    select u;
            totalRecords = q.Count();

            foreach (var u in q)
            {
                users.Add(FromDB(u));
            }
            return users;


        }

        MembershipUser FromDB(EntityUser u)
        {
            var pwdq = from p in Utils.Connection.GetTable<EntityPassword>()
                       where p.Id == u.Id
                       select p.Date;
            if (pwdq.Count() != 1)
                throw new Exception("Database corrupted or smth");
            return new MembershipUser(this.Name,
                                        u.Name,
                                        u.Id,
                                        u.Email,
                                        null,
                                        null,
                                        true,
                                        false,
                                        DateTime.Now.AddDays(-30),
                                        DateTime.Now,
                                        DateTime.Now,
                                        pwdq.First(),
                                        DateTime.Now.AddDays(-20));
        }



        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var q = from u in Utils.Connection.GetTable<EntityUser>()
                    where u.Name == username
                    select u;
            if (q.Count() == 0)
                return null;
            if (q.Count() > 1)
                throw new Exception("Database corrupted or smth");
            return FromDB(q.First());
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            Guid id = (Guid)providerUserKey;
            var q = from u in Utils.Connection.GetTable<EntityUser>()
                    where u.Id == id
                    select u;
            if (q.Count() == 0)
                return null;
            if (q.Count() > 1)
                throw new Exception("Database corrupted or smth");
            return FromDB(q.First());
        }

        public override bool ValidateUser(string username, string password)
        {
            var uq = from u in Utils.Connection.GetTable<EntityUser>()
                     where u.Name == username
                     select u;
            if (uq.Count() == 0)
                return false;
            if (uq.Count() > 1)
                throw new Exception("Database corrupted or smth");

            var theUser = uq.First();

            var pq = from p in Utils.Connection.GetTable<EntityPassword>()
                     where p.Id == theUser.Id
                     select p;

            if (pq.Count() != 1)
                throw new Exception("Database corrupted or smth");

            var realPwd = pq.First();
            return CheckPassword(realPwd, password);



        }




        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }
        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

    }
}