﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GTI_Models.Models {
    public class Certidao_debito_doc {
        [Key]
        [Column(Order = 1)]
        public int Numero { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Ret { get; set; }
        public DateTime Data_emissao { get; set; }
        public string Nome { get; set; }
        public string Cpf_cnpj { get; set; }
        public string Validacao { get; set; }
        public string Tributo { get; set; }
    }

    public class Certidao_debito_documento {
        public int _Codigo { get; set; }
        public string _Nome { get; set; }
        public int _Ret { get; set; }
        public string _Tributo { get; set; }
    }


}
