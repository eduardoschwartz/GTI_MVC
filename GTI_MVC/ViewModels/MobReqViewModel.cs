using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class MobReqViewModel {
        public int Evento_Codigo { get; set; }
        public string Evento_Nome { get; set; }
        public string CpfValue { get; set; }
        public string Razao_Social { get; set; }
        public string Rg_IE { get; set; }
        public string Atividade { get; set; }
        public string Obs { get; set; }
        public string Data_Evento { get; set; }
        public int Codigo { get; set; }
        public string Guid { get; set; }
        public int Situacao_Codigo { get; set; }
        public string Situacao_Nome { get; set; }
        public string Data_Evento2 { get; set; }
        public string Funcionario { get; set; }
        public string AnoNumero { get; set; }
    }

    public class MobReqQueryViewModel {
        public int Ano_Selected { get; set; }
        public List<Mobreq_main_Struct> Lista_req { get; set; }
    }


}