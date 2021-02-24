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

        public List<Redesim_evento> Lista_Evento() {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Lista_Evento();
        }

        public int Incluir_Evento(string Name) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Incluir_Evento(Name);
        }

        public void Incluir_Registro_Evento(string Protocolo, int[] ListaEvento) {
            Redesim_Data obj = new Redesim_Data(_connection);
            obj.Incluir_Registro_Evento(Protocolo, ListaEvento);
        }

        public List<Redesim_porte_empresa> Lista_Porte_Empresa() {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Lista_Porte_Empresa();
        }

        public int Incluir_Porte_Empresa(string Name) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Incluir_Porte_Empresa(Name);
        }

        public List<Redesim_forma_atuacao> Lista_Forma_Atuacao() {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Lista_Forma_Atuacao();
        }

        public int Incluir_Forma_Atuacao(string Name) {
            Redesim_Data obj = new Redesim_Data(_connection);
            return obj.Incluir_Forma_Atuacao(Name);
        }

        public void Incluir_Registro_Forma_Atuacao(string Protocolo, int[] ListaForma) {
            Redesim_Data obj = new Redesim_Data(_connection);
            obj.Incluir_Registro_Forma_Atuacao(Protocolo, ListaForma);
        }

        public void Incluir_Cnae(string Protocolo, string[] ListaCnae) {
            Redesim_Data obj = new Redesim_Data(_connection);
            obj.Incluir_Cnae(Protocolo, ListaCnae);
        }
    }
}
