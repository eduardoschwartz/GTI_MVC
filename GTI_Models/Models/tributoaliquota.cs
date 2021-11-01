
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Tributoaliquota {
        [Key]
        [Column(Order=1)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 2)]
        public  short Codtributo { get; set; }
        public decimal Valoraliq { get; set; }
    }

    public class TributoAliquotaStruct {
        public short Ano { get; set; }
        public short Tributo_Codigo { get; set; }
        public string Tributo_Nome { get; set; }
        public decimal Valor_Aliquota { get; set; }
    }

}
