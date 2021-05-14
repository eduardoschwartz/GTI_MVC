
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Cidadao_socio {
        [Key]
        [Column(Order=1)]
        public int Codigo_Empresa { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Codigo_Socio { get; set; }
    }
}
