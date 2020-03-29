using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_MVC.Models {
    public class Parceladocumento {
        [Key]
        [Column(Order = 1)]
        public int Codreduzido { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Anoexercicio { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Codlancamento { get; set; }
        [Key]
        [Column(Order = 4)]
        public short Seqlancamento { get; set; }
        [Key]
        [Column(Order = 5)]
        public byte Numparcela { get; set; }
        [Key]
        [Column(Order = 6)]
        public byte Codcomplemento { get; set; }
        [Key]
        [Column(Order = 7)]
        public int Numdocumento { get; set; }
        public decimal? Valorjuros { get; set; }
        public int? Codbanco { get; set; }
        public decimal? Valormulta { get; set; }
        public decimal? Valorcorrecao { get; set; }
        public bool? Intacto { get; set; }
        public short? Plano { get; set; }
    }
}
