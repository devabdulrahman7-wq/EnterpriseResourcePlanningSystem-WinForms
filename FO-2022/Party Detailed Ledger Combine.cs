using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_Detailed_Ledger_Combine : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime fromDate;
        private DateTime toDate;
       private string selectedPartyId = "";

        public Party_Detailed_Ledger_Combine()
        {
            InitializeComponent();
        }

        private void Party_Detailed_Ledger_Combine_Load(object sender, EventArgs e)
        {
            // Default date selection
            todaybtn.Checked = true;

            // Load Salesman dropdown
            LoadSalesmen();

            // Focus PartyID textbox
            partyidtxtbox.Focus();
        }

        private void LoadSalesmen()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SalesmanID, SalesmanName FROM Salesmen ORDER BY SalesmanName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        salesmancombobox.DisplayMember = "SalesmanName";
                        salesmancombobox.ValueMember = "SalesmanID";
                        salesmancombobox.DataSource = dt;
                        salesmancombobox.SelectedIndex = -1; // no selection initially
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading salesman list: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchpartyid_Click(object sender, EventArgs e)
        {
            string partyID = partyidtxtbox.Text.Trim();
            string partyName = partynametxtbox.Text.Trim();

            if (string.IsNullOrEmpty(partyID) && string.IsNullOrEmpty(partyName))
            {
                MessageBox.Show("Please enter Party ID or Party Name",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                partyidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (!string.IsNullOrEmpty(partyID))
                    {
                        query = "SELECT PartyID, PartyName FROM Parties WHERE PartyID = @Value";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", partyID);
                    }
                    else
                    {
                        query = "SELECT PartyID, PartyName FROM Parties WHERE PartyName = @Value";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Value", partyName);
                    }

                    using (cmd)
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            partyidtxtbox.Text = reader["PartyID"].ToString();
                            partynametxtbox.Text = reader["PartyName"].ToString();
                            selectedPartyId = reader["PartyID"].ToString();
                        }
                        else
                        {
                            partyidtxtbox.Clear();
                            partynametxtbox.Clear();
                            selectedPartyId = "";

                            MessageBox.Show("Party not found!",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            partyidtxtbox.Focus();
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


        private void previewbtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            DataTable ledgerData = GetLedgerData();
            if (ledgerData == null || ledgerData.Rows.Count == 0)
            {
                MessageBox.Show("No transactions found for this party in selected date range.", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var parameters = BuildReportParameters();

            var dataSources = new Dictionary<string, DataTable>
            {
                {"DataSet1", ledgerData}
            };

            Report_viewer reportViewer = new Report_viewer();
            reportViewer.ShowReport("Party_Detailed_Ledger_Combine.rdlc", dataSources, parameters);
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Party ID", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                partyidtxtbox.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(partynametxtbox.Text))
            {
                MessageBox.Show("Please search and select a valid party", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (salesmancombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a salesman", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salesmancombobox.Focus();
                return false;
            }

            // If date between selected but fromDate or toDate not set
            if (datebwbtn.Checked && (fromDate == DateTime.MinValue || toDate == DateTime.MinValue))
            {
                MessageBox.Show("Please select a valid date range", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private DataTable GetLedgerData()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SPPartyDetailedLedger3", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FROMDATE", GetFromDate());
                        cmd.Parameters.AddWithValue("@TODATE", GetToDate());
                        cmd.Parameters.AddWithValue("@accountno", partyidtxtbox.Text.Trim());

                        cmd.CommandTimeout = 55;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                // Agar data mila to balance calculate karo
                if (dt.Rows.Count > 0)
                {
                    dt = PrepareLedgerReportTable(dt);
                }

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching SP data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private DateTime GetFromDate()
        {
            if (alldatebtn.Checked)
                return new DateTime(1900, 1, 1); // very old date for all data
            if (datebwbtn.Checked)
                return fromDate.Date;
            if (daybtn.Checked || todaybtn.Checked)
                return DateTime.Today.Date;
            return DateTime.Today.Date;
        }

        private DateTime GetToDate()
        {
            if (alldatebtn.Checked)
                return DateTime.Today.Date.AddDays(1).AddSeconds(-1); // end of today
            if (datebwbtn.Checked)
                return toDate.Date.AddDays(1).AddSeconds(-1); // end of toDate
            if (daybtn.Checked || todaybtn.Checked)
                return DateTime.Today.Date.AddDays(1).AddSeconds(-1);
            return DateTime.Today.Date.AddDays(1).AddSeconds(-1);
        }

        private string GetDateRangeText()
        {
            if (alldatebtn.Checked) return "All Date";
            else if (datebwbtn.Checked) return $"{fromDate:dd-MMM-yyyy} to {toDate:dd-MMM-yyyy}";
            else if (daybtn.Checked) return "A Day";
            else if (todaybtn.Checked) return "Today";
            return "";
        }

        private Dictionary<string, string> BuildReportParameters()
        {
            return new Dictionary<string, string>
            {
                { "ReportTitle", "PARTY DETAILED LEDGER" },
                { "FromDate", GetFromDate().ToString("dd-MMM-yyyy") },
                { "ToDate", GetToDate().ToString("dd-MMM-yyyy") },
                { "AccountName", partynametxtbox.Text.Trim() },
                { "PrintedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt") },
                { "PrintedBy", string.IsNullOrWhiteSpace(Environment.UserName) ? "SYSTEM" : Environment.UserName }
            };
        }

        private DataTable PrepareLedgerReportTable(DataTable sourceTable)
        {
            DataTable table = sourceTable.Clone();

            EnsureReportColumns(table);

            var sortedRows = sourceTable.AsEnumerable()
                .OrderBy(r => r.Field<DateTime?>("VoucherDate") ?? DateTime.MinValue)
                .ThenBy(r => Convert.ToString(r["VoucherNo"], CultureInfo.InvariantCulture));

            foreach (DataRow row in sortedRows)
            {
                table.ImportRow(row);
            }

            decimal totalDebit = 0m;
            decimal totalCredit = 0m;
            decimal runningBalance = 0m;

            AddSpecialRow(table, "OPENING", "Opening Balance", string.Empty, string.Empty, string.Empty, 0m, 0m, runningBalance);

            foreach (DataRow row in table.Select("", "VoucherDate ASC"))
            {
                if (Convert.ToString(row["RowType"]) == "OPENING")
                {
                    continue;
                }

                decimal debit = GetDecimal(row, "Debit");
                decimal credit = GetDecimal(row, "Credit");

                totalDebit += debit;
                totalCredit += credit;
                runningBalance += debit - credit;

                row["RowType"] = "DETAIL";
                row["DisplayDate"] = row["VoucherDate"] == DBNull.Value
                    ? string.Empty
                    : Convert.ToDateTime(row["VoucherDate"]).ToString("dd-MMM-yyyy");
                row["DisplayVoucherNo"] = Convert.ToString(row["VoucherNo"]);
                row["DisplayReference"] = Convert.ToString(row["Refrence"]);
                row["DisplayDocProof"] = FirstNonEmpty(Convert.ToString(row["BilltyNo"]), Convert.ToString(row["Shipment"]));
                row["DisplayDescription"] = NormalizeDescription(Convert.ToString(row["Narration"]));
                row["DebitText"] = debit == 0m ? string.Empty : debit.ToString("#,##0.00");
                row["CreditText"] = credit == 0m ? string.Empty : credit.ToString("#,##0.00");
                row["Balance"] = runningBalance;
                row["BalanceText"] = FormatBalance(runningBalance);
            }

            AddSpecialRow(table, "TOTAL", "Transaction Totals", string.Empty, string.Empty, string.Empty, totalDebit, totalCredit, runningBalance);
            AddSpecialRow(table, "CLOSING", "Closing Balance", string.Empty, string.Empty, string.Empty, 0m, 0m, runningBalance);

            return table;
        }

        private void EnsureReportColumns(DataTable table)
        {
            AddColumnIfMissing(table, "RowType", typeof(string));
            AddColumnIfMissing(table, "DisplayDate", typeof(string));
            AddColumnIfMissing(table, "DisplayVoucherNo", typeof(string));
            AddColumnIfMissing(table, "DisplayReference", typeof(string));
            AddColumnIfMissing(table, "DisplayDocProof", typeof(string));
            AddColumnIfMissing(table, "DisplayDescription", typeof(string));
            AddColumnIfMissing(table, "DebitText", typeof(string));
            AddColumnIfMissing(table, "CreditText", typeof(string));
            AddColumnIfMissing(table, "Balance", typeof(decimal));
            AddColumnIfMissing(table, "BalanceText", typeof(string));
        }

        private void AddColumnIfMissing(DataTable table, string columnName, Type type)
        {
            if (!table.Columns.Contains(columnName))
            {
                table.Columns.Add(columnName, type);
            }
        }

        private void AddSpecialRow(
            DataTable table,
            string rowType,
            string description,
            string voucherNo,
            string reference,
            string docProof,
            decimal debit,
            decimal credit,
            decimal runningBalance)
        {
            DataRow row = table.NewRow();
            row["RowType"] = rowType;
            row["DisplayDate"] = string.Empty;
            row["DisplayVoucherNo"] = voucherNo;
            row["DisplayReference"] = reference;
            row["DisplayDocProof"] = docProof;
            row["DisplayDescription"] = description;
            row["Debit"] = debit;
            row["Credit"] = credit;
            row["DebitText"] = debit == 0m ? string.Empty : debit.ToString("#,##0.00");
            row["CreditText"] = credit == 0m ? string.Empty : credit.ToString("#,##0.00");
            row["Balance"] = runningBalance;
            row["BalanceText"] = FormatBalance(runningBalance);
            table.Rows.Add(row);
        }

        private decimal GetDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return 0m;
            }

            decimal value;
            return decimal.TryParse(Convert.ToString(row[columnName]), out value) ? value : 0m;
        }

        private string NormalizeDescription(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
        }

        private string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        private string FormatBalance(decimal amount)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:#,##0.00} {1}", Math.Abs(amount), amount >= 0 ? "Dr" : "Cr");
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            previewbtn_Click(sender, e); // same as preview for now
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Date Radio buttons handling with Date Between popup
        private void datebwbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (datebwbtn.Checked)
            {
                using (Form dateForm = new Form())
                {
                    dateForm.Text = "Select Date Range";
                    dateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dateForm.StartPosition = FormStartPosition.CenterParent;
                    dateForm.TopMost = true;
                    dateForm.Width = 300;
                    dateForm.Height = 200;

                    DateTimePicker dtpFrom = new DateTimePicker { Location = new Point(50, 20), Format = DateTimePickerFormat.Short };
                    DateTimePicker dtpTo = new DateTimePicker { Location = new Point(50, 60), Format = DateTimePickerFormat.Short };
                    Button okBtn = new Button { Text = "OK", Location = new Point(50, 100), DialogResult = DialogResult.OK };
                    Button cancelBtn = new Button { Text = "Cancel", Location = new Point(150, 100), DialogResult = DialogResult.Cancel };

                    dateForm.Controls.Add(dtpFrom);
                    dateForm.Controls.Add(dtpTo);
                    dateForm.Controls.Add(okBtn);
                    dateForm.Controls.Add(cancelBtn);

                    dateForm.AcceptButton = okBtn;
                    dateForm.CancelButton = cancelBtn;

                    if (dateForm.ShowDialog() == DialogResult.OK)
                    {
                        fromDate = dtpFrom.Value.Date;
                        toDate = dtpTo.Value.Date;
                    }
                    else
                    {
                        alldatebtn.Checked = true;
                    }
                }
            }
        }

        private void alldatebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (alldatebtn.Checked)
            {
                fromDate = new DateTime(1900, 1, 1);
                toDate = DateTime.Today.Date.AddDays(1).AddSeconds(-1);
            }
        }

        private void daybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (daybtn.Checked)
            {
                fromDate = DateTime.Today.Date;
                toDate = DateTime.Today.Date.AddDays(1).AddSeconds(-1);
            }
        }

        private void todaybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (todaybtn.Checked)
            {
                fromDate = DateTime.Today.Date;
                toDate = DateTime.Today.Date.AddDays(1).AddSeconds(-1);
            }
        }

        // Optional empty handlers for other UI events - can be left empty or implemented later
        private void partyidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private void partynametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private void salesmancombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void detaillgrasdbtn_CheckedChanged(object sender, EventArgs e) { }
        private void detailbtn_CheckedChanged(object sender, EventArgs e) { }
        private void landscapebtn_CheckedChanged(object sender, EventArgs e) { }
        private void portrtbtn_CheckedChanged(object sender, EventArgs e) { }
        private void partyidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void partynametxtbox_TextChanged(object sender, EventArgs e) { }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
