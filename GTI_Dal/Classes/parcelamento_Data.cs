using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace GTI_Dal.Classes {
    public class Parcelamento_Data {
        private readonly string _connection;

        public Parcelamento_Data(string sConnection) {
            _connection = sConnection;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(int Codigo, char Tipo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 600;
                List<SpParcelamentoOrigem> ListaOrigem = new List<SpParcelamentoOrigem>();

                Tributario_Data tributarioRepository = new Tributario_Data("GTIconnection");
                List<SpExtrato_Parcelamento> _listaTributo = tributarioRepository.Lista_Extrato_Tributo_Parcelamento(Codigo);
                List<SpExtrato_Parcelamento> _extrato = tributarioRepository.Lista_Extrato_Parcela_Parcelamento(_listaTributo);

                return ListaOrigem;
            }
        }

        public Exception Incluir_Parcelamento_Web_Master(Parcelamento_web_master Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[2];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_master(guid,codigo) " +
                                              "VALUES(@guid,@codigo)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Parcelamento_Web_Master(string _guid) {
            bool bRet = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Parcelamento_Web_Master.Count(a => a.Guid == _guid);
                if (existingReg != 0) {
                    bRet = true;
                }
            }
            return bRet;
        }

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(Parcelamento_web_lista_codigo Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[6];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.Int, SqlValue = Reg.Id };
                Parametros[2] = new SqlParameter { ParameterName = "@grupo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Grupo };
                Parametros[3] = new SqlParameter { ParameterName = "@texto", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Texto };
                Parametros[4] = new SqlParameter { ParameterName = "@valor", SqlDbType = SqlDbType.Int, SqlValue = Reg.Valor };
                Parametros[5] = new SqlParameter { ParameterName = "@selected", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Selected };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_lista_codigo(guid,id,grupo,texto,valor,selected) " +
                                              "VALUES(@guid,@id,@grupo,@texto,@valor,@selected)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Lista_Codigo where t.Guid == guid orderby t.Id select t).ToList();
                List<Parcelamento_web_lista_codigo> Lista = new List<Parcelamento_web_lista_codigo>();
                foreach (var item in reg) {
                    Parcelamento_web_lista_codigo Linha = new Parcelamento_web_lista_codigo {
                        Guid = guid,
                        Id = item.Id,
                        Grupo=item.Grupo,
                        Texto=item.Texto,
                        Valor=item.Valor,
                        Selected=item.Selected
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }


    }
}
