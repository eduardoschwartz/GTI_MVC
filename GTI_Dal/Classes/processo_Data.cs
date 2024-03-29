﻿using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Processo_Data {

        private static string _connection;
        public Processo_Data(string sConnection) {
            _connection = sConnection;
        }

        public List<Documento> Lista_Documento() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = from c in db.Documento orderby c.Nome select c;
                return Sql.ToList();
            }
        }

        public string Retorna_Documento(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string Sql = (from c in db.Documento where c.Codigo == Codigo select c.Nome).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Incluir_Documento(Documento reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Documento select c).Count();
                short maxCod = 1;
                if (cntCod > 0)
                    maxCod = Convert.ToInt16((from c in db.Documento select c.Codigo).Max() + 1);
                reg.Codigo = maxCod;
                db.Documento.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Documento(Documento reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCoddoc = reg.Codigo;
                Documento b = db.Documento.First(i => i.Codigo == nCoddoc);
                b.Nome = reg.Nome;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Documento(Documento reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCoddoc = reg.Codigo;
                Documento b = db.Documento.First(i => i.Codigo == nCoddoc);
                try {
                    db.Documento.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Despacho> Lista_Despacho() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Despacho orderby c.Descricao select c);
                return Sql.ToList();
            }
        }

        public string Retorna_Despacho(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string Sql = (from c in db.Despacho where c.Codigo == Codigo select c.Descricao).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Incluir_Despacho(Despacho reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Despacho select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Despacho select c.Codigo).Max() + 1;
                reg.Codigo = Convert.ToInt16(maxCod);
                db.Despacho.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Despacho(Despacho reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCoddoc = reg.Codigo;
                Despacho b = db.Despacho.First(i => i.Codigo == nCoddoc);
                b.Descricao = reg.Descricao;
                b.Ativo = reg.Ativo;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Despacho(Despacho reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCoddoc = reg.Codigo;
                Despacho b = db.Despacho.First(i => i.Codigo == nCoddoc);
                try {
                    db.Despacho.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Assunto> Lista_Assunto(bool Somente_Ativo, bool Somente_Inativo, string Filter = "") {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Assunto select c);
                Sql = Sql.Where(c => c.Nome.Contains(Filter));
                if (Somente_Ativo)
                    Sql = Sql.Where(c => c.Ativo == true);
                if (Somente_Inativo)
                    Sql = Sql.Where(c => c.Ativo == false);
                Sql = Sql.OrderBy(c => c.Nome);
                return Sql.ToList();
            }
        }

        public string Retorna_Assunto(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string Sql = (from c in db.Assunto where c.Codigo == Codigo select c.Nome).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Incluir_Assunto(Assunto reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Assunto select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Assunto select c.Codigo).Max() + 1;
                reg.Codigo = Convert.ToInt16(maxCod);

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO Assunto(Codigo,nome,ativo) VALUES(@Codigo,@nome,@ativo)",
                        new SqlParameter("@Codigo", reg.Codigo),
                        new SqlParameter("@nome", reg.Nome),
                        new SqlParameter("@ativo", reg.Ativo));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Assunto_Local(List<Assuntocc> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM Assuntocc WHERE CodAssunto=@CodAssunto",
                        new SqlParameter("@CodAssunto", Lista[0].Codassunto));
                } catch (Exception ex) {
                    return ex;
                }

                foreach (Assuntocc item in Lista) {
                    Assuntocc reg = new Assuntocc {
                        Codassunto = item.Codassunto,
                        Codcc = item.Codcc,
                        Seq = item.Seq
                    };
                    db.Assuntocc.Add(reg);
                    try {
                        db.SaveChanges();
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Incluir_Assunto_Documento(List<Assuntodoc> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM Assuntodoc WHERE CodAssunto=@CodAssunto",
                        new SqlParameter("@CodAssunto", Lista[0].Codassunto));
                } catch (Exception ex) {
                    return ex;
                }

                foreach (Assuntodoc item in Lista) {
                    Assuntodoc reg = new Assuntodoc {
                        Codassunto = item.Codassunto,
                        Coddoc = item.Coddoc
                    };
                    db.Assuntodoc.Add(reg);
                    try {
                        db.SaveChanges();
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Alterar_Assunto(Assunto reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCoddoc = reg.Codigo;
                Assunto b = db.Assunto.First(i => i.Codigo == nCoddoc);
                b.Nome = reg.Nome;
                b.Ativo = reg.Ativo;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Assunto(Assunto reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM Assuntodoc WHERE CodAssunto=@CodAssunto", new SqlParameter("@CodAssunto", reg.Codigo));
                    db.Database.ExecuteSqlCommand("DELETE FROM Assuntocc WHERE CodAssunto=@CodAssunto", new SqlParameter("@CodAssunto", reg.Codigo));
                    db.Database.ExecuteSqlCommand("DELETE FROM Assunto WHERE Codigo=@CodAssunto", new SqlParameter("@CodAssunto", reg.Codigo));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Centrocusto> Lista_Local(bool Somente_Ativo, bool Local) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Centrocusto select c);
                if (Somente_Ativo)
                    Sql = Sql.Where(c => c.Ativo == true);
                if (Local)
                    Sql = Sql.Where(c => c.Local == true);
                Sql = Sql.OrderBy(c => c.Descricao);
                return Sql.ToList();
            }
        }

        public List<AssuntoLocal> Lista_Assunto_Local(short Assunto) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Assuntocc join c in db.Centrocusto on a.Codcc equals c.Codigo where a.Codassunto == Assunto
                           select new AssuntoLocal { Seq = (short)a.Seq, Codigo = (short)a.Codcc, Nome = c.Descricao }).OrderBy(u => u.Seq);
                return Sql.ToList();
            }
        }

        public List<AssuntoDocStruct> Lista_Assunto_Documento(short Assunto) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Assuntodoc join d in db.Documento on a.Coddoc equals d.Codigo where a.Codassunto == Assunto
                           select new AssuntoDocStruct { Codigo = (short)a.Coddoc, Nome = d.Nome }).OrderBy(u => u.Nome);
                return Sql.ToList();
            }
        }

        public Exception Incluir_Local(Centrocusto reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Centrocusto select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Centrocusto select c.Codigo).Max() + 1;
                reg.Codigo = Convert.ToInt16(maxCod);
                db.Centrocusto.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Local(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Centrocusto b = db.Centrocusto.First(i => i.Codigo == Codigo);
                try {
                    db.Centrocusto.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public short Retorna_Ultimo_Codigo_Local() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Centrocusto orderby c.Codigo descending select c.Codigo).FirstOrDefault();
                return Sql;
            }
        }

        public bool Existe_Processo(int Ano, int Numero) {
            bool bValido = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Processogti.Count(a => a.Ano == Ano && a.Numero == Numero);
                if (existingReg > 0)
                    bValido = true;
            }
            return bValido;
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

        public ProcessoCidadaoStruct Processo_cidadao_old(int ano, int numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from pc in db.Processocidadao
                           join l in db.Logradouro on pc.Codlogradouro equals l.Codlogradouro into pcl from l in pcl.DefaultIfEmpty()
                           join c in db.Cidade on new { p1 = pc.Siglauf, p2 = pc.Codcidade } equals new { p1 = c.Siglauf, p2 = c.Codcidade, } into pcc from c in pcc.DefaultIfEmpty()
                           join b in db.Bairro on new { p1 = pc.Siglauf, p2 = pc.Codcidade, p3 = pc.Codbairro } equals new { p1 = b.Siglauf, p2 = b.Codcidade, p3 = b.Codbairro } into pcb from b in pcb.DefaultIfEmpty()
                           where pc.Anoproc == ano && pc.Numproc == numero
                           select new ProcessoCidadaoStruct {
                               Codigo = pc.Codcidadao, Nome = pc.Nomecidadao, Documento = pc.Doc, Logradouro_Codigo = pc.Codlogradouro, Logradouro_Nome = l.Endereco.ToString(),
                               Numero = pc.Numimovel, Complemento = pc.Complemento, Bairro_Codigo = pc.Codbairro, Cidade_Nome = c.Desccidade, Bairro_Nome = b.Descbairro.ToString(),
                               UF = pc.Siglauf, Cep = pc.Cep, RG = pc.Rg.ToString(), Orgao = pc.Orgao.ToString()
                           }).FirstOrDefault();
                return reg;
            }
        }

        public ProcessoStruct Dados_Processo(int nAno, int nNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from c in db.Processogti
                           join u in db.Usuario on c.Userid equals u.Id into uc from u in uc.DefaultIfEmpty()
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
                        Cidadao_Data clsCidadao = new Cidadao_Data(_connection);
                        row.NomeCidadao = clsCidadao.Retorna_Cidadao((int)row.CodigoCidadao).Nomecidadao;
                    } else
                        row.NomeCidadao = "";
                } else {
                    row.CentroCusto = Convert.ToInt16(reg.CentroCusto);
                    row.CentroCustoNome = Retorna_CentroCusto((int)reg.CentroCusto);
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
        }

        private List<ProcessoDocStruct> ListProcessoDoc(int nAno, int nNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from pd in db.Processodoc join d in db.Documento on pd.Coddoc equals d.Codigo where pd.Ano == nAno && pd.Numero == nNumero
                           select new ProcessoDocStruct { CodigoDocumento = pd.Coddoc, NomeDocumento = d.Nome, DataEntrega = pd.Data });
                return Sql.ToList();
            }
        }

        private List<ProcessoEndStruct> ListProcessoEnd(int nAno, int nNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from pe in db.Processoend join l in db.Logradouro on pe.Codlogr equals l.Codlogradouro where pe.Ano == nAno && pe.Numprocesso == nNumero
                           select new ProcessoEndStruct { CodigoLogradouro = pe.Codlogr, NomeLogradouro = l.Endereco, Numero = pe.Numero });
                return Sql.ToList();
            }
        }

        private List<ProcessoAnexoStruct> ListProcessoAnexo(int nAno, int nNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Anexo join p in db.Processogti on new { p1 = a.Anoanexo, p2 = a.Numeroanexo } equals new { p1 = p.Ano, p2 = p.Numero }
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into pc from c in pc.DefaultIfEmpty()
                           join u in db.Centrocusto on p.Centrocusto equals u.Codigo into pcu from u in pcu.DefaultIfEmpty()
                           where a.Ano == nAno && a.Numero == nNumero
                           select new ProcessoAnexoStruct { AnoAnexo = a.Anoanexo, NumeroAnexo = a.Numeroanexo, Complemento = p.Complemento, Requerente = c.Nomecidadao, CentroCusto = u.Descricao });
                return Sql.ToList();
            }
        }

        private List<Anexo_logStruct> ListProcessoAnexoLog(int nAno, int nNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from a in db.Anexo_log
                           join u in db.Usuario on a.Userid equals u.Id into ac from u in ac.DefaultIfEmpty()
                           where a.Ano == nAno && a.Numero == nNumero
                           select new Anexo_logStruct {
                               Ano = (short)nAno, Numero = (short)nNumero, Ano_anexo = a.Ano_anexo, Numero_anexo = a.Numero_anexo,
                               Data = a.Data, Sid = a.Sid, Userid = a.Userid, Removido = a.Removido, Ocorrencia = a.Removido ? "Removido" : "Anexado", UserName = u.Nomecompleto
                           });
                return Sql.ToList();
            }
        }

        public Exception Cancelar_Processo(int Ano, int Numero, string Observacao) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processogti p = db.Processogti.First(i => i.Ano == Ano && i.Numero == Numero);
                p.Datacancel = DateTime.Now;
                p.Dataarquiva = null;
                p.Datareativa = null;
                p.Datasuspenso = null;
                p.Obsc = Observacao;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Reativar_Processo(int Ano, int Numero, string Observacao) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processogti p = db.Processogti.First(i => i.Ano == Ano && i.Numero == Numero);
                p.Datareativa = DateTime.Now;
                p.Dataarquiva = null;
                p.Datacancel = null;
                p.Datasuspenso = null;
                p.Obsr = Observacao;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Suspender_Processo(int Ano, int Numero, string Observacao) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processogti p = db.Processogti.First(i => i.Ano == Ano && i.Numero == Numero);
                p.Datareativa = null;
                p.Dataarquiva = null;
                p.Datacancel = null;
                p.Datasuspenso = DateTime.Now;
                p.Obss = Observacao;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Arquivar_Processo(int Ano, int Numero, string Observacao) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processogti p = db.Processogti.First(i => i.Ano == Ano && i.Numero == Numero);
                p.Datareativa = null;
                p.Dataarquiva = DateTime.Now;
                p.Datacancel = null;
                p.Datasuspenso = null;
                p.Obsa = Observacao;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Arquivar_Processo(int Ano, int Numero, DateTime Data_Arquivamento) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Processogti p = db.Processogti.First(i => i.Ano == Ano && i.Numero == Numero);
                p.Dataarquiva = Data_Arquivamento;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


        public Exception Excluir_Anexo(Anexo reg, string usuario) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM anexo WHERE ano=@ano AND numero=@numero AND anoanexo=@anoanexo AND numeroanexo=@numeroanexo",
                        new SqlParameter("@ano", reg.Ano), new SqlParameter("@numero", reg.Numero), new SqlParameter("@anoanexo", reg.Anoanexo), new SqlParameter("@numeroanexo", reg.Numeroanexo));
                } catch (Exception ex) {
                    return ex;
                }
                Sistema_Data clsSistema = new Sistema_Data(_connection);
                string sMsg = "O processo " + reg.Numeroanexo.ToString() + "-" + DvProcesso(reg.Numeroanexo) + "/" + reg.Anoanexo.ToString() + " foi desanexado por " + clsSistema.Retorna_User_FullName(usuario) + ".";
                Anexo_log p = new Anexo_log {
                    Ano = reg.Ano,
                    Numero = reg.Numero,
                    Ano_anexo = reg.Anoanexo,
                    Numero_anexo = reg.Numeroanexo,
                    Data = DateTime.Now,
                    Removido = true
                };
                p.Userid = clsSistema.Retorna_User_LoginId(usuario);
                db.Anexo_log.Add(p);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }

                return null;
            }
        }

        public Exception Incluir_Anexo(Anexo reg, string usuario) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Anexo.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                Sistema_Data clsSistema = new Sistema_Data(_connection);
                string sMsg = "O processo " + reg.Numeroanexo.ToString() + "-" + DvProcesso(reg.Numeroanexo) + "/" + reg.Anoanexo.ToString() + " foi anexado por " + clsSistema.Retorna_User_FullName(usuario) + ".";
                Anexo_log p = new Anexo_log {
                    Ano = reg.Ano,
                    Numero = reg.Numero,
                    Ano_anexo = reg.Anoanexo,
                    Numero_anexo = reg.Numeroanexo,
                    Data = DateTime.Now,
                    Removido = false
                };
                p.Userid = clsSistema.Retorna_User_LoginId(usuario);
                db.Anexo_log.Add(p);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Historico_Processo(short Ano, int Numero, string Historico, string Usuario) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntSeq = (from p in db.Processo_historico where p.Ano == Ano && p.Numero == Numero select p).Count();
                int maxSeq = 1;
                if (cntSeq > 0)
                    maxSeq = (from p in db.Processo_historico where p.Ano == Ano && p.Numero == Numero select p.Seq).Max() + 1;
                Processo_historico reg = new Processo_historico {
                    Ano = Ano,
                    Numero = Numero,
                    Seq = maxSeq,
                    Historico = Historico.Length > 5000 ? Historico.Substring(0, 5000) : Historico,
                    Datahora = DateTime.Now,
                    Usuario = Usuario
                };
                db.Processo_historico.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Assunto_uso_processo(short Codigo_Assunto) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod1 = (from p in db.Processogti where p.Codassunto == Codigo_Assunto select p).Count();
                return cntCod1 > 0 ? true : false;
            }
        }

        public Exception Incluir_MovimentoCC(short Ano, int Numero, List<TramiteStruct> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string sql = "DELETE FROM TRAMITACAOCC WHERE ANO = @P0 AND NUMERO=@P1";
                List<object> parameterList = new List<object> {
                    Ano,
                    Numero
                };
                object[] parameters1 = parameterList.ToArray();
                int result = db.Database.ExecuteSqlCommand(sql, parameters1);

                short x = 1;
                foreach (TramiteStruct item in Lista) {
                    Tramitacaocc NewReg = new Tramitacaocc {
                        Ano = Convert.ToInt16(Ano),
                        Numero = Numero,
                        Seq = x,
                        Ccusto = Convert.ToInt16(item.CentroCustoCodigo)
                    };
                    db.Tramitacaocc.Add(NewReg);

                    try {
                        db.SaveChanges();
                    } catch (Exception ex) {
                        return ex.InnerException;
                    }
                    x++;
                }
                return null;
            }
        }

        public List<TramiteStruct> DadosTramite(short Ano, int Numero, int CodAssunto) {
            List<TramiteStruct> Lista = new List<TramiteStruct>();
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from v in db.Tramitacaocc where v.Ano == Ano && v.Numero == Numero orderby v.Seq select new { v.Seq, v.Ccusto });
                if (reg.Count() > 0) {
                    var reg5 = (from tcc in db.Tramitacaocc join cc in db.Centrocusto on tcc.Ccusto equals cc.Codigo where tcc.Ano == Ano && tcc.Numero == Numero select new { tcc.Seq, tcc.Ccusto, cc.Descricao, cc.Telefone });
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
                    var reg2 = (from t in db.Tramitacao join cc in db.Centrocusto on t.Ccusto equals cc.Codigo into tcc from cc in tcc.DefaultIfEmpty()
                                where t.Ano == Ano && t.Numero == Numero
                                select new { t.Seq, t.Ccusto, cc.Descricao });
                    foreach (var query in reg2) {
                        TramiteStruct Linha = new TramiteStruct {
                            Seq = query.Seq,
                            CentroCustoCodigo = Convert.ToInt16(query.Ccusto),
                            CentroCustoNome = query.Descricao
                        };
                        Lista.Add(Linha);
                    }
                    var reg3 = (from a in db.Assuntocc join cc in db.Centrocusto on a.Codcc equals cc.Codigo
                                where a.Codassunto == CodAssunto select new { a.Seq, cc.Codigo, cc.Descricao });
                    foreach (var query in reg3) {
                        TramiteStruct Linha = new TramiteStruct {
                            Seq = query.Seq,
                            CentroCustoCodigo = Convert.ToInt16(query.Codigo),
                            CentroCustoNome = query.Descricao
                        };
                        Lista.Add(Linha);
                    }
                    Incluir_MovimentoCC(Ano, Numero, Lista);
                }

                //Verifica os trâmites concluidos
                Sistema_Data clsSistema = new Sistema_Data(_connection);
                string sFullName = "";
                for (int i = 0; i < Lista.Count; i++) {
                    short Seq = Convert.ToInt16(Lista[i].Seq);
                    var reg4 = (from t in db.Tramitacao
                                join d in db.Despacho on t.Despacho equals d.Codigo into td from d in td.DefaultIfEmpty()
                                join u in db.Usuario on t.Userid equals u.Id into tu from u in tu.DefaultIfEmpty()
                                where t.Ano == Ano && t.Numero == Numero && t.Seq == Seq
                                select new { t.Seq, t.Ccusto, t.Datahora, t.Dataenvio, t.Despacho, d.Descricao, t.Userid, t.Userid2, Usuario1 = u.Nomelogin, t.Obs, t.Obsinterna });

                    foreach (var query in reg4) {
                        Lista[i].DataEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("dd/MM/yyyy");
                        Lista[i].HoraEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("hh:mm");
                        sFullName = string.IsNullOrEmpty(query.Usuario1) ? "" : clsSistema.Retorna_User_FullName(query.Usuario1);
                        Lista[i].Userid1 = query.Userid;
                        Lista[i].Usuario1 = sFullName;
                        Lista[i].DespachoCodigo = query.Despacho == null ? (short)0 : (short)query.Despacho;
                        Lista[i].DespachoNome = string.IsNullOrEmpty(query.Descricao) ? "" : query.Descricao;
                        Lista[i].DataEnvio = query.Dataenvio == null ? "" : DateTime.Parse(query.Dataenvio.ToString()).ToString("dd/MM/yyyy");
                        Lista[i].Userid2 = query.Userid2;

                        if (query.Userid2 != null) {
                            string NomeLogin = clsSistema.Retorna_User_LoginName((int)query.Userid2);
                            Lista[i].Usuario2 = clsSistema.Retorna_User_FullName(NomeLogin);
                        } else
                            Lista[i].Usuario2 = "";
                        Lista[i].Obs = string.IsNullOrEmpty(query.Obs) ? "" : query.Obs;
                        Lista[i].ObsGeral = string.IsNullOrEmpty(query.Obs) ? "" : query.Obs;
                        Lista[i].ObsInterna = string.IsNullOrEmpty(query.Obsinterna) ? "" : query.Obsinterna;
                    }
                }
            }
            return Lista;
        }

        public List<UsuariocentroCusto> ListaCentrocustoUsuario(int idLogin) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from u in db.Usuariocc join c in db.Centrocusto on u.Codigocc equals c.Codigo where u.Userid == idLogin
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
        }

        public Exception Alterar_Local(Centrocusto reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCodigo = reg.Codigo;
                Centrocusto b = db.Centrocusto.First(i => i.Codigo == nCodigo);
                b.Descricao = reg.Descricao;
                b.Telefone = reg.Telefone;
                b.Ativo = reg.Ativo;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Observacao_Tramite(int Ano, int Numero, int Seq, string Observacao_Geral, string Observacao_Interna) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    Tramitacao t = db.Tramitacao.First(i => i.Ano == Ano && i.Numero == Numero && i.Seq == Seq);
                    t.Obs = string.IsNullOrWhiteSpace(Observacao_Geral) ? null : Observacao_Geral;
                    t.Obsinterna = string.IsNullOrWhiteSpace(Observacao_Interna) ? null : Observacao_Interna;
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Despacho(int Ano, int Numero, int Seq, short CodigoDespacho) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    Tramitacao t = db.Tramitacao.First(i => i.Ano == Ano && i.Numero == Numero && i.Seq == Seq);
                    t.Despacho = CodigoDespacho;
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<UsuarioFuncStruct> ListaFuncionario(int LoginId) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from f in db.Usuariofunc join u in db.Usuario on f.Funclogin equals u.Nomelogin
                           where f.Userid == LoginId orderby u.Nomecompleto select new { f.Funclogin, u.Nomecompleto });
                List<UsuarioFuncStruct> Lista = new List<UsuarioFuncStruct>();

                foreach (var query in reg) {
                    UsuarioFuncStruct Linha = new UsuarioFuncStruct {
                        FuncLogin = Convert.ToInt16(query.Funclogin.Substring(query.Funclogin.Length - 3)),
                        NomeCompleto = query.Nomecompleto
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public Exception Excluir_Tramite(int Ano, int Numero, int Seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    Tramitacao t = db.Tramitacao.FirstOrDefault(i => i.Ano == Ano && i.Numero == Numero && i.Seq == Seq);
                    if (t != null) {
                        db.Tramitacao.Remove(t);
                        db.SaveChanges();
                    }
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Tramite(Tramitacao Reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Tramitacao.Add(Reg);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Tramite(Tramitacao Reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    Tramitacao t = db.Tramitacao.First(i => i.Ano == Reg.Ano && i.Numero == Reg.Numero && i.Seq == Reg.Seq);
                    t.Despacho = Reg.Despacho;
                    t.Dataenvio = Reg.Dataenvio;
                    t.Userid2 = Reg.Userid2;
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Cidadao_Processo(int id_cidadao) {
            int _contador = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
            Inicio:;
                try {
                    _contador = (from p in db.Processogti where p.Codcidadao == id_cidadao select p.Numero).Count();
                } catch {
                    goto Inicio; //este erro só acontece no timeout, então tenta até conseguir.                   
                }
                return _contador > 0 ? true : false;
            }
        }

        public List<ProcessoStruct> Lista_Processos_Abertos() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processogti where  p.Ano>=2022 && p.Dataarquiva == null select p).ToList();
                List<ProcessoStruct> Lista = new List<ProcessoStruct>();
                foreach (Processogti item in Sql) {
                    ProcessoStruct reg = new ProcessoStruct() {
                        Numero = item.Numero,
                        Ano = item.Ano
                    };
                    Lista.Add(reg);
                }
                return Lista;
            }
        }

        public List<ProcessoStruct> Lista_Processos(ProcessoFilter Filter) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processogti
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into cp from c in cp.DefaultIfEmpty()
                           join a in db.Assunto on p.Codassunto equals a.Codigo into ap from a in ap.DefaultIfEmpty()
                           join e in db.Processoend on new { P1 = p.Ano, P2 = p.Numero } equals new { P1 = e.Ano, P2 = e.Numprocesso } into ep from e in ep.DefaultIfEmpty()
                           join l in db.Logradouro on e.Codlogr equals l.Codlogradouro into le from l in le.DefaultIfEmpty()
                           join u in db.Centrocusto on p.Centrocusto equals u.Codigo into pu from u in pu.DefaultIfEmpty()
                           orderby p.Ano, p.Numero
                           select new ProcessoStruct {
                               Ano = p.Ano, Numero = p.Numero, NomeCidadao = c.Nomecidadao, Assunto = a.Nome, DataEntrada = p.Dataentrada, DataCancelado = p.Datacancel,
                               DataReativacao = p.Datareativa, DataArquivado = p.Dataarquiva, DataSuspensao = p.Datasuspenso, Interno = p.Interno, Fisico = p.Fisico, LogradouroNome = l.Endereco,
                               LogradouroNumero = e.Numero, Complemento = p.Complemento, CentroCustoNome = u.Descricao, Inscricao = p.Insc, CodigoCidadao = p.Codcidadao, CodigoAssunto = p.Codassunto,
                               CentroCusto = p.Centrocusto
                           });
                if (!string.IsNullOrWhiteSpace(Filter.SNumProcesso))
                    Sql = Sql.Where(c => c.Ano == Filter.Ano && c.Numero == Filter.Numero);
                if (Filter.AnoIni > 0)
                    Sql = Sql.Where(c => c.Ano >= Filter.AnoIni);
                if (Filter.AnoFim > 0)
                    Sql = Sql.Where(c => c.Ano <= Filter.AnoFim);
                if (Filter.Arquivado == false)
                    Sql = Sql.Where(c => c.DataArquivado == null);
                if (Filter.Arquivado == true)
                    Sql = Sql.Where(c => c.DataArquivado != null);
                if (Filter.Requerente != null && Filter.Requerente > 0)
                    Sql = Sql.Where(c => c.CodigoCidadao == Filter.Requerente);
                if (Filter.DataEntrada != null)
                    Sql = Sql.Where(c => c.DataEntrada == Filter.DataEntrada);
                if (Filter.Setor > 0)
                    Sql = Sql.Where(c => c.CentroCusto == Filter.Setor);
                if (Filter.AssuntoCodigo > 0)
                    Sql = Sql.Where(c => c.CodigoAssunto <= Filter.AssuntoCodigo);
                if (!string.IsNullOrEmpty(Filter.Complemento))
                    Sql = Sql.Where(c => c.Complemento.Contains(Filter.Complemento));
                if (Filter.Fisico != null)
                    Sql = Sql.Where(c => c.Fisico == Filter.Fisico);
                if (Filter.Interno != null)
                    Sql = Sql.Where(c => c.Interno == Filter.Interno);

                return Sql.ToList();
            }
        }

        public List<ProcessoStruct> Lista_Processos_Web(ProcessoFilter Filter) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processogti
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into cp from c in cp.DefaultIfEmpty()
                           join a in db.Assunto on p.Codassunto equals a.Codigo into ap from a in ap.DefaultIfEmpty()
                           join e in db.Processoend on new { P1 = p.Ano, P2 = p.Numero } equals new { P1 = e.Ano, P2 = e.Numprocesso } into ep from e in ep.DefaultIfEmpty()
                           join l in db.Logradouro on e.Codlogr equals l.Codlogradouro into le from l in le.DefaultIfEmpty()
                           join u in db.Centrocusto on p.Centrocusto equals u.Codigo into pu from u in pu.DefaultIfEmpty()
                           orderby p.Ano, p.Numero
                           select new ProcessoStruct {
                               Ano = p.Ano, Numero = p.Numero, NomeCidadao = c.Nomecidadao, Assunto = a.Nome, DataEntrada = p.Dataentrada, DataCancelado = p.Datacancel,
                               DataReativacao = p.Datareativa, DataArquivado = p.Dataarquiva, DataSuspensao = p.Datasuspenso, Interno = p.Interno, Fisico = p.Fisico, LogradouroNome = l.Endereco,
                               LogradouroNumero = e.Numero, Complemento = p.Complemento, CentroCustoNome = u.Descricao, Inscricao = p.Insc, CodigoCidadao = p.Codcidadao, CodigoAssunto = p.Codassunto,
                               CentroCusto = p.Centrocusto, LogradouroCodigo = e.Codlogr
                           });
                if (Filter.Ano > 0)
                    Sql = Sql.Where(c => c.Ano == Filter.Ano);
                if (Filter.Numero > 0)
                    Sql = Sql.Where(c => c.Numero == Filter.Numero);
                if (dalCore.IsDate(Filter.DataEntrada))
                    Sql = Sql.Where(c => c.DataEntrada == Filter.DataEntrada);
                if (Filter.AssuntoCodigo > 0)
                    Sql = Sql.Where(c => c.CodigoAssunto == Filter.AssuntoCodigo);
                if (Filter.CodLogradouro > 0)
                    Sql = Sql.Where(c => c.LogradouroCodigo == Filter.CodLogradouro);
                if (Filter.NumEnd > 0)
                    Sql = Sql.Where(c => c.LogradouroNumero == Filter.NumEnd.ToString());
                if (Filter.Requerente > 0) {
                    if (Filter.Requerente < 500000) {
                        Sql = Sql.Where(c => c.CentroCusto == Filter.Requerente);
                    } else {
                        Sql = Sql.Where(c => c.CodigoCidadao == Filter.Requerente);
                    }
                }
                return Sql.ToList();
            }
        }


        public DateTime? Data_Processo(int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                DateTime? Sql = (from p in db.Processogti where p.Ano == Ano && p.Numero == Numero select p.Dataentrada).FirstOrDefault();
                if (Sql == null)
                    return null;
                else
                    return Sql;
            }
        }

        public Exception Incluir_Processo(Processogti reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[26];
                Parametros[0] = new SqlParameter { ParameterName = "@Ano", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Ano };
                Parametros[1] = new SqlParameter { ParameterName = "@Numero", SqlDbType = SqlDbType.Int, SqlValue = reg.Numero };
                Parametros[2] = new SqlParameter { ParameterName = "@Fisico", SqlDbType = SqlDbType.Bit, SqlValue = reg.Fisico };
                Parametros[3] = new SqlParameter { ParameterName = "@Origem", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Origem };
                Parametros[4] = new SqlParameter { ParameterName = "@Interno", SqlDbType = SqlDbType.Bit, SqlValue = reg.Interno };
                Parametros[5] = new SqlParameter { ParameterName = "@Codassunto", SqlDbType = SqlDbType.SmallInt, SqlValue = reg.Codassunto };
                if (reg.Complemento != null)
                    Parametros[6] = new SqlParameter { ParameterName = "@Complemento", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Complemento };
                else
                    Parametros[6] = new SqlParameter { ParameterName = "@Complemento", SqlValue = DBNull.Value };
                if (reg.Observacao != null)
                    Parametros[7] = new SqlParameter { ParameterName = "@Observacao", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Observacao };
                else
                    Parametros[7] = new SqlParameter { ParameterName = "@Observacao", SqlValue = DBNull.Value };
                if (reg.Dataentrada != null)
                    Parametros[8] = new SqlParameter { ParameterName = "@Dataentrada", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Dataentrada };
                else
                    Parametros[8] = new SqlParameter { ParameterName = "@Dataentrada", SqlValue = DBNull.Value };
                if (reg.Datareativa != null)
                    Parametros[9] = new SqlParameter { ParameterName = "@Datareativa", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Datareativa };
                else
                    Parametros[9] = new SqlParameter { ParameterName = "@Datareativa", SqlValue = DBNull.Value };
                if (reg.Datacancel != null)
                    Parametros[10] = new SqlParameter { ParameterName = "@Datacancel", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Datacancel };
                else
                    Parametros[10] = new SqlParameter { ParameterName = "@Datacancel", SqlValue = DBNull.Value };
                if (reg.Dataarquiva != null)
                    Parametros[11] = new SqlParameter { ParameterName = "@Dataarquiva", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Dataarquiva };
                else
                    Parametros[11] = new SqlParameter { ParameterName = "@Dataarquiva", SqlValue = DBNull.Value };
                if (reg.Datasuspenso != null)
                    Parametros[12] = new SqlParameter { ParameterName = "@Datasuspenso", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Datasuspenso };
                else
                    Parametros[12] = new SqlParameter { ParameterName = "@Datasuspenso", SqlValue = DBNull.Value };
                if (reg.Etiqueta != null)
                    Parametros[13] = new SqlParameter { ParameterName = "@Etiqueta", SqlDbType = SqlDbType.Bit, SqlValue = reg.Etiqueta };
                else
                    Parametros[13] = new SqlParameter { ParameterName = "@Etiqueta", SqlValue = DBNull.Value };
                if (reg.Codcidadao != null)
                    Parametros[14] = new SqlParameter { ParameterName = "@Codcidadao", SqlDbType = SqlDbType.Int, SqlValue = reg.Codcidadao };
                else
                    Parametros[14] = new SqlParameter { ParameterName = "@Codcidadao", SqlValue = DBNull.Value };
                if (reg.Motivocancel != null)
                    Parametros[15] = new SqlParameter { ParameterName = "@Motivocancel", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Motivocancel };
                else
                    Parametros[15] = new SqlParameter { ParameterName = "@Motivocancel", SqlValue = DBNull.Value };
                if (reg.Centrocusto != null)
                    Parametros[16] = new SqlParameter { ParameterName = "@Centrocusto", SqlDbType = SqlDbType.Int, SqlValue = reg.Centrocusto };
                else
                    Parametros[16] = new SqlParameter { ParameterName = "@Centrocusto", SqlValue = DBNull.Value };
                if (reg.Obsa != null)
                    Parametros[17] = new SqlParameter { ParameterName = "@Obsa", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Obsa };
                else
                    Parametros[17] = new SqlParameter { ParameterName = "@Obsa", SqlValue = DBNull.Value };
                if (reg.Obsc != null)
                    Parametros[18] = new SqlParameter { ParameterName = "@Obsc", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Obsc };
                else
                    Parametros[18] = new SqlParameter { ParameterName = "@Obsc", SqlValue = DBNull.Value };
                if (reg.Obss != null)
                    Parametros[19] = new SqlParameter { ParameterName = "@Obss", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Obss };
                else
                    Parametros[19] = new SqlParameter { ParameterName = "@Obss", SqlValue = DBNull.Value };
                if (reg.Obsr != null)
                    Parametros[20] = new SqlParameter { ParameterName = "@Obsr", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Obsr };
                else
                    Parametros[20] = new SqlParameter { ParameterName = "@Obsr", SqlValue = DBNull.Value };
                if (reg.Hora != null)
                    Parametros[21] = new SqlParameter { ParameterName = "@Hora", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Hora };
                else
                    Parametros[21] = new SqlParameter { ParameterName = "@Hora", SqlValue = DBNull.Value };
                if (reg.Insc != null)
                    Parametros[22] = new SqlParameter { ParameterName = "@Insc", SqlDbType = SqlDbType.Int, SqlValue = reg.Insc };
                else
                    Parametros[22] = new SqlParameter { ParameterName = "@Insc", SqlValue = DBNull.Value };
                if (reg.Tipoend != null)
                    Parametros[23] = new SqlParameter { ParameterName = "@Tipoend", SqlDbType = SqlDbType.Char, SqlValue = reg.Tipoend };
                else
                    Parametros[23] = new SqlParameter { ParameterName = "@Tipoend", SqlValue = DBNull.Value };
                if (reg.Userid != null)
                    Parametros[24] = new SqlParameter { ParameterName = "@Userid", SqlDbType = SqlDbType.Int, SqlValue = reg.Userid };
                else
                    Parametros[24] = new SqlParameter { ParameterName = "@Userid", SqlValue = DBNull.Value };
                Parametros[25] = new SqlParameter { ParameterName = "@Userweb", SqlDbType = SqlDbType.Bit, SqlValue = reg.Userweb };

                db.Database.ExecuteSqlCommand("INSERT INTO processogti(ano,numero,fisico,origem,interno,codassunto,complemento,observacao,dataentrada,datareativa," +
                                              "datacancel,dataarquiva,datasuspenso,etiqueta,codcidadao,motivocancel,centrocusto,obsa,obsc,obss,obsr,hora,insc,tipoend," +
                                              "userid,Userweb) VALUES(@ano,@numero,@fisico,@origem,@interno,@codassunto,@complemento,@observacao,@dataentrada,@datareativa," +
                                              "@datacancel,@dataarquiva,@datasuspenso,@etiqueta,@codcidadao,@motivocancel,@centrocusto,@obsa,@obsc,@obss,@obsr,@hora," +
                                              "@insc,@tipoend,@userid,@Userweb)", Parametros);

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public int Retorna_Numero_Disponivel(int Ano) {
            int maxCod = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from p in db.Processogti where p.Ano == Ano select p.Numero).Max();
                maxCod = Convert.ToInt32(Sql) + 1;
            }
            return maxCod;
        }

        public Exception Incluir_Processo_Endereco(List<Processoend> Lista, int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {

                try {
                    db.Database.ExecuteSqlCommand("delete from processoend where ano=@ano and numprocesso=@numprocesso",
                        new SqlParameter("@ano", Ano), new SqlParameter("@numprocesso", Numero));
                } catch (Exception ex) {
                    return ex;
                }
                foreach (Processoend item in Lista) {
                    try {
                        db.Database.ExecuteSqlCommand("insert into processoend(ano,numprocesso,codlogr,numero) VALUES(@ano,@numprocesso,@codlogr,@numero)",
                        new SqlParameter("@ano", item.Ano),
                        new SqlParameter("@numprocesso", item.Numprocesso),
                        new SqlParameter("@codlogr", item.Codlogr),
                        new SqlParameter("@numero", item.Numero));
                    } catch (Exception ex) {
                        return ex;
                    }
                }
                return null;
            }
        }

        public Exception Incluir_Processo_Documento(List<Processodoc> Lista, int Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {

                try {
                    db.Database.ExecuteSqlCommand("delete from processodoc where ano=@ano and numero=@numero",
                        new SqlParameter("@ano", Ano), new SqlParameter("@numero", Numero));
                } catch (Exception ex) {
                    return ex;
                }
                foreach (Processodoc item in Lista) {
                    try {
                        object[] Parametros = new object[4];
                        Parametros[0] = new SqlParameter { ParameterName = "@ano", SqlDbType = SqlDbType.SmallInt, SqlValue = item.Ano };
                        Parametros[1] = new SqlParameter { ParameterName = "@numero", SqlDbType = SqlDbType.Int, SqlValue = item.Numero };
                        Parametros[2] = new SqlParameter { ParameterName = "@coddoc", SqlDbType = SqlDbType.SmallInt, SqlValue = item.Coddoc };
                        if (item.Data != null)
                            Parametros[3] = new SqlParameter { ParameterName = "@data", SqlDbType = SqlDbType.SmallDateTime, SqlValue = item.Data };
                        else
                            Parametros[3] = new SqlParameter { ParameterName = "@data", SqlValue = DBNull.Value };

                        db.Database.ExecuteSqlCommand("INSERT INTO processodoc(ano,numero,coddoc,data) VALUES(@ano,@numero,@coddoc,@data)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }
                return null;
            }
        }

        public Exception Alterar_Processo(Processogti reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                short _ano = reg.Ano;
                int _numero = reg.Numero;
                Processogti p = db.Processogti.First(i => i.Ano == _ano && i.Numero == _numero);
                p.Centrocusto = reg.Centrocusto;
                p.Codassunto = reg.Codassunto;
                p.Codcidadao = reg.Codcidadao;
                p.Complemento = reg.Complemento;
                p.Dataarquiva = reg.Dataarquiva;
                p.Datacancel = reg.Datacancel;
                p.Dataentrada = reg.Dataentrada;
                p.Datareativa = reg.Datareativa;
                p.Datasuspenso = reg.Datasuspenso;
                p.Etiqueta = reg.Etiqueta;
                p.Fisico = reg.Fisico;
                p.Hora = reg.Hora;
                p.Insc = reg.Insc;
                p.Interno = reg.Interno;
                p.Motivocancel = reg.Motivocancel;
                p.Numero = reg.Numero;
                p.Obsa = reg.Obsa;
                p.Obsc = reg.Obsc;
                p.Observacao = reg.Observacao;
                p.Obsr = reg.Obsr;
                p.Obss = reg.Obss;
                p.Origem = reg.Origem;
                p.Tipoend = reg.Tipoend;
                p.Userid = reg.Userid;

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Processo_Web(Processogti reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                short _ano = reg.Ano;
                int _numero = reg.Numero;
                Processogti p = db.Processogti.First(i => i.Ano == _ano && i.Numero == _numero);
                p.Complemento = reg.Complemento;
                p.Observacao = reg.Observacao;

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


        public List<Processo_Numero> Lista_Processo_Parcelamento_Header(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from p in db.Processoreparc where p.Codigoresp == Codigo orderby p.Anoproc, p.Numproc
                           select new Processo_Numero { Numero_processo = p.Numprocesso, Numero = p.Numproc, Ano = p.Anoproc });
                return Sql.ToList();
            }
        }

        public bool Existe_Tramite(int Ano, int Numero, int Seq) {
            bool bValido = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq);
                if (existingReg > 0)
                    bValido = true;
            }
            return bValido;
        }

        public Exception Receber_Processo(Tramitacao Reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                if (Existe_Tramite(Reg.Ano, Reg.Numero, Reg.Seq)) {
                    Tramitacao t = db.Tramitacao.First(i => i.Ano == Reg.Ano && i.Numero == Reg.Numero && i.Seq == Reg.Seq);
                    db.Tramitacao.Remove(t);
                    db.SaveChanges();
                }

                try {
                    db.Tramitacao.Add(Reg);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public TramiteStruct Dados_Tramite(int Ano, int Numero, int Seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Sistema_Data clsSistema = new Sistema_Data(_connection);
                var sql = (from t in db.Tramitacao
                           join d in db.Despacho on t.Despacho equals d.Codigo into td from d in td.DefaultIfEmpty()
                           join u in db.Usuario on t.Userid equals u.Id into tu from u in tu.DefaultIfEmpty()
                           where t.Ano == Ano && t.Numero == Numero && t.Seq == Seq
                           select new { t.Seq, t.Ccusto, t.Datahora, t.Dataenvio, t.Despacho, d.Descricao, t.Userid, t.Userid2, Usuario1 = u.Nomelogin, t.Obs, t.Obsinterna });

                TramiteStruct reg = new TramiteStruct();
                foreach (var query in sql) {
                    reg.DataEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("dd/MM/yyyy");
                    reg.HoraEntrada = query.Datahora.ToString() == "" ? "" : DateTime.Parse(query.Datahora.ToString()).ToString("hh:mm");
                    reg.Usuario1 = String.IsNullOrEmpty(query.Usuario1) ? "" : clsSistema.Retorna_User_FullName(query.Usuario1);
                    reg.Userid1 = query.Userid;
                    reg.DespachoCodigo = query.Despacho == null ? (short)0 : (short)query.Despacho;
                    reg.DespachoNome = String.IsNullOrEmpty(query.Descricao) ? "" : query.Descricao;
                    reg.DataEnvio = query.Dataenvio == null ? "" : DateTime.Parse(query.Dataenvio.ToString()).ToString("dd/MM/yyyy");
                    reg.Userid2 = query.Userid2;
                    if (query.Userid2 != null) {
                        string NomeLogin = clsSistema.Retorna_User_LoginName((int)query.Userid2);
                        reg.Usuario2 = clsSistema.Retorna_User_FullName(NomeLogin);
                    } else
                        reg.Usuario2 = "";
                    reg.Obs = String.IsNullOrEmpty(query.Obs) ? "" : query.Obs;
                    reg.ObsGeral = String.IsNullOrEmpty(query.Obs) ? "" : query.Obs;
                    reg.ObsInterna = String.IsNullOrEmpty(query.Obsinterna) ? "" : query.Obsinterna;
                }

                return reg;
            }
        }

        public int Retorna_CCusto_TramiteCC(int Ano, int Numero, int Seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int Sql = (from t in db.Tramitacaocc where t.Ano == Ano && t.Numero == Numero && t.Seq == Seq select t.Ccusto).FirstOrDefault();
                return Sql;
            }
        }

        public int Retorna_Processo_Assunto(short Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int Sql = (from t in db.Processogti where t.Ano == Ano && t.Numero == Numero select t.Codassunto).FirstOrDefault();
                return Sql;
            }
        }

        //public Processogti Retorna_ProcessoGti(short Ano, int Numero) {
        //    using (GTI_Context db = new GTI_Context(_connection)) {
        //        Processogti _reg = new Processogti();
        //        try {
        //            var sql = (from t in db.Processogti where t.Ano == Ano && t.Numero == Numero select t).First();
        //            if (sql != null) {
        //                _reg.Codassunto = sql.Codassunto;
        //                _reg.Dataarquiva = sql.Dataarquiva;
        //                _reg.Datasuspenso = sql.Datasuspenso;
        //                _reg.Datacancel = sql.Datacancel;
        //                _reg.Dataentrada = sql.Dataentrada;
        //            }
        //        } catch (Exception) {

        //        }
        //        if (_reg.Dataentrada == null) {
        //            _reg.Datacancel = DateTime.Now;
        //        }
        //        return _reg;
        //    }
        //}

        public Exception Enviar_Processo(Tramitacao reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Tramitacao t = db.Tramitacao.First(i => i.Ano == reg.Ano && i.Numero == reg.Numero && i.Seq == reg.Seq);

                try {
                    t.Dataenvio = reg.Dataenvio;
                    t.Despacho = reg.Despacho;
                    t.Userid2 = reg.Userid2;
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public Exception Move_Sequencia_Tramite_Acima(int Numero, int Ano, int Seq) {

            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=100 WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=@Seq",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq));
                } catch (Exception ex) {
                    return ex;
                }

                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=@Seq WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=@Seq2",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq),
                        new SqlParameter("@Seq2", Seq - 1));
                } catch (Exception ex) {
                    return ex;
                }

                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=@Seq WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=100",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq - 1));
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public Exception Move_Sequencia_Tramite_Abaixo(int Numero, int Ano, int Seq) {

            using (GTI_Context db = new GTI_Context(_connection)) {
                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=100 WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=@Seq",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq + 1));
                } catch (Exception ex) {
                    return ex;
                }


                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=@Seq2 WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=@Seq",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq),
                        new SqlParameter("@Seq2", Seq + 1));
                } catch (Exception ex) {
                    return ex;
                }

                try {
                    db.Database.ExecuteSqlCommand("UPDATE TRAMITACAOCC SET SEQ=@Seq WHERE ANO=@Ano AND NUMERO=@Numero AND SEQ=100",
                        new SqlParameter("@Ano", Ano),
                        new SqlParameter("@Numero", Numero),
                        new SqlParameter("@Seq", Seq));
                } catch (Exception ex) {
                    return ex;
                }


            }


            return null;
        }

        public Exception Inserir_Local(int Numero, int Ano, int Seq, int CCusto_Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Tramitacaocc> _lista_atual = (from c in db.Tramitacaocc where c.Ano == Ano && c.Numero == Numero orderby c.Seq select c).ToList();
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

                db.Database.ExecuteSqlCommand("DELETE FROM TRAMITACAOCC WHERE ANO=@Ano AND NUMERO=@Numero", new SqlParameter("@Ano", Ano), new SqlParameter("@Numero", Numero));
                try {
                    foreach (Tramitacaocc item in _lista_nova.OrderBy(m => m.Seq)) {
                        db.Database.ExecuteSqlCommand("INSERT TRAMITACAOCC VALUES(@Ano,@Numero,@Seq,@CCusto)",
                            new SqlParameter("@Ano", Ano), new SqlParameter("@Numero", Numero),
                            new SqlParameter("@Seq", item.Seq), new SqlParameter("@CCusto", item.Ccusto));

                    }
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public Exception Remover_Local(int Numero, int Ano, int Seq) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                List<Tramitacaocc> _lista_atual = (from c in db.Tramitacaocc where c.Ano == Ano && c.Numero == Numero orderby c.Seq select c).ToList();
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

                db.Database.ExecuteSqlCommand("DELETE FROM TRAMITACAOCC WHERE ANO=@Ano AND NUMERO=@Numero", new SqlParameter("@Ano", Ano), new SqlParameter("@Numero", Numero));
                int _pos = 1;

                try {
                    foreach (Tramitacaocc item in _lista_nova.OrderBy(m => m.Seq)) {
                        db.Database.ExecuteSqlCommand("INSERT TRAMITACAOCC VALUES(@Ano,@Numero,@Seq,@CCusto)",
                            new SqlParameter("@Ano", Ano), new SqlParameter("@Numero", Numero),
                            new SqlParameter("@Seq", _pos), new SqlParameter("@CCusto", item.Ccusto));
                        _pos++;
                    }
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public Exception Alterar_Obs(int Ano, int Numero, int Seq, string ObsGeral, string ObsInterna) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                Tramitacao _sql = db.Tramitacao.First(t => t.Ano == Ano && t.Numero == Numero && t.Seq == Seq);
                _sql.Obs = ObsGeral;
                _sql.Obsinterna = ObsInterna;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
            }
            return null;
        }

        public bool Tramite_Recebido(int Ano, int Numero, int Seq) {
            bool _valido = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq && a.Datahora != null);
                if (existingReg > 0)
                    _valido = true;
                return _valido;
            }
        }

        public bool Tramite_Enviado(int Ano, int Numero, int Seq) {
            bool _valido = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Tramitacao.Count(a => a.Ano == Ano && a.Numero == Numero && a.Seq == Seq && a.Dataenvio != null);
                if (existingReg > 0)
                    _valido = true;
                return _valido;
            }
        }

        public List<ProcessoStruct> Lista_Processos_Requerente(string PartialName) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processogti
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into cp from c in cp.DefaultIfEmpty()
                           join a in db.Assunto on p.Codassunto equals a.Codigo into ap from a in ap.DefaultIfEmpty()
                           join e in db.Processoend on new { P1 = p.Ano, P2 = p.Numero } equals new { P1 = e.Ano, P2 = e.Numprocesso } into ep from e in ep.DefaultIfEmpty()
                           join l in db.Logradouro on e.Codlogr equals l.Codlogradouro into le from l in le.DefaultIfEmpty()
                           join u in db.Centrocusto on p.Centrocusto equals u.Codigo into pu from u in pu.DefaultIfEmpty()
                           orderby p.Ano, p.Numero
                           select new ProcessoStruct {
                               Ano = p.Ano, Numero = p.Numero, NomeCidadao = c.Nomecidadao, Assunto = a.Nome, DataEntrada = p.Dataentrada, DataCancelado = p.Datacancel,
                               DataReativacao = p.Datareativa, DataArquivado = p.Dataarquiva, DataSuspensao = p.Datasuspenso, Interno = p.Interno, Fisico = p.Fisico, LogradouroNome = l.Endereco,
                               LogradouroNumero = e.Numero, Complemento = p.Complemento, CentroCustoNome = u.Descricao, Inscricao = p.Insc, CodigoCidadao = p.Codcidadao, CodigoAssunto = p.Codassunto,
                               CentroCusto = p.Centrocusto
                           });
                Sql = Sql.Where(c => c.NomeCidadao.Contains(PartialName));
                Sql = Sql.OrderBy(o => o.NomeCidadao).ThenBy(m => m.Ano).ThenBy(n => n.Numero);
                return Sql.ToList();
            }
        }

        public List<ProcessoStruct> Lista_Processos(int Ano, int Numero, string PartialName, string PartialEndereco, int EnderecoNumero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processogti
                           join c in db.Cidadao on p.Codcidadao equals c.Codcidadao into cp from c in cp.DefaultIfEmpty()
                           join a in db.Assunto on p.Codassunto equals a.Codigo into ap from a in ap.DefaultIfEmpty()
                           join e in db.Processoend on new { P1 = p.Ano, P2 = p.Numero } equals new { P1 = e.Ano, P2 = e.Numprocesso } into ep from e in ep.DefaultIfEmpty()
                           join l in db.Logradouro on e.Codlogr equals l.Codlogradouro into le from l in le.DefaultIfEmpty()
                           join u in db.Centrocusto on p.Centrocusto equals u.Codigo into pu from u in pu.DefaultIfEmpty()
                           orderby p.Ano, p.Numero
                           select new ProcessoStruct {
                               Ano = p.Ano, Numero = p.Numero, NomeCidadao = c.Nomecidadao, Assunto = a.Nome, DataEntrada = p.Dataentrada, DataCancelado = p.Datacancel,
                               DataReativacao = p.Datareativa, DataArquivado = p.Dataarquiva, DataSuspensao = p.Datasuspenso, Interno = p.Interno, Fisico = p.Fisico, LogradouroNome = l.Endereco,
                               LogradouroNumero = e.Numero, Complemento = p.Complemento, CentroCustoNome = u.Descricao, Inscricao = p.Insc, CodigoCidadao = p.Codcidadao, CodigoAssunto = p.Codassunto,
                               CentroCusto = p.Centrocusto
                           });
                if (Ano > 0)
                    Sql = Sql.Where(a => a.Ano == Ano & a.Numero == Numero);
                if (!string.IsNullOrEmpty(PartialName))
                    Sql = Sql.Where(c => c.NomeCidadao.Contains(PartialName));
                if (!string.IsNullOrEmpty(PartialEndereco))
                    Sql = Sql.Where(c => c.LogradouroNome.Contains(PartialEndereco));
                if (EnderecoNumero > 0)
                    Sql = Sql.Where(c => c.LogradouroNumero == EnderecoNumero.ToString());


                Sql = Sql.OrderBy(m => m.Ano).ThenBy(n => n.Numero);
                return Sql.ToList();
            }
        }

        //public Local_Tramite Verificar_Processo(short Ano, int Numero) {
        //    Local_Tramite lt = new Local_Tramite() {
        //        Ano = Ano,
        //        Numero = Numero
        //    };

        //    List<Lista_Tramitacao> _listaTramitacao = new List<Lista_Tramitacao>();
        //    Processogti _proc = Retorna_ProcessoGti(Ano, Numero);

        //    if (_proc.Dataarquiva != null) {
        //        lt.Local_Codigo = 0;
        //        lt.Local_Nome = "";
        //        lt.Arquivado = true;
        //        lt.Suspenso = false;
        //        lt.Data_Evento = Convert.ToDateTime(_proc.Dataarquiva);
        //        return lt;
        //    }
        //    if (_proc.Datasuspenso != null) {
        //        lt.Local_Codigo = 0;
        //        lt.Local_Nome = "";
        //        lt.Arquivado = false;
        //        lt.Suspenso = true;
        //        lt.Data_Evento = Convert.ToDateTime(_proc.Datasuspenso);
        //        return lt;
        //    }
        //    if (_proc.Datacancel != null) {
        //        lt.Local_Codigo = 0;
        //        lt.Local_Nome = "";
        //        lt.Arquivado = false;
        //        lt.Suspenso = true;
        //        lt.Data_Evento = Convert.ToDateTime(_proc.Datacancel);
        //        return lt;
        //    }

        //    List<TramiteStruct> ListaTramite = DadosTramite(Ano, Numero, _proc.Codassunto);

        //    foreach (TramiteStruct Reg in ListaTramite) {
        //        Lista_Tramitacao _reg = new Lista_Tramitacao() {
        //            Seq = Reg.Seq,
        //            CentroCusto_Codigo = Reg.CentroCustoCodigo,
        //            CentroCusto_Nome = Reg.CentroCustoNome,
        //            Usuario1 = Reg.Usuario1,
        //            Usuario2 = Reg.Usuario2,
        //            Despacho_Codigo=Reg.DespachoCodigo,
        //            Despacho_Nome=Reg.DespachoNome
        //        };
        //        if (!string.IsNullOrEmpty(Reg.DataEntrada)) {
        //            _reg.Data_Entrada = Convert.ToDateTime(Reg.DataEntrada);
        //        }
        //        if (!string.IsNullOrEmpty(Reg.DataEnvio)) {
        //            _reg.Data_Envio = Convert.ToDateTime(Reg.DataEnvio);
        //        }
        //        _listaTramitacao.Add(_reg);
        //    }

        //    int _rows = _listaTramitacao.Count;

        //    //1º caso, a tabela possui apenas 1 linha, neste caso o processo estará neste local
        //    if (_rows == 1) {
        //        lt.Local_Codigo = _listaTramitacao[0].CentroCusto_Codigo;
        //        lt.Local_Nome = _listaTramitacao[0].CentroCusto_Nome;
        //        lt.Arquivado = false;
        //        lt.Suspenso = false;
        //        if (_listaTramitacao[0].Data_Envio == null)
        //            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
        //        else
        //            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Envio);
        //        return lt;
        //    }

        //    for (int _row = 0; _row < _rows; _row++) {
        //        string _data1 = _listaTramitacao[_row].Data_Entrada == null ? "" : _listaTramitacao[_row].Data_Entrada.ToString();
        //        string _data2 = _listaTramitacao[_row].Data_Envio == null ? "" : _listaTramitacao[_row].Data_Envio.ToString();

        //        if (_row == 0 && _data1 == "") {
        //            //2º caso, o processo possui mais de uma linha mas nenhuma foi tramitada, neste caso o processo estará na 1ª linha
        //            lt.Local_Codigo = _listaTramitacao[0].CentroCusto_Codigo;
        //            lt.Local_Nome = _listaTramitacao[0].CentroCusto_Nome;
        //            lt.Arquivado = false;
        //            lt.Suspenso = false;
        //            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
        //            return lt;
        //        }

        //        if (_data2 == "" && _data1 != "") {
        //            if (_row == _rows - 1) {
        //                //3º caso, o processo foi recebido mas não enviado e esta na última linha, neste caso o processo estará na 1ª linha
        //                lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
        //                lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
        //                lt.Arquivado = false;
        //                lt.Suspenso = false;
        //                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
        //                return lt;
        //            } else {
        //                //4º caso, o processo foi recebido mas não enviado e não esta na última linha
        //                string _data3 = _listaTramitacao[_row + 1].Data_Entrada == null ? "" : _listaTramitacao[_row + 1].Data_Entrada.ToString();
        //                //Verificamos se houve entrada na proxima linha
        //                if (_data3 == "") {
        //                    //Se não houve entrada na próxima linha, então o processo estará nesta linha
        //                    lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
        //                    lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
        //                    lt.Arquivado = false;
        //                    lt.Suspenso = false;
        //                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
        //                    return lt;
        //                } else {
        //                    //Iremos verificar as outras linhas até aonde não houver mais recebimento
        //                    for (int t = _row; t < _rows; t++) {
        //                        _data3 = _listaTramitacao[t].Data_Entrada == null ? "" : _listaTramitacao[t].Data_Entrada.ToString();
        //                        if (_row + 1 == _rows) {
        //                            //Se a última linha estiver recebida, então o processo estará nesta linha
        //                            lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
        //                            lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
        //                            lt.Arquivado = false;
        //                            lt.Suspenso = false;
        //                            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
        //                            return lt;
        //                        } else {
        //                            if (t == _rows - 1) {
        //                                lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
        //                                lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
        //                                lt.Arquivado = false;
        //                                lt.Suspenso = false;
        //                                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
        //                                return lt;
        //                            } else {
        //                                string _data4 = _listaTramitacao[t + 1].Data_Entrada == null ? "" : _listaTramitacao[t + 1].Data_Entrada.ToString();
        //                                if (_data4 == "") {
        //                                    //Se a linha seguinte não estiver recebida, então o processo estará nesta linha
        //                                    lt.Local_Codigo = _listaTramitacao[t + 1].CentroCusto_Codigo;
        //                                    lt.Local_Nome = _listaTramitacao[t + 1].CentroCusto_Nome;
        //                                    lt.Arquivado = false;
        //                                    lt.Suspenso = false;
        //                                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
        //                                    return lt;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        } else {
        //            if (_data1 == "" && _data2 == "") {
        //                lt.Local_Codigo = _listaTramitacao[_row - 1].CentroCusto_Codigo;
        //                lt.Local_Nome = _listaTramitacao[_row - 1].CentroCusto_Nome;
        //                lt.Arquivado = false;
        //                lt.Suspenso = false;
        //                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row - 1].Data_Entrada);
        //                return lt;
        //            } else {
        //                if (_data1 != "" && _data2 != "") {
        //                    if (_row == _rows - 1) {
        //                        lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
        //                        lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
        //                        lt.Arquivado = false;
        //                        lt.Suspenso = false;
        //                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
        //                        return lt;
        //                    } else {
        //                        for (int t = _rows-1; t >-1; t--) {
        //                            if (_listaTramitacao[t].Data_Envio != null) {
        //                                lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
        //                                lt.Local_Nome = _listaTramitacao[t ].CentroCusto_Nome;
        //                                lt.Arquivado = false;
        //                                lt.Suspenso = false;
        //                                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t ].Data_Entrada);
        //                                return lt;
        //                            } else {
        //                                if (_listaTramitacao[t].Despacho_Codigo >0) {
        //                                    if (_listaTramitacao[t].Despacho_Nome.Contains("ARQUIVADO")){
        //                                        lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
        //                                        lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
        //                                        lt.Arquivado = true;
        //                                        lt.Suspenso = false;
        //                                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
        //                                        return lt;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    }

        //    return lt;

        //}

        public List<Processogti> Lista_Processos_CCusto(int Local) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                var Sql = (from p in db.Processogti
                           join t in db.Tramitacao on new { p1 = p.Ano, p2 = p.Numero } equals new { p1 = t.Ano, p2 = t.Numero } into tp from t in tp.DefaultIfEmpty()
                           where p.Dataarquiva == null && p.Datacancel == null && t.Ccusto == Local orderby new { p.Ano, p.Numero } select p).ToList();
                return Sql;
            }
        }

        public Exception Incluir_Processo_Web(Processo_web reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[9];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@data_geracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = reg.Data_geracao };
                Parametros[2] = new SqlParameter { ParameterName = "@centro_custo_codigo", SqlDbType = SqlDbType.Int, SqlValue = reg.Centro_custo_codigo };
                Parametros[3] = new SqlParameter { ParameterName = "@centro_custo_nome", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Centro_custo_nome };
                Parametros[4] = new SqlParameter { ParameterName = "@Interno", SqlDbType = SqlDbType.Bit, SqlValue = reg.Interno };
                Parametros[5] = new SqlParameter { ParameterName = "@user_id", SqlDbType = SqlDbType.Int, SqlValue = reg.User_id };
                Parametros[6] = new SqlParameter { ParameterName = "@user_pref", SqlDbType = SqlDbType.Bit, SqlValue = reg.User_pref };
                Parametros[7] = new SqlParameter { ParameterName = "@fisico", SqlDbType = SqlDbType.Bit, SqlValue = reg.Fisico };
                Parametros[8] = new SqlParameter { ParameterName = "@assunto_codigo", SqlDbType = SqlDbType.Int, SqlValue = reg.Assunto_codigo };

                db.Database.ExecuteSqlCommand("INSERT INTO processo_web(guid,data_geracao,centro_custo_codigo,centro_custo_nome,interno,user_id,user_pref,fisico,assunto_codigo) " +
                    "VALUES(@guid,@data_geracao,@centro_custo_codigo,@centro_custo_nome,@interno,@user_id,@user_pref,@fisico,@assunto_codigo)", Parametros);

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Processo_web Retorna_Processo_Web(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                var Sql = (from p in db.Processo_Web where p.Guid == guid select p).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_CentroCusto(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string Sql = (from c in db.Centrocusto where c.Codigo == Codigo select c.Descricao).FirstOrDefault();
                return Sql;
            }
        }

        public List<ProcessoDocStruct> Lista_Processo_Documento(int ano, int numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = from p in db.Processodoc
                          join d in db.Documento on p.Coddoc equals d.Codigo into pd from d in pd.DefaultIfEmpty()
                          where p.Ano == ano && p.Numero == numero
                          orderby d.Nome select new { Codigo = p.Coddoc, Nome = d.Nome, Data = p.Data };

                List<ProcessoDocStruct> Lista = new List<ProcessoDocStruct>();

                foreach (var item in Sql) {
                    ProcessoDocStruct reg = new ProcessoDocStruct() {
                        CodigoDocumento = item.Codigo,
                        NomeDocumento = item.Nome
                    };
                    if (item.Data != null)
                        reg.DataEntrega = item.Data;
                    Lista.Add(reg);
                }

                return Lista;
            }
        }

        public List<ProcessoEndStruct> Lista_Processo_Endereco(short ano, int numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = from p in db.Processoend
                          join d in db.Logradouro on p.Codlogr equals d.Codlogradouro into pd from d in pd.DefaultIfEmpty()
                          where p.Ano == ano && p.Numprocesso == numero
                          orderby d.Endereco_resumido select new { Codigo = p.Codlogr, Nome = d.Endereco_resumido, Numero = p.Numero };

                List<ProcessoEndStruct> Lista = new List<ProcessoEndStruct>();

                foreach (var item in Sql) {
                    ProcessoEndStruct reg = new ProcessoEndStruct() {
                        CodigoLogradouro = item.Codigo,
                        NomeLogradouro = item.Nome,
                        Numero = item.Numero
                    };
                    Lista.Add(reg);
                }

                return Lista;
            }
        }

        public  Secretaria Retorna_Secretaria(int CodigoCC) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.Secretaria where c.Codigocc == CodigoCC select c).FirstOrDefault();
            }
        }

        public  List<Secretaria> Lista_Secretaria() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.Secretaria select c).ToList();
            }
        }

        public  Tuple<short, string> Retorna_Vinculo_Top_CentroCusto(short centro_custo) {
            short _codigo = 0;
            string _nome = "";
            List<Centrocusto> listaCC = Lista_Local(false, false);
            short _vinculo = 0;

            foreach (Centrocusto item in listaCC) {
                if (item.Codigo == centro_custo) {
                    _codigo = item.Codigo;
                    _nome = item.Descricao;
                    _vinculo = item.Vinculo;
                    break;
                }
            }
            centro_custo = _vinculo;
            if (_vinculo > 0) {
                bool loop = true;
                while (loop) {
                    if (_vinculo == 0) {
                        loop = false;
                    } else {
                        foreach (Centrocusto item in listaCC) {
                            if (item.Codigo == centro_custo) {
                                _codigo = item.Codigo;
                                _nome = item.Descricao;
                                _vinculo = item.Vinculo;
                                centro_custo = _vinculo;
                                break;
                            }
                        }
                    }
                }
            }


            return new Tuple<short, string>(_codigo, _nome);
        }

        public List<ProcessoAnoNumero> Lista_Processos_Atraso(DateTime Data_Inicio, DateTime Data_Final) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from p in db.Processogti
                        where p.Ano >= 2020  && (p.Dataentrada >= Data_Inicio) &&  p.Dataentrada <= Data_Final && p.Interno == false && p.Dataarquiva == null && p.Datacancel==null 
                        orderby p.Ano, p.Numero
                        select new ProcessoAnoNumero{Ano= p.Ano,Numero  =p.Numero }).ToList();
            }
        }

        public  ProcessoStruct Retorna_ProcessoGti(short Ano, int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                ProcessoStruct _reg = new ProcessoStruct();
                try {
                    var sql = (from t in db.Processogti
                               join a in db.Assunto on t.Codassunto equals a.Codigo into ap from a in ap.DefaultIfEmpty()
                               where t.Ano == Ano && t.Numero == Numero select new {
                                   Assunto_Codigo = t.Codassunto, Data_Arquiva = t.Dataarquiva, Data_Suspenso = t.Datasuspenso,
                                   Data_Cancel = t.Datacancel, Data_Entrada = t.Dataentrada, Assunto_Nome = a.Nome
                               }).First();
                    if (sql != null) {
                        _reg.CodigoAssunto = sql.Assunto_Codigo;
                        _reg.DataArquivado = sql.Data_Arquiva;
                        _reg.DataSuspensao = sql.Data_Suspenso;
                        _reg.DataCancelado = sql.Data_Cancel;
                        _reg.DataEntrada = sql.Data_Entrada;
                        _reg.Assunto = sql.Assunto_Nome;
                    }
                } catch {
                }
                if (_reg.DataEntrada == null) {
                    _reg.DataCancelado = DateTime.Now;
                }
                return _reg;
            }
        }

        public Local_Tramite Verificar_Processo(short Ano, int Numero) {
            Local_Tramite lt = new Local_Tramite() {
                Ano = Ano,
                Numero = Numero
            };

            List<Lista_Tramitacao> _listaTramitacao = new List<Lista_Tramitacao>();
            ProcessoStruct _proc = Retorna_ProcessoGti(Ano, Numero);
            lt.Data_Entrada = Convert.ToDateTime(_proc.DataEntrada);
            lt.Assunto_Nome = _proc.Assunto;

            if (_proc.DataArquivado != null) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = true;
                lt.Suspenso = false;
                lt.Data_Evento = Convert.ToDateTime(_proc.DataArquivado);
                return lt;
            }
            if (_proc.DataSuspensao != null) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = false;
                lt.Suspenso = true;
                lt.Data_Evento = Convert.ToDateTime(_proc.DataSuspensao);
                return lt;
            }
            if (_proc.DataCancelado != null) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = false;
                lt.Suspenso = true;
                lt.Data_Evento = Convert.ToDateTime(_proc.DataCancelado);
                return lt;
            }

            List<TramiteStruct> ListaTramite = DadosTramite(Ano, Numero, (int)_proc.CodigoAssunto);

            DateTime _dataEntrada, _dataSaida;
            TimeSpan ts;
            foreach (TramiteStruct Reg in ListaTramite) {
                Lista_Tramitacao _reg = new Lista_Tramitacao() {
                    Seq = Reg.Seq,
                    CentroCusto_Codigo = Reg.CentroCustoCodigo,
                    CentroCusto_Nome = Reg.CentroCustoNome,
                    Usuario1 = Reg.Usuario1,
                    Usuario2 = Reg.Usuario2,
                    Despacho_Codigo = Reg.DespachoCodigo,
                    Despacho_Nome = Reg.DespachoNome
                };
                if (!string.IsNullOrEmpty(Reg.DataEntrada)) {
                    _reg.Data_Entrada = Convert.ToDateTime(Reg.DataEntrada);
                }
                if (!string.IsNullOrEmpty(Reg.DataEnvio)) {
                    _reg.Data_Envio = Convert.ToDateTime(Reg.DataEnvio);
                }
                _listaTramitacao.Add(_reg);
            }

            int _rows = _listaTramitacao.Count;

            //1º caso, a tabela possui apenas 1 linha, neste caso o processo estará neste local
            if (_rows == 1) {
                lt.Local_Codigo = _listaTramitacao[0].CentroCusto_Codigo;
                lt.Local_Nome = _listaTramitacao[0].CentroCusto_Nome;
                lt.Arquivado = false;
                lt.Suspenso = false;
                if (_listaTramitacao[0].Data_Envio == null) {
                    if (_listaTramitacao[0].Data_Entrada == null) {
                        lt.Data_Evento = Convert.ToDateTime(_proc.DataEntrada);
                        _dataEntrada = lt.Data_Evento;
                    } else {
                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
                    }
                    _dataSaida = DateTime.Now;
                } else {
                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Envio);
                    _dataEntrada = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
                    _dataSaida = Convert.ToDateTime(_listaTramitacao[0].Data_Envio);
                }
                ts = (_dataSaida - _dataEntrada);
                lt.Dias = (int)ts.TotalDays;
                return lt;
            }

            for (int _row = 0; _row < _rows; _row++) {
                string _data1 = _listaTramitacao[_row].Data_Entrada == null ? "" : _listaTramitacao[_row].Data_Entrada.ToString();
                string _data2 = _listaTramitacao[_row].Data_Envio == null ? "" : _listaTramitacao[_row].Data_Envio.ToString();

                if (_row == 0 && _data1 == "") {
                    //2º caso, o processo possui mais de uma linha mas nenhuma foi tramitada, neste caso o processo estará na 1ª linha
                    lt.Local_Codigo = _listaTramitacao[0].CentroCusto_Codigo;
                    lt.Local_Nome = _listaTramitacao[0].CentroCusto_Nome;
                    lt.Arquivado = false;
                    lt.Suspenso = false;
                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);

                    _dataEntrada = Convert.ToDateTime(_proc.DataEntrada);
                    _dataSaida = DateTime.Now;
                    ts = (_dataSaida - _dataEntrada);
                    lt.Dias = (int)ts.TotalDays;

                    return lt;
                }

                if (_data2 == "" && _data1 != "") {
                    if (_row == _rows - 1) {
                        //3º caso, o processo foi recebido mas não enviado e esta na última linha, neste caso o processo estará na 1ª linha
                        lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
                        lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
                        lt.Arquivado = false;
                        lt.Suspenso = false;
                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                        _dataSaida = DateTime.Now;
                        ts = (_dataSaida - _dataEntrada);
                        lt.Dias = (int)ts.TotalDays;

                        return lt;
                    } else {
                        //4º caso, o processo foi recebido mas não enviado e não esta na última linha
                        string _data3 = _listaTramitacao[_row + 1].Data_Entrada == null ? "" : _listaTramitacao[_row + 1].Data_Entrada.ToString();
                        //Verificamos se houve entrada na proxima linha
                        if (_data3 == "") {
                            //Se não houve entrada na próxima linha, então o processo estará nesta linha
                            lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
                            lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
                            lt.Arquivado = false;
                            lt.Suspenso = false;
                            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                            _dataEntrada = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                            _dataSaida = DateTime.Now;
                            ts = (_dataSaida - _dataEntrada);
                            lt.Dias = (int)ts.TotalDays;

                            return lt;
                        } else {
                            //Iremos verificar as outras linhas até aonde não houver mais recebimento
                            for (int t = _row; t < _rows; t++) {
                                _data3 = _listaTramitacao[t].Data_Entrada == null ? "" : _listaTramitacao[t].Data_Entrada.ToString();
                                if (_row + 1 == _rows) {
                                    //Se a última linha estiver recebida, então o processo estará nesta linha
                                    lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
                                    lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
                                    lt.Arquivado = false;
                                    lt.Suspenso = false;
                                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                    if (_listaTramitacao[t].Data_Envio == null) {
                                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                        _dataSaida = DateTime.Now;
                                    } else {
                                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                        _dataSaida = Convert.ToDateTime(_listaTramitacao[t].Data_Envio);
                                    }
                                    ts = (_dataSaida - _dataEntrada);
                                    lt.Dias = (int)ts.TotalDays;

                                    return lt;
                                } else {
                                    if (t == _rows - 1) {
                                        lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
                                        lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
                                        lt.Arquivado = false;
                                        lt.Suspenso = false;
                                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                        if (_listaTramitacao[t].Data_Envio == null) {
                                            _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                            _dataSaida = DateTime.Now;
                                        } else {
                                            _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                            _dataSaida = Convert.ToDateTime(_listaTramitacao[t].Data_Envio);
                                        }
                                        ts = (_dataSaida - _dataEntrada);
                                        lt.Dias = (int)ts.TotalDays;

                                        return lt;
                                    } else {
                                        string _data4 = _listaTramitacao[t + 1].Data_Entrada == null ? "" : _listaTramitacao[t + 1].Data_Entrada.ToString();
                                        if (_data4 == "") {
                                            //Se a linha seguinte não estiver recebida, então o processo estará nesta linha
                                            lt.Local_Codigo = _listaTramitacao[t + 1].CentroCusto_Codigo;
                                            lt.Local_Nome = _listaTramitacao[t + 1].CentroCusto_Nome;
                                            lt.Arquivado = false;
                                            lt.Suspenso = false;
                                            lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                            if (_listaTramitacao[t].Data_Envio == null) {
                                                _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                                _dataSaida = DateTime.Now;
                                            } else {
                                                _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                                _dataSaida = Convert.ToDateTime(_listaTramitacao[t].Data_Envio);
                                            }
                                            ts = (_dataSaida - _dataEntrada);
                                            lt.Dias = (int)ts.TotalDays;

                                            return lt;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } else {
                    if (_data1 == "" && _data2 == "") {
                        lt.Local_Codigo = _listaTramitacao[_row - 1].CentroCusto_Codigo;
                        lt.Local_Nome = _listaTramitacao[_row - 1].CentroCusto_Nome;
                        lt.Arquivado = false;
                        lt.Suspenso = false;
                        lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row - 1].Data_Entrada);
                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[_row - 1].Data_Entrada);
                        _dataSaida = DateTime.Now;
                        ts = (_dataSaida - _dataEntrada);
                        lt.Dias = (int)ts.TotalDays;

                        return lt;
                    } else {
                        if (_data1 != "" && _data2 != "") {
                            if (_row == _rows - 1) {
                                lt.Local_Codigo = _listaTramitacao[_row].CentroCusto_Codigo;
                                lt.Local_Nome = _listaTramitacao[_row].CentroCusto_Nome;
                                lt.Arquivado = false;
                                lt.Suspenso = false;
                                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                                _dataEntrada = Convert.ToDateTime(_listaTramitacao[_row].Data_Entrada);
                                _dataSaida = Convert.ToDateTime(_listaTramitacao[_row].Data_Envio);
                                ts = (_dataSaida - _dataEntrada);
                                lt.Dias = (int)ts.TotalDays;

                                return lt;
                            } else {
                                for (int t = _rows - 1; t > -1; t--) {
                                    if (_listaTramitacao[t].Data_Envio != null) {

                                        lt.Local_Codigo = _listaTramitacao[t + 1].CentroCusto_Codigo;
                                        lt.Local_Nome = _listaTramitacao[t + 1].CentroCusto_Nome;
                                        lt.Arquivado = false;
                                        lt.Suspenso = false;
                                        lt.Data_Evento = DateTime.Now;
                                        _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Envio);
                                        _dataSaida = DateTime.Now;
                                        ts = (_dataSaida - _dataEntrada);
                                        lt.Dias = (int)ts.TotalDays;


                                        return lt;
                                    } else {
                                        if (_listaTramitacao[t].Despacho_Codigo > 0) {
                                            if (_listaTramitacao[t].Despacho_Nome.Contains("ARQUIVADO")) {
                                                lt.Local_Codigo = _listaTramitacao[t].CentroCusto_Codigo;
                                                lt.Local_Nome = _listaTramitacao[t].CentroCusto_Nome;
                                                lt.Arquivado = true;
                                                lt.Suspenso = false;
                                                lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                                if (_listaTramitacao[t].Data_Envio == null) {
                                                    _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                                    _dataSaida = DateTime.Now;
                                                } else {
                                                    _dataEntrada = Convert.ToDateTime(_listaTramitacao[t].Data_Entrada);
                                                    _dataSaida = Convert.ToDateTime(_listaTramitacao[t].Data_Envio);
                                                }
                                                ts = (_dataSaida - _dataEntrada);
                                                lt.Dias = (int)ts.TotalDays;

                                                return lt;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lt;
        }

        public short Retorna_Seq_Processo_Secretaria_Remessa(short Codigo) {
            DateTime _data = DateTime.Now.Date;
            using (GTI_Context db = new GTI_Context(_connection)) {
                short maxSeq = 1;
                var ret = (from c in db.Secretaria_Processo_Remessa where c.Codigo==Codigo && c.Data == _data orderby c.Seq descending select c).FirstOrDefault();
                if (ret != null) {
                     maxSeq = Convert.ToInt16(ret.Seq + 1);
                }
                return maxSeq;
            }
        }

        public Exception Incluir_Secretaria_Processo_Remessa(Secretaria_processo_remessa reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Secretaria_Processo_Remessa.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

    }
}
