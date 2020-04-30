using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Encargo_cvd {
        [Key]
        [Column(Order = 1)]
        public int Codigo { get; set; }
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
        [Key]
        [Column(Order = 7)]
        public short Exercicio_enc { get; set; }
        [Key]
        [Column(Order = 8)]
        public short Lancamento_enc { get; set; }
        [Key]
        [Column(Order = 9)]
        public short Sequencia_enc { get; set; }
        [Key]
        [Column(Order = 10)]
        public byte Parcela_enc { get; set; }
        [Key]
        [Column(Order = 11)]
        public byte Complemento_enc { get; set; }
        public int Documento { get; set; }
    }
}

