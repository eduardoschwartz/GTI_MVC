using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Notificacao_terreno {
        [Key]
        [Column(Order=1)]
        public int Ano_not { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Numero_not { get; set; }
        public int Codigo { get; set; }
        public int Situacao { get; set; }
        public string Endereco_infracao { get; set; }
        public string Endereco_prop { get; set; }
        public string Endereco_entrega { get; set; }
        public string Nome { get; set; }
        public string Inscricao { get; set; }
        public int Prazo { get; set; }
        public DateTime Data_cadastro { get; set; }
        public int Userid { get; set; }
    }
}
