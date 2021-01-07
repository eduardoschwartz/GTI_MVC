using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Tributario.EditorTemplates;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using static GTI_Models.modelCore;
using static GTI_Mvc.Functions;

namespace GTI_Mvc.Controllers {
    [Route("Tributario")]
    public class TributarioController : Controller
    {

        [Route("Certidao/Certidao_Debito_Codigo")]
        [HttpGet]
        public ViewResult Certidao_Debito_Codigo() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Certidao/Certidao_Debito_Codigo")]
        [HttpPost]
        public ActionResult Certidao_Debito_Codigo(CertidaoViewModel model) {
            int _codigo = 0;
            short _ret =0;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito);
            bool _existeCod = false;
            string _tipoCertidao = "",_nao="", _sufixo = "XX",_reportName="", _numProcesso = "9222-3/2012", _dataProcesso = "18/04/2012",_cpf,_cnpj; 
            TipoCadastro _tipoCadastro=new TipoCadastro();
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 100000) {
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
                    _tipoCadastro = TipoCadastro.Imovel;
                } else {
                    if (_codigo >= 100000 && _codigo < 500000) {
                        _existeCod = empresaRepository.Existe_Empresa(_codigo);
                        _tipoCadastro = TipoCadastro.Empresa;
                    } else {
                        ViewBag.Result = "Inscrição inválida.";
                        return View(certidaoViewModel);
                    }
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(certidaoViewModel);
            }

            if (!_existeCod) {
                ViewBag.Result = "Inscrição não cadastrada.";
                return View(certidaoViewModel);
            }


            //***Verifica débito

            Certidao_debito_detalhe dadosCertidao = tributarioRepository.Certidao_Debito(_codigo);
            string _tributo = dadosCertidao.Descricao_Lancamentos;

            if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.Negativa) {
                _tipoCertidao = "NEGATIVA";
                _nao = "não ";
                _ret = 3;
                _sufixo = "CN";
                if (_tipoCadastro == TipoCadastro.Imovel)
                    _reportName = "Certidao_Debito_Imovel.rpt";
                else
                    _reportName = "Certidao_Debito_Empresa.rpt";
            } else {
                if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.Positiva) {
                    _tipoCertidao = "POSITIVA";
                    _ret = 4;
                    _sufixo = "CP";
                    if (_tipoCadastro == TipoCadastro.Imovel)
                        _reportName = "Certidao_Debito_Imovel.rpt";
                    else
                        _reportName = "Certidao_Debito_Empresa.rpt";
                } else {
                    if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.NegativaPositiva) {
                        _ret = 5;
                        _tipoCertidao = "POSITIVA COM EFEITO NEGATIVA";
                        _sufixo = "PN";
                        if (_tipoCadastro == TipoCadastro.Imovel)
                            _reportName = "Certidao_Debito_Imovel_PN.rpt";
                        else
                            _reportName = "Certidao_Debito_Empresa_PN.rpt";
                    }
                }
            }
                      
            int _numero_certidao =tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito);
            int _ano_certidao = DateTime.Now.Year;
            List<Certidao> certidao = new List<Certidao>();
            Certidao reg = new Certidao();
            if (_tipoCadastro == TipoCadastro.Imovel) {
                List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
                _cpf = listaProp[0].CPF;
                ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
                reg.Codigo = _dados.Codigo;
                reg.Cpf_Cnpj = _cpf;
                reg.Inscricao = _dados.Inscricao;
                reg.Endereco = _dados.NomeLogradouro;
                reg.Endereco_Numero = (int)_dados.Numero;
                reg.Endereco_Complemento = _dados.Complemento;
//                reg.Bairro = _dados.NomeBairro ?? "";
                reg.Cidade = "JABOTICABAL";
                reg.Uf = "SP";
                reg.Atividade_Extenso = "";
                reg.Nome_Requerente = listaProp[0].Nome;
                reg.Ano = DateTime.Now.Year;
                reg.Numero = _numero;
                reg.Quadra_Original = _dados.QuadraOriginal ?? "";
                reg.Lote_Original = _dados.LoteOriginal ?? "";
                reg.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _codigo.ToString() + "-" + _sufixo;
                reg.Tipo_Certidao = _tipoCertidao;
                reg.Nao = _nao;
                reg.Tributo = _tributo;

                Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");
                Bairro _bairro = enderecoRepository.RetornaLogradouroBairro((int)_dados.CodigoLogradouro, (short)_dados.Numero);
                reg.Bairro = _bairro.Descbairro ?? "";


            } else {
                EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);

                string Regime = empresaRepository.RegimeEmpresa(_codigo);
                if (Regime == "V") {
                    //Verifica competência
                    Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
                    Eicon_bll eicon_Class = new Eicon_bll("GTIEicon");
                    int _holes = tributario_Class.Competencias_Nao_Encerradas(eicon_Class.Resumo_CompetenciaISS(_codigo, _dados.Data_Encerramento));
                    if (_holes == 0) {
                        ViewBag.Result = "";
                    } else {
                        ViewBag.Result = "A empresa possui uma ou mais competências não encerradas.";
                        return View(certidaoViewModel);
                    }
                } else {
                    ViewBag.Result = "";
                }

                _cpf = _dados.Cpf ?? "";
                _cnpj = _dados.Cnpj ?? "";
                reg.Codigo = _dados.Codigo;
                reg.Cpf_Cnpj = _dados.Cpf_cnpj;
                reg.Inscricao = _dados.Inscricao_estadual ?? "";
                reg.Endereco = _dados.Endereco_nome;
                reg.Endereco_Numero = (int)_dados.Numero;
                reg.Endereco_Complemento = _dados.Complemento;
                reg.Bairro = _dados.Bairro_nome ?? "";
                reg.Cidade = _dados.Cidade_nome;
                reg.Uf = _dados.UF;
                reg.Atividade_Extenso = _dados.Atividade_extenso;
                reg.Nome_Requerente = _dados.Razao_social;
                reg.Ano = DateTime.Now.Year;
                reg.Numero = _numero;
                reg.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _codigo.ToString() + "-" + _sufixo;
                reg.Tipo_Certidao = _tipoCertidao;
                reg.Nao = _nao;
                reg.Tributo = _tributo;
            }
            reg.Numero_Ano = _numero_certidao.ToString("00000") + "/" + _ano_certidao.ToString("0000");
            certidao.Add(reg);

            Certidao_debito cert = new Certidao_debito {
                Codigo = _codigo,
                Ano = (short)DateTime.Now.Year,
                Ret = _ret,
                Numero = _numero_certidao,
                Datagravada = DateTime.Now,
                Inscricao = reg.Inscricao,
                Nome = reg.Nome_Requerente,
                Logradouro = reg.Endereco,
                Numimovel = (short)reg.Endereco_Numero,
                Bairro = reg.Bairro,
                Cidade = reg.Cidade,
                UF = reg.Uf,
                Processo = _numProcesso,
                Dataprocesso = Convert.ToDateTime(_dataProcesso),
                Atendente = "GTI.Web",
                Cpf = _cpf,
                Atividade = reg.Atividade_Extenso,
                Suspenso="",
                Lancamento = dadosCertidao.Descricao_Lancamentos
                
            };
            Exception ex = tributarioRepository.Insert_Certidao_Debito(cert);

            Certidao_impressao cimp = new Certidao_impressao() {
                Ano=reg.Ano,
                Numero=reg.Numero,
                Codigo=Convert.ToInt32(reg.Codigo).ToString("000000"),
                Endereco=reg.Endereco,
                Endereco_Numero=reg.Endereco_Numero,
                Endereco_Complemento=reg.Endereco_Complemento,
                Bairro=reg.Bairro,
                Cidade=reg.Cidade,
                Uf=reg.Uf,
                Quadra_Original=reg.Quadra_Original,
                Lote_Original=reg.Lote_Original,
                Inscricao=reg.Inscricao,
                Numero_Ano=reg.Numero_Ano,
                Nome=reg.Nome_Requerente,
                Cpf_Cnpj=reg.Cpf_Cnpj,
                Atividade=reg.Atividade_Extenso,
                Tributo=reg.Tributo,
                Tipo_Certidao=reg.Tipo_Certidao,
                Nao=reg.Nao.ToUpper()
            };

            //##### QRCode ##########################################################
            string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkgticd?c=" + reg.Controle;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    cimp.QRCodeImage = byteImage;
                }
            }
            //#######################################################################
            ex = tributarioRepository.Insert_Certidao_Impressao(cimp);

            Certidao_debito_doc RegSave = new Certidao_debito_doc() {
                Ano =(short) _ano_certidao,
                Numero = _numero_certidao,
                Ret = _ret,
                Cpf_cnpj = reg.Cpf_Cnpj,
                Data_emissao = DateTime.Now,
                Nome = reg.Nome_Requerente,
                Tributo = reg.Tributo,
                Validacao = reg.Controle
            };
            ex = tributarioRepository.Insert_Certidao_Debito_Doc(RegSave);


            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View("Certidao_Debito_Codigo");
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/" + _reportName));
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

            try {
                rd.RecordSelectionFormula = "{certidao_impressao.ano}=" + cimp.Ano + " and {certidao_impressao.numero}=" + cimp.Numero;
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Debito.pdf");
            } catch {
                throw;
            }
        }

        [Route("Certidao/Certidao_Debito_Doc")]
        [HttpGet]
        public ViewResult Certidao_Debito_Doc() {
            CertidaoViewModel model = new CertidaoViewModel {
                OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = true },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = false }
            },
                SelectedValue = "cpfCheck"
            };
            return View(model);
        }

        [Route("Certidao/Certidao_Debito_Doc")]
        [HttpPost]
        public ActionResult Certidao_Debito_Doc(CertidaoViewModel model) {
            string sNome = "";
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito_Doc);
            ViewBag.Result = "";

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.CpfValue != null || model.CnpjValue != null) {
                List<int> _lista = new List<int>();
                
                if (model.CpfValue != null) {
                    sNome = sistemaRepository.Nome_por_Cpf(RetornaNumero(model.CpfValue));
                    _lista = sistemaRepository.Lista_Codigos_Documento(RetornaNumero(model.CpfValue), TipoDocumento.Cpf);
//                    _lista = empresaRepository.Retorna_Codigo_por_CPF(RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        sNome = sistemaRepository.Nome_por_Cnpj(RetornaNumero(model.CnpjValue));
                        _lista = sistemaRepository.Lista_Codigos_Documento(RetornaNumero(model.CnpjValue), TipoDocumento.Cnpj);
//                        _lista = empresaRepository.Retorna_Codigo_por_CNPJ(RetornaNumero(model.CnpjValue));
                    }
                }
                if (_lista.Count == 0) {
                    ViewBag.Result = "Não existem cadastros com este CPF/CNPJ.";
                    return View(model);
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            //####################################

            string sData = "18/04/2012", sTributo = "", sNao = "", sCertifica = "",_reportName="",_tipo_certidao="";
            short nRet = 0;
            List < Certidao_debito_documento> _lista_certidao = new List<Certidao_debito_documento>();
            RetornoCertidaoDebito _tipo_Certidao;

            DateTime dDataProc = Convert.ToDateTime(sData);
            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");

            bool bEmpresa = false, bCidadao = false, bImovel = false;
            List<int> _codigos = sistemaRepository.Lista_Codigos_Documento(!string.IsNullOrWhiteSpace( model.CpfValue)  ? model.CpfValue : model.CnpjValue, !string.IsNullOrWhiteSpace( model.CpfValue)  ? TipoDocumento.Cpf : TipoDocumento.Cnpj);

            foreach (int _codigo in _codigos) {
                TipoCadastro _tipo_cadastro = _codigo < 100000 ? TipoCadastro.Imovel : _codigo >= 100000 && _codigo < 500000 ? TipoCadastro.Empresa : TipoCadastro.Cidadao;
                //***Verifica débito
                Certidao_debito_detalhe dadosCertidao = tributario_Class.Certidao_Debito(_codigo);
                if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.Negativa) {
                    nRet = 3;
                    sTributo = "";
                } else {
                    if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.Positiva) {
                        if (_tipo_cadastro == TipoCadastro.Empresa) bEmpresa = true;
                        if (_tipo_cadastro == TipoCadastro.Cidadao) bCidadao = true;
                        if (_tipo_cadastro == TipoCadastro.Imovel) bImovel = true;
                        nRet = 4;
                        sTributo = dadosCertidao.Descricao_Lancamentos;
                    } else {
                        if (dadosCertidao.Tipo_Retorno == RetornoCertidaoDebito.NegativaPositiva) {
                            nRet = 5;
                            sTributo = dadosCertidao.Descricao_Lancamentos;
                        }
                    }
                }

                Certidao_debito_documento reg = new Certidao_debito_documento {
                    _Codigo = _codigo,
                    _Ret = nRet,
                    _Tributo = sTributo,
                    _Nome = sNome == null ? "" : sNome.Trim()
                };

                _lista_certidao.Add(reg);
            }

            bool _find = false;
            foreach (Certidao_debito_documento reg in _lista_certidao) {
                if (reg._Ret != 3) {
                    _find = true;
                    break;
                }
            }
            if (!_find) {
                _tipo_Certidao = RetornoCertidaoDebito.Negativa;
                sNao = " não";
                _reportName = "CertidaoDebitoDocumentoN.rpt";
            } else {
                _find = false;
                foreach (Certidao_debito_documento reg in _lista_certidao) {
                    if (reg._Ret == 4) {
                        _find = true;
                        break;
                    }
                }
                if (_find) {
                    _tipo_Certidao = RetornoCertidaoDebito.Positiva;
                    nRet = 4;
                    if (!bEmpresa && !bCidadao && !bImovel) {
                        //Se a certidão positiva for apenas de imóvel, verifica se esta no prazo das parcelas únicas em aberto.
                        bool bUnicaNaoPago = false;
                        foreach (int _codigo in _codigos) {
                            bUnicaNaoPago = tributario_Class.Parcela_Unica_IPTU_NaoPago(_codigo, DateTime.Now.Year);
                            if (bUnicaNaoPago) break;
                        }
                        if (bUnicaNaoPago) {
                            sCertifica = " embora conste parcela(s) não paga(s) do IPTU de " + DateTime.Now.Year.ToString() + ", em razão da possibilidade do pagamento integral deste imposto em data futura, ";
                            sNao = " não";
                            nRet = 3;
                            _tipo_Certidao = RetornoCertidaoDebito.Negativa;
                        }
                    }
                } else {
                    _tipo_Certidao = RetornoCertidaoDebito.NegativaPositiva;
                }
            }

            string _tributo = "";
            foreach (Certidao_debito_documento item in _lista_certidao) {
                if (item._Tributo != "")
                    _tributo += item._Tributo + " (IM:" + item._Codigo + ")" + ",";
            }
            if (_tributo.Length > 0)
                _tributo = _tributo.Substring(0, _tributo.Length - 1);
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(TipoCertidao.Debito);
            int _ano_certidao = DateTime.Now.Year;
            Certidao cert = new Certidao {
                Ano = (short)_ano_certidao,

                Numero = _numero_certidao,
                Nome_Requerente = _lista_certidao[0]._Nome,
                Cpf_Cnpj = !string.IsNullOrWhiteSpace(model.CpfValue) ? model.CpfValue : model.CnpjValue,
                Numero_Ano = _numero_certidao.ToString("00000") + "/" + _ano_certidao.ToString()
            };
            if (_tipo_Certidao == RetornoCertidaoDebito.Negativa) {
                cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IN";
//                cert.Tributo = "Não consta débito apurado contra o(a) mesmo(a).";
                _tipo_certidao = "Negativa";
                cert.Nao = "NÃO";
            } else {
                if (_tipo_Certidao == RetornoCertidaoDebito.Positiva) {
                    cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IP";
                    //                    cert.Tributo = "Consta débito apurado contra o(a) mesmo(a) com referência a: " + _tributo;
                    cert.Tributo =  _tributo;
                    cert.Nao = "";
                    _tipo_certidao = "Positiva";
                    _reportName = "CertidaoDebitoDocumentoP.rpt";
                } else {
                    if (_tipo_Certidao == RetornoCertidaoDebito.NegativaPositiva) {
                        cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IS";
//                        cert.Tributo = "Consta débito apurado contra o(a) mesmo(a) com referência a: " + _tributo + " que se encontram em sua exigibilidade suspensa, em razão de parcelamento dos débitos";
                        _reportName = "CertidaoDebitoDocumentoPN.rpt";
                        _tipo_certidao = "Positiva com efeito negativa";
                        cert.Nao = "";
                    }
                }
            }
            List<Certidao> ListaCertidao = new List<Certidao> {
                cert
            };

            Certidao_debito_doc RegSave = new Certidao_debito_doc() {
                Ano = (short)_ano_certidao,
                Numero = _numero_certidao,
                Ret = nRet,
                Cpf_cnpj = cert.Cpf_Cnpj,
                Data_emissao=DateTime.Now,
                Nome = cert.Nome_Requerente,
                Tributo = cert.Tributo,
                Validacao = cert.Controle
            };
            Exception ex = tributario_Class.Insert_Certidao_Debito_Doc(RegSave);


            Certidao_impressao cimp = new Certidao_impressao() {
                Ano = (short)_ano_certidao,
                Numero = _numero_certidao,
                Numero_Ano = cert.Numero_Ano,
                Nome = cert.Nome_Requerente,
                Cpf_Cnpj = cert.Cpf_Cnpj,
                Tributo = cert.Tributo,
                Tipo_Certidao =_tipo_certidao,
                Nao = cert.Nao.ToUpper()
            };

            //##### QRCode ##########################################################
            string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkgticd?c=" + cert.Controle;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    cimp.QRCodeImage = byteImage;
                }
            }
            //#######################################################################
            ex = tributarioRepository.Insert_Certidao_Impressao(cimp);

            if (ex != null) {
                throw ex;
            } else {

                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/" + _reportName));
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

                try {
                    rd.RecordSelectionFormula = "{certidao_impressao.ano}=" + cimp.Ano + " and {certidao_impressao.numero}=" + cimp.Numero;
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Certidao_Debito.pdf");
                } catch {
                    throw;
                }

            }
        }

        [Route("Comprovante_Pagamento")]
        [HttpGet]
        public ViewResult Comprovante_Pagamento() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Comprovante_Pagamento")]
        [HttpPost]
        public ActionResult Comprovante_Pagamento(CertidaoViewModel model) {
            int _codigo = Convert.ToInt32(model.Inscricao);
            int _documento;
            bool _existe;
            string _nome,_cpfcnpj;
            if (model.Documento.Length < 16) {
                ViewBag.Result = "Nº de documento inválido.";
                return View(model);
            } else
                _documento = Convert.ToInt32(model.Documento.Substring(model.Documento.Length - 8, 8));

            ViewBag.Result = "";

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            TipoCadastro _tipoCadastro = Tipo_Cadastro(_codigo);
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Cidadao_bll requerenteRepository = new Cidadao_bll("GTIconnection");
            if (_tipoCadastro == TipoCadastro.Imovel) {
                _existe = imovelRepository.Existe_Imovel(_codigo);
                if (!_existe) {
                    ViewBag.Result = "Inscrição não cadastrada.";
                    return View(model);

                } else {
                    ImovelStruct _dadosImovel = imovelRepository.Dados_Imovel(_codigo);
                    _nome = _dadosImovel.Proprietario_Nome;
                    List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
                    _nome = listaProp[0].Nome;
                    _cpfcnpj = listaProp[0].CPF ?? listaProp[0].CPF;
                }
            } else {
                if (_tipoCadastro == TipoCadastro.Empresa) {
                    _existe = empresaRepository.Existe_Empresa(_codigo);
                    if (!_existe) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);

                    } else {
                        EmpresaStruct _dadosEmpresa = empresaRepository.Retorna_Empresa(_codigo);
                        _nome = _dadosEmpresa.Razao_social;
                        _cpfcnpj = _dadosEmpresa.Cpf_cnpj;
                    }
                } else {
                    _existe = requerenteRepository.ExisteCidadao(_codigo);
                    if (!_existe) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);

                    } else {
                        CidadaoStruct _dadosCidadao = requerenteRepository.Dados_Cidadao(_codigo);
                        _nome = _dadosCidadao.Nome;
                        _cpfcnpj = _dadosCidadao.Cpf ?? _dadosCidadao.Cnpj;
                    }
                }
            }

            int _codigoBD = tributarioRepository.Retorna_Codigo_por_Documento(_documento);
            if (_codigo != _codigoBD) {
                ViewBag.Result = "O documento informado não pertence a esta inscrição.";
                return View(model);
            }

            List<Certidao> certidao = new List<Certidao>();
            DebitoPagoStruct regPag = tributarioRepository.Retorna_DebitoPago_Documento(_documento);
            if (regPag == null) {
                ViewBag.Result = "Pagamento não encontrado para este documento.";
                return View(model);
            }
            int _numero_certidao = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Comprovante_Pagamento);
            Certidao reg = new Certidao {
                Codigo=_codigo,
                Nome_Requerente = _nome,
                Ano = DateTime.Now.Year,
                Numero = _numero_certidao,
                Banco_Nome = regPag.Banco_Nome + " Agência: " + regPag.Codigo_Agencia ?? "",
                Cpf_Cnpj = _cpfcnpj,
                Data_Geracao = DateTime.Now,
                Data_Pagamento = regPag.Data_Pagamento,
                Numero_Documento = _documento,
                Valor_Pago = (decimal)regPag.Valor_Pago_Real
            };
            reg.Controle = reg.Numero.ToString("00000") + reg.Ano.ToString("0000") + "/" + _codigo.ToString() + "-PG";


            certidao.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Comprovante_Pagamento.rpt" ));
            try {
                rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Comprovante_Pagamento.pdf");
            } catch {
                throw;
            }

        }

        [Route("Dama")]
        [HttpGet]
        public ViewResult Dama() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Dama")]
        [HttpPost]
        public ActionResult Dama(CertidaoViewModel model) {
            bool _isdate = IsDate(model.DataVencimento);
            if (!_isdate) {
                ViewBag.Result = "Data de vencimento inválida.";
                return View(model);
            }

            DateTime _dataVencto = Convert.ToDateTime( Convert.ToDateTime(model.DataVencimento).ToString("dd/MM/yyyy"));
            DateTime _dataAtual = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
            if (_dataVencto< _dataAtual) {
                ViewBag.Result = "Data de vencimento inferior a data atual.";
                return View(model);
            }

            if (_dataVencto < _dataAtual) {
                ViewBag.Result = "Data de vencimento inferior a data atual.";
                return View(model);
            }

            double _days = (_dataVencto - _dataAtual).TotalDays;
            if (_days > 30) {
                ViewBag.Result = "Data de vencimento superior a 30 dias.";
                return View(model);
            }
            return View("Damb", model);
        }

        [Route("Damb")]
        [HttpGet]
        public ActionResult Damb(CertidaoViewModel model) {
            if (model.Inscricao == null) {
                return RedirectToAction("Login_gti", "Home");
            }
            CertidaoViewModel modelt = new CertidaoViewModel {
                OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = true },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = false }
            },
                SelectedValue = "cpfCheck"
            };
            model.OptionList = modelt.OptionList;
            return View(model);
        }

        [Route("Damb")]
        [HttpPost]
        public ActionResult Damb(CertidaoViewModel model,int Codigo=0) {
            if (model.CpfValue != null) {
                if (!ValidaCpf(model.CpfValue)) {
                    ViewBag.Result = "CPF inválido.";
                    return View(model);
                }
            }
            if (model.CnpjValue != null) {
                if (!ValidaCNPJ(model.CnpjValue)) {
                    ViewBag.Result = "CNPJ inválido.";
                    return View(model);
                }
            }
            if (Codigo == 0) {
                
            }
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Cidadao_bll requerenteRepository = new Cidadao_bll("GTIconnection");
            bool _existeCod;
            string _nome,_cpfcnpj;
            int _codigo = Convert.ToInt32(model.Inscricao);
            if (_codigo < 100000) {
                _existeCod = imovelRepository.Existe_Imovel(_codigo);
                if (!_existeCod) {
                    ViewBag.Result = "Inscrição não cadastrada.";
                    return View(model);
                } else {
                    if (model.CpfValue != null) {
                        _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, RetornaNumero(model.CpfValue));
                        if (!_existeCod) {
                            ViewBag.Result = "CPF não pertence a esta inscrição.";
                            return View(model);
                        }
                    } else {
                        if (model.CnpjValue != null) {
                            _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, RetornaNumero( model.CnpjValue));
                            if (!_existeCod) {
                                ViewBag.Result = "CNPJ não pertence a esta inscrição.";
                                return View(model);
                            }
                        }
                    }
                    List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);
                    _nome = _prop[0].Nome;
                    _cpfcnpj = _prop[0].CPF ?? _prop[0].CPF;
                }
            } else  {
                if (_codigo >= 100000 && _codigo < 500000) {
                    _existeCod = empresaRepository.Existe_Empresa(_codigo);
                    if (!_existeCod) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);
                    } else {
                        if (model.CpfValue != null) {
                            _existeCod = empresaRepository.ExisteEmpresaCpf(RetornaNumero(model.CpfValue))>0;
                            if (!_existeCod) {
                                ViewBag.Result = "CPF não pertence a esta inscrição.";
                                return View(model);
                            }
                        } else {
                            if (model.CnpjValue != null) {
                                _existeCod = empresaRepository.ExisteEmpresaCnpj(RetornaNumero(model.CnpjValue))>0;
                                if (!_existeCod) {
                                    ViewBag.Result = "CNPJ não pertence a esta inscrição.";
                                    return View(model);
                                }
                            }
                        }
                    }
                    EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);
                    _nome = _empresa.Razao_social;
                    _cpfcnpj = _empresa.Cpf_cnpj;
                } else {
                    _existeCod = requerenteRepository.ExisteCidadao(_codigo);
                    if (!_existeCod) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);
                    } else {
                        if (model.CpfValue != null) {
                            _existeCod = requerenteRepository.Existe_Cidadao_Cpf(_codigo, RetornaNumero(model.CpfValue));
                            if (!_existeCod) {
                                ViewBag.Result = "CPF não pertence a esta inscrição.";
                                return View(model);
                            }
                        } else {
                            if (model.CnpjValue != null) {
                                _existeCod = requerenteRepository.Existe_Cidadao_Cnpj(_codigo, RetornaNumero(model.CnpjValue));
                                if (!_existeCod) {
                                    ViewBag.Result = "CNPJ não pertence a esta inscrição.";
                                    return View(model);
                                }
                            }
                        }
                    }
                    CidadaoStruct _cidadao = requerenteRepository.Dados_Cidadao(_codigo);
                    _nome = _cidadao.Nome;
                    _cpfcnpj = string.IsNullOrWhiteSpace(_cidadao.Cpf) ? _cidadao.Cnpj: _cidadao.Cpf;
                }
            }
            DebitoSelectionViewModel modelt = new DebitoSelectionViewModel() {
                Inscricao=_codigo,
                Nome=_nome,
                CpfCnpjLabel=_cpfcnpj,
                Data_Vencimento=Convert.ToDateTime(model.DataVencimento)
            };

            TempData["debito"]= modelt;
            return RedirectToAction("Damc");
        }

        [Route("Damc")]
        [HttpGet]
        public ActionResult Damc() {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            var value = TempData["debito"];

            if (!(value is DebitoSelectionViewModel model)) {
                return RedirectToAction("Login_gti", "Home");
            }

            DateTime _dataVencto = model.Data_Vencimento;
            List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(model.Inscricao, 1980, 2050, 0, 99, 0, 99, 0, 999, 0, 99, 0, 99, _dataVencto, "Web");
            List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);
            List<DebitoStructure> Lista_debitos = new List<DebitoStructure>();

            int nPlano = 0;
            decimal nSomaPrincipal = 0, nSomaJuros = 0, nSomaMulta = 0, nSomaCorrecao = 0, nSomaTotal = 0, nSomaHonorario = 0;

            foreach (var item in ListaParcela) {
                if (item.Statuslanc == 3 || item.Statuslanc == 19 || item.Statuslanc == 38 || item.Statuslanc == 39 || item.Statuslanc == 42 || item.Statuslanc == 43) {
                    DebitoStructure reg = new DebitoStructure {
                        Codigo_Reduzido = item.Codreduzido,
                        Ano_Exercicio = item.Anoexercicio,
                        Codigo_Lancamento = Convert.ToInt16(item.Codlancamento),
                        Descricao_Lancamento = item.Desclancamento,
                        Sequencia_Lancamento = Convert.ToInt16(item.Seqlancamento),
                        Numero_Parcela = Convert.ToInt16(item.Numparcela),
                        Complemento = item.Codcomplemento,
                        Data_Vencimento = item.Datavencimento,
                        Codigo_Situacao = Convert.ToInt16(item.Statuslanc),
                        Soma_Principal = item.Valortributo,
                        Soma_Juros = item.Valorjuros,
                        Soma_Multa = item.Valormulta,
                        Soma_Correcao = item.Valorcorrecao,
                        Soma_Total = Math.Round(item.Valortributo, 2,MidpointRounding.AwayFromZero)+ Math.Round(item.Valorjuros, 2, MidpointRounding.AwayFromZero)+ Math.Round(item.Valormulta, 2, MidpointRounding.AwayFromZero)+ Math.Round(item.Valorcorrecao, 2, MidpointRounding.AwayFromZero),
                        Data_Ajuizamento = item.Dataajuiza,
                        Data_Inscricao = item.Datainscricao,
                    };
                    if (item.Dataajuiza != null)
                        reg.Soma_Honorario = item.Valortotal * (decimal)0.1;
                    else
                        reg.Soma_Honorario = 0;
                    Lista_debitos.Add(reg);
                }
            }

            if (Lista_debitos.Count == 0) {
                ViewBag.Result = "Não existem débitos.";
                return View(model);
            }

            foreach (DebitoStructure item in Lista_debitos) {
                nSomaPrincipal += item.Soma_Principal;
                nSomaJuros += item.Soma_Juros;
                nSomaMulta += item.Soma_Multa;
                nSomaCorrecao += item.Soma_Correcao;
                nSomaTotal += item.Soma_Total;
                nSomaHonorario += item.Soma_Honorario;
            }

            int _linha = 1;
            List<DebitoStructureWeb> ListaWeb = new List<DebitoStructureWeb>();
            foreach (DebitoStructure item in Lista_debitos) {
                DebitoStructureWeb reg = new DebitoStructureWeb {
                    Row = _linha,
                    Ano_Exercicio = item.Ano_Exercicio,
                    Codigo_Lancamento = item.Codigo_Lancamento,
                    Descricao_Lancamento = item.Descricao_Lancamento,
                    Sequencia_Lancamento = item.Sequencia_Lancamento,
                    Numero_Parcela = item.Numero_Parcela,
                    Complemento = item.Complemento,
                    Data_Vencimento = Convert.ToDateTime(item.Data_Vencimento).ToString("dd/MM/yyyy"),
                    Soma_Principal = item.Soma_Principal.ToString("#0.00"),
                    Soma_Juros = item.Soma_Juros.ToString("#0.00"),
                    Soma_Multa = item.Soma_Multa.ToString("#0.00"),
                    Soma_Correcao = item.Soma_Correcao.ToString("#0.00"),
                    Soma_Total = item.Soma_Total.ToString("#0.00"),
                    Soma_Honorario = item.Data_Ajuizamento == null ? "0,00" : item.Soma_Honorario.ToString("#0.00"),
                    AJ = item.Data_Ajuizamento == null ? "N" : "S",
                    DA = item.Data_Inscricao == null ? "N" : "S",
                    Pt=item.Codigo_Situacao==38?"S":"N",
                    Ev=item.Codigo_Situacao==39?"S":"N"
                };
                ListaWeb.Add(reg);
                _linha++;
            }

            decimal _somaP = 0, _somaJ = 0, _somaM = 0, _somaC = 0, _somaT = 0;
            _linha = 1;
            foreach (DebitoStructure _debitos in Lista_debitos) {
                _somaP += _debitos.Soma_Principal;
                _somaJ += _debitos.Soma_Juros;
                _somaM += _debitos.Soma_Multa;
                _somaC += _debitos.Soma_Correcao;
                _somaT += _debitos.Soma_Total;
                var editorViewModel = new SelectDebitoEditorViewModel() {
                    Id = _linha,
                    Exercicio = _debitos.Ano_Exercicio,
                    Lancamento = _debitos.Codigo_Lancamento,
                    Seq = _debitos.Sequencia_Lancamento,
                    Parcela = (short)_debitos.Numero_Parcela,
                    Complemento = _debitos.Complemento,
                    Lancamento_Nome = _debitos.Descricao_Lancamento,
                    Data_Vencimento = Convert.ToDateTime(_debitos.Data_Vencimento).ToString("dd/MM/yyyy"),
                    Da=_debitos.Data_Inscricao==null?"N":"S",
                    Aj = _debitos.Data_Ajuizamento == null ? "N" : "S",
                    Selected = false,
                    Soma_Principal=_debitos.Soma_Principal,
                    Soma_Juros=_debitos.Soma_Juros,
                    Soma_Juros_Hidden=_debitos.Soma_Juros,
                    Soma_Multa=_debitos.Soma_Multa,
                    Soma_Multa_Hidden=_debitos.Soma_Multa,
                    Soma_Correcao=_debitos.Soma_Correcao,
                    Soma_Total=_debitos.Soma_Total,
                    Soma_Honorario=_debitos.Soma_Honorario,
                    Pt=_debitos.Codigo_Situacao==38?"S":"N",
                    Ep= _debitos.Codigo_Situacao == 39 ? "S" : "N"
                };
                if (Convert.ToDateTime(editorViewModel.Data_Vencimento).Year == 2020 && Convert.ToDateTime(editorViewModel.Data_Vencimento).Month>3 && Convert.ToDateTime(editorViewModel.Data_Vencimento).Month < 7) {
                    editorViewModel.Soma_Juros = 0;
                    editorViewModel.Soma_Multa = 0;
                    editorViewModel.Soma_Total = editorViewModel.Soma_Principal+editorViewModel.Soma_Correcao;
                }
                _linha++;
                model.Debito.Add(editorViewModel);
                
            }
            model.Soma_Principal = _somaP;
            model.Soma_Juros = _somaJ;
            model.Soma_Multa = _somaM;
            model.Soma_Correcao = _somaC;
            model.Soma_Total = _somaT;
            model.Soma_Juros_Hidden = _somaJ;
            model.Soma_Multa_Hidden = _somaM;
            model.Plano = nPlano;
            return View(model);
        }

        [HttpPost]
        public ActionResult SubmitSelected(DebitoSelectionViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Cidadao_bll requerenteRepository = new Cidadao_bll("GTIconnection");
            Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");

            int _codigo = model.Inscricao;
            string _endereco="",_complemento="",_cidade="",_uf="",_cep="";
            TipoCadastro _tipoCadastro = Tipo_Cadastro(_codigo);
            if (_tipoCadastro == TipoCadastro.Imovel) {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                _complemento = string.IsNullOrWhiteSpace(_imovel.Complemento) ? "" : " " + _imovel.Complemento;
                _endereco = _imovel.NomeLogradouro + ", " + _imovel.Numero.ToString() +  _complemento + " " + _imovel.NomeBairro;
                _cidade = "JABOTICABAL";
                _uf = "SP";
                _cep = _imovel.Cep;
            } else {
                if (_tipoCadastro == TipoCadastro.Empresa) {
                    EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);
                    _endereco = _empresa.Nome_logradouro + ", " + _empresa.Numero.ToString() + _empresa.Complemento == null ? "" : " " + _empresa.Complemento + " " + _empresa.Bairro_nome;
                    _cidade = _empresa.Cidade_nome;
                    _uf = _empresa.UF;
                    _cep = _empresa.Cep;
                } else {
                    CidadaoStruct _cidadao = requerenteRepository.Dados_Cidadao(_codigo);
                    _endereco = _cidadao.EnderecoR + ", " + _cidadao.NumeroR.ToString() + (_cidadao.ComplementoR == null ? "" : " " + _cidadao.ComplementoR) + " " + _cidadao.NomeBairroR;
                    _cidade = _cidadao.NomeCidadeR;
                    _uf = _cidadao.UfR;
                    if (_cidadao.CodigoCidadeR == 413)
                        _cep = (enderecoRepository.RetornaCep((int)_cidadao.CodigoLogradouroR, (short)_cidadao.NumeroR)).ToString();
                    else {
                        _cep = _cidadao.CepR.ToString();
                    }
                    if (_cep == "0")
                        _cep = "14870000";

                }
            }

            var selectedIds = model.getSelectedIds();
            var modelt = new DebitoListViewModel() {
                Nome=model.Nome,
                Inscricao=model.Inscricao,
                CpfCnpjLabel=model.CpfCnpjLabel,
                Endereco=_endereco,
                Cidade=_cidade,
                UF=_uf,
                Cep=RetornaNumero( _cep)
            };
            decimal _somaP = 0,_somaJ=0,_somaM=0,_somaC=0,_somaT=0,_somaH=0;

            bool IsRefis = false, DebitoAnoAtual = false, DebitoNoRefis=false;
            int nPlano = 0;
            decimal nPerc = 0;

            foreach (SelectDebitoEditorViewModel _debitos in model.Debito.Where(m => m.Selected == true)) {
                if (Convert.ToDateTime(_debitos.Data_Vencimento) >= Convert.ToDateTime("30/06/2020")) {
                    if (_debitos.Lancamento != 78 && _debitos.Lancamento != 41) {
                        DebitoAnoAtual = true;
                    }
                }
            }

            foreach (SelectDebitoEditorViewModel _debitos in model.Debito.Where(m => m.Selected == true)) {
                if (Convert.ToDateTime(_debitos.Data_Vencimento) < Convert.ToDateTime("30/06/2020")) {
                     DebitoNoRefis = true;
                }
            }

            if(IsRefis && DebitoNoRefis && DebitoAnoAtual) {
                ViewBag.Result = "Não é permitido emitir guia com débitos anteriores à 30/06/2020 junto com débitos posteriores, durante o período do Refis. Por favor emitir em guias separadas.";
                return View("Damc",model);
            }


            foreach (SelectDebitoEditorViewModel _debitos in model.Debito.Where(m=>m.Selected==true)) {
                var editorViewModel = new ListDebitoEditorViewModel() {
                    Exercicio = _debitos.Exercicio,
                    Lancamento = _debitos.Lancamento,
                    Seq = _debitos.Seq,
                    Parcela = (short)_debitos.Parcela,
                    Complemento = _debitos.Complemento,
                    Soma_Principal= _debitos.Soma_Principal,
                    Soma_Juros = _debitos.Soma_Juros,
                    Soma_Multa = _debitos.Soma_Multa,
                    Soma_Juros_Hidden = _debitos.Soma_Juros_Hidden,
                    Soma_Multa_Hidden = _debitos.Soma_Multa_Hidden,
                    Soma_Correcao = _debitos.Soma_Correcao,
                    Soma_Total =_debitos.Soma_Total,
                    Soma_Honorario = _debitos.Soma_Honorario,
                    Lancamento_Nome =_debitos.Lancamento_Nome,
                    Data_Vencimento=_debitos.Data_Vencimento,
                    Aj=_debitos.Aj,
                    Da=_debitos.Da
                };

                if (IsRefis && !DebitoAnoAtual) {
                    if (Convert.ToDateTime( model.Data_Vencimento) <= Convert.ToDateTime("19/10/2020")) {
                        nPerc = 1M;
                        nPlano = 41;
                    } else if (Convert.ToDateTime(model.Data_Vencimento) > Convert.ToDateTime("19/10/2020") && Convert.ToDateTime(model.Data_Vencimento) <= Convert.ToDateTime("30/11/2020")) {
                        nPerc = 0.8M;
                        nPlano = 42;
                    } else if (Convert.ToDateTime(model.Data_Vencimento) > Convert.ToDateTime("30/11/2020") && Convert.ToDateTime(model.Data_Vencimento) <= Convert.ToDateTime("22/12/2020")) {
                        nPerc = 0.7M;
                        nPlano = 43;
                    }
                    if (nPlano > 0) {
                        editorViewModel.Soma_Juros = Convert.ToDecimal(editorViewModel.Soma_Juros) - (Convert.ToDecimal(editorViewModel.Soma_Juros) * nPerc);
                        editorViewModel.Soma_Multa = Convert.ToDecimal(editorViewModel.Soma_Multa) - (Convert.ToDecimal(editorViewModel.Soma_Multa) * nPerc);
                        editorViewModel.Soma_Total = editorViewModel.Soma_Principal + editorViewModel.Soma_Juros + editorViewModel.Soma_Multa + editorViewModel.Soma_Correcao;
                        editorViewModel.Soma_Juros_Hidden = editorViewModel.Soma_Juros;
                        editorViewModel.Soma_Multa_Hidden = editorViewModel.Soma_Multa;
                        _debitos.Soma_Juros = editorViewModel.Soma_Juros;
                        _debitos.Soma_Multa = editorViewModel.Soma_Multa;
                        if(_debitos.Aj=="S")
                        _debitos.Soma_Honorario = ((editorViewModel.Soma_Principal + editorViewModel.Soma_Juros + editorViewModel.Soma_Multa + editorViewModel.Soma_Correcao)*10)/100;
                    }
                }

                _somaP += Math.Round(_debitos.Soma_Principal, 2, MidpointRounding.AwayFromZero);
                _somaJ += Math.Round(_debitos.Soma_Juros, 2, MidpointRounding.AwayFromZero);
                _somaM += Math.Round(_debitos.Soma_Multa, 2, MidpointRounding.AwayFromZero);
                _somaC += Math.Round(_debitos.Soma_Correcao, 2, MidpointRounding.AwayFromZero);
                _somaH += Math.Round(_debitos.Soma_Honorario, 2, MidpointRounding.AwayFromZero);
                _somaT += _debitos.Soma_Principal + _debitos.Soma_Juros + _debitos.Soma_Multa + _debitos.Soma_Correcao + _debitos.Soma_Honorario;
                modelt.Debito.Add(editorViewModel);
            }
            modelt.Inscricao = model.Inscricao;
            modelt.Data_Vencimento = model.Data_Vencimento;
            //modelt.Plano = model.Plano;
            modelt.Plano = nPlano;
            modelt.Soma_Principal = _somaP;
            modelt.Soma_Juros = _somaJ;
            modelt.Soma_Multa = _somaM;
            modelt.Soma_Juros_Hidden = _somaJ;
            modelt.Soma_Multa_Hidden = _somaM;
            modelt.Soma_Correcao = _somaC;
            modelt.Soma_Honorario = _somaH;
            modelt.Soma_Total = _somaP+_somaJ+_somaM+_somaC+_somaH;
            modelt.Valor_Boleto = Convert.ToInt32((_somaP + _somaJ + _somaM + _somaC + _somaH) * 100).ToString();
            TempData["debito"] = modelt;
            return RedirectToAction("Damd");
        }

        public ActionResult Damd() {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            if(TempData["debito"]==null)
                return RedirectToAction("Login_gti", "Home");
            DebitoListViewModel model =(DebitoListViewModel) TempData["debito"];
            //DebitoListViewModel model = value;

            //grava o documento
            Numdocumento docReg = new Numdocumento() { 
                Datadocumento= Convert.ToDateTime( DateTime.Now.ToString("dd/MM/yyyy")),
                Emissor = "WebDam",
                Registrado=true,
                Valorguia = model.Soma_Total
            };
            int _documento = tributarioRepository.Insert_Documento(docReg);

            //parcela x documento
            foreach (ListDebitoEditorViewModel _debitos in model.Debito) {
                Parceladocumento parcReg = new Parceladocumento() { 
                    Codreduzido=model.Inscricao,
                    Anoexercicio=(short)_debitos.Exercicio,
                    Codlancamento=(short)_debitos.Lancamento,
                    Seqlancamento=(short)_debitos.Seq,
                    Numparcela=(byte)_debitos.Parcela,
                    Codcomplemento=(byte)_debitos.Complemento,
                    Plano=Convert.ToInt16(model.Plano.ToString()),
                    Numdocumento=_documento
                };
                Exception ex = tributarioRepository.Insert_Parcela_Documento(parcReg);
            }

            if (model.Soma_Honorario > 0) {
                short _seqHon = tributarioRepository.Retorna_Ultima_Seq_Honorario(model.Inscricao, DateTime.Now.Year);
                _seqHon++;
                Debitoparcela regParcela = new Debitoparcela {
                    Codreduzido = model.Inscricao,
                    Anoexercicio = (short)DateTime.Now.Year,
                    Codlancamento = 41,
                    Seqlancamento = _seqHon,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Statuslanc = 3,
                    Datavencimento = model.Data_Vencimento,
                    Datadebase = DateTime.Now,
                    Userid = 236
                };
                Exception ex = tributarioRepository.Insert_Debito_Parcela(regParcela);
                if (ex == null) {
                    Debitotributo regTributo = new Debitotributo {
                        Codreduzido = model.Inscricao,
                        Anoexercicio = (short)DateTime.Now.Year,
                        Codlancamento = 41,
                        Seqlancamento = _seqHon,
                        Numparcela = 1,
                        Codcomplemento = 0,
                        Codtributo = 90,
                        Valortributo = model.Soma_Honorario
                    };
                    Exception ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
                    Parceladocumento parcReg = new Parceladocumento() {
                        Codreduzido = model.Inscricao,
                        Anoexercicio = (short)DateTime.Now.Year,
                        Codlancamento = 41,
                        Seqlancamento = _seqHon,
                        Numparcela = 1,
                        Codcomplemento = 0,
                        Plano = Convert.ToInt16(model.Plano.ToString()),
                        Numdocumento = _documento
                    };
                    ex2 = tributarioRepository.Insert_Parcela_Documento(parcReg);
                }
            }

            //######### Decreto 7186 ###########

            //foreach (ListDebitoEditorViewModel deb in model.Debito) {
            //    if (Convert.ToDateTime(deb.Data_Vencimento).Year == 2020 && Convert.ToDateTime(deb.Data_Vencimento).Month > 3 && Convert.ToDateTime(deb.Data_Vencimento).Month < 7) {
            //        short _seqDec = tributarioRepository.Retorna_Ultima_Seq_Decreto(model.Inscricao, DateTime.Now.Year);
            //        _seqDec++;

            //        if (deb.Soma_Multa_Hidden > 0 || deb.Soma_Juros_Hidden > 0) {
            //            Debitoparcela regParcela = new Debitoparcela {
            //                Codreduzido = model.Inscricao,
            //                Anoexercicio = 2020,
            //                Codlancamento = 85,
            //                Seqlancamento = _seqDec,
            //                Numparcela = 1,
            //                Codcomplemento = 0,
            //                Statuslanc = 3,
            //                Datavencimento = Convert.ToDateTime("30/12/2020"),
            //                Datadebase = DateTime.Now,
            //                Userid = 236
            //            };

            //            Exception ex = tributarioRepository.Insert_Debito_Parcela(regParcela);
            //        }
            //        if (deb.Soma_Multa_Hidden > 0) { 
            //            Debitotributo regTributo = new Debitotributo {
            //                Codreduzido = model.Inscricao,
            //                Anoexercicio = 2020,
            //                Codlancamento = 85,
            //                Seqlancamento = _seqDec,
            //                Numparcela = 1,
            //                Codcomplemento = 0,
            //                Codtributo = 112,
            //                Valortributo = deb.Soma_Multa_Hidden
            //            };
            //            Exception ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            //        }

            //        if (deb.Soma_Juros_Hidden > 0) {
            //            Debitotributo regTributo = new Debitotributo {
            //                Codreduzido = model.Inscricao,
            //                Anoexercicio = 2020,
            //                Codlancamento = 85,
            //                Seqlancamento = _seqDec,
            //                Numparcela = 1,
            //                Codcomplemento = 0,
            //                Codtributo = 113,
            //                Valortributo = deb.Soma_Juros_Hidden
            //            };
            //            Exception ex3 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            //        }
            //        if (deb.Soma_Multa_Hidden > 0 || deb.Soma_Juros_Hidden > 0) {
            //            Encargo_cvd regCvd = new Encargo_cvd {
            //                Codigo = model.Inscricao,
            //                Exercicio = (short)deb.Exercicio,
            //                Lancamento = (short)deb.Lancamento,
            //                Sequencia = (short)deb.Seq,
            //                Parcela = (byte)deb.Parcela,
            //                Complemento = (byte)deb.Complemento,
            //                Exercicio_enc = 2020,
            //                Lancamento_enc = 85,
            //                Sequencia_enc = _seqDec,
            //                Parcela_enc = 1,
            //                Complemento_enc = 0,
            //                Documento = _documento
            //            };
            //            Exception ex = tributarioRepository.Insert_Encargo_CVD(regCvd);

            //            ex = tributarioRepository.Atualiza_Plano_Documento(_documento, 40);
            //        }

            //    }
            //}
            //##################################
            model.Data_Vencimento_String = Convert.ToDateTime(model.Data_Vencimento.ToString()).ToString("ddMMyyyy");
            model.RefTran = "287353200" + _documento.ToString();
            if (model == null)
                return RedirectToAction("Login_gti", "Home");
            else
                return View(model);
        }

        
        [HttpPost]
        [Route("Validate_CDoc")]
        [Route("Certidao/Validate_CDoc")]
        public ActionResult Validate_CDoc(CertidaoViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _codigo, _ano, _numero;
            string _chave = model.Chave;

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.Chave != null) {
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Debito_Doc", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;
                    List<Certidao> certidao = new List<Certidao>();
                    Certidao_debito_doc _dados = tributarioRepository.Retorna_Certidao_Debito_Doc(model.Chave);
                    if (_dados != null) {
                        Certidao reg = new Certidao() {
                            Codigo = _codigo,
                            Nome_Requerente = _dados.Nome,
                            Ano = _ano,
                            Numero = _numero,
                            Numero_Ano = _dados.Numero.ToString("00000") + "/" + _dados.Ano.ToString(),
                            Controle = _chave,
                            Tributo=_dados.Tributo,
                            Cpf_Cnpj = _dados.Cpf_cnpj,
                            Data_Geracao=_dados.Data_emissao,
                            Tipo_Certidao=_dados.Ret==1?"Negativa":_dados.Ret==2?"Positiva":"Positiva com Efeito Negativa"
                        };
                        certidao.Add(reg);
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Certidao_Inscricao", model);
                    }

                    ReportDocument rd = new ReportDocument();
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/CertidaoDebitoDocValida.rpt"));

                    try {
                        rd.SetDataSource(certidao);
                        Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "Certidao_Debito.pdf");
                    } catch {

                        throw;
                    }
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Inscricao", model);
            }
        }

        [Route("GateBank")]
        [HttpGet]
        public ActionResult GateBank(string p1,string p2,string p3,string p4,string p5,string p6,string p7,string p8,string p9) {
            /*p1-nome,p2-endereco,p3-data dam,p4-documento,p5-nosso numero,p6-valor,p7-cidade,p8-uf,p9-cep*/


            if (string.IsNullOrWhiteSpace(p1)) {
                return RedirectToAction("Login_gti", "Home");
            }
            BoletoViewModel model = new BoletoViewModel();
            TAcessoFunction tAcesso_Class = new TAcessoFunction();
            try {
                //string _nosso_numero = "287353200" + tAcesso_Class.DecryptGTI(p5);
                //string _valor_boleto = tAcesso_Class.DecryptGTI(p6);
                //string _valor_boleto_full = (Convert.ToDecimal(_valor_boleto) / 100).ToString("#0.00");
                //string _cpf = tAcesso_Class.DecryptGTI(p2);
                //string _tipodoc = _cpf.Length == 11 ? "1" : "2";
                //model.Nome = tAcesso_Class.DecryptGTI(p1);
                //model.CpfCnpj = _cpf;
                //model.TipoDoc = _tipodoc;
                //model.Endereco = tAcesso_Class.DecryptGTI(p2);
                //model.Cep = tAcesso_Class.DecryptGTI(p4);
                //model.Data_Vencimento = tAcesso_Class.DecryptGTI(p3);
                //model.Valor_Boleto = _valor_boleto;
                //model.Nosso_Numero = _nosso_numero;
                //model.Cidade = tAcesso_Class.DecryptGTI(p7);
                //model.Uf = tAcesso_Class.DecryptGTI(p8);
                //model.Valor_Boleto_Full = _valor_boleto_full;
                //model.Cep= tAcesso_Class.DecryptGTI(p9);

                if (p3.Length > 8)
                    p3 = p3.Substring(0, 2) + p3.Substring(3, 2) + StringRight(p3, 4);

                string _nosso_numero = p5;
                string _valor_boleto = p6;
                string _valor_boleto_full = (Convert.ToDecimal(_valor_boleto) / 100).ToString("#0.00");
                string _cpf = p4;
                string _tipodoc = _cpf.Length == 11 ? "1" : "2";
                model.Nome = p1;
                model.CpfCnpj = _cpf;
                model.TipoDoc = _tipodoc;
                model.Endereco = p2;
                model.Cep = p4;
                model.Data_Vencimento = p3;
                model.Valor_Boleto = _valor_boleto;
                model.Nosso_Numero = _nosso_numero;
                model.Cidade = p7;
                model.Uf = p8;
                model.Cep = p9;
                model.Valor_Boleto_Full = _valor_boleto_full;
            } catch (Exception) {
                throw;
            }
            return View(model);
        }

        //public async Task<HttpResponseMessage> GateBank(DebitoViewModel model, int Codigo = 0) {
        //    HttpClient client = new HttpClient();
        //    var content = new FormUrlEncodedContent(new[]{
        //        new KeyValuePair<string, string>("msgLoja", "RECEBER SOMENTE ATE O VENCIMENTO, APOS ATUALIZAR O BOLETO NO SITE www.jaboticabal.sp.gov.br"),
        //        new KeyValuePair<string, string>("cep", "15990450"),
        //        new KeyValuePair<string, string>("uf", "SP"),
        //        new KeyValuePair<string, string>("cidade", "JABOTICABAL"),
        //        new KeyValuePair<string, string>("endereco", "av. rua 1"),
        //        new KeyValuePair<string, string>("nome", "DEBORA FARINELLI"),
        //        new KeyValuePair<string, string>("urlInforma", "www.google.com"),
        //        new KeyValuePair<string, string>("urlRetorno", "www.google.com"),
        //        new KeyValuePair<string, string>("tpDuplicata", "DS"),
        //        new KeyValuePair<string, string>("dataLimiteDesconto", "0"),
        //        new KeyValuePair<string, string>("valorDesconto", "0"),
        //        new KeyValuePair<string, string>("indicadorPessoa", "1"),
        //        new KeyValuePair<string, string>("cpfCnpj", "15172927867"),
        //        new KeyValuePair<string, string>("tpPagamento", "2"),
        //        new KeyValuePair<string, string>("dtVenc", "24042020"),
        //        new KeyValuePair<string, string>("qtdPontos", "0"),
        //        new KeyValuePair<string, string>("valor", "5321"),
        //        new KeyValuePair<string, string>("refTran", "28735320016301528"),
        //        new KeyValuePair<string, string>("idConv", "317203")
        //    });
        //    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        //    return await client.PostAsync("https://mpag.bb.com.br/site/mpag/", content);
        //}

        [Route("SegundaVia_Parcelamento")]
        [HttpGet]
        public ViewResult SegundaVia_Parcelamento() {
            DebitoViewModel model = new DebitoViewModel();
            return View(model);
        }

        [Route("SegundaVia_Parcelamento")]
        [HttpPost]
        public ActionResult SegundaVia_Parcelamento(DebitoViewModel model,string action) {

            if(action== "btnDigito") {
                string _antigo = model.ProcessoAntigo;
                if (string.IsNullOrEmpty(_antigo)) {
                    ViewBag.Result = "Nº do processo antigo inválido.";
                    return View(model);
                } else {
                    Processo_bll processoRepository = new Processo_bll("GTIconnection");
                    string _novo = processoRepository.ValidaProcessoAntigo(_antigo);
                    if (_novo=="") {
                        ViewBag.Result = "Nº do processo antigo inválido.";
                        return View(model);
                    } else {
                        ViewBag.Result = "";
                        model.ProcessoNovo = _novo;
                        return View(model);
                    }
                }
            }
            model.ProcessoAntigo =null;
            model.ProcessoNovo = null;

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            string _nome, _endereco, _endereco_rua, _complemento, _bairro, _cpfcnpj, _cidade, _cep, _uf;
            short _numero, _totParcela;
            int _codigo = Convert.ToInt32(model.Inscricao);
            string _processo = model.Numero_Processo;

            if (_codigo < 100000) {
                Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
                bool ExisteImovel = imovel_Class.Existe_Imovel(_codigo);
                if (!ExisteImovel) {
                    ViewBag.Result = "Inscrição não cadastrada.";
                    return View(model);
                } else {
                    ImovelStruct reg = imovel_Class.Dados_Imovel(_codigo);
                    List<ProprietarioStruct> regProp = imovel_Class.Lista_Proprietario(_codigo, true);
                    _endereco = reg.NomeLogradouro + ", " + reg.Numero + " " + reg.Complemento;
                    _endereco_rua = reg.NomeLogradouro;
                    _numero = (short)reg.Numero;
                    _complemento = reg.Complemento;
                    _bairro = reg.NomeBairro;
                    _cidade = "JABOTICABAL/SP";
                    _uf = "SP";
                    _cep = reg.Cep;
                    _nome = regProp[0].Nome;
                    _cpfcnpj = regProp[0].CPF;
                }
            } else {
                if (_codigo >= 100000 && _codigo < 500000) {
                    Empresa_bll empresa_Class = new Empresa_bll("GTIconnection");
                    bool ExisteEmpresa = empresa_Class.Existe_Empresa(_codigo);
                    if (!ExisteEmpresa) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);
                    } else {
                        EmpresaStruct _empresa = empresa_Class.Retorna_Empresa(_codigo);
                        _nome = _empresa.Razao_social;
                        _endereco = _empresa.Endereco_nome + ", " + _empresa.Numero.ToString() + " " + _empresa.Complemento;
                        _endereco_rua = _empresa.Endereco_nome;
                        _numero = (short)_empresa.Numero;
                        _complemento = _empresa.Complemento;
                        _bairro = _empresa.Bairro_nome;
                        _cidade = _empresa.Cidade_nome + "/" + _empresa.UF;
                        _uf = _empresa.UF;
                        _cep = _empresa.Cep;
                        _cpfcnpj = _empresa.Cpf_cnpj;
                    }
                } else {
                    Cidadao_bll cidadao_Class = new Cidadao_bll("GTIconnection");
                    bool ExisteCidadao = cidadao_Class.ExisteCidadao(_codigo);
                    if (!ExisteCidadao) {
                        ViewBag.Result = "Inscrição não cadastrada.";
                        return View(model);
                    } else {
                        CidadaoStruct reg = cidadao_Class.LoadReg(_codigo);
                        if (reg.EtiquetaR != null && reg.EtiquetaR == "S") {
                            _endereco = reg.EnderecoR + ", " + reg.NumeroR + " " + reg.ComplementoR;
                            _endereco_rua = reg.EnderecoR;
                            _numero = (short)reg.NumeroR;
                            _complemento = reg.ComplementoR;
                            _bairro = reg.NomeBairroR;
                            _cidade = reg.NomeCidadeR + "/" + reg.UfR;
                            _cep = reg.CepR.ToString();
                            _uf = reg.UfR;
                        } else {
                            _endereco = reg.EnderecoC + ", " + reg.NumeroC + " " + reg.ComplementoC;
                            _endereco_rua = reg.EnderecoC;
                            _numero = (short)reg.NumeroC;
                            _complemento = reg.ComplementoC;
                            _bairro = reg.NomeBairroC;
                            _cidade = reg.NomeCidadeC + "/" + reg.UfC;
                            _cep = reg.CepC.ToString();
                            _uf = reg.UfC;
                        }
                        _nome = reg.Nome;
                        if (!string.IsNullOrWhiteSpace(reg.Cnpj))
                            _cpfcnpj = reg.Cnpj;
                        else
                            _cpfcnpj = reg.Cpf;
                    }
                }
            }

            _processo = _processo.Substring(0, _processo.LastIndexOf('-')) + _processo.Substring(_processo.Length - 5, 5);
            Tributario_bll tributario_class = new Tributario_bll("GTIconnection");
            List<Destinoreparc> Lista = tributario_class.Lista_Destino_Parcelamento(_processo);
            if (Lista.Count == 0) {
                ViewBag.Result = "Processo não cadastrado.";
                return View(model);
            } else {
                int _codigoParc = Lista[0].Codreduzido;
                if (_codigoParc != _codigo) {
                    ViewBag.Result = "Processo não pertence a esta inscrição.";
                    return View(model);
                } else {
                    Processoreparc pr = tributario_class.Retorna_Processo_Parcelamento(_processo);
                    _totParcela = (short)pr.Qtdeparcela;

                    int _seq = Lista[0].Numsequencia;
                    List<DebitoStructure> ListaDebito = tributario_class.Lista_Parcelas_Parcelamento_Ano(_codigo, 2021, _seq);
                    if (ListaDebito.Count == 0) {
                        ViewBag.Result = "Não existem parcelas a serem impressas.";
                        return View(model);
                    } else {
                        

                        bool _find = false;
                        foreach (DebitoStructure itemtmp in ListaDebito) {
                            if (itemtmp.Codigo_Situacao < 3) {
                                _find = true;
                                break;
                            }
                        }
                        if (ListaDebito[0].Numero_Parcela > 1)
                            _find = true;
                        if (!_find) {
                            ViewBag.Result = "Liberação do carnê somente após o pagamento da primeira parcela.";
                            return View(model);
                        } else {
                            string _descricao_lancamento = "PARCELAMENTO DE DÉBITOS";
                            int nSid = Functions.GetRandomNumber();
                            int nPos = 0;
                            foreach (DebitoStructure item in ListaDebito.Where(m => m.Codigo_Situacao == 3)) {

                                //criamos um documento novo para cada parcela da vigilância
                                Numdocumento regDoc = new Numdocumento {
                                    Valorguia = item.Soma_Principal,
                                    Emissor = "Gti.Web/2ViaVS",
                                    Datadocumento = DateTime.Now,
                                    Registrado = false,
                                    Percisencao = 0
                                };
                                regDoc.Percisencao = 0;
                                int _novo_documento = tributario_class.Insert_Documento(regDoc);
                                regDoc.numdocumento = _novo_documento;
                                for (int i = 0; i < ListaDebito.Count; i++) {
                                    if (ListaDebito[i].Numero_Parcela == item.Numero_Parcela) {
                                        ListaDebito[i].Numero_Documento = _novo_documento;
                                        break;
                                    }
                                }
                                //}


                                //grava o documento na parcela
                                Parceladocumento regParc = new Parceladocumento {
                                    Codreduzido = item.Codigo_Reduzido,
                                    Anoexercicio = Convert.ToInt16(item.Ano_Exercicio),
                                    Codlancamento = Convert.ToInt16(item.Codigo_Lancamento),
                                    Seqlancamento = Convert.ToInt16(item.Sequencia_Lancamento),
                                    Numparcela = Convert.ToByte(item.Numero_Parcela),
                                    Codcomplemento = Convert.ToByte(item.Complemento),
                                    Numdocumento = _novo_documento,
                                    Valorjuros = 0,
                                    Valormulta = 0,
                                    Valorcorrecao = 0,
                                    Plano = 0
                                };
                                tributario_class.Insert_Parcela_Documento(regParc);

                                //Registrar os novos documentos
                                Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                                    Nome = _nome.Length > 40 ? _nome.Substring(0, 40) : _nome,
                                    Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco,
                                    Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro,
                                    Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade,
                                    Cep = _cep ?? "14870000",
                                    Cpf = _cpfcnpj,
                                    Numero_documento = _novo_documento,
                                    Data_vencimento = Convert.ToDateTime(item.Data_Vencimento),
                                    Valor_documento = Convert.ToDecimal(item.Soma_Principal),
                                    Uf = _uf
                                };
                                if (item.Data_Vencimento >= Convert.ToDateTime( DateTime.Now.ToString("dd/MM/yyyy"))) {
                                    Exception ex = tributario_class.Insert_Ficha_Compensacao_Documento(ficha);
                                    if (ex == null)
                                        ex = tributario_class.Marcar_Documento_Registrado(_novo_documento);
                                }
                                nPos++;
                            }

                            short _index = 0;
                            string _convenio = "2873532";
                                List<Boletoguia> ListaBoleto = new List<Boletoguia>();
                                foreach (DebitoStructure item in ListaDebito.Where(m => m.Codigo_Situacao == 3)) {
                                    if (item.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"))) {
                                    Boletoguia reg = new Boletoguia {
                                        Usuario = "Gti.Web/LibParc",
                                        Computer = "web",
                                        Sid = nSid,
                                        Seq = _index,
                                        Codreduzido = _codigo.ToString("000000"),
                                        Nome = _nome,
                                        Cpf = _cpfcnpj,
                                        Numimovel = _numero,
                                        Endereco = _endereco_rua,
                                        Complemento = _complemento,
                                        Bairro = _bairro,
                                        Cidade = "JABOTICABAL",
                                        Uf = "SP",
                                        Cep = _cep,
                                        Desclanc = _descricao_lancamento,
                                        Fulllanc = _descricao_lancamento,
                                        Numdoc = item.Numero_Documento.ToString(),
                                        Numparcela = (short)item.Numero_Parcela,
                                        Datadoc = DateTime.Now,
                                        Datavencto = item.Data_Vencimento,
                                        Numdoc2 = item.Numero_Documento.ToString(),
                                        Valorguia = item.Soma_Principal,
                                        Valor_ISS = 0,
                                        Valor_Taxa = 0,
                                        Totparcela = _totParcela,
                                        Obs = "Referente ao parcelamento de débitos: processo nº " + _processo,
                                        Numproc = _processo
                                    };

                                    //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
                                    DateTime _data_base = Convert.ToDateTime("07/10/1997");
                                        TimeSpan ts = Convert.ToDateTime(item.Data_Vencimento) - _data_base;
                                        int _fator_vencto = ts.Days;
                                        string _quinto_grupo = String.Format("{0:D4}", _fator_vencto);
                                        string _valor_boleto_str = string.Format("{0:0.00}", reg.Valorguia);
                                        _quinto_grupo += string.Format("{0:D10}", Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
                                        string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}", Convert.ToInt32(_convenio));
                                        _barra += String.Format("{0:D10}", Convert.ToInt64(reg.Numdoc)) + "17";
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
                                        reg.Totparcela = (short)ListaDebito.Count;
                                        if (item.Numero_Parcela == 0) {
                                            reg.Parcela = "Única";
                                        } else
                                            reg.Parcela = reg.Numparcela.ToString("00") + "/" + _totParcela.ToString("00");

                                    reg.Digitavel = _linha_digitavel;
                                    reg.Codbarra = _codigo_barra;
                                    reg.Nossonumero = _convenio + String.Format("{0:D10}", Convert.ToInt64(reg.Numdoc));
                                    ListaBoleto.Add(reg);
                                    _index++;
                                }
                            }
                            string mimeType = string.Empty;
                            string encoding = string.Empty;
                            string extension = string.Empty;
                            Session["sid"] = "";
                            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
                            if (ListaBoleto.Count > 0) {
                                tributario_Class.Insert_Carne_Web(Convert.ToInt32(ListaBoleto[0].Codreduzido), DateTime.Now.Year);
                                DataSet Ds = Functions.ToDataSet(ListaBoleto);
                                ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia", Ds.Tables[0]);
                                ReportViewer viewer = new ReportViewer();
                                viewer.LocalReport.Refresh();
                                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_Parcelamento.rdlc"); ;
                                viewer.LocalReport.DataSources.Add(rdsAct);
                                byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out string[] streamIds, out Warning[] warnings);

                                Response.Buffer = true;
                                Response.Clear();
                                Response.ContentType = mimeType;
                                Response.AddHeader("content-disposition", "attachment; filename= guia_pmj" + "." + extension);
                                Response.OutputStream.Write(bytes, 0, bytes.Length);
                                Response.Flush();
                                Response.End();
                            }
                        }
                        // }
                    }
                }
            }

            ViewBag.Result = "Não existem parcelas a serem impressas.";
            return View(model);
        }

        [Route("Guia")]
        [HttpGet]
        public ViewResult Guia() {
            DebitoViewModel model = new DebitoViewModel();
            return View(model);
        }

        [Route("Detalhe_Boleto")]
        [HttpGet]
        public ViewResult Detalhe_Boleto() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Detalhe_Boleto")]
        [HttpPost]
        public ActionResult Detalhe_Boleto(CertidaoViewModel model) {
            string _cpfMask = model.CpfCnpjLabel;
            if (Functions.ValidaCNPJ(_cpfMask.PadLeft(14, '0'))) {
                _cpfMask = _cpfMask.PadLeft(14, '0');
            } else {
                if (Functions.ValidaCpf(_cpfMask.PadLeft(11, '0'))) {
                    _cpfMask = _cpfMask.PadLeft(11, '0');
                }
            }

            string _cpf = "", _cnpj = "";
            if (_cpfMask.Length == 11)
                _cpf = _cpfMask;
            else
                _cnpj = _cpfMask;
            //            _cnpj = model.CnpjValue;
            bool _bCpf = _cpf != "";
            int _documento;
            if (model.Documento.Length < 8) {
                ViewBag.Result = "Nº de documento inválido.";
                return View(model);
            } else
                if (model.Documento.Length > 8)
                _documento = Convert.ToInt32(model.Documento.Substring(model.Documento.Length - 8, 8));
            else
                _documento = Convert.ToInt32(model.Documento);

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            bool _existe = tributarioRepository.Existe_Documento(_documento);
            if (!_existe) {
                ViewBag.Result = "Documento não cadastrado.";
                return View(model);
            }

            int _codigo = tributarioRepository.Retorna_Codigo_por_Documento(_documento);
            Numdocumento DadosDoc = tributarioRepository.Retorna_Dados_Documento(_documento);
            DateTime dDataDoc = Convert.ToDateTime(DadosDoc.Datadocumento);
            decimal nValorGuia = Convert.ToDecimal(DadosDoc.Valorguia);

            if (_codigo < 100000) {
                Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
                ImovelStruct reg = imovel_Class.Dados_Imovel(_codigo);
                List<ProprietarioStruct> regProp = imovel_Class.Lista_Proprietario(_codigo, true);
                if (_bCpf) {
                    if (Convert.ToInt64(Functions.RetornaNumero(regProp[0].CPF)).ToString("00000000000") != _cpf) {
                        ViewBag.Result = "CPF informado não pertence a este documento.";
                        return View(model);
                    }
                } else {
                    if (Convert.ToInt64(Functions.RetornaNumero(regProp[0].CPF)).ToString("00000000000000") != _cnpj) {
                        ViewBag.Result = "CNPJ informado não pertence a este documento.";
                        return View(model);
                    }
                }
            } else {
                if (_codigo >= 100000 && _codigo < 500000) {
                    Empresa_bll empresa_Class = new Empresa_bll("GTIconnection");
                    EmpresaStruct reg = empresa_Class.Retorna_Empresa(_codigo);
                    if (_bCpf) {
                        if (Convert.ToInt64(Functions.RetornaNumero(reg.Cpf_cnpj)).ToString("00000000000") != _cpf) {
                            ViewBag.Result = "CPF informado não pertence a este documento.";
                            return View(model);
                        }
                    } else {
                        if (Convert.ToInt64(Functions.RetornaNumero(reg.Cpf_cnpj)).ToString("00000000000000") != _cnpj) {
                            ViewBag.Result = "CNPJ informado não pertence a este documento.";
                            return View(model);
                        }
                    }
                } else {
                    Cidadao_bll cidadao_Class = new Cidadao_bll("GTIconnection");
                    CidadaoStruct reg = cidadao_Class.LoadReg(_codigo);
                    if (_bCpf) {
                        if (Convert.ToInt64(Functions.RetornaNumero(reg.Cpf)).ToString("00000000000") != _cpf) {
                            ViewBag.Result = "CPF informado não pertence a este documento.";
                            return View(model);
                        }
                    } else {
                        if (Convert.ToInt64(Functions.RetornaNumero(reg.Cnpj)).ToString("00000000000000") != _cnpj) {
                            ViewBag.Result = "CNPJ informado não pertence a este documento.";
                            return View(model);
                        }
                    }
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            ViewBag.Result = "";

            List<DebitoStructure> ListaParcelas = Carregaparcelas(_documento, dDataDoc);
            int nSid = tributarioRepository.Insert_Boleto_DAM(ListaParcelas, _documento, dDataDoc);
            Session["sid"] = "";
            Tributario_bll tributario_Repository = new Tributario_bll("GTIconnection");
            List<GTI_Models.Models.Boleto> ListaBoleto = tributarioRepository.Lista_Boleto_DAM(nSid);
            DataSet Ds = Functions.ToDataSet(ListaBoleto);
            ReportDataSource rdsAct = new ReportDataSource("dsDam", Ds.Tables[0]);
            ReportViewer viewer = new ReportViewer();
            viewer.LocalReport.Refresh();
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/rptDetalheBoleto.rdlc"); ;
            viewer.LocalReport.DataSources.Add(rdsAct);
            byte[] bytes = viewer.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
            tributario_Repository.Excluir_Carne(nSid);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename= detalhe_guia" + "." + extension);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            return View(model);
        }

        private List<DebitoStructure> Carregaparcelas(int nNumDoc, DateTime dDataDoc) {
            int i = 0;
            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            List<DebitoStructure> ListaParcelas = tributario_Class.Lista_Tabela_Parcela_Documento(nNumDoc);

            short _plano = tributario_Class.Retorna_Plano_Desconto(nNumDoc);
            foreach (DebitoStructure Linha in ListaParcelas) {
                if (_plano > 0) {
                    decimal _perc = tributario_Class.Retorna_Plano_Desconto_Perc(_plano);
                    ListaParcelas[i].Soma_Juros = Convert.ToDecimal(ListaParcelas[i].Soma_Juros) - (Convert.ToDecimal(ListaParcelas[i].Soma_Juros) * _perc/100);
                    ListaParcelas[i].Soma_Multa = Convert.ToDecimal(ListaParcelas[i].Soma_Multa) - (Convert.ToDecimal(ListaParcelas[i].Soma_Multa) * _perc/100);
                    ListaParcelas[i].Soma_Total = ListaParcelas[i].Soma_Principal + ListaParcelas[i].Soma_Correcao;
                }


                List<SpExtrato> ListaTributo = tributario_Class.Lista_Extrato_Tributo(Linha.Codigo_Reduzido, (short)Linha.Ano_Exercicio, (short)Linha.Ano_Exercicio, (short)Linha.Codigo_Lancamento, (short)Linha.Codigo_Lancamento,
                    (short)Linha.Sequencia_Lancamento, (short)Linha.Sequencia_Lancamento, (short)Linha.Numero_Parcela, (short)Linha.Numero_Parcela, Linha.Complemento, Linha.Complemento, 0, 99, dDataDoc, "Web");
                List<SpExtrato> ListaParcela = tributario_Class.Lista_Extrato_Parcela(ListaTributo);

                for (i = 0; i < ListaParcelas.Count; i++) {
                    if (ListaParcelas[i].Ano_Exercicio == Linha.Ano_Exercicio & ListaParcelas[i].Codigo_Lancamento == Linha.Codigo_Lancamento & ListaParcelas[i].Sequencia_Lancamento == Linha.Sequencia_Lancamento &
                        ListaParcelas[i].Numero_Parcela == Linha.Numero_Parcela & ListaParcelas[i].Complemento == Linha.Complemento)
                        break;
                }
                ListaParcelas[i].Soma_Principal = ListaParcela[0].Valortributo;
                ListaParcelas[i].Soma_Multa = ListaParcela[0].Valormulta;
                ListaParcelas[i].Soma_Juros = ListaParcela[0].Valorjuros;
                ListaParcelas[i].Soma_Correcao = ListaParcela[0].Valorcorrecao;
                ListaParcelas[i].Soma_Total = ListaParcela[0].Valortotal;
                ListaParcelas[i].Descricao_Lancamento = ListaParcela[0].Desclancamento;
                string DescTributo = "";


                List<int> aTributos = new List<int>();
                foreach (SpExtrato Trib in ListaTributo) {
                    bool bFind = false;
                    for (int b = 0; b < aTributos.Count; b++) {
                        if (aTributos[b] == Trib.Codtributo) {
                            bFind = true;
                            break;
                        }
                    }
                    if (!bFind)
                        aTributos.Add(Trib.Codtributo);
                }

                for (int c = 0; c < aTributos.Count; c++)
                    DescTributo += aTributos[c].ToString("000") + "-" + tributario_Class.Lista_Tributo(aTributos[c])[0].Abrevtributo + ",";

                DescTributo = DescTributo.Substring(0, DescTributo.Length - 1);
                ListaParcelas[i].Descricao_Tributo = DescTributo;
                ListaParcelas[i].Data_Vencimento = ListaParcela[0].Datavencimento;
            }

            return ListaParcelas;

        }

        [Route("Notificacao_iss")]
        [HttpGet]
        public ActionResult Notificacao_iss() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoIssViewModel model = new NotificacaoIssViewModel();

            List<Usoconstr> Lista_Uso = new List<Usoconstr> {
                new Usoconstr() { Codusoconstr = 1, Descusoconstr = "Residencial" },
                new Usoconstr() { Codusoconstr = 2, Descusoconstr = "Industrial" },
                new Usoconstr() { Codusoconstr = 3, Descusoconstr = "Comercial" }
            };
            ViewBag.Lista_Uso = new SelectList(Lista_Uso, "Codusoconstr", "Descusoconstr");

            List<Categconstr> Lista_Cat = new List<Categconstr>();

            List<int> Lista_Ano = new List<int>();
            for (int i = 2000; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            DateTime _data = DateTime.Now.AddDays(30);
            model.Data_vencimento = _data;
            ViewBag.Lista_Cat = new SelectList(Lista_Cat, "Codcategconstr", "Desccategconstr");
            return View(model);
        }

        [Route("Notificacao_iss")]
        [HttpPost]
        public ActionResult Notificacao_iss(NotificacaoIssViewModel model,string action) {
            List<Usoconstr> Lista_Uso = new List<Usoconstr> {
                new Usoconstr() { Codusoconstr = 1, Descusoconstr = "Residencial" },
                new Usoconstr() { Codusoconstr = 2, Descusoconstr = "Industrial" },
                new Usoconstr() { Codusoconstr = 3, Descusoconstr = "Comercial" }
            };
            ViewBag.Lista_Uso = new SelectList(Lista_Uso, "Codusoconstr", "Descusoconstr");

            List<int> Lista_Ano = new List<int>();
            for (int i = 2000; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            List<Categconstr> Lista_Cat = new List<Categconstr>();
            switch (model.Uso_Construcao) {
                case 1:
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 179, Desccategconstr = "Baixo" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 691, Desccategconstr = "Popular" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 180, Desccategconstr = "Médio" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 181, Desccategconstr = "Alto" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 676, Desccategconstr = "Fino" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 670, Desccategconstr = "Luxuoso" });
                    break;
                case 2:
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 185, Desccategconstr = "Único" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 671, Desccategconstr = "Barracão" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 672, Desccategconstr = "Popular" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 673, Desccategconstr = "Médio" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 674, Desccategconstr = "Bom" });
                    break;
                case 3:
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 182, Desccategconstr = "Baixo" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 689, Desccategconstr = "Barracão" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 690, Desccategconstr = "Popular" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 183, Desccategconstr = "Médio" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 184, Desccategconstr = "Alto" });
                    Lista_Cat.Add(new Categconstr() { Codcategconstr = 675, Desccategconstr = "Fino" });
                    break;
                default:
                    break;
            }

            ViewBag.Lista_Cat = new SelectList(Lista_Cat, "Codcategconstr", "Desccategconstr");

            if (model.Categoria_Construcao > 0) {
                decimal _valor = tributarioRepository.Retorna_Valor_Tributo(model.Ano_Notificacao, model.Categoria_Construcao);
                model.Valor_m2 = Math.Round(_valor, 2, MidpointRounding.AwayFromZero);
                model.Valor_Total = Math.Round((model.Valor_m2 * model.Area_Notificada)-model.Iss_Pago, 2, MidpointRounding.AwayFromZero);
            } else {
                model.Valor_m2 = 0;
                model.Valor_Total = 0;
            }

            if (action == "btnValida") {
                if (model.Valor_Total == 0) {
                    ViewBag.Result = "Valor da notificação não informado.";
                    return View(model);
                }

                bool _isdate = IsDate(model.Data_vencimento);
                if (!_isdate) {
                    ViewBag.Result = "Data de vencimento inválida.";
                    return View(model);
                }

                DateTime _dataVencto = Convert.ToDateTime(Convert.ToDateTime(model.Data_vencimento).ToString("dd/MM/yyyy"));
                DateTime _dataAtual = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                if (_dataVencto < _dataAtual) {
                    ViewBag.Result = "Data de vencimento inferior a data atual.";
                    return View(model);
                }

                if (_dataVencto < _dataAtual) {
                    ViewBag.Result = "Data de vencimento inferior a data atual.";
                    return View(model);
                }

                bool _existeNumero = tributarioRepository.Existe_NotificacaoISS_Numero(model.Ano_Notificacao, model.Numero_Notificacao);
                if (_existeNumero) {
                    ViewBag.Result = "Nº de notificação já cadastrado.";
                    return View(model);
                }

                //double _days = (_dataVencto - _dataAtual).TotalDays;
                //if (_days > 30) {
                //    ViewBag.Result = "Data de vencimento superior a 30 dias.";
                //    return View(model);
                //}

                Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
                bool _existe = cidadaoRepository.ExisteCidadao(model.Codigo_Cidadao);
                if (!_existe) {
                    ViewBag.Result = "Código cidadão não cadastrado.";
                    return View(model);
                }

                Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
                _existe = imovelRepository.Existe_Imovel(model.Codigo_Imovel);

                if (!_existe) {
                    ViewBag.Result = "Imóvel não cadastrado.";
                    return View(model);
                }

                EnderecoStruct _imovel = imovelRepository.Dados_Endereco(model.Codigo_Imovel,TipoEndereco.Local);
                CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(model.Codigo_Cidadao);
                string _bairro = "",_endereco="",_compl="",_cidade="JABOTICABAL",_nome="";
                string _cpf_cnpj = string.IsNullOrWhiteSpace(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                int _cep = 14870000, _codigo = model.Codigo_Cidadao,_fiscal= Convert.ToInt32(Session["hashid"]);
                short _ano=(short)model.Ano_Notificacao;
                _nome = _cidadao.Nome;
                _endereco = _imovel.Endereco ;
                _bairro = _imovel.NomeBairro;
                int _numero = (int)_imovel.Numero;
                _compl = _imovel.Complemento;
                _cidade = "JABOTICABAL";
                _cep = Convert.ToInt32(_imovel.Cep);

                if(string.IsNullOrEmpty(_endereco) || string.IsNullOrEmpty(_bairro)) {
                    ViewBag.Result = "O Contribuinte possui endereço incompleto.";
                    return View(model);
                }

                if (string.IsNullOrEmpty(_cpf_cnpj)) {
                    ViewBag.Result = "O Cpf/Cnpj do Contribuinte é inválido.";
                    return View(model);
                }

                //***************************************************************************
                //Grava os dados do boleto e gera o lançamento
                //***************************************************************************
                _nome = _nome.Length > 40 ? _nome.Substring(0, 40) : _nome;
                _endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco;
                _bairro = _bairro.Length > 40 ? _bairro.Substring(0, 40) : _bairro;
                _cidade = _cidade.Length > 40 ? _cidade.Substring(0, 40) : _cidade;

                short _seq = tributarioRepository.Retorna_Proxima_Seq_NotificacaoIssWeb(_codigo, _ano);
                //grava parcela
                Debitoparcela regParcela = new Debitoparcela {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 65,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Statuslanc = 3,
                    Datavencimento = _dataVencto,
                    Datadebase = DateTime.Now,
                    Userid = _fiscal
                };
                Exception ex2 = tributarioRepository.Insert_Debito_Parcela(regParcela);

                //grava tributo
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 65,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)model.Categoria_Construcao,
                    Valortributo = model.Valor_Total
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);

                //grava o documento
                Numdocumento regDoc = new Numdocumento {
                    Valorguia = model.Valor_Total,
                    Emissor = "Gti.Web/NotificaoIss",
                    Datadocumento = DateTime.Now,

                    Registrado = true,
                    Percisencao = 0
                };
                regDoc.Percisencao = 0;
                int _novo_documento = tributarioRepository.Insert_Documento(regDoc);
                //int _novo_documento = 17888999;

                //grava o documento na parcela
                Parceladocumento regParc = new Parceladocumento {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 65,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Numdocumento = _novo_documento,
                    Valorjuros = 0,
                    Valormulta = 0,
                    Valorcorrecao = 0,
                    Plano = 0
                };
                tributarioRepository.Insert_Parcela_Documento(regParc);

                string sHist = "Iss construção civil lançado no código " + _codigo + " processo nº " + model.Numero_Processo + " notificação nº " + model.Numero_Notificacao.ToString("0000") + "/" + model.Ano_Notificacao.ToString() + " Área notificada: " + model.Area_Notificada.ToString("#0.00") + " m²";
                //Incluir a observação da parcela
                Obsparcela ObsReg = new Obsparcela() {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 65,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Obs = sHist,
                    Userid = _fiscal,
                    Data = DateTime.Now
                };
                ex2 = tributarioRepository.Insert_Observacao_Parcela(ObsReg);

                //Gravar histórico no imóvel
                //Incluir a observação da parcela
                Historico ObsImovel = new Historico() {
                    Codreduzido = model.Codigo_Imovel,
                    Seq=0,
                    Datahist2=DateTime.Now,
                    Deschist=sHist,
                    Userid = _fiscal,
                };
                ex2 = imovelRepository.Incluir_Historico(ObsImovel);

                //Enviar para registrar 
                Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                    Nome = _nome,
                    Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco,
                    Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro,
                    Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade,
                    Cep = Functions.RetornaNumero(_cep.ToString()) ?? "14870000",
                    Cpf = Functions.RetornaNumero(_cpf_cnpj),
                    Numero_documento = _novo_documento,
                    Data_vencimento = _dataVencto,
                    Valor_documento = Convert.ToDecimal(model.Valor_Total),
                    Uf = "SP"
                };
                ex2 = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
                ex2 = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);

                //**************************************************************************
               

                //Grava a notificacao
                Notificacao_iss_web _not = new Notificacao_iss_web() {
                    Ano_notificacao = model.Ano_Notificacao,
                    Area = model.Area_Notificada,
                    Bairro = _bairro,
                    Categoria = model.Categoria_Construcao,
                    Cep = _cep,
                    Cidade = _cidade,
                    Codigo_cidadao = model.Codigo_Cidadao,
                    Codigo_imovel = model.Codigo_Imovel,
                    Complemento = _compl??"",
                    Cpf_cnpj = _cpf_cnpj,
                    Data_gravacao= DateTime.Now,
                    Data_vencimento= model.Data_vencimento,
                    Fiscal = _fiscal,
                    Habitese = model.Habitese,
                    Isspago = model.Iss_Pago,
                    Logradouro= _endereco,
                    Nome= _nome,
                    Numero= _numero,
                    Numero_notificacao = model.Numero_Notificacao,
                    Processo = model.Numero_Processo,
                    Uf= "SP",
                    Uso= model.Uso_Construcao,
                    Valorm2= model.Valor_m2,
                    Valortotal= model.Valor_Total,
                    Versao=1,
                    Situacao=2
                };
               
                _not.Guid = Guid.NewGuid().ToString("N");
                model.Guid = _not.Guid;
                _not.Numero_guia = _novo_documento;

                ex2 = tributarioRepository.Insert_notificacao_iss_web(_not);

                return RedirectToAction("Notificacao_menu");
            }

            if (!string.IsNullOrEmpty(model.Numero_Processo)) {
                Processo_bll processoRepository = new Processo_bll("GTIconnection");
                Exception ex = processoRepository.ValidaProcesso(model.Numero_Processo);
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message)) {
                    ViewBag.Result = ex.Message;
                    return View(model);
                }
            }

            return View(model);
        }

        [Route("Notificacao_menu")]
        [HttpGet]
        public ActionResult Notificacao_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Notificacao_query")]
        [HttpGet]
        public ViewResult Notificacao_query() {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _ano = DateTime.Now.Year;
            StartQueryNotificacao:
            List<Notificacao_iss_web_Struct> Lista = tributarioRepository.Retorna_Notificacao_Iss_Web(_ano);
            List<NotificacaoIssViewModel> model = new List<NotificacaoIssViewModel>();
            List<AnoList> ListaAno = tributarioRepository.Retorna_Ano_Notificacao();
            foreach (Notificacao_iss_web_Struct item in Lista) {
                NotificacaoIssViewModel reg = new NotificacaoIssViewModel() {
                    Guid=item.Guid,
                    Ano_Notificacao = item.Ano_notificacao,
                    Numero_Notificacao = item.Numero_notificacao,
                    Nome = Functions.TruncateTo(  item.Nome,25),
                    Data_Emissao = item.Data_gravacao,
                    SituacaoNome=item.Situacao_nome,
                    AnoNumero=item.Numero_notificacao.ToString("0000") + "/" + item.Ano_notificacao.ToString()
                };
                model.Add(reg);
            }
            if (model.Count == 0) {
                _ano = DateTime.Now.Year-1;
                goto StartQueryNotificacao;
            }
                
            ViewBag.ListaAno = new SelectList(ListaAno, "Codigo", "Descricao", ListaAno[ListaAno.Count - 1].Codigo);
            return View(model);
        }

        [Route("Notificacao_query")]
        [HttpPost]
        public ViewResult Notificacao_query(List<NotificacaoIssViewModel> model) {
            int _ano = model[0].Ano_Selected ;
            if (model[0].Ano_Selected == 0)
                _ano = DateTime.Now.Year;

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            List<Notificacao_iss_web_Struct> Lista = tributarioRepository.Retorna_Notificacao_Iss_Web(Convert.ToInt32(_ano));
            model = new List<NotificacaoIssViewModel>();

            foreach (Notificacao_iss_web_Struct item in Lista) {
                NotificacaoIssViewModel reg = new NotificacaoIssViewModel() {
                    Guid = item.Guid,
                    Ano_Notificacao = item.Ano_notificacao,
                    Numero_Notificacao = item.Numero_notificacao,
                    Nome = Functions.TruncateTo(item.Nome, 25),
                    Data_Emissao = item.Data_gravacao,
                    SituacaoNome = item.Situacao_nome,
                    AnoNumero = item.Numero_notificacao.ToString("0000") + "/" + item.Ano_notificacao.ToString(),
                    Ano_Selected = DateTime.Now.Year
                };
                model.Add(reg);
            }
            List<AnoList> ListaAno = tributarioRepository.Retorna_Ano_Notificacao();
            ViewBag.ListaAno = new SelectList(ListaAno, "Codigo", "Descricao", _ano);
            model[0].Ano_Selected = _ano;
            return View(model);
        }

        public ActionResult Notificacao_print(string p) {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Notificacao_iss_web_Struct _not = tributarioRepository.Retorna_Notificacao_Iss_Web(p);
            Notificacao_Iss_Tabela _tabela = tributarioRepository.Retorna_Notificacao_Iss_Tabela(_not.Ano_notificacao);

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");

            if (_not.Fiscal == 0)
                _not.Fiscal = 421;
            Assinatura _ass = sistemaRepository.Retorna_Usuario_Assinatura(_not.Fiscal);
            if (_ass == null)
                _ass = new Assinatura();
            string _nosso_numero = "287353200" + _not.Numero_guia.ToString();
            string _convenio = "2873532";
            //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
            DateTime _data_base = Convert.ToDateTime("07/10/1997");
            TimeSpan ts = Convert.ToDateTime(_not.Data_vencimento) - _data_base;
            int _fator_vencto = ts.Days;
            string _quinto_grupo = String.Format("{0:D4}", _fator_vencto);
            string _valor_boleto_str = string.Format("{0:0.00}", _not.Valortotal);
            _quinto_grupo += string.Format("{0:D10}", Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
            string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}", Convert.ToInt32(_convenio));
            _barra += String.Format("{0:D10}", Convert.ToInt64(_not.Numero_guia)) + "17";
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

            NotificacaoIssReport _notR = new NotificacaoIssReport() {
                Ano_notificacao = _not.Ano_notificacao,
                Area = _not.Area,
                Assinatura = _ass.Fotoass2==null?new byte[0]:_ass.Fotoass2,
                Bairro = _not.Bairro,
                Decreto = _tabela.Decreto,
                C179 = _tabela.C179,
                C180 = _tabela.C180,
                C181 = _tabela.C181,
                C182 = _tabela.C182,
                C183 = _tabela.C183,
                C184 = _tabela.C184,
                C185 = _tabela.C185,
                C670 = _tabela.C670,
                C671 = _tabela.C671,
                C672 = _tabela.C672,
                C673 = _tabela.C673,
                C674 = _tabela.C674,
                C675 = _tabela.C675,
                C676 = _tabela.C676,
                C689 = _tabela.C689,
                C690 = _tabela.C690,
                C691 = _tabela.C691,
                Cargo = _ass.Cargo,
                Categoria = _not.Categoria,
                Categoria_nome=_not.Categoria_Nome??"",
                Cep = _not.Cep,
                Cidade = _not.Cidade,
                Codigo_barra = _codigo_barra,
                Codigo_cidadao = _not.Codigo_cidadao,
                Codigo_imovel = _not.Codigo_imovel,
                Complemento = _not.Complemento,
                Cpf_cnpj = _not.Cpf_cnpj,
                Data_gravacao = _not.Data_gravacao,
                Data_vencimento = _not.Data_vencimento,
                Fiscal = _not.Fiscal,
                FiscalNome = _ass.Nome,
                Guid = _not.Guid,
                Habitese = _not.Habitese,
                Isspago = _not.Isspago,
                Linha_digitavel = _linha_digitavel,
                Logradouro = _not.Logradouro + ", " + _not.Numero,
                Nome = _not.Nome,
                Numero = _not.Numero,
                Numero_guia = _not.Numero_guia,
                Numero_notificacao = _not.Numero_notificacao,
                Nosso_numero = _nosso_numero,
                Processo = _not.Processo,
                Uf = _not.Uf,
                Uso = _not.Uso,
                Uso_nome=_not.Uso_Nome,
                Valorm2 = _not.Valorm2,
                Valortotal = _not.Valortotal
               
            };
            if (_not.Habitese)
                _notR.Msg = "O Setor de Fiscalização de Tributos da Prefeitura Municipal de Jaboticabal, tendo em vista o processo de pedido de HABITE-SE em referência, vem NOTIFICAR o contribuinte acima identificado do lançamento do Imposto Sobre Serviços da Construção Civil, relativo ao imóvel abaixo descrito, calculado conforme os parâmetros abaixo indicados, para no prazo de 30 dias, a contar do recebimento desta Notificação, efetuar o pagamento/parcelamento ou apresentar reclamação contra a mesma.";
            else
                _notR.Msg = "O Setor de Fiscalização de Tributos da Prefeitura Municipal de Jaboticabal, vem através desta NOTIFICAR o contribuinte em referência do lançamento do Imposto Sobre Serviços de Qualquer Natureza (ISSQN) incidente sobre a mão de obra para construção de imóvel, calculado conforme os parâmetros abaixo indicados, para no prazo de 30 dias a contar do recebimento desta efetuar o pagamento/parcelamento ou apresentar recurso contra o mesmo.";

            _not.Logradouro += ", " + _not.Numero.ToString();
            //Gera Boleto

            List<NotificacaoIssReport> Lista = new List<NotificacaoIssReport> {
                _notR
            };
            DataSet Ds = Functions.ToDataSet(Lista);
            ReportDataSource rdsAct = new ReportDataSource("dsNotificacaoISS", Ds.Tables[0]);
            ReportViewer viewer = new ReportViewer();
            viewer.LocalReport.Refresh();
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Boleto_NotificacaoISS_m001.rdlc");
            viewer.LocalReport.DataSources.Add(rdsAct);


            List<ReportParameter> parameters = new List<ReportParameter> {
                new ReportParameter("NumeroNot", Lista[0].Numero_notificacao.ToString("0000") + "/" + Lista[0].Ano_notificacao.ToString()),
                new ReportParameter("Processo", Lista[0].Processo)
            };
            viewer.LocalReport.SetParameters(parameters);
            byte[] bytes = viewer.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename= NotificacaoISS" + "." + extension);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            return null;
        }

        [Route("Rod_menu")]
        [HttpGet]
        public ActionResult Rod_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _userid = Convert.ToInt32(Session["hashid"]);
            bool _func = Session["hashfunc"].ToString() == "S";

            List<Rodo_empresa> Lista = tributarioRepository.Lista_Rodo_empresa(_userid,_func);
            ViewBag.Lista_Empresa = new SelectList(Lista, "Codigo", "Nome");
            
            RodoviariaViewModel model = new RodoviariaViewModel();
            return View(model);
        }

        [Route("Rod_menu")]
        [HttpPost]
        public ActionResult Rod_menu(RodoviariaViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            
            List<Rodo_empresa> Lista = tributarioRepository.Lista_Rodo_empresa();
            ViewBag.Lista_Empresa = new SelectList(Lista, "Codigo", "Nome");
            
            return RedirectToAction("Rod_plat_query",new { a= Encrypt( model.Codigo.ToString()),c=Encrypt(DateTime.Now.Year.ToString())});
        }

        [Route("Rod_plat_query")]
        [HttpGet]
        public ActionResult Rod_plat_query(string a,string c) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _codigo, _ano;
            try {
                _codigo = Convert.ToInt32(Decrypt(a));
                _ano = Convert.ToInt32(Decrypt(c));
            } catch (Exception) {
                return RedirectToAction("Login", "Home");
            }
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            List<Rodo_uso_plataforma_Struct> Lista = tributarioRepository.Lista_Rodo_uso_plataforma(_codigo,_ano);
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
            string _nome = cidadaoRepository.Retorna_Nome_Cidadao(_codigo);
            RodoviariaViewModel model = new RodoviariaViewModel {
                Codigo = _codigo,
                Nome=_nome,
                Lista_uso_plataforma=Lista
            };

            return View(model);
        }

        [Route("Rod_plat_query")]
        [HttpPost]
        public ActionResult Rod_plat_query(string DataDe,string DataAte,string Codigo,string Qtde1,string Qtde2,string Qtde3) {
            //            FormCollection collection=new FormCollection();
            var data1 = DataDe;
            bool t = DateTime.TryParse(data1, out DateTime _data1);
            t = DateTime.TryParse(DataAte, out DateTime _data2);
            var cod = Codigo;
            int _codigo=Convert.ToInt32(cod);
            var qtde1 = Qtde1;
            int _qtde1 = Convert.ToInt32(qtde1);
            var qtde2 = Qtde2;
            int _qtde2 = Convert.ToInt32(qtde2);
            var qtde3 = Qtde3;
            int _qtde3 = Convert.ToInt32(qtde3);
            short _ano = (short)_data1.Year;
            int _userId =  Convert.ToInt32(Session["hashid"]);
            decimal _valorGuia =0;

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            decimal _valor1 = tributarioRepository.Retorna_Valor_Tributo(_ano, 154);
            decimal _valor2 = tributarioRepository.Retorna_Valor_Tributo(_ano, 155);
            decimal _valor3 = tributarioRepository.Retorna_Valor_Tributo(_ano, 156);

            short _seq = tributarioRepository.Retorna_Ultima_Seq_Uso_Plataforma(_codigo, _ano);
            _seq++;
            DateTime _dataVencto = _data2.AddDays(10);

            Exception ex2 = null;
            //grava parcela
            Debitoparcela regParcela = new Debitoparcela {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Statuslanc = 3,
                Datavencimento = _dataVencto,
                Datadebase = DateTime.Now,
                Userid = _userId
            };
            try {
                ex2 = tributarioRepository.Insert_Debito_Parcela(regParcela);
            } catch (Exception ) {

                throw;
            }

            //grava tributo
            if (_qtde1 > 0) {
                decimal _valorTotal1 = _valor1 * _qtde1;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)154,
                    Valortributo = Math.Round(_valorTotal1, 2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }
            if (_qtde2 > 0) {
                decimal _valorTotal2 = _valor2 * _qtde2;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)155,
                    Valortributo = Math.Round(_valorTotal2 , 2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }
            if (_qtde3 > 0) {
                decimal _valorTotal3 = _valor3 * _qtde3;
                Debitotributo regTributo = new Debitotributo {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = 52,
                    Seqlancamento = _seq,
                    Numparcela = 1,
                    Codcomplemento = 0,
                    Codtributo = (short)156,
                    Valortributo = Math.Round(_valorTotal3, 2)
                };
                ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);
            }

            //retorna o valor atualizado do débito (lançamento retroativo)
            List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, _ano, _ano, 52, 52, _seq, _seq, 1, 1, 0, 0, 3, 3, DateTime.Now, "Web");
            List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);
            
            foreach (SpExtrato item in ListaParcela) {
                _valorGuia += item.Valortotal;
            }

            if (_dataVencto < DateTime.Now) {
                _dataVencto = DateTime.Now.AddDays(10);
            }

            //grava o documento
            Numdocumento regDoc = new Numdocumento {
                Valorguia = _valorGuia,
                Emissor = "Gti.Web/UsoPlataforma",
                Datadocumento = _dataVencto,
                Registrado = true,
                Percisencao = 0
            };
            regDoc.Percisencao = 0;
            int _novo_documento = tributarioRepository.Insert_Documento(regDoc);

            //grava o documento na parcela
            Parceladocumento regParc = new Parceladocumento {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Numdocumento = _novo_documento,
                Valorjuros = 0,
                Valormulta = 0,
                Valorcorrecao = 0,
                Plano = 0
            };
            tributarioRepository.Insert_Parcela_Documento(regParc);

            string sHist = "REFERENTE A " + (_qtde1+_qtde2+_qtde3).ToString()  +  " TAXAS DE EMBARQUE DO TERMINAL RODOVIÁRIO DO PERÍODO DE " + _data1.ToString("dd/MM/yyyy")  +  " À " +  _data2.ToString("dd/MM/yyyy") + ".";
            //Incluir a observação da parcela
            Obsparcela ObsReg = new Obsparcela() {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = 52,
                Seqlancamento = _seq,
                Numparcela = 1,
                Codcomplemento = 0,
                Obs = sHist,
                Userid = _userId,
                Data = DateTime.Now
            };
            ex2 = tributarioRepository.Insert_Observacao_Parcela(ObsReg);

            //Incluir rodo_uso_plataforma
            short _seq2 = tributarioRepository.Retorna_Ultima_Seq_Uso_Plataforma(_codigo, _data1,_data2);
           // _seq2++;

            Rodo_uso_plataforma regR = new Rodo_uso_plataforma {
                Codigo = _codigo,
                Datade = _data1,
                Dataate = _data2,
                Seq=(byte)_seq2,
                SeqDebito = (byte)_seq,
                Qtde1 =_qtde1,
                Qtde2=_qtde2,
                Qtde3=_qtde3,
                Numero_Guia=_novo_documento,
                Valor_Guia=_valorGuia,
                Situacao=7 //não pago
            };
            ex2 = tributarioRepository.Insert_Rodo_Uso_Plataforma(regR);

            //Enviar para registrar 
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            string _bairro = "", _endereco = "", _compl = "", _cidade = "JABOTICABAL";
            string _cpf_cnpj = string.IsNullOrWhiteSpace(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
            int _cep = 14870000;
            bool _r = _cidadao.EtiquetaC != "S";
            string _nome = _cidadao.Nome;
            _endereco = _r ? _cidadao.EnderecoR : _cidadao.EnderecoC;
            _bairro = _r ? _cidadao.NomeBairroR : _cidadao.NomeBairroC;
            int _numero = _r ? (int)_cidadao.NumeroR : (int)_cidadao.NumeroC;
            _compl = _r ? _cidadao.ComplementoR : _cidadao.ComplementoC;
            _cidade = _r ? _cidadao.NomeCidadeR : _cidadao.NomeCidadeC;
            string _uf = _r ? _cidadao.UfR : _cidadao.UfC;
            _cep = _r ? (int)_cidadao.CepR : (int)_cidadao.CepC;


            Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                Nome = _nome,
                Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco,
                Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro,
                Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade,
                Cep = Functions.RetornaNumero(_cep.ToString()) ?? "14870000",
                Cpf = Functions.RetornaNumero(_cpf_cnpj),
                Numero_documento = _novo_documento,
                Data_vencimento = _dataVencto,
                Valor_documento = Convert.ToDecimal(_valorGuia),
                Uf = _uf
            };
            ex2 = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
            ex2 = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);

            return Json(new { success = true, responseText = "Dados enviados com sucesso!" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rod_uso_plataforma_print(string p1,string p2,string p3,string p4) {

            bool b = DateTime.TryParseExact(p1, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _datade);
            bool c = DateTime.TryParseExact(p2, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _dataate);
            int _ano = _datade.Year;
            short _seq = Convert.ToInt16(p3);
            int _codigo = Convert.ToInt32(p4);
            p1 = ""; p2 = ""; p3 = ""; p4 = "";

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Rodo_uso_plataforma_Struct reg = tributarioRepository.Retorna_Rodo_uso_plataforma(_codigo, _datade, _dataate, _seq);

            short _seqdebito = reg.SeqDebito;
            decimal _aliq1 = tributarioRepository.Retorna_Valor_Tributo(_ano, 154);
            decimal _aliq2 = tributarioRepository.Retorna_Valor_Tributo(_ano, 155);
            decimal _aliq3 = tributarioRepository.Retorna_Valor_Tributo(_ano, 156);

            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            string _bairro = "", _endereco = "", _compl = "", _cidade = "JABOTICABAL", _nome = "";
            string _cpf_cnpj = string.IsNullOrWhiteSpace(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _r = _cidadao.EtiquetaC != "S";
            _nome = _cidadao.Nome;
            _endereco = _r ? _cidadao.EnderecoR : _cidadao.EnderecoC;
            _bairro = _r ? _cidadao.NomeBairroR : _cidadao.NomeBairroC;
            int _numero = _r ? (int)_cidadao.NumeroR : (int)_cidadao.NumeroC;
            _compl = _r ? _cidadao.ComplementoR : _cidadao.ComplementoC;
            _cidade = _r ? _cidadao.NomeCidadeR : _cidadao.NomeCidadeC;
            string _uf = _r ? _cidadao.UfR : _cidadao.UfC;
            int _cep = _r ? (int)_cidadao.CepR : (int)_cidadao.CepC;

            List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, (short)_ano, (short)_ano, 52, 52, _seqdebito, _seqdebito, 1, 1, 0, 0, 0, 99, DateTime.Now, "Web");
            decimal _vp1 = 0, _vm1 = 0, _vj1 = 0, _vt1 = 0;
            decimal _vp2 = 0, _vm2 = 0, _vj2 = 0, _vt2 = 0;
            decimal _vp3 = 0, _vm3 = 0, _vj3 = 0, _vt3 = 0;

            foreach (SpExtrato item in ListaTributo) {

                switch (item.Codtributo) {
                    case 154:
                        _vp1 = item.Valortributo;
                        _vm1 = item.Valormulta;
                        _vj1 = item.Valorjuros;
                        _vt1 = item.Valortotal;
                        break;
                    case 155:
                        _vp2 = item.Valortributo;
                        _vm2 = item.Valormulta;
                        _vj2 = item.Valorjuros;
                        _vt2 = item.Valortotal;
                        break;
                    case 156:
                        _vp3 = item.Valortributo;
                        _vm3 = item.Valormulta;
                        _vj3 = item.Valorjuros;
                        _vt3 = item.Valortotal;
                        break;
                    default:
                        break;
                }
            }

            Numdocumento doc = tributarioRepository.Retorna_Dados_Documento(reg.Numero_Guia);
            DateTime _dataVencto =(DateTime) doc.Datadocumento;

            string _nosso_numero = "287353200" + reg.Numero_Guia.ToString();
            string _convenio = "2873532";
            //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
            DateTime _data_base = Convert.ToDateTime("07/10/1997");
            TimeSpan ts = Convert.ToDateTime(_dataVencto) - _data_base;
            int _fator_vencto = ts.Days;
            string _quinto_grupo = String.Format("{0:D4}", _fator_vencto);
            string _valor_boleto_str = string.Format("{0:0.00}", reg.Valor_Guia);
            _quinto_grupo += string.Format("{0:D10}", Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
            string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}", Convert.ToInt32(_convenio));
            _barra += String.Format("{0:D10}", Convert.ToInt64(reg.Numero_Guia)) + "17";
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

            UsoPlataformaReport _usoR = new UsoPlataformaReport() {
                Aliquota1=_aliq1,
                Aliquota2=_aliq2,
                Aliquota3=_aliq3,
                Bairro=_bairro,
                Cidade=_cidade,
                Codigo=_codigo,
                Codigo_Barra=_codigo_barra,
                CpfCnpj=_cpf_cnpj,
                Data_Final=_dataate,
                Data_Inicio=_datade,
                Data_Documento=DateTime.Now,
                Data_Vencimento=_dataVencto,
                Endereco=_endereco + ", " + _numero.ToString() + " " + _compl,
                Linha_Digitavel=_linha_digitavel, Nome = _nome,
                Nosso_Numero =_nosso_numero,
                Numero_Guia=reg.Numero_Guia,
                Qtde1=reg.Qtde1,
                Qtde2=reg.Qtde2,
                Qtde3=reg.Qtde3,
                UF=_uf,
                Valor1J=_vj1,
                Valor1M=_vm1,
                Valor1P=_vp1,
                Valor1T=_vt1,
                Valor2J = _vj2,
                Valor2M = _vm2,
                Valor2P = _vp2,
                Valor2T = _vt2,
                Valor3J = _vj3,
                Valor3M = _vm3,
                Valor3P = _vp3,
                Valor3T = _vt3,
                Valor_Guia=reg.Valor_Guia
            };

            //Gera Boleto

            List<UsoPlataformaReport> Lista = new List<UsoPlataformaReport> {
                _usoR
            };
            DataSet Ds = Functions.ToDataSet(Lista);
            ReportDataSource rdsAct = new ReportDataSource("dsUsoPlataforma", Ds.Tables[0]);
            ReportViewer viewer = new ReportViewer();
            viewer.LocalReport.Refresh();
            viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Boleto_Uso_Plataforma.rdlc");
            viewer.LocalReport.DataSources.Add(rdsAct);
            byte[] bytes = viewer.LocalReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = string.Empty;
            Response.AddHeader("content-disposition", "attachment; filename= UsoPlataforma" + "." + extension);
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            return null;
        }

        public ActionResult Rod_uso_plataforma_cancel(string p1, string p2, string p3, string p4,string p5) {
            bool b = DateTime.TryParseExact(p1, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _datade);
            bool c = DateTime.TryParseExact(p2, "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _dataate);
            int _ano = _datade.Year;
            short _seq = Convert.ToInt16(p3);
            int _codigo = Convert.ToInt32(p4);
            int _doc = Convert.ToInt32(p5);

            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");

            List<DebitoStructure> Lista = tributarioRepository.Lista_Tabela_Parcela_Documento(_doc);
            int _seqdebito = Lista[0].Sequencia_Lancamento;

            Exception ex = tributarioRepository.Alterar_Status_Lancamento(_codigo, (short)_ano, 52, (short)_seqdebito, 1, 0, 5);
            ex = tributarioRepository.Alterar_Uso_Plataforma_Situacao(_codigo,_datade,_dataate,_seq,4 );

            return RedirectToAction("Rod_plat_query", "Tributario",new { a =Encrypt( p4), c = Encrypt(_ano.ToString()) });

        }
    }
}
