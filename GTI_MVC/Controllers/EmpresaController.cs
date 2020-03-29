using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Models.ReportModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using GTI_Bll.Classes;
using GTI_Models.Models;
using static GTI_Models.modelCore;
using GTI_Mvc.ViewModels;

namespace GTI_Mvc.Controllers {

    [Route("Empresa")]
    public class EmpresaController : Controller {

        public EmpresaController( ) {
        }

        [Route("Details")]
        [HttpGet]
        public ViewResult Details() {
            if (HttpContext.Session["gti_V3id"]==null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            return View();
        }
        
       
        [Route("Details")]
        [HttpPost]
        public ViewResult Details(EmpresaDetailsViewModel model) {
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection"); 
            int _codigo = 0;
            bool _existeCod = false;
            EmpresaDetailsViewModel empresaDetailsViewModel = new EmpresaDetailsViewModel();

            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo >= 100000 && _codigo < 210000) //Se estiver fora deste intervalo nem precisa checar se a empresa existe
                    _existeCod = empresaRepository.Existe_Empresa(_codigo);
            } else {
                if (model.CnpjValue != null) {
                    string _cnpj = model.CnpjValue;
                    bool _valida = Functions.ValidaCNPJ(_cnpj); //CNPJ válido?
                    if (_valida) {
                        _codigo = empresaRepository.ExisteEmpresaCnpj(_cnpj);
                        if (_codigo > 0)
                            _existeCod = true;
                    } else {
                        empresaDetailsViewModel.ErrorMessage = "Cnpj inválido.";
                        return View(empresaDetailsViewModel);
                    }
                } else {
                    if (model.CpfValue != null) {
                        string _cpf = model.CpfValue;
                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                        if (_valida) {
                            _codigo = empresaRepository.ExisteEmpresaCpf(_cpf);
                            if (_codigo > 0)
                                _existeCod = true;

                        } else {
                            empresaDetailsViewModel.ErrorMessage = "Cpf inválido.";
                            return View(empresaDetailsViewModel);
                        }
                    }
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                empresaDetailsViewModel.ErrorMessage = "Código de verificação inválido.";
                return View(empresaDetailsViewModel);
            }

            if (_existeCod) {
                EmpresaStruct empresa = empresaRepository.Retorna_Empresa(_codigo);
                empresaDetailsViewModel.EmpresaStruct = empresa;
                empresaDetailsViewModel.TaxaLicenca = empresaRepository.Empresa_tem_TL(_codigo) ? "Sim" : "Não";
                empresaDetailsViewModel.Vigilancia_Sanitaria = empresaRepository.Empresa_tem_VS(_codigo) ? "Sim" : "Não";
                empresaDetailsViewModel.Mei = empresaRepository.Empresa_Mei(_codigo) ? "Sim" : "Não";
                List<CnaeStruct> ListaCnae = empresaRepository.Lista_Cnae_Empresa(_codigo);
                string sCnae = "";
                foreach (CnaeStruct cnae in ListaCnae) {
                    //sCnae += cnae.CNAE + "-" + cnae.Descricao + System.Environment.NewLine;
                    sCnae += cnae.CNAE + "-" + cnae.Descricao + "; ";
                }
                empresaDetailsViewModel.Cnae = sCnae;
                string sRegime = empresaRepository.RegimeEmpresa(_codigo);
                if (sRegime == "F")
                    sRegime = "ISS FIXO";
                else {
                    if (sRegime == "V")
                        sRegime = "ISS VARIÁVEL";
                    else {
                        if (sRegime == "E")
                            sRegime = "ISS ESTIMADO";
                        else
                            sRegime = "NENHUM";
                    }
                }
                empresaDetailsViewModel.Regime_Iss = sRegime;
                empresaDetailsViewModel.ErrorMessage = "";
                return View("DetailsTable", empresaDetailsViewModel);
            } else {
                empresaDetailsViewModel.ErrorMessage = "Empresa não cadastrada.";
                return View(empresaDetailsViewModel);
            }

        }

        [Route("get-captcha-image")]
        public ActionResult GetCaptchaImage() {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session["CaptchaCode"]= result.CaptchaCode;
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [Route("Certidao/Certidao_Inscricao")]
        [HttpGet]
        public ViewResult Certidao_Inscricao() {
            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }

            CertidaoViewModel model = new CertidaoViewModel {
                OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = true },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = false }
            },
                SelectedValue = "cpfCheck"
            };
            return View(model);
        }

        [Route("Retorna_Codigos")]
        [Route("Certidao/Retorna_Codigos")]
        public ActionResult Retorna_Codigos(CertidaoViewModel model) {
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }

            if (model.CpfValue!=null || model.CnpjValue != null) {

                model.OptionList = new List<SelectListaItem> {
                    new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                    new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
                };

                List<int> _lista = new List<int>();
                if (model.CpfValue != null) {
                    _lista = empresaRepository.Retorna_Codigo_por_CPF(Functions.RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        _lista = empresaRepository.Retorna_Codigo_por_CNPJ(Functions.RetornaNumero(model.CnpjValue));
                    }
                }
                if (_lista.Count > 0) {
                    ViewBag.Lista_Codigo = _lista;
                    return View("Certidao_Inscricao", model);
                } else {
                    ViewBag.Result = "Não foi localizada nenhuma empresa cadastrada com o CPF/CNPJ informado.";
                    return View("Certidao_Inscricao", model);
                }
            }
            ViewBag.Result = "Informe o CPF ou o CNPJ para busca.";
            return View("Certidao_Inscricao",model);
        }

        [HttpPost]
        [Route("Validate_CI")]
        [Route("Certidao/Validate_CI")]
        public ActionResult Validate_CI(CertidaoViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _codigo , _ano ,_numero;
            string _chave = model.Chave;

            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.Chave != null) {
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Inscricao", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;
                    List<Comprovante_Inscricao> certidao = new List<Comprovante_Inscricao>();
                    Certidao_inscricao _dados = tributarioRepository.Retorna_Certidao_Inscricao(_ano, _numero,_codigo);
                    if (_dados != null) {
                        Comprovante_Inscricao reg = new Comprovante_Inscricao() {
                            Codigo = _codigo,
                            Razao_Social = _dados.Nome,
                            Nome_Fantasia = _dados.Nome_fantasia,
                            Cep = _dados.Cep,
                            Cidade = _dados.Cidade,
                            Email = _dados.Email,
                            Inscricao_Estadual = _dados.Inscricao_estadual,
                            Endereco = _dados.Endereco + ", " + _dados.Numero,
                            Complemento = _dados.Complemento,
                            Bairro = _dados.Bairro ?? "",
                            Ano = _ano,
                            Numero = _numero,
                            Controle = _chave,
                            Atividade = _dados.Atividade,
                            Atividade2 = _dados.Atividade_secundaria,
                            //Atividade_Extenso=_dados.Atividade_Extenso,
                            Cpf_Cnpj = _dados.Documento,
                            Data_Abertura = (DateTime)_dados.Data_abertura,
                            Processo_Abertura = _dados.Processo_abertura,
                            Processo_Encerramento = _dados.Processo_encerramento,
                            Situacao = _dados.Situacao,
                            Telefone = _dados.Telefone,
                            Area = (decimal)_dados.Area,
                            Mei = _dados.Mei,
                            Vigilancia_Sanitaria = _dados.Vigilancia_sanitaria,
                            Taxa_Licenca = _dados.Taxa_licenca
                        };
                        if (_dados.Data_encerramento != null)
                            reg.Data_Encerramento = (DateTime)_dados.Data_encerramento;
                        certidao.Add(reg);
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Certidao_Inscricao", model);
                    }

                    ReportDocument rd = new ReportDocument();
                    rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\Comprovante_Inscricao_Valida.rpt");

                    try {
                        rd.SetDataSource(certidao);
                        Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "Certidao_Endereco.pdf");
                    } catch {

                        throw;
                    }
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Inscricao", model);
            }
        }


        [Route("Certidao_Inscricao")]
        [Route("Certidao/Certidao_Inscricao")]
        [HttpPost]
        public ActionResult Certidao_Inscricao(CertidaoViewModel model) {
            int _codigo;
            bool _valida = false;
            int _numero;
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito);
            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            ViewBag.Result = "";

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.CpfValue != null || model.CnpjValue != null) {
                List<int> _lista = new List<int>();
                if (model.CpfValue != null) {
                    _lista = empresaRepository.Retorna_Codigo_por_CPF(Functions.RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        _lista = empresaRepository.Retorna_Codigo_por_CNPJ(Functions.RetornaNumero(model.CnpjValue));
                    }
                }
                if (_lista.Count > 0) {
                    ViewBag.Lista_Codigo = _lista;
                } 
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }
            
            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
            } else {
                ViewBag.Result = "Erro ao recuperar as informações.";
                return View(model);
            }

            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);
            string _sufixo = model.Extrato ? _dados.Data_Encerramento == null ? "XA" : "XE" : "IE";
            List<CnaeStruct> ListaCnae = empresaRepository.Lista_Cnae_Empresa(_codigo);
            string _cnae = "", _cnae2 = "";
            foreach (CnaeStruct cnae in ListaCnae) {
                if (cnae.Principal)
                    _cnae = cnae.CNAE + "-" + cnae.Descricao;
                else
                    _cnae2 += cnae.CNAE + "-" + cnae.Descricao + System.Environment.NewLine;
            }

            Comprovante_Inscricao reg = new Comprovante_Inscricao() {
                Codigo = _codigo,
                Data_Emissao=DateTime.Now,
                Razao_Social=_dados.Razao_social,
                Nome_Fantasia=_dados.Nome_fantasia,
                Cep=_dados.Cep,
                Cidade=_dados.Cidade_nome + "/" + _dados.UF,
                Email=_dados.Email_contato,
                Inscricao_Estadual = _dados.Inscricao_estadual,
                Endereco = _dados.Endereco_nome +", " + _dados.Numero,
                Complemento = _dados.Complemento,
                Bairro = _dados.Bairro_nome ?? "",
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-" + _sufixo,
                Atividade = _cnae,
                Atividade2 = _cnae2,
                Atividade_Extenso=_dados.Atividade_extenso,
                Cpf_Cnpj = _dados.Cpf_cnpj,
                Rg=_dados.Rg,
                Data_Abertura = (DateTime)_dados.Data_abertura,
                Processo_Abertura = _dados.Numprocesso,
                Processo_Encerramento = _dados.Numprocessoencerramento,
                Situacao=_dados.Situacao,
                Telefone=_dados.Fone_contato,
                Uf=_dados.UF,
                Area=(decimal)_dados.Area,
                Mei = Convert.ToBoolean(_dados.Mei) ? "SIM" : "NÃO",
                Vigilancia_Sanitaria = empresaRepository.Empresa_tem_VS(_codigo)?"SIM":"NÃO",
                Taxa_Licenca = empresaRepository.Empresa_tem_TL(_codigo) ? "SIM" : "NÃO"
            };
            if (_dados.Data_Encerramento != null)
                reg.Data_Encerramento = (DateTime)_dados.Data_Encerramento;

            Certidao_inscricao reg2 = new Certidao_inscricao() {
                Cadastro = reg.Codigo,
                Data_emissao=reg.Data_Emissao,
                Data_encerramento=reg.Data_Encerramento,
                Nome = reg.Razao_Social,
                Nome_fantasia = reg.Nome_Fantasia??"",
                Cep = reg.Cep??"",
                Cidade = reg.Cidade??"",
                Email = reg.Email??"",
                Inscricao_estadual = reg.Inscricao_Estadual??"",
                Endereco = reg.Endereco + ", " + reg.Numero,
                Complemento = reg.Complemento??"",
                Bairro = reg.Bairro ?? "",
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Atividade = _cnae??"",
                Atividade_secundaria = _cnae2??"",
              //  Atividade_Extenso=reg.Atividade_Extenso,
                Rg = reg.Rg ?? "",
                Documento = reg.Cpf_Cnpj,
                Data_abertura = (DateTime)reg.Data_Abertura,
                Processo_abertura = reg.Processo_Abertura??"",
                Processo_encerramento = reg.Processo_Encerramento??"",
                Situacao = reg.Situacao,
                Telefone = reg.Telefone??"",
                Area = (decimal)reg.Area,
                Mei = reg.Mei,
                Vigilancia_sanitaria = reg.Vigilancia_Sanitaria,
                Taxa_licenca = reg.Taxa_Licenca
            };
            if (reg.Data_Encerramento != null)
                reg2.Data_encerramento = (DateTime)reg.Data_Encerramento;

            Exception ex = tributarioRepository.Insert_Certidao_Inscricao(reg2);
            if (ex != null)
                throw ex;

            List<Certidao> Lista_Certidao = new List<Certidao>();
            if (model.Extrato) {

                List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, 1980, 2050, 0, 99, 0, 99, 0, 999, 0, 99, 0, 99, DateTime.Now, "Web");
                List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);
                Certidao regCert = new Certidao();

                foreach (SpExtrato item in ListaParcela.Where(x => (x.Codlancamento == 2 || x.Codlancamento == 6 || x.Codlancamento == 14) && x.Statuslanc < 3)) {
                    Certidao_inscricao_extrato regExt = new Certidao_inscricao_extrato {
                        Id = reg.Controle,
                        Numero_certidao = reg.Numero,
                        Ano_certidao = (short)reg.Ano,
                        Ano = item.Anoexercicio,
                        Codigo = item.Codreduzido,
                        Complemento = item.Codcomplemento,
                    };
                    if (item.Datapagamento != null)
                        regExt.Data_Pagamento = Convert.ToDateTime(item.Datapagamento);
                    regExt.Data_Vencimento = item.Datavencimento;
                    regExt.Lancamento_Codigo = item.Codlancamento;
                    regExt.Lancamento_Descricao = item.Desclancamento;
                    regExt.Parcela = (byte)item.Numparcela;
                    regExt.Sequencia = (byte)item.Seqlancamento;
                    regExt.Valor_Pago = (decimal)item.Valorpagoreal;
                    ex = tributarioRepository.Insert_Certidao_Inscricao_Extrato(regExt);
                    if (ex != null)
                        throw ex;
                    regCert.Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-" + _sufixo;
                    regCert.Codigo = _codigo;
                    regCert.Razao_Social = reg2.Nome;
                    regCert.Nome_Requerente = reg2.Nome;
                    regCert.Data_Abertura = Convert.ToDateTime(reg2.Data_abertura);
                    regCert.Processo_Encerramento = reg2.Processo_encerramento;
                    regCert.Endereco = reg2.Endereco;
                    regCert.Endereco_Numero = reg2.Numero;
                    regCert.Endereco_Complemento = reg2.Complemento;
                    regCert.Bairro = reg2.Bairro;
                    regCert.Cidade = reg2.Cidade ;
                    //regCert.Atividade_Extenso = reg2.Atividade_Extenso;
                    regCert.Rg = reg2.Rg;
                    regCert.Cpf_Cnpj = reg2.Documento;
                    regCert.Exercicio = regExt.Ano;
                    regCert.Lancamento_codigo = (byte)regExt.Lancamento_Codigo;
                    regCert.Lancamento_Nome = regExt.Lancamento_Descricao;
                    regCert.Sequencia_Lancamento = regExt.Sequencia;
                    regCert.Complemento = regExt.Complemento;
                    regCert.Data_Vencimento = regExt.Data_Vencimento;
                    regCert.Data_Pagamento = Convert.ToDateTime(regExt.Data_Pagamento);
                    regCert.Valor_Pago = regExt.Valor_Pago;
                    regCert.Processo_Abertura = reg2.Processo_abertura;
                    regCert.Numero_Ano = regExt.Numero_certidao.ToString("00000") + "/" + regExt.Ano_certidao;
                    if (reg2.Data_encerramento != null)
                        regCert.Data_Encerramento = (DateTime)reg.Data_Encerramento;

                    Lista_Certidao.Add(regCert);

                }
                if (Lista_Certidao.Count == 0) {
                    ViewBag.Result = "Esta empresa não possui débitos pagos de ISS/Taxa.";
                    return View(model);
                }

            }

            List<Comprovante_Inscricao> certidao = new List<Comprovante_Inscricao> {
                reg
            };

            ReportDocument rd = new ReportDocument();
            if (model.Extrato) {
                if (_dados.Data_Encerramento != null) {
                    rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\CertidaoInscricaoExtratoEncerrada.rpt");
                } else {
                    rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\CertidaoInscricaoExtratoAtiva.rpt");
                }
            } else {
                if (_valida) {
                    rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\Comprovante_InscricaoValida.rpt");
                } else
                    rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\Comprovante_Inscricao.rpt");
            }
            try {
                if (model.Extrato)
                    rd.SetDataSource(Lista_Certidao);
                else
                    rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Inscricao.pdf");
            } catch {
                throw;
            }

        }

        [Route("Certidao_Pagamento")]
        [HttpGet]
        public ViewResult Certidao_Pagamento() {
            if (HttpContext.Session["gti_V3id"]==null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }

            CertidaoViewModel model = new CertidaoViewModel {
                OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = true },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = false }
            },
                SelectedValue = "cpfCheck"
            };
            return View(model);
        }

        [Route("Certidao_Pagamento")]
        [HttpPost]
        public ActionResult Certidao_Pagamento(CertidaoViewModel model) {
            int _codigo;
            Empresa_bll empresaRepository = new Empresa_bll("GTIconnection");
            if (string.IsNullOrWhiteSpace(HttpContext.Session["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            ViewBag.Result = "";

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.CpfValue != null || model.CnpjValue != null) {
                List<int> _lista = new List<int>();
                if (model.CpfValue != null) {
                    _lista = empresaRepository.Retorna_Codigo_por_CPF(Functions.RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        _lista = empresaRepository.Retorna_Codigo_por_CNPJ(Functions.RetornaNumero(model.CnpjValue));
                    }
                }
                if (_lista.Count > 0) {
                    ViewBag.Lista_Codigo = _lista;
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                bool _existe = empresaRepository.Existe_Empresa(_codigo);
                if (!_existe) {
                    ViewBag.Result = "Empresa não cadastrada.";
                    return View(model);

                }
                EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);
                string _cpf = _dados.Cpf;
                string _cnpj = _dados.Cnpj;

                if (model.CpfValue != null) {
                    bool _validaDoc= Functions.ValidaCpf(model.CpfValue);
                    if (!_validaDoc) {
                        ViewBag.Result = "Nº de CPF inválido.";
                        return View(model);
                    } else {
                        if (Convert.ToInt64(Functions.RetornaNumero(model.CpfValue)).ToString("00000000000") != Convert.ToInt64(Functions.RetornaNumero(_cpf)).ToString("00000000000")) {
                            ViewBag.Result = "Nº de CPF não pertence a esta inscrição.";
                            return View(model);
                        }
                    }
                }

                if (model.CnpjValue != null) {
                    bool _validaDoc = Functions.ValidaCNPJ(model.CnpjValue);
                    if (!_validaDoc) {
                        ViewBag.Result = "Nº de CNPJ inválido.";
                        return View(model);
                    } else {
                        if (Convert.ToInt64(Functions.RetornaNumero(model.CnpjValue)).ToString("00000000000000") != Convert.ToInt64(Functions.RetornaNumero(_cnpj)).ToString("00000000000000")) {
                            ViewBag.Result = "Nº de CNPJ não pertence a esta inscrição.";
                            return View(model);
                        }
                    }
                }

                //se chegou até aqui então a empresa esta ok para verificar os débitos
                Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
                List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, (short)DateTime.Now.Year, (short)DateTime.Now.Year, 0, 99, 0, 99, 0, 999, 0, 99, 0, 99, DateTime.Now, "Web");
                List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);

                if (ListaParcela.Count == 0) {
                    ViewBag.Result = "Não existem débitos de ISS Fixo,Taxa de Licença e Vig.Sanitária para o ano atual.";
                    return View(model);
                }

                int _numero_certidao = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Comprovante_Pagamento);

                List<Certidao> certidao = new List<Certidao>();
                foreach (SpExtrato item in ListaParcela) {
                    if (item.Codlancamento == 2 || item.Codlancamento == 6 || item.Codlancamento == 13 || item.Codlancamento == 14) {
                        if (item.Numparcela > 0 && item.Statuslanc == 1) goto Proximo;
                        if (item.Numparcela == 0 && item.Statuslanc != 1) goto Proximo;

                        Certidao reg = new Certidao {
                            Numero_Ano = _numero_certidao.ToString("00000") + "/" + DateTime.Now.Year.ToString("0000")
                        };
                        reg.Controle= reg.Numero_Ano = _numero_certidao.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-PG";
                        reg.Codigo = _dados.Codigo;
                        reg.Razao_Social = _dados.Razao_social;
                        reg.Cpf_Cnpj = _dados.Cpf_cnpj;
                        reg.Atividade_Extenso = _dados.Atividade_extenso;

                        reg.Ano = item.Anoexercicio;
                        reg.Lancamento_codigo = (byte)item.Codlancamento;
                        reg.Lancamento_Nome = item.Desclancamento;
                        reg.Sequencia_Lancamento = (byte)item.Seqlancamento;
                        reg.Parcela = (byte)item.Numparcela;
                        reg.Complemento = (byte)item.Codcomplemento;
                        reg.Valor_Pago = 0;
                        if (item.Datapagamento != null) {
                            reg.Data_Pagamento = Convert.ToDateTime( item.Datapagamento);
                            reg.Valor_Pago = (decimal)item.Valorpagoreal;
                        }
                        reg.Data_Vencimento = item.Datavencimento;
                        reg.Situacao_Codigo = item.Statuslanc;
                        reg.Situacao_Nome = item.Situacao;
                        certidao.Add(reg);
                    }
                Proximo:;
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\Situacao_Pagamento.rpt");
                try {
                    rd.SetDataSource(certidao);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Situacao_Pagamento.pdf");
                } catch {
                    throw;
                }
            } else {
                ViewBag.Result = "Empresa não cadastrada.";
                return View(model);
            }
        }

        [Route("Empresa_Details_Report")]
        [HttpPost]
        public ActionResult Empresa_Details_Report(EmpresaDetailsViewModel model) {
            if (model.EmpresaStruct.Codigo > 0) {
                List<Empresa_Detalhe> _lista_Dados = new List<Empresa_Detalhe>();
                Empresa_Detalhe _dados = new Empresa_Detalhe() {
                    Codigo = model.EmpresaStruct.Codigo,
                    Razao_Social = model.EmpresaStruct.Razao_social,
                    Data_Abertura = Convert.ToDateTime(model.EmpresaStruct.Data_abertura).ToString("dd/MM/yyyy"),
                    Data_Encerramento = model.EmpresaStruct.Data_Encerramento == null ? "" : Convert.ToDateTime(model.EmpresaStruct.Data_Encerramento).ToString("dd/MM/yyyy"),
                    Area =  model.EmpresaStruct.Area==null?0:(decimal)model.EmpresaStruct.Area,
                    Bairro = model.EmpresaStruct.Bairro_nome,
                    Cidade = model.EmpresaStruct.Cidade_nome,
                    Endereco = model.EmpresaStruct.Endereco_nome + ", " + model.EmpresaStruct.Numero.ToString() + " " + model.EmpresaStruct.Complemento,
                    Cep=model.EmpresaStruct.Cep,
                    Cpf_Cnpj=model.EmpresaStruct.Cpf_cnpj,
                    Cnae=model.Cnae,
                    Email=model.EmpresaStruct.Email_contato,
                    Inscricao_Estadual=model.EmpresaStruct.Inscricao_estadual,
                    Mei=model.Mei,
                    Proprietario=model.EmpresaStruct.prof_responsavel_nome,
                    RegimeISS=model.Regime_Iss,
                    Situacao=model.EmpresaStruct.Situacao,
                    Taxa_Licenca=model.TaxaLicenca,
                    Telefone=model.EmpresaStruct.Fone_contato,
                    Uf=model.EmpresaStruct.UF,
                    Vigilancia_Sanitaria=model.Vigilancia_Sanitaria,
                    Atividade=model.EmpresaStruct.Atividade_extenso
                };
                _lista_Dados.Add(_dados);
                ReportDocument rd = new ReportDocument();
                rd.Load(HostingEnvironment.ApplicationVirtualPath + "\\reports\\Empresa_Detalhe.rpt");
                try {
                    rd.SetDataSource(_lista_Dados);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Ficha_Cadastral.pdf");
                } catch {
                    throw;
                }

            } else {
                ViewBag.Result = "Ocorreu um erro ao gerar o relatório.";
                return View();
            }


        }

    }
}





