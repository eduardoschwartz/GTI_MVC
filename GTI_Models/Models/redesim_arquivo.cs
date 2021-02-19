using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_arquivo {
        [Key]
        public string Guid { get; set; }
        public DateTime Periodode { get; set; }
        public DateTime Periodoate { get; set; }
        public char Tipo { get; set; }
    }
}

