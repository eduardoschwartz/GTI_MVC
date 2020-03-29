using GTI_Mvc.Interfaces;
using GTI_Mvc.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Configuration;

namespace GTI_Mvc.Repository {
    public class ProtocoloRepository:IProtocoloRepository {
        private readonly AppDbContext context;

        public ProtocoloRepository(AppDbContext context) {
            this.context = context;
        }

        public ProcessoStruct Dados_Processo(int nAno, int nNumero) {
            RequerenteRepository requerenteRepository = new RequerenteRepository(context);

            var reg = (from c in context.Processogti
                       join u in context.Usuario on c.Userid equals u.Id into uc from u in uc.DefaultIfEmpty()
                       where c.Ano == nAno && c.Numero == nNumero select new ProcessoStruct {
                           Ano = c.Ano, CodigoAssunto = c.Codassunto, AtendenteNome = u.Nomelogin, CentroCusto = (int)c.Centrocusto,
                           CodigoCidadao = (int)c.Codcidadao, Complemento = c.Complemento, DataArquivado = c.Dataarquiva, DataCancelado = c.Datacancel, DataEntrada = c.Dataentrada, DataReativacao = c.Datareativa,
                           DataSuspensao = c.Datasuspenso, Fisico = c.Fisico, Hora = c.Hora, Inscricao = (int)c.Insc, Interno = c.Interno, Numero = c.Numero, ObsArquiva = c.Obsa,
                           ObsCancela = c.Obsc, ObsReativa = c.Obsr, ObsSuspensao = c.Obss, Observacao = c.Observacao, Origem = c.Origem, TipoEnd = c.Tipoend, AtendenteId = (int)u.Id
                       }).FirstOrDefault();
            if (reg == null)
                return null;
            ProcessoStruct row = new ProcessoStruct {
                AtendenteNome = reg.AtendenteNome,
                AtendenteId = reg.AtendenteId,
                Dv = DvProcesso(nNumero)
            };
            row.SNumero = nNumero.ToString() + "-" + row.Dv.ToString() + "/" + nAno.ToString();
            row.Complemento = reg.Complemento;
            row.CodigoAssunto = Convert.ToInt32(reg.CodigoAssunto);
            row.Assunto = Retorna_Assunto(Convert.ToInt32(reg.CodigoAssunto));
            row.Observacao = reg.Observacao;
            row.Hora = reg.Hora == null ? "00:00" : reg.Hora.ToString();
            row.Inscricao = Convert.ToInt32(reg.Inscricao);
            row.DataEntrada = reg.DataEntrada;
            row.DataSuspensao = reg.DataSuspensao;
            row.DataReativacao = reg.DataReativacao;
            row.DataCancelado = reg.DataCancelado;
            row.DataArquivado = reg.DataArquivado;
            row.ListaAnexo = ListProcessoAnexo(nAno, nNumero);
            row.Anexo = ListProcessoAnexo(nAno, nNumero).Count().ToString() + " Anexo(s)";
            row.ListaAnexoLog = ListProcessoAnexoLog(nAno, nNumero);
            row.Interno = Convert.ToBoolean(reg.Interno);
            row.Fisico = Convert.ToBoolean(reg.Fisico);
            row.Origem = Convert.ToInt16(reg.Origem);
            if (!row.Interno) {
                row.CentroCusto = 0;
                row.CodigoCidadao = Convert.ToInt32(reg.CodigoCidadao);
                if (row.CodigoCidadao > 0) {
                    row.NomeCidadao = requerenteRepository.Dados_Cidadao((int)row.CodigoCidadao).Nome;
                } else
                    row.NomeCidadao = "";
            } else {
                row.CentroCusto = Convert.ToInt16(reg.CentroCusto);
                row.CodigoCidadao = 0;
                row.NomeCidadao = "";
            }
            row.ListaProcessoEndereco = ListProcessoEnd(nAno, nNumero);
            row.ObsArquiva = reg.ObsArquiva;
            row.ObsCancela = reg.ObsCancela;
            row.ObsReativa = reg.ObsReativa;
            row.ObsSuspensao = reg.ObsSuspensao;
            row.ListaProcessoDoc = ListProcessoDoc(nAno, nNumero);
            if (reg.TipoEnd == "1" || reg.TipoEnd == "2")
                row.TipoEnd = reg.TipoEnd.ToString();
            else
                row.TipoEnd = "R";
            return row;
        }

        public int DvProcesso(int Numero) {
            int soma = 0, index = 0, Mult = 6;
            string sNumProc = Numero.ToString().PadLeft(5, '0');
            while (index < 5) {
                int nChar = Convert.ToInt32(sNumProc.Substring(index, 1));
                soma += (Mult * nChar);
                Mult--;
                index++;
            }

            int DigAux = soma % 11;
            int Digito = 11 - DigAux;
            if (Digito == 10)
                Digito = 0;
            if (Digito == 11)
                Digito = 1;

            return Digito;
        }

        public string Retorna_Assunto(int Codigo) {
            string Sql = (from c in context.Assunto where c.Codigo == Codigo select c.Nome).FirstOrDefault();
            return Sql;
        }

        public List<ProcessoAnexoStruct> ListProcessoAnexo(int nAno, int nNumero) {
            var Sql = (from a in context.Anexo join p in context.Processogti on new { p1 = a.Anoanexo, p2 = a.Numeroanexo } equals new { p1 = p.Ano, p2 = p.Numero }
                       join c in context.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                       join u in context.Centrocusto on p.Centrocusto equals u.Codigo into pcu from u in pcu.DefaultIfEmpty()
                       where a.Ano == nAno && a.Numero == nNumero
                       select new ProcessoAnexoStruct { AnoAnexo = a.Anoanexo, NumeroAnexo = a.Numeroanexo, Complemento = p.Complemento, Requerente = c.Nomecidadao, CentroCusto = u.Descricao });
            return Sql.ToList();
        }

        public List<Anexo_logStruct> ListProcessoAnexoLog(int nAno, int nNumero) {
            var Sql = (from a in context.Anexo_Log
                       join u in context.Usuario on a.Userid equals u.Id into ac from u in ac.DefaultIfEmpty()
                       where a.Ano == nAno && a.Numero == nNumero
                       select new Anexo_logStruct {
                           Ano = (short)nAno, Numero = (short)nNumero, Ano_anexo = a.Ano_anexo, Numero_anexo = a.Numero_anexo,
                           Data = a.Data, Sid = a.Sid, Userid = a.Userid, Removido = a.Removido, Ocorrencia = a.Removido ? "Removido" : "Anexado", UserName = u.Nomecompleto
                       });
            return Sql.ToList();
        }

        public List<ProcessoDocStruct> ListProcessoDoc(int nAno, int nNumero) {
            var Sql = (from pd in context.Processodoc join d in context.Documento on pd.Coddoc equals d.Codigo where pd.Ano == nAno && pd.Numero == nNumero
                       select new ProcessoDocStruct { CodigoDocumento = pd.Coddoc, NomeDocumento = d.Nome, DataEntrega = pd.Data });
            return Sql.ToList();
        }

        public List<ProcessoEndStruct> ListProcessoEnd(int nAno, int nNumero) {
            var Sql = (from pe in context.Processoend join l in context.Logradouro on pe.Codlogr equals l.Codlogradouro where pe.Ano == nAno && pe.Numprocesso == nNumero
                       select new ProcessoEndStruct { CodigoLogradouro = pe.Codlogr, NomeLogradouro = l.Endereco, Numero = pe.Numero });
            return Sql.ToList();
        }

        public List<Tramitacao> DadosTramite(int Ano, int Numero) {
            var sql = (from t in context.Tramitacao where t.Ano==Ano && t.Numero==Numero orderby t.Seq select t).ToList();
            return sql;
        }

        public List<TramiteStruct> DadosTramite(short Ano, int Numero, int CodAssunto) {
            List<TramiteStruct> Lista = new List<TramiteStruct>();
            var reg = (from v in context.Tramitacaocc where v.Ano == Ano && v.Numero == Numero orderby v.Seq select new { v.Seq, v.Ccusto });
            if (reg.Count() > 0) {
                var reg5 = (from tcc in context.Tramitacaocc join cc in context.Centrocusto on tcc.Ccusto equals cc.Codigo where tcc.Ano == Ano && tcc.Numero == Numero select new { tcc.Seq, tcc.Ccusto, cc.Descricao, cc.Telefone });
                foreach (var query in reg5) {
                    TramiteStruct Linha = new TramiteStruct {
                        Seq = query.Seq,
                        CentroCustoCodigo = Convert.ToInt16(query.Ccusto),
                        CentroCustoNome = query.Descricao,
                        Telefone = query.Telefone
                    };
                    Lista.Add(Linha);
                }
            } else {
                var reg2 = (from t in context.Tramitacao join cc in context.Centrocusto on t.Ccusto equals cc.Codigo into tcc from cc in tcc.DefaultIfEmpty()
                            where t.Ano == Ano && t.Numero == Numero
                            select new { t.Seq, t.Ccusto, t.Despacho, cc.Descricao });
                foreach (var query in reg2) {
                    TramiteStruct Linha = new TramiteStruct {
                        Seq = query.Seq,
                        DespachoCodigo = (short)query.Despacho,
                        CentroCustoCodigo = Convert.ToInt16(query.Ccusto),
                        CentroCustoNome = query.Descricao
                    };
                    Lista.Add(Linha);
                }
                var reg3 = (from a in context.Assuntocc join cc in context.Centrocusto on a.Codcc equals cc.Codigo
                            where a.Codassunto == CodAssunto select new { a.Seq, cc.Codigo, cc.Descricao });
                foreach (var query in reg3) {
                    TramiteStruct Linha = new TramiteStruct {
                        Seq = query.Seq,
                        CentroCustoCodigo = Convert.ToInt16(query.Codigo),
                        CentroCustoNome = query.Descricao
                    };
                    Lista.Add(Linha);
                }
                //Incluir_MovimentoCC(Ano, Numero, Lista);
            }

            //Verifica os trâmites concluidos
            string sFullName = "";
            SistemaRepository sistemaRepository = new SistemaRepository(context);
            for (int i = 0; i < Lista.Count; i++) {
                short Seq = Convert.ToInt16(Lista[i].Seq);
                var reg4 = (from t in context.Tramitacao
                            join d in context.Despacho on t.Despacho equals d.Codigo into td from d in td.DefaultIfEmpty()
                            join u in context.Usuario on t.Userid equals u.Id into tu from u in tu.DefaultIfEmpty()
                            where t.Ano == Ano && t.Numero == Numero && t.Seq == Seq
                            select new { t.Seq, t.Ccusto, t.Datahora, t.Dataenvio, d.Descricao, t.Userid, t.Userid2, Usuario1 = u.Nomelogin, t.Obs,t.Obsinterna, t.Despacho });

                foreach (var query in reg4) {
                    Lista[i].Ano = Ano;
                    Lista[i].Numero = Numero;
                    Lista[i].DataEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("dd/MM/yyyy");
                    Lista[i].HoraEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("HH:mm");
                    sFullName = String.IsNullOrEmpty(query.Usuario1) ? "" : sistemaRepository.Retorna_User_FullName(query.Usuario1);
                    Lista[i].Userid1 = query.Userid;
                    Lista[i].Usuario1 = sFullName;
                    Lista[i].DespachoCodigo = (short)query.Despacho;
                    Lista[i].DespachoNome = String.IsNullOrEmpty(query.Descricao) ? "" : query.Descricao;
                    Lista[i].DataEnvio = query.Dataenvio == null ? "" : DateTime.Parse(query.Dataenvio.ToString()).ToString("dd/MM/yyyy");
                    Lista[i].Userid2 = query.Userid2;

                    if (query.Userid2 != null) {
                        string NomeLogin = sistemaRepository.Retorna_User_LoginName((int)query.Userid2);
                        Lista[i].Usuario2 = sistemaRepository.Retorna_User_FullName(NomeLogin);
                    } else
                        Lista[i].Usuario2 = "";
                    Lista[i].ObsGeral = String.IsNullOrEmpty(query.Obs) ? "" : query.Obs;
                    Lista[i].ObsInterna = String.IsNullOrEmpty(query.Obs) ? "" : query.Obsinterna;
                }
            }
            return Lista;
        }

        public List<Despacho> Lista_Despacho() {
                var Sql = (from c in context.Despacho orderby  c.Descricao select c);
                return Sql.ToList();
        }

        public List<Centrocusto> Lista_CentroCusto() {
            var Sql = (from c in context.Centrocusto where c.Ativo==true orderby c.Descricao select c);
            return Sql.ToList();
        }

        public List<UsuariocentroCusto> ListaCentroCustoUsuario(int idLogin) {
                var reg = (from u in context.Usuariocc join c in context.Centrocusto on u.Codigocc equals c.Codigo where u.Userid == idLogin
                           select new UsuariocentroCusto { Codigo = u.Codigocc, Nome = c.Descricao });
                List<UsuariocentroCusto> Lista = new List<UsuariocentroCusto>();
                foreach (var query in reg) {
                    UsuariocentroCusto Linha = new UsuariocentroCusto {
                        Codigo = query.Codigo,
                        Nome = query.Nome
                    };
                    Lista.Add(Linha);
                }
                return Lista;
        }

        public Exception Incluir_MovimentoCC(short Ano, int Numero, List<TramiteStruct> Lista) {

            var connStr = WebConfigurationManager.ConnectionStrings["GTIconnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);

            string sql = $"DELETE FROM TRAMITACAOCC WHERE ANO = {Ano} AND NUMERO={Numero}";
            using (SqlCommand command = new SqlCommand(sql, con)) {
                con.Open();
                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    return ex;
                }
            }

            short x = 1;
            foreach (TramiteStruct item in Lista) {
                Tramitacaocc NewReg = new Tramitacaocc {
                    Ano = Convert.ToInt16(Ano),
                    Numero = Numero,
                    Seq = x,
                    Ccusto = Convert.ToInt16(item.CentroCustoCodigo)
                };
                context.Tramitacaocc.Add(NewReg);

                try {
                    context.SaveChanges();
                } catch (Exception ex) {
                    return ex.InnerException;
                }
                x++;
            }
            return null;
        }

        public Exception Receber_Processo(Tramitacao Reg) {
            if (Existe_Tramite(Reg.Ano, Reg.Numero, Reg.Seq)) {
                Tramitacao t = context.Tramitacao.First(i => i.Ano == Reg.Ano && i.Numero == Reg.Numero && i.Seq == Reg.Seq);
                context.Tramitacao.Remove(t);
                context.SaveChanges();
            }

            try {
                context.Tramitacao.Add(Reg);
                context.SaveChanges();
            } catch (Exception ex) {
                return ex;
            }
            return null;
        }

        public Exception Enviar_Processo(Tramitacao Reg) {
            if (Existe_Tramite(Reg.Ano, Reg.Numero, Reg.Seq)) {
                Tramitacao t = context.Tramitacao.First(i => i.Ano == Reg.Ano && i.Numero == Reg.Numero && i.Seq == Reg.Seq);
                context.Tramitacao.Remove(t);
                context.SaveChanges();
            }
            try {
                context.Tramitacao.Add(Reg);
                context.SaveChanges();
            } catch (Exception ex) {
                return ex;
            }
            return null;
        }

        public bool Existe_Tramite(int Ano, int Numero, int Seq) {
            bool _valido = false;
            var existingReg = context.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq);
            if (existingReg > 0)
                _valido = true;
            return _valido;
        }

        public bool Tramite_Recebido(int Ano, int Numero, int Seq) {
            bool _valido = false;
            var existingReg = context.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq && a.Datahora!=null);
            if (existingReg > 0)
                _valido = true;
            return _valido;
        }

        public bool Tramite_Enviado(int Ano, int Numero, int Seq) {
            bool _valido = false;
            var existingReg = context.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq && a.Dataenvio != null);
            if (existingReg > 0)
                _valido = true;
            return _valido;
        }

        public Exception Move_Sequencia_Tramite_Acima(int Numero, int Ano, int Seq) {
            var connStr = WebConfigurationManager.ConnectionStrings["GTIconnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            con.Open();

            string sql = $"UPDATE TRAMITACAOCC SET SEQ=100 WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ={Seq}";
            SqlCommand command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            sql = $"UPDATE TRAMITACAOCC SET SEQ={Seq} WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ={Seq - 1}";
            command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            sql = $"UPDATE TRAMITACAOCC SET SEQ={Seq - 1} WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ=100";
            command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }
            con.Close();

            return null;
        }

        public Exception Move_Sequencia_Tramite_Abaixo(int Numero, int Ano, int Seq) {
            var connStr = WebConfigurationManager.ConnectionStrings["GTIconnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            con.Open();

            string sql = $"UPDATE TRAMITACAOCC SET SEQ=100 WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ={Seq + 1}";
            SqlCommand command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            sql = $"UPDATE TRAMITACAOCC SET SEQ={Seq + 1} WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ={Seq }";
            command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            sql = $"UPDATE TRAMITACAOCC SET SEQ={Seq } WHERE ANO = {Ano} AND NUMERO={Numero} AND SEQ=100";
            command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }
            con.Close();

            return null;
        }

        public Exception Inserir_Local(int Numero, int Ano, int Seq, int CCusto_Codigo) {

            List<Tramitacaocc> _lista_atual = (from c in context.Tramitacaocc where c.Ano == Ano && c.Numero == Numero orderby c.Seq select c).ToList();
            List<Tramitacaocc> _lista_nova = new List<Tramitacaocc>();

            int _pos = 1, _seq = 0;
            foreach (Tramitacaocc item in _lista_atual) {
                _seq = item.Seq;
                if (_pos > Seq)
                    _seq = item.Seq + 1;

                Tramitacaocc _newOrder = new Tramitacaocc() {
                    Ano = item.Ano,
                    Numero = item.Numero,
                    Seq = (byte)_seq,
                    Ccusto = item.Ccusto
                };

                _lista_nova.Add(_newOrder);
                _pos++;
            }

            Tramitacaocc _newItem = new Tramitacaocc() {
                Ano = (short)Ano,
                Numero = Numero,
                Seq = (byte)(Seq + 1),
                Ccusto = (short)CCusto_Codigo
            };

            _lista_nova.Add(_newItem);

            var connStr = WebConfigurationManager.ConnectionStrings["GTIconnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            string sql = $"DELETE FROM TRAMITACAOCC WHERE ANO = {Ano} AND NUMERO={Numero}";
            SqlCommand command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            foreach (Tramitacaocc item in _lista_nova.OrderBy(m => m.Seq)) {
                sql = $"INSERT TRAMITACAOCC VALUES({item.Ano},{item.Numero},{item.Seq},{item.Ccusto})";
                command = new SqlCommand(sql, con);
                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    return ex;
                }
            }

            return null;
        }

        public Exception Remover_Local(int Numero, int Ano, int Seq, int CCusto_Codigo) {

            List<Tramitacaocc> _lista_atual = (from c in context.Tramitacaocc where c.Ano == Ano && c.Numero == Numero orderby c.Seq select c).ToList();
            List<Tramitacaocc> _lista_nova = new List<Tramitacaocc>();

            foreach (Tramitacaocc item in _lista_atual) {
                if (item.Seq != Seq) {
                    Tramitacaocc _newOrder = new Tramitacaocc() {
                        Ano = item.Ano,
                        Numero = item.Numero,
                        Seq = item.Seq,
                        Ccusto = item.Ccusto
                    };

                    _lista_nova.Add(_newOrder);
                }
            }
            var connStr = WebConfigurationManager.ConnectionStrings["GTIconnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            string sql = $"DELETE FROM TRAMITACAOCC WHERE ANO = {Ano} AND NUMERO={Numero}";
            SqlCommand command = new SqlCommand(sql, con);
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {

                return ex;
            }

            int _pos = 1;
            foreach (Tramitacaocc item in _lista_nova.OrderBy(m => m.Seq)) {
                sql = $"INSERT TRAMITACAOCC VALUES({item.Ano},{item.Numero},{_pos},{item.Ccusto})";
                command = new SqlCommand(sql, con);
                try {
                    command.ExecuteNonQuery();
                } catch (Exception ex) {

                    return ex;
                }
                _pos++;
            }

            return null;
        }

        public Exception Alterar_Obs(int Ano,int Numero,int Seq,string ObsGeral,string ObsInterna) {
            Tramitacao _sql = context.Tramitacao.First(t => t.Ano == Ano && t.Numero==Numero && t.Seq==Seq);
            _sql.Obs = ObsGeral;
            _sql.Obsinterna = ObsInterna;
            try {
                context.SaveChanges();
            } catch (Exception ex) {
                return ex;
            }
            return null;
        }


    }
}
