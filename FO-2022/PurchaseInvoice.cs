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
        // Database connection string from app.config
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable productsDataTable;
        private decimal grossTotal = 0;
        private decimal discountTotal = 0;
        private decimal salesTaxTotal = 0;
        private decimal netTotal = 0;
        private decimal paidAmount = 0;

        public PurchaseInvoice()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeDataGridView();
            AutoGeneratePurchaseID();
            SetCurrentDate();
            dayshftbtn.Checked = true; // Default day shift
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
            LoadPackingsComboBox();
        }

        private void LoadPackingsComboBox()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                var packNames = new List<string>();
                while (reader.Read())
                {
                    packNames.Add(reader["PackingName"].ToString());
                }
                reader.Close();

                // Set items for the ComboBox column
                var comboCol = dataGridView1.Columns["Pack Name"] as DataGridViewComboBoxColumn;
                if (comboCol != null)
                {
                    comboCol.Items.Clear();
                    comboCol.Items.AddRange(packNames.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message);
            }
        }

        private void AutoGeneratePurchaseID()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(PurchaseID), 0) + 1 FROM Purchase";
                SqlCommand cmd = new SqlCommand(query, connection);
                object result = cmd.ExecuteScalar();
                purchaseidtxtbox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Purchase ID: " + ex.Message);
            }
        }

        private void SetCurrentDate()
        {
            purchasedatepcker.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        // Vendor ID Search
        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            SearchVendor();
        }

        private void SearchVendor()
        {
            try
            {
                string query = "SELECT PartyID, PartyName, City, Phone1 FROM Parties WHERE PartyID = @PartyID AND PartyType = 'V'";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PartyID", vendoridtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    vendornametxtbox.Text = reader["PartyName"].ToString();
                    // You can add more vendor details if needed
                }
                else
                {
                    MessageBox.Show("Vendor not found! Make sure vendor exists and PartyType is 'V'", "Vendor Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching vendor: " + ex.Message);
            }
        }

        // Save Button
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SavePurchaseInvoice();
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(vendoridtxtbox.Text))
            {
                MessageBox.Show("Please select a vendor!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (productsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate vendor exists and is actually a vendor
            if (!ValidateVendor())
            {
                return false;
            }

            return true;
        }

        private bool ValidateVendor()
        {
            try
            {
                string query = "SELECT PartyID FROM Parties WHERE PartyID = @PartyID AND PartyType = 'V'";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PartyID", vendoridtxtbox.Text);

                object result = cmd.ExecuteScalar();
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        private void SavePurchaseInvoice()
        {
            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();

                // Save to Purchase table
                string purchaseQuery = @"INSERT INTO Purchase (PurchaseID, PurchaseDate, OfficeID, VendorID, BillNo, BillDate, 
                                       IsUploaded, PaidAmount, Narration, SpecialDiscount, DueDate, UserNo, IsPendingRate, 
                                       IsNightShift, GrossValue, DiscountValue, SalesTaxValue, NetValue)
                                       VALUES (@PurchaseID, @PurchaseDate, @OfficeID, @VendorID, @BillNo, @BillDate, 
                                       0, @PaidAmount, @Narration, @SpecialDiscount, @DueDate, @UserNo, @IsPendingRate, 
                                       @IsNightShift, @GrossValue, @DiscountValue, @SalesTaxValue, @NetValue)";

                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt32(purchaseidtxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@PurchaseDate", purchasedatepcker.Value);
                purchaseCmd.Parameters.AddWithValue("@OfficeID", DBNull.Value); // Set as needed
                purchaseCmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnootxtbox.Text) ? (object)DBNull.Value : billnootxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillDate", dateTimePicker2.Value);
                purchaseCmd.Parameters.AddWithValue("@PaidAmount", string.IsNullOrEmpty(paidtxtbox.Text) ? 0 : Convert.ToDecimal(paidtxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@SpecialDiscount", string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@DueDate", string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
                purchaseCmd.Parameters.AddWithValue("@UserNo", 1); // Default user
                purchaseCmd.Parameters.AddWithValue("@IsPendingRate", pendignchkbox.Checked);
                purchaseCmd.Parameters.AddWithValue("@IsNightShift", nightshftbtn.Checked);
                purchaseCmd.Parameters.AddWithValue("@GrossValue", grossTotal);
                purchaseCmd.Parameters.AddWithValue("@DiscountValue", discountTotal);
                purchaseCmd.Parameters.AddWithValue("@SalesTaxValue", salesTaxTotal);
                purchaseCmd.Parameters.AddWithValue("@NetValue", netTotal);

                purchaseCmd.ExecuteNonQuery();

                // Save to PurchasesBody table
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        string purchasesBodyQuery = @"INSERT INTO PurchasesBody (PurchaseId, ProductId, PackingId, Multiplier, QtyPack, 
                                                    QtyLose, BnsLose, PricePack, DiscRatio, DiscValPack, DiscValLose, OfferDiscount, 
                                                    STaxRatio, STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, 
                                                    TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct)
                                                    VALUES (@PurchaseId, @ProductId, @PackingId, @Multiplier, @QtyPack, 
                                                    @QtyLose, @BnsLose, @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @OfferDiscount, 
                                                    @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, 
                                                    @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct)";

                        SqlCommand purchasesBodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);
                        purchasesBodyCmd.Parameters.AddWithValue("@PurchaseId", Convert.ToInt32(purchaseidtxtbox.Text));
                        purchasesBodyCmd.Parameters.AddWithValue("@ProductId", row["Pro ID"]?.ToString() ?? "");
                        purchasesBodyCmd.Parameters.AddWithValue("@PackingId", GetPackingIdByName(row["Pack Name"]?.ToString() ?? ""));
                        purchasesBodyCmd.Parameters.AddWithValue("@Multiplier", Convert.ToInt32(row["Pack"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["Qty(P)"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["Qty(L)"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["Bns(L)"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["Price"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["Disc%"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["Disc.Val"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@DiscValLose", Convert.ToDecimal(row["Disc VAl-L"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@OfferDiscount", 0);
                        purchasesBodyCmd.Parameters.AddWithValue("@STaxRatio", Convert.ToDecimal(row["Sales Tax"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@STaxValPack", 0); // Calculate if needed
                        purchasesBodyCmd.Parameters.AddWithValue("@IsValueB4Ratio", true);
                        purchasesBodyCmd.Parameters.AddWithValue("@IsSTaxOnBns", false);
                        purchasesBodyCmd.Parameters.AddWithValue("@IsFTaxExempted", false);
                        purchasesBodyCmd.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["Net Value"]));
                        purchasesBodyCmd.Parameters.AddWithValue("@TTLSTax", 0); // Calculate if needed
                        purchasesBodyCmd.Parameters.AddWithValue("@TTLFTax", 0);
                        purchasesBodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                        purchasesBodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                        purchasesBodyCmd.Parameters.AddWithValue("@PerUnitCostLose", 0);
                        purchasesBodyCmd.Parameters.AddWithValue("@IsFreeProduct", false);

                        purchasesBodyCmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                MessageBox.Show("Purchase invoice saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                AutoGeneratePurchaseID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving purchase invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPackingIdByName(string packingName)
        {
            try
            {
                string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PackingName", packingName);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 1;
            }
            catch
            {
                return 1;
            }
        }

        // Calculate Totals
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
                        grossTotal += Convert.ToDecimal(row["Net Value"]) + Convert.ToDecimal(row["Disc.Val"]);
                        discountTotal += Convert.ToDecimal(row["Disc.Val"]);
                        salesTaxTotal += Convert.ToDecimal(row["Sales Tax"]);
                        netTotal += Convert.ToDecimal(row["Net Value"]);
                    }
                }

                decimal specialDisc = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : Convert.ToDecimal(specialdisctxtbox.Text);
                paidAmount = string.IsNullOrEmpty(paidtxtbox.Text) ? 0 : Convert.ToDecimal(paidtxtbox.Text);

                // Apply special discount
                netTotal -= specialDisc;

                totalgrosstxtbox.Text = grossTotal.ToString("N2");
                totaldiscounttxtbox.Text = discountTotal.ToString("N2");
                topsalestxtbox.Text = salesTaxTotal.ToString("N2");
                specialdisctxtbox.Text = specialDisc.ToString("N2");
                netvaluetxtbox.Text = netTotal.ToString("N2");
                paidtxtbox.Text = paidAmount.ToString("N2");

                // Calculate remaining balance
                decimal remainingBalance = netTotal - paidAmount;
                remainingtxtbox.Text = remainingBalance.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating totals: " + ex.Message);
            }
        }

        // Clear Form
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            try
            {
                vendoridtxtbox.Clear();
                vendornametxtbox.Clear();
                billnootxtbox.Clear();
                productsDataTable.Clear();
                narrationtxtbox.Clear();
                duedatetxtbox.Clear();
                totalgrosstxtbox.Clear();
                totaldiscounttxtbox.Clear();
                topsalestxtbox.Clear();
                specialdisctxtbox.Clear();
                netvaluetxtbox.Clear();
                paidtxtbox.Clear();
                remainingtxtbox.Clear();
                pendignchkbox.Checked = false;
                showloaddocxhkbox.Checked = false;
                dayshftbtn.Checked = true;
                SetCurrentDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message);
            }
        }

        // DataGridView Events
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                CalculateRowTotal(e.RowIndex);
                CalculateTotals();
            }
        }

        private void CalculateRowTotal(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                if (row.Cells["Qty(P)"].Value != null && row.Cells["Price"].Value != null)
                {
                    int qtyPack = Convert.ToInt32(row.Cells["Qty(P)"].Value);
                    int qtyLose = row.Cells["Qty(L)"].Value != null ? Convert.ToInt32(row.Cells["Qty(L)"].Value) : 0;
                    int bnsLose = row.Cells["Bns(L)"].Value != null ? Convert.ToInt32(row.Cells["Bns(L)"].Value) : 0;
                    decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                    decimal discPercent = row.Cells["Disc%"].Value != null ? Convert.ToDecimal(row.Cells["Disc%"].Value) : 0;
                    decimal salesTaxPercent = row.Cells["Sales Tax"].Value != null ? Convert.ToDecimal(row.Cells["Sales Tax"].Value) : 0;

                    // Calculate total quantity including bonus
                    decimal totalQty = qtyPack + (decimal)(qtyLose + bnsLose) / 100;

                    // Calculate gross value
                    decimal grossValue = totalQty * price;

                    // Calculate discount values
                    decimal discVal = grossValue * discPercent / 100;
                    decimal discValPerLose = discVal / (totalQty * 100); // Per lose unit

                    // Calculate net value before tax
                    decimal netBeforeTax = grossValue - discVal;

                    // Calculate sales tax
                    decimal salesTax = netBeforeTax * salesTaxPercent / 100;

                    // Calculate final net value
                    decimal finalNetValue = netBeforeTax + salesTax;

                    row.Cells["Disc VAl-L"].Value = discValPerLose;
                    row.Cells["Disc.Val"].Value = discVal;
                    row.Cells["Net Value"].Value = finalNetValue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating row total: " + ex.Message);
            }
        }

        // Remove selected row from DataGridView
        private void removebtn_Click(object sender, EventArgs e)
        {
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error removing row: " + ex.Message);
            }
        }

        // TextBox Change Events for Auto Calculation
        private void paidtxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        private void specialdisctxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotals();
        }

        // Product Search in DataGridView
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 0) // Pro ID column
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.KeyDown -= ProductIdTextBox_KeyDown;
                    textBox.KeyDown += ProductIdTextBox_KeyDown;
                }
            }
        }

        private void ProductIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    SearchProductInGrid(textBox.Text);
                }
            }
        }

        private void SearchProductInGrid(string productId)
        {
            try
            {
                string query = "SELECT ProductID, ProductName, PurchasePrice FROM Products WHERE ProductID = @ProductID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    dataGridView1.CurrentRow.Cells["Product Name"].Value = reader["ProductName"].ToString();
                    dataGridView1.CurrentRow.Cells["Price"].Value = reader["PurchasePrice"];
                    // Set default values
                    dataGridView1.CurrentRow.Cells["Pack"].Value = 1;
                    dataGridView1.CurrentRow.Cells["Qty(P)"].Value = 0;
                    dataGridView1.CurrentRow.Cells["Qty(L)"].Value = 0;
                    dataGridView1.CurrentRow.Cells["Bns(L)"].Value = 0;
                    dataGridView1.CurrentRow.Cells["Disc%"].Value = 0;
                    dataGridView1.CurrentRow.Cells["Sales Tax"].Value = 0;
                }
                else
                {
                    MessageBox.Show("Product not found!");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product: " + ex.Message);
            }
        }

        // Auto-add new row when needed
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                // Auto-fill default values for new row
                dataGridView1.Rows[e.RowIndex].Cells["Pack"].Value = 1;
                dataGridView1.Rows[e.RowIndex].Cells["Qty(P)"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Qty(L)"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Bns(L)"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Disc%"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Disc VAl-L"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Disc.Val"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Sales Tax"].Value = 0;
                dataGridView1.Rows[e.RowIndex].Cells["Net Value"].Value = 0;
            }
        }

        // Existing methods
        private void PurchaseInvoice_Load(object sender, EventArgs e) { }

        private void label5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            SearchPurchaseInvoice spi = new SearchPurchaseInvoice();
            spi.Show();
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
                MessageBox.Show("Error closing connection: " + ex.Message);
            }
            base.OnFormClosing(e);
        }
    }
}