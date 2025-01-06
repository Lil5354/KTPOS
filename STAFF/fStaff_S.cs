//code tham khảo để lấy data từ txtNote đưa vào parameter Note trong report
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
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class fStaff_S : Form
    {
        private int idTable = 0;
        private int idBill = 0;
        private string idStaff;
        private int idCus = 0;
        private DataTable billTable;
        private int billdiscount = 0;
        private bool checkbill = false;
        private bool checkclose = false;
        private string Time;
        private bool checkdelete = true;
        public fStaff_S(int tableId, string staffId)
        {
            InitializeComponent();
            LoadFilter();
            UpdateTotal();
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
            SetDoubleBuffered(FlowMenu, true);
            SetDoubleBuffered(dtgvBillCus, true);
            SetDoubleBuffered(btnMaxSize, true);
            SetDoubleBuffered(btnMinSize, true);
            SetDoubleBuffered(PanelBillCus, true);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);
            nUDItem.Visible = false;
            numericDown.Visible = false;
            numericUp.Visible = false;
            idTable = tableId;
            idStaff = staffId;
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            InitializeBillTable();
            set();
            getidbill();
            CheckDiscount();
        }
        private void getidbill()
        {
            if (idTable > 0)
            {
                string query = $"SELECT ID FROM BILL WHERE IDTABLE = {idTable} AND STATUS = 0";
                DataTable table = GetDatabase.Instance.ExecuteQuery(query);
                foreach (DataRow row in table.Rows)
                {
                    idBill = Convert.ToInt32(row["ID"]);
                    checkbill = true;
                    break;
                }
            }
        }
        public void set()
        {
            getidbill();
            CheckDiscount();
            string query = $"SELECT c.PHONE AS PHONE FROM BILL b JOIN CUSTOMER c ON b.IDCUSTOMER = c.ID WHERE b.ID = '{idBill}'";
            DataTable dt = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row1 in dt.Rows)
            {
                string number = row1["PHONE"].ToString();
                if (number != null)
                {
                    txtPhone.Text = number;
                }
            }
            AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();
            query = "SELECT PHONE FROM CUSTOMER";
            DataTable dataTable = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                suggestions.Add(row["PHONE"].ToString());
            }
            txtPhone.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtPhone.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtPhone.AutoCompleteCustomSource = suggestions;
        }
        private void CheckDiscount()
        {
            try
            {
                string query2 = "SELECT DISCOUNT FROM PROMOTION WHERE GETDATE() BETWEEN [START_DATE] AND END_DATE AND APPLY_TO = 'Bill' ORDER BY DISCOUNT DESC;";
                DataTable data2 = GetDatabase.Instance.ExecuteQuery(query2);
                foreach (DataRow row2 in data2.Rows)
                {
                    billdiscount = Convert.ToInt32(row2["DISCOUNT"]);
                    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading menu items: " + ex.Message);
            }
        }
        private void InitializeBillTable()
        {
            billTable = new DataTable();
            billTable.Columns.Add("NAME", typeof(string));
            billTable.Columns.Add("QTY", typeof(int));
            billTable.Columns.Add("PRICE", typeof(string));
            billTable.Columns.Add("ID", typeof(int));
            dtgvBillCus.DataSource = billTable;
        }

        private void SetDoubleBuffered(Control control, bool value)
        {
            var property = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            property?.SetValue(control, value, null);
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
        private void btnBack_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
            // Kiểm tra nếu có thể quay lại trang trước đó
            this.Hide(); // Đóng form hiện tại (fStaff_S)
            // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
            fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
            previousForm?.Show();
        }
        private void LoadFilter()
        {
            Filter.Items.Clear();
            string query = "SELECT TAGNAME FROM Tag;";
            DataTable data = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in data.Rows)
            {
                Filter.Items.Add(row["TAGNAME"]);
            }
        }
        private void LoadMenuItems(string query)
        {
            try
            {
                int[] menu = new int[100];
                DataTable data = GetDatabase.Instance.ExecuteQuery(query);
                FlowMenu.Controls.Clear();
                foreach (DataRow row in data.Rows)
                {
                    int itemId = Convert.ToInt32(row["ID"]);
                    if (menu[itemId] == 1) continue;
                    menu[itemId] = 1;
                    string itemName = row["FNAME"].ToString();
                    decimal price1 = Convert.ToDecimal(row["PRICE"]);
                    string category = row["CATEGORY"].ToString();
                    decimal discountitem = Convert.ToInt32(row["DISCOUNT"]);
                    if (discountitem > 0) discountitem /= 100;
                    decimal cost1 = price1 - (price1 * discountitem);
                    int cost = (int)cost1;
                    int price = (int)price1;
                    string pricetext;
                    if (price != cost)
                    {
                        pricetext = $"{price:N0} VND -> {cost:N0} VND";
                    }
                    else
                    {
                        pricetext = $"{price:N0} VND";
                    }

                    // Create the main container using Guna2Panel for rounded corners
                    Guna2Panel itemPanel = new Guna2Panel
                    {
                        Width = 150,
                        Height = 180,
                        Margin = new Padding(10),
                        FillColor = Color.White,
                        BorderRadius = 15,
                        ShadowDecoration = {
                    Enabled = true,
                    Depth = 5,
                    Color = Color.FromArgb(20, 0, 0, 0)
                },
                        Cursor = Cursors.Hand
                    };

                    // Create and configure the image using Guna2PictureBox
                    Guna2PictureBox itemImage = new Guna2PictureBox
                    {
                        Width = 100,
                        Height = 100,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Location = new Point(25, 10),
                        BorderRadius = 10
                    };

                    // Load image based on category
                    try
                    {
                        string imagePath = $@"C:\Thư mục mới (2)\KTPOS\Image Items\" + itemName + ".jpg";
                        if (File.Exists(imagePath))
                        {
                            itemImage.Image = Image.FromFile(imagePath);
                        }
                        else
                        {
                            // Load default image based on category
                            string defaultImagePath = $@"../../Images/{category}.png";
                            if (File.Exists(defaultImagePath))
                            {
                                itemImage.Image = Image.FromFile(defaultImagePath);
                            }
                            else
                            {
                                itemImage.Image = Properties.Resources.cocktail; // Your default resource
                            }
                        }
                    }
                    catch
                    {
                        itemImage.Image = Properties.Resources.cocktail; // Your default resource
                    }
                    itemImage.Click += (sender, e) => ItemImage_Click(itemId, itemName, price);
                    // Create and configure the name label
                    Label nameLabel = new Label
                    {
                        Text = itemName,
                        AutoSize = false,
                        Width = 130,
                        Height = 20,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Location = new Point(10, 115),
                        BackColor = Color.Transparent
                    };

                    // Create and configure the price label
                    Label priceLabel = new Label
                    {
                        Text = pricetext,
                        AutoSize = false,
                        Width = 130,
                        Height = 20,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 9),
                        Location = new Point(10, 135),
                        BackColor = Color.Transparent
                    };

                    // Add hover effect using Guna properties
                    itemPanel.MouseEnter += (sender, e) =>
                    {
                        itemPanel.FillColor = Color.FromArgb(240, 240, 240);
                        itemPanel.Cursor = Cursors.Hand;
                    };
                    itemPanel.MouseLeave += (sender, e) =>
                    {
                        itemPanel.FillColor = Color.White;
                    };

                    // Add click event
                    itemPanel.Click += (sender, e) => ItemPanel_Click(itemId, itemName, price);

                    // Add controls to the panel
                    itemPanel.Controls.Add(itemImage);
                    itemPanel.Controls.Add(nameLabel);
                    itemPanel.Controls.Add(priceLabel);

                    // Add panel to the flow layout
                    FlowMenu.Controls.Add(itemPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading menu items: " + ex.Message);
            }
        }
        private void ItemPanel_Click(int itemId, string itemName, decimal price)
        {
            bool kt = true;
            foreach (DataGridViewRow row in dtgvBillCus.Rows)
            {
                if (row.Cells["NAME"].Value.ToString() == itemName)
                {
                    kt = false;
                    int d = int.Parse(row.Cells["QTY"].Value.ToString());
                    d++;
                    decimal cost = (decimal)d * price;
                    row.Cells["QTY"].Value = d.ToString();
                    row.Cells["PRICE"].Value = cost.ToString();
                    break;
                }
            }
            if (kt)
            {
                DataTable currentTable = (DataTable)dtgvBillCus.DataSource;
                DataRow newRow = currentTable.NewRow();
                newRow["NAME"] = itemName;
                newRow["QTY"] = 1;
                newRow["PRICE"] = price.ToString("N0");
                newRow["ID"] = itemId;
                currentTable.Rows.Add(newRow);
            }
            UpdateTotal();
            nUDItem.Visible = false;
            numericDown.Visible = false;
            numericUp.Visible = false;
        }
        private void ItemImage_Click(int itemId, string itemName, decimal price)
        {
            ItemPanel_Click(itemId, itemName, price);
            nUDItem.Visible = false;
            numericDown.Visible = false;
            numericUp.Visible = false;
        }
        public void UpdateTotal()
        {
            decimal total = 0;
            if (dtgvBillCus.DataSource is DataTable dataTable)
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
            if (billdiscount > 0)
            {
                decimal cost1 = total - (total * billdiscount / 100);
                int cost = (int)cost1;
                txtTotal.Text = $"{total:N0} VND -> {cost:N0} VND";
            }
            else
            {
                txtTotal.Text = total.ToString("N0") + " VND";
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Control control in FlowMenu.Controls)
            {
                if (control is Guna2Panel panel)
                {
                    // Get the name label from the panel
                    Label nameLabel = panel.Controls.OfType<Label>().FirstOrDefault(
                        label => label.Location.Y == 115);  // Using the Y position we set earlier

                    if (nameLabel != null)
                    {
                        string itemName = nameLabel.Text;
                        panel.Visible = itemName.ToLower().Contains(txtSearch.Text.Trim().ToLower());
                    }
                }
            }
        }
        private void nUDItem_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtgvBillCus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            checkdelete = true;
            if (dtgvBillCus.Rows.Count > 0)
            {
                nUDItem.Value = int.Parse(dtgvBillCus.SelectedRows[0].Cells["QTY"].Value.ToString());
                nUDItem.Visible = true;
                numericDown.Visible = true;
                numericUp.Visible = true;
            }
        }
        private void SaveCus()
        {
            string query;
            if (txtPhone.Text == "")
            {
                idCus = 0;
                query = $"UPDATE BILL SET IDCUSTOMER = NULL WHERE ID = {idBill.ToString()};";
                GetDatabase.Instance.ExecuteNonQuery(query);
                return;
            }
            string number = txtPhone.Text;
            query = $"SELECT ID FROM CUSTOMER WHERE PHONE = '{number}';";
            DataTable table = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                idCus = Convert.ToInt32(row["ID"]);
                break;
            }
            if (idCus == 0)
            {
                MessageBox.Show("Chua them khach hang");
                checkclose = false;
            }
            query = $"UPDATE BILL SET IDCUSTOMER = {idCus} WHERE ID = {idBill.ToString()};";
            GetDatabase.Instance.ExecuteNonQuery(query);
        }
        private void AddBill()
        {
            string type;
            string table = idTable.ToString();
            if (idTable > 0) type = "1";
            else
            {
                type = "0";
                table = "NULL";
            }
            string cus;
            if (idCus == 0) cus = "NULL";
            else cus = idCus.ToString();
            string query = $"INSERT INTO BILL (IDTABLE, IDSTAFF, CHKIN_TIME, CHKOUT_TIME, STATUS, BILLTYPE, IDCUSTOMER) VALUES ({table}, '{idStaff}', '{Time}', NULL, 0, {type}, {cus})";
            GetDatabase.Instance.ExecuteNonQuery(query);
            query = "SELECT MAX(ID) AS ID FROM BILL;";
            DataTable result1 = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in result1.Rows)
            {
                idBill = Convert.ToInt32(row["ID"]);
                break;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dtgvBillCus.RowCount == 0)
            {
                MessageBox.Show("You haven't order any yet.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult dialog = MessageBox.Show("Do you really want to Order?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                checkclose = true;
                SaveCus();
                if (checkclose == false) return;
                if (checkbill == false || idTable <= 0)
                    AddBill();
                string filePath = "C:\\Thư mục mới (2)\\KTPOS\\Note\\BillNote";
                filePath = filePath + idBill.ToString() + ".txt";
                if (txtNoteBill.Text != "") using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(txtNoteBill.Text);
                    }
                // Lưu hóa đơn chi tiết
                foreach (DataGridViewRow row in dtgvBillCus.Rows)
                {
                    // Kiểm tra nếu dòng không phải là dòng trống
                    if (row.IsNewRow) continue;
                    int count = Convert.ToInt32(row.Cells["QTY"].Value);
                    int idFD = Convert.ToInt32(row.Cells["ID"].Value);
                    // Chuẩn bị câu lệnh SQL để chèn dữ liệu
                    string queryin = $"INSERT INTO BILLINF (IDBILL, IDFD, COUNT) VALUES ({idBill.ToString()}, {idFD.ToString()}, {count.ToString()})";
                    GetDatabase.Instance.ExecuteNonQuery(queryin);
                }
                
                this.Close(); // Đóng form hiện tại (fStaff_S)
                              // Hiển thị lại form trước đó
                fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
                previousForm?.Show();
                previousForm.LoadTables();
            }
            else
            {
                MessageBox.Show("Order cancelled. Continue your activity.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Focus();
            }
        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            billTable = ((DataTable)dtgvBillCus.DataSource).Copy();
            dtgvBillCus.DataSource = billTable;
        }

        private void Filter_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (Filter.SelectedItem == null) return;
            string filter = Filter.SelectedItem.ToString();
            string query = $"SELECT ITEM.ID AS ID, ITEM.FNAME AS FNAME, ITEM.CATEGORY AS CATEGORY, ITEM.PRICE AS PRICE, COALESCE(PROMOTION.DISCOUNT, 0) AS DISCOUNT FROM ITEM LEFT JOIN ITEM_PROMOTION ON ITEM.ID = ITEM_PROMOTION.IDITEM LEFT JOIN PROMOTION ON ITEM_PROMOTION.IDPROMOTION = PROMOTION.ID AND PROMOTION.[START_DATE] <= CAST(GETDATE() AS DATE) AND PROMOTION.END_DATE >= CAST(GETDATE() AS DATE) AND PROMOTION.STATUS = 1 INNER JOIN ITEM_TAG ON ITEM.ID = ITEM_TAG.IDITEM INNER JOIN TAG ON ITEM_TAG.IDTAG = TAG.ID WHERE ITEM.VISIBLE = 1 AND TAG.TAGNAME = '{filter}' ORDER BY DISCOUNT DESC;";
            LoadMenuItems(query);
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string query = "SELECT ITEM.ID AS ID, ITEM.FNAME AS FNAME, ITEM.CATEGORY AS CATEGORY, ITEM.PRICE AS PRICE, COALESCE(PROMOTION.DISCOUNT, 0) AS DISCOUNT FROM ITEM LEFT JOIN ITEM_PROMOTION ON ITEM.ID = ITEM_PROMOTION.IDITEM LEFT JOIN PROMOTION ON ITEM_PROMOTION.IDPROMOTION = PROMOTION.ID AND PROMOTION.[START_DATE] <= CAST(GETDATE() AS DATE) AND PROMOTION.END_DATE >= CAST(GETDATE() AS DATE) AND PROMOTION.STATUS = 1 WHERE ITEM.VISIBLE = 1 ORDER BY DISCOUNT DESC;";
            LoadMenuItems(query);
        }

        private void btnFood_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string query = "SELECT ITEM.ID AS ID, ITEM.FNAME AS FNAME, ITEM.CATEGORY AS CATEGORY, ITEM.PRICE AS PRICE, COALESCE(PROMOTION.DISCOUNT, 0) AS DISCOUNT FROM ITEM LEFT JOIN ITEM_PROMOTION ON ITEM.ID = ITEM_PROMOTION.IDITEM LEFT JOIN PROMOTION ON ITEM_PROMOTION.IDPROMOTION = PROMOTION.ID AND PROMOTION.[START_DATE] <= CAST(GETDATE() AS DATE) AND PROMOTION.END_DATE >= CAST(GETDATE() AS DATE) AND PROMOTION.STATUS = 1 WHERE ITEM.VISIBLE = 1 AND CATEGORY = 'Food' ORDER BY DISCOUNT DESC;";
            LoadMenuItems(query);
        }

        private void btnDrink_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string query = "SELECT ITEM.ID AS ID, ITEM.FNAME AS FNAME, ITEM.CATEGORY AS CATEGORY, ITEM.PRICE AS PRICE, COALESCE(PROMOTION.DISCOUNT, 0) AS DISCOUNT FROM ITEM LEFT JOIN ITEM_PROMOTION ON ITEM.ID = ITEM_PROMOTION.IDITEM LEFT JOIN PROMOTION ON ITEM_PROMOTION.IDPROMOTION = PROMOTION.ID AND PROMOTION.[START_DATE] <= CAST(GETDATE() AS DATE) AND PROMOTION.END_DATE >= CAST(GETDATE() AS DATE) AND PROMOTION.STATUS = 1 WHERE ITEM.VISIBLE = 1 AND CATEGORY = 'Drink' ORDER BY DISCOUNT DESC;";
            LoadMenuItems(query);
        }

        private void lbCancel_Click_1(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dtgvBillCus.Rows)
            {
                dtgvBillCus.Rows.Remove(row);
            }
        }

        private void txtPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string number = txtPhone.Text;
                fCus newform = new fCus(number);
                newform.ShowDialog();
            }
        }

        private void nUDItem_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void numericUp_Click(object sender, EventArgs e)
        {
            nUDItem.Value++;
            int d = int.Parse(dtgvBillCus.SelectedRows[0].Cells["QTY"].Value.ToString());
            decimal cost = decimal.Parse(dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value.ToString());
            cost /= d;
            dtgvBillCus.SelectedRows[0].Cells["QTY"].Value = nUDItem.Value.ToString();
            dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value = ((int)nUDItem.Value * cost).ToString();
            UpdateTotal();
        }

        private void numericDown_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dtgvBillCus.SelectedRows[0].Cells["ID"].Value);
            int d = (int)nUDItem.Value;
            d--;
            if (d == 0)
            {
                DialogResult dialog = MessageBox.Show("Do you really want to delete?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    dtgvBillCus.Rows.Remove(dtgvBillCus.SelectedRows[0]);
                    return;
                }
                else
                {
                    MessageBox.Show("Exit cancelled. Continue your activity ❤️.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Focus();
                    return;
                }
            }
            nUDItem.Value--;
            d = int.Parse(dtgvBillCus.SelectedRows[0].Cells["QTY"].Value.ToString());
            decimal cost = decimal.Parse(dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value.ToString());
            cost /= d;
            dtgvBillCus.SelectedRows[0].Cells["QTY"].Value = nUDItem.Value.ToString();
            dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value = ((int)nUDItem.Value * cost).ToString();
            UpdateTotal();
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; 
            }
        }

        private void txtPhone_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string number = txtPhone.Text;
                fCus newform = new fCus(number);
                newform.ShowDialog();
                if (newform.check == false) return;
                string query = "SELECT PHONE FROM CUSTOMER WHERE ID = (SELECT MAX(ID) FROM CUSTOMER);";
                DataTable dt = GetDatabase.Instance.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    number = row["PHONE"].ToString();
                }
                txtPhone.Text = number;
            }
        }
    }
}
