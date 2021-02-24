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
    public class RedeSimController : Controller {

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
            int _id = 0;
            string _msg = "", _tipo = "", _guid = "", _tipoSigla = "";
            bool _ok = false;
            var fileName = "";
            if (model.ListaArquivo == null) model.ListaArquivo = new List<RedesimImportFilesViewModel>();

            Redesim_bll redesimRepository = new Redesim_bll("GTIconnection");
            

            foreach (var file in model.Files) {
                List<Redesim_RegistroStruct> _listaRegistro = new List<Redesim_RegistroStruct>();
                List<Redesim_ViabilidadeStuct> _listaViabilidade = new List<Redesim_ViabilidadeStuct>();
                if (file == null) {
                    goto Fim;
                }
                fileName = Path.GetFileName(file.FileName);
                if (file.ContentLength > 0) {
                    if (file.ContentType == "application/vnd.ms-excel") {

                        _guid = Guid.NewGuid().ToString("N");
                        string _path = "~/Files/Redesim/";
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(_path), _guid);
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
                        _listaRegistro.Clear();
                        _listaViabilidade.Clear();
                        //Lê Registro
                        if (_bRegistro) {
                            _listaRegistro = Read_Registro(path);
                            _listaRegistro = Insert_Registro(_listaRegistro, _guid);
                            _ok = true;
                            _tipoSigla = "R";
                        }

                        //Lê Viabilidade
                        if (_bViabilidade) {
                            _listaViabilidade = Read_Viabilidade(path);
                            _listaViabilidade = Insert_Viabilidade(_listaViabilidade, _guid);
                            _ok = true;
                            _tipoSigla = "V";
                        }

                        if (_ok) {
                            Redesim_arquivo reg = new Redesim_arquivo() {
                                Guid = _guid,
                                Tipo = _tipoSigla
                            };
                            Exception ex = redesimRepository.Incluir_Arquivo(reg);
                            _msg = "Arquivo importado";
                        }
                        //#####FIM ARQUIVO DE REGISTRO  #######
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
                    ListaRegistro = _listaRegistro,
                    ListaViabilidade = _listaViabilidade
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
            Redesim_bll redesimRepository = new Redesim_bll("GTIconnection");
            List<Redesim_natureza_juridica> _listaNatJuridica = redesimRepository.Lista_Natureza_Juridica();
            List<Redesim_evento> _listaEvento = redesimRepository.Lista_Evento();
            List<Redesim_porte_empresa> _listaPorte = redesimRepository.Lista_Porte_Empresa();
            List<Redesim_forma_atuacao> _listaForma = redesimRepository.Lista_Forma_Atuacao();

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

                        //Natureza Juridica
                        int _codigo = 0;
                        foreach (Redesim_natureza_juridica item in _listaNatJuridica) {
                            if (item.Nome == _linhaReg.NaturezaJuridica) {
                                _codigo = item.Codigo;
                                break;
                            }
                        }
                        if (_codigo == 0) {
                            _codigo = redesimRepository.Incluir_Natureza_Juridica( _linhaReg.NaturezaJuridica );
                            _listaNatJuridica.Add(new Redesim_natureza_juridica() { Codigo = _codigo, Nome = _linhaReg.NaturezaJuridica });
                        }
                        _linhaReg.NaturezaJuridicaCodigo = _codigo;

                        //Evento
                        _codigo = 0;
                        int[] _listaCod = new int[_linhaReg.Evento.Length];
                        int _indexEvento = 0;
                        foreach (string ev in _linhaReg.Evento) {
                            foreach (Redesim_evento item in _listaEvento) {
                                if (item.Nome == ev) {
                                    _codigo = item.Codigo;
                                    break;
                                }
                            }
                            if (_codigo == 0) {
                                _codigo = redesimRepository.Incluir_Evento(ev);
                                _listaEvento.Add(new Redesim_evento() { Codigo = _codigo, Nome = ev });
                            }
                            _listaCod[_indexEvento]=_codigo;
                            _indexEvento++;
                        }
                        _linhaReg.EventoCodigo= _listaCod;
                        redesimRepository.Incluir_Registro_Evento(_linhaReg.Protocolo, _listaCod);

                        //Porte Empresa
                        _codigo = 0;
                        foreach (Redesim_porte_empresa item in _listaPorte) {
                            if (item.Nome == _linhaReg.PorteEmpresa) {
                                _codigo = item.Codigo;
                                break;
                            }
                        }
                        if (_codigo == 0) {
                            _codigo = redesimRepository.Incluir_Porte_Empresa(_linhaReg.PorteEmpresa);
                            _listaPorte.Add(new Redesim_porte_empresa() { Codigo = _codigo, Nome = _linhaReg.PorteEmpresa });
                        }
                        _linhaReg.PorteEmpresaCodigo = _codigo;

                        //Forma Atuação
                        _codigo = 0;
                        int[] _listaFormaCod = new int[_linhaReg.FormaAtuacao.Length];
                        int _indexForma = 0;
                        foreach (string ev in _linhaReg.FormaAtuacao) {
                            foreach (Redesim_forma_atuacao item in _listaForma) {
                                if (item.Nome == ev) {
                                    _codigo = item.Codigo;
                                    break;
                                }
                            }
                            if (_codigo == 0) {
                                _codigo = redesimRepository.Incluir_Forma_Atuacao(ev);
                                _listaForma.Add(new Redesim_forma_atuacao() { Codigo = _codigo, Nome = ev });
                            }
                            _listaFormaCod[_indexForma] = _codigo;
                            _indexForma++;
                        }
                        _linhaReg.FormaAtuacaoCodigo = _listaFormaCod;
                        redesimRepository.Incluir_Registro_Forma_Atuacao(_linhaReg.Protocolo, _listaFormaCod);

                        //Cnae Secundária
                        int _size = _linhaReg.CnaeSecundaria.Length;
                        if (_size > 0) {
                            string[] _listaCnae = new string[_size];
                            int _indexCnae = 0;
                            foreach (string ev in _linhaReg.CnaeSecundaria) {
                                _listaCnae[_indexCnae] = ev;
                                _indexCnae++;
                            }
                            redesimRepository.Incluir_Cnae(_linhaReg.Protocolo, _listaCnae);
                        }

                        _listaRegistro.Add(_linhaReg);
                    }
                }
                _linha++;
            }
            return _listaRegistro;
        }

        private List<Redesim_ViabilidadeStuct> Read_Viabilidade(string _path) {
            int _linha = 1;
            List<Redesim_ViabilidadeStuct> _listaViabilidade = new List<Redesim_ViabilidadeStuct>();
            StreamReader reader = new StreamReader(@_path, Encoding.Default);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line)) {
                    if (_linha > 1) {
                        string[] values = line.Split(';');
                        Redesim_ViabilidadeStuct _linhaReg;
                        _linhaReg = new Redesim_ViabilidadeStuct() {
                            Protocolo = values[0],
                            Analise = values[1],
                            Nire = values[2],
                            Cnpj = values[3],
                            EmpresaEstabelecida = values[4],
                            Cnae = values[5].Split(','),
                            AtividadeAuxiliar = values[6],
                            DataProtocolo = values[7],
                            DataResultadoAnalise = values[8],
                            DataResultadoViabilidade = values[9],
                            TempoAndamento = values[10],
                            cdEvento = values[11].Split(','),
                            Evento = values[12].Split(','),
                            Cep = values[13],
                            TipoInscricaoImovel = values[14],
                            NumeroInscricaoImovel = values[15],
                            TipoLogradouro = values[16],
                            Logradouro = values[17],
                            Numero = values[18],
                            Bairro = values[19],
                            Complemento = values[20],
                            TipoUnidade = values[21],
                            FormaAtuacao = values[22],
                            Municipio = values[23],
                            RazaoSocial = values[24],
                            Orgao = values[25],
                            AreaImovel = values[26],
                            AreaEstabelecimento = values[27]
                        };
                        _listaViabilidade.Add(_linhaReg);
                    }
                }
                _linha++;
            }
            return _listaViabilidade;
        }

        private List<Redesim_RegistroStruct> Insert_Registro(List<Redesim_RegistroStruct> _listaRegistro, string _guid) {
            Redesim_bll redesimRepository = new Redesim_bll("GTIconnection");
            int _pos = 0;
            foreach (Redesim_RegistroStruct item in _listaRegistro) {
                bool _existe = redesimRepository.Existe_Registro(item.Protocolo);
                if (!_existe) {
                    Redesim_Registro reg = new Redesim_Registro() {
                        Protocolo = item.Protocolo,
                        Arquivo = _guid,
                        Cnpj = item.Cnpj,
                        Razao_Social = item.NomeEmpresarial.ToUpper(),
                        Cep = item.Cep,
                        Complemento = item.Complementos,
                        MatrizFilial = item.MatrizFilial,
                        Natureza_Juridica = item.NaturezaJuridicaCodigo,
                        Porte_Empresa = item.PorteEmpresaCodigo,
                        Cnae_Principal=item.CnaePrincipal
                    };
                    if (Functions.RetornaNumero(item.Numero) == "")
                        reg.Numero = 0;
                    else
                        reg.Numero = Convert.ToInt32(item.Numero);
                    Exception ex = redesimRepository.Incluir_Registro(reg);
                }
                _listaRegistro[_pos].Duplicado = _existe;
                _listaRegistro[_pos].Arquivo = _guid;
                _pos++;
            }
            return _listaRegistro;
        }

        private List<Redesim_ViabilidadeStuct> Insert_Viabilidade(List<Redesim_ViabilidadeStuct> _listaViabilidade, string _guid) {
            Redesim_bll redesimRepository = new Redesim_bll("GTIconnection");
            int _pos = 0;
            foreach (Redesim_ViabilidadeStuct item in _listaViabilidade) {
                bool _existe = redesimRepository.Existe_Viabilidade(item.Protocolo);
                if (!_existe) {
                    Redesim_Viabilidade reg = new Redesim_Viabilidade() {
                        Arquivo = _guid,
                        Protocolo = item.Protocolo,
                        Cnpj = item.Cnpj,
                        Razao_Social = item.RazaoSocial.ToUpper(),
                        Analise = item.Analise,
                        Nire = item.Nire.Trim(),
                        EmpresaEstabelecida = item.EmpresaEstabelecida == "Sim" ? true : false,
                        DataProtocolo = Convert.ToDateTime(item.DataProtocolo),
                        Cep = item.Cep,
                        Complemento = item.Complemento,
                        TipoUnidade = item.TipoUnidade,
                        AreaImovel = Convert.ToDecimal(item.AreaImovel),
                        AreaEstabelecimento = Convert.ToDecimal(item.AreaEstabelecimento)
                    };
                    if (Functions.RetornaNumero(item.Numero) == "")
                        reg.Numero = 0;
                    else
                        reg.Numero = Convert.ToInt32(item.Numero);
                    string _num = Functions.RetornaNumero(item.NumeroInscricaoImovel);
                    if (_num == "" || item.TipoInscricaoImovel.Trim() != "Número IPTU")
                        reg.NumeroInscricaoImovel = 0;
                    else
                        reg.NumeroInscricaoImovel = Convert.ToInt32(_num);
                    Exception ex = redesimRepository.Incluir_Viabilidade(reg);
                }
                _listaViabilidade[_pos].Duplicado = _existe;
                _listaViabilidade[_pos].Arquivo = _guid;
                _pos++;
            }
            return _listaViabilidade;

        }
    }
}