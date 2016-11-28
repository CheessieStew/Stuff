using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public static class SomeData
{
    static DataTable _values;
    public static DataTable Values
    {
        get
        {
            if (_values == null)
            {
                _values = new DataTable("some name");

                DataColumn col = new DataColumn();
                col.DataType = typeof(string);
                col.ColumnName = "Name";
                col.Caption = "Name";
                _values.Columns.Add(col);
                col = new DataColumn();
                col.DataType = typeof(int);
                col.ColumnName = "Number";
                col.Caption = "#";
                _values.Columns.Add(col);
                for (int i = 0; i < 20; i++)
                {
                    DataRow row = _values.NewRow();
                    row["Number"] = i;
                    row["Name"] = $"Element # {i}";
                    _values.Rows.Add(row);
                }
            }
            return _values;
        }
    }

    static List<DateTime> _times;
    public static List<DateTime> Times
    {
        get
        {
            if (_times == null)
            {
                _times = new List<DateTime>();
                for (int i = 0; i < 20; i++)
                    _times.Add(DateTime.Now.AddDays(i));
            }
            return _times;
        }
    }
}

namespace z3_1
{
    public partial class bind1 : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            List1.DataSource = SomeData.Values;
            List1.DataBind();
            List1.DataTextField = "Name";
            List1.DataValueField = "Number";
            List1.DataBind();

            List2.DataSource = SomeData.Times;
            List2.DataBind();

            LinqDataSource src = new LinqDataSource();
            src.ContextTypeName = "SomeData";
            src.TableName = "Times";
            List3.DataSource = src;
            List3.DataBind();
        }
    }
}