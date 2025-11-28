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
//    public partial class Purchase_Direct_to_Sale : Form
//    {
//        public Purchase_Direct_to_Sale()
//        {
//            InitializeComponent();
//        }

//        private void groupBox1_Enter(object sender, EventArgs e)
//        {

//        }

//        private void returndatepcker_ValueChanged(object sender, EventArgs e)
//        {

//        }

//        private void billlnolbl_Click(object sender, EventArgs e)
//        {

//        }

//        private void orderidlbl_Click(object sender, EventArgs e)
//        {

//        }

//        private void orderdatelbl_Click(object sender, EventArgs e)
//        {

//        }

//        private void billnotxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void label19_Click(object sender, EventArgs e)
//        {

//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void vehiclenotxt_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void netpurvaluetxt_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void currentstocltxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void netsalevaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totalgrosstxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void netvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totalpurvaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void topsalestaxtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void paidamounttxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totalratediff_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void totalsalevaluetxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
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
    public partial class Purchase_Direct_to_Sale : Form
    {
        private SqlConnection conn;
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable dtProducts;
        private DataTable dtPackings;

        public Purchase_Direct_to_Sale()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadInitialData();
            SetDefaultValues();
        }

        private void InitializeDatabase()
        {
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInitialData()
        {
            LoadProducts();
            LoadPackings();
            LoadNextIDs();
            ConfigureDataGridView();
        }

        private void LoadProducts()
        {
            try
            {
                string query = "SELECT ProductID, ProductName FROM Products WHERE IsFinishProduct = 1";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                dtProducts = new DataTable();
                da.Fill(dtProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void LoadPackings()
        {
            try
            {
                string query = "SELECT PackingId, PackingName FROM Packings";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                dtPackings = new DataTable();
                da.Fill(dtPackings);

                // Populate combobox column
                var packingColumn = (DataGridViewComboBoxColumn)dataGridView1.Columns["Column3"];
                packingColumn.DataSource = dtPackings;
                packingColumn.DisplayMember = "PackingName";
                packingColumn.ValueMember = "PackingId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message);
            }
        }

        private void LoadNextIDs()
        {
            try
            {
                // Get next Purchase ID
                string purchaseQuery = "SELECT ISNULL(MAX(PurchaseID), 0) + 1 FROM Purchase";
                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, conn);
                purchasedtxtbox.Text = purchaseCmd.ExecuteScalar().ToString();

                // Get next Sale ID
                string saleQuery = "SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Sales";
                SqlCommand saleCmd = new SqlCommand(saleQuery, conn);
                saleidtxtbox.Text = saleCmd.ExecuteScalar().ToString();

                // Generate bill numbers
                billpurchasetxtbox.Text = (Convert.ToInt32(purchasedtxtbox.Text) + 1000).ToString();
                salebilltxtbox.Text = (Convert.ToInt32(saleidtxtbox.Text) + 1000).ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading next IDs: " + ex.Message);
            }
        }

        private void ConfigureDataGridView()
        {
            // Add event handlers for calculations
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.RowsAdded += DataGridView1_RowsAdded;
            dataGridView1.RowsRemoved += DataGridView1_RowsRemoved;
        }

        private void SetDefaultValues()
        {
            purchasedatepcker.Value = DateTime.Now;
            saledatepcker.Value = DateTime.Now;
            dateTimePicker1.Value = DateTime.Now;

            // Set default values for calculations
            totalpurvaluetxtbox.Text = "0.00";
            totalsalevaluetxtbox.Text = "0.00";
            totalgrosstxtbox.Text = "0.00";
            totaldiscounttxtbox.Text = "0.00";
            topsalestaxtxtbox.Text = "0.00";
            netvaluetxtbox.Text = "0.00";
            paidamounttxtbox.Text = "0.00";
            totalratediff.Text = "0.00";
            netsalevaluetxtbox.Text = "0.00";
            alenetvaluetxtbox.Text = "0.00";
            prevbaltxtbox.Text = "0.00";
            finalbaltxtbox.Text = "0.00";
            receivedtxtbox.Text = "0.00";
            specialdistxtbox.Text = "0.00";
        }

        // Purchase Bill No changed - Load Purchase Data
        private void billpurchasetxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(billpurchasetxtbox.Text))
            {
                LoadPurchaseData();
            }
        }

        private void LoadPurchaseData()
        {
            try
            {
                string query = @"
                    SELECT 
                        p.PurchaseID, p.PurchaseDate, p.BillNo, p.BillDate,
                        pur.PartyName as VendorName, pur.City,
                        pb.ProductId, pr.ProductName, pb.PackingId, pk.PackingName,
                        pb.QtyPack, pb.QtyLose, pb.BnsLose, pb.PricePack, 
                        pb.DiscRatio, pb.DiscValLose, pb.STaxRatio, pb.PerUnitCostLose,
                        pb.TTLValue
                    FROM Purchase p
                    INNER JOIN Parties pur ON p.VendorID = pur.PartyID
                    INNER JOIN PurchasesBody pb ON p.PurchaseID = pb.PurchaseId
                    INNER JOIN Products pr ON pb.ProductId = pr.ProductID
                    INNER JOIN Packings pk ON pb.PackingId = pk.PackingId
                    WHERE p.BillNo = @BillNo AND pb.IsDeleted = 0";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BillNo", billpurchasetxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();

                dataGridView1.Rows.Clear();
                decimal totalPurchaseValue = 0;

                while (reader.Read())
                {
                    // Fill vendor info (only once)
                    if (dataGridView1.Rows.Count == 0)
                    {
                        purchasedtxtbox.Text = reader["PurchaseID"].ToString();
                        purchasedatepcker.Value = Convert.ToDateTime(reader["PurchaseDate"]);
                        dateTimePicker1.Value = Convert.ToDateTime(reader["BillDate"]);
                        vendortxtbox.Text = reader["VendorName"].ToString();
                        citytxtbox.Text = reader["City"].ToString();
                    }

                    // Add product to grid
                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Column1"].Value = reader["ProductId"];
                    dataGridView1.Rows[rowIndex].Cells["Column2"].Value = reader["ProductName"];
                    dataGridView1.Rows[rowIndex].Cells["Column3"].Value = reader["PackingId"];
                    dataGridView1.Rows[rowIndex].Cells["Column4"].Value = reader["PackingName"];
                    dataGridView1.Rows[rowIndex].Cells["Column5"].Value = reader["QtyPack"];
                    dataGridView1.Rows[rowIndex].Cells["Column6"].Value = reader["QtyLose"];
                    dataGridView1.Rows[rowIndex].Cells["Column7"].Value = reader["BnsLose"];
                    dataGridView1.Rows[rowIndex].Cells["Column8"].Value = reader["PricePack"];

                    // Calculate sale price (10% markup for example)
                    decimal purchasePrice = Convert.ToDecimal(reader["PricePack"]);
                    decimal salePrice = purchasePrice * 1.10m;
                    dataGridView1.Rows[rowIndex].Cells["Column10"].Value = salePrice;

                    dataGridView1.Rows[rowIndex].Cells["Column9"].Value = reader["DiscRatio"];
                    dataGridView1.Rows[rowIndex].Cells["Column11"].Value = reader["DiscValLose"];
                    dataGridView1.Rows[rowIndex].Cells["Column13"].Value = reader["STaxRatio"];

                    // Calculate rate difference
                    decimal rateDiff = salePrice - purchasePrice;
                    dataGridView1.Rows[rowIndex].Cells["Column14"].Value = rateDiff;

                    totalPurchaseValue += Convert.ToDecimal(reader["TTLValue"]);
                }
                reader.Close();

                netpurvaluetxt.Text = totalPurchaseValue.ToString("N2");
                CalculateAllTotals();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase data: " + ex.Message);
            }
        }

        // Customer ID changed - Load Customer Info
        private void customeridtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                LoadCustomerInfo();
            }
        }

        private void LoadCustomerInfo()
        {
            try
            {
                string query = @"
                    SELECT 
                        p.PartyName, 
                        s.SectorName, 
                        t.TownName,
                        p.City,
                        ISNULL((SELECT SUM(Debit - Credit) FROM AllVouchers WHERE AccountNo = p.PartyID), 0) as Balance
                    FROM Parties p
                    LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                    LEFT JOIN Towns t ON s.TownID = t.TownID
                    WHERE p.PartyID = @PartyID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PartyID", customeridtxtbox.Text);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    customernametxtbox.Text = reader["PartyName"].ToString();
                    sectortxtbox.Text = reader["SectorName"].ToString();
                    towntxtbox.Text = reader["TownName"].ToString();
                    prevbaltxtbox.Text = Convert.ToDecimal(reader["Balance"]).ToString("N2");

                    // Update final balance
                    UpdateFinalBalance();
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer info: " + ex.Message);
            }
        }

        // Salesman ID changed - Load Salesman Info
        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(salesmanidtxtbox.Text))
            {
                try
                {
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        salesmannametxtbox.Text = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading salesman info: " + ex.Message);
                }
            }
        }

        // Calculation Methods
        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                CalculateRowTotals(e.RowIndex);
                CalculateAllTotals();
            }
        }

        private void DataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CalculateAllTotals();
        }

        private void DataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateAllTotals();
        }

        private void CalculateRowTotals(int rowIndex)
        {
            try
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                decimal qtyPack = Convert.ToDecimal(row.Cells["Column5"].Value ?? 0);
                decimal qtyLose = Convert.ToDecimal(row.Cells["Column6"].Value ?? 0);
                decimal purPrice = Convert.ToDecimal(row.Cells["Column8"].Value ?? 0);
                decimal salePrice = Convert.ToDecimal(row.Cells["Column10"].Value ?? 0);
                decimal discRatio = Convert.ToDecimal(row.Cells["Column9"].Value ?? 0);
                decimal salesTaxRatio = Convert.ToDecimal(row.Cells["Column13"].Value ?? 0);

                // Calculate rate difference
                decimal rateDiff = salePrice - purPrice;
                row.Cells["Column14"].Value = rateDiff;

            }
            catch (Exception ex)
            {
                // Handle calculation errors silently
            }
        }

        private void CalculateAllTotals()
        {
            try
            {
                decimal totalPurValue = 0;
                decimal totalSaleValue = 0;
                decimal totalGross = 0;
                decimal totalDiscount = 0;
                decimal totalSalesTax = 0;
                decimal totalRateDiff = 0;
                decimal totalNetValue = 0;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue;

                    decimal qtyPack = Convert.ToDecimal(row.Cells["Column5"].Value ?? 0);
                    decimal qtyLose = Convert.ToDecimal(row.Cells["Column6"].Value ?? 0);
                    decimal purPrice = Convert.ToDecimal(row.Cells["Column8"].Value ?? 0);
                    decimal salePrice = Convert.ToDecimal(row.Cells["Column10"].Value ?? 0);
                    decimal discRatio = Convert.ToDecimal(row.Cells["Column9"].Value ?? 0);
                    decimal salesTaxRatio = Convert.ToDecimal(row.Cells["Column13"].Value ?? 0);

                    decimal totalQty = qtyPack + qtyLose;

                    // Calculate values
                    decimal purValue = totalQty * purPrice;
                    decimal saleValue = totalQty * salePrice;
                    decimal discount = saleValue * (discRatio / 100);
                    decimal netValue = saleValue - discount;
                    decimal salesTax = netValue * (salesTaxRatio / 100);
                    decimal rateDiff = salePrice - purPrice;

                    totalPurValue += purValue;
                    totalSaleValue += saleValue;
                    totalGross += saleValue;
                    totalDiscount += discount;
                    totalSalesTax += salesTax;
                    totalRateDiff += rateDiff * totalQty;
                    totalNetValue += netValue;
                }

                // Update all text boxes
                totalpurvaluetxtbox.Text = totalPurValue.ToString("N2");
                totalsalevaluetxtbox.Text = totalSaleValue.ToString("N2");
                totalgrosstxtbox.Text = totalGross.ToString("N2");
                totaldiscounttxtbox.Text = totalDiscount.ToString("N2");
                topsalestaxtxtbox.Text = totalSalesTax.ToString("N2");
                netvaluetxtbox.Text = totalNetValue.ToString("N2");
                totalratediff.Text = totalRateDiff.ToString("N2");
                netsalevaluetxtbox.Text = totalNetValue.ToString("N2");
                alenetvaluetxtbox.Text = totalNetValue.ToString("N2");

                // Update final balance
                UpdateFinalBalance();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in calculations: " + ex.Message);
            }
        }

        private void UpdateFinalBalance()
        {
            try
            {
                decimal previousBalance = Convert.ToDecimal(prevbaltxtbox.Text);
                decimal netValue = Convert.ToDecimal(netvaluetxtbox.Text);
                decimal received = Convert.ToDecimal(receivedtxtbox.Text ?? "0");
                decimal specialDiscount = Convert.ToDecimal(specialdistxtbox.Text ?? "0");

                decimal finalBalance = previousBalance + netValue - received - specialDiscount;
                finalbaltxtbox.Text = finalBalance.ToString("N2");

            }
            catch (Exception ex)
            {
                // Handle calculation errors
            }
        }

        // Received amount or Special discount changed
        private void receivedtxtbox_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalBalance();
        }

        private void specialdistxtbox_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalBalance();
        }

        // Save Button - Save Purchase to Sale Transaction
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                SaveTransaction();
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please enter Customer ID", "Validation Error");
                return false;
            }

            if (dataGridView1.Rows.Count == 0 || (dataGridView1.Rows.Count == 1 && dataGridView1.Rows[0].IsNewRow))
            {
                MessageBox.Show("Please add products to the sale", "Validation Error");
                return false;
            }

            return true;
        }

        private void SaveTransaction()
        {
            SqlTransaction transaction = null;
            try
            {
                transaction = conn.BeginTransaction();

                // 1. Save Sale Record
                SaveSaleRecord(transaction);

                // 2. Save Sale Body Records
                SaveSaleBodyRecords(transaction);

                // 3. Update Stock
                UpdateStock(transaction);

                transaction.Commit();
                MessageBox.Show("Purchase to Sale transaction completed successfully!", "Success");
                ClearForm();

            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving transaction: " + ex.Message, "Error");
            }
        }

        private void SaveSaleRecord(SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO Sales (
                    SaleId, SaleDate, BillNo, CustomerId, SectorID, SalesmanId, IsStockist,
                    Disc2Ratio, FTaxRatio, SpecialDiscount, ReceivedAmount, PreviousCredit,
                    InvoiceNotes, IsPosted, UserNo, BiltyNo, TransportName, Narration,
                    IsPendingRate, IsLocked
                ) VALUES (
                    @SaleId, @SaleDate, @BillNo, @CustomerId, @SectorID, @SalesmanId, @IsStockist,
                    @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @ReceivedAmount, @PreviousCredit,
                    @InvoiceNotes, 1, @UserNo, @BiltyNo, @TransportName, @Narration,
                    0, 1
                )";

            SqlCommand cmd = new SqlCommand(query, conn, transaction);

            // Add parameters
            cmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(saleidtxtbox.Text));
            cmd.Parameters.AddWithValue("@SaleDate", saledatepcker.Value);
            cmd.Parameters.AddWithValue("@BillNo", salebilltxtbox.Text);
            cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);
            cmd.Parameters.AddWithValue("@SectorID", GetSectorID() ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@SalesmanId", salesmanidtxtbox.Text ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsStockist", 0);
            cmd.Parameters.AddWithValue("@Disc2Ratio", 0);
            cmd.Parameters.AddWithValue("@FTaxRatio", 0);
            cmd.Parameters.AddWithValue("@SpecialDiscount", Convert.ToDecimal(specialdistxtbox.Text ?? "0"));
            cmd.Parameters.AddWithValue("@ReceivedAmount", Convert.ToDecimal(receivedtxtbox.Text ?? "0"));
            cmd.Parameters.AddWithValue("@PreviousCredit", Convert.ToDecimal(prevbaltxtbox.Text));
            cmd.Parameters.AddWithValue("@InvoiceNotes", narrationtxtbox.Text);
            cmd.Parameters.AddWithValue("@UserNo", 1); // Default user
            cmd.Parameters.AddWithValue("@BiltyNo", vehiclenotxt.Text ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@TransportName", "Direct Purchase to Sale");
            cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);

            cmd.ExecuteNonQuery();
        }

        private void SaveSaleBodyRecords(SqlTransaction transaction)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string query = @"
                    INSERT INTO SalesBody (
                        SaleId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose,
                        PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack,
                        IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax,
                        ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId,
                        OfferDiscount, IsDeleted
                    ) VALUES (
                        @SaleId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
                        @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack,
                        @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax,
                        @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId,
                        @OfferDiscount, 0
                    )";

                SqlCommand cmd = new SqlCommand(query, conn, transaction);

                // Get values from grid
                decimal qtyPack = Convert.ToDecimal(row.Cells["Column5"].Value ?? 0);
                decimal qtyLose = Convert.ToDecimal(row.Cells["Column6"].Value ?? 0);
                decimal salePrice = Convert.ToDecimal(row.Cells["Column10"].Value ?? 0);
                decimal discRatio = Convert.ToDecimal(row.Cells["Column9"].Value ?? 0);
                decimal salesTaxRatio = Convert.ToDecimal(row.Cells["Column13"].Value ?? 0);
                decimal purPrice = Convert.ToDecimal(row.Cells["Column8"].Value ?? 0);

                // Calculate totals
                decimal totalQty = qtyPack + qtyLose;
                decimal grossValue = totalQty * salePrice;
                decimal discount = grossValue * (discRatio / 100);
                decimal netValue = grossValue - discount;
                decimal salesTax = netValue * (salesTaxRatio / 100);

                // Add parameters
                cmd.Parameters.AddWithValue("@SaleId", Convert.ToInt32(saleidtxtbox.Text));
                cmd.Parameters.AddWithValue("@ProductId", row.Cells["Column1"].Value);
                cmd.Parameters.AddWithValue("@PackingId", row.Cells["Column3"].Value);
                cmd.Parameters.AddWithValue("@Multiplier", 1);
                cmd.Parameters.AddWithValue("@QtyPack", qtyPack);
                cmd.Parameters.AddWithValue("@QtyLose", qtyLose);
                cmd.Parameters.AddWithValue("@BnsLose", Convert.ToDecimal(row.Cells["Column7"].Value ?? 0));
                cmd.Parameters.AddWithValue("@PricePack", salePrice);
                cmd.Parameters.AddWithValue("@DiscRatio", discRatio);
                cmd.Parameters.AddWithValue("@DiscValPack", 0);
                cmd.Parameters.AddWithValue("@DiscValLose", 0);
                cmd.Parameters.AddWithValue("@STaxRatio", salesTaxRatio);
                cmd.Parameters.AddWithValue("@STaxValPack", 0);
                cmd.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                cmd.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                cmd.Parameters.AddWithValue("@IsFTaxExempted", 0);
                cmd.Parameters.AddWithValue("@TTLValue", netValue);
                cmd.Parameters.AddWithValue("@TTLSTax", salesTax);
                cmd.Parameters.AddWithValue("@TTLFTax", 0);
                cmd.Parameters.AddWithValue("@ExtraSTaxBns", 0);
                cmd.Parameters.AddWithValue("@ExtraFTaxBns", 0);
                cmd.Parameters.AddWithValue("@PerUnitCostLose", purPrice);
                cmd.Parameters.AddWithValue("@IsFreeProduct", 0);
                cmd.Parameters.AddWithValue("@PriceHistoryId", 0);
                cmd.Parameters.AddWithValue("@OfferDiscount", 0);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateStock(SqlTransaction transaction)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string productId = row.Cells["Column1"].Value.ToString();
                int qtyLose = Convert.ToInt32(row.Cells["Column6"].Value ?? 0);

                // Update CurrentStock table
                string query = @"
                    MERGE CurrentStock AS target
                    USING (SELECT @ProductId as ProductiD, @QtyLose as QtyLose) AS source
                    ON target.ProductiD = source.ProductiD
                    WHEN MATCHED THEN
                        UPDATE SET QtyLose = target.QtyLose - source.QtyLose
                    WHEN NOT MATCHED THEN
                        INSERT (ProductiD, QtyLose, PerUnitCostLose)
                        VALUES (source.ProductiD, -source.QtyLose, 0);";

                SqlCommand cmd = new SqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@QtyLose", qtyLose);
                cmd.ExecuteNonQuery();
            }
        }

        private int? GetSectorID()
        {
            try
            {
                string query = "SELECT SectorID FROM Parties WHERE PartyID = @PartyID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PartyID", customeridtxtbox.Text);

                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
            catch
            {
                return null;
            }
        }

        // Clear Button
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all inputs
            dataGridView1.Rows.Clear();
            narrationtxtbox.Clear();
            customeridtxtbox.Clear();
            customernametxtbox.Clear();
            sectortxtbox.Clear();
            towntxtbox.Clear();
            salesmanidtxtbox.Clear();
            salesmannametxtbox.Clear();
            receivedtxtbox.Clear();
            specialdistxtbox.Clear();

            // Reset purchase section
            billpurchasetxtbox.Clear();
            vendortxtbox.Clear();
            citytxtbox.Clear();
            textBox2.Clear();
            vehiclenotxt.Clear();
            netpurvaluetxt.Clear();

            // Reset totals
            SetDefaultValues();

            // Load new IDs
            LoadNextIDs();
        }

        // Close Button
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Existing event handlers (keep these)
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void returndatepcker_ValueChanged(object sender, EventArgs e) { }
        private void billlnolbl_Click(object sender, EventArgs e) { }
        private void orderidlbl_Click(object sender, EventArgs e) { }
        private void orderdatelbl_Click(object sender, EventArgs e) { }
        private void label19_Click(object sender, EventArgs e) { }
        private void pictureBox2_Click(object sender, EventArgs e) { this.Close(); }
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
        private void totalratediff_TextChanged(object sender, EventArgs e) { }
        private void totalsalevaluetxtbox_TextChanged(object sender, EventArgs e) { }

        // Form closing
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (conn != null && conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            base.OnFormClosing(e);
        }

        private void purchasedtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void billpurchasetxtbox_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}