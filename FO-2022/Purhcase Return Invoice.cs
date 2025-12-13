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
//    public partial class Purhcase_Return_Invoice : Form
//    {
//        public Purhcase_Return_Invoice()
//        {
//            InitializeComponent();
//        }

//        private void returnidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void billnotxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void purhcaseidsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void purchasedatetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void vendoridsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void vendornametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void grossvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void netvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void topsalestaxtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void receivedamounttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totalvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void openbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void productidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void productnametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void packnametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void textBox4_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void bnsltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void pricetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void discpercenttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void discvalltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void discvaltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void salestaxtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void valuetxtbox_TextChanged(object sender, EventArgs e)
//        {

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
    public partial class Purhcase_Return_Invoice : Form
    {
        // Connection string
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        private SqlConnection connection;
        private DataTable dataTable;
        private decimal grossValue = 0;
        private decimal totalDiscount = 0;
        private decimal netValue = 0;
        private decimal totalSalesTax = 0;
        private decimal totalValue = 0;

        public Purhcase_Return_Invoice()
        {
            InitializeComponent();
            InitializeConnection();
            InitializeDataGrid();
            SetupForm();
        }

        private void InitializeConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        private void InitializeDataGrid()
        {
            dataTable = new DataTable();
            dataTable.Columns.Add("ProductID", typeof(string));
            dataTable.Columns.Add("ProductName", typeof(string));
            dataTable.Columns.Add("Packing", typeof(string));
            dataTable.Columns.Add("QtyPack", typeof(int));
            dataTable.Columns.Add("QtyLose", typeof(int));
            dataTable.Columns.Add("BnsLose", typeof(int));
            dataTable.Columns.Add("PricePack", typeof(decimal));
            dataTable.Columns.Add("DiscPercent", typeof(decimal));
            dataTable.Columns.Add("DiscVal", typeof(decimal));
            dataTable.Columns.Add("DiscValLose", typeof(decimal));
            dataTable.Columns.Add("SalesTax", typeof(decimal));
            dataTable.Columns.Add("Value", typeof(decimal));

            dataGridView1.DataSource = dataTable;
        }

        private void SetupForm()
        {
            // Set date format
            purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Add Enter key event for productidtxtbox
            productidtxtbox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadProductData();
                }
            };
        }

        private void returnidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-generate Return ID if needed
            if (string.IsNullOrEmpty(returnidtxtbox.Text))
            {
                returnidtxtbox.Text = GetNextReturnId().ToString();
            }
        }

        private void billnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // You can add validation here
        }

        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Reset purchase date when purchase ID changes
            if (string.IsNullOrEmpty(purchaseidtxtbox.Text))
            {
                purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        private void purhcaseidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(purchaseidtxtbox.Text))
            {
                MessageBox.Show("Please enter Purchase ID");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PurchaseDate FROM Purchase WHERE PurchaseID = @PurchaseID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurchaseID", purchaseidtxtbox.Text);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        purchasedatetxtbox.Text = Convert.ToDateTime(result).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        MessageBox.Show("Purchase ID not found");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase date: " + ex.Message);
            }
        }

        private void purchasedatetxtbo_TextChanged(object sender, EventArgs e)
        {
            // Date validation can be added here
        }

        private void vendoridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear vendor name when ID changes
            vendornametxtbox.Clear();
        }

        private void vendoridsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(vendoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Vendor ID");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PartyID", vendoridtxtbox.Text);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        vendornametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Vendor ID not found");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor name: " + ex.Message);
            }
        }

        private void vendornametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Nothing needed here
        }

        private void LoadProductData()
        {
            if (string.IsNullOrEmpty(productidtxtbox.Text))
            {
                MessageBox.Show("Please enter Product ID");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                        p.ProductID,
                        p.ProductName,
                        pk.PackingName,
                        pp.Multiplier,
                        p.PurchasePrice,
                        p.PurDiscRatio,
                        p.PurSTRatio
                    FROM Products p
                    LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                    LEFT JOIN Packings pk ON pp.PackingId = pk.PackingId
                    WHERE p.ProductID = @ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productnametxtbox.Text = reader["ProductName"].ToString();
                            packnametxtbox.Text = reader["PackingName"].ToString();

                            // Get multiplier
                            int multiplier = 1;
                            if (reader["Multiplier"] != DBNull.Value)
                                multiplier = Convert.ToInt32(reader["Multiplier"]);

                            // Set default quantities
                            qtyptxtbox.Text = "0";
                            qtyltxtbox.Text = "0";
                            bnsltxtbox.Text = "0";

                            // Get prices and calculate
                            decimal price = Convert.ToDecimal(reader["PurchasePrice"]);
                            decimal discPercent = Convert.ToDecimal(reader["PurDiscRatio"]);
                            decimal salesTaxPercent = Convert.ToDecimal(reader["PurSTRatio"]);

                            pricetxtbox.Text = price.ToString("N2");
                            discpercenttxtbox.Text = discPercent.ToString("N2");

                            // Calculate discount value
                            decimal discValue = (price * discPercent) / 100;
                            discvalltxtbox.Text = discValue.ToString("N2");
                            discvaltxtbox.Text = discValue.ToString("N2");

                            // Calculate sales tax on discounted price
                            decimal discountedPrice = price - discValue;
                            decimal salesTaxValue = (discountedPrice * salesTaxPercent) / 100;
                            salestaxtxtbox.Text = salesTaxValue.ToString("N2");

                            // Calculate total value (including sales tax)
                            decimal value = discountedPrice + salesTaxValue;
                            valuetxtbox.Text = value.ToString("N2");

                            // Set focus to quantity
                            qtyptxtbox.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Product ID not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product data: " + ex.Message);
            }
        }

        private void AddProductToGrid()
        {
            // Validate all required fields
            if (string.IsNullOrEmpty(productidtxtbox.Text) ||
                string.IsNullOrEmpty(productnametxtbox.Text))
            {
                MessageBox.Show("Please load product data first");
                return;
            }

            try
            {
                // Parse values
                int qtyPack = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyLose = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
                int bnsLose = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
                decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
                decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
                decimal discVal = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : Convert.ToDecimal(discvalltxtbox.Text);
                decimal discValLose = string.IsNullOrEmpty(discvaltxtbox.Text) ? 0 : Convert.ToDecimal(discvaltxtbox.Text);
                decimal salesTax = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
                decimal value = string.IsNullOrEmpty(valuetxtbox.Text) ? 0 : Convert.ToDecimal(valuetxtbox.Text);

                // Add row to DataTable
                DataRow row = dataTable.NewRow();
                row["ProductID"] = productidtxtbox.Text;
                row["ProductName"] = productnametxtbox.Text;
                row["Packing"] = packnametxtbox.Text;
                row["QtyPack"] = qtyPack;
                row["QtyLose"] = qtyLose;
                row["BnsLose"] = bnsLose;
                row["PricePack"] = price;
                row["DiscPercent"] = discPercent;
                row["DiscVal"] = discVal;
                row["DiscValLose"] = discValLose;
                row["SalesTax"] = salesTax;
                row["Value"] = value;

                dataTable.Rows.Add(row);

                // Calculate totals
                CalculateTotals();

                // Clear product fields for next entry
                ClearProductFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to grid: " + ex.Message);
            }
        }

        private void CalculateTotals()
        {
            grossValue = 0;
            totalDiscount = 0;
            totalSalesTax = 0;
            totalValue = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                int qtyPack = Convert.ToInt32(row["QtyPack"]);
                int qtyLose = Convert.ToInt32(row["QtyLose"]);
                decimal price = Convert.ToDecimal(row["PricePack"]);
                decimal discVal = Convert.ToDecimal(row["DiscVal"]);
                decimal salesTax = Convert.ToDecimal(row["SalesTax"]);
                decimal value = Convert.ToDecimal(row["Value"]);

                // Calculate gross value (price * quantity)
                decimal lineGross = price * (qtyPack + qtyLose);
                grossValue += lineGross;

                // Calculate total discount
                decimal lineDiscount = discVal * (qtyPack + qtyLose);
                totalDiscount += lineDiscount;

                // Calculate sales tax
                decimal lineSalesTax = salesTax * (qtyPack + qtyLose);
                totalSalesTax += lineSalesTax;

                // Calculate net value
                totalValue += value * (qtyPack + qtyLose);
            }

            netValue = grossValue - totalDiscount;

            // Update text boxes
            grossvaluetxtbox.Text = grossValue.ToString("N2");
            totaldiscounttxtbox.Text = totalDiscount.ToString("N2");
            netvaluetxtbox.Text = netValue.ToString("N2");
            topsalestaxtxtbox.Text = totalSalesTax.ToString("N2");
            totalvaluetxtbox.Text = totalValue.ToString("N2");
        }

        private void ClearProductFields()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnametxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            pricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            discvaltxtbox.Clear();
            salestaxtxtbox.Clear();
            valuetxtbox.Clear();

            productidtxtbox.Focus();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle grid cell clicks if needed
        }

        private void grossvaluetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculated field
        }

        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculated field
        }

        private void netvaluetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculated field
        }

        private void topsalestaxtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculated field
        }

        private void receivedamounttxtbox_TextChanged(object sender, EventArgs e)
        {
            // You can add validation for received amount
            if (!string.IsNullOrEmpty(receivedamounttxtbox.Text))
            {
                try
                {
                    decimal received = Convert.ToDecimal(receivedamounttxtbox.Text);
                    // Validate against total value
                }
                catch
                {
                    MessageBox.Show("Please enter a valid amount");
                    receivedamounttxtbox.Clear();
                }
            }
        }

        private void totalvaluetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-calculated field
        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Nothing needed here
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(returnidtxtbox.Text) ||
                string.IsNullOrEmpty(vendoridtxtbox.Text) ||
                dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please fill all required fields and add at least one product");
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
                        // Insert into PurReturns header
                        string headerQuery = @"INSERT INTO PurReturns 
                            (PurReturnId, VendorId, ReturnDate, Disc2Ratio, FTaxRatio, 
                             SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax,
                             ReceivedAmount, PreviousCredit, Narration, IsPosted, UserNo, BillNo)
                            VALUES 
                            (@PurReturnId, @VendorId, @ReturnDate, @Disc2Ratio, @FTaxRatio,
                             @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
                             @ReceivedAmount, @PreviousCredit, @Narration, @IsPosted, @UserNo, @BillNo)";

                        SqlCommand headerCmd = new SqlCommand(headerQuery, conn, transaction);
                        headerCmd.Parameters.AddWithValue("@PurReturnId", Convert.ToInt32(returnidtxtbox.Text));
                        headerCmd.Parameters.AddWithValue("@VendorId", vendoridtxtbox.Text);
                        headerCmd.Parameters.AddWithValue("@ReturnDate", DateTime.Parse(purchasedatetxtbox.Text));
                        headerCmd.Parameters.AddWithValue("@Disc2Ratio", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@FTaxRatio", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@SpecialDiscount", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@InvOfferDiscount", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@InvOfferSTax", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@InvOfferFTax", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@ReceivedAmount",
                            string.IsNullOrEmpty(receivedamounttxtbox.Text) ? 0 : Convert.ToDecimal(receivedamounttxtbox.Text));
                        headerCmd.Parameters.AddWithValue("@PreviousCredit", 0); // Default value
                        headerCmd.Parameters.AddWithValue("@Narration",
                            string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
                        headerCmd.Parameters.AddWithValue("@IsPosted", 1); // Mark as posted
                        headerCmd.Parameters.AddWithValue("@UserNo", 1); // Default user
                        headerCmd.Parameters.AddWithValue("@BillNo",
                            string.IsNullOrEmpty(billnotxtbox.Text) ? DBNull.Value : (object)Convert.ToInt32(billnotxtbox.Text));

                        headerCmd.ExecuteNonQuery();

                        // Insert into PurReturnsBody for each product
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string bodyQuery = @"INSERT INTO PurReturnsBody 
                                (PurReturnId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose,
                                 PricePack, DiscRatio, DiscValPack, DiscValLose, OfferDiscount, STaxRatio,
                                 STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue,
                                 TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, IsFreeProduct, IsDeleted)
                                VALUES 
                                (@PurReturnId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
                                 @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @OfferDiscount, @STaxRatio,
                                 @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue,
                                 @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns, @IsFreeProduct, @IsDeleted)";

                            SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn, transaction);
                            bodyCmd.Parameters.AddWithValue("@PurReturnId", Convert.ToInt32(returnidtxtbox.Text));
                            bodyCmd.Parameters.AddWithValue("@ProductId", row["ProductID"].ToString());
                            bodyCmd.Parameters.AddWithValue("@PackingId", GetPackingId(row["Packing"].ToString()));
                            bodyCmd.Parameters.AddWithValue("@Multiplier", 1); // Default multiplier
                            bodyCmd.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["QtyPack"]));
                            bodyCmd.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["QtyLose"]));
                            bodyCmd.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["BnsLose"]));
                            bodyCmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["PricePack"]));
                            bodyCmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["DiscPercent"]));
                            bodyCmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["DiscVal"]));
                            bodyCmd.Parameters.AddWithValue("@DiscValLose", Convert.ToDecimal(row["DiscValLose"]));
                            bodyCmd.Parameters.AddWithValue("@OfferDiscount", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@STaxRatio", 0); // Get from product if needed
                            bodyCmd.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["SalesTax"]));
                            bodyCmd.Parameters.AddWithValue("@IsValueB4Ratio", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@IsSTaxOnBns", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@IsFTaxExempted", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["Value"]));
                            bodyCmd.Parameters.AddWithValue("@TTLSTax", Convert.ToDecimal(row["SalesTax"]));
                            bodyCmd.Parameters.AddWithValue("@TTLFTax", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@IsFreeProduct", 0); // Default value
                            bodyCmd.Parameters.AddWithValue("@IsDeleted", 0); // Default value

                            bodyCmd.ExecuteNonQuery();
                        }

                        // Commit transaction
                        transaction.Commit();

                        MessageBox.Show("Purchase Return saved successfully!");
                        ClearAllFields();
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
                MessageBox.Show("Error saving purchase return: " + ex.Message);
            }
        }

        private int GetPackingId(string packingName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PackingName", packingName);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1; // Default to 1 if not found
                }
            }
            catch
            {
                return 1; // Default packing ID
            }
        }

        private int GetNextReturnId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(PurReturnId), 0) + 1 FROM PurReturns";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch
            {
                return 1; // Default starting ID
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            // Clear all text boxes
            returnidtxtbox.Clear();
            billnotxtbox.Clear();
            purchaseidtxtbox.Clear();
            purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            vendoridtxtbox.Clear();
            vendornametxtbox.Clear();
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnametxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            pricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvalltxtbox.Clear();
            discvaltxtbox.Clear();
            salestaxtxtbox.Clear();
            valuetxtbox.Clear();
            grossvaluetxtbox.Clear();
            totaldiscounttxtbox.Clear();
            netvaluetxtbox.Clear();
            topsalestaxtxtbox.Clear();
            receivedamounttxtbox.Clear();
            totalvaluetxtbox.Clear();
            narrationtxtbox.Clear();

            // Clear data grid
            dataTable.Clear();

            // Reset totals
            grossValue = 0;
            totalDiscount = 0;
            netValue = 0;
            totalSalesTax = 0;
            totalValue = 0;

            // Generate new return ID
            returnidtxtbox.Text = GetNextReturnId().ToString();

            // Set focus to purchase ID
            purchaseidtxtbox.Focus();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            // Implement open functionality if needed
            MessageBox.Show("Open functionality not implemented yet");
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }

                // Recalculate totals
                CalculateTotals();
            }
            else
            {
                MessageBox.Show("Please select a row to remove");
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Add this method to handle adding product when Enter is pressed in quantity fields
        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddProductToGrid();
            }
        }

        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddProductToGrid();
            }
        }

        // Add these event handlers in your InitializeComponent or constructor
        private void SetupEventHandlers()
        {
            qtyptxtbox.KeyDown += qtyptxtbox_KeyDown;
            qtyltxtbox.KeyDown += qtyltxtbox_KeyDown;
        }
    }
}