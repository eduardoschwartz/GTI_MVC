using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers
{
    public class ParametroController : Controller    {
        private readonly string _connection = "GTIconnection";

        public ActionResult Bairro_Edit()
        {
            return View();
        }
    }
}