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

        public Exception Insert_W_Imovel_Main(W_Imovel_Main Reg) {
            w_imovel_Data obj = new w_imovel_Data(_connection);
            Exception ex = obj.Insert_W_Imovel_Main(Reg);
            return ex;
        }

    }
}
