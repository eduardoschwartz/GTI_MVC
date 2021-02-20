using GTI_Models.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace GTI_Dal.Classes {
    public class Redesim_Data {
        private readonly string _connection;

        public Redesim_Data(string sConnection) {
            _connection = sConnection;
        }

        public Exception Incluir_Arquivo(Redesim_arquivo reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@periodode", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Periodode };
                Parametros[2] = new SqlParameter { ParameterName = "@periodoate", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Periodoate };
                Parametros[3] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Tipo };
                db.Database.ExecuteSqlCommand("INSERT INTO redesim_arquivo(guid,periodode,periodoate,tipo) " +
                                              " VALUES(@guid,@periodode,@periodoate,@tipo)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


    }
}
