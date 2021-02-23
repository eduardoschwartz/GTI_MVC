using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

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

        public Exception Incluir_Viabilidade(Redesim_Viabilidade reg) {
            Redesim_Data obj = new Redesim_Data(_connection);
            Exception ex = obj.Incluir_Viabilidade(reg);
            return ex;
        }

        public bool Existe_Registro(string Processo) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Existe_Registro(Processo);
        }

        public bool Existe_Viabilidade(string Processo) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Existe_Viabilidade(Processo);
        }

        public List<Redesim_natureza_juridica> Lista_Natureza_Juridica() {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Lista_Natureza_Juridica();
        }

        public int Incluir_Natureza_Juridica(string Name) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Incluir_Natureza_Juridica(Name);
        }

    }
}
