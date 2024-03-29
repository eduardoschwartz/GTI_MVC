﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Imunidade_Issqn {
        [Key]
        [Column(Order = 1)]
        public int Ano { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Numero { get; set; }
        public int Codigo { get; set; }
        public string Razao_social { get; set; }
        public string Endereco { get; set; }
        public DateTime Data_Gravada { get; set; }
        public byte[] QRCodeImage { get; set; }
        public string Documento { get; set; }
        public string Controle { get; set; }
    }
}
