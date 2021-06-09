using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class RodoviariaViewModel {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public int Ano { get; set; }
        public List<Rodo_uso_plataforma_Struct> Lista_uso_plataforma { get; set; }
        public DateTime DataDe { get; set; }
        public DateTime DataAte { get; set; }
        public int Qtde1 { get; set; }
        public int Qtde2 { get; set; }
        public int Qtde3 { get; set; }
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
    }
}