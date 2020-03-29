using GTI_Models.EditorTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTI_Models.ViewModels {
    public class DebitoListViewModel {
        public List<ListDebitoEditorViewModel> Debito { get; set; }
        public int Inscricao { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
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
        public string RefTran { get; set; }
        public string Valor_Boleto { get; set; }
        public string Data_Vencimento_String { get; set; }

        public DebitoListViewModel() {
            this.Debito = new List<ListDebitoEditorViewModel>();
        }

        public IEnumerable<int> getSelectedIds() {
            // Return an Enumerable containing the Id's of the selected people:
            return (from p in this.Debito select p.Id).ToList();
        }
    }
}
