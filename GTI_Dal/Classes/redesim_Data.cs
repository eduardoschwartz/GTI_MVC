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
                db.Database.ExecuteSqlCommand("INSERT INTO redesim_arquivo(guid,tipo) " +
                                              " VALUES(@guid,@tipo)", Parametros);
                try {
                    db.SaveChanges();
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
                Parametros[6] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cep };
                Parametros[7] = new SqlParameter { ParameterName = "@matrizfilial", SqlDbType = SqlDbType.VarChar, SqlValue = reg.MatrizFilial };
                Parametros[8] = new SqlParameter { ParameterName = "@natureza_juridica", SqlDbType = SqlDbType.Int, SqlValue = reg.Natureza_Juridica };
                Parametros[9] = new SqlParameter { ParameterName = "@porte_empresa", SqlDbType = SqlDbType.Int, SqlValue = reg.Porte_Empresa };
                Parametros[10] = new SqlParameter { ParameterName = "@cnae_principal", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnae_Principal };
                db.Database.ExecuteSqlCommand("INSERT INTO redesim_registro(protocolo,arquivo,cnpj,razao_social,numero,complemento,cep,matrizfilial,natureza_juridica,porte_empresa,cnae_principal) " +
                                              " VALUES(@protocolo,@arquivo,@cnpj,@razao_social,@numero,@complemento,@cep,@matrizfilial,@natureza_juridica,@porte_empresa,@cnae_principal)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Viabilidade(Redesim_Viabilidade reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[15];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Arquivo };
                Parametros[2] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[3] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                Parametros[4] = new SqlParameter { ParameterName = "@analise", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Analise };
                if(reg.Nire=="")
                    Parametros[5] = new SqlParameter { ParameterName = "@nire",  SqlValue = DBNull.Value };
                else
                    Parametros[5] = new SqlParameter { ParameterName = "@nire", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Nire };
                Parametros[6] = new SqlParameter { ParameterName = "@empresaestabelecida", SqlDbType = SqlDbType.Bit, SqlValue = reg.EmpresaEstabelecida };
                Parametros[7] = new SqlParameter { ParameterName = "@dataprotocolo", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.DataProtocolo };
                Parametros[8] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cep };
                Parametros[9] = new SqlParameter { ParameterName = "@numeroinscricaoimovel", SqlDbType = SqlDbType.Int, SqlValue = reg.NumeroInscricaoImovel };
                if(reg.Complemento=="")
                    Parametros[10] = new SqlParameter { ParameterName = "@complemento",  SqlValue = DBNull.Value };
                else
                    Parametros[10] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                Parametros[11] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                Parametros[12] = new SqlParameter { ParameterName = "@tipounidade", SqlDbType = SqlDbType.VarChar, SqlValue = reg.TipoUnidade };
                Parametros[13] = new SqlParameter { ParameterName = "@areaimovel", SqlDbType = SqlDbType.Decimal, SqlValue = reg.AreaImovel };
                Parametros[14] = new SqlParameter { ParameterName = "@areaestabelecimento", SqlDbType = SqlDbType.Decimal, SqlValue = reg.AreaEstabelecimento };
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO redesim_viabilidade(protocolo,arquivo,cnpj,razao_social,analise,nire,empresaestabelecida,dataprotocolo,cep,numeroinscricaoimovel," +
                    "complemento,numero,tipounidade,areaimovel,areaestabelecimento) VALUES(@protocolo,@arquivo,@cnpj,@razao_social,@analise,@nire,@empresaestabelecida,@dataprotocolo,@cep,@numeroinscricaoimovel," +
                    "@complemento,@numero,@tipounidade,@areaimovel,@areaestabelecimento)", Parametros);
                    db.SaveChanges();
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
                var reg = (from i in db.Redesim_Viabilidade where i.Protocolo == Processo select i.Razao_Social).FirstOrDefault();
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
                    if (item.Trim() != "") {
                        try {
                            db.Database.ExecuteSqlCommand("INSERT redesim_cnae(protocolo,cnae) values(@protocolo,@cnae)",
                                new SqlParameter("@protocolo", Protocolo), new SqlParameter("@cnae", item));
                        } catch {

                        }
                    }
                }
            }
        }


    }
}
