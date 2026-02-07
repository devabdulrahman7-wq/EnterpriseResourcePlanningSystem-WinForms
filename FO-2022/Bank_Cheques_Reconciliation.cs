//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Bank_Cheques_Reconciliation : Form
//    {
//        private string connectionString;
//        private decimal selectedVoucherNo = 0;
//        private string selectedBankAccountNo = "";

//        public Bank_Cheques_Reconciliation()
//        {
//            InitializeComponent();

//            // Get connection string from config
//            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//            InitializeForm();
//        }

//        private void InitializeForm()
//        {
//            // Clear textboxes
//            banknotxtbox.Clear();
//            banknametxtbox.Clear();

//            // Setup grid
//            SetupGridColumns();

//            // Select "All" radio button by default
//            allradiobtn.Checked = true;
//        }

//        private void SetupGridColumns()
//        {
//            // Clear existing columns
//            availablechequegrid.Columns.Clear();

//            // Add only UNCOMMENTED columns
//            availablechequegrid.Columns.Add("VoucherNo", "V. No");
//            availablechequegrid.Columns.Add("VoucherDate", "Voucher Date");
//            availablechequegrid.Columns.Add("ChequeNo", "Cheque No");
//            availablechequegrid.Columns.Add("ChequeDate", "Cheque Date");
//            availablechequegrid.Columns.Add("Amount", "Amount");
//            availablechequegrid.Columns.Add("Narration", "Narration");
//            availablechequegrid.Columns.Add("ReconcileDate", "Reconcile Date");
//            availablechequegrid.Columns.Add("IsLost", "Lost");

//            // Hidden columns for internal use
//            availablechequegrid.Columns.Add("BankAccountNo", "Bank Account"); // Hidden
//            availablechequegrid.Columns.Add("AccountNo", "From Account"); // Hidden
//            availablechequegrid.Columns.Add("IsPosted", "Posted"); // Hidden

//            // Format columns
//            availablechequegrid.Columns["VoucherDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
//            availablechequegrid.Columns["ChequeDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
//            availablechequegrid.Columns["ReconcileDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
//            availablechequegrid.Columns["Amount"].DefaultCellStyle.Format = "N2";
//            availablechequegrid.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

//            // Hide internal columns
//            availablechequegrid.Columns["BankAccountNo"].Visible = false;
//            availablechequegrid.Columns["AccountNo"].Visible = false;
//            availablechequegrid.Columns["IsPosted"].Visible = false;

//            // Set column widths
//            availablechequegrid.Columns["VoucherNo"].Width = 60;
//            availablechequegrid.Columns["VoucherDate"].Width = 80;
//            availablechequegrid.Columns["ChequeNo"].Width = 90;
//            availablechequegrid.Columns["ChequeDate"].Width = 80;
//            availablechequegrid.Columns["Amount"].Width = 80;
//            availablechequegrid.Columns["Narration"].Width = 150;
//            availablechequegrid.Columns["ReconcileDate"].Width = 90;
//            availablechequegrid.Columns["IsLost"].Width = 50;
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        //private void accountnosearch_Click(object sender, EventArgs e)
//        //{
//        //    if (string.IsNullOrWhiteSpace(banknotxtbox.Text))
//        //    {
//        //        MessageBox.Show("Please enter bank account number");
//        //        return;
//        //    }

//        //    try
//        //    {
//        //        using (SqlConnection conn = new SqlConnection(connectionString))
//        //        {
//        //            conn.Open();

//        //            // Fetch bank name from ChartOfAccounts
//        //            string query = @"SELECT AccountName FROM ChartOfAccounts 
//        //                           WHERE AccountNo = @AccountNo";

//        //            SqlCommand cmd = new SqlCommand(query, conn);
//        //            cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

//        //            object result = cmd.ExecuteScalar();

//        //            if (result != null)
//        //            {
//        //                banknametxtbox.Text = result.ToString();
//        //                selectedBankAccountNo = banknotxtbox.Text.Trim();

//        //                // Load cheques for this bank account
//        //                LoadBankCheques();
//        //            }
//        //            else
//        //            {
//        //                MessageBox.Show("Bank account not found!");
//        //                banknametxtbox.Clear();
//        //                availablechequegrid.Rows.Clear();
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error fetching bank account: " + ex.Message);
//        //    }
//        //}

//        private void LoadBankCheques()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    // Build query based on selected radio button
//                    string whereClause = "WHERE BankAccountNo = @BankAccountNo";

//                    if (unreconciledradiobtn.Checked)
//                    {
//                        whereClause += " AND ReconcileDate IS NULL AND IsLost = 0";
//                    }
//                    else if (reconciledradiobtn.Checked)
//                    {
//                        whereClause += " AND ReconcileDate IS NOT NULL";
//                    }
//                    else if (lostradiobtn.Checked)
//                    {
//                        whereClause += " AND IsLost = 1";
//                    }
//                    // "All" radio button: no extra conditions

//                    string query = $@"SELECT 
//                                    VoucherNo,
//                                    VoucherDate,
//                                    ChequeNo,
//                                    ChequeDate,
//                                    BankAccountNo,
//                                    AccountNo,
//                                    Amount,
//                                    Narration,
//                                    ReconcileDate,
//                                    IsLost,
//                                    IsPosted
//                                    FROM BankCheques 
//                                    {whereClause}
//                                    ORDER BY VoucherDate DESC, ChequeDate DESC";

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        availablechequegrid.Rows.Clear();

//                        while (reader.Read())
//                        {
//                            int rowIndex = availablechequegrid.Rows.Add();
//                            DataGridViewRow row = availablechequegrid.Rows[rowIndex];

//                            // Fill visible columns
//                            row.Cells["VoucherNo"].Value = reader["VoucherNo"];
//                            row.Cells["VoucherDate"].Value = reader["VoucherDate"];
//                            row.Cells["ChequeNo"].Value = reader["ChequeNo"]?.ToString() ?? "";

//                            if (reader["ChequeDate"] != DBNull.Value)
//                            {
//                                row.Cells["ChequeDate"].Value = reader["ChequeDate"];
//                            }
//                            else
//                            {
//                                row.Cells["ChequeDate"].Value = "";
//                            }

//                            row.Cells["Amount"].Value = Convert.ToDecimal(reader["Amount"]);
//                            row.Cells["Narration"].Value = reader["Narration"]?.ToString() ?? "";

//                            if (reader["ReconcileDate"] != DBNull.Value)
//                            {
//                                row.Cells["ReconcileDate"].Value = reader["ReconcileDate"];
//                            }
//                            else
//                            {
//                                row.Cells["ReconcileDate"].Value = "";
//                            }

//                            bool isLost = Convert.ToBoolean(reader["IsLost"]);
//                            row.Cells["IsLost"].Value = isLost ? "YES" : "NO";

//                            // Fill hidden columns for internal use
//                            row.Cells["BankAccountNo"].Value = reader["BankAccountNo"];
//                            row.Cells["AccountNo"].Value = reader["AccountNo"]?.ToString() ?? "";

//                            bool isPosted = Convert.ToBoolean(reader["IsPosted"]);
//                            row.Cells["IsPosted"].Value = isPosted;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading cheques: " + ex.Message);
//            }
//        }

//        private void availablechequegrid_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.RowIndex >= 0 && e.RowIndex < availablechequegrid.Rows.Count)
//            {
//                DataGridViewRow row = availablechequegrid.Rows[e.RowIndex];

//                // Store selected voucher number
//                if (row.Cells["VoucherNo"].Value != null)
//                {
//                    selectedVoucherNo = Convert.ToDecimal(row.Cells["VoucherNo"].Value);
//                }
//            }
//        }

//        // Radio button change events - SIMPLE VERSION
//        private void unreconciledradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (unreconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
//            {
//                LoadBankCheques();
//            }
//        }

//        private void reconciledradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (reconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
//            {
//                LoadBankCheques();
//            }
//        }

//        private void lostradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (lostradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
//            {
//                LoadBankCheques();
//            }
//        }

//        private void allradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (allradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
//            {
//                LoadBankCheques();
//            }
//        }

//        //private void savebtn_Click(object sender, EventArgs e)
//        //{
//        //    if (selectedVoucherNo == 0)
//        //    {
//        //        MessageBox.Show("Please select a cheque from the grid");
//        //        return;
//        //    }

//        //    if (string.IsNullOrEmpty(selectedBankAccountNo))
//        //    {
//        //        MessageBox.Show("Please select a bank account first");
//        //        return;
//        //    }

//        //    // Get the selected cheque status from grid
//        //    DataGridViewRow selectedRow = null;
//        //    foreach (DataGridViewRow row in availablechequegrid.Rows)
//        //    {
//        //        if (row.Cells["VoucherNo"].Value != null &&
//        //            Convert.ToDecimal(row.Cells["VoucherNo"].Value) == selectedVoucherNo)
//        //        {
//        //            selectedRow = row;
//        //            break;
//        //        }
//        //    }

//        //    if (selectedRow == null)
//        //    {
//        //        MessageBox.Show("Selected cheque not found in grid");
//        //        return;
//        //    }

//        //    // Check current status
//        //    string reconcileDate = selectedRow.Cells["ReconcileDate"].Value?.ToString() ?? "";
//        //    string isLost = selectedRow.Cells["IsLost"].Value?.ToString() ?? "";

//        //    bool currentlyReconciled = !string.IsNullOrEmpty(reconcileDate);
//        //    bool currentlyLost = (isLost == "YES");

//        //    // Simple logic based on radio button selection
//        //    if (lostradiobtn.Checked)
//        //    {
//        //        // User is viewing Lost cheques - Allow to mark as Found
//        //        MarkChequeAsFound();
//        //    }
//        //    else if (reconciledradiobtn.Checked)
//        //    {
//        //        // User is viewing Reconciled cheques - Allow to Unreconcile
//        //        UnreconcileCheque();
//        //    }
//        //    else if (unreconciledradiobtn.Checked)
//        //    {
//        //        // User is viewing Unreconciled cheques - Show options
//        //        ShowUnreconciledOptions(currentlyLost);
//        //    }
//        //    else
//        //    {
//        //        // User is viewing All cheques - Show appropriate action
//        //        ShowAllOptions(currentlyReconciled, currentlyLost);
//        //    }
//        //}

//        //private void ShowUnreconciledOptions(bool currentlyLost)
//        //{
//        //    if (currentlyLost)
//        //    {
//        //        // Unreconciled and Lost - Mark as Found or Reconcile
//        //        SimpleOptionDialog dialog = new SimpleOptionDialog(
//        //            "Cheque is marked as LOST. What do you want to do?",
//        //            "Mark as Found",
//        //            "Reconcile"
//        //        );

//        //        if (dialog.ShowDialog() == DialogResult.OK)
//        //        {
//        //            if (dialog.SelectedOption == 1)
//        //            {
//        //                MarkChequeAsFound();
//        //            }
//        //            else
//        //            {
//        //                ReconcileCheque();
//        //            }
//        //        }
//        //    }
//        //    else
//        //    {
//        //        // Simple unreconciled - Reconcile or Mark as Lost
//        //        SimpleOptionDialog dialog = new SimpleOptionDialog(
//        //            "What do you want to do with this cheque?",
//        //            "Reconcile",
//        //            "Mark as Lost"
//        //        );

//        //        if (dialog.ShowDialog() == DialogResult.OK)
//        //        {
//        //            if (dialog.SelectedOption == 1)
//        //            {
//        //                ReconcileCheque();
//        //            }
//        //            else
//        //            {
//        //                MarkChequeAsLost();
//        //            }
//        //        }
//        //    }
//        //}
//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (selectedVoucherNo == 0)
//            {
//                MessageBox.Show("Please select a cheque from the grid");
//                return;
//            }

//            if (string.IsNullOrEmpty(selectedBankAccountNo))
//            {
//                MessageBox.Show("Please select a bank account first");
//                return;
//            }

//            // Get selected row
//            DataGridViewRow selectedRow = null;
//            foreach (DataGridViewRow row in availablechequegrid.Rows)
//            {
//                if (row.Cells["VoucherNo"].Value != null &&
//                    Convert.ToDecimal(row.Cells["VoucherNo"].Value) == selectedVoucherNo)
//                {
//                    selectedRow = row;
//                    break;
//                }
//            }

//            if (selectedRow == null)
//            {
//                MessageBox.Show("Selected cheque not found in grid");
//                return;
//            }

//            // DIRECT ACTION BASED ON RADIO BUTTON - NO QUESTIONS!

//            if (lostradiobtn.Checked)
//            {
//                // User is viewing LOST cheques → Mark as FOUND
//                MarkChequeAsFound();
//            }
//            else if (reconciledradiobtn.Checked)
//            {
//                // User is viewing RECONCILED cheques → UNRECONCILE
//                UnreconcileCheque();
//            }
//            else if (unreconciledradiobtn.Checked)
//            {
//                // User is viewing UNRECONCILED cheques → RECONCILE
//                ReconcileCheque();
//            }
//            else
//            {
//                // User is viewing ALL cheques → Check status and decide

//                string reconcileDate = selectedRow.Cells["ReconcileDate"].Value?.ToString() ?? "";
//                string isLost = selectedRow.Cells["IsLost"].Value?.ToString() ?? "";

//                bool currentlyReconciled = !string.IsNullOrEmpty(reconcileDate);
//                bool currentlyLost = (isLost == "YES");

//                if (currentlyLost)
//                {
//                    // Cheque is LOST → Mark as FOUND
//                    MarkChequeAsFound();
//                }
//                else if (currentlyReconciled)
//                {
//                    // Cheque is RECONCILED → UNRECONCILE
//                    UnreconcileCheque();
//                }
//                else
//                {
//                    // Cheque is UNRECONCILED → RECONCILE
//                    ReconcileCheque();
//                }
//            }
//        }

//        private void ShowAllOptions(bool currentlyReconciled, bool currentlyLost)
//        {
//            if (currentlyLost)
//            {
//                SimpleOptionDialog dialog = new SimpleOptionDialog(
//                    "Cheque is LOST. What do you want to do?",
//                    "Mark as Found",
//                    "Reconcile"
//                );

//                if (dialog.ShowDialog() == DialogResult.OK)
//                {
//                    if (dialog.SelectedOption == 1)
//                    {
//                        MarkChequeAsFound();
//                    }
//                    else
//                    {
//                        ReconcileCheque();
//                    }
//                }
//            }
//            else if (currentlyReconciled)
//            {
//                SimpleOptionDialog dialog = new SimpleOptionDialog(
//                    "Cheque is RECONCILED. What do you want to do?",
//                    "Unreconcile",
//                    "Mark as Lost"
//                );

//                if (dialog.ShowDialog() == DialogResult.OK)
//                {
//                    if (dialog.SelectedOption == 1)
//                    {
//                        UnreconcileCheque();
//                    }
//                    else
//                    {
//                        MarkChequeAsLost();
//                    }
//                }
//            }
//            else
//            {
//                // Not lost, not reconciled
//                SimpleOptionDialog dialog = new SimpleOptionDialog(
//                    "What do you want to do with this cheque?",
//                    "Reconcile",
//                    "Mark as Lost"
//                );

//                if (dialog.ShowDialog() == DialogResult.OK)
//                {
//                    if (dialog.SelectedOption == 1)
//                    {
//                        ReconcileCheque();
//                    }
//                    else
//                    {
//                        MarkChequeAsLost();
//                    }
//                }
//            }
//        }

//        private void ReconcileCheque()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string updateQuery = @"UPDATE BankCheques 
//                                          SET ReconcileDate = GETDATE(),
//                                              IsLost = 0
//                                          WHERE VoucherNo = @VoucherNo";

//                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

//                    int rowsAffected = cmd.ExecuteNonQuery();

//                    if (rowsAffected > 0)
//                    {
//                        // Also update BankDeposit table
//                        UpdateBankDepositReconcile(conn);
//                        MessageBox.Show("Cheque reconciled successfully!");
//                        LoadBankCheques();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error reconciling cheque: " + ex.Message);
//            }
//        }

//        private void UnreconcileCheque()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string updateQuery = @"UPDATE BankCheques 
//                                          SET ReconcileDate = NULL
//                                          WHERE VoucherNo = @VoucherNo";

//                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

//                    int rowsAffected = cmd.ExecuteNonQuery();

//                    if (rowsAffected > 0)
//                    {
//                        // Also update BankDeposit table
//                        UpdateBankDepositUnreconcile(conn);
//                        MessageBox.Show("Cheque unreconciled successfully!");
//                        LoadBankCheques();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error unreconciling cheque: " + ex.Message);
//            }
//        }

//        private void MarkChequeAsLost()
//        {
//            if (MessageBox.Show("Are you sure you want to mark this cheque as LOST?",
//                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
//            {
//                try
//                {
//                    using (SqlConnection conn = new SqlConnection(connectionString))
//                    {
//                        conn.Open();

//                        string updateQuery = @"UPDATE BankCheques 
//                                              SET IsLost = 1,
//                                                  ReconcileDate = NULL
//                                              WHERE VoucherNo = @VoucherNo";

//                        SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                        cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

//                        int rowsAffected = cmd.ExecuteNonQuery();

//                        if (rowsAffected > 0)
//                        {
//                            // Also update BankDeposit table
//                            UpdateBankDepositBounced(conn);
//                            MessageBox.Show("Cheque marked as lost!");
//                            LoadBankCheques();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Error marking cheque as lost: " + ex.Message);
//                }
//            }
//        }

//        private void MarkChequeAsFound()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string updateQuery = @"UPDATE BankCheques 
//                                          SET IsLost = 0
//                                          WHERE VoucherNo = @VoucherNo";

//                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

//                    int rowsAffected = cmd.ExecuteNonQuery();

//                    if (rowsAffected > 0)
//                    {
//                        MessageBox.Show("Cheque marked as found!");
//                        LoadBankCheques();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error marking cheque as found: " + ex.Message);
//            }
//        }

//        private void UpdateBankDepositReconcile(SqlConnection conn)
//        {
//            try
//            {
//                string updateQuery = @"UPDATE BankDeposit 
//                                      SET ReconcileDate = GETDATE(),
//                                          IsBounced = 0
//                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

//                SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
//                cmd.ExecuteNonQuery();
//            }
//            catch
//            {
//                // Ignore if not in BankDeposit
//            }
//        }

//        private void UpdateBankDepositUnreconcile(SqlConnection conn)
//        {
//            try
//            {
//                string updateQuery = @"UPDATE BankDeposit 
//                                      SET ReconcileDate = NULL
//                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

//                SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
//                cmd.ExecuteNonQuery();
//            }
//            catch
//            {
//                // Ignore if not in BankDeposit
//            }
//        }

//        private void UpdateBankDepositBounced(SqlConnection conn)
//        {
//            try
//            {
//                string updateQuery = @"UPDATE BankDeposit 
//                                      SET IsBounced = 1,
//                                          ReconcileDate = NULL
//                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

//                SqlCommand cmd = new SqlCommand(updateQuery, conn);
//                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
//                cmd.ExecuteNonQuery();
//            }
//            catch
//            {
//                // Ignore if not in BankDeposit
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//        }

//        private void ClearForm()
//        {
//            banknotxtbox.Clear();
//            banknametxtbox.Clear();
//            availablechequegrid.Rows.Clear();

//            selectedVoucherNo = 0;
//            selectedBankAccountNo = "";

//            // Reset radio buttons
//            allradiobtn.Checked = true;
//        }

//        private void Bank_Cheques_Reconciliation_Load(object sender, EventArgs e)
//        {
//            // Initialize form on load
//            InitializeForm();
//        }

//        // Simple Option Dialog Class
//        public class SimpleOptionDialog : Form
//        {
//            private Button option1Btn;
//            private Button option2Btn;
//            private Button cancelBtn;
//            private Label messageLabel;

//            public int SelectedOption { get; private set; }

//            public SimpleOptionDialog(string message, string option1Text, string option2Text)
//            {
//                //InitializeDialog(message, option1Text, option2Text);
//            }

//            private void InitializeDialog(string message, string option1Text, string option2Text)
//            {
//                this.Text = "Select Action";
//                this.Size = new Size(350, 180);
//                this.FormBorderStyle = FormBorderStyle.FixedDialog;
//                this.StartPosition = FormStartPosition.CenterParent;
//                this.MaximizeBox = false;
//                this.MinimizeBox = false;

//                // Message label
//                messageLabel = new Label();
//                messageLabel.Text = message;
//                messageLabel.Location = new Point(20, 20);
//                messageLabel.Size = new Size(300, 50);
//                messageLabel.AutoSize = false;
//                messageLabel.TextAlign = ContentAlignment.MiddleLeft;

//                // Option 1 button
//                option1Btn = new Button();
//                option1Btn.Text = option1Text;
//                option1Btn.Location = new Point(20, 90);
//                option1Btn.Size = new Size(90, 30);
//                option1Btn.Click += (s, e) =>
//                {
//                    SelectedOption = 1;
//                    this.DialogResult = DialogResult.OK;
//                    this.Close();
//                };

//                // Option 2 button
//                option2Btn = new Button();
//                option2Btn.Text = option2Text;
//                option2Btn.Location = new Point(120, 90);
//                option2Btn.Size = new Size(90, 30);
//                option2Btn.Click += (s, e) =>
//                {
//                    SelectedOption = 2;
//                    this.DialogResult = DialogResult.OK;
//                    this.Close();
//                };

//                // Cancel button
//                cancelBtn = new Button();
//                cancelBtn.Text = "Cancel";
//                cancelBtn.Location = new Point(220, 90);
//                cancelBtn.Size = new Size(90, 30);
//                cancelBtn.Click += (s, e) =>
//                {
//                    this.DialogResult = DialogResult.Cancel;
//                    this.Close();
//                };

//                this.Controls.Add(messageLabel);
//                this.Controls.Add(option1Btn);
//                this.Controls.Add(option2Btn);
//                this.Controls.Add(cancelBtn);
//            }
//        }

//        // Empty event handlers
//        private void banknotxtbox_TextChanged(object sender, EventArgs e) { }
//        private void banknametxtbox_TextChanged(object sender, EventArgs e) { }
//        private void availablechequegrid_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
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
    public partial class Bank_Cheques_Reconciliation : Form
    {
        private string connectionString;
        private decimal selectedVoucherNo = 0;
        private string selectedBankAccountNo = "";
        private decimal totalReconcileBalance = 0;
        private decimal totalUnreconcileBalance = 0;

        public Bank_Cheques_Reconciliation()
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

            // Select "All" radio button by default
            allradiobtn.Checked = true;
        }

        private void SetupGridColumns()
        {
            // Clear existing columns
            availablechequegrid.Columns.Clear();

            // Add only UNCOMMENTED columns
            availablechequegrid.Columns.Add("VoucherNo", "V. No");
            availablechequegrid.Columns.Add("VoucherDate", "Voucher Date");
            availablechequegrid.Columns.Add("ChequeNo", "Cheque No");
            availablechequegrid.Columns.Add("ChequeDate", "Cheque Date");
            availablechequegrid.Columns.Add("Amount", "Amount");
            availablechequegrid.Columns.Add("Narration", "Narration");
            availablechequegrid.Columns.Add("ReconcileDate", "Reconcile Date");
            availablechequegrid.Columns.Add("IsLost", "Lost");

            // Hidden columns for internal use
            availablechequegrid.Columns.Add("BankAccountNo", "Bank Account"); // Hidden
            availablechequegrid.Columns.Add("AccountNo", "From Account"); // Hidden
            availablechequegrid.Columns.Add("IsPosted", "Posted"); // Hidden

            // Format columns
            availablechequegrid.Columns["VoucherDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablechequegrid.Columns["ChequeDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablechequegrid.Columns["ReconcileDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            availablechequegrid.Columns["Amount"].DefaultCellStyle.Format = "N2";
            availablechequegrid.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Hide internal columns
            availablechequegrid.Columns["BankAccountNo"].Visible = false;
            availablechequegrid.Columns["AccountNo"].Visible = false;
            availablechequegrid.Columns["IsPosted"].Visible = false;

            // Set column widths
            availablechequegrid.Columns["VoucherNo"].Width = 60;
            availablechequegrid.Columns["VoucherDate"].Width = 80;
            availablechequegrid.Columns["ChequeNo"].Width = 90;
            availablechequegrid.Columns["ChequeDate"].Width = 80;
            availablechequegrid.Columns["Amount"].Width = 80;
            availablechequegrid.Columns["Narration"].Width = 150;
            availablechequegrid.Columns["ReconcileDate"].Width = 90;
            availablechequegrid.Columns["IsLost"].Width = 50;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void accountnosearch_Click(object sender, EventArgs e)
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

                    // Fetch bank name from ChartOfAccounts
                    string query = @"SELECT AccountName FROM ChartOfAccounts 
                                   WHERE AccountNo = @AccountNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        banknametxtbox.Text = result.ToString();
                        selectedBankAccountNo = banknotxtbox.Text.Trim();

                        // Load cheques for this bank account
                        LoadBankCheques();

                        // Calculate balances
                        CalculateBalances();
                    }
                    else
                    {
                        MessageBox.Show("Bank account not found!");
                        banknametxtbox.Clear();
                        availablechequegrid.Rows.Clear();
                        reconciledbalancetxtbox.Clear();
                        unreconciledbalancetxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bank account: " + ex.Message);
            }
        }

        private void LoadBankCheques()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Build query based on selected radio button
                    string whereClause = "WHERE BankAccountNo = @BankAccountNo";

                    if (unreconciledradiobtn.Checked)
                    {
                        whereClause += " AND ReconcileDate IS NULL AND IsLost = 0";
                    }
                    else if (reconciledradiobtn.Checked)
                    {
                        whereClause += " AND ReconcileDate IS NOT NULL";
                    }
                    else if (lostradiobtn.Checked)
                    {
                        whereClause += " AND IsLost = 1";
                    }
                    // "All" radio button: no extra conditions

                    string query = $@"SELECT 
                                    VoucherNo,
                                    VoucherDate,
                                    ChequeNo,
                                    ChequeDate,
                                    BankAccountNo,
                                    AccountNo,
                                    Amount,
                                    Narration,
                                    ReconcileDate,
                                    IsLost,
                                    IsPosted
                                    FROM BankCheques 
                                    {whereClause}
                                    ORDER BY VoucherDate DESC, ChequeDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        availablechequegrid.Rows.Clear();

                        while (reader.Read())
                        {
                            int rowIndex = availablechequegrid.Rows.Add();
                            DataGridViewRow row = availablechequegrid.Rows[rowIndex];

                            // Fill visible columns
                            row.Cells["VoucherNo"].Value = reader["VoucherNo"];
                            row.Cells["VoucherDate"].Value = reader["VoucherDate"];
                            row.Cells["ChequeNo"].Value = reader["ChequeNo"]?.ToString() ?? "";

                            if (reader["ChequeDate"] != DBNull.Value)
                            {
                                row.Cells["ChequeDate"].Value = reader["ChequeDate"];
                            }
                            else
                            {
                                row.Cells["ChequeDate"].Value = "";
                            }

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

                            bool isLost = Convert.ToBoolean(reader["IsLost"]);
                            row.Cells["IsLost"].Value = isLost ? "YES" : "NO";

                            // Fill hidden columns for internal use
                            row.Cells["BankAccountNo"].Value = reader["BankAccountNo"];
                            row.Cells["AccountNo"].Value = reader["AccountNo"]?.ToString() ?? "";

                            bool isPosted = Convert.ToBoolean(reader["IsPosted"]);
                            row.Cells["IsPosted"].Value = isPosted;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cheques: " + ex.Message);
            }
        }

        private void CalculateBalances()
        {
            totalReconcileBalance = 0;
            totalUnreconcileBalance = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Calculate reconciled balance (ReconcileDate IS NOT NULL AND IsLost = 0)
                    string reconciledQuery = @"SELECT ISNULL(SUM(Amount), 0) 
                                              FROM BankCheques 
                                              WHERE BankAccountNo = @BankAccountNo 
                                              AND ReconcileDate IS NOT NULL 
                                              AND IsLost = 0";

                    SqlCommand reconciledCmd = new SqlCommand(reconciledQuery, conn);
                    reconciledCmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);
                    object reconciledResult = reconciledCmd.ExecuteScalar();
                    totalReconcileBalance = reconciledResult != DBNull.Value ? Convert.ToDecimal(reconciledResult) : 0;

                    // Calculate unreconciled balance (ReconcileDate IS NULL AND IsLost = 0)
                    string unreconciledQuery = @"SELECT ISNULL(SUM(Amount), 0) 
                                                FROM BankCheques 
                                                WHERE BankAccountNo = @BankAccountNo 
                                                AND ReconcileDate IS NULL 
                                                AND IsLost = 0";

                    SqlCommand unreconciledCmd = new SqlCommand(unreconciledQuery, conn);
                    unreconciledCmd.Parameters.AddWithValue("@BankAccountNo", selectedBankAccountNo);
                    object unreconciledResult = unreconciledCmd.ExecuteScalar();
                    totalUnreconcileBalance = unreconciledResult != DBNull.Value ? Convert.ToDecimal(unreconciledResult) : 0;

                    // Display in textboxes
                    reconciledbalancetxtbox.Text = totalReconcileBalance.ToString("N2");
                    unreconciledbalancetxtbox.Text = totalUnreconcileBalance.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating balances: " + ex.Message);
                reconciledbalancetxtbox.Clear();
                unreconciledbalancetxtbox.Clear();
            }
        }

        private void availablechequegrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < availablechequegrid.Rows.Count)
            {
                DataGridViewRow row = availablechequegrid.Rows[e.RowIndex];

                // Store selected voucher number
                if (row.Cells["VoucherNo"].Value != null)
                {
                    selectedVoucherNo = Convert.ToDecimal(row.Cells["VoucherNo"].Value);
                }
            }
        }

        // Radio button change events
        private void unreconciledradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (unreconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankCheques();
                CalculateBalances();
            }
        }

        private void reconciledradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (reconciledradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankCheques();
                CalculateBalances();
            }
        }

        private void lostradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (lostradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankCheques();
                CalculateBalances();
            }
        }

        private void allradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (allradiobtn.Checked && !string.IsNullOrEmpty(selectedBankAccountNo))
            {
                LoadBankCheques();
                CalculateBalances();
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (selectedVoucherNo == 0)
            {
                MessageBox.Show("Please select a cheque from the grid");
                return;
            }

            if (string.IsNullOrEmpty(selectedBankAccountNo))
            {
                MessageBox.Show("Please select a bank account first");
                return;
            }

            // Get selected row
            DataGridViewRow selectedRow = null;
            foreach (DataGridViewRow row in availablechequegrid.Rows)
            {
                if (row.Cells["VoucherNo"].Value != null &&
                    Convert.ToDecimal(row.Cells["VoucherNo"].Value) == selectedVoucherNo)
                {
                    selectedRow = row;
                    break;
                }
            }

            if (selectedRow == null)
            {
                MessageBox.Show("Selected cheque not found in grid");
                return;
            }

            // DIRECT ACTION BASED ON RADIO BUTTON - NO QUESTIONS!

            if (lostradiobtn.Checked)
            {
                // User is viewing LOST cheques → Mark as FOUND
                MarkChequeAsFound();
            }
            else if (reconciledradiobtn.Checked)
            {
                // User is viewing RECONCILED cheques → UNRECONCILE
                UnreconcileCheque();
            }
            else if (unreconciledradiobtn.Checked)
            {
                // User is viewing UNRECONCILED cheques → RECONCILE
                ReconcileCheque();
            }
            else
            {
                // User is viewing ALL cheques → Check status and decide

                string reconcileDate = selectedRow.Cells["ReconcileDate"].Value?.ToString() ?? "";
                string isLost = selectedRow.Cells["IsLost"].Value?.ToString() ?? "";

                bool currentlyReconciled = !string.IsNullOrEmpty(reconcileDate);
                bool currentlyLost = (isLost == "YES");

                if (currentlyLost)
                {
                    // Cheque is LOST → Mark as FOUND
                    MarkChequeAsFound();
                }
                else if (currentlyReconciled)
                {
                    // Cheque is RECONCILED → UNRECONCILE
                    UnreconcileCheque();
                }
                else
                {
                    // Cheque is UNRECONCILED → RECONCILE
                    ReconcileCheque();
                }
            }
        }

        private void ReconcileCheque()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"UPDATE BankCheques 
                                          SET ReconcileDate = GETDATE(),
                                              IsLost = 0
                                          WHERE VoucherNo = @VoucherNo";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Also update BankDeposit table
                        UpdateBankDepositReconcile(conn);
                        MessageBox.Show("Cheque reconciled successfully!");

                        // Refresh data and calculate balances
                        LoadBankCheques();
                        CalculateBalances();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reconciling cheque: " + ex.Message);
            }
        }

        private void UnreconcileCheque()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"UPDATE BankCheques 
                                          SET ReconcileDate = NULL
                                          WHERE VoucherNo = @VoucherNo";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Also update BankDeposit table
                        UpdateBankDepositUnreconcile(conn);
                        MessageBox.Show("Cheque unreconciled successfully!");

                        // Refresh data and calculate balances
                        LoadBankCheques();
                        CalculateBalances();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error unreconciling cheque: " + ex.Message);
            }
        }

        private void MarkChequeAsLost()
        {
            if (MessageBox.Show("Are you sure you want to mark this cheque as LOST?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string updateQuery = @"UPDATE BankCheques 
                                              SET IsLost = 1,
                                                  ReconcileDate = NULL
                                              WHERE VoucherNo = @VoucherNo";

                        SqlCommand cmd = new SqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Also update BankDeposit table
                            UpdateBankDepositBounced(conn);
                            MessageBox.Show("Cheque marked as lost!");

                            // Refresh data and calculate balances
                            LoadBankCheques();
                            CalculateBalances();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error marking cheque as lost: " + ex.Message);
                }
            }
        }

        private void MarkChequeAsFound()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"UPDATE BankCheques 
                                          SET IsLost = 0
                                          WHERE VoucherNo = @VoucherNo";

                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        //MessageBox.Show("Cheque marked as found!");

                        // Refresh data and calculate balances
                        LoadBankCheques();
                        CalculateBalances();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error marking cheque as found: " + ex.Message);
            }
        }

        private void UpdateBankDepositReconcile(SqlConnection conn)
        {
            try
            {
                string updateQuery = @"UPDATE BankDeposit 
                                      SET ReconcileDate = GETDATE(),
                                          IsBounced = 0
                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore if not in BankDeposit
            }
        }

        private void UpdateBankDepositUnreconcile(SqlConnection conn)
        {
            try
            {
                string updateQuery = @"UPDATE BankDeposit 
                                      SET ReconcileDate = NULL
                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore if not in BankDeposit
            }
        }

        private void UpdateBankDepositBounced(SqlConnection conn)
        {
            try
            {
                string updateQuery = @"UPDATE BankDeposit 
                                      SET IsBounced = 1,
                                          ReconcileDate = NULL
                                      WHERE VoucherNo = @VoucherNo AND IsCheque = 1";

                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@VoucherNo", selectedVoucherNo);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore if not in BankDeposit
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
            availablechequegrid.Rows.Clear();
            reconciledbalancetxtbox.Clear();
            unreconciledbalancetxtbox.Clear();

            selectedVoucherNo = 0;
            selectedBankAccountNo = "";
            totalReconcileBalance = 0;
            totalUnreconcileBalance = 0;

            // Reset radio buttons
            allradiobtn.Checked = true;
        }

        private void Bank_Cheques_Reconciliation_Load(object sender, EventArgs e)
        {
            // Initialize form on load
            InitializeForm();
        }

        // Simple Option Dialog Class
        public class SimpleOptionDialog : Form
        {
            private Button option1Btn;
            private Button option2Btn;
            private Button cancelBtn;
            private Label messageLabel;

            public int SelectedOption { get; private set; }

            public SimpleOptionDialog(string message, string option1Text, string option2Text)
            {
                InitializeDialog(message, option1Text, option2Text);
            }

            private void InitializeDialog(string message, string option1Text, string option2Text)
            {
                this.Text = "Select Action";
                this.Size = new Size(350, 180);
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MaximizeBox = false;
                this.MinimizeBox = false;

                // Message label
                messageLabel = new Label();
                messageLabel.Text = message;
                messageLabel.Location = new Point(20, 20);
                messageLabel.Size = new Size(300, 50);
                messageLabel.AutoSize = false;
                messageLabel.TextAlign = ContentAlignment.MiddleLeft;

                // Option 1 button
                option1Btn = new Button();
                option1Btn.Text = option1Text;
                option1Btn.Location = new Point(20, 90);
                option1Btn.Size = new Size(90, 30);
                option1Btn.Click += (s, e) =>
                {
                    SelectedOption = 1;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                // Option 2 button
                option2Btn = new Button();
                option2Btn.Text = option2Text;
                option2Btn.Location = new Point(120, 90);
                option2Btn.Size = new Size(90, 30);
                option2Btn.Click += (s, e) =>
                {
                    SelectedOption = 2;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };

                // Cancel button
                cancelBtn = new Button();
                cancelBtn.Text = "Cancel";
                cancelBtn.Location = new Point(220, 90);
                cancelBtn.Size = new Size(90, 30);
                cancelBtn.Click += (s, e) =>
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                };

                this.Controls.Add(messageLabel);
                this.Controls.Add(option1Btn);
                this.Controls.Add(option2Btn);
                this.Controls.Add(cancelBtn);
            }
        }

        // Empty event handlers
        private void banknotxtbox_TextChanged(object sender, EventArgs e) { }
        private void banknametxtbox_TextChanged(object sender, EventArgs e) { }
        private void availablechequegrid_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}