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

//namespace FO_2022
//{
//    public partial class Promises_Processing : Form
//    {
//        private SqlConnection connection;
//        private DataTable promisesDataTable;
//        private int selectedPromiseNo = 0;
//        private decimal originalAmount = 0;
//        private DateTime originalPromiseDate;
//        private bool isLoading = false;
//        private string currentUserNo = "1"; // Replace with actual user session

//        public Promises_Processing()
//        {
//            InitializeComponent();
//            InitializeConnection();
//        }

//        private void InitializeConnection()
//        {
//            try
//            {
//                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//                connection = new SqlConnection(connectionString);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error initializing database connection: " + ex.Message,
//                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void Promises_Processing_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                // Set default dates
//                newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
//                promisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

//                // Load ALL unprocessed promises
//                LoadAllUnprocessedPromises();

//                // Initialize form controls state
//                ResetForm();
//                SetControlsReadOnly(true);

//                // Select default radio button
//                radioButtonprocessrecovery.Checked = true;
//                fullprocessradiobtn.Checked = true;

//                // Focus on grid
//                dataGridView1.Focus();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading form: " + ex.Message,
//                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Load ALL unprocessed promises
//        private void LoadAllUnprocessedPromises()
//        {
//            try
//            {
//                Cursor = Cursors.WaitCursor;

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                // Query to get ALL unprocessed promises with only 3 columns
//                string query = @"SELECT 
//                                CONVERT(varchar, p.PromiseDate, 103) as PromiseDate,
//                                c.AccountName, 
//                                p.Amount,
//                                p.PromiseNo,
//                                p.AccountNo,
//                                p.Narration,
//                                p.IsPayment,
//                                p.IsPosted
//                                FROM Promises p
//                                LEFT JOIN ChartOfAccounts c ON p.AccountNo = c.AccountNo
//                                WHERE p.IsPosted = 0
//                                ORDER BY p.PromiseDate, p.PromiseNo";

//                SqlCommand cmd = new SqlCommand(query, connection);

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                promisesDataTable = new DataTable();
//                adapter.Fill(promisesDataTable);

//                // Bind to DataGridView
//                dataGridView1.DataSource = promisesDataTable;

//                // Format columns - show only 3 columns
//                FormatDataGridView();

//                // If no data found, show message
//                if (promisesDataTable.Rows.Count == 0)
//                {
//                    MessageBox.Show("No unprocessed promises found in database",
//                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading promises: " + ex.Message,
//                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//                Cursor = Cursors.Default;
//            }
//        }

//        private void FormatDataGridView()
//        {
//            try
//            {
//                if (dataGridView1.Columns.Count > 0)
//                {
//                    // Hide all columns except the 3 we want to show
//                    foreach (DataGridViewColumn column in dataGridView1.Columns)
//                    {
//                        // Show only these 3 columns
//                        if (column.Name == "PromiseDate" || column.Name == "AccountName" || column.Name == "Amount")
//                        {
//                            column.Visible = true;

//                            // Set headers
//                            if (column.Name == "PromiseDate")
//                                column.HeaderText = "Promise Date";
//                            else if (column.Name == "AccountName")
//                                column.HeaderText = "Account Name";
//                            else if (column.Name == "Amount")
//                            {
//                                column.HeaderText = "Amount";
//                                column.DefaultCellStyle.Format = "N2";
//                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//                            }
//                        }
//                        else
//                        {
//                            // Hide other columns but keep them for data access
//                            column.Visible = false;
//                        }
//                    }

//                    // Auto-size columns
//                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

//                    // Set selection mode
//                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//                    dataGridView1.MultiSelect = false;

//                    // Set row height
//                    dataGridView1.RowTemplate.Height = 25;
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Error formatting grid: " + ex.Message);
//            }
//        }

//        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            try
//            {
//                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
//                {
//                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

//                    isLoading = true;

//                    // Safely get values from hidden columns
//                    selectedPromiseNo = row.Cells["PromiseNo"].Value != DBNull.Value ?
//                        Convert.ToInt32(row.Cells["PromiseNo"].Value) : 0;

//                    promisesidtxtbox.Text = selectedPromiseNo > 0 ? selectedPromiseNo.ToString() : "";

//                    accountnotxtbox.Text = row.Cells["AccountNo"].Value != DBNull.Value ?
//                        row.Cells["AccountNo"].Value.ToString() : "";

//                    accountnametxtbox.Text = row.Cells["AccountName"].Value != DBNull.Value ?
//                        row.Cells["AccountName"].Value.ToString() : "";

//                    promisedatetxtbox.Text = row.Cells["PromiseDate"].Value != DBNull.Value ?
//                        row.Cells["PromiseDate"].Value.ToString() : "";

//                    amounttxtbox.Text = row.Cells["Amount"].Value != DBNull.Value ?
//                        Convert.ToDecimal(row.Cells["Amount"].Value).ToString("N2") : "0.00";

//                    narrationtxtbox.Text = row.Cells["Narration"].Value != DBNull.Value ?
//                        row.Cells["Narration"].Value.ToString() : "";

//                    // Store original values for validation
//                    originalAmount = row.Cells["Amount"].Value != DBNull.Value ?
//                        Convert.ToDecimal(row.Cells["Amount"].Value) : 0;

//                    if (row.Cells["PromiseDate"].Value != DBNull.Value &&
//                        DateTime.TryParse(row.Cells["PromiseDate"].Value.ToString(), out DateTime promiseDate))
//                    {
//                        originalPromiseDate = promiseDate;
//                    }
//                    else
//                    {
//                        originalPromiseDate = DateTime.Now;
//                    }

//                    // Set initial values for processing
//                    paidamounttxtbox.Text = originalAmount.ToString("N2");
//                    newpromisedatetxtbox.Text = promisedatetxtbox.Text;

//                    // Enable process controls
//                    EnableProcessControls(true);

//                    isLoading = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                isLoading = false;
//                MessageBox.Show("Error selecting promise: " + ex.Message,
//                    "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // No specific action needed for content click
//        }

//        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
//        {
//            // No validation needed for read-only grid
//        }

//        private void radioButtonprocesspayment_CheckedChanged(object sender, EventArgs e)
//        {
//            if (radioButtonprocesspayment.Checked && !isLoading)
//            {
//                FilterGridByPaymentType(true);
//            }
//        }

//        private void radioButtonprocessrecovery_CheckedChanged(object sender, EventArgs e)
//        {
//            if (radioButtonprocessrecovery.Checked && !isLoading)
//            {
//                FilterGridByPaymentType(false);
//            }
//        }

//        private void FilterGridByPaymentType(bool isPayment)
//        {
//            try
//            {
//                if (promisesDataTable != null && promisesDataTable.Rows.Count > 0)
//                {
//                    DataView dv = new DataView(promisesDataTable);
//                    dv.RowFilter = $"IsPayment = {(isPayment ? "1" : "0")}";
//                    dataGridView1.DataSource = dv.ToTable();
//                    FormatDataGridView();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error filtering data: " + ex.Message,
//                    "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
//        }

//        private void fullprocessradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (fullprocessradiobtn.Checked && !isLoading)
//            {
//                paidamounttxtbox.Text = originalAmount.ToString("N2");
//                paidamounttxtbox.ReadOnly = true;
//                newpromisedatetxtbox.ReadOnly = true;
//            }
//        }

//        private void partialprocessradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (partialprocessradiobtn.Checked && !isLoading)
//            {
//                paidamounttxtbox.Text = "0.00";
//                paidamounttxtbox.ReadOnly = false;
//                newpromisedatetxtbox.ReadOnly = true;
//                paidamounttxtbox.Focus();
//                paidamounttxtbox.SelectAll();
//            }
//        }

//        private void postponeprocessradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (postponeprocessradiobtn.Checked && !isLoading)
//            {
//                paidamounttxtbox.Text = "0.00";
//                paidamounttxtbox.ReadOnly = true;
//                newpromisedatetxtbox.ReadOnly = false;
//                newpromisedatetxtbox.Focus();
//                newpromisedatetxtbox.SelectAll();
//            }
//        }

//        private void amounttxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Read-only field, no action needed
//        }

//        private void paidamounttxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Validate and format paid amount
//            if (!isLoading && !string.IsNullOrEmpty(paidamounttxtbox.Text))
//            {
//                if (decimal.TryParse(paidamounttxtbox.Text.Replace(",", ""), out decimal paidAmount))
//                {
//                    // Validate against original amount
//                    if (paidAmount > originalAmount)
//                    {
//                        MessageBox.Show("Paid amount cannot exceed original amount!",
//                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        paidamounttxtbox.Text = originalAmount.ToString("N2");
//                    }
//                    else if (paidAmount < 0)
//                    {
//                        MessageBox.Show("Paid amount cannot be negative!",
//                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        paidamounttxtbox.Text = "0.00";
//                    }
//                    else
//                    {
//                        paidamounttxtbox.Text = paidAmount.ToString("N2");
//                    }
//                    paidamounttxtbox.SelectionStart = paidamounttxtbox.Text.Length;
//                }
//            }
//        }

//        private void promisesidtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Read-only field - no action needed
//        }

//        private void accountnotxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Read-only field
//        }

//        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Read-only field
//        }

//        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Limit narration to 100 characters
//            if (narrationtxtbox.Text.Length > 100)
//            {
//                narrationtxtbox.Text = narrationtxtbox.Text.Substring(0, 100);
//                narrationtxtbox.SelectionStart = 100;
//            }
//        }

//        private void promisedatetxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Read-only field
//        }

//        private void newpromisedatetxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Validate date format
//            if (!isLoading && !string.IsNullOrEmpty(newpromisedatetxtbox.Text))
//            {
//                if (DateTime.TryParse(newpromisedatetxtbox.Text, out DateTime newDate))
//                {
//                    newpromisedatetxtbox.Text = newDate.ToString("dd/MM/yyyy");

//                    // Validate new date is not before today for postpone
//                    if (postponeprocessradiobtn.Checked && newDate < DateTime.Now.Date)
//                    {
//                        MessageBox.Show("New promise date cannot be in the past!",
//                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
//                    }
//                }
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Validate input
//                if (!ValidateInput())
//                    return;

//                // Confirm action
//                DialogResult result = MessageBox.Show($"Are you sure you want to process Promise #{selectedPromiseNo}?\n\n" +
//                    $"Account: {accountnametxtbox.Text}\nAmount: {originalAmount:N2}\nProcess Type: {GetProcessTypeDescription()}",
//                    "Confirm Processing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//                if (result != DialogResult.Yes)
//                    return;

//                Cursor = Cursors.WaitCursor;

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                using (SqlTransaction transaction = connection.BeginTransaction())
//                {
//                    try
//                    {
//                        bool success = false;

//                        if (fullprocessradiobtn.Checked)
//                        {
//                            success = ProcessFull(transaction);
//                        }
//                        else if (partialprocessradiobtn.Checked)
//                        {
//                            success = ProcessPartial(transaction);
//                        }
//                        else if (postponeprocessradiobtn.Checked)
//                        {
//                            success = ProcessPostpone(transaction);
//                        }

//                        if (success)
//                        {
//                            transaction.Commit();
//                            MessageBox.Show("Promise processed successfully!",
//                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                            // Refresh grid and reset form
//                            LoadAllUnprocessedPromises();
//                            ResetForm();
//                        }
//                        else
//                        {
//                            transaction.Rollback();
//                            MessageBox.Show("Failed to process promise.",
//                                "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        transaction.Rollback();
//                        MessageBox.Show("Error during transaction: " + ex.Message,
//                            "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error processing promise: " + ex.Message,
//                    "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//                Cursor = Cursors.Default;
//            }
//        }

//        private bool ValidateInput()
//        {
//            // Check if promise is selected
//            if (selectedPromiseNo == 0)
//            {
//                MessageBox.Show("Please select a promise from the list.",
//                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                dataGridView1.Focus();
//                return false;
//            }

//            // Check process type selection
//            if (!fullprocessradiobtn.Checked && !partialprocessradiobtn.Checked && !postponeprocessradiobtn.Checked)
//            {
//                MessageBox.Show("Please select a process type (Full/Partial/Postpone).",
//                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return false;
//            }

//            // Validate partial process
//            if (partialprocessradiobtn.Checked)
//            {
//                if (!decimal.TryParse(paidamounttxtbox.Text.Replace(",", ""), out decimal paidAmount))
//                {
//                    MessageBox.Show("Please enter a valid paid amount.",
//                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    paidamounttxtbox.Focus();
//                    return false;
//                }

//                if (paidAmount <= 0)
//                {
//                    MessageBox.Show("Paid amount must be greater than 0 for partial processing.",
//                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    paidamounttxtbox.Focus();
//                    return false;
//                }

//                if (paidAmount >= originalAmount)
//                {
//                    MessageBox.Show("For partial processing, paid amount must be less than original amount.",
//                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    paidamounttxtbox.Focus();
//                    return false;
//                }
//            }

//            // Validate postpone process
//            if (postponeprocessradiobtn.Checked)
//            {
//                if (!DateTime.TryParse(newpromisedatetxtbox.Text, out DateTime newDate))
//                {
//                    MessageBox.Show("Please enter a valid new promise date.",
//                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    newpromisedatetxtbox.Focus();
//                    return false;
//                }

//                if (newDate <= originalPromiseDate)
//                {
//                    MessageBox.Show("New promise date must be after the original promise date.",
//                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    newpromisedatetxtbox.Focus();
//                    return false;
//                }
//            }

//            return true;
//        }

//        private bool ProcessFull(SqlTransaction transaction)
//        {
//            try
//            {
//                // Update promise as posted
//                string updatePromise = @"UPDATE Promises 
//                                       SET IsPosted = 1, 
//                                           Narration = @Narration
//                                       WHERE PromiseNo = @PromiseNo";

//                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
//                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
//                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());
//                cmd.ExecuteNonQuery();

//                // Create transaction record
//                CreateTransactionRecord(transaction, originalAmount);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Full processing failed: " + ex.Message, ex);
//            }
//        }

//        private bool ProcessPartial(SqlTransaction transaction)
//        {
//            try
//            {
//                decimal paidAmount = decimal.Parse(paidamounttxtbox.Text.Replace(",", ""));
//                decimal remainingAmount = originalAmount - paidAmount;

//                // Update original promise with partial amount and mark as posted
//                string updatePromise = @"UPDATE Promises 
//                                       SET Amount = @PaidAmount,
//                                           IsPosted = 1,
//                                           Narration = @Narration
//                                       WHERE PromiseNo = @PromiseNo";

//                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
//                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
//                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
//                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim() + " (Partial - Paid: " + paidAmount.ToString("N2") + ")");
//                cmd.ExecuteNonQuery();

//                // Create new promise for remaining amount
//                string newNarration = "Remaining from Promise #" + selectedPromiseNo;
//                if (!string.IsNullOrEmpty(narrationtxtbox.Text))
//                    newNarration += " - " + narrationtxtbox.Text.Trim();

//                string insertPromise = @"INSERT INTO Promises 
//                                       (CurrentDate, AccountNo, PromiseDate, IsPayment, 
//                                        Narration, Amount, UserNo, IsPosted)
//                                       VALUES (@CurrentDate, @AccountNo, @PromiseDate, @IsPayment,
//                                               @Narration, @Amount, @UserNo, 0)";

//                cmd = new SqlCommand(insertPromise, connection, transaction);
//                cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
//                cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text);
//                cmd.Parameters.AddWithValue("@PromiseDate", originalPromiseDate);
//                cmd.Parameters.AddWithValue("@IsPayment", radioButtonprocesspayment.Checked ? 1 : 0);
//                cmd.Parameters.AddWithValue("@Narration", newNarration);
//                cmd.Parameters.AddWithValue("@Amount", remainingAmount);
//                cmd.Parameters.AddWithValue("@UserNo", currentUserNo);
//                cmd.ExecuteNonQuery();

//                // Create transaction record for paid amount
//                CreateTransactionRecord(transaction, paidAmount);

//                return true;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Partial processing failed: " + ex.Message, ex);
//            }
//        }

//        private bool ProcessPostpone(SqlTransaction transaction)
//        {
//            try
//            {
//                DateTime newDate = DateTime.Parse(newpromisedatetxtbox.Text);

//                // Update promise date
//                string updatePromise = @"UPDATE Promises 
//                                       SET PromiseDate = @NewPromiseDate,
//                                           Narration = @Narration
//                                       WHERE PromiseNo = @PromiseNo";

//                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
//                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
//                cmd.Parameters.AddWithValue("@NewPromiseDate", newDate);
//                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim() + " (Postponed to: " + newDate.ToString("dd/MM/yyyy") + ")");
//                cmd.ExecuteNonQuery();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Postpone processing failed: " + ex.Message, ex);
//            }
//        }

//        private void CreateTransactionRecord(SqlTransaction transaction, decimal amount)
//        {
//            bool isPayment = radioButtonprocesspayment.Checked;

//            if (isPayment)
//            {
//                // Create Payment transaction (BankVoucher or CreditVoucher)
//                CreatePaymentTransaction(transaction, amount);
//            }
//            else
//            {
//                // Create Recovery transaction
//                CreateRecoveryTransaction(transaction, amount);
//            }
//        }

//        private void CreatePaymentTransaction(SqlTransaction transaction, decimal amount)
//        {
//            // Implementation depends on your specific payment system
//            MessageBox.Show($"Payment transaction of {amount:N2} would be created here.\n" +
//                "Implement CreatePaymentTransaction based on your business logic.",
//                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        private void CreateRecoveryTransaction(SqlTransaction transaction, decimal amount)
//        {
//            try
//            {
//                // Insert into Recovery table
//                string recoveryQuery = @"INSERT INTO Recovery 
//                                       (RecoveryDate, IsWithInvoice, SalesmanID, UserNO, IsPosted)
//                                       VALUES (@RecoveryDate, 0, NULL, @UserNO, 1);
//                                       SELECT SCOPE_IDENTITY();";

//                SqlCommand cmd = new SqlCommand(recoveryQuery, connection, transaction);
//                cmd.Parameters.AddWithValue("@RecoveryDate", DateTime.Now);
//                cmd.Parameters.AddWithValue("@UserNO", currentUserNo);

//                int recoveryId = Convert.ToInt32(cmd.ExecuteScalar());

//                // Insert into RecoveryBody
//                string recoveryBodyQuery = @"INSERT INTO RecoveryBody 
//                                           (RecoveryID, SaleID, CustomerID, Amount, Discount, 
//                                            IsDeleted, Narration, CompanyID, PaymentMode, IsPosted, VendorID)
//                                           VALUES (@RecoveryID, NULL, @CustomerID, @Amount, 0, 
//                                                   0, @Narration, NULL, 'Cash', 1, NULL)";

//                cmd = new SqlCommand(recoveryBodyQuery, connection, transaction);
//                cmd.Parameters.AddWithValue("@RecoveryID", recoveryId);
//                cmd.Parameters.AddWithValue("@CustomerID", accountnotxtbox.Text);
//                cmd.Parameters.AddWithValue("@Amount", amount);
//                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());
//                cmd.ExecuteNonQuery();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Recovery transaction creation failed: " + ex.Message, ex);
//            }
//        }

//        private string GetProcessTypeDescription()
//        {
//            if (fullprocessradiobtn.Checked)
//                return "Full " + (radioButtonprocesspayment.Checked ? "Payment" : "Recovery");
//            else if (partialprocessradiobtn.Checked)
//                return "Partial " + (radioButtonprocesspayment.Checked ? "Payment" : "Recovery");
//            else if (postponeprocessradiobtn.Checked)
//                return "Postpone";
//            else
//                return "Unknown";
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ResetForm();
//        }

//        private void ResetForm()
//        {
//            isLoading = true;

//            // Clear all textboxes
//            promisesidtxtbox.Clear();
//            accountnotxtbox.Clear();
//            accountnametxtbox.Clear();
//            promisedatetxtbox.Clear();
//            amounttxtbox.Clear();
//            paidamounttxtbox.Clear();
//            newpromisedatetxtbox.Clear();
//            narrationtxtbox.Clear();

//            // Set default values
//            newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
//            promisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

//            // Reset selection
//            selectedPromiseNo = 0;
//            originalAmount = 0;
//            originalPromiseDate = DateTime.Now;

//            // Reset radio buttons to defaults
//            radioButtonprocessrecovery.Checked = true;
//            fullprocessradiobtn.Checked = true;

//            // Disable process controls
//            EnableProcessControls(false);

//            isLoading = false;

//            // Focus on grid
//            dataGridView1.Focus();
//        }

//        private void EnableProcessControls(bool enable)
//        {
//            // Enable/disable process type radio buttons
//            fullprocessradiobtn.Enabled = enable;
//            partialprocessradiobtn.Enabled = enable;
//            postponeprocessradiobtn.Enabled = enable;

//            // Enable/disable save button
//            savebtn.Enabled = enable;
//        }

//        private void SetControlsReadOnly(bool readOnly)
//        {
//            promisesidtxtbox.ReadOnly = true;
//            accountnotxtbox.ReadOnly = true;
//            accountnametxtbox.ReadOnly = true;
//            promisedatetxtbox.ReadOnly = true;
//            amounttxtbox.ReadOnly = true;

//            // These will be controlled by radio buttons
//            paidamounttxtbox.ReadOnly = true;
//            newpromisedatetxtbox.ReadOnly = true;

//            narrationtxtbox.ReadOnly = false; // User can always edit narration
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        // Add this to test if data is loading
//        private void btnTest_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Promises WHERE IsPosted = 0", connection);
//                int count = Convert.ToInt32(cmd.ExecuteScalar());
//                MessageBox.Show($"Total unprocessed promises in database: {count}", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                cmd.CommandText = "SELECT TOP 5 * FROM Promises WHERE IsPosted = 0 ORDER BY PromiseNo";
//                SqlDataReader reader = cmd.ExecuteReader();
//                string data = "";
//                while (reader.Read())
//                {
//                    data += $"PromiseNo: {reader["PromiseNo"]}, Date: {reader["PromiseDate"]}, Amount: {reader["Amount"]}\n";
//                }
//                reader.Close();

//                if (!string.IsNullOrEmpty(data))
//                    MessageBox.Show($"Sample data:\n{data}", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Test failed: {ex.Message}", "Debug Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
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

namespace FO_2022
{
    public partial class Promises_Processing : Form
    {
        private SqlConnection connection;
        private DataTable promisesDataTable;
        private int selectedPromiseNo = 0;
        private decimal originalAmount = 0;
        private DateTime originalPromiseDate;
        private bool isLoading = false;
        private string currentUserNo = "1"; // User session se lein

        public Promises_Processing()
        {
            InitializeComponent();
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing database connection: " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Promises_Processing_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default dates
                newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
                promisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

                // Load ALL unprocessed promises
                LoadAllUnprocessedPromises();

                // Select default radio button
                radioButtonprocessrecovery.Checked = true;
                fullprocessradiobtn.Checked = true;

                // Focus on grid
                dataGridView1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message,
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load ALL unprocessed promises
        private void LoadAllUnprocessedPromises()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // Query to get ALL unprocessed promises with only 3 columns
                string query = @"SELECT 
                                CONVERT(varchar, p.PromiseDate, 103) as PromiseDate,
                                c.AccountName, 
                                p.Amount,
                                p.PromiseNo,
                                p.AccountNo,
                                p.Narration,
                                p.IsPayment,
                                p.IsPosted
                                FROM Promises p
                                LEFT JOIN ChartOfAccounts c ON p.AccountNo = c.AccountNo
                                WHERE p.IsPosted = 0
                                ORDER BY p.PromiseDate, p.PromiseNo";

                SqlCommand cmd = new SqlCommand(query, connection);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                promisesDataTable = new DataTable();
                adapter.Fill(promisesDataTable);

                // Bind to DataGridView
                dataGridView1.DataSource = promisesDataTable;

                // Format columns - show only 3 columns
                FormatDataGridView();

                // If no data found, show message
                if (promisesDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("No unprocessed promises found in database",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading promises: " + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                Cursor = Cursors.Default;
            }
        }

        private void FormatDataGridView()
        {
            try
            {
                if (dataGridView1.Columns.Count > 0)
                {
                    // Hide all columns except the 3 we want to show
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        // Show only these 3 columns
                        if (column.Name == "PromiseDate" || column.Name == "AccountName" || column.Name == "Amount")
                        {
                            column.Visible = true;

                            // Set headers
                            if (column.Name == "PromiseDate")
                                column.HeaderText = "Promise Date";
                            else if (column.Name == "AccountName")
                                column.HeaderText = "Account Name";
                            else if (column.Name == "Amount")
                            {
                                column.HeaderText = "Amount";
                                column.DefaultCellStyle.Format = "N2";
                                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                        }
                        else
                        {
                            // Hide other columns but keep them for data access
                            column.Visible = false;
                        }
                    }

                    // Auto-size columns
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                    // Set selection mode
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;

                    // Set row height
                    dataGridView1.RowTemplate.Height = 25;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error formatting grid: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    isLoading = true;

                    // Safely get values from hidden columns
                    selectedPromiseNo = row.Cells["PromiseNo"].Value != DBNull.Value ?
                        Convert.ToInt32(row.Cells["PromiseNo"].Value) : 0;

                    promisesidtxtbox.Text = selectedPromiseNo > 0 ? selectedPromiseNo.ToString() : "";

                    accountnotxtbox.Text = row.Cells["AccountNo"].Value != DBNull.Value ?
                        row.Cells["AccountNo"].Value.ToString() : "";

                    accountnametxtbox.Text = row.Cells["AccountName"].Value != DBNull.Value ?
                        row.Cells["AccountName"].Value.ToString() : "";

                    promisedatetxtbox.Text = row.Cells["PromiseDate"].Value != DBNull.Value ?
                        row.Cells["PromiseDate"].Value.ToString() : "";

                    amounttxtbox.Text = row.Cells["Amount"].Value != DBNull.Value ?
                        Convert.ToDecimal(row.Cells["Amount"].Value).ToString("N2") : "0.00";

                    narrationtxtbox.Text = row.Cells["Narration"].Value != DBNull.Value ?
                        row.Cells["Narration"].Value.ToString() : "";

                    // Store original values for validation
                    originalAmount = row.Cells["Amount"].Value != DBNull.Value ?
                        Convert.ToDecimal(row.Cells["Amount"].Value) : 0;

                    if (row.Cells["PromiseDate"].Value != DBNull.Value &&
                        DateTime.TryParse(row.Cells["PromiseDate"].Value.ToString(), out DateTime promiseDate))
                    {
                        originalPromiseDate = promiseDate;
                    }
                    else
                    {
                        originalPromiseDate = DateTime.Now;
                    }

                    // Set initial values for processing
                    paidamounttxtbox.Text = originalAmount.ToString("N2");
                    newpromisedatetxtbox.Text = promisedatetxtbox.Text;

                    // Enable process controls
                    EnableProcessControls(true);

                    isLoading = false;
                }
            }
            catch (Exception ex)
            {
                isLoading = false;
                MessageBox.Show("Error selecting promise: " + ex.Message,
                    "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // No specific action needed for content click
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // No validation needed for read-only grid
        }

        private void radioButtonprocesspayment_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonprocesspayment.Checked && !isLoading)
            {
                FilterGridByPaymentType(true);
            }
        }

        private void radioButtonprocessrecovery_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonprocessrecovery.Checked && !isLoading)
            {
                FilterGridByPaymentType(false);
            }
        }

        private void FilterGridByPaymentType(bool isPayment)
        {
            try
            {
                if (promisesDataTable != null && promisesDataTable.Rows.Count > 0)
                {
                    DataView dv = new DataView(promisesDataTable);
                    dv.RowFilter = $"IsPayment = {(isPayment ? "1" : "0")}";
                    dataGridView1.DataSource = dv.ToTable();
                    FormatDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering data: " + ex.Message,
                    "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void fullprocessradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (fullprocessradiobtn.Checked && !isLoading)
            {
                paidamounttxtbox.Text = originalAmount.ToString("N2");
                paidamounttxtbox.ReadOnly = true;
                newpromisedatetxtbox.ReadOnly = true;
            }
        }

        private void partialprocessradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (partialprocessradiobtn.Checked && !isLoading)
            {
                paidamounttxtbox.Text = "0.00";
                paidamounttxtbox.ReadOnly = false;
                newpromisedatetxtbox.ReadOnly = true;
                paidamounttxtbox.Focus();
                paidamounttxtbox.SelectAll();
            }
        }

        private void postponeprocessradiobtn_CheckedChanged(object sender, EventArgs e)
        {
            if (postponeprocessradiobtn.Checked && !isLoading)
            {
                paidamounttxtbox.Text = "0.00";
                paidamounttxtbox.ReadOnly = true;
                newpromisedatetxtbox.ReadOnly = false;
                newpromisedatetxtbox.Focus();
                newpromisedatetxtbox.SelectAll();
            }
        }

        private void amounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field, no action needed
        }

        private void paidamounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate and format paid amount
            if (!isLoading && !string.IsNullOrEmpty(paidamounttxtbox.Text))
            {
                if (decimal.TryParse(paidamounttxtbox.Text.Replace(",", ""), out decimal paidAmount))
                {
                    // Validate against original amount
                    if (paidAmount > originalAmount)
                    {
                        MessageBox.Show("Paid amount cannot exceed original amount!",
                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        paidamounttxtbox.Text = originalAmount.ToString("N2");
                    }
                    else if (paidAmount < 0)
                    {
                        MessageBox.Show("Paid amount cannot be negative!",
                            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        paidamounttxtbox.Text = "0.00";
                    }
                    else
                    {
                        paidamounttxtbox.Text = paidAmount.ToString("N2");
                    }
                    paidamounttxtbox.SelectionStart = paidamounttxtbox.Text.Length;
                }
            }
        }

        private void promisesidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field - no action needed
        }

        private void accountnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }

        private void accountnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Limit narration to 100 characters
            if (narrationtxtbox.Text.Length > 100)
            {
                narrationtxtbox.Text = narrationtxtbox.Text.Substring(0, 100);
                narrationtxtbox.SelectionStart = 100;
            }
        }

        private void promisedatetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }

        private void newpromisedatetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate date format
            if (!isLoading && !string.IsNullOrEmpty(newpromisedatetxtbox.Text))
            {
                if (DateTime.TryParse(newpromisedatetxtbox.Text, out DateTime newDate))
                {
                    newpromisedatetxtbox.Text = newDate.ToString("dd/MM/yyyy");

                    // Validate new date is not before today for postpone
                    //if (postponeprocessradiobtn.Checked && newDate < DateTime.Now.Date)
                    //{
                    //    MessageBox.Show("New promise date cannot be in the past!",
                    //        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    //}
                }
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (!ValidateInput())
                    return;

                // Confirm action
                DialogResult result = MessageBox.Show($"Are you sure you want to process Promise #{selectedPromiseNo}?\n\n" +
                    $"Account: {accountnametxtbox.Text}\nAmount: {originalAmount:N2}\nProcess Type: {GetProcessTypeDescription()}",
                    "Confirm Processing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                Cursor = Cursors.WaitCursor;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        bool success = false;

                        if (fullprocessradiobtn.Checked)
                        {
                            success = ProcessFull(transaction);
                        }
                        else if (partialprocessradiobtn.Checked)
                        {
                            success = ProcessPartial(transaction);
                        }
                        else if (postponeprocessradiobtn.Checked)
                        {
                            success = ProcessPostpone(transaction);
                        }

                        if (success)
                        {
                            transaction.Commit();
                            MessageBox.Show("Promise processed successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Refresh grid and reset form
                            LoadAllUnprocessedPromises();
                            ResetForm();
                        }
                        else
                        {
                            transaction.Rollback();
                            MessageBox.Show("Failed to process promise.",
                                "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error during transaction: " + ex.Message,
                            "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing promise: " + ex.Message,
                    "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                Cursor = Cursors.Default;
            }
        }

        private bool ValidateInput()
        {
            // Check if promise is selected
            if (selectedPromiseNo == 0)
            {
                MessageBox.Show("Please select a promise from the list.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dataGridView1.Focus();
                return false;
            }

            // Check process type selection
            if (!fullprocessradiobtn.Checked && !partialprocessradiobtn.Checked && !postponeprocessradiobtn.Checked)
            {
                MessageBox.Show("Please select a process type (Full/Partial/Postpone).",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate partial process
            if (partialprocessradiobtn.Checked)
            {
                if (!decimal.TryParse(paidamounttxtbox.Text.Replace(",", ""), out decimal paidAmount))
                {
                    MessageBox.Show("Please enter a valid paid amount.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    paidamounttxtbox.Focus();
                    return false;
                }

                if (paidAmount <= 0)
                {
                    MessageBox.Show("Paid amount must be greater than 0 for partial processing.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    paidamounttxtbox.Focus();
                    return false;
                }

                if (paidAmount >= originalAmount)
                {
                    MessageBox.Show("For partial processing, paid amount must be less than original amount.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    paidamounttxtbox.Focus();
                    return false;
                }
            }

            // Validate postpone process
            if (postponeprocessradiobtn.Checked)
            {
                if (!DateTime.TryParse(newpromisedatetxtbox.Text, out DateTime newDate))
                {
                    MessageBox.Show("Please enter a valid new promise date.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    newpromisedatetxtbox.Focus();
                    return false;
                }

                if (newDate <= originalPromiseDate)
                {
                    MessageBox.Show("New promise date must be after the original promise date.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    newpromisedatetxtbox.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool ProcessFull(SqlTransaction transaction)
        {
            try
            {
                // ✅ PROMISE TABLE UPDATE - IsPosted = 1
                string updatePromise = @"UPDATE Promises 
                                       SET IsPosted = 1, 
                                           Narration = @Narration
                                       WHERE PromiseNo = @PromiseNo";

                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());
                cmd.ExecuteNonQuery();

                // ✅ Create transaction record (COMMENTED FOR NOW)
                // CreateTransactionRecord(transaction, originalAmount);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Full processing failed: " + ex.Message, ex);
            }
        }

        private bool ProcessPartial(SqlTransaction transaction)
        {
            try
            {
                decimal paidAmount = decimal.Parse(paidamounttxtbox.Text.Replace(",", ""));
                decimal remainingAmount = originalAmount - paidAmount;

                // ✅ PROMISE TABLE UPDATE - Original promise update
                string updatePromise = @"UPDATE Promises 
                                       SET Amount = @PaidAmount,
                                           IsPosted = 1,
                                           Narration = @Narration
                                       WHERE PromiseNo = @PromiseNo";

                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim() + " (Partial - Paid: " + paidAmount.ToString("N2") + ")");
                cmd.ExecuteNonQuery();

                // ✅ PROMISE TABLE INSERT - New promise for remaining amount
                string newNarration = "Remaining from Promise #" + selectedPromiseNo;
                if (!string.IsNullOrEmpty(narrationtxtbox.Text))
                    newNarration += " - " + narrationtxtbox.Text.Trim();

                string insertPromise = @"INSERT INTO Promises 
                                       (CurrentDate, AccountNo, PromiseDate, IsPayment, 
                                        Narration, Amount, UserNo, IsPosted)
                                       VALUES (@CurrentDate, @AccountNo, @PromiseDate, @IsPayment,
                                               @Narration, @Amount, @UserNo, 0)";

                cmd = new SqlCommand(insertPromise, connection, transaction);
                cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text);
                cmd.Parameters.AddWithValue("@PromiseDate", originalPromiseDate);
                cmd.Parameters.AddWithValue("@IsPayment", radioButtonprocesspayment.Checked ? 1 : 0);
                cmd.Parameters.AddWithValue("@Narration", newNarration);
                cmd.Parameters.AddWithValue("@Amount", remainingAmount);
                cmd.Parameters.AddWithValue("@UserNo", currentUserNo);
                cmd.ExecuteNonQuery();

                // ✅ Create transaction record for paid amount (COMMENTED FOR NOW)
                // CreateTransactionRecord(transaction, paidAmount);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Partial processing failed: " + ex.Message, ex);
            }
        }

        private bool ProcessPostpone(SqlTransaction transaction)
        {
            try
            {
                DateTime newDate = DateTime.Parse(newpromisedatetxtbox.Text);

                // ✅ PROMISE TABLE UPDATE - Update promise date
                string updatePromise = @"UPDATE Promises 
                                       SET PromiseDate = @NewPromiseDate,
                                           Narration = @Narration
                                       WHERE PromiseNo = @PromiseNo";

                SqlCommand cmd = new SqlCommand(updatePromise, connection, transaction);
                cmd.Parameters.AddWithValue("@PromiseNo", selectedPromiseNo);
                cmd.Parameters.AddWithValue("@NewPromiseDate", newDate);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim() + " (Postponed to: " + newDate.ToString("dd/MM/yyyy") + ")");
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Postpone processing failed: " + ex.Message, ex);
            }
        }

        /**********************************************************************
        * RECOVERY TRANSACTION CODE (COMMENTED - UNCOMMENT WHEN NEEDED)
        **********************************************************************/
        /*
        private void CreateTransactionRecord(SqlTransaction transaction, decimal amount)
        {
            bool isPayment = radioButtonprocesspayment.Checked;

            if (isPayment)
            {
                // Create Payment transaction
                CreatePaymentTransaction(transaction, amount);
            }
            else
            {
                // Create Recovery transaction
                CreateRecoveryTransaction(transaction, amount);
            }
        }

        private void CreatePaymentTransaction(SqlTransaction transaction, decimal amount)
        {
            try
            {
                // ✅ BankVouchers table mein entry
                string bankVoucherQuery = @"INSERT INTO BankVouchers 
                                           (VoucherNo, VoucherDate, LastUpdateDate, UserNo, IsPosted)
                                           VALUES (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, 1);
                                           SELECT SCOPE_IDENTITY();";
                
                // Get next VoucherNo
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM BankVouchers", connection, transaction);
                int voucherNo = Convert.ToInt32(cmd.ExecuteScalar());
                
                cmd.CommandText = bankVoucherQuery;
                cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                cmd.Parameters.AddWithValue("@VoucherDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserNo", currentUserNo);
                int voucherId = Convert.ToInt32(cmd.ExecuteScalar());
                
                // ✅ BankVouchersBody mein entry
                string bankVoucherBodyQuery = @"INSERT INTO BankVouchersBody 
                                               (VoucherNo, AccountNo, Narration, Debit, Credit)
                                               VALUES (@VoucherNo, @AccountNo, @Narration, 0, @Amount)";
                
                cmd = new SqlCommand(bankVoucherBodyQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Payment transaction creation failed: " + ex.Message, ex);
            }
        }

        private void CreateRecoveryTransaction(SqlTransaction transaction, decimal amount)
        {
            try
            {
                // ✅ Recovery table mein entry
                string recoveryQuery = @"INSERT INTO Recovery 
                                       (RecoveryDate, IsWithInvoice, SalesmanID, UserNO, IsPosted)
                                       OUTPUT INSERTED.RecoveryID
                                       VALUES (@RecoveryDate, 0, NULL, @UserNO, 1)";

                SqlCommand cmd = new SqlCommand(recoveryQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@RecoveryDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@UserNO", currentUserNo);

                int recoveryId = Convert.ToInt32(cmd.ExecuteScalar());

                // ✅ RecoveryBody table mein entry
                string recoveryBodyQuery = @"INSERT INTO RecoveryBody 
                                           (RecoveryID, SaleID, CustomerID, Amount, Discount, 
                                            IsDeleted, Narration, CompanyID, PaymentMode, IsPosted, VendorID)
                                           VALUES (@RecoveryID, NULL, @CustomerID, @Amount, 0, 
                                                   0, @Narration, NULL, 'Cash', 1, NULL)";

                cmd = new SqlCommand(recoveryBodyQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@RecoveryID", recoveryId);
                cmd.Parameters.AddWithValue("@CustomerID", accountnotxtbox.Text);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text.Trim());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Recovery transaction creation failed: " + ex.Message, ex);
            }
        }
        */
        /**********************************************************************
        * END OF RECOVERY TRANSACTION CODE
        **********************************************************************/

        private string GetProcessTypeDescription()
        {
            if (fullprocessradiobtn.Checked)
                return "Full " + (radioButtonprocesspayment.Checked ? "Payment" : "Recovery");
            else if (partialprocessradiobtn.Checked)
                return "Partial " + (radioButtonprocesspayment.Checked ? "Payment" : "Recovery");
            else if (postponeprocessradiobtn.Checked)
                return "Postpone";
            else
                return "Unknown";
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            isLoading = true;

            // Clear all textboxes
            promisesidtxtbox.Clear();
            accountnotxtbox.Clear();
            accountnametxtbox.Clear();
            promisedatetxtbox.Clear();
            amounttxtbox.Clear();
            paidamounttxtbox.Clear();
            newpromisedatetxtbox.Clear();
            narrationtxtbox.Clear();

            // Set default values
            newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            promisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Reset selection
            selectedPromiseNo = 0;
            originalAmount = 0;
            originalPromiseDate = DateTime.Now;

            // Reset radio buttons to defaults
            radioButtonprocessrecovery.Checked = true;
            fullprocessradiobtn.Checked = true;

            // Disable process controls
            EnableProcessControls(false);

            isLoading = false;

            // Focus on grid
            dataGridView1.Focus();
        }

        private void EnableProcessControls(bool enable)
        {
            // Enable/disable process type radio buttons
            fullprocessradiobtn.Enabled = enable;
            partialprocessradiobtn.Enabled = enable;
            postponeprocessradiobtn.Enabled = enable;

            // Enable/disable save button
            savebtn.Enabled = enable;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Helper method to debug connection
        private void TestConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Promises WHERE IsPosted = 0", connection);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                MessageBox.Show($"Found {count} unprocessed promises in database", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection test failed: {ex.Message}", "Debug Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }
    }
}