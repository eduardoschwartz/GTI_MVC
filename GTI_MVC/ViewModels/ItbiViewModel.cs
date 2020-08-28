using GTI_Models.Models;
using GTI_Mvc.Views.Imovel.EditorTemplates;
using System;
using System.Collections.Generic;

namespace GTI_Mvc.ViewModels {
    public class ItbiViewModel {
        public string Guid { get; set; }
        public int Itbi_Numero { get; set; }
        public short Itbi_Ano { get; set; }
        public string Itbi_NumeroAno { get; set; }
        public string Tipo_Imovel { get; set; }
        public int Situacao_Itbi_codigo { get; set; }
        public string Situacao_Itbi_Nome { get; set; }
        public DateTime Data_cadastro { get; set; }
        public string Codigo { get; set; }
        public string Inscricao { get; set; }
        public int Proprietario_codigo { get; set; }
        public string Proprietario_nome { get; set; }
        public ImovelStruct Dados_Imovel { get; set; }
        public Comprador_Itbi Comprador { get; set; }
        public int Natureza_Codigo { get; set; }
        public string Natureza_Nome { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string Comprador_Cpf_cnpj_tmp { get; set; }
        public string Comprador_Nome_tmp { get; set; }
        public string Vendedor_Cpf_cnpj_tmp { get; set; }
        public string Vendedor_Nome_tmp { get; set; }
        public List<ListCompradorEditorViewModel> Lista_Comprador { get; set; }
        public List<ListVendedorEditorViewModel> Lista_Vendedor { get; set; }
        public decimal Valor_Transacao { get; set; }
        public decimal Valor_Avaliacao { get; set; }
        public decimal Valor_Avaliacao_atual { get; set; }
        public decimal Valor_Venal { get; set; }
        public decimal Valor_Venal_Territorial { get; set; }
        public decimal Recursos_proprios_valor { get; set; }
        public decimal Recursos_proprios_aliq { get; set; }
        public decimal Recursos_proprios_atual { get; set; }
        public decimal Recursos_conta_valor { get; set; }
        public decimal Recursos_conta_aliq { get; set; }
        public decimal Recursos_conta_atual { get; set; }
        public decimal Recursos_concedido_valor { get; set; }
        public decimal Recursos_concedido_aliq { get; set; }
        public decimal Recursos_concedido_atual { get; set; }
        public decimal Financiamento_valor { get; set; }
        public decimal Financiamento_aliq { get; set; }
        public decimal Financiamento_atual { get; set; }
        public int Tipo_Financiamento { get; set; }
        public string Tipo_Financiamento_Nome { get; set; }
        public DateTime? Data_Transacao { get; set; }
        public string Tipo_Instrumento { get; set; } = "Particular";
        public string[] Lista_Instrumento = new[] { "Particular", "Público" };
        public string[] Lista_Tipo_Imovel = new[] { "Urbano", "Rural" };
        public string Totalidade { get; set; }
        public string[] Lista_Totalidade = new[] { "Sim", "Não" };
        public decimal Totalidade_Perc { get; set; }
        public Int64 Matricula { get; set; }
        public string Inscricao_Incra { get; set; }
        public string Receita_Federal { get; set; }
        public string Descricao_Imovel { get; set; }
        public List<string> Lista_Erro { get; set; }
        public List<Imovel_Isencao> Lista_Isencao { get; set; }
        public List<ListAnexoEditorViewModel> Lista_Anexo { get; set; }
        public string Anexo_Nome_tmp { get; set; }
        public string Anexo_Desc_tmp { get; set; }
        public int UserId { get; set; }
        public decimal Valor_guia { get; set; }
        public decimal Valor_guia_atual { get; set; }
        public int Numero_Guia { get; set; }
        public DateTime? Data_Vencimento { get; set; }
        public bool Funcionario { get; set; }
        public bool Utilizar_VVT { get; set; }
        public string[] Lista_Status = new[] { "0-Exibir todos os ITBI's", "1-Enviado para análise","2-Aguardando pagamento","3-ITBI's Concluídos","4-ITBI's Cancelados" };
        public string Status_Query { get; set; }

        public ItbiViewModel() {
            Lista_Comprador = new List<ListCompradorEditorViewModel>();
            Lista_Vendedor = new List<ListVendedorEditorViewModel>();
        }
    }

    public class Anexo_Itbi {
        public int Seq { get; set; }
        public string Id { get; set; }
        public string Descricao { get; set; }
        public string Arquivo { get; set; }
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

    public class Itbi_Forum {
        public string Guid { get; set; }
        public short Seq { get; set; }
        public DateTime Datahora { get; set; }
        public int User_id { get; set; }
        public string User_Name { get; set; }
        public string User_Email { get; set; }
        public string User_Fone { get; set; }
        public string Mensagem { get; set; }
        public string Ano_Numero { get; set; }
        public string Tipo_Itbi { get; set; }
        public string Comprador_Nome { get; set; }
        public DateTime Data_Itbi { get; set; }
        public string Action { get; set; }
    }

    public class Imovel_Isencao {
        public int Seq { get; set; }
        public string Tipo { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }



}