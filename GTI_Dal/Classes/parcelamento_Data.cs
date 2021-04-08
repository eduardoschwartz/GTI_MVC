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

                    if (_row.Numparcela == 0)
                        goto NextReg;

                    SpParcelamentoOrigem _reg = new SpParcelamentoOrigem() {
                        Idx = _pos,
                        Exercicio = _row.Anoexercicio,
                        Lancamento = _row.Codlancamento,
                        Nome_lancamento = _row.Desclancamento,
                        Sequencia = _row.Seqlancamento,
                        Parcela = _row.Numparcela,
                        Complemento = _row.Codcomplemento,
                        Data_vencimento = _row.Datavencimento,
                        Valor_principal = _row.Valortributo,
                        Valor_juros = _row.Valorjuros,
                        Valor_multa = _row.Valormulta,
                        Valor_correcao = _row.Valorcorrecao,
                        Valor_total = _row.Valortotal,
                        Ajuizado = _row.Dataajuiza != null ? "S" : "N",
                        Qtde_parcelamento = Qtde_Parcelamento_Efetuados(Codigo, _row.Anoexercicio, _row.Codlancamento, _row.Seqlancamento, _row.Numparcela)
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
                if (Reg.Requerente_Complemento == null)
                    Parametros[10] = new SqlParameter { ParameterName = "@Requerente_Complemento", SqlValue = DBNull.Value };
                else
                    Parametros[10] = new SqlParameter { ParameterName = "@Requerente_Complemento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Requerente_Complemento };
                Parametros[11] = new SqlParameter { ParameterName = "@Requerente_Cep", SqlDbType = SqlDbType.Int, SqlValue = Reg.Requerente_Cep };
                if (Reg.Requerente_Telefone == null)
                    Parametros[12] = new SqlParameter { ParameterName = "@Requerente_Telefone", SqlValue = DBNull.Value };
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

        public Exception Excluir_parcelamento_Web_Origem(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_origem WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }

        }

        public Exception Incluir_Parcelamento_Web_Lista_Codigo(List<Parcelamento_web_lista_codigo> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_web_lista_codigo Reg in Lista) {
                    object[] Parametros = new object[6];
                    Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@Codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
                    Parametros[2] = new SqlParameter { ParameterName = "@Tipo", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Tipo };
                    Parametros[3] = new SqlParameter { ParameterName = "@Documento", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Documento };
                    Parametros[4] = new SqlParameter { ParameterName = "@Descricao", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Descricao };
                    Parametros[5] = new SqlParameter { ParameterName = "@selected", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Selected };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_lista_codigo(guid,Codigo,Tipo,Documento,Descricao,selected) " +
                                                      "VALUES(@guid,@Codigo,@Tipo,@Documento,@Descricao,@selected)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public List<Parcelamento_web_lista_codigo> Lista_Parcelamento_Lista_Codigo(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Lista_Codigo where t.Guid == guid orderby t.Codigo select t).ToList();
                List<Parcelamento_web_lista_codigo> Lista = new List<Parcelamento_web_lista_codigo>();
                foreach (var item in reg) {
                    Parcelamento_web_lista_codigo Linha = new Parcelamento_web_lista_codigo {
                        Guid = guid,
                        Codigo = item.Codigo,
                        Tipo = item.Tipo,
                        Documento = item.Documento,
                        Descricao = item.Descricao,
                        Selected = item.Selected
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        private short Qtde_Parcelamento_Efetuados(int Codigo, short Ano, short Lancamento, short Sequencia, short Parcela) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                DateTime _data = Convert.ToDateTime("08/08/2017");
                return (short)(from o in db.Origemreparc
                               join p in db.Processoreparc on o.Numprocesso equals p.Numprocesso into op from p in op.DefaultIfEmpty()
                               where o.Codreduzido == Codigo && o.Anoexercicio == Ano && o.Codlancamento == Lancamento && o.Numsequencia == Sequencia && o.Numparcela == Parcela && p.Dataprocesso>_data select o.Numprocesso).Distinct().Count();
            }
        }

        public Exception Incluir_Parcelamento_Web_Origem(List<Parcelamento_web_origem> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_web_origem Reg in Lista) {
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

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_origem(guid,idx,ano,lancamento,sequencia,parcela,complemento,data_vencimento,valor_tributo,valor_multa,valor_juros,valor_correcao," +
                            "valor_total,qtde_parcelamento,perc_penalidade,valor_penalidade,lancamento_nome,ajuizado) VALUES(@guid,@idx,@ano,@lancamento,@sequencia,@parcela,@complemento,@data_vencimento,@valor_tributo," +
                            "@valor_multa,@valor_juros,@valor_correcao,@valor_total,@qtde_parcelamento,@perc_penalidade,@valor_penalidade,@lancamento_nome,@ajuizado)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
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

        public Exception Atualizar_Simulado_Master(Parcelamento_web_master reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[15];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Sim_Liquido", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Liquido };
                Parametros[2] = new SqlParameter { ParameterName = "@Sim_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Juros };
                Parametros[3] = new SqlParameter { ParameterName = "@Sim_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Multa };
                Parametros[4] = new SqlParameter { ParameterName = "@Sim_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Correcao };
                Parametros[5] = new SqlParameter { ParameterName = "@Sim_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Principal };
                Parametros[6] = new SqlParameter { ParameterName = "@Sim_Honorario", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Honorario };
                Parametros[7] = new SqlParameter { ParameterName = "@Sim_Juros_apl", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Juros_apl };
                Parametros[8] = new SqlParameter { ParameterName = "@Sim_Total", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Total };
                Parametros[9] = new SqlParameter { ParameterName = "@Sim_Perc_Liquido", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Liquido };
                Parametros[10] = new SqlParameter { ParameterName = "@Sim_Perc_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Juros };
                Parametros[11] = new SqlParameter { ParameterName = "@Sim_Perc_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Multa };
                Parametros[12] = new SqlParameter { ParameterName = "@Sim_Perc_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Correcao };
                Parametros[13] = new SqlParameter { ParameterName = "@Sim_Perc_Juros_Apl", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Juros_Apl };
                Parametros[14] = new SqlParameter { ParameterName = "@Sim_Perc_Honorario", SqlDbType = SqlDbType.Decimal, SqlValue = reg.Sim_Perc_Honorario };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE parcelamento_web_master set Sim_Liquido=@Sim_Liquido,Sim_Juros=@Sim_Juros,Sim_Multa=@Sim_Multa,Sim_Correcao=@Sim_Correcao,Sim_Honorario=@Sim_Honorario," +
                        "Sim_Juros_apl=@Sim_Juros_apl,Sim_Total=@Sim_Total,Sim_Perc_Liquido=@Sim_Perc_Liquido,Sim_Perc_Juros=@Sim_Perc_Juros,Sim_Perc_Multa=@Sim_Perc_Multa," +
                        "Sim_Perc_Correcao=@Sim_Perc_Correcao,Sim_Perc_Juros_Apl=@Sim_Perc_Juros_Apl,Sim_Perc_Honorario=@Sim_Perc_Honorario WHERE guid=@guid", Parametros);
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

        public Exception Incluir_Parcelamento_Web_Selected(List<Parcelamento_web_selected> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_web_selected Reg in Lista) {
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

        public decimal Retorna_Parcelamento_Valor_Minimo(short Ano, bool Distrito, string Tipo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.Parcelamento_Valor_Minimo where c.Ano == Ano && c.DistritoIndustrial == Distrito && c.Tipo == Tipo select c.Valor).FirstOrDefault();
            }
        }

        public Exception Incluir_Parcelamento_Web_Simulado(Parcelamento_Web_Simulado Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[15];
                Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Qtde_Parcela", SqlDbType = SqlDbType.Int, SqlValue = Reg.Qtde_Parcela };
                Parametros[2] = new SqlParameter { ParameterName = "@Numero_Parcela", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Numero_Parcela };
                Parametros[3] = new SqlParameter { ParameterName = "@Data_Vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                Parametros[4] = new SqlParameter { ParameterName = "@Valor_Liquido", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Liquido };
                Parametros[5] = new SqlParameter { ParameterName = "@Valor_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Juros };
                Parametros[6] = new SqlParameter { ParameterName = "@Valor_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Multa };
                Parametros[7] = new SqlParameter { ParameterName = "@Valor_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Correcao };
                Parametros[8] = new SqlParameter { ParameterName = "@Valor_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Principal };
                Parametros[9] = new SqlParameter { ParameterName = "@Saldo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Saldo };
                Parametros[10] = new SqlParameter { ParameterName = "@Juros_Perc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Perc };
                Parametros[11] = new SqlParameter { ParameterName = "@Juros_Mes", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Mes };
                Parametros[12] = new SqlParameter { ParameterName = "@Juros_Apl", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Apl };
                Parametros[13] = new SqlParameter { ParameterName = "@Valor_Honorario", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Honorario };
                Parametros[14] = new SqlParameter { ParameterName = "@Valor_Total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Total };

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_simulado(Guid,Qtde_Parcela,Numero_Parcela,Data_Vencimento,Valor_Liquido,Valor_Juros,Valor_Multa,Valor_Correcao,Valor_Principal," +
                        "Saldo,Juros_Perc,Juros_Mes,Juros_Apl,Valor_Honorario,Valor_Total) VALUES(@Guid,@Qtde_Parcela,@Numero_Parcela,@Data_Vencimento,@Valor_Liquido,@Valor_Juros,@Valor_Multa,@Valor_Correcao," +
                        "@Valor_Principal,@Saldo,@Juros_Perc,@Juros_Mes,@Juros_Apl,@Valor_Honorario,@Valor_Total)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_parcelamento_Web_Simulado(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_simulado WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }

        }

        public List<Parcelamento_Web_Simulado> Retorna_Parcelamento_Web_Simulado(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from p in db.Parcelamento_Web_Simulado where p.Guid == guid select p).ToList();
            }
        }

        public List<Parcelamento_Web_Simulado> Retorna_Parcelamento_Web_Simulado(string guid, int qtde_parcela) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from p in db.Parcelamento_Web_Simulado where p.Guid == guid && p.Qtde_Parcela == qtde_parcela select p).ToList();
            }
        }

        public List<Parcelamento_Web_Simulado> Lista_Parcelamento_Destino(string Guid,short Plano, DateTime Data_Vencimento,bool Ajuizado,bool Honorario,decimal Principal,decimal Juros,decimal Multa, decimal Correcao,decimal Total,decimal Adicional,decimal Valor_Minimo) {
            
            Tributario_Data tributarioRepository = new Tributario_Data(_connection);
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 600;
                object[] Parametros = new object[12];
                Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Plano", SqlDbType = SqlDbType.SmallInt, SqlValue = Plano };
                Parametros[2] = new SqlParameter { ParameterName = "@Data_Vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Data_Vencimento };
                Parametros[3] = new SqlParameter { ParameterName = "@Ajuizado", SqlDbType = SqlDbType.Bit, SqlValue = Ajuizado };
                Parametros[4] = new SqlParameter { ParameterName = "@Honorario", SqlDbType = SqlDbType.Bit, SqlValue = Honorario };
                Parametros[5] = new SqlParameter { ParameterName = "@Principal", SqlDbType = SqlDbType.Decimal, SqlValue = Principal };
                Parametros[6] = new SqlParameter { ParameterName = "@Juros", SqlDbType = SqlDbType.Decimal, SqlValue = Juros };
                Parametros[7] = new SqlParameter { ParameterName = "@Multa", SqlDbType = SqlDbType.Decimal, SqlValue = Multa };
                Parametros[8] = new SqlParameter { ParameterName = "@Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Correcao };
                Parametros[9] = new SqlParameter { ParameterName = "@Total", SqlDbType = SqlDbType.Decimal, SqlValue = Total };
                Parametros[10] = new SqlParameter { ParameterName = "@Adicional", SqlDbType = SqlDbType.Decimal, SqlValue = Adicional };
                Parametros[11] = new SqlParameter { ParameterName = "@Valor_Minimo", SqlDbType = SqlDbType.Decimal, SqlValue = Valor_Minimo };
                var result = db.spParcelamentoDestinoWeb.SqlQuery("EXEC spPARCELAMENTODESTINOWEB @Guid,@Plano,@Data_Vencimento, @Ajuizado, @Honorario, " +
                    "@Principal ,@Juros ,@Multa, @Correcao ,@Total, @Adicional, @Valor_Minimo ", Parametros).ToList();


                List<Parcelamento_Web_Simulado> Lista = new List<Parcelamento_Web_Simulado>();
                foreach (SpParcelamentoDestinoWeb item in result) {
                    if (item.Guid == Guid) {
                        Parcelamento_Web_Simulado reg = new Parcelamento_Web_Simulado() {
                            Data_Vencimento = item.Data_Vencimento,
                            Valor_Honorario = item.Valor_Honorario,
                            Juros_Apl = item.Juros_Apl,
                            Juros_Mes = item.Juros_Mes,
                            Juros_Perc = item.Juros_Perc,
                            Numero_Parcela = item.Numero_Parcela,
                            Qtde_Parcela = item.Qtde_Parcela,
                            Saldo = item.Saldo,
                            Valor_Juros = item.Valor_Juros,
                            Valor_Correcao = item.Valor_Correcao,
                            Valor_Liquido = item.Valor_Liquido,
                            Valor_Multa = item.Valor_Multa,
                            Valor_Total = item.Valor_Total,
                            Valor_Principal = item.Valor_Principal
                        };
                        Lista.Add(reg);
                    }
                }

                return Lista;
            }
        }

        public Exception Incluir_Parcelamento_Web_Simulado_Resumo(List<Parcelamento_Web_Simulado_Resumo> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_Web_Simulado_Resumo Reg in Lista) {
                    object[] Parametros = new object[5];
                    Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@Qtde_Parcela", SqlDbType = SqlDbType.Int, SqlValue = Reg.Qtde_Parcela };
                    Parametros[2] = new SqlParameter { ParameterName = "@Valor_Entrada", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Entrada };
                    Parametros[3] = new SqlParameter { ParameterName = "@Valor_N", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_N };
                    Parametros[4] = new SqlParameter { ParameterName = "@Valor_Total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Total };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_simulado_resumo(Guid,Qtde_Parcela,Valor_Entrada,Valor_N,Valor_Total) VALUES(@Guid,@Qtde_Parcela,@Valor_Entrada,@Valor_N,@Valor_Total)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Excluir_parcelamento_Web_Simulado_Resumo(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_simulado_resumo WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }

        }

        public Exception Atualizar_QtdeParcela_Master(string Guid,int Qtde) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[2];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@qtde", SqlDbType = SqlDbType.Int, SqlValue =Qtde };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE parcelamento_web_master set Qtde_Parcela=@qtde WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Atualizar_Processo_Master(string Guid,short Ano ,int Numero) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[3];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@processo_ano", SqlDbType = SqlDbType.SmallInt, SqlValue = Ano };
                Parametros[2] = new SqlParameter { ParameterName = "@processo_numero", SqlDbType = SqlDbType.Int, SqlValue = Numero };
                try {
                    db.Database.ExecuteSqlCommand("UPDATE parcelamento_web_master set processo_ano=@processo_ano,processo_numero=@processo_numero WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Parcelamento_Web_Destino(List<Parcelamento_Web_Destino> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_Web_Destino Reg in Lista) {
                    object[] Parametros = new object[15];
                    Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                    Parametros[1] = new SqlParameter { ParameterName = "@Numero_Parcela", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Numero_Parcela };
                    Parametros[2] = new SqlParameter { ParameterName = "@Data_Vencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Data_Vencimento };
                    Parametros[3] = new SqlParameter { ParameterName = "@Valor_Liquido", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Liquido };
                    Parametros[4] = new SqlParameter { ParameterName = "@Valor_Juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Juros };
                    Parametros[5] = new SqlParameter { ParameterName = "@Valor_Multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Multa };
                    Parametros[6] = new SqlParameter { ParameterName = "@Valor_Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Correcao };
                    Parametros[7] = new SqlParameter { ParameterName = "@Valor_Principal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Principal };
                    Parametros[8] = new SqlParameter { ParameterName = "@Saldo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Saldo };
                    Parametros[9] = new SqlParameter { ParameterName = "@Juros_Perc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Perc };
                    Parametros[10] = new SqlParameter { ParameterName = "@Juros_Mes", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Mes };
                    Parametros[11] = new SqlParameter { ParameterName = "@Juros_Apl", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros_Apl };
                    Parametros[12] = new SqlParameter { ParameterName = "@Valor_Honorario", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Honorario };
                    Parametros[13] = new SqlParameter { ParameterName = "@Valor_Total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor_Total };
                    Parametros[14] = new SqlParameter { ParameterName = "@Proporcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Proporcao };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_destino(Guid,Numero_Parcela,Data_Vencimento,Valor_Liquido,Valor_Juros,Valor_Multa,Valor_Correcao,Valor_Principal," +
                            "Saldo,Juros_Perc,Juros_Mes,Juros_Apl,Valor_Honorario,Valor_Total,Proporcao) VALUES(@Guid,@Numero_Parcela,@Data_Vencimento,@Valor_Liquido,@Valor_Juros,@Valor_Multa,@Valor_Correcao," +
                            "@Valor_Principal,@Saldo,@Juros_Perc,@Juros_Mes,@Juros_Apl,@Valor_Honorario,@Valor_Total,@Proporcao)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Excluir_Parcelamento_Web_Destino(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_destino WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }

        }

        public Exception Excluir_parcelamento_Web_Lista_Codigo(string Guid) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[1];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Guid };
                try {
                    db.Database.ExecuteSqlCommand("DELETE FROM parcelamento_web_lista_codigo WHERE guid=@guid", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }

        }

        public Exception Incluir_ProcessoReparc(Processoreparc Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[18];
                Parametros[0] = new SqlParameter { ParameterName = "@Numprocesso", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Numprocesso };
                Parametros[1] = new SqlParameter { ParameterName = "@Numproc", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numproc };
                Parametros[2] = new SqlParameter { ParameterName = "@Anoproc", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoproc };
                Parametros[3] = new SqlParameter { ParameterName = "@Dataprocesso", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Dataprocesso };
                Parametros[4] = new SqlParameter { ParameterName = "@Datareparc", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Datareparc };
                Parametros[5] = new SqlParameter { ParameterName = "@Qtdeparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Qtdeparcela };
                Parametros[6] = new SqlParameter { ParameterName = "@Valorentrada", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valorentrada };
                Parametros[7] = new SqlParameter { ParameterName = "@Percentrada", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Percentrada };
                Parametros[8] = new SqlParameter { ParameterName = "@Calculamulta", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Calculamulta };
                Parametros[9] = new SqlParameter { ParameterName = "@Calculajuros", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Calculajuros };
                Parametros[10] = new SqlParameter { ParameterName = "@Calculacorrecao", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Calculacorrecao };
                Parametros[11] = new SqlParameter { ParameterName = "@Penhora", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Penhora };
                Parametros[12] = new SqlParameter { ParameterName = "@Honorario", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Honorario };
                Parametros[13] = new SqlParameter { ParameterName = "@Codigoresp", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigoresp };
                Parametros[14] = new SqlParameter { ParameterName = "@Plano", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Plano };
                Parametros[15] = new SqlParameter { ParameterName = "@Novo", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Novo };
                Parametros[16] = new SqlParameter { ParameterName = "@Userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                Parametros[17] = new SqlParameter { ParameterName = "@Userweb", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Userweb };

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO processoreparc(Numprocesso,Numproc,Anoproc,Dataprocesso,Datareparc,Qtdeparcela,Valorentrada,Percentrada,Calculamulta,Calculajuros," +
                        "Calculacorrecao,Penhora,Honorario,Codigoresp,Plano,Novo,Userid,Userweb) VALUES(@Numprocesso,@Numproc,@Anoproc,@Dataprocesso,@Datareparc,@Qtdeparcela,@Valorentrada,@Percentrada," +
                        "@Calculamulta,@Calculajuros,@Calculacorrecao,@Penhora,@Honorario,@Codigoresp,@Plano,@Novo,@Userid,@Userweb)", Parametros);
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Parcelamento_Web_Destino> Lista_Parcelamento_Web_Destino(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Destino where t.Guid == guid orderby t.Data_Vencimento select t).ToList();
                List<Parcelamento_Web_Destino> Lista = new List<Parcelamento_Web_Destino>();
                foreach (var item in reg) {
                    Parcelamento_Web_Destino Linha = new Parcelamento_Web_Destino {
                        Data_Vencimento = item.Data_Vencimento,
                        Valor_Honorario = item.Valor_Honorario,
                        Juros_Apl = item.Juros_Apl,
                        Juros_Mes = item.Juros_Mes,
                        Juros_Perc = item.Juros_Perc,
                        Numero_Parcela = item.Numero_Parcela,
                        Saldo = item.Saldo,
                        Valor_Juros = item.Valor_Juros,
                        Valor_Correcao = item.Valor_Correcao,
                        Valor_Liquido = item.Valor_Liquido,
                        Valor_Multa = item.Valor_Multa,
                        Valor_Total = item.Valor_Total,
                        Valor_Principal = item.Valor_Principal
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public Exception Incluir_OrigemReparc(List<Origemreparc> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Origemreparc Reg in Lista) {
                    object[] Parametros = new object[13];
                    Parametros[0] = new SqlParameter { ParameterName = "@Numprocesso", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Numprocesso };
                    Parametros[1] = new SqlParameter { ParameterName = "@Numproc", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numproc };
                    Parametros[2] = new SqlParameter { ParameterName = "@Anoproc", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoproc };
                    Parametros[3] = new SqlParameter { ParameterName = "@Codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codreduzido };
                    Parametros[4] = new SqlParameter { ParameterName = "@Anoexercicio", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoexercicio };
                    Parametros[5] = new SqlParameter { ParameterName = "@Codlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Codlancamento };
                    Parametros[6] = new SqlParameter { ParameterName = "@Numsequencia", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numsequencia };
                    Parametros[7] = new SqlParameter { ParameterName = "@Numparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numparcela };
                    Parametros[8] = new SqlParameter { ParameterName = "@Codcomplemento", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Codcomplemento };
                    Parametros[9] = new SqlParameter { ParameterName = "@Principal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Principal };
                    Parametros[10] = new SqlParameter { ParameterName = "@Juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros };
                    Parametros[11] = new SqlParameter { ParameterName = "@Multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Multa };
                    Parametros[12] = new SqlParameter { ParameterName = "@Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Correcao };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO origemreparc(Numprocesso,Numproc,Anoproc,Codreduzido,Anoexercicio,Codlancamento,Numsequencia,Numparcela,Codcomplemento,Principal," +
                            "Juros,Multa,Correcao) VALUES(@Numprocesso,@Numproc,@Anoproc,@Codreduzido,@Anoexercicio,@Codlancamento,@Numsequencia,@Numparcela,@Codcomplemento,@Principal," +
                            "@Juros,@Multa,@Correcao)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Incluir_DestinoReparc(List<Destinoreparc> Lista) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Destinoreparc Reg in Lista) {
                    object[] Parametros = new object[20];
                    Parametros[0] = new SqlParameter { ParameterName = "@Numprocesso", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Numprocesso };
                    Parametros[1] = new SqlParameter { ParameterName = "@Numproc", SqlDbType = SqlDbType.Int, SqlValue = Reg.Numproc };
                    Parametros[2] = new SqlParameter { ParameterName = "@Anoproc", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoproc };
                    Parametros[3] = new SqlParameter { ParameterName = "@Codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codreduzido };
                    Parametros[4] = new SqlParameter { ParameterName = "@Anoexercicio", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoexercicio };
                    Parametros[5] = new SqlParameter { ParameterName = "@Codlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Codlancamento };
                    Parametros[6] = new SqlParameter { ParameterName = "@Numsequencia", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numsequencia };
                    Parametros[7] = new SqlParameter { ParameterName = "@Numparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numparcela };
                    Parametros[8] = new SqlParameter { ParameterName = "@Codcomplemento", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Codcomplemento };
                    Parametros[9] = new SqlParameter { ParameterName = "@Valorliquido", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valorliquido };
                    Parametros[10] = new SqlParameter { ParameterName = "@Juros", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Juros };
                    Parametros[11] = new SqlParameter { ParameterName = "@Multa", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Multa };
                    Parametros[12] = new SqlParameter { ParameterName = "@Correcao", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Correcao };
                    Parametros[13] = new SqlParameter { ParameterName = "@Valorprincipal", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valorprincipal };
                    Parametros[14] = new SqlParameter { ParameterName = "@Saldo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Saldo };
                    Parametros[15] = new SqlParameter { ParameterName = "@Jurosperc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Jurosperc };
                    Parametros[16] = new SqlParameter { ParameterName = "@Jurosvalor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Jurosvalor };
                    Parametros[17] = new SqlParameter { ParameterName = "@Jurosapl", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Jurosapl };
                    Parametros[18] = new SqlParameter { ParameterName = "@Honorario", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Honorario };
                    Parametros[19] = new SqlParameter { ParameterName = "@Total", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Total };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO destinoreparc(Numprocesso,Numproc,Anoproc,Codreduzido,Anoexercicio,Codlancamento,Numsequencia,Numparcela,Codcomplemento,Valorliquido," +
                            "Juros,Multa,Correcao,Valorprincipal,Saldo,Jurosperc,Jurosvalor,Jurosapl,Honorario,Total) VALUES(@Numprocesso,@Numproc,@Anoproc,@Codreduzido,@Anoexercicio,@Codlancamento," +
                            "@Numsequencia,@Numparcela,@Codcomplemento,@Valorliquido,@Juros,@Multa,@Correcao,@Valorprincipal,@Saldo,@Jurosperc,@Jurosvalor,@Jurosapl,@Honorario,@Total)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public byte Retorna_Seq_Disponivel(int Codigo) {
            byte _seq = 0;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from t in db.Debitoparcela where t.Codreduzido==Codigo && t.Codlancamento==20 orderby t.Seqlancamento descending select t).FirstOrDefault();
                if (Sql != null) {
                    _seq = (byte)(Sql.Seqlancamento + 1);
                }
            }
            return _seq;
        }

        public Exception Incluir_Debito_Parcela(List<Debitoparcela> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Debitoparcela Reg in Lista) {
                    object[] Parametros = new object[10];
                    Parametros[0] = new SqlParameter { ParameterName = "@codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codreduzido };
                    Parametros[1] = new SqlParameter { ParameterName = "@anoexercicio", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoexercicio };
                    Parametros[2] = new SqlParameter { ParameterName = "@codlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Codlancamento };
                    Parametros[3] = new SqlParameter { ParameterName = "@seqlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Seqlancamento };
                    Parametros[4] = new SqlParameter { ParameterName = "@numparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numparcela };
                    Parametros[5] = new SqlParameter { ParameterName = "@codcomplemento", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Codcomplemento };
                    Parametros[6] = new SqlParameter { ParameterName = "@statuslanc", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Statuslanc };
                    Parametros[7] = new SqlParameter { ParameterName = "@datavencimento", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Datavencimento };
                    Parametros[8] = new SqlParameter { ParameterName = "@datadebase", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Reg.Datadebase };
                    Parametros[9] = new SqlParameter { ParameterName = "@Numprocesso", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Numprocesso };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO debitoparcela(codreduzido,anoexercicio,codlancamento,seqlancamento,numparcela,codcomplemento,statuslanc,datavencimento,datadebase,Numprocesso) " +
                            "VALUES(@codreduzido,@anoexercicio,@codlancamento,@seqlancamento,@numparcela,@codcomplemento,@statuslanc,@datavencimento,@datadebase,@Numprocesso)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public Exception Incluir_Parcelamento_Web_Tributo(Parcelamento_Web_Tributo Reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;
                object[] Parametros = new object[4];
                Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@Tributo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Tributo };
                Parametros[2] = new SqlParameter { ParameterName = "@Valor", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valor };
                Parametros[3] = new SqlParameter { ParameterName = "@Perc", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Perc };

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_tributo(guid,tributo,valor,perc) VALUES(@guid,@tributo,@valor,@perc)", Parametros);

                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Parcelamento_Web_Tributo(List<Parcelamento_Web_Tributo> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Parcelamento_Web_Tributo item in Lista) {
                    try {
                        object[] Parametros = new object[4];
                        Parametros[0] = new SqlParameter { ParameterName = "@Guid", SqlDbType = SqlDbType.VarChar, SqlValue = item.Guid };
                        Parametros[1] = new SqlParameter { ParameterName = "@Tributo", SqlDbType = SqlDbType.Int, SqlValue = item.Tributo };
                        Parametros[2] = new SqlParameter { ParameterName = "@Valor", SqlDbType = SqlDbType.Decimal, SqlValue = item.Valor };
                        Parametros[3] = new SqlParameter { ParameterName = "@Perc", SqlDbType = SqlDbType.Decimal, SqlValue = item.Perc };

                        db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_tributo(guid,tributo,valor,perc) VALUES(@guid,@tributo,@valor,@perc)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public List<Parcelamento_Web_Tributo> Lista_Parcelamento_Tributo(string guid) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Tributo where t.Guid == guid orderby t.Tributo select t).ToList();
                List<Parcelamento_Web_Tributo> Lista = new List<Parcelamento_Web_Tributo>();
                foreach (var item in reg) {
                    Parcelamento_Web_Tributo Linha = new Parcelamento_Web_Tributo {
                      Guid=item.Guid,
                      Tributo=item.Tributo,
                      Valor=item.Valor,
                      Perc=item.Perc
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public Exception Incluir_Debito_Tributo(List<Debitotributo> Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                foreach (Debitotributo Reg in Lista) {
                    object[] Parametros = new object[8];
                    Parametros[0] = new SqlParameter { ParameterName = "@codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codreduzido };
                    Parametros[1] = new SqlParameter { ParameterName = "@anoexercicio", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Anoexercicio };
                    Parametros[2] = new SqlParameter { ParameterName = "@codlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Codlancamento };
                    Parametros[3] = new SqlParameter { ParameterName = "@seqlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Seqlancamento };
                    Parametros[4] = new SqlParameter { ParameterName = "@numparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Numparcela };
                    Parametros[5] = new SqlParameter { ParameterName = "@codcomplemento", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Codcomplemento };
                    Parametros[6] = new SqlParameter { ParameterName = "@Codtributo", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Codtributo };
                    Parametros[7] = new SqlParameter { ParameterName = "@Valortributo", SqlDbType = SqlDbType.Decimal, SqlValue = Reg.Valortributo };

                    try {
                        db.Database.ExecuteSqlCommand("INSERT INTO debitotributo(codreduzido,anoexercicio,codlancamento,seqlancamento,numparcela,codcomplemento,Codtributo,Valortributo) " +
                            "VALUES(@codreduzido,@anoexercicio,@codlancamento,@seqlancamento,@numparcela,@codcomplemento,@Codtributo,@Valortributo)", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }
                return null;
            }
        }

        public Exception Atualizar_Status_Origem(int Codigo,List<SpParcelamentoOrigem>Lista) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                object[] Parametros = new object[6];

                foreach (SpParcelamentoOrigem Reg in Lista) {
                    Parametros[0] = new SqlParameter { ParameterName = "@codreduzido", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
                    Parametros[1] = new SqlParameter { ParameterName = "@anoexercicio", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Exercicio };
                    Parametros[2] = new SqlParameter { ParameterName = "@codlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Lancamento };
                    Parametros[3] = new SqlParameter { ParameterName = "@seqlancamento", SqlDbType = SqlDbType.SmallInt, SqlValue = Reg.Sequencia };
                    Parametros[4] = new SqlParameter { ParameterName = "@numparcela", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Parcela };
                    Parametros[5] = new SqlParameter { ParameterName = "@codcomplemento", SqlDbType = SqlDbType.TinyInt, SqlValue = Reg.Complemento };
                    try {
                        db.Database.ExecuteSqlCommand("UPDATE debitoparcela set statuslanc=4 WHERE codreduzido=@codreduzido and anoexercicio=@anoexercicio and codlancamento=@codlancamento and " +
                            "seqlancamento=@seqlancamento and numparcela=@numparcela and codcomplemento=@codcomplemento", Parametros);
                    } catch (Exception ex) {
                        return ex;
                    }
                }

                return null;
            }
        }

        public List<Parc_Processos> Lista_Parcelamento_Processos(int _userId) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Parcelamento_Web_Master where t.Processo_Numero>0 && t.User_id==_userId orderby t.Data_Geracao select t).ToList();
                List<Parc_Processos> Lista = new List<Parc_Processos>();
                foreach (var item in reg) {
                    Parc_Processos Linha = new Parc_Processos {
                        Guid = item.Guid,
                        Codigo_Contribuinte = item.Contribuinte_Codigo,
                        Data_Parcelamento = item.Data_Geracao,
                        Nome_Contribuinte = dalCore.TruncateTo( item.Contribuinte_nome,37),
                        Numero_Processo = item.Processo_Numero.ToString() + "-" + dalCore.RetornaDvProcesso(item.Processo_Numero) + "/" + item.Processo_Ano.ToString(),
                        Situacao = "ATIVO"
                    };
                    Linha.Tipo= item.Contribuinte_Codigo < 50000 ? "Imóvel" : item.Contribuinte_Codigo >= 500000 ? "Outros" : "Empresa";
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public List<Destinoreparc> Lista_Destino_Parcelamento(short AnoProcesso,int NumProcesso) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 3 * 60;
                List<Destinoreparc> Lista = (from d in db.Destinoreparc where d.Anoproc == AnoProcesso && d.Numproc==NumProcesso orderby d.Numparcela  select d).Distinct().ToList();
                return Lista;
            }
        }

        public List<Debitotributo> Lista_Debito_Tributo(int Codigo,short Ano,short Lanc,short Seq,short Parcela,short Compl) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from t in db.Debitotributo where t.Codreduzido==Codigo && t.Anoexercicio==Ano && t.Codlancamento==Lanc && t.Seqlancamento==Seq && t.Numparcela==Parcela && t.Codcomplemento==Compl orderby t.Codtributo select t).ToList();
                List<Debitotributo> Lista = new List<Debitotributo>();
                foreach (var item in reg) {
                    Debitotributo t = new Debitotributo {
                        Codreduzido=item.Codreduzido,
                        Anoexercicio=item.Anoexercicio,
                        Codlancamento=item.Codlancamento,
                        Seqlancamento=item.Seqlancamento,
                        Numparcela=item.Numparcela,
                        Codcomplemento=item.Codcomplemento,
                        Codtributo=item.Codtributo,
                        Valortributo=item.Valortributo
                    };
                    Lista.Add(t);
                }
                return Lista;
            }
        }



    }
}


