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


    }
}
