using GTI_Mvc.Views.Tributario.EditorTemplates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GTI_Mvc.ViewModels {
    public class DebitoSelectionViewModel {
        public List<SelectDebitoEditorViewModel> Debito { get; set; }
        [Display(Name = "Inscrição Municipal")]
        public int Inscricao { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public string Nome { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        public int Plano { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Honorario { get; set; }
        public decimal Soma_Juros_Hidden { get; set; }
        public decimal Soma_Multa_Hidden { get; set; }

        public DebitoSelectionViewModel() {
            this.Debito = new List<SelectDebitoEditorViewModel>();
        }

        public IEnumerable<int> getSelectedIds() {
            // Return an Enumerable containing the Id's of the selected people:
            return (from p in this.Debito where p.Selected select p.Id).ToList();
        }

    }
}
