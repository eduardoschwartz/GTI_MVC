using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Tributoartigo {
        [Key]
        public int Codtributo { get; set; }
        public string Artigo { get; set; }
    }

    public class TributoArtigoStruct {
        public int Tributo_Codigo { get; set; }
        public string Tributo_Nome { get; set; }
        public string Artigo { get; set; }
    }

}
