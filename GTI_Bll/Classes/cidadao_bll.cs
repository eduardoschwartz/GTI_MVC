﻿using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Cidadao_bll {

        private string _connection;
        public Cidadao_bll(string sConnection) {
            _connection = sConnection;
        }


        public List<Cidadao> Lista_Cidadao(string Nome, string Cpf, string CNPJ) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Cidadao(Nome,Cpf,CNPJ);
        }


        public CidadaoStruct LoadReg(Int32 nCodigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.LoadReg(nCodigo);
        }

        public Cidadao Retorna_Cidadao(int Codigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Retorna_Cidadao(Codigo);
        }

        public string Retorna_Nome_Cidadao(int Codigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Retorna_Nome_Cidadao(Codigo);
        }


        public int Retorna_Ultimo_Codigo_Cidadao() {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Retorna_Ultimo_Codigo_Cidadao();
        }

        public bool ExisteCidadao(int nCodigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.ExisteCidadao(nCodigo);
        }

        public int Incluir_cidadao(Cidadao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            int _cod = obj.Incluir_cidadao(reg);
            return _cod;
        }

        public Exception Alterar_cidadao(Cidadao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex= obj.Alterar_cidadao(reg);
            return ex;
        }

        public Exception Excluir_cidadao(int Codigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = Cidadao_em_uso(Codigo);
            if (ex == null)
                ex = obj.Excluir_cidadao(Codigo);
            return ex;
        }

        public Exception Excluir_Profissao(Profissao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = Profissao_em_uso(reg.Codigo);
            if (ex == null)
                ex = obj.Excluir_Profissao(reg);
            return ex;
        }

        private Exception Validated(Cidadao reg) {
            Exception AppEx;
            if (String.IsNullOrEmpty(reg.Nomecidadao)) {
                AppEx = new Exception("Digite o nome do cidadão");
                return AppEx;
            }

            if (!bllCore.IsEmptyDate(reg.Data_nascimento.ToString()) &&    !bllCore.IsDate(reg.Data_nascimento.ToString())) {
                AppEx = new Exception("Data de nascimento inválida.");
                return AppEx;
            }

            if (reg.Data_nascimento != null && reg.Data_nascimento >= DateTime.Now) {
                AppEx = new Exception("Data de nascimento inválida.");
                return AppEx;
            }

            return null;
        }

        public List<Profissao> Lista_Profissao() {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Profissao();
        }


        public List<Tipousuario> Lista_TipoCidadao() {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_TipoCidadao();
        }


        public Exception Incluir_Profissao(Profissao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = obj.Incluir_profissao(reg);
            return ex;
        }

        public Exception Alterar_Profissao(Profissao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = obj.Alterar_Profissao(reg);
            return ex;
        }


        private Exception Profissao_em_uso( int id_profissao) {
            Exception AppEx = null;
            Cidadao_Data obj = new Cidadao_Data(_connection);
            bool bcidadao = obj.Profissao_cidadao( id_profissao);
            if (bcidadao)
                AppEx = new Exception("Exclusão não permitida. Profissão em uso - cadastro cidadão.");
            return AppEx;
        }

        private Exception Cidadao_em_uso(int id_cidadao) {
            Exception AppEx = null;
            Processo_Data obj = new Processo_Data(_connection);
            bool bcidadao = obj.Cidadao_Processo(id_cidadao);
            if (bcidadao)
                AppEx = new Exception("Exclusão não permitida. cidadão em uso - processo.");
            return AppEx;
        }

        public List<Historico_CidadaoStruct> Lista_Historico(int CodigoCidadao) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Historico(CodigoCidadao);
        }

        public List<Observacao_CidadaoStruct> Lista_Observacao(int CodigoCidadao) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Observacao(CodigoCidadao);
        }

        public Exception Incluir_observacao_cidadao(obscidadao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = obj.Incluir_observacao_cidadao(reg);
            return ex;
        }

        public CidadaoStruct Dados_Cidadao(int _codigo) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Dados_Cidadao(_codigo);
        }

        public bool Existe_Cidadao_Cpf(int Codigo, string Cpf) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Existe_Cidadao_Cpf(Codigo,Cpf);
        }

        public bool Existe_Cidadao_Cnpj(int Codigo, string Cnpj) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Existe_Cidadao_Cnpj(Codigo, Cnpj);
        }

        public int Existe_Cidadao_Cpf(string Cpf) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Existe_Cidadao_Cpf( Cpf);
        }

        public int Existe_Cidadao_Cnpj(string Cnpj) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Existe_Cidadao_Cnpj(Cnpj);
        }

        public int Incluir_Cidadao_Itbi(Cidadao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Incluir_Cidadao_Itbi(reg);
        }

        public Exception Incluir_Historico_Cidadao(historicocidadao reg) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            Exception ex = obj.Incluir_Historico_Cidadao(reg);
            return ex;
        }

        public int Novo_Codigo_Cidadao(int OldCod) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Novo_Codigo_Cidadao(OldCod);
        }

        public List<CidadaoHeader> Lista_Cidadao_Socio(int Codigo_Empresa) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Cidadao_Socio(Codigo_Empresa);
        }

        public List<Cidadao> Lista_Cidadao(int Codigo, string Nome, string CpfCnpj, int maxRows) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Lista_Cidadao(Codigo,Nome,CpfCnpj,maxRows);
        }

        public string Retorna_Profissao_Nome(int id_profissao) {
            Cidadao_Data obj = new Cidadao_Data(_connection);
            return obj.Retorna_Profissao_Nome(id_profissao);
        }

    }
}
