using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers
{
    public class MobReqController : Controller
    {
        private readonly string _connection = "GTIconnection";

        [Route("Mobreq_menu")]
        [HttpGet]
        public ActionResult Mobreq_menu() {
            Session["hashform"] = "mobreq";
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");
            ViewBag.Fiscal = Session["hashfiscalmov"] == null ? "N" : Session["hashfiscalmov"].ToString();
            return View();
        }

        [Route("Mobreq_sol")]
        [HttpGet]
        public ActionResult Mobreq_sol() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            MobReqViewModel model = new MobReqViewModel();

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_evento> Lista = mobreqRepository.Lista_vento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);

            model.Evento_Codigo = 1;
            return View(model);
        }

        [Route("Mobreq_sol")]
        [HttpPost]
        public ActionResult Mobreq_sol(MobReqViewModel model) {

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_evento> Lista = mobreqRepository.Lista_vento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);


            TempData["cpfcnpj"] = model.CpfValue;
            TempData["evento"] = model.Evento_Codigo;
            return RedirectToAction("Mobreq_solb");
        }

        [Route("Mobreq_solb")]
        [HttpGet]
        public ActionResult Mobreq_solb() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            string _cpfcnpj = TempData["cpfcnpj"].ToString();
            int _evento = Convert.ToInt32(TempData["evento"]);

            MobReqViewModel model = new MobReqViewModel();
            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_evento> Lista = mobreqRepository.Lista_vento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);

            model.Evento_Codigo = _evento;
            model.CpfValue = _cpfcnpj;
            return View(model);
        }

    }
}