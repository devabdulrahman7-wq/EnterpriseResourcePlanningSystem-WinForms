using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_List_Summarize : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Party_List_Summarize()
        {
            InitializeComponent();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            // Print and preview same action; user can print from viewer
            LoadReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadReport()
        {
            try
            {
                // 1. Fetch data from Parties table
                DataTable dt = GetPartyData();
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No party records found.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 2. Prepare data sources for report
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }   // Must match dataset name in RDLC
                };

                // 3. Show report using existing Report_viewer
                Report_viewer viewer = new Report_viewer();
                viewer.ShowReport(
                    reportName: "Party_List_rv.rdlc",
                    dataSources: dataSources
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}\n\n{ex.StackTrace}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetPartyData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to fetch required columns from Parties table
                    string query = @"
                        SELECT 
                            PartyID,
                            Address,
                            City,
                            Phone1,
                            Phone2,
                            Mobile
                        FROM Parties
                        ORDER BY PartyName";  // Optional: order by name for better listing

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return dt;
        }
    }
}