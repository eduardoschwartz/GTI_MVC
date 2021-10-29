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
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Uf> listaUf = enderecoRepository.Lista_UF();
            ViewBag.ListaUF = new SelectList(listaUf, "siglauf", "descuf");
            return View(model);
        }

        public JsonResult Lista_Cidade(string uf) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Cidade> Lista = enderecoRepository.Lista_Cidade(uf);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Bairro(string uf,string cidade) {
            if (cidade == "") return null;
            int _cidade = Convert.ToInt32(cidade);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Bairro> Lista = enderecoRepository.Lista_Bairro(uf,_cidade);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Incluir_Bairro(string uf,string cidade,string bairro) {
            short _cidade = Convert.ToInt16(cidade);
            bairro = bairro.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            bool existeBairro = enderecoRepository.Existe_Bairro(uf, _cidade, bairro);
            if (existeBairro) {
                var result = new { Bairro_Codigo = 0, Success = "False",Msg="Bairro já cadastrado!" };
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            Bairro reg = new Bairro() {
                Siglauf=uf,
                Codcidade=_cidade,
                Descbairro=bairro
            };
            int _codigo = enderecoRepository.Incluir_bairro(reg);

            var result2 = new  { Bairro_Codigo=(short)_codigo, Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            
        }

        public JsonResult Alterar_Bairro(string uf, string cidade, string bairro,string novo_nome) {
            short _cidade = Convert.ToInt16(cidade);
            short _bairro = Convert.ToInt16(cidade);
            novo_nome =novo_nome.ToUpper();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            Bairro reg = new Bairro() {
                Siglauf = uf,
                Codcidade = _cidade,
                Codbairro=_bairro,
                Descbairro = novo_nome
            };
            Exception ex = enderecoRepository.Alterar_Bairro(reg);

            var result2 = new { Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}