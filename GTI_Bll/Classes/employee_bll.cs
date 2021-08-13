using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using static GTI_Models.modelCore;

namespace GTI_Bll.Classes {
    public class Employee_bll {

        private string _connection;
        public Employee_bll(string sConnection) {
            _connection = sConnection;
        }

        public IEnumerable<employees> ListaEmployee() {
            Employee_Data obj = new Employee_Data(_connection);
            return obj.ListaEmployee();
        }

        public employees RetornaEmployee(int id) {
            Employee_Data obj = new Employee_Data(_connection);
            return obj.RetornaEmployee(id);
        }

    }
}
