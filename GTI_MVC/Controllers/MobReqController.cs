using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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

            int _num = mobreqRepository.Incluir_Mobreq_Main(reg);
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
            model.AnoNumero= _req.Numero.ToString("0000") + "/" + _req.Ano.ToString();
            return View(model);
        }

        [Route("Mobreq_sole")]
        [HttpPost]
        public ActionResult Mobreq_sole(MobReqViewModel model,string action) {
            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            short _novaSituacao = 1;
            if(action== "btnConcluido") {
                _novaSituacao = 2;
            } else {
                if(action== "btnCancelar") {
                    _novaSituacao = 3;
                }
            }
            Exception ex = mobreqRepository.Alterar_Situacao(model.Guid,_novaSituacao,_userId);
            return RedirectToAction("Mobreq_query");
        }

        public ActionResult Mobreq_print(string p) {
            Mobreq_bll mobreqRepository = new Mobreq_bll(_connection);
            Mobreq_main_Struct _req = mobreqRepository.Retorna_Requerimento(p);
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_req.Codigo);

            string _rgie = "N/D";
            bool _bCpf = _req.CpfCnpj.Length == 11 ? true : false;
            if(_bCpf)
                _rgie = string.IsNullOrEmpty(_dados.Rg) ? _rgie : _dados.Rg;
            else
                _rgie = string.IsNullOrEmpty(_dados.Inscricao_estadual) ? _rgie : _dados.Inscricao_estadual;

            string _endereco = _dados.Endereco_nome_abreviado + ", " + _dados.Numero.ToString() + " " + _dados.Complemento??"" + ", " + _dados.Bairro_nome + " " ;
            _endereco += _dados.Cidade_nome + "/" + _dados.UF;


            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(_req.UserId);

            string _filename = "";
            if(_req.Tipo_Codigo == 1)
                _filename = "MobReq_Inscricao.rpt";
            else {
                if(_req.Tipo_Codigo == 2) { 
                    _filename = "MobReq_Baixa.rpt";
                } else {
                    if(_req.Tipo_Codigo == 3) {
                        _filename = "MobReq_Alteracao.rpt";
                    } else {
                        if(_req.Tipo_Codigo == 4) {
                            _filename = "MobReq_Reativa.rpt";
                        }
                    }
                }
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/"+_filename));
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
            string IPAddress = builder.DataSource;
            string _userId = builder.UserID;
            string _pwd = builder.Password;

            crConnectionInfo.ServerName = IPAddress;
            crConnectionInfo.DatabaseName = "Tributacao";
            crConnectionInfo.UserID = _userId;
            crConnectionInfo.Password = _pwd;
            CrTables = rd.Database.Tables;
            foreach(Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            rd.SetParameterValue("Guid",p);
            rd.SetParameterValue("Razao",_dados.Razao_social);
            rd.SetParameterValue("CpfCnpj",Functions.FormatarCpfCnpj( _req.CpfCnpj));
            rd.SetParameterValue("RgIe",_rgie);
            rd.SetParameterValue("DataCadastro",_req.Data_Evento);
            rd.SetParameterValue("Endereco",_endereco);
            rd.SetParameterValue("Atividade",_dados.Atividade_extenso);
            rd.SetParameterValue("Nome",_user.Nome);
            rd.SetParameterValue("Telefone",_user.Telefone);
            rd.SetParameterValue("Email",_user.Email);
            rd.SetParameterValue("Obs",_req.Obs??"");

            try {
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream,"application/pdf","Requerimento.pdf");
            } catch {
                throw;
            }
        }

    }
}