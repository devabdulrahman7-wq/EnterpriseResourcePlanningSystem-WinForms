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

            // Initialize components
            dataGridView1.AutoGenerateColumns = true;
            InitializeDatabase();
            InitializeDataGridView();
            AutoGeneratePurchaseID();
            SetCurrentDate();
            dayshftradiobtn.Checked = true;

            // Load products in grid on form load
            LoadAllProductsInGrid();
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

                // Remove focus from grid
                vendoridtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error");
            }
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
                MessageBox.Show("Error generating Purchase ID: " + ex.Message, "Error");
            }
        }

        private void SetCurrentDate()
        {
            purchasedatepicker.Value = DateTime.Now;
            billdatepicker.Value = DateTime.Now;
        }

        // ============================ VENDOR SEARCH ============================
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
                    MessageBox.Show("Please enter Vendor ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = @"SELECT PartyName FROM Parties 
                                WHERE PartyID = @PartyID AND PartyType = 'V'";

                SqlCommand cmd = new SqlCommand(query, connection);
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
                        MessageBox.Show("Vendor not found or not of type 'V'!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        vendornametxtbox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching vendor: " + ex.Message, "Error");
            }
        }

        // ============================ PRODUCT SEARCH ============================
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchProduct();
                e.SuppressKeyPress = true;
            }
        }

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

                string query = @"SELECT p.ProductID, p.ProductName, pp.PackingId, pp.Multiplier,
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
                            productnametxtbox.Text = reader["ProductName"].ToString();

                            if (reader["PackingName"] != DBNull.Value)
                                packnametxtbox.Text = reader["PackingName"].ToString();
                            else
                                packnametxtbox.Text = "Piece";

                            if (reader["Multiplier"] != DBNull.Value)
                                packtxtbox.Text = Convert.ToInt32(reader["Multiplier"]).ToString();
                            else
                                packtxtbox.Text = "1";

                            // Set default quantities
                            qtyptxtbox.Text = "0";
                            qtyltxtbox.Text = "0";
                            bnsltxtbox.Text = "0";

                            qtyptxtbox.Focus();
                            qtyptxtbox.SelectAll();

                            MessageBox.Show($"Product '{reader["ProductName"]}' loaded!", "Success");
                        }
                        else
                        {
                            MessageBox.Show("Product not found!", "Error");
                            ClearProductTextboxes();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching product: " + ex.Message, "Error");
            }
        }

        // ============================ ADD PRODUCT TO GRID ============================
        private void AddProductToGrid()
        {
            try
            {
                if (string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    MessageBox.Show("Please enter Product ID", "Validation");
                    return;
                }

                if (string.IsNullOrEmpty(qtyptxtbox.Text) && string.IsNullOrEmpty(qtyltxtbox.Text) && string.IsNullOrEmpty(bnsltxtbox.Text))
                {
                    MessageBox.Show("Please enter at least one quantity", "Validation");
                    return;
                }

                // Get values
                string proId = productidtxtbox.Text;
                string productName = productnametxtbox.Text;
                string packName = packnametxtbox.Text;
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

                // Clear for next entry
                ClearProductTextboxes();
                productidtxtbox.Focus();

                MessageBox.Show("Product added to grid!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product: " + ex.Message, "Error");
            }
        }

        // ============================ GRID CLICK EVENT ============================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                try
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // Load grid data to textboxes
                    productidtxtbox.Text = row.Cells["Pro ID"].Value?.ToString() ?? "";
                    productnametxtbox.Text = row.Cells["Product Name"].Value?.ToString() ?? "";
                    packnametxtbox.Text = row.Cells["Pack Name"].Value?.ToString() ?? "";
                    packtxtbox.Text = row.Cells["Pack"].Value?.ToString() ?? "1";
                    qtyptxtbox.Text = row.Cells["Qty(P)"].Value?.ToString() ?? "0";
                    qtyltxtbox.Text = row.Cells["Qty(L)"].Value?.ToString() ?? "0";
                    bnsltxtbox.Text = row.Cells["Bns(L)"].Value?.ToString() ?? "0";

                    productidtxtbox.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading row: " + ex.Message);
                }
            }
        }

        // ============================ SAVE INVOICE ============================
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
                MessageBox.Show("Please enter Vendor ID!", "Validation");
                vendoridtxtbox.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(vendornametxtbox.Text))
            {
                MessageBox.Show("Please select a valid vendor!", "Validation");
                return false;
            }

            if (productsDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product!", "Validation");
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
                purchaseCmd.Parameters.AddWithValue("@DueDate", string.IsNullOrEmpty(duedatetxtbox.Text) ? (object)DBNull.Value : Convert.ToDateTime(duedatetxtbox.Text));
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
                            string purchasesBodyQuery = @"INSERT INTO PurchasesBody (PurchaseId, ProductId, QtyPack, QtyLose, BnsLose)
                                                        VALUES (@PurchaseId, @ProductId, @QtyPack, @QtyLose, @BnsLose)";

                            SqlCommand bodyCmd = new SqlCommand(purchasesBodyQuery, connection, transaction);
                            bodyCmd.Parameters.AddWithValue("@PurchaseId", currentPurchaseId);
                            bodyCmd.Parameters.AddWithValue("@ProductId", proId);
                            bodyCmd.Parameters.AddWithValue("@QtyPack", row["Qty(P)"] != DBNull.Value ? Convert.ToInt32(row["Qty(P)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@QtyLose", row["Qty(L)"] != DBNull.Value ? Convert.ToInt32(row["Qty(L)"]) : 0);
                            bodyCmd.Parameters.AddWithValue("@BnsLose", row["Bns(L)"] != DBNull.Value ? Convert.ToInt32(row["Bns(L)"]) : 0);

                            bodyCmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show($"Invoice #{currentPurchaseId} saved successfully!\nTotal Products: {productsDataTable.Rows.Count}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                ClearForm();
                AutoGeneratePurchaseID();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show("Error saving invoice: " + ex.Message, "Error");
            }
        }

        // ============================ REMOVE ROW ============================
        private void removebtn_Click(object sender, EventArgs e)
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
            }
            else if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
            {
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
            else
            {
                MessageBox.Show("Please select row(s) to remove", "Information");
            }
        }

        // ============================ CLEAR FORM ============================
        private void clearbtn_Click(object sender, EventArgs e)
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

                // Set dates
                SetCurrentDate();

                // New ID
                AutoGeneratePurchaseID();

                // Focus
                vendoridtxtbox.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing form: " + ex.Message);
            }
        }

        private void ClearProductTextboxes()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnametxtbox.Clear();
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            bnsltxtbox.Clear();
        }

        // ============================ OPEN INVOICE ============================
        private void openbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open functionality to be implemented", "Information");
        }

        // ============================ FORM CLOSING ============================
        private void closebtn_Click(object sender, EventArgs e)
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
                MessageBox.Show("Error closing connection: " + ex.Message);
            }
            base.OnFormClosing(e);
        }

        // ============================ ADD PRODUCT BUTTON (Missing in design, add manually) ============================
        //private void CreateAddProductButton()
        //{
        //    Button addBtn = new Button();
        //    addBtn.Text = "Add Product";
        //    addBtn.Location = new Point(700, 240);
        //    addBtn.Size = new Size(120, 25);
        //    addBtn.BackColor = Color.FromArgb(128, 128, 255);
        //    addBtn.FlatStyle = FlatStyle.Flat;
        //    addBtn.Font = new Font("Times New Roman", 9.75F, FontStyle.Bold);
        //    addBtn.Click += (s, ev) => AddProductToGrid();
        //    this.Controls.Add(addBtn);
        //}

        // ============================ EMPTY EVENT HANDLERS ============================
        private void purchaseidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void billnotxtbox_TextChanged(object sender, EventArgs e) { }
        private void duedatetxtbox_TextChanged(object sender, EventArgs e) { }
        private void vendoridtxtbox_TextChanged(object sender, EventArgs e) { }
        private void vendornametxtbox_TextChanged(object sender, EventArgs e) { }
        private void showloaddocxhkbox_CheckedChanged(object sender, EventArgs e) { }
        private void dayshftbtn_CheckedChanged(object sender, EventArgs e) { }
        private void nightshftbtn_CheckedChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void narrationtxtbox_TextChanged(object sender, EventArgs e) { }
        private void bnsltxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyltxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }

        private void Purchase_Invoice_01_Load(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pendignchkbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void billlnolbl_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void productnametxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void purchasedatepicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void packtxtbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void purchasedatelbl_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void billdatepicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void purchaseidlbl_Click(object sender, EventArgs e)
        {

        }
    }
}