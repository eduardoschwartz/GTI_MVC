using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Assinatura {
        [Key]
        public string Usuario { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public byte[] Fotoass { get; set; }
        public int Codigo { get; set; }
    }
}
