using System;
using System.Collections.Generic;
using GTI_Models.Models;
using GTI_Dal.Classes;
using GTI_Bll.Classes;
using static GTI_Models.modelCore;

namespace GTI_Bll.Classes {
    public class Imovel_bll {
        private string _connection;

        public Imovel_bll(string sConnection) {
            _connection = sConnection;
        }

        /// <summary>
        /// Retorna os dados de um imóvel
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public ImovelStruct Dados_Imovel(int nCodigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_Imovel(nCodigo);
        }

        public List<ProprietarioStruct> Lista_Proprietario(int CodigoImovel, bool Principal = false) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Proprietario(CodigoImovel, Principal);
        }

        public List<LogradouroStruct> Lista_Logradouro(String Filter = "") {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Logradouro(Filter);
        }

        /// <summary>
        /// Verifica se existe o imóvel informado
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool Existe_Imovel(int nCodigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Imovel(nCodigo);
        }

        /// <summary>
        /// Verifica e o imóvel existe
        /// </summary>
        /// <param name="distrito"></param>
        /// <param name="setor"></param>
        /// <param name="quadra"></param>
        /// <param name="lote"></param>
        /// <param name="unidade"></param>
        /// <param name="subunidade"></param>
        /// <returns></returns>
        public int Existe_Imovel(int distrito, int setor, int quadra, int lote, int unidade, int subunidade) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Imovel(distrito,setor,quadra,lote,unidade,subunidade);
        }
        
        /// <summary>
        /// Verifica se existe a Face informada
        /// </summary>
        /// <param name="Distrito"></param>
        /// <param name="Setor"></param>
        /// <param name="Quadra"></param>
        /// <param name="Face"></param>
        /// <returns></returns>
        public bool Existe_Face_Quadra(int Distrito, int Setor, int Quadra, int Face) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Face_Quadra(Distrito,Setor,Quadra,Face);
        }

        public EnderecoStruct Dados_Endereco(int Codigo, TipoEndereco Tipo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_Endereco(Codigo,Tipo);
        }

        public List<Categprop> Lista_Categoria_Propriedade() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Categoria_Propriedade();
        }

        public List<Topografia> Lista_Topografia() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Topografia();
        }

        public List<Situacao> Lista_Situacao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Situacao();
        }

        public List<Benfeitoria> Lista_Benfeitoria() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Benfeitoria();
        }

        public List<Pedologia> Lista_Pedologia() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Pedologia();
        }

        public List<Usoterreno> Lista_uso_terreno() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Uso_Terreno();
        }

        public List<Testada> Lista_Testada(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Testada(Codigo);
        }

        public List<AreaStruct> Lista_Area(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Area(Codigo);
        }

        /// <summary>
        /// Lista do histórico do imóvel
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<HistoricoStruct> Lista_Historico(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Historico(Codigo);
        }
        
        public List<Usoconstr> Lista_Uso_Construcao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Uso_Construcao();
        }

        public List<Categconstr> Lista_Categoria_Construcao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Categoria_Construcao();
        }

        public List<Tipoconstr> Lista_Tipo_Construcao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Tipo_Construcao();
        }

        ///<summary> Retorna os dados de um condomínio
        ///</summary>
        public CondominioStruct Dados_Condominio(int nCodigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_Condominio(nCodigo);
        }

        ///<summary> Retorna a lista das áreas de um condomínio
        ///</summary>
        public List<AreaStruct> Lista_Area_Condominio(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Area_Condominio(Codigo);
        }

        /// <summary>
        /// Retorna a inscrição cadastral completa do imóvel
        /// </summary>
        /// <param name="Logradouro"></param>
        /// <param name="Numero"></param>
        /// <returns></returns>
        public ImovelStruct Inscricao_imovel(int Logradouro, short Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Inscricao_imovel(Logradouro,Numero);
        }

        /// <summary>
        /// Lista dos condomínios cadastrados
        /// </summary>
        /// <returns></returns>
        public List<Condominio> Lista_Condominio() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Condominio();
        }

        /// <summary>
        /// Lista das testadas do condomínio
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<Testadacondominio> Lista_Testada_Condominio(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Testada_Condominio(Codigo);
        }
        
        /// <summary>
        /// Lista das unidades do condomínio
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<Condominiounidade> Lista_Unidade_Condominio(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Unidade_Condominio(Codigo);
        }

        /// <summary>
        /// Retorna a Lista dos imóveis filtrados
        /// </summary>
        /// <param name="Reg"></param>
        /// <returns></returns>
        public List<ImovelStruct> Lista_Imovel(ImovelStruct Reg, ImovelStruct OrderByField) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel(Reg,OrderByField);
        }

        /// <summary>
        /// Retorna os dados de IPTU de um imóvel em um ano
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Ano"></param>
        /// <returns></returns>
        public Laseriptu Dados_IPTU(int Codigo, int Ano) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_IPTU(Codigo, Ano);
        }

        /// <summary>
        /// Retorna os dados IPTU de um imóvel em todos os anos
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<Laseriptu> Dados_IPTU(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_IPTU(Codigo);
        }

        /// <summary>
        /// Soma das áreas construidas do imóvel
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public decimal Soma_Area(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Soma_Area(Codigo);
        }

        /// <summary>
        /// Retorna a quantidade de imóveis que um contribuinte possui como proprietário
        /// </summary>
        /// <param name="CodigoImovel"></param>
        /// <returns></returns>
        public int Qtde_Imovel_Cidadao(int CodigoImovel) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Qtde_Imovel_Cidadao(CodigoImovel);
        }

        /// <summary>
        /// Retorna verdadeiro se o imóvel for imune e falso se não for
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public bool Verifica_Imunidade(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Verifica_Imunidade(Codigo);
        }

        /// <summary>
        /// Retorna a lista de isenções de um imóvel, caso o ano for especificado retorna apenas a isenção do ano.
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Ano"></param>
        /// <returns></returns>
        public List<IsencaoStruct> Lista_Imovel_Isencao(int Codigo, int Ano = 0) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel_Isencao(Codigo,Ano);
        }

        /// <summary>
        /// Inativar o imóvel especificado
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public Exception Inativar_imovel(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Inativar_imovel(Codigo);
            return ex;
        }

        /// <summary>
        /// Retorna o código reduzido através da inscrição cadastral ou zero se não existir
        /// </summary>
        /// <param name="distrito"></param>
        /// <param name="setor"></param>
        /// <param name="quadra"></param>
        /// <param name="lote"></param>
        /// <param name="face"></param>
        /// <param name="unidade"></param>
        /// <param name="subunidade"></param>
        /// <returns></returns>
        public int Retorna_Imovel_Inscricao(int distrito, int setor, int quadra, int lote, int face, int unidade, int subunidade) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Imovel_Inscricao(distrito,setor,quadra,lote,face,unidade,subunidade);
        }

        /// <summary>
        /// Retorna a lista de faces de quadra
        /// </summary>
        /// <param name="distrito"></param>
        /// <param name="setor"></param>
        /// <param name="quadra"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public List<FacequadraStruct> Lista_FaceQuadra(int distrito, int setor, int quadra, int face) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_FaceQuadra(distrito, setor, quadra, face);
        }

        /// <summary>
        /// Retorna o próximo código disponivel de imóvel
        /// </summary>
        /// <returns></returns>
        public int Retorna_Codigo_Disponivel() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Codigo_Disponivel();
        }

        /// <summary>
        /// Retorna o próximo código disponível de condomínio
        /// </summary>
        /// <returns></returns>
        public int Retorna_Codigo_Condominio_Disponivel() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Codigo_Disponivel();
        }

        /// <summary>
        /// Incluir um novo imóvel
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Incluir_Imovel(Cadimob reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Imovel(reg);
            return ex;
        }

        /// <summary>
        /// Incluir um novo condomínio
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Incluir_Condominio(Condominio reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Condominio(reg);
            return ex;
        }
        
        /// <summary>
        /// Alterar o imóvel selecionado
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Alterar_Imovel(Cadimob reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Imovel(reg);
            return ex;
        }

        /// <summary>
        /// Alterar o condomínio selecionado
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Alterar_Condominio(Condominio reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Condominio(reg);
            return ex;
        }

        /// <summary>
        /// Grava os proprietários do imóvel
        /// </summary>
        /// <param name="Lista"></param>
        /// <returns></returns>
        public Exception Incluir_Proprietario(List<Proprietario> Lista) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Proprietario(Lista);
            return ex;
        }

        /// <summary>
        /// Grava as testadas do imóvel
        /// </summary>
        /// <param name="testadas"></param>
        /// <returns></returns>
        public Exception Incluir_Testada(List<Testada> testadas) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Testada(testadas);
            return ex;
        }

        /// <summary>
        /// Grava as testadas do condomínio
        /// </summary>
        /// <param name="testadas"></param>
        /// <returns></returns>
        public Exception Incluir_Testada_Condominio(List<Testadacondominio> testadas) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Testada_Condominio(testadas);
            return ex;
        }

        /// <summary>
        /// Grava os históricos do imóvel
        /// </summary>
        /// <param name="historicos"></param>
        /// <returns></returns>
        public Exception Incluir_Historico(List<Historico> historicos) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Historico(historicos);
            return ex;
        }

        /// <summary>
        /// Grava as áreas do imóvel
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public Exception Incluir_Area(List<Areas> areas) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Area(areas);
            return ex;
        }

        /// <summary>
        /// Grava as áreas do condomínio
        /// </summary>
        /// <param name="areas"></param>
        /// <returns></returns>
        public Exception Incluir_Area_Condominio(List<Condominioarea> areas) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Area_Condominio(areas);
            return ex;
        }

        /// <summary>
        /// Grava as unidades do condomínio
        /// </summary>
        /// <param name="unidades"></param>
        /// <returns></returns>
        public Exception Incluir_Unidade_Condominio(List<Condominiounidade> unidades) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Unidade_Condominio(unidades);
            return ex;
        }

        /// <summary>
        /// Lista os códigos dos imóveis ativos
        /// </summary>
        /// <returns></returns>
        public List<int> Lista_Imovel_Ativo() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel_Ativo();
        }

        /// <summary>
        /// Retorna os códigos dos imóveis que receberam o comunicado de isenção
        /// </summary>
        /// <returns></returns>
        public List<int> Lista_Comunicado_Isencao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Comunicado_Isencao();
        }

        /// <summary>
        /// Grava na tabela Comunicado_Isencao
        /// </summary>
        /// <param name="Reg"></param>
        /// <returns></returns>
        public Exception Insert_Comunicado_Isencao(Comunicado_isencao Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Insert_Comunicado_Isencao(Reg);
            return ex;
        }

        /// <summary>
        /// Retorna a lista das fotos de um imóvel
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<Foto_imovel> Lista_Foto_Imovel(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Foto_Imovel(Codigo);
        }

        public List<Uf> Lista_UF() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_UF();
        }

        public Exception Insert_Dados_Imovel(dados_imovel_web Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Insert_Dados_Imovel(Reg);
            return ex;
        }

        public bool Existe_Imovel_Cpf(int Codigo, string Cpf) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Imovel_Cpf(Codigo,Cpf);
        }

        public bool Existe_Imovel_Cnpj(int Codigo, string Cnpj) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Imovel_Cnpj(Codigo, Cnpj);
        }

        public Testada Retorna_Testada_principal(int Codigo, int Face) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Testada_principal(Codigo, Face);
        }

        public Exception Incluir_notificacao_terreno(Notificacao_terreno Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_notificacao_terreno(Reg);
            return ex;
        }

        public bool Existe_Notificacao_Terreno(int Ano, int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Notificacao_Terreno(Ano,Numero);
        }

        public List<Notificacao_Terreno_Struct> Lista_Notificacao_Terreno(int Ano) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Notificacao_Terreno(Ano);
        }

        public Notificacao_Terreno_Struct Retorna_Notificacao_Terreno(int Ano, int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Notificacao_Terreno(Ano,Numero);
        }

        public List<ImovelStruct> Lista_Imovel_Proprietario(string PartialName) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel_Proprietario(PartialName);
        }

        public List<ImovelStruct> Lista_Imovel_Endereco(string PartialName,int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel_Endereco(PartialName,Numero);
        }

        public List<ImovelStruct> Lista_Imovel(int Codigo, int Distrito, int Setor, int Quadra, int Lote, int Face, int Unidade, int SubUnidade, string PartialName, string PartialEndereco, int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Imovel(Codigo, Distrito,  Setor,  Quadra,  Lote,  Face,  Unidade,  SubUnidade, PartialName,  PartialEndereco,  Numero);
        }

        public Exception Incluir_auto_infracao(Auto_infracao Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_auto_infracao(Reg);
            return ex;
        }

        public bool Existe_Auto_Infracao(int Ano, int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Auto_Infracao(Ano, Numero);
        }

        public List<Auto_Infracao_Struct> Lista_Auto_Infracao(int Ano) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Auto_Infracao(Ano);
        }

        public Auto_Infracao_Struct Retorna_Auto_Infracao(int Ano, int Numero) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Auto_Infracao(Ano,Numero);
        }

        #region ITBI
        public List<Itbi_natureza> Lista_Itbi_Natureza() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Itbi_Natureza();
        }

        public List<Itbi_financiamento> Lista_Itbi_Financiamento() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_Itbi_Financiamento();
        }

        public bool Existe_Itbi(string guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Existe_Itbi(guid);
        }

        public Exception Incluir_Itbi_main(Itbi_main Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_main(Reg);
            return ex;
        }

        public Exception Alterar_Itbi_Main(Itbi_main Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Main(Reg);
            return ex;
        }

        public Itbi_main Retorna_Itbi_Main(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Main(Guid);
        }

        public List<Itbi_comprador> Retorna_Itbi_Comprador(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Comprador(Guid);
        }

        public Exception Excluir_Itbi(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi(Guid);
            return ex;
        }

        public Exception Incluir_Itbi_comprador(List<Itbi_comprador> Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_comprador(Reg);
            return ex;
        }

        public Exception Excluir_Itbi_comprador(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_comprador(Guid);
            return ex;
        }

        public Exception Excluir_Itbi_comprador(string Guid, int seq) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_comprador(Guid,seq);
            return ex;
        }

        public Exception Excluir_Itbi_vendedor(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_vendedor(Guid);
            return ex;
        }

        public Exception Excluir_Itbi_vendedor(string Guid, int seq) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_vendedor(Guid, seq);
            return ex;
        }

        public Exception Excluir_Itbi_anexo(string guid, int seq) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_anexo(guid, seq);
            return ex;
        }

        public Exception Incluir_Itbi_vendedor(List<Itbi_vendedor> Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_vendedor(Reg);
            return ex;
        }

        public List<Itbi_vendedor> Retorna_Itbi_vendedor(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Vendedor(Guid);
        }

        public List<Itbi_anexo> Retorna_Itbi_Anexo(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Anexo(Guid);
        }

        public byte Retorna_Itbi_Anexo_Disponivel(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Anexo_Disponivel(Guid);
        }

        public Exception Incluir_Itbi_Anexo(Itbi_anexo item) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_Anexo(item);
            return ex;      
        }

        public int Retorna_Itbi_Disponivel() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Disponivel();
        }

        public ItbiAnoNumero Alterar_Itbi_Main(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Alterar_Itbi_Main(Guid);
        }

        public List<Itbi_Lista> Retorna_Itbi_Query(int user,bool f, int status,int ano) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Query(user,f,status,ano);
        }

        public string Retorna_Itbi_Natureza_nome(int codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Natureza_nome(codigo);
        }

        public string Retorna_Itbi_Financimento_nome(int codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Financimento_nome(codigo);
        }

        public string Retorna_Itbi_Situacao(int codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Situacao(codigo);
        }

        public Exception Alterar_Itbi_Situacao(string p, int s) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Alterar_Itbi_Situacao(p,s);
        }

        public Itbi_status Retorna_Itbi_Situacao(string guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Situacao(guid);
        }

        public Exception Incluir_Itbi_Forum(Itbi_forum item) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_Forum(item);
            return ex;
        }

        public Exception Alterar_Itbi_Forum(string p, short s, string msg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Forum(p,s,msg);
            return ex;
        }

        public Exception Excluir_Itbi_Forum(string p, short s) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_Forum(p, s);
            return ex;
        }

        public List<Itbi_forum> Retorna_Itbi_Forum(string p) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Forum(p);
        }

        public Exception Incluir_Itbi_Guia(Itbi_Guia Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_Guia(Reg);
            return ex;
        }

        public Exception Alterar_Itbi_Guia(string p, int n, DateTime d,int f) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Guia(p,n,d,f);
            return ex;
        }

        public Exception Incluir_isencao_main(Itbi_isencao_main Reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_isencao_main(Reg);
            return ex;
        }

        public Exception Incluir_Itbi_isencao_imovel(List<Itbi_isencao_imovel> Lista) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Itbi_isencao_imovel(Lista);
            return ex;
        }

        public Itbi_isencao_main_Struct Retorna_Itbi_Isencao_Main(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Isencao_Main(Guid);
        }

        public List<Itbi_isencao_imovel> Retorna_Itbi_Isencao_Imovel(string Guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Isencao_Imovel(Guid);
        }

        public Exception Excluir_Itbi_Isencao_Imovel(string guid, int seq) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_Isencao_Imovel(guid,seq);
            return ex;
        }

        public Exception Excluir_Itbi_Guia(string guid) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Excluir_Itbi_Guia(guid);
            return ex;
        }

        public List<Itbi_natureza_isencao> Lista_itbi_natureza_isencao() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Lista_itbi_natureza_isencao();
        }

        public Exception Alterar_Itbi_Isencao(Itbi_isencao_main reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Isencao(reg);
            return ex;
        }

        public List<Itbi_Lista> Retorna_Itbi_Isencao_Query(int user, bool f, int status) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Isencao_Query(user,f,status);
        }

        public int Retorna_Itbi_Isencao_Disponivel() {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Itbi_Isencao_Disponivel();
        }

        public Exception Liberar_Itbi_Isencao(string p,int FiscalId) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Liberar_Itbi_Isencao(p,FiscalId);
            return ex;
        }

        public Exception Alterar_Itbi_Isencao_Situacao(string p, int s) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Isencao_Situacao(p, s);
            return ex;
        }

        public Exception Alterar_Itbi_Isencao_QRCode(string p, byte[] code) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Isencao_QRCode(p, code);
            return ex;
        }

        public Exception Incluir_Historico(Historico reg) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Incluir_Historico(reg);
            return ex;

        }

        public string Retorna_Imovel_Inscricao(int Codigo) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Retorna_Imovel_Inscricao(Codigo);
        }

        public Laseriptu_ext Dados_IPTU_Ext(int Codigo, int Ano) {
            Imovel_Data obj = new Imovel_Data(_connection);
            return obj.Dados_IPTU_Ext(Codigo,Ano);
        }

        public Exception Alterar_Itbi_Isencao_Natureza(string p, int n) {
            Imovel_Data obj = new Imovel_Data(_connection);
            Exception ex = obj.Alterar_Itbi_Isencao_Natureza(p,n);
            return ex;
        }


        #endregion

    }//end class
    }
