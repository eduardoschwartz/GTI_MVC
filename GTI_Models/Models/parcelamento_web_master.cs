﻿
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Parcelamento_web_master {
        [Key]
        public string Guid { get; set; }
        public int Codigo { get; set; }
        public int User_id { get; set; }
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
        public string contribuinte_nome { get; set; }
        public string contribuinte_cpfcnpj { get; set; }
        public string contribuinte_endereco { get; set; }
        public string contribuinte_bairro { get; set; }
        public int contribuinte_cep { get; set; }
        public string contribuinte_cidade { get; set; }
        public string contribuinte_uf { get; set; }
    }
}


