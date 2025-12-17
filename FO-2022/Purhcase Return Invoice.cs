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
        private DataTable productsDataTable; // New: For storing products data
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
                        pk.PackingName,
                        p.PurchasePrice,
                        p.PurDiscRatio,
                        p.PurSTRatio
                    FROM Products p
                    LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                    LEFT JOIN Packings pk ON pp.PackingId = pk.PackingId
                    ORDER BY p.ProductID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                    // Create a new DataTable to store all products
                    productsDataTable = new DataTable();
                    adapter.Fill(productsDataTable);

                    // Show data in grid
                    DisplayProductsInGrid(productsDataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products data: " + ex.Message);
            }
        }

        private void DisplayProductsInGrid(DataTable productsTable)
        {
            
            // Clear existing data in dataTable
            dataTable.Clear();

            // Add rows from productsTable to dataTable
            foreach (DataRow productRow in productsTable.Rows)
            {
                DataRow newRow = dataTable.NewRow();
                newRow["ProductID"] = productRow["ProductID"].ToString();
                newRow["ProductName"] = productRow["ProductName"].ToString();

                // PackingName ko check karein
                string packingName = productRow["PackingName"] != DBNull.Value ?
                                    productRow["PackingName"].ToString() :
                                    "Not Set";
                newRow["Packing"] = packingName;

                newRow["QtyPack"] = 0;
                newRow["QtyLose"] = 0;
                newRow["BnsLose"] = 0;

                // Price ko safe way se handle karein
                decimal price = 0;
                if (productRow["PurchasePrice"] != DBNull.Value)
                    price = Convert.ToDecimal(productRow["PurchasePrice"]);
                newRow["PricePack"] = price;

                decimal discPercent = 0;
                decimal salesTaxPercent = 0;

                if (productRow["PurDiscRatio"] != DBNull.Value)
                    discPercent = Convert.ToDecimal(productRow["PurDiscRatio"]);

                if (productRow["PurSTRatio"] != DBNull.Value)
                    salesTaxPercent = Convert.ToDecimal(productRow["PurSTRatio"]);

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

                    // Add items to combobox
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
            // Set date format
            purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Load all products data in grid when form loads
            LoadAllProductsData();

            // Load packings data in combobox
            LoadPackingData();

            // Add Enter key event for productidtxtbox
            productidtxtbox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadProductData();
                }
            };

            // Add cell click event for dataGridView1
            dataGridView1.CellClick += DataGridView1_CellClick;

            // NEW: Add event handler for combobox selection change
            packnamecombobox.SelectedIndexChanged += Packnamecombobox_SelectedIndexChanged;
        }
        private void Packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Agar koi row select hai grid mein
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get selected row
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                int selectedIndex = selectedRow.Index;

                // Get selected packing name from combobox
                string selectedPacking = packnamecombobox.Text;

                // Update grid cell
                if (selectedIndex >= 0 && selectedIndex < dataTable.Rows.Count)
                {
                    dataTable.Rows[selectedIndex]["Packing"] = selectedPacking;

                    // Optional: Recalculate values if needed
                    CalculateTotals();
                }
            }

            // Ya phir agar product ID textbox mein product hai
            else if (!string.IsNullOrEmpty(productidtxtbox.Text))
            {
                // Product ID ke hisaab se grid mein row dhundhein
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["ProductID"].ToString() == productidtxtbox.Text)
                    {
                        // Update packing in grid
                        dataTable.Rows[i]["Packing"] = packnamecombobox.Text;

                        // Optional: Recalculate values if needed
                        CalculateTotals();
                        break;
                    }
                }
            }
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

           
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                // Get the selected row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Populate textboxes with selected row data
                productidtxtbox.Text = row.Cells["ProductID"].Value?.ToString() ?? "";
                productnametxtbox.Text = row.Cells["ProductName"].Value?.ToString() ?? "";

                // Set combobox value from grid - IMPORTANT
                string packingName = row.Cells["Packing"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(packingName))
                {
                    // Direct text set karein
                    packnamecombobox.Text = packingName;
                }
                else
                {
                    // Agar packing name nahi hai to first item select karein
                    if (packnamecombobox.Items.Count > 0)
                        packnamecombobox.SelectedIndex = 0;
                }

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
            
        }

        private void purchasedatetxtbo_TextChanged(object sender, EventArgs e)
        {
            // Date validation can be added here
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
                    COALESCE(pk.PackingName, 'Not Set') as PackingName,
                    pp.PackingId,
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

                            // Set combobox value based on PackingId
                            if (reader["PackingId"] != DBNull.Value)
                            {
                                int packingId = Convert.ToInt32(reader["PackingId"]);
                                SetComboBoxByPackingId(packingId);
                            }
                            else
                            {
                                packnamecombobox.SelectedIndex = -1;
                            }

                            // Get multiplier
                            int multiplier = 1;
                            if (reader["Multiplier"] != DBNull.Value)
                                multiplier = Convert.ToInt32(reader["Multiplier"]);

                            // Set default quantities
                            qtyptxtbox.Text = "0";
                            qtyltxtbox.Text = "0";
                            bnsltxtbox.Text = "0";

                            // Get prices and calculate
                            decimal price = reader["PurchasePrice"] != DBNull.Value ?
                                          Convert.ToDecimal(reader["PurchasePrice"]) : 0;
                            decimal discPercent = reader["PurDiscRatio"] != DBNull.Value ?
                                                Convert.ToDecimal(reader["PurDiscRatio"]) : 0;
                            decimal salesTaxPercent = reader["PurSTRatio"] != DBNull.Value ?
                                                    Convert.ToDecimal(reader["PurSTRatio"]) : 0;

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
        private void SetComboBoxByPackingId(int packingId)
        {
            try
            {
                foreach (DataRowView item in packnamecombobox.Items)
                {
                    if (Convert.ToInt32(item["PackingId"]) == packingId)
                    {
                        packnamecombobox.SelectedItem = item;
                        return;
                    }
                }

                // Agar packing ID nahi mila to first item select kar lein
                if (packnamecombobox.Items.Count > 0)
                    packnamecombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting combobox: " + ex.Message);
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

            // Validate packing selection
            if (packnamecombobox.SelectedIndex == -1 && string.IsNullOrEmpty(packnamecombobox.Text))
            {
                MessageBox.Show("Please select packing type");
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

                // Get packing name from combobox - FIXED
                string packingName = "";

                // Check if combobox has selected item
                if (packnamecombobox.SelectedItem != null)
                {
                    DataRowView selectedRow = packnamecombobox.SelectedItem as DataRowView;
                    if (selectedRow != null)
                    {
                        packingName = selectedRow["PackingName"].ToString();
                    }
                }
                // If no selected item, try to get from text
                else if (!string.IsNullOrEmpty(packnamecombobox.Text))
                {
                    packingName = packnamecombobox.Text;
                }
                else
                {
                    packingName = "Not Set";
                }

                // Add row to DataTable
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

            // Combobox ko reset karein
            if (packnamecombobox.Items.Count > 0)
                packnamecombobox.SelectedIndex = 0;

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

            // Clear data grid and reload products
            dataTable.Clear();
            LoadAllProductsData();

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

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // You can add logic here if needed when packing selection changes
            UpdateSelectedRowPacking();
        }
        private void UpdateSelectedRowPacking()
        {
            // Agar koi product select hai textboxes mein
            if (!string.IsNullOrEmpty(productidtxtbox.Text))
            {
                // Product ID ke hisaab se existing row dhundhein
                DataRow existingRow = null;
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["ProductID"].ToString() == productidtxtbox.Text)
                    {
                        existingRow = row;
                        break;
                    }
                }

                if (existingRow != null)
                {
                    // Update existing row
                    string packingName = packnamecombobox.Text;
                    existingRow["Packing"] = packingName;

                    // Recalculate totals
                    CalculateTotals();
                }
            }
        }
        // Add this method to refresh products data
        private void RefreshProductsData()
        {
            LoadAllProductsData();
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
                    string query = @"SELECT 
                PurchaseDate,
                BillDate,
                VendorID,
                BillNo
            FROM Purchase 
            WHERE PurchaseID = @PurchaseID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PurchaseID", Convert.ToInt32(purchaseidtxtbox.Text));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Purchase date lein
                            DateTime purchaseDate = Convert.ToDateTime(reader["PurchaseDate"]);
                            purchasedatetxtbox.Text = purchaseDate.ToString("dd/MM/yyyy");

                            // Bill date agar hai to
                            if (reader["BillDate"] != DBNull.Value)
                            {
                                DateTime billDate = Convert.ToDateTime(reader["BillDate"]);
                                // Bill date ko kisi aur textbox main show kar sakte hain
                                // Agar aap ko chahiye to
                            }

                            // Vendor ID automatic fill kar dein
                            vendoridtxtbox.Text = reader["VendorID"].ToString();

                            // Bill number automatic fill kar dein
                            if (reader["BillNo"] != DBNull.Value)
                            {
                                billnotxtbox.Text = reader["BillNo"].ToString();
                            }

                            // Ab vendor ka name bhi automatically fetch kar lein
                            LoadVendorName(reader["VendorID"].ToString());
                        }
                        else
                        {
                            MessageBox.Show("Purchase ID not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase data: " + ex.Message);
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

                    // Sab se pehle check karein ke ye vendor Parties table main hai
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
                        // Agar Parties main nahi mila to ChartOfAccounts main check karein
                        query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@AccountNo", vendorId);

                        result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            vendornametxtbox.Text = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Vendor ID not found in Parties or Chart of Accounts");
                            vendornametxtbox.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendor name: " + ex.Message);
            }
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
                        DateTime returnDate;

                        if (!DateTime.TryParseExact(purchasedatetxtbox.Text,
                            new string[] { "dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "yyyy-MM-dd" },
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out returnDate))
                        {
                            returnDate = DateTime.Now;
                            purchasedatetxtbox.Text = returnDate.ToString("dd/MM/yyyy");
                        }

                       
                        // MODIFIED QUERY - Corrected column name to PurchaseId
                        string headerQuery = @"INSERT INTO PurReturns 
    (PurReturnId, PurchaseId, VendorId, ReturnDate, Disc2Ratio, FTaxRatio, 
     SpecialDiscount, InvOfferDiscount, InvOfferSTax, InvOfferFTax,
     ReceivedAmount, PreviousCredit, Narration, IsPosted, UserNo, BillNo)
    VALUES 
    (@PurReturnId, @PurchaseId, @VendorId, @ReturnDate, @Disc2Ratio, @FTaxRatio,
     @SpecialDiscount, @InvOfferDiscount, @InvOfferSTax, @InvOfferFTax,
     @ReceivedAmount, @PreviousCredit, @Narration, @IsPosted, @UserNo, @BillNo)";

                        SqlCommand headerCmd = new SqlCommand(headerQuery, conn, transaction);
                        headerCmd.Parameters.AddWithValue("@PurReturnId", Convert.ToInt32(returnidtxtbox.Text));

                        // Correct parameter name
                        if (!string.IsNullOrEmpty(purchaseidtxtbox.Text) && int.TryParse(purchaseidtxtbox.Text, out int purchaseId))
                        {
                            headerCmd.Parameters.AddWithValue("@PurchaseId", purchaseId);
                        }
                        else
                        {
                            headerCmd.Parameters.AddWithValue("@PurchaseId", 0);
                        }

                        headerCmd.Parameters.AddWithValue("@VendorId", vendoridtxtbox.Text);
                        headerCmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                        headerCmd.Parameters.AddWithValue("@Disc2Ratio", 0);
                        headerCmd.Parameters.AddWithValue("@FTaxRatio", 0);
                        headerCmd.Parameters.AddWithValue("@SpecialDiscount", 0);
                        headerCmd.Parameters.AddWithValue("@InvOfferDiscount", 0);
                        headerCmd.Parameters.AddWithValue("@InvOfferSTax", 0);
                        headerCmd.Parameters.AddWithValue("@InvOfferFTax", 0);
                        headerCmd.Parameters.AddWithValue("@ReceivedAmount",
                            string.IsNullOrEmpty(receivedamounttxtbox.Text) ? 0 : Convert.ToDecimal(receivedamounttxtbox.Text));
                        headerCmd.Parameters.AddWithValue("@PreviousCredit", 0);
                        headerCmd.Parameters.AddWithValue("@Narration",
                            string.IsNullOrEmpty(narrationtxtbox.Text) ? DBNull.Value : (object)narrationtxtbox.Text);
                        headerCmd.Parameters.AddWithValue("@IsPosted", 1);
                        headerCmd.Parameters.AddWithValue("@UserNo", 1);

                        if (!string.IsNullOrEmpty(billnotxtbox.Text) && int.TryParse(billnotxtbox.Text, out int billNo))
                        {
                            headerCmd.Parameters.AddWithValue("@BillNo", billNo);
                        }
                        else
                        {
                            headerCmd.Parameters.AddWithValue("@BillNo", DBNull.Value);
                        }

                        headerCmd.ExecuteNonQuery();

                        // ... rest of your code for PurReturnsBody ...

                        transaction.Commit();
                        MessageBox.Show("Purchase Return saved successfully!");
                        ClearAllFields();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error in transaction: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving purchase return: " + ex.Message);
            }
        }

        

        private void purchaseidtxtbox_TextChanged_1(object sender, EventArgs e)
        {
            // Reset purchase date when purchase ID changes
            if (string.IsNullOrEmpty(purchaseidtxtbox.Text))
            {
                purchasedatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        private void clearbtn_Click_1(object sender, EventArgs e)
        {
            ClearAllFields();
        }
    }
    }
