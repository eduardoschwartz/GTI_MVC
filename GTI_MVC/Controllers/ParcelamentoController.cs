using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using Microsoft.Reporting.WebForms;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using static GTI_Models.modelCore;
using System.Net;
using Newtonsoft.Json.Linq;

namespace GTI_MVC.Controllers
{
    public class ParcelamentoController : Controller
    {
        [Route("Parc_index")]
        [HttpGet]
        public ActionResult Parc_index()
        {
            return View();
        }

        [Route("Parc_req")]
        [HttpGet]
        public ActionResult Parc_req() {
            ParcelamentoViewModel model = new ParcelamentoViewModel();
            model.Requerente = new Parc_Requerente();
            return View(model);
        }

        [Route("Parc_req")]
        [HttpPost]
        public ActionResult Parc_req(ParcelamentoViewModel model) {
            return View(model);
        }
















    }
}