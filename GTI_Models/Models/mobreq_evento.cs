using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Mobreq_evento {
        [Key]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
