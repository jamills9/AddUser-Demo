using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MockUserAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Key language features and tools used by this MVC3 app:";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
