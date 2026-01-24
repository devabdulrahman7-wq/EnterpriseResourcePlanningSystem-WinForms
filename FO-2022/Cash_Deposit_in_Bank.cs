using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace FO_2022
{
    public partial class Cash_Deposit_in_Bank : Form
    {
        private string connectionString;
        private bool isEditMode = false;
        private decimal currentVoucherNo = 0;

        public Cash_Deposit_in_Bank()
        {
            InitializeComponent();

            // Get connection string from config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            InitializeForm();
        }

        private void InitializeForm()
        {
            voucherdatepcker.Value = DateTime.Now;
            depositdatepcker.Value = DateTime.Now;
            voucherdatepcker.Checked = false;

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
                    string query = "SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM BankDeposit";
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

        private void depositchqbox_CheckedChanged(object sender, EventArgs e)
        {
            if (reconcilethisdepositchqbox.Checked)
            {
                label3.Text = "Cheque No";
                //slipnotxtbox.PlaceholderText = "Enter Cheque Number";
            }
            else
            {
                label3.Text = "Slip No";
                //slipnotxtbox.PlaceholderText = "Enter Slip Number";
            }
        }

        //private void accountnosearch_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
        //    {
        //        MessageBox.Show("Please enter account number");
        //        return;
        //    }

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            string query = @"SELECT AccountName FROM ChartOfAccounts 
        //                           WHERE AccountNo = @AccountNo AND AccountType LIKE '%Bank%'";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

        //            object result = cmd.ExecuteScalar();

        //            if (result != null)
        //            {
        //                banknametxtbox.Text = result.ToString();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Bank account not found or not a bank account");
        //                banknametxtbox.Clear();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error fetching bank account: " + ex.Message);
        //    }
        //}
        private void accountnosearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
            {
                MessageBox.Show("Please enter account number");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // REMOVE the Bank type restriction - fetch ANY account
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                           WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        banknametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Account not found!");
                        banknametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching account: " + ex.Message);
            }
        }
        private void Cash_Deposit_in_Bank_Load(object sender, EventArgs e)
        {
            depositchqbox_CheckedChanged(null, null);
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (isEditMode)
                {
                    UpdateBankDeposit();
                }
                else
                {
                    InsertBankDeposit();
                }

                MessageBox.Show("Bank deposit saved successfully!");
                ClearForm();
                GenerateVoucherNo();
                isEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving bank deposit: " + ex.Message);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
            {
                MessageBox.Show("Please enter bank account number");
                return false;
            }

            if (string.IsNullOrWhiteSpace(banknametxtbox.Text))
            {
                MessageBox.Show("Please search and verify bank account");
                return false;
            }

            if (string.IsNullOrWhiteSpace(amounttxtbox.Text) || !decimal.TryParse(amounttxtbox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter valid amount");
                return false;
            }

            if (reconcilethisdepositchqbox.Checked && string.IsNullOrWhiteSpace(slipnotxtbox.Text))
            {
                MessageBox.Show("Please enter cheque number");
                return false;
            }

            if (!voucherdatepcker.Checked && string.IsNullOrWhiteSpace(slipnotxtbox.Text))
            {
                MessageBox.Show("Please enter slip number");
                return false;
            }

            return true;
        }

        private void InsertBankDeposit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO BankDeposit 
                                (VoucherNo, VoucherDate, IsCheque, ChequeNo, SlipNo, DepositedDate, 
                                 ReconcileDate, BankAccountNo, AccountNo, UserNo, Amount, 
                                 DepositedBy, IsPosted, Narration, IsBounced)
                                VALUES 
                                (@VoucherNo, @VoucherDate, @IsCheque, @ChequeNo, @SlipNo, @DepositedDate,
                                 NULL, @BankAccountNo, @AccountNo, @UserNo, @Amount,
                                 @DepositedBy, 0, @Narration, 0)";

                SqlCommand cmd = new SqlCommand(query, conn);

                decimal voucherNo = decimal.Parse(vouchernotxtbox.Text);

                cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                cmd.Parameters.AddWithValue("@IsCheque", reconcilethisdepositchqbox.Checked);

                if (reconcilethisdepositchqbox.Checked)
                {
                    cmd.Parameters.AddWithValue("@ChequeNo", slipnotxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@SlipNo", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ChequeNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SlipNo", slipnotxtbox.Text.Trim());
                }

                cmd.Parameters.AddWithValue("@DepositedDate", depositdatepcker.Value);
                cmd.Parameters.AddWithValue("@BankAccountNo", banknotxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@AccountNo", DBNull.Value);
                cmd.Parameters.AddWithValue("@UserNo", 1);
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                cmd.Parameters.AddWithValue("@DepositedBy", depositedtxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());

                cmd.ExecuteNonQuery();

                // Also insert into BankVouchers
                InsertIntoBankVouchers(conn, voucherNo);
            }
        }

        private void InsertIntoBankVouchers(SqlConnection conn, decimal voucherNo)
        {
            try
            {
                // First check if voucher already exists
                string checkQuery = "SELECT COUNT(*) FROM BankVouchers WHERE VoucherNo = @VoucherNo";
                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count == 0)
                {
                    // Insert into BankVouchers
                    string voucherQuery = @"INSERT INTO BankVouchers 
                                          (VoucherNo, VoucherDate, LastUpdateDate, UserNo, OfficeID, IsUploaded, IsPosted)
                                          VALUES 
                                          (@VoucherNo, @VoucherDate, GETDATE(), @UserNo, NULL, 0, 0)";

                    SqlCommand voucherCmd = new SqlCommand(voucherQuery, conn);
                    voucherCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                    voucherCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                    voucherCmd.Parameters.AddWithValue("@UserNo", 1);
                    voucherCmd.ExecuteNonQuery();
                }

                // Delete existing entries for this voucher
                string deleteQuery = "DELETE FROM BankVouchersBody WHERE VoucherNo = @VoucherNo";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                deleteCmd.ExecuteNonQuery();

                // Insert into BankVouchersBody for the bank account (credit side)
                string bodyQuery = @"INSERT INTO BankVouchersBody 
                                   (VoucherNo, AccountNo, Narration, Debit, Credit)
                                   VALUES 
                                   (@VoucherNo, @AccountNo, @Narration, 0, @Amount)";

                SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn);
                bodyCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                bodyCmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Narration", "Bank Deposit: " + narrationtxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                bodyCmd.ExecuteNonQuery();

                // Insert into BankVouchersBody for cash account (debit side)
                string cashAccountNo = GetCashAccountNo();

                bodyCmd.Parameters.Clear();
                bodyCmd.Parameters.AddWithValue("@VoucherNo", (short)voucherNo);
                bodyCmd.Parameters.AddWithValue("@AccountNo", cashAccountNo);
                bodyCmd.Parameters.AddWithValue("@Narration", "Cash deposited in bank: " + narrationtxtbox.Text.Trim());
                bodyCmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                bodyCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Log error but don't stop the main insertion
                Console.WriteLine("Error inserting into BankVouchers: " + ex.Message);
            }
        }

        private string GetCashAccountNo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT TOP 1 AccountNo FROM ChartOfAccounts 
                                   WHERE AccountName LIKE '%Cash%' AND AccountType LIKE '%Current Asset%' 
                                   ORDER BY AccountNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "1001";
                }
            }
            catch
            {
                return "1001";
            }
        }

        private void UpdateBankDeposit()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"UPDATE BankDeposit SET
                                VoucherDate = @VoucherDate,
                                IsCheque = @IsCheque,
                                ChequeNo = @ChequeNo,
                                SlipNo = @SlipNo,
                                DepositedDate = @DepositedDate,
                                BankAccountNo = @BankAccountNo,
                                Amount = @Amount,
                                DepositedBy = @DepositedBy,
                                Narration = @Narration
                                WHERE VoucherNo = @VoucherNo";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
                cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                cmd.Parameters.AddWithValue("@IsCheque", reconcilethisdepositchqbox.Checked);

                if (reconcilethisdepositchqbox.Checked)
                {
                    cmd.Parameters.AddWithValue("@ChequeNo", slipnotxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@SlipNo", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ChequeNo", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SlipNo", slipnotxtbox.Text.Trim());
                }

                cmd.Parameters.AddWithValue("@DepositedDate", depositdatepcker.Value);
                cmd.Parameters.AddWithValue("@BankAccountNo", banknotxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                cmd.Parameters.AddWithValue("@DepositedBy", depositedtxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Update BankVouchers
                    UpdateBankVouchers(conn);
                }
            }
        }

        private void UpdateBankVouchers(SqlConnection conn)
        {
            try
            {
                // Update BankVouchers date
                string voucherQuery = @"UPDATE BankVouchers SET 
                                      VoucherDate = @VoucherDate,
                                      LastUpdateDate = GETDATE()
                                      WHERE VoucherNo = @VoucherNo";

                SqlCommand voucherCmd = new SqlCommand(voucherQuery, conn);
                voucherCmd.Parameters.AddWithValue("@VoucherNo", (short)currentVoucherNo);
                voucherCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                voucherCmd.ExecuteNonQuery();

                // Delete existing entries
                string deleteQuery = "DELETE FROM BankVouchersBody WHERE VoucherNo = @VoucherNo";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn);
                deleteCmd.Parameters.AddWithValue("@VoucherNo", (short)currentVoucherNo);
                deleteCmd.ExecuteNonQuery();

                // Insert updated entries
                InsertIntoBankVouchers(conn, currentVoucherNo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating BankVouchers: " + ex.Message);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            isEditMode = false;
        }

        private void ClearForm()
        {
            voucherdatepcker.Value = DateTime.Now;
            depositdatepcker.Value = DateTime.Now;
            reconcilethisdepositchqbox.Checked = false;
            slipnotxtbox.Clear();
            banknotxtbox.Clear();
            banknametxtbox.Clear();
            amounttxtbox.Clear();
            amountinwordstxtbox.Clear();
            narrationtxtbox.Clear();
            depositedtxtbox.Clear();

            depositchqbox_CheckedChanged(null, null);
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (!isEditMode || currentVoucherNo == 0)
            {
                MessageBox.Show("Please open a record to delete");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this deposit?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Delete from BankVouchersBody first
                        string deleteBodyQuery = "DELETE FROM BankVouchersBody WHERE VoucherNo = @VoucherNo";
                        SqlCommand bodyCmd = new SqlCommand(deleteBodyQuery, conn);
                        bodyCmd.Parameters.AddWithValue("@VoucherNo", (short)currentVoucherNo);
                        bodyCmd.ExecuteNonQuery();

                        // Delete from BankVouchers
                        string deleteVoucherQuery = "DELETE FROM BankVouchers WHERE VoucherNo = @VoucherNo";
                        SqlCommand voucherCmd = new SqlCommand(deleteVoucherQuery, conn);
                        voucherCmd.Parameters.AddWithValue("@VoucherNo", (short)currentVoucherNo);
                        voucherCmd.ExecuteNonQuery();

                        // Delete from BankDeposit
                        string deleteQuery = "DELETE FROM BankDeposit WHERE VoucherNo = @VoucherNo";
                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@VoucherNo", currentVoucherNo);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Bank deposit deleted successfully!");
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
                    MessageBox.Show("Error deleting bank deposit: " + ex.Message);
                }
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Bank Deposit";
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
                            LoadBankDeposit(voucherNo);
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
                MessageBox.Show("Error opening bank deposit: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBankDeposit(decimal voucherNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"SELECT * FROM BankDeposit WHERE VoucherNo = @VoucherNo";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isEditMode = true;
                            currentVoucherNo = voucherNo;

                            vouchernotxtbox.Text = reader["VoucherNo"].ToString();
                            voucherdatepcker.Value = Convert.ToDateTime(reader["VoucherDate"]);

                            bool isCheque = Convert.ToBoolean(reader["IsCheque"]);
                            reconcilethisdepositchqbox.Checked = isCheque;

                            if (isCheque)
                            {
                                slipnotxtbox.Text = reader["ChequeNo"]?.ToString() ?? "";
                            }
                            else
                            {
                                slipnotxtbox.Text = reader["SlipNo"]?.ToString() ?? "";
                            }

                            if (reader["DepositedDate"] != DBNull.Value)
                            {
                                depositdatepcker.Value = Convert.ToDateTime(reader["DepositedDate"]);
                            }

                            banknotxtbox.Text = reader["BankAccountNo"].ToString();
                            accountnosearch_Click(null, null);

                            amounttxtbox.Text = Convert.ToDecimal(reader["Amount"]).ToString("N2");
                            amounttxtbox_KeyDown(null, new KeyEventArgs(Keys.Enter));

                            depositedtxtbox.Text = reader["DepositedBy"]?.ToString() ?? "";
                            narrationtxtbox.Text = reader["Narration"]?.ToString() ?? "";
                        }
                        else
                        {
                            MessageBox.Show("Voucher not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bank deposit: " + ex.Message);
            }
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            if (!isEditMode || currentVoucherNo == 0)
            {
                MessageBox.Show("Please open a record to print");
                return;
            }

            // Implement printing logic here
            MessageBox.Show("Print functionality to be implemented");
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

        // Other event handlers that don't need implementation
        private void vouchernotxtbox_TextChanged(object sender, EventArgs e) { }
        private void slipnotxtbox_TextChanged(object sender, EventArgs e) { }
        //private void voucherdatepicker_ValueChanged(object sender, EventArgs e) { }
        private void depositdatepcker_ValueChanged(object sender, EventArgs e) { }
        private void banknotxtbox_TextChanged(object sender, EventArgs e) { }
        private void banknametxtbox_TextChanged(object sender, EventArgs e) { }
        private void amounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void amountinwordstxtbox_TextChanged(object sender, EventArgs e) { }
        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
        private void depositedtxtbox_TextChanged(object sender, EventArgs e) { }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }
    }
}