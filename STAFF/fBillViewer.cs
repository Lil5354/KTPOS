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
using System.IO;

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

                    DataTable dtBillInfo = new DataTable();
                    string billQuery = @"
                SELECT 
                    b.ID,
                    CASE WHEN b.BILLTYPE = 1 THEN N'Tại quán' ELSE N'Mang về' END AS BillType,
                    a.FULLNAME as StaffName,
                    FORMAT(b.CHKOUT_TIME, 'dd-MM-yyyy HH:mm') as CheckoutTime,
                    CASE 
                        WHEN b.BILLTYPE = 1 THEN t.FNAME 
                        WHEN b.BILLTYPE = 0 THEN N'Mang đi'
                        ELSE N'Mang đi'
                    END as TableInfo
                FROM BILL b
                JOIN ACCOUNT a ON b.IDSTAFF = a.IDSTAFF
                LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID
                WHERE b.ID = @BillID";

                    using (SqlCommand cmdBill = new SqlCommand(billQuery, sqlCon))
                    {
                        cmdBill.Parameters.AddWithValue("@BillID", _billId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdBill))
                        {
                            adapter.Fill(dtBillInfo);
                        }
                    }

                    using (SqlCommand cmdItems = new SqlCommand("sp_CalculateBillDetails", sqlCon))
                    {
                        cmdItems.CommandType = CommandType.StoredProcedure;
                        cmdItems.Parameters.AddWithValue("@IDBILL", _billId);
                        DataTable dtDetails = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdItems))
                        {
                            adapter.Fill(dtDetails);
                        }

                        var firstRow = dtDetails.Rows[0];
                        decimal subtotal = Convert.ToDecimal(firstRow["subtotal"]);
                        decimal totalDiscount = Convert.ToDecimal(firstRow["totaldiscount"]);
                        decimal total = Convert.ToDecimal(firstRow["total"]);

                        // Read note from file with default value
                        string noteContent = "Không có";
                        string notePath = $@"E:\App\ok\KTPOS\Note\BillNote{_billId}.txt";
                        if (File.Exists(notePath))
                        {
                            string fileContent = File.ReadAllText(notePath);
                            if (!string.IsNullOrWhiteSpace(fileContent))
                            {
                                noteContent = fileContent;
                            }
                        }

                        rpBill.ProcessingMode = ProcessingMode.Local;
                        rpBill.LocalReport.ReportEmbeddedResource = "KTPOS.STAFF.rptBill.rdlc";
                        rpBill.LocalReport.DataSources.Clear();
                        rpBill.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtDetails));

                        if (dtBillInfo.Rows.Count > 0)
                        {
                            var row = dtBillInfo.Rows[0];
                            var parameters = new ReportParameter[]
                            {
                                new ReportParameter("BillID", row["ID"].ToString()),
                                new ReportParameter("BillType", row["BillType"].ToString()),
                                new ReportParameter("StaffName", row["StaffName"].ToString()),
                                new ReportParameter("CheckoutTime", row["CheckoutTime"].ToString()),
                                new ReportParameter("Table", row["TableInfo"].ToString()),
                                new ReportParameter("Subtotal", subtotal.ToString("N0")),
                                new ReportParameter("TotalDiscount", totalDiscount.ToString("N0")),
                                new ReportParameter("Total", total.ToString("N0")),
                                new ReportParameter("Note", noteContent)
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
