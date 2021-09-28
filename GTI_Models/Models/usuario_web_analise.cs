using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Usuario_Web_Analise {
        [Key]
        [Column(Order=1)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime Data_envio { get; set; }
        public bool Autorizado { get; set; }
        public DateTime? Data_autorizado { get; set; }
        public int Autorizado_por { get; set; }
    }

    public class Usuario_Web_Analise_Struct {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Email { get; set; }
        public DateTime Data_envio { get; set; }
        public bool Autorizado { get; set; }
        public DateTime? Data_autorizado { get; set; }
        public int Fiscal_Codigo { get; set; }
        public string Fiscal_Nome { get; set; }
    }

}


