using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Web.Mvc;

namespace GTI_MVC.Controllers {
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

            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> ListaCC = processoRepository.Lista_Local(true,false);
            ViewBag.Lista_CCusto = new SelectList(ListaCC, "Codigo", "Descricao");

            return View(model);
        }

        [Route("Processo_tp")]
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
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Processo_bll processoRepository = new Processo_bll(_connection);
            Processo_web _proc = processoRepository.Retorna_Processo_Web(p);

            List<Assunto> ListaAssunto= processoRepository.Lista_Assunto(true, false, "");
            ViewBag.Lista_Assunto= new SelectList(ListaAssunto, "Codigo", "Nome");

            Processo2ViewModel model = new Processo2ViewModel();
            model.Guid = p;
            model.Centro_Custo_Nome = _proc.Centro_custo_nome;
            model.Centro_Custo_Codigo = _proc.Centro_custo_codigo;
            model.Tipo_Requerente = _proc.Interno ? "Prefeitura" : "Contribuinte";
            return View(model);
        }

        //[Route("Processo_add")]
        //[HttpPost]
        //public ActionResult Processo_add(Processo2ViewModel model) {
        //    if (Session["hashid"] == null)
        //        return RedirectToAction("Login", "Home");

        //    if (model.Assunto_Codigo == 0) {
        //        ViewBag.Result = "Selecione um assunto válido!";
        //        return View(model);
        //    }

        //    Processo_bll processoRepository = new Processo_bll(_connection);

        //    return View(model);
        //}

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


        public JsonResult Processo_addx(List<Processo2ViewModel> dados) {
            string reg = "";
            //foreach (TableEndereco _end in Lista_End) {
            //    reg += _end.Endereco;
            //}
            return Json(reg);
        }


    }
}