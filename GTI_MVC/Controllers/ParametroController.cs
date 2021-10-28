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
    public class ParametroController : Controller    {
        private readonly string _connection = "GTIconnection";

        [Route("Bairro_Edit")]
        [HttpGet]
        public ActionResult Bairro_Edit()
        {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Uf> listaUf = enderecoRepository.Lista_UF();
            ViewBag.ListaUF = new SelectList(listaUf, "siglauf", "descuf");
            return View();
        }

        [Route("Bairro_Edit")]
        [HttpPost]
        public ActionResult Bairro_Edit(BairroViewModel model) {
            return View(model);
        }

        public JsonResult Lista_Cidade(string uf) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Cidade> Lista = enderecoRepository.Lista_Cidade(uf);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}