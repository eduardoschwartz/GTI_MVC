using GTI_Models.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.ViewModels {
    public class CertidaoViewModel {
        public ImovelStruct ImovelStruct { get; set; }
        public EmpresaStruct EmpresaStruct { get; set; }
        [Display(Name = "Inscrição Municipal")]
        public string Inscricao { get; set; }
        public string Nome { get; set; }
        public string SelectedValue { get; set; }
        public List<SelectListaItem> OptionList { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        [Display(Name = "Nº de documento")]
        public string Documento { get; set; }
        public string ErrorMessage { get; set; }
        [Required]
        [StringLength(4)]
        public string CaptchaCode { get; set; }
        [Display(Name = "Chave de validação")]
        public string Chave { get; set; }
        public bool Extrato { get; set; }
        [Display(Name = "Data de vencimento")]
        public string DataVencimento { get; set; }
    }
}
