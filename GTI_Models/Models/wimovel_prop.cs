using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class WImovel_Prop {
        [Key]
        [Column(Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public bool Principal { get; set; }
    }
}
