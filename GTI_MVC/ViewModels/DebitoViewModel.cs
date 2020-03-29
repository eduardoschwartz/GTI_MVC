using GTI_Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class DebitoViewModel {
        [Display(Name = "Inscrição Municipal")]
        public string Inscricao { get; set; }
        public string Nome { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string ErrorMessage { get; set; }
        [Display(Name = "Data de vencimento")]
        public string DataVencimento { get; set; }
        public List<DebitoStructureWeb> Lista_Debito { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Honorario { get; set; }
        public int Plano { get; set; }
    }
}
