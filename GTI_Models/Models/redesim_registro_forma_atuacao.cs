using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Redesim_registro_forma_atuacao {
        [Key]
        [Column(Order=1)]
        public string Protocolo { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Forma_Atuacao { get; set; }
    }
}
