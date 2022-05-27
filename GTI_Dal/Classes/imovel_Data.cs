using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GTI_Models.modelCore;

namespace GTI_Dal.Classes {
    public class Imovel_Data {

        private readonly string _connection;

        public Imovel_Data( string sConnection) {
            _connection = sConnection;
            }

        public ImovelStruct Dados_Imovel(int nCodigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Cadimob
                           join c in db.Condominio on i.Codcondominio equals c.Cd_codigo into ic from c in ic.DefaultIfEmpty()
                           join b in db.Benfeitoria on i.Dt_codbenf equals b.Codbenfeitoria into ib from b in ib.DefaultIfEmpty()
                           join p in db.Pedologia on i.Dt_codpedol equals p.Codpedologia into ip from p in ip.DefaultIfEmpty()
                           join t in db.Topografia on i.Dt_codtopog equals t.Codtopografia into it from t in it.DefaultIfEmpty()
                           join s in db.Situacao on i.Dt_codsituacao equals s.Codsituacao into ist from s in ist.DefaultIfEmpty()
                           join cp in db.Categprop on i.Dt_codcategprop equals cp.Codcategprop into icp from cp in icp.DefaultIfEmpty()
                           join u in db.Usoterreno on i.Dt_codusoterreno equals u.Codusoterreno into iu from u in iu.DefaultIfEmpty()
                           where i.Codreduzido == nCodigo
                           select new ImovelStruct { Codigo = i.Codreduzido, Distrito = i.Distrito, Setor = i.Setor, Quadra = i.Quadra, Lote = i.Lote, Seq = i.Seq,
                               Unidade = i.Unidade, SubUnidade = i.Subunidade, NomeCondominio = c.Cd_nomecond, Imunidade = i.Imune, TipoMat = i.Tipomat, NumMatricula = i.Nummat,Cip = i.Cip, Conjugado=i.Conjugado,
                               Numero = i.Li_num, Complemento = i.Li_compl, QuadraOriginal = i.Li_quadras, LoteOriginal = i.Li_lotes, ResideImovel = i.Resideimovel, Inativo = i.Inativo,
                               FracaoIdeal = i.Dt_fracaoideal, Area_Terreno = i.Dt_areaterreno, Benfeitoria = i.Dt_codbenf, Categoria = i.Dt_codcategprop, Pedologia = i.Dt_codpedol, Topografia = i.Dt_codtopog,
                               Uso_terreno = i.Dt_codusoterreno, Situacao = i.Dt_codsituacao, EE_TipoEndereco = i.Ee_tipoend, Benfeitoria_Nome = b.Descbenfeitoria, Pedologia_Nome = p.Descpedologia,
                               Topografia_Nome = t.Desctopografia, Situacao_Nome = s.Descsituacao, Categoria_Nome = cp.Desccategprop, Uso_terreno_Nome = u.Descusoterreno, CodigoCondominio = c.Cd_codigo
                               }).FirstOrDefault();

                ImovelStruct row = new ImovelStruct();
                if (reg == null)
                    return row;
                row.Codigo = nCodigo;
                row.Distrito = reg.Distrito;
                row.Setor = reg.Setor;
                row.Quadra = reg.Quadra;
                row.Lote = reg.Lote;
                row.Seq = reg.Seq;
                row.Unidade = reg.Unidade;
                row.SubUnidade = reg.SubUnidade;
                row.Inscricao = reg.Distrito.ToString() + "." + reg.Setor.ToString("00") + "." + reg.Quadra.ToString("0000") + "." + reg.Lote.ToString("00000") + "." + reg.Seq.ToString("00") + "." + reg.Unidade.ToString("00") + "." + reg.SubUnidade.ToString("000");
                row.CodigoCondominio = reg.CodigoCondominio == null ? 0 : reg.CodigoCondominio;
                row.NomeCondominio = reg.NomeCondominio ?? "";
                row.Imunidade = reg.Imunidade == null ? false : Convert.ToBoolean(reg.Imunidade);
                row.Cip = reg.Cip == null ? false : Convert.ToBoolean(reg.Cip);
                row.Conjugado= reg.Conjugado == null ? false : Convert.ToBoolean(reg.Conjugado);
                row.ResideImovel = reg.ResideImovel == null ? false : Convert.ToBoolean(reg.ResideImovel);
                row.Inativo = reg.Inativo == null ? false : Convert.ToBoolean(reg.Inativo);
                if (reg.TipoMat == null || reg.TipoMat == "M")
                    row.TipoMat = "M";
                else
                    row.TipoMat = "T";
                row.NumMatricula = reg.NumMatricula;
                row.QuadraOriginal = reg.QuadraOriginal == null ? "" : reg.QuadraOriginal.ToString();
                row.LoteOriginal = reg.LoteOriginal == null ? "" : reg.LoteOriginal.ToString();
                row.FracaoIdeal = reg.FracaoIdeal;
                row.Area_Terreno = reg.Area_Terreno;
                row.Benfeitoria = reg.Benfeitoria;
                row.Benfeitoria_Nome = reg.Benfeitoria_Nome;
                row.Categoria = reg.Categoria;
                row.Categoria_Nome = reg.Categoria_Nome;
                row.Pedologia = reg.Pedologia;
                row.Pedologia_Nome = reg.Pedologia_Nome;
                row.Situacao = reg.Situacao;
                row.Situacao_Nome = reg.Situacao_Nome;
                row.Topografia = reg.Topografia;
                row.Topografia_Nome = reg.Topografia_Nome;
                row.Uso_terreno = reg.Uso_terreno;
                row.Uso_terreno_Nome = reg.Uso_terreno_Nome;
                row.EE_TipoEndereco = reg.EE_TipoEndereco;

                EnderecoStruct regEnd = Dados_Endereco(nCodigo, TipoEndereco.Local);
                row.CodigoLogradouro = regEnd.CodLogradouro;
                row.NomeLogradouro = regEnd.Endereco;
                row.NomeLogradouroAbreviado = regEnd.Endereco_Abreviado;
                row.Numero = regEnd.Numero;
                row.Complemento = regEnd.Complemento;
                row.Cep = regEnd.Cep;
                row.CodigoBairro = regEnd.CodigoBairro;
                row.NomeBairro = regEnd.NomeBairro;

                return row;
                }
            }//End LoadReg

        public decimal Soma_Area( int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var sum = db.Areas.Where(x => x.Codreduzido == Codigo).Sum(x => x.Areaconstr);
                return Convert.ToDecimal(sum);
                }
            }

        public int Qtde_Imovel_Cidadao(int CodigoImovel) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int Sql = (from v in db.Vwproprietarioduplicado join p in db.Proprietario on v.Codproprietario equals p.Codcidadao where p.Codreduzido == CodigoImovel && p.Tipoprop == "P" select v.Qtdeimovel).FirstOrDefault();
                return Sql;
                }
            }

        public List<ProprietarioStruct> Lista_Proprietario(int CodigoImovel, bool Principal = false) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from p in db.Proprietario
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao
                           where p.Codreduzido == CodigoImovel
                           orderby p.Principal descending
                           select new { p.Codcidadao, c.Nomecidadao, p.Tipoprop, p.Principal, c.Cpf, c.Cnpj, c.Rg });

                if (Principal)
                    reg = reg.Where(u => u.Tipoprop == "P" && u.Principal == true);

                List<ProprietarioStruct> Lista = new List<ProprietarioStruct>();
                foreach (var query in reg) {
                    string sDoc;
                    if (!string.IsNullOrEmpty(query.Cpf) && query.Cpf.ToString().Length > 5)
                        sDoc = Convert.ToInt64( query.Cpf).ToString("00000000000");
                    else {
                        if (!string.IsNullOrEmpty(query.Cnpj) && query.Cnpj.ToString().Length > 10)
                            sDoc = query.Cnpj;
                        else
                            sDoc = "";
                        }

                    ProprietarioStruct Linha = new ProprietarioStruct {
                        Codigo = query.Codcidadao,
                        Nome = query.Nomecidadao,
                        Tipo = query.Tipoprop,
                        Principal = Convert.ToBoolean(query.Principal),
                        CPF = sDoc,
                        RG = query.Rg
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<LogradouroStruct> Lista_Logradouro(String Filter = "") {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from l in db.Logradouro
                           select new { l.Codlogradouro, l.Endereco });
                if (!String.IsNullOrEmpty(Filter))
                    reg = reg.Where(u => u.Endereco.Contains(Filter));

                List<LogradouroStruct> Lista = new List<LogradouroStruct>();
                foreach (var query in reg) {
                    LogradouroStruct Linha = new LogradouroStruct {
                        CodLogradouro = query.Codlogradouro,
                        Endereco = query.Endereco
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<FacequadraStruct> Lista_FaceQuadra(int distrito, int setor, int quadra, int face) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from f in db.Facequadra
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into lf from l in lf.DefaultIfEmpty()
                           where f.Coddistrito == distrito && f.Codsetor == setor && f.Codquadra == quadra && f.Codface == face
                           select new FacequadraStruct { Agrupamento = f.Codagrupa, Logradouro_codigo = f.Codlogr, Logradouro_nome = l.Endereco });

                List<FacequadraStruct> Lista = new List<FacequadraStruct>();
                foreach (var query in reg) {
                    FacequadraStruct Linha = new FacequadraStruct {
                        Logradouro_codigo = query.Logradouro_codigo,
                        Logradouro_nome = query.Logradouro_nome,
                        Agrupamento = query.Agrupamento
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public bool Existe_Imovel(int nCodigo) {
            bool bRet = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Cadimob.Count(a => a.Codreduzido == nCodigo);
                if (existingReg != 0) {
                    bRet = true;
                    }
                }
            return bRet;
            }

        public int Existe_Imovel(int distrito, int setor, int quadra, int lote, int unidade, int subunidade) {
            int nRet = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                nRet = (from a in db.Cadimob where a.Distrito == distrito && a.Setor == setor && a.Quadra == quadra && a.Lote == lote && a.Unidade == unidade && a.Subunidade == subunidade select a.Codreduzido).FirstOrDefault();
                }
            return nRet;
            }

        public int Retorna_Imovel_Inscricao(int distrito, int setor, int quadra, int lote, int face, int unidade, int subunidade) {
            int nRet = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                nRet = (from a in db.Cadimob where a.Distrito == distrito && a.Setor == setor && a.Quadra == quadra && a.Lote == lote && a.Seq == face && a.Unidade == unidade && a.Subunidade == subunidade select a.Codreduzido).FirstOrDefault();
                }
            return nRet;
            }

        public string Retorna_Imovel_Inscricao(int Codigo) {
            string _inscricao = "";
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from a in db.Cadimob where a.Codreduzido == Codigo select a).FirstOrDefault();
                _inscricao = reg.Distrito.ToString() + "." + reg.Setor.ToString("00") + "." + reg.Quadra.ToString("0000") + "." + reg.Lote.ToString("00000") + "." + reg.Seq.ToString("00") + "." + reg.Unidade.ToString("00") + "." + reg.Subunidade.ToString("000");
                }
            return _inscricao;
            }

        public EnderecoStruct Dados_Endereco(int Codigo, TipoEndereco Tipo) {
            EnderecoStruct regEnd = new EnderecoStruct();
            using (GTI_Context db = new GTI_Context(_connection)) {
                if (Tipo == TipoEndereco.Local) {
                    var reg = (from i in db.Cadimob
                               join b in db.Bairro on i.Li_codbairro equals b.Codbairro into ib from b in ib.DefaultIfEmpty()
                               join fq in db.Facequadra on new { p1 = i.Distrito, p2 = i.Setor, p3 = i.Quadra, p4 = i.Seq } equals new { p1 = fq.Coddistrito, p2 = fq.Codsetor, p3 = fq.Codquadra, p4 = fq.Codface } into ifq from fq in ifq.DefaultIfEmpty()
                               join l in db.Logradouro on fq.Codlogr equals l.Codlogradouro into lfq from l in lfq.DefaultIfEmpty()
                               where i.Codreduzido == Codigo && b.Siglauf == "SP" && b.Codcidade == 413
                               select new {
                                   i.Li_num, i.Li_codbairro, b.Descbairro, fq.Codlogr, l.Endereco, i.Li_compl, l.Endereco_resumido
                                   }).FirstOrDefault();
                    if (reg == null)
                        return regEnd;
                    else {
                        regEnd.CodigoBairro = reg.Li_codbairro;
                        regEnd.NomeBairro = reg.Descbairro.ToString();
                        regEnd.CodigoCidade = 413;
                        regEnd.NomeCidade = "JABOTICABAL";
                        regEnd.UF = "SP";
                        regEnd.CodLogradouro = reg.Codlogr;
                        regEnd.Endereco = reg.Endereco ?? "";
                        regEnd.Endereco_Abreviado = reg.Endereco_resumido ?? "";
                        regEnd.Numero = reg.Li_num;
                        regEnd.Complemento = reg.Li_compl ?? "";
                        regEnd.CodigoBairro = reg.Li_codbairro;
                        regEnd.NomeBairro = reg.Descbairro;
                        Endereco_Data clsCep = new Endereco_Data(_connection);
                        regEnd.Cep = clsCep.RetornaCep(Convert.ToInt32(reg.Codlogr), Convert.ToInt16(reg.Li_num)) == 0 ? "14870000" : clsCep.RetornaCep(Convert.ToInt32(reg.Codlogr), Convert.ToInt16(reg.Li_num)).ToString("0000");
                        }
                    } else if (Tipo == TipoEndereco.Entrega) {
                    var reg = (from ee in db.Endentrega
                               join b in db.Bairro on new { p1 = ee.Ee_uf, p2 = ee.Ee_cidade, p3 = ee.Ee_bairro } equals new { p1 = b.Siglauf, p2 = (short?)b.Codcidade, p3 = (short?)b.Codbairro } into eeb from b in eeb.DefaultIfEmpty()
                               join c in db.Cidade on new { p1 = ee.Ee_uf, p2 = ee.Ee_cidade } equals new { p1 = c.Siglauf, p2 = (short?)c.Codcidade } into eec from c in eec.DefaultIfEmpty()
                               join l in db.Logradouro on ee.Ee_codlog equals l.Codlogradouro into lee from l in lee.DefaultIfEmpty()
                               where ee.Codreduzido == Codigo
                               select new {
                                   ee.Ee_numimovel, ee.Ee_bairro, b.Descbairro, c.Desccidade, ee.Ee_uf, ee.Ee_cidade, ee.Ee_codlog, ee.Ee_nomelog, l.Endereco, ee.Ee_complemento, l.Endereco_resumido, ee.Ee_cep
                                   }).FirstOrDefault();
                    if (reg == null)
                        return regEnd;
                    else {
                        regEnd.CodigoBairro = reg.Ee_bairro;
                        regEnd.NomeBairro = reg.Descbairro == null ? "" : reg.Descbairro.ToString();
                        regEnd.CodigoCidade = reg.Ee_cidade == null ? 0 : reg.Ee_cidade;
                        regEnd.NomeCidade = reg.Descbairro == null ? "" : reg.Desccidade;
                        regEnd.UF = "SP";
                        regEnd.CodLogradouro = reg.Ee_codlog;
                        regEnd.Endereco = reg.Ee_nomelog ?? "";
                        regEnd.Endereco_Abreviado = reg.Endereco_resumido ?? "";
                        if (!String.IsNullOrEmpty(reg.Endereco))
                            regEnd.Endereco = reg.Endereco.ToString();
                        regEnd.Numero = reg.Ee_numimovel;
                        regEnd.Complemento = reg.Ee_complemento ?? "";
                        regEnd.CodigoBairro = reg.Ee_bairro;
                        regEnd.NomeBairro = reg.Descbairro;
                        Endereco_Data clsCep = new Endereco_Data(_connection);
                        if (regEnd.CodLogradouro == 0)
                            regEnd.Cep = dalCore.RetornaNumero(reg.Ee_cep) == "" ? "00000000" : Convert.ToInt32(dalCore.RetornaNumero(reg.Ee_cep)).ToString("00000000");
                        else
                            regEnd.Cep = clsCep.RetornaCep(Convert.ToInt32(regEnd.CodLogradouro), Convert.ToInt16(reg.Ee_numimovel)) == 0 ? "00000000" : clsCep.RetornaCep(Convert.ToInt32(regEnd.CodLogradouro), Convert.ToInt16(reg.Ee_numimovel)).ToString("0000");
                        }
                    } else if (Tipo == TipoEndereco.Proprietario) {
                    List<ProprietarioStruct> _lista_proprietario = Lista_Proprietario(Codigo, true);
                    int _codigo_proprietario = _lista_proprietario[0].Codigo;
                    Cidadao_Data clsCidadao = new Cidadao_Data(_connection);
                    CidadaoStruct _cidadao = clsCidadao.LoadReg(_codigo_proprietario);
                    if (_cidadao.EtiquetaC == "S") {
                        regEnd.Endereco = _cidadao.EnderecoC;
                        regEnd.Endereco_Abreviado = _cidadao.EnderecoC;
                        regEnd.Numero = _cidadao.NumeroC;
                        regEnd.Complemento = _cidadao.ComplementoC;
                        regEnd.CodigoBairro = _cidadao.CodigoBairroC;
                        regEnd.NomeBairro = _cidadao.NomeBairroC;
                        regEnd.CodigoCidade = _cidadao.CodigoCidadeC;
                        regEnd.NomeCidade = _cidadao.NomeCidadeC;
                        regEnd.UF = _cidadao.UfC;
                        regEnd.Cep = _cidadao.CepC.ToString();
                        } else {
                        regEnd.Endereco = _cidadao.EnderecoR;
                        regEnd.Endereco_Abreviado = _cidadao.EnderecoR;
                        regEnd.Numero = _cidadao.NumeroR;
                        regEnd.Complemento = _cidadao.ComplementoR;
                        regEnd.CodigoBairro = _cidadao.CodigoBairroR;
                        regEnd.NomeBairro = _cidadao.NomeBairroR;
                        regEnd.CodigoCidade = _cidadao.CodigoCidadeR;
                        regEnd.NomeCidade = _cidadao.NomeCidadeR;
                        regEnd.UF = _cidadao.UfR;
                        regEnd.Cep = _cidadao.CepR.ToString();
                        }
                    }
                }

            return regEnd;
            }

        public List<Categprop> Lista_Categoria_Propriedade() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from c in db.Categprop where c.Codcategprop != 999 orderby c.Desccategprop select new { c.Codcategprop, c.Desccategprop }).ToList();
                List<Categprop> Lista = new List<Categprop>();
                foreach (var item in reg) {
                    Categprop Linha = new Categprop {
                        Codcategprop = item.Codcategprop,
                        Desccategprop = item.Desccategprop
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Topografia> Lista_Topografia() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Topografia where t.Codtopografia != 999 orderby t.Desctopografia select new { t.Codtopografia, t.Desctopografia }).ToList();
                List<Topografia> Lista = new List<Topografia>();
                foreach (var item in reg) {
                    Topografia Linha = new Topografia {
                        Codtopografia = item.Codtopografia,
                        Desctopografia = item.Desctopografia
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Situacao> Lista_Situacao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Situacao where t.Codsituacao != 999 orderby t.Descsituacao select new { t.Codsituacao, t.Descsituacao }).ToList();
                List<Situacao> Lista = new List<Situacao>();
                foreach (var item in reg) {
                    Situacao Linha = new Situacao {
                        Codsituacao = item.Codsituacao,
                        Descsituacao = item.Descsituacao
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Benfeitoria> Lista_Benfeitoria() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Benfeitoria where t.Codbenfeitoria != 999 orderby t.Descbenfeitoria select new { t.Codbenfeitoria, t.Descbenfeitoria }).ToList();
                List<Benfeitoria> Lista = new List<Benfeitoria>();
                foreach (var item in reg) {
                    Benfeitoria Linha = new Benfeitoria {
                        Codbenfeitoria = item.Codbenfeitoria,
                        Descbenfeitoria = item.Descbenfeitoria
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Pedologia> Lista_Pedologia() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Pedologia where t.Codpedologia != 999 orderby t.Descpedologia select new { t.Codpedologia, t.Descpedologia }).ToList();
                List<Pedologia> Lista = new List<Pedologia>();
                foreach (var item in reg) {
                    Pedologia Linha = new Pedologia {
                        Codpedologia = item.Codpedologia,
                        Descpedologia = item.Descpedologia
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Usoterreno> Lista_Uso_Terreno() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Usoterreno where t.Codusoterreno != 999 orderby t.Descusoterreno select new { t.Codusoterreno, t.Descusoterreno }).ToList();
                List<Usoterreno> Lista = new List<Usoterreno>();
                foreach (var item in reg) {
                    Usoterreno Linha = new Usoterreno {
                        Codusoterreno = item.Codusoterreno,
                        Descusoterreno = item.Descusoterreno
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Usoconstr> Lista_Uso_Construcao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Usoconstr where t.Codusoconstr != 999 orderby t.Descusoconstr select new { t.Codusoconstr, t.Descusoconstr }).ToList();
                List<Usoconstr> Lista = new List<Usoconstr>();
                foreach (var item in reg) {
                    Usoconstr Linha = new Usoconstr {
                        Codusoconstr = item.Codusoconstr,
                        Descusoconstr = item.Descusoconstr
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Categconstr> Lista_Categoria_Construcao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from c in db.Categconstr where c.Codcategconstr != 999 orderby c.Desccategconstr select new { c.Codcategconstr, c.Desccategconstr }).ToList();
                List<Categconstr> Lista = new List<Categconstr>();
                foreach (var item in reg) {
                    Categconstr Linha = new Categconstr {
                        Codcategconstr = item.Codcategconstr,
                        Desccategconstr = item.Desccategconstr
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Tipoconstr> Lista_Tipo_Construcao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from c in db.Tipoconstr where c.Codtipoconstr != 999 orderby c.Desctipoconstr select new { c.Codtipoconstr, c.Desctipoconstr }).ToList();
                List<Tipoconstr> Lista = new List<Tipoconstr>();
                foreach (var item in reg) {
                    Tipoconstr Linha = new Tipoconstr {
                        Codtipoconstr = item.Codtipoconstr,
                        Desctipoconstr = item.Desctipoconstr
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Testada> Lista_Testada(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Testada where t.Codreduzido == Codigo orderby t.Numface select t).ToList();
                List<Testada> Lista = new List<Testada>();
                foreach (var item in reg) {
                    Testada Linha = new Testada {
                        Codreduzido = item.Codreduzido,
                        Numface = item.Numface,
                        Areatestada = item.Areatestada
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public Testada Retorna_Testada_principal(int Codigo, int Face) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Testada reg = (from t in db.Testada where t.Codreduzido == Codigo && t.Numface == Face orderby t.Numface select t).FirstOrDefault();
                return reg;
                }
            }

        public List<AreaStruct> Lista_Area(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from a in db.Areas
                           join c in db.Categconstr on a.Catconstr equals c.Codcategconstr into ac from c in ac.DefaultIfEmpty()
                           join t in db.Tipoconstr on a.Tipoconstr equals t.Codtipoconstr into at from t in at.DefaultIfEmpty()
                           join u in db.Usoconstr on a.Usoconstr equals u.Codusoconstr into au from u in au.DefaultIfEmpty()
                           where a.Codreduzido == Codigo orderby a.Seqarea select new AreaStruct { Codigo = a.Codreduzido, Data_Aprovacao = a.Dataaprova, Area = (decimal)a.Areaconstr, Categoria_Codigo = a.Catconstr,
                               Tipo_Nome = t.Desctipoconstr, Categoria_Nome = c.Desccategconstr, Data_Processo = a.Dataprocesso, Numero_Processo = a.Numprocesso, Pavimentos = a.Qtdepav, Seq = a.Seqarea, Tipo_Area = a.Tipoarea, Tipo_Codigo = a.Tipoconstr,
                               Uso_Codigo = a.Usoconstr, Uso_Nome = u.Descusoconstr }).ToList();
                List<AreaStruct> Lista = new List<AreaStruct>();
                foreach (var item in reg) {
                    AreaStruct Linha = new AreaStruct {
                        Codigo = item.Codigo,
                        Seq = item.Seq,
                        Area = item.Area,
                        Categoria_Codigo = item.Categoria_Codigo,
                        Categoria_Nome = item.Categoria_Nome,
                        Uso_Codigo = item.Uso_Codigo,
                        Uso_Nome = item.Uso_Nome,
                        Tipo_Codigo = item.Tipo_Codigo,
                        Tipo_Nome = item.Tipo_Nome,
                        Pavimentos = item.Pavimentos,
                        Numero_Processo = item.Numero_Processo,
                        Data_Processo = item.Data_Processo,
                        Data_Aprovacao = item.Data_Aprovacao,
                        Tipo_Area = item.Tipo_Area
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public ImovelStruct Inscricao_imovel(int Logradouro, short Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Cadimob
                           join f in db.Facequadra on new { p1 = i.Distrito, p2 = i.Setor, p3 = i.Quadra, p4 = i.Seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into fi from f in fi.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into lf from l in lf.DefaultIfEmpty()
                           where f.Codlogr == Logradouro && i.Li_num == Numero
                           select new ImovelStruct { Codigo = i.Codreduzido,
                               Distrito = i.Distrito, Setor = i.Setor, Quadra = i.Quadra, Lote = i.Lote, Seq = i.Seq, Unidade = i.Unidade, SubUnidade = i.Subunidade
                               }).FirstOrDefault();

                ImovelStruct row = new ImovelStruct();
                if (reg == null)
                    return row;
                row.Codigo = reg.Codigo;
                row.Distrito = reg.Distrito;
                row.Setor = reg.Setor;
                row.Quadra = reg.Quadra;
                row.Lote = reg.Lote;
                row.Seq = reg.Seq;
                row.Unidade = reg.Unidade;
                row.SubUnidade = reg.SubUnidade;

                return row;
                }
            }

        public List<int> Lista_Imovel_Ativo() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from e in db.Cadimob where e.Inativo == false orderby e.Codreduzido select e.Codreduzido).ToList();
                return Sql;
                }
            }

        public List<Condominio> Lista_Condominio() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from e in db.Condominio where e.Cd_codigo > 0 orderby e.Cd_nomecond select e).ToList();
                return Sql;
                }
            }

        public CondominioStruct Dados_Condominio(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from m in db.Condominio
                           join b in db.Bairro on m.Cd_codbairro equals b.Codbairro into mb from b in mb.DefaultIfEmpty()
                           join c in db.Cidade on new { p1 = (short)m.Cd_codcidade, p2 = m.Cd_uf } equals new { p1 = c.Codcidade, p2 = c.Siglauf } into mc from c in mc.DefaultIfEmpty()
                           join f in db.Facequadra on new { p1 = m.Cd_distrito, p2 = m.Cd_setor, p3 = m.Cd_quadra, p4 = m.Cd_seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into mf from f in mf.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into lm from l in lm.DefaultIfEmpty()
                           join bf in db.Benfeitoria on m.Cd_codbenf equals bf.Codbenfeitoria into ib from bf in ib.DefaultIfEmpty()
                           join pd in db.Pedologia on m.Cd_codpedol equals pd.Codpedologia into ip from pd in ip.DefaultIfEmpty()
                           join tp in db.Topografia on m.Cd_codtopog equals tp.Codtopografia into it from tp in it.DefaultIfEmpty()
                           join st in db.Situacao on m.Cd_codsituacao equals st.Codsituacao into ist from st in ist.DefaultIfEmpty()
                           join cp in db.Categprop on m.Cd_codcategprop equals cp.Codcategprop into icp from cp in icp.DefaultIfEmpty()
                           join u in db.Usoterreno on m.Cd_codusoterreno equals u.Codusoterreno into iu from u in iu.DefaultIfEmpty()
                           where m.Cd_codigo == Codigo
                           select new CondominioStruct {
                               Codigo = m.Cd_codigo, Nome = m.Cd_nomecond, Codigo_Logradouro = f.Codlogr, Nome_Logradouro = l.Endereco, Numero = m.Cd_num, Cep = m.Cd_cep,
                               Complemento = m.Cd_compl, Codigo_Bairro = m.Cd_codbairro, Nome_Bairro = b.Descbairro, Area_Construida = m.Cd_areatotconstr, Area_Terreno = m.Cd_areaterreno,
                               Benfeitoria = m.Cd_codbenf, Categoria = m.Cd_codcategprop, Fracao_Ideal = m.Cd_fracao, Pedologia = m.Cd_codpedol, Lote_Original = m.Cd_lotes, Quadra_Original = m.Cd_quadras,
                               Situacao = m.Cd_codsituacao, Topografia = m.Cd_codtopog, Uso_terreno = m.Cd_codusoterreno, Codigo_Proprietario = m.Cd_prop, Qtde_Unidade = m.Cd_numunid, Distrito = m.Cd_distrito,
                               Setor = m.Cd_setor, Quadra = m.Cd_quadra, Lote = m.Cd_lote, Seq = m.Cd_seq, Benfeitoria_Nome = bf.Descbenfeitoria, Categoria_Nome = cp.Desccategprop, Pedologia_Nome = pd.Descpedologia,
                               Situacao_Nome = st.Descsituacao, Topografia_Nome = tp.Desctopografia, Uso_terreno_Nome = u.Descusoterreno
                               }).FirstOrDefault();

                CondominioStruct row = new CondominioStruct();
                if (reg == null)
                    return row;
                row.Codigo = reg.Codigo;
                row.Nome = reg.Nome;
                row.Codigo_Logradouro = reg.Codigo_Logradouro;
                row.Nome_Logradouro = reg.Nome_Logradouro;
                row.Numero = reg.Numero;
                row.Complemento = reg.Complemento;
                row.Codigo_Bairro = reg.Codigo_Bairro;
                row.Nome_Bairro = reg.Nome_Bairro;
                row.Distrito = reg.Distrito;
                row.Setor = reg.Setor;
                row.Quadra = reg.Quadra;
                row.Lote = reg.Lote;
                row.Seq = reg.Seq;
                row.Area_Construida = reg.Area_Construida;
                row.Area_Terreno = reg.Area_Terreno;
                row.Benfeitoria = reg.Benfeitoria;
                row.Benfeitoria_Nome = reg.Benfeitoria_Nome;
                row.Categoria = reg.Categoria;
                row.Categoria_Nome = reg.Categoria_Nome;
                row.Cep = reg.Cep;
                row.Codigo_Proprietario = reg.Codigo_Proprietario;
                row.Fracao_Ideal = reg.Fracao_Ideal;
                row.Lote_Original = reg.Lote_Original;
                row.Pedologia = reg.Pedologia;
                row.Pedologia_Nome = reg.Pedologia_Nome;
                row.Qtde_Unidade = reg.Qtde_Unidade;
                row.Quadra_Original = reg.Quadra_Original;
                row.Situacao = reg.Situacao;
                row.Situacao_Nome = reg.Situacao_Nome;
                row.Topografia = reg.Topografia;
                row.Topografia_Nome = reg.Topografia_Nome;
                row.Uso_terreno = reg.Uso_terreno;
                row.Uso_terreno_Nome = reg.Uso_terreno_Nome;

                if (reg.Codigo_Logradouro > 0) {
                    Endereco_Data Cep_Class = new Endereco_Data(_connection);
                    int nCep = Cep_Class.RetornaCep((int)reg.Codigo_Logradouro, (short)reg.Numero);
                    row.Cep = nCep == 0 ? "00000000" : nCep.ToString("0000");
                    }

                return row;
                }
            }

        public List<AreaStruct> Lista_Area_Condominio(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from a in db.CondominioArea
                           join c in db.Categconstr on a.Catconstr equals c.Codcategconstr into ac from c in ac.DefaultIfEmpty()
                           join t in db.Tipoconstr on a.Tipoconstr equals t.Codtipoconstr into at from t in at.DefaultIfEmpty()
                           join u in db.Usoconstr on a.Usoconstr equals u.Codusoconstr into au from u in au.DefaultIfEmpty()
                           where a.Codcondominio == Codigo orderby a.Seqarea select new AreaStruct {
                               Codigo = a.Codcondominio, Data_Aprovacao = a.Dataaprova, Area = a.Areaconstr, Categoria_Codigo = a.Catconstr, Tipo_Nome = t.Desctipoconstr, Categoria_Nome = c.Desccategconstr,
                               Data_Processo = a.Dataprocesso, Numero_Processo = a.Numprocesso, Pavimentos = a.Qtdepav, Seq = a.Seqarea, Tipo_Area = a.Tipoarea, Tipo_Codigo = a.Tipoconstr,
                               Uso_Codigo = a.Usoconstr, Uso_Nome = u.Descusoconstr
                               }).ToList();
                List<AreaStruct> Lista = new List<AreaStruct>();
                foreach (var item in reg) {
                    AreaStruct Linha = new AreaStruct {
                        Codigo = item.Codigo,
                        Seq = item.Seq,
                        Area = item.Area,
                        Categoria_Codigo = item.Categoria_Codigo,
                        Categoria_Nome = item.Categoria_Nome,
                        Uso_Codigo = item.Uso_Codigo,
                        Uso_Nome = item.Uso_Nome,
                        Tipo_Codigo = item.Tipo_Codigo,
                        Tipo_Nome = item.Tipo_Nome,
                        Pavimentos = item.Pavimentos,
                        Numero_Processo = item.Numero_Processo,
                        Data_Processo = item.Data_Processo,
                        Data_Aprovacao = item.Data_Aprovacao,
                        Tipo_Area = item.Tipo_Area
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Testadacondominio> Lista_Testada_Condominio(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Testadacondominio where t.Codcond == Codigo orderby t.Numface select t).ToList();
                List<Testadacondominio> Lista = new List<Testadacondominio>();
                foreach (var item in reg) {
                    Testadacondominio Linha = new Testadacondominio {
                        Codcond = item.Codcond,
                        Numface = item.Numface,
                        Areatestada = item.Areatestada
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<Condominiounidade> Lista_Unidade_Condominio(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.CondominioUnidade where t.Cd_codigo == Codigo orderby new { t.Cd_unidade, t.Cd_subunidades } select t).ToList();
                List<Condominiounidade> Lista = new List<Condominiounidade>();
                foreach (var item in reg) {
                    Condominiounidade Linha = new Condominiounidade {
                        Cd_codigo = item.Cd_codigo,
                        Cd_unidade = item.Cd_unidade,
                        Cd_subunidades = item.Cd_subunidades
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public List<ImovelStruct> Lista_Imovel(ImovelStruct Reg, ImovelStruct OrderByField) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cadimob
                           join f in db.Facequadra on new { p1 = c.Distrito, p2 = c.Setor, p3 = c.Quadra, p4 = c.Seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into fc from f in fc.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into fl from l in fl.DefaultIfEmpty()
                           join b in db.Bairro on new { p1 = c.Li_uf, p2 = (short)c.Li_codcidade, p3 = (short)c.Li_codbairro } equals new { p1 = b.Siglauf, p2 = b.Codcidade, p3 = b.Codbairro } into cb from b in cb.DefaultIfEmpty()
                           join o in db.Condominio on c.Codcondominio equals o.Cd_codigo into co from o in co.DefaultIfEmpty()
                           join p in db.Proprietario on c.Codreduzido equals p.Codreduzido into pc from p in pc.DefaultIfEmpty()
                           join i in db.Cidadao on p.Codcidadao equals i.Codcidadao into ip from i in ip.DefaultIfEmpty()
                           select new ImovelStruct {
                               Codigo = c.Codreduzido, Distrito = c.Distrito, Setor = c.Setor, Quadra = c.Quadra, Lote = c.Lote, Seq = c.Seq, Unidade = c.Unidade,
                               SubUnidade = c.Subunidade, Proprietario_Codigo = p.Codcidadao, Proprietario_Nome = i.Nomecidadao, Proprietario_Principal = p.Principal, CodigoLogradouro = f.Codlogr,
                               NomeLogradouro = l.Endereco, Numero = c.Li_num, CodigoCondominio = c.Codcondominio, NomeCondominio = o.Cd_nomecond, CodigoBairro = c.Li_codbairro, NomeBairro = b.Descbairro,
                               Complemento = c.Li_compl
                               });
                if (Reg.Codigo > 0)
                    Sql = Sql.Where(c => c.Codigo == Reg.Codigo);
                if (Reg.Proprietario_Codigo > 0)
                    Sql = Sql.Where(c => c.Proprietario_Codigo == Reg.Proprietario_Codigo);
                if (Convert.ToBoolean(Reg.Proprietario_Principal))
                    Sql = Sql.Where(c => c.Proprietario_Principal == Reg.Proprietario_Principal);
                if (Reg.Distrito > 0)
                    Sql = Sql.Where(c => c.Distrito == Reg.Distrito);
                if (Reg.Setor > 0)
                    Sql = Sql.Where(c => c.Setor == Reg.Setor);
                if (Reg.Quadra > 0)
                    Sql = Sql.Where(c => c.Quadra == Reg.Quadra);
                if (Reg.Lote > 0)
                    Sql = Sql.Where(c => c.Lote == Reg.Lote);
                if (Reg.CodigoCondominio > 0)
                    Sql = Sql.Where(c => c.CodigoCondominio == Reg.CodigoCondominio);
                if (Reg.CodigoLogradouro > 0)
                    Sql = Sql.Where(c => c.CodigoLogradouro == Reg.CodigoLogradouro);
                if (Reg.CodigoBairro > 0)
                    Sql = Sql.Where(c => c.CodigoBairro == Reg.CodigoBairro);
                if (Reg.Numero > 0)
                    Sql = Sql.Where(c => c.Numero == Reg.Numero);

                if (OrderByField.Codigo > 0)
                    Sql = Sql.OrderBy(c => c.Codigo);
                if (!string.IsNullOrWhiteSpace(OrderByField.Inscricao))
                    Sql = Sql.OrderBy(c => c.Inscricao);
                if (!string.IsNullOrWhiteSpace(OrderByField.Proprietario_Nome))
                    Sql = Sql.OrderBy(c => c.Proprietario_Nome);
                if (!string.IsNullOrWhiteSpace(OrderByField.NomeLogradouro))
                    Sql = Sql.OrderBy(c => c.NomeLogradouro);
                if (!string.IsNullOrWhiteSpace(OrderByField.NomeBairro))
                    Sql = Sql.OrderBy(c => c.NomeBairro);
                if (!string.IsNullOrWhiteSpace(OrderByField.NomeCondominio))
                    Sql = Sql.OrderBy(c => c.NomeCondominio);

                List<ImovelStruct> Lista = new List<ImovelStruct>();
                foreach (var item in Sql) {
                    ImovelStruct Linha = new ImovelStruct {
                        Codigo = item.Codigo,
                        Inscricao = item.Distrito.ToString() + "." + item.Setor.ToString("00") + "." + item.Quadra.ToString("0000") + "." + item.Lote.ToString("00000") + "." + item.Seq.ToString("00") + "." + item.Unidade.ToString("00") + "." + item.SubUnidade.ToString("000"),
                        Proprietario_Codigo = item.Proprietario_Codigo, Proprietario_Nome = item.Proprietario_Nome, CodigoLogradouro = item.CodigoLogradouro, NomeLogradouro = item.NomeLogradouro, Numero = item.Numero, NomeCondominio = item.NomeCondominio,
                        CodigoBairro = item.CodigoBairro, NomeBairro = item.NomeBairro, CodigoCondominio = item.CodigoCondominio, Complemento = item.Complemento, Distrito = item.Distrito, Setor = item.Setor, Quadra = item.Quadra, Lote = item.Lote, Seq = item.Seq,
                        Unidade = item.Unidade, SubUnidade = item.SubUnidade
                        };
                    Lista.Add(Linha);
                    }
                return Lista.ToList();
                }
            }

        public List<ImovelStruct> Lista_Imovel_Proprietario(string PartialName) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cadimob
                           join f in db.Facequadra on new { p1 = c.Distrito, p2 = c.Setor, p3 = c.Quadra, p4 = c.Seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into fc from f in fc.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into fl from l in fl.DefaultIfEmpty()
                           join b in db.Bairro on new { p1 = c.Li_uf, p2 = (short)c.Li_codcidade, p3 = (short)c.Li_codbairro } equals new { p1 = b.Siglauf, p2 = b.Codcidade, p3 = b.Codbairro } into cb from b in cb.DefaultIfEmpty()
                           join o in db.Condominio on c.Codcondominio equals o.Cd_codigo into co from o in co.DefaultIfEmpty()
                           join p in db.Proprietario on c.Codreduzido equals p.Codreduzido into pc from p in pc.DefaultIfEmpty()
                           join i in db.Cidadao on p.Codcidadao equals i.Codcidadao into ip from i in ip.DefaultIfEmpty()
                           select new ImovelStruct {
                               Codigo = c.Codreduzido, Distrito = c.Distrito, Setor = c.Setor, Quadra = c.Quadra, Lote = c.Lote, Seq = c.Seq, Unidade = c.Unidade,
                               SubUnidade = c.Subunidade, Proprietario_Codigo = p.Codcidadao, Proprietario_Nome = i.Nomecidadao, Proprietario_Principal = p.Principal, CodigoLogradouro = f.Codlogr,
                               NomeLogradouroAbreviado = l.Endereco_resumido, NomeLogradouro = l.Endereco, Numero = c.Li_num, CodigoCondominio = c.Codcondominio, NomeCondominio = o.Cd_nomecond, CodigoBairro = c.Li_codbairro, NomeBairro = b.Descbairro,
                               Complemento = c.Li_compl
                               });
                Sql = Sql.Where(p => p.Proprietario_Nome.Contains(PartialName));
                Sql = Sql.OrderBy(p => p.Proprietario_Nome).ThenBy(n => n.NomeLogradouro).ThenBy(o => o.Numero);

                List<ImovelStruct> Lista = new List<ImovelStruct>();
                foreach (var item in Sql) {
                    ImovelStruct Linha = new ImovelStruct {
                        Codigo = item.Codigo,
                        Inscricao = item.Distrito.ToString() + "." + item.Setor.ToString("00") + "." + item.Quadra.ToString("0000") + "." + item.Lote.ToString("00000") + "." + item.Seq.ToString("00") + "." + item.Unidade.ToString("00") + "." + item.SubUnidade.ToString("000"),
                        Proprietario_Codigo = item.Proprietario_Codigo, Proprietario_Nome = item.Proprietario_Nome, CodigoLogradouro = item.CodigoLogradouro, NomeLogradouro = item.NomeLogradouro, Numero = item.Numero, NomeCondominio = item.NomeCondominio,
                        CodigoBairro = item.CodigoBairro, NomeBairro = item.NomeBairro, CodigoCondominio = item.CodigoCondominio, Complemento = item.Complemento, Distrito = item.Distrito, Setor = item.Setor, Quadra = item.Quadra, Lote = item.Lote, Seq = item.Seq,
                        Unidade = item.Unidade, SubUnidade = item.SubUnidade, NomeLogradouroAbreviado = item.NomeLogradouroAbreviado
                        };
                    Lista.Add(Linha);
                    }
                return Lista.ToList();
                }
            }

        public List<ImovelStruct> Lista_Imovel_Endereco(string PartialName, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cadimob
                           join f in db.Facequadra on new { p1 = c.Distrito, p2 = c.Setor, p3 = c.Quadra, p4 = c.Seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into fc from f in fc.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into fl from l in fl.DefaultIfEmpty()
                           join b in db.Bairro on new { p1 = c.Li_uf, p2 = (short)c.Li_codcidade, p3 = (short)c.Li_codbairro } equals new { p1 = b.Siglauf, p2 = b.Codcidade, p3 = b.Codbairro } into cb from b in cb.DefaultIfEmpty()
                           join o in db.Condominio on c.Codcondominio equals o.Cd_codigo into co from o in co.DefaultIfEmpty()
                           join p in db.Proprietario on c.Codreduzido equals p.Codreduzido into pc from p in pc.DefaultIfEmpty()
                           join i in db.Cidadao on p.Codcidadao equals i.Codcidadao into ip from i in ip.DefaultIfEmpty()
                           select new ImovelStruct {
                               Codigo = c.Codreduzido, Distrito = c.Distrito, Setor = c.Setor, Quadra = c.Quadra, Lote = c.Lote, Seq = c.Seq, Unidade = c.Unidade,
                               SubUnidade = c.Subunidade, Proprietario_Codigo = p.Codcidadao, Proprietario_Nome = i.Nomecidadao, Proprietario_Principal = p.Principal, CodigoLogradouro = f.Codlogr,
                               NomeLogradouroAbreviado = l.Endereco_resumido, NomeLogradouro = l.Endereco, Numero = c.Li_num, CodigoCondominio = c.Codcondominio, NomeCondominio = o.Cd_nomecond, CodigoBairro = c.Li_codbairro, NomeBairro = b.Descbairro,
                               Complemento = c.Li_compl
                               });
                Sql = Sql.Where(p => p.NomeLogradouro.Contains(PartialName));
                if (Numero > 0)
                    Sql = Sql.Where(p => p.Numero == Numero);
                Sql = Sql.OrderBy(p => p.NomeLogradouro).ThenBy(n => n.Numero);

                List<ImovelStruct> Lista = new List<ImovelStruct>();
                foreach (var item in Sql) {
                    ImovelStruct Linha = new ImovelStruct {
                        Codigo = item.Codigo,
                        Inscricao = item.Distrito.ToString() + "." + item.Setor.ToString("00") + "." + item.Quadra.ToString("0000") + "." + item.Lote.ToString("00000") + "." + item.Seq.ToString("00") + "." + item.Unidade.ToString("00") + "." + item.SubUnidade.ToString("000"),
                        Proprietario_Codigo = item.Proprietario_Codigo, Proprietario_Nome = item.Proprietario_Nome, CodigoLogradouro = item.CodigoLogradouro, NomeLogradouro = item.NomeLogradouro, Numero = item.Numero, NomeCondominio = item.NomeCondominio,
                        CodigoBairro = item.CodigoBairro, NomeBairro = item.NomeBairro, CodigoCondominio = item.CodigoCondominio, Complemento = item.Complemento, Distrito = item.Distrito, Setor = item.Setor, Quadra = item.Quadra, Lote = item.Lote, Seq = item.Seq,
                        Unidade = item.Unidade, SubUnidade = item.SubUnidade, NomeLogradouroAbreviado = item.NomeLogradouroAbreviado
                        };
                    Lista.Add(Linha);
                    }
                return Lista.ToList();
                }
            }

        public List<ImovelStruct> Lista_Imovel(int Codigo, int Distrito, int Setor, int Quadra, int Lote, int Face, int Unidade, int SubUnidade, string PartialName, string PartialEndereco, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cadimob
                           join f in db.Facequadra on new { p1 = c.Distrito, p2 = c.Setor, p3 = c.Quadra, p4 = c.Seq } equals new { p1 = f.Coddistrito, p2 = f.Codsetor, p3 = f.Codquadra, p4 = f.Codface } into fc from f in fc.DefaultIfEmpty()
                           join l in db.Logradouro on f.Codlogr equals l.Codlogradouro into fl from l in fl.DefaultIfEmpty()
                           join b in db.Bairro on new { p1 = c.Li_uf, p2 = (short)c.Li_codcidade, p3 = (short)c.Li_codbairro } equals new { p1 = b.Siglauf, p2 = b.Codcidade, p3 = b.Codbairro } into cb from b in cb.DefaultIfEmpty()
                           join o in db.Condominio on c.Codcondominio equals o.Cd_codigo into co from o in co.DefaultIfEmpty()
                           join p in db.Proprietario on c.Codreduzido equals p.Codreduzido into pc from p in pc.DefaultIfEmpty()
                           join i in db.Cidadao on p.Codcidadao equals i.Codcidadao into ip from i in ip.DefaultIfEmpty()
                           select new ImovelStruct {
                               Codigo = c.Codreduzido, Distrito = c.Distrito, Setor = c.Setor, Quadra = c.Quadra, Lote = c.Lote, Seq = c.Seq, Unidade = c.Unidade,
                               SubUnidade = c.Subunidade, Proprietario_Codigo = p.Codcidadao, Proprietario_Nome = i.Nomecidadao, Proprietario_Principal = p.Principal, CodigoLogradouro = f.Codlogr,
                               NomeLogradouroAbreviado = l.Endereco_resumido, NomeLogradouro = l.Endereco, Numero = c.Li_num, CodigoCondominio = c.Codcondominio, NomeCondominio = o.Cd_nomecond, CodigoBairro = c.Li_codbairro, NomeBairro = b.Descbairro,
                               Complemento = c.Li_compl
                               });
                if (Codigo > 0)
                    Sql = Sql.Where(p => p.Codigo == Codigo);
                if (Distrito > 0)
                    Sql = Sql.Where(p => p.Distrito == Distrito && p.Setor == Setor && p.Quadra == Quadra && p.Lote == Lote && p.Seq == Face && p.Unidade == Unidade && p.SubUnidade == SubUnidade);
                if (!string.IsNullOrEmpty(PartialName))
                    Sql = Sql.Where(p => p.Proprietario_Nome.Contains(PartialName));
                if (!string.IsNullOrEmpty(PartialEndereco))
                    Sql = Sql.Where(p => p.NomeLogradouro.Contains(PartialEndereco));
                if (Numero > 0)
                    Sql = Sql.Where(p => p.Numero == Numero);
                Sql = Sql.OrderBy(p => p.Codigo);

                List<ImovelStruct> Lista = new List<ImovelStruct>();
                foreach (var item in Sql) {
                    ImovelStruct Linha = new ImovelStruct {
                        Codigo = item.Codigo,
                        Inscricao = item.Distrito.ToString() + "." + item.Setor.ToString("00") + "." + item.Quadra.ToString("0000") + "." + item.Lote.ToString("00000") + "." + item.Seq.ToString("00") + "." + item.Unidade.ToString("00") + "." + item.SubUnidade.ToString("000"),
                        Proprietario_Codigo = item.Proprietario_Codigo, Proprietario_Nome = item.Proprietario_Nome, CodigoLogradouro = item.CodigoLogradouro, NomeLogradouro = item.NomeLogradouro, Numero = item.Numero, NomeCondominio = item.NomeCondominio,
                        CodigoBairro = item.CodigoBairro, NomeBairro = item.NomeBairro, CodigoCondominio = item.CodigoCondominio, Complemento = item.Complemento, Distrito = item.Distrito, Setor = item.Setor, Quadra = item.Quadra, Lote = item.Lote, Seq = item.Seq,
                        Unidade = item.Unidade, SubUnidade = item.SubUnidade, NomeLogradouroAbreviado = item.NomeLogradouroAbreviado
                        };
                    Lista.Add(Linha);
                    }
                return Lista.ToList();
                }
            }

        public Laseriptu Dados_IPTU(int Codigo, int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from i in db.Laser_iptu where i.Ano == Ano && i.Codreduzido == Codigo select i).FirstOrDefault();
                return Sql;
                }
            }

        public List<Laseriptu> Dados_IPTU(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from i in db.Laser_iptu where i.Codreduzido == Codigo orderby i.Ano select i).ToList();
                return Sql;
                }
            }

        public Laseriptu_ext Dados_IPTU_Ext(int Codigo, int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from i in db.Laser_iptu_ext where i.Ano == Ano && i.Codreduzido == Codigo select i).FirstOrDefault();
                return Sql;
                }
            }

        public bool Verifica_Imunidade(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Cadimob Sql = (from c in db.Cadimob where c.Codreduzido == Codigo select c).FirstOrDefault();
                if (Sql.Imune == null)
                    return false;
                else
                    return Convert.ToBoolean(Sql.Imune);
                }
            }

        public List<IsencaoStruct> Lista_Imovel_Isencao(int Codigo, int Ano = 0) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processo_Data processo_Class = new Processo_Data(_connection);

                var reg = (from t in db.Isencao where t.Codreduzido == Codigo orderby t.Anoisencao select t).ToList();
                if (Ano > 0)
                    reg = reg.Where(t => t.Anoisencao == Ano).ToList();
                List<IsencaoStruct> Lista = new List<IsencaoStruct>();
                foreach (var item in reg) {
                    IsencaoStruct Linha = new IsencaoStruct {
                        Codreduzido = item.Codreduzido,
                        Anoisencao = item.Anoisencao,
                        Anoproc = item.Anoproc,
                        Codisencao = item.Codisencao,
                        dataaltera = item.dataaltera,
                        dataprocesso = processo_Class.Dados_Processo((short)item.Anoproc, (int)item.Numproc).DataEntrada,
                        Filantropico = item.Filantropico,
                        Motivo = item.Motivo,
                        Numproc = item.Numproc,
                        Numprocesso = item.Numprocesso,
                        Percisencao = item.Percisencao,
                        Periodo = item.Periodo,
                        Userid = item.Userid
                        };
                    Lista.Add(Linha);
                    }
                return Lista;
                }
            }

        public Exception Inativar_imovel(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Cadimob i = db.Cadimob.First(x => x.Codreduzido == Codigo);
                i.Inativo = true;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public List<HistoricoStruct> Lista_Historico(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from h in db.Historico
                           join u in db.Usuario on h.Userid equals u.Id into hu from u in hu.DefaultIfEmpty()
                           where h.Codreduzido == Codigo orderby h.Seq select new HistoricoStruct {
                               Codigo = Codigo, Data = h.Datahist2, Seq = h.Seq, Descricao = h.Deschist, Usuario_Codigo = (int)h.Userid, Usuario_Nome = u.Nomecompleto
                               }).ToList();
                List<HistoricoStruct> Lista = new List<HistoricoStruct>();
                foreach (var item in reg) {
                    Lista.Add(new HistoricoStruct {
                        Codigo = item.Codigo,
                        Seq = item.Seq,
                        Data = item.Data,
                        Descricao = item.Descricao,
                        Usuario_Codigo = item.Usuario_Codigo,
                        Usuario_Nome = item.Usuario_Nome
                        });
                    }
                return Lista;
                }
            }

        public bool Existe_Face_Quadra(int Distrito, int Setor, int Quadra, int Face) {
            bool bRet = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Facequadra.Count(a => a.Coddistrito == Distrito && a.Codsetor == Setor && a.Codquadra == Quadra && a.Codface == Face);
                if (existingReg != 0) {
                    bRet = true;
                    }
                }
            return bRet;
            }

        public int Retorna_Codigo_Disponivel() {
            int maxCod = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                maxCod = (from c in db.Cadimob select c.Codreduzido).Max();
                maxCod = Convert.ToInt32(maxCod + 1);
                }
            return maxCod;
            }

        public int Retorna_Codigo_Condominio_Disponivel() {
            int maxCod = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                maxCod = (from c in db.Condominio select c.Cd_codigo).Max();
                maxCod = Convert.ToInt32(maxCod + 1);
                }
            return maxCod;
            }

        public Exception Incluir_Imovel(Cadimob reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[35];
                Parametros[0] = new SqlParameter { ParameterName = "@Cip", SqlDbType = SqlDbType.Bit, SqlValue = reg.Cip };
                Parametros[1] = new SqlParameter { ParameterName = "@Codcondominio", SqlDbType = SqlDbType.Int, SqlValue = reg.Codcondominio };
                Parametros[2] = new SqlParameter { ParameterName = "@Conjugado", SqlDbType = SqlDbType.Bit, SqlValue = reg.Conjugado };
                Parametros[3] = new SqlParameter { ParameterName = "@Datainclusao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Datainclusao };
                Parametros[4] = new SqlParameter { ParameterName = "@Dc_qtdeedif", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dc_qtdeedif };
                Parametros[5] = new SqlParameter { ParameterName = "@Distrito", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Distrito };
                Parametros[6] = new SqlParameter { ParameterName = "@Dt_areaterreno", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Dt_areaterreno };
                Parametros[7] = new SqlParameter { ParameterName = "@Dt_codbenf", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codbenf };
                Parametros[8] = new SqlParameter { ParameterName = "@Dt_codcategprop", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codcategprop };
                Parametros[9] = new SqlParameter { ParameterName = "@Dt_codpedol", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codpedol };
                Parametros[10] = new SqlParameter { ParameterName = "@Dt_codsituacao", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codsituacao };
                Parametros[11] = new SqlParameter { ParameterName = "@Dt_codtopog", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codtopog };
                Parametros[12] = new SqlParameter { ParameterName = "@Dt_codusoterreno", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Dt_codusoterreno };
                Parametros[13] = new SqlParameter { ParameterName = "@Dt_fracaoIdeal", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Dt_fracaoideal };
                Parametros[14] = new SqlParameter { ParameterName = "@EE_tipoend", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Ee_tipoend };
                Parametros[15] = new SqlParameter { ParameterName = "@Imune", SqlDbType = SqlDbType.Bit, SqlValue = reg.Imune };
                Parametros[16] = new SqlParameter { ParameterName = "@Inativo", SqlDbType = SqlDbType.Bit, SqlValue = reg.Inativo };
                Parametros[17] = new SqlParameter { ParameterName = "@Li_cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Li_cep };
                Parametros[18] = new SqlParameter { ParameterName = "@Li_codbairro", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Li_codbairro };
                Parametros[19] = new SqlParameter { ParameterName = "@Li_codcidade", SqlDbType = SqlDbType.Int, SqlValue = reg.Li_codcidade };
                Parametros[20] = new SqlParameter { ParameterName = "@Li_compl", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Li_compl };
                Parametros[21] = new SqlParameter { ParameterName = "@Li_lotes", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Li_lotes };
                Parametros[22] = new SqlParameter { ParameterName = "@Li_num", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Li_num };
                Parametros[23] = new SqlParameter { ParameterName = "@Li_quadras", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Li_quadras };
                Parametros[24] = new SqlParameter { ParameterName = "@Li_uf", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Li_uf };
                Parametros[25] = new SqlParameter { ParameterName = "@Lote", SqlDbType = SqlDbType.Int, SqlValue = reg.Lote };
                Parametros[26] = new SqlParameter { ParameterName = "@Nummat", SqlDbType = SqlDbType.Int, SqlValue = reg.Nummat };
                Parametros[27] = new SqlParameter { ParameterName = "@Quadra", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Quadra };
                Parametros[28] = new SqlParameter { ParameterName = "@Resideimovel", SqlDbType = SqlDbType.Bit, SqlValue = reg.Resideimovel };
                Parametros[29] = new SqlParameter { ParameterName = "@Seq", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Seq };
                Parametros[30] = new SqlParameter { ParameterName = "@Setor", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Setor };
                Parametros[31] = new SqlParameter { ParameterName = "@Subunidade", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Subunidade };
                Parametros[32] = new SqlParameter { ParameterName = "@Tipomat", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Tipomat };
                Parametros[33] = new SqlParameter { ParameterName = "@Unidade", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Unidade };
                Parametros[34] = new SqlParameter { ParameterName = "@Codreduzido", SqlDbType = SqlDbType.Int, SqlValue = reg.Codreduzido };

                db.Database.ExecuteSqlCommand("INSERT INTO cadimob(dv,cip,codcondominio,conjugado,datainclusao,dc_qtdeedif,distrito,dt_areaterreno,dt_codbenf,dt_codcategprop,dt_codpedol," +
                    "dt_codsituacao,dt_codtopog,dt_codusoterreno,dt_fracaoideal,ee_tipoend,imune,inativo,li_cep,li_codbairro,li_codcidade,li_compl,li_lotes,li_num,li_quadras,li_uf," +
                    "lote,nummat,quadra,resideimovel,seq,setor,subunidade,tipomat,unidade,codreduzido) VALUES(0,@cip, @codcondominio, @conjugado, @datainclusao, @dc_qtdeedif, @distrito, @dt_areaterreno," +
                    "@dt_codbenf,@dt_codcategprop,@dt_codpedol,@dt_codsituacao,@dt_codtopog,@dt_codusoterreno,@dt_fracaoideal,@ee_tipoend,@imune,@inativo,@li_cep,@li_codbairro,@li_codcidade," +
                    "@li_compl,@li_lotes,@li_num,@li_quadras,@li_uf,@lote,@nummat,@quadra,@resideimovel,@seq,@setor,@subunidade,@tipomat,@unidade,@codreduzido)", Parametros);

                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_Condominio(Condominio reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[26];
                Parametros[0] = new SqlParameter { ParameterName = "@cd_codigo", SqlDbType = SqlDbType.Int, SqlValue = reg.Cd_codigo };
                Parametros[1] = new SqlParameter { ParameterName = "@cd_nomecond", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_nomecond };
                Parametros[2] = new SqlParameter { ParameterName = "@cd_distrito", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_distrito };
                Parametros[3] = new SqlParameter { ParameterName = "@cd_setor", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_setor };
                Parametros[4] = new SqlParameter { ParameterName = "@cd_quadra", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_quadra };
                Parametros[5] = new SqlParameter { ParameterName = "@cd_lote", SqlDbType = SqlDbType.Int, SqlValue = reg.Cd_lote };
                Parametros[6] = new SqlParameter { ParameterName = "@cd_seq", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_seq };
                Parametros[7] = new SqlParameter { ParameterName = "@cd_num", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_num };
                Parametros[8] = new SqlParameter { ParameterName = "@cd_compl", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_compl };
                Parametros[9] = new SqlParameter { ParameterName = "@cd_uf", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_uf };
                Parametros[10] = new SqlParameter { ParameterName = "@cd_codcidade", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codcidade };
                Parametros[11] = new SqlParameter { ParameterName = "@cd_codbairro", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codbairro };
                Parametros[12] = new SqlParameter { ParameterName = "@cd_cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_cep };
                Parametros[13] = new SqlParameter { ParameterName = "@cd_quadras", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_quadras };
                Parametros[14] = new SqlParameter { ParameterName = "@cd_lotes", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cd_lotes };
                Parametros[15] = new SqlParameter { ParameterName = "@cd_areaterreno", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Cd_areaterreno };
                Parametros[16] = new SqlParameter { ParameterName = "@cd_codusoterreno", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codusoterreno };
                Parametros[17] = new SqlParameter { ParameterName = "@cd_codbenf", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codbenf };
                Parametros[18] = new SqlParameter { ParameterName = "@cd_codtopog", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codtopog };
                Parametros[19] = new SqlParameter { ParameterName = "@cd_codcategprop", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codcategprop };
                Parametros[20] = new SqlParameter { ParameterName = "@cd_codsituacao", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codsituacao };
                Parametros[21] = new SqlParameter { ParameterName = "@cd_codpedol", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_codpedol };
                Parametros[22] = new SqlParameter { ParameterName = "@cd_areatotconstr", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Cd_areatotconstr };
                Parametros[23] = new SqlParameter { ParameterName = "@cd_numunid", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Cd_numunid };
                Parametros[24] = new SqlParameter { ParameterName = "@cd_prop", SqlDbType = SqlDbType.Int, SqlValue = reg.Cd_prop };
                Parametros[25] = new SqlParameter { ParameterName = "@cd_fracao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Cd_fracao };

                db.Database.ExecuteSqlCommand("INSERT INTO condominio(cd_cogdigo,cd_nomecond,cd_distrito,cd_setor,cd_quadra,cd_lote,cd_seq,cd_num,cd_compl,cd_uf,cd_codcidade," +
                    "cd_codbairro,cd_cep,cd_quadras,cd_lotes,cd_areaterreno,cd_codusoterreno,cd_codbenf,cd_codtopog,cd_codcategprop,cd_codsituacao,cd_codpedol,cd_arratotconstr," +
                    "cd_numunid,cd_prop,cd_fracao) VALUES(@cd_codigo, @cd_nomecond, @cd_distrito, @cd_setor, @cd_quadra, @cd_lote, @cd_seq, @cd_num, @cd_compl,@cd_uf, @cd_codcidade, " +
                    "@cd_codbairro,@cd_cep,@cd_quadras,@cd_lotes,@cd_areaterreno,@cd_codusoterreno,@cd_codbenf,@cd_codtopog,@cd_codcategprop,@cd_codsituacao,@cd_codpedol,@cd_arratotconstr," +
                    "@cd_numunid,@cd_prop,@cd_fracao)", Parametros);

                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Alterar_Condominio(Condominio reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Condominio b = db.Condominio.First(i => i.Cd_codigo == reg.Cd_codigo);
                b.Cd_areaterreno = reg.Cd_areaterreno;
                b.Cd_areatotconstr = reg.Cd_areatotconstr;
                b.Cd_cep = reg.Cd_cep;
                b.Cd_codbairro = reg.Cd_codbairro;
                b.Cd_codbenf = reg.Cd_codbenf;
                b.Cd_codcategprop = reg.Cd_codcategprop;
                b.Cd_codcidade = reg.Cd_codcidade;
                b.Cd_codpedol = reg.Cd_codpedol;
                b.Cd_codsituacao = reg.Cd_codsituacao;
                b.Cd_codtopog = reg.Cd_codtopog;
                b.Cd_codusoterreno = reg.Cd_codusoterreno;
                b.Cd_compl = reg.Cd_compl;
                b.Cd_distrito = reg.Cd_distrito;
                b.Cd_fracao = reg.Cd_fracao;
                b.Cd_lote = reg.Cd_lote;
                b.Cd_lotes = reg.Cd_lotes;
                b.Cd_nomecond = reg.Cd_nomecond;
                b.Cd_num = reg.Cd_num;
                b.Cd_numunid = reg.Cd_numunid;
                b.Cd_prop = reg.Cd_prop;
                b.Cd_quadra = reg.Cd_quadra;
                b.Cd_quadras = reg.Cd_quadras;
                b.Cd_seq = reg.Cd_seq;
                b.Cd_setor = reg.Cd_setor;
                b.Cd_uf = reg.Cd_uf;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Alterar_Imovel(Cadimob reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Cadimob b = db.Cadimob.First(i => i.Codreduzido == reg.Codreduzido);
                b.Cip = reg.Cip;
                b.Codcondominio = reg.Codcondominio;
                b.Conjugado = reg.Conjugado;
                b.Dc_qtdeedif = reg.Dc_qtdeedif;
                b.Dt_areaterreno = reg.Dt_areaterreno;
                b.Dt_codbenf = reg.Dt_codbenf;
                b.Dt_codcategprop = reg.Dt_codcategprop;
                b.Dt_codpedol = reg.Dt_codpedol;
                b.Dt_codsituacao = reg.Dt_codsituacao;
                b.Dt_codtopog = reg.Dt_codtopog;
                b.Dt_codusoterreno = reg.Dt_codusoterreno;
                b.Dt_fracaoideal = reg.Dt_fracaoideal;
                b.Ee_tipoend = reg.Ee_tipoend;
                b.Imune = reg.Imune;
                b.Inativo = reg.Inativo;
                b.Li_cep = reg.Li_cep;
                b.Li_codbairro = reg.Li_codbairro;
                b.Li_codcidade = reg.Li_codcidade;
                b.Li_compl = reg.Li_compl;
                b.Li_lotes = reg.Li_lotes;
                b.Li_num = reg.Li_num;
                b.Li_quadras = reg.Li_quadras;
                b.Li_uf = reg.Li_uf;
                b.Nummat = reg.Nummat;
                b.Resideimovel = reg.Resideimovel;
                b.Tipomat = reg.Tipomat;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_Proprietario(List<Proprietario> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM proprietario WHERE Codreduzido=@Codreduzido",
                        new SqlParameter("@Codreduzido", Lista[0].Codreduzido));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Proprietario item in Lista) {
                    Proprietario reg = new Proprietario {
                        Codcidadao = item.Codcidadao,
                        Codreduzido = item.Codreduzido,
                        Tipoprop = item.Tipoprop,
                        Principal = item.Principal
                        };
                    db.Proprietario.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Testada(List<Testada> testadas) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM TESTADA WHERE Codreduzido=@Codreduzido",
                        new SqlParameter("@Codreduzido", testadas[0].Codreduzido));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Testada item in testadas) {
                    Testada reg = new Testada {
                        Codreduzido = item.Codreduzido,
                        Numface = item.Numface,
                        Areatestada = item.Areatestada
                        };
                    db.Testada.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Testada_Condominio(List<Testadacondominio> testadas) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM TESTADACONDOMINIO WHERE CODCOND=@Codreduzido",
                        new SqlParameter("@Codreduzido", testadas[0].Codcond));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Testadacondominio item in testadas) {
                    Testadacondominio reg = new Testadacondominio {
                        Codcond = item.Codcond,
                        Numface = item.Numface,
                        Areatestada = item.Areatestada
                        };
                    db.Testadacondominio.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Historico(List<Historico> historicos) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM HISTORICO WHERE Codreduzido=@Codreduzido",
                        new SqlParameter("@Codreduzido", historicos[0].Codreduzido));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Historico item in historicos) {
                    Historico reg = new Historico {
                        Codreduzido = item.Codreduzido,
                        Seq = item.Seq,
                        Datahist2 = item.Datahist2,
                        Deschist = item.Deschist,
                        Userid = item.Userid
                        };
                    db.Historico.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Historico(Historico reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Historico where c.Codreduzido == reg.Codreduzido select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Historico select c.Seq).Max() + 1;

                reg.Seq = Convert.ToInt16(maxCod);
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO historico(codreduzido,seq,deschist,datahist2,userid) VALUES(@codreduzido,@seq,@deschist,@datahist2,@userid)",
                        new SqlParameter("@codreduzido", reg.Codreduzido),
                        new SqlParameter("@seq", reg.Seq),
                        new SqlParameter("@deschist", reg.Deschist),
                        new SqlParameter("@datahist2", reg.Datahist2),
                        new SqlParameter("@userid", reg.Userid));
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }


        public Exception Incluir_Area(List<Areas> areas) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM AREAS WHERE Codreduzido=@Codreduzido",
                        new SqlParameter("@Codreduzido", areas[0].Codreduzido));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Areas item in areas) {
                    Areas reg = new Areas {
                        Codreduzido = item.Codreduzido,
                        Areaconstr = item.Areaconstr,
                        Catconstr = item.Catconstr,
                        Dataaprova = item.Dataaprova,
                        Numprocesso = item.Numprocesso,
                        Qtdepav = item.Qtdepav,
                        Seqarea = item.Seqarea,
                        Tipoarea = item.Tipoarea,
                        Tipoconstr = item.Tipoconstr,
                        Usoconstr = item.Usoconstr
                        };
                    db.Areas.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Area_Condominio(List<Condominioarea> areas) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM Condominioarea WHERE Codcondominio=@Codreduzido",
                        new SqlParameter("@Codreduzido", areas[0].Codcondominio));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Condominioarea item in areas) {
                    Condominioarea reg = new Condominioarea {
                        Codcondominio = item.Codcondominio,
                        Areaconstr = item.Areaconstr,
                        Catconstr = item.Catconstr,
                        Dataaprova = item.Dataaprova,
                        Numprocesso = item.Numprocesso,
                        Qtdepav = item.Qtdepav,
                        Seqarea = item.Seqarea,
                        Tipoarea = item.Tipoarea,
                        Tipoconstr = item.Tipoconstr,
                        Usoconstr = item.Usoconstr
                        };
                    db.CondominioArea.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public Exception Incluir_Unidade_Condominio(List<Condominiounidade> unidades) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM Condominiounidade WHERE Cd_codigo=@Codreduzido",
                        new SqlParameter("@Codreduzido", unidades[0].Cd_codigo));
                    } catch (Exception ex) {
                    return ex;
                    }
                foreach (Condominiounidade item in unidades) {
                    Condominiounidade reg = new Condominiounidade {
                        Cd_codigo = item.Cd_codigo,
                        Cd_unidade = item.Cd_unidade,
                        Cd_subunidades = item.Cd_subunidades
                        };
                    db.CondominioUnidade.Add(reg);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }
                return null;
                }
            }

        public List<int> Lista_Comunicado_Isencao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                DateTime _data_alteracao = Convert.ToDateTime("21/10/2018");
                List<int> Codigos = (from t in db.Isencao where t.dataaltera > _data_alteracao orderby t.Codreduzido select t.Codreduzido).Distinct().ToList();
                return Codigos;
                }
            }

        public Exception Insert_Comunicado_Isencao(Comunicado_isencao Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[17];
                Parametros[0] = new SqlParameter { ParameterName = "@remessa", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Remessa };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[3] = new SqlParameter { ParameterName = "@cpf_cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf_cnpj };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco };
                Parametros[5] = new SqlParameter { ParameterName = "@bairro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Bairro };
                Parametros[6] = new SqlParameter { ParameterName = "@cidade", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cidade };
                Parametros[7] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cep };
                Parametros[8] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[9] = new SqlParameter { ParameterName = "@bairro_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Bairro_entrega };
                Parametros[10] = new SqlParameter { ParameterName = "@cidade_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cidade_entrega };
                Parametros[11] = new SqlParameter { ParameterName = "@cep_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cep_entrega };
                Parametros[12] = new SqlParameter { ParameterName = "@data_documento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_documento };
                Parametros[13] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[14] = new SqlParameter { ParameterName = "@lote", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Lote };
                Parametros[15] = new SqlParameter { ParameterName = "@quadra", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Quadra };
                Parametros[16] = new SqlParameter { ParameterName = "@Cep_entrega_cod", SqlDbType = SqlDbType.Int, SqlValue = Reg.Cep_entrega_cod };

                db.Database.ExecuteSqlCommand("INSERT INTO comunicado_isencao(remessa,codigo,nome,cpf_cnpj,endereco,bairro,cidade,cep,endereco_entrega,bairro_entrega,cidade_entrega,cep_entrega," +
                    "data_documento,inscricao,lote,quadra,Cep_entrega_cod) VALUES(@remessa,@codigo,@nome,@cpf_cnpj,@endereco,@bairro,@cidade,@cep,@endereco_entrega,@bairro_entrega,@cidade_entrega," +
                    "@cep_entrega,@data_documento,@inscricao,@lote,@quadra,@Cep_entrega_cod)", Parametros);

                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public List<Foto_imovel> Lista_Foto_Imovel(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from f in db.Foto_imovel where f.Codigo == Codigo select f).ToList();
                }
            }

        public Exception Insert_Dados_Imovel(dados_imovel_web Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[38];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_certidao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_Certidao };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_certidao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_Certidao };
                Parametros[2] = new SqlParameter { ParameterName = "@controle", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Controle };
                Parametros[3] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[4] = new SqlParameter { ParameterName = "@proprietario", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Proprietario };
                Parametros[5] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[6] = new SqlParameter { ParameterName = "@ativo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ativo };
                Parametros[7] = new SqlParameter { ParameterName = "@endereco", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco };
                Parametros[8] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero };
                Parametros[9] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Complemento };
                Parametros[10] = new SqlParameter { ParameterName = "@bairro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Bairro };
                Parametros[11] = new SqlParameter { ParameterName = "@quadra", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Quadra };
                Parametros[12] = new SqlParameter { ParameterName = "@lote", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Lote };
                Parametros[13] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cep };
                Parametros[14] = new SqlParameter { ParameterName = "@areaterreno", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Areaterreno };
                Parametros[15] = new SqlParameter { ParameterName = "@fracaoideal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Fracaoideal };
                Parametros[16] = new SqlParameter { ParameterName = "@topografia", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Topografia };
                Parametros[17] = new SqlParameter { ParameterName = "@pedologia", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Pedologia };
                Parametros[18] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Situacao };
                Parametros[19] = new SqlParameter { ParameterName = "@usoterreno", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usoterreno };
                Parametros[20] = new SqlParameter { ParameterName = "@benfeitoria", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Benfeitoria };
                Parametros[21] = new SqlParameter { ParameterName = "@categoria", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Categoria };
                Parametros[22] = new SqlParameter { ParameterName = "@testada", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Testada };
                Parametros[23] = new SqlParameter { ParameterName = "@agrupamento", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Agrupamento };
                Parametros[24] = new SqlParameter { ParameterName = "@somafator", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Somafator };
                Parametros[25] = new SqlParameter { ParameterName = "@vvt", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Vvt };
                Parametros[26] = new SqlParameter { ParameterName = "@vvc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Vvc };
                Parametros[27] = new SqlParameter { ParameterName = "@vvi", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Vvi };
                Parametros[28] = new SqlParameter { ParameterName = "@iptu", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Iptu };
                Parametros[29] = new SqlParameter { ParameterName = "@areapredial", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Areapredial };
                Parametros[30] = new SqlParameter { ParameterName = "@condominio", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Condominio };
                Parametros[31] = new SqlParameter { ParameterName = "@imunidade", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imunidade };
                Parametros[32] = new SqlParameter { ParameterName = "@reside", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Reside };
                Parametros[33] = new SqlParameter { ParameterName = "@isentocip", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Isentocip };
                Parametros[34] = new SqlParameter { ParameterName = "@qtdeedif", SqlDbType = SqlDbType.Int, SqlValue = Reg.Qtdeedif };
                Parametros[35] = new SqlParameter { ParameterName = "@mt", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Mt };
                Parametros[36] = new SqlParameter { ParameterName = "@data_impressao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[37] = new SqlParameter { ParameterName = "@proprietario2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Proprietario2 };

                db.Database.ExecuteSqlCommand("INSERT INTO dados_imovel_web(ano_certidao,numero_certidao,controle,codigo,proprietario,inscricao,ativo,endereco,numero,complemento,bairro,quadra,lote," +
                    "cep,areaterreno,fracaoideal,topografia,pedologia,situacao,usoterreno,benfeitoria,categoria,testada,agrupamento,somafator,vvt,vvc,vvi,iptu,areapredial," +
                    "condominio,imunidade,reside,isentocip,qtdeedif,mt,data_impressao,proprietario2) VALUES(@ano_certidao,@numero_certidao,@controle,@codigo,@proprietario,@inscricao,@ativo,@endereco,@numero,@complemento," +
                    "@bairro,@quadra,@lote,@cep,@areaterreno,@fracaoideal,@topografia,@pedologia,@situacao,@usoterreno,@benfeitoria,@categoria,@testada,@agrupamento," +
                    "@somafator,@vvt,@vvc,@vvi,@iptu,@areapredial,@condominio,@imunidade,@reside,@isentocip,@qtdeedif,@mt,@data_impressao,@proprietario2)", Parametros);

                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public bool Existe_Imovel_Cpf(int Codigo, string Cpf) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from p in db.Proprietario
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                           where p.Codreduzido == Codigo && p.Principal == true select new { Nome = c.Nomecidadao, Cpf = c.Cpf }).FirstOrDefault();
                if (string.IsNullOrEmpty(  reg.Cpf))
                    return false;
                else {
                    string _cpf = Convert.ToInt64(dalCore.RetornaNumero( reg.Cpf)).ToString();
                    _cpf = _cpf.PadLeft(11, '0');
                    if (_cpf == Cpf)
                        return true;
                    else
                        return false;
                    }
                }
            }

        public bool Existe_Imovel_Cnpj(int Codigo, string Cnpj) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from p in db.Proprietario
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                           where p.Codreduzido == Codigo && c.Cnpj == Cnpj && p.Principal == true select c.Nomecidadao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
                }
            }

        public List<Itbi_natureza> Lista_Itbi_Natureza() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_natureza> Sql = (from t in db.Itbi_Natureza orderby t.Codigo select t).ToList();
                return Sql;
                }
            }

        public List<Itbi_financiamento> Lista_Itbi_Financiamento() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_financiamento> Sql = (from t in db.itbi_Financiamento where t.Codigo > 0 orderby t.Codigo select t).ToList();
                return Sql;
                }
            }

        public List<Uf> Lista_UF() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Uf> Sql = (from t in db.Uf orderby t.Siglauf select t).ToList();
                return Sql;
                }
            }

        public bool Existe_Itbi(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Itbi_Main
                           where i.Guid == guid select i.Guid).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
                }
            }

        public Exception Incluir_Itbi_main(Itbi_main Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[8];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[2] = new SqlParameter { ParameterName = "@imovel_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Imovel_codigo };
                if (Reg.Inscricao != null)
                    Parametros[3] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                else
                    Parametros[3] = new SqlParameter { ParameterName = "@inscricao", SqlValue = DBNull.Value };
                Parametros[4] = new SqlParameter { ParameterName = "@proprietario_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Proprietario_Codigo };
                if (Reg.Proprietario_Nome != null)
                    Parametros[5] = new SqlParameter { ParameterName = "@proprietario_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Proprietario_Nome };
                else
                    Parametros[5] = new SqlParameter { ParameterName = "@proprietario_nome", SqlValue = DBNull.Value };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[7] = new SqlParameter { ParameterName = "@situacao_itbi", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao_itbi };
                db.Database.ExecuteSqlCommand("INSERT INTO itbi_main(guid,data_cadastro,imovel_codigo,inscricao,proprietario_codigo,proprietario_nome,userid,situacao_itbi) " +
                                              " VALUES(@guid,@data_cadastro,@imovel_codigo,@inscricao,@proprietario_codigo,@proprietario_nome,@userid,@situacao_itbi)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }

                try {
                    db.Itbi_Comprador.RemoveRange(db.Itbi_Comprador.Where(i => i.Guid == Reg.Guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }

                try {
                    db.Itbi_Vendedor.RemoveRange(db.Itbi_Vendedor.Where(i => i.Guid == Reg.Guid));
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }

                return null;
                }
            }

        public Exception Alterar_Itbi_Main(Itbi_main Reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_main i = db.Itbi_Main.First(g => g.Guid == Reg.Guid);
                i.Imovel_codigo = Reg.Imovel_codigo;
                i.Inscricao = Reg.Inscricao;
                i.Proprietario_Codigo = Reg.Proprietario_Codigo;
                i.Proprietario_Nome = Reg.Proprietario_Nome;
                i.Imovel_endereco = Reg.Imovel_endereco;
                i.Imovel_numero = Reg.Imovel_numero;
                i.Imovel_complemento = Reg.Imovel_complemento;
                i.Imovel_cep = Reg.Imovel_cep;
                i.Imovel_bairro = Reg.Imovel_bairro;
                i.Imovel_Quadra = Reg.Imovel_Quadra;
                i.Imovel_Lote = Reg.Imovel_Lote;
                i.Natureza_Codigo = Reg.Natureza_Codigo;
                i.Comprador_cpf_cnpj = Reg.Comprador_cpf_cnpj;
                i.Comprador_codigo = Reg.Comprador_codigo;
                i.Comprador_nome = Reg.Comprador_nome;
                i.Comprador_logradouro_codigo = Reg.Comprador_logradouro_codigo;
                i.Comprador_logradouro_nome = Reg.Comprador_logradouro_nome;
                i.Comprador_numero = Reg.Comprador_numero;
                i.Comprador_complemento = Reg.Comprador_complemento;
                i.Comprador_cep = Reg.Comprador_cep;
                i.Comprador_bairro_codigo = Reg.Comprador_bairro_codigo;
                i.Comprador_bairro_nome = Reg.Comprador_bairro_nome;
                i.Comprador_cidade_codigo = Reg.Comprador_cidade_codigo;
                i.Comprador_cidade_nome = Reg.Comprador_cidade_nome;
                i.Comprador_uf = Reg.Comprador_uf;
                i.Comprador_telefone = Reg.Comprador_telefone;
                i.Comprador_email = Reg.Comprador_email;
                i.Tipo_Financiamento = Reg.Tipo_Financiamento;
                i.Tipo_Instrumento = Reg.Tipo_Instrumento;
                i.Data_Transacao = Reg.Data_Transacao == DateTime.MinValue ? null : Reg.Data_Transacao;
                i.Valor_Transacao = Reg.Valor_Transacao;
                i.Valor_Avaliacao = Reg.Valor_Avaliacao;
                i.Valor_Avaliacao_atual = Reg.Valor_Avaliacao_atual;
                i.Valor_Venal = Reg.Valor_Venal;
                i.Valor_guia = Reg.Valor_guia;
                i.Valor_guia_atual = Reg.Valor_guia_atual;
                i.Recursos_proprios_valor = Reg.Recursos_proprios_valor;
                i.Recursos_proprios_aliq = Reg.Recursos_proprios_aliq;
                i.Recursos_proprios_atual = Reg.Recursos_proprios_atual;
                i.Recursos_conta_valor = Reg.Recursos_conta_valor;
                i.Recursos_conta_atual = Reg.Recursos_conta_atual;
                i.Recursos_concedido_valor = Reg.Recursos_concedido_valor;
                i.Recursos_concedido_aliq = Reg.Recursos_concedido_aliq;
                i.Recursos_concedido_atual = Reg.Recursos_concedido_atual;
                i.Financiamento_valor = Reg.Financiamento_valor;
                i.Financiamento_aliq = Reg.Financiamento_aliq;
                i.Financiamento_atual = Reg.Financiamento_atual;
                i.Totalidade = Reg.Totalidade;
                i.Totalidade_Perc = Reg.Totalidade_Perc;
                i.Matricula = Reg.Matricula;
                i.Inscricao_Incra = Reg.Inscricao_Incra;
                i.Receita_Federal = Reg.Receita_Federal;
                i.Descricao_Imovel = Reg.Descricao_Imovel;
                i.Userid = Reg.Userid;
                i.Utilizar_vvt = Reg.Utilizar_vvt;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Excluir_Itbi(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_main b = db.Itbi_Main.First(i => i.Guid == Guid);
                try {
                    db.Itbi_Main.Remove(b);
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                try {
                    db.Itbi_Comprador.RemoveRange(db.Itbi_Comprador.Where(i => i.Guid == Guid));
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Excluir_Itbi_comprador(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Itbi_Comprador.RemoveRange(db.Itbi_Comprador.Where(i => i.Guid == Guid));
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Excluir_Itbi_vendedor(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Itbi_Vendedor.RemoveRange(db.Itbi_Vendedor.Where(i => i.Guid == Guid));
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Excluir_Itbi_comprador(string guid, int seq) {
            object[] Parametros = new object[2];
            using (GTI_Context db = new GTI_Context(_connection)) {
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = seq };

                db.Database.ExecuteSqlCommand("DELETE FROM itbi_comprador WHERE guid=@guid AND seq=@seq", Parametros);
                return null;
                }
            }

        public Exception Excluir_Itbi_vendedor(string guid, int seq) {
            object[] Parametros = new object[2];
            using (GTI_Context db = new GTI_Context(_connection)) {
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = seq };

                db.Database.ExecuteSqlCommand("DELETE FROM itbi_vendedor WHERE guid=@guid AND seq=@seq", Parametros);
                return null;
                }
            }

        public Exception Excluir_Itbi_anexo(string guid, int seq) {
            object[] Parametros = new object[2];
            using (GTI_Context db = new GTI_Context(_connection)) {
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = seq };

                db.Database.ExecuteSqlCommand("DELETE FROM itbi_anexo WHERE guid=@guid AND seq=@seq", Parametros);
                return null;
                }
            }

        public Itbi_main Retorna_Itbi_Main(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Main where t.Guid == Guid select t).First();
                Itbi_main itbi = new Itbi_main() {
                    Guid = Sql.Guid,
                    Data_cadastro = Sql.Data_cadastro,
                    Imovel_codigo = Sql.Imovel_codigo,
                    Inscricao = Sql.Inscricao,
                    Proprietario_Codigo = Sql.Proprietario_Codigo,
                    Proprietario_Nome = Sql.Proprietario_Nome,
                    Imovel_endereco = Sql.Imovel_endereco,
                    Imovel_numero = Sql.Imovel_numero,
                    Imovel_complemento = Sql.Imovel_complemento,
                    Imovel_cep = Sql.Imovel_cep,
                    Imovel_bairro = Sql.Imovel_bairro,
                    Imovel_Quadra = Sql.Imovel_Quadra,
                    Imovel_Lote = Sql.Imovel_Lote,
                    Natureza_Codigo = Sql.Natureza_Codigo,
                    Comprador_cpf_cnpj = Sql.Comprador_cpf_cnpj,
                    Comprador_codigo = Sql.Comprador_codigo,
                    Comprador_nome = Sql.Comprador_nome,
                    Comprador_logradouro_codigo = Sql.Comprador_logradouro_codigo,
                    Comprador_logradouro_nome = Sql.Comprador_logradouro_nome,
                    Comprador_numero = Sql.Comprador_numero,
                    Comprador_complemento = Sql.Comprador_complemento,
                    Comprador_cep = Sql.Comprador_cep,
                    Comprador_bairro_codigo = Sql.Comprador_bairro_codigo,
                    Comprador_bairro_nome = Sql.Comprador_bairro_nome,
                    Comprador_cidade_codigo = Sql.Comprador_cidade_codigo,
                    Comprador_cidade_nome = Sql.Comprador_cidade_nome,
                    Comprador_uf = Sql.Comprador_uf,
                    Comprador_telefone = Sql.Comprador_telefone,
                    Comprador_email = Sql.Comprador_email,
                    Tipo_Financiamento = Sql.Tipo_Financiamento,
                    Tipo_Instrumento = Sql.Tipo_Instrumento,
                    Data_Transacao = Sql.Data_Transacao,
                    Valor_Transacao = Sql.Valor_Transacao,
                    Valor_Avaliacao = Sql.Valor_Avaliacao,
                    Valor_Avaliacao_atual = Sql.Valor_Avaliacao_atual,
                    Valor_Venal = Sql.Valor_Venal,
                    Valor_guia = Sql.Valor_guia,
                    Valor_guia_atual = Sql.Valor_guia_atual,
                    Recursos_proprios_valor = Sql.Recursos_proprios_valor,
                    Recursos_proprios_aliq = Sql.Recursos_proprios_aliq,
                    Recursos_proprios_atual = Sql.Recursos_proprios_atual,
                    Recursos_conta_valor = Sql.Recursos_conta_valor,
                    Recursos_conta_aliq = Sql.Recursos_conta_aliq,
                    Recursos_conta_atual = Sql.Recursos_conta_atual,
                    Recursos_concedido_valor = Sql.Recursos_concedido_valor,
                    Recursos_concedido_aliq = Sql.Recursos_concedido_aliq,
                    Recursos_concedido_atual = Sql.Recursos_concedido_atual,
                    Financiamento_valor = Sql.Financiamento_valor,
                    Financiamento_aliq = Sql.Financiamento_aliq,
                    Financiamento_atual = Sql.Financiamento_atual,
                    Totalidade = Sql.Totalidade,
                    Totalidade_Perc = Sql.Totalidade_Perc,
                    Matricula = Sql.Matricula,
                    Itbi_Numero = Sql.Itbi_Numero,
                    Itbi_Ano = Sql.Itbi_Ano,
                    Inscricao_Incra = Sql.Inscricao_Incra,
                    Receita_Federal = Sql.Receita_Federal,
                    Descricao_Imovel = Sql.Descricao_Imovel,
                    Userid = Sql.Userid,
                    Situacao_itbi = Sql.Situacao_itbi,
                    Liberado_por = Sql.Liberado_por,
                    Liberado_em = Sql.Liberado_em,
                    Data_Vencimento = Sql.Data_Vencimento,
                    Numero_Guia = Sql.Numero_Guia,
                    Utilizar_vvt = Sql.Utilizar_vvt
                    };
                return itbi;
                }
            }

        public List<Itbi_comprador> Retorna_Itbi_Comprador(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_comprador> Sql = (from t in db.Itbi_Comprador orderby t.Seq where t.Guid == Guid select t).ToList();
                return Sql;
                }
            }

        public List<Itbi_vendedor> Retorna_Itbi_Vendedor(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_vendedor> Sql = (from t in db.Itbi_Vendedor orderby t.Seq where t.Guid == Guid select t).ToList();
                return Sql;
                }
            }

        public List<Itbi_anexo> Retorna_Itbi_Anexo(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_anexo> Sql = (from t in db.Itbi_Anexo orderby t.Seq where t.Guid == Guid select t).ToList();
                return Sql;
                }
            }

        public Exception Incluir_Itbi_comprador(List<Itbi_comprador> Lista) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[4];
                int z = 0;
                foreach (Itbi_comprador item in Lista) {
                    Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = z };
                    Parametros[2] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = item.Nome };
                    Parametros[3] = new SqlParameter { ParameterName = "@cpf_cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = item.Cpf_cnpj };

                    db.Database.ExecuteSqlCommand("INSERT INTO itbi_comprador(guid,seq,nome,cpf_cnpj) " +
                                                  " VALUES(@guid,@seq,@nome,@cpf_cnpj)", Parametros);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    z++;
                    }

                return null;
                }
            }

        public Exception Incluir_Itbi_vendedor(List<Itbi_vendedor> Lista) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[4];
                int z = 0;
                foreach (Itbi_vendedor item in Lista) {
                    Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = z };
                    Parametros[2] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = item.Nome };
                    Parametros[3] = new SqlParameter { ParameterName = "@cpf_cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = item.Cpf_cnpj };

                    db.Database.ExecuteSqlCommand("INSERT INTO itbi_vendedor(guid,seq,nome,cpf_cnpj) " +
                                                  " VALUES(@guid,@seq,@nome,@cpf_cnpj)", Parametros);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    z++;
                    }

                return null;
                }
            }

        public byte Retorna_Itbi_Anexo_Disponivel(string Guid) {
            byte maxCod = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Anexo orderby t.Seq descending where t.Guid == Guid select t).FirstOrDefault();
                if (Sql != null) {
                    maxCod = (byte)(Sql.Seq + 1);
                    }
                }
            return maxCod;
            }

        public Exception Incluir_Itbi_Anexo(Itbi_anexo item) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = item.Seq };
                Parametros[2] = new SqlParameter { ParameterName = "@descricao", SqlDbType = SqlDbType.VarChar, SqlValue = item.Descricao };
                Parametros[3] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = item.Arquivo };

                db.Database.ExecuteSqlCommand("INSERT INTO itbi_anexo(guid,seq,descricao,arquivo) VALUES(@guid,@seq,@descricao,@arquivo)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }

                return null;
                }
            }

        public int Retorna_Itbi_Disponivel() {
            int _numero = 1;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Main orderby t.Itbi_Numero descending where t.Itbi_Ano == DateTime.Now.Year select t).FirstOrDefault();
                if (Sql != null) {
                    _numero = (short)(Sql.Itbi_Numero + 1);
                    }
                }
            return _numero;
            }

        public ItbiAnoNumero Alterar_Itbi_Main(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_main i = db.Itbi_Main.First(g => g.Guid == Guid);
                int _numero = Retorna_Itbi_Disponivel();
                short _ano = (short)DateTime.Now.Year;
                i.Itbi_Numero = _numero;
                i.Itbi_Ano = _ano;

                try {
                    db.SaveChanges();
                    } catch {
                    }

                ItbiAnoNumero _ret = new ItbiAnoNumero() {
                    Numero = _numero == 0 ? 1 : _numero,
                    Ano = _ano
                    };
                return _ret;
                }
            }

        public List<Itbi_Lista> Retorna_Itbi_Query(int user, bool f, int status, int ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_Lista> Lista = new List<Itbi_Lista>();

                var Sql = (from t in db.Itbi_Main
                           join c in db.Itbi_Status on t.Situacao_itbi equals c.Codigo into tc from c in tc.DefaultIfEmpty()
                           orderby  t.Itbi_Ano, t.Itbi_Numero, t.Situacao_itbi descending where t.Itbi_Ano == ano && t.Itbi_Numero > 0 select new { Ano = t.Itbi_Ano, Numero = t.Itbi_Numero, Guid = t.Guid, UserId = t.Userid,
                               DataCadastro = t.Data_cadastro, ImovelCodigo = t.Imovel_codigo, NomeComprador = t.Comprador_nome, SituacaoCodigo = t.Situacao_itbi, SituacaoNome = c.Descricao });
                if (status > 0)
                    Sql = Sql.Where(m => m.SituacaoCodigo == status);
                if (!f)//se for fiscal pode consultar qualquer ITBI
                    Sql = Sql.Where(m => m.UserId == user);
                else {
                    //  Sql = Sql.Where(m => m.SituacaoCodigo <3);//Só consultar ITBIs abertos
                    }

                foreach (var reg in Sql) {
                    Itbi_Lista item = new Itbi_Lista() {
                        Ano = reg.Ano,
                        Numero = reg.Numero,
                        Numero_Ano = reg.Numero.ToString("000000") + "/" + (reg.Ano).ToString(),
                        Guid = reg.Guid,
                        Data = reg.DataCadastro,
                        Tipo = reg.ImovelCodigo > 0 ? "Urbano" : "Rural",
                        Nome_Comprador = reg.NomeComprador,
                        Situacao = reg.SituacaoNome,
                        Situacao_Codigo = reg.SituacaoCodigo
                        };
                    Lista.Add(item);
                    }

                return Lista;
                }
            }

        public string Retorna_Itbi_Natureza_nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Natureza where t.Codigo == codigo select t.Descricao).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_Itbi_Financimento_nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.itbi_Financiamento where t.Codigo == codigo select t.Descricao).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_Itbi_Situacao(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Status where t.Codigo == codigo select t.Descricao).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public Itbi_status Retorna_Itbi_Situacao(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Main
                           join s in db.Itbi_Status on t.Situacao_itbi equals s.Codigo into ts from s in ts
                           where t.Guid == guid
                           select new { Codigo = t.Situacao_itbi, Descricao = s.Descricao }).FirstOrDefault();
                Itbi_status ret = new Itbi_status {
                    Codigo = Sql.Codigo,
                    Descricao = Sql.Descricao
                    };
                return ret;
                }
            }

        public Exception Alterar_Itbi_Situacao(string p, int s) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_main i = db.Itbi_Main.First(g => g.Guid == p);
                i.Situacao_itbi = s;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Liberar_Itbi_Isencao(string p, int FiscalId) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_isencao_main i = db.Itbi_Isencao_Main.First(g => g.Guid == p);
                i.Situacao = 5;
                i.Fiscal_id = FiscalId;
                DateTime newDate = DateTime.Today.AddDays(30);
                i.Data_validade = newDate.ToString("dd/MM/yyyy");

                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_Itbi_Forum(Itbi_forum item) {
            using (var db = new GTI_Context(_connection)) {
                int _seq = db.Itbi_Forum.Where(x => x.Guid == item.Guid)
                          .Select(x => x.Seq)
                          .DefaultIfEmpty((short)0)
                          .Max();
                _seq++;

                object[] Parametros = new object[6];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.SmallInt, SqlValue = _seq };
                Parametros[2] = new SqlParameter { ParameterName = "@datahora", SqlDbType = SqlDbType.DateTime, SqlValue = item.Datahora };
                Parametros[3] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = item.Userid };
                Parametros[4] = new SqlParameter { ParameterName = "@mensagem", SqlDbType = SqlDbType.VarChar, SqlValue = item.Mensagem };
                Parametros[5] = new SqlParameter { ParameterName = "@funcionario", SqlDbType = SqlDbType.Bit, SqlValue = item.Funcionario };

                db.Database.ExecuteSqlCommand("INSERT INTO itbi_forum(guid,seq,datahora,userid,mensagem,funcionario) VALUES(@guid,@seq,@datahora,@userid,@mensagem,@funcionario)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Alterar_Itbi_Forum(string p, short s, string msg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_forum i = db.Itbi_Forum.First(g => g.Guid == p && g.Seq == s);
                i.Mensagem = msg;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Excluir_Itbi_Forum(string p, short s) {
            object[] Parametros = new object[2];
            using (GTI_Context db = new GTI_Context(_connection)) {
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = p };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.SmallInt, SqlValue = s };

                db.Database.ExecuteSqlCommand("DELETE FROM itbi_forum WHERE guid=@guid AND seq=@seq", Parametros);
                return null;
                }
            }

        public List<Itbi_forum> Retorna_Itbi_Forum(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_forum> Sql = (from t in db.Itbi_Forum orderby t.Seq where t.Guid == p select t).ToList();
                return Sql;
                }
            }

        public Exception Incluir_Itbi_Guia(Itbi_Guia Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros2 = new object[1];
                Parametros2[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                db.Database.ExecuteSqlCommand("DELETE FROM itbi_guia WHERE guid=@guid", Parametros2);
                db.SaveChanges();

                object[] Parametros = new object[51];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Cadastro };
                Parametros[2] = new SqlParameter { ParameterName = "@imovel_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Imovel_Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[4] = new SqlParameter { ParameterName = "@proprietario_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Proprietario_Nome };
                Parametros[5] = new SqlParameter { ParameterName = "@imovel_endereco", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imovel_Endereco };
                Parametros[6] = new SqlParameter { ParameterName = "@imovel_numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Imovel_Numero };
                Parametros[7] = new SqlParameter { ParameterName = "@imovel_complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imovel_Complemento };
                Parametros[8] = new SqlParameter { ParameterName = "@imovel_cep", SqlDbType = SqlDbType.Int, SqlValue = Reg.Imovel_Cep };
                Parametros[9] = new SqlParameter { ParameterName = "@imovel_bairro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imovel_Bairro };
                Parametros[10] = new SqlParameter { ParameterName = "@imovel_quadra", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imovel_Quadra };
                Parametros[11] = new SqlParameter { ParameterName = "@imovel_lote", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Imovel_Lote };
                Parametros[12] = new SqlParameter { ParameterName = "@comprador_cpf_cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Cpf_Cnpj };
                Parametros[13] = new SqlParameter { ParameterName = "@comprador_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Comprador_Codigo };
                Parametros[14] = new SqlParameter { ParameterName = "@comprador_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Nome };
                Parametros[15] = new SqlParameter { ParameterName = "@comprador_logradouro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Logradouro };
                Parametros[16] = new SqlParameter { ParameterName = "@comprador_numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Comprador_Numero };
                Parametros[17] = new SqlParameter { ParameterName = "@comprador_complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Complemento };
                Parametros[18] = new SqlParameter { ParameterName = "@comprador_cep", SqlDbType = SqlDbType.Int, SqlValue = Reg.Comprador_Cep };
                Parametros[19] = new SqlParameter { ParameterName = "@comprador_bairro", SqlDbType = SqlDbType.VarChar, SqlValue =  dalCore.TruncateTo( Reg.Comprador_Bairro,40) };
                Parametros[20] = new SqlParameter { ParameterName = "@comprador_cidade", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Cidade };
                Parametros[21] = new SqlParameter { ParameterName = "@comprador_uf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Comprador_Uf };
                Parametros[22] = new SqlParameter { ParameterName = "@tipo_instrumento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo_Instrumento };
                Parametros[23] = new SqlParameter { ParameterName = "@data_transacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Transacao };
                Parametros[24] = new SqlParameter { ParameterName = "@valor_transacao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Transacao };
                Parametros[25] = new SqlParameter { ParameterName = "@valor_avaliacao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Avaliacao };
                Parametros[26] = new SqlParameter { ParameterName = "@valor_venal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Venal };
                Parametros[27] = new SqlParameter { ParameterName = "@recursos_proprios_valor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_proprios_Valor };
                Parametros[28] = new SqlParameter { ParameterName = "@recursos_proprios_atual", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_proprios_Atual };
                Parametros[29] = new SqlParameter { ParameterName = "@recursos_conta_valor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_conta_Valor };
                Parametros[30] = new SqlParameter { ParameterName = "@recursos_conta_atual", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_conta_Atual };
                Parametros[31] = new SqlParameter { ParameterName = "@recursos_concedido_valor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_concedido_Valor };
                Parametros[32] = new SqlParameter { ParameterName = "@recursos_concedido_atual", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Recursos_concedido_Atual };
                Parametros[33] = new SqlParameter { ParameterName = "@financiamento_valor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Financiamento_Valor };
                Parametros[34] = new SqlParameter { ParameterName = "@financiamento_atual", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Financiamento_Atual };
                Parametros[35] = new SqlParameter { ParameterName = "@totalidade", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Totalidade };
                Parametros[36] = new SqlParameter { ParameterName = "@totalidade_perc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Totalidade_Perc };
                Parametros[37] = new SqlParameter { ParameterName = "@matricula", SqlDbType = SqlDbType.BigInt, SqlValue = Reg.Matricula };
                Parametros[38] = new SqlParameter { ParameterName = "@itbi_numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Itbi_Numero };
                Parametros[39] = new SqlParameter { ParameterName = "@itbi_ano", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Itbi_Ano };
                Parametros[40] = new SqlParameter { ParameterName = "@inscricao_incra", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao_Incra };
                Parametros[41] = new SqlParameter { ParameterName = "@receita_federal", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Receita_Federal };
                Parametros[42] = new SqlParameter { ParameterName = "@descricao_imovel", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Descricao_Imovel };
                Parametros[43] = new SqlParameter { ParameterName = "@valor_guia", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Guia };
                Parametros[44] = new SqlParameter { ParameterName = "@numero_guia", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_guia };
                Parametros[45] = new SqlParameter { ParameterName = "@nosso_numero", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nosso_Numero };
                Parametros[46] = new SqlParameter { ParameterName = "@linha_digitavel", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Linha_Digitavel };
                Parametros[47] = new SqlParameter { ParameterName = "@codigo_barra", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Codigo_Barra };
                Parametros[48] = new SqlParameter { ParameterName = "@data_vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                Parametros[49] = new SqlParameter { ParameterName = "@natureza", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Natureza };
                Parametros[50] = new SqlParameter { ParameterName = "@tipo_financiamento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo_Financiamento };

                db.Database.ExecuteSqlCommand("INSERT INTO itbi_guia(guid,data_cadastro,imovel_codigo,inscricao,proprietario_nome,imovel_endereco,imovel_numero,imovel_complemento,imovel_cep," +
                                            "imovel_bairro,imovel_quadra,imovel_lote,comprador_cpf_cnpj,comprador_codigo,comprador_nome,comprador_logradouro,comprador_numero,comprador_complemento," +
                                            "comprador_cep,comprador_bairro,comprador_cidade,comprador_uf,tipo_instrumento,data_transacao,valor_transacao,valor_avaliacao,valor_venal,recursos_proprios_valor," +
                                            "recursos_proprios_atual,recursos_conta_valor,recursos_conta_atual,recursos_concedido_valor,recursos_concedido_atual,financiamento_valor,financiamento_atual," +
                                            "totalidade,totalidade_perc,matricula,itbi_numero,itbi_ano,inscricao_incra,receita_federal,descricao_imovel,valor_guia,numero_guia,nosso_numero,linha_digitavel," +
                                            "codigo_barra,data_vencimento,natureza,tipo_financiamento) VALUES(@guid,@data_cadastro,@imovel_codigo,@inscricao,@proprietario_nome,@imovel_endereco,@imovel_numero,@imovel_complemento,@imovel_cep," +
                                            "@imovel_bairro,@imovel_quadra,@imovel_lote,@comprador_cpf_cnpj,@comprador_codigo,@comprador_nome,@comprador_logradouro,@comprador_numero,@comprador_complemento," +
                                            "@comprador_cep,@comprador_bairro,@comprador_cidade,@comprador_uf,@tipo_instrumento,@data_transacao,@valor_transacao,@valor_avaliacao,@valor_venal,@recursos_proprios_valor," +
                                            "@recursos_proprios_atual,@recursos_conta_valor,@recursos_conta_atual,@recursos_concedido_valor,@recursos_concedido_atual,@financiamento_valor,@financiamento_atual," +
                                            "@totalidade,@totalidade_perc,@matricula,@itbi_numero,@itbi_ano,@inscricao_incra,@receita_federal,@descricao_imovel,@valor_guia,@numero_guia,@nosso_numero,@linha_digitavel," +
                                            "@codigo_barra,@data_vencimento,@natureza,@tipo_financiamento)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }

                return null;
                }
            }

        public Exception Alterar_Itbi_Guia(string p, int n, DateTime d, int f) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_main i = db.Itbi_Main.First(g => g.Guid == p);
                i.Data_Vencimento = d;
                i.Numero_Guia = n;
                i.Liberado_por = f;
                i.Liberado_em = DateTime.Now;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_isencao_main(Itbi_isencao_main Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[10];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@isencao_numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Isencao_numero };
                Parametros[2] = new SqlParameter { ParameterName = "@isencao_ano", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Isencao_ano };
                Parametros[3] = new SqlParameter { ParameterName = "@fiscal_id", SqlDbType = SqlDbType.Int, SqlValue = Reg.Fiscal_id };
                Parametros[4] = new SqlParameter { ParameterName = "@usuario_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usuario_nome };
                Parametros[5] = new SqlParameter { ParameterName = "@usuario_doc", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usuario_doc };
                Parametros[6] = new SqlParameter { ParameterName = "@natureza", SqlDbType = SqlDbType.Int, SqlValue = Reg.Natureza };
                Parametros[7] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[8] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[9] = new SqlParameter { ParameterName = "@usuario_id", SqlDbType = SqlDbType.Int, SqlValue = Reg.Usuario_id };
                db.Database.ExecuteSqlCommand("INSERT INTO itbi_isencao_main(guid,isencao_numero,isencao_ano,fiscal_id,usuario_nome,usuario_doc,natureza,data_cadastro,situacao,usuario_id) " +
                                              " VALUES(@guid,@isencao_numero,@isencao_ano,@fiscal_id,@usuario_nome,@usuario_doc,@natureza,@data_cadastro,@situacao,@usuario_id)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Alterar_Itbi_Isencao_Natureza(string p, int n) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_isencao_main i = db.Itbi_Isencao_Main.First(g => g.Guid == p);
                i.Natureza = n;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_Itbi_isencao_imovel(List<Itbi_isencao_imovel> Lista) {
            using (var db = new GTI_Context(_connection)) {
                string guid = Lista[0].Guid;
                try {
                    db.Itbi_Isencao_Imovel.RemoveRange(db.Itbi_Isencao_Imovel.Where(i => i.Guid == guid));
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }

                object[] Parametros = new object[5];
                foreach (Itbi_isencao_imovel item in Lista) {
                    Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = item.Seq };
                    Parametros[2] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.VarChar, SqlValue = item.Tipo };
                    Parametros[3] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = item.Codigo };
                    Parametros[4] = new SqlParameter { ParameterName = "@descricao", SqlDbType = SqlDbType.VarChar, SqlValue = item.Descricao };

                    db.Database.ExecuteSqlCommand("INSERT INTO Itbi_isencao_imovel(guid,seq,tipo,codigo,descricao) " +
                                                  " VALUES(@guid,@seq,@tipo,@codigo,@descricao)", Parametros);
                    try {
                        db.SaveChanges();
                        } catch (Exception ex) {
                        return ex;
                        }
                    }

                return null;
                }
            }

        public Itbi_isencao_main_Struct Retorna_Itbi_Isencao_Main(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Isencao_Main
                           join c in db.Itbi_Status on t.Situacao equals c.Codigo into tc from c in tc.DefaultIfEmpty()
                           join n in db.Itbi_Natureza_Isencao on t.Natureza equals n.Codigo into tn from n in tn.DefaultIfEmpty()
                           where t.Guid == Guid select new { guid = t.Guid, data_cadastro = t.Data_cadastro, natureza_codigo = t.Natureza, usuario_nome = t.Usuario_nome, usuario_doc = t.Usuario_doc, validade = t.Data_validade,
                               fiscal_id = t.Fiscal_id, isencao_ano = t.Isencao_ano, isencao_numero = t.Isencao_numero, natureza_nome = n.Descricao, situacao_nome = c.Descricao, situacao_codigo = t.Situacao }).First();
                Itbi_isencao_main_Struct itbi = new Itbi_isencao_main_Struct() {
                    Guid = Sql.guid,
                    Data_cadastro = Sql.data_cadastro,
                    Natureza = Sql.natureza_codigo,
                    Usuario_nome = Sql.usuario_nome,
                    Usuario_doc = Sql.usuario_doc,
                    Fiscal_id = Sql.fiscal_id,
                    Isencao_ano = Sql.isencao_ano,
                    Isencao_numero = Sql.isencao_numero,
                    Situacao = Sql.situacao_codigo,
                    Situacao_Nome = Sql.situacao_nome,
                    Natureza_Nome = Sql.natureza_nome,
                    Data_validade = Sql.validade
                    };
                return itbi;
                }

            }

        public List<Itbi_isencao_imovel> Retorna_Itbi_Isencao_Imovel(string Guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_isencao_imovel> Sql = (from t in db.Itbi_Isencao_Imovel orderby t.Seq where t.Guid == Guid select t).ToList();
                return Sql;
                }
            }

        public Exception Excluir_Itbi_Isencao_Imovel(string guid, int seq) {
            object[] Parametros = new object[2];
            using (GTI_Context db = new GTI_Context(_connection)) {
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.TinyInt, SqlValue = seq };

                db.Database.ExecuteSqlCommand("DELETE FROM itbi_isencao_imovel WHERE guid=@guid AND seq=@seq", Parametros);
                return null;
                }
            }

        public Exception Excluir_Itbi_Guia(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[2];
                var Sql = (from t in db.Itbi_Guia where t.Guid == guid select t.Numero_guia).FirstOrDefault();
                int documento = 0;
                if (Sql > 0)
                    documento = Convert.ToInt32(Sql);
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = guid };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_guia", SqlDbType = SqlDbType.Int, SqlValue = documento };

                if (Sql > 0)
                    db.Database.ExecuteSqlCommand("DELETE FROM itbi_guia WHERE guid=@guid and numero_guia=@numero_guia", Parametros);

                db.Database.ExecuteSqlCommand("UPDATE itbi_main set situacao_itbi=1, liberado_por=0,liberado_em=null,numero_guia=0,data_vencimento=null WHERE guid=@guid", Parametros);



                return null;
                }
            }

        public List<Itbi_natureza_isencao> Lista_itbi_natureza_isencao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_natureza_isencao> Sql = (from t in db.Itbi_Natureza_Isencao orderby t.Codigo select t).ToList();
                return Sql;
                }
            }

        public Exception Alterar_Itbi_Isencao(Itbi_isencao_main reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_isencao_main i = db.Itbi_Isencao_Main.First(g => g.Guid == reg.Guid);
                i.Natureza = reg.Natureza;
                i.Isencao_ano = reg.Isencao_ano;
                i.Isencao_numero = reg.Isencao_numero;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public List<Itbi_Lista> Retorna_Itbi_Isencao_Query(int user, bool f, int status) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Itbi_Lista> Lista = new List<Itbi_Lista>();

                var Sql = (from t in db.Itbi_Isencao_Main
                           join c in db.Itbi_Status on t.Situacao equals c.Codigo into tc from c in tc.DefaultIfEmpty()
                           orderby new { t.Isencao_ano, t.Isencao_numero } where t.Isencao_numero > 0 select new {
                               Ano = t.Isencao_ano, Numero = t.Isencao_numero, Guid = t.Guid, UserId = t.Usuario_id, UsuarioNome = t.Usuario_nome,
                               DataCadastro = t.Data_cadastro, SituacaoCodigo = t.Situacao, SituacaoNome = c.Descricao, t.Data_validade
                               });
                if (status > 0)
                    Sql = Sql.Where(m => m.SituacaoCodigo == status);
                if (!f)//se for fiscal pode consultar qualquer ITBI
                    Sql = Sql.Where(m => m.UserId == user);
                else
                    Sql = Sql.Where(m => m.SituacaoCodigo == 1 || m.SituacaoCodigo == 5);

                foreach (var reg in Sql) {
                    Itbi_Lista item = new Itbi_Lista() {
                        Ano = reg.Ano,
                        Numero = reg.Numero,
                        Numero_Ano = reg.Numero.ToString("000000") + "/" + (reg.Ano).ToString(),
                        Guid = reg.Guid,
                        Data = reg.DataCadastro,
                        Nome_Requerente = reg.UsuarioNome,
                        Situacao = reg.SituacaoNome,
                        Situacao_Codigo = reg.SituacaoCodigo,
                        Usuario_Id = reg.UserId,
                        Validade = reg.Data_validade
                        };
                    Lista.Add(item);
                    }

                return Lista;
                }
            }

        public int Retorna_Itbi_Isencao_Disponivel() {
            int _numero = 1;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Itbi_Isencao_Main orderby t.Isencao_numero descending where t.Isencao_ano == DateTime.Now.Year
                           select new { t.Guid, t.Data_cadastro, t.Data_validade, t.Fiscal_id, t.Isencao_ano, t.Isencao_numero, t.Natureza,
                               t.QRCode, t.Situacao, t.Usuario_doc, t.Usuario_nome }).FirstOrDefault();
                if (Sql != null) {
                    _numero = (short)(Sql.Isencao_numero + 1);
                    }
                }
            return _numero;
            }

        public Exception Alterar_Itbi_Isencao_Situacao(string p, int s) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_isencao_main i = db.Itbi_Isencao_Main.First(g => g.Guid == p);
                i.Situacao = s;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Alterar_Itbi_Isencao_QRCode(string p, byte[] code) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Itbi_isencao_main i = db.Itbi_Isencao_Main.First(g => g.Guid == p);
                i.QRCode = code;
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public Exception Incluir_notificacao_terreno(Notificacao_terreno Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[21];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_not };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_not };
                Parametros[2] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco_infracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_infracao };
                Parametros[5] = new SqlParameter { ParameterName = "@endereco_prop", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop };
                Parametros[6] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[7] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[9] = new SqlParameter { ParameterName = "@prazo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Prazo };
                Parametros[10] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[11] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[12] = new SqlParameter { ParameterName = "@nome2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome2 ?? "" };
                Parametros[13] = new SqlParameter { ParameterName = "@codigo_cidadao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao };
                Parametros[14] = new SqlParameter { ParameterName = "@codigo_cidadao2", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao2 };
                Parametros[15] = new SqlParameter { ParameterName = "@cpf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf };
                Parametros[16] = new SqlParameter { ParameterName = "@rg", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg ?? "" };
                Parametros[17] = new SqlParameter { ParameterName = "@cpf2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf2 ?? "" };
                Parametros[18] = new SqlParameter { ParameterName = "@rg2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg2 ?? "" };
                Parametros[19] = new SqlParameter { ParameterName = "@endereco_prop2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop2 ?? "" };
                Parametros[20] = new SqlParameter { ParameterName = "@endereco_entrega2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega2 ?? "" };

                db.Database.ExecuteSqlCommand("INSERT INTO notificacao_terreno(ano_not,numero_not,codigo,situacao,endereco_infracao,endereco_prop,endereco_entrega,nome,inscricao,prazo,data_cadastro," +
                                              "userid,nome2,codigo_cidadao,codigo_cidadao2,cpf,rg,cpf2,rg2,endereco_prop2,endereco_entrega2) " +
                                              " VALUES(@ano_not,@numero_not,@codigo,@situacao,@endereco_infracao,@endereco_prop,@endereco_entrega,@nome,@inscricao,@prazo,@data_cadastro,@userid," +
                                              "@nome2,@codigo_cidadao,@codigo_cidadao2,@cpf,@rg,@cpf2,@rg2,@endereco_prop2,@endereco_entrega2)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public bool Existe_Notificacao_Terreno(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Notificacao_Terreno
                           where i.Ano_not == Ano && i.Numero_not == Numero select i.Inscricao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
                }
            }

        public List<Notificacao_Terreno_Struct> Lista_Notificacao_Terreno(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Terreno
                           where t.Ano_not == Ano
                           orderby t.Numero_not select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo
                               }).ToList();
                List<Notificacao_Terreno_Struct> Lista = new List<Notificacao_Terreno_Struct>();
                foreach (var item in Sql) {
                    Notificacao_Terreno_Struct reg = new Notificacao_Terreno_Struct() {
                        Ano_Notificacao = item.Ano,
                        Numero_Notificacao = item.Numero,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Userid = item.Usuario,
                        Situacao = item.Situacao,
                        Prazo = item.Prazo,
                        AnoNumero = item.Numero.ToString("0000") + "/" + item.Ano.ToString(),
                        Nome_Proprietario = item.Nome
                        };
                    Lista.Add(reg);
                    }
                return Lista;
                }
            }

        public Notificacao_Terreno_Struct Retorna_Notificacao_Terreno(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Terreno
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu
                           where t.Ano_not == Ano && t.Numero_not == Numero select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo,
                               Endereco_entrega = t.Endereco_entrega, Endereco_prop = t.Endereco_prop, Endereco_Infracao = t.Endereco_infracao, Usuario_Nome = u.Nomecompleto, Inscricao = t.Inscricao,
                               t.Nome2, t.Codigo_cidadao, t.Codigo_cidadao2, t.Cpf, t.Rg, t.Cpf2, t.Rg2, t.Endereco_entrega2, t.Endereco_prop2
                               }).FirstOrDefault();
                Notificacao_Terreno_Struct reg = null;
                if (Sql != null) {
                    reg = new Notificacao_Terreno_Struct() {
                        Ano_Notificacao = Sql.Ano,
                        Numero_Notificacao = Sql.Numero,
                        AnoNumero = Sql.Numero.ToString("0000") + "/" + Sql.Ano.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Prazo = Sql.Prazo,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                        };
                    }
                return reg;
                }
            }

        public Exception Incluir_auto_infracao(Auto_infracao Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_auto };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_auto };
                Parametros[2] = new SqlParameter { ParameterName = "@ano_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_notificacao };
                Parametros[3] = new SqlParameter { ParameterName = "@numero_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_notificacao };
                Parametros[4] = new SqlParameter { ParameterName = "@data_notificacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_notificacao };
                Parametros[5] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };

                db.Database.ExecuteSqlCommand("INSERT INTO auto_infracao(ano_auto,numero_auto,ano_notificacao,numero_notificacao,data_notificacao,data_cadastro,userid) " +
                                              "VALUES(@ano_auto,@numero_auto,@ano_notificacao,@numero_notificacao,@data_notificacao,@data_cadastro,@userid)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public bool Existe_Auto_Infracao(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Auto_Infracao
                           where i.Ano_auto == Ano && i.Numero_auto == Numero select i.Numero_notificacao).FirstOrDefault();
                if (reg == 0)
                    return false;
                else
                    return true;
                }
            }

        public List<Auto_Infracao_Struct> Lista_Auto_Infracao(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = "SELECT auto_infracao.ano_auto,auto_infracao.numero_auto,auto_infracao.ano_notificacao,auto_infracao.numero_notificacao,auto_infracao.data_notificacao ,auto_infracao.data_cadastro,";
                Sql += "auto_infracao.userid,notificacao_terreno.codigo,notificacao_terreno.nome as Nome_Proprietario FROM dbo.notificacao_terreno INNER JOIN dbo.auto_infracao ON notificacao_terreno.ano_not = auto_infracao.ano_notificacao ";
                Sql += "AND notificacao_terreno.numero_not = auto_infracao.numero_notificacao WHERE auto_infracao.ano_auto = @Ano";
                var Ret = db.Database.SqlQuery<Auto_Infracao_Struct>(Sql, new SqlParameter("@Ano", Ano)).ToList();
                //var Sql = (from a in db.Auto_Infracao
                //           join n in db.Notificacao_Terreno on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                //           where a.Ano_notificacao == Ano
                //           orderby a.Numero_notificacao select new {
                //               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                //               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome
                //               }).ToList();
                List<Auto_Infracao_Struct> Lista = new List<Auto_Infracao_Struct>();
                foreach (var item in Ret) {
                    Auto_Infracao_Struct reg = new Auto_Infracao_Struct() {
                        Ano_Auto = item.Ano_Auto,
                        Numero_Auto = item.Numero_Auto,
                        Ano_Notificacao = item.Ano_Notificacao,
                        Numero_Notificacao = item.Numero_Notificacao,
                        Codigo_Imovel = item.Codigo_Imovel,
                        Data_Cadastro = item.Data_Cadastro,
                        Data_Notificacao = item.Data_Notificacao,
                        Userid = item.Userid,
                        AnoNumero = item.Numero_Notificacao.ToString("0000") + "/" + item.Ano_Notificacao.ToString(),
                        AnoNumeroAuto = item.Numero_Auto.ToString("0000") + "/" + item.Ano_Auto.ToString(),
                        Nome_Proprietario = item.Nome_Proprietario
                        };
                    Lista.Add(reg);
                    }
                return Lista;
                }
            }

        public Auto_Infracao_Struct Retorna_Auto_Infracao(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao
                           join n in db.Notificacao_Terreno on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           join u in db.Usuario on a.Userid equals u.Id into tu from u in tu
                           where a.Ano_auto == Ano && a.Numero_auto == Numero select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome, Endereco_entrega = n.Endereco_entrega, Endereco_prop = n.Endereco_prop, Endereco_Infracao = n.Endereco_infracao,
                               Usuario_Nome = u.Nomecompleto, Inscricao = n.Inscricao, n.Nome2, n.Codigo_cidadao, n.Codigo_cidadao2, n.Cpf, n.Rg, n.Cpf2, n.Rg2, n.Endereco_entrega2, n.Endereco_prop2

                               }).FirstOrDefault();
                Auto_Infracao_Struct reg = null;
                if (Sql != null) {
                    reg = new Auto_Infracao_Struct() {
                        Ano_Auto = Sql.AnoAuto,
                        Numero_Auto = Sql.NumeroAuto,
                        Ano_Notificacao = Sql.AnoNot,
                        Numero_Notificacao = Sql.NumeroNot,
                        AnoNumero = Sql.NumeroNot.ToString("0000") + "/" + Sql.AnoNot.ToString(),
                        AnoNumeroAuto = Sql.NumeroAuto.ToString("0000") + "/" + Sql.AnoAuto.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Data_Notificacao = Sql.Data_Notificaao,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                        };
                    }
                return reg;
                }
            }

        public string Retorna_Benfeitoria_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Benfeitoria where t.Codbenfeitoria == codigo select t.Descbenfeitoria).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_CategoriaProp_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Categprop where t.Codcategprop == codigo select t.Desccategprop).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_Pedologia_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Pedologia where t.Codpedologia == codigo select t.Descpedologia).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_Situacao_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Situacao where t.Codsituacao == codigo select t.Descsituacao).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_Topografia_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Topografia where t.Codtopografia == codigo select t.Desctopografia).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public string Retorna_UsoConstr_Nome(int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Usoconstr where t.Codusoconstr == codigo select t.Descusoconstr).FirstOrDefault();
                if (Sql == null)
                    return "";
                else
                    return Sql;
                }
            }

        public Exception Incluir_Endereco_Entrega(Endentrega Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM endentrega WHERE Codreduzido=@Codreduzido",
                        new SqlParameter("@Codreduzido", Reg.Codreduzido));
                    } catch (Exception ex) {
                    return ex;
                    }

                object[] Parametros = new object[9];
                Parametros[0] = new SqlParameter { ParameterName = "@codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codreduzido };
                Parametros[1] = new SqlParameter { ParameterName = "@ee_codlog", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ee_codlog };
                Parametros[2] = new SqlParameter { ParameterName = "@ee_nomelog", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ee_nomelog };
                Parametros[3] = new SqlParameter { ParameterName = "@ee_numimovel", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Ee_numimovel };
                Parametros[4] = new SqlParameter { ParameterName = "@ee_complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ee_complemento };
                Parametros[5] = new SqlParameter { ParameterName = "@ee_uf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ee_uf };
                Parametros[6] = new SqlParameter { ParameterName = "@ee_cidade", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Ee_cidade };
                Parametros[7] = new SqlParameter { ParameterName = "@ee_bairro", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Ee_bairro };
                Parametros[8] = new SqlParameter { ParameterName = "@ee_cep", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ee_cep };

                db.Database.ExecuteSqlCommand("INSERT INTO endentrega(codreduzido,ee_codlog,ee_nomelog,ee_numimovel,ee_complemento,ee_uf,ee_cidade,ee_bairro,ee_cep) " +
                                              "VALUES(@codreduzido,@ee_codlog,@ee_nomelog,@ee_numimovel,@ee_complemento,@ee_uf,@ee_cidade,@ee_bairro,@ee_cep)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public int Retorna_Codigo_Endereco(int Logradouro, int Numero) {
            EnderecoStruct regEnd = new EnderecoStruct();
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Cadimob
                           join fq in db.Facequadra on new { p1 = i.Distrito, p2 = i.Setor, p3 = i.Quadra, p4 = i.Seq } equals new { p1 = fq.Coddistrito, p2 = fq.Codsetor, p3 = fq.Codquadra, p4 = fq.Codface } into ifq from fq in ifq.DefaultIfEmpty()
                           join l in db.Logradouro on fq.Codlogr equals l.Codlogradouro into lfq from l in lfq.DefaultIfEmpty()
                           where l.Codlogradouro == Logradouro && i.Li_num == Numero
                           select i.Codreduzido).FirstOrDefault();
                if (reg == 0)
                    return 0;
                else
                    return reg;
                }

            }

        public List<int> Lista_Imovel_Cpf(string Cpf) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var reg = (from p in db.Proprietario
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                           where c.Cpf == Cpf select p.Codreduzido).ToList();
                return reg;
                }
            }

        public List<int> Lista_Imovel_Cnpj(string Cnpj) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var reg = (from p in db.Proprietario
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                           where c.Cnpj == Cnpj select p.Codreduzido).ToList();
                return reg;
                }
            }

        public Exception Incluir_notificacao_obra(Notificacao_Obra Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[21];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_not };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_not };
                Parametros[2] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco_infracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_infracao };
                Parametros[5] = new SqlParameter { ParameterName = "@endereco_prop", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop };
                Parametros[6] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[7] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[9] = new SqlParameter { ParameterName = "@prazo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Prazo };
                Parametros[10] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[11] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[12] = new SqlParameter { ParameterName = "@nome2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome2 ?? "" };
                Parametros[13] = new SqlParameter { ParameterName = "@codigo_cidadao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao };
                Parametros[14] = new SqlParameter { ParameterName = "@codigo_cidadao2", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao2 };
                Parametros[15] = new SqlParameter { ParameterName = "@cpf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf };
                Parametros[16] = new SqlParameter { ParameterName = "@rg", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg ?? "" };
                Parametros[17] = new SqlParameter { ParameterName = "@cpf2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf2 ?? "" };
                Parametros[18] = new SqlParameter { ParameterName = "@rg2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg2 ?? "" };
                Parametros[19] = new SqlParameter { ParameterName = "@endereco_prop2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop2 ?? "" };
                Parametros[20] = new SqlParameter { ParameterName = "@endereco_entrega2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega2 ?? "" };

                db.Database.ExecuteSqlCommand("INSERT INTO notificacao_obra(ano_not,numero_not,codigo,situacao,endereco_infracao,endereco_prop,endereco_entrega,nome,inscricao,prazo,data_cadastro," +
                                              "userid,nome2,codigo_cidadao,codigo_cidadao2,cpf,rg,cpf2,rg2,endereco_prop2,endereco_entrega2) " +
                                              " VALUES(@ano_not,@numero_not,@codigo,@situacao,@endereco_infracao,@endereco_prop,@endereco_entrega,@nome,@inscricao,@prazo,@data_cadastro,@userid," +
                                              "@nome2,@codigo_cidadao,@codigo_cidadao2,@cpf,@rg,@cpf2,@rg2,@endereco_prop2,@endereco_entrega2)", Parametros);
                try {
                    db.SaveChanges();
                    } catch (Exception ex) {
                    return ex;
                    }
                return null;
                }
            }

        public bool Existe_Notificacao_Obra(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Notificacao_Obra
                           where i.Ano_not == Ano && i.Numero_not == Numero select i.Inscricao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
                }
            }

        public List<Notificacao_Obra_Struct> Lista_Notificacao_Obra(int Ano) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Obra
                           where t.Ano_not == Ano
                           orderby t.Numero_not select new {
                               Ano = t.Ano_not,Numero = t.Numero_not,Codigo = t.Codigo,Data_Cadastro = t.Data_cadastro,Usuario = t.Userid,Situacao = t.Situacao,Nome = t.Nome,Prazo = t.Prazo
                           }).ToList();
                List<Notificacao_Obra_Struct> Lista = new List<Notificacao_Obra_Struct>();
                foreach(var item in Sql) {
                    Notificacao_Obra_Struct reg = new Notificacao_Obra_Struct() {
                        Ano_Notificacao = item.Ano,
                        Numero_Notificacao = item.Numero,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Userid = item.Usuario,
                        Situacao = item.Situacao,
                        Prazo = item.Prazo,
                        AnoNumero = item.Numero.ToString("0000") + "/" + item.Ano.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Notificacao_Obra_Struct Retorna_Notificacao_Obra(int Ano,int Numero) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Obra
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu
                           where t.Ano_not == Ano && t.Numero_not == Numero select new {
                               Ano = t.Ano_not,Numero = t.Numero_not,Codigo = t.Codigo,Data_Cadastro = t.Data_cadastro,Usuario = t.Userid,Situacao = t.Situacao,Nome = t.Nome,Prazo = t.Prazo,
                               Endereco_entrega = t.Endereco_entrega,Endereco_prop = t.Endereco_prop,Endereco_Infracao = t.Endereco_infracao,Usuario_Nome = u.Nomecompleto,Inscricao = t.Inscricao,
                               t.Nome2,t.Codigo_cidadao,t.Codigo_cidadao2,t.Cpf,t.Rg,t.Cpf2,t.Rg2,t.Endereco_entrega2,t.Endereco_prop2
                           }).FirstOrDefault();
                Notificacao_Obra_Struct reg = null;
                if(Sql != null) {
                    reg = new Notificacao_Obra_Struct() {
                        Ano_Notificacao = Sql.Ano,
                        Numero_Notificacao = Sql.Numero,
                        AnoNumero = Sql.Numero.ToString("0000") + "/" + Sql.Ano.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Prazo = Sql.Prazo,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                    };
                }
                return reg;
            }
        }

        public short Retorna_Proxima_Seq_Historico(int Codigo) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Historico where c.Codreduzido == Codigo orderby c.Seq descending select c.Seq).FirstOrDefault();
                return (short)(Sql + 1);
            }
        }

        public List<CidadaoHeader> Lista_Imovel_Cnpj(string Cnpj,bool Principal = false) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from s in db.Cidadao_Socio
                           join c in db.Cidadao on s.Codigo_Socio equals c.Codcidadao into sc from c in sc.DefaultIfEmpty()
                           where c.Cnpj == Cnpj orderby s.Codigo_Socio select new CidadaoHeader { Codigo = s.Codigo_Empresa,Nome=c.Nomecidadao,Cpf=c.Cpf,Cnpj=c.Cnpj }).ToList();

                return Sql;
            }
        }

        public List<int> Lista_Imovel_Socio(int Codigo_Socio) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from p in db.Proprietario where p.Codcidadao == Codigo_Socio && p.Principal==true orderby p.Codreduzido select p.Codreduzido).ToList();

                return Sql;
            }
        }

        public bool Existe_AutoInfracao_Queimada(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Auto_Infracao_Queimada
                           where i.Ano_multa == Ano && i.Numero_multa == Numero select i.Inscricao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
                }
            }

        public Exception Incluir_AutoInfracao_Queimada(Auto_Infracao_Queimada Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[21];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_multa", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_multa };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_multa", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_multa };
                Parametros[2] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco_infracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_infracao };
                Parametros[5] = new SqlParameter { ParameterName = "@endereco_prop", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop };
                Parametros[6] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[7] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[9] = new SqlParameter { ParameterName = "@prazo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Prazo };
                Parametros[10] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[11] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[12] = new SqlParameter { ParameterName = "@nome2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome2 ?? "" };
                Parametros[13] = new SqlParameter { ParameterName = "@codigo_cidadao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao };
                Parametros[14] = new SqlParameter { ParameterName = "@codigo_cidadao2", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao2 };
                Parametros[15] = new SqlParameter { ParameterName = "@cpf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf };
                Parametros[16] = new SqlParameter { ParameterName = "@rg", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg ?? "" };
                Parametros[17] = new SqlParameter { ParameterName = "@cpf2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf2 ?? "" };
                Parametros[18] = new SqlParameter { ParameterName = "@rg2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg2 ?? "" };
                Parametros[19] = new SqlParameter { ParameterName = "@endereco_prop2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop2 ?? "" };
                Parametros[20] = new SqlParameter { ParameterName = "@endereco_entrega2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega2 ?? "" };

                db.Database.ExecuteSqlCommand("INSERT INTO auto_infracao_queimada(ano_multa,numero_multa,codigo,situacao,endereco_infracao,endereco_prop,endereco_entrega,nome,inscricao,prazo,data_cadastro," +
                                              "userid,nome2,codigo_cidadao,codigo_cidadao2,cpf,rg,cpf2,rg2,endereco_prop2,endereco_entrega2) " +
                                              " VALUES(@ano_multa,@numero_multa,@codigo,@situacao,@endereco_infracao,@endereco_prop,@endereco_entrega,@nome,@inscricao,@prazo,@data_cadastro,@userid," +
                                              "@nome2,@codigo_cidadao,@codigo_cidadao2,@cpf,@rg,@cpf2,@rg2,@endereco_prop2,@endereco_entrega2)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Auto_Infracao_Queimada_Struct> Lista_AutoInfracao_Queimada(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Auto_Infracao_Queimada
                           where t.Ano_multa == Ano
                           orderby t.Numero_multa select new {
                               Ano = t.Ano_multa, Numero = t.Numero_multa, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo
                           }).ToList();
                List<Auto_Infracao_Queimada_Struct> Lista = new List<Auto_Infracao_Queimada_Struct>();
                foreach (var item in Sql) {
                    Auto_Infracao_Queimada_Struct reg = new Auto_Infracao_Queimada_Struct() {
                        Ano_Multa = item.Ano,
                        Numero_Multa = item.Numero,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Userid = item.Usuario,
                        Situacao = item.Situacao,
                        Prazo = item.Prazo,
                        AnoNumero = item.Numero.ToString("0000") + "/" + item.Ano.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Auto_Infracao_Queimada_Struct Retorna_AutoInfracao_Queimada(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Auto_Infracao_Queimada
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu
                           where t.Ano_multa == Ano && t.Numero_multa == Numero select new {
                               Ano = t.Ano_multa, Numero = t.Numero_multa, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo,
                               Endereco_entrega = t.Endereco_entrega, Endereco_prop = t.Endereco_prop, Endereco_Infracao = t.Endereco_infracao, Usuario_Nome = u.Nomecompleto, Inscricao = t.Inscricao,
                               t.Nome2, t.Codigo_cidadao, t.Codigo_cidadao2, t.Cpf, t.Rg, t.Cpf2, t.Rg2, t.Endereco_entrega2, t.Endereco_prop2
                           }).FirstOrDefault();
                Auto_Infracao_Queimada_Struct reg = null;
                if (Sql != null) {
                    reg = new Auto_Infracao_Queimada_Struct() {
                        Ano_Multa = Sql.Ano,
                        Numero_Multa = Sql.Numero,
                        AnoNumero = Sql.Numero.ToString("0000") + "/" + Sql.Ano.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Prazo = Sql.Prazo,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                    };
                }
                return reg;
            }
        }

        public bool Existe_Auto_Infracao_Obra(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Auto_Infracao_Obra
                           where i.Ano_auto == Ano && i.Numero_auto == Numero select i.Numero_notificacao).FirstOrDefault();
                if (reg == 0)
                    return false;
                else
                    return true;
            }
        }

        public Exception Incluir_auto_infracao_Obra(Auto_infracao_obra Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_auto };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_auto };
                Parametros[2] = new SqlParameter { ParameterName = "@ano_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_notificacao };
                Parametros[3] = new SqlParameter { ParameterName = "@numero_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_notificacao };
                Parametros[4] = new SqlParameter { ParameterName = "@data_notificacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_notificacao };
                Parametros[5] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };

                db.Database.ExecuteSqlCommand("INSERT INTO auto_infracao_obra(ano_auto,numero_auto,ano_notificacao,numero_notificacao,data_notificacao,data_cadastro,userid) " +
                                              "VALUES(@ano_auto,@numero_auto,@ano_notificacao,@numero_notificacao,@data_notificacao,@data_cadastro,@userid)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Auto_Infracao_Obra_Struct> Lista_Auto_Infracao_Obra(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Obra
                           join n in db.Notificacao_Obra on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           where a.Ano_auto == Ano
                           orderby a.Numero_notificacao select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome
                           }).ToList();
                List<Auto_Infracao_Obra_Struct> Lista = new List<Auto_Infracao_Obra_Struct>();
                foreach (var item in Sql) {
                    Auto_Infracao_Obra_Struct reg = new Auto_Infracao_Obra_Struct() {
                        Ano_Auto = item.AnoAuto,
                        Numero_Auto = item.NumeroAuto,
                        Ano_Notificacao = item.AnoNot,
                        Numero_Notificacao = item.NumeroNot,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Data_Notificacao = item.Data_Notificaao,
                        Userid = item.Usuario,
                        AnoNumero = item.NumeroNot.ToString("0000") + "/" + item.AnoNot.ToString(),
                        AnoNumeroAuto = item.NumeroAuto.ToString("0000") + "/" + item.AnoAuto.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Auto_Infracao_Obra_Struct Retorna_Auto_Infracao_Obra(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Obra
                           join n in db.Notificacao_Obra on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           join u in db.Usuario on a.Userid equals u.Id into tu from u in tu
                           where a.Ano_auto == Ano && a.Numero_auto == Numero select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome, Endereco_entrega = n.Endereco_entrega, Endereco_prop = n.Endereco_prop, Endereco_Infracao = n.Endereco_infracao,
                               Usuario_Nome = u.Nomecompleto, Inscricao = n.Inscricao, n.Nome2, n.Codigo_cidadao, n.Codigo_cidadao2, n.Cpf, n.Rg, n.Cpf2, n.Rg2, n.Endereco_entrega2, n.Endereco_prop2

                           }).FirstOrDefault();
                Auto_Infracao_Obra_Struct reg = null;
                if (Sql != null) {
                    reg = new Auto_Infracao_Obra_Struct() {
                        Ano_Auto = Sql.AnoAuto,
                        Numero_Auto = Sql.NumeroAuto,
                        Ano_Notificacao = Sql.AnoNot,
                        Numero_Notificacao = Sql.NumeroNot,
                        AnoNumero = Sql.NumeroNot.ToString("0000") + "/" + Sql.AnoNot.ToString(),
                        AnoNumeroAuto = Sql.NumeroAuto.ToString("0000") + "/" + Sql.AnoAuto.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Data_Notificacao = Sql.Data_Notificaao,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                    };
                }
                return reg;
            }
        }

        public List<int> Lista_Terrenos_Cip() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 360;
                //var subselect = (from b in db.Areas select b.Codreduzido).ToList();
                //var result = (from c in db.Cadimob where c.Inativo==false && c.Imune==false  && c.Cip==false && !subselect.Contains(c.Codreduzido) select c.Codreduzido).ToList();
                List<int> Lista = new List<int>();
                var result = db.SpLista_Terreno_Cip.SqlQuery("EXEC spLista_Terreno_Cip").ToList();
                foreach (var item in result) {
                    Lista.Add(Convert.ToInt32(item.Codreduzido));
                }

                return Lista;
            }
        }

        public Imovel_Full Dados_Imovel_Full(int nCodigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Cadimob
                           join c in db.Condominio on i.Codcondominio equals c.Cd_codigo into ic from c in ic.DefaultIfEmpty()
                           join b in db.Benfeitoria on i.Dt_codbenf equals b.Codbenfeitoria into ib from b in ib.DefaultIfEmpty()
                           join p in db.Pedologia on i.Dt_codpedol equals p.Codpedologia into ip from p in ip.DefaultIfEmpty()
                           join t in db.Topografia on i.Dt_codtopog equals t.Codtopografia into it from t in it.DefaultIfEmpty()
                           join s in db.Situacao on i.Dt_codsituacao equals s.Codsituacao into ist from s in ist.DefaultIfEmpty()
                           join cp in db.Categprop on i.Dt_codcategprop equals cp.Codcategprop into icp from cp in icp.DefaultIfEmpty()
                           join u in db.Usoterreno on i.Dt_codusoterreno equals u.Codusoterreno into iu from u in iu.DefaultIfEmpty()
                           where i.Codreduzido == nCodigo
                           select new ImovelStruct {
                               Codigo = i.Codreduzido, Distrito = i.Distrito, Setor = i.Setor, Quadra = i.Quadra, Lote = i.Lote, Seq = i.Seq,
                               Unidade = i.Unidade, SubUnidade = i.Subunidade, NomeCondominio = c.Cd_nomecond, Imunidade = i.Imune, TipoMat = i.Tipomat, NumMatricula = i.Nummat,
                               Numero = i.Li_num, Complemento = i.Li_compl, QuadraOriginal = i.Li_quadras, LoteOriginal = i.Li_lotes, ResideImovel = i.Resideimovel, Inativo = i.Inativo,
                               FracaoIdeal = i.Dt_fracaoideal, Area_Terreno = i.Dt_areaterreno, Benfeitoria = i.Dt_codbenf, Categoria = i.Dt_codcategprop, Pedologia = i.Dt_codpedol, Topografia = i.Dt_codtopog,
                               Uso_terreno = i.Dt_codusoterreno, Situacao = i.Dt_codsituacao, EE_TipoEndereco = i.Ee_tipoend, Benfeitoria_Nome = b.Descbenfeitoria, Pedologia_Nome = p.Descpedologia,
                               Topografia_Nome = t.Desctopografia, Situacao_Nome = s.Descsituacao, Categoria_Nome = cp.Desccategprop, Uso_terreno_Nome = u.Descusoterreno, CodigoCondominio = c.Cd_codigo
                           }).FirstOrDefault();

                Imovel_Full row = new Imovel_Full();
                if (reg == null)
                    return row;
                row.Codigo = nCodigo;
                row.Inscricao = reg.Distrito.ToString() + "." + reg.Setor.ToString("00") + "." + reg.Quadra.ToString("0000") + "." + reg.Lote.ToString("00000") + "." + reg.Seq.ToString("00") + "." + reg.Unidade.ToString("00") + "." + reg.SubUnidade.ToString("000");
                row.Condominio_Codigo = reg.CodigoCondominio == null ? 999 : (int)reg.CodigoCondominio;
                row.Condominio_Nome = reg.CodigoCondominio==null || reg.CodigoCondominio==999 ? "N/A": row.Condominio_Nome;
                row.Imunidade = reg.Imunidade == null ? false : Convert.ToBoolean(reg.Imunidade);
                row.Cip = reg.Cip == null ? false : Convert.ToBoolean(reg.Cip);
                row.ResideImovel = reg.ResideImovel == null ? false : Convert.ToBoolean(reg.ResideImovel);
                row.Inativo = reg.Inativo == null ? false : Convert.ToBoolean(reg.Inativo);
                if (reg.TipoMat == null || reg.TipoMat == "M")
                    row.TipoMat = "M";
                else
                    row.TipoMat = "T";
                row.NumMatricula = reg.NumMatricula==null?0:(long)reg.NumMatricula;
                row.QuadraOriginal = reg.QuadraOriginal == null ? "N/D" : reg.QuadraOriginal.ToString();
                row.LoteOriginal = reg.LoteOriginal == null ? "N/D" : reg.LoteOriginal.ToString();
                row.FracaoIdeal = reg.FracaoIdeal;
                row.Area_Terreno = reg.Area_Terreno;
                row.Benfeitoria =(short) reg.Benfeitoria;
                row.Benfeitoria_Nome = reg.Benfeitoria_Nome;
                row.Categoria =(short) reg.Categoria;
                row.Categoria_Nome = reg.Categoria_Nome;
                row.Pedologia = (short)reg.Pedologia;
                row.Pedologia_Nome = reg.Pedologia_Nome;
                row.Situacao = (short)reg.Situacao;
                row.Situacao_Nome = reg.Situacao_Nome;
                row.Topografia = (short)reg.Topografia;
                row.Topografia_Nome = reg.Topografia_Nome;
                row.Uso_terreno = (short)reg.Uso_terreno;
                row.Uso_terreno_Nome = reg.Uso_terreno_Nome;
                row.EE_TipoEndereco = (short)reg.EE_TipoEndereco;

                row.Endereco_Imovel = Dados_Endereco(nCodigo, TipoEndereco.Local);
                if (reg.EE_TipoEndereco == 0)
                    row.Endereco_Entrega = row.Endereco_Imovel;
                else {
                    if (reg.EE_TipoEndereco == 1) {
                        row.Endereco_Entrega = Dados_Endereco(nCodigo, TipoEndereco.Proprietario);
                    } else {
                        row.Endereco_Entrega = Dados_Endereco(nCodigo, TipoEndereco.Entrega);
                    }
                }

                row.Lista_Proprietario = Lista_Proprietario(nCodigo);
                row.Lista_Testada = Lista_Testada(nCodigo);
                row.Lista_Area = Lista_Area(nCodigo);


                return row;
            }
        }

        public Exception Incluir_Notificacao_Habitese(Notificacao_habitese Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[22];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_not };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_not };
                Parametros[2] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco_infracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_infracao };
                Parametros[5] = new SqlParameter { ParameterName = "@endereco_prop", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop };
                Parametros[6] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[7] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[9] = new SqlParameter { ParameterName = "@prazo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Prazo };
                Parametros[10] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[11] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[12] = new SqlParameter { ParameterName = "@nome2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome2 ?? "" };
                Parametros[13] = new SqlParameter { ParameterName = "@codigo_cidadao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao };
                Parametros[14] = new SqlParameter { ParameterName = "@codigo_cidadao2", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao2 };
                Parametros[15] = new SqlParameter { ParameterName = "@cpf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf };
                Parametros[16] = new SqlParameter { ParameterName = "@rg", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg ?? "" };
                Parametros[17] = new SqlParameter { ParameterName = "@cpf2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf2 ?? "" };
                Parametros[18] = new SqlParameter { ParameterName = "@rg2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg2 ?? "" };
                Parametros[19] = new SqlParameter { ParameterName = "@endereco_prop2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop2 ?? "" };
                Parametros[20] = new SqlParameter { ParameterName = "@endereco_entrega2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega2 ?? "" };
                Parametros[21] = new SqlParameter { ParameterName = "@projeto", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Projeto ?? "" };

                db.Database.ExecuteSqlCommand("INSERT INTO notificacao_habitese(ano_not,numero_not,codigo,situacao,endereco_infracao,endereco_prop,endereco_entrega,nome,inscricao,prazo,data_cadastro," +
                                              "userid,nome2,codigo_cidadao,codigo_cidadao2,cpf,rg,cpf2,rg2,endereco_prop2,endereco_entrega2,projeto) " +
                                              " VALUES(@ano_not,@numero_not,@codigo,@situacao,@endereco_infracao,@endereco_prop,@endereco_entrega,@nome,@inscricao,@prazo,@data_cadastro,@userid," +
                                              "@nome2,@codigo_cidadao,@codigo_cidadao2,@cpf,@rg,@cpf2,@rg2,@endereco_prop2,@endereco_entrega2,@projeto)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Notificacao_Habitese(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Notificacao_Habitese
                           where i.Ano_not == Ano && i.Numero_not == Numero select i.Inscricao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
            }
        }

        public List<Notificacao_Habitese_Struct> Lista_Notificacao_Habitese(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Habitese
                           where t.Ano_not == Ano
                           orderby t.Numero_not select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo
                           }).ToList();
                List<Notificacao_Habitese_Struct> Lista = new List<Notificacao_Habitese_Struct>();
                foreach (var item in Sql) {
                    Notificacao_Habitese_Struct reg = new Notificacao_Habitese_Struct() {
                        Ano_Notificacao = item.Ano,
                        Numero_Notificacao = item.Numero,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Userid = item.Usuario,
                        Situacao = item.Situacao,
                        Prazo = item.Prazo,
                        AnoNumero = item.Numero.ToString("0000") + "/" + item.Ano.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Notificacao_Habitese_Struct Retorna_Notificacao_Habitese(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Habitese
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu
                           where t.Ano_not == Ano && t.Numero_not == Numero select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo,t.Projeto,
                               Endereco_entrega = t.Endereco_entrega, Endereco_prop = t.Endereco_prop, Endereco_Infracao = t.Endereco_infracao, Usuario_Nome = u.Nomecompleto, Inscricao = t.Inscricao,
                               t.Nome2, t.Codigo_cidadao, t.Codigo_cidadao2, t.Cpf, t.Rg, t.Cpf2, t.Rg2, t.Endereco_entrega2, t.Endereco_prop2
                           }).FirstOrDefault();
                Notificacao_Habitese_Struct reg = null;
                if (Sql != null) {
                    reg = new Notificacao_Habitese_Struct() {
                        Ano_Notificacao = Sql.Ano,
                        Numero_Notificacao = Sql.Numero,
                        AnoNumero = Sql.Numero.ToString("0000") + "/" + Sql.Ano.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Prazo = Sql.Prazo,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2,
                        Projeto=Sql.Projeto
                    };
                }
                return reg;
            }
        }

        public bool Existe_Auto_Infracao_Habitese(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Auto_Infracao_Habitese
                           where i.Ano_auto == Ano && i.Numero_auto == Numero select i.Numero_notificacao).FirstOrDefault();
                if (reg == 0)
                    return false;
                else
                    return true;
            }
        }

        public Exception Incluir_auto_infracao_Habitese(Auto_infracao_habitese Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_auto };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_auto };
                Parametros[2] = new SqlParameter { ParameterName = "@ano_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_notificacao };
                Parametros[3] = new SqlParameter { ParameterName = "@numero_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_notificacao };
                Parametros[4] = new SqlParameter { ParameterName = "@data_notificacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_notificacao };
                Parametros[5] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };

                db.Database.ExecuteSqlCommand("INSERT INTO auto_infracao_habitese(ano_auto,numero_auto,ano_notificacao,numero_notificacao,data_notificacao,data_cadastro,userid) " +
                                              "VALUES(@ano_auto,@numero_auto,@ano_notificacao,@numero_notificacao,@data_notificacao,@data_cadastro,@userid)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Auto_Infracao_Habitese_Struct> Lista_Auto_Infracao_Habitese(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Habitese
                           join n in db.Notificacao_Habitese on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           where a.Ano_auto == Ano
                           orderby a.Numero_notificacao select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome
                           }).ToList();
                List<Auto_Infracao_Habitese_Struct> Lista = new List<Auto_Infracao_Habitese_Struct>();
                foreach (var item in Sql) {
                    Auto_Infracao_Habitese_Struct reg = new Auto_Infracao_Habitese_Struct() {
                        Ano_Auto = item.AnoAuto,
                        Numero_Auto = item.NumeroAuto,
                        Ano_Notificacao = item.AnoNot,
                        Numero_Notificacao = item.NumeroNot,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Data_Notificacao = item.Data_Notificaao,
                        Userid = item.Usuario,
                        AnoNumero = item.NumeroNot.ToString("0000") + "/" + item.AnoNot.ToString(),
                        AnoNumeroAuto = item.NumeroAuto.ToString("0000") + "/" + item.AnoAuto.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Auto_Infracao_Habitese_Struct Retorna_Auto_Infracao_Habitese(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Habitese
                           join n in db.Notificacao_Habitese on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           join u in db.Usuario on a.Userid equals u.Id into tu from u in tu
                           where a.Ano_auto == Ano && a.Numero_auto == Numero select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,n.Projeto,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome, Endereco_entrega = n.Endereco_entrega, Endereco_prop = n.Endereco_prop, Endereco_Infracao = n.Endereco_infracao,
                               Usuario_Nome = u.Nomecompleto, Inscricao = n.Inscricao, n.Nome2, n.Codigo_cidadao, n.Codigo_cidadao2, n.Cpf, n.Rg, n.Cpf2, n.Rg2, n.Endereco_entrega2, n.Endereco_prop2

                           }).FirstOrDefault();
                Auto_Infracao_Habitese_Struct reg = null;
                if (Sql != null) {
                    reg = new Auto_Infracao_Habitese_Struct() {
                        Ano_Auto = Sql.AnoAuto,
                        Numero_Auto = Sql.NumeroAuto,
                        Ano_Notificacao = Sql.AnoNot,
                        Numero_Notificacao = Sql.NumeroNot,
                        AnoNumero = Sql.NumeroNot.ToString("0000") + "/" + Sql.AnoNot.ToString(),
                        AnoNumeroAuto = Sql.NumeroAuto.ToString("0000") + "/" + Sql.AnoAuto.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Data_Notificacao = Sql.Data_Notificaao,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Projeto=Sql.Projeto
                    };
                }
                return reg;
            }
        }

        public bool Existe_Notificacao_Passeio(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Notificacao_Passeio
                           where i.Ano_not == Ano && i.Numero_not == Numero select i.Inscricao).FirstOrDefault();
                if (string.IsNullOrEmpty(reg))
                    return false;
                else
                    return true;
            }
        }

        public List<Notificacao_Passeio_Struct> Lista_Notificacao_Passeio(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Passeio
                           where t.Ano_not == Ano
                           orderby t.Numero_not select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo
                           }).ToList();
                List<Notificacao_Passeio_Struct> Lista = new List<Notificacao_Passeio_Struct>();
                foreach (var item in Sql) {
                    Notificacao_Passeio_Struct reg = new Notificacao_Passeio_Struct() {
                        Ano_Notificacao = item.Ano,
                        Numero_Notificacao = item.Numero,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Userid = item.Usuario,
                        Situacao = item.Situacao,
                        Prazo = item.Prazo,
                        AnoNumero = item.Numero.ToString("0000") + "/" + item.Ano.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Notificacao_Passeio_Struct Retorna_Notificacao_Passeio(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Notificacao_Terreno
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu
                           where t.Ano_not == Ano && t.Numero_not == Numero select new {
                               Ano = t.Ano_not, Numero = t.Numero_not, Codigo = t.Codigo, Data_Cadastro = t.Data_cadastro, Usuario = t.Userid, Situacao = t.Situacao, Nome = t.Nome, Prazo = t.Prazo,
                               Endereco_entrega = t.Endereco_entrega, Endereco_prop = t.Endereco_prop, Endereco_Infracao = t.Endereco_infracao, Usuario_Nome = u.Nomecompleto, Inscricao = t.Inscricao,
                               t.Nome2, t.Codigo_cidadao, t.Codigo_cidadao2, t.Cpf, t.Rg, t.Cpf2, t.Rg2, t.Endereco_entrega2, t.Endereco_prop2
                           }).FirstOrDefault();
                Notificacao_Passeio_Struct reg = null;
                if (Sql != null) {
                    reg = new Notificacao_Passeio_Struct() {
                        Ano_Notificacao = Sql.Ano,
                        Numero_Notificacao = Sql.Numero,
                        AnoNumero = Sql.Numero.ToString("0000") + "/" + Sql.Ano.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Prazo = Sql.Prazo,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Rg2 = Sql.Rg2
                    };
                }
                return reg;
            }
        }

        public Exception Incluir_Notificacao_Passeio(Notificacao_Passeio Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[22];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_not };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_not", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_not };
                Parametros[2] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Situacao };
                Parametros[4] = new SqlParameter { ParameterName = "@endereco_infracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_infracao };
                Parametros[5] = new SqlParameter { ParameterName = "@endereco_prop", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop };
                Parametros[6] = new SqlParameter { ParameterName = "@endereco_entrega", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega };
                Parametros[7] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[9] = new SqlParameter { ParameterName = "@prazo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Prazo };
                Parametros[10] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_cadastro };
                Parametros[11] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[12] = new SqlParameter { ParameterName = "@nome2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome2 ?? "" };
                Parametros[13] = new SqlParameter { ParameterName = "@codigo_cidadao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao };
                Parametros[14] = new SqlParameter { ParameterName = "@codigo_cidadao2", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo_cidadao2 };
                Parametros[15] = new SqlParameter { ParameterName = "@cpf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf };
                Parametros[16] = new SqlParameter { ParameterName = "@rg", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg ?? "" };
                Parametros[17] = new SqlParameter { ParameterName = "@cpf2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cpf2 ?? "" };
                Parametros[18] = new SqlParameter { ParameterName = "@rg2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Rg2 ?? "" };
                Parametros[19] = new SqlParameter { ParameterName = "@endereco_prop2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_prop2 ?? "" };
                Parametros[20] = new SqlParameter { ParameterName = "@endereco_entrega2", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Endereco_entrega2 ?? "" };
                Parametros[21] = new SqlParameter { ParameterName = "@projeto", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Projeto ?? "" };

                db.Database.ExecuteSqlCommand("INSERT INTO notificacao_passeio(ano_not,numero_not,codigo,situacao,endereco_infracao,endereco_prop,endereco_entrega,nome,inscricao,prazo,data_cadastro," +
                                              "userid,nome2,codigo_cidadao,codigo_cidadao2,cpf,rg,cpf2,rg2,endereco_prop2,endereco_entrega2,projeto) " +
                                              " VALUES(@ano_not,@numero_not,@codigo,@situacao,@endereco_infracao,@endereco_prop,@endereco_entrega,@nome,@inscricao,@prazo,@data_cadastro,@userid," +
                                              "@nome2,@codigo_cidadao,@codigo_cidadao2,@cpf,@rg,@cpf2,@rg2,@endereco_prop2,@endereco_entrega2,@projeto)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_auto_infracao_Passeio(Auto_infracao_passeio Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@ano_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_auto };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_auto", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_auto };
                Parametros[2] = new SqlParameter { ParameterName = "@ano_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Ano_notificacao };
                Parametros[3] = new SqlParameter { ParameterName = "@numero_notificacao", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numero_notificacao };
                Parametros[4] = new SqlParameter { ParameterName = "@data_notificacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_notificacao };
                Parametros[5] = new SqlParameter { ParameterName = "@data_cadastro", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };

                db.Database.ExecuteSqlCommand("INSERT INTO auto_infracao_passeio(ano_auto,numero_auto,ano_notificacao,numero_notificacao,data_notificacao,data_cadastro,userid) " +
                                              "VALUES(@ano_auto,@numero_auto,@ano_notificacao,@numero_notificacao,@data_notificacao,@data_cadastro,@userid)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Auto_Infracao_Passeio_Struct> Lista_Auto_Infracao_Passeio(int Ano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Passeio
                           join n in db.Notificacao_Passeio on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           where a.Ano_auto == Ano
                           orderby a.Numero_notificacao select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome
                           }).ToList();
                List<Auto_Infracao_Passeio_Struct> Lista = new List<Auto_Infracao_Passeio_Struct>();
                foreach (var item in Sql) {
                    Auto_Infracao_Passeio_Struct reg = new Auto_Infracao_Passeio_Struct() {
                        Ano_Auto = item.AnoAuto,
                        Numero_Auto = item.NumeroAuto,
                        Ano_Notificacao = item.AnoNot,
                        Numero_Notificacao = item.NumeroNot,
                        Codigo_Imovel = item.Codigo,
                        Data_Cadastro = item.Data_Cadastro,
                        Data_Notificacao = item.Data_Notificaao,
                        Userid = item.Usuario,
                        AnoNumero = item.NumeroNot.ToString("0000") + "/" + item.AnoNot.ToString(),
                        AnoNumeroAuto = item.NumeroAuto.ToString("0000") + "/" + item.AnoAuto.ToString(),
                        Nome_Proprietario = item.Nome
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public Auto_Infracao_Passeio_Struct Retorna_Auto_Infracao_Passeio(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Auto_Infracao_Passeio
                           join n in db.Notificacao_Passeio on new { p1 = a.Ano_notificacao, p2 = a.Numero_notificacao } equals new { p1 = n.Ano_not, p2 = n.Numero_not } into an from n in an.DefaultIfEmpty()
                           join u in db.Usuario on a.Userid equals u.Id into tu from u in tu
                           where a.Ano_auto == Ano && a.Numero_auto == Numero select new {
                               AnoAuto = a.Ano_auto, NumeroAuto = a.Numero_auto, AnoNot = a.Ano_notificacao, NumeroNot = a.Numero_notificacao, Codigo = n.Codigo, Data_Notificaao = a.Data_notificacao, n.Projeto,
                               Data_Cadastro = a.Data_cadastro, Usuario = a.Userid, Nome = n.Nome, Endereco_entrega = n.Endereco_entrega, Endereco_prop = n.Endereco_prop, Endereco_Infracao = n.Endereco_infracao,
                               Usuario_Nome = u.Nomecompleto, Inscricao = n.Inscricao, n.Nome2, n.Codigo_cidadao, n.Codigo_cidadao2, n.Cpf, n.Rg, n.Cpf2, n.Rg2, n.Endereco_entrega2, n.Endereco_prop2

                           }).FirstOrDefault();
                Auto_Infracao_Passeio_Struct reg = null;
                if (Sql != null) {
                    reg = new Auto_Infracao_Passeio_Struct() {
                        Ano_Auto = Sql.AnoAuto,
                        Numero_Auto = Sql.NumeroAuto,
                        Ano_Notificacao = Sql.AnoNot,
                        Numero_Notificacao = Sql.NumeroNot,
                        AnoNumero = Sql.NumeroNot.ToString("0000") + "/" + Sql.AnoNot.ToString(),
                        AnoNumeroAuto = Sql.NumeroAuto.ToString("0000") + "/" + Sql.AnoAuto.ToString(),
                        Codigo_Imovel = Sql.Codigo,
                        Data_Cadastro = Sql.Data_Cadastro,
                        Data_Notificacao = Sql.Data_Notificaao,
                        Userid = Sql.Usuario,
                        Nome_Proprietario = Sql.Nome,
                        Endereco_Entrega = Sql.Endereco_entrega,
                        Endereco_entrega2 = Sql.Endereco_entrega2,
                        Endereco_Local = Sql.Endereco_Infracao,
                        Endereco_Prop = Sql.Endereco_prop,
                        Endereco_prop2 = Sql.Endereco_prop2,
                        UsuarioNome = Sql.Usuario_Nome,
                        Inscricao = Sql.Inscricao,
                        Nome_Proprietario2 = Sql.Nome2,
                        Codigo_cidadao = Sql.Codigo_cidadao,
                        Codigo_cidadao2 = Sql.Codigo_cidadao2,
                        Cpf = Sql.Cpf,
                        Cpf2 = Sql.Cpf2,
                        Rg = Sql.Rg,
                        Projeto = Sql.Projeto
                    };
                }
                return reg;
            }
        }

        public bool Existe_Auto_Infracao_Passeio(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Auto_Infracao_Passeio
                           where i.Ano_auto == Ano && i.Numero_auto == Numero select i.Numero_notificacao).FirstOrDefault();
                if (reg == 0)
                    return false;
                else
                    return true;
            }
        }

    }//end class
}


