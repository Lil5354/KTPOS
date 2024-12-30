using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private string userRole;
        public fStaff_F(string role)
        {
            InitializeComponent();
            this.userRole = role;
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

            const string billQuery = @"
        SELECT b.ID 
        FROM Bill b 
        WHERE b.idTable = @tableId AND b.status = 0";

            object billIdResult = GetDatabase.Instance.ExecuteScalar(billQuery, new object[] { tableId });

            if (billIdResult == null)
            {
                MessageBox.Show("No unpaid bill found for this table.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int billId = Convert.ToInt32(billIdResult);
            const string detailsQuery = @"
        SELECT 
            i.FNAME as NAME,
            bi.COUNT as QTY,
            i.PRICE as PRICE,
            i.ID as ID
        FROM BILLINF bi
        JOIN ITEM i ON bi.IDFD = i.ID
        WHERE bi.IDBILL = @billId";

            DataTable billDetails = GetDatabase.Instance.ExecuteQuery(detailsQuery, new object[] { billId });
            using (fStaff_S staffForm = new fStaff_S(tableId))
            {
                if (staffForm.Controls.Find("txtTable", true).FirstOrDefault() is Guna2HtmlLabel txtTable)
                {
                    txtTable.Text = tableName;
                }
                if (staffForm.Controls.Find("txtNoteBill", true).FirstOrDefault() is Guna2TextBox txtNoteBill)
                {
                    string filePath = "E:\\App\\ok\\KTPOS\\Note\\BillNote" + tableId.ToString() + ".txt";
                    if (File.Exists(filePath))
                    {
                        string note = File.ReadAllText(filePath);
                        txtNoteBill.Text = note;
                    }
                }
                if (staffForm.Controls.Find("dtgvBillCus", true).FirstOrDefault() is Guna2DataGridView dtgvBillCus && staffForm.Controls.Find("txtTotal", true).FirstOrDefault() is Guna2HtmlLabel txtTotal)
                {
                    dtgvBillCus.DataSource = billDetails;
                    dtgvBillCus.Columns["ID"].Visible = false;
                    dtgvBillCus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    decimal total = 0;
                    if (dtgvBillCus.DataSource is DataTable dataTable)
                    {
                        if (dtgvBillCus.Rows.Count > 0)
                        {
                            foreach (DataGridViewRow row1 in dtgvBillCus.Rows)
                            {
                                string priceStr = row1.Cells["PRICE"].Value.ToString().Replace(",", "");
                                int d = int.Parse(row1.Cells["QTY"].Value.ToString());
                                if (decimal.TryParse(priceStr, out decimal price))
                                {
                                    row1.Cells["PRICE"].Value = (price * d).ToString();
                                }
                                total += price * d;
                            }
                        }
                        else total = 0;
                    }
                    txtTotal.Text = total.ToString("N0") + " VND";
                    // Thiết lập canh giữa tiêu đề các cột
                    dtgvBillCus.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    // Kiểm tra nếu cột "DEL" chưa tồn tại, thì thêm cột Button
                    if (!dtgvBillCus.Columns.Contains("DEL_BUTTON"))
                    {
                        DataGridViewButtonColumn delButtonColumn = new DataGridViewButtonColumn
                        {
                            Name = "DEL_BUTTON",
                            HeaderText = "DEL",
                            Text = "x",
                            UseColumnTextForButtonValue = true, // Hiển thị "Delete" làm nút
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                        };
                        dtgvBillCus.Columns.Add(delButtonColumn);
                    }

                    // Xử lý sự kiện khi click vào cột "DEL_BUTTON"
                    dtgvBillCus.CellClick += (s, evt) =>
                    {
                        if (evt.RowIndex >= 0 && evt.ColumnIndex == dtgvBillCus.Columns["DEL_BUTTON"].Index)
                        {
                            // Lấy thông tin dòng được click
                            var row = dtgvBillCus.Rows[evt.RowIndex];
                            string itemName = row.Cells["NAME"].Value.ToString();
                            int qty = Convert.ToInt32(row.Cells["QTY"].Value);

                            // Hành động xóa dòng (có thể tùy chỉnh theo logic)
                            DialogResult confirm = MessageBox.Show(
                                $"Do you want to delete {qty} of {itemName}?",
                                "Confirm Delete",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning
                            );

                            if (confirm == DialogResult.Yes)
                            {
                                // Thực hiện xóa ở DataGridView (hoặc cập nhật trong cơ sở dữ liệu)
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
                                }
                            }
                        }
                    };
                }
                this.Hide();
                staffForm.ShowDialog();
            }
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
    ISNULL(t.capacity, 1) AS Quantity,
    b.datepayment AS DatePayment,
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
                    int status = Convert.ToInt32(row["Status"]);
                    DataGridViewRow gridRow = new DataGridViewRow();
                    gridRow.CreateCells(ListBill);

                    gridRow.Cells[0].Value = row["TableName"];  // TABLE
                    gridRow.Cells[1].Value = row["Quantity"];   // QTY
                    gridRow.Cells[2].Value = row["DatePayment"]; // TIME
                    gridRow.Cells[3].Value = row["Method"];     // METHOD
                    gridRow.Cells[4].Value = status == 1 ? "Done" : "Not Paid"; // PAYMENT
                    gridRow.Cells[5].Value = "Print";           // BILL
                    gridRow.Cells[6].Value = row["BillID"];     // Hidden BillID

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
        private object selectedBillId;

        public int SelectedBillId { get; internal set; }

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
                if (ListBill.Columns[e.ColumnIndex].Name == "PAYMENT")
                {
                    string paymentStatus = row.Cells["PAYMENT"].Value?.ToString();
                    string method = row.Cells["METHOD"].Value?.ToString();
                    int billId = Convert.ToInt32(row.Cells["BillID"].Value);

                    if (paymentStatus == "Not Paid" && method == "Cash")
                    {
                        try
                        {
                            string query = "UPDATE BILL SET STATUS = 1, datepayment = GETDATE() WHERE ID = @billId";
                            int result = GetDatabase.Instance.ExecuteNonQuery(query, new object[] { billId });

                            if (result > 0)
                            {
                                row.Cells["PAYMENT"].Value = "Done";
                                row.Cells["METHOD"].Value = "Cash"; // Set method to Cash
                                DataGridViewButtonCell paymentCell = row.Cells["PAYMENT"] as DataGridViewButtonCell;
                                if (paymentCell != null)
                                {
                                    paymentCell.Style.BackColor = Color.Green;
                                    paymentCell.Style.ForeColor = Color.White;
                                }

                                MessageBox.Show("Payment completed successfully.", "Success",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error updating payment status: " + ex.Message);
                        }
                    }
                    else if (paymentStatus == "Not Paid" && method == "Transfer")
                    {
                        try
                        {
                            string tableName = row.Cells["TABLE"].Value?.ToString() ?? "Unknown";

                            // Calculate total amount from bill items
                            string totalQuery = @"
                        SELECT SUM(bi.COUNT * i.PRICE) as TotalAmount
                        FROM BILLINF bi
                        JOIN ITEM i ON bi.IDFD = i.ID
                        WHERE bi.IDBILL = @billId";

                            object totalResult = GetDatabase.Instance.ExecuteScalar(totalQuery, new object[] { billId });
                            decimal totalAmount = totalResult != null ? Convert.ToDecimal(totalResult) : 0;

                            // Create and show UC_QRPayment with the correct billId
                            UC_QRPayment ucQrPayment = new UC_QRPayment();
                            ucQrPayment.GetBillId(billId); // Pass the actual billId

                            ucQrPayment.UpdateQRCode(
                                content: $"Bill {billId} - {tableName}",
                                amount: totalAmount
                            );

                            AddUserControl(ucQrPayment);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error showing QR payment: " + ex.Message);
                        }
                    }
                }
            }
        }
        private void SetupListBillColumns()
        {
            ListBill.Columns.Clear();

            // Setup các columns
            ListBill.Columns.Add("TABLE", "TABLE");
            ListBill.Columns.Add("QTY", "QTY");
            ListBill.Columns.Add("TIME", "TIME");

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
            ISNULL(t.capacity, 1) AS Quantity,
            b.datepayment AS DatePayment,
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
                        queryString += " ORDER BY b.datepayment DESC, b.ID DESC";
                        break;
                    case "Oldest":
                        queryString += " ORDER BY b.datepayment ASC, b.ID ASC";
                        break;
                    case "Not Paid":
                        queryString += " AND b.status = 0 ORDER BY b.ID DESC";
                        break;
                    case "Done":
                        queryString += " AND b.status = 1 ORDER BY b.ID DESC";
                        break;
                    case "Take Away":
                        queryString += " AND t.fname IS NULL ORDER BY b.ID DESC";
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

                        gridRow.Cells[0].Value = row["TableName"];
                        gridRow.Cells[1].Value = row["Quantity"];
                        gridRow.Cells[2].Value = row["DatePayment"];
                        gridRow.Cells[3].Value = row["Method"];
                        gridRow.Cells[4].Value = status == 1 ? "Done" : "Not Paid";
                        gridRow.Cells[5].Value = "Print";
                        gridRow.Cells[6].Value = row["BillID"];

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
        }
    }

