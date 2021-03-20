using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Parcelamento_bll {

        private readonly string _connection;
        public Parcelamento_bll(string sConnection) {
            _connection = sConnection;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(int Codigo, char Tipo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Origem(Codigo,Tipo);
        }

        public bool Existe_Parcelamento_Web_Master(string _guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Existe_Parcelamento_Web_Master(_guid);
        }

        public Exception Incluir_Parcelamento_Web_Master(Parcelamento_web_master Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Master(Reg);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(Parcelamento_web_lista_codigo Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Lista_Codigo(Reg);
            return ex;
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Lista_Codigo(guid);
        }

        public Exception Incluir_Parcelamento_Web_Origem(Parcelamento_web_origem Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Origem(Reg);
            return ex;
        }

        public Parcelamento_web_master Retorna_Parcelamento_Web_Master(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Parcelamento_Web_Master(guid);
        }

        public Exception Atualizar_Codigo_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Codigo_Master(reg);
            return ex;
        }

        public Exception Atualizar_Criterio_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Criterio_Master(reg);
            return ex;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Origem(guid);
        }

    }
}
