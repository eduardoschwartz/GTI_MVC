using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_master {
        [Key]
        public string Protocolo { get; set; }
        public DateTime Data_licenca { get; set; }
        public string Razao_Social { get; set; }
        public string Cnpj { get; set; }
        public int Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string Cnae_Principal { get; set; }
    }
}
