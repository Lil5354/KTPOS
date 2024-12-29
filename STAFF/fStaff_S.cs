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
using System.Xml.Linq;
using Guna.UI2.WinForms;
using KTPOS.Proccess;

namespace KTPOS.STAFF
{
    public partial class fStaff_S : Form
    {
        private int idBill = 0;
        private DataTable billTable;
        public fStaff_S(int tableId)
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
            idBill = tableId;
            if (dtgvBillCus.DataSource == null)
            {
                InitializeBillTable();
            }
            else
            {
                // Get the existing DataTable from the DataGridView
                billTable = ((DataTable)dtgvBillCus.DataSource).Copy();
                dtgvBillCus.DataSource = billTable;
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
            this.Close(); // Đóng form hiện tại (fStaff_S)

            // Hiển thị lại form trước đó, tức là fStaff_F
            // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
            fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
            previousForm?.Show();
        }
        private void LoadFilter()
        {
            string query = "SELECT DISTINCT SALEGROUP FROM ITEM;";
            DataTable data = GetDatabase.Instance.ExecuteQuery(query);
            Filter.Controls.Clear();
            foreach (DataRow row in data.Rows)
                Filter.Items.Add(row["SALEGROUP"]);
        }
        private void LoadMenuItems(string query)
        {
            try
            {
                DataTable data = GetDatabase.Instance.ExecuteQuery(query);
                FlowMenu.Controls.Clear();
                foreach (DataRow row in data.Rows)
                {
                    int itemId = Convert.ToInt32(row["ID"]);
                    string itemName = row["FNAME"].ToString();
                    decimal price = Convert.ToDecimal(row["PRICE"]);
                    string category = row["CATEGORY"].ToString();

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
                        string imagePath = $@"E:\App\Menu\Image Items\" + itemName + ".jpg";
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
                        Text = string.Format("{0:N0} VND", price),
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
                    decimal cost = (decimal) d * price;
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
        }
        private void ItemImage_Click(int itemId, string itemName, decimal price)
        {
            ItemPanel_Click(itemId, itemName, price);
            nUDItem.Visible = false;
        }
        private void UpdateTotal()
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
            txtTotal.Text = total.ToString("N0") + " VND";
        }
        private void btnMenu_Click_1(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string query = "SELECT i.ID, i.FNAME, i.PRICE, i.DISCOUNTRATE, fc.FNAME as CATEGORY " +
                  "FROM ITEM i " +
                  "JOIN [F&BCATEGORY] fc ON i.IDCATEGORY = fc.ID " +
                  "WHERE i.VISIBLE = 1";
            LoadMenuItems(query);
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

        private void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            string filter = Filter.SelectedItem.ToString();
            string query = "SELECT i.ID, i.FNAME, i.PRICE, i.DISCOUNTRATE, fc.FNAME as CATEGORY " +
                  "FROM ITEM i " +
                  "JOIN [F&BCATEGORY] fc ON i.IDCATEGORY = fc.ID " +
                  "WHERE i.SALEGROUP = '" + filter + "'";
            LoadMenuItems(query);
        }
        private void nUDItem_ValueChanged(object sender, EventArgs e)
        {
            int d = int.Parse(dtgvBillCus.SelectedRows[0].Cells["QTY"].Value.ToString());
            decimal cost = decimal.Parse(dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value.ToString());
            cost /= d;
            dtgvBillCus.SelectedRows[0].Cells["QTY"].Value = nUDItem.Value.ToString();
            dtgvBillCus.SelectedRows[0].Cells["PRICE"].Value = ((int)nUDItem.Value * cost).ToString();
            UpdateTotal();
            if (nUDItem.Value == 0)
            {
                dtgvBillCus.Rows.Remove(dtgvBillCus.SelectedRows[0]);
                nUDItem.Visible = false;
            }
        }

        private void dtgvBillCus_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgvBillCus.Rows.Count > 0)
            {
                nUDItem.Value = int.Parse(dtgvBillCus.SelectedRows[0].Cells["QTY"].Value.ToString());
                nUDItem.Visible = true;
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
                string filePath = "E:\\App\\KTPOS\\Note\\BillNote";
                string query = "DELETE from BILLINF where IDBILL = " + idBill.ToString();
                GetDatabase.Instance.ExecuteNonQuery(query);
                filePath = filePath + idBill.ToString() + ".txt";
                if (txtNoteBill != null) using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.Write(txtNoteBill.Text);
                    }

                foreach (DataGridViewRow row in dtgvBillCus.Rows)
                {
                    // Kiểm tra nếu dòng không phải là dòng trống
                    if (row.IsNewRow) continue;
                    int count = Convert.ToInt32(row.Cells["QTY"].Value);
                    int idFD = Convert.ToInt32(row.Cells["ID"].Value);

                    // Chuẩn bị câu lệnh SQL để chèn dữ liệu
                    string queryin = "INSERT INTO BILLINF (IDBILL, IDFD, COUNT) VALUES (" + idBill.ToString() + "," + idFD.ToString() + "," + count.ToString() + ")";

                    DataTable result1 = GetDatabase.Instance.ExecuteQuery(queryin);
                }
                this.Close(); // Đóng form hiện tại (fStaff_S)

                // Hiển thị lại form trước đó, tức là fStaff_F
                // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
                fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
                previousForm?.Show();
            }
            else
            {
                MessageBox.Show("Order cancelled. Continue your activity.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Focus();
            }
        }

        private void lbCancel_Click(object sender, EventArgs e)
        {
            btnBack_Click(sender, e);
        }
    }
}
