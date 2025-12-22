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

namespace FO_2022
{
    public partial class Stock_Transfer_Salesmen_to_Salesmen : Form
    {
        // Connection string directly
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        private List<ProductItem> productItems = new List<ProductItem>();
        private string fromSalesmanId = "";
        private string toSalesmanId = "";

        public class ProductItem
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string PackName { get; set; }
            public int Pack { get; set; }
            public int QtyPack { get; set; }
            public int QtyLose { get; set; }
            public decimal PerUnitCostLose { get; set; }
            public decimal TotalValue { get; set; }
            public int PackingId { get; set; }
        }

        public Stock_Transfer_Salesmen_to_Salesmen()
        {
            InitializeComponent();
        }

        private void Stock_Transfer_Salesmen_to_Salesmen_Load(object sender, EventArgs e)
        {
            // Set today's date
            dateTimePicker1.Value = DateTime.Now;

            // Set focus to from salesman ID
            fromsalesmenidtxtbox.Focus();

            // Initialize grid columns
            InitializeGridColumns();
        }

        private void InitializeGridColumns()
        {
            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Add new columns
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            col1.HeaderText = "Product ID";
            col1.Name = "colProductID";
            col1.Width = 80;
            dataGridView1.Columns.Add(col1);

            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col2.HeaderText = "Product Name";
            col2.Name = "colProductName";
            col2.Width = 150;
            dataGridView1.Columns.Add(col2);

            DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
            col3.HeaderText = "Pack Name";
            col3.Name = "colPackName";
            col3.Width = 100;
            dataGridView1.Columns.Add(col3);

            DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
            col4.HeaderText = "Pack";
            col4.Name = "colPack";
            col4.Width = 60;
            dataGridView1.Columns.Add(col4);

            DataGridViewTextBoxColumn col5 = new DataGridViewTextBoxColumn();
            col5.HeaderText = "Qty Pack";
            col5.Name = "colQtyPack";
            col5.Width = 80;
            dataGridView1.Columns.Add(col5);

            DataGridViewTextBoxColumn col6 = new DataGridViewTextBoxColumn();
            col6.HeaderText = "Qty Lose";
            col6.Name = "colQtyLose";
            col6.Width = 80;
            dataGridView1.Columns.Add(col6);
        }

        // 1. FROM SALESMAN ID - Enter Press
        private void fromsalesmenidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string id = fromsalesmenidtxtbox.Text.Trim();
                if (!string.IsNullOrEmpty(id))
                {
                    LoadSalesmanName(id, fromsalesmennametxtbox, ref fromSalesmanId);
                }
                else
                {
                    MessageBox.Show("Please enter From Salesman ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // 2. TO SALESMAN ID - Enter Press
        private void tosalesmenidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string id = tosalesmenidtxtbox.Text.Trim();
                if (!string.IsNullOrEmpty(id))
                {
                    LoadSalesmanName(id, tosalesmennametxtbox, ref toSalesmanId);
                }
                else
                {
                    MessageBox.Show("Please enter To Salesman ID", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void LoadSalesmanName(string salesmanId, TextBox nameBox, ref string storeId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID", salesmanId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        nameBox.Text = result.ToString();
                        storeId = salesmanId;
                        MessageBox.Show($"Salesman found: {result}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Move focus
                        if (nameBox == fromsalesmennametxtbox)
                            tosalesmenidtxtbox.Focus();
                        else
                            productidtxtbox.Focus();
                    }
                    else
                    {
                        nameBox.Clear();
                        MessageBox.Show("Salesman ID not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (nameBox == fromsalesmennametxtbox)
                        {
                            fromsalesmenidtxtbox.Focus();
                            fromsalesmenidtxtbox.SelectAll();
                        }
                        else
                        {
                            tosalesmenidtxtbox.Focus();
                            tosalesmenidtxtbox.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. PRODUCT ID - Enter Press
        private void productidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(fromSalesmanId))
                {
                    MessageBox.Show("Please enter From Salesman ID first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fromsalesmenidtxtbox.Focus();
                    return;
                }

                string productId = productidtxtbox.Text.Trim();
                if (!string.IsNullOrEmpty(productId))
                {
                    LoadProductInfo(productId);
                }
            }
        }

        private void LoadProductInfo(string productId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT p.ProductName, sm.PackingId, pk.PackingName, sm.Multiplier 
                        FROM SMOpeningStock sm
                        INNER JOIN Products p ON sm.ProductId = p.ProductID
                        INNER JOIN Packings pk ON sm.PackingId = pk.PackingId
                        WHERE sm.ProductId = @ProductID 
                        AND sm.SalesmanId = @SalesmanID
                        AND (sm.IsDeleted IS NULL OR sm.IsDeleted = 0)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@SalesmanID", fromSalesmanId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Product found
                            productnametxtbox.Text = reader["ProductName"].ToString();
                            packtxtbox.Text = reader["Multiplier"].ToString();
                            packnamecombobox.Text = reader["PackingName"].ToString();

                            MessageBox.Show("Product found in salesman stock", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            qtyptxtbox.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Product not in salesman stock", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClearProductFields();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. PACK TEXTBOX - Enter Press
        private void packtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                qtyptxtbox.Focus();
            }
        }

        // 5. QTY PACK - Enter Press
        private void qtyptxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CalculateLooseQty();
                qtyltxtbox.Focus();
            }
        }

        private void CalculateLooseQty()
        {
            try
            {
                int qtyPack, multiplier;

                if (int.TryParse(qtyptxtbox.Text, out qtyPack) &&
                    int.TryParse(packtxtbox.Text, out multiplier))
                {
                    int qtyLose = qtyPack * multiplier;
                    qtyltxtbox.Text = qtyLose.ToString();
                }
                else
                {
                    qtyltxtbox.Text = "0";
                }
            }
            catch
            {
                qtyltxtbox.Text = "0";
            }
        }

        // 6. QTY LOSE - Enter Press (Add to Grid)
        private void qtyltxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddToGrid();
            }
        }

        private void AddToGrid()
        {
            // Validation
            if (string.IsNullOrEmpty(productidtxtbox.Text))
            {
                MessageBox.Show("Select product first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int qtyPack, qtyLose, packMultiplier;

            if (!int.TryParse(qtyptxtbox.Text, out qtyPack) || qtyPack <= 0)
            {
                MessageBox.Show("Enter valid pack quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                qtyptxtbox.Focus();
                return;
            }

            if (!int.TryParse(qtyltxtbox.Text, out qtyLose) || qtyLose <= 0)
            {
                MessageBox.Show("Enter valid loose quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                qtyltxtbox.Focus();
                return;
            }

            if (!int.TryParse(packtxtbox.Text, out packMultiplier))
            {
                packMultiplier = 1;
            }

            // Add to DataGridView
            int rowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowIndex].Cells["colProductID"].Value = productidtxtbox.Text.Trim();
            dataGridView1.Rows[rowIndex].Cells["colProductName"].Value = productnametxtbox.Text.Trim();
            dataGridView1.Rows[rowIndex].Cells["colPackName"].Value = packnamecombobox.Text;
            dataGridView1.Rows[rowIndex].Cells["colPack"].Value = packMultiplier;
            dataGridView1.Rows[rowIndex].Cells["colQtyPack"].Value = qtyPack;
            dataGridView1.Rows[rowIndex].Cells["colQtyLose"].Value = qtyLose;

            // Get PackingId
            int packingId = GetPackingIdFromDB(packnamecombobox.Text);

            // Calculate cost
            decimal perUnitCost = GetProductCost(productidtxtbox.Text.Trim(), packMultiplier);
            decimal totalValue = perUnitCost * qtyLose;

            // Add to list
            ProductItem item = new ProductItem
            {
                ProductID = productidtxtbox.Text.Trim(),
                ProductName = productnametxtbox.Text.Trim(),
                PackName = packnamecombobox.Text,
                Pack = packMultiplier,
                QtyPack = qtyPack,
                QtyLose = qtyLose,
                PerUnitCostLose = perUnitCost,
                TotalValue = totalValue,
                PackingId = packingId
            };
            productItems.Add(item);

            MessageBox.Show("Product added to list", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Clear for next entry
            ClearProductFields();
        }

        private int GetPackingIdFromDB(string packingName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT PackingId FROM Packings WHERE PackingName = @Name";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", packingName);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private decimal GetProductCost(string productId, int multiplier)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT PurchasePrice FROM Products WHERE ProductID = @ID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        decimal price = Convert.ToDecimal(result);
                        return price / (multiplier > 0 ? multiplier : 1);
                    }
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private void ClearProductFields()
        {
            productidtxtbox.Clear();
            productnametxtbox.Clear();
            packnamecombobox.Text = "";
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            productidtxtbox.Focus();
        }

        // SAVE BUTTON
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateTransfer())
            {
                SaveTransfer();
            }
        }

        private bool ValidateTransfer()
        {
            if (string.IsNullOrEmpty(fromSalesmanId))
            {
                MessageBox.Show("Enter From Salesman ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(toSalesmanId))
            {
                MessageBox.Show("Enter To Salesman ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (fromSalesmanId == toSalesmanId)
            {
                MessageBox.Show("Salesmen cannot be same", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Add products first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void SaveTransfer()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        foreach (ProductItem item in productItems)
                        {
                            int totalLose = (item.QtyPack * item.Pack) + item.QtyLose;

                            // 1. Deduct from FROM salesman
                            DeductFromSalesman(con, transaction, item, totalLose);

                            // 2. Add to TO salesman
                            AddToSalesman(con, transaction, item, totalLose);
                        }

                        transaction.Commit();

                        MessageBox.Show("Stock transfer successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Transfer failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeductFromSalesman(SqlConnection con, SqlTransaction trans, ProductItem item, int totalLose)
        {
            // Update SMOpeningStock
            string query1 = @"
                UPDATE SMOpeningStock 
                SET QtyPack = ISNULL(QtyPack, 0) - @QtyPack,
                    QtyLose = ISNULL(QtyLose, 0) - @QtyLose
                WHERE ProductId = @ProductID 
                AND SalesmanId = @SalesmanID 
                AND PackingId = @PackingID";

            SqlCommand cmd1 = new SqlCommand(query1, con, trans);
            cmd1.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd1.Parameters.AddWithValue("@SalesmanID", fromSalesmanId);
            cmd1.Parameters.AddWithValue("@PackingID", item.PackingId);
            cmd1.Parameters.AddWithValue("@QtyPack", item.QtyPack);
            cmd1.Parameters.AddWithValue("@QtyLose", item.QtyLose);
            cmd1.ExecuteNonQuery();

            // Update SMCurrentStock
            string query2 = @"
                UPDATE SMCurrentStock 
                SET Qtylose = Qtylose - @TotalLose
                WHERE ProductId = @ProductID 
                AND SalesmanId = @SalesmanID";

            SqlCommand cmd2 = new SqlCommand(query2, con, trans);
            cmd2.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd2.Parameters.AddWithValue("@SalesmanID", fromSalesmanId);
            cmd2.Parameters.AddWithValue("@TotalLose", totalLose);
            cmd2.ExecuteNonQuery();
        }

        private void AddToSalesman(SqlConnection con, SqlTransaction trans, ProductItem item, int totalLose)
        {
            // Update SMOpeningStock
            string query1 = @"
                UPDATE SMOpeningStock 
                SET QtyPack = ISNULL(QtyPack, 0) + @QtyPack,
                    QtyLose = ISNULL(QtyLose, 0) + @QtyLose
                WHERE ProductId = @ProductID 
                AND SalesmanId = @SalesmanID 
                AND PackingId = @PackingID";

            SqlCommand cmd1 = new SqlCommand(query1, con, trans);
            cmd1.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd1.Parameters.AddWithValue("@SalesmanID", toSalesmanId);
            cmd1.Parameters.AddWithValue("@PackingID", item.PackingId);
            cmd1.Parameters.AddWithValue("@QtyPack", item.QtyPack);
            cmd1.Parameters.AddWithValue("@QtyLose", item.QtyLose);
            cmd1.ExecuteNonQuery();

            // Update SMCurrentStock
            string query2 = @"
                UPDATE SMCurrentStock 
                SET Qtylose = Qtylose + @TotalLose
                WHERE ProductId = @ProductID 
                AND SalesmanId = @SalesmanID";

            SqlCommand cmd2 = new SqlCommand(query2, con, trans);
            cmd2.Parameters.AddWithValue("@ProductID", item.ProductID);
            cmd2.Parameters.AddWithValue("@SalesmanID", toSalesmanId);
            cmd2.Parameters.AddWithValue("@TotalLose", totalLose);
            cmd2.ExecuteNonQuery();
        }

        // CLEAR BUTTON
        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            fromsalesmenidtxtbox.Clear();
            fromsalesmennametxtbox.Clear();
            tosalesmenidtxtbox.Clear();
            tosalesmennametxtbox.Clear();
            ClearProductFields();
            dataGridView1.Rows.Clear();
            productItems.Clear();
            fromSalesmanId = "";
            toSalesmanId = "";
            fromsalesmenidtxtbox.Focus();
        }

        // REMOVE BUTTON
        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    string productId = row.Cells["colProductID"].Value?.ToString();
                    if (!string.IsNullOrEmpty(productId))
                    {
                        ProductItem item = productItems.FirstOrDefault(p => p.ProductID == productId);
                        if (item != null)
                        {
                            productItems.Remove(item);
                        }
                    }
                    dataGridView1.Rows.Remove(row);
                }
            }
        }

        // CLOSE BUTTON
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mainform1 = new Mainform1();
            mainform1.Show();
        }

        // Other event handlers (keep as is)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void productidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void productnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void packnamecombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void packtxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) { }
        private void qtyltxtbox_TextChanged(object sender, EventArgs e) { }
        private void fromsalesmenidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void fromsalesmennametxtbox_TextChanged(object sender, EventArgs e) { }
        private void tosalesmenidtxtbox_TextChanged(object sender, EventArgs e) { }
        private void tosalesmennametxtbox_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) { }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }
    }
}