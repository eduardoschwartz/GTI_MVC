
namespace GTI_Desktop_Core.Forms {
    partial class Login {
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LoginToolStrip = new System.Windows.Forms.ToolStrip();
            this.SairButton = new System.Windows.Forms.ToolStripButton();
            this.SenhaButton = new System.Windows.Forms.ToolStripButton();
            this.LoginButton = new System.Windows.Forms.ToolStripButton();
            this.PwdText = new System.Windows.Forms.TextBox();
            this.LoginText = new System.Windows.Forms.TextBox();
            this.ServerText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.CancelButton = new System.Windows.Forms.ToolStripButton();
            this.Pwd2Text = new System.Windows.Forms.TextBox();
            this.Pwd1Text = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            this.LoginToolStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::GTI_Desktop_Core.Properties.Resources.GTI_logo;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(3, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(185, 176);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(174, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "PREFEITURA MUNICIPAL";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(197, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "DE JABOTICABAL";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::GTI_Desktop_Core.Properties.Resources.Brasao;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox2.Location = new System.Drawing.Point(354, 7);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(69, 63);
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LoginToolStrip);
            this.panel1.Controls.Add(this.PwdText);
            this.panel1.Controls.Add(this.LoginText);
            this.panel1.Controls.Add(this.ServerText);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(194, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 107);
            this.panel1.TabIndex = 4;
            // 
            // LoginToolStrip
            // 
            this.LoginToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.LoginToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LoginToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.LoginToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SairButton,
            this.SenhaButton,
            this.LoginButton});
            this.LoginToolStrip.Location = new System.Drawing.Point(0, 82);
            this.LoginToolStrip.Name = "LoginToolStrip";
            this.LoginToolStrip.Size = new System.Drawing.Size(231, 25);
            this.LoginToolStrip.TabIndex = 6;
            this.LoginToolStrip.Text = "toolStrip1";
            // 
            // SairButton
            // 
            this.SairButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SairButton.AutoSize = false;
            this.SairButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SairButton.Image = global::GTI_Desktop_Core.Properties.Resources.Exit;
            this.SairButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SairButton.Name = "SairButton";
            this.SairButton.Size = new System.Drawing.Size(63, 22);
            this.SairButton.Text = "Sair";
            this.SairButton.ToolTipText = "Sair do sistema";
            this.SairButton.Click += new System.EventHandler(this.SairButton_Click);
            // 
            // SenhaButton
            // 
            this.SenhaButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SenhaButton.AutoSize = false;
            this.SenhaButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SenhaButton.Image = global::GTI_Desktop_Core.Properties.Resources.download;
            this.SenhaButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SenhaButton.Name = "SenhaButton";
            this.SenhaButton.Size = new System.Drawing.Size(63, 22);
            this.SenhaButton.Text = "Senha";
            this.SenhaButton.ToolTipText = "Mudar a senha";
            this.SenhaButton.Click += new System.EventHandler(this.SenhaButton_Click);
            // 
            // LoginButton
            // 
            this.LoginButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.LoginButton.AutoSize = false;
            this.LoginButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LoginButton.Image = global::GTI_Desktop_Core.Properties.Resources.OK;
            this.LoginButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(63, 22);
            this.LoginButton.Text = "Entrar";
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // PwdText
            // 
            this.PwdText.Location = new System.Drawing.Point(81, 57);
            this.PwdText.MaxLength = 30;
            this.PwdText.Name = "PwdText";
            this.PwdText.PasswordChar = '*';
            this.PwdText.Size = new System.Drawing.Size(148, 23);
            this.PwdText.TabIndex = 5;
            // 
            // LoginText
            // 
            this.LoginText.Location = new System.Drawing.Point(81, 31);
            this.LoginText.MaxLength = 30;
            this.LoginText.Name = "LoginText";
            this.LoginText.Size = new System.Drawing.Size(148, 23);
            this.LoginText.TabIndex = 4;
            // 
            // ServerText
            // 
            this.ServerText.Location = new System.Drawing.Point(81, 5);
            this.ServerText.MaxLength = 20;
            this.ServerText.Name = "ServerText";
            this.ServerText.Size = new System.Drawing.Size(148, 23);
            this.ServerText.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(8, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Senha....:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(8, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Usuário..:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(8, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Servidor.:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(10, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "A senha deve ter no mínimo 6 carácteres";
            // 
            // panel2
            // 
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.toolStrip2);
            this.panel2.Controls.Add(this.Pwd2Text);
            this.panel2.Controls.Add(this.Pwd1Text);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Location = new System.Drawing.Point(7, 226);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(298, 90);
            this.panel2.TabIndex = 6;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveButton,
            this.CancelButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 63);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(296, 25);
            this.toolStrip2.TabIndex = 11;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Image = global::GTI_Desktop_Core.Properties.Resources.gravar;
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(23, 22);
            this.SaveButton.Text = "toolStripButton4";
            this.SaveButton.ToolTipText = "Gravar os dados";
            // 
            // CancelButton
            // 
            this.CancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CancelButton.Image = global::GTI_Desktop_Core.Properties.Resources.cancel2;
            this.CancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(23, 22);
            this.CancelButton.Text = "toolStripButton5";
            this.CancelButton.ToolTipText = "Cancelar";
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // Pwd2Text
            // 
            this.Pwd2Text.Location = new System.Drawing.Point(100, 34);
            this.Pwd2Text.MaxLength = 30;
            this.Pwd2Text.Name = "Pwd2Text";
            this.Pwd2Text.Size = new System.Drawing.Size(181, 23);
            this.Pwd2Text.TabIndex = 10;
            // 
            // Pwd1Text
            // 
            this.Pwd1Text.Location = new System.Drawing.Point(100, 8);
            this.Pwd1Text.MaxLength = 30;
            this.Pwd1Text.Name = "Pwd1Text";
            this.Pwd1Text.Size = new System.Drawing.Size(181, 23);
            this.Pwd1Text.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(6, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Confirmar Senha:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(6, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Nova Senha......:";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Linen;
            this.ClientSize = new System.Drawing.Size(433, 326);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.LoginToolStrip.ResumeLayout(false);
            this.LoginToolStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private PictureBox pictureBox2;
        private Panel panel1;
        private TextBox PwdText;
        private TextBox LoginText;
        private TextBox ServerText;
        private Label label5;
        private Label label4;
        private Label label3;
        private ToolStrip LoginToolStrip;
        private ToolStripButton SairButton;
        private ToolStripButton SenhaButton;
        private ToolStripButton LoginButton;
        private Label label6;
        private Panel panel2;
        private ToolStrip toolStrip2;
        private ToolStripButton SaveButton;
        private ToolStripButton CancelButton;
        private TextBox Pwd2Text;
        private TextBox Pwd1Text;
        private Label label8;
        private Label label7;
        private ToolTip toolTip1;
    }
}