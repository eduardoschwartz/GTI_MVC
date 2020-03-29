
using System.ComponentModel.DataAnnotations;

namespace GTI_Mvc.Models {
    public class Parametros {
        [Key]
        public short Seq { get; set; }
        public string Nomeparam { get; set; }
        public string Valparam { get; set; }
    }
}
