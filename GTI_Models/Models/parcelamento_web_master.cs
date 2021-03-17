using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Parcelamento_web_master {
        [Key]
        public string Guid { get; set; }
        public int Codigo { get; set; }
    }
}
