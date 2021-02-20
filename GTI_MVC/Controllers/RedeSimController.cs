using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
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
                if (file == null) {
                    goto Fim;
                }
                fileName = Path.GetFileName(file.FileName);
                if (file.ContentLength > 0) {
                    if (file.ContentType == "application/vnd.ms-excel") {

                        _guid = Guid.NewGuid().ToString("N");
                        string _path = "~/Files/Redesim/";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path ), _guid);
                        file.SaveAs(path);

                        //###Verifica tipo de arquivo####
                        bool _bRegistro = false, _bViabilidade = false, _bLicenciamento = false;
                        StreamReader reader = new StreamReader(@path, Encoding.Default);
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line)) {
                            string[] values = line.Split(';');
                            if (values[0].ToString().ToUpper() == "PROTOCOLO" && values[1].ToString().ToUpper() == "CNPJ") {
                                _tipo = "Registro";
                                _bRegistro = true;
                            } else {
                                if (values[0].ToString().ToUpper() == "PROTOCOLO" && values[1].ToString().ToUpper() == "ANALISE") {
                                    _tipo = "Viabilidade";
                                    _bViabilidade = true;
                                } else {
                                    if (values[0].ToString().ToUpper() == "PROTOCOLOLICENCA" && values[1].ToString().ToUpper() == "IDSOLICITACAO") {
                                        _tipo = "Licenciamento";
                                        _bLicenciamento = true;
                                    }
                                }
                            }
                        }
                        reader.Close();
                        if (!_bRegistro && !_bViabilidade && !_bLicenciamento) {
                            _ok = false;
                            _msg = "Arquivo inválido";
                            goto ProximoArquivo;
                        }


                        if (_bRegistro) {
                            _listaRegistro = Read_Registro(path);
                            _ok = true;
                        }


                    //#################################
                    ProximoArquivo:
                        if (_ok) {
                            Redesim_arquivo reg = new Redesim_arquivo() {
                                Guid = _guid,
                                Periodode = DateTime.Now,
                                Periodoate = DateTime.Now,
                                Tipo = "R"
                            };
                            Exception ex = redesimRepository.Incluir_Arquivo(reg);
                        }
                    } else {
                        _ok = false;
                        _msg = "Arquivo inválido";
                    }
                } else {
                    _ok = false;
                    _msg = "Tamanho inválido";
                }
                RedesimImportFilesViewModel _reg = new RedesimImportFilesViewModel() {
                    Guid = _guid,
                    NomeArquivo = fileName,
                    Mensagem = _msg,
                    Valido = _ok,
                    Tipo = _tipo,
                    PeriodoDe = _dataDe,
                    PeriodoAte = _dataAte
                };
                Lista_Files.Add(_reg);
                _id++;
            }
        Fim:
            model.ListaArquivo = Lista_Files;
            return View(model);
        }


        private List<Redesim_Registro> Read_Registro(string _path) {
            int _linha = 1;
            List<Redesim_Registro> _listaRegistro = new List<Redesim_Registro>();
            StreamReader reader = new StreamReader(@_path, Encoding.Default);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line)) {
                    if (_linha > 1) {
                        string[] values = line.Split(';');
                        Redesim_Registro _linhaReg = new Redesim_Registro() {
                            Protocolo = values[0],
                            Cnpj = values[1],
                            Evento = values[2].Split(',')
                        };
                        _listaRegistro.Add(_linhaReg);
                    }
                }
                _linha++;
            }
            return _listaRegistro;
        }





    }
}