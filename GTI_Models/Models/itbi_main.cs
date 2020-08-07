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
        public int Comprador_cep { get; set; }
        public int Comprador_bairro_codigo { get; set; }
        public string Comprador_bairro_nome { get; set; }
        public int Comprador_cidade_codigo { get; set; }
        public string Comprador_cidade_nome { get; set; }
        public string Comprador_uf { get; set; }
        public string Comprador_telefone { get; set; }
        public string Comprador_email { get; set; }
        public int Tipo_Financiamento { get; set; }
        public string Tipo_Instrumento { get; set; }
        public DateTime? Data_Transacao { get; set; }
        public decimal Valor_Transacao { get; set; }
        public decimal Valor_Avaliacao { get; set; }
        public decimal Valor_Avaliacao_atual { get; set; }
        public decimal Valor_Venal { get; set; }
        public decimal Recursos_proprios_valor { get; set; }
        public decimal Recursos_proprios_aliq { get; set; }
        public decimal Recursos_proprios_atual { get; set; }
        public decimal Recursos_conta_valor { get; set; }
        public decimal Recursos_conta_aliq { get; set; }
        public decimal Recursos_conta_atual { get; set; }
        public decimal Recursos_concedido_valor { get; set; }
        public decimal Recursos_concedido_aliq { get; set; }
        public decimal Recursos_concedido_atual { get; set; }
        public decimal Financiamento_valor { get; set; }
        public decimal Financiamento_aliq { get; set; }
        public decimal Financiamento_atual { get; set; }
        public string Totalidade { get; set; }
        public decimal Totalidade_Perc { get; set; }
        public Int64 Matricula { get; set; }
        public int Itbi_Numero  { get; set; }
        public short Itbi_Ano { get; set; }
        public string Inscricao_Incra { get; set; }
        public string Receita_Federal { get; set; }
        public string Descricao_Imovel { get; set; }
        public int Userid { get; set; }
        public int Situacao_itbi { get; set; }
        public int? Liberado_por { get; set; }
        public DateTime? Liberado_em { get; set; }
        public decimal  Valor_guia { get; set; }
        public decimal Valor_guia_atual { get; set; }
        public int Numero_Guia { get; set; }
        public DateTime? Data_Vencimento { get; set; }
        public bool Funcionario { get; set; }
        public bool Utilizar_vvt { get; set; }
    }

    public class ItbiAnoNumero {
        public int Numero { get; set; }
        public short Ano { get; set; }
    }

    public class Itbi_Lista {
        public string Guid { get; set; }
        public short Ano { get; set; }
        public int Numero { get; set; }
        public string Numero_Ano { get; set; }
        public DateTime Data { get; set; }
        public string Nome_Comprador { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
    }

}

