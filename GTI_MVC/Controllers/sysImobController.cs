using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace GTI_Mvc.Controllers
{
    public class sysImobController : Controller   {
        private readonly string _connection = "GTIconnection";

        [HttpGet]
        [Route("imovel_data")]
        public ActionResult imovel_data(string c="3") {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();

            if (c != "") {
                int _codigo = Convert.ToInt32(c);
                Imovel_bll imovelRepository = new Imovel_bll(_connection);
                model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);
            }

            return View(model);
        }

    }
}