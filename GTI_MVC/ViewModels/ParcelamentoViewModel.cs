using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc.ViewModels {
    public class ParcelamentoViewModel {
        public int Processo_ano { get; set; }
        public int Processo_numero { get; set; }
        public string NumeroProcesso { get; set; }
        public Parc_Requerente Requerente { get; set; }
        public List<SelectListItem> Lista_Codigos { get; set; }
    }

    public class Parc_Requerente {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cpf_Cnpj { get; set; }
        public int Logradouro_Codigo { get; set; }
        public string Logradouro_Nome { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public int Bairro_Codigo { get; set; }
        public string Bairro_Nome { get; set; }
        public int Cidade_Codigo { get; set; }
        public string Cidade_Nome { get; set; }
        public string UF { get; set; }
        public string Telefone { get; set; }
        public string TipoEnd { get; set; }
        public string Email { get; set; }
    }


}