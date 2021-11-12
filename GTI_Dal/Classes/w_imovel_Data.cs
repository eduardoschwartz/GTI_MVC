using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GTI_Models.modelCore;

namespace GTI_Dal.Classes {
    public class w_imovel_Data {
        private readonly string _connection;

        public w_imovel_Data(string sConnection) {
            _connection = sConnection;
        }

        public Exception Insert_W_Imovel_Main(string Guid,int Codigo,int UserId) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[3] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = UserId };

                db.Database.ExecuteSqlCommand("INSERT INTO w_imovel_main(guid,codigo,data_alteracao,userid) VALUES(@guid,@codigo,@data_alteracao,@userid)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Main2(W_Imovel_Main Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[24];
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
                Parametros[15] = new SqlParameter { ParameterName = "@condominio", SqlDbType = SqlDbType.Int, SqlValue = Reg.Condominio };
                Parametros[16] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Alteracao };
                Parametros[17] = new SqlParameter { ParameterName = "@condominio_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Condominio_Nome };
                Parametros[18] = new SqlParameter { ParameterName = "@topografia_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Topografia_Nome };
                Parametros[19] = new SqlParameter { ParameterName = "@pedologia_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Pedologia_Nome };
                Parametros[20] = new SqlParameter { ParameterName = "@benfeitoria_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Benfeitoria_Nome };
                Parametros[21] = new SqlParameter { ParameterName = "@categoria_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Categoria_Nome };
                Parametros[22] = new SqlParameter { ParameterName = "@usoterreno_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usoterreno_Nome };
                Parametros[23] = new SqlParameter { ParameterName = "@situacao_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Situacao_Nome };

                db.Database.ExecuteSqlCommand("INSERT INTO w_imovel_main(guid,codigo,cip,imune,conjugado,reside,area_terreno,topografia,pedologia,benfeitoria,categoria,usoterreno,situacao,userid," +
                    "inscricao,condominio,data_alteracao,condominio_nome,topografia_nome,pedologia_nome,benfeitoria_nome,categoria_nome,usoterreno_nome,situacao_nome) " +
                                              "VALUES(@guid,@codigo,@cip,@imune,@conjugado,@reside,@area_terreno,@topografia,@pedologia,@benfeitoria,@categoria,@usoterreno,@situacao,@userid," +
                                              "@inscricao,@condominio,@data_alteracao,@condominio_nome,@topografia_nome,@pedologia_nome,@benfeitoria_nome,@categoria_nome,@usoterreno_nome,@situacao_nome)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Update_W_Imovel_Main(W_Imovel_Main Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[24];
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
                Parametros[15] = new SqlParameter { ParameterName = "@condominio", SqlDbType = SqlDbType.Int, SqlValue = Reg.Condominio };
                Parametros[16] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Alteracao };
                Parametros[17] = new SqlParameter { ParameterName = "@condominio_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Condominio_Nome };
                Parametros[18] = new SqlParameter { ParameterName = "@topografia_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Topografia_Nome };
                Parametros[19] = new SqlParameter { ParameterName = "@pedologia_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Pedologia_Nome };
                Parametros[20] = new SqlParameter { ParameterName = "@benfeitoria_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Benfeitoria_Nome };
                Parametros[21] = new SqlParameter { ParameterName = "@categoria_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Categoria_Nome };
                Parametros[22] = new SqlParameter { ParameterName = "@usoterreno_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usoterreno_Nome };
                Parametros[23] = new SqlParameter { ParameterName = "@situacao_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Situacao_Nome };

                db.Database.ExecuteSqlCommand("update w_imovel_main set cip=@cip,imune=@imune,conjugado=@conjugado,reside=@reside,area_terreno=@area_terreno,topografia=@topografia,pedologia=@pedologia," +
                    "benfeitoria=@benfeitoria,categoria=@categoria,usoterreno=@usoterreno,situacao=@situacao,topografia_nome=@topografia_nome,pedologia_nome=@pedologia_nome,benfeitoria_nome=@benfeitoria_nome," +
                    "categoria_nome=@categoria_nome,usoterreno_nome=@usoterreno_nome,situacao_nome=@situacao_nome,condominio_nome=@condominio_nome,inscricao=@inscricao where guid=@guid ", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public W_Imovel_Main Retorna_Imovel_Main(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                W_Imovel_Main Sql = (from t in db.W_Imovel_Main where t.Guid == p select t).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Codigo(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.W_Imovel_Main.RemoveRange(db.W_Imovel_Main.Where(i => i.Codigo == Codigo));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Prop(W_Imovel_Prop Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[5];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[3] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo };
                Parametros[4] = new SqlParameter { ParameterName = "@principal", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Principal };

                db.Database.ExecuteSqlCommand("INSERT INTO w_imovel_prop(guid,codigo,nome,tipo,principal) VALUES(@guid,@codigo,@nome,@tipo,@principal)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_W_Imovel_Prop_Codigo(string guid, int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    W_Imovel_Prop b= db.W_Imovel_Prop.Find(db.W_Imovel_Prop.Where(i =>i.Guid==guid && i.Codigo == codigo));
                    db.W_Imovel_Prop.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_W_Imovel_Prop_Guid(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.W_Imovel_Prop.RemoveRange(db.W_Imovel_Prop.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }
    }
}
