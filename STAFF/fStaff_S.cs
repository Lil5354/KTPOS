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
        private Dictionary<string, int> Count = new Dictionary<string, int>();
        private decimal total = 0;
        private DataTable billTable;
        public fStaff_S()
        {
            InitializeComponent();
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
            billTable.Columns.Add("ID", typeof(string));
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
            // Kiểm tra nếu có thể quay lại trang trước đó
            this.Close(); // Đóng form hiện tại (fStaff_S)

            // Hiển thị lại form trước đó, tức là fStaff_F
            // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
            fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
            previousForm?.Show();
        }
        private void LoadMenuItems()
        {
            try
            {
                string query = "SELECT i.ID, i.FNAME, i.PRICE, i.DISCOUNTRATE, fc.FNAME as CATEGORY " +
                              "FROM ITEM i " +
                              "JOIN [F&BCATEGORY] fc ON i.IDCATEGORY = fc.ID " +
                              "WHERE i.VISIBLE = 1";

                DataTable data = GetDatabase.Instance.ExecuteQuery(query);
                FlowMenu.Controls.Clear();

                foreach (DataRow row in data.Rows)
                {
                    int itemId = Convert.ToInt32(row["ID"]);
                    string itemName = row["FNAME"].ToString();
                    double price = Convert.ToDouble(row["PRICE"]);
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
                        string imagePath = $@"../../Images/{itemName.Replace(" ", "")}.png";
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
                    itemPanel.MouseEnter += (sender, e) => {
                        itemPanel.FillColor = Color.FromArgb(240, 240, 240);
                        itemPanel.Cursor = Cursors.Hand;
                    };
                    itemPanel.MouseLeave += (sender, e) => {
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
        public void AddOrUpdate(Dictionary<string, int> dict, string key)
        {
            if (dict.ContainsKey(key))
                dict[key]++;
            else
                dict.Add(key, 1);
        }
        private void ItemPanel_Click(int itemId, string itemName, double price)
        {
            try
            {
                string ID = itemId.ToString();

                // Query to get item details
                string query = "SELECT * FROM ITEM WHERE ID = @ID";
                DataTable result = GetDatabase.Instance.ExecuteQuery(query, new object[] { ID });

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    string name = row["FNAME"].ToString();
                    decimal itemPrice = Convert.ToDecimal(row["PRICE"]);

                    // Update count dictionary
                    AddOrUpdate(Count, name);

                    // Get the current DataTable
                    DataTable currentTable = (DataTable)dtgvBillCus.DataSource;

                    // Find existing row
                    DataRow[] foundRows = currentTable.Select($"NAME = '{name}'");
                    if (foundRows.Length > 0)
                    {
                        // Update existing row
                        foundRows[0]["QTY"] = Count[name];
                        foundRows[0]["PRICE"] = (itemPrice * Count[name]).ToString("N0");
                    }
                    else
                    {
                        // Add new row to DataTable
                        DataRow newRow = currentTable.NewRow();
                        newRow["NAME"] = name;
                        newRow["QTY"] = 1;
                        newRow["PRICE"] = itemPrice.ToString("N0");
                        currentTable.Rows.Add(newRow);
                    }

                    // Refresh the DataGridView
                    dtgvBillCus.Refresh();

                    // Update total
                    UpdateTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            nUDItem.Visible = false;
        }
        private void UpdateTotal()
        {
            total = 0;
            if (dtgvBillCus.DataSource is DataTable dataTable)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["QTY"] != DBNull.Value && row["PRICE"] != DBNull.Value)
                    {
                        int qty = Convert.ToInt32(row["QTY"]);
                        string priceStr = row["PRICE"].ToString().Replace(",", "");
                        if (decimal.TryParse(priceStr, out decimal price))
                        {
                            total += price;
                        }
                    }
                }
            }
            txtTotal.Text = total.ToString("N0") + " VND";
        }
        
        private bool IsNumeric(string value)
        {
            return decimal.TryParse(value.Replace(",", ""), out _);
        }
        private void btnMenu_Click_1(object sender, EventArgs e)
        {
            LoadMenuItems();
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
        
    }
}
