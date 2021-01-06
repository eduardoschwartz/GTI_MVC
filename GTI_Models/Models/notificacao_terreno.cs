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

    public class Notificacao_Terreno_Struct {
        public int Ano_Notificacao { get; set; }
        public int Numero_Notificacao { get; set; }
        public string AnoNumero { get; set; }
        public int Codigo_Imovel { get; set; }
        public int Situacao { get; set; }
        public string Inscricao { get; set; }
        public string Nome_Proprietario { get; set; }
        public string Endereco_Local { get; set; }
        public string Endereco_Entrega { get; set; }
        public string Endereco_Prop { get; set; }
        public int Prazo { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public int Userid  { get; set; }
        public string UsuarioNome { get; set; }

    }


}
