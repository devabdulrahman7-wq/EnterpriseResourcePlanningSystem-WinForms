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
using System.Configuration;

namespace FO_2022
{
    public partial class Purhcase_Return_Invoice : Form
    {
        // Connection string from App.config
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

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

        private void LoadAllProductsData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT 
                        p.ProductID,
                        p.ProductName,
                        COALESCE(pk.PackingName, 'Not Set') as PackingName,
                        p.PurchasePrice,
                        p.PurDiscRatio,
                        p.PurSTRatio
                    FROM Products p
                    LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                    LEFT JOIN Packings pk ON pp.PackingId = pk.PackingId
                    ORDER BY p.ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable productsTable = new DataTable();
                    adapter.Fill(productsTable);

                    DisplayProductsInGrid(productsTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void DisplayProductsInGrid(DataTable productsTable)
        {
            dataTable.Clear();

            foreach (DataRow productRow in productsTable.Rows)
            {
                DataRow newRow = dataTable.NewRow();
                newRow["ProductID"] = productRow["ProductID"].ToString();
                newRow["ProductName"] = productRow["ProductName"].ToString();

                string packingName = productRow["PackingName"] != DBNull.Value ?
                    productRow["PackingName"].ToString() : "Not Set";
                newRow["Packing"] = packingName;

                newRow["QtyPack"] = 0;
                newRow["QtyLose"] = 0;
                newRow["BnsLose"] = 0;

                decimal price = productRow["PurchasePrice"] != DBNull.Value ?
                    Convert.ToDecimal(productRow["PurchasePrice"]) : 0;
                newRow["PricePack"] = price;

                decimal discPercent = productRow["PurDiscRatio"] != DBNull.Value ?
                    Convert.ToDecimal(productRow["PurDiscRatio"]) : 0;
                decimal salesTaxPercent = productRow["PurSTRatio"] != DBNull.Value ?
                    Convert.ToDecimal(productRow["PurSTRatio"]) : 0;

                decimal discValue = (price * discPercent) / 100;
                decimal discountedPrice = price - discValue;
                decimal salesTaxValue = (discountedPrice * salesTaxPercent) / 100;
                decimal value = discountedPrice + salesTaxValue;

                newRow["DiscPercent"] = discPercent;
                newRow["DiscVal"] = discValue;
                newRow["DiscValLose"] = discValue;
                newRow["SalesTax"] = salesTaxValue;
                newRow["Value"] = value;

                dataTable.Rows.Add(newRow);
            }
        }

        private void LoadPackingData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable packingTable = new DataTable();
                    adapter.Fill(packingTable);

                    packnamecombobox.Items.Clear();
                    packnamecombobox.DisplayMember = "PackingName";
                    packnamecombobox.ValueMember = "PackingId";
                    packnamecombobox.DataSource = packingTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packing data: " + ex.Message);
            }
        }

        private void SetupForm()
        {
            purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            LoadAllProductsData();
            LoadPackingData();

            productidtxtbox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter) LoadProductData();
            };

            dataGridView1.CellClick += DataGridView1_CellClick;
            packnamecombobox.SelectedIndexChanged += Packnamecombobox_SelectedIndexChanged;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                productidtxtbox.Text = row.Cells["ProductID"].Value?.ToString() ?? "";
                productnametxtbox.Text = row.Cells["ProductName"].Value?.ToString() ?? "";

                string packingName = row.Cells["Packing"].Value?.ToString() ?? "";
                packnamecombobox.Text = !string.IsNullOrEmpty(packingName) ? packingName :
                    (packnamecombobox.Items.Count > 0 ? packnamecombobox.Text : "");

                qtyptxtbox.Text = row.Cells["QtyPack"].Value?.ToString() ?? "0";
                qtyltxtbox.Text = row.Cells["QtyLose"].Value?.ToString() ?? "0";
                bnsltxtbox.Text = row.Cells["BnsLose"].Value?.ToString() ?? "0";
                pricetxtbox.Text = row.Cells["PricePack"].Value?.ToString() ?? "0";
                discpercenttxtbox.Text = row.Cells["DiscPercent"].Value?.ToString() ?? "0";
                discvalltxtbox.Text = row.Cells["DiscVal"].Value?.ToString() ?? "0";
                discvaltxtbox.Text = row.Cells["DiscValLose"].Value?.ToString() ?? "0";
                salestaxtxtbox.Text = row.Cells["SalesTax"].Value?.ToString() ?? "0";
                valuetxtbox.Text = row.Cells["Value"].Value?.ToString() ?? "0";
            }
        }

        private void vendoridsearch_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(vendoridtxtbox.Text))
            {
                MessageBox.Show("Please enter Vendor ID");
                return;
            }

            LoadVendorName(vendoridtxtbox.Text);
        }

        private void LoadVendorName(string vendorId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PartyName FROM Parties WHERE PartyID = @PartyID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PartyID", vendorId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        vendornametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Vendor ID not found");
                        vendornametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor: " + ex.Message);
            }
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
                        COALESCE(pk.PackingName, 'Not Set') as PackingName,
                        pp.PackingId,
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

                            if (reader["PackingId"] != DBNull.Value)
                            {
                                int packingId = Convert.ToInt32(reader["PackingId"]);
                                SetComboBoxByPackingId(packingId);
                            }
                            else
                            {
                                packnamecombobox.SelectedIndex = -1;
                            }

                            decimal price = reader["PurchasePrice"] != DBNull.Value ?
                                Convert.ToDecimal(reader["PurchasePrice"]) : 0;
                            decimal discPercent = reader["PurDiscRatio"] != DBNull.Value ?
                                Convert.ToDecimal(reader["PurDiscRatio"]) : 0;
                            decimal salesTaxPercent = reader["PurSTRatio"] != DBNull.Value ?
                                Convert.ToDecimal(reader["PurSTRatio"]) : 0;

                            pricetxtbox.Text = price.ToString("N2");
                            discpercenttxtbox.Text = discPercent.ToString("N2");

                            decimal discValue = (price * discPercent) / 100;
                            discvalltxtbox.Text = discValue.ToString("N2");
                            discvaltxtbox.Text = discValue.ToString("N2");

                            decimal discountedPrice = price - discValue;
                            decimal salesTaxValue = (discountedPrice * salesTaxPercent) / 100;
                            salestaxtxtbox.Text = salesTaxValue.ToString("N2");

                            decimal value = discountedPrice + salesTaxValue;
                            valuetxtbox.Text = value.ToString("N2");

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
                MessageBox.Show("Error loading product: " + ex.Message);
            }
        }

        private void SetComboBoxByPackingId(int packingId)
        {
            foreach (DataRowView item in packnamecombobox.Items)
            {
                if (Convert.ToInt32(item["PackingId"]) == packingId)
                {
                    packnamecombobox.SelectedItem = item;
                    return;
                }
            }
            if (packnamecombobox.Items.Count > 0) packnamecombobox.SelectedIndex = 0;
        }

        private void AddProductToGrid()
        {
            if (string.IsNullOrEmpty(productidtxtbox.Text) ||
                string.IsNullOrEmpty(productnametxtbox.Text))
            {
                MessageBox.Show("Please load product data first");
                return;
            }

            try
            {
                int qtyPack = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyLose = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
                int bnsLose = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);
                decimal price = string.IsNullOrEmpty(pricetxtbox.Text) ? 0 : Convert.ToDecimal(pricetxtbox.Text);
                decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : Convert.ToDecimal(discpercenttxtbox.Text);
                decimal discVal = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : Convert.ToDecimal(discvalltxtbox.Text);
                decimal discValLose = string.IsNullOrEmpty(discvaltxtbox.Text) ? 0 : Convert.ToDecimal(discvaltxtbox.Text);
                decimal salesTax = string.IsNullOrEmpty(salestaxtxtbox.Text) ? 0 : Convert.ToDecimal(salestaxtxtbox.Text);
                decimal value = string.IsNullOrEmpty(valuetxtbox.Text) ? 0 : Convert.ToDecimal(valuetxtbox.Text);

                string packingName = packnamecombobox.Text;
                if (string.IsNullOrEmpty(packingName)) packingName = "Not Set";

                DataRow row = dataTable.NewRow();
                row["ProductID"] = productidtxtbox.Text;
                row["ProductName"] = productnametxtbox.Text;
                row["Packing"] = packingName;
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
                CalculateTotals();
                ClearProductFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }

        private void CalculateTotals()
        {
            grossValue = 0; totalDiscount = 0; totalSalesTax = 0; totalValue = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                int qtyPack = Convert.ToInt32(row["QtyPack"]);
                int qtyLose = Convert.ToInt32(row["QtyLose"]);
                decimal price = Convert.ToDecimal(row["PricePack"]);
                decimal discVal = Convert.ToDecimal(row["DiscVal"]);
                decimal salesTax = Convert.ToDecimal(row["SalesTax"]);
                decimal value = Convert.ToDecimal(row["Value"]);

                int totalQty = qtyPack + qtyLose;
                grossValue += price * totalQty;
                totalDiscount += discVal * totalQty;
                totalSalesTax += salesTax * totalQty;
                totalValue += value * totalQty;
            }

            netValue = grossValue - totalDiscount;

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
            if (packnamecombobox.Items.Count > 0) packnamecombobox.SelectedIndex = 0;
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

        private int GetNextReturnId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(PurReturnId), 0) + 1 FROM PurReturns";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 1; }
        }
        //private int GetPackingId(string packingName)
        //{
        //    if (string.IsNullOrEmpty(packingName) || packingName == "Not Set") return 1;

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();
        //            string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            cmd.Parameters.AddWithValue("@PackingName", packingName);
        //            object result = cmd.ExecuteScalar();
        //            return result != null ? Convert.ToInt32(result) : 1;
        //        }
        //    }
        //    catch { return 1; }
        //}
        private int GetPackingId(string packingName)
        {
            if (string.IsNullOrEmpty(packingName) || packingName == "Not Set")
                return 1; // Default packing ID

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PackingName", packingName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1;
                }
            }
            catch { return 1; }
        }
        private void ClearAllFields()
        {
            returnidtxtbox.Clear();
            billnotxtbox.Clear();
            purchaseidtxtbox.Clear();
            purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            vendoridtxtbox.Clear();
            vendornametxtbox.Clear();
            productidtxtbox.Clear();
            productnametxtbox.Clear();
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

            dataTable.Clear();
            LoadAllProductsData();

            grossValue = totalDiscount = netValue = totalSalesTax = totalValue = 0;
            returnidtxtbox.Text = GetNextReturnId().ToString();
            purchaseidtxtbox.Focus();
        }

        // Event Handlers
        private void returnidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(returnidtxtbox.Text))
                returnidtxtbox.Text = GetNextReturnId().ToString();
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
                    string query = @"SELECT PurchaseDate, VendorID, BillNo 
                                  FROM Purchase WHERE PurchaseID = @PurchaseID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt32(purchaseidtxtbox.Text));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            purchasedatetxtbox.Text = Convert.ToDateTime(reader["PurchaseDate"]).ToString("dd/MM/yyyy");
                            vendoridtxtbox.Text = reader["VendorID"].ToString();
                            if (reader["BillNo"] != DBNull.Value)
                                billnotxtbox.Text = reader["BillNo"].ToString();

                            LoadVendorName(reader["VendorID"].ToString());
                        }
                        else MessageBox.Show("Purchase ID not found");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase: " + ex.Message);
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open functionality not implemented yet");
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    dataGridView1.Rows.Remove(row);
                CalculateTotals();
            }
            else MessageBox.Show("Please select a row to remove");
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) AddProductToGrid();
        }

        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) AddProductToGrid();
        }

        private void Packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedRowPacking();
        }

        private void UpdateSelectedRowPacking()
        {
            if (!string.IsNullOrEmpty(productidtxtbox.Text))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["ProductID"].ToString() == productidtxtbox.Text)
                    {
                        row["Packing"] = packnamecombobox.Text;
                        CalculateTotals();
                        break;
                    }
                }
            }
        }

        private void clearbtn_Click_1(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
                // Validation
                if (string.IsNullOrEmpty(returnidtxtbox.Text) ||
                    string.IsNullOrEmpty(vendoridtxtbox.Text) ||
                    dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Please fill all required fields");
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
                            int purReturnId = Convert.ToInt32(returnidtxtbox.Text);

                            // 1. Insert Header
                            string headerQuery = @"INSERT INTO PurReturns 
                    (PurReturnId, PurchaseId, VendorId, ReturnDate, Disc2Ratio, FTaxRatio, 
                    SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax,
                    ReceivedAmount, PreviousCredit, Narration, IsPosted, UserNo, BillNo)
                    VALUES 
                    (@PurReturnId, @PurchaseId, @VendorId, @ReturnDate, @Disc2Ratio, @FTaxRatio,
                    @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
                    @ReceivedAmount, @PreviousCredit, @Narration, @IsPosted, @UserNo, @BillNo)";

                            using (SqlCommand headerCmd = new SqlCommand(headerQuery, conn, transaction))
                            {
                                headerCmd.Parameters.AddWithValue("@PurReturnId", purReturnId);
                                headerCmd.Parameters.AddWithValue("@VendorId", vendoridtxtbox.Text);
                                headerCmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                                headerCmd.Parameters.AddWithValue("@IsPosted", 1);
                                headerCmd.Parameters.AddWithValue("@UserNo", 1);

                                if (!string.IsNullOrEmpty(purchaseidtxtbox.Text) &&
                                    int.TryParse(purchaseidtxtbox.Text, out int purchaseId))
                                    headerCmd.Parameters.AddWithValue("@PurchaseId", purchaseId);
                                else
                                    headerCmd.Parameters.AddWithValue("@PurchaseId", DBNull.Value);

                                headerCmd.Parameters.AddWithValue("@Disc2Ratio", 0.00m);
                                headerCmd.Parameters.AddWithValue("@FTaxRatio", 0.00m);
                                headerCmd.Parameters.AddWithValue("@SpecialDiscount", 0.00m);
                                headerCmd.Parameters.AddWithValue("@InvOfferDiscount", 0.00m);
                                headerCmd.Parameters.AddWithValue("@InvOfferSTax", 0.00m);
                                headerCmd.Parameters.AddWithValue("@InvOfferFTax", 0.00m);

                                decimal receivedAmount = 0;
                                if (!string.IsNullOrEmpty(receivedamounttxtbox.Text))
                                    decimal.TryParse(receivedamounttxtbox.Text, out receivedAmount);
                                headerCmd.Parameters.AddWithValue("@ReceivedAmount", receivedAmount);

                                headerCmd.Parameters.AddWithValue("@PreviousCredit", 0.00m);

                                if (!string.IsNullOrEmpty(narrationtxtbox.Text))
                                    headerCmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);
                                else
                                    headerCmd.Parameters.AddWithValue("@Narration", DBNull.Value);

                                if (!string.IsNullOrEmpty(billnotxtbox.Text))
                                    headerCmd.Parameters.AddWithValue("@BillNo", billnotxtbox.Text);
                                else
                                    headerCmd.Parameters.AddWithValue("@BillNo", DBNull.Value);

                                headerCmd.ExecuteNonQuery();
                            }

                            // 2. Insert Body Rows - WITH DUPLICATE CHECKING
                            string bodyQuery = @"INSERT INTO PurReturnsBody 
                    (PurReturnId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose,
                    PricePack, DiscRatio, DiscValPack, DiscValLose, OfferDiscount,
                    STaxRatio, STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted,
                    TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns,
                    PerUnitCostLose, IsFreeProduct, IsDeleted)
                    VALUES 
                    (@PurReturnId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
                    @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @OfferDiscount,
                    @STaxRatio, @STaxValPack, @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted,
                    @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns, @ExtraFTaxBns,
                    @PerUnitCostLose, @IsFreeProduct, @IsDeleted)";

                            // Group rows by (ProductId, PackingId) to avoid duplicates
                            var groupedRows = dataTable.AsEnumerable()
                                .GroupBy(row => new
                                {
                                    ProductId = row["ProductID"].ToString(),
                                    PackingId = GetPackingId(row["Packing"]?.ToString())
                                })
                                .Select(group => new
                                {
                                    ProductId = group.Key.ProductId,
                                    PackingId = group.Key.PackingId,
                                    QtyPack = group.Sum(r => Convert.ToInt32(r["QtyPack"])),
                                    QtyLose = group.Sum(r => Convert.ToInt32(r["QtyLose"])),
                                    // Take first row's values for other fields (they should be same for same product+packing)
                                    FirstRow = group.First()
                                })
                                .ToList();

                            int rowsInserted = 0;
                            foreach (var item in groupedRows)
                            {
                                using (SqlCommand bodyCmd = new SqlCommand(bodyQuery, conn, transaction))
                                {
                                    DataRow row = item.FirstRow;

                                    bodyCmd.Parameters.AddWithValue("@PurReturnId", purReturnId);
                                    bodyCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                                    bodyCmd.Parameters.AddWithValue("@PackingId", item.PackingId);
                                    bodyCmd.Parameters.AddWithValue("@Multiplier", (short)1);
                                    bodyCmd.Parameters.AddWithValue("@QtyPack", item.QtyPack);
                                    bodyCmd.Parameters.AddWithValue("@QtyLose", item.QtyLose);
                                    bodyCmd.Parameters.AddWithValue("@BnsLose", (short)0);
                                    bodyCmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["PricePack"]));
                                    bodyCmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["DiscPercent"]));
                                    bodyCmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["DiscVal"]));
                                    bodyCmd.Parameters.AddWithValue("@DiscValLose", Convert.ToDecimal(row["DiscValLose"]));
                                    bodyCmd.Parameters.AddWithValue("@OfferDiscount", 0.00m);
                                    bodyCmd.Parameters.AddWithValue("@STaxRatio", 0.00m);
                                    bodyCmd.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["SalesTax"]));
                                    bodyCmd.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                                    bodyCmd.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                                    bodyCmd.Parameters.AddWithValue("@IsFTaxExempted", 0);

                                    int totalQty = item.QtyPack + item.QtyLose;
                                    decimal valuePerUnit = Convert.ToDecimal(row["Value"]);
                                    decimal salesTaxPerUnit = Convert.ToDecimal(row["SalesTax"]);

                                    bodyCmd.Parameters.AddWithValue("@TTLValue",
                                        Math.Round(valuePerUnit * totalQty, 2));
                                    bodyCmd.Parameters.AddWithValue("@TTLSTax",
                                        Math.Round(salesTaxPerUnit * totalQty, 2));
                                    bodyCmd.Parameters.AddWithValue("@TTLFTax", 0.00m);
                                    bodyCmd.Parameters.AddWithValue("@ExtraSTaxBns", 0.00m);
                                    bodyCmd.Parameters.AddWithValue("@ExtraFTaxBns", 0.00m);
                                    bodyCmd.Parameters.AddWithValue("@PerUnitCostLose", DBNull.Value);
                                    bodyCmd.Parameters.AddWithValue("@IsFreeProduct", 0);
                                    bodyCmd.Parameters.AddWithValue("@IsDeleted", 0);

                                    bodyCmd.ExecuteNonQuery();
                                    rowsInserted++;
                                }
                            }

                            transaction.Commit();
                            MessageBox.Show($"Purchase Return saved successfully!\nReturn ID: {purReturnId}\nProducts: {rowsInserted}");
                            ClearAllFields();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error saving: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection error: " + ex.Message);
                }
            }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 m = new Mainform1();
            m.Show();
        }
    }
}