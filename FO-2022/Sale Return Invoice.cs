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
    public partial class Sale_Return_Invoice : Form
    {
        private SqlConnection connection;
        private int nextReturnId = 1;
        private DataTable gridDataTable;
        private bool isGeneralReturn = false; // Flag to track return type

        public Sale_Return_Invoice()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeForm();
        }

        private void InitializeConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeForm()
        {
            LoadNextReturnId();
            saleidtxtbox.Enabled = false;
            InitializeDataGridView();
            LoadPackings();

            // Start with general return mode by default
            SetReturnTypeUI(true);
            dataGridView1.SelectionChanged += DataGridView1_SelectionChanged;
            // Attach quantity validation events
            qtyptxtbox.TextChanged += qtyptxtbox_TextChanged;
            qtyltxtbox.TextChanged += qtyltxtbox_TextChanged;
            bnsltxtbox.TextChanged += bnsltxtbox_TextChanged;
        }
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            // Get the first selected row
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

            // Extract data from the row
            productidtxtbox.Text = selectedRow.Cells["ProductID"].Value?.ToString() ?? "";
            productnametxtbox.Text = selectedRow.Cells["ProductName"].Value?.ToString() ?? "";

            // Handle Packing combo box
            string packingName = selectedRow.Cells["PackingId"].Value?.ToString() ?? "";
            packnamecomboBox.Text = packingName;

            // Set numeric values
            multipliertxtbox.Text = selectedRow.Cells["Multiplier"].Value?.ToString() ?? "0";
            qtyptxtbox.Text = selectedRow.Cells["QtyPack"].Value?.ToString() ?? "0";
            qtyltxtbox.Text = selectedRow.Cells["QtyLose"].Value?.ToString() ?? "0";
            bnsltxtbox.Text = selectedRow.Cells["BnsLose"].Value?.ToString() ?? "0";
            pricetxtbox.Text = selectedRow.Cells["PricePack"].Value?.ToString() ?? "0";
            discpercenttxtbox.Text = selectedRow.Cells["DiscRatio"].Value?.ToString() ?? "0";
            discvalltxtbox.Text = selectedRow.Cells["DiscValLose"].Value?.ToString() ?? "0";
            discvaltxtbox.Text = selectedRow.Cells["DiscValPack"].Value?.ToString() ?? "0";
            salestaxtxtbox.Text = selectedRow.Cells["STaxRatio"].Value?.ToString() ?? "0";
            ttlvaluetxtbox.Text = selectedRow.Cells["TTLValue"].Value?.ToString() ?? "0";

            // Recalculate values (optional)
            CalculateValues();
        }

        // Automatically determine return type based on Sale ID presence
        private void CheckAndSetReturnType()
        {
            if (string.IsNullOrWhiteSpace(orderidtxtbox.Text))
            {
                // No Sale ID entered - General Return
                SetReturnTypeUI(true);
            }
            else
            {
                // Sale ID entered - Sale-linked Return
                SetReturnTypeUI(false);
            }
        }

        // Set UI based on return type
        private void SetReturnTypeUI(bool isGeneral)
        {
            isGeneralReturn = isGeneral;

            if (isGeneral)
            {
                // General Return mode
                orderidlbl.Text = "Sale ID (Optional)";
                orderidlbl.ForeColor = Color.Gray;

                // Enable customer entry
                customeridtxtbox.BackColor = Color.White;
                customeridtxtbox.Enabled = true;
                customeridtxtbox.ReadOnly = false;

                // Auto-focus customer ID if empty
                if (string.IsNullOrWhiteSpace(customeridtxtbox.Text))
                {
                    customeridtxtbox.Focus();
                }
            }
            else
            {
                // Sale-linked Return mode
                orderidlbl.Text = "Sale ID";
                orderidlbl.ForeColor = Color.Black;

                // Customer will be auto-filled from sale
                customeridtxtbox.BackColor = Color.LightGray;
                customeridtxtbox.Enabled = false;
            }
        }
        private void EnsureConnectionOpen()
        {
            if (connection == null)
            {
                InitializeConnection();
            }
            else if (connection.State != ConnectionState.Open)
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open connection: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
        }

        private void LoadNextReturnId()
        {
            try
            {
                EnsureConnectionOpen(); // Add this line

                string query = "SELECT ISNULL(MAX(ReturnId), 0) + 1 FROM SaleReturns";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    nextReturnId = Convert.ToInt32(cmd.ExecuteScalar());
                    saleidtxtbox.Text = nextReturnId.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading next Return ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridView()
        {
            gridDataTable = new DataTable();
            gridDataTable.Columns.Add("ProductID", typeof(string));
            gridDataTable.Columns.Add("ProductName", typeof(string));
            gridDataTable.Columns.Add("PackingId", typeof(string));
            gridDataTable.Columns.Add("Multiplier", typeof(int));
            gridDataTable.Columns.Add("QtyPack", typeof(int));
            gridDataTable.Columns.Add("QtyLose", typeof(int));
            gridDataTable.Columns.Add("BnsLose", typeof(int));
            gridDataTable.Columns.Add("PricePack", typeof(decimal));
            gridDataTable.Columns.Add("DiscRatio", typeof(decimal));
            gridDataTable.Columns.Add("DiscValPack", typeof(decimal));
            gridDataTable.Columns.Add("DiscValLose", typeof(decimal));
            gridDataTable.Columns.Add("STaxRatio", typeof(decimal));
            gridDataTable.Columns.Add("TTLValue", typeof(decimal));

            dataGridView1.DataSource = gridDataTable;

            // Format the DataGridView columns
            FormatDataGridViewColumns();
        }

        private void FormatDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Set column widths
                dataGridView1.Columns["ProductID"].Width = 80;
                dataGridView1.Columns["ProductName"].Width = 150;
                dataGridView1.Columns["PackingId"].Width = 80;
                dataGridView1.Columns["Multiplier"].Width = 60;
                dataGridView1.Columns["QtyPack"].Width = 60;
                dataGridView1.Columns["QtyLose"].Width = 60;
                dataGridView1.Columns["BnsLose"].Width = 60;
                dataGridView1.Columns["PricePack"].Width = 70;
                dataGridView1.Columns["DiscRatio"].Width = 60;
                dataGridView1.Columns["DiscValPack"].Width = 70;
                dataGridView1.Columns["DiscValLose"].Width = 70;
                dataGridView1.Columns["STaxRatio"].Width = 60;
                dataGridView1.Columns["TTLValue"].Width = 80;

                // Format numeric columns
                dataGridView1.Columns["PricePack"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["DiscRatio"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["DiscValPack"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["DiscValLose"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["STaxRatio"].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns["TTLValue"].DefaultCellStyle.Format = "N2";

                // Right-align numeric columns
                dataGridView1.Columns["Multiplier"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["QtyPack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["QtyLose"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["BnsLose"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["PricePack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["DiscRatio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["DiscValPack"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["DiscValLose"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["STaxRatio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns["TTLValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void LoadPackings()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    packnamecomboBox.Items.Clear();
                    while (reader.Read())
                    {
                        packnamecomboBox.Items.Add(reader["PackingName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading packings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        // Method to load products from the original sale
        private void orderidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(orderidtxtbox.Text))
            {
                MessageBox.Show("Please enter Sale ID", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string saleId = orderidtxtbox.Text.Trim();

                // First, check if sale exists
                string saleQuery = @"
SELECT
    S.SaleId,
    S.SaleDate,
    S.BillNo,
    S.CustomerId,
    C.PartyName AS CustomerName,
    SE.SectorName AS Sector,
    T.TownName AS Town,
    S.SalesmanId,
    SM.PartyName AS SalesmanName
FROM Sales S
LEFT JOIN Parties C ON S.CustomerId = C.PartyID
LEFT JOIN Sectors SE ON S.SectorID = SE.SectorID
LEFT JOIN Towns T ON SE.TownID = T.TownID
LEFT JOIN Parties SM ON S.SalesmanId = SM.PartyID
WHERE S.SaleId = @SaleId;";

                using (SqlCommand cmd = new SqlCommand(saleQuery, connection))
                {
                    cmd.Parameters.Add("@SaleId", SqlDbType.Int).Value = Convert.ToInt32(saleId);

                    // Use EnsureConnectionOpen instead of checking state manually
                    EnsureConnectionOpen();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Found sale - switch to sale-linked return mode
                            SetReturnTypeUI(false);

                            orderdatetxtbo.Text = Convert.ToDateTime(reader["SaleDate"]).ToString("yyyy-MM-dd");
                            billnootxtbox.Text = reader["BillNo"].ToString();
                            customeridtxtbox.Text = reader["CustomerId"].ToString();
                            customernametxtbox.Text = reader["CustomerName"].ToString();
                            searchtowntxtbox.Text = reader["Town"].ToString() + ", " + reader["Sector"].ToString();
                            salesmenidtxtbox.Text = reader["SalesmanId"].ToString();
                            salesmennametxtbox.Text = reader["SalesmanName"].ToString();

                            //MessageBox.Show("Sale found! Loading product details...", "Success",
                            //    MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Sale not found - stay in general return mode
                            MessageBox.Show("Sale ID not found. Please enter customer details manually for General Return.",
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            SetReturnTypeUI(true);
                            customeridtxtbox.Focus();
                            return;
                        }
                    }
                }

                // Now fetch all product details from SalesBody table
                LoadSaleProducts(Convert.ToInt32(saleId));
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid numeric Sale ID", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetReturnTypeUI(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching sale data:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetReturnTypeUI(true);
            }
            // REMOVE the finally block that closes connection
        }
       
        private void LoadSaleProducts(int saleId)
        {
            try
            {
                // First, count how many products are in the sale
                string countQuery = "SELECT COUNT(*) FROM SalesBody WHERE SaleId = @SaleId";
                int productCount = 0;

                using (SqlCommand cmd = new SqlCommand(countQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    EnsureConnectionOpen(); // Add this
                    productCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (productCount == 0)
                {
                    MessageBox.Show("No products found for this sale. Please add products manually.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Ask user if they want to load the products
                if (!ConfirmLoadProducts(productCount))
                {
                    return;
                }

                // Clear existing items in grid
                gridDataTable.Clear();

                // Query to fetch product details from SalesBody table
                string productQuery = @"
SELECT 
    SB.ProductId,
    P.ProductName,
    PK.PackingName,
    SB.Multiplier,
    SB.QtyPack,
    SB.QtyLose,
    SB.BnsLose,
    SB.PricePack,
    SB.DiscRatio,
    SB.DiscValPack,
    SB.DiscValLose,
    SB.STaxRatio,
    SB.TTLValue
FROM SalesBody SB
INNER JOIN Products P ON SB.ProductId = P.ProductID
LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId
WHERE SB.SaleId = @SaleId
ORDER BY SB.ProductId";

                using (SqlCommand cmd = new SqlCommand(productQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    EnsureConnectionOpen(); // Add this

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int loadedCount = 0;

                        while (reader.Read())
                        {
                            DataRow newRow = gridDataTable.NewRow();

                            // Map all columns from the reader to DataGridView
                            newRow["ProductID"] = reader["ProductId"].ToString().Trim();
                            newRow["ProductName"] = reader["ProductName"].ToString();

                            // Handle PackingId - use PackingName if available
                            if (reader["PackingName"] != DBNull.Value)
                            {
                                newRow["PackingId"] = reader["PackingName"].ToString();
                            }
                            else
                            {
                                newRow["PackingId"] = "Unknown";
                            }

                            newRow["Multiplier"] = reader["Multiplier"];
                            newRow["QtyPack"] = reader["QtyPack"];
                            newRow["QtyLose"] = reader["QtyLose"];
                            newRow["BnsLose"] = reader["BnsLose"];
                            newRow["PricePack"] = reader["PricePack"];
                            newRow["DiscRatio"] = reader["DiscRatio"];
                            newRow["DiscValPack"] = reader["DiscValPack"];
                            newRow["DiscValLose"] = reader["DiscValLose"];
                            newRow["STaxRatio"] = reader["STaxRatio"];
                            newRow["TTLValue"] = reader["TTLValue"];

                            gridDataTable.Rows.Add(newRow);
                            loadedCount++;
                        }

                        if (loadedCount > 0)
                        {
                            // Recalculate totals
                            CalculateTotalValues();

                            MessageBox.Show($"Successfully loaded {loadedCount} product(s) from the original sale.",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Confirm with user before loading products
        private bool ConfirmLoadProducts(int productCount)
        {
            DialogResult result = MessageBox.Show(
                $"Found {productCount} product(s) in the original sale. Do you want to load them into the return?\n\n" +
                "Click Yes to load all products.\n" +
                "Click No to clear and start fresh.",
                "Load Products from Sale",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            return result == DialogResult.Yes;
        }

        // Auto-check return type when Sale ID text changes
        private void orderidtxtbox_TextChanged(object sender, EventArgs e)
        {
            CheckAndSetReturnType();
        }

        private void customeridtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoadCustomerDetails();
            }
        }

        private void LoadCustomerDetails()
        {
            if (string.IsNullOrWhiteSpace(customeridtxtbox.Text))
                return;

            try
            {
                string customerId = customeridtxtbox.Text.Trim();

                string query = @"SELECT p.PartyName, s.SectorName, t.TownName 
                        FROM Parties p 
                        LEFT JOIN Sectors s ON p.SectorID = s.SectorID 
                        LEFT JOIN Towns t ON s.TownID = t.TownID 
                        WHERE p.PartyID = @PartyID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (int.TryParse(customerId, out int intCustomerId))
                    {
                        cmd.Parameters.AddWithValue("@PartyID", intCustomerId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PartyID", customerId);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customernametxtbox.Text = reader["PartyName"].ToString();

                            string sectorName = reader["SectorName"] != DBNull.Value ?
                                              reader["SectorName"].ToString() : "";
                            string townName = reader["TownName"] != DBNull.Value ?
                                            reader["TownName"].ToString() : "";

                            searchtowntxtbox.Text = $"{sectorName}, {townName}".Trim(new char[] { ',', ' ' });
                        }
                        else
                        {
                            MessageBox.Show("Customer not found", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void salesmenidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoadSalesmanDetails();
            }
        }

        private void LoadSalesmanDetails()
        {
            if (string.IsNullOrWhiteSpace(salesmenidtxtbox.Text))
                return;

            try
            {
                string salesmanId = salesmenidtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(salesmanId))
                {
                    salesmennametxtbox.Clear();
                    return;
                }

                string query = "SELECT PartyName FROM Parties WHERE PartyID = @SalesmanID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (int.TryParse(salesmanId, out int intSalesmanId))
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", intSalesmanId);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                    }

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        salesmennametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        salesmennametxtbox.Clear();
                        if (!string.IsNullOrEmpty(salesmanId))
                        {
                            MessageBox.Show("Salesman not found", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading salesman details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void productidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                LoadProductDetails();
            }
        }

        private void LoadProductDetails()
        {
            if (string.IsNullOrWhiteSpace(productidtxtbox.Text))
                return;

            try
            {
                EnsureConnectionOpen(); // Ensure connection is open

                // Check if we're in sale-linked return mode
                if (!isGeneralReturn && !string.IsNullOrWhiteSpace(orderidtxtbox.Text))
                {
                    // For sale-linked returns, fetch data from SalesBody
                    string saleQuery = @"
SELECT 
    SB.ProductId,
    P.ProductName,
    PK.PackingName,
    SB.Multiplier,
    SB.QtyPack,
    SB.QtyLose,
    SB.BnsLose,
    SB.PricePack,
    SB.DiscRatio,
    SB.DiscValPack,
    SB.DiscValLose,
    SB.STaxRatio,
    SB.TTLValue
FROM SalesBody SB
INNER JOIN Products P ON SB.ProductId = P.ProductID
LEFT JOIN Packings PK ON SB.PackingId = PK.PackingId
WHERE SB.SaleId = @SaleId AND SB.ProductId = @ProductId";

                    using (SqlCommand cmd = new SqlCommand(saleQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(orderidtxtbox.Text));
                        cmd.Parameters.AddWithValue("@ProductId", productidtxtbox.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Fill all textboxes with data from the sale
                                productnametxtbox.Text = reader["ProductName"].ToString();
                                packnamecomboBox.Text = reader["PackingName"]?.ToString() ?? "";
                                multipliertxtbox.Text = reader["Multiplier"].ToString();
                                qtyptxtbox.Text = reader["QtyPack"].ToString();
                                qtyltxtbox.Text = reader["QtyLose"].ToString();
                                bnsltxtbox.Text = reader["BnsLose"].ToString();
                                pricetxtbox.Text = reader["PricePack"].ToString();
                                discpercenttxtbox.Text = reader["DiscRatio"].ToString();
                                discvaltxtbox.Text = reader["DiscValPack"].ToString();
                                discvalltxtbox.Text = reader["DiscValLose"].ToString();
                                salestaxtxtbox.Text = reader["STaxRatio"].ToString();
                                ttlvaluetxtbox.Text = reader["TTLValue"].ToString();

                                // Set maximum allowed quantity from sale
                                int maxQtyPack = Convert.ToInt32(reader["QtyPack"]);
                                SetMaxQuantityLimits(maxQtyPack);

                                return;
                            }
                        }
                    }
                }

                // For general returns or when product not found in sale, fetch from Products table
                string query = @"
SELECT 
    p.ProductName, 
    pp.PackingId, 
    pp.Multiplier, 
    pr.PricePackSal 
FROM Products p 
LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId 
LEFT JOIN PriceHistory pr ON p.PriceControlId = pr.PriceHistoryId 
WHERE p.ProductID = @ProductID";

                int packingId = 0;
                string productName = "";
                string multiplier = "1";
                string price = "0";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productName = reader["ProductName"].ToString();

                            if (reader["PackingId"] != DBNull.Value)
                            {
                                packingId = Convert.ToInt32(reader["PackingId"]);
                            }

                            multiplier = reader["Multiplier"]?.ToString() ?? "1";
                            price = reader["PricePackSal"]?.ToString() ?? "0";
                        }
                        else
                        {
                            MessageBox.Show("Product not found", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                productnametxtbox.Text = productName;
                multipliertxtbox.Text = multiplier;
                pricetxtbox.Text = price;

                // Set default values for other fields
                qtyptxtbox.Text = "0";
                qtyltxtbox.Text = "0";
                bnsltxtbox.Text = "0";
                discpercenttxtbox.Text = "0";
                discvaltxtbox.Text = "0";
                discvalltxtbox.Text = "0";
                salestaxtxtbox.Text = "0";
                ttlvaluetxtbox.Text = "0";

                if (packingId > 0)
                {
                    LoadPackingName(packingId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product details: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetMaxQuantityLimits(int maxQtyPack)
        {
            // Store the maximum allowed quantity for validation
            // You can use this to validate during entry
            // For example, you can add a TextChanged event handler
        }
        private void ValidateQuantityAgainstSale()
        {
            if (isGeneralReturn || string.IsNullOrWhiteSpace(orderidtxtbox.Text))
                return; // No validation for general returns

            try
            {
                EnsureConnectionOpen();

                string query = @"
SELECT QtyPack, QtyLose, BnsLose 
FROM SalesBody 
WHERE SaleId = @SaleId AND ProductId = @ProductId";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(orderidtxtbox.Text));
                    cmd.Parameters.AddWithValue("@ProductId", productidtxtbox.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int maxQtyPack = Convert.ToInt32(reader["QtyPack"]);
                            int maxQtyLose = Convert.ToInt32(reader["QtyLose"]);
                            int maxBnsLose = Convert.ToInt32(reader["BnsLose"]);

                            int enteredQtyPack = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : int.Parse(qtyptxtbox.Text);
                            int enteredQtyLose = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : int.Parse(qtyltxtbox.Text);
                            int enteredBnsLose = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : int.Parse(bnsltxtbox.Text);

                            if (enteredQtyPack > maxQtyPack)
                            {
                                MessageBox.Show($"Cannot return more than {maxQtyPack} packs. This product had only {maxQtyPack} packs in the original sale.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                qtyptxtbox.Text = maxQtyPack.ToString();
                            }

                            if (enteredQtyLose > maxQtyLose)
                            {
                                MessageBox.Show($"Cannot return more than {maxQtyLose} loose items. This product had only {maxQtyLose} loose items in the original sale.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                qtyltxtbox.Text = maxQtyLose.ToString();
                            }

                            if (enteredBnsLose > maxBnsLose)
                            {
                                MessageBox.Show($"Cannot return more than {maxBnsLose} bonus items. This product had only {maxBnsLose} bonus items in the original sale.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                bnsltxtbox.Text = maxBnsLose.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error quietly
                Console.WriteLine($"Error validating quantity: {ex.Message}");
            }
        }


        private void LoadPackingName(int packingId)
        {
            try
            {
                string query = "SELECT PackingName FROM Packings WHERE PackingId = @PackingId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PackingId", packingId);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        packnamecomboBox.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading packing name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTextBox_TextChanged(sender, e);
            if (!string.IsNullOrEmpty(qtyptxtbox.Text))
            {
                ValidateQuantityAgainstSale();
            }
        }

        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTextBox_TextChanged(sender, e);
            if (!string.IsNullOrEmpty(qtyltxtbox.Text))
            {
                ValidateQuantityAgainstSale();
            }
        }

        private void bnsltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTextBox_TextChanged(sender, e);
            if (!string.IsNullOrEmpty(bnsltxtbox.Text))
            {
                ValidateQuantityAgainstSale();
            }
        }

        private void CalculateValues()
        {
            try
            {
                int qtyPack = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : int.Parse(qtyptxtbox.Text);
                int qtyLose = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : int.Parse(qtyltxtbox.Text);
                int bnsLose = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : int.Parse(bnsltxtbox.Text);
                decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : decimal.Parse(pricetxtbox.Text);
                decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : decimal.Parse(discpercenttxtbox.Text);
                decimal discValPack = string.IsNullOrEmpty(discvaltxtbox.Text) ? 0 : decimal.Parse(discvaltxtbox.Text);
                decimal discValLose = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : decimal.Parse(discvalltxtbox.Text);
                decimal salesTaxPercent = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : decimal.Parse(salestaxtxtbox.Text);

                int totalQty = qtyPack + qtyLose + bnsLose;
                decimal totalDiscount = discValPack + discValLose;
                decimal discountAmount = (price * totalQty) * (discPercent / 100);
                decimal totalValueBeforeTax = (price * totalQty) - discountAmount - totalDiscount;
                decimal salesTaxAmount = totalValueBeforeTax * (salesTaxPercent / 100);
                decimal totalValue = totalValueBeforeTax + salesTaxAmount;

                ttlvaluetxtbox.Text = totalValue.ToString("N2");
                grossvaluetxtbox.Text = (price * totalQty).ToString("N2");
                totaldiscounttxtbox.Text = (discountAmount + totalDiscount).ToString("N2");
                topsalestxtbox.Text = salesTaxAmount.ToString("N2");
                totalvaluetxtbox.Text = totalValue.ToString("N2");

                decimal prevBalance = string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text);
                decimal specialDisc = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : decimal.Parse(specialdisctxtbox.Text);
                decimal paidAmount = string.IsNullOrEmpty(paidamounttxtbox.Text) ? 0 : decimal.Parse(paidamounttxtbox.Text);

                decimal remainingBalance = (prevBalance + totalValue) - specialDisc - paidAmount;
                remainingbalancextxtbox.Text = remainingBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in calculation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddToDataGridView(decimal totalValue)
        {
            DataRow newRow = gridDataTable.NewRow();

            newRow["ProductID"] = productidtxtbox.Text.Trim();
            newRow["ProductName"] = productnametxtbox.Text;
            newRow["PackingId"] = packnamecomboBox.Text;
            newRow["Multiplier"] = string.IsNullOrWhiteSpace(multipliertxtbox.Text) ? 0 : Convert.ToInt32(multipliertxtbox.Text);
            newRow["QtyPack"] = string.IsNullOrWhiteSpace(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
            newRow["QtyLose"] = string.IsNullOrWhiteSpace(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
            newRow["BnsLose"] = string.IsNullOrWhiteSpace(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
            newRow["PricePack"] = string.IsNullOrWhiteSpace(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
            newRow["DiscRatio"] = string.IsNullOrWhiteSpace(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
            newRow["DiscValPack"] = string.IsNullOrWhiteSpace(discvaltxtbox.Text) ? 0 : Convert.ToDecimal(discvaltxtbox.Text);
            newRow["DiscValLose"] = string.IsNullOrWhiteSpace(discvalltxtbox.Text) ? 0 : Convert.ToDecimal(discvalltxtbox.Text);
            newRow["STaxRatio"] = string.IsNullOrWhiteSpace(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
            newRow["TTLValue"] = totalValue;

            gridDataTable.Rows.Add(newRow);
        }

        private void CalculateTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender == qtyptxtbox || sender == qtyltxtbox || sender == bnsltxtbox ||
                sender == discpercenttxtbox || sender == discvalltxtbox || sender == discvaltxtbox ||
                sender == salestaxtxtbox)
            {
                CalculateValues();
            }
        }

        private void OnlyNumbers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(customeridtxtbox.Text))
            {
                MessageBox.Show("Customer ID is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                customeridtxtbox.Focus();
                return;
            }

            if (gridDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product to the return!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productidtxtbox.Focus();
                return;
            }

            try
            {
                EnsureConnectionOpen(); // Add this line

                // Auto-determine return type before saving
                CheckAndSetReturnType();

                string returnTypeMessage = isGeneralReturn ? "General Return" : "Sale-linked Return";

                // Confirm with user
                DialogResult confirm = MessageBox.Show(
                    $"Save as {returnTypeMessage}?\n\n" +
                    $"Sale ID: {(string.IsNullOrWhiteSpace(orderidtxtbox.Text) ? "[Not provided]" : orderidtxtbox.Text)}\n" +
                    $"Customer: {customernametxtbox.Text}\n" +
                    $"Items: {gridDataTable.Rows.Count}",
                    "Confirm Save",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Save to SaleReturns table
                string query = @"INSERT INTO SaleReturns 
                        (ReturnId, ReturnDate, SaleId, CustomerId, SalesmanId, 
                         IsStockist, OldIsStockist, Disc2Ratio, FTaxRatio, SpecialDiscount, 
                         InvOfferDiscount, InvOfferSTax, InvOfferFTax, 
                         PaidAmount, PreviousCredit, Narration, IsPosted, UserNo, BillNo) 
                        VALUES 
                        (@ReturnId, @ReturnDate, @SaleId, @CustomerId, @SalesmanId,
                         @IsStockist, @OldIsStockist, @Disc2Ratio, @FTaxRatio, @SpecialDiscount,
                         @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
                         @PaidAmount, @PreviousCredit, @Narration, @IsPosted, @UserNo, @BillNo)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ReturnId", nextReturnId);
                    cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now);

                    // Handle SaleId based on automatic detection
                    if (!isGeneralReturn && !string.IsNullOrWhiteSpace(orderidtxtbox.Text))
                    {
                        // Sale-linked return
                        cmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(orderidtxtbox.Text));
                        cmd.Parameters.AddWithValue("@BillNo", string.IsNullOrWhiteSpace(billnootxtbox.Text) ?
                            (object)DBNull.Value : Convert.ToInt32(billnootxtbox.Text));
                    }
                    else
                    {
                        // General return
                        cmd.Parameters.AddWithValue("@SaleId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@BillNo", DBNull.Value);
                    }

                    cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);
                    cmd.Parameters.AddWithValue("@SalesmanId", string.IsNullOrWhiteSpace(salesmenidtxtbox.Text) ?
                        (object)DBNull.Value : salesmenidtxtbox.Text);
                    cmd.Parameters.AddWithValue("@IsStockist", false);
                    cmd.Parameters.AddWithValue("@OldIsStockist", false);
                    cmd.Parameters.AddWithValue("@Disc2Ratio", 0);
                    cmd.Parameters.AddWithValue("@FTaxRatio", 0);
                    cmd.Parameters.AddWithValue("@SpecialDiscount", string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : decimal.Parse(specialdisctxtbox.Text));
                    cmd.Parameters.AddWithValue("@InvOfferDiscount", 0);
                    cmd.Parameters.AddWithValue("@InvOfferSTax", 0);
                    cmd.Parameters.AddWithValue("@InvOfferFTax", 0);
                    cmd.Parameters.AddWithValue("@PaidAmount", string.IsNullOrEmpty(paidamounttxtbox.Text) ? 0 : decimal.Parse(paidamounttxtbox.Text));
                    cmd.Parameters.AddWithValue("@PreviousCredit", string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text));
                    cmd.Parameters.AddWithValue("@Narration", string.IsNullOrWhiteSpace(narrationtxtbox.Text) ?
                        (isGeneralReturn ? "General Return" : "Sale Return") : narrationtxtbox.Text);
                    cmd.Parameters.AddWithValue("@IsPosted", true);
                    cmd.Parameters.AddWithValue("@UserNo", 1); // Replace with actual user ID

                    cmd.ExecuteNonQuery();
                }

                // Save to SaleReturnsBody table
                foreach (DataRow row in gridDataTable.Rows)
                {
                    SaveReturnBody(row);
                }

                // Update stock if needed
                UpdateStockForReturn();

                MessageBox.Show($"Sale Return {nextReturnId} saved successfully!\nType: {returnTypeMessage}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadNextReturnId();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveReturnBody(DataRow row)
        {
            string query = @"INSERT INTO SaleReturnsBody 
                    (ReturnId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, 
                     BnsLose, PricePack, DiscRatio, DiscValLose, DiscValPack, 
                     OfferDiscount, ExFromSTax, STaxRatio, STaxValPack, 
                     IsValueB4Ratio, IsSTaxOnBnsPur, IsSTaxOnBns, IsFTaxExempted,
                     TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns,
                     PriceHistoryId, PerUnitCostLose, IsFreeProduct, IsDeleted) 
                    VALUES 
                    (@ReturnId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose,
                     @BnsLose, @PricePack, @DiscRatio, @DiscValLose, @DiscValPack,
                     @OfferDiscount, @ExFromSTax, @STaxRatio, @STaxValPack,
                     @IsValueB4Ratio, @IsSTaxOnBnsPur, @IsSTaxOnBns, @IsFTaxExempted,
                     @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns,
                     @PriceHistoryId, @PerUnitCostLose, @IsFreeProduct, @IsDeleted)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ReturnId", nextReturnId);
                cmd.Parameters.AddWithValue("@ProductId", row["ProductID"].ToString().Trim());
                cmd.Parameters.AddWithValue("@PackingId", GetPackingId(row["PackingId"].ToString()));
                cmd.Parameters.AddWithValue("@Multiplier", row["Multiplier"]);
                cmd.Parameters.AddWithValue("@QtyPack", row["QtyPack"]);
                cmd.Parameters.AddWithValue("@QtyLose", row["QtyLose"]);
                cmd.Parameters.AddWithValue("@BnsLose", row["BnsLose"]);
                cmd.Parameters.AddWithValue("@PricePack", row["PricePack"]);
                cmd.Parameters.AddWithValue("@DiscRatio", row["DiscRatio"]);
                cmd.Parameters.AddWithValue("@DiscValLose", row["DiscValLose"]);
                cmd.Parameters.AddWithValue("@DiscValPack", row["DiscValPack"]);
                cmd.Parameters.AddWithValue("@OfferDiscount", 0);
                cmd.Parameters.AddWithValue("@ExFromSTax", 0);
                cmd.Parameters.AddWithValue("@STaxRatio", row["STaxRatio"]);
                cmd.Parameters.AddWithValue("@STaxValPack", 0);
                cmd.Parameters.AddWithValue("@IsValueB4Ratio", false);
                cmd.Parameters.AddWithValue("@IsSTaxOnBnsPur", false);
                cmd.Parameters.AddWithValue("@IsSTaxOnBns", false);
                cmd.Parameters.AddWithValue("@IsFTaxExempted", false);
                cmd.Parameters.AddWithValue("@TTLValue", row["TTLValue"]);
                cmd.Parameters.AddWithValue("@TTLSTax", 0);
                cmd.Parameters.AddWithValue("@TTLFTax", 0);
                cmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                cmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                cmd.Parameters.AddWithValue("@PriceHistoryId", 1);
                cmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                cmd.Parameters.AddWithValue("@IsFreeProduct", false);
                cmd.Parameters.AddWithValue("@IsDeleted", false);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateStockForReturn()
        {
            // Implement stock update logic here
            foreach (DataRow row in gridDataTable.Rows)
            {
                string productId = row["ProductID"].ToString();
                int packingId = GetPackingId(row["PackingId"].ToString());
                int qtyPack = Convert.ToInt32(row["QtyPack"]);
                int qtyLose = Convert.ToInt32(row["QtyLose"]);

                // Stock update logic would go here
            }
        }

        private int GetPackingId(string packingName)
        {
            try
            {
                string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PackingName", packingName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all textboxes except return ID
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox && textBox.Name != "saleidtxtbox")
                {
                    textBox.Clear();
                }
            }

            // Clear combo box
            packnamecomboBox.SelectedIndex = -1;

            // Clear DataGridView
            gridDataTable.Clear();
            ClearItemFields();

            // Reset to general return mode
            SetReturnTypeUI(true);

            // Reload next Return ID
            LoadNextReturnId();
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                ClearItemFields();

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
                CalculateTotalValues();
            }
            else
            {
                MessageBox.Show("Please select a row to delete", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearItemFields()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnamecomboBox.SelectedIndex = -1;
            multipliertxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            pricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvaltxtbox.Clear();
            discvalltxtbox.Clear();
            salestaxtxtbox.Clear();
            ttlvaluetxtbox.Clear();
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            //this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.ShowDialog();
        }

        private void Sale_Return_Invoice_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        //private void qtyptxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        //private void qtyltxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        //private void bnsltxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        private void discpercenttxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        private void discvalltxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        private void discvaltxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);
        private void salestaxtxtbox_TextChanged(object sender, EventArgs e) => CalculateTextBox_TextChanged(sender, e);

        private void ttlvaluetxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                if (string.IsNullOrWhiteSpace(productidtxtbox.Text))
                {
                    MessageBox.Show("Please select a product first");
                    return;
                }

                decimal totalValue = Convert.ToDecimal(ttlvaluetxtbox.Text);
                AddToDataGridView(totalValue);

                // Clear item entry fields
                productidtxtbox.Clear();
                productnametxtbox.Clear();
                packnamecomboBox.SelectedIndex = -1;
                multipliertxtbox.Clear();
                qtyptxtbox.Clear();
                qtyltxtbox.Clear();
                bnsltxtbox.Clear();
                pricetxtbox.Clear();
                discpercenttxtbox.Clear();
                discvaltxtbox.Clear();
                discvalltxtbox.Clear();
                salestaxtxtbox.Clear();
                ttlvaluetxtbox.Clear();
                ClearItemFields();
                dataGridView1.ClearSelection();

                productidtxtbox.Focus();
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Form inputForm = new Form();
                inputForm.Text = "Open Sale Return";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;

                Label label = new Label();
                label.Text = "Enter Return ID to open:";
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
                        if (int.TryParse(input, out int returnId))
                        {
                            LoadSaleReturn(returnId);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid numeric Return ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening sale return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSaleReturn(int returnId)
        {
            try
            {
                ClearForm();

                string query = @"SELECT ReturnDate, SaleId, CustomerId, SalesmanId, 
                IsStockist, OldIsStockist, Disc2Ratio, FTaxRatio, 
                SpecialDiscount, InvOfferDiscount, InvOfferSTax, 
                InvOfferFTax, PaidAmount, PreviousCredit, Narration, 
                IsPosted, UserNo, BillNo 
                FROM SaleReturns 
                WHERE ReturnId = @ReturnId";

                string customerId = "";
                string salesmanId = "";
                int? saleId = null;

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ReturnId", returnId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            saleidtxtbox.Text = returnId.ToString();
                            nextReturnId = returnId;

                            // Check if this is a general return or sale-linked return
                            if (reader["SaleId"] != DBNull.Value)
                            {
                                saleId = Convert.ToInt32(reader["SaleId"]);
                                orderidtxtbox.Text = saleId.ToString();
                                SetReturnTypeUI(false);

                                // Load sale details
                                LoadSaleDetails(saleId.Value);
                            }
                            else
                            {
                                SetReturnTypeUI(true);
                            }

                            if (reader["CustomerId"] != DBNull.Value)
                            {
                                customerId = reader["CustomerId"].ToString();
                            }

                            if (reader["SalesmanId"] != DBNull.Value)
                            {
                                salesmanId = reader["SalesmanId"].ToString();
                            }

                            if (reader["SpecialDiscount"] != DBNull.Value)
                            {
                                specialdisctxtbox.Text = reader["SpecialDiscount"].ToString();
                            }

                            if (reader["PaidAmount"] != DBNull.Value)
                            {
                                paidamounttxtbox.Text = reader["PaidAmount"].ToString();
                            }

                            if (reader["PreviousCredit"] != DBNull.Value)
                            {
                                prevbaltxtbox.Text = reader["PreviousCredit"].ToString();
                            }

                            if (reader["Narration"] != DBNull.Value)
                            {
                                narrationtxtbox.Text = reader["Narration"].ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Return ID {returnId} not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(customerId))
                {
                    customeridtxtbox.Text = customerId.Trim();
                    LoadCustomerDetails();
                }

                if (!string.IsNullOrEmpty(salesmanId))
                {
                    salesmenidtxtbox.Text = salesmanId.Trim();
                    LoadSalesmanDetails();
                }

                LoadReturnBodyItems(returnId);
                CalculateTotalValues();

                string returnType = (saleId.HasValue) ? "Sale-linked Return" : "General Return";
                MessageBox.Show($"{returnType} {returnId} loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sale return: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSaleDetails(int saleId)
        {
            try
            {
                string query = @"SELECT SaleDate, BillNo FROM Sales WHERE SaleId = @SaleId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            orderdatetxtbo.Text = Convert.ToDateTime(reader["SaleDate"]).ToString("yyyy-MM-dd");
                            billnootxtbox.Text = reader["BillNo"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sale details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadReturnBodyItems(int returnId)
        {
            try
            {
                string bodyQuery = @"SELECT ProductId, PackingId, Multiplier, QtyPack, QtyLose, 
                       BnsLose, PricePack, DiscRatio, DiscValLose, DiscValPack, 
                       STaxRatio, TTLValue 
                       FROM SaleReturnsBody 
                       WHERE ReturnId = @ReturnId AND IsDeleted = 0";

                using (SqlCommand cmd = new SqlCommand(bodyQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ReturnId", returnId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        gridDataTable.Clear();

                        while (reader.Read())
                        {
                            DataRow newRow = gridDataTable.NewRow();

                            string productId = reader["ProductId"].ToString();
                            newRow["ProductID"] = productId;
                            newRow["ProductName"] = GetProductName(productId);

                            int packingId = Convert.ToInt32(reader["PackingId"]);
                            newRow["PackingId"] = GetPackingName(packingId);

                            newRow["Multiplier"] = reader["Multiplier"];
                            newRow["QtyPack"] = reader["QtyPack"];
                            newRow["QtyLose"] = reader["QtyLose"];
                            newRow["BnsLose"] = reader["BnsLose"];
                            newRow["PricePack"] = reader["PricePack"];
                            newRow["DiscRatio"] = reader["DiscRatio"];
                            newRow["DiscValPack"] = reader["DiscValPack"];
                            newRow["DiscValLose"] = reader["DiscValLose"];
                            newRow["STaxRatio"] = reader["STaxRatio"];
                            newRow["TTLValue"] = reader["TTLValue"];

                            gridDataTable.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading return items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetProductName(string productId)
        {
            try
            {
                string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "Unknown Product";
                }
            }
            catch
            {
                return "Unknown Product";
            }
        }

        private string GetPackingName(int packingId)
        {
            try
            {
                string query = "SELECT PackingName FROM Packings WHERE PackingId = @PackingId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PackingId", packingId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "Unknown Packing";
                }
            }
            catch
            {
                return "Unknown Packing";
            }
        }

        private void CalculateTotalValues()
        {
            decimal totalGrossValue = 0;
            decimal totalDiscount = 0;
            decimal totalSalesTax = 0;
            decimal totalValue = 0;

            foreach (DataRow row in gridDataTable.Rows)
            {
                totalValue += Convert.ToDecimal(row["TTLValue"]);

                decimal price = Convert.ToDecimal(row["PricePack"]);
                int qtyPack = Convert.ToInt32(row["QtyPack"]);
                int qtyLose = Convert.ToInt32(row["QtyLose"]);
                int bnsLose = Convert.ToInt32(row["BnsLose"]);
                decimal discRatio = Convert.ToDecimal(row["DiscRatio"]);
                decimal staxRatio = Convert.ToDecimal(row["STaxRatio"]);

                // Calculate gross value
                int totalQty = qtyPack + qtyLose + bnsLose;
                totalGrossValue += price * totalQty;

                // Calculate discount
                decimal itemDiscount = (price * totalQty) * (discRatio / 100);
                itemDiscount += Convert.ToDecimal(row["DiscValPack"]) + Convert.ToDecimal(row["DiscValLose"]);
                totalDiscount += itemDiscount;

                // Calculate sales tax
                decimal itemValueBeforeTax = (price * totalQty) - itemDiscount;
                totalSalesTax += itemValueBeforeTax * (staxRatio / 100);
            }

            grossvaluetxtbox.Text = totalGrossValue.ToString("N2");
            totaldiscounttxtbox.Text = totalDiscount.ToString("N2");
            topsalestxtbox.Text = totalSalesTax.ToString("N2");
            totalvaluetxtbox.Text = totalValue.ToString("N2");

            decimal prevBalance = string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text);
            decimal specialDisc = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : decimal.Parse(specialdisctxtbox.Text);
            decimal paidAmount = string.IsNullOrEmpty(paidamounttxtbox.Text) ? 0 : decimal.Parse(paidamounttxtbox.Text);

            decimal remainingBalance = (prevBalance + totalValue) - specialDisc - paidAmount;
            remainingbalancextxtbox.Text = remainingBalance.ToString("N2");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}