using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static GTI_Models.modelCore;
using System.Net;
using Newtonsoft.Json.Linq;
using GTI_Mvc;

namespace GTI_MVC.Controllers
{
    public class ParcelamentoController : Controller
    {
        [Route("Parc_index")]
        [HttpGet]
        public ActionResult Parc_index()
        {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Parc_req")]
        [HttpGet]
        public ActionResult Parc_req() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
            Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");

            int _user_id = Convert.ToInt32(Session["hashid"]);
            Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(_user_id);
                        
            string _cpfcnpj = _user.Cpf_Cnpj;
            int _codigo;
            bool _bCpf = false;
            if (_cpfcnpj.Length == 11) {
                _codigo = cidadaoRepository.Existe_Cidadao_Cpf(_cpfcnpj);
                _bCpf = true;
            } else {
                _codigo = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfcnpj);
            }

            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            Parc_Requerente _req = new Parc_Requerente {
                Nome=_cidadao.Nome,
                Email=_user.Email
            };
            _req.Cpf_Cnpj = Functions.FormatarCpfCnpj(_cpfcnpj);
            string _tipoEnd = _cidadao.EnderecoC == "S" ? "C" : "R";
            _req.TipoEnd = _tipoEnd == "R" ? "RESIDENCIAL" : "COMERCIAL";
            if (_tipoEnd == "R") {
                _req.Bairro_Codigo = (int)_cidadao.CodigoBairroR;
                _req.Bairro_Nome = _cidadao.NomeBairroR;
                _req.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                _req.Cidade_Nome = _cidadao.NomeCidadeR;
                _req.UF = _cidadao.UfR;
                _req.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroR;
                _req.Logradouro_Nome = _cidadao.EnderecoR;
                _req.Numero = (int)_cidadao.NumeroR;
                _req.Complemento = _cidadao.ComplementoR;
                _req.Telefone = _cidadao.TelefoneR;
                _req.Cep = _cidadao.CepR.ToString();
            } else {
                _req.Bairro_Codigo = (int)_cidadao.CodigoBairroC;
                _req.Bairro_Nome = _cidadao.NomeBairroC;
                _req.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                _req.Cidade_Nome = _cidadao.NomeCidadeC;
                _req.UF = _cidadao.UfC;
                _req.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroC;
                _req.Logradouro_Nome = _cidadao.EnderecoC;
                _req.Numero = (int)_cidadao.NumeroC;
                _req.Complemento = _cidadao.ComplementoC;
                _req.Telefone = _cidadao.TelefoneC;
                _req.Cep = _cidadao.CepC.ToString();
            }
            if (_req.Logradouro_Codigo> 0) {
                int nCep = enderecoRepository.RetornaCep(Convert.ToInt32(_req.Logradouro_Codigo), (short)_req.Numero);
                _req.Cep = nCep.ToString("00000-000");
            }

            ParcelamentoViewModel model = new ParcelamentoViewModel();
            model.Requerente = _req;
            List<SelectListItem> _listaCodigo = new List<SelectListItem>();

            //Lista de imóvel
            List<int> _listaImovel = null;
            if (_bCpf)
                _listaImovel = imovelRepository.Lista_Imovel_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaImovel = imovelRepository.Lista_Imovel_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            SelectListGroup _grupo = new SelectListGroup() { Name = "Imovel" };
            foreach (int cod in _listaImovel) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = cod.ToString() + " - Imóvel localizado na(o) " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            //Lista de empresas
            List<int> _listaEmpresa = null;
            if (_bCpf)
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            _grupo = new SelectListGroup() { Name = "Empresa" };
            foreach (int cod in _listaEmpresa) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = cod.ToString() + " - " + _header.Nome +  ", localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            //Lista de cidadão
            List<Cidadao> _listaCidadao = null;
            if (_bCpf)
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null, Functions.RetornaNumero( _req.Cpf_Cnpj),null);
            else
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null,null, Functions.RetornaNumero(_req.Cpf_Cnpj));

            _grupo = new SelectListGroup() { Name = "Cidadao" };
            foreach (Cidadao cod in _listaCidadao) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod.Codcidadao);
                string _desc = cod.Codcidadao.ToString() + " - Inscrição localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.Codcidadao.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            model.Lista_Codigos = _listaCodigo;

            return View(model);
        }

        [Route("Parc_req")]
        [HttpPost]
        public ActionResult Parc_req(ParcelamentoViewModel model,string listacod) {
            int _codigo = Convert.ToInt32(listacod);

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll("GTIconenction");
            Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(_codigo);
            char _tipo = _header.Cpf_cnpj.Length == 11 ? 'F' : 'J';

            List<SpParcelamentoOrigem> Lista_Origem = parcelamentoRepository.Lista_Parcelamento_Origem(_codigo, _tipo);
            if (Lista_Origem.Count == 0) {
                ViewBag.Result = "Não existem débitos a serem parcelados para esta inscrição.";
                return View(model);
            }

            return View(model);
        }
















    }
}