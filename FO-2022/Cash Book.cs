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
    public partial class Cash_Book : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime fromDate;
        private DateTime toDate;
        private string reportType = "CashBook"; // Default to Cash Book

        public Cash_Book()
        {
            InitializeComponent();
        }

        private void Cash_Book_Load(object sender, EventArgs e)
        {
            // Set default radio buttons
            alldatebtn.Checked = true;

            // Set default dates
            fromDate = new DateTime(1900, 1, 1);
            toDate = DateTime.Today;

            // Set default report and view types
            reportType = "CashBook";

            // Set default print view
            if (viewprtrtbtn != null)
                viewprtrtbtn.Checked = true;
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

        // ========== DATE OPTION HANDLERS ==========

        private void alldatebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (alldatebtn.Checked)
            {
                fromDate = new DateTime(1900, 1, 1);
                toDate = DateTime.Today;
            }
        }

        private void datebwbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (datebwbtn.Checked)
            {
                ShowDateRangePopup(true); // true = Date Between mode
            }
        }

        private void todaybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (todaybtn.Checked)
            {
                fromDate = DateTime.Today;
                toDate = DateTime.Today;
            }
        }

        private void daybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (daybtn.Checked)
            {
                ShowDateRangePopup(false); // false = A Day mode
            }
        }

        // ========== VIEW TYPE HANDLERS (GroupBox1) ==========

        private void viewprntbtn_CheckedChanged(object sender, EventArgs e)
        {
            // View Print selected - Set to Cash Book
            if (viewprntbtn.Checked)
            {
                reportType = "CashBook";
            }
        }

        private void viewlandscapebtn_CheckedChanged(object sender, EventArgs e)
        {
            // View Landscape selected - Set to Cash Ledger
            if (viewlandscapebtn.Checked)
            {
                reportType = "CashLedger";
            }
        }

        // ========== PRINT TYPE HANDLERS (GroupBox2) ==========

        private void viewprtrtbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Portrait view selected
        }

        private void viewlndscpbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Landscape view selected
        }

        // ========== BUTTON HANDLERS ==========

        private void previewbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(false);
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            GenerateReport(true);
        }

        // ========== MAIN METHODS ==========

        private void ShowDateRangePopup(bool isDateBetween)
        {
            using (CashBookDateRangePopup popup = new CashBookDateRangePopup(isDateBetween))
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    if (isDateBetween)
                    {
                        // Date Between mode
                        fromDate = popup.SelectedFromDate;
                        toDate = popup.SelectedToDate;
                    }
                    else
                    {
                        // A Day mode
                        fromDate = popup.SelectedSingleDate;
                        toDate = popup.SelectedSingleDate;
                    }
                }
                else
                {
                    // User cancelled, revert to All Date
                    alldatebtn.Checked = true;
                }
            }
        }

        private void GenerateReport(bool printImmediately)
        {
            try
            {
                // Get cash data using stored procedure
                DataTable cashData = GetCashBookData();

                if (cashData == null || cashData.Rows.Count == 0)
                {
                    MessageBox.Show("No cash transactions found for selected date range.",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate running balance
                cashData = CalculateRunningBalance(cashData);

                // Prepare data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", cashData }
                };

                // Get report parameters
                Dictionary<string, string> parameters = GetReportParameters();

                // Show report
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    string reportName = (reportType == "CashBook") ? "Cash_Book.rdlc" : "Cash_Ledger.rdlc";

                    reportViewer.ShowReport(reportName, dataSources);

                    if (printImmediately)
                    {
                        MessageBox.Show("Report loaded. Click the PRINT button in the report toolbar.",
                            "Ready to Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private DataTable GetCashBookData()
        //{
        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            con.Open();

        //            using (SqlCommand cmd = new SqlCommand("SPCashBook", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@FromDate", fromDate);
        //                cmd.Parameters.AddWithValue("@ToDate", toDate);
        //                cmd.Parameters.AddWithValue("@ProceedingZeros", 5);
        //                cmd.CommandTimeout = 120;
        //                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //                {
        //                    da.Fill(dt);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error fetching cash data: {ex.Message}",
        //            "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    return dt;
        //}
        private DataTable GetCashBookData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SPCashBook", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 300; // 5 minutes - SP is complex
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);
                        cmd.Parameters.AddWithValue("@ProceedingZeros", 5);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }

                // Debug: Show what columns are returned
                if (dt.Columns.Count > 0)
                {
                    string columnInfo = "Columns returned by SPCashBook:\n\n";
                    foreach (DataColumn col in dt.Columns)
                    {
                        columnInfo += $"{col.ColumnName} ({col.DataType.Name})\n";
                    }
                    columnInfo += $"\nTotal rows: {dt.Rows.Count}";

                    MessageBox.Show(columnInfo, "Debug - SP Result",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Check if we have required columns
                if (dt.Columns.Contains("AccountName") && dt.Rows.Count > 0)
                {
                    // Test: Show first row AccountName
                    string firstAccName = dt.Rows[0]["AccountName"] != DBNull.Value ?
                        dt.Rows[0]["AccountName"].ToString() : "NULL";
                    MessageBox.Show($"First row AccountName: {firstAccName}",
                        "Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching cash data: {ex.Message}\n\n" +
                               $"Make sure stored procedure SPCashBook exists and is working.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private DataTable CalculateRunningBalance(DataTable dataTable)
        {
            // Check if dataTable is valid
            if (dataTable == null || dataTable.Columns.Count == 0)
                return dataTable;

            // Add RunningBalance column if not exists
            if (!dataTable.Columns.Contains("RunningBalance"))
            {
                dataTable.Columns.Add("RunningBalance", typeof(decimal));
            }

            decimal runningBalance = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                // Check if required columns exist
                string voucherType = "";
                if (dataTable.Columns.Contains("VoucherType") && row["VoucherType"] != DBNull.Value)
                    voucherType = row["VoucherType"].ToString();

                decimal debit = 0;
                if (dataTable.Columns.Contains("Debit") && row["Debit"] != DBNull.Value)
                    debit = Convert.ToDecimal(row["Debit"]);

                decimal credit = 0;
                if (dataTable.Columns.Contains("Credit") && row["Credit"] != DBNull.Value)
                    credit = Convert.ToDecimal(row["Credit"]);

                // For opening balance row (VoucherType = '-')
                if (voucherType == "-")
                {
                    runningBalance = debit - credit;
                }
                else
                {
                    runningBalance = runningBalance + debit - credit;
                }

                row["RunningBalance"] = runningBalance;
            }

            return dataTable;
        }
        //private DataTable CalculateRunningBalance(DataTable dataTable)
        //{
        //    // Add RunningBalance column if not exists
        //    if (!dataTable.Columns.Contains("RunningBalance"))
        //    {
        //        dataTable.Columns.Add("RunningBalance", typeof(decimal));
        //    }

        //    decimal runningBalance = 0;

        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
        //        decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;

        //        // For opening balance row (VoucherType = '-')
        //        if (row["VoucherType"].ToString() == "-")
        //        {
        //            runningBalance = debit - credit;
        //        }
        //        else
        //        {
        //            runningBalance = runningBalance + debit - credit;
        //        }

        //        row["RunningBalance"] = runningBalance;
        //    }

        //    return dataTable;
        //}

        private Dictionary<string, string> GetReportParameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            // Add report parameters
            //parameters.Add("ReportTitle", (reportType == "CashBook") ? "CASH BOOK" : "CASH LEDGER");
            //parameters.Add("ReportType", (reportType == "CashBook") ? "Book" : "Ledger");
            //parameters.Add("Orientation", viewprtrtbtn.Checked ? "Portrait" : "Landscape");
            //parameters.Add("GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt"));

            // Add date range text
            string dateRangeText = "";
            if (alldatebtn.Checked)
                dateRangeText = "All Date";
            else if (datebwbtn.Checked)
                dateRangeText = $"{fromDate:dd-MMM-yyyy} to {toDate:dd-MMM-yyyy}";
            else if (todaybtn.Checked)
                dateRangeText = "Today";
            else if (daybtn.Checked)
                dateRangeText = $"{fromDate:dd-MMM-yyyy}";

            parameters.Add("DateRange", dateRangeText);
            parameters.Add("FromDate", fromDate.ToString("dd-MMM-yyyy"));
            parameters.Add("ToDate", toDate.ToString("dd-MMM-yyyy"));

            return parameters;
        }
    }

    // ========== DATE RANGE POPUP FORM ==========
    public class CashBookDateRangePopup : Form
    {
        private bool _isDateBetweenMode;
        private DateTimePicker _dtpFrom;
        private DateTimePicker _dtpTo;
        private DateTimePicker _dtpSingle;
        private Button _btnOK;
        private Button _btnCancel;

        public DateTime SelectedFromDate { get; private set; }
        public DateTime SelectedToDate { get; private set; }
        public DateTime SelectedSingleDate { get; private set; }

        public CashBookDateRangePopup(bool isDateBetween)
        {
            _isDateBetweenMode = isDateBetween;
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Form settings based on mode
            if (_isDateBetweenMode)
            {
                this.Text = "Select Date Range";
                this.Size = new Size(320, 200);
            }
            else
            {
                this.Text = "Select A Day";
                this.Size = new Size(320, 160);
            }

            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
            if (_isDateBetweenMode)
            {
                // DATE BETWEEN MODE
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
                _dtpFrom = new DateTimePicker
                {
                    Location = new Point(100, 23),
                    Size = new Size(180, 22),
                    Value = DateTime.Today.AddMonths(-1),
                    Format = DateTimePickerFormat.Short,
                    Font = new Font("Arial", 9)
                };
                this.Controls.Add(_dtpFrom);

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
                _dtpTo = new DateTimePicker
                {
                    Location = new Point(100, 63),
                    Size = new Size(180, 22),
                    Value = DateTime.Today,
                    Format = DateTimePickerFormat.Short,
                    Font = new Font("Arial", 9)
                };
                this.Controls.Add(_dtpTo);
            }
            else
            {
                // A DAY MODE
                // Single Date Label
                Label lblDate = new Label
                {
                    Text = "Select Date:",
                    Location = new Point(20, 25),
                    Size = new Size(70, 20),
                    Font = new Font("Arial", 9)
                };
                this.Controls.Add(lblDate);

                // Single Date Picker
                _dtpSingle = new DateTimePicker
                {
                    Location = new Point(100, 23),
                    Size = new Size(180, 22),
                    Value = DateTime.Today,
                    Format = DateTimePickerFormat.Short,
                    Font = new Font("Arial", 9)
                };
                this.Controls.Add(_dtpSingle);
            }

            // OK Button
            _btnOK = new Button
            {
                Text = "OK",
                Location = new Point(100, _isDateBetweenMode ? 105 : 70),
                Size = new Size(85, 30),
                Font = new Font("Arial", 9),
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += BtnOK_Click;
            this.Controls.Add(_btnOK);

            // Cancel Button
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(195, _isDateBetweenMode ? 105 : 70),
                Size = new Size(85, 30),
                Font = new Font("Arial", 9),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(_btnCancel);

            // Set default buttons
            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (_isDateBetweenMode)
            {
                if (_dtpFrom.Value > _dtpTo.Value)
                {
                    MessageBox.Show("From date cannot be after To date",
                        "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                }
                else
                {
                    SelectedFromDate = _dtpFrom.Value;
                    SelectedToDate = _dtpTo.Value;
                }
            }
            else
            {
                SelectedSingleDate = _dtpSingle.Value;
                SelectedFromDate = SelectedSingleDate;
                SelectedToDate = SelectedSingleDate;
            }
        }
    }
}