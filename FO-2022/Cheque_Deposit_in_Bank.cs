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
    public partial class Cheque_Deposit_in_Bank : Form
    {
        private string connectionString;
        private bool isEditMode = false;
        private decimal currentVoucherNo = 0;

        public Cheque_Deposit_in_Bank()
        {
            InitializeComponent();

            // Get connection string from config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            InitializeForm();
        }

        private void InitializeForm()
        {
            voucherdatepcker.Value = DateTime.Now;
            chqdatepcker.Value = DateTime.Now;
            chqdatepcker.Value = DateTime.Now;
            depositconfirmedchqbox.Checked = false;
            reconcilecthisdeposithqbox.Checked = false;

            // Auto-generate voucher number
            GenerateVoucherNo();
        }

        private void GenerateVoucherNo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM BankCheques";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        vouchernotxtbox.Text = result.ToString();
                        currentVoucherNo = Convert.ToDecimal(result);
                    }
                    else
                    {
                        vouchernotxtbox.Text = "1";
                        currentVoucherNo = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating voucher number: " + ex.Message);
                vouchernotxtbox.Text = "1";
                currentVoucherNo = 1;
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        // Search for Bank Account (Where cheque is being deposited)
        private void bankaccountnosearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bankaccountnotxtbox.Text))
            {
                MessageBox.Show("Please enter bank account number");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch from ChartOfAccounts table
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                                   WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", bankaccountnotxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        bankaccountnametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Bank account not found in Chart of Accounts!");
                        bankaccountnametxtbox.Clear();
                        bankaccountnotxtbox.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bank account: " + ex.Message);
            }
        }

        // Search for "Received From" Account (Who gave the cheque)
        private void accoutnnodsearch_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(receivedbytxtbox.Text))
            //{
            //    MessageBox.Show("Please enter account number");
            //    return;
            //}

            //try
            //{
            //    using (SqlConnection conn = new SqlConnection(connectionString))
            //    {
            //        conn.Open();

            //        // Fetch from ChartOfAccounts table
            //        string query = @"SELECT AccountName FROM ChartOfAccounts 
            //                       WHERE AccountNo = @AccountNo";

            //        SqlCommand cmd = new SqlCommand(query, conn);
            //        cmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());

            //        object result = cmd.ExecuteScalar();

            //        if (result != null)
            //        {
            //            receivedbyaccountnametxtbox.Text = result.ToString();
            //        }
            //        else
            //        {
            //            MessageBox.Show("Account not found in Chart of Accounts!");
            //            receivedbyaccountnametxtbox.Clear();
            //            receivedbytxtbox.Focus();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error fetching account: " + ex.Message);
            //}
            if (string.IsNullOrWhiteSpace(bankaccountnotxtbox.Text))
            {
                MessageBox.Show("Please enter BANK account number");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch from ChartOfAccounts table
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                           WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", bankaccountnotxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        bankaccountnametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Bank account not found in Chart of Accounts!");
                        bankaccountnametxtbox.Clear();
                        bankaccountnotxtbox.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bank account: " + ex.Message);
            }
        }

        private void Cheque_Deposit_in_Bank_Load(object sender, EventArgs e)
        {
            // Initialize form
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (isEditMode)
                {
                    UpdateChequeDeposit();
                }
                else
                {
                    InsertChequeDeposit();
                }

                MessageBox.Show("Cheque deposit saved successfully!");
                ClearForm();
                GenerateVoucherNo();
                isEditMode = false;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign key violation
                {
                    MessageBox.Show("Error: Account does not exist in Chart of Accounts.\n" +
                                  "Please verify both account numbers exist in Chart of Accounts.",
                                  "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error saving cheque deposit: " + sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving cheque deposit: " + ex.Message);
            }
        }

        private bool ValidateInput()
        {
            // Validate Bank Account (Where cheque is deposited)
            if (string.IsNullOrWhiteSpace(bankaccountnotxtbox.Text))
            {
                MessageBox.Show("Please enter bank account number");
                bankaccountnotxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(bankaccountnametxtbox.Text))
            {
                MessageBox.Show("Please search and verify bank account");
                bankaccountnotxtbox.Focus();
                return false;
            }

            // Validate Received From Account (Who gave cheque)
            if (string.IsNullOrWhiteSpace(receivedbytxtbox.Text))
            {
                MessageBox.Show("Please enter 'Received From' account number");
                receivedbytxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(receivedbyaccountnametxtbox.Text))
            {
                MessageBox.Show("Please search and verify 'Received From' account");
                receivedbytxtbox.Focus();
                return false;
            }

            // Validate Cheque Details
            if (string.IsNullOrWhiteSpace(chqnotxtbox.Text))
            {
                MessageBox.Show("Please enter cheque number");
                chqnotxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(amounttxtbox.Text) ||
                !decimal.TryParse(amounttxtbox.Text, out decimal amount) ||
                amount <= 0)
            {
                MessageBox.Show("Please enter valid amount");
                amounttxtbox.Focus();
                return false;
            }

            // Validate Slip Number
            if (string.IsNullOrWhiteSpace(sliptxtnobox.Text))
            {
                MessageBox.Show("Please enter slip number");
                sliptxtnobox.Focus();
                return false;
            }

            return true;
        }

        private void InsertChequeDeposit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // INSERT INTO BankCheques TABLE (Main table for cheque deposits)
                string query = @"INSERT INTO BankCheques 
                                (VoucherNo, VoucherDate, ChequeNo, ChequeDate, 
                                 ReconcileDate, BankAccountNo, AccountNo, UserNo, 
                                 Amount, ReceivedBy, IsPosted, Narration, IsLost)
                                VALUES 
                                (@VoucherNo, @VoucherDate, @ChequeNo, @ChequeDate,
                                 @ReconcileDate, @BankAccountNo, @AccountNo, @UserNo, 
                                 @Amount, @ReceivedBy, 0, @Narration, 0)";

                SqlCommand cmd = new SqlCommand(query, conn);

                decimal voucherNo = decimal.Parse(vouchernotxtbox.Text);

                cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                cmd.Parameters.AddWithValue("@ChequeNo", chqnotxtbox.Text.Trim());

                if (chqdatepcker.Value != DateTime.MinValue)
                {
                    cmd.Parameters.AddWithValue("@ChequeDate", chqdatepcker.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ChequeDate", DBNull.Value);
                }

                // Set ReconcileDate if checkbox is checked
                if (reconcilecthisdeposithqbox.Checked)
                {
                    cmd.Parameters.AddWithValue("@ReconcileDate", chqdatepcker.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ReconcileDate", DBNull.Value);
                }

                // BankAccountNo = Bank where cheque is deposited
                cmd.Parameters.AddWithValue("@BankAccountNo", bankaccountnotxtbox.Text.Trim());

                // AccountNo = Who gave the cheque (Received From)
                cmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());

                cmd.Parameters.AddWithValue("@UserNo", 1); // Current user ID
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));

                // ReceivedBy = Employee who physically received the cheque
                cmd.Parameters.AddWithValue("@ReceivedBy", depositedtxtbox.Text.Trim());

                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());

                cmd.ExecuteNonQuery();

                // OPTIONAL: Also insert into BankDeposit table
                try
                {
                    InsertIntoBankDeposit(conn, voucherNo);
                }
                catch
                {
                    // Ignore if BankDeposit insert fails
                }

                // OPTIONAL: Insert voucher entries for accounting
                try
                {
                    InsertVoucherEntries(conn, voucherNo);
                }
                catch
                {
                    // Ignore if voucher entries fail
                }
            }
        }

        private void InsertIntoBankDeposit(SqlConnection conn, decimal voucherNo)
        {
            // Optional: Insert into BankDeposit table for consistency
            string query = @"INSERT INTO BankDeposit 
                            (VoucherNo, VoucherDate, IsCheque, ChequeNo, SlipNo, 
                             DepositedDate, ReconcileDate, BankAccountNo, AccountNo, 
                             UserNo, Amount, DepositedBy, IsPosted, Narration, IsBounced)
                            VALUES 
                            (@VoucherNo, @VoucherDate, 1, @ChequeNo, @SlipNo,
                             @DepositedDate, @ReconcileDate, @BankAccountNo, @AccountNo,
                             @UserNo, @Amount, @DepositedBy, 0, @Narration, 0)";

            SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
            cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
            cmd.Parameters.AddWithValue("@ChequeNo", chqnotxtbox.Text.Trim());
            cmd.Parameters.AddWithValue("@SlipNo", sliptxtnobox.Text.Trim());
            cmd.Parameters.AddWithValue("@DepositedDate", chqdatepcker.Value);

            if (reconcilecthisdeposithqbox.Checked)
            {
                cmd.Parameters.AddWithValue("@ReconcileDate", chqdatepcker.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ReconcileDate", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@BankAccountNo", bankaccountnotxtbox.Text.Trim());
            cmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());
            cmd.Parameters.AddWithValue("@UserNo", 1);
            cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
            cmd.Parameters.AddWithValue("@DepositedBy", depositedtxtbox.Text.Trim());
            cmd.Parameters.AddWithValue("@Narration", "Cheque Deposit: " + narrationtxtbox.Text.Trim());

            cmd.ExecuteNonQuery();
        }

        private void InsertVoucherEntries(SqlConnection conn, decimal voucherNo)
        {
            // Optional: Create accounting voucher entries
            try
            {
                // Insert into BankVouchers
                string voucherQuery = @"INSERT INTO BankVouchers 
                                      (VoucherNo, VoucherDate, LastUpdateDate, UserNo, 
                                       OfficeID, IsUploaded, IsPosted)
                                      VALUES 
                                      (@VoucherNo, @VoucherDate, GETDATE(), @UserNo, 
                                       NULL, 0, 0)";

                SqlCommand voucherCmd = new SqlCommand(voucherQuery, conn);
                voucherCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                voucherCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                voucherCmd.Parameters.AddWithValue("@UserNo", 1);
                voucherCmd.ExecuteNonQuery();

                decimal amount = decimal.Parse(amounttxtbox.Text);

                // Create voucher body entries
                string bodyQuery = @"INSERT INTO BankVouchersBody 
                                   (VoucherNo, AccountNo, Narration, Debit, Credit)
                                   VALUES 
                                   (@VoucherNo, @AccountNo, @Narration, @Debit, @Credit)";

                // 1. Bank Account - Debit (Bank receives cheque)
                SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn);
                bodyCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                bodyCmd.Parameters.AddWithValue("@AccountNo", bankaccountnotxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Narration", "Cheque received: " + narrationtxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Debit", amount);
                bodyCmd.Parameters.AddWithValue("@Credit", 0);
                bodyCmd.ExecuteNonQuery();

                // 2. Customer Account - Credit (Customer gave cheque)
                bodyCmd.Parameters.Clear();
                bodyCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                bodyCmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Narration", "Cheque issued: " + narrationtxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Debit", 0);
                bodyCmd.Parameters.AddWithValue("@Credit", amount);
                bodyCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Ignore voucher entry errors
                Console.WriteLine("Voucher entry error: " + ex.Message);
            }
        }

        private void UpdateChequeDeposit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // UPDATE BankCheques TABLE
                string query = @"UPDATE BankCheques SET
                                VoucherDate = @VoucherDate,
                                ChequeNo = @ChequeNo,
                                ChequeDate = @ChequeDate,
                                ReconcileDate = @ReconcileDate,
                                BankAccountNo = @BankAccountNo,
                                AccountNo = @AccountNo,
                                Amount = @Amount,
                                ReceivedBy = @ReceivedBy,
                                Narration = @Narration
                                WHERE VoucherNo = @VoucherNo";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                cmd.Parameters.AddWithValue("@ChequeNo", chqnotxtbox.Text.Trim());

                if (chqdatepcker.Value != DateTime.MinValue)
                {
                    cmd.Parameters.AddWithValue("@ChequeDate", chqdatepcker.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ChequeDate", DBNull.Value);
                }

                if (reconcilecthisdeposithqbox.Checked)
                {
                    cmd.Parameters.AddWithValue("@ReconcileDate", chqdatepcker.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ReconcileDate", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@BankAccountNo", bankaccountnotxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                cmd.Parameters.AddWithValue("@ReceivedBy", depositedtxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Cheque deposit updated successfully!");
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            isEditMode = false;
        }

        private void ClearForm()
        {
            // Keep voucher number (it auto-generates)
            voucherdatepcker.Value = DateTime.Now;
            chqdatepcker.Value = DateTime.Now;
            chqdatepcker.Value = DateTime.Now;
            depositconfirmedchqbox.Checked = false;
            reconcilecthisdeposithqbox.Checked = false;

            sliptxtnobox.Clear();
            chqnotxtbox.Clear();
            bankaccountnotxtbox.Clear();
            bankaccountnametxtbox.Clear();
            receivedbytxtbox.Clear();
            receivedbyaccountnametxtbox.Clear();
            amounttxtbox.Clear();
            amountinwordstxtbox.Clear();
            narrationtxtbox.Clear();
            depositedtxtbox.Clear();
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (!isEditMode || currentVoucherNo == 0)
            {
                MessageBox.Show("Please open a record to delete");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this cheque deposit?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Delete from BankCheques (main table)
                        string deleteQuery = "DELETE FROM BankCheques WHERE VoucherNo = @VoucherNo";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cheque deposit deleted successfully!");
                            ClearForm();
                            GenerateVoucherNo();
                            isEditMode = false;
                        }
                        else
                        {
                            MessageBox.Show("Record not found!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting cheque deposit: " + ex.Message);
                }
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Simple input dialog for voucher number
                Form inputForm = new Form();
                inputForm.Text = "Open Cheque Deposit";
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
                        if (decimal.TryParse(input, out decimal voucherNo))
                        {
                            LoadChequeDeposit(voucherNo);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid numeric Voucher No!", "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening cheque deposit: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChequeDeposit(decimal voucherNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // SELECT FROM BankCheques TABLE
                    string query = @"SELECT * FROM BankCheques WHERE VoucherNo = @VoucherNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isEditMode = true;
                            currentVoucherNo = voucherNo;

                            // Load data from BankCheques table
                            vouchernotxtbox.Text = reader["VoucherNo"].ToString();

                            if (reader["VoucherDate"] != DBNull.Value)
                            {
                                voucherdatepcker.Value = Convert.ToDateTime(reader["VoucherDate"]);
                            }

                            chqnotxtbox.Text = reader["ChequeNo"].ToString();

                            if (reader["ChequeDate"] != DBNull.Value)
                            {
                                chqdatepcker.Value = Convert.ToDateTime(reader["ChequeDate"]);
                            }

                            // Bank Account (where cheque deposited)
                            bankaccountnotxtbox.Text = reader["BankAccountNo"].ToString();
                            bankaccountnosearch_Click(null, null); // Fetch bank name

                            // Received From Account (who gave cheque)
                            receivedbytxtbox.Text = reader["AccountNo"].ToString();
                            accoutnnodsearch_Click(null, null); // Fetch account name

                            amounttxtbox.Text = Convert.ToDecimal(reader["Amount"]).ToString("N2");
                            amounttxtbox_KeyDown(null, new KeyEventArgs(Keys.Enter)); // Convert to words

                            depositedtxtbox.Text = reader["ReceivedBy"]?.ToString() ?? "";
                            narrationtxtbox.Text = reader["Narration"]?.ToString() ?? "";

                            // Check reconcile status
                            if (reader["ReconcileDate"] != DBNull.Value)
                            {
                                reconcilecthisdeposithqbox.Checked = true;
                                chqdatepcker.Value = Convert.ToDateTime(reader["ReconcileDate"]);
                            }
                            else
                            {
                                reconcilecthisdeposithqbox.Checked = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cheque deposit not found!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cheque deposit: " + ex.Message);
            }
        }

        private void amounttxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (decimal.TryParse(amounttxtbox.Text, out decimal amount))
                {
                    amountinwordstxtbox.Text = ConvertNumberToWords(amount);
                }
                else
                {
                    amountinwordstxtbox.Text = "Invalid amount";
                }
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private string ConvertNumberToWords(decimal number)
        {
            if (number == 0)
                return "Zero Rupees Only";

            long rupees = (long)Math.Floor(number);
            long paisa = (long)((number - rupees) * 100);

            string rupeesWords = ConvertToWords(rupees) + " Rupees";
            string paisaWords = paisa > 0 ? " and " + ConvertToWords(paisa) + " Paisa" : "";

            return rupeesWords + paisaWords + " Only";
        }

        private string ConvertToWords(long number)
        {
            if (number == 0)
                return "Zero";

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ConvertToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertToWords(number / 100000) + " Lac ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            if (!isEditMode || currentVoucherNo == 0)
            {
                MessageBox.Show("Please open a record to print");
                return;
            }

            MessageBox.Show("Print functionality to be implemented");
        }

        // Empty event handlers
        private void vouchernotxtbox_TextChanged(object sender, EventArgs e) { }
        private void sliptxtnobox_TextChanged(object sender, EventArgs e) { }
        private void chqdatepcker_ValueChanged(object sender, EventArgs e) { }
        private void chqnotxtbox_TextChanged(object sender, EventArgs e) { }
        private void depositconfirmedchqbox_CheckedChanged(object sender, EventArgs e) { }
        private void reconcilecthisdeposithqbox_CheckedChanged(object sender, EventArgs e) { }
        private void receivedbytxtbox_TextChanged(object sender, EventArgs e) { }
        private void bankaccountnotxtbox_TextChanged(object sender, EventArgs e) { }
        private void bankaccountnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void receivedbyaccountnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void amounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void amountinwordstxtbox_TextChanged(object sender, EventArgs e) { }
        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
        private void depositedtxtbox_TextChanged(object sender, EventArgs e) { }
        private void voucherdatepcker_ValueChanged(object sender, EventArgs e) { }

        private void receivedbysearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(receivedbytxtbox.Text))
            {
                MessageBox.Show("Please enter RECEIVED FROM account number");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Fetch from ChartOfAccounts table
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                           WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", receivedbytxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        receivedbyaccountnametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Account not found in Chart of Accounts!");
                        receivedbyaccountnametxtbox.Clear();
                        receivedbytxtbox.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching account: " + ex.Message);
            }
        }
        //private void chqdatepcker_ValueChanged(object sender, EventArgs e) { }
    }
}