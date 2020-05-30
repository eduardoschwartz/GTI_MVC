using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Usuario_web {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public string Cpf_Cnpj { get; set; }
        public bool Ativo { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public bool Bloquedo { get; set; }
    }
}
