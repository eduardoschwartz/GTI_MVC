using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Plano {
        [Key]
        public short Codigo { get; set; }
        public string Nome { get; set; }
        public decimal Desconto { get; set; }
        public short Ano { get; set; }
        public bool Distrito_Industrial { get; set; }
        public bool Dam { get; set; }
        public  int Qtde_Parcela { get; set; }

        public static explicit operator Plano(decimal v) {
            throw new NotImplementedException();
        }
    }
}
