using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class ImovelDetailsViewModel {
        public ImovelStruct ImovelStruct { get; set; }
        public List<ProprietarioStruct> Lista_Proprietario { get; set; }
        public EnderecoStruct Endereco_Entrega { get; set; }
        public List<AreaStruct> Lista_Areas { get; set; }
        public List<Testada> Lista_Testada { get; set; }
        [Display(Name = "Inscrição Municipal")]
        public string Inscricao { get; set; }
        public string CpfCnpjLabel { get; set; }
        public string CpfValue { get; set; }
        public string CnpjValue { get; set; }
        public string ErrorMessage { get; set; }
        public string CaptchaCode { get; set; }
    }
}