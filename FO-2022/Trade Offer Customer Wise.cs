//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Trade_Offer_Customer_Wise : Form
//    {
//        public Trade_Offer_Customer_Wise()
//        {
//            InitializeComponent();
//        }

//        private void label4_Click(object sender, EventArgs e)
//        {

//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void slabidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void datepckr_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void cstmrididsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void cstmrnametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void startdatetimepicker_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void enddatetimepicker_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void slctcstmrtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void newbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void editbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {

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
using System.Configuration;

namespace FO_2022
{
    public partial class Trade_Offer_Customer_Wise : Form
    {
        private string connectionString;
        private int currentSlabID;
        private DataTable tempDataTable;
        private DataTable savedDataTable;


        public Trade_Offer_Customer_Wise()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            InitializeForm();
            LoadCustomerTOData();
        }

        private void InitializeForm()
        {
            // Initialize data tables
            tempDataTable = new DataTable();
            savedDataTable = new DataTable();

            // Setup temp data table structure
            SetupDataTables();

            // Load next SlabID
            LoadNextSlabID();

            // Set dates
            datepckr.Value = DateTime.Now;
            startdatetimepicker.Value = DateTime.Now;
            enddatetimepicker.Value = DateTime.Now.AddDays(30);

            // Disable SlabID textbox
            slabidtxtbox.Enabled = false;

            // Setup data gridviews
            SetupDataGridViews();

            // Set up auto-search timer for product
            InitializeAutoSearchTimer();
        }

        private void InitializeAutoSearchTimer()
        {
            // Create a timer for delayed auto-search
            Timer searchTimer = new Timer();
            searchTimer.Interval = 500; // 500ms delay
            searchTimer.Tick += (s, e) =>
            {
                searchTimer.Stop();
                if (!string.IsNullOrWhiteSpace(productidtxtbox.Text))
                {
                    AutoSearchProduct();
                }
            };

            // Attach to text changed event
            productidtxtbox.TextChanged += (s, e) =>
            {
                searchTimer.Stop();
                searchTimer.Start();
            };
        }

        private void SetupDataTables()
        {
            // Setup temp data table structure
            tempDataTable.Columns.Add("ProductID", typeof(string));
            tempDataTable.Columns.Add("ProductName", typeof(string));
            tempDataTable.Columns.Add("PackingID", typeof(int));
            tempDataTable.Columns.Add("PackingName", typeof(string));
            tempDataTable.Columns.Add("Qty", typeof(int));
            tempDataTable.Columns.Add("Rate", typeof(decimal));
            tempDataTable.Columns.Add("Disc", typeof(decimal));
            tempDataTable.Columns.Add("Disc2", typeof(decimal));
            tempDataTable.Columns.Add("Total", typeof(decimal));

            // Setup saved data table structure
            savedDataTable.Columns.Add("SerialNo", typeof(int));
            savedDataTable.Columns.Add("ProductID", typeof(string));
            savedDataTable.Columns.Add("ProductName", typeof(string));
            savedDataTable.Columns.Add("PackingID", typeof(int));
            savedDataTable.Columns.Add("PackingName", typeof(string));
            savedDataTable.Columns.Add("Qty", typeof(int));
            savedDataTable.Columns.Add("Rate", typeof(decimal));
            savedDataTable.Columns.Add("Disc", typeof(decimal));
            savedDataTable.Columns.Add("Disc2", typeof(decimal));
            savedDataTable.Columns.Add("Total", typeof(decimal));
            savedDataTable.Columns.Add("IsDeleted", typeof(bool));
        }

        private void SetupDataGridViews()
        {
            // Setup dataGridView1 (temp items)
            dataGridView1.DataSource = tempDataTable;

            // Setup dataGridView2 (saved items)
            dataGridView2.DataSource = savedDataTable;
            dataGridView2.Columns["IsDeleted"].Visible = false;

            // Make grids read-only
            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;

            // Format columns
            FormatDataGridColumns();
        }

        private void FormatDataGridColumns()
        {
            // Format dataGridView1
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns["Rate"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Disc"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Disc2"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["Total"].DefaultCellStyle.Format = "N2";
            }

            // Format dataGridView2
            if (dataGridView2.Columns.Count > 0)
            {
                dataGridView2.Columns["Rate"].DefaultCellStyle.Format = "N2";
                dataGridView2.Columns["Disc"].DefaultCellStyle.Format = "N2";
                dataGridView2.Columns["Disc2"].DefaultCellStyle.Format = "N2";
                dataGridView2.Columns["Total"].DefaultCellStyle.Format = "N2";
            }
        }

        private void LoadNextSlabID()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(SlabID), 0) + 1 FROM CustomerTO";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    currentSlabID = Convert.ToInt32(cmd.ExecuteScalar());
                    slabidtxtbox.Text = currentSlabID.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading SlabID: " + ex.Message);
                currentSlabID = 1;
                slabidtxtbox.Text = "1";
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Not needed
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void slabidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Not needed
        }

        private void datepckr_ValueChanged(object sender, EventArgs e)
        {
            // Not needed
        }

        private void cstmridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-search customer as user types (optional)
            // You can add timer-based search here similar to product search
        }

        private void cstmrididsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
            {
                MessageBox.Show("Please enter a Customer ID");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName, SectorID, Address, City, Phone1 FROM Parties WHERE PartyID = @PartyID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PartyID", cstmridtxtbox.Text.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cstmrnametxtbox.Text = reader["PartyName"].ToString();
                            // You can display other fields in additional textboxes if needed
                        }
                        else
                        {
                            MessageBox.Show("Customer not found!");
                            cstmrnametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching customer: " + ex.Message);
            }
        }

        private void cstmrnametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Not needed
        }

        private void AutoSearchProduct()
        {
            if (string.IsNullOrWhiteSpace(productidtxtbox.Text))
            {
                ClearProductFields();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT p.ProductName, 
                               pp.PackingId,
                               pa.PackingName,
                               pp.Multiplier as Qty
                        FROM Products p
                        INNER JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                        INNER JOIN Packings pa ON pp.PackingId = pa.PackingId
                        WHERE p.ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productnametxtbox.Text = reader["ProductName"].ToString();

                            // Fill packing combo box
                            packnamecombobox.Items.Clear();
                            packnamecombobox.DisplayMember = "PackingName";
                            packnamecombobox.ValueMember = "PackingId";

                            // Create DataTable for combo box
                            DataTable packingTable = new DataTable();
                            packingTable.Columns.Add("PackingId", typeof(int));
                            packingTable.Columns.Add("PackingName", typeof(string));
                            packingTable.Columns.Add("Qty", typeof(int));

                            // Add first row
                            DataRow row = packingTable.NewRow();
                            row["PackingId"] = reader["PackingId"];
                            row["PackingName"] = reader["PackingName"];
                            row["Qty"] = reader["Qty"];
                            packingTable.Rows.Add(row);

                            // Read remaining rows
                            while (reader.Read())
                            {
                                row = packingTable.NewRow();
                                row["PackingId"] = reader["PackingId"];
                                row["PackingName"] = reader["PackingName"];
                                row["Qty"] = reader["Qty"];
                                packingTable.Rows.Add(row);
                            }

                            packnamecombobox.DataSource = packingTable;
                            packnamecombobox.SelectedIndex = 0;

                            // Set Qty
                            qtyptxtbox.Text = packingTable.Rows[0]["Qty"].ToString();

                            // Load price
                            LoadProductPrice(productidtxtbox.Text.Trim(), Convert.ToInt32(packingTable.Rows[0]["PackingId"]));
                        }
                        else
                        {
                            ClearProductFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClearProductFields();
            }
        }

        private void productidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(productidtxtbox.Text))
            {
                MessageBox.Show("Please enter a Product ID");
                return;
            }

            AutoSearchProduct();
        }

        private void ClearProductFields()
        {
            productnametxtbox.Clear();
            //packnamecombobox.Items.Clear();
            qtyptxtbox.Clear();
            ratetxt.Clear();
            disctxtbox.Clear();
            disctwotxt.Clear();
            packnamecombobox.DataSource = null; // Yeh line pehle add karein
            packnamecombobox.Items.Clear();
        }

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packnamecombobox.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)packnamecombobox.SelectedItem;
                qtyptxtbox.Text = selectedRow["Qty"].ToString();

                // Load price from CurrentStock or another table
                LoadProductPrice(productidtxtbox.Text.Trim(), Convert.ToInt32(selectedRow["PackingId"]));
            }
        }

        private void LoadProductPrice(string productId, int packingId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 1 PerUnitCostLose 
                        FROM CurrentStock 
                        WHERE ProductiD = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        decimal price = Convert.ToDecimal(result);
                        ratetxt.Text = price.ToString("N2");
                    }
                    else
                    {
                        ratetxt.Text = "0.00";
                    }
                }
            }
            catch (Exception ex)
            {
                ratetxt.Text = "0.00";
            }
            finally
            {
                CalculateAndDisplay();
            }
        }

        private void CalculateAndDisplay()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ratetxt.Text) ||
                    string.IsNullOrWhiteSpace(qtyptxtbox.Text) ||
                    string.IsNullOrWhiteSpace(disctxtbox.Text) ||
                    string.IsNullOrWhiteSpace(disctwotxt.Text))
                {
                    return;
                }

                decimal rate = decimal.Parse(ratetxt.Text);
                int qty = int.Parse(qtyptxtbox.Text);
                decimal disc = decimal.Parse(disctxtbox.Text);
                decimal disc2 = decimal.Parse(disctwotxt.Text);

                // Calculate total with discounts
                decimal subtotal = rate * qty;
                decimal discount1Amount = subtotal * (disc / 100);
                decimal amountAfterDisc1 = subtotal - discount1Amount;
                decimal discount2Amount = amountAfterDisc1 * (disc2 / 100);
                decimal total = amountAfterDisc1 - discount2Amount;

                // Display calculated values
                // You can add a calculated total textbox if needed
            }
            catch
            {
                // Ignore calculation errors if fields are empty
            }
        }

        private void addbtn_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(productidtxtbox.Text) ||
                string.IsNullOrWhiteSpace(productnametxtbox.Text) ||
                packnamecombobox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(ratetxt.Text) ||
                string.IsNullOrWhiteSpace(disctxtbox.Text) ||
                string.IsNullOrWhiteSpace(disctwotxt.Text))
            {
                MessageBox.Show("Please fill all product details first");
                return;
            }

            try
            {
                DataRowView selectedPacking = (DataRowView)packnamecombobox.SelectedItem;

                // Get values
                int qty = int.Parse(qtyptxtbox.Text);
                decimal rate = decimal.Parse(ratetxt.Text);
                decimal disc = decimal.Parse(disctxtbox.Text);
                decimal disc2 = decimal.Parse(disctwotxt.Text);
                decimal total = rate * qty * (1 - disc / 100) * (1 - disc2 / 100);

                // Add to temp data table
                DataRow newRow = tempDataTable.NewRow();
                newRow["ProductID"] = productidtxtbox.Text.Trim();
                newRow["ProductName"] = productnametxtbox.Text;
                newRow["PackingID"] = selectedPacking["PackingId"];
                newRow["PackingName"] = selectedPacking["PackingName"];
                newRow["Qty"] = qty;
                newRow["Rate"] = rate;
                newRow["Disc"] = disc;
                newRow["Disc2"] = disc2;
                newRow["Total"] = total;

                tempDataTable.Rows.Add(newRow);

                // Clear product fields for next entry
                ClearProductFields();
                productidtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }

        private void startdatetimepicker_ValueChanged(object sender, EventArgs e)
        {
            // Not needed
        }

        private void enddatetimepicker_ValueChanged(object sender, EventArgs e)
        {
            // Not needed
        }

      
        private void slctcstmrtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(slctcstmrtxtbox.Text))
            {
                // Agar textbox empty hai to saara data show karein
                LoadCustomerTOData();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string searchText = slctcstmrtxtbox.Text.Trim();
                    string query = @"
                SELECT 
                    ct.SlabID,
                    ct.EntryDate,
                    ct.PartyID,
                    p.PartyName,
                    ct.StartDate,
                    ct.EndDate,
                    COUNT(ctb.SerialNo) as TotalItems
                FROM CustomerTO ct
                LEFT JOIN Parties p ON ct.PartyID = p.PartyID
                LEFT JOIN CustomerTOBody ctb ON ct.SlabID = ctb.SlabID AND ctb.IsDeleted = 0
                WHERE ct.SlabID LIKE @SearchID OR ct.PartyID LIKE @SearchID OR p.PartyName LIKE @SearchName
                GROUP BY ct.SlabID, ct.EntryDate, ct.PartyID, p.PartyName, ct.StartDate, ct.EndDate
                ORDER BY ct.EntryDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SearchID", "%" + searchText + "%");
                    cmd.Parameters.AddWithValue("@SearchName", "%" + searchText + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable searchTable = new DataTable();
                    adapter.Fill(searchTable);

                    dataGridView2.DataSource = searchTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching CustomerTO data: " + ex.Message);
            }
        }
      

      

        private void LoadSavedItems(int slabID)
        {
            try
            {
                // Pehle savedDataTable clear karna
                savedDataTable.Clear();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    ctb.SerialNo,
                    ctb.ProductID,
                    p.ProductName,
                    ctb.PackingId,
                    pa.PackingName,
                    ctb.Qty,
                    ctb.Rate,
                    ctb.Disc,
                    ctb.Disc2,
                    (ctb.Qty * ctb.Rate * (1 - ctb.Disc/100) * (1 - ctb.Disc2/100)) as Total
                FROM CustomerTOBody ctb
                INNER JOIN Products p ON ctb.ProductID = p.ProductID
                LEFT JOIN Packings pa ON ctb.PackingId = pa.PackingId
                WHERE ctb.SlabID = @SlabID AND ctb.IsDeleted = 0
                ORDER BY ctb.SerialNo";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SlabID", slabID);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable itemsTable = new DataTable();
                    adapter.Fill(itemsTable);

                    // savedDataTable mein data add karna
                    foreach (DataRow row in itemsTable.Rows)
                    {
                        DataRow newRow = savedDataTable.NewRow();
                        newRow["SerialNo"] = row["SerialNo"];
                        newRow["ProductID"] = row["ProductID"];
                        newRow["ProductName"] = row["ProductName"];
                        newRow["PackingID"] = row["PackingId"];
                        newRow["PackingName"] = row["PackingName"];
                        newRow["Qty"] = row["Qty"];
                        newRow["Rate"] = row["Rate"];
                        newRow["Disc"] = row["Disc"];
                        newRow["Disc2"] = row["Disc2"];
                        newRow["Total"] = row["Total"];
                        newRow["IsDeleted"] = false;

                        savedDataTable.Rows.Add(newRow);
                    }

                    // DataGridView refresh karna
                    dataGridView2.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading saved items: " + ex.Message);
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            // Clear all fields and reset form
            ClearAllFields();
            LoadNextSlabID();
            tempDataTable.Clear();
            savedDataTable.Clear();
            datepckr.Value = DateTime.Now;
            startdatetimepicker.Value = DateTime.Now;
            enddatetimepicker.Value = DateTime.Now.AddDays(30);
        }

        private void ClearAllFields()
        {
            // Clear customer fields
            cstmridtxtbox.Clear();
            cstmrnametxtbox.Clear();
            slctcstmrtxtbox.Clear();

            // Clear product fields
            ClearProductFields();
            productidtxtbox.Clear();

            // Clear other fields
            // slabidtxtbox is handled by LoadNextSlabID()
        }

       
        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row from CustomerTO list first");
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer first");
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
                        // CustomerTO table update karna
                        string updateHeaderQuery = @"
                    UPDATE CustomerTO 
                    SET EntryDate = @EntryDate,
                        PartyID = @PartyID,
                        StartDate = @StartDate,
                        EndDate = @EndDate
                    WHERE SlabID = @SlabID";

                        SqlCommand headerCmd = new SqlCommand(updateHeaderQuery, conn, transaction);
                        headerCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                        headerCmd.Parameters.AddWithValue("@EntryDate", datepckr.Value);
                        headerCmd.Parameters.AddWithValue("@PartyID", cstmridtxtbox.Text.Trim());
                        headerCmd.Parameters.AddWithValue("@StartDate", startdatetimepicker.Value);
                        headerCmd.Parameters.AddWithValue("@EndDate", enddatetimepicker.Value);
                        headerCmd.ExecuteNonQuery();

                        // Purane items ko mark as deleted karna
                        string markDeletedQuery = "UPDATE CustomerTOBody SET IsDeleted = 1 WHERE SlabID = @SlabID";
                        SqlCommand deleteCmd = new SqlCommand(markDeletedQuery, conn, transaction);
                        deleteCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                        deleteCmd.ExecuteNonQuery();

                        // Temp table ke items ko save karna
                        int serialNo = 1;
                        foreach (DataRow row in tempDataTable.Rows)
                        {
                            string insertDetailQuery = @"
                        INSERT INTO CustomerTOBody (SerialNo, SlabID, ProductID, Qty, Rate, Disc, Disc2, IsDeleted, PackingId)
                        VALUES (@SerialNo, @SlabID, @ProductID, @Qty, @Rate, @Disc, @Disc2, 0, @PackingId)";

                            SqlCommand detailCmd = new SqlCommand(insertDetailQuery, conn, transaction);
                            detailCmd.Parameters.AddWithValue("@SerialNo", serialNo);
                            detailCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                            detailCmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                            detailCmd.Parameters.AddWithValue("@Qty", row["Qty"]);
                            detailCmd.Parameters.AddWithValue("@Rate", row["Rate"]);
                            detailCmd.Parameters.AddWithValue("@Disc", row["Disc"]);
                            detailCmd.Parameters.AddWithValue("@Disc2", row["Disc2"]);
                            detailCmd.Parameters.AddWithValue("@PackingId", row["PackingID"]);
                            detailCmd.ExecuteNonQuery();

                            serialNo++;
                        }

                        // Saved items ko bhi update karna
                        foreach (DataRow row in savedDataTable.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted && Convert.ToBoolean(row["IsDeleted"]) == false)
                            {
                                string updateDetailQuery = @"
                            UPDATE CustomerTOBody 
                            SET ProductID = @ProductID,
                                Qty = @Qty,
                                Rate = @Rate,
                                Disc = @Disc,
                                Disc2 = @Disc2,
                                PackingId = @PackingId,
                                IsDeleted = 0
                            WHERE SlabID = @SlabID AND SerialNo = @SerialNo";

                                SqlCommand updateCmd = new SqlCommand(updateDetailQuery, conn, transaction);
                                updateCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                                updateCmd.Parameters.AddWithValue("@SerialNo", row["SerialNo"]);
                                updateCmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                                updateCmd.Parameters.AddWithValue("@Qty", row["Qty"]);
                                updateCmd.Parameters.AddWithValue("@Rate", row["Rate"]);
                                updateCmd.Parameters.AddWithValue("@Disc", row["Disc"]);
                                updateCmd.Parameters.AddWithValue("@Disc2", row["Disc2"]);
                                updateCmd.Parameters.AddWithValue("@PackingId", row["PackingID"]);
                                updateCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();

                        // Refresh data
                        LoadCustomerTOData();
                        LoadSavedItems(currentSlabID);
                        tempDataTable.Clear();

                        MessageBox.Show("Trade offer updated successfully!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error updating trade offer: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(cstmridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer first");
                return;
            }

            if (tempDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Begin transaction
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Save to CustomerTO table
                        string headerQuery = @"
                            INSERT INTO CustomerTO (SlabID, EntryDate, PartyID, StartDate, EndDate)
                            VALUES (@SlabID, @EntryDate, @PartyID, @StartDate, @EndDate)";

                        SqlCommand headerCmd = new SqlCommand(headerQuery, conn, transaction);
                        headerCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                        headerCmd.Parameters.AddWithValue("@EntryDate", datepckr.Value);
                        headerCmd.Parameters.AddWithValue("@PartyID", cstmridtxtbox.Text.Trim());
                        headerCmd.Parameters.AddWithValue("@StartDate", startdatetimepicker.Value);
                        headerCmd.Parameters.AddWithValue("@EndDate", enddatetimepicker.Value);
                        headerCmd.ExecuteNonQuery();

                        // Save to CustomerTOBody table
                        int serialNo = 1;
                        foreach (DataRow row in tempDataTable.Rows)
                        {
                            string detailQuery = @"
                                INSERT INTO CustomerTOBody (SerialNo, SlabID, ProductID, Qty, Rate, Disc, Disc2, IsDeleted, PackingId)
                                VALUES (@SerialNo, @SlabID, @ProductID, @Qty, @Rate, @Disc, @Disc2, 0, @PackingId)";

                            SqlCommand detailCmd = new SqlCommand(detailQuery, conn, transaction);
                            detailCmd.Parameters.AddWithValue("@SerialNo", serialNo);
                            detailCmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                            detailCmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                            detailCmd.Parameters.AddWithValue("@Qty", row["Qty"]);
                            detailCmd.Parameters.AddWithValue("@Rate", row["Rate"]);
                            detailCmd.Parameters.AddWithValue("@Disc", row["Disc"]);
                            detailCmd.Parameters.AddWithValue("@Disc2", row["Disc2"]);
                            detailCmd.Parameters.AddWithValue("@PackingId", row["PackingID"]);
                            detailCmd.ExecuteNonQuery();

                            // Add to savedDataTable for display
                            DataRow savedRow = savedDataTable.NewRow();
                            savedRow["SerialNo"] = serialNo;
                            savedRow["ProductID"] = row["ProductID"];
                            savedRow["ProductName"] = row["ProductName"];
                            savedRow["PackingID"] = row["PackingID"];
                            savedRow["PackingName"] = row["PackingName"];
                            savedRow["Qty"] = row["Qty"];
                            savedRow["Rate"] = row["Rate"];
                            savedRow["Disc"] = row["Disc"];
                            savedRow["Disc2"] = row["Disc2"];
                            savedRow["Total"] = row["Total"];
                            savedRow["IsDeleted"] = false;
                            savedDataTable.Rows.Add(savedRow);

                            serialNo++;
                        }

                        // Commit transaction
                        transaction.Commit();

                        // Clear temp table
                        tempDataTable.Clear();

                        MessageBox.Show("Trade offer saved successfully!");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error saving trade offer: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to remove");
                return;
            }

            if (MessageBox.Show("Are you sure you want to remove this item?", "Confirm Removal",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];
                    int serialNo = Convert.ToInt32(selectedRow.Cells["SerialNo"].Value);

                    // Mark as deleted in database
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE CustomerTOBody SET IsDeleted = 1 WHERE SlabID = @SlabID AND SerialNo = @SerialNo";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@SlabID", currentSlabID);
                        cmd.Parameters.AddWithValue("@SerialNo", serialNo);
                        cmd.ExecuteNonQuery();
                    }

                    // Remove from savedDataTable
                    DataRow[] rows = savedDataTable.Select($"SerialNo = {serialNo}");
                    if (rows.Length > 0)
                    {
                        rows[0].Delete();
                        savedDataTable.AcceptChanges();
                    }

                    // Remove from data grid view
                    dataGridView2.Rows.Remove(selectedRow);

                    MessageBox.Show("Item removed successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error removing item: " + ex.Message);
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            tempDataTable.Clear();
            productidtxtbox.Focus();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Text changed events for auto-calculation
        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplay();
        }

        private void ratetxt_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplay();
        }

        private void disctxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplay();
        }

        private void disctwotxt_TextChanged(object sender, EventArgs e)
        {
            CalculateAndDisplay();
        }
        private void LoadCustomerTOData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // CustomerTO table se data load karna
                    string query = @"
                SELECT 
                    ct.SlabID,
                    ct.EntryDate,
                    ct.PartyID,
                    p.PartyName,
                    ct.StartDate,
                    ct.EndDate,
                    COUNT(ctb.SerialNo) as TotalItems
                FROM CustomerTO ct
                LEFT JOIN Parties p ON ct.PartyID = p.PartyID
                LEFT JOIN CustomerTOBody ctb ON ct.SlabID = ctb.SlabID AND ctb.IsDeleted = 0
                GROUP BY ct.SlabID, ct.EntryDate, ct.PartyID, p.PartyName, ct.StartDate, ct.EndDate
                ORDER BY ct.EntryDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable customerTOTable = new DataTable();
                    adapter.Fill(customerTOTable);

                    // DataGridView2 mein data bind karna
                    dataGridView2.DataSource = customerTOTable;

                    // Column headers set karna
                    dataGridView2.Columns["SlabID"].HeaderText = "Slab ID";
                    dataGridView2.Columns["EntryDate"].HeaderText = "Entry Date";
                    dataGridView2.Columns["PartyID"].HeaderText = "Customer ID";
                    dataGridView2.Columns["PartyName"].HeaderText = "Customer Name";
                    dataGridView2.Columns["StartDate"].HeaderText = "Start Date";
                    dataGridView2.Columns["EndDate"].HeaderText = "End Date";
                    dataGridView2.Columns["TotalItems"].HeaderText = "Total Items";

                    // Date formatting
                    dataGridView2.Columns["EntryDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dataGridView2.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dataGridView2.Columns["EndDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

                    // Column widths adjust karna
                    dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CustomerTO data: " + ex.Message);
            }
        }

        private void disctwotxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Add product when Enter is pressed in disc2 textbox
                addbtn_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Trigger search when Enter is pressed
                productidsearch_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;

                // Move to next field
                packnamecombobox.Focus();
            }
        }

        private void cstmridtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Trigger customer search when Enter is pressed
                cstmrididsearch_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;

                // Move to next field
                productidtxtbox.Focus();
            }
        }

        private void ratetxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Move to next field
                disctxtbox.Focus();
                disctxtbox.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void disctxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Move to next field
                disctwotxt.Focus();
                disctwotxt.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-search is handled by timer in InitializeAutoSearchTimer()
        }

        private void packnamecombobox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Move to next field
                qtyptxtbox.Focus();
                qtyptxtbox.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Move to next field
                ratetxt.Focus();
                ratetxt.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void LoadGrid1Data()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductID, ProductName, CompanyID, SalePrice FROM Products";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Manual column headers
                dt.Columns["ProductID"].ColumnName = "Product ID";
                dt.Columns["ProductName"].ColumnName = "Product Name";
                dt.Columns["CompanyID"].ColumnName = "Company ID";
                dt.Columns["SalePrice"].ColumnName = "Sale Price";

                dataGridView1.DataSource = dt;
            }
        }

        private void LoadGrid2Data()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP 80000 SlabID, PartyID, StartDate, EndDate, EntryDate FROM CustomerTO";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Manual column headers
                dt.Columns["SlabID"].ColumnName = "Slab ID";
                dt.Columns["PartyID"].ColumnName = "Party ID";
                dt.Columns["StartDate"].ColumnName = "Start Date";
                dt.Columns["EndDate"].ColumnName = "End Date";
                dt.Columns["EntryDate"].ColumnName = "Entry Date";

                dataGridView2.DataSource = dt;
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Trade_Offer_Customer_Wise_Load(object sender, EventArgs e)
        {
            LoadGrid1Data();
            LoadGrid2Data();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}