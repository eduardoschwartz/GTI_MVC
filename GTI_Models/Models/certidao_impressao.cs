using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Certidao_impressao {
        [Key]
        [Column(Order = 1)]
        public int Ano { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Numero { get; set; }
        public string Codigo { get; set; }
        public string Numero_Ano { get; set; }
        public string Inscricao { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public int Endereco_Numero { get; set; }
        public string Endereco_Complemento { get; set; }
        public string Bairro { get; set; }
        public string Quadra_Original { get; set; }
        public string Lote_Original { get; set; }
        public string Tipo_Certidao { get; set; }
        public string Nao { get; set; }
        public string Tributo { get; set; }
        public byte[] QRCodeImage { get; set; }
    }
}
