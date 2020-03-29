using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GTI_MVC.ViewModels {
    public class ProcessoViewModel {
        public int Numero { get; set; }
        public int Ano { get; set; }
        public int Seq { get; set; }
        public string Numero_Ano { get; set; }
        public string Data_Processo { get; set; }
        public string Requerente { get; set; }
        public int? Despacho_Codigo { get; set; }
        public string Despacho_Nome { get; set; }
        public int? CCusto_Codigo { get; set; }
        [DataType(DataType.MultilineText)]
        public string ObsGeral { get; set; }
        public string ObsInterna { get; set; }
        public string Assunto_Nome { get; set; }
        public string ErrorMessage { get; set; }
        [Required]
        [StringLength(4)]
        public string CaptchaCode { get; set; }
        public List<TramiteStruct > Lista_Tramite { get; set; }
        public string Lista_CC { get; set; }
        public int User_Id { get; set; }

    }
}
