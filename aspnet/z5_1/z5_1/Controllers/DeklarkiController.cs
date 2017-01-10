using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace z5_1.Controllers
{
    public class DeklarkiController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Print([Bind()] Models.Deklarka dek)
        {

            return View(dek);
        }
    }
}