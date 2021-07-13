using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class LogWeb {
        [Key]
        public int Id { get; set; }
        public DateTime Data_evento { get; set; }
        public int UserId { get; set; }
        public bool Pref { get; set; }
        public short Evento { get; set; }
        public string Obs { get; set; }
    }
}
