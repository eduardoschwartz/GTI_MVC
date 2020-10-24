using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Notificacao_iss_web {
        [Key]
        public string Guid { get; set; }
        public int Ano_notificacao { get; set; }
        public int Numero_notificacao { get; set; }
        public int Codigo_cidadao { get; set; }
        public int Codigo_imovel { get; set; }
        public DateTime Data_gravacao { get; set; }
        public string Processo { get; set; }
        public decimal Isspago { get; set; }
        public bool Habitese { get; set; }
        public decimal Area { get; set; }
        public int Uso { get; set; }
        public int Categoria { get; set; }
        public decimal Valorm2 { get; set; }
        public decimal Valortotal { get; set; }
        public int Versao { get; set; }
        public DateTime Data_vencimento { get; set; }
        public int Numero_guia { get; set; }
        public string Cpf_cnpj { get; set; }
        public string Nome { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public int Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public int Fiscal { get; set; }
        public int Situacao { get; set; }
    }

    public class Notificacao_iss_web_Struct {
        public string Guid { get; set; }
        public int Ano_notificacao { get; set; }
        public int Numero_notificacao { get; set; }
        public int Codigo_cidadao { get; set; }
        public int Codigo_imovel { get; set; }
        public DateTime Data_gravacao { get; set; }
        public string Processo { get; set; }
        public decimal Isspago { get; set; }
        public bool Habitese { get; set; }
        public decimal Area { get; set; }
        public int Uso { get; set; }
        public int Categoria { get; set; }
        public decimal Valorm2 { get; set; }
        public decimal Valortotal { get; set; }
        public int Versao { get; set; }
        public DateTime Data_vencimento { get; set; }
        public int Numero_guia { get; set; }
        public string Cpf_cnpj { get; set; }
        public string Nome { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public int Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public int Fiscal { get; set; }
        public string Categoria_Nome { get; set; }
        public string Uso_Nome { get; set; }
        public int Situacao { get; set; }
        public string Situacao_nome { get; set; }
    }

}



