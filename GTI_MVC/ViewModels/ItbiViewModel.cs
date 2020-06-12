using GTI_Models.Models;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using System;
using System.Collections.Generic;

namespace GTI_Mvc.ViewModels {
    public class ItbiViewModel {
        public string Guid { get; set; }
        public DateTime Data_cadastro { get; set; }
        public string Codigo { get; set; }
        public string Inscricao { get; set; }
        public int Proprietario_codigo { get; set; }
        public string Proprietario_nome { get; set; }
        public ImovelStruct Dados_Imovel { get; set; }
        public Comprador_Itbi Comprador { get; set; }
        public int Natureza_Codigo { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string Comprador_Cpf_cnpj_tmp { get; set; }
        public string Comprador_Nome_tmp { get; set; }
        public List<ListCompradorEditorViewModel> Lista_Comprador { get; set; }
        public ItbiViewModel() {
            this.Lista_Comprador = new List<ListCompradorEditorViewModel>();
        }

    }

    public class Comprador_Itbi {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cnpj { get; set; }
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
        public string Email { get; set; }
    }

}