using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class ItbiViewModel {
        public string Codigo { get; set; }
        public string Inscricao { get; set; }
        public ImovelStruct Dados_Imovel { get; set; }
        public CidadaoStruct Comprador { get; set; }
        public int Natureza_Codigo { get; set; }
    }
}