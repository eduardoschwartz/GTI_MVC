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
                var existingReg = db.parcelamento_Web_Master.Count(a => a.Guid == _guid);
                if (existingReg != 0) {
                    bRet = true;
                }
            }
            return bRet;
        }


    }
}
