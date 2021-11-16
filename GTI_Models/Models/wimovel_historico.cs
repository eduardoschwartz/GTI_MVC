using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class WImovel_Historico {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Seq { get; set; }
        public string Data_Alteracao { get; set; }
        public string Historico { get; set; }
        public int Usuario_Codigo { get; set; }
        public string Usuario_Nome { get; set; }
    }
}
