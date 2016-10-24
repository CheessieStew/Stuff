using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z1_7
{

    public partial class dekl : System.Web.UI.Page
    {
        List<int> ints = Enumerable.Range(1, 10).ToList();

        protected void Page_Load(object sender, EventArgs e)
        {

            TasksRepeater.DataSource = ints;
            TasksRepeater.DataBind();

            if(Request.RequestType == "POST")
            {
                bool right = true;
                if (Name.Text.Length==0)
                {
                    right = false;
                    Name.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    Name.BackColor = System.Drawing.Color.White;
                }

                if (Surname.Text.Length == 0)
                {
                    right = false;
                    Surname.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    Surname.BackColor = System.Drawing.Color.White;
                }

                DateTime date = DateTime.Now;
                if (Date.Text.Length == 0 || !DateTime.TryParse(Date.Text, out date))
                {
                    right = false;
                    Date.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    Date.BackColor = System.Drawing.Color.White;
                }

                if (ListNo.Text.Length == 0)
                {
                    right = false;
                    ListNo.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    ListNo.BackColor = System.Drawing.Color.White;
                }

                if (right) Server.Transfer("print.aspx");
            }
        }

    }
}