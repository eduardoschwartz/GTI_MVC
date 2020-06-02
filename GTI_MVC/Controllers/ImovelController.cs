using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
            Session["CaptchaCode"] = result.CaptchaCode;
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        #region Certidões
        [Route("Certidao/Certidao_Endereco")]
        [HttpGet]
        public ViewResult Certidao_Endereco() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
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
            ViewBag.Result = "";

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 100000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
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


            Certidao_impressao cimp = new Certidao_impressao() {
                Ano = reg.Ano,
                Numero = reg.Numero,
                Codigo = Convert.ToInt32(reg.Codigo).ToString("000000"),
                Endereco = reg.Endereco,
                Endereco_Numero = reg.Endereco_Numero,
                Endereco_Complemento = reg.Endereco_Complemento,
                Bairro = reg.Bairro,
                Cidade = reg.Cidade,
                Uf = reg.Uf,
                Quadra_Original = reg.Quadra_Original,
                Lote_Original = reg.Lote_Original,
                Inscricao = reg.Inscricao,
                Numero_Ano = reg.Numero_Ano,
                Nome = reg.Nome_Requerente,
                Cpf_Cnpj = reg.Cpf_Cnpj,
                Atividade = reg.Atividade_Extenso,
                Tributo = reg.Tributo,
                Tipo_Certidao = reg.Tipo_Certidao,
                Nao = reg.Nao.ToUpper()
            };
            //##### QRCode ##########################################################
            string Code = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + "/Shared/Checkgticd?c=" + reg.Controle;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            using (Bitmap bitmap = qrCode.GetGraphic(20)) {
                using (MemoryStream ms = new MemoryStream()) {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    regCert.QRCodeImage = byteImage;
                }
            }
            //#######################################################################



            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View(certidaoViewModel);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Endereco.rpt"));
            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            crConnectionInfo.ServerName = "200.232.123.115";
            crConnectionInfo.DatabaseName = "Tributacao";
            crConnectionInfo.UserID = "gtisys";
            crConnectionInfo.Password = "everest";
            CrTables =rd.Database.Tables;
            foreach (Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            try {
                rd.RecordSelectionFormula = "{certidao_endereco.ano}=" + regCert.Ano + " and {certidao_endereco.numero}=" + regCert.Numero;
                rd.SetParameterValue("ANONUMERO", regCert.Numero.ToString("00000") + "/" + regCert.Ano.ToString("0000"));
                rd.SetParameterValue("CADASTRO", regCert.Codigo.ToString("000000")) ;
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
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Endereco_Valida.rpt"));

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
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Certidao/Certidao_Valor_Venal")]
        [HttpPost]
        public ActionResult Certidao_Valor_Venal(CertidaoViewModel model) {
            int _codigo;
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.ValorVenal);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            _codigo = Convert.ToInt32(model.Inscricao);
            if (_codigo < 100000)
                _existeCod =imovelRepository.Existe_Imovel(_codigo);

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
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
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-VV",
                Data_Geracao=DateTime.Now
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

            Certidao_impressao cimp = new Certidao_impressao() {
                Ano = reg.Ano,
                Numero = reg.Numero,
                Codigo = Convert.ToInt32(reg.Codigo).ToString("000000"),
                Endereco = reg.Endereco,
                Endereco_Numero = reg.Endereco_Numero,
                Endereco_Complemento = reg.Endereco_Complemento,
                Bairro = reg.Bairro,
                Cidade = "JABOTICABAL",
                Uf = "SP",
                Quadra_Original = reg.Quadra_Original??"",
                Lote_Original = reg.Lote_Original??"",
                Inscricao = reg.Inscricao,
                Numero_Ano = reg.Numero_Ano,
                Nome = reg.Nome_Requerente,
                Cpf_Cnpj = reg.Cpf_Cnpj,
                Atividade = reg.Atividade_Extenso??"",
                Tributo = reg.Tributo??"",
                Tipo_Certidao = reg.Tipo_Certidao??"",
                Nao = "",
                Vvt=reg.VVT,
                Vvp=reg.VVP,
                Vvi=reg.VVI
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

            if (ex != null) {
                ViewBag.Result = "Ocorreu um erro no processamento das informações.";
                return View(certidaoViewModel);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Valor_Venal.rpt"));
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
                rd.RecordSelectionFormula = "{Certidao_impressao.ano}=" + regCert.Ano + " and {Certidao_impressao.numero}=" + regCert.Numero;
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
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Valor_venal_Valida.rpt"));

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
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
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
            ViewBag.Result = "";

                if (model.Inscricao != null) {
                    _codigo = Convert.ToInt32(model.Inscricao);
                    if (_codigo < 100000)
                        _existeCod = imovelRepository.Existe_Imovel(_codigo);
                }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
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
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/" + reportName));

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
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Isencao_Valida.rpt"));

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

        [Route("Dados_Imovel")]
        [HttpGet]
        public ViewResult Dados_Imovel() {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            return View(model);
        }

        [Route("Dados_Imovel")]
        [HttpPost]
        public ActionResult Dados_Imovel(ImovelDetailsViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _codigo = 0;
            bool _existeCod = false;
            ImovelDetailsViewModel imovelDetailsViewModel = new ImovelDetailsViewModel();

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo  < 50000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
            } else {
                if (model.CnpjValue != null) {
                    string _cnpj = model.CnpjValue;
                    bool _valida = Functions.ValidaCNPJ(_cnpj); //CNPJ válido?
                    if (_valida) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, _cnpj);
                    } else {
                        imovelDetailsViewModel.ErrorMessage = "Cnpj inválido.";
                        return View(imovelDetailsViewModel);
                    }
                } else {
                    if (model.CpfValue != null) {
                        string _cpf = model.CpfValue;
                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                        if (_valida) {
                            _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, _cpf);
                        } else {
                            imovelDetailsViewModel.ErrorMessage = "Cpf inválido.";
                            return View(imovelDetailsViewModel);
                        }
                    }
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                imovelDetailsViewModel.ErrorMessage = "Código de verificação inválido.";
                return View(imovelDetailsViewModel);
            }

            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(TipoCertidao.Ficha_Imovel);
            int _ano_certidao = DateTime.Now.Year;
            string _controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _codigo.ToString() + "-FI";


            string _prop1 = "", _prop2 = "";
            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, false);
            foreach (ProprietarioStruct _prop in listaProp) {
                if (_prop.Tipo == "P" && _prop.Principal)
                    _prop1 = _prop.Nome;
                else
                    _prop2 +=  _prop.Nome + ";";
            }
            if (_prop2.Length > 0)
                _prop2 = _prop2.Substring(0, _prop2.Length - 1);

            List<AreaStruct> areas = imovelRepository.Lista_Area(_codigo);

            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Laseriptu _calc = imovelRepository.Dados_IPTU(_codigo, DateTime.Now.Year);

            Imovel_Detalhe _reg = new Imovel_Detalhe() {
                Codigo = _codigo,
                Inscricao = _dados.Inscricao,
                Situacao_Imovel = _dados.Inativo == true ? "INATIVO" : "ATIVO",
                MT = _dados.NumMatricula.ToString(),
                Proprietario = _prop1,
                Proprietario2 = _prop2,
                Endereco = _dados.NomeLogradouroAbreviado,
                Numero = (int)_dados.Numero,
                Complemento = _dados.Complemento,
                Bairro = _dados.NomeBairro,
                Cep = _dados.Cep,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Area_Terreno = (decimal)_dados.Area_Terreno,
                Fracao_Ideal = (decimal)_dados.FracaoIdeal,
                Testada = (decimal)_calc.Testadaprinc,
                Agrupamento = (decimal)_calc.Agrupamento,
                Soma_Fatores = (decimal)(_calc.Fatorgle * _calc.Fatorped * _calc.Fatorpro * _calc.Fatorsit * _calc.Fatortop),
                Area_Predial=(decimal)_calc.Areaconstrucao,
                Benfeitoria=_dados.Benfeitoria_Nome,
                Categoria=_dados.Categoria_Nome,
                Pedologia=_dados.Pedologia_Nome,
                Topografia=_dados.Topografia_Nome,
                Situacao=_dados.Situacao_Nome,
                Uso_Terreno=_dados.Uso_terreno_Nome,
                Condominio=_dados.NomeCondominio=="NÃO CADASTRADO"?"":_dados.NomeCondominio,
                Iptu=_calc.Impostopredial==0?(decimal)_calc.Impostoterritorial:(decimal)_calc.Impostopredial,
                Qtde_Edif=areas.Count,
                Vvt=(decimal)_calc.Vvt,
                Vvp=(decimal)_calc.Vvc,
                Vvi=(decimal)_calc.Vvi,
                Isento_Cip=_dados.Cip==true?"Sim":"Não",
                Reside_Imovel=_dados.ResideImovel==true?"Sim":"Não",
                Imunidade=_dados.Imunidade==true?"Sim":"Não",
                Controle=_controle
            };

            List<Imovel_Detalhe> _lista_Dados = new List<Imovel_Detalhe>();
            _lista_Dados.Add(_reg);
            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Imovel_Detalhe.rpt"));
            try {
                rd.SetDataSource(_lista_Dados);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Ficha_Cadastral.pdf");
            } catch {
                throw;
            }

        }

        [Route("Carne_Iptu")]
        [HttpGet]
        public ViewResult Carne_Iptu() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Carne_Iptu")]
        [HttpPost]
        public ActionResult Carne_Iptu(CertidaoViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _codigo = 0;
            bool _existeCod = false;
            ImovelDetailsViewModel imovelDetailsViewModel = new ImovelDetailsViewModel();

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 50000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
            } else {
                if (model.CnpjValue != null) {
                    string _cnpj = model.CnpjValue;
                    bool _valida = Functions.ValidaCNPJ(_cnpj); //CNPJ válido?
                    if (_valida) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, _cnpj);
                    } else {
                        imovelDetailsViewModel.ErrorMessage = "Cnpj inválido.";
                        return View(imovelDetailsViewModel);
                    }
                } else {
                    if (model.CpfValue != null) {
                        string _cpf = model.CpfValue;
                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                        if (_valida) {
                            _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, _cpf);
                        } else {
                            imovelDetailsViewModel.ErrorMessage = "Cpf inválido.";
                            return View(imovelDetailsViewModel);
                        }
                    }
                }
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                imovelDetailsViewModel.ErrorMessage = "Código de verificação inválido.";
                return View(imovelDetailsViewModel);
            }

            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            List<AreaStruct> areas = imovelRepository.Lista_Area(_codigo);

            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Laseriptu _calc = imovelRepository.Dados_IPTU(_codigo, DateTime.Now.Year);
            List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);

            List<DebitoStructure> Extrato_Lista = tributario_Class.Lista_Parcelas_IPTU(_codigo, DateTime.Now.Year);
            if (Extrato_Lista.Count == 0) {
                imovelDetailsViewModel.ErrorMessage = "Não é possível emitir 2ª via de IPTU para este contribuinte.";
                return View(imovelDetailsViewModel);
            }

            List<Boletoguia> ListaBoleto = new List<Boletoguia>();
            foreach (DebitoStructure item in Extrato_Lista) {
                Boletoguia reg = new Boletoguia() {
                    Codreduzido = _codigo.ToString("000000"),
                    Nome = _prop[0].Nome,
                    Inscricao_cadastral = _dados.Inscricao,
                    Bairro = _dados.NomeBairro,
                    Cep = _dados.Cep,
                    Cidade = "JABOTICABAL",
                    Cpf = _prop[0].CPF,
                    Datadoc = DateTime.Now,
                    Endereco = _dados.NomeLogradouroAbreviado + "," + _dados.Numero.ToString() + " " + _dados.Complemento,
                    Lote = _dados.LoteOriginal,
                    Quadra = _dados.QuadraOriginal,
                    Totparcela = (short)_calc.Qtdeparc ,
                    Numdoc=item.Numero_Documento.ToString(),
                    Nossonumero= "287353200" + item.Numero_Documento.ToString(),
                    Datavencto = Convert.ToDateTime(item.Data_Vencimento),
                    Valorguia= Convert.ToDecimal(item.Soma_Principal)
                };

                if (item.Numero_Parcela == 0) {
                    if (item.Complemento == 0)
                        reg.Parcela = "Única 5%";
                    else {
                        if (item.Complemento == 91)
                            reg.Parcela = "Única 4%";
                        else
                            reg.Parcela = "Única 3%";
                    }
                } else
                    reg.Parcela = ((int)item.Numero_Parcela).ToString("00") + "/" + reg.Totparcela.ToString("00");

                string _convenio = "2873532";
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

                reg.Codbarra = _codigo_barra;
                reg.Digitavel = _digitavel;

                if(reg.Datavencto>=DateTime.Now)
                    ListaBoleto.Add(reg);
            }

            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            Session["sid"] = "";
            if (ListaBoleto.Count > 0) {
                tributario_Class.Insert_Carne_Web(Convert.ToInt32(ListaBoleto[0].Codreduzido), 2020);
                DataSet Ds = Functions.ToDataSet(ListaBoleto);
                ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia", Ds.Tables[0]);
                ReportViewer viewer = new ReportViewer();
                viewer.LocalReport.Refresh();
                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_IPTU.rdlc"); ;
                viewer.LocalReport.DataSources.Add(rdsAct); // Add  datasource here       

                Laseriptu RegIPTU = tributario_Class.Carrega_Dados_IPTU(Convert.ToInt32(ListaBoleto[0].Codreduzido), DateTime.Now.Year);

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("QUADRA", "Quadra: " + ListaBoleto[0].Quadra + " Lote: " + ListaBoleto[0].Lote));
                parameters.Add(new ReportParameter("DATADOC", Convert.ToDateTime(ListaBoleto[0].Datadoc).ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("NOME", ListaBoleto[0].Nome));
                parameters.Add(new ReportParameter("ENDERECO", ListaBoleto[0].Endereco + " " + ListaBoleto[0].Complemento));
                parameters.Add(new ReportParameter("BAIRRO", ListaBoleto[0].Bairro));
                parameters.Add(new ReportParameter("CIDADE", ListaBoleto[0].Cidade + "/" + ListaBoleto[0].Uf));
                parameters.Add(new ReportParameter("QUADRAO", ListaBoleto[0].Quadra));
                parameters.Add(new ReportParameter("LOTEO", ListaBoleto[0].Lote));
                parameters.Add(new ReportParameter("CODIGO", ListaBoleto[0].Codreduzido));
                parameters.Add(new ReportParameter("INSC", ListaBoleto[0].Inscricao_cadastral));
                parameters.Add(new ReportParameter("FRACAO", Convert.ToDecimal(RegIPTU.Fracaoideal).ToString("#0.00")));
                parameters.Add(new ReportParameter("NATUREZA", RegIPTU.Natureza));
                parameters.Add(new ReportParameter("TESTADA", Convert.ToDecimal(RegIPTU.Testadaprinc).ToString("#0.00")));
                parameters.Add(new ReportParameter("AREAT", Convert.ToDecimal(RegIPTU.Areaterreno).ToString("#0.00")));
                parameters.Add(new ReportParameter("AREAC", Convert.ToDecimal(RegIPTU.Areaconstrucao).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVT", Convert.ToDecimal(RegIPTU.Vvt).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVC", Convert.ToDecimal(RegIPTU.Vvc).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVI", Convert.ToDecimal(RegIPTU.Vvi).ToString("#0.00")));
                parameters.Add(new ReportParameter("IPTU", Convert.ToDecimal(RegIPTU.Impostopredial).ToString("#0.00")));
                parameters.Add(new ReportParameter("ITU", Convert.ToDecimal(RegIPTU.Impostoterritorial).ToString("#0.00")));
                if (RegIPTU.Natureza == "predial")
                    parameters.Add(new ReportParameter("TOTALPARC", Convert.ToDecimal(RegIPTU.Impostopredial).ToString("#0.00")));
                else
                    parameters.Add(new ReportParameter("TOTALPARC", Convert.ToDecimal(RegIPTU.Impostoterritorial).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA1", Convert.ToDecimal(RegIPTU.Valortotalunica).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA2", Convert.ToDecimal(RegIPTU.Valortotalunica2).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA3", Convert.ToDecimal(RegIPTU.Valortotalunica3).ToString("#0.00")));
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
            return null;
        }

        [Route("CadImovel")]
        [HttpGet]
        public ViewResult CadImovel() {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            return View(model);
        }

        [Route("CadImovel")]
        [HttpPost]
        public ViewResult CadImovel(ImovelDetailsViewModel model) {
            model = HomeLoad(Convert.ToInt32( model.Inscricao));
            if (model.ImovelStruct.EE_TipoEndereco == null) {
                ViewBag.Result = "Imóvel não cadastrado.";
            }
            return View(model);
        }
        
        public ImovelDetailsViewModel HomeLoad(int Codigo) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
            model.ImovelStruct = imovel_Class.Dados_Imovel(Codigo);
            model.Lista_Proprietario = imovel_Class.Lista_Proprietario(Codigo, false);
            model.Lista_Areas = imovel_Class.Lista_Area(Codigo);
            if (model.ImovelStruct.EE_TipoEndereco != null) {
                short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                if (_tipoEE == 0)
                    model.Endereco_Entrega = imovel_Class.Dados_Endereco(Codigo, TipoEndereco.Local);
                else {
                    if (_tipoEE == 1)
                        model.Endereco_Entrega = imovel_Class.Dados_Endereco(Codigo, TipoEndereco.Proprietario);
                    else
                        model.Endereco_Entrega = imovel_Class.Dados_Endereco(Codigo, TipoEndereco.Entrega);
                }
            }
            return model;
        }

        [Route("Itbi_menu")]
        [HttpGet]
        public ActionResult Itbi_menu() {
            if (Functions.pUserId == 0)
                return RedirectToAction("Login", "Home");
            return View();
        }

    }
}