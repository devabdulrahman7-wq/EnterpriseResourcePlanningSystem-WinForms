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
////    public partial class Journal_Voucher : Form
////    {
////        public Journal_Voucher()
////        {
////            InitializeComponent();
////        }

////        private void groupBox1_Enter(object sender, EventArgs e)
////        {

////        }



////        private void vouchernotxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void voucherdatepcker_ValueChanged(object sender, EventArgs e)
////        {

////        }

////        private void singleentrybtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void allsavebtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void refreshbtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void recoverydatepcker_ValueChanged(object sender, EventArgs e)
////        {

////        }

////        private void allbtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void vbtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void chequebtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void onlinebtn_CheckedChanged(object sender, EventArgs e)
////        {

////        }

////        private void getentryalbtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {

////        }

////        private void ischeckbox_CheckedChanged(object sender, EventArgs e)
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

////        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void referencetxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void billnotxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void debittxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void credittxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void AccountdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {

////        }



////        private void pictureBox1_Click(object sender, EventArgs e)
////        {

////        }

////        private void clearpicbtn_Click(object sender, EventArgs e)
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

////        private void closebtn_Click(object sender, EventArgs e)
////        {

////        }

////        private void debitfetchtxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }

////        private void creditfetchtxtbox_TextChanged(object sender, EventArgs e)
////        {

////        }
////    }
////}






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
    public partial class Journal_Voucher : Form
    {
        private SqlConnection con;
        private DataTable voucherData;
        private decimal totalDebit = 0;
        private decimal totalCredit = 0;
        private int currentVoucherNo = 0;
        private bool isPictureLoaded = false;

        public Journal_Voucher()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadLastVoucherNo();

            // Make total boxes read-only programmatically (also do this in designer)
            debitfetchtxtbox.ReadOnly = true;
            creditfetchtxtbox.ReadOnly = true;
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                // Update this connection string according to your database configuration
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                con = new SqlConnection(connectionString);
                //con.Open();
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadLastVoucherNo()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(VoucherNo), 0) + 1 AS NextVoucherNo FROM JournalVouchers";
                SqlCommand cmd = new SqlCommand(query, con);
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    currentVoucherNo = Convert.ToInt32(result);
                    vouchernotxtbox.Text = currentVoucherNo.ToString();
                }
                else
                {
                    vouchernotxtbox.Text = "1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading voucher number: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                vouchernotxtbox.Text = "1";
            }
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(vouchernotxtbox.Text))
                {
                    MessageBox.Show("Please enter a Voucher No.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int voucherNo;
                if (!int.TryParse(vouchernotxtbox.Text, out voucherNo))
                {
                    MessageBox.Show("Please enter a valid Voucher No.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // FIXED: Changed Reference to Refrence to match database column name
                string query = @"SELECT jvb.SerialNo, jvb.AccountNo, jvb.Narration, jvb.Refrence, 
                                        jvb.BillNo, jvb.Debit, jvb.Credit
                                 FROM JournalVouchersBody jvb
                                 WHERE jvb.VoucherNo = @VoucherNo";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;

                // Clear previous totals
                totalDebit = 0;
                totalCredit = 0;

                // Calculate totals
                foreach (DataRow row in dt.Rows)
                {
                    totalDebit += Convert.ToDecimal(row["Debit"]);
                    totalCredit += Convert.ToDecimal(row["Credit"]);
                }

                debitfetchtxtbox.Text = totalDebit.ToString("N2");
                creditfetchtxtbox.Text = totalCredit.ToString("N2");

                MessageBox.Show("Data loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void accountidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                FetchAccountName();
                e.Handled = true;
            }
        }

        private void accountidsearch_Click(object sender, EventArgs e)
        {
            FetchAccountName();
        }

  
        private void FetchAccountName()
        {
            try
            {
                if (string.IsNullOrEmpty(accountidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Account No.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                SqlCommand cmd = new SqlCommand(query, con);

                // Since ChartOfAccounts has AccountNo as varchar(10), pass as string
                cmd.Parameters.AddWithValue("@AccountNo", accountidtxtbox.Text);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    accountnametxtbox.Text = result.ToString();
                }
                else
                {
                    accountnametxtbox.Text = "Account not found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching account name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void debittxtbox_TextChanged(object sender, EventArgs e)
        {
            UpdateCurrentDebit();
        }

        private void UpdateCurrentDebit()
        {
            try
            {
                // This only updates the current entry's debit value
                // The total will be calculated from DataGridView
            }
            catch
            {
                // Handle exception
            }
        }

        private void credittxtbox_TextChanged(object sender, EventArgs e)
        {
            UpdateCurrentCredit();
        }

        private void UpdateCurrentCredit()
        {
            try
            {
                // This only updates the current entry's credit value
                // The total will be calculated from DataGridView
            }
            catch
            {
                // Handle exception
            }
        }

        private void credittxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
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
                // Check if any radio button is selected
                if (!allbtn.Checked && !vbtn.Checked && !chequebtn.Checked && !onlinebtn.Checked)
                {
                    MessageBox.Show("Please select a payment mode (All/V/Cheque/Online).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(accountidtxtbox.Text) ||
                    string.IsNullOrEmpty(accountnametxtbox.Text) ||
                    (string.IsNullOrEmpty(debittxtbox.Text) && string.IsNullOrEmpty(credittxtbox.Text)))
                {
                    MessageBox.Show("Please fill all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse debit and credit values
                decimal debitValue = 0;
                decimal creditValue = 0;

                if (!string.IsNullOrEmpty(debittxtbox.Text) && decimal.TryParse(debittxtbox.Text, out debitValue))
                {
                    // debitValue already set
                }

                if (!string.IsNullOrEmpty(credittxtbox.Text) && decimal.TryParse(credittxtbox.Text, out creditValue))
                {
                    // creditValue already set
                }

                // Add to DataGridView
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(AccountdataGridView);

                row.Cells[0].Value = accountidtxtbox.Text;
                row.Cells[1].Value = accountnametxtbox.Text;
                row.Cells[2].Value = narrationtxtbox.Text;
                row.Cells[3].Value = referencetxtbox.Text;
                row.Cells[4].Value = billnotxtbox.Text;
                row.Cells[5].Value = debitValue.ToString("N2");
                row.Cells[6].Value = creditValue.ToString("N2");

                AccountdataGridView.Rows.Add(row);

                // Update totals from the entire DataGridView
                UpdateTotalsFromGrid();

                // Clear only the entry fields, NOT the total boxes
                ClearEntryFieldsOnly();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding entry: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearEntryFieldsOnly()
        {
            // Clear only the entry fields, NOT the total boxes
            accountidtxtbox.Clear();
            accountnametxtbox.Clear();
            narrationtxtbox.Clear();
            referencetxtbox.Clear();
            billnotxtbox.Clear();
            debittxtbox.Clear();
            credittxtbox.Clear();
            accountidtxtbox.Focus();
        }

        private void UpdateTotalsFromGrid()
        {
            decimal totalDebitGrid = 0;
            decimal totalCreditGrid = 0;

            foreach (DataGridViewRow row in AccountdataGridView.Rows)
            {
                if (row.Cells[5].Value != null)
                {
                    if (decimal.TryParse(row.Cells[5].Value.ToString(), out decimal debit))
                    {
                        totalDebitGrid += debit;
                    }
                }
                if (row.Cells[6].Value != null)
                {
                    if (decimal.TryParse(row.Cells[6].Value.ToString(), out decimal credit))
                    {
                        totalCreditGrid += credit;
                    }
                }
            }

            debitfetchtxtbox.Text = totalDebitGrid.ToString("N2");
            creditfetchtxtbox.Text = totalCreditGrid.ToString("N2");
        }





        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate radio button selection
                if (!allbtn.Checked && !vbtn.Checked && !chequebtn.Checked && !onlinebtn.Checked)
                {
                    MessageBox.Show("Please select a payment mode (All/V/Cheque/Online).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate DataGridView has data
                if (AccountdataGridView.Rows.Count == 0)
                {
                    MessageBox.Show("Please add at least one account entry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get totals from the total boxes (no validation needed)
                if (!decimal.TryParse(debitfetchtxtbox.Text, out decimal totalDebitGrid) ||
                    !decimal.TryParse(creditfetchtxtbox.Text, out decimal totalCreditGrid))
                {
                    MessageBox.Show("Invalid total values.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // NO DEBIT-CREDIT VALIDATION - REMOVED THIS CHECK
                // Save to JournalVouchers table
                string voucherQuery = @"INSERT INTO JournalVouchers (VoucherNo, VoucherDate, LastUpdateDate, UserNo, OfficeID, IsUploaded, IsPosted, ScannedDocuments)
                       VALUES (@VoucherNo, @VoucherDate, @LastUpdateDate, @UserNo, @OfficeID, @IsUploaded, @IsPosted, @ScannedDocuments)";

                SqlCommand voucherCmd = new SqlCommand(voucherQuery, con);
                voucherCmd.Parameters.AddWithValue("@VoucherNo", Convert.ToInt32(vouchernotxtbox.Text));
                voucherCmd.Parameters.AddWithValue("@VoucherDate", voucherdatepcker.Value);
                voucherCmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now);
                voucherCmd.Parameters.AddWithValue("@UserNo", 1); // Change this to actual user

                // FIX: OfficeID is int NOT NULL, so use a valid integer value
                voucherCmd.Parameters.AddWithValue("@OfficeID", 0); // Use 0 or your actual office ID

                voucherCmd.Parameters.AddWithValue("@IsUploaded", 0);
                voucherCmd.Parameters.AddWithValue("@IsPosted", 0);

                if (isPictureLoaded && pictureBox1.Image != null)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat);
                        voucherCmd.Parameters.AddWithValue("@ScannedDocuments", ms.ToArray());
                    }
                }
                else
                {
                    // FIX: Proper way to set varbinary(max) to NULL
                    voucherCmd.Parameters.Add("@ScannedDocuments", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                }

                voucherCmd.ExecuteNonQuery();

                // Save to JournalVouchersBody table
                int serialNo = 1;
                foreach (DataGridViewRow row in AccountdataGridView.Rows)
                {
                    if (row.Cells[0].Value != null)
                    {
                        // FIXED: Column name is Refrence (typo in database)
                        string bodyQuery = @"INSERT INTO JournalVouchersBody (SerialNo, VoucherNo, AccountNo, Narration, Debit, Credit, BillNo, Refrence, IsRecVoucher, RecAccountNo)
                                    VALUES (@SerialNo, @VoucherNo, @AccountNo, @Narration, @Debit, @Credit, @BillNo, @Refrence, @IsRecVoucher, @RecAccountNo)";

                        SqlCommand bodyCmd = new SqlCommand(bodyQuery, con);
                        bodyCmd.Parameters.AddWithValue("@SerialNo", serialNo++);
                        bodyCmd.Parameters.AddWithValue("@VoucherNo", Convert.ToInt32(vouchernotxtbox.Text));

                        // FIX: AccountNo conversion - JournalVouchersBody.AccountNo is int
                        if (int.TryParse(row.Cells[0].Value.ToString(), out int accountNo))
                        {
                            bodyCmd.Parameters.AddWithValue("@AccountNo", accountNo);
                        }
                        else
                        {
                            MessageBox.Show($"Invalid AccountNo: {row.Cells[0].Value}. Must be a number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        bodyCmd.Parameters.AddWithValue("@Narration", row.Cells[2].Value != null ? row.Cells[2].Value.ToString() : "");

                        // Parse debit and credit values
                        decimal debit = row.Cells[5].Value != null ? Convert.ToDecimal(row.Cells[5].Value) : 0;
                        decimal credit = row.Cells[6].Value != null ? Convert.ToDecimal(row.Cells[6].Value) : 0;

                        bodyCmd.Parameters.AddWithValue("@Debit", debit);
                        bodyCmd.Parameters.AddWithValue("@Credit", credit);

                        // BillNo is varchar(50) in database
                        if (row.Cells[4].Value != null && !string.IsNullOrEmpty(row.Cells[4].Value.ToString()))
                        {
                            bodyCmd.Parameters.AddWithValue("@BillNo", row.Cells[4].Value.ToString());
                        }
                        else
                        {
                            bodyCmd.Parameters.AddWithValue("@BillNo", DBNull.Value);
                        }

                        // FIXED: Parameter name must be @Refrence to match database column name
                        if (row.Cells[3].Value != null && !string.IsNullOrEmpty(row.Cells[3].Value.ToString()))
                        {
                            bodyCmd.Parameters.AddWithValue("@Refrence", row.Cells[3].Value.ToString());
                        }
                        else
                        {
                            bodyCmd.Parameters.AddWithValue("@Refrence", DBNull.Value);
                        }

                        bodyCmd.Parameters.AddWithValue("@IsRecVoucher", 0);

                        // RecAccountNo is int in database
                        bodyCmd.Parameters.AddWithValue("@RecAccountNo", DBNull.Value);

                        bodyCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Journal Voucher saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Clear form for next entry
                ClearForm();
                LoadLastVoucherNo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearForm()
        {
            voucherdatepcker.Value = DateTime.Now;
            AccountdataGridView.Rows.Clear();
            dataGridView1.DataSource = null;
            pictureBox1.Image = null;
            isPictureLoaded = false;
            allbtn.Checked = false;
            vbtn.Checked = false;
            chequebtn.Checked = false;
            onlinebtn.Checked = false;
            singleentrybtn.Checked = false;
            allsavebtn.Checked = false;
            debitfetchtxtbox.Text = "0";
            creditfetchtxtbox.Text = "0";
            ClearEntryFieldsOnly();
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            MessageBox.Show("Form cleared successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (AccountdataGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in AccountdataGridView.SelectedRows)
                    {
                        AccountdataGridView.Rows.Remove(row);
                    }
                    UpdateTotalsFromGrid();
                    MessageBox.Show("Selected rows removed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select rows to remove.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing rows: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Open file dialog to select image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Select Scanned Document";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(openFileDialog.FileName);
                    isPictureLoaded = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void clearpicbtn_Click(object sender, EventArgs e)
        {
            if (isPictureLoaded)
            {
                pictureBox1.Image = null;
                isPictureLoaded = false;
                MessageBox.Show("Picture cleared successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No picture to clear.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (con != null && con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Journal Voucher";
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
                            LoadJournalVoucher(voucherNo);
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
                MessageBox.Show("Error opening journal voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJournalVoucher(int voucherNo)
        {
            try
            {
                // Load voucher header
                string headerQuery = @"SELECT * FROM JournalVouchers WHERE VoucherNo = @VoucherNo";
                SqlCommand headerCmd = new SqlCommand(headerQuery, con);
                headerCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                SqlDataReader reader = headerCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    vouchernotxtbox.Text = reader["VoucherNo"].ToString();
                    voucherdatepcker.Value = Convert.ToDateTime(reader["VoucherDate"]);

                    // Load image if exists
                    if (reader["ScannedDocuments"] != DBNull.Value)
                    {
                        byte[] imageData = (byte[])reader["ScannedDocuments"];
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                            isPictureLoaded = true;
                        }
                    }
                    reader.Close();

                    // FIXED: Changed to select specific columns
                    string bodyQuery = @"SELECT AccountNo, Narration, Debit, Credit, BillNo, Refrence 
                                         FROM JournalVouchersBody 
                                         WHERE VoucherNo = @VoucherNo 
                                         ORDER BY SerialNo";

                    SqlCommand bodyCmd = new SqlCommand(bodyQuery, con);
                    bodyCmd.Parameters.AddWithValue("@VoucherNo", voucherNo);

                    SqlDataAdapter adapter = new SqlDataAdapter(bodyCmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Populate AccountdataGridView
                    AccountdataGridView.Rows.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        DataGridViewRow dgvRow = new DataGridViewRow();
                        dgvRow.CreateCells(AccountdataGridView);

                        dgvRow.Cells[0].Value = row["AccountNo"].ToString();
                        dgvRow.Cells[1].Value = GetAccountName(row["AccountNo"].ToString());
                        dgvRow.Cells[2].Value = row["Narration"].ToString();
                        dgvRow.Cells[3].Value = row["Refrence"].ToString(); // Changed to Refrence
                        dgvRow.Cells[4].Value = row["BillNo"].ToString();
                        dgvRow.Cells[5].Value = row["Debit"].ToString();
                        dgvRow.Cells[6].Value = row["Credit"].ToString();

                        AccountdataGridView.Rows.Add(dgvRow);
                    }

                    // Update totals
                    UpdateTotalsFromGrid();

                    MessageBox.Show("Voucher loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    reader.Close();
                    MessageBox.Show("Voucher not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading voucher: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetAccountName(string accountNo)
        {
            try
            {
                string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AccountNo", accountNo);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "";
            }
            catch
            {
                return "";
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