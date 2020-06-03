using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Logradouro_bairro {
        [Key]
        [Column(Order = 1)]
        public int Logradouro { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Inicial { get; set; }
        public int Final { get; set; }
        public int Bairro { get; set; }
    }
}
