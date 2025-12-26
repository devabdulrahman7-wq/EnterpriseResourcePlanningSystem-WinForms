using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Search_Recovery_Invoice : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public int SelectedRecoveryID { get; private set; }

        public Search_Recovery_Invoice()
        {
            InitializeComponent();
        }

        //private void Search_Recovery_Invoice_Load(object sender, EventArgs e)
        //{
        //   
        //}

        private void LoadAllRecoveries()
        {
            try
            {
                string query = @"
                    SELECT 
                        r.RecoveryID,
                        r.RecoveryDate,
                        ISNULL(sm.SalesmanName, '') as SalesmanName,
                        ISNULL(SUM(rb.Amount), 0) as TotalAmount,
                        ISNULL(SUM(rb.Discount), 0) as TotalDiscount,
                        ISNULL(SUM(rb.Amount - rb.Discount), 0) as NetRecovery
                    FROM Recovery1st r
                    LEFT JOIN RecoveryBody1st rb ON r.RecoveryID = rb.RecoveryID
                    LEFT JOIN (
                        SELECT DISTINCT r2.RecoveryID, s.SalesmanId, sm.SalesmanName
                        FROM Recovery1st r2
                        LEFT JOIN RecoveryBody1st rb2 ON r2.RecoveryID = rb2.RecoveryID
                        LEFT JOIN Sales s ON rb2.SaleID = s.SaleId
                        LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                    ) sm ON r.RecoveryID = sm.RecoveryID
                    WHERE rb.IsDeleted = 0 OR rb.IsDeleted IS NULL
                    GROUP BY r.RecoveryID, r.RecoveryDate, sm.SalesmanName
                    ORDER BY r.RecoveryID DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        // Format columns
                        if (dataGridView1.Columns.Count > 0)
                        {
                            dataGridView1.Columns["RecoveryID"].HeaderText = "R.ID";
                            dataGridView1.Columns["RecoveryDate"].HeaderText = "Date";
                            dataGridView1.Columns["RecoveryDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                            dataGridView1.Columns["SalesmanName"].HeaderText = "Salesman";
                            dataGridView1.Columns["TotalAmount"].HeaderText = "Amount";
                            dataGridView1.Columns["TotalDiscount"].HeaderText = "Discount";
                            dataGridView1.Columns["NetRecovery"].HeaderText = "Net Recovery";

                            dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
                            dataGridView1.Columns["TotalDiscount"].DefaultCellStyle.Format = "N2";
                            dataGridView1.Columns["NetRecovery"].DefaultCellStyle.Format = "N2";

                            // Right align numeric columns
                            dataGridView1.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dataGridView1.Columns["TotalDiscount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dataGridView1.Columns["NetRecovery"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recoveries: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

      

        private void SearchByRecoveryID()
        {
            if (string.IsNullOrWhiteSpace(searchrecoveryidtxtbox.Text))
            {
                LoadAllRecoveries();
                return;
            }

            if (!int.TryParse(searchrecoveryidtxtbox.Text, out int recoveryID))
            {
                MessageBox.Show("Please enter a valid Recovery ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = @"
                    SELECT 
                        r.RecoveryID,
                        r.RecoveryDate,
                        ISNULL(sm.SalesmanName, '') as SalesmanName,
                        ISNULL(SUM(rb.Amount), 0) as TotalAmount,
                        ISNULL(SUM(rb.Discount), 0) as TotalDiscount,
                        ISNULL(SUM(rb.Amount - rb.Discount), 0) as NetRecovery
                    FROM Recovery1st r
                    LEFT JOIN RecoveryBody1st rb ON r.RecoveryID = rb.RecoveryID
                    LEFT JOIN (
                        SELECT DISTINCT r2.RecoveryID, s.SalesmanId, sm.SalesmanName
                        FROM Recovery1st r2
                        LEFT JOIN RecoveryBody1st rb2 ON r2.RecoveryID = rb2.RecoveryID
                        LEFT JOIN Sales s ON rb2.SaleID = s.SaleId
                        LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                    ) sm ON r.RecoveryID = sm.RecoveryID
                    WHERE r.RecoveryID = @RecoveryID
                    AND (rb.IsDeleted = 0 OR rb.IsDeleted IS NULL)
                    GROUP BY r.RecoveryID, r.RecoveryDate, sm.SalesmanName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RecoveryID", recoveryID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show($"No recovery found with ID: {recoveryID}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching recovery: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchByDateRange()
        {
            DateTime fromDate = dateTimePicker1.Value.Date;
            DateTime toDate = dateTimePicker2.Value.Date;

            if (fromDate > toDate)
            {
                MessageBox.Show("From Date cannot be greater than To Date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string query = @"
                    SELECT 
                        r.RecoveryID,
                        r.RecoveryDate,
                        ISNULL(sm.SalesmanName, '') as SalesmanName,
                        ISNULL(SUM(rb.Amount), 0) as TotalAmount,
                        ISNULL(SUM(rb.Discount), 0) as TotalDiscount,
                        ISNULL(SUM(rb.Amount - rb.Discount), 0) as NetRecovery
                    FROM Recovery1st r
                    LEFT JOIN RecoveryBody1st rb ON r.RecoveryID = rb.RecoveryID
                    LEFT JOIN (
                        SELECT DISTINCT r2.RecoveryID, s.SalesmanId, sm.SalesmanName
                        FROM Recovery1st r2
                        LEFT JOIN RecoveryBody1st rb2 ON r2.RecoveryID = rb2.RecoveryID
                        LEFT JOIN Sales s ON rb2.SaleID = s.SaleId
                        LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                    ) sm ON r.RecoveryID = sm.RecoveryID
                    WHERE r.RecoveryDate BETWEEN @FromDate AND @ToDate
                    AND (rb.IsDeleted = 0 OR rb.IsDeleted IS NULL)
                    GROUP BY r.RecoveryID, r.RecoveryDate, sm.SalesmanName
                    ORDER BY r.RecoveryID DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate.AddDays(1).AddSeconds(-1));
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching by date: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void searchrecoveryidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchByRecoveryID();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            SearchByDateRange();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            SearchByDateRange();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            LoadAllRecoveries();
            searchrecoveryidtxtbox.Clear();
        }

        private void cancelbtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
                this.Close();
        }

        private void selectbtn_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                SelectedRecoveryID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["RecoveryID"].Value);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a recovery from the grid", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells["RecoveryID"].Value != null)
                {
                    SelectedRecoveryID = Convert.ToInt32(row.Cells["RecoveryID"].Value);
                }
            }
        }

        private void Search_Recovery_Invoice_Load(object sender, EventArgs e)
        {
            LoadAllRecoveries();
                dateTimePicker1.Value = DateTime.Now.AddDays(-30);
                dateTimePicker2.Value = DateTime.Now;
        }
    }
}