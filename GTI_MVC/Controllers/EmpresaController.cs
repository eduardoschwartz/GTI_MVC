
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Models.ReportModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using GTI_Bll.Classes;
using GTI_Models.Models;
using static GTI_Models.modelCore;
using GTI_Mvc.ViewModels;
using GTI_MVC;
using QRCoder;
using System.Drawing;
using Microsoft.Reporting.WebForms;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.SqlClient;


namespace GTI_Mvc.Controllers {

    [Route("Empresa")]
    public class EmpresaController : Controller {
        private readonly string _connection = "GTIconnection";
        public EmpresaController( ) {
        }

        [Route("Details")]
        [HttpGet]
        public ActionResult Details() {
            //Session["hashform"] = "13";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            EmpresaDetailsViewModel model = new EmpresaDetailsViewModel();
            return View(model);
        }
               
        [Route("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(EmpresaDetailsViewModel model) {
        
            Empresa_bll empresaRepository = new Empresa_bll(_connection); 
            int _codigo = 0;
            bool _existeCod = false;
            bool _bCpf = model.CpfValue.Length == 14 ? true : false;
            string _cpf = Functions.RetornaNumero(model.CpfValue);
            EmpresaDetailsViewModel empresaDetailsViewModel = new EmpresaDetailsViewModel();

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secretKey,response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if(!status) {
                empresaDetailsViewModel.ErrorMessage = "Recaptcha inválido.";
                return View(empresaDetailsViewModel);
            }
                       

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo >= 100000 && _codigo < 210000) //Se estiver fora deste intervalo nem precisa checar se a empresa existe
                    _existeCod = empresaRepository.Existe_Empresa(_codigo);
            } else {
                if (!_bCpf) {
                    bool _valida = Functions.ValidaCNPJ(_cpf); //CNPJ válido?
                    if (_valida) {
                        _codigo = empresaRepository.ExisteEmpresaCnpj_Todas(_cpf);
                        if (_codigo > 0)
                            _existeCod = true;
                    } else {
                        empresaDetailsViewModel.ErrorMessage = "Cnpj inválido.";
                        return View(empresaDetailsViewModel);
                    }
                } else {
                    if (model.CpfValue != null) {
                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                        if (_valida) {
                            _codigo = empresaRepository.ExisteEmpresaCpf_Todas(_cpf);
                            if (_codigo > 0)
                                _existeCod = true;

                        } else {
                            empresaDetailsViewModel.ErrorMessage = "Cpf inválido.";
                            return View(empresaDetailsViewModel);
                        }
                    }
                }
            }

            if (_existeCod) {
                //**** log ****************
                int _userid = 2;
                bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
                if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
                string _obs = "Inscrição: " + _codigo.ToString() ;
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 13, Pref = _prf, Obs = _obs };
                sistemaRepository.Incluir_LogWeb(regWeb);
                //*************************

                EmpresaStruct empresa = empresaRepository.Retorna_Empresa(_codigo);
                empresaDetailsViewModel.EmpresaStruct = empresa;
                empresaDetailsViewModel.TaxaLicenca = empresaRepository.Empresa_tem_TL(_codigo) ? "Sim" : "Não";
                empresaDetailsViewModel.Vigilancia_Sanitaria = empresaRepository.Empresa_tem_VS(_codigo) ? "Sim" : "Não";
                empresaDetailsViewModel.Mei = empresaRepository.Empresa_Mei(_codigo) ? "Sim" : "Não";
                List<CnaeStruct> ListaCnae = empresaRepository.Lista_Cnae_Empresa(_codigo);
                string sCnae = "";
                foreach (CnaeStruct cnae in ListaCnae) {
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
                return View( empresaDetailsViewModel);
            }

        }

        public CaptchaResultNew GetCaptcha() {
            string captchaText = CaptchaNew.GenerateRandomCode();
            Session.Add("CaptchaCode", captchaText);
            return new CaptchaResultNew(captchaText);
        }

        [Route("get-captcha-image")]
        public ActionResult GetCaptchaImage() {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            Session["CaptchaCode"]= result.CaptchaCode;
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [Route("Certidao/Certidao_Inscricao")]
        [HttpGet]
        public ActionResult Certidao_Inscricao() {
            //Session["hashform"] = "15";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

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
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
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
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _codigo , _ano ,_numero;
            string _chave = model.Chave;

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
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Comprovante_Inscricao_Valida.rpt"));

                    try {
                        rd.SetDataSource(certidao);
                        Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "Certidao_Inscricao.pdf");
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
        [ValidateAntiForgeryToken]
        public ActionResult Certidao_Inscricao(CertidaoViewModel model) {
            int _codigo;
            bool _valida = false;
            int _numero;
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Debito);
            ViewBag.Result = "";

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.CpfValue != null || model.CnpjValue != null) {
                List<int> _lista = new List<int>();
                if (model.CpfValue != null) {
                    if (!Functions.ValidaCpf(model.CpfValue)) {
                        ViewBag.Result = "CPF inválido.";
                        return View(model);
                    } else
                        _lista = empresaRepository.Retorna_Codigo_por_CPF(Functions.RetornaNumero(model.CpfValue));
                } else {
                    if (model.CnpjValue != null) {
                        if (!Functions.ValidaCNPJ(model.CnpjValue)) {
                            ViewBag.Result = "CNPJ inválido.";
                            return View(model);
                        } else
                            _lista = empresaRepository.Retorna_Codigo_por_CNPJ(Functions.RetornaNumero(model.CnpjValue));
                    }
                }
                if (_lista.Count > 0) {
                    ViewBag.Lista_Codigo = _lista;
                } 
            }

            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
            } else {
                ViewBag.Result = "Nenhum código selecionado ou não disponível.";
                return View(model);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Inscrição: " + _codigo.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 15, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

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
                Nome = reg.Razao_Social,
                Nome_fantasia = reg.Nome_Fantasia??"",
                Cep = reg.Cep??"",
                Cidade = reg.Cidade??"",
                Email = reg.Email??"",
                Inscricao_estadual = reg.Inscricao_Estadual??"",
                Endereco = reg.Endereco ,
                Complemento = reg.Complemento??"",
                Bairro = reg.Bairro ?? "",
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Atividade = _cnae??"",
                Atividade_secundaria = _cnae2??"",
                Atividade_extenso=reg.Atividade_Extenso,
                Rg = reg.Rg ?? "",
                Documento = reg.Cpf_Cnpj,
                Data_abertura = reg.Data_Abertura,
                Processo_abertura = reg.Processo_Abertura??"",
                Processo_encerramento = reg.Processo_Encerramento??"",
                Situacao = reg.Situacao,
                Telefone = reg.Telefone??"",
                Area = reg.Area,
                Mei = reg.Mei,
                Vigilancia_sanitaria = reg.Vigilancia_Sanitaria,
                Taxa_licenca = reg.Taxa_Licenca
            };
            if (reg.Data_Encerramento != null && reg.Data_Encerramento!=DateTime.MinValue)
                reg2.Data_encerramento =reg.Data_Encerramento;

            Exception ex = tributarioRepository.Insert_Certidao_Inscricao(reg2);
            if (ex != null)
                throw ex;

            List<Certidao> Lista_Certidao = new List<Certidao>();
            if (model.Extrato) {

                List<SpExtrato> ListaTributo = tributarioRepository.Lista_Extrato_Tributo(_codigo, 1980, 2050, 0, 99, 0, 99, 0, 999, 0, 99, 0, 99, DateTime.Now, "Web");
                List<SpExtrato> ListaParcela = tributarioRepository.Lista_Extrato_Parcela(ListaTributo);
                

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
                    Certidao regCert = new Certidao();
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
                    regCert.Atividade_Extenso = reg2.Atividade_extenso;
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
                        regCert.Data_Encerramento = reg.Data_Encerramento;

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

            //##### QRCode ##########################################################
            //string Code = "http://www.google.com";
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            //using (Bitmap bitmap = qrCode.GetGraphic(20)) {
            //    using (MemoryStream ms = new MemoryStream()) {
            //        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        byte[] byteImage = ms.ToArray();
            //        certidao[0].QRCodeImage = byteImage;
            //    }
            //}
            //#######################################################################
   
            ReportDocument rd = new ReportDocument();
            if (model.Extrato) {
                if (_dados.Data_Encerramento != null) {
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/CertidaoInscricaoExtratoEncerrada.rpt"));
                } else {
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/CertidaoInscricaoExtratoAtiva.rpt"));
                }
            } else {
                if (_valida) {
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Comprovante_InscricaoValida.rpt"));
                } else
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Comprovante_Inscricao.rpt"));
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
        public ActionResult Certidao_Pagamento() {
            //Session["hashform"] = "17";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

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
        [ValidateAntiForgeryToken]
        public ActionResult Certidao_Pagamento(CertidaoViewModel model) {
            int _codigo;
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
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

            //if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
            //    ViewBag.Result = "Código de verificação inválido.";
            //    return View(model);
            //}
            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
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
                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
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
                        if (item.Numparcela == 0 && item.Statuslanc > 2) goto Proximo;

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
                if(certidao.Count == 0) {
                    ViewBag.Result = "Empresa não possue lançamentos pagos de Iss Fixo, Taxa de Licença e/ou Taxa de Vigilânica Sanitária.";
                    return View(model);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Situacao_Pagamento.rpt"));
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

        public ActionResult PrintDetalhes(EmpresaDetailsViewModel model) {
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
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Empresa_Detalhe.rpt")) ;
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

        [Route("Alvara_Funcionamento")]
        [HttpGet]
        public ActionResult Alvara_Funcionamento() {
            //Session["hashform"] = "14";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel {
                OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = true },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = false }
            },
                SelectedValue = "cpfCheck"
            };
            return View(model);
        }

        [Route("Alvara_Funcionamento")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alvara_Funcionamento(CertidaoViewModel model) {

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            int _codigo = 0;
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";
            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo == 125256 || _codigo == 122881 || _codigo == 122555 || _codigo == 124682 || _codigo == 114560 || _codigo == 116919 || _codigo == 121114 || _codigo == 120576 || 
                    _codigo == 121081 || _codigo == 120580 || _codigo == 120597 || _codigo == 120616 || _codigo == 120699 || _codigo == 120659 || _codigo == 120582 || _codigo ==  120827 || 
                    _codigo == 120734 || _codigo == 118835 || _codigo == 118225 || _codigo == 126742 || _codigo == 112471) { 
                    certidaoViewModel.ErrorMessage = "Esta empresa não pode emitir renovação de alvará.";
                    return View(certidaoViewModel);
                }

                if (_codigo >= 100000 && _codigo < 210000) //Se estiver fora deste intervalo nem precisa checar se a empresa existe
                    _existeCod = empresaRepository.Existe_Empresa(_codigo);

            }
            if(model.CnpjValue != null) {
                string _cnpj = Functions.RetornaNumero( model.CnpjValue);
                bool _valida = Functions.ValidaCNPJ(_cnpj); //CNPJ válido?
                if(_valida) {
                    int _codigo2 = empresaRepository.ExisteEmpresaCnpj(_cnpj);
                    if(_codigo2 != _codigo) {
                        ViewBag.Result = "Cnpj não pertence a esta inscrição.";
                        return View(certidaoViewModel);
                    } 
                } else {
                    ViewBag.Result = "Cnpj inválido.";
                    return View(certidaoViewModel);
                }
            } else {
                if(model.CpfValue != null) {
                    string _cpf = Functions.RetornaNumero( model.CpfValue);
                    bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                    if(_valida) {
                        int _codigo2 = empresaRepository.ExisteEmpresaCpf(_cpf);
                        if(_codigo2 != _codigo) {
                            ViewBag.Result = "Cpf não pertence a esta inscrição.";
                            return View(certidaoViewModel);
                        }
                    } else {
                        ViewBag.Result = "Cpf inválido.";
                        return View(certidaoViewModel);
                    }
                }
            }


            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            if (_existeCod) {
                //**** log ****************
                int _userid = 2;
                bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
                if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
                string _obs = "Inscrição: " + _codigo.ToString();
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 14, Pref = _prf, Obs = _obs };
                sistemaRepository.Incluir_LogWeb(regWeb);
                //*************************

                int _ano_certidao = DateTime.Now.Year;
                int _numero_certidao = empresaRepository.Retorna_Alvara_Disponivel(_ano_certidao);
                string controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _codigo.ToString() + "-AF";

                EmpresaStruct empresa = empresaRepository.Retorna_Empresa(_codigo);

                if (empresaRepository.EmpresaSuspensa(_codigo)) {
                    ViewBag.Result = "A empresa encontra-se suspensa.";
                    return View(certidaoViewModel);
                }
                if (empresa.Data_Encerramento!=null) {
                    ViewBag.Result = "A empresa encontra-se encerrada.";
                    return View(certidaoViewModel);
                }
                if (Convert.ToDateTime(empresa.Data_abertura).Year == DateTime.Now.Year) {
                    ViewBag.Result = "Empresa aberta este ano não pode renovar o alvará.";
                    return View(certidaoViewModel);
                }
                int _atividade_codigo = (int)empresa.Atividade_codigo;
                bool bAtividadeAlvara = empresaRepository.Atividade_tem_Alvara(_atividade_codigo);
                if (!bAtividadeAlvara) {
                    ViewBag.Result = "Atividade da empresa não permite renovar o alvará .";
                    return View(certidaoViewModel);
                }
                bool bIsentoTaxa;
                if (empresa.Isento_taxa == 1)
                    bIsentoTaxa = true;
                else
                    bIsentoTaxa = false;

                if (!bIsentoTaxa) {
                    int _qtde = empresaRepository.Qtde_Parcelas_TLL_Vencidas(_codigo);
                    if (_qtde > 0) {
                        ViewBag.Result = "A taxa de licença não esta paga, favor dirigir-se à Prefeitura para regularizar.";
                        return View(certidaoViewModel);
                    } else {
                        if (empresa.Endereco_codigo == 123 && empresa.Numero == 146) {
                            certidaoViewModel.ErrorMessage = "O endereço desta empresa não permite a emissão de alvará automático.";
                            return View(certidaoViewModel);
                        }
                    }
                }


                Alvara_funcionamento alvara = new Alvara_funcionamento();
                alvara.Ano = (short)_ano_certidao;
                alvara.Numero = _numero_certidao;
                alvara.Controle = controle;
                alvara.Codigo = _codigo;
                alvara.Razao_social = empresa.Razao_social;
                string sDoc = "";
                if (empresa.Cpf_cnpj.Length == 11)
                    sDoc = Convert.ToInt64(Functions.RetornaNumero(empresa.Cpf_cnpj)).ToString(@"000\.000\.000\-00");
                else
                    sDoc = Convert.ToInt64(Functions.RetornaNumero(empresa.Cpf_cnpj)).ToString(@"00\.000\.000\/0000\-00");

                alvara.Documento = sDoc;
                alvara.Endereco = empresa.Endereco_nome + ", " + empresa.Numero.ToString() + " " + empresa.Complemento;
                alvara.Bairro = empresa.Bairro_nome;
                alvara.Atividade = empresa.Atividade_extenso;
                alvara.Horario = string.IsNullOrWhiteSpace(empresa.Horario_extenso) ? empresa.Horario_Nome : empresa.Horario_extenso;
                alvara.Validade = Convert.ToDateTime("30/06/2019");
                alvara.Data_Gravada = DateTime.Now;

                //##### QRCode ##########################################################
                string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkgticd?c=" + alvara.Controle;
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                    using (MemoryStream ms = new MemoryStream()) {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        alvara.QRCodeImage = byteImage;
                    }
                }
                //#######################################################################

                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
                Exception ex = tributarioRepository.Insert_Alvara_Funcionamento(alvara);
                if (ex != null){
                    certidaoViewModel.ErrorMessage = ex.InnerException.ToString();
                    return View(certidaoViewModel);
                }

                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Alvara_Funcionamento.rpt"));
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
                    rd.RecordSelectionFormula = "{Alvara_Funcionamento.ano}=" + alvara.Ano + " and {Alvara_Funcionamento.numero}=" + alvara.Numero;
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Alvara.pdf");
                } catch {
                    throw;
                }
            } else {
                certidaoViewModel.ErrorMessage = "Empresa não cadastrada.";
                return View(certidaoViewModel);
            }

        }

        [HttpPost]
        [Route("Validate_AF")]
        public ActionResult Validate_AF(CertidaoViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _codigo, _ano, _numero;
            string _chave = model.Chave;

            model.OptionList = new List<SelectListaItem> {
                new SelectListaItem { Text = " CPF", Value = "cpfCheck", Selected = model.SelectedValue == "cpfCheck" },
                new SelectListaItem { Text = " CNPJ", Value = "cnpjCheck", Selected = model.SelectedValue == "cnpjCheck" }
            };

            if (model.Chave != null) {
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação inválida.";
                    return View("Alvara_Funcionamento", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;
                    List<Certidao> certidao = new List<Certidao>();
                    Empresa_bll empresaRepository = new Empresa_bll(_connection);
                    Alvara_funcionamento _dados = empresaRepository.Alvara_Funcionamento_gravado(_chave);
                    if (_dados != null) {
                        Certidao reg = new Certidao() {
                            Codigo = _codigo,
                            Razao_Social = _dados.Razao_social,
                            Endereco = _dados.Endereco + ", " + _dados.Numero, 
                            Bairro = _dados.Bairro ?? "",
                            Ano = _ano,
                            Numero = _numero,
                            Controle = _chave,
                            Atividade_Extenso=_dados.Atividade,
                            Cpf_Cnpj = _dados.Documento,
                            Horario = _dados.Horario
                        };
                        certidao.Add(reg);
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Alvara_Funcionamento", model);
                    }

                    ReportDocument rd = new ReportDocument();
                    rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Alvara_Funcionamento_Valida.rpt"));

                    try {
                        rd.SetDataSource(certidao);
                        Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                        return File(stream, "application/pdf", "AlvaraValida.pdf");
                    } catch {

                        throw;
                    }
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Alvara_Funcionamento", model);
            }
        }

        [Route("Carne_tl")]
        [HttpGet]
        public ActionResult Carne_tl() {
            //Session["hashform"] = "11";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Carne_tl")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Carne_tl(CertidaoViewModel model) {
            int _codigo = Convert.ToInt32(model.Inscricao);
            int _ano = 2022;
            bool _bCpf = model.CpfValue.Length == 14 ? true : false;
            string _cpf = Functions.RetornaNumero(model.CpfValue);
            ViewBag.Result = "";
   
            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secretKey,response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if(!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            bool bFind = empresaRepository.Existe_Empresa(_codigo);
            if (bFind) {
                EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);
                if (_bCpf) {
                    if (Convert.ToInt64(Functions.RetornaNumero(_empresa.Cpf_cnpj)).ToString("00000000000") !=   _cpf) {
                        ViewBag.Result = "CPF não pertence ao proprietário desta empresa!";
                        return View(model);
                    }
                } else {
                    if (Convert.ToInt64(Functions.RetornaNumero(_empresa.Cpf_cnpj)).ToString("00000000000000") != _cpf) {
                       ViewBag.Result = "CNPJ não pertence ao proprietário desta empresa!";
                        return View(model);
                    }
                }
            } else {
                ViewBag.Result = "Inscrição Municipal não cadastrada!";
                return View(model);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Inscrição: " + _codigo.ToString() + ", exercício: " + DateTime.Now.Year.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 11, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);

            Paramparcela _parametro_parcela = tributarioRepository.Retorna_Parametro_Parcela(_ano, (int)TipoCarne.Iss_Taxa);
            decimal _SomaISS = 0, _SomaTaxa = 0;

            List<DebitoStructure> Lista_Taxa = tributarioRepository.Lista_Parcelas_Taxa(_codigo, _ano);
            List<DebitoStructure> Lista_Iss = tributarioRepository.Lista_Parcelas_Iss_Fixo(_codigo, _ano);
            bool _temtaxa = Lista_Taxa.Count > 0 ? true : false;
            bool _temiss = Lista_Iss.Count > 0 ? true : false;

            if (_temtaxa) {
                if (Lista_Taxa[0].Soma_Principal == 0)
                    _temtaxa = false;
            }
            List<DebitoStructure> Lista_Unificada = new List<DebitoStructure>();
            int _novo_documento = 0;
            if (_temtaxa) {
                foreach (DebitoStructure item in Lista_Taxa) {//carrega a lista unificada com os dados das taxas
                    DebitoStructure reg = new DebitoStructure();
                    reg.Ano_Exercicio = item.Ano_Exercicio;
                    reg.Codigo_Lancamento = item.Codigo_Lancamento;
                    reg.Sequencia_Lancamento = item.Sequencia_Lancamento;
                    reg.Numero_Parcela = item.Numero_Parcela;
                    reg.Complemento = item.Complemento;
                    reg.Codigo_Tributo = item.Codigo_Tributo;
                    reg.Abreviatura_Tributo = item.Abreviatura_Tributo;
                    reg.Data_Vencimento = item.Data_Vencimento;
                    reg.Numero_Parcela = item.Numero_Parcela;
                    reg.Soma_Principal = item.Soma_Principal;
                    reg.Data_Base = item.Data_Base;

                    //criamos um documento novo para cada parcela da taxa
                    Numdocumento regDoc = new Numdocumento {
                        Valorguia = item.Soma_Principal,
                        Emissor = "Gti.Web/2ViaTL",
                        Datadocumento = DateTime.Now,
                        Registrado = false,
                        Percisencao = 0
                    };
                    regDoc.Percisencao = 0;
                    _novo_documento = tributarioRepository.Insert_Documento(regDoc);
                    reg.Numero_Documento = _novo_documento;
                    item.Numero_Documento = _novo_documento;
                    Lista_Unificada.Add(reg);

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
                    Exception ex = tributarioRepository.Insert_Parcela_Documento(regParc);
                    if (item.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
                        _SomaTaxa += item.Soma_Principal;
                }
            }

            if (_temiss) {
                if (_temtaxa) {//se tiver taxa, tem que juntar os dois na lista unificada
                    bFind = false;
                    int _index = 0;
                    foreach (DebitoStructure item in Lista_Taxa) {
                        decimal _valor_principal = 0;
                        foreach (var item2 in Lista_Iss) {
                            if (item.Ano_Exercicio == item2.Ano_Exercicio  && item.Numero_Parcela==item2.Numero_Parcela ) {
                                _valor_principal = item2.Soma_Principal;
                                Lista_Unificada[_index].Soma_Principal += _valor_principal;
                                if (item2.Numero_Parcela > 0) {
                                    if (item2.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
                                        _SomaISS += item2.Soma_Principal;
                                    Numdocumento _doc = tributarioRepository.Retorna_Dados_Documento((int)item.Numero_Documento);
                                    decimal _valorDoc = (decimal)_doc.Valorguia;
                                    _valorDoc += item2.Soma_Principal;
                                    //Altera valor do documento
                                    Exception ex = tributarioRepository.Alterar_Valor_Documento((int)item.Numero_Documento,_valorDoc);
                                    //grava o documento na parcela
                                    Parceladocumento regParc = new Parceladocumento {
                                        Codreduzido = item2.Codigo_Reduzido,
                                        Anoexercicio = Convert.ToInt16(item2.Ano_Exercicio),
                                        Codlancamento = Convert.ToInt16(item2.Codigo_Lancamento),
                                        Seqlancamento = Convert.ToInt16(item2.Sequencia_Lancamento),
                                        Numparcela = Convert.ToByte(item2.Numero_Parcela),
                                        Codcomplemento = Convert.ToByte(item2.Complemento),
                                        Numdocumento = (int)item.Numero_Documento,
                                        Valorjuros = 0,
                                        Valormulta = 0,
                                        Valorcorrecao = 0,
                                        Plano = 0
                                    };
                                    ex = tributarioRepository.Insert_Parcela_Documento(regParc);
                                }
                                _index++;
                                break;
                            }
                        }
                    }
                } else { //se não tiver taxa, a lista unficada conterá apenas os dados de iss
                    foreach (DebitoStructure item in Lista_Iss) {
                        if (item.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
                            _SomaISS += item.Soma_Principal;
                        DebitoStructure reg = new DebitoStructure();
                        reg.Ano_Exercicio = item.Ano_Exercicio;
                        reg.Codigo_Lancamento =14;
                        reg.Sequencia_Lancamento = item.Sequencia_Lancamento;
                        reg.Numero_Parcela = item.Numero_Parcela;
                        reg.Complemento = item.Complemento;
                        reg.Codigo_Tributo = item.Codigo_Tributo;
                        reg.Abreviatura_Tributo = item.Abreviatura_Tributo;
                        reg.Data_Vencimento = item.Data_Vencimento;
                        reg.Numero_Parcela = item.Numero_Parcela;
                        reg.Soma_Principal = item.Soma_Principal;
                        reg.Data_Base = item.Data_Base;

                        //criamos um documento novo para cada parcela da taxa
                        Numdocumento regDoc = new Numdocumento {
                            Valorguia = item.Soma_Principal,
                            Emissor = "Gti.Web/2ViaTL",
                            Datadocumento = DateTime.Now,
                            Registrado = false,
                            Percisencao = 0
                        };
                        regDoc.Percisencao = 0;
                        _novo_documento = tributarioRepository.Insert_Documento(regDoc);
                        reg.Numero_Documento = _novo_documento;
                        item.Numero_Documento = _novo_documento;
                        Lista_Unificada.Add(reg);

                        //grava o documento na parcela
                        Parceladocumento regParc = new Parceladocumento {
                            Codreduzido = item.Codigo_Reduzido,
                            Anoexercicio = Convert.ToInt16(item.Ano_Exercicio),
                            Codlancamento = Convert.ToInt16(14),
                            Seqlancamento = Convert.ToInt16(item.Sequencia_Lancamento),
                            Numparcela = Convert.ToByte(item.Numero_Parcela),
                            Codcomplemento = Convert.ToByte(item.Complemento),
                            Numdocumento = _novo_documento,
                            Valorjuros = 0,
                            Valormulta = 0,
                            Valorcorrecao = 0,
                            Plano = 0
                        };
                        Exception ex = tributarioRepository.Insert_Parcela_Documento(regParc);
                    }
                }
            }
            int _qtde_parcela = Lista_Unificada.Count;
            if (!_temtaxa && !_temiss) {
                ViewBag.Result = "Solicitação não disponível.";
                return View(model);
            } else {
                string _descricao_lancamento;
                if (_temtaxa && _temiss)
                    _descricao_lancamento = "ISS FIXO/TAXA DE LICENÇA";
                else {
                    if (_temtaxa && !_temiss)
                        _descricao_lancamento = "TAXA DE LICENÇA";
                    else
                        _descricao_lancamento = "ISS FIXO";
                }
                int nSid = Functions.GetRandomNumber();
                EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);
                string _razao = _empresa.Razao_social;
                string _cpfcnpj;
                if (_empresa.Juridica)
                    _cpfcnpj = Convert.ToUInt64(_empresa.Cpf_cnpj).ToString(@"00\.000\.000\/0000\-00");
                else {
                    if (_empresa.Cpf_cnpj.Length > 1)
                        _cpfcnpj = Convert.ToUInt64(_empresa.Cpf_cnpj).ToString(@"000\.000\.000\-00");
                    else
                        _cpfcnpj = "";
                }
                string _cidade = _empresa.Cidade_nome ?? "";
                string _uf = _empresa.UF ?? "";
                string _endereco = _empresa.Endereco_nome;
                short _numimovel = (short)_empresa.Numero;
                string _complemento = _empresa.Complemento;
                string _bairro = _empresa.Bairro_nome;
                string _cep = _empresa.Cep;
                _endereco += ", " + _numimovel.ToString();

                //Registrar os novos documentos
                foreach (DebitoStructure item in Lista_Unificada) {
                    Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                        Nome = _razao.Length > 40 ? _razao.Substring(0, 40) : _razao,
                        Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco,
                        Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro,
                        Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade,
                        Cep = Functions.RetornaNumero( _cep) ?? "14870000",
                        Cpf = Functions.RetornaNumero( _cpfcnpj),
                        Numero_documento = (int)item.Numero_Documento,
                        Data_vencimento = Convert.ToDateTime(item.Data_Vencimento),
                        Valor_documento = Convert.ToDecimal(item.Soma_Principal),
                        Uf = _uf
                    };
                    if (item.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"))) {
                        Exception ex = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
                        if (ex == null)
                            ex = tributarioRepository.Marcar_Documento_Registrado((int)item.Numero_Documento);
                    }
                }

                short _index = 0;
                string _convenio = "2873532";
                List<Boletoguia> ListaBoleto = new List<Boletoguia>();
                string _msg = "";
                foreach (DebitoStructure item in Lista_Unificada) {
                    if (item.Numero_Parcela > 0)
                        _msg = "Após o vencimento tirar 2ª via no site da prefeitura www.jaboticabal.sp.gov.br";

                    Boletoguia reg = new Boletoguia();
                    reg.Usuario = "Gti.Web/2ViaISSTLL";
                    reg.Computer = "web";
                    reg.Sid = nSid;
                    reg.Seq = _index;
                    reg.Codreduzido = _codigo.ToString("000000");
                    reg.Nome = _razao;
                    reg.Cpf = _cpfcnpj;
                    reg.Numimovel = _numimovel;
                    reg.Endereco = _endereco;
                    reg.Complemento = _complemento;
                    reg.Bairro = _bairro;
                    reg.Cidade = "JABOTICABAL";
                    reg.Uf = "SP";
                    reg.Cep = _cep;
                    reg.Desclanc = _descricao_lancamento;
                    reg.Fulllanc = _descricao_lancamento;
                    reg.Numdoc = item.Numero_Documento.ToString();
                    reg.Numparcela = (short)item.Numero_Parcela;
                    reg.Datadoc = item.Data_Base;
                    reg.Datavencto = item.Data_Vencimento;
                    reg.Numdoc2 = item.Numero_Documento.ToString();
                    reg.Valorguia = item.Soma_Principal;
                    reg.Valor_ISS = _SomaISS;
                    reg.Valor_Taxa = _SomaTaxa;
                    reg.Msg = _msg;

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
                    reg.Totparcela = (short)_qtde_parcela;
                    if (item.Numero_Parcela == 0) {
                        reg.Parcela = "Única";
                    } else
                        reg.Parcela = reg.Numparcela.ToString("00") + "/" + reg.Totparcela.ToString("00");


                    reg.Digitavel = _linha_digitavel;
                    reg.Codbarra = _codigo_barra;
                    reg.Nossonumero = _convenio + String.Format("{0:D10}", Convert.ToInt64(reg.Numdoc));
                    if (Convert.ToDateTime(Convert.ToDateTime(reg.Datavencto).ToString("dd/MM/yyyy")) >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
                        ListaBoleto.Add(reg);
                    _index++;
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                if (ListaBoleto.Count > 0) {
                    DataSet Ds = Functions.ToDataSet(ListaBoleto);
                    ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia", Ds.Tables[0]);
                    ReportViewer viewer = new ReportViewer();
                    viewer.LocalReport.Refresh();
                    viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_ISS_TLL.rdlc"); ;
                    viewer.LocalReport.DataSources.Add(rdsAct); // Add  datasource here       

                    decimal _valor_aliquota = empresaRepository.Aliquota_Taxa_Licenca(_codigo);

                    List<ReportParameter> parameters = new List<ReportParameter>();
                    parameters.Add(new ReportParameter("DATADOC", Convert.ToDateTime(ListaBoleto[0].Datadoc).ToString("dd/MM/yyyy")));
                    parameters.Add(new ReportParameter("NOME", ListaBoleto[0].Nome));
                    parameters.Add(new ReportParameter("ENDERECO", ListaBoleto[0].Endereco  + " " + ListaBoleto[0].Complemento));
                    parameters.Add(new ReportParameter("BAIRRO", ListaBoleto[0].Bairro));
                    parameters.Add(new ReportParameter("CIDADE", ListaBoleto[0].Cidade + "/" + ListaBoleto[0].Uf));
                    parameters.Add(new ReportParameter("CODIGO", _codigo.ToString()));
                    parameters.Add(new ReportParameter("IE", _empresa.Inscricao_estadual == "" ? " " : _empresa.Inscricao_estadual));
                    parameters.Add(new ReportParameter("DOC", ListaBoleto[0].Cpf));
                    parameters.Add(new ReportParameter("ATIVIDADE", _empresa.Atividade_extenso));
                    parameters.Add(new ReportParameter("ISS", Convert.ToDecimal(ListaBoleto[0].Valor_ISS).ToString("#0.00")));
                    parameters.Add(new ReportParameter("TAXA", Convert.ToDecimal(ListaBoleto[0].Valor_Taxa).ToString("#0.00")));
                    parameters.Add(new ReportParameter("AREA", Convert.ToDecimal(_empresa.Area).ToString("#0.00")));
                    parameters.Add(new ReportParameter("ALIQUOTA", _valor_aliquota.ToString("#0.00")));
                    viewer.LocalReport.SetParameters(parameters);

                    byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimeType;
                    Response.AddHeader("content-disposition", "attachment; filename= guia_pmj" + "." + extension);
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.Flush();
                    Response.End();
                } else {
                    ViewBag.Result = "Solicitação não disponível.";
                    return View(model);
                }
            }
             return null;
        }


        [Route("Carne_vs")]
        [HttpGet]
        public ActionResult Carne_vs() {
            //Session["hashform"] = "12";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Carne_vs")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Carne_vs(CertidaoViewModel model) {
            int _codigo = Convert.ToInt32(model.Inscricao);
            int _ano = 2022;
            bool _bCpf = model.CpfValue.Length == 14 ? true : false;
            string _cpf = Functions.RetornaNumero(model.CpfValue);

            ViewBag.Result = "";
            var response = Request["g-recaptcha-response"];
            var client = new WebClient();
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secretKey,response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if(!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            bool bFind = empresaRepository.Existe_Empresa(_codigo);
            if (bFind) {
                EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);
                if (_bCpf ) {
                    if (Convert.ToInt64(Functions.RetornaNumero(_empresa.Cpf_cnpj)).ToString("00000000000") != _cpf) {
                        ViewBag.Result = "CPF não pertence ao proprietário desta empresa!";
                        return View(model);
                    }
                } else {
                    if (Convert.ToInt64(Functions.RetornaNumero(_empresa.Cpf_cnpj)).ToString("00000000000000") != _cpf) {
                        ViewBag.Result = "CNPJ não pertence ao proprietário desta empresa!";
                        return View(model);
                    }
                }
            } else {
                ViewBag.Result = "Inscrição Municipal não cadastrada!";
                return View(model);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Inscrição: " + _codigo.ToString() + ", exercício: " + DateTime.Now.Year.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 12, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Paramparcela _parametro_parcela = tributarioRepository.Retorna_Parametro_Parcela(_ano, (int)TipoCarne.Vigilancia);
            int _qtde_parcela = (int)_parametro_parcela.Qtdeparcela;

            List<DebitoStructure> Lista_Taxa = tributarioRepository.Lista_Parcelas_Vigilancia(_codigo, _ano);
            bool _temtaxa = Lista_Taxa.Count > 0 ? true : false;
            decimal _total = Lista_Taxa.Sum(item => item.Soma_Principal);

            if (!_temtaxa) {
                ViewBag.Result = "Solicitação não disponível.";
                return View(model);
            } else {
                string _descricao_lancamento = "VIGILÂNCIA SANITÁRIA";
                int nSid = Functions.GetRandomNumber();

                EmpresaStruct _empresa = empresaRepository.Retorna_Empresa(_codigo);

                string _razao = _empresa.Razao_social;
                string _cpfcnpj;
                if (_empresa.Juridica)
                    _cpfcnpj = Convert.ToUInt64(_empresa.Cpf_cnpj).ToString(@"00\.000\.000\/0000\-00");
                else {
                    if (_empresa.Cpf_cnpj.Length > 1)
                        _cpfcnpj = Convert.ToUInt64(_empresa.Cpf_cnpj).ToString(@"000\.000\.000\-00");
                    else
                        _cpfcnpj = "";
                }
                string _endereco = _empresa.Endereco_nome;
                short _numimovel = (short)_empresa.Numero;
                string _complemento = _empresa.Complemento;
                string _bairro = _empresa.Bairro_nome;
                string _cep = _empresa.Cep;

                short _index = 0;
                string _convenio = "2873532";
                List<Boletoguia> ListaBoleto = new List<Boletoguia>();
                string _msg = "";
                foreach (DebitoStructure item in Lista_Taxa) {
                    if (item.Numero_Parcela > 0)
                        _msg = "Após o vencimento tirar 2ª via no site da prefeitura www.jaboticabal.sp.gov.br";

                    Boletoguia reg = new Boletoguia();
                    reg.Usuario = "Gti.Web/2ViaVSTLL";
                    reg.Computer = "web";
                    reg.Sid = nSid;
                    reg.Seq = _index;
                    reg.Codreduzido = _codigo.ToString("000000");
                    reg.Nome = _razao;
                    reg.Cpf = _cpfcnpj;
                    reg.Numimovel = _numimovel;
                    reg.Endereco = _endereco;
                    reg.Complemento = _complemento;
                    reg.Bairro = _bairro;
                    reg.Cidade = "JABOTICABAL";
                    reg.Uf = "SP";
                    reg.Cep = _cep;
                    reg.Desclanc = _descricao_lancamento;
                    reg.Fulllanc = _descricao_lancamento;
                    reg.Numdoc = item.Numero_Documento.ToString();
                    reg.Numparcela = (short)item.Numero_Parcela;
                    reg.Datadoc = item.Data_Base;
                    reg.Datavencto = item.Data_Vencimento;
                    reg.Numdoc2 = item.Numero_Documento.ToString();
                    reg.Valorguia = item.Soma_Principal;
                    reg.Valor_ISS = 0;
                    reg.Valor_Taxa = _total;
                    reg.Msg = _msg;

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
                    reg.Totparcela = (short)_qtde_parcela;
                    if (item.Numero_Parcela == 0) {
                        reg.Parcela = "Única";
                    } else
                        reg.Parcela = reg.Numparcela.ToString("00") + "/" + (Lista_Taxa.Count - 1).ToString("00");

                    reg.Digitavel = _linha_digitavel;
                    reg.Codbarra = _codigo_barra;
                    reg.Nossonumero = _convenio + String.Format("{0:D10}", Convert.ToInt64(reg.Numdoc));
                    if (Convert.ToDateTime(Convert.ToDateTime(reg.Datavencto).ToString("dd/MM/yyyy")) >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
                        ListaBoleto.Add(reg);

                    _index++;
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                if (ListaBoleto.Count > 0) {
                    DataSet Ds = Functions.ToDataSet(ListaBoleto);
                    ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia", Ds.Tables[0]);
                    DataTable DataTable1 = GetData(_codigo);
                    ReportDataSource rdsCnae = new ReportDataSource("dsMobiliarioAtividadeVS", DataTable1);
                    ReportViewer viewer = new ReportViewer();
                    viewer.LocalReport.Refresh();
                    viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_VS.rdlc"); ;
                    viewer.LocalReport.DataSources.Add(rdsAct); // Add  datasource here       
                    viewer.LocalReport.DataSources.Add(rdsCnae); // Add  datasource here       

                    decimal _valor_aliquota = empresaRepository.Aliquota_Taxa_Licenca(_codigo);

                    List<ReportParameter> parameters = new List<ReportParameter>();
                    parameters.Add(new ReportParameter("DATADOC", Convert.ToDateTime(ListaBoleto[0].Datadoc).ToString("dd/MM/yyyy")));
                    parameters.Add(new ReportParameter("NOME", ListaBoleto[0].Nome));
                    parameters.Add(new ReportParameter("ENDERECO", ListaBoleto[0].Endereco + " " + ListaBoleto[0].Complemento));
                    parameters.Add(new ReportParameter("BAIRRO", ListaBoleto[0].Bairro));
                    parameters.Add(new ReportParameter("CIDADE", ListaBoleto[0].Cidade + "/" + ListaBoleto[0].Uf));
                    parameters.Add(new ReportParameter("CODIGO", _codigo.ToString()));
                    parameters.Add(new ReportParameter("IE", _empresa.Inscricao_estadual == "" ? " " : _empresa.Inscricao_estadual));
                    parameters.Add(new ReportParameter("DOC", ListaBoleto[0].Cpf));
                    parameters.Add(new ReportParameter("ATIVIDADE", _empresa.Atividade_extenso));
                    parameters.Add(new ReportParameter("ISS", Convert.ToDecimal(ListaBoleto[0].Valor_ISS).ToString("#0.00")));
                    parameters.Add(new ReportParameter("TAXA", Convert.ToDecimal(ListaBoleto[0].Valor_Taxa).ToString("#0.00")));
                    parameters.Add(new ReportParameter("AREA", Convert.ToDecimal(_empresa.Area).ToString("#0.00")));
                    parameters.Add(new ReportParameter("ALIQUOTA", _valor_aliquota.ToString("#0.00")));
                    viewer.LocalReport.SetParameters(parameters);

                    byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimeType;
                    Response.AddHeader("content-disposition", "attachment; filename= guia_pmj" + "." + extension);
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.Flush();
                    Response.End();
                }

            }
            return null;
        }

        private DataTable GetData(int Codigo) {

            DataTable dt = new DataTable();
            dt.Columns.Add("Codigo");
            dt.Columns.Add("CNAE");
            dt.Columns.Add("VALOR");

            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            List<CnaeStruct> ListaVS = empresaRepository.Lista_Cnae_Empresa_VS(Codigo);

            foreach (CnaeStruct item in ListaVS) {
                dt.Rows.Add(Codigo, item.CNAE, item.Valor);
            }
            return dt;
        }

        [Route("Alvara")]
        [HttpGet]
        public ActionResult Alvara() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            AlvaraViewModel model = new AlvaraViewModel();
            return View(model);
        }

        [Route("Alvara")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alvara(AlvaraViewModel model) {

            int _codigo = Convert.ToInt32(model.Codigo);
            Empresa_bll empresaRepository = new Empresa_bll(_connection);
            bool _existeCod = empresaRepository.Existe_Empresa(_codigo);
            if (!_existeCod) {
                ViewBag.Result = "Empresa não cadastrada.";
                return View(model);
            }

            string _processoStr = model.Numero_Processo;
            ProcessoNumero _processo = Functions.Split_Processo_Numero(_processoStr);
            Processo_bll processoRepository = new Processo_bll(_connection);
            Exception ex  = processoRepository.ValidaProcesso(_processoStr);
            if(ex!=null){
                ViewBag.Result = "Nº de processo inválido.";
                return View(model);
            }

            bool IsVre = string.IsNullOrEmpty(model.Protocolo_Vre) ? false : true;
            if (IsVre){
                if(model.Data_Vre==null || !Functions.IsDate(model.Data_Vre)) {
                    ViewBag.Result = "Data do protocolo inválida.";
                    return View(model);
                }
                DateTime.TryParse(model.Data_Vre.ToString(), out DateTime _dataVre);
            }

            Empresa_bll empresa_Bll = new Empresa_bll(_connection);
            EmpresaStruct _dados = empresaRepository.Retorna_Empresa(_codigo);
            if (_dados.Data_Encerramento!=null) {
                ViewBag.Result = "Esta empresa esta encerrada.";
                return View(model);
            }

            bool IsProvisorio = model.IsProvisorio;
            if(IsProvisorio && model.Data_Vencimento==DateTime.MinValue) {
                ViewBag.Result = "Data de validade do alvará provisório não informada.";
                return View(model);
            }

            int _ano =  DateTime.Now.Year;
            int _numero = empresaRepository.Retorna_Alvara_Disponivel(_ano);

            Alvara_funcionamento _alvara = new Alvara_funcionamento() {
                Ano=_ano,
                Numero=_numero,
                Codigo=_codigo,
                Razao_social=_dados.Razao_social,
                Documento= Functions.FormatarCpfCnpj(_dados.Cpf_cnpj),
                Endereco=_dados.Endereco_nome+ ", " + _dados.Numero.ToString() + " " + _dados.Complemento??"",
                Bairro=_dados.Bairro_nome??"",
                Atividade=_dados.Atividade_extenso,
                Horario=_dados.Horario_Nome,
                Num_processo=model.Numero_Processo,
                Data_Gravada=DateTime.Now,
                Validade=new DateTime(2022,12,31),
                Redesim=IsVre,
                Provisorio=IsProvisorio,
                Ponto=_dados.Ponto_agencia??""
            };
            List<string> ListaPlaca = empresaRepository.Lista_Placas(_codigo);
            string _placa = "";
            if (ListaPlaca.Count > 0) {
                _placa = ListaPlaca[0];
            }
            _alvara.Placa = _placa;
            if (IsProvisorio)
                _alvara.Validade = model.Data_Vencimento;

            if (IsVre) {
                _alvara.Num_protocolo_vre = model.Protocolo_Vre;
                _alvara.Data_protocolo_vre = model.Data_Vre;
            }

            if (_alvara.Provisorio) {
                _alvara.Redesim = false;
                _alvara.Num_protocolo_vre = null;
                _alvara.Data_protocolo_vre = null;
            }

            string controle = _numero.ToString("00000") + _ano.ToString("0000") + "/" + _codigo.ToString() + "-AF";
            //##### QRCode ##########################################################
            string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkgticd?c=" + controle;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    _alvara.QRCodeImage = byteImage;
                    _alvara.Controle = controle;
                }
            }
            //#######################################################################

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
             ex = tributarioRepository.Insert_Alvara_Funcionamento_Def(_alvara);
            if (ex != null) {
                ViewBag.Result = "Erro ao gravar!";
                return View(model);
            }

            ReportDocument rd = new ReportDocument();
            if(IsProvisorio)
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Alvara_Funcionamento_Prov.rpt"));
            else
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Alvara_Funcionamento_Def.rpt"));
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
                rd.RecordSelectionFormula = "{Alvara_Funcionamento.ano}=" + _alvara.Ano + " and {Alvara_Funcionamento.numero}=" + _alvara.Numero;
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Alvara.pdf");
            } catch {
                throw;
            }

        }

    }
}





