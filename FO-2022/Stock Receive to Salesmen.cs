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
    public partial class Stock_Receive_to_Salesmen : Form
    {
        private string connectionString;
        private int currentReturnId = 0;
        private DataTable gridDataTable;

        public Stock_Receive_to_Salesmen()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            gridDataTable = new DataTable();
        }

        private void Stock_Receive_to_Salesmen_Load(object sender, EventArgs e)
        {
            LoadNextReturnId();
            SetupDataGridView();
            returndatedtp.Value = DateTime.Now;
            saleidtxtbox.Enabled = false;

            // TextBox ke events attach karein
            salesmenidtxtbox.LostFocus += Salesmenidtxtbox_LostFocus;
            productidtxtbox.LostFocus += Productidtxtbox_LostFocus;
        }

        // Salesmen ID jab focus lose kare tab fetch karega
        private void Salesmenidtxtbox_LostFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(salesmenidtxtbox.Text))
            {
                FetchSalesmanName();
            }
            else
            {
                //salesmennametxtbox.Clear();
                salesmennametxtbox.Clear();
            }
        }

        // Product ID jab focus lose kare tab fetch karega
        private void Productidtxtbox_LostFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(productidtxtbox.Text))
            {
                FetchProductDetails();
            }
            else
            {
                ClearProductFields();
            }
        }

        private void LoadNextReturnId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(ReturnId), 0) + 1 FROM StockReturn";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        currentReturnId = Convert.ToInt32(result);
                        saleidtxtbox.Text = currentReturnId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Return ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDataGridView()
        {
            gridDataTable.Columns.Clear();
            gridDataTable.Columns.Add("ProductID", typeof(string));
            gridDataTable.Columns.Add("ProductName", typeof(string));
            gridDataTable.Columns.Add("PackingId", typeof(int));
            gridDataTable.Columns.Add("PackName", typeof(string));
            gridDataTable.Columns.Add("Multiplier", typeof(int));
            gridDataTable.Columns.Add("QtyPack", typeof(int));
            gridDataTable.Columns.Add("QtyLose", typeof(int));
            gridDataTable.Columns.Add("PerUnitCostLose", typeof(decimal));
            gridDataTable.Columns.Add("TTLValue", typeof(decimal));

            dataGridView1.DataSource = gridDataTable;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Label click par form close
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Salesmen ID TextChanged event - sirf tab key press check karega
        private void salesmenidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Ismein kuch na karein, LostFocus event handle karega
        }

        // Salesmen ID mein Enter key press kare to
        private void salesmenidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Next control par move kare
                productidtxtbox.Focus();

                // Agar value hai to fetch kare
                if (!string.IsNullOrEmpty(salesmenidtxtbox.Text))
                {
                    FetchSalesmanName();
                }
            }
        }

        private void FetchSalesmanName()
        {
            try
            {
                // Pehle check karein ki SalesmanID valid length ki hai
                if (salesmenidtxtbox.Text.Length < 3) // Minimum length check
                {
                    salesmennametxtbox.Clear();
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text.Trim());

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        salesmennametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        salesmennametxtbox.Clear();
                        MessageBox.Show("Salesman not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching salesman: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Product ID TextChanged event
        private void productidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Ismein kuch na karein, LostFocus event handle karega
        }

        // Product ID mein Enter key press kare to
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Next control par move kare
                qtyloosetxtbox.Focus();

                // Agar value hai to fetch kare
                if (!string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    FetchProductDetails();
                }
            }
        }

       
        //dellosoft


        private void FetchProductDetails()
        {
            try
            {
                // Pehle check karein ki ProductID valid hai
                if (string.IsNullOrWhiteSpace(productidtxtbox.Text) || productidtxtbox.Text.Trim().Length < 4)
                {
                    ClearProductFields();
                    return;
                }

                string productId = productidtxtbox.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: Product Name fetch karein
                    string productNameQuery = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
                    using (SqlCommand productNameCmd = new SqlCommand(productNameQuery, conn))
                    {
                        productNameCmd.Parameters.AddWithValue("@ProductID", productId);
                        object productNameResult = productNameCmd.ExecuteScalar();

                        if (productNameResult != null)
                        {
                            productnametxtbox.Text = productNameResult.ToString();

                            // Step 2: Packing details fetch karein
                            string packingQuery = @"
                        SELECT pk.PackingName, pp.Multiplier, pk.PackingId 
                        FROM ProductPacking pp 
                        INNER JOIN Packings pk ON pp.PackingId = pk.PackingId 
                        WHERE pp.ProductId = @ProductID 
                        ORDER BY pk.PackingName";

                            using (SqlCommand packingCmd = new SqlCommand(packingQuery, conn))
                            {
                                packingCmd.Parameters.AddWithValue("@ProductID", productId);

                                using (SqlDataReader reader = packingCmd.ExecuteReader())
                                {
                                    List<ComboBoxItem> comboItems = new List<ComboBoxItem>();

                                    while (reader.Read())
                                    {
                                        comboItems.Add(new ComboBoxItem
                                        {
                                            Text = reader["PackingName"].ToString(),
                                            Value = Convert.ToInt32(reader["PackingId"]),
                                            Multiplier = Convert.ToInt32(reader["Multiplier"])
                                        });
                                    }

                                    if (comboItems.Count > 0)
                                    {
                                        // ComboBox ko bind karein
                                        packnamecombobox.DataSource = null; // Pehle clear karein
                                        packnamecombobox.DisplayMember = "Text";
                                        packnamecombobox.ValueMember = "Value";
                                        packnamecombobox.DataSource = comboItems;

                                        // Auto-select first item
                                        if (packnamecombobox.Items.Count > 0)
                                        {
                                            packnamecombobox.SelectedIndex = 0;

                                            // Multiplier update karein
                                            if (packnamecombobox.SelectedItem is ComboBoxItem selectedItem)
                                            {
                                                Multipliertxtbox.Text = selectedItem.Multiplier.ToString();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("No packing found for this product!", "Information",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        ClearProductFields();
                                    }
                                }
                            }
                        }
                        else
                        {
                            ClearProductFields();
                            MessageBox.Show("Product not found!", "Information",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearProductFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearProductFields();
            }
        }
       

        //Dellosoft

        public class ComboBoxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }  // Changed from object to int
            public int Multiplier { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (packnamecombobox.SelectedItem != null && packnamecombobox.SelectedItem is ComboBoxItem selectedItem)
                {
                    // Selected packing ka multiplier update karein
                    Multipliertxtbox.Text = selectedItem.Multiplier.ToString();

                    // Agar QtyLose already filled hai to auto-calculate QtyPack
                    if (!string.IsNullOrEmpty(qtyloosetxtbox.Text))
                    {
                        int qtyLose;
                        if (int.TryParse(qtyloosetxtbox.Text, out qtyLose) && selectedItem.Multiplier > 0)
                        {
                            int qtyPack = qtyLose / selectedItem.Multiplier;
                            qtyptxtbox.Text = qtyPack.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading multiplier: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearProductFields()
        {
            productnametxtbox.Clear();
            packnamecombobox.DataSource = null;
            packnamecombobox.Items.Clear();
            packnamecombobox.Text = "";
            Multipliertxtbox.Clear();
            qtyptxtbox.Clear();
            qtyloosetxtbox.Clear();
            productidtxtbox.SelectAll();
        }


        private void productidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control keys
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // If Enter key is pressed
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Trigger the fetch
                if (!string.IsNullOrEmpty(productidtxtbox.Text))
                {
                    FetchProductDetails();
                }

                // Move to next control
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
        }
        private void AddToGrid()
        {
            int qtyLose = Convert.ToInt32(qtyloosetxtbox.Text.Trim());
            int multiplier = Convert.ToInt32(Multipliertxtbox.Text.Trim());
            if (ValidateInput())
            {

                try
                {


                    // Additional safety check (though ValidateInput already checked)
                    if (multiplier == 0)
                    {
                        MessageBox.Show("Multiplier cannot be zero", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Multipliertxtbox.Focus();
                        return;
                    }

                    int qtyPack = qtyLose / multiplier;
                    int remainingLose = qtyLose % multiplier;

                    // Use manual Qty Pack if provided
                    if (!string.IsNullOrEmpty(qtyptxtbox.Text.Trim()))
                    {
                        qtyPack = Convert.ToInt32(qtyptxtbox.Text.Trim());
                    }

                    decimal perUnitCost = CalculatePerUnitCost(productidtxtbox.Text.Trim());
                    decimal totalValue = qtyLose * perUnitCost;

                    DataRow newRow = gridDataTable.NewRow();
                    newRow["ProductID"] = productidtxtbox.Text.Trim();
                    newRow["ProductName"] = productnametxtbox.Text.Trim();
                    newRow["PackingId"] = GetPackingIdFromComboBox();
                    newRow["PackName"] = packnamecombobox.Text.Trim();
                    newRow["Multiplier"] = multiplier;
                    newRow["QtyPack"] = qtyPack;
                    newRow["QtyLose"] = qtyLose;
                    newRow["PerUnitCostLose"] = perUnitCost;
                    newRow["TTLValue"] = totalValue;

                    gridDataTable.Rows.Add(newRow);

                    ClearProductFields();
                    productidtxtbox.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding to grid: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            // 1. Validate Product ID
            if (string.IsNullOrEmpty(productidtxtbox.Text.Trim()))
            {
                MessageBox.Show("Please enter Product ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productidtxtbox.Focus();
                return false;
            }

            // 2. Validate Product Name (if you have this field)
            if (!string.IsNullOrEmpty(productnametxtbox.Text) && string.IsNullOrEmpty(productnametxtbox.Text.Trim()))
            {
                MessageBox.Show("Please enter Product Name", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productnametxtbox.Focus();
                return false;
            }

            // 3. Validate Qty Lose
            if (string.IsNullOrEmpty(qtyloosetxtbox.Text.Trim()))
            {
                MessageBox.Show("Please enter Qty Lose", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyloosetxtbox.Focus();
                return false;
            }




            if (!int.TryParse(qtyloosetxtbox.Text.Trim(), out int qtyLose))
            {
                MessageBox.Show("Please enter valid Qty Lose (whole number only)", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyloosetxtbox.Focus();
                qtyloosetxtbox.SelectAll();
                return false;
            }

            if (qtyLose <= 0)
            {
                MessageBox.Show("Qty Lose must be greater than zero", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyloosetxtbox.Focus();
                qtyloosetxtbox.SelectAll();
                return false;
            }

            // 4. VALIDATE MULTIPLIER - THIS IS WHAT'S MISSING
            if (string.IsNullOrEmpty(Multipliertxtbox.Text.Trim()))
            {
                MessageBox.Show("Please enter Multiplier value", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Multipliertxtbox.Focus();
                return false;
            }

            if (!int.TryParse(Multipliertxtbox.Text.Trim(), out int multiplier))
            {
                MessageBox.Show("Multiplier must be a valid whole number", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Multipliertxtbox.Focus();
                Multipliertxtbox.SelectAll();
                return false;
            }

            if (multiplier <= 0)
            {
                MessageBox.Show("Multiplier must be greater than zero", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Multipliertxtbox.Focus();
                Multipliertxtbox.SelectAll();
                return false;
            }

            // 5. Validate Pack Name (from combobox)
            if (packnamecombobox.SelectedItem == null || string.IsNullOrEmpty(packnamecombobox.Text.Trim()))
            {
                MessageBox.Show("Please select a Pack Name", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packnamecombobox.Focus();
                return false;
            }

            // 6. Optional: Validate Qty Pack if entered
            if (!string.IsNullOrEmpty(qtyptxtbox.Text.Trim()))
            {
                if (!int.TryParse(qtyptxtbox.Text.Trim(), out int qtyPack))
                {
                    MessageBox.Show("Qty Pack must be a valid whole number", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    qtyptxtbox.Focus();
                    qtyptxtbox.SelectAll();
                    return false;
                }

                if (qtyPack < 0)
                {
                    MessageBox.Show("Qty Pack cannot be negative", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    qtyptxtbox.Focus();
                    qtyptxtbox.SelectAll();
                    return false;
                }

                // Optional: Check if Qty Pack is logical based on Qty Lose and Multiplier
                //int maxPossiblePacks = qtyLose / multiplier;
                //if (qtyPack > maxPossiblePacks)
                //{
                //    MessageBox.Show($"Qty Pack cannot exceed {maxPossiblePacks} based on Qty Lose ({qtyLose}) and Multiplier ({multiplier})",
                //                   "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    qtyptxtbox.Focus();
                //    qtyptxtbox.SelectAll();
                //    return false;
                //}
            }

            return true;
        }


        private decimal CalculatePerUnitCost(string productId)
        {
            // Ye function tum adjust kar sakte ho based on tumhari business logic
            // Main yahan ek sample logic de raha hun
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 PerUnitCostLose FROM StockReturnBody WHERE ProductID = @ProductID ORDER BY ReturnID DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToDecimal(result);
                    }
                }
            }
            catch { }

            // Default value
            return 0;
        }

       
        //dellosoft

        private int GetPackingIdFromComboBox()
        {
            try
            {
                if (packnamecombobox.SelectedItem != null && packnamecombobox.SelectedItem is ComboBoxItem selectedItem)
                {
                    return selectedItem.Value;  // Directly return the integer value
                }
            }
            catch
            {
                // Error handling
            }

            return 0;
        }
        // Clear Button
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void ClearAllFields()
        {
            //salesmenidtxtbox.Clear();
            //salesmennametxtbox.Clear();
            //ClearProductFields();
            //gridDataTable.Clear();
            //LoadNextReturnId();

            //returndatedtp.Value = DateTime.Now;
            productnametxtbox.Clear();
            packnamecombobox.DataSource = null;
            packnamecombobox.Items.Clear();
            Multipliertxtbox.Clear();
            qtyptxtbox.Clear();
            qtyloosetxtbox.Clear();
        }

        // Save Button
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateSave())
            {
                SaveData();
            }
        }

        private bool ValidateSave()
        {
            if (string.IsNullOrEmpty(salesmenidtxtbox.Text))
            {
                MessageBox.Show("Please enter Salesmen ID", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salesmenidtxtbox.Focus();
                return false;
            }

            if (gridDataTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least one product", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                productidtxtbox.Focus();
                return false;
            }

            return true;
        }

        private void SaveData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // StockReturn table mein insert karein
                        string headerQuery = @"
                            INSERT INTO StockReturn (ReturnId, Returndate, SalesmanID, IsUploaded) 
                            VALUES (@ReturnId, @ReturnDate, @SalesmanID, 0)";

                        SqlCommand headerCmd = new SqlCommand(headerQuery, conn, transaction);
                        headerCmd.Parameters.AddWithValue("@ReturnId", currentReturnId);
                        headerCmd.Parameters.AddWithValue("@ReturnDate", returndatedtp.Value);
                        headerCmd.Parameters.AddWithValue("@SalesmanID", salesmenidtxtbox.Text);

                        headerCmd.ExecuteNonQuery();

                        // StockReturnBody table mein har row insert karein
                        foreach (DataRow row in gridDataTable.Rows)
                        {
                            string detailQuery = @"
                                INSERT INTO StockReturnBody 
                                (ReturnID, ProductID, PackingId, Multiplier, QtyPack, QtyLose, TTLValue, PerUnitCostLose) 
                                VALUES (@ReturnID, @ProductID, @PackingId, @Multiplier, @QtyPack, @QtyLose, @TTLValue, @PerUnitCostLose)";

                            SqlCommand detailCmd = new SqlCommand(detailQuery, conn, transaction);
                            detailCmd.Parameters.AddWithValue("@ReturnID", currentReturnId);
                            detailCmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                            detailCmd.Parameters.AddWithValue("@PackingId", row["PackingId"]);
                            detailCmd.Parameters.AddWithValue("@Multiplier", row["Multiplier"]);
                            detailCmd.Parameters.AddWithValue("@QtyPack", row["QtyPack"]);
                            detailCmd.Parameters.AddWithValue("@QtyLose", row["QtyLose"]);
                            detailCmd.Parameters.AddWithValue("@TTLValue", row["TTLValue"]);
                            detailCmd.Parameters.AddWithValue("@PerUnitCostLose", row["PerUnitCostLose"]);

                            detailCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear form for new entry
                        ClearAllFields();
                        LoadNextReturnId();
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
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Open Button - Existing record open karne ke liye
        private void openbtn_Click(object sender, EventArgs e)
        {
            // Yeh tum implement kar sakte ho agar existing record open karna hai
            MessageBox.Show("Open functionality to be implemented", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Remove Button - Grid se selected row delete karega
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
            else
            {
                MessageBox.Show("Please select a row to remove", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Close Button
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            // Agar MainForm1 open karna hai to
            // MainForm1 mainForm = new MainForm1();
            // mainForm.Show();
        }

        // PictureBox click (if needed for close)
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Other event handlers
        private void saleidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
        private void salesmennametxtbox_TextChanged(object sender, EventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }
        //private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Multipliertxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // Form close hone par events detach karein
        private void Stock_Receive_to_Salesmen_FormClosing(object sender, FormClosingEventArgs e)
        {
            salesmenidtxtbox.LostFocus -= Salesmenidtxtbox_LostFocus;
            productidtxtbox.LostFocus -= Productidtxtbox_LostFocus;
        }

        private void qtyloosetxtbox_TextChanged(object sender, EventArgs e)
        {

            //if (e.KeyCode == Keys.Enter)
            //{
            //    AddToGrid();
            //}
        }
        private void qtyloosetxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddToGrid();
                e.SuppressKeyPress = true; // Optional: prevents the "ding" sound
            }
        }
    }
}




