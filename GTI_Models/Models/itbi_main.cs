using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_main {
        [Key]
        public string Guid { get; set; }
        public DateTime Data_cadastro { get; set; }
        public int Imovel_codigo { get; set; }
    }
}
