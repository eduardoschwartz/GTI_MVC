using System;

namespace GTI_Mvc.ViewModels {
    public class AlvaraViewModel {
        public int Codigo { get; set; }
        public string Numero_Processo { get; set; }
        public string Protocolo_Vre { get; set; }
        public DateTime? Data_Vre { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public bool IsProvisorio { get; set; }
    }
}