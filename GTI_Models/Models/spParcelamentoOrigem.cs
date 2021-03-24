using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class SpParcelamentoOrigem {
        [Key]
        [Column(Order = 1)]
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
    }
}
