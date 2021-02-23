using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_natureza_juridica {
        [Key]
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }
}
