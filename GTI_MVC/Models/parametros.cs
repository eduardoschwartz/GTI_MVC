﻿
using System.ComponentModel.DataAnnotations;

namespace GTI_MVC.Models {
    public class Parametros {
        [Key]
        public short Seq { get; set; }
        public string Nomeparam { get; set; }
        public string Valparam { get; set; }
    }
}
