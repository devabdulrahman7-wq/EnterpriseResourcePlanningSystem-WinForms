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
    public partial class Purchase_Invoice_01 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private DataTable productsDataTable;
        private int currentPurchaseId = 0;

        public Purchase_Invoice_01()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeDataGridView();
            AutoGeneratePurchaseID();
            SetCurrentDate();
            dayshftradiobtn.Checked = true;
            LoadAllPackingsInComboBox(); // Load all pack names
            LoadAllProductsInGrid(); // Load data when grid loads
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

            // Grid columns (same as textboxes)
            productsDataTable.Columns.Add("Pro ID", typeof(string));
            productsDataTable.Columns.Add("Product Name", typeof(string));
            productsDataTable.Columns.Add("Pack Name", typeof(string));
            productsDataTable.Columns.Add("Pack", typeof(int));
            productsDataTable.Columns.Add("Qty(P)", typeof(int));
            productsDataTable.Columns.Add("Qty(L)", typeof(int));
            productsDataTable.Columns.Add("Bns(L)", typeof(int));

            dataGridView1.DataSource = productsDataTable;
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void LoadAllPackingsInComboBox()
        {
            try
            {
                string query = @"SELECT PackingName FROM Packings ORDER BY PackingName";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        packnamecombobox.Items.Add(reader["PackingName"].ToString());
                    }
                }

                // Add default "Piece" option if exists
                if (!packnamecombobox.Items.Contains("Piece"))
                {
                    packnamecombobox.Items.Insert(0, "Piece");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packing names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllProductsInGrid()
        {
            try
            {
                productsDataTable.Clear();

                string query = @"SELECT p.ProductID, p.ProductName, pp.PackingId, pp.Multiplier,
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

                        newRow["Pro ID"] = reader["ProductID"].ToString();
                        newRow["Product Name"] = reader["ProductName"].ToString();

                        if (reader["PackingName"] != DBNull.Value)
                            newRow["Pack Name"] = reader["PackingName"].ToString();
                        else
                            newRow["Pack Name"] = "Piece";

                        if (reader["Multiplier"] != DBNull.Value)
                            newRow["Pack"] = Convert.ToInt32(reader["Multiplier"]);
                        else
                            newRow["Pack"] = 1;

                        newRow["Qty(P)"] = 0;
                        newRow["Qty(L)"] = 0;
                        newRow["Bns(L)"] = 0;

                        productsDataTable.Rows.Add(newRow);
                    }
                }

                // Add "Add to Grid" and "Update Grid" buttons
                //AddProductButtons();
            }
            catch (Exception ex)
            {
                //    MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void AddProductButtons()
        //{
        //    // Add to Grid button
        //    Button addBtn = new Button();
        //    addBtn.Text = "Add to Grid";
        //    addBtn.Location = new Point(700, 240);
        //    addBtn.Size = new Size(120, 25);
        //    addBtn.BackColor = Color.FromArgb(128, 128, 255);
        //    addBtn.FlatStyle = FlatStyle.Flat;
        //    addBtn.Font = new Font("Times New Roman", 9.75F, FontStyle.Bold);
        //    addBtn.Click += AddBtn_Click;
        //    this.Controls.Add(addBtn);

        //    // Update Grid button
        //    Button updateBtn = new Button();
        //    updateBtn.Text = "Update Grid";
        //    updateBtn.Location = new Point(700, 270);
        //    updateBtn.Size = new Size(120, 25);
        //    updateBtn.BackColor = Color.FromArgb(128, 128, 255);
        //    updateBtn.FlatStyle = FlatStyle.Flat;
        //    updateBtn.Font = new Font("Times New Roman", 9.75F, FontStyle.Bold);
        //    updateBtn.Click += UpdateBtn_Click;
        //    this.Controls.Add(updateBtn);
        //}

        ////private void AddBtn_Click(object sender, EventArgs e)
        ////{
        ////    AddProductToGrid();
        ////}

        //private void UpdateBtn_Click(object sender, EventArgs e)
        //{
        //    UpdateGridRow();
        //}

        // ==================== VENDOR SEARCH ====================
        private void vendoridsearch_Click(object sender, EventArgs e)
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
                    MessageBox.Show("Please enter Vendor ID", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // FIX: Use parameterized query with @PartyID
                string query = @"SELECT PartyName FROM Parties 
                        WHERE PartyID = @PartyID AND PartyType = 'V'";

                SqlCommand cmd = new SqlCommand(query, connection);

                // ADDED THIS LINE - Main fix
                cmd.Parameters.AddWithValue("@PartyID", vendorId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        vendornametxtbox.Text = reader["PartyName"].ToString();
                        productidtxtbox.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Vendor not found! Please check Vendor ID.", "Vendor Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        vendornametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching vendor: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== PRODUCT SEARCH ====================
        //private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        e.SuppressKeyPress = true;
        //    }
        //}

        private void SearchProduct()
        {
            try
            {
                string productId = productidtxtbox.Text.Trim();

                if (string.IsNullOrEmpty(productId))
                {
                    MessageBox.Show("Please enter Product ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    productidtxtbox.Focus();
                    return;
                }

                // Step 1: Get Product Name, Packing ID and Multiplier
                string productQuery = @"SELECT p.ProductID, p.ProductName, 
                                       pp.PackingId, pp.Multiplier,
                                       p.PurPackingId, p.SalPackingId
                                FROM Products p
                                LEFT JOIN ProductPacking pp ON p.ProductID = pp.ProductId
                                WHERE p.ProductID = @ProductID";

                string packingName = "";
                int multiplier = 1;
                string productName = "";

                using (SqlCommand cmd = new SqlCommand(productQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get Product Name
                            productName = reader["ProductName"].ToString();

                            // Get Multiplier
                            if (reader["Multiplier"] != DBNull.Value)
                                multiplier = Convert.ToInt32(reader["Multiplier"]);

                            // Get Packing ID (check purchase packing first, then sale packing)
                            int packingId = 0;
                            if (reader["PurPackingId"] != DBNull.Value && Convert.ToInt32(reader["PurPackingId"]) > 0)
                                packingId = Convert.ToInt32(reader["PurPackingId"]);
                            else if (reader["SalPackingId"] != DBNull.Value && Convert.ToInt32(reader["SalPackingId"]) > 0)
                                packingId = Convert.ToInt32(reader["SalPackingId"]);
                            else if (reader["PackingId"] != DBNull.Value)
                                packingId = Convert.ToInt32(reader["PackingId"]);

                            reader.Close(); // Close reader first

                            // Step 2: Get Packing Name
                            if (packingId > 0)
                            {
                                string packingQuery = @"SELECT PackingName FROM Packings WHERE PackingId = @PackingId";
                                using (SqlCommand packingCmd = new SqlCommand(packingQuery, connection))
                                {
                                    packingCmd.Parameters.AddWithValue("@PackingId", packingId);
                                    object result = packingCmd.ExecuteScalar();
                                    if (result != null)
                                        packingName = result.ToString();
                                }
                            }

                            // Step 3: Fill textboxes
                            productnametxtbox.Text = productName;
                            packtxtbox.Text = multiplier.ToString();

                            // Set combo box
                            if (!string.IsNullOrEmpty(packingName))
                            {
                                if (packnamecombobox.Items.Contains(packingName))
                                    packnamecombobox.SelectedItem = packingName;
                                else
                                    packnamecombobox.SelectedIndex = 0; // Default
                            }
                            else
                            {
                                packnamecombobox.SelectedIndex = 0; // Default "Piece"
                            }

                            // Set default quantities
                            qtyptxtbox.Text = "0";
                            qtyltxtbox.Text = "0";
                            bnsltxtbox.Text = "0";

                            // Focus on quantity
                            qtyptxtbox.Focus();
                            qtyptxtbox.SelectAll();
                        }
                        else
                        {
                            MessageBox.Show("Product not found! Please check Product ID.", "Product Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClearProductTextboxes();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ==================== ADD PRODUCT TO GRID ====================
        private void AddProductToGrid()
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(qtyptxtbox.Text) && string.IsNullOrEmpty(qtyltxtbox.Text) && string.IsNullOrEmpty(bnsltxtbox.Text))
                {
                    MessageBox.Show("Please enter at least one quantity", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get values from textboxes
                string proId = productidtxtbox.Text;
                string productName = productnametxtbox.Text;
                string packName = packnamecombobox.SelectedItem?.ToString() ?? "Piece";
                int pack = string.IsNullOrEmpty(packtxtbox.Text) ? 1 : Convert.ToInt32(packtxtbox.Text);
                int qtyP = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                int qtyL = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
                int bnsL = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);

                // Check if product already exists in grid
                bool productExists = false;
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row["Pro ID"].ToString() == proId)
                    {
                        // Update existing row
                        row["Qty(P)"] = Convert.ToInt32(row["Qty(P)"]) + qtyP;
                        row["Qty(L)"] = Convert.ToInt32(row["Qty(L)"]) + qtyL;
                        row["Bns(L)"] = Convert.ToInt32(row["Bns(L)"]) + bnsL;
                        productExists = true;
                        break;
                    }
                }

                if (!productExists)
                {
                    // Add new row
                    DataRow newRow = productsDataTable.NewRow();
                    newRow["Pro ID"] = proId;
                    newRow["Product Name"] = productName;
                    newRow["Pack Name"] = packName;
                    newRow["Pack"] = pack;
                    newRow["Qty(P)"] = qtyP;
                    newRow["Qty(L)"] = qtyL;
                    newRow["Bns(L)"] = bnsL;

                    productsDataTable.Rows.Add(newRow);
                }

                // Clear product textboxes for next entry (but keep product ID)
                productnametxtbox.Clear();
                packnamecombobox.SelectedIndex = -1;
                packtxtbox.Clear();
                qtyptxtbox.Clear();
                qtyltxtbox.Clear();
                bnsltxtbox.Clear();

                // Keep product ID for next entry
                productidtxtbox.Focus();
                productidtxtbox.SelectAll();

                MessageBox.Show("Product added to grid successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to grid: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== GRID CLICK EVENT ====================
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // Fetch grid data to textboxes
                    productidtxtbox.Text = row.Cells["Pro ID"].Value?.ToString() ?? "";
                    productnametxtbox.Text = row.Cells["Product Name"].Value?.ToString() ?? "";

                    // Set Pack Name in combo box
                    string packName = row.Cells["Pack Name"].Value?.ToString() ?? "Piece";
                    if (packnamecombobox.Items.Contains(packName))
                    {
                        packnamecombobox.SelectedItem = packName;
                    }
                    else
                    {
                        packnamecombobox.SelectedIndex = 0; // Default "Piece"
                    }

                    packtxtbox.Text = row.Cells["Pack"].Value?.ToString() ?? "1";
                    qtyptxtbox.Text = row.Cells["Qty(P)"].Value?.ToString() ?? "0";
                    qtyltxtbox.Text = row.Cells["Qty(L)"].Value?.ToString() ?? "0";
                    bnsltxtbox.Text = row.Cells["Bns(L)"].Value?.ToString() ?? "0";

                    productidtxtbox.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading row data: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ==================== UPDATE ROW IN GRID ====================
        private void UpdateGridRow()
        {
            try
            {
                if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
                {
                    int rowIndex = dataGridView1.CurrentRow.Index;

                    // Update current row with textbox values
                    dataGridView1.Rows[rowIndex].Cells["Pro ID"].Value = productidtxtbox.Text;
                    dataGridView1.Rows[rowIndex].Cells["Product Name"].Value = productnametxtbox.Text;
                    dataGridView1.Rows[rowIndex].Cells["Pack Name"].Value = packnamecombobox.SelectedItem?.ToString() ?? "Piece";
                    dataGridView1.Rows[rowIndex].Cells["Pack"].Value = string.IsNullOrEmpty(packtxtbox.Text) ? 1 : Convert.ToInt32(packtxtbox.Text);
                    dataGridView1.Rows[rowIndex].Cells["Qty(P)"].Value = string.IsNullOrEmpty(qtyptxtbox.Text) ? 0 : Convert.ToInt32(qtyptxtbox.Text);
                    dataGridView1.Rows[rowIndex].Cells["Qty(L)"].Value = string.IsNullOrEmpty(qtyltxtbox.Text) ? 0 : Convert.ToInt32(qtyltxtbox.Text);
                    dataGridView1.Rows[rowIndex].Cells["Bns(L)"].Value = string.IsNullOrEmpty(bnsltxtbox.Text) ? 0 : Convert.ToInt32(bnsltxtbox.Text);

                    MessageBox.Show("Row updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please select a row to update", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating row: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== SAVE INVOICE ====================
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
                MessageBox.Show("Please enter Vendor ID!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendoridtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(vendornametxtbox.Text))
            {
                MessageBox.Show("Please search and select a valid vendor!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendoridtxtbox.Focus();
                return false;
            }

            if (productsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate each product has at least some quantity
            bool hasValidProducts = false;
            foreach (DataRow row in productsDataTable.Rows)
            {
                int qtyP = row["Qty(P)"] != DBNull.Value ? Convert.ToInt32(row["Qty(P)"]) : 0;
                int qtyL = row["Qty(L)"] != DBNull.Value ? Convert.ToInt32(row["Qty(L)"]) : 0;
                int bnsL = row["Bns(L)"] != DBNull.Value ? Convert.ToInt32(row["Bns(L)"]) : 0;

                if (qtyP > 0 || qtyL > 0 || bnsL > 0)
                {
                    hasValidProducts = true;
                    break;
                }
            }

            //if (!hasValidProducts)
            //{
            //    MessageBox.Show("All products have zero quantities. Please enter quantities.", "Validation",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return false;
            //}

            return true;
        }

        private void SavePurchaseInvoice()
        {
            SqlTransaction transaction = null;

            try
            {
                transaction = connection.BeginTransaction();

                // 1. Save to Purchase table
                string purchaseQuery = @"INSERT INTO Purchase (PurchaseID, PurchaseDate, VendorID, BillNo, BillDate, 
                               PaidAmount, Narration, DueDate, UserNo, IsPendingRate, IsNightShift)
                               VALUES (@PurchaseID, @PurchaseDate, @VendorID, @BillNo, @BillDate, 
                               0, @Narration, @DueDate, 1, @IsPendingRate, @IsNightShift)";

                SqlCommand purchaseCmd = new SqlCommand(purchaseQuery, connection, transaction);
                purchaseCmd.Parameters.AddWithValue("@PurchaseID", currentPurchaseId);
                purchaseCmd.Parameters.AddWithValue("@PurchaseDate", purchasedatepicker.Value);
                purchaseCmd.Parameters.AddWithValue("@VendorID", vendoridtxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillNo", string.IsNullOrEmpty(billnotxtbox.Text) ? (object)DBNull.Value : billnotxtbox.Text);
                purchaseCmd.Parameters.AddWithValue("@BillDate", billdatepicker.Value);
                purchaseCmd.Parameters.AddWithValue("@Narration", string.IsNullOrEmpty(narrationtxtbox.Text) ? "" : narrationtxtbox.Text);

                // Date handling
                object dueDateParam = DBNull.Value;

                if (!string.IsNullOrWhiteSpace(duedatetxtbox.Text))
                {
                    string dateText = duedatetxtbox.Text.Trim();
                    DateTime dueDate;

                    if (DateTime.TryParse(dateText, out dueDate))
                    {
                        dueDateParam = dueDate;
                    }
                    else
                    {
                        string[] formats = { "dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };
                        if (DateTime.TryParseExact(dateText, formats,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None, out dueDate))
                        {
                            dueDateParam = dueDate;
                        }
                    }
                }

                purchaseCmd.Parameters.AddWithValue("@DueDate", dueDateParam);
                purchaseCmd.Parameters.AddWithValue("@IsPendingRate", pendignchkbox.Checked);
                purchaseCmd.Parameters.AddWithValue("@IsNightShift", nightshftradiobtn.Checked);

                purchaseCmd.ExecuteNonQuery();

                // 2. Save to PurchasesBody table
                foreach (DataRow row in productsDataTable.Rows)
                {
                    if (row.RowState != DataRowState.Deleted)
                    {
                        string proId = row["Pro ID"].ToString();
                        string packName = row["Pack Name"]?.ToString() ?? "Piece";

                        if (!string.IsNullOrEmpty(proId))
                        {
                            // Get PackingId from Packings table using Pack Name
                            int packingId = GetPackingIdFromName(packName, transaction);

                            string purchasesBodyQuery = @"INSERT INTO PurchasesBody 
                    (PurchaseId, ProductId, PackingId, QtyPack, QtyLose, BnsLose)
                    VALUES (@PurchaseId, @ProductId, @PackingId, @QtyPack, @QtyLose, @BnsLose)";

                            SqlCommand bodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);
                            bodyCmd.Parameters.AddWithValue("@PurchaseId", currentPurchaseId);
                            bodyCmd.Parameters.AddWithValue("@ProductId", proId);
                            bodyCmd.Parameters.AddWithValue("@PackingId", packingId);
                            bodyCmd.Parameters.AddWithValue("@QtyPack", row["Qty(P)"] != DBNull.Value ? Convert.ToInt32(row["Qty(P)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@QtyLose", row["Qty(L)"] != DBNull.Value ? Convert.ToInt32(row["Qty(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@BnsLose", row["Bns(L)"] != DBNull.Value ? Convert.ToInt32(row["Bns(L)"]) : 0);

                            bodyCmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show($"Purchase Invoice #{currentPurchaseId} saved successfully!\nTotal Products: {productsDataTable.Rows.Count}",
                    "Invoice Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                ClearForm();
                AutoGeneratePurchaseID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving invoice: " + ex.Message, "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== REMOVE ROW ====================
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show($"Are you sure you want to delete {dataGridView1.SelectedRows.Count} selected row(s)?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                }
            }
            else if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                var result = MessageBox.Show("Are you sure you want to delete this row?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                }
            }
            else
            {
                MessageBox.Show("Please select row(s) to remove", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ==================== CLEAR FORM ====================
        private void clearbtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all data?",
                "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearForm();
            }
        }

        private void ClearForm()
        {
            try
            {
                // Clear vendor info
                vendoridtxtbox.Clear();
                vendornametxtbox.Clear();
                billnotxtbox.Clear();
                narrationtxtbox.Clear();
                duedatetxtbox.Clear();

                // Clear product textboxes
                ClearProductTextboxes();

                // Clear grid
                productsDataTable.Clear();

                // Reset UI
                pendignchkbox.Checked = false;
                showloaddocxhkbox.Checked = false;
                dayshftradiobtn.Checked = true;
                packnamecombobox.SelectedIndex = -1;

                // Set dates
                SetCurrentDate();

                // New ID
                AutoGeneratePurchaseID();

                // Focus
                vendoridtxtbox.Focus();

                // Reload products in grid
                LoadAllProductsInGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearProductTextboxes()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
        }

        // ==================== AUTO GENERATE PURCHASE ID ====================
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
                MessageBox.Show("Error generating Purchase ID: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SetCurrentDate()
        {
            purchasedatepicker.Value = DateTime.Now;
            billdatepicker.Value = DateTime.Now;
        }

        // ==================== FORM CLOSING ====================
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        // ==================== OPEN INVOICE ====================
        private void openbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open invoice functionality will be implemented in next version.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Error closing database connection: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            base.OnFormClosing(e);
        }
        private void productidtxtbox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchProduct();

                e.SuppressKeyPress = true;
            }
        }
        private int GetPackingIdFromName(string packingName, SqlTransaction transaction)
        {
            try
            {
                string query = "SELECT PackingId FROM Packings WHERE PackingName = @PackingName";
                SqlCommand cmd = new SqlCommand(query, connection, transaction);
                cmd.Parameters.AddWithValue("@PackingName", packingName);

                object result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return 1; // Default if not found
            }
            catch
            {
                return 1; // Default if error
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mf=new Mainform1();
            //mf.Show();
        }
    }
}