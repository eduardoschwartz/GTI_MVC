using GTI_Models.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class DebitoViewModel {
        [Display(Name = "Inscrição Municipal")]
      //  public int Tipo_Cadastro { get; set; }
        public string Inscricao { get; set; }
        public string Nome { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        public string ErrorMessage { get; set; }
        [Display(Name = "Nº do Processo")]
        public string Numero_Processo { get; set; }
        [Display(Name = "Data de vencimento")]
        public string DataVencimento { get; set; }
        public List<DebitoStructureWeb> Lista_Debito { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Honorario { get; set; }
        public int Plano { get; set; }
        [StringLength(4)]
        public string CaptchaCode { get; set; }
        public string Cadastro { get; set; } = "Imóvel";
        public string[] Tipo_Cadastro = new[] { "Imóvel", "Empresa", "Cidadão" };
    }

}

