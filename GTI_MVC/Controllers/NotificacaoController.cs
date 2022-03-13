using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Models.ReportModels;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers {
    public class NotificacaoController : Controller {

        private readonly string _connection = "GTIconnection";

        [Route("Notificacao_ter_menu")]
        [HttpGet]
        public ActionResult Notificacao_ter_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
            }

        [Route("Notificacao_ter_add")]
        [HttpGet]
        public ActionResult Notificacao_ter_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            return View(model);
            }

        [Route("Notificacao_ter_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_ter_add(NotificacaoTerViewModel model, string action) {
            if (model.Codigo_Imovel == 0) {
                ViewBag.Result = "Código de imóvel inválido.";
                return View(model);
                }
            int _codigo = model.Codigo_Imovel;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (action == "btnCodigoOK") {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                List<ProprietarioStruct> Listaprop = imovelRepository.Lista_Proprietario(_codigo, false);
                if (Listaprop.Count == 0) {
                    ViewBag.Result = "Não é possível emitir notificação para este imóvel.";
                    model = new NotificacaoTerViewModel();
                    return View(model);
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && prop.Principal) {
                        model.Codigo_cidadao = prop.Codigo;
                        model.Nome_Proprietario = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg = _cidadao.Rg;
                        model.Cpf = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && !prop.Principal) {
                        model.Codigo_cidadao2 = prop.Codigo;
                        model.Nome_Proprietario2 = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg2 = _cidadao.Rg;
                        model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                if (string.IsNullOrEmpty(model.Nome_Proprietario2) && Listaprop.Count > 1) {
                    foreach (ProprietarioStruct prop in Listaprop) {
                        if (prop.Tipo != "P") {
                            model.Codigo_cidadao2 = prop.Codigo;
                            model.Nome_Proprietario2 = prop.Nome;
                            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                            model.Rg2 = _cidadao.Rg;
                            model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                            model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf2);
                            break;
                            }
                        }
                    }

                model.Inscricao = _imovel.Inscricao;
                EnderecoStruct _endLocal = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                string _compl = _endLocal.Complemento == null ? "" : " " + _endLocal.Complemento;
                model.Endereco_Local = _endLocal.Endereco + ", " + _endLocal.Numero.ToString() + _compl + " - " + _endLocal.NomeBairro.ToString() + " - " + _endLocal.NomeCidade + "/" + _endLocal.UF + " Cep:" + _endLocal.Cep;

                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                Contribuinte_Header_Struct _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao);
                _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                model.Endereco_Prop = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;

                if (model.Codigo_cidadao2 > 0) {
                    _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao2);
                    _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                    model.Endereco_prop2 = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;
                    }

                EnderecoStruct _endEntrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                if (_endEntrega.Endereco != null) {
                    _compl = _endEntrega.Complemento == null ? "" : " " + _endEntrega.Complemento;
                    model.Endereco_Entrega = _endEntrega.Endereco + ", " + _endEntrega.Numero.ToString() + _compl + " - " + _endEntrega.NomeBairro??"" + " - " + _endEntrega.NomeCidade + "/" + _endEntrega.UF + " Cep:" + _endEntrega.Cep;
                    } else {
                    if (_imovel.EE_TipoEndereco == 0)
                        model.Endereco_Entrega = model.Endereco_Local;
                    else {
                        model.Endereco_Entrega = model.Endereco_Prop;
                        }
                    }
                }

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
                }
            if (action == "btnValida") {
                bool _existe = imovelRepository.Existe_Notificacao_Terreno(model.Ano_Notificacao, model.Numero_Notificacao);
                if (_existe) {
                    ViewBag.Result = "Nº de notificação já cadastrado.";
                    return View(model);
                    } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                        }
                    Save_Notificacao_Terreno(model);
                    return RedirectToAction("Notificacao_ter_query");
                    }
                }

            return View(model);
            }

        private ActionResult Save_Notificacao_Terreno(NotificacaoTerViewModel model) {
            Notificacao_terreno reg = new Notificacao_terreno() {
                Ano_not = model.Ano_Notificacao,
                Numero_not = model.Numero_Notificacao,
                Codigo = model.Codigo_Imovel,
                Inscricao = model.Inscricao,
                Endereco_entrega = model.Endereco_Entrega,
                Endereco_entrega2 = model.Endereco_entrega2,
                Endereco_infracao = model.Endereco_Local,
                Endereco_prop = model.Endereco_Prop,
                Endereco_prop2 = model.Endereco_prop2,
                Prazo = model.Prazo,
                Nome = model.Nome_Proprietario,
                Situacao = 3,//concluido
                Userid = Convert.ToInt32(Session["hashid"]),
                Data_cadastro = DateTime.Now,
                Nome2 = model.Nome_Proprietario2,
                Codigo_cidadao = model.Codigo_cidadao,
                Codigo_cidadao2 = model.Codigo_cidadao2,
                Cpf = model.Cpf,
                Cpf2 = model.Cpf2,
                Rg = model.Rg,
                Rg2 = model.Rg2
                };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_notificacao_terreno(reg);
            return null;

            }

        [Route("Notificacao_ter_query")]
        [HttpGet]
        public ActionResult Notificacao_ter_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Terreno_Struct> _listaNot = imovelRepository.Lista_Notificacao_Terreno(DateTime.Now.Year);
            foreach (Notificacao_Terreno_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                    };
                ListaNot.Add(reg);
                }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
            }

        [Route("Notificacao_ter_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_ter_query(NotificacaoTerQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Terreno_Struct> _listaNot = imovelRepository.Lista_Notificacao_Terreno(_ano);
            foreach (Notificacao_Terreno_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                    };
                ListaNot.Add(reg);
                }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;
            return View(model);
            }

        public ActionResult Notificacao_terreno_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Notificacao_Terreno_Struct _not = imovelRepository.Retorna_Notificacao_Terreno(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumero = _not.AnoNumero,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Prazo = _not.Prazo,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                PrazoText = Functions.Escrever_Valor_Extenso(_not.Prazo),
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? "",
                Data_Cadastro = _not.Data_Cadastro
                };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
                }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Notificacao_Terreno.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Notificacao_Terreno.pdf");
                } catch {

                throw;
                }

            }

        [Route("Notificacao_obra_menu")]
        [HttpGet]
        public ActionResult Notificacao_obra_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
            }

        [Route("Notificacao_obra_add")]
        [HttpGet]
        public ActionResult Notificacao_obra_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            return View(model);
            }

        [Route("Notificacao_obra_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_obra_add(NotificacaoTerViewModel model, string action) {
            if (model.Codigo_Imovel == 0) {
                ViewBag.Result = "Código de imóvel inválido.";
                return View(model);
                }
            int _codigo = model.Codigo_Imovel;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (action == "btnCodigoOK") {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                List<ProprietarioStruct> Listaprop = imovelRepository.Lista_Proprietario(_codigo, false);
                if (Listaprop.Count == 0) {
                    ViewBag.Result = "Não é possível emitir notificação para este imóvel.";
                    model = new NotificacaoTerViewModel();
                    return View(model);
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && prop.Principal) {
                        model.Codigo_cidadao = prop.Codigo;
                        model.Nome_Proprietario = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg = _cidadao.Rg;
                        model.Cpf = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && !prop.Principal) {
                        model.Codigo_cidadao2 = prop.Codigo;
                        model.Nome_Proprietario2 = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg2 = _cidadao.Rg;
                        model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                if (string.IsNullOrEmpty(model.Nome_Proprietario2) && Listaprop.Count > 1) {
                    foreach (ProprietarioStruct prop in Listaprop) {
                        if (prop.Tipo != "P") {
                            model.Codigo_cidadao2 = prop.Codigo;
                            model.Nome_Proprietario2 = prop.Nome;
                            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                            model.Rg2 = _cidadao.Rg;
                            model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                            model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf2);
                            break;
                            }
                        }
                    }

                model.Inscricao = _imovel.Inscricao;
                EnderecoStruct _endLocal = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                string _compl = _endLocal.Complemento == null ? "" : " " + _endLocal.Complemento;
                model.Endereco_Local = _endLocal.Endereco + ", " + _endLocal.Numero.ToString() + _compl + " - " + _endLocal.NomeBairro.ToString() + " - " + _endLocal.NomeCidade + "/" + _endLocal.UF + " Cep:" + _endLocal.Cep;

                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                Contribuinte_Header_Struct _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao);
                _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                model.Endereco_Prop = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;

                if (model.Codigo_cidadao2 > 0) {
                    _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao2);
                    _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                    model.Endereco_prop2 = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;
                    }

                EnderecoStruct _endEntrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                if (_endEntrega.Endereco != null) {
                    _compl = _endEntrega.Complemento == null ? "" : " " + _endEntrega.Complemento;
                    model.Endereco_Entrega = _endEntrega.Endereco + ", " + _endEntrega.Numero.ToString() + _compl + " - " + _endEntrega.NomeBairro??"" + " - " + _endEntrega.NomeCidade + "/" + _endEntrega.UF + " Cep:" + _endEntrega.Cep;
                    } else {
                    if (_imovel.EE_TipoEndereco == 0)
                        model.Endereco_Entrega = model.Endereco_Local;
                    else {
                        model.Endereco_Entrega = model.Endereco_Prop;
                        }
                    }
                }

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
                }
            if (action == "btnValida") {
                bool _existe = imovelRepository.Existe_Notificacao_Obra(model.Ano_Notificacao, model.Numero_Notificacao);
                if (_existe) {
                    ViewBag.Result = "Nº de notificação já cadastrado.";
                    return View(model);
                    } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                        }
                    Save_Notificacao_Obra(model);
                    return RedirectToAction("Notificacao_obra_query");
                    }
                }

            return View(model);
            }

        private ActionResult Save_Notificacao_Obra(NotificacaoTerViewModel model) {
            Notificacao_Obra reg = new Notificacao_Obra() {
                Ano_not = model.Ano_Notificacao,
                Numero_not = model.Numero_Notificacao,
                Codigo = model.Codigo_Imovel,
                Inscricao = model.Inscricao,
                Endereco_entrega = model.Endereco_Entrega,
                Endereco_entrega2 = model.Endereco_entrega2,
                Endereco_infracao = model.Endereco_Local,
                Endereco_prop = model.Endereco_Prop,
                Endereco_prop2 = model.Endereco_prop2,
                Prazo = model.Prazo,
                Nome = model.Nome_Proprietario,
                Situacao = 3,//concluido
                Userid = Convert.ToInt32(Session["hashid"]),
                Data_cadastro = DateTime.Now,
                Nome2 = model.Nome_Proprietario2,
                Codigo_cidadao = model.Codigo_cidadao,
                Codigo_cidadao2 = model.Codigo_cidadao2,
                Cpf = model.Cpf,
                Cpf2 = model.Cpf2,
                Rg = model.Rg,
                Rg2 = model.Rg2
                };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_notificacao_obra(reg);
            return null;

            }

        [Route("Notificacao_obra_query")]
        [HttpGet]
        public ActionResult Notificacao_obra_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Obra_Struct> _listaNot = imovelRepository.Lista_Notificacao_Obra(DateTime.Now.Year);
            foreach (Notificacao_Obra_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                    };
                ListaNot.Add(reg);
                }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
            }

        [Route("Notificacao_obra_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_obra_query(NotificacaoTerQueryViewModel model2) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Notificacao_Obra_Struct> _listaNot = imovelRepository.Lista_Notificacao_Obra(model2.Ano_Selected);
            foreach (Notificacao_Obra_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = model2.Ano_Selected;
            return View(model);
        }

        public ActionResult Notificacao_obra_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Notificacao_Obra_Struct _not = imovelRepository.Retorna_Notificacao_Obra(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumero = _not.AnoNumero,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Prazo = _not.Prazo,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                PrazoText = Functions.Escrever_Valor_Extenso(_not.Prazo),
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? "",
                Data_Cadastro = _not.Data_Cadastro
                };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
                }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Notificacao_Obra.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Notificacao_Obra.pdf");
                } catch {

                throw;
                }

            }

        [Route("AutoInfracao_ter_add")]
        [HttpGet]
        public ActionResult AutoInfracao_ter_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            model.Ano_Auto = DateTime.Now.Year;
            return View(model);
            }

        [Route("AutoInfracao_ter_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_ter_add(NotificacaoTerViewModel model, string action) {
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
                }

            int _num = model.Numero_Notificacao;
            int _ano = model.Ano_Notificacao;

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (action == "btnCodigoOK") {

                bool _existe = imovelRepository.Existe_Notificacao_Terreno(_ano, _num);
                if (!_existe) {
                    ViewBag.Result = "Notificação não cadastrada.";
                    return View(model);
                    }
                Notificacao_Terreno_Struct _dados = imovelRepository.Retorna_Notificacao_Terreno(_ano, _num);
                model.Codigo_cidadao = _dados.Codigo_cidadao;
                model.Codigo_cidadao2 = _dados.Codigo_cidadao2;
                model.Nome_Proprietario = _dados.Nome_Proprietario;
                model.Nome_Proprietario2 = _dados.Nome_Proprietario2;
                model.Codigo_Imovel = _dados.Codigo_Imovel;
                model.Endereco_Local = _dados.Endereco_Local;
                model.Endereco_Prop = _dados.Endereco_Prop;
                model.Endereco_prop2 = _dados.Endereco_prop2;
                model.Endereco_Entrega = _dados.Endereco_Entrega;
                model.Endereco_entrega2 = _dados.Endereco_entrega2;
                model.Data_Cadastro = _dados.Data_Cadastro;
                }
            model.Data_Notificacao = model.Data_Notificacao == null ? model.Data_Cadastro.ToString("dd/MM/yyyy") : model.Data_Notificacao;

            if (action == "btnValida") {

                int _num_auto = model.Numero_Auto;
                int _ano_auto = model.Ano_Auto;
                if (_num_auto == 0) {
                    ViewBag.Result = "Digite o nº do auto de infração.";
                    return View(model);
                    }

                bool _isdata = Functions.IsDate(model.Data_Notificacao);
                if (!_isdata) {
                    ViewBag.Result = "Data de infração inválida.";
                    return View(model);
                    }

                bool _existe = imovelRepository.Existe_Auto_Infracao(_ano_auto, _num_auto);
                if (_existe) {
                    ViewBag.Result = "Auto de infração já cadastrado.";
                    return View(model);
                    } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                        }
                    Save_Auto_Infracao(model);
                    return RedirectToAction("AutoInfracao_ter_query");
                    }
                }

            return View(model);
            }

        private ActionResult Save_Auto_Infracao(NotificacaoTerViewModel model) {
            int _userid = Convert.ToInt32(Session["hashid"]);
            Auto_infracao reg = new Auto_infracao() {
                Ano_auto = model.Ano_Auto,
                Numero_auto = model.Numero_Auto,
                Ano_notificacao = model.Ano_Notificacao,
                Numero_notificacao = model.Numero_Notificacao,
                Data_notificacao = Convert.ToDateTime(model.Data_Notificacao),
                Userid = _userid
                };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_auto_infracao(reg);
            return null;
            }

        [Route("AutoInfracao_ter_query")]
        [HttpGet]
        public ActionResult AutoInfracao_ter_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Auto_Infracao_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao(DateTime.Now.Year);
            foreach (Auto_Infracao_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto = item.Ano_Auto,
                    Numero_Auto = item.Numero_Auto,
                    AnoNumeroAuto = item.AnoNumeroAuto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                    };
                ListaNot.Add(reg);
                }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
            }

        [Route("AutoInfracao_ter_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_ter_query(NotificacaoTerQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Auto_Infracao_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao(_ano);
            foreach (Auto_Infracao_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto=item.Ano_Auto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    AnoNumeroAuto = item.AnoNumeroAuto,
                    Numero_Auto=item.Numero_Auto,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    //Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;
            
            return View(model);
            }

        public ActionResult AutoInfracao_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Auto_Infracao_Struct _not = imovelRepository.Retorna_Auto_Infracao(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumeroAuto = _not.AnoNumeroAuto,
                AnoNumero = _not.AnoNumero,
                Data_Notificacao = _not.Data_Notificacao,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? ""
                };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
                }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Auto_Infracao.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Auto_Infracao.pdf");
                } catch {

                throw;
                }
            }

        [Route("AutoInfracao_queimada_menu")]
        [HttpGet]
        public ActionResult AutoInfracao_queimada_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("AutoInfracao_queimada_add")]
        [HttpGet]
        public ActionResult AutoInfracao_queimada_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Auto = DateTime.Now.Year;
            return View(model);
            }

        [Route("AutoInfracao_queimada_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_queimada_add(NotificacaoTerViewModel model, string action) {
            if (model.Codigo_Imovel == 0) {
                ViewBag.Result = "Código de imóvel inválido.";
                return View(model);
                }
            int _codigo = model.Codigo_Imovel;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
                }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (action == "btnCodigoOK") {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                List<ProprietarioStruct> Listaprop = imovelRepository.Lista_Proprietario(_codigo, false);
                if (Listaprop.Count == 0) {
                    ViewBag.Result = "Não é possível emitir multa para este imóvel.";
                    model = new NotificacaoTerViewModel();
                    return View(model);
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && prop.Principal) {
                        model.Codigo_cidadao = prop.Codigo;
                        model.Nome_Proprietario = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg = _cidadao.Rg;
                        model.Cpf = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && !prop.Principal) {
                        model.Codigo_cidadao2 = prop.Codigo;
                        model.Nome_Proprietario2 = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg2 = _cidadao.Rg;
                        model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                        }
                    }
                if (string.IsNullOrEmpty(model.Nome_Proprietario2) && Listaprop.Count > 1) {
                    foreach (ProprietarioStruct prop in Listaprop) {
                        if (prop.Tipo != "P") {
                            model.Codigo_cidadao2 = prop.Codigo;
                            model.Nome_Proprietario2 = prop.Nome;
                            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                            model.Rg2 = _cidadao.Rg;
                            model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                            model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf2);
                            break;
                            }
                        }
                    }

                model.Inscricao = _imovel.Inscricao;
                EnderecoStruct _endLocal = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                string _compl = _endLocal.Complemento == null ? "" : " " + _endLocal.Complemento;
                model.Endereco_Local = _endLocal.Endereco + ", " + _endLocal.Numero.ToString() + _compl + " - " + _endLocal.NomeBairro.ToString() + " - " + _endLocal.NomeCidade + "/" + _endLocal.UF + " Cep:" + _endLocal.Cep;

                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                Contribuinte_Header_Struct _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao);
                _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                model.Endereco_Prop = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;

                if (model.Codigo_cidadao2 > 0) {
                    _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao2);
                    _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                    model.Endereco_prop2 = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;
                    }

                EnderecoStruct _endEntrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                if (_endEntrega.Endereco != null) {
                    _compl = _endEntrega.Complemento == null ? "" : " " + _endEntrega.Complemento;
                    model.Endereco_Entrega = _endEntrega.Endereco + ", " + _endEntrega.Numero.ToString() + _compl + " - " + _endEntrega.NomeBairro??"" + " - " + _endEntrega.NomeCidade + "/" + _endEntrega.UF + " Cep:" + _endEntrega.Cep;
                    } else {
                    if (_imovel.EE_TipoEndereco == 0)
                        model.Endereco_Entrega = model.Endereco_Local;
                    else {
                        model.Endereco_Entrega = model.Endereco_Prop;
                        }
                    }
                }

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
                }
            if (action == "btnValida") {
                bool _existe = imovelRepository.Existe_AutoInfracao_Queimada(model.Ano_Auto, model.Numero_Auto);
                if (_existe) {
                    ViewBag.Result = "Nº de Auto de Infração já cadastrado.";
                    return View(model);
                    } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                        }
                    Save_AutoInfracao_Queimada(model);
                    return RedirectToAction("AutoInfracao_queimada_query");
                    }
                }

            return View(model);
            }

        private ActionResult Save_AutoInfracao_Queimada(NotificacaoTerViewModel model) {
            Auto_Infracao_Queimada reg = new Auto_Infracao_Queimada() {
                Ano_multa = model.Ano_Auto,
                Numero_multa = model.Numero_Auto,
                Codigo = model.Codigo_Imovel,
                Inscricao = model.Inscricao,
                Endereco_entrega = model.Endereco_Entrega,
                Endereco_entrega2 = model.Endereco_entrega2,
                Endereco_infracao = model.Endereco_Local,
                Endereco_prop = model.Endereco_Prop,
                Endereco_prop2 = model.Endereco_prop2,
                Prazo = model.Prazo,
                Nome = model.Nome_Proprietario,
                Situacao = 3,//concluido
                Userid = Convert.ToInt32(Session["hashid"]),
                Data_cadastro = DateTime.Now,
                Nome2 = model.Nome_Proprietario2,
                Codigo_cidadao = model.Codigo_cidadao,
                Codigo_cidadao2 = model.Codigo_cidadao2,
                Cpf = model.Cpf,
                Cpf2 = model.Cpf2,
                Rg = model.Rg,
                Rg2 = model.Rg2
            };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_AutoInfracao_Queimada(reg);
            return null;
        }

        [Route("AutoInfracao_Queimada_Query")]
        [HttpGet]
        public ActionResult AutoInfracao_Queimada_Query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Auto_Infracao_Queimada_Struct> _listaNot = imovelRepository.Lista_AutoInfracao_Queimada(DateTime.Now.Year);
            foreach (Auto_Infracao_Queimada_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto = item.Ano_Multa,
                    Numero_Auto = item.Numero_Multa,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
        }

        public ActionResult AutoInfracao_Queimada_Print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Auto_Infracao_Queimada_Struct _not = imovelRepository.Retorna_AutoInfracao_Queimada(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumero = _not.AnoNumero,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Prazo = _not.Prazo,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                PrazoText = Functions.Escrever_Valor_Extenso(_not.Prazo),
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? "",
                Data_Cadastro = _not.Data_Cadastro
            };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
            }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/AutoInfracao_Queimada.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "AutoInfracao.pdf");
            } catch {

                throw;
            }

        }

        [Route("AutoInfracao_obra_add")]
        [HttpGet]
        public ActionResult AutoInfracao_obra_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoTerViewModel model = new NotificacaoTerViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            model.Ano_Auto = DateTime.Now.Year;
            return View(model);
        }

        [Route("AutoInfracao_obra_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_obra_add(NotificacaoTerViewModel model, string action ) {
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);

            if (action == "btnCodigoCancel") {
                model = new NotificacaoTerViewModel();
                return View(model);
            }

            int _num = model.Numero_Notificacao;
            int _ano = model.Ano_Notificacao;

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (action == "btnCodigoOK") {

                bool _existe = imovelRepository.Existe_Notificacao_Obra(_ano, _num);
                if (!_existe) {
                    ViewBag.Result = "Notificação não cadastrada.";
                    return View(model);
                }
                Notificacao_Obra_Struct _dados = imovelRepository.Retorna_Notificacao_Obra(_ano, _num);
                model.Codigo_cidadao = _dados.Codigo_cidadao;
                model.Codigo_cidadao2 = _dados.Codigo_cidadao2;
                model.Nome_Proprietario = _dados.Nome_Proprietario;
                model.Nome_Proprietario2 = _dados.Nome_Proprietario2;
                model.Codigo_Imovel = _dados.Codigo_Imovel;
                model.Endereco_Local = _dados.Endereco_Local;
                model.Endereco_Prop = _dados.Endereco_Prop;
                model.Endereco_prop2 = _dados.Endereco_prop2;
                model.Endereco_Entrega = _dados.Endereco_Entrega;
                model.Endereco_entrega2 = _dados.Endereco_entrega2;
                model.Data_Cadastro = _dados.Data_Cadastro;
            }
            model.Data_Notificacao = model.Data_Notificacao == null ? model.Data_Cadastro.ToString("dd/MM/yyyy") : model.Data_Notificacao;

            if (action == "btnValida") {

                int _num_auto = model.Numero_Auto;
                int _ano_auto = model.Ano_Auto;
                if (_num_auto == 0) {
                    ViewBag.Result = "Digite o nº do auto de infração.";
                    return View(model);
                }

                bool _isdata = Functions.IsDate(model.Data_Notificacao);
                if (!_isdata) {
                    ViewBag.Result = "Data de infração inválida.";
                    return View(model);
                }

                bool _existe = imovelRepository.Existe_Auto_Infracao_Obra(_ano_auto, _num_auto);
                if (_existe) {
                    ViewBag.Result = "Auto de infração já cadastrado.";
                    return View(model);
                } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                    }
                    Save_Auto_Infracao_Obra(model);
                    return RedirectToAction("AutoInfracao_obra_query");
                }
            }

            return View(model);
        }

        private ActionResult Save_Auto_Infracao_Obra(NotificacaoTerViewModel model) {
            int _userid = Convert.ToInt32(Session["hashid"]);
            Auto_infracao_obra reg = new Auto_infracao_obra() {
                Ano_auto = model.Ano_Auto,
                Numero_auto = model.Numero_Auto,
                Ano_notificacao = model.Ano_Notificacao,
                Numero_notificacao = model.Numero_Notificacao,
                Data_notificacao = Convert.ToDateTime(model.Data_Notificacao),
                Userid = _userid
            };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_auto_infracao_Obra(reg);
            return null;
        }

        [Route("AutoInfracao_obra_query")]
        [HttpGet]
        public ActionResult AutoInfracao_obra_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Auto_Infracao_Obra_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao_Obra(DateTime.Now.Year);
            foreach (Auto_Infracao_Obra_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto = item.Ano_Auto,
                    Numero_Auto = item.Numero_Auto,
                    AnoNumeroAuto = item.AnoNumeroAuto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
        }

        [Route("AutoInfracao_obra_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_obra_query(NotificacaoTerQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoTerViewModel> ListaNot = new List<NotificacaoTerViewModel>();
            List<Auto_Infracao_Obra_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao_Obra(_ano);
            foreach (Auto_Infracao_Obra_Struct item in _listaNot) {
                NotificacaoTerViewModel reg = new NotificacaoTerViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto=item.Ano_Auto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    AnoNumeroAuto=item.AnoNumeroAuto,
                    Numero_Auto=item.Numero_Auto,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                  //  Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoTerQueryViewModel model = new NotificacaoTerQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;
            return View(model);
        }

        public ActionResult AutoInfracao_obra_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Auto_Infracao_Obra_Struct _not = imovelRepository.Retorna_Auto_Infracao_Obra(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumeroAuto = _not.AnoNumeroAuto,
                AnoNumero = _not.AnoNumero,
                Data_Cadastro = _not.Data_Cadastro,
                Data_Notificacao = _not.Data_Notificacao,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? ""
            };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
            }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Auto_Infracao_Obra.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Auto_Infracao.pdf");
            } catch {

                throw;
            }
        }

        [Route("Notificacao_hab_menu")]
        [HttpGet]
        public ActionResult Notificacao_hab_menu() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }

        [Route("Notificacao_hab_add")]
        [HttpGet]
        public ActionResult Notificacao_hab_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoHabViewModel model = new NotificacaoHabViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            return View(model);
        }

        [Route("Notificacao_hab_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_hab_add(NotificacaoHabViewModel model, string action) {
            if (model.Codigo_Imovel == 0) {
                ViewBag.Result = "Código de imóvel inválido.";
                return View(model);
            }
            int _codigo = model.Codigo_Imovel;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (action == "btnCodigoOK") {
                ImovelStruct _imovel = imovelRepository.Dados_Imovel(_codigo);
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                List<ProprietarioStruct> Listaprop = imovelRepository.Lista_Proprietario(_codigo, false);
                if (Listaprop.Count == 0) {
                    ViewBag.Result = "Não é possível emitir notificação para este imóvel.";
                    model = new NotificacaoHabViewModel();
                    return View(model);
                }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && prop.Principal) {
                        model.Codigo_cidadao = prop.Codigo;
                        model.Nome_Proprietario = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg = _cidadao.Rg;
                        model.Cpf = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                    }
                }
                foreach (ProprietarioStruct prop in Listaprop) {
                    if (prop.Tipo == "P" && !prop.Principal) {
                        model.Codigo_cidadao2 = prop.Codigo;
                        model.Nome_Proprietario2 = prop.Nome;
                        CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                        model.Rg2 = _cidadao.Rg;
                        model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                        model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf);
                        break;
                    }
                }
                if (string.IsNullOrEmpty(model.Nome_Proprietario2) && Listaprop.Count > 1) {
                    foreach (ProprietarioStruct prop in Listaprop) {
                        if (prop.Tipo != "P") {
                            model.Codigo_cidadao2 = prop.Codigo;
                            model.Nome_Proprietario2 = prop.Nome;
                            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(prop.Codigo);
                            model.Rg2 = _cidadao.Rg;
                            model.Cpf2 = string.IsNullOrEmpty(_cidadao.Cnpj) ? _cidadao.Cpf : _cidadao.Cnpj;
                            model.Cpf2 = Functions.FormatarCpfCnpj(model.Cpf2);
                            break;
                        }
                    }
                }

                model.Inscricao = _imovel.Inscricao;
                EnderecoStruct _endLocal = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                string _compl = _endLocal.Complemento == null ? "" : " " + _endLocal.Complemento;
                model.Endereco_Local = _endLocal.Endereco + ", " + _endLocal.Numero.ToString() + _compl + " - " + _endLocal.NomeBairro.ToString() + " - " + _endLocal.NomeCidade + "/" + _endLocal.UF + " Cep:" + _endLocal.Cep;

                Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                Contribuinte_Header_Struct _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao);
                _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                model.Endereco_Prop = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;

                if (model.Codigo_cidadao2 > 0) {
                    _endProp = sistemaRepository.Contribuinte_Header(model.Codigo_cidadao2);
                    _compl = _endProp.Complemento == null ? "" : " " + _endProp.Complemento;
                    model.Endereco_prop2 = _endProp.Endereco + ", " + _endProp.Numero.ToString() + _compl + " - " + _endProp.Nome_bairro.ToString() + " - " + _endProp.Nome_cidade + "/" + _endProp.Nome_uf + " Cep:" + _endProp.Cep;
                }

                EnderecoStruct _endEntrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                if (_endEntrega.Endereco != null) {
                    _compl = _endEntrega.Complemento == null ? "" : " " + _endEntrega.Complemento;
                    model.Endereco_Entrega = _endEntrega.Endereco + ", " + _endEntrega.Numero.ToString() + _compl + " - " + _endEntrega.NomeBairro ?? "" + " - " + _endEntrega.NomeCidade + "/" + _endEntrega.UF + " Cep:" + _endEntrega.Cep;
                } else {
                    if (_imovel.EE_TipoEndereco == 0)
                        model.Endereco_Entrega = model.Endereco_Local;
                    else {
                        model.Endereco_Entrega = model.Endereco_Prop;
                    }
                }
            }

            if (action == "btnCodigoCancel") {
                model = new NotificacaoHabViewModel();
                return View(model);
            }
            if (action == "btnValida") {
                bool _existe = imovelRepository.Existe_Notificacao_Habitese(model.Ano_Notificacao, model.Numero_Notificacao);
                if (_existe) {
                    ViewBag.Result = "Nº de notificação já cadastrado.";
                    return View(model);
                } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                    }
                    Save_Notificacao_Habitese(model);
                    return RedirectToAction("Notificacao_ter_query");
                }
            }

            return View(model);
        }

        private ActionResult Save_Notificacao_Habitese(NotificacaoHabViewModel model) {
            Notificacao_habitese reg = new Notificacao_habitese() {
                Ano_not = model.Ano_Notificacao,
                Numero_not = model.Numero_Notificacao,
                Codigo = model.Codigo_Imovel,
                Inscricao = model.Inscricao,
                Endereco_entrega = model.Endereco_Entrega,
                Endereco_entrega2 = model.Endereco_entrega2,
                Endereco_infracao = model.Endereco_Local,
                Endereco_prop = model.Endereco_Prop,
                Endereco_prop2 = model.Endereco_prop2,
                Prazo = model.Prazo,
                Nome = model.Nome_Proprietario,
                Situacao = 3,//concluido
                Userid = Convert.ToInt32(Session["hashid"]),
                Data_cadastro = DateTime.Now,
                Nome2 = model.Nome_Proprietario2,
                Codigo_cidadao = model.Codigo_cidadao,
                Codigo_cidadao2 = model.Codigo_cidadao2,
                Cpf = model.Cpf,
                Cpf2 = model.Cpf2,
                Rg = model.Rg,
                Rg2 = model.Rg2
            };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_Notificacao_Habitese(reg);
            return null;

        }

        [Route("Notificacao_hab_query")]
        [HttpGet]
        public ActionResult Notificacao_hab_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoHabViewModel> ListaNot = new List<NotificacaoHabViewModel>();
            List<Notificacao_Habitese_Struct> _listaNot = imovelRepository.Lista_Notificacao_Habitese(DateTime.Now.Year);
            foreach (Notificacao_Habitese_Struct item in _listaNot) {
                NotificacaoHabViewModel reg = new NotificacaoHabViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoHabQueryViewModel model = new NotificacaoHabQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
        }

        [Route("Notificacao_hab_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Notificacao_hab_query(NotificacaoHabQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoHabViewModel> ListaNot = new List<NotificacaoHabViewModel>();
            List<Notificacao_Habitese_Struct> _listaNot = imovelRepository.Lista_Notificacao_Habitese(_ano);
            foreach (Notificacao_Habitese_Struct item in _listaNot) {
                NotificacaoHabViewModel reg = new NotificacaoHabViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoHabQueryViewModel model = new NotificacaoHabQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;
            return View(model);
        }

        public ActionResult Notificacao_habitese_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Notificacao_Habitese_Struct _not = imovelRepository.Retorna_Notificacao_Habitese(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumero = _not.AnoNumero,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Prazo = _not.Prazo,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                PrazoText = Functions.Escrever_Valor_Extenso(_not.Prazo),
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? "",
                Data_Cadastro = _not.Data_Cadastro
            };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
            }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Notificacao_Habitese.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Notificacao_Habitese.pdf");
            } catch {

                throw;
            }

        }

        [Route("AutoInfracao_hab_add")]
        [HttpGet]
        public ActionResult AutoInfracao_hab_add() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            NotificacaoHabViewModel model = new NotificacaoHabViewModel();
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            model.Ano_Notificacao = DateTime.Now.Year;
            model.Ano_Auto = DateTime.Now.Year;
            return View(model);
        }

        [Route("AutoInfracao_hab_add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_hab_add(NotificacaoHabViewModel model, string action) {
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);

            if (action == "btnCodigoCancel") {
                model = new NotificacaoHabViewModel();
                return View(model);
            }

            int _num = model.Numero_Notificacao;
            int _ano = model.Ano_Notificacao;

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            if (action == "btnCodigoOK") {

                bool _existe = imovelRepository.Existe_Notificacao_Habitese(_ano, _num);
                if (!_existe) {
                    ViewBag.Result = "Notificação não cadastrada.";
                    return View(model);
                }
                Notificacao_Habitese_Struct _dados = imovelRepository.Retorna_Notificacao_Habitese(_ano, _num);
                model.Codigo_cidadao = _dados.Codigo_cidadao;
                model.Codigo_cidadao2 = _dados.Codigo_cidadao2;
                model.Nome_Proprietario = _dados.Nome_Proprietario;
                model.Nome_Proprietario2 = _dados.Nome_Proprietario2;
                model.Codigo_Imovel = _dados.Codigo_Imovel;
                model.Endereco_Local = _dados.Endereco_Local;
                model.Endereco_Prop = _dados.Endereco_Prop;
                model.Endereco_prop2 = _dados.Endereco_prop2;
                model.Endereco_Entrega = _dados.Endereco_Entrega;
                model.Endereco_entrega2 = _dados.Endereco_entrega2;
                model.Data_Cadastro = _dados.Data_Cadastro;
            }
            model.Data_Notificacao = model.Data_Notificacao == null ? model.Data_Cadastro.ToString("dd/MM/yyyy") : model.Data_Notificacao;

            if (action == "btnValida") {

                int _num_auto = model.Numero_Auto;
                int _ano_auto = model.Ano_Auto;
                if (_num_auto == 0) {
                    ViewBag.Result = "Digite o nº do auto de infração.";
                    return View(model);
                }

                bool _isdata = Functions.IsDate(model.Data_Notificacao);
                if (!_isdata) {
                    ViewBag.Result = "Data de infração inválida.";
                    return View(model);
                }

                bool _existe = imovelRepository.Existe_Auto_Infracao_Habitese(_ano_auto, _num_auto);
                if (_existe) {
                    ViewBag.Result = "Auto de infração já cadastrado.";
                    return View(model);
                } else {
                    int _userid = Convert.ToInt32(Session["hashid"]);
                    if (_userid == 0) {
                        ViewBag.Result = "Sua sessão foi encerrada, favor fazer login novamente.";
                        return View(model);
                    }
                    Save_Auto_Infracao_Habitese(model);
                    return RedirectToAction("AutoInfracao_hab_query");
                }
            }

            return View(model);
        }

        private ActionResult Save_Auto_Infracao_Habitese(NotificacaoHabViewModel model) {
            int _userid = Convert.ToInt32(Session["hashid"]);
            Auto_infracao_habitese reg = new Auto_infracao_habitese() {
                Ano_auto = model.Ano_Auto,
                Numero_auto = model.Numero_Auto,
                Ano_notificacao = model.Ano_Notificacao,
                Numero_notificacao = model.Numero_Notificacao,
                Data_notificacao = Convert.ToDateTime(model.Data_Notificacao),
                Userid = _userid
            };
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Exception ex = imovelRepository.Incluir_auto_infracao_Habitese(reg);
            return null;
        }

        [Route("AutoInfracao_hab_query")]
        [HttpGet]
        public ActionResult AutoInfracao_hab_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoHabViewModel> ListaNot = new List<NotificacaoHabViewModel>();
            List<Auto_Infracao_Habitese_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao_Habitese(DateTime.Now.Year);
            foreach (Auto_Infracao_Habitese_Struct item in _listaNot) {
                NotificacaoHabViewModel reg = new NotificacaoHabViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto = item.Ano_Auto,
                    Numero_Auto = item.Numero_Auto,
                    AnoNumeroAuto = item.AnoNumeroAuto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoHabQueryViewModel model = new NotificacaoHabQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = DateTime.Now.Year;
            return View(model);
        }

        [Route("AutoInfracao_hab_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AutoInfracao_hab_query(NotificacaoHabQueryViewModel model2) {
            int _ano = model2.Ano_Selected;
            List<int> Lista_Ano = new List<int>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) {
                Lista_Ano.Add(i);
            }
            ViewBag.Lista_Ano = new SelectList(Lista_Ano);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<NotificacaoHabViewModel> ListaNot = new List<NotificacaoHabViewModel>();
            List<Auto_Infracao_Habitese_Struct> _listaNot = imovelRepository.Lista_Auto_Infracao_Habitese(_ano);
            foreach (Auto_Infracao_Habitese_Struct item in _listaNot) {
                NotificacaoHabViewModel reg = new NotificacaoHabViewModel() {
                    AnoNumero = item.AnoNumero,
                    Ano_Auto = item.Ano_Auto,
                    Ano_Notificacao = item.Ano_Notificacao,
                    Numero_Notificacao = item.Numero_Notificacao,
                    AnoNumeroAuto = item.AnoNumeroAuto,
                    Numero_Auto = item.Numero_Auto,
                    Codigo_Imovel = item.Codigo_Imovel,
                    Data_Cadastro = item.Data_Cadastro,
                    //Prazo = item.Prazo,
                    Nome_Proprietario = Functions.TruncateTo(item.Nome_Proprietario, 45),
                    Situacao = item.Situacao
                };
                ListaNot.Add(reg);
            }

            NotificacaoHabQueryViewModel model = new NotificacaoHabQueryViewModel();
            model.ListaNotificacao = ListaNot;
            model.Ano_Selected = _ano;

            return View(model);
        }

        public ActionResult AutoInfracao_habitese_print(int a, int n) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Auto_Infracao_Habitese_Struct _not = imovelRepository.Retorna_Auto_Infracao_Habitese(a, n);

            List<DtNotificacao> ListaNot = new List<DtNotificacao>();

            DtNotificacao reg = new DtNotificacao() {
                AnoNumeroAuto = _not.AnoNumeroAuto,
                AnoNumero = _not.AnoNumero,
                Data_Notificacao = _not.Data_Notificacao,
                Codigo = _not.Codigo_Imovel.ToString("00000"),
                Nome = _not.Codigo_cidadao.ToString() + "-" + _not.Nome_Proprietario,
                Cpf = _not.Cpf ?? "",
                Rg = _not.Rg ?? "",
                Endereco_Entrega = _not.Endereco_Entrega,
                Endereco_entrega2 = _not.Endereco_entrega2,
                Endereco_Local = _not.Endereco_Local,
                Endereco_Prop = _not.Endereco_Prop,
                Endereco_prop2 = _not.Endereco_prop2,
                Usuario = _not.UsuarioNome,
                Inscricao = _not.Inscricao,
                Cpf2 = _not.Cpf2 ?? "",
                Rg2 = _not.Rg2 ?? ""
            };
            if (_not.Codigo_cidadao2 > 0) {
                reg.Nome2 = _not.Codigo_cidadao2.ToString() + "-" + _not.Nome_Proprietario2;
            }
            ListaNot.Add(reg);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Auto_Infracao_Habitese.rpt"));

            try {
                rd.SetDataSource(ListaNot);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Auto_Infracao_Habitese.pdf");
            } catch {

                throw;
            }
        }



    }
}