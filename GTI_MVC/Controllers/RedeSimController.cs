using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;
using GTI_Mvc;

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
            DateTime _dataDe=DateTime.Now , _dataAte=DateTime.Now ;
            bool _ok=false;
            var fileName = "";
            List<Redesim_RegistroStruct> _listaRegistro = new List<Redesim_RegistroStruct>();
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
                            foreach (Redesim_RegistroStruct item in _listaRegistro) {
                                Redesim_Registro reg = new Redesim_Registro() {
                                    Protocolo = item.Protocolo,
                                    Arquivo=_guid,
                                    Cnpj=item.Cnpj,
                                    Razao_Social=item.NomeEmpresarial,
                                    Cep=item.Cep,
                                    Complemento=item.Complementos
                                };
                                if (Functions.RetornaNumero(item.Numero) == "")
                                    reg.Numero = 0;
                                else
                                    reg.Numero = Convert.ToInt32(item.Numero);
                                Exception ex = redesimRepository.Incluir_Registro(reg);
                            }
                            _ok = true;
                        }



                    //#################################
                        if (_ok) {
                            Redesim_arquivo reg = new Redesim_arquivo() {
                                Guid = _guid,
                                Periodode = _dataDe,
                                Periodoate = _dataAte,
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
            ProximoArquivo:
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


        private List<Redesim_RegistroStruct> Read_Registro(string _path) {
            int _linha = 1;
            List<Redesim_RegistroStruct> _listaRegistro = new List<Redesim_RegistroStruct>();
            StreamReader reader = new StreamReader(@_path, Encoding.Default);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line)) {
                    if (_linha > 1) {
                        string[] values = line.Split(';');
                        Redesim_RegistroStruct _linhaReg;
                        if (values[14] == "Sim" || values[14] == "Não") {
                            _linhaReg = new Redesim_RegistroStruct() {
                                Protocolo = values[0],
                                Cnpj = values[1],
                                Evento = values[2].Split(','),
                                NomeEmpresarial = values[3],
                                MatrizFilial = values[4],
                                DataAberturaEstabelecimento = values[5],
                                DataAberturaEmpresa = values[6],
                                Logradouro = values[7],
                                Numero = values[8],
                                Complementos = values[9],
                                Bairro = values[10],
                                Cep = values[11],
                                Municipio = values[12],
                                Referencia = values[13],
                                EmpresaEstabelecida = values[14],
                                NaturezaJuridica = values[15],
                                OrgaoRegistro = values[16],
                                NumeroOrgaoRegistro = values[17],
                                CapitalSocial = values[18],
                                CpfResponsavel = values[19],
                                NomeResponsavel = values[20],
                                QualificacaoResponsavel = values[21],
                                TelefoneResponsavel = values[22],
                                EmailResponsavel = values[23],
                                PorteEmpresa = values[24],
                                CnaePrincipal = values[25],
                                CnaeSecundaria = values[26].Split(','),
                                AtividadeAuxiliar = values[27],
                                TipoUnidade = values[28],
                                FormaAtuacao = values[29].Split(','),
                                Qsa = values[30],
                                CpfRepresentante = values[31],
                                NomeRepresentante = values[32]
                            };
                        } else {
                            //Criado para corrigir um erro no arquivo, onde alguns registros vem com posição errada
                            _linhaReg = new Redesim_RegistroStruct() {
                                Protocolo = values[0],
                                Cnpj = values[1],
                                Evento = values[2].Split(','),
                                NomeEmpresarial = values[3],
                                MatrizFilial = values[4],
                                DataAberturaEstabelecimento = values[5],
                                DataAberturaEmpresa = values[6],
                                Logradouro = values[7],
                                Numero = values[8],
                                Complementos = values[9],
                                Bairro = values[10],
                                Cep = values[11],
                                Municipio = values[12],
                                Referencia = "",
                                EmpresaEstabelecida = values[15],
                                NaturezaJuridica = values[16],
                                OrgaoRegistro = values[17],
                                NumeroOrgaoRegistro = values[18],
                                CapitalSocial = values[19],
                                CpfResponsavel = values[20],
                                NomeResponsavel = values[21],
                                QualificacaoResponsavel = values[22],
                                TelefoneResponsavel = values[23],
                                EmailResponsavel = values[24],
                                PorteEmpresa = values[25],
                                CnaePrincipal = values[26],
                                CnaeSecundaria = values[27].Split(','),
                                AtividadeAuxiliar = values[28],
                                TipoUnidade = values[29],
                                FormaAtuacao = values[30].Split(','),
                                Qsa = values[31],
                                CpfRepresentante = values[32],
                                NomeRepresentante = values[33]
                            };
                        }
                        _listaRegistro.Add(_linhaReg);
                    }
                }
                _linha++;
            }
            return _listaRegistro;
        }



    }
}