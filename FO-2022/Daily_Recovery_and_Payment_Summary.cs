using FO_2022.Reports;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Daily_Recovery_and_Payment_Summary : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Daily_Recovery_and_Payment_Summary()
        {
            InitializeComponent();
            fromdatepcker.Value = DateTime.Today;
            todatepcker.Value = DateTime.Today;
        }

        private void Daily_Recovery_and_Payment_Summary_Load(object sender, EventArgs e)
        {
            LoadSalesmenToListView();
            fromdatepcker.MinDate = new DateTime(2000, 1, 1);
            todatepcker.MaxDate = DateTime.Today;
        }

        private void LoadSalesmenToListView()
        {
            try
            {
                listViewsalesman.Items.Clear();
                listViewsalesman.CheckBoxes = true;
                listViewsalesman.View = View.Details;

                // Add columns
                listViewsalesman.Columns.Clear();
                listViewsalesman.Columns.Add("Salesman Name", 200);
                listViewsalesman.Columns.Add("Salesman ID", 100);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListViewItem item = new ListViewItem(reader["SalesmanName"].ToString());
                                item.SubItems.Add(reader["SalesmanID"].ToString());
                                item.Checked = true;
                                listViewsalesman.Items.Add(item);
                            }
                        }
                    }
                }

                listViewsalesman.Columns[0].Width = -2;
                listViewsalesman.Columns[1].Width = -2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading salesmen: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void slctallbtnfromsectorlistview_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewsalesman.Items)
            {
                item.Checked = true;
            }
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be greater than To date",
                        "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get selected salesmen
                List<string> selectedSalesmen = GetSelectedSalesmen();

                if (selectedSalesmen.Count == 0)
                {
                    MessageBox.Show("Please select at least one salesman",
                        "No Salesman Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data
                DataTable dt = GetDailyRecoveryPaymentData(selectedSalesmen);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No recovery or payment data found for selected criteria",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare parameters
                var parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd-MMM-yyyy") },
                    //{ "TotalSalesmen", selectedSalesmen.Count.ToString() },
                    //{ "ReportDate", DateTime.Today.ToString("dd-MMM-yyyy HH:mm") }
                };

                // Create data source
                var dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", dt }
                };

                // Show report
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Daily_Recovery_and_Payment_Summary", dataSources, parameters);

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetSelectedSalesmen()
        {
            List<string> selectedSalesmen = new List<string>();

            foreach (ListViewItem item in listViewsalesman.Items)
            {
                if (item.Checked)
                {
                    selectedSalesmen.Add(item.SubItems[1].Text);
                }
            }

            return selectedSalesmen;
        }

        private DataTable GetDailyRecoveryPaymentData(List<string> selectedSalesmen)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    sm.SalesmanName,
                    FORMAT(r.RecoveryDate, 'dd-MMM-yyyy') as RecoveryDate,
                    COUNT(rb.RecoveryID) as TransactionCount,
                    SUM(rb.Amount) as TotalRecovery,
                    SUM(rb.Discount) as TotalDiscount,
                    SUM(rb.Amount - rb.Discount) as NetAmount,
                    MAX(r.RecoveryDate) as LastRecoveryDate
                FROM Recovery r
                INNER JOIN RecoveryBody rb ON r.RecoveryID = rb.RecoveryID
                INNER JOIN Salesmen sm ON r.SalesmanID = sm.SalesmanID
                WHERE r.RecoveryDate BETWEEN @FromDate AND @ToDate
                AND r.SalesmanID IN (";

                // Add salesman placeholders
                for (int i = 0; i < selectedSalesmen.Count; i++)
                {
                    query += $"@SalesmanID{i}";
                    if (i < selectedSalesmen.Count - 1) query += ",";
                }
                query += @")
                AND r.IsPosted = 1
                GROUP BY sm.SalesmanName, FORMAT(r.RecoveryDate, 'dd-MMM-yyyy')
                ORDER BY sm.SalesmanName, RecoveryDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value.Date);
                    cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value.Date.AddDays(1).AddSeconds(-1));

                    // Add salesman parameters
                    for (int i = 0; i < selectedSalesmen.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@SalesmanID{i}", selectedSalesmen[i]);
                    }

                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}