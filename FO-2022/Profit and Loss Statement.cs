using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Profit_and_Loss_Statement : Form
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime fromDate;
        private DateTime toDate;

        public Profit_and_Loss_Statement()
        {
            InitializeComponent();

            // Set defaults
            tilldatebtnradio.Checked = true;
            dateTimePickerfortilldate.Value = DateTime.Now;
            SetTillDateRange();
        }

        private void SetTillDateRange()
        {
            toDate = dateTimePickerfortilldate.Value;
            // Set from date to start of current month (you can change this logic)
            fromDate = new DateTime(toDate.Year, toDate.Month, 1);
        }

        private void tilldatebtnradio_CheckedChanged(object sender, EventArgs e)
        {
            if (tilldatebtnradio.Checked)
            {
                dateTimePickerfortilldate.Enabled = true;
                SetTillDateRange();
            }
        }

        private void daterangebtnradio_CheckedChanged(object sender, EventArgs e)
        {
            if (daterangebtnradio.Checked)
            {
                dateTimePickerfortilldate.Enabled = false;

                // Create simple date range dialog
                SimpleDateRangeDialog dialog = new SimpleDateRangeDialog();
                dialog.TopMost = true; // Make it topmost

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fromDate = dialog.FromDate;
                    toDate = dialog.ToDate;
                }
                else
                {
                    tilldatebtnradio.Checked = true;
                }
            }
        }

        private void dateTimePickerfortilldate_ValueChanged(object sender, EventArgs e)
        {
            if (tilldatebtnradio.Checked)
            {
                SetTillDateRange();
            }
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();

            try
            {
                // Simple test query to check if we have data
                string query = @"
                SELECT 
                    ca.AccountNo,
                    ca.AccountName,
                    CASE WHEN ca.PLFlag = 'R' THEN 'Revenue' ELSE 'Expense' END as AccountCategory,
                    SUM(ISNULL(av.Credit, 0)) as Credit,
                    SUM(ISNULL(av.Debit, 0)) as Debit
                FROM ChartOfAccounts ca
                LEFT JOIN AllVouchers av ON ca.AccountNo = av.AccountNo
                    AND av.VoucherDate BETWEEN @FromDate AND @ToDate
                WHERE ca.PLFlag IN ('R', 'E')
                GROUP BY ca.AccountNo, ca.AccountName, ca.PLFlag
                HAVING SUM(ISNULL(av.Credit, 0)) != 0 OR SUM(ISNULL(av.Debit, 0)) != 0
                ORDER BY ca.PLFlag DESC, ca.AccountName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void ShowReport()
        {
            DataTable dt = GetData();

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No data found. Try changing date range.", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>
            {
                {"DataSet1", dt}
            };

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                //{"FromDate", fromDate.ToString("dd/MM/yyyy")},
                //{"ToDate", toDate.ToString("dd/MM/yyyy")},
                //{"ReportTitle", "Profit and Loss Statement"}
            };

            Reports.Report_viewer reportViewer = new Reports.Report_viewer();
            reportViewer.ShowReport("Profit_and_Loss_Statement", dataSources);
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Profit_and_Loss_Statement_Load(object sender, EventArgs e)
        {

        }
    }

    // Simple Date Range Dialog Class
    public class SimpleDateRangeDialog : Form
    {
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }

        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;

        public SimpleDateRangeDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Select Date Range";
            this.Size = new System.Drawing.Size(300, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // From Date
            Label lblFrom = new Label();
            lblFrom.Text = "From Date:";
            lblFrom.Location = new System.Drawing.Point(20, 20);
            lblFrom.Size = new System.Drawing.Size(70, 20);

            dtpFrom = new DateTimePicker();
            dtpFrom.Format = DateTimePickerFormat.Short;
            dtpFrom.Location = new System.Drawing.Point(100, 20);
            dtpFrom.Size = new System.Drawing.Size(150, 20);
            dtpFrom.Value = DateTime.Now.AddMonths(-1);

            // To Date
            Label lblTo = new Label();
            lblTo.Text = "To Date:";
            lblTo.Location = new System.Drawing.Point(20, 60);
            lblTo.Size = new System.Drawing.Size(70, 20);

            dtpTo = new DateTimePicker();
            dtpTo.Format = DateTimePickerFormat.Short;
            dtpTo.Location = new System.Drawing.Point(100, 60);
            dtpTo.Size = new System.Drawing.Size(150, 20);
            dtpTo.Value = DateTime.Now;

            // Buttons
            Button btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new System.Drawing.Point(80, 100);
            btnOK.Size = new System.Drawing.Size(80, 30);
            btnOK.Click += (s, e) =>
            {
                FromDate = dtpFrom.Value;
                ToDate = dtpTo.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(170, 100);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            // Add controls
            this.Controls.Add(lblFrom);
            this.Controls.Add(dtpFrom);
            this.Controls.Add(lblTo);
            this.Controls.Add(dtpTo);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }
    }
}