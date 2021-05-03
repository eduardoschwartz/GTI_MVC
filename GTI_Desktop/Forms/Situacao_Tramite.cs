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
            int Assunto = processoRepository.Retorna_Processo_Assunto(Ano, Numero);

            lvMain.Items.Clear();
            gtiCore.Ocupado(this);
            
            List<TramiteStruct> Lista = processoRepository.DadosTramite(Ano, Numero,Assunto );

            foreach (TramiteStruct Reg in Lista) {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add(Reg.Seq.ToString("00"));
                lvi.SubItems.Add(Reg.CentroCustoCodigo.ToString());
                lvi.SubItems.Add(Reg.CentroCustoNome ?? "");
                lvi.SubItems.Add(Reg.DataEntrada ?? "");
                lvi.SubItems.Add(Reg.HoraEntrada ?? "");
                lvi.SubItems.Add(Reg.Usuario1 ?? "");
                lvi.SubItems.Add(Reg.DespachoNome ?? "");
                lvi.SubItems.Add("0");
                lvi.SubItems.Add(Reg.DataEnvio ?? "");
                lvi.SubItems.Add(Reg.Usuario2 ?? "");
                lvi.SubItems.Add(Reg.Obs ?? "");
                lvi.Tag = Reg.Obs ?? "";
                if (!string.IsNullOrEmpty(Reg.Obs)) lvi.ImageIndex = 0;
                lvMain.Items.Add(lvi);
            }

            Verificar_Processo(Ano,Numero);
            gtiCore.Liberado(this);


        }

        private void Limpa() {
            LocalCodigoText.Text = "0";
            LocalNomeText.Text = "...";
            ArquivadoText.Text = "Não";
            SuspensoText.Text = "Não";
            DataText.Text = "__/__/____";
        }


        private Local_Tramite Verificar_Processo(short Ano,int Numero) {
            Local_Tramite lt = new Local_Tramite() {
                Ano=Ano,
                Numero=Numero
            };

            List<Lista_Tramitacao> _listaTramitacao = new List<Lista_Tramitacao>();
            Processo_bll processoRepository = new Processo_bll(_connection);
            int Assunto = processoRepository.Retorna_Processo_Assunto(Ano, Numero);
            List<TramiteStruct> ListaTramite = processoRepository.DadosTramite(Ano, Numero, Assunto);

            foreach (TramiteStruct Reg in ListaTramite) {
                Lista_Tramitacao _reg = new Lista_Tramitacao(){ 
                    Seq=Reg.Seq,
                    CentroCusto_Codigo=Reg.CentroCustoCodigo,
                    CentroCusto_Nome=Reg.CentroCustoNome,
                    Usuario1=Reg.Usuario1,
                    Usuario2=Reg.Usuario2
                };
                if (!string.IsNullOrEmpty( Reg.DataEntrada )) {
                    _reg.Data_Entrada = Convert.ToDateTime( Reg.DataEntrada);
                }
                if (!string.IsNullOrEmpty( Reg.DataEnvio)) {
                    _reg.Data_Envio = Convert.ToDateTime(Reg.DataEnvio);
                }
                _listaTramitacao.Add(_reg);
            }



            return lt;
        
        }


    }

    class Lista_Tramitacao {
        public int Seq { get; set; }
        public int  CentroCusto_Codigo { get; set; }
        public string CentroCusto_Nome { get; set; }
        public DateTime? Data_Entrada { get; set; }
        public string Usuario1 { get; set; }
        public DateTime? Data_Envio { get; set; }
        public string Usuario2 { get; set; }
    }


    class Local_Tramite {
        public short Ano { get; set; }
        public int Numero { get; set; }
        public int  Local_Codigo { get; set; }
        public string Local_Nome { get; set; }
        public bool Arquivado { get; set; }
        public bool Suspenso { get; set; }
        public DateTime Data_Evento { get; set; }
    }

}
