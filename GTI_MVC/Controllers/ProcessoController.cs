using Antlr.Runtime.Tree;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Web.Helpers;
using System.Web.Mvc;

namespace GTI_MVC.Controllers {
    public class ProcessoController : Controller    {
        private readonly string _connection = "GTIconnectionTeste";
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
            public void OnAuthorization(AuthorizationContext filterContext) {
                if (filterContext == null) {
                    throw new ArgumentNullException("filterContext");
                }

                var httpContext = filterContext.HttpContext;
                var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
            }
        }

        [Route("Processo_menu")]
        [HttpGet]
        public ActionResult Processo_menu() {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Processo_tp")]
        [HttpGet]
        public ActionResult Processo_tp() {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");
            Processo2ViewModel model = new Processo2ViewModel();

            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> ListaCC = processoRepository.Lista_Local(true,false);
            ViewBag.Lista_CCusto = new SelectList(ListaCC, "Codigo", "Descricao");

            return View(model);
        }

        [Route("Processo_tp")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Processo_tp(Processo2ViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _func = Session["hashfunc"].ToString() == "S" ? true : false;

            Processo_bll processoRepository = new Processo_bll(_connection);
            string _guid= Guid.NewGuid().ToString("N");
            Processo_web reg = new Processo_web {
                Guid = _guid,
                Centro_custo_codigo = model.Centro_Custo_Codigo,
                Centro_custo_nome = model.Centro_Custo_Nome,
                Data_geracao = DateTime.Now,
                Interno = model.Tipo_Requerente == "Prefeitura" ? true : false,
                User_id = _userId,
                User_pref = _func,
                Fisico=false,
                Assunto_codigo=0
            };

            Exception ex = processoRepository.Incluir_Processo_Web(reg);

            return RedirectToAction("Processo_add",new {p=_guid });
        }

        [HttpGet]
        public JsonResult Lista_Cidadao(string codigo, string nome, string cpfcnpj) {
            if (string.IsNullOrEmpty(codigo)) codigo = "0";
            int _cod = Convert.ToInt32(Functions.RetornaNumero(codigo));
            string _nome = nome.Trim() ?? "";
            string _cpfcnpj = Functions.RetornaNumero(cpfcnpj) ?? "";

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Cidadao> Lista = cidadaoRepository.Lista_Cidadao(_cod, _nome, _cpfcnpj,15);

            List<Cidadao> ObjCid = new List<Cidadao>();
            foreach (Cidadao cid in Lista) {
                if (string.IsNullOrEmpty(cid.Cnpj) && cid.Cnpj!="0")
                    _cpfcnpj = cid.Cpf;
                else
                    _cpfcnpj = cid.Cnpj;

                Cidadao reg = new Cidadao() {
                    Codcidadao = cid.Codcidadao,
                    Nomecidadao = Functions.TruncateTo( cid.Nomecidadao.ToUpper(),45),
                    Cpf = Functions.FormatarCpfCnpj( _cpfcnpj)
                };
                ObjCid.Add(reg);
            }

            return Json(ObjCid, JsonRequestBehavior.AllowGet);
        }

        [Route("Processo_add")]
        [HttpGet]
        public ActionResult Processo_add(string p) {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }

            Processo_bll processoRepository = new Processo_bll(_connection);
            Processo_web _proc = processoRepository.Retorna_Processo_Web(p);

            if (_proc == null) {
                return RedirectToAction("Processo_tp", "Processo");
            }

            List<Assunto> ListaAssunto= processoRepository.Lista_Assunto(true, false, "");
            ViewBag.Lista_Assunto= new SelectList(ListaAssunto, "Codigo", "Nome");

            Processo2ViewModel model = new Processo2ViewModel();
            model.Guid = p;
            model.Centro_Custo_Nome = _proc.Centro_custo_nome;
            model.Centro_Custo_Codigo = _proc.Centro_custo_codigo;
            model.Tipo_Requerente = _proc.Interno ? "Prefeitura" : "Contribuinte";
            return View(model);
        }
      
        public JsonResult Lista_Assunto(string search) {
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Assunto> Lista_Search = processoRepository.Lista_Assunto(true, false, search);
            return new JsonResult { Data = Lista_Search,JsonRequestBehavior= JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Doc(string assunto) {
            short _codAss = Convert.ToInt16(assunto);
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<AssuntoDocStruct> Lista_Search = processoRepository.Lista_Assunto_Documento(_codAss);
            return new JsonResult { Data = Lista_Search, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateJsonAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Processo_addx(List<Processo2ViewModel> dados) {
            if (dados[0].Assunto_Codigo == 0) {
                return Json(new { success = false, responseText = "Selecione um assunto válido." }, JsonRequestBehavior.AllowGet);
            }

            Processo_bll processoRepository = new Processo_bll(_connection);
            int _numero = processoRepository.Retorna_Numero_Disponivel(DateTime.Now.Year);
            int _user = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));
            bool _isFunc =Functions.Decrypt( Request.Cookies["2FN*"].Value) == "S" ? true : false;
            short _tipoRequerente = dados[0].Tipo_Requerente == "Prefeitura" ? (short)1 : (short)2;

            Processogti reg = new Processogti() {
                Ano = (short)DateTime.Now.Year,
                Numero=_numero,
                Fisico = dados[0].Fisico,
                Origem = 1,
                Interno=_tipoRequerente==1,
                Codassunto =(short) dados[0].Assunto_Codigo,
                Observacao=dados[0].Observacao,
                Dataentrada=DateTime.Now.Date,
                Userid=_user,
                Userweb=!_isFunc,
                Complemento=dados[0].Complemento,
                Etiqueta=false,
                Hora= DateTime.Now.ToShortTimeString().ToString()
            };
            if (_tipoRequerente == 2) {
                reg.Codcidadao = dados[0].Centro_Custo_Codigo;
                reg.Centrocusto = 0;
            } else {
                reg.Codcidadao = 0;
                reg.Centrocusto = dados[0].Centro_Custo_Codigo;
            }

            Exception ex = processoRepository.Incluir_Processo(reg);







            //foreach (TableEndereco _end in Lista_End) {
            //    reg += _end.Endereco;
            //}

            return Json(new { success = true, responseText = "Processo gravado com sucesso!" }, JsonRequestBehavior.AllowGet);
        }


    }
}