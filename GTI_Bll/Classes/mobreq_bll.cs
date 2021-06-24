using GTI_Dal.Classes;
using GTI_Models.Models;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Mobreq_bll {
        private string _connection;
        public Mobreq_bll(string sConnection) {
            _connection = sConnection;
        }


        public List<Mobreq_evento> Lista_vento() {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            return obj.Lista_Evento();
        }


    }


}
