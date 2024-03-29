﻿using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Mobreq_bll {
        private string _connection;
        public Mobreq_bll(string sConnection) {
            _connection = sConnection;
        }


        public List<Mobreq_evento> Lista_Evento() {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            return obj.Lista_Evento();
        }

        public int Incluir_Mobreq_Main(Mobreq_main Reg) {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            int _numero = obj.Incluir_Mobreq_Main(Reg);
            return _numero;

        }

        public List<Mobreq_main_Struct> Lista_Requerimentos(int AnoInclusao) {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            return obj.Lista_Requerimentos(AnoInclusao);
        }

        public string Retorna_Evento(int Codigo) {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            return obj.Retorna_Evento(Codigo);
        }

        public Mobreq_main_Struct Retorna_Requerimento(string Guid) {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            return obj.Retorna_Requerimento(Guid);
        }

        public Exception Alterar_Situacao(string Guid,short NovaSituacao,int User) {
            Mobreq_Data obj = new Mobreq_Data(_connection);
            Exception ex = obj.Alterar_Situacao(Guid,NovaSituacao,User);
            return ex;
        }

    }
}
