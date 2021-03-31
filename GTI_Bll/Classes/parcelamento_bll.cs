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

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(Parcelamento_web_lista_codigo Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Lista_Codigo(Reg);
            return ex;
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Lista_Codigo(guid);
        }

        public Exception Incluir_Parcelamento_Web_Origem(Parcelamento_web_origem Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Origem(Reg);
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

        public List<Parcelamento_Web_Simulado> Lista_Parcelamento_Destino(string Guid, short Plano, DateTime Data_Vencimento, bool Ajuizado, bool Honorario, decimal Principal, decimal Juros, decimal Multa, decimal Correcao, decimal Total, decimal Adicional, decimal Valor_Minimo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Lista_Parcelamento_Destino(Guid, Plano, Data_Vencimento, Ajuizado, Honorario, Principal, Juros, Multa, Correcao, Total, Adicional, Valor_Minimo);
        }

        public Exception Incluir_Parcelamento_Web_Simulado_Resumo(Parcelamento_Web_Simulado_Resumo Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Simulado_Resumo(Reg);
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

        public Exception Incluir_Parcelamento_Web_Destino(Parcelamento_Web_Destino Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Destino(Reg);
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

        public Exception Incluir_OrigemReparc(Origemreparc Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_OrigemReparc(Reg);
            return ex;
        }

        public Exception Incluir_DestinoReparc(Destinoreparc Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_DestinoReparc(Reg);
            return ex;
        }

        public byte Retorna_Seq_Disponivel(int Codigo) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            return obj.Retorna_Seq_Disponivel(Codigo);
        }

        public Exception Incluir_Debito_Parcela(Debitoparcela Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Debito_Parcela(Reg);
            return ex;
        }

        public Exception Incluir_Parcelamento_Web_Tributo(Parcelamento_Web_Tributo Reg) {
            Parcelamento_Data obj = new Parcelamento_Data(_connection);
            Exception ex = obj.Incluir_Parcelamento_Web_Tributo(Reg);
            return ex;
        }


    }
}
