using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Mobreq_main {
        [Key]
        public string Guid { get; set; }
        public int Tipo { get; set; }
        public int Codigo { get; set; }
        public DateTime Data_Inclusao { get; set; }
        public DateTime Data_Evento { get; set; }
        public string Obs { get; set; }
        public int UserId { get; set; }
        public bool UserPrf { get; set; }
        public short Situacao { get; set; }
        public int? UserId2 { get; set; }
        public DateTime? Data_Evento2 { get; set; }
    }

    public class Mobreq_main_Struct {
        public string Guid { get; set; }
        public int Tipo_Codigo { get; set; }
        public string Tipo_Nome { get; set; }
        public int Codigo { get; set; }
        public string Razao_Social { get; set; }
        public string CpfCnpj { get; set; }
        public DateTime Data_Inclusao { get; set; }
        public DateTime Data_Evento { get; set; }
        public string Obs { get; set; }
        public int UserId { get; set; }
        public bool UserPrf { get; set; }
        public short Situacao_Codigo { get; set; }
        public string Situacao_Nome { get; set; }
        public int?  UserId2_Codigo { get; set; }
        public string UserId2_Nome { get; set; }
        public DateTime? Data_Evento2 { get; set; }
    }


}
