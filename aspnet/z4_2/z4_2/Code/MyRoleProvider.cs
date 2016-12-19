using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace z4_2
{
    public class MyRoleProvider : RoleProvider
    {
        private string applicationName;



        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }

            set
            {
                applicationName = value;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            // 
            // Initialize values from web.config. 
            // 

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "MyRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Best role provider evah");
            }

            // Initialize the abstract base class. 
            base.Initialize(name, config);

            applicationName = config["applicationName"];
            if (String.IsNullOrEmpty(applicationName))
                applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;


        }

        public override void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            foreach (string roleName in roleNames)
            {
                if (!RoleExists(roleName))
                {
                    throw new ProviderException("Role name not found.");
                }
            }

            List<EntityUserRole> added = new List<EntityUserRole>();
            var ut = Utils.Connection.GetTable<EntityUser>();
            var rt = Utils.Connection.GetTable<EntityRole>();
            var urt = Utils.Connection.GetTable<EntityUserRole>();

            foreach (string userName in userNames)
            {
                if (Membership.GetUser(userName)==null)
                    throw new ProviderException("User name not found");

                foreach (string roleName in roleNames)
                {
                    if (IsUserInRole(userName, roleName))
                    {
                        throw new ProviderException("User is already in role.");
                    }
                    EntityUserRole ur = new EntityUserRole();
                    ur.RoleId = rt.First(r => r.Name == roleName).Id;
                    ur.UserId = ut.First(u => u.Name == userName).Id;
                    ur.PairId = Guid.NewGuid();
                    added.Add(ur);
                }
            }
            urt.InsertAllOnSubmit(added);
            Utils.Connection.SubmitChanges();
        }

        public override void CreateRole(string roleName)
        {
            if (RoleExists(roleName))
                throw new Exception();
            var rt = Utils.Connection.GetTable<EntityRole>();
            EntityRole role = new EntityRole();
            role.Name = roleName;
            role.Id = Guid.NewGuid();
            rt.InsertOnSubmit(role);
            Utils.Connection.SubmitChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (!RoleExists(roleName))
                throw new Exception();
            EntityRole role = Utils.Connection.GetTable<EntityRole>().First(r => r.Name == roleName);
            Guid rId = role.Id;
            if (throwOnPopulatedRole &&
                Utils.Connection.GetTable<EntityUserRole>().Any())
                throw new ProviderException("Role populated");
            Utils.Connection.GetTable<EntityRole>().DeleteOnSubmit(role);
            Utils.Connection.SubmitChanges();
            return true;
        }


        public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
        {
            var ut = Utils.Connection.GetTable<EntityUser>();
            var rt = Utils.Connection.GetTable<EntityRole>();
            var urt = Utils.Connection.GetTable<EntityUserRole>();
            var q = from ur in urt
                    where roleNames.Contains(rt.First(r => r.Id == ur.RoleId).Name) &&
                          userNames.Contains(ut.First(u => u.Id == ur.UserId).Name)
                    select ur;
            foreach (var ur in q)
                urt.DeleteOnSubmit(ur);
            Utils.Connection.SubmitChanges();
        }

        public override string[] GetAllRoles()
        {
            return Utils.Connection.GetTable<EntityRole>().Select(r => r.Name).ToArray();
        }

        public override string[] GetRolesForUser(string userName)
        {
            Guid uId = Utils.Connection.GetTable<EntityUser>().First(u => u.Name == userName).Id;
            var urt = Utils.Connection.GetTable<EntityUserRole>();
            return Utils.Connection.GetTable<EntityRole>()
                .Where(r =>
                    urt.Any(ur => 
                        ur.RoleId == r.Id && ur.UserId == uId))
                .Select(r => r.Name).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            Guid rId = Utils.Connection.GetTable<EntityRole>().First(r => r.Name == roleName).Id;
            var urt = Utils.Connection.GetTable<EntityUserRole>();
            return Utils.Connection.GetTable<EntityUser>()
                .Where(u =>
                    urt.Any(ur =>
                        ur.RoleId == rId && ur.UserId == u.Id))
                .Select(r => r.Name).ToArray();
        }

        public override bool IsUserInRole(string userName, string roleName)
        {
            Guid rId = Utils.Connection.GetTable<EntityRole>().First(r => r.Name == roleName).Id;
            Guid uId = Utils.Connection.GetTable<EntityUser>().First(u => u.Name == userName).Id;
            return Utils.Connection.GetTable<EntityUserRole>().Any(ur => ur.RoleId == rId && ur.UserId == uId);
        }



        public override bool RoleExists(string roleName)
        {
            return Utils.Connection.GetTable<EntityRole>().Any(r => r.Name == roleName);
        }

        public override string[] FindUsersInRole(string roleName, string userNameToMatch)
        {
            var ut = Utils.Connection.GetTable<EntityUser>();
            var rt = Utils.Connection.GetTable<EntityRole>();
            var urt = Utils.Connection.GetTable<EntityUserRole>();
            Guid rId = rt.First(r => r.Name == roleName).Id;

            var x = from u in ut
                    where u.Name.Contains(userNameToMatch) &&
                          urt.Any(ur => ur.UserId == u.Id && ur.RoleId == rId)
                    select u.Name;
            return x.ToArray();
        }
    }
}