using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTPOS.Proccess
{
    public class LgAccount
    {
        private static LgAccount instance;
        public static LgAccount Instance
        {
            get { if (instance == null) instance = new LgAccount(); return instance; }
            private set { instance = value; }
        }
        public string LgManage(string email, string password)
        {
            string query = "SELECT * FROM ACCOUNT WHERE Email = N'" + email + "' AND [Password] =N'" + password + "' ";
            DataTable result = GetDatabase.Instance.ExecuteQuery(query);
            if (result.Rows.Count > 0)
            {
                // Return the role of the logged-in user
                return result.Rows[0]["Role"].ToString();
            }
            // Return null if login fails
            return null;
        }
    }
}
