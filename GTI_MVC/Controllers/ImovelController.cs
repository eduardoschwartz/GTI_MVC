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
using System.Net;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Mvc.Controllers {

    [Route("Imovel")]
    public class ImovelController : Controller {
        private readonly string _connection = "GTIconnection";
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
        public ActionResult Certidao_Endereco() {
            //Session["hashform"] = "5";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Certidao/Certidao_Endereco")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Certidao_Endereco(CertidaoViewModel model) {
            int _codigo = 0;
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.Endereco);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 100000)
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
            }

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            if (!_existeCod) {
                ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 5, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

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

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
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
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            //using (Bitmap bitmap = qrCode.GetGraphic(20)) {
            //    using (MemoryStream ms = new MemoryStream()) {
            //        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        byte[] byteImage = ms.ToArray();
            //        regCert.QRCodeImage = byteImage;
            //    }
            //}
            //#######################################################################

            regCert.QRCodeImage = Functions.Generate_QRCode(Code);
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
                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
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
        public ActionResult Certidao_Valor_Venal() {
            //Session["hashform"] = "6";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Certidao/Certidao_Valor_Venal")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Certidao_Valor_Venal(CertidaoViewModel model) {
            int _codigo;
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(TipoCertidao.ValorVenal);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            _codigo = Convert.ToInt32(model.Inscricao);
            if (_codigo < 100000)
                _existeCod = imovelRepository.Existe_Imovel(_codigo);

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            if (!_existeCod) {
                ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 6, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

            List<ProprietarioStruct> listaProp = imovelRepository.Lista_Proprietario(_codigo, true);
            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Certidao reg = new Certidao() {
                Codigo = _dados.Codigo,
                Inscricao = _dados.Inscricao,
                Endereco = _dados.NomeLogradouro,
                Endereco_Numero = (int)_dados.Numero,
                Endereco_Complemento = _dados.Complemento,
                //Bairro = _dados.NomeBairro ?? "",
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-VV",
                Data_Geracao = DateTime.Now
            };

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            Bairro _bairro = enderecoRepository.RetornaLogradouroBairro((int)_dados.CodigoLogradouro, (short)_dados.Numero);
            reg.Bairro = _bairro.Descbairro ?? "";


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
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(Code, QRCodeGenerator.ECCLevel.Q);
            //using (Bitmap bitmap = qrCode.GetGraphic(20)) {
            //    using (MemoryStream ms = new MemoryStream()) {
            //        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //        byte[] byteImage = ms.ToArray();
            //        cimp.QRCodeImage = byteImage;
            //    }
            //}
            //#######################################################################

            cimp.QRCodeImage = Functions.Generate_QRCode(Code);

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
            string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
            string _server = builder.DataSource;
            string _userId = builder.UserID;
            string _pwd = builder.Password;

            crConnectionInfo.ServerName = _server;
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
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
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
        public ActionResult Certidao_Isencao() {
            //Session["hashform"] = "7";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Certidao/Certidao_Isencao")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Certidao_Isencao(CertidaoViewModel model) {
            int _codigo = 0;
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
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

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
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

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 7, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

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
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
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
        [ValidateAntiForgeryToken]
        public ActionResult Dados_Imovel(ImovelDetailsViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _codigo = 0;
            bool _existeCod = false;
            ImovelDetailsViewModel imovelDetailsViewModel = new ImovelDetailsViewModel();

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secretKey,response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if(!status) {
                ViewBag.Result = "Código Recaptcha inválido.";
                return View(model);
            }

            if(model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 50000) {
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
                }
                if (model.CpfValue == null ) {
                    ViewBag.Result = "Cpf/Cnpj não informado.";
                    return View(imovelDetailsViewModel);
                }
                bool _bCpf = model.CpfValue.Length == 14 ? true : false;
                string _cpf = Functions.RetornaNumero(model.CpfValue);

                if(!_bCpf) {
//                    bool _valida = Functions.ValidaCNPJ(_cpf); //CNPJ válido?
//                    if (_valida) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, _cpf);
//                    } else {
//                        ViewBag.Result = "Cnpj inválido.";
//                        return View(imovelDetailsViewModel);
//                    }
                } else {
                    if (model.CpfValue != null) {
//                        bool _valida = Functions.ValidaCpf(_cpf); //CPF válido?
//                        if (_valida) {
                            _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, _cpf);
 //                       } else {
 //                           ViewBag.Result = "Cpf inválido.";
 //                           return View(imovelDetailsViewModel);
 //                       }
                    }
                }
                if (!_existeCod) {
                    ViewBag.Result = "Cpf/Cnpj não pertence a este imóvel.";
                    return View(imovelDetailsViewModel);
                }

            } else {
                ViewBag.Result = "Inscrição não informada.";
                return View(imovelDetailsViewModel);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 3, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

            Tributario_bll tributario_Class = new Tributario_bll(_connection);
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
        public ActionResult Carne_Iptu() {
            //Session["hashform"] = "2";
            //if (Session["hashid"] == null)
            //    return RedirectToAction("Login", "Home");

            CertidaoViewModel model = new CertidaoViewModel();
            return View(model);
        }

        [Route("Carne_Iptu")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Carne_Iptu(CertidaoViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _codigo = 0;
            int _ano = 2022;
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

                if(model.CpfValue == null) {
                    model.ErrorMessage = "Cpf/Cnpj não informado.";
                    return View(model);
                }

                bool _bCpf = model.CpfValue.Length == 14 ? true : false;
                string _cpf = Functions.RetornaNumero(model.CpfValue);
                if(!_bCpf) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo,_cpf);
                    if(!_existeCod) {
                        model.ErrorMessage = "Este Cnpj não pertence ao imóvel.";
                        return View(model);
                    }
                } else {
                        _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo,_cpf);
                    if(!_existeCod) {
                        model.ErrorMessage = "Este Cpf não pertence ao imóvel.";
                        return View(model);
                    }
                }
            } else {
                model.ErrorMessage = "Digite o Código do imóvel.";
                return View(model);
            }

            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                model.ErrorMessage = "Código Recaptcha inválido.";
                return View(model);
            }

            Tributario_bll tributario_Class = new Tributario_bll(_connection);
            List<AreaStruct> areas = imovelRepository.Lista_Area(_codigo);

            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            Laseriptu _calc = null;
            Laseriptu_ext _calc2 = imovelRepository.Dados_IPTU_Ext(_codigo, _ano);
            if (_calc2 == null)
                _calc = imovelRepository.Dados_IPTU(_codigo, _ano);
            else {
                _calc = new Laseriptu() { 
                    Agrupamento=_calc2.Agrupamento,
                    Aliquota= _calc2.Aliquota,
                    Areaconstrucao =_calc2.Areaconstrucao,
                    Areaterreno= _calc2.Areaterreno,
                    Codreduzido= _calc2.Codreduzido,
                    Fatorcat= _calc2.Fatorcat,
                    Fatordis= _calc2.Fatordis,
                    Fatorgle= _calc2.Fatorgle,
                    Fatorped= _calc2.Fatorped,
                    Fatorpro= _calc2.Fatorpro,
                    Fatorsit= _calc2.Fatorsit,
                    Fatortop= _calc2.Fatortop,
                    Fracaoideal= _calc2.Fracaoideal,
                    Impostopredial= _calc2.Impostopredial,
                    Impostoterritorial= _calc2.Impostoterritorial,
                    Natureza= _calc2.Natureza,
                    Qtdeparc= _calc2.Qtdeparc,
                    Testadaprinc= _calc2.Testadaprinc,
                    Valortotalparc= _calc2.Valortotalparc,
                    Valortotalunica= _calc2.Valortotalunica,
                    Valortotalunica2= _calc2.Valortotalunica2,
                    Valortotalunica3= _calc2.Valortotalunica3
                };
            }

            List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);

            List<DebitoStructure> Extrato_Lista = tributario_Class.Lista_Parcelas_IPTU(_codigo,_ano);
            if (Extrato_Lista.Count == 0 || (_calc==null && _calc2==null)) {
                model.ErrorMessage = "Não é possível emitir 2ª via de IPTU para este contribuinte.";
                return View(model);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"]==null?false:  Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null)  _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString() + ", exercício: " + DateTime.Now.Year.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 2, Pref = _prf ,Obs=_obs};
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************


            //*** Renumera parcelas de 2021 ***
            //if (DateTime.Now.Year == 2021) {
            //    int _seq = 0;
            //    foreach(DebitoStructure item in Extrato_Lista) {
            //        Extrato_Lista[_seq].Numero_Parcela_Old = (short)item.Numero_Parcela ;
            //        if(item.Numero_Parcela == 6) {
            //            Extrato_Lista[_seq].Numero_Parcela = 96;
            //        } else {
            //            if(item.Numero_Parcela == 7) {
            //                Extrato_Lista[_seq].Numero_Parcela = 97;
            //            } else {
            //                if(item.Numero_Parcela == 8) {
            //                    Extrato_Lista[_seq].Numero_Parcela = 98;
            //                } else {
            //                    if(item.Numero_Parcela == 9) {
            //                        Extrato_Lista[_seq].Numero_Parcela = 99;
            //                    }
            //                }
            //            }
            //        }
            //        _seq++;
            //    }
            //    int _idx = 0;
            //    foreach(DebitoStructure item in Extrato_Lista) {
                    
            //        if(item.Numero_Parcela == 96) {
            //            Extrato_Lista[_idx].Numero_Parcela = 8;
            //        } else {
            //            if(item.Numero_Parcela == 97) {
            //                Extrato_Lista[_idx].Numero_Parcela = 9;
            //            } else {
            //                if(item.Numero_Parcela == 98) {
            //                    Extrato_Lista[_idx].Numero_Parcela =6;
            //                } else {
            //                    if(item.Numero_Parcela == 99) {
            //                        Extrato_Lista[_idx].Numero_Parcela = 7;
            //                    }
            //                }
            //            }
            //        }
            //        _idx++;
            //    }
            //    Extrato_Lista = Extrato_Lista.OrderBy(o => o.Data_Vencimento).ThenBy(o => o.Numero_Parcela).ToList();
            //}
            //**************************************************


            //***  Novos Documento ******
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            int _idx2 = 0;
            foreach(DebitoStructure item in Extrato_Lista) {

                //grava o documento
                Numdocumento docReg = new Numdocumento() {
                    Datadocumento = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy")),
                    Emissor = "WebDam",
                    Registrado = true,
                    Valorguia = item.Soma_Total
                };
                int _documento = tributarioRepository.Insert_Documento(docReg);

                //parcela x documento
                Parceladocumento parcReg = new Parceladocumento() {
                    Codreduzido = item.Codigo_Reduzido,
                    Anoexercicio = (short)item.Ano_Exercicio,
                    Codlancamento = (short)item.Codigo_Lancamento,
                    Seqlancamento = (short)item.Sequencia_Lancamento,
                    Numparcela = (byte)item.Numero_Parcela,
                    Codcomplemento = (byte)item.Complemento,
                    Plano = 0,
                    Numdocumento = _documento
                };
                Exception ex = tributarioRepository.Insert_Parcela_Documento(parcReg);

                Extrato_Lista[_idx2].Numero_Documento = _documento;

                _idx2++;
            }

            //**************************************************


            string _msg ="" ;
            List<Boletoguia> ListaBoleto = new List<Boletoguia>();
            foreach (DebitoStructure item in Extrato_Lista) {
                if (item.Numero_Parcela > 0) {
                    _msg = "Após o vencimento tirar 2ª via no site da prefeitura www.jaboticabal.sp.gov.br";
                    //if(DateTime.Now.Year == 2021 && item.Numero_Parcela == 6) {
                    //    _msg += Environment.NewLine + "Referente a parcela original 8/12";
                    //}
                    //if(DateTime.Now.Year == 2021 && item.Numero_Parcela == 7) {
                    //    _msg += Environment.NewLine + "Referente a parcela original 9/12";
                    //}
                    //if(DateTime.Now.Year == 2021 && item.Numero_Parcela == 8) {
                    //    _msg += Environment.NewLine + "Referente a parcela original 6/12";
                    //}
                    //if(DateTime.Now.Year == 2021 && item.Numero_Parcela == 9) {
                    //    _msg += Environment.NewLine + "Referente a parcela original 7/12";
                    //}

                }

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


                //Registrar os novos documentos
                Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                    Nome = reg.Nome.Length > 40 ? reg.Nome.Substring(0,40) : reg.Nome,
                    Endereco = reg.Endereco.Length > 40 ? reg.Endereco.Substring(0,40) : reg.Endereco,
                    Bairro = reg.Bairro.Length > 15 ? reg.Bairro.Substring(0,15) : reg.Bairro,
                    Cidade = reg.Cidade.Length > 30 ? reg.Cidade.Substring(0,30) : reg.Cidade,
                    Cep = reg.Cep ?? "14870000",
                    Cpf = reg.Cpf,
                    Numero_documento = Convert.ToInt32( item.Numero_Documento),
                    Data_vencimento = Convert.ToDateTime(item.Data_Vencimento),
                    Valor_documento = Convert.ToDecimal(item.Soma_Principal),
                    Uf = reg.Uf??"SP"
                };
                if(item.Data_Vencimento >= Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"))) {
                    Exception ex = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
                    if(ex == null)
                        ex = tributarioRepository.Marcar_Documento_Registrado(Convert.ToInt32( item.Numero_Documento));
                }

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

                //Laseriptu RegIPTU = tributario_Class.Carrega_Dados_IPTU(Convert.ToInt32(ListaBoleto[0].Codreduzido), _ano);
                //Laseriptu_ext RegIPTU_Ext = null;
                //if(RegIPTU==null)
                //    RegIPTU_Ext = tributario_Class.Carrega_Dados_IPTU_Ext(Convert.ToInt32(ListaBoleto[0].Codreduzido), _ano);


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
                parameters.Add(new ReportParameter("FRACAO", Convert.ToDecimal(_calc.Fracaoideal).ToString("#0.00")));
                parameters.Add(new ReportParameter("NATUREZA", _calc.Natureza));
                parameters.Add(new ReportParameter("TESTADA", Convert.ToDecimal(_calc.Testadaprinc).ToString("#0.00")));
                parameters.Add(new ReportParameter("AREAT", Convert.ToDecimal(_calc.Areaterreno).ToString("#0.00")));
                parameters.Add(new ReportParameter("AREAC", Convert.ToDecimal(_calc.Areaconstrucao).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVT", Convert.ToDecimal(_calc.Vvt).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVC", Convert.ToDecimal(_calc.Vvc).ToString("#0.00")));
                parameters.Add(new ReportParameter("VVI", Convert.ToDecimal(_calc.Vvi).ToString("#0.00")));
                parameters.Add(new ReportParameter("IPTU", Convert.ToDecimal(_calc.Impostopredial).ToString("#0.00")));
                parameters.Add(new ReportParameter("ITU", Convert.ToDecimal(_calc.Impostoterritorial).ToString("#0.00")));
                parameters.Add(new ReportParameter("TOTALPARC", Convert.ToDecimal((_calc.Qtdeparc) * ListaBoleto[ListaBoleto.Count-1].Valorguia).ToString("#0.00")));
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
        public ActionResult Carne_Cip() {
            return null;
            //CertidaoViewModel model = new CertidaoViewModel();
            //return View(model);
        }

        [Route("Carne_Cip")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Carne_Cip(CertidaoViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _codigo = 0;
            bool _existeCod = false;
            ImovelDetailsViewModel imovelDetailsViewModel = new ImovelDetailsViewModel();


            var response = Request["g-recaptcha-response"];
            string secretKey = "6LfRjG0aAAAAACH5nVGFkotzXTQW_V8qpKzUTqZV";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            string msg = status ? "Sucesso" : "Falha";
            if (!status) {
                imovelDetailsViewModel.ErrorMessage = "Código Recaptcha inválido.";
                return View(model);
            }

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 50000) {
                    _existeCod = imovelRepository.Existe_Imovel(_codigo);
                }
                if (!_existeCod) {
                    model.ErrorMessage = "Imóvel não cadastrado.";
                    return View(model);
                } else {
                    bool _bCpf = model.CpfValue.Length == 14 ? true : false;
                    string _cpf = Functions.RetornaNumero(model.CpfValue);
                    if (!_bCpf) {
                        _existeCod = imovelRepository.Existe_Imovel_Cnpj(_codigo, _cpf);
                        if (!_existeCod) {
                            model.ErrorMessage = "Este Cnpj não pertence ao imóvel.";
                            return View(model);
                        }
                    } else {
                        _existeCod = imovelRepository.Existe_Imovel_Cpf(_codigo, _cpf);
                        if (!_existeCod) {
                            model.ErrorMessage = "Este Cpf não pertence ao imóvel.";
                            return View(model);
                        }
                    }
                }
            } else {
                model.ErrorMessage = "Digite o código do imóvel.";
                return View(model);
            }

            Tributario_bll tributario_Class = new Tributario_bll(_connection);
            List<AreaStruct> areas = imovelRepository.Lista_Area(_codigo);

            ImovelStruct _dados = imovelRepository.Dados_Imovel(_codigo);
            List<ProprietarioStruct> _prop = imovelRepository.Lista_Proprietario(_codigo, true);

            List<DebitoStructure> Extrato_Lista = tributario_Class.Lista_Parcelas_CIP(_codigo, DateTime.Now.Year);
            if (Extrato_Lista.Count == 0) {
                imovelDetailsViewModel.ErrorMessage = "Não é possível emitir 2ª via da CIP para este contribuinte.";
                return View(imovelDetailsViewModel);
            }

            //**** log ****************
            int _userid = 2;
            bool _prf = Session["hashfunc"] == null ? false : Session["hashfunc"].ToString() == "S" ? true : false;
            if (Session["hashid"] != null) _userid = Convert.ToInt32(Session["hashid"]);
            string _obs = "Imóvel código: " + _codigo.ToString() + ", exercício: " + DateTime.Now.Year.ToString();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 4, Pref = _prf, Obs = _obs };
            sistemaRepository.Incluir_LogWeb(regWeb);
            //*************************

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
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
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


        [Route("CadImovelqryC")]
        [HttpGet]
        public ActionResult CadImovelqryC(string id) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            if (string.IsNullOrEmpty(id))
                model.Lista_Imovel = new List<ImovelLista>();
            else {
                return RedirectToAction("CadImovel", new { c = id.Replace('-', '/') });
            }
            return View(model);
        }

        [Route("CadImovelqryC")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadImovelqryC(ImovelDetailsViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            model.Lista_Imovel = new List<ImovelLista>();
            int _codigo = Convert.ToInt32(model.Codigo);
            string _partialName = model.NomeProprietario ?? "";
            if (_partialName != "") {
                if (_partialName.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do nome do proprietário.";
                    return View(model);
                }
            }
            string _partialEndereco = model.NomeEndereco ?? "";
            if (_partialEndereco != "") {
                if (_partialEndereco.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do endereço.";
                    return View(model);
                }
            }
            int _numero = model.Numero==null?0:Convert.ToInt32(model.Numero);
            int _distrito = 0, _setor = 0, _quadra = 0, _lote = 0, _face = 0, _unidade = 0, _subunidade = 0;
            string _insc = model.Inscricao ?? "";

            if (_insc != "" ) {
                if (_insc.Length == 25) {
                    _insc = Functions.RetornaNumero(model.Inscricao);
                    _distrito = Convert.ToInt32(_insc.Substring(0, 1));
                    _setor = Convert.ToInt32(_insc.Substring(1, 2));
                    _quadra = Convert.ToInt32(_insc.Substring(3, 4));
                    _lote = Convert.ToInt32(_insc.Substring(7, 5));
                    _face = Convert.ToInt32(_insc.Substring(12, 2));
                    _unidade = Convert.ToInt32(_insc.Substring(14, 2));
                    _subunidade = Convert.ToInt32(_insc.Substring(16, 3));
                } else {
                    ViewBag.Result = "Inscrição cadastral inválida.";
                    return View(model);
                }
            }

            if(_codigo==0 && _insc=="" && _partialEndereco=="" && _partialName == "") {
                ViewBag.Result = "Selecione ao menos um critério de busca.";
                return View(model);
            }

            List<ImovelStruct> ListaImovel = imovelRepository.Lista_Imovel(_codigo, _distrito, _setor, _quadra, _lote, _face, _unidade, _subunidade, _partialName,_partialEndereco,_numero);
            if (ListaImovel.Count == 0) {
                ViewBag.Result = "Não foi localizado nenhum imóvel com este(s) critério(s).";
                return View(model);
            }
            List<ImovelLista> _lista = new List<ImovelLista>();
            foreach (ImovelStruct item in ListaImovel) {
                ImovelLista reg = new ImovelLista() {
                    Codigo = item.Codigo.ToString("00000"),
                    Nome = Functions.TruncateTo(item.Proprietario_Nome, 30),
                    Endereco = string.IsNullOrEmpty(item.NomeLogradouroAbreviado) ? item.NomeLogradouro : item.NomeLogradouroAbreviado
                };
                reg.Endereco += ", " + item.Numero.ToString() + " " + item.Complemento;
                reg.Endereco = Functions.TruncateTo(reg.Endereco, 52);
                _lista.Add(reg);
            }

            model.Lista_Imovel = _lista;
            return View(model);

        }

    }
}

