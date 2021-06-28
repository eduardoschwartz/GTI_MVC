using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Mobreq_Data {
        private readonly string _connection;
        public Mobreq_Data(string sConnection) {
            _connection = sConnection;
        }

        public List<Mobreq_evento> Lista_Evento() {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Mobreq_Evento orderby c.Descricao select c);
                return Sql.ToList();
            }
        }

        public Exception Incluir_Mobreq_Main(Mobreq_main Reg) {
            using(var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[8];
                Parametros[0] = new SqlParameter { ParameterName = "@guid",SqlDbType = SqlDbType.VarChar,SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo",SqlDbType = SqlDbType.Int,SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@tipo",SqlDbType = SqlDbType.Int,SqlValue = Reg.Tipo };
                Parametros[3] = new SqlParameter { ParameterName = "@data_evento",SqlDbType = SqlDbType.SmallDateTime,SqlValue = Reg.Data_Evento };
                Parametros[4] = new SqlParameter { ParameterName = "@data_inclusao",SqlDbType = SqlDbType.SmallDateTime,SqlValue = Reg.Data_Inclusao };
                Parametros[5] = new SqlParameter { ParameterName = "@obs",SqlDbType = SqlDbType.VarChar,SqlValue = Reg.Obs };
                Parametros[6] = new SqlParameter { ParameterName = "@userid",SqlDbType = SqlDbType.Int,SqlValue = Reg.UserId };
                Parametros[7] = new SqlParameter { ParameterName = "@userprf",SqlDbType = SqlDbType.Bit,SqlValue = Reg.UserPrf };

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO mobreq_main(guid,Codigo,Tipo,data_evento,data_inclusao,obs,userid,userprf) " +
                                                  "VALUES(@guid,@Codigo,@Tipo,@data_evento,@data_inclusao,@obs,@userid,@userprf)",Parametros);
                } catch(Exception ex) {
                    return ex;
                }


                return null;
            }
        }


    }
}
