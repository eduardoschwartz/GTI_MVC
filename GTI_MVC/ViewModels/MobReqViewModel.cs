﻿using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class MobReqViewModel {
        public int Evento_Codigo { get; set; }
        public string CpfValue { get; set; }
        public string Razao_Social { get; set; }
        public string Rg_IE { get; set; }
        public string Atividade { get; set; }
        public string Obs { get; set; }
        public string Data_Evento { get; set; }
        public int Codigo { get; set; }
     }

    public class MobReqQueryViewModel {
        public int Ano_Selected { get; set; }
        public List<Mobreq_main_Struct> Lista_req { get; set; }
    }


}