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
        public Parc_Contribuinte Contribuinte { get; set; }
        public  List<Parc_Codigos> Lista_Codigos { get; set; }
        public string Data_Vencimento { get; set; }
        public int Plano_Codigo { get; set; }
        public string Plano_Nome { get; set; } = "Nenhum";
        public string[] Lista_Plano_Desconto = new[] { "Nenhum", "Refis" };
        public List<SelectDebitoParcelamentoEditorViewModel> Lista_Origem { get; set; }
        public List<SelectDebitoParcelamentoEditorViewModel> Lista_Origem_Selected { get; set; }
        public List<Parcelamento_Web_Simulado> Lista_Simulado { get; set; }
        public decimal Soma_Principal { get; set; }
        public decimal Soma_Juros { get; set; }
        public decimal Soma_Multa { get; set; }
        public decimal Soma_Correcao { get; set; }
        public decimal Soma_Total { get; set; }
        public decimal Soma_Penalidade { get; set; }
        public int Qtde_Maxima_Parcela { get; set; }
        public decimal Perc_desconto { get; set; }
        public decimal Valor_Minimo { get; set; }
        public List<Parc_Resumo> Lista_Resumo { get; set; }



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

    public class Parc_Contribuinte {
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
        public string Tipo { get; set; }
    }

    public class Parc_Resumo {
        public int Qtde_Parcela { get; set; }
        public string Valor_Entrada { get; set; }
        public string Valor_N { get; set; }
        public string Valor_Total { get; set; }
        public string Texto { get; set; }
        public bool Selected { get; set; }
    }

    public class Parc_Codigos {
        public int Codigo { get; set; }
        public string Tipo { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string Descricao { get; set; }
        public bool Selected { get; set; }
    }


}