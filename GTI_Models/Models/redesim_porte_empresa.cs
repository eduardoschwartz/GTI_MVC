﻿using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_porte_empresa {
        [Key]
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }
}
