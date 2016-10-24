using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z1_6
{
    public partial class index : System.Web.UI.Page
    {
        protected String SomeString;

        protected void Page_Load(object sender, EventArgs e)
        {
            SomeString = "Method: " + Request.HttpMethod;
            SomeString += "<br>QueryString:";
            foreach(String str in Request.QueryString)
            {
                SomeString += $"<br>{str}={Request.QueryString[str]}";
            }
            SomeString += "<br>Form:";    
            foreach(String str in Request.Form.AllKeys)
            {
                SomeString += $"<br>{str}={Request.Form[str]}";
            }
        }

    }
}