using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
using KTPOS.Proccess;
using KTPOS.STAFF;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static TheArtOfDevHtmlRenderer.Adapters.RGraphicsPath;
using static ZXing.QrCode.Internal.Mode;

namespace KTPOS.MANAGER
{
    public partial class fManager : Form
    {
        int index = -1;
        string query = "", q = "";
        public fManager()
        {
            InitializeComponent();
            dtgvTable.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Load += ManagementControl_Load;
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 450);
        }
        private void ManagementControl_Load(object sender, EventArgs e)
        {
            // Đặt trạng thái mặc định khi UserControl được load
            tcManager.SelectedTab = null;
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
        private void btnMaxSize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            btnMaxSize.Visible = false;
            btnMinSize.Visible = true;
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
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide(); // Đóng form hiện tại (fStaff_S)
            // Nếu form gọi (fStaff_F) vẫn mở, thì có thể chỉ cần gọi lại form đó.
            fStaff_F previousForm = Application.OpenForms["fStaff_F"] as fStaff_F;
            previousForm?.Show();
        }
        //LOAD DATAGRIDVIEW
        private void tcManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcManager.SelectedTab != null)
            {
                string selectedTab = tcManager.SelectedTab.Text;
                switch (selectedTab)
                {
                    case "ACCOUNT":
                        query = "SELECT IDSTAFF AS ID, FULLNAME AS [FULL NAME], DOB AS [D.O.B], EMAIL, PHONE, [ROLE], CASE WHEN STATUS = 1 THEN 'ON' ELSE 'OFF' END AS STATUS FROM ACCOUNT Order by [STATUS] DESC";
                        DTManger.Instance.LoadList(query, dtgvAccount);
                        foreach (DataGridViewRow gridRow in dtgvAccount.Rows)
                        {
                            if (gridRow.Cells["STATUSACC"].Value != null) // Check if the STATUS column has a value
                            {
                                string status = gridRow.Cells["STATUSACC"].Value.ToString();

                                DataGridViewButtonCell statusCell = gridRow.Cells[6] as DataGridViewButtonCell;

                                if (statusCell != null)
                                {
                                    // Set the cell color based on the STATUS value
                                    if (status == "ON")
                                    {
                                        statusCell.Style.BackColor = Color.Green;
                                        statusCell.Style.ForeColor = Color.White;
                                    }
                                    else
                                    {
                                        statusCell.Style.BackColor = Color.Red;
                                        statusCell.Style.ForeColor = Color.White;
                                    }
                                }
                            }
                        }
                        index = -1;
                        break;
                    case "TABLE": 
                        query = @"SELECT 
                            T.FNAME AS[TABLE],
                            T.CAPACITY, 
                            CASE WHEN T.STATUS = 1 THEN 'Available' ELSE 'Unavailable' END AS [STATUS], 
                            COALESCE(SUM(B.TOTAL), 0) AS[TOTAL REVENUE], 
                            COALESCE(SUM(B.DURATION), 0) AS[TOTAL DURATION],
                            CASE WHEN T.VISIBLE = 1 THEN 'ON' ELSE 'OFF' END AS [STATE]
                        FROM
                            [TABLE] T
                        LEFT JOIN
                            BILL B ON T.ID = B.IDTABLE
                        GROUP BY
                           T.FNAME, T.CAPACITY, T.STATUS, T.VISIBLE
                        ORDER BY  T.VISIBLE DESC";
                        DTManger.Instance.LoadList(query, dtgvTable);
                        foreach (DataGridViewRow gridRow in dtgvTable.Rows)
                        {
                            if (gridRow.Cells["STATETABLE"].Value != null) // Check if the STATUS column has a value
                            {
                                string status = gridRow.Cells["STATETABLE"].Value.ToString();

                                DataGridViewButtonCell statusCell = gridRow.Cells["STATETABLE"] as DataGridViewButtonCell;

                                if (statusCell != null)
                                {
                                    // Set the cell color based on the STATUS value
                                    if (status == "ON")
                                    {
                                        statusCell.Style.BackColor = Color.Green;
                                        statusCell.Style.ForeColor = Color.White;
                                    }
                                    else
                                    {
                                        statusCell.Style.BackColor = Color.Red;
                                        statusCell.Style.ForeColor = Color.White;
                                    }
                                }
                            }
                        }
                        index = -1;
                        break;
                    case "BILL":
                        query = @"SELECT 
                           B.ID,
                           B.CHKIN_TIME AS [DATE],
                           C.FULLNAME AS [CUSTOMER],
                           CASE WHEN B.BILLTYPE = 1 THEN 'Dine-In' ELSE 'Take away' END AS [TYPE],
                           (SELECT SUM(bi.COUNT * i.PRICE)
                            FROM BILLINF bi
                            JOIN ITEM i ON bi.IDFD = i.ID 
                            WHERE bi.IDBILL = B.ID) AS TOTAL,
                           (SELECT SUM(bi.COUNT * i.PRICE)
                            FROM BILLINF bi
                            JOIN ITEM i ON bi.IDFD = i.ID 
                            WHERE bi.IDBILL = B.ID) - B.TOTAL AS DISCOUNT,
                           B.TOTAL AS PAYMENT
                        FROM BILL B
                        LEFT JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF 
                        LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID;";
                        DTManger.Instance.LoadList(query, dtgvBill);
                        index = -1;
                        break;
                    case "F&B":
                        cbbTag_SelectedIndexChanged(sender, e);
                        q = "SELECT TAGNAME FROM TAG";
                        GetDatabase.Instance.LoadDataToComboBox(q, cbbTag);
                        q = "SELECT DISTINCT CATEGORY FROM [ITEM]";
                        GetDatabase.Instance.LoadDataToComboBox(q, cbCategoriesFB);
                        index = -1;
                        break;

                    case "REVENUE":
                        query = "";
                        index = -1;
                        break;
                    default:
                        query = "";
                        break;
                }
            }
        }
        //CRUD TC ACCOUNT
        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                index = e.RowIndex;
                // Lấy hàng hiện tại
                DataGridViewRow row = dtgvAccount.Rows[e.RowIndex];
                // Gán dữ liệu từ các cột vào TextBox
                txtFullName.Text = row.Cells[1].Value?.ToString();
                dtpDOB.Text = row.Cells[2].Value?.ToString();
                txtEmail.Text = row.Cells[3].Value?.ToString();
                txtPhone.Text = row.Cells[4].Value?.ToString();
                cbBRole.Text = row.Cells[5].Value?.ToString();
            }
        }
        private void ClearTxtAccount()
        {
            txtSearchAcc.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            dtpDOB.Value = DateTime.Now;
            cbBRole.SelectedIndex = -1;
        }
        private void dtgvAccount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtgvAccount.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DialogResult dialog = MessageBox.Show("Do you really want to layoff/hire this employee?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    if (dtgvAccount.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        // Get the current row and STATUS cell
                        DataGridViewRow selectedRow = dtgvAccount.Rows[e.RowIndex];
                        string currentStatus = selectedRow.Cells["STATUSACC"].Value.ToString();
                        int CurrentIndex = dtgvAccount.CurrentCell.RowIndex;// Adjust "STATUS" to your column name
                        string ID = Convert.ToString(dtgvAccount.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                        // Toggle the status
                        if (currentStatus == "OFF")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATUSACC"].Value = "ON";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            int n = DTManger.Instance.ResolveAccount(1, ID);
                            if (n > 0)
                            {
                                MessageBox.Show("Account set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtAccount();
                            }
                        }
                        else if (currentStatus == "ON")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATUSACC"].Value = "OFF";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            // Update the database
                            int n = DTManger.Instance.ResolveAccount(0, ID);
                            if (n > 0)
                            {
                                MessageBox.Show("Account set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtAccount();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Exit cancelled. Continue your activity ❤️.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Focus();
                }
            }
        }
        private void btnAddAcc_Click(object sender, EventArgs e)
        {
            if (txtFullName.Text.Trim() == "" || txtEmail.Text.Trim() == "" || cbBRole.Text == "" || txtPhone.Text.Trim()=="" || cbBRole.SelectedIndex == -1)
            {
                MessageBox.Show("Error add account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string name = txtFullName.Text, email = txtEmail.Text,phone = txtPhone.Text , role = cbBRole.Text;
                    DateTime bday = dtpDOB.Value;
                    string dob = bday.ToString("yyyy-MM-dd");
                    int n = DTManger.Instance.InsertAccount(name, email, phone, dob, role);
                    if (n > 0)
                    {
                        MessageBox.Show("Account add successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tcManager_SelectedIndexChanged(sender, e);
                        ClearTxtAccount();
                    }
                    else
                    {
                        MessageBox.Show("Error add account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error add account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnUpdateAcc_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Please chose a employee need to be update!", "Notice!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (txtFullName.Text.Trim() == "" || txtEmail.Text.Trim() == "" || cbBRole.Text == "" || txtPhone.Text.Trim() == "")
                {
                    MessageBox.Show("Error update account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        int CurrentIndex = dtgvAccount.CurrentCell.RowIndex;
                        string ID = Convert.ToString(dtgvAccount.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                        string name = txtFullName.Text, email = txtEmail.Text, phone = txtPhone.Text, role = cbBRole.Text;
                        DateTime bday = dtpDOB.Value;
                        string dob = bday.ToString("yyyy-MM-dd"); 
                        int n = DTManger.Instance.UpdateAccount(ID, name, email, phone, dob, role);
                        if (n > 0)
                        {
                            MessageBox.Show("Account update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tcManager_SelectedIndexChanged(sender, e);
                            ClearTxtAccount();
                        }
                        else
                        {
                            MessageBox.Show("Error update account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error update account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void txtSearchAcc_KeyUp(object sender, KeyEventArgs e)
        {
            query = "SELECT IDSTAFF AS ID, FULLNAME AS [FULL NAME], DOB AS [D.O.B], EMAIL, PHONE, [ROLE], CASE WHEN STATUS = 1 THEN 'ON' ELSE 'OFF' END AS STATUS FROM ACCOUNT WHERE FULLNAME Like " + "N'%"+ txtSearchAcc.Text.ToString() +"%' Order by [STATUS] DESC";
            DTManger.Instance.LoadList(query, dtgvAccount);
        }
        //CRUD TABLE
        private void dtgvTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                index = e.RowIndex;
                // Lấy hàng hiện tại
                DataGridViewRow row = dtgvTable.Rows[e.RowIndex];
                // Gán dữ liệu từ các cột vào TextBox
                txtTableName.Text = row.Cells[0].Value?.ToString();
                txtCapacity.Text = row.Cells[1].Value?.ToString();
            }
        }
        private void dtgvTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtgvTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DataGridViewRow selectedRow = dtgvTable.Rows[e.RowIndex];
                string status = selectedRow.Cells[2].Value.ToString();
                if (status == "Available")
                {
                    DialogResult dialog = MessageBox.Show("Do you really want to show/hide this table?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialog == DialogResult.Yes)
                    {
                        if (dtgvTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                        {
                            // Get the current row and STATUS cell
                            string currentStatus = selectedRow.Cells["STATETABLE"].Value.ToString();
                            int CurrentIndex = dtgvTable.CurrentCell.RowIndex;
                            string name = Convert.ToString(dtgvTable.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                                                                                                                    // Toggle the status
                            if (currentStatus == "OFF")
                            {
                                // Update the DataGridView
                                selectedRow.Cells["STATETABLE"].Value = "ON";
                                selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                                selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                                int n = DTManger.Instance.ResolveTable(1, name);
                                if (n > 0)
                                {
                                    MessageBox.Show("TABLE set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    tcManager_SelectedIndexChanged(sender, e);
                                }
                            }
                            else if (currentStatus == "ON")
                            {
                                // Update the DataGridView
                                selectedRow.Cells["STATETABLE"].Value = "OFF";
                                selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                                selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                                // Update the database
                                int n = DTManger.Instance.ResolveTable(0, name);
                                if (n > 0)
                                {
                                    MessageBox.Show("TABLE set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    tcManager_SelectedIndexChanged(sender, e);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Exit cancelled. Continue your activity ❤️.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Can't hide the table is unavailable. Continue your activity next time!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Focus();
                }
            }
        }
        private void ClearTxtTable()
        {
            txtSearchTable.Clear();
            txtTableName.Clear();
            txtCapacity.Clear();
        }
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            if (txtTableName.Text.Trim() == "" || txtCapacity.Text.Trim() == "" )
            {
                MessageBox.Show("Error add table. Please insert information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string name = txtTableName.Text, capacity = txtCapacity.Text;
                   
                    int n = DTManger.Instance.InsertTable(name, capacity);
                    if (n > 0)
                    {
                        MessageBox.Show("Table add successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tcManager_SelectedIndexChanged(sender, e);
                        ClearTxtTable();
                    }
                    else
                    {
                        MessageBox.Show("Error add table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error add table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnEditTable_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Please chose the row need to be edit!", "Notice!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (txtTableName.Text.Trim() == "")
                {
                    MessageBox.Show("Error edit Table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        int CurrentIndex = dtgvTable.CurrentCell.RowIndex;
                        string fname = Convert.ToString(dtgvTable.Rows[CurrentIndex].Cells[0].Value.ToString());
                        string name = txtTableName.Text, capacity = txtCapacity.Text;
                        int n = DTManger.Instance.UpdateTable(name, fname, capacity);
                        if (n > 0)
                        {
                            MessageBox.Show("Table update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tcManager_SelectedIndexChanged(sender, e);
                            ClearTxtTable();
                        }
                        else
                        {
                            MessageBox.Show("Error update Table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error update Table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void txtSearchTable_KeyUp(object sender, KeyEventArgs e)
        {
            query = "SELECT T.FNAME AS[TABLE],T.CAPACITY, CASE WHEN T.STATUS = 1 THEN 'Available' ELSE 'Unavailable' END AS [STATUS], COALESCE(SUM(B.TOTAL), 0) AS[TOTAL REVENUE], COALESCE(SUM(B.DURATION), 0) AS[TOTAL DURATION],CASE WHEN T.VISIBLE = 1 THEN 'ON' ELSE 'OFF' END AS [STATE]"
                +"FROM[TABLE] T LEFT JOIN BILL B ON T.ID = B.IDTABLE WHERE T.FNAME Like N'%"+txtSearchTable.Text.ToString()+"%'"
                +"GROUP BY T.FNAME, T.CAPACITY, T.STATUS, T.VISIBLE; ";
            DTManger.Instance.LoadList(query, dtgvTable);
        }
        //CRUD F&B
        private void ClearTxtFB()
        {
            txtNameFB.Clear();
            cbCategoriesFB.SelectedIndex = -1;
            txtPriceFB.Clear();
        }
        private void cbbTag_SelectedIndexChanged(object sender, EventArgs e)
        { 
            query = "SELECT I.FNAME AS [ITEM NAME], I.CATEGORY,  I.PRICE, ISNULL(SUM(bi.COUNT), 0) AS QTY, MAX(CASE WHEN T.TAGNAME = '" + cbbTag.Text + "' THEN 1 ELSE 0 END) AS TAG," +
                "CASE WHEN I.VISIBLE = 1 THEN 'ON' ELSE 'OFF' END AS [STATE] FROM ITEM I LEFT JOIN BILLINF bi ON i.ID = bi.IDFD LEFT JOIN ITEM_TAG IT ON I.ID = IT.IDITEM " +
                "LEFT JOIN TAG T ON IT.IDTAG = T.ID GROUP BY I.FNAME, I.CATEGORY,I.PRICE,I.VISIBLE ORDER BY I.VISIBLE DESC; ";
            DTManger.Instance.LoadList(query, dtgvFandB);
        }
        private void dtgvFandB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                index = e.RowIndex;
                // Lấy hàng hiện tại
                DataGridViewRow row = dtgvFandB.Rows[e.RowIndex];
                // Gán dữ liệu từ các cột vào TextBox
                txtNameFB.Text = row.Cells[0].Value?.ToString();
                cbCategoriesFB.Text = row.Cells[1].Value?.ToString();
                txtPriceFB.Text = row.Cells[2].Value?.ToString();
            }
        }
        private void dtgvFandB_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtgvFandB.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DialogResult dialog = MessageBox.Show("Do you really want to SHOW/HIDE this item?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    if (dtgvFandB.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        // Get the current row and STATUS cell
                        DataGridViewRow selectedRow = dtgvFandB.Rows[e.RowIndex];
                        string currentStatus = selectedRow.Cells["STATEFB"].Value.ToString();
                        int CurrentIndex = dtgvFandB.CurrentCell.RowIndex;// Adjust "STATUS" to your column name
                        string ID = Convert.ToString(dtgvFandB.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                        // Toggle the status
                        if (currentStatus == "OFF")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATEFB"].Value = "ON";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            int n = DTManger.Instance.ResolveFB(1, ID);
                            if (n > 0)
                            {
                                MessageBox.Show("Item set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtFB();
                            }
                        }
                        else if (currentStatus == "ON")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATEFB"].Value = "OFF";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            // Update the database
                            int n = DTManger.Instance.ResolveFB(0, ID);
                            if (n > 0)
                            {
                                MessageBox.Show("Item set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtFB();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Exit cancelled. Continue your activity ❤️.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Focus();
                }
            }
        }
        private void dtgvFandB_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgvFandB.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                var cellValue = dtgvFandB.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                bool isChecked = cellValue != DBNull.Value && cellValue != null && Convert.ToBoolean(cellValue);

                string fname = txtNameFB.Text;
                string tag = cbbTag.Text;
                int n;

                if (isChecked)
                {
                    n = DTManger.Instance.ResolveTag(1,fname, tag);
                }
                else
                {
                    n = DTManger.Instance.ResolveTag(2, fname, tag);
                }

                if (n > 0)
                {
                    MessageBox.Show("Tag update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbbTag_SelectedIndexChanged(sender, e);
                    ClearTxtFB();
                }
                else
                {
                    MessageBox.Show("Error update Tag", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        private void btnAddFB_Click(object sender, EventArgs e)
        {
            if (txtNameFB.Text.Trim() == "" || txtPriceFB.Text.Trim() == "" || cbCategoriesFB.SelectedIndex == -1)
            {
                MessageBox.Show("Error add Item. Please insert information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string name = txtNameFB.Text, price = txtPriceFB.Text, cate = cbCategoriesFB.Text;

                    int n = DTManger.Instance.InsertItem(name, cate, price);
                    if (n > 0)
                    {
                        MessageBox.Show("Item add successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tcManager_SelectedIndexChanged(sender, e);
                        ClearTxtFB();
                    }
                    else
                    {
                        MessageBox.Show("Error add Item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error add Item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnEditFB_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Please chose the row need to be edit!", "Notice!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (txtNameFB.Text.Trim() == "" || txtPriceFB.Text.Trim() == "" || cbCategoriesFB.SelectedIndex == -1)
                {
                    MessageBox.Show("Error edit Item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        int CurrentIndex = dtgvFandB.CurrentCell.RowIndex;
                        string fname = Convert.ToString(dtgvFandB.Rows[CurrentIndex].Cells[0].Value.ToString());
                        string name = txtNameFB.Text, price = txtPriceFB.Text, cate = cbCategoriesFB.Text; 
                        int n = DTManger.Instance.UpdateItem(name, cate, price, fname);
                        if (n > 0)
                        {
                            MessageBox.Show("Item update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tcManager_SelectedIndexChanged(sender, e);
                            ClearTxtFB();
                        }
                        else
                        {
                            MessageBox.Show("Error update Item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error update Item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtSearchFB_KeyUp(object sender, KeyEventArgs e)
        {
            query = "SELECT I.FNAME AS [ITEM NAME], I.CATEGORY,  I.PRICE, ISNULL(SUM(bi.COUNT), 0) AS QTY, MAX(CASE WHEN T.TAGNAME = '" + cbbTag.Text + "' THEN 1 ELSE 0 END) AS TAG," +
                "CASE WHEN I.VISIBLE = 1 THEN 'ON' ELSE 'OFF' END AS [STATE] FROM ITEM I LEFT JOIN BILLINF bi ON i.ID = bi.IDFD LEFT JOIN ITEM_TAG IT ON I.ID = IT.IDITEM " +
                "LEFT JOIN TAG T ON IT.IDTAG = T.ID WHERE I.FNAME LIKE '%" + txtSearchFB.Text + "%'GROUP BY I.FNAME, I.CATEGORY,I.PRICE,I.VISIBLE ORDER BY I.VISIBLE DESC; ";
            DTManger.Instance.LoadList(query, dtgvFandB);
        }
        //BILL
        
        
        
    }
}
