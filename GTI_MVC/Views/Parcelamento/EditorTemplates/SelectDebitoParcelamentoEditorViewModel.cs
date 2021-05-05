using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTI_Mvc.Views.Parcelamento.EditorTemplates {
    public class SelectDebitoParcelamentoEditorViewModel {
        public int Idx { get; set; }
        public short Exercicio { get; set; }
        public short Lancamento { get; set; }
        public string Nome_lancamento { get; set; }
        public short Sequencia { get; set; }
        public short Parcela { get; set; }
        public short Complemento { get; set; }
        public DateTime Data_vencimento { get; set; }
        public decimal Valor_principal { get; set; }
        public decimal Valor_juros { get; set; }
        public decimal Valor_multa { get; set; }
        public decimal Valor_correcao { get; set; }
        public decimal Valor_total { get; set; }
        public string Ajuizado { get; set; }
        public short Qtde_parcelamento { get; set; }
        public decimal Perc_penalidade { get; set; }
        public decimal Valor_penalidade { get; set; }
        public bool Selected { get; set; }
        public string Execucao_Fiscal { get; set; }
        public string Protesto { get; set; }
    }
}
