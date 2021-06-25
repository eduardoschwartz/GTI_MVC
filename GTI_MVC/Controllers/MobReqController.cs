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
            if(model.Evento_Codigo==3)
                return RedirectToAction("Mobreq_sola");
            else {
                return View(model);
            }
        }

        [Route("Mobreq_sola")]
        [HttpGet]
        public ActionResult Mobreq_sola() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            string _cpfcnpj = Functions.RetornaNumero( TempData["cpfcnpj"].ToString());
            int _evento = Convert.ToInt32(TempData["evento"]);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            int _codigo = 0;
            if(_bCpf)
                _codigo = empresaRepository.ExisteEmpresaCpf(_cpfcnpj);
            else
                _codigo = empresaRepository.ExisteEmpresaCnpj(_cpfcnpj);

            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);

            string _rgie = "N/D";
            if(_bCpf)
                _rgie = string.IsNullOrEmpty( _dados.Rg) ? _rgie:_dados.Rg;
            else
                _rgie = string.IsNullOrEmpty(_dados.Inscricao_estadual) ? _rgie : _dados.Inscricao_estadual;

            MobReqViewModel model = new MobReqViewModel();
            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_evento> Lista = mobreqRepository.Lista_vento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);


            model.Razao_Social = _dados.Razao_social;
            model.Codigo = _codigo;
            model.Rg_IE = _rgie;
            model.Atividade = _dados.Atividade_extenso;
            model.Evento_Codigo = _evento;
            model.CpfValue = Functions.FormatarCpfCnpj( _cpfcnpj);
            return View(model);
        }

        [Route("Mobreq_sola")]
        [HttpPost]
        public ActionResult Mobreq_sola(MobReqViewModel model) {


            return View(model);
        }


    }
}