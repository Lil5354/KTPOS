using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTPOS.MANAGER;

namespace KTPOS.STAFF
{
    public partial class fStaff_F : Form
    {
        public fStaff_F()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Do you really want to exit?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Exit cancelled. Continue your activity ❤️.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Focus();
            }
        }

        private void btnMinSize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.Manual;

            // Tùy chỉnh lại vị trí nếu cần (giữ nguyên vị trí hiện tại)
            this.Location = new Point(0, 0);
            btnMinSize.Visible = false;
            btnMaxSize.Visible = true;
        }

        private void btnMaxSize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaxSize.Visible = false;
            btnMinSize.Visible = true;
        }

        private void btnManage_Click(object sender, EventArgs e)
        {
            fManager newForm = new fManager();
            newForm.Show();
            this.Close();
        }
    }
}
