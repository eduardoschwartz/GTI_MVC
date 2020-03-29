using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class dados_imovel_web {
        [Key]
        public string Controle { get; set; }
        public int Codigo { get; set; }
        public string Proprietario { get; set; }
        public string Inscricao { get; set; }
        public string Ativo { get; set; }
        public string Endereco { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Quadra { get; set; }
        public string Lote { get; set; }
        public string Cep { get; set; }
        public decimal Areaterreno { get; set; }
        public decimal Fracaoideal { get; set; }
        public string Topografia { get; set; }
        public string Pedologia { get; set; }
        public string Situacao { get; set; }
        public string Usoterreno { get; set; }
        public string Benfeitoria { get; set; }
        public string Categoria { get; set; }
        public decimal Testada { get; set; }
        public decimal Agrupamento { get; set; }
        public decimal Somafator { get; set; }
        public decimal Vvt { get; set; }
        public decimal Vvc { get; set; }
        public decimal Vvi { get; set; }
        public decimal Iptu { get; set; }
        public decimal Areapredial { get; set; }
        public string Condominio { get; set; }
        public string Imunidade { get; set; }
        public string Reside { get; set; }
        public string Isentocip { get; set; }
        public int Qtdeedif { get; set; }
        public string Mt { get; set; }
        public DateTime Data_impressao { get; set; }
        public string Proprietario2 { get; set; }
        public int Ano_Certidao { get; set; }
        public int Numero_Certidao { get; set; }
    }
}
