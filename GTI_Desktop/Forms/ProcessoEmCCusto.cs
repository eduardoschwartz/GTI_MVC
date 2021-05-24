using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static GTI_Desktop.Classes.GtiTypes;

namespace GTI_Desktop.Forms {
    public partial class ProcessoEmCCusto : Form {
        private string _connection = gtiCore.Connection_Name();
        public ProcessoEmCCusto() {
            InitializeComponent();
        }

        private void ProcessoEmCCusto_Load(object sender, EventArgs e) {
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Centrocusto> _lista = processoRepository.Lista_Local(true, false);
            List<CustomListBoxItem> myItems = new List<CustomListBoxItem>();
            foreach (Centrocusto item in _lista) {
                    myItems.Add(new CustomListBoxItem(item.Descricao, item.Codigo));
            }

            LocalList.DisplayMember = "_name";
            LocalList.ValueMember = "_value";
            LocalList.DataSource = myItems;

            LocalList.SelectedIndex = 0;
        }

        private void PrintButton_Click(object sender, EventArgs e) {
            CustomListBoxItem _item = (CustomListBoxItem)LocalList.SelectedItem;
            int _local = _item._value;
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<Processogti> _lista = processoRepository.Lista_Processos_CCusto(_local);

            TextWriter tw = new StreamWriter( "d:\\tmp\\lista.txt");

            List<Rel> _listaRel = new List<Rel>();
            int _pos=1,_total=_lista.Count;
            foreach (Processogti reg in _lista) {

                Local_Tramite lt = processoRepository.Verificar_Processo(reg.Ano, reg.Numero);
                DateTime _data = lt.Data_Evento;
                if (_data == DateTime.MinValue) {
                    _data = reg.Dataentrada;
                }

                bool _find = false;
                if (lt.Local_Codigo == _local) {
                    foreach(Rel check in _listaRel) {
                        if(check.Ano == reg.Ano && check.Numero == reg.Numero) {
                            _find = true;
                        }
                    }
                    if(!_find) {
                        short ret = Convert.ToInt16(processoRepository.DvProcesso(reg.Numero));
                        string _row = reg.Numero + "-" + ret.ToString() + "/" + reg.Ano.ToString();
                        tw.WriteLine(_row);
                        Rel _r = new Rel() {
                            Ano=reg.Ano,
                            Numero=reg.Numero
                        };
                        _listaRel.Add(_r);
                    }
                }

                if (_pos % 10 == 0) {
                    PBar.Value = _pos * 100 / _total;
                    PBar.Update();
                    System.Windows.Forms.Application.DoEvents();
                }
                _pos++;

            }


            tw.Close();
            MessageBox.Show("Fim");

        }
    }

      class Rel {
        public int Ano { get; set; }
        public int Numero { get; set; }
    }
}
