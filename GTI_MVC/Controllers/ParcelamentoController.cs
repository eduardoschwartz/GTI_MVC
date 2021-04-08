using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Parcelamento.EditorTemplates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

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
                List<Parcelamento_web_lista_codigo> _listaCodigo = new List<Parcelamento_web_lista_codigo>();
                foreach (Parc_Codigos item in _listaCodigos) {
                    Parcelamento_web_lista_codigo _cod = new Parcelamento_web_lista_codigo() {
                        Guid = model.Guid,
                        Codigo = item.Codigo,
                        Tipo = item.Tipo,
                        Documento = item.Cpf_Cnpj,
                        Descricao = item.Descricao,
                        Selected = item.Selected
                    };
                    _listaCodigo.Add(_cod);
                }
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Lista_Codigo(_listaCodigo);
                if (ex != null)
                    throw ex;

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
                List<Parcelamento_web_origem> _listaWebOrigem = new List<Parcelamento_web_origem>();
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
                    _listaWebOrigem.Add(reg);
                }
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Origem(_listaWebOrigem);
                if (ex != null)
                    throw ex;

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

            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(model.Guid);
            model.Sim_Correcao = _master.Sim_Correcao;
            model.Sim_Honorario = _master.Sim_Honorario;
            model.Sim_Juros = _master.Sim_Juros;
            model.Sim_Juros_apl = _master.Sim_Juros_apl;
            model.Sim_Liquido = _master.Sim_Liquido;
            model.Sim_Multa = _master.Sim_Multa;
            model.Sim_Principal = _master.Sim_Principal;
            model.Sim_Total = _master.Sim_Total;

            var selectedIds = model.getSelectedIds();
            int t = 1;
            decimal _totalSel = 0;
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
                    _totalSel += item.Valor_total;
                    _listaOrigem.Add(_r);
                    t++;
                }
            }
            model.Lista_Origem_Selected = _listaOrigem;
            if (_totalSel /2 < model.Valor_Minimo) {
                ViewBag.Result = "Valor mínimo da parcela deve ser de R$" + model.Valor_Minimo.ToString("#0.00");
                return View(model);
            }
            t = 1;
            Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Selected(model.Guid);
            bool _ajuizado = false;
            decimal _somaP = 0, _somaJ = 0, _somaM = 0, _somaC = 0, _somaT = 0,_somaE=0;
            List<Parcelamento_web_selected> _listaSelect = new List<Parcelamento_web_selected>();
            foreach (SelectDebitoParcelamentoEditorViewModel item in _listaOrigem) {
                _ajuizado = item.Ajuizado == "S" ? true : false;
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
                _listaSelect.Add(reg);
                
                t++;
            }

            ex = parcelamentoRepository.Incluir_Parcelamento_Web_Selected(_listaSelect);

            //Atualiza Totais
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
                Guid = model.Guid,
                Soma_Principal = _somaP,
                Soma_Juros = _somaJ,
                Soma_Multa = _somaM,
                Soma_Correcao = _somaC,
                Soma_Total = _somaT,
                Soma_Entrada = _somaE,
                Perc_Principal = _percP,
                Perc_Juros = _percJ,
                Perc_Multa = _percM,
                Perc_Correcao = _percC,
                Valor_add_Principal = _valorAddP,
                Valor_add_Juros = _valorAddJ,
                Valor_add_Multa = _valorAddM,
                Valor_add_Correcao = _valorAddC
            };
            ex = parcelamentoRepository.Atualizar_Totais_Master(regP);

            //Grava os tributos

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(model.Guid);
            List<Parcelamento_Web_Tributo> _listaTributo = new List<Parcelamento_Web_Tributo>();
            decimal _total =0;
            foreach (SpParcelamentoOrigem item in _listaSelected) {
                List<SpExtrato> _listaExtrato = tributarioRepository.Lista_Extrato_Tributo(model.Contribuinte.Codigo, item.Exercicio, item.Exercicio, item.Lancamento, item.Lancamento, item.Sequencia, item.Sequencia, item.Parcela, item.Parcela, item.Complemento, item.Complemento);
                foreach (SpExtrato _ext in _listaExtrato) {
                    bool _find = false;
                    int _pos = 0;
                    foreach (Parcelamento_Web_Tributo _trib in _listaTributo) {
                        if (_trib.Tributo == _ext.Codtributo) {
                            _find = true;
                            break;
                        }
                        _pos++;
                    }
                    _total += _ext.Valortributo;
                    if (!_find) {
                        Parcelamento_Web_Tributo regT = new Parcelamento_Web_Tributo() {
                            Guid=model.Guid,
                            Tributo=_ext.Codtributo,
                            Valor=_ext.Valortributo,
                            Perc=0
                        };
                        _listaTributo.Add(regT);
                    }
                    else{
                        _listaTributo[_pos].Valor += _ext.Valortributo;
                    }
                }
            }

            for (int i = 0; i < _listaTributo.Count; i++) {
                _listaTributo[i].Perc = _listaTributo[i].Valor * 100 / _total;
            }

            //Grava Honorario(90), Juros(113), Multa(112), Correcao(26) e Juros Apl(585)
            Parcelamento_Web_Tributo r = null;
            if (_somaJ > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 113,
                    Valor = _somaJ,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if (_ajuizado) {
                 r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 90,
                    Valor = _somaT * 10 / 100,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if (_somaM > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 112,
                    Valor = _somaM,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if (_somaC > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 26,
                    Valor = _somaC,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }


            ex = parcelamentoRepository.Incluir_Parcelamento_Web_Tributo(_listaTributo);

            //###################

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
            List<Parcelamento_Web_Simulado_Resumo> _lista_Web_Simulado_Resumo = new List<Parcelamento_Web_Simulado_Resumo>();
            foreach (int linha in _listaQtde) {
                foreach (Parcelamento_Web_Simulado item in _listaSimulado) {
                    if(item.Qtde_Parcela==linha && item.Numero_Parcela==1) {
                        _valor1 = item.Valor_Total;
                    } else {
                        if (item.Qtde_Parcela == linha && item.Numero_Parcela == 2) {
                            if (item.Valor_Total  < model.Valor_Minimo)
                                goto Fim;
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
                _lista_Web_Simulado_Resumo.Add(t);
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
            ex = parcelamentoRepository.Incluir_Parcelamento_Web_Simulado_Resumo(_lista_Web_Simulado_Resumo);
        Fim:;
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
            model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid, _master.Qtde_Parcela);

            //Atualiza Totais
            decimal _SomaH = 0, _SomaJapl = 0, _SomaL = 0;
            _SomaP = 0; _SomaC = 0; _SomaJ = 0; _SomaM = 0; _SomaT = 0;

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
                Guid = model.Guid,
                Sim_Correcao = _SomaC,
                Sim_Honorario = _SomaH,
                Sim_Juros = _SomaJ,
                Sim_Juros_apl = _SomaJapl,
                Sim_Liquido = _SomaL,
                Sim_Multa = _SomaM,
                Sim_Perc_Correcao = _SomaC * 100 / _SomaT,
                Sim_Perc_Honorario = _SomaH * 100 / _SomaT,
                Sim_Perc_Juros = _SomaJ * 100 / _SomaT,
                Sim_Perc_Juros_Apl = _SomaJapl * 100 / _SomaT,
                Sim_Perc_Liquido = _SomaL * 100 / _SomaT,
                Sim_Perc_Multa = _SomaM * 100 / _SomaT,
                Sim_Principal = _SomaP,
                Sim_Total = _SomaT
            };
            Exception ex = parcelamentoRepository.Atualizar_Simulado_Master(reg);
            model.Sim_Correcao = _SomaC;
            model.Sim_Honorario = _SomaH;
            model.Sim_Juros = _SomaJ;
            model.Sim_Juros_apl = _SomaJapl;
            model.Sim_Liquido = _SomaL;
            model.Sim_Multa = _SomaM;
            model.Sim_Principal = _SomaP;
            model.Sim_Total = _SomaT;

            return View(model);
        }

        [Route("Parc_reqe")]
        [HttpPost]
        public ActionResult Parc_reqe(ParcelamentoViewModel model, string action, string value) {
            if (Session["hashid"] == null) {
                return RedirectToAction("Login", "Home");
            }

            //int _userId = Convert.ToInt32(Session["hashid"]);
            //bool _userWeb = Session["hashfunc"].ToString() == "S" ? false : true;

            //string _guid = model.Guid;

            //Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            //Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(_guid);
            //List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(_guid);
            //IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
            //int _codigoR = _master.Requerente_Codigo;
            //int _codigoC = _master.Contribuinte_Codigo;
            //int _qtdeParc = _master.Qtde_Parcela;

            ////Criar Processo
            //Processo_bll protocoloRepository = new Processo_bll(_connection);
            //short _ano =(short) DateTime.Now.Year;
            //int _numero = protocoloRepository.Retorna_Numero_Disponivel(_ano);
            //string _compl = "PARCELAMENTO DE DÉBITOS CÓD: " + _codigoC.ToString();
            //string _obs = "Exercícios: ";
            //foreach (short _anoP in _listaAnos) {
            //    _obs += _anoP.ToString() + ", ";
            //}
            //_obs = _obs.Substring(0, _obs.Length - 1) + " parcelado em: " + _qtdeParc.ToString() + " vezes.";

            //Processogti _p = new Processogti() {
            //    Ano=_ano,
            //    Numero=_numero,
            //    Interno=false,
            //    Fisico=false,
            //    Hora=DateTime.Now.ToString("hh:mm"),
            //    Userid=_userId,
            //    Codassunto=606,
            //    Complemento=_compl,
            //    Observacao=_obs,
            //    Codcidadao=_codigoR,
            //    Dataentrada=DateTime.Now,
            //    Origem=2,
            //    Insc=_codigoC,
            //    Tipoend="R",
            //    Etiqueta=false,
            //    Userweb=_userWeb
            //};
            //Exception ex = protocoloRepository.Incluir_Processo(_p);
            //ex = parcelamentoRepository.Atualizar_Processo_Master(_guid, _ano, _numero);

            ////Grava tabela web_destino com as parcelas do simulado
            //model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid, _master.Qtde_Parcela);
            //ex = parcelamentoRepository.Excluir_parcelamento_Web_Destino(model.Guid);
            //List<Parcelamento_Web_Destino> _lista_Parcelamento_Web_Destino = new List<Parcelamento_Web_Destino>();
            //foreach (Parcelamento_Web_Simulado _s in model.Lista_Simulado.Where(m=>m.Qtde_Parcela==_qtdeParc)) {
            //    Parcelamento_Web_Destino _d = new Parcelamento_Web_Destino() {
            //        Data_Vencimento = _s.Data_Vencimento,
            //        Guid = _s.Guid,
            //        Numero_Parcela = _s.Numero_Parcela,
            //        Juros_Apl = _s.Juros_Apl,
            //        Juros_Mes = _s.Juros_Mes,
            //        Juros_Perc = _s.Juros_Perc,
            //        Saldo = _s.Saldo,
            //        Valor_Correcao = _s.Valor_Correcao,
            //        Valor_Honorario = _s.Valor_Honorario,
            //        Valor_Juros = _s.Valor_Juros,
            //        Valor_Liquido = _s.Valor_Liquido,
            //        Valor_Multa = _s.Valor_Multa,
            //        Valor_Principal = _s.Valor_Principal,
            //        Valor_Total = _s.Valor_Total,
            //        Proporcao = _s.Valor_Liquido * 100 / _master.Sim_Liquido
            //    };
            //    _lista_Parcelamento_Web_Destino.Add(_d);
            //}
            //ex = parcelamentoRepository.Incluir_Parcelamento_Web_Destino(_lista_Parcelamento_Web_Destino);

            ////Apaga o simulado
            //ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado(_guid);

            ////Apaga a origem
            //ex = parcelamentoRepository.Excluir_parcelamento_Web_Origem(_guid);

            ////Apaga os códigos
            //ex = parcelamentoRepository.Excluir_parcelamento_Web_Lista_Codigo(_guid);

            ////grava tabela processoreparc
            //string _numProc = _numero.ToString() + "/" + _ano.ToString();
            //Processoreparc reg = new Processoreparc() {
            //    Numprocesso = _numProc,
            //    Anoproc = _ano,
            //    Numproc = _numero,
            //    Codigoresp = _codigoC,
            //    Dataprocesso = DateTime.Now,
            //    Datareparc = DateTime.Now,
            //    Calculacorrecao = true,
            //    Calculajuros = true,
            //    Calculamulta = true,
            //    Penhora = false,
            //    Honorario = true,
            //    Novo = true,
            //    Userid = _userId,
            //    Userweb = _userWeb,
            //    Qtdeparcela = Convert.ToByte(_qtdeParc),
            //    Plano = _master.Plano_Codigo.ToString(),
            //    Valorentrada = 0,
            //    Percentrada = 0
            //};
            //ex = parcelamentoRepository.Incluir_ProcessoReparc(reg);

            ////grava tabela origemreparc
            //List<Origemreparc> _listaOrigem = new List<Origemreparc>();
            //foreach (SpParcelamentoOrigem item in _listaSelected) {
            //    Origemreparc _o = new Origemreparc() {
            //        Numprocesso=_numProc,
            //        Anoproc=_ano,
            //        Numproc=_numero,
            //        Codreduzido=_codigoC,
            //        Anoexercicio=item.Exercicio,
            //        Codlancamento=item.Lancamento,
            //        Numsequencia=(byte)item.Sequencia,
            //        Numparcela=(byte)item.Parcela,
            //        Codcomplemento=(byte)item.Complemento,
            //        Principal=item.Valor_principal,
            //        Multa=item.Valor_multa,
            //        Juros=item.Valor_juros,
            //        Correcao=item.Valor_correcao
            //    };
            //    _listaOrigem.Add(_o);    
            //}
            //ex = parcelamentoRepository.Incluir_OrigemReparc(_listaOrigem);

            ////grava tabela origemreparc
            //byte _lastSeq = parcelamentoRepository.Retorna_Seq_Disponivel(_codigoC);

            //List<Parcelamento_Web_Destino> _listaDestino = parcelamentoRepository.Lista_Parcelamento_Web_Destino(_guid);
            ////Adiiona o tributo 585 Juros.Apl lista de tributos
            //List<Parcelamento_Web_Tributo> _listaTributo = new List<Parcelamento_Web_Tributo>();
            //Parcelamento_Web_Tributo r = new Parcelamento_Web_Tributo() {
            //    Guid = model.Guid,
            //    Tributo = 585,
            //    Valor = _lista_Parcelamento_Web_Destino[0].Juros_Apl,
            //    Perc = 100
            //};
            //_listaTributo.Add(r);
            //ex = parcelamentoRepository.Incluir_Parcelamento_Web_Tributo(_listaTributo);

            //List<Destinoreparc> _listaDestinoReparc = new List<Destinoreparc>();
            //List<Debitoparcela> _listaDebitoParcela = new List<Debitoparcela>();
            //List<Debitotributo> _listaDebitoTributo = new List<Debitotributo>();
            //foreach (Parcelamento_Web_Destino item in _listaDestino) {
            //    Destinoreparc _d = new Destinoreparc() {
            //        Numprocesso = _numProc,
            //        Anoproc = _ano,
            //        Numproc = _numero,
            //        Codreduzido = _codigoC,
            //        Anoexercicio = (short)item.Data_Vencimento.Year,
            //        Codlancamento = 20,
            //        Numsequencia = _lastSeq,
            //        Numparcela = (byte)item.Numero_Parcela,
            //        Codcomplemento = 0,
            //        Valorliquido = item.Valor_Liquido,
            //        Multa = item.Valor_Multa,
            //        Juros = item.Valor_Juros,
            //        Correcao = item.Valor_Correcao,
            //        Valorprincipal = item.Valor_Principal,
            //        Honorario = item.Valor_Honorario,
            //        Jurosapl = item.Juros_Apl,
            //        Jurosperc = item.Juros_Perc,
            //        Jurosvalor = item.Juros_Mes,
            //        Saldo = item.Saldo,
            //        Total = item.Valor_Total
            //    };
            //    _listaDestinoReparc.Add(_d);

            //    byte _status;
            //    if (item.Numero_Parcela == 1)
            //        _status = 3;
            //    else
            //        _status= 18;
            //    Debitoparcela dp = new Debitoparcela() {
            //        Codreduzido=_d.Codreduzido,
            //        Anoexercicio=_d.Anoexercicio,
            //        Codlancamento=_d.Codlancamento,
            //        Seqlancamento=_d.Numsequencia,
            //        Numparcela=_d.Numparcela,
            //        Codcomplemento=_d.Codcomplemento,
            //        Datadebase=DateTime.Now,
            //        Datavencimento=item.Data_Vencimento,
            //        Numprocesso=_numProc,
            //        Statuslanc=_status
            //    };
            //    _listaDebitoParcela.Add(dp);

            //    //Gravar os tributos
            //    decimal _Perc1 = _lista_Parcelamento_Web_Destino[0].Proporcao;
            //    decimal _PercN = _lista_Parcelamento_Web_Destino[1].Proporcao;

            //List<Parcelamento_Web_Tributo> _ListaTributo = parcelamentoRepository.Lista_Parcelamento_Tributo(model.Guid);
            //    foreach (Parcelamento_Web_Tributo trib in _ListaTributo) {
            //        Debitotributo _dt = new Debitotributo() {
            //            Codreduzido=dp.Codreduzido,
            //            Anoexercicio=dp.Anoexercicio,
            //            Codlancamento=dp.Codlancamento,
            //            Seqlancamento=dp.Seqlancamento,
            //            Numparcela=dp.Numparcela,
            //            Codcomplemento=dp.Codcomplemento,
            //            Codtributo=(short)trib.Tributo

            //        };
            //        if(_dt.Numparcela==1)
            //            _dt.Valortributo = trib.Valor*_Perc1/100;
            //        else
            //            _dt.Valortributo = trib.Valor*_PercN/100;

            //        if(_dt.Codtributo==585)
            //            _dt.Valortributo = trib.Valor ;
            //        _listaDebitoTributo.Add(_dt);
            //    }
            //}

            //ex = parcelamentoRepository.Incluir_DestinoReparc(_listaDestinoReparc);
            //ex = parcelamentoRepository.Incluir_Debito_Parcela(_listaDebitoParcela);
            //ex = parcelamentoRepository.Incluir_Debito_Tributo(_listaDebitoTributo);
            //ex = parcelamentoRepository.Atualizar_Status_Origem(_codigoC, _listaSelected);

            //return RedirectToAction("Parc_reqf", new { p = model.Guid });
            return RedirectToAction("Parc_tan", new { p = model.Guid });
        }

        [Route("Parc_tan")]
        [HttpGet]
        public ActionResult Parc_tan(string p) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            ParcelamentoViewModel model = new ParcelamentoViewModel();
            model.Guid = p;
            return View(model);
        }

        [Route("Parc_tan")]
        [HttpPost]
        public ActionResult Parc_tan(ParcelamentoViewModel model, string action) {

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            string _guid = model.Guid;

            if (action == "btnYes") {
                int _userId = Convert.ToInt32(Session["hashid"]);
                bool _userWeb = Session["hashfunc"].ToString() == "S" ? false : true;
                Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(_guid);
                List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(_guid);
                IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
                int _codigoR = _master.Requerente_Codigo;
                int _codigoC = _master.Contribuinte_Codigo;
                int _qtdeParc = _master.Qtde_Parcela;


                //Criar Processo
                Processo_bll protocoloRepository = new Processo_bll(_connection);
                short _ano = (short)DateTime.Now.Year;
                int _numero = protocoloRepository.Retorna_Numero_Disponivel(_ano);
                string _compl = "PARCELAMENTO DE DÉBITOS CÓD: " + _codigoC.ToString();
                string _obs = "Exercícios: ";
                foreach (short _anoP in _listaAnos) {
                    _obs += _anoP.ToString() + ", ";
                }
                _obs = _obs.Substring(0, _obs.Length - 1) + " parcelado em: " + _qtdeParc.ToString() + " vezes.";

                Processogti _p = new Processogti() {
                    Ano = _ano,
                    Numero = _numero,
                    Interno = false,
                    Fisico = false,
                    Hora = DateTime.Now.ToString("hh:mm"),
                    Userid = _userId,
                    Codassunto = 606,
                    Complemento = _compl,
                    Observacao = _obs,
                    Codcidadao = _codigoR,
                    Dataentrada = DateTime.Now,
                    Origem = 2,
                    Insc = _codigoC,
                    Tipoend = "R",
                    Etiqueta = false,
                    Userweb = _userWeb
                };
                Exception ex = protocoloRepository.Incluir_Processo(_p);
                ex = parcelamentoRepository.Atualizar_Processo_Master(_guid, _ano, _numero);
                return RedirectToAction("Parc_tcd", new { p = model.Guid });
            } else {
                if (action == "btPrint") {
                    return Termo_anuencia_print(model.Guid);
                } 
            }
            return RedirectToAction("Login", "Home");
        }

        [Route("Parc_tcd")]
        [HttpGet]
        public ActionResult Parc_tcd(string p) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            string _guid = p;
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(_guid);

            string _endereco = _master.Contribuinte_endereco + " " + _master.Contribuinte_bairro;

            ParcelamentoViewModel model = new ParcelamentoViewModel();
            Parc_Contribuinte pc = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Logradouro_Nome=_master.Contribuinte_Codigo<50000?_endereco:""
            };
            Parc_Requerente pr = new Parc_Requerente() {
                Nome = _master.Requerente_Nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Requerente_CpfCnpj)
            };

            List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
            string _ano = "";
            foreach (short item in _listaAnos) {
                _ano += item.ToString() + ", ";
            }
            _ano = _ano.Substring(0, _ano.Length - 2);


            model.Contribuinte = pc;
            model.Requerente = pr;
            model.Qtde_Parcela = _master.Qtde_Parcela;
            model.Soma_Total = _master.Sim_Total;
            model.NumeroProcesso = _master.Processo_Numero.ToString() + "-" + Functions.RetornaDvProcesso(_master.Processo_Numero) + "/" + _master.Processo_Ano.ToString();
            model.Data_Vencimento = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy");
            model.Exercicios = _ano;
            model.Guid = p;
            return View(model);
        }

        [Route("Parc_tcd")]
        [HttpPost]
        public ActionResult Parc_tcd(ParcelamentoViewModel model, string action) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            int _userId = Convert.ToInt32(Session["hashid"]);
            bool _userWeb = Session["hashfunc"].ToString() == "S" ? false : true;
            string _guid = model.Guid;

            if (action == "btnYes") {

                Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(_guid);
                List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(_guid);
                IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
                int _codigoR = _master.Requerente_Codigo;
                int _codigoC = _master.Contribuinte_Codigo;
                int _qtdeParc = _master.Qtde_Parcela;
                short _ano = _master.Processo_Ano;
                int _numero = _master.Processo_Numero;

                //Grava tabela web_destino com as parcelas do simulado
                model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid, _master.Qtde_Parcela);
                Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Destino(model.Guid);
                List<Parcelamento_Web_Destino> _lista_Parcelamento_Web_Destino = new List<Parcelamento_Web_Destino>();
                foreach (Parcelamento_Web_Simulado _s in model.Lista_Simulado.Where(m => m.Qtde_Parcela == _qtdeParc)) {
                    Parcelamento_Web_Destino _d = new Parcelamento_Web_Destino() {
                        Data_Vencimento = _s.Data_Vencimento,
                        Guid = _s.Guid,
                        Numero_Parcela = _s.Numero_Parcela,
                        Juros_Apl = _s.Juros_Apl,
                        Juros_Mes = _s.Juros_Mes,
                        Juros_Perc = _s.Juros_Perc,
                        Saldo = _s.Saldo,
                        Valor_Correcao = _s.Valor_Correcao,
                        Valor_Honorario = _s.Valor_Honorario,
                        Valor_Juros = _s.Valor_Juros,
                        Valor_Liquido = _s.Valor_Liquido,
                        Valor_Multa = _s.Valor_Multa,
                        Valor_Principal = _s.Valor_Principal,
                        Valor_Total = _s.Valor_Total,
                        Proporcao = _s.Valor_Liquido * 100 / _master.Sim_Liquido
                    };
                    _lista_Parcelamento_Web_Destino.Add(_d);
                }
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Destino(_lista_Parcelamento_Web_Destino);

                //Apaga o simulado
                ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado(_guid);

                //Apaga a origem
                ex = parcelamentoRepository.Excluir_parcelamento_Web_Origem(_guid);

                //Apaga os códigos
                ex = parcelamentoRepository.Excluir_parcelamento_Web_Lista_Codigo(_guid);

                //grava tabela processoreparc
                string _numProc = _numero.ToString() + "/" + _ano.ToString();
                Processoreparc reg = new Processoreparc() {
                    Numprocesso = _numProc,
                    Anoproc = _ano,
                    Numproc = _numero,
                    Codigoresp = _codigoC,
                    Dataprocesso = DateTime.Now,
                    Datareparc = DateTime.Now,
                    Calculacorrecao = true,
                    Calculajuros = true,
                    Calculamulta = true,
                    Penhora = false,
                    Honorario = true,
                    Novo = true,
                    Userid = _userId,
                    Userweb = _userWeb,
                    Qtdeparcela = Convert.ToByte(_qtdeParc),
                    Plano = _master.Plano_Codigo.ToString(),
                    Valorentrada = 0,
                    Percentrada = 0
                };
                ex = parcelamentoRepository.Incluir_ProcessoReparc(reg);

                //grava tabela origemreparc
                List<Origemreparc> _listaOrigem = new List<Origemreparc>();
                foreach (SpParcelamentoOrigem item in _listaSelected) {
                    Origemreparc _o = new Origemreparc() {
                        Numprocesso = _numProc,
                        Anoproc = _ano,
                        Numproc = _numero,
                        Codreduzido = _codigoC,
                        Anoexercicio = item.Exercicio,
                        Codlancamento = item.Lancamento,
                        Numsequencia = (byte)item.Sequencia,
                        Numparcela = (byte)item.Parcela,
                        Codcomplemento = (byte)item.Complemento,
                        Principal = item.Valor_principal,
                        Multa = item.Valor_multa,
                        Juros = item.Valor_juros,
                        Correcao = item.Valor_correcao
                    };
                    _listaOrigem.Add(_o);
                }
                ex = parcelamentoRepository.Incluir_OrigemReparc(_listaOrigem);

                //grava tabela origemreparc
                byte _lastSeq = parcelamentoRepository.Retorna_Seq_Disponivel(_codigoC);

                List<Parcelamento_Web_Destino> _listaDestino = parcelamentoRepository.Lista_Parcelamento_Web_Destino(_guid);
                //Adiiona o tributo 585 Juros.Apl lista de tributos
                List<Parcelamento_Web_Tributo> _listaTributo = new List<Parcelamento_Web_Tributo>();
                Parcelamento_Web_Tributo r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 585,
                    Valor = _lista_Parcelamento_Web_Destino[0].Juros_Apl,
                    Perc = 100
                };
                _listaTributo.Add(r);
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Tributo(_listaTributo);

                List<Destinoreparc> _listaDestinoReparc = new List<Destinoreparc>();
                List<Debitoparcela> _listaDebitoParcela = new List<Debitoparcela>();
                List<Debitotributo> _listaDebitoTributo = new List<Debitotributo>();
                foreach (Parcelamento_Web_Destino item in _listaDestino) {
                    Destinoreparc _d = new Destinoreparc() {
                        Numprocesso = _numProc,
                        Anoproc = _ano,
                        Numproc = _numero,
                        Codreduzido = _codigoC,
                        Anoexercicio = (short)item.Data_Vencimento.Year,
                        Codlancamento = 20,
                        Numsequencia = _lastSeq,
                        Numparcela = (byte)item.Numero_Parcela,
                        Codcomplemento = 0,
                        Valorliquido = item.Valor_Liquido,
                        Multa = item.Valor_Multa,
                        Juros = item.Valor_Juros,
                        Correcao = item.Valor_Correcao,
                        Valorprincipal = item.Valor_Principal,
                        Honorario = item.Valor_Honorario,
                        Jurosapl = item.Juros_Apl,
                        Jurosperc = item.Juros_Perc,
                        Jurosvalor = item.Juros_Mes,
                        Saldo = item.Saldo,
                        Total = item.Valor_Total
                    };
                    _listaDestinoReparc.Add(_d);

                    byte _status;
                    if (item.Numero_Parcela == 1)
                        _status = 3;
                    else
                        _status = 18;
                    Debitoparcela dp = new Debitoparcela() {
                        Codreduzido = _d.Codreduzido,
                        Anoexercicio = _d.Anoexercicio,
                        Codlancamento = _d.Codlancamento,
                        Seqlancamento = _d.Numsequencia,
                        Numparcela = _d.Numparcela,
                        Codcomplemento = _d.Codcomplemento,
                        Datadebase = DateTime.Now,
                        Datavencimento = item.Data_Vencimento,
                        Numprocesso = _numProc,
                        Statuslanc = _status
                    };
                    _listaDebitoParcela.Add(dp);

                    //Gravar os tributos
                    decimal _Perc1 = _lista_Parcelamento_Web_Destino[0].Proporcao;
                    decimal _PercN = _lista_Parcelamento_Web_Destino[1].Proporcao;

                    List<Parcelamento_Web_Tributo> _ListaTributo = parcelamentoRepository.Lista_Parcelamento_Tributo(model.Guid);
                    foreach (Parcelamento_Web_Tributo trib in _ListaTributo) {
                        Debitotributo _dt = new Debitotributo() {
                            Codreduzido = dp.Codreduzido,
                            Anoexercicio = dp.Anoexercicio,
                            Codlancamento = dp.Codlancamento,
                            Seqlancamento = dp.Seqlancamento,
                            Numparcela = dp.Numparcela,
                            Codcomplemento = dp.Codcomplemento,
                            Codtributo = (short)trib.Tributo

                        };
                        if (_dt.Numparcela == 1)
                            _dt.Valortributo = trib.Valor * _Perc1 / 100;
                        else
                            _dt.Valortributo = trib.Valor * _PercN / 100;

                        if (_dt.Codtributo == 585)
                            _dt.Valortributo = trib.Valor;
                        _listaDebitoTributo.Add(_dt);
                    }
                }

                ex = parcelamentoRepository.Incluir_DestinoReparc(_listaDestinoReparc);
                ex = parcelamentoRepository.Incluir_Debito_Parcela(_listaDebitoParcela);
                ex = parcelamentoRepository.Incluir_Debito_Tributo(_listaDebitoTributo);
                ex = parcelamentoRepository.Atualizar_Status_Origem(_codigoC, _listaSelected);

                return RedirectToAction("Parc_reqf", new { p = model.Guid });
            } else {
                if (action == "btPrint") {
                    return Termo_confissao_divida(model.Guid);
                }
            }
            return RedirectToAction("Login", "Home");
        }

        [Route("Parc_reqf")]
        [HttpGet]
        public ActionResult Parc_reqf(string p) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if (!_existe)
                return RedirectToAction("Login_gti", "Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            string _numProcesso = _master.Processo_Numero.ToString()+"-"+Functions.RetornaDvProcesso(_master.Processo_Numero)+"/"+_master.Processo_Ano.ToString();
            ParcelamentoViewModel model = new ParcelamentoViewModel() {
                Guid = p,
                Plano_Nome = _master.Plano_Nome,
                Data_Vencimento = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy"),
                Plano_Codigo = _master.Plano_Codigo,
                Valor_Minimo = _master.Valor_minimo,
                Perc_desconto = _master.Perc_Desconto,
                Processo_ano=_master.Processo_Ano,
                Processo_numero=_master.Processo_Numero,
                NumeroProcesso=_numProcesso,
                Sim_Correcao=_master.Sim_Correcao,
                Sim_Honorario=_master.Sim_Honorario,
                Sim_Juros=_master.Sim_Juros,
                Sim_Juros_apl=_master.Sim_Juros_apl,
                Sim_Liquido=_master.Sim_Liquido,
                Sim_Multa=_master.Sim_Multa,
                Sim_Principal=_master.Sim_Principal,
                Sim_Total=_master.Sim_Total,
                Soma_Correcao=_master.Soma_Correcao,
                Soma_Juros=_master.Soma_Juros,
                Soma_Multa=_master.Soma_Multa,
                Soma_Penalidade=_master.Soma_Entrada,
                Soma_Principal=_master.Soma_Principal,
                Soma_Total=_master.Soma_Total
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
            }
            model.Lista_Origem_Selected = _listaP;

            //Carrega Destino
            List<Parcelamento_Web_Destino> _listaD = parcelamentoRepository.Lista_Parcelamento_Web_Destino(model.Guid);
            List<Parcelamento_Web_Simulado> _listaS = new List<Parcelamento_Web_Simulado>();
            foreach (Parcelamento_Web_Destino item in _listaD) {
                Parcelamento_Web_Simulado _s = new Parcelamento_Web_Simulado() {
                    Data_Vencimento=item.Data_Vencimento,
                    Guid=item.Guid,
                    Juros_Apl=item.Juros_Apl,
                    Juros_Mes=item.Juros_Mes,
                    Juros_Perc=item.Juros_Perc,
                    Numero_Parcela=item.Numero_Parcela ,
                    Qtde_Parcela=_listaD.Count,
                    Saldo=item.Saldo,
                    Valor_Correcao=item.Valor_Correcao,
                    Valor_Honorario=item.Valor_Honorario,
                    Valor_Juros=item.Valor_Juros,
                    Valor_Liquido=item.Valor_Liquido,
                    Valor_Multa=item.Valor_Multa,
                    Valor_Principal=item.Valor_Principal,
                    Valor_Total=item.Valor_Total
                };
                _listaS.Add(_s);
            }

            model.Lista_Simulado = _listaS;

            return View(model);
        }

        [Route("Parc_reqf")]
        [HttpPost]
        public ActionResult Parc_reqf(ParcelamentoViewModel model, string action) {
            if (Session["hashid"] == null) {
                return RedirectToAction("Login", "Home");
            }
            if(action=="btnVoltar")
                return RedirectToAction("Parc_query");
            else {
                if (action == "btnValida") {
                    return RedirectToAction("Parc_bk", new { p = model.Guid });
                }
            }
            return View(model);
        }

        [Route("Parc_query")]
        [HttpGet]
        public ActionResult Parc_query() {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _userId = Convert.ToInt32(Session["hashid"]);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

            List<Parc_Processos> _listaProc = parcelamentoRepository.Lista_Parcelamento_Processos(_userId);
            ParcelamentoViewModel model = new ParcelamentoViewModel();
            model.Lista_Processo = _listaProc;

            return View(model);
        }

        [Route("Parc_bk")]
        [HttpGet]
        public ActionResult Parc_bk(string p) {
            if (Session["hashid"] == null)
                return RedirectToAction("Login", "Home");
            int _userId = Convert.ToInt32(Session["hashid"]);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            int _codigo = _master.Contribuinte_Codigo;
            string _nome = _master.Contribuinte_nome;
            string _cpfcnpj = _master.Contribuinte_cpfcnpj;
            string _endereco = _master.Contribuinte_endereco + " " + _master.Contribuinte_bairro;
            string _cidade = _master.Contribuinte_cidade;
            string _uf = _master.Contribuinte_uf;
            string _cep =  _master.Contribuinte_cep.ToString();
            string _proc = _master.Processo_Numero.ToString() + "-" + Functions.RetornaDvProcesso(_master.Processo_Numero) + "/" + _master.Processo_Ano.ToString();

            //Dados da 1º parcela do parcelamento
            List<Destinoreparc> _listaD = parcelamentoRepository.Lista_Destino_Parcelamento(_master.Processo_Ano, _master.Processo_Numero);
            short _ano = _listaD[0].Anoexercicio;
            short _lanc = 20;
            short _seq = _listaD[0].Numsequencia;
            byte _parcela = 1;
            byte _compl = 0;
            DateTime _dataVencto = (DateTime)_master.Data_Vencimento;

            List<Debitotributo> _listaT = parcelamentoRepository.Lista_Debito_Tributo(_codigo, _ano, _lanc, _seq, _parcela, _compl);
            decimal? _soma = _listaT.Sum(m => m.Valortributo);

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Tributario_bll tributarioRepositoryTmp = new Tributario_bll("GTIconnection");
            //Criar o documento para ela
            Numdocumento regDoc = new Numdocumento {
                Valorguia = _soma,
                Emissor = "Parc/Web",
                Datadocumento = DateTime.Now,
                Registrado = true,
                Percisencao = 0
            };
            regDoc.Percisencao = 0;
            int _novo_documento = tributarioRepositoryTmp.Insert_Documento(regDoc);

            Parceladocumento pd = new Parceladocumento() {
                Codreduzido = _codigo,
                Anoexercicio=_ano,
                Codlancamento=_lanc,
                Seqlancamento=_seq,
                Numparcela=_parcela,
                Codcomplemento=_compl,
                Numdocumento=_novo_documento
            };
            Exception ex = tributarioRepository.Insert_Parcela_Documento(pd);

            string _refTran = "287353200" + _novo_documento.ToString();

            DebitoListViewModel model = new DebitoListViewModel() {
                Nome = _nome,
                Inscricao = _codigo,
                CpfCnpjLabel = _cpfcnpj,
                Endereco = _endereco,
                Cidade = _cidade,
                UF = _uf,
                Cep = _cep,
                RefTran = _refTran,
                Valor_Boleto = Functions.RetornaNumero(_soma.ToString()),
                Data_Vencimento_String = Convert.ToDateTime(_dataVencto.ToString()).ToString("ddMMyyyy"),
                Data_Vencimento = _dataVencto,
                Numero_Processo = _proc
            };
            
            return View(model);
        }

        public ActionResult Termo_anuencia_print(string p) {
            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Termo_Anuencia.rpt"));

            try {
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Termo_Anuencia.pdf");
            } catch (Exception ex) {
                throw ex;
            }
        }

        public ActionResult Termo_confissao_divida(string p) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);

            string _nome = _master.Contribuinte_nome;
            string _cod = _master.Contribuinte_Codigo.ToString("000000");
            string _doc = Functions.FormatarCpfCnpj(_master.Requerente_CpfCnpj);
            string _qtdeParc = _master.Qtde_Parcela.ToString("00");
            string _valor = _master.Sim_Total.ToString("#0.00");
            string _proc = _master.Processo_Numero.ToString() + "-" + Functions.RetornaDvProcesso(_master.Processo_Numero) + "/" + _master.Processo_Ano.ToString();
            string _data = _master.Data_Geracao.ToString("dd/MM/yyyy");
            string _end = "";
            if (_master.Contribuinte_Codigo < 50000) {
                _end = _master.Contribuinte_endereco + " - " + _master.Contribuinte_bairro;
            }

            List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
            string _ano = "";
            foreach (short item in _listaAnos) {
                _ano += item.ToString() + ", ";
            }
            _ano = _ano.Substring(0, _ano.Length - 2);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Termo_ConfDivida.rpt"));
            
            try {
                rd.SetParameterValue("NOME", _nome);
                rd.SetParameterValue("PROCESSO", _proc);
                rd.SetParameterValue("DATACONF", _data);
                rd.SetParameterValue("CODIGO", _cod);
                rd.SetParameterValue("DOC", _doc);
                rd.SetParameterValue("QTDEPARC", _qtdeParc);
                rd.SetParameterValue("VALOR", _valor);
                rd.SetParameterValue("ENDERECO", _end);
                rd.SetParameterValue("EXERCICIO", _ano);

                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Termo_ConfDivida.pdf");
            } catch (Exception ex) {
                throw ex;
            }
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