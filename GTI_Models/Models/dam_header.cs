using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Dam_header {
        [Key]
        public string Guid { get; set; }
        public int Form { get; set; }
        public int Codigo { get; set; }
        public string Lancamento { get; set; }
        public string Nome { get; set; }
        public string Cpf_cnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Quadra { get; set; }
        public string Lote { get; set; }
        public int Cep { get; set; }
        public string Nosso_Numero { get; set; }
        public int Numero_documento { get; set; }
        public decimal Valor_guia { get; set; }
        public DateTime Data_vencimento { get; set; }
        public string Codigo_barra { get; set; }
        public string Linha_digitavel { get; set; }
        public byte[] Qrcodeimage { get; set; }
        public string Url { get; set; }
        public string Txid { get; set; }
        public string Emv { get; set; }
        public byte[] Codebar { get; set; }
    }
}
