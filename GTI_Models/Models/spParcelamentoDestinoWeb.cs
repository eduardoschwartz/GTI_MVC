using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class SpParcelamentoDestinoWeb {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Qtde_Parcela { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Numero_Parcela { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public decimal Valor_Liquido { get; set; }
        public decimal Valor_Juros { get; set; }
        public decimal Valor_Multa { get; set; }
        public decimal Valor_Correcao { get; set; }
        public decimal Valor_Principal { get; set; }
        public decimal Saldo { get; set; }
        public decimal Juros_Perc { get; set; }
        public decimal Juros_Mes { get; set; }
        public decimal Juros_Apl { get; set; }
        public decimal Valor_Honorario { get; set; }
        public decimal Valor_Total { get; set; }
    }
}

