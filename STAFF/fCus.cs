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

namespace KTPOS.STAFF
{
    public partial class fCus : Form
    {
        private string Phone;
        public fCus(string phone)
        {
            InitializeComponent();
            Phone = phone;
            txtPhone.Text = Phone;
            Gender.Items.Add("Male");
            Gender.Items.Add("Female");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string Name = txtName.Text;
            string Home = txtHomeTown.Text;
            string gender = Gender.Text;
            string query = $"INSERT INTO CUSTOMER (FULLNAME, PHONE, GENDER, HOMETOWN) VALUES ('{Name}', '{Phone}', '{gender}', N'{Home}')";
            GetDatabase.Instance.ExecuteNonQuery(query);
            this.Close();
        }
    }
}
