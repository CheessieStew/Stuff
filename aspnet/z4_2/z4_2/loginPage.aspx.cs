using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;

namespace z4_2
{
    public partial class loginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Roles.RoleExists("Gardener"))
                Roles.CreateRole("Gardener");
            if (!Roles.RoleExists("Admin"))
                Roles.CreateRole("Admin");
            if (Membership.GetUser("Admin")!= null)
                if (!Roles.GetRolesForUser("Admin").Contains("Admin"))
                    Roles.AddUsersToRole(new String[] { "Admin"}, "Admin");
        }
    }
}