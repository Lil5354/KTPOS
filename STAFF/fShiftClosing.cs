using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTPOS.Proccess;
using static KTPOS.fLogin;

namespace KTPOS.STAFF
{
    public partial class fShiftClosing : Form
    {
        public fShiftClosing()
        {
            InitializeComponent();
            lblShiftStartTime.Text = LoginInfo.LoginTime.ToString("dd/MM/yyyy HH:mm:ss");
            lblDate.Text = LoginInfo.LoginTime.ToString("dd/MM/yyyy");
            lblEmployee.Text = LoginInfo.EmployeeName;
            lblOpeningBalance.TextChanged += lblOpeningBalance_TextChanged;
            lblRevenue.TextChanged += lblRevenue_TextChanged;
            lblShiftStartTime.Visible = false;
            fStaff_F.CashTotalChanged += UpdateCashLabel;
            fStaff_F.TransferTotalChanged += UpdateTransferLabel;
            UpdateCashLabel(null, fStaff_F.CashTotal);
            UpdateTransferLabel(null, fStaff_F.TransferTotal);
            LoadDailyStatistics();
            UpdateFinancialStatistics();
            // Tính toán ban đầu
            CalculateClosingBalance();
        }
        private void UpdateCashLabel(object sender, decimal amount)
        {
            if (!IsDisposed && lblCash != null)
            {
                if (lblCash.InvokeRequired)
                {
                    lblCash.Invoke(new Action(() => {
                        if (!IsDisposed && lblCash != null)
                            lblCash.Text = $"{amount:N0} VND";
                    }));
                }
                else
                {
                    lblCash.Text = $"{amount:N0} VND";
                }
            }
        }

        private void UpdateTransferLabel(object sender, decimal amount)
        {
            if (!IsDisposed && lblTransfer != null)
            {
                if (lblTransfer.InvokeRequired)
                {
                    lblTransfer.Invoke(new Action(() => {
                        if (!IsDisposed && lblTransfer != null)
                            lblTransfer.Text = $"{amount:N0} VND";
                    }));
                }
                else
                {
                    lblTransfer.Text = $"{amount:N0} VND";
                }
            }
        }

        private void LoadDailyStatistics()
        {
            try
            {
                lblTotalInvoices.Text = "0";
                lblRevenue.Text = "0";
                DateTime shiftStartTime = DateTime.ParseExact(lblShiftStartTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                string query = $@"
WITH BillDetails AS (
    SELECT 
        B.ID,
        SUM(bi.COUNT * i.PRICE) as OriginalAmount,
        SUM(bi.COUNT * i.PRICE) - B.TOTAL as Discount,
        B.TOTAL as FinalAmount
    FROM BILL B
    JOIN BILLINF bi ON B.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF
    WHERE B.CHKOUT_TIME >= '{shiftStartTime:yyyy-MM-dd HH:mm:ss}'
    AND A.FULLNAME = N'{lblEmployee.Text.Replace("'", "''")}'
    AND B.STATUS = 1
    GROUP BY B.ID, B.TOTAL
)
SELECT 
    COUNT(ID) as TotalInvoices,
    ISNULL(SUM(Discount), 0) as TotalDiscounts,
    ISNULL(SUM(FinalAmount), 0) as TotalRevenue
FROM BillDetails";

                DataTable result = GetDatabase.Instance.ExecuteQuery(query);
                if (result != null && result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    int totalInvoices = row["TotalInvoices"] != DBNull.Value ?
                                      Convert.ToInt32(row["TotalInvoices"]) : 0;
                    lblTotalInvoices.Text = totalInvoices.ToString();

                    decimal totalDiscounts = row["TotalDiscounts"] != DBNull.Value ?
                                           Convert.ToDecimal(row["TotalDiscounts"]) : 0;
                    lblDiscounts.Text = string.Format("{0:N0} VND", totalDiscounts);

                    decimal totalRevenue = row["TotalRevenue"] != DBNull.Value ?
                                         Convert.ToDecimal(row["TotalRevenue"]) : 0;
                    lblRevenue.Text = string.Format("{0:N0} VND", totalRevenue);
                }
                else
                {
                    SetDefaultValues();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetDefaultValues();
            }
        }
        private void UpdateFinancialStatistics()
        {
            try
            {
                DateTime shiftStartTime = DateTime.ParseExact(lblShiftStartTime.Text, "dd/MM/yyyy HH:mm:ss", null);
                string query = $@"
WITH BillDetails AS (
    SELECT 
        B.ID,
        SUM(bi.COUNT * i.PRICE) as TotalSales,
        SUM(bi.COUNT * i.PRICE) - B.TOTAL as Discount
    FROM BILL B
    JOIN BILLINF bi ON B.ID = bi.IDBILL
    JOIN ITEM i ON bi.IDFD = i.ID
    JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF
    WHERE B.CHKOUT_TIME >= '{shiftStartTime:yyyy-MM-dd HH:mm:ss}'
    AND A.FULLNAME = N'{lblEmployee.Text.Replace("'", "''")}'
    AND B.STATUS = 1
    GROUP BY B.ID, B.TOTAL
)
SELECT 
    CAST(ISNULL(SUM(TotalSales), 0) AS decimal(10,0)) AS TotalSales,
    CAST(ISNULL(SUM(Discount), 0) AS decimal(10,0)) AS TotalDiscounts
FROM BillDetails";

                using (DataTable result = GetDatabase.Instance.ExecuteQuery(query))
                {
                    if (result != null && result.Rows.Count > 0)
                    {
                        DataRow row = result.Rows[0];
                        lblTotalSales.Text = $"{row["TotalSales"]:N0} VND";
                        lblDiscounts.Text = $"{row["TotalDiscounts"]:N0} VND";
                    }
                    else
                    {
                        lblTotalSales.Text = "0 VND";
                        lblDiscounts.Text = "0 VND";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating financial statistics: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblTotalSales.Text = "0 VND";
                lblDiscounts.Text = "0 VND";
            }
        }
        private void SetDefaultValues()
        {
            lblTotalInvoices.Text = "0";
            lblDiscounts.Text = "0 VND";
            lblRevenue.Text = "0 VND";
        }
        private void CalculateClosingBalance()
        {
            try
            {
                // Parse giá trị từ lblOpeningBalance, bỏ qua các ký tự không phải số
                string openingText = new string(lblOpeningBalance.Text.Where(c => char.IsDigit(c)).ToArray());
                decimal openingBalance = decimal.Parse(openingText);

                // Parse giá trị từ lblRevenue
                string revenueText = new string(lblRevenue.Text.Where(c => char.IsDigit(c)).ToArray());
                decimal revenue = decimal.Parse(revenueText);

                // Tính tổng
                decimal closingBalance = openingBalance + revenue;

                // Hiển thị kết quả với định dạng tiền tệ
                lblClosingBalance.Text = string.Format("{0:N0} VND", closingBalance);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu parse không thành công
                MessageBox.Show("Error calculating closing balance: " + ex.Message);
                lblClosingBalance.Text = "0 VND";
            }
        }

        private void lblOpeningBalance_TextChanged(object sender, EventArgs e)
        {
            CalculateClosingBalance();
        }

        private void lblRevenue_TextChanged(object sender, EventArgs e)
        {
            CalculateClosingBalance();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            try
            {
                fStaff_F.ResetTotals();
                // Create and show new login form
                fLogin loginForm = new fLogin();

                // Clear the login info
                LoginInfo.EmployeeID = null;
                LoginInfo.EmployeeName = null;
                LoginInfo.LoginTime = DateTime.MinValue;
                string targetFormName = "fStaff_F"; // Tên form cần đóng.

                // Duyệt qua tất cả các form đang mở.
                foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
                {
                    // Kiểm tra nếu form trùng tên và không phải form hiện tại.
                    if (openForm.Name == targetFormName && openForm != Form.ActiveForm)
                    {
                        openForm.Close(); // Đóng form.
                        break; // Thoát khỏi vòng lặp sau khi đóng.
                    }
                }
                // Hide current form
                this.Close();
                

                // Reset the login form fields
                foreach (Control control in loginForm.Controls)
                {
                    if (control is TextBox)
                    {
                        ((TextBox)control).Clear();
                    }
                }

                // Show login form
                loginForm.Show();

                // Close current form
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning to login: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
