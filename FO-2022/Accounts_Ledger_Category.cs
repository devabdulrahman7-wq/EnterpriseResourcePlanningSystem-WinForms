using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Accounts_Ledger_Category : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Accounts_Ledger_Category()
        {
            InitializeComponent();
        }

        private void Accounts_Ledger_Category_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default dates
                fromdatepcker.Value = DateTime.Today;
                todatepcker.Value = DateTime.Today;

                // Set default radio button
                samedayradiobtn.Checked = true;

                // Load categories
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                categorycombobox.Items.Clear();
                categorycombobox.Items.Add("ALL CATEGORIES"); // Default option

                string query = "SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categorycombobox.Items.Add(new KeyValuePair<int, string>(
                                Convert.ToInt32(reader["CategoryId"]),
                                reader["CategoryName"].ToString()
                            ));
                        }
                    }
                }

                // Display category names
                categorycombobox.DisplayMember = "Value";
                categorycombobox.ValueMember = "Key";
                categorycombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Radio button events
        private void samedayradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (samedayradiobtn.Checked)
            {
                // Set to same date as from date
                todatepcker.Value = fromdatepcker.Value;
                todatepcker.Enabled = false;
            }
        }

        private void dayplusradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (dayousradionbtn.Checked)
            {
                // Set to date = from date + 1 day
                todatepcker.Value = fromdatepcker.Value.AddDays(1);
                todatepcker.Enabled = true;
            }
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Adjust to date based on radio button selection
            if (samedayradiobtn.Checked)
            {
                todatepcker.Value = fromdatepcker.Value;
            }
            else if (dayousradionbtn.Checked)
            {
                todatepcker.Value = fromdatepcker.Value.AddDays(1);
            }
        }

        // Validate input
        private bool ValidateInput()
        {
            // Check date range
            if (todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // Generate Report
        private void GenerateReport()
        {
            try
            {
                // Validate input
                if (!ValidateInput()) return;

                // Get data for report
                DataTable dt = GetCategoryLedgerData(
                    GetSelectedCategoryId(),
                    fromdatepcker.Value,
                    todatepcker.Value
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No ledger data found for the selected category.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "ToDate", todatepcker.Value.ToString("dd/MM/yyyy") },
                    //{ "ReportTitle", "Accounts Ledger by Category" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Add category filter parameter
                if (categorycombobox.SelectedIndex > 0)
                {
                    var selectedItem = (KeyValuePair<int, string>)categorycombobox.SelectedItem;
                    //parameters.Add("CategoryFilter", selectedItem.Value);
                }
                else
                {
                    //parameters.Add("CategoryFilter", "ALL CATEGORIES");
                }

                // Add date range type parameter
                if (samedayradiobtn.Checked)
                {
                    //parameters.Add("DateRangeType", "Same Day");
                }
                else
                {
                    //parameters.Add("DateRangeType", "Next Day");
                }

                // Show report viewer
                Reports.Report_viewer reportViewer = new Reports.Report_viewer();
                reportViewer.ShowReport("Accounts_Ledger_Category", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetSelectedCategoryId()
        {
            if (categorycombobox.SelectedIndex == 0) return 0; // ALL CATEGORIES

            var selectedItem = (KeyValuePair<int, string>)categorycombobox.SelectedItem;
            return selectedItem.Key;
        }

        private DataTable GetCategoryLedgerData(int categoryId, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("AccountNo", typeof(string));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("CategoryName", typeof(string));
            dt.Columns.Add("OpeningBalance", typeof(decimal));
            dt.Columns.Add("OpeningBalanceType", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("ClosingBalance", typeof(decimal));
            dt.Columns.Add("ClosingBalanceType", typeof(string));
            dt.Columns.Add("PartyName", typeof(string));
            dt.Columns.Add("PartyType", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));

            try
            {
                string query = @"
                    -- Get account details and opening balance
                    SELECT 
                        coa.AccountNo,
                        coa.AccountName,
                        cat.CategoryName,
                        ISNULL(ab.OpeningBal, 0) as OpeningBalance,
                        ISNULL(ab.OpeningBalType, 'Dr') as OpeningBalanceType,
                        p.PartyName,
                        p.PartyType,
                        p.Phone1 as Phone,
                        p.Address
                    FROM ChartOfAccounts coa
                    LEFT JOIN Parties p ON coa.AccountNo = p.PartyID
                    LEFT JOIN Category cat ON p.CategoryId = cat.CategoryId
                    LEFT JOIN AccountsBalances ab ON coa.AccountNo = ab.AccountNo
                    WHERE 1=1";

                // Add category filter if not "ALL CATEGORIES"
                if (categoryId > 0)
                {
                    query += " AND cat.CategoryId = @CategoryId";
                }
                else
                {
                    // Only show accounts that have a category (i.e., are parties)
                    query += " AND p.CategoryId IS NOT NULL";
                }

                query += @"
                    ORDER BY cat.CategoryName, coa.AccountName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (categoryId > 0)
                        {
                            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string accountNo = reader["AccountNo"].ToString();

                                // Get transaction totals for the period
                                decimal totalDebit = 0;
                                decimal totalCredit = 0;

                                string transactionQuery = @"
                                    SELECT 
                                        SUM(Debit) as TotalDebit,
                                        SUM(Credit) as TotalCredit
                                    FROM AllVouchers 
                                    WHERE AccountNo = @AccountNo 
                                      AND VoucherDate BETWEEN @FromDate AND @ToDate";

                                using (SqlCommand transCmd = new SqlCommand(transactionQuery, conn))
                                {
                                    transCmd.Parameters.AddWithValue("@AccountNo", accountNo);
                                    transCmd.Parameters.AddWithValue("@FromDate", fromDate);
                                    transCmd.Parameters.AddWithValue("@ToDate", toDate);

                                    using (SqlDataReader transReader = transCmd.ExecuteReader())
                                    {
                                        if (transReader.Read())
                                        {
                                            totalDebit = transReader["TotalDebit"] != DBNull.Value ?
                                                Convert.ToDecimal(transReader["TotalDebit"]) : 0;
                                            totalCredit = transReader["TotalCredit"] != DBNull.Value ?
                                                Convert.ToDecimal(transReader["TotalCredit"]) : 0;
                                        }
                                    }
                                }

                                // Calculate closing balance
                                decimal openingBalance = Convert.ToDecimal(reader["OpeningBalance"]);
                                string openingBalanceType = reader["OpeningBalanceType"].ToString();
                                decimal closingBalance = 0;
                                string closingBalanceType = "Dr";

                                if (openingBalanceType == "Dr")
                                {
                                    closingBalance = openingBalance + totalDebit - totalCredit;
                                    if (closingBalance < 0)
                                    {
                                        closingBalance = Math.Abs(closingBalance);
                                        closingBalanceType = "Cr";
                                    }
                                }
                                else // "Cr"
                                {
                                    closingBalance = openingBalance + totalCredit - totalDebit;
                                    if (closingBalance < 0)
                                    {
                                        closingBalance = Math.Abs(closingBalance);
                                        closingBalanceType = "Dr";
                                    }
                                }

                                DataRow row = dt.NewRow();
                                row["AccountNo"] = reader["AccountNo"];
                                row["AccountName"] = reader["AccountName"];
                                row["CategoryName"] = reader["CategoryName"];
                                row["OpeningBalance"] = openingBalance;
                                row["OpeningBalanceType"] = openingBalanceType;
                                row["Debit"] = totalDebit;
                                row["Credit"] = totalCredit;
                                row["ClosingBalance"] = closingBalance;
                                row["ClosingBalanceType"] = closingBalanceType;
                                row["PartyName"] = reader["PartyName"];
                                row["PartyType"] = reader["PartyType"];
                                row["Phone"] = reader["Phone"];
                                row["Address"] = reader["Address"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching category ledger data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void categorycombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: You can add logic here if needed
        }
    }
}