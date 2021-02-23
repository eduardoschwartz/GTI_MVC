using System.ComponentModel.DataAnnotations;

namespace GTI_Models.Models {
    public class Redesim_Registro {
        [Key]
        public string Protocolo { get; set; }
        public string Arquivo { get; set; }
        public string Cnpj { get; set; }
        public string Razao_Social { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string MatrizFilial { get; set; }
        public int Natureza_Juridica { get; set; }
    }

    public class Redesim_RegistroStruct {
        public string Arquivo { get; set; }
        public string Protocolo { get; set; }
        public string Cnpj { get; set; }
        public string[] Evento { get; set; }
        public int[] EventoCodigo { get; set; }
        public string NomeEmpresarial { get; set; }
        public string MatrizFilial { get; set; }
        public string DataAberturaEstabelecimento { get; set; }
        public string DataAberturaEmpresa { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complementos { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public string Municipio { get; set; }
        public string Referencia { get; set; }
        public string EmpresaEstabelecida { get; set; }
        public string NaturezaJuridica { get; set; }
        public int NaturezaJuridicaCodigo { get; set; }
        public string OrgaoRegistro { get; set; }
        public string NumeroOrgaoRegistro { get; set; }
        public string CapitalSocial { get; set; }
        public string CpfResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public string QualificacaoResponsavel { get; set; }
        public string TelefoneResponsavel { get; set; }
        public string EmailResponsavel { get; set; }
        public string PorteEmpresa { get; set; }
        public string CnaePrincipal { get; set; }
        public string[] CnaeSecundaria { get; set; }
        public string AtividadeAuxiliar { get; set; }
        public string TipoUnidade { get; set; }
        public string[] FormaAtuacao { get; set; }
        public string Qsa { get; set; }
        public string CpfRepresentante { get; set; }
        public string NomeRepresentante { get; set; }
        public bool Duplicado { get; set; }
    }

}
