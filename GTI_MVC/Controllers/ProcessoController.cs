using Antlr.Runtime.Tree;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GTI_MVC.Controllers {
    public class ProcessoController : Controller    {
        private readonly string _connection = "GTIconnectionTeste";
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
            public void OnAuthorization(AuthorizationContext filterContext) {
                if (filterContext == null) {
                    throw new ArgumentNullException("filterContext");
                }

                var httpContext = filterContext.HttpContext;
                var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
            }
        }

        [Route("Processo_menu")]
        [HttpGet]
        public ActionResult Processo_menu() {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("Processo_tp")]
        [HttpGet]
        public ActionResult Processo_tp() {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");
            Processo2ViewModel model = new Processo2ViewModel();

            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> ListaCC = processoRepository.Lista_Local(true,false);
            ViewBag.Lista_CCusto = new SelectList(ListaCC, "Codigo", "Descricao");

            return View(model);
        }

        [Route("Processo_tp")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Processo_tp(Processo2ViewModel model) {
            if (Request.Cookies["2lG1H*"] == null || Request.Cookies["2uC*"]==null || Request.Cookies["2fN*"]==null)
                return RedirectToAction("Login", "Home");
            
            int _userId = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));
            bool _func = Functions.Decrypt(Request.Cookies["2fN*"].Value) == "S" ? true : false;

            TempData["CentroCustoCod"] = model.Centro_Custo_Codigo;
            TempData["CentroCustoNome"] = model.Centro_Custo_Nome;
            return RedirectToAction("Processo_add");
        }

        [HttpGet]
        public JsonResult Lista_Cidadao(string codigo, string nome, string cpfcnpj) {
            if (string.IsNullOrEmpty(codigo)) codigo = "0";
            int _cod = Convert.ToInt32(Functions.RetornaNumero(codigo));
            string _nome = nome.Trim() ?? "";
            string _cpfcnpj = Functions.RetornaNumero(cpfcnpj) ?? "";

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Cidadao> Lista = cidadaoRepository.Lista_Cidadao(_cod, _nome, _cpfcnpj,15);

            List<Cidadao> ObjCid = new List<Cidadao>();
            foreach (Cidadao cid in Lista) {
                if (string.IsNullOrEmpty(cid.Cnpj) && cid.Cnpj!="0")
                    _cpfcnpj = cid.Cpf;
                else
                    _cpfcnpj = cid.Cnpj;

                Cidadao reg = new Cidadao() {
                    Codcidadao = cid.Codcidadao,
                    Nomecidadao = Functions.TruncateTo( cid.Nomecidadao.ToUpper(),45),
                    Cpf = Functions.FormatarCpfCnpj( _cpfcnpj)
                };
                ObjCid.Add(reg);
            }

            return Json(ObjCid, JsonRequestBehavior.AllowGet);
        }

        [Route("Processo_add")]
        [HttpGet]
        public ActionResult Processo_add(string p) {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }
            int _centro_custo_cod = Convert.ToInt32(TempData["CentroCustoCod"]);
            string _centro_custo_nome = TempData["CentroCustoNome"].ToString();

            Processo_bll processoRepository = new Processo_bll(_connection);

            if (_centro_custo_cod == 0) {
                return RedirectToAction("Processo_tp", "Processo");
            }

            List<Assunto> ListaAssunto= processoRepository.Lista_Assunto(true, false, "");
            ViewBag.Lista_Assunto= new SelectList(ListaAssunto, "Codigo", "Nome");

            Processo2ViewModel model = new Processo2ViewModel();
            model.Centro_Custo_Nome = _centro_custo_nome;
            model.Centro_Custo_Codigo = _centro_custo_cod;
            model.Tipo_Requerente = _centro_custo_cod<500000 ? "Prefeitura" : "Contribuinte";
            model.Interno = _centro_custo_cod<500000 ? "Sim" : "Não";
            return View(model);
        }
      
        public JsonResult Lista_Assunto(string search) {
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Assunto> Lista_Search = processoRepository.Lista_Assunto(true, false, search);
            return new JsonResult { Data = Lista_Search,JsonRequestBehavior= JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Doc(string assunto) {
            short _codAss = Convert.ToInt16(assunto);
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<AssuntoDocStruct> Lista_Search = processoRepository.Lista_Assunto_Documento(_codAss);
            return new JsonResult { Data = Lista_Search, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [ValidateJsonAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [Route("Processo_addx")]
        public ActionResult Processo_addx(List<Processo2ViewModel> dados) {
            if (dados[0].Assunto_Codigo == 0) {
                return Json(new { success = false, responseText = "Selecione um assunto válido." }, JsonRequestBehavior.AllowGet);
            }

            if(Request.Cookies["2uC*"]==null || Request.Cookies["2fN*"].Value==null)
                return RedirectToAction("Login", "Home");

            Processo_bll processoRepository = new Processo_bll(_connection);
            int _numero = processoRepository.Retorna_Numero_Disponivel(DateTime.Now.Year);
            if(_numero==0)
                return Json(new { success = false, responseText = "Erro ao gravar." }, JsonRequestBehavior.AllowGet);

            short _ano = Convert.ToInt16(DateTime.Now.Year);
            int _user = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));
            bool _isFunc =Functions.Decrypt( Request.Cookies["2fN*"].Value) == "S" ? true : false;
            short _tipoRequerente = dados[0].Tipo_Requerente == "Prefeitura" ? (short)1 : (short)2;

            Processogti reg = new Processogti() {
                Ano = _ano,
                Numero=_numero,
                Fisico = dados[0].Fisico,
                Origem = 1,
                Interno=_tipoRequerente==1,
                Codassunto =(short) dados[0].Assunto_Codigo,
                Observacao=dados[0].Observacao,
                Dataentrada=DateTime.Now.Date,
                Userid=_user,
                Userweb=!_isFunc,
                Complemento=dados[0].Complemento,
                Etiqueta=false,
                Hora= DateTime.Now.ToShortTimeString().ToString()
            };
            if (_tipoRequerente == 2) {
                reg.Codcidadao = dados[0].Centro_Custo_Codigo;
                reg.Centrocusto = 0;
            } else {
                reg.Codcidadao = 0;
                reg.Centrocusto = dados[0].Centro_Custo_Codigo;
            }

            Exception ex = processoRepository.Incluir_Processo(reg);

            if (dados[0].Lista_Endereco != null) {
                List<Processoend> _listaE = new List<Processoend>();
                foreach (ProcessoEndStruct end in dados[0].Lista_Endereco) {
                    Processoend regE = new Processoend() {
                        Ano = _ano,
                        Numprocesso = _numero,
                        Codlogr = Convert.ToInt16(end.CodigoLogradouro),
                        Numero = end.Numero
                    };
                    _listaE.Add(regE);
                }
                ex = processoRepository.Incluir_Processo_Endereco(_listaE, _ano, _numero);
            }


            if (dados[0].Lista_Documento != null) {
                List<Processodoc> _listaD = new List<Processodoc>();
                foreach (ProcessoDocStruct doc in dados[0].Lista_Documento) {
                    Processodoc regD = new Processodoc() {
                        Ano = _ano,
                        Numero = _numero,
                        Coddoc = (short)doc.CodigoDocumento
                    };
                    if (Functions.IsDate(doc.DataEntrega))
                        regD.Data = Convert.ToDateTime(doc.DataEntrega);

                    _listaD.Add(regD);
                }
                ex = processoRepository.Incluir_Processo_Documento(_listaD, _ano, _numero);
            }
            
            string _p = Functions.Encrypt( _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString());
            TempData["p"] = _p;
            return Json(new { success = true, responseText = "Processo gravado com sucesso!", processo=_p}, JsonRequestBehavior.AllowGet);
            
        }

        [Route("Processo_vw")]
        [HttpGet]
        public ActionResult Processo_vw(string p) {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }

            string _processo;
            if (TempData["p"] == null) {
                if (p == null)
                    return RedirectToAction("sysMenu", "Home");
                else
                    _processo = Functions.Decrypt(p);
            } else
                _processo = Functions.Decrypt(TempData["p"].ToString());

            if (_processo=="")
                return RedirectToAction("Processo_menu", "Processo");

            Processo_bll processoRepository = new Processo_bll(_connection);
            
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_processo);
            int _ano = processoNumero.Ano;
            int _numero = processoNumero.Numero;

            bool _existe = processoRepository.Existe_Processo(_ano, _numero);
            if(!_existe)
                return RedirectToAction("sysMenu", "Home");

            ProcessoStruct _proc = processoRepository.Dados_Processo(_ano, _numero);

            string _assunto = processoRepository.Retorna_Assunto((int)_proc.CodigoAssunto);

            Processo2ViewModel model = new Processo2ViewModel();
            model.NumProcesso = _numero;
            model.AnoProcesso = _ano;
            model.Numero_Processo = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            model.Complemento = _proc.Complemento;
            model.Assunto_Codigo =(int) _proc.CodigoAssunto;
            model.Assunto_Nome = _assunto;
            model.Observacao = _proc.Observacao;
            if (_proc.Interno) {
                model.Centro_Custo_Codigo = (int)_proc.CentroCusto;
                model.Centro_Custo_Nome = _proc.CentroCustoNome;
            } else {
                model.Centro_Custo_Codigo = (int)_proc.CodigoCidadao;
                model.Centro_Custo_Nome = _proc.NomeCidadao;
            }
            model.Interno = _proc.Interno ? "Sim" : "Não";
            model.Fisico_Nome = _proc.Fisico ? "Sim" : "Não";
            model.Lista_Documento = processoRepository.Lista_Processo_Documento(_ano, _numero);
            model.Lista_Endereco = processoRepository.Lista_Processo_Endereco((short)_ano, _numero);

            return View( model);
        }

        [Route("Processo_vw")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Processo_vw(Processo2ViewModel model, string action) {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }
            if (action == "btnEditar") {
                TempData["p"] = model.Numero_Processo;
                return RedirectToAction("Processo_edit");
            }

            return View(model);
        }

        [Route("Processo_qry")]
        [HttpGet]
        public ActionResult Processo_qry() {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");

            Processo2ViewModel model = new Processo2ViewModel();

            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> ListaCC = processoRepository.Lista_Local(true, false);
            ViewBag.Lista_CCusto = new SelectList(ListaCC, "Codigo", "Descricao");
            return View(model);
        }

        [Route("Processo_qry")]
        [HttpPost]
        public ActionResult Processo_qry(Processo2ViewModel model) {
            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");
            
            if(string.IsNullOrEmpty( model.Numero_Processo))
                return RedirectToAction("Login", "Home");

            string _numero = model.Numero_Processo;
            string _p = Functions.Encrypt(_numero);
            TempData["p"] = _p;

            if(model.Evento=="btnDetalhe")
                return RedirectToAction("Processo_vw");
            else {
                if(model.Evento=="btnTramite")
                    return RedirectToAction("Processo_trm");
            }

            return View(model);
        }

        [ValidateJsonAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [Route("Processo_qryx")]
        public ActionResult Processo_qryx(List<Processo2ViewModel> dados) {
            if (Request.Cookies["2uC*"] == null || Request.Cookies["2fN*"].Value == null)
                return RedirectToAction("Login", "Home");

            ProcessoFilter _filter = new ProcessoFilter() {};

            string _numero_processo = "";
            int _anoProc = 0, _numProc = 0;
            List<Processo2ViewModel> Lista_Proc = new List<Processo2ViewModel>();

            if (!string.IsNullOrEmpty(dados[0].Numero_Processo)) {
                _numero_processo = dados[0].Numero_Processo;
            }
            int _endereco_codigo = dados[0].Endereco_Codigo;
            int _assunto_codigo = dados[0].Assunto_Codigo; ;
            int _exercicio = dados[0].AnoProcesso;
            string _data_entrada = dados[0].Data_Entrada;
            int _endereco_numero= dados[0].Endereco_Numero;
            int _centro_custo = dados[0].Centro_Custo_Codigo;
            bool _interno = dados[0].Interno == "S" ? true : false;

            Processo_bll processoRepository = new Processo_bll(_connection);

            if (_numero_processo != "") {
                ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_numero_processo);
                _anoProc = processoNumero.Ano;
                _numProc = processoNumero.Numero;
                int _dv = Functions.RetornaDvProcesso(_numProc);
                if(_dv != processoNumero.Dv) {
                    Processo2ViewModel reg = new Processo2ViewModel() {Erro = "Digito verificador inválido!"};
                    Lista_Proc.Add(reg);
                    return new JsonResult { Data = Lista_Proc, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                _filter.Ano = _anoProc;
                _filter.Numero = _numProc;
            }

            if(Functions.IsDate(_data_entrada)) {
                _filter.DataEntrada = Convert.ToDateTime(_data_entrada);
            }

            if (_assunto_codigo > 0) {
                _filter.AssuntoCodigo = _assunto_codigo;
            }
            if (_exercicio > 0) {
                _filter.Ano = _exercicio;
            }
            if (_endereco_codigo > 0) {
                _filter.CodLogradouro = _endereco_codigo;
                _filter.NumEnd = _endereco_numero;
            }

            if (_centro_custo > 0) {
                _filter.Interno = _interno;
                _filter.Requerente = _centro_custo;
            } else {
                _filter.Requerente = 0;
            }

            if(_numero_processo=="" && !Functions.IsDate( _data_entrada) && _exercicio == 0){
                Processo2ViewModel reg = new Processo2ViewModel() {Erro = "Selecione um exercício válido!"};
                Lista_Proc.Add(reg);
                return new JsonResult { Data = Lista_Proc, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            if(_numero_processo == "" && !Functions.IsDate(_data_entrada) &&_assunto_codigo==0 && _endereco_codigo == 0 && _centro_custo==0) {
                Processo2ViewModel reg = new Processo2ViewModel() { Erro = "Selecione algum critério!" };
                Lista_Proc.Add(reg);
                return new JsonResult { Data = Lista_Proc, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            List<ProcessoStruct> Lista = processoRepository.Lista_Processos_Web(_filter);

            foreach (ProcessoStruct item in Lista) {
                Processo2ViewModel reg = new Processo2ViewModel() {
                    Numero_Processo = item.Numero.ToString("00000") + "-" + Functions.RetornaDvProcesso(item.Numero) + "/" + item.Ano.ToString(),
                    Data_Entrada = Convert.ToDateTime(item.DataEntrada).ToString("dd/MM/yyyy"),
                    Assunto_Nome=Functions.TruncateTo( item.Assunto,35),
                    Erro = ""
                };
                if (!string.IsNullOrEmpty( item.CentroCustoNome))
                    reg.Centro_Custo_Nome =  item.CentroCustoNome ?? "";
                else
                    reg.Centro_Custo_Nome = item.NomeCidadao ?? "";
                reg.Centro_Custo_Nome = Functions.TruncateTo(reg.Centro_Custo_Nome, 35);

                bool _existe = false;//Avoid duplicate regs
                foreach (Processo2ViewModel t in Lista_Proc) {
                    if (t.Numero_Processo == reg.Numero_Processo) {
                        _existe = true;
                        break;
                    }
                }
                if(!_existe)
                    Lista_Proc.Add(reg);
            };
        
            return new JsonResult { Data = Lista_Proc, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Route("Processo_edit")]
        [HttpGet]
        public ActionResult Processo_edit() {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }

            if (TempData["p"] == null)
                return RedirectToAction("sysMenu", "Home");

            string _processo = TempData["p"].ToString();

            Processo_bll processoRepository = new Processo_bll(_connection);

            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_processo);
            int _ano = processoNumero.Ano;
            int _numero = processoNumero.Numero;

            bool _existe = processoRepository.Existe_Processo(_ano, _numero);
            if (!_existe)
                return RedirectToAction("sysMenu", "Home");

            ProcessoStruct _proc = processoRepository.Dados_Processo(_ano, _numero);

            string _assunto = processoRepository.Retorna_Assunto((int)_proc.CodigoAssunto);

            Processo2ViewModel model = new Processo2ViewModel();
            model.NumProcesso = _numero;
            model.AnoProcesso = _ano;
            model.Numero_Processo = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            model.Complemento = _proc.Complemento;
            model.Assunto_Codigo = (int)_proc.CodigoAssunto;
            model.Assunto_Nome = _assunto;
            model.Observacao = _proc.Observacao;
            if (_proc.Interno) {
                model.Centro_Custo_Codigo = (int)_proc.CentroCusto;
                model.Centro_Custo_Nome = _proc.CentroCustoNome;
            } else {
                model.Centro_Custo_Codigo = (int)_proc.CodigoCidadao;
                model.Centro_Custo_Nome = _proc.NomeCidadao;
            }
            model.Interno = _proc.Interno ? "Sim" : "Não";
            model.Fisico_Nome = _proc.Fisico ? "Sim" : "Não";
            model.Lista_Documento = processoRepository.Lista_Processo_Documento(_ano, _numero);
            model.Lista_Endereco = processoRepository.Lista_Processo_Endereco((short)_ano, _numero);

            return View(model);
        }

        [Route("Processo_edit")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Processo_edit(Processo2ViewModel model, string action) {
            if (Request.Cookies["2lG1H*"] == null) {
                return RedirectToAction("Login", "Home");
            }
            
            if (action == "btnCancelar") {
                string _numero = model.Numero_Processo;
                string _p = Functions.Encrypt(_numero);
                TempData["p"] = _p;
                return RedirectToAction("Processo_vw");
            }
           
            return View(model);
        }

        [ValidateJsonAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        [Route("Processo_editx")]
        public ActionResult Processo_editx(List<Processo2ViewModel> dados) {
            if (Request.Cookies["2uC*"] == null || Request.Cookies["2fN*"].Value == null)
                return RedirectToAction("Login", "Home");

            string _numero_processo = dados[0].Numero_Processo;

            Processo_bll processoRepository = new Processo_bll(_connection);
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_numero_processo);
            short _ano = Convert.ToInt16(processoNumero.Ano);
            int _numero = processoNumero.Numero;

            bool _existe = processoRepository.Existe_Processo(_ano, _numero);
            if (!_existe) {
                return RedirectToAction("SysMenu", "Home");
            }

            Processogti reg = new Processogti() {
                Ano = _ano,
                Numero = _numero,
                Observacao = dados[0].Observacao,
                Complemento = dados[0].Complemento,
            };

            Exception ex = processoRepository.Alterar_Processo_Web(reg);

            if (dados[0].Lista_Endereco != null) {
                List<Processoend> _listaE = new List<Processoend>();
                foreach (ProcessoEndStruct end in dados[0].Lista_Endereco) {
                    Processoend regE = new Processoend() {
                        Ano = _ano,
                        Numprocesso = _numero,
                        Codlogr = Convert.ToInt16(end.CodigoLogradouro),
                        Numero = end.Numero
                    };
                    _listaE.Add(regE);
                }
                ex = processoRepository.Incluir_Processo_Endereco(_listaE, _ano, _numero);
            }


            if (dados[0].Lista_Documento != null) {
                List<Processodoc> _listaD = new List<Processodoc>();
                foreach (ProcessoDocStruct doc in dados[0].Lista_Documento) {
                    Processodoc regD = new Processodoc() {
                        Ano = _ano,
                        Numero = _numero,
                        Coddoc = (short)doc.CodigoDocumento
                    };
                    if (Functions.IsDate(doc.DataEntrega))
                        regD.Data = Convert.ToDateTime(doc.DataEntrega);

                    _listaD.Add(regD);
                }
                ex = processoRepository.Incluir_Processo_Documento(_listaD, _ano, _numero);
            }

            //string _p = Functions.Encrypt(_numero_processo);
            //TempData["p"] = _p;
            //return RedirectToAction("Processo_vw");
            return Json(Url.Action("Processo_vw", "Processo", new { p = Functions.Encrypt(_numero_processo) }));

        }

        [Route("Processo_trm")]
        [HttpGet]
        public ActionResult Processo_trm(string p) {
            string _processo;
            if (TempData["p"] == null) {
                if (p == null)
                    return RedirectToAction("sysMenu", "Home");
                else
                    _processo = Functions.Decrypt(p);
            }else
                _processo = Functions.Decrypt(TempData["p"].ToString());

            Processo_bll processoRepository = new Processo_bll(_connection);
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_processo);
            int _ano = processoNumero.Ano;
            int _numero = processoNumero.Numero;

            if (Request.Cookies["2lG1H*"] == null)
                return RedirectToAction("Login", "Home");

            if (_ano == 0)
                RedirectToAction("Login", "Home");

            List<Centrocusto> Lista_CentroCusto = processoRepository.Lista_Local(true, false);
            ViewBag.Lista_CentroCusto = new SelectList(Lista_CentroCusto, "Codigo", "Descricao");

            Processo2ViewModel model = new Processo2ViewModel();
            ProcessoStruct _dados = processoRepository.Dados_Processo(processoNumero.Ano, processoNumero.Numero);
            model.Numero_Processo = _processo;
            model.AnoProcesso = _ano;
            model.NumProcesso = _numero;
            model.Data_Entrada = Convert.ToDateTime(_dados.DataEntrada).ToString("dd/MM/yyyy");
            model.Centro_Custo_Nome = _dados.CentroCustoNome == null ? _dados.NomeCidadao ?? "" : _dados.CentroCustoNome;
            model.Assunto_Codigo =(int) _dados.CodigoAssunto;
            model.Assunto_Nome = _dados.Assunto;
            model.Complemento = _dados.Complemento ?? "";
            return View(model);
        }

        public JsonResult Carrega_Tramite(string processo, int assunto) {
            int _user = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));

            Processo_bll processoRepository = new Processo_bll(_connection);
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(processo);
            List<TramiteStruct> Lista_Tramite = processoRepository.DadosTramite((short)processoNumero.Ano, processoNumero.Numero, assunto);
            return new JsonResult { Data = Lista_Tramite, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult MoveUp(int Ano, int Numero, int Seq) {
            Processo_bll protocoloRepository = new Processo_bll(_connection);
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Acima(Numero, Ano, Seq);
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            return Json(Url.Action("Processo_trm", "Processo", new { p = Functions.Encrypt(Numero_Ano) }));
        }

        public ActionResult MoveDown(int Ano, int Numero, int Seq) {
            Processo_bll protocoloRepository = new Processo_bll(_connection);
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Abaixo(Numero, Ano, Seq);
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            return Json(Url.Action("Processo_trm", "Processo", new { p = Functions.Encrypt(Numero_Ano) }));
        }

        public JsonResult AddPlace(int Ano, int Numero, int Seq, int CCusto) {
            Processo_bll protocoloRepository = new Processo_bll(_connection);
            Exception ex = protocoloRepository.Inserir_Local(Numero, Ano, Seq, CCusto);
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            return Json(Url.Action("Processo_trm", "Processo", new { p = Functions.Encrypt(Numero_Ano) }));
        }

        public ActionResult RemovePlace(int Ano, int Numero, int Seq) {
            Processo_bll protocoloRepository = new Processo_bll(_connection);
            Exception ex = protocoloRepository.Remover_Local(Numero, Ano, Seq);
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            return Json(Url.Action("Processo_trm", "Processo", new { p = Functions.Encrypt(Numero_Ano) }));
        }

        public JsonResult Lista_CCusto() {
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> Lista = processoRepository.Lista_Local(true,false);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Despacho() {
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Despacho> Lista = processoRepository.Lista_Despacho();
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Receber(int Ano, int Numero, int Seq, int Despacho,int CentroCusto) {
            Processo_bll protocoloRepository = new Processo_bll(_connection);
            int _user = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));
            Tramitacao reg = new Tramitacao() {
                Ano = (short)Ano,
                Numero = Numero,
                Seq = (byte)Seq,
                Despacho = (short)Despacho,
                Userid = _user,
                Datahora = DateTime.Now,
                Ccusto = (short)CentroCusto
            };

            Exception ex = protocoloRepository.Receber_Processo(reg);
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            return Json(Url.Action("Processo_trm", "Processo", new { p = Functions.Encrypt(Numero_Ano) }));
        }
    }
}