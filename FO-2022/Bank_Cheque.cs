using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Bank_Cheque : Form
    {
        private string connectionString;
        private DataTable chequeDataTable;
        private string currentAccountNo = "";
        private string currentAccountName = "";

        public Bank_Cheque()
        {
            InitializeComponent();
            InitializeConnectionString();
            InitializeForm();
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
                // Fallback connection string if configuration fails
                connectionString = "Data Source=.;Initial Catalog=db_ac0035_pos;Integrated Security=True";
            }
        }

        private void InitializeForm()
        {
            try
            {
                // Set default date to today
                datepcker.Value = DateTime.Now;

                // Load last IssueID from ChequeBook table
                LoadLastIssueID();

                // Initialize DataGridView
                InitializeDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLastIssueID()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(IssueID), 0) + 1 FROM ChequeBook";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int lastID = Convert.ToInt32(cmd.ExecuteScalar());
                        idtxtbox.Text = lastID.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading last Issue ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridView()
        {
            chequeDataTable = new DataTable();
            // Add columns for display
            chequeDataTable.Columns.Add("SerialNo", typeof(int));
            chequeDataTable.Columns.Add("AccountNo", typeof(string));
            chequeDataTable.Columns.Add("AccountName", typeof(string));
            chequeDataTable.Columns.Add("ChequeNo", typeof(string));
            // Hidden columns for database operations
            chequeDataTable.Columns.Add("Remarks", typeof(string));
            chequeDataTable.Columns.Add("IsCancel", typeof(bool));

            dataGridView1.DataSource = chequeDataTable;

            // Configure DataGridView columns
            ConfigureDataGridViewColumns();

            // Add auto increment for SerialNo
            chequeDataTable.Columns["SerialNo"].AutoIncrement = true;
            chequeDataTable.Columns["SerialNo"].AutoIncrementSeed = 1;
            chequeDataTable.Columns["SerialNo"].AutoIncrementStep = 1;
        }

        private void ConfigureDataGridViewColumns()
        {
            // Hide unnecessary columns
            dataGridView1.Columns["Remarks"].Visible = false;
            dataGridView1.Columns["IsCancel"].Visible = false;

            // Set column headers and properties
            dataGridView1.Columns["SerialNo"].HeaderText = "S.No";
            dataGridView1.Columns["SerialNo"].Width = 50;
            dataGridView1.Columns["SerialNo"].ReadOnly = true;

            dataGridView1.Columns["AccountNo"].HeaderText = "Account No";
            dataGridView1.Columns["AccountNo"].Width = 100;
            dataGridView1.Columns["AccountNo"].ReadOnly = true;

            dataGridView1.Columns["AccountName"].HeaderText = "Account Name";
            dataGridView1.Columns["AccountName"].Width = 200;
            dataGridView1.Columns["AccountName"].ReadOnly = true;

            dataGridView1.Columns["ChequeNo"].HeaderText = "Cheque No";
            dataGridView1.Columns["ChequeNo"].Width = 150;
            dataGridView1.Columns["ChequeNo"].ReadOnly = true;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            try
            {
                Mainform1 mf = new Mainform1();
                mf.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void idtxtbox_TextChanged(object sender, EventArgs e)
        {
            // You can add validation if needed
        }

        private void datepcker_ValueChanged(object sender, EventArgs e)
        {
            // Date changed event
        }

        private void banknotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Account number changed
        }

        private void banknosearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(banknotxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No. first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            bankaccounttxtbox.Text = result.ToString();
                            currentAccountNo = banknotxtbox.Text;
                            currentAccountName = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Account not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            bankaccounttxtbox.Text = "";
                            currentAccountNo = "";
                            currentAccountName = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bankaccounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // Account name changed
        }

        private void firstchqtxtbox_TextChanged(object sender, EventArgs e)
        {
            // First cheque number changed
        }

        private void lastchqtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Last cheque number changed - you can add auto-fill logic here
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Cell click event - you can add specific cell handling if needed
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(banknotxtbox.Text) || string.IsNullOrEmpty(bankaccounttxtbox.Text) ||
                    string.IsNullOrEmpty(firstchqtxtbox.Text) || string.IsNullOrEmpty(lastchqtxtbox.Text))
                {
                    MessageBox.Show("Please fill all required fields!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (chequeDataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Please generate cheque numbers first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            // Insert into ChequeBook table
                            string chequeBookQuery = @"INSERT INTO ChequeBook 
                                                     (IssueID, IssueDate, AccountNo, FirstChequeNo, LastChequeNo, UserNo, IsPosted) 
                                                     VALUES (@IssueID, @IssueDate, @AccountNo, @FirstChequeNo, @LastChequeNo, @UserNo, @IsPosted)";

                            using (SqlCommand cmd = new SqlCommand(chequeBookQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IssueID", Convert.ToInt32(idtxtbox.Text));
                                cmd.Parameters.AddWithValue("@IssueDate", datepcker.Value);
                                cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text);
                                cmd.Parameters.AddWithValue("@FirstChequeNo", firstchqtxtbox.Text);
                                cmd.Parameters.AddWithValue("@LastChequeNo", lastchqtxtbox.Text);
                                cmd.Parameters.AddWithValue("@UserNo", 1); // Assuming UserNo = 1, change as needed
                                cmd.Parameters.AddWithValue("@IsPosted", false);

                                cmd.ExecuteNonQuery();
                            }

                            // Insert into ChequeBookBody table
                            string chequeBookBodyQuery = @"INSERT INTO ChequeBookBody 
                                                         (SerialNo, IssueID, AccountNo, ChequeNo, Remarks, IsCancel, IsDeleted) 
                                                         VALUES (@SerialNo, @IssueID, @AccountNo, @ChequeNo, @Remarks, @IsCancel, @IsDeleted)";

                            foreach (DataRow row in chequeDataTable.Rows)
                            {
                                using (SqlCommand cmd = new SqlCommand(chequeBookBodyQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@SerialNo", row["SerialNo"]);
                                    cmd.Parameters.AddWithValue("@IssueID", Convert.ToInt32(idtxtbox.Text));
                                    cmd.Parameters.AddWithValue("@AccountNo", banknotxtbox.Text);
                                    cmd.Parameters.AddWithValue("@ChequeNo", row["ChequeNo"]);
                                    cmd.Parameters.AddWithValue("@Remarks", row["Remarks"] ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@IsCancel", row["IsCancel"]);
                                    cmd.Parameters.AddWithValue("@IsDeleted", false);

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Commit transaction
                            transaction.Commit();

                            MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Load next IssueID
                            LoadLastIssueID();
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
                MessageBox.Show("Error saving data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear all fields
                datepcker.Value = DateTime.Now;
                banknotxtbox.Text = "";
                bankaccounttxtbox.Text = "";
                firstchqtxtbox.Text = "";
                lastchqtxtbox.Text = "";
                currentAccountNo = "";
                currentAccountName = "";

                // Clear DataGridView
                chequeDataTable.Rows.Clear();

                // Load next IssueID
                LoadLastIssueID();

                MessageBox.Show("All fields cleared!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Cheque Book";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;

                Label label = new Label();
                label.Text = "Enter Issue ID to open:";
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
                        if (int.TryParse(input, out int issueId))
                        {
                            LoadChequeBook(issueId);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid numeric Issue ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening cheque book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadChequeBook(int issueId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load ChequeBook header
                    string headerQuery = "SELECT * FROM ChequeBook WHERE IssueID = @IssueID";
                    using (SqlCommand cmd = new SqlCommand(headerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@IssueID", issueId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idtxtbox.Text = reader["IssueID"].ToString();
                                datepcker.Value = Convert.ToDateTime(reader["IssueDate"]);
                                banknotxtbox.Text = reader["AccountNo"].ToString();
                                firstchqtxtbox.Text = reader["FirstChequeNo"].ToString();
                                lastchqtxtbox.Text = reader["LastChequeNo"].ToString();

                                // Load account name
                                bankaccounttxtbox.Text = GetAccountName(banknotxtbox.Text);
                                currentAccountNo = banknotxtbox.Text;
                                currentAccountName = bankaccounttxtbox.Text;
                            }
                            else
                            {
                                MessageBox.Show("Cheque Book not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // Load ChequeBookBody details
                    chequeDataTable.Rows.Clear();
                    string detailQuery = "SELECT * FROM ChequeBookBody WHERE IssueID = @IssueID ORDER BY SerialNo";
                    using (SqlCommand cmd = new SqlCommand(detailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@IssueID", issueId);
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                chequeDataTable.Rows.Add(
                                    row["SerialNo"],
                                    currentAccountNo,
                                    currentAccountName,
                                    row["ChequeNo"],
                                    row["Remarks"],
                                    row["IsCancel"]
                                );
                            }
                        }
                    }

                    MessageBox.Show("Cheque Book loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cheque book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetAccountName(string accountNo)
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
                        return result != null ? result.ToString() : "";
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(idtxtbox.Text))
                {
                    MessageBox.Show("Please enter Issue ID or load a record first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to delete this Cheque Book?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Begin transaction
                        using (SqlTransaction transaction = conn.BeginTransaction())
                        {
                            try
                            {
                                // Delete from ChequeBookBody first (foreign key constraint)
                                string deleteBodyQuery = "DELETE FROM ChequeBookBody WHERE IssueID = @IssueID";
                                using (SqlCommand cmd = new SqlCommand(deleteBodyQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@IssueID", Convert.ToInt32(idtxtbox.Text));
                                    cmd.ExecuteNonQuery();
                                }

                                // Delete from ChequeBook
                                string deleteHeaderQuery = "DELETE FROM ChequeBook WHERE IssueID = @IssueID";
                                using (SqlCommand cmd = new SqlCommand(deleteHeaderQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@IssueID", Convert.ToInt32(idtxtbox.Text));
                                    cmd.ExecuteNonQuery();
                                }

                                transaction.Commit();

                                MessageBox.Show("Cheque Book deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Clear form and load next IssueID
                                clearbtn_Click(sender, e);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting cheque book: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Event handler for Enter key in lastchqtxtbox
        private void lastchqtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GenerateChequeNumbers();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void GenerateChequeNumbers()
        {
            try
            {
                if (string.IsNullOrEmpty(firstchqtxtbox.Text) || string.IsNullOrEmpty(lastchqtxtbox.Text))
                {
                    MessageBox.Show("Please enter both First and Last Cheque Numbers!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(banknotxtbox.Text) || string.IsNullOrEmpty(bankaccounttxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No. and get Account Name first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Clear existing data
                chequeDataTable.Rows.Clear();

                // Parse cheque numbers
                string firstCheque = firstchqtxtbox.Text;
                string lastCheque = lastchqtxtbox.Text;

                // Set current account info
                currentAccountNo = banknotxtbox.Text;
                currentAccountName = bankaccounttxtbox.Text;

                // Extract numeric parts if any
                if (TryExtractChequeNumbers(firstCheque, lastCheque, out int startNum, out int endNum))
                {
                    for (int i = startNum; i <= endNum; i++)
                    {
                        // Format cheque number based on pattern
                        string chequeNo = FormatChequeNumber(firstCheque, i);

                        chequeDataTable.Rows.Add(
                            chequeDataTable.Rows.Count + 1,
                            currentAccountNo,
                            currentAccountName,
                            chequeNo,
                            "",
                            false
                        );
                    }
                }
                else
                {
                    // If not numeric, just add single entry
                    chequeDataTable.Rows.Add(
                        1,
                        currentAccountNo,
                        currentAccountName,
                        firstchqtxtbox.Text,
                        "",
                        false
                    );
                }

                MessageBox.Show($"{chequeDataTable.Rows.Count} cheque numbers generated!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating cheque numbers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryExtractChequeNumbers(string first, string last, out int startNum, out int endNum)
        {
            startNum = 0;
            endNum = 0;

            // Try to extract numeric suffix
            string firstPrefix = GetNonNumericPrefix(first);
            string lastPrefix = GetNonNumericPrefix(last);

            if (firstPrefix != lastPrefix)
                return false;

            string firstNumStr = first.Substring(firstPrefix.Length);
            string lastNumStr = last.Substring(lastPrefix.Length);

            if (int.TryParse(firstNumStr, out startNum) && int.TryParse(lastNumStr, out endNum))
            {
                return true;
            }

            return false;
        }

        private string GetNonNumericPrefix(string str)
        {
            int index = 0;
            while (index < str.Length && !char.IsDigit(str[index]))
            {
                index++;
            }
            return str.Substring(0, index);
        }

        private string FormatChequeNumber(string pattern, int number)
        {
            string prefix = GetNonNumericPrefix(pattern);
            string numericPart = pattern.Substring(prefix.Length);

            // Format number with same length as original
            if (int.TryParse(numericPart, out int originalNum))
            {
                return prefix + number.ToString().PadLeft(numericPart.Length, '0');
            }

            return pattern;
        }

        // Add these event handlers for automatic account name fetch when leaving AccountNo field
        private void banknotxtbox_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(banknotxtbox.Text) && string.IsNullOrEmpty(bankaccounttxtbox.Text))
            {
                banknosearch_Click(sender, e);
            }
        }

        // Add KeyDown event for banknotxtbox to search on Enter
        private void banknotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                banknosearch_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
