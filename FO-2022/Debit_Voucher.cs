//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Debit_Voucher : Form
//    {
//        public Debit_Voucher()
//        {
//            InitializeComponent();
//        }

//        private void vouchernotxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void voucherdatepcker_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void accountidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void accountidsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void amounttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void billnotxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totaltxtbox_Click(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void openbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void printbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {

//        }
//    }
//}




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

namespace FO_2022
{
    public partial class Debit_Voucher : Form
    {
        private string connectionString;
        private decimal totalAmount = 0;
        private DataTable voucherDataTable;

        public Debit_Voucher()
        {
            InitializeComponent();
            InitializeConnectionString();
            InitializeDataGridView();
            LoadNextVoucherNo();
        }

        private void InitializeConnectionString()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading connection string: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridView()
        {
            voucherDataTable = new DataTable();
            voucherDataTable.Columns.Add("SerialNo", typeof(int));
            voucherDataTable.Columns.Add("AccountNo", typeof(string));
            voucherDataTable.Columns.Add("AccountName", typeof(string));
            voucherDataTable.Columns.Add("Narration", typeof(string));
            voucherDataTable.Columns.Add("Amount", typeof(decimal));
            //voucherDataTable.Columns.Add("BillNo", typeof(string));
            voucherDataTable.Columns.Add("BillNo", typeof(string));


            dataGridView1.DataSource = voucherDataTable;

            // Set auto-increment for SerialNo
            voucherDataTable.Columns["SerialNo"].AutoIncrement = true;
            voucherDataTable.Columns["SerialNo"].AutoIncrementSeed = 1;
            voucherDataTable.Columns["SerialNo"].AutoIncrementStep = 1;

            // Format DataGridView columns
            dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "N2";
        }

        private void LoadNextVoucherNo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(VoucherNo), 0) + 1 AS NextVoucherNo FROM DebitVouchers";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            vouchernotxtbox.Text = result.ToString();
                        }
                    }
                }
                vouchernotxtbox.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading next voucher number: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void vouchernotxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void voucherdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Add any date validation or formatting if needed
        }

        private void accountidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        //private void accountidsearch_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(accountidtxtbox.Text))
        //        {
        //            MessageBox.Show("Please enter Account No to search!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            string query = @"SELECT AccountNo, AccountName 
        //                            FROM ChartOfAccounts 
        //                            WHERE AccountNo = @AccountNo";

        //            using (SqlCommand cmd = new SqlCommand(query, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text.Trim());

        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        accountnametxtbox.Text = reader["AccountName"].ToString();
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show("Account not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                        ClearAccountFields();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error searching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void accountidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No to search!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to search in ChartOfAccounts first
                    string accountQuery = @"SELECT AccountNo, AccountName 
                                  FROM ChartOfAccounts 
                                  WHERE AccountNo = @AccountNo";

                    using (SqlCommand accountCmd = new SqlCommand(accountQuery, conn))
                    {
                        accountCmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text.Trim());

                        using (SqlDataReader accountReader = accountCmd.ExecuteReader())
                        {
                            if (accountReader.Read())
                            {
                                accountnametxtbox.Text = accountReader["AccountName"].ToString();
                                accountReader.Close(); // Close the reader before next query

                                // Now search for sales data using this account ID
                                // We'll try to find sales where this account appears as CustomerId, SalesmanId, or DistributorPartyId
                                string salesQuery = @"SELECT TOP 1 BillNo, Narration, CustomerId, SalesmanId, DistributorPartyId
                                            FROM Sales 
                                            WHERE CustomerId = @AccountId 
                                               OR SalesmanId = @AccountId 
                                               OR DistributorPartyId = @AccountId
                                            ORDER BY SaleDate DESC";

                                using (SqlCommand salesCmd = new SqlCommand(salesQuery, conn))
                                {
                                    salesCmd.Parameters.AddWithValue("@AccountId", accountidtxtbox.Text.Trim());

                                    using (SqlDataReader salesReader = salesCmd.ExecuteReader())
                                    {
                                        if (salesReader.Read())
                                        {
                                            // Auto-fill BillNo if found
                                            if (salesReader["BillNo"] != DBNull.Value)
                                            {
                                                billnotxtbox.Text = salesReader["BillNo"].ToString();
                                            }

                                            // Auto-fill Narration if found
                                            if (salesReader["Narration"] != DBNull.Value)
                                            {
                                                narrationtxtbox.Text = salesReader["Narration"].ToString();
                                            }
                                        }
                                        // If no sales found, leave BillNo and Narration empty
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Account not found in Chart of Accounts!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ClearAccountFields();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void amounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void billnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void amounttxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, decimal point, and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void amounttxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddToDataGridView();
            }
        }

        private void AddToDataGridView()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(accountnametxtbox.Text))
                {
                    MessageBox.Show("Please search account first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(amounttxtbox.Text) || !decimal.TryParse(amounttxtbox.Text, out decimal amount))
                {
                    MessageBox.Show("Please enter valid amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Add new row to DataTable
                DataRow newRow = voucherDataTable.NewRow();
                newRow["AccountNo"] = accountidtxtbox.Text.Trim();
                newRow["AccountName"] = accountnametxtbox.Text.Trim();
                newRow["Narration"] = narrationtxtbox.Text.Trim();
                newRow["Amount"] = decimal.Parse(amounttxtbox.Text);
                newRow["BillNo"] = billnotxtbox.Text.Trim();

                voucherDataTable.Rows.Add(newRow);

                // Update total amount
                totalAmount += amount;
                totaltxtbox.Text = totalAmount.ToString("N2");

                // Clear input fields for next entry
                ClearInputFields();
                accountidtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding to grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void totaltxtbox_Click(object sender, EventArgs e)
        {
            // This is intentionally left blank as per requirement
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Add any specific cell click handling if needed
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (voucherDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No entries to save!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Begin transaction
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Save to DebitVouchers table
                            string voucherQuery = @"INSERT INTO DebitVouchers 
                                                  (VoucherNo, VoucherDate, LastUpdateDate, UserNo, OfficeID, IsUploaded, IsPosted, SalesmanID) 
                                                  VALUES 
                                                  (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, @OfficeID, @IsUploaded, @IsPosted, @SalesmanID)";

                            using (SqlCommand voucherCmd = new SqlCommand(voucherQuery, conn, transaction))
                            {
                                // You need to get these values from your application context or user input
                                voucherCmd.Parameters.AddWithValue("@VoucherNo", long.Parse(vouchernotxtbox.Text));
                                voucherCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value.Date);
                                voucherCmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                                voucherCmd.Parameters.AddWithValue("@UserNo", 1); // Replace with actual user
                                voucherCmd.Parameters.AddWithValue("@OfficeID", 1); // Replace with actual office
                                voucherCmd.Parameters.AddWithValue("@IsUploaded", false);
                                voucherCmd.Parameters.AddWithValue("@IsPosted", false);
                                voucherCmd.Parameters.AddWithValue("@SalesmanID", DBNull.Value); // Set if available

                                voucherCmd.ExecuteNonQuery();
                            }

                            // Save to DebitVouchersBody table
                            string bodyQuery = @"INSERT INTO DebitVouchersBody 
                                                (SerialNo, VoucherNo, AccountNo, Narration, Amount, BillNo) 
                                                VALUES 
                                                (@SerialNo, @VoucherNo, @AccountNo, @Narration, @Amount, @BillNo)";

                            foreach (DataRow row in voucherDataTable.Rows)
                            {
                                using (SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn, transaction))
                                {
                                    bodyCmd.Parameters.AddWithValue("@SerialNo", row["SerialNo"]);
                                    bodyCmd.Parameters.AddWithValue("@VoucherNo", long.Parse(vouchernotxtbox.Text));
                                    bodyCmd.Parameters.AddWithValue("@AccountNo", row["AccountNo"]);
                                    bodyCmd.Parameters.AddWithValue("@Narration", row["Narration"] ?? DBNull.Value);
                                    bodyCmd.Parameters.AddWithValue("@Amount", row["Amount"]);
                                    bodyCmd.Parameters.AddWithValue("@BillNo", row["BillNo"] ?? DBNull.Value);

                                    bodyCmd.ExecuteNonQuery();
                                }
                            }

                            // Commit transaction
                            transaction.Commit();

                            MessageBox.Show("Voucher saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Clear form and load next voucher number
                            ClearForm();
                            LoadNextVoucherNo();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ClearInputFields();
            voucherDataTable.Clear();
            totalAmount = 0;
            totaltxtbox.Text = "0.00";
            LoadNextVoucherNo();
            voucherdatepcker.Value = DateTime.Now;
        }

        private void ClearInputFields()
        {
            accountidtxtbox.Clear();
            accountnametxtbox.Clear();
            amounttxtbox.Clear();
            billnotxtbox.Clear();
            narrationtxtbox.Clear();
        }

        private void ClearAccountFields()
        {
            accountnametxtbox.Clear();
            amounttxtbox.Clear();
            billnotxtbox.Clear();
            narrationtxtbox.Clear();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Debit Voucher";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.TopMost= true;
                Label label = new Label();
                label.Text = "Enter Voucher No to open:";
                label.SetBounds(10, 20, 280, 20);

                TextBox textBox = new TextBox();
                textBox.SetBounds(10, 45, 260, 20);

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.SetBounds(60, 80, 75, 25);

                Button cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.SetBounds(140, 80, 75, 25);

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(okButton);
                inputForm.Controls.Add(cancelButton);
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string input = textBox.Text;

                    if (!string.IsNullOrEmpty(input))
                    {
                        if (long.TryParse(input, out long voucherNo))
                        {
                            LoadDebitVoucher(voucherNo);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid Voucher No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening debit voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDebitVoucher(long voucherNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load voucher header
                    string headerQuery = @"SELECT * FROM DebitVouchers WHERE VoucherNo = @VoucherNo";
                    using (SqlCommand headerCmd = new SqlCommand(headerQuery, conn))
                    {
                        headerCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                        using (SqlDataReader reader = headerCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                vouchernotxtbox.Text = reader["VoucherNo"].ToString();
                                vouchernotxtbox.Enabled = false;

                                if (reader["VoucherDate"] != DBNull.Value)
                                {
                                    voucherdatepcker.Value = Convert.ToDateTime(reader["VoucherDate"]);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Voucher not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // Load voucher body
                    voucherDataTable.Clear();
                    totalAmount = 0;

                    string bodyQuery = @"SELECT dvb.*, coa.AccountName 
                                       FROM DebitVouchersBody dvb
                                       LEFT JOIN ChartOfAccounts coa ON dvb.AccountNo = coa.AccountNo
                                       WHERE dvb.VoucherNo = @VoucherNo
                                       ORDER BY dvb.SerialNo";

                    using (SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn))
                    {
                        bodyCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                        using (SqlDataReader reader = bodyCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow newRow = voucherDataTable.NewRow();
                                newRow["SerialNo"] = reader["SerialNo"];
                                newRow["AccountNo"] = reader["AccountNo"].ToString();
                                newRow["AccountName"] = reader["AccountName"].ToString();
                                newRow["Narration"] = reader["Narration"].ToString();
                                newRow["Amount"] = reader["Amount"];
                                newRow["BillNo"] = reader["BillNo"].ToString();

                                voucherDataTable.Rows.Add(newRow);

                                // Update total amount
                                totalAmount += Convert.ToDecimal(reader["Amount"]);
                            }
                        }
                    }

                    totaltxtbox.Text = totalAmount.ToString("N2");
                    MessageBox.Show("Voucher loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (row.DataBoundItem != null)
                        {
                            decimal amount = Convert.ToDecimal(row.Cells["Amount"].Value);
                            totalAmount -= amount;
                            totaltxtbox.Text = totalAmount.ToString("N2");

                            voucherDataTable.Rows.RemoveAt(row.Index);
                        }
                    }
                }
                else if (dataGridView1.CurrentRow != null)
                {
                    decimal amount = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["Amount"].Value);
                    totalAmount -= amount;
                    totaltxtbox.Text = totalAmount.ToString("N2");

                    voucherDataTable.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                }
                else
                {
                    MessageBox.Show("Please select a row to remove!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing row: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (voucherDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No data to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Create print preview dialog
                PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
                printPreviewDialog.Text = "Print Debit Voucher";
                printPreviewDialog.WindowState = FormWindowState.Maximized;
                printPreviewDialog.PrintPreviewControl.Zoom = 1.0;

                // Create print document
                System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();
                printDoc.DocumentName = "Debit Voucher - " + vouchernotxtbox.Text;

                printDoc.PrintPage += (s, ev) =>
                {
                    // Define fonts
                    Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                    Font headerFont = new Font("Arial", 10, FontStyle.Bold);
                    Font detailFont = new Font("Arial", 9);

                    // Define starting point
                    float startX = 50;
                    float startY = 50;
                    float lineHeight = 20;

                    // Print title
                    ev.Graphics.DrawString("DEBIT VOUCHER", titleFont, Brushes.Black, startX, startY);
                    startY += lineHeight * 2;

                    // Print voucher details
                    ev.Graphics.DrawString("Voucher No: " + vouchernotxtbox.Text, detailFont, Brushes.Black, startX, startY);
                    ev.Graphics.DrawString("Date: " + voucherdatepcker.Value.ToShortDateString(), detailFont, Brushes.Black, startX + 200, startY);
                    startY += lineHeight;

                    // Draw line separator
                    ev.Graphics.DrawLine(Pens.Black, startX, startY, startX + 700, startY);
                    startY += lineHeight;

                    // Print table headers
                    float[] columnWidths = { 50, 100, 200, 150, 100, 100 };
                    string[] headers = { "S.No", "Account No", "Account Name", "Narration", "Amount", "Bill No" };

                    for (int i = 0; i < headers.Length; i++)
                    {
                        ev.Graphics.DrawString(headers[i], headerFont, Brushes.Black, startX, startY);
                        startX += columnWidths[i];
                    }

                    startX = 50;
                    startY += lineHeight;
                    ev.Graphics.DrawLine(Pens.Black, startX, startY, startX + 700, startY);
                    startY += 5;

                    // Print voucher details
                    foreach (DataRow row in voucherDataTable.Rows)
                    {
                        startX = 50;

                        ev.Graphics.DrawString(row["SerialNo"].ToString(), detailFont, Brushes.Black, startX, startY);
                        startX += columnWidths[0];

                        ev.Graphics.DrawString(row["AccountNo"].ToString(), detailFont, Brushes.Black, startX, startY);
                        startX += columnWidths[1];

                        ev.Graphics.DrawString(row["AccountName"].ToString(), detailFont, Brushes.Black, startX, startY);
                        startX += columnWidths[2];

                        ev.Graphics.DrawString(row["Narration"].ToString(), detailFont, Brushes.Black, startX, startY);
                        startX += columnWidths[3];

                        ev.Graphics.DrawString(Convert.ToDecimal(row["Amount"]).ToString("N2"), detailFont, Brushes.Black, startX, startY);
                        startX += columnWidths[4];

                        ev.Graphics.DrawString(row["BillNo"].ToString(), detailFont, Brushes.Black, startX, startY);

                        startY += lineHeight;
                        startX = 50;

                        // Check if we need a new page
                        if (startY > ev.MarginBounds.Height - 100)
                        {
                            ev.HasMorePages = true;
                            startY = 50;
                            return;
                        }
                    }

                    // Draw total line
                    startY += lineHeight;
                    ev.Graphics.DrawLine(Pens.Black, startX, startY, startX + 700, startY);
                    startY += lineHeight;

                    // Print total
                    ev.Graphics.DrawString("Total Amount:", headerFont, Brushes.Black, startX + 400, startY);
                    ev.Graphics.DrawString(totaltxtbox.Text, headerFont, Brushes.Black, startX + 550, startY);
                };

                printPreviewDialog.Document = printDoc;
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}