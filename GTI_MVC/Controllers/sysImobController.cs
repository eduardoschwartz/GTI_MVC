﻿using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers
{
    public class sysImobController : Controller   {
        private readonly string _connection = "GTIconnection";

        [HttpGet]
        [Route("imovel_data")]
        public ActionResult imovel_data(string cod) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            ViewBag.Codigo = cod;
            cod = Functions.RetornaNumero(cod);
            if (cod == "")  {
                ViewBag.Result = "Código inválido.";
                return View(model);
            }
            int _codigo = Convert.ToInt32(cod);
            bool existe = imovelRepository.Existe_Imovel(_codigo);
            if (existe) {
                model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);
                model.Lista_Proprietario = imovelRepository.Lista_Proprietario(_codigo, false);
                model.Lista_Areas = imovelRepository.Lista_Area(_codigo);
                model.Lista_Testada = imovelRepository.Lista_Testada(_codigo);
                if (model.ImovelStruct.EE_TipoEndereco != null) {
                    short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                    if (_tipoEE == 0)
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                    else {
                        if (_tipoEE == 1)
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Proprietario);
                        else
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                    }
                }
            }
            else
                ViewBag.Result = "Imóvel não cadastrado.";

            return View(model);

        }

        [Route("imovel_query")]
        [HttpGet]
        public ActionResult imovel_query(string id) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            model.Lista_Imovel = new List<ImovelLista>();
            return View(model);
        }

        [Route("imovel_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult imovel_query(ImovelDetailsViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            model.Lista_Imovel = new List<ImovelLista>();
            int _codigo = Convert.ToInt32(model.Codigo);
            string _partialName = model.NomeProprietario ?? "";
            if (_partialName != "") {
                if (_partialName.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do nome do proprietário.";
                    return View(model);
                }
            }
            string _partialEndereco = model.NomeEndereco ?? "";
            if (_partialEndereco != "") {
                if (_partialEndereco.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do endereço.";
                    return View(model);
                }
            }
            int _numero = model.Numero == null ? 0 : Convert.ToInt32(model.Numero);
            int _distrito = 0, _setor = 0, _quadra = 0, _lote = 0, _face = 0, _unidade = 0, _subunidade = 0;
            string _insc = model.Inscricao ?? "";

            if (_insc != "") {
                if (_insc.Length == 25) {
                    _insc = Functions.RetornaNumero(model.Inscricao);
                    _distrito = Convert.ToInt32(_insc.Substring(0, 1));
                    _setor = Convert.ToInt32(_insc.Substring(1, 2));
                    _quadra = Convert.ToInt32(_insc.Substring(3, 4));
                    _lote = Convert.ToInt32(_insc.Substring(7, 5));
                    _face = Convert.ToInt32(_insc.Substring(12, 2));
                    _unidade = Convert.ToInt32(_insc.Substring(14, 2));
                    _subunidade = Convert.ToInt32(_insc.Substring(16, 3));
                } else {
                    ViewBag.Result = "Inscrição cadastral inválida.";
                    return View(model);
                }
            }

            if (_codigo == 0 && _insc == "" && _partialEndereco == "" && _partialName == "") {
                ViewBag.Result = "Selecione ao menos um critério de busca.";
                return View(model);
            }

            List<ImovelStruct> ListaImovel = imovelRepository.Lista_Imovel(_codigo, _distrito, _setor, _quadra, _lote, _face, _unidade, _subunidade, _partialName, _partialEndereco, _numero);
            if (ListaImovel.Count == 0) {
                ViewBag.Result = "Não foi localizado nenhum imóvel com este(s) critério(s).";
                return View(model);
            }
            List<ImovelLista> _lista = new List<ImovelLista>();
            foreach (ImovelStruct item in ListaImovel) {
                ImovelLista reg = new ImovelLista() {
                    Codigo = item.Codigo.ToString("00000"),
                    Nome = Functions.TruncateTo(item.Proprietario_Nome, 30),
                    Endereco = string.IsNullOrEmpty(item.NomeLogradouroAbreviado) ? item.NomeLogradouro : item.NomeLogradouroAbreviado
                };
                reg.Endereco += ", " + item.Numero.ToString() + " " + item.Complemento;
                reg.Endereco = Functions.TruncateTo(reg.Endereco, 52);
                _lista.Add(reg);
            }

            model.Lista_Imovel = _lista;
            return View(model);

        }

        [HttpGet]
        [Route("imovel_edit")]
        public ActionResult imovel_edit(string cod)
        {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            ViewBag.Codigo = cod;
            cod = Functions.RetornaNumero(cod);
            if (cod == "")   {
                ViewBag.Result = "Código inválido.";
                return View(model);
            }
            int _codigo = Convert.ToInt32(cod);
            bool existe = imovelRepository.Existe_Imovel(_codigo);
            if (existe)
            {
                model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);
                model.Lista_Proprietario = imovelRepository.Lista_Proprietario(_codigo, false);
                model.Lista_Areas = imovelRepository.Lista_Area(_codigo);
                model.Lista_Testada = imovelRepository.Lista_Testada(_codigo);
                if (model.ImovelStruct.EE_TipoEndereco != null)
                {
                    short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                    if (_tipoEE == 0)
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                    else
                    {
                        if (_tipoEE == 1)
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Proprietario);
                        else
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                    }
                }
            }
            else
                ViewBag.Result = "Imóvel não cadastrado.";

            return View(model);

        }


    }
}