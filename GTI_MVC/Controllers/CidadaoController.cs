using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_MVC.Controllers
{
    public class CidadaoController : Controller   {
        private readonly string _connection = "GTIconnection";
        private readonly string _connectionTeste = "GTIconnectionTeste";

        [Route("Cidadao_menu")]
        [HttpGet]
        public ActionResult Cidadao_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Cidadao_add")]
        [HttpGet]
        public ActionResult Cidadao_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();

            return View(model);
        }

        [Route("Cidadao_add")]
        [HttpPost]
        public ActionResult Cidadao_add(CidadaoViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");


            return View(model);
        }


    }
}