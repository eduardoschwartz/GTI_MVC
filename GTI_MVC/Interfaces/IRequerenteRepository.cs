using GTI_Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTI_Mvc.Interfaces {
    public interface IRequerenteRepository {
        CidadaoStruct Dados_Cidadao(int _codigo);
        bool Existe_Cidadao_Codigo(int nCodigo);
        bool Existe_Cidadao_Cpf(int Codigo, string Cpf);
        bool Existe_Cidadao_Cnpj(int Codigo, string Cnpj);
    }
}
