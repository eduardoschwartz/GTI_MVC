
namespace GTI_Desktop.Forms {
    partial class Lista_Devedor {
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
            this.BarToolStrip = new System.Windows.Forms.ToolStrip();
            this.FindButton = new System.Windows.Forms.ToolStripButton();
            this.ExitButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Label2 = new System.Windows.Forms.Label();
            this.Codigo1Text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Codigo2Text = new System.Windows.Forms.TextBox();
            this.PBar = new System.Windows.Forms.ToolStripProgressBar();
            this.BarToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // BarToolStrip
            // 
            this.BarToolStrip.AllowMerge = false;
            this.BarToolStrip.BackColor = System.Drawing.SystemColors.Control;
            this.BarToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BarToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.BarToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindButton,
            this.ExitButton,
            this.toolStripSeparator1,
            this.PBar});
            this.BarToolStrip.Location = new System.Drawing.Point(0, 268);
            this.BarToolStrip.Name = "BarToolStrip";
            this.BarToolStrip.Padding = new System.Windows.Forms.Padding(6, 0, 1, 0);
            this.BarToolStrip.Size = new System.Drawing.Size(611, 25);
            this.BarToolStrip.TabIndex = 148;
            this.BarToolStrip.Text = "toolStrip1";
            // 
            // FindButton
            // 
            this.FindButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FindButton.Image = global::GTI_Desktop.Properties.Resources.Consultar;
            this.FindButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FindButton.Name = "FindButton";
            this.FindButton.Size = new System.Drawing.Size(23, 22);
            this.FindButton.Text = "toolStripButton4";
            this.FindButton.ToolTipText = "Abrir tela de pesquisa";
            this.FindButton.Click += new System.EventHandler(this.FindButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExitButton.Image = global::GTI_Desktop.Properties.Resources.Exit;
            this.ExitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(23, 22);
            this.ExitButton.Text = "toolStripButton5";
            this.ExitButton.ToolTipText = "Sair";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(7, 20);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(64, 13);
            this.Label2.TabIndex = 149;
            this.Label2.Text = "Código de..:";
            // 
            // Codigo1Text
            // 
            this.Codigo1Text.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Codigo1Text.Location = new System.Drawing.Point(77, 17);
            this.Codigo1Text.MaxLength = 6;
            this.Codigo1Text.Name = "Codigo1Text";
            this.Codigo1Text.Size = new System.Drawing.Size(60, 20);
            this.Codigo1Text.TabIndex = 147;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 151;
            this.label1.Text = "até..:";
            // 
            // Codigo2Text
            // 
            this.Codigo2Text.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.Codigo2Text.Location = new System.Drawing.Point(184, 17);
            this.Codigo2Text.MaxLength = 6;
            this.Codigo2Text.Name = "Codigo2Text";
            this.Codigo2Text.Size = new System.Drawing.Size(60, 20);
            this.Codigo2Text.TabIndex = 150;
            // 
            // PBar
            // 
            this.PBar.ForeColor = System.Drawing.Color.Maroon;
            this.PBar.Name = "PBar";
            this.PBar.Size = new System.Drawing.Size(100, 22);
            this.PBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // Lista_Devedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 293);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Codigo2Text);
            this.Controls.Add(this.BarToolStrip);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Codigo1Text);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Lista_Devedor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lista de Devedores";
            this.Load += new System.EventHandler(this.Lista_Devedor_Load);
            this.BarToolStrip.ResumeLayout(false);
            this.BarToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip BarToolStrip;
        private System.Windows.Forms.ToolStripButton FindButton;
        private System.Windows.Forms.ToolStripButton ExitButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox Codigo1Text;
        internal System.Windows.Forms.Label label1;
        internal System.Windows.Forms.TextBox Codigo2Text;
        private System.Windows.Forms.ToolStripProgressBar PBar;
    }
}