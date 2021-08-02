using GTI_Bll.Classes;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTI_MVC.Controllers
{
    public class ProcessoController : Controller    {
        private readonly string _connection = "GTIconnectionTeste";

        [Route("Processo_menu")]
        [HttpGet]
        public ActionResult Processo_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Processo_tp")]
        [HttpGet]
        public ActionResult Processo_tp() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Processo2ViewModel model = new Processo2ViewModel();

            return View(model);
        }

        [Route("Processo_tp")]
        [HttpPost]
        public ActionResult Processo_tp(Processo2ViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");


            return View(model);
        }


    }
}