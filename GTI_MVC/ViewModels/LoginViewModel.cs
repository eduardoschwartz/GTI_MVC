using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class LoginViewModel {
        [Display(Name = "Usuário: ")]
        public string Usuario { get; set; }
        [Display(Name = "Senha: ")]
        public string Senha { get; set; }
        [Display(Name = "Nova Senha: ")]
        public string Senha2 { get; set; }
        [Display(Name = "Confimação: ")]
        public string Senha3 { get; set; }
        public string ErrorMessage { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        [Required]
        [StringLength(6)]
        public string CaptchaCode { get; set; }
        [Display(Name = "Chave de validação")]
        public string Chave { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool RememberMe { get; set; }
    }
}
