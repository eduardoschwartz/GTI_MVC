using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class W_Imovel_Main {
        [Key]
        public string Guid { get; set; }
        public int Codigo { get; set; }
        public bool Cip { get; set; }
        public bool Imune { get; set; }
        public bool Conjugado { get; set; }
        public bool Reside { get; set; }
        public decimal Area_Terreno { get; set; }
        public short Topografia { get; set; }
        public short Pedologia { get; set; }
        public short Benfeitoria { get; set; }
        public short Categoria { get; set; }
        public short Usoterreno { get; set; }
        public short Situacao { get; set; }
        public int Userid { get; set; }
        public string Inscricao { get; set; }
        public string Condominio { get; set; }
        public DateTime Data_Alteracao { get; set; }
    }
}
