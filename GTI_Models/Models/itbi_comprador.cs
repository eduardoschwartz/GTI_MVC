using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Itbi_comprador {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public byte Seq { get; set; }
        public string Nome { get; set; }
        public string Cpf_cnpj { get; set; }
    }
}
