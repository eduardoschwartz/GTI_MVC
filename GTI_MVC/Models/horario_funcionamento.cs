﻿using System.ComponentModel.DataAnnotations;

namespace GTI_MVC.Models {
    public class Horario_funcionamento {
        [Key]
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}
