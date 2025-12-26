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
    public partial class PurchaseInvoice : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable productsDataTable;
        private decimal grossTotal = 0;
        private decimal discountTotal = 0;
        private decimal salesTaxTotal = 0;
        private decimal netTotal = 0;
        private decimal paidAmount = 0;
        private bool isEditing = false;
        private int currentPurchaseId = 0;
        private DataTable packingsTable;
        private bool isLoadingPurchase = false;

        // Constructor for new purchase
        public PurchaseInvoice()
        {
            InitializeComponent();
            InitializeForm();
        }

        // Constructor for editing existing purchase
        public PurchaseInvoice(int purchaseId)
        {
            InitializeComponent();
            InitializeFormForEdit(purchaseId);
        }

        private void InitializeForm()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = null;

            InitializeDatabase();
            InitializeDataGridView();
            AutoGeneratePurchaseID();
            SetCurrentDate();
            dayshftradiobtn.Checked = true;
            LoadPackingsData();

            // Events attach karo
            SetupTextBoxEvents();
        }

        private void InitializeFormForEdit(int purchaseId)
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = null;

            InitializeDatabase();
            InitializeDataGridView();

            currentPurchaseId = purchaseId;
            isEditing = true;
            purchaseidtxtbox.Text = purchaseId.ToString();
            purchaseidtxtbox.ReadOnly = true;

            SetCurrentDate();
            dayshftradiobtn.Checked = true;
            LoadPackingsData();

            // Events attach karo
            SetupTextBoxEvents();

            // Load the existing purchase
            LoadExistingPurchase(purchaseId);
        }

        private void SetupTextBoxEvents()
        {
            productidtxtbox.KeyDown += productidtxtbox_KeyDown;
            qtyptxtbox.KeyDown += QuantityTextBox_KeyDown;
            qtyltxtbox.KeyDown += QuantityTextBox_KeyDown;
            bnsltxtbox.KeyDown += QuantityTextBox_KeyDown;
        }

        private void InitializeDatabase()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPackingsData()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                packingsTable = new DataTable();
                da.Fill(packingsTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridView()
        {
            productsDataTable = new DataTable();
            productsDataTable.Columns.Add("Pro ID", typeof(string));
            productsDataTable.Columns.Add("Product Name", typeof(string));
            productsDataTable.Columns.Add("Pack Name", typeof(string));
            productsDataTable.Columns.Add("Pack", typeof(int));
            productsDataTable.Columns.Add("Qty(P)", typeof(int));
            productsDataTable.Columns.Add("Qty(L)", typeof(int));
            productsDataTable.Columns.Add("Bns(L)", typeof(int));
            productsDataTable.Columns.Add("Price", typeof(decimal));
            productsDataTable.Columns.Add("Disc%", typeof(decimal));
            productsDataTable.Columns.Add("Disc VAl-L", typeof(decimal));
            productsDataTable.Columns.Add("Disc.Val", typeof(decimal));
            productsDataTable.Columns.Add("Sales Tax", typeof(decimal));
            productsDataTable.Columns.Add("Net Value", typeof(decimal));

            dataGridView1.DataSource = productsDataTable;
        }

        private void AutoGeneratePurchaseID()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(PurchaseID), 0) + 1 FROM Purchase";
                SqlCommand cmd = new SqlCommand(query, connection);
                object result = cmd.ExecuteScalar();
                currentPurchaseId = Convert.ToInt32(result);
                purchaseidtxtbox.Text = currentPurchaseId.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Purchase ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetCurrentDate()
        {
            purchasedatepcker.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        private void LoadExistingPurchase(int purchaseId)
        {
            try
            {
                isLoadingPurchase = true;

                // Clear existing data
                productsDataTable.Clear();
                ClearFormWithoutPurchaseID();

                // 1. Load Purchase header
                string purchaseQuery = @"SELECT * FROM Purchase WHERE PurchaseID = @PurchaseID";
                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection);
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", purchaseId);

                // Store vendor ID before closing reader
                string vendorId = "";

                using (SqlDataReader purchaseReader = purchaseCmd.ExecuteReader())
                {
                    if (purchaseReader.Read())
                    {
                        // Load basic information
                        purchaseidtxtbox.Text = purchaseReader["PurchaseID"].ToString();
                        purchasedatepcker.Value = Convert.ToDateTime(purchaseReader["PurchaseDate"]);
                        vendorId = purchaseReader["VendorID"].ToString();
                        vendoridtxtbox.Text = vendorId;

                        // Other fields - get values before closing reader
                        billnootxtbox.Text = purchaseReader["BillNo"] != DBNull.Value ? purchaseReader["BillNo"].ToString() : "";
                        dateTimePicker2.Value = purchaseReader["BillDate"] != DBNull.Value ?
                            Convert.ToDateTime(purchaseReader["BillDate"]) : DateTime.Now;
                        narrationtxtbox.Text = purchaseReader["Narration"] != DBNull.Value ?
                            purchaseReader["Narration"].ToString() : "";
                        specialdisctxtbox.Text = purchaseReader["SpecialDiscount"] != DBNull.Value ?
                            Convert.ToDecimal(purchaseReader["SpecialDiscount"]).ToString("N2") : "0.00";
                        paidtxtbox.Text = purchaseReader["PaidAmount"] != DBNull.Value ?
                            Convert.ToDecimal(purchaseReader["PaidAmount"]).ToString("N2") : "0.00";

                        if (purchaseReader["DueDate"] != DBNull.Value)
                        {
                            duedatetxtbox.Text = Convert.ToDateTime(purchaseReader["DueDate"]).ToString("yyyy-MM-dd");
                        }

                        pendignchkbox.Checked = Convert.ToBoolean(purchaseReader["IsPendingRate"]);
                        nightshftradiobtn.Checked = Convert.ToBoolean(purchaseReader["IsNightShift"]);
                        dayshftradiobtn.Checked = !nightshftradiobtn.Checked;
                    }
                    else
                    {
                        purchaseReader.Close();
                        MessageBox.Show($"Purchase #{purchaseId} not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        isEditing = false;
                        purchaseidtxtbox.ReadOnly = false;
                        AutoGeneratePurchaseID();
                        isLoadingPurchase = false;
                        return;
                    }
                } // Reader automatically closes here due to using statement

                // Now load vendor name (after reader is closed)
                LoadVendorName(vendorId);

                // 2. Load Purchase body items
                LoadPurchaseBodyItems(purchaseId);

                // 3. Calculate totals
                CalculateTotals();

                isLoadingPurchase = false;
            }
            catch (Exception ex)
            {
                isLoadingPurchase = false;
                MessageBox.Show("Error loading purchase: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPurchaseBodyItems(int purchaseId)
        {
            try
            {
                string bodyQuery = @"SELECT pb.*, p.ProductName, pack.PackingName 
                               FROM PurchasesBody pb 
                               INNER JOIN Products p ON pb.ProductId = p.ProductID
                               LEFT JOIN Packings pack ON pb.PackingId = pack.PackingId
                               WHERE pb.PurchaseId = @PurchaseId AND pb.IsDeleted = 0
                               ORDER BY pb.ProductId";

                SqlCommand bodyCmd = new SqlCommand(bodyQuery, connection);
                bodyCmd.Parameters.AddWithValue("@PurchaseId", purchaseId);

                using (SqlDataReader bodyReader = bodyCmd.ExecuteReader())
                {
                    while (bodyReader.Read())
                    {
                        DataRow newRow = productsDataTable.NewRow();

                        newRow["Pro ID"] = bodyReader["ProductId"].ToString();
                        newRow["Product Name"] = bodyReader["ProductName"].ToString();
                        newRow["Pack Name"] = bodyReader["PackingName"] != DBNull.Value ?
                            bodyReader["PackingName"].ToString() : "Piece";
                        newRow["Pack"] = Convert.ToInt32(bodyReader["Multiplier"]);
                        newRow["Qty(P)"] = Convert.ToInt32(bodyReader["QtyPack"]);
                        newRow["Qty(L)"] = Convert.ToInt32(bodyReader["QtyLose"]);
                        newRow["Bns(L)"] = Convert.ToInt32(bodyReader["BnsLose"]);
                        newRow["Price"] = Convert.ToDecimal(bodyReader["PricePack"]);
                        newRow["Disc%"] = Convert.ToDecimal(bodyReader["DiscRatio"]);
                        newRow["Disc VAl-L"] = Convert.ToDecimal(bodyReader["DiscValLose"]);
                        newRow["Disc.Val"] = Convert.ToDecimal(bodyReader["DiscValPack"]);
                        newRow["Sales Tax"] = Convert.ToDecimal(bodyReader["STaxRatio"]);
                        newRow["Net Value"] = Convert.ToDecimal(bodyReader["TTLValue"]);

                        productsDataTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase items: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVendorName(string vendorId)
        {
            try
            {
                // Close any open DataReader first
                string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PartyID", vendorId);

                    // Use ExecuteScalar instead of DataReader
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        vendornametxtbox.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ============================ KEY DOWN EVENTS ============================
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchProductInTextboxes();
                e.Handled = true;
            }
        }

        private void QuantityTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Add product to grid when Enter is pressed in any quantity field
                AddProductToGrid();
                e.Handled = true;
            }
        }

        private void vendoridtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchVendor();
                e.Handled = true;
            }
        }

        // ============================ VENDOR SEARCH ============================
        private void vendoridsearch_Click_1(object sender, EventArgs e)
        {
            SearchVendor();
        }

        private void SearchVendor()
        {
            try
            {
                string vendorId = vendoridtxtbox.Text.Trim();
                if (string.IsNullOrEmpty(vendorId))
                {
                    MessageBox.Show("Please enter Vendor ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"SELECT PartyID, PartyName, Address, City, Phone1, CreditLimit 
                                FROM Parties 
                                WHERE PartyID = @PartyID AND PartyType = 'V'";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PartyID", vendorId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vendornametxtbox.Text = reader["PartyName"].ToString();
                        productidtxtbox.Focus(); // Move focus to product ID
                    }
                    else
                    {
                        MessageBox.Show("Vendor not found or not of type 'V'!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        vendornametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching vendor: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ PRODUCT SEARCH ============================
        private void SearchProductInTextboxes()
        {
            try
            {
                string productId = productidtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(productId))
                {
                    MessageBox.Show("Please enter Product ID",
                                  "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    productidtxtbox.Focus();
                    productidtxtbox.SelectAll();
                    return;
                }

                string query = @"SELECT p.ProductID, p.ProductName, p.PurchasePrice, 
                                p.PurDiscRatio, pp.PackingId, pp.Multiplier,
                                pack.PackingName
                         FROM Products p
                         LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                         LEFT JOIN Packings pack ON pp.PackingId = pack.PackingId
                         WHERE p.ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // 1. Product Name
                            productnametxtbox.Text = reader["ProductName"].ToString();

                            // 2. Purchase Price
                            decimal purchasePrice = Convert.ToDecimal(reader["PurchasePrice"]);
                            pricetxtbox.Text = purchasePrice.ToString("N2");

                            // 3. Discount %
                            decimal discPercent = Convert.ToDecimal(reader["PurDiscRatio"]);
                            discpercenttxtbox.Text = discPercent.ToString("N2");

                            // 4. Packing Name
                            if (reader["PackingName"] != DBNull.Value)
                            {
                                packnametxtbox.Text = reader["PackingName"].ToString();
                            }
                            else
                            {
                                packnametxtbox.Text = "Piece";
                            }

                            // 5. Multiplier
                            if (reader["Multiplier"] != DBNull.Value)
                            {
                                textBox4.Text = Convert.ToInt32(reader["Multiplier"]).ToString();
                            }
                            else
                            {
                                textBox4.Text = "1";
                            }

                            // 6. Set default quantities
                            qtyptxtbox.Text = "1"; // Default 1 pack
                            qtyltxtbox.Text = "0";
                            bnsltxtbox.Text = "0";

                            // 7. Clear calculated fields
                            discvalltxtbox.Clear();
                            discvaltxtbox.Clear();
                            salestaxtxtbox.Clear();
                            valuetxtbox.Clear();

                            // 8. Auto-calculate for default quantity
                            CalculateRowTotal();

                            // 9. Move focus to Qty(P)
                            qtyptxtbox.Focus();
                            qtyptxtbox.SelectAll();
                        }
                        else
                        {
                            MessageBox.Show($"Product ID '{productId}' not found in database!",
                                          "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            ClearProductTextboxes();
                            productidtxtbox.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearProductTextboxes()
        {
            productnametxtbox.Clear();
            packnametxtbox.Clear();
            textBox4.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            pricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            discvaltxtbox.Clear();
            salestaxtxtbox.Clear();
            valuetxtbox.Clear();
        }

        // ============================ ADD PRODUCT TO GRID ============================
        private void AddProductToGridButton_Click(object sender, EventArgs e)
        {
            AddProductToGrid();
        }

        private void AddProductToGrid()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    productidtxtbox.Focus();
                    return;
                }

                // Check if at least one quantity is entered
                int qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);

                if (qtyP == 0 && qtyL == 0)
                {
                    MessageBox.Show("Please enter at least one quantity", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    qtyptxtbox.Focus();
                    return;
                }

                // Get values from textboxes
                string proId = productidtxtbox.Text;
                string productName = productnametxtbox.Text;
                string packName = packnametxtbox.Text;
                int pack = string.IsNullOrEmpty(textBox4.Text) ? 1 : Convert.ToInt32(textBox4.Text);
                int bnsL = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
                decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
                decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
                decimal discValL = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : Convert.ToDecimal(discvalltxtbox.Text);
                decimal discVal = string.IsNullOrEmpty(discvaltxtbox.Text) ? 0 : Convert.ToDecimal(discvaltxtbox.Text);
                decimal salesTax = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
                decimal netValue = string.IsNullOrEmpty(valuetxtbox.Text) ? 0 : Convert.ToDecimal(valuetxtbox.Text);

                // Calculate net value if not provided
                if (netValue == 0)
                {
                    // Calculate total quantity in lose units
                    int totalLoseUnits = (qtyP * pack) + qtyL + bnsL;

                    // Calculate gross value
                    decimal grossValue = totalLoseUnits * price;

                    // Calculate discount
                    decimal calculatedDiscVal = grossValue * discPercent / 100;
                    decimal calculatedDiscValL = totalLoseUnits > 0 ? calculatedDiscVal / totalLoseUnits : 0;

                    // Calculate net before tax
                    decimal netBeforeTax = grossValue - calculatedDiscVal;

                    // Calculate sales tax
                    decimal calculatedSalesTax = netBeforeTax * salesTax / 100;

                    // Calculate final net value
                    netValue = netBeforeTax + calculatedSalesTax;

                    // Update textboxes with calculated values
                    discvalltxtbox.Text = calculatedDiscValL.ToString("N6");
                    discvaltxtbox.Text = calculatedDiscVal.ToString("N2");
                    salestaxtxtbox.Text = calculatedSalesTax.ToString("N2");
                    valuetxtbox.Text = netValue.ToString("N2");

                    // Update the variables
                    discValL = calculatedDiscValL;
                    discVal = calculatedDiscVal;
                    salesTax = calculatedSalesTax;
                }

                // Check if product already exists in grid
                bool productExists = false;
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row["Pro ID"].ToString() == proId && row["Pack"].ToString() == pack.ToString())
                    {
                        // Update existing row
                        row["Qty(P)"] = Convert.ToInt32(row["Qty(P)"]) + qtyP;
                        row["Qty(L)"] = Convert.ToInt32(row["Qty(L)"]) + qtyL;
                        row["Bns(L)"] = Convert.ToInt32(row["Bns(L)"]) + bnsL;
                        row["Net Value"] = Convert.ToDecimal(row["Net Value"]) + netValue;
                        row["Disc.Val"] = Convert.ToDecimal(row["Disc.Val"]) + discVal;
                        row["Sales Tax"] = Convert.ToDecimal(row["Sales Tax"]) + salesTax;
                        productExists = true;
                        break;
                    }
                }

                if (!productExists)
                {
                    // Add new row to DataTable
                    DataRow newRow = productsDataTable.NewRow();
                    newRow["Pro ID"] = proId;
                    newRow["Product Name"] = productName;
                    newRow["Pack Name"] = packName;
                    newRow["Pack"] = pack;
                    newRow["Qty(P)"] = qtyP;
                    newRow["Qty(L)"] = qtyL;
                    newRow["Bns(L)"] = bnsL;
                    newRow["Price"] = price;
                    newRow["Disc%"] = discPercent;
                    newRow["Disc VAl-L"] = discValL;
                    newRow["Disc.Val"] = discVal;
                    newRow["Sales Tax"] = salesTax;
                    newRow["Net Value"] = netValue;

                    productsDataTable.Rows.Add(newRow);
                }

                // Clear textboxes for next entry
                ClearProductTextboxes();
                productidtxtbox.Focus();

                // Calculate totals
                CalculateTotals();

                // Show success message
                MessageBox.Show("Product added to grid successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values for quantities and prices", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ CALCULATIONS ============================
        private void CalculateRowTotal()
        {
            try
            {
                // Get values from textboxes
                decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
                int qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
                int bnsL = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
                decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
                decimal salesTaxPercent = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
                int packMultiplier = string.IsNullOrEmpty(textBox4.Text) ? 1 : Convert.ToInt32(textBox4.Text);

                // Calculate total quantity in lose units
                int totalLoseUnits = (qtyP * packMultiplier) + qtyL + bnsL;

                if (totalLoseUnits <= 0)
                {
                    // Agar quantity 0 hai to clear calculated fields
                    discvalltxtbox.Text = "0.000000";
                    discvaltxtbox.Text = "0.00";
                    salestaxtxtbox.Text = "0.00";
                    valuetxtbox.Text = "0.00";
                    return;
                }

                // Calculate gross value
                decimal grossValue = totalLoseUnits * price;

                // Calculate discount
                decimal discValue = grossValue * discPercent / 100;
                decimal discPerLoseUnit = discValue / totalLoseUnits;

                // Calculate net before tax
                decimal netBeforeTax = grossValue - discValue;

                // Calculate sales tax
                decimal salesTax = netBeforeTax * salesTaxPercent / 100;

                // Calculate final net value
                decimal finalNetValue = netBeforeTax + salesTax;

                // Update textboxes with calculated values
                discvalltxtbox.Text = discPerLoseUnit.ToString("N6");
                discvaltxtbox.Text = discValue.ToString("N2");
                salestaxtxtbox.Text = salesTax.ToString("N2");
                valuetxtbox.Text = finalNetValue.ToString("N2");
            }
            catch (Exception ex)
            {
                // Don't show error, just log it
                Console.WriteLine($"CalculateRowTotal Error: {ex.Message}");
            }
        }

        private void CalculateTotals()
        {
            try
            {
                grossTotal = 0;
                discountTotal = 0;
                salesTaxTotal = 0;
                netTotal = 0;

                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        decimal netValue = row["Net Value"] != DBNull.Value ? Convert.ToDecimal(row["Net Value"]) : 0;
                        decimal discVal = row["Disc.Val"] != DBNull.Value ? Convert.ToDecimal(row["Disc.Val"]) : 0;
                        decimal salesTax = row["Sales Tax"] != DBNull.Value ? Convert.ToDecimal(row["Sales Tax"]) : 0;

                        grossTotal += netValue + discVal;
                        discountTotal += discVal;
                        salesTaxTotal += salesTax;
                        netTotal += netValue;
                    }
                }

                // Apply special discount
                decimal specialDisc = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text);
                netTotal -= specialDisc;

                // Get paid amount
                paidAmount = string.IsNullOrEmpty(paidtxtbox.Text) ? 0 : Convert.ToDecimal(paidtxtbox.Text);

                // Update text boxes
                totalgrosstxtbox.Text = grossTotal.ToString("N2");
                totaldiscounttxtbox.Text = discountTotal.ToString("N2");
                topsalestxtbox.Text = salesTaxTotal.ToString("N2");
                netvaluetxtbox.Text = netTotal.ToString("N2");
                paidtxtbox.Text = paidAmount.ToString("N2");

                // Calculate remaining balance
                decimal remainingBalance = netTotal - paidAmount;
                remainingtxtbox.Text = remainingBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating totals: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ TEXT CHANGE EVENTS ============================
        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void bnsltxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void pricetxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void discpercenttxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void salestaxtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateRowTotal();
        }

        private void paidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateTotals();
        }

        private void specialdisctxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingPurchase)
                CalculateTotals();
        }

        // ============================ GRID EVENTS ============================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // Load data from grid to textboxes
                    productidtxtbox.Text = row.Cells["Pro ID"].Value?.ToString() ?? "";
                    productnametxtbox.Text = row.Cells["Product Name"].Value?.ToString() ?? "";
                    packnametxtbox.Text = row.Cells["Pack Name"].Value?.ToString() ?? "";
                    textBox4.Text = row.Cells["Pack"].Value?.ToString() ?? "1";
                    qtyptxtbox.Text = row.Cells["Qty(P)"].Value?.ToString() ?? "0";
                    qtyltxtbox.Text = row.Cells["Qty(L)"].Value?.ToString() ?? "0";
                    bnsltxtbox.Text = row.Cells["Bns(L)"].Value?.ToString() ?? "0";
                    pricetxtbox.Text = row.Cells["Price"].Value?.ToString() ?? "0";
                    discpercenttxtbox.Text = row.Cells["Disc%"].Value?.ToString() ?? "0";
                    discvalltxtbox.Text = row.Cells["Disc VAl-L"].Value?.ToString() ?? "0";
                    discvaltxtbox.Text = row.Cells["Disc.Val"].Value?.ToString() ?? "0";
                    salestaxtxtbox.Text = row.Cells["Sales Tax"].Value?.ToString() ?? "0";
                    valuetxtbox.Text = row.Cells["Net Value"].Value?.ToString() ?? "0";

                    // Focus on product ID for editing
                    productidtxtbox.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading row data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ============================ SAVE/UPDATE ============================
        private void savebtn_Click_1(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                if (isEditing)
                {
                    UpdatePurchaseInvoice();
                }
                else
                {
                    SavePurchaseInvoice();
                }
            }
        }

        private bool ValidateForm()
        {
            // Validate Vendor
            if (string.IsNullOrEmpty(vendoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Vendor ID!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendoridtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(vendornametxtbox.Text))
            {
                MessageBox.Show("Please select a valid vendor!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate Products
            if (productsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SavePurchaseInvoice()
        {
            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();

                // Get packing IDs
                Dictionary<string, int> packingNameToId = new Dictionary<string, int>();
                if (packingsTable != null)
                {
                    foreach (DataRow row in packingsTable.Rows)
                    {
                        packingNameToId[row["PackingName"].ToString()] = Convert.ToInt32(row["PackingId"]);
                    }
                }

                // 1. Save to Purchase table
                string purchaseQuery = @"INSERT INTO Purchase (PurchaseID, PurchaseDate, VendorID, BillNo, BillDate, 
                                       IsUploaded, PaidAmount, Narration, SpecialDiscount, DueDate, UserNo, 
                                       IsPendingRate, IsNightShift)
                                       VALUES (@PurchaseID, @PurchaseDate, @VendorID, @BillNo, @BillDate, 
                                       0, @PaidAmount, @Narration, @SpecialDiscount, @DueDate, @UserNo, 
                                       @IsPendingRate, @IsNightShift)";

                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);

                // Add parameters
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", currentPurchaseId);
                purchaseCmd.Parameters.AddWithValue("@PurchaseDate", purchasedatepcker.Value);
                purchaseCmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillNo",
                    string.IsNullOrEmpty(billnootxtbox.Text) ? (object)DBNull.Value : billnootxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillDate",
                    dateTimePicker2.Value.Date == DateTime.Now.Date ? (object)DBNull.Value : dateTimePicker2.Value);
                purchaseCmd.Parameters.AddWithValue("@PaidAmount",
                    string.IsNullOrEmpty(paidtxtbox.Text) ? 0 : Convert.ToDecimal(paidtxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@Narration",
                    string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@SpecialDiscount",
                    string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@DueDate",
                    string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@UserNo", 1);
                purchaseCmd.Parameters.AddWithValue("@IsPendingRate", pendignchkbox.Checked);
                purchaseCmd.Parameters.AddWithValue("@IsNightShift", nightshftradiobtn.Checked);

                purchaseCmd.ExecuteNonQuery();

                // 2. Save to PurchasesBody table
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        string proId = row["Pro ID"].ToString();
                        if (!string.IsNullOrEmpty(proId))
                        {
                            string purchasesBodyQuery = @"INSERT INTO PurchasesBody (PurchaseId, ProductId, PackingId, Multiplier, QtyPack, 
                                                        QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, DiscValLose, OfferDiscount, 
                                                        STaxRatio, STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, 
                                                        TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, IsDeleted)
                                                        VALUES (@PurchaseId, @ProductId, @PackingId, @Multiplier, @QtyPack, 
                                                        @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @OfferDiscount, 
                                                        @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, 
                                                        @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @IsDeleted)";

                            SqlCommand bodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);

                            // Get packing ID
                            int packingId = 1;
                            string packName = row["Pack Name"].ToString();
                            if (!string.IsNullOrEmpty(packName) && packingNameToId.ContainsKey(packName))
                            {
                                packingId = packingNameToId[packName];
                            }

                            // Calculate values
                            decimal netValue = row["Net Value"] != DBNull.Value ? Convert.ToDecimal(row["Net Value"]) : 0;
                            decimal salesTax = row["Sales Tax"] != DBNull.Value ? Convert.ToDecimal(row["Sales Tax"]) : 0;
                            decimal ttlSTax = salesTax;
                            decimal ttlFTax = 0;

                            // Add parameters
                            bodyCmd.Parameters.AddWithValue("@PurchaseId", currentPurchaseId);
                            bodyCmd.Parameters.AddWithValue("@ProductId", proId);
                            bodyCmd.Parameters.AddWithValue("@PackingId", packingId);
                            bodyCmd.Parameters.AddWithValue("@Multiplier", row["Pack"] != DBNull.Value ? Convert.ToInt32(row["Pack"]) : 1);
                            bodyCmd.Parameters.AddWithValue("@QtyPack", row["Qty(P)"] != DBNull.Value ? Convert.ToInt32(row["Qty(P)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@QtyLose", row["Qty(L)"] != DBNull.Value ? Convert.ToInt32(row["Qty(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@BnsLose", row["Bns(L)"] != DBNull.Value ? Convert.ToInt32(row["Bns(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@PricePack", row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscRatio", row["Disc%"] != DBNull.Value ? Convert.ToDecimal(row["Disc%"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscValPack", row["Disc.Val"] != DBNull.Value ? Convert.ToDecimal(row["Disc.Val"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscValLose", row["Disc VAl-L"] != DBNull.Value ? Convert.ToDecimal(row["Disc VAl-L"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@OfferDiscount", 0);
                            bodyCmd.Parameters.AddWithValue("@STaxRatio", row["Sales Tax"] != DBNull.Value ? Convert.ToDecimal(row["Sales Tax"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@STaxValPack", 0);
                            bodyCmd.Parameters.AddWithValue("@IsValueB4Ratio", true);
                            bodyCmd.Parameters.AddWithValue("@IsSTaxOnBns", false);
                            bodyCmd.Parameters.AddWithValue("@IsFTaxExempted", false);
                            bodyCmd.Parameters.AddWithValue("@TTLValue", netValue);
                            bodyCmd.Parameters.AddWithValue("@TTLSTax", ttlSTax);
                            bodyCmd.Parameters.AddWithValue("@TTLFTax", ttlFTax);
                            bodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                            bodyCmd.Parameters.AddWithValue("@IsFreeProduct", false);
                            bodyCmd.Parameters.AddWithValue("@IsDeleted", false);

                            bodyCmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show($"Purchase invoice saved successfully!\nInvoice ID: {currentPurchaseId}\nVendor: {vendornametxtbox.Text}\nTotal Amount: {netTotal:N2}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form for new entry
                isEditing = false;
                purchaseidtxtbox.ReadOnly = false;
                ClearForm();
                AutoGeneratePurchaseID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving purchase invoice: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePurchaseInvoice()
        {
            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();

                // Get packing IDs
                Dictionary<string, int> packingNameToId = new Dictionary<string, int>();
                if (packingsTable != null)
                {
                    foreach (DataRow row in packingsTable.Rows)
                    {
                        packingNameToId[row["PackingName"].ToString()] = Convert.ToInt32(row["PackingId"]);
                    }
                }

                // 1. Update Purchase table
                string purchaseQuery = @"UPDATE Purchase SET 
                                PurchaseDate = @PurchaseDate,
                                VendorID = @VendorID,
                                BillNo = @BillNo,
                                BillDate = @BillDate,
                                PaidAmount = @PaidAmount,
                                Narration = @Narration,
                                SpecialDiscount = @SpecialDiscount,
                                DueDate = @DueDate,
                                UserNo = @UserNo,
                                IsPendingRate = @IsPendingRate,
                                IsNightShift = @IsNightShift
                                WHERE PurchaseID = @PurchaseID";

                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);

                // Add parameters
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", currentPurchaseId);
                purchaseCmd.Parameters.AddWithValue("@PurchaseDate", purchasedatepcker.Value);
                purchaseCmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillNo",
                    string.IsNullOrEmpty(billnootxtbox.Text) ? (object)DBNull.Value : billnootxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillDate",
                    dateTimePicker2.Value.Date == DateTime.Now.Date ? (object)DBNull.Value : dateTimePicker2.Value);
                purchaseCmd.Parameters.AddWithValue("@PaidAmount",
                    string.IsNullOrEmpty(paidtxtbox.Text) ? 0 : Convert.ToDecimal(paidtxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@Narration",
                    string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@SpecialDiscount",
                    string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@DueDate",
                    string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@UserNo", 1);
                purchaseCmd.Parameters.AddWithValue("@IsPendingRate", pendignchkbox.Checked);
                purchaseCmd.Parameters.AddWithValue("@IsNightShift", nightshftradiobtn.Checked);

                purchaseCmd.ExecuteNonQuery();

                // 2. Delete existing items first (use DELETE instead of soft delete)
                string deleteQuery = @"DELETE FROM PurchasesBody WHERE PurchaseId = @PurchaseId";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction);
                deleteCmd.Parameters.AddWithValue("@PurchaseId", currentPurchaseId);
                deleteCmd.ExecuteNonQuery();

                // 3. Insert updated items with new serial numbers
                int serialCounter = 1;
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        string proId = row["Pro ID"].ToString();
                        if (!string.IsNullOrEmpty(proId))
                        {
                            string purchasesBodyQuery = @"INSERT INTO PurchasesBody 
                                                (PurchaseId, ProductId, PackingId, Multiplier, QtyPack, 
                                                QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, DiscValLose, 
                                                STaxRatio, STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, 
                                                TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, 
                                                PerUnitCostLose, IsFreeProduct, IsDeleted)
                                                VALUES 
                                                (@PurchaseId, @ProductId, @PackingId, @Multiplier, @QtyPack, 
                                                @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, 
                                                @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, 
                                                @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, 
                                                @PerUnitCostLose, @IsFreeProduct, @IsDeleted)";

                            SqlCommand bodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);

                            // Get packing ID
                            int packingId = 1;
                            string packName = row["Pack Name"].ToString();
                            if (!string.IsNullOrEmpty(packName) && packingNameToId.ContainsKey(packName))
                            {
                                packingId = packingNameToId[packName];
                            }

                            // Calculate values
                            decimal netValue = row["Net Value"] != DBNull.Value ? Convert.ToDecimal(row["Net Value"]) : 0;
                            decimal salesTax = row["Sales Tax"] != DBNull.Value ? Convert.ToDecimal(row["Sales Tax"]) : 0;
                            decimal ttlSTax = salesTax;
                            decimal ttlFTax = 0;

                            // Add parameters
                            bodyCmd.Parameters.AddWithValue("@PurchaseId", currentPurchaseId);
                            bodyCmd.Parameters.AddWithValue("@ProductId", proId);
                            bodyCmd.Parameters.AddWithValue("@PackingId", packingId);
                            bodyCmd.Parameters.AddWithValue("@Multiplier", row["Pack"] != DBNull.Value ? Convert.ToInt32(row["Pack"]) : 1);
                            bodyCmd.Parameters.AddWithValue("@QtyPack", row["Qty(P)"] != DBNull.Value ? Convert.ToInt32(row["Qty(P)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@QtyLose", row["Qty(L)"] != DBNull.Value ? Convert.ToInt32(row["Qty(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@BnsLose", row["Bns(L)"] != DBNull.Value ? Convert.ToInt32(row["Bns(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@PricePack", row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscRatio", row["Disc%"] != DBNull.Value ? Convert.ToDecimal(row["Disc%"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscValPack", row["Disc.Val"] != DBNull.Value ? Convert.ToDecimal(row["Disc.Val"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@DiscValLose", row["Disc VAl-L"] != DBNull.Value ? Convert.ToDecimal(row["Disc VAl-L"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@STaxRatio", row["Sales Tax"] != DBNull.Value ? Convert.ToDecimal(row["Sales Tax"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@STaxValPack", 0);
                            bodyCmd.Parameters.AddWithValue("@IsValueB4Ratio", true);
                            bodyCmd.Parameters.AddWithValue("@IsSTaxOnBns", false);
                            bodyCmd.Parameters.AddWithValue("@IsFTaxExempted", false);
                            bodyCmd.Parameters.AddWithValue("@TTLValue", netValue);
                            bodyCmd.Parameters.AddWithValue("@TTLSTax", ttlSTax);
                            bodyCmd.Parameters.AddWithValue("@TTLFTax", ttlFTax);
                            bodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                            bodyCmd.Parameters.AddWithValue("@IsFreeProduct", false);
                            bodyCmd.Parameters.AddWithValue("@IsDeleted", false);

                            bodyCmd.ExecuteNonQuery();
                            serialCounter++;
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show($"Purchase invoice updated successfully!\nInvoice ID: {currentPurchaseId}\nVendor: {vendornametxtbox.Text}\nTotal Amount: {netTotal:N2}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error updating purchase invoice: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ REMOVE ROW ============================
        private void removebtn_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                }
                CalculateTotals();
            }
            else if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                CalculateTotals();
            }
            else
            {
                MessageBox.Show("Please select row(s) to remove", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ============================ CLEAR FORM ============================
        private void clearbtn_Click_1(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                // Clear vendor info
                vendoridtxtbox.Clear();
                vendornametxtbox.Clear();
                billnootxtbox.Clear();
                narrationtxtbox.Clear();
                duedatetxtbox.Clear();
                specialdisctxtbox.Clear();
                paidtxtbox.Clear();

                // Clear totals
                totalgrosstxtbox.Clear();
                totaldiscounttxtbox.Clear();
                topsalestxtbox.Clear();
                netvaluetxtbox.Clear();
                remainingtxtbox.Clear();

                // Clear product textboxes
                ClearProductTextboxes();

                // Clear grid
                productsDataTable.Clear();

                // Reset checkboxes and radio buttons
                pendignchkbox.Checked = false;
                showloaddocxhkbox.Checked = false;
                dayshftradiobtn.Checked = true;

                // Set current dates
                SetCurrentDate();

                // Generate new ID if not editing
                if (!isEditing)
                {
                    AutoGeneratePurchaseID();
                }

                // Focus on vendor ID
                vendoridtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFormWithoutPurchaseID()
        {
            try
            {
                // Clear vendor info
                vendoridtxtbox.Clear();
                vendornametxtbox.Clear();
                billnootxtbox.Clear();
                narrationtxtbox.Clear();
                duedatetxtbox.Clear();
                specialdisctxtbox.Clear();
                paidtxtbox.Clear();

                // Clear totals
                totalgrosstxtbox.Clear();
                totaldiscounttxtbox.Clear();
                topsalestxtbox.Clear();
                netvaluetxtbox.Clear();
                remainingtxtbox.Clear();

                // Clear product textboxes
                ClearProductTextboxes();

                // Clear grid
                productsDataTable.Clear();

                // Reset checkboxes and radio buttons
                pendignchkbox.Checked = false;
                showloaddocxhkbox.Checked = false;
                dayshftradiobtn.Checked = true;

                // Set current dates
                SetCurrentDate();

                // Focus on vendor ID
                vendoridtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ OTHER CONTROLS ============================
        private void openbtn_Click(object sender, EventArgs e)
        {
            SearchPurchaseInvoice searchForm = new SearchPurchaseInvoice();
            searchForm.Show();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing connection: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            base.OnFormClosing(e);
        }

        private void PurchaseInvoice_Load(object sender, EventArgs e)
        {
            // Focus on vendor ID when form loads
            vendoridtxtbox.Focus();
        }

        private void purchaseidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        // ============================ EMPTY EVENT HANDLERS ============================
        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void netvaluetxtbox_TextChanged(object sender, EventArgs e) { }
        private void topsalestxtbox_TextChanged(object sender, EventArgs e) { }
        private void remainingtxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void vendornametxtbox_TextChanged(object sender, EventArgs e) { }
        private void vendoridtxtbox_TextChanged(object sender, EventArgs e) { }
        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void billnootxtbox_TextChanged(object sender, EventArgs e) { }
        private void purchasedatepcker_ValueChanged(object sender, EventArgs e) { }
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) { }
        private void duedatetxtbox_TextChanged(object sender, EventArgs e) { }
        private void nightshftradiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void dayshftradiobtn_CheckedChanged(object sender, EventArgs e) { }
        private void showloaddocxhkbox_CheckedChanged(object sender, EventArgs e) { }
        private void productidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void packnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged_1(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void qtyltxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void bnsltxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void pricetxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void discpercenttxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void discvalltxtbox_TextChanged(object sender, EventArgs e) { }
        private void discvaltxtbox_TextChanged(object sender, EventArgs e) { }
        private void salestaxtxtbox_TextChanged_1(object sender, EventArgs e) { }
        private void valuetxtbox_TextChanged(object sender, EventArgs e) { }
    }
}