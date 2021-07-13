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
        private readonly string _connection = "GTIconnectionTeste";

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
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            return View(model);
        }

        [Route("Cidadao_add")]
        [HttpPost]
        public ActionResult Cidadao_add(CidadaoViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            return View(model);
        }


    }
}