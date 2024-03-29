﻿using GTI_Models.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Endereco_Data {

        private string _connection;
        public Endereco_Data(string sConnection) {
            _connection = sConnection;
        }

        public bool Bairro_uso_cidadao(string id_UF, int id_cidade,int id_bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod1 = (from c in db.Cidadao where c.Siglauf == id_UF && c.Codcidade == id_cidade && c.Codbairro == id_bairro select c.Codcidadao).Count();
                var cntCod2 = (from c in db.Cidadao where c.Siglauf == id_UF && c.Codcidade == id_cidade && c.Codbairro2 == id_bairro select c.Codcidadao).Count();
                return cntCod1 > 0||cntCod2>0 ? true : false;
            }
        }

        public bool Bairro_uso_empresa(string id_UF, int id_cidade, int id_bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod1 = (from c in db.Mobiliario where c.Siglauf == id_UF && c.Codcidade == id_cidade && c.Codbairro == id_bairro select c.Codigomob).Count();
                return cntCod1 > 0  ? true : false;
            }
        }

        public bool Bairro_uso_processo(string id_UF, int id_cidade, int id_bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod1 = (from c in db.Processocidadao where c.Siglauf == id_UF && c.Codcidade == id_cidade && c.Codbairro == id_bairro select c.Codcidadao).Count();
                return cntCod1 > 0 ? true : false;
            }
        }

        public bool Pais_uso_cidadao(int id_pais) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod1 = (from c in db.Cidadao where c.Codpais == id_pais  select c.Codcidadao).Count();
                var cntCod2 = (from c in db.Cidadao where c.Codpais2 == id_pais select c.Codcidadao).Count();
                return cntCod1 > 0 || cntCod2 > 0 ? true : false;
            }
        }
               
        public List<Bairro> Lista_Bairro(string UF, int cidade) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from b in db.Bairro where b.Siglauf == UF && b.Codcidade == cidade orderby b.Descbairro where b.Codbairro!=999 && b.Codbairro>0  && b.Descbairro.Trim()!=""  select b);
                return Sql.ToList();
            }
        }

        public int Incluir_bairro(Bairro reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod = (from c in db.Bairro where c.Siglauf == reg.Siglauf && c.Codcidade == reg.Codcidade select c.Codbairro).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Bairro where c.Siglauf == reg.Siglauf && c.Codcidade == reg.Codcidade select c.Codbairro).Max() + 1;
                reg.Codbairro = Convert.ToInt16(maxCod);
                db.Bairro.Add(reg);
                try {
                    db.SaveChanges();
                } catch  {
                    maxCod = 0;
                }
                return maxCod;
            }
        }

        public int Incluir_cidade(Cidade reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var cntCod = (from c in db.Cidade where c.Siglauf == reg.Siglauf  select c.Codcidade).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Cidade where c.Siglauf == reg.Siglauf  select c.Codcidade).Max() + 1;
                reg.Codcidade = Convert.ToInt16(maxCod);
                db.Cidade.Add(reg);
                try {
                    db.SaveChanges();
                } catch {
                    maxCod = 0;
                }
                return maxCod;
            }
        }



        public Exception Alterar_Bairro(Bairro reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string sUF = reg.Siglauf;
                int nCodCidade = reg.Codcidade;
                int nCodBairro = reg.Codbairro;
                Bairro b = db.Bairro.First(i => i.Siglauf == sUF && i.Codcidade == nCodCidade && i.Codbairro == nCodBairro);
                b.Descbairro = reg.Descbairro;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Cidade(Cidade reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string sUF = reg.Siglauf;
                int nCodCidade = reg.Codcidade;
                Cidade b = db.Cidade.First(i => i.Siglauf == sUF && i.Codcidade == nCodCidade );
                b.Desccidade = reg.Desccidade;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }



        public Exception Excluir_Bairro(Bairro reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                
                    string sUF = reg.Siglauf;
                    int nCodCidade = reg.Codcidade;
                    int nCodBairro = reg.Codbairro;
                    Bairro b = db.Bairro.First(i => i.Siglauf == sUF && i.Codcidade == nCodCidade && i.Codbairro == nCodBairro);
                try {
                    db.Bairro.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List <Cidade> Lista_Cidade(string sUF) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cidade select c);
                if (!string.IsNullOrEmpty(sUF))
                     Sql = Sql.Where(u => u.Siglauf == sUF).OrderBy(u=>u.Desccidade);

                return Sql.ToList();
            }
        }

        public List<Uf> Lista_UF() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Uf orderby c.Descuf select c);
                return Sql.ToList();
            }
        }

        public List<Pais> Lista_Pais() {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Pais select c);
                return Sql.ToList();
            }
        }

        public string Retorna_Logradouro(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Logradouro where c.Codlogradouro == Codigo select c.Endereco).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_Pais(int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                string Sql = (from c in db.Pais where c.Id_pais == Codigo select c.Nome_pais).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_Cidade(string UF, int Codigo) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cidade where c.Siglauf == UF && c.Codcidade==Codigo select c.Desccidade).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_UfNome(string UF) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Uf where c.Siglauf == UF  select c.Descuf).FirstOrDefault();
                return Sql;
            }
        }

        public int Retorna_Cidade(string UF, string Cidade) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cidade where c.Siglauf == UF && c.Desccidade == Cidade select c.Codcidade).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_Bairro(string UF, int Cidade,int Bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Bairro where c.Siglauf == UF && c.Codcidade == Cidade && c.Codbairro==Bairro select c.Descbairro).FirstOrDefault();
                return Sql;
            }
        }

        public int Retorna_Bairro(string UF, int Cidade, string Bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int Sql = (from c in db.Bairro where c.Siglauf == UF && c.Codcidade == Cidade && c.Descbairro == Bairro select c.Codbairro).FirstOrDefault();
                return Sql;
            }
        }

        public Exception Incluir_Pais(Pais reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Pais select c).Count();
                int maxCod = 1;
                if(cntCod>0)
                    maxCod = (from c in db.Pais select c.Id_pais).Max()+1;
                reg.Id_pais = maxCod;
                db.Pais.Add(reg);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Pais(Pais reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCodPais = reg.Id_pais;
                Pais b = db.Pais.First(i => i.Id_pais == nCodPais);
                b.Nome_pais = reg.Nome_pais;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Pais(Pais reg) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                int nCodPais = reg.Id_pais;
                Pais b = db.Pais.First(i =>  i.Id_pais == nCodPais);
                try {
                    db.Pais.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


        public List<Logradouro> Lista_Logradouro(String Filter = "") {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from l in db.Logradouro
                           select new { l.Codlogradouro, l.Endereco });
                if (!String.IsNullOrEmpty(Filter))
                    reg = reg.Where(u => u.Endereco.Contains(Filter)).OrderBy(u=>u.Endereco);

                List<Logradouro> Lista = new List<Logradouro>();
                foreach (var query in reg) {
                    Logradouro Linha = new Logradouro {
                        Codlogradouro = query.Codlogradouro,
                        Endereco = query.Endereco
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }
        
        public int RetornaCep(Int32 CodigoLogradouro, Int16 Numero) {
            int nCep = 0;
            int Num1, Num2;
            bool bPar, bImpar;

            if (Numero % 2 == 0) {
                bPar = true; bImpar = false;
            } else {
                bPar = false; bImpar = true;
            }

            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cep where c.Codlogr == CodigoLogradouro select c).ToList();
                if (Sql.Count == 0)
                    nCep = 0;
                else if (Sql.Count == 1)
                    nCep = Sql[0].cep;
                else {
                    foreach (var item in Sql) {
                        Num1 = Convert.ToInt32(item.Valor1);
                        Num2 = Convert.ToInt32(item.Valor2);
                        if (Numero >= Num1 && Numero <= Num2) {
                            if ((bImpar && item.Impar ) || (bPar && item.Par )) {
                                nCep = item.cep;
                                break;
                            }
                        } else if (Numero >= Num1 && Num2 == 0) {
                            if ((bImpar && item.Impar) || (bPar && item.Par )) {
                                nCep = item.cep;
                                break;
                            }
                        }
                    }
                }
            }
            return nCep;
        }


        public Bairro RetornaLogradouroBairro(Int32 CodigoLogradouro, Int16 Numero) {
            int nBairro = 0;
            int Num1, Num2;
            Bairro _bairro = new Bairro();

            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from e in db.Logradouro_Bairro where e.Logradouro == CodigoLogradouro select e).ToList();
                if (Sql.Count == 0)
                    nBairro = 0;
                else if (Sql.Count == 1)
                    nBairro = Sql[0].Bairro;
                else {
                    foreach (var item in Sql) {
                        Num1 = Convert.ToInt32(item.Inicial);
                        Num2 = Convert.ToInt32(item.Final);
                        if (Numero >= Num1 && Numero <= Num2) {
                                nBairro = item.Bairro;
                                break;
                        } else if (Numero >= Num1 && Num2 == 0) {
                                nBairro = item.Bairro;
                                break;
                        }
                    }

                    
                }
                if (nBairro > 0) {
                    _bairro = (from e in db.Bairro where e.Siglauf=="SP" && e.Codcidade==413 &&  e.Codbairro == nBairro select e).FirstOrDefault();
                }

            }
            return _bairro;
        }

        public LogradouroStruct Retorna_Logradour_Cep(int Cep) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Cep
                           join l in db.Logradouro on c.Codlogr equals l.Codlogradouro into cl from l in cl.DefaultIfEmpty()
                           where c.cep == Cep select new { CodLogradouro = c.Codlogr, Endereco = l.Endereco }).FirstOrDefault();

                LogradouroStruct reg = new LogradouroStruct();
                if (Sql != null && Sql.CodLogradouro > 0) {
                    reg.CodLogradouro = Sql.CodLogradouro;
                    reg.Endereco = Sql.Endereco;
                };

                return reg;
            }
        }

        public bool Existe_Bairro(string uf,int cidade,string bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from i in db.Bairro
                           where i.Siglauf == uf && i.Codcidade==cidade && i.Descbairro==bairro select i.Codbairro).FirstOrDefault();
                if (reg==0)
                    return false;
                else
                    return true;
            }
        }

        public bool Existe_Cidade(string uf,  string cidade) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                var reg = (from c in db.Cidade
                           where c.Siglauf == uf && c.Desccidade == cidade select c.Codcidade).FirstOrDefault();
                if (reg == 0)
                    return false;
                else
                    return true;
            }
        }

        public Cepdb Retorna_CepDB(int Cep) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            using (GTI_Context db = new GTI_Context(_connection)) {
                Cepdb _cepdb = null;
                var sql= (from c in db.CepDB where c.Cep == _cep select c).FirstOrDefault();

                if (sql != null) {
                    _cepdb = new Cepdb() {
                        Cep = sql.Cep,
                        Uf = sql.Uf,
                        Cidadecodigo = sql.Cidadecodigo,
                        Bairrocodigo = sql.Bairrocodigo,
                        Logradouro = sql.Logradouro
                    };
                } else {
                    _cepdb = new Cepdb() {
                        Cep = Cep.ToString(),
                        Uf = "",
                        Cidadecodigo = 0,
                        Bairrocodigo = 0,
                        Logradouro = ""
                    };
                }

           //     _cepdb.Cidade = Retorna_Cidade(_cepdb.Uf, _cepdb.Cidadecodigo);
          //      _cepdb.Bairro = Retorna_Bairro(_cepdb.Uf, _cepdb.Cidadecodigo,_cepdb.Bairrocodigo);
                return _cepdb;
            }
        }

        public Cepdb Retorna_CepDB(int Cep,string Logradouro) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            Cepdb _cepdb = null;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var sql = (from c in db.CepDB where c.Cep == _cep  && c.Logradouro==Logradouro select c).FirstOrDefault();
                if (sql != null) {
                    _cepdb = new Cepdb() {
                        Cep = sql.Cep,
                        Uf = sql.Uf,
                        Cidadecodigo = sql.Cidadecodigo,
                        Bairrocodigo = sql.Bairrocodigo,
                        Logradouro = sql.Logradouro
                    };
                } else {
                    _cepdb = new Cepdb() {
                        Cep = Cep.ToString(),
                        Uf = "",
                        Cidadecodigo = 0,
                        Bairrocodigo = 0,
                        Logradouro = ""
                    };
                }

          //      _cepdb.Cidade = Retorna_Cidade(_cepdb.Uf, _cepdb.Cidadecodigo);
//_cepdb.Bairro = Retorna_Bairro(_cepdb.Uf, _cepdb.Cidadecodigo, _cepdb.Bairrocodigo);
                return _cepdb;
            }
        }

        public Uf Retorna_Cep_Estado(int Cep) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            int _cep1 =  Convert.ToInt32( _cep.Substring(0, 5) + "000");
            int _cep2 = Convert.ToInt32(_cep.Substring(0, 5) + "999");

            Uf reg = new Uf();

            using (GTI_Context db = new GTI_Context(_connection)) {
                var Sql = (from u in db.Uf orderby u.Cep1 select u).ToList();
                foreach (Uf item in Sql) {
                    if(_cep1>=item.Cep1 && _cep1 <= item.Cep2) {
                        reg.Siglauf = item.Siglauf;
                        reg.Descuf = item.Descuf;
                        reg.Cep1 = item.Cep1;
                        reg.Cep2 = item.Cep2;
                        break;
                    }
                }
            }

            return reg;
        }

        public List<string> Retorna_CepDB_Logradouro(int Cep) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.CepDB where c.Cep == _cep orderby c.Logradouro select c.Logradouro).ToList();
            }
        }

        public List<Cepdb> Retorna_CepDB_Logradouro_Codigo(int Cep) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.CepDB where c.Cep == Cep.ToString() orderby c.Logradouro select c).ToList();
            }
        }

        public List<Cepdb> Retorna_CepDB_Logradouro_Codigo(int Cep,int Bairro) {
            using (GTI_Context db = new GTI_Context(_connection)) {
                return (from c in db.CepDB where c.Cep == Cep.ToString() && c.Bairrocodigo==Bairro orderby c.Logradouro select c).ToList();
            }
        }


        public Cidade Retorna_CepDB_Cidade(int Cep) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            Cidade reg = null;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var sql= (from c in db.CepDB
                        join l in db.Cidade on new { p1 = c.Uf, p2 = (short)c.Cidadecodigo } equals new { p1 = l.Siglauf, p2 = l.Codcidade } into cl from l in cl.DefaultIfEmpty()
                        where c.Cep == _cep select new  { Siglauf = c.Uf, Codcidade = (short)c.Cidadecodigo, Desccidade = l.Desccidade }).FirstOrDefault();
                if (sql != null) {
                    reg = new Cidade() {
                        Siglauf = sql.Siglauf,
                        Codcidade = sql.Codcidade,
                        Desccidade = sql.Desccidade
                    };
                } else {
                    reg = new Cidade() {
                        Siglauf = "",
                        Codcidade = 0,
                        Desccidade = ""
                    };
                }

                return reg;
            }
        }

        public Bairro Retorna_CepDB_Bairro(int Cep) {
            string _cep = Cep.ToString().PadLeft(8, '0');
            Bairro reg = null;
            using (GTI_Context db = new GTI_Context(_connection)) {
                var sql= (from c in db.CepDB
                        join l in db.Bairro on c.Bairrocodigo equals l.Codbairro into cl from l in cl.DefaultIfEmpty()
                        where c.Cep == _cep select new  { Siglauf = c.Uf, Codcidade = (short)c.Cidadecodigo, CodBairro= (short)c.Bairrocodigo ,Descbairro = l.Descbairro }).FirstOrDefault();
                if (sql != null) {
                    reg = new Bairro() {
                        Siglauf = sql.Siglauf,
                        Codcidade = sql.Codcidade,
                        Codbairro = sql.CodBairro,
                        Descbairro = sql.Descbairro
                    };
                } else {
                    reg = new Bairro() {
                        Siglauf = "",
                        Codcidade = 0,
                        Codbairro = 0,
                        Descbairro = ""
                    };
                }
                return reg;
            }
        }

        public Exception Incluir_CepDB(Cepdb Reg) {
            using (var db = new GTI_Context(_connection)) {
                object[] Parametros = new object[7];
                Parametros[0] = new SqlParameter { ParameterName = "@cep", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Cep };
                Parametros[1] = new SqlParameter { ParameterName = "@uf", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Uf };
                Parametros[2] = new SqlParameter { ParameterName = "@cidadecodigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Cidadecodigo };
                Parametros[3] = new SqlParameter { ParameterName = "@bairrocodigo", SqlDbType = SqlDbType.Int, SqlValue = Reg.Bairrocodigo };
                Parametros[4] = new SqlParameter { ParameterName = "@logradouro", SqlDbType = SqlDbType.VarChar, SqlValue = Reg.Logradouro };
                Parametros[5] = new SqlParameter { ParameterName = "@func", SqlDbType = SqlDbType.Bit, SqlValue = Reg.Func };
                Parametros[6] = new SqlParameter { ParameterName = "@userid", SqlDbType = SqlDbType.Int, SqlValue = Reg.Userid };
                db.Database.ExecuteSqlCommand("INSERT INTO cepdb(cep,uf,cidadecodigo,bairrocodigo,logradouro,func,userid) " +
                                              " VALUES(@cep,@uf,@cidadecodigo,@bairrocodigo,@logradouro,@func,@userid)", Parametros);
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
