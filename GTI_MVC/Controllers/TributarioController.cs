using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Tributario.EditorTemplates;
using System;
using System.Collections.Generic;
using System.Data;
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
                        _tipoCadastro = TipoCadastro.Cidadao;
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
                reg.Inscricao = _dados.Inscricao;
                reg.Endereco = _dados.NomeLogradouro;
                reg.Endereco_Numero = (int)_dados.Numero;
                reg.Endereco_Complemento = _dados.Complemento;
                reg.Bairro = _dados.NomeBairro ?? "";
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
            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View("Certidao_Debito_Codigo");
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Reports/" + _reportName));

            try {
                rd.SetDataSource(certidao);
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
            int _numero;
            string sNome = "";
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito_Doc);
            ViewBag.Result = "";

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.CpfValue != null || model.CnpjValue != null) {
                List<int> _lista = new List<int>();
                
                if (model.CpfValue != null) {
                    sNome = sistemaRepository.Nome_por_Cpf(Functions.RetornaNumero(model.CpfValue));
                    _lista = empresaRepository.Retorna_Codigo_por_CPF(Functions.RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        sNome = sistemaRepository.Nome_por_Cnpj(Functions.RetornaNumero(model.CnpjValue));
                        _lista = empresaRepository.Retorna_Codigo_por_CNPJ(Functions.RetornaNumero(model.CnpjValue));
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


            string sData = "18/04/2012", sTributo = "", sNao = "", sCertifica = "";
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

                Certidao_debito_documento reg = new Certidao_debito_documento();
                reg._Codigo = _codigo;
                reg._Ret = nRet;
                reg._Tributo = sTributo;
                reg._Nome = sNome == null ? "" : sNome.Trim();
                
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
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(TipoCertidao.Debito_Doc);
            int _ano_certidao = DateTime.Now.Year;
            Certidao cert = new Certidao();
            cert.Ano = (short)_ano_certidao;

            cert.Numero = _numero_certidao;
            cert.Nome_Requerente = _lista_certidao[0]._Nome;
            cert.Cpf_Cnpj = !string.IsNullOrWhiteSpace( model.CpfValue)?model.CpfValue:model.CnpjValue;
            cert.Numero_Ano = _numero_certidao.ToString("00000") + "/" + _ano_certidao.ToString();
            if (_tipo_Certidao == RetornoCertidaoDebito.Negativa) {
                cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IN";
                cert.Tributo = "Não consta débito apurado contra o(a) mesmo(a).";
                cert.Nao = "NÃO";
            } else {
                if (_tipo_Certidao == RetornoCertidaoDebito.Positiva) {
                    cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IP";
                    cert.Tributo = "Consta débito apurado contra o(a) mesmo(a) com referência a: " + _tributo;
                } else {
                    if (_tipo_Certidao == RetornoCertidaoDebito.NegativaPositiva) {
                        cert.Controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _lista_certidao[0]._Codigo.ToString() + "-IS";
                        cert.Tributo = "Consta débito apurado contra o(a) mesmo(a) com referência a: " + _tributo + " que se encontram em sua exigibilidade suspensa, em razão de parcelamento dos débitos";
                    }
                }
            }
            List<Certidao> ListaCertidao = new List<Certidao>();
            ListaCertidao.Add(cert);

            certidao_debito_doc RegSave = new certidao_debito_doc() {
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

            if (ex != null) {
                throw ex;
            } else {
                if (_tipo_Certidao == RetornoCertidaoDebito.Negativa) {
                    ReportDocument rd = new ReportDocument();
                    rd.Load(Server.MapPath("~/Reports/CertidaoDebitoDocumentoN.rpt"));

                    try {
                        rd.SetDataSource(ListaCertidao);
                        Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "Certidao_Debito.pdf");
                    } catch {
                        throw;
                    }
                } else {
                    if (_tipo_Certidao == RetornoCertidaoDebito.Positiva) {
                        ReportDocument rd = new ReportDocument();
                        rd.Load(Server.MapPath("~/Reports/CertidaoDebitoDocumentoP.rpt"));

                        try {
                            rd.SetDataSource(ListaCertidao);
                            Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                            return File(stream, "application/pdf", "Certidao_Debito.pdf");
                        } catch {
                            throw;
                        }
                    } else {
                        if (_tipo_Certidao == RetornoCertidaoDebito.NegativaPositiva) {
                            ReportDocument rd = new ReportDocument();
                            rd.Load(Server.MapPath("~/Reports/CertidaoDebitoDocumentoPN.rpt"));

                            try {
                                rd.SetDataSource(ListaCertidao);
                                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                                return File(stream, "application/pdf", "Certidao_Debito.pdf");
                            } catch {
                                throw;
                            }
                        }
                    }
                }
            }

            return View(model);
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
            rd.Load(Server.MapPath("~/Reports/Comprovante_Pagamento.rpt" ));
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
            bool _isdate = Functions.IsDate(model.DataVencimento);
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
#pragma warning disable IDE0060 // Remove unused parameter
        public ActionResult Damb(CertidaoViewModel model,int Codigo=0) {
#pragma warning restore IDE0060 // Remove unused parameter
            if (model.CpfValue != null) {
                if (!Functions.ValidaCpf(model.CpfValue)) {
                    ViewBag.Result = "CPF inválido.";
                    return View(model);
                }
            }
            if (model.CnpjValue != null) {
                if (!Functions.ValidaCNPJ(model.CnpjValue)) {
                    ViewBag.Result = "CNPJ inválido.";
                    return View(model);
                }
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
                        _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, Functions.RetornaNumero(model.CpfValue));
                        if (!_existeCod) {
                            ViewBag.Result = "CPF não pertence a esta inscrição.";
                            return View(model);
                        }
                    } else {
                        if (model.CnpjValue != null) {
                            _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, Functions.RetornaNumero( model.CnpjValue));
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
                            _existeCod = empresaRepository.ExisteEmpresaCpf(RetornaNumero(model.CpfValue))>0?true:false;
                            if (!_existeCod) {
                                ViewBag.Result = "CPF não pertence a esta inscrição.";
                                return View(model);
                            }
                        } else {
                            if (model.CnpjValue != null) {
                                _existeCod = empresaRepository.ExisteEmpresaCnpj(RetornaNumero(model.CnpjValue))>0?true:false;
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
                            _existeCod = requerenteRepository.Existe_Cidadao_Cpf(_codigo, Functions.RetornaNumero(model.CpfValue));
                            if (!_existeCod) {
                                ViewBag.Result = "CPF não pertence a esta inscrição.";
                                return View(model);
                            }
                        } else {
                            if (model.CnpjValue != null) {
                                _existeCod = requerenteRepository.Existe_Cidadao_Cnpj(_codigo, Functions.RetornaNumero(model.CnpjValue));
                                if (!_existeCod) {
                                    ViewBag.Result = "CNPJ não pertence a esta inscrição.";
                                    return View(model);
                                }
                            }
                        }
                    }
                    CidadaoStruct _cidadao = requerenteRepository.Dados_Cidadao(_codigo);
                    _nome = _cidadao.Nome;
                    _cpfcnpj = _cidadao.Cpf ?? _cidadao.Cnpj;
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

            bool IsRefis = true;
            int nIndex = 0, nPlano = 0;
            decimal nPerc=0, nSomaPrincipal = 0, nSomaJuros = 0, nSomaMulta = 0, nSomaCorrecao = 0, nSomaTotal = 0, nSomaHonorario = 0;

            if (IsRefis) {
                foreach (var item in ListaParcela) {
                    if (Convert.ToDateTime(item.Datavencimento) <= Convert.ToDateTime("30/06/2019")) {
                        short CodLanc = item.Codlancamento;
                        if (CodLanc != 48 || CodLanc != 69 || CodLanc != 78) {

                            if (_dataVencto <= Convert.ToDateTime("18/10/2019")) {
                                nPerc = 1M;
                                nPlano = 33;
                            } else if (Convert.ToDateTime(_dataVencto) > Convert.ToDateTime("18/10/2019") && Convert.ToDateTime(_dataVencto) <= Convert.ToDateTime("29/11/2019")) {
                                nPerc = 0.9M;
                                nPlano = 34;
                            } else if (Convert.ToDateTime(_dataVencto) > Convert.ToDateTime("29/11/2019") && Convert.ToDateTime(_dataVencto) <= Convert.ToDateTime("20/12/2019")) {
                                nPerc = 0.8M;
                                nPlano = 35;
                            }
                            if (nPlano > 0) {
                                item.Valorjuros = Convert.ToDecimal(item.Valorjuros) - (Convert.ToDecimal(item.Valorjuros) * nPerc);
                                item.Valormulta = Convert.ToDecimal(item.Valormulta) - (Convert.ToDecimal(item.Valormulta) * nPerc);
                                item.Valortotal = item.Valortributo + item.Valorjuros + item.Valormulta + item.Valorcorrecao;
                            }
                            ListaParcela[nIndex].Valorjuros = item.Valorjuros;
                            ListaParcela[nIndex].Valormulta = item.Valormulta;
                            ListaParcela[nIndex].Valortotal = item.Valortotal;
                        }
                    }
                    nIndex++;
                }
            }

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
                    Soma_Multa=_debitos.Soma_Multa,
                    Soma_Correcao=_debitos.Soma_Correcao,
                    Soma_Total=_debitos.Soma_Total,
                    Soma_Honorario=_debitos.Soma_Honorario,
                    Pt=_debitos.Codigo_Situacao==38?"S":"N",
                    Ep=_debitos.Codigo_Situacao==39?"S":"N"
                };
                _linha++;
                model.Debito.Add(editorViewModel);
            }
            model.Soma_Principal = _somaP;
            model.Soma_Juros = _somaJ;
            model.Soma_Multa = _somaM;
            model.Soma_Correcao = _somaC;
            model.Soma_Total = _somaT;
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
                    _endereco = _cidadao.EnderecoR + ", " + _cidadao.NumeroR.ToString() + _cidadao.ComplementoR == null ? "" : " " + _cidadao.ComplementoR + " " + _cidadao.NomeBairroR;
                    _cidade = _cidadao.NomeCidadeR;
                    _uf = _cidadao.UfR;
                    _cep = (enderecoRepository.RetornaCep((int)_cidadao.CodigoLogradouroR, (short)_cidadao.NumeroR)).ToString();
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
                Cep=_cep
            };
            decimal _somaP = 0,_somaJ=0,_somaM=0,_somaC=0,_somaT=0,_somaH=0;
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
                    Soma_Correcao = _debitos.Soma_Correcao,
                    Soma_Total =_debitos.Soma_Total,
                    Soma_Honorario = _debitos.Soma_Honorario,
                    Lancamento_Nome =_debitos.Lancamento_Nome,
                    Data_Vencimento=_debitos.Data_Vencimento,
                    Aj=_debitos.Aj,
                    Da=_debitos.Da
                };
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
            modelt.Plano = model.Plano;
            modelt.Soma_Principal = _somaP;
            modelt.Soma_Juros = _somaJ;
            modelt.Soma_Multa = _somaM;
            modelt.Soma_Correcao = _somaC;
            modelt.Soma_Honorario = _somaH;
            modelt.Soma_Total = _somaP+_somaJ+_somaM+_somaC+_somaH;
            modelt.Valor_Boleto = Convert.ToInt32((_somaP + _somaJ + _somaM + _somaC + _somaH) * 100).ToString();
            TempData["debito"] = modelt;
            return RedirectToAction("Damd");
        }

        public ActionResult Damd() {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            DebitoListViewModel value =(DebitoListViewModel) TempData["debito"];
            DebitoListViewModel model = value as DebitoListViewModel;

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
                    Plano=Convert.ToInt16(value.Plano.ToString()),
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
                        Plano = Convert.ToInt16(value.Plano.ToString()),
                        Numdocumento = _documento
                    };
                    ex2 = tributarioRepository.Insert_Parcela_Documento(parcReg);
                }
            }

            model.Data_Vencimento_String = Convert.ToDateTime(value.Data_Vencimento.ToString()).ToString("ddMMyyyy");
            model.RefTran = "287353200" + _documento.ToString();
            if (model==null)
                return RedirectToAction("Login_gti","Home");
            else
                return View( model);
        }

        //[Route("PrintDam")]
        //[HttpPost]
        //public ActionResult PrintDam(DebitoSelectionViewModel model) {
        //    var selectedIds = model.getSelectedIds();
        //    var modelt = new DebitoListViewModel() {
        //        Nome = model.Nome,
        //        Inscricao = model.Inscricao,
        //        CpfCnpjLabel = model.CpfCnpjLabel
        //    };
        //    return RedirectToAction("Login_gti","Home");
        //}
         
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
                    certidao_debito_doc _dados = tributarioRepository.Retorna_Certidao_Debito_Doc(model.Chave);
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
                    rd.Load(Server.MapPath("~/Reports/CertidaoDebitoDocValida.rpt"));

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



    }
}