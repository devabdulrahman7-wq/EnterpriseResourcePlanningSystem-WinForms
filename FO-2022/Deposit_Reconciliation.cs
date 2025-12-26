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
    public partial class Deposit_Reconciliation : Form
    {
        private string connectionString;
        private decimal selectedVoucherNo = 0;
        private string selectedBankAccountNo = "";

        public Deposit_Reconciliation()
        {
            InitializeComponent();

            // Get connection string from config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            InitializeForm();
        }

        private void InitializeForm()
        {
            // Clear textboxes
            banknotxtbox.Clear();
            banknametxtbox.Clear();
            reconciledbalancetxtbox.Clear();
            unreconciledbalancetxtbox.Clear();

            // Setup grid
            SetupGridColumns();
        }

        private void SetupGridColumns()
        {
            // Clear existing columns
            availablecheckgrid.Columns.Clear();

            // Add columns manually
            availablecheckgrid.Columns.Add("VoucherNo", "V. No");
            availablecheckgrid.Columns.Add("VoucherDate", "Voucher Date");
            availablecheckgrid.Columns.Add("ChequeNo", "Cheque No");
            availablecheckgrid.Columns.Add("SlipNo", "Slip No");
            availablecheckgrid.Columns.Add("DepositedDate", "Deposit Date");
            availablecheckgrid.Columns.Add("Amount", "Amount");
            availablecheckgrid.Columns.Add("Narration", "Narration");
            availablecheckgrid.Columns.Add("ReconcileDate", "Reconcile Date");
            availablecheckgrid.Columns.Add("IsBounced", "Bounced");
            availablecheckgrid.Columns.Add("BankAccountNo", "Bank Account"); // Hidden column
            availablecheckgrid.Columns.Add("IsCheque", "Is Cheque"); // Hidden column

            // Format columns
            availablecheckgrid.Columns["VoucherDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablecheckgrid.Columns["DepositedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablecheckgrid.Columns["ReconcileDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablecheckgrid.Columns["Amount"].DefaultCellStyle.Format = "N2";
            availablecheckgrid.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Hide extra columns
            availablecheckgrid.Columns["BankAccountNo"].Visible = false;
            availablecheckgrid.Columns["IsCheque"].Visible = false;

            // Set column widths
            availablecheckgrid.Columns["VoucherNo"].Width = 80;
            availablecheckgrid.Columns["VoucherDate"].Width = 90;
            availablecheckgrid.Columns["ChequeNo"].Width = 80;
            availablecheckgrid.Columns["SlipNo"].Width = 80;
            availablecheckgrid.Columns["DepositedDate"].Width = 100;
            availablecheckgrid.Columns["Amount"].Width = 80;
            availablecheckgrid.Columns["Narration"].Width = 150;
            availablecheckgrid.Columns["ReconcileDate"].Width = 100;
            availablecheckgrid.Columns["IsBounced"].Width = 80;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void searchbankno_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
            {
                MessageBox.Show("Please enter bank account number");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Fetch bank name from ChartOfAccounts
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                                   WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        banknametxtbox.Text = result.ToString();
                        selectedBankAccountNo = banknotxtbox.Text.Trim();

                        // 2. Load deposits for this bank account
                        LoadBankDeposits();

                        // 3. Calculate balances
                        CalculateBalances();
                    }
                    else
                    {
                        MessageBox.Show("Bank account not found!");
                        banknametxtbox.Clear();
                        availablecheckgrid.Rows.Clear();
                        ClearBalanceFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bank account: " + ex.Message);
            }
        }

        private void LoadBankDeposits()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Build WHERE clause based on selected radio button
                    string whereClause = "WHERE BankAccountNo = @BankAccountNo";

                    if (unreconciledradiobtn.Checked)
                    {
                        whereClause += " AND ReconcileDate IS NULL AND IsBounced = 0";
                    }
                    else if (reconciledradiobtn.Checked)
                    {
                        whereClause += " AND ReconcileDate IS NOT NULL";
                    }
                    else if (bouncedradiobtn.Checked)
                    {
                        whereClause += " AND IsBounced = 1";
                    }
                    // "All" radio button: no extra conditions

                    string query = $@"SELECT 
                            VoucherNo,
                            VoucherDate,
                            IsCheque,
                            ChequeNo,
                            SlipNo,
                            DepositedDate,
                            Amount,
                            Narration,
                            ReconcileDate,
                            IsBounced,
                            BankAccountNo
                            FROM BankDeposit 
                            {whereClause}
                            ORDER BY VoucherDate DESC, VoucherNo DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        availablecheckgrid.Rows.Clear();

                        while (reader.Read())
                        {
                            // Add row to grid
                            int rowIndex = availablecheckgrid.Rows.Add();
                            DataGridViewRow row = availablecheckgrid.Rows[rowIndex];

                            // Fill data
                            row.Cells["VoucherNo"].Value = reader["VoucherNo"];
                            row.Cells["VoucherDate"].Value = reader["VoucherDate"];

                            bool isCheque = Convert.ToBoolean(reader["IsCheque"]);
                            row.Cells["IsCheque"].Value = isCheque;

                            if (isCheque)
                            {
                                row.Cells["ChequeNo"].Value = reader["ChequeNo"]?.ToString() ?? "";
                                row.Cells["SlipNo"].Value = "";
                            }
                            else
                            {
                                row.Cells["ChequeNo"].Value = "";
                                row.Cells["SlipNo"].Value = reader["SlipNo"]?.ToString() ?? "";
                            }

                            row.Cells["DepositedDate"].Value = reader["DepositedDate"];
                            row.Cells["Amount"].Value = Convert.ToDecimal(reader["Amount"]);
                            row.Cells["Narration"].Value = reader["Narration"]?.ToString() ?? "";

                            if (reader["ReconcileDate"] != DBNull.Value)
                            {
                                row.Cells["ReconcileDate"].Value = reader["ReconcileDate"];
                            }
                            else
                            {
                                row.Cells["ReconcileDate"].Value = "";
                            }

                            bool isBounced = Convert.ToBoolean(reader["IsBounced"]);
                            row.Cells["IsBounced"].Value = isBounced ? "YES" : "NO";

                            row.Cells["BankAccountNo"].Value = reader["BankAccountNo"];

                            // Color code rows
                            //if (isBounced)
                            //{
                            //    row.DefaultCellStyle.BackColor = Color.LightPink;
                            //}
                            //else if (reader["ReconcileDate"] != DBNull.Value)
                            //{
                            //    row.DefaultCellStyle.BackColor = Color.LightGreen;
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading deposits: " + ex.Message);
            }
        }

        private void CalculateBalances()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Calculate RECONCILED balance (where ReconcileDate is NOT NULL)
                    string reconciledQuery = @"SELECT ISNULL(SUM(Amount), 0) 
                                             FROM BankDeposit 
                                             WHERE BankAccountNo = @BankAccountNo 
                                             AND ReconcileDate IS NOT NULL";

                    SqlCommand reconciledCmd = new SqlCommand(reconciledQuery, conn);
                    reconciledCmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);
                    decimal reconciledBalance = Convert.ToDecimal(reconciledCmd.ExecuteScalar());

                    // Calculate UNRECONCILED balance (where ReconcileDate IS NULL)
                    string unreconciledQuery = @"SELECT ISNULL(SUM(Amount), 0) 
                                                FROM BankDeposit 
                                                WHERE BankAccountNo = @BankAccountNo 
                                                AND ReconcileDate IS NULL";

                    SqlCommand unreconciledCmd = new SqlCommand(unreconciledQuery, conn);
                    unreconciledCmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);
                    decimal unreconciledBalance = Convert.ToDecimal(unreconciledCmd.ExecuteScalar());

                    // Calculate TOTAL balance
                    decimal totalBalance = reconciledBalance + unreconciledBalance;

                    // Display balances
                    reconciledbalancetxtbox.Text = reconciledBalance.ToString("N2");
                    unreconciledbalancetxtbox.Text = unreconciledBalance.ToString("N2");
                    //totalbalancetxtbox.Text = totalBalance.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating balances: " + ex.Message);
            }
        }

        private void ClearBalanceFields()
        {
            reconciledbalancetxtbox.Clear();
            unreconciledbalancetxtbox.Clear();
            //totalbalancetxtbox.Clear();
        }

        private void availablecheckgrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < availablecheckgrid.Rows.Count)
            {
                DataGridViewRow row = availablecheckgrid.Rows[e.RowIndex];

                // Store selected voucher number
                if (row.Cells["VoucherNo"].Value != null)
                {
                    selectedVoucherNo = Convert.ToDecimal(row.Cells["VoucherNo"].Value);
                }
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (selectedVoucherNo == 0)
            {
                MessageBox.Show("Please select a deposit from the grid");
                return;
            }

            if (string.IsNullOrEmpty(selectedBankAccountNo))
            {
                MessageBox.Show("Please select a bank account first");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get current reconcile status
                    string checkQuery = @"SELECT ReconcileDate FROM BankDeposit 
                                        WHERE VoucherNo = @VoucherNo";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                    object currentDate = checkCmd.ExecuteScalar();

                    if (currentDate != DBNull.Value && currentDate != null)
                    {
                        // Already reconciled - ask to unreconcile
                        DialogResult result = MessageBox.Show(
                            "This deposit is already reconciled.\nDo you want to UNRECONCILE it?",
                            "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            UnreconcileDeposit(conn);
                        }
                    }
                    else
                    {
                        // Not reconciled - reconcile it
                        ReconcileDeposit(conn);
                    }

                    // Refresh grid and balances
                    LoadBankDeposits();
                    CalculateBalances();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ReconcileDeposit(SqlConnection conn)
        {
            // Set ReconcileDate to current date
            string updateQuery = @"UPDATE BankDeposit 
                                  SET ReconcileDate = GETDATE()
                                  WHERE VoucherNo = @VoucherNo";

            SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                // Also update BankCheques table if it's a cheque
                UpdateBankChequesReconcile(conn);

                MessageBox.Show("Deposit reconciled successfully!");
            }
        }

        private void UnreconcileDeposit(SqlConnection conn)
        {
            // Set ReconcileDate to NULL
            string updateQuery = @"UPDATE BankDeposit 
                                  SET ReconcileDate = NULL
                                  WHERE VoucherNo = @VoucherNo";

            SqlCommand cmd = new SqlCommand(updateQuery, conn);
            cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                // Also update BankCheques table if it's a cheque
                UpdateBankChequesUnreconcile(conn);

                MessageBox.Show("Deposit unreconciled successfully!");
            }
        }

        private void UpdateBankChequesReconcile(SqlConnection conn)
        {
            try
            {
                // Update BankCheques table as well
                string chequeQuery = @"UPDATE BankCheques 
                                      SET ReconcileDate = GETDATE()
                                      WHERE VoucherNo = @VoucherNo";

                SqlCommand chequeCmd = new SqlCommand(chequeQuery, conn);
                chequeCmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                chequeCmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore if cheque doesn't exist
            }
        }

        private void UpdateBankChequesUnreconcile(SqlConnection conn)
        {
            try
            {
                // Update BankCheques table as well
                string chequeQuery = @"UPDATE BankCheques 
                                      SET ReconcileDate = NULL
                                      WHERE VoucherNo = @VoucherNo";

                SqlCommand chequeCmd = new SqlCommand(chequeQuery, conn);
                chequeCmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                chequeCmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore if cheque doesn't exist
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            banknotxtbox.Clear();
            banknametxtbox.Clear();
            availablecheckgrid.Rows.Clear();
            reconciledbalancetxtbox.Clear();
            unreconciledbalancetxtbox.Clear();
            //totalbalancetxtbox.Clear();

            selectedVoucherNo = 0;
            selectedBankAccountNo = "";
        }

        //private void bouncedradiobtn_Click(object sender, EventArgs e)
        //{
          
        //}

        private void MarkChequeAsBounced(SqlConnection conn)
        {
            try
            {
                // Check if it's a cheque
                string checkQuery = @"SELECT IsCheque FROM BankDeposit 
                                    WHERE VoucherNo = @VoucherNo";

                SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                checkCmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                object result = checkCmd.ExecuteScalar();

                if (result != null && Convert.ToBoolean(result))
                {
                    // Update BankCheques table
                    string chequeQuery = @"UPDATE BankCheques 
                                          SET IsLost = 1
                                          WHERE VoucherNo = @VoucherNo";

                    SqlCommand chequeCmd = new SqlCommand(chequeQuery, conn);
                    chequeCmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                    chequeCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        private void Deposit_Reconciliation_Load(object sender, EventArgs e)
        {
            // Initialize form on load
            InitializeForm();
        }

        // Add these missing event handlers
        private void banknotxtbox_TextChanged(object sender, EventArgs e) { }
        private void banknametxtbox_TextChanged(object sender, EventArgs e) { }
        private void reconciledbalancetxtbox_TextChanged(object sender, EventArgs e) { }
        private void unreconciledbalancetxtbox_TextChanged(object sender, EventArgs e) { }
        private void availablecheckgrid_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void allradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (allradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankDeposits();
            }
        }

        private void reconciledradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (reconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankDeposits();
            }
        }

        private void unreconciledradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (unreconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankDeposits();
            }
        }

        private void bouncedradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (bouncedradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankDeposits();
            }
        }

        //private void bouncedradiobtn_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (selectedVoucherNo == 0)
        //    {
        //        MessageBox.Show("Please select a deposit from the grid");
        //        return;
        //    }

        //    if (MessageBox.Show("Mark this deposit as BOUNCED? This cannot be undone.",
        //        "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();

        //                // Mark as bounced in BankDeposit
        //                string updateQuery = @"UPDATE BankDeposit 
        //                                      SET IsBounced = 1
        //                                      WHERE VoucherNo = @VoucherNo";

        //                SqlCommand cmd = new SqlCommand(updateQuery, conn);
        //                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
        //                cmd.ExecuteNonQuery();

        //                // If it's a cheque, also update BankCheques
        //                MarkChequeAsBounced(conn);

        //                MessageBox.Show("Deposit marked as bounced!");

        //                // Refresh grid
        //                LoadBankDeposits();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: " + ex.Message);
        //        }
        //    }
        //}

        // Add new controls for better UI
        //private Label totalbalancelabel;
        //private TextBox totalbalancetxtbox;
        //private Button markbouncedbtn;

        //private void InitializeComponent()
        //{
        //    // Your existing initialization code
        //    // Add these controls:

        //    // Total Balance Label
        //    totalbalancelabel = new Label();
        //    totalbalancelabel.Text = "Total Balance:";
        //    totalbalancelabel.Location = new Point(400, 150);
        //    totalbalancelabel.Size = new Size(100, 20);
        //    this.Controls.Add(totalbalancelabel);

        //    // Total Balance Textbox
        //    totalbalancetxtbox = new TextBox();
        //    totalbalancetxtbox.Location = new Point(500, 150);
        //    totalbalancetxtbox.Size = new Size(120, 20);
        //    totalbalancetxtbox.ReadOnly = true;
        //    totalbalancetxtbox.BackColor = Color.LightYellow;
        //    this.Controls.Add(totalbalancetxtbox);

        //    // Mark Bounced Button
        //    markbouncedbtn = new Button();
        //    markbouncedbtn.Text = "Mark Bounced";
        //    markbouncedbtn.Location = new Point(400, 400);
        //    markbouncedbtn.Size = new Size(120, 30);
        //    markbouncedbtn.Click += markbouncedbtn_Click;
        //    this.Controls.Add(markbouncedbtn);
        //}
    }
}