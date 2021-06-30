using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
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
        public ActionResult Mobreq_sol(string t) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            MobReqViewModel model = new MobReqViewModel();

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            //List<Mobreq_evento> Lista = mobreqRepository.Lista_Evento();
            //ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);

            int _evento_codigo = Convert.ToInt32(t);
            if(string.IsNullOrEmpty(t)) t = "1";
            if(_evento_codigo < 1 ||  _evento_codigo > 4) _evento_codigo = 1;
            model.Evento_Codigo = _evento_codigo;
            model.Evento_Nome = mobreqRepository.Retorna_Evento(_evento_codigo);
            return View(model);
        }

        [Route("Mobreq_sol")]
        [HttpPost]
        public ActionResult Mobreq_sol(MobReqViewModel model) {

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_evento> Lista = mobreqRepository.Lista_Evento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            int _codigo = 0;
            string _cpfcnpj = Functions.RetornaNumero(model.CpfValue);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;
            if(_bCpf)
                _codigo = empresaRepository.ExisteEmpresaCpf_Todas(_cpfcnpj);
            else
                _codigo = empresaRepository.ExisteEmpresaCnpj_Todas(_cpfcnpj);

            if(_codigo == 0) {
                ViewBag.Result = "Não existe empresa com este Cpf/Cnpj";
                return View(model);
            }

            TempData["cpfcnpj"] = model.CpfValue;
            TempData["evento"] = model.Evento_Codigo;
            TempData["codigo"] = _codigo;
            return RedirectToAction("Mobreq_sola");
        }

        [Route("Mobreq_sola")]
        [HttpGet]
        public ActionResult Mobreq_sola() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            string _cpfcnpj = Functions.RetornaNumero( TempData["cpfcnpj"].ToString());
            int _evento = Convert.ToInt32(TempData["evento"]);
            int _codigo = Convert.ToInt32(TempData["codigo"]);
            string _evento_nome = mobreqRepository.Retorna_Evento(_evento);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);

            string _rgie = "N/D";
            if(_bCpf)
                _rgie = string.IsNullOrEmpty( _dados.Rg) ? _rgie:_dados.Rg;
            else
                _rgie = string.IsNullOrEmpty(_dados.Inscricao_estadual) ? _rgie : _dados.Inscricao_estadual;

            MobReqViewModel model = new MobReqViewModel();
            
            List<Mobreq_evento> Lista = mobreqRepository.Lista_Evento();
            ViewBag.ListaEvento = new SelectList(Lista,"Codigo","Descricao",1);

            model.Razao_Social = _dados.Razao_social;
            model.Codigo = _codigo;
            model.Rg_IE = _rgie;
            model.Atividade = _dados.Atividade_extenso;
            model.Evento_Codigo = _evento;
            model.Evento_Nome = _evento_nome;
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
                Obs=model.Obs,
                Situacao=1
            };

            Exception ex = mobreqRepository.Incluir_Mobreq_Main(reg);
            ViewBag.Result = "Dados enviados com sucesso.";
            return RedirectToAction("Mobreq_menu");
        }

        [Route("Mobreq_query")]
        [HttpGet]
        public ActionResult Mobreq_query() {
            Session["hashform"] = "mobreq";
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            List<int> Lista_Ano = new List<int>();
            for(int i = 2021;i <= DateTime.Now.Year;i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.ListaAno = new SelectList(Lista_Ano);

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            List<Mobreq_main_Struct> Lista_Req = mobreqRepository.Lista_Requerimentos(DateTime.Now.Year);

            MobReqQueryViewModel model = new MobReqQueryViewModel() {
                Ano_Selected = DateTime.Now.Year,
                Lista_req=Lista_Req
            };


            return View(model);
        }

        [Route("Mobreq_sole")]
        [HttpGet]
        public ActionResult Mobreq_sole(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            MobReqViewModel model = new MobReqViewModel();

            Mobreq_main_Struct _req = mobreqRepository.Retorna_Requerimento(p);

            bool _bCpf = _req.CpfCnpj.Length == 11 ? true : false;
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_req.Codigo);

            string _rgie = "N/D";
            if(_bCpf)
                _rgie = string.IsNullOrEmpty(_dados.Rg) ? _rgie : _dados.Rg;
            else
                _rgie = string.IsNullOrEmpty(_dados.Inscricao_estadual) ? _rgie : _dados.Inscricao_estadual;


            model.Razao_Social = _dados.Razao_social;
            model.Codigo = _req.Codigo;
            model.Obs = _req.Obs;
            model.Data_Evento = _req.Data_Evento.ToString("dd/MM/yyyy");
            model.Rg_IE = _rgie;
            model.Atividade = _dados.Atividade_extenso;
            model.Evento_Codigo = _req.Tipo_Codigo;
            model.Evento_Nome = _req.Tipo_Nome;
            model.CpfValue = Functions.FormatarCpfCnpj(_req.CpfCnpj);
            model.Guid = p;
            model.Data_Evento2 = _req.Data_Evento2==null?"": Convert.ToDateTime(_req.Data_Evento2).ToString("dd/MM/yyyy");
            model.Funcionario = _req.UserId2_Nome??"";
            model.Situacao_Codigo = _req.Situacao_Codigo;
            model.Situacao_Nome = _req.Situacao_Nome;
            return View(model);
        }




    }
}