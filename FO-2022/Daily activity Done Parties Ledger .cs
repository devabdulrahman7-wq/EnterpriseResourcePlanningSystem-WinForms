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
using System.Configuration;
using FO_2022.Reports;

namespace FO_2022
{
    public partial class Daily_activity_Done_Parties_Ledger : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Daily_activity_Done_Parties_Ledger()
        {
            InitializeComponent();
        }

        // ========== FORM EVENTS ==========

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void Daily_activity_Done_Parties_Ledger_Load(object sender, EventArgs e)
        {
            // Set default date to today
            todaydatetimepicker.Value = DateTime.Today;

            // Set radio button default
            todayradiobtn.Checked = true;
            datebwradiobtn.Checked = false;
            todaydatetimepicker.Enabled = true;

            // Disable buttons initially
            previewbtn.Enabled = false;
            printbtn.Enabled = false;

            // Set focus to party ID textbox
            partyidtxtbox.Focus();
        }

        // ========== RADIO BUTTON EVENTS ==========

        private void todayradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            todaydatetimepicker.Enabled = true;
        }

        private void datebwradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            todaydatetimepicker.Enabled = false;
        }

        // ========== SEARCH BUTTON EVENT ==========

        private void searchpartyid_Click(object sender, EventArgs e)
        {
            SearchParty();
        }

        // ========== PREVIEW & PRINT BUTTONS ==========

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport(false);  // false = preview mode
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport(true);   // true = print mode
        }

        // ========== TEXTBOX EVENTS ==========

        private void partyidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SearchParty();
                e.Handled = true;
            }
        }

        private void partyidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear party name and disable buttons when ID changes
            partynametxtbox.Text = "";
            previewbtn.Enabled = false;
            printbtn.Enabled = false;
        }

        // ========== MAIN METHODS ==========

        private void SearchParty()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Party ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                partyidtxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyidtxtbox.Text.Trim());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                partynametxtbox.Text = reader["PartyName"].ToString();
                                previewbtn.Enabled = true;
                                printbtn.Enabled = true;
                                previewbtn.Focus();
                            }
                            else
                            {
                                MessageBox.Show("Party ID not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                partynametxtbox.Text = "";
                                previewbtn.Enabled = false;
                                printbtn.Enabled = false;
                                partyidtxtbox.Focus();
                                partyidtxtbox.SelectAll();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowReport(bool printMode)
        {
            // Step 1: Validate inputs
            if (!ValidateInputs())
                return;

            // Step 2: Get date range
            if (!GetDateRange(out DateTime fromDate, out DateTime toDate))
                return;

            // Step 3: Get report data
            DataTable reportData = GetLedgerData(partyidtxtbox.Text.Trim(), fromDate, toDate);
            if (reportData == null || reportData.Rows.Count == 0)
            {
                MessageBox.Show("No ledger data found", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Step 4: Get report parameters
            Dictionary<string, string> parameters = GetReportParameters(
                partyidtxtbox.Text.Trim(),
                partynametxtbox.Text,
                fromDate,
                toDate);

            // Step 5: Show report
            ShowReportToUser(reportData, parameters, printMode);
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Party ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                partyidtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(partynametxtbox.Text))
            {
                MessageBox.Show("Please search for party first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SearchParty();
                return false;
            }

            return true;
        }

        private bool GetDateRange(out DateTime fromDate, out DateTime toDate)
        {
            fromDate = DateTime.MinValue;
            toDate = DateTime.MinValue;

            try
            {
                if (todayradiobtn.Checked)
                {
                    // For "Today" option
                    fromDate = todaydatetimepicker.Value.Date;
                    toDate = todaydatetimepicker.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                    return true;
                }
                else if (datebwradiobtn.Checked)
                {
                    // For "Date Between" option - show popup
                    using (DateRangePopup popup = new DateRangePopup())
                    {
                        if (popup.ShowDialog() == DialogResult.OK)
                        {
                            fromDate = popup.FromDate;
                            toDate = popup.ToDate.AddDays(1).AddSeconds(-1); // End of day
                            return true;
                        }
                        else
                        {
                            return false; // User cancelled
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select date option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting date range: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private DataTable GetLedgerData(string partyID, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Try to use stored procedure if it exists
                    using (SqlCommand cmd = new SqlCommand("SPPartyDetailedLedger3", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FROMDATE", fromDate);
                        cmd.Parameters.AddWithValue("@TODATE", toDate);
                        cmd.Parameters.AddWithValue("@accountno", partyID);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // If stored procedure didn't return data, try direct query
                if (dt.Rows.Count == 0)
                {
                    dt = GetLedgerDataDirect(partyID, fromDate, toDate);
                }

                // Add running balance calculation
                if (dt.Columns.Contains("Debit") && dt.Columns.Contains("Credit"))
                {
                    dt.Columns.Add("RunningBalance", typeof(decimal));
                    decimal runningBalance = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
                        decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;
                        runningBalance = runningBalance + debit - credit;
                        row["RunningBalance"] = runningBalance;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // If stored procedure not found, use direct method
                if (sqlEx.Message.Contains("Could not find stored procedure"))
                {
                    dt = GetLedgerDataDirect(partyID, fromDate, toDate);
                }
                else
                {
                    MessageBox.Show($"Database Error: {sqlEx.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private DataTable GetLedgerDataDirect(string partyID, DateTime fromDate, DateTime toDate)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        -- Get transactions from Journal Vouchers
                        SELECT 
                            'JV' as VoucherType,
                            CONVERT(varchar, jv.VoucherNo) as VoucherNo,
                            jv.VoucherDate,
                            jvb.Narration,
                            jvb.Debit,
                            jvb.Credit,
                            '' as BilltyNo,
                            '' as Shipment,
                            ISNULL(jvb.Refrence, '') as Reference,
                            CASE WHEN jv.ScannedDocuments IS NOT NULL THEN 'Available' ELSE '' END as DocumentProof,
                            8 as OrderCol
                        FROM JournalVouchers jv
                        INNER JOIN JournalVouchersBody jvb ON jv.VoucherNo = jvb.VoucherNo
                        WHERE jvb.AccountNo = @PartyID 
                            AND jv.VoucherDate >= @FromDate 
                            AND jv.VoucherDate <= @ToDate
                            AND jv.IsPosted = 1
                        
                        UNION ALL
                        
                        -- Get Purchase Invoices
                        SELECT 
                            'PI' as VoucherType,
                            CONVERT(varchar, p.PurchaseID) as VoucherNo,
                            p.PurchaseDate as VoucherDate,
                            'Purchase Invoice - ' + 
                            STUFF((SELECT ', ' + pb.ProductId + ' Qty:' + CONVERT(varchar, pb.QtyPack)
                                   FROM PurchasesBody pb 
                                   WHERE pb.PurchaseId = p.PurchaseID
                                   FOR XML PATH('')), 1, 2, '') as Narration,
                            0 as Debit,
                            ISNULL((SELECT SUM(pb.TTLValue) FROM PurchasesBody pb WHERE pb.PurchaseId = p.PurchaseID), 0) as Credit,
                            ISNULL(p.BiltyNo, '') as BilltyNo,
                            ISNULL(p.TransportName, '') as Shipment,
                            ISNULL(p.BillNo, '') as Reference,
                            CASE WHEN p.ScannedDocuments IS NOT NULL THEN 'Available' ELSE '' END as DocumentProof,
                            1 as OrderCol
                        FROM Purchase p
                        WHERE p.VendorID = @PartyID 
                            AND p.PurchaseDate >= @FromDate 
                            AND p.PurchaseDate <= @ToDate
                        
                        UNION ALL
                        
                        -- Get Sales Invoices
                        SELECT 
                            'SI' as VoucherType,
                            CONVERT(varchar, s.SaleId) as VoucherNo,
                            s.SaleDate as VoucherDate,
                            s.Narration,
                            ISNULL((SELECT SUM(sb.TTLValue) FROM SalesBody sb WHERE sb.SaleId = s.SaleID), 0) as Debit,
                            0 as Credit,
                            ISNULL(s.BiltyNo, '') as BilltyNo,
                            ISNULL(s.TransportName, '') as Shipment,
                            ISNULL(s.BillNo, '') as Reference,
                            CASE WHEN s.ScannedDocuments IS NOT NULL THEN 'Available' ELSE '' END as DocumentProof,
                            2 as OrderCol
                        FROM Sales s
                        WHERE s.CustomerId = @PartyID 
                            AND s.SaleDate >= @FromDate 
                            AND s.SaleDate <= @ToDate
                            AND s.IsPosted = 1
                        
                        ORDER BY VoucherDate";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ledger data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private Dictionary<string, string> GetReportParameters(string partyID, string partyName,
            DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get party details
                    string partyQuery = @"
                        SELECT 
                            ISNULL(p.Address, '') as Address,
                            ISNULL(p.City, '') as City,
                            ISNULL(p.Phone1, '') as Phone1,
                            ISNULL(p.Mobile, '') as Mobile
                        FROM Parties p
                        WHERE p.PartyID = @PartyID";

                    using (SqlCommand cmd = new SqlCommand(partyQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string address = reader["Address"].ToString();
                                if (!string.IsNullOrEmpty(reader["City"].ToString()))
                                    address += ", " + reader["City"].ToString();

                                string phone = !string.IsNullOrEmpty(reader["Mobile"].ToString())
                                    ? reader["Mobile"].ToString()
                                    : reader["Phone1"].ToString();

                                parameters.Add("PartyID", partyID);
                                parameters.Add("PartyName", partyName);
                                parameters.Add("PartyAddress", address);
                                parameters.Add("PartyPhone", phone);
                            }
                        }
                    }

                    // Add date parameters
                    //parameters.Add("FromDate", fromDate.ToString("dd-MMM-yyyy"));
                    //parameters.Add("ToDate", toDate.ToString("dd-MMM-yyyy"));
                    //parameters.Add("GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"));
                    //parameters.Add("ReportTitle", "DAILY PARTY LEDGER REPORT");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting parameters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return parameters;
        }

        private void ShowReportToUser(DataTable reportData, Dictionary<string, string> parameters, bool printMode)
        {
            try
            {
                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", reportData }
                };

                // Create report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    // Show the report
                    reportViewer.ShowReport("Daily_Parties_ledger.rdlc", dataSources);

                    // If in print mode, show message to user
                    if (printMode)
                    {
                        MessageBox.Show("Report loaded. Click the PRINT button (📄) in the report toolbar.",
                            "Ready to Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ========== DATE RANGE POPUP FORM ==========

    public class DateRangePopup : Form
    {
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnOK;
        private Button btnCancel;

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        public DateRangePopup()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Form settings
            this.Text = "Select Date Range";
            this.Size = new Size(320, 180);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            //this.TopMost = true;

            // From Date Label
            Label lblFrom = new Label
            {
                Text = "From Date:",
                Location = new Point(20, 25),
                Size = new Size(70, 20),
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(lblFrom);

            // From Date Picker
            dtpFrom = new DateTimePicker
            {
                Location = new Point(100, 23),
                Size = new Size(180, 22),
                Value = DateTime.Today,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(dtpFrom);

            // To Date Label
            Label lblTo = new Label
            {
                Text = "To Date:",
                Location = new Point(20, 65),
                Size = new Size(70, 20),
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(lblTo);

            // To Date Picker
            dtpTo = new DateTimePicker
            {
                Location = new Point(100, 63),
                Size = new Size(180, 22),
                Value = DateTime.Today,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Arial", 9)
            };
            this.Controls.Add(dtpTo);

            // OK Button
            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(100, 105),
                Size = new Size(85, 30),
                Font = new Font("Arial", 9),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(195, 105),
                Size = new Size(85, 30),
                Font = new Font("Arial", 9),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancel);

            // Set default buttons
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dtpFrom.Value > dtpTo.Value)
            {
                MessageBox.Show("From date cannot be after To date", "Invalid",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
            }
            else
            {
                FromDate = dtpFrom.Value;
                ToDate = dtpTo.Value;
            }
        }
    }
}