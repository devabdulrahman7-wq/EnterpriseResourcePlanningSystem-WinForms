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

        public PurchaseInvoice()
        {
            InitializeComponent();

            // YE DO LINES ADD KARO
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = null;

            InitializeDatabase();
            InitializeDataGridView();
            AutoGeneratePurchaseID();
            SetCurrentDate();
            dayshftradiobtn.Checked = true;
            LoadPackingsData();

            // Events attach karo
            //vendoridsearch.Click += vendoridsearch_Click_1;
            productidtxtbox.KeyDown += productidtxtbox_KeyDown;
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

        // ============================ VENDOR SEARCH ============================
        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            SearchVendor();
        }

        private void vendoridtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchVendor();
            }
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
                        MessageBox.Show($"Vendor Found!\nName: {reader["PartyName"]}\nAddress: {reader["Address"]}\nCity: {reader["City"]}\nPhone: {reader["Phone1"]}",
                            "Vendor Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // ============================ PRODUCT SEARCH IN TEXTBOXES ============================
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchProductInTextboxes();
            }
        }

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
                            // DEBUG: Show what we're fetching
                            Console.WriteLine($"Found Product: {reader["ProductName"]}");

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
                                Console.WriteLine($"Packing Name: {reader["PackingName"]}");
                            }
                            else
                            {
                                packnametxtbox.Text = "Piece";
                                Console.WriteLine("Packing Name: Piece (Default)");
                            }

                            // 5. Multiplier
                            if (reader["Multiplier"] != DBNull.Value)
                            {
                                textBox4.Text = Convert.ToInt32(reader["Multiplier"]).ToString();
                                Console.WriteLine($"Multiplier: {reader["Multiplier"]}");
                            }
                            else
                            {
                                textBox4.Text = "1";
                                Console.WriteLine("Multiplier: 1 (Default)");
                            }

                            // 6. Set default quantities
                            qtyptxtbox.Text = "0";
                            qtyltxtbox.Text = "1";
                            bnsltxtbox.Text = "0";

                            // 7. Clear calculated fields
                            discvalltxtbox.Clear();
                            discvaltxtbox.Clear();
                            salestaxtxtbox.Clear();
                            valuetxtbox.Clear();

                            // 8. Auto-calculate for 1 quantity
                            CalculateRowTotal();

                            // 9. Move focus to Qty(P)
                            qtyptxtbox.Focus();
                            qtyptxtbox.SelectAll();

                            // 10. Success message
                            MessageBox.Show($"Product '{reader["ProductName"]}' loaded successfully!",
                                          "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Product ID '{productId}' not found in database!",
                                          "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //ClearProductTextboxes();

                            productidtxtbox.Focus();
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid numeric Product ID",
                               "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}",
                               "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\nStack Trace: {ex.StackTrace}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void CalculateRowTotal()
        //{
        //    try
        //    {
        //        // Get values from textboxes
        //        decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
        //        int qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
        //        int qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
        //        int bnsL = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
        //        decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
        //        decimal salesTaxPercent = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
        //        int packMultiplier = string.IsNullOrEmpty(textBox4.Text) ? 1 : Convert.ToInt32(textBox4.Text);

        //        // Calculate total quantity in lose units
        //        int totalLoseUnits = (qtyP * packMultiplier) + qtyL + bnsL;
        //        if (totalLoseUnits == 0) return;

        //        // Calculate gross value
        //        decimal grossValue = totalLoseUnits * price;

        //        // Calculate discount
        //        decimal discValue = grossValue * discPercent / 100;
        //        decimal discPerLoseUnit = discValue / totalLoseUnits;

        //        // Calculate net before tax
        //        decimal netBeforeTax = grossValue - discValue;

        //        // Calculate sales tax
        //        decimal salesTax = netBeforeTax * salesTaxPercent / 100;

        //        // Calculate final net value
        //        decimal finalNetValue = netBeforeTax + salesTax;

        //        // Update textboxes with calculated values
        //        discvalltxtbox.Text = discPerLoseUnit.ToString("N6");
        //        discvaltxtbox.Text = discValue.ToString("N2");
        //        salestaxtxtbox.Text = salesTax.ToString("N2");
        //        valuetxtbox.Text = finalNetValue.ToString("N2");
        //    }
        //    catch (Exception)
        //    {
        //        // Silently handle calculation errors
        //    }
        //}
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
        private void AddProductToGrid()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(qtyptxtbox.Text) && string.IsNullOrEmpty(qtyltxtbox.Text))
                {
                    MessageBox.Show("Please enter at least one quantity", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get values from textboxes
                string proId = productidtxtbox.Text;
                string productName = productnametxtbox.Text;
                string packName = packnametxtbox.Text;
                int pack = string.IsNullOrEmpty(textBox4.Text) ? 1 : Convert.ToInt32(textBox4.Text);
                int qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
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
                }

                // Add to DataTable
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
                newRow["Disc VAl-L"] = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : Convert.ToDecimal(discvalltxtbox.Text);
                newRow["Disc.Val"] = string.IsNullOrEmpty(discvaltxtbox.Text) ? 0 : Convert.ToDecimal(discvaltxtbox.Text);
                newRow["Sales Tax"] = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
                newRow["Net Value"] = netValue;

                productsDataTable.Rows.Add(newRow);

                // Clear textboxes for next entry
                ClearProductTextboxes();
                productidtxtbox.Focus();

                // Calculate totals
                CalculateTotals();
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

                Console.WriteLine($"Calculated: Price={price}, Qty={totalLoseUnits}, Disc={discValue}, Net={finalNetValue}");
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

        // ============================ TEXTBOX CHANGE EVENTS FOR CALCULATIONS ============================
        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void bnsltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void pricetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void discpercenttxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        private void salestaxtxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRowTotal();
        }

        // ============================ GRID CLICK EVENT ============================
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

        // ============================ SAVE PURCHASE INVOICE ============================
        private void savebtn_Click_1(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SavePurchaseInvoice();
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
                string purchaseQuery = @"INSERT INTO Purchase (PurchaseID, PurchaseDate, OfficeID, VendorID, BillNo, BillDate, 
                                       IsUploaded, PaidAmount, Narration, SpecialDiscount, DueDate, UserNo, 
                                       IsPendingRate, IsNightShift)
                                       VALUES (@PurchaseID, @PurchaseDate, @OfficeID, @VendorID, @BillNo, @BillDate, 
                                       0, @PaidAmount, @Narration, @SpecialDiscount, @DueDate, @UserNo, 
                                       @IsPendingRate, @IsNightShift)";

                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);

                // Add parameters
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", currentPurchaseId);
                purchaseCmd.Parameters.AddWithValue("@PurchaseDate", purchasedatepcker.Value);
                purchaseCmd.Parameters.AddWithValue("@OfficeID", DBNull.Value);
                purchaseCmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnootxtbox.Text) ? (object)DBNull.Value : billnootxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillDate", dateTimePicker2.Value);
                purchaseCmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                purchaseCmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@SpecialDiscount", string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@DueDate", string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@UserNo", 1); // Default admin user
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
                                                        TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct)
                                                        VALUES (@PurchaseId, @ProductId, @PackingId, @Multiplier, @QtyPack, 
                                                        @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @OfferDiscount, 
                                                        @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, 
                                                        @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct)";

                            SqlCommand bodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);

                            // Get packing ID
                            int packingId = 1;
                            string packName = row["Pack Name"].ToString();
                            if (!string.IsNullOrEmpty(packName) && packingNameToId.ContainsKey(packName))
                            {
                                packingId = packingNameToId[packName];
                            }

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
                            bodyCmd.Parameters.AddWithValue("@TTLValue", row["Net Value"] != DBNull.Value ? Convert.ToDecimal(row["Net Value"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@TTLSTax", 0);
                            bodyCmd.Parameters.AddWithValue("@TTLFTax", 0);
                            bodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                            bodyCmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                            bodyCmd.Parameters.AddWithValue("@IsFreeProduct", false);

                            bodyCmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show($"Purchase invoice saved successfully!\nInvoice ID: {currentPurchaseId}\nVendor: {vendornametxtbox.Text}\nTotal Amount: {netTotal:N2}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //RefreshGridAfterSave();

                // Reset form for new entry
                //ClearForm();
                AutoGeneratePurchaseID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving purchase invoice: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ REMOVE ROW FROM GRID ============================
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
                //productsDataTable.Clear();

                // Reset checkboxes and radio buttons
                pendignchkbox.Checked = false;
                showloaddocxhkbox.Checked = false;
                dayshftradiobtn.Checked = true;

                // Set current dates
                SetCurrentDate();

                // Generate new ID
                AutoGeneratePurchaseID();

                // Focus on vendor ID
                vendoridtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================ ADD PRODUCT BUTTON CLICK ============================
        private void AddProductToGridButton_Click(object sender, EventArgs e)
        {
            AddProductToGrid();
        }

        // ============================ TEXTBOX CHANGE EVENTS FOR TOTALS ============================
        private void paidtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void specialdisctxtbox_TextChanged_1(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        // ============================ OPEN EXISTING INVOICE ============================
        private void openbtn_Click(object sender, EventArgs e)
        {
            SearchPurchaseInvoice searchForm = new SearchPurchaseInvoice();
            searchForm.Show();
            //MessageBox.Show("Open functionality to be implemented", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ============================ FORM CLOSING ============================
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

        // ============================ OTHER EVENT HANDLERS ============================
        private void PurchaseInvoice_Load(object sender, EventArgs e)
        {
            // Focus on vendor ID when form loads
            vendoridtxtbox.Focus();
            LoadAllProductsInGrid();

        }
        private void LoadAllProductsInGrid()
        {
            try
            {
                // Clear existing data
                productsDataTable.Clear();

                string query = @"SELECT p.ProductID, p.ProductName, p.PurchasePrice, 
                                p.PurDiscRatio, pp.PackingId, pp.Multiplier,
                                pack.PackingName
                         FROM Products p
                         LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                         LEFT JOIN Packings pack ON pp.PackingId = pack.PackingId
                         ORDER BY p.ProductID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataRow newRow = productsDataTable.NewRow();

                        // Database se data grid ke columns mein daalo
                        newRow["Pro ID"] = reader["ProductID"].ToString();
                        newRow["Product Name"] = reader["ProductName"].ToString();
                        newRow["Price"] = Convert.ToDecimal(reader["PurchasePrice"]);
                        newRow["Disc%"] = Convert.ToDecimal(reader["PurDiscRatio"]);

                        if (reader["PackingName"] != DBNull.Value)
                            newRow["Pack Name"] = reader["PackingName"].ToString();
                        else
                            newRow["Pack Name"] = "Piece";

                        if (reader["Multiplier"] != DBNull.Value)
                            newRow["Pack"] = Convert.ToInt32(reader["Multiplier"]);
                        else
                            newRow["Pack"] = 1;

                        // Default values
                        newRow["Qty(P)"] = 0;
                        newRow["Qty(L)"] = 0;
                        newRow["Bns(L)"] = 0;
                        newRow["Disc VAl-L"] = 0;
                        newRow["Disc.Val"] = 0;
                        newRow["Sales Tax"] = 0;
                        newRow["Net Value"] = 0;

                        productsDataTable.Rows.Add(newRow);
                    }
                }

                //MessageBox.Show($"Loaded {productsDataTable.Rows.Count} products in grid",
                               //"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error");
            }
        }
        private void purchaseidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }
        // Empty event handlers for auto-generated events
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

        private void bnsltxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void productnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void packnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void qtyptxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void qtyltxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void pricetxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void discpercenttxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void discvalltxtbox_TextChanged(object sender, EventArgs e)
        {

        }
        private void discvaltxtbox_TextChanged(object sender, EventArgs e)
        {

        }
        private void salestaxtxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void valuetxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void vendoridsearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                string vendorId = vendoridtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(vendorId))
                {
                    MessageBox.Show("Please enter Vendor ID", "Validation",

                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    vendoridtxtbox.Focus();
                    return;
                }

                string query = @"SELECT PartyID, PartyName, Address, City, Phone1, CreditLimit 
                        FROM Parties 
                        WHERE PartyID = @PartyID AND PartyType = 'V'";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PartyID", vendorId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Fill vendor information
                            vendornametxtbox.Text = reader["PartyName"].ToString();

                            // Show success message
                            //MessageBox.Show($"Vendor Found!\n" +
                            //              $"Name: {reader["PartyName"]}\n" +
                            //              $"Address: {reader["Address"]}\n" +
                            //              $"City: {reader["City"]}\n" +
                            //              $"Phone: {reader["Phone1"]}",
                            //              "Vendor Details",
                            //              MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Move focus to product search
                            productidtxtbox.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Vendor not found or not of type 'V'!",
                                          "Not Found",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            vendornametxtbox.Clear();
                            vendoridtxtbox.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching vendor: " + ex.Message,
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        
    }
    }
}