using GTI_Bll.Classes;
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
            if (cod == "")  {
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
                //Save W_Imovel data
                int _user_id = 413;
                if (Request.Cookies["2uC*"] != null)
                    _user_id = Convert.ToInt32(Functions.Decrypt(Request.Cookies["2uC*"].Value));

                W_Imovel_bll w_imovelRepository = new W_Imovel_bll(_connection);
                Exception ex = w_imovelRepository.Excluir_W_Imovel_Codigo(_codigo);
                W_Imovel_Main _mainR = new W_Imovel_Main() {
                    Guid = _guid,
                    Codigo = _codigo
                };
                ex = w_imovelRepository.Insert_W_Imovel_Main(_guid,_codigo,_user_id);
            } else
                ViewBag.Result = "Imóvel não cadastrado.";

            return View(model);

        }


        [HttpGet]
        [Route("imovel_edit")]
        public ActionResult imovel_edit(string c)
        {
            ImovelDetailsViewModel model = new ImovelDetailsViewModel();
            W_Imovel_bll w_imovelRepository = new W_Imovel_bll(_connection);
            W_Imovel_Main w_main = w_imovelRepository.Retorna_Imovel_Main(c);
            if (w_main == null)
                return RedirectToAction("imovel_query");

            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            int _codigo = w_main.Codigo;
            ViewBag.Codigo = _codigo;
            
             model.ImovelStruct = imovelRepository.Dados_Imovel(_codigo);
            //model.Lista_Proprietario = imovelRepository.Lista_Proprietario(_codigo, false);
            //model.Lista_Areas = imovelRepository.Lista_Area(_codigo);
            //model.Lista_Testada = imovelRepository.Lista_Testada(_codigo);
            //if (model.ImovelStruct.EE_TipoEndereco != null)
            //{
            //    short _tipoEE = (short)model.ImovelStruct.EE_TipoEndereco;
            //    if (_tipoEE == 0)
            //        model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Local);
            //    else
            //    {
            //        if (_tipoEE == 1)
            //            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Proprietario);
            //        else
            //            model.Endereco_Entrega = imovelRepository.Dados_Endereco(_codigo, TipoEndereco.Entrega);
            //    }

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

            //Save W_Imovel_Main
            W_Imovel_Main _mainR = new W_Imovel_Main() {
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
                Usoterreno_Nome = model.ImovelStruct.Uso_terreno_Nome
            };
            Exception ex = w_imovelRepository.Update_W_Imovel_Main(_mainR);

            //Save W_Imovel_Prop


            return View(model);
        }

    }
}