using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Cepdb {
        [Key]
        public string Cep { get; set; }
        public string Uf { get; set; }
        public string Cidade { get; set; }
        public int Cidadecodigo { get; set; }
        public string Bairro { get; set; }
        public int Bairrocodigo { get; set; }
        public string Logradouro { get; set; }
        public bool Func { get; set; }
        public int Userid { get; set; }
    }
}
