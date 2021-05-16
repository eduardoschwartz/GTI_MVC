using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_Bll.Classes;
using GTI_Models.Models;
using GTI_Mvc;
using GTI_Mvc.ViewModels;
using GTI_Mvc.Views.Parcelamento.EditorTemplates;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GTI_MVC.Controllers {
    public class ParcelamentoController:Controller {
        private readonly string _connection = "GTIconnectionTeste";
        [Route("Parc_index")]
        [HttpGet]
        public ActionResult Parc_index() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            return View();
        }

        [Route("Parc_req")]
        [HttpGet]
        public ActionResult Parc_req() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

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
            if(_cpfcnpj.Length == 11) {
                _codigo = cidadaoRepository.Existe_Cidadao_Cpf(_cpfcnpj);
                _bCpf = true;
            } else {
                _codigo = cidadaoRepository.Existe_Cidadao_Cnpj(_cpfcnpj);
            }

            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            Parc_Requerente _req = new Parc_Requerente {
                Codigo = _cidadao.Codigo,
                Nome = _cidadao.Nome
            };
            _req.Cpf_Cnpj = Functions.FormatarCpfCnpj(_cpfcnpj);
            string _tipoEnd = _cidadao.EnderecoC == "S" ? "C" : "R";
            _req.TipoEnd = _tipoEnd == "R" ? "RESIDENCIAL" : "COMERCIAL";
            if(_tipoEnd == "R") {
                _req.Bairro_Nome = _cidadao.NomeBairroR;
                _req.Bairro_Codigo = (int)_cidadao.CodigoBairroR;
                _req.Cidade_Nome = _cidadao.NomeCidadeR;
                _req.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                _req.UF = _cidadao.UfR;
                _req.Logradouro_Codigo = _cidadao.CodigoLogradouroR == null ? 0 : (int)_cidadao.CodigoLogradouroR;
                _req.Logradouro_Nome = _cidadao.EnderecoR;
                _req.Numero = (int)_cidadao.NumeroR;
                _req.Complemento = _cidadao.ComplementoR;
                _req.Telefone = _cidadao.TelefoneR;
                _req.Cep = _cidadao.CepR.ToString();
                _req.Email = _cidadao.EmailR;
            } else {
                _req.Bairro_Nome = _cidadao.NomeBairroC;
                _req.Bairro_Codigo = (int)_cidadao.CodigoBairroC;
                _req.Cidade_Nome = _cidadao.NomeCidadeC;
                _req.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                _req.UF = _cidadao.UfC;
                _req.Logradouro_Codigo = _cidadao.CodigoLogradouroC == null ? 0 : (int)_cidadao.CodigoLogradouroC;
                _req.Logradouro_Nome = _cidadao.EnderecoC;
                _req.Numero = (int)_cidadao.NumeroC;
                _req.Complemento = _cidadao.ComplementoC;
                _req.Telefone = _cidadao.TelefoneC;
                _req.Cep = _cidadao.CepC.ToString();
                _req.Email = _cidadao.EmailC;
            }
            if(_req.Logradouro_Codigo > 0) {
                int nCep = enderecoRepository.RetornaCep(Convert.ToInt32(_req.Logradouro_Codigo),(short)_req.Numero);
                _req.Cep = nCep.ToString("00000-000");
            }

            if(_req.Bairro_Nome == null)
                _req.Bairro_Nome = enderecoRepository.Retorna_Bairro(_req.UF,_req.Cidade_Codigo,_req.Bairro_Codigo);

            ParcelamentoViewModel model = new ParcelamentoViewModel {
                Requerente = _req
            };
            List<Parc_Codigos> _listaCodigos = new List<Parc_Codigos>();

            //Lista de imóvel
            List<int> _listaImovel;
            if(_bCpf)
                _listaImovel = imovelRepository.Lista_Imovel_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaImovel = imovelRepository.Lista_Imovel_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            foreach(int cod in _listaImovel) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = "Imóvel localizada na " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if(!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if(!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if(!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                Parc_Codigos item = new Parc_Codigos() {
                    Codigo = _header.Codigo,
                    Tipo = "Imóvel",
                    Cpf_Cnpj = Functions.FormatarCpfCnpj(_header.Cpf_cnpj),
                    Descricao = _desc
                };
                _listaCodigos.Add(item);
            }

            //Lista de empresas
            List<int> _listaEmpresa;
            if(_bCpf)
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cpf(Functions.RetornaNumero(_req.Cpf_Cnpj));
            else
                _listaEmpresa = empresaRepository.Lista_Empresa_Proprietario_Cnpj(Functions.RetornaNumero(_req.Cpf_Cnpj));

            foreach(int cod in _listaEmpresa) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod);
                string _desc = _header.Nome + ", localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if(!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if(!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if(!string.IsNullOrEmpty(_header.Lote_original))
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
            List<Cidadao> _listaCidadao;
            if(_bCpf)
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null,Functions.RetornaNumero(_req.Cpf_Cnpj),null);
            else
                _listaCidadao = cidadaoRepository.Lista_Cidadao(null,null,Functions.RetornaNumero(_req.Cpf_Cnpj));

            foreach(Cidadao cod in _listaCidadao) {
                Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(cod.Codcidadao);
                string _desc = "Inscrição localizada na(o): " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                if(!string.IsNullOrEmpty(_header.Complemento))
                    _desc += " " + _header.Complemento;
                _desc += ", " + _header.Nome_bairro;
                if(!string.IsNullOrEmpty(_header.Quadra_original))
                    _desc += " Quadra:" + _header.Quadra_original;
                if(!string.IsNullOrEmpty(_header.Lote_original))
                    _desc += ", Lote:" + _header.Lote_original;

                Parc_Codigos item = new Parc_Codigos() {
                    Codigo = _header.Codigo,
                    Tipo = "Outros",
                    Cpf_Cnpj = Functions.FormatarCpfCnpj(_header.Cpf_cnpj),
                    Descricao = _desc
                };
                _listaCodigos.Add(item);
            }

            //Lista de sócios de empresa de fora
            int y = 0;
            foreach(Cidadao cod in _listaCidadao) {
                List<CidadaoHeader> _listaSocio = cidadaoRepository.Lista_Cidadao_Socio(cod.Codcidadao);
                foreach(CidadaoHeader head in _listaSocio) {
                    string _cnpj = head.Cnpj;
                    List<int> _lista_imovel_socio = imovelRepository.Lista_Imovel_Socio(head.Codigo);
                    foreach(int imovel in _lista_imovel_socio) {
                        if(_lista_imovel_socio.Count > 0) {
                            Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(imovel);
                            string _desc = "Imóvel localizado na: " + _header.Endereco_abreviado + ", " + _header.Numero.ToString();
                            if(!string.IsNullOrEmpty(_header.Complemento))
                                _desc += " " + _header.Complemento;
                            _desc += ", " + _header.Nome_bairro;
                            if(!string.IsNullOrEmpty(_header.Quadra_original))
                                _desc += " Quadra:" + _header.Quadra_original;
                            if(!string.IsNullOrEmpty(_header.Lote_original))
                                _desc += ", Lote:" + _header.Lote_original;

                            Parc_Codigos item = new Parc_Codigos() {
                                Codigo = _header.Codigo,
                                Tipo = "Imóvel",
                                Cpf_Cnpj = Functions.FormatarCpfCnpj(_header.Cpf_cnpj),
                                Descricao = _desc
                            };
                            _listaCodigos.Add(item);
                        }
                    }
                }
            }


            model.Lista_Codigos = _listaCodigos;

            //Antes de retornar gravamos os dados
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            if(model.Guid == null) {
                //Grava Master
                model.Guid = Guid.NewGuid().ToString("N");
                Parcelamento_web_master reg = new Parcelamento_web_master() {
                    Guid = model.Guid,
                    User_id = _user_id,
                    Data_Vencimento = DateTime.Now,
                    Requerente_Codigo = _req.Codigo,
                    Requerente_Bairro = _req.Bairro_Nome ?? "",
                    Requerente_Cep = Convert.ToInt32(Functions.RetornaNumero(_req.Cep)),
                    Requerente_Cidade = _req.Cidade_Nome ?? "",
                    Requerente_Complemento = _req.Complemento ?? "",
                    Requerente_CpfCnpj = _req.Cpf_Cnpj,
                    Requerente_Logradouro = _req.Logradouro_Nome ?? "",
                    Requerente_Nome = _req.Nome,
                    Requerente_Numero = _req.Numero,
                    Requerente_Telefone = _req.Telefone ?? "",
                    Requerente_Uf = _req.UF ?? "",
                    Requerente_Email = _req.Email ?? ""
                };
                Exception ex = parcelamentoRepository.Incluir_Parcelamento_Web_Master(reg);
                if(ex != null)
                    throw ex;
                List<Parcelamento_web_lista_codigo> _listaCodigo = new List<Parcelamento_web_lista_codigo>();
                foreach(Parc_Codigos item in _listaCodigos.OrderBy(m => m.Codigo)) {
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
                if(ex != null)
                    throw ex;

                _listaCodigos.Clear();
                foreach(Parcelamento_web_lista_codigo item in _listaCodigo) {
                    Parc_Codigos item2 = new Parc_Codigos() {
                        Codigo = item.Codigo,
                        Tipo = item.Tipo,
                        Cpf_Cnpj = item.Documento,
                        Descricao = item.Descricao
                    };
                    _listaCodigos.Add(item2);
                }
                model.Lista_Codigos = _listaCodigos;


            }

            return View(model);
        }

        [Route("Parc_req")]
        [HttpPost]
        public ActionResult Parc_req(ParcelamentoViewModel model,string listacod,string action) {
            if(action == "btnAtualiza") {
                return RedirectToAction("Parc_cid",new { p = model.Guid });
            }

            int _codigo = 0;
            for(int i = 0;i < model.Lista_Codigos.Count;i++) {
                if(model.Lista_Codigos[i].Selected) {
                    _codigo = Convert.ToInt32(model.Lista_Codigos[i].Codigo);
                    break;
                }
            }
            if(_codigo == 0) {
                ViewBag.Result = "Nenhuma inscrição foi selecionada.";
                return View(model);
            }

            if(string.IsNullOrEmpty(model.Requerente.Logradouro_Nome) || string.IsNullOrEmpty(model.Requerente.Bairro_Nome)) {
                ViewBag.Result = "Dados Cadastrais incompletos.";
                return View(model);
            }

            List<Parc_Codigos> _listaCodigos = new List<Parc_Codigos>();
            List<Parcelamento_web_lista_codigo> _Lista_Codigos = new List<Parcelamento_web_lista_codigo>();
            Sistema_bll sistemaRepository = new Sistema_bll(_connection);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Contribuinte_Header_Struct _header = sistemaRepository.Contribuinte_Header(_codigo);
            if(_header.Cpf_cnpj == null) {
                _Lista_Codigos = parcelamentoRepository.Lista_Parcelamento_Lista_Codigo(model.Guid);
                foreach(Parcelamento_web_lista_codigo item in _Lista_Codigos) {
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

            List<SpParcelamentoOrigem> Lista_Origem = parcelamentoRepository.Lista_Parcelamento_Origem(_codigo,_tipo);

            _Lista_Codigos = parcelamentoRepository.Lista_Parcelamento_Lista_Codigo(model.Guid);
            foreach(Parcelamento_web_lista_codigo item in _Lista_Codigos) {
                Parc_Codigos item2 = new Parc_Codigos() {
                    Codigo = item.Codigo,
                    Tipo = item.Tipo,
                    Cpf_Cnpj = item.Documento,
                    Descricao = item.Descricao
                };
                _listaCodigos.Add(item2);
            }
            model.Lista_Codigos = _listaCodigos;

            if(Lista_Origem.Count == 0) {
                ViewBag.Result = "Não existem débitos a serem parcelados para esta inscrição.";
                return View(model);
            } else {
                Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Origem(model.Guid);
                List<Parcelamento_web_origem> _listaWebOrigem = new List<Parcelamento_web_origem>();
                foreach(SpParcelamentoOrigem item in Lista_Origem) {
                    Parcelamento_web_origem reg = new Parcelamento_web_origem() {
                        Guid = model.Guid,
                        Idx = item.Idx,
                        Ano = item.Exercicio,
                        Lancamento = item.Lancamento,
                        Sequencia = item.Sequencia,
                        Parcela = item.Parcela,
                        Complemento = item.Complemento,
                        Data_Vencimento = item.Data_vencimento,
                        Lancamento_Nome = item.Nome_lancamento,
                        Ajuizado = item.Ajuizado,
                        Valor_Tributo = item.Valor_principal,
                        Valor_Juros = item.Valor_juros,
                        Valor_Multa = item.Valor_multa,
                        Valor_Correcao = item.Valor_correcao,
                        Valor_Total = Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero),
                        Valor_Penalidade = item.Valor_penalidade,
                        Perc_Penalidade = item.Perc_penalidade,
                        Qtde_Parcelamento = item.Qtde_parcelamento,
                        Execucao_Fiscal = item.Execucao_Fiscal,
                        Protesto = item.Protesto
                    };
                    _listaWebOrigem.Add(reg);
                }
                ex = parcelamentoRepository.Incluir_Parcelamento_Web_Origem(_listaWebOrigem);
                if(ex != null)
                    throw ex;

            }

            _header = sistemaRepository.Contribuinte_Header(_codigo);
            bool _ativo = _header.Ativo;
            string _tipoDoc = "F";
            if(_header.Cpf_cnpj.Length > 11) {
                _tipoDoc = "J";
                if(_codigo > 100000 && _codigo < 300000) {
                    if(!_ativo)
                        _tipoDoc = "F"; //Empresas inativas são tratadas como Físicas
                    Empresa_bll empresaRepository = new Empresa_bll(_connection);
                    if(empresaRepository.EmpresaSuspensa(_codigo))
                        _tipoDoc = "F"; //Empresas suspensas são tratadas como Físicas

                }
            }

            string _end = _header.Endereco + ", " + _header.Numero.ToString();
            if(!string.IsNullOrEmpty(_header.Complemento))
                _end += " " + _header.Complemento;
            if(!string.IsNullOrEmpty(_header.Quadra_original))
                _end += " Quadra:" + _header.Quadra_original;
            if(!string.IsNullOrEmpty(_header.Lote_original))
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
                Contribuinte_tipo = _tipoDoc,

            };

            short _plano_codigo = 4; //(4=sem plano)
            Plano _plano = parcelamentoRepository.Retorna_Plano_Desconto(_plano_codigo);
            regP.Qtde_Maxima_Parcela = _plano.Qtde_Parcela;
            regP.Perc_Desconto = _plano.Desconto;
            regP.Plano_Codigo = _plano_codigo;
            regP.Plano_Nome = _plano.Nome;

            decimal _valor_minimo = parcelamentoRepository.Retorna_Parcelamento_Valor_Minimo((short)DateTime.Now.Year,false,_tipoDoc);
            regP.Valor_minimo = _valor_minimo;
            Exception ex2 = parcelamentoRepository.Atualizar_Codigo_Master(regP);
            if(ex2 != null)
                throw ex2;

            return RedirectToAction("Parc_reqb",new { p = model.Guid });

        }

        [Route("Parc_reqb")]
        [HttpGet]
        public ActionResult Parc_reqb(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

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
                Cep = _master.Contribuinte_cep.ToString("00000-000"),
                Tipo = _master.Contribuinte_tipo
            };
            return View(model);
        }

        [Route("Parc_reqb")]
        [HttpPost]
        public ActionResult Parc_reqb(ParcelamentoViewModel model) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

            return RedirectToAction("Parc_reqc",new { p = model.Guid });
        }

        [Route("Parc_reqc")]
        [HttpGet]
        public ActionResult Parc_reqc(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

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
            decimal _SomaP = 0, _SomaM = 0, _SomaJ = 0, _SomaC = 0, _SomaT = 0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Origem(p);
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = Functions.TruncateTo(item.Nome_lancamento,16),
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
                    Valor_total = item.Valor_total,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaT += Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero);
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
            foreach(SelectDebitoParcelamentoEditorViewModel item in model.Lista_Origem) {
                if(item.Selected) {
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
                        Valor_total = item.Valor_total,
                        Execucao_Fiscal = item.Execucao_Fiscal,
                        Protesto = item.Protesto
                    };
                    _totalSel += item.Valor_total;
                    _listaOrigem.Add(_r);
                    t++;
                }
            }
            model.Lista_Origem_Selected = _listaOrigem;
            if(_totalSel / 2 < model.Valor_Minimo) {
                ViewBag.Result = "Valor mínimo da parcela deve ser de R$" + model.Valor_Minimo.ToString("#0.00");
                return View(model);
            }

            t = 1;
            Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Selected(model.Guid);
            bool _ajuizado = false;
            decimal _somaP = 0, _somaJ = 0, _somaM = 0, _somaC = 0, _somaT = 0, _somaE = 0, _somaH = 0;
            List<Parcelamento_web_selected> _listaSelect = new List<Parcelamento_web_selected>();
            foreach(SelectDebitoParcelamentoEditorViewModel item in _listaOrigem) {
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
                    Sequencia = item.Sequencia,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                if(item.Ajuizado == "S")
                    reg.Valor_Honorario += item.Valor_total * (decimal)0.1;

                _somaP += item.Valor_principal;
                _somaJ += item.Valor_juros;
                _somaM += item.Valor_multa;
                _somaC += item.Valor_correcao;
                _somaT += item.Valor_total;
                _somaE += item.Valor_penalidade;
                if(item.Ajuizado == "S")
                    _somaH += item.Valor_total;

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
            decimal _total = 0;
            foreach(SpParcelamentoOrigem item in _listaSelected) {
                List<SpExtrato> _listaExtrato = tributarioRepository.Lista_Extrato_Tributo(model.Contribuinte.Codigo,item.Exercicio,item.Exercicio,item.Lancamento,item.Lancamento,item.Sequencia,item.Sequencia,item.Parcela,item.Parcela,item.Complemento,item.Complemento);
                foreach(SpExtrato _ext in _listaExtrato) {
                    bool _find = false;
                    int _pos = 0;
                    foreach(Parcelamento_Web_Tributo _trib in _listaTributo) {
                        if(_trib.Tributo == _ext.Codtributo) {
                            _find = true;
                            break;
                        }
                        _pos++;
                    }
                    _total += _ext.Valortributo;
                    if(!_find) {
                        Parcelamento_Web_Tributo regT = new Parcelamento_Web_Tributo() {
                            Guid = model.Guid,
                            Tributo = _ext.Codtributo,
                            Valor = _ext.Valortributo,
                            Perc = 0
                        };
                        _listaTributo.Add(regT);
                    } else {
                        _listaTributo[_pos].Valor += _ext.Valortributo;
                    }
                }
            }

            for(int i = 0;i < _listaTributo.Count;i++) {
                _listaTributo[i].Perc = _listaTributo[i].Valor * 100 / _total;
            }

            //Grava Honorario(90), Juros(113), Multa(112), Correcao(26) e Juros Apl(585)
            Parcelamento_Web_Tributo r = null;
            if(_somaJ > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 113,
                    Valor = _somaJ,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if(_somaH > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 90,
                    Valor = _somaH * 10 / 100,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if(_somaM > 0) {
                r = new Parcelamento_Web_Tributo() {
                    Guid = model.Guid,
                    Tributo = 112,
                    Valor = _somaM,
                    Perc = 100
                };
                _listaTributo.Add(r);
            }

            if(_somaC > 0) {
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

            return RedirectToAction("Parc_reqd",new { p = model.Guid });

        }

        [Route("Parc_reqd")]
        [HttpGet]
        public ActionResult Parc_reqd(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

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
            decimal _SomaP = 0, _SomaM = 0, _SomaJ = 0, _SomaC = 0, _SomaT = 0, _SomaE = 0, _SomaH = 0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            bool _bAjuizado = ListaOrigem[0].Ajuizado == "S";
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = Functions.TruncateTo(item.Nome_lancamento,16),
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
                    Valor_total = item.Valor_total,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaE += item.Valor_penalidade;
                _SomaT += Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero);
                if(item.Ajuizado == "S")
                    _SomaH += (Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero)) * ((decimal)0.1);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Penalidade = _SomaE;
            model.Soma_Total = _SomaT;
            model.Lista_Origem_Selected = _listaP;

            //Grava parcelamento_web_selected_name
            Exception ex = parcelamentoRepository.Excluir_parcelamento_Web_Selected_Name(p);
            List<Parcelamento_Web_Selected_Name> _listaName = new List<Parcelamento_Web_Selected_Name>();
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                bool _find = false;
                foreach(Parcelamento_Web_Selected_Name s in _listaName) {
                    if(s.Ano == item.Exercicio && s.Lancamento == item.Lancamento) {
                        _find = true;
                        break;
                    }
                }
                if(!_find) {
                    Parcelamento_Web_Selected_Name reg = new Parcelamento_Web_Selected_Name() {
                        Guid = p,
                        Ano = item.Exercicio,
                        Lancamento = item.Lancamento
                    };
                    _listaName.Add(reg);
                }
            }
            ex = parcelamentoRepository.Incluir_Parcelamento_Web_Selected_Name(_listaName);

            //########### Carrega Simulado ###################################


            ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado(model.Guid);
            ex = parcelamentoRepository.Excluir_parcelamento_Web_Simulado_Resumo(model.Guid);
            List<Parcelamento_Web_Simulado> _listaSimulado = parcelamentoRepository.Lista_Parcelamento_Destino(model.Guid,(short)model.Plano_Codigo,DateTime.Now,_bAjuizado,_bAjuizado,_SomaP,_SomaJ,_SomaM,_SomaC,_SomaT,_SomaE,model.Valor_Minimo,_SomaH);

            bool _issCCivilAVencer = false;
            foreach(SpParcelamentoOrigem _d in ListaOrigem) {
                if(_d.Lancamento == 65) {
                    if(_d.Data_vencimento > DateTime.Now) {
                        _issCCivilAVencer = true;
                        break;
                    }
                }
            }

            int _qtdeMaxParcela = 0;
            if(_issCCivilAVencer) _qtdeMaxParcela = 12;


            IEnumerable<int> _listaQtde = _listaSimulado.Select(o => o.Qtde_Parcela).Distinct();

            decimal _valor1 = 0, _valorN = 0;
            List<Parc_Resumo> Lista_resumo = new List<Parc_Resumo>();
            List<Parcelamento_Web_Simulado_Resumo> _lista_Web_Simulado_Resumo = new List<Parcelamento_Web_Simulado_Resumo>();
            foreach(int linha in _listaQtde) {
                if(_qtdeMaxParcela > 0) {
                    if(linha > _qtdeMaxParcela) {
                        goto Fim;
                    }
                }

                foreach(Parcelamento_Web_Simulado item in _listaSimulado) {
                    if(item.Qtde_Parcela == linha && item.Numero_Parcela == 1) {
                        _valor1 = item.Valor_Total;
                    } else {
                        if(item.Qtde_Parcela == linha && item.Numero_Parcela == 2) {
                            if(item.Valor_Total < model.Valor_Minimo)
                                goto Fim;
                            _valorN = item.Valor_Total;
                            break;
                        }
                    }
                }
                Parcelamento_Web_Simulado_Resumo t = new Parcelamento_Web_Simulado_Resumo() {
                    Guid = model.Guid,
                    Qtde_Parcela = linha,
                    Valor_Entrada = _valor1,
                    Valor_N = _valorN,
                    Valor_Total = _valor1 + (_valorN * (linha - 1))
                };
                _lista_Web_Simulado_Resumo.Add(t);
                Parc_Resumo r = new Parc_Resumo() {
                    Qtde_Parcela = linha,
                    Valor_Entrada = "R$" + _valor1.ToString("#0.00"),
                    Valor_N = "R$" + _valorN.ToString("#0.00"),
                    Valor_Total = "R$" + t.Valor_Total.ToString("#0.00"),
                };
                if(linha == 2)
                    r.Selected = true;
                Lista_resumo.Add(r);
            }
Fim:;
            ex = parcelamentoRepository.Incluir_Parcelamento_Web_Simulado_Resumo(_lista_Web_Simulado_Resumo);
            model.Lista_Resumo = Lista_resumo;

            //################################################################

            return View(model);
        }

        [Route("Parc_reqd")]
        [HttpPost]
        public ActionResult Parc_reqd(ParcelamentoViewModel model,string action) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            if(action == "btPrint") {
                List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(model.Guid);
                string _anos = "";
                IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
                foreach(short item in _listaAnos) {
                    _anos += item.ToString() + ", ";
                }
                _anos = _anos.Substring(0,_anos.Length - 2);

                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Simulado_Parcelamento.rpt"));

                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;
                string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
                string IPAddress = builder.DataSource;
                string _userId = builder.UserID;
                string _pwd = builder.Password;

                crConnectionInfo.ServerName = IPAddress;
                crConnectionInfo.DatabaseName = "TributacaoTeste";
                crConnectionInfo.UserID = _userId;
                crConnectionInfo.Password = _pwd;
                CrTables = rd.Database.Tables;
                foreach(Table CrTable in CrTables) {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                try {
                    rd.RecordSelectionFormula = "{Parcelamento_Web_Master.Guid}='" + model.Guid + "'";
                    rd.SetParameterValue("EXERCICIO",_anos);
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream,"application/pdf","Simulado_Parcelamento.pdf");
                } catch {
                    throw;
                }
            } else {
                foreach(Parc_Resumo item in model.Lista_Resumo) {
                    if(item.Selected) {
                        Exception ex = parcelamentoRepository.Atualizar_QtdeParcela_Master(model.Guid,item.Qtde_Parcela);
                        break;
                    }
                }
                //return RedirectToAction("Parc_reqe", new { p = model.Guid });
                Parc_reqe_old(model.Guid); //Executa o antigo get do parc_reqE
                return RedirectToAction("Parc_tan",new { p = model.Guid });
            }

        }

        [Route("Parc_reqe")]
        [HttpGet]
        public ActionResult Parc_reqe(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            if(_master.Processo_Numero > 0)
                return RedirectToAction("Parc_index");

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
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = Functions.TruncateTo(item.Nome_lancamento,16),
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
                    Valor_total = item.Valor_total,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaE += item.Valor_penalidade;
                _SomaT += Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Penalidade = _SomaE;
            model.Soma_Total = _SomaT;
            model.Lista_Origem_Selected = _listaP;

            //Carrega Simulado
            model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid,_master.Qtde_Parcela);

            //Atualiza Totais
            decimal _SomaH = 0, _SomaJapl = 0, _SomaL = 0;
            _SomaP = 0; _SomaC = 0; _SomaJ = 0; _SomaM = 0; _SomaT = 0;

            foreach(Parcelamento_Web_Simulado item in model.Lista_Simulado) {
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
        public ActionResult Parc_reqe(ParcelamentoViewModel model,string action,string value) {
            if(Session["hashid"] == null) {
                return RedirectToAction("Login","Home");
            }

            if(action == "btnResumo") {
                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Resumo_Parcelamento_previa.rpt"));
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;
                string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
                string IPAddress = builder.DataSource;
                string _userId = builder.UserID;
                string _pwd = builder.Password;

                crConnectionInfo.ServerName = IPAddress;
                crConnectionInfo.DatabaseName = "TributacaoTeste";
                crConnectionInfo.UserID = _userId;
                crConnectionInfo.Password = _pwd;
                CrTables = rd.Database.Tables;
                foreach(Table CrTable in CrTables) {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                try {
                    rd.RecordSelectionFormula = "{Parcelamento_Web_Master.Guid}='" + model.Guid + "'";
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream,"application/pdf","Resumo_Parcelamento.pdf");
                } catch {
                    throw;
                }
            }
            return RedirectToAction("Parc_tan",new { p = model.Guid });
        }

        [Route("Parc_tan")]
        [HttpGet]
        public ActionResult Parc_tan(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            if(_master.Processo_Numero > 0)
                return RedirectToAction("Parc_index");


            //Carrega Origem
            List<SpParcelamentoOrigem> _listaO = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            string _ajuizado = _listaO[0].Ajuizado;
            if(_ajuizado == "N") {
                foreach(SpParcelamentoOrigem item in _listaO) {
                    if(item.Protesto == "S") {
                        _ajuizado = "S";
                        break;
                    }
                }
            }
            ViewBag.ajuizado = _ajuizado;
            ParcelamentoViewModel model = new ParcelamentoViewModel();
            model.Guid = p;
            return View(model);
        }

        [Route("Parc_tan")]
        [HttpPost]
        public ActionResult Parc_tan(ParcelamentoViewModel model,string action) {

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            string _guid = model.Guid;

            if(action == "Parc_tan") {
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
                string _obs = "";

                List<Parcelamento_Web_Selected_Name_Struct> _listaName = parcelamentoRepository.Lista_Parcelamento_Web_Selected_Name(_guid);
                IEnumerable<string> _listaLanc = _listaName.Select(o => o.Lancamento_Nome).Distinct();
                foreach(string _lanc in _listaLanc) {
                    string _Exercicio = "";
                    foreach(Parcelamento_Web_Selected_Name_Struct item in _listaName) {
                        if(item.Lancamento_Nome == _lanc) {
                            _Exercicio += item.Ano.ToString() + ", ";
                        }
                    }
                    _Exercicio = _Exercicio.Substring(0,_Exercicio.Length - 2);
                    _obs += _lanc + ": " + _Exercicio + " ";
                }

                _obs += "parcelado em: " + _qtdeParc.ToString() + " vezes.";

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
                ex = parcelamentoRepository.Atualizar_Processo_Master(_guid,_ano,_numero);


                //Grava tabela web_destino com as parcelas do simulado
                model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid,_master.Qtde_Parcela);
                ex = parcelamentoRepository.Excluir_parcelamento_Web_Destino(model.Guid);
                List<Parcelamento_Web_Destino> _lista_Parcelamento_Web_Destino = new List<Parcelamento_Web_Destino>();
                foreach(Parcelamento_Web_Simulado _s in model.Lista_Simulado.Where(m => m.Qtde_Parcela == _qtdeParc)) {
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
                foreach(SpParcelamentoOrigem item in _listaSelected) {
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
                foreach(Parcelamento_Web_Destino item in _listaDestino) {
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
                    if(item.Data_Vencimento.Year == DateTime.Now.Year)
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
                    foreach(Parcelamento_Web_Tributo trib in _ListaTributo) {
                        Debitotributo _dt = new Debitotributo() {
                            Codreduzido = dp.Codreduzido,
                            Anoexercicio = dp.Anoexercicio,
                            Codlancamento = dp.Codlancamento,
                            Seqlancamento = dp.Seqlancamento,
                            Numparcela = dp.Numparcela,
                            Codcomplemento = dp.Codcomplemento,
                            Codtributo = (short)trib.Tributo

                        };
                        if(_dt.Numparcela == 1)
                            _dt.Valortributo = trib.Valor * _Perc1 / 100;
                        else
                            _dt.Valortributo = trib.Valor * _PercN / 100;

                        if(_dt.Codtributo == 585)
                            _dt.Valortributo = trib.Valor;
                        _listaDebitoTributo.Add(_dt);
                    }
                }

                ex = parcelamentoRepository.Incluir_DestinoReparc(_listaDestinoReparc);
                ex = parcelamentoRepository.Incluir_Debito_Parcela(_listaDebitoParcela);
                ex = parcelamentoRepository.Incluir_Debito_Tributo(_listaDebitoTributo);
                ex = parcelamentoRepository.Atualizar_Status_Origem(_codigoC,_listaSelected);


                return RedirectToAction("Parc_tcd",new { p = model.Guid });
            } else {
                if(action == "btPrint") {
                    return Termo_anuencia_print(model.Guid);
                }
            }
            return RedirectToAction("Login","Home");
        }

        [Route("Parc_tcd")]
        [HttpGet]
        public ActionResult Parc_tcd(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            string _guid = p;
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(_guid);

            string _endereco = _master.Contribuinte_endereco + " " + _master.Contribuinte_bairro;

            ParcelamentoViewModel model = new ParcelamentoViewModel();
            Parc_Contribuinte pc = new Parc_Contribuinte() {
                Codigo = _master.Contribuinte_Codigo,
                Nome = _master.Contribuinte_nome,
                Logradouro_Nome = _endereco,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Contribuinte_cpfcnpj)
            };
            Parc_Requerente pr = new Parc_Requerente() {
                Nome = _master.Requerente_Nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Requerente_CpfCnpj)
            };

            List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
            string _ano = "";
            foreach(short item in _listaAnos) {
                _ano += item.ToString() + ", ";
            }
            _ano = _ano.Substring(0,_ano.Length - 2);

            model.Contribuinte = pc;
            model.Requerente = pr;
            model.Qtde_Parcela = _master.Qtde_Parcela;
            model.Soma_Total = _master.Sim_Total;
            model.NumeroProcesso = _master.Processo_Extenso;
            model.Data_Vencimento = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy");
            model.Exercicios = _ano;
            model.Guid = p;
            return View(model);
        }

        [Route("Parc_tcd")]
        [HttpPost]
        public ActionResult Parc_tcd(ParcelamentoViewModel model,string action) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            if(action == "btnPrintTermo") {
                return Termo_confissao_divida(model.Guid);
            }

            if(action == "btnPrintResumo") {
                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Resumo_Parcelamento.rpt"));
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;
                string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
                string IPAddress = builder.DataSource;
                string _userId = builder.UserID;
                string _pwd = builder.Password;

                crConnectionInfo.ServerName = IPAddress;
                crConnectionInfo.DatabaseName = "TributacaoTeste";
                crConnectionInfo.UserID = _userId;
                crConnectionInfo.Password = _pwd;
                CrTables = rd.Database.Tables;
                foreach(Table CrTable in CrTables) {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                try {
                    rd.RecordSelectionFormula = "{Parcelamento_Web_Master.Guid}='" + model.Guid + "'";
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream,"application/pdf","Resumo_Parcelamento.pdf");
                } catch {
                    throw;
                }

            }
            if(action == "btnPrintBoleto") {
                return RedirectToAction("Parc_bk",new { p = model.Guid });
            }
            if(action == "btnFinalizar")
                return RedirectToAction("Parc_query");

            return RedirectToAction("Login","Home");
        }

        [Route("Parc_reqf")]
        [HttpGet]
        public ActionResult Parc_reqf(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            ParcelamentoViewModel model = new ParcelamentoViewModel() {
                Guid = p,
                Plano_Nome = _master.Plano_Nome,
                Data_Vencimento = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy"),
                Plano_Codigo = _master.Plano_Codigo,
                Valor_Minimo = _master.Valor_minimo,
                Perc_desconto = _master.Perc_Desconto,
                Processo_ano = _master.Processo_Ano,
                Processo_numero = _master.Processo_Numero,
                NumeroProcesso = _master.Processo_Extenso,
                Sim_Correcao = _master.Sim_Correcao,
                Sim_Honorario = _master.Sim_Honorario,
                Sim_Juros = _master.Sim_Juros,
                Sim_Juros_apl = _master.Sim_Juros_apl,
                Sim_Liquido = _master.Sim_Liquido,
                Sim_Multa = _master.Sim_Multa,
                Sim_Principal = _master.Sim_Principal,
                Sim_Total = _master.Sim_Total,
                Soma_Correcao = _master.Soma_Correcao,
                Soma_Juros = _master.Soma_Juros,
                Soma_Multa = _master.Soma_Multa,
                Soma_Penalidade = _master.Soma_Entrada,
                Soma_Principal = _master.Soma_Principal,
                Soma_Total = _master.Soma_Total
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
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = Functions.TruncateTo(item.Nome_lancamento,16),
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
                    Valor_total = item.Valor_total,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                _listaP.Add(d);
            }
            model.Lista_Origem_Selected = _listaP;

            //Carrega Destino
            List<Parcelamento_Web_Destino> _listaD = parcelamentoRepository.Lista_Parcelamento_Web_Destino(model.Guid);
            List<Parcelamento_Web_Simulado> _listaS = new List<Parcelamento_Web_Simulado>();
            foreach(Parcelamento_Web_Destino item in _listaD) {
                Parcelamento_Web_Simulado _s = new Parcelamento_Web_Simulado() {
                    Data_Vencimento = item.Data_Vencimento,
                    Guid = item.Guid,
                    Juros_Apl = item.Juros_Apl,
                    Juros_Mes = item.Juros_Mes,
                    Juros_Perc = item.Juros_Perc,
                    Numero_Parcela = item.Numero_Parcela,
                    Qtde_Parcela = _listaD.Count,
                    Saldo = item.Saldo,
                    Valor_Correcao = item.Valor_Correcao,
                    Valor_Honorario = item.Valor_Honorario,
                    Valor_Juros = item.Valor_Juros,
                    Valor_Liquido = item.Valor_Liquido,
                    Valor_Multa = item.Valor_Multa,
                    Valor_Principal = item.Valor_Principal,
                    Valor_Total = item.Valor_Total
                };
                _listaS.Add(_s);
            }

            model.Lista_Simulado = _listaS;

            return View(model);
        }

        [Route("Parc_reqf")]
        [HttpPost]
        public ActionResult Parc_reqf(ParcelamentoViewModel model,string action) {
            if(Session["hashid"] == null) {
                return RedirectToAction("Login","Home");
            }
            if(action == "btnVoltar")
                return RedirectToAction("Parc_query");

            if(action == "btnPrintResumo") {
                ReportDocument rd = new ReportDocument();
                rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Resumo_Parcelamento.rpt"));
                TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
                TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
                ConnectionInfo crConnectionInfo = new ConnectionInfo();
                Tables CrTables;
                string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
                string IPAddress = builder.DataSource;
                string _userId = builder.UserID;
                string _pwd = builder.Password;

                crConnectionInfo.ServerName = IPAddress;
                crConnectionInfo.DatabaseName = "TributacaoTeste";
                crConnectionInfo.UserID = _userId;
                crConnectionInfo.Password = _pwd;
                CrTables = rd.Database.Tables;
                foreach(Table CrTable in CrTables) {
                    crtableLogoninfo = CrTable.LogOnInfo;
                    crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                    CrTable.ApplyLogOnInfo(crtableLogoninfo);
                }

                try {
                    rd.RecordSelectionFormula = "{Parcelamento_Web_Master.Guid}='" + model.Guid + "'";
                    Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                    return File(stream,"application/pdf","Resumo_Parcelamento.pdf");
                } catch {
                    throw;
                }

            }

            return View(model);
        }

        [Route("Parc_query")]
        [HttpGet]
        public ActionResult Parc_query() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");
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
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");
            int _userId = Convert.ToInt32(Session["hashid"]);
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            int _codigo = _master.Contribuinte_Codigo;
            string _nome = _master.Contribuinte_nome;
            string _cpfcnpj = _master.Contribuinte_cpfcnpj;
            string _endereco = _master.Contribuinte_endereco + " " + _master.Contribuinte_bairro;
            string _cidade = _master.Contribuinte_cidade;
            string _uf = _master.Contribuinte_uf;
            string _cep = _master.Contribuinte_cep.ToString();
            string _proc = _master.Processo_Extenso;

            //Dados da 1º parcela do parcelamento
            List<Destinoreparc> _listaD = parcelamentoRepository.Lista_Destino_Parcelamento(_master.Processo_Ano,_master.Processo_Numero);
            short _ano = _listaD[0].Anoexercicio;
            short _lanc = 20;
            short _seq = _listaD[0].Numsequencia;
            byte _parcela = 1;
            byte _compl = 0;
            DateTime _dataVencto = (DateTime)_master.Data_Vencimento;

            List<Debitotributo> _listaT = parcelamentoRepository.Lista_Debito_Tributo(_codigo,_ano,_lanc,_seq,_parcela,_compl);
            decimal? _soma = _listaT.Sum(m => m.Valortributo);
            decimal _somaT = Math.Round((decimal)_soma,2);

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Tributario_bll tributarioRepositoryTmp = new Tributario_bll("GTIconnection");
            //Criar o documento para ela
            Numdocumento regDoc = new Numdocumento {
                Valorguia = _somaT,
                Emissor = "Parc/Web",
                Datadocumento = DateTime.Now,
                Registrado = true,
                Percisencao = 0
            };
            regDoc.Percisencao = 0;
            int _novo_documento = tributarioRepositoryTmp.Insert_Documento(regDoc);

            Parceladocumento pd = new Parceladocumento() {
                Codreduzido = _codigo,
                Anoexercicio = _ano,
                Codlancamento = _lanc,
                Seqlancamento = _seq,
                Numparcela = _parcela,
                Codcomplemento = _compl,
                Numdocumento = _novo_documento
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
                Valor_Boleto = Functions.RetornaNumero(_somaT.ToString()),
                Data_Vencimento_String = Convert.ToDateTime(_dataVencto.ToString()).ToString("ddMMyyyy"),
                Data_Vencimento = _dataVencto,
                Numero_Processo = _proc
            };

            //***Enviar para registro ***
            using(var client = new HttpClient()) {
                var values = new {
                    msgLoja = " RECEBER SOMENTE ATE O VENCIMENTO, APOS ATUALIZAR O BOLETO NO SITE www.jaboticabal.sp.gov.br, referente ao parcelamento: " + model.Numero_Processo,
                    cep = Convert.ToInt64(Regex.Replace(model.Cep," [^.0-9]","")),
                    uf = model.UF,
                    cidade = model.Cidade,
                    endereco = model.Endereco,
                    nome = model.Nome,
                    urlInforma = "sistemas.jaboticabal.sp.gov.br/gti",
                    urlRetorno = "sistemas.jaboticabal.sp.gov.br/gti",
                    tpDuplicata = "DS",
                    dataLimiteDesconto = 0,
                    valorDesconto = 0,
                    indicadorPessoa = model.CpfCnpjLabel.Length == 14 ? 2 : 1,
                    cpfCnpj = Regex.Replace(model.CpfCnpjLabel," [^0-9]",""),
                    tpPagamento = 2,
                    dtVenc = model.Data_Vencimento_String,
                    qtdPontos = 0,
                    valor = Convert.ToInt64(model.Valor_Boleto),
                    refTran = string.IsNullOrEmpty(model.RefTran) ? 0 : Convert.ToInt64(model.RefTran),
                    idConv = 317203
                };


                string URLAuth = "https://mpag.bb.com.br/site/mpag/";
                string postString = string.Format("msgLoja={0}&cep={1}&uf={2}&cidade={3}&endereco={4}&nome={5}&urlInforma={6}&urlRetorno={7}&tpDuplicata={8}&dataLimiteDesconto={9}&valorDesconto={10}" +
                    "&indicadorPessoa={11}&cpfCnpj={12}&tpPagamento={13}&dtVenc={14}&qtdPontos={15}&valor={16}&refTran={17}&idConv={18}",values.msgLoja,values.cep,values.uf,values.cidade,values.endereco,
                    values.nome,values.urlInforma,values.urlRetorno,values.tpDuplicata,values.dataLimiteDesconto,values.valorDesconto,values.indicadorPessoa,values.cpfCnpj,values.tpPagamento,values.dtVenc,
                    values.qtdPontos,values.valor,values.refTran,values.idConv);

                const string contentType = "application/x-www-form-urlencoded";
                ServicePointManager.Expect100Continue = false;

                CookieContainer cookies = new CookieContainer();
                HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
                webRequest.Method = "POST";
                webRequest.ContentType = contentType;
                webRequest.CookieContainer = cookies;
                webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1";
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                webRequest.Referer = "https://mpag.bb.com.br/site/mpag/";

                StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                requestWriter.Write(postString);
                requestWriter.Close();

                StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                string responseData = responseReader.ReadToEnd();
                responseReader.Close();
                webRequest.GetResponse().Close();



            }


            //*** Gerar o carnê de parcelamento *****
            Processoreparc pr = tributarioRepository.Retorna_Processo_Parcelamento(_proc);
            short _totParcela = (short)pr.Qtdeparcela;

            List<DebitoStructure> ListaDebito = tributarioRepository.Lista_Parcelas_Parcelamento_Ano(_codigo,DateTime.Now.Year,_seq);
            short _index = 0;
            string _convenio = "2873532";
            List<Boletoguia> ListaBoleto = new List<Boletoguia>();
            foreach(DebitoStructure item in ListaDebito) {
                Boletoguia reg = new Boletoguia {
                    Usuario = "Gti.Web/Parcelamento",
                    Computer = "web",
                    Seq = _index,
                    Codreduzido = _codigo.ToString("000000"),
                    Nome = _nome,
                    Cpf = _cpfcnpj,
                    Endereco = _endereco,
                    Cidade = _cidade,
                    Uf = _uf,
                    Cep = _cep,
                    Desclanc = "PARCELAMENTO DE DÉBITOS",
                    Fulllanc = "PARCELAMENTO DE DÉBITOS",
                    Numdoc = item.Numero_Documento.ToString(),
                    Numparcela = (short)item.Numero_Parcela,
                    Datadoc = DateTime.Now,
                    Datavencto = item.Data_Vencimento,
                    Numdoc2 = item.Numero_Documento.ToString(),
                    Valorguia = item.Soma_Principal,
                    Totparcela = _totParcela,
                    Obs = "Referente ao parcelamento de débitos: processo nº " + _proc,
                    Numproc = _proc
                };

                //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
                DateTime _data_base = Convert.ToDateTime("07/10/1997");
                TimeSpan ts = Convert.ToDateTime(item.Data_Vencimento) - _data_base;
                int _fator_vencto = ts.Days;
                string _quinto_grupo = String.Format("{0:D4}",_fator_vencto);
                string _valor_boleto_str = string.Format("{0:0.00}",reg.Valorguia);
                _quinto_grupo += string.Format("{0:D10}",Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
                string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}",Convert.ToInt32(_convenio));
                _barra += String.Format("{0:D10}",Convert.ToInt64(reg.Numdoc)) + "17";
                string _campo1 = "0019" + _barra.Substring(19,5);
                string _digitavel = _campo1 + Functions.Calculo_DV10(_campo1).ToString();
                string _campo2 = _barra.Substring(23,10);
                _digitavel += _campo2 + Functions.Calculo_DV10(_campo2).ToString();
                string _campo3 = _barra.Substring(33,10);
                _digitavel += _campo3 + Functions.Calculo_DV10(_campo3).ToString();
                string _campo5 = _quinto_grupo;
                string _campo4 = Functions.Calculo_DV11(_barra).ToString();
                _digitavel += _campo4 + _campo5;
                _barra = _barra.Substring(0,4) + _campo4 + _barra.Substring(4,_barra.Length - 4);
                //**Resultado final**
                string _linha_digitavel = _digitavel.Substring(0,5) + "." + _digitavel.Substring(5,5) + " " + _digitavel.Substring(10,5) + "." + _digitavel.Substring(15,6) + " ";
                _linha_digitavel += _digitavel.Substring(21,5) + "." + _digitavel.Substring(26,6) + " " + _digitavel.Substring(32,1) + " " + Functions.StringRight(_digitavel,14);
                string _codigo_barra = Functions.Gera2of5Str(_barra);
                //**************************************************
                reg.Totparcela = (short)ListaDebito.Count;
                if(item.Numero_Parcela == 0) {
                    reg.Parcela = "Única";
                } else
                    reg.Parcela = reg.Numparcela.ToString("00") + "/" + _totParcela.ToString("00");

                reg.Digitavel = _linha_digitavel;
                reg.Codbarra = _codigo_barra;
                reg.Nossonumero = _convenio + String.Format("{0:D10}",Convert.ToInt64(reg.Numdoc));
                ListaBoleto.Add(reg);
                _index++;

            }
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            Session["sid"] = "";
            Tributario_bll tributario_Class = new Tributario_bll(_connection);
            if(ListaBoleto.Count > 0) {
                tributario_Class.Insert_Carne_Web(Convert.ToInt32(ListaBoleto[0].Codreduzido),DateTime.Now.Year);
                DataSet Ds = Functions.ToDataSet(ListaBoleto);
                ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia",Ds.Tables[0]);
                ReportViewer viewer = new ReportViewer();
                viewer.LocalReport.Refresh();
                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_Parcelamento.rdlc"); ;
                viewer.LocalReport.DataSources.Add(rdsAct);
                byte[] bytes = viewer.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,out string[] streamIds,out Warning[] warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition","attachment; filename= guia_pmj" + "." + extension);
                Response.OutputStream.Write(bytes,0,bytes.Length);
                Response.Flush();
                Response.End();
            }




            //***************************************


            return View(model);
        }

        public ActionResult Termo_anuencia_print(string p) {
            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Termo_Anuencia.rpt"));

            try {
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream,"application/pdf","Termo_Anuencia.pdf");
            } catch(Exception ex) {
                throw ex;
            }
        }

        public ActionResult Termo_confissao_divida(string p) {
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);

            string _nome = _master.Contribuinte_nome;
            string _nomeR = _master.Requerente_Nome;
            string _cod = _master.Contribuinte_Codigo.ToString("000000");
            string _doc = Functions.FormatarCpfCnpj(_master.Contribuinte_cpfcnpj);
            string _docR = Functions.FormatarCpfCnpj(_master.Requerente_CpfCnpj);
            string _qtdeParc = _master.Qtde_Parcela.ToString("00");
            string _valor = _master.Sim_Total.ToString("#0.00");
            string _proc = _master.Processo_Extenso;
            string _data = _master.Data_Geracao.ToString("dd/MM/yyyy");
            string _end = "";
            string _vct = Convert.ToDateTime(_master.Data_Vencimento).ToString("dd/MM/yyyy");
            _end = _master.Contribuinte_endereco + " - " + _master.Contribuinte_bairro;

            List<SpParcelamentoOrigem> _listaSelected = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            IEnumerable<short> _listaAnos = _listaSelected.Select(o => o.Exercicio).Distinct();
            string _ano = "";
            foreach(short item in _listaAnos) {
                _ano += item.ToString() + ", ";
            }
            _ano = _ano.Substring(0,_ano.Length - 2);

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/Termo_ConfDivida.rpt"));

            try {
                rd.SetParameterValue("NOME",_nome);
                rd.SetParameterValue("PROCESSO",_proc);
                rd.SetParameterValue("DATACONF",_data);
                rd.SetParameterValue("CODIGO",_cod);
                rd.SetParameterValue("DOC",_doc);
                rd.SetParameterValue("QTDEPARC",_qtdeParc);
                rd.SetParameterValue("VALOR",_valor);
                rd.SetParameterValue("ENDERECO",_end);
                rd.SetParameterValue("EXERCICIO",_ano);
                rd.SetParameterValue("DATAVENCTO",_vct);
                rd.SetParameterValue("NOMER",_nomeR);
                rd.SetParameterValue("DOCR",_docR);

                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream,"application/pdf","Termo_ConfDivida.pdf");
            } catch(Exception ex) {
                throw ex;
            }
        }

        [Route("Parc_cid")]
        [HttpGet]
        public ActionResult Parc_cid(string p) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(p);
            if(!_existe)
                return RedirectToAction("Login_gti","Home");

            //Load Master
            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            ParcelamentoViewModel model = new ParcelamentoViewModel() {
                Guid = p,
            };

            int _codigo = _master.Requerente_Codigo;
            model.Requerente = new Parc_Requerente() {
                Codigo = _codigo,
                Nome = _master.Requerente_Nome,
                Cpf_Cnpj = Functions.FormatarCpfCnpj(_master.Requerente_CpfCnpj)
            };

            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            CidadaoStruct _cidadao = cidadaoRepository.Dados_Cidadao(_codigo);
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);

            if(_cidadao.EtiquetaC == "S") {
                Bairro _bairro = null;
                if(_cidadao.CodigoLogradouroC == null) {
                    model.Requerente.Bairro_Nome = _cidadao.NomeBairroR;
                } else {
                    _bairro = enderecoRepository.RetornaLogradouroBairro((int)_cidadao.CodigoLogradouroC,(short)_cidadao.NumeroC);
                    model.Requerente.Bairro_Nome = _bairro.Descbairro;
                }
                model.Requerente.Logradouro_Codigo = _cidadao.CodigoLogradouroC == null ? 0 : (int)_cidadao.CodigoLogradouroC;
                model.Requerente.Logradouro_Nome = _cidadao.EnderecoC;
                model.Requerente.Numero = (int)_cidadao.NumeroC;
                model.Requerente.Complemento = _cidadao.ComplementoC;
                model.Requerente.Bairro_Codigo = _bairro.Codbairro;
                model.Requerente.Cidade_Codigo = (int)_cidadao.CodigoCidadeC;
                model.Requerente.Cidade_Nome = _cidadao.NomeCidadeC;
                model.Requerente.UF = _cidadao.UfC;
                model.Requerente.Cep = _cidadao.CepC == null ? "" : ((int)_cidadao.CepC).ToString("00000-000");
                model.Requerente.Email = _cidadao.EmailC;
                model.Requerente.Telefone = _cidadao.TelefoneC;
                model.Requerente.TipoEnd = "C";
            } else {
                Bairro _bairro = null;
                if(_cidadao.CodigoLogradouroR == null) {
                    model.Requerente.Bairro_Nome = _cidadao.NomeBairroR;
                } else {
                    _bairro = enderecoRepository.RetornaLogradouroBairro((int)_cidadao.CodigoLogradouroR,(short)_cidadao.NumeroR);
                    model.Requerente.Bairro_Nome = _bairro.Descbairro;
                }
                model.Requerente.Logradouro_Codigo = _cidadao.CodigoLogradouroR == null ? 0 : (int)_cidadao.CodigoLogradouroR;
                model.Requerente.Logradouro_Nome = _cidadao.EnderecoR;
                model.Requerente.Numero = (int)_cidadao.NumeroR;
                model.Requerente.Complemento = _cidadao.ComplementoR;
                model.Requerente.Bairro_Codigo = (int)_cidadao.CodigoBairroR;
                model.Requerente.Cidade_Codigo = (int)_cidadao.CodigoCidadeR;
                model.Requerente.Cidade_Nome = _cidadao.NomeCidadeR;
                model.Requerente.UF = _cidadao.UfR;
                model.Requerente.Cep = _cidadao.CepR == null ? "" : ((int)_cidadao.CepR).ToString("00000-000");
                model.Requerente.Email = _cidadao.EmailR;
                model.Requerente.Telefone = _cidadao.TelefoneR;
                model.Requerente.TipoEnd = "R";
            }
            if(model.Requerente.Bairro_Nome == null)
                model.Requerente.Bairro_Nome = enderecoRepository.Retorna_Bairro(model.Requerente.UF,model.Requerente.Cidade_Codigo,model.Requerente.Bairro_Codigo);

            return View(model);
        }

        [Route("Parc_cid")]
        [HttpPost]
        public ActionResult Parc_cid(ParcelamentoViewModel model,string action) {

            if(action == "btnCep") {
                if(model.Requerente.Cep == null || model.Requerente.Cep.Length < 9) {
                    ViewBag.Error = "* Cep do Requerente inválido, utilize o formato 00000-000.";
                    if(model.Requerente != null)
                        model.Requerente.Cep = Convert.ToInt32(model.Requerente.Cep).ToString("00000-000");
                    return View(model);
                }

                var cepObj = GTI_Mvc.Classes.Cep.Busca_CepDB(Convert.ToInt32(Functions.RetornaNumero(model.Requerente.Cep)));
                if(cepObj.Cidade != null) {
                    string rua = cepObj.Endereco;
                    if(rua.IndexOf('-') > 0) {
                        rua = rua.Substring(0,rua.IndexOf('-'));
                    }

                    Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                    LogradouroStruct _log = enderecoRepository.Retorna_Logradouro_Cep(Convert.ToInt32(Functions.RetornaNumero(cepObj.CEP)));
                    if(_log.Endereco != null) {
                        model.Requerente.Logradouro_Codigo = (int)_log.CodLogradouro;
                        model.Requerente.Logradouro_Nome = _log.Endereco;
                    } else {
                        model.Requerente.Logradouro_Codigo = 0;
                        model.Requerente.Logradouro_Nome = rua.ToUpper();
                    }

                    Bairro bairro = enderecoRepository.RetornaLogradouroBairro(model.Requerente.Logradouro_Codigo,(short)model.Requerente.Numero);
                    if(bairro.Descbairro != null) {
                        model.Requerente.Bairro_Codigo = bairro.Codbairro;
                        model.Requerente.Bairro_Nome = bairro.Descbairro;
                    } else {
                        string _uf = cepObj.Estado;
                        string _cidade = cepObj.Cidade;
                        string _bairro = cepObj.Bairro;
                        int _codcidade = enderecoRepository.Retorna_Cidade(_uf,_cidade);
                        if(_codcidade > 0) {
                            model.Requerente.Cidade_Codigo = _codcidade;
                            if(_codcidade != 413) {
                                //verifica se bairro existe nesta cidade
                                bool _existeBairro = enderecoRepository.Existe_Bairro(_uf,_codcidade,_bairro);
                                if(!_existeBairro) {
                                    Bairro reg = new Bairro() {
                                        Siglauf = _uf,
                                        Codcidade = (short)_codcidade,
                                        Descbairro = _bairro.ToUpper()
                                    };
                                    int _codBairro = enderecoRepository.Incluir_bairro(reg);
                                    model.Requerente.Bairro_Codigo = _codBairro;
                                }
                            }
                            model.Requerente.Bairro_Codigo = enderecoRepository.Retorna_Bairro(_uf,_codcidade,_bairro);
                            model.Requerente.Bairro_Nome = cepObj.Bairro.ToUpper();
                        } else {
                            model.Requerente.Cidade_Codigo = 0;
                        }
                    }

                    model.Requerente.Cidade_Nome = cepObj.Cidade.ToUpper();
                    model.Requerente.UF = cepObj.Estado;
                } else {
                    model.Requerente.Logradouro_Codigo = 0;
                    model.Requerente.Logradouro_Nome = "";
                    model.Requerente.Bairro_Codigo = 0;
                    model.Requerente.Bairro_Nome = "";
                    model.Requerente.Cidade_Codigo = 0;
                    model.Requerente.Cidade_Nome = "";
                    model.Requerente.Numero = 0;
                    model.Requerente.Complemento = "";
                    model.Requerente.UF = "";

                    ViewBag.Error = "* Cep do comprador não localizado.";
                }
                return View(model);
            } else {
                if(action == "btnValida" || action == "Parc_cid") {
                    Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
                    bool _existe = parcelamentoRepository.Existe_Parcelamento_Web_Master(model.Guid);
                    if(!_existe)
                        return RedirectToAction("Login_gti","Home");

                    //Load Master
                    Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(model.Guid);
                    Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                    CidadaoStruct _cidOriginal = cidadaoRepository.Dados_Cidadao(_master.Requerente_Codigo);


                    Cidadao _cidadao = new Cidadao() {
                        Codcidadao = _master.Requerente_Codigo,
                        Nomecidadao = _cidOriginal.Nome,
                        Data_nascimento = _cidOriginal.DataNascto,
                        Codprofissao = _cidOriginal.CodigoProfissao,
                        Cnh = _cidOriginal.Cnh,
                        Rg = _cidOriginal.Rg,
                        Orgao = _cidOriginal.Orgao,
                        Orgaocnh = _cidOriginal.Orgaocnh
                    };

                    string _doc = Functions.RetornaNumero(model.Requerente.Cpf_Cnpj);
                    if(_doc.Length == 14) {
                        _cidadao.Cpf = null;
                        _cidadao.Cnpj = _doc;
                        _cidadao.Juridica = true;
                    } else {
                        _cidadao.Cpf = _doc;
                        _cidadao.Cnpj = null;
                        _cidadao.Juridica = false;
                    }

                    if(model.Requerente.TipoEnd == "R") {
                        _cidadao.Cep = Convert.ToInt32(Functions.RetornaNumero(model.Requerente.Cep));
                        _cidadao.Codbairro = (short)model.Requerente.Bairro_Codigo;
                        _cidadao.Codcidade = (short)model.Requerente.Cidade_Codigo;
                        _cidadao.Siglauf = model.Requerente.UF;
                        _cidadao.Numimovel = (short)model.Requerente.Numero;
                        _cidadao.Complemento = model.Requerente.Complemento;
                        _cidadao.Telefone = model.Requerente.Telefone;
                        _cidadao.Email = model.Requerente.Email;
                        _cidadao.Etiqueta = "S";
                        _cidadao.Codpais = 1;
                        _cidadao.Temfone = string.IsNullOrEmpty(model.Requerente.Telefone) ? true : false;
                        _cidadao.Whatsapp = _cidOriginal.Whatsapp;
                        _cidadao.Codlogradouro = model.Requerente.Logradouro_Codigo;
                        if(model.Requerente.Logradouro_Codigo == 0) {
                            _cidadao.Nomelogradouro = model.Requerente.Logradouro_Nome;
                        }
                    } else {
                        _cidadao.Cep2 = Convert.ToInt32(Functions.RetornaNumero(model.Requerente.Cep));
                        _cidadao.Codbairro2 = (short)model.Requerente.Bairro_Codigo;
                        _cidadao.Codcidade2 = (short)model.Requerente.Cidade_Codigo;
                        _cidadao.Siglauf2 = model.Requerente.UF;
                        _cidadao.Numimovel2 = (short)model.Requerente.Numero;
                        _cidadao.Complemento2 = model.Requerente.Complemento;
                        _cidadao.Telefone2 = model.Requerente.Telefone;
                        _cidadao.Email2 = model.Requerente.Email;
                        _cidadao.Codpais2 = 1;
                        _cidadao.Etiqueta2 = "S";
                        _cidadao.Temfone2 = string.IsNullOrEmpty(model.Requerente.Telefone) ? true : false;
                        _cidadao.Whatsapp2 = _cidOriginal.Whatsapp;
                        _cidadao.Codlogradouro2 = model.Requerente.Logradouro_Codigo;
                        if(model.Requerente.Logradouro_Codigo == 0) {
                            _cidadao.Nomelogradouro2 = model.Requerente.Logradouro_Nome;
                        }
                    }
                    Exception ex = cidadaoRepository.Alterar_cidadao(_cidadao);

                    //Atualiza Master
                    int _user_id = Convert.ToInt32(Session["hashid"]);
                    Sistema_bll sistemaRepository = new Sistema_bll(_connection);
                    Usuario_web _user = sistemaRepository.Retorna_Usuario_Web(_user_id);

                    CidadaoStruct _cid = cidadaoRepository.Dados_Cidadao(_master.Requerente_Codigo);
                    Parc_Requerente _req = new Parc_Requerente {
                        Codigo = _cid.Codigo,
                        Nome = _cid.Nome
                    };
                    _req.Cpf_Cnpj = Functions.FormatarCpfCnpj(model.Requerente.Cpf_Cnpj);
                    string _tipoEnd = _cid.EnderecoC == "S" ? "C" : "R";
                    _req.TipoEnd = _tipoEnd == "R" ? "RESIDENCIAL" : "COMERCIAL";
                    if(_tipoEnd == "R") {
                        _req.Bairro_Nome = _cid.NomeBairroR;
                        _req.Cidade_Nome = _cid.NomeCidadeR;
                        _req.UF = _cid.UfR;
                        _req.Logradouro_Codigo = _cid.CodigoLogradouroR == null ? 0 : (int)_cid.CodigoLogradouroR;
                        _req.Logradouro_Nome = _cid.EnderecoR;
                        _req.Numero = (int)_cid.NumeroR;
                        _req.Complemento = _cid.ComplementoR;
                        _req.Telefone = _cid.TelefoneR;
                        _req.Cep = _cid.CepR.ToString();
                        _req.Email = _cid.EmailR;
                    } else {
                        _req.Bairro_Nome = _cid.NomeBairroC;
                        _req.Cidade_Nome = _cid.NomeCidadeC;
                        _req.UF = _cid.UfC;
                        _req.Logradouro_Codigo = _cid.CodigoLogradouroC == null ? 0 : (int)_cid.CodigoLogradouroC;
                        _req.Logradouro_Nome = _cid.EnderecoC;
                        _req.Numero = (int)_cid.NumeroC;
                        _req.Complemento = _cid.ComplementoC;
                        _req.Telefone = _cid.TelefoneC;
                        _req.Cep = _cid.CepC.ToString();
                        _req.Email = _cid.EmailC;
                    }
                    model.Requerente = _req;

                    Parcelamento_web_master _m = new Parcelamento_web_master() {
                        Guid = _master.Guid,
                        Requerente_Bairro = _req.Bairro_Nome,
                        Requerente_Cep = Convert.ToInt32(Functions.RetornaNumero(_req.Cep)),
                        Requerente_Complemento = _req.Complemento,
                        Requerente_Cidade = _req.Cidade_Nome,
                        Requerente_Email = _req.Email,
                        Requerente_Telefone = _req.Telefone,
                        Requerente_Uf = _req.UF,
                        Requerente_Numero = _req.Numero,
                        Requerente_Logradouro = _req.Logradouro_Nome
                    };
                    ex = parcelamentoRepository.Atualizar_Requerente_Master(_m);
                }
            }


            return RedirectToAction("Parc_req","Parcelamento");
        }

        [Route("Parc_rel")]
        [HttpGet]
        public ActionResult Parc_rel() {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            ParcelamentoViewModel model = new ParcelamentoViewModel();
            return View(model);
        }

        [Route("Parc_rel")]
        [HttpPost]
        public ActionResult Parc_rel(ParcelamentoViewModel model) {
            if(Session["hashid"] == null)
                return RedirectToAction("Login","Home");

            bool _isDate1 = Functions.IsDate(model.DataDe);
            bool _isDate2 = Functions.IsDate(model.DataAte);

            if(!_isDate1) {
                ViewBag.Result = "Data inicial inválida!";
                return View(model);
            }
            if(!_isDate2) {
                ViewBag.Result = "Data final inválida!";
                return View(model);
            }

            string _sData1 = DateTime.ParseExact(model.DataDe,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("MM/dd/yyyy",CultureInfo.InvariantCulture);
            string _sData2 = DateTime.ParseExact(model.DataAte,"dd/MM/yyyy",CultureInfo.InvariantCulture).ToString("MM/dd/yyyy",CultureInfo.InvariantCulture);

            DateTime _data1 = DateTime.Parse(_sData1,CultureInfo.InvariantCulture);
            DateTime _data2 = DateTime.Parse(_sData2,CultureInfo.InvariantCulture);

            if(_data1 > _data2) {
                ViewBag.Result = "Data inicial maior que data final!";
                return View(model);
            }

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);
            List<Parcelamento_web_master> _listaMaster = parcelamentoRepository.Lista_Parcelamento_Web_Master(_sData1,_sData2);
            if(_listaMaster.Count == 0) {
                ViewBag.Result = "Nenhum parcelamento foi gerado no período informado!";
                return View(model);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(System.Web.HttpContext.Current.Server.MapPath("~/Reports/ParcelamentoWeb.rpt"));

            TableLogOnInfos crtableLogoninfos = new TableLogOnInfos();
            TableLogOnInfo crtableLogoninfo = new TableLogOnInfo();
            ConnectionInfo crConnectionInfo = new ConnectionInfo();
            Tables CrTables;
            string myConn = ConfigurationManager.ConnectionStrings[_connection].ToString();
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(myConn);
            string IPAddress = builder.DataSource;
            string _userId = builder.UserID;
            string _pwd = builder.Password;

            crConnectionInfo.ServerName = IPAddress;
            crConnectionInfo.DatabaseName = "TributacaoTeste";
            crConnectionInfo.UserID = _userId;
            crConnectionInfo.Password = _pwd;
            CrTables = rd.Database.Tables;
            foreach(Table CrTable in CrTables) {
                crtableLogoninfo = CrTable.LogOnInfo;
                crtableLogoninfo.ConnectionInfo = crConnectionInfo;
                CrTable.ApplyLogOnInfo(crtableLogoninfo);
            }

            try {
                //      rd.RecordSelectionFormula = "{Parcelamento_Web_Master.data_geracao}>=#" + _data1 + "# and {Parcelamento_Web_Master.data_geracao}<=#" + _data2 + "#";
                rd.SetParameterValue("DATA1",_data1);
                rd.SetParameterValue("DATA2",_data2);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream,"application/pdf","Lista_Parcelamento.pdf");
            } catch {
                throw;
            }



            return View(model);
        }


        public void Parc_reqe_old(string p) {

            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

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


            //Load Origem
            decimal _SomaP = 0, _SomaM = 0, _SomaJ = 0, _SomaC = 0, _SomaT = 0, _SomaE = 0;
            List<SpParcelamentoOrigem> ListaOrigem = parcelamentoRepository.Lista_Parcelamento_Selected(p);
            bool _bAjuizado = ListaOrigem[0].Ajuizado == "S";
            List<SelectDebitoParcelamentoEditorViewModel> _listaP = new List<SelectDebitoParcelamentoEditorViewModel>();
            foreach(SpParcelamentoOrigem item in ListaOrigem) {
                SelectDebitoParcelamentoEditorViewModel d = new SelectDebitoParcelamentoEditorViewModel() {
                    Ajuizado = item.Ajuizado,
                    Complemento = item.Complemento,
                    Data_vencimento = item.Data_vencimento,
                    Exercicio = item.Exercicio,
                    Idx = item.Idx,
                    Lancamento = item.Lancamento,
                    Nome_lancamento = Functions.TruncateTo(item.Nome_lancamento,16),
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
                    Valor_total = item.Valor_total,
                    Execucao_Fiscal = item.Execucao_Fiscal,
                    Protesto = item.Protesto
                };
                _listaP.Add(d);
                _SomaP += item.Valor_principal;
                _SomaM += item.Valor_multa;
                _SomaJ += item.Valor_juros;
                _SomaC += item.Valor_correcao;
                _SomaE += item.Valor_penalidade;
                _SomaT += Math.Round(item.Valor_principal,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_juros,2,MidpointRounding.AwayFromZero) + Math.Round(item.Valor_multa,2,MidpointRounding.AwayFromZero) + +Math.Round(item.Valor_correcao,2,MidpointRounding.AwayFromZero);
            }
            model.Soma_Principal = _SomaP;
            model.Soma_Multa = _SomaM;
            model.Soma_Juros = _SomaJ;
            model.Soma_Correcao = _SomaC;
            model.Soma_Penalidade = _SomaE;
            model.Soma_Total = _SomaT;
            model.Lista_Origem_Selected = _listaP;

            //Carrega Simulado
            model.Lista_Simulado = parcelamentoRepository.Retorna_Parcelamento_Web_Simulado(model.Guid,_master.Qtde_Parcela);

            //Atualiza Totais
            decimal _SomaH = 0, _SomaJapl = 0, _SomaL = 0;
            _SomaP = 0; _SomaC = 0; _SomaJ = 0; _SomaM = 0; _SomaT = 0;

            foreach(Parcelamento_Web_Simulado item in model.Lista_Simulado) {
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

        }

        public JsonResult bank_Method(string p) {
            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Tributario_bll tributarioRepositoryTmp = new Tributario_bll("GTIconnection");

            string _ret;
            int _userId = Convert.ToInt32(Session["hashid"]);
            int nSid = Functions.GetRandomNumber();
            Parcelamento_bll parcelamentoRepository = new Parcelamento_bll(_connection);

            Parcelamento_web_master _master = parcelamentoRepository.Retorna_Parcelamento_Web_Master(p);
            int _codigo = _master.Contribuinte_Codigo;
            string _nome = _master.Contribuinte_nome;
            string _cpfcnpj = _master.Contribuinte_cpfcnpj;
            string _endereco = _master.Contribuinte_endereco + " " + _master.Contribuinte_bairro;
            string _bairro = _master.Contribuinte_bairro ?? "";
            string _cidade = _master.Contribuinte_cidade;
            string _uf = _master.Contribuinte_uf;
            string _cep = _master.Contribuinte_cep.ToString();
            string _proc = _master.Processo_Extenso;


            //Dados das parcelas do parcelamento do exercício
            List<Destinoreparc> _listaD = parcelamentoRepository.Lista_Destino_Parcelamento(_master.Processo_Ano,_master.Processo_Numero);
            List<DebitoStructure> ListaDebito = parcelamentoRepository.Lista_Parcelas_Parcelamento_Ano_Web(_codigo,2021,_listaD[0].Numsequencia);
            short _index = 0;
            string _convenio = "2873532";
            List<Boletoguia> ListaBoleto = new List<Boletoguia>();

            foreach(DebitoStructure _parc in ListaDebito) {
                short _ano = (short)_parc.Ano_Exercicio;
                short _lanc = 20;
                short _seq = (short)_parc.Sequencia_Lancamento;
                byte _parcela = (byte)_parc.Numero_Parcela;
                byte _compl = (byte)_parc.Complemento;
                DateTime _dataVencto = (DateTime)_parc.Data_Vencimento;

                List<Debitotributo> _listaT = parcelamentoRepository.Lista_Debito_Tributo(_codigo,_ano,_lanc,_seq,_parcela,_compl);
                decimal? _soma = _listaT.Sum(m => m.Valortributo);
                decimal _somaT = Math.Round((decimal)_soma,2);

                //Criar o documento para ela
                Numdocumento regDoc = new Numdocumento {
                    Valorguia = _somaT,
                    Emissor = "Parc/Web",
                    Datadocumento = DateTime.Now,
                    Registrado = true,
                    Percisencao = 0
                };
                regDoc.Percisencao = 0;
                int _novo_documento = tributarioRepositoryTmp.Insert_Documento(regDoc);

                Parceladocumento pd = new Parceladocumento() {
                    Codreduzido = _codigo,
                    Anoexercicio = _ano,
                    Codlancamento = _lanc,
                    Seqlancamento = _seq,
                    Numparcela = _parcela,
                    Codcomplemento = _compl,
                    Numdocumento = _novo_documento
                };
                Exception ex = tributarioRepository.Insert_Parcela_Documento(pd);

                string _refTran = "287353200" + _novo_documento.ToString();
                if(_parcela == 1) {
                    //1ª parcela registrar por webservice

                    DebitoListViewModel model = new DebitoListViewModel() {
                        Nome = _nome,
                        Inscricao = _codigo,
                        CpfCnpjLabel = _cpfcnpj,
                        Endereco = _endereco,
                        Cidade = _cidade,
                        UF = _uf,
                        Cep = _cep,
                        RefTran = _refTran,
                        Valor_Boleto = Functions.RetornaNumero(_somaT.ToString()),
                        Data_Vencimento_String = Convert.ToDateTime(_dataVencto.ToString()).ToString("ddMMyyyy"),
                        Data_Vencimento = _dataVencto,
                        Numero_Processo = _proc
                    };

                    //***Enviar para registro ***
                    using(var client = new HttpClient()) {
                        var values = new {
                            msgLoja = " RECEBER SOMENTE ATE O VENCIMENTO, APOS ATUALIZAR O BOLETO NO SITE www.jaboticabal.sp.gov.br, referente ao parcelamento: " + model.Numero_Processo,
                            cep = Convert.ToInt64(Regex.Replace(model.Cep," [^.0-9]","")),
                            uf = model.UF,
                            cidade = model.Cidade,
                            endereco = model.Endereco,
                            nome = model.Nome,
                            urlInforma = "sistemas.jaboticabal.sp.gov.br/gti",
                            urlRetorno = "sistemas.jaboticabal.sp.gov.br/gti",
                            tpDuplicata = "DS",
                            dataLimiteDesconto = 0,
                            valorDesconto = 0,
                            indicadorPessoa = model.CpfCnpjLabel.Length == 14 ? 2 : 1,
                            cpfCnpj = Regex.Replace(model.CpfCnpjLabel," [^0-9]",""),
                            tpPagamento = 2,
                            dtVenc = model.Data_Vencimento_String,
                            qtdPontos = 0,
                            valor = Convert.ToInt64(model.Valor_Boleto),
                            refTran = string.IsNullOrEmpty(model.RefTran) ? 0 : Convert.ToInt64(model.RefTran),
                            idConv = 317203
                        };

                        string URLAuth = "https://mpag.bb.com.br/site/mpag/";
                        string postString = string.Format("msgLoja={0}&cep={1}&uf={2}&cidade={3}&endereco={4}&nome={5}&urlInforma={6}&urlRetorno={7}&tpDuplicata={8}&dataLimiteDesconto={9}&valorDesconto={10}" +
                            "&indicadorPessoa={11}&cpfCnpj={12}&tpPagamento={13}&dtVenc={14}&qtdPontos={15}&valor={16}&refTran={17}&idConv={18}",values.msgLoja,values.cep,values.uf,values.cidade,values.endereco,
                            values.nome,values.urlInforma,values.urlRetorno,values.tpDuplicata,values.dataLimiteDesconto,values.valorDesconto,values.indicadorPessoa,values.cpfCnpj,values.tpPagamento,values.dtVenc,
                            values.qtdPontos,values.valor,values.refTran,values.idConv);

                        const string contentType = "application/x-www-form-urlencoded";
                        ServicePointManager.Expect100Continue = false;

                        CookieContainer cookies = new CookieContainer();
                        HttpWebRequest webRequest = WebRequest.Create(URLAuth) as HttpWebRequest;
                        webRequest.Method = "POST";
                        webRequest.ContentType = contentType;
                        webRequest.CookieContainer = cookies;
                        webRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1";
                        webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        webRequest.Referer = "https://mpag.bb.com.br/site/mpag/";

                        StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                        requestWriter.Write(postString);
                        requestWriter.Close();

                        StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                        string responseData = responseReader.ReadToEnd();
                        responseReader.Close();
                        webRequest.GetResponse().Close();

                        if(responseData.Contains("seguintes problemas")) {
                            _ret = "Erro ao registrar o documento" + _novo_documento.ToString();
                            return Json(new { success = false,msg = _ret,dados = responseData },JsonRequestBehavior.AllowGet);
                        }
                    }
                } else {
                    //Demais parcelas enviar para registrar por arquivo
                    Ficha_compensacao_documento ficha = new Ficha_compensacao_documento {
                        Nome = _nome.Length > 40 ? _nome.Substring(0,40) : _nome,
                        Endereco = _endereco.Length > 40 ? _endereco.Substring(0,40) : _endereco,
                        Bairro = _bairro.Length > 15 ? _bairro.Substring(0,15) : _bairro,
                        Cidade = _cidade.Length > 30 ? _cidade.Substring(0,30) : _cidade,
                        Cep = _cep ?? "14870000",
                        Cpf = _cpfcnpj,
                        Numero_documento = _novo_documento,
                        Data_vencimento = _dataVencto,
                        Valor_documento = Convert.ToDecimal(_somaT),
                        Uf = _uf
                    };
                    ex = tributarioRepository.Insert_Ficha_Compensacao_Documento(ficha);
                    if(ex == null)
                        ex = tributarioRepository.Marcar_Documento_Registrado(_novo_documento);
                }

                //Gera Boletos para impressão
                Boletoguia reg = new Boletoguia {
                    Usuario = "Gti.Web/LibParc",
                    Computer = "web",
                    Sid = nSid,
                    Seq = _index,
                    Codreduzido = _codigo.ToString("000000"),
                    Nome = _nome,
                    Cpf = _cpfcnpj,
                    Numimovel = 0,
                    Endereco = _endereco,
                    Complemento = "",
                    Bairro = _bairro,
                    Cidade = _cidade,
                    Uf = _uf,
                    Cep = _cep,
                    Desclanc = "PARCELAMENTO",
                    Fulllanc = "PARCELAMENTO",
                    Numdoc = _novo_documento.ToString(),
                    Numparcela = (short)_parc.Numero_Parcela,
                    Datadoc = DateTime.Now,
                    Datavencto = _parc.Data_Vencimento,
                    Numdoc2 = _novo_documento.ToString(),
                    Valorguia = _parc.Soma_Principal,
                    Valor_ISS = 0,
                    Valor_Taxa = 0,
                    Totparcela = (short)_master.Qtde_Parcela,
                    Obs = "Referente ao parcelamento de débitos: processo nº " + _master.Processo_Extenso,
                    Numproc = _master.Processo_Extenso
                };

                //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
                DateTime _data_base = Convert.ToDateTime("07/10/1997");
                TimeSpan ts = Convert.ToDateTime(_parc.Data_Vencimento) - _data_base;
                int _fator_vencto = ts.Days;
                string _quinto_grupo = String.Format("{0:D4}",_fator_vencto);
                string _valor_boleto_str = string.Format("{0:0.00}",reg.Valorguia);
                _quinto_grupo += string.Format("{0:D10}",Convert.ToInt64(Functions.RetornaNumero(_valor_boleto_str)));
                string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}",Convert.ToInt32(_convenio));
                _barra += String.Format("{0:D10}",Convert.ToInt64(reg.Numdoc)) + "17";
                string _campo1 = "0019" + _barra.Substring(19,5);
                string _digitavel = _campo1 + Functions.Calculo_DV10(_campo1).ToString();
                string _campo2 = _barra.Substring(23,10);
                _digitavel += _campo2 + Functions.Calculo_DV10(_campo2).ToString();
                string _campo3 = _barra.Substring(33,10);
                _digitavel += _campo3 + Functions.Calculo_DV10(_campo3).ToString();
                string _campo5 = _quinto_grupo;
                string _campo4 = Functions.Calculo_DV11(_barra).ToString();
                _digitavel += _campo4 + _campo5;
                _barra = _barra.Substring(0,4) + _campo4 + _barra.Substring(4,_barra.Length - 4);
                //**Resultado final**
                string _linha_digitavel = _digitavel.Substring(0,5) + "." + _digitavel.Substring(5,5) + " " + _digitavel.Substring(10,5) + "." + _digitavel.Substring(15,6) + " ";
                _linha_digitavel += _digitavel.Substring(21,5) + "." + _digitavel.Substring(26,6) + " " + _digitavel.Substring(32,1) + " " + Functions.StringRight(_digitavel,14);
                string _codigo_barra = Functions.Gera2of5Str(_barra);
                //**************************************************
                reg.Totparcela = (short)_master.Qtde_Parcela;
                reg.Parcela = reg.Numparcela.ToString("00") + "/" + _master.Qtde_Parcela.ToString("00");

                reg.Digitavel = _linha_digitavel;
                reg.Codbarra = _codigo_barra;
                reg.Nossonumero = _convenio + String.Format("{0:D10}",Convert.ToInt64(reg.Numdoc));
                ListaBoleto.Add(reg);

            }
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            if(ListaBoleto.Count > 0) {
                Exception ex = tributarioRepository.Insert_Carne_Web(Convert.ToInt32(ListaBoleto[0].Codreduzido),DateTime.Now.Year);
                DataSet Ds = Functions.ToDataSet(ListaBoleto);
                ReportDataSource rdsAct = new ReportDataSource("dsBoletoGuia",Ds.Tables[0]);
                ReportViewer viewer = new ReportViewer();
                viewer.LocalReport.Refresh();
                viewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Reports/Carne_Parcelamento.rdlc"); ;
                viewer.LocalReport.DataSources.Add(rdsAct);

                string strAttachment = Server.MapPath(@"/Reports/haha.pdf");
                byte[] renderdByte = viewer.LocalReport.Render("Pdf","",out mimeType,out encoding,out extension,out string[] streamIds,out Warning[] warnings);
                string base64EncodedPDF = Convert.ToBase64String(renderdByte);
                return Json(base64EncodedPDF,JsonRequestBehavior.AllowGet);


                //                byte[] bytes = viewer.LocalReport.Render("PDF",null,out mimeType,out encoding,out extension,out string[] streamIds,out Warning[] warnings);
                //                FileStream fs = new FileStream(@System.Web.HttpContext.Current.Server.MapPath("~/Files/tmp/"+ p + ".pdf"),FileMode.Create);
                //                fs.Write(bytes,0,bytes.Length);
                //                fs.Close();

                //                Response.Buffer = true;
                //                Response.Clear();
                //                Response.ContentType = mimeType;
                //                Response.AddHeader("content-disposition","attachment; filename= guia_pmj" + "." + extension);
                ////                Response.OutputStream.Write(bytes,0,bytes.Length);
                //                return Json(new { success = true,msg = "Sucesso",url = System.Web.HttpContext.Current.Server.MapPath("~/Files/tmp/" + p + ".pdf") },JsonRequestBehavior.AllowGet);
                //Response.Flush();
                //Response.End();

            } else {
                return Json(new { success = true,msg = "Sucesso" },JsonRequestBehavior.AllowGet);
            }
                        


        }

        // }











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