using GTI_Mvc.Models;
using System;
using System.Collections.Generic;

namespace GTI_Mvc.Interfaces {
    public interface ISistemaRepository {
        Exception Alterar_Senha(Usuario reg);
        List<usuarioStruct> Lista_Usuarios();
        usuarioStruct Retorna_Usuario(int Id);
        int Retorna_User_LoginId(string loginName);
        string Retorna_User_FullName(string loginName);
        string Retorna_User_FullName(int idUser);
        string Retorna_User_LoginName(string fullName);
        string Retorna_User_LoginName(int idUser);
    }
}
