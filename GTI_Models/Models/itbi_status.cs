﻿using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Itbi_status {
        [Key]
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    }
}
