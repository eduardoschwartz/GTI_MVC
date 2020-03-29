using GTI_Mvc.Models;
using GTI_Mvc.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using static GTI_Mvc.Functions;

namespace GTI_Mvc.Repository {
    public class SistemaRepository:ISistemaRepository {
        private readonly AppDbContext context;

        public SistemaRepository(AppDbContext context) {
            this.context = context;
        }

        public Exception Alterar_Senha(Usuario reg) {
                string sLogin = reg.Nomelogin;
                Usuario b = context.Usuario.First(i => i.Nomelogin == sLogin);
                b.Senha2 = reg.Senha2;
                try {
                    context.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
        }

        public List<usuarioStruct> Lista_Usuarios() {
                var reg = (from t in context.Usuario
                           join cc in context.Centrocusto on t.Setor_atual equals cc.Codigo into tcc from cc in tcc.DefaultIfEmpty()
                           orderby t.Nomecompleto select new { t.Nomelogin, t.Nomecompleto, t.Ativo, t.Id, t.Senha,t.Senha2, t.Setor_atual, cc.Descricao }).ToList();
                List<usuarioStruct> Lista = new List<usuarioStruct>();
                foreach (var item in reg) {
                    usuarioStruct Linha = new usuarioStruct {
                        Nome_login = item.Nomelogin,
                        Nome_completo = item.Nomecompleto,
                        Ativo = item.Ativo,
                        Id = item.Id,
                        Senha = item.Senha,
                        Senha2=item.Senha2,
                        Setor_atual = item.Setor_atual,
                        Nome_setor = item.Descricao
                    };
                    Lista.Add(Linha);
                }
                return Lista;
        }

        public usuarioStruct Retorna_Usuario(int Id) {
                var reg = (from t in context.Usuario
                           where t.Id == Id
                           orderby t.Nomelogin select new usuarioStruct {
                               Nome_login = t.Nomelogin, Nome_completo = t.Nomecompleto, Ativo = t.Ativo,
                               Id = t.Id, Senha = t.Senha, Senha2 = t.Senha2
                           }).FirstOrDefault();
                usuarioStruct Sql = new usuarioStruct {
                    Id = reg.Id,
                    Nome_completo = reg.Nome_completo,
                    Nome_login = reg.Nome_login,
                    Senha = reg.Senha,
                    Senha2 = reg.Senha2,
                    Ativo = reg.Ativo
                };
                return Sql;
        }

        public int Retorna_User_LoginId(string loginName) {
            int _userId = 0;
            try {
                var Sql = (from u in context.Usuario where u.Nomelogin == loginName select u.Id).FirstOrDefault();
                _userId = Convert.ToInt32(Sql);
            } catch  {
            }
                
            return _userId;
        }

        public string Retorna_User_FullName(string loginName) {
            string Sql = (from u in context.Usuario where u.Nomelogin == loginName select u.Nomecompleto).FirstOrDefault();
            return Sql;
        }

        public string Retorna_User_FullName(int idUser) {
            string Sql = (from u in context.Usuario where u.Id == idUser select u.Nomecompleto).FirstOrDefault();
            return Sql;
        }

        public string Retorna_User_LoginName(string fullName) {
            string Sql = (from u in context.Usuario where u.Nomecompleto == fullName select u.Nomelogin).FirstOrDefault();
            return Sql;
        }

        public string Retorna_User_LoginName(int idUser) {
            string Sql = (from u in context.Usuario where u.Id == idUser select u.Nomelogin).FirstOrDefault();
            return Sql;
        }

    }

}
