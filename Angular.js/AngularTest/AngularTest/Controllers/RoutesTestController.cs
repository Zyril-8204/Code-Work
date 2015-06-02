using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AngularTest.Controllers
{
    public class RoutesTestController : Controller
    {
        public ActionResult One()
        {
            return View();
        }

        public ActionResult Two(int id = 1)
        {
            // set a fake id for now
            ViewBag.ID = id;
            return View();
        }

        public ActionResult Three()
        {
            return View();
        }

        public ActionResult Four()
        {
            return View();
        }
    }
}