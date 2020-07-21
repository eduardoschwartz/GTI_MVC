using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
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
                Cidade = "JABOTICABAL",
                Uf = "SP",
                Quadra_Original = reg.Quadra_Original ?? "",
                Lote_Original = reg.Lote_Original ?? "",
                Inscricao = reg.Inscricao,
                Numero_Ano = reg.Numero_Ano,
                Nome = reg.Nome_Requerente,
                Cpf_Cnpj = listaProp[0].CPF
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
            CrTables = rd.Database.Tables;
            foreach (Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            try {
                rd.RecordSelectionFormula = "{certidao_endereco.ano}=" + regCert.Ano + " and {certidao_endereco.numero}=" + regCert.Numero;
                rd.SetParameterValue("ANONUMERO", regCert.Numero.ToString("00000") + "/" + regCert.Ano.ToString("0000"));
                rd.SetParameterValue("CADASTRO", regCert.Codigo.ToString("000000"));
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
                _existeCod = imovelRepository.Existe_Imovel(_codigo);

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
                Data_Geracao = DateTime.Now
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
                Quadra_Original = reg.Quadra_Original ?? "",
                Lote_Original = reg.Lote_Original ?? "",
                Inscricao = reg.Inscricao,
                Numero_Ano = reg.Numero_Ano,
                Nome = reg.Nome_Requerente,
                Cpf_Cnpj = reg.Cpf_Cnpj,
                Atividade = reg.Atividade_Extenso ?? "",
                Tributo = reg.Tributo ?? "",
                Tipo_Certidao = reg.Tipo_Certidao ?? "",
                Nao = "",
                Vvt = reg.VVT,
                Vvp = reg.VVP,
                Vvi = reg.VVI
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
            string _numero_processo = "";
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
                Quadra_Original = _dados.QuadraOriginal == null ? "" : _dados.QuadraOriginal.Replace("\"", ""),
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-CI",
                Numero_Processo = _numero_processo,
                Area = SomaArea
            };
            if (ListaIsencao.Count > 0) {
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
                        reg.Data_Processo = DateTime.Now;
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
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(TipoCertidao.Ficha_Imovel);
            int _ano_certidao = DateTime.Now.Year;
            string _controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + _codigo.ToString() + "-FI";


            string _prop1 = "", _prop2 = "";
            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, false);
            foreach (ProprietarioStruct _prop in listaProp) {
                if (_prop.Tipo == "P" && _prop.Principal)
                    _prop1 = _prop.Nome;
                else
                    _prop2 += _prop.Nome + ";";
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
                Area_Predial = (decimal)_calc.Areaconstrucao,
                Benfeitoria = _dados.Benfeitoria_Nome,
                Categoria = _dados.Categoria_Nome,
                Pedologia = _dados.Pedologia_Nome,
                Topografia = _dados.Topografia_Nome,
                Situacao = _dados.Situacao_Nome,
                Uso_Terreno = _dados.Uso_terreno_Nome,
                Condominio = _dados.NomeCondominio == "NÃO CADASTRADO" ? "" : _dados.NomeCondominio,
                Iptu = _calc.Impostopredial == 0 ? (decimal)_calc.Impostoterritorial : (decimal)_calc.Impostopredial,
                Qtde_Edif = areas.Count,
                Vvt = (decimal)_calc.Vvt,
                Vvp = (decimal)_calc.Vvc,
                Vvi = (decimal)_calc.Vvi,
                Isento_Cip = _dados.Cip == true ? "Sim" : "Não",
                Reside_Imovel = _dados.ResideImovel == true ? "Sim" : "Não",
                Imunidade = _dados.Imunidade == true ? "Sim" : "Não",
                Controle = _controle
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
                    Totparcela = (short)_calc.Qtdeparc,
                    Numdoc = item.Numero_Documento.ToString(),
                    Nossonumero = "287353200" + item.Numero_Documento.ToString(),
                    Datavencto = Convert.ToDateTime(item.Data_Vencimento),
                    Valorguia = Convert.ToDecimal(item.Soma_Principal)
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

                if (reg.Datavencto >= DateTime.Now)
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
            model = HomeLoad(Convert.ToInt32(model.Inscricao));
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
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Itbi_urbano")]
        [HttpGet]
        public ActionResult Itbi_urbano(string guid, string a, int s = 0) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ItbiViewModel model = new ItbiViewModel() {
                UserId = Convert.ToInt32(Session["hashid"])
                //UserId = Functions.pUserId
            };
            if (guid == "" || guid == null) {
                model.Codigo = "";
                model.Cpf_Cnpj = "";
                model.Comprador = new Comprador_Itbi();
                model.Lista_Erro = new List<string>();
            } else {
                Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
                List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
                ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
                List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
                ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
                ViewBag.ListaErro = new List<string>();

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
            return View(model);
        }

        [Route("Itbi_urbano")]
        [HttpPost]
        public ActionResult Itbi_urbano(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;

            string _guid = "";
            ModelState.Clear();

            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            List<Itbi_natureza> Lista_Natureza = imovelRepository.Lista_Itbi_Natureza();
            ViewBag.Lista_Natureza = new SelectList(Lista_Natureza, "Codigo", "Descricao");
            List<Itbi_financiamento> Lista_Financimento = imovelRepository.Lista_Itbi_Financiamento();
            ViewBag.Lista_Financiamento = new SelectList(Lista_Financimento, "Codigo", "Descricao");
            ViewBag.Lista_Erro = new List<string>();
            if (model.Comprador == null) {
                model.Comprador = new Comprador_Itbi();
            }
            if (model.Lista_Anexo == null )
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
                        if (Functions.ValidaCNPJ(_cpfCnpj.PadLeft(14, '0'))) {
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

                var cepObj = Classes.Cep.Busca_Correio(Functions.RetornaNumero(model.Comprador.Cep));
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll("GTiconnection");
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
                            var fileName = Path.GetFileName(file.FileName);
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Files/Itbi/") + model.Guid);
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Itbi/" + model.Guid), fileName);
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
                                Seq=model.Lista_Anexo.Count,
                                Arquivo=fileName,
                                Nome=model.Anexo_Desc_tmp
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

            return View(model);
        }

        [Route("Itbi_ok")]
        [HttpGet]
        public ActionResult Itbi_ok() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Itbi_query")]
        [HttpGet]
        public ActionResult Itbi_query(string e = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _fiscal = Session["hashfiscalitbi"]!=null && Session["hashfiscalitbi"].ToString() == "S" ? true : false;
            //List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Query(Functions.pUserId,Functions.pFiscalItbi);
            List<Itbi_Lista> Lista = imovelRepository.Retorna_Itbi_Query(_userId, _fiscal);
            List<ItbiViewModel> model = new List<ItbiViewModel>();
            foreach (Itbi_Lista reg in Lista) {
                ItbiViewModel item = new ItbiViewModel() {
                    Guid = reg.Guid,
                    Data_cadastro = Convert.ToDateTime(reg.Data.ToString("dd/MM/yyyy")),
                    Itbi_NumeroAno = reg.Numero_Ano,
                    Tipo_Imovel = reg.Tipo,
                    Comprador_Nome_tmp = Functions.TruncateTo(reg.Nome_Comprador, 26),
                    Situacao_Itbi_Nome = reg.Situacao
                };
                model.Add(item);
            }
            ViewBag.Erro = e;
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
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
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
            return View(model);
        }

        [Route("Itbi_rural")]
        [HttpPost]
        public ActionResult Itbi_rural(ItbiViewModel model, HttpPostedFileBase file, string action, int seq = 0) {
            bool _bcpf = false, _bcnpj = false;

            string _guid = "";
            ModelState.Clear();

            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
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
                        if (Functions.ValidaCNPJ(_cpfCnpj.PadLeft(14, '0'))) {
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

                var cepObj = Classes.Cep.Busca_Correio(Functions.RetornaNumero(model.Comprador.Cep));
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll("GTiconnection");
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
                            var fileName = Path.GetFileName(file.FileName);
                            Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Files/Itbi/") + model.Guid);
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/Itbi/" + model.Guid), fileName);
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
            ViewBag.Fiscal = Session["hashfiscalitbi"]==null?"N":   Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            return View(model);
        }

        [Route("Itbi_urbano_q")]
        [HttpPost]
        public ActionResult Itbi_urbano_q(ItbiViewModel model, string button) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            if (button==null || button == "print")
                return Itbi_print(model.Guid, true);
            else {
                Itbi_gravar_guia(model);
                model = Retorna_Itbi_Gravado(model.Guid);
                return RedirectToAction("Itbi_query");
            }
        }

        public FileResult Itbi_Download(string p, string f) {
            string fullName = Server.MapPath("~");
            fullName = Path.Combine(fullName, "Files");
            fullName = Path.Combine(fullName, "Itbi");
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
        public ActionResult Itbi_rural_q(ItbiViewModel model, string button) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ViewBag.Fiscal = Session["hashfiscalitbi"].ToString();
            //ViewBag.Fiscal = Functions.pFiscalItbi ? "S" : "N";
            if (button == null || button == "print")
                return Itbi_print(model.Guid, false);
            else {
                Itbi_gravar_guia(model);
                model = Retorna_Itbi_Gravado(model.Guid);
                return RedirectToAction("Itbi_query");
            }
        }

        [Route("Itbi_urbano_e")]
        [HttpGet]
        public ActionResult Itbi_urbano_e(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
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
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            return View("Itbi_urbano_e", model);
        }

        [Route("Itbi_rural_e")]
        [HttpGet]
        public ActionResult Itbi_rural_e(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
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
            ItbiViewModel model = Retorna_Itbi_Gravado(p);
            return View("Itbi_rural_e", model);
        }


        [Route("Itbi_forum")]
        [HttpGet]
        public ActionResult Itbi_forum(string p = "") {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            ItbiViewModel gravado = Retorna_Itbi_Gravado(p);
            List<Itbi_forum> lista = imovelRepository.Retorna_Itbi_Forum(p);
            List<Itbi_Forum> model = new List<Itbi_Forum>();
            if (lista.Count == 0) {
                Itbi_Forum item = new Itbi_Forum() {
                    Guid = gravado.Guid,
                    User_id = gravado.UserId,
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
                }
                model.Add(item);
            } else {
                foreach (Itbi_forum reg in lista) {
                    Itbi_Forum item = new Itbi_Forum() {
                        Guid = reg.Guid,
                        Seq = reg.Seq,
                        Datahora = reg.Datahora,
                        User_id = reg.Userid,
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
                    }
                    model.Add(item);
                }
            }

            return View(model);
        }

        [Route("Itbi_forum")]
        [HttpPost]
        public ActionResult Itbi_forum(List<Itbi_Forum> model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ModelState.Clear();
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            if (model[0].Action == "btnOkMsg") {
                Itbi_forum reg = new Itbi_forum() {
                    Guid = model[0].Guid,
                    Datahora = DateTime.Now,
                    Mensagem = model[0].Mensagem,
                    Userid = Convert.ToInt32( Session["hashid"]),
                    Funcionario=Session["hashfunc"].ToString()=="S"?true:false
                    //Userid = Functions.pUserId,
                    //Funcionario = Functions.pUserGTI
                };
                Exception ex = imovelRepository.Incluir_Itbi_Forum(reg);
            }
          
            return RedirectToAction("Itbi_forum", new { p = model[0].Guid });
        }

        public ItbiViewModel Itbi_Urbano_Load(ItbiViewModel model, bool _bcpf, bool _bcnpj) {
            int Codigo = Convert.ToInt32(model.Codigo);
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
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

                Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
                SpCalculo _calculo = tributarioRepository.Calculo_IPTU(Codigo, DateTime.Now.Year);
                model.Valor_Venal = _calculo.Vvi;

                int _codcidadao = 0;
                if (_bcpf) {
                    _codcidadao = cidadaoRepository.Existe_Cidadao_Cpf(_cpfCnpj.PadLeft(11, '0'));
                } else {
                    if (_bcnpj) {
                        _codcidadao = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfCnpj.PadLeft(14, '0'));
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
                        _comprador.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroR;
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
                        _comprador.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroC;
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
            }
            return model;
        }

        public ItbiViewModel Itbi_Rural_Load(ItbiViewModel model, bool _bcpf, bool _bcnpj) {
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");

            if (model.Comprador == null)
                model.Comprador = new Comprador_Itbi();

            string _cpfCnpj = model.Cpf_Cnpj;

            int _codcidadao = 0;
            if (_bcpf) {
                _codcidadao = cidadaoRepository.Existe_Cidadao_Cpf(_cpfCnpj.PadLeft(11, '0'));
            } else {
                if (_bcnpj) {
                    _codcidadao = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfCnpj.PadLeft(14, '0'));
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
                    _comprador.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroR;
                    _comprador.Logradouro_Nome = _cidadao.EnderecoR;
                    _comprador.Numero = (int)_cidadao.NumeroR;
                    _comprador.Complemento = _cidadao.ComplementoR;
                    _comprador.Bairro_Codigo = (int)_cidadao.CodigoBairroR;
                    _comprador.Bairro_Nome = _cidadao.NomeBairroR;
                    _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                    _comprador.Cidade_Nome = _cidadao.NomeCidadeR;
                    _comprador.UF = _cidadao.UfR;
                    _comprador.Cep =_cidadao.CepR==null?"": ((int)_cidadao.CepR).ToString("00000-000");
                    _comprador.Email = _cidadao.EmailR;
                    _comprador.Telefone = _cidadao.TelefoneR;
                } else {
                    _comprador.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroC;
                    _comprador.Logradouro_Nome = _cidadao.EnderecoC;
                    _comprador.Numero = (int)_cidadao.NumeroC;
                    _comprador.Complemento = _cidadao.ComplementoC;
                    _comprador.Bairro_Codigo = (int)_cidadao.CodigoBairroC;
                    _comprador.Bairro_Nome = _cidadao.NomeBairroC;
                    _comprador.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                    _comprador.Cidade_Nome = _cidadao.NomeCidadeC;
                    _comprador.UF = _cidadao.UfC;
                    _comprador.Cep = _cidadao.CepC==null?"":  ((int)_cidadao.CepC).ToString("00000-000");
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
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            string _guid;
            Exception ex = null;

            //################### Grava Itbi_Main #####################

            if (model.Guid == null) {
                _guid = Guid.NewGuid().ToString("N");
                model.Guid = _guid;
                Itbi_main regMain = new Itbi_main() {
                    Imovel_codigo = Convert.ToInt32(model.Codigo == null ? "0" : model.Codigo),
                    Guid = _guid,
                    Data_cadastro = DateTime.Now,
                    Inscricao = model.Dados_Imovel.Inscricao,
                    Proprietario_Codigo = model.Dados_Imovel.Proprietario_Codigo == null ? 0 : (int)model.Dados_Imovel.Proprietario_Codigo,
                    Proprietario_Nome = model.Dados_Imovel.Proprietario_Nome,
                    Situacao_itbi = 1,
                    Userid = Convert.ToInt32(Session["hashid"]),
                    Funcionario = Session["hashfunc"].ToString() == "S" ? true : false
                };
                ex = imovelRepository.Incluir_Itbi_main(regMain);
            } else {
                _guid = model.Guid;
                Itbi_main regMain = imovelRepository.Retorna_Itbi_Main(_guid);
                if (Functions.IsDate(model.Data_Transacao))
                    regMain.Data_Transacao = model.Data_Transacao;
                regMain.Tipo_Instrumento = model.Tipo_Instrumento;
                regMain.Valor_Venal = model.Valor_Venal;
                regMain.Valor_Avaliacao = model.Valor_Avaliacao;
                regMain.Valor_Transacao = model.Valor_Transacao;
                regMain.Tipo_Financiamento = model.Tipo_Financiamento;
                regMain.Totalidade = model.Totalidade;
                regMain.Totalidade_Perc = model.Totalidade_Perc;
                regMain.Matricula = model.Matricula;
                regMain.Inscricao_Incra = model.Inscricao_Incra;
                regMain.Receita_Federal = model.Receita_Federal;
                regMain.Descricao_Imovel = model.Descricao_Imovel;
                regMain.Natureza_Codigo = model.Natureza_Codigo;
                regMain.Imovel_endereco = model.Dados_Imovel.NomeLogradouro;
                regMain.Imovel_numero = model.Dados_Imovel.Numero == null ? 0 : (short)model.Dados_Imovel.Numero;
                regMain.Imovel_complemento = model.Dados_Imovel.Complemento;
                regMain.Imovel_cep = Convert.ToInt32(Functions.RetornaNumero(model.Dados_Imovel.Cep));
                regMain.Imovel_bairro = model.Dados_Imovel.NomeBairro;
                regMain.Imovel_Quadra = model.Dados_Imovel.QuadraOriginal;
                regMain.Imovel_Lote = model.Dados_Imovel.LoteOriginal;
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
                regMain.Recursos_proprios_aliq = 1.5M;
                regMain.Recursos_proprios_atual = model.Recursos_proprios_atual;
                regMain.Recursos_conta_valor = model.Recursos_conta_valor;
                regMain.Recursos_conta_aliq = 1.5M;
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
                regMain.Valor_guia_atual =  model.Valor_guia_atual ;
                regMain.Valor_Transacao = model.Valor_Transacao;
                regMain.Valor_Venal = model.Valor_Venal;

                ex = imovelRepository.Alterar_Itbi_Main(regMain);
            }

            //################### Grava Itbi_Comprador #####################
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

            return _guid;
        }

        private ItbiViewModel Retorna_Itbi_Gravado(string guid) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");

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
                Numero_Guia = regMain.Numero_Guia
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
            if (model.Valor_Venal == 0) {
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
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Itbi_status stat = imovelRepository.Retorna_Itbi_Situacao(p);
            if (stat.Codigo != 2) {
                return RedirectToAction("Itbi_query", new { e = "P" });
            } else {
                Itbi_main _itbi = imovelRepository.Retorna_Itbi_Main(p);

                Itbi_Guia _guia = new Itbi_Guia() {
                    Guid = _itbi.Guid,
                    Inscricao = _itbi.Inscricao??"",
                    Imovel_Codigo = _itbi.Imovel_codigo,
                    Imovel_Endereco = _itbi.Imovel_endereco??"",
                    Imovel_Numero = _itbi.Imovel_numero,
                    Imovel_Complemento = _itbi.Imovel_complemento ?? "",
                    Imovel_Bairro = _itbi.Imovel_bairro ?? "",
                    Imovel_Cep = _itbi.Imovel_cep,
                    Imovel_Lote = _itbi.Imovel_Lote ?? "",
                    Imovel_Quadra = _itbi.Imovel_Quadra ?? "",
                    Proprietario_Nome = _itbi.Proprietario_Nome??"",
                    Itbi_Ano = _itbi.Itbi_Ano,
                    Itbi_Numero = _itbi.Itbi_Numero,
                    Data_Cadastro = _itbi.Data_cadastro,
                    Data_Transacao = Convert.ToDateTime(_itbi.Data_Transacao),
                    Comprador_Codigo = _itbi.Comprador_codigo,
                    Comprador_Nome = _itbi.Comprador_nome,
                    Comprador_Cpf_Cnpj = _itbi.Comprador_cpf_cnpj,
                    Comprador_Logradouro = _itbi.Comprador_logradouro_nome,
                    Comprador_Numero = _itbi.Comprador_numero,
                    Comprador_Complemento = _itbi.Imovel_complemento ?? "",
                    Comprador_Bairro = _itbi.Comprador_bairro_nome ?? "",
                    Comprador_Cep = _itbi.Comprador_cep,
                    Comprador_Cidade = _itbi.Comprador_cidade_nome,
                    Comprador_Uf = _itbi.Comprador_uf,
                    Inscricao_Incra = _itbi.Inscricao_Incra ?? "",
                    Receita_Federal = _itbi.Receita_Federal ?? "",
                    Descricao_Imovel = _itbi.Descricao_Imovel ?? "",
                    Matricula = _itbi.Matricula,
                    Valor_Avaliacao = _itbi.Valor_Avaliacao,
                    Valor_Guia =    _itbi.Valor_guia_atual>0? _itbi.Valor_guia_atual: _itbi.Valor_guia,
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
                    Valor_Avaliacao_Atual=_itbi.Valor_Avaliacao_atual,
                    Valor_Guia_Atual= _itbi.Valor_guia_atual > 0 ? _itbi.Valor_guia_atual : _itbi.Valor_guia,
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
                if (string.IsNullOrWhiteSpace( _itbi.Descricao_Imovel ))
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
                rd.RecordSelectionFormula = "{itbi_main.guid}='" + p + "'";
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Resumo_Itbi.pdf");
            } catch (Exception ex) {
                throw;
            }
        }

        public ActionResult Itbi_cancel(string p, int s) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
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

            Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");

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
            if (_data.DayOfWeek == DayOfWeek.Saturday)
                _data.AddDays(2);
            else {
                if (_data.DayOfWeek == DayOfWeek.Sunday)
                    _data.AddDays(1);
            }

            return _data;
        }

        private void Itbi_gravar_guia(ItbiViewModel model){
            Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
            int _fiscal = Convert.ToInt32(Session["hashid"]);
            //int _fiscal = Functions.pUserId;
            int _codigo = Convert.ToInt32(model.Comprador.Codigo);
            short _ano = (short)DateTime.Now.Year;
            short _seq = tributarioRepository.Retorna_Proxima_Seq_Itbi(_codigo, _ano);
            DateTime _dataVencto = Retorna_Data_Vencimento_Itbi();
            string _nome = model.Comprador.Nome.Length > 40 ? model.Comprador.Nome.Substring(0, 40) : model.Comprador.Nome;
            string _endereco = model.Comprador.Logradouro_Nome.Length > 40 ? model.Comprador.Logradouro_Nome.Substring(0, 40) : model.Comprador.Logradouro_Nome;
            string _bairro = model.Comprador.Bairro_Nome.Length > 40 ? model.Comprador.Bairro_Nome.Substring(0, 40) : model.Comprador.Bairro_Nome;
            string _cidade = model.Comprador.Cidade_Nome.Length > 40 ? model.Comprador.Cidade_Nome.Substring(0, 40) : model.Comprador.Cidade_Nome;

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
                Valortributo = model.Valor_guia_atual
            };
            Exception ex2 = tributarioRepository.Insert_Debito_Tributo(regTributo);

            //grava o documento
            Numdocumento regDoc = new Numdocumento();
            regDoc.Valorguia = model.Valor_guia_atual;
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
            regParc.Seqlancamento =_seq;
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
                Userid=_fiscal,
                Data=DateTime.Now
            };
            ex = tributarioRepository.Insert_Observacao_Parcela(ObsReg);

            //Enviar para registrar 
            Ficha_compensacao_documento ficha = new Ficha_compensacao_documento();
            ficha.Nome = _nome;
            ficha.Endereco = _endereco.Length > 40 ? _endereco.Substring(0, 40) : _endereco;
            ficha.Bairro = _bairro.Length > 15 ? _bairro.Substring(0, 15) : _bairro;
            ficha.Cidade = _cidade.Length > 30 ? _cidade.Substring(0, 30) : _cidade;
            ficha.Cep = Functions.RetornaNumero( model.Comprador.Cep) ?? "14870000";
            ficha.Cpf = Functions.RetornaNumero( model.Cpf_Cnpj);
            ficha.Numero_documento =  _novo_documento;
            ficha.Data_vencimento = _dataVencto;
            ficha.Valor_documento = Convert.ToDecimal(model.Valor_guia_atual);
            ficha.Uf = model.Comprador.UF;
            ex = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
            ex = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);

            //Alterar Itbi
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            ex = imovelRepository.Alterar_Itbi_Guia(model.Guid, _novo_documento, _dataVencto,_fiscal);
            ex = imovelRepository.Alterar_Itbi_Situacao(model.Guid, 2);
            return;
        }
        

    }
}
