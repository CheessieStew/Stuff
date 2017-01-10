using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace z5_1
{
    public static class Utils
    {
        static string name1 = "name1";
        public static DataContext Connection
        {

            get
            {
                if (HttpContext.Current.Items[name1] != null)
                    return (DataContext)HttpContext.Current.Items[name1];

                DataContext res = new DataContext(ConfigurationManager.ConnectionStrings["Base"].ConnectionString);
                HttpContext.Current.Items.Add(name1, res);
                return res;
            }
        }
    }

    [Table(Name = "USER")]
    public class EntityUser
    {
        Guid _userID;
        [Column(IsPrimaryKey = true, Storage = "_userID")]
        public Guid Id
        {
            get { return _userID; }
            set { _userID = value; }
        }

        string _userName;
        [Column(Storage = "_userName")]
        public string Name
        {
            get { return _userName; }
            set { _userName = value; }
        }

        string _userEmail;
        [Column(Storage = "_userEmail")]
        public string Email
        {
            get { return _userEmail; }
            set { _userEmail = value; }
        }
    }

    [Table(Name = "PASSWORD")]
    public class EntityPassword
    {
        Guid _pwdID;
        [Column(IsPrimaryKey = true, Storage = "_pwdID")]
        public Guid Id
        {
            get { return _pwdID; }
            set { _pwdID = value; }
        }

        string _short;
        [Column(Storage = "_short")]
        public string Short
        {
            get { return _short; }
            set { _short = value; }
        }

        string _salt;
        [Column(Storage = "_salt")]
        public string Salt
        {
            get { return _salt; }
            set { _salt = value; }
        }

        int _rounds;
        [Column(Storage = "_rounds")]
        public int Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        DateTime _date;
        [Column(Storage = "_date")]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
    }

    [Table(Name = "USERINROLE")]
    public class EntityUserRole
    {
        Guid _userID;
        [Column(Storage = "_userID")]
        public Guid UserId
        {
            get { return _userID; }
            set { _userID = value; }
        }

        Guid _roleID;
        [Column(Storage = "_roleID")]
        public Guid RoleId
        {
            get { return _roleID; }
            set { _roleID = value; }
        }

        Guid _pairID;
        [Column(IsPrimaryKey = true, Storage = "_pairID")]
        public Guid PairId
        {
            get { return _pairID; }
            set { _pairID = value; }
        }
    }

    [Table(Name = "ROLE")]
    public class EntityRole
    {
        Guid _roleID;
        [Column(IsPrimaryKey = true, Storage = "_roleID")]
        public Guid Id
        {
            get { return _roleID; }
            set { _roleID = value; }
        }

        string _roleName;
        [Column(Storage = "_roleName")]
        public string Name
        {
            get { return _roleName; }
            set { _roleName = value; }
        }
    }
}