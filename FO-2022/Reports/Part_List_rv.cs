using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class Part_List_rv : Form
    {
        public Part_List_rv()
        {
            InitializeComponent();
        }

        private void Part_List_rv_Load(object sender, EventArgs e)
        {
            try
            {
                // Set report path
                //string reportPath = Path.Combine(Application.StartupPath, "Reports", "Party_List_Report.rdlc");
                string reportPath = Path.Combine(Application.StartupPath, "..", "..", "Reports", "Party_List_rv.rdlc");

                if (File.Exists(reportPath))
                {
                    reportViewer1.LocalReport.ReportPath = reportPath;
                }
                else
                {
                    MessageBox.Show("Report file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load all party data
                LoadAllPartyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllPartyData()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    MessageBox.Show("Connection string not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get ALL parties
                    string query = @"
                    SELECT 
                        PartyID,
                        PartyName,
                        ISNULL(Address, '') AS Address,
                        ISNULL(City, '') AS City,
                        ISNULL(Phone1, '') AS Phone1,
                        ISNULL(Phone2, '') AS Phone2,
                        ISNULL(Mobile, '') AS Mobile
                    FROM Parties 
                    ORDER BY PartyName";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            MessageBox.Show("No parties found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Bind to report
                        reportViewer1.LocalReport.DataSources.Clear();
                        reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataTable));
                        reportViewer1.LocalReport.SetParameters(new ReportParameter[] {
                            //new ReportParameter("ReportDate", DateTime.Now.ToString("dd-MMM-yyyy HH:mm")),
                            //new ReportParameter("TotalParties", dataTable.Rows.Count.ToString())
                        });

                        reportViewer1.RefreshReport();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("SQL Error: " + sqlEx.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrintReport()
        {
            try
            {
                if (reportViewer1.LocalReport.DataSources.Count == 0)
                {
                    LoadAllPartyData();
                }

                if (reportViewer1.LocalReport.DataSources.Count > 0)
                {
                    reportViewer1.PrintDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Print Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
            // Empty
        }
    }
}