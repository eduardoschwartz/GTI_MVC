﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using Microsoft.Reporting.WebForms;
using MMLib.Extensions;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class ItbiController : Controller {
        private readonly string _connection = "GTIconnection";
        #region Emissão de Itbi

        [Route("Itbi_menu")]
        [HttpGet]
        public ActionResult Itbi_menu() {
            Session["hashform"] = "itbi";
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            return View();
        }

        [Route("Itbi_resumo")]
        [HttpGet]
        public ActionResult Itbi_resumo() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Resumo_Pagto_Itbi.rpt"));
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
            try {
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Resumo_Pagto_Itbi.pdf");
            } catch (Exception ex) {
                    throw ex;
            }
        }

        [Route("Itbi_resumo_filter")]
        [HttpGet]
        public ActionResult Itbi_resumo_filter() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll _imovel = new Imovel_bll(_connection);
            List<Itbi_status> lista = _imovel.Lista_Itbi_Status();
            ViewBag.ListaStatus = new SelectList(lista.Where(c=>c.Codigo<5), "codigo", "descricao");

            return View();
        }

        [Route("Itbi_resumo_filter")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Itbi_resumo_filter(Itbi_Relatorio model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Resumo_Pagto_Itbi.rpt"));
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
            try {
                rd.RecordSelectionFormula = "{itbi_main.data_cadastro}>=#" + Convert.ToDateTime(model.Data_Inicio).ToString("MM/dd/yyyy") + "# and { itbi_main.data_cadastro}<= #" + Convert.ToDateTime(model.Data_Final).ToString("MM/dd/yyyy") + "# and { itbi_main.Situacao_itbi}=" + model.Situacao_Id + " and { itbi_main.Itbi_Numero}>0";
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Resumo_Pagto_Itbi.pdf");
            } catch (Exception ex) {
                throw ex;
            }
        }

        [Route("Itbi_urbano")]
        [HttpGet]
        public ActionResult Itbi_urbano(string guid, string a, int s = 0) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ItbiViewModel model = new ItbiViewModel() {
                UserId = Convert.ToInt32(Session["hashid"])
            };
            if (guid == "" || guid == null) {
                model.Codigo = "";
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
                model.Lista_Erro = new List<string>();
            } else {
                Imovel_bll imovelRepository = new Imovel_bll(_connection);
                List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
                ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
                List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
                ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
                ViewBag.ListaErro = new List<string>();

                if (a == "rc") {//remover comprador
                    Exception ex = imovelRepository.Excluir_Itbi_comprador(guid, s);
                    if (ex != null)
                        throw ex;

                }
                if (a == "rv") {//remover vendedor
                    Exception ex = imovelRepository.Excluir_Itbi_vendedor(guid, s);
                    if (ex != null)
                        throw ex;

                }
                if (a == "ra") {//remover anexo
                    Exception ex = imovelRepository.Excluir_Itbi_anexo(guid, s);
                    if (ex != null)
                        throw ex;

                }
                model = Retorna_Itbi_Gravado(guid);

            }
            return View(model);
        }

        [Route("Itbi_urbano")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_urbano(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<ListCompradorEditorViewModel> Lista_comprador = new List<ListCompradorEditorViewModel>();
            List<Itbi_comprador> listaC = imovelRepository.Retorna_Itbi_Comprador(model.Guid);
            foreach (Itbi_comprador item in listaC) {
                ListCompradorEditorViewModel itemC = new ListCompradorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_comprador.Add(itemC);
            }
            model.Lista_Comprador = Lista_comprador;

            List<ListVendedorEditorViewModel> Lista_vendedor = new List<ListVendedorEditorViewModel>();
            List<Itbi_vendedor> listaV = imovelRepository.Retorna_Itbi_vendedor(model.Guid);
            foreach (Itbi_vendedor item in listaV) {
                ListVendedorEditorViewModel itemV = new ListVendedorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_vendedor.Add(itemV);
            }
            model.Lista_Vendedor = Lista_vendedor;


            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            string _guid = "";
            ModelState.Clear();

            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");
            if (model.Comprador != null && model.Comprador.Cep != null) {
                int _ceptmp = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_ceptmp);
                Lista_Logradouro = new List<Logradouro>();
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


                if (model.Comprador.Logradouro_Nome != null) {
                    string a = Functions.RetornaNumero(model.Comprador.Logradouro_Nome);
                    Cepdb _cepdb = null;
                    if (a == "")
                        _cepdb = enderecoRepository.Retorna_CepDB(_ceptmp, model.Comprador.Logradouro_Nome);
                    else {
                        if (Lista_Logradouro.Count > 0) {
                            int b = Convert.ToInt32(a) - 1;
                            _cepdb = enderecoRepository.Retorna_CepDB(_ceptmp, Lista_Logradouro[b].Endereco);
                            if (_cepdb.Bairrocodigo > 0) {
                                model.Comprador.Bairro_Codigo = _cepdb.Bairrocodigo;
                                model.Comprador.Bairro_Nome = enderecoRepository.Retorna_Bairro(model.Comprador.UF, _cepdb.Cidadecodigo, _cepdb.Bairrocodigo);
                            }
                        }

                    }
                }
            }

            
            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.Lista_Erro = new List<string>();
            if (model.Comprador == null) {
                model.Comprador = new Comprador_Itbi();
            }
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            else {
                List<Itbi_anexo> Lista_Anexo_tmp = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
                model.Lista_Anexo.Clear();
                foreach (Itbi_anexo itemA in Lista_Anexo_tmp) {
                    ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                        Seq = itemA.Seq,
                        Nome = itemA.Descricao,
                        Arquivo = itemA.Arquivo.RemoveDiacritics()
                    };
                    model.Lista_Anexo.Add(regA);
                }
            }

            model.Lista_Erro = new List<string>();
            if (model.Totalidade == "Sim")
                model.Totalidade_Perc = 0;

            bool _find = false;
            if (model.Comprador_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Comprador.Count; i++) {
                    if (model.Lista_Comprador[i].Cpf_Cnpj == model.Comprador_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (model.Cpf_Cnpj == Functions.RetornaNumero(model.Comprador_Cpf_cnpj_tmp)) {
                _find = true;
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListCompradorEditorViewModel();
                editorViewModel.Seq = model.Lista_Comprador.Count;
                editorViewModel.Nome = model.Comprador_Nome_tmp != null ? model.Comprador_Nome_tmp.ToUpper() : model.Comprador_Nome_tmp;

                string _cpfMask = model.Comprador_Cpf_cnpj_tmp;
                if (_cpfMask != null) {
                    if (_cpfMask.Length>11 &&   Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(14, '0');
                    } else {
                        if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(11, '0');
                        }
                    }
                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Comprador.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Comprador.Add(editorViewModel);
                }

            }
            model.Comprador_Cpf_cnpj_tmp = "";

            _find = false;
            if (model.Vendedor_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Vendedor.Count; i++) {
                    if (Functions.RetornaNumero(model.Lista_Vendedor[i].Cpf_Cnpj) == model.Vendedor_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListVendedorEditorViewModel();
                editorViewModel.Seq = model.Lista_Vendedor.Count;
                editorViewModel.Nome = model.Vendedor_Nome_tmp != null ? model.Vendedor_Nome_tmp.ToUpper() : model.Vendedor_Nome_tmp;
                string _cpfMask = model.Vendedor_Cpf_cnpj_tmp;
                if (_cpfMask != null) {
                    if (_cpfMask.Length > 11 && Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(14, '0');
                    } else {
                        if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(11, '0');
                        }
                    }
                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Vendedor.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Vendedor.Add(editorViewModel);
                }
            }


            if (action == "btnCodigoCancel") {
                if (model.Guid != null) {
                    Exception ex = imovelRepository.Excluir_Itbi(model.Guid);
                }
                ViewBag.Error = "";
                model = new ItbiViewModel();
                return View(model);
            }

            if (action == "btnCpfCompradorOK") {
                if (model.Cpf_Cnpj != null) {
                    string _cpfCnpj = model.Cpf_Cnpj;
                    if (_cpfCnpj.Length > 11) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        if (!Functions.ValidaCNPJ(_cpfCnpj)) {
                            ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                            model.Cpf_Cnpj = "";
                            return View(model);
                        } else {
                            _bcnpj = true;
                        }
                    } else {
                        if (Functions.ValidaCpf(_cpfCnpj.PadLeft(11, '0'))) {
                            _bcpf = true;
                        }
                    }
                    if (_bcnpj) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                    } else {
                        if (_bcpf) {
                            _cpfCnpj = _cpfCnpj.PadLeft(11, '0');
                            _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                        }
                    }
                    if (_bcpf || _bcnpj) {
                        model.Cpf_Cnpj = _cpfCnpj;
                    } else {
                        ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                        model.Cpf_Cnpj = "";
                        return View(model);
                    }
                } else {
                    ViewBag.Error = "* Digite o Cpf/Cnpj do comprador.";
                    return View(model);
                }
            }

            if (action == "btnCpfCompradorCancel") {
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
            }

            if (action == "btnCepCompradorOK") {
                if (model.Comprador.Cep == null || model.Comprador.Cep.Length < 9) {
                    ViewBag.Error = "* Cep do comprador inválido.";
                    return View(model);
                }

                int _cep = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                var cepObj = Classes.Cep.Busca_CepDB(_cep);
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                    Lista_Logradouro = new List<Logradouro>();
                    int s = 1;
                    foreach (string item in Lista_Tmp) {
                        Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                        s++;
                    }
                    ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


                    Bairro bairro = enderecoRepository.Retorna_CepDB_Bairro(_cep);
                    if (bairro != null) {
                        model.Comprador.Bairro_Codigo = bairro.Codbairro;
                        model.Comprador.Bairro_Nome = bairro.Descbairro;
                    }
                    Cidade cidade = enderecoRepository.Retorna_CepDB_Cidade(_cep);
                    if (cidade != null) {
                        model.Comprador.Cidade_Codigo = cidade.Codcidade;
                        model.Comprador.Cidade_Nome = cidade.Desccidade;
                    }
                    model.Comprador.UF = cepObj.Estado;

                } else {
                    model.Comprador.Logradouro_Codigo = 0;
                    model.Comprador.Logradouro_Nome = "";
                    model.Comprador.Bairro_Codigo = 0;
                    model.Comprador.Bairro_Nome = "";
                    model.Comprador.Cidade_Codigo = 0;
                    model.Comprador.Cidade_Nome = "";
                    model.Comprador.Numero = 0;
                    model.Comprador.Complemento = "";
                    model.Comprador.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                    return View(model);
                }
            }

            if (action == "btnValida") {
                model.Lista_Erro = Itbi_Valida(model);

                if (model.Lista_Erro.Count > 0) {
                    ViewBag.ListaErro = new SelectList(model.Lista_Erro);
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);
                    return View(model);
                } else {
                    if (model.Itbi_Numero == 0) {
                        ItbiAnoNumero _num = imovelRepository.Alterar_Itbi_Main(model.Guid);
                        model.Itbi_Numero = _num.Numero;
                        model.Itbi_Ano = _num.Ano;
                    }
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);

                    //**** log ****************
                    int _userid = 2;
                    bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
                    if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
                    string _obs = "Imóvel código: " + model.Inscricao.ToString() + ", Itbi nº " + model.Itbi_Numero.ToString() + "/" + model.Itbi_Ano.ToString();
                    Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                    LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 8, Pref = _prf, Obs = _obs };
                    sistemaRepository.Incluir_LogWeb(regWeb);
                    //*************************

                    return RedirectToAction("itbi_ok");
                }
            }

            if (action == "btnAnexoAdd") {
                if (file != null) {
                    if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                        ViewBag.Error = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                        return View(model);
                    } else {
                        if (file.ContentType != "application/pdf") {
                            ViewBag.Error = "* Este tipo de arquivo não pode ser enviado como anexo.";
                            return View(model);
                        } else {
                            string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                            string _path = "~/Files/Itbi/" + _ano + "/";
                            var fileName = Path.GetFileName(file.FileName);
                            fileName = fileName.RemoveDiacritics();
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                            file.SaveAs(path);

                            byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                            Itbi_anexo regA = new Itbi_anexo() {
                                Guid = model.Guid,
                                Seq = seqA,
                                Descricao = model.Anexo_Desc_tmp,
                                Arquivo = fileName
                            };
                            Exception ex = imovelRepository.Incluir_Itbi_Anexo(regA);
                            ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                Seq = model.Lista_Anexo.Count,
                                Arquivo = fileName,
                                Nome = model.Anexo_Desc_tmp
                            };
                            model.Lista_Anexo.Add(Anexo);

                            Itbi_Save(model);
                            return View(model);
                        }
                    }
                } else {
                    ViewBag.Error = "* Nenhum arquivo selecionado.";
                    return View(model);
                }
            }

            List<Itbi_anexo> Lista_Anexo = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
            model.Lista_Anexo.Clear();
            foreach (Itbi_anexo itemA in Lista_Anexo) {
                ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                    Seq = itemA.Seq,
                    Nome = itemA.Descricao,
                    Arquivo = itemA.Arquivo
                };
                model.Lista_Anexo.Add(regA);
            }

            model.Vendedor_Cpf_cnpj_tmp = "";
            Int64 _matricula = model.Matricula;
            model = Itbi_Urbano_Load(model, _bcpf, _bcnpj);
            model.Matricula = _matricula > 0 ? _matricula : model.Matricula;


            if (model.Comprador.Cep != null) {
                int _ceptmp = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_ceptmp);
                Lista_Logradouro = new List<Logradouro>();
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");

                if (model.Comprador.Logradouro_Nome != null) {
                    Cepdb _cepdb = enderecoRepository.Retorna_CepDB(_ceptmp, model.Comprador.Logradouro_Nome);

                    if (_cepdb.Bairrocodigo > 0) {
                        model.Comprador.Bairro_Codigo = _cepdb.Bairrocodigo;
                        model.Comprador.Bairro_Nome = enderecoRepository.Retorna_Bairro(model.Comprador.UF, _cepdb.Cidadecodigo, _cepdb.Bairrocodigo);
                    }
                }
            }

            if (model.Inscricao == null && Convert.ToInt32(model.Codigo) > 0) {
                ViewBag.Error = "* Imóvel não cadastrado.";
                return View(model);
            }

            _guid = Itbi_Save(model);
            model.Guid = _guid;
            if (_guid == "") {
                ViewBag.Error = "* Ocorreu um erro ao gravar.";
            }
            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            return View(model);
        }

        [Route("Itbi_urbano_e")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_urbano_e(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;
            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            List<ListCompradorEditorViewModel> Lista_comprador = new List<ListCompradorEditorViewModel>();
            List<Itbi_comprador> listaC = imovelRepository.Retorna_Itbi_Comprador(model.Guid);
            foreach (Itbi_comprador item in listaC) {
                ListCompradorEditorViewModel itemC = new ListCompradorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_comprador.Add(itemC);
            }
            model.Lista_Comprador = Lista_comprador;

            List<ListVendedorEditorViewModel> Lista_vendedor = new List<ListVendedorEditorViewModel>();
            List<Itbi_vendedor> listaV = imovelRepository.Retorna_Itbi_vendedor(model.Guid);
            foreach (Itbi_vendedor item in listaV) {
                ListVendedorEditorViewModel itemV = new ListVendedorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_vendedor.Add(itemV);
            }
            model.Lista_Vendedor = Lista_vendedor;


            string _guid = "";
            ModelState.Clear();

            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");

            
            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.Lista_Erro = new List<string>();
            if (model.Comprador == null) {
                model.Comprador = new Comprador_Itbi();
            }
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            else {
                List<Itbi_anexo> Lista_Anexo_tmp = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
                model.Lista_Anexo.Clear();
                foreach (Itbi_anexo itemA in Lista_Anexo_tmp) {
                    ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                        Seq = itemA.Seq,
                        Nome = itemA.Descricao,
                        Arquivo = itemA.Arquivo
                    };
                    model.Lista_Anexo.Add(regA);
                }
            }

            model.Lista_Erro = new List<string>();
            if (model.Totalidade == "Sim")
                model.Totalidade_Perc = 0;

            bool _find = false;
            if (model.Comprador_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Comprador.Count; i++) {
                    if (model.Lista_Comprador[i].Cpf_Cnpj == model.Comprador_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (model.Cpf_Cnpj == Functions.RetornaNumero(model.Comprador_Cpf_cnpj_tmp)) {
                _find = true;
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListCompradorEditorViewModel();
                editorViewModel.Seq = model.Lista_Comprador.Count;
                editorViewModel.Nome = model.Comprador_Nome_tmp != null ? model.Comprador_Nome_tmp.ToUpper() : model.Comprador_Nome_tmp;
                string _cpfMask = model.Comprador_Cpf_cnpj_tmp;
                if (_cpfMask != null) {
                    if (Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(14, '0');
                    } else {
                        if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(11, '0');
                        }
                    }
                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Comprador.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Comprador.Add(editorViewModel);
                }

            }
            model.Comprador_Cpf_cnpj_tmp = "";

            _find = false;
            if (model.Vendedor_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Vendedor.Count; i++) {
                    if (Functions.RetornaNumero(model.Lista_Vendedor[i].Cpf_Cnpj) == model.Vendedor_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListVendedorEditorViewModel();
                editorViewModel.Seq = model.Lista_Vendedor.Count;
                editorViewModel.Nome = model.Vendedor_Nome_tmp != null ? model.Vendedor_Nome_tmp.ToUpper() : model.Vendedor_Nome_tmp;
                string _cpfMask = model.Vendedor_Cpf_cnpj_tmp;
                if (_cpfMask != null) {
                    if (_cpfMask.Length>11 &&  Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(14, '0');
                    } else {
                        if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(11, '0');
                        }
                    }
                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Vendedor.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Vendedor.Add(editorViewModel);
                }
            }

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (model.Comprador.Cep != null) {
                int _ceptmp = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_ceptmp);
                Lista_Logradouro = new List<Logradouro>();
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");
                if (model.Comprador.Logradouro_Nome != null) {
                    Cepdb _cepdb = enderecoRepository.Retorna_CepDB(_ceptmp, Lista_Logradouro[Convert.ToInt32(model.Comprador.Logradouro_Nome) - 1].Endereco);

                    if (_cepdb != null) {
                        model.Comprador.Bairro_Codigo = _cepdb.Bairrocodigo;
                        model.Comprador.Bairro_Nome = enderecoRepository.Retorna_Bairro(model.Comprador.UF, _cepdb.Cidadecodigo, _cepdb.Bairrocodigo);
                    }
                }
            }



            if (action == "btnAtualizarImovel") {
                if (model.Guid != null) {
                    ImovelStruct imovel = imovelRepository.Dados_Imovel(Convert.ToInt32(model.Codigo));
                    model.Dados_Imovel = imovel;
                    List<ProprietarioStruct> ListaProp = imovelRepository.Lista_Proprietario(Convert.ToInt32(model.Codigo), true);
                    if (ListaProp.Count > 0) {
                        model.Dados_Imovel.Proprietario_Codigo = ListaProp[0].Codigo;
                        model.Dados_Imovel.Proprietario_Nome = ListaProp[0].Nome;
                        Itbi_Save(model);
                    }
                }
            }

            if (action == "btnCodigoCancel") {
                if (model.Guid != null) {
                    Exception ex = imovelRepository.Excluir_Itbi(model.Guid);
                }
                ViewBag.Error = "";
                model = new ItbiViewModel();
                return View(model);
            }

            if (action == "btnCpfCompradorOK") {
                if (model.Cpf_Cnpj != null) {
                    string _cpfCnpj = model.Cpf_Cnpj;
                    if (_cpfCnpj.Length > 11) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        if (!Functions.ValidaCNPJ(_cpfCnpj)) {
                            ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                            model.Cpf_Cnpj = "";
                            return View(model);
                        } else {
                            _bcnpj = true;
                        }
                    } else {
                        if (_cpfCnpj.Length>11 &&   Functions.ValidaCNPJ(_cpfCnpj.PadLeft(14, '0'))) {
                            _bcnpj = true;
                        } else {
                            if (Functions.ValidaCpf(_cpfCnpj.PadLeft(11, '0'))) {
                                _bcpf = true;
                            }
                        }
                    }
                    if (_bcnpj) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                    } else {
                        if (_bcpf) {
                            _cpfCnpj = _cpfCnpj.PadLeft(11, '0');
                            _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                        }
                    }
                    if (_bcpf || _bcnpj) {
                        model.Cpf_Cnpj = _cpfCnpj;
                    } else {
                        ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                        model.Cpf_Cnpj = "";
                        return View(model);
                    }
                } else {
                    ViewBag.Error = "* Digite o Cpf/Cnpj do comprador.";
                    return View(model);
                }
            }

            if (action == "btnCpfCompradorCancel") {
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
            }

            if (action == "btnCepCompradorOK") {
                if (model.Comprador.Cep == null || model.Comprador.Cep.Length < 9) {
                    ViewBag.Error = "* Cep do comprador inválido.";
                    return View(model);
                }

                int _cep = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                var cepObj = Classes.Cep.Busca_CepDB(_cep);
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                    Lista_Logradouro = new List<Logradouro>();
                    int s = 1;
                    foreach (string item in Lista_Tmp) {
                        Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                        s++;
                    }
                    ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


                    Bairro bairro = enderecoRepository.Retorna_CepDB_Bairro(_cep);
                    if (bairro != null) {
                        model.Comprador.Bairro_Codigo = bairro.Codbairro;
                        model.Comprador.Bairro_Nome = bairro.Descbairro;
                    }
                    Cidade cidade = enderecoRepository.Retorna_CepDB_Cidade(_cep);
                    if (cidade != null) {
                        model.Comprador.Cidade_Codigo = cidade.Codcidade;
                        model.Comprador.Cidade_Nome = cidade.Desccidade;
                    }
                    model.Comprador.UF = cepObj.Estado;

                } else {
                    model.Comprador.Logradouro_Codigo = 0;
                    model.Comprador.Logradouro_Nome = "";
                    model.Comprador.Bairro_Codigo = 0;
                    model.Comprador.Bairro_Nome = "";
                    model.Comprador.Cidade_Codigo = 0;
                    model.Comprador.Cidade_Nome = "";
                    model.Comprador.Numero = 0;
                    model.Comprador.Complemento = "";
                    model.Comprador.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                    return View(model);
                }

            }

            if (action == "btnValida") {
                model.Lista_Erro = Itbi_Valida(model);

                if (model.Lista_Erro.Count > 0) {
                    ViewBag.ListaErro = new SelectList(model.Lista_Erro);
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);
                    return View(model);
                } else {
                    if (model.Itbi_Numero == 0) {
                        ItbiAnoNumero _num = imovelRepository.Alterar_Itbi_Main(model.Guid);
                        model.Itbi_Numero = _num.Numero;
                        model.Itbi_Ano = _num.Ano;
                    }
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);

                    return RedirectToAction("itbi_ok");
                }
            }

            if (action == "btnAnexoAdd") {
                if (file != null) {
                    if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                        ViewBag.Error = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                        return View(model);
                    } else {
                        if (file.ContentType != "application/pdf") {
                            ViewBag.Error = "* Este tipo de arquivo não pode ser enviado como anexo.";
                            return View(model);
                        } else {
                            string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                            string _path = "~/Files/Itbi/" + _ano + "/";
                            var fileName = Path.GetFileName(file.FileName);
                            fileName = fileName.RemoveDiacritics();
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                            file.SaveAs(path);

                            byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                            Itbi_anexo regA = new Itbi_anexo() {
                                Guid = model.Guid,
                                Seq = seqA,
                                Descricao = model.Anexo_Desc_tmp,
                                Arquivo = fileName
                            };
                            Exception ex = imovelRepository.Incluir_Itbi_Anexo(regA);
                            ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                Seq = model.Lista_Anexo.Count,
                                Arquivo = fileName,
                                Nome = model.Anexo_Desc_tmp
                            };
                            model.Lista_Anexo.Add(Anexo);

                            Itbi_Save(model);
                            return View(model);
                        }
                    }
                } else {
                    ViewBag.Error = "* Nenhum arquivo selecionado.";
                    return View(model);
                }
            }

            List<Itbi_anexo> Lista_Anexo = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
            model.Lista_Anexo.Clear();
            foreach (Itbi_anexo itemA in Lista_Anexo) {
                ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                    Seq = itemA.Seq,
                    Nome = itemA.Descricao,
                    Arquivo = itemA.Arquivo
                };
                model.Lista_Anexo.Add(regA);
            }

            model.Vendedor_Cpf_cnpj_tmp = "";
            Int64 _matricula = model.Matricula;
            model = Itbi_Urbano_Load(model, _bcpf, _bcnpj);
            model.Matricula = _matricula > 0 ? _matricula : model.Matricula;

            if (model.Inscricao == null && Convert.ToInt32(model.Codigo) > 0) {
                ViewBag.Error = "* Imóvel não cadastrado.";
                return View(model);
            }

            _guid = Itbi_Save(model);
            model.Guid = _guid;
            if (_guid == "") {
                ViewBag.Error = "* Ocorreu um erro ao gravar.";
            }

            if (model.Itbi_Ano > 0)
                model.Itbi_NumeroAno = model.Itbi_Numero.ToString("000000/") + model.Itbi_Ano.ToString();


            return View(model);
        }

        [Route("Itbi_ok")]
        [HttpGet]
        public ActionResult Itbi_ok() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Itbi_isencao_ok")]
        [HttpGet]
        public ActionResult Itbi_isencao_ok() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Itbi_query")]
        [HttpGet]
        public ActionResult Itbi_query(string e = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            if (_userId == 262)
                ViewBag.ReadOnly = "S";
            else
                ViewBag.ReadOnly = "N";
            bool _fiscal = Session["hashfiscalitbi"] != null && Session["hashfiscalitbi"].ToString() == "S" ? true : false;
            List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Query(_userId, _fiscal, 0, DateTime.Now.Year);
            List<ItbiViewModel> model = new List<ItbiViewModel>();
            foreach (Itbi_Lista reg in Lista) {
                ItbiViewModel item = new ItbiViewModel() {
                    Guid = reg.Guid,
                    Data_cadastro = Convert.ToDateTime(reg.Data.ToString("dd/MM/yyyy")),
                    Itbi_NumeroAno = reg.Numero_Ano,
                    Tipo_Imovel = reg.Tipo,
                    Comprador_Nome_tmp = Functions.TruncateTo(reg.Nome_Comprador, 25),
                    Situacao_Itbi_Nome = reg.Situacao,
                    Situacao_Itbi_codigo = reg.Situacao_Codigo,
                    Ano_Selected = DateTime.Now.Year.ToString()
                };
                model.Add(item);
            }
            if (Lista.Count == 0) {
                ItbiViewModel item = new ItbiViewModel() {
                    Ano_Selected = DateTime.Now.Year.ToString()
                };
                model.Add(item);
            }
                
            ViewBag.Erro = e;
            return View(model);
        }

        [Route("Itbi_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Itbi_query(List<ItbiViewModel> model) {
            string _ano = model[0].Ano_Selected ?? "";
            if (_ano == "")
                _ano = DateTime.Now.Year.ToString();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _fiscal = Session["hashfiscalitbi"] != null && Session["hashfiscalitbi"].ToString() == "S" ? true : false;
            List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Query(_userId, _fiscal, Convert.ToInt32(model[0].Status_Query), Convert.ToInt32(_ano));
            model.Clear();
            foreach (Itbi_Lista reg in Lista) {
                ItbiViewModel item = new ItbiViewModel() {
                    Guid = reg.Guid,
                    Data_cadastro = Convert.ToDateTime(reg.Data.ToString("dd/MM/yyyy")),
                    Itbi_NumeroAno = reg.Numero_Ano,
                    Tipo_Imovel = reg.Tipo,
                    Comprador_Nome_tmp = Functions.TruncateTo(reg.Nome_Comprador, 26),
                    Situacao_Itbi_Nome = reg.Situacao,
                    Situacao_Itbi_codigo=reg.Situacao_Codigo
                };
                model.Add(item);
            }
            return View(model);
        }

        [Route("Itbi_rural")]
        [HttpGet]
        public ActionResult Itbi_rural(string guid, string a, int s = 0) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ItbiViewModel model = new ItbiViewModel() {
                UserId = Convert.ToInt32(Session["hashid"])
                //UserId = Functions.pUserId
            };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.ListaErro = new List<string>();
            model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            if (guid == "" || guid == null) {
                model.Codigo = "";
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
                model.Lista_Erro = new List<string>();
            } else {

                if (a == "rc") {//remover comprador
                    Exception ex = imovelRepository.Excluir_Itbi_comprador(guid, s);
                }
                if (a == "rv") {//remover vendedor
                    Exception ex = imovelRepository.Excluir_Itbi_vendedor(guid, s);
                }
                if (a == "ra") {//remover anexo
                    Exception ex = imovelRepository.Excluir_Itbi_anexo(guid, s);
                }
                model = Retorna_Itbi_Gravado(guid);
            }
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            return View(model);
        }

        [Route("Itbi_rural")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_rural(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;

            string _guid = "";
            ModelState.Clear();

            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            List<ListCompradorEditorViewModel> Lista_comprador = new List<ListCompradorEditorViewModel>();
            List<Itbi_comprador> listaC = imovelRepository.Retorna_Itbi_Comprador(model.Guid);
            foreach (Itbi_comprador item in listaC) {
                ListCompradorEditorViewModel itemC = new ListCompradorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_comprador.Add(itemC);
            }
            model.Lista_Comprador = Lista_comprador;

            List<ListVendedorEditorViewModel> Lista_vendedor = new List<ListVendedorEditorViewModel>();
            List<Itbi_vendedor> listaV = imovelRepository.Retorna_Itbi_vendedor(model.Guid);
            foreach (Itbi_vendedor item in listaV) {
                ListVendedorEditorViewModel itemV = new ListVendedorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_vendedor.Add(itemV);
            }
            model.Lista_Vendedor = Lista_vendedor;

            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            ViewBag.Lista_Erro = new List<string>();
            if (model.Comprador == null) {
                model.Comprador = new Comprador_Itbi();
            }
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            else {
                List<Itbi_anexo> Lista_Anexo_tmp = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
                model.Lista_Anexo.Clear();
                foreach (Itbi_anexo itemA in Lista_Anexo_tmp) {
                    ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                        Seq = itemA.Seq,
                        Nome = itemA.Descricao,
                        Arquivo = itemA.Arquivo
                    };
                    model.Lista_Anexo.Add(regA);
                }
            }

            model.Lista_Erro = new List<string>();
            if (model.Totalidade == "Sim")
                model.Totalidade_Perc = 0;

            bool _find = false;
            if (model.Comprador_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Comprador.Count; i++) {
                    if (model.Lista_Comprador[i].Cpf_Cnpj == model.Comprador_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (model.Cpf_Cnpj == Functions.RetornaNumero(model.Comprador_Cpf_cnpj_tmp)) {
                _find = true;
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListCompradorEditorViewModel();
                editorViewModel.Seq = model.Lista_Comprador.Count;
                editorViewModel.Nome = model.Comprador_Nome_tmp != null ? model.Comprador_Nome_tmp.ToUpper() : model.Comprador_Nome_tmp;
                string _cpfMask = model.Comprador_Cpf_cnpj_tmp;
                if (_cpfMask != null) {
                    if (_cpfMask.Length<11 &&  Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(14, '0');
                    } else {
                        if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(11, '0');
                        }
                    }
                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Comprador.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Comprador.Add(editorViewModel);
                }

            }
            model.Comprador_Cpf_cnpj_tmp = "";

            _find = false;
            if (model.Vendedor_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Vendedor.Count; i++) {
                    if (Functions.RetornaNumero(model.Lista_Vendedor[i].Cpf_Cnpj) == model.Vendedor_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListVendedorEditorViewModel();
                editorViewModel.Seq = model.Lista_Vendedor.Count;
                editorViewModel.Nome = model.Vendedor_Nome_tmp != null ? model.Vendedor_Nome_tmp.ToUpper() : model.Vendedor_Nome_tmp;

                string _cpfMask = Convert.ToInt64(model.Vendedor_Cpf_cnpj_tmp).ToString();
                if (_cpfMask != null) {
                    if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(11, '0');
                    } else {
                        if (Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(14, '0');
                        }
                    }

                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Vendedor.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Vendedor.Add(editorViewModel);
                }
            }

            if (action == "btnCpfCompradorOK") {
                if (model.Cpf_Cnpj != null) {
                    string _cpfCnpj = model.Cpf_Cnpj;
                    if (_cpfCnpj.Length > 11) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        if (!Functions.ValidaCNPJ(_cpfCnpj)) {
                            ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                            model.Cpf_Cnpj = "";
                            return View(model);
                        } else {
                            _bcnpj = true;
                        }
                    } else {
                        if (_cpfCnpj.Length > 11 && Functions.ValidaCNPJ(_cpfCnpj.PadLeft(14, '0'))) {
                            _bcnpj = true;
                        } else {
                            if (Functions.ValidaCpf(_cpfCnpj.PadLeft(11, '0'))) {
                                _bcpf = true;
                            }
                        }
                    }
                    if (_bcnpj) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                    } else {
                        if (_bcpf) {
                            _cpfCnpj = _cpfCnpj.PadLeft(11, '0');
                            _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                        }
                    }
                    if (_bcpf || _bcnpj) {
                        model.Cpf_Cnpj = _cpfCnpj;
                    } else {
                        ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                        model.Cpf_Cnpj = "";
                        return View(model);
                    }
                } else {
                    ViewBag.Error = "* Digite o Cpf/Cnpj do comprador.";
                    return View(model);
                }
            }

            if (action == "btnCpfCompradorCancel") {
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
            }

            if (action == "btnCepCompradorOK") {
                if (model.Comprador.Cep == null || model.Comprador.Cep.Length < 9) {
                    ViewBag.Error = "* Cep do comprador inválido.";
                    if (model.Comprador != null)
                        model.Comprador.Cep = Convert.ToInt32(model.Comprador.Cep).ToString("00000-000");
                    return View(model);
                }

                var cepObj = Classes.Cep.Busca_CepDB(Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep)));
                //var cepObj = Classes.Cep.Busca_Correio(Functions.RetornaNumero(model.Comprador.Cep));
                //var cepObj = Classes.Cep.Busca(Functions.RetornaNumero(model.Comprador.Cep));
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                    LogradouroStruct _log = enderecoRepository.Retorna_Logradouro_Cep(Convert.ToInt32(Functions.RetornaNumero(cepObj.CEP)));
                    if (_log.Endereco != null) {
                        model.Comprador.Logradouro_Codigo = (int)_log.CodLogradouro;
                        model.Comprador.Logradouro_Nome = _log.Endereco;
                    } else {
                        model.Comprador.Logradouro_Codigo = 0;
                        model.Comprador.Logradouro_Nome = rua.ToUpper();
                    }

                    Bairro bairro = enderecoRepository.RetornaLogradouroBairro(model.Comprador.Logradouro_Codigo, (short)model.Comprador.Numero);
                    if (bairro.Descbairro != null) {
                        model.Comprador.Bairro_Codigo = bairro.Codbairro;
                        model.Comprador.Bairro_Nome = bairro.Descbairro;
                    } else {
                        string _uf = cepObj.Estado;
                        string _cidade = cepObj.Cidade;
                        string _bairro = cepObj.Bairro;
                        int _codcidade = enderecoRepository.Retorna_Cidade(_uf, _cidade);
                        if (_codcidade > 0) {
                            model.Comprador.Cidade_Codigo = _codcidade;
                            if (_codcidade != 413) {
                                //verifica se bairro existe nesta cidade
                                bool _existeBairro = enderecoRepository.Existe_Bairro(_uf, _codcidade, _bairro);
                                if (!_existeBairro) {
                                    Bairro reg = new Bairro() {
                                        Siglauf = _uf,
                                        Codcidade = (short)_codcidade,
                                        Descbairro = _bairro.ToUpper()
                                    };
                                    int _codBairro = enderecoRepository.Incluir_bairro(reg);
                                    model.Comprador.Bairro_Codigo = _codBairro;
                                }
                            }
                        } else {
                            model.Comprador.Cidade_Codigo = 0;
                        }
                        model.Comprador.Bairro_Nome = cepObj.Bairro.ToUpper();
                    }

                    model.Comprador.Cidade_Nome = cepObj.Cidade.ToUpper();
                    model.Comprador.UF = cepObj.Estado;
                } else {
                    model.Comprador.Logradouro_Codigo = 0;
                    model.Comprador.Logradouro_Nome = "";
                    model.Comprador.Bairro_Codigo = 0;
                    model.Comprador.Bairro_Nome = "";
                    model.Comprador.Cidade_Codigo = 0;
                    model.Comprador.Cidade_Nome = "";
                    model.Comprador.Numero = 0;
                    model.Comprador.Complemento = "";
                    model.Comprador.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                    return View(model);
                }
            }

            if (action == "btnValida") {
                model.Lista_Erro = Itbi_Valida(model);

                if (model.Lista_Erro.Count > 0) {
                    ViewBag.ListaErro = new SelectList(model.Lista_Erro);
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);
                    return View(model);
                } else {
                    if (model.Itbi_Numero == 0) {
                        ItbiAnoNumero _num = imovelRepository.Alterar_Itbi_Main(model.Guid);
                        model.Itbi_Numero = _num.Numero;
                        model.Itbi_Ano = _num.Ano;
                    }
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);

                    //**** log ****************
                    int _userid = 2;
                    bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
                    if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
                    string _obs = "Itbi nº " + model.Itbi_NumeroAno;
                    Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                    LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 9, Pref = _prf, Obs = _obs };
                    sistemaRepository.Incluir_LogWeb(regWeb);
                    //*************************

                    return RedirectToAction("itbi_ok");
                }
            }

            if (action == "btnAnexoAdd") {
                if (model.Guid == null){
                    ViewBag.Error = "* Selecione o comprador antes de anexar os documentos).";
                    return View(model);
                }

                if (file != null && model.Guid!=null) {
                    if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                        ViewBag.Error = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                        return View(model);
                    } else {
                        if (file.ContentType != "application/pdf") {
                            ViewBag.Error = "* Este tipo de arquivo não pode ser enviado como anexo.";
                            return View(model);
                        } else {
                            string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                            string _path = "~/Files/Itbi/" + _ano + "/";
                            var fileName = Path.GetFileName(file.FileName);
                            fileName = fileName.RemoveDiacritics();
                            bool _existe = false;
                            foreach (ListAnexoEditorViewModel item in model.Lista_Anexo) {
                                if (fileName.ToUpper() == item.Arquivo.ToUpper()) {
                                    _existe = true;
                                    break;
                                }
                            }
                            if (_existe) {
                                ViewBag.Error = "* Já foi incluído um arquivo com o mesmo nome (" + fileName + ").";
                                return View(model);
                            }

                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                            file.SaveAs(path);

                            byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                            Itbi_anexo regA = new Itbi_anexo() {
                                Guid = model.Guid,
                                Seq = seqA,
                                Descricao = model.Anexo_Desc_tmp,
                                Arquivo = fileName
                            };
                            Exception ex = imovelRepository.Incluir_Itbi_Anexo(regA);
                            ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                Seq = model.Lista_Anexo.Count,
                                Arquivo = fileName,
                                Nome = model.Anexo_Desc_tmp
                            };
                            model.Lista_Anexo.Add(Anexo);

                            Itbi_Save(model);
                            return View(model);
                        }
                    }
                } else {
                    ViewBag.Error = "* Nenhum arquivo selecionado.";
                    return View(model);
                }
            }

            List<Itbi_anexo> Lista_Anexo = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
            model.Lista_Anexo.Clear();
            foreach (Itbi_anexo itemA in Lista_Anexo) {
                ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                    Seq = itemA.Seq,
                    Nome = itemA.Descricao,
                    Arquivo = itemA.Arquivo
                };
                model.Lista_Anexo.Add(regA);
            }

            Int64 _matricula = model.Matricula;
            model = Itbi_Rural_Load(model, _bcpf, _bcnpj);
            model.Matricula = _matricula;

            _guid = Itbi_Save(model);
            model.Guid = _guid;
            if (_guid == "") {
                ViewBag.Error = "* Ocorreu um erro ao gravar.";
            }

            return View(model);
        }

        [Route("Itbi_rural_e")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_rural_e(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;

            string _guid = "";
            ModelState.Clear();

            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            List<ListCompradorEditorViewModel> Lista_comprador = new List<ListCompradorEditorViewModel>();
            List<Itbi_comprador> listaC = imovelRepository.Retorna_Itbi_Comprador(model.Guid);
            foreach (Itbi_comprador item in listaC) {
                ListCompradorEditorViewModel itemC = new ListCompradorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_comprador.Add(itemC);
            }
            model.Lista_Comprador = Lista_comprador;

            List<ListVendedorEditorViewModel> Lista_vendedor = new List<ListVendedorEditorViewModel>();
            List<Itbi_vendedor> listaV = imovelRepository.Retorna_Itbi_vendedor(model.Guid);
            foreach (Itbi_vendedor item in listaV) {
                ListVendedorEditorViewModel itemV = new ListVendedorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_vendedor.Add(itemV);
            }
            model.Lista_Vendedor = Lista_vendedor;
            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.Lista_Erro = new List<string>();

            if (model.Itbi_Ano > 0)
                model.Itbi_NumeroAno = model.Itbi_Numero.ToString("000000/") + model.Itbi_Ano.ToString();

            if (model.Comprador == null) {
                model.Comprador = new Comprador_Itbi();
            }
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            else {
                List<Itbi_anexo> Lista_Anexo_tmp = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
                model.Lista_Anexo.Clear();
                foreach (Itbi_anexo itemA in Lista_Anexo_tmp) {
                    ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                        Seq = itemA.Seq,
                        Nome = itemA.Descricao,
                        Arquivo = itemA.Arquivo
                    };
                    model.Lista_Anexo.Add(regA);
                }
            }

            model.Lista_Erro = new List<string>();
            if (model.Totalidade == "Sim")
                model.Totalidade_Perc = 0;

            bool _find = false;
            if (model.Comprador_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Comprador.Count; i++) {
                    if (model.Lista_Comprador[i].Cpf_Cnpj == model.Comprador_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (model.Cpf_Cnpj == Functions.RetornaNumero(model.Comprador_Cpf_cnpj_tmp)) {
                _find = true;
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListCompradorEditorViewModel();
                editorViewModel.Seq = model.Lista_Comprador.Count;
                editorViewModel.Nome = model.Comprador_Nome_tmp != null ? model.Comprador_Nome_tmp.ToUpper() : model.Comprador_Nome_tmp;
                string _cpfMask = model.Comprador_Cpf_cnpj_tmp;
                if (_cpfMask != null) {

                    if ( Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(11, '0');
                    } else {
                        if (Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(14, '0');
                        }
                    }

                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Comprador.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Comprador.Add(editorViewModel);
                }

            }
            model.Comprador_Cpf_cnpj_tmp = "";

            _find = false;
            if (model.Vendedor_Nome_tmp != null) {
                for (int i = 0; i < model.Lista_Vendedor.Count; i++) {
                    if (Functions.RetornaNumero(model.Lista_Vendedor[i].Cpf_Cnpj) == model.Vendedor_Cpf_cnpj_tmp) {
                        _find = true;
                        break;
                    }
                };
            }

            if (_find) {
                ViewBag.Error = "* Cpf/Cnpj já cadastrado.";
            } else {
                var editorViewModel = new ListVendedorEditorViewModel();
                editorViewModel.Seq = model.Lista_Vendedor.Count;
                editorViewModel.Nome = model.Vendedor_Nome_tmp != null ? model.Vendedor_Nome_tmp.ToUpper() : model.Vendedor_Nome_tmp;
                string _cpfMask = Convert.ToInt64( model.Vendedor_Cpf_cnpj_tmp).ToString();
                if (_cpfMask != null) {
                    if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                        _cpfMask = _cpfMask.PadLeft(11, '0');
                    } else {
                        if (Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                            _cpfMask = _cpfMask.PadLeft(14, '0');
                        }
                    }

                    _cpfMask = Functions.FormatarCpfCnpj(_cpfMask);
                }
                editorViewModel.Cpf_Cnpj = _cpfMask;
                if (editorViewModel.Nome != null) {
                    editorViewModel.Seq = model.Lista_Vendedor.Count;
                    if (editorViewModel.Cpf_Cnpj != null)
                        model.Lista_Vendedor.Add(editorViewModel);
                }
            }

            if (action == "btnCpfCompradorOK") {
                if (model.Cpf_Cnpj != null) {
                    string _cpfCnpj = model.Cpf_Cnpj;
                    if (_cpfCnpj.Length > 11) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        if (!Functions.ValidaCNPJ(_cpfCnpj)) {
                            ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                            model.Cpf_Cnpj = "";
                            return View(model);
                        } else {
                            _bcnpj = true;
                        }
                    } else {

                        if (Functions.ValidaCpf(_cpfCnpj.PadLeft(11, '0'))) {
                            _bcpf = true;
                        } else {
                            if (Functions.ValidaCNPJ(_cpfCnpj.PadLeft(14, '0'))) {
                                _bcnpj = true;
                            }
                        }

                    }
                    if (_bcnpj) {
                        _cpfCnpj = _cpfCnpj.PadLeft(14, '0');
                        _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                    } else {
                        if (_bcpf) {
                            _cpfCnpj = _cpfCnpj.PadLeft(11, '0');
                            _cpfCnpj = Functions.FormatarCpfCnpj(_cpfCnpj);
                        }
                    }
                    if (_bcpf || _bcnpj) {
                        model.Cpf_Cnpj = _cpfCnpj;
                    } else {
                        ViewBag.Error = "* Cpf/Cnpj do comprador inválido.";
                        model.Cpf_Cnpj = "";
                        return View(model);
                    }
                } else {
                    ViewBag.Error = "* Digite o Cpf/Cnpj do comprador.";
                    return View(model);
                }
            }

            if (action == "btnCpfCompradorCancel") {
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
            }

            if (action == "btnCepCompradorOK") {
                if (model.Comprador.Cep == null || model.Comprador.Cep.Length < 9) {
                    ViewBag.Error = "* Cep do comprador inválido.";
                    if (model.Comprador != null)
                        model.Comprador.Cep = Convert.ToInt32(model.Comprador.Cep).ToString("00000-000");
                    return View(model);
                }

                var cepObj = Classes.Cep.Busca_CepDB(Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep)));
                //var cepObj = Classes.Cep.Busca_Correio(Functions.RetornaNumero(model.Comprador.Cep));
                //var cepObj = Classes.Cep.Busca(Functions.RetornaNumero(model.Comprador.Cep));
                int _cep = Convert.ToInt32(Functions.RetornaNumero(cepObj.CEP));
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                    LogradouroStruct _log = enderecoRepository.Retorna_Logradouro_Cep(_cep);
                    if (_log.Endereco != null) {
                        model.Comprador.Logradouro_Codigo = (int)_log.CodLogradouro;
                        model.Comprador.Logradouro_Nome = _log.Endereco;
                    } else {
                        model.Comprador.Logradouro_Codigo = 0;
                        model.Comprador.Logradouro_Nome = rua.ToUpper();
                    }


                    Bairro bairro = enderecoRepository.RetornaLogradouroBairro(model.Comprador.Logradouro_Codigo, (short)model.Comprador.Numero);
                    if (bairro.Descbairro != null) {
                        model.Comprador.Bairro_Codigo = bairro.Codbairro;
                        model.Comprador.Bairro_Nome = bairro.Descbairro;
                    } else {
                        string _uf = cepObj.Estado;
                        string _cidade = cepObj.Cidade;
                        string _bairro = cepObj.Bairro;
                        int _codcidade = enderecoRepository.Retorna_Cidade(_uf, _cidade);
                        if (_codcidade > 0) {
                            model.Comprador.Cidade_Codigo = _codcidade;
                            if (_codcidade != 413) {
                                //verifica se bairro existe nesta cidade
                                bool _existeBairro = enderecoRepository.Existe_Bairro(_uf, _codcidade, _bairro);
                                if (!_existeBairro) {
                                    Bairro reg = new Bairro() {
                                        Siglauf = _uf,
                                        Codcidade = (short)_codcidade,
                                        Descbairro = _bairro.ToUpper()
                                    };
                                    int _codBairro = enderecoRepository.Incluir_bairro(reg);
                                    model.Comprador.Bairro_Codigo = _codBairro;
                                }
                            }
                        } else {
                            model.Comprador.Cidade_Codigo = 0;
                        }
                        model.Comprador.Bairro_Nome = cepObj.Bairro.ToUpper();
                    }

                    model.Comprador.Cidade_Nome = cepObj.Cidade.ToUpper();
                    model.Comprador.UF = cepObj.Estado;
                } else {
                    model.Comprador.Logradouro_Codigo = 0;
                    model.Comprador.Logradouro_Nome = "";
                    model.Comprador.Bairro_Codigo = 0;
                    model.Comprador.Bairro_Nome = "";
                    model.Comprador.Cidade_Codigo = 0;
                    model.Comprador.Cidade_Nome = "";
                    model.Comprador.Numero = 0;
                    model.Comprador.Complemento = "";
                    model.Comprador.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                    return View(model);
                }
            }

            if (action == "btnValida") {
                model.Lista_Erro = Itbi_Valida(model);

                if (model.Lista_Erro.Count > 0) {
                    ViewBag.ListaErro = new SelectList(model.Lista_Erro);
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);
                    return View(model);
                } else {
                    if (model.Itbi_Numero == 0) {
                        ItbiAnoNumero _num = imovelRepository.Alterar_Itbi_Main(model.Guid);
                        model.Itbi_Numero = _num.Numero;
                        model.Itbi_Ano = _num.Ano;
                    }
                    if (model.Comprador.Codigo == 0) {
                        model.Comprador.Codigo = Grava_Cidadao(model);
                    }
                    Itbi_Save(model);
                    return RedirectToAction("itbi_ok");
                }
            }

            if (action == "btnAnexoAdd") {
                if (file != null) {
                    if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                        ViewBag.Error = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                        return View(model);
                    } else {
                        if (file.ContentType != "application/pdf") {
                            ViewBag.Error = "* Este tipo de arquivo não pode ser enviado como anexo.";
                            return View(model);
                        } else {
                            string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                            string _path = "~/Files/Itbi/" + _ano + "/";
                            var fileName = Path.GetFileName(file.FileName);
                            fileName = fileName.RemoveDiacritics();

                            bool _existe = false;
                            foreach (ListAnexoEditorViewModel item in model.Lista_Anexo) {
                                if (fileName.ToUpper() == item.Arquivo.ToUpper()) {
                                    _existe = true;
                                    break;
                                }
                            }
                            if (_existe) {
                                ViewBag.Error = "* Já foi incluído um arquivo com o mesmo nome (" + fileName + ").";
                                return View(model);
                            }

                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);

                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                            file.SaveAs(path);

                            byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                            Itbi_anexo regA = new Itbi_anexo() {
                                Guid = model.Guid,
                                Seq = seqA,
                                Descricao = model.Anexo_Desc_tmp,
                                Arquivo = fileName
                            };
                            Exception ex = imovelRepository.Incluir_Itbi_Anexo(regA);
                            ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                Seq = model.Lista_Anexo.Count,
                                Arquivo = fileName,
                                Nome = model.Anexo_Desc_tmp
                            };
                            model.Lista_Anexo.Add(Anexo);

                            Itbi_Save(model);
                            return View(model);
                        }
                    }
                } else {
                    ViewBag.Error = "* Nenhum arquivo selecionado.";
                    return View(model);
                }
            }

            List<Itbi_anexo> Lista_Anexo = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
            model.Lista_Anexo.Clear();
            foreach (Itbi_anexo itemA in Lista_Anexo) {
                ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                    Seq = itemA.Seq,
                    Nome = itemA.Descricao,
                    Arquivo = itemA.Arquivo
                };
                model.Lista_Anexo.Add(regA);
            }

            Int64 _matricula = model.Matricula;
            model = Itbi_Rural_Load(model, _bcpf, _bcnpj);
            model.Matricula = _matricula;

            _guid = Itbi_Save(model);
            model.Guid = _guid;
            if (_guid == "") {
                ViewBag.Error = "* Ocorreu um erro ao gravar.";
            }

            return View(model);
        }

        [Route("Itbi_urbano_q")]
        [HttpGet]
        public ActionResult Itbi_urbano_q(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _userId = Convert.ToInt32(Session["hashid"]);
            if (_userId == 262)
                ViewBag.ReadOnly = "S";
            else
                ViewBag.ReadOnly = "N";

            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            return View(model);
        }

        [Route("Itbi_urbano_q")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_urbano_q(ItbiViewModel model, string button) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            if (button == null || button == "print")
                return Itbi_print(model.Guid, true);
            else {
                if (button == "excluir_guia") {
                    Imovel_bll imovelRepository = new Imovel_bll(_connection);
                    Exception ex = imovelRepository.Excluir_Itbi_Guia(model.Guid);
                    return RedirectToAction("Itbi_query");
                } else {
                    Itbi_gravar_guia(model);
                    model = Retorna_Itbi_Gravado(model.Guid);
                    return RedirectToAction("Itbi_query");
                }
            }
        }

        public FileResult Itbi_Download(string p, string f, int a) {
            string fullName = Server.MapPath("~");
            fullName = Path.Combine(fullName, "Files");
            fullName = Path.Combine(fullName, "Itbi");
            fullName = Path.Combine(fullName, a.ToString());
            fullName = Path.Combine(fullName, p);
            fullName = Path.Combine(fullName, f);
            fullName = fullName.Replace("\\", "/");
            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, f);
        }

        [Route("Itbi_rural_q")]
        [HttpGet]
        public ActionResult Itbi_rural_q(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            return View(model);
        }

        [Route("Itbi_rural_q")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_rural_q(ItbiViewModel model, string button) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _userId = Convert.ToInt32(Session["hashid"]);
            if (_userId == 262)
                ViewBag.ReadOnly = "S";
            else
                ViewBag.ReadOnly = "N";


            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            if (button == null || button == "print")
                return Itbi_print(model.Guid, true);
            else {
                if (button == "excluir_guia") {
                    Imovel_bll imovelRepository = new Imovel_bll(_connection);
                    Exception ex = imovelRepository.Excluir_Itbi_Guia(model.Guid);
                    return RedirectToAction("Itbi_query");
                } else {
                    Itbi_gravar_guia(model);
                    model = Retorna_Itbi_Gravado(model.Guid);
                    return RedirectToAction("Itbi_query");
                }
            }
        }

        [Route("Itbi_urbano_e")]
        [HttpGet]
        public ActionResult Itbi_urbano_e(string p = "", string a = "", int s = 0) {
            if (Session["hashid"] == null || p=="")
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Itbi_status stat = imovelRepository.Retorna_Itbi_Situacao(p);
            if (stat.Codigo > 1) {
                return RedirectToAction("Itbi_query", new { e = "A" });
            }

            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.ListaErro = new List<string>();
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();


            if (a == "rc") {//remover comprador
                Exception ex = imovelRepository.Excluir_Itbi_comprador(p, s);
            }
            if (a == "rv") {//remover vendedor
                Exception ex = imovelRepository.Excluir_Itbi_vendedor(p, s);
            }
            if (a == "ra") {//remover anexo
                Exception ex = imovelRepository.Excluir_Itbi_anexo(p, s);
            }
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            if (model.Utilizar_VVT)
                model.Valor_Venal_Territorial = model.Valor_Venal;

            int _ceptmp = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_ceptmp);
            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            int z = 1;
            foreach (string item in Lista_Tmp) {
                Lista_Logradouro.Add(new Logradouro() { Codlogradouro = z, Endereco = item.ToUpper() });
                if (Lista_Logradouro[z - 1].Endereco == model.Comprador.Logradouro_Nome)
                    model.Comprador.Logradouro_Nome = (z).ToString();

                z++;
            }

            ViewBag.Logradouro = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");


            return View("Itbi_urbano_e", model);
        }

        [Route("Itbi_rural_e")]
        [HttpGet]
        public ActionResult Itbi_rural_e(string p = "", string a = "", int s = 0) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Itbi_status stat = imovelRepository.Retorna_Itbi_Situacao(p);
            if (stat.Codigo > 1) {
                return RedirectToAction("Itbi_query", new { e = "A" });
            }

            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.ListaErro = new List<string>();
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            if (a == "rc") {//remover comprador
                Exception ex = imovelRepository.Excluir_Itbi_comprador(p, s);
            }
            if (a == "rv") {//remover vendedor
                Exception ex = imovelRepository.Excluir_Itbi_vendedor(p, s);
            }
            if (a == "ra") {//remover anexo
                Exception ex = imovelRepository.Excluir_Itbi_anexo(p, s);
            }
            ItbiViewModel model = Retorna_Itbi_Gravado(p);

            return View("Itbi_rural_e", model);
        }

        [Route("Itbi_forum")]
        [HttpGet]
        public ActionResult Itbi_forum(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            ItbiViewModel gravado = Retorna_Itbi_Gravado(p);
            List<Itbi_forum> lista = imovelRepository.Retorna_Itbi_Forum(p);
            List<Itbi_Forum> model = new List<Itbi_Forum>();
            
            if (lista.Count == 0) {
                Itbi_Forum item = new Itbi_Forum() {
                    Guid = gravado.Guid,
                    User_id = gravado.UserId,
                    User_id_Declara=gravado.UserId,
                    Tipo_Itbi = gravado.Tipo_Imovel,
                    Data_Itbi = gravado.Data_cadastro,
                    Comprador_Nome = gravado.Comprador.Nome,
                    Ano_Numero = gravado.Itbi_Numero.ToString("000000/") + gravado.Itbi_Ano.ToString()
                };
                if (gravado.Funcionario)
                    item.User_Name = sistemaRepository.Retorna_User_FullName(gravado.UserId);
                else {

                    Usuario_web uw = sistemaRepository.Retorna_Usuario_Web(gravado.UserId);
                    item.User_Name = uw.Nome;
                    item.User_Email = uw.Email;
                    item.User_Fone = uw.Telefone;
                    item.User_Name_Decalara = uw.Nome;
                    item.User_Email_Declara = uw.Email;
                    item.User_Fone_Declara = uw.Telefone;
                }
                model.Add(item);
            } else {
                foreach (Itbi_forum reg in lista) {
                    Itbi_Forum item = new Itbi_Forum() {
                        Guid = reg.Guid,
                        Seq = reg.Seq,
                        Datahora = reg.Datahora,
                        User_id = reg.Userid,
                        User_id_Declara=reg.Userid,
                        Mensagem = reg.Mensagem,
                        Tipo_Itbi = gravado.Tipo_Imovel,
                        Data_Itbi = gravado.Data_cadastro,
                        Comprador_Nome = gravado.Comprador.Nome,
                        Ano_Numero = gravado.Itbi_Numero.ToString("000000/") + gravado.Itbi_Ano.ToString()
                    };
                    if (reg.Funcionario)
                        item.User_Name = sistemaRepository.Retorna_User_FullName(reg.Userid);
                    else {
                        Usuario_web uw = sistemaRepository.Retorna_Usuario_Web(reg.Userid);
                        item.User_Name = uw.Nome;
                        item.User_Email = uw.Email;
                        item.User_Fone = uw.Telefone;
                    }
                    Usuario_web uwd = sistemaRepository.Retorna_Usuario_Web(gravado.UserId);
                    item.User_Name_Decalara = uwd.Nome;
                    item.User_Email_Declara = uwd.Email;
                    item.User_Fone_Declara = uwd.Telefone;
                    model.Add(item);
                }
            }

            return View(model);
        }

        [Route("Itbi_forum")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_forum(List<Itbi_Forum> model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ModelState.Clear();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (model[0].Action == "btnOkMsg") {
                Itbi_forum reg = new Itbi_forum() {
                    Guid = model[0].Guid,
                    Datahora = DateTime.Now,
                    Mensagem = model[0].Mensagem,
                    Userid = Convert.ToInt32(Session["hashid"]),
                    Funcionario = Session["hashfunc"].ToString() == "S" ? true : false
                    //Userid = Functions.pUserId,
                    //Funcionario = Functions.pUserGTI
                };
                Exception ex = imovelRepository.Incluir_Itbi_Forum(reg);
            }

            return RedirectToAction("Itbi_forum", new { p = model[0].Guid });
        }

        [Route("Itbi_forum_isencao")]
        [HttpGet]
        public ActionResult Itbi_forum_isencao(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            ItbiViewModel gravado = Retorna_Itbi_Isencao_Gravado(p);
            List<Itbi_forum> lista = imovelRepository.Retorna_Itbi_Forum(p);
            List<Itbi_Forum> model = new List<Itbi_Forum>();

            if (lista.Count == 0) {
                Itbi_Forum item = new Itbi_Forum() {
                    Guid = gravado.Guid,
                    User_id = gravado.UserId,
                    User_id_Declara = gravado.UserId,
                    Data_Itbi = gravado.Data_cadastro,
                    Comprador_Nome = gravado.Comprador_Nome_tmp,
                    Ano_Numero = gravado.Itbi_NumeroAno
                };
                if (gravado.Funcionario)
                    item.User_Name = sistemaRepository.Retorna_User_FullName(gravado.UserId);
                else {

                    Usuario_web uw = sistemaRepository.Retorna_Usuario_Web(gravado.UserId);
                    item.User_Name = uw.Nome;
                    item.User_Email = uw.Email;
                    item.User_Fone = uw.Telefone;
                    item.User_Name_Decalara = uw.Nome;
                    item.User_Email_Declara = uw.Email;
                    item.User_Fone_Declara = uw.Telefone;
                }
                model.Add(item);
            } else {
                foreach (Itbi_forum reg in lista) {
                    Itbi_Forum item = new Itbi_Forum() {
                        Guid = reg.Guid,
                        Seq = reg.Seq,
                        Datahora = reg.Datahora,
                        User_id = reg.Userid,
                        User_id_Declara = reg.Userid,
                        Mensagem = reg.Mensagem,
                        Tipo_Itbi = gravado.Tipo_Imovel,
                        Data_Itbi = gravado.Data_cadastro,
                        Comprador_Nome = gravado.Comprador.Nome,
                        Ano_Numero = gravado.Itbi_Numero.ToString("000000/") + gravado.Itbi_Ano.ToString()
                    };
                    if (reg.Funcionario)
                        item.User_Name = sistemaRepository.Retorna_User_FullName(reg.Userid);
                    else {
                        Usuario_web uw = sistemaRepository.Retorna_Usuario_Web(reg.Userid);
                        item.User_Name = uw.Nome;
                        item.User_Email = uw.Email;
                        item.User_Fone = uw.Telefone;
                    }
                    Usuario_web uwd = sistemaRepository.Retorna_Usuario_Web(gravado.UserId);
                    item.User_Name_Decalara = uwd.Nome;
                    item.User_Email_Declara = uwd.Email;
                    item.User_Fone_Declara = uwd.Telefone;
                    model.Add(item);
                }
            }

            return View(model);
        }



        public ItbiViewModel Itbi_Urbano_Load(ItbiViewModel model, bool _bcpf, bool _bcnpj) {
            int Codigo = Convert.ToInt32(model.Codigo);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            ImovelStruct imovel = imovelRepository.Dados_Imovel(Codigo);

            if (imovel != null && imovel.Inscricao != null) {
                model.Codigo = Codigo.ToString();
                model.Inscricao = imovel.Inscricao;
                model.Dados_Imovel = imovel;
                List<ProprietarioStruct> ListaProp = imovelRepository.Lista_Proprietario(Codigo, true);
                if (ListaProp.Count > 0) {
                    model.Dados_Imovel.Proprietario_Codigo = ListaProp[0].Codigo;
                    model.Dados_Imovel.Proprietario_Nome = ListaProp[0].Nome;
                }
                model.Matricula = imovel.NumMatricula == null ? 0 : (Int64)imovel.NumMatricula;
                if (model.Comprador == null)
                    model.Comprador = new Comprador_Itbi();

                string _cpfCnpj = model.Cpf_Cnpj;

                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
                SpCalculo _calculo = tributarioRepository.Calculo_IPTU(Codigo, DateTime.Now.Year);
                model.Valor_Venal = _calculo.Vvi;
                model.Valor_Venal_Territorial = _calculo.Vvt;

                int _codcidadao = 0;
                if (_bcpf) {
                    _codcidadao = cidadaoRepository.Existe_Cidadao_Cpf(Functions.RetornaNumero(_cpfCnpj).PadLeft(11, '0'));
                } else {
                    if (_bcnpj) {
                        _codcidadao = cidadaoRepository.Existe_Cidadao_Cnpj(Functions.RetornaNumero(_cpfCnpj).PadLeft(14, '0'));
                    }
                }
                if (_codcidadao > 0) {
                    CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codcidadao);
                    Comprador_Itbi _comprador = new Comprador_Itbi() {
                        Codigo = _cidadao.Codigo,
                        Nome = _cidadao.Nome
                    };
                    if (_bcpf)
                        _comprador.Cpf = Functions.FormatarCpfCnpj(model.Cpf_Cnpj);
                    if (_bcnpj)
                        _comprador.Cnpj = Functions.FormatarCpfCnpj(model.Cpf_Cnpj);

                    Endereco_bll enderecoRepository = new Endereco_bll(_connection);

                    if (_cidadao.EtiquetaR == "S") {
                        Bairro _bairro=null;
                        if (_cidadao.CodigoLogradouroR == null) {
                            _comprador.Bairro_Nome = _cidadao.NomeBairroR;
                        } else {
                            _bairro = enderecoRepository.RetornaLogradouroBairro((int)_cidadao.CodigoLogradouroR, (short)_cidadao.NumeroR);
                            _comprador.Bairro_Nome = _bairro.Descbairro;
                        }
                        _comprador.Logradouro_Codigo = _cidadao.CodigoLogradouroR == null ? 0 : (int)_cidadao.CodigoLogradouroR;
                        _comprador.Logradouro_Nome = _cidadao.EnderecoR;
                        _comprador.Numero = (int)_cidadao.NumeroR;
                        _comprador.Complemento = _cidadao.ComplementoR;
                        _comprador.Bairro_Codigo = _cidadao.CodigoBairroR==null?0: (int)_cidadao.CodigoBairroR;
                        
                        _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                        _comprador.Cidade_Nome = _cidadao.NomeCidadeR;
                        _comprador.UF = _cidadao.UfR;
                        _comprador.Cep = _cidadao.CepR == null ? "" : ((int)_cidadao.CepR).ToString("00000-000");
                        _comprador.Email = _cidadao.EmailR;
                        _comprador.Telefone = _cidadao.TelefoneR;
                    } else {
                        Bairro _bairro = null;
                        if (_cidadao.CodigoLogradouroC == null) {
                            _comprador.Bairro_Nome = _cidadao.NomeBairroR;
                        } else {
                            _bairro = enderecoRepository.RetornaLogradouroBairro((int)_cidadao.CodigoLogradouroC, (short)_cidadao.NumeroC);
                            _comprador.Bairro_Nome = _bairro.Descbairro;
                        }
                        _comprador.Logradouro_Codigo = _cidadao.CodigoLogradouroC == null ? 0 : (int)_cidadao.CodigoLogradouroC;
                        _comprador.Logradouro_Nome = _cidadao.EnderecoC;
                        _comprador.Numero = (int)_cidadao.NumeroC;
                        _comprador.Complemento = _cidadao.ComplementoC;
                        if(_bairro==null)
                            _comprador.Bairro_Codigo =0;
                        else
                            _comprador.Bairro_Codigo = _bairro.Codbairro;

                        _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                        _comprador.Cidade_Nome = _cidadao.NomeCidadeC;
                        _comprador.UF = _cidadao.UfC;
                        _comprador.Cep = _cidadao.CepC == null ? "" : ((int)_cidadao.CepC).ToString("00000-000");
                        _comprador.Email = _cidadao.EmailC;
                        _comprador.Telefone = _cidadao.TelefoneC;
                    }

                    model.Comprador = _comprador;
                } else {
                    if (model.Comprador == null)
                        model.Comprador = new Comprador_Itbi();
                }
            }
            return model;
        }

        public ItbiViewModel Itbi_Rural_Load(ItbiViewModel model, bool _bcpf, bool _bcnpj) {
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);

            if (model.Comprador == null)
                model.Comprador = new Comprador_Itbi();

            string _cpfCnpj = model.Cpf_Cnpj;

            int _codcidadao = 0;
            if (_bcpf) {
                _codcidadao = cidadaoRepository.Existe_Cidadao_Cpf(Functions.RetornaNumero(_cpfCnpj).PadLeft(11, '0'));
            } else {
                if (_bcnpj) {
                    _codcidadao = cidadaoRepository.Existe_Cidadao_Cnpj(Functions.RetornaNumero(_cpfCnpj).PadLeft(14, '0'));
                }
            }
            if (_codcidadao > 0) {
                CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codcidadao);
                Comprador_Itbi _comprador = new Comprador_Itbi() {
                    Codigo = _cidadao.Codigo,
                    Nome = _cidadao.Nome
                };
                if (_bcpf)
                    _comprador.Cpf = Functions.FormatarCpfCnpj(model.Cpf_Cnpj);
                if (_bcnpj)
                    _comprador.Cnpj = Functions.FormatarCpfCnpj(model.Cpf_Cnpj);

                if (_cidadao.EtiquetaR == "S") {
                    _comprador.Logradouro_Codigo = _cidadao.CodigoLogradouroR == null ? 0 : (int)_cidadao.CodigoLogradouroR;
                    _comprador.Logradouro_Nome = _cidadao.EnderecoR;
                    _comprador.Numero = (int)_cidadao.NumeroR;
                    _comprador.Complemento = _cidadao.ComplementoR;
                    _comprador.Bairro_Codigo = (int)_cidadao.CodigoBairroR;
                    _comprador.Bairro_Nome = _cidadao.NomeBairroR;
                    _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                    _comprador.Cidade_Nome = _cidadao.NomeCidadeR;
                    _comprador.UF = _cidadao.UfR;
                    _comprador.Cep = _cidadao.CepR == null ? "" : ((int)_cidadao.CepR).ToString("00000-000");
                    _comprador.Email = _cidadao.EmailR;
                    _comprador.Telefone = _cidadao.TelefoneR;
                } else {
                    _comprador.Logradouro_Codigo = _cidadao.CodigoLogradouroC == null ? 0 : (int)_cidadao.CodigoLogradouroC;
                    _comprador.Logradouro_Nome = _cidadao.EnderecoC;
                    _comprador.Numero = (int)_cidadao.NumeroC;
                    _comprador.Complemento = _cidadao.ComplementoC;
                    _comprador.Bairro_Codigo = (int)_cidadao.CodigoBairroC;
                    _comprador.Bairro_Nome = _cidadao.NomeBairroC;
                    _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                    _comprador.Cidade_Nome = _cidadao.NomeCidadeC;
                    _comprador.UF = _cidadao.UfC;
                    _comprador.Cep = _cidadao.CepC == null ? "" : ((int)_cidadao.CepC).ToString("00000-000");
                    _comprador.Email = _cidadao.EmailC;
                    _comprador.Telefone = _cidadao.TelefoneC;
                }

                model.Comprador = _comprador;
            } else {
                if (model.Comprador == null)
                    model.Comprador = new Comprador_Itbi();
            }
            return model;
        }

        private string Itbi_Save(ItbiViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            string _guid;
            string oldpos = "";
            Exception ex = null;

            //################### Grava Itbi_Main #####################

            if (model.Guid == null) {
                _guid = Guid.NewGuid().ToString("N");
                model.Guid = _guid;
                Itbi_main regMain = new Itbi_main() {
                    Imovel_codigo = Convert.ToInt32(model.Codigo == null ? "0" : model.Codigo),
                    Guid = _guid,
                    Data_cadastro = DateTime.Now,
                    Situacao_itbi = 1,
                    Userid = Convert.ToInt32(Session["hashid"]),
                    Funcionario = Session["hashfunc"].ToString() == "S" ? true : false
                };
                if (model.Dados_Imovel != null) {
                    model.Dados_Imovel.Proprietario_Codigo = model.Dados_Imovel.Proprietario_Codigo == null ? 0 : (int)model.Dados_Imovel.Proprietario_Codigo;
                    model.Dados_Imovel.Proprietario_Nome = model.Dados_Imovel.Proprietario_Nome;
                    model.Dados_Imovel.Inscricao = model.Dados_Imovel.Inscricao;
                }
                ex = imovelRepository.Incluir_Itbi_main(regMain);
            } else {
                _guid = model.Guid;
                if (model.Comprador != null && model.Comprador.Nome != null) {
                    string a = Functions.RetornaNumero(model.Comprador.Logradouro_Nome);
                    if (a != "") {
                        int b = Convert.ToInt32(a);
                        if (b > 0) {
                            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                            int _ceptmp = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                            List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_ceptmp);
                            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
                            int s = 1;
                            foreach (string item in Lista_Tmp) {
                                Lista_Logradouro.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                                s++;
                            }
                            oldpos = b.ToString();
                            if (Lista_Logradouro.Count > 0) {
                                if (b < Lista_Logradouro.Count)
                                    model.Comprador.Logradouro_Nome = Lista_Logradouro[b - 1].Endereco;
                                else
                                    model.Comprador.Logradouro_Nome = Lista_Logradouro[Lista_Logradouro.Count - 1].Endereco;
                            }
                        }
                    }
                }

                Itbi_main regMain = imovelRepository.Retorna_Itbi_Main(_guid);
                if (Functions.IsDate(model.Data_Transacao))
                    regMain.Data_Transacao = model.Data_Transacao;

                if (model.Dados_Imovel != null) {
                    regMain.Proprietario_Codigo = model.Dados_Imovel.Proprietario_Codigo == null ? 0 : (int)model.Dados_Imovel.Proprietario_Codigo;
                    regMain.Proprietario_Nome = model.Dados_Imovel.Proprietario_Nome;
                    regMain.Imovel_endereco = model.Dados_Imovel.NomeLogradouro;
                    regMain.Imovel_numero = model.Dados_Imovel.Numero == null ? 0 : (short)model.Dados_Imovel.Numero;
                    regMain.Imovel_complemento = model.Dados_Imovel.Complemento;
                    regMain.Imovel_cep = Convert.ToInt32(Functions.RetornaNumero(model.Dados_Imovel.Cep));
                    regMain.Imovel_bairro = model.Dados_Imovel.NomeBairro;
                    regMain.Imovel_Quadra = model.Dados_Imovel.QuadraOriginal;
                    regMain.Imovel_Lote = model.Dados_Imovel.LoteOriginal;
                }
                regMain.Inscricao = model.Inscricao;
                regMain.Tipo_Instrumento = model.Tipo_Instrumento;
                regMain.Valor_Venal = model.Valor_Venal;
                regMain.Valor_Avaliacao = model.Valor_Avaliacao;
                regMain.Valor_Transacao = model.Valor_Transacao;
                regMain.Tipo_Financiamento = model.Tipo_Financiamento;
                regMain.Totalidade = model.Totalidade;
                regMain.Totalidade_Perc = model.Totalidade == "Sim" ? 100 : model.Totalidade_Perc;
                regMain.Matricula = model.Matricula;
                regMain.Inscricao_Incra = model.Inscricao_Incra;
                regMain.Receita_Federal = model.Receita_Federal;
                regMain.Descricao_Imovel = model.Descricao_Imovel;
                regMain.Natureza_Codigo = model.Natureza_Codigo;
                regMain.Comprador_cpf_cnpj = model.Cpf_Cnpj;
                regMain.Comprador_codigo = model.Comprador.Codigo;
                regMain.Comprador_nome = model.Comprador.Nome;
                regMain.Comprador_logradouro_codigo = model.Comprador.Logradouro_Codigo;
                regMain.Comprador_logradouro_nome = model.Comprador.Logradouro_Nome;
                regMain.Comprador_numero = model.Comprador.Numero;
                regMain.Comprador_complemento = model.Comprador.Complemento;
                regMain.Comprador_cep = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
                regMain.Comprador_bairro_codigo = model.Comprador.Bairro_Codigo;
                regMain.Comprador_bairro_nome = model.Comprador.Bairro_Nome;
                regMain.Comprador_cidade_codigo = model.Comprador.Cidade_Codigo;
                regMain.Comprador_cidade_nome = model.Comprador.Cidade_Nome;
                regMain.Comprador_uf = model.Comprador.UF;
                regMain.Comprador_telefone = model.Comprador.Telefone;
                regMain.Comprador_email = model.Comprador.Email;
                regMain.Recursos_proprios_valor = model.Recursos_proprios_valor;
                regMain.Recursos_proprios_aliq = 3M;
                regMain.Recursos_proprios_atual = model.Recursos_proprios_atual;
                regMain.Recursos_conta_valor = model.Recursos_conta_valor;
                regMain.Recursos_conta_aliq = 3M;
                regMain.Recursos_conta_atual = model.Recursos_conta_atual;
                regMain.Recursos_concedido_valor = model.Recursos_concedido_valor;
                regMain.Recursos_concedido_aliq = 0.5M;
                regMain.Recursos_concedido_atual = model.Recursos_concedido_atual;
                regMain.Financiamento_valor = model.Financiamento_valor;
                regMain.Financiamento_aliq = 0.5M;
                regMain.Financiamento_atual = model.Financiamento_atual;
                regMain.Valor_Avaliacao = model.Valor_Avaliacao;
                regMain.Valor_Avaliacao_atual = model.Valor_Avaliacao_atual;
                regMain.Valor_guia = model.Valor_guia;
                regMain.Valor_guia_atual = model.Valor_guia_atual;
                regMain.Valor_Transacao = model.Valor_Transacao;
                regMain.Valor_Venal = model.Utilizar_VVT ? model.Valor_Venal_Territorial : model.Valor_Venal;
                regMain.Utilizar_vvt = model.Utilizar_VVT;

                ex = imovelRepository.Alterar_Itbi_Main(regMain);
            }

            //################### Grava Itbi_Comprador #####################
            
            //if (model.Lista_Comprador.Count == 0) {
            //    List<Itbi_comprador> _listaC= imovelRepository.Retorna_Itbi_Comprador(model.Guid);
            //    byte y = 0;
            //    foreach (Itbi_comprador item in _listaC) {
            //        ListCompradorEditorViewModel reg = new ListCompradorEditorViewModel() {
            //            Nome=item.Nome,
            //            Cpf_Cnpj=item.Cpf_cnpj,
            //            Seq=y
            //        };
            //        model.Lista_Comprador.Add(reg);
            //        y++;
            //    }
            //}

            ex = imovelRepository.Excluir_Itbi_comprador(model.Guid);

            List<Itbi_comprador> ListaC = new List<Itbi_comprador>();
            foreach (ListCompradorEditorViewModel comp in model.Lista_Comprador) {
                Itbi_comprador regC = new Itbi_comprador() {
                    Guid = model.Guid,
                    Seq = (byte)comp.Seq,
                    Nome = comp.Nome,
                    Cpf_cnpj = comp.Cpf_Cnpj
                };
                ListaC.Add(regC);
            }
            ex = imovelRepository.Incluir_Itbi_comprador(ListaC);

            //################### Grava Itbi_Vendedor #####################
            //if (model.Lista_Vendedor.Count == 0) {
            //    List<Itbi_vendedor> _listaV = imovelRepository.Retorna_Itbi_vendedor(model.Guid);
            //    byte y = 0;
            //    foreach (Itbi_vendedor item in _listaV) {
            //        ListVendedorEditorViewModel reg = new ListVendedorEditorViewModel() {
            //            Nome = item.Nome,
            //            Cpf_Cnpj = item.Cpf_cnpj,
            //            Seq = y
            //        };
            //        model.Lista_Vendedor.Add(reg);
            //        y++;
            //    }
            //}


            ex = imovelRepository.Excluir_Itbi_vendedor(model.Guid);

            List<Itbi_vendedor> ListaV = new List<Itbi_vendedor>();
            foreach (ListVendedorEditorViewModel vend in model.Lista_Vendedor) {
                Itbi_vendedor regV = new Itbi_vendedor() {
                    Guid = model.Guid,
                    Seq = (byte)vend.Seq,
                    Nome = vend.Nome,
                    Cpf_cnpj = vend.Cpf_Cnpj
                };
                ListaV.Add(regV);
            }
            ex = imovelRepository.Incluir_Itbi_vendedor(ListaV);
            //#########################################################
            if (oldpos != "")
                model.Comprador.Logradouro_Nome = oldpos;
            return _guid;
        }

        private ItbiViewModel Retorna_Itbi_Gravado(string guid) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            Itbi_main regMain = imovelRepository.Retorna_Itbi_Main(guid);
            ItbiViewModel itbi = new ItbiViewModel {
                Guid = regMain.Guid,
                Data_cadastro = regMain.Data_cadastro,
                Codigo = regMain.Imovel_codigo.ToString(),
                Inscricao = regMain.Inscricao,
                Proprietario_codigo = regMain.Proprietario_Codigo,
                Proprietario_nome = regMain.Proprietario_Nome,
                Natureza_Codigo = regMain.Natureza_Codigo,
                Valor_Transacao = regMain.Valor_Transacao,
                Data_Transacao = Convert.ToDateTime(regMain.Data_Transacao),
                Tipo_Financiamento = regMain.Tipo_Financiamento,
                Valor_Avaliacao = regMain.Valor_Avaliacao,
                Valor_Venal = regMain.Valor_Venal,
                Tipo_Instrumento = regMain.Tipo_Instrumento,
                Matricula = regMain.Matricula,
                Totalidade = regMain.Totalidade,
                Totalidade_Perc = regMain.Totalidade_Perc,
                Inscricao_Incra = regMain.Inscricao_Incra,
                Descricao_Imovel = regMain.Descricao_Imovel,
                Receita_Federal = regMain.Receita_Federal,
                Itbi_Ano = regMain.Itbi_Ano,
                Itbi_Numero = regMain.Itbi_Numero,
                Itbi_NumeroAno = regMain.Itbi_Numero.ToString("000000/") + regMain.Itbi_Ano.ToString(),
                Situacao_Itbi_codigo = regMain.Situacao_itbi,
                Tipo_Imovel = regMain.Inscricao == null ? "Rural" : "Urbano",
                UserId = regMain.Userid,
                Financiamento_valor = regMain.Financiamento_valor,
                Financiamento_aliq = regMain.Financiamento_aliq,
                Financiamento_atual = regMain.Financiamento_atual,
                Recursos_conta_valor = regMain.Recursos_conta_valor,
                Recursos_conta_aliq = regMain.Recursos_conta_aliq,
                Recursos_conta_atual = regMain.Recursos_conta_atual,
                Recursos_concedido_valor = regMain.Recursos_concedido_valor,
                Recursos_concedido_aliq = regMain.Recursos_concedido_aliq,
                Recursos_concedido_atual = regMain.Recursos_concedido_atual,
                Recursos_proprios_valor = regMain.Recursos_proprios_valor,
                Recursos_proprios_aliq = regMain.Recursos_proprios_aliq,
                Recursos_proprios_atual = regMain.Recursos_proprios_atual,
                Valor_Avaliacao_atual = regMain.Valor_Avaliacao_atual,
                Valor_guia = regMain.Valor_guia,
                Valor_guia_atual = regMain.Valor_guia_atual,
                Data_Vencimento = regMain.Data_Vencimento,
                Numero_Guia = regMain.Numero_Guia,
                Utilizar_VVT = regMain.Utilizar_vvt
            };
            itbi.Situacao_Itbi_Nome = imovelRepository.Retorna_Itbi_Situacao(regMain.Situacao_itbi);
            itbi.Natureza_Nome = imovelRepository.Retorna_Itbi_Natureza_nome(regMain.Natureza_Codigo);
            itbi.Tipo_Financiamento_Nome = imovelRepository.Retorna_Itbi_Financimento_nome(regMain.Tipo_Financiamento);
            if (itbi.Dados_Imovel == null)
                itbi.Dados_Imovel = new ImovelStruct();
            itbi.Dados_Imovel.NomeLogradouro = regMain.Imovel_endereco;
            itbi.Dados_Imovel.Numero = (short)regMain.Imovel_numero;
            itbi.Dados_Imovel.Complemento = regMain.Imovel_complemento;
            itbi.Dados_Imovel.Cep = regMain.Imovel_cep.ToString("00000-000");
            itbi.Dados_Imovel.NomeBairro = regMain.Imovel_bairro;
            itbi.Dados_Imovel.QuadraOriginal = regMain.Imovel_Quadra;
            itbi.Dados_Imovel.LoteOriginal = regMain.Imovel_Lote;
            itbi.Dados_Imovel.Proprietario_Nome = regMain.Proprietario_Nome;

            itbi.Cpf_Cnpj = regMain.Comprador_cpf_cnpj;
            if (itbi.Comprador == null)
                itbi.Comprador = new Comprador_Itbi();
            itbi.Comprador.Codigo = regMain.Comprador_codigo;
            itbi.Comprador.Nome = regMain.Comprador_nome;
            itbi.Comprador.Logradouro_Codigo = regMain.Comprador_logradouro_codigo;
            itbi.Comprador.Logradouro_Nome = regMain.Comprador_logradouro_nome;
            itbi.Comprador.Numero = regMain.Comprador_numero;
            itbi.Comprador.Complemento = regMain.Comprador_complemento;
            itbi.Comprador.Bairro_Codigo = regMain.Comprador_bairro_codigo;
            itbi.Comprador.Bairro_Nome = regMain.Comprador_bairro_nome;
            itbi.Comprador.Cidade_Codigo = regMain.Comprador_cidade_codigo;
            itbi.Comprador.Cidade_Nome = regMain.Comprador_cidade_nome;
            itbi.Comprador.UF = regMain.Comprador_uf;
            itbi.Comprador.Cep = regMain.Comprador_cep.ToString("00000-000");
            itbi.Comprador.Telefone = regMain.Comprador_telefone;
            itbi.Comprador.Email = regMain.Comprador_email;

            if (itbi.Inscricao == null && Convert.ToInt32(itbi.Codigo) > 0) {
                itbi.Inscricao = imovelRepository.Retorna_Imovel_Inscricao(Convert.ToInt32(itbi.Codigo));
            }

            List<ListCompradorEditorViewModel> Lista_comprador = new List<ListCompradorEditorViewModel>();
            List<Itbi_comprador> listaC = imovelRepository.Retorna_Itbi_Comprador(guid);
            foreach (Itbi_comprador item in listaC) {
                ListCompradorEditorViewModel itemC = new ListCompradorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_comprador.Add(itemC);
            }
            itbi.Lista_Comprador = Lista_comprador;

            List<ListVendedorEditorViewModel> Lista_vendedor = new List<ListVendedorEditorViewModel>();
            List<Itbi_vendedor> listaV = imovelRepository.Retorna_Itbi_vendedor(guid);
            foreach (Itbi_vendedor item in listaV) {
                ListVendedorEditorViewModel itemV = new ListVendedorEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Nome,
                    Cpf_Cnpj = item.Cpf_cnpj
                };
                Lista_vendedor.Add(itemV);
            }
            itbi.Lista_Vendedor = Lista_vendedor;


            List<ListAnexoEditorViewModel> Lista_Anexo = new List<ListAnexoEditorViewModel>();
            List<Itbi_anexo> listaA = imovelRepository.Retorna_Itbi_Anexo(guid);
            foreach (Itbi_anexo item in listaA) {
                ListAnexoEditorViewModel itemA = new ListAnexoEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Descricao,
                    Arquivo = item.Arquivo
                };
                Lista_Anexo.Add(itemA);
            }
            itbi.Lista_Anexo = Lista_Anexo;

            return itbi;
        }

        private List<string> Itbi_Valida(ItbiViewModel model) {
            List<string> Lista = new List<string>();

            if (model.Natureza_Codigo == 0) {
                Lista.Add("Natureza da transação não informada");
            }
            if (string.IsNullOrWhiteSpace(model.Cpf_Cnpj)) {
                Lista.Add("Cpf/Cnpj do comprador não informado ou inválido");
            }
            if (string.IsNullOrWhiteSpace(model.Comprador.Nome)) {
                Lista.Add("Nome do comprador não informado");
            }
            if (Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep)) == 0) {
                Lista.Add("Cep do comprador não informado");
            }

            if (string.IsNullOrWhiteSpace(model.Comprador.Logradouro_Nome)) {
                Lista.Add("Endereço do comprador não informado");
            }
            if (model.Valor_Transacao == 0) {
                Lista.Add("Valor da transação não informado");
            }
            if (model.Valor_Venal == 0 && model.Valor_Venal_Territorial == 0) {
                Lista.Add("Valor venal/ITR não informado");
            }
            if (!Functions.IsDate(model.Data_Transacao)) {
                Lista.Add("Data da transação inválida");
            }

            if (model.Data_Transacao != null && model.Data_Transacao.Value.Year < 1960) {
                Lista.Add("Data da transação inválida");
            }

            if (model.Totalidade == "Não" && model.Totalidade_Perc == 0) {
                Lista.Add("Informe a proporção do imóvel a ser transmitida");
            }

            if (model.Matricula == 0) {
                Lista.Add("Informe a matrícula/transcrição do imóvel");
            }

            if (model.Lista_Anexo.Count == 0) {
                Lista.Add("Nenhum documento foi anexado");
            }

            return Lista;
        }

        byte[] GetFile(string s) {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }

        public ActionResult Itbi_guia(string p) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Itbi_status stat = imovelRepository.Retorna_Itbi_Situacao(p);
            if (stat.Codigo != 2) {
                return RedirectToAction("Itbi_query", new { e = "P" });
            } else {
                Itbi_main _itbi = imovelRepository.Retorna_Itbi_Main(p);
                string _data1 =Convert.ToDateTime(_itbi.Data_Vencimento).ToString("dd/MM/yyyy");
                string _data2 =DateTime.Now.ToString("dd/MM/yyyy");
                if (Convert.ToDateTime( _data1) < Convert.ToDateTime(_data2) ){
                    return RedirectToAction("Itbi_query", new { e = "V" });
                }

                Itbi_Guia _guia = new Itbi_Guia() {
                    Guid = _itbi.Guid,
                    Inscricao = _itbi.Inscricao ?? "",
                    Imovel_Codigo = _itbi.Imovel_codigo,
                    Imovel_Endereco = _itbi.Imovel_endereco ?? "",
                    Imovel_Numero = _itbi.Imovel_numero,
                    Imovel_Complemento = _itbi.Imovel_complemento ?? "",
                    Imovel_Bairro = _itbi.Imovel_bairro ?? "",
                    Imovel_Cep = _itbi.Imovel_cep,
                    Imovel_Lote = _itbi.Imovel_Lote ?? "",
                    Imovel_Quadra = _itbi.Imovel_Quadra ?? "",
                    Proprietario_Nome = _itbi.Proprietario_Nome ?? "",
                    Itbi_Ano = _itbi.Itbi_Ano,
                    Itbi_Numero = _itbi.Itbi_Numero,
                    Data_Cadastro = _itbi.Data_cadastro,
                    Data_Transacao = Convert.ToDateTime(_itbi.Data_Transacao),
                    Comprador_Codigo = _itbi.Comprador_codigo,
                    Comprador_Nome = _itbi.Comprador_nome,
                    Comprador_Cpf_Cnpj = _itbi.Comprador_cpf_cnpj,
                    Comprador_Logradouro = _itbi.Comprador_logradouro_nome,
                    Comprador_Numero = _itbi.Comprador_numero,
                    Comprador_Complemento = _itbi.Comprador_complemento ?? "",
                    Comprador_Bairro = _itbi.Comprador_bairro_nome ?? "",
                    Comprador_Cep = _itbi.Comprador_cep,
                    Comprador_Cidade = _itbi.Comprador_cidade_nome,
                    Comprador_Uf = _itbi.Comprador_uf,
                    Inscricao_Incra = _itbi.Inscricao_Incra ?? "",
                    Receita_Federal = _itbi.Receita_Federal ?? "",
                    Descricao_Imovel = _itbi.Descricao_Imovel ?? "",
                    Matricula = _itbi.Matricula,
                    Valor_Avaliacao = _itbi.Valor_Avaliacao,
                    Valor_Guia = _itbi.Valor_guia_atual > 0 ? _itbi.Valor_guia_atual : _itbi.Valor_guia,
                    Valor_Transacao = _itbi.Valor_Transacao,
                    Valor_Venal = _itbi.Valor_Venal,
                    Recursos_proprios_Valor = _itbi.Recursos_proprios_valor,
                    Recursos_proprios_Atual = _itbi.Recursos_proprios_atual,
                    Recursos_conta_Valor = _itbi.Recursos_conta_valor,
                    Recursos_conta_Atual = _itbi.Recursos_conta_atual,
                    Recursos_concedido_Valor = _itbi.Recursos_concedido_valor,
                    Recursos_concedido_Atual = _itbi.Recursos_concedido_atual,
                    Tipo_Instrumento = _itbi.Tipo_Instrumento,
                    Financiamento_Valor = _itbi.Financiamento_valor,
                    Financiamento_Atual = _itbi.Financiamento_atual,
                    Totalidade = _itbi.Totalidade,
                    Totalidade_Perc = _itbi.Totalidade_Perc,
                    Data_Vencimento = Convert.ToDateTime(_itbi.Data_Vencimento),
                    Natureza = imovelRepository.Retorna_Itbi_Natureza_nome(_itbi.Natureza_Codigo),
                    Tipo_Financiamento = imovelRepository.Retorna_Itbi_Financimento_nome(_itbi.Tipo_Financiamento),
                    Valor_Avaliacao_Atual = _itbi.Valor_Avaliacao_atual,
                    Valor_Guia_Atual = _itbi.Valor_guia_atual > 0 ? _itbi.Valor_guia_atual : _itbi.Valor_guia,
                };
                _guia.Numero_guia = _itbi.Numero_Guia;
                _guia.Nosso_Numero = "287353200" + _guia.Numero_guia.ToString();

                string _convenio = "2873532";
                //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
                DateTime _data_base = Convert.ToDateTime("07/10/1997");
                TimeSpan ts = Convert.ToDateTime(_guia.Data_Vencimento) - _data_base;
                int _fator_vencto = ts.Days;
                string _quinto_grupo = String.Format("{0:D4}", _fator_vencto);
                string _valor_boleto_str = string.Format("{0:0.00}", _guia.Valor_Guia);
                _quinto_grupo += string.Format("{0:D10}", Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
                string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}", Convert.ToInt32(_convenio));
                _barra += String.Format("{0:D10}", Convert.ToInt64(_guia.Numero_guia)) + "17";
                string _campo1 = "0019" + _barra.Substring(19, 5);
                string _digitavel = _campo1 + Functions.Calculo_DV10(_campo1).ToString();
                string _campo2 = _barra.Substring(23, 10);
                _digitavel += _campo2 + Functions.Calculo_DV10(_campo2).ToString();
                string _campo3 = _barra.Substring(33, 10);
                _digitavel += _campo3 + Functions.Calculo_DV10(_campo3).ToString();
                string _campo5 = _quinto_grupo;
                string _campo4 = Functions.Calculo_DV11(_barra).ToString();
                _digitavel += _campo4 + _campo5;
                _barra = _barra.Substring(0, 4) + _campo4 + _barra.Substring(4, _barra.Length - 4);
                //**Resultado final**
                string _linha_digitavel = _digitavel.Substring(0, 5) + "." + _digitavel.Substring(5, 5) + " " + _digitavel.Substring(10, 5) + "." + _digitavel.Substring(15, 6) + " ";
                _linha_digitavel += _digitavel.Substring(21, 5) + "." + _digitavel.Substring(26, 6) + " " + _digitavel.Substring(32, 1) + " " + Functions.StringRight(_digitavel, 14);
                string _codigo_barra = Functions.Gera2of5Str(_barra);
                //**************************************************

                _guia.Codigo_Barra = _codigo_barra;
                _guia.Linha_Digitavel = _linha_digitavel;

                Exception ex = imovelRepository.Incluir_Itbi_Guia(_guia);
                List<Itbi_Guia> Lista = new List<Itbi_Guia>();
                Lista.Add(_guia);

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty, encoding = string.Empty, extension = string.Empty;
                DataSet Ds = Functions.ToDataSet(Lista);
                ReportDataSource rdsAct = new ReportDataSource("dsGuia_Itbi", Ds.Tables[0]);
                ReportViewer viewer = new ReportViewer();
                viewer.LocalReport.Refresh();
                if (_itbi.Imovel_codigo > 0)
                    viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Boleto_ITBI.rdlc");
                else
                    viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Boleto_ITBI_R.rdlc");
                viewer.LocalReport.DataSources.Add(rdsAct);

                byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename= guia_itbi" + "." + extension);
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.Flush();
                Response.End();
                ViewBag.Erro = "M";
                return RedirectToAction("Itbi_query");
            }
        }

        public ActionResult Itbi_print(string p, bool u) {
            ReportDocument rd = new ReportDocument();
            if (u)
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Itbi_Main.rpt"));
            else
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Itbi_Rural.rpt"));
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

            try {
                rd.RecordSelectionFormula = "{itbi_main.guid}='" + p + "'";
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Resumo_Itbi.pdf");
            } catch  {
                throw;
            }
        }

        public ActionResult Itbi_cancel(string p, int s) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Itbi_status stat = imovelRepository.Retorna_Itbi_Situacao(p);
            if (stat.Codigo > 1) {
                return RedirectToAction("Itbi_query", new { e = "C" });
            } else {
                Exception ex = imovelRepository.Alterar_Itbi_Situacao(p, s);
                return RedirectToAction("Itbi_query");
            }
        }

        public int Grava_Cidadao(ItbiViewModel model) {
            int _codigo = 0, _numero = model.Comprador.Numero;
            string _nome = model.Comprador.Nome;
            if (string.IsNullOrWhiteSpace(_nome)) return 0;
            int _endereco_codigo = model.Comprador.Logradouro_Codigo;
            string _endereco_nome = model.Comprador.Logradouro_Nome;
            string _complemento = model.Comprador.Complemento;
            string _cpf = Functions.RetornaNumero(model.Cpf_Cnpj == null || model.Cpf_Cnpj.Length > 14 ? "" : model.Cpf_Cnpj);
            string _cnpj = Functions.RetornaNumero(model.Cpf_Cnpj == null || model.Cpf_Cnpj.Length == 14 ? "" : model.Cpf_Cnpj);
            int _bairro_codigo = model.Comprador.Bairro_Codigo;
            string _bairro_nome = model.Comprador.Bairro_Nome;
            int _cidade_codigo = model.Comprador.Cidade_Codigo;
            string _cidade_nome = model.Comprador.Cidade_Nome;
            int _cep = Convert.ToInt32(Functions.RetornaNumero(model.Comprador.Cep));
            string _uf = model.Comprador.UF;
            string _fone = model.Comprador.Telefone;
            string _email = model.Comprador.Email;

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);

            if (_cidade_codigo == 0) {
                _cidade_codigo = enderecoRepository.Retorna_Cidade(_uf, _cidade_nome);
            }

            if (_bairro_codigo == 0) {
                bool _existe = enderecoRepository.Existe_Bairro(_uf, _cidade_codigo, _bairro_nome);
                if (_existe) {
                    _bairro_codigo = enderecoRepository.Retorna_Bairro(_uf, _cidade_codigo, _bairro_nome);
                } else {
                    Bairro regBairro = new Bairro() {
                        Siglauf = _uf,
                        Codcidade = (short)_cidade_codigo,
                        Descbairro = _bairro_nome
                    };
                    _bairro_codigo = enderecoRepository.Incluir_bairro(regBairro);
                }
            }

            Cidadao regCidadao;
            LogradouroStruct _log = new LogradouroStruct();
            if (_cidade_codigo == 413) {
                if (_endereco_codigo == 0) {
                    _log = enderecoRepository.Retorna_Logradouro_Cep(_cep);
                } else {
                    _log.CodLogradouro = _endereco_codigo;
                }
            } else {
                _log.CodLogradouro = 0;
            }
            regCidadao = new Cidadao() {
                Nomecidadao = _nome,
                Cpf = _cpf,
                Cnpj = _cnpj,
                Cep = _cep,
                Codlogradouro = _log.CodLogradouro,
                Nomelogradouro = _log.Endereco,
                Numimovel = (short)_numero,
                Complemento = _complemento ?? "",
                Codbairro = (short)_bairro_codigo,
                Codcidade = (short)_cidade_codigo,
                Siglauf = _uf,
                Telefone = _fone ?? "",
                Email = _email ?? "",
                Etiqueta = "S"
            };
            if (_endereco_codigo == 0)
                regCidadao.Nomelogradouro = _endereco_nome;

            _codigo = cidadaoRepository.Incluir_Cidadao_Itbi(regCidadao);

            return _codigo;
        }

        public DateTime Retorna_Data_Vencimento_Itbi() {
            DateTime _data = DateTime.Now;
            _data = _data.AddDays(30);
            //_data = new DateTime(2021,1,11);
            if (_data.DayOfWeek == DayOfWeek.Saturday)
                _data.AddDays(2);
            else {
                if (_data.DayOfWeek == DayOfWeek.Sunday)
                    _data.AddDays(1);
            }

            return _data;
        }

        private void Itbi_gravar_guia(ItbiViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _fiscal = Convert.ToInt32(Session["hashid"]);
            //int _fiscal = Functions.pUserId;
            int _codigo = Convert.ToInt32(model.Comprador.Codigo);
            short _ano = (short)DateTime.Now.Year;
            short _seq = tributarioRepository.Retorna_Proxima_Seq_Itbi(_codigo, _ano);
            DateTime _dataVencto = Retorna_Data_Vencimento_Itbi();
            string _nome = model.Comprador.Nome.Length > 40 ? model.Comprador.Nome.Substring(0, 40) : model.Comprador.Nome;
            string _endereco = model.Comprador.Logradouro_Nome.Length > 40 ? model.Comprador.Logradouro_Nome.Substring(0, 40) : model.Comprador.Logradouro_Nome;
            string _bairro = model.Comprador.Bairro_Nome ?? "";
            _bairro = _bairro.Length > 40 ? _bairro.Substring(0, 40) : _bairro;
            string _cidade = "JABOTICABAL";
            if (model.Comprador.Cidade_Nome != null)
                _cidade = model.Comprador.Cidade_Nome.Length > 40 ? model.Comprador.Cidade_Nome.Substring(0, 40) : model.Comprador.Cidade_Nome;

            //grava parcela
            Debitoparcela regParcela = new Debitoparcela {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 36,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Statuslanc = 3,
                Datavencimento = _dataVencto,
                Datadebase = DateTime.Now,
                Userid = _fiscal
            };
            Exception ex = tributarioRepository.Insert_Debito_Parcela(regParcela);

            //grava tributo
            Debitotributo regTributo = new Debitotributo {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 36,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Codtributo = 84,
                Valortributo = model.Valor_guia_atual == 0 ? model.Valor_guia : model.Valor_guia_atual
            };
            Exception ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);

            //grava o documento
            Numdocumento regDoc = new Numdocumento();
            regDoc.Valorguia = model.Valor_guia_atual == 0 ? model.Valor_guia : model.Valor_guia_atual;
            regDoc.Emissor = "Gti.Web/ITBI";
            regDoc.Datadocumento = DateTime.Now;
            regDoc.Registrado = false;
            regDoc.Percisencao = 0;
            regDoc.Percisencao = 0;
            int _novo_documento = tributarioRepository.Insert_Documento(regDoc);
            model.Numero_Guia = _novo_documento;
            model.Data_Vencimento = _dataVencto;

            //grava o documento na parcela
            Parceladocumento regParc = new Parceladocumento();
            regParc.Codreduzido = _codigo;
            regParc.Anoexercicio = _ano;
            regParc.Codlancamento = 36;
            regParc.Seqlancamento = _seq;
            regParc.Numparcela = 1;
            regParc.Codcomplemento = 0;
            regParc.Numdocumento = _novo_documento;
            regParc.Valorjuros = 0;
            regParc.Valormulta = 0;
            regParc.Valorcorrecao = 0;
            regParc.Plano = 0;
            tributarioRepository.Insert_Parcela_Documento(regParc);

            //Alterar a observação da parcela
            Obsparcela ObsReg = new Obsparcela() {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 36,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Obs = "Referente ao ITBI nº" + model.Itbi_Numero.ToString("000000") + "/" + model.Itbi_Ano.ToString(),
                Userid = _fiscal,
                Data = DateTime.Now
            };
            ex = tributarioRepository.Insert_Observacao_Parcela(ObsReg);

            //Enviar para registrar 
            Ficha_compensacao_documento ficha = new Ficha_compensacao_documento();
            ficha.Nome = _nome;
            ficha.Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco;
            ficha.Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro;
            ficha.Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade;
            ficha.Cep = Functions.RetornaNumero(model.Comprador.Cep) ?? "14870000";
            ficha.Cpf = Functions.RetornaNumero(model.Cpf_Cnpj);
            ficha.Numero_documento = _novo_documento;
            ficha.Data_vencimento = _dataVencto;
            ficha.Valor_documento = Convert.ToDecimal(model.Valor_guia_atual == 0 ? model.Valor_guia : model.Valor_guia_atual);
            ficha.Uf = model.Comprador.UF;
            ex = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
            ex = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);

            //Alterar Itbi
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            ex = imovelRepository.Alterar_Itbi_Guia(model.Guid, _novo_documento, _dataVencto, _fiscal);
            ex = imovelRepository.Alterar_Itbi_Situacao(model.Guid, 2);
            return;
        }

        [Route("Itbi_isencao")]
        [HttpGet]
        public ActionResult Itbi_isencao(string guid, string a, int s = 0, string natureza = "27") {
            Session["hashform"] = "16";
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            ItbiViewModel model = new ItbiViewModel();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (String.IsNullOrWhiteSpace(guid)) {
                bool bFuncionario = Session["hashfunc"].ToString() == "S" ? true : false;
                int nId = Convert.ToInt32(Session["hashid"]);
                string usuario_Nome = "", usuario_Doc = "";
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                if (bFuncionario) {
                    usuarioStruct _user = sistemaRepository.Retorna_Usuario(nId);
                    usuario_Nome = _user.Nome_completo;
                } else {
                    Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(nId);
                    usuario_Nome = _user.Nome;
                    usuario_Doc = Functions.FormatarCpfCnpj(_user.Cpf_Cnpj);
                }

                model.UserId = Convert.ToInt32(Session["hashid"]);
                model.Guid = Guid.NewGuid().ToString("N");
                model.Data_cadastro = DateTime.Now;

                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);

                Itbi_isencao_main regMain = new Itbi_isencao_main() {
                    Guid = model.Guid,
                    Data_cadastro = DateTime.Now,
                    Situacao = 1,
                    Fiscal_id = 0,
                    Usuario_nome = usuario_Nome,
                    Usuario_doc = usuario_Doc,
                    Natureza = 0,
                    Isencao_ano = 0,
                    Isencao_numero = 0,
                    Usuario_id = nId
                };
                Exception ex = imovelRepository.Incluir_isencao_main(regMain);
            } else {
                if (a == "rv") {//remover imóvel
                    Exception ex = imovelRepository.Excluir_Itbi_Isencao_Imovel(guid, s);
                }

                model = Retorna_Itbi_Isencao_Gravado(guid);
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
            }
            model.Lista_Anexo = new List<ListAnexoEditorViewModel>();
            return View(model);
        }

        [Route("Itbi_isencao")]
        [HttpPost]
        public ActionResult Itbi_isencao(ItbiViewModel model, string natureza, HttpPostedFileBase file, string action, int seq = 0) {
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (action == "btnValida") {
                Exception ex2 = imovelRepository.Alterar_Itbi_Isencao_Natureza(model.Guid, Convert.ToInt32(natureza));
                goto ActionPos;
            } else {
                if (action == "btnAnexoAdd") {
                    if (file != null){
                        if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                            ViewBag.Result = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                            return View(model);
                        } else {
                            if (file.ContentType != "application/pdf") {
                                ViewBag.Result = "* Este tipo de arquivo não pode ser enviado como anexo.";
                                return View(model);
                            } else {
                                string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                                string _path = "~/Files/Itbi/" + _ano + "/";
                                var fileName = Path.GetFileName(file.FileName);
                                fileName = fileName.RemoveDiacritics();
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);
                                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                                file.SaveAs(path);
                                byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                                Itbi_anexo regA = new Itbi_anexo() {
                                    Guid = model.Guid,
                                    Seq = seqA,
                                    Descricao = model.Anexo_Desc_tmp,
                                    Arquivo = fileName
                                };
                                Exception ex2 = imovelRepository.Incluir_Itbi_Anexo(regA);
                                ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                    Seq = model.Lista_Anexo.Count,
                                    Arquivo = fileName,
                                    Nome = model.Anexo_Desc_tmp
                                };
                                model.Lista_Anexo.Add(Anexo);
                                goto ActionPos;
                            }
                        }
                    } else {
                        ViewBag.Result = "* Nenhum arquivo selecionado.";
                        goto ActionPos;
                    }
                }

            }

            List<Itbi_anexo> Lista_Anexo = imovelRepository.Retorna_Itbi_Anexo(model.Guid);
            model.Lista_Anexo.Clear();
            foreach (Itbi_anexo itemA in Lista_Anexo) {
                ListAnexoEditorViewModel regA = new ListAnexoEditorViewModel() {
                    Seq = itemA.Seq,
                    Nome = itemA.Descricao,
                    Arquivo = itemA.Arquivo
                };
                model.Lista_Anexo.Add(regA);
            }

            int _codigo = Convert.ToInt32(model.Vendedor_Cpf_cnpj_tmp);
            
            model.Tipo_Imovel = _codigo == 0 ? "Rural" : "Urbano";
            bool _urbano = model.Tipo_Imovel == "Urbano";
            List<Itbi_isencao_imovel> Lista = imovelRepository.Retorna_Itbi_Isencao_Imovel(model.Guid);
            if (_urbano && !imovelRepository.Existe_Imovel(_codigo)) {
                ViewBag.Result = "Imóvel não cadastrado.";
                model = Retorna_Itbi_Isencao_Gravado(model.Guid);
                model.Vendedor_Cpf_cnpj_tmp = "";
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
                return View(model);
            } else {
                if (_urbano) {
                    ImovelStruct imovel = imovelRepository.Dados_Imovel(_codigo);
                    model.Codigo = imovel.Codigo.ToString("00000");
                    model.Inscricao = imovel.Inscricao;
                    model.Dados_Imovel = imovel;
                    decimal _somaarea = imovelRepository.Soma_Area(_codigo);
                    bool _predial = _somaarea > 0;
                    string _descricao = "";
                    Imovel_Isencao reg = new Imovel_Isencao() {
                        Tipo = model.Tipo_Imovel,
                        Codigo = _codigo.ToString()
                    };
                    if (_predial)
                        _descricao = "um imóvel ";
                    else
                        _descricao = "um terreno ";

                    _descricao += "com " + imovel.Area_Terreno.ToString("#.#0") + "m² localizado no(a) " + imovel.NomeLogradouroAbreviado + ", " + imovel.Numero.ToString();
                    if (imovel.Complemento != "")
                        _descricao += " " + imovel.Complemento;
                    if (imovel.QuadraOriginal != "")
                        _descricao += " Quadra: " + imovel.QuadraOriginal + " Lote: " + imovel.LoteOriginal;

                    _descricao += " no bairro " + imovel.NomeBairro + " na cidade de JABOTICABAL/SP, ";
                    _descricao += "inscrição municipal: " + imovel.Inscricao + ", cadastro municipal: " + imovel.Codigo.ToString() + ",";
                    reg.Descricao = _descricao;
                    model.Lista_Isencao.Add(reg);
                } else {
                    model.Codigo = "0";
                    model.Inscricao = "";
                    decimal _somaarea = 0;
                    bool _predial = _somaarea > 0;
                    string _descricao = model.Descricao_Imovel;
                    Imovel_Isencao reg = new Imovel_Isencao() {
                        Tipo = model.Tipo_Imovel,
                        Codigo = _codigo.ToString(),
                        Descricao = model.Vendedor_Nome_tmp
                    };

                    model.Lista_Isencao.Add(reg);
                }
            }

            byte y = 1;
            foreach (Itbi_isencao_imovel i in Lista) {
                i.Seq = y;
                y++;
            }
            foreach (Imovel_Isencao item in model.Lista_Isencao) {
                if (item != null) {
                    Itbi_isencao_imovel regC = new Itbi_isencao_imovel() {
                        Guid = model.Guid,
                        Seq = y,
                        Tipo = item.Tipo,
                        Codigo = Convert.ToInt32(item.Codigo),
                        Descricao = item.Descricao
                    };
                    Lista.Add(regC);
                    y++;
                }
            }
            Exception ex = imovelRepository.Incluir_Itbi_isencao_imovel(Lista);
            if (ex != null)
                throw ex;

            Itbi_isencao_main regM = new Itbi_isencao_main();
            regM.Guid = model.Guid;
            regM.Natureza = Convert.ToInt32(natureza);
            if (regM.Isencao_numero == 0) {
                regM.Isencao_numero = imovelRepository.Retorna_Itbi_Isencao_Disponivel();
                regM.Isencao_ano = (short)DateTime.Now.Year;
            }

            ex = imovelRepository.Alterar_Itbi_Isencao(regM);
        ActionPos:

            if (action == "btnValida")
                return RedirectToAction("itbi_isencao_ok");
            else {
                model = Retorna_Itbi_Isencao_Gravado(model.Guid);
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
                model.Vendedor_Cpf_cnpj_tmp = null;
                model.Vendedor_Nome_tmp = null;
                return View(model);
            }
        }

        private ItbiViewModel Retorna_Itbi_Isencao_Gravado(string guid) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);

            Itbi_isencao_main_Struct regMain = imovelRepository.Retorna_Itbi_Isencao_Main(guid);
            ItbiViewModel itbi = new ItbiViewModel {
                Guid = regMain.Guid,
                Data_cadastro = regMain.Data_cadastro,
                Natureza_Isencao_Codigo=regMain.Natureza,
                Itbi_Ano=regMain.Isencao_ano,
                Itbi_NumeroAno = regMain.Isencao_numero.ToString("00000") + "/" + regMain.Isencao_ano.ToString(),
                Natureza_Nome = regMain.Natureza_Nome,
                Situacao_Itbi_codigo = regMain.Situacao,
                Situacao_Itbi_Nome = regMain.Situacao_Nome,
                Comprador_Nome_tmp = regMain.Usuario_nome,
                UserId=regMain.Usuario_id
            };

            List<Itbi_isencao_imovel> ListaImovel = imovelRepository.Retorna_Itbi_Isencao_Imovel(guid);
            List<Imovel_Isencao> ListaIsencao = new List<Imovel_Isencao>();
            foreach (Itbi_isencao_imovel item in ListaImovel) {
                Imovel_Isencao reg = new Imovel_Isencao() {
                    Seq = item.Seq,
                    Codigo = item.Codigo.ToString(),
                    Tipo = item.Tipo,
                    Descricao = item.Descricao
                };
                ListaIsencao.Add(reg);
            }
            itbi.Lista_Isencao = ListaIsencao;
            
            List<ListAnexoEditorViewModel> Lista_Anexo = new List<ListAnexoEditorViewModel>();
            List<Itbi_anexo> listaA = imovelRepository.Retorna_Itbi_Anexo(guid);
            foreach (Itbi_anexo item in listaA) {
                ListAnexoEditorViewModel itemA = new ListAnexoEditorViewModel() {
                    Seq = item.Seq,
                    Nome = item.Descricao,
                    Arquivo = item.Arquivo
                };
                Lista_Anexo.Add(itemA);
            }
            itbi.Lista_Anexo = Lista_Anexo;
            return itbi;
        }

        private List<SelectListItem> Lista_Natureza_Isencao(string selectedValue) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<SelectListItem> Lista = new List<SelectListItem>();
            List<Itbi_natureza_isencao> lista_natureza_isencao = imovelRepository.Lista_itbi_natureza_isencao();

            List<SelectListGroup> _lista_grupo = new List<SelectListGroup>();
            foreach (var item in lista_natureza_isencao) {
                bool _find = false;
                foreach (var itemGrupo in _lista_grupo) {
                    if (item.Artigo == itemGrupo.Name) {
                        _find = true;
                        break;
                    }
                }
                if (!_find) {
                    SelectListGroup _grupo = new SelectListGroup() { Name = item.Artigo };
                    _lista_grupo.Add(_grupo);
                }
            }

            foreach (Itbi_natureza_isencao temp in lista_natureza_isencao) {
                foreach (SelectListGroup _grupo in _lista_grupo) {
                    if (temp.Artigo == _grupo.Name) {
                        SelectListItem item = new SelectListItem() {
                            Group = _grupo,
                            Text = temp.Descricao,
                            Value = temp.Codigo.ToString(),
                            Selected = temp.Codigo.ToString() == selectedValue ? true : false
                        };
                        Lista.Add(item);
                        break;
                    }
                }
            }
            return Lista;
        }

        [Route("Itbi_query_isencao")]
        [HttpGet]
        public ActionResult Itbi_query_isencao(string e = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _fiscal = Session["hashfiscalitbi"] != null && Session["hashfiscalitbi"].ToString() == "S" ? true : false;
            List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Isencao_Query(_userId, _fiscal, 0,2020);
            List<ItbiViewModel> model = new List<ItbiViewModel>();
            foreach (Itbi_Lista reg in Lista) {
                ItbiViewModel item = new ItbiViewModel() {
                    Guid = reg.Guid,
                    Data_cadastro = Convert.ToDateTime(reg.Data.ToString("dd/MM/yyyy")),
                    Itbi_NumeroAno = reg.Numero_Ano,
                    Tipo_Imovel = reg.Tipo,
                    Comprador_Nome_tmp = Functions.TruncateTo(reg.Nome_Requerente, 24),
                    Situacao_Itbi_Nome = reg.Situacao,
                    Situacao_Itbi_codigo = reg.Situacao_Codigo
                };
                if (!string.IsNullOrEmpty(reg.Validade))
                    item.Data_Validade = Convert.ToDateTime(reg.Validade).ToString("dd/MM/yyyy");
                model.Add(item);
            }
            ViewBag.Erro = e;
            return View(model);
        }

        [Route("Itbi_query_isencao")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Itbi_query_isencao(List<ItbiViewModel> model) {
            string _ano = model[0].Ano_Selected ?? "";
            if (_ano == "")
                _ano = DateTime.Now.Year.ToString();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _fiscal = Session["hashfiscalitbi"] != null && Session["hashfiscalitbi"].ToString() == "S" ? true : false;
            List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Isencao_Query(_userId, _fiscal, Convert.ToInt32(model[0].Status_Query), Convert.ToInt32(_ano));
            model.Clear();
            foreach (Itbi_Lista reg in Lista) {
                ItbiViewModel item = new ItbiViewModel() {
                    Guid = reg.Guid,
                    Data_cadastro = Convert.ToDateTime(reg.Data.ToString("dd/MM/yyyy")),
                    Itbi_NumeroAno = reg.Numero_Ano,
                    Tipo_Imovel = reg.Tipo,
                    Comprador_Nome_tmp = Functions.TruncateTo(reg.Nome_Requerente, 24),
                    Situacao_Itbi_Nome = reg.Situacao,
                    Situacao_Itbi_codigo = reg.Situacao_Codigo
            };
                if (!string.IsNullOrEmpty(reg.Validade))
                    item.Data_Validade = Convert.ToDateTime(reg.Validade).ToString("dd/MM/yyyy");
                model.Add(item);
            }
            return View(model);
        }


        [Route("Itbi_isencao_q")]
        [HttpGet]
        public ActionResult Itbi_isencao_q(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"] == null ? "N" : Session["hashfiscalitbi"].ToString();
            ItbiViewModel model = Retorna_Itbi_Isencao_Gravado(p);
            return View(model);
        }

        [Route("Itbi_isencao_q")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Itbi_isencao_q(ItbiViewModel model, string button) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Liberar_Itbi_Isencao(model.Guid, Convert.ToInt32(Session["hashid"]));
            return RedirectToAction("Itbi_query_isencao");
        }

        public ActionResult Itbi_isencao_cancel(string p, int s) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Alterar_Itbi_Isencao_Situacao(p, 4);
            return RedirectToAction("Itbi_query_isencao");
        }

        public ActionResult Itbi_isencao_print(string p) {
            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Isencao_Itbi.rpt"));
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

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            //##### QRCode ##########################################################

   
   //         string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkguid?c=" + asciiString;
            string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared//Checkguid?c=" + HttpUtility.UrlEncode(p);
            ViewBag.Code = Code;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    Exception ex = imovelRepository.Alterar_Itbi_Isencao_QRCode(p, byteImage);
                }
            }

            Itbi_isencao_main_Struct reg = imovelRepository.Retorna_Itbi_Isencao_Main(p);
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Assinatura assinatura = sistemaRepository.Retorna_Usuario_Assinatura(reg.Fiscal_id);
            usuarioStruct usuario = sistemaRepository.Retorna_Usuario(reg.Fiscal_id);
            rd.SetParameterValue("ANONUMERO", reg.Isencao_numero.ToString("00000") + "/" + reg.Isencao_ano.ToString("0000"));
            rd.SetParameterValue("NATUREZA", reg.Natureza_Nome);
            rd.SetParameterValue("NOMEFISCAL", usuario.Nome_completo);
            rd.SetParameterValue("CARGO", assinatura.Cargo);

            string imovel = "";
            List<Itbi_isencao_imovel> Lista = imovelRepository.Retorna_Itbi_Isencao_Imovel(p);
            foreach (Itbi_isencao_imovel item in Lista) {
                imovel += item.Descricao + ", ";
            }
            imovel = imovel.Substring(0, imovel.Length - 2);
            rd.SetParameterValue("IMOVEL", imovel);
            try {
                rd.RecordSelectionFormula = "{itbi_isencao_main.guid}='" + p + "'";
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Isencao_Itbi.pdf");
            } catch (Exception ex) {
                throw;
            }
        }

        #endregion

        [Route("Itbi_isencao_e")]
        [HttpGet]
        public ActionResult Itbi_isencao_e( string guid, string a, int s = 0, string natureza = "27") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ItbiViewModel model = Retorna_Itbi_Isencao_Gravado(guid);
            natureza = model.Natureza_Isencao_Codigo.ToString();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (String.IsNullOrWhiteSpace(guid)) {
                bool bFuncionario = Session["hashfunc"].ToString() == "S" ? true : false;
                int nId = Convert.ToInt32(Session["hashid"]);
                string usuario_Nome = "", usuario_Doc = "";
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                if (bFuncionario) {
                    usuarioStruct _user = sistemaRepository.Retorna_Usuario(nId);
                    usuario_Nome = _user.Nome_completo;
                } else {
                    Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(nId);
                    usuario_Nome = _user.Nome;
                    usuario_Doc = Functions.FormatarCpfCnpj(_user.Cpf_Cnpj);
                }

                model.UserId = Convert.ToInt32(Session["hashid"]);
                model.Guid = Guid.NewGuid().ToString("N");
                model.Data_cadastro = DateTime.Now;

                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);

                Itbi_isencao_main regMain = new Itbi_isencao_main() {
                    Guid = model.Guid,
                    Data_cadastro = DateTime.Now,
                    Situacao = 1,
                    Fiscal_id = 0,
                    Usuario_nome = usuario_Nome,
                    Usuario_doc = usuario_Doc,
                    Natureza = 0,
                    Isencao_ano = 0,
                    Isencao_numero = 0,
                    Usuario_id = nId
                };
                Exception ex = imovelRepository.Incluir_isencao_main(regMain);
            } else {
                if (a == "rv") {//remover imóvel
                    Exception ex = imovelRepository.Excluir_Itbi_Isencao_Imovel(guid, s);
                }

                model = Retorna_Itbi_Isencao_Gravado(guid);
                natureza = model.Natureza_Isencao_Codigo.ToString();
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
            }
            return View(model);
        }

        [Route("Itbi_isencao_e")]
        [HttpPost]
        public ActionResult Itbi_isencao_e(ItbiViewModel model, string natureza, HttpPostedFileBase file, string action) {
            if (model.Lista_Anexo == null)
                model.Lista_Anexo = new List<ListAnexoEditorViewModel>();

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (action == "btnValida") {
                Exception ex2 = imovelRepository.Alterar_Itbi_Isencao_Natureza(model.Guid, Convert.ToInt32(natureza));
                goto ActionPos;
            } else {
                if (action == "btnAnexoAdd") {
                    if (file != null) {
                        if (string.IsNullOrWhiteSpace(model.Anexo_Desc_tmp)) {
                            ViewBag.Result = "* Digite uma descrição para o anexo (é necessário selecionar novamente o anexo).";
                            return View(model);
                        } else {
                            if (file.ContentType != "application/pdf") {
                                ViewBag.Result = "* Este tipo de arquivo não pode ser enviado como anexo.";
                                return View(model);
                            } else {
                                string _ano = model.Itbi_Ano == 0 ? DateTime.Now.Year.ToString() : model.Itbi_Ano.ToString();
                                string _path = "~/Files/Itbi/" + _ano + "/";
                                var fileName = Path.GetFileName(file.FileName);
                                fileName = fileName.RemoveDiacritics();
                                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(_path) + model.Guid);
                                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path + model.Guid), fileName);
                                file.SaveAs(path);
                                byte seqA = imovelRepository.Retorna_Itbi_Anexo_Disponivel(model.Guid);
                                Itbi_anexo regA = new Itbi_anexo() {
                                    Guid = model.Guid,
                                    Seq = seqA,
                                    Descricao = model.Anexo_Desc_tmp,
                                    Arquivo = fileName
                                };
                                Exception ex2 = imovelRepository.Incluir_Itbi_Anexo(regA);
                                ListAnexoEditorViewModel Anexo = new ListAnexoEditorViewModel() {
                                    Seq = model.Lista_Anexo.Count,
                                    Arquivo = fileName,
                                    Nome = model.Anexo_Desc_tmp
                                };
                                model.Lista_Anexo.Add(Anexo);
                                goto ActionPos;
                            }
                        }
                    } else {
                        ViewBag.Result = "* Nenhum arquivo selecionado.";
                        goto ActionPos;
                    }
                }
            }
            int _codigo = Convert.ToInt32(model.Vendedor_Cpf_cnpj_tmp);
            model.Tipo_Imovel = _codigo == 0 ? "Rural" : "Urbano";
            bool _urbano = model.Tipo_Imovel == "Urbano";
            List<Itbi_isencao_imovel> Lista = imovelRepository.Retorna_Itbi_Isencao_Imovel(model.Guid);
            if (_urbano && !imovelRepository.Existe_Imovel(_codigo)) {
                ViewBag.Result = "Imóvel não cadastrado.";
                model = Retorna_Itbi_Isencao_Gravado(model.Guid);
                model.Vendedor_Cpf_cnpj_tmp = "";
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
                return View(model);
            } else {
                if (_urbano) {
                    ImovelStruct imovel = imovelRepository.Dados_Imovel(_codigo);
                    model.Codigo = imovel.Codigo.ToString("00000");
                    model.Inscricao = imovel.Inscricao;
                    model.Dados_Imovel = imovel;
                    decimal _somaarea = imovelRepository.Soma_Area(_codigo);
                    bool _predial = _somaarea > 0;
                    string _descricao = "";
                    Imovel_Isencao reg = new Imovel_Isencao() {
                        Tipo = model.Tipo_Imovel,
                        Codigo = _codigo.ToString()
                    };
                    if (_predial)
                        _descricao = "um imóvel ";
                    else
                        _descricao = "um terreno ";

                    _descricao += "com " + imovel.Area_Terreno.ToString("#.#0") + "m² localizado no(a) " + imovel.NomeLogradouroAbreviado + ", " + imovel.Numero.ToString();
                    if (imovel.Complemento != "")
                        _descricao += " " + imovel.Complemento;
                    if (imovel.QuadraOriginal != "")
                        _descricao += " Quadra: " + imovel.QuadraOriginal + " Lote: " + imovel.LoteOriginal;

                    _descricao += " no bairro " + imovel.NomeBairro + " na cidade de JABOTICABAL/SP, ";
                    _descricao += "inscrição municipal: " + imovel.Inscricao + ", cadastro municipal: " + imovel.Codigo.ToString() + ",";
                    reg.Descricao = _descricao;
                    model.Lista_Isencao.Add(reg);
                } else {
                    model.Codigo = "0";
                    model.Inscricao = "";
                    decimal _somaarea = 0;
                    bool _predial = _somaarea > 0;
                    string _descricao = model.Descricao_Imovel;
                    Imovel_Isencao reg = new Imovel_Isencao() {
                        Tipo = model.Tipo_Imovel,
                        Codigo = _codigo.ToString(),
                        Descricao = model.Vendedor_Nome_tmp
                    };

                    model.Lista_Isencao.Add(reg);
                }
            }

            byte y = 1;
            foreach (Itbi_isencao_imovel i in Lista) {
                i.Seq = y;
                y++;
            }
            foreach (Imovel_Isencao item in model.Lista_Isencao) {
                if (item != null) {
                    Itbi_isencao_imovel regC = new Itbi_isencao_imovel() {
                        Guid = model.Guid,
                        Seq = y,
                        Tipo = item.Tipo,
                        Codigo = Convert.ToInt32(item.Codigo),
                        Descricao = item.Descricao
                    };
                    Lista.Add(regC);
                    y++;
                }
            }
            Exception ex = imovelRepository.Incluir_Itbi_isencao_imovel(Lista);
            if (ex != null)
                throw ex;

            Itbi_isencao_main regM = new Itbi_isencao_main();
            regM.Guid = model.Guid;
            regM.Natureza = Convert.ToInt32(natureza);
            if (regM.Isencao_numero == 0) {
                regM.Isencao_numero = imovelRepository.Retorna_Itbi_Isencao_Disponivel();
                regM.Isencao_ano = (short)DateTime.Now.Year;
            }

            ex = imovelRepository.Alterar_Itbi_Isencao(regM);
            if (ex != null)
                throw ex;

            ActionPos:

            if (action == "btnValida")
                return RedirectToAction("itbi_isencao_ok");
            else {
                model = Retorna_Itbi_Isencao_Gravado(model.Guid);
                model.Lista_Natureza_Isencao = Lista_Natureza_Isencao(natureza);
                model.Vendedor_Cpf_cnpj_tmp = null;
                model.Vendedor_Nome_tmp = null;
                return View(model);
            }
        }


    }
}