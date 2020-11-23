using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {

    public class Uf {
        [Key]
        public string Siglauf { get; set; }
        public string Descuf{ get; set; }
        public int Cep1 { get; set; }
        public int Cep2 { get; set; }

//        public List<Cidade> Cidades { get; set; }
    }

}
