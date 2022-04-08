using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GTI_Desktop.Forms {
    public partial class Processos_Email : Form {
        string _connection = gtiCore.Connection_Name();
        public Processos_Email() {
            InitializeComponent();
        }

        private void GerarButton_Click(object sender, EventArgs e) {
            MainListView.Items.Clear();
            string _d1 = DataInicioMask.Text;
            string _d2 = DataFinalMask.Text;

            if (!gtiCore.IsDate(_d1)) {
                MessageBox.Show("Data inicial inválida!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!gtiCore.IsDate(_d2)) {
                MessageBox.Show("Data final inválida!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime _dataInicio = Convert.ToDateTime(_d1);
            DateTime _dataFinal = Convert.ToDateTime(_d2);
            

            if (_dataInicio > _dataFinal) {
                MessageBox.Show("Data inicial maior que a data final!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            gtiCore.Ocupado(this);
            Processo_bll processoRepository = new Processo_bll(_connection);
            List<ProcessoAnoNumero> Lista = processoRepository.Lista_Processos_Atraso(_dataInicio, _dataFinal);
            int _total = Lista.Count();
            if (_total == 0) {
                gtiCore.Liberado(this);
                MessageBox.Show("Nenhum processo em atraso no período informado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<int> listaSecretariaRel = new List<int>();
            int _pos = 0;
            List<Local_Tramite> _listaProcessos = new List<Local_Tramite>();
            foreach (ProcessoAnoNumero item in Lista) {
                short _ano = item.Ano;
                int _numero = item.Numero;

                Local_Tramite lt = processoRepository.Verificar_Processo(_ano, _numero);
                DateTime? _data = lt.Data_Evento;
                if (_data == DateTime.MinValue) {
                    _data = lt.Data_Entrada;
                }

                int Local_Codigo = lt.Local_Codigo;
                DateTime Data_Evento = Convert.ToDateTime(_data);
                bool Arquivado = lt.Arquivado;
                bool Suspenso = lt.Suspenso;
                string _assunto = lt.Assunto_Nome;
                int NumDias = lt.Dias;

                if (NumDias < 5) goto Proximo;

                if (!Arquivado && !Suspenso) {
                    Tuple<short, string> Secretaria = processoRepository.Retorna_Vinculo_Top_CentroCusto((short)Local_Codigo);
                    int secretaria_codigo = Secretaria.Item1;
                    string secretaria_nome = Secretaria.Item2;

                    Local_Tramite reg = new Local_Tramite() {
                        Ano = _ano,
                        Numero = _numero,
                        Secretaria_Codigo = secretaria_codigo,
                        Secretaria_Nome = secretaria_nome,
                        Local_Codigo = Local_Codigo,
                        Local_Nome = lt.Local_Nome,
                        Data_Evento = Data_Evento,
                        Dias = NumDias,
                        Assunto_Nome = _assunto
                    };
                    _listaProcessos.Add(reg);

                    bool _find = false;
                    for (int i = 0; i < listaSecretariaRel.Count; i++) {
                        if (listaSecretariaRel[i] == secretaria_codigo) break;
                    }
                    if (!_find)
                        listaSecretariaRel.Add(secretaria_codigo);

                }

                if (_pos % 10 == 0) {
                    pBar.Value = _pos * 100 / _total;
                    pBar.Update();
                    Refresh();
                }
            Proximo:;
                _pos++;

            }
            pBar.Value = 0;
            pBar.Update();
            Refresh();

            List<Secretaria> listaSecretaria = processoRepository.Lista_Secretaria();
            foreach (Local_Tramite reg in _listaProcessos) {
                short _cod2 = 0;
                for (int i = 0; i < listaSecretaria.Count; i++) {
                    if (listaSecretaria[i].Codigocc == reg.Secretaria_Codigo) {
                        _cod2 = listaSecretaria[i].Codigo;
                        break;
                    }
                }
                string _processo = reg.Numero.ToString() + "-" + processoRepository.DvProcesso(reg.Numero).ToString() + "/" + reg.Ano.ToString();
                ListViewItem lvi = new ListViewItem(_processo);
                lvi.SubItems.Add(reg.Local_Codigo.ToString("000"));
                lvi.SubItems.Add(reg.Local_Nome);
                lvi.SubItems.Add(reg.Secretaria_Codigo.ToString("000"));
                lvi.SubItems.Add(_cod2.ToString("000"));
                lvi.SubItems.Add(reg.Secretaria_Nome);
                lvi.SubItems.Add(reg.Data_Evento.ToString("dd/MM/yyyy"));
                lvi.SubItems.Add(reg.Dias.ToString());
                lvi.SubItems.Add(reg.Assunto_Nome);
                
                
                MainListView.Items.Add(lvi);
            }

            gtiCore.Liberado(this);

        }
    }



}
