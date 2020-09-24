using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_isencao_main {
        [Key]
        public string Guid { get; set; }
        public int Isencao_numero { get; set; }
        public short Isencao_ano { get; set; }
        public int Fiscal_id { get; set; }
        public string Usuario_nome { get; set; }
        public string Usuario_doc { get; set; }
        public int Natureza { get; set; }
        public DateTime Data_cadastro { get; set; }
        public int Situacao { get; set; }
        public int Usuario_id { get; set; }
        public string Data_validade { get; set; }
    }

    public class Itbi_isencao_main_Struct {
        [Key]
        public string Guid { get; set; }
        public int Isencao_numero { get; set; }
        public short Isencao_ano { get; set; }
        public int Fiscal_id { get; set; }
        public string Usuario_nome { get; set; }
        public string Usuario_doc { get; set; }
        public int Natureza { get; set; }
        public DateTime Data_cadastro { get; set; }
        public int Situacao { get; set; }
        public int Usuario_id { get; set; }
        public string Data_validade { get; set; }
        public string Situacao_Nome { get; set; }
        public string Natureza_Nome { get; set; }
    }
}
