using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class LogWeb_Evento {
        [Key]
        public short Id { get; set; }
        public string Descricao { get; set; }
    }
}
