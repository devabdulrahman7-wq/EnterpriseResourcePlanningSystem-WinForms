using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_Detailed_Ledger_Combine : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime fromDate;
        private DateTime toDate;

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
            if (string.IsNullOrEmpty(partyID))
            {
                MessageBox.Show("Please enter Party ID", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
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

            var parameters = new Dictionary<string, string>
            {
                //{"PartyID", partyidtxtbox.Text.Trim() },
                //{"PartyName", partynametxtbox.Text.Trim() },
                //{"Salesman", salesmancombobox.Text },
                //{"ReportTitle", detaillgrasdbtn.Checked ? "Detail Ledger" : "Detail Ledger Asad Umair"},
                //{"DateRange", GetDateRangeText() },
                //{"Orientation", portrtbtn.Checked ? "Portrait" : "Landscape"}
            };

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
                        cmd.Parameters.AddWithValue("@salesman", salesmancombobox.SelectedValue);

                        cmd.CommandTimeout = 180; // 3 minutes for large data

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching SP data: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return dt;
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
    }
}
