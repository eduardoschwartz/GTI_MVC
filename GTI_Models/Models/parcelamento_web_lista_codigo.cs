using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_web_lista_codigo {
        [Key]
        [Column(Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Codigo  { get; set; }
        public string Tipo { get; set; }
        public string Documento { get; set; }
        public string Descricao { get; set; }
        public bool Selected { get; set; }
    }
}


