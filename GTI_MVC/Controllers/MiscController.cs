using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI_Bll.Classes;
using GTI_Models.Models;

namespace GTI_MVC.Controllers
{
    public class MiscController : Controller
    {

        public ActionResult Calevent()
        {
            return View();
        }

        public JsonResult GetEvents() {
            misc_bll miscRepository = new misc_bll("GTIconnection");
            var events = miscRepository.Lista_Evento();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

    }
}