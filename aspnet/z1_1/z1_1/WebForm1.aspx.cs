using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z1_1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        StreamReader sr;
        String filename;

        protected String PageText()
        {
            if (sr != null)
                return "file " + filename + ":" + sr.ReadToEnd();
            else return filename + ": File not found ;-;";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            filename = Server.MapPath("~") + Request.QueryString["filename"];
            try
            {
                sr = new StreamReader(filename);
            } catch { sr = null; }
        }


    }
}