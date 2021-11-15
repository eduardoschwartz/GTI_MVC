using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class WImovel_Main {
        [Key]
        public string Guid { get; set; }
        public int Codigo { get; set; }
        public bool Cip { get; set; }
        public bool Imune { get; set; }
        public bool Conjugado { get; set; }
        public bool Reside { get; set; }
        public decimal Area_Terreno { get; set; }
        public short Topografia { get; set; }
        public short Pedologia { get; set; }
        public short Benfeitoria { get; set; }
        public short Categoria { get; set; }
        public short Usoterreno { get; set; }
        public short Situacao { get; set; }
        public int Userid { get; set; }
        public string Inscricao { get; set; }
        public int Condominio { get; set; }
        public DateTime Data_Alteracao { get; set; }
        public string Condominio_Nome { get; set; }
        public string Topografia_Nome { get; set; }
        public string Pedologia_Nome { get; set; }
        public string Benfeitoria_Nome { get; set; }
        public string Categoria_Nome { get; set; }
        public string Usoterreno_Nome { get; set; }
        public string Situacao_Nome { get; set; }
        public string Quadra_Original { get; set; }
        public string Lote_Original { get; set; }
        public char Tipo_Matricula { get; set; }
        public Int64 Numero_Matricula { get; set; }
        public short Tipo_Endereco { get; set; }
    }
}
