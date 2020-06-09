using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_main {
        [Key]
        public string Guid { get; set; }
        public DateTime Data_cadastro { get; set; }
        public int Imovel_codigo { get; set; }
        public string Inscricao { get; set; }
        public int Proprietario_Codigo { get; set; }
        public string Proprietario_Nome { get; set; }
        public string Imovel_endereco { get; set; }
        public int Imovel_numero { get; set; }
        public string Imovel_complemento { get; set; }
        public int Imovel_cep { get; set; }
        public string Imovel_bairro { get; set; }
        public int Natureza_Codigo { get; set; }
    }
}
