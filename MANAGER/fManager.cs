using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using KTPOS.Proccess;
using KTPOS.STAFF;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.AxHost;
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
                        OR1.Hide();
                        OR2.Hide();
                        OR3.Hide();
                        gBFilter.Hide();
                        index = -1;
                        break;
                    case "PROMOTION":
                        query = "SELECT FNAME AS NAME, DISCOUNT, START_DATE AS [START], END_DATE AS [END], CASE WHEN STATUS = 1 THEN 'ON' ELSE 'OFF' END AS STATUS , APPLY_TO AS [APPLY TO] FROM PROMOTION";
                        DTManger.Instance.LoadList(query,dtgvPMB);
                        q = "SELECT FNAME FROM PROMOTION WHERE APPLY_TO = 'ITEM' AND STATUS = 1";
                        GetDatabase.Instance.LoadDataToComboBox(q, cbbAddItem);
                        foreach (DataGridViewRow gridRow in dtgvPMB.Rows)
                        {
                            if (gridRow.Cells["STATUSPM"].Value != null) // Check if the STATUS column has a value
                            {
                                string status = gridRow.Cells["STATUSPM"].Value.ToString();

                                DataGridViewButtonCell statusCell = gridRow.Cells[4] as DataGridViewButtonCell;

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
                        btnSaveAdd.Hide();
                        dtgvPMI.Hide();

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
        private void btnCompileB_Click(object sender, EventArgs e)
        {
            DateTime S = dtpB1.Value;
            string Start = S.ToString("yyyy-MM-dd");
            DateTime E = dtpB2.Value;
            string End = E.ToString("yyyy-MM-dd");
            query = "SELECT B.CHKIN_TIME AS [DATE], C.FULLNAME AS [CUSTOMER], CASE WHEN B.BILLTYPE = 1 THEN 'Dine-In' ELSE 'Take away' END AS [TYPE], " +
                "(SELECT SUM(bi.COUNT * i.PRICE) FROM BILLINF bi JOIN ITEM i ON bi.IDFD = i.ID WHERE bi.IDBILL = B.ID) AS TOTAL, " +
                "(SELECT SUM(bi.COUNT * i.PRICE) FROM BILLINF bi JOIN ITEM i ON bi.IDFD = i.ID WHERE bi.IDBILL = B.ID) - B.TOTAL AS DISCOUNT, " +
                "B.TOTAL AS PAYMENT FROM BILL B LEFT JOIN ACCOUNT A ON B.IDSTAFF = A.IDSTAFF LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID " +
                "WHERE B.CHKIN_TIME BETWEEN '" + Start + "' AND '"+End+"';";
            DTManger.Instance.LoadList(query, dtgvBill);
            
        }
        //PROMOTION
        private void ClearTxtPromotion()
        {
            txtNameP.Clear();
            txtDiscP.Clear();
            dtpSP.Value = DateTime.Now;
            dtpEP.Value = DateTime.Now;
            cbbApply.SelectedIndex = -1;
        }
        private void dtgvPMB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dtgvPMB.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                DialogResult dialog = MessageBox.Show("Do you really want to hide/show this program?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    if (dtgvPMB.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
                    {
                        // Get the current row and STATUS cell
                        DataGridViewRow selectedRow = dtgvPMB.Rows[e.RowIndex];
                        string currentStatus = selectedRow.Cells["STATUSPM"].Value.ToString();
                        int CurrentIndex = dtgvPMB.CurrentCell.RowIndex;// Adjust "STATUS" to your column name
                        string name = Convert.ToString(dtgvPMB.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                        // Toggle the status
                        if (currentStatus == "OFF")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATUSPM"].Value = "ON";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Green;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            int n = DTManger.Instance.ResolvePromo(1, name);
                            if (n > 0)
                            {
                                MessageBox.Show("Promotion set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtPromotion();
                            }
                        }
                        else if (currentStatus == "ON")
                        {
                            // Update the DataGridView
                            selectedRow.Cells["STATUSPM"].Value = "OFF";
                            selectedRow.Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                            selectedRow.Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                            // Update the database
                            int n = DTManger.Instance.ResolvePromo(0, name);
                            if (n > 0)
                            {
                                MessageBox.Show("Promotion set successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                tcManager_SelectedIndexChanged(sender, e);
                                ClearTxtPromotion();
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
        private void dtgvPMB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
             {
                 index = e.RowIndex;
                 // Lấy hàng hiện tại
                 DataGridViewRow row = dtgvPMB.Rows[e.RowIndex];
                 // Gán dữ liệu từ các cột vào TextBox
                 txtNameP.Text = row.Cells[0].Value?.ToString();
                 txtDiscP.Text = row.Cells[1].Value?.ToString();
                 dtpSP.Text = row.Cells[2].Value?.ToString();
                 dtpEP.Text = row.Cells[3].Value?.ToString();
                 cbbApply.Text = row.Cells[5].Value?.ToString(); 
             }
        }
        private void btnAddPromo_Click(object sender, EventArgs e)
        {
            if (txtNameP.Text.Trim() == "" || txtNameP.Text.Trim() == "" || txtDiscP.Text == "")
            {
                MessageBox.Show("Error add promotion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    string name = txtNameP.Text, disc = txtDiscP.Text, apply = cbbApply.Text;
                    DateTime S= dtpSP.Value;
                    string Start = S.ToString("yyyy-MM-dd");
                    DateTime E = dtpEP.Value;
                    string End = E.ToString("yyyy-MM-dd");
                    int n = DTManger.Instance.InsertPromo(name, disc, Start, End, apply);
                    if (n > 0)
                    {
                        MessageBox.Show("Promotion add successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        tcManager_SelectedIndexChanged(sender, e);
                        ClearTxtAccount();
                    }
                    else
                    {
                        MessageBox.Show("Error add promotion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error add promotion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnEditPromo_Click(object sender, EventArgs e)
        {
            if (index == -1)
            {
                MessageBox.Show("Please chose a employee need to be update!", "Notice!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (txtNameP.Text.Trim() == "" || txtNameP.Text.Trim() == "" || txtDiscP.Text == "")
                {
                    MessageBox.Show("Error update account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        int CurrentIndex = dtgvPMB.CurrentCell.RowIndex;
                        string fname = Convert.ToString(dtgvPMB.Rows[CurrentIndex].Cells[0].Value.ToString()); // Adjust "ID" to your column name
                        string name = txtNameP.Text, disc = txtDiscP.Text, apply = cbbApply.Text;
                        DateTime S = dtpSP.Value;
                        string Start = S.ToString("yyyy-MM-dd");
                        DateTime E = dtpEP.Value;
                        string End = E.ToString("yyyy-MM-dd");
                        int n = DTManger.Instance.UpdatePromo(name, disc, Start, End, apply, fname);
                        if (n > 0)
                        {
                            MessageBox.Show("Promotion update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            tcManager_SelectedIndexChanged(sender, e);
                            ClearTxtAccount();
                        }
                        else
                        {
                            MessageBox.Show("Error update promotion", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error update promotion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void cbbAddItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbbAddItem.SelectedIndex != -1)
            {
                string name = cbbAddItem.Text;
                dtgvPMB.Hide();
                dtgvPMI.Show();                
                query = "SELECT I.FNAME AS [ITEM NAME], MAX(CASE WHEN P.ID IS NOT NULL THEN 1 ELSE 0 END) AS [APPLY TO] FROM ITEM I LEFT JOIN ITEM_PROMOTION IP ON I.ID = IP.IDITEM " +
                    "LEFT JOIN PROMOTION P ON IP.IDPROMOTION = P.ID AND P.FNAME = '"+name+ "' AND P.APPLY_TO = 'ITEM' GROUP BY I.FNAME;";
                DTManger.Instance.LoadList(query, dtgvPMI);
            }
        }
        private void btnSaveAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tag update successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            tcManager_SelectedIndexChanged(sender, e);
            dtgvPMB.Show();
            cbbAddItem.SelectedIndex = -1;
            ClearTxtPromotion();
        }
        private void dtgvPMI_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dtgvPMI.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                var cellValue = dtgvPMI.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                bool isChecked = cellValue != DBNull.Value && cellValue != null && Convert.ToBoolean(cellValue);
                int CurrentIndex = dtgvPMI.CurrentCell.RowIndex;
                string iname = Convert.ToString(dtgvPMI.Rows[CurrentIndex].Cells[0].Value.ToString());
                string promo = cbbAddItem.Text;
                int n;

                if (isChecked)
                {
                    n = DTManger.Instance.ResolvePromoItem(1, iname, promo);
                }
                else
                {
                    n = DTManger.Instance.ResolvePromoItem(2, iname, promo);
                }

                if (n > 0)
                {
                    cbbTag_SelectedIndexChanged(sender, e);
                    btnSaveAdd.Show();
                }
                else
                {
                    MessageBox.Show("Error update Tag", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        //REVENUE CHART
        private void LoadChart()
        {
            if (!rdoMonth.Checked && !rdoYear.Checked)
            {
                MessageBox.Show("Please choose either Month or Year option before proceeding Total Revenue Chart.");
                return;
            }
            else
            {
                DateTime Start = dtpStart.Value;
                string StartD = Start.ToString("yyyy-MM-dd");
                DateTime End = dtpEnd.Value;
                string EndD = End.ToString("yyyy-MM-dd");
                object[] parameters = new object[] { StartD, EndD };
                string queryProductRevenue = @"SELECT
                            I.FNAME AS[ItemName],
                            SUM(BI.COUNT) AS[TotalQuantity]
                        FROM
                            BILLINF BI
                                                JOIN
                            BILL B ON BI.IDBILL = B.ID
                        JOIN
                            ITEM I ON BI.IDFD = I.ID
                        WHERE
                            B.CHKIN_TIME BETWEEN @StartDate AND @EndDate AND B.STATUS = 1-- Only include completed bills
                        GROUP BY
                            I.FNAME
                        ORDER BY
                            [TotalQuantity] DESC";
                string queryServiceRevenue = @"
                        SELECT 
                            CASE WHEN B.[BILLTYPE] = 1 THEN 'Dine-In' ELSE 'Take away' END AS [TYPE],
                            SUM(BI.COUNT * I.PRICE) AS [TotalRevenue]
                        FROM 
                            BILLINF BI
                        JOIN 
                            BILL B ON BI.IDBILL = B.ID
                        JOIN 
                            ITEM I ON BI.IDFD = I.ID
                        WHERE  B.CHKIN_TIME BETWEEN @StartDate AND @EndDate AND  B.STATUS = 1 GROUP BY B.BILLTYPE ORDER BY [TotalRevenue] DESC";
                var productRevenueData = GetDatabase.Instance.GetChartData(queryProductRevenue, parameters);
                var serviceRevenueData = GetDatabase.Instance.GetChartData(queryServiceRevenue, parameters);
                // Product Revenue Chart Setup
                chartProduct.Series.Clear();
                var productSeries = chartProduct.Series.Add("Doanh Thu Mặt Hàng");
                productSeries.ChartType = SeriesChartType.Pie;
                productSeries["PieLabelStyle"] = "Outside";
                productSeries["PieStartAngle"] = "200";
                // Configure simplified percentage labels
                productSeries.Label = "#PERCENT{P2}%";
                productSeries.Font = new Font("Arial", 10, FontStyle.Bold);
                double totalProductRevenue = productRevenueData.Sum(x => x.Value);
                foreach (var dataPoint in productRevenueData)
                {
                    DataPoint point = new DataPoint();
                    point.AxisLabel = dataPoint.Key;
                    point.YValues = new double[] { dataPoint.Value };
                    point.LegendText = $"{dataPoint.Key}: {(dataPoint.Value / totalProductRevenue * 100):F2}%";
                    productSeries.Points.Add(point);
                }
                // Service Revenue Chart Setup
                chartServiceType.Series.Clear();
                var serviceSeries = chartServiceType.Series.Add("Doanh Thu Phục Vụ");
                serviceSeries.ChartType = SeriesChartType.Pie;
                serviceSeries["PieLabelStyle"] = "Outside";
                serviceSeries["PieStartAngle"] = "270";
                // Configure simplified percentage labels
                serviceSeries.Label = "#PERCENT{P2}%";
                serviceSeries.Font = new Font("Arial", 10, FontStyle.Bold);
                double totalServiceRevenue = serviceRevenueData.Sum(x => x.Value);
                foreach (var dataPoint in serviceRevenueData)
                {
                    DataPoint point = new DataPoint();
                    point.AxisLabel = dataPoint.Key;
                    point.YValues = new double[] { dataPoint.Value };
                    point.LegendText = $"{dataPoint.Key}: {(dataPoint.Value / totalServiceRevenue * 100):F2}%";
                    serviceSeries.Points.Add(point);
                }
                // Legend configuration
                chartProduct.Legends[0].Docking = Docking.Right;
                chartProduct.Legends[0].Alignment = StringAlignment.Center;
                chartProduct.Legends[0].Font = new Font("Arial", 10);
                chartServiceType.Legends[0].Docking = Docking.Bottom;
                chartServiceType.Legends[0].Alignment = StringAlignment.Center;
                chartServiceType.Legends[0].Font = new Font("Arial", 10);
                if (rdoMonth.Checked)
                {
                    string queryMonthlyBase = @"
                WITH BillDetails AS (
                    SELECT 
                        FORMAT(B.CHKIN_TIME, 'yyyy-MM') AS [MONTH],
                        SUM(bi.COUNT * i.PRICE) AS TOTAL_SALES,
                        SUM(bi.COUNT * i.PRICE) - B.TOTAL AS DISCOUNT,
                        B.TOTAL AS PAYMENT
                    FROM BILL B
                    JOIN BILLINF bi ON B.ID = bi.IDBILL
                    JOIN ITEM i ON bi.IDFD = i.ID
                    WHERE B.CHKIN_TIME BETWEEN @StartDate AND @EndDate 
                    GROUP BY FORMAT(B.CHKIN_TIME, 'yyyy-MM'), B.TOTAL
                )
                SELECT 
                    [MONTH],
                    SUM(TOTAL_SALES) AS Value
                FROM BillDetails
                GROUP BY [MONTH]
                ORDER BY [MONTH]";

                    // Get monthly data for each metric
                    var monthlySalesData = GetDatabase.Instance.GetChartData(queryMonthlyBase, parameters);
                    var monthlyDiscountData = GetDatabase.Instance.GetChartData(
                        queryMonthlyBase.Replace("SUM(TOTAL_SALES)", "SUM(DISCOUNT)"),
                        parameters
                    );
                    var monthlyRevenueData = GetDatabase.Instance.GetChartData(
                        queryMonthlyBase.Replace("SUM(TOTAL_SALES)", "SUM(PAYMENT)"),
                        parameters
                    );

                    // Setup monthly chart series
                    chartTR.Series.Clear();
                    var monthlySales = chartTR.Series.Add("Total Sales");
                    var monthlyDiscounts = chartTR.Series.Add("Discounts");
                    var monthlyRevenue = chartTR.Series.Add("Final Revenue");

                    // Configure series types
                    foreach (var series in chartTR.Series)
                    {
                        series.ChartType = SeriesChartType.Line;
                        series.MarkerStyle = MarkerStyle.Circle;
                    }

                    // Add monthly data points
                    foreach (var dataPoint in monthlySalesData)
                        monthlySales.Points.AddXY(dataPoint.Key, dataPoint.Value);

                    foreach (var dataPoint in monthlyDiscountData)
                        monthlyDiscounts.Points.AddXY(dataPoint.Key, dataPoint.Value);

                    foreach (var dataPoint in monthlyRevenueData)
                        monthlyRevenue.Points.AddXY(dataPoint.Key, dataPoint.Value);
                }
                else if (rdoYear.Checked)
                {
                    string queryYearlyBase = @"
                    WITH BillDetails AS (
                        SELECT 
                            YEAR(B.CHKIN_TIME) AS [Year],
                            SUM(bi.COUNT * i.PRICE) AS TOTAL_SALES,
                            SUM(bi.COUNT * i.PRICE) - B.TOTAL AS DISCOUNT,
                            B.TOTAL AS PAYMENT
                        FROM BILL B
                        JOIN BILLINF bi ON B.ID = bi.IDBILL
                        JOIN ITEM i ON bi.IDFD = i.ID
                        WHERE 
                            B.CHKIN_TIME BETWEEN @StartDate AND @EndDate 
                        GROUP BY 
                            YEAR(B.CHKIN_TIME), B.TOTAL
                    )
                    SELECT 
                        [Year],
                        SUM(TOTAL_SALES) AS Value
                    FROM BillDetails
                    GROUP BY [Year]
                    ORDER BY [Year]";

                    // Get yearly data for each metric
                    var salesData = GetDatabase.Instance.GetChartData(queryYearlyBase, parameters);
                    var discountData = GetDatabase.Instance.GetChartData(
                        queryYearlyBase.Replace("SUM(TOTAL_SALES)", "SUM(DISCOUNT)"),
                        parameters
                    );
                    var revenueData = GetDatabase.Instance.GetChartData(
                        queryYearlyBase.Replace("SUM(TOTAL_SALES)", "SUM(PAYMENT)"),
                        parameters
                    );

                    // Setup yearly chart series
                    chartTR.Series.Clear();
                    var salesSeries = chartTR.Series.Add("Total Sales");
                    var discountSeries = chartTR.Series.Add("Discounts");
                    var revenueSeries = chartTR.Series.Add("Final Revenue");

                    // Configure series types
                    foreach (var series in chartTR.Series)
                    {
                        series.ChartType = SeriesChartType.Line;
                        series.MarkerStyle = MarkerStyle.Circle;
                    }

                    // Add yearly data points
                    foreach (var dataPoint in salesData)
                        salesSeries.Points.AddXY(dataPoint.Key, dataPoint.Value);

                    foreach (var dataPoint in discountData)
                        discountSeries.Points.AddXY(dataPoint.Key, dataPoint.Value);

                    foreach (var dataPoint in revenueData)
                        revenueSeries.Points.AddXY(dataPoint.Key, dataPoint.Value);
                }
              
            }

        }
        private void LoadChartCus()
        {
            
                DateTime Start = dtpStart.Value;
                string StartD = Start.ToString("yyyy-MM-dd");
                DateTime End = dtpEnd.Value;
                string EndD = End.ToString("yyyy-MM-dd");
                object[] parameters = new object[] { StartD, EndD };

                // Query for Customer vs Walk-in Revenue
                string queryCustomerType = @"
                    SELECT 
                        CASE 
                            WHEN C.ID IS NOT NULL THEN 'Registered Customer'
                            ELSE 'Walk-in Customer'
                        END AS CustomerType,
                        SUM(B.TOTAL) AS TotalRevenue
                    FROM BILL B
                    LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID
                    WHERE B.CHKIN_TIME BETWEEN @StartDate AND @EndDate 
                        AND B.STATUS = 1
                    GROUP BY 
                        CASE 
                            WHEN C.ID IS NOT NULL THEN 'Registered Customer'
                            ELSE 'Walk-in Customer'
                        END
                    ORDER BY TotalRevenue DESC";

                // Query for Gender Distribution
                string queryGenderRevenue = @"
                    SELECT 
                        C.GENDER,
                        SUM(B.TOTAL) AS TotalRevenue
                    FROM BILL B
                    JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID
                    WHERE B.CHKIN_TIME BETWEEN @StartDate AND @EndDate 
                        AND B.STATUS = 1
                    GROUP BY C.GENDER
                    ORDER BY TotalRevenue DESC";

                var customerTypeData = GetDatabase.Instance.GetChartData(queryCustomerType, parameters);
                var genderRevenueData = GetDatabase.Instance.GetChartData(queryGenderRevenue, parameters);

                // Customer Type Chart Setup (chartProduct)
                chartProduct.Series.Clear();
                var customerTypeSeries = chartProduct.Series.Add("Customer Revenue Distribution");
                customerTypeSeries.ChartType = SeriesChartType.Pie;
                customerTypeSeries["PieLabelStyle"] = "Outside";
                customerTypeSeries["PieStartAngle"] = "200";
                customerTypeSeries.Label = "#PERCENT{P2}%";
                customerTypeSeries.Font = new Font("Arial", 10, FontStyle.Bold);

                double totalCustomerTypeRevenue = customerTypeData.Sum(x => x.Value);
                foreach (var dataPoint in customerTypeData)
                {
                    DataPoint point = new DataPoint();
                    point.AxisLabel = dataPoint.Key;
                    point.YValues = new double[] { dataPoint.Value };
                    point.LegendText = $"{dataPoint.Key}: {(dataPoint.Value / totalCustomerTypeRevenue * 100):F2}%";
                    customerTypeSeries.Points.Add(point);
                }

                // Gender Revenue Chart Setup (chartServiceType)
                chartServiceType.Series.Clear();
                var genderSeries = chartServiceType.Series.Add("Gender Revenue Distribution");
                genderSeries.ChartType = SeriesChartType.Pie;
                genderSeries["PieLabelStyle"] = "Outside";
                genderSeries["PieStartAngle"] = "270";
                genderSeries.Label = "#PERCENT{P2}%";
                genderSeries.Font = new Font("Arial", 10, FontStyle.Bold);

                double totalGenderRevenue = genderRevenueData.Sum(x => x.Value);
                foreach (var dataPoint in genderRevenueData)
                {
                    DataPoint point = new DataPoint();
                    point.AxisLabel = dataPoint.Key;
                    point.YValues = new double[] { dataPoint.Value };
                    point.LegendText = $"{dataPoint.Key}: {(dataPoint.Value / totalGenderRevenue * 100):F2}%";
                    genderSeries.Points.Add(point);
                }

                // Configure legends
                chartProduct.Legends[0].Docking = Docking.Right;
                chartProduct.Legends[0].Alignment = StringAlignment.Center;
                chartProduct.Legends[0].Font = new Font("Arial", 10);

                chartServiceType.Legends[0].Docking = Docking.Bottom;
                chartServiceType.Legends[0].Alignment = StringAlignment.Center;
                chartServiceType.Legends[0].Font = new Font("Arial", 10);

                // Hometown Revenue Analysis (Line Chart)
                string queryHometown = @"
                    SELECT 
                        ISNULL(C.HOMETOWN, 'Unknown') AS HOMETOWN,
                        SUM(B.TOTAL) AS TotalRevenue
                    FROM BILL B
                    LEFT JOIN CUSTOMER C ON B.IDCUSTOMER = C.ID
                    WHERE B.CHKIN_TIME BETWEEN @StartDate AND @EndDate 
                        AND B.STATUS = 1
                    GROUP BY C.HOMETOWN
                    ORDER BY TotalRevenue DESC";

                var hometownData = GetDatabase.Instance.GetChartData(queryHometown, parameters);

                // Setup Hometown Revenue Chart
                chartTR.Series.Clear();
                var hometownSeries = chartTR.Series.Add("Hometown Revenue Distribution");
                hometownSeries.ChartType = SeriesChartType.Column;

                // Configure chart appearance
                chartTR.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Rotate labels for better readability
                chartTR.ChartAreas[0].AxisX.Interval = 1; // Show all labels
                chartTR.ChartAreas[0].AxisY.Title = "Revenue";
                chartTR.ChartAreas[0].AxisY.LabelStyle.Format = "N0"; // Format numbers without decimals

                double totalHometownRevenue = hometownData.Sum(x => x.Value);
                foreach (var dataPoint in hometownData)
                {
                    DataPoint point = new DataPoint();
                    point.AxisLabel = dataPoint.Key;
                    point.YValues = new double[] { dataPoint.Value };
                    point.Label = dataPoint.Value.ToString("N0"); // Add value labels on top of columns
                    point.LegendText = $"{dataPoint.Key}: {(dataPoint.Value / totalHometownRevenue * 100):F2}%";
                    hometownSeries.Points.Add(point);
                }

                // Configure column chart specific properties
                hometownSeries.IsValueShownAsLabel = true; // Show values on top of columns
                hometownSeries["PointWidth"] = "0.8"; // Adjust column width

                // Format value labels
                hometownSeries.LabelFormat = "N0";
                hometownSeries.Font = new Font("Arial", 9);

                // Configure legend for hometown chart
                chartTR.Legends[0].Enabled = false;
        }
        private void UpdateTotalLabel(string s)
        {
            // Find the Final Revenue series from the chart
            Series revenueSeries = chartTR.Series.FindByName(s);

            // Calculate total by summing all Y values (revenue points) in the series
            decimal totalRevenue = 0;
            if (revenueSeries != null && revenueSeries.Points.Count > 0)
            {
                totalRevenue = revenueSeries.Points.Sum(point => Convert.ToDecimal(point.YValues[0]));
            }

            // Format the total with currency symbol and thousands separator
            lblTotal.Text = totalRevenue.ToString("#,##0");
        }
        private void btnCompile_Click(object sender, EventArgs e)
        {
            if (cbbFilterRevenue.SelectedIndex != -1)
            {

                string selectedTab = cbbFilterRevenue.Text;
                switch (selectedTab)
                {
                    case "OVERALL REVENUE":
                        ClearAllCharts();
                        LoadChart();
                        UpdateTotalLabel("Final Revenue");
                        break;
                    case "CUSTOMER":
                        ClearAllCharts();
                        LoadChartCus();
                        UpdateTotalLabel("Hometown Revenue Distribution");
                        break;
                    default:
                        break;
                }
            }
        }
        private void ClearAllCharts()
        {
            // Clear primary charts
            if (chartProduct != null && chartProduct.Series.Count > 0)
            {
                chartProduct.Series.Clear();
                chartProduct.Titles.Clear();
                chartProduct.ChartAreas[0].RecalculateAxesScale();
            }

            if (chartServiceType != null && chartServiceType.Series.Count > 0)
            {
                chartServiceType.Series.Clear();
                chartServiceType.Titles.Clear();
                chartServiceType.ChartAreas[0].RecalculateAxesScale();
            }

            if (chartTR != null && chartTR.Series.Count > 0)
            {
                chartTR.Series.Clear();
                chartTR.Titles.Clear();
                chartTR.ChartAreas[0].RecalculateAxesScale();
            }
        }
        private void cbbFilterRevenue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbFilterRevenue.SelectedIndex != -1)
            {
                string selectedTab = cbbFilterRevenue.Text;
                switch (selectedTab)
                {
                    case "OVERALL REVENUE":
                        rdoMonth.Visible = true;
                        rdoYear.Visible = true;
                        rdoMonth.Checked = false;
                        rdoYear.Checked = false;
                        ClearAllCharts();
                        OR1.Show();
                        OR2.Show();
                        OR3.Show();
                        OR1.Text = "Top Selling Intems";
                        OR2.Text = "Revenue by Service Type";
                        OR3.Text = "Total Revenue by Month/ Year";
                        break;
                    case "CUSTOMER":
                        rdoMonth.Visible = false;
                        rdoYear.Visible = false;
                        ClearAllCharts();
                        OR1.Show();
                        OR2.Show();
                        OR3.Show();
                        OR1.Text = "Percentage of membership";
                        OR2.Text = "Percentage of Gender";
                        OR3.Text = "Revenue ratio by customer area";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
