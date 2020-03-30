using GTI_Models.Models;
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class EmpresaDetailsViewModel {
        public EmpresaStruct EmpresaStruct { get; set; }
        [Display(Name = "Inscrição Municipal")]
        public string Inscricao { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        public string TaxaLicenca { get; set; }
        public string Vigilancia_Sanitaria { get; set; }
        public string Regime_Iss { get; set; }
        public string Mei { get; set; }
        public string Cnae { get; set; }
        public string ErrorMessage { get; set; }
        public string CaptchaCode { get; set; }
    }
}
