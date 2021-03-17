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

                Tributario_Data tributarioRepository = new Tributario_Data("GTIconnection");
                List<SpExtrato_Parcelamento> _listaTributo = tributarioRepository.Lista_Extrato_Tributo_Parcelamento(Codigo);
                List<SpExtrato_Parcelamento> _extrato = tributarioRepository.Lista_Extrato_Parcela_Parcelamento(_listaTributo);


                return ListaOrigem;
            }
        }



        //public List<SpParcelamentoOrigem> Lista_Parcelamento_Origem(int Codigo,char Tipo) {

        //    using (GTI_Context db = new GTI_Context(_connection)) {
        //        db.Database.CommandTimeout = 600;
        //        object[] Parametros = new object[12];
        //        Parametros[0] = new SqlParameter { ParameterName = "@codigo", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
        //        Parametros[1] = new SqlParameter { ParameterName = "@tipo", SqlDbType = SqlDbType.Char, SqlValue = Tipo };
        //        Parametros[2] = new SqlParameter { ParameterName = "@juros", SqlDbType = SqlDbType.Bit, SqlValue = 1 };
        //        Parametros[3] = new SqlParameter { ParameterName = "@multa", SqlDbType = SqlDbType.Bit, SqlValue = 1 };
        //        Parametros[4] = new SqlParameter { ParameterName = "@correcao", SqlDbType = SqlDbType.Bit, SqlValue = 1 }; 
        //        Parametros[5] = new SqlParameter { ParameterName = "@dataatualiza", SqlDbType = SqlDbType.SmallDateTime, SqlValue = DateTime.Now };
        //        var result = db.spParcelamentoOrigem.SqlQuery("EXEC spParcelamentoOrigem @codigo, @tipo, @juros ,@multa, @correcao, @dataatualiza",Parametros).ToList();

        //        List<SpParcelamentoOrigem> ListaOrigem = new List<SpParcelamentoOrigem>();
        //        int _x = 1;
        //        foreach (SpParcelamentoOrigem item in result) {
        //            SpParcelamentoOrigem reg = new SpParcelamentoOrigem {
        //                Idx=_x,
        //                Exercicio=item.Exercicio,
        //                Lancamento=item.Lancamento,
        //                Nome_lancamento=item.Nome_lancamento,
        //                Sequencia=item.Sequencia,
        //                Parcela=item.Parcela,
        //                Complemento=item.Complemento,
        //                Data_vencimento=item.Data_vencimento,
        //                Ajuizado=item.Ajuizado,
        //                Valor_principal=item.Valor_principal,
        //                Valor_juros=item.Valor_juros,
        //                Valor_multa=item.Valor_multa,
        //                Valor_correcao=item.Valor_correcao,
        //                Valor_total=item.Valor_total,
        //                Valor_penalidade=item.Valor_penalidade,
        //                Perc_penalidade=item.Perc_penalidade,
        //                Qtde_parcelamento=item.Qtde_parcelamento
        //            };
        //            _x++;
        //            ListaOrigem.Add(reg);
        //        }
        //        return ListaOrigem;
        //    }
        //}



    }
}
