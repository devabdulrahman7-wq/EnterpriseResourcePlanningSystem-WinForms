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

            // Disable datepicker initially (for Date Between option)
            todaydatetimepicker.Enabled = true;

            // Disable buttons initially
            previewbtn.Enabled = false;
            printbtn.Enabled = false;
        }

        private void todayradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (todayradiobtn.Checked)
            {
                // Today selected - enable datepicker
                todaydatetimepicker.Enabled = true;
                todaydatetimepicker.Value = DateTime.Today;
            }
        }

        private void datebwradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (datebwradiobtn.Checked)
            {
                // Date Between selected - disable datepicker
                todaydatetimepicker.Enabled = false;
                todaydatetimepicker.Value = DateTime.Today;
            }
        }

        private void searchpartyid_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please enter Party ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                // Display party name
                                partynametxtbox.Text = reader["PartyName"].ToString();

                                // Enable preview and print buttons
                                previewbtn.Enabled = true;
                                printbtn.Enabled = true;
                            }
                            else
                            {
                                MessageBox.Show("Party ID not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                partynametxtbox.Text = "";
                                previewbtn.Enabled = false;
                                printbtn.Enabled = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateAndShowReport(bool printImmediately = false)
        {
            if (string.IsNullOrWhiteSpace(partyidtxtbox.Text))
            {
                MessageBox.Show("Please select a party first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DateTime fromDate, toDate;

                if (todayradiobtn.Checked)
                {
                    // Today option - use selected date
                    fromDate = todaydatetimepicker.Value.Date;
                    toDate = todaydatetimepicker.Value.Date;
                }
                else if (datebwradiobtn.Checked)
                {
                    // Date Between option - SHOW INPUT BOX for dates
                    InputDateRangeForm dateRangeForm = new InputDateRangeForm();
                    if (dateRangeForm.ShowDialog() == DialogResult.OK)
                    {
                        fromDate = dateRangeForm.FromDate;
                        toDate = dateRangeForm.ToDate;

                        if (fromDate > toDate)
                        {
                            MessageBox.Show("From date cannot be greater than To date", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        return; // User cancelled
                    }
                }
                else
                {
                    MessageBox.Show("Please select date option", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data for report
                Dictionary<string, DataTable> dataSources = GetReportDataSources(partyidtxtbox.Text.Trim(), fromDate, toDate);

                if (dataSources.Count == 0 || !dataSources.ContainsKey("DataSet1") || dataSources["DataSet1"].Rows.Count == 0)
                {
                    MessageBox.Show("No ledger data found for the selected party and date range", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get report parameters
                Dictionary<string, string> parameters = GetReportParameters(partyidtxtbox.Text.Trim(), fromDate, toDate);

                // Create and show report viewer
                Report_viewer reportViewer = new Report_viewer();

                if (printImmediately)
                {
                    reportViewer.ShowReport("Daily_Parties_ledger.rdlc", dataSources);
                }
                else
                {
                    reportViewer.ShowReport("Daily_Parties_ledger.rdlc", dataSources);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Dictionary<string, DataTable> GetReportDataSources(string partyID, DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
            DataTable dtLedger = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get party details
                    string partyName = "";
                    string partyPhone = "";
                    string partyAddress = "";
                    string partyCity = "";
                    decimal openingBalance = 0;

                    string partyQuery = @"
                        SELECT 
                            p.PartyName, 
                            p.Phone1, 
                            p.Address, 
                            p.City,
                            ISNULL(ab.OpeningBal, 0) as OpeningBalance
                        FROM Parties p
                        LEFT JOIN AccountsBalances ab ON p.PartyID = ab.AccountNo
                        WHERE p.PartyID = @PartyID";

                    using (SqlCommand cmd = new SqlCommand(partyQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                partyName = reader["PartyName"].ToString();
                                partyPhone = reader["Phone1"].ToString();
                                partyAddress = reader["Address"].ToString();
                                partyCity = reader["City"].ToString();
                                openingBalance = Convert.ToDecimal(reader["OpeningBalance"]);
                            }
                        }
                    }

                    // Get transactions
                    string transactionQuery = @"
                        SELECT 
                            CONVERT(varchar, VoucherDate, 106) as Date,
                            VoucherNo,
                            Narration as Description,
                            ISNULL(Debit, 0) as Debit,
                            ISNULL(Credit, 0) as Credit
                        FROM AllVouchers 
                        WHERE AccountNo = @PartyID 
                            AND VoucherDate >= @FromDate 
                            AND VoucherDate <= @ToDate
                        ORDER BY VoucherDate, VoucherNo";

                    using (SqlCommand cmd = new SqlCommand(transactionQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtLedger);

                            // Add additional columns for the report
                            if (!dtLedger.Columns.Contains("PartyName"))
                                dtLedger.Columns.Add("PartyName", typeof(string));

                            if (!dtLedger.Columns.Contains("PartyPhone"))
                                dtLedger.Columns.Add("PartyPhone", typeof(string));

                            if (!dtLedger.Columns.Contains("OpeningBalance"))
                                dtLedger.Columns.Add("OpeningBalance", typeof(decimal));

                            if (!dtLedger.Columns.Contains("PartyAddress"))
                                dtLedger.Columns.Add("PartyAddress", typeof(string));

                            // Add party details to all rows
                            string fullAddress = partyAddress;
                            if (!string.IsNullOrWhiteSpace(partyCity))
                                fullAddress += ", " + partyCity;

                            foreach (DataRow row in dtLedger.Rows)
                            {
                                row["PartyName"] = partyName;
                                row["PartyPhone"] = partyPhone;
                                row["PartyAddress"] = fullAddress;
                                row["OpeningBalance"] = openingBalance;
                            }

                            // If no transactions found, add one row with party info only
                            if (dtLedger.Rows.Count == 0)
                            {
                                DataRow newRow = dtLedger.NewRow();
                                newRow["PartyName"] = partyName;
                                newRow["PartyPhone"] = partyPhone;
                                newRow["PartyAddress"] = fullAddress;
                                newRow["OpeningBalance"] = openingBalance;
                                dtLedger.Rows.Add(newRow);
                            }
                        }
                    }
                }

                dataSources.Add("DataSet1", dtLedger);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading ledger data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataSources;
        }

        private Dictionary<string, string> GetReportParameters(string partyID, DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get party name for parameter
                    string partyQuery = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                    string partyName = "";

                    using (SqlCommand cmd = new SqlCommand(partyQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            partyName = result.ToString();
                    }

                    // Add parameters
                    //parameters.Add("ReportTitle", "DAILY PARTY LEDGER REPORT");
                    //parameters.Add("PartyID", partyID);
                    //parameters.Add("PartyName", partyName);
                    //parameters.Add("FromDate", fromDate.ToString("dd-MMM-yyyy"));
                    //parameters.Add("ToDate", toDate.ToString("dd-MMM-yyyy"));
                    //parameters.Add("GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"));

                    // Get summary information
                    string summaryQuery = @"
                        SELECT 
                            COUNT(*) as TotalTransactions,
                            ISNULL(SUM(Debit), 0) as TotalDebit,
                            ISNULL(SUM(Credit), 0) as TotalCredit
                        FROM AllVouchers 
                        WHERE AccountNo = @PartyID 
                            AND VoucherDate >= @FromDate 
                            AND VoucherDate <= @ToDate";

                    using (SqlCommand cmd = new SqlCommand(summaryQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", partyID);
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                parameters.Add("TotalTransactions", reader["TotalTransactions"].ToString());
                                parameters.Add("TotalDebit", string.Format("{0:N2}", reader["TotalDebit"]));
                                parameters.Add("TotalCredit", string.Format("{0:N2}", reader["TotalCredit"]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting report parameters: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return parameters;
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateAndShowReport(false);
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateAndShowReport(true);
        }

        private void partyidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow Enter key to trigger search
            if (e.KeyChar == (char)Keys.Enter)
            {
                searchpartyid_Click(sender, e);
                e.Handled = true;
            }
        }
    }

    // 🔽 ADD THIS FORM CLASS FOR DATE RANGE INPUT
    public class InputDateRangeForm : Form
    {
        private DateTimePicker fromDatePicker;
        private DateTimePicker toDatePicker;
        private Button btnOK;
        private Button btnCancel;

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        public InputDateRangeForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Date Range";
            this.Size = new Size(300, 180);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // From Date Label
            Label lblFrom = new Label();
            lblFrom.Text = "From Date:";
            lblFrom.Location = new Point(20, 20);
            lblFrom.Size = new Size(70, 20);
            this.Controls.Add(lblFrom);

            // From Date Picker
            fromDatePicker = new DateTimePicker();
            fromDatePicker.Location = new Point(100, 20);
            fromDatePicker.Size = new Size(150, 20);
            fromDatePicker.Value = DateTime.Today;
            this.Controls.Add(fromDatePicker);

            // To Date Label
            Label lblTo = new Label();
            lblTo.Text = "To Date:";
            lblTo.Location = new Point(20, 60);
            lblTo.Size = new Size(70, 20);
            this.Controls.Add(lblTo);

            // To Date Picker
            toDatePicker = new DateTimePicker();
            toDatePicker.Location = new Point(100, 60);
            toDatePicker.Size = new Size(150, 20);
            toDatePicker.Value = DateTime.Today;
            this.Controls.Add(toDatePicker);

            // OK Button
            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new Point(100, 100);
            btnOK.Size = new Size(75, 25);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(180, 100);
            btnCancel.Size = new Size(75, 25);
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            FromDate = fromDatePicker.Value.Date;
            ToDate = toDatePicker.Value.Date;
        }
    }
}