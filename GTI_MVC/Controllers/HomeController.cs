﻿using GTI_Bll.Classes;
using GTI_Models;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {

          public ViewResult Login_gti() {
            LoginViewModel model = new LoginViewModel();
            if (Functions.pUserId == 0) {
                Functions.pUserFullName = "Visitante";
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
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Logout")]
        [HttpGet]
        public ViewResult Logout() {
            Functions.pUserFullName = "Visitante";
            Functions.pUserLoginName = "";
            Functions.pUserId = 0;
            return View("Login_gti");
        }

        [Route("SysMenu")]
        [HttpGet]
        public ViewResult SysMenu() {

            if (Functions.pUserId == 0) { 
                return View("Login");
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
            sOldPwd = sistema_Class.Retorna_User_Password(sLogin);
            int UserId = sistema_Class.Retorna_User_LoginId(sLogin);
            if (sOldPwd == null) {
                Functions.pUserId = 0;
                ViewBag.Result = "Usuário/Senha inválido!";
                return View(loginViewModel);
            } else {
                sOldPwd2 = tacesso_Class.DecryptGTI(sOldPwd);
                if (sOldPwd2 != sNewPwd) {
                    ViewBag.Result = "Usuário/Senha inválido!";
                    Functions.pUserId = 0;
                    return View(loginViewModel);
                } else {
                    ViewBag.Result = "";
                    Functions.pUserId = UserId;
                }
            }

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            if (UserId == 0) {
                ViewBag.Result = "Usuário/Senha inválido.";
                return View(loginViewModel);
            }

            usuarioStruct _user = sistemaRepository.Retorna_Usuario(UserId);
            if (_user.Ativo == 0) {
                ViewBag.Result = "Usuário inativo.";
                return View(loginViewModel);
            } else {
                Functions.pUserLoginName = _user.Nome_login;
                Functions.pUserFullName = _user.Nome_completo;
                return View("../Home/SysMenu");
            }
        }

        [Route("Login_update")]
        [HttpGet]
        public ViewResult Login_update() {
            if (Functions.pUserId == 0)
                return View("Login");
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [Route("Login_update")]
        [HttpPost]
        public ViewResult Login_update(LoginViewModel model) {
            Sistema_bll sistema_Class = new Sistema_bll("GTIconnection");
            TAcessoFunction tacesso_Class = new TAcessoFunction();
            string _oldPwd = tacesso_Class.DecryptGTI(sistema_Class.Retorna_User_Password(Functions.pUserLoginName));
            if (model.Senha != _oldPwd) {
                ViewBag.Result = "Senha atual não confere!";
            } else {

                Usuario reg = new Usuario {
                    Nomelogin = Functions.pUserLoginName,
                    Senha = tacesso_Class.Encrypt128(model.Senha2)
                };
                Exception ex = sistema_Class.Alterar_Senha(reg);
                if (ex != null) {
                    ViewBag.Result = "Erro, senha não alterada";
                } else {
                    ViewBag.Result = "Sua senha foi alterada, por favor efetue login novamente";
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
            List<Contribuinte_Header_Struct> ListaCodigo = sistemaClass.CodigoHeader(tipo, cpf, cnpj, name);

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
            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            Usuario_web reg = new Usuario_web() {
                Nome = model.Usuario,
                Email = model.Email,
                Telefone = model.Telefone,
                Cpf_Cnpj = model.CpfValue == null ? Functions.RetornaNumero( model.CnpjValue) : Functions.RetornaNumero(model.CpfValue),
                Senha=Functions.Encrypt(model.Senha2),
                Data_Cadastro=DateTime.Now,
                Ativo=false,
                Bloquedo=false
            };
            int id = sistemaRepository.Incluir_Usuario_Web(reg);
            string sid = Functions.Encrypt("#Prefeitura Municipal de Jaboticabal - GTI - Serviços Online#"+id.ToString("000000"));

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
                    MailClient.Credentials = new System.Net.NetworkCredential("gti.jaboticabal@gmail.com", "esnssgzxxjcdjrpk");
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

    }
}