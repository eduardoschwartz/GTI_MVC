using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_web_lista_codigo {
        [Key]
        [Column(Order=1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Id { get; set; }
        public string Grupo { get; set; }
        public string Texto { get; set; }
        public int Valor { get; set; }
        public bool Selected { get; set; }
    }
}
