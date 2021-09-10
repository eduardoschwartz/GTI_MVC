using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class Processo2ViewModel {
        public string Numero_Processo { get; set; }
        public int AnoProcesso { get; set; }
        public int NumProcesso { get; set; }
        public string Guid { get; set; }
        public string Tipo_Requerente { get; set; }
        public int Centro_Custo_Codigo { get; set; }
        public string Centro_Custo_Nome { get; set; }
        public string Centro_Custo_CpfCnpj { get; set; }
        public bool Fisico { get; set; }
        public string Fisico_Nome { get; set; }
        public string Interno { get; set; }
        public int Assunto_Codigo { get; set; }
        public string Assunto_Nome { get; set; }
        public int Endereco_Codigo { get; set; }
        public string Endereco_Nome { get; set; }
        public int Endereco_Numero { get; set; }
        public string Complemento { get; set; }
        public string Observacao { get; set; }
        public string Data_Entrada { get; set; }
        public List<ProcessoEndStruct> Lista_Endereco { get; set; }
        public List<ProcessoDocStruct> Lista_Documento { get; set; }
        public string Erro { get; set; }
    }
     


}