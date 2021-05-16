using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Parcelamento_bll {

        private readonly string _connection;
        public Parcelamento_bll(string sConnection) {
            _connection = sConnection;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(int Codigo, char Tipo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Origem(Codigo, Tipo);
        }

        public bool Existe_Parcelamento_Web_Master(string _guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Existe_Parcelamento_Web_Master(_guid);
        }

        public Exception Incluir_Parcelamento_Web_Master(Parcelamento_web_master Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Master(Reg);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(List<Parcelamento_web_lista_codigo> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Lista_Codigo(Lista);
            return ex;
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Lista_Codigo(guid);
        }

        public Exception Incluir_Parcelamento_Web_Origem(List<Parcelamento_web_origem> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Origem(Lista);
            return ex;
        }

        public Parcelamento_web_master Retorna_Parcelamento_Web_Master(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Parcelamento_Web_Master(guid);
        }

        public Exception Atualizar_Codigo_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Codigo_Master(reg);
            return ex;
        }

        public Exception Atualizar_Totais_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Totais_Master(reg);
            return ex;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Origem(guid);
        }

        public Exception Incluir_Parcelamento_Web_Selected(Parcelamento_web_selected Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Selected(Reg);
            return ex;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Selected(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Selected(guid);
        }

        public Plano Retorna_Plano_Desconto(short Plano) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Plano_Desconto(Plano);
        }

        public decimal Retorna_Parcelamento_Valor_Minimo(short Ano, bool Distrito, string Tipo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Parcelamento_Valor_Minimo(Ano, Distrito, Tipo);
        }

        public Exception Excluir_parcelamento_Web_Selected(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Selected(Guid);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Simulado(Parcelamento_Web_Simulado Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Simulado(Reg);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Simulado(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Simulado(Guid);
            return ex;
        }

        public List<Parcelamento_Web_Simulado> Retorna_Parcelamento_Web_Simulado(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Parcelamento_Web_Simulado(guid);
        }

        public List<Parcelamento_Web_Simulado> Retorna_Parcelamento_Web_Simulado(string guid, int qtde_parcela) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Parcelamento_Web_Simulado(guid, qtde_parcela);
        }

        public List<Parcelamento_Web_Simulado> Lista_Parcelamento_Destino(string Guid, short Plano, DateTime Data_Vencimento, bool Ajuizado, bool Honorario, decimal Principal, decimal Juros, decimal Multa, decimal Correcao, decimal Total, decimal Adicional, decimal Valor_Minimo,decimal Soma_Honorario) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Destino(Guid, Plano, Data_Vencimento, Ajuizado, Honorario, Principal, Juros, Multa, Correcao, Total, Adicional, Valor_Minimo,Soma_Honorario);
        }

        public Exception Incluir_Parcelamento_Web_Simulado_Resumo(List<Parcelamento_Web_Simulado_Resumo> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Simulado_Resumo(Lista);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Simulado_Resumo(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Simulado_Resumo(Guid);
            return ex;
        }

        public Exception Atualizar_QtdeParcela_Master(string Guid, int Qtde) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_QtdeParcela_Master(Guid, Qtde);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Origem(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Origem(Guid);
            return ex;
        }

        public Exception Atualizar_Simulado_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Simulado_Master(reg);
            return ex;
        }

        public Exception Atualizar_Processo_Master(string Guid, short Ano, int Numero) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Processo_Master(Guid, Ano, Numero);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Destino(List<Parcelamento_Web_Destino> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Destino(Lista);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Destino(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_Parcelamento_Web_Destino(Guid);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Lista_Codigo(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Lista_Codigo(Guid);
            return ex;
        }

        public Exception Incluir_ProcessoReparc(Processoreparc Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_ProcessoReparc(Reg);
            return ex;
        }

        public List<Parcelamento_Web_Destino> Lista_Parcelamento_Web_Destino(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Web_Destino(guid);
        }

        public Exception Incluir_OrigemReparc(List<Origemreparc> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_OrigemReparc(Lista);
            return ex;
        }

        public Exception Incluir_DestinoReparc(List<Destinoreparc> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_DestinoReparc(Lista);
            return ex;
        }

        public byte Retorna_Seq_Disponivel(int Codigo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Seq_Disponivel(Codigo);
        }

        public Exception Incluir_Debito_Parcela(List<Debitoparcela> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Debito_Parcela(Lista);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Tributo(Parcelamento_Web_Tributo Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Tributo(Reg);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Tributo(List<Parcelamento_Web_Tributo> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Tributo(Lista);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Selected(List<Parcelamento_web_selected> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Selected(Lista);
            return ex;
        }

        public List<Parcelamento_Web_Tributo> Lista_Parcelamento_Tributo(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Tributo(guid);
        }

        public Exception Incluir_Debito_Tributo(List<Debitotributo> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Debito_Tributo(Lista);
            return ex;
        }

        public Exception Atualizar_Status_Origem(int Codigo, List<SpParcelamentoOrigem> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Status_Origem(Codigo,Lista);
            return ex;
        }

        public List<Parc_Processos> Lista_Parcelamento_Processos(int _userId) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Processos(_userId);
        }

        public List<Destinoreparc> Lista_Destino_Parcelamento(short AnoProcesso, int NumProcesso) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Destino_Parcelamento(AnoProcesso,NumProcesso);
        }

        public List<Debitotributo> Lista_Debito_Tributo(int Codigo, short Ano, short Lanc, short Seq, short Parcela, short Compl) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Debito_Tributo(Codigo, Ano, Lanc, Seq, Parcela, Compl);
        }

        public Exception Atualizar_Requerente_Master(Parcelamento_web_master reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Atualizar_Requerente_Master(reg);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Selected_Name(List<Parcelamento_Web_Selected_Name> Lista) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Selected_Name(Lista);
            return ex;
        }

        public Exception Excluir_parcelamento_Web_Selected_Name(string Guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Excluir_parcelamento_Web_Selected_Name(Guid);
            return ex;
        }

        public List<Parcelamento_Web_Selected_Name_Struct> Lista_Parcelamento_Web_Selected_Name(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Web_Selected_Name(guid);
        }

        public List<Parcelamento_web_master> Lista_Parcelamento_Web_Master(string _dataInicio, string _dataFim) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Web_Master(_dataInicio,_dataFim);
        }

        public List<DebitoStructure> Lista_Parcelas_Parcelamento_Ano_Web(int nCodigo,int nAno,int nSeq) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            List<DebitoStructure> Lista = obj.Lista_Parcelas_Parcelamento_Ano_Web(nCodigo,nAno,nSeq);
            return Lista;
        }


    }
}
