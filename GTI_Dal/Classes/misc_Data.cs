using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GTI_Models.modelCore;

namespace GTI_Dal.Classes {
    public class Misc_Data {
        private readonly string _connection;

        public Misc_Data(string sConnection) {
            _connection = sConnection;
        }

        public List<Calendar_event> Lista_Evento() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Calendar_Event select c);
                return Sql.ToList();
            }
        }

    }
}
