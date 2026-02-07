using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Weekly_Payment_Schedule : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Weekly_Payment_Schedule()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Weekly_Payment_Schedule_Load(object sender, EventArgs e)
        {
            LoadGridData();
        }

        private void LoadGridData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM WeeklyPaymentSchedule";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Set custom column headers
                if (dt.Columns.Count > 0)
                {
                    dt.Columns["SerialNo"].ColumnName = "Serial No";
                    dt.Columns["WeekID"].ColumnName = "Week ID";
                    dt.Columns["WeekStartDate"].ColumnName = "Week Start Date";
                    dt.Columns["WeekEndDate"].ColumnName = "Week End Date";
                    dt.Columns["PartyID"].ColumnName = "Party ID";
                    dt.Columns["PartyName"].ColumnName = "Party Name";
                    dt.Columns["WeeklyPayment"].ColumnName = "Weekly Payment";
                    dt.Columns["CashAllWeek"].ColumnName = "Cash All Week";
                    dt.Columns["MON"].ColumnName = "Monday";
                    dt.Columns["TUE"].ColumnName = "Tuesday";
                    dt.Columns["WED"].ColumnName = "Wednesday";
                    dt.Columns["THR"].ColumnName = "Thursday";
                    dt.Columns["FRI"].ColumnName = "Friday";
                    dt.Columns["SAT"].ColumnName = "Saturday";
                    dt.Columns["SUN"].ColumnName = "Sunday";
                    dt.Columns["NetPaid"].ColumnName = "Net Paid";
                    dt.Columns["RemainingPayment"].ColumnName = "Remaining Payment";
                    dt.Columns["IsPosted"].ColumnName = "Is Posted";
                }

                dataGridView1.DataSource = dt;
            }
        }
    }
}
