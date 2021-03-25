using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_valor_minimo {
        [Key]
        [Column(Order=1)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 2)]
        public bool DistritoIndustrial { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
    }
}


