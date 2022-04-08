using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Secretaria {
            [Key]
            public short Codigo { get; set; }
            public string Nome { get; set; }
            public string Sigla { get; set; }
            public short? Codigocc { get; set; }
            public string Email { get; set; }
    }
}
