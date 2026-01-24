using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FO_2022.Reports;
using System.Configuration;

namespace FO_2022
{
    public partial class Accounts_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime fromDate;
        private DateTime toDate;

        public Accounts_Ledger()
        {
            InitializeComponent();
        }

        // ===================== PARTY SEARCH =====================
        private void searchpartyid_Click(object sender, EventArgs e)
        {
            try
            {
                string partyID = partyidtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(partyID))
                {
                    MessageBox.Show("Please enter Party ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            partynametxtbox.Text = result.ToString();
                        else
                        {
                            MessageBox.Show("Party not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            partynametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching party: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== PREVIEW REPORT =====================
        private void previewbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(partyidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Party ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    partyidtxtbox.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(partynametxtbox.Text))
                {
                    MessageBox.Show("Please search and select a valid party", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable ledgerData = GetLedgerData();
                if (ledgerData == null || ledgerData.Rows.Count == 0)
                {
                    MessageBox.Show("No transactions found for this party in selected date range.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "PartyID", partyidtxtbox.Text },
                    { "PartyName", partynametxtbox.Text }
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
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== GET LEDGER DATA FROM SP =====================
        private DataTable GetLedgerData()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string partyID = partyidtxtbox.Text.Trim();

                    using (SqlCommand cmd = new SqlCommand("SPPartyDetailedLedger", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Pass required parameters
                        cmd.Parameters.AddWithValue("@AccountNo", partyID);

                        // Determine FromDate and ToDate based on radiobuttons
                        if (todaybtn.Checked || daybtn.Checked)
                        {
                            fromDate = DateTime.Today;
                            toDate = DateTime.Today;
                        }
                        else if (datebwbtn.Checked)
                        {
                            // fromDate and toDate already set by popup
                        }
                        else if (alldatebtn.Checked)
                        {
                            fromDate = new DateTime(1900, 1, 1);
                            toDate = DateTime.Today;
                        }
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    AddColumnsToDataTable(dt);
                    ProcessLedgerBalances(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching SP data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return dt;
        }

        // ===================== DATATABLE SETUP =====================
        private void AddColumnsToDataTable(DataTable dt)
        {
            if (!dt.Columns.Contains("Balance")) dt.Columns.Add("Balance", typeof(decimal));
            if (!dt.Columns.Contains("BalanceType")) dt.Columns.Add("BalanceType", typeof(string));
            if (!dt.Columns.Contains("Voucher")) dt.Columns.Add("Voucher", typeof(string));
            if (!dt.Columns.Contains("DisplayDate")) dt.Columns.Add("DisplayDate", typeof(string));
        }

        private void ProcessLedgerBalances(DataTable dt)
        {
            decimal runningBalance = 0;
            string runningType = "Dr";

            // Add Opening Balance Row
            DataRow openingRow = dt.NewRow();
            openingRow["DisplayDate"] = "";
            openingRow["Voucher"] = "OPENING BALANCE";
            openingRow["Narration"] = "Balance Brought Forward";
            openingRow["Debit"] = 0;
            openingRow["Credit"] = 0;
            openingRow["Balance"] = runningBalance;
            openingRow["BalanceType"] = runningType;
            dt.Rows.InsertAt(openingRow, 0);

            for (int i = 1; i < dt.Rows.Count; i++)
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
                dt.Rows[i]["Voucher"] = $"{voucherNo} ({GetVoucherTypeName(voucherType)})";

                // Calculate running balance
                decimal debit = Convert.ToDecimal(dt.Rows[i]["Debit"]);
                decimal credit = Convert.ToDecimal(dt.Rows[i]["Credit"]);

                runningBalance = runningBalance + debit - credit;
                dt.Rows[i]["Balance"] = Math.Abs(runningBalance);
                dt.Rows[i]["BalanceType"] = runningBalance >= 0 ? "Dr" : "Cr";
            }

            // Closing balance
            DataRow closingRow = dt.NewRow();
            closingRow["DisplayDate"] = "";
            closingRow["Voucher"] = "CLOSING BALANCE";
            closingRow["Narration"] = "Balance Carried Forward";
            closingRow["Debit"] = 0;
            closingRow["Credit"] = 0;
            decimal lastBal = Convert.ToDecimal(dt.Rows[dt.Rows.Count - 1]["Balance"]);
            string lastType = dt.Rows[dt.Rows.Count - 1]["BalanceType"].ToString();
            closingRow["Balance"] = lastBal;
            closingRow["BalanceType"] = lastType;
            dt.Rows.Add(closingRow);
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
                case "SI": return "Sales";
                case "RI": return "ReceiptInvoice";
                default: return code;
            }
        }

        // ===================== DATE RANGE POPUP =====================
        private void datebwbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (datebwbtn.Checked)
            {
                using (Form dateForm = new Form())
                {
                    dateForm.Text = "Select Date Range";
                    dateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    dateForm.StartPosition = FormStartPosition.CenterParent;
                    dateForm.Width = 300;
                    dateForm.Height = 200;

                    // Make form top-most
                    dateForm.TopMost = true;

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

        // ===================== RADIO BUTTONS =====================
        private void todaybtn_CheckedChanged(object sender, EventArgs e) { }
        private void daybtn_CheckedChanged(object sender, EventArgs e) { }
        private void alldatebtn_CheckedChanged(object sender, EventArgs e) { }

        // ===================== LANGUAGE =====================
        private void engbtn_CheckedChanged(object sender, EventArgs e) { this.RightToLeft = RightToLeft.No; this.RightToLeftLayout = false; }
        private void urdubtn_CheckedChanged(object sender, EventArgs e) { this.RightToLeft = RightToLeft.Yes; this.RightToLeftLayout = true; }

        // ===================== PAPER SIZE & LEDGER =====================
        private void normalpaperbtn_CheckedChanged(object sender, EventArgs e) { }
        private void smallpaperbtn_CheckedChanged(object sender, EventArgs e) { }
        private void normalledgerbtn_CheckedChanged(object sender, EventArgs e) { }
        private void ledgergrpbtn_CheckedChanged(object sender, EventArgs e) { }

        // ===================== FORM LOAD =====================
        private void Accounts_Ledger_Load(object sender, EventArgs e)
        {
            todaybtn.Checked = true;
            normalledgerbtn.Checked = true;
            normalpaperbtn.Checked = true;
            engbtn.Checked = true;
            partyidtxtbox.Focus();
        }

        // ===================== BUTTONS =====================
        private void closebtn_Click(object sender, EventArgs e) { this.Close(); }
        private void printbtn_Click(object sender, EventArgs e) { previewbtn_Click(sender, e); }

        private void partyidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }

        }

        private void partynametxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }

        }
    }
}
