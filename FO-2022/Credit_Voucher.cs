////using System;
////using System.Collections.Generic;
////using System.ComponentModel;
////using System.Data;
////using System.Drawing;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using System.Windows.Forms;

////namespace FO_2022
////{
////    public partial class totalbtn : Form
////    {
////        public totalbtn()
////        {
////            InitializeComponent();
////        }

////        private void closebtn_Click(object sender, EventArgs e)
////        {
////            Mainform1 mf = new Mainform1();
////            mf.Show();
////            this.Hide();
////        }

////        private void vouchernotxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void voucherdatepcker_ValueChanged(object sender, EventArgs e)
////        {

////        }

////        private void accountidtxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void accountidsearch_Click(object sender, EventArgs e)
////        {

////        }

////        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void amounttxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void billnotxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {

////        }

////        private void totaltxtbox_Click(object sender, EventArgs e)
////        {

////        }

////        private void savebtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void clearbtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void openbtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void removebtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void printbtn_Click(object sender, EventArgs e)
////        {

////        }
////    }
////}





//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Data.SqlClient;
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Credit_Voucher : Form
//    {
//        private SqlConnection connection;
//        private DataTable voucherData;
//        private decimal totalAmount = 0;
//        private bool isNewVoucher = true;
//        private int currentVoucherNo = 0;

//        public Credit_Voucher()
//        {
//            InitializeComponent();
//            InitializeConnection();
//            InitializeVoucher();
//        }

//        private void InitializeConnection()
//        {
//            try
//            {
//                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//                connection = new SqlConnection(connectionString);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void InitializeVoucher()
//        {
//            try
//            {
//                connection.Open();

//                // Get the last voucher number and increment by 1
//                string query = "SELECT MAX(VoucherNo) FROM CreditVouchers";
//                SqlCommand cmd = new SqlCommand(query, connection);
//                object result = cmd.ExecuteScalar();

//                if (result != null && result != DBNull.Value)
//                {
//                    currentVoucherNo = Convert.ToInt32(result) + 1;
//                }
//                else
//                {
//                    currentVoucherNo = 1;
//                }

//                vouchernotxtbox.Text = currentVoucherNo.ToString();
//                vouchernotxtbox.Enabled = false;

//                // Set current date
//                voucherdatepcker.Value = DateTime.Now;

//                // Initialize DataGridView
//                InitializeDataGridView();

//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error initializing voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void InitializeDataGridView()
//        {
//            voucherData = new DataTable();
//            voucherData.Columns.Add("AccountNo", typeof(string));
//            voucherData.Columns.Add("AccountName", typeof(string));
//            voucherData.Columns.Add("Amount", typeof(decimal));
//            voucherData.Columns.Add("BillNo", typeof(string));
//            voucherData.Columns.Add("Narration", typeof(string));

//            dataGridView1.DataSource = voucherData;
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void accountidsearch_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(accountidtxtbox.Text))
//                {
//                    MessageBox.Show("Please enter Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                connection.Open();

//                string query = "SELECT AccountName, AccountNo FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text);

//                SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    accountnametxtbox.Text = reader["AccountName"].ToString();
//                }
//                else
//                {
//                    MessageBox.Show("Account not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    accountnametxtbox.Clear();
//                }

//                reader.Close();
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error fetching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void amounttxtbox_KeyPress(object sender, KeyPressEventArgs e)
//        {
//            // Allow numbers, decimal point, and backspace
//            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
//            {
//                e.Handled = true;
//            }

//            // Only allow one decimal point
//            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
//            {
//                e.Handled = true;
//            }

//            // If Enter key is pressed, add to DataGridView
//            if (e.KeyChar == (char)Keys.Enter)
//            {
//                AddToDataGridView();
//                e.Handled = true;
//            }
//        }

//        private void AddToDataGridView()
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(accountidtxtbox.Text) || string.IsNullOrEmpty(accountnametxtbox.Text) || string.IsNullOrEmpty(amounttxtbox.Text))
//                {
//                    MessageBox.Show("Please fill all required fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                decimal amount;
//                if (!decimal.TryParse(amounttxtbox.Text, out amount))
//                {
//                    MessageBox.Show("Please enter a valid amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                // Add row to DataTable
//                DataRow newRow = voucherData.NewRow();
//                newRow["AccountNo"] = accountidtxtbox.Text;
//                newRow["AccountName"] = accountnametxtbox.Text;
//                newRow["Amount"] = amount;
//                newRow["BillNo"] = billnotxtbox.Text;
//                newRow["Narration"] = narrationtxtbox.Text;
//                voucherData.Rows.Add(newRow);

//                // Update total
//                totalAmount += amount;
//                totaltxtbox.Text = totalAmount.ToString("N2");

//                // Clear input fields
//                accountidtxtbox.Clear();
//                accountnametxtbox.Clear();
//                amounttxtbox.Clear();
//                billnotxtbox.Clear();
//                narrationtxtbox.Clear();

//                // Focus back to account number field
//                accountidtxtbox.Focus();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error adding row: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (voucherData.Rows.Count == 0)
//                {
//                    MessageBox.Show("Please add at least one entry!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                connection.Open();
//                SqlTransaction transaction = connection.BeginTransaction();

//                try
//                {
//                    // Insert into CreditVouchers (header)
//                    string headerQuery = @"INSERT INTO CreditVouchers 
//                                        (VoucherNo, VoucherDate, LastUpdateDate, UserNo, OfficeID, IsUploaded, IsPosted) 
//                                        VALUES 
//                                        (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, @OfficeID, @IsUploaded, @IsPosted)";

//                    SqlCommand headerCmd = new SqlCommand(headerQuery, connection, transaction);
//                    headerCmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
//                    headerCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
//                    headerCmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
//                    headerCmd.Parameters.AddWithValue("@UserNo", 1); // Assuming user 1
//                    headerCmd.Parameters.AddWithValue("@OfficeID", DBNull.Value);
//                    headerCmd.Parameters.AddWithValue("@IsUploaded", 0);
//                    headerCmd.Parameters.AddWithValue("@IsPosted", 0);
//                    headerCmd.ExecuteNonQuery();

//                    // Insert into CreditVouchersBody (details)
//                    string detailQuery = @"INSERT INTO CreditVouchersBody 
//                                         (VoucherNo, AccountNo, Narration, Amount, BillNo) 
//                                         VALUES 
//                                         (@VoucherNo, @AccountNo, @Narration, @Amount, @BillNo)";

//                    foreach (DataRow row in voucherData.Rows)
//                    {
//                        SqlCommand detailCmd = new SqlCommand(detailQuery, connection, transaction);
//                        detailCmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
//                        detailCmd.Parameters.AddWithValue("@AccountNo", row["AccountNo"].ToString());
//                        detailCmd.Parameters.AddWithValue("@Narration", row["Narration"].ToString());
//                        detailCmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(row["Amount"]));
//                        detailCmd.Parameters.AddWithValue("@BillNo", row["BillNo"].ToString());
//                        detailCmd.ExecuteNonQuery();
//                    }

//                    transaction.Commit();
//                    MessageBox.Show("Voucher saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                    // Reset form for new voucher
//                    InitializeVoucher();
//                    voucherData.Clear();
//                    totalAmount = 0;
//                    totaltxtbox.Text = "0.00";
//                }
//                catch (Exception ex)
//                {
//                    transaction.Rollback();
//                    throw ex;
//                }
//                finally
//                {
//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            // Clear all input fields
//            accountidtxtbox.Clear();
//            accountnametxtbox.Clear();
//            amounttxtbox.Clear();
//            billnotxtbox.Clear();
//            narrationtxtbox.Clear();

//            // Clear DataGridView
//            voucherData.Clear();

//            // Reset total
//            totalAmount = 0;
//            totaltxtbox.Text = "0.00";

//            // Focus on account number field
//            accountidtxtbox.Focus();
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count > 0)
//            {
//                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
//                {
//                    // Subtract amount from total
//                    decimal rowAmount = Convert.ToDecimal(row.Cells["Amount"].Value);
//                    totalAmount -= rowAmount;

//                    // Remove from DataTable
//                    voucherData.Rows.RemoveAt(row.Index);
//                }

//                totaltxtbox.Text = totalAmount.ToString("N2");
//            }
//            else
//            {
//                MessageBox.Show("Please select a row to remove!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
//        }

//        private void openbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Create custom input dialog
//                Form inputForm = new Form();
//                inputForm.Text = "Open Credit Voucher";
//                inputForm.Width = 300;
//                inputForm.Height = 150;
//                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
//                inputForm.MaximizeBox = false;
//                inputForm.MinimizeBox = false;
//                inputForm.StartPosition = FormStartPosition.CenterParent;

//                Label label = new Label();
//                label.Text = "Enter Voucher No to open:";
//                label.SetBounds(10, 20, 280, 20);

//                TextBox textBox = new TextBox();
//                textBox.SetBounds(10, 45, 260, 20);

//                Button okButton = new Button();
//                okButton.Text = "OK";
//                okButton.DialogResult = DialogResult.OK;
//                okButton.SetBounds(60, 80, 75, 25);

//                Button cancelButton = new Button();
//                cancelButton.Text = "Cancel";
//                cancelButton.DialogResult = DialogResult.Cancel;
//                cancelButton.SetBounds(140, 80, 75, 25);

//                inputForm.Controls.Add(label);
//                inputForm.Controls.Add(textBox);
//                inputForm.Controls.Add(okButton);
//                inputForm.Controls.Add(cancelButton);
//                inputForm.AcceptButton = okButton;
//                inputForm.CancelButton = cancelButton;

//                if (inputForm.ShowDialog() == DialogResult.OK)
//                {
//                    string input = textBox.Text;

//                    if (!string.IsNullOrEmpty(input))
//                    {
//                        if (int.TryParse(input, out int voucherNo))
//                        {
//                            LoadVoucher(voucherNo);
//                        }
//                        else
//                        {
//                            MessageBox.Show("Please enter a valid numeric Voucher No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error opening voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadVoucher(int voucherNo)
//        {
//            try
//            {
//                connection.Open();

//                // Check if voucher exists
//                string checkQuery = "SELECT COUNT(*) FROM CreditVouchers WHERE VoucherNo = @VoucherNo";
//                SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
//                checkCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
//                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

//                if (count == 0)
//                {
//                    MessageBox.Show("Voucher not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    connection.Close();
//                    return;
//                }

//                // Load voucher header
//                string headerQuery = "SELECT * FROM CreditVouchers WHERE VoucherNo = @VoucherNo";
//                SqlCommand headerCmd = new SqlCommand(headerQuery, connection);
//                headerCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

//                SqlDataReader headerReader = headerCmd.ExecuteReader();
//                if (headerReader.Read())
//                {
//                    vouchernotxtbox.Text = voucherNo.ToString();
//                    voucherdatepcker.Value = Convert.ToDateTime(headerReader["VoucherDate"]);
//                }
//                headerReader.Close();

//                // Clear existing data
//                voucherData.Clear();
//                totalAmount = 0;

//                // Load voucher details
//                string detailQuery = @"SELECT cvb.*, coa.AccountName 
//                                     FROM CreditVouchersBody cvb 
//                                     INNER JOIN ChartOfAccounts coa ON cvb.AccountNo = coa.AccountNo 
//                                     WHERE cvb.VoucherNo = @VoucherNo";

//                SqlCommand detailCmd = new SqlCommand(detailQuery, connection);
//                detailCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

//                SqlDataReader detailReader = detailCmd.ExecuteReader();
//                while (detailReader.Read())
//                {
//                    DataRow newRow = voucherData.NewRow();
//                    newRow["AccountNo"] = detailReader["AccountNo"].ToString();
//                    newRow["AccountName"] = detailReader["AccountName"].ToString();
//                    newRow["Amount"] = Convert.ToDecimal(detailReader["Amount"]);
//                    newRow["BillNo"] = detailReader["BillNo"].ToString();
//                    newRow["Narration"] = detailReader["Narration"].ToString();
//                    voucherData.Rows.Add(newRow);

//                    totalAmount += Convert.ToDecimal(detailReader["Amount"]);
//                }
//                detailReader.Close();

//                totaltxtbox.Text = totalAmount.ToString("N2");
//                isNewVoucher = false;
//                currentVoucherNo = voucherNo;

//                MessageBox.Show("Voucher loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void printbtn_Click(object sender, EventArgs e)
//        {
//            if (voucherData.Rows.Count == 0)
//            {
//                MessageBox.Show("No data to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            // Create print dialog
//            PrintDialog printDialog = new PrintDialog();

//            if (printDialog.ShowDialog() == DialogResult.OK)
//            {
//                // Here you would implement printing logic
//                // For now, just show a message
//                MessageBox.Show("Printing feature will be implemented here.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void totaltxtbox_Click(object sender, EventArgs e)
//        {
//            // This button shows the total, no action needed
//        }

//        // TextChanged events (keep them empty or add validation if needed)
//        private void vouchernotxtbox_TextChanged(object sender, EventArgs e) { }
//        private void voucherdatepcker_ValueChanged(object sender, EventArgs e) { }
//        private void accountidtxtbox_TextChanged(object sender, EventArgs e) { }
//        private void accountnametxtbox_TextChanged(object sender, EventArgs e) { }
//        private void billnotxtbox_TextChanged(object sender, EventArgs e) { }
//        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
//    }
//}







using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace FO_2022
{
    public partial class Credit_Voucher : Form
    {
        private SqlConnection connection;
        private DataTable voucherData;
        private decimal totalAmount = 0;
        private bool isNewVoucher = true;
        private int currentVoucherNo = 0;
        private DataTable billsDataTable; // For storing available bills

        public Credit_Voucher()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeVoucher();
        }

        private void InitializeConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeVoucher()
        {
            try
            {
                connection.Open();

                // Get the last voucher number and increment by 1
                string query = "SELECT MAX(VoucherNo) FROM CreditVouchers";
                SqlCommand cmd = new SqlCommand(query, connection);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    currentVoucherNo = Convert.ToInt32(result) + 1;
                }
                else
                {
                    currentVoucherNo = 1;
                }

                vouchernotxtbox.Text = currentVoucherNo.ToString();
                vouchernotxtbox.Enabled = false;

                // Set current date
                voucherdatepcker.Value = DateTime.Now;

                // Initialize DataGridView
                InitializeDataGridView();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void InitializeDataGridView()
        {
            voucherData = new DataTable();
            voucherData.Columns.Add("AccountNo", typeof(string));
            voucherData.Columns.Add("AccountName", typeof(string));
            voucherData.Columns.Add("Amount", typeof(decimal));
            voucherData.Columns.Add("BillNo", typeof(string));
            voucherData.Columns.Add("Narration", typeof(string));
            //voucherData.Columns.Add("CustomerName", typeof(string)); // Added customer name

            dataGridView1.DataSource = voucherData;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void accountidsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(accountidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                connection.Open();

                // 1. Get Account Name from ChartOfAccounts
                string accountQuery = @"
                    SELECT ca.AccountName, p.PartyName 
                    FROM ChartOfAccounts ca 
                    LEFT JOIN Parties p ON ca.AccountNo = p.PartyID 
                    WHERE ca.AccountNo = @AccountNo";

                SqlCommand accountCmd = new SqlCommand(accountQuery, connection);
                accountCmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text);

                SqlDataReader accountReader = accountCmd.ExecuteReader();

                if (accountReader.Read())
                {
                    accountnametxtbox.Text = accountReader["AccountName"].ToString();

                    // Get customer name if available
                    string customerName = accountReader["PartyName"] != DBNull.Value ?
                                          accountReader["PartyName"].ToString() :
                                          accountReader["AccountName"].ToString();

                    // Show customer name in a label or textbox
                    //lblCustomerName.Text = "Customer: " + customerName;
                    //lblCustomerName.Visible = true;
                }
                else
                {
                    MessageBox.Show("Account not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    accountnametxtbox.Clear();
                    //lblCustomerName.Visible = false;
                    connection.Close();
                    return;
                }
                accountReader.Close();

                // 2. Fetch available bills for this account
                FetchAvailableBills(accountidtxtbox.Text);

                // 3. If there are bills, auto-fill the latest one
                if (billsDataTable != null && billsDataTable.Rows.Count > 0)
                {
                    DataRow latestBill = billsDataTable.Rows[0];
                    billnotxtbox.Text = latestBill["BillNo"].ToString();

                    // Auto-fill narration from the bill
                    if (latestBill["InvoiceNotes"] != DBNull.Value &&
                        !string.IsNullOrEmpty(latestBill["InvoiceNotes"].ToString()))
                    {
                        narrationtxtbox.Text = latestBill["InvoiceNotes"].ToString();
                    }
                    else if (latestBill["Narration"] != DBNull.Value &&
                             !string.IsNullOrEmpty(latestBill["Narration"].ToString()))
                    {
                        narrationtxtbox.Text = latestBill["Narration"].ToString();
                    }

                    // Optionally show amount from bill
                    if (latestBill["ReceivedAmount"] != DBNull.Value)
                    {
                        amounttxtbox.Text = Convert.ToDecimal(latestBill["ReceivedAmount"]).ToString("N2");
                    }
                }
                else
                {
                    // No bills found
                    billnotxtbox.Clear();
                    MessageBox.Show("No sales bills found for this account.", "Information",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching account: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }


        private void FetchAvailableBills(string accountNo)
        {
            try
            {
                // First, let's debug what account we're actually looking for
                Console.WriteLine($"Looking for bills for AccountNo: {accountNo}");

                string billsQuery = @"
            SELECT 
                SaleId,
                BillNo,
                SaleDate,
                ReceivedAmount,
                ISNULL(InvoiceNotes, '') as InvoiceNotes,
                ISNULL(Narration, '') as Narration,
                CustomerName,
                CustomerId,  -- Add this to see the actual CustomerId
                DistributorPartyId  -- And this if it exists
            FROM Sales 
            WHERE (CustomerId = @AccountNo OR DistributorPartyId = @AccountNo)
            AND BillNo IS NOT NULL 
            AND BillNo <> ''
            ORDER BY SaleDate DESC";

                SqlCommand cmd = new SqlCommand(billsQuery, connection);
                cmd.Parameters.AddWithValue("@AccountNo", accountNo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                billsDataTable = new DataTable();
                adapter.Fill(billsDataTable);

                // Debug output
                Console.WriteLine($"Found {billsDataTable.Rows.Count} bills");
                foreach (DataRow row in billsDataTable.Rows)
                {
                    Console.WriteLine($"Bill: {row["BillNo"]}, CustomerId: {row["CustomerId"]}, CustomerName: {row["CustomerName"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching bills: " + ex.Message);
                billsDataTable = new DataTable();
            }
        }

        private void showBillsbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(accountidtxtbox.Text))
            {
                MessageBox.Show("Please enter Account No first!", "Warning",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                connection.Open();
                FetchAvailableBills(accountidtxtbox.Text);

                if (billsDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No bills found for this account.", "Information",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    connection.Close();
                    return;
                }

                // Show bills in a form for selection
                Form billsForm = new Form();
                billsForm.Text = "Available Bills for Account: " + accountidtxtbox.Text;
                billsForm.Width = 800;
                billsForm.Height = 400;
                billsForm.StartPosition = FormStartPosition.CenterParent;

                DataGridView dgv = new DataGridView();
                dgv.DataSource = billsDataTable;
                dgv.Dock = DockStyle.Fill;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.AutoResizeColumns();

                // Add double-click event
                dgv.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0)
                    {
                        DataRow selectedRow = billsDataTable.Rows[ev.RowIndex];
                        billnotxtbox.Text = selectedRow["BillNo"].ToString();

                        // Fill narration from InvoiceNotes or Narration
                        string narration = "";
                        if (!string.IsNullOrEmpty(selectedRow["InvoiceNotes"].ToString()))
                            narration = selectedRow["InvoiceNotes"].ToString();
                        else if (!string.IsNullOrEmpty(selectedRow["Narration"].ToString()))
                            narration = selectedRow["Narration"].ToString();

                        narrationtxtbox.Text = narration;

                        // Fill amount
                        amounttxtbox.Text = Convert.ToDecimal(selectedRow["ReceivedAmount"]).ToString("N2");

                        billsForm.Close();
                    }
                };

                Button selectButton = new Button();
                selectButton.Text = "Select Bill";
                selectButton.Dock = DockStyle.Bottom;
                selectButton.Height = 40;
                selectButton.Font = new Font("Arial", 10, FontStyle.Bold);

                selectButton.Click += (s, ev) =>
                {
                    if (dgv.SelectedRows.Count > 0)
                    {
                        DataRowView selectedRow = (DataRowView)dgv.SelectedRows[0].DataBoundItem;
                        billnotxtbox.Text = selectedRow["BillNo"].ToString();

                        string narration = "";
                        if (!string.IsNullOrEmpty(selectedRow["InvoiceNotes"].ToString()))
                            narration = selectedRow["InvoiceNotes"].ToString();
                        else if (!string.IsNullOrEmpty(selectedRow["Narration"].ToString()))
                            narration = selectedRow["Narration"].ToString();

                        narrationtxtbox.Text = narration;
                        amounttxtbox.Text = Convert.ToDecimal(selectedRow["ReceivedAmount"]).ToString("N2");
                        billsForm.Close();
                    }
                };

                Button cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.Dock = DockStyle.Bottom;
                cancelButton.Height = 40;

                Panel buttonPanel = new Panel();
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 80;
                buttonPanel.Controls.Add(selectButton);
                buttonPanel.Controls.Add(cancelButton);

                selectButton.Left = (buttonPanel.Width - selectButton.Width) / 2 - 60;
                selectButton.Top = 20;
                cancelButton.Left = (buttonPanel.Width - cancelButton.Width) / 2 + 60;
                cancelButton.Top = 20;

                billsForm.Controls.Add(dgv);
                billsForm.Controls.Add(buttonPanel);
                billsForm.ShowDialog();

                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error showing bills: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void searchBillbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(accountidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No first!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(billnotxtbox.Text))
                {
                    MessageBox.Show("Please enter Bill No to search!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                connection.Open();

                string query = @"
                    SELECT 
                        BillNo, 
                        SaleDate, 
                        SaleId, 
                        ReceivedAmount,
                        ISNULL(InvoiceNotes, '') as InvoiceNotes,
                        ISNULL(Narration, '') as Narration,
                        CustomerName
                    FROM Sales 
                    WHERE CustomerId = @AccountNo 
                    AND BillNo = @BillNo";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text);
                cmd.Parameters.AddWithValue("@BillNo", billnotxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Fill the narration from the bill
                    string narration = "";
                    if (!string.IsNullOrEmpty(reader["InvoiceNotes"].ToString()))
                        narration = reader["InvoiceNotes"].ToString();
                    else if (!string.IsNullOrEmpty(reader["Narration"].ToString()))
                        narration = reader["Narration"].ToString();

                    narrationtxtbox.Text = narration;

                    // Optionally fill amount
                    amounttxtbox.Text = Convert.ToDecimal(reader["ReceivedAmount"]).ToString("N2");

                    MessageBox.Show($"Bill found!\nDate: {reader["SaleDate"]:dd/MM/yyyy}\nAmount: {reader["ReceivedAmount"]:N2}",
                                  "Bill Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Bill not found for this account!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching bill: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void amounttxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow numbers, decimal point, and backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            // If Enter key is pressed, add to DataGridView
            if (e.KeyChar == (char)Keys.Enter)
            {
                AddToDataGridView();
                e.Handled = true;
            }
        }

        private void AddToDataGridView()
        {
            try
            {
                if (string.IsNullOrEmpty(accountidtxtbox.Text) ||
                    string.IsNullOrEmpty(accountnametxtbox.Text) ||
                    string.IsNullOrEmpty(amounttxtbox.Text))
                {
                    MessageBox.Show("Please fill all required fields!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal amount;
                if (!decimal.TryParse(amounttxtbox.Text, out amount))
                {
                    MessageBox.Show("Please enter a valid amount!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get customer name from label
                //string customerName = lblCustomerName.Text.Replace("Customer: ", "");

                // Add row to DataTable
                DataRow newRow = voucherData.NewRow();
                newRow["AccountNo"] = accountidtxtbox.Text;
                newRow["AccountName"] = accountnametxtbox.Text;
                newRow["Amount"] = amount;
                newRow["BillNo"] = billnotxtbox.Text;
                newRow["Narration"] = narrationtxtbox.Text;
                //newRow["CustomerName"] = customerName;
                voucherData.Rows.Add(newRow);

                // Update total
                totalAmount += amount;
                totaltxtbox.Text = totalAmount.ToString("N2");

                // Clear input fields but keep account info
                amounttxtbox.Clear();
                billnotxtbox.Clear();
                narrationtxtbox.Clear();

                // Focus back to amount field for next entry
                amounttxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding row: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (voucherData.Rows.Count == 0)
                {
                    MessageBox.Show("Please add at least one entry!", "Warning",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert into CreditVouchers (header) - CORRECT
                    string headerQuery = @"INSERT INTO CreditVouchers 
                (VoucherNo, VoucherDate, LastUpdateDate, UserNo, OfficeID, IsUploaded, IsPosted) 
                VALUES 
                (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, @OfficeID, @IsUploaded, @IsPosted)";

                    SqlCommand headerCmd = new SqlCommand(headerQuery, connection, transaction);
                    headerCmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
                    headerCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                    headerCmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                    headerCmd.Parameters.AddWithValue("@UserNo", 1);
                    headerCmd.Parameters.AddWithValue("@OfficeID", 1); // Fixed OfficeID
                    headerCmd.Parameters.AddWithValue("@IsUploaded", 0);
                    headerCmd.Parameters.AddWithValue("@IsPosted", 0);
                    headerCmd.ExecuteNonQuery();

                    // CORRECTED: Insert into CreditVouchersBody with SerialNo column
                    string detailQuery = @"INSERT INTO CreditVouchersBody 
                 (SerialNo, VoucherNo, AccountNo, Narration, Amount, BillNo) 
                 VALUES 
                 (@SerialNo, @VoucherNo, @AccountNo, @Narration, @Amount, @BillNo)";

                    int serialNo = 1; // Start serial numbers from 1

                    foreach (DataRow row in voucherData.Rows)
                    {
                        SqlCommand detailCmd = new SqlCommand(detailQuery, connection, transaction);
                        detailCmd.Parameters.AddWithValue("@SerialNo", serialNo);
                        detailCmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);

                        // Convert AccountNo from string to int (since table expects int)
                        if (!int.TryParse(row["AccountNo"].ToString(), out int accountNo))
                        {
                            throw new Exception($"Invalid AccountNo: {row["AccountNo"]}. Must be a number.");
                        }
                        detailCmd.Parameters.AddWithValue("@AccountNo", accountNo);

                        detailCmd.Parameters.AddWithValue("@Narration", row["Narration"].ToString());
                        detailCmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(row["Amount"]));

                        // Handle BillNo - if empty, insert DBNull
                        if (string.IsNullOrEmpty(row["BillNo"].ToString()))
                            detailCmd.Parameters.AddWithValue("@BillNo", DBNull.Value);
                        else
                            detailCmd.Parameters.AddWithValue("@BillNo", row["BillNo"].ToString());

                        detailCmd.ExecuteNonQuery();
                        serialNo++; // Increment for next row
                    }

                    transaction.Commit();
                    MessageBox.Show("Voucher saved successfully!\nVoucher No: " + currentVoucherNo,
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // DON'T CALL InitializeVoucher() here
                    // Instead, just get the next voucher number
                    currentVoucherNo++;
                    vouchernotxtbox.Text = currentVoucherNo.ToString();

                    // Reset form for new voucher
                    voucherData.Clear();
                    totalAmount = 0;
                    totaltxtbox.Text = "0.00";
                    accountidtxtbox.Clear();
                    accountnametxtbox.Clear();
                    voucherdatepcker.Value = DateTime.Now;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving voucher: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void clearbtn_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            accountidtxtbox.Clear();
            accountnametxtbox.Clear();
            amounttxtbox.Clear();
            billnotxtbox.Clear();
            narrationtxtbox.Clear();
            //lblCustomerName.Visible = false;

            // Clear DataGridView
            voucherData.Clear();

            // Reset total
            totalAmount = 0;
            totaltxtbox.Text = "0.00";

            // Focus on account number field
            accountidtxtbox.Focus();
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    // Subtract amount from total
                    decimal rowAmount = Convert.ToDecimal(row.Cells["Amount"].Value);
                    totalAmount -= rowAmount;

                    // Remove from DataTable
                    voucherData.Rows.RemoveAt(row.Index);
                }

                totaltxtbox.Text = totalAmount.ToString("N2");
            }
            else
            {
                MessageBox.Show("Please select a row to remove!", "Warning",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Rest of your existing methods (openbtn, LoadVoucher, printbtn, etc.) remain the same
        // ... [keep all your existing methods for open, print, etc.]

        private void Credit_Voucher_Load(object sender, EventArgs e)
        {
            // Initialize customer name label
            //lblCustomerName.Visible = false;
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Credit Voucher";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;

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
                        if (int.TryParse(input, out int voucherNo))
                        {
                            LoadVoucher(voucherNo);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid numeric Voucher No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVoucher(int voucherNo)
        {
            try
            {
                connection.Open();

                // Check if voucher exists
                string checkQuery = "SELECT COUNT(*) FROM CreditVouchers WHERE VoucherNo = @VoucherNo";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    MessageBox.Show("Voucher not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    connection.Close();
                    return;
                }

                // Load voucher header
                string headerQuery = "SELECT * FROM CreditVouchers WHERE VoucherNo = @VoucherNo";
                SqlCommand headerCmd = new SqlCommand(headerQuery, connection);
                headerCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                SqlDataReader headerReader = headerCmd.ExecuteReader();
                if (headerReader.Read())
                {
                    vouchernotxtbox.Text = voucherNo.ToString();
                    voucherdatepcker.Value = Convert.ToDateTime(headerReader["VoucherDate"]);
                }
                headerReader.Close();

                // Clear existing data
                voucherData.Clear();
                totalAmount = 0;

                // Load voucher details
                string detailQuery = @"SELECT cvb.*, coa.AccountName 
                                                 FROM CreditVouchersBody cvb 
                                                 INNER JOIN ChartOfAccounts coa ON cvb.AccountNo = coa.AccountNo 
                                                 WHERE cvb.VoucherNo = @VoucherNo";

                SqlCommand detailCmd = new SqlCommand(detailQuery, connection);
                detailCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                SqlDataReader detailReader = detailCmd.ExecuteReader();
                while (detailReader.Read())
                {
                    DataRow newRow = voucherData.NewRow();
                    newRow["AccountNo"] = detailReader["AccountNo"].ToString();
                    newRow["AccountName"] = detailReader["AccountName"].ToString();
                    newRow["Amount"] = Convert.ToDecimal(detailReader["Amount"]);
                    newRow["BillNo"] = detailReader["BillNo"].ToString();
                    newRow["Narration"] = detailReader["Narration"].ToString();
                    voucherData.Rows.Add(newRow);

                    totalAmount += Convert.ToDecimal(detailReader["Amount"]);
                }
                detailReader.Close();

                totaltxtbox.Text = totalAmount.ToString("N2");
                isNewVoucher = false;
                currentVoucherNo = voucherNo;

                MessageBox.Show("Voucher loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
        

        private void printbtn_Click(object sender, EventArgs e)
        {
            if (voucherData.Rows.Count == 0)
            {
                MessageBox.Show("No data to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create print dialog
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Here you would implement printing logic
                // For now, just show a message
                MessageBox.Show("Printing feature will be implemented here.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.ShowDialog();
            this.Close();
        }

        // Add these methods to your form designer.cs or create the controls:

        // 1. Add a Label named "lblCustomerName" 
        // 2. Add a Button named "showBillsbtn" with Text "Show Bills"
        // 3. Add a Button named "searchBillbtn" with Text "Search Bill"
    }
}



