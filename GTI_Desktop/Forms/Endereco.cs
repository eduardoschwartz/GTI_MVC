using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static GTI_Desktop.Classes.GtiTypes;

namespace GTI_Desktop.Forms {
    public partial class Endereco : Form {
        bool _camposObrigatorios=false;
        bool _telefone;
        string _connection = gtiCore.Connection_Name();
        int StartLocationX;
        int StartLocationY;
        string _title;
        public GTI_Models.Models.Endereco EndRetorno { get; set; }

        public Endereco( GTI_Models.Models.Endereco reg, Endereco_Enable fields, int xPos=200,int yPos=200,string Title="Cadastro de endere�o") {
            InitializeComponent();
            Carrega_Endereco(reg);
            LogradouroText.Enabled = fields.Endereco;
            LogradouroList.Enabled = fields.Endereco;
            NumeroList.Enabled = fields.Numero;
            ComplementoText.Enabled = fields.Complemento;
            PaisList.Enabled = fields.Pais;
            EmailText.Enabled = fields.Email;
            UFList.Enabled = fields.Uf;
            CidadeList.Enabled = fields.Cidade;
            BairroList.Enabled = fields.Bairro;
            BairroText.Enabled = fields.Bairro;
            CepMask.Enabled = fields.Endereco;
            PaisButton_Refresh.Enabled = fields.Pais;
            BairroButton_Refresh.Enabled = fields.Bairro;
            _telefone = fields.Telefone;
            TelefoneText.Enabled = _telefone;
            TemFoneCheck.Enabled = _telefone;
            WhatsAppCheck.Enabled = _telefone;
            StartLocationX = xPos;
            StartLocationY = yPos;
            _title = Title;
            Esconde_Bairro(reg.Sigla_uf, reg.Id_cidade);
        }

        private void CmbUF_SelectedIndexChanged(object sender, EventArgs e) {
            if (UFList.SelectedIndex > -1) {
                Endereco_bll clsCidade = new Endereco_bll(_connection);
                List<Cidade> lista = clsCidade.Lista_Cidade(UFList.SelectedValue.ToString());
                List<CustomListBoxItem> myItems = new List<CustomListBoxItem>();
                foreach (Cidade item in lista) {
                    myItems.Add(new CustomListBoxItem(item.Desccidade, item.Codcidade));
                }
                CidadeList.DisplayMember = "_name";
                CidadeList.ValueMember = "_value";
                CidadeList.DataSource = myItems;
                if (UFList.SelectedIndex > 0 && UFList.SelectedValue.ToString() == "SP") {
                    CidadeList.SelectedValue = 413;
                }
                Esconde_Bairro(UFList.SelectedValue.ToString(), Convert.ToInt32(CidadeList.SelectedValue));
            }
        }

        private void Carrega_Bairro() {
            if (CidadeList.SelectedIndex > -1) {
                Endereco_bll EnderecoRepository = new Endereco_bll(_connection);
                List<GTI_Models.Models.Bairro> lista = EnderecoRepository.Lista_Bairro(UFList.SelectedValue.ToString(), Convert.ToInt32(CidadeList.SelectedValue));
                List<CustomListBoxItem> myItems = new List<CustomListBoxItem>();
                foreach (GTI_Models.Models.Bairro item in lista) {
                    myItems.Add(new CustomListBoxItem(item.Descbairro, item.Codbairro));
                }
                BairroList.DisplayMember = "_name";
                BairroList.ValueMember = "_value";
                BairroList.DataSource = myItems;
            }
        }

        private void CmbCidade_SelectedIndexChanged(object sender, EventArgs e) {
            Carrega_Bairro();
            BairroList.SelectedIndex = -1;
            if (Convert.ToInt32(CidadeList.SelectedValue) == 413)
                CepMask.ReadOnly = true;
            else
                CepMask.ReadOnly = false;
            Esconde_Bairro(UFList.SelectedValue.ToString(), Convert.ToInt32(CidadeList.SelectedValue));
        }

        private void Carrega_Pais() {
            Endereco_bll clsCidade = new Endereco_bll(_connection);
            PaisList.DataSource = clsCidade.Lista_Pais();
            PaisList.DisplayMember = "nome_pais";
            PaisList.ValueMember = "id_pais";
        }

        private void Carrega_UF() {
            Endereco_bll EnderecoRepository = new Endereco_bll(_connection);
            List<GTI_Models.Models.Uf> lista = EnderecoRepository.Lista_UF();
            List<CustomListBoxItem6> myItems = new List<CustomListBoxItem6>();
            myItems.Add(new CustomListBoxItem6(" ", "EX"));
            foreach (GTI_Models.Models.Uf item in lista) {
                myItems.Add(new CustomListBoxItem6(item.Descuf, item.Siglauf));
            }
            UFList.DisplayMember = "_name";
            UFList.ValueMember = "_value";
            UFList.DataSource = myItems;

        }

        private void BtPais_Refresh_Click(object sender, EventArgs e) {
            Pais frmPais = new Pais();
            frmPais.ShowDialog();
            Carrega_Pais();
        }

        private void BtBairro_Refresh_Click(object sender, EventArgs e) {
            Bairro frmBairro = new Bairro();
            frmBairro.ShowDialog();
            Carrega_Bairro();
        }

        private void TxtNum_TextChanged(object sender, EventArgs e) {
            CepMask.Text = "";
            if (System.Text.RegularExpressions.Regex.IsMatch(NumeroList.Text, "[^0-9]")) {
                MessageBox.Show("Digite apenas n�meros.","Aten��o",MessageBoxButtons.OK,MessageBoxIcon.Error);
                NumeroList.Text = NumeroList.Text.Remove(NumeroList.Text.Length - 1);
                if (NumeroList.Text.Length > 0) {
                    NumeroList.SelectionStart = NumeroList.Text.Length;
                }
                NumeroList.SelectionLength = 0;
            } else {
                if (string.IsNullOrEmpty(LogradouroText.Tag.ToString())) LogradouroText.Tag = "0";
            }
            CarregaCep();
        }

        private void Carrega_Endereco(GTI_Models.Models.Endereco reg) {
            Carrega_Pais();
            Carrega_UF();
            if (reg.Id_pais > 0)
                PaisList.SelectedValue = reg.Id_pais;
            if (!string.IsNullOrWhiteSpace( reg.Sigla_uf) ) {
                UFList.SelectedValue = reg.Sigla_uf;
                CmbUF_SelectedIndexChanged(null, null);
            }
            if (reg.Id_cidade > 0) {
                CidadeList.SelectedValue = Convert.ToInt32(reg.Id_cidade);
                CmbCidade_SelectedIndexChanged(null, null);
            }
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (reg.Id_logradouro > 0) {
                LogradouroText.Text = enderecoRepository.Retorna_Logradouro(reg.Id_logradouro);
            } else
                LogradouroText.Text = reg.Nome_logradouro;
            LogradouroText.Tag = reg.Id_logradouro;
            ComplementoText.Text = reg.Complemento;
            EmailText.Text = reg.Email;
            NumeroList.Text = reg.Numero_imovel > 0 ? reg.Numero_imovel.ToString() : "";

            if (reg.Id_bairro > 0) {
                if (reg.Sigla_uf == "SP" && reg.Id_cidade == 413) {
                    //GTI_Models.Models.Bairro _bairro = enderecoRepository.RetornaLogradouroBairro(reg.Id_logradouro, (short)reg.Numero_imovel);
                    //BairroText.Text = _bairro.Descbairro;
                    //BairroText.Tag = _bairro.Codbairro.ToString();
                } else {
                    BairroText.Text= enderecoRepository.Retorna_Bairro(reg.Sigla_uf, reg.Id_cidade,reg.Id_bairro);
                    BairroText.Tag = reg.Id_bairro.ToString();
                }
                BairroList.SelectedValue = reg.Id_bairro;
            }

            if (reg.Cep > 0)
                CepMask.Text = reg.Cep.ToString();
            else
                CarregaCep();

            TelefoneText.Text = reg.Telefone ?? "";
            if (reg.TemFone == null)
                TemFoneCheck.CheckState = CheckState.Unchecked;
            else {
                if (reg.TemFone == true)
                    TemFoneCheck.CheckState = CheckState.Checked;
            }
            if (reg.WhatsApp == null)
                WhatsAppCheck.CheckState = CheckState.Unchecked;
            else {
                if (reg.WhatsApp == true)
                    WhatsAppCheck.CheckState = CheckState.Checked;
            }
            BairroList.Focus();
        }

        private void TxtLogradouro_KeyDown(object sender, KeyEventArgs e) {
            if (Convert.ToInt32(CidadeList.SelectedValue) != 413) {
                CepMask.Text = "";
                return;
            }
            if (!string.IsNullOrEmpty(LogradouroText.Text) && e.KeyCode == Keys.Enter) {
                Endereco_bll clsImovel = new Endereco_bll(_connection);
                List<Logradouro> Listalogradouro = clsImovel.Lista_Logradouro(LogradouroText.Text);

                LogradouroList.DataSource = Listalogradouro;
                LogradouroList.DisplayMember = "endereco";
                LogradouroList.ValueMember = "codlogradouro";
                if (LogradouroList.Items.Count > 0) {
                    LogradouroList.Visible = true;
                    LogradouroList.BringToFront();
                    LogradouroList.DroppedDown = true;
                    LogradouroList.Focus();
                } else {
                    MessageBox.Show("Logradouro n�o localizado.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogradouroText.Focus();
                }
            } else
                LogradouroText.Tag = "";
        }

        private void LstLogr_KeyDown(object sender, KeyEventArgs e) {
            if (LogradouroList.SelectedValue == null) return;
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            if (e.KeyCode == Keys.Escape) {
                LogradouroList.Visible = false;


                LogradouroText.Focus();
                return;
            }
            if (e.KeyCode == Keys.Enter) {
                LogradouroText.Text = LogradouroList.Text;
                LogradouroText.Tag = LogradouroList.SelectedValue.ToString();
                LogradouroList.Visible = false;
                CarregaCep();
                NumeroList.Focus();
            }
        }

        private void LstLogr_Leave(object sender, EventArgs e) {
            if (LogradouroList.SelectedValue == null) {
                LogradouroText.Text = "";
                LogradouroText.Tag = "";
            } else {
                LogradouroText.Text = LogradouroList.Text;
                LogradouroText.Tag = LogradouroList.SelectedValue.ToString();
            }
            LogradouroList.Visible = false;
            NumeroList.Focus();
            CarregaCep();
        }

        private void CmbBairro_SelectedIndexChanged(object sender, EventArgs e) {
            LogradouroText.Focus();
        }

        private void TxtNum_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                ComplementoText.Focus();
        }

        private void TxtComplemento_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                CepMask.Focus();
        }

        private void MskCep_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                if (EmailText.Enabled)
                    EmailText.Focus();
                else
                    ReturnButton.Focus();
        }

        private void CarregaCep() {
            if (Convert.ToInt32(LogradouroText.Tag.ToString()) == 0)
                LogradouroText.Tag = "0";

            if (UFList.SelectedValue.ToString() == "SP" && Convert.ToInt32(CidadeList.SelectedValue) == 413)  {
                Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                int nCep = enderecoRepository.RetornaCep(Convert.ToInt32(LogradouroText.Tag.ToString()), NumeroList.Text==""?(short)0:  Convert.ToInt16(NumeroList.Text));
                CepMask.Text = nCep.ToString("00000-000");

                short _num = 0;
                if (gtiCore.IsNumeric(NumeroList.Text))
                    _num = Convert.ToInt16(NumeroList.Text);
                GTI_Models.Models.Bairro _bairro = enderecoRepository.RetornaLogradouroBairro(Convert.ToInt32(LogradouroText.Tag.ToString()), _num);
                BairroText.Text = _bairro.Descbairro;
                BairroText.Tag = _bairro.Codbairro.ToString();

            }
        }

        private void LstLogr_TextChanged(object sender, EventArgs e) {
            CepMask.Text = "";
        }

        private void BtCancel_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Cancelar a edi��o do endere�o?", "Confirma��o", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                EndRetorno = new GTI_Models.Models.Endereco {
                    Cancelar = true
                };
                Close();
                return;
            }
        }

        private void EmailText_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter)
                if (TelefoneText.Enabled)
                    TelefoneText.Focus();
                else
                    ReturnButton.Focus();

        }

        private void TemFoneCheck_CheckedChanged(object sender, EventArgs e) {
            if (TemFoneCheck.Checked)
                TelefoneText.Text = "";
        }

        private void Endereco_Load(object sender, EventArgs e) {
            SetDesktopLocation(StartLocationX, StartLocationY);
            this.Text = _title;
        }

        private void Esconde_Bairro(string UF,int Cidade) {
            if (UF == "SP" && Cidade == 413) {
                BairroList.Visible = false;
                BairroButton_Refresh.Visible = false;
                BairroText.Visible = true;
            } else {
                BairroList.Visible = true;
                BairroButton_Refresh.Visible = true;
                BairroText.Visible = false;
            }
        }

        private void BtReturn_Click(object sender, EventArgs e) {
            if (_camposObrigatorios) {
                if (Convert.ToInt32(CidadeList.SelectedValue) == 413) {
                    if (LogradouroText.Tag.ToString() == "") LogradouroText.Tag = "0";
                    if (Convert.ToInt32(LogradouroText.Tag.ToString()) == 0) {
                        MessageBox.Show("Selecione um logradouro v�lido!", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                } else {
                    if (string.IsNullOrWhiteSpace(LogradouroText.Text)) {
                        MessageBox.Show("Digite o nome do logradouro!", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (CepMask.Text.Trim() != "-") {
                    if (!CepMask.MaskFull) {
                        MessageBox.Show("Cep inv�lido!", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                if (!string.IsNullOrWhiteSpace(EmailText.Text) & !gtiCore.Valida_Email(EmailText.Text)) {
                    MessageBox.Show("Endere�o de email inv�lido.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (TelefoneText.Text.Trim() == "" && WhatsAppCheck.Checked) {
                    MessageBox.Show("Digite o n� do WhatsApp, ou desmarque esta op��o.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (TelefoneText.Text.Trim() != "" && TemFoneCheck.Checked) {
                    MessageBox.Show("Apague o n�mero de telefone, ou desmarque a op��o que n�o possui telefone.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            EndRetorno = new GTI_Models.Models.Endereco();
            if (PaisList.SelectedIndex > -1) {
                EndRetorno.Id_pais = Convert.ToInt32(PaisList.SelectedValue);
                EndRetorno.Nome_pais = PaisList.Text;
            } else {
                EndRetorno.Id_pais = 0;
                EndRetorno.Nome_pais = "";
            }


            if (UFList.SelectedIndex > -1) {
                EndRetorno.Sigla_uf = UFList.SelectedValue.ToString();
                EndRetorno.Nome_uf = UFList.Text;
            } else {
                EndRetorno.Sigla_uf = "";
                EndRetorno.Nome_uf = "";
            }

            if (EndRetorno.Id_pais == 1 && EndRetorno.Sigla_uf == "EX") {
                MessageBox.Show("Selecione a UF.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (EndRetorno.Id_pais != 1 && EndRetorno.Sigla_uf != "EX") {
                MessageBox.Show("Apenas o Brasil pode ter UF.", "Aten��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CidadeList.SelectedIndex > -1) {
                EndRetorno.Id_cidade = Convert.ToInt32(CidadeList.SelectedValue);
                EndRetorno.Nome_cidade = CidadeList.Text;
            } else {
                EndRetorno.Id_cidade = 0;
                EndRetorno.Nome_cidade = "";
            }
            if (BairroList.SelectedIndex > -1) {
                EndRetorno.Id_bairro = Convert.ToInt32(BairroList.SelectedValue);
                EndRetorno.Nome_bairro = BairroList.Text;
            } else {
                EndRetorno.Id_bairro = 0;
                EndRetorno.Nome_bairro = "";
            }

            if (BairroText.Text != "") {
                EndRetorno.Id_bairro = Convert.ToInt32( BairroText.Tag.ToString());
                EndRetorno.Nome_bairro = BairroText.Text;
            }

            if (string.IsNullOrEmpty(LogradouroText.Tag.ToString())) LogradouroText.Tag = "0";
            EndRetorno.Id_logradouro = Convert.ToInt32(LogradouroText.Tag.ToString());
            EndRetorno.Nome_logradouro = LogradouroText.Text;
            if (string.IsNullOrEmpty(NumeroList.Text.ToString())) NumeroList.Text = "0";
            EndRetorno.Numero_imovel = Convert.ToInt32(NumeroList.Text);
            EndRetorno.Complemento = ComplementoText.Text;
            EndRetorno.Email = EmailText.Text;
            string _cep = gtiCore.ExtractNumber(CepMask.Text);
            EndRetorno.Cep = _cep == "" ? 0 : Convert.ToInt32(_cep);
            EndRetorno.Cancelar = false;
            EndRetorno.Telefone = TelefoneText.Text;
            EndRetorno.TemFone = TemFoneCheck.Checked;
            EndRetorno.WhatsApp = WhatsAppCheck.Checked;
            Close();
            return;
        }

    }
}
