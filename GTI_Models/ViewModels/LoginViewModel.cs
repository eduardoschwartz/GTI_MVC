using System.ComponentModel.DataAnnotations;

namespace GTI_Models.ViewModels {
    public class LoginViewModel {
        [Display(Name = "Usuário: ")]
        public string Usuario { get; set; }
        [Display(Name = "Senha: ")]
        public string Senha { get; set; }
        public string ErrorMessage { get; set; }
    }
}
