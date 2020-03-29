using System.ComponentModel.DataAnnotations;

namespace GTI_MVC.Models {
    public class Pedologia {
        [Key]
        public short Codpedologia { get; set; }
        public string Descpedologia { get; set; }
    }
}
