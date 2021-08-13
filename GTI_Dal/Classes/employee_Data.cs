using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Employee_Data {

        private static string _connection;
        public Employee_Data(string sConnection) {
            _connection = sConnection;
        }


        public IEnumerable<employees> ListaEmployee() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return db.Employees.ToList();
            }

        }

        public employees RetornaEmployee(int id) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return db.Employees.FirstOrDefault(e=>e.Id==id);
            }
        }


    }
}
