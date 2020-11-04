using System;

namespace GTI_Models.ReportModels {
    public class UsoPlataformaReport {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Final { get; set; }
        public int Qtde1 { get; set; }
        public decimal Aliquota1 { get; set; }
        public decimal Valor1P { get; set; }
        public decimal Valor1M { get; set; }
        public decimal Valor1J { get; set; }
        public decimal Valor1T { get; set; }
        public int Qtde2 { get; set; }
        public decimal Aliquota2 { get; set; }
        public decimal Valor2P { get; set; }
        public decimal Valor2M { get; set; }
        public decimal Valor2J { get; set; }
        public decimal Valor2T { get; set; }
        public int Qtde3 { get; set; }
        public decimal Aliquota3 { get; set; }
        public decimal Valor3P { get; set; }
        public decimal Valor3M { get; set; }
        public decimal Valor3J { get; set; }
        public decimal Valor3T { get; set; }
        public string Nosso_Numero { get; set; }
        public string Linha_Digitavel { get; set; }
        public string Codigo_Barra { get; set; }
        public int Numero_Guia { get; set; }
        public decimal Valor_Guia { get; set; }
    }
}
