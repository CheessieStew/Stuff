using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z1_7
{
    public partial class print : System.Web.UI.Page
    {
        List<int> ints = Enumerable.Range(1, 10).ToList();
        protected int sum;

        protected void Page_Load(object sender, EventArgs e)
        {

            TaskNo.DataSource = ints;

            List<String> points = ints.Select(i => Request.Form[$"TasksRepeater$ctl0{i - 1}$TaskN"]).ToList();
            TaskPo.DataSource = points;
            sum = points.Aggregate(0, (s,x)=> (x!=null && x.Length>0) ? s+int.Parse(x) : s);
            TaskNo.DataBind();
            TaskPo.DataBind();
        }

        protected string PointsString(object o)
        {
            if (o != null && o.ToString().Length > 0)
                return o.ToString();
            else return "0";
        }
    }
}