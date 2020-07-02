using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_Guia {
        [Key]
        public string Guid { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public int Imovel_Codigo { get; set; }
        public string Inscricao { get; set; }
        public string Proprietario_Nome { get; set; }
        public string Imovel_Endereco { get; set; }
        public int Imovel_Numero { get; set; }
        public string Imovel_Complemento { get; set; }
        public int Imovel_Cep { get; set; }
        public string Imovel_Bairro { get; set; }
        public string Imovel_Quadra { get; set; }
        public string Imovel_Lote { get; set; }
        public string Comprador_Cpf_Cnpj { get; set; }
        public int Comprador_Codigo { get; set; }
        public string Comprador_Nome { get; set; }
        public string Comprador_Logradouro { get; set; }
        public int Comprador_Numero { get; set; }
        public string Comprador_Complemento { get; set; }
        public int Comprador_Cep { get; set; }
        public string Comprador_Bairro { get; set; }
        public string Comprador_Cidade { get; set; }
        public string Comprador_Uf { get; set; }
        public string Tipo_Instrumento { get; set; }
        public DateTime Data_Transacao { get; set; }
        public decimal Valor_Transacao { get; set; }
        public decimal Valor_Avaliacao { get; set; }
        public decimal Valor_Venal { get; set; }
        public decimal Recursos_proprios_Valor { get; set; }
        public decimal Recursos_proprios_Atual { get; set; }
        public decimal Recursos_conta_Valor { get; set; }
        public decimal Recursos_conta_Atual { get; set; }
        public decimal Recursos_concedido_Valor { get; set; }
        public decimal Recursos_concedido_Atual { get; set; }
        public decimal Financiamento_Valor { get; set; }
        public decimal Financiamento_Atual { get; set; }
        public string Totalidade { get; set; }
        public decimal Totalidade_Perc { get; set; }
        public Int64 Matricula { get; set; }
        public int Itbi_Numero { get; set; }
        public short Itbi_Ano { get; set; }
        public string Inscricao_Incra { get; set; }
        public string Receita_Federal { get; set; }
        public string Descricao_Imovel { get; set; }
        public decimal Valor_Guia { get; set; }
        public int Numero_guia { get; set; }
        public string Nosso_Numero { get; set; }
        public string Linha_Digitavel { get; set; }
        public string Codigo_Barra { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public string Natureza { get; set; }
        public string Tipo_Financiamento { get; set; }
    }
}
