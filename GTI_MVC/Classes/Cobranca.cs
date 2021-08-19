namespace GTI_Mvc.Classes {
    public class Cobranca {
        public int numeroConvenio { get; set; }
        public int numeroCarteira { get; set; }
        public int numeroVariacaoCarteira { get; set; }
        public int codigoModalidade { get; set; }
        public string dataEmissao { get; set; }
        public string dataVencimento { get; set; }
        public double valorOriginal { get; set; }
        public double valorAbatimento { get; set; }
        public string codigoAceite { get; set; }
        public int codigoTipoTitulo { get; set; }
        public string indicadorPermissaoRecebimentoParcial { get; set; }
        public string numeroTituloBeneficiario { get; set; }
        public string campoUtilizacaoBeneficiario { get; set; }
        public string numeroTituloCliente { get; set; }
        public string mensagemBloquetoOcorrencia { get; set; }
        public CobrancaPagador pagador { get; set; }
        public string indicadorPix { get; set; }
    }

    public class CobrancaPagador {
        public int tipoInscricao { get; set; }
        public long numeroInscricao { get; set; }
        public string nome { get; set; }
        public string endereco { get; set; }
        public int cep { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string uf { get; set; }
        public string telefone { get; set; }
    }

}