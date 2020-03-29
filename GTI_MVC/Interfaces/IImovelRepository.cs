using GTI_Mvc.Models;
using System;
using System.Collections.Generic;

namespace GTI_Mvc.Interfaces {
    public interface IImovelRepository {
    //    SpCalculo Calculo_IPTU(int Codigo, int Ano);
        Laseriptu Dados_IPTU(int Codigo, int Ano);
        List<Laseriptu> Dados_IPTU(int Codigo);
        ImovelStruct Dados_Imovel(int nCodigo);
        bool Existe_Imovel(int nCodigo);
        bool Existe_Imovel_Cpf(int Codigo, string Cpf);
        bool Existe_Imovel_Cnpj(int Codigo, string Cnpj);
        List<ProprietarioStruct> Lista_Proprietario(int CodigoImovel, bool Principal = false);
        int Qtde_Imovel_Cidadao(int CodigoImovel);
        List<IsencaoStruct> Lista_Imovel_Isencao(int Codigo, int Ano = 0);
        decimal Soma_Area(int Codigo);
        bool Verifica_Imunidade(int Codigo);
    }
}
