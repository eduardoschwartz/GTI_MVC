﻿using System;
using System.Collections.Generic;
using System.Web;

namespace GTI_Mvc.ViewModels {
    public class RedesimImportViewModel {
        public IEnumerable<HttpPostedFileBase> Files { get; set; }
        public List<RedesimImportFilesViewModel> ListaArquivo { get; set; }
    }


    public class RedesimImportFilesViewModel {
        public string Guid { get; set; }
        public DateTime PeriodoDe { get; set; }
        public DateTime PeriodoAte { get; set; }
        public string Tipo { get; set; }
        public bool Valido { get; set; }
        public string Mensagem { get; set; }
    }

}