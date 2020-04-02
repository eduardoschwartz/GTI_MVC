using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {

        public ViewResult Index() {
            if (Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Certidao")]
        public ViewResult Certidao() {
            if (Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Login")]
        [HttpGet]
        public ViewResult Login() {
            if (Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Logout")]
        [HttpGet]
        public ViewResult Logout() {
            ViewBag.UserId = null;
            Session.Clear();
            ViewBag.FullName = "Visitante";
            ViewBag.LoginName = "";
            return View("Index");
        }


        [Route("SysMenu")]
        [HttpGet]
        public ViewResult SysMenu() {
            if (Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(Session["gti_V3id"].ToString());
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
                Session["pUserId"] = 0;
                ViewBag.Result = "Usuário/Senha inválido!";
                return View(loginViewModel);
            } else {
                sOldPwd2 = tacesso_Class.DecryptGTI(sOldPwd);
                if (sOldPwd2 != sNewPwd) {
                    Session["pUserId"] = 0;
                    ViewBag.Result = "Usuário/Senha inválido!";
                    return View(loginViewModel);
                } else {
                    ViewBag.Result = "";
                    Session["pUserId"] = UserId;
                }
            }

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            if (UserId == 0) {
                ViewBag.Result = "Usuário/Senha inválido.";
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View(loginViewModel);
            }

            usuarioStruct _user = sistemaRepository.Retorna_Usuario(UserId);
            if (_user.Ativo == 0) {
                ViewBag.Result = "Usuário inativo.";
                return View(loginViewModel);
            } else {

                Session["gti_V3id"] = Functions.Encrypt(UserId.ToString("00000"));
                Session["gti_V3login"] = Functions.Encrypt(_user.Nome_login);
                Session["gti_V3full"] = Functions.Encrypt(_user.Nome_completo);
                ViewBag.LoginName = _user.Nome_login;
                ViewBag.FullName = _user.Nome_completo;
                return View("../Home/SysMenu");
            }
        }

    }
}