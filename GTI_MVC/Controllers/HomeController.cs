using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class HomeController : Controller {

    

        public ViewResult Index() {
            if (HttpContext.Session["gti_V3id"]==null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session["gti_V3id"].ToString());
            }
            return View();
        }

        [Route("Certidao")]
        public ViewResult Certidao() {
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

        [Route("Login")]
        [HttpGet]
        public ViewResult Login() {
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

        [Route("Logout")]
        [HttpGet]
        public ViewResult Logout() {
            ViewBag.UserId = null;
            HttpContext.Session.Clear();
            ViewBag.FullName = "Visitante";
            ViewBag.LoginName = "";
            return View("Index");
        }


        [Route("SysMenu")]
        [HttpGet]
        public ViewResult SysMenu() {
            if (HttpContext.Session["gti_V3id"] == null) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt( HttpContext.Session["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt( HttpContext.Session["gti_V3id"].ToString());
                return View();
            }
        }


        [Route("Login")]
        [HttpPost]
        public ViewResult Login(LoginViewModel model) {

            //List<usuarioStruct> _lista = sistemaRepository.Lista_Usuarios();
            //foreach (usuarioStruct item in _lista) {
            //    if (item.Senha2 != null) {
            //        Usuario reg = new Usuario() {
            //            Nomelogin = item.Nome_login,
            //            Senha2 = Functions.Encrypt(item.Senha2)
            //        };
            //        Exception ex = sistemaRepository.Alterar_Senha(reg);
            //    }
            //}
            LoginViewModel loginViewModel = new LoginViewModel();

            Sistema_bll sistemaRepository = new Sistema_bll("GTIconnection");
            int _userId = sistemaRepository.Retorna_User_LoginId(model.Usuario);
            if (_userId == 0) {
                ViewBag.Result = "Usuário/Senha inválido.";
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View(loginViewModel);
            }

            usuarioStruct _user = sistemaRepository.Retorna_Usuario(_userId);
            if (_user.Ativo == 0) {
                ViewBag.Result = "Usuário inativo.";
                return View(loginViewModel);
            } else {
                if (Functions.Decrypt( _user.Senha2) == model.Senha) {
                    HttpContext.Session[ "gti_V3id"]= Functions.Encrypt(_userId.ToString("00000"));
                    HttpContext.Session["gti_V3login"]= Functions.Encrypt(_user.Nome_login);
                    HttpContext.Session["gti_V3full"]= Functions.Encrypt(_user.Nome_completo);
                    ViewBag.LoginName = _user.Nome_login;
                    ViewBag.FullName = _user.Nome_completo;
                    return View("../Home/SysMenu");
                } else {
                    ViewBag.Result = "Usuário/Senha inválido.";
                    ViewBag.LoginName = "";
                    ViewBag.FullName = "Visitante";
                    return View(loginViewModel);
                }
            }

        }

       



    }
}