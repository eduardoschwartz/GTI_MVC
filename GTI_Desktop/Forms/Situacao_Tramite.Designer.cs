namespace GTI_Desktop.Forms {
    partial class Situacao_Tramite {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lvMain = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NumeroText = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.AnoText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ConsultarButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LocalCodigoText = new System.Windows.Forms.Label();
            this.DataText = new System.Windows.Forms.Label();
            this.ArquivadoText = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspensoText = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.LocalNomeText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvMain
            // 
            this.lvMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader6,
            this.columnHeader11,
            this.columnHeader7,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader10,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader12});
            this.lvMain.FullRowSelect = true;
            this.lvMain.GridLines = true;
            this.lvMain.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMain.HideSelection = false;
            this.lvMain.Location = new System.Drawing.Point(3, 38);
            this.lvMain.MinimumSize = new System.Drawing.Size(565, 176);
            this.lvMain.Name = "lvMain";
            this.lvMain.Size = new System.Drawing.Size(831, 243);
            this.lvMain.TabIndex = 160;
            this.lvMain.UseCompatibleStateImageBehavior = false;
            this.lvMain.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Obs";
            this.columnHeader1.Width = 32;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Sq";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 30;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "CCCod";
            this.columnHeader11.Width = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Local";
            this.columnHeader7.Width = 230;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Data";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Hora";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 50;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Recebido por";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Despacho";
            this.columnHeader5.Width = 100;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Dias";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader10.Width = 35;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Dt.Envio";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 70;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Usuário";
            this.columnHeader9.Width = 90;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Observação";
            this.columnHeader12.Width = 0;
            // 
            // NumeroText
            // 
            this.NumeroText.Location = new System.Drawing.Point(77, 12);
            this.NumeroText.MaxLength = 6;
            this.NumeroText.Name = "NumeroText";
            this.NumeroText.Size = new System.Drawing.Size(63, 20);
            this.NumeroText.TabIndex = 161;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Label1.Location = new System.Drawing.Point(16, 15);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(53, 13);
            this.Label1.TabIndex = 162;
            this.Label1.Text = "Número..:";
            // 
            // AnoText
            // 
            this.AnoText.Location = new System.Drawing.Point(197, 12);
            this.AnoText.MaxLength = 4;
            this.AnoText.Name = "AnoText";
            this.AnoText.Size = new System.Drawing.Size(63, 20);
            this.AnoText.TabIndex = 163;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label2.Location = new System.Drawing.Point(153, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 164;
            this.label2.Text = "Ano...:";
            // 
            // ConsultarButton
            // 
            this.ConsultarButton.Image = global::GTI_Desktop.Properties.Resources.Consultar;
            this.ConsultarButton.Location = new System.Drawing.Point(266, 10);
            this.ConsultarButton.Name = "ConsultarButton";
            this.ConsultarButton.Size = new System.Drawing.Size(23, 22);
            this.ConsultarButton.TabIndex = 165;
            this.ConsultarButton.UseVisualStyleBackColor = true;
            this.ConsultarButton.Click += new System.EventHandler(this.ConsultarButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label3.Location = new System.Drawing.Point(12, 297);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 166;
            this.label3.Text = "Encontra-se..:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label4.Location = new System.Drawing.Point(483, 297);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 167;
            this.label4.Text = "Desde..:";
            // 
            // LocalCodigoText
            // 
            this.LocalCodigoText.AutoSize = true;
            this.LocalCodigoText.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.LocalCodigoText.ForeColor = System.Drawing.Color.Maroon;
            this.LocalCodigoText.Location = new System.Drawing.Point(91, 297);
            this.LocalCodigoText.Name = "LocalCodigoText";
            this.LocalCodigoText.Size = new System.Drawing.Size(13, 13);
            this.LocalCodigoText.TabIndex = 168;
            this.LocalCodigoText.Text = "0";
            // 
            // DataText
            // 
            this.DataText.AutoSize = true;
            this.DataText.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.DataText.ForeColor = System.Drawing.Color.Maroon;
            this.DataText.Location = new System.Drawing.Point(536, 297);
            this.DataText.Name = "DataText";
            this.DataText.Size = new System.Drawing.Size(65, 13);
            this.DataText.TabIndex = 169;
            this.DataText.Text = "__/__/____";
            // 
            // ArquivadoText
            // 
            this.ArquivadoText.AutoSize = true;
            this.ArquivadoText.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ArquivadoText.ForeColor = System.Drawing.Color.Maroon;
            this.ArquivadoText.Location = new System.Drawing.Point(687, 297);
            this.ArquivadoText.Name = "ArquivadoText";
            this.ArquivadoText.Size = new System.Drawing.Size(27, 13);
            this.ArquivadoText.TabIndex = 171;
            this.ArquivadoText.Text = "Não";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label6.Location = new System.Drawing.Point(617, 297);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 170;
            this.label6.Text = "Arquivado..:";
            // 
            // SuspensoText
            // 
            this.SuspensoText.AutoSize = true;
            this.SuspensoText.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.SuspensoText.ForeColor = System.Drawing.Color.Maroon;
            this.SuspensoText.Location = new System.Drawing.Point(797, 297);
            this.SuspensoText.Name = "SuspensoText";
            this.SuspensoText.Size = new System.Drawing.Size(27, 13);
            this.SuspensoText.TabIndex = 173;
            this.SuspensoText.Text = "Não";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.label8.Location = new System.Drawing.Point(731, 297);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 172;
            this.label8.Text = "Suspenso..:";
            // 
            // LocalNomeText
            // 
            this.LocalNomeText.AutoSize = true;
            this.LocalNomeText.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.LocalNomeText.ForeColor = System.Drawing.Color.Maroon;
            this.LocalNomeText.Location = new System.Drawing.Point(124, 297);
            this.LocalNomeText.Name = "LocalNomeText";
            this.LocalNomeText.Size = new System.Drawing.Size(16, 13);
            this.LocalNomeText.TabIndex = 174;
            this.LocalNomeText.Text = "...";
            // 
            // Situacao_Tramite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 326);
            this.Controls.Add(this.LocalNomeText);
            this.Controls.Add(this.SuspensoText);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.ArquivadoText);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.DataText);
            this.Controls.Add(this.LocalCodigoText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ConsultarButton);
            this.Controls.Add(this.AnoText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NumeroText);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lvMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Situacao_Tramite";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Local em que um processo se encontra";
            this.Load += new System.EventHandler(this.Situacao_Tramite_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvMain;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.TextBox NumeroText;
        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox AnoText;
        internal System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ConsultarButton;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Label LocalCodigoText;
        internal System.Windows.Forms.Label DataText;
        internal System.Windows.Forms.Label ArquivadoText;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.Label SuspensoText;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label LocalNomeText;
    }
}