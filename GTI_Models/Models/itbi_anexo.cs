using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Itbi_anexo {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public byte Seq { get; set; }
        public string Descricao { get; set; }
        public string Arquivo { get; set; }
    }
}
