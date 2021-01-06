using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class NotificacaoTerViewModel {
        public int Ano_Notificacao { get; set; }
        public int Numero_Notificacao { get; set; }
        public int Codigo_Imovel { get; set; }
        public int Situacao { get; set; }
        public string Inscricao { get; set; }
        public List<ProprietarioStruct> Proprietarios { get; set; }
        public string Endereco_Local { get; set; }
        public string Endereco_Entrega { get; set; }
        public string Endereco_Prop { get; set; }
        public int Prazo { get; set; }
    }
}