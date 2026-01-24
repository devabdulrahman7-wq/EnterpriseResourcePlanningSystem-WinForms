using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FO_2022.Reports;
using System.Configuration;

namespace FO_2022
{
    public partial class Accounts_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Accounts_Ledger()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        // Party Search - Parties table se
        private void searchpartyid_Click(object sender, EventArgs e)
        {
            try
            {
                string partyID = partyidtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(partyID))
                {
                    MessageBox.Show("Please enter Party ID", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Parties table se search karo
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            partynametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Party not found in database!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            partynametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching party: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Preview Button
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(partyidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Party ID", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    partyidtxtbox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(partynametxtbox.Text))
                {
                    MessageBox.Show("Please search and select a valid party", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable ledgerData = GetLedgerData();

                if (ledgerData == null)
                {
                    return; // Error already shown in GetLedgerData
                }

                // Agar sirf opening balance row hai (transactions nahi)
                if (ledgerData.Rows.Count <= 2) // Opening + Closing rows
                {
                    DialogResult result = MessageBox.Show(
                        "No transactions found for this party.\nDo you want to show empty ledger?",
                        "No Transactions",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                // Prepare report
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "PartyID", partyidtxtbox.Text },
                    { "PartyName", partynametxtbox.Text },
                    //{ "ReportTitle", "Accounts Ledger" },
                    //{ "PrintDate", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") },
                    //{ "DateRange", GetDateRangeText() },
                    //{ "PaperSize", smallpaperbtn.Checked ? "A5" : "A4" },
                    //{ "Language", urdubtn.Checked ? "Urdu" : "English" },
                    //{ "LedgerType", ledgergrpbtn.Checked ? "Grouped" : "Normal" }
                };

                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", ledgerData }
                };

                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Accounts_Ledger.rdlc", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetLedgerData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string partyID = partyidtxtbox.Text;

                    // 1. Parties me verify karo
                    string verifyQuery = "SELECT COUNT(1) FROM Parties WHERE PartyID = @PartyID";
                    using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, conn))
                    {
                        verifyCmd.Parameters.AddWithValue("@PartyID", partyID);
                        int partyExists = Convert.ToInt32(verifyCmd.ExecuteScalar());

                        if (partyExists == 0)
                        {
                            MessageBox.Show("Party not found in database!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                    }

                    // 2. Opening balance - ChartOfAccounts me check karo
                    decimal openingBalance = 0;
                    string openingType = "";

                    string openingQuery = @"
                        SELECT OpeningDebit, OpeningCredit 
                        FROM ChartOfAccounts 
                        WHERE AccountNo = @PartyID";

                    using (SqlCommand cmd = new SqlCommand(openingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal openingDebit = Convert.ToDecimal(reader["OpeningDebit"]);
                                decimal openingCredit = Convert.ToDecimal(reader["OpeningCredit"]);

                                if (openingDebit > openingCredit)
                                {
                                    openingBalance = openingDebit - openingCredit;
                                    openingType = "Dr";
                                }
                                else if (openingCredit > openingDebit)
                                {
                                    openingBalance = openingCredit - openingDebit;
                                    openingType = "Cr";
                                }
                            }
                            // Agar ChartOfAccounts me nahi, to openingBalance = 0 hi rahega
                        }
                    }

                    // 3. AllVouchers se transactions
                    string transactionQuery = @"
                        SELECT 
                            VoucherNo,
                            VoucherType,
                            VoucherDate,
                            Narration,
                            ISNULL(Debit, 0) as Debit,
                            ISNULL(Credit, 0) as Credit
                        FROM AllVouchers 
                        WHERE AccountNo = @PartyID";

                    // Date filter based on radio buttons
                    if (todaybtn.Checked)
                    {
                        transactionQuery += " AND CONVERT(date, VoucherDate) = CONVERT(date, GETDATE())";
                    }
                    else if (daybtn.Checked)
                    {
                        // For "A Day" - show today's
                        transactionQuery += " AND CONVERT(date, VoucherDate) = CONVERT(date, GETDATE())";
                    }
                    else if (datebwbtn.Checked)
                    {
                        // Last 30 days
                        transactionQuery += " AND VoucherDate BETWEEN DATEADD(day, -30, GETDATE()) AND GETDATE()";
                    }
                    // All dates - no filter

                    transactionQuery += " ORDER BY VoucherDate, VoucherNo";

                    using (SqlCommand cmd = new SqlCommand(transactionQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    // 4. Prepare DataTable
                    AddColumnsToDataTable(dt);

                    // 5. Add opening balance row
                    AddOpeningBalanceRow(dt, openingBalance, openingType);

                    // 6. Process transactions and calculate running balance
                    ProcessTransactions(dt, openingBalance, openingType);

                    // 7. Add closing balance
                    AddClosingBalanceRow(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return dt;
        }

        private void AddColumnsToDataTable(DataTable dt)
        {
            if (!dt.Columns.Contains("Balance"))
                dt.Columns.Add("Balance", typeof(decimal));
            if (!dt.Columns.Contains("BalanceType"))
                dt.Columns.Add("BalanceType", typeof(string));
            if (!dt.Columns.Contains("Voucher"))
                dt.Columns.Add("Voucher", typeof(string));
            if (!dt.Columns.Contains("DisplayDate"))
                dt.Columns.Add("DisplayDate", typeof(string));
        }

        private void AddOpeningBalanceRow(DataTable dt, decimal openingBalance, string openingType)
        {
            DataRow openingRow = dt.NewRow();
            openingRow["DisplayDate"] = "";
            openingRow["Voucher"] = "OPENING BALANCE";
            openingRow["Narration"] = "Balance Brought Forward";
            openingRow["Debit"] = openingType == "Dr" ? openingBalance : 0;
            openingRow["Credit"] = openingType == "Cr" ? openingBalance : 0;
            openingRow["Balance"] = openingBalance;
            openingRow["BalanceType"] = openingType;
            //dt.Rows.InsertAt(openingRow, 0);
        }

        private void ProcessTransactions(DataTable dt, decimal openingBalance, string openingType)
        {
            decimal runningBalance = openingBalance;
            string runningType = openingType;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                // Format date
                if (dt.Rows[i]["VoucherDate"] != DBNull.Value)
                {
                    DateTime voucherDate = Convert.ToDateTime(dt.Rows[i]["VoucherDate"]);
                    dt.Rows[i]["DisplayDate"] = voucherDate.ToString("dd-MMM-yyyy");
                }

                // Format voucher
                string voucherNo = dt.Rows[i]["VoucherNo"].ToString();
                string voucherType = dt.Rows[i]["VoucherType"].ToString();
                string voucherTypeName = GetVoucherTypeName(voucherType);
                dt.Rows[i]["Voucher"] = $"{voucherNo} ({voucherTypeName})";

                // Calculate balance
                decimal debit = Convert.ToDecimal(dt.Rows[i]["Debit"]);
                decimal credit = Convert.ToDecimal(dt.Rows[i]["Credit"]);

                if (runningType == "Dr")
                    runningBalance = runningBalance + debit - credit;
                else if (runningType == "Cr")
                    runningBalance = runningBalance + credit - debit;
                else
                {
                    if (debit > 0)
                    {
                        runningBalance = debit - credit;
                        runningType = "Dr";
                    }
                    else if (credit > 0)
                    {
                        runningBalance = credit - debit;
                        runningType = "Cr";
                    }
                }

                // Determine type
                if (runningBalance > 0)
                    runningType = "Dr";
                else if (runningBalance < 0)
                {
                    runningType = "Cr";
                    runningBalance = Math.Abs(runningBalance);
                }
                else
                    runningType = "";

                dt.Rows[i]["Balance"] = runningBalance;
                dt.Rows[i]["BalanceType"] = runningType;
            }
        }

        private void AddClosingBalanceRow(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                decimal lastBalance = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Balance"]);
                string lastType = dt.Rows[dt.Rows.Count - 1]["BalanceType"].ToString();

                DataRow closingRow = dt.NewRow();
                closingRow["DisplayDate"] = "";
                closingRow["Voucher"] = "CLOSING BALANCE";
                closingRow["Narration"] = "Balance Carried Forward";
                closingRow["Debit"] = 0;
                closingRow["Credit"] = 0;
                closingRow["Balance"] = lastBalance;
                closingRow["BalanceType"] = lastType;
                dt.Rows.Add(closingRow);
            }
        }

        private string GetVoucherTypeName(string code)
        {
            switch (code.ToUpper())
            {
                case "JV": return "Journal";
                case "BV": return "Bank";
                case "CV": return "Credit";
                case "DV": return "Debit";
                case "PV": return "Payment";
                case "RV": return "Receipt";
                case "SV": return "Sales";
                case "PUR": return "Purchase";
                default: return code;
            }
        }

        private string GetDateRangeText()
        {
            if (alldatebtn.Checked) return "All Dates";
            else if (datebwbtn.Checked) return "Last 30 Days";
            else if (daybtn.Checked) return "Today";
            else if (todaybtn.Checked) return $"Today: {DateTime.Today:dd-MMM-yyyy}";
            return "";
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e);
        }

        private void Accounts_Ledger_Load(object sender, EventArgs e)
        {
            todaybtn.Checked = true;
            normalledgerbtn.Checked = true;
            normalpaperbtn.Checked = true;
            engbtn.Checked = true;
            partyidtxtbox.Focus();
        }

        private void urdubtn_CheckedChanged(object sender, EventArgs e)
        {
            if (urdubtn.Checked)
            {
                this.RightToLeft = RightToLeft.Yes;
                this.RightToLeftLayout = true;
            }
        }

        private void engbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (engbtn.Checked)
            {
                this.RightToLeft = RightToLeft.No;
                this.RightToLeftLayout = false;
            }
        }

        // Other events
        private void partyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void partynametxtbox_TextChanged(object sender, EventArgs e) { }
        private void normalpaperbtn_CheckedChanged(object sender, EventArgs e) { }
        private void smallpaperbtn_CheckedChanged(object sender, EventArgs e) { }
        private void ledgergrpbtn_CheckedChanged(object sender, EventArgs e) { }
        private void normalledgerbtn_CheckedChanged(object sender, EventArgs e) { }
        private void alldatebtn_CheckedChanged(object sender, EventArgs e) { }
        private void datebwbtn_CheckedChanged(object sender, EventArgs e) { }
        private void daybtn_CheckedChanged(object sender, EventArgs e) { }
        private void todaybtn_CheckedChanged(object sender, EventArgs e) { }
    }
}