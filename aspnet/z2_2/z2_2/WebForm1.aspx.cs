using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace z2_2
{

    static class Kontenerek
    {
        static string n1 = "c1";
        static string n2 = "c2";
        static string n3 = "c3";
        public static int c1
        {
            get
            {
                if (HttpContext.Current.Application[n1]!=null)
                {
                    return (int)HttpContext.Current.Application[n1];
                }
                return 0;
            }
            set
            {
                if (HttpContext.Current.Application[n1] != null)
                {
                    HttpContext.Current.Application[n1] = value;
                }
                else
                {
                    HttpContext.Current.Application.Add(n1, value);
                }
            }
        }
        public static int c2
        {
            get
            {
                if (HttpContext.Current.Session[n2] != null)
                {
                    return (int)HttpContext.Current.Session[n2];
                }
                return 0;
            }
            set
            {
                if (HttpContext.Current.Session[n2] != null)
                {
                    HttpContext.Current.Session[n2] = value;
                }
                else
                {
                    HttpContext.Current.Session.Add(n2, value);
                }
            }
        }
        public static int c3
        {
            get
            {
                if (HttpContext.Current.Items[n3] != null)
                {
                    return (int)HttpContext.Current.Items[n3];
                }
                return 0;
            }
            set
            {
                if (HttpContext.Current.Items[n3] != null)
                {
                    HttpContext.Current.Items[n3] = value;
                }
                else
                {
                    HttpContext.Current.Items.Add(n3, value);
                }
            }
        }
    }

    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lab2.Text = Request.Headers["Host"];
            lab3.Text = Server.MapPath("asdf.txt");
            lc1.Text = Kontenerek.c1.ToString();
            lc2.Text = Kontenerek.c2.ToString();
            lc3.Text = Kontenerek.c3.ToString();
            Kontenerek.c1++;
            Kontenerek.c2++;
            Kontenerek.c3++;
            if (Request.HttpMethod == "POST")
            {
                if (file.HasFile)
                {
                    System.IO.Stream stream = file.PostedFile.InputStream;
                    int sum = 0;
                    int len = 0;
                    int read = stream.ReadByte();
                    while (read != -1)
                    {
                        sum += read;
                        sum %= 0xFFFF;
                        read = stream.ReadByte();
                    }
                    //Response.Headers
                    Response.Clear();
                    Response.Write($"<opis>\n<nazwa>{file.PostedFile.FileName}</nazwa>\n");
                    Response.Write($"<rozmiar>{file.PostedFile.ContentLength}</rozmiar>\n");
                    Response.Write($"<sygnatura>{sum}</sygnatura>\n</opis>");
                    Response.ContentType = "text/xml";
                    Response.ContentEncoding = System.Text.Encoding.Unicode;
                    Response.AddHeader("content-disposition", @"attachment;filename=""result.xml""");
                    Response.End();
                }
                HttpCookie cookie = new HttpCookie("cookie", text1.Text);
                if (text1.Text == "")
                    cookie.Expires = DateTime.Now.AddYears(-100);
                Response.Cookies.Add(cookie);
                if (text1.Text == "abandon")
                    Session.Abandon();
                //Response.Headers.Remove("Server");
            }
            //HttpContext.Current.PageInstrumentation ?????? żadna właściwość currenta nie ma typu System.Web.UI.Page
     

            HttpCookie cookier = Request.Cookies["cookie"];
            if (cookier != null) lab1.Text = cookier.Value;
            else lab1.Text = "no cookie";
            Response.Headers.Add("CustomHeader", "CustomHeader content");

        }
    }
}