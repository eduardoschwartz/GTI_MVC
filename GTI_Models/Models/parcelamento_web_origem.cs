using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_web_origem {
        [Key]
        [Column (Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Idx { get; set; }
        public short Ano { get; set; }
        public short Lancamento { get; set; }
        public short Sequencia { get; set; }
        public short Parcela { get; set; }
        public short Complemento { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public decimal Valor_Tributo { get; set; }
        public decimal Valor_Multa { get; set; }
        public decimal Valor_Juros { get; set; }
        public decimal Valor_Correcao { get; set; }
        public decimal Valor_Total { get; set; }
        public short Qtde_Parcelamento { get; set; }
        public decimal Perc_Penalidade { get; set; }
        public decimal Valor_Penalidade { get; set; }
        public string Lancamento_Nome { get; set; }
        public char Ajuizado { get; set; }
    }
}
