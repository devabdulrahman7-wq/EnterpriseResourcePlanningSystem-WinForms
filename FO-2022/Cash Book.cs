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
        private string reportType = "CashBook";
        private bool formLoaded = false;

        public Cash_Book()
        {
            InitializeComponent();
        }

        private void Cash_Book_Load(object sender, EventArgs e)
        {
            formLoaded = true;

            // Set default radio buttons
            alldatebtn.Checked = true;
            viewprntbtn.Checked = true;
            viewprtrtbtn.Checked = true;

            // Set default dates for all dates option
            fromDate = new DateTime(1900, 1, 1);
            toDate = DateTime.Today;
        }

        // ========== DATE OPTION HANDLERS ==========
        private void alldatebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded || !alldatebtn.Checked) return;

            fromDate = new DateTime(1900, 1, 1);
            toDate = DateTime.Today;
        }

        private void todaybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded || !todaybtn.Checked) return;

            fromDate = DateTime.Today;
            toDate = DateTime.Today;
        }

        private void datebwbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded || !datebwbtn.Checked) return;

            ShowDateRangePopup(true);
        }

        private void daybtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded || !daybtn.Checked) return;

            ShowDateRangePopup(false);
        }

        // ========== VIEW TYPE HANDLERS ==========
        private void viewprntbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;
            if (viewprntbtn.Checked)
            {
                reportType = "CashBook";
            }
        }

        private void viewlandscapebtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;
            if (viewlandscapebtn.Checked)
            {
                reportType = "CashBookLandscape";
            }
        }

        // ========== PRINT TYPE HANDLERS ==========
        private void viewprtrtbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Portrait view - no action needed as it's default
        }

        private void viewlndscpbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Landscape view - no action needed
        }

        // ========== BUTTON HANDLERS ==========
        private void previewbtn_Click(object sender, EventArgs e)
        {
            AnimateButton(previewbtn);
            GenerateReport(false);
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            AnimateButton(printbtn);
            GenerateReport(true);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ========== HELPER METHODS ==========
        private void AnimateButton(Button button)
        {
            Color originalColor = button.BackColor;
            button.BackColor = Color.LightBlue;

            Timer timer = new Timer();
            timer.Interval = 200;
            timer.Tick += (s, ev) =>
            {
                button.BackColor = originalColor;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ShowDateRangePopup(bool isDateBetween)
        {
            try
            {
                using (CashBookDateRangePopup popup = new CashBookDateRangePopup(isDateBetween))
                {
                    popup.TopMost = true; // Make it topmost

                    if (popup.ShowDialog(this) == DialogResult.OK)
                    {
                        if (isDateBetween)
                        {
                            fromDate = popup.SelectedFromDate;
                            toDate = popup.SelectedToDate;
                        }
                        else
                        {
                            fromDate = popup.SelectedSingleDate;
                            toDate = popup.SelectedSingleDate;
                        }
                    }
                    else
                    {
                        // User cancelled - revert to All Date
                        alldatebtn.Checked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in date selection: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                alldatebtn.Checked = true;
            }
        }

        private void GenerateReport(bool printImmediately)
        {
            try
            {
                // Validate date range
                if (fromDate > toDate)
                {
                    MessageBox.Show("From date cannot be after To date.",
                        "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;
                EnableButtons(false);

                // Get data from database
                DataTable cashData = GetCashBookData();

                if (cashData == null || cashData.Rows.Count == 0)
                {
                    MessageBox.Show("No cash transactions found for selected date range.",
                        "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate running balance
                CalculateRunningBalance(cashData);

                // Show report
                ShowReport(cashData, printImmediately);
            }
            catch (SqlException sqlEx)
            {
                HandleSqlException(sqlEx);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                EnableButtons(true);
            }
        }

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
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);
                        cmd.Parameters.AddWithValue("@ProceedingZeros", 5);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get cash book data: {ex.Message}", ex);
            }

            return dt;
        }

        private void CalculateRunningBalance(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
                return;

            // Ensure required columns exist
            if (!dataTable.Columns.Contains("RunningBalance"))
                dataTable.Columns.Add("RunningBalance", typeof(decimal));

            if (!dataTable.Columns.Contains("Debit"))
                dataTable.Columns.Add("Debit", typeof(decimal));

            if (!dataTable.Columns.Contains("Credit"))
                dataTable.Columns.Add("Credit", typeof(decimal));

            decimal runningBalance = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                // Handle DBNull values
                string voucherType = row["VoucherType"] != DBNull.Value ? row["VoucherType"].ToString() : "";
                decimal debit = row["Debit"] != DBNull.Value ? Convert.ToDecimal(row["Debit"]) : 0;
                decimal credit = row["Credit"] != DBNull.Value ? Convert.ToDecimal(row["Credit"]) : 0;

                if (voucherType == "-") // Opening balance
                {
                    runningBalance = debit - credit;
                }
                else
                {
                    runningBalance = runningBalance + debit - credit;
                }

                row["RunningBalance"] = runningBalance;
            }
        }

        private void ShowReport(DataTable cashData, bool printImmediately)
        {
            try
            {
                // Determine which report to show based on view type and print type
                string reportName = "Cash_Book.rdlc";

                if (viewlandscapebtn.Checked)
                {
                    reportName = "Cash_Book.rdlc"; // Or create a separate landscape version
                }
                else if (viewlndscpbtn.Checked && viewprntbtn.Checked)
                {
                    reportName = "Cash_Book.rdlc";
                }

                // Prepare parameters for the report
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //    { "CompanyName", "Your Company Name" },
                    //    { "ReportTitle", "CASH BOOK" },
                    //    { "DateRange", GetDateRangeText() },
                    //    { "FromDate", fromDate.ToString("dd-MMM-yyyy") },
                    //    { "ToDate", toDate.ToString("dd-MMM-yyyy") },
                    //    { "GeneratedOn", DateTime.Now.ToString("dd-MMM-yyyy HH:mm") },
                    //    { "TotalRecords", cashData.Rows.Count.ToString() }
                };

            // Prepare data source
            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
                {
                    { "DataSet1", cashData }
                };

                // Show report viewer
                using (Report_viewer reportViewer = new Report_viewer())
                {
                    reportViewer.ShowReport(reportName, dataSources);

                    if (printImmediately)
                    {
                        //reportViewer.PrintReport();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing report: {ex.Message}",
                    "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetDateRangeText()
        {
            if (alldatebtn.Checked)
                return "All Dates";
            else if (todaybtn.Checked)
                return $"Today: {DateTime.Today:dd-MMM-yyyy}";
            else if (daybtn.Checked)
                return $"{fromDate:dd-MMM-yyyy}";
            else
                return $"{fromDate:dd-MMM-yyyy} to {toDate:dd-MMM-yyyy}";
        }

        private void EnableButtons(bool enable)
        {
            previewbtn.Enabled = enable;
            printbtn.Enabled = enable;
            alldatebtn.Enabled = enable;
            todaybtn.Enabled = enable;
            datebwbtn.Enabled = enable;
            daybtn.Enabled = enable;
            viewprntbtn.Enabled = enable;
            viewlandscapebtn.Enabled = enable;
            viewprtrtbtn.Enabled = enable;
            viewlndscpbtn.Enabled = enable;
        }

        private void HandleSqlException(SqlException sqlEx)
        {
            if (sqlEx.Number == -2 || sqlEx.Message.Contains("timeout"))
            {
                MessageBox.Show("Database query timed out. Please try:\n" +
                               "1. Selecting a smaller date range\n" +
                               "2. Trying during off-peak hours\n" +
                               "3. Contacting your system administrator",
                               "Query Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}\nError Code: {sqlEx.Number}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            // Form settings
            this.Text = _isDateBetweenMode ? "Select Date Range" : "Select A Day";
            this.Size = _isDateBetweenMode ? new Size(350, 180) : new Size(350, 150);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true; // Make it topmost

            int yPos = 20;

            if (_isDateBetweenMode)
            {
                // From Date
                Label lblFrom = new Label
                {
                    Text = "From Date:",
                    Location = new Point(20, yPos),
                    Size = new Size(80, 25),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblFrom);

                _dtpFrom = new DateTimePicker
                {
                    Location = new Point(120, yPos),
                    Size = new Size(200, 25),
                    Value = DateTime.Today.AddMonths(-1),
                    Format = DateTimePickerFormat.Short
                };
                this.Controls.Add(_dtpFrom);

                yPos += 40;

                // To Date
                Label lblTo = new Label
                {
                    Text = "To Date:",
                    Location = new Point(20, yPos),
                    Size = new Size(80, 25),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblTo);

                _dtpTo = new DateTimePicker
                {
                    Location = new Point(120, yPos),
                    Size = new Size(200, 25),
                    Value = DateTime.Today,
                    Format = DateTimePickerFormat.Short
                };
                this.Controls.Add(_dtpTo);

                yPos += 40;
            }
            else
            {
                // Single Date
                Label lblDate = new Label
                {
                    Text = "Select Date:",
                    Location = new Point(20, yPos),
                    Size = new Size(80, 25),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblDate);

                _dtpSingle = new DateTimePicker
                {
                    Location = new Point(120, yPos),
                    Size = new Size(200, 25),
                    Value = DateTime.Today,
                    Format = DateTimePickerFormat.Short
                };
                this.Controls.Add(_dtpSingle);

                yPos += 50;
            }

            // OK Button
            _btnOK = new Button
            {
                Text = "OK",
                Location = new Point(120, yPos),
                Size = new Size(85, 30),
                DialogResult = DialogResult.OK
            };
            _btnOK.Click += BtnOK_Click;
            this.Controls.Add(_btnOK);

            // Cancel Button
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(215, yPos),
                Size = new Size(85, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(_btnCancel);

            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (_isDateBetweenMode)
            {
                if (_dtpFrom.Value > _dtpTo.Value)
                {
                    MessageBox.Show("From date cannot be after To date.",
                        "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                SelectedFromDate = _dtpFrom.Value.Date;
                SelectedToDate = _dtpTo.Value.Date;
            }
            else
            {
                SelectedSingleDate = _dtpSingle.Value.Date;
                SelectedFromDate = SelectedSingleDate;
                SelectedToDate = SelectedSingleDate;
            }
        }
    }
}