using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace z5_1.Controllers
{
    public class ExamplesController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View(new Models.RandomModel());
        }

        [HttpPost]
        public ActionResult Index(Models.RandomModel m)
        {
            if (m == null)
                return View(new Models.RandomModel());
            else return View(m);
        }

    }
}