using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Processo_web {
        [Key]
        public string Guid { get; set; }
        public DateTime Data_geracao { get; set; }
        public int Centro_custo_codigo { get; set; }
        public string Centro_custo_nome { get; set; }
        public bool Interno { get; set; }
    }
}
