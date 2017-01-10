using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace z5_1.Models
{
    public class RandomModel
    {
        public bool Bool1 { get; set; }
        public bool Bool2 { get; set; }

        public IEnumerable<SelectListItem> Ints1
        {
            get
            {
                return new SelectList(Ints2.Reverse().Select(i => $"smth{i}"));
            }
        }

        public int[] Ints2
        {
            get
            { return new int[] { 3, 2, 1 }; }
        }

        public String Pwd1 { get; set; }
        public String Pwd2 { get; set; }

        public String Text1 { get; set; }
        public String Text2 { get; set; }


        public String BigText1 { get; set; }
        public String BigText2 { get; set; }

        public String Option1 { get; set; }
        public String Option2 { get; set; }


        public String Html { get { return "<br /> <br />"; } }

        public RandomModel()
        {
            
        }

    }
}