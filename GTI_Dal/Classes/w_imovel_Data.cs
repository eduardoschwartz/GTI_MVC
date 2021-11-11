using GTI_Models.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace GTI_Dal.Classes {
    public class w_imovel_Data {
        private readonly string _connection;

        public w_imovel_Data(string sConnection) {
            _connection = sConnection;
        }

        public Exception Insert_W_Imovel_Main(W_Imovel_Main Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[17];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@cip", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Cip };
                Parametros[3] = new SqlParameter { ParameterName = "@imune", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Imune };
                Parametros[4] = new SqlParameter { ParameterName = "@conjugado", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Conjugado };
                Parametros[5] = new SqlParameter { ParameterName = "@reside", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Reside };
                Parametros[6] = new SqlParameter { ParameterName = "@area_terreno", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Area_Terreno };
                Parametros[7] = new SqlParameter { ParameterName = "@topografia", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Topografia };
                Parametros[8] = new SqlParameter { ParameterName = "@pedologia", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Pedologia };
                Parametros[9] = new SqlParameter { ParameterName = "@benfeitoria", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Benfeitoria };
                Parametros[10] = new SqlParameter { ParameterName = "@categoria", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Categoria };
                Parametros[11] = new SqlParameter { ParameterName = "@usoterreno", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Usoterreno };
                Parametros[12] = new SqlParameter { ParameterName = "@situacao", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Situacao };
                Parametros[13] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[14] = new SqlParameter { ParameterName = "@inscricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Inscricao };
                Parametros[15] = new SqlParameter { ParameterName = "@condominio", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Condominio };
                Parametros[16] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Alteracao };

                db.Database.ExecuteSqlCommand("INSERT INTO w_imovel_main(guid,codigo,cip,imune,conjugado,reside,area_terreno,topografia,pedologia,benfeitoria,categoria,usoterreno,situacao,userid,inscricao,condominio,data_alteracao) " +
                                              "VALUES(@guid,@codigo,@cip,@imune,@conjugado,@reside,@area_terreno,@topografia,@pedologia,@benfeitoria,@categoria,@usoterreno,@situacao,@userid,@inscricao,@condominio,@data_alteracao)", Parametros);
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
