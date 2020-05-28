using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_MVC.Controllers
{
    public class SharedController : Controller {

        [Route("Checkgticd")]
        [HttpGet]
        public ActionResult Checkgticd(string c)
        {
            ViewBag.c = c;
            return View();
        }

        [Route("Checkgticd")]
        [HttpPost]
        public ActionResult Checkgticd(CertidaoViewModel model,string c) {
            int _codigo, _ano, _numero;
            string _chave = c;
            if (c != null) {
                Tributario_bll tributarioRepository = new Tributario_bll("GTIconnection");
                Certidao reg = new Certidao();
                List<Certidao> certidao = new List<Certidao>();
                chaveStruct _chaveStruct = tributarioRepository.Valida_Certidao(_chave);
                if (!_chaveStruct.Valido) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View("Certidao_Endereco", model);
                } else {
                    _codigo = _chaveStruct.Codigo;
                    _numero = _chaveStruct.Numero;
                    _ano = _chaveStruct.Ano;

                    Certidao_endereco certidaoGerada = tributarioRepository.Retorna_Certidao_Endereco(_ano, _numero, _codigo);
                    if (certidaoGerada != null) {
                        reg.Codigo = _codigo;
                        reg.Ano = _ano;
                        reg.Numero = _numero;
                        reg.Endereco = certidaoGerada.Logradouro;
                        reg.Endereco_Numero = certidaoGerada.Li_num;
                        reg.Endereco_Complemento = certidaoGerada.Li_compl ?? "";
                        reg.Bairro = certidaoGerada.descbairro;
                        reg.Nome_Requerente = certidaoGerada.Nomecidadao;
                        reg.Data_Geracao = certidaoGerada.Data;
                        reg.Inscricao = certidaoGerada.Inscricao;
                        reg.Numero_Ano = reg.Numero.ToString("00000") + "/" + reg.Ano;
                    } else {
                        ViewBag.Result = "Ocorreu um erro ao processar as informações.";
                        return View("Certidao_Endereco", model);
                    }
                };

                certidao.Add(reg);

                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Certidao_Endereco_Valida.rpt"));

                try {
                    rd.SetDataSource(certidao);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream, "application/pdf", "Certidao_Endereco.pdf");
                } catch {

                    throw;
                }
            } else {
                ViewBag.Result = "Chave de validação inválida.";
                return View("Certidao_Endereço", model);
            }
        }

    }
}