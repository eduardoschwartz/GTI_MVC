using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class misc_bll {

        private string _connection;

        public misc_bll(string sConnection) {
            _connection = sConnection;
        }

        public List<Calendar_event> Lista_Evento() {
            Misc_Data obj = new Misc_Data(_connection);
            return obj.Lista_Evento();
        }


    }
}
