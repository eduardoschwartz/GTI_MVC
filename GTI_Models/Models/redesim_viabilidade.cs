using System;
using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_Viabilidade {
        [Key]
        public string Protocolo { get; set; }
        public string Arquivo { get; set; }
        public string Cnpj { get; set; }
        public string Razao_Social { get; set; }
        public string Analise { get; set; }
        public string Nire { get; set; }
        public bool EmpresaEstabelecida { get; set; }
        public DateTime DataProtocolo { get; set; }
        public string Cep { get; set; }
        public int NumeroInscricaoImovel { get; set; }
        public string Complemento { get; set; }
        public int Numero { get; set; }
        public string TipoUnidade { get; set; }
        public decimal AreaImovel { get; set; }
        public decimal AreaEstabelecimento { get; set; }
    }

    public class Redesim_ViabilidadeStuct {
        public string Arquivo { get; set; }
        public string Protocolo { get; set; }
        public string Analise { get; set; }
        public string Nire { get; set; }
        public string Cnpj { get; set; }
        public string EmpresaEstabelecida { get; set; }
        public string[] Cnae { get; set; }
        public string AtividadeAuxiliar { get; set; }
        public string DataProtocolo { get; set; }
        public string DataResultadoAnalise { get; set; }
        public string DataResultadoViabilidade { get; set; }
        public string TempoAndamento { get; set; }
        public string[] cdEvento { get; set; }
        public string[] Evento { get; set; }
        public string Cep { get; set; }
        public string TipoInscricaoImovel { get; set; }
        public string NumeroInscricaoImovel { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string TipoUnidade { get; set; }
        public string FormaAtuacao { get; set; }
        public string Municipio { get; set; }
        public string RazaoSocial { get; set; }
        public string Orgao { get; set; }
        public string AreaImovel { get; set; }
        public string AreaEstabelecimento { get; set; }
        public bool Duplicado { get; set; }
    }

    public class Redesim_evento {
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }

}
