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
    }
}
