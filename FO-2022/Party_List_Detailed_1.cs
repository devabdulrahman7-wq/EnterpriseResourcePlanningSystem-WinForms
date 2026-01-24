using FO_2022.Reports;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_List_Detailed_1 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Party_List_Detailed_1()
        {
            InitializeComponent();
        }

        private void Party_List_Detailed_1_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            // Party Type ComboBox
            partycombobox.Items.Clear();
            partycombobox.Items.Add("ALL PARTIES");
            partycombobox.Items.Add("C");
            partycombobox.Items.Add("V");
            partycombobox.Items.Add("O");
            partycombobox.SelectedIndex = 0;

            // Category ComboBox
            categorycombobox.Items.Clear();
            categorycombobox.Items.Add("All Category");

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT CategoryName FROM Category ORDER BY CategoryName", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        categorycombobox.Items.Add(reader["CategoryName"].ToString());
                    }
                    reader.Close();
                }
                categorycombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        // Search Town by ID
        private void townidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(townidtxtbox.Text))
            {
                MessageBox.Show("Please enter Town ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT TownName FROM Towns WHERE TownID = @TownID", con);
                    cmd.Parameters.AddWithValue("@TownID", townidtxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        townnametxtbox.Text = result.ToString();
                    else
                        townnametxtbox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Search Sector by ID
        private void sectoridsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Sector ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT SectorName FROM Sectors WHERE SectorID = @SectorID", con);
                    cmd.Parameters.AddWithValue("@SectorID", sectoridtxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        sectornametxtbox.Text = result.ToString();
                    else
                        sectornametxtbox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // Get Filtered Data
        private DataTable GetPartyData()
        {
            DataTable dt = new DataTable();

            try
            {
                string query = @"
                    SELECT 
                        p.PartyID,
                        p.PartyName,
                        p.City,
                        p.Mobile,
                        p.PartyType,
                        ISNULL(c.CategoryName, '') AS CategoryName,
                        p.CreditDays,
                        ISNULL(s.SectorName, '') AS SectorName
                    FROM Parties p
                    LEFT JOIN Category c ON p.CategoryId = c.CategoryId
                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                    WHERE 1=1";

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Party Type filter
                if (partycombobox.SelectedIndex > 0 && partycombobox.Text != "ALL PARTIES")
                {
                    query += " AND p.PartyType = @PartyType";
                    parameters.Add(new SqlParameter("@PartyType", partycombobox.Text));
                }

                // Category filter
                if (categorycombobox.SelectedIndex > 0 && categorycombobox.Text != "All Category")
                {
                    query += " AND c.CategoryName = @Category";
                    parameters.Add(new SqlParameter("@Category", categorycombobox.Text));
                }

                // Town filter
                if (!string.IsNullOrWhiteSpace(townidtxtbox.Text))
                {
                    query += " AND s.TownID = @TownID";
                    parameters.Add(new SqlParameter("@TownID", townidtxtbox.Text.Trim()));
                }

                // Sector filter
                if (!string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
                {
                    query += " AND s.SectorID = @SectorID";
                    parameters.Add(new SqlParameter("@SectorID", sectoridtxtbox.Text.Trim()));
                }

                query += " ORDER BY p.PartyName";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);

                    foreach (var param in parameters)
                    {
                        cmd.Parameters.Add(param);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }

            return dt;
        }

        // Preview Button
        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        // Print Button (Same as Preview)
        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        // Show Report
        private void ShowReport()
        {
            try
            {
                DataTable dt = GetPartyData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No records found with selected filters.");
                    return;
                }

                // Prepare data for report
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Prepare parameters (optional)
                //Dictionary<string, string> parameters = new Dictionary<string, string>
                //{
                    //{ "ReportTitle", "Party List Detailed" },
                    //{ "PrintDate", DateTime.Now.ToString("dd-MMM-yyyy") }
                //};

                // Show report using your universal Report_viewer
                Report_viewer viewer = new Report_viewer();
                //viewer.ShowReport("Party_List", dataSources, parameters);
                viewer.ShowReport("Party_List", dataSources);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Report Error: " + ex.Message);
            }
        }

        // Close Button
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Clear town name when ID changes
        private void townidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(townidtxtbox.Text))
                townnametxtbox.Clear();
        }

        // Clear sector name when ID changes
        private void sectoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(sectoridtxtbox.Text))
                sectornametxtbox.Clear();
        }

        // Empty event handlers
        private void partycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void categorycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void townnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectornametxtbox_TextChanged(object sender, EventArgs e) { }
    }
}