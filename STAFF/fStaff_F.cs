using System;
using System.Collections;
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
using Guna.UI2.WinForms;
using KTPOS.MANAGER;
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class fStaff_F : Form
    {
        private static decimal _cashTotal = 0;
        private static decimal _transferTotal = 0;
        private string staffid;
        private string userRole;
        public static event EventHandler<decimal> CashTotalChanged;
        public static event EventHandler<decimal> TransferTotalChanged;
        public static void ResetTotals()
        {
            _cashTotal = 0;
            _transferTotal = 0;
            CashTotalChanged?.Invoke(null, 0);
            TransferTotalChanged?.Invoke(null, 0);
        }
        public static decimal CashTotal
        {
            get => _cashTotal;
            set
            {
                _cashTotal = value;
                CashTotalChanged?.Invoke(null, value);
            }
        }

        public static decimal TransferTotal
        {
            get => _transferTotal;
            set
            {
                _transferTotal = value;
                TransferTotalChanged?.Invoke(null, value);
            }
        }

        public fStaff_F(string role, string idstaff)
        {
            InitializeComponent();
            ListBill.CellClick += ListBill_CellClick;
            this.userRole = role;
            staffid = idstaff;
            ConfigureUIBasedOnRole();
            LoadTables();
            SetupListBillColumns();
            LoadBillData();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);

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
            this.Hide();
        }
        private void LoadTables()
        {
            try
            {
                string query = "SELECT ID, fname, status FROM [TABLE]";
                DataTable data = GetDatabase.Instance.ExecuteQuery(query);

                flTable.Controls.Clear();

                // Add TakeAway button first
                Guna2Button takeAwayButton = new Guna2Button
                {
                    Text = "Take Away\nAvailable",
                    Width = 100,
                    Height = 100,
                    BorderRadius = 20,
                    FillColor = Color.LightGreen, // Different color to distinguish from tables
                    ForeColor = Color.Maroon,
                    Font = new Font("Segoe UI Semibold", 9, FontStyle.Bold),
                    Margin = new Padding(10),
                    HoverState = { FillColor = Color.Gray },
                    Tag = -1 // Use -1 as special ID for TakeAway
                };

                takeAwayButton.Click += TableButton_Click;
                flTable.Controls.Add(takeAwayButton);

                // Add regular table buttons
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
            if (!(sender is Guna2Button clickedButton)) return;

            int tableId = Convert.ToInt32(clickedButton.Tag);
            string tableName = clickedButton.Text.Split('\n')[0];

            // Special handling for TakeAway
            if (tableId == -1)
            {
                tableName = "Take Away";
            }
            string detailsQuery = $"SELECT IT.FNAME AS NAME, BI.COUNT AS QTY, IT.Price AS PRICE, BI.IDFD AS ID FROM BILL B JOIN BILLINF BI ON B.ID = BI.IDBILL JOIN ITEM IT ON BI.IDFD = IT.ID WHERE B.IDTABLE = {tableId} AND B.STATUS = 0;";
            DataTable billDetails = GetDatabase.Instance.ExecuteQuery(detailsQuery);
            using (fStaff_S staffForm = new fStaff_S(tableId, staffid))
            {
                if (staffForm.Controls.Find("txtTable", true).FirstOrDefault() is Guna2HtmlLabel txtTable)
                {
                    txtTable.Text = tableName;
                }
                if (staffForm.Controls.Find("txtNoteBill", true).FirstOrDefault() is Guna2TextBox txtNoteBill)
                {
                    string filePath = "D:\\clone\\KTPOS - Sao chép\\Note\\BillNote" + tableId.ToString() + ".txt";
                    if (File.Exists(filePath))
                    {
                        string note = File.ReadAllText(filePath);
                        txtNoteBill.Text = note;
                    }
                }
                if (staffForm.Controls.Find("dtgvBillCus", true).FirstOrDefault() is Guna2DataGridView dtgvBillCus &&
                    staffForm.Controls.Find("txtTotal", true).FirstOrDefault() is Guna2HtmlLabel txtTotal)
                {
                    dtgvBillCus.Columns["ID"].Visible = false;
                    dtgvBillCus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    decimal total = 0;
                    dtgvBillCus.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    if (!dtgvBillCus.Columns.Contains("DEL_BUTTON"))
                    {
                        DataGridViewButtonColumn delButtonColumn = new DataGridViewButtonColumn
                        {
                            Name = "DEL_BUTTON",
                            HeaderText = "DEL",
                            Text = "x",
                            UseColumnTextForButtonValue = true,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                        };
                        dtgvBillCus.Columns.Add(delButtonColumn);
                    }

                    dtgvBillCus.CellClick += (s, evt) =>
                    {
                        if (evt.RowIndex >= 0 && evt.ColumnIndex == dtgvBillCus.Columns["DEL_BUTTON"].Index)
                        {
                            var row = dtgvBillCus.Rows[evt.RowIndex];
                            string itemName = row.Cells["NAME"].Value.ToString();
                            int qty = Convert.ToInt32(row.Cells["QTY"].Value);

                            DialogResult confirm = MessageBox.Show(
                                $"Do you want to delete {qty} of {itemName}?",
                                "Confirm Delete",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            );

                            if (confirm == DialogResult.Yes)
                            {
                                dtgvBillCus.Rows.RemoveAt(evt.RowIndex);
                                {
                                    total = 0;
                                    if (dtgvBillCus.DataSource is DataTable dataTable1)
                                    {
                                        if (dtgvBillCus.Rows.Count > 0)
                                        {
                                            foreach (DataGridViewRow row1 in dtgvBillCus.Rows)
                                            {
                                                string priceStr = row1.Cells["PRICE"].Value.ToString().Replace(",", "");
                                                if (decimal.TryParse(priceStr, out decimal price))
                                                {
                                                    total += price;
                                                }
                                            }
                                        }
                                        else total = 0;
                                    }
                                    txtTotal.Text = total.ToString("N0") + " VND";
                                    staffForm.UpdateTotal();
                                }
                            }
                        }
                    };
                }
                this.Hide();
                staffForm.ShowDialog();
            }
            LoadBillData();
        }
        public void LoadBillData()
        {
            ListBill.Rows.Clear();
            string queryString = @"
SELECT 
    CASE 
        WHEN t.fname IS NULL THEN 'Take Away'
        ELSE t.fname 
    END AS TableName,
    b.CHKIN_TIME AS CheckInTime,
    b.CHKOUT_TIME AS CheckOutTime,
    'Cash' AS Method,
    CASE 
        WHEN b.status = 1 THEN 'Done'
        WHEN b.status = 0 THEN 'Not Paid'
    END AS StatusText,
    'Print' AS PrintButton,
    b.ID as BillID,
    b.status as Status
FROM BILL b
LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID
ORDER BY b.status, b.ID DESC";

            try
            {
                DataTable data = GetDatabase.Instance.ExecuteQuery(queryString);
                foreach (DataRow row in data.Rows)
                {
                    DateTime checkInTime = Convert.ToDateTime(row["CheckInTime"]);
                    DateTime? checkOutTime = row["CheckOutTime"] != DBNull.Value ?
                        (DateTime?)Convert.ToDateTime(row["CheckOutTime"]) : null;

                    // Format check-in time
                    string formattedCheckIn = checkInTime.ToString("dd/MM/yyyy HH:mm");

                    // Handle check-out time formatting with validation
                    string formattedCheckOut = "";
                    if (checkOutTime.HasValue)
                    {
                        // If checkout is on a different day, set it to 23:59 of check-in day
                        if (checkOutTime.Value.Date > checkInTime.Date)
                        {
                            checkOutTime = checkInTime.Date.AddHours(23).AddMinutes(59);
                        }
                        formattedCheckOut = checkOutTime.Value.ToString("dd/MM/yyyy HH:mm");
                    }

                    int status = Convert.ToInt32(row["Status"]);
                    DataGridViewRow gridRow = new DataGridViewRow();
                    gridRow.CreateCells(ListBill);
                    gridRow.Cells[0].Value = row["TableName"];    // TABLE
                    gridRow.Cells[1].Value = formattedCheckIn;    // CHECK-IN
                    gridRow.Cells[2].Value = formattedCheckOut;   // CHECK-OUT
                    gridRow.Cells[3].Value = row["Method"];       // METHOD
                    gridRow.Cells[4].Value = status == 1 ? "Done" : "Not Paid"; // PAYMENT
                    gridRow.Cells[5].Value = "Print";             // BILL
                    gridRow.Cells[6].Value = row["BillID"];       // Hidden BillID

                    // Set button cell appearance based on status
                    if (status == 1)
                    {
                        DataGridViewButtonCell paymentCell = gridRow.Cells[4] as DataGridViewButtonCell;
                        if (paymentCell != null)
                        {
                            paymentCell.Style.BackColor = Color.Green;
                            paymentCell.Style.ForeColor = Color.White;
                        }
                    }
                    ListBill.Rows.Add(gridRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bill data: " + ex.Message);
            }
        }
        private void fStaff_F_Load(object sender, EventArgs e)
        {
            LoadBillData();
        }
        private UserControl currentUserControl;
        public void AddUserControl(UserControl userControl)
        {
            if (currentUserControl != null)
            {
                this.Controls.Remove(currentUserControl);
                currentUserControl.Dispose(); // Đảm bảo giải phóng tài nguyên đúng cách
            }

            // Các cải tiến:
            userControl.SuspendLayout(); // Tạm dừng layout để giảm thiểu việc vẽ lại
            this.Controls.Add(userControl);
            userControl.Location = new Point(this.Width - userControl.Width, 103);
            userControl.Anchor = AnchorStyles.Right;
            userControl.BringToFront();
            userControl.ResumeLayout(true); // Khôi phục layout một cách hiệu quả

            currentUserControl = userControl;
        }
        private void ListBill_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = ListBill.Rows[e.RowIndex];
                // Check if clicking the Payment column
                if (ListBill.Columns[e.ColumnIndex].Name == "TABLE")
                {
                    string tableCell = row.Cells["TABLE"].Value?.ToString();
                    int billId = Convert.ToInt32(row.Cells["BillID"].Value);

                    // Query order details
                    string query = @"
        SELECT i.FNAME as ItemName, bi.COUNT as Quantity
        FROM BILLINF bi
        JOIN ITEM i ON bi.IDFD = i.ID
        WHERE bi.IDBILL = @billId";

                    DataTable orderDetails = GetDatabase.Instance.ExecuteQuery(query, new object[] { billId });

                    // Show preview form
                    using (fOrderPreview preview = new fOrderPreview(orderDetails))
                    {
                        preview.ShowDialog();
                    }
                }
                if (ListBill.Columns[e.ColumnIndex].Name == "PAYMENT")
                {
                    string paymentStatus = row.Cells["PAYMENT"].Value?.ToString();
                    string method = row.Cells["METHOD"].Value?.ToString();
                    int billId = Convert.ToInt32(row.Cells["BillID"].Value);

                    if (paymentStatus == "Not Paid" && method == "Cash")
                    {
                        try
                        {
                            // Get final total for the bill
                            string finalTotalQuery = @"
                    SELECT CAST(b.TOTAL as decimal(10,0)) as FinalTotal 
                    FROM BILL b 
                    WHERE b.ID = @billId";

                            object finalTotalResult = GetDatabase.Instance.ExecuteScalar(finalTotalQuery, new object[] { billId });
                            decimal finalTotal = finalTotalResult != null ? Convert.ToDecimal(finalTotalResult) : 0;

                            if (finalTotal <= 0)
                            {
                                MessageBox.Show("Invalid bill amount", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Show payment form
                            using (fPayment paymentForm = new fPayment(finalTotal, billId))
                            {
                                if (paymentForm.ShowDialog() == DialogResult.OK)
                                {
                                    // Update the ListBill display
                                    row.Cells["PAYMENT"].Value = "Done";
                                    row.Cells["METHOD"].Value = "Cash";
                                    row.Cells["CHECKOUT"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                                    _cashTotal += finalTotal;
                                    DataGridViewButtonCell paymentCell = row.Cells["PAYMENT"] as DataGridViewButtonCell;
                                    if (paymentCell != null)
                                    {
                                        paymentCell.Style.BackColor = Color.Green;
                                        paymentCell.Style.ForeColor = Color.White;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error processing payment: " + ex.Message);
                        }
                    }
                    else if (paymentStatus == "Not Paid" && method == "Transfer")
                    {
                        try
                        {
                            // Debug logging
                            Console.WriteLine($"Starting QR generation for bill");

                            // Verify billId exists and is valid
                            if (billId == null)
                            {
                                MessageBox.Show("Bill ID is null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Convert billId to string then parse to int to ensure valid format
                            string billIdStr = billId.ToString();
                            if (!int.TryParse(billIdStr, out int parsedBillId))
                            {
                                MessageBox.Show($"Invalid bill ID format: {billIdStr}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            Console.WriteLine($"Parsed bill ID: {parsedBillId}");

                            // Get table name with null check
                            string tableName = "Unknown";
                            if (row?.Cells != null && row.Cells["TABLE"]?.Value != null)
                            {
                                tableName = row.Cells["TABLE"].Value.ToString();
                            }

                            Console.WriteLine($"Table name: {tableName}");

                            // Simplified query to test database connection
                            string testQuery = "SELECT COUNT(*) FROM BILL WHERE ID = @billId";
                            object testResult = GetDatabase.Instance.ExecuteScalar(testQuery, new object[] { parsedBillId });

                            if (Convert.ToInt32(testResult) == 0)
                            {
                                MessageBox.Show("Bill not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Calculate total with simplified query first
                            string finalTotalQuery = @"
                            SELECT CAST(b.TOTAL as decimal(10,0)) as FinalTotal 
                            FROM BILL b 
                            WHERE b.ID = @billId";

                            object finalTotalResult = GetDatabase.Instance.ExecuteScalar(finalTotalQuery, new object[] { parsedBillId });
                            decimal finalTotal = finalTotalResult != null ? Convert.ToDecimal(finalTotalResult) : 0;
                            Console.WriteLine($"Final total calculated: {finalTotal}");


                            if (finalTotal <= 0)
                            {
                                MessageBox.Show("Invalid bill amount (zero or negative)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Create QR payment control with explicit error handling
                            try
                            {
                                Console.WriteLine("Creating UC_QRPayment control");
                                UC_QRPayment ucQrPayment = new UC_QRPayment();
                                ucQrPayment.GetBillId(parsedBillId);
                                TransferTotal += finalTotal;
                                Console.WriteLine("Updating QR code");
                                ucQrPayment.UpdateQRCode(
                                    content: $"Bill {parsedBillId} - {tableName}",
                                    amount: finalTotal
                                );
                                _transferTotal += finalTotal;
                                Console.WriteLine("Adding control to form");
                                AddUserControl(ucQrPayment);
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show($"Error in QR control creation: {innerEx.Message}\nStack trace: {innerEx.StackTrace}",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error showing QR payment: {ex.Message}\nStack trace: {ex.StackTrace}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Console.WriteLine($"Exception details: {ex}");
                        }
                    }
                }
                else if (ListBill.Columns[e.ColumnIndex].Name == "BILL")
                {
                    try
                    {
                        object checkoutTime = ListBill.Rows[e.RowIndex].Cells["CHECKOUT"].Value;
                        if (checkoutTime == null || checkoutTime == DBNull.Value)
                        {
                            MessageBox.Show("This bill has not been paid yet", "Unpaid Bill",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        // Get BillID directly from the row with null check
                        if (ListBill.Rows[e.RowIndex].Cells["BillID"].Value == null)
                        {
                            MessageBox.Show("Invalid Bill ID", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int billId = Convert.ToInt32(ListBill.Rows[e.RowIndex].Cells["BillID"].Value);

                        // Validate billId
                        if (billId <= 0)
                        {
                            MessageBox.Show("Invalid Bill ID value", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Create and show the bill viewer form with try-finally
                        fBillViewer billViewer = new fBillViewer(billId);
                        try
                        {
                            billViewer.MinimizeBox = false;
                            billViewer.MaximizeBox = false;
                            billViewer.StartPosition = FormStartPosition.CenterParent;
                            billViewer.ShowDialog(this);
                        }
                        finally
                        {
                            billViewer.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error printing bill: {ex.Message}\nStack Trace: {ex.StackTrace}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void SetupListBillColumns()
        {
            ListBill.Columns.Clear();

            // Setup các columns
            ListBill.Columns.Add("TABLE", "TABLE");
            ListBill.Columns.Add("CHECKIN", "CHECKIN");
            ListBill.Columns.Add("CHECKOUT", "CHECKOUT");

            // Setup Method ComboBox column
            DataGridViewComboBoxColumn methodColumn = new DataGridViewComboBoxColumn();
            methodColumn.Name = "METHOD";
            methodColumn.HeaderText = "METHOD";
            methodColumn.Items.AddRange(new string[] { "Cash", "Transfer" });
            ListBill.Columns.Add(methodColumn);

            // Setup Payment Button column
            DataGridViewButtonColumn paymentColumn = new DataGridViewButtonColumn();
            paymentColumn.Name = "PAYMENT";
            paymentColumn.HeaderText = "PAYMENT";
            paymentColumn.FlatStyle = FlatStyle.Flat;
            ListBill.Columns.Add(paymentColumn);

            // Setup Bill Button column
            DataGridViewButtonColumn billColumn = new DataGridViewButtonColumn();
            billColumn.Name = "BILL";
            billColumn.HeaderText = "BILL";
            billColumn.Text = "Print";
            billColumn.UseColumnTextForButtonValue = true;
            ListBill.Columns.Add(billColumn);

            // Add hidden BillID column
            DataGridViewTextBoxColumn billIdColumn = new DataGridViewTextBoxColumn();
            billIdColumn.Name = "BillID";
            billIdColumn.Visible = false;
            ListBill.Columns.Add(billIdColumn);

            // Set column styles
            ListBill.EnableHeadersVisualStyles = false;
            ListBill.ColumnHeadersDefaultCellStyle.BackColor = Color.Maroon;
            ListBill.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            ListBill.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Filter.SelectedItem == null) return;
            string selectedFilter = Filter.SelectedItem.ToString();
            ListBill.Rows.Clear();
            string queryString = @"
SELECT 
    CASE 
        WHEN t.fname IS NULL THEN 'Take Away'
        ELSE t.fname 
    END AS TableName,
    FORMAT(b.CHKIN_TIME, 'dd/MM/yyyy HH:mm') AS CheckIn,
    FORMAT(b.CHKOUT_TIME, 'dd/MM/yyyy HH:mm') AS CheckOut,
    'Cash' AS Method,
    CASE 
        WHEN b.status = 1 THEN 'Done'
        WHEN b.status = 0 THEN 'Not Paid'
    END AS StatusText,
    'Print' AS PrintButton,
    b.ID as BillID,
    b.status as Status
FROM BILL b
LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID
WHERE 1=1 "; // Base condition to make it easier to add filters

            switch (selectedFilter)
            {
                case "Newest":
                    queryString += " ORDER BY b.CHKIN_TIME DESC, b.ID DESC";
                    break;
                case "Oldest":
                    queryString += " ORDER BY b.CHKIN_TIME ASC, b.ID ASC";
                    break;
                case "Not Paid":
                    queryString += " AND b.status = 0 ORDER BY b.ID DESC";
                    break;
                case "Done":
                    queryString += " AND b.status = 1 ORDER BY b.ID DESC";
                    break;
                case "Take Away":
                    queryString += " AND b.BILLTYPE = 0 ORDER BY b.ID DESC";
                    break;
                default:
                    queryString += " ORDER BY b.status, b.ID DESC";
                    break;
            }

            try
            {
                DataTable data = GetDatabase.Instance.ExecuteQuery(queryString);
                foreach (DataRow row in data.Rows)
                {
                    int status = Convert.ToInt32(row["Status"]);
                    DataGridViewRow gridRow = new DataGridViewRow();
                    gridRow.CreateCells(ListBill);
                    gridRow.Cells[0].Value = row["TableName"];  // TABLE
                    gridRow.Cells[1].Value = row["CheckIn"];    // CHECK-IN
                    gridRow.Cells[2].Value = row["CheckOut"];   // CHECK-OUT
                    gridRow.Cells[3].Value = row["Method"];     // METHOD
                    gridRow.Cells[4].Value = status == 1 ? "Done" : "Not Paid"; // PAYMENT
                    gridRow.Cells[5].Value = "Print";           // BILL
                    gridRow.Cells[6].Value = row["BillID"];     // Hidden BillID

                    if (status == 1)
                    {
                        DataGridViewButtonCell paymentCell = gridRow.Cells[4] as DataGridViewButtonCell;
                        if (paymentCell != null)
                        {
                            paymentCell.Style.BackColor = Color.Green;
                            paymentCell.Style.ForeColor = Color.White;
                        }
                    }
                    ListBill.Rows.Add(gridRow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading filtered data: " + ex.Message);
            }
        }

        private void ListBill_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnCloseShift_Click(object sender, EventArgs e)
        {
            fShiftClosing shiftClosingForm = new fShiftClosing();
            shiftClosingForm.ShowDialog();
        }
    }
}

