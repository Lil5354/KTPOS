using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTPOS.Proccess;
using KTPOS.STAFF;

namespace KTPOS.MANAGER
{
    public partial class fManager : Form
    {
        int index = -1;
        string query = "", q = "";
        public fManager()
        {
            InitializeComponent();
            this.Load += ManagementControl_Load;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);
        }
        private void ManagementControl_Load(object sender, EventArgs e)
        {
            // Đặt trạng thái mặc định khi UserControl được load
            tcManager.SelectedTab = null;
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
        private void btnMaxSize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaxSize.Visible = false;
            btnMinSize.Visible = true;
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
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Đóng form hiện tại (fStaff_S)

            // Hiển thị lại form trước đó, tức là fStaff_F
            // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
            fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
            previousForm?.Show();
        }
        private void tcManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcManager.SelectedTab != null)
            {
                string selectedTab = tcManager.SelectedTab.Text;
                switch (selectedTab)
                {
                    case "ACCOUNT":
                        query = "SELECT IDSTAFF AS ID, FULLNAME AS [FULL NAME], DOB AS [D.O.B], EMAIL, PHONE, [ROLE] FROM ACCOUNT WHERE Visible = 1 Order by [Role] ASC";
                        DTManger.Instance.LoadList(query, dtgvAccount);
                        index = -1;
                        break;
                    case "TABLE":
                        query = "SELECT T.FNAME AS [TABLE], T.CAPACITY AS QTY, T.STATUS, ISNULL(SUM(BI.COUNT * I.PRICE), 0) AS [TOTAL PRICE]  FROM [TABLE] T " +
                            "LEFT JOIN BILL B ON T.ID = B.IDTABLE AND B.STATUS = 1 LEFT JOIN BILLINF BI ON B.ID = BI.IDBILL LEFT JOIN ITEM I ON BI.IDFD = I.ID WHERE T.VISIBLE = 1 " +
                            "GROUP BY T.FNAME, T.CAPACITY, T.STATUS ORDER BY [TOTAL PRICE] DESC;";
                        DTManger.Instance.LoadList(query, dtgvTable);
                        index = -1;
                        break;
                    case "BILL":
                        query = "SELECT fname AS [NAME CATEGORIES] FROM [F&BCATEGORY] WHERE Visible = 1";
                        //DTManger.Instance.LoadList(query);
                        index = -1;
                        break;
                    case "F&B":
                        query = "SELECT fb.fname [TYPE], i.fname AS [NAME], price[PRICE] FROM ITEM i JOIN [F&BCATEGORY] fb ON idCategory = fb.ID  " +
                            "WHERE i.Visible = 1 Order by [Type] ASC";
                        DTManger.Instance.LoadList(query, dtgvFandB);
                        q = "SELECT fname AS [NAME CATEGORIES] FROM [F&BCATEGORY] WHERE Visible = 1";
                        GetDatabase.Instance.LoadDataToComboBox(q, cbCategoriesFB);
                        index = -1;
                        break;
                    case "REVENUE":
                        query = "SELECT b.ID AS [ID BILL], t.fname AS [TABLE NAME], SUM(i.price * bi.count) AS [TOTAL PRICE], b.Datepayment AS [DATE CHECKOUT] FROM BILLINF bi " +
                            "JOIN Bill b ON bi.idBill = b.ID JOIN [TABLE] t ON b.idTable = t.ID JOIN ITEM i ON bi.idFD = i.ID GROUP BY b.ID, t.fname, b.Datepayment ORDER BY b.ID;";
                        DTManger.Instance.LoadList(query, dtgvRevenue);
                        index = -1;
                        break;
                    default:
                        query = "";
                        break;
                }
            }
        }
        private void ClearTxtAccount()
        {
            txtSearchAcc.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            cbBRole.SelectedIndex = -1;
        }

        private void cbBRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                index = e.RowIndex;
                // Lấy hàng hiện tại
                DataGridViewRow row = dtgvAccount.Rows[e.RowIndex];
                // Gán dữ liệu từ các cột vào TextBox
                txtFullName.Text = row.Cells[1].Value?.ToString();
                txtEmail.Text = row.Cells[3].Value?.ToString();
                cbBRole.Text = row.Cells[5].Value?.ToString();
            }
        }
    }
}
