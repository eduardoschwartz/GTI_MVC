using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_MVC.Models {
    public class Usuariocc {
        [Key]
        [Column(Order = 1)]
        public int Userid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Codigocc { get; set; }
    }
}
