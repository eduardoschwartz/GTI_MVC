using GTI_Dal.Classes;
using GTI_Models.Models;
using System;

namespace GTI_Bll.Classes {
    public class Redesim_bll {
        private static string _connection;
        public Redesim_bll(string sConnection) {
            _connection = sConnection;

        }

        public Exception Incluir_Arquivo(Redesim_arquivo reg) {
            Redesim_Data obj = new Redesim_Data(_connection);
            Exception ex = obj.Incluir_Arquivo(reg);
            return ex;
        }

        public Exception Incluir_Registro(Redesim_Registro reg) {
            Redesim_Data obj = new Redesim_Data(_connection);
            Exception ex = obj.Incluir_Registro(reg);
            return ex;
        }

    }
}
