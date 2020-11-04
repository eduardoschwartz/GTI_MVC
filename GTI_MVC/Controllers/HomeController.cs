using GTI_Bll.Classes;
using GTI_Models;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.UI;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {

          public ViewResult Login_gti() {
            LoginViewModel model = new LoginViewModel();
            if (Session["hashid"] == null) {
                Session.Remove("hashfname");
                //Functions.pUserFullName = "Visitante";
            }
            return View(model);
        }

        [Route("Certidao")]
        public ViewResult Certidao() {
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public ViewResult Login() {
            
            if (Session["hashid"]!= null) {
                return View("SysMenu");
            }
            Session.Remove("hashid");
            Session.Remove("hashfname");
//            Functions.pUserFullName = "Visitante";
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Logout")]
        [HttpGet]
        public ViewResult Logout() {
            Session.Remove("hashfname");
            Session.Remove("hashlname");
//            Functions.pUserFullName = "Visitante";
 //           Functions.pUserLoginName = "";
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
                return View();
            }
        }

        [Route("Login")]
        [HttpPost]
        public ViewResult Login(LoginViewModel model) {
            string sLogin = model.Usuario, sNewPwd = model.Senha, sOldPwd, sOldPwd2, sName;
            LoginViewModel loginViewModel = new LoginViewModel();

            Sistema_bll sistema_Class = new Sistema_bll("GTIconnection");
            TAcessoFunction tacesso_Class = new TAcessoFunction();

            bool bFuncionario = model.Usuario.LastIndexOf('@') > 1 ? false : true;
            Session["hashfunc"] = bFuncionario ? "S" : "N";
           // Functions.pUserGTI = bFuncionario;
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");

            if (bFuncionario) {
                sOldPwd = sistema_Class.Retorna_User_Password(sLogin);
                int UserId = sistema_Class.Retorna_User_LoginId(sLogin);
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
                    Session["hashfiscal"] = _user.Fiscal ? "S" : "N";
                    if (Session["hashid"] == null) {
                        Session.Add("hashid", _user.Id);
                        Session.Add("hashfname", _user.Nome_completo);
                        Session.Add("hashlname", _user.Nome_login);
                        Session.Add("hashfiscalitbi", "N");
                        Session.Add("hashfiscal", "N");
                        Session.Add("hashfunc", "N");
                    }
                    return View("../Home/SysMenu");
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
                                return View("../Home/SysMenu");
                            }
                        }
                    }
                }
            }
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
        public ViewResult Login_update(LoginViewModel model) {
            int _id=0;
            Sistema_bll sistema_Class = new Sistema_bll("GTIconnection");
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

            Sistema_bll sistemaClass = new Sistema_bll("GTIconnection");
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
        public ActionResult Login_create(LoginViewModel model) {

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");

            if (sistemaRepository.Existe_Usuario_Web(model.Email)) {
                ViewBag.Result = "Este email já esta cadastrado.";
                return View(model);
            }

            Usuario_web reg = new Usuario_web() {
                Nome = model.Usuario,
                Email = model.Email,
                Telefone = model.Telefone,
                Cpf_Cnpj = model.CpfValue == null ? Functions.RetornaNumero( model.CnpjValue) : Functions.RetornaNumero(model.CpfValue),
                Senha=Functions.Encrypt(model.Senha2),
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
                Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
                Exception ex = sistemaRepository.Ativar_Usuario_Web(Id);
                return View();
            } else {
                return RedirectToAction("Login");
            }
        }

        [Route("Login_welcome")]
        [HttpPost]
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
        public ViewResult Login_resend(LoginViewModel model) {
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
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
        public ViewResult Login_resend_pwd(LoginViewModel model) {
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
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
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(Id);
            LoginViewModel model = new LoginViewModel() {
                Email = reg.Email
            };
            return View(model);
        }

        [Route("Login_reset")]
        [HttpPost]
        public ActionResult Login_reset(LoginViewModel model) {
            //if (Session["hashid"] == null) {
            //    Functions.pUserFullName = "Visitante";
            //    return View(model);
            //}
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            Usuario_web reg = sistemaRepository.Retorna_Usuario_Web(model.Email);
            int Id = reg.Id;
            Exception ex = sistemaRepository.Alterar_Usuario_Web_Senha(Id, model.Senha);

            ViewBag.Message = "A senha foi alterar com sucesso.";

            return View(model);
        }


    }
}
