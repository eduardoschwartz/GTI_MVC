using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


namespace GTI_Dal.Classes {
    public class Parcelamento_Data {
        private readonly string _connection;

        public Parcelamento_Data(string sConnection) {
            _connection = sConnection;
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(int Codigo, char Tipo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 600;
                List<SpParcelamentoOrigem> ListaOrigem = new List<SpParcelamentoOrigem>();

                Tributario_Data tributarioRepository = new Tributario_Data(_connection);
                List<SpExtrato_Parcelamento> _listaTributo = tributarioRepository.Lista_Extrato_Tributo_Parcelamento(Codigo);
                List<SpExtrato_Parcelamento> _extrato = tributarioRepository.Lista_Extrato_Parcela_Parcelamento(_listaTributo);

                int _pos = 1;
                DateTime _dataNow = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                foreach (SpExtrato_Parcelamento _row in _extrato) {
                    if (_row.Datavencimento >= _dataNow) {
                        if (_row.Codlancamento != 65 && _row.Codlancamento != 62 && _row.Codlancamento != 16 && _row.Codlancamento != 38 && _row.Codlancamento != 76) {
                            goto NextReg;
                        }
                    }

                    if(_row.Numparcela==0)
                        goto NextReg;

                    SpParcelamentoOrigem _reg = new SpParcelamentoOrigem() {
                        Idx=_pos,
                        Exercicio=_row.Anoexercicio,
                        Lancamento=_row.Codlancamento,
                        Nome_lancamento=_row.Desclancamento,
                        Sequencia=_row.Seqlancamento,
                        Parcela=_row.Numparcela,
                        Complemento=_row.Codcomplemento,
                        Data_vencimento=_row.Datavencimento,
                        Valor_principal=_row.Valortributo,
                        Valor_juros=_row.Valorjuros,
                        Valor_multa=_row.Valormulta,
                        Valor_correcao=_row.Valorcorrecao,
                        Valor_total=_row.Valortotal,
                        Ajuizado=_row.Dataajuiza!=null?"S":"N",
                        Qtde_parcelamento=Qtde_Parcelamento_Efetuados(Codigo,_row.Anoexercicio,_row.Codlancamento,_row.Seqlancamento,_row.Numparcela)
                    };

                    //DECRETO ANISITIA MULTA E JUROS PARA PARCELAS 4,5, E 6 DE 2020
                    if (_row.Anoexercicio == 2020 && (_row.Datavencimento.Month == 4 || _row.Datavencimento.Month == 5 || _row.Datavencimento.Month == 6)) {
                        _reg.Valor_juros = 0;
                        _reg.Valor_multa = 0;
                        _reg.Valor_total = _reg.Valor_principal + _reg.Valor_correcao;
                    }

                    if (Tipo == 'F')  //empresa física 5 % por parcelamento
                        _reg.Perc_penalidade = _reg.Qtde_parcelamento * 5;
                     else 
                        _reg.Perc_penalidade = _reg.Qtde_parcelamento * 10;

                    _reg.Valor_penalidade = _reg.Valor_total * (_reg.Perc_penalidade / 100);

                    ListaOrigem.Add(_reg);
                    _pos++;
                NextReg:;
                }
                
                return ListaOrigem;
            }
        }

        public Exception Incluir_Parcelamento_Web_Master(Parcelamento_web_master Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[16];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Requerente_Codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Requerente_Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@user_id", SqlDbType = SqlDbType.Int, SqlValue = Reg.User_id };
                Parametros[3] = new SqlParameter { ParameterName = "@Requerente_Nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Nome };
                Parametros[4] = new SqlParameter { ParameterName = "@Requerente_CpfCnpj", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_CpfCnpj };
                Parametros[5] = new SqlParameter { ParameterName = "@Requerente_Bairro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Bairro };
                Parametros[6] = new SqlParameter { ParameterName = "@Requerente_Cidade", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Cidade };
                Parametros[7] = new SqlParameter { ParameterName = "@Requerente_Uf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Uf };
                Parametros[8] = new SqlParameter { ParameterName = "@Requerente_Logradouro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Logradouro };
                Parametros[9] = new SqlParameter { ParameterName = "@Requerente_Numero", SqlDbType = SqlDbType.Int, SqlValue = Reg.Requerente_Numero };
                if(Reg.Requerente_Complemento==null)
                    Parametros[10] = new SqlParameter { ParameterName = "@Requerente_Complemento", SqlValue = DBNull.Value };
                else
                    Parametros[10] = new SqlParameter { ParameterName = "@Requerente_Complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Complemento };
                Parametros[11] = new SqlParameter { ParameterName = "@Requerente_Cep", SqlDbType = SqlDbType.Int, SqlValue = Reg.Requerente_Cep };
                if(Reg.Requerente_Telefone==null)
                    Parametros[12] = new SqlParameter { ParameterName = "@Requerente_Telefone", SqlValue = DBNull.Value};
                else
                    Parametros[12] = new SqlParameter { ParameterName = "@Requerente_Telefone", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Telefone };
                Parametros[13] = new SqlParameter { ParameterName = "@Requerente_email", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Email };
                Parametros[14] = new SqlParameter { ParameterName = "@Data_Geracao", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
                Parametros[15] = new SqlParameter { ParameterName = "@Data_Vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_master(guid,Requerente_Codigo,user_id,Requerente_Nome,Requerente_CpfCnpj,Requerente_Bairro,Requerente_Cidade,Requerente_Uf,Requerente_Logradouro," +
                    "Requerente_Numero,Requerente_Complemento,Requerente_Cep,Requerente_Telefone,Requerente_Email,Data_Geracao,Data_Vencimento) VALUES(@guid,@Requerente_Codigo,@user_id,@Requerente_Nome,@Requerente_CpfCnpj,@Requerente_Bairro,@Requerente_Cidade," +
                    "@Requerente_Uf,@Requerente_Logradouro,@Requerente_Numero,@Requerente_Complemento,@Requerente_Cep,@Requerente_Telefone,@Requerente_Email,@Data_Geracao,@Data_Vencimento)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Parcelamento_Web_Master(string _guid) {
            bool bRet = false;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var existingReg = db.Parcelamento_Web_Master.Count(a => a.Guid == _guid);
                if (existingReg != 0) {
                    bRet = true;
                }
            }
            return bRet;
        }

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(Parcelamento_web_lista_codigo Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[6];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@id", SqlDbType = SqlDbType.Int, SqlValue = Reg.Id };
                Parametros[2] = new SqlParameter { ParameterName = "@grupo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Grupo };
                Parametros[3] = new SqlParameter { ParameterName = "@texto", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Texto };
                Parametros[4] = new SqlParameter { ParameterName = "@valor", SqlDbType = SqlDbType.Int, SqlValue = Reg.Valor };
                Parametros[5] = new SqlParameter { ParameterName = "@selected", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Selected };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_lista_codigo(guid,id,grupo,texto,valor,selected) " +
                                              "VALUES(@guid,@id,@grupo,@texto,@valor,@selected)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Lista_Codigo where t.Guid == guid orderby t.Id select t).ToList();
                List<Parcelamento_web_lista_codigo> Lista = new List<Parcelamento_web_lista_codigo>();
                foreach (var item in reg) {
                    Parcelamento_web_lista_codigo Linha = new Parcelamento_web_lista_codigo {
                        Guid = guid,
                        Id = item.Id,
                        Grupo=item.Grupo,
                        Texto=item.Texto,
                        Valor=item.Valor,
                        Selected=item.Selected
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        private short Qtde_Parcelamento_Efetuados(int Codigo,short Ano,short Lancamento,short Sequencia,short Parcela) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (short) (from o in db.Origemreparc
                           join p in db.Processoreparc on o.Numprocesso equals p.Numprocesso into op from p in op.DefaultIfEmpty()
                           where o.Codreduzido==Codigo && o.Anoexercicio==Ano && o.Codlancamento==Lancamento && o.Numsequencia==Sequencia && o.Numparcela==Parcela select o.Numprocesso).Distinct().Count();

            }
        }

        public Exception Incluir_Parcelamento_Web_Origem(Parcelamento_web_origem Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[18];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@idx", SqlDbType = SqlDbType.Int, SqlValue = Reg.Idx };
                Parametros[2] = new SqlParameter { ParameterName = "@ano", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Ano};
                Parametros[3] = new SqlParameter { ParameterName = "@lancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Lancamento };
                Parametros[4] = new SqlParameter { ParameterName = "@sequencia", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Sequencia };
                Parametros[5] = new SqlParameter { ParameterName = "@parcela", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Parcela };
                Parametros[6] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Complemento };
                Parametros[7] = new SqlParameter { ParameterName = "@data_vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                Parametros[8] = new SqlParameter { ParameterName = "@valor_tributo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Tributo };
                Parametros[9] = new SqlParameter { ParameterName = "@valor_multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Multa };
                Parametros[10] = new SqlParameter { ParameterName = "@valor_juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Juros };
                Parametros[11] = new SqlParameter { ParameterName = "@valor_correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Correcao };
                Parametros[12] = new SqlParameter { ParameterName = "@valor_total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Total };
                Parametros[13] = new SqlParameter { ParameterName = "@qtde_parcelamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Qtde_Parcelamento };
                Parametros[14] = new SqlParameter { ParameterName = "@perc_penalidade", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Perc_Penalidade };
                Parametros[15] = new SqlParameter { ParameterName = "@valor_penalidade", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Penalidade };
                Parametros[16] = new SqlParameter { ParameterName = "@lancamento_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Lancamento_Nome };
                Parametros[17] = new SqlParameter { ParameterName = "@ajuizado", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ajuizado };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_origem(guid,idx,ano,lancamento,sequencia,parcela,complemento,data_vencimento,valor_tributo,valor_multa,valor_juros,valor_correcao," +
                    "valor_total,qtde_parcelamento,perc_penalidade,valor_penalidade,lancamento_nome,ajuizado) VALUES(@guid,@idx,@ano,@lancamento,@sequencia,@parcela,@complemento,@data_vencimento,@valor_tributo," +
                    "@valor_multa,@valor_juros,@valor_correcao,@valor_total,@qtde_parcelamento,@perc_penalidade,@valor_penalidade,@lancamento_nome,@ajuizado)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Parcelamento_web_master Retorna_Parcelamento_Web_Master(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from p in db.Parcelamento_Web_Master where p.Guid == guid select p).FirstOrDefault();
            }
        }

        public Exception Atualizar_Codigo_Master(Parcelamento_web_master reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[15];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Contribuinte_Codigo", SqlDbType = SqlDbType.Int, SqlValue = reg.Contribuinte_Codigo };
                Parametros[2] = new SqlParameter { ParameterName = "@contribuinte_nome", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_nome };
                Parametros[3] = new SqlParameter { ParameterName = "@contribuinte_cpfcnpj", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_cpfcnpj };
                Parametros[4] = new SqlParameter { ParameterName = "@contribuinte_endereco", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_endereco };
                Parametros[5] = new SqlParameter { ParameterName = "@contribuinte_bairro", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_bairro };
                Parametros[6] = new SqlParameter { ParameterName = "@contribuinte_cep", SqlDbType = SqlDbType.Int, SqlValue = reg.Contribuinte_cep };
                Parametros[7] = new SqlParameter { ParameterName = "@contribuinte_cidade", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_cidade };
                Parametros[8] = new SqlParameter { ParameterName = "@contribuinte_uf", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_uf };
                Parametros[9] = new SqlParameter { ParameterName = "@contribuinte_tipo", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Contribuinte_tipo };
                Parametros[10] = new SqlParameter { ParameterName = "@Plano_Nome", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Plano_Nome };
                Parametros[11] = new SqlParameter { ParameterName = "@Plano_Codigo", SqlDbType = SqlDbType.Int, SqlValue = reg.Plano_Codigo };
                Parametros[12] = new SqlParameter { ParameterName = "@Qtde_Maxima_Parcela", SqlDbType = SqlDbType.Int, SqlValue = reg.Qtde_Maxima_Parcela };
                Parametros[13] = new SqlParameter { ParameterName = "@Perc_Desconto", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Perc_Desconto };
                Parametros[14] = new SqlParameter { ParameterName = "@Valor_minimo", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Valor_minimo };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE parcelamento_web_master set Contribuinte_Codigo=@Contribuinte_Codigo,contribuinte_nome=@contribuinte_nome,contribuinte_cpfcnpj=@contribuinte_cpfcnpj,contribuinte_endereco=@contribuinte_endereco," +
                        "contribuinte_bairro=@contribuinte_bairro,contribuinte_cep=@contribuinte_cep,contribuinte_cidade=@contribuinte_cidade,contribuinte_uf=@contribuinte_uf,Contribuinte_tipo=@Contribuinte_tipo,Plano_Nome=@Plano_Nome," +
                        "Plano_Codigo=@Plano_Codigo,Qtde_Maxima_Parcela=@Qtde_Maxima_Parcela,Perc_Desconto=@Perc_Desconto,Valor_minimo=@Valor_minimo WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Atualizar_Totais_Master(Parcelamento_web_master reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[15];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Soma_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Principal };
                Parametros[2] = new SqlParameter { ParameterName = "@Soma_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Juros };
                Parametros[3] = new SqlParameter { ParameterName = "@Soma_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Multa };
                Parametros[4] = new SqlParameter { ParameterName = "@Soma_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Correcao };
                Parametros[5] = new SqlParameter { ParameterName = "@Soma_Total", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Total };
                Parametros[6] = new SqlParameter { ParameterName = "@Soma_Entrada", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Soma_Entrada };
                Parametros[7] = new SqlParameter { ParameterName = "@Perc_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Perc_Principal };
                Parametros[8] = new SqlParameter { ParameterName = "@Perc_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Perc_Juros };
                Parametros[9] = new SqlParameter { ParameterName = "@Perc_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Perc_Multa };
                Parametros[10] = new SqlParameter { ParameterName = "@Perc_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Perc_Correcao };
                Parametros[11] = new SqlParameter { ParameterName = "@Valor_add_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Valor_add_Principal };
                Parametros[12] = new SqlParameter { ParameterName = "@Valor_add_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Valor_add_Juros };
                Parametros[13] = new SqlParameter { ParameterName = "@Valor_add_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Valor_add_Multa };
                Parametros[14] = new SqlParameter { ParameterName = "@Valor_add_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Valor_add_Correcao };

                try {
                    db.Database.ExecuteSqlCommand("UPDATE parcelamento_web_master set Soma_Principal=@Soma_Principal,Soma_Juros=@Soma_Juros,Soma_Multa=@Soma_Multa,Soma_Correcao=@Soma_Correcao," +
                        "Soma_Total=@Soma_Total,Soma_Entrada=@Soma_Entrada,Perc_Principal=@Perc_Principal,Perc_Juros=@Perc_Juros,Perc_Multa=@Perc_Multa,Perc_Correcao=@Perc_Correcao,Valor_add_Principal=@Valor_add_Principal," +
                        "Valor_add_Juros=@Valor_add_Juros,Valor_add_Multa=@Valor_add_Multa,Valor_add_Correcao=@Valor_add_Correcao WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Origem where t.Guid == guid orderby t.Idx select t).ToList();
                List<SpParcelamentoOrigem> Lista = new List<SpParcelamentoOrigem>();
                foreach (var item in reg) {
                    SpParcelamentoOrigem Linha = new SpParcelamentoOrigem {
                        Idx=item.Idx,
                        Exercicio=item.Ano,
                        Lancamento=item.Lancamento,
                        Nome_lancamento=item.Lancamento_Nome,
                        Sequencia=item.Sequencia,
                        Parcela=item.Parcela,
                        Complemento=item.Complemento,
                        Data_vencimento=item.Data_Vencimento,
                        Ajuizado=item.Ajuizado,
                        Perc_penalidade=item.Perc_Penalidade,
                        Qtde_parcelamento=item.Qtde_Parcelamento,
                        Valor_principal=item.Valor_Tributo,
                        Valor_juros=item.Valor_Juros,
                        Valor_multa=item.Valor_Multa,
                        Valor_correcao=item.Valor_Correcao,
                        Valor_total=item.Valor_Total,
                        Valor_penalidade=item.Valor_Penalidade
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public List<SpParcelamentoOrigem> Lista_Parcelamento_Selected(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Selected where t.Guid == guid orderby t.Idx select t).ToList();
                List<SpParcelamentoOrigem> Lista = new List<SpParcelamentoOrigem>();
                foreach (var item in reg) {
                    SpParcelamentoOrigem Linha = new SpParcelamentoOrigem {
                        Idx = item.Idx,
                        Exercicio = item.Ano,
                        Lancamento = item.Lancamento,
                        Nome_lancamento = item.Lancamento_Nome,
                        Sequencia = item.Sequencia,
                        Parcela = item.Parcela,
                        Complemento = item.Complemento,
                        Data_vencimento = item.Data_Vencimento,
                        Ajuizado = item.Ajuizado,
                        Perc_penalidade = item.Perc_Penalidade,
                        Qtde_parcelamento = item.Qtde_Parcelamento,
                        Valor_principal = item.Valor_Tributo,
                        Valor_juros = item.Valor_Juros,
                        Valor_multa = item.Valor_Multa,
                        Valor_correcao = item.Valor_Correcao,
                        Valor_total = item.Valor_Total,
                        Valor_penalidade = item.Valor_Penalidade
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public Exception Incluir_Parcelamento_Web_Selected(Parcelamento_web_selected Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[18];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@idx", SqlDbType = SqlDbType.Int, SqlValue = Reg.Idx };
                Parametros[2] = new SqlParameter { ParameterName = "@ano", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Ano };
                Parametros[3] = new SqlParameter { ParameterName = "@lancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Lancamento };
                Parametros[4] = new SqlParameter { ParameterName = "@sequencia", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Sequencia };
                Parametros[5] = new SqlParameter { ParameterName = "@parcela", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Parcela };
                Parametros[6] = new SqlParameter { ParameterName = "@complemento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Complemento };
                Parametros[7] = new SqlParameter { ParameterName = "@data_vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                Parametros[8] = new SqlParameter { ParameterName = "@valor_tributo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Tributo };
                Parametros[9] = new SqlParameter { ParameterName = "@valor_multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Multa };
                Parametros[10] = new SqlParameter { ParameterName = "@valor_juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Juros };
                Parametros[11] = new SqlParameter { ParameterName = "@valor_correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Correcao };
                Parametros[12] = new SqlParameter { ParameterName = "@valor_total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Total };
                Parametros[13] = new SqlParameter { ParameterName = "@qtde_parcelamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Qtde_Parcelamento };
                Parametros[14] = new SqlParameter { ParameterName = "@perc_penalidade", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Perc_Penalidade };
                Parametros[15] = new SqlParameter { ParameterName = "@valor_penalidade", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Penalidade };
                Parametros[16] = new SqlParameter { ParameterName = "@lancamento_nome", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Lancamento_Nome };
                Parametros[17] = new SqlParameter { ParameterName = "@ajuizado", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Ajuizado };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_selected(guid,idx,ano,lancamento,sequencia,parcela,complemento,data_vencimento,valor_tributo,valor_multa,valor_juros,valor_correcao," +
                    "valor_total,qtde_parcelamento,perc_penalidade,valor_penalidade,lancamento_nome,ajuizado) VALUES(@guid,@idx,@ano,@lancamento,@sequencia,@parcela,@complemento,@data_vencimento,@valor_tributo," +
                    "@valor_multa,@valor_juros,@valor_correcao,@valor_total,@qtde_parcelamento,@perc_penalidade,@valor_penalidade,@lancamento_nome,@ajuizado)", Parametros);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_parcelamento_Web_Selected(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_selected WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
            
        }

        public Plano Retorna_Plano_Desconto(short Plano) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.Plano where c.Codigo == Plano select c).FirstOrDefault();
            }
        }

        public decimal Retorna_Parcelamento_Valor_Minimo(short Ano,bool Distrito,string Tipo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.Parcelamento_Valor_Minimo where c.Ano == Ano && c.DistritoIndustrial==Distrito && c.Tipo==Tipo select c.Valor).FirstOrDefault();
            }
        }



    }
}
