using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_MVC.Controllers
{
    public class CidadaoController : Controller   {
        private readonly string _connection = "GTIconnectionTeste";

        [Route("Cidadao_menu")]
        [HttpGet]
        public ActionResult Cidadao_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Cidadao_add")]
        [HttpGet]
        public ActionResult Cidadao_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");
            model.EnderecoR = new EnderecoStruct();
            model.EnderecoC = new EnderecoStruct();

            return View(model);
        }

        [Route("Cidadao_add")]
        [HttpPost]
        public ActionResult Cidadao_add(CidadaoViewModel model, string action) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            if (action == "btnCepR") {
                int _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoR.Cep));
                var cepObj = GTI_Mvc.Classes.Cep.Busca_CepDB(_cep);
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                    List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                    List<Logradouro> Lista_Logradouro = new List<Logradouro>();
                    int s = 1;
                    foreach (string item in Lista_Tmp) {
                        Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                        s++;
                    }
                    ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


                    Bairro bairro = enderecoRepository.Retorna_CepDB_Bairro(_cep);
                    if (bairro != null) {
                        model.EnderecoR.CodigoBairro = bairro.Codbairro;
                        model.EnderecoR.NomeBairro = bairro.Descbairro;
                    }
                    Cidade cidade = enderecoRepository.Retorna_CepDB_Cidade(_cep);
                    if (cidade != null) {
                        model.EnderecoR.CodigoCidade = cidade.Codcidade;
                        model.EnderecoR.NomeCidade = cidade.Desccidade;
                    }
                    model.EnderecoR.UF = cepObj.Estado;

                } else {
                    model.EnderecoR.CodLogradouro = 0;
                    model.EnderecoR.Endereco = "";
                    model.EnderecoR.CodigoBairro = 0;
                    model.EnderecoR.NomeBairro = "";
                    model.EnderecoR.CodigoCidade = 0;
                    model.EnderecoR.NomeCidade = "";
                    model.EnderecoR.Numero = 0;
                    model.EnderecoR.Complemento = "";
                    model.EnderecoR.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                    return View(model);
                }
            }



            return View(model);
        }


    }
}