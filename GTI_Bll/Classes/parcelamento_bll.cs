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


    }
}
