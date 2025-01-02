using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

            // Tính toán ban đầu
            CalculateClosingBalance();
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
    }
}
