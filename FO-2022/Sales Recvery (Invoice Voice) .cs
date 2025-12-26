using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sales_Recvery__Invoice_Voice_ : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable recoveryItems = new DataTable();
        private int currentRecoveryID = 0;
        private bool isEditing = false;

        // Property to load recovery from search form
        public int RecoveryIDToLoad { get; set; }

        public Sales_Recvery__Invoice_Voice_()
        {
            InitializeComponent();
            InitializeDataTable();
        }

        private void InitializeDataTable()
        {
            recoveryItems.Clear();
            recoveryItems.Columns.Clear();

            // These columns match what we display and save
            recoveryItems.Columns.Add("SaleID", typeof(int));
            recoveryItems.Columns.Add("CustomerID", typeof(string));
            recoveryItems.Columns.Add("CustomerName", typeof(string));
            recoveryItems.Columns.Add("SectorTown", typeof(string));
            recoveryItems.Columns.Add("Receivables", typeof(decimal));
            recoveryItems.Columns.Add("Recovered", typeof(decimal));
            recoveryItems.Columns.Add("Recovery", typeof(decimal));
            recoveryItems.Columns.Add("Discount", typeof(decimal));
            recoveryItems.Columns.Add("Balance", typeof(decimal));
            recoveryItems.Columns.Add("PaymentMode", typeof(string));
            recoveryItems.Columns.Add("AccountNo", typeof(string));
            recoveryItems.Columns.Add("Narration", typeof(string));

            dataGridView1.DataSource = recoveryItems;
            FormatGridColumns();
        }

        private void FormatGridColumns()
        {
            // Check if columns exist before formatting
            if (dataGridView1.Columns.Contains("Receivables"))
                dataGridView1.Columns["Receivables"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("Recovered"))
                dataGridView1.Columns["Recovered"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("Recovery"))
                dataGridView1.Columns["Recovery"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("Discount"))
                dataGridView1.Columns["Discount"].DefaultCellStyle.Format = "N2";
            if (dataGridView1.Columns.Contains("Balance"))
                dataGridView1.Columns["Balance"].DefaultCellStyle.Format = "N2";
        }

        private void Sales_Recvery__Invoice_Voice__Load(object sender, EventArgs e)
        {
            vpaymentbtn.Checked = true;
            GenerateRecoveryID();
            dateTimePicker1.Value = DateTime.Now;

            if (RecoveryIDToLoad > 0)
            {
                LoadRecoveryForEditing(RecoveryIDToLoad);
            }
        }

        private void GenerateRecoveryID()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(RecoveryID), 0) + 1 FROM Recovery1st";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        currentRecoveryID = Convert.ToInt32(result);
                        recoveyridtxtbox.Text = currentRecoveryID.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Recovery ID: " + ex.Message);
            }
        }

        private void LoadRecoveryForEditing(int recoveryID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load header information
                    string headerQuery = @"
                SELECT RecoveryDate, IsWithInvoice 
                FROM Recovery1st 
                WHERE RecoveryID = @RecoveryID";

                    using (SqlCommand cmd = new SqlCommand(headerQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RecoveryID", recoveryID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dateTimePicker1.Value = Convert.ToDateTime(reader["RecoveryDate"]);
                                chkwitninvoice.Checked = Convert.ToBoolean(reader["IsWithInvoice"]);
                            }
                            else
                            {
                                MessageBox.Show("Recovery not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }

                    // Load details from RecoveryBody1st
                    string detailsQuery = @"
                SELECT 
                    SaleID,
                    CustomerID,
                    Amount as Recovery,
                    Discount,
                    Narration,
                    PaymentMode
                FROM RecoveryBody1st 
                WHERE RecoveryID = @RecoveryID 
                AND IsDeleted = 0";

                    using (SqlCommand cmd = new SqlCommand(detailsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RecoveryID", recoveryID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        recoveryItems.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            int saleID = Convert.ToInt32(row["SaleID"]);
                            string customerID = row["CustomerID"].ToString();
                            decimal recovery = Convert.ToDecimal(row["Recovery"]);
                            decimal discount = Convert.ToDecimal(row["Discount"]);

                            // Get customer name
                            string customerName = GetCustomerName(customerID);

                            // Get invoice details
                            decimal receivables = GetInvoiceAmount(saleID);
                            decimal alreadyRecovered = GetAlreadyRecovered(saleID, recoveryID);

                            decimal balance = receivables - (alreadyRecovered + recovery + discount);

                            recoveryItems.Rows.Add(
                                saleID,
                                customerID,
                                customerName,
                                "", // SectorTown
                                receivables,
                                alreadyRecovered,
                                recovery,
                                discount,
                                balance,
                                row["PaymentMode"].ToString(),
                                "", // AccountNo
                                row["Narration"].ToString()
                            );
                        }
                    }
                }

                currentRecoveryID = recoveryID;
                recoveyridtxtbox.Text = recoveryID.ToString();
                isEditing = true;
                UpdateTotals();

                MessageBox.Show($"Loaded {recoveryItems.Rows.Count} recovery items", "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recovery: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetCustomerName(string customerID)
        {
            try
            {
                string query = "SELECT PartyName FROM Parties WHERE PartyID = @CustomerID";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : customerID;
                    }
                }
            }
            catch
            {   
                return customerID;
            }
        }

        private decimal GetInvoiceAmount(int saleID)
        {
            try
            {
                string query = @"
                SELECT ReceivedAmount 
                FROM Sales 
                WHERE SaleId = @SaleID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SaleID", saleID);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetAlreadyRecovered(int saleID, int excludeRecoveryID = 0)
        {
            try
            {
                string query = @"
                SELECT ISNULL(SUM(Amount), 0) 
                FROM RecoveryBody1st 
                WHERE SaleID = @SaleID 
                AND IsDeleted = 0";

                if (excludeRecoveryID > 0)
                {
                    query += " AND RecoveryID != @RecoveryID";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@SaleID", saleID);
                        if (excludeRecoveryID > 0)
                        {
                            cmd.Parameters.AddWithValue("@RecoveryID", excludeRecoveryID);
                        }
                        object result = cmd.ExecuteScalar();
                        return Convert.ToDecimal(result);
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void customeridsearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
            {
                LoadCustomerData(cstmridtxtbox.Text);
                LoadCustomerAllSalesToGrid(cstmridtxtbox.Text);
            }
            else
            {
                MessageBox.Show("Please enter Customer ID", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadCustomerData(string customerID)
        {
            try
            {
                string query = @"
                    SELECT PartyName 
                    FROM Parties 
                    WHERE PartyID = @CustomerID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        object result = cmd.ExecuteScalar();
                        fetchedcustomernametxtbox.Text = result != null ? result.ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer: " + ex.Message);
            }
        }

        private void LoadCustomerAllSalesToGrid(string customerID)
        {
            try
            {
                string query = @"
                    SELECT 
                        s.SaleId,
                        s.CustomerId,
                        p.PartyName as CustomerName,
                        s.ReceivedAmount as InvoiceAmount
                    FROM Sales s
                    LEFT JOIN Parties p ON s.CustomerId = p.PartyID
                    WHERE s.CustomerId = @CustomerID
                    AND s.IsPosted = 1
                    ORDER BY s.SaleId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", customerID);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        recoveryItems.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            int saleID = Convert.ToInt32(row["SaleId"]);
                            decimal invoiceAmount = Convert.ToDecimal(row["InvoiceAmount"]);
                            decimal alreadyRecovered = GetAlreadyRecovered(saleID);

                            // Add to grid - Recovery amount = 0 initially
                            recoveryItems.Rows.Add(
                                row["SaleId"],
                                row["CustomerId"],
                                row["CustomerName"],
                                "", // SectorTown
                                invoiceAmount,
                                alreadyRecovered,
                                0,  // Recovery = 0 initially
                                0,  // Discount = 0 initially
                                invoiceAmount - alreadyRecovered, // Balance
                                GetPaymentMode(),
                                "", // AccountNo
                                narrationtxtbox.Text
                            );
                        }

                        UpdateTotals();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer sales: " + ex.Message);
            }
        }

        private string GetPaymentMode()
        {
            if (vpaymentbtn.Checked) return "VPayment";
            else if (onlinebtn.Checked) return "Online";
            else if (chqbtn.Checked) return "Cheque";
            else if (cashbtn.Checked) return "Cash";
            return "VPayment";
        }

        private void UpdateTotals()
        {
            decimal totalReceivables = 0;
            decimal totalRecovered = 0;
            decimal totalRecovery = 0;
            decimal totalDiscount = 0;
            decimal totalBalance = 0;

            foreach (DataRow row in recoveryItems.Rows)
            {
                totalReceivables += Convert.ToDecimal(row["Receivables"]);
                totalRecovered += Convert.ToDecimal(row["Recovered"]);
                totalRecovery += Convert.ToDecimal(row["Recovery"]);
                totalDiscount += Convert.ToDecimal(row["Discount"]);
                totalBalance += Convert.ToDecimal(row["Balance"]);
            }

            totaltxtbox1.Text = totalReceivables.ToString("N2");
            totaltxtbox2.Text = totalRecovered.ToString("N2");
            totaltxtbox3.Text = totalRecovery.ToString("N2");
            totaltxtbox14.Text = totalBalance.ToString("N2");
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < recoveryItems.Rows.Count)
            {
                DataRow row = recoveryItems.Rows[e.RowIndex];

                saleidtxtbox.Text = row["SaleID"].ToString();
                customeridtxtbox.Text = row["CustomerID"].ToString();
                customernametxtbox.Text = row["CustomerName"].ToString();
                townandsectortxtbox.Text = row["SectorTown"].ToString();
                invoiceamounttxtbox.Text = Convert.ToDecimal(row["Receivables"]).ToString("N2");
                recoveredtxtbox.Text = Convert.ToDecimal(row["Recovered"]).ToString("N2");
                recoverytxtbox.Text = Convert.ToDecimal(row["Recovery"]).ToString("N2");
                disctxtbox.Text = Convert.ToDecimal(row["Discount"]).ToString("N2");
                balancetxtbox.Text = Convert.ToDecimal(row["Balance"]).ToString("N2");
            }
        }

        private void CalculateBalance()
        {
            try
            {
                if (!string.IsNullOrEmpty(invoiceamounttxtbox.Text) &&
                    !string.IsNullOrEmpty(recoveredtxtbox.Text))
                {
                    decimal invoiceAmount = Convert.ToDecimal(invoiceamounttxtbox.Text);
                    decimal alreadyRecovered = Convert.ToDecimal(recoveredtxtbox.Text);
                    decimal recovery = string.IsNullOrEmpty(recoverytxtbox.Text) ? 0 : Convert.ToDecimal(recoverytxtbox.Text);
                    decimal discount = string.IsNullOrEmpty(disctxtbox.Text) ? 0 : Convert.ToDecimal(disctxtbox.Text);

                    decimal balance = invoiceAmount - (alreadyRecovered + recovery + discount);
                    balancetxtbox.Text = balance.ToString("N2");
                }
            }
            catch
            {
                // Ignore conversion errors
            }
        }

        private void balancetxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateSelectedGridRow();
            }
        }

        private void UpdateSelectedGridRow()
        {
            try
            {
                if (dataGridView1.CurrentRow == null) return;

                int rowIndex = dataGridView1.CurrentRow.Index;
                if (rowIndex >= 0 && rowIndex < recoveryItems.Rows.Count)
                {
                    DataRow row = recoveryItems.Rows[rowIndex];

                    row["Recovery"] = string.IsNullOrEmpty(recoverytxtbox.Text) ? 0 : Convert.ToDecimal(recoverytxtbox.Text);
                    row["Discount"] = string.IsNullOrEmpty(disctxtbox.Text) ? 0 : Convert.ToDecimal(disctxtbox.Text);

                    decimal receivables = Convert.ToDecimal(row["Receivables"]);
                    decimal recovered = Convert.ToDecimal(row["Recovered"]);
                    decimal recovery = Convert.ToDecimal(row["Recovery"]);
                    decimal discount = Convert.ToDecimal(row["Discount"]);
                    decimal balance = receivables - (recovered + recovery + discount);
                    row["Balance"] = balance;

                    row["PaymentMode"] = GetPaymentMode();
                    row["Narration"] = narrationtxtbox.Text;

                    UpdateTotals();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating row: " + ex.Message);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (recoveryItems.Rows.Count == 0)
            {
                MessageBox.Show("Please add recovery items first.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Save to Recovery1st
                        string recoveryQuery = @"
                    IF EXISTS (SELECT 1 FROM Recovery1st WHERE RecoveryID = @RecoveryID)
                    BEGIN
                        UPDATE Recovery1st SET 
                            RecoveryDate = @RecoveryDate,
                            IsWithInvoice = @IsWithInvoice,
                            IsPosted = 0
                        WHERE RecoveryID = @RecoveryID
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Recovery1st (RecoveryID, RecoveryDate, IsWithInvoice, IsPosted)
                        VALUES (@RecoveryID, @RecoveryDate, @IsWithInvoice, 0)
                    END";

                        using (SqlCommand cmd = new SqlCommand(recoveryQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@RecoveryID", currentRecoveryID);
                            cmd.Parameters.AddWithValue("@RecoveryDate", dateTimePicker1.Value);
                            cmd.Parameters.AddWithValue("@IsWithInvoice", chkwitninvoice.Checked);
                            cmd.ExecuteNonQuery();
                        }

                        // Clear existing details if editing
                        if (isEditing)
                        {
                            // First check if there are any records to delete
                            string checkQuery = "SELECT COUNT(*) FROM RecoveryBody1st WHERE RecoveryID = @RecoveryID AND IsDeleted = 0";
                            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@RecoveryID", currentRecoveryID);
                                int recordCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                                if (recordCount > 0)
                                {
                                    string deleteQuery = "UPDATE RecoveryBody1st SET IsDeleted = 1 WHERE RecoveryID = @RecoveryID";
                                    using (SqlCommand delCmd = new SqlCommand(deleteQuery, conn, transaction))
                                    {
                                        delCmd.Parameters.AddWithValue("@RecoveryID", currentRecoveryID);
                                        delCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        // Save to RecoveryBody1st - DO NOT include SerialNo as it's IDENTITY
                        int rowsInserted = 0;
                        foreach (DataRow row in recoveryItems.Rows)
                        {
                            decimal recoveryAmount = Convert.ToDecimal(row["Recovery"]);
                            decimal discountAmount = Convert.ToDecimal(row["Discount"]);

                            // Only insert rows with recovery or discount amount
                            if (recoveryAmount > 0 || discountAmount > 0)
                            {
                                string detailQuery = @"
                            INSERT INTO RecoveryBody1st (
                                RecoveryID, 
                                SaleID, 
                                CustomerID, 
                                Amount, 
                                Discount, 
                                IsDeleted,
                                Narration, 
                                CompanyID, 
                                PaymentMode, 
                                IsPosted, 
                                VendorID
                            ) VALUES (
                                @RecoveryID, 
                                @SaleID, 
                                @CustomerID, 
                                @Amount, 
                                @Discount, 
                                @IsDeleted,
                                @Narration, 
                                @CompanyID, 
                                @PaymentMode, 
                                @IsPosted, 
                                @VendorID
                            )";

                                using (SqlCommand cmd = new SqlCommand(detailQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@RecoveryID", currentRecoveryID);
                                    cmd.Parameters.AddWithValue("@SaleID", Convert.ToInt32(row["SaleID"]));
                                    cmd.Parameters.AddWithValue("@CustomerID", row["CustomerID"].ToString());
                                    cmd.Parameters.AddWithValue("@Amount", recoveryAmount);
                                    cmd.Parameters.AddWithValue("@Discount", discountAmount);
                                    cmd.Parameters.AddWithValue("@IsDeleted", 0); // Not deleted
                                    cmd.Parameters.AddWithValue("@Narration", row["Narration"] != DBNull.Value ? row["Narration"].ToString() : "");
                                    cmd.Parameters.AddWithValue("@CompanyID", DBNull.Value); // NULL as per your schema
                                    cmd.Parameters.AddWithValue("@PaymentMode", row["PaymentMode"] != DBNull.Value ? row["PaymentMode"].ToString() : "Cash");
                                    cmd.Parameters.AddWithValue("@IsPosted", 0); // Not posted
                                    cmd.Parameters.AddWithValue("@VendorID", DBNull.Value); // NULL for customer recoveries

                                    int result = cmd.ExecuteNonQuery();
                                    if (result > 0)
                                    {
                                        rowsInserted++;
                                    }
                                }
                            }
                        }

                        transaction.Commit();

                        MessageBox.Show($"Recovery saved successfully!\nRecovery ID: {currentRecoveryID}\nRows inserted: {rowsInserted}");
                        isEditing = false;

                        // Generate new recovery ID for next entry
                        GenerateRecoveryID();

                        // Clear data for next entry
                        recoveryItems.Clear();
                        ClearForm();
                        UpdateTotals();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Error saving recovery: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Log the error details
                        Console.WriteLine($"Error details: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all data?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                recoveryItems.Clear();
                ClearForm();
                GenerateRecoveryID();
                isEditing = false;
            }
        }

        private void ClearForm()
        {
            cstmridtxtbox.Clear();
            fetchedcustomernametxtbox.Clear();
            cstmrbalancetxtbox.Clear();
            vendoridtxtbox.Clear();
            fetchedvendornametxtbox.Clear();
            vendorpartyrecoverytxtbox.Clear();
            narrationtxtbox.Clear();
            saleidtxtbox.Clear();
            customeridtxtbox.Clear();
            customernametxtbox.Clear();
            townandsectortxtbox.Clear();
            invoiceamounttxtbox.Clear();
            recoveredtxtbox.Clear();
            recoverytxtbox.Clear();
            disctxtbox.Clear();
            balancetxtbox.Clear();
            totaltxtbox1.Clear();
            totaltxtbox2.Clear();
            totaltxtbox3.Clear();
            totaltxtbox14.Clear();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            Search_Recovery_Invoice searchForm = new Search_Recovery_Invoice();
            if (searchForm.ShowDialog() == DialogResult.OK)
            {
                LoadRecoveryForEditing(searchForm.SelectedRecoveryID);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentRow.Index >= 0)
            {
                if (MessageBox.Show("Remove this item?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    recoveryItems.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    UpdateTotals();
                }
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void recoverytxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void disctxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void invoiceamounttxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void recoveredtxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateBalance();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
            {
                LoadVendorData(vendoridtxtbox.Text);
                // Note: Vendor recovery functionality can be added here if needed
                //MessageBox.Show("Vendor recovery functionality is not implemented in this version.");
            }
        }

        private void LoadVendorData(string vendorID)
        {
            try
            {
                string query = @"
                    SELECT PartyName 
                    FROM Parties 
                    WHERE PartyID = @VendorID AND PartyType = 'V'";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@VendorID", vendorID);
                        object result = cmd.ExecuteScalar();
                        fetchedvendornametxtbox.Text = result != null ? result.ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor: " + ex.Message);
            }
        }

        // Add the missing event handlers that are referenced in designer
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            // This can be left empty or add date validation logic if needed
        }

        private void chqbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Radio button checked change - nothing needed usually
        }

        private void onlinebtn_CheckedChanged(object sender, EventArgs e)
        {
            // Radio button checked change - nothing needed usually
        }

        private void vpaymentbtn_CheckedChanged(object sender, EventArgs e)
        {
            // Radio button checked change - nothing needed usually
        }

        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed event for customer ID textbox
        }

        private void cstmrbalancetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed event for customer balance textbox
        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed event for narration textbox
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Cell content click event - can be left empty
        }

        private void totaltxtbox1_TextChanged(object sender, EventArgs e)
        {
            // Total textbox changed event
        }

        private void totaltxtbox2_TextChanged(object sender, EventArgs e)
        {
            // Total textbox changed event
        }

        private void totaltxtbox14_TextChanged(object sender, EventArgs e)
        {
            // Total textbox changed event
        }

        private void totaltxtbox3_TextChanged(object sender, EventArgs e)
        {
            // Total textbox changed event
        }

        private void saleidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Sale ID textbox changed event
        }

        private void customeridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Customer ID textbox changed event
        }

        private void customernametxtbox_TextChanged_1(object sender, EventArgs e)
        {
            // Customer name textbox changed event
        }

        private void townandsectortxtbox_TextChanged(object sender, EventArgs e)
        {
            // Town/Sector textbox changed event
        }

        private void balancetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Balance textbox changed event
        }

        private void vendorpartyrecoverytxtbox_TextChanged(object sender, EventArgs e)
        {
            // Vendor party recovery textbox changed event
        }

        // Note: The cashbtn_CheckedChanged event is commented out in designer, so we don't need to implement it
    }
}