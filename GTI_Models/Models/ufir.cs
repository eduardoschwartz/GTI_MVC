using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Ufir {
        [Key]
        public short Anoufir { get; set; }
        public Single Valorufir { get; set; }
        public Single Ipca { get; set; }
    }
}
