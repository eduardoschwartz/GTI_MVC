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
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-EA"
            };

            Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");
            Bairro _bairro = enderecoRepository.RetornaLogradouroBairro((int)_dados.CodigoLogradouro, (short)_dados.Numero);
            reg.Bairro = _bairro.Descbairro ?? "";

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
            if (ListaIsencao != null && ListaIsencao.Count > 0) {
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
                    if (SomaArea <= 65 && SomaArea > 0) {
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

            if (!bImune) {
                List<AreaStruct> ListaArea = imovelRepository.Lista_Area(_codigo);
                foreach (AreaStruct item in ListaArea) {
                    if (item.Tipo_Codigo == 2) {
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
                Dataprocesso = reg.Data_Processo == DateTime.MinValue ? DateTime.Now : reg.Data_Processo,
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
            //            Laseriptu _calc = imovelRepository.Dados_IPTU(_codigo, DateTime.Now.Year);
            Testada _testada = imovelRepository.Retorna_Testada_principal(_codigo, _dados.Seq);
            //            if (_calc == null) {
            SpCalculo _calc = tributario_Class.Calculo_IPTU(_codigo, DateTime.Now.Year);
            //                _calc.Agrupamento = _newcalc.Agrupamento;
            //               _calc.Fatorcat = _newcalc.Fcat;
            //              _calc.f
            //         }

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
                Testada = (decimal)_testada.Areatestada,
                Agrupamento = (decimal)_calc.Agrupamento,
                Soma_Fatores = (decimal)(_calc.Fgle * _calc.Fped * _calc.Fpro * _calc.Fsit * _calc.Ftop),
                Area_Predial = (decimal)_calc.Areapredial,
                Benfeitoria = _dados.Benfeitoria_Nome,
                Categoria = _dados.Categoria_Nome,
                Pedologia = _dados.Pedologia_Nome,
                Topografia = _dados.Topografia_Nome,
                Situacao = _dados.Situacao_Nome,
                Uso_Terreno = _dados.Uso_terreno_Nome,
                Condominio = _dados.NomeCondominio == "NÃO CADASTRADO" ? "" : _dados.NomeCondominio,
                Iptu = _calc.Valoriptu == 0 ? (decimal)_calc.Valoritu : (decimal)_calc.Valoriptu,
                Qtde_Edif = areas.Count,
                Vvt = (decimal)_calc.Vvt,
                Vvp = (decimal)_calc.Vvp,
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
            int _ano = 2021;
            bool _existeCod = false;
            ImovelDetailsViewModel imovelDetailsViewModel = new ImovelDetailsViewModel();

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 50000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
                else {
                    model.ErrorMessage = "Código inválido.";
                    return View(model);
                }
                if (!_existeCod) {
                    model.ErrorMessage = "Imóvel não cadastrado.";
                    return View(model);
                }
                if (model.CnpjValue != null) {
                    string _cnpj = model.CnpjValue;
                    bool _valida = Functions.ValidaCNPJ(_cnpj); //CNPJ válido?
                    if (_valida) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo,Functions.RetornaNumero( _cnpj));
                    } else {
                        model.ErrorMessage = "Cnpj inválido.";
                        return View(model);
                    }
                    if (!_existeCod) {
                        model.ErrorMessage = "Este Cnpj não pertence ao imóvel.";
                        return View(model);
                    }
                } else {
                    if (model.CpfValue != null) {
                        string _cpf = model.CpfValue;
                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
                        if (_valida) {
                            _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, Functions.RetornaNumero( _cpf));
                        } else {
                            model.ErrorMessage = "Cpf inválido.";
                            return View(model);
                        }
                        if (!_existeCod) {
                            model.ErrorMessage = "Este Cpf não pertence ao imóvel.";
                            return View(model);
                        }
                    }
                }
            } else {
                model.ErrorMessage = "Digite o Código do imóvel.";
                return View(model);
            }


            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                model.ErrorMessage = "Código de verificação inválido.";
                return View(model);
            }

            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            List<AreaStruct> areas = imovelRepository.Lista_Area(_codigo);

            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Laseriptu _calc = imovelRepository.Dados_IPTU(_codigo, _ano);
            Laseriptu_ext _calc2= null;
            if (_calc==null)
                 _calc2 = imovelRepository.Dados_IPTU_Ext(_codigo, _ano);

            List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);

            List<DebitoStructure> Extrato_Lista = tributario_Class.Lista_Parcelas_IPTU(_codigo,_ano);
            if (Extrato_Lista.Count == 0 || (_calc==null && _calc2==null)) {
                model.ErrorMessage = "Não é possível emitir 2ª via de IPTU para este contribuinte.";
                return View(model);
            }

            string _msg="" ;
            List<Boletoguia> ListaBoleto = new List<Boletoguia>();
            foreach (DebitoStructure item in Extrato_Lista) {
                if (item.Numero_Parcela > 0)
                    _msg = "Após o vencimento tirar 2ª via no site da prefeitura www.jaboticabal.sp.gov.br";

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
                    Totparcela =  _calc!=null?  (short)_calc.Qtdeparc : (short)_calc2.Qtdeparc,
                    Numdoc = item.Numero_Documento.ToString(),
                    Nossonumero = "287353200" + item.Numero_Documento.ToString(),
                    Datavencto = Convert.ToDateTime(item.Data_Vencimento),
                    Valorguia = Convert.ToDecimal(item.Soma_Principal),
                    Msg=_msg
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

                if (Convert.ToDateTime(Convert.ToDateTime(reg.Datavencto).ToString("dd/MM/yyyy")) >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")))
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

                Laseriptu RegIPTU = tributario_Class.Carrega_Dados_IPTU(Convert.ToInt32(ListaBoleto[0].Codreduzido), _ano);
                Laseriptu_ext RegIPTU_Ext = null;
                if(RegIPTU==null)
                    RegIPTU_Ext = tributario_Class.Carrega_Dados_IPTU_Ext(Convert.ToInt32(ListaBoleto[0].Codreduzido), _ano);


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
                parameters.Add(new ReportParameter("TOTALPARC", Convert.ToDecimal((ListaBoleto.Count - 3) * ListaBoleto[3].Valorguia).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA1", Convert.ToDecimal(ListaBoleto[0].Valorguia).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA2", Convert.ToDecimal(ListaBoleto[1].Valorguia).ToString("#0.00")));
                parameters.Add(new ReportParameter("UNICA3", Convert.ToDecimal(ListaBoleto[2].Valorguia).ToString("#0.00")));
       
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

        [Route("Carne_Cip")]
        [HttpGet]
        public ViewResult Carne_Cip() {
            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Carne_Cip")]
        [HttpPost]
        public ActionResult Carne_Cip(CertidaoViewModel model) {
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
            List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);

            List<DebitoStructure> Extrato_Lista = tributario_Class.Lista_Parcelas_CIP(_codigo, DateTime.Now.Year);
            if (Extrato_Lista.Count == 0) {
                imovelDetailsViewModel.ErrorMessage = "Não é possível emitir 2ª via da CIP para este contribuinte.";
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
                    Totparcela = 3,
                    Numdoc = item.Numero_Documento.ToString(),
                    Nossonumero = "287353200" + item.Numero_Documento.ToString(),
                    Datavencto = Convert.ToDateTime(item.Data_Vencimento),
                    Valorguia = Convert.ToDecimal(item.Soma_Principal),
                    Desclanc = "CONTRIBUIÇÃO DE ILUMINAÇÃO PÚBLICA (CIP-2020)"
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

                string _convenio = "2950230";
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
                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_CIP.rdlc"); ;
                viewer.LocalReport.DataSources.Add(rdsAct); // Add  datasource here       



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
        public ActionResult CadImovel(string c) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            int _codigo = 0;
            try {
                _codigo = Convert.ToInt32(Functions.Decrypt(c));
            } catch {
                return RedirectToAction("Login_gti", "Home");
            }
            
            model = HomeLoad(_codigo);
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
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            model.ImovelStruct = imovelRepository.Dados_Imovel(Codigo);
            model.Lista_Proprietario = imovelRepository.Lista_Proprietario(Codigo, false);
            model.Lista_Areas = imovelRepository.Lista_Area(Codigo);
            model.Lista_Testada = imovelRepository.Lista_Testada(Codigo);
            if (model.ImovelStruct.EE_TipoEndereco != null) {
                short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                if (_tipoEE == 0)
                    model.Endereco_Entrega = imovelRepository.Dados_Endereco(Codigo, TipoEndereco.Local);
                else {
                    if (_tipoEE == 1)
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(Codigo, TipoEndereco.Proprietario);
                    else
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(Codigo, TipoEndereco.Entrega);
                }
            }
            return model;
        }

        [Route("Notificacao_ter_menu")]
        [HttpGet]
        public ActionResult Notificacao_ter_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Notificacao_ter_add")]
        [HttpGet]
        public ActionResult Notificacao_ter_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            return View(model);
        }

        [Route("Notificacao_ter_add")]
        [HttpPost]
        public ActionResult Notificacao_ter_add(NotificacaoTerViewModel model, string action) {
            if (model.Codigo_Imovel == 0) {
                ViewBag.Result = "Código de imóvel inválido.";
                return View(model);
            }
            int _codigo = model.Codigo_Imovel;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Endereco_bll enderecoRepository = new Endereco_bll("GTIconnection");
            if (action == "btnCodigoOK") {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                Cidadao_bll cidadaoRepository = new Cidadao_bll("GTIconnection");
                List<ProprietarioStruct> Listaprop = imovelRepository.Lista_Proprietario(_codigo, false);
                if (Listaprop.Count == 0) {
                    ViewBag.Result = "Não é possível emitir notificação para este imóvel.";
                    model = new NotificacaoTerViewModel();
                    return View(model);
                }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if( prop.Tipo=="P" &&  prop.Principal ) {
                        model.Codigo_cidadao =prop.Codigo;
                        model.Nome_Proprietario = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg = _cidadao.Rg;
                        model.Cpf = string.IsNullOrEmpty(_cidadao.Cnpj)? _cidadao.Cpf:_cidadao.Cnpj;
                        model.Cpf = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                    }
                }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && !prop.Principal) {
                        model.Codigo_cidadao2 = prop.Codigo;
                        model.Nome_Proprietario2 = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg2 = _cidadao.Rg;
                        model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                    }
                }
                if(string.IsNullOrEmpty(model.Nome_Proprietario2) && Listaprop.Count > 1) {
                    foreach (ProprietarioStruct prop in Listaprop) {
                        if (prop.Tipo != "P") {
                            model.Codigo_cidadao2 = prop.Codigo;
                            model.Nome_Proprietario2 = prop.Nome;
                            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                            model.Rg2 = _cidadao.Rg;
                            model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                            model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf2);
                            break;
                        }
                    }
                }

                model.Inscricao = _imovel.Inscricao;
                EnderecoStruct _endLocal = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                string _compl = _endLocal.Complemento == null ? "" : " " + _endLocal.Complemento;
                model.Endereco_Local = _endLocal.Endereco + ", " +_endLocal.Numero.ToString()  + _compl +  " - " + _endLocal.NomeBairro.ToString() + " - " + _endLocal.NomeCidade + "/" + _endLocal.UF + " Cep:" + _endLocal.Cep;

                Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
                Contribuinte_Header_Struct _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao);
                _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                model.Endereco_Prop = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep ;

                if (model.Codigo_cidadao2 > 0) {
                    _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao2);
                    _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                    model.Endereco_prop2 = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;
                }

                EnderecoStruct _endEntrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                if (_endEntrega.Endereco != null) {
                    _compl = _endEntrega.Complemento == null ? "" : " " + _endEntrega.Complemento;
                    model.Endereco_Entrega = _endEntrega.Endereco + ", " + _endEntrega.Numero.ToString() + _compl + " - " + _endEntrega.NomeBairro.ToString() + " - " + _endEntrega.NomeCidade + "/" + _endEntrega.UF + " Cep:" + _endEntrega.Cep;
                } else {
                    if (_imovel.EE_TipoEndereco == 0)
                        model.Endereco_Entrega = model.Endereco_Local;
                    else {
                        model.Endereco_Entrega = model.Endereco_Prop;
                    }
                }
            }

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
            }
            if (action == "btnValida") {
                bool _existe = imovelRepository.Existe_Notificacao_Terreno(model.Ano_Notificacao, model.Numero_Notificacao);
                if (_existe) {
                    ViewBag.Result = "Nº de notificação já cadastrado.";
                    return View(model);
                } else { 
                    Save_Notificacao_Terreno(model);
                    return RedirectToAction("Notificacao_ter_query");
                }
            }

            return View(model);
        }

        private ActionResult Save_Notificacao_Terreno(NotificacaoTerViewModel model) {
            Notificacao_terreno reg = new Notificacao_terreno() {
                Ano_not = model.Ano_Notificacao,
                Numero_not = model.Numero_Notificacao,
                Codigo = model.Codigo_Imovel,
                Inscricao = model.Inscricao,
                Endereco_entrega = model.Endereco_Entrega,
                Endereco_entrega2 = model.Endereco_entrega2,
                Endereco_infracao = model.Endereco_Local,
                Endereco_prop = model.Endereco_Prop,
                Endereco_prop2 = model.Endereco_prop2,
                Prazo = model.Prazo,
                Nome = model.Nome_Proprietario,
                Situacao = 3,//concluido
                Userid = Convert.ToInt32(Session["hashid"]),
                Data_cadastro=DateTime.Now,
                Nome2 = model.Nome_Proprietario2,
                Codigo_cidadao=model.Codigo_cidadao,
                Codigo_cidadao2=model.Codigo_cidadao2,
                Cpf=model.Cpf,
                Cpf2=model.Cpf2,
                Rg=model.Rg,
                Rg2=model.Rg2
            };
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Exception ex = imovelRepository.Incluir_notificacao_terreno(reg);
            return null;
            
        }

        [Route("Notificacao_ter_query")]
        [HttpGet]
        public ActionResult Notificacao_ter_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Terreno_Struct> _listaNot = imovelRepository.Lista_Notificacao_Terreno(DateTime.Now.Year);
            foreach (Notificacao_Terreno_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = item.Nome_Proprietario,
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
        }

        [Route("Notificacao_ter_query")]
        [HttpPost]
        public ActionResult Notificacao_ter_query(NotificacaoTerQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Terreno_Struct> _listaNot = imovelRepository.Lista_Notificacao_Terreno(_ano);
            foreach (Notificacao_Terreno_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = item.Nome_Proprietario,
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;
            return View(model);
        }

        public ActionResult Notificacao_terreno_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            Notificacao_Terreno_Struct _not = imovelRepository.Retorna_Notificacao_Terreno(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumero = _not.AnoNumero,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Prazo = _not.Prazo,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                PrazoText = Functions.Escrever_Valor_Extenso(_not.Prazo),
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? ""
            };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
            }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Notificacao_Terreno.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Notificacao_Terreno.pdf");
            } catch {

                throw;
            }

        }

        [Route("CadImovelMnu")]
        [HttpGet]
        public ActionResult CadImovelMnu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            return View(model);
        }

        [Route("CadImovelMnu")]
        [HttpPost]
        public ActionResult CadImovelMnu(ImovelDetailsViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            int _codigo = Convert.ToInt32(model.Inscricao);
            bool _existe = imovelRepository.Existe_Imovel(_codigo);
            if (!_existe) {
                ViewBag.Result = "Código de imóvel não cadastrado.";
                return View(model);
            }
            string _codStr = Functions.Encrypt(_codigo.ToString());
            return RedirectToAction("CadImovel", new { c = _codStr });
        }

        [Route("CadImovelqryP")]
        [HttpGet]
        public ActionResult CadImovelqryP() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            model.Lista_Imovel = new List<ImovelStruct>();
            return View(model);
        }

        [Route("CadImovelqryP")]
        [HttpPost]
        public ActionResult CadImovelqryP(ImovelDetailsViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            model.Lista_Imovel = new List<ImovelStruct>();
            Imovel_bll imovelRepository = new Imovel_bll("GTIconnection");
            string _nome = model.NomeProprietario;
            if (_nome.Length < 5) {
                ViewBag.Result = "Digite ao menos 5 caracteres do nome.";
                return View(model);
            }
            List<ImovelStruct> ListaImovel = imovelRepository.Lista_Imovel_Proprietario(_nome);
            if (ListaImovel.Count == 0) {
                ViewBag.Result = "Não foi localizado nenhum imóvel com este proprietário.";
                return View(model);
            }

            List<ImovelStruct> _lista = new List<ImovelStruct>();
            foreach (ImovelStruct item in ListaImovel) {
                ImovelStruct reg = new ImovelStruct() {
                    Codigo = item.Codigo,
                    Proprietario_Nome= Functions.TruncateTo(  item.Proprietario_Nome,37),
                    NomeLogradouro=item.NomeLogradouro + ", " + item.Numero.ToString() + " " + item.Complemento,
                    Numero=item.Numero,
                    Complemento=item.Complemento
                };
                reg.NomeLogradouro = Functions.TruncateTo(reg.NomeLogradouro, 52);
                _lista.Add(reg);
            }

            model.Lista_Imovel = _lista;
            return View(model);
        }

    }
}
