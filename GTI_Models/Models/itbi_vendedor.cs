using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_vendedor {
        [Key]
        public string Guid { get; set; }
        public byte Seq { get; set; }
        public string Nome { get; set; }
        public string Cpf_cnpj { get; set; }
    }
}
