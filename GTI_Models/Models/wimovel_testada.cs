using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class WImovel_Testada {
        [Key]
        [Column(Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Face { get; set; }
        public decimal Comprimento { get; set; }
    }
}
