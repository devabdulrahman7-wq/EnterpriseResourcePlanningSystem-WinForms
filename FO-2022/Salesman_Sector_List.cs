using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using Microsoft.Reporting.WinForms;
namespace FO_2022
{
    public partial class Salesman_Sector_List : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private string selectedSalesmanId = "";

        public Salesman_Sector_List()
        {
            InitializeComponent();
        }

        // 📌 SEARCH BUTTON
        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();
            string salesmanName = salesmannametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanId) && string.IsNullOrEmpty(salesmanName))
            {
                MessageBox.Show("Please enter Salesman ID or Name",
                    "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;

                    if (!string.IsNullOrEmpty(salesmanId))
                    {
                        query = @"SELECT SalesmanID, SalesmanName 
                          FROM Salesmen 
                          WHERE SalesmanID = @Value";
                    }
                    else
                    {
                        query = @"SELECT SalesmanID, SalesmanName 
                          FROM Salesmen 
                          WHERE SalesmanName = @Value";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Value",
                            !string.IsNullOrEmpty(salesmanId) ? salesmanId : salesmanName);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                salesmanidtxtbox.Text = reader["SalesmanID"].ToString();
                                salesmannametxtbox.Text = reader["SalesmanName"].ToString();
                                selectedSalesmanId = reader["SalesmanID"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Salesman not found",
                                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                salesmanidtxtbox.Clear();
                                salesmannametxtbox.Clear();
                                selectedSalesmanId = "";
                                salesmanidtxtbox.Focus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 📌 GET SALESMAN DATA WITH SECTORS
        private DataTable GetSalesmanWithSectorsData(string salesmanID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            sm.SalesmanID,
            sm.SalesmanName,
            ISNULL(sm.Mobile, '') as Mobile,
            ISNULL(sec.SectorName, 'No Sector Assigned') as SectorName,
            CASE 
                WHEN @IncludeParties = 1 THEN ISNULL(p.PartyID, '')
                ELSE ''
            END as PartyID,
            CASE 
                WHEN @IncludeParties = 1 THEN ISNULL(p.PartyName, '')
                ELSE ''
            END as PartyName
        FROM Salesmen sm
        LEFT JOIN SalesmanSectors ss ON sm.SalesmanID = ss.SalesmanID
        LEFT JOIN Sectors sec ON ss.SectorID = sec.SectorID
        LEFT JOIN Parties p ON sec.SectorID = p.SectorID
        WHERE sm.SalesmanID = @SalesmanID
        ORDER BY 
            ISNULL(sec.SectorName, 'No Sector Assigned'),
            CASE WHEN @IncludeParties = 1 THEN ISNULL(p.PartyName, '') ELSE '' END";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SalesmanID", salesmanID);
                cmd.Parameters.AddWithValue("@IncludeParties",
                    includepartiesforthereasoncheckbox.Checked ? 1 : 0);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        // 📌 PREVIEW BUTTON
        private void previewbtn_Click(object sender, EventArgs e)
        {
            string salesmanID = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(salesmanID) || string.IsNullOrEmpty(salesmannametxtbox.Text))
            {
                MessageBox.Show("Please search for a salesman first", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get data
                DataTable dtReport = GetSalesmanWithSectorsData(salesmanID);

                if (dtReport.Rows.Count == 0)
                {
                    DataRow dr = dtReport.NewRow();
                    dr["SalesmanID"] = salesmanID;
                    dr["SalesmanName"] = salesmannametxtbox.Text;
                    dr["Mobile"] = "";
                    dr["SectorName"] = "No Sector Assigned";
                    dr["PartyID"] = includepartiesforthereasoncheckbox.Checked ? "" : "";
                    dr["PartyName"] = includepartiesforthereasoncheckbox.Checked ? "" : "";
                    dtReport.Rows.Add(dr);
                }

                // ✅ Open report form
                Reports.Salesman_Sector_rv reportForm = new Reports.Salesman_Sector_rv();
                reportForm.ReportData = dtReport;
                reportForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        // 📌 CHECKBOX CHANGED
        private void includepartiesforthereasoncheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // No action needed - data loaded at preview time
        }

        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            salesmannametxtbox.Clear();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Salesman_Sector_List_Load(object sender, EventArgs e)
        {
            // Optional: Set focus
            salesmanidtxtbox.Focus();
        }

        private void salesmanidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}