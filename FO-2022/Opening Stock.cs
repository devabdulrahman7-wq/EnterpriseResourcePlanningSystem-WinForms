//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Opening_Stock : Form
//    {
//        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";
//        private SqlConnection connection;
//        private DataTable openingStockData;
//        private List<StockItem> stockItems = new List<StockItem>();
//        private int currentSerialNo = 0;

//        public class StockItem
//        {
//            public int SrNo { get; set; }
//            public string ProductId { get; set; }
//            public string ProductName { get; set; }
//            public string PackName { get; set; }
//            public string Pack { get; set; }
//            public int QtyPack { get; set; }
//            public int QtyLose { get; set; }
//            public decimal Price { get; set; }
//            public decimal Value { get; set; }
//            public int PackingId { get; set; }
//            public int Multiplier { get; set; }
//        }

//        public Opening_Stock()
//        {
//            InitializeComponent();
//        }

//        private void Opening_Stock_Load(object sender, EventArgs e)
//        {
//            InitializeDatabaseConnection();
//            LoadCompanyComboBox();
//            LoadPackNameComboBox();
//            LoadOpeningStockData();
//            ClearForm();

//            // Set Enter key behavior
//            SetEnterKeyNavigation();
//        }

//        private void SetEnterKeyNavigation()
//        {
//            prodidtxtbox.KeyDown += TextBox_KeyDown;
//            prodnametxtbox.KeyDown += TextBox_KeyDown;
//            packnamecombobox.KeyDown += ComboBox_KeyDown;
//            packtxtbox.KeyDown += TextBox_KeyDown;
//            qtyptxtbox.KeyDown += TextBox_KeyDown;
//            qtyltxtbox.KeyDown += TextBox_KeyDown;
//            pricetxtbox.KeyDown += TextBox_KeyDown;
//            netvaluetxtbox.KeyDown += TextBox_KeyDown;
//            cmpnynamecombox.KeyDown += ComboBox_KeyDown;
//        }

//        private void TextBox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                e.SuppressKeyPress = true;
//                Control current = (Control)sender;

//                Control[] controls = {
//                    prodidtxtbox, prodnametxtbox, packnamecombobox,
//                    packtxtbox, qtyptxtbox, qtyltxtbox,
//                    pricetxtbox, netvaluetxtbox, savebtn
//                };

//                int currentIndex = Array.IndexOf(controls, current);

//                if (currentIndex < controls.Length - 1)
//                {
//                    controls[currentIndex + 1].Focus();

//                    if (controls[currentIndex + 1] is ComboBox)
//                    {
//                        ((ComboBox)controls[currentIndex + 1]).DroppedDown = true;
//                    }
//                    else if (controls[currentIndex + 1] is Button)
//                    {
//                        savebtn.PerformClick();
//                    }
//                }
//            }
//        }

//        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                e.SuppressKeyPress = true;
//                Control current = (Control)sender;

//                Control[] controls = {
//                    prodidtxtbox, prodnametxtbox, packnamecombobox,
//                    packtxtbox, qtyptxtbox, qtyltxtbox,
//                    pricetxtbox, netvaluetxtbox, savebtn
//                };

//                int currentIndex = Array.IndexOf(controls, current);

//                if (currentIndex < controls.Length - 1)
//                {
//                    controls[currentIndex + 1].Focus();

//                    if (controls[currentIndex + 1] is Button)
//                    {
//                        savebtn.PerformClick();
//                    }
//                }
//            }
//        }

//        private void InitializeDatabaseConnection()
//        {
//            try
//            {
//                connection = new SqlConnection(connectionString);
//                connection.Open();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadCompanyComboBox()
//        {
//            try
//            {
//                cmpnynamecombox.Items.Clear();
//                string query = "SELECT DISTINCT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
//                using (SqlCommand cmd = new SqlCommand(query, connection))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        cmpnynamecombox.Items.Add(new
//                        {
//                            Id = reader["CompanyID"].ToString(),
//                            Name = reader["CompanyName"].ToString()
//                        });
//                    }
//                }
//                cmpnynamecombox.DisplayMember = "Name";
//                cmpnynamecombox.ValueMember = "Id";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadPackNameComboBox()
//        {
//            try
//            {
//                packnamecombobox.Items.Clear();
//                string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
//                using (SqlCommand cmd = new SqlCommand(query, connection))
//                using (SqlDataReader reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        packnamecombobox.Items.Add(new
//                        {
//                            Id = Convert.ToInt32(reader["PackingId"]),
//                            Name = reader["PackingName"].ToString()
//                        });
//                    }
//                }
//                packnamecombobox.DisplayMember = "Name";
//                packnamecombobox.ValueMember = "Id";
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadOpeningStockData()
//        {
//            try
//            {
//                string query = @"SELECT os.SerialNo, os.ProductId, p.ProductName, pk.PackingName, 
//                                os.Multiplier, os.QtyPack, os.QtyLose, os.PricePack, os.TTLValue
//                                FROM OpeningStock os
//                                INNER JOIN Products p ON os.ProductId = p.ProductID
//                                INNER JOIN Packings pk ON os.PackingId = pk.PackingId
//                                ORDER BY os.SerialNo";

//                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
//                openingStockData = new DataTable();
//                adapter.Fill(openingStockData);

//                stockItems.Clear();
//                foreach (DataRow row in openingStockData.Rows)
//                {
//                    stockItems.Add(new StockItem
//                    {
//                        SrNo = Convert.ToInt32(row["SerialNo"]),
//                        ProductId = row["ProductId"].ToString(),
//                        ProductName = row["ProductName"].ToString(),
//                        PackName = row["PackingName"].ToString(),
//                        Pack = row["Multiplier"].ToString(),
//                        QtyPack = Convert.ToInt32(row["QtyPack"]),
//                        QtyLose = Convert.ToInt32(row["QtyLose"]),
//                        Price = Convert.ToDecimal(row["PricePack"]),
//                        Value = Convert.ToDecimal(row["TTLValue"]),
//                        PackingId = 0,
//                        Multiplier = Convert.ToInt32(row["Multiplier"])
//                    });
//                }

//                RefreshDataGridView();
//            }
//            catch (Exception ex)
//            {
//                RefreshDataGridView();
//            }
//        }

//        private void RefreshDataGridView()
//        {
//            dataGridView1.Rows.Clear();
//            int srNo = 1;

//            foreach (var item in stockItems)
//            {
//                dataGridView1.Rows.Add(
//                    srNo++,
//                    item.ProductId,
//                    item.ProductName,
//                    item.PackName,
//                    item.Multiplier,
//                    item.QtyPack,
//                    item.QtyLose,
//                    item.Price.ToString("N2"),
//                    item.Value.ToString("N2")
//                );
//            }
//        }

//        private void ClearForm()
//        {
//            prodidtxtbox.Clear();
//            prodnametxtbox.Clear();
//            packtxtbox.Clear();
//            qtyptxtbox.Clear();
//            qtyltxtbox.Clear();
//            pricetxtbox.Clear();
//            netvaluetxtbox.Clear();
//            packnamecombobox.SelectedIndex = -1;
//            prodidtxtbox.Focus();
//        }

//        private bool ValidateInput()
//        {
//            if (string.IsNullOrWhiteSpace(prodidtxtbox.Text))
//            {
//                MessageBox.Show("Product ID cannot be empty!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                prodidtxtbox.Focus();
//                return false;
//            }

//            if (string.IsNullOrWhiteSpace(prodnametxtbox.Text))
//            {
//                MessageBox.Show("Product name cannot be empty!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                prodnametxtbox.Focus();
//                return false;
//            }

//            if (packnamecombobox.SelectedIndex == -1)
//            {
//                MessageBox.Show("Please select packing name!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                packnamecombobox.Focus();
//                return false;
//            }

//            if (!int.TryParse(packtxtbox.Text, out int multiplier) || multiplier <= 0)
//            {
//                MessageBox.Show("Please enter valid multiplier!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                packtxtbox.Focus();
//                return false;
//            }

//            if (!int.TryParse(qtyptxtbox.Text, out int qtyPack))
//            {
//                MessageBox.Show("Please enter valid pack quantity!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                qtyptxtbox.Focus();
//                return false;
//            }

//            if (!int.TryParse(qtyltxtbox.Text, out int qtyLose))
//            {
//                MessageBox.Show("Please enter valid loose quantity!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                qtyltxtbox.Focus();
//                return false;
//            }

//            if (!decimal.TryParse(pricetxtbox.Text, out decimal price) || price <= 0)
//            {
//                MessageBox.Show("Please enter valid price!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                pricetxtbox.Focus();
//                return false;
//            }

//            return true;
//        }

//        private void CalculateNetValue()
//        {
//            if (int.TryParse(packtxtbox.Text, out int multiplier) &&
//                int.TryParse(qtyptxtbox.Text, out int qtyPack) &&
//                int.TryParse(qtyltxtbox.Text, out int qtyLose) &&
//                decimal.TryParse(pricetxtbox.Text, out decimal price))
//            {
//                decimal totalValue = (qtyPack * multiplier + qtyLose) * price;
//                netvaluetxtbox.Text = totalValue.ToString("N2");
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (!ValidateInput()) return;

//            try
//            {
//                string productId = prodidtxtbox.Text.Trim();
//                string productName = prodnametxtbox.Text.Trim();
//                dynamic selectedPack = packnamecombobox.SelectedItem;
//                int packingId = selectedPack.Id;
//                string packName = selectedPack.Name;
//                int multiplier = Convert.ToInt32(packtxtbox.Text);
//                int qtyPack = Convert.ToInt32(qtyptxtbox.Text);
//                int qtyLose = Convert.ToInt32(qtyltxtbox.Text);
//                decimal price = Convert.ToDecimal(pricetxtbox.Text);
//                decimal netValue = Convert.ToDecimal(netvaluetxtbox.Text);

//                string checkProductQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
//                using (SqlCommand checkCmd = new SqlCommand(checkProductQuery, connection))
//                {
//                    checkCmd.Parameters.AddWithValue("@ProductID", productId);
//                    int productCount = (int)checkCmd.ExecuteScalar();

//                    if (productCount == 0)
//                    {
//                        string insertProductQuery = @"INSERT INTO Products (ProductID, ProductName, PurchasePrice, SalePrice, 
//                                                    PurDiscRatio, SaleDiscRatio, PurDiscValue, SaleDiscValue, 
//                                                    PurSTRatio, SaleSTRatio, CompanyID, MinStockLevel, OrderStockLevel, ProductWeight, IsFinishProduct)
//                                                    VALUES (@ProductID, @ProductName, 0, 0, 0, 0, 0, 0, 0, 0, 
//                                                    (SELECT TOP 1 CompanyID FROM Companies), 0, 0, 0, 0)";

//                        using (SqlCommand prodCmd = new SqlCommand(insertProductQuery, connection))
//                        {
//                            prodCmd.Parameters.AddWithValue("@ProductID", productId);
//                            prodCmd.Parameters.AddWithValue("@ProductName", productName);
//                            prodCmd.ExecuteNonQuery();
//                        }
//                    }
//                }

//                string maxSerialQuery = "SELECT ISNULL(MAX(SerialNo), 0) + 1 FROM OpeningStock";
//                int newSerialNo = 0;
//                using (SqlCommand maxCmd = new SqlCommand(maxSerialQuery, connection))
//                {
//                    newSerialNo = Convert.ToInt32(maxCmd.ExecuteScalar());
//                }

//                string insertQuery = @"INSERT INTO OpeningStock 
//                                    (SerialNo, ProductId, PackingId, Multiplier, QtyPack, QtyLose, 
//                                    BnsLose, PricePack, DiscRatio, DiscValPack, DiscValLose, STaxRatio, 
//                                    STaxValPack, IsValueB4Ratio, IsSTaxOnBns, IsFTaxExempted, TTLValue, 
//                                    TTLSTax, TTLFTax, ExtraSTaxBns, ExtraFTaxBns, IsDeleted)
//                                    VALUES 
//                                    (@SerialNo, @ProductId, @PackingId, @Multiplier, @QtyPack, @QtyLose,
//                                    0, @PricePack, 0, 0, 0, 0, 0, 0, 0, 0, @TTLValue, 0, 0, 0, 0, 0)";

//                using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
//                {
//                    cmd.Parameters.AddWithValue("@SerialNo", newSerialNo);
//                    cmd.Parameters.AddWithValue("@ProductId", productId);
//                    cmd.Parameters.AddWithValue("@PackingId", packingId);
//                    cmd.Parameters.AddWithValue("@Multiplier", multiplier);
//                    cmd.Parameters.AddWithValue("@QtyPack", qtyPack);
//                    cmd.Parameters.AddWithValue("@QtyLose", qtyLose);
//                    cmd.Parameters.AddWithValue("@PricePack", price);
//                    cmd.Parameters.AddWithValue("@TTLValue", netValue);

//                    cmd.ExecuteNonQuery();
//                }

//                stockItems.Add(new StockItem
//                {
//                    SrNo = newSerialNo,
//                    ProductId = productId,
//                    ProductName = productName,
//                    PackName = packName,
//                    Pack = multiplier.ToString(),
//                    QtyPack = qtyPack,
//                    QtyLose = qtyLose,
//                    Price = price,
//                    Value = netValue,
//                    PackingId = packingId,
//                    Multiplier = multiplier
//                });

//                RefreshDataGridView();
//                ClearForm();
//                MessageBox.Show("Opening stock saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving opening stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView1.SelectedRows.Count > 0)
//            {
//                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
//                int selectedIndex = selectedRow.Index;

//                if (selectedIndex < stockItems.Count)
//                {
//                    var item = stockItems[selectedIndex];

//                    DialogResult result = MessageBox.Show($"Delete product '{item.ProductName}'?", "Confirm Delete",
//                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//                    if (result == DialogResult.Yes)
//                    {
//                        try
//                        {
//                            string deleteQuery = "DELETE FROM OpeningStock WHERE SerialNo = @SerialNo";
//                            using (SqlCommand cmd = new SqlCommand(deleteQuery, connection))
//                            {
//                                cmd.Parameters.AddWithValue("@SerialNo", item.SrNo);
//                                cmd.ExecuteNonQuery();
//                            }

//                            stockItems.RemoveAt(selectedIndex);
//                            RefreshDataGridView();
//                            MessageBox.Show("Opening stock removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show("Error removing opening stock: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                        }
//                    }
//                }
//            }
//            else
//            {
//                MessageBox.Show("Please select a row to remove!", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//        }

//        private void prodidtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            string productId = prodidtxtbox.Text.Trim();
//            if (!string.IsNullOrEmpty(productId))
//            {
//                try
//                {
//                    string query = "SELECT ProductName FROM Products WHERE ProductID = @ProductID";
//                    using (SqlCommand cmd = new SqlCommand(query, connection))
//                    {
//                        cmd.Parameters.AddWithValue("@ProductID", productId);
//                        object result = cmd.ExecuteScalar();
//                        if (result != null)
//                        {
//                            prodnametxtbox.Text = result.ToString();
//                        }
//                    }
//                }
//                catch { }
//            }
//        }

//        private void prodnametxtbox_TextChanged(object sender, EventArgs e)
//        {
//        }

//        private void packtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            CalculateNetValue();
//        }

//        private void qtyptxtbox_TextChanged(object sender, EventArgs e)
//        {
//            CalculateNetValue();
//        }

//        private void qtyltxtbox_TextChanged(object sender, EventArgs e)
//        {
//            CalculateNetValue();
//        }

//        private void pricetxtbox_TextChanged(object sender, EventArgs e)
//        {
//            CalculateNetValue();
//        }
//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//        }
//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//        }

//        private void Opening_Stock_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (connection != null && connection.State == ConnectionState.Open)
//                connection.Close();
//        }
//    }
//}