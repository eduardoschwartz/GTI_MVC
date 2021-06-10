using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Rodo_uso_plataforma {
        [Key]
        [Column(Order = 1)]
        public int Codigo { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime Datade { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime Dataate { get; set; }
        [Key]
        [Column(Order = 4)]
        public byte Seq { get; set; }
        public byte SeqDebito { get; set; }
        public int Qtde1 { get; set; }
        public int Qtde2 { get; set; }
        public int Qtde3 { get; set; }
        public int Numero_Guia { get; set; }
        public decimal Valor_Guia { get; set; }
        public int Situacao { get; set; }
        public string Anexo { get; set; }
    }

    public class Rodo_uso_plataforma_Struct {
        public int Codigo { get; set; }
        public DateTime Datade { get; set; }
        public DateTime Dataate { get; set; }
        public byte Seq { get; set; }
        public byte SeqDebito { get; set; }
        public int Qtde1 { get; set; }
        public int Qtde2 { get; set; }
        public int Qtde3 { get; set; }
        public int Numero_Guia { get; set; }
        public decimal Valor_Guia { get; set; }
        public int Situacao { get; set; }
        public string Situacao_Nome { get; set; }
        public string Anexo { get; set; }
    }

    
}
