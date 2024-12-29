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
            this.Close();
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
                    string filePath = "E:\\App\\KTPOS\\Note\\BillNote" + tableId.ToString() + ".txt";
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
                                    row1.Cells["PRICE"].Value = (price*d).ToString();
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
            ListBill.Rows.Clear(); // Xóa dữ liệu cũ

            string queryString = @"
            SELECT 
            CASE 
                WHEN t.fname IS NULL THEN 'Take Away'
                ELSE t.fname 
            END AS TableName,
            ISNULL(t.capacity, 1) AS Quantity, -- Lấy giá trị từ CAPACITY, mặc định là 10 nếu NULL
            b.datepayment AS DatePayment,
            'Cash' AS Method,
            CASE 
                WHEN b.status = 1 THEN 'Done'
                WHEN b.status = 0 THEN 'Not Paid'
            END AS StatusText,
            'Print' AS PrintButton
        FROM BILL b
        LEFT JOIN [TABLE] t ON b.IDTABLE = t.ID
        ORDER BY b.status, b.ID DESC;";

            try
            {
                // Trước khi load data, cấu hình combobox column
                DataGridViewComboBoxColumn methodColumn = ListBill.Columns["Method"] as DataGridViewComboBoxColumn;
                if (methodColumn != null)
                {
                    methodColumn.Items.Clear();
                    methodColumn.Items.AddRange(new string[] { "Transfer", "Cash" });
                }

                DataTable data = GetDatabase.Instance.ExecuteQuery(queryString);
                foreach (DataRow row in data.Rows)
                {
                    ListBill.Rows.Add(
                        row["TableName"],
                        row["Quantity"],
                        row["DatePayment"],
                        row["Method"],
                        row["StatusText"],
                        row["PrintButton"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bill data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void fStaff_F_Load(object sender, EventArgs e)
        {
            LoadBillData();
        }

        private void flTable_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flTable_MouseClick(object sender, MouseEventArgs e)
        {

        }
    }
}
