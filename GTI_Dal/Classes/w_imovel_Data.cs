using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class w_imovel_Data {
        private readonly string _connection;

        public w_imovel_Data(string sConnection) {
            _connection = sConnection;
        }

        public Exception Insert_wimovel_main(string Guid, int Codigo, int UserId) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[3] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = UserId };

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_main(guid,codigo,data_alteracao,userid) VALUES(@guid,@codigo,@data_alteracao,@userid)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_wimovel_main2(WImovel_Main Reg) {
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

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_main(guid,codigo,cip,imune,conjugado,reside,area_terreno,topografia,pedologia,benfeitoria,categoria,usoterreno,situacao,userid," +
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

        public Exception Update_wimovel_main(WImovel_Main Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[29];
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
                if(Reg.Lote_Original!=null)
                    Parametros[24] = new SqlParameter { ParameterName = "@lote_original", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Lote_Original };
                else
                    Parametros[24] = new SqlParameter { ParameterName = "@lote_original", SqlValue =DBNull.Value };
                if(Reg.Quadra_Original!=null)
                    Parametros[25] = new SqlParameter { ParameterName = "@quadra_original", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Quadra_Original };
                else
                    Parametros[25] = new SqlParameter { ParameterName = "@quadra_original", SqlValue = DBNull.Value};
                Parametros[26] = new SqlParameter { ParameterName = "@tipo_matricula", SqlDbType = SqlDbType.Char, SqlValue = Reg.Tipo_Matricula };
                Parametros[27] = new SqlParameter { ParameterName = "@numero_matricula", SqlDbType = SqlDbType.BigInt, SqlValue = Reg.Numero_Matricula };
                Parametros[28] = new SqlParameter { ParameterName = "@tipo_endereco", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Tipo_Endereco };

                db.Database.ExecuteSqlCommand("update wimovel_main set cip=@cip,imune=@imune,conjugado=@conjugado,reside=@reside,area_terreno=@area_terreno,topografia=@topografia,pedologia=@pedologia," +
                    "benfeitoria=@benfeitoria,categoria=@categoria,usoterreno=@usoterreno,situacao=@situacao,topografia_nome=@topografia_nome,pedologia_nome=@pedologia_nome,benfeitoria_nome=@benfeitoria_nome," +
                    "categoria_nome=@categoria_nome,usoterreno_nome=@usoterreno_nome,situacao_nome=@situacao_nome,condominio_nome=@condominio_nome,inscricao=@inscricao,lote_original=@lote_original," +
                    "quadra_original=@quadra_original,tipo_matricula=@tipo_matricula,tipo_endereco=@tipo_endereco,numero_matricula=@numero_matricula where guid=@guid ", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public WImovel_Main Retorna_Imovel_Main(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                WImovel_Main Sql = (from t in db.WImovel_Main where t.Guid == p select t).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Codigo(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.WImovel_Main.RemoveRange(db.WImovel_Main.Where(i => i.Codigo == Codigo));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Prop(WImovel_Prop Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[5];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Nome };
                Parametros[3] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo };
                Parametros[4] = new SqlParameter { ParameterName = "@principal", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Principal };

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_prop(guid,codigo,nome,tipo,principal) VALUES(@guid,@codigo,@nome,@tipo,@principal)", Parametros);
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
                    WImovel_Prop b = db.WImovel_Prop.Find(db.WImovel_Prop.Where(i => i.Guid == guid && i.Codigo == codigo));
                    db.WImovel_Prop.Remove(b);
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
                    db.WImovel_Prop.RemoveRange(db.WImovel_Prop.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<WImovel_Prop> Lista_WImovel_Prop(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<WImovel_Prop> Sql = (from t in db.WImovel_Prop where t.Guid == p select t).ToList();
                return Sql;
            }
        }

        public Exception Insert_W_Imovel_Endereco(WImovel_Endereco Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[8];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@logradouro_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Logradouro_codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@logradouro_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Logradouro_nome };
                Parametros[3] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Numero };
                if(Reg.Complemento!=null)
                    Parametros[4] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Complemento };
                else
                    Parametros[4] = new SqlParameter { ParameterName = "@complemento",  SqlValue = DBNull.Value};
                Parametros[5] = new SqlParameter { ParameterName = "@bairro_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Bairro_codigo };
                Parametros[6] = new SqlParameter { ParameterName = "@bairro_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Bairro_nome };
                Parametros[7] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cep };

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_endereco(guid,logradouro_codigo,logradouro_nome,numero,complemento,bairro_codigo,bairro_nome,cep) VALUES(@guid,@logradouro_codigo,@logradouro_nome,@numero,@complemento,@bairro_codigo,@bairro_nome,@cep)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public WImovel_Endereco Retorna_Imovel_Endereco(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                WImovel_Endereco Sql = (from t in db.WImovel_Endereco where t.Guid == p select t).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Endereco(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.WImovel_Endereco.RemoveRange(db.WImovel_Endereco.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Testada(WImovel_Testada Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[3];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@face", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Face  };
                Parametros[2] = new SqlParameter { ParameterName = "@comprimento", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Comprimento};

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_testada(guid,face,comprimento) VALUES(@guid,@face,@comprimento)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<WImovel_Testada> Lista_WImovel_Testada(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<WImovel_Testada> Sql = (from t in db.WImovel_Testada where t.Guid == p select t).ToList();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Testada_Guid(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.WImovel_Testada.RemoveRange(db.WImovel_Testada.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_W_Imovel_Testada_Codigo(string guid, int codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    WImovel_Testada b = db.WImovel_Testada.Find(db.WImovel_Testada.Where(i => i.Guid == guid && i.Face == codigo));
                    db.WImovel_Testada.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Area(WImovel_Area Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[13];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Seq };
                Parametros[2] = new SqlParameter { ParameterName = "@area", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Area };
                Parametros[3] = new SqlParameter { ParameterName = "@uso_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Uso_codigo };
                Parametros[4] = new SqlParameter { ParameterName = "@uso_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Uso_nome };
                Parametros[5] = new SqlParameter { ParameterName = "@tipo_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Tipo_codigo };
                Parametros[6] = new SqlParameter { ParameterName = "@tipo_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo_nome };
                Parametros[7] = new SqlParameter { ParameterName = "@categoria_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Categoria_codigo };
                Parametros[8] = new SqlParameter { ParameterName = "@categoria_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Categoria_nome };
                if (Reg.Data_Aprovacao == null )
                    Parametros[9] = new SqlParameter { ParameterName = "@data_aprovacao", SqlValue = DBNull.Value };
                else
                    Parametros[9] = new SqlParameter { ParameterName = "@data_aprovacao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Data_Aprovacao };
                if (Reg.Processo_Data == null )
                    Parametros[10] = new SqlParameter { ParameterName = "@processo_data", SqlValue = DBNull.Value };
                else
                    Parametros[10] = new SqlParameter { ParameterName = "@processo_data", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Processo_Data };
                if(Reg.Processo_Numero==null)
                    Parametros[11] = new SqlParameter { ParameterName = "@processo_numero",  SqlValue = DBNull.Value};
                else
                    Parametros[11] = new SqlParameter { ParameterName = "@processo_numero", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Processo_Numero };
                Parametros[12] = new SqlParameter { ParameterName = "@pavimentos", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Pavimentos };

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_area(guid,seq,area,uso_codigo,uso_nome,tipo_codigo,tipo_nome,categoria_codigo,categoria_nome,data_aprovacao,processo_data,processo_numero," +
                    "pavimentos) VALUES(@guid,@seq,@area,@uso_codigo,@uso_nome,@tipo_codigo,@tipo_nome,@categoria_codigo,@categoria_nome,@data_aprovacao,@processo_data,@processo_numero,@pavimentos)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<WImovel_Area> Lista_WImovel_Area(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<WImovel_Area> Sql = (from t in db.WImovel_Area where t.Guid == p select t).ToList();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Area_Guid(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.WImovel_Area.RemoveRange(db.WImovel_Area.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_W_Imovel_Area_Codigo(string guid, int seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    WImovel_Area b = db.WImovel_Area.Find(db.WImovel_Area.Where(i => i.Guid == guid && i.Seq == seq));
                    db.WImovel_Area.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_W_Imovel_Historico(WImovel_Historico Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[6];

                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@seq", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Seq };
                Parametros[2] = new SqlParameter { ParameterName = "@data_alteracao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Data_Alteracao };
                Parametros[3] = new SqlParameter { ParameterName = "@historico", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Historico };
                Parametros[4] = new SqlParameter { ParameterName = "@usuario_codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Usuario_Codigo };
                if(Reg.Usuario_Codigo==0)
                    Parametros[5] = new SqlParameter { ParameterName = "@usuario_nome",  SqlValue = DBNull.Value };
                else
                    Parametros[5] = new SqlParameter { ParameterName = "@usuario_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Usuario_Nome };

                db.Database.ExecuteSqlCommand("INSERT INTO wimovel_historico(guid,seq,data_alteracao,historico,usuario_codigo,usuario_nome) VALUES(@guid,@seq,@data_alteracao,@historico,@usuario_codigo,@usuario_nome)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<WImovel_Historico> Lista_WImovel_Historico(string p) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<WImovel_Historico> Sql = (from t in db.WImovel_Historico where t.Guid == p select t).ToList();
                return Sql;
            }
        }

        public Exception Excluir_W_Imovel_Historico_Guid(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.WImovel_Historico.RemoveRange(db.WImovel_Historico.Where(i => i.Guid == guid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_W_Imovel_Historico_Seq(string guid, int seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    WImovel_Historico b = db.WImovel_Historico.Find(db.WImovel_Historico.Where(i => i.Guid == guid && i.Seq == seq));
                    db.WImovel_Historico.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


    }
}
