using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z7_1
{
    public partial class deklarka : System.Web.UI.Page
    {
        List<int> ints = Enumerable.Range(1, 10).ToList();
        protected int sum;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TasksRepeater.DataSource = ints;
                TasksRepeater.DataBind();
            }
            List<string> points = new List<string>();
            foreach (RepeaterItem item in TasksRepeater.Items)
            {
                TextBox TaskNTB = (TextBox)item.FindControl("TaskNTB");
                points.Add(TaskNTB != null ? TaskNTB.Text : "0");
            }

            NameLabel.Text = NameTB.Text + " " + SurnameTB.Text;
            SumLabel.Text = points.Aggregate(0, (s, x) => (x != null && x.Length > 0) ? s + int.Parse(x) : s).ToString();

            ListNoLabel.Text = ListNoTB.Text;
            DateLabel.Text = dateTB.Text;

            TaskNo.DataSource = ints;
            TaskNo.DataBind();

            TaskPo.DataSource = points;
            TaskPo.DataBind();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ListNoLabel.Text = DateTime.Now.ToLongTimeString();
        }

        protected string PointsString(object o)
        {
            if (o != null && o.ToString().Length > 0)
                return o.ToString();
            else return "0";
        }

        protected void masterbutton_Click(object sender, EventArgs e)
        {
            List<string> points = new List<string>();
            foreach (RepeaterItem item in TasksRepeater.Items)
            {
                TextBox TaskNTB = (TextBox)item.FindControl("TaskNTB");
                points.Add(TaskNTB != null ? TaskNTB.Text : "0");
            }
            ViewState["PointsData"] = points;
        }
    }
}