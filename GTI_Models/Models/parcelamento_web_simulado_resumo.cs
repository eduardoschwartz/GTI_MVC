using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Parcelamento_Web_Simulado_Resumo {
        [Key]
        [Column(Order = 1)]
        public string Guid { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Qtde_Parcela { get; set; }
        public decimal Valor_Entrada { get; set; }
        public decimal Valor_N { get; set; }
        public decimal Valor_Total { get; set; }
    }
}
