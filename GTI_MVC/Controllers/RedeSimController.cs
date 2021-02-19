using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace GTI_MVC.Controllers {
    public class RedeSimController : Controller
    {

        [Route("UploadFiles")]
        [HttpGet]
        public ActionResult UploadFiles() {
            List<RedesimImportFilesViewModel> Lista_Files = new List<RedesimImportFilesViewModel>();
            RedesimImportViewModel model = new RedesimImportViewModel() {
                ListaArquivo = Lista_Files
            };
            return View(model);
        }

        [Route("UploadFiles")]
        [HttpPost]
        public ActionResult UploadFiles(RedesimImportViewModel model) {
            List<RedesimImportFilesViewModel> Lista_Files = new List<RedesimImportFilesViewModel>();
            int _id = 1;
            string _msg = "",_tipo="";
            DateTime _dataDe = DateTime.Now, _dataAte = DateTime.Now;
            bool _ok=false;
            foreach (var file in model.Files) {
                var fileName= Path.GetFileName(file.FileName);
                if (file.ContentLength > 0) {
                    if (file.ContentType == "application/vnd.ms-excel") {
                        _msg = "Arquivo importado";
                        _ok = true;
                        _tipo = "Licenciamento";
                        
                        var filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
//                        file.SaveAs(filePath);
                    } else {
                        _msg = "Arquivo inválido";
                    }
                } else {
                    _msg = "Tamanho inválido";
                }
                RedesimImportFilesViewModel _reg = new RedesimImportFilesViewModel() {
                    Id = _id,
                    Nome=fileName,
                    Mensagem=_msg,
                    Valido=_ok,
                    Tipo=_tipo,
                    PeriodoDe=_dataDe,
                    PeriodoAte=_dataAte
                };
                Lista_Files.Add(_reg);
                _id++;
            }
            model.ListaArquivo = Lista_Files;
            return View(model);
      
        }
    }
}