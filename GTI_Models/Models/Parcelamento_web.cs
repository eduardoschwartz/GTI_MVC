using System;
using System.Collections.Generic;
using System.Linq;

namespace GTI_Models.Models {
    public class Parcelamento_web {
    }

    public class Parcelamento_Origem {
        public int Codigo { get; set; }
        public short Exercicio { get; set; }
        public short Lancamento { get; set; }
        public short Sequencia { get; set; }
        public byte Parcela { get; set; }
        public byte Complemento { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public decimal Valor_Juros { get; set; }
        public decimal Valor_Multa { get; set; }
        public decimal Valor_Correcao { get; set; }
        public decimal Valor_Penalidade { get; set; }
        public decimal Valor_Total { get; set; }
        public int Qtde_Penalidade { get; set; }
        public int Qtde_Parcelamento { get; set; }
    }

    public class Parcelamento_Origem_Qtde {
        public string NumProcesso { get; set; }
        public int Codigo { get; set; }
        public short Exercicio { get; set; }
        public short Lancamento { get; set; }
        public byte Sequencia { get; set; }
        public byte Parcela { get; set; }
        public byte Complemento { get; set; }
        public int? Qtde_Parcelamento { get; set; }
    }


}
