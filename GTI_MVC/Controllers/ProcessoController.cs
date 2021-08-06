﻿using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
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

            Processo_bll processoRepository = new Processo_bll(_connection);
            string _guid= Guid.NewGuid().ToString("N");
            Processo_web reg = new Processo_web {
                Guid = _guid,
                Centro_custo_codigo=model.Centro_Custo_Codigo,
                Centro_custo_nome=model.Centro_Custo_Nome,
                Data_geracao=DateTime.Now,
                Interno=model.Tipo_Requerente=="Prefeitura"?true:false
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
            Processo2ViewModel model = new Processo2ViewModel();
            model.Centro_Custo_Nome = _proc.Centro_custo_nome;

            return View(model);
        }


    }
}