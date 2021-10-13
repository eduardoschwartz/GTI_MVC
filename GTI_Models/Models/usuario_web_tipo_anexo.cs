using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Usuario_web_tipo_anexo {
        [Key]
        public short Codigo { get; set; }
        public bool Fisica { get; set; }
        public string Descricao { get; set; }
        public bool Obrigatorio { get; set; }
    }
}
