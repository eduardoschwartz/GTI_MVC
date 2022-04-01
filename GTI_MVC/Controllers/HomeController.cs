using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using static GTI_Models.modelCore;
using Newtonsoft.Json.Linq;
using System.Web;
using System.IO;
using System.Runtime.InteropServices;
using static GTI_MVC.Controllers.ProcessoController;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {
        private readonly string _connection = "GTIconnection";
        [Route("Login_gti")]
        [HttpGet]
        public ActionResult Login_gti(string c) {
            LoginViewModel model = new LoginViewModel();
            if (Session["hashid"] == null) {
                Session.Remove("hashfname");
            }
           
            ViewBag.FiscalMov = Session["hashfiscalmov"] == null ? "N" : Session["hashfiscalmov"].ToString();
            return View(model);
        }


        [Route("Certidao")]
        public ViewResult Certidao() {
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public ViewResult Login() {
            LoginViewModel model = new LoginViewModel();
            if (Session["hashid"]!= null ) {
                int _userid = Convert.ToInt32(Session["hashid"]);
                bool _func = Session["hashfunc"].ToString() == "S" ? true : false;
                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
                List<int> ListaUsoPlataforma = tributarioRepository.Lista_Rodo_Uso_Plataforma_UserEmpresa(_userid, _func);
                if (ListaUsoPlataforma.Count == 0) {
                    ViewBag.UsoPlataforma = "N";
                } else {
                    ViewBag.UsoPlataforma = "S";
                }
                return View("SysMenu");
            } else {
                if (Request.Cookies["2lG*"] != null) {
                    model.RememberMe = true;
                    model.Usuario = Functions.Decrypt( Request.Cookies["2lG*"].Value.ToString());
                } 
                if (Request.Cookies["2pW*"] != null) {
                    model.RememberMe = true;
                    model.Senha = Functions.Decrypt( Request.Cookies["2pW*"].Value.ToString());
                }

            }
            Session.Remove("hashid");
            Session.Remove("hashfname");
            return View(model);
        }

        [Route("Logout")]
        [HttpGet]
        public ViewResult Logout() {
            Session.Remove("hashfname");
            Session.Remove("hashlname");
            Session.Remove("hashid");
            return View("Login_gti");
        }

        [Route("SysMenu")]
        [HttpGet]
        public ViewResult SysMenu() {

            if (Session["hashid"] == null) {
                LoginViewModel model = new LoginViewModel();
                return View("Login",model);
            } else {
                int _userid = Convert.ToInt32(Session["hashid"]);
                bool _func = Session["hashfunc"].ToString() == "S" ? true : false;
                Tributario_bll tributarioRepository = new Tributario_bll(_connection);
                List<int> ListaUsoPlataforma = tributarioRepository.Lista_Rodo_Uso_Plataforma_UserEmpresa(_userid, _func);
                if (ListaUsoPlataforma.Count == 0) {
                    ViewBag.UsoPlataforma = "N";
                } else {
                    ViewBag.UsoPlataforma = "S";
                }

                return View();
            }
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model) {
            string sLogin = model.Usuario.Trim(), sNewPwd = model.Senha, sOldPwd, sOldPwd2, sName;
            LoginViewModel loginViewModel = new LoginViewModel();

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            TAcessoFunction tacesso_Class = new TAcessoFunction();

            bool bFuncionario = model.Usuario.LastIndexOf('@') > 1 ? false : true;
            Session["hashfunc"] = bFuncionario ? "S" : "N";

            var cookieF = new HttpCookie("2fN*", Functions.Encrypt(bFuncionario ? "S" : "N"));
            cookieF.Expires = DateTime.Now.AddHours(1);
            System.Web.HttpContext.Current.Response.Cookies.Add(cookieF);


            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Form_Redirect fr = new Form_Redirect();

            if (bFuncionario) {
                sOldPwd = sistemaRepository.Retorna_User_Password(sLogin);
                int UserId = sistemaRepository.Retorna_User_LoginId(sLogin);
                if (sOldPwd == null) {
                    Session.Remove("hashid");
                    ViewBag.Result = "Usuário/Senha inválido!";
                    return View(loginViewModel);
                } else {
                    sOldPwd2 = tacesso_Class.DecryptGTI(sOldPwd);
                    if (sOldPwd2 != sNewPwd) {
                        ViewBag.Result = "Usuário/Senha inválido!";
                        Session.Remove("hashid");
                        return View(loginViewModel);
                    } else {
                        ViewBag.Result = "";
                        Session["hashid"] = UserId;
                    }
                }
                
                if (UserId == 0) {
                    ViewBag.Result = "Usuário/Senha inválido.";
                    return View(loginViewModel);
                }

                usuarioStruct _user = sistemaRepository.Retorna_Usuario(UserId);
                if (_user.Ativo == 0) {
                    ViewBag.Result = "Usuário inativo.";
                    return View(loginViewModel);
                } else {
                    Session["hashlname"] = _user.Nome_login;
                    Session["hashfname"] = _user.Nome_completo;
                    Session["hashfiscalitbi"] = _user.Fiscal_Itbi ? "S" : "N";
                    Session["hashfiscalpostura"] = _user.Fiscal_postura ? "S" : "N";
                    Session["hashfiscalmov"] = _user.Fiscal_mov ? "S" : "N";
                    Session["hashfiscal"] = _user.Fiscal ? "S" : "N";
                    if (Session["hashid"] == null) {
                        Session.Add("hashid", _user.Id);
                        Session.Add("hashfname", _user.Nome_completo);
                        Session.Add("hashlname", _user.Nome_login);
                        Session.Add("hashfiscalitbi", "N");
                        Session.Add("hashfiscalpostura", "N");
                        Session.Add("hashfiscalmov", "N");
                        Session.Add("hashfiscal", "N");
                        Session.Add("hashfunc", "N");
                    }
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    bool _func = Session["hashfunc"].ToString() == "S" ? true : false;

                    //log 
                    //LogWeb regWeb = new LogWeb() {UserId=_userid,Evento=1,Pref=true};
                    //sistemaRepository.Incluir_LogWeb(regWeb);
                    //***

                    List<int> ListaUsoPlataforma = tributarioRepository.Lista_Rodo_Uso_Plataforma_UserEmpresa(_userid, _func);
                    if (ListaUsoPlataforma.Count == 0) {
                        ViewBag.UsoPlataforma = "N";
                    } else {
                        ViewBag.UsoPlataforma = "S";
                    }

                    // **Rememeber me
                    if (model.RememberMe) {
                        var cookie = new HttpCookie("2lG*", Functions.Encrypt( model.Usuario));
                        cookie.Expires = DateTime.Now.AddDays(30);
                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                        
                        cookie = new HttpCookie("2pW*", Functions.Encrypt( model.Senha));
                        cookie.Expires = DateTime.Now.AddDays(30);
                        System.Web.HttpContext.Current.Response.Cookies.Add( cookie);
                    }
                    else{
                        Response.Cookies["2lG*"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["2pW*"].Expires = DateTime.Now.AddDays(-1);
                    }
                    //******************
                    var cookie2 = new HttpCookie("2lG1H*", Functions.Encrypt(model.Usuario));
                    cookie2.Expires = DateTime.Now.AddHours(1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie2);
                    var cookie3 = new HttpCookie("2uC*", Functions.Encrypt(_userid.ToString()));
                    cookie3.Expires = DateTime.Now.AddHours(1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie3);

                    if (Session["hashform"] == null) {
                        return View("../Home/SysMenu");
                    } else {
                        fr= RedirectToForm(Session["hashform"].ToString());
                    }
                }
            } else {
                Usuario_web user_web = sistemaRepository.Retorna_Usuario_Web(model.Usuario);
                if (user_web == null) {
                    ViewBag.Result = "Usuário/Senha inválido.";
                    return View(loginViewModel);
                } else {
                    if (model.Senha != Functions.Decrypt(user_web.Senha)) {
                        ViewBag.Result = "Usuário/Senha inválido.";
                        return View(loginViewModel);
                    } else {
                        if (!user_web.Ativo) {
                            ViewBag.Result = "Esta conta encontra-se inativa.";
                            return View(loginViewModel);
                        } else {
                            if (user_web.Bloqueado) {
                                ViewBag.Result = "Esta conta encontra-se bloqueada.";
                                return View(loginViewModel);
                            } else {
                                Session["hashid"] = user_web.Id;
                                Session["hashlname"] = user_web.Email;
                                Session["hashfname"] = user_web.Nome;
                                Session.Add("hashfiscalitbi", "N");
                                Session.Add("hashfiscal", "N");
                                Session.Add("hashfunc", "N");
                                int _userid = Convert.ToInt32(Session["hashid"]);
                                bool _func = Session["hashfunc"].ToString() == "S" ? true : false;

                                //log 
                                //LogWeb regWeb = new LogWeb() { UserId = _userid, Evento = 1, Pref = false };
                                //sistemaRepository.Incluir_LogWeb(regWeb);
                                //***
                                // **Rememeber me
                                if (model.RememberMe) {
                                    var cookie = new HttpCookie("2lG*", Functions.Encrypt(model.Usuario));
                                    cookie.Expires = DateTime.Now.AddDays(30);
                                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);

                                    cookie = new HttpCookie("2pW*", Functions.Encrypt(model.Senha));
                                    cookie.Expires = DateTime.Now.AddDays(30);
                                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                                } else {
                                    Response.Cookies["2lG*"].Expires = DateTime.Now.AddDays(-1);
                                    Response.Cookies["2pW*"].Expires = DateTime.Now.AddDays(-1);
                                }
                                //******************
                                var cookie2 = new HttpCookie("2lG1H*", Functions.Encrypt(model.Usuario));
                                cookie2.Expires = DateTime.Now.AddDays(1);
                                System.Web.HttpContext.Current.Response.Cookies.Add(cookie2);
                                var cookie3 = new HttpCookie("2uC*", Functions.Encrypt(user_web.Id.ToString()));
                                cookie3.Expires = DateTime.Now.AddHours(1);
                                System.Web.HttpContext.Current.Response.Cookies.Add(cookie3);



                                List<int> ListaUsoPlataforma = tributarioRepository.Lista_Rodo_Uso_Plataforma_UserEmpresa(_userid, _func);
                                if (ListaUsoPlataforma.Count == 0) {
                                    ViewBag.UsoPlataforma = "N";
                                } else {
                                    ViewBag.UsoPlataforma = "S";
                                }
                                if (Session["hashform"] == null) {
                                    return View("../Home/SysMenu");
                                } else {
                                    fr = RedirectToForm(Session["hashform"].ToString());
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction(fr.Action,fr.Controller);
        }

        [Route("Login_update")]
        [HttpGet]
        public ViewResult Login_update() {
            if (Session["hashid"] == null)
                return View("Login");
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Login_update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Login_update(LoginViewModel model) {
            int _id=0;
            Sistema_bll sistema_Class = new Sistema_bll(_connection);
            string sLogin = Session["hashfname"].ToString();
            TAcessoFunction tacesso_Class = new TAcessoFunction();
            string _oldPwd = "";
            if (Session["hashfunc"].ToString() == "S")
                _oldPwd = tacesso_Class.DecryptGTI(sistema_Class.Retorna_User_Password(Session["hashlname"].ToString()));
            else {
                Usuario_web user = sistema_Class.Retorna_Usuario_Web(Session["hashlname"].ToString());
                _id = user.Id;
                _oldPwd = Functions.Decrypt(user.Senha);
            }


            if (model.Senha != _oldPwd) {
                ViewBag.Result = "Senha atual não confere!";
            } else {
                if (Session["hashfunc"].ToString() == "N") {
                    Exception ex = sistema_Class.Alterar_Usuario_Web_Senha(_id, Functions.Encrypt( model.Senha2));
                    if (ex != null) {
                        ViewBag.Result = "Erro, senha não alterada";
                    } else {
                        ViewBag.Result = "Sua senha foi alterada, por favor efetue login novamente";
                        return View("sysMenu");
                    }
                } else {
                    Usuario reg = new Usuario {
                        Nomelogin = Session["hashlname"].ToString(),
                        Senha = tacesso_Class.Encrypt128(model.Senha2)
                    };
                    Exception ex = sistema_Class.Alterar_Senha(reg);
                    if (ex != null) {
                        ViewBag.Result = "Erro, senha não alterada";
                    } else {
                        ViewBag.Result = "Sua senha foi alterada, por favor efetue login novamente";
                    }
                }
            }
            return View(model);
        }

        [Route("Findcd")]
        [HttpGet]
        public ViewResult Findcd() {
            DebitoViewModel model = new DebitoViewModel();
            return View(model);
        }

        [Route("Findcd")]
        [HttpPost]
        public ActionResult Findcd(DebitoViewModel model,string action) {
            TipoCadastro tipo = model.Cadastro == "Imóvel" ? TipoCadastro.Imovel : model.Cadastro == "Empresa" ? TipoCadastro.Empresa : TipoCadastro.Cidadao;
            string cpf = model.CpfValue == null ? "" : Functions.RetornaNumero(model.CpfValue);
            string cnpj = model.CnpjValue == null ? "" : Functions.RetornaNumero(model.CnpjValue);
            string name =  model.Nome==null?"":  model.Nome.Trim();

            Sistema_bll sistemaClass = new Sistema_bll(_connection);
            model.Lista_Header=sistemaClass.CodigoHeader(tipo, cpf, cnpj, name);
            if (model.Lista_Header.Count == 0)
                ViewBag.Erro = "Nenhum contribuinte localizado.";
            return View( model);
        }

        [Route("Login_create")]
        [HttpGet]
        public ViewResult Login_create() {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Login_create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_create(LoginViewModel model) {

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

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);

            if (sistemaRepository.Existe_Usuario_Web(model.Email)) {
                ViewBag.Result = "Este email já esta cadastrado.";
                return View(model);
            }

            Usuario_web reg = new Usuario_web() {
                Nome = model.Usuario,
                Email = model.Email,
                Telefone = model.Telefone,
                Cpf_Cnpj =  Functions.RetornaNumero(model.CpfValue),
                Senha =Functions.Encrypt(model.Senha2),
                Data_Cadastro=DateTime.Now,
                Ativo=false,
                Bloqueado=false
            };
            int id = sistemaRepository.Incluir_Usuario_Web(reg);

            string sid =  Url.Encode( Functions.Encrypt("#GTI - Serviços Online#"+id.ToString("000000")));
            string Body = System.IO.File.ReadAllText( System.Web.HttpContext.Current.Server.MapPath("~/Files/AccessTemplate.htm"));
            Body = Body.Replace("#$$$#", sid);
            using (MailMessage emailMessage = new MailMessage()) {
                emailMessage.From = new MailAddress("gti@jaboticabal.sp.gov.br", "Prefeitura de Jaboticabal");
                emailMessage.To.Add(new MailAddress(model.Email));
                emailMessage.Subject = "Prefeitura Municipal de Jaboticabal - Acesso aos serviços online (G.T.I.)";
                emailMessage.Body = Body;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.IsBodyHtml = true;

                using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587)) {
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
                    MailClient.Send(emailMessage);
                }
            }

            return View("Login_create2", model);
        }

        [Route("Login_create2")]
        [HttpGet]
        public ViewResult Login_create2(LoginViewModel model) {
            return View(model);
        }

        [Route("Login_create2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_create2() {
            return RedirectToAction("Login");
        }

        [Route("Login_welcome")]
        [HttpGet]
        public ActionResult Login_welcome(string c) {
            if (c == null) {
                return RedirectToAction("Login");
            }
            string p = Functions.Decrypt( c);
            if (p == "") {
                return RedirectToAction("Login");
            }
            if (p.Substring(0, 4) == "#GTI") {
                int Id =  Convert.ToInt32(Functions.StringRight(p, 7).Substring(1,6));
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                Exception ex = sistemaRepository.Ativar_Usuario_Web(Id);
                return View();
            } else {
                return RedirectToAction("Login");
            }
        }

        [Route("Login_welcome")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_welcome() {
            return RedirectToAction("Login");
        }

        [Route("Login_resend")]
        [HttpGet]
        public ViewResult Login_resend() {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Login_resend")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Login_resend(LoginViewModel model) {
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

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(model.Email);
            if (reg == null) {
                ViewBag.Result = "Erro, email não cadastrado.";
                return View(model);
            } else {
                if (reg.Bloqueado) {
                    ViewBag.Result = "Erro, este endereço de email encontra-se bloqueado.";
                    return View(model);
                } else {
                    if (reg.Ativo) {
                        ViewBag.Result = "Erro, este endereço de email já foi ativado.";
                        return View(model);
                    }
                }
            }

            string sid = Url.Encode(Functions.Encrypt("#GTI - Serviços Online#" + reg.Id.ToString("000000")));
            string Body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Files/AccessTemplate.htm"));
            Body = Body.Replace("#$$$#", sid);
            using (MailMessage emailMessage = new MailMessage()) {
                emailMessage.From = new MailAddress("gti@jaboticabal.sp.gov.br", "Prefeitura de Jaboticabal");
                emailMessage.To.Add(new MailAddress(model.Email));
                emailMessage.Subject = "Prefeitura Municipal de Jaboticabal - Acesso aos serviços online (G.T.I.)";
                emailMessage.Body = Body;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.IsBodyHtml = true;

                using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587)) {
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
                    MailClient.Send(emailMessage);
                }
            }

            return View("Login_create2", model);
        }
        
        [Route("Login_resend_pwd")]
        [HttpGet]
        public ViewResult Login_resend_pwd() {
            LoginViewModel model = new LoginViewModel();
 
            return View();
        }

        [Route("Login_resend_pwd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewResult Login_resend_pwd(LoginViewModel model) {
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

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(model.Email);
            if (reg == null) {
                ViewBag.Result = "Erro, email não cadastrado.";
                return View(model);
            } else {
                if (reg.Bloqueado) {
                    ViewBag.Result = "Erro, este endereço de email encontra-se bloqueado.";
                    return View(model);
                } else {
                    if (!reg.Ativo) {
                        ViewBag.Result = "Erro, este endereço de email não foi ativado.";
                        return View(model);
                    }
                }
            }
            string sid = Url.Encode(Functions.Encrypt("#GTI - Serviços Online#" + reg.Id.ToString("000000")));
            string Body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Files/PwdTemplate.htm"));
                Body = Body.Replace("#email#", model.Email);
                Body = Body.Replace("#$$$#", sid);
            using (MailMessage emailMessage = new MailMessage()) {
                emailMessage.From = new MailAddress("gti@jaboticabal.sp.gov.br", "Prefeitura de Jaboticabal");
                emailMessage.To.Add(new MailAddress(model.Email));
                emailMessage.Subject = "Prefeitura Municipal de Jaboticabal - Acesso aos serviços online (G.T.I.)";
                emailMessage.Body = Body;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.IsBodyHtml = true;

                using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587)) {
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
                    MailClient.Send(emailMessage);
                }
            }

            return View("Login_create2", model);
        }

        [Route("Login_reset")]
        [HttpGet]
        public ActionResult Login_reset(string c) {
            if (c == null) {
                return RedirectToAction("Login");
            }
            string p = Functions.Decrypt(c);
            if (p == "") {
                return RedirectToAction("Login");
            }
            int Id = Convert.ToInt32(Functions.StringRight(p, 7).Substring(1, 6));
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(Id);
            LoginViewModel model = new LoginViewModel() {
                Email = reg.Email
            };
            return View(model);
        }

        [Route("Login_reset")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_reset(LoginViewModel model) {
            //if (Session["hashid"] == null) {
            //    Functions.pUserFullName = "Visitante";
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

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(model.Email);
            int Id = reg.Id;
            Exception ex = sistemaRepository.Alterar_Usuario_Web_Senha(Id, model.Senha);

            ViewBag.Message = "A senha foi alterar com sucesso.";

            return View(model);
        }

        Form_Redirect RedirectToForm(string _name) {
            ViewBag.Fiscal = Session["hashfiscalmov"] == null ? "N" : Session["hashfiscalmov"].ToString();
            Form_Redirect _fr = new Form_Redirect();
            Session["hashform"] = "";
            switch (_name) {
                case "mobreq":
                    _fr.Controller = "MobReq"; _fr.Action = "Mobreq_menu";
                    break;
                case "itbi":
                    _fr.Controller = "Itbi"; _fr.Action = "Itbi_menu";
                    break;
                case "parc":
                    _fr.Controller = "Parcelamento"; _fr.Action = "Parc_index";
                    break;
                case "2":
                    _fr.Controller = "Imovel"; _fr.Action = "Carne_Iptu";
                    break;
                case "3":
                    _fr.Controller = "Imovel"; _fr.Action = "Carne_Cip";
                    break;
                case "5":
                    _fr.Controller = "Imovel"; _fr.Action = "Certidao_Endereco";
                    break;
                case "6":
                    _fr.Controller = "Imovel"; _fr.Action = "Certidao_Valor_Venal";
                    break;
                case "7":
                    _fr.Controller = "Imovel"; _fr.Action = "Certidao_Isencao";
                    break;
                case "10":
                    _fr.Controller = "Tributario"; _fr.Action = "Dama";
                    break;
                case "11":
                    _fr.Controller = "Empresa"; _fr.Action = "Carne_tl";
                    break;
                case "12":
                    _fr.Controller = "Empresa"; _fr.Action = "Carne_vs";
                    break;
                case "13":
                    _fr.Controller = "Empresa"; _fr.Action = "Details";
                    break;
                case "14":
                    _fr.Controller = "Empresa"; _fr.Action = "Alvara_Funcionamento";
                    break;
                case "15":
                    _fr.Controller = "Empresa"; _fr.Action = "Certidao_Inscricao";
                    break;
                case "16":
                    _fr.Controller = "Itbi"; _fr.Action = "Itbi_isencao";
                    break;
                case "17":
                    _fr.Controller = "Empresa"; _fr.Action = "Certidao_Pagamento";
                    break;
                case "18":
                    _fr.Controller = "Tributario"; _fr.Action = "Certidao_Debito_Codigo";
                    break;
                case "19":
                    _fr.Controller = "Tributario"; _fr.Action = "Certidao_Debito_Doc";
                    break;
                case "20":
                    _fr.Controller = "Tributario"; _fr.Action = "Comprovante_Pagamento";
                    break;
                case "21":
                    _fr.Controller = "Tributario"; _fr.Action = "Detalhe_Boleto";
                    break;
                case "22":
                    _fr.Controller = "Protocolo"; _fr.Action = "Consulta_Processo";
                    break;
                case "23":
                    _fr.Controller = "Tributario"; _fr.Action = "SegundaVia_Parcelamento";
                    break;
                default:
                    _fr.Controller = "Home"; _fr.Action = "SysMenu";
                    break;
            }
            return _fr;
        }

        [Route("User_Query")]
        [HttpGet]
        public ActionResult User_Query() {
            LoginViewModel model = new LoginViewModel();
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            List<Usuario_web> Lista = new List<Usuario_web>();

            model.Lista_Usuario_Web = Lista;
            return View(model);
        }


        [Route("User_Query")]
        [HttpPost]
        public ActionResult User_Query(LoginViewModel model,int? ide,string tp,  string action) {

            List<Usuario_web> Lista = new List<Usuario_web>();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);

            if (action == "rs") {

                //Usuario_web _user = sistemaRepository.Retorna_Usuario_Web((int)ide);
                //if (!_user.Ativo) {
                //    ViewBag.Result = "A conta não esta ativa";
                //    return View(model);
                //} else {
                    Exception ex = sistemaRepository.Alterar_Usuario_Web_Senha((int)ide, Functions.Encrypt("123456"));
                //    ViewBag.Result = "A senha foi reseta com sucesso";
                //}
                model.Lista_Usuario_Web = Lista;
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);

            }

            if (!string.IsNullOrEmpty(model.Filter))
                Lista = sistemaRepository.Lista_Usuario_Web(model.Filter);

            model.Lista_Usuario_Web = Lista;
//            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            return View(model);
        }

        [Route("User_doc")]
        [HttpGet]
        public ActionResult User_doc() {
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login");

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            int _userId = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));

            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(_userId);
           
            model.UserId = _userId;
            model.Usuario = reg.Nome;
            model.CpfCnpjLabel = Functions.FormatarCpfCnpj(reg.Cpf_Cnpj);
            bool _fisica = reg.Cpf_Cnpj.Length == 11 ? true : false;

            model.Lista_Usuario_Web_Anexo = sistemaRepository.Lista_Usuario_Web_Tipo_Anexo(_userId, _fisica);
            int _pos = 0;
            foreach (Usuario_Web_Anexo_Struct _anexo in model.Lista_Usuario_Web_Anexo) {
                Usuario_web_anexo _reg = sistemaRepository.Retorna_Web_Anexo(_userId, _anexo.Codigo);
                if (_reg != null) {
                    model.Lista_Usuario_Web_Anexo[_pos].Arquivo = Functions.TruncateTo( _reg.Arquivo,32);
                }
                _pos++;
            }

            return View(model);
        }

        public JsonResult Inserir_Anexo(string Seq,string Id) {
            if (Request.Files.Count > 0) {
                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                foreach (string file in Request.Files) {
                    var _file = Request.Files[file];
                    Usuario_web_anexo reg = new Usuario_web_anexo() {
                        Userid = Convert.ToInt32(Id),
                        Tipo = Convert.ToInt16(Seq),
                        Arquivo = _file.FileName
                    };
                    Exception ex = sistemaRepository.Incluir_Usuario_Web_Anexo(reg);

                    //Salva cópia do Anexo
                    string fileName = "";
                    string _cod = Convert.ToInt32(Id).ToString("00000");
                    string _path = "~/Files/UserDoc/" + _cod;
                    Directory.CreateDirectory(Server.MapPath(_path));
                    fileName = _file.FileName;
                    var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path), fileName);
                    _file.SaveAs(path);
                    break;
                }
            }
            return Json(new { success = true, responseText = "Arquivo anexado com sucesso." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Remove_Anexo(string Seq, string Id) {
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Exception ex = sistemaRepository.Excluir_Usuario_Web_Anexo(Convert.ToInt32(Id), Convert.ToInt16(Seq));
            return Json(new { success = true, responseText = "Arquivo removido com sucesso." }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Enviar_Analise(string Id,string Nome,string Cpf) {
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            int _userId = Convert.ToInt32(Id);
            Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(_userId);

            string Body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("~/Files/UserDocTemplate.htm"));
            Body = Body.Replace("#N#", Nome);
            Body = Body.Replace("#C#", Cpf);
            Body = Body.Replace("#E#", _user.Email);
            Body = Body.Replace("#F#", _user.Telefone);
            using (MailMessage emailMessage = new MailMessage()) {
                emailMessage.From = new MailAddress("gti@jaboticabal.sp.gov.br", "Prefeitura de Jaboticabal");
                emailMessage.To.Add(new MailAddress("pad@jaboticabal.sp.gov.br"));
                emailMessage.Subject = "Prefeitura Municipal de Jaboticabal - Acesso aos serviços online (G.T.I.)";
                emailMessage.Body = Body;
                emailMessage.Priority = MailPriority.Normal;
                emailMessage.IsBodyHtml = true;

                using (SmtpClient MailClient = new SmtpClient("smtp.gmail.com", 587)) {
                    MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
                    MailClient.Send(emailMessage);
                }
            }

            Usuario_Web_Analise _aut = new Usuario_Web_Analise() {
                Id=_userId,
                Data_envio=DateTime.Now,
                Autorizado=false,
                Autorizado_por=0
            };
            Exception ex = sistemaRepository.Incluir_Usuario_Web_Analise(_aut);

            return Json(new { success = true, responseText = "Analise enviada com sucesso." }, JsonRequestBehavior.AllowGet);
        }

        [Route("User_query_doc")]
        [HttpGet]
        public ActionResult User_query_doc() {
            LoginViewModel model = new LoginViewModel();
            if (Session["hashid"] == null)
                return RedirectToAction("Login");

            bool _func = Session["hashfunc"].ToString() == "S" ? true : false;
            if (!_func)
                return RedirectToAction("Login");
            int _id = Convert.ToInt32(Session["hashid"].ToString());
            if(_id!=392 && _id!=270 && _id!=118 && _id!=433 && _id!=427)//392-renata,270-joseane,118-elivaine ,433-schwartz, 427-rose
                return RedirectToAction("Login");

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            int _fiscal = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));

            usuarioStruct reg = sistemaRepository.Retorna_Usuario(_fiscal);

            List<Usuario_Web_Analise_Struct> Lista = sistemaRepository.Lista_Usuario_Web_Analise();
            model.Lista_Usuario_Web_Analise = Lista;

            return View(model);
        }

        public JsonResult Carrega_User_Doc(string userId) {
            int _user = Convert.ToInt32(userId);
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web _userWeb = sistemaRepository.Retorna_Usuario_Web(_user);
            bool _fisica = _userWeb.Cpf_Cnpj.Length == 11 ? true : false;

            List<Usuario_Web_Anexo_Struct>Lista = sistemaRepository.Lista_Usuario_Web_Tipo_Anexo(_user, _fisica);
            int _pos = 0;
            foreach (Usuario_Web_Anexo_Struct _anexo in Lista) {
                Usuario_web_anexo _reg = sistemaRepository.Retorna_Web_Anexo(_user, _anexo.Codigo);
                if (_reg != null) {
                    Lista[_pos].Arquivo = Functions.TruncateTo(_reg.Arquivo, 32);
                }
                _pos++;
            }
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public FileResult Anexo_Download(string userid, string tipo) {
            int _id = Convert.ToInt32(userid);
            short _tipo = Convert.ToInt16(tipo);
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Usuario_web_anexo _anexo = sistemaRepository.Retorna_Usuario_Web_Anexo(_id, _tipo);
            string _file = _anexo.Arquivo;

            string fullName = Server.MapPath("~");
            fullName = Path.Combine(fullName, "Files");
            fullName = Path.Combine(fullName, "UserDoc");
            fullName = Path.Combine(fullName, _id.ToString("00000"));
            fullName = Path.Combine(fullName, _file);
            fullName = fullName.Replace("\\", "/");
            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, _file);
        }

        byte[] GetFile(string s) {
            FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(s);
            return data;
        }

        public JsonResult Libera_Acesso(string userId,string dataenvio) {
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            int _fiscal = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));
            string _dataenvio = Convert.ToDateTime(dataenvio).ToString("dd/MM/yyyy HH:mm");
            Exception ex = sistemaRepository.Ativar_Usuario_Web_Doc(Convert.ToInt32(userId),_fiscal,Convert.ToDateTime(_dataenvio));
            return Json(new { success = true, responseText = "Acesso liberado com sucesso." }, JsonRequestBehavior.AllowGet);
        }

        [Route("gtiSys")]
        [HttpGet]
        public ActionResult gtiSys() {


            return View();
        }


    }
}
