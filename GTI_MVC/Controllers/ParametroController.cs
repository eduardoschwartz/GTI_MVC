﻿using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers
{
    public class ParametroController : Controller    {
        private readonly string _connection = "GTIconnection";

        #region Cadastro de Bairros e Cidades
        [Route("Bairro_Edit")]
        [HttpGet]
        public ActionResult Bairro_Edit()
        {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Uf> listaUf = enderecoRepository.Lista_UF();
            ViewBag.ListaUF = new SelectList(listaUf, "siglauf", "descuf");
            return View();
        }

        [Route("Cidade_Edit")]
        [HttpGet]
        public ActionResult Cidade_Edit() {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Uf> listaUf = enderecoRepository.Lista_UF();
            ViewBag.ListaUF = new SelectList(listaUf, "siglauf", "descuf");
            return View();
        }

        public JsonResult Lista_Cidade(string uf) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Cidade> Lista = enderecoRepository.Lista_Cidade(uf);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Bairro(string uf,string cidade) {
            if (cidade == "") return null;
            int _cidade = Convert.ToInt32(cidade);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Bairro> Lista = enderecoRepository.Lista_Bairro(uf,_cidade);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Incluir_Bairro(string uf,string cidade,string bairro) {
            short _cidade = Convert.ToInt16(cidade);
            bairro = bairro.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            bool existeBairro = enderecoRepository.Existe_Bairro(uf, _cidade, bairro);
            if (existeBairro) {
                var result = new { Bairro_Codigo = 0, Success = "False",Msg="Bairro já cadastrado!" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            Bairro reg = new Bairro() {
                Siglauf=uf,
                Codcidade=_cidade,
                Descbairro=bairro
            };
            int _codigo = enderecoRepository.Incluir_bairro(reg);

            var result2 = new  { Bairro_Codigo=(short)_codigo, Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
        }

        public JsonResult Alterar_Bairro(string uf, string cidade, string bairro,string novo_nome) {
            short _cidade = Convert.ToInt16(cidade);
            short _bairro = Convert.ToInt16(bairro);
            novo_nome =novo_nome.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            Bairro reg = new Bairro() {
                Siglauf = uf,
                Codcidade = _cidade,
                Codbairro=_bairro,
                Descbairro = novo_nome
            };
            Exception ex = enderecoRepository.Alterar_Bairro(reg);

            var result2 = new { Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Incluir_Cidade(string uf, string cidade) {
            cidade = cidade.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            bool existeCidade = enderecoRepository.Existe_Cidade(uf, cidade);
            if (existeCidade) {
                var result = new { Cidade_Codigo = 0, Success = "False", Msg = "Cidade já cadastrada!" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            Cidade reg = new Cidade() {
                Siglauf = uf,
                Desccidade = cidade
            };
            int _codigo = enderecoRepository.Incluir_cidade(reg);

            var result2 = new { Cidade_Codigo = (short)_codigo, Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult Alterar_Cidade(string uf, string cidade, string novo_nome) {
            short _cidade = Convert.ToInt16(cidade);
            novo_nome = novo_nome.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            Cidade reg = new Cidade() {
                Siglauf = uf,
                Codcidade = _cidade,
                Desccidade = novo_nome
            };
            Exception ex = enderecoRepository.Alterar_Cidade(reg);

            var result2 = new { Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Cadastro de Lançamentos e Tributos

        [Route("Lancamento_Edit")]
        [HttpGet]
        public ActionResult Lancamento_Edit() {
            LancTribViewModel model = new LancTribViewModel();
            return View(model);
        }

        public JsonResult Lista_Lancamento() {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            List<Lancamento> Lista = tributarioRepository.Lista_Lancamento();
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Incluir_Lancamento(string nomeC,string nomeR) {
            Lancamento reg = new Lancamento() {
                Descfull = nomeC.ToUpper(),
                Descreduz=nomeR.ToUpper()
            };
            
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Exception ex = tributarioRepository.Insert_Lancamento(reg);

            var result = new {  Success = "True" };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult Alterar_Lancamento(string codigo, string nomeC, string nomeR) {
            short _codigo = Convert.ToInt16(codigo);
            Lancamento reg = new Lancamento() {
                Codlancamento=_codigo,
                Descfull = nomeC.ToUpper(),
                Descreduz = nomeR.ToUpper()
            };
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Exception ex = tributarioRepository.Alterar_Lancamento(reg);

            var result = new { Success = "True" };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Route("Tributo_Edit")]
        [HttpGet]
        public ActionResult Tributo_Edit() {
            LancTribViewModel model = new LancTribViewModel();
            return View(model);
        }

        public JsonResult Lista_Tributo() {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            List<Tributo> Lista = tributarioRepository.Lista_Tributo();
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Incluir_Tributo(string nomeC, string nomeR) {
            Tributo reg = new Tributo() {
                Desctributo = nomeC.ToUpper(),
                Abrevtributo = nomeR.ToUpper()
            };

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Exception ex = tributarioRepository.Insert_Tributo(reg);

            var result = new { Success = "True" };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult Alterar_Tributo(string codigo, string nomeC, string nomeR) {
            short _codigo = Convert.ToInt16(codigo);
            Tributo reg = new Tributo() {
                Codtributo = _codigo,
                Desctributo = nomeC.ToUpper(),
                Abrevtributo = nomeR.ToUpper()
            };
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Exception ex = tributarioRepository.Alterar_Tributo(reg);

            var result = new { Success = "True" };
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        #endregion

    }
}