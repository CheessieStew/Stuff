using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace z3_3
{
    public partial class ShopMain : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                ListView1.DataBind();
        }

        protected void ListView1_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "ShowInsertTemplate":

                    ListView1.InsertItemPosition = InsertItemPosition.FirstItem;
                    ListView1.DataBind();
                    ListView1.InsertItem.DataBind();

                    break;

                case "HideInsertTemplate":

                    ListView1.InsertItemPosition = InsertItemPosition.None;

                    break;

                case "DownloadXml":

                    int osobaId = Convert.ToInt32(e.CommandArgument);
                    ShopItem o = Model.Instance.Items[osobaId];

                    Response.Clear();
                    Response.AppendHeader("Content-Type", "text/xml");
                    Response.AppendHeader("Content-Disposition", "attachment; filename=kartoteka.xml");

                    Response.Write(o.Xml);

                    Response.Flush();
                    Response.End();

                    break;

            }
        }

        protected void ObjectDataSource1_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            // do metody biznesowej trzeba w e przekazać to, czego spodziewa się sygnatura metody Update

            int ID = (int)e.InputParameters["ID"];


            // czytanie nowych wartości 
            string name = ((TextBox)ListView1.EditItem.FindControl("NameEdit")).Text;
            double price = double.Parse(((TextBox)ListView1.EditItem.FindControl("PriceEdit")).Text);
            string description = ((TextBox)ListView1.EditItem.FindControl("DescriptionEdit")).Text;
            string picLink = ((TextBox)ListView1.EditItem.FindControl("PictureEdit")).Text;

            ShopItem item = Model.Instance.Items[ID];
            item.name = name;
            item.price = price;
            item.description = description;
            item.picLink = picLink;

            e.InputParameters.Clear();
            e.InputParameters.Add("item", item);
        }
        protected void ObjectDataSource1_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            // do metody biznesowej trzeba w e przekazać to, czego spodziewa się sygnatura metody Update


        }
        protected void ObjectDataSource1_Updated(object sender, ObjectDataSourceStatusEventArgs e)
        {
            ListView1.DataBind();
        }
        protected void ObjectDataSource1_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            ShopItem item = new ShopItem();

            // czytanie nowych wartości 
            string name = ((TextBox)ListView1.InsertItem.FindControl("NameInsert")).Text;
            double price = double.Parse(((TextBox)ListView1.InsertItem.FindControl("PriceInsert")).Text);
            string description = ((TextBox)ListView1.InsertItem.FindControl("DescriptionInsert")).Text;
            string picLink = ((TextBox)ListView1.InsertItem.FindControl("PictureInsert")).Text;

            item.name = name;
            item.price = price;
            item.description = description;
            item.picLink = picLink;

            e.InputParameters.Clear();
            e.InputParameters.Add("item", item);
        }
        protected void ObjectDataSource1_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
        {
            ListView1.InsertItemPosition = InsertItemPosition.None;
            ListView1.DataBind();
        }
    }

}