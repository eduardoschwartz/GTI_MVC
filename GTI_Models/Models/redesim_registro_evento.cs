using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Redesim_registro_evento {
        [Key]
        [Column(Order=1) ]
        public string Protocolo { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Evento { get; set; }
    }
}
