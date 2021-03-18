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
                foreach (SpExtrato_Parcelamento _row in _extrato) {
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
                        Ajuizado=_row.Dataajuiza==null?'S':'N',
                        Qtde_parcelamento=Qtde_Parcelamento_Efetuados(Codigo,_row.Anoexercicio,_row.Codlancamento,_row.Seqlancamento,_row.Numparcela)
                    };





                    ListaOrigem.Add(_reg);
                    _pos++;
                }
                
                return ListaOrigem;
            }
        }

        public Exception Incluir_Parcelamento_Web_Master(Parcelamento_web_master Reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.CommandTimeout = 180;

                object[] Parametros = new object[13];
                Parametros[0] = new SqlParameter { ParameterName = "@guid", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Guid };
                Parametros[1] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Codigo };
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

                db.Database.ExecuteSqlCommand("INSERT INTO parcelamento_web_master(guid,codigo,user_id,Requerente_Nome,Requerente_CpfCnpj,Requerente_Bairro,Requerente_Cidade,Requerente_Uf,Requerente_Logradouro," +
                    "Requerente_Numero,Requerente_Complemento,Requerente_Cep,Requerente_Telefone) VALUES(@guid,@codigo,@user_id,@Requerente_Nome,@Requerente_CpfCnpj,@Requerente_Bairro,@Requerente_Cidade," +
                    "@Requerente_Uf,@Requerente_Logradouro,@Requerente_Numero,@Requerente_Complemento,@Requerente_Cep,@Requerente_Telefone)", Parametros);
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



    }
}
