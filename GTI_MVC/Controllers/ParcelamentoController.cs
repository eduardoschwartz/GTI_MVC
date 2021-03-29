using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Parcelamento.EditorTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
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
                _req.Logradouro_Codigo = _cidadao.CodigoLogradouroR==null?0:(int)_cidadao.CodigoLogradouroR;
                _req.Logradouro_Nome = _cidadao.EnderecoR;
                _req.Numero = (int)_cidadao.NumeroR;
                _req.Complemento = _cidadao.ComplementoR;
                _req.Telefone = _cidadao.TelefoneR;
                _req.Cep = _cidadao.CepR.ToString();
            } else {
                _req.Bairro_Nome = _cidadao.NomeBairroC;
                _req.Cidade_Nome = _cidadao.NomeCidadeC;
                _req.UF = _cidadao.UfC;
                _req.Logradouro_Codigo = _cidadao.CodigoLogradouroC==null?0:(int)_cidadao.CodigoLogradouroC;
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
            List<Parc_Codigos> _listaCodigos = new List<Parc_Codigos>();

            //Lista de imóvel
            List<int> _listaImovel ;
            if (_bCpf)
                _listaImovel = imovelRepository.Lista_Imovel_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaImovel = imovelRepository.Lista_Imovel_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            foreach (int cod in _listaImovel) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = "Imóvel localizado na(o) " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                Parc_Codigos item = new Parc_Codigos() {
                    Codigo=_header.Codigo,
                    Tipo="Imóvel",
                    Cpf_Cnpj=Functions.FormatarCpfCnpj( _header.Cpf_cnpj),
                    Descricao=_desc
                };
                _listaCodigos.Add(item);
            }

            //Lista de empresas
            List<int> _listaEmpresa ;
            if (_bCpf)
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            foreach (int cod in _listaEmpresa) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc =  _header.Nome +  ", localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if (!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if (!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if (!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                Parc_Codigos item = new Parc_Codigos() {
                    Codigo = _header.Codigo,
                    Tipo = "Empresa",
                    Cpf_Cnpj = Functions.FormatarCpfCnpj(_header.Cpf_cnpj),
                    Descricao = _desc
                };
                _listaCodigos.Add(item);
            }

            //Lista de cidadão
            List<Cidadao> _listaCidadao ;
            if (_bCpf)
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null, Functions.RetornaNumero( _req.Cpf_Cnpj),null);
            else
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null,null, Functions.RetornaNumero(_req.Cpf_Cnpj));

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

                Parc_Codigos item = new Parc_Codigos() {
                    Codigo = _header.Codigo,
                    Tipo = "Outros",
                    Cpf_Cnpj = Functions.FormatarCpfCnpj(_header.Cpf_cnpj),
                    Descricao = _desc
                };
                _listaCodigos.Add(item);

            }

            model.Lista_Codigos = _listaCodigos;

            //Antes de retornar gravamos os dados
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            if (model.Guid == null) {
                //Grava Master
                model.Guid = Guid.NewGuid().ToString("N");
                Parcelamento_web_master reg = new Parcelamento_web_master() {
                    Guid = model.Guid,
                    User_id=_user_id,
                    Data_Vencimento=DateTime.Now,
                    Requerente_Codigo = _req.Codigo,
                    Requerente_Bairro=_req.Bairro_Nome??"",
                    Requerente_Cep=Convert.ToInt32(Functions.RetornaNumero(_req.Cep)),
                    Requerente_Cidade=_req.Cidade_Nome,
                    Requerente_Complemento=_req.Complemento??"",
                    Requerente_CpfCnpj=_req.Cpf_Cnpj,
                    Requerente_Logradouro=_req.Logradouro_Nome,
                    Requerente_Nome=_req.Nome,
                    Requerente_Numero=_req.Numero,
                    Requerente_Telefone=_req.Telefone??"",
                    Requerente_Uf=_req.UF,
                    Requerente_Email=_req.Email??""
                };
                Exception ex = parcelamentoRepository.Incluir_Parcelamento_Web_Master(reg);
                if (ex != null)
                    throw ex;
                foreach (Parc_Codigos item in _listaCodigos) {
                    Parcelamento_web_lista_codigo _cod = new Parcelamento_web_lista_codigo() {
                        Guid = model.Guid,
                        Codigo = item.Codigo,
                        Tipo = item.Tipo,
                        Documento = item.Cpf_Cnpj,
                        Descricao = item.Descricao,
                        Selected = item.Selected
                    };
                    ex = parcelamentoRepository.Incluir_Parcelamento_Web_Lista_Codigo(_cod);
                    if (ex != null)
                        throw ex;
                }
            }

            return View(model);
        }

        [Route("Parc_req")]
        [HttpPost]
        public ActionResult Parc_req(ParcelamentoViewModel model,string listacod) {
            int _codigo = 0;
            for (int i = 0; i < model.Lista_Codigos.Count; i++) {
                if (model.Lista_Codigos[i].Selected) {
                    _codigo = Convert.ToInt32(model.Lista_Codigos[i].Codigo);
                    break;
                }
            }

            List<Parc_Codigos> _listaCodigos = new List<Parc_Codigos>();
            List<Parcelamento_web_lista_codigo> _Lista_Codigos = new List<Parcelamento_web_lista_codigo>();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(_codigo);
            if (_header.Cpf_cnpj == null) {
                _Lista_Codigos = parcelamentoRepository.Lista_Parcelamento_Lista_Codigo(model.Guid);
                foreach (Parcelamento_web_lista_codigo item in _Lista_Codigos) {
                    Parc_Codigos item2 = new Parc_Codigos() {
                        Codigo = item.Codigo,
                        Tipo = item.Tipo,
                        Cpf_Cnpj = item.Documento,
                        Descricao = item.Descricao
                    };
                    _listaCodigos.Add(item2);
                }
                ViewBag.Result = "Cpf/Cnpj não cadstrado.";
                model.Lista_Codigos = _listaCodigos;
                return View(model);
            }
            char _tipo = _header.Cpf_cnpj.Length == 11 ? 'F' : 'J';

            List<SpParcelamentoOrigem> Lista_Origem = parcelamentoRepository.Lista_Parcelamento_Origem(_codigo, _tipo);
            
            _Lista_Codigos = parcelamentoRepository.Lista_Parcelamento_Lista_Codigo(model.Guid);
            foreach (Parcelamento_web_lista_codigo item in _Lista_Codigos) {
                Parc_Codigos item2 = new Parc_Codigos() {
                    Codigo = item.Codigo,
                    Tipo = item.Tipo,
                    Cpf_Cnpj = item.Documento,
                    Descricao = item.Descricao
                };
                _listaCodigos.Add(item2);
            }
            model.Lista_Codigos = _listaCodigos;

            if (Lista_Origem.Count == 0) {
                ViewBag.Result = "Não existem débitos a serem parcelados para esta inscrição.";
                return View(model);
            } else {
                Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Origem(model.Guid);

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
                        Valor_Total=Math.Round( item.Valor_principal,2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa, 2, MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao, 2, MidpointRounding.AwayFromZero),
                        Valor_Penalidade=item.Valor_penalidade,
                        Perc_Penalidade=item.Perc_penalidade,
                        Qtde_Parcelamento=item.Qtde_parcelamento
                    };
                    ex = parcelamentoRepository.Incluir_Parcelamento_Web_Origem(reg);
                    if (ex != null)
                        throw ex;

                }
            }

            _header = sistemaRepository.Contribuinte_Header(_codigo);
            bool _ativo = _header.Ativo;
            string _tipoDoc = "F";
            if (_header.Cpf_cnpj.Length > 11) {
                _tipoDoc = "J";
                if(_codigo>100000 && _codigo<300000) {
                    if (!_ativo)
                        _tipoDoc = "F"; //Empresas inativas são tratadas como Físicas
                }
            }

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
                Contribuinte_uf = _header.Nome_uf,
                Contribuinte_tipo=_tipoDoc
            };

            short _plano_codigo = 4; //(4=sem plano)
            Plano _plano = parcelamentoRepository.Retorna_Plano_Desconto(_plano_codigo);
            regP.Qtde_Maxima_Parcela = _plano.Qtde_Parcela;
            regP.Perc_Desconto = _plano.Desconto;
            regP.Plano_Codigo = _plano_codigo;
            regP.Plano_Nome = _plano.Nome;
            
            decimal _valor_minimo = parcelamentoRepository.Retorna_Parcelamento_Valor_Minimo((short)DateTime.Now.Year, false,_tipoDoc);
            regP.Valor_minimo = _valor_minimo;
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
                Plano_Codigo = _master.Plano_Codigo,
                Valor_Minimo = _master.Valor_minimo,
                Perc_desconto = _master.Perc_Desconto,
                Data_Vencimento = DateTime.Now.ToString("dd/MM/yyyy")
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

            model.Contribuinte = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Cpf_Cnpj =  Functions.FormatarCpfCnpj( _master.Contribuinte_cpfcnpj),
                Logradouro_Nome = _master.Contribuinte_endereco,
                Bairro_Nome = _master.Contribuinte_bairro+" - " + _master.Contribuinte_cidade + "/" + _master.Contribuinte_uf,
                Cep = _master.Contribuinte_cep.ToString("00000-000"),
                Tipo=_master.Contribuinte_tipo
            };
            return View(model);
        }

        [Route("Parc_reqb")]
        [HttpPost]
        public ActionResult Parc_reqb(ParcelamentoViewModel model) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            //Parcelamento_web_master regP = new Parcelamento_web_master() {
            //    Guid = model.Guid,
            //    Data_Vencimento=DateTime.Now
            //};
            //Exception ex = parcelamentoRepository.Atualizar_Criterio_Master(regP);
            //if (ex != null)
            //    throw ex;

            return RedirectToAction("Parc_reqc", new { p = model.Guid });
        }

        [Route("Parc_reqc")]
        [HttpGet]
        public ActionResult Parc_reqc(string p) {
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
                Plano_Nome=_master.Plano_Nome,
                Data_Vencimento=Convert.ToDateTime( _master.Data_Vencimento).ToString("dd/MM/yyyy"),
                Plano_Codigo=_master.Plano_Codigo,
                Valor_Minimo=_master.Valor_minimo,
                Perc_desconto=_master.Perc_Desconto
            };
            model.Requerente = new Parc_Requerente() {
                Codigo = _master.Requerente_Codigo,
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

            model.Contribuinte = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Contribuinte_cpfcnpj),
                Logradouro_Nome = _master.Contribuinte_endereco,
                Bairro_Nome = _master.Contribuinte_bairro + " - " + _master.Contribuinte_cidade + "/" + _master.Contribuinte_uf,
                Cep = _master.Contribuinte_cep.ToString("00000-000")
            };

            //Load Origem
            decimal _SomaP=0, _SomaM=0, _SomaJ=0, _SomaC=0,_SomaT=0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Origem(p);
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach (SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado=item.Ajuizado,
                    Complemento=item.Complemento,
                    Data_vencimento=item.Data_vencimento,
                    Exercicio=item.Exercicio,
                    Idx=item.Idx,
                    Lancamento=item.Lancamento,
                    Nome_lancamento=item.Nome_lancamento,
                    Parcela=item.Parcela,
                    Perc_penalidade=item.Perc_penalidade,
                    Qtde_parcelamento=item.Qtde_parcelamento,
                    Selected=item.Selected,
                    Sequencia=item.Sequencia,
                    Valor_correcao=item.Valor_correcao,
                    Valor_juros=item.Valor_juros,
                    Valor_multa=item.Valor_multa,
                    Valor_penalidade=item.Valor_penalidade,
                    Valor_principal=item.Valor_principal,
                    Valor_total=item.Valor_total
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaT += Math.Round(item.Valor_principal, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa, 2, MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao, 2, MidpointRounding.AwayFromZero);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Total = _SomaT;
            model.Lista_Origem = _listaP;
            return View(model);
        }

        [Route("Parc_reqc")]
        [HttpPost]
        public ActionResult Parc_reqc(ParcelamentoViewModel model) {

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            var selectedIds = model.getSelectedIds();
            int t = 1;
            List<SelectDebitoParcelamentoEditorViewModel> _listaOrigem = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach (SelectDebitoParcelamentoEditorViewModel item in model.Lista_Origem) {
                if (item.Selected) {
                    SelectDebitoParcelamentoEditorViewModel _r = new SelectDebitoParcelamentoEditorViewModel {
                        Ajuizado = item.Ajuizado,
                        Complemento = item.Complemento,
                        Data_vencimento = item.Data_vencimento,
                        Exercicio = item.Exercicio,
                        Idx = t,
                        Lancamento = item.Lancamento,
                        Nome_lancamento = item.Nome_lancamento,
                        Parcela = item.Parcela,
                        Perc_penalidade = item.Perc_penalidade,
                        Qtde_parcelamento = item.Qtde_parcelamento,
                        Selected = item.Selected,
                        Sequencia = item.Sequencia,
                        Valor_correcao = item.Valor_correcao,
                        Valor_juros = item.Valor_juros,
                        Valor_multa = item.Valor_multa,
                        Valor_penalidade = item.Valor_penalidade,
                        Valor_principal = item.Valor_principal,
                        Valor_total = item.Valor_total
                    };
                    _listaOrigem.Add(_r);
                    t++;
                }
            }
            model.Lista_Origem_Selected = _listaOrigem;
            t = 1;
            Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Selected(model.Guid);

            decimal _somaP = 0, _somaJ = 0, _somaM = 0, _somaC = 0, _somaT = 0,_somaE=0;
            foreach (SelectDebitoParcelamentoEditorViewModel item in _listaOrigem) {
                Parcelamento_web_selected reg = new Parcelamento_web_selected() {
                    Ajuizado = item.Ajuizado,
                    Ano = item.Exercicio,
                    Complemento = item.Complemento,
                    Data_Vencimento = item.Data_vencimento,
                    Guid = model.Guid,
                    Idx = t,
                    Lancamento = item.Lancamento,
                    Lancamento_Nome = item.Nome_lancamento,
                    Parcela = item.Parcela,
                    Qtde_Parcelamento = item.Qtde_parcelamento,
                    Perc_Penalidade = item.Perc_penalidade,
                    Valor_Correcao = item.Valor_correcao,
                    Valor_Juros = item.Valor_juros,
                    Valor_Multa = item.Valor_multa,
                    Valor_Penalidade = item.Valor_penalidade,
                    Valor_Total = item.Valor_total,
                    Valor_Tributo = item.Valor_principal,
                    Sequencia = item.Sequencia
                };
                _somaP += item.Valor_principal;
                _somaJ += item.Valor_juros;
                _somaM += item.Valor_multa;
                _somaC += item.Valor_correcao;
                _somaT += item.Valor_total;
                _somaE += item.Valor_penalidade;
                
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Selected(reg);
                t++;
            }

            decimal _percP = 0, _percJ = 0, _percM = 0, _percC = 0;
            decimal _valorAddP = 0, _valorAddJ = 0, _valorAddM = 0, _valorAddC = 0;
            _percP = _somaP * 100 / _somaT;
            _percJ = _somaJ * 100 / _somaT;
            _percM = _somaM * 100 / _somaT;
            _percC = _somaC * 100 / _somaT;

            _valorAddP = _somaE * _percP / 100;
            _valorAddJ = _somaE * _percJ / 100;
            _valorAddM = _somaE * _percM / 100;
            _valorAddC = _somaE * _percC / 100;

            Parcelamento_web_master regP = new Parcelamento_web_master() {
                Guid=model.Guid,
                Soma_Principal=_somaP,
                Soma_Juros=_somaJ,
                Soma_Multa=_somaM,
                Soma_Correcao=_somaC,
                Soma_Total=_somaT,
                Soma_Entrada=_somaE,
                Perc_Principal=_percP,
                Perc_Juros=_percJ,
                Perc_Multa=_percM,
                Perc_Correcao=_percC,
                Valor_add_Principal=_valorAddP,
                Valor_add_Juros=_valorAddJ,
                Valor_add_Multa=_valorAddM,
                Valor_add_Correcao=_valorAddC
            };
            ex = parcelamentoRepository.Atualizar_Totais_Master(regP);

            return RedirectToAction("Parc_reqd", new { p = model.Guid });

        }

        [Route("Parc_reqd")]
        [HttpGet]
        public ActionResult Parc_reqd(string p) {
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
                Plano_Nome = _master.Plano_Nome,
                Data_Vencimento=Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy"),
                Plano_Codigo = _master.Plano_Codigo,
                Valor_Minimo = _master.Valor_minimo,
                Perc_desconto = _master.Perc_Desconto
            };
            model.Requerente = new Parc_Requerente() {
                Codigo = _master.Requerente_Codigo,
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

            model.Contribuinte = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Contribuinte_cpfcnpj),
                Logradouro_Nome = _master.Contribuinte_endereco,
                Bairro_Nome = _master.Contribuinte_bairro + " - " + _master.Contribuinte_cidade + "/" + _master.Contribuinte_uf,
                Cep = _master.Contribuinte_cep.ToString("00000-000")
            };

            //Load Origem
            decimal _SomaP = 0, _SomaM = 0, _SomaJ = 0, _SomaC = 0, _SomaT = 0,_SomaE =0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            bool _bAjuizado = ListaOrigem[0].Ajuizado=="S";
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach (SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = item.Nome_lancamento,
                    Parcela = item.Parcela,
                    Perc_penalidade = item.Perc_penalidade,
                    Qtde_parcelamento = item.Qtde_parcelamento,
                    Selected = item.Selected,
                    Sequencia = item.Sequencia,
                    Valor_correcao = item.Valor_correcao,
                    Valor_juros = item.Valor_juros,
                    Valor_multa = item.Valor_multa,
                    Valor_penalidade = item.Valor_penalidade,
                    Valor_principal = item.Valor_principal,
                    Valor_total = item.Valor_total
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaE += item.Valor_penalidade;
                _SomaT += Math.Round(item.Valor_principal, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa, 2, MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao, 2, MidpointRounding.AwayFromZero);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Penalidade = _SomaE;
            model.Soma_Total = _SomaT;
            model.Lista_Origem_Selected = _listaP;

            //########### Carrega Simulado ###################################
            Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado(model.Guid);
            ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado_Resumo(model.Guid);
            List<Parcelamento_Web_Simulado> _listaSimulado = parcelamentoRepository.Lista_Parcelamento_Destino(model.Guid, (short)model.Plano_Codigo,DateTime.Now, _bAjuizado, _bAjuizado, _SomaP, _SomaJ, _SomaM, _SomaC, _SomaT, _SomaE, model.Valor_Minimo);
            IEnumerable<int> _listaQtde = _listaSimulado.Select(o => o.Qtde_Parcela).Distinct();

            decimal _valor1 = 0, _valorN = 0;
            List<Parc_Resumo> Lista_resumo = new List<Parc_Resumo>();
            foreach (int linha in _listaQtde) {
                foreach (Parcelamento_Web_Simulado item in _listaSimulado) {
                    if(item.Qtde_Parcela==linha && item.Numero_Parcela==1) {
                        _valor1 = item.Valor_Total;
                    } else {
                        if (item.Qtde_Parcela == linha && item.Numero_Parcela == 2) {
                            _valorN = item.Valor_Total;
                            break;
                        }
                    }
                }
                Parcelamento_Web_Simulado_Resumo t = new Parcelamento_Web_Simulado_Resumo() {
                    Guid=model.Guid,
                    Qtde_Parcela=linha,
                    Valor_Entrada = _valor1,
                    Valor_N = _valorN,
                    Valor_Total = _valor1 + (_valorN * (linha - 1))
                };
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Simulado_Resumo(t);
                Parc_Resumo r = new Parc_Resumo() {
                    Qtde_Parcela = linha,
                    Valor_Entrada="R$" + _valor1.ToString("#0.00"),
                    Valor_N = "R$" + _valorN.ToString("#0.00"),
                    Valor_Total = "R$" + t.Valor_Total.ToString("#0.00"),
                };
                if (linha == 2)
                    r.Selected = true;
                Lista_resumo.Add(r);
            }
            model.Lista_Resumo = Lista_resumo;

            //################################################################

            return View(model);
        }

        [Route("Parc_reqd")]
        [HttpPost]
        public ActionResult Parc_reqd(ParcelamentoViewModel model) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            foreach (Parc_Resumo item in model.Lista_Resumo) {
                if (item.Selected) {
                    Exception ex = parcelamentoRepository.Atualizar_QtdeParcela_Master(model.Guid, item.Qtde_Parcela);
                    break;
                }
            }

            return RedirectToAction("Parc_reqe", new { p = model.Guid });
        }

        [Route("Parc_reqe")]
        [HttpGet]
        public ActionResult Parc_reqe(string p) {
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
                Plano_Nome = _master.Plano_Nome,
                Data_Vencimento = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy"),
                Plano_Codigo = _master.Plano_Codigo,
                Valor_Minimo = _master.Valor_minimo,
                Perc_desconto = _master.Perc_Desconto
            };
            model.Requerente = new Parc_Requerente() {
                Codigo = _master.Requerente_Codigo,
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

            model.Contribuinte = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Contribuinte_cpfcnpj),
                Logradouro_Nome = _master.Contribuinte_endereco,
                Bairro_Nome = _master.Contribuinte_bairro + " - " + _master.Contribuinte_cidade + "/" + _master.Contribuinte_uf,
                Cep = _master.Contribuinte_cep.ToString("00000-000")
            };

            //Load Origem
            decimal _SomaP = 0, _SomaM = 0, _SomaJ = 0, _SomaC = 0, _SomaT = 0, _SomaE = 0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            bool _bAjuizado = ListaOrigem[0].Ajuizado == "S";
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach (SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = item.Nome_lancamento,
                    Parcela = item.Parcela,
                    Perc_penalidade = item.Perc_penalidade,
                    Qtde_parcelamento = item.Qtde_parcelamento,
                    Selected = item.Selected,
                    Sequencia = item.Sequencia,
                    Valor_correcao = item.Valor_correcao,
                    Valor_juros = item.Valor_juros,
                    Valor_multa = item.Valor_multa,
                    Valor_penalidade = item.Valor_penalidade,
                    Valor_principal = item.Valor_principal,
                    Valor_total = item.Valor_total
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaE += item.Valor_penalidade;
                _SomaT += Math.Round(item.Valor_principal, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros, 2, MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa, 2, MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao, 2, MidpointRounding.AwayFromZero);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Penalidade = _SomaE;
            model.Soma_Total = _SomaT;
            model.Lista_Origem_Selected = _listaP;

            //Carrega Simulado
            model.Lista_Simulado=parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid, _master.Qtde_Parcela);

            //Atualiza Totais
            decimal _SomaH = 0, _SomaJapl = 0, _SomaL = 0;
            _SomaP = 0;_SomaC = 0;_SomaJ = 0;_SomaM = 0;_SomaT = 0;
            
            foreach (Parcelamento_Web_Simulado item in model.Lista_Simulado) {
                _SomaL += item.Valor_Liquido;
                _SomaJ += item.Valor_Juros;
                _SomaM += item.Valor_Multa;
                _SomaC += item.Valor_Correcao;
                _SomaP += item.Valor_Principal;
                _SomaT += item.Valor_Total;
                _SomaH += item.Valor_Honorario;
                _SomaJapl += item.Juros_Apl;
            }

            Parcelamento_web_master reg = new Parcelamento_web_master() {
                Guid=model.Guid,
                Sim_Correcao=_SomaC,
                Sim_Honorario=_SomaH,
                Sim_Juros=_SomaJ,
                Sim_Juros_apl=_SomaJapl,
                Sim_Liquido=_SomaL,
                Sim_Multa=_SomaM,
                Sim_Perc_Correcao= _SomaC * 100 / _SomaT,
                Sim_Perc_Honorario= _SomaH * 100 / _SomaT,
                Sim_Perc_Juros= _SomaJ * 100 / _SomaT,
                Sim_Perc_Juros_Apl= _SomaJapl * 100 / _SomaT,
                Sim_Perc_Liquido= _SomaL * 100 / _SomaT,
                Sim_Perc_Multa= _SomaM * 100 / _SomaT,
                Sim_Principal= _SomaP ,
                Sim_Total=_SomaT
            };
            Exception ex = parcelamentoRepository.Atualizar_Simulado_Master(reg);

            return View(model);
        }









        //[ChildActionOnly]
        //public ActionResult Parc_simulado(string p) {

        //    Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
        //    //Load Master
        //    Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
        //    SelectDebitoParcelamentoEditorViewModel model = new SelectDebitoParcelamentoEditorViewModel() {

        //    };

        //    return PartialView("Parc_simulado", model);
        //}

    }
}