using GTI_Desktop.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTI_Desktop {
    public partial class Main : Form {

        string _connection = gtiCore.Connection_Name();
        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams {
            get {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }


        public Main() {
            InitializeComponent();
            this.DoubleBuffered = true;
            DateTimePicker t = new DateTimePicker {
                AutoSize = false,
                Width = 100,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Name = "sbDataBase",
            };
            TopBarToolStrip.Renderer = new MySR();
            t.CloseUp += new System.EventHandler(SbDataBase_CloseUp);

            BarStatus.Items.Insert(17, new ToolStripControlHost(t));
            MaquinaToolStripStatus.Text = Environment.MachineName;
            DataBaseToolStripStatus.Text = Properties.Settings.Default.DataBaseReal;
        }

        private void SbDataBase_CloseUp(object sender, EventArgs e) {
            MessageBox.Show(ReturnDataBaseValue().ToString());
        }

        public DateTime ReturnDataBaseValue() {
            DateTime dValue = DateTime.Today;

            DateTimePicker dbox;
            foreach (Control c in BarStatus.Controls) {
                if (c is DateTimePicker) {
                    dbox = c as DateTimePicker;
                    dValue = dbox.Value.Date;
                }
            }
            return dValue;
        }


        private void SairButton_Click(object sender, EventArgs e) {
            //if (MessageBox.Show("Deseja fechar todas as janelas e retornar a tela de login?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
            //    Form[] charr = this.MdiChildren;
            //    foreach (Form chform in charr) {
            //        chform.Close();
            //    }
            //    LockForm(true);
            //    Forms.Login login = new Forms.Login();
            //    login.ShowDialog();
            //}
        }
    }
}
