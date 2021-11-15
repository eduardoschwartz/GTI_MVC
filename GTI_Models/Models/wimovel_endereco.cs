using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class WImovel_Endereco {
        [Key]
        public string Guid { get; set; }
        public int Logradouro_codigo { get; set; }
        public string Logradouro_nome { get; set; }
        public short Numero { get; set; }
        public string Complemento { get; set; }
        public int Bairro_codigo { get; set; }
        public string Bairro_nome { get; set; }
        public string Cep { get; set; }
    }
}

