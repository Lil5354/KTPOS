using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using KTPOS.MANAGER;
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class fStaff_F : Form
    {
        private string userRole;
        public fStaff_F(string role)
        {
            InitializeComponent();
            this.userRole = role;
            ConfigureUIBasedOnRole();
            LoadTables();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);

        }

        public fStaff_F()
        {
        }

        private void ConfigureUIBasedOnRole()
        {
            // Hide the manager button if the user role is Staff
            btnManage.Visible = userRole == "Manager";
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
        private void LoadTables()
        {
            try
            {
                string query = "SELECT ID, fname, status FROM [TABLE]";
                DataTable data = GetDatabase.Instance.ExecuteQuery(query);

                flTable.Controls.Clear();

                foreach (DataRow row in data.Rows)
                {
                    int tableId = Convert.ToInt32(row["ID"]);
                    string tableName = row["fname"].ToString();
                    int status = Convert.ToInt32(row["status"]);
                    string buttonText = status == 1 ? $"{tableName}\nAvailable" : $"{tableName}\nUnavailable";

                    Guna2Button tableButton = new Guna2Button
                    {
                        Text = buttonText,
                        Width = 100,
                        Height = 100,
                        BorderRadius = 20,
                        FillColor = status == 1 ? Color.White : Color.BurlyWood,
                        ForeColor = Color.Maroon,
                        Font = new Font("Segoe UI Semibold", 9, FontStyle.Bold),
                        Margin = new Padding(10),
                        HoverState = { FillColor = Color.Gray },
                        Tag = tableId
                    };

                    tableButton.Click += TableButton_Click;

                    flTable.Controls.Add(tableButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void TableButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
