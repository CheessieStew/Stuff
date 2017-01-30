using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace z5_1.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Models.Deklarka m = new Models.Deklarka();
            m.Class = "";
            m.Date = "";
            m.Points = new int[10];
            m.Name = "";
            return View(m);
        }

        [HttpPost]
        public ActionResult Index([Bind()] Models.Deklarka dek)
        {

            return View(dek);
        }
    }
}