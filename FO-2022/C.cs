using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class SaleInvoice : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DataTable invoiceDetails;
        private decimal grossTotal = 0;
        private decimal salesTaxTotal = 0;
        private decimal discountTotal = 0;
        private decimal discount2Total = 0;
        private decimal specialDiscountTotal = 0;
        private decimal freightExpenseTotal = 0;
        private decimal previousBalance = 0;
        private decimal cashReceived = 0;
        private int currentPackingId = 0;
        private bool isLoadingInvoice = false;
        private bool isUpdateMode = false;
        private int currentSaleId = 0;

        public SaleInvoice()
        {
            InitializeComponent();
            InitializeInvoiceDetailsTable();
            SetupAutoComplete();
            LoadDistributors();
        }

        private void SaleInvoice_Load(object sender, EventArgs e)
        {
            GenerateSaleID();
            saleidtxtbox.Enabled = false;
            saledatepcker.Value = DateTime.Now;
            SetDefaultValues();
            customeridtxtbox.Focus();
        }

        private void InitializeInvoiceDetailsTable()
        {
            if (invoiceDetails == null)
            {
                invoiceDetails = new DataTable();
            }
            else
            {
                invoiceDetails.Clear();
            }

            invoiceDetails.Columns.Add("Pro ID", typeof(string));
            invoiceDetails.Columns.Add("Product Name", typeof(string));
            invoiceDetails.Columns.Add("Pack Name", typeof(string));
            invoiceDetails.Columns.Add("Pack", typeof(int));
            invoiceDetails.Columns.Add("Qty(P)", typeof(int));
            invoiceDetails.Columns.Add("Qty(L)", typeof(int));
            invoiceDetails.Columns.Add("Bns(L)", typeof(int));
            invoiceDetails.Columns.Add("Price", typeof(decimal));
            invoiceDetails.Columns.Add("Disc%", typeof(decimal));
            invoiceDetails.Columns.Add("Disc 2%", typeof(decimal));
            invoiceDetails.Columns.Add("Disc.Val", typeof(decimal));
            invoiceDetails.Columns.Add("S-T%", typeof(decimal));
            invoiceDetails.Columns.Add("S-T Val", typeof(decimal));
            invoiceDetails.Columns.Add("TTL Value", typeof(decimal));
            invoiceDetails.Columns.Add("PackingId", typeof(int));

            dataGridView1.DataSource = invoiceDetails;
            ConfigureGridViewColumns();
        }

        private void ConfigureGridViewColumns()
        {
            dataGridView1.Columns["Pro ID"].Width = 85;
            dataGridView1.Columns["Product Name"].Width = 115;
            dataGridView1.Columns["Pack Name"].Width = 90;
            dataGridView1.Columns["Pack"].Width = 50;
            dataGridView1.Columns["Qty(P)"].Width = 35;
            dataGridView1.Columns["Qty(L)"].Width = 45;
            dataGridView1.Columns["Bns(L)"].Width = 45;
            dataGridView1.Columns["Price"].Width = 50;
            dataGridView1.Columns["Disc%"].Width = 60;
            dataGridView1.Columns["Disc 2%"].Width = 75;
            dataGridView1.Columns["Disc.Val"].Width = 75;
            dataGridView1.Columns["S-T%"].Width = 45;
            dataGridView1.Columns["S-T Val"].Width = 75;
            dataGridView1.Columns["TTL Value"].Width = 90;
            dataGridView1.Columns["PackingId"].Visible = false;
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

        private void LoadDistributors()
        {
            try
            {
                distributornamecombobox.Items.Clear();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT PartyID, PartyName FROM Parties WHERE PartyType='C' AND IsDistributor=1 ORDER BY PartyName", con);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        distributornamecombobox.Items.Add(reader["PartyName"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading distributors: " + ex.Message);
            }
        }

        private void GenerateSaleID()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Sales", con);
                    int newSaleId = Convert.ToInt32(cmd.ExecuteScalar());
                    saleidtxtbox.Text = newSaleId.ToString();
                    currentSaleId = newSaleId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Sale ID: " + ex.Message);
                saleidtxtbox.Text = "1";
                currentSaleId = 1;
            }
        }

        private void SetDefaultValues()
        {
            grosstxtbox.Text = "0.00";
            discounttxtbox.Text = "0.00";
            salestxtbox.Text = "0.00";
            disc2txtbox.Text = "0.00";
            specialdisctxtbox.Text = "0.00";
            freightexpensetxtbox.Text = "0.00";
            netvaluetxtbox.Text = "0.00";
            prevbaltxtbox.Text = "0.00";
            cashrectxtbox.Text = "0.00";
            balancetxtbox.Text = "0.00";
        }

        private void saleidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(saleidtxtbox.Text, out int saleId))
            {
                currentSaleId = saleId;
            }
        }

        private void customeridtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Auto-fetch customer details when ID is entered
            if (!string.IsNullOrEmpty(customeridtxtbox.Text) && customeridtxtbox.Text.Length >= 6)
            {
                FetchCustomerDetails();
            }
        }

        private void FetchCustomerDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT p.PartyName, s.SectorName, t.TownName " +
                        "FROM Parties p " +
                        "LEFT JOIN Sectors s ON p.SectorID = s.SectorID " +
                        "LEFT JOIN Towns t ON s.TownID = t.TownID " +
                        "WHERE p.PartyID = @CustomerID", con);
                    cmd.Parameters.AddWithValue("@CustomerID", customeridtxtbox.Text.Trim());

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        customernametxtbox.Text = reader["PartyName"].ToString();
                        string sector = reader["SectorName"] != DBNull.Value ? reader["SectorName"].ToString() : "";
                        string town = reader["TownName"] != DBNull.Value ? reader["TownName"].ToString() : "";
                        sectortowntxtbox.Text = $"{sector} / {town}";
                    }
                    else
                    {
                        customernametxtbox.Clear();
                        sectortowntxtbox.Clear();
                    }
                    reader.Close();

                    // Get previous balance
                    GetPreviousBalance(customeridtxtbox.Text.Trim());
                }
            }
            catch
            {
                // Silently fail for auto-fetch
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

        private void orderidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(orderidtxtbox.Text))
            {
                MessageBox.Show("Please enter Order ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        @"SELECT 
                    so.OrderDate,
                    so.CustomerId,
                    p.PartyName AS CustomerName,
                    so.SalesmanId,
                    sm.SalesmanName,
                    so.DistributorPartyId,
                    d.PartyName AS DistributorName,
                    so.SpecialDiscount,
                    so.FreightExpenses,
                    so.Narration,
                    s.SectorName,
                    t.TownName
                FROM SaleOrder1st so
                LEFT JOIN Parties p ON so.CustomerId = p.PartyID
                LEFT JOIN Salesmen sm ON so.SalesmanId = sm.SalesmanID
                LEFT JOIN Parties d ON so.DistributorPartyId = d.PartyID
                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                LEFT JOIN Towns t ON s.TownID = t.TownID
                WHERE so.OrderId = @OrderID", con);

                    cmd.Parameters.AddWithValue("@OrderID", orderidtxtbox.Text.Trim());

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        orderdatetxtbox.Text = Convert.ToDateTime(reader["OrderDate"]).ToString("dd/MM/yyyy");
                        customeridtxtbox.Text = reader["CustomerId"].ToString();
                        customernametxtbox.Text = reader["CustomerName"].ToString();
                        salesmenidtxtbox.Text = reader["SalesmanId"] != DBNull.Value ? reader["SalesmanId"].ToString() : "";
                        salesmennametxtbox.Text = reader["SalesmanName"] != DBNull.Value ? reader["SalesmanName"].ToString() : "";

                        string sector = reader["SectorName"] != DBNull.Value ? reader["SectorName"].ToString() : "";
                        string town = reader["TownName"] != DBNull.Value ? reader["TownName"].ToString() : "";
                        sectortowntxtbox.Text = $"{sector} / {town}";

                        if (reader["DistributorName"] != DBNull.Value)
                        {
                            distributornamecombobox.Text = reader["DistributorName"].ToString();
                        }

                        specialdisctxtbox.Text = reader["SpecialDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["SpecialDiscount"]).ToString("N2") : "0.00";
                        freightexpensetxtbox.Text = reader["FreightExpenses"] != DBNull.Value ? Convert.ToDecimal(reader["FreightExpenses"]).ToString("N2") : "0.00";
                        narrationtxtbox.Text = reader["Narration"] != DBNull.Value ? reader["Narration"].ToString() : "";

                        reader.Close();

                        // Load order items - FIXED QUERY
                        LoadOrderItems(orderidtxtbox.Text.Trim());

                        // Get customer balance
                        GetPreviousBalance(customeridtxtbox.Text.Trim());

                        MessageBox.Show("Order loaded successfully!");
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Order ID not found!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order: " + ex.Message);
            }
        }

        private void LoadOrderItems(string orderId)
        {
            try
            {
                invoiceDetails.Clear();

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // FIXED QUERY: Check which columns actually exist in SaleOrderBody1St table
                    SqlCommand cmd = new SqlCommand(
                        @"SELECT 
                    sob.ProductId,
                    p.ProductName,
                    pk.PackingName,
                    sob.Multiplier,
                    sob.QtyPack,
                    sob.QtyLose,
                    sob.BnsLose,
                    sob.PricePack,
                    sob.DiscRatio,
                    sob.DiscValPack,
                    sob.STaxRatio,
                    sob.STaxValPack,
                    sob.TTLValue,
                    sob.PackingId,
                    sob.OfferDiscount,
                    sob.PerUnitCostLose
                FROM SaleOrderBody1St sob
                INNER JOIN Products p ON sob.ProductId = p.ProductID
                INNER JOIN Packings pk ON sob.PackingId = pk.PackingId
                WHERE sob.OrderId = @OrderID AND (sob.IsDeleted = 0 OR sob.IsDeleted IS NULL)", con);

                    cmd.Parameters.AddWithValue("@OrderID", orderId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow row = invoiceDetails.NewRow();
                        row["Pro ID"] = reader["ProductId"].ToString();
                        row["Product Name"] = reader["ProductName"].ToString();
                        row["Pack Name"] = reader["PackingName"].ToString();
                        row["Pack"] = reader["Multiplier"];
                        row["Qty(P)"] = reader["QtyPack"];
                        row["Qty(L)"] = reader["QtyLose"];
                        row["Bns(L)"] = reader["BnsLose"];
                        row["Price"] = reader["PricePack"];
                        row["Disc%"] = reader["DiscRatio"];
                        row["Disc 2%"] = 0; // Default value if column doesn't exist
                        row["Disc.Val"] = reader["DiscValPack"];
                        row["S-T%"] = reader["STaxRatio"];
                        row["S-T Val"] = reader["STaxValPack"];
                        row["TTL Value"] = reader["TTLValue"];
                        row["PackingId"] = reader["PackingId"];

                        invoiceDetails.Rows.Add(row);
                    }
                    reader.Close();

                    UpdateTotals();
                    MessageBox.Show($"Loaded {invoiceDetails.Rows.Count} products from order");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order items: " + ex.Message);
            }
        }

        private void salesmenidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(salesmenidtxtbox.Text) && salesmenidtxtbox.Text.Length >= 6)
            {
                FetchSalesmanDetails();
            }
        }

        private void FetchSalesmanDetails()
        {
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
                        salesmennametxtbox.Clear();
                    }
                }
            }
            catch
            {
                // Silently fail for auto-fetch
            }
        }

        private void salesmanudsearch_Click(object sender, EventArgs e)
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

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(productidtxtbox.Text) && productidtxtbox.Text.Length >= 6)
            {
                FetchProductDetails();
            }
        }

        private void FetchProductDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT ProductName, SalePrice, SaleDiscRatio, SaleSTRatio " +
                        "FROM Products WHERE ProductID = @ProductID", con);
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
                        textBox1.Text = taxPercent.ToString("N2");

                        // Calculate discount value
                        decimal discountValue = price * (discPercent / 100);
                        discvaltxtbox.Text = discountValue.ToString("N2");

                        // Calculate tax value
                        decimal priceAfterDiscount = price - discountValue;
                        decimal taxValue = priceAfterDiscount * (taxPercent / 100);
                        salestaxtxtbox.Text = taxValue.ToString("N2");

                        // Load packings for this product
                        LoadProductPackings(productidtxtbox.Text.Trim());
                    }
                    else
                    {
                        ClearProductFields();
                    }
                    reader.Close();
                }
            }
            catch
            {
                // Silently fail for auto-fetch
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

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
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

        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string productId = productidtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(productId))
                {
                    MessageBox.Show("Please enter Product ID");
                    return;
                }

                if (productId.Length > 6)
                {
                    productId = productId.Substring(0, 5);
                }

                productidtxtbox.Text = productId;

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(
                            "SELECT ProductName, SalePrice, SaleDiscRatio, SaleSTRatio " +
                            "FROM Products WHERE ProductID = @ProductID", con);
                        cmd.Parameters.AddWithValue("@ProductID", productId);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            productnametxtbox.Text = reader["ProductName"].ToString();
                            decimal price = Convert.ToDecimal(reader["SalePrice"]);
                            decimal discPercent = Convert.ToDecimal(reader["SaleDiscRatio"]);
                            decimal taxPercent = Convert.ToDecimal(reader["SaleSTRatio"]);

                            pricetxtbox.Text = price.ToString("N2");
                            discpercenttxtbox.Text = discPercent.ToString("N2");
                            textBox1.Text = taxPercent.ToString("N2");

                            decimal discountValue = price * (discPercent / 100);
                            discvaltxtbox.Text = discountValue.ToString("N2");

                            decimal priceAfterDiscount = price - discountValue;
                            decimal taxValue = priceAfterDiscount * (taxPercent / 100);
                            salestaxtxtbox.Text = taxValue.ToString("N2");

                            LoadProductPackings(productId);
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

        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalValue();
        }

        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateTotalValue();
        }

        private void qtyptxtbox_KeyDown_1(object sender, KeyEventArgs e)
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

        private void CalculateTotalValue()
        {
            try
            {
                if (!string.IsNullOrEmpty(packtxtbox.Text) && int.TryParse(packtxtbox.Text, out int multiplier) &&
                    !string.IsNullOrEmpty(pricetxtbox.Text) && decimal.TryParse(pricetxtbox.Text, out decimal price))
                {
                    decimal qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : decimal.Parse(qtyptxtbox.Text);
                    decimal qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : decimal.Parse(qtyltxtbox.Text);
                    decimal discPercent = string.IsNullOrEmpty(discpercenttxtbox.Text) ? 0 : decimal.Parse(discpercenttxtbox.Text);
                    decimal disc2Percent = string.IsNullOrEmpty(discvalltxtbox.Text) ? 0 : decimal.Parse(discvalltxtbox.Text);
                    decimal taxPercent = string.IsNullOrEmpty(textBox1.Text) ? 0 : decimal.Parse(textBox1.Text);

                    decimal equivalentPacks = multiplier > 0 ? qtyL / multiplier : 0;
                    decimal totalQty = qtyP + equivalentPacks;

                    // Calculate step by step
                    decimal totalValueBeforeDiscount = totalQty * price;
                    decimal discountValue = totalValueBeforeDiscount * (discPercent / 100);
                    discvaltxtbox.Text = discountValue.ToString("N2");

                    decimal valueAfterFirstDiscount = totalValueBeforeDiscount - discountValue;
                    decimal discount2Value = valueAfterFirstDiscount * (disc2Percent / 100);

                    decimal valueAfterAllDiscounts = valueAfterFirstDiscount - discount2Value;
                    decimal taxValue = valueAfterAllDiscounts * (taxPercent / 100);
                    salestaxtxtbox.Text = taxValue.ToString("N2");

                    decimal totalFinalValue = valueAfterAllDiscounts + taxValue;
                    valuetxtbox.Text = totalFinalValue.ToString("N2");
                }
            }
            catch (Exception)
            {
                valuetxtbox.Text = "0.00";
                discvaltxtbox.Text = "0.00";
                salestaxtxtbox.Text = "0.00";
            }
        }

        private void AddProductToGrid()
        {
            try
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

                PackingItem selectedPacking = (PackingItem)packnamecombobox.SelectedItem;
                int multiplier = selectedPacking.Multiplier;
                string packingName = selectedPacking.PackingName;
                int packingId = selectedPacking.PackingId;

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

                int bonus = 0;
                if (!string.IsNullOrEmpty(bnsltxtbox.Text) && int.TryParse(bnsltxtbox.Text, out int parsedBonus))
                {
                    bonus = parsedBonus;
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

                decimal disc2Percent = 0;
                if (!string.IsNullOrEmpty(discvalltxtbox.Text) && decimal.TryParse(discvalltxtbox.Text, out decimal parsedDisc2Percent))
                {
                    disc2Percent = parsedDisc2Percent;
                }

                decimal discValue = 0;
                if (!string.IsNullOrEmpty(discvaltxtbox.Text) && decimal.TryParse(discvaltxtbox.Text, out decimal parsedDiscValue))
                {
                    discValue = parsedDiscValue;
                }

                decimal taxPercent = 0;
                if (!string.IsNullOrEmpty(textBox1.Text) && decimal.TryParse(textBox1.Text, out decimal parsedTaxPercent))
                {
                    taxPercent = parsedTaxPercent;
                }

                decimal taxValue = 0;
                if (!string.IsNullOrEmpty(salestaxtxtbox.Text) && decimal.TryParse(salestaxtxtbox.Text, out decimal parsedTaxValue))
                {
                    taxValue = parsedTaxValue;
                }

                decimal totalValue = 0;
                if (!string.IsNullOrEmpty(valuetxtbox.Text) && decimal.TryParse(valuetxtbox.Text, out decimal parsedTotalValue))
                {
                    totalValue = parsedTotalValue;
                }

                DataRow row = invoiceDetails.NewRow();
                row["Pro ID"] = productidtxtbox.Text.Trim();
                row["Product Name"] = productnametxtbox.Text;
                row["Pack Name"] = packingName;
                row["Pack"] = multiplier;
                row["Qty(P)"] = qtyP;
                row["Qty(L)"] = qtyL;
                row["Bns(L)"] = bonus;
                row["Price"] = price;
                row["Disc%"] = discPercent;
                row["Disc 2%"] = disc2Percent;
                row["Disc.Val"] = discValue;
                row["S-T%"] = taxPercent;
                row["S-T Val"] = taxValue;
                row["TTL Value"] = totalValue;
                row["PackingId"] = packingId;

                invoiceDetails.Rows.Add(row);
                UpdateTotals();
                ClearProductFields();
                productidtxtbox.Focus();

                MessageBox.Show("Product added to grid!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message);
            }
        }

        private void UpdateTotals()
        {
            grossTotal = 0;
            salesTaxTotal = 0;
            discountTotal = 0;
            discount2Total = 0;

            foreach (DataRow row in invoiceDetails.Rows)
            {
                grossTotal += Convert.ToDecimal(row["TTL Value"]);
                discountTotal += Convert.ToDecimal(row["Disc.Val"]);
                salesTaxTotal += Convert.ToDecimal(row["S-T Val"]);
                discount2Total += Convert.ToDecimal(row["Disc 2%"]);
            }

            grosstxtbox.Text = grossTotal.ToString("N2");
            discounttxtbox.Text = discountTotal.ToString("N2");
            salestxtbox.Text = salesTaxTotal.ToString("N2");
            disc2txtbox.Text = discount2Total.ToString("N2");

            decimal specialDiscount = 0;
            if (!string.IsNullOrEmpty(specialdisctxtbox.Text) && decimal.TryParse(specialdisctxtbox.Text, out decimal parsedSpecialDiscount))
            {
                specialDiscount = parsedSpecialDiscount;
            }

            decimal freightExpense = 0;
            if (!string.IsNullOrEmpty(freightexpensetxtbox.Text) && decimal.TryParse(freightexpensetxtbox.Text, out decimal parsedFreightExpense))
            {
                freightExpense = parsedFreightExpense;
            }

            decimal netValue = grossTotal + salesTaxTotal - discountTotal - specialDiscount + freightExpense;
            netvaluetxtbox.Text = netValue.ToString("N2");

            decimal cashReceived = 0;
            if (!string.IsNullOrEmpty(cashrectxtbox.Text) && decimal.TryParse(cashrectxtbox.Text, out decimal parsedCashReceived))
            {
                cashReceived = parsedCashReceived;
            }

            decimal balance = previousBalance + netValue - cashReceived;
            balancetxtbox.Text = balance.ToString("N2");
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
            discvalltxtbox.Clear();
            discvaltxtbox.Clear();
            textBox1.Clear();
            salestaxtxtbox.Clear();
            valuetxtbox.Clear();

            qtyptxtbox.Enabled = false;
            qtyltxtbox.Enabled = false;
            currentPackingId = 0;
        }

        private void CalculateRemainingBalance()
        {
            if (decimal.TryParse(netvaluetxtbox.Text, out decimal netValue) &&
                decimal.TryParse(prevbaltxtbox.Text, out decimal prevBal) &&
                decimal.TryParse(cashrectxtbox.Text, out decimal cashRec))
            {
                decimal remainingBalance = prevBal + netValue - cashRec;
                balancetxtbox.Text = remainingBalance.ToString("N2");
            }
        }

        private void cashrectxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRemainingBalance();
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateInvoice())
            {
                if (isUpdateMode)
                {
                    // Agar Sale ID Pendings table me hai to Pendings se Sales me move karo
                    if (IsPendingInvoice(currentSaleId))
                    {
                        MovePendingToSales(); // Pendings se Sales me move karo aur Pendings se delete karo
                    }
                    else
                    {
                        UpdateInvoice(); // Sales table me existing invoice update karo
                    }
                }
                else
                {
                    SaveInvoice(); // New invoice save karo
                }
            }
        }

        private void MovePendingToSales()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // 1. Generate NEW Sale ID for Sales table
                    SqlCommand getNewSaleIdCmd = new SqlCommand("SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Sales", con, tran);
                    int newSaleId = Convert.ToInt32(getNewSaleIdCmd.ExecuteScalar());

                    // 2. Get data from Pendings
                    string getPendingData = @"
                SELECT SaleDate, BillNo, CustomerId, SectorID, SalesmanID, 
                       Narration, SpecialDiscount, ReceivedAmount, PreviousCredit, 
                       UserNo, CustomerName, CashReceived, DueDate
                FROM Pendings 
                WHERE SaleId = @PendingSaleId";

                    SqlCommand cmdGetData = new SqlCommand(getPendingData, con, tran);
                    cmdGetData.Parameters.AddWithValue("@PendingSaleId", currentSaleId);

                    SqlDataReader reader = cmdGetData.ExecuteReader();
                    if (!reader.Read())
                    {
                        reader.Close();
                        throw new Exception("Pending invoice not found!");
                    }

                    DateTime saleDate = Convert.ToDateTime(reader["SaleDate"]);
                    object billNo = reader["BillNo"];
                    string customerId = reader["CustomerId"].ToString();
                    object sectorId = reader["SectorID"];
                    object salesmanId = reader["SalesmanID"];
                    object narration = reader["Narration"];
                    decimal specialDiscount = Convert.ToDecimal(reader["SpecialDiscount"]);
                    decimal receivedAmount = Convert.ToDecimal(reader["ReceivedAmount"]);
                    decimal previousCredit = Convert.ToDecimal(reader["PreviousCredit"]);
                    int userNo = Convert.ToInt32(reader["UserNo"]);
                    string customerName = reader["CustomerName"].ToString();
                    decimal cashReceived = Convert.ToDecimal(reader["CashReceived"]);
                    object dueDate = reader["DueDate"];

                    reader.Close();

                    // 3. Insert into Sales with NEW Sale ID
                    string insertSales = @"
                INSERT INTO Sales 
                (SaleId, SaleDate, BillNo, CustomerId, SectorID, SalesmanId, IsStockist, OldIsStockist,
                 Narration, Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax,
                 InvOfferFTax, ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo,
                 BiltyNo, TransportName, OrderId, CustomerName, CustomerNameUrdu, CashReceived,
                 Expenses, DueDate, InvDiscRatio, CashReturn, IsReplaced, ReplacementId,
                 ReplacementDate, RepId, BillYear, IsPendingRate, DistributorPartyId,
                 WithPartyDescription, PartyDescription, BiltyDate, ScannedDocuments,
                 FreightExpenses, IsLocked)
                VALUES 
                (@NewSaleId, @SaleDate, @BillNo, @CustomerId, @SectorID, @SalesmanId, 0, NULL,
                 @Narration, 0, 0, @SpecialDiscount, NULL, NULL,
                 NULL, @ReceivedAmount, @PreviousCredit, NULL, 1, @UserNo,
                 NULL, NULL, NULL, @CustomerName, NULL, @CashReceived,
                 NULL, @DueDate, 0, NULL, 0, NULL, NULL, NULL, NULL, 
                 @IsPendingRate, NULL, 0, NULL, NULL, NULL, @FreightExpenses, 1)";

                    SqlCommand cmdSales = new SqlCommand(insertSales, con, tran);

                    cmdSales.Parameters.AddWithValue("@NewSaleId", newSaleId);
                    cmdSales.Parameters.AddWithValue("@SaleDate", saleDate);
                    cmdSales.Parameters.AddWithValue("@BillNo", billNo != DBNull.Value ? billNo : newSaleId);
                    cmdSales.Parameters.AddWithValue("@CustomerId", customerId);
                    cmdSales.Parameters.AddWithValue("@SectorID", sectorId != DBNull.Value ? sectorId : DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@SalesmanId", salesmanId != DBNull.Value ? salesmanId : DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@Narration", narration != DBNull.Value ? narration : DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@SpecialDiscount", specialDiscount);
                    cmdSales.Parameters.AddWithValue("@ReceivedAmount", receivedAmount);
                    cmdSales.Parameters.AddWithValue("@PreviousCredit", previousCredit);
                    cmdSales.Parameters.AddWithValue("@UserNo", userNo);
                    cmdSales.Parameters.AddWithValue("@CustomerName", customerName);
                    cmdSales.Parameters.AddWithValue("@CashReceived", cashReceived);
                    cmdSales.Parameters.AddWithValue("@DueDate", dueDate != DBNull.Value ? dueDate : DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@IsPendingRate", pendingratechkbox.Checked ? 1 : 0);

                    decimal freightExpenses = string.IsNullOrEmpty(freightexpensetxtbox.Text) ? 0 : decimal.Parse(freightexpensetxtbox.Text);
                    cmdSales.Parameters.AddWithValue("@FreightExpenses", freightExpenses);

                    cmdSales.ExecuteNonQuery();

                    // 4. Copy items from PendingBody to SalesBody with NEW Sale ID
                    string copyItems = @"
                INSERT INTO SalesBody 
                (SaleId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, PricePack,
                 DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, IsValueB4Ratio,
                 IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns,
                 ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId, OfferDiscount,
                 IsDeleted, WeightLose, Disc2Ratio)
                SELECT 
                    @NewSaleId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, PricePack,
                    DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, IsValueB4Ratio,
                    IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns,
                    ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId, OfferDiscount,
                    IsDeleted, 0, 0
                FROM PendingBody 
                WHERE SaleId = @PendingSaleId";

                    SqlCommand cmdCopyItems = new SqlCommand(copyItems, con, tran);
                    cmdCopyItems.Parameters.AddWithValue("@NewSaleId", newSaleId);
                    cmdCopyItems.Parameters.AddWithValue("@PendingSaleId", currentSaleId);
                    cmdCopyItems.ExecuteNonQuery();

                    // 5. Delete from Pendings (original pending ID)
                    string deletePending = "DELETE FROM Pendings WHERE SaleId = @PendingSaleId";
                    SqlCommand cmdDeletePending = new SqlCommand(deletePending, con, tran);
                    cmdDeletePending.Parameters.AddWithValue("@PendingSaleId", currentSaleId);
                    cmdDeletePending.ExecuteNonQuery();

                    // 6. Delete from PendingBody (original pending ID)
                    string deletePendingBody = "DELETE FROM PendingBody WHERE SaleId = @PendingSaleId";
                    SqlCommand cmdDeletePendingBody = new SqlCommand(deletePendingBody, con, tran);
                    cmdDeletePendingBody.Parameters.AddWithValue("@PendingSaleId", currentSaleId);
                    cmdDeletePendingBody.ExecuteNonQuery();

                    // 7. Update customer balance
                    decimal netValue = decimal.Parse(netvaluetxtbox.Text);
                    UpdateCustomerBalance(customerId, previousCredit + netValue - receivedAmount, con, tran);

                    tran.Commit();

                    MessageBox.Show($"✅ Pending Invoice complete!\n\n" +
                                  //$"Old Pending ID: {currentSaleId}\n" +
                                  $"New Sales ID: {newSaleId}\n\n",
                                  //$"Invoice has been moved from Pendings to Sales table.",
                                  "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 8. Update form with new Sale ID
                    saleidtxtbox.Text = newSaleId.ToString();
                    currentSaleId = newSaleId;

                    // 9. Mark as updated in Sales table (not pending anymore)
                    isUpdateMode = true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show($"Error converting pending to sales: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool IsPendingInvoice(int saleId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pendings WHERE SaleId = @SaleId", con);
                    cmd.Parameters.AddWithValue("@SaleId", saleId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateInvoice()
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer");
                customeridtxtbox.Focus();
                return false;
            }

            if (invoiceDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product");
                productidtxtbox.Focus();
                return false;
            }

            return true;
        }

        private void SaveInvoice()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    decimal previousCredit = decimal.Parse(prevbaltxtbox.Text);
                    decimal receivedAmount = decimal.Parse(cashrectxtbox.Text);
                    decimal specialDiscount = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : decimal.Parse(specialdisctxtbox.Text);
                    decimal freightExpenses = string.IsNullOrEmpty(freightexpensetxtbox.Text) ? 0 : decimal.Parse(freightexpensetxtbox.Text);

                    string insertSales = @"
                    INSERT INTO Sales 
                    (SaleId, SaleDate, BillNo, CustomerId, SectorID, SalesmanId, IsStockist, OldIsStockist,
                     Narration, Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax,
                     InvOfferFTax, ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo,
                     BiltyNo, TransportName, OrderId, CustomerName, CustomerNameUrdu, CashReceived,
                     Expenses, DueDate, InvDiscRatio, CashReturn, IsReplaced, ReplacementId,
                     ReplacementDate, RepId, BillYear, IsPendingRate, DistributorPartyId,
                     WithPartyDescription, PartyDescription, BiltyDate, ScannedDocuments,
                     FreightExpenses, IsLocked)
                    VALUES 
                    (@SaleId, @SaleDate, @BillNo, @CustomerId, @SectorID, @SalesmanId, @IsStockist, @OldIsStockist,
                     @Narration, @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax,
                     @InvOfferFTax, @ReceivedAmount, @PreviousCredit, @InvoiceNotes, @IsPosted, @UserNo,
                     @BiltyNo, @TransportName, @OrderId, @CustomerName, @CustomerNameUrdu, @CashReceived,
                     @Expenses, @DueDate, @InvDiscRatio, @CashReturn, @IsReplaced, @ReplacementId,
                     @ReplacementDate, @RepId, @BillYear, @IsPendingRate, @DistributorPartyId,
                     @WithPartyDescription, @PartyDescription, @BiltyDate, @ScannedDocuments,
                     @FreightExpenses, @IsLocked)";

                    SqlCommand cmdSales = new SqlCommand(insertSales, con, tran);

                    cmdSales.Parameters.AddWithValue("@SaleId", currentSaleId);
                    cmdSales.Parameters.AddWithValue("@SaleDate", saledatepcker.Value);
                    cmdSales.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnootxtbox.Text) ? currentSaleId : int.Parse(billnootxtbox.Text));
                    cmdSales.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

                    int sectorId = GetSectorIdFromCustomer(customeridtxtbox.Text);
                    cmdSales.Parameters.AddWithValue("@SectorID", sectorId > 0 ? (object)sectorId : DBNull.Value);

                    cmdSales.Parameters.AddWithValue("@SalesmanId", string.IsNullOrEmpty(salesmenidtxtbox.Text) ? DBNull.Value : (object)salesmenidtxtbox.Text);
                    cmdSales.Parameters.AddWithValue("@IsStockist", 0);
                    cmdSales.Parameters.AddWithValue("@OldIsStockist", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
                    cmdSales.Parameters.AddWithValue("@Disc2Ratio", 0m);
                    cmdSales.Parameters.AddWithValue("@FTaxRatio", 0m);
                    cmdSales.Parameters.AddWithValue("@SpecialDiscount", specialDiscount);
                    cmdSales.Parameters.AddWithValue("@InvOfferDiscount", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@InvOfferSTax", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@InvOfferFTax", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@ReceivedAmount", receivedAmount);
                    cmdSales.Parameters.AddWithValue("@PreviousCredit", previousCredit);
                    cmdSales.Parameters.AddWithValue("@InvoiceNotes", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@IsPosted", 1);
                    cmdSales.Parameters.AddWithValue("@UserNo", 1);

                    cmdSales.Parameters.AddWithValue("@BiltyNo", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@TransportName", DBNull.Value);

                    if (!string.IsNullOrEmpty(orderidtxtbox.Text) && int.TryParse(orderidtxtbox.Text, out int orderId))
                    {
                        cmdSales.Parameters.AddWithValue("@OrderId", orderId);
                    }
                    else
                    {
                        cmdSales.Parameters.AddWithValue("@OrderId", DBNull.Value);
                    }

                    cmdSales.Parameters.AddWithValue("@CustomerName", customernametxtbox.Text);
                    cmdSales.Parameters.AddWithValue("@CustomerNameUrdu", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@CashReceived", receivedAmount);
                    cmdSales.Parameters.AddWithValue("@Expenses", DBNull.Value);

                    if (!string.IsNullOrEmpty(duedatetxtbox.Text) && DateTime.TryParse(duedatetxtbox.Text, out DateTime dueDate))
                    {
                        cmdSales.Parameters.AddWithValue("@DueDate", dueDate);
                    }
                    else
                    {
                        cmdSales.Parameters.AddWithValue("@DueDate", DBNull.Value);
                    }

                    cmdSales.Parameters.AddWithValue("@InvDiscRatio", 0m);
                    cmdSales.Parameters.AddWithValue("@CashReturn", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@IsReplaced", 0);
                    cmdSales.Parameters.AddWithValue("@ReplacementId", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@ReplacementDate", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@RepId", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@BillYear", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@IsPendingRate", pendingratechkbox.Checked ? 1 : 0);

                    if (!string.IsNullOrEmpty(distributornamecombobox.Text))
                    {
                        string distributorId = GetDistributorIdFromName(distributornamecombobox.Text);
                        if (!string.IsNullOrEmpty(distributorId))
                        {
                            cmdSales.Parameters.AddWithValue("@DistributorPartyId", distributorId);
                        }
                        else
                        {
                            cmdSales.Parameters.AddWithValue("@DistributorPartyId", DBNull.Value);
                        }
                    }
                    else
                    {
                        cmdSales.Parameters.AddWithValue("@DistributorPartyId", DBNull.Value);
                    }

                    cmdSales.Parameters.AddWithValue("@WithPartyDescription", withpartydescriptionchkbox.Checked ? 1 : 0);
                    cmdSales.Parameters.AddWithValue("@PartyDescription", DBNull.Value);
                    cmdSales.Parameters.AddWithValue("@BiltyDate", DBNull.Value);
                    cmdSales.Parameters.Add("@ScannedDocuments", SqlDbType.Image).Value = new byte[0];
                    cmdSales.Parameters.AddWithValue("@FreightExpenses", freightExpenses);
                    cmdSales.Parameters.AddWithValue("@IsLocked", 1);

                    cmdSales.ExecuteNonQuery();

                    foreach (DataRow row in invoiceDetails.Rows)
                    {
                        string insertSalesBody = @"
                        INSERT INTO SalesBody 
                        (SaleId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, PricePack,
                         DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, IsValueB4Ratio,
                         IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns,
                         ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId, OfferDiscount,
                         IsDeleted, WeightLose, Disc2Ratio)
                        VALUES 
                        (@SaleId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose, @PricePack,
                         @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack, @IsValueB4Ratio,
                         @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns,
                         @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId, @OfferDiscount,
                         @IsDeleted, @WeightLose, @Disc2Ratio)";

                        SqlCommand cmdSalesBody = new SqlCommand(insertSalesBody, con, tran);

                        string productId = row["Pro ID"].ToString().Trim();
                        if (productId.Length > 5) productId = productId.Substring(0, 5);

                        cmdSalesBody.Parameters.AddWithValue("@SaleId", currentSaleId);
                        cmdSalesBody.Parameters.AddWithValue("@ProductId", productId);
                        cmdSalesBody.Parameters.AddWithValue("@PackingId", row["PackingId"]);
                        cmdSalesBody.Parameters.AddWithValue("@Multiplier", row["Pack"]);
                        cmdSalesBody.Parameters.AddWithValue("@QtyPack", row["Qty(P)"]);
                        cmdSalesBody.Parameters.AddWithValue("@QtyLose", row["Qty(L)"]);
                        cmdSalesBody.Parameters.AddWithValue("@BnsLose", row["Bns(L)"]);
                        cmdSalesBody.Parameters.AddWithValue("@PricePack", row["Price"]);
                        cmdSalesBody.Parameters.AddWithValue("@DiscRatio", row["Disc%"]);
                        cmdSalesBody.Parameters.AddWithValue("@DiscValPack", row["Disc.Val"]);
                        cmdSalesBody.Parameters.AddWithValue("@DiscValLose", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@STaxRatio", row["S-T%"]);
                        cmdSalesBody.Parameters.AddWithValue("@STaxValPack", row["S-T Val"]);
                        cmdSalesBody.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                        cmdSalesBody.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                        cmdSalesBody.Parameters.AddWithValue("@IsFTaxExempted", 0);
                        cmdSalesBody.Parameters.AddWithValue("@TTLValue", row["TTL Value"]);
                        cmdSalesBody.Parameters.AddWithValue("@TTLSTax", row["S-T Val"]);
                        cmdSalesBody.Parameters.AddWithValue("@TTLFTax", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@ExtraSTaxBns", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@ExtraFTaxBns", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@PerUnitCostLose", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@IsFreeProduct", 0);
                        cmdSalesBody.Parameters.AddWithValue("@PriceHistoryId", 0);
                        cmdSalesBody.Parameters.AddWithValue("@OfferDiscount", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@IsDeleted", 0);
                        cmdSalesBody.Parameters.AddWithValue("@WeightLose", 0m);
                        cmdSalesBody.Parameters.AddWithValue("@Disc2Ratio", row["Disc 2%"]);

                        cmdSalesBody.ExecuteNonQuery();
                    }

                    UpdateCustomerBalance(customeridtxtbox.Text, previousCredit + decimal.Parse(netvaluetxtbox.Text) - receivedAmount, con, tran);

                    tran.Commit();
                    MessageBox.Show($"Invoice Saved Successfully!\nSale ID: {currentSaleId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearForm();
                    GenerateSaleID();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Error saving invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateInvoice()
        {
            // Implementation for update mode
            MessageBox.Show("Update functionality not implemented yet.");
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
            }
            return 0;
        }

        private string GetDistributorIdFromName(string distributorName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT PartyID FROM Parties WHERE PartyName = @PartyName AND IsDistributor=1", con);
                    cmd.Parameters.AddWithValue("@PartyName", distributorName);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        private void UpdateCustomerBalance(string customerId, decimal newBalance, SqlConnection con, SqlTransaction tran)
        {
            try
            {
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM AccountsBalances WHERE AccountNo = @AccountNo", con, tran);
                checkCmd.Parameters.AddWithValue("@AccountNo", customerId);
                int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (exists > 0)
                {
                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE AccountsBalances SET Bal = @Balance, BalType = @BalType WHERE AccountNo = @AccountNo", con, tran);
                    updateCmd.Parameters.AddWithValue("@Balance", newBalance);
                    updateCmd.Parameters.AddWithValue("@BalType", newBalance >= 0 ? "DR" : "CR");
                    updateCmd.Parameters.AddWithValue("@AccountNo", customerId);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand insertCmd = new SqlCommand(
                        "INSERT INTO AccountsBalances (AccountNo, OpeningDebit, OpeningCredit, OpeningBal, OpeningBalType, Debit, Credit, Bal, BalType) " +
                        "VALUES (@AccountNo, 0, 0, 0, 'DR', 0, 0, @Balance, @BalType)", con, tran);
                    insertCmd.Parameters.AddWithValue("@AccountNo", customerId);
                    insertCmd.Parameters.AddWithValue("@Balance", newBalance);
                    insertCmd.Parameters.AddWithValue("@BalType", newBalance >= 0 ? "DR" : "CR");
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating customer balance: " + ex.Message);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            saleidtxtbox.Clear();
            billnootxtbox.Clear();
            orderidtxtbox.Clear();
            orderdatetxtbox.Clear();
            customeridtxtbox.Clear();
            customernametxtbox.Clear();
            sectortowntxtbox.Clear();
            salesmenidtxtbox.Clear();
            salesmennametxtbox.Clear();
            distributornamecombobox.SelectedIndex = -1;
            narrationtxtbox.Clear();
            duedatetxtbox.Clear();

            ClearProductFields();
            SetDefaultValues();
            invoiceDetails.Clear();

            isUpdateMode = false;
            GenerateSaleID();
            customeridtxtbox.Focus();
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(saleidtxtbox.Text))
            {
                MessageBox.Show("Please enter Sale ID to open");
                return;
            }

            try
            {
                isLoadingInvoice = true;
                isUpdateMode = true;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string masterQuery = @"
                    SELECT s.*, p.PartyName as CustomerName, sm.SalesmanName
                    FROM Sales s
                    LEFT JOIN Parties p ON s.CustomerId = p.PartyID
                    LEFT JOIN Salesmen sm ON s.SalesmanId = sm.SalesmanID
                    WHERE s.SaleId = @SaleId";

                    SqlCommand masterCmd = new SqlCommand(masterQuery, con);
                    masterCmd.Parameters.AddWithValue("@SaleId", saleidtxtbox.Text);

                    SqlDataReader reader = masterCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        saledatepcker.Value = Convert.ToDateTime(reader["SaleDate"]);
                        billnootxtbox.Text = reader["BillNo"].ToString();
                        orderidtxtbox.Text = reader["OrderId"] != DBNull.Value ? reader["OrderId"].ToString() : "";
                        customeridtxtbox.Text = reader["CustomerId"].ToString();
                        customernametxtbox.Text = reader["CustomerName"] != DBNull.Value ? reader["CustomerName"].ToString() : "";
                        salesmenidtxtbox.Text = reader["SalesmanId"] != DBNull.Value ? reader["SalesmanId"].ToString() : "";
                        salesmennametxtbox.Text = reader["SalesmanName"] != DBNull.Value ? reader["SalesmanName"].ToString() : "";
                        prevbaltxtbox.Text = Convert.ToDecimal(reader["PreviousCredit"]).ToString("N2");
                        cashrectxtbox.Text = Convert.ToDecimal(reader["ReceivedAmount"]).ToString("N2");
                        specialdisctxtbox.Text = reader["SpecialDiscount"] != DBNull.Value ? Convert.ToDecimal(reader["SpecialDiscount"]).ToString("N2") : "0.00";
                        freightexpensetxtbox.Text = reader["FreightExpenses"] != DBNull.Value ? Convert.ToDecimal(reader["FreightExpenses"]).ToString("N2") : "0.00";
                        narrationtxtbox.Text = reader["Narration"] != DBNull.Value ? reader["Narration"].ToString() : "";

                        if (reader["DueDate"] != DBNull.Value)
                        {
                            duedatetxtbox.Text = Convert.ToDateTime(reader["DueDate"]).ToShortDateString();
                        }

                        if (reader["DistributorPartyId"] != DBNull.Value)
                        {
                            string distributorId = reader["DistributorPartyId"].ToString();
                            distributornamecombobox.Text = GetDistributorNameFromId(distributorId);
                        }

                        pendingratechkbox.Checked = Convert.ToBoolean(reader["IsPendingRate"]);
                        withpartydescriptionchkbox.Checked = Convert.ToBoolean(reader["WithPartyDescription"]);
                    }
                    else
                    {
                        MessageBox.Show("Invoice not found!");
                        reader.Close();
                        isLoadingInvoice = false;
                        isUpdateMode = false;
                        return;
                    }
                    reader.Close();

                    invoiceDetails.Clear();
                    string detailQuery = @"
                    SELECT 
                        sb.ProductId,
                        p.ProductName,
                        pk.PackingName,
                        sb.Multiplier as Pack,
                        sb.QtyPack,
                        sb.QtyLose,
                        sb.BnsLose,
                        sb.PricePack,
                        sb.DiscRatio,
                        sb.Disc2Ratio,
                        sb.DiscValPack,
                        sb.STaxRatio,
                        sb.STaxValPack,
                        sb.TTLValue,
                        sb.PackingId
                    FROM SalesBody sb
                    INNER JOIN Products p ON sb.ProductId = p.ProductID
                    INNER JOIN Packings pk ON sb.PackingId = pk.PackingId
                    WHERE sb.SaleId = @SaleId AND (sb.IsDeleted = 0 OR sb.IsDeleted IS NULL)";

                    SqlCommand detailCmd = new SqlCommand(detailQuery, con);
                    detailCmd.Parameters.AddWithValue("@SaleId", saleidtxtbox.Text);

                    SqlDataReader detailReader = detailCmd.ExecuteReader();
                    while (detailReader.Read())
                    {
                        DataRow row = invoiceDetails.NewRow();
                        row["Pro ID"] = detailReader["ProductId"];
                        row["Product Name"] = detailReader["ProductName"];
                        row["Pack Name"] = detailReader["PackingName"];
                        row["Pack"] = detailReader["Pack"];
                        row["Qty(P)"] = detailReader["QtyPack"];
                        row["Qty(L)"] = detailReader["QtyLose"];
                        row["Bns(L)"] = detailReader["BnsLose"];
                        row["Price"] = detailReader["PricePack"];
                        row["Disc%"] = detailReader["DiscRatio"];
                        row["Disc 2%"] = detailReader["Disc2Ratio"];
                        row["Disc.Val"] = detailReader["DiscValPack"];
                        row["S-T%"] = detailReader["STaxRatio"];
                        row["S-T Val"] = detailReader["STaxValPack"];
                        row["TTL Value"] = detailReader["TTLValue"];
                        row["PackingId"] = detailReader["PackingId"];

                        invoiceDetails.Rows.Add(row);
                    }
                    detailReader.Close();

                    UpdateTotals();
                    MessageBox.Show("Invoice loaded successfully!");
                }

                isLoadingInvoice = false;
            }
            catch (Exception ex)
            {
                isLoadingInvoice = false;
                isUpdateMode = false;
                MessageBox.Show("Error opening invoice: " + ex.Message);
            }
        }

        private string GetDistributorNameFromId(string distributorId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT PartyName FROM Parties WHERE PartyID = @PartyID", con);
                    cmd.Parameters.AddWithValue("@PartyID", distributorId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
            return "";
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            //this.Close();
            this.Close();
        }

        private void openpendingbtn_Click(object sender, EventArgs e)
        {
            Search_Pending_Invoice sp = new Search_Pending_Invoice();
            sp.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
            }
        }

        private void netvaluetxtbox_TextChanged(object sender, EventArgs e)
        {
            CalculateRemainingBalance();
        }

        private void freightexpensetxtbox_TextChanged(object sender, EventArgs e)
        {
            UpdateTotals();
        }

        private void salestxtbox_TextChanged(object sender, EventArgs e)
        {
        }

        private void orderdatetxtbox_TextChanged(object sender, EventArgs e)
        {
        }

        private void pendingbtn_Click(object sender, EventArgs e)
        {
            if (ValidateForPending())
            {
                SaveToPendingTables();
            }
        }

        private bool ValidateForPending()
        {
            if (string.IsNullOrEmpty(customeridtxtbox.Text))
            {
                MessageBox.Show("Please select a customer first");
                customeridtxtbox.Focus();
                return false;
            }

            if (invoiceDetails.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product");
                productidtxtbox.Focus();
                return false;
            }

            return true;
        }

        private void SaveToPendingTables()
        {
            try
            {
                // Pehle Pendings table me save karo
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Generate Sale ID for pending
                    SqlCommand getMaxCmd = new SqlCommand("SELECT ISNULL(MAX(SaleId), 0) + 1 FROM Pendings", con);
                    int pendingSaleId = Convert.ToInt32(getMaxCmd.ExecuteScalar());

                    // Insert into Pendings table
                    string insertPending = @"
            INSERT INTO Pendings 
            (SaleId, SaleDate, BillNo, CustomerId, SectorID, SalesmanID, IsStockist, OldIsStockist,
             Narration, Disc2Ratio, FTaxRatio, SpecialDiscount, InvOfferDiscount, InvOfferSTax,
             InvOfferFTax, ReceivedAmount, PreviousCredit, InvoiceNotes, IsPosted, UserNo,
             BiltyNo, TransportName, CustomerName, CustomerNameUrdu, CashReceived, Expenses, DueDate, InvDiscRatio)
            VALUES 
            (@SaleId, @SaleDate, @BillNo, @CustomerId, @SectorID, @SalesmanID, @IsStockist, @OldIsStockist,
             @Narration, @Disc2Ratio, @FTaxRatio, @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax,
             @InvOfferFTax, @ReceivedAmount, @PreviousCredit, @InvoiceNotes, @IsPosted, @UserNo,
             @BiltyNo, @TransportName, @CustomerName, @CustomerNameUrdu, @CashReceived, @Expenses, @DueDate, @InvDiscRatio)";

                    SqlCommand cmdPending = new SqlCommand(insertPending, con);

                    // Add parameters
                    cmdPending.Parameters.AddWithValue("@SaleId", pendingSaleId);
                    cmdPending.Parameters.AddWithValue("@SaleDate", saledatepcker.Value);
                    cmdPending.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnootxtbox.Text) ? pendingSaleId : int.Parse(billnootxtbox.Text));
                    cmdPending.Parameters.AddWithValue("@CustomerId", customeridtxtbox.Text);

                    int sectorId = GetSectorIdFromCustomer(customeridtxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@SectorID", sectorId > 0 ? (object)sectorId : DBNull.Value);

                    cmdPending.Parameters.AddWithValue("@SalesmanID", string.IsNullOrEmpty(salesmenidtxtbox.Text) ? DBNull.Value : (object)salesmenidtxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@IsStockist", 0);
                    cmdPending.Parameters.AddWithValue("@OldIsStockist", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@Disc2Ratio", 0m);
                    cmdPending.Parameters.AddWithValue("@FTaxRatio", 0m);

                    decimal specialDiscount = string.IsNullOrEmpty(specialdisctxtbox.Text) ? 0 : decimal.Parse(specialdisctxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@SpecialDiscount", specialDiscount);
                    cmdPending.Parameters.AddWithValue("@InvOfferDiscount", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@InvOfferSTax", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@InvOfferFTax", DBNull.Value);

                    decimal cashReceived = string.IsNullOrEmpty(cashrectxtbox.Text) ? 0 : decimal.Parse(cashrectxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@ReceivedAmount", cashReceived);

                    decimal previousBalance = string.IsNullOrEmpty(prevbaltxtbox.Text) ? 0 : decimal.Parse(prevbaltxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@PreviousCredit", previousBalance);

                    cmdPending.Parameters.AddWithValue("@InvoiceNotes", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@IsPosted", 0); // Pending ke liye 0
                    cmdPending.Parameters.AddWithValue("@UserNo", 1);

                    cmdPending.Parameters.AddWithValue("@BiltyNo", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@TransportName", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@CustomerName", customernametxtbox.Text);
                    cmdPending.Parameters.AddWithValue("@CustomerNameUrdu", DBNull.Value);
                    cmdPending.Parameters.AddWithValue("@CashReceived", cashReceived);
                    cmdPending.Parameters.AddWithValue("@Expenses", DBNull.Value);

                    if (!string.IsNullOrEmpty(duedatetxtbox.Text) && DateTime.TryParse(duedatetxtbox.Text, out DateTime dueDate))
                    {
                        cmdPending.Parameters.AddWithValue("@DueDate", dueDate);
                    }
                    else
                    {
                        cmdPending.Parameters.AddWithValue("@DueDate", DBNull.Value);
                    }

                    cmdPending.Parameters.AddWithValue("@InvDiscRatio", 0m);

                    cmdPending.ExecuteNonQuery();

                    // Ab PendingBody me data insert karo
                    foreach (DataRow row in invoiceDetails.Rows)
                    {
                        string insertPendingBody = @"
                INSERT INTO PendingBody 
                (SaleId, ProductId, PackingId, Multiplier, QtyPack, QtyLose, BnsLose, PricePack,
                 DiscRatio, DiscValPack, DiscValLose, STaxRatio, STaxValPack, IsValueB4Ratio,
                 IsSTaxOnBns, IsFTaxExempted, TTLValue, TTLSTax, TTLFTax, ExtraSTaxBns,
                 ExtraFTaxBns, PerUnitCostLose, IsFreeProduct, PriceHistoryId, OfferDiscount,
                 IsDeleted)
                VALUES 
                (@SaleId, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose, @BnsLose, @PricePack,
                 @DiscRatio, @DiscValPack, @DiscValLose, @STaxRatio, @STaxValPack, @IsValueB4Ratio,
                 @IsSTaxOnBns, @IsFTaxExempted, @TTLValue, @TTLSTax, @TTLFTax, @ExtraSTaxBns,
                 @ExtraFTaxBns, @PerUnitCostLose, @IsFreeProduct, @PriceHistoryId, @OfferDiscount,
                 @IsDeleted)";

                        SqlCommand cmdPendingBody = new SqlCommand(insertPendingBody, con);

                        string productId = row["Pro ID"].ToString().Trim();
                        if (productId.Length > 5) productId = productId.Substring(0, 5);

                        cmdPendingBody.Parameters.AddWithValue("@SaleId", pendingSaleId);
                        cmdPendingBody.Parameters.AddWithValue("@ProductId", productId);
                        cmdPendingBody.Parameters.AddWithValue("@PackingId", row["PackingId"]);
                        cmdPendingBody.Parameters.AddWithValue("@Multiplier", row["Pack"]);
                        cmdPendingBody.Parameters.AddWithValue("@QtyPack", row["Qty(P)"]);
                        cmdPendingBody.Parameters.AddWithValue("@QtyLose", row["Qty(L)"]);
                        cmdPendingBody.Parameters.AddWithValue("@BnsLose", row["Bns(L)"]);
                        cmdPendingBody.Parameters.AddWithValue("@PricePack", row["Price"]);
                        cmdPendingBody.Parameters.AddWithValue("@DiscRatio", row["Disc%"]);
                        cmdPendingBody.Parameters.AddWithValue("@DiscValPack", row["Disc.Val"]);
                        cmdPendingBody.Parameters.AddWithValue("@DiscValLose", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@STaxRatio", row["S-T%"]);
                        cmdPendingBody.Parameters.AddWithValue("@STaxValPack", row["S-T Val"]);
                        cmdPendingBody.Parameters.AddWithValue("@IsValueB4Ratio", 0);
                        cmdPendingBody.Parameters.AddWithValue("@IsSTaxOnBns", 0);
                        cmdPendingBody.Parameters.AddWithValue("@IsFTaxExempted", 0);
                        cmdPendingBody.Parameters.AddWithValue("@TTLValue", row["TTL Value"]);
                        cmdPendingBody.Parameters.AddWithValue("@TTLSTax", row["S-T Val"]);
                        cmdPendingBody.Parameters.AddWithValue("@TTLFTax", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@ExtraSTaxBns", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@ExtraFTaxBns", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@PerUnitCostLose", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@IsFreeProduct", 0);
                        cmdPendingBody.Parameters.AddWithValue("@PriceHistoryId", 0);
                        cmdPendingBody.Parameters.AddWithValue("@OfferDiscount", 0m);
                        cmdPendingBody.Parameters.AddWithValue("@IsDeleted", 0);

                        cmdPendingBody.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Invoice saved to Pending successfully!\nPending Sale ID: {pendingSaleId}", "Success");

                    // Form clear karo
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving to pending: " + ex.Message, "Error");
            }
        }

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
        // SaleInvoice.cs me yeh methods add karo
        public void LoadPendingInvoice(int pendingId, DateTime saleDate, int billNo,
            string customerId, string customerName, string sector, string town,
            string salesmanId, string salesmanName, decimal previousCredit,
            decimal cashReceived, decimal specialDiscount, string narration)
        {
            // Enable update mode
            isUpdateMode = true;

            // Set form fields
            saleidtxtbox.Text = pendingId.ToString();
            currentSaleId = pendingId;

            saledatepcker.Value = saleDate;
            billnootxtbox.Text = billNo.ToString();

            customeridtxtbox.Text = customerId;
            customernametxtbox.Text = customerName;

            // Combine sector and town
            if (!string.IsNullOrEmpty(sector) && !string.IsNullOrEmpty(town))
                sectortowntxtbox.Text = $"{sector} / {town}";
            else if (!string.IsNullOrEmpty(sector))
                sectortowntxtbox.Text = sector;
            else if (!string.IsNullOrEmpty(town))
                sectortowntxtbox.Text = town;

            salesmenidtxtbox.Text = salesmanId;
            salesmennametxtbox.Text = salesmanName;

            prevbaltxtbox.Text = previousCredit.ToString("N2");
            cashrectxtbox.Text = cashReceived.ToString("N2");
            specialdisctxtbox.Text = specialDiscount.ToString("N2");
            narrationtxtbox.Text = narration;

            // Clear existing items
            invoiceDetails.Clear();

            // Update mode ko true karo
            isUpdateMode = true;
        }

        public void LoadPendingItems(DataTable itemsTable)
        {
            try
            {
                invoiceDetails.Clear();

                foreach (DataRow row in itemsTable.Rows)
                {
                    DataRow newRow = invoiceDetails.NewRow();

                    newRow["Pro ID"] = row["ProductId"];
                    newRow["Product Name"] = row["ProductName"];
                    newRow["Pack Name"] = row["PackingName"];
                    newRow["Pack"] = row["Multiplier"];
                    newRow["Qty(P)"] = row["QtyPack"];
                    newRow["Qty(L)"] = row["QtyLose"];
                    newRow["Bns(L)"] = row["BnsLose"];
                    newRow["Price"] = row["PricePack"];
                    newRow["Disc%"] = row["DiscRatio"];
                    newRow["Disc 2%"] = 0; // Default if not available
                    newRow["Disc.Val"] = row["DiscValPack"];
                    newRow["S-T%"] = row["STaxRatio"];
                    newRow["S-T Val"] = row["STaxValPack"];
                    newRow["TTL Value"] = row["TTLValue"];
                    newRow["PackingId"] = row["PackingId"];

                    invoiceDetails.Rows.Add(newRow);
                }

                UpdateTotals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending items: " + ex.Message);
            }
        }

        // SaleInvoice form me isUpdateMode variable add karo class level par:
        // private bool isUpdateMode = false;
        private void customeridsearch_Click(object sender, EventArgs e)
        {
          
            string customerId = customeridtxtbox.Text.Trim();

            if (string.IsNullOrEmpty(customerId))
            {
                MessageBox.Show("Please enter Customer ID");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Direct query - NO JOINS agar simple chahiye
                    string query = @"
                SELECT 
                    p.PartyName,
                    ISNULL(s.SectorName, '') AS SectorName,
                    ISNULL(t.TownName, '') AS TownName
                FROM Parties p
                LEFT JOIN Sectors s ON p.SectorID = s.SectorID
                LEFT JOIN Towns t ON s.TownID = t.TownID
                WHERE p.PartyID = @CustomerID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    object result = cmd.ExecuteScalar();

                    if (result != null) // Customer exists
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // Name
                            customernametxtbox.Text = reader["PartyName"].ToString();

                            // Sector/Town
                            string sector = reader["SectorName"].ToString();
                            string town = reader["TownName"].ToString();

                            // Combine karo agar dono available hain
                            if (!string.IsNullOrEmpty(sector) && !string.IsNullOrEmpty(town))
                            {
                                sectortowntxtbox.Text = $"{sector} / {town}";
                            }
                            else if (!string.IsNullOrEmpty(sector))
                            {
                                sectortowntxtbox.Text = sector;
                            }
                            else
                            {
                                sectortowntxtbox.Text = town;
                            }
                        }
                        reader.Close();

                        //MessageBox.Show("Customer found!");
                    }
                    else
                    {
                        MessageBox.Show("Invalid Customer ID!");
                        customernametxtbox.Clear();
                        sectortowntxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
    }
