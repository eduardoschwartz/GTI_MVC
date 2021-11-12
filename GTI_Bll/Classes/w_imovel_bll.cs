using System;
using System.Collections.Generic;
using GTI_Models.Models;
using GTI_Dal.Classes;
using GTI_Bll.Classes;
using static GTI_Models.modelCore;

namespace GTI_Bll.Classes {
    public class W_Imovel_bll {
        private string _connection;

        public W_Imovel_bll(string sConnection) {
            _connection = sConnection;
        }

        public Exception Insert_W_Imovel_Main2(WImovel_Main Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_wimovel_main2(Reg);
            return ex;
        }

        public Exception Insert_W_Imovel_Main(string Guid, int Codigo, int UserId) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_wimovel_main(Guid, Codigo, UserId);
            return ex;
        }

        public WImovel_Main Retorna_Imovel_Main(string p) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            return obj.Retorna_Imovel_Main(p);
        }

        public Exception Update_W_Imovel_Main(WImovel_Main Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Update_wimovel_main(Reg);
            return ex;
        }

        public Exception Excluir_W_Imovel_Codigo(int Codigo) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Codigo(Codigo);
            return ex;
        }

        public Exception Insert_W_Imovel_Prop(WImovel_Prop Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_W_Imovel_Prop(Reg);
            return ex;
        }

        public Exception Excluir_W_Imovel_Prop_Codigo(string guid, int codigo) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Prop_Codigo(guid, codigo);
            return ex;
        }

        public Exception Excluir_W_Imovel_Prop_Guid(string guid) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Prop_Guid(guid);
            return ex;
        }

        public List<WImovel_Prop> Lista_WImovel_Prop(string p) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            return obj.Lista_WImovel_Prop(p);
        }

    }
}
