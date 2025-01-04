using KTPOS.Proccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KTPOS.STAFF
{
    public partial class fCus : Form
    {
        private string Phone;
        public bool check = false;
        public fCus(string phone)
        {
            InitializeComponent();
            Phone = phone;
            txtPhone.Text = Phone;
            Gender.Items.Add("Male");
            Gender.Items.Add("Female");
            setHome();
        }
        private void setHome()
        {
            AutoCompleteStringCollection suggestions = new AutoCompleteStringCollection();
            string query = "SELECT HomeTown FROM CUSTOMER";
            DataTable dataTable = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in dataTable.Rows)
            {
                suggestions.Add(row["HomeTown"].ToString());
            }
            Home.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            Home.AutoCompleteSource = AutoCompleteSource.CustomSource;
            Home.AutoCompleteCustomSource = suggestions;
            Home.Items.AddRange(suggestions.Cast<string>().ToArray());
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string home = Home.Text;
            string gender = Gender.Text;
            string phone = txtPhone.Text;
            if (name == "")
            {
                MessageBox.Show("Please enter Name");
                txtName.Focus();
                return;
            }
            if (phone == "")
            {
                MessageBox.Show("Please enter Phone");
                txtPhone.Focus();
                return;
            }
            if (home == "")
            {
                MessageBox.Show("Please enter Home");
                Home.Focus();
                return;
            }
            if (gender == "")
            {
                MessageBox.Show("Please choose valid Gender");
                Gender.Focus();
                return;
            }
            int idCus = 0;
            string query = $"SELECT ID FROM CUSTOMER WHERE PHONE = '{phone}';";
            DataTable table = GetDatabase.Instance.ExecuteQuery(query);
            foreach (DataRow row in table.Rows)
            {
                idCus = Convert.ToInt32(row["ID"]);
                break;
            }
            if (idCus != 0)
            {
                MessageBox.Show("Phone exist");
                txtPhone.Focus();
                return;
            }
            query = $"INSERT INTO CUSTOMER (FULLNAME, PHONE, GENDER, HOMETOWN) VALUES ('{name}', '{phone}', '{gender}', N'{home}')";
            GetDatabase.Instance.ExecuteNonQuery(query);
            check = true;
            this.Close();
        }
    }
}
