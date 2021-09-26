using GTI_Models.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.ViewModels {
    public class LoginViewModel {
        public int UserId { get; set; }
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
        public List<Usuario_web> Lista_Usuario_Web { get; set; }
        public string Filter { get; set; }
        public List<Usuario_Web_Anexo_Struct> Lista_Usuario_Web_Anexo { get; set; }
    }
}
