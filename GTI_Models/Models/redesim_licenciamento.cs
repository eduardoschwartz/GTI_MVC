using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {

    public class Redesim_licenciamento {
        [Key]
        [Column(Order=1)]
        public string Protocolo { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime Data_Solicitacao { get; set; }
        public string Arquivo { get; set; }
        public int Situacao_Solicitacao { get; set; }
        public DateTime Data_Emissao { get; set; }
        public DateTime Data_Validade { get; set; }
        public bool Mei { get; set; }
    }

    public class Redesim_licenciamentoStruct {
        public string Protocolo { get; set; }
        public string IdSolicitacao { get; set; }
        public string SituacaoSolicitacao { get; set; }
        public string Orgao { get; set; }
        public string DataSolicitacao { get; set; }
        public string IdLicenca { get; set; }
        public string ProtocoloOrgao { get; set; }
        public string NumeroLicenca { get; set; }
        public string DetalheLicenca { get; set; }
        public string OrgaoLicenca { get; set; }
        public string Risco { get; set; }
        public string SituacaoLicenca { get; set; }
        public string DataEmissao { get; set; }
        public string DataValidade { get; set; }
        public string DataProtocolo { get; set; }
        public string Cnpj { get; set; }
        public string RazaoSocial { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string TipoInscricao { get; set; }
        public string NumeroInscricao { get; set; }
        public string PorteEmpresaMei { get; set; }
        public string EmpresaTeraEstabelecimento { get; set; }
        public string[] Cnae { get; set; }
        public string[] AtividadesAuxiliares { get; set; }
    }
}
