﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers {

    [Route("Imovel")]
    public class ImovelController : Controller {

        [Route("get-captcha-image")]
        public ActionResult GetCaptchaImage() {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session["CaptchaCode"] = result.CaptchaCode;
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        #region Certidões
        [Route("Certidao/Certidao_Endereco")]
        [HttpGet]
        public ViewResult Certidao_Endereco() {
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Certidao/Certidao_Endereco")]
        [HttpPost]
        public ActionResult Certidao_Endereco(CertidaoViewModel model) {
            int _codigo = 0;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Endereco);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            ViewBag.Result = "";

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 100000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(certidaoViewModel);
            }

            if (!_existeCod) {
                ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            List<Certidao> certidao = new List<Certidao>();
            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Certidao reg = new Certidao() {
                Codigo = _dados.Codigo,
                Inscricao = _dados.Inscricao,
                Endereco = _dados.NomeLogradouro,
                Endereco_Numero = (int)_dados.Numero,
                Endereco_Complemento = _dados.Complemento,
                Bairro = _dados.NomeBairro ?? "",
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-EA"
            };

            reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
            certidao.Add(reg);

            Certidao_endereco regCert = new Certidao_endereco() {
                Ano = reg.Ano,
                Codigo = reg.Codigo,
                Data = DateTime.Now,
                descbairro = reg.Bairro,
                Inscricao = reg.Inscricao,
                Logradouro = reg.Endereco,
                Nomecidadao = reg.Nome_Requerente,
                Li_lotes = reg.Lote_Original,
                Li_compl = reg.Endereco_Complemento,
                Li_num = reg.Endereco_Numero,
                Li_quadras = reg.Quadra_Original,
                Numero = reg.Numero
            };
            Exception ex = tributarioRepository.Insert_Certidao_Endereco(regCert);
            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View(certidaoViewModel);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Reports/Certidao_Endereco.rpt"));

            try {
                rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Endereco.pdf");
            } catch {
                throw;
            }
        }

        [HttpPost]
        [Route("Validate_CE")]
        [Route("Certidao/Validate_CE")]
        public ActionResult Validate_CE(CertidaoViewModel model) {
            int _codigo, _ano, _numero;
            string _chave = model.Chave;
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            if (model.Chave != null) {
                Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
                Certidao reg = new Certidao();
                List<Certidao> certidao = new List<Certidao>();
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Endereco", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;

                    Certidao_endereco certidaoGerada = tributarioRepository.Retorna_Certidao_Endereco(_ano, _numero, _codigo);
                    if (certidaoGerada != null) {
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
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Certidao_Endereco", model);
                    }
                };
                
                certidao.Add(reg);

                ReportDocument rd = new ReportDocument();
                rd.Load(Server.MapPath("~/Reports/Certidao_Endereco_Valida.rpt"));

                try {
                    rd.SetDataSource(certidao);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Certidao_Endereco.pdf");
                } catch {

                    throw;
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Endereço", model);
            }
        }

        [Route("Certidao/Certidao_Valor_Venal")]
        [HttpGet]
        public ViewResult Certidao_Valor_Venal() {
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Certidao/Certidao_Valor_Venal")]
        [HttpPost]
        public ActionResult Certidao_Valor_Venal(CertidaoViewModel model) {
            int _codigo;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.ValorVenal);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            ViewBag.Result = "";
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            _codigo = Convert.ToInt32(model.Inscricao);
            if (_codigo < 100000)
                _existeCod =imovelRepository.Existe_Imovel(_codigo);

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(certidaoViewModel);
            }

            if (!_existeCod) {
                ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Certidao reg = new Certidao() {
                Codigo = _dados.Codigo,
                Inscricao = _dados.Inscricao,
                Endereco = _dados.NomeLogradouro,
                Endereco_Numero = (int)_dados.Numero,
                Endereco_Complemento = _dados.Complemento,
                Bairro = _dados.NomeBairro ?? "",
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-VV"
            };

            SpCalculo RegCalculo = tributarioRepository.Calculo_IPTU(_dados.Codigo, DateTime.Now.Year);
            if (RegCalculo == null) {
                ViewBag.Result = "Erro ao processar a certidão.";
                return View(certidaoViewModel);
            } else {
                reg.Area = RegCalculo.Areaterreno;
                reg.VVT = RegCalculo.Vvt;
                reg.VVP = RegCalculo.Vvp;
                reg.VVI = RegCalculo.Vvi;
                reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
            }

            Certidao_valor_venal regCert = new Certidao_valor_venal() {
                Ano = reg.Ano,
                Codigo = reg.Codigo,
                Data = DateTime.Now,
                Descbairro = reg.Bairro,
                Inscricao = reg.Inscricao,
                Logradouro = reg.Endereco,
                Nomecidadao = reg.Nome_Requerente,
                Li_lotes = reg.Lote_Original,
                Li_compl = reg.Endereco_Complemento,
                Li_num = reg.Endereco_Numero,
                Li_quadras = reg.Quadra_Original,
                Numero = reg.Numero,
                Areaterreno = RegCalculo.Areaterreno,
                Vvt = RegCalculo.Vvt,
                Vvp = RegCalculo.Vvp,
                Vvi = RegCalculo.Vvi
            };

            Exception ex = tributarioRepository.Insert_Certidao_ValorVenal(regCert);
            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View(certidaoViewModel);
            }
            List<Certidao> certidao = new List<Certidao> {
                reg
            };

            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Reports/Certidao_Valor_Venal.rpt"));

            try {
                rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_VVenal.pdf");
            } catch {
                throw;
            }
        }

        [HttpPost]
        [Route("Validate_VV")]
        [Route("Certidao/Validate_VV")]
        public ActionResult Validate_VV(CertidaoViewModel model) {
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _codigo, _ano, _numero;
            string _chave = model.Chave;
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            if (model.Chave != null) {
                Certidao reg = new Certidao();
                List<Certidao> certidao = new List<Certidao>();
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Isencao", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;

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
                };

                certidao.Add(reg);

                ReportDocument rd = new ReportDocument();
                rd.Load(Server.MapPath("~/Reports/Certidao_Valor_venal_Valida.rpt"));

                try {
                    rd.SetDataSource(certidao);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Certidao_ValorVenal.pdf");
                } catch {

                    throw;
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Valor_Venal", model);
            }
        }

        [Route("Certidao/Certidao_Isencao")]
        [HttpGet]
        public ViewResult Certidao_Isencao() {
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Certidao/Certidao_Isencao")]
        [HttpPost]
        public ActionResult Certidao_Isencao(CertidaoViewModel model) {
            int _codigo = 0;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Isencao);
            bool _existeCod = false;
            string _numero_processo="";
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            ViewBag.Result = "";

                if (model.Inscricao != null) {
                    _codigo = Convert.ToInt32(model.Inscricao);
                    if (_codigo < 100000)
                        _existeCod = imovelRepository.Existe_Imovel(_codigo);
                }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext.Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(certidaoViewModel);
            }

                if (!_existeCod) {
                    ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            decimal SomaArea = imovelRepository.Soma_Area(_codigo);

            bool bImune = imovelRepository.Verifica_Imunidade(_codigo);
            bool bIsentoProcesso = false;
            List<IsencaoStruct> ListaIsencao = null;
            if (!bImune) {
                ListaIsencao = imovelRepository.Lista_Imovel_Isencao(_codigo, DateTime.Now.Year);
                if (ListaIsencao.Count > 0) {
                    bIsentoProcesso = true;
                    _numero_processo = ListaIsencao[0].Numprocesso ?? "";
                }
            }

            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Certidao reg = new Certidao() {
                Codigo = _dados.Codigo,
                Inscricao = _dados.Inscricao,
                Endereco = _dados.NomeLogradouro,
                Endereco_Numero = (int)_dados.Numero,
                Endereco_Complemento = _dados.Complemento,
                Bairro = _dados.NomeBairro ?? "",
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-CI",
                Numero_Processo=_numero_processo,
                Area=SomaArea
            };
            if (ListaIsencao.Count>0) {
                reg.Percentual_Isencao = (decimal)ListaIsencao[0].Percisencao;
                reg.Data_Processo = (DateTime)ListaIsencao[0].dataprocesso;
            }

            decimal nPerc;
            string reportName;
            if (bImune) {
                reportName = "Certidao_Imunidade.rpt";  
                nPerc = 100;
            } else {
                if (bIsentoProcesso) {
                    reportName = "Certidao_Isencao_Processo.rpt";
                    nPerc = (decimal)ListaIsencao[0].Percisencao;
                } else {
                    if (SomaArea <= 65) {
                        //Se tiver área < 65m² mas tiver mais de 1 imóvel, perde a isenção.
                        int nQtdeImovel = imovelRepository.Qtde_Imovel_Cidadao(_codigo);
                        if (nQtdeImovel > 1) {
                            ViewBag.Result = "Este imóvel não esta isento da cobrança de IPTU no ano atual.";
                            return View(certidaoViewModel);
                        }
                        nPerc = 100;
                        reportName = "Certidao_Isencao_65metros.rpt";
                    } else {
                        ViewBag.Result = "Este imóvel não esta isento da cobrança de IPTU no ano atual.";
                        return View(certidaoViewModel);
                    }
                }
            }

            List<Certidao> certidao = new List<Certidao>();
                Certidao_isencao regCert = new Certidao_isencao() {
                    Ano = reg.Ano,
                    Codigo = reg.Codigo,
                    Data = DateTime.Now,
                    Descbairro = reg.Bairro,
                    Inscricao = reg.Inscricao,
                    Logradouro = reg.Endereco,
                    Nomecidadao = reg.Nome_Requerente,
                    Li_lotes = reg.Lote_Original,
                    Li_compl = reg.Endereco_Complemento,
                    Li_num = reg.Endereco_Numero,
                    Li_quadras = reg.Quadra_Original,
                    Numero = reg.Numero,
                    Area = SomaArea,
                    Numprocesso = reg.Numero_Processo ?? "",
                    Dataprocesso = reg.Data_Processo,
                    Percisencao = nPerc
                };
                reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
                certidao.Add(reg);
                Exception ex = tributarioRepository.Insert_Certidao_Isencao(regCert);
                if (ex != null) {
                    ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                    return View(certidaoViewModel);
                }

            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/Reports/" + reportName));

            try {
                rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Isencao.pdf");
            } catch {
                throw;
            }
        }

        [HttpPost]
        [Route("Validate_CS")]
        [Route("Certidao/Validate_CS")]
        public ActionResult Validate_CS(CertidaoViewModel model) {
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            int _codigo, _ano, _numero;
            string _chave = model.Chave;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            if (model.Chave != null) {
                Certidao reg = new Certidao();
                List<Certidao> certidao = new List<Certidao>();
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Isencao", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;

                    Certidao_isencao certidaoGerada = tributarioRepository.Retorna_Certidao_Isencao(_ano, _numero, _codigo);
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
                        reg.Percentual_Isencao = (decimal)certidaoGerada.Percisencao;
                        reg.Numero_Processo = certidaoGerada.Numprocesso ?? "";
                        if (certidaoGerada.Dataprocesso != null)
                            reg.Data_Processo = (DateTime)certidaoGerada.Dataprocesso;
                        reg.Area = (decimal)certidaoGerada.Area;
                        reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Certidao_Endereco", model);
                    }
                };

                certidao.Add(reg);

                ReportDocument rd = new ReportDocument();
                rd.Load(Server.MapPath("~/Reports/Certidao_Isencao_Valida.rpt"));

                try {
                    rd.SetDataSource(certidao);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Certidao_Isencao.pdf");
                } catch {

                    throw;
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Isencao", model);
            }
        }

        #endregion



    }
}