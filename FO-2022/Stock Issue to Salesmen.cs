//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Data.SqlClient;

//namespace FO_2022
//{
//    public partial class Stock_Issue_to_Salesmen : Form
//    {
//        // Connection string from App.config
//        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

//        // Variables to store data
//        private List<ProductItem> productItems = new List<ProductItem>();
//        private DataTable dtPackings = new DataTable();
//        private int nextIssueId = 1;
//        private bool isDataLoaded = false;

//        // Store current product cost
//        private decimal currentProductCost = 0;

//        // Class to hold product data
//        public class ProductItem
//        {
//            public string ProductID { get; set; }
//            public string ProductName { get; set; }
//            public string PackName { get; set; }
//            public int Pack { get; set; }
//            public int QtyPack { get; set; }
//            public int QtyLose { get; set; }
//            public decimal PerUnitCostLose { get; set; }
//            public decimal TTLValue { get; set; }
//        }

//        public Stock_Issue_to_Salesmen()
//        {
//            InitializeComponent();
//        }

//        private void Stock_Issue_to_Salesmen_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                // Add columns to grid programmatically
//                AddColumnsToGrid();

//                // Load data when form loads
//                LoadNextIssueId();
//                LoadPackings();
//                isDataLoaded = true;

//                // Set current date
//                issuedatepcker.Value = DateTime.Today;

//                // Set focus to salesman ID
//                salesmenidtxtbox.Focus();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void AddColumnsToGrid()
//        {
//            // Clear any existing columns
//            dataGridView1.Columns.Clear();

//            // Add columns programmatically
//            // Column 1: Pro ID
//            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
//            col1.HeaderText = "Pro ID";
//            col1.Name = "colProductID";
//            col1.Width = 85;
//            dataGridView1.Columns.Add(col1);

//            // Column 2: Product Name
//            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
//            col2.HeaderText = "Product Name";
//            col2.Name = "colProductName";
//            col2.Width = 115;
//            dataGridView1.Columns.Add(col2);

//            // Column 3: Pack Name (TextBox, not ComboBox)
//            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
//            col3.HeaderText = "Pack Name";
//            col3.Name = "colPackName";
//            col3.Width = 90;
//            dataGridView1.Columns.Add(col3);

//            // Column 4: Pack 
//            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
//            col4.HeaderText = "Pack";
//            col4.Name = "colPack";
//            col4.Width = 50;
//            dataGridView1.Columns.Add(col4);

//            // Column 5: Qty Pack
//            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
//            col5.HeaderText = "Qty Pack";
//            col5.Name = "colQtyPack";
//            col5.Width = 90;
//            dataGridView1.Columns.Add(col5);

//            // Column 6: Qty Loose
//            DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
//            col6.HeaderText = "Qty Loose";
//            col6.Name = "colQtyLoose";
//            col6.Width = 95;
//            dataGridView1.Columns.Add(col6);
//        }

//        private void LoadNextIssueId()
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();
//                    string query = "SELECT ISNULL(MAX(IssueID), 0) + 1 AS NextIssueID FROM StockIssue";
//                    SqlCommand cmd = new SqlCommand(query, con);
//                    object result = cmd.ExecuteScalar();

//                    if (result != null && result != DBNull.Value)
//                    {
//                        nextIssueId = Convert.ToInt32(result);
//                        issueidtxtbox.Text = nextIssueId.ToString();
//                    }
//                    else
//                    {
//                        issueidtxtbox.Text = "1";
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading next Issue ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                issueidtxtbox.Text = "1";
//            }
//        }

//        private void LoadPackings()
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();
//                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
//                    SqlDataAdapter da = new SqlDataAdapter(query, con);
//                    dtPackings.Clear();
//                    da.Fill(dtPackings);

//                    // Populate the combobox above the grid (not in the grid)
//                    packnamecombobox.DataSource = dtPackings;
//                    packnamecombobox.DisplayMember = "PackingName";
//                    packnamecombobox.ValueMember = "PackingId";
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadProductCost(string productId)
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();

//                    // Get product purchase price from Products table
//                    string query = @"
//                        SELECT PurchasePrice, PurDiscRatio, PurDiscValue, PurSTRatio 
//                        FROM Products 
//                        WHERE ProductID = @ProductID";

//                    SqlCommand cmd = new SqlCommand(query, con);
//                    cmd.Parameters.AddWithValue("@ProductID", productId);

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            decimal purchasePrice = Convert.ToDecimal(reader["PurchasePrice"]);
//                            decimal purDiscRatio = Convert.ToDecimal(reader["PurDiscRatio"]);
//                            decimal purDiscValue = Convert.ToDecimal(reader["PurDiscValue"]);
//                            decimal purSTRatio = Convert.ToDecimal(reader["PurSTRatio"]);

//                            // Calculate actual cost per unit (pack)
//                            // Formula: Purchase Price - Discount + Sales Tax
//                            decimal discountAmount = purchasePrice * (purDiscRatio / 100);
//                            discountAmount = Math.Max(discountAmount, purDiscValue);
//                            decimal priceAfterDiscount = purchasePrice - discountAmount;
//                            decimal salesTaxAmount = priceAfterDiscount * (purSTRatio / 100);

//                            currentProductCost = priceAfterDiscount + salesTaxAmount;
//                        }
//                        else
//                        {
//                            currentProductCost = 0;
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading product cost: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                currentProductCost = 0;
//            }
//        }

//        private void salesmenidtxtbox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                if (!string.IsNullOrEmpty(salesmenidtxtbox.Text.Trim()))
//                {
//                    LoadSalesmanDetails();
//                }
//            }
//        }

//        private void LoadSalesmanDetails()
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();
//                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
//                    SqlCommand cmd = new SqlCommand(query, con);
//                    cmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text.Trim());

//                    object result = cmd.ExecuteScalar();
//                    if (result != null)
//                    {
//                        salesmennametxtbox.Text = result.ToString();
//                        productidtxtbox.Focus(); // Move focus to product ID
//                    }
//                    else
//                    {
//                        salesmennametxtbox.Text = "";
//                        MessageBox.Show("Salesman ID not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        salesmenidtxtbox.Focus();
//                        salesmenidtxtbox.SelectAll();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading salesman details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                if (!string.IsNullOrEmpty(productidtxtbox.Text.Trim()))
//                {
//                    LoadProductDetails();
//                }
//            }
//        }

//        private void LoadProductDetails()
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();

//                    // Get product details
//                    string query = @"
//                        SELECT p.ProductID, p.ProductName, pp.PackingId, pk.PackingName, pp.Multiplier 
//                        FROM Products p 
//                        LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId 
//                        LEFT JOIN Packings pk ON pp.PackingId = pk.PackingId 
//                        WHERE p.ProductID = @ProductID";

//                    SqlCommand cmd = new SqlCommand(query, con);
//                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            productnametxtbox.Text = reader["ProductName"].ToString();

//                            // Load product cost
//                            LoadProductCost(productidtxtbox.Text.Trim());

//                            // Set packing combobox
//                            if (reader["PackingId"] != DBNull.Value)
//                            {
//                                int packingId = Convert.ToInt32(reader["PackingId"]);
//                                packnamecombobox.SelectedValue = packingId;
//                                packtxtbox.Text = reader["Multiplier"].ToString();
//                            }
//                            else
//                            {
//                                packnamecombobox.SelectedIndex = -1;
//                                packtxtbox.Text = "1"; // Default multiplier
//                            }

//                            qtyptxtbox.Focus();
//                        }
//                        else
//                        {
//                            MessageBox.Show("Product ID not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                            ClearProductFields();
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading product details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            if (isDataLoaded && packnamecombobox.SelectedValue != null && !string.IsNullOrEmpty(productidtxtbox.Text))
//            {
//                LoadMultiplierForSelectedPacking();
//            }
//        }

//        private void LoadMultiplierForSelectedPacking()
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();
//                    string query = "SELECT Multiplier FROM ProductPacking WHERE ProductId = @ProductID AND PackingId = @PackingId";
//                    SqlCommand cmd = new SqlCommand(query, con);
//                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());
//                    cmd.Parameters.AddWithValue("@PackingId", packnamecombobox.SelectedValue);

//                    object result = cmd.ExecuteScalar();
//                    if (result != null && result != DBNull.Value)
//                    {
//                        packtxtbox.Text = result.ToString();
//                    }
//                    else
//                    {
//                        packtxtbox.Text = "1"; // Default multiplier
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading multiplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                if (!string.IsNullOrEmpty(qtyptxtbox.Text))
//                {
//                    // Calculate loose quantity
//                    CalculateLooseQuantity();
//                    qtyltxtbox.Focus();
//                }
//            }
//        }

//        private void CalculateLooseQuantity()
//        {
//            try
//            {
//                if (int.TryParse(qtyptxtbox.Text, out int qtyPack) && int.TryParse(packtxtbox.Text, out int multiplier))
//                {
//                    int qtyLose = qtyPack * multiplier;
//                    qtyltxtbox.Text = qtyLose.ToString();
//                }
//                else
//                {
//                    qtyltxtbox.Text = "0";
//                }
//            }
//            catch
//            {
//                qtyltxtbox.Text = "0";
//            }
//        }

//        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                if (!string.IsNullOrEmpty(qtyltxtbox.Text))
//                {
//                    AddProductToGrid();
//                }
//            }
//        }

//        private void AddProductToGrid()
//        {
//            // Validate required fields
//            if (string.IsNullOrEmpty(productidtxtbox.Text) || string.IsNullOrEmpty(productnametxtbox.Text))
//            {
//                MessageBox.Show("Please select a product first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                productidtxtbox.Focus();
//                return;
//            }

//            if (packnamecombobox.SelectedItem == null)
//            {
//                MessageBox.Show("Please select a packing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                packnamecombobox.Focus();
//                return;
//            }

//            if (!int.TryParse(qtyptxtbox.Text, out int qtyPack) || qtyPack <= 0)
//            {
//                MessageBox.Show("Please enter valid quantity pack!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                qtyptxtbox.Focus();
//                qtyptxtbox.SelectAll();
//                return;
//            }

//            if (!int.TryParse(qtyltxtbox.Text, out int qtyLose) || qtyLose <= 0)
//            {
//                MessageBox.Show("Please enter valid quantity loose!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                qtyltxtbox.Focus();
//                qtyltxtbox.SelectAll();
//                return;
//            }

//            if (!int.TryParse(packtxtbox.Text, out int pack))
//            {
//                pack = 1;
//            }

//            // Check if product already exists in grid
//            bool productExists = false;
//            foreach (DataGridViewRow gridRow in dataGridView1.Rows)
//            {
//                if (!gridRow.IsNewRow && gridRow.Cells["colProductID"].Value != null)
//                {
//                    if (gridRow.Cells["colProductID"].Value.ToString() == productidtxtbox.Text.Trim())
//                    {
//                        productExists = true;
//                        break;
//                    }
//                }
//            }

//            if (productExists)
//            {
//                MessageBox.Show("Product already exists in the list! Remove it first to add again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                ClearProductFields();
//                productidtxtbox.Focus();
//                return;
//            }

//            // Calculate PerUnitCostLose and TTLValue
//            decimal perUnitCostLose = 0;
//            decimal ttlValue = 0;

//            if (currentProductCost > 0 && pack > 0)
//            {
//                // PerUnitCostLose = Cost per pack ÷ Multiplier
//                perUnitCostLose = currentProductCost / pack;

//                // TTLValue = PerUnitCostLose × QtyLose
//                ttlValue = perUnitCostLose * qtyLose;
//            }

//            // Add to grid
//            int rowIndex = dataGridView1.Rows.Add();
//            DataGridViewRow newRow = dataGridView1.Rows[rowIndex];

//            newRow.Cells["colProductID"].Value = productidtxtbox.Text.Trim();
//            newRow.Cells["colProductName"].Value = productnametxtbox.Text.Trim();
//            newRow.Cells["colPackName"].Value = packnamecombobox.Text;
//            newRow.Cells["colPack"].Value = pack;
//            newRow.Cells["colQtyPack"].Value = qtyPack;
//            newRow.Cells["colQtyLoose"].Value = qtyLose;

//            // Store calculated values in ProductItem list
//            ProductItem item = new ProductItem
//            {
//                ProductID = productidtxtbox.Text.Trim(),
//                ProductName = productnametxtbox.Text.Trim(),
//                PackName = packnamecombobox.Text,
//                Pack = pack,
//                QtyPack = qtyPack,
//                QtyLose = qtyLose,
//                PerUnitCostLose = perUnitCostLose,
//                TTLValue = ttlValue
//            };
//            productItems.Add(item);

//            // Clear product fields for next entry
//            ClearProductFields();
//        }

//        private void ClearProductFields()
//        {
//            productidtxtbox.Clear();
//            productnametxtbox.Clear();
//            packnamecombobox.SelectedIndex = -1;
//            packtxtbox.Clear();
//            qtyptxtbox.Clear();
//            qtyltxtbox.Clear();
//            currentProductCost = 0;
//            productidtxtbox.Focus();
//        }

//        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // You can add functionality here if needed
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count > 0)
//            {
//                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
//                {
//                    if (!row.IsNewRow)
//                    {
//                        // Remove from productItems list
//                        string productId = row.Cells["colProductID"].Value?.ToString();
//                        if (!string.IsNullOrEmpty(productId))
//                        {
//                            ProductItem itemToRemove = productItems.FirstOrDefault(p => p.ProductID == productId);
//                            if (itemToRemove != null)
//                            {
//                                productItems.Remove(itemToRemove);
//                            }
//                        }

//                        // Remove from grid
//                        dataGridView1.Rows.Remove(row);
//                    }
//                }
//            }
//            else
//            {
//                MessageBox.Show("Please select a row to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Validate required fields
//                if (string.IsNullOrEmpty(salesmenidtxtbox.Text.Trim()))
//                {
//                    MessageBox.Show("Please enter Salesman ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    salesmenidtxtbox.Focus();
//                    return;
//                }

//                if (string.IsNullOrEmpty(salesmennametxtbox.Text.Trim()))
//                {
//                    MessageBox.Show("Salesman not found! Please enter a valid Salesman ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    salesmenidtxtbox.Focus();
//                    return;
//                }

//                if (dataGridView1.Rows.Count <= 1) // Only header row exists
//                {
//                    MessageBox.Show("Please add at least one product!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    productidtxtbox.Focus();
//                    return;
//                }

//                // Begin transaction
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();
//                    SqlTransaction transaction = con.BeginTransaction();

//                    try
//                    {
//                        int issueId = Convert.ToInt32(issueidtxtbox.Text);
//                        string salesmanId = salesmenidtxtbox.Text.Trim();

//                        // Save to StockIssue table
//                        string insertHeader = @"
//                            INSERT INTO StockIssue (IssueID, IssueDate, SalesmanID, IsUploaded) 
//                            VALUES (@IssueID, @IssueDate, @SalesmanID, 0)";

//                        SqlCommand cmdHeader = new SqlCommand(insertHeader, con, transaction);
//                        cmdHeader.Parameters.AddWithValue("@IssueID", issueId);
//                        cmdHeader.Parameters.AddWithValue("@IssueDate", issuedatepcker.Value);
//                        cmdHeader.Parameters.AddWithValue("@SalesmanID", salesmanId);
//                        cmdHeader.ExecuteNonQuery();

//                        // Save to StockIssueBody table with calculated values
//                        string insertBody = @"
//                            INSERT INTO StockIssueBody (IssueID, ProductID, PackingId, Multiplier, QtyPack, QtyLose, PerUnitCostLose, TTLValue) 
//                            VALUES (@IssueID, @ProductID, @PackingId, @Multiplier, @QtyPack, @QtyLose, @PerUnitCostLose, @TTLValue)";

//                        foreach (DataGridViewRow row in dataGridView1.Rows)
//                        {
//                            if (!row.IsNewRow && row.Cells["colProductID"].Value != null)
//                            {
//                                string productId = row.Cells["colProductID"].Value.ToString();

//                                // Find product in the list to get calculated values
//                                ProductItem productItem = productItems.FirstOrDefault(p => p.ProductID == productId);

//                                // Get PackingId from PackingName
//                                int packingId = 0;
//                                string packingName = row.Cells["colPackName"].Value?.ToString();
//                                if (!string.IsNullOrEmpty(packingName))
//                                {
//                                    DataRow[] rows = dtPackings.Select($"PackingName = '{packingName}'");
//                                    if (rows.Length > 0)
//                                    {
//                                        packingId = Convert.ToInt32(rows[0]["PackingId"]);
//                                    }
//                                }

//                                int multiplier = Convert.ToInt32(row.Cells["colPack"].Value);
//                                int qtyPack = Convert.ToInt32(row.Cells["colQtyPack"].Value);
//                                int qtyLose = Convert.ToInt32(row.Cells["colQtyLoose"].Value);

//                                SqlCommand cmdBody = new SqlCommand(insertBody, con, transaction);
//                                cmdBody.Parameters.AddWithValue("@IssueID", issueId);
//                                cmdBody.Parameters.AddWithValue("@ProductID", productId);
//                                cmdBody.Parameters.AddWithValue("@PackingId", packingId);
//                                cmdBody.Parameters.AddWithValue("@Multiplier", multiplier);
//                                cmdBody.Parameters.AddWithValue("@QtyPack", qtyPack);
//                                cmdBody.Parameters.AddWithValue("@QtyLose", qtyLose);

//                                // Use calculated values from productItems
//                                if (productItem != null)
//                                {
//                                    cmdBody.Parameters.AddWithValue("@PerUnitCostLose", productItem.PerUnitCostLose);
//                                    cmdBody.Parameters.AddWithValue("@TTLValue", productItem.TTLValue);
//                                }
//                                else
//                                {
//                                    // Fallback calculation if item not found in list
//                                    // Get product cost
//                                    LoadProductCost(productId);
//                                    decimal perUnitCostLose = currentProductCost / (multiplier > 0 ? multiplier : 1);
//                                    decimal ttlValue = perUnitCostLose * qtyLose;

//                                    cmdBody.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
//                                    cmdBody.Parameters.AddWithValue("@TTLValue", ttlValue);
//                                }

//                                cmdBody.ExecuteNonQuery();

//                                // Update SMCurrentStock table
//                                UpdateSMCurrentStock(con, transaction, salesmanId, productId, qtyLose, productItem?.PerUnitCostLose ?? 0);

//                                // Update SMOpeningStock table
//                                UpdateSMOpeningStock(con, transaction, salesmanId, productId, packingId, multiplier, qtyPack, qtyLose, productItem?.PerUnitCostLose ?? 0);
//                            }
//                        }

//                        // Commit transaction
//                        transaction.Commit();

//                        MessageBox.Show("Stock issue saved successfully!\nData inserted into:\n1. StockIssue\n2. StockIssueBody\n3. SMCurrentStock\n4. SMOpeningStock",
//                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                        // Refresh form for new entry
//                        ClearForm();
//                        LoadNextIssueId();
//                    }
//                    catch (Exception ex)
//                    {
//                        transaction.Rollback();
//                        throw ex;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void UpdateSMCurrentStock(SqlConnection con, SqlTransaction transaction, string salesmanId, string productId, int qtyLose, decimal perUnitCostLose)
//        {
//            try
//            {
//                // Check if record exists for this salesman and product
//                string checkQuery = @"
//                    SELECT COUNT(*) FROM SMCurrentStock 
//                    WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

//                SqlCommand checkCmd = new SqlCommand(checkQuery, con, transaction);
//                checkCmd.Parameters.AddWithValue("@ProductId", productId);
//                checkCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

//                if (exists > 0)
//                {
//                    // Update existing record
//                    string updateQuery = @"
//                        UPDATE SMCurrentStock 
//                        SET Qtylose = Qtylose + @QtyLose,
//                            PerUnitCostLose = CASE 
//                                WHEN Qtylose + @QtyLose > 0 
//                                THEN ((PerUnitCostLose * Qtylose) + (@PerUnitCostLose * @QtyLose)) / (Qtylose + @QtyLose)
//                                ELSE 0 
//                            END
//                        WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

//                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
//                    updateCmd.Parameters.AddWithValue("@ProductId", productId);
//                    updateCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                    updateCmd.Parameters.AddWithValue("@QtyLose", qtyLose);
//                    updateCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
//                    updateCmd.ExecuteNonQuery();
//                }
//                else
//                {
//                    // Insert new record
//                    string insertQuery = @"
//                        INSERT INTO SMCurrentStock (ProductId, SalesmanId, Qtylose, PerUnitCostLose) 
//                        VALUES (@ProductId, @SalesmanId, @QtyLose, @PerUnitCostLose)";

//                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
//                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
//                    insertCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                    insertCmd.Parameters.AddWithValue("@QtyLose", qtyLose);
//                    insertCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
//                    insertCmd.ExecuteNonQuery();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error updating SMCurrentStock: {ex.Message}");
//            }
//        }

//        private void UpdateSMOpeningStock(SqlConnection con, SqlTransaction transaction, string salesmanId, string productId,
//                                          int packingId, int multiplier, int qtyPack, int qtyLose, decimal perUnitCostLose)
//        {
//            try
//            {
//                // Check if record exists for this salesman and product
//                string checkQuery = @"
//                    SELECT COUNT(*) FROM SMOpeningStock 
//                    WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

//                SqlCommand checkCmd = new SqlCommand(checkQuery, con, transaction);
//                checkCmd.Parameters.AddWithValue("@ProductId", productId);
//                checkCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

//                // Calculate QtyLose for SMOpeningStock: (pack * qtyPack) + qtyLose
//                int totalQtyLose = (multiplier * qtyPack) + qtyLose;

//                if (exists > 0)
//                {
//                    // Update existing record
//                    string updateQuery = @"
//                        UPDATE SMOpeningStock 
//                        SET PackingId = @PackingId,
//                            Multiplier = @Multiplier,
//                            QtyPack = ISNULL(QtyPack, 0) + @QtyPack,
//                            QtyLose = ISNULL(QtyLose, 0) + @TotalQtyLose,
//                            PerUnitCostLose = CASE 
//                                WHEN ISNULL(QtyLose, 0) + @TotalQtyLose > 0 
//                                THEN ((ISNULL(PerUnitCostLose, 0) * ISNULL(QtyLose, 0)) + (@PerUnitCostLose * @TotalQtyLose)) / (ISNULL(QtyLose, 0) + @TotalQtyLose)
//                                ELSE 0 
//                            END,
//                            IsDeleted = 0
//                        WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

//                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
//                    updateCmd.Parameters.AddWithValue("@ProductId", productId);
//                    updateCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                    updateCmd.Parameters.AddWithValue("@PackingId", packingId);
//                    updateCmd.Parameters.AddWithValue("@Multiplier", multiplier);
//                    updateCmd.Parameters.AddWithValue("@QtyPack", qtyPack);
//                    updateCmd.Parameters.AddWithValue("@TotalQtyLose", totalQtyLose);
//                    updateCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
//                    updateCmd.ExecuteNonQuery();
//                }
//                else
//                {
//                    // Insert new record
//                    string insertQuery = @"
//                        INSERT INTO SMOpeningStock (ProductId, SalesmanId, PackingId, Multiplier, QtyPack, QtyLose, PerUnitCostLose, IsDeleted) 
//                        VALUES (@ProductId, @SalesmanId, @PackingId, @Multiplier, @QtyPack, @TotalQtyLose, @PerUnitCostLose, 0)";

//                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
//                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
//                    insertCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
//                    insertCmd.Parameters.AddWithValue("@PackingId", packingId);
//                    insertCmd.Parameters.AddWithValue("@Multiplier", multiplier);
//                    insertCmd.Parameters.AddWithValue("@QtyPack", qtyPack);
//                    insertCmd.Parameters.AddWithValue("@TotalQtyLose", totalQtyLose);
//                    insertCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
//                    insertCmd.ExecuteNonQuery();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error updating SMOpeningStock: {ex.Message}");
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//        }

//        private void ClearForm()
//        {
//            // Clear all fields except issue ID
//            salesmenidtxtbox.Clear();
//            salesmennametxtbox.Clear();
//            ClearProductFields();
//            dataGridView1.Rows.Clear();
//            productItems.Clear();
//            issuedatepcker.Value = DateTime.Today;
//            salesmenidtxtbox.Focus();
//        }

//        private void openbtn_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Create custom input dialog
//                Form inputForm = new Form();
//                inputForm.Text = "Open Stock Issue";
//                inputForm.Width = 300;
//                inputForm.Height = 150;
//                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
//                inputForm.MaximizeBox = false;
//                inputForm.MinimizeBox = false;
//                inputForm.StartPosition = FormStartPosition.CenterParent;

//                Label label = new Label();
//                label.Text = "Enter Issue ID to open:";
//                label.SetBounds(10, 20, 280, 20);

//                TextBox textBox = new TextBox();
//                textBox.SetBounds(10, 45, 260, 20);

//                Button okButton = new Button();
//                okButton.Text = "OK";
//                okButton.DialogResult = DialogResult.OK;
//                okButton.SetBounds(60, 80, 75, 25);

//                Button cancelButton = new Button();
//                cancelButton.Text = "Cancel";
//                cancelButton.DialogResult = DialogResult.Cancel;
//                cancelButton.SetBounds(140, 80, 75, 25);

//                inputForm.Controls.Add(label);
//                inputForm.Controls.Add(textBox);
//                inputForm.Controls.Add(okButton);
//                inputForm.Controls.Add(cancelButton);
//                inputForm.AcceptButton = okButton;
//                inputForm.CancelButton = cancelButton;

//                if (inputForm.ShowDialog() == DialogResult.OK)
//                {
//                    string input = textBox.Text;

//                    if (!string.IsNullOrEmpty(input))
//                    {
//                        if (int.TryParse(input, out int issueId))
//                        {
//                            LoadStockIssue(issueId);
//                        }
//                        else
//                        {
//                            MessageBox.Show("Please enter a valid numeric Issue ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error opening stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadStockIssue(int issueId)
//        {
//            try
//            {
//                using (SqlConnection con = new SqlConnection(connectionString))
//                {
//                    con.Open();

//                    // Load header
//                    string headerQuery = @"
//                        SELECT si.IssueID, si.IssueDate, si.SalesmanID, sm.SalesmanName 
//                        FROM StockIssue si 
//                        LEFT JOIN Salesmen sm ON si.SalesmanID = sm.SalesmanID 
//                        WHERE si.IssueID = @IssueID";

//                    SqlCommand cmdHeader = new SqlCommand(headerQuery, con);
//                    cmdHeader.Parameters.AddWithValue("@IssueID", issueId);

//                    using (SqlDataReader reader = cmdHeader.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            issueidtxtbox.Text = reader["IssueID"].ToString();
//                            issuedatepcker.Value = Convert.ToDateTime(reader["IssueDate"]);
//                            salesmenidtxtbox.Text = reader["SalesmanID"].ToString();
//                            salesmennametxtbox.Text = reader["SalesmanName"].ToString();
//                        }
//                        else
//                        {
//                            MessageBox.Show("Stock issue not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                            return;
//                        }
//                    }

//                    // Load body
//                    dataGridView1.Rows.Clear();
//                    productItems.Clear();
//                    string bodyQuery = @"
//                        SELECT sib.ProductID, p.ProductName, pk.PackingName, sib.Multiplier, 
//                               sib.QtyPack, sib.QtyLose, sib.PerUnitCostLose, sib.TTLValue,
//                               sib.PackingId
//                        FROM StockIssueBody sib 
//                        LEFT JOIN Products p ON sib.ProductID = p.ProductID 
//                        LEFT JOIN Packings pk ON sib.PackingId = pk.PackingId 
//                        WHERE sib.IssueID = @IssueID";

//                    SqlCommand cmdBody = new SqlCommand(bodyQuery, con);
//                    cmdBody.Parameters.AddWithValue("@IssueID", issueId);

//                    using (SqlDataReader reader = cmdBody.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            int rowIndex = dataGridView1.Rows.Add();
//                            DataGridViewRow row = dataGridView1.Rows[rowIndex];

//                            row.Cells["colProductID"].Value = reader["ProductID"].ToString();
//                            row.Cells["colProductName"].Value = reader["ProductName"].ToString();
//                            row.Cells["colPackName"].Value = reader["PackingName"].ToString();
//                            row.Cells["colPack"].Value = Convert.ToInt32(reader["Multiplier"]);
//                            row.Cells["colQtyPack"].Value = Convert.ToInt32(reader["QtyPack"]);
//                            row.Cells["colQtyLoose"].Value = Convert.ToInt32(reader["QtyLose"]);

//                            // Store in productItems list
//                            ProductItem item = new ProductItem
//                            {
//                                ProductID = reader["ProductID"].ToString(),
//                                ProductName = reader["ProductName"].ToString(),
//                                PackName = reader["PackingName"].ToString(),
//                                Pack = Convert.ToInt32(reader["Multiplier"]),
//                                QtyPack = Convert.ToInt32(reader["QtyPack"]),
//                                QtyLose = Convert.ToInt32(reader["QtyLose"]),
//                                PerUnitCostLose = Convert.ToDecimal(reader["PerUnitCostLose"]),
//                                TTLValue = Convert.ToDecimal(reader["TTLValue"])
//                            };
//                            productItems.Add(item);
//                        }
//                    }

//                    MessageBox.Show("Stock issue loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Other event handlers
//        private void issueidtxtbox_TextChanged(object sender, EventArgs e) { }
//        private void salesmenidtxtbox_TextChanged(object sender, EventArgs e) { }
//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
//        private void packtxtbox_TextChanged(object sender, EventArgs e) { }
//        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }
//        private void qtyltxtbox_TextChanged(object sender, EventArgs e) { }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mainform1 = new Mainform1();
//            mainform1.Show();
//        }
//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mainform1 = new Mainform1();
//            mainform1.Show();
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FO_2022
{
    public partial class Stock_Issue_to_Salesmen : Form
    {
        // Connection string from App.config
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Variables to store data
        private List<ProductItem> productItems = new List<ProductItem>();
        private DataTable dtPackings = new DataTable();
        private int nextIssueId = 1;
        private bool isDataLoaded = false;

        // Store current product cost
        private decimal currentProductCost = 0;

        // Class to hold product data
        public class ProductItem
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string PackName { get; set; }
            public int Pack { get; set; }
            public int QtyPack { get; set; }
            public int QtyLose { get; set; }
            public decimal PerUnitCostLose { get; set; }
            public decimal TTLValue { get; set; }
        }

        public Stock_Issue_to_Salesmen()
        {
            InitializeComponent();
        }

        private void Stock_Issue_to_Salesmen_Load(object sender, EventArgs e)
        {
            try
            {
                // Add columns to grid programmatically
                AddColumnsToGrid();

                // Load data when form loads
                LoadNextIssueId();
                LoadPackings();
                isDataLoaded = true;

                // Set current date
                issuedatepcker.Value = DateTime.Today;

                // Set focus to salesman ID
                salesmenidtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddColumnsToGrid()
        {
            // Clear any existing columns
            dataGridView1.Columns.Clear();

            // Add columns programmatically
            // Column 1: Pro ID
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "Pro ID";
            col1.Name = "colProductID";
            col1.Width = 85;
            dataGridView1.Columns.Add(col1);

            // Column 2: Product Name
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "Product Name";
            col2.Name = "colProductName";
            col2.Width = 115;
            dataGridView1.Columns.Add(col2);

            // Column 3: Pack Name (TextBox, not ComboBox)
            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "Pack Name";
            col3.Name = "colPackName";
            col3.Width = 90;
            dataGridView1.Columns.Add(col3);

            // Column 4: Pack 
            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "Pack";
            col4.Name = "colPack";
            col4.Width = 50;
            dataGridView1.Columns.Add(col4);

            // Column 5: Qty Pack
            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
            col5.HeaderText = "Qty Pack";
            col5.Name = "colQtyPack";
            col5.Width = 90;
            dataGridView1.Columns.Add(col5);

            // Column 6: Qty Loose
            DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
            col6.HeaderText = "Qty Loose";
            col6.Name = "colQtyLoose";
            col6.Width = 95;
            dataGridView1.Columns.Add(col6);
        }

        private void LoadNextIssueId()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT ISNULL(MAX(IssueID), 0) + 1 AS NextIssueID FROM StockIssue";
                    SqlCommand cmd = new SqlCommand(query, con);
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        nextIssueId = Convert.ToInt32(result);
                        issueidtxtbox.Text = nextIssueId.ToString();
                    }
                    else
                    {
                        issueidtxtbox.Text = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading next Issue ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                issueidtxtbox.Text = "1";
            }
        }

        private void LoadPackings()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    dtPackings.Clear();
                    da.Fill(dtPackings);

                    // Populate the combobox above the grid (not in the grid)
                    packnamecombobox.DataSource = dtPackings;
                    packnamecombobox.DisplayMember = "PackingName";
                    packnamecombobox.ValueMember = "PackingId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductCost(string productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get product purchase price from Products table
                    string query = @"
                        SELECT PurchasePrice, PurDiscRatio, PurDiscValue, PurSTRatio 
                        FROM Products 
                        WHERE ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal purchasePrice = Convert.ToDecimal(reader["PurchasePrice"]);
                            decimal purDiscRatio = Convert.ToDecimal(reader["PurDiscRatio"]);
                            decimal purDiscValue = Convert.ToDecimal(reader["PurDiscValue"]);
                            decimal purSTRatio = Convert.ToDecimal(reader["PurSTRatio"]);

                            // Calculate actual cost per unit (pack)
                            // Formula: Purchase Price - Discount + Sales Tax
                            decimal discountAmount = purchasePrice * (purDiscRatio / 100);
                            discountAmount = Math.Max(discountAmount, purDiscValue);
                            decimal priceAfterDiscount = purchasePrice - discountAmount;
                            decimal salesTaxAmount = priceAfterDiscount * (purSTRatio / 100);

                            currentProductCost = priceAfterDiscount + salesTaxAmount;
                        }
                        else
                        {
                            currentProductCost = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product cost: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                currentProductCost = 0;
            }
        }

        private void salesmenidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(salesmenidtxtbox.Text.Trim()))
                {
                    LoadSalesmanDetails();
                }
            }
        }

        private void LoadSalesmanDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        salesmennametxtbox.Text = result.ToString();
                        productidtxtbox.Focus(); // Move focus to product ID
                    }
                    else
                    {
                        salesmennametxtbox.Text = "";
                        MessageBox.Show("Salesman ID not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        salesmenidtxtbox.Focus();
                        salesmenidtxtbox.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading salesman details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(productidtxtbox.Text.Trim()))
                {
                    LoadProductDetails();
                }
            }
        }

        private void LoadProductDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Get product details
                    string query = @"
                        SELECT p.ProductID, p.ProductName, pp.PackingId, pk.PackingName, pp.Multiplier 
                        FROM Products p 
                        LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId 
                        LEFT JOIN Packings pk ON pp.PackingId = pk.PackingId 
                        WHERE p.ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productnametxtbox.Text = reader["ProductName"].ToString();

                            // Load product cost
                            LoadProductCost(productidtxtbox.Text.Trim());

                            // Set packing combobox
                            if (reader["PackingId"] != DBNull.Value)
                            {
                                int packingId = Convert.ToInt32(reader["PackingId"]);
                                packnamecombobox.SelectedValue = packingId;
                                packtxtbox.Text = reader["Multiplier"].ToString();
                            }
                            else
                            {
                                packnamecombobox.SelectedIndex = -1;
                                packtxtbox.Text = "1"; // Default multiplier
                            }

                            qtyptxtbox.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Product ID not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearProductFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isDataLoaded && packnamecombobox.SelectedValue != null && !string.IsNullOrEmpty(productidtxtbox.Text))
            {
                LoadMultiplierForSelectedPacking();
            }
        }

        private void LoadMultiplierForSelectedPacking()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Multiplier FROM ProductPacking WHERE ProductId = @ProductID AND PackingId = @PackingId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@PackingId", packnamecombobox.SelectedValue);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        packtxtbox.Text = result.ToString();
                    }
                    else
                    {
                        packtxtbox.Text = "1"; // Default multiplier
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading multiplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(qtyptxtbox.Text))
                {
                    // Calculate loose quantity
                    CalculateLooseQuantity();
                    qtyltxtbox.Focus();
                }
            }
        }

        private void CalculateLooseQuantity()
        {
            try
            {
                if (int.TryParse(qtyptxtbox.Text, out int qtyPack) && int.TryParse(packtxtbox.Text, out int multiplier))
                {
                    int qtyLose = qtyPack * multiplier;
                    qtyltxtbox.Text = qtyLose.ToString();
                }
                else
                {
                    qtyltxtbox.Text = "0";
                }
            }
            catch
            {
                qtyltxtbox.Text = "0";
            }
        }

        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrEmpty(qtyltxtbox.Text))
                {
                    AddProductToGrid();
                }
            }
        }

        private void AddProductToGrid()
        {
            // Validate required fields
            if (string.IsNullOrEmpty(productidtxtbox.Text) || string.IsNullOrEmpty(productnametxtbox.Text))
            {
                MessageBox.Show("Please select a product first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productidtxtbox.Focus();
                return;
            }

            if (packnamecombobox.SelectedItem == null)
            {
                MessageBox.Show("Please select a packing!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packnamecombobox.Focus();
                return;
            }

            if (!int.TryParse(qtyptxtbox.Text, out int qtyPack) || qtyPack <= 0)
            {
                MessageBox.Show("Please enter valid quantity pack!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyptxtbox.Focus();
                qtyptxtbox.SelectAll();
                return;
            }

            if (!int.TryParse(qtyltxtbox.Text, out int qtyLose) || qtyLose <= 0)
            {
                MessageBox.Show("Please enter valid quantity loose!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyltxtbox.Focus();
                qtyltxtbox.SelectAll();
                return;
            }

            if (!int.TryParse(packtxtbox.Text, out int pack))
            {
                pack = 1;
            }

            // Check if product already exists in grid
            bool productExists = false;
            foreach (DataGridViewRow gridRow in dataGridView1.Rows)
            {
                if (!gridRow.IsNewRow && gridRow.Cells["colProductID"].Value != null)
                {
                    if (gridRow.Cells["colProductID"].Value.ToString() == productidtxtbox.Text.Trim())
                    {
                        productExists = true;
                        break;
                    }
                }
            }

            if (productExists)
            {
                MessageBox.Show("Product already exists in the list! Remove it first to add again.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearProductFields();
                productidtxtbox.Focus();
                return;
            }

            // Calculate PerUnitCostLose and TTLValue
            decimal perUnitCostLose = 0;
            decimal ttlValue = 0;

            if (currentProductCost > 0 && pack > 0)
            {
                // PerUnitCostLose = Cost per pack ÷ Multiplier
                perUnitCostLose = currentProductCost / pack;

                // TTLValue = PerUnitCostLose × QtyLose
                ttlValue = perUnitCostLose * qtyLose;
            }

            // Add to grid
            int rowIndex = dataGridView1.Rows.Add();
            DataGridViewRow newRow = dataGridView1.Rows[rowIndex];

            newRow.Cells["colProductID"].Value = productidtxtbox.Text.Trim();
            newRow.Cells["colProductName"].Value = productnametxtbox.Text.Trim();
            newRow.Cells["colPackName"].Value = packnamecombobox.Text;
            newRow.Cells["colPack"].Value = pack;
            newRow.Cells["colQtyPack"].Value = qtyPack;
            newRow.Cells["colQtyLoose"].Value = qtyLose;

            // Store calculated values in ProductItem list
            ProductItem item = new ProductItem
            {
                ProductID = productidtxtbox.Text.Trim(),
                ProductName = productnametxtbox.Text.Trim(),
                PackName = packnamecombobox.Text,
                Pack = pack,
                QtyPack = qtyPack,
                QtyLose = qtyLose,
                PerUnitCostLose = perUnitCostLose,
                TTLValue = ttlValue
            };
            productItems.Add(item);

            // Clear product fields for next entry
            ClearProductFields();
        }

        private void ClearProductFields()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnamecombobox.SelectedIndex = -1;
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            currentProductCost = 0;
            productidtxtbox.Focus();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // You can add functionality here if needed
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        // Remove from productItems list
                        string productId = row.Cells["colProductID"].Value?.ToString();
                        if (!string.IsNullOrEmpty(productId))
                        {
                            ProductItem itemToRemove = productItems.FirstOrDefault(p => p.ProductID == productId);
                            if (itemToRemove != null)
                            {
                                productItems.Remove(itemToRemove);
                            }
                        }

                        // Remove from grid
                        dataGridView1.Rows.Remove(row);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(salesmenidtxtbox.Text.Trim()))
                {
                    MessageBox.Show("Please enter Salesman ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    salesmenidtxtbox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(salesmennametxtbox.Text.Trim()))
                {
                    MessageBox.Show("Salesman not found! Please enter a valid Salesman ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    salesmenidtxtbox.Focus();
                    return;
                }

                if (dataGridView1.Rows.Count <= 1) // Only header row exists
                {
                    MessageBox.Show("Please add at least one product!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    productidtxtbox.Focus();
                    return;
                }

                // Begin transaction
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        int issueId = Convert.ToInt32(issueidtxtbox.Text);
                        string salesmanId = salesmenidtxtbox.Text.Trim();

                        // Save to StockIssue table
                        string insertHeader = @"
                            INSERT INTO StockIssue (IssueID, IssueDate, SalesmanID, IsUploaded) 
                            VALUES (@IssueID, @IssueDate, @SalesmanID, 0)";

                        SqlCommand cmdHeader = new SqlCommand(insertHeader, con, transaction);
                        cmdHeader.Parameters.AddWithValue("@IssueID", issueId);
                        cmdHeader.Parameters.AddWithValue("@IssueDate", issuedatepcker.Value);
                        cmdHeader.Parameters.AddWithValue("@SalesmanID", salesmanId);
                        cmdHeader.ExecuteNonQuery();

                        // Save to StockIssueBody table with calculated values
                        string insertBody = @"
                            INSERT INTO StockIssueBody (IssueID, ProductID, PackingId, Multiplier, QtyPack, QtyLose, PerUnitCostLose, TTLValue) 
                            VALUES (@IssueID, @ProductID, @PackingId, @Multiplier, @QtyPack, @QtyLose, @PerUnitCostLose, @TTLValue)";

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow && row.Cells["colProductID"].Value != null)
                            {
                                string productId = row.Cells["colProductID"].Value.ToString();

                                // Find product in the list to get calculated values
                                ProductItem productItem = productItems.FirstOrDefault(p => p.ProductID == productId);

                                // Get PackingId from PackingName
                                int packingId = 0;
                                string packingName = row.Cells["colPackName"].Value?.ToString();
                                if (!string.IsNullOrEmpty(packingName))
                                {
                                    DataRow[] rows = dtPackings.Select($"PackingName = '{packingName}'");
                                    if (rows.Length > 0)
                                    {
                                        packingId = Convert.ToInt32(rows[0]["PackingId"]);
                                    }
                                }

                                int multiplier = Convert.ToInt32(row.Cells["colPack"].Value);
                                int qtyPack = Convert.ToInt32(row.Cells["colQtyPack"].Value);
                                int qtyLose = Convert.ToInt32(row.Cells["colQtyLoose"].Value);

                                SqlCommand cmdBody = new SqlCommand(insertBody, con, transaction);
                                cmdBody.Parameters.AddWithValue("@IssueID", issueId);
                                cmdBody.Parameters.AddWithValue("@ProductID", productId);
                                cmdBody.Parameters.AddWithValue("@PackingId", packingId);
                                cmdBody.Parameters.AddWithValue("@Multiplier", multiplier);
                                cmdBody.Parameters.AddWithValue("@QtyPack", qtyPack);
                                cmdBody.Parameters.AddWithValue("@QtyLose", qtyLose);

                                // Use calculated values from productItems
                                if (productItem != null)
                                {
                                    cmdBody.Parameters.AddWithValue("@PerUnitCostLose", productItem.PerUnitCostLose);
                                    cmdBody.Parameters.AddWithValue("@TTLValue", productItem.TTLValue);
                                }
                                else
                                {
                                    // Fallback calculation if item not found in list
                                    // Get product cost
                                    LoadProductCost(productId);
                                    decimal perUnitCostLose = currentProductCost / (multiplier > 0 ? multiplier : 1);
                                    decimal ttlValue = perUnitCostLose * qtyLose;

                                    cmdBody.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
                                    cmdBody.Parameters.AddWithValue("@TTLValue", ttlValue);
                                }

                                cmdBody.ExecuteNonQuery();

                                // Calculate total QtyLose for SMCurrentStock: (multiplier * qtyPack) + qtyLose
                                int totalQtyLoseForSMCurrent = (multiplier * qtyPack) + qtyLose;

                                // Update SMCurrentStock table with new calculation
                                UpdateSMCurrentStock(con, transaction, salesmanId, productId, totalQtyLoseForSMCurrent, productItem?.PerUnitCostLose ?? 0);

                                // Update SMOpeningStock table with only qtyLose (user entered value)
                                UpdateSMOpeningStock(con, transaction, salesmanId, productId, packingId, multiplier, qtyPack, qtyLose, productItem?.PerUnitCostLose ?? 0);
                            }
                        }

                        // Commit transaction
                        transaction.Commit();

                        MessageBox.Show("Stock issue saved successfully!\nData inserted into:\n1. StockIssue\n2. StockIssueBody\n3. SMCurrentStock (with calculated QtyLose)\n4. SMOpeningStock (with user entered QtyLose)",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh form for new entry
                        ClearForm();
                        LoadNextIssueId();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSMCurrentStock(SqlConnection con, SqlTransaction transaction, string salesmanId, string productId, int totalQtyLose, decimal perUnitCostLose)
        {
            try
            {
                // Check if record exists for this salesman and product
                string checkQuery = @"
                    SELECT COUNT(*) FROM SMCurrentStock 
                    WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con, transaction);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);
                checkCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists > 0)
                {
                    // Update existing record - Add totalQtyLose (multiplier * qtyPack + qtyLose)
                    string updateQuery = @"
                        UPDATE SMCurrentStock 
                        SET Qtylose = Qtylose + @TotalQtyLose,
                            PerUnitCostLose = CASE 
                                WHEN Qtylose + @TotalQtyLose > 0 
                                THEN ((PerUnitCostLose * Qtylose) + (@PerUnitCostLose * @TotalQtyLose)) / (Qtylose + @TotalQtyLose)
                                ELSE 0 
                            END
                        WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
                    updateCmd.Parameters.AddWithValue("@ProductId", productId);
                    updateCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                    updateCmd.Parameters.AddWithValue("@TotalQtyLose", totalQtyLose);
                    updateCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Insert new record
                    string insertQuery = @"
                        INSERT INTO SMCurrentStock (ProductId, SalesmanId, Qtylose, PerUnitCostLose) 
                        VALUES (@ProductId, @SalesmanId, @TotalQtyLose, @PerUnitCostLose)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                    insertCmd.Parameters.AddWithValue("@TotalQtyLose", totalQtyLose);
                    insertCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating SMCurrentStock: {ex.Message}");
            }
        }

        private void UpdateSMOpeningStock(SqlConnection con, SqlTransaction transaction, string salesmanId, string productId,
                                          int packingId, int multiplier, int qtyPack, int qtyLose, decimal perUnitCostLose)
        {
            try
            {
                // Check if record exists for this salesman and product
                string checkQuery = @"
                    SELECT COUNT(*) FROM SMOpeningStock 
                    WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

                SqlCommand checkCmd = new SqlCommand(checkQuery, con, transaction);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);
                checkCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                // For SMOpeningStock: Use only qtyLose (user entered value)
                int qtyLoseForSMOpening = qtyLose;

                if (exists > 0)
                {
                    // Update existing record
                    string updateQuery = @"
                        UPDATE SMOpeningStock 
                        SET PackingId = @PackingId,
                            Multiplier = @Multiplier,
                            QtyPack = ISNULL(QtyPack, 0) + @QtyPack,
                            QtyLose = ISNULL(QtyLose, 0) + @QtyLose,
                            PerUnitCostLose = CASE 
                                WHEN ISNULL(QtyLose, 0) + @QtyLose > 0 
                                THEN ((ISNULL(PerUnitCostLose, 0) * ISNULL(QtyLose, 0)) + (@PerUnitCostLose * @QtyLose)) / (ISNULL(QtyLose, 0) + @QtyLose)
                                ELSE 0 
                            END,
                            IsDeleted = 0
                        WHERE ProductId = @ProductId AND SalesmanId = @SalesmanId";

                    SqlCommand updateCmd = new SqlCommand(updateQuery, con, transaction);
                    updateCmd.Parameters.AddWithValue("@ProductId", productId);
                    updateCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                    updateCmd.Parameters.AddWithValue("@PackingId", packingId);
                    updateCmd.Parameters.AddWithValue("@Multiplier", multiplier);
                    updateCmd.Parameters.AddWithValue("@QtyPack", qtyPack);
                    updateCmd.Parameters.AddWithValue("@QtyLose", qtyLoseForSMOpening);
                    updateCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Insert new record
                    string insertQuery = @"
                        INSERT INTO SMOpeningStock (ProductId, SalesmanId, PackingId, Multiplier, QtyPack, QtyLose, PerUnitCostLose, IsDeleted) 
                        VALUES (@ProductId, @SalesmanId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @PerUnitCostLose, 0)";

                    SqlCommand insertCmd = new SqlCommand(insertQuery, con, transaction);
                    insertCmd.Parameters.AddWithValue("@ProductId", productId);
                    insertCmd.Parameters.AddWithValue("@SalesmanId", salesmanId);
                    insertCmd.Parameters.AddWithValue("@PackingId", packingId);
                    insertCmd.Parameters.AddWithValue("@Multiplier", multiplier);
                    insertCmd.Parameters.AddWithValue("@QtyPack", qtyPack);
                    insertCmd.Parameters.AddWithValue("@QtyLose", qtyLoseForSMOpening);
                    insertCmd.Parameters.AddWithValue("@PerUnitCostLose", perUnitCostLose);
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating SMOpeningStock: {ex.Message}");
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all fields except issue ID
            salesmenidtxtbox.Clear();
            salesmennametxtbox.Clear();
            ClearProductFields();
            dataGridView1.Rows.Clear();
            productItems.Clear();
            issuedatepcker.Value = DateTime.Today;
            salesmenidtxtbox.Focus();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Stock Issue";
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
                            LoadStockIssue(issueId);
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
                MessageBox.Show("Error opening stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStockIssue(int issueId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Load header
                    string headerQuery = @"
                        SELECT si.IssueID, si.IssueDate, si.SalesmanID, sm.SalesmanName 
                        FROM StockIssue si 
                        LEFT JOIN Salesmen sm ON si.SalesmanID = sm.SalesmanID 
                        WHERE si.IssueID = @IssueID";

                    SqlCommand cmdHeader = new SqlCommand(headerQuery, con);
                    cmdHeader.Parameters.AddWithValue("@IssueID", issueId);

                    using (SqlDataReader reader = cmdHeader.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            issueidtxtbox.Text = reader["IssueID"].ToString();
                            issuedatepcker.Value = Convert.ToDateTime(reader["IssueDate"]);
                            salesmenidtxtbox.Text = reader["SalesmanID"].ToString();
                            salesmennametxtbox.Text = reader["SalesmanName"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Stock issue not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Load body
                    dataGridView1.Rows.Clear();
                    productItems.Clear();
                    string bodyQuery = @"
                        SELECT sib.ProductID, p.ProductName, pk.PackingName, sib.Multiplier, 
                               sib.QtyPack, sib.QtyLose, sib.PerUnitCostLose, sib.TTLValue,
                               sib.PackingId
                        FROM StockIssueBody sib 
                        LEFT JOIN Products p ON sib.ProductID = p.ProductID 
                        LEFT JOIN Packings pk ON sib.PackingId = pk.PackingId 
                        WHERE sib.IssueID = @IssueID";

                    SqlCommand cmdBody = new SqlCommand(bodyQuery, con);
                    cmdBody.Parameters.AddWithValue("@IssueID", issueId);

                    using (SqlDataReader reader = cmdBody.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowIndex];

                            row.Cells["colProductID"].Value = reader["ProductID"].ToString();
                            row.Cells["colProductName"].Value = reader["ProductName"].ToString();
                            row.Cells["colPackName"].Value = reader["PackingName"].ToString();
                            row.Cells["colPack"].Value = Convert.ToInt32(reader["Multiplier"]);
                            row.Cells["colQtyPack"].Value = Convert.ToInt32(reader["QtyPack"]);
                            row.Cells["colQtyLoose"].Value = Convert.ToInt32(reader["QtyLose"]);

                            // Store in productItems list
                            ProductItem item = new ProductItem
                            {
                                ProductID = reader["ProductID"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                PackName = reader["PackingName"].ToString(),
                                Pack = Convert.ToInt32(reader["Multiplier"]),
                                QtyPack = Convert.ToInt32(reader["QtyPack"]),
                                QtyLose = Convert.ToInt32(reader["QtyLose"]),
                                PerUnitCostLose = Convert.ToDecimal(reader["PerUnitCostLose"]),
                                TTLValue = Convert.ToDecimal(reader["TTLValue"])
                            };
                            productItems.Add(item);
                        }
                    }

                    MessageBox.Show("Stock issue loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading stock issue: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Other event handlers
        private void issueidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void salesmenidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void packtxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyltxtbox_TextChanged(object sender, EventArgs e) { }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mainform1 = new Mainform1();
            //mainform1.Show();
        }
    }
}