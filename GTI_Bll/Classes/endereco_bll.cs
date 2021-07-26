using System;
using System.Collections.Generic;
using GTI_Models.Models;
using GTI_Dal.Classes;

namespace GTI_Bll.Classes {
    public class Endereco_bll {

        private string _connection;
        public Endereco_bll(string sConnection) {
            _connection = sConnection;
        }

        public List<Cidade> Lista_Cidade(string UF) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Lista_Cidade(UF);
        }

        public List<Uf> Lista_UF() {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Lista_UF();
        }

        public List<Logradouro> Lista_Logradouro(String Filter = "") {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Lista_Logradouro(Filter);
        }

        public List<Bairro> Lista_Bairro(string UF, int cidade) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Lista_Bairro(UF, cidade);
        }

        public int Incluir_bairro(Bairro reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Incluir_bairro(reg);
        }

        public string Retorna_UfNome(string UF) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_UfNome(UF);
        }

        public Exception Alterar_Bairro(Bairro reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex= obj.Alterar_Bairro(reg);
            return ex;
        }

        public Exception Excluir_Bairro(Bairro reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex = Bairro_em_uso(reg.Siglauf, reg.Codcidade,reg.Codbairro);
            if (ex == null) 
                ex =obj.Excluir_Bairro(reg);
            return ex;
        }

        private Exception Bairro_em_uso(string id_UF, int id_cidade,int id_bairro) {
            Exception AppEx=null;
            Endereco_Data obj = new Endereco_Data(_connection);
            bool bcidadao = obj.Bairro_uso_cidadao(id_UF, id_cidade, id_bairro);
            if(bcidadao)
                AppEx = new Exception("Exclusão não permitida. Bairro em uso - cadastro cidadão.");
            else {
                bool bempresa = obj.Bairro_uso_empresa(id_UF, id_cidade, id_bairro);
                if (bempresa)
                    AppEx = new Exception("Exclusão não permitida. Bairro em uso - cadastro mobiliário.");
                else {
                    bool bprocesso = obj.Bairro_uso_processo(id_UF, id_cidade, id_bairro);
                    if (bempresa)
                        AppEx = new Exception("Exclusão não permitida. Bairro em uso - processo.");
                }
            }
            return AppEx;
        }

        public List<Pais> Lista_Pais() {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Lista_Pais();
        }

        public Exception Incluir_Pais(Pais reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex = obj.Incluir_Pais(reg);
            return ex;
        }

        public Exception Alterar_Pais(Pais reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex = obj.Alterar_Pais(reg);
            return ex;
        }

        public Exception Excluir_Pais(Pais reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex = Pais_em_uso(reg.Id_pais);
            if(ex==null)
                ex = obj.Excluir_Pais(reg);
            return ex;
        }

        private Exception Pais_em_uso(int id_pais) {
            Exception AppEx = null;
            Endereco_Data obj = new Endereco_Data(_connection);
            bool bcidadao = obj.Pais_uso_cidadao(id_pais);
            if (bcidadao)
                AppEx = new Exception("Exclusão não permitida. País em uso - cadastro cidadão.");
            return AppEx;
        }

        public string Retorna_Pais(int Codigo) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Pais(Codigo);
        }

        public string Retorna_Logradouro(int Codigo) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Logradouro(Codigo);
        }

        public string Retorna_Cidade(string UF, int Codigo) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Cidade(UF,Codigo);
        }

        public string Retorna_Bairro(string UF, int Cidade, int Bairro) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Bairro(UF, Cidade,Bairro);
        }

        public int Retorna_Bairro(string UF, int Cidade, string Bairro) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Bairro(UF, Cidade, Bairro);
        }

        public int RetornaCep(int CodigoLogradouro, short Numero) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.RetornaCep(CodigoLogradouro, Numero);
        }

        public Bairro RetornaLogradouroBairro(int CodigoLogradouro, short Numero) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.RetornaLogradouroBairro(CodigoLogradouro, Numero);
        }

        public LogradouroStruct Retorna_Logradouro_Cep(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Logradour_Cep(Cep);
        }

        public int Retorna_Cidade(string UF, string Cidade) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Cidade(UF,Cidade);
        }

        public bool Existe_Bairro(string uf, int cidade, string bairro) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Existe_Bairro(uf, cidade,bairro);
        }

        public Cepdb Retorna_CepDB(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB(Cep);
        }

        public Cepdb Retorna_CepDB(int Cep, string Logradouro) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB(Cep,Logradouro);
        }

        public Uf Retorna_Cep_Estado(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_Cep_Estado(Cep);
        }

        public List<string> Retorna_CepDB_Logradouro(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB_Logradouro(Cep);
        }

        public List<Cepdb> Retorna_CepDB_Logradouro_Codigo(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB_Logradouro_Codigo(Cep);
        }

        public Exception Incluir_CepDB(Cepdb Reg) {
            Endereco_Data obj = new Endereco_Data(_connection);
            Exception ex = obj.Incluir_CepDB(Reg);
            return ex;
        }

        public Cidade Retorna_CepDB_Cidade(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB_Cidade(Cep);
        }

        public Bairro Retorna_CepDB_Bairro(int Cep) {
            Endereco_Data obj = new Endereco_Data(_connection);
            return obj.Retorna_CepDB_Bairro(Cep);
        }


    }
}

