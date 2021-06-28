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

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            int _codigo = 0;
            string _cpfcnpj = Functions.RetornaNumero(model.CpfValue);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;
            if(_bCpf)
                _codigo = empresaRepository.ExisteEmpresaCpf(_cpfcnpj);
            else
                _codigo = empresaRepository.ExisteEmpresaCnpj(_cpfcnpj);

            if(_codigo == 0) {
                ViewBag.Result = "Não existe empresa com este Cpf/Cnpj";
                return View(model);
            }

            TempData["cpfcnpj"] = model.CpfValue;
            TempData["evento"] = model.Evento_Codigo;
            TempData["codigo"] = _codigo;
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
            int _codigo = Convert.ToInt32(TempData["codigo"]);

            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;

            Empresa_bll empresaRepository = new Empresa_bll(_connection);

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

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            string _guid= _guid = Guid.NewGuid().ToString("N");
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _func = Session["hashfunc"].ToString() == "S" ? true : false;

            Mobreq_main reg = new Mobreq_main() {
                Guid = _guid,
                Codigo= model.Codigo,
                Tipo = model.Evento_Codigo,
                Data_Inclusao =DateTime.Now,
                Data_Evento= Convert.ToDateTime( model.Data_Evento),
                UserId=_userId,
                UserPrf=_func,
                Obs=model.Obs
            };

            Exception ex = mobreqRepository.Incluir_Mobreq_Main(reg);
            ViewBag.msg = "ok";
            return View(model);
        }


    }
}