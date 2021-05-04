
namespace GTI_Desktop.Forms {
    partial class ProcessoEmCCusto {
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
            this.label1 = new System.Windows.Forms.Label();
            this.LocalList = new System.Windows.Forms.ComboBox();
            this.PrintButton = new System.Windows.Forms.Button();
            this.PBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Local..:";
            // 
            // LocalList
            // 
            this.LocalList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LocalList.FormattingEnabled = true;
            this.LocalList.Location = new System.Drawing.Point(71, 37);
            this.LocalList.Name = "LocalList";
            this.LocalList.Size = new System.Drawing.Size(398, 21);
            this.LocalList.TabIndex = 1;
            // 
            // PrintButton
            // 
            this.PrintButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PrintButton.Image = global::GTI_Desktop.Properties.Resources.print;
            this.PrintButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PrintButton.Location = new System.Drawing.Point(475, 33);
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.PrintButton.Size = new System.Drawing.Size(74, 25);
            this.PrintButton.TabIndex = 10;
            this.PrintButton.Text = "Imprimir";
            this.PrintButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.PrintButton.UseVisualStyleBackColor = true;
            this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
            // 
            // PBar
            // 
            this.PBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PBar.Location = new System.Drawing.Point(295, 72);
            this.PBar.Name = "PBar";
            this.PBar.Size = new System.Drawing.Size(174, 8);
            this.PBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.PBar.TabIndex = 163;
            // 
            // ProcessoEmCCusto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 92);
            this.Controls.Add(this.PBar);
            this.Controls.Add(this.PrintButton);
            this.Controls.Add(this.LocalList);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ProcessoEmCCusto";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Processos que estão em um Centro de Custos";
            this.Load += new System.EventHandler(this.ProcessoEmCCusto_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox LocalList;
        private System.Windows.Forms.Button PrintButton;
        private System.Windows.Forms.ProgressBar PBar;
    }
}