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
            Processogti _proc = processoRepository.Retorna_ProcessoGti(Ano, Numero);

            lvMain.Items.Clear();
            gtiCore.Ocupado(this);
            
            List<TramiteStruct> Lista = processoRepository.DadosTramite(Ano, Numero,_proc.Codassunto );

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

            Local_Tramite lt= Verificar_Processo(Ano,Numero);
            LocalCodigoText.Text = lt.Local_Codigo.ToString();
            DataText.Text = lt.Data_Evento==DateTime.MinValue?_proc.Dataentrada.ToString("dd/MM/yyyy"):   lt.Data_Evento.ToString("dd/MM/yyyy");
            ArquivadoText.Text = lt.Arquivado ? "Sim" : "Não";
            SuspensoText.Text = lt.Suspenso ? "Sim" : "Não";
            if(lt.Arquivado)
                LocalNomeText.Text = "PROCESSO ARQUIVADO";
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


        private Local_Tramite Verificar_Processo(short Ano,int Numero) {
            Local_Tramite lt = new Local_Tramite() {
                Ano=Ano,
                Numero=Numero
            };

            List<Lista_Tramitacao> _listaTramitacao = new List<Lista_Tramitacao>();
            Processo_bll processoRepository = new Processo_bll(_connection);
            Processogti _proc = processoRepository.Retorna_ProcessoGti(Ano, Numero);

            if (_proc.Dataarquiva != null) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = true;
                lt.Suspenso = false;
                lt.Data_Evento = Convert.ToDateTime(_proc.Dataarquiva);
                return lt;
            } 
            if (_proc.Datasuspenso != null ) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = false;
                lt.Suspenso = true;
                lt.Data_Evento = Convert.ToDateTime(_proc.Datasuspenso);
                return lt;
            }
            if (_proc.Datacancel != null) {
                lt.Local_Codigo = 0;
                lt.Local_Nome = "";
                lt.Arquivado = false;
                lt.Suspenso = true;
                lt.Data_Evento = Convert.ToDateTime(_proc.Datacancel);
                return lt;
            }

            List<TramiteStruct> ListaTramite = processoRepository.DadosTramite(Ano, Numero, _proc.Codassunto);

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

            int _rows = _listaTramitacao.Count;
            //1º caso, a tabela possui apenas 1 linha, neste caso o processo estara neste local
            if (_rows == 1) {
                lt.Local_Codigo = _listaTramitacao[0].CentroCusto_Codigo;
                lt.Local_Nome = _listaTramitacao[0].CentroCusto_Nome;
                lt.Arquivado = false;
                lt.Suspenso = false;
                if(_listaTramitacao[0].Data_Envio==null)
                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Entrada);
                else
                    lt.Data_Evento = Convert.ToDateTime(_listaTramitacao[0].Data_Envio);
                return lt;
            }

            for (int _row = 0; _row < _rows; _row++) {
                string _data1 =  _listaTramitacao[_row].Data_Entrada==null ? "": _listaTramitacao[_row].Data_Entrada.ToString();
                string _data2 = _listaTramitacao[_row].Data_Envio == null ? "" : _listaTramitacao[_row].Data_Envio.ToString();






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
