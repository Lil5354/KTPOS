using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class fPayment : Form
    {
        private decimal finalTotal;
        private int billId;
        public fPayment(decimal total, int id)
        {
            InitializeComponent();
            txtTotal.Visible = false;
            finalTotal = total;
            billId = id;
            txtTotal.Text = finalTotal.ToString("N0") + " VND";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtCustomerPayment.Text.Replace(",", ""), out decimal customerPayment))
            {
                decimal change = customerPayment - finalTotal;
                if (change >= 0)
                {
                    MessageBox.Show($"Change to return: {change:N0} VND", "Change", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Update bill status
                    string query = "UPDATE BILL SET STATUS = 1, CHKOUT_TIME = GETDATE() WHERE ID = @billId";
                    int result = GetDatabase.Instance.ExecuteNonQuery(query, new object[] { billId });

                    if (result > 0)
                    {
                        MessageBox.Show("Payment successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Insufficient payment amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
