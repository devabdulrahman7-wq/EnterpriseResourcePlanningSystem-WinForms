using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sectors_List_A11 : Form
    {
        public Sectors_List_A11()
        {
            InitializeComponent();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Get connection string from configuration
                string connectionString = GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Database connection string is not configured.", "Configuration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create and show the report viewer form
                Reports.Sector_rv reportForm = new Reports.Sector_rv();

                // Get sector data from database
                DataTable sectorData = GetSectorData(connectionString);

                // Pass data to report form
                reportForm.SetReportData(sectorData);

                // Show the report form
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Get connection string from configuration
                string connectionString = GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Database connection string is not configured.", "Configuration Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get sector data
                DataTable sectorData = GetSectorData(connectionString);

                // Create report viewer form
                Reports.Sector_rv reportForm = new Reports.Sector_rv();
                reportForm.SetReportData(sectorData);

                // Print directly without showing the form
                reportForm.PrintReport();

                MessageBox.Show("Report sent to printer successfully.", "Print",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataTable GetSectorData(string connectionString)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT SectorID, SectorName FROM dbo.Sectors ORDER BY SectorID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private string GetConnectionString()
        {
            try
            {
                // Get connection string from app.config
                return ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show($"Configuration error: {ex.Message}", "Configuration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void Sectors_List_A11_Load(object sender, EventArgs e)
        {
            // Optional: Test connection on form load
            try
            {
                string connectionString = GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Please configure the database connection in app.config file.",
                        "Configuration Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
                // Silent fail on load
            }
        }
    }
}