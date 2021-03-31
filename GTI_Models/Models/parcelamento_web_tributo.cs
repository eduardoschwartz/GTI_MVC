
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_Web_Tributo {
        [Key]
        [Column(Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Tributo { get; set; }
        public decimal Valor { get; set; }
        public decimal Perc { get; set; }
    }
}

