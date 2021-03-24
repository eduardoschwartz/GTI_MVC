using GTI_Models.Models;
using GTI_Mvc.Views.Parcelamento.EditorTemplates;
using GTI_Mvc.Views.Tributario.EditorTemplates;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GTI_Mvc.ViewModels {
    public class ParcelamentoViewModel {
        public string  Guid { get; set; }
        public int Processo_ano { get; set; }
        public int Processo_numero { get; set; }
        public string NumeroProcesso { get; set; }
        public Parc_Requerente Requerente { get; set; }
        public Parc_Requerente Contribuinte { get; set; }
        public  List<SelectListItem> Lista_Codigos { get; set; }
        public string Data_Vencimento { get; set; }
        public int Plano_Codigo { get; set; }
        public string Plano_Nome { get; set; } = "Nenhum";
        public string[] Lista_Plano_Desconto = new[] { "Nenhum", "Refis" };
        public List<SelectDebitoParcelamentoEditorViewModel> Lista_Origem { get; set; }
        public List<SelectDebitoParcelamentoEditorViewModel> Lista_Origem_Selected { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Penalidade { get; set; }

        public ParcelamentoViewModel() {
            this.Lista_Origem = new List<SelectDebitoParcelamentoEditorViewModel>();
        }

        public IEnumerable<int> getSelectedIds() {
            return (from p in this.Lista_Origem where p.Selected select p.Idx).ToList();
        }

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
        public string Bairro_Nome { get; set; }
        public string Cidade_Nome { get; set; }
        public string UF { get; set; }
        public string Telefone { get; set; }
        public string TipoEnd { get; set; }
        public string Email { get; set; }
    }


}