using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Itbi_forum {
        [Key]
        [Column(Order =1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Seq { get; set; }
        public DateTime Datahora { get; set; }
        public int Userid { get; set; }
        public string Mensagem { get; set; }
    }
}
