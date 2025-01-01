using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;

namespace KTPOS.STAFF
{
    public partial class fBillViewer : Form
    {
        private readonly string strcon = @"Data Source=LAPTOP-T5G4R7PV\SQLEXPRESS01;Initial Catalog=KTPOS;Integrated Security=True";
        public int _billId { get; set; } // 3,8,10 lỗi

        public fBillViewer(int billId)
        {
            InitializeComponent();
            _billId = billId;
        }
        private void LoadBillReport()
        {
            try
    {
        using (SqlConnection sqlCon = new SqlConnection(strcon))
        {
            sqlCon.Open();

            // Get bill details
            DataTable dtBillInfo = new DataTable();
            string billQuery = @"
                SELECT 
                    b.ID,
                    CASE WHEN b.BILLTYPE = 1 THEN N'Tại quán' ELSE N'Mang về' END AS BillType,
                    a.FULLNAME as StaffName,
                    FORMAT(b.CHKOUT_TIME, 'dd-MM-yyyy HH:mm') as CheckoutTime
                FROM BILL b
                JOIN ACCOUNT a ON b.IDSTAFF = a.IDSTAFF
                WHERE b.ID = @BillID";

            using (SqlCommand cmdBill = new SqlCommand(billQuery, sqlCon))
            {
                cmdBill.Parameters.AddWithValue("@BillID", _billId);
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmdBill))
                {
                    adapter.Fill(dtBillInfo);
                }
            }

            // Get bill items and totals
            using (SqlCommand cmdItems = new SqlCommand("sp_CalculateBillDetails", sqlCon))
            {
                cmdItems.CommandType = CommandType.StoredProcedure;
                cmdItems.Parameters.AddWithValue("@IDBILL", _billId);
                DataTable dtDetails = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmdItems))
                {
                    adapter.Fill(dtDetails);
                }

                // Get the totals from the first row
                var firstRow = dtDetails.Rows[0];
                decimal subtotal = Convert.ToDecimal(firstRow["subtotal"]);
                decimal totalDiscount = Convert.ToDecimal(firstRow["totaldiscount"]);
                decimal total = Convert.ToDecimal(firstRow["total"]);

                rpBill.ProcessingMode = ProcessingMode.Local;
                rpBill.LocalReport.ReportEmbeddedResource = "KTPOS.STAFF.rptBill.rdlc";
                rpBill.LocalReport.DataSources.Clear();
                rpBill.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtDetails));

                // Add parameters for the textboxes
                if (dtBillInfo.Rows.Count > 0)
                {
                    var row = dtBillInfo.Rows[0];
                    var parameters = new ReportParameter[]
                    {
                        new ReportParameter("BillID", row["ID"].ToString()),
                        new ReportParameter("BillType", row["BillType"].ToString()),
                        new ReportParameter("StaffName", row["StaffName"].ToString()),
                        new ReportParameter("CheckoutTime", row["CheckoutTime"].ToString()),
                        new ReportParameter("Subtotal", subtotal.ToString("N0")),
                        new ReportParameter("TotalDiscount", totalDiscount.ToString("N0")),
                        new ReportParameter("Total", total.ToString("N0"))
                    };
                    rpBill.LocalReport.SetParameters(parameters);
                }

                rpBill.RefreshReport();
            }
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error loading report: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
        }
        private void fBillViewer_Load(object sender, EventArgs e)
        {
            LoadBillReport();
        }
    }
}
