﻿using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GTI_Models.modelCore;

namespace GTI_Mvc.Controllers
{
    public class sysImobController : Controller   {
        private readonly string _connection = "GTIconnection";

        [Route("imovel_query")]
        [HttpGet]
        public ActionResult imovel_query(string id) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            model.Lista_Imovel = new List<ImovelLista>();
            return View(model);
        }

        [Route("imovel_query")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult imovel_query(ImovelDetailsViewModel model) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            model.Lista_Imovel = new List<ImovelLista>();
            int _codigo = Convert.ToInt32(model.Codigo);
            string _partialName = model.NomeProprietario ?? "";
            if (_partialName != "") {
                if (_partialName.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do nome do proprietário.";
                    return View(model);
                }
            }
            string _partialEndereco = model.NomeEndereco ?? "";
            if (_partialEndereco != "") {
                if (_partialEndereco.Length < 5) {
                    ViewBag.Result = "Digite ao menos 5 caracteres do endereço.";
                    return View(model);
                }
            }
            int _numero = model.Numero == null ? 0 : Convert.ToInt32(model.Numero);
            int _distrito = 0, _setor = 0, _quadra = 0, _lote = 0, _face = 0, _unidade = 0, _subunidade = 0;
            string _insc = model.Inscricao ?? "";

            if (_insc != "") {
                if (_insc.Length == 25) {
                    _insc = Functions.RetornaNumero(model.Inscricao);
                    _distrito = Convert.ToInt32(_insc.Substring(0, 1));
                    _setor = Convert.ToInt32(_insc.Substring(1, 2));
                    _quadra = Convert.ToInt32(_insc.Substring(3, 4));
                    _lote = Convert.ToInt32(_insc.Substring(7, 5));
                    _face = Convert.ToInt32(_insc.Substring(12, 2));
                    _unidade = Convert.ToInt32(_insc.Substring(14, 2));
                    _subunidade = Convert.ToInt32(_insc.Substring(16, 3));
                } else {
                    ViewBag.Result = "Inscrição cadastral inválida.";
                    return View(model);
                }
            }

            if (_codigo == 0 && _insc == "" && _partialEndereco == "" && _partialName == "") {
                ViewBag.Result = "Selecione ao menos um critério de busca.";
                return View(model);
            }

            List<ImovelStruct> ListaImovel = imovelRepository.Lista_Imovel(_codigo, _distrito, _setor, _quadra, _lote, _face, _unidade, _subunidade, _partialName, _partialEndereco, _numero);
            if (ListaImovel.Count == 0) {
                ViewBag.Result = "Não foi localizado nenhum imóvel com este(s) critério(s).";
                return View(model);
            }
            List<ImovelLista> _lista = new List<ImovelLista>();
            foreach (ImovelStruct item in ListaImovel) {
                ImovelLista reg = new ImovelLista() {
                    Codigo = item.Codigo.ToString("00000"),
                    Nome = Functions.TruncateTo(item.Proprietario_Nome, 30),
                    Endereco = string.IsNullOrEmpty(item.NomeLogradouroAbreviado) ? item.NomeLogradouro : item.NomeLogradouroAbreviado
                };
                reg.Endereco += ", " + item.Numero.ToString() + " " + item.Complemento;
                reg.Endereco = Functions.TruncateTo(reg.Endereco, 52);
                _lista.Add(reg);
            }

            model.Lista_Imovel = _lista;
            return View(model);

        }

        [HttpGet]
        [Route("imovel_data")]
        public ActionResult imovel_data(string cod) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            string _guid = Guid.NewGuid().ToString("N");
            ViewBag.Codigo = cod;
            ViewBag.Guid = _guid;
            cod = Functions.RetornaNumero(cod);
            if (cod == "") {
                ViewBag.Result = "Código inválido.";
                return View(model);
            }
            int _codigo = Convert.ToInt32(cod);
            bool existe = imovelRepository.Existe_Imovel(_codigo);
            if (existe) {
                model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);
                model.Lista_Proprietario = imovelRepository.Lista_Proprietario(_codigo, false);
                model.Lista_Areas = imovelRepository.Lista_Area(_codigo);
                model.Lista_Testada = imovelRepository.Lista_Testada(_codigo);
                model.Tipo_Matricula = model.ImovelStruct.TipoMat == "T" ? "Transcrição" : "Matrícula";
                if (model.ImovelStruct.EE_TipoEndereco != null) {
                    short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                    if (_tipoEE == 0)
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                    else {
                        if (_tipoEE == 1)
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Proprietario);
                        else
                            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                    }
                }
            } else
                ViewBag.Result = "Imóvel não cadastrado.";

            return View(model);

        }

        [HttpGet]
        [Route("imovel_edit")]
        public ActionResult imovel_edit(string c) {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            W_Imovel_bll w_imovelRepository = new W_Imovel_bll(_connection);
            WImovel_Main w_main = w_imovelRepository.Retorna_Imovel_Main(c);
            if (w_main == null)
                return RedirectToAction("imovel_query");

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _codigo = w_main.Codigo;
            model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);

            ViewBag.Codigo = _codigo;
            ViewBag.Guid = w_main.Guid;
            ViewBag.TipoEnd = model.ImovelStruct.EE_TipoEndereco;
            ViewBag.Imune = model.ImovelStruct.Imunidade==null?false:model.ImovelStruct.Imunidade;
            ViewBag.Cip = model.ImovelStruct.Cip==null?false:model.ImovelStruct.Cip;
            ViewBag.Conjugado = model.ImovelStruct.Conjugado == null ? false : model.ImovelStruct.Conjugado;
            ViewBag.Reside = model.ImovelStruct.ResideImovel == null ? false : model.ImovelStruct.ResideImovel;
            if (model.ImovelStruct.EE_TipoEndereco != null) {
                short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
                if (_tipoEE == 0)
                    model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
                else {
                    if (_tipoEE == 1)
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Proprietario);
                    else
                        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
                }
            }
            List<Topografia> listaTop = imovelRepository.Lista_Topografia();
            ViewBag.ListaTop = new SelectList(listaTop, "Codtopografia", "Desctopografia");
            List<Situacao> listaSit = imovelRepository.Lista_Situacao();
            ViewBag.ListaSit = new SelectList(listaSit, "Codsituacao", "Descsituacao");
            List<Categprop> listaCat = imovelRepository.Lista_Categoria_Propriedade();
            ViewBag.ListaCat = new SelectList(listaCat, "Codcategprop", "Desccategprop");
            List<Benfeitoria> listaBen = imovelRepository.Lista_Benfeitoria();
            ViewBag.ListaBen = new SelectList(listaBen, "Codbenfeitoria", "Descbenfeitoria");
            List<Pedologia> listaPed = imovelRepository.Lista_Pedologia();
            ViewBag.ListaPed = new SelectList(listaPed, "Codpedologia", "Descpedologia");
            List<Usoterreno> listaUso = imovelRepository.Lista_uso_terreno();
            ViewBag.ListaUso = new SelectList(listaUso, "Codusoterreno", "Descusoterreno");
            List<SelectListItem> ListaMat = new List<SelectListItem>();
            List<string> Lista_Mat = new List<string>();
            Lista_Mat.Add("Matrícula");
            Lista_Mat.Add("Transcrição");
            ViewBag.Lista_Matricula = new SelectList(Lista_Mat);
            if(model.ImovelStruct.TipoMat==null || model.ImovelStruct.TipoMat=="M")
                model.Tipo_Matricula = "Matrícula";
            else
                model.Tipo_Matricula = "Transcrição";
            
            //Save WImovel_Main
            WImovel_Main _mainR = new WImovel_Main() {
                Guid = w_main.Guid,
                Codigo = _codigo,
                Area_Terreno = model.ImovelStruct.Area_Terreno,
                Cip = model.ImovelStruct.Cip == null ? false : (bool)model.ImovelStruct.Cip,
                Imune = model.ImovelStruct.Imunidade == null ? false : (bool)model.ImovelStruct.Imunidade,
                Conjugado = model.ImovelStruct.Conjugado == null ? false : (bool)model.ImovelStruct.Conjugado,
                Reside = model.ImovelStruct.ResideImovel == null ? false : (bool)model.ImovelStruct.ResideImovel,
                Topografia = (short)model.ImovelStruct.Topografia,
                Pedologia = (short)model.ImovelStruct.Pedologia,
                Situacao = (short)model.ImovelStruct.Situacao,
                Usoterreno = (short)model.ImovelStruct.Uso_terreno,
                Benfeitoria = (short)model.ImovelStruct.Benfeitoria,
                Categoria = (short)model.ImovelStruct.Categoria,
                Inscricao = model.ImovelStruct.Inscricao,
                Condominio = (int)model.ImovelStruct.CodigoCondominio,
                Data_Alteracao = DateTime.Now,
                Condominio_Nome = model.ImovelStruct.NomeCondominio,
                Benfeitoria_Nome = model.ImovelStruct.Benfeitoria_Nome,
                Categoria_Nome = model.ImovelStruct.Categoria_Nome,
                Pedologia_Nome = model.ImovelStruct.Pedologia_Nome,
                Situacao_Nome = model.ImovelStruct.Situacao_Nome,
                Topografia_Nome = model.ImovelStruct.Topografia_Nome,
                Usoterreno_Nome = model.ImovelStruct.Uso_terreno_Nome,
                Lote_Original=model.ImovelStruct.LoteOriginal,
                Quadra_Original=model.ImovelStruct.QuadraOriginal,
                Tipo_Matricula=model.ImovelStruct.TipoMat==null?'M': Convert.ToChar( model.ImovelStruct.TipoMat),
                Tipo_Endereco=(short)model.ImovelStruct.EE_TipoEndereco
            };
            if (model.ImovelStruct.NumMatricula != null && (long)model.ImovelStruct.NumMatricula != 0)
                _mainR.Numero_Matricula = (long)model.ImovelStruct.NumMatricula;
            else
                _mainR.Numero_Matricula = 0;
            Exception ex = w_imovelRepository.Update_W_Imovel_Main(_mainR);

            //Save WImovel_Prop
            ex = w_imovelRepository.Excluir_W_Imovel_Prop_Guid(w_main.Guid);
            List<ProprietarioStruct>ListaP= imovelRepository.Lista_Proprietario(_codigo);
            foreach (ProprietarioStruct item in ListaP) {
                WImovel_Prop _mainP = new WImovel_Prop() {
                    Guid=w_main.Guid,
                    Codigo=item.Codigo,
                    Nome=item.Nome,
                    Tipo=item.Tipo=="P"?"Proprietário":"Solidário",
                    Principal=item.Principal
                };
                ex = w_imovelRepository.Insert_W_Imovel_Prop(_mainP);
            }

            //Save WImovel_Endereco
            ex = w_imovelRepository.Excluir_W_Imovel_Endereco(w_main.Guid);
            if (model.ImovelStruct.EE_TipoEndereco == 2) {
                WImovel_Endereco _mainE = new WImovel_Endereco() {
                    Guid=w_main.Guid,
                    Logradouro_codigo=(int)model.Endereco_Entrega.CodLogradouro,
                    Logradouro_nome=model.Endereco_Entrega.Endereco_Abreviado,
                    Numero=(short)model.Endereco_Entrega.Numero,
                    Complemento=model.Endereco_Entrega.Complemento,
                    Bairro_codigo=(int)model.Endereco_Entrega.CodigoBairro,
                    Bairro_nome=model.Endereco_Entrega.NomeBairro,
                    Cep=model.Endereco_Entrega.Cep
                };
                ex = w_imovelRepository.Insert_W_Imovel_Endereco(_mainE);
            }

            //Save WImovel_Testada
            ex = w_imovelRepository.Excluir_W_Imovel_Testada_Guid(w_main.Guid);
            List<Testada> ListaT = imovelRepository.Lista_Testada(_codigo);
            foreach (Testada item in ListaT) {
                WImovel_Testada _mainT = new WImovel_Testada() {
                    Guid = w_main.Guid,
                    Face = item.Numface,
                    Comprimento = item.Areatestada
                };
                ex = w_imovelRepository.Insert_W_Imovel_Testada(_mainT);
            }

            //Save WImovel_Area
            ex = w_imovelRepository.Excluir_W_Imovel_Area_Guid(w_main.Guid);
            List<AreaStruct> ListaA = imovelRepository.Lista_Area(_codigo);
            foreach (AreaStruct item in ListaA) {
                WImovel_Area _mainA = new WImovel_Area() {
                    Guid = w_main.Guid,
                    Seq=item.Seq,
                    Area=item.Area,
                    Uso_codigo=item.Uso_Codigo,
                    Uso_nome=item.Uso_Nome,
                    Tipo_codigo=item.Tipo_Codigo,
                    Tipo_nome=item.Tipo_Nome,
                    Categoria_codigo=item.Categoria_Codigo,
                    Categoria_nome=item.Categoria_Nome,
                    Processo_Numero=item.Numero_Processo,
                    Pavimentos=item.Pavimentos
                };
                if (item.Data_Aprovacao != null && item.Data_Aprovacao != DateTime.MinValue)
                    _mainA.Data_Aprovacao = Convert.ToDateTime(item.Data_Aprovacao).ToString("dd/MM/yyyy");
                if (item.Data_Processo != null && item.Data_Processo != DateTime.MinValue)
                    _mainA.Processo_Data = Convert.ToDateTime(item.Data_Processo).ToString("dd/MM/yyyy");
                ex = w_imovelRepository.Insert_W_Imovel_Area(_mainA);
            }

            //Save WImovel_Historico
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            ex = w_imovelRepository.Excluir_W_Imovel_Historico_Guid(w_main.Guid);
            List<HistoricoStruct> ListaH = imovelRepository.Lista_Historico(_codigo);
            foreach (HistoricoStruct item in ListaH) {
                WImovel_Historico _mainH = new WImovel_Historico() {
                    Guid = w_main.Guid,
                    Seq = item.Seq,
                    Data_Alteracao = Convert.ToDateTime( item.Data).ToString("dd/MM/yyyy"),
                    Historico = item.Descricao,
                    Usuario_Codigo = (int)item.Usuario_Codigo,
                    Usuario_Nome = sistemaRepository.Retorna_User_LoginName((int)item.Usuario_Codigo)
                };
                ex = w_imovelRepository.Insert_W_Imovel_Historico(_mainH);
            }

            return View(model);
        }

        public JsonResult wImovelnew(string guid, string cod) {
            int _user_id = 413;
            if (Request.Cookies["2uC*"] != null)
                _user_id = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));

            int _codigo = Convert.ToInt32(cod);
            W_Imovel_bll w_imovelRepository = new W_Imovel_bll(_connection);

            Exception ex = w_imovelRepository.Excluir_W_Imovel_Codigo(_codigo);
            ex = w_imovelRepository.Excluir_W_Imovel_Prop_Guid(guid);

            WImovel_Main _mainR = new WImovel_Main() {
                Guid = guid,
                Codigo = _codigo
            };
            ex = w_imovelRepository.Insert_W_Imovel_Main(guid, _codigo, _user_id);

            var result2 = new { Bairro_Codigo = (short)_codigo, Success = "True" };
            return new JsonResult { Data = result2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }

        public JsonResult Lista_WImovel_Prop(string guid) {
            W_Imovel_bll wimovelRepository = new W_Imovel_bll(_connection);
            List<WImovel_Prop> Lista = wimovelRepository.Lista_WImovel_Prop(guid);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Imovel_Prop(string cod) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<ProprietarioStruct> Lista = imovelRepository.Lista_Proprietario(Convert.ToInt32(cod));
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_WImovel_Testada(string guid) {
            W_Imovel_bll wimovelRepository = new W_Imovel_bll(_connection);
            List<WImovel_Testada> Lista = wimovelRepository.Lista_WImovel_Testada(guid);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_WImovel_Area(string guid) {
            W_Imovel_bll wimovelRepository = new W_Imovel_bll(_connection);
            List<WImovel_Area> Lista = wimovelRepository.Lista_WImovel_Area(guid);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_WImovel_Historico(string guid) {
            W_Imovel_bll wimovelRepository = new W_Imovel_bll(_connection);
            List<WImovel_Historico> Lista = wimovelRepository.Lista_WImovel_Historico(guid);
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Imovel_Historico(string cod) {
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            List<HistoricoStruct> Lista = imovelRepository.Lista_Historico(Convert.ToInt32(cod));
            return new JsonResult { Data = Lista, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Lista_Cidadao(string codigo, string nome, string cpfcnpj) {
            if (string.IsNullOrEmpty(codigo)) codigo = "0";
            int _cod = Convert.ToInt32(Functions.RetornaNumero(codigo));
            string _nome = nome.Trim() ?? "";
            string _cpfcnpj = Functions.RetornaNumero(cpfcnpj) ?? "";

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<Cidadao> Lista = cidadaoRepository.Lista_Cidadao(_cod, _nome, _cpfcnpj, 15);

            List<Cidadao> ObjCid = new List<Cidadao>();
            foreach (Cidadao cid in Lista) {
                if (string.IsNullOrEmpty(cid.Cnpj) && cid.Cnpj != "0")
                    _cpfcnpj = cid.Cpf;
                else
                    _cpfcnpj = cid.Cnpj;

                Cidadao reg = new Cidadao() {
                    Codcidadao = cid.Codcidadao,
                    Nomecidadao = Functions.TruncateTo(cid.Nomecidadao.ToUpper(), 45),
                    Cpf = Functions.FormatarCpfCnpj(_cpfcnpj)
                };
                ObjCid.Add(reg);
            }

            return Json(ObjCid, JsonRequestBehavior.AllowGet);
        }


    }
}