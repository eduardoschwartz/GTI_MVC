using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class CidadaoViewModel {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Data_Nascto { get; set; }
        public string Rg_Numero { get; set; }
        public string Rg_Orgao { get; set; }
        public string Cnh_Numero { get; set; }
        public string Cnh_Orgao { get; set; }
        public int Profissao_Codigo { get; set; }
    }
}