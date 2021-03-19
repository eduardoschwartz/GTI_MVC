using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
//using System.Configuration;

namespace GTI_MVC.Controllers {
    public class ParcelamentoController : Controller
    {
        private readonly string _connection = "GTIconnectionTeste";
        [Route("Parc_index")]
        [HttpGet]
        public ActionResult Parc_index()
        {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            //string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
            return View();
        }

        [Route("Parc_req")]
        [HttpGet]
        public ActionResult Parc_req() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            Imovel_bll imovelRepository = new Imovel_bll(_connection);
            Empresa_bll empresaRepository = new Empresa_bll(_connection);

            int _user_id = Convert.ToInt32(Session["hashid"]);
            Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(_user_id);
                        
            string _cpfcnpj = _user.Cpf_Cnpj;
            int _codigo;
            bool _bCpf = false;
            if (_cpfcnpj.Length == 11) {
                _codigo = cidadaoRepository.Existe_Cidadao_Cpf(_cpfcnpj);
                _bCpf = true;
            } else {
                _codigo = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfcnpj);
            }

            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            Parc_Requerente _req = new Parc_Requerente {
                Codigo=_cidadao.Codigo,
                Nome=_cidadao.Nome,

                Email=_user.Email
            };
            _req.Cpf_Cnpj = Functions.FormatarCpfCnpj(_cpfcnpj);
            string _tipoEnd = _cidadao.EnderecoC == "S" ? "C" : "R";
            _req.TipoEnd = _tipoEnd == "R" ? "RESIDENCIAL" : "COMERCIAL";
            if (_tipoEnd == "R") {
                _req.Bairro_Nome = _cidadao.NomeBairroR;
                _req.Cidade_Nome = _cidadao.NomeCidadeR;
                _req.UF = _cidadao.UfR;
                _req.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroR;
                _req.Logradouro_Nome = _cidadao.EnderecoR;
                _req.Numero = (int)_cidadao.NumeroR;
                _req.Complemento = _cidadao.ComplementoR;
                _req.Telefone = _cidadao.TelefoneR;
                _req.Cep = _cidadao.CepR.ToString();
            } else {
                _req.Bairro_Nome = _cidadao.NomeBairroC;
                _req.Cidade_Nome = _cidadao.NomeCidadeC;
                _req.UF = _cidadao.UfC;
                _req.Logradouro_Codigo = (int)_cidadao.CodigoLogradouroC;
                _req.Logradouro_Nome = _cidadao.EnderecoC;
                _req.Numero = (int)_cidadao.NumeroC;
                _req.Complemento = _cidadao.ComplementoC;
                _req.Telefone = _cidadao.TelefoneC;
                _req.Cep = _cidadao.CepC.ToString();
            }
            if (_req.Logradouro_Codigo> 0) {
                int nCep = enderecoRepository.RetornaCep(Convert.ToInt32(_req.Logradouro_Codigo), (short)_req.Numero);
                _req.Cep = nCep.ToString("00000-000");
            }

            ParcelamentoViewModel model = new ParcelamentoViewModel {
                Requerente = _req
            };
            List<SelectListItem> _listaCodigo = new List<SelectListItem>();

            //Lista de imóvel
            List<int> _listaImovel ;
            if (_bCpf)
                _listaImovel = imovelRepository.Lista_Imovel_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaImovel = imovelRepository.Lista_Imovel_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            SelectListGroup _grupo = new SelectListGroup() { Name = "Imovel" };
            foreach (int cod in _listaImovel) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = cod.ToString() + " - Imóvel localizado na(o) " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            //Lista de empresas
            List<int> _listaEmpresa ;
            if (_bCpf)
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            _grupo = new SelectListGroup() { Name = "Empresa" };
            foreach (int cod in _listaEmpresa) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = cod.ToString() + " - " + _header.Nome +  ", localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            //Lista de cidadão
            List<Cidadao> _listaCidadao ;
            if (_bCpf)
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null, Functions.RetornaNumero( _req.Cpf_Cnpj),null);
            else
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null,null, Functions.RetornaNumero(_req.Cpf_Cnpj));

            _grupo = new SelectListGroup() { Name = "Cidadao" };
            foreach (Cidadao cod in _listaCidadao) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod.Codcidadao);
                string _desc = cod.Codcidadao.ToString() + " - Inscrição localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                SelectListItem item = new SelectListItem() {
                    Group = _grupo,
                    Text = _desc,
                    Value = cod.Codcidadao.ToString(),
                    Selected = false
                };
                _listaCodigo.Add(item);
            }

            model.Lista_Codigos = _listaCodigo;

            //Antes de retornar gravamos os dados
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            if (model.Guid == null) {
                //Grava Master
                model.Guid = Guid.NewGuid().ToString("N");
                Parcelamento_web_master reg = new Parcelamento_web_master() {
                    Guid = model.Guid,
                    User_id=_user_id,
                    Requerente_Codigo = _req.Codigo,
                    Requerente_Bairro=_req.Bairro_Nome,
                    Requerente_Cep=Convert.ToInt32(Functions.RetornaNumero(_req.Cep)),
                    Requerente_Cidade=_req.Cidade_Nome,
                    Requerente_Complemento=_req.Complemento,
                    Requerente_CpfCnpj=_req.Cpf_Cnpj,
                    Requerente_Logradouro=_req.Logradouro_Nome,
                    Requerente_Nome=_req.Nome,
                    Requerente_Numero=_req.Numero,
                    Requerente_Telefone=_req.Telefone,
                    Requerente_Uf=_req.UF,
                    Requerente_Email=_req.Email
                };
                Exception ex = parcelamentoRepository.Incluir_Parcelamento_Web_Master(reg);
                if (ex != null)
                    throw ex;
                //Grava ListaCodigos
                short t = 1;
                foreach (SelectListItem item in _listaCodigo) {
                    Parcelamento_web_lista_codigo _cod = new Parcelamento_web_lista_codigo() {
                        Guid = model.Guid,
                        Id = t,
                        Grupo=item.Group.Name,
                        Texto=item.Text,
                        Valor=Convert.ToInt32(item.Value),
                        Selected=item.Selected
                    };
                    ex = parcelamentoRepository.Incluir_Parcelamento_Web_Lista_Codigo(_cod);
                    if (ex != null)
                        throw ex;
                    t++;
                }
            }

            return View(model);
        }

        [Route("Parc_req")]
        [HttpPost]
        public ActionResult Parc_req(ParcelamentoViewModel model,string listacod) {
            int _codigo = Convert.ToInt32(listacod);

            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(_codigo);
            char _tipo = _header.Cpf_cnpj.Length == 11 ? 'F' : 'J';

            List<SpParcelamentoOrigem> Lista_Origem = parcelamentoRepository.Lista_Parcelamento_Origem(_codigo, _tipo);
            List<SelectListItem> _listaCodigo = new List<SelectListItem>();
            List<Parcelamento_web_lista_codigo> _Lista_Codigos = parcelamentoRepository.Lista_Parcelamento_Lista_Codigo(model.Guid);
            foreach (Parcelamento_web_lista_codigo item in _Lista_Codigos) {
                SelectListGroup _grupo = new SelectListGroup() { Name = item.Grupo };
                SelectListItem item2 = new SelectListItem() {
                    Group = _grupo,
                    Text = item.Texto,
                    Value = item.Valor.ToString()
                };
                if(item.Valor==_codigo)
                    item2.Selected = true;
                else
                    item2.Selected = false;
                _listaCodigo.Add(item2);
            }
            model.Lista_Codigos = _listaCodigo;

            if (Lista_Origem.Count == 0) {
                ViewBag.Result = "Não existem débitos a serem parcelados para esta inscrição.";
                return View(model);
            } else {
                foreach (SpParcelamentoOrigem item in Lista_Origem) {
                    Parcelamento_web_origem reg = new Parcelamento_web_origem() {
                        Guid=model.Guid,
                        Idx=item.Idx,
                        Ano=item.Exercicio,
                        Lancamento=item.Lancamento,
                        Sequencia=item.Sequencia,
                        Parcela=item.Parcela,
                        Complemento=item.Complemento,
                        Data_Vencimento=item.Data_vencimento,
                        Lancamento_Nome=item.Nome_lancamento,
                        Ajuizado=item.Ajuizado,
                        Valor_Tributo=item.Valor_principal,
                        Valor_Juros=item.Valor_juros,
                        Valor_Multa=item.Valor_multa,
                        Valor_Correcao=item.Valor_correcao,
                        Valor_Total=item.Valor_total,
                        Valor_Penalidade=item.Valor_penalidade,
                        Perc_Penalidade=item.Perc_penalidade,
                        Qtde_Parcelamento=item.Qtde_parcelamento
                    };
                    Exception ex = parcelamentoRepository.Incluir_Parcelamento_Web_Origem(reg);
                    if (ex != null)
                        throw ex;

                }
            }

            _header = sistemaRepository.Contribuinte_Header(_codigo);
            string _end = _header.Endereco + ", " + _header.Numero.ToString();
            if (!string.IsNullOrEmpty(_header.Complemento))
                _end += " " + _header.Complemento;
            _end += ", " + _header.Nome_bairro;
            if (!string.IsNullOrEmpty(_header.Quadra_original))
                _end += " Quadra:" + _header.Quadra_original;
            if (!string.IsNullOrEmpty(_header.Lote_original))
                _end += ", Lote:" + _header.Lote_original;
            Parcelamento_web_master regP = new Parcelamento_web_master() {
                Guid = model.Guid,
                Contribuinte_Codigo = _codigo,
                Contribuinte_nome = _header.Nome,
                Contribuinte_cpfcnpj = _header.Cpf_cnpj,
                Contribuinte_endereco = _end,
                Contribuinte_bairro = _header.Nome_bairro,
                Contribuinte_cep = Convert.ToInt32(Functions.RetornaNumero(_header.Cep)),
                Contribuinte_cidade = _header.Nome_cidade,
                Contribuinte_uf = _header.Nome_uf
            };
            Exception ex2 = parcelamentoRepository.Atualizar_Codigo_Master(regP);
            if (ex2 != null)
                throw ex2;

            return RedirectToAction("Parc_reqb", new { p = model.Guid });

        }

        [Route("Parc_reqb")]
        [HttpGet]
        public ActionResult Parc_reqb(string p) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if (!_existe) 
                return RedirectToAction("Login_gti", "Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            ParcelamentoViewModel model = new ParcelamentoViewModel() {
                Guid = p,
                Data_Vencimento= _master.Data_Vencimento==null?"": Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy")
            };
            model.Requerente = new Parc_Requerente() {
                Codigo=_master.Requerente_Codigo,
                Nome = _master.Requerente_Nome,
                Cpf_Cnpj = _master.Requerente_CpfCnpj,
                Logradouro_Nome = _master.Requerente_Logradouro,
                Numero = _master.Requerente_Numero,
                Complemento = _master.Requerente_Complemento,
                Bairro_Nome = _master.Requerente_Bairro,
                Cidade_Nome = _master.Requerente_Cidade,
                UF = _master.Requerente_Uf,
                Telefone = _master.Requerente_Telefone,
                Email = _master.Requerente_Email,
                Cep = _master.Requerente_Cep.ToString("00000-000")
            };

            model.Contribuinte = new Parc_Requerente() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Cpf_Cnpj =  Functions.FormatarCpfCnpj( _master.Contribuinte_cpfcnpj),
                Logradouro_Nome = _master.Contribuinte_endereco,
                Bairro_Nome = _master.Contribuinte_bairro+" - " + _master.Contribuinte_cidade + "/" + _master.Contribuinte_uf,
                Cep = _master.Contribuinte_cep.ToString("00000-000")
            };

            return View(model);
        }

        [Route("Parc_reqb")]
        [HttpPost]
        public ActionResult Parc_reqb(ParcelamentoViewModel model) {
            return View(model);
        }


    }
}