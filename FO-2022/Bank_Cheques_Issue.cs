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
    public partial class Bank_Cheques_Issue : Form
    {
        public Bank_Cheques_Issue()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
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

        }

        private void removebtn_Click(object sender, EventArgs e)
        {

        }

        private void savebtn_Click(object sender, EventArgs e)
        {

        }

        private void clearbtn_Click(object sender, EventArgs e)
        {

        }

        private void Bank_Cheques_Issue_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        private void accountsearch_Click(object sender, EventArgs e)
        {

        }
    }
}
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
//                promisesdatepcker.Value = DateTime.Now.Date;
//                newpromisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
//                promisedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

//                // Load initial data
//                LoadPromisesForDate(DateTime.Now.Date);

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

//        private void LoadPromisesForDate(DateTime date)
//        {
//            try
//            {
//                Cursor = Cursors.WaitCursor;

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                string query = @"SELECT p.PromiseNo, 
//                                CONVERT(varchar, p.CurrentDate, 103) as CurrentDate,
//                                p.AccountNo, 
//                                c.AccountName, 
//                                CONVERT(varchar, p.PromiseDate, 103) as PromiseDate,
//                                p.Amount, 
//                                p.Narration, 
//                                p.IsPayment,
//                                p.IsPosted
//                                FROM Promises p
//                                LEFT JOIN ChartOfAccounts c ON p.AccountNo = c.AccountNo
//                                WHERE CAST(p.PromiseDate AS DATE) = @PromiseDate 
//                                AND p.IsPosted = 0
//                                ORDER BY p.PromiseDate, p.PromiseNo";

//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@PromiseDate", date.Date);

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                promisesDataTable = new DataTable();
//                adapter.Fill(promisesDataTable);

//                // Bind to DataGridView
//                dataGridView1.DataSource = promisesDataTable;

//                // Format columns
//                FormatDataGridView();

//                // Update status label
//                lblStatus.Text = $"Loaded {promisesDataTable.Rows.Count} promise(s) for {date.ToString("dd/MM/yyyy")}";
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
//                    // Hide technical columns
//                    dataGridView1.Columns["IsPayment"].Visible = false;
//                    dataGridView1.Columns["IsPosted"].Visible = false;

//                    // Set column headers
//                    if (dataGridView1.Columns.Contains("PromiseNo"))
//                        dataGridView1.Columns["PromiseNo"].HeaderText = "Promise ID";

//                    if (dataGridView1.Columns.Contains("CurrentDate"))
//                        dataGridView1.Columns["CurrentDate"].HeaderText = "Entry Date";

//                    if (dataGridView1.Columns.Contains("AccountNo"))
//                        dataGridView1.Columns["AccountNo"].HeaderText = "Account No";

//                    if (dataGridView1.Columns.Contains("AccountName"))
//                        dataGridView1.Columns["AccountName"].HeaderText = "Account Name";

//                    if (dataGridView1.Columns.Contains("PromiseDate"))
//                        dataGridView1.Columns["PromiseDate"].HeaderText = "Promise Date";

//                    if (dataGridView1.Columns.Contains("Amount"))
//                    {
//                        dataGridView1.Columns["Amount"].HeaderText = "Amount";
//                        dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "N2";
//                        dataGridView1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//                    }

//                    if (dataGridView1.Columns.Contains("Narration"))
//                        dataGridView1.Columns["Narration"].HeaderText = "Narration";

//                    // Auto-size columns
//                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

//                    // Set selection mode
//                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//                    dataGridView1.MultiSelect = false;
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

//                    // Populate form fields
//                    selectedPromiseNo = Convert.ToInt32(row.Cells["PromiseNo"].Value);
//                    promisesidtxtbox.Text = selectedPromiseNo.ToString();
//                    accountnotxtbox.Text = row.Cells["AccountNo"].Value?.ToString() ?? "";
//                    accountnametxtbox.Text = row.Cells["AccountName"].Value?.ToString() ?? "";
//                    promisedatetxtbox.Text = row.Cells["PromiseDate"].Value?.ToString() ?? "";
//                    amounttxtbox.Text = Convert.ToDecimal(row.Cells["Amount"].Value).ToString("N2");
//                    narrationtxtbox.Text = row.Cells["Narration"].Value?.ToString() ?? "";

//                    // Store original values for validation
//                    originalAmount = Convert.ToDecimal(row.Cells["Amount"].Value);
//                    originalPromiseDate = DateTime.Parse(row.Cells["PromiseDate"].Value?.ToString() ?? DateTime.Now.ToString());

//                    // Set initial values for processing
//                    paidamounttxtbox.Text = originalAmount.ToString("N2");
//                    newpromisedatetxtbox.Text = promisedatetxtbox.Text;

//                    // Enable process controls
//                    EnableProcessControls(true);

//                    // Highlight selected row
//                    dataGridView1.Rows[e.RowIndex].Selected = true;

//                    isLoading = false;

//                    lblStatus.Text = $"Selected Promise #{selectedPromiseNo} - {accountnametxtbox.Text}";
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
//                lblProcessType.Text = "Processing: Payments";
//                FilterGridByPaymentType(true);
//            }
//        }

//        private void radioButtonprocessrecovery_CheckedChanged(object sender, EventArgs e)
//        {
//            if (radioButtonprocessrecovery.Checked && !isLoading)
//            {
//                lblProcessType.Text = "Processing: Recoveries";
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
//                paidamounttxtbox.BackColor = SystemColors.Control;
//                newpromisedatetxtbox.BackColor = SystemColors.Control;
//            }
//        }

//        private void partialprocessradiobtn_CheckedChanged(object sender, EventArgs e)
//        {
//            if (partialprocessradiobtn.Checked && !isLoading)
//            {
//                paidamounttxtbox.Text = "0.00";
//                paidamounttxtbox.ReadOnly = false;
//                newpromisedatetxtbox.ReadOnly = true;
//                paidamounttxtbox.BackColor = Color.White;
//                newpromisedatetxtbox.BackColor = SystemColors.Control;
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
//                paidamounttxtbox.BackColor = SystemColors.Control;
//                newpromisedatetxtbox.BackColor = Color.White;
//                newpromisedatetxtbox.Focus();
//                newpromisedatetxtbox.SelectAll();
//            }
//        }

//        private void promisesdatepcker_ValueChanged(object sender, EventArgs e)
//        {
//            if (!isLoading)
//            {
//                LoadPromisesForDate(promisesdatepcker.Value);
//                ResetForm();
//            }
//        }

//        private void amounttxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Format amount as user types
//            if (!isLoading && !string.IsNullOrEmpty(amounttxtbox.Text))
//            {
//                if (decimal.TryParse(amounttxtbox.Text.Replace(",", ""), out decimal amount))
//                {
//                    amounttxtbox.Text = amount.ToString("N2");
//                    amounttxtbox.SelectionStart = amounttxtbox.Text.Length;
//                }
//            }
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
//            // Read-only field
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
//                            LoadPromisesForDate(promisesdatepcker.Value);
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
//                        throw new Exception("Transaction failed: " + ex.Message, ex);
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
//            // This is a template - adjust based on your requirements

//            /*
//            string voucherQuery = @"INSERT INTO BankVouchers (VoucherNo, VoucherDate, LastUpdateDate, UserNo, IsPosted)
//                                   VALUES (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, 1)";
            
//            // Get next voucher number
//            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM BankVouchers", connection, transaction);
//            int voucherNo = Convert.ToInt32(cmd.ExecuteScalar());
            
//            cmd.CommandText = voucherQuery;
//            cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
//            cmd.Parameters.AddWithValue("@VoucherDate", DateTime.Now);
//            cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
//            cmd.Parameters.AddWithValue("@UserNo", currentUserNo);
//            cmd.ExecuteNonQuery();
//            */

//            // For now, just log it
//            Console.WriteLine($"Payment transaction created for amount: {amount:N2}");
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
//            promisesdatepcker.Value = DateTime.Now.Date;
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

//            // Clear status
//            lblStatus.Text = "Select a promise to process";

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

//            // Set textbox backgrounds
//            if (enable)
//            {
//                paidamounttxtbox.BackColor = fullprocessradiobtn.Checked ? SystemColors.Control : Color.White;
//                newpromisedatetxtbox.BackColor = postponeprocessradiobtn.Checked ? Color.White : SystemColors.Control;
//            }
//            else
//            {
//                paidamounttxtbox.BackColor = SystemColors.Control;
//                newpromisedatetxtbox.BackColor = SystemColors.Control;
//            }
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

//            // Set background colors for read-only fields
//            Color backColor = readOnly ? SystemColors.Control : Color.White;
//            promisesidtxtbox.BackColor = backColor;
//            accountnotxtbox.BackColor = backColor;
//            accountnametxtbox.BackColor = backColor;
//            promisedatetxtbox.BackColor = backColor;
//            amounttxtbox.BackColor = backColor;
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        // Helper method to format currency
//        private string FormatCurrency(decimal amount)
//        {
//            return amount.ToString("N2");
//        }

//        // Add this to your form if not already present
//        private Label lblStatus;
//        private Label lblProcessType;

//        // Initialize these labels in your form designer:
//        // lblStatus: For displaying status messages
//        // lblProcessType: For showing current process type
//    }
//}