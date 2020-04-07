using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
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
            if (Functions.pUserId == 0) 
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

            return RedirectToAction("Tramite_Processo2", new { processoViewModel.Ano, processoViewModel.Numero });
        }

        [Route("Tramite_Processo2/{Ano}/{Numero}")]
        [HttpGet]
        public ActionResult Tramite_Processo2(int Ano=0,int Numero=0) {
            ModelState.Clear();

            if (Functions.pUserId == 0)
                return View("../Home/Login");

            if (Ano == 0) 
                RedirectToAction("Login", "Home");

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel modelt = Exibe_Tramite(Numero_Ano);
            if (modelt.Lista_Tramite == null)
                return View("Tramite_Processo");
            else
                return View( modelt);
        }
               
        private ProcessoViewModel Exibe_Tramite(string Numero_Ano,int Seq=0) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            ProcessoViewModel processoViewModel = new ProcessoViewModel();
            int _userId = Functions.pUserId;
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
                    processoViewModel.Despacho_Codigo = Lista_Tramite[0].DespachoCodigo;
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
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano, Numero }));
        }

        public ActionResult MoveDown(int Ano, int Numero, int Seq) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Move_Sequencia_Tramite_Abaixo(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao mover o trâmite";

            ProcessoViewModel processoViewModel = new ProcessoViewModel {
                Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString()
            };
            return Json(Url.Action("Tramite_Processo2", "Protocolo", new { Ano,Numero }));
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
        public ViewResult Receive(int Ano=0, int Numero=0, int Seq=0) {
            if (Functions.pUserId == 0)
                return View("../Home/Login");

            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;

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
                model.Despacho_Codigo = 998;
            }

            if (Functions.pUserId > 0) {
                Tramitacao reg = new Tramitacao() {
                    Ano = (short)model.Ano,
                    Numero = model.Numero,
                    Seq = (byte)model.Seq,
                    Despacho = (short)model.Despacho_Codigo,
                    Userid = Functions.pUserId,
                    Datahora = DateTime.Now,
                    Ccusto = (short)model.CCusto_Codigo
                };
                Exception ex = protocoloRepository.Receber_Processo(reg);
                if (ex != null)
                    ViewBag.Result = "Ocorreu um erro no recebimento do processo";
            }
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

        [Route("Send/{Ano}/{Numero}/{Seq}")]
        [HttpGet]
        public ViewResult Send(int Ano=0, int Numero=0, int Seq=0) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");

            string Numero_Ano = Numero.ToString() + "-" + Functions.RetornaDvProcesso(Numero) + "/" + Ano.ToString();
            ProcessoViewModel processoViewModel = Exibe_Tramite(Numero_Ano, Seq);
            processoViewModel.CCusto_Codigo = processoViewModel.Lista_Tramite[0].CentroCustoCodigo;

            List<Despacho> Lista_Despacho = protocoloRepository.Lista_Despacho();
            ViewBag.Lista_Despacho = new SelectList(Lista_Despacho, "Codigo", "Descricao",processoViewModel.Lista_Tramite[0].DespachoCodigo);

            return View(processoViewModel);
        }

        [Route("Send/{Ano}/{Numero}/{Seq}")]
        [HttpPost]
        public ActionResult Send(ProcessoViewModel model) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");

            if (Functions.pUserId > 0) {
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
                    Userid2 = Functions.pUserId
                };
                Exception ex = protocoloRepository.Enviar_Processo(reg);
                if (ex != null)
                    ViewBag.Result = "Ocorreu um erro no envio do processo";
            }
            model.Numero_Ano = model.Numero.ToString() + "-" + Functions.RetornaDvProcesso(model.Numero) + "/" + model.Ano.ToString();
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

        [Route("AddPlace/{Ano}/{Numero}/{Seq}/{CentroCustoCodigo}")]
        [HttpGet]
        public ViewResult AddPlace(int Ano=0, int Numero=0, int Seq=0) {
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
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

        public ActionResult RemovePlace(int Ano,int Numero,int Seq) {
            Processo_bll protocoloRepository = new Processo_bll("GTIconnection");
            Exception ex = protocoloRepository.Remover_Local(Numero, Ano, Seq);
            if (ex != null)
                ViewBag.Result = "Ocorreu um erro ao remover o local";
            return RedirectToAction("Tramite_Processo2", new {Ano, Numero });
        }

        [Route("Obs/{Ano}/{Numero}/{Seq}")]
        [HttpGet]
        public ViewResult Obs(int Ano, int Numero, int Seq=0) {
            if (Functions.pUserId == 0)
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
            if (Functions.pUserId == 0)
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
            return RedirectToAction("Tramite_Processo2", new { model.Ano, model.Numero });
        }

    }
}