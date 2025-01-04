using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace KTPOS.STAFF
{
    public partial class fBillCheff : Form
    {
        private readonly string strcon = @"Data Source=LAPTOP-T5G4R7PV\SQLEXPRESS01;Initial Catalog=KTPOS;Integrated Security=True";
        private object rptBillChef;

        public int _billId { get; set; } // 3,8,10 lỗi
        public fBillCheff(int idBill)
        {
            InitializeComponent();
            _billId = idBill;
        }
        private void LoadBillReport()
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(strcon))
                {
                    sqlCon.Open();

                    // Load bill info
                    DataTable dtBillInfo = new DataTable();
                    string billQuery = @"
                SELECT 
                    b.ID,
                    CASE WHEN b.BILLTYPE = 1 THEN N'Tại quán' ELSE N'Mang về' END AS BillType,
                    a.FULLNAME as StaffName,
                    FORMAT(b.CHKIN_TIME, 'dd-MM-yyyy HH:mm') as ChkinTime,
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

                    // Sử dụng procedure mới
                    using (SqlCommand cmdItems = new SqlCommand("sp_GetBillItemsAndQuantities", sqlCon))
                    {
                        cmdItems.CommandType = CommandType.StoredProcedure;
                        cmdItems.Parameters.AddWithValue("@IDBILL", _billId);
                        DataTable dtDetails = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmdItems))
                        {
                            adapter.Fill(dtDetails);
                        }
                        var orderGroups = dtDetails.AsEnumerable()
                        .GroupBy(row => row.Field<string>("ThuTuOrder"))
                        .OrderBy(g => g.Key)
                        .ToList();

                        // Read note
                        string noteContent = "Không có";
                        string notePath = $@"D:\clone\Thư mục mới\KTPOS\Note\BillNote{_billId}.txt";
                        if (File.Exists(notePath))
                        {
                            string fileContent = File.ReadAllText(notePath);
                            if (!string.IsNullOrWhiteSpace(fileContent))
                            {
                                noteContent = fileContent;
                            }
                        }

                        rptBillCheff.ProcessingMode = ProcessingMode.Local;
                        rptBillCheff.LocalReport.ReportEmbeddedResource = "KTPOS.STAFF.rptBillCheff.rdlc";
                        rptBillCheff.LocalReport.DataSources.Clear();
                        rptBillCheff.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dtDetails));

                        if (dtBillInfo.Rows.Count > 0)
                        {
                            var row = dtBillInfo.Rows[0];
                            var parameters = new ReportParameter[]
                            {
                        new ReportParameter("BillID", row["ID"].ToString()),
                        new ReportParameter("StaffName", row["StaffName"].ToString()),
                        new ReportParameter("ChkinTime", row["ChkinTime"].ToString()),
                        new ReportParameter("Table", row["TableInfo"].ToString()),
                        new ReportParameter("Note", noteContent)
                            };
                            rptBillCheff.LocalReport.SetParameters(parameters);
                        }

                        rptBillCheff.RefreshReport();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void fBillCheff_Load(object sender, EventArgs e)
        {
            LoadBillReport();
            this.rptBillCheff.RefreshReport();
        }
    }
}
