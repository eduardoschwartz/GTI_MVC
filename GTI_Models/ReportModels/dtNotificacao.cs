using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTI_Models.ReportModels {
    public class DtNotificacao {
        public string AnoNumero { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Endereco_Local { get; set; }
        public string Endereco_Entrega { get; set; }
        public string Endereco_Prop { get; set; }
        public int Prazo { get; set; }
        public string Usuario { get; set; }
        public DateTime Data_Cadastro { get; set; }
        public string Inscricao { get; set; }
        public string PrazoText { get; set; }
    }
}
