using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {

        //public ViewResult Index() {
        //    if (Functions.pUserId==0) {
        //        Functions.pUserFullName = "Visitante";
        //    }
        //    return View("Login_gti");
        //}

        public ViewResult Login_gti() {
            if (Functions.pUserId == 0) {
                Functions.pUserFullName = "Visitante";
            }
            return View();
        }

        [Route("Certidao")]
        public ViewResult Certidao() {
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public ViewResult Login() {
            return View();
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

    }
}