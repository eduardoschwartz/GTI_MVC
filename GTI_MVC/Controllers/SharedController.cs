using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace GTI_MVC.Controllers {
    public class SharedController : Controller {

        [Route("Checkgticd")]
        [HttpGet]
        public ActionResult Checkgticd(string c)
        {
            ViewBag.c = c;
            return View();
        }

        [Route("Checkgticd")]
        [HttpPost]
        public ActionResult Checkgticd(CertidaoViewModel model, string c) {
            int _codigo, _ano, _numero;
            string _tipo, _chave = c,_pdfFileName="";
            if (c != null) {
                Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
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
                            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
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
        public ActionResult Checkguid(string c) {
            ViewBag.c = c;
            return View();
        }


        [Route("Checkguid")]
        [HttpPost]
        public ActionResult Checkguid(CertidaoViewModel model, string c) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Itbi_isencao_main_Struct _itbi = imovelRepository.Retorna_Itbi_Isencao_Main(c);
            List<Itbi_isencao_imovel> _Lista = imovelRepository.Retorna_Itbi_Isencao_Imovel(c);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Isencao_Itbi_Valida.rpt"));
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            crConnectionInfo.ServerName = "200.232.123.115";
            crConnectionInfo.DatabaseName = "Tributacao";
            crConnectionInfo.UserID = "gtisys";
            crConnectionInfo.Password = "everest";
            CrTables = rd.Database.Tables;
            foreach (Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
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
            } catch (Exception ex) {
                throw;
            }

        }

        [Route("Cep_inc")]
        [HttpGet]
        public ActionResult Cep_inc() {
            CepViewModel model = new CepViewModel();
            return View(model);
        }

        [Route("Cep_inc")]
        [HttpPost]
        public ActionResult Cep_inc(CepViewModel model, string action) {
            ViewBag.Error = "";
            if (action == "btnCepOK") {
                if (model.Cep == null || model.Cep.Length < 9) {
                    ViewBag.Error = "* Cep digitado inválido.";
                    return View(model);
                }
            }
            if (action == "btnCepCancel") {
                model.Cep = null;
                return View(model);
            }

             model.NomeUf = "TESTE";
             return View(model);
        }


    }
}