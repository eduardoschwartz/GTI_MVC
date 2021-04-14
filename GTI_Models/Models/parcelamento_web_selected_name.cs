using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_Web_Selected_Name {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Lancamento { get; set; }
    }

    public class Parcelamento_Web_Selected_Name_Struct {
        public short Ano { get; set; }
        public short Lancamento_Codigo { get; set; }
        public string Lancamento_Nome { get; set; }
    }

}
