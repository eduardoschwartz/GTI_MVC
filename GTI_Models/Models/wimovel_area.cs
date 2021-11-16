using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class WImovel_Area {
        [Key]
        [Column(Order =1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Seq { get; set; }
        public decimal Area { get; set; }
        public short Uso_codigo { get; set; }
        public string Uso_nome { get; set; }
        public short Tipo_codigo { get; set; }
        public string Tipo_nome { get; set; }
        public short Categoria_codigo { get; set; }
        public string Categoria_nome { get; set; }
        public DateTime? Data_Aprovacao { get; set; }
        public string Processo_Numero { get; set; }
        public DateTime? Processo_Data { get; set; }
        public short Pavimentos { get; set; }

    }
}

