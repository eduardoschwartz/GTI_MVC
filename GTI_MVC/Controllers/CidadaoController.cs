using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_MVC.Controllers
{
    public class CidadaoController : Controller   {
        private readonly string _connection = "GTIconnectionTeste";

        [Route("Cidadao_menu")]
        [HttpGet]
        public ActionResult Cidadao_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Cidadao_chk")]
        [HttpGet]
        public ActionResult Cidadao_chk() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();

            return View(model);
        }

        [Route("Cidadao_chk")]
        [HttpPost]
        public ActionResult Cidadao_chk(CidadaoViewModel model) {
            string _cpfcnpj = Functions.RetornaNumero(model.CpfCnpj);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;
            bool _bCnpj = !_bCpf;

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            int _codigo = 0;
            if (_bCpf) {
                _codigo = cidadaoRepository.Existe_Cidadao_Cpf(_cpfcnpj);
            } else {
                _codigo = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfcnpj);
            }

            if (_codigo > 0) {
                ViewBag.Result = "Já existe um cadastro com este Cpf/Cnpj!";
                return View(model);
            }

            TempData["c"] = model.CpfCnpj;
            return RedirectToAction("Cidadao_add");
        }

        [Route("Cidadao_add")]
        [HttpGet]
        public ActionResult Cidadao_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            List<Logradouro> Lista_Logradouro = new List<Logradouro>();
            ViewBag.LogradouroR = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");
            ViewBag.LogradouroC = new SelectList(Lista_Logradouro, "Codlogradouro", "Endereco");

            model.EnderecoR = new EnderecoStruct();
            model.EnderecoC = new EnderecoStruct();
            var _cpfcnpj = TempData["c"];
            if(_cpfcnpj==null)
                return RedirectToAction("Cidadao_chk");
            else
                model.CpfCnpj = TempData["c"].ToString();
            return View(model);
        }

        [Route("Cidadao_add")]
        [HttpPost]
        public ActionResult Cidadao_add(CidadaoViewModel model, string action) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _cep;
            

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");
            List<Logradouro> Lista_LogradouroR = new List<Logradouro>();
            List<Logradouro> Lista_LogradouroC = new List<Logradouro>();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            if (action == "btnCepR") {
                 _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoR.Cep));
                 var cepObj = GTI_Mvc.Classes.Cep.Busca_CepDB(_cep);
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }
                    
                    List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                    int s = 1;
                    foreach (string item in Lista_Tmp) {
                        Lista_LogradouroR.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                        s++;
                    }
                    ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");

                    Bairro bairro = enderecoRepository.Retorna_CepDB_Bairro(_cep);
                    if (bairro != null) {
                        model.EnderecoR.CodigoBairro = bairro.Codbairro;
                        model.EnderecoR.NomeBairro = bairro.Descbairro;
                    }
                    Cidade cidade = enderecoRepository.Retorna_CepDB_Cidade(_cep);
                    if (cidade != null) {
                        model.EnderecoR.CodigoCidade = cidade.Codcidade;
                        model.EnderecoR.NomeCidade = cidade.Desccidade;
                    }
                    model.EnderecoR.UF = cepObj.Estado;

                } else {
                    model.EnderecoR.CodLogradouro = 0;
                    model.EnderecoR.Endereco = "";
                    model.EnderecoR.CodigoBairro = 0;
                    model.EnderecoR.NomeBairro = "";
                    model.EnderecoR.CodigoCidade = 0;
                    model.EnderecoR.NomeCidade = "";
                    model.EnderecoR.Numero = 0;
                    model.EnderecoR.Complemento = "";
                    model.EnderecoR.UF = "";

                    ViewBag.Error = "* Cep do endereço residencial não localizado.";
                    return View(model);
                }
            } else {
                if (action == "btnCepC") {
                    _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoC.Cep));
                    var cepObjC = GTI_Mvc.Classes.Cep.Busca_CepDB(_cep);
                    if (cepObjC.CEP != null) {
                        string rua = cepObjC.Endereco;
                        if (rua.IndexOf('-') > 0) {
                            rua = rua.Substring(0, rua.IndexOf('-'));
                        }

                        Endereco_bll EnderecoCepository = new Endereco_bll(_connection);
                        List<string> Lista_Tmp = EnderecoCepository.Retorna_CepDB_Logradouro(_cep);
                        int s = 1;
                        foreach (string item in Lista_Tmp) {
                            Lista_LogradouroC.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                            s++;
                        }
                        ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");


                        Bairro bairro = EnderecoCepository.Retorna_CepDB_Bairro(_cep);
                        if (bairro != null) {
                            model.EnderecoC.CodigoBairro = bairro.Codbairro;
                            model.EnderecoC.NomeBairro = bairro.Descbairro;
                        }
                        Cidade cidade = EnderecoCepository.Retorna_CepDB_Cidade(_cep);
                        if (cidade != null) {
                            model.EnderecoC.CodigoCidade = cidade.Codcidade;
                            model.EnderecoC.NomeCidade = cidade.Desccidade;
                        }
                        model.EnderecoC.UF = cepObjC.Estado;

                    } else {
                        model.EnderecoC.CodLogradouro = 0;
                        model.EnderecoC.Endereco = "";
                        model.EnderecoC.CodigoBairro = 0;
                        model.EnderecoC.NomeBairro = "";
                        model.EnderecoC.CodigoCidade = 0;
                        model.EnderecoC.NomeCidade = "";
                        model.EnderecoC.Numero = 0;
                        model.EnderecoC.Complemento = "";
                        model.EnderecoC.UF = "";

                        ViewBag.Error = "* Cep do endereço comercial não localizado.";
                        return View(model);
                    }
                } else {
                    if (action == "btnCancel") {
                        return RedirectToAction("Cidadao_menu");
                    } else {
                        if (action == "btnValida" || action=="Cidadao_add") {
                            Grava_Cidadao(model,true);
                            return RedirectToAction("Cidadao_menu");
                        }
                    }
                }
            }

            if (ViewBag.LogradouroR==null && !string.IsNullOrEmpty(model.EnderecoR.Cep)){
                _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoR.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroR.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");
            } else {
                ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");
            }
            if (ViewBag.LogradouroC==null && !string.IsNullOrEmpty(model.EnderecoC.Cep)) {
                _cep = Convert.ToInt32(Functions.RetornaNumero( model.EnderecoC.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroC.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");
            } else {
                ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");
            }

            return View(model);
        }

        [Route("Cidadao_chkedt")]
        [HttpGet]
        public ActionResult Cidadao_chkedt() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();

            return View(model);
        }

        [Route("Cidadao_chkedt")]
        [HttpPost]
        public ActionResult Cidadao_chkedt(CidadaoViewModel model) {
            string _cpfcnpj = Functions.RetornaNumero(model.CpfCnpj);
            bool _bCpfCnpj = _cpfcnpj.Length >2 ? true : false;
            bool _bCodigo = model.Codigo > 0;

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            int _codigo = model.Codigo;

            if (_bCodigo ) {
                bool _existeCod = cidadaoRepository.ExisteCidadao(_codigo);
                if (!_existeCod) {
                    ViewBag.Result = "Não existe um cadastro cidadão com este Código!";
                    return View(model);
                }
            }

            if (_bCpfCnpj) {
                if (_cpfcnpj.Length==11) {
                    _codigo = cidadaoRepository.Existe_Cidadao_Cpf(_cpfcnpj);
                } else {
                    _codigo = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfcnpj);
                }
                if (_codigo == 0) {
                    ViewBag.Result = "Não existe um cadastro cidadão com este Cpf/Cnpj!";
                    return View(model);
                }
            }

            TempData["cod"] = _codigo;
            return RedirectToAction("Cidadao_edt");
        }

        [Route("Cidadao_edt")]
        [HttpGet]
        public ActionResult Cidadao_edt() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");

            var _cod = TempData["cod"];
            int _codigo = 0;
            if (_cod == null)
                return RedirectToAction("Cidadao_chkedt");
            else
                _codigo = Convert.ToInt32(_cod);

            if (_codigo == 0)
                return RedirectToAction("Cidadao_chkedt");

            CidadaoStruct _cid = cidadaoRepository.Dados_Cidadao(_codigo);
            CidadaoViewModel model = new CidadaoViewModel() {
                Codigo = _codigo,
                CpfCnpj = string.IsNullOrEmpty(_cid.Cnpj) ? Functions.FormatarCpfCnpj(_cid.Cpf) : Functions.FormatarCpfCnpj(_cid.Cnpj),
                Nome = _cid.Nome,
                Data_Nascto = _cid.DataNascto == null ? "" : Convert.ToDateTime(_cid.DataNascto).ToString("dd/MM/yyyy"),
                Rg_Numero=_cid.Rg??"",
                Rg_Orgao=_cid.Orgao??"",
                Cnh_Numero=_cid.Cnh??"",
                Cnh_Orgao=_cid.Orgaocnh??"",
                Profissao_Codigo=_cid.CodigoProfissao==null?0:(int)_cid.CodigoProfissao,
                EnderecoR = new EnderecoStruct() { 
                    Cep=_cid.CepR==null?"":_cid.CepR.ToString(),
                    Numero=_cid.NumeroR,
                    Complemento=_cid.ComplementoR??"",
                    NomeBairro=_cid.NomeBairroR??"",
                    NomeCidade=_cid.NomeCidadeR??"",
                    UF=_cid.UfR,
                    Telefone=_cid.TelefoneR??"",
                    Email=_cid.EmailR??""
                },
                EnderecoC = new EnderecoStruct() {
                    Cep = _cid.CepC == null ? "" : _cid.CepC.ToString(),
                    Numero = _cid.NumeroC,
                    Complemento = _cid.ComplementoC ?? "",
                    NomeBairro = _cid.NomeBairroC ?? "",
                    NomeCidade = _cid.NomeCidadeC ?? "",
                    UF = _cid.UfC,
                    Telefone = _cid.TelefoneC ?? "",
                    Email = _cid.EmailC ?? ""
                },
            };

            List<Logradouro> Lista_LogradouroR = new List<Logradouro>();
            List<Logradouro> Lista_LogradouroC = new List<Logradouro>();
            ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");
            ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");

            int _cepR = Convert.ToInt32(model.EnderecoR.Cep);
            int _cepC = Convert.ToInt32(model.EnderecoC.Cep);

            if (_cepR >0) {
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cepR);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroR.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                List<Cepdb> cepdbR = enderecoRepository.Retorna_CepDB_Logradouro_Codigo(_cepR, (int)_cid.CodigoBairroR);
                if (cepdbR.Count > 0)
                    model.EnderecoR.Endereco = cepdbR[0].Logradouro;
            } 
            if (_cepC > 0) {
                //if(_cid.UfC!=null && _cid.CodigoCidadeC!=null && _cid.CodigoBairroC!=null && _cid.NomeBairroC == null) {
                //    model.EnderecoC.NomeBairro = enderecoRepository.Retorna_Bairro(_cid.UfC,(int) _cid.CodigoCidadeC,(int) _cid.CodigoBairroC);
                //}

                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cepC);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroC.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                List<Cepdb> cepdbC = enderecoRepository.Retorna_CepDB_Logradouro_Codigo(_cepC, (int)_cid.CodigoBairroC);
                if (cepdbC.Count > 0)
                    model.EnderecoC.Endereco = cepdbC[0].Logradouro;
            }

            return View(model);
        }

        [Route("Cidadao_edt")]
        [HttpPost]
        public ActionResult Cidadao_edt(CidadaoViewModel model, string action) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _cep;

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Profissao> Lista = cidadaoRepository.Lista_Profissao();
            ViewBag.Lista_Profissao = new SelectList(Lista, "Codigo", "Nome");
            List<Logradouro> Lista_LogradouroR = new List<Logradouro>();
            List<Logradouro> Lista_LogradouroC = new List<Logradouro>();
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            if (action == "btnCepR") {
                _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoR.Cep));
                var cepObj = GTI_Mvc.Classes.Cep.Busca_CepDB(_cep);
                if (cepObj.CEP != null) {
                    string rua = cepObj.Endereco;
                    if (rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0, rua.IndexOf('-'));
                    }

                    List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                    int s = 1;
                    foreach (string item in Lista_Tmp) {
                        Lista_LogradouroR.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                        s++;
                    }
                    ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");

                    Bairro bairro = enderecoRepository.Retorna_CepDB_Bairro(_cep);
                    if (bairro != null) {
                        model.EnderecoR.CodigoBairro = bairro.Codbairro;
                        model.EnderecoR.NomeBairro = bairro.Descbairro;
                    }
                    Cidade cidade = enderecoRepository.Retorna_CepDB_Cidade(_cep);
                    if (cidade != null) {
                        model.EnderecoR.CodigoCidade = cidade.Codcidade;
                        model.EnderecoR.NomeCidade = cidade.Desccidade;
                    }
                    model.EnderecoR.UF = cepObj.Estado;

                } else {
                    model.EnderecoR.CodLogradouro = 0;
                    model.EnderecoR.Endereco = "";
                    model.EnderecoR.CodigoBairro = 0;
                    model.EnderecoR.NomeBairro = "";
                    model.EnderecoR.CodigoCidade = 0;
                    model.EnderecoR.NomeCidade = "";
                    model.EnderecoR.Numero = 0;
                    model.EnderecoR.Complemento = "";
                    model.EnderecoR.UF = "";

                    ViewBag.Error = "* Cep do endereço residencial não localizado.";
                    return View(model);
                }
            } else {
                if (action == "btnCepC") {
                    _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoC.Cep));
                    var cepObjC = GTI_Mvc.Classes.Cep.Busca_CepDB(_cep);
                    if (cepObjC.CEP != null) {
                        string rua = cepObjC.Endereco;
                        if (rua.IndexOf('-') > 0) {
                            rua = rua.Substring(0, rua.IndexOf('-'));
                        }

                        Endereco_bll EnderecoCepository = new Endereco_bll(_connection);
                        List<string> Lista_Tmp = EnderecoCepository.Retorna_CepDB_Logradouro(_cep);
                        int s = 1;
                        foreach (string item in Lista_Tmp) {
                            Lista_LogradouroC.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                            s++;
                        }
                        ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");


                        Bairro bairro = EnderecoCepository.Retorna_CepDB_Bairro(_cep);
                        if (bairro != null) {
                            model.EnderecoC.CodigoBairro = bairro.Codbairro;
                            model.EnderecoC.NomeBairro = bairro.Descbairro;
                        }
                        Cidade cidade = EnderecoCepository.Retorna_CepDB_Cidade(_cep);
                        if (cidade != null) {
                            model.EnderecoC.CodigoCidade = cidade.Codcidade;
                            model.EnderecoC.NomeCidade = cidade.Desccidade;
                        }
                        model.EnderecoC.UF = cepObjC.Estado;

                    } else {
                        model.EnderecoC.CodLogradouro = 0;
                        model.EnderecoC.Endereco = "";
                        model.EnderecoC.CodigoBairro = 0;
                        model.EnderecoC.NomeBairro = "";
                        model.EnderecoC.CodigoCidade = 0;
                        model.EnderecoC.NomeCidade = "";
                        model.EnderecoC.Numero = 0;
                        model.EnderecoC.Complemento = "";
                        model.EnderecoC.UF = "";

                        ViewBag.Error = "* Cep do endereço comercial não localizado.";
                        return View(model);
                    }
                } else {
                    if (action == "btnCancel") {
                        return RedirectToAction("Cidadao_menu");
                    } else {
                        if (action == "btnValida" || action == "Cidadao_edt" ) {
                            Grava_Cidadao(model,false);
                            return RedirectToAction("Cidadao_menu");
                        }

                    }
                }
            }

            if (ViewBag.LogradouroR == null && !string.IsNullOrEmpty(model.EnderecoR.Cep)) {
                _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoR.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroR.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");
            } else {
                ViewBag.LogradouroR = new SelectList(Lista_LogradouroR, "Codlogradouro", "Endereco");
            }
            if (ViewBag.LogradouroC == null && !string.IsNullOrEmpty(model.EnderecoC.Cep)) {
                _cep = Convert.ToInt32(Functions.RetornaNumero(model.EnderecoC.Cep));
                List<string> Lista_Tmp = enderecoRepository.Retorna_CepDB_Logradouro(_cep);
                int s = 1;
                foreach (string item in Lista_Tmp) {
                    Lista_LogradouroC.Add(new Logradouro() { Codlogradouro = s, Endereco = item.ToUpper() });
                    s++;
                }
                ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");
            } else {
                ViewBag.LogradouroC = new SelectList(Lista_LogradouroC, "Codlogradouro", "Endereco");
            }

            return View(model);
        }

        private int Grava_Cidadao(CidadaoViewModel model,bool _new) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);

            string _nome = model.Nome.ToUpper();
            string _cpfcnpj = Functions.RetornaNumero(model.CpfCnpj);
            bool _bCpf = _cpfcnpj.Length == 11 ? true : false;
            bool _bCnpj = !_bCpf;
            bool _juridica = false;
            string _cpf = "", _cnpj = "";
            if (_bCpf)
                _cpf = _cpfcnpj;
            else {
                _cnpj = _cpfcnpj;
                _juridica = true;
            }
            DateTime _dataNascto = Convert.ToDateTime(model.Data_Nascto);
            int _profissao = model.Profissao_Codigo;
            string _rg = model.Rg_Numero ?? "";
            string _rgorgao = model.Rg_Orgao ?? "";
            string _cnh = model.Cnh_Numero ?? "";
            string _cnhorgao = model.Cnh_Orgao ?? "";

            //Tratamento do endereço residencial
            string _cepRes = model.EnderecoR.Cep;
            int _cepR = string.IsNullOrEmpty(_cepRes) ? 0 : Convert.ToInt32(Functions.RetornaNumero(_cepRes));
            string _logradouroNomeR = model.EnderecoR.Endereco;
            int _logradouroCodigoR = 0, _paisR = 0;
            short _bairroCodigoR = 0, _numImovelR = 0;
            short _cidadeCodigoR = 0;
            string _ufR = "", _telefoneR = "", _emailR = "", _complR = "";
            string _etiqR = "N";

            if (_cepR > 0) {
                Cepdb _cepdbR = enderecoRepository.Retorna_CepDB(_cepR);
                _bairroCodigoR = (short)_cepdbR.Bairrocodigo;
                _telefoneR = model.EnderecoR.Telefone ?? "";
                _emailR = model.EnderecoR.Email ?? "";
                _complR = model.EnderecoR.Complemento ?? "";
                _etiqR = "S";
                if (model.EnderecoR.Numero != null)
                    _numImovelR = Convert.ToInt16(model.EnderecoR.Numero);
                _paisR = 1;
                if (_cepdbR.Uf == "SP" && _cepdbR.Cidadecodigo == 413) {
                    LogradouroStruct _ruaR = enderecoRepository.Retorna_Logradouro_Cep(_cepR);
                    _logradouroCodigoR = (int)_ruaR.CodLogradouro;
                    _cidadeCodigoR = 413;
                    _ufR = "SP";
                } else {
                    _cidadeCodigoR = (short)_cepdbR.Cidadecodigo;
                    _ufR = _cepdbR.Uf;
                    _logradouroNomeR = _cepdbR.Logradouro;
                }
            }

            //Tratamento do endereço comercial
            string _cepCom = model.EnderecoC.Cep;
            int _cepC = string.IsNullOrEmpty(_cepCom) ? 0 : Convert.ToInt32(Functions.RetornaNumero(_cepCom));
            string _logradouroNomeC = model.EnderecoR.Endereco;
            int _logradouroCodigoC = 0, _paisC = 0;
            short _bairroCodigoC = 0, _numImovelC = 0;
            short _cidadeCodigoC = 0;
            string _ufC = "", _telefoneC = "", _emailC = "", _complC = "";
            string _etiqC = "N";

            if (_cepC > 0) {
                Cepdb _cepdbC = enderecoRepository.Retorna_CepDB(_cepC);
                _bairroCodigoC = (short)_cepdbC.Bairrocodigo;
                _telefoneC = model.EnderecoC.Telefone ?? "";
                _emailC = model.EnderecoC.Email ?? "";
                _complC = model.EnderecoC.Complemento ?? "";
                _etiqC = "S";
                if (model.EnderecoC.Numero != null)
                    _numImovelC = Convert.ToInt16(model.EnderecoC.Numero);
                _paisC = 1;
                if (_cepdbC.Uf == "SP" && _cepdbC.Cidadecodigo == 413) {
                    LogradouroStruct _ruaC = enderecoRepository.Retorna_Logradouro_Cep(_cepC);
                    _logradouroCodigoC = (int)_ruaC.CodLogradouro;
                    _cidadeCodigoC = 413;
                    _ufC = "SP";
                } else {
                    _cidadeCodigoC = (short)_cepdbC.Cidadecodigo;
                    _ufC = _cepdbC.Uf;
                    _logradouroNomeC = _cepdbC.Logradouro;
                }
            }

            Cidadao _cid = new Cidadao() {
                Codcidadao=model.Codigo,
                Nomecidadao = _nome,
                Cpf = _cpf,
                Cnpj = _cnpj,
                Data_nascimento = _dataNascto,
                Rg = _rg,
                Orgao = _rgorgao,
                Cnh = _cnh,
                Orgaocnh = _cnhorgao,
                Cep = _cepR,
                Cep2 = _cepC,
                Codlogradouro = _logradouroCodigoR,
                Nomelogradouro = _logradouroNomeR,
                Complemento = _complR,
                Numimovel = _numImovelR,
                Codbairro = _bairroCodigoR,
                Codcidade = _cidadeCodigoR,
                Codpais = _paisR,
                Siglauf = _ufR,
                Telefone = _telefoneR,
                Email = _emailR,
                Etiqueta = _etiqR,
                Codlogradouro2 = _logradouroCodigoC,
                Nomelogradouro2 = _logradouroNomeC,
                Complemento2 = _complC,
                Numimovel2 = _numImovelC,
                Codbairro2 = _bairroCodigoC,
                Codcidade2 = _cidadeCodigoC,
                Siglauf2 = _ufC,
                Telefone2 = _telefoneC,
                Email2 = _emailC,
                Etiqueta2 = _etiqC,
                Codpais2 = _paisC,
                Codprofissao = _profissao,
                Juridica = _juridica
            };

            int _codigo = 0;
            if(_new)
                _codigo = cidadaoRepository.Incluir_cidadao(_cid);
            else {
                _codigo = model.Codigo;
                Exception ex = cidadaoRepository.Alterar_cidadao(_cid);
            }

            return _codigo;
        }

        [Route("Cidadao_qry")]
        [HttpGet]
        public ActionResult Cidadao_qry() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            CidadaoViewModel model = new CidadaoViewModel();
            List<CidadaoHeader> Lista_Cidadao = new List<CidadaoHeader>();
            model.Lista_Cidadao = Lista_Cidadao;

            return View(model);
        }


    }
}