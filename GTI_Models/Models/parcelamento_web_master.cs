
using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Parcelamento_web_master {
        [Key]
        public string Guid { get; set; }
        public int User_id { get; set; }
        public int Requerente_Codigo { get; set; }
        public string Requerente_Nome { get; set; }
        public string Requerente_CpfCnpj { get; set; }
        public string Requerente_Bairro { get; set; }
        public string Requerente_Cidade { get; set; }
        public string Requerente_Uf { get; set; }
        public string Requerente_Logradouro { get; set; }
        public int Requerente_Numero { get; set; }
        public string Requerente_Complemento { get; set; }
        public int Requerente_Cep { get; set; }
        public string Requerente_Telefone { get; set; }
        public string Requerente_Email { get; set; }
        public int Contribuinte_Codigo { get; set; }
        public string Contribuinte_nome { get; set; }
        public string Contribuinte_cpfcnpj { get; set; }
        public string Contribuinte_endereco { get; set; }
        public string Contribuinte_bairro { get; set; }
        public int Contribuinte_cep { get; set; }
        public string Contribuinte_cidade { get; set; }
        public string Contribuinte_uf { get; set; }
        public DateTime? Data_Vencimento { get; set; }
    }
}


