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
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@protocolo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Protocolo };
                Parametros[1] = new SqlParameter { ParameterName = "@arquivo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Arquivo };
                Parametros[2] = new SqlParameter { ParameterName = "@cnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cnpj };
                Parametros[3] = new SqlParameter { ParameterName = "@razao_social", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Razao_Social };
                Parametros[4] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                Parametros[5] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                Parametros[6] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Cep };
                db.Database.ExecuteSqlCommand("INSERT INTO redesim_registro(protocolo,arquivo,cnpj,razao_social,numero,complemento,cep) " +
                                              " VALUES(@protocolo,@arquivo,@cnpj,@razao_social,@numero,@complemento,@cep)", Parametros);
                try {
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


    }
}
