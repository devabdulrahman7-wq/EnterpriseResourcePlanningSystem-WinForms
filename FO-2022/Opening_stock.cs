using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Opening_stock : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection con;
        private DataTable stockDataTable = new DataTable();
        private int selectedSerialNo = 0;
        private bool isEditMode = false;

        public Opening_stock()
        {
            InitializeComponent();
            con = new SqlConnection(connectionString);
            SetupDataTable();
        }

        private void SetupDataTable()
        {
            stockDataTable.Columns.Clear();
            stockDataTable.Columns.Add("SerialNo", typeof(int));
            stockDataTable.Columns.Add("ProductId", typeof(int));
            stockDataTable.Columns.Add("ProductName", typeof(string));
            stockDataTable.Columns.Add("P_Name", typeof(string));
            stockDataTable.Columns.Add("Multiplier", typeof(decimal));
            stockDataTable.Columns.Add("QtyPack", typeof(decimal));
            stockDataTable.Columns.Add("QtyLose", typeof(decimal));
            stockDataTable.Columns.Add("Price", typeof(decimal));
            stockDataTable.Columns.Add("Value", typeof(decimal));

            dataGridView1.DataSource = stockDataTable;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            // Set column headers
            dataGridView1.Columns["SerialNo"].HeaderText = "Sr #";
            dataGridView1.Columns["ProductId"].HeaderText = "Product ID";
            dataGridView1.Columns["ProductName"].HeaderText = "Product Name";
            dataGridView1.Columns["P_Name"].HeaderText = "Packing Name";
            dataGridView1.Columns["Multiplier"].HeaderText = "Multiplier";
            dataGridView1.Columns["QtyPack"].HeaderText = "Qty (P)";
            dataGridView1.Columns["QtyLose"].HeaderText = "Qty (L)";
            dataGridView1.Columns["Price"].HeaderText = "Price";
            dataGridView1.Columns["Value"].HeaderText = "Value";

            dataGridView1.Columns["SerialNo"].Width = 50;
            dataGridView1.Columns["ProductId"].Width = 70;
            dataGridView1.Columns["ProductName"].Width = 150;
            dataGridView1.Columns["P_Name"].Width = 80;
            dataGridView1.Columns["Multiplier"].Width = 70;
            dataGridView1.Columns["QtyPack"].Width = 70;
            dataGridView1.Columns["QtyLose"].Width = 70;
            dataGridView1.Columns["Price"].Width = 80;
            dataGridView1.Columns["Value"].Width = 90;
        }

        private void Opening_stock_Load(object sender, EventArgs e)
        {
            LoadPackNameComboBox();
            LoadGrid();
            LoadCompanyComboBox();   // Add this line

            prodidtxtbox.Focus();
        }
        private void LoadCompanyComboBox()
        {
            try
            {
                cmpnynamecombox.Items.Clear();
                cmpnynamecombox.Items.Add("-- Select Company --");

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand("SELECT CompanyName FROM Companies ORDER BY CompanyName", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    cmpnynamecombox.Items.Add(reader["CompanyName"].ToString());

                reader.Close();
                con.Close();

                cmpnynamecombox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadPackNameComboBox()
        {
            try
            {
                packnamecombobox.Items.Clear();
                packnamecombobox.Items.Add("-- Select Packing --");

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand("SELECT PackingName FROM Packings ORDER BY PackingName", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    packnamecombobox.Items.Add(reader["PackingName"].ToString());
                reader.Close();
                con.Close();

                packnamecombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error loading packing names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void prodidtxtbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(prodidtxtbox.Text))
            {
                prodnametxtbox.Clear();
                return;
            }

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                int productId;
                if (!int.TryParse(prodidtxtbox.Text, out productId))
                {
                    prodnametxtbox.Clear();
                    return;
                }

                SqlCommand cmd = new SqlCommand("SELECT ProductName FROM Products WHERE ProductID = @pid", con);
                cmd.Parameters.AddWithValue("@pid", productId);
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null)
                {
                    prodnametxtbox.Text = result.ToString();
                }
                else
                {
                    prodnametxtbox.Clear();
                }
            }
            catch
            {
                con.Close();
                prodnametxtbox.Clear();
            }
        }

        private void CalculateNetValue()
        {
            try
            {
                decimal qtyP = decimal.TryParse(qtyptxtbox.Text, out decimal qp) ? qp : 0;
                decimal qtyL = decimal.TryParse(qtyltxtbox.Text, out decimal ql) ? ql : 0;
                decimal multiplier = decimal.TryParse(packtxtbox.Text, out decimal m) ? m : 0;
                decimal price = decimal.TryParse(pricetxtbox.Text, out decimal pr) ? pr : 0;

                decimal netValue = ((qtyP * multiplier) + qtyL) * price;
                netvaluetxtbox.Text = netValue.ToString("N2");
            }
            catch
            {
                netvaluetxtbox.Text = "0.00";
            }
        }

        private void packtxtbox_TextChanged(object sender, EventArgs e) => CalculateNetValue();
        private void qtyptxtbox_TextChanged(object sender, EventArgs e) => CalculateNetValue();
        private void qtyltxtbox_TextChanged(object sender, EventArgs e) => CalculateNetValue();
        private void pricetxtbox_TextChanged(object sender, EventArgs e) => CalculateNetValue();

        private bool IsProductIdValid()
        {
            if (string.IsNullOrWhiteSpace(prodidtxtbox.Text))
                return false;

            try
            {
                int productId;
                if (!int.TryParse(prodidtxtbox.Text, out productId))
                    return false;

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductID = @pid", con);
                cmd.Parameters.AddWithValue("@pid", productId);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                return count > 0;
            }
            catch
            {
                con.Close();
                return false;
            }
        }

        private bool ValidateForm()
        {
            // Check Product ID
            if (!IsProductIdValid())
            {
                MessageBox.Show("Please enter a valid Product ID.\n", "Invalid Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                prodidtxtbox.Focus();
                return false;
            }

            // Check if Product Name is filled (means product exists)
            if (string.IsNullOrWhiteSpace(prodnametxtbox.Text))
            {
                MessageBox.Show("Product not found. Please enter a valid Product ID.", "Product Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                prodidtxtbox.Focus();
                return false;
            }

            // Check Packing
            if (packnamecombobox.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a Packing name.", "Packing Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packnamecombobox.Focus();
                return false;
            }

            // Check Multiplier
            if (string.IsNullOrWhiteSpace(packtxtbox.Text) || !decimal.TryParse(packtxtbox.Text, out decimal multiplier) || multiplier <= 0)
            {
                MessageBox.Show("Please enter a validPack  (greater than 0).", "Invalid Multiplier", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packtxtbox.Focus();
                return false;
            }

            // Check Quantity
            if (string.IsNullOrWhiteSpace(qtyptxtbox.Text) || !decimal.TryParse(qtyptxtbox.Text, out decimal qtyP))
            {
                MessageBox.Show("Please enter a valid Quantity (Pack).", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                qtyptxtbox.Focus();
                return false;
            }

            // Check Price
            //if (string.IsNullOrWhiteSpace(pricetxtbox.Text) || !decimal.TryParse(pricetxtbox.Text, out decimal price) || price <= 0)
            //{
            //    MessageBox.Show("Please enter a valid Price (greater than 0).", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    pricetxtbox.Focus();
            //    return false;
            //}

            return true;
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            try
            {
                int productId = int.Parse(prodidtxtbox.Text.Trim());
                string packName = packnamecombobox.Text;
                int packingId = GetPackingId(packName);

                if (packingId <= 0)
                {
                    MessageBox.Show("Invalid Packing selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal multiplier = decimal.Parse(packtxtbox.Text);
                decimal qtyPack = decimal.Parse(qtyptxtbox.Text);
                decimal qtyLose = decimal.TryParse(qtyltxtbox.Text, out decimal ql) ? ql : 0;
                decimal price = decimal.Parse(pricetxtbox.Text);
                decimal netValue = decimal.Parse(netvaluetxtbox.Text);

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd;

                if (selectedSerialNo == 0)
                {
                    // INSERT NEW RECORD
                    cmd = new SqlCommand(@"
                        INSERT INTO OpeningStock 
                        (ProductId, PackingId, Multiplier, QtyPack, QtyLose, PricePack, TTLValue, IsDeleted) 
                        VALUES (@pid, @packid, @multi, @qtp, @qtl, @price, @ttl, 0)", con);
                }
                else
                {
                    // UPDATE EXISTING RECORD
                    cmd = new SqlCommand(@"
                        UPDATE OpeningStock 
                        SET ProductId = @pid, 
                            PackingId = @packid, 
                            Multiplier = @multi, 
                            QtyPack = @qtp, 
                            QtyLose = @qtl, 
                            PricePack = @price, 
                            TTLValue = @ttl 
                        WHERE SerialNo = @sno", con);
                    cmd.Parameters.AddWithValue("@sno", selectedSerialNo);
                }

                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@packid", packingId);
                cmd.Parameters.AddWithValue("@multi", multiplier);
                cmd.Parameters.AddWithValue("@qtp", qtyPack);
                cmd.Parameters.AddWithValue("@qtl", qtyLose);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@ttl", netValue);

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    string message = selectedSerialNo == 0 ?

                        "Record saved successfully!" : "Record updated successfully!";
                    MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadGrid();
                    ClearBoxes();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error saving record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetPackingId(string packName)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand("SELECT PackingId FROM Packings WHERE PackingName = @name", con);
                cmd.Parameters.AddWithValue("@name", packName);
                object result = cmd.ExecuteScalar();
                con.Close();

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                con.Close();
                return 0;
            }
        }

        private void LoadGrid()
        {
            try
            {
                stockDataTable.Clear();

                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT OS.SerialNo, 
                           OS.ProductId, 
                           P.ProductName, 
                           PK.PackingName as P_Name,
                           OS.Multiplier,
                           OS.QtyPack, 
                           OS.QtyLose, 
                           OS.PricePack as Price, 
                           OS.TTLValue as Value
                    FROM OpeningStock OS
                    INNER JOIN Products P ON OS.ProductId = P.ProductID
                    INNER JOIN Packings PK ON OS.PackingId = PK.PackingId
                    WHERE OS.IsDeleted = 0
                    ORDER BY OS.SerialNo", con);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable tempTable = new DataTable();
                adapter.Fill(tempTable);
                con.Close();

                foreach (DataRow row in tempTable.Rows)
                {
                    stockDataTable.Rows.Add(
                        row["SerialNo"],
                        row["ProductId"],
                        row["ProductName"],
                        row["P_Name"],
                        row["Multiplier"],
                        row["QtyPack"],
                        row["QtyLose"],
                        row["Price"],
                        row["Value"]
                    );
                }

                // Refresh data grid view
                dataGridView1.Refresh();
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= stockDataTable.Rows.Count)
                return;

            DataRow row = stockDataTable.Rows[e.RowIndex];

            selectedSerialNo = Convert.ToInt32(row["SerialNo"]);
            prodidtxtbox.Text = row["ProductId"].ToString();
            prodnametxtbox.Text = row["ProductName"].ToString();
            packnamecombobox.Text = row["P_Name"].ToString();
            packtxtbox.Text = row["Multiplier"].ToString();
            qtyptxtbox.Text = row["QtyPack"].ToString();
            qtyltxtbox.Text = row["QtyLose"].ToString();
            pricetxtbox.Text = row["Price"].ToString();
            netvaluetxtbox.Text = row["Value"].ToString();

            isEditMode = true;
            savebtn.Text = "Update";
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (selectedSerialNo == 0)
            {
                MessageBox.Show("Please select a record from the grid to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand("UPDATE OpeningStock SET IsDeleted = 1 WHERE SerialNo = @sno", con);
                cmd.Parameters.AddWithValue("@sno", selectedSerialNo);
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Record deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGrid();
                    ClearBoxes();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                MessageBox.Show("Error deleting record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearBoxes();
        }

        private void ClearBoxes()
        {
            prodidtxtbox.Clear();
            prodnametxtbox.Clear();
            packnamecombobox.SelectedIndex = 0;
            packtxtbox.Clear();
            qtyptxtbox.Clear();
            qtyltxtbox.Clear();
            pricetxtbox.Clear();
            netvaluetxtbox.Clear();

            selectedSerialNo = 0;
            isEditMode = false;
            savebtn.Text = "Save";
            dataGridView1.ClearSelection();
            prodidtxtbox.Focus();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
          
        }

        private void prodidtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers and control keys
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void packtxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow numbers, decimal point and control keys
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            //Mainform1 mf = new Mainform1();
            //mf.Show();
        }
    }
}