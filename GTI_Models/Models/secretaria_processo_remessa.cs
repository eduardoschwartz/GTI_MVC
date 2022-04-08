using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Secretaria_processo_remessa {
        [Key]
        [Column(Order=1)]
        public short Codigo { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime Data { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Seq { get; set; }
        public int Qtde { get; set; }
    }
}
