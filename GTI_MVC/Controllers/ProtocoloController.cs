using Antlr.Runtime.Tree;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using GTI_MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class ProtocoloController : Controller {

        [Route("Tramite_Processo")]
        [HttpGet]
        public ViewResult Tramite_Processo() {
            ProcessoViewModel model = new ProcessoViewModel();
            if (Session["hashid"] == null) 
                return View("../Home/Login");
            else 
                return View(model);
        }

        [Route("Tramite_Processo")]
        [HttpPost]
        public ActionResult Tramite_Processo(ProcessoViewModel model) {
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(model.Numero_Ano);
            processoViewModel.Numero_Ano = model.Numero_Ano;
            processoViewModel.Numero = processoNumero.Numero;
            processoViewModel.Ano = processoNumero.Ano;
            processoViewModel.User_Id = Convert.ToInt32(ViewBag.UserId);

            return RedirectToAction("Tramite_Processo2", new {Ano=Functions.Encrypt( processoViewModel.Ano.ToString()),Numero=Functions.Encrypt( processoViewModel.Numero.ToString()) });
        }

        [Route("Tramite_Processo2/{Ano}/{Numero}")]
        [HttpGet]
        public ActionResult Tramite_Processo2(string Ano="0",string Numero="0") {
            int _ano = 0,_numero=0;
            try {
                _ano = Convert.ToInt32(Functions.Decrypt(Ano));
                _numero = Convert.ToInt32(Functions.Decrypt(Numero));
            } catch (Exception) {
                RedirectToAction("Login", "Home");
            }
            ModelState.Clear();

            if (Session["hashid"] == null)
                return View("../Home/Login");

            if (_ano == 0) 
                RedirectToAction("Login", "Home");

            string Numero_Ano = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            ProcessoViewModel modelt = Exibe_Tramite(Numero_Ano,0);
            if (modelt.Lista_Tramite == null)
                return View("Tramite_Processo");
            else
                return View( modelt);
        }
               
        private ProcessoViewModel Exibe_Tramite(string Numero_Ano,int Seq=0) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            //int _userId = Functions.pUserId;
            int _userId = Convert.ToInt32(Session["hashid"]);
            if (_userId > 0) {

                List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentrocustoUsuario(_userId);
                string Lista_CC = "";
                foreach (UsuariocentroCusto item in _listaCC) {
                    Lista_CC += item.Codigo + ",";
                }
                Lista_CC = Lista_CC.Substring(0, Lista_CC.Length - 1);


                List<Centrocusto> Lista_CentroCusto = protocoloRepository.Lista_Local(true, false);
                ViewBag.Lista_CentroCusto = new SelectList(Lista_CentroCusto, "Codigo", "Descricao");

                ProcessoNumero processoNumero = Functions.Split_Processo_Numero(Numero_Ano);
                ProcessoStruct _dados = protocoloRepository.Dados_Processo(processoNumero.Ano, processoNumero.Numero);
                if (_dados != null) {
                    List<TramiteStruct> Lista_Tramite = protocoloRepository.DadosTramite((short)processoNumero.Ano, processoNumero.Numero, (int)_dados.CodigoAssunto);

                    if (Seq > 0) {
                        Lista_Tramite = Lista_Tramite.Where(m => m.Seq == Seq).ToList();
                    }
                    TramiteStruct TramiteAtual = protocoloRepository.Dados_Tramite(processoNumero.Ano, processoNumero.Numero, Seq);


                    processoViewModel.Despacho_Codigo = TramiteAtual.DespachoCodigo;
                    processoViewModel.Ano = processoNumero.Ano;
                    processoViewModel.Numero = processoNumero.Numero;
                    processoViewModel.User_Id = Convert.ToInt32(ViewBag.UserId);
                    processoViewModel.Data_Processo = Convert.ToDateTime(_dados.DataEntrada).ToString("dd/MM/yyyy");
                    processoViewModel.Requerente = _dados.NomeCidadao;
                    processoViewModel.Assunto_Nome = _dados.Assunto;
                    processoViewModel.Lista_Tramite = Lista_Tramite;
                    processoViewModel.Lista_CC = Lista_CC;
                    processoViewModel.Numero_Ano = Numero_Ano;
                    processoViewModel.ObsGeral = Lista_Tramite[0].ObsGeral;
                    processoViewModel.ObsInterna = Lista_Tramite[0].ObsInterna;
                } else {
                    ViewBag.Result = "Processo não cadastrado.";
                }
            }
            return processoViewModel;
        }
     
        public ActionResult MoveUp(int Ano,int Numero,int Seq) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Acima(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao mover o trâmite";


            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString()
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano = Functions.Encrypt(Ano.ToString()), Numero = Functions.Encrypt(Numero.ToString()) }));
        }

        public ActionResult MoveDown(int Ano, int Numero, int Seq) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Abaixo(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao mover o trâmite";

            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString()
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano = Functions.Encrypt(Ano.ToString()), Numero = Functions.Encrypt(Numero.ToString()) }));
        }

        public ActionResult Inserir_Save(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Inserir_Local(model.Numero, model.Ano, model.Seq, (int)model.CCusto_Codigo);
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            ProcessoViewModel processoViewModel = new ProcessoViewModel() {
                Numero_Ano = model.Numero_Ano
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { processoViewModel.Ano, processoViewModel.Numero }));
        }

        public ActionResult Alterar_Obs(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Alterar_Obs(model.Ano, model.Numero, model.Seq, model.ObsGeral,model.ObsInterna);
            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString()
            };
            return RedirectToAction("Tramite_Processo2", new { processoViewModel.Ano, processoViewModel.Numero });
        }

        /****************************************************************
         * 
         * **************************************************************/
        [Route("Receive/{Ano}/{Numero}/{Seq}")]
        [HttpGet]
        public ViewResult Receive(string p1 = "", string p2 = "", string p3 = "") {
            if (Session["hashid"] == null) {
                LoginViewModel model = new LoginViewModel();
                return View("../Home/Login", model);
            }

            int _ano = 0, _numero = 0, _seq = 0;
            try {
                _ano = Convert.ToInt32(Functions.Decrypt(p1)); _numero = Convert.ToInt32(Functions.Decrypt(p2)); _seq = Convert.ToInt32(Functions.Decrypt(p3));
            } catch {
                LoginViewModel model = new LoginViewModel();
                return View("../Home/Login",model);
            }

            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            string Numero_Ano = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, _seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;
            processoViewModel.Seq = processoViewModel.Lista_Tramite[0].Seq;
            List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentrocustoUsuario(Convert.ToInt32(ViewBag.UserId));
 
            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao");
            
            return View(processoViewModel);
        }

        [Route("Receive/{Ano}/{Numero}/{Seq}")]
        [HttpPost]
        public ActionResult Receive(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");

            if (model.Despacho_Codigo == null) {
                model.Despacho_Codigo = 0;
            }

            if (Session["hashid"]!= null) {
                if (model.Despacho_Codigo > 0) {
                    Tramitacao reg = new Tramitacao() {
                        Ano = (short)model.Ano,
                        Numero = model.Numero,
                        Seq = (byte)model.Seq,
                        Despacho = (short)model.Despacho_Codigo,
                        Userid =  Convert.ToInt32(Session["hashid"]),
                        Datahora = DateTime.Now,
                        Ccusto = (short)model.CCusto_Codigo
                    };
                    Exception ex = protocoloRepository.Receber_Processo(reg);
                    if (ex != null)
                        ViewBag.Result = "Ocorreu um erro no recebimento do processo";
                }
            }
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { Ano = Functions.Encrypt(model.Ano.ToString()), Numero = Functions.Encrypt(model.Numero.ToString()) });
        }

        [Route("Send/{Ano}/{Numero}/{Seq}")]
        [HttpGet]
        public ViewResult Send(string p1="", string p2="", string p3="") {
            if (Session["hashid"] == null) {
                LoginViewModel model = new LoginViewModel();
                return View("../Home/Login", model);
            }
            int _ano = 0, _numero = 0, _seq = 0;
            try {
                _ano = Convert.ToInt32(Functions.Decrypt(p1)); _numero = Convert.ToInt32(Functions.Decrypt(p2)); _seq = Convert.ToInt32(Functions.Decrypt(p3));
            } catch  {
                LoginViewModel model = new LoginViewModel();
                return View("../Home/Login", model);
            }

            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            string Numero_Ano = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, _seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;
            processoViewModel.Seq = processoViewModel.Lista_Tramite[0].Seq;
            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao",processoViewModel.Lista_Tramite[0].DespachoCodigo);

            return View(processoViewModel);
        }

        [Route("Send/{Ano}/{Numero}/{Seq}")]
        [HttpPost]
        public ActionResult Send(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");

            if (Session["hashid"] != null) {

                List<TramiteStruct> _regOld = protocoloRepository.DadosTramite((short)model.Ano, model.Numero, model.Seq);
                Tramitacao reg = new Tramitacao() {
                    Ano = (short)model.Ano,
                    Numero = model.Numero,
                    Seq = (byte)model.Seq,
                    Despacho = (short)model.Despacho_Codigo,
                    Userid = _regOld[0].Userid1,
                    Datahora = Convert.ToDateTime(_regOld[0].DataEntrada + " " + _regOld[0].HoraEntrada),
                    Ccusto = _regOld[0].CentroCustoCodigo,
                    Dataenvio = DateTime.Now,
                    Userid2 = Convert.ToInt32(Session["hashid"])
                };
                Exception ex = protocoloRepository.Enviar_Processo(reg);
                if (ex != null)
                    ViewBag.Result = "Ocorreu um erro no envio do processo";
            }
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { Ano = Functions.Encrypt(model.Ano.ToString()), Numero = Functions.Encrypt(model.Numero.ToString()) });
        }

        [Route("AddPlace/{Ano}/{Numero}/{Seq}/{CentroCustoCodigo}")]
        [HttpGet]
        public ViewResult AddPlace(int Ano = 0, int Numero = 0, int Seq = 0) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);

            return View(processoViewModel);
        }

        [Route("AddPlace/{Ano}/{Numero}/{Seq}/{CentroCustoCodigo}")]
        [HttpPost]
        public ActionResult AddPlace(ProcessoViewModel model) {
            if (model.CCusto_Codigo != null) {
                Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
                Exception ex = protocoloRepository.Inserir_Local(model.Numero, model.Ano, model.Seq, (int)model.CCusto_Codigo);
                if (ex != null)
                    ViewBag.Result = "Ocorreu um erro ao inserir um local";
                model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            }
            return RedirectToAction("Tramite_Processo2", new { Ano = Functions.Encrypt(model.Ano.ToString()), Numero = Functions.Encrypt(model.Numero.ToString()) });
        }

        public ActionResult RemovePlace(int Ano,int Numero,int Seq) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Remover_Local(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao remover o local";
            return RedirectToAction("Tramite_Processo2", new { Ano = Functions.Encrypt(Ano.ToString()), Numero = Functions.Encrypt(Numero.ToString()) });
        }

        [Route("Obs/{Ano}/{Numero}/{Seq}")]
        [HttpGet]
        public ViewResult Obs(int Ano, int Numero, int Seq=0) {
            if (Session["hashid"] == null)
                return View("../Home/Login");

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;
            processoViewModel.ObsGeral = processoViewModel.ObsGeral ?? "";
            processoViewModel.ObsInterna = processoViewModel.ObsInterna ?? "";

            return View(processoViewModel);
        }

        [Route("Obs/{Ano}/{Numero}/{Seq}")]
        [HttpPost]
        public ActionResult Obs(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            if (Session["hashid"] == null)
                return Json(Url.Action("Login_gti", "Home"));

            Tramitacao reg = new Tramitacao() {
                Ano = (short)model.Ano,
                Numero = model.Numero,
                Seq = (byte)model.Seq,
                Obs=model.ObsGeral,
                Obsinterna=model.ObsInterna
            };
            Exception ex = protocoloRepository.Alterar_Obs(reg.Ano,reg.Numero,reg.Seq,reg.Obs,reg.Obsinterna);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro na observação do trâmite";

            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { Ano = Functions.Encrypt(model.Ano.ToString()), Numero = Functions.Encrypt(model.Numero.ToString()) });
        }

        [Route("Consulta_Processo")]
        [HttpGet]
        public ViewResult Consulta_Processo() {
            ProcessoViewModel model = new ProcessoViewModel();
            return View(model);
        }

        [Route("Consulta_Processo")]
        [HttpPost]
        public ActionResult Consulta_Processo(ProcessoViewModel model) {
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, Session["CaptchaCode"].ToString())) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }

            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(model.Numero_Ano);
            processoViewModel.Numero_Ano = model.Numero_Ano;
            processoViewModel.Numero = processoNumero.Numero;
            processoViewModel.Ano = processoNumero.Ano;
            processoViewModel.User_Id = Convert.ToInt32(ViewBag.UserId);

            return RedirectToAction("Consulta_Processo2", new { Ano = Functions.Encrypt(processoViewModel.Ano.ToString()), Numero = Functions.Encrypt(processoViewModel.Numero.ToString()) });
        }

        [Route("Consulta_Processo2/{Ano}/{Numero}")]
        [HttpGet]
        public ActionResult Consulta_Processo2(string Ano = "0", string Numero = "0") {
            int _ano = 0, _numero = 0;
            try {
                _ano = Convert.ToInt32(Functions.Decrypt(Ano));
                _numero = Convert.ToInt32(Functions.Decrypt(Numero));
            } catch  {
               
            }

            if (_ano == 0)
                return View("Consulta_Processo");

            string Numero_Ano = _numero.ToString() + "-" + Functions.RetornaDvProcesso(_numero) + "/" + _ano.ToString();
            ProcessoViewModel modelt = Exibe_Tramite2(Numero_Ano);
            modelt.Numero_Ano = Numero_Ano;
            if (modelt.Lista_Tramite == null)
                return View("Consulta_Processo");
            else { 

                return View(modelt);
            }
                
        }

        private ProcessoViewModel Exibe_Tramite2(string Numero_Ano, int Seq = 0) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            ProcessoViewModel processoViewModel = new ProcessoViewModel();


            List<Centrocusto> Lista_CentroCusto = protocoloRepository.Lista_Local(true, false);
            ViewBag.Lista_CentroCusto = new SelectList(Lista_CentroCusto, "Codigo", "Descricao");

            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(Numero_Ano);
            ProcessoStruct _dados = protocoloRepository.Dados_Processo(processoNumero.Ano, processoNumero.Numero);
            if (_dados != null) {
                List<TramiteStruct> Lista_Tramite = protocoloRepository.DadosTramite((short)processoNumero.Ano, processoNumero.Numero, (int)_dados.CodigoAssunto);

                if (Seq > 0) {
                    Lista_Tramite = Lista_Tramite.Where(m => m.Seq == Seq).ToList();
                }
                TramiteStruct TramiteAtual = protocoloRepository.Dados_Tramite(processoNumero.Ano, processoNumero.Numero, Seq);


                processoViewModel.Despacho_Codigo = TramiteAtual.DespachoCodigo;
                processoViewModel.Ano = processoNumero.Ano;
                processoViewModel.Numero = processoNumero.Numero;
                processoViewModel.User_Id = Convert.ToInt32(ViewBag.UserId);
                processoViewModel.Data_Processo = Convert.ToDateTime(_dados.DataEntrada).ToString("dd/MM/yyyy");
                processoViewModel.Requerente = _dados.NomeCidadao;
                processoViewModel.Assunto_Nome = _dados.Assunto;
                processoViewModel.Lista_Tramite = Lista_Tramite;
                processoViewModel.Numero_Ano = Numero_Ano;
                processoViewModel.ObsGeral = Lista_Tramite[0].ObsGeral;
                processoViewModel.ObsInterna = Lista_Tramite[0].ObsInterna;
            } else {
                ViewBag.Result = "Processo não cadastrado.";
            }
            return processoViewModel;
        }

        [Route("ProcessoMnu")]
        [HttpGet]
        public ActionResult ProcessoMnu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("ProcessoData")]
        [HttpGet]
        public ActionResult ProcessoData(string c) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            string _numStr;
            try {
                _numStr = Functions.Decrypt(c);
            } catch {
                return RedirectToAction("Login", "Home");
            }

            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(_numStr);
            int _numero = processoNumero.Numero;
            int _ano = processoNumero.Ano;

            ProcessoViewModel modelt = Exibe_Tramite2(_numStr);
            Processo_bll processoRepository = new Processo_bll("GTIconnection");
            ProcessoStruct _dados = processoRepository.Dados_Processo(_ano, _numero);
            modelt.Observacao = _dados.Observacao ?? "";
            List<ProcessoEndStruct> ListaEnd = _dados.ListaProcessoEndereco;
            string _end = "";
            foreach (ProcessoEndStruct item in ListaEnd) {
                _end += item.NomeLogradouro + ", " + item.Numero.ToString() + "; ";
            }

            modelt.Endereco_Ocorrencia = _end;
            modelt.Numero_Ano = _numStr;
            return View(modelt);

        }


        [Route("ProcessoqryC")]
        [HttpGet]
        public ActionResult ProcessoqryC(string id) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ProcessoViewModel model = new ProcessoViewModel();
            if (string.IsNullOrEmpty(id))
                model.Lista_Processo = new List<ProcessoLista>();
            else {
                return RedirectToAction("ProcessoData", new { c = id.Replace('-', '/') });
            }
            return View(model);
        }

        [Route("ProcessoqryC")]
        [HttpPost]
        public ActionResult ProcessoqryC(ProcessoViewModel model) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            Processo_bll processoRepository = new Processo_bll("GTIconnection");

            bool _filterN = false;
            if (!string.IsNullOrEmpty(model.Numero_Ano)) _filterN = true;

            int _ano = 0,_numero = 0;
            List<ProcessoLista> _lista = new List<ProcessoLista>();
            model.Lista_Processo = _lista;
            if (_filterN) {
                ProcessoNumero processoNumero = Functions.Split_Processo_Numero(model.Numero_Ano);
                string _numStr = model.Numero_Ano;
                _numero = processoNumero.Numero;
                _ano = processoNumero.Ano;

                short _dv = processoRepository.ExtractDvProcesso(_numStr);
                short _realdv = Convert.ToInt16(processoRepository.DvProcesso(_numero));
                if (_dv != _realdv) {
                    ViewBag.Result = "Nº de processo inválido.";
                    return View(model);
                }

                bool _existe = processoRepository.Existe_Processo(_ano, _numero);
                if (!_existe) {
                    ViewBag.Result = "Nº de processo não cadastrado.";
                    return View(model);
                }
            }

            string _nome = model.Requerente;
            string _endereco = model.Endereco;
            if (!string.IsNullOrEmpty(_nome)){
                if (_nome.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do nome.";
                    return View(model);
                }
            }
            if (!string.IsNullOrEmpty(_endereco)) {
                if (_endereco.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do endereco.";
                    return View(model);
                }
            }

            List<ProcessoStruct> ListaProcesso = processoRepository.Lista_Processos(_ano,_numero,model.Requerente,model.Endereco,model.EnderecoNumero);
            if (ListaProcesso.Count == 0) {
                ViewBag.Result = "Não foi localizado nenhum processo com este requerente.";
                return View(model);
            }

            foreach (ProcessoStruct item in ListaProcesso) {
                ProcessoLista reg = new ProcessoLista() {
                    AnoNumero = item.Numero.ToString("00000") +"-"+ Functions.RetornaDvProcesso(item.Numero).ToString() + "/" + item.Ano.ToString(),
                    Requerente = Functions.TruncateTo(item.NomeCidadao, 30),
                    Assunto = Functions.TruncateTo(item.Assunto, 30),
                    Endereco = string.IsNullOrEmpty(item.LogradouroNome) ? "" : item.LogradouroNome
                };
                if (reg.Endereco != "") {
                    reg.Endereco += ", " + item.LogradouroNumero.ToString();
                    reg.Endereco = Functions.TruncateTo(reg.Endereco, 32);
                }
                _lista.Add(reg);
            }

            model.Lista_Processo = _lista;
            return View(model);


        }


    }
}