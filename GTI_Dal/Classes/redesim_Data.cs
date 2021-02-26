using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GTI_Models.modelCore;

namespace GTI_Dal.Classes {
    public class Redesim_Data {
        private readonly string _connection;

        public Redesim_Data(string sConnection) {
            _connection = sConnection;
        }

        public Exception Incluir_Arquivo(Redesim_arquivo reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[2];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Tipo };
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_arquivo(guid,tipo) VALUES(@guid,@tipo)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Registro(Redesim_Registro reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[11];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Arquivo };
                Parametros[2] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[3] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                Parametros[4] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                if (reg.Complemento == "")
                    Parametros[5] = new SqlParameter { ParameterName = "@complemento", SqlValue = DBNull.Value };
                else
                    Parametros[5] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                Parametros[6] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.Int, SqlValue = reg.Cep };
                Parametros[7] = new SqlParameter { ParameterName = "@matrizfilial", SqlDbType = SqlDbType.VarChar, SqlValue = reg.MatrizFilial };
                Parametros[8] = new SqlParameter { ParameterName = "@natureza_juridica", SqlDbType = SqlDbType.Int, SqlValue = reg.Natureza_Juridica };
                Parametros[9] = new SqlParameter { ParameterName = "@porte_empresa", SqlDbType = SqlDbType.Int, SqlValue = reg.Porte_Empresa };
                Parametros[10] = new SqlParameter { ParameterName = "@cnae_principal", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnae_Principal };
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_registro(protocolo,arquivo,cnpj,razao_social,numero,complemento,cep,matrizfilial,natureza_juridica,porte_empresa,cnae_principal) " +
                                              " VALUES(@protocolo,@arquivo,@cnpj,@razao_social,@numero,@complemento,@cep,@matrizfilial,@natureza_juridica,@porte_empresa,@cnae_principal)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Viabilidade(Redesim_Viabilidade reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[9];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Arquivo };
                Parametros[2] = new SqlParameter { ParameterName = "@analise", SqlDbType = SqlDbType.Int, SqlValue = reg.Analise };
                if(reg.Nire=="")
                    Parametros[3] = new SqlParameter { ParameterName = "@nire",  SqlValue = DBNull.Value };
                else
                    Parametros[3] = new SqlParameter { ParameterName = "@nire", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Nire };
                Parametros[4] = new SqlParameter { ParameterName = "@empresaestabelecida", SqlDbType = SqlDbType.Bit, SqlValue = reg.EmpresaEstabelecida };
                Parametros[5] = new SqlParameter { ParameterName = "@dataprotocolo", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.DataProtocolo };
                Parametros[6] = new SqlParameter { ParameterName = "@numeroinscricaoimovel", SqlDbType = SqlDbType.Int, SqlValue = reg.NumeroInscricaoImovel };
                Parametros[7] = new SqlParameter { ParameterName = "@areaimovel", SqlDbType = SqlDbType.Decimal, SqlValue = reg.AreaImovel };
                Parametros[8] = new SqlParameter { ParameterName = "@areaestabelecimento", SqlDbType = SqlDbType.Decimal, SqlValue = reg.AreaEstabelecimento };
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_viabilidade(protocolo,arquivo,analise,nire,empresaestabelecida,dataprotocolo,numeroinscricaoimovel," +
                    "areaimovel,areaestabelecimento) VALUES(@protocolo,@arquivo,@analise,@nire,@empresaestabelecida,@dataprotocolo,@numeroinscricaoimovel," +
                    "@areaimovel,@areaestabelecimento)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Registro(string Processo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Redesim_Registro where i.Protocolo == Processo select i.Razao_Social).FirstOrDefault();
                if (reg == null)
                    return false;
                else
                    return true;
            }
        }

        public bool Existe_Viabilidade(string Processo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Redesim_Viabilidade where i.Protocolo == Processo select i.Arquivo).FirstOrDefault();
                if (reg == null)
                    return false;
                else
                    return true;
            }
        }

        public bool Existe_Licenciamento(string Processo,DateTime DataSolicitacao) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Redesim_Licenciamento where i.Protocolo == Processo && i.Data_Solicitacao==DataSolicitacao  select i.Arquivo).FirstOrDefault();
                if (reg == null)
                    return false;
                else
                    return true;
            }
        }

        public List<Redesim_natureza_juridica> Lista_Natureza_Juridica() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Natureza_Juridica select c);
                return Sql.ToList();
            }
        }

        public int Incluir_Natureza_Juridica(string Name) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Redesim_Natureza_Juridica select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Redesim_Natureza_Juridica select c.Codigo).Max() + 1;

                try {
                    db.Database.ExecuteSqlCommand("INSERT redesim_natureza_juridica(codigo,nome) values(@codigo,@nome)",
                        new SqlParameter("@codigo", maxCod), new SqlParameter("@nome", Name));
                } catch (Exception ex) {
                    throw ex;
                }

                return maxCod;
            }
        }

        public List<Redesim_evento> Lista_Evento() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Evento select c);
                return Sql.ToList();
            }
        }

        public int Incluir_Evento(string Name) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Redesim_Evento select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Redesim_Evento select c.Codigo).Max() + 1;

                try {
                    db.Database.ExecuteSqlCommand("INSERT redesim_evento(codigo,nome) values(@codigo,@nome)",
                        new SqlParameter("@codigo", maxCod), new SqlParameter("@nome", Name));
                } catch (Exception ex) {
                    throw ex;
                }

                return maxCod;
            }
        }

        public void Incluir_Registro_Evento(string Protocolo, int[] ListaEvento) {
            using (GTI_Context db = new GTI_Context(_connection)) {

                foreach (int item in ListaEvento) {
                    try {
                        db.Database.ExecuteSqlCommand("INSERT redesim_registro_evento(protocolo,evento) values(@protocolo,@evento)",
                            new SqlParameter("@protocolo", Protocolo), new SqlParameter("@evento", item));
                    } catch  {

                    }
                }
            }
        }

        public List<Redesim_porte_empresa> Lista_Porte_Empresa() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Porte_Empresa select c);
                return Sql.ToList();
            }
        }

        public int Incluir_Porte_Empresa(string Name) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Redesim_Porte_Empresa select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Redesim_Porte_Empresa select c.Codigo).Max() + 1;
                try {
                    db.Database.ExecuteSqlCommand("INSERT redesim_porte_empresa(codigo,nome) values(@codigo,@nome)",
                        new SqlParameter("@codigo", maxCod), new SqlParameter("@nome", Name));
                } catch (Exception ex) {
                    throw ex;
                }

                return maxCod;
            }
        }

        public List<Redesim_forma_atuacao> Lista_Forma_Atuacao() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Forma_Atuacao select c);
                return Sql.ToList();
            }
        }

        public int Incluir_Forma_Atuacao(string Name) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Redesim_Forma_Atuacao select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Redesim_Forma_Atuacao select c.Codigo).Max() + 1;

                try {
                    db.Database.ExecuteSqlCommand("INSERT redesim_forma_atuacao(codigo,nome) values(@codigo,@nome)",
                        new SqlParameter("@codigo", maxCod), new SqlParameter("@nome", Name));
                } catch (Exception ex) {
                    throw ex;
                }

                return maxCod;
            }
        }

        public void Incluir_Registro_Forma_Atuacao(string Protocolo, int[] ListaForma) {
            using (GTI_Context db = new GTI_Context(_connection)) {

                foreach (int item in ListaForma) {
                    try {
                        db.Database.ExecuteSqlCommand("INSERT redesim_registro_forma_atuacao(protocolo,forma_atuacao) values(@protocolo,@forma_atuacao)",
                            new SqlParameter("@protocolo", Protocolo), new SqlParameter("@forma_atuacao", item));
                    } catch {

                    }
                }
            }
        }

        public void Incluir_Cnae(string Protocolo, string[] ListaCnae) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                foreach (string item in ListaCnae) {
                    
                    if (!string.IsNullOrEmpty(item)) {
                        try {
                            db.Database.ExecuteSqlCommand("INSERT redesim_cnae(protocolo,cnae) values(@protocolo,@cnae)",
                                new SqlParameter("@protocolo", Protocolo), new SqlParameter("@cnae", item));
                        } catch {

                        }
                    }
                }
            }
        }

        public List<Redesim_viabilidade_analise> Lista_Viabilidade_Analise() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Viabilidade_Analise select c);
                return Sql.ToList();
            }
        }

        public int Incluir_Viabilidade_Analise(string Name) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Redesim_Viabilidade_Analise select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Redesim_Viabilidade_Analise select c.Codigo).Max() + 1;
                try {
                    db.Database.ExecuteSqlCommand("INSERT redesim_viabilidade_analise(codigo,nome) values(@codigo,@nome)",
                        new SqlParameter("@codigo", maxCod), new SqlParameter("@nome", Name));
                } catch (Exception ex) {
                    throw ex;
                }
                return maxCod;
            }
        }

        public Exception Incluir_Licenciamento(Redesim_licenciamento reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[12];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@data_solicitacao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Data_Solicitacao };
                Parametros[2] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Arquivo };
                Parametros[3] = new SqlParameter { ParameterName = "@situacao_solicitacao", SqlDbType = SqlDbType.Int, SqlValue = reg.Situacao_Solicitacao };
                if(reg.Data_Validade==DateTime.MinValue)
                    Parametros[4] = new SqlParameter { ParameterName = "@data_validade",  SqlValue = DBNull.Value };
                else
                    Parametros[4] = new SqlParameter { ParameterName = "@data_validade", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Data_Validade };
                Parametros[5] = new SqlParameter { ParameterName = "@mei", SqlDbType = SqlDbType.Bit, SqlValue = reg.Mei };
                Parametros[6] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[7] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                Parametros[8] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                if (reg.Complemento == "")
                    Parametros[9] = new SqlParameter { ParameterName = "@complemento", SqlValue = DBNull.Value };
                else
                    Parametros[9] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                Parametros[10] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.Int, SqlValue = reg.Cep };
                Parametros[11] = new SqlParameter { ParameterName = "@cnae_principal", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnae_Principal };

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_licenciamento(protocolo,data_solicitacao,arquivo,situacao_solicitacao,data_validade,mei,cnpj,razao_social," +
                        "numero,complemento,cep,cnae_principal) VALUES(@protocolo,@data_solicitacao,@arquivo,@situacao_solicitacao,@data_validade,@mei,@cnpj,@razao_social," +
                        "@numero,@complemento,@cep,@cnae_principal)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Redesim_master> Lista_Master(int Ano,int Mes) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Master where c.Data_licenca.Year==Ano && c.Data_licenca.Month==Mes select c);
                return Sql.ToList();
            }
        }

        public Exception Incluir_Master(Redesim_master reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[11];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@data_licenca", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Data_licenca };
                if (string.IsNullOrEmpty( reg.Razao_Social))
                    Parametros[2] = new SqlParameter { ParameterName = "@razao_social", SqlValue = DBNull.Value };
                else
                    Parametros[2] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                if (string.IsNullOrEmpty( reg.Cnpj))
                    Parametros[3] = new SqlParameter { ParameterName = "@cnpj", SqlValue = DBNull.Value };
                else
                    Parametros[3] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[4] = new SqlParameter { ParameterName = "@logradouro", SqlDbType = SqlDbType.Int, SqlValue = reg.Logradouro };
                Parametros[5] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                if (string.IsNullOrEmpty(reg.Complemento))
                    Parametros[6] = new SqlParameter { ParameterName = "@complemento", SqlValue = DBNull.Value };
                else
                    Parametros[6] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                if (string.IsNullOrEmpty(reg.Cep))
                    Parametros[7] = new SqlParameter { ParameterName = "@cep", SqlValue = DBNull.Value };
                else
                    Parametros[7] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cep };
                if (string.IsNullOrEmpty(reg.Cnae_Principal))
                    Parametros[8] = new SqlParameter { ParameterName = "@cnae_principal", SqlValue = DBNull.Value };
                else
                    Parametros[8] = new SqlParameter { ParameterName = "@cnae_principal", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnae_Principal };
                Parametros[9] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.Int, SqlValue = reg.Inscricao };
                Parametros[10] = new SqlParameter { ParameterName = "@numero_imovel", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero_Imovel };
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_master(protocolo,data_licenca,razao_social,cnpj,logradouro,numero,complemento,cep,cnae_principal,inscricao,numero_imovel) " +
                        "VALUES(@protocolo,@data_licenca,@razao_social,@cnpj,@logradouro,@numero,@complemento,@cep,@cnae_principal,@inscricao,@numero_imovel)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Master(string Protocolo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Redesim_Master where i.Protocolo == Protocolo select i.Protocolo).FirstOrDefault();
                if (reg == null)
                    return false;
                else
                    return true;
            }
        }

        public Exception Atualizar_Master_Registro(Redesim_Registro reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Endereco_Data enderecoRepository = new Endereco_Data("GTIconnection");
                LogradouroStruct _log= enderecoRepository.Retorna_Logradour_Cep(Convert.ToInt32(reg.Cep));
                int _logradouro = (int)_log.CodLogradouro;

                object[] Parametros = new object[9];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                Parametros[2] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[3] = new SqlParameter { ParameterName = "@logradouro", SqlDbType = SqlDbType.Int, SqlValue = _logradouro };
                Parametros[4] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                if (string.IsNullOrEmpty(reg.Complemento))
                    Parametros[5] = new SqlParameter { ParameterName = "@complemento", SqlValue = DBNull.Value };
                else
                    Parametros[5] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                Parametros[6] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cep };
                Parametros[7] = new SqlParameter { ParameterName = "@cnae_principal", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnae_Principal };
                Parametros[8] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.Int, SqlValue = reg.Inscricao };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE redesim_master SET razao_social=@razao_social,cnpj=@cnpj,logradouro=@logradouro,numero=@numero,complemento=@complemento," +
                        "cep=@cep,cnae_principal=@cnae_principal,inscricao=@inscricao where protocolo=@protocolo", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Redesim_Registro Retorna_Registro(string Protocolo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Registro where c.Protocolo==Protocolo select c).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Atualizar_Master_Viabilidade(Redesim_Viabilidade reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@numero_imovel", SqlDbType = SqlDbType.Int, SqlValue = reg.NumeroInscricaoImovel };
                Parametros[2] = new SqlParameter { ParameterName = "@area_estabelecimento", SqlDbType = SqlDbType.Decimal, SqlValue = reg.AreaEstabelecimento };
                Parametros[3] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.Int, SqlValue = reg.Inscricao };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE redesim_master SET numero_imovel=@numero_imovel,area_estabelecimento=@area_estabelecimento,inscricao=@inscricao where protocolo=@protocolo", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Redesim_Viabilidade Retorna_Viabilidade(string Protocolo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Redesim_Viabilidade where c.Protocolo==Protocolo select c).FirstOrDefault();
                return Sql;
            }
        }



    }
}
