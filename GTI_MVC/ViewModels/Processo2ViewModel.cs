using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class Processo2ViewModel {
        public string Guid { get; set; }
        public string Tipo_Requerente { get; set; }
        public int Centro_Custo_Codigo { get; set; }
        public string Centro_Custo_Nome { get; set; }
        public string Centro_Custo_CpfCnpj { get; set; }

    }
}