using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTI_Mvc.Views.Tributario.EditorTemplates { 
    public class ListDebitoEditorViewModel {
        public int Id { get; set; }
        public int Exercicio { get; set; }
        public int Lancamento { get; set; }
        public int Seq { get; set; }
        public int Parcela { get; set; }
        public int Complemento { get; set; }
        public string Lancamento_Nome { get; set; }
        public string Data_Vencimento { get; set; }
        public string Da { get; set; }
        public string Aj { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Honorario { get; set; }
        public decimal Soma_Juros_Hidden { get; set; }
        public decimal Soma_Multa_Hidden { get; set; }
        public string Pt { get; set; }
        public string Ep { get; set; }
    }
}
