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

                object[] Parametros = new object[9];
                Parametros[0] = new SqlParameter { ParameterName = "@guid",SqlDbType = SqlDbType.VarChar,SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo",SqlDbType = SqlDbType.Int,SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@tipo",SqlDbType = SqlDbType.Int,SqlValue = Reg.Tipo };
                Parametros[3] = new SqlParameter { ParameterName = "@data_evento",SqlDbType = SqlDbType.SmallDateTime,SqlValue = Reg.Data_Evento };
                Parametros[4] = new SqlParameter { ParameterName = "@data_inclusao",SqlDbType = SqlDbType.SmallDateTime,SqlValue = Reg.Data_Inclusao };
                Parametros[5] = new SqlParameter { ParameterName = "@obs",SqlDbType = SqlDbType.VarChar,SqlValue = Reg.Obs };
                Parametros[6] = new SqlParameter { ParameterName = "@userid",SqlDbType = SqlDbType.Int,SqlValue = Reg.UserId };
                Parametros[7] = new SqlParameter { ParameterName = "@userprf",SqlDbType = SqlDbType.Bit,SqlValue = Reg.UserPrf };
                Parametros[8] = new SqlParameter { ParameterName = "@situacao",SqlDbType = SqlDbType.Int,SqlValue = Reg.Situacao };

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO mobreq_main(guid,Codigo,Tipo,data_evento,data_inclusao,obs,userid,userprf,situacao) " +
                                                  "VALUES(@guid,@Codigo,@Tipo,@data_evento,@data_inclusao,@obs,@userid,@userprf,@situacao)",Parametros);
                } catch(Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Mobreq_main_Struct> Lista_Requerimentos(int AnoInclusao) {
            using(GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Mobreq_Main
                           join m in db.Mobiliario on c.Codigo equals m.Codigomob into cm from m in cm.DefaultIfEmpty()
                           join t in db.Mobreq_Evento on c.Tipo equals t.Codigo into ct from t in ct.DefaultIfEmpty()
                           join s in db.Mobreq_Situacao on c.Situacao equals s.Codigo into st from s in st.DefaultIfEmpty()
                           join u in db.Usuario on c.UserId2 equals u.Id into ut from u in ut.DefaultIfEmpty()
                           where c.Data_Inclusao.Year==AnoInclusao
                           orderby c.Data_Inclusao 
                           select new Mobreq_main_Struct {
                                Codigo=c.Codigo,Data_Evento=c.Data_Evento,Data_Inclusao=c.Data_Inclusao,CpfCnpj=m.Cnpj==null?m.Cpf:m.Cnpj,
                                Guid=c.Guid,Obs=c.Obs,Razao_Social= m.Razaosocial.Substring(0,27),Tipo_Codigo=c.Tipo,
                                Tipo_Nome=t.Descricao.Substring(0,30),UserId=c.UserId,UserPrf=c.UserPrf,Situacao_Codigo=c.Situacao,
                                Situacao_Nome=s.Descricao,UserId2_Codigo=c.UserId2,Data_Evento2=c.Data_Evento2,UserId2_Nome=u.Nomecompleto
                           });
                return Sql.ToList();
            }
        }

    }
}
