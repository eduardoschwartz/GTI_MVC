
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
        public string Contribuinte_tipo { get; set; }
        public DateTime? Data_Vencimento { get; set; }
        public DateTime Data_Geracao { get; set; }
        public int Plano_Codigo { get; set; }
        public string Plano_Nome { get; set; } ="Nenhum";
        public decimal Valor_minimo { get; set; }
        public bool Refis { get; set; }
        public decimal Perc_Desconto { get; set; }
        public int Qtde_Maxima_Parcela { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Entrada { get; set; }
        public decimal Perc_Principal { get; set; }
        public decimal Perc_Juros { get; set; }
        public decimal Perc_Multa { get; set; }
        public decimal Perc_Correcao { get; set; }
        public decimal Valor_add_Principal { get; set; }
        public decimal Valor_add_Juros { get; set; }
        public decimal Valor_add_Multa { get; set; }
        public decimal Valor_add_Correcao { get; set; }
        public int Qtde_Parcela { get; set; }
        public decimal Sim_Liquido { get; set; }
        public decimal Sim_Juros { get; set; }
        public decimal Sim_Multa { get; set; }
        public decimal Sim_Correcao { get; set; }
        public decimal Sim_Principal { get; set; }
        public decimal Sim_Honorario { get; set; }
        public decimal Sim_Juros_apl { get; set; }
        public decimal Sim_Total { get; set; }
        public decimal Sim_Perc_Liquido { get; set; }
        public decimal Sim_Perc_Juros { get; set; }
        public decimal Sim_Perc_Multa { get; set; }
        public decimal Sim_Perc_Correcao { get; set; }
        public decimal Sim_Perc_Juros_Apl { get; set; }
        public decimal Sim_Perc_Honorario { get; set; }
    }
}
