using System;
using System.Collections.Generic;

namespace GTI_Mvc.ViewModels {
    public class NotificacaoIssViewModel {
        public string Guid { get; set; }
        public int Ano_Notificacao { get; set; }
        public int Numero_Notificacao { get; set; }
        public int Codigo_Imovel { get; set; }
        public string Numero_Processo { get; set; }
        public int Uso_Construcao { get; set; }
        public int Categoria_Construcao { get; set; }
        public decimal Valor_m2 { get; set; }
        public DateTime Data_vencimento { get; set; }
        public int Codigo_Cidadao { get; set; }
        public decimal Area_Notificada { get; set; }
        public decimal Iss_Pago { get; set; }
        public bool Habitese { get; set; }
        public decimal Valor_Total { get; set; }
        public string Nome { get; set; }
        public int SituacaoCodigo { get; set; }
        public string SituacaoNome { get; set; }
        public DateTime Data_Emissao { get; set; }
        public string AnoNumero { get; set; }
        public string Ano_Selected { get; set; }
    }

}