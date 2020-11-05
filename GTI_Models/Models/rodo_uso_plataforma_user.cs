using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Rodo_uso_plataforma_user {
        [Key]
        [Column(Order = 1)]
        public int User_id { get; set; }
        [Key]
        [Column(Order = 2)]
        public bool Funcionario { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Empresa { get; set; }
    }
}
