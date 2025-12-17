using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;

namespace FO_2022
{
    public partial class Purchase_Direct_to_Sale : Form
    {
        private SqlConnection connection;
        private DataTable packingData = new DataTable();
        private decimal totalGross = 0;
        private decimal totalDiscount = 0;
        private decimal netValue = 0;
        private decimal totalPurValue = 0;
        private decimal topSalesTax = 0;
        private decimal totalRateDiff = 0;
        private decimal totalSaleValue = 0;

        public Purchase_Direct_to_Sale()
        {
            InitializeComponent();
            InitializeConnection();
            LoadInitialData();
            SetupDataGridView();
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
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInitialData()
        {
            try
            {
                // Load next Purchase ID
                purchasedtxtbox.Text = GetNextPurchaseId().ToString();
                purchasedtxtbox.Enabled = false;

                // Load next Sale ID
                saleidtxtbox.Text = GetNextSaleId().ToString();
                saleidtxtbox.Enabled = false;

                // Load packing data
                LoadPackingData();

                // Set current dates
                dateTimePicker1.Value = DateTime.Now;
                saledatepcker.Value = DateTime.Now;
                //returndatepcker.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading initial data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetNextPurchaseId()
        {
            string query = "SELECT ISNULL(MAX(PurchaseID), 0) + 1 FROM Purchase";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int GetNextSaleId()
        {
            string query = "SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Sales";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void LoadPackingData()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                using (SqlDataAdapter da = new SqlDataAdapter(query, connection))
                {
                    packingData.Clear();
                    da.Fill(packingData);
                    packnamecombobox.DataSource = packingData;
                    packnamecombobox.DisplayMember = "PackingName";
                    packnamecombobox.ValueMember = "PackingId";
                    packnamecombobox.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packing data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Add columns
            dataGridView1.Columns.Add("colProductID", "Product ID");
            dataGridView1.Columns.Add("colProductName", "Product Name");
            dataGridView1.Columns.Add("colPacking", "Packing");
            dataGridView1.Columns.Add("colQtyP", "Qty(P)");
            dataGridView1.Columns.Add("colQtyL", "Qty(L)");
            dataGridView1.Columns.Add("colBnsL", "Bns(L)");
            dataGridView1.Columns.Add("colPurPrice", "Pur Price");
            dataGridView1.Columns.Add("colDiscPercent", "Disc%");
            dataGridView1.Columns.Add("colDiscountValue", "Disc Value");
            dataGridView1.Columns.Add("colSalePrice", "Sale Price");
            dataGridView1.Columns.Add("colSaleTax", "Sales Tax%");
            dataGridView1.Columns.Add("colSalesTaxValue", "Sales Tax Value");
            dataGridView1.Columns.Add("colRateDiff", "Rate Diff");
            dataGridView1.Columns.Add("colTotalValue", "Total Value");
            dataGridView1.Columns.Add("colTotalSaleValue", "Total Sale Value");

            // Set column widths
            dataGridView1.Columns["colProductID"].Width = 80;
            dataGridView1.Columns["colProductName"].Width = 150;
            dataGridView1.Columns["colPacking"].Width = 80;
            dataGridView1.Columns["colQtyP"].Width = 60;
            dataGridView1.Columns["colQtyL"].Width = 60;
            dataGridView1.Columns["colBnsL"].Width = 60;
            dataGridView1.Columns["colPurPrice"].Width = 80;
            dataGridView1.Columns["colDiscPercent"].Width = 60;
            dataGridView1.Columns["colDiscountValue"].Width = 80;
            dataGridView1.Columns["colSalePrice"].Width = 80;
            dataGridView1.Columns["colSaleTax"].Width = 80;
            dataGridView1.Columns["colSalesTaxValue"].Width = 100;
            dataGridView1.Columns["colRateDiff"].Width = 80;
            dataGridView1.Columns["colTotalValue"].Width = 90;
            dataGridView1.Columns["colTotalSaleValue"].Width = 100;

            // Make some columns read-only
            dataGridView1.Columns["colTotalValue"].ReadOnly = true;
            dataGridView1.Columns["colTotalSaleValue"].ReadOnly = true;
            dataGridView1.Columns["colDiscountValue"].ReadOnly = true;
            dataGridView1.Columns["colSalesTaxValue"].ReadOnly = true;
            dataGridView1.Columns["colRateDiff"].ReadOnly = true;
        }

        // ============= VENDOR SECTION =============
        private void vendoridtxtbox_Leave(object sender, EventArgs e)
        {
            FetchVendorData();
        }

        private void vendoridtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string originalText = vendoridtxtbox.Text;
                FetchVendorData();
                vendoridtxtbox.Text = originalText;
                this.SelectNextControl(vendoridtxtbox, true, true, true, true);
            }
        }

        private void FetchVendorData()
        {
            string vendorId = vendoridtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(vendorId))
            {
                ClearVendorFields();
                return;
            }

            try
            {
                string query = "SELECT PartyName, City FROM Parties WHERE PartyID = @PartyID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PartyID", vendorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vendornametxtbox.Text = reader["PartyName"].ToString();
                            citytxtbox.Text = reader["City"] != DBNull.Value ? reader["City"].ToString() : "";
                            //citytxtbox.Text = reader["TownName"] != DBNull.Value ? reader["TownName"].ToString() : "";

                        }
                        else
                        {
                            MessageBox.Show("Vendor not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearVendorFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching vendor data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearVendorFields()
        {
            vendornametxtbox.Clear();
            citytxtbox.Clear();
        }

        // ============= PRODUCT SECTION =============
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string productId = productidtxtbox.Text.Trim();

                if (string.IsNullOrWhiteSpace(productId))
                {
                    MessageBox.Show("Please enter Product ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    productidtxtbox.Focus();
                    return;
                }

                FetchProductData(productId);

                // Move focus to quantity field
                qtyptxtbox.Focus();
                qtyptxtbox.SelectAll();
            }
        }

        private void productidtxtbox_Leave(object sender, EventArgs e)
        {
            string productId = productidtxtbox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(productId))
            {
                FetchProductData(productId);
            }
        }

        private void FetchProductData(string productId)
        {
            try
            {
                string query = @"SELECT p.ProductID, p.ProductName, p.PurchasePrice, p.SalePrice, 
                                p.PurDiscRatio, p.SaleDiscRatio, p.PurSTRatio, p.SaleSTRatio,
                                p.RetailPrice, p.MinStockLevel, p.OrderStockLevel,
                                c.CompanyName, pg.GroupName, psg.SubGroupName
                                FROM Products p
                                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                                LEFT JOIN ProductSubGroups psg ON p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId AND p.CompanyID = psg.CompanyId
                                WHERE p.ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Update UI on the main thread
                            this.Invoke(new Action(() =>
                            {
                                productnametxtbox.Text = reader["ProductName"].ToString();
                                purpricetxtbox.Text = Convert.ToDecimal(reader["PurchasePrice"]).ToString("F2");
                                salepricetxtbox.Text = Convert.ToDecimal(reader["SalePrice"]).ToString("F2");
                                discpercenttxtbox.Text = Convert.ToDecimal(reader["PurDiscRatio"]).ToString("F2");
                                salestaxtxtbox.Text = Convert.ToDecimal(reader["PurSTRatio"]).ToString("F2");

                                // Calculate discount value
                                CalculateDiscountValue();

                                // Calculate rate difference
                                CalculateRateDifference();

                                // Load current stock
                                //LoadCurrentStock(productId);

                                // Reset product ID to ensure it stays
                                productidtxtbox.Text = productId;
                            }));
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show($"Product ID '{productId}' not found!", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ClearProductFieldsExceptId(productId);
                                productidtxtbox.Focus();
                                productidtxtbox.SelectAll();
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching product data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateDiscountValue()
        {
            if (decimal.TryParse(purpricetxtbox.Text, out decimal purchasePrice) &&
                decimal.TryParse(discpercenttxtbox.Text, out decimal discountPercent))
            {
                decimal discountValue = purchasePrice * (discountPercent / 100);
                discvalltxtbox.Text = discountValue.ToString("F2");
            }
        }

        private void CalculateRateDifference()
        {
            if (decimal.TryParse(purpricetxtbox.Text, out decimal purchasePrice) &&
                decimal.TryParse(salepricetxtbox.Text, out decimal salePrice))
            {
                decimal rateDiff = salePrice - purchasePrice;
                ratedifftxtbox.Text = rateDiff.ToString("F2");
            }
        }



        // ============= ADD PRODUCT TO GRID =============

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (packnamecombobox.SelectedValue == null ||
                packnamecombobox.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(productidtxtbox.Text) ||
                string.IsNullOrWhiteSpace(productnametxtbox.Text))
                return;
            // Naya code: Multiplier hasil karein aur dikhayein
            try
            {
                string productId = productidtxtbox.Text.Trim();
                string packingId = packnamecombobox.SelectedValue.ToString();

                // Multiplier hasil karein
                decimal multiplier = GetPackingMultiplier(productId, packingId);

                // Multiplier ko text box mein dikhayein
                multipliertxtbox.Text = multiplier.ToString();


                // Validate required fields
                if (string.IsNullOrWhiteSpace(qtyptxtbox.Text) || string.IsNullOrWhiteSpace(qtyltxtbox.Text))
                {
                    MessageBox.Show("Please enter quantity in both pack and loose", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string PackingId = packnamecombobox.SelectedValue.ToString();
                    string ProductId = productidtxtbox.Text;

                    // Get multiplier
                    decimal multipliertxtbox = GetPackingMultiplier(productId, packingId);

                    // Parse values with validation
                    decimal qtyPack = decimal.Parse(qtyptxtbox.Text);
                    decimal qtyLose = decimal.Parse(qtyltxtbox.Text);
                    decimal bnsLose = string.IsNullOrWhiteSpace(bnsltxtbox.Text) ? 0 : decimal.Parse(bnsltxtbox.Text);
                    decimal purchasePrice = decimal.Parse(purpricetxtbox.Text);
                    decimal discountPercent = decimal.Parse(discpercenttxtbox.Text);
                    decimal salesPrice = decimal.Parse(salepricetxtbox.Text);
                    decimal salesTaxPercent = decimal.Parse(salestaxtxtbox.Text);

                    // Calculations
                    decimal discountValue = purchasePrice * (discountPercent / 100);
                    decimal netPurchasePrice = purchasePrice - discountValue;
                    decimal salesTaxValue = salesPrice * (salesTaxPercent / 100);
                    decimal netSalePrice = salesPrice + salesTaxValue;
                    decimal rateDiff = netSalePrice - netPurchasePrice;

                    decimal totalValue = (qtyPack * netPurchasePrice) + (qtyLose * netPurchasePrice / multiplier);
                    decimal totalSaleVal = (qtyPack * netSalePrice) + (qtyLose * netSalePrice / multiplier);

                    // Add to DataGridView
                    int rowIndex = dataGridView1.Rows.Add(
                        productId,
                        productnametxtbox.Text,
                        packnamecombobox.Text,
                        qtyPack.ToString("F2"),
                        qtyLose.ToString("F2"),
                        bnsLose.ToString("F2"),
                        purchasePrice.ToString("F2"),
                        discountPercent.ToString("F2"),
                        discountValue.ToString("F2"),
                        salesPrice.ToString("F2"),
                        salesTaxPercent.ToString("F2"),
                        salesTaxValue.ToString("F2"),
                        rateDiff.ToString("F2"),
                        totalValue.ToString("F2"),
                        totalSaleVal.ToString("F2")
                    );

                    // Ensure all cells have values
                    for (int i = 0; i < dataGridView1.Rows[rowIndex].Cells.Count; i++)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells[i].Value == null)
                        {
                            dataGridView1.Rows[rowIndex].Cells[i].Value = "";
                        }
                    }

                    // Update totals
                    UpdateTotals();

                    // Clear fields for next entry
                    ClearProductFieldsForNextEntry(productId);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter valid numeric values", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding product to grid: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving packing multiplier: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddProductToGridFromQuantity();
                this.SelectNextControl(qtyltxtbox, true, true, true, true);
            }
        }
        private void AddProductToGridFromQuantity()
        {
            try
            {
                // Validate required fields
                if (packnamecombobox.SelectedValue == null ||
                    packnamecombobox.SelectedIndex == -1 ||
                    string.IsNullOrWhiteSpace(productidtxtbox.Text) ||
                    string.IsNullOrWhiteSpace(productnametxtbox.Text) ||
                    string.IsNullOrWhiteSpace(qtyltxtbox.Text))
                {
                    MessageBox.Show("Please fill all required fields", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate other required fields
                if (string.IsNullOrWhiteSpace(qtyptxtbox.Text))
                {
                    MessageBox.Show("Please enter quantity in pack", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    qtyptxtbox.Focus();
                    return;
                }

                string packingId = packnamecombobox.SelectedValue.ToString();
                string productId = productidtxtbox.Text;

                // Get multiplier
                decimal multiplier = GetPackingMultiplier(productId, packingId);

                // Set multiplier in text box
                multipliertxtbox.Text = multiplier.ToString();

                // Parse values with validation
                decimal qtyPack = decimal.Parse(qtyptxtbox.Text);
                decimal qtyLose = decimal.Parse(qtyltxtbox.Text);
                decimal bnsLose = string.IsNullOrWhiteSpace(bnsltxtbox.Text) ? 0 : decimal.Parse(bnsltxtbox.Text);
                decimal purchasePrice = decimal.Parse(purpricetxtbox.Text);
                decimal discountPercent = decimal.Parse(discpercenttxtbox.Text);
                decimal salesPrice = decimal.Parse(salepricetxtbox.Text);
                decimal salesTaxPercent = decimal.Parse(salestaxtxtbox.Text);

                // Calculations
                decimal discountValue = purchasePrice * (discountPercent / 100);
                decimal netPurchasePrice = purchasePrice - discountValue;
                decimal salesTaxValue = salesPrice * (salesTaxPercent / 100);
                decimal netSalePrice = salesPrice + salesTaxValue;
                decimal rateDiff = netSalePrice - netPurchasePrice;

                decimal totalValue = (qtyPack * netPurchasePrice) + (qtyLose * netPurchasePrice / multiplier);
                decimal totalSaleVal = (qtyPack * netSalePrice) + (qtyLose * netSalePrice / multiplier);

                // Add to DataGridView
                int rowIndex = dataGridView1.Rows.Add(
                    productId,
                    productnametxtbox.Text,
                    packnamecombobox.Text,
                    qtyPack.ToString("F2"),
                    qtyLose.ToString("F2"),
                    bnsLose.ToString("F2"),
                    purchasePrice.ToString("F2"),
                    discountPercent.ToString("F2"),
                    discountValue.ToString("F2"),
                    salesPrice.ToString("F2"),
                    salesTaxPercent.ToString("F2"),
                    salesTaxValue.ToString("F2"),
                    rateDiff.ToString("F2"),
                    totalValue.ToString("F2"),
                    totalSaleVal.ToString("F2")
                );

                // Ensure all cells have values
                for (int i = 0; i < dataGridView1.Rows[rowIndex].Cells.Count; i++)
                {
                    if (dataGridView1.Rows[rowIndex].Cells[i].Value == null)
                    {
                        dataGridView1.Rows[rowIndex].Cells[i].Value = "";
                    }
                }

                // Update totals
                UpdateTotals();


                // Clear fields for next entry
                ClearProductFieldsForNextEntry(productId);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to grid: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private decimal GetPackingMultiplier(string productId, string packingId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(packingId))
                    return 1;

                string query = "SELECT Multiplier FROM ProductPacking WHERE ProductId = @ProductID AND PackingId = @PackingId";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@PackingId", packingId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDecimal(result) : 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        private void ClearProductFieldsForNextEntry(string productIdToKeep)
        {
            productidtxtbox.Text = productIdToKeep;
            productnametxtbox.Clear();
            packnamecombobox.SelectedIndex = -1;
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            purpricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            salepricetxtbox.Clear();
            salestaxtxtbox.Clear();
            ratedifftxtbox.Clear();
            currentstocltxtbox.Clear();

            productidtxtbox.Focus();
            productidtxtbox.SelectAll();
        }

        private void ClearProductFieldsExceptId(string productId)
        {
            productidtxtbox.Text = productId;
            productnametxtbox.Clear();
            packnamecombobox.SelectedIndex = -1;
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            purpricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            salepricetxtbox.Clear();
            salestaxtxtbox.Clear();
            ratedifftxtbox.Clear();
            currentstocltxtbox.Clear();
        }

        // ============= CUSTOMER SECTION =============
        private void customeridtxtbox_Leave(object sender, EventArgs e)
        {
            FetchCustomerData();
        }

        private void customeridtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string originalText = customeridtxtbox.Text;
                FetchCustomerData();
                customeridtxtbox.Text = originalText;
                this.SelectNextControl(customeridtxtbox, true, true, true, true);
            }
        }

        private void FetchCustomerData()
        {
            string customerId = customeridtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                ClearCustomerFields();
                return;
            }

            try
            {
                string query = @"SELECT p.PartyName, s.SectorName, t.TownName 
                                FROM Parties p
                                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                                LEFT JOIN Towns t ON s.TownID = t.TownID
                                WHERE p.PartyID = @PartyID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PartyID", customerId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customernametxtbox.Text = reader["PartyName"].ToString();
                            sectortxtbox.Text = reader["SectorName"] != DBNull.Value ? reader["SectorName"].ToString() : "";
                            towntxtbox.Text = reader["TownName"] != DBNull.Value ? reader["TownName"].ToString() : "";
                        }
                        else
                        {
                            MessageBox.Show("Customer not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearCustomerFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching customer data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearCustomerFields()
        {
            customernametxtbox.Clear();
            sectortxtbox.Clear();
            towntxtbox.Clear();
        }

        // ============= SALESMAN SECTION =============
        private void salesmanidtxtbox_Leave(object sender, EventArgs e)
        {
            FetchSalesmanData();
        }

        private void salesmanidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string originalText = salesmanidtxtbox.Text;
                FetchSalesmanData();
                salesmanidtxtbox.Text = originalText;
                this.SelectNextControl(salesmanidtxtbox, true, true, true, true);
            }
        }

        private void FetchSalesmanData()
        {
            string salesmanId = salesmanidtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(salesmanId))
            {
                salesmannametxtbox.Clear();
                return;
            }

            try
            {
                string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                    object result = cmd.ExecuteScalar();
                    salesmannametxtbox.Text = result?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching salesman data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============= CALCULATIONS =============
      
        private void UpdateTotals()
        {
            totalGross = 0;
            totalDiscount = 0;
            netValue = 0;
            totalPurValue = 0;
            topSalesTax = 0;
            totalRateDiff = 0;
            totalSaleValue = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["colTotalValue"].Value != null)
                    totalPurValue += decimal.Parse(row.Cells["colTotalValue"].Value.ToString());

                if (row.Cells["colDiscountValue"].Value != null)
                    totalDiscount += decimal.Parse(row.Cells["colDiscountValue"].Value.ToString());

                if (row.Cells["colSalesTaxValue"].Value != null)
                    topSalesTax += decimal.Parse(row.Cells["colSalesTaxValue"].Value.ToString());

                if (row.Cells["colRateDiff"].Value != null)
                    totalRateDiff += decimal.Parse(row.Cells["colRateDiff"].Value.ToString());

                if (row.Cells["colTotalSaleValue"].Value != null)
                    totalSaleValue += decimal.Parse(row.Cells["colTotalSaleValue"].Value.ToString());
            }

            // Calculate other totals
            totalGross = totalPurValue + totalDiscount;
            netValue = totalPurValue;

            // Update textboxes
            totalgrosstxtbox.Text = totalGross.ToString("F2");
            totaldiscounttxtbox.Text = totalDiscount.ToString("F2");
            netvaluetxtbox.Text = netValue.ToString("F2");
            totalpurvaluetxtbox.Text = totalPurValue.ToString("F2");
            topsalestaxtxtbox.Text = topSalesTax.ToString("F2");
            totalratedifftxtbox.Text = totalRateDiff.ToString("F2");
            totalsalevaluetxtbox.Text = totalSaleValue.ToString("F2");

            // NEW CODE: Copy values to other textboxes
            // Copy total purchase value to net purchase value
            if (!string.IsNullOrWhiteSpace(totalpurvaluetxtbox.Text))
            {
                netpurvaluetxt.Text = totalpurvaluetxtbox.Text;
            }

            // Copy total sale value to net sale value
            if (!string.IsNullOrWhiteSpace(totalsalevaluetxtbox.Text))
            {
                netsalevaluetxtbox.Text = totalsalevaluetxtbox.Text;
            }

            // Calculate final balance
            CalculateFinalBalance();
        }
        
        private void CalculateFinalBalance()
        {
            try
            {
                decimal previousBalance = string.IsNullOrWhiteSpace(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text);
                decimal specialDiscount = string.IsNullOrWhiteSpace(specialdistxtbox.Text) ? 0 : decimal.Parse(specialdistxtbox.Text);
                decimal receivedAmount = string.IsNullOrWhiteSpace(receivedtxtbox.Text) ? 0 : decimal.Parse(receivedtxtbox.Text);

                // Get total sale value from totalsalevaluetxtbox instead of netvaluetxtbox + salestax
                decimal totalSaleValue = string.IsNullOrWhiteSpace(totalsalevaluetxtbox.Text) ? 0 : decimal.Parse(totalsalevaluetxtbox.Text);

                // NEW CALCULATION: Use totalSaleValue instead of (netAmount + salesTax)
                decimal totalAmount = totalSaleValue - specialDiscount;
                decimal finalBalance = previousBalance + totalAmount - receivedAmount;

                finalbaltxtbox.Text = finalBalance.ToString("F2");
                alenetvaluetxtbox.Text = totalAmount.ToString("F2");

                // Calculate paid amount
                decimal paidAmount = totalAmount - receivedAmount;
                paidamounttxtbox.Text = paidAmount.ToString("F2");
            }
            catch
            {
                // Ignore parse errors
            }
        }

        private void receivedtxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFinalBalance();
        }

        private void prevbaltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFinalBalance();
        }

        private void specialdistxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateFinalBalance();
        }

        // ============= SAVE TRANSACTION =============
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    try
                    {
                        // 1. Save Purchase Header
                        SavePurchaseHeader();

                        // 2. Save Purchase Body
                        SavePurchaseBody();

                        // 3. Save Sale Header
                        SaveSaleHeader();

                        // 4. Save Sale Body
                        SaveSaleBody();

                        // 5. Update Current Stock
                        UpdateCurrentStock();

                        // Complete transaction
                        transaction.Complete();

                        MessageBox.Show("Purchase and Sale saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear form and reload IDs
                        ClearForm();
                        LoadInitialData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving transaction: " + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw; // This will rollback the transaction
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transaction error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            // Validate Vendor
            if (string.IsNullOrWhiteSpace(vendoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Vendor ID", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendoridtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(vendornametxtbox.Text))
            {
                MessageBox.Show("Please select a valid vendor", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendoridtxtbox.Focus();
                return false;
            }

            // Validate Customer
            if (string.IsNullOrWhiteSpace(customeridtxtbox.Text))
            {
                MessageBox.Show("Please enter Customer ID", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                customeridtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(customernametxtbox.Text))
            {
                MessageBox.Show("Please select a valid customer", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                customeridtxtbox.Focus();
                return false;
            }

            // Validate Products
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productidtxtbox.Focus();
                return false;
            }

            // Validate Bill Numbers
            if (string.IsNullOrWhiteSpace(billpurchasetxtbox.Text))
            {
                MessageBox.Show("Please enter Purchase Bill No", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                billpurchasetxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(salebilltxtbox.Text))
            {
                MessageBox.Show("Please enter Sale Bill No", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salebilltxtbox.Focus();
                return false;
            }

            return true;
        }

        private void SavePurchaseHeader()
        {
            string query = @"INSERT INTO Purchase (PurchaseID, PurchaseDate, VendorID, BillNo, BillDate, 
                           PaidAmount, Narration, BiltyNo, UserNo, IsPendingRate, IsNightShift)
                           VALUES (@PurchaseID, @PurchaseDate, @VendorID, @BillNo, @BillDate, 
                           @PaidAmount, @Narration, @BiltyNo, @UserNo, @IsPendingRate, @IsNightShift)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PurchaseID", int.Parse(purchasedtxtbox.Text));
                cmd.Parameters.AddWithValue("@PurchaseDate", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                cmd.Parameters.AddWithValue("@BillNo", billpurchasetxtbox.Text);
                cmd.Parameters.AddWithValue("@BillDate", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@PaidAmount", DBNull.Value);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);
                cmd.Parameters.AddWithValue("@BiltyNo", vehiclenotxt.Text);
                cmd.Parameters.AddWithValue("@UserNo", 1); // Current user ID
                cmd.Parameters.AddWithValue("@IsPendingRate", 0);
                cmd.Parameters.AddWithValue("@IsNightShift", 0);

                cmd.ExecuteNonQuery();
            }
        }

        private void SavePurchaseBody()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Skip empty rows
                if (row.IsNewRow) continue;

                // Validate all required values
                if (row.Cells["colProductID"].Value == null ||
                    row.Cells["colPacking"].Value == null ||
                    row.Cells["colQtyP"].Value == null ||
                    row.Cells["colQtyL"].Value == null ||
                    row.Cells["colBnsL"].Value == null ||
                    row.Cells["colPurPrice"].Value == null ||
                    row.Cells["colDiscPercent"].Value == null ||
                    row.Cells["colDiscountValue"].Value == null ||
                    row.Cells["colSaleTax"].Value == null ||
                    row.Cells["colSalesTaxValue"].Value == null ||
                    row.Cells["colTotalValue"].Value == null)
                {
                    MessageBox.Show($"Row {row.Index + 1} has missing values. Please check and try again.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"INSERT INTO PurchasesBody (PurchaseId, ProductId, PackingId, Multiplier, 
                       QtyPack, QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, 
                       STaxRatio, STaxValPack, TTLValue, TTLSTax)
                       VALUES (@PurchaseId, @ProductId, @PackingId, @Multiplier, 
                       @QtyPack, @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, 
                       @STaxRatio, @STaxValPack, @TTLValue, @TTLSTax)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    string packingName = row.Cells["colPacking"].Value.ToString();
                    string productId = row.Cells["colProductID"].Value.ToString();
                    int packingId = GetPackingId(packingName);
                    decimal multiplier = GetPackingMultiplier(productId, packingId.ToString());

                    cmd.Parameters.AddWithValue("@PurchaseId", int.Parse(purchasedtxtbox.Text));
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@PackingId", packingId);
                    cmd.Parameters.AddWithValue("@Multiplier", multiplier);
                    cmd.Parameters.AddWithValue("@QtyPack", decimal.Parse(row.Cells["colQtyP"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@QtyLose", decimal.Parse(row.Cells["colQtyL"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@BnsLose",
                        row.Cells["colBnsL"].Value != null && !string.IsNullOrEmpty(row.Cells["colBnsL"].Value.ToString())
                        ? decimal.Parse(row.Cells["colBnsL"].Value.ToString())
                        : 0);
                    cmd.Parameters.AddWithValue("@PricePack", decimal.Parse(row.Cells["colPurPrice"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@DiscRatio", decimal.Parse(row.Cells["colDiscPercent"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@DiscValPack", decimal.Parse(row.Cells["colDiscountValue"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@STaxRatio", decimal.Parse(row.Cells["colSaleTax"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@STaxValPack", decimal.Parse(row.Cells["colSalesTaxValue"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@TTLValue", decimal.Parse(row.Cells["colTotalValue"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@TTLSTax", decimal.Parse(row.Cells["colSalesTaxValue"].Value.ToString()));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveSaleHeader()
        {
            string query = @"INSERT INTO Sales (SaleId, SaleDate, CustomerId, SalesmanId, 
                           Narration, SpecialDiscount, ReceivedAmount, PreviousCredit, 
                           InvoiceNotes, IsPosted, UserNo, BiltyNo, CashReceived, 
                           CustomerName, BillYear, IsLocked)
                           VALUES (@SaleId, @SaleDate, @CustomerId, @SalesmanId, 
                           @Narration, @SpecialDiscount, @ReceivedAmount, @PreviousCredit, 
                           @InvoiceNotes, @IsPosted, @UserNo, @BiltyNo, @CashReceived, 
                           @CustomerName, @BillYear, @IsLocked)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@SaleId", int.Parse(saleidtxtbox.Text));
                cmd.Parameters.AddWithValue("@SaleDate", saledatepcker.Value);
                cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);
                cmd.Parameters.AddWithValue("@SalesmanId", salesmanidtxtbox.Text);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);
                cmd.Parameters.AddWithValue("@SpecialDiscount",
                    string.IsNullOrWhiteSpace(specialdistxtbox.Text) ? 0 : decimal.Parse(specialdistxtbox.Text));
                cmd.Parameters.AddWithValue("@ReceivedAmount",
                    string.IsNullOrWhiteSpace(receivedtxtbox.Text) ? 0 : decimal.Parse(receivedtxtbox.Text));
                cmd.Parameters.AddWithValue("@PreviousCredit",
                    string.IsNullOrWhiteSpace(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text));
                cmd.Parameters.AddWithValue("@InvoiceNotes", "");
                cmd.Parameters.AddWithValue("@IsPosted", 1);
                cmd.Parameters.AddWithValue("@UserNo", 1);
                cmd.Parameters.AddWithValue("@BiltyNo", vehiclenotxt.Text);
                cmd.Parameters.AddWithValue("@CashReceived",
                    string.IsNullOrWhiteSpace(receivedtxtbox.Text) ? 0 : decimal.Parse(receivedtxtbox.Text));
                cmd.Parameters.AddWithValue("@CustomerName", customernametxtbox.Text);
                cmd.Parameters.AddWithValue("@BillYear", DateTime.Now.Year.ToString());
                cmd.Parameters.AddWithValue("@IsLocked", 1);

                cmd.ExecuteNonQuery();
            }
        }

      
        private void SaveSaleBody()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Skip empty rows
                if (row.IsNewRow) continue;

                // Validate all required values
                if (row.Cells["colProductID"].Value == null ||
                    row.Cells["colPacking"].Value == null ||
                    row.Cells["colQtyP"].Value == null ||
                    row.Cells["colQtyL"].Value == null ||
                    row.Cells["colBnsL"].Value == null ||
                    row.Cells["colSalePrice"].Value == null ||
                    row.Cells["colSaleTax"].Value == null ||
                    row.Cells["colSalesTaxValue"].Value == null ||
                    row.Cells["colTotalSaleValue"].Value == null)
                {
                    MessageBox.Show($"Row {row.Index + 1} has missing values for sale. Please check and try again.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"INSERT INTO SalesBody (SaleId, ProductId, PackingId, Multiplier, 
                       QtyPack, QtyLose, BnsLose, PricePack, STaxRatio, STaxValPack, 
                       TTLValue, TTLSTax)
                       VALUES (@SaleId, @ProductId, @PackingId, @Multiplier, 
                       @QtyPack, @QtyLose, @BnsLose, @PricePack, @STaxRatio, @STaxValPack, 
                       @TTLValue, @TTLSTax)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    string packingName = row.Cells["colPacking"].Value.ToString();
                    string productId = row.Cells["colProductID"].Value.ToString();
                    int packingId = GetPackingId(packingName);
                    decimal multiplier = GetPackingMultiplier(productId, packingId.ToString());

                    cmd.Parameters.AddWithValue("@SaleId", int.Parse(saleidtxtbox.Text));
                    cmd.Parameters.AddWithValue("@ProductId", productId);
                    cmd.Parameters.AddWithValue("@PackingId", packingId);
                    cmd.Parameters.AddWithValue("@Multiplier", multiplier);
                    cmd.Parameters.AddWithValue("@QtyPack", decimal.Parse(row.Cells["colQtyP"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@QtyLose", decimal.Parse(row.Cells["colQtyL"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@BnsLose",
                        row.Cells["colBnsL"].Value != null && !string.IsNullOrEmpty(row.Cells["colBnsL"].Value.ToString())
                        ? decimal.Parse(row.Cells["colBnsL"].Value.ToString())
                        : 0);
                    cmd.Parameters.AddWithValue("@PricePack", decimal.Parse(row.Cells["colSalePrice"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@STaxRatio", decimal.Parse(row.Cells["colSaleTax"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@STaxValPack", decimal.Parse(row.Cells["colSalesTaxValue"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@TTLValue", decimal.Parse(row.Cells["colTotalSaleValue"].Value.ToString()));
                    cmd.Parameters.AddWithValue("@TTLSTax", decimal.Parse(row.Cells["colSalesTaxValue"].Value.ToString()));

                    cmd.ExecuteNonQuery();
                }
            }
        }

      
        private void UpdateCurrentStock()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Skip empty rows
                if (row.IsNewRow) continue;

                string productId = row.Cells["colProductID"].Value?.ToString();
                if (string.IsNullOrEmpty(productId)) continue;

                decimal qtyLose = decimal.Parse(row.Cells["colQtyL"].Value.ToString());
                decimal qtyPack = decimal.Parse(row.Cells["colQtyP"].Value.ToString());

                // Get packing multiplier for proper calculation
                string packingName = row.Cells["colPacking"].Value.ToString();
                int packingId = GetPackingId(packingName);
                decimal multiplier = GetPackingMultiplier(productId, packingId.ToString());

                // Calculate total quantity in lose units
                decimal totalQty = qtyLose + (qtyPack * multiplier);

                // Check if product exists in CurrentStock
                string checkQuery = "SELECT COUNT(*) FROM CurrentStock WHERE ProductiD = @ProductID";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@ProductID", productId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        // Update existing stock (reduce stock as we're selling)
                        string updateQuery = "UPDATE CurrentStock SET QtyLose = QtyLose - @QtyLose WHERE ProductiD = @ProductID";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@QtyLose", totalQty);
                            updateCmd.Parameters.AddWithValue("@ProductID", productId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insert new record (negative quantity as we're reducing stock)
                        string insertQuery = "INSERT INTO CurrentStock (ProductiD, QtyLose) VALUES (@ProductID, @QtyLose)";
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@ProductID", productId);
                            insertCmd.Parameters.AddWithValue("@QtyLose", -totalQty);
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

       
        private int GetPackingId(string packingName)
        {
            if (string.IsNullOrEmpty(packingName))
                return 1; // Default packing ID

            foreach (DataRow row in packingData.Rows)
            {
                if (row["PackingName"].ToString() == packingName)
                    return Convert.ToInt32(row["PackingId"]);
            }
            return 1; // Default packing ID
        }
        // ============= CLEAR FORM =============
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all fields
            billpurchasetxtbox.Clear();
            vehiclenotxt.Clear();
            vendoridtxtbox.Clear();
            vendornametxtbox.Clear();
            citytxtbox.Clear();
            narrationtxtbox.Clear();

            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnamecombobox.SelectedIndex = -1;
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            purpricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            salepricetxtbox.Clear();
            salestaxtxtbox.Clear();
            ratedifftxtbox.Clear();
            currentstocltxtbox.Clear();
            multipliertxtbox.Clear();

            salebilltxtbox.Clear();
            customeridtxtbox.Clear();
            customernametxtbox.Clear();
            towntxtbox.Clear();
            sectortxtbox.Clear();
            salesmanidtxtbox.Clear();
            salesmannametxtbox.Clear();

            alenetvaluetxtbox.Clear();
            prevbaltxtbox.Clear();
            specialdistxtbox.Clear();
            receivedtxtbox.Clear();
            finalbaltxtbox.Clear();

            totalgrosstxtbox.Clear();
            totaldiscounttxtbox.Clear();
            netvaluetxtbox.Clear();
            totalpurvaluetxtbox.Clear();
            topsalestaxtxtbox.Clear();
            paidamounttxtbox.Clear();
            totalratedifftxtbox.Clear();
            totalsalevaluetxtbox.Clear();

            // Clear DataGridView
            dataGridView1.Rows.Clear();

            // Reset dates
            dateTimePicker1.Value = DateTime.Now;
            saledatepcker.Value = DateTime.Now;
            //returndatepcker.Value = DateTime.Now;

            // Set focus to first field
            productidtxtbox.Focus();
        }

        // ============= CLOSE FORM =============
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ============= FORM CLOSING =============
        private void Purchase_Direct_to_Sale_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        // ============= AUTO CALCULATION EVENTS =============
        private void purpricetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateDiscountValue();
            CalculateRateDifference();
        }

        private void salepricetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRateDifference();
        }

        private void discpercenttxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateDiscountValue();
        }

        // ============= EMPTY EVENT HANDLERS (Keep these for Designer compatibility) =============
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void returndatepcker_ValueChanged(object sender, EventArgs e) { }
        private void billlnolbl_Click(object sender, EventArgs e) { }
        private void orderidlbl_Click(object sender, EventArgs e) { }
        private void orderdatelbl_Click(object sender, EventArgs e) { }
        private void billnotxtbox_TextChanged(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void vehiclenotxt_TextChanged(object sender, EventArgs e) { }
        private void netpurvaluetxt_TextChanged(object sender, EventArgs e) { }
        private void currentstocltxtbox_TextChanged(object sender, EventArgs e) { }
        private void netsalevaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
        private void totalgrosstxtbox_TextChanged(object sender, EventArgs e) { }
        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void netvaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void totalpurvaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void topsalestaxtxtbox_TextChanged(object sender, EventArgs e) { }
        private void paidamounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void totalsalevaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void purchasedtxtbox_TextChanged(object sender, EventArgs e) { }
        private void billpurchasetxtbox_TextChanged(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void citytxtbox_TextChanged(object sender, EventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyltxtbox_TextChanged(object sender, EventArgs e) { }
        private void bnsltxtbox_TextChanged(object sender, EventArgs e) { }
        private void discvalltxtbox_TextChanged(object sender, EventArgs e) { }
        private void salestaxtxtbox_TextChanged(object sender, EventArgs e) { }
        private void ratedifftxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
        private void saledatepcker_ValueChanged(object sender, EventArgs e) { }
        private void salebilltxtbox_TextChanged(object sender, EventArgs e) { }
        private void towntxtbox_TextChanged(object sender, EventArgs e) { }
        private void sectortxtbox_TextChanged(object sender, EventArgs e) { }
        private void alenetvaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void finalbaltxtbox_TextChanged(object sender, EventArgs e) { }
        private void vendornametxtbox_TextChanged(object sender, EventArgs e) { }
        private void totalratedifftxtbox_TextChanged(object sender, EventArgs e) { }

        private void multipliertxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
