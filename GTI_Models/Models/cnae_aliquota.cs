
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Cnae_Aliquota {
        [Key]
        [Column(Order =1)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Cnae { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Criterio { get; set; }
        public decimal Valor { get; set; }
    }
}

