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

        public Exception Insert_W_Imovel_Endereco(WImovel_Endereco Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_W_Imovel_Endereco(Reg);
            return ex;
        }

        public WImovel_Endereco Retorna_Imovel_Endereco(string p) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            return obj.Retorna_Imovel_Endereco(p);
        }

        public Exception Excluir_W_Imovel_Endereco(string guid) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Endereco(guid);
            return ex;
        }

        public Exception Insert_W_Imovel_Testada(WImovel_Testada Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_W_Imovel_Testada(Reg);
            return ex;
        }

        public List<WImovel_Testada> Lista_WImovel_Testada(string p) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            return obj.Lista_WImovel_Testada(p);
        }

        public Exception Excluir_W_Imovel_Testada_Guid(string guid) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Testada_Guid(guid);
            return ex;
        }

        public Exception Excluir_W_Imovel_Testada_Codigo(string guid, int codigo) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Testada_Codigo(guid,codigo);
            return ex;
        }

        public Exception Insert_W_Imovel_Area(WImovel_Area Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_W_Imovel_Area(Reg);
            return ex;
        }

        public List<WImovel_Area> Lista_WImovel_Area(string p) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            return obj.Lista_WImovel_Area(p);
        }

        public Exception Excluir_W_Imovel_Area_Guid(string guid) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Area_Guid(guid);
            return ex;
        }

        public Exception Excluir_W_Imovel_Area_Codigo(string guid, int codigo) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Excluir_W_Imovel_Area_Codigo(guid,codigo);
            return ex;
        }

    }
}
