using GTI_Mvc.Models;
using System;
using System.Collections.Generic;

namespace GTI_Mvc.Interfaces {
    public interface IProtocoloRepository {
        Exception Alterar_Obs(int Ano, int Numero, int Seq, string ObsGeral, string ObsInterna);
        ProcessoStruct Dados_Processo(int nAno, int nNumero);
        List<Tramitacao> DadosTramite(int Ano, int Numero);
        List<TramiteStruct> DadosTramite(short Ano, int Numero, int CodAssunto);
        int DvProcesso(int Numero);
        Exception Enviar_Processo(Tramitacao Reg);
     //   Exception Inserir_Local(int Numero, int Ano, int Seq, int CCusto_Codigo);
        List<Centrocusto> Lista_CentroCusto();
        List<UsuariocentroCusto> ListaCentroCustoUsuario(int idLogin);
        List<Despacho> Lista_Despacho();
        List<ProcessoAnexoStruct> ListProcessoAnexo(int nAno, int nNumero);
        List<ProcessoDocStruct> ListProcessoDoc(int nAno, int nNumero);
        List<ProcessoEndStruct> ListProcessoEnd(int nAno, int nNumero);
        List<Anexo_logStruct> ListProcessoAnexoLog(int nAno, int nNumero);
     //   Exception Move_Sequencia_Tramite_Abaixo(int Numero, int Ano, int Seq);
     //   Exception Move_Sequencia_Tramite_Acima(int Numero, int Ano, int Seq);
        Exception Receber_Processo(Tramitacao Reg);
     //   Exception Remover_Local(int Numero, int Ano, int Seq, int CCusto_Codigo);
        string Retorna_Assunto(int Codigo);
        bool Tramite_Enviado(int Ano, int Numero, int Seq);
        bool Tramite_Recebido(int Ano, int Numero, int Seq);

    }
}
