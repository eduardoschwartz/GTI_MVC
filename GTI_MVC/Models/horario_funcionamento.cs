using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.Models {
    public class Horario_funcionamento {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}
