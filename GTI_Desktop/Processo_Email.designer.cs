
namespace GTI_Desktop.Forms {
    partial class Processo_Email {
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
            this.label2 = new System.Windows.Forms.Label();
            this.DataInicioMask = new System.Windows.Forms.MaskedTextBox();
            this.DataFinalMask = new System.Windows.Forms.MaskedTextBox();
            this.GerarButton = new System.Windows.Forms.Button();
            this.pBar = new System.Windows.Forms.ProgressBar();
            this.MainListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data de";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data Até";
            // 
            // DataInicioMask
            // 
            this.DataInicioMask.Location = new System.Drawing.Point(88, 12);
            this.DataInicioMask.Mask = "00/00/0000";
            this.DataInicioMask.Name = "DataInicioMask";
            this.DataInicioMask.Size = new System.Drawing.Size(100, 20);
            this.DataInicioMask.TabIndex = 0;
            // 
            // DataFinalMask
            // 
            this.DataFinalMask.Location = new System.Drawing.Point(268, 12);
            this.DataFinalMask.Mask = "00/00/0000";
            this.DataFinalMask.Name = "DataFinalMask";
            this.DataFinalMask.Size = new System.Drawing.Size(100, 20);
            this.DataFinalMask.TabIndex = 1;
            // 
            // GerarButton
            // 
            this.GerarButton.Image = global::GTI_Desktop.Properties.Resources.executar;
            this.GerarButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GerarButton.Location = new System.Drawing.Point(392, 10);
            this.GerarButton.Name = "GerarButton";
            this.GerarButton.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.GerarButton.Size = new System.Drawing.Size(75, 23);
            this.GerarButton.TabIndex = 2;
            this.GerarButton.Text = "  Gerar";
            this.GerarButton.UseVisualStyleBackColor = true;
            this.GerarButton.Click += new System.EventHandler(this.GerarButton_Click);
            // 
            // pBar
            // 
            this.pBar.Location = new System.Drawing.Point(495, 22);
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(172, 10);
            this.pBar.TabIndex = 5;
            // 
            // MainListView
            // 
            this.MainListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.MainListView.HideSelection = false;
            this.MainListView.Location = new System.Drawing.Point(12, 49);
            this.MainListView.Name = "MainListView";
            this.MainListView.Size = new System.Drawing.Size(1030, 357);
            this.MainListView.TabIndex = 3;
            this.MainListView.UseCompatibleStateImageBehavior = false;
            this.MainListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Processo";
            this.columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Cod";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 40;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Local";
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Cod";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Cod2";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 40;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Secretaria";
            this.columnHeader6.Width = 150;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Data";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 90;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Dias";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader8.Width = 40;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Assunto";
            this.columnHeader9.Width = 100;
            // 
            // Processo_Email
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 437);
            this.Controls.Add(this.MainListView);
            this.Controls.Add(this.pBar);
            this.Controls.Add(this.GerarButton);
            this.Controls.Add(this.DataFinalMask);
            this.Controls.Add(this.DataInicioMask);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Processo_Email";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Processo_Email";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox DataInicioMask;
        private System.Windows.Forms.MaskedTextBox DataFinalMask;
        private System.Windows.Forms.Button GerarButton;
        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.ListView MainListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
    }
}