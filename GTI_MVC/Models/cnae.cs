
using System.ComponentModel.DataAnnotations;

namespace GTI_MVC.Models {
    public class Cnae {
        [Key]
        public string cnae  { get; set; }
        public string Descricao { get; set; }
    }
}
