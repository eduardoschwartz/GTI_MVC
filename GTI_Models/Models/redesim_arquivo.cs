using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_arquivo {
        [Key]
        public string Guid { get; set; }
        public string Tipo { get; set; }
    }
}

