using System.Reflection;

namespace GTI_Desktop_Core
{
    public partial class Main : Form
    {
        
        private const int CP_NOCLOSE_BUTTON = 0x200;

        protected override CreateParams CreateParams {
            get {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        public Main()
        {
            InitializeComponent();
            DoubleBuffered = true;
            MaquinaToolStripStatus.Text = Environment.MachineName;
            DataBaseToolStripStatus.Text = "PRODUÇÂO";
            ServidorToolStripStatus.Text = gtiProperty.ServerName;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            IsMdiContainer = true;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            FillBackgroundColor(false);
            VersaoToolStripStatus.Text = gtiProperty.Version;

            LockForm(true);
            Forms.Login login = new Forms.Login();
            login.ShowDialog();

        }

        private void FillBackgroundColor(bool bTeste) {
            Color cor = bTeste ? Color.FromArgb(250, 218, 226) : Color.LightBlue;
            CorFundo(cor);
        }

        private void CorFundo(Color cor) {
            MdiClient ctlMDI;

            foreach (Control ctl in Controls) {
                try {
                    ctlMDI = (MdiClient)ctl;
                    ctlMDI.BackColor = cor;
                } catch {
                }
            }
        }

        public void Ocupado(Form frm) {
            LedGreen.Enabled = false;
            LedRed.Enabled = true;
            Refresh();
            frm.Cursor = Cursors.WaitCursor;
            frm.Refresh();
            Application.DoEvents();
        }

        public void Liberado(Form frm) {
            LedGreen.Enabled = true;
            LedRed.Enabled = false;
            frm.Cursor = Cursors.Default;
            frm.Refresh();
            Application.DoEvents();
        }

        public void Main_FormClosing(object sender, FormClosingEventArgs e) {
            Application.Exit();
        }

        public void LockForm(bool bLocked) {
            foreach (ToolStripItem ts in TopBarToolStrip.Items) {
                ts.Enabled = !bLocked;
            }

            List<ToolStripMenuItem> mItems = gtiCore.GetItems(this.MenuBarStrip);
            foreach (var item in mItems) {
                item.Enabled = !bLocked;
            }
        }

        private void SairButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Deseja fechar todas as janelas e retornar a tela de login?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                Form[] charr = this.MdiChildren;
                foreach (Form chform in charr) {
                    chform.Close();
                }
                LockForm(true);
                Forms.Login login = new Forms.Login();
                login.ShowDialog();
            }
        }
    }
}