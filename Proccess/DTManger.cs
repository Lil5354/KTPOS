using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace KTPOS.Proccess
{
    public class DTManger
    {
        private static DTManger instance;
        public static DTManger Instance
        {
            get { if (instance == null) instance = new DTManger(); return instance; }
            private set { instance = value; }
        }
        private DTManger() { }
        private static void AutoBindColumns(DataGridView dgv, DataTable dataTable)
        {
            foreach (DataGridViewColumn dgvColumn in dgv.Columns)
            {
                if (dataTable.Columns.Contains(dgvColumn.HeaderText))
                {
                    dgvColumn.DataPropertyName = dgvColumn.HeaderText;
                }
            }
        }
        public void LoadList(string query, DataGridView dtgv)
        {
            try
            {
                DataTable data = GetDatabase.Instance.ExecuteQuery(query);
                AutoBindColumns(dtgv, data);
                dtgv.DataSource = data;
                dtgv.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public int InsertAccount(string name, string email, string phone, string dob, string role)
        {
            string query = "INSERT INTO ACCOUNT (FULLNAME, EMAIL, PHONE, DOB, [ROLE]) VALUES (N'" + name + "','" + email + "','" + phone + "','" + dob + "','" + role +"' )";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            // Return null if function false
            return 0;
        }
        public int ResolveAccount(int s, string ID)
        {
            string query = "UPDATE ACCOUNT SET STATUS = "+s+" WHERE IDSTAFF = '" + ID + "' ";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            return 0;
        }
        public int UpdateAccount(string ID, string name, string email, string phone, string dob, string role)
        {
            string query = "UPDATE ACCOUNT SET FullName = N'" + name + "', Email = '" + email + "',PHONE = '" + phone + "',DOB = '" + dob + "',[Role] = '" + role + "'WHERE IDSTAFF = N'" + ID + "' ";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            // Return null if function false
            return 0;
        }
        public int ResolveTable(int s, string name) {
            string query = "UPDATE [TABLE] SET VISIBLE = " + s + " WHERE FNAME = '" + name + "' ";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            return 0;
        }
        public int InsertTable(string name, string capacity)
        {
            string query = "INSERT INTO [TABLE] (FNAME, CAPACITY) VALUES ('"+name+"', "+capacity+")";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            // Return null if function false
            return 0;
        }
        public int UpdateTable(string name, string fname, string capacity)
        {
            string query = "UPDATE [TABLE] SET FNAME = N'" + name + "', CAPACITY ="+capacity+" WHERE fname = N'" + fname + "'";
            int result = GetDatabase.Instance.ExecuteNonQuery(query);
            if (result > 0)
            {
                return result;
            }
            // Return null if login fails
            return 0;
        }
    }
}
