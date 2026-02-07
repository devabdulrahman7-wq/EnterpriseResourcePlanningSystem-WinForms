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
    public partial class Bank_Cheques_Issue : Form
    {
        private string connectionString;
        private DataTable gridDataTable;

        public Bank_Cheques_Issue()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            gridDataTable = new DataTable();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void vouchernotxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void voucherdatepcker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void banknotxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void banknametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void chqnotxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void chqdatepcker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void reconcilethisdepositchqbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void paytotxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void paytoaccountnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void amounttxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void receivedbytxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void totalchqamounttxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Cheque Issue";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.TopMost = true;

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
                            LoadBankCheque(voucherNo);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid Voucher Number!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening cheque issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBankCheque(decimal voucherNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT * FROM BankCheques WHERE VoucherNo = @VoucherNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                vouchernotxtbox.Text = reader["VoucherNo"].ToString();
                                voucherdatepcker.Value = Convert.ToDateTime(reader["VoucherDate"]);
                                chqnotxtbox.Text = reader["ChequeNo"].ToString();
                                chqdatepcker.Value = reader["ChequeDate"] != DBNull.Value ? Convert.ToDateTime(reader["ChequeDate"]) : DateTime.Now;
                                banknotxtbox.Text = reader["BankAccountNo"].ToString();
                                paytotxtbox.Text = reader["AccountNo"].ToString();
                                // amounttxtbox.Text = reader["Amount"].ToString(); // Remove this line
                                receivedbytxtbox.Text = reader["ReceivedBy"]?.ToString();
                                narrationtxtbox.Text = reader["Narration"]?.ToString();

                                // Load bank name
                                LoadBankAccountName(reader["BankAccountNo"].ToString());
                                // Load pay to account name
                                LoadPayToAccountName(reader["AccountNo"].ToString());

                                // Add the amount to grid
                                decimal amount = Convert.ToDecimal(reader["Amount"]);
                                if (amount > 0)
                                {
                                    // Clear existing grid data
                                    gridDataTable.Clear();

                                    // Add row to grid
                                    DataRow newRow = gridDataTable.NewRow();
                                    newRow["AccountNo"] = reader["AccountNo"].ToString();
                                    newRow["AccountName"] = paytoaccountnametxtbox.Text;
                                    newRow["Amount"] = amount;
                                    gridDataTable.Rows.Add(newRow);

                                    // Update total amount
                                    UpdateTotalAmount();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Voucher not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cheque: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void removebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(vouchernotxtbox.Text))
                {
                    MessageBox.Show("Please select a cheque to remove!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to remove this cheque?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM BankCheques WHERE VoucherNo = @VoucherNo";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@VoucherNo", decimal.Parse(vouchernotxtbox.Text));
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Cheque removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                clearbtn.PerformClick(); // Clear all fields
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing cheque: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(banknotxtbox.Text))
                {
                    MessageBox.Show("Please enter Bank Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(paytotxtbox.Text))
                {
                    MessageBox.Show("Please enter Pay To Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(chqnotxtbox.Text))
                {
                    MessageBox.Show("Please enter Cheque No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check if there's any amount in the grid
                if (gridDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Please add at least one amount to the grid!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get total amount from grid
                decimal totalAmount = 0;
                foreach (DataRow row in gridDataTable.Rows)
                {
                    totalAmount += Convert.ToDecimal(row["Amount"]);
                }

                if (totalAmount <= 0)
                {
                    MessageBox.Show("Please enter a valid amount in the grid!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal voucherNo = 0;
                bool isUpdate = false;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if voucher exists (for update)
                    string checkQuery = "SELECT COUNT(*) FROM BankCheques WHERE VoucherNo = @VoucherNo";

                    if (string.IsNullOrEmpty(vouchernotxtbox.Text))
                    {
                        // Get next voucher number for new record
                        voucherNo = GetNextVoucherNumber(conn);
                        vouchernotxtbox.Text = voucherNo.ToString();
                    }
                    else
                    {
                        voucherNo = decimal.Parse(vouchernotxtbox.Text);

                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                            isUpdate = (int)checkCmd.ExecuteScalar() > 0;
                        }
                    }

                    string query;
                    if (isUpdate)
                    {
                        query = @"UPDATE BankCheques SET 
                        VoucherDate = @VoucherDate,
                        ChequeNo = @ChequeNo,
                        ChequeDate = @ChequeDate,
                        ReconcileDate = @ReconcileDate,
                        BankAccountNo = @BankAccountNo,
                        AccountNo = @AccountNo,
                        UserNo = @UserNo,
                        Amount = @Amount,
                        ReceivedBy = @ReceivedBy,
                        IsPosted = @IsPosted,
                        Narration = @Narration,
                        IsLost = @IsLost
                        WHERE VoucherNo = @VoucherNo";
                    }
                    else
                    {
                        query = @"INSERT INTO BankCheques 
                        (VoucherNo, VoucherDate, ChequeNo, ChequeDate, ReconcileDate, 
                        BankAccountNo, AccountNo, UserNo, Amount, ReceivedBy, IsPosted, Narration, IsLost)
                        VALUES 
                        (@VoucherNo, @VoucherDate, @ChequeNo, @ChequeDate, @ReconcileDate,
                        @BankAccountNo, @AccountNo, @UserNo, @Amount, @ReceivedBy, @IsPosted, @Narration, @IsLost)";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                        cmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value.Date);
                        cmd.Parameters.AddWithValue("@ChequeNo", chqnotxtbox.Text);
                        cmd.Parameters.AddWithValue("@ChequeDate", chqdatepcker.Value.Date);
                        cmd.Parameters.AddWithValue("@ReconcileDate", reconcilethisdepositchqbox.Checked ? (object)DateTime.Now.Date : DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankAccountNo", banknotxtbox.Text);
                        cmd.Parameters.AddWithValue("@AccountNo", paytotxtbox.Text);
                        cmd.Parameters.AddWithValue("@UserNo", 1); // Replace with actual user no
                        cmd.Parameters.AddWithValue("@Amount", totalAmount); // Use total amount from grid
                        cmd.Parameters.AddWithValue("@ReceivedBy", string.IsNullOrEmpty(receivedbytxtbox.Text) ? DBNull.Value : (object)receivedbytxtbox.Text);
                        cmd.Parameters.AddWithValue("@IsPosted", false);
                        cmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
                        cmd.Parameters.AddWithValue("@IsLost", false);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Cheque saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // **NEW: Auto clear form after successful save**
                            ClearFormAfterSave(conn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving cheque: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormAfterSave(SqlConnection conn)
        {
            try
            {
                // Clear all text boxes except voucherno (we'll reload it)
                vouchernotxtbox.Clear();
                voucherdatepcker.Value = DateTime.Now;
                banknotxtbox.Clear();
                banknametxtbox.Clear();
                chqnotxtbox.Clear();
                chqdatepcker.Value = DateTime.Now;
                reconcilethisdepositchqbox.Checked = false;
                paytotxtbox.Clear();
                paytoaccountnametxtbox.Clear();
                amounttxtbox.Clear();
                receivedbytxtbox.Clear();
                narrationtxtbox.Clear();
                totalchqamounttxtbox.Clear();

                // Clear data grid view
                gridDataTable.Clear();

                // **Load next voucher number immediately**
                LoadNextVoucherNumber();

                // Set focus to first field
                banknotxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadNextVoucherNumber()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    decimal nextVoucherNo = GetNextVoucherNumber(conn);
                    vouchernotxtbox.Text = nextVoucherNo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading voucher number: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal GetNextVoucherNumber(SqlConnection conn)
        {
            string query = "SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM BankCheques";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }
        
        private void clearbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear all text boxes
                vouchernotxtbox.Clear();
                voucherdatepcker.Value = DateTime.Now;
                banknotxtbox.Clear();
                banknametxtbox.Clear();
                chqnotxtbox.Clear();
                chqdatepcker.Value = DateTime.Now;
                reconcilethisdepositchqbox.Checked = false;
                paytotxtbox.Clear();
                paytoaccountnametxtbox.Clear();
                amounttxtbox.Clear();
                receivedbytxtbox.Clear();
                narrationtxtbox.Clear();
                totalchqamounttxtbox.Clear();

                // Clear data grid view
                gridDataTable.Clear();

                // **Reload next voucher number**
                LoadNextVoucherNumber();

                // Set focus to first field
                banknotxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Bank_Cheques_Issue_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize grid data table structure
                InitializeGridDataTable();

                // Load initial voucher number
                LoadNextVoucherNumber();

                // Set current date
                voucherdatepcker.Value = DateTime.Now;
                chqdatepcker.Value = DateTime.Now;

                // Make voucherno textbox read-only
                vouchernotxtbox.ReadOnly = true;

                // Set Enter key behavior for amount textbox
                amounttxtbox.KeyDown += Amounttxtbox_KeyDown;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeGridDataTable()
        {
            gridDataTable.Columns.Clear();
            gridDataTable.Columns.Add("AccountNo", typeof(string));
            gridDataTable.Columns.Add("AccountName", typeof(string));
            gridDataTable.Columns.Add("Amount", typeof(decimal));
            dataGridView1.DataSource = gridDataTable;
        }

       
        private void Amounttxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddAmountToGrid();
                e.SuppressKeyPress = true; // Prevent the beep sound
            }
        }

        private void AddAmountToGrid()
        {
            try
            {
                if (string.IsNullOrEmpty(amounttxtbox.Text) || !decimal.TryParse(amounttxtbox.Text, out decimal amount))
                {
                    MessageBox.Show("Please enter a valid amount!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(paytotxtbox.Text) || string.IsNullOrEmpty(paytoaccountnametxtbox.Text))
                {
                    MessageBox.Show("Please select an account first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Add row to grid
                DataRow newRow = gridDataTable.NewRow();
                newRow["AccountNo"] = paytotxtbox.Text;
                newRow["AccountName"] = paytoaccountnametxtbox.Text;
                newRow["Amount"] = amount;
                gridDataTable.Rows.Add(newRow);

                // Update total amount
                UpdateTotalAmount();

                // Clear amount field
                amounttxtbox.Clear();

                // Focus back to amount field
                amounttxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding amount to grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalAmount()
        {
            decimal total = 0;
            foreach (DataRow row in gridDataTable.Rows)
            {
                total += Convert.ToDecimal(row["Amount"]);
            }
            totalchqamounttxtbox.Text = total.ToString("N2");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // You can implement row selection logic here if needed
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }

        private void accountsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(banknotxtbox.Text))
                {
                    MessageBox.Show("Please enter Bank Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadBankAccountName(banknotxtbox.Text);
                LoadChequeNumbers(banknotxtbox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBankAccountName(string accountNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            banknametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Account not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            banknametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bank account name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChequeNumbers(string accountNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT TOP 1 ChequeNo FROM ChequeBookBody 
                                   WHERE AccountNo = @AccountNo AND IsCancel = 0 AND IsDeleted = 0
                                   ORDER BY SerialNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            chqnotxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("No available cheque numbers found for this account!", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            chqnotxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cheque numbers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void paysearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(paytotxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LoadPayToAccountName(paytotxtbox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPayToAccountName(string accountNo)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", accountNo);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            paytoaccountnametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Account not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            paytoaccountnametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(vouchernotxtbox.Text))
                {
                    MessageBox.Show("No cheque data to print!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Here you can implement your printing logic
                // For now, just show a message
                MessageBox.Show("Print functionality to be implemented. Voucher No: " + vouchernotxtbox.Text,
                    "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // You can add code to print the cheque voucher
                // Example: PrintDocument, Crystal Reports, RDLC, etc.
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}