using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Mobreq_Situacao {
        [Key]
        public byte Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
