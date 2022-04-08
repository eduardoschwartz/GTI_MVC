using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GTI_Desktop.Forms {
    public partial class Situacao_Tramite : Form {
        private string _connection = gtiCore.Connection_Name();

        public Situacao_Tramite() {
            InitializeComponent();
        }

        private void Situacao_Tramite_Load(object sender, EventArgs e) {

        }

        private void ConsultarButton_Click(object sender, EventArgs e) {
            Processo_bll processoRepository = new Processo_bll(_connection);
            Limpa();
            int Numero = Convert.ToInt32(NumeroText.Text);
            short Ano = Convert.ToInt16(AnoText.Text);
            //Processogti _proc = processoRepository.Retorna_ProcessoGti(Ano, Numero);

            //lvMain.Items.Clear();
            //gtiCore.Ocupado(this);
            
            //List<TramiteStruct> Lista = processoRepository.DadosTramite(Ano, Numero,_proc.Codassunto );

            //foreach (TramiteStruct Reg in Lista) {
            //    ListViewItem lvi = new ListViewItem();
            //    lvi.SubItems.Add(Reg.Seq.ToString("00"));
            //    lvi.SubItems.Add(Reg.CentroCustoCodigo.ToString());
            //    lvi.SubItems.Add(Reg.CentroCustoNome ?? "");
            //    lvi.SubItems.Add(Reg.DataEntrada ?? "");
            //    lvi.SubItems.Add(Reg.HoraEntrada ?? "");
            //    lvi.SubItems.Add(Reg.Usuario1 ?? "");
            //    lvi.SubItems.Add(Reg.DespachoNome ?? "");
            //    lvi.SubItems.Add("0");
            //    lvi.SubItems.Add(Reg.DataEnvio ?? "");
            //    lvi.SubItems.Add(Reg.Usuario2 ?? "");
            //    lvi.SubItems.Add(Reg.Obs ?? "");
            //    lvi.Tag = Reg.Obs ?? "";
            //    if (!string.IsNullOrEmpty(Reg.Obs)) lvi.ImageIndex = 0;
            //    lvMain.Items.Add(lvi);
            //}

            Local_Tramite lt= processoRepository.Verificar_Processo(Ano,Numero);
            DateTime _data = lt.Data_Evento;
            if(_data == DateTime.MinValue) {
                ProcessoStruct _proc = processoRepository.Retorna_ProcessoGti(Ano, Numero);
                _data = Convert.ToDateTime(_proc.DataEntrada);
            }

            LocalCodigoText.Text = lt.Local_Codigo.ToString();
            DataText.Text = _data.ToString("dd/MM/yyyy");
            ArquivadoText.Text = lt.Arquivado ? "Sim" : "Não";
            SuspensoText.Text = lt.Suspenso ? "Sim" : "Não";
            if(lt.Arquivado)
                LocalNomeText.Text = lt.Local_Nome;
//            LocalNomeText.Text = "PROCESSO ARQUIVADO";
            else {
                if (lt.Suspenso) {
                    LocalNomeText.Text = "PROCESSO SUSPENSO/CANCELADO";
                } else {
                    LocalNomeText.Text = lt.Local_Nome;
                }
            }
            
            gtiCore.Liberado(this);

        }

        private void Limpa() {
            LocalCodigoText.Text = "0";
            LocalNomeText.Text = "...";
            ArquivadoText.Text = "Não";
            SuspensoText.Text = "Não";
            DataText.Text = "__/__/____";
        }
               




    }


}
