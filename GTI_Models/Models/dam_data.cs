using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Dam_data {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Exercicio { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Lancamento { get; set; }
        [Key]
        [Column(Order = 4)]
        public short Sequencia { get; set; }
        [Key]
        [Column(Order = 5)]
        public byte Parcela { get; set; }
        [Key]
        [Column(Order = 6)]
        public byte Complemento { get; set; }
        public DateTime Datavencimento { get; set; }
        public string Descricao { get; set; }
        public char Da { get; set; }
        public char Aj { get; set; }
        public decimal Principal { get; set; }
        public decimal Juros { get; set; }
        public decimal Multa { get; set; }
        public decimal Correcao { get; set; }
        public decimal Total { get; set; }
    }
}

