
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_financiamento {
        [Key]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
