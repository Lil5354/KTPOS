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
using System.Xml.Linq;
using KTPOS.Proccess;
using KTPOS.STAFF;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;

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
                        query = "SELECT T.FNAME AS [TABLE], T.CAPACITY, T.STATUS, ISNULL(SUM(BI.COUNT * I.PRICE), 0) AS [TOTAL PRICE], ISNULL(SUM(B.DURATION), 0) AS [TOTAL DURATION]" +
                            "  FROM [TABLE] T " +
                            "LEFT JOIN BILL B ON T.ID = B.IDTABLE AND B.STATUS = 1 LEFT JOIN BILLINF BI ON B.ID = BI.IDBILL LEFT JOIN ITEM I ON BI.IDFD = I.ID WHERE T.VISIBLE = 1 " +
                            "GROUP BY T.FNAME, T.CAPACITY, T.STATUS ORDER BY [TOTAL PRICE] DESC;";
                        DTManger.Instance.LoadList(query, dtgvTable);
                        index = -1;
                        break;
                    case "BILL":
                        query = @"SELECT 
                        ID,
                        CASE 
                            WHEN BILLTYPE = 1 THEN 'Dine-In'
                            ELSE 'Take Away'
                        END AS [TYPE],
                        CHKIN_TIME AS [CHK-IN],
                        CHKOUT_TIME AS [CHK-OUT],
                        DURATION,
                        CASE 
                            WHEN STATUS = 1 THEN 'Paid'
                            ELSE 'Unpaid'
                        END AS PAYMENT
                    FROM BILL;";
                        DTManger.Instance.LoadList(query, dtgvBill);
                        index = -1;
                        break;
                    case "F&B":
                        query = @"SELECT 
                            I.FNAME AS [ITEM NAME],
                            I.CATEGORY,
                            I.PRICE,
                            ISNULL(SUM(BI.COUNT), 0) AS QTY,
                            I.SALESFLAG,
                            ROUND(I.PRICE * (1 - I.DISCOUNTRATE / 100), 2) AS [PRICE DISCOUNT]
                        FROM
                            ITEM I
                        LEFT JOIN
                            BILLINF BI ON I.ID = BI.IDFD
                        WHERE
                            I.VISIBLE = 1
                        GROUP BY
                            I.FNAME, I.CATEGORY, I.PRICE, I.SALESFLAG, I.DISCOUNTRATE; ";
                        DTManger.Instance.LoadList(query, dtgvFandB);
                        q = "SELECT DISTINCT CATEGORY FROM [ITEM]";
                        GetDatabase.Instance.LoadDataToComboBox(q, cbCategoriesFB);
                        index = -1;
                        break;
                    case "REVENUE":
                        query = "SELECT b.ID AS [ID BILL], t.fname AS [TABLE NAME], SUM(i.price * bi.count) AS [TOTAL PRICE], b.Datepayment AS [DATE CHECKOUT] FROM BILLINF bi " +
                            "JOIN Bill b ON bi.idBill = b.ID JOIN [TABLE] t ON b.idTable = t.ID JOIN ITEM i ON bi.idFD = i.ID GROUP BY b.ID, t.fname, b.Datepayment ORDER BY b.ID;";
                        DTManger.Instance.LoadList(query, dtgvRevenue);
                        index = -1;
                        break;
                    case "SALETAG":
                        cbbTag_SelectedIndexChanged(sender, e);
                        q = "SELECT TAGNAME FROM TAG";
                        GetDatabase.Instance.LoadDataToComboBox(q,cbbTag);
                        break;
                    default:
                        query = "";
                        break;
                }
            }
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
                dtpDOB.Text = row.Cells[2].Value?.ToString();
                txtEmail.Text = row.Cells[3].Value?.ToString();
                txtPhone.Text = row.Cells[4].Value?.ToString();
                cbBRole.Text = row.Cells[5].Value?.ToString();
            }
        }
        private void ClearTxtAccount()
        {
            txtSearchAcc.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            cbBRole.SelectedIndex = -1;
        }

        private void cbbTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            query = @"SELECT I.FNAME AS [ITEM NAME], MAX(CASE WHEN T.TAGNAME = '"+cbbTag.Text+"' THEN 1 ELSE 0 END) AS TAG, I.SALESFLAG as SALEFLAG FROM ITEM I LEFT JOIN ITEM_TAG IT ON I.ID = IT.IDITEM LEFT JOIN TAG T ON IT.IDTAG = T.ID GROUP BY I.FNAME, I.SALESFLAG ORDER BY I.FNAME; ";
            DTManger.Instance.LoadList(query, dtgvSaleTag);
        }

        private void dtgvFandB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị của cột "SalesFlag" tại dòng được chọn
                DataGridViewRow row = dtgvFandB.Rows[e.RowIndex];
                bool isSalesFlagChecked = Convert.ToBoolean(row.Cells[4].Value);
                // Hiển thị hoặc ẩn Label và TextBox "Discount Rate" dựa vào trạng thái checkbox
                lbDiscount.Visible = isSalesFlagChecked;    
                txtDiscountR.Visible = isSalesFlagChecked;
                txtNameFB.Text = row.Cells[0].Value?.ToString();
                cbCategoriesFB.Text = row.Cells[1].Value?.ToString();
                txtPriceFB.Text = row.Cells[2].Value?.ToString();
                if (row.Cells[2].Value != null && row.Cells[5].Value != null)
                {
                    decimal price = Convert.ToDecimal(row.Cells[2].Value);
                    decimal priceDiscount = Convert.ToDecimal(row.Cells[5].Value);
                    // Tính Discount Rate (%) = ((PRICE - PRICE_DISCOUNT) / PRICE) * 100
                    decimal discountRate = (price - priceDiscount) / price * 100;
                    // Gán giá trị tính được vào TextBox
                    txtDiscountR.Text = discountRate.ToString("0.##")+"%";
                }
            }
        }

        
    }
}
