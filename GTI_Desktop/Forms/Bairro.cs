﻿using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static GTI_Desktop.Classes.GtiTypes;

namespace GTI_Desktop.Forms {


    public partial class Bairro : Form {

        string _connection = gtiCore.Connection_Name();
        public Bairro() {
            InitializeComponent();
        }

        private void Bairro_Load(object sender, EventArgs e) {
            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            UFCombo.DataSource = enderecoRepository.Lista_UF();
            UFCombo.DisplayMember = "siglauf";
            UFCombo.ValueMember = "siglauf";
            UFCombo.SelectedValue = "SP";
        }

        private void BtExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void CmbUF_SelectedIndexChanged(object sender, EventArgs e) {
            BairroListBox.DataSource = null;
            if (UFCombo.SelectedIndex == -1) return;
            gtiCore.Ocupado(this);
            Uf Estado = (Uf)UFCombo.SelectedItem;
            String sUF = Estado.Siglauf;

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<Cidade> lista = enderecoRepository.Lista_Cidade(sUF);

            List<CustomListBoxItem> myItems = new List<CustomListBoxItem>();
            foreach (Cidade item in lista) {
                myItems.Add(new CustomListBoxItem(item.Desccidade, item.Codcidade));
            }
            CidadeCombo.DisplayMember = "_name";
            CidadeCombo.ValueMember = "_value";
            CidadeCombo.DataSource = myItems;

            if (UFCombo.SelectedIndex > 0 && UFCombo.SelectedValue.ToString() == "SP") {
                CidadeCombo.SelectedValue = 413;
            }

            gtiCore.Liberado(this);
        }

        private void CmbCidade_SelectedIndexChanged(object sender, EventArgs e) {
            BairroListBox.DataSource = null;
            if (CidadeCombo.SelectedIndex == -1) return;
            gtiCore.Ocupado(this);
            String sUF = UFCombo.SelectedValue.ToString();
            CustomListBoxItem city = (CustomListBoxItem)CidadeCombo.SelectedItem;
            Int32 nCodCidade = city._value;

            Endereco_bll enderecoRepository = new Endereco_bll(_connection);
            List<GTI_Models.Models.Bairro> lista = enderecoRepository.Lista_Bairro(sUF, nCodCidade);
            BairroListBox.DataSource = lista;
            BairroListBox.DisplayMember = "descbairro";
            BairroListBox.ValueMember = "codbairro";

            gtiCore.Liberado(this);
        }

        private void BtAdd_Click(object sender, EventArgs e) {
            inputBox iBox = new inputBox();

            bool bAllowLocal = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Local);
            bool bAllowFora = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Fora);

            if (!bAllowLocal && !bAllowFora) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (UFCombo.SelectedValue.ToString() == "SP" && Convert.ToInt32(CidadeCombo.SelectedValue) == 413 && !bAllowLocal) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string _nomeBairro = iBox.Show("", "Informação", "Digite o nome do bairro.", 40);
            if (!string.IsNullOrEmpty(_nomeBairro)) {
                _nomeBairro = _nomeBairro.ToUpper();
                string _uf =UFCombo.SelectedValue.ToString();
                short _cidade = Convert.ToInt16(CidadeCombo.SelectedValue.ToString());
                Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                bool _existe = enderecoRepository.Existe_Bairro(_uf,_cidade,_nomeBairro);
                if (_existe) {
                    MessageBox.Show("Bairro já cadastrado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else {
                    GTI_Models.Models.Bairro reg = new GTI_Models.Models.Bairro {
                        Siglauf = _uf,
                        Codcidade = _cidade,
                        Descbairro = _nomeBairro.ToUpper()
                    };
                    int _cod = enderecoRepository.Incluir_bairro(reg);
                    CmbCidade_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void BtEdit_Click(object sender, EventArgs e) {
            if (BairroListBox.SelectedItem == null) return;
            bool bAllowLocal = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Local);
            bool bAllowFora = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Fora);

            if (!bAllowLocal && !bAllowFora) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (UFCombo.SelectedValue.ToString() == "SP" && Convert.ToInt32(CidadeCombo.SelectedValue) == 413 && !bAllowLocal) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            inputBox iBox = new inputBox();
            String sCod = iBox.Show(BairroListBox.Text, "Informação", "Digite o nome do bairro.", 40);
            if (!string.IsNullOrEmpty(sCod)) {
                Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                GTI_Models.Models.Bairro reg = new GTI_Models.Models.Bairro {
                    Siglauf = UFCombo.SelectedValue.ToString(),
                    Codcidade = Convert.ToInt16(CidadeCombo.SelectedValue.ToString()),
                    Codbairro = Convert.ToInt16(BairroListBox.SelectedValue.ToString()),
                    Descbairro = sCod.ToUpper()
                };
                Exception ex=enderecoRepository.Alterar_Bairro(reg);
                if (ex != null) {
                    ErrorBox eBox = new ErrorBox("Atenção", "Bairro já cadastrado.", ex);
                    eBox.ShowDialog();
                } else
                    CmbCidade_SelectedIndexChanged(sender, e);
            }
        }

        private void BtDel_Click(object sender, EventArgs e) {
            if (BairroListBox.SelectedItem == null) return;

            bool bAllowLocal = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Local);
            bool bAllowFora = gtiCore.GetBinaryAccess((int)TAcesso.CadastroBairro_Alterar_Fora);

            if (!bAllowLocal && !bAllowFora) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (UFCombo.SelectedValue.ToString() == "SP" && Convert.ToInt32(CidadeCombo.SelectedValue) == 413 && !bAllowLocal) {
                MessageBox.Show("Acesso não permitido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Excluir este bairro?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                Endereco_bll enderecoRepository = new Endereco_bll(_connection);
                GTI_Models.Models.Bairro reg = new GTI_Models.Models.Bairro {
                    Siglauf = UFCombo.SelectedValue.ToString(),
                    Codcidade = Convert.ToInt16(CidadeCombo.SelectedValue.ToString()),
                    Codbairro = Convert.ToInt16(BairroListBox.SelectedValue.ToString())
                };
                Exception ex= enderecoRepository.Excluir_Bairro(reg);
                if (ex != null) {
                    ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                    eBox.ShowDialog();
                } else
                    CmbCidade_SelectedIndexChanged(sender, e);
            }
        }

        private void LstBairro_DoubleClick(object sender, EventArgs e) {
            if (BairroListBox.SelectedItem == null) return;
            EditButton.PerformClick();
        }
    }
}
