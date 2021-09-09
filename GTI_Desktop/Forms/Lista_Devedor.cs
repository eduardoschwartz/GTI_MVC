using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTI_Desktop.Forms {
    public partial class Lista_Devedor : Form {
        string _connection = gtiCore.Connection_Name();
        public Lista_Devedor() {
            InitializeComponent();
        }

        private void Lista_Devedor_Load(object sender, EventArgs e) {
            
        }

        private void FindButton_Click(object sender, EventArgs e) {
            bool _ExisteReg = false;
            int _codigo1 = 1,_codigo2=5000;
            int _userId = Properties.Settings.Default.UserId;
            short _ano=0;
            DateTime _data_vencimento = DateTime.Now.Date;

            Tributario_bll tributarioRepository = new Tributario_bll(_connection);
            Exception ex = tributarioRepository.Excluir_Lista_Devedor(_userId);

            List<int> _lista_codigo = tributarioRepository.Lista_Codigo_Devedor(_codigo1, _codigo2,_data_vencimento);
            if (_lista_codigo.Count == 0) {
                MessageBox.Show("Não existem débitos com estes parâmentros.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int _pos = 1;
            foreach (int _cod in _lista_codigo) {
                List<SpExtrato_carta> Lista_Extrato_Tributo = tributarioRepository.Lista_Extrato_Tributo_Devedor(_cod,2016,2017,DateTime.Now,"GTI");
                List<SpExtrato_carta> Lista_Extrato_Parcela = tributarioRepository.Lista_Extrato_Parcela_Carta(Lista_Extrato_Tributo);
                decimal _soma2016 = 0,_soma2017=0;
                for (int i = 0; i < Lista_Extrato_Parcela.Count; i++) {
                    _ano = Lista_Extrato_Parcela[i].Anoexercicio;                  
                    if(_ano==2016)
                        _soma2016 += Lista_Extrato_Parcela[i].Valortotal;
                    else
                        _soma2017 += Lista_Extrato_Parcela[i].Valortotal;
                }
                if (_soma2016 > 0) {
                    _ExisteReg = true;
                    Lista_devedor reg = new Lista_devedor() {
                        Userid = _userId,
                        Codigo = _cod,
                        Ano = 2016,
                        valor_total = _soma2016
                    };
                    ex = tributarioRepository.Insert_Lista_devedor(reg);
                }
                if (_soma2017 > 0) {
                    _ExisteReg = true;
                    Lista_devedor reg = new Lista_devedor() {
                        Userid = _userId,
                        Codigo = _cod,
                        Ano = 2017,
                        valor_total = _soma2017
                    };
                    ex = tributarioRepository.Insert_Lista_devedor(reg);
                }
                PBar.Value = _pos * 100 / _lista_codigo.Count;
                _pos++;
            }

            
            if (!!_ExisteReg) {
                MessageBox.Show("Não existem débitos com estes parâmentros.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }else
                MessageBox.Show("FIM.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}
