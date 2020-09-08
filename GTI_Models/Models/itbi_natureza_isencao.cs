
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_natureza_isencao {
        [Key]
        public short Codigo { get; set; }
        public string Descricao { get; set; }
        public string Artigo { get; set; }
    }
}
