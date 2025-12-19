using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Sale_Order : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable orderDetails;
        private decimal grossTotal = 0;
        private decimal salesTaxTotal = 0;
        private decimal discountTotal = 0;
        private decimal previousBalance = 0;
        private int currentPackingId = 0;
        private bool isLoadingOrder = false;

        public Sale_Order()
        {
            InitializeComponent();
            InitializeOrderDetailsTable();
            SetupAutoComplete();
        }

        private void Sale_Order_Load(object sender, EventArgs e)
        {
            // Generate auto Order ID
            GenerateOrderID();
            orderidtxtbox.Enabled = false;

            // Set current date
            orderdatetimepcker.Value = DateTime.Now;

            // Set focus to customer ID
            customeridtxtbox.Focus();

        }

        private void InitializeOrderDetailsTable()
        {
            if (orderDetails == null)
            {
                orderDetails = new DataTable();
            }
            else
            {
                orderDetails.Clear();
            }

            orderDetails.Columns.Add("Pro.ID", typeof(string));
            orderDetails.Columns.Add("Pro.Name", typeof(string));
            orderDetails.Columns.Add("Pack Name", typeof(string));
            orderDetails.Columns.Add("Pack", typeof(int));
            orderDetails.Columns.Add("Qty(P)", typeof(int));
            orderDetails.Columns.Add("Qty(L)", typeof(int));
            orderDetails.Columns.Add("Bns(L)", typeof(int));
            orderDetails.Columns.Add("Price", typeof(decimal));
            orderDetails.Columns.Add("Disc%", typeof(decimal));
            orderDetails.Columns.Add("Disc Val", typeof(decimal));
            orderDetails.Columns.Add("S_T %", typeof(decimal));
            orderDetails.Columns.Add("S_T Val", typeof(decimal));
            orderDetails.Columns.Add("TTL Value", typeof(decimal));
            orderDetails.Columns.Add("PackingId", typeof(int));

            dataGridView1.DataSource = orderDetails;
        }

        private void SetupAutoComplete()
        {
            try
            {
                // Setup auto-complete for product ID
                productidtxtbox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                productidtxtbox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection productCollection = new AutoCompleteStringCollection();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ProductID FROM Products ORDER BY ProductID", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        productCollection.Add(reader["ProductID"].ToString());
                    }
                    reader.Close();
                }
                productidtxtbox.AutoCompleteCustomSource = productCollection;

                // Setup auto-complete for customer ID
                customeridtxtbox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                customeridtxtbox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection customerCollection = new AutoCompleteStringCollection();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT PartyID FROM Parties WHERE PartyType='C' ORDER BY PartyID", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        customerCollection.Add(reader["PartyID"].ToString());
                    }
                    reader.Close();
                }
                customeridtxtbox.AutoCompleteCustomSource = customerCollection;

                // Setup auto-complete for salesman ID
                salesmenidtxtbox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                salesmenidtxtbox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection salesmanCollection = new AutoCompleteStringCollection();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT SalesmanID FROM Salesmen ORDER BY SalesmanID", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        salesmanCollection.Add(reader["SalesmanID"].ToString());
                    }
                    reader.Close();
                }
                salesmenidtxtbox.AutoCompleteCustomSource = salesmanCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in auto-complete setup: " + ex.Message);
            }
        }

        private void GenerateOrderID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Check if the table exists first
                    SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SaleOrder1st'", con);
                    int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (tableExists == 0)
                    {
                        // If table doesn't exist, start from 1
                        orderidtxtbox.Text = "1";
                        return;
                    }

                    // Get the next available Order ID
                    SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(OrderId), 0) + 1 FROM SaleOrder1st", con);
                    int newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                    orderidtxtbox.Text = newOrderId.ToString();
                }
            }
            catch (Exception ex)
            {
                // If there's an error, show message and set to 1
                MessageBox.Show("Error generating Order ID: " + ex.Message);
                orderidtxtbox.Text = "1";
            }
        }
        private void customeridsearch_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please enter Customer ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT p.PartyName AS CustomerName, s.SectorName AS TownSector, t.TownName " +
                        "FROM Parties p " +
                        "LEFT JOIN Sectors s ON p.SectorID = s.SectorID " +
                        "LEFT JOIN Towns t ON s.TownID = t.TownID " +
                        "WHERE p.PartyID = @CustomerID", con);
                    cmd.Parameters.AddWithValue("@CustomerID", customeridtxtbox.Text.Trim());

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        customernametxtbox.Text = reader["CustomerName"].ToString();
                        townsectortxtbox.Text = reader["TownSector"].ToString() + " - " + reader["TownName"].ToString();
                        reader.Close();
                        GetPreviousBalance(customeridtxtbox.Text.Trim());
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Customer not found!");
                        customernametxtbox.Clear();
                        townsectortxtbox.Clear();
                        prevbaltxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void GetPreviousBalance(string customerId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT ISNULL(Bal, 0) FROM AccountsBalances WHERE AccountNo = @CustomerID", con);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    object result = cmd.ExecuteScalar();
                    previousBalance = result != null ? Convert.ToDecimal(result) : 0;
                    prevbaltxtbox.Text = previousBalance.ToString("N2");
                    CalculateRemainingBalance();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting balance: " + ex.Message);
            }
        }

        private void salesmanudsearch_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(salesmenidtxtbox.Text))
            {
                MessageBox.Show("Please enter Salesman ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID", con);
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        salesmennametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Salesman not found!");
                        salesmennametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void productidtxtbox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID");
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(
                            "SELECT p.ProductName, p.SalePrice, p.SaleDiscRatio, p.SaleSTRatio " +
                            "FROM Products p " +
                            "WHERE p.ProductID = @ProductID", con);
                        cmd.Parameters.AddWithValue("@ProductID", productidtxtbox.Text.Trim());

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            productnametxtbox.Text = reader["ProductName"].ToString();
                            decimal price = Convert.ToDecimal(reader["SalePrice"]);
                            decimal discPercent = Convert.ToDecimal(reader["SaleDiscRatio"]);
                            decimal taxPercent = Convert.ToDecimal(reader["SaleSTRatio"]);

                            pricetxtbox.Text = price.ToString("N2");
                            discpercenttxtbox.Text = discPercent.ToString("N2");
                            salestaxpercenttxtbox.Text = taxPercent.ToString("N2");

                            decimal discountValue = price * (discPercent / 100);
                            discvaltxtbox.Text = discountValue.ToString("N2");

                            decimal priceAfterDiscount = price - discountValue;
                            decimal taxValue = priceAfterDiscount * (taxPercent / 100);
                            salestaxvaltxtbox.Text = taxValue.ToString("N2");

                            LoadProductPackings(productidtxtbox.Text.Trim());
                            packnamecombobox.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Product not found!");
                            ClearProductFields();
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoadProductPackings(string productId)
        {
            packnamecombobox.Items.Clear();
            packnamecombobox.Text = "";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT pp.PackingId, p.PackingName, pp.Multiplier " +
                        "FROM ProductPacking pp " +
                        "INNER JOIN Packings p ON pp.PackingId = p.PackingId " +
                        "WHERE pp.ProductId = @ProductID", con);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int packingId = Convert.ToInt32(reader["PackingId"]);
                        string packingName = reader["PackingName"].ToString();
                        int multiplier = Convert.ToInt32(reader["Multiplier"]);
                        packnamecombobox.Items.Add(new PackingItem(packingId, packingName, multiplier));
                    }
                    reader.Close();
                }

                packnamecombobox.DisplayMember = "PackingName";
                packnamecombobox.ValueMember = "PackingId";

                if (packnamecombobox.Items.Count > 0)
                {
                    packnamecombobox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message);
            }
        }

        private void packnamecombobox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (packnamecombobox.SelectedItem != null)
            {
                PackingItem selectedItem = (PackingItem)packnamecombobox.SelectedItem;
                packtxtbox.Text = selectedItem.Multiplier.ToString();
                currentPackingId = selectedItem.PackingId;
                qtyptxtbox.Enabled = true;
                qtyltxtbox.Enabled = true;
                qtyptxtbox.Focus();
            }
        }

        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalValue();
        }

        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalValue();
        }

        private void CalculateTotalValue()
        {
            try
            {
                if (!string.IsNullOrEmpty(packtxtbox.Text) && int.TryParse(packtxtbox.Text, out int multiplier) &&
                    !string.IsNullOrEmpty(pricetxtbox.Text) && decimal.TryParse(pricetxtbox.Text, out decimal price))
                {
                    decimal qtyP = 0;
                    if (!string.IsNullOrEmpty(qtyptxtbox.Text) && decimal.TryParse(qtyptxtbox.Text, out decimal parsedQtyP))
                    {
                        qtyP = parsedQtyP;
                    }

                    decimal qtyL = 0;
                    if (!string.IsNullOrEmpty(qtyltxtbox.Text) && decimal.TryParse(qtyltxtbox.Text, out decimal parsedQtyL))
                    {
                        qtyL = parsedQtyL;
                    }

                    decimal equivalentPacks = multiplier > 0 ? qtyL / multiplier : 0;
                    decimal totalQty = qtyP + equivalentPacks;
                    decimal totalValue = totalQty * price;
                    ttlvaluetxtbox.Text = totalValue.ToString("N2");
                }
            }
            catch (Exception)
            {
                ttlvaluetxtbox.Text = "0.00";
            }
        }

        private void qtyltxtbox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (packnamecombobox.SelectedItem == null)
                {
                    MessageBox.Show("Please select packing first!");
                    packnamecombobox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please select a product first!");
                    productidtxtbox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(qtyptxtbox.Text) && string.IsNullOrEmpty(qtyltxtbox.Text))
                {
                    MessageBox.Show("Please enter quantity (Qty(P) or Qty(L))");
                    qtyptxtbox.Focus();
                    return;
                }

                AddToGrid();
            }
        }

        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (packnamecombobox.SelectedItem == null)
                {
                    MessageBox.Show("Please select packing first!");
                    packnamecombobox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please select a product first!");
                    productidtxtbox.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(qtyptxtbox.Text) && string.IsNullOrEmpty(qtyltxtbox.Text))
                {
                    MessageBox.Show("Please enter quantity (Qty(P) or Qty(L))");
                    qtyptxtbox.Focus();
                    return;
                }

                AddToGrid();
            }
        }

        private void AddToGrid()
        {
            try
            {
                int qtyP = 0;
                if (!string.IsNullOrEmpty(qtyptxtbox.Text) && int.TryParse(qtyptxtbox.Text, out int parsedQtyP))
                {
                    qtyP = parsedQtyP;
                }

                int qtyL = 0;
                if (!string.IsNullOrEmpty(qtyltxtbox.Text) && int.TryParse(qtyltxtbox.Text, out int parsedQtyL))
                {
                    qtyL = parsedQtyL;
                }

                decimal price = 0;
                if (!string.IsNullOrEmpty(pricetxtbox.Text) && decimal.TryParse(pricetxtbox.Text, out decimal parsedPrice))
                {
                    price = parsedPrice;
                }

                decimal discPercent = 0;
                if (!string.IsNullOrEmpty(discpercenttxtbox.Text) && decimal.TryParse(discpercenttxtbox.Text, out decimal parsedDiscPercent))
                {
                    discPercent = parsedDiscPercent;
                }

                decimal discValue = 0;
                if (!string.IsNullOrEmpty(discvaltxtbox.Text) && decimal.TryParse(discvaltxtbox.Text, out decimal parsedDiscValue))
                {
                    discValue = parsedDiscValue;
                }

                decimal taxPercent = 0;
                if (!string.IsNullOrEmpty(salestaxpercenttxtbox.Text) && decimal.TryParse(salestaxpercenttxtbox.Text, out decimal parsedTaxPercent))
                {
                    taxPercent = parsedTaxPercent;
                }

                decimal taxValue = 0;
                if (!string.IsNullOrEmpty(salestaxvaltxtbox.Text) && decimal.TryParse(salestaxvaltxtbox.Text, out decimal parsedTaxValue))
                {
                    taxValue = parsedTaxValue;
                }

                decimal totalValue = 0;
                if (!string.IsNullOrEmpty(ttlvaluetxtbox.Text) && decimal.TryParse(ttlvaluetxtbox.Text, out decimal parsedTotalValue))
                {
                    totalValue = parsedTotalValue;
                }

                int bonus = 0;
                if (!string.IsNullOrEmpty(bnsltxtbox.Text) && int.TryParse(bnsltxtbox.Text, out int parsedBonus))
                {
                    bonus = parsedBonus;
                }

                if (packnamecombobox.SelectedItem == null)
                {
                    MessageBox.Show("Please select packing first!");
                    packnamecombobox.Focus();
                    return;
                }

                PackingItem selectedPacking = (PackingItem)packnamecombobox.SelectedItem;
                int multiplier = selectedPacking.Multiplier;
                string packingName = selectedPacking.PackingName;
                int packingId = selectedPacking.PackingId;

                DataRow row = orderDetails.NewRow();
                row["Pro.ID"] = productidtxtbox.Text.Trim(); // Fixed: Added .Trim() here
                row["Pro.Name"] = productnametxtbox.Text;
                row["Pack Name"] = packingName;
                row["Pack"] = multiplier;
                row["Qty(P)"] = qtyP;
                row["Qty(L)"] = qtyL;
                row["Bns(L)"] = bonus;
                row["Price"] = price;
                row["Disc%"] = discPercent;
                row["Disc Val"] = discValue;
                row["S_T %"] = taxPercent;
                row["S_T Val"] = taxValue;
                row["TTL Value"] = totalValue;
                row["PackingId"] = packingId;

                orderDetails.Rows.Add(row);
                UpdateTotals();
                ClearProductFields();
                productidtxtbox.Focus();
                MessageBox.Show("Product added to order successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding to grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotals()
        {
            grossTotal = 0;
            salesTaxTotal = 0;
            discountTotal = 0;

            foreach (DataRow row in orderDetails.Rows)
            {
                grossTotal += Convert.ToDecimal(row["TTL Value"]);
                discountTotal += Convert.ToDecimal(row["Disc Val"]);
                salesTaxTotal += Convert.ToDecimal(row["S_T Val"]);
            }

            grossvaluetxtbox.Text = grossTotal.ToString("N2");
            totaldiscounttxtbox.Text = discountTotal.ToString("N2");
            topsalestxtbox.Text = salesTaxTotal.ToString("N2");

            decimal netValue = grossTotal + salesTaxTotal - discountTotal;
            netvaluetxtbox.Text = netValue.ToString("N2");
            CalculateRemainingBalance();
        }

        private void CalculateRemainingBalance()
        {
            if (decimal.TryParse(netvaluetxtbox.Text, out decimal netValue) &&
                decimal.TryParse(prevbaltxtbox.Text, out decimal prevBal))
            {
                decimal remainingBalance = prevBal + netValue;
                remainingbaltxtbox.Text = remainingBalance.ToString("N2");
            }
        }

        private void ClearProductFields()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnamecombobox.Items.Clear();
            packnamecombobox.Text = "";
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
            pricetxtbox.Clear();
            discpercenttxtbox.Clear();
            discvaltxtbox.Clear();
            salestaxpercenttxtbox.Clear();
            salestaxvaltxtbox.Clear();
            ttlvaluetxtbox.Clear();

            qtyptxtbox.Enabled = false;
            qtyltxtbox.Enabled = false;
            currentPackingId = 0;
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateOrder())
            {
                SaveOrder();
                // ClearForm() is already called in SaveOrder() after successful save
            }
        }

        private bool ValidateOrder()
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer");
                return false;
            }

            if (orderDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product");
                return false;
            }

            return true;
        }

        //private void SaveOrder()
        //{
        //    if (!decimal.TryParse(prevbaltxtbox.Text, out decimal prevBalance))
        //    {
        //        prevBalance = 0;
        //    }

        //    if (!int.TryParse(orderidtxtbox.Text, out int newOrderId))
        //    {
        //        MessageBox.Show("Invalid Order ID format!");
        //        return;
        //    }

        //    int billNo = newOrderId;
        //    if (!string.IsNullOrEmpty(billnotxtbox.Text) && int.TryParse(billnotxtbox.Text, out int parsedBillNo))
        //    {
        //        billNo = parsedBillNo;
        //    }

        //    using (SqlConnection con = new SqlConnection(connStr))
        //    {
        //        con.Open();
        //        SqlTransaction tran = con.BeginTransaction();

        //        try
        //        {
        //            string insertMaster = @"
        //        INSERT INTO SaleOrder1st 
        //        (OrderId, OrderDate, BillNo, CustomerId, SalesmanId, IsStockist, Disc2Ratio, FTaxRatio, 
        //         ReceivedAmount, PreviousCredit, IsPosted, UserNo, IsParchoon, IsApproved, ApprovedDate,
        //         DispatchedDate, IsFinalized, IsPartial, IsPartialDisperse, VehicleNo, DriverName, 
        //         FreightExpenses, DistributorPartyId)
        //        VALUES 
        //        (@OrderId, @OrderDate, @BillNo, @CustomerId, @SalesmanId, @IsStockist, @Disc2Ratio, @FTaxRatio,
        //         @ReceivedAmount, @PreviousCredit, @IsPosted, @UserNo, @IsParchoon, @IsApproved, @ApprovedDate,
        //         @DispatchedDate, @IsFinalized, @IsPartial, @IsPartialDisperse, @VehicleNo, @DriverName, 
        //         @FreightExpenses, @DistributorPartyId)";

        //            SqlCommand cmd = new SqlCommand(insertMaster, con, tran);
        //            cmd.Parameters.AddWithValue("@OrderId", newOrderId);
        //            cmd.Parameters.AddWithValue("@OrderDate", orderdatetimepcker.Value);
        //            cmd.Parameters.AddWithValue("@BillNo", billNo);
        //            cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

        //            if (string.IsNullOrEmpty(salesmenidtxtbox.Text))
        //                cmd.Parameters.AddWithValue("@SalesmanId", DBNull.Value);
        //            else
        //                cmd.Parameters.AddWithValue("@SalesmanId", salesmenidtxtbox.Text);

        //            cmd.Parameters.AddWithValue("@IsStockist", 0);
        //            cmd.Parameters.AddWithValue("@Disc2Ratio", 0m);
        //            cmd.Parameters.AddWithValue("@FTaxRatio", 0m);
        //            cmd.Parameters.AddWithValue("@ReceivedAmount", 0m);
        //            cmd.Parameters.AddWithValue("@PreviousCredit", prevBalance);
        //            cmd.Parameters.AddWithValue("@IsPosted", 0);
        //            cmd.Parameters.AddWithValue("@UserNo", 1);
        //            cmd.Parameters.AddWithValue("@IsParchoon", 0);
        //            cmd.Parameters.AddWithValue("@IsApproved", 0);
        //            cmd.Parameters.AddWithValue("@ApprovedDate", DateTime.Now);
        //            cmd.Parameters.AddWithValue("@DispatchedDate", DateTime.Now);
        //            cmd.Parameters.AddWithValue("@IsFinalized", 0);
        //            cmd.Parameters.AddWithValue("@IsPartial", 0);
        //            cmd.Parameters.AddWithValue("@IsPartialDisperse", 0);
        //            cmd.Parameters.AddWithValue("@VehicleNo", DBNull.Value);
        //            cmd.Parameters.AddWithValue("@DriverName", DBNull.Value);
        //            cmd.Parameters.AddWithValue("@FreightExpenses", DBNull.Value);
        //            cmd.Parameters.AddWithValue("@DistributorPartyId", DBNull.Value);

        //            cmd.ExecuteNonQuery();

        //            foreach (DataRow row in orderDetails.Rows)
        //            {
        //                string insertDetail = @"
        //            INSERT INTO SaleOrderBody1St 
        //            (OrderId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, 
        //             PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, 
        //             IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax,
        //             ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId,
        //             OfferDiscount, IsDeleted, IsDispatched)
        //            VALUES 
        //            (@OrderId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
        //             @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack,
        //             @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax,
        //             @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId,
        //             @OfferDiscount, @IsDeleted, @IsDispatched)";

        //                SqlCommand cmdDetail = new SqlCommand(insertDetail, con, tran);

        //                int packingId = 1;
        //                if (row.Table.Columns.Contains("PackingId") && row["PackingId"] != DBNull.Value)
        //                {
        //                    packingId = Convert.ToInt32(row["PackingId"]);
        //                }

        //                // FIXED: Trim the ProductId before inserting
        //                string productId = row["Pro.ID"].ToString().Trim();
        //                if (productId.Length > 5)
        //                {
        //                    productId = productId.Substring(0, 5);
        //                }

        //                cmdDetail.Parameters.AddWithValue("@OrderId", newOrderId);
        //                cmdDetail.Parameters.AddWithValue("@ProductId", productId);
        //                cmdDetail.Parameters.AddWithValue("@PackingId", packingId);
        //                cmdDetail.Parameters.AddWithValue("@Multiplier", Convert.ToInt32(row["Pack"]));
        //                cmdDetail.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["Qty(P)"]));
        //                cmdDetail.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["Qty(L)"]));
        //                cmdDetail.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["Bns(L)"]));
        //                cmdDetail.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["Price"]));
        //                cmdDetail.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["Disc%"]));
        //                cmdDetail.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["Disc Val"]));
        //                cmdDetail.Parameters.AddWithValue("@DiscValLose", 0m);
        //                cmdDetail.Parameters.AddWithValue("@STaxRatio", Convert.ToDecimal(row["S_T %"]));
        //                cmdDetail.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["S_T Val"]));
        //                cmdDetail.Parameters.AddWithValue("@IsValueB4Ratio", 0); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@IsSTaxOnBns", 0); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@IsFTaxExempted", 0); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["TTL Value"]));
        //                cmdDetail.Parameters.AddWithValue("@TTLSTax", Convert.ToDecimal(row["S_T Val"]));
        //                cmdDetail.Parameters.AddWithValue("@TTLFTax", 0m); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@ExtraSTaxBns", 0m); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@ExtraFTaxBns", 0m); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@PerUnitCostLose", 0m); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@IsFreeProduct", 0); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@PriceHistoryId", 0); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@OfferDiscount", 0m); // Add default value
        //                cmdDetail.Parameters.AddWithValue("@IsDeleted", 0);
        //                cmdDetail.Parameters.AddWithValue("@IsDispatched", 0);

        //                cmdDetail.ExecuteNonQuery();
        //            }

        //            tran.Commit();
        //            MessageBox.Show($"Order Saved Successfully!\nOrder ID: {newOrderId}\nBill No: {billNo}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            ClearForm();
        //            GenerateOrderID();
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            MessageBox.Show("Error saving order: " + ex.Message + "\n\nPlease check that all required fields are filled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
        private void SaveOrder()
        {
            if (!decimal.TryParse(prevbaltxtbox.Text, out decimal prevBalance))
            {
                prevBalance = 0;
            }

            if (!int.TryParse(orderidtxtbox.Text, out int newOrderId))
            {
                MessageBox.Show("Invalid Order ID format!");
                return;
            }

            int billNo = newOrderId;
            if (!string.IsNullOrEmpty(billnotxtbox.Text) && int.TryParse(billnotxtbox.Text, out int parsedBillNo))
            {
                billNo = parsedBillNo;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1. SaleOrder table me data insert karein
                    SaveToSaleOrder(con, tran, newOrderId, billNo, prevBalance);

                    // 2. SaleOrder1st table me data insert karein
                    SaveToSaleOrder1st(con, tran, newOrderId, billNo, prevBalance);

                    // 3. SaleOrderBody table me data insert karein
                    SaveToSaleOrderBody(con, tran, newOrderId);

                    // 4. SaleOrderBody1St table me data insert karein
                    SaveToSaleOrderBody1St(con, tran, newOrderId);

                    tran.Commit();
                    MessageBox.Show($"Order Saved Successfully in all 4 tables!\nOrder ID: {newOrderId}\nBill No: {billNo}",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    GenerateOrderID();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error saving order: " + ex.Message + "\n\nPlease check that all required fields are filled.",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveToSaleOrder(SqlConnection con, SqlTransaction tran, int orderId, int billNo, decimal prevBalance)
        {
            string insertSaleOrder = @"
        INSERT INTO SaleOrder 
        (OrderId, OrderDate, BillNo, CustomerId, SalesmanId, IsStockist, OldIsStockist, Narration,
         Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax,
         ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo, BiltyNo, TransportName,
         DistributorPartyId, ScannedDocuments, VehicleNo, DriverName, FreightExpenses, DispatchedDate,
         IsParchoon, SectorID)
        VALUES 
        (@OrderId, @OrderDate, @BillNo, @CustomerId, @SalesmanId, @IsStockist, @OldIsStockist, @Narration,
         @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
         @ReceivedAmount, @PreviousCredit, @InvoiceNotes, @IsPosted, @UserNo, @BiltyNo, @TransportName,
         @DistributorPartyId, @ScannedDocuments, @VehicleNo, @DriverName, @FreightExpenses, @DispatchedDate,
         @IsParchoon, @SectorID)";

            SqlCommand cmd = new SqlCommand(insertSaleOrder, con, tran);

            // Get SectorID if available from customer
            int sectorId = GetSectorIdFromCustomer(customeridtxtbox.Text);

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@OrderDate", orderdatetimepcker.Value);
            cmd.Parameters.AddWithValue("@BillNo", billNo);
            cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

            if (string.IsNullOrEmpty(salesmenidtxtbox.Text))
                cmd.Parameters.AddWithValue("@SalesmanId", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@SalesmanId", salesmenidtxtbox.Text);

            cmd.Parameters.AddWithValue("@IsStockist", 0);
            cmd.Parameters.AddWithValue("@OldIsStockist", DBNull.Value);
            cmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
            cmd.Parameters.AddWithValue("@Disc2Ratio", 0m);
            cmd.Parameters.AddWithValue("@FTaxRatio", 0m);
            cmd.Parameters.AddWithValue("@SpecialDiscount", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferDiscount", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferSTax", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferFTax", DBNull.Value);
            cmd.Parameters.AddWithValue("@ReceivedAmount", 0m);
            cmd.Parameters.AddWithValue("@PreviousCredit", prevBalance);
            cmd.Parameters.AddWithValue("@InvoiceNotes", DBNull.Value);
            cmd.Parameters.AddWithValue("@IsPosted", 0);
            cmd.Parameters.AddWithValue("@UserNo", 1);
            cmd.Parameters.AddWithValue("@BiltyNo", DBNull.Value);
            cmd.Parameters.AddWithValue("@TransportName", DBNull.Value);
            cmd.Parameters.AddWithValue("@DistributorPartyId", DBNull.Value);

            // FIXED: Image column ke liye empty byte array use karein
            cmd.Parameters.Add("@ScannedDocuments", SqlDbType.Image).Value = new byte[0];

            cmd.Parameters.AddWithValue("@VehicleNo", DBNull.Value);
            cmd.Parameters.AddWithValue("@DriverName", DBNull.Value);
            cmd.Parameters.AddWithValue("@FreightExpenses", DBNull.Value);
            cmd.Parameters.AddWithValue("@DispatchedDate", orderdatetimepcker.Value);
            cmd.Parameters.AddWithValue("@IsParchoon", 0);
            cmd.Parameters.AddWithValue("@SectorID", sectorId > 0 ? (object)sectorId : DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        private void SaveToSaleOrder1st(SqlConnection con, SqlTransaction tran, int orderId, int billNo, decimal prevBalance)
        {
            string insertSaleOrder1st = @"
        INSERT INTO SaleOrder1st 
        (OrderId, OrderDate, BillNo, CustomerId, SalesmanId, IsStockist, OldIsStockist, Narration,
         Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax,
         ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo, BiltyNo, TransportName,
         DistributorPartyId, IsApproved, ApprovedDate, DispatchedDate, IsFinalized, IsPartial,
         IsPartialDisperse, VehicleNo, DriverName, FreightExpenses, IsParchoon, SectorID)
        VALUES 
        (@OrderId, @OrderDate, @BillNo, @CustomerId, @SalesmanId, @IsStockist, @OldIsStockist, @Narration,
         @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
         @ReceivedAmount, @PreviousCredit, @InvoiceNotes, @IsPosted, @UserNo, @BiltyNo, @TransportName,
         @DistributorPartyId, @IsApproved, @ApprovedDate, @DispatchedDate, @IsFinalized, @IsPartial,
         @IsPartialDisperse, @VehicleNo, @DriverName, @FreightExpenses, @IsParchoon, @SectorID)";

            SqlCommand cmd = new SqlCommand(insertSaleOrder1st, con, tran);

            // Get SectorID if available from customer
            int sectorId = GetSectorIdFromCustomer(customeridtxtbox.Text);

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@OrderDate", orderdatetimepcker.Value);
            cmd.Parameters.AddWithValue("@BillNo", billNo);
            cmd.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

            if (string.IsNullOrEmpty(salesmenidtxtbox.Text))
                cmd.Parameters.AddWithValue("@SalesmanId", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("@SalesmanId", salesmenidtxtbox.Text);

            cmd.Parameters.AddWithValue("@IsStockist", 0);
            cmd.Parameters.AddWithValue("@OldIsStockist", DBNull.Value);
            cmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
            cmd.Parameters.AddWithValue("@Disc2Ratio", 0m);
            cmd.Parameters.AddWithValue("@FTaxRatio", 0m);
            cmd.Parameters.AddWithValue("@SpecialDiscount", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferDiscount", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferSTax", DBNull.Value);
            cmd.Parameters.AddWithValue("@InvOfferFTax", DBNull.Value);
            cmd.Parameters.AddWithValue("@ReceivedAmount", 0m);
            cmd.Parameters.AddWithValue("@PreviousCredit", prevBalance);
            cmd.Parameters.AddWithValue("@InvoiceNotes", DBNull.Value);
            cmd.Parameters.AddWithValue("@IsPosted", 0);
            cmd.Parameters.AddWithValue("@UserNo", 1);
            cmd.Parameters.AddWithValue("@BiltyNo", DBNull.Value);
            cmd.Parameters.AddWithValue("@TransportName", DBNull.Value);
            cmd.Parameters.AddWithValue("@DistributorPartyId", DBNull.Value);
            cmd.Parameters.AddWithValue("@IsApproved", 0);
            cmd.Parameters.AddWithValue("@ApprovedDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@DispatchedDate", orderdatetimepcker.Value);
            cmd.Parameters.AddWithValue("@IsFinalized", 0);
            cmd.Parameters.AddWithValue("@IsPartial", 0);
            cmd.Parameters.AddWithValue("@IsPartialDisperse", 0);
            cmd.Parameters.AddWithValue("@VehicleNo", DBNull.Value);
            cmd.Parameters.AddWithValue("@DriverName", DBNull.Value);
            cmd.Parameters.AddWithValue("@FreightExpenses", DBNull.Value);
            cmd.Parameters.AddWithValue("@IsParchoon", 0);
            cmd.Parameters.AddWithValue("@SectorID", sectorId > 0 ? (object)sectorId : DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        private void SaveToSaleOrderBody(SqlConnection con, SqlTransaction tran, int orderId)
        {
            foreach (DataRow row in orderDetails.Rows)
            {
                string insertSaleOrderBody = @"
            INSERT INTO SaleOrderBody 
            (OrderId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, 
             PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, 
             IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax,
             ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId,
             OfferDiscount, IsDeleted)
            VALUES 
            (@OrderId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
             @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack,
             @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax,
             @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId,
             @OfferDiscount, @IsDeleted)";

                SqlCommand cmd = new SqlCommand(insertSaleOrderBody, con, tran);

                string productId = row["Pro.ID"].ToString().Trim();
                if (productId.Length > 5)
                {
                    productId = productId.Substring(0, 5);
                }

                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@PackingId", Convert.ToInt32(row["PackingId"]));
                cmd.Parameters.AddWithValue("@Multiplier", Convert.ToInt32(row["Pack"]));
                cmd.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["Qty(P)"]));
                cmd.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["Qty(L)"]));
                cmd.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["Bns(L)"]));
                cmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["Price"]));
                cmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["Disc%"]));
                cmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["Disc Val"]));
                cmd.Parameters.AddWithValue("@DiscValLose", 0m);
                cmd.Parameters.AddWithValue("@STaxRatio", Convert.ToDecimal(row["S_T %"]));
                cmd.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["S_T Val"]));
                cmd.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                cmd.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                cmd.Parameters.AddWithValue("@IsFTaxExempted", 0);
                cmd.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["TTL Value"]));
                cmd.Parameters.AddWithValue("@TTLSTax", Convert.ToDecimal(row["S_T Val"]));
                cmd.Parameters.AddWithValue("@TTLFTax", 0m);
                cmd.Parameters.AddWithValue("@ExtraSTaxBns", 0m);
                cmd.Parameters.AddWithValue("@ExtraFTaxBns", 0m);
                cmd.Parameters.AddWithValue("@PerUnitCostLose", 0m);
                cmd.Parameters.AddWithValue("@IsFreeProduct", 0);
                cmd.Parameters.AddWithValue("@PriceHistoryId", 0);
                cmd.Parameters.AddWithValue("@OfferDiscount", 0m);
                cmd.Parameters.AddWithValue("@IsDeleted", 0);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveToSaleOrderBody1St(SqlConnection con, SqlTransaction tran, int orderId)
        {
            foreach (DataRow row in orderDetails.Rows)
            {
                string insertSaleOrderBody1St = @"
            INSERT INTO SaleOrderBody1St 
            (OrderId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, 
             PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, 
             IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax,
             ExtraSTaxBns, ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId,
             OfferDiscount, IsDeleted, IsDispatched)
            VALUES 
            (@OrderId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose,
             @PricePack, @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack,
             @IsValueB4Ratio, @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax,
             @ExtraSTaxBns, @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId,
             @OfferDiscount, @IsDeleted, @IsDispatched)";

                SqlCommand cmd = new SqlCommand(insertSaleOrderBody1St, con, tran);

                string productId = row["Pro.ID"].ToString().Trim();
                if (productId.Length > 5)
                {
                    productId = productId.Substring(0, 5);
                }

                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@PackingId", Convert.ToInt32(row["PackingId"]));
                cmd.Parameters.AddWithValue("@Multiplier", Convert.ToInt32(row["Pack"]));
                cmd.Parameters.AddWithValue("@QtyPack", Convert.ToInt32(row["Qty(P)"]));
                cmd.Parameters.AddWithValue("@QtyLose", Convert.ToInt32(row["Qty(L)"]));
                cmd.Parameters.AddWithValue("@BnsLose", Convert.ToInt32(row["Bns(L)"]));
                cmd.Parameters.AddWithValue("@PricePack", Convert.ToDecimal(row["Price"]));
                cmd.Parameters.AddWithValue("@DiscRatio", Convert.ToDecimal(row["Disc%"]));
                cmd.Parameters.AddWithValue("@DiscValPack", Convert.ToDecimal(row["Disc Val"]));
                cmd.Parameters.AddWithValue("@DiscValLose", 0m);
                cmd.Parameters.AddWithValue("@STaxRatio", Convert.ToDecimal(row["S_T %"]));
                cmd.Parameters.AddWithValue("@STaxValPack", Convert.ToDecimal(row["S_T Val"]));
                cmd.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                cmd.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                cmd.Parameters.AddWithValue("@IsFTaxExempted", 0);
                cmd.Parameters.AddWithValue("@TTLValue", Convert.ToDecimal(row["TTL Value"]));
                cmd.Parameters.AddWithValue("@TTLSTax", Convert.ToDecimal(row["S_T Val"]));
                cmd.Parameters.AddWithValue("@TTLFTax", 0m);
                cmd.Parameters.AddWithValue("@ExtraSTaxBns", 0m);
                cmd.Parameters.AddWithValue("@ExtraFTaxBns", 0m);
                cmd.Parameters.AddWithValue("@PerUnitCostLose", 0m);
                cmd.Parameters.AddWithValue("@IsFreeProduct", 0);
                cmd.Parameters.AddWithValue("@PriceHistoryId", 0);
                cmd.Parameters.AddWithValue("@OfferDiscount", 0m);
                cmd.Parameters.AddWithValue("@IsDeleted", 0);
                cmd.Parameters.AddWithValue("@IsDispatched", 0);

                cmd.ExecuteNonQuery();
            }
        }

        private int GetSectorIdFromCustomer(string customerId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT SectorID FROM Parties WHERE PartyID = @CustomerID", con);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception)
            {
                // Agar error aaye to 0 return karein
            }
            return 0;
        }
        private void openbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orderidtxtbox.Text))
            {
                MessageBox.Show("Please enter Order ID to open");
                return;
            }

            try
            {
                isLoadingOrder = true;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string masterQuery = @"
                        SELECT so.*, p.PartyName as CustomerName, s.SalesmanName
                        FROM SaleOrder1st so
                        LEFT JOIN Parties p ON so.CustomerId = p.PartyID
                        LEFT JOIN Salesmen s ON so.SalesmanId = s.SalesmanID
                        WHERE so.OrderId = @OrderId";

                    SqlCommand masterCmd = new SqlCommand(masterQuery, con);
                    masterCmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);

                    SqlDataReader reader = masterCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        orderdatetimepcker.Value = Convert.ToDateTime(reader["OrderDate"]);
                        billnotxtbox.Text = reader["BillNo"].ToString();
                        customeridtxtbox.Text = reader["CustomerId"].ToString();
                        customernametxtbox.Text = reader["CustomerName"].ToString();
                        salesmenidtxtbox.Text = reader["SalesmanId"] != DBNull.Value ? reader["SalesmanId"].ToString() : "";
                        salesmennametxtbox.Text = reader["SalesmanName"] != DBNull.Value ? reader["SalesmanName"].ToString() : "";
                        prevbaltxtbox.Text = Convert.ToDecimal(reader["PreviousCredit"]).ToString("N2");
                    }
                    else
                    {
                        MessageBox.Show("Order not found!");
                        reader.Close();
                        isLoadingOrder = false;
                        return;
                    }
                    reader.Close();

                    orderDetails.Clear();
                    string detailQuery = @"
                        SELECT 
                            sob.ProductId,
                            p.ProductName,
                            pk.PackingName,
                            sob.Multiplier as Pack,
                            sob.QtyPack,
                            sob.QtyLose,
                            sob.BnsLose,
                            sob.PricePack,
                            sob.DiscRatio,
                            sob.DiscValPack,
                            sob.STaxRatio,
                            sob.STaxValPack,
                            sob.TTLValue
                        FROM SaleOrderBody1St sob
                        INNER JOIN Products p ON sob.ProductId = p.ProductID
                        INNER JOIN Packings pk ON sob.PackingId = pk.PackingId
                        WHERE sob.OrderId = @OrderId AND (sob.IsDeleted = 0 OR sob.IsDeleted IS NULL)";

                    SqlCommand detailCmd = new SqlCommand(detailQuery, con);
                    detailCmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);

                    SqlDataReader detailReader = detailCmd.ExecuteReader();
                    while (detailReader.Read())
                    {
                        DataRow row = orderDetails.NewRow();
                        row["Pro.ID"] = detailReader["ProductId"];
                        row["Pro.Name"] = detailReader["ProductName"];
                        row["Pack Name"] = detailReader["PackingName"];
                        row["Pack"] = detailReader["Pack"];
                        row["Qty(P)"] = detailReader["QtyPack"];
                        row["Qty(L)"] = detailReader["QtyLose"];
                        row["Bns(L)"] = detailReader["BnsLose"];
                        row["Price"] = detailReader["PricePack"];
                        row["Disc%"] = detailReader["DiscRatio"];
                        row["Disc Val"] = detailReader["DiscValPack"];
                        row["S_T %"] = detailReader["STaxRatio"];
                        row["S_T Val"] = detailReader["STaxValPack"];
                        row["TTL Value"] = detailReader["TTLValue"];
                        row["PackingId"] = detailReader["PackingId"];

                        orderDetails.Rows.Add(row);
                    }
                    detailReader.Close();

                    UpdateTotals();
                    MessageBox.Show("Order loaded successfully!");
                }

                isLoadingOrder = false;
            }
            catch (Exception ex)
            {
                isLoadingOrder = false;
                MessageBox.Show("Error opening order: " + ex.Message);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orderidtxtbox.Text))
            {
                MessageBox.Show("Please enter Order ID to remove");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DeleteOrder();
            }
        }

        private void DeleteOrder()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand detailCmd = new SqlCommand(
                        "UPDATE SaleOrderBody1St SET IsDeleted = 1 WHERE OrderId = @OrderId", con, tran);
                    detailCmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);
                    detailCmd.ExecuteNonQuery();

                    SqlCommand masterCmd = new SqlCommand(
                        "DELETE FROM SaleOrder1st WHERE OrderId = @OrderId", con, tran);
                    masterCmd.Parameters.AddWithValue("@OrderId", orderidtxtbox.Text);
                    masterCmd.ExecuteNonQuery();

                    tran.Commit();
                    MessageBox.Show("Order deleted successfully!");
                    ClearForm();
                    GenerateOrderID();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error deleting order: " + ex.Message);
                }
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            // Don't call GenerateOrderID() here as it's already called in ClearForm()
        }
        private void ClearForm()
        {
            // First save the current order ID for reference if needed
            string currentOrderId = orderidtxtbox.Text;

            // Clear all fields
            billnotxtbox.Clear();
            customeridtxtbox.Clear();
            customernametxtbox.Clear();
            townsectortxtbox.Clear();
            salesmenidtxtbox.Clear();
            salesmennametxtbox.Clear();
            prevbaltxtbox.Clear();
            grossvaluetxtbox.Clear();
            totaldiscounttxtbox.Clear();
            topsalestxtbox.Clear();
            netvaluetxtbox.Clear();
            remainingbaltxtbox.Clear();
            narrationtxtbox.Clear();

            ClearProductFields();
            orderDetails.Clear();

            grossTotal = 0;
            salesTaxTotal = 0;
            discountTotal = 0;
            previousBalance = 0;

            // Generate new Order ID
            GenerateOrderID();

            // Set focus to customer field
            customeridtxtbox.Focus();
        }

        private void prntbtn_Click(object sender, EventArgs e)
        {
            if (orderDetails.Rows.Count == 0)
            {
                MessageBox.Show("No data to print");
                return;
            }

            string printContent = "SALE ORDER\n";
            printContent += "================================\n";
            printContent += $"Order ID: {orderidtxtbox.Text}\n";
            printContent += $"Date: {orderdatetimepcker.Value.ToShortDateString()}\n";
            printContent += $"Customer: {customernametxtbox.Text}\n";
            printContent += "================================\n";
            printContent += "Products:\n";

            foreach (DataRow row in orderDetails.Rows)
            {
                printContent += $"{row["Pro.Name"]} - Qty(P): {row["Qty(P)"]}, Qty(L): {row["Qty(L)"]}, Price: {Convert.ToDecimal(row["Price"]).ToString("N2")}, Total: {Convert.ToDecimal(row["TTL Value"]).ToString("N2")}\n";
            }

            printContent += "================================\n";
            printContent += $"Gross Total: {grossTotal.ToString("N2")}\n";
            printContent += $"Discount: {discountTotal.ToString("N2")}\n";
            printContent += $"Tax: {salesTaxTotal.ToString("N2")}\n";
            printContent += $"Net Total: {netvaluetxtbox.Text}\n";

            MessageBox.Show(printContent, "Print Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Helper class for packing items
        private class PackingItem
        {
            public int PackingId { get; set; }
            public string PackingName { get; set; }
            public int Multiplier { get; set; }

            public PackingItem(int packingId, string packingName, int multiplier)
            {
                PackingId = packingId;
                PackingName = packingName;
                Multiplier = multiplier;
            }

            public override string ToString()
            {
                return PackingName;
            }
        }

        // Other event handlers
        private void totaldiscounttxtbox_TextChanged(object sender, EventArgs e) { }
        private void remainingbaltxtbox_TextChanged(object sender, EventArgs e) { }     
        private void salestaxpercenttxtbox_TextChanged(object sender, EventArgs e) { }
        private void packtxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void productidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void salesmenidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void customernametxtbox_TextChanged(object sender, EventArgs e) { }
        private void salesmennametxtbox_TextChanged(object sender, EventArgs e) { }
        private void townsectortxtbox_TextChanged(object sender, EventArgs e) { }
        private void bnsltxtbox_TextChanged(object sender, EventArgs e) { }
        private void pricetxtbox_TextChanged(object sender, EventArgs e) { }
        private void discpercenttxtbox_TextChanged(object sender, EventArgs e) { }

        private void orderidtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Sale_Order_Load_1(object sender, EventArgs e)
        {

        }
    }
}