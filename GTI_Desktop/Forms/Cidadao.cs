using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GTI_Desktop.Forms {
    public partial class Cidadao : Form {
        bool bAddNew;
        int _CodCidadao = 0;
        string _connection = gtiCore.Connection_Name();
        CidadaoStruct regHist = null;

        public int CodCidadao {
            get { return (_CodCidadao); }
            set {
                _CodCidadao = value;
                bAddNew = false;
                if(_CodCidadao>0)
                    LoadReg(_CodCidadao);
            }
        }

        public Cidadao() {
            InitializeComponent();
            Carrega_Profissao();
            Clear_Reg();
            ControlBehaviour(true);
        }

        private void Carrega_Profissao() {
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            List<GTI_Models.Models.Profissao> lista = cidadaoRepository.Lista_Profissao();
            ProfissaoList.DataSource = lista;
            ProfissaoList.DisplayMember = "nome";
            ProfissaoList.ValueMember = "codigo";
        }

        private void CmbPessoa_SelectedIndexChanged(object sender, EventArgs e) {
            if (PessoaList.SelectedIndex == 0) {
                NomeDocText.Text = "CPF....:";
                CPFMask.BringToFront();
                CNPJMask.SendToBack();
            } else {
                NomeDocText.Text = "CNPJ..:";
                CPFMask.SendToBack();
                CNPJMask.BringToFront();
            }
            PessoaText.Text = PessoaList.Text;
            PessoaText.Tag = (PessoaList.SelectedIndex+1).ToString();
        }

        private void Clear_Reg() {
            CodigoText.Text = "000000";
            TipoCidadaoText.Text = "";
            NomeText.Text = "";
            PessoaList.SelectedIndex = 0;
            PessoaText.Text = "";
            PessoaText.Tag = "";
            CPFMask.Text = "";
            CNPJMask.Text = "";
            RGText.Text = "";
            OrgaoText.Text = "";
            CnhText.Text = "";
            CnhOrgaotext.Text = "";
            DataNasctoMask.Text = "";
            ProfissaoList.SelectedIndex = -1;
            ProfissaoText.Text = "";
            ProfissaoText.Tag = "";
            EmailRText.Text = "";
            FoneRText.Text = "";
            LogradouroRText.Text = "";
            NumeroRText.Text = "";
            BairroRText.Text = "";
            ComplementoRText.Text = "";
            CidadeRText.Text = "";
            UFRText.Text = "";
            CepRText.Text = "";
            PaisRText.Text = "";
            EtiquetaRCheck.Checked = false;
            TemFoneRCheck.Checked = false;
            WhatsAppRCheck.Checked = false;
            LogradouroCText.Text = "";
            NumeroCText.Text = "";
            BairroCText.Text = "";
            ComplementoCText.Text = "";
            CidadeCText.Text = "";
            UFCText.Text = "";
            CepCText.Text = "";
            PaisCText.Text = "";
            EtiquetaCButton.Checked = false;
            TemFoneCCheck.Checked = false;
            WhatsAppCCheck.Checked = false;
        }

        private void ControlBehaviour(bool bStart) {
            Color color_enable = Color.White, color_disable = this.BackColor;
            FindCodigoButton.Enabled = bStart;
            AddButton.Enabled = bStart;
            EditButton.Enabled = bStart;
            DelButton.Enabled = bStart;
            ExitButton.Enabled = bStart;
            FindButton.Enabled = bStart;
            NewCodButton.Enabled = bStart;
            GravarButton.Enabled = !bStart;
            CancelarButton.Enabled = !bStart;
            Profissao_EditButton.Enabled = !bStart;
            Profissao_DelButton.Enabled = !bStart;
            AddEnderecoRButton.Enabled = !bStart;
            AddEnderecoCButton.Enabled = !bStart;
            DelEnderecoRButton.Enabled = !bStart;
            DelEnderecoCButton.Enabled = !bStart;

            TextBox box;
            MaskedTextBox mbox;

            foreach (Control c in this.Controls) {
                if (c is TextBox) {
                    box = c as TextBox;
                    box.ReadOnly = bStart;
                } else if (c is MaskedTextBox) {
                    mbox = c as MaskedTextBox;
                    mbox.ReadOnly = bStart;
                }
            }

            foreach (Control c in this.EndRPanel.Controls) {
                if (c is TextBox) {
                    box = c as TextBox;
                    box.ReadOnly = true;
                    box.BackColor =  color_disable;
                } else if (c is MaskedTextBox) {
                    mbox = c as MaskedTextBox;
                    mbox.ReadOnly = true;
                }
            }

            foreach (Control c in this.EndCPanel.Controls) {
                if (c is TextBox) {
                    box = c as TextBox;
                    box.ReadOnly = true;
                    box.BackColor = color_disable;
                } else if (c is MaskedTextBox) {
                    mbox = c as MaskedTextBox;
                    mbox.ReadOnly = true;
                }
            }

            PessoaText.ReadOnly = true;
            PessoaText.Visible = bStart;
            PessoaList.Enabled = !bStart;
            PessoaList.Visible = !bStart;
            ProfissaoText.Visible = bStart;
            ProfissaoList.Enabled = !bStart;
            ProfissaoList.Visible = !bStart;
            JuridicaCheck.Enabled = !bStart;
            EtiquetaRCheck.Enabled = !bStart;
            EtiquetaCButton.Enabled = !bStart;
        }

        private void BtFindCodigo_Click(object sender, EventArgs e) {
            inputBox z = new inputBox();
            string sCod = z.Show("", "Informação", "Digite o código do cidadão.", 6, gtiCore.eTweakMode.IntegerPositive);
            if (!string.IsNullOrEmpty(sCod)) {
                gtiCore.Ocupado(this);
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                if (cidadaoRepository.ExisteCidadao(Convert.ToInt32(sCod))) {
                    Clear_Reg();
                    LoadReg(Convert.ToInt32(sCod));
                } else
                    MessageBox.Show("Cidadão não cadastrado.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gtiCore.Liberado(this);
            }
        }

        private void BtAdd_Click(object sender, EventArgs e) {

            bool bAllow = gtiCore.GetBinaryAccess((int)TAcesso.CadastroCidadao_Alterar_Total);
            if (bAllow) {
                bAddNew = true;
                Clear_Reg();
                ControlBehaviour(false);
                NomeText.Focus();
            } else
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void BtEdit_Click(object sender, EventArgs e) {
            bool bAllow = gtiCore.GetBinaryAccess((int)TAcesso.CadastroCidadao_Alterar_Total);
            if (bAllow) {
                if (Convert.ToInt32(CodigoText.Text) == 0)
                    MessageBox.Show("Selecione um cidadão.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else {
                    int _cod = Convert.ToInt32(CodigoText.Text);
                    if (_cod == 0) return;
                    if (_cod < 500000) {
                        MessageBox.Show("Apenas códigos válidos podem ser alterados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    } else {
                        bAddNew = false;
                        ControlBehaviour(false);
                    }
                }
            } else
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtGravar_Click(object sender, EventArgs e) {
            if (ValidateReg()) {
                SaveReg();
                FindCodigoButton.Focus();
            }
        }

        private void BtCancelar_Click(object sender, EventArgs e) {
            int nCod = Convert.ToInt32(CodigoText.Text);
            if (nCod > 0)
                LoadReg(nCod);
            ControlBehaviour(true);
        }

        private void BtExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void BtFind_Click(object sender, EventArgs e) {
            using (var form = new Cidadao_Lista()) {
                var result = form.ShowDialog(this);
                if (result == DialogResult.OK) {
                    int val = form.ReturnValue;
                    Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                    if(cidadaoRepository.ExisteCidadao(val))
                        LoadReg(val);
                    else {
                        MessageBox.Show("Cidadão não cadastrado!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void LoadReg(int nCodigo) {
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            CidadaoStruct reg = cidadaoRepository.LoadReg(nCodigo);
            regHist = reg;
            CodigoText.Text = reg.Codigo.ToString("000000");
            NomeText.Text = reg.Nome;
            RGText.Text = reg.Rg ?? "";
            OrgaoText.Text = reg.Orgao ?? "";
            CnhText.Text = reg.Cnh ?? "";
            CnhOrgaotext.Text = reg.Orgaocnh ?? "";
            if (reg.DataNascto != null)
                DataNasctoMask.Text = Convert.ToDateTime(reg.DataNascto).ToString("dd/MM/yyyy");
            if (reg.CodigoProfissao == null || reg.CodigoProfissao == 0) {
                ProfissaoList.SelectedIndex = -1;
                ProfissaoText.Text = "";
                ProfissaoText.Tag = "";
            } else {
                ProfissaoList.SelectedValue = reg.CodigoProfissao;
                ProfissaoText.Text = ProfissaoList.Text;
                ProfissaoText.Tag = reg.CodigoProfissao;
            }
            JuridicaCheck.Checked = reg.Juridica;

            if (reg.Tipodoc == 1) {
                PessoaList.SelectedIndex = 0;
                CPFMask.Text = reg.Cpf;
            } else if(reg.Tipodoc==2){
                PessoaList.SelectedIndex = 1;
                CNPJMask.Text = reg.Cnpj;
            } else {
                PessoaList.SelectedIndex = 0;
                CPFMask.Text = "";
            }
            PessoaText.Text = PessoaList.Text;
            PessoaText.Tag = reg.Tipodoc;

            LogradouroRText.Text = reg.EnderecoR;
            LogradouroRText.Tag = reg.CodigoLogradouroR.ToString();
            if(!string.IsNullOrWhiteSpace(LogradouroRText.Text)) {
                NumeroRText.Text = reg.NumeroR.ToString();
                ComplementoRText.Text = reg.ComplementoR;
                BairroRText.Text = reg.NomeBairroR;
                BairroRText.Tag = reg.CodigoBairroR.ToString();
                CidadeRText.Text = reg.NomeCidadeR;
                CidadeRText.Tag = reg.CodigoCidadeR.ToString();
                UFRText.Text = reg.UfR;
                PaisRText.Text = reg.PaisR;
                PaisRText.Tag = reg.CodigoPaisR.ToString();
                CepRText.Text = reg.CepR.ToString();
                EmailRText.Text = reg.EmailR;
                EtiquetaRCheck.Checked = reg.EtiquetaR == "S";
                FoneRText.Text = reg.TelefoneR ?? "";
                TemFoneRCheck.Checked = reg.Temfone==null?false:(bool)reg.Temfone;
                WhatsAppRCheck.Checked =  reg.Whatsapp==null?false:  (bool)reg.Whatsapp;
            } else {
                NumeroRText.Text = "";
                ComplementoRText.Text = "";
                BairroRText.Text = "";
                BairroRText.Tag = "";
                CidadeRText.Text = "";
                CidadeRText.Tag = "";
                UFRText.Text = "";
                PaisRText.Text = "";
                PaisRText.Tag = "";
                CepRText.Text = "";
                EmailRText.Text = "";
                EtiquetaRCheck.Checked =false;
                FoneRText.Text = "";
                TemFoneRCheck.Checked = false;
                WhatsAppRCheck.Checked = false;
            }

            LogradouroCText.Text = reg.EnderecoC;
            LogradouroCText.Tag = reg.CodigoLogradouroC.ToString();
            if (!string.IsNullOrWhiteSpace(LogradouroCText.Text)) {
                NumeroCText.Text = reg.NumeroC.ToString();
                ComplementoCText.Text = reg.ComplementoC;
                BairroCText.Text = reg.NomeBairroC;
                BairroCText.Tag = reg.CodigoBairroC.ToString();
                CidadeCText.Text = reg.NomeCidadeC;
                CidadeCText.Tag = reg.CodigoCidadeC.ToString();
                UFCText.Text = reg.UfC;
                PaisCText.Text = reg.PaisC;
                PaisCText.Tag = reg.CodigoPaisC.ToString();
                CepCText.Text = reg.CepC.ToString();
                EmailCText.Text = reg.EmailC;
                EtiquetaCButton.Checked = reg.EtiquetaC == "S";
                FoneCText.Text = reg.TelefoneC ?? "";
                TemFoneCCheck.Checked = reg.Temfone2==null?false: (bool)reg.Temfone2;
                WhatsAppCCheck.Checked = reg.Whatsapp2==null?false: (bool)reg.Whatsapp2;
            } else {
                LogradouroCText.Text = "";
                LogradouroCText.Tag = "";
                NumeroCText.Text = "";
                ComplementoCText.Text = "";
                BairroCText.Text = "";
                BairroCText.Tag = "";
                CidadeCText.Text = "";
                CidadeCText.Tag = "";
                UFCText.Text = "";
                PaisCText.Text = "";
                PaisCText.Tag = "";
                CepCText.Text = "";
                EmailCText.Text = "";
                FoneCText.Text = "";
                EtiquetaCButton.Checked = false;
                TemFoneCCheck.Checked = false;
                WhatsAppCCheck.Checked = false;
            }

            if (!EtiquetaRCheck.Checked && !EtiquetaCButton.Checked) EtiquetaRCheck.Checked = true;
        }

        private bool ValidateReg() {
            if (gtiCore.IsEmptyDate(DataNasctoMask.Text) || !gtiCore.IsDate(DataNasctoMask.Text)) {
                MessageBox.Show("Data de nascimento inválida.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Convert.ToDateTime(DataNasctoMask.Text) >= DateTime.Now) {
                MessageBox.Show("Data de nascimento inválida.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(LogradouroRText.Text) & string.IsNullOrEmpty(LogradouroCText.Text)) {
                MessageBox.Show("Digite um endereço válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!EtiquetaRCheck.Checked & !EtiquetaCButton.Checked) {
                MessageBox.Show("Selecione o endereço principal.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (EtiquetaRCheck.Checked & EtiquetaCButton.Checked) {
                MessageBox.Show("Selecione apenas um endereço como principal.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if ((string.IsNullOrEmpty(LogradouroCText.Text) & EtiquetaCButton.Checked) || (string.IsNullOrEmpty(LogradouroRText.Text) & EtiquetaRCheck.Checked)) {
                MessageBox.Show("Endereço principal inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (PessoaList.SelectedIndex==0 && !CPFMask.MaskFull) {
                MessageBox.Show("Digite um CPF válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (PessoaList.SelectedIndex == 0 && !gtiCore.Valida_CPF(CPFMask.Text)) {
                MessageBox.Show("Digite um CPF válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (PessoaList.SelectedIndex == 1 && !CNPJMask.MaskFull) {
                MessageBox.Show("Digite um CNPJ válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (PessoaList.SelectedIndex == 1 && !gtiCore.Valida_CNPJ(CNPJMask.Text)) {
                MessageBox.Show("Digite um CNPJ válido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!string.IsNullOrEmpty(EmailRText.Text) & !gtiCore.Valida_Email(EmailRText.Text)) {
                MessageBox.Show("Endereço de email inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!string.IsNullOrEmpty(EmailCText.Text) & !gtiCore.Valida_Email(EmailCText.Text)) {
                MessageBox.Show("Endereço de email inválido.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!gtiCore.IsDate(DataNasctoMask.Text)) {
                MessageBox.Show("Data de nascimento inválida.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SaveReg() {
            if (MessageBox.Show("Gravar os dados?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes){
                GTI_Models.Models.Cidadao reg = new GTI_Models.Models.Cidadao {
                    Nomecidadao = NomeText.Text,
                    Rg = string.IsNullOrWhiteSpace(RGText.Text) ? null : RGText.Text,
                    Orgao = string.IsNullOrWhiteSpace(OrgaoText.Text) ? null : OrgaoText.Text,
                    Cnh = string.IsNullOrWhiteSpace(CnhText.Text) ? null : CnhText.Text,
                    Orgaocnh = string.IsNullOrWhiteSpace(CnhOrgaotext.Text) ? null : CnhOrgaotext.Text
                };
                if (PessoaList.SelectedIndex == 0) {
                    if (CPFMask.Text != "")
                        reg.Cpf = CPFMask.Text;
                } else {
                    if (CNPJMask.Text != "")
                        reg.Cnpj = CNPJMask.Text;
                }
                
                reg.Juridica = JuridicaCheck.Checked ? true : false;
                if ( DataNasctoMask.MaskCompleted &&  gtiCore.IsDate(DataNasctoMask.Text))
                    reg.Data_nascimento = Convert.ToDateTime(DataNasctoMask.Text);
                else
                    reg.Data_nascimento = null;
                if (ProfissaoList.SelectedIndex > -1)
                    reg.Codprofissao = Convert.ToInt32(ProfissaoList.SelectedValue);


                if (PaisRText.Tag.ToString()=="") PaisRText.Tag = "1";
                if (PaisCText.Tag.ToString() == "") PaisCText.Tag = "1";
                if (!string.IsNullOrWhiteSpace(LogradouroRText.Text)) {
                    reg.Codpais = PaisRText.Tag == null ? 0 : Convert.ToInt32(PaisRText.Tag.ToString());
                    reg.Siglauf = UFRText.Text;
                    reg.Codcidade = string.IsNullOrWhiteSpace(CidadeRText.Tag.ToString()) ? (short)0 : Convert.ToInt16(CidadeRText.Tag.ToString());
                    reg.Codbairro = string.IsNullOrWhiteSpace( BairroRText.Tag.ToString())? (short)0 : Convert.ToInt16(BairroRText.Tag.ToString());
                    reg.Codlogradouro =string.IsNullOrWhiteSpace( LogradouroRText.Tag.ToString()) ? 0 : Convert.ToInt32(LogradouroRText.Tag.ToString());
                    reg.Nomelogradouro = reg.Codcidade != 413 ? LogradouroRText.Text : "";
                    reg.Numimovel = NumeroRText.Text == "" ? (short)0 : Convert.ToInt16(NumeroRText.Text);
                    reg.Complemento = ComplementoRText.Text;
                    reg.Cep = reg.Codcidade != 413 ? CepRText.Text == "" ? 0 : Convert.ToInt32(gtiCore.RetornaNumero( CepRText.Text)) : 0;
                    reg.Email = EmailRText.Text;
                    reg.Etiqueta = EtiquetaRCheck.Checked ? "S" : "N";
                    reg.Telefone = string.IsNullOrWhiteSpace(FoneRText.Text) ? null : FoneRText.Text;
                    reg.Temfone = TemFoneRCheck.Checked;
                    reg.Whatsapp = WhatsAppRCheck.Checked;
                }

                if (!string.IsNullOrWhiteSpace(LogradouroCText.Text)) {
                    reg.Codpais2 = PaisCText.Tag == null ? 0 : Convert.ToInt32(PaisCText.Tag.ToString());
                    reg.Siglauf2 = UFCText.Text;
                    reg.Codcidade2 = string.IsNullOrWhiteSpace(CidadeCText.Tag.ToString()) ? (short)0 : Convert.ToInt16(CidadeCText.Tag.ToString());
                    reg.Codbairro2 = string.IsNullOrWhiteSpace(BairroCText.Tag.ToString()) ? (short)0 : Convert.ToInt16(BairroCText.Tag.ToString());
                    reg.Codlogradouro2 = string.IsNullOrWhiteSpace( LogradouroCText.Tag.ToString()) ? 0 : Convert.ToInt32(LogradouroCText.Tag.ToString());
                    reg.Nomelogradouro2 = reg.Codcidade2 != 413 ? LogradouroCText.Text : "";
                    reg.Numimovel2 = NumeroCText.Text == "" ? (short)0 : Convert.ToInt16(NumeroCText.Text);
                    reg.Complemento2 = ComplementoCText.Text;
                    reg.Cep2 = reg.Codcidade2 != 413 ? CepCText.Text == "" ? 0 : Convert.ToInt32(gtiCore.RetornaNumero( CepCText.Text)) : 0;
                    reg.Email2 = EmailCText.Text;
                    reg.Etiqueta2 = EtiquetaCButton.Checked ? "S" : "N";
                    reg.Telefone2 = string.IsNullOrWhiteSpace(FoneCText.Text) ? null : FoneCText.Text;
                    reg.Temfone2 = TemFoneCCheck.Checked;
                    reg.Whatsapp2 = WhatsAppCCheck.Checked;
                }

                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                Exception ex;
                
                if (bAddNew) {
                    int nLastCod = cidadaoRepository.Incluir_cidadao(reg);
                    CodigoText.Text = nLastCod.ToString();
                    ControlBehaviour(true);
                } else {
                    reg.Codcidadao = Convert.ToInt32(CodigoText.Text);
                    ex = cidadaoRepository.Alterar_cidadao(reg);
                    if (ex != null) {
                        ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                        eBox.ShowDialog();
                    } else {
                        Save_Historico(reg);
                        ControlBehaviour(true);
                    }
                }
            }
        }

        private void BtDel_Click(object sender, EventArgs e) {
            bool bAllow = gtiCore.GetBinaryAccess((int)TAcesso.CadastroCidadao_Alterar_Total);
            if (bAllow) {
                int nCodigo = Convert.ToInt32(CodigoText.Text);
                if (nCodigo == 0)
                    MessageBox.Show("Selecione um cidadão.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else {
                    if (MessageBox.Show("Excluir este registro?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                        Exception ex = cidadaoRepository.Excluir_cidadao(nCodigo);
                        if (ex != null) {
                            ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                            eBox.ShowDialog();
                        } else
                            Clear_Reg();
                    }
                }
            } else
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void BtProfissao_Del_Click(object sender, EventArgs e) {
            ProfissaoList.SelectedIndex = -1;
        }

        private void BtProfissao_Edit_Click(object sender, EventArgs e) {
            Carrega_Profissao();
        }

        private void CmbProfissao_SelectedIndexChanged(object sender, EventArgs e) {
            ProfissaoText.Text = ProfissaoList.Text;
            if (ProfissaoList.SelectedIndex==-1)
                ProfissaoText.Tag = "";
            else
                ProfissaoText.Tag = ProfissaoList.SelectedValue.ToString();

        }

        private void ChkEtiquetaR_CheckedChanged(object sender, EventArgs e) {
            EtiquetaCButton.Checked = !EtiquetaRCheck.Checked;
        }

        private void ChkEtiquetaC_CheckedChanged(object sender, EventArgs e) {
            EtiquetaRCheck.Checked = !EtiquetaCButton.Checked;
        }

        private void BtAddEnderecoR_Click(object sender, EventArgs e) {
            GTI_Models.Models.Endereco reg = new GTI_Models.Models.Endereco {
                Id_pais = string.IsNullOrWhiteSpace(PaisRText.Text) ? 1 : Convert.ToInt32(PaisRText.Tag.ToString()),
                Sigla_uf = UFRText.Text == "" ? "SP" : UFRText.Text,
                Id_cidade = string.IsNullOrWhiteSpace(CidadeRText.Text) ? 413 : Convert.ToInt32(CidadeRText.Tag.ToString()),
                Id_bairro = string.IsNullOrWhiteSpace(BairroRText.Text) ? 0 : Convert.ToInt32(BairroRText.Tag.ToString())
            };
            if (LogradouroRText.Tag == null) LogradouroRText.Tag = "0";
            if (string.IsNullOrWhiteSpace(LogradouroRText.Tag.ToString()))
                LogradouroRText.Tag = "0";
            reg.Id_logradouro = string.IsNullOrWhiteSpace(LogradouroRText.Text) ? 0 : Convert.ToInt32(LogradouroRText.Tag.ToString());
            reg.Nome_logradouro = reg.Id_cidade != 413 ? LogradouroRText.Text : "";
            reg.Numero_imovel = NumeroRText.Text == "" ? 0 : Convert.ToInt32(NumeroRText.Text);
            reg.Complemento = ComplementoRText.Text;
            reg.Email = EmailRText.Text;
            reg.Cep = reg.Id_cidade != 413 ? CepRText.Text == "" ? 0 : Convert.ToInt32(gtiCore.ExtractNumber( CepRText.Text)) : 0;
            reg.Telefone = FoneRText.Text;
            reg.TemFone = TemFoneRCheck.Checked;
            reg.WhatsApp = WhatsAppRCheck.Checked;
            int _x = Location.X + EndRPanel.Location.X + AddEnderecoRButton.Location.X;
            int _y = Location.Y + EndRPanel.Location.Y + AddEnderecoRButton.Location.Y;
            Endereco_Enable _fields = new Endereco_Enable() { Bairro = true, Cidade = true, Email = true, Endereco = true, Pais = true, Telefone = true, Uf = true, Complemento = true, Numero = true };
            Endereco f1 = new Endereco(reg, _fields, _x, _y - 20, "Cadastro de endereço residencial");
            f1.ShowDialog();
            if (!f1.EndRetorno.Cancelar) {
                PaisRText.Text = f1.EndRetorno.Nome_pais;
                PaisRText.Tag = f1.EndRetorno.Id_pais.ToString();
                UFRText.Text = f1.EndRetorno.Sigla_uf;
                CidadeRText.Text = f1.EndRetorno.Nome_cidade;
                CidadeRText.Tag = f1.EndRetorno.Id_cidade.ToString();
                BairroRText.Text = f1.EndRetorno.Nome_bairro;
                BairroRText.Tag = f1.EndRetorno.Id_bairro.ToString();
                LogradouroRText.Text = f1.EndRetorno.Nome_logradouro;
                LogradouroRText.Tag = f1.EndRetorno.Id_logradouro.ToString();
                NumeroRText.Text = f1.EndRetorno.Numero_imovel.ToString();
                ComplementoRText.Text = f1.EndRetorno.Complemento;
                EmailRText.Text = f1.EndRetorno.Email;
                CepRText.Text = f1.EndRetorno.Cep.ToString("00000-000");
                FoneRText.Text = f1.EndRetorno.Telefone;
                TemFoneRCheck.Checked =(bool) f1.EndRetorno.TemFone;
                WhatsAppRCheck.Checked = (bool)f1.EndRetorno.WhatsApp;
            }
        }

        private void BtAddEnderecoC_Click(object sender, EventArgs e) {
            GTI_Models.Models.Endereco reg = new GTI_Models.Models.Endereco {
                Id_pais = string.IsNullOrWhiteSpace(PaisCText.Text) ? 1 : Convert.ToInt32(PaisCText.Tag.ToString()),
                Sigla_uf = UFCText.Text == "" ? "SP" : UFCText.Text,
                Id_cidade = string.IsNullOrWhiteSpace(CidadeCText.Text) ? 413 : Convert.ToInt32(CidadeCText.Tag.ToString()),
                Id_bairro = string.IsNullOrWhiteSpace(BairroCText.Text) ? 0 : Convert.ToInt32(BairroCText.Tag.ToString())
            };
            if (LogradouroCText.Tag == null) LogradouroCText.Tag = "0";
            if (string.IsNullOrWhiteSpace(LogradouroCText.Tag.ToString()))
                LogradouroCText.Tag = "0";
            reg.Id_logradouro = string.IsNullOrWhiteSpace(LogradouroCText.Text) ? 0 : Convert.ToInt32(LogradouroCText.Tag.ToString());
            reg.Nome_logradouro = reg.Id_cidade != 413 ? LogradouroCText.Text : "";
            reg.Numero_imovel = NumeroCText.Text == "" ? 0 : Convert.ToInt32(NumeroCText.Text);
            reg.Complemento = ComplementoCText.Text;
            reg.Email = EmailCText.Text;
            reg.Telefone = FoneCText.Text;
            reg.Cep = reg.Id_cidade != 413 ? CepCText.Text == "" ? 0 : Convert.ToInt32(gtiCore.ExtractNumber( CepCText.Text)) : 0;
            reg.TemFone = TemFoneCCheck.Checked;
            reg.WhatsApp = WhatsAppCCheck.Checked;
            int _x = Location.X + EndRPanel.Location.X + AddEnderecoCButton.Location.X;
            int _y = Location.Y + EndRPanel.Location.Y + AddEnderecoCButton.Location.Y;
            Endereco_Enable _fields = new Endereco_Enable() { Bairro = true, Cidade = true, Email = true, Endereco = true, Pais = true, Telefone = true, Uf = true, Complemento = true, Numero = true };
            Endereco f1 = new Endereco(reg, _fields,_x,_y-20,"Cadastro de endereço comercial");
            f1.ShowDialog();
            if (!f1.EndRetorno.Cancelar) {
                PaisCText.Text = f1.EndRetorno.Nome_pais;
                PaisCText.Tag = f1.EndRetorno.Id_pais.ToString();
                UFCText.Text = f1.EndRetorno.Sigla_uf;
                CidadeCText.Text = f1.EndRetorno.Nome_cidade;
                CidadeCText.Tag = f1.EndRetorno.Id_cidade.ToString();
                BairroCText.Text = f1.EndRetorno.Nome_bairro;
                BairroCText.Tag = f1.EndRetorno.Id_bairro.ToString();
                LogradouroCText.Text = f1.EndRetorno.Nome_logradouro;
                LogradouroCText.Tag = f1.EndRetorno.Id_logradouro.ToString();
                NumeroCText.Text = f1.EndRetorno.Numero_imovel.ToString();
                ComplementoCText.Text = f1.EndRetorno.Complemento;
                EmailCText.Text = f1.EndRetorno.Email;
                CepCText.Text = f1.EndRetorno.Cep.ToString("00000-000");
                FoneCText.Text = f1.EndRetorno.Telefone;
                TemFoneCCheck.Checked = (bool)f1.EndRetorno.TemFone;
                WhatsAppCCheck.Checked = (bool)f1.EndRetorno.WhatsApp;
            }
        }

        private void DelEnderecoRButton_Click(object sender, EventArgs e) {
            PaisRText.Text = "";
            PaisRText.Tag = "";
            UFRText.Text = "";
            CidadeRText.Text = "";
            CidadeRText.Tag = "";
            BairroRText.Text = "";
            BairroRText.Tag = "";
            LogradouroRText.Text = "";
            LogradouroRText.Tag = "";
            NumeroRText.Text = "";
            ComplementoRText.Text = "";
            EmailRText.Text = "";
            CepRText.Text = "";
            FoneRText.Text = "";
            TemFoneRCheck.Checked = false;
            WhatsAppRCheck.Checked = false;
        }

        private void DelEnderecoCButton_Click(object sender, EventArgs e) {
            PaisCText.Text = "";
            PaisCText.Tag = "";
            UFCText.Text = "";
            CidadeCText.Text = "";
            CidadeCText.Tag = "";
            BairroCText.Text = "";
            BairroCText.Tag = "";
            LogradouroCText.Text = "";
            LogradouroCText.Tag = "";
            NumeroCText.Text = "";
            ComplementoCText.Text = "";
            EmailCText.Text = "";
            CepCText.Text = "";
            FoneCText.Text = "";
            TemFoneCCheck.Checked = false;
            WhatsAppCCheck.Checked = false;
        }

        private void CpfCnpjButton_Click(object sender, EventArgs e) {

        }

        private void HistoricoButton_Click(object sender, EventArgs e) {
            if (NomeText.Text != "") {
                Cidadao_Historico frm = new Cidadao_Historico(Convert.ToInt32(CodigoText.Text),"H");
                frm.ShowDialog();
            }
        }

        private void ObservaçãoButton_Click(object sender, EventArgs e) {
            if (NomeText.Text != "") {
                Cidadao_Historico frm = new Cidadao_Historico(Convert.ToInt32(CodigoText.Text), "O");
                frm.ShowDialog();
            }

        }

        private void Save_Historico(GTI_Models.Models.Cidadao regNew) {
            List<string> aLog = new List<string>();
            if (regNew.Nomecidadao != regHist.Nome) {
                aLog.Add("Alterado o nome de " + regHist.Nome + " para " + regNew.Nomecidadao);
            }
            if (gtiCore.SubNull(regNew.Rg) != gtiCore.SubNull(regHist.Rg)) {
                aLog.Add("Alterado o Rg de " + regHist.Rg + " para " + regNew.Rg);
            }
            if (gtiCore.SubNull(regNew.Cpf) != gtiCore.SubNull(regHist.Cpf)) {
                aLog.Add("Alterado o Cpf de " + regHist.Cpf + " para " + regNew.Cpf);
            }
            if (gtiCore.SubNull(regNew.Cnpj) != gtiCore.SubNull(regHist.Cnpj)) {
                aLog.Add("Alterado o Cnpj de " + regHist.Cnpj + " para " + regNew.Cnpj);
            }
            if (gtiCore.SubNull(regNew.Cnh) != gtiCore.SubNull(regHist.Cnh)) {
                aLog.Add("Alterado a Cnh de " + regHist.Cnh + " para " + regNew.Cnh);
            }
            if (regNew.Data_nascimento != regHist.DataNascto) {
                aLog.Add("Alterado a data de nascto. de " + regHist.DataNascto + " para " + regNew.Data_nascimento);
            }
            if (gtiCore.SubNull(LogradouroRText.Text) != gtiCore.SubNull(regHist.EnderecoR)) {
                aLog.Add("Alterado o logradouro res. de " + regHist.EnderecoR + " para " + regNew.Nomelogradouro);
            }
            if (gtiCore.SubNull(LogradouroCText.Text) != gtiCore.SubNull(regHist.EnderecoC)) {
                aLog.Add("Alterado o logradouro com. de " + regHist.EnderecoC + " para " + regNew.Nomelogradouro2);
            }
            if( gtiCore.SubNull(regNew.Numimovel) != gtiCore.SubNull(regHist.NumeroR)) {
                aLog.Add("Alterado o nº res. de " + regHist.NumeroR + " para " + regNew.Numimovel);
            }
            if (gtiCore.SubNull(regNew.Numimovel2) != gtiCore.SubNull(regHist.NumeroC)) {
                aLog.Add("Alterado o nº com. de " + regHist.NumeroC + " para " + regNew.Numimovel2);
            }
            if (gtiCore.SubNull(regNew.Profissao) != gtiCore.SubNull(regHist.Profissao)) {
                aLog.Add("Alterado a profissão de " + regHist.Profissao + " para " + regNew.Profissao);
            }

            if (aLog.Count > 0) {
                Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
                foreach (string item in aLog) {
                    historicocidadao _hist = new historicocidadao {
                        Codigo = regNew.Codcidadao,
                        Obs = item,
                        Userid = Properties.Settings.Default.UserId,
                        Data=DateTime.Now
                    };
                    Exception ex = cidadaoRepository.Incluir_Historico_Cidadao(_hist);
                }
            }
        }

        private void NewCodButton_Click(object sender, EventArgs e) {
            int _cod = Convert.ToInt32(CodigoText.Text);
            if (_cod == 0) return;
            if (_cod >= 500000) {
                MessageBox.Show("Apenas códigos antigos podem ser renumerados.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Cidadao_bll cidadaoRepository = new Cidadao_bll(_connection);
            int _newcod = cidadaoRepository.Novo_Codigo_Cidadao(_cod);
            CodigoText.Text = _newcod.ToString();

            historicocidadao _hist = new historicocidadao {
                Codigo = _newcod,
                Obs = "Alterado código de " + _cod.ToString() + " para " + _newcod.ToString(),
                Userid = Properties.Settings.Default.UserId,
                Data = DateTime.Now
            };
            Exception ex = cidadaoRepository.Incluir_Historico_Cidadao(_hist);
        }


    }
}
