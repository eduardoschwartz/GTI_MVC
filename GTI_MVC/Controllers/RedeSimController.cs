using CsvHelper;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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
            string _msg = "",_tipo="",_guid="";
            DateTime _dataDe = DateTime.Now, _dataAte = DateTime.Now;
            bool _ok=false;
            var fileName = "";
            List<Redesim_Registro> _listaRegistro = new List<Redesim_Registro>();
            Redesim_bll redesimRepository = new Redesim_bll("GTIconnection");
            foreach (var file in model.Files) {
                fileName = Path.GetFileName(file.FileName);
                if (file.ContentLength > 0) {
                    if (file.ContentType == "application/vnd.ms-excel") {

                        _guid = Guid.NewGuid().ToString("N");
                        string _path = "~/Files/Redesim/";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path ), _guid);
                        file.SaveAs(path);

                        //###Leitura do arquivo gravado####
                        {
                            StreamReader reader = new StreamReader(@path,Encoding.Default);
                            while (!reader.EndOfStream) {
                                string line = reader.ReadLine();
                                if (!String.IsNullOrWhiteSpace(line)) {
                                    string[] values = line.Split(';');
                                    if (values.Length == 34) {

                                    }
                                }
                            }
                        }
                        //#################################

                        Redesim_arquivo reg = new Redesim_arquivo() {
                            Guid = _guid,
                            Periodode = DateTime.Now,
                            Periodoate = DateTime.Now,
                            Tipo = "R"
                        };
                        Exception ex = redesimRepository.Incluir_Arquivo(reg);

                        _msg = "Arquivo importado";
                        _ok = true;
                        _tipo = "Licenciamento";

                    } else {
                        _msg = "Arquivo inválido";
                    }
                } else {
                    _msg = "Tamanho inválido";
                }
                RedesimImportFilesViewModel _reg = new RedesimImportFilesViewModel() {
                    Guid=_guid,
                    NomeArquivo=fileName,
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