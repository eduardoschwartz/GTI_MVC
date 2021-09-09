using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Lista_devedor {
        [Key]
        [Column(Order = 1)]
        public int Userid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Codigo { get; set; }
        public short    Ano { get; set; }
        public decimal valor_total { get; set; }
    }
}
