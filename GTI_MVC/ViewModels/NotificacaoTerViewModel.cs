using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class NotificacaoTerViewModel {
        public int Ano_Notificacao { get; set; }
        public int Numero_Notificacao { get; set; }
        public string AnoNumero { get; set; }
        public int Codigo_Imovel { get; set; }
        public int Situacao { get; set; }
        public string Inscricao { get; set; }
        public string Nome_Proprietario { get; set; }
        public string Endereco_Local { get; set; }
        public string Endereco_Entrega { get; set; }
        public string Endereco_Prop { get; set; }
        public int Prazo { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public string Nome2 { get; set; }
        public string Endereco_prop2 { get; set; }
        public string Endereco_entrega2 { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public string Cpf2 { get; set; }
        public string Rg2 { get; set; }
        public int Codigo_cidadao { get; set; }
        public int Codigo_cidadao2 { get; set; }

    }

    public class NotificacaoTerQueryViewModel {
        public int Ano_Selected { get; set; }
        public int Situacao_Selected { get; set; }
        public List<NotificacaoTerViewModel> ListaNotificacao { get; set; }
    }

}