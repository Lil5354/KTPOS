using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    }
}
