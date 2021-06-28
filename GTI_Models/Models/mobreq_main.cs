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
    }
}
