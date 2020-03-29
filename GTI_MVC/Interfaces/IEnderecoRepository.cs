using System;

namespace GTI_Mvc.Interfaces {
    public interface IEnderecoRepository {
        int RetornaCep(int CodigoLogradouro, short Numero);

    }
}
