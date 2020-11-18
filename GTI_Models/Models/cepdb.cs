using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Cepdb {
        [Key]
        public string Cep { get; set; }
        public string Uf { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Logradouro { get; set; }
    }
}
