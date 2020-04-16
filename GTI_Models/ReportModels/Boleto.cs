using System;

namespace GTI_Models.ReportModels {
    public class Boleto {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Quadra { get; set; }
        public string Lote { get; set; }
        public string Inscricao { get; set; }
        public decimal Fracao_Ideal { get; set; }
        public string Natureza { get; set; }
        public decimal Area_Territorial { get; set; }
        public decimal Area_Predial { get; set; }
        public decimal Testada { get; set; }
        public decimal Vvt { get; set; }
        public decimal Vvc { get; set; }
        public decimal Vvi { get; set; }
        public decimal Valor_Iptu { get; set; }
        public decimal Valor_Itu { get; set; }
        public decimal Total_Parcela { get; set; }
        public decimal Valor_Unica1 { get; set; }
        public decimal Valor_Unica2 { get; set; }
        public decimal Valor_Unica3 { get; set; }
        public string  Pacela_Label { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public decimal Valor_Guia { get; set; }
        public string Nosso_Numero { get; set; }
        public int Numero_Documento { get; set; }
        public DateTime Data_Documento { get; set; }
        public string Codigo_Barra { get; set; }
        public string Digitavel { get; set; }
    }
}
