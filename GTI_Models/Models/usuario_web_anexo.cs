using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Usuario_web_anexo {
        [Key]
        [Column(Order =1)]
        public int Userid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Tipo { get; set; }
        public string Arquivo { get; set; }
    }

    public class Usuario_Web_Anexo_Struct {
        public int UserId { get; set; }
        public int Codigo { get; set; }
        public bool Fisica { get; set; }
        public short Tipo { get; set; }
        public string Descricao { get; set; }
        public string Arquivo { get; set; }
    }


}
