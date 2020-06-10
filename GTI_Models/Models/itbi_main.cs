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
        public string Imovel_Quadra { get; set; }
        public string Imovel_Lote { get; set; }
        public int Natureza_Codigo { get; set; }
        public string Comprador_cpf_cnpj { get; set; }
        public int Comprador_codigo { get; set; }
        public string Comprador_nome { get; set; }
        public int Comprador_logradouro_codigo { get; set; }
        public string Comprador_logradouro_nome { get; set; }
        public int Comprador_numero { get; set; }
        public string Comprador_complemento { get; set; }
        public int Comprador_bairro_codigo { get; set; }
        public string Comprador_bairro_nome { get; set; }
        public int Comprador_cidade_codigo { get; set; }
        public string Comprador_cidade_nome { get; set; }
        public string Comprador_uf { get; set; }
        public string Comprador_telefone { get; set; }
        public string Comprador_email { get; set; }
    }
}

