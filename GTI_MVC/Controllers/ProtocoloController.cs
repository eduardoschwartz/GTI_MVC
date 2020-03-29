using GTI_Mvc.Interfaces;
using GTI_Mvc.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GTI_Mvc.Controllers {
    public class ProtocoloController : Controller {
        private readonly IProtocoloRepository protocoloRepository;

        public ProtocoloController(IProtocoloRepository protocoloRepository ) {
            this.protocoloRepository = protocoloRepository;
        }

        [Route("Tramite_Processo")]
        [HttpGet]
        public ViewResult Tramite_Processo() {

            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
                return View();
            }
        }

        [Route("Tramite_Processo")]
        [HttpPost]
        public ActionResult Tramite_Processo(ProcessoViewModel model) {
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext)) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(model);
            }
            ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
            ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
            ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());

            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(model.Numero_Ano);
            processoViewModel.Numero_Ano = model.Numero_Ano;
            processoViewModel.Numero = processoNumero.Numero;
            processoViewModel.Ano = processoNumero.Ano;
            processoViewModel.User_Id = Convert.ToInt32(ViewBag.UserId);

            return RedirectToAction("Tramite_Processo2", new { processoViewModel.Ano, processoViewModel.Numero });
        }

        [HttpGet("Tramite_Processo2/{Ano}/{Numero}")]
        public ActionResult Tramite_Processo2(int Ano,int Numero) {
            ModelState.Clear();
            if (Ano == 0)
                return View();

            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
            }

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel modelt = Exibe_Tramite(Numero_Ano);
            if (modelt.Lista_Tramite == null)
                return View("Tramite_Processo");
            else
                return View( modelt);
        }
               
        private ProcessoViewModel Exibe_Tramite(string Numero_Ano,int Seq=0) {
            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            int _userId = Convert.ToInt32(Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString()));

            List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentroCustoUsuario(_userId);
            string Lista_CC = "";
            foreach (UsuariocentroCusto item in _listaCC) {
                Lista_CC += item.Codigo + ",";
            }
            Lista_CC = Lista_CC.Substring(0, Lista_CC.Length - 1);
            

            List<Centrocusto> Lista_CentroCusto = protocoloRepository.Lista_CentroCusto();
            ViewBag.Lista_CentroCusto = new SelectList(Lista_CentroCusto, "Codigo", "Descricao");

            ProcessoNumero processoNumero = Functions.Split_Processo_Numero(Numero_Ano);
            ProcessoStruct _dados = protocoloRepository.Dados_Processo(processoNumero.Ano, processoNumero.Numero);
            if (_dados != null) {
                List<TramiteStruct> Lista_Tramite = protocoloRepository.DadosTramite((short)processoNumero.Ano, processoNumero.Numero, (int)_dados.CodigoAssunto);
                if (Seq > 0) {
                    Lista_Tramite = Lista_Tramite.Where(m => m.Seq == Seq).ToList();

                }

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
            return processoViewModel;
        }
     
        public ActionResult MoveUp(int Ano,int Numero,int Seq) {
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Acima(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao mover o trâmite";


            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString()
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano, Numero }));
        }

        public ActionResult MoveDown(int Ano, int Numero, int Seq) {
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Abaixo(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao mover o trâmite";

            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString()
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano,Numero }));
        }

        //public ActionResult Inserir_Save(ProcessoViewModel model) {
        //    int _user_Id = Convert.ToInt32(Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString()));
        //    Exception ex = protocoloRepository.Inserir_Local(model.Numero, model.Ano, model.Seq,(int)model.CCusto_Codigo);
        //    model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
        //    ProcessoViewModel processoViewModel = new ProcessoViewModel() {
        //        Numero_Ano=model.Numero_Ano
        //    };
        //    return RedirectToAction("Tramite_Processo2", new { processoViewModel.Ano, processoViewModel.Numero });
        //}


        //public ActionResult Alterar_Obs(ProcessoViewModel model) {
        //    Exception ex = protocoloRepository.Alterar_Obs( model.Ano,model.Numero, model.Seq,model.Obs);
        //    ProcessoViewModel processoViewModel = new ProcessoViewModel {
        //        Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString()
        //    };
        //    return RedirectToAction("Tramite_Processo2", new { processoViewModel.Ano, processoViewModel.Numero });
        //}
        
        /****************************************************************
         * 
         * **************************************************************/
        [HttpGet("Receive/{Ano}/{Numero}/{Seq}")]
        public ViewResult Receive(int Ano, int Numero, int Seq) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
            }

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;

            List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentroCustoUsuario(Convert.ToInt32(ViewBag.UserId));
            bool _find = false;
            foreach (UsuariocentroCusto item in _listaCC) {
                if (item.Codigo == processoViewModel.CCusto_Codigo) {
                    _find = true;
                    break;
                }
            }

            bool _recebido = false,_enviado=true;
            if (Seq > 1) {
                _recebido = protocoloRepository.Tramite_Recebido(Ano, Numero, Seq);
                if (!_recebido) {
                    _enviado = protocoloRepository.Tramite_Enviado(Ano, Numero, Seq-1);
                }
            }

            if (!_find || _recebido || !_enviado) {
                HttpContext.Session.Clear();
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            }

            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao");

            return View(processoViewModel);
        }

        [HttpPost("Receive/{Ano}/{Numero}/{Seq}")]
        public ActionResult Receive(ProcessoViewModel model) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return Json(Url.Action("Tramite_Processo2", "Protocolo", new { model.Ano, model.Numero}));
            }

            int _user_Id = Convert.ToInt32(Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString()));
            Tramitacao reg = new Tramitacao() {
                Ano = (short)model.Ano,
                Numero = model.Numero,
                Seq = (byte)model.Seq,
                Despacho = (short)model.Despacho_Codigo,
                Userid = _user_Id,
                Datahora = DateTime.Now,
                Ccusto = (short)model.CCusto_Codigo
            };
            Exception ex = protocoloRepository.Receber_Processo(reg);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro no recebimento do processo";

            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

        [HttpGet("Send/{Ano}/{Numero}/{Seq}")]
        public ViewResult Send(int Ano, int Numero, int Seq) {

            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
            }

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            //processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;

            //List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentroCustoUsuario(Convert.ToInt32(ViewBag.UserId));
            //bool _find = false;
            //foreach (UsuariocentroCusto item in _listaCC) {
            //    if (item.Codigo == processoViewModel.CCusto_Codigo) {
            //        _find = true;
            //        break;
            //    }
            //}

            //bool _recebido = true, _enviado = false;
            //if (Seq > 1) {
            //    _recebido = protocoloRepository.Tramite_Recebido(Ano, Numero, Seq);
            //    if (_recebido) {
            //        _enviado = protocoloRepository.Tramite_Enviado(Ano, Numero, Seq - 1);
            //    }
            //}

            //if (!_find || !_recebido || _enviado) {
            //    HttpContext.Session.Clear();
            //    ViewBag.LoginName = "";
            //    ViewBag.FullName = "Visitante";
            //    return View("../Home/Login");
            //}

                       

            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao",processoViewModel.Lista_Tramite[0].DespachoCodigo);


            return View(processoViewModel);
        }

        [HttpPost("Send/{Ano}/{Numero}/{Seq}")]
        public ActionResult Send(ProcessoViewModel model) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return RedirectToAction("Index", "Home");
            }

            int _user_Id = Convert.ToInt32(Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString()));


            List<TramiteStruct> _regOld = protocoloRepository.DadosTramite((short)model.Ano, model.Numero, model.Seq);

            Tramitacao reg = new Tramitacao() {
                Ano = (short)model.Ano,
                Numero = model.Numero,
                Seq = (byte)model.Seq,
                Despacho = model.Despacho_Codigo == null ? _regOld[0].DespachoCodigo : (short)model.Despacho_Codigo,
                Userid = _regOld[0].Userid1,
                Datahora = Convert.ToDateTime(_regOld[0].DataEntrada + " " + _regOld[0].HoraEntrada),
                Ccusto = _regOld[0].CentroCustoCodigo,
                Dataenvio = DateTime.Now,
                Userid2 = _user_Id
            };
            Exception ex = protocoloRepository.Enviar_Processo(reg);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro no envio do processo";

            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

        [HttpGet("AddPlace/{Ano}/{Numero}/{Seq}/{CentroCustoCodigo}")]
        public ViewResult AddPlace(int Ano, int Numero, int Seq) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
            }

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao", processoViewModel.Lista_Tramite[0].DespachoCodigo);

            return View(processoViewModel);
        }

        [HttpPost("AddPlace/{Ano}/{Numero}/{Seq}/{CentroCustoCodigo}")]
        public ActionResult AddPlace(ProcessoViewModel model) {

            Exception ex = protocoloRepository.Inserir_Local(model.Numero, model.Ano, model.Seq, (int)model.CCusto_Codigo);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao inserir um local";
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { model.Ano, model.Numero }));
        }

        public ActionResult RemovePlace(ProcessoViewModel model) {
            Exception ex = protocoloRepository.Remover_Local(model.Numero, model.Ano, model.Seq, (int)model.CCusto_Codigo);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao remover o local";

            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            ProcessoViewModel processoViewModel = new ProcessoViewModel() {
                Numero_Ano = model.Numero_Ano
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { model.Ano, model.Numero }));
        }

        [HttpGet("Obs/{Ano}/{Numero}/{Seq}")]
        public ViewResult Obs(int Ano, int Numero, int Seq) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            } else {
                ViewBag.LoginName = Functions.Decrypt(HttpContext.Session.["gti_V3login"].ToString());
                ViewBag.FullName = Functions.Decrypt(HttpContext.Session.["gti_V3full"].ToString());
                ViewBag.UserId = Functions.Decrypt(HttpContext.Session.["gti_V3id"].ToString());
            }

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;

            List<UsuariocentroCusto> _listaCC = protocoloRepository.ListaCentroCustoUsuario(Convert.ToInt32(ViewBag.UserId));
            bool _find = false;
            foreach (UsuariocentroCusto item in _listaCC) {
                if (item.Codigo == processoViewModel.CCusto_Codigo) {
                    _find = true;
                    break;
                }
            }

            if (!_find) {
                HttpContext.Session.Clear();
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return View("../Home/Login");
            }

            return View(processoViewModel);
        }

        [HttpPost("Obs/{Ano}/{Numero}/{Seq}")]
        public ActionResult Obs(ProcessoViewModel model) {
            if (string.IsNullOrWhiteSpace(HttpContext.Session.["gti_V3id"].ToString())) {
                ViewBag.LoginName = "";
                ViewBag.FullName = "Visitante";
                return Json(Url.Action( "Index", "Home"));
            }

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
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

    }
}