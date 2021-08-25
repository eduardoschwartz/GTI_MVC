using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using GTI_Mvc.Classes;
using System.Web.Script.Serialization;
using Microsoft.Ajax.Utilities;
using Cep = GTI_Mvc.Classes.Cep;

namespace GTI_MVC.Controllers {
    public class SharedController : Controller {
        private readonly string _connection = "GTIconnection";
        [Route("Checkgticd")]
        [HttpGet]
        public ActionResult Checkgticd(string c)
        {
            ViewBag.c = c;
            return View();
        }

        [Route("Checkgticd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkgticd(CertidaoViewModel model, string c) {
            int _codigo, _ano, _numero;
            string _tipo, _chave = c,_pdfFileName="";
            if (c != null) {
                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
                Certidao reg = new Certidao();
                List<Certidao> certidao = new List<Certidao>();
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                _codigo = _chaveStruct.Codigo;
                _numero = _chaveStruct.Numero;
                _ano = _chaveStruct.Ano;
                _tipo = _chaveStruct.Tipo;
                if (_numero == 0)
                    return null;
                ReportDocument rd = new ReportDocument();
                switch (_tipo) {
                    case "EA": {
                            //####################Certidão endereço####################################
                            Certidao_endereco certidaoGerada = tributarioRepository.Retorna_Certidao_Endereco(_ano, _numero, _codigo);
                            reg.Codigo = _codigo;
                            reg.Ano = _ano;
                            reg.Numero = _numero;
                            reg.Endereco = certidaoGerada.Logradouro;
                            reg.Endereco_Numero = certidaoGerada.Li_num;
                            reg.Endereco_Complemento = certidaoGerada.Li_compl ?? "";
                            reg.Bairro = certidaoGerada.descbairro;
                            reg.Nome_Requerente = certidaoGerada.Nomecidadao;
                            reg.Data_Geracao = certidaoGerada.Data;
                            reg.Inscricao = certidaoGerada.Inscricao;
                            reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
                            certidao.Add(reg);
                            
                            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Endereco_Valida.rpt"));
                            rd.SetDataSource(certidao);
                            _pdfFileName= "Certidao_Endereco.pdf";
                            break;
                        }
                    //#################################################################################
                    case "IN": case "IP":  case "IS":  case "CN":  case "CP": case "PN": {
                            //##########################Certidão débito####################################
                            Certidao_debito_doc _dados = tributarioRepository.Retorna_Certidao_Debito_Doc(_chave);
                            Certidao regdeb = new Certidao() {
                                Codigo = _codigo,
                                Nome_Requerente = _dados.Nome,
                                Ano = _ano,
                                Numero = _numero,
                                Numero_Ano = _dados.Numero.ToString("00000") + "/" + _dados.Ano.ToString(),
                                Controle = _chave,
                                Tributo = _dados.Ret==3?"N/A": _dados.Tributo,
                                Cpf_Cnpj = _dados.Cpf_cnpj,
                                Data_Geracao = _dados.Data_emissao,
                                Tipo_Certidao = _dados.Ret == 3 ? "Negativa" : _dados.Ret == 4 ? "Positiva" : "Positiva com Efeito Negativa"
                            };
                            certidao.Add(regdeb);
                            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/CertidaoDebitoDocValida.rpt"));
                            rd.SetDataSource(certidao);
                            _pdfFileName = "Certidao_Debito.pdf";
                            break;
                        }
                    //#################################################################################
                    case "AF": case "AN": {
                            //#########################Alvará de Funcionamento#############################
                            _codigo = _chaveStruct.Codigo;
                            _numero = _chaveStruct.Numero;
                            _ano = _chaveStruct.Ano;
                            Empresa_bll empresaRepository = new Empresa_bll(_connection);
                            Alvara_funcionamento _dadosalvara = empresaRepository.Alvara_Funcionamento_gravado(_chave);
                            if (_dadosalvara != null) {
                                Certidao regAlvara = new Certidao() {
                                    Codigo = _codigo,
                                    Razao_Social = _dadosalvara.Razao_social,
                                    Endereco = _dadosalvara.Endereco + ", " + _dadosalvara.Numero,
                                    Bairro = _dadosalvara.Bairro ?? "",
                                    Ano = _ano,
                                    Numero = _numero,
                                    Controle = _chave,
                                    Atividade_Extenso = _dadosalvara.Atividade,
                                    Cpf_Cnpj = _dadosalvara.Documento,
                                    Horario = _dadosalvara.Horario
                                };
                                certidao.Add(regAlvara);
                            }
                            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Alvara_Funcionamento_Valida.rpt"));
                            rd.SetDataSource(certidao);
                            _pdfFileName = "AlvaraValida.pdf";
                            break;
                        }
                    //#################################################################################
                    case "IE": case "XE": case "XA":
                        break;
                    case "CI":
                        break;
                    case "VV": {
                            //#########################Certidão Valor Venal############################
                            Certidao_valor_venal certidaoGerada = tributarioRepository.Retorna_Certidao_ValorVenal(_ano, _numero, _codigo);
                            if (certidaoGerada != null) {
                                reg.Codigo = _codigo;
                                reg.Ano = _ano;
                                reg.Numero = _numero;
                                reg.Endereco = certidaoGerada.Logradouro;
                                reg.Endereco_Numero = certidaoGerada.Li_num;
                                reg.Endereco_Complemento = certidaoGerada.Li_compl ?? "";
                                reg.Bairro = certidaoGerada.Descbairro;
                                reg.Nome_Requerente = certidaoGerada.Nomecidadao;
                                reg.Data_Geracao = certidaoGerada.Data;
                                reg.Inscricao = certidaoGerada.Inscricao;
                                reg.Area = certidaoGerada.Areaterreno;
                                reg.VVT = certidaoGerada.Vvt;
                                reg.VVP = certidaoGerada.Vvp;
                                reg.VVI = certidaoGerada.Vvi;
                                reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
                            } else {
                                ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                                return View("Certidao_Valor_Venal", model);
                            }

                            certidao.Add(reg);
                            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Valor_venal_Valida.rpt"));
                            rd.SetDataSource(certidao);
                            _pdfFileName = "Certidao_ValorVenal.pdf";
                            break;
                        }
                    //#################################################################################
                    default:
                        break;
                }

                try {
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", _pdfFileName);
                } catch {
                    throw;
                }

            }

            return null;

        }

        [Route("Checkguid")]
        [HttpGet]
        public ActionResult Checkguid(string c,string p) {
            if (string.IsNullOrEmpty(c))
                c = p;
            ViewBag.c = c;
            return View();
        }


        [Route("Checkguid")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkguid(CertidaoViewModel model, string c) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Itbi_isencao_main_Struct _itbi = imovelRepository.Retorna_Itbi_Isencao_Main(c);
            List<Itbi_isencao_imovel> _Lista = imovelRepository.Retorna_Itbi_Isencao_Imovel(c);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Isencao_Itbi_Valida.rpt"));
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
            foreach (Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Assinatura assinatura = sistemaRepository.Retorna_Usuario_Assinatura(_itbi.Fiscal_id);
            usuarioStruct usuario = sistemaRepository.Retorna_Usuario(_itbi.Fiscal_id);
            rd.SetParameterValue("ANONUMERO", _itbi.Isencao_numero.ToString("00000") + "/" + _itbi.Isencao_ano.ToString("0000"));
            rd.SetParameterValue("NATUREZA", _itbi.Natureza_Nome);
            rd.SetParameterValue("NOMEFISCAL", usuario.Nome_completo);
            rd.SetParameterValue("CARGO", assinatura.Cargo);

            string imovel = "";
            foreach (Itbi_isencao_imovel item in _Lista) {
                imovel += item.Descricao + ", ";
            }
            imovel = imovel.Substring(0, imovel.Length - 2);
            rd.SetParameterValue("IMOVEL", imovel);
            try {
                rd.RecordSelectionFormula = "{itbi_isencao_main.guid}='" + c + "'";
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Isencao_Itbi_Valida.pdf");
            } catch {
                throw;
            }

        }

        [Route("Cep_inc")]
        [HttpGet]
        public ActionResult Cep_inc() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            CepViewModel model = new CepViewModel();
            return View(model);
        }

        [Route("Cep_inc")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cep_inc(CepViewModel model, string action) {
            ViewBag.Error = "";
            if (action == "btnCepOK") {
                if (model.Cep == null || model.Cep.Length < 9) {
                    ViewBag.Error = "* Cep digitado inválido.";
                    return View(model);
                }
            }
            if (action == "btnCepCancel") {
                model = new CepViewModel();
                return View(model);
            }

            //if (model.Cidade_Codigo == 0) {
            //    model.Bairro_Codigo_New = 0;
            //    model.Bairro_Nome_New = "";
            //}

            int _cep = Convert.ToInt32(Functions.RetornaNumero(model.Cep));
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);


            List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            int s = 1;
            foreach (string item in Lista_Tmp) {
                Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                s++;
            }
            ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


            Cepdb _cepdb = null;
            if(model.Logradouro!=null)
                _cepdb = enderecoRepository.Retorna_CepDB(_cep, Lista_Logradouro[Convert.ToInt32(model.Logradouro) - 1].Endereco);
            else
                _cepdb = enderecoRepository.Retorna_CepDB(_cep);

            if (_cepdb != null) {
                model.Uf = _cepdb.Uf;
                model.NomeUf = enderecoRepository.Retorna_UfNome(_cepdb.Uf);
                if (_cepdb.Cidadecodigo > 0) {
                    model.Cidade_Codigo = _cepdb.Cidadecodigo;
                    model.Bairro_Codigo = _cepdb.Bairrocodigo;
                    model.Logradouro = _cepdb.Logradouro.ToUpper();
                    model.Bairro_Nome=   enderecoRepository.Retorna_Bairro(_cepdb.Uf, _cepdb.Cidadecodigo, _cepdb.Bairrocodigo);
                    model.Cidade_Nome = enderecoRepository.Retorna_Cidade(_cepdb.Uf, _cepdb.Cidadecodigo);
                }
            }

            Uf _uf = enderecoRepository.Retorna_Cep_Estado(_cep);
            if (_uf == null) {
                ViewBag.Error = "* Cep não existente.";
                model.Uf = "";
                model.NomeUf = "";
                return View(model);
            }

            List<Cidade> Lista_Cidade = enderecoRepository.Lista_Cidade(_uf.Siglauf);
            ViewBag.Cidade = new SelectList(Lista_Cidade, "Codcidade", "Desccidade");

            List<Bairro> Lista_Bairro_New = null;
            if (model.Cidade_Codigo_New > 0) {
                Lista_Bairro_New = enderecoRepository.Lista_Bairro(_uf.Siglauf, model.Cidade_Codigo_New);
                ViewBag.Bairro_New = new SelectList(Lista_Bairro_New, "Codbairro", "Descbairro");
            } else {
                if (model.Cidade_Codigo > 0) {
                    Lista_Bairro_New = enderecoRepository.Lista_Bairro(_uf.Siglauf, model.Cidade_Codigo);
                    ViewBag.Bairro_New = new SelectList(Lista_Bairro_New, "Codbairro", "Descbairro");
                } else {
                    Lista_Bairro_New = new List<Bairro>();
                    ViewBag.Bairro_New = new SelectList(Lista_Bairro_New, "Codbairro", "Descbairro");
                }
            }

            //if (!string.IsNullOrEmpty(model.Bairro_Nome_New)) {
            //    string _bairronew = model.Bairro_Nome_New.ToUpper();
            //    foreach (Bairro item in Lista_Bairro_New) {
            //        if (item.Descbairro.ToUpper() == model.Bairro_Nome_New.ToUpper()) {
            //            ViewBag.Error = "Bairro já cadastrado.";
            //            return View(model);
            //        }
            //    }
            //}

            int _cidade = model.Cidade_Codigo > 0 ? model.Cidade_Codigo : Lista_Cidade[0].Codcidade;
            List<Bairro> Lista_Bairro = enderecoRepository.Lista_Bairro(_uf.Siglauf,model.Cidade_Codigo);
            ViewBag.Bairro = new SelectList(Lista_Bairro, "Codbairro", "Descbairro");

            model.Uf = _uf.Siglauf;
            model.NomeUf = _uf.Descuf;



            if (action == "btnValida") {
                if(model.Logradouro_New==null || model.Logradouro_New.Trim() == "") {
                    ViewBag.Error = "Digite o nome do logradouro.";
                    return View(model);
                }
                if (model.Bairro_Codigo_New == 0) {
                    ViewBag.Error = "Selecione o bairro.";
                    return View(model);
                }

                foreach (Logradouro rua in Lista_Logradouro) {
                    if (model.Logradouro_New.ToUpper() == rua.Endereco) {
                        ViewBag.Error = "Logradouro já cadastrado para este Cep.";
                        return View(model);
                    }
                }

                //Grava o novo Cep

                Cepdb _reg = new Cepdb() {
                    Cep=_cep.ToString("00000000"),
                    Uf=model.Uf,
                    Cidadecodigo=model.Cidade_Codigo_New==0?model.Cidade_Codigo:model.Cidade_Codigo_New,
                    Bairrocodigo=model.Bairro_Codigo_New,
                    Logradouro=model.Logradouro_New.ToUpper(),
                    Func = Session["hashfunc"].ToString() == "S",
                    Userid= Convert.ToInt32(Session["hashid"])
                };
                Exception ex = enderecoRepository.Incluir_CepDB(_reg);
                model = new CepViewModel();
                return View(model);
            }

            if (model.Bairro_Nome_New != null && model.Bairro_Nome_New != "") {
                Bairro _bairro = new Bairro() {
                    Siglauf = model.Uf,
                    Codcidade = (short)model.Cidade_Codigo_New==0?(short)_cidade:(short)model.Cidade_Codigo_New,
                    Descbairro = model.Bairro_Nome_New.ToUpper()
                };
                model.Bairro_Codigo_New = enderecoRepository.Incluir_bairro(_bairro);
                model.Bairro_Nome_New = "";
                ViewBag.Error = "";
                Lista_Bairro_New = enderecoRepository.Lista_Bairro(model.Uf, model.Cidade_Codigo_New);
                ViewBag.Bairro_New = new SelectList(Lista_Bairro_New, "Codbairro", "Descbairro");
                return View(model);
            }


            return View(model);
        }

        public JsonResult Lista_Endereco(string search) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Logradouro> Lista_Search = enderecoRepository.Lista_Logradouro( search);
            return new JsonResult { Data = Lista_Search, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Route("Pagto_pix")]
        [HttpGet]
        public ActionResult Pagto_Pix(string c, string p) {
            return View();
        }

        [Route("Pagto_pix")]
        [HttpPost]
        public ActionResult Pagto_Pix() {
            //***********Geração do Token****************
            var client = new RestClient("https://oauth.hm.bb.com.br/oauth/token?gw-dev-app-key=d27b67790cffab50136be17db0050c56b9d1a5b1");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic ZXlKcFpDSTZJalV5TnpoaE16WXRPVEJrTUMwME9UUXhMV0l4WXpVdE1pSXNJbU52WkdsbmIxQjFZbXhwWTJGa2IzSWlPakFzSW1OdlpHbG5iMU52Wm5SM1lYSmxJam95TURjMk5pd2ljMlZ4ZFdWdVkybGhiRWx1YzNSaGJHRmpZVzhpT2pGOTpleUpwWkNJNklqaGhZbVZqTUNJc0ltTnZaR2xuYjFCMVlteHBZMkZrYjNJaU9qQXNJbU52WkdsbmIxTnZablIzWVhKbElqb3lNRGMyTml3aWMyVnhkV1Z1WTJsaGJFbHVjM1JoYkdGallXOGlPakVzSW5ObGNYVmxibU5wWVd4RGNtVmtaVzVqYVdGc0lqb3hMQ0poYldKcFpXNTBaU0k2SW1odmJXOXNiMmRoWTJGdklpd2lhV0YwSWpveE5qSTVNRE0xTlRneE9UY3dmUQ==");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "cobrancas.boletos-info cobrancas.boletos-requisicao");
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            IRestResponse response = client.Execute(request);

            dynamic responseContent = JsonConvert.DeserializeObject(response.Content);
            string _token = "";
            foreach (dynamic prop in responseContent) {
                string _name = prop.Name.ToString();
                string _value = prop.Value.ToString();
                if (_name == "access_token") {
                    _token = _value;  //<============ Token Gerado
                    break;
                }
            }
            if (_token == "") {//<========= Se o token for inválido retorna erro
                return View();
            }

            //**********************************************
            //***********Geração da Cobrança****************
            //**********************************************

            //****Campos variaveis****
            int _numeroConvenio = 3128557;
            string _dataEmissao = DateTime.Now.ToString("dd.MM.yyyy");
            string _dataVencimento = Convert.ToDateTime("25/08/2021").ToString("dd.MM.yyyy");
            double _valorOriginal = Math.Round(235.42, 2);
            string _numeroTituloBeneficiario = "17345783";
            string _campoUtilizacaoBeneficiario = Functions.RemoveDiacritics("UMA OBSERVAÇÃO");
            string _numeroTituloCliente = "000" + _numeroConvenio.ToString() + "00" + _numeroTituloBeneficiario.ToString();
            string _mensagemBloquetoOcorrencia = Functions.RemoveDiacritics("OUTRO TEXTO");
            int _tipoInscricao = 1; //(1-Cpf,2-Cnpj)
            long _numeroInscricao = 96050176876;
            string _nome= "VALERIO DE AGUIAR ZORZATO";
            string _endereco = "AVENIDA DIAS GOMES 1970";
            int _cep = 77458000;
            string _cidade = "SUCUPIRA";
            string _bairro = "CENTRO";
            string _uf = "TO";
            string _telefone = "63987654321";
             //************************

            client = new RestClient("https://api.hm.bb.com.br/cobrancas/v2/boletos?gw-dev-app-key=d27b67790cffab50136be17db0050c56b9d1a5b1");
            client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + _token); //<===== Informar o token gerado
            request.AddHeader("Content-Type", "application/json");

            var obj = new Cobranca {
                numeroConvenio = _numeroConvenio,
                numeroCarteira = 17,
                numeroVariacaoCarteira=35,
                codigoModalidade=1,
                dataEmissao=_dataEmissao,
                dataVencimento=_dataVencimento,
                valorOriginal=_valorOriginal,
                valorAbatimento=0,
                codigoAceite="N",
                codigoTipoTitulo=2,
                indicadorPermissaoRecebimentoParcial="N",
                numeroTituloBeneficiario=_numeroTituloBeneficiario,
                campoUtilizacaoBeneficiario=_campoUtilizacaoBeneficiario,
                numeroTituloCliente= _numeroTituloCliente,
                mensagemBloquetoOcorrencia = _mensagemBloquetoOcorrencia,
                pagador = new CobrancaPagador {
                    tipoInscricao = _tipoInscricao,
                    numeroInscricao = _numeroInscricao,
                    nome = _nome,
                    endereco= _endereco,
                    cep= _cep,
                    cidade=_cidade,
                    bairro=_bairro,
                    uf=_uf,
                    telefone=_telefone
                },
                indicadorPix="S"
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            request.AddParameter("application/json", json, RestSharp.ParameterType.RequestBody);
            IRestResponse response2 = client.Execute(request);

            string _linhaDigitavel = "",_codigoBarraNumerico="",_qrCode="",_numero="",_url="",_txId="",_emv="";
            responseContent = JsonConvert.DeserializeObject(response2.Content);
            foreach (dynamic prop in responseContent) {
                string _name = prop.Name.ToString();
                string _value = prop.Value.ToString();
                if (_name == "linhaDigitavel") {
                    _linhaDigitavel = _value;
                }
                if (_name == "codigoBarraNumerico") {
                    _codigoBarraNumerico = _value;
                }
                if (_name == "numero") {
                    _numero = _value;
                }
                if (_name == "qrCode") {
                    _qrCode = _value;
                    dynamic responseContent2 = JsonConvert.DeserializeObject(_qrCode);
                    foreach (dynamic prop2 in responseContent2) {
                        _name = prop2.Name.ToString();
                        _value = prop2.Value.ToString();
                        if (_name == "url") {
                            _url = _value;
                        }
                        if (_name == "txId") {
                            _txId = _value;
                        }
                        if (_name == "emv") {
                            _emv = _value;
                        }
                    }
                }
            }
            if (_numero == "") {
                return View();
            }


            return View();
        }


        [Route("ViaCep")]
        [HttpGet]
        public ActionResult ViaCep() {
            return View();
        }

        [Route("ViaCep")]
        [HttpPost]
        public ActionResult ViaCep(string cep) {
            string cep_new = Functions.RetornaNumero(cep);
            Cep _cep = GTI_Mvc.Classes.Cep.BuscaViaCep(cep_new);
            
            if(_cep==null)
                ViewBag.Result = "Cep não Localizado";
            else {
                if(_cep.Endereco!="")
                    ViewBag.Result = _cep.Endereco;
                else
                    ViewBag.Result = "Cep não Localizado";
            }
            return View();
        }

    }
}