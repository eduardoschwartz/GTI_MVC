using GTI_Models.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GTI_Dal {
    public class GTI_Context :DbContext{
        public GTI_Context(string Connection_Name) : base(Connection_Name) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer<GTI_Context>(null);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(14, 4));

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Uf> Uf { get; set; }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Bairro> Bairro { get; set; }
        public DbSet<Cidadao> Cidadao { get; set; }
        public DbSet<Profissao> Profissao { get; set; }
        public DbSet<Pais> Pais { get; set; }
        public DbSet<Logradouro> Logradouro { get; set; }
        public DbSet<Cep> Cep { get; set; }
        public DbSet<Cepdb> CepDB { get; set; }
        public DbSet<Processogti> Processogti { get; set; }
        public DbSet<Anexo> Anexo { get; set; }
        public DbSet<Anexo_log> Anexo_log { get; set; }
        public DbSet<Processo_historico> Processo_historico { get; set; }
        public DbSet<Processoend> Processoend { get; set; }
        public DbSet<Processodoc> Processodoc { get; set; }
        public DbSet<Processocidadao> Processocidadao { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Despacho> Despacho { get; set; }
        public DbSet<Assunto> Assunto { get; set; }
        public DbSet<Centrocusto> Centrocusto { get; set; }
        public DbSet<Assuntocc> Assuntocc { get; set; }
        public DbSet<Assuntodoc> Assuntodoc { get; set; }
        public DbSet<Benfeitoria> Benfeitoria { get; set; }
        public DbSet<Categprop> Categprop { get; set; }
        public DbSet<Categconstr> Categconstr { get; set; }
        public DbSet<Pedologia> Pedologia { get; set; }
        public DbSet<Situacao> Situacao { get; set; }
        public DbSet<Topografia> Topografia { get; set; }
        public DbSet<Tipoconstr> Tipoconstr { get; set; }
        public DbSet<Usoterreno> Usoterreno { get; set; }
        public DbSet<Usoconstr> Usoconstr { get; set; }
        public DbSet<Mobiliario> Mobiliario { get; set; }
        public DbSet<Mobiliarioevento> Mobiliarioevento { get; set; }
        public DbSet<Horariofunc> Horariofunc { get; set; }
        public DbSet<Mobiliarioatividadevs2> Mobiliarioatividadevs2 { get; set; }
        public DbSet<Mobiliarioatividadeiss> Mobiliarioatividadeiss { get; set; }
        public DbSet<Mobiliarioproprietario> Mobiliarioproprietario { get; set; }
        public DbSet<Mei> Mei { get; set; }
        public DbSet<Usuariocc> Usuariocc { get; set; }
        public DbSet<Usuariofunc> Usuariofunc { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Tipousuario> Tipousuario { get; set; }
        public DbSet<Tramitacao> Tramitacao { get; set; }
        public DbSet<Tramitacaocc> Tramitacaocc { get; set; }
        public DbSet<Cadimob> Cadimob { get; set; }
        public DbSet<Proprietario> Proprietario { get; set; }
        public DbSet<Condominio> Condominio { get; set; }
        public DbSet<Condominioarea> CondominioArea { get; set; }
        public DbSet<Condominiounidade> CondominioUnidade { get; set; }
        public DbSet<Testadacondominio> Testadacondominio { get; set; }
        public DbSet<Facequadra> Facequadra { get; set; }
        public DbSet<Endentrega> Endentrega { get; set; }
        public DbSet<Testada> Testada { get; set; }
        public DbSet<Areas> Areas { get; set; }
        public DbSet<Lancamento> Lancamento { get; set; }
        public DbSet<Tipolivro> Tipolivro { get; set; }
        public DbSet<Debitoparcela> Debitoparcela { get; set; }
        public DbSet<Tributo> Tributo { get; set; }
        public DbSet<Tributolancamento> Tributolancamento { get; set; }
        public DbSet<Tributoaliquota> Tributoaliquota { get; set; }
        public DbSet<SpExtrato> SpExtrato { get; set; }
        public DbSet<SpExtrato_carta> SpExtrato_carta { get; set; }
        public DbSet<Situacaolancamento> Situacaolancamento { get; set; }
        public DbSet<Debitotributo> Debitotributo { get; set; }
        public DbSet<Obsparcela> Obsparcela { get; set; }
        public DbSet<Debitoobservacao> Debitoobservacao { get; set; }
        public DbSet<Numdocumento> Numdocumento { get; set; }
        public DbSet<Parceladocumento> Parceladocumento { get; set; }
        public DbSet<security_event> Security_event { get; set; }
        public DbSet<mobiliarioplaca> Mobiliarioplaca { get; set; }
        public DbSet<sil> Sil { get; set; }
        public DbSet<mobiliarioendentrega> Mobiliarioendentrega { get; set; }
        public DbSet<Escritoriocontabil> Escritoriocontabil { get; set; }
        public DbSet<DEmpresa> DEmpresa { get; set; }
        public DbSet<Mobiliariocnae> Mobiliariocnae { get; set; }
        public DbSet<cnaesubclasse> Cnaesubclasse { get; set; }
        public DbSet<Boletoguia> Boletoguia { get; set; }
        public DbSet<Boleto> Boleto { get; set; }
        public DbSet<Segunda_via_web> Segunda_via_web { get; set; }
        public DbSet<Laseriptu> Laser_iptu { get; set; }
        public DbSet<Laseriptu_ext> Laser_iptu_ext { get; set; }
        public DbSet<comercio_eletronico> Comercio_eletronico { get; set; }
        public DbSet<Vre_empresa> Vre_empresa { get; set; }
        public DbSet<Vre_atividade> Vre_atividade { get; set; }
        public DbSet<Vre_socio> Vre_socio { get; set; }
        public DbSet<Vre_licenciamento> Vre_licencimento { get; set; }
        public DbSet<Certidao_endereco> Certidao_endereco { get; set; }
        public DbSet<Certidao_valor_venal> Certidao_valor_venal { get; set; }
        public DbSet<Certidao_isencao> Certidao_isencao { get; set; }
        public DbSet<Certidao_debito> Certidao_debito { get; set; }
        public DbSet<Certidao_debito_doc> Certidao_debito_doc { get; set; }
        public DbSet<Certidao_inscricao> Certidao_inscricao { get; set; }
        public DbSet<Parametros> Parametros { get; set; }
        public DbSet<Vwproprietarioduplicado> Vwproprietarioduplicado { get; set; }
        public DbSet<Isencao> Isencao { get; set; }
        public DbSet<Banco> Banco { get; set; }
        public DbSet<SpCalculo> SpCalculo { get; set; }
        public DbSet<Debitopago> Debitopago { get; set; }
        public DbSet<Certidao_inscricao_extrato> Certidao_inscricao_extrato { get; set; }
        public DbSet<Comprovante_pagamento> Comprovante_pagamento { get; set; }
        public DbSet<Historico> Historico { get; set; }
        public DbSet<Carta_cobranca> Carta_cobranca { get; set; }
        public DbSet<Carta_cobranca_exclusao> Carta_cobranca_exclusao { get; set; }
        public DbSet<Carta_cobranca_detalhe> Carta_cobranca_detalhe { get; set; }
        public DbSet<Atividade> Atividade { get; set; }
        public DbSet<Cnaecriterio> Cnaecriterio { get; set; }
        public DbSet<Cnaecriteriodesc> Cnaecriteriodesc { get; set; }
        public DbSet<Paramparcela> Paramparcela { get; set; }
        public DbSet<Calculo_resumo> Calculo_iss_vs { get; set; }
        public DbSet<Alvara_funcionamento> Alvara_funcionamento { get; set; }
        public DbSet<Comunicado_isencao> Comunicado_Isencao { get; set; }
        public DbSet<Atividadeiss> Atividadeiss { get; set; }
        public DbSet<Mobiliariohist> Mobiliariohist { get; set; }
        public DbSet<Mobiliariovs> Mobiliariovs { get; set; }
        public DbSet<Cnae> Cnae { get; set; }
        public DbSet<Cnae_criterio> Cnae_criterio { get; set; }
        public DbSet<Tabelaiss> Tabelaiss { get; set; }
        public DbSet<Serasa> Serasa { get; set; }
        public DbSet<Processoreparc> Processoreparc { get; set; }
        public DbSet<Ficha_compensacao_documento> Ficha_compensacao_documento { get; set; }
        public DbSet<historicocidadao> Historicocidadao { get; set; }
        public DbSet<obscidadao> Obscidadao { get; set; }
        public DbSet<Horario_funcionamento> Horario_funcionamento { get; set; }
        public DbSet<Foto_imovel> Foto_imovel { get; set; }
        public DbSet<Periodomei> Periodomei { get; set; }
        public DbSet<Origemreparc> Origemreparc { get; set; }
        public DbSet<Destinoreparc> Destinoreparc { get; set; }
        public DbSet<Debitocancel> Debitocancel { get; set; }
        public DbSet<Logevento> Logevento { get; set; }
        public DbSet<Livro> Livro { get; set; }
        public DbSet<dados_imovel_web> Dados_Imovel { get; set; }
        public DbSet<Encargo_cvd> Encargo_Cvd { get; set; }
        public DbSet<Certidao_impressao> Certidao_impressao { get; set; }
        public DbSet<Usuario_web> Usuario_Web { get; set; }
        public DbSet<Logradouro_bairro> Logradouro_Bairro { get; set; }
        public DbSet<Itbi_natureza> Itbi_Natureza { get; set; }
        public DbSet<Itbi_financiamento> itbi_Financiamento { get; set; }
        public DbSet<Itbi_main> Itbi_Main { get; set; }
        public DbSet<Itbi_comprador> Itbi_Comprador { get; set; }
        public DbSet<Itbi_vendedor> Itbi_Vendedor { get; set; }
        public DbSet<Itbi_anexo> Itbi_Anexo { get; set; }
        public DbSet<Itbi_status> Itbi_Status { get; set; }
        public DbSet<Itbi_forum> Itbi_Forum { get; set; }
        public DbSet<Itbi_Guia> Itbi_Guia { get; set; }
        public DbSet <Itbi_isencao_main>Itbi_Isencao_Main { get; set; }
        public DbSet<Itbi_isencao_imovel> Itbi_Isencao_Imovel { get; set; }
        public DbSet<Plano> Plano { get; set; }
        public DbSet<Assinatura> Assinatura { get; set; }
        public DbSet<Itbi_natureza_isencao> Itbi_Natureza_Isencao { get; set; }
        public DbSet<Notificacao_iss_web> Notificacao_Iss_Web { get; set; }
        public DbSet<Notificacao_Iss_Tabela> Notificacao_Iss_Tabela { get; set; }
        public DbSet<Rodo_empresa> Rodo_Empresa { get; set; }
        public DbSet<Rodo_uso_plataforma> Rodo_Uso_Palataforma { get; set; }
        public DbSet<Rodo_uso_plataforma_user> Rodo_Uso_Palataforma_User { get; set; }
        public DbSet<Notificacao_terreno> Notificacao_Terreno { get; set; }
        public DbSet<Notificacao_Obra> Notificacao_Obra { get; set; }
        public DbSet<Auto_infracao> Auto_Infracao { get; set; }
        public DbSet<Redesim_arquivo> Redesim_Arquivo { get; set; }
        public DbSet<Redesim_Registro> Redesim_Registro { get; set; }
        public DbSet<Redesim_Viabilidade> Redesim_Viabilidade { get; set; }
        public DbSet<Redesim_natureza_juridica> Redesim_Natureza_Juridica { get; set; }
        public DbSet<Redesim_evento> Redesim_Evento { get; set; }
        public DbSet<Redesim_registro_evento> Redesim_Registro_Evento { get; set; }
        public DbSet<Redesim_porte_empresa> Redesim_Porte_Empresa { get; set; }
        public DbSet<Redesim_registro_forma_atuacao> Redesim_Registro_Forma_Atuacao { get; set; }
        public DbSet<Redesim_forma_atuacao> Redesim_Forma_Atuacao { get; set; }
        public DbSet<Redesim_cnae> Redesim_Cnae { get; set; }
        public DbSet<Redesim_viabilidade_analise> Redesim_Viabilidade_Analise { get; set; }
        public DbSet<Redesim_licenciamento> Redesim_Licenciamento { get; set; }
        public DbSet<Redesim_master> Redesim_Master { get; set; }
        public DbSet<Calendar_event> Calendar_Event { get; set; }
        public DbSet<SpParcelamentoOrigem> spParcelamentoOrigem { get; set; }
        public DbSet<SpParcelamentoDestinoWeb> spParcelamentoDestinoWeb { get; set; }
        public DbSet<SpExtrato_Parcelamento>  spExtrato_Parcelamento { get; set; }
        public DbSet<Parcelamento_web_master> Parcelamento_Web_Master { get; set; }
        public DbSet<Parcelamento_web_lista_codigo> Parcelamento_Web_Lista_Codigo { get; set; }
        public DbSet<Parcelamento_web_origem> Parcelamento_Web_Origem { get; set; }
        public DbSet<Parcelamento_web_selected> Parcelamento_Web_Selected { get; set; }
        public DbSet<Parcelamento_valor_minimo> Parcelamento_Valor_Minimo { get; set; }
        public DbSet<Parcelamento_Web_Simulado> Parcelamento_Web_Simulado { get; set; }
        public DbSet<Parcelamento_Web_Simulado_Resumo> Parcelamento_Web_Simulado_Resumo { get; set; }
        public DbSet<Parcelamento_Web_Destino> Parcelamento_Web_Destino { get; set; }
        public DbSet<Parcelamento_Web_Tributo> Parcelamento_Web_Tributo { get; set; }
        public DbSet<Parcelamento_Web_Selected_Name> Parcelamento_Web_Selected_Name { get; set; }
        public DbSet<Cidadao_socio> Cidadao_Socio { get; set; }
        public DbSet<Mobreq_evento> Mobreq_Evento { get; set; }
        public DbSet<Mobreq_main> Mobreq_Main { get; set; }
        public DbSet<Mobreq_Situacao> Mobreq_Situacao { get; set; }
        public DbSet<Auto_Infracao_Queimada> Auto_Infracao_Queimada { get; set; }
        public DbSet<LogWeb> LogWeb { get; set; }
        public DbSet<LogWeb_Evento> LogWeb_Evento { get; set; }
        public DbSet<Auto_infracao_obra> Auto_Infracao_Obra { get; set; }
        public DbSet<Processo_web> Processo_Web { get; set; }
        public DbSet<SpLista_Terreno_Cip> SpLista_Terreno_Cip { get; set; }
    }
}
