////using System;
////using System.Collections.Generic;
////using System.ComponentModel;
////using System.Data;
////using System.Data.SqlClient;
////using System.Drawing;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;
////using System.Windows.Forms;

////namespace FO_2022
////{
////    public partial class stockalertlvltxtbox : Form
////    {
////        // Connection string
////        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

////        // DataTables for storing data
////        private DataTable dtCompanies;
////        private DataTable dtProductGroups;
////        private DataTable dtSubGroups;
////        private DataTable dtPackings;
////        private DataTable dtProducts;

////        public stockalertlvltxtbox()
////        {
////            InitializeComponent();
////        }

////        private void Products_Load(object sender, EventArgs e)
////        {
////            LoadAllData();
////        }

////        private void LoadAllData()
////        {
////            try
////            {
////                // Load Companies
////                LoadCompanies();

////                // Load Product Groups
////                LoadProductGroups();

////                // Load Sub Groups
////                LoadSubGroups();

////                // Load Packings
////                LoadPackings();

////                // Load Products Grid
////                LoadProductsGrid();
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        #region Data Loading Methods

////        private void LoadCompanies()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
////                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
////                    dtCompanies = new DataTable();
////                    da.Fill(dtCompanies);

////                    // Bind to combobox
////                    cmpnynamecombobox1.DataSource = dtCompanies;
////                    cmpnynamecombobox1.DisplayMember = "CompanyName";
////                    cmpnynamecombobox1.ValueMember = "CompanyID";

////                    // Also bind to other company combobox if exists
////                    if (slctcmpnycombobox != null)
////                    {
////                        slctcmpnycombobox.DataSource = dtCompanies.Copy();
////                        slctcmpnycombobox.DisplayMember = "CompanyName";
////                        slctcmpnycombobox.ValueMember = "CompanyID";
////                    }
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void LoadProductGroups()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    string query = "SELECT CompanyId, GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
////                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
////                    dtProductGroups = new DataTable();
////                    da.Fill(dtProductGroups);

////                    // Bind to combobox
////                    grpnamecomboBox.DataSource = dtProductGroups;
////                    grpnamecomboBox.DisplayMember = "GroupName";
////                    grpnamecomboBox.ValueMember = "GroupId";

////                    // Also bind to other product group combobox if exists
////                    if (slctprdctgrpcomboBox1 != null)
////                    {
////                        slctprdctgrpcomboBox1.DataSource = dtProductGroups.Copy();
////                        slctprdctgrpcomboBox1.DisplayMember = "GroupName";
////                        slctprdctgrpcomboBox1.ValueMember = "GroupId";
////                    }
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void LoadSubGroups()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    string query = @"SELECT sg.GroupID, sg.SubGroupId, sg.CompanyId, sg.SubGroupName 
////                                   FROM ProductSubGroups sg
////                                   INNER JOIN ProductGroups pg ON sg.CompanyId = pg.CompanyId AND sg.GroupID = pg.GroupId
////                                   ORDER BY sg.SubGroupName";
////                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
////                    dtSubGroups = new DataTable();
////                    da.Fill(dtSubGroups);

////                    // Bind to combobox
////                    subgrpnamecomboBox.DataSource = dtSubGroups;
////                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
////                    subgrpnamecomboBox.ValueMember = "SubGroupId";
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void LoadPackings()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
////                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
////                    dtPackings = new DataTable();
////                    da.Fill(dtPackings);

////                    // Bind to purchase pack combobox
////                    purchasepackcombobox.DataSource = dtPackings.Copy();
////                    purchasepackcombobox.DisplayMember = "PackingName";
////                    purchasepackcombobox.ValueMember = "PackingId";

////                    // Bind to sale pack combobox
////                    salepackcombobox.DataSource = dtPackings.Copy();
////                    salepackcombobox.DisplayMember = "PackingName";
////                    salepackcombobox.ValueMember = "PackingId";
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void LoadProductsGrid()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    string query = @"SELECT 
////                                    p.ProductID,
////                                    c.CompanyName,
////                                    pg.GroupName,
////                                    psg.SubGroupName,
////                                    p.ProductName,
////                                    p.PurchasePrice,
////                                    p.SalePrice,
////                                    p.RetailPrice,
////                                    p.MinStockLevel,
////                                    p.OrderStockLevel,
////                                    p.BarCode,
////                                    p.ProductWeight,
////                                    p.IsFinishProduct
////                                FROM Products p
////                                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
////                                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
////                                LEFT JOIN ProductSubGroups psg ON p.CompanyID = psg.CompanyId AND p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId
////                                ORDER BY p.ProductID";

////                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
////                    dtProducts = new DataTable();
////                    da.Fill(dtProducts);

////                    // Bind to DataGridView
////                    dataGridView2.DataSource = dtProducts;
////                    FormatProductsGrid();
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void FormatProductsGrid()
////        {
////            if (dataGridView2.Columns.Count > 0)
////            {
////                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
////                dataGridView2.Columns["ProductID"].HeaderText = "Product ID";
////                dataGridView2.Columns["CompanyName"].HeaderText = "Company";
////                dataGridView2.Columns["GroupName"].HeaderText = "Group";
////                dataGridView2.Columns["SubGroupName"].HeaderText = "Sub Group";
////                dataGridView2.Columns["ProductName"].HeaderText = "Product Name";
////                dataGridView2.Columns["PurchasePrice"].HeaderText = "Purchase Price";
////                dataGridView2.Columns["SalePrice"].HeaderText = "Sale Price";
////                dataGridView2.Columns["RetailPrice"].HeaderText = "Retail Price";
////                dataGridView2.Columns["MinStockLevel"].HeaderText = "Min Stock";
////                dataGridView2.Columns["OrderStockLevel"].HeaderText = "Order Stock";
////                dataGridView2.Columns["BarCode"].HeaderText = "Barcode";
////                dataGridView2.Columns["ProductWeight"].HeaderText = "Weight";
////                dataGridView2.Columns["IsFinishProduct"].HeaderText = "Finish Product";
////            }
////        }

////        #endregion

////        #region Event Handlers

////        private void pictureBox2_Click(object sender, EventArgs e)
////        {
////            this.Close();
////        }

////        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
////        {
////            // When company changes, filter product groups
////            if (cmpnynamecombobox1.SelectedValue != null)
////            {
////                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
////                FilterProductGroupsByCompany(selectedCompanyId);
////            }
////        }

////        private void FilterProductGroupsByCompany(string companyId)
////        {
////            try
////            {
////                DataView dv = new DataView(dtProductGroups);
////                dv.RowFilter = $"CompanyId = '{companyId}'";

////                grpnamecomboBox.DataSource = dv;
////                grpnamecomboBox.DisplayMember = "GroupName";
////                grpnamecomboBox.ValueMember = "GroupId";
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error filtering groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void grpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e)
////        {
////            // When group changes, filter sub groups
////            if (grpnamecomboBox.SelectedValue != null && cmpnynamecombobox1.SelectedValue != null)
////            {
////                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
////                string selectedGroupId = grpnamecomboBox.SelectedValue.ToString();
////                FilterSubGroups(selectedCompanyId, selectedGroupId);
////            }
////        }

////        private void FilterSubGroups(string companyId, string groupId)
////        {
////            try
////            {
////                DataView dv = new DataView(dtSubGroups);
////                dv.RowFilter = $"CompanyId = '{companyId}' AND GroupID = {groupId}";

////                subgrpnamecomboBox.DataSource = dv;
////                subgrpnamecomboBox.DisplayMember = "SubGroupName";
////                subgrpnamecomboBox.ValueMember = "SubGroupId";
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error filtering sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        #endregion

////        #region Button Click Events

////        private void newbtn_Click(object sender, EventArgs e)
////        {
////            ClearForm();
////            ptdcidttxtbox1.Focus();
////        }

////        private void editbtn_Click(object sender, EventArgs e)
////        {
////            if (dataGridView2.SelectedRows.Count > 0)
////            {
////                LoadProductForEdit();
////            }
////            else
////            {
////                MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
////            }
////        }

////        private void removebtn_Click(object sender, EventArgs e)
////        {
////            if (dataGridView2.SelectedRows.Count > 0)
////            {
////                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
////                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
////                {
////                    DeleteProduct();
////                }
////            }
////            else
////            {
////                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
////            }
////        }

////        private void savebtn_Click(object sender, EventArgs e)
////        {
////            if (ValidateForm())
////            {
////                SaveProduct();
////            }
////        }

////        private void clearbtn_Click(object sender, EventArgs e)
////        {
////            ClearForm();
////        }

////        private void closebtn_Click(object sender, EventArgs e)
////        {
////            this.Close();
////        }

////        #endregion

////        #region CRUD Operations

////        private void LoadProductForEdit()
////        {
////            try
////            {
////                DataGridViewRow row = dataGridView2.SelectedRows[0];

////                ptdcidttxtbox1.Text = row.Cells["ProductID"].Value.ToString();
////                prdctnametxtbox.Text = row.Cells["ProductName"].Value.ToString();

////                // Set company
////                string companyName = row.Cells["CompanyName"].Value.ToString();
////                cmpnynamecombobox1.SelectedIndex = cmpnynamecombobox1.FindStringExact(companyName);

////                // Set group
////                string groupName = row.Cells["GroupName"].Value.ToString();
////                grpnamecomboBox.SelectedIndex = grpnamecomboBox.FindStringExact(groupName);

////                // Set sub group
////                string subGroupName = row.Cells["SubGroupName"].Value.ToString();
////                subgrpnamecomboBox.SelectedIndex = subgrpnamecomboBox.FindStringExact(subGroupName);

////                purchasepricetxtbox1.Text = row.Cells["PurchasePrice"].Value.ToString();
////                salepricetxtbox2.Text = row.Cells["SalePrice"].Value.ToString();
////                retailpricetxtbox.Text = row.Cells["RetailPrice"].Value.ToString();
////                orderstocktxtbox.Text = row.Cells["OrderStockLevel"].Value.ToString();
////                barcodetxtbox.Text = row.Cells["BarCode"].Value.ToString();
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void SaveProduct()
////        {
////            try
////            {
////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    conn.Open();

////                    string productId = ptdcidttxtbox1.Text.Trim();
////                    string companyId = cmpnynamecombobox1.SelectedValue?.ToString();
////                    string groupId = grpnamecomboBox.SelectedValue?.ToString();
////                    string subGroupId = subgrpnamecomboBox.SelectedValue?.ToString();
////                    string packingId = purchasepackcombobox.SelectedValue?.ToString();
////                    string salePackingId = salepackcombobox.SelectedValue?.ToString();

////                    // Check if product exists
////                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
////                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
////                    checkCmd.Parameters.AddWithValue("@ProductID", productId);
////                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

////                    string query;
////                    if (exists > 0)
////                    {
////                        // Update existing product
////                        query = @"UPDATE Products SET 
////                                CompanyID = @CompanyID,
////                                GroupId = @GroupId,
////                                SubGroupId = @SubGroupId,
////                                ProductName = @ProductName,
////                                PurchasePrice = @PurchasePrice,
////                                SalePrice = @SalePrice,
////                                RetailPrice = @RetailPrice,
////                                PurPackingId = @PurPackingId,
////                                SalPackingId = @SalPackingId,
////                                OrderStockLevel = @OrderStockLevel,
////                                BarCode = @BarCode,
////                                ProductWeight = @ProductWeight
////                                WHERE ProductID = @ProductID";
////                    }
////                    else
////                    {
////                        // Insert new product
////                        query = @"INSERT INTO Products 
////                                (ProductID, CompanyID, GroupId, SubGroupId, ProductName, 
////                                PurchasePrice, SalePrice, RetailPrice, PurPackingId, 
////                                SalPackingId, OrderStockLevel, BarCode, ProductWeight,
////                                PurDiscRatio, SaleDiscRatio, PurDiscValue, SaleDiscValue,
////                                PurSTRatio, SaleSTRatio, MinStockLevel, IsFinishProduct)
////                                VALUES 
////                                (@ProductID, @CompanyID, @GroupId, @SubGroupId, @ProductName,
////                                @PurchasePrice, @SalePrice, @RetailPrice, @PurPackingId,
////                                @SalPackingId, @OrderStockLevel, @BarCode, @ProductWeight,
////                                0, 0, 0, 0, 0, 0, 0, 0)";
////                    }

////                    SqlCommand cmd = new SqlCommand(query, conn);
////                    cmd.Parameters.AddWithValue("@ProductID", productId);
////                    cmd.Parameters.AddWithValue("@CompanyID", companyId ?? (object)DBNull.Value);
////                    cmd.Parameters.AddWithValue("@GroupId", groupId ?? (object)DBNull.Value);
////                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId ?? (object)DBNull.Value);
////                    cmd.Parameters.AddWithValue("@ProductName", prdctnametxtbox.Text.Trim());
////                    cmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(purchasepricetxtbox1.Text));
////                    cmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(salepricetxtbox2.Text));
////                    cmd.Parameters.AddWithValue("@RetailPrice", Convert.ToDecimal(retailpricetxtbox.Text));
////                    cmd.Parameters.AddWithValue("@PurPackingId", packingId ?? (object)DBNull.Value);
////                    cmd.Parameters.AddWithValue("@SalPackingId", salePackingId ?? (object)DBNull.Value);
////                    cmd.Parameters.AddWithValue("@OrderStockLevel", Convert.ToInt32(orderstocktxtbox.Text));
////                    cmd.Parameters.AddWithValue("@BarCode", barcodetxtbox.Text.Trim());
////                    cmd.Parameters.AddWithValue("@ProductWeight", Convert.ToDecimal(packinloseweighttxtbox.Text));

////                    cmd.ExecuteNonQuery();

////                    MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
////                    LoadProductsGrid();
////                    ClearForm();
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private void DeleteProduct()
////        {
////            try
////            {
////                string productId = dataGridView2.SelectedRows[0].Cells["ProductID"].Value.ToString();

////                using (SqlConnection conn = new SqlConnection(connectionString))
////                {
////                    conn.Open();
////                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";
////                    SqlCommand cmd = new SqlCommand(query, conn);
////                    cmd.Parameters.AddWithValue("@ProductID", productId);
////                    cmd.ExecuteNonQuery();

////                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
////                    LoadProductsGrid();
////                }
////            }
////            catch (Exception ex)
////            {
////                MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
////            }
////        }

////        private bool ValidateForm()
////        {
////            if (string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
////            {
////                MessageBox.Show("Please enter Product ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
////                ptdcidttxtbox1.Focus();
////                return false;
////            }

////            if (string.IsNullOrWhiteSpace(prdctnametxtbox.Text))
////            {
////                MessageBox.Show("Please enter Product Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
////                prdctnametxtbox.Focus();
////                return false;
////            }

////            if (cmpnynamecombobox1.SelectedIndex == -1)
////            {
////                MessageBox.Show("Please select a Company", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
////                cmpnynamecombobox1.Focus();
////                return false;
////            }

////            return true;
////        }

////        private void ClearForm()
////        {
////            ptdcidttxtbox1.Clear();
////            prdctnametxtbox.Clear();
////            cmpnynamecombobox1.SelectedIndex = -1;
////            grpnamecomboBox.SelectedIndex = -1;
////            subgrpnamecomboBox.SelectedIndex = -1;
////            purchasepackcombobox.SelectedIndex = -1;
////            salepackcombobox.SelectedIndex = -1;
////            packinloseweighttxtbox.Clear();
////            retailpricetxtbox.Clear();
////            purchasepricetxtbox1.Clear();
////            salepricetxtbox2.Clear();
////            purchasedisctxtbox1.Clear();
////            saledisctxtbox2.Clear();
////            purchasediscvaltxtbox1.Clear();
////            putchasediscvaltxtbox2.Clear();
////            sttxtbox.Clear();
////            stvaltxtbox.Clear();
////            orderstocktxtbox.Clear();
////            barcodetxtbox.Clear();
////            genbarcodetxtbox.Clear();
////        }

////        #endregion

////        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {
////            // Handle grid cell clicks if needed
////        }

////        private void dataGridViewavlblepkng_CellContentClick(object sender, DataGridViewCellEventArgs e)
////        {
////            // Handle available packings grid clicks if needed
////        }

////        // Other event handlers remain as they were
////        private void ptdcidttxtbox1_TextChanged(object sender, EventArgs e) { }
////        private void prdctnametxtbox_TextChanged(object sender, EventArgs e) { }
////        private void notxtbox_TextChanged(object sender, EventArgs e) { }
////        private void subgrpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e) { }
////        private void slctcmpnycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
////        private void slctprdctgrpcomboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
////        private void purchasepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
////        private void salepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
////        private void packinloseweighttxtbox_TextChanged(object sender, EventArgs e) { }
////        private void retailpricetxtbox_TextChanged(object sender, EventArgs e) { }
////        private void purchasepricetxtbox1_TextChanged(object sender, EventArgs e) { }
////        private void salepricetxtbox2_TextChanged(object sender, EventArgs e) { }
////        private void purchasedisctxtbox1_TextChanged(object sender, EventArgs e) { }
////        private void saledisctxtbox2_TextChanged(object sender, EventArgs e) { }
////        private void purchasediscvaltxtbox1_TextChanged(object sender, EventArgs e) { }
////        private void putchasediscvaltxtbox2_TextChanged(object sender, EventArgs e) { }
////        private void sttxtbox_TextChanged(object sender, EventArgs e) { }
////        private void stvaltxtbox_TextChanged(object sender, EventArgs e) { }
////        private void orderstocktxtbox_TextChanged(object sender, EventArgs e) { }
////        private void barcodetxtbox_TextChanged(object sender, EventArgs e) { }
////        private void genbarcodetxtbox_TextChanged(object sender, EventArgs e) { }
////    }
////}





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
//    public partial class stockalertlvltxtbox : Form
//    {
//        // Connection string
//        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

//        // DataTables for storing data
//        private DataTable dtCompanies;
//        private DataTable dtProductGroups;
//        private DataTable dtSubGroups;
//        private DataTable dtPackings;
//        private DataTable dtProducts;

//        public stockalertlvltxtbox()
//        {
//            InitializeComponent();
//        }
//        private int SafeParseInt(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return 0;

//            text = text.Trim();

//            if (int.TryParse(text, out int result))
//                return result;

//            return 0; // default value, ya phir aap according to requirement
//        }

//        private void Products_Load(object sender, EventArgs e)
//        {
//            LoadAllData();
//        }

//        private void LoadAllData()
//        {
//            try
//            {
//                // Load Companies
//                LoadCompanies();

//                // Load Product Groups
//                LoadProductGroups();

//                // Load Sub Groups
//                LoadSubGroups();

//                // Load Packings
//                LoadPackings();

//                // Load Products Grid
//                LoadProductsGrid();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        #region Data Loading Methods

//        private void LoadCompanies()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
//                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
//                    dtCompanies = new DataTable();
//                    da.Fill(dtCompanies);

//                    // Bind to combobox
//                    cmpnynamecombobox1.DataSource = dtCompanies.Copy();
//                    cmpnynamecombobox1.DisplayMember = "CompanyName";
//                    cmpnynamecombobox1.ValueMember = "CompanyID";

//                    // Also bind to other company combobox if exists
//                    if (slctcmpnycombobox != null)
//                    {
//                        slctcmpnycombobox.DataSource = dtCompanies.Copy();
//                        slctcmpnycombobox.DisplayMember = "CompanyName";
//                        slctcmpnycombobox.ValueMember = "CompanyID";
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadProductGroups()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT CompanyId, GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
//                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
//                    dtProductGroups = new DataTable();
//                    da.Fill(dtProductGroups);

//                    // Ensure columns exist
//                    if (!dtProductGroups.Columns.Contains("CompanyId"))
//                        dtProductGroups.Columns.Add("CompanyId", typeof(string));
//                    if (!dtProductGroups.Columns.Contains("GroupId"))
//                        dtProductGroups.Columns.Add("GroupId", typeof(int));
//                    if (!dtProductGroups.Columns.Contains("GroupName"))
//                        dtProductGroups.Columns.Add("GroupName", typeof(string));

//                    // Bind to combobox
//                    grpnamecomboBox.DataSource = dtProductGroups.Copy();
//                    grpnamecomboBox.DisplayMember = "GroupName";
//                    grpnamecomboBox.ValueMember = "GroupId";

//                    // Also bind to other product group combobox if exists
//                    if (slctprdctgrpcomboBox1 != null)
//                    {
//                        slctprdctgrpcomboBox1.DataSource = dtProductGroups.Copy();
//                        slctprdctgrpcomboBox1.DisplayMember = "GroupName";
//                        slctprdctgrpcomboBox1.ValueMember = "GroupId";
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadSubGroups()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"SELECT sg.GroupID, sg.SubGroupId, sg.CompanyId, sg.SubGroupName 
//                                   FROM ProductSubGroups sg
//                                   INNER JOIN ProductGroups pg ON sg.CompanyId = pg.CompanyId AND sg.GroupID = pg.GroupId
//                                   ORDER BY sg.SubGroupName";
//                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
//                    dtSubGroups = new DataTable();
//                    da.Fill(dtSubGroups);

//                    // Ensure columns exist
//                    if (!dtSubGroups.Columns.Contains("CompanyId"))
//                        dtSubGroups.Columns.Add("CompanyId", typeof(string));
//                    if (!dtSubGroups.Columns.Contains("GroupID"))
//                        dtSubGroups.Columns.Add("GroupID", typeof(int));
//                    if (!dtSubGroups.Columns.Contains("SubGroupId"))
//                        dtSubGroups.Columns.Add("SubGroupId", typeof(int));
//                    if (!dtSubGroups.Columns.Contains("SubGroupName"))
//                        dtSubGroups.Columns.Add("SubGroupName", typeof(string));

//                    // Bind to combobox
//                    subgrpnamecomboBox.DataSource = dtSubGroups.Copy();
//                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
//                    subgrpnamecomboBox.ValueMember = "SubGroupId";
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadPackings()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
//                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
//                    dtPackings = new DataTable();
//                    da.Fill(dtPackings);

//                    // Bind to purchase pack combobox
//                    purchasepackcombobox.DataSource = dtPackings.Copy();
//                    purchasepackcombobox.DisplayMember = "PackingName";
//                    purchasepackcombobox.ValueMember = "PackingId";

//                    // Bind to sale pack combobox
//                    salepackcombobox.DataSource = dtPackings.Copy();
//                    salepackcombobox.DisplayMember = "PackingName";
//                    salepackcombobox.ValueMember = "PackingId";
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void LoadProductsGrid()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    string query = @"SELECT 
//                                    p.ProductID,
//                                    c.CompanyName,
//                                    pg.GroupName,
//                                    psg.SubGroupName,
//                                    p.ProductName,
//                                    p.PurchasePrice,
//                                    p.SalePrice,
//                                    p.RetailPrice,
//                                    p.MinStockLevel,
//                                    p.OrderStockLevel,
//                                    p.BarCode,
//                                    p.ProductWeight,
//                                    p.IsFinishProduct
//                                FROM Products p
//                                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
//                                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
//                                LEFT JOIN ProductSubGroups psg ON p.CompanyID = psg.CompanyId AND p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId
//                                ORDER BY p.ProductID";

//                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
//                    dtProducts = new DataTable();
//                    da.Fill(dtProducts);

//                    // Bind to DataGridView
//                    dataGridView2.DataSource = dtProducts;
//                    FormatProductsGrid();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void FormatProductsGrid()
//        {
//            if (dataGridView2.Columns.Count > 0)
//            {
//                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
//                dataGridView2.Columns["ProductID"].HeaderText = "Product ID";
//                dataGridView2.Columns["CompanyName"].HeaderText = "Company";
//                dataGridView2.Columns["GroupName"].HeaderText = "Group";
//                dataGridView2.Columns["SubGroupName"].HeaderText = "Sub Group";
//                dataGridView2.Columns["ProductName"].HeaderText = "Product Name";
//                dataGridView2.Columns["PurchasePrice"].HeaderText = "Purchase Price";
//                dataGridView2.Columns["SalePrice"].HeaderText = "Sale Price";
//                dataGridView2.Columns["RetailPrice"].HeaderText = "Retail Price";
//                dataGridView2.Columns["MinStockLevel"].HeaderText = "Min Stock";
//                dataGridView2.Columns["OrderStockLevel"].HeaderText = "Order Stock";
//                dataGridView2.Columns["BarCode"].HeaderText = "Barcode";
//                dataGridView2.Columns["ProductWeight"].HeaderText = "Weight";
//                dataGridView2.Columns["IsFinishProduct"].HeaderText = "Finish Product";
//            }
//        }

//        #endregion

//        #region Event Handlers

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            // When company changes, filter product groups
//            if (cmpnynamecombobox1.SelectedValue != null && dtProductGroups != null && dtProductGroups.Rows.Count > 0)
//            {
//                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
//                FilterProductGroupsByCompany(selectedCompanyId);
//            }
//            else
//            {
//                ResetGroupComboBoxes();
//            }
//        }

//        private void FilterProductGroupsByCompany(string companyId)
//        {
//            try
//            {
//                if (dtProductGroups == null || dtProductGroups.Rows.Count == 0)
//                {
//                    ResetGroupComboBoxes();
//                    return;
//                }

//                DataView dv = new DataView(dtProductGroups);
//                dv.RowFilter = $"CompanyId = '{companyId}'";

//                // Create a new DataTable from the filtered DataView
//                DataTable filteredTable = dv.ToTable();

//                // Check if the filtered table has rows
//                if (filteredTable.Rows.Count > 0)
//                {
//                    grpnamecomboBox.DataSource = filteredTable;
//                    grpnamecomboBox.DisplayMember = "GroupName";
//                    grpnamecomboBox.ValueMember = "GroupId";
//                    grpnamecomboBox.Enabled = true;
//                }
//                else
//                {
//                    // If no groups for this company, reset the combobox
//                    grpnamecomboBox.DataSource = null;
//                    grpnamecomboBox.Items.Clear();
//                    grpnamecomboBox.Text = "";
//                    grpnamecomboBox.Enabled = false;

//                    // Also reset sub group combobox
//                    subgrpnamecomboBox.DataSource = null;
//                    subgrpnamecomboBox.Items.Clear();
//                    subgrpnamecomboBox.Text = "";
//                    subgrpnamecomboBox.Enabled = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error filtering groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                ResetGroupComboBoxes();
//            }
//        }

//        private void grpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            // When group changes, filter sub groups
//            if (grpnamecomboBox.SelectedValue != null && cmpnynamecombobox1.SelectedValue != null
//                && dtSubGroups != null && dtSubGroups.Rows.Count > 0)
//            {
//                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
//                string selectedGroupId = grpnamecomboBox.SelectedValue.ToString();
//                FilterSubGroups(selectedCompanyId, selectedGroupId);
//            }
//            else
//            {
//                ResetSubGroupComboBox();
//            }
//        }

//        private void FilterSubGroups(string companyId, string groupId)
//        {
//            try
//            {
//                if (dtSubGroups == null || dtSubGroups.Rows.Count == 0)
//                {
//                    ResetSubGroupComboBox();
//                    return;
//                }

//                DataView dv = new DataView(dtSubGroups);
//                dv.RowFilter = $"CompanyId = '{companyId}' AND GroupID = {groupId}";

//                // Create a new DataTable from the filtered DataView
//                DataTable filteredTable = dv.ToTable();

//                // Check if the filtered table has rows
//                if (filteredTable.Rows.Count > 0)
//                {
//                    subgrpnamecomboBox.DataSource = filteredTable;
//                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
//                    subgrpnamecomboBox.ValueMember = "SubGroupId";
//                    subgrpnamecomboBox.Enabled = true;
//                }
//                else
//                {
//                    // If no sub-groups for this group, reset the combobox
//                    subgrpnamecomboBox.DataSource = null;
//                    subgrpnamecomboBox.Items.Clear();
//                    subgrpnamecomboBox.Text = "";
//                    subgrpnamecomboBox.Enabled = false;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error filtering sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                ResetSubGroupComboBox();
//            }
//        }

//        private void ResetGroupComboBoxes()
//        {
//            grpnamecomboBox.DataSource = null;
//            grpnamecomboBox.Items.Clear();
//            grpnamecomboBox.Text = "";
//            grpnamecomboBox.Enabled = false;

//            ResetSubGroupComboBox();
//        }

//        private void ResetSubGroupComboBox()
//        {
//            subgrpnamecomboBox.DataSource = null;
//            subgrpnamecomboBox.Items.Clear();
//            subgrpnamecomboBox.Text = "";
//            subgrpnamecomboBox.Enabled = false;
//        }

//        #endregion

//        #region Button Click Events

//        private void newbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//            ptdcidttxtbox1.Focus();
//        }

//        private void editbtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView2.SelectedRows.Count > 0)
//            {
//                LoadProductForEdit();
//            }
//            else
//            {
//                MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (dataGridView2.SelectedRows.Count > 0)
//            {
//                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
//                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
//                {
//                    DeleteProduct();
//                }
//            }
//            else
//            {
//                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (ValidateForm())
//            {
//                SaveProduct();
//            }
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }

//        #endregion

//        #region CRUD Operations

//        private void LoadProductForEdit()
//        {
//            try
//            {
//                DataGridViewRow row = dataGridView2.SelectedRows[0];

//                ptdcidttxtbox1.Text = row.Cells["ProductID"].Value.ToString();
//                prdctnametxtbox.Text = row.Cells["ProductName"].Value.ToString();

//                // Set company
//                string companyName = row.Cells["CompanyName"].Value.ToString();
//                foreach (DataRowView item in cmpnynamecombobox1.Items)
//                {
//                    if (item["CompanyName"].ToString() == companyName)
//                    {
//                        cmpnynamecombobox1.SelectedItem = item;
//                        break;
//                    }
//                }

//                // Set group
//                string groupName = row.Cells["GroupName"].Value.ToString();
//                foreach (DataRowView item in grpnamecomboBox.Items)
//                {
//                    if (item["GroupName"].ToString() == groupName)
//                    {
//                        grpnamecomboBox.SelectedItem = item;
//                        break;
//                    }
//                }

//                // Set sub group
//                string subGroupName = row.Cells["SubGroupName"].Value.ToString();
//                foreach (DataRowView item in subgrpnamecomboBox.Items)
//                {
//                    if (item["SubGroupName"].ToString() == subGroupName)
//                    {
//                        subgrpnamecomboBox.SelectedItem = item;
//                        break;
//                    }
//                }

//                purchasepricetxtbox1.Text = row.Cells["PurchasePrice"].Value.ToString();
//                salepricetxtbox2.Text = row.Cells["SalePrice"].Value.ToString();
//                retailpricetxtbox.Text = row.Cells["RetailPrice"].Value.ToString();
//                orderstocktxtbox.Text = row.Cells["OrderStockLevel"].Value.ToString();
//                barcodetxtbox.Text = row.Cells["BarCode"]?.Value?.ToString() ?? "";

//                // Load other fields if they exist in the grid
//                if (row.Cells["ProductWeight"].Value != null)
//                    packinloseweighttxtbox.Text = row.Cells["ProductWeight"].Value.ToString();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        //private void SaveProduct()
//        //{
//        //    try
//        //    {
//        //        using (SqlConnection conn = new SqlConnection(connectionString))
//        //        {
//        //            conn.Open();

//        //            string productId = ptdcidttxtbox1.Text.Trim();
//        //            string companyId = cmpnynamecombobox1.SelectedValue?.ToString();
//        //            string groupId = grpnamecomboBox.SelectedValue?.ToString();
//        //            string subGroupId = subgrpnamecomboBox.SelectedValue?.ToString();
//        //            string packingId = purchasepackcombobox.SelectedValue?.ToString();
//        //            string salePackingId = salepackcombobox.SelectedValue?.ToString();

//        //            // Check if product exists
//        //            string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
//        //            SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
//        //            checkCmd.Parameters.AddWithValue("@ProductID", productId);
//        //            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

//        //            string query;
//        //            if (exists > 0)
//        //            {
//        //                // Update existing product
//        //                query = @"UPDATE Products SET 
//        //                        CompanyID = @CompanyID,
//        //                        GroupId = @GroupId,
//        //                        SubGroupId = @SubGroupId,
//        //                        ProductName = @ProductName,
//        //                        PurchasePrice = @PurchasePrice,
//        //                        SalePrice = @SalePrice,
//        //                        RetailPrice = @RetailPrice,
//        //                        PurPackingId = @PurPackingId,
//        //                        SalPackingId = @SalPackingId,
//        //                        OrderStockLevel = @OrderStockLevel,
//        //                        BarCode = @BarCode,
//        //                        ProductWeight = @ProductWeight
//        //                        WHERE ProductID = @ProductID";
//        //            }
//        //            else
//        //            {
//        //                // Insert new product
//        //                query = @"INSERT INTO Products 
//        //                        (ProductID, CompanyID, GroupId, SubGroupId, ProductName, 
//        //                        PurchasePrice, SalePrice, RetailPrice, PurPackingId, 
//        //                        SalPackingId, OrderStockLevel, BarCode, ProductWeight,
//        //                        PurDiscRatio, SaleDiscRatio, PurDiscValue, SaleDiscValue,
//        //                        PurSTRatio, SaleSTRatio, MinStockLevel, IsFinishProduct)
//        //                        VALUES 
//        //                        (@ProductID, @CompanyID, @GroupId, @SubGroupId, @ProductName,
//        //                        @PurchasePrice, @SalePrice, @RetailPrice, @PurPackingId,
//        //                        @SalPackingId, @OrderStockLevel, @BarCode, @ProductWeight,
//        //                        0, 0, 0, 0, 0, 0, 0, 0)";
//        //            }

//        //            SqlCommand cmd = new SqlCommand(query, conn);
//        //            cmd.Parameters.AddWithValue("@ProductID", productId);
//        //            cmd.Parameters.AddWithValue("@CompanyID", companyId ?? (object)DBNull.Value);
//        //            cmd.Parameters.AddWithValue("@GroupId", groupId ?? (object)DBNull.Value);
//        //            cmd.Parameters.AddWithValue("@SubGroupId", subGroupId ?? (object)DBNull.Value);
//        //            cmd.Parameters.AddWithValue("@ProductName", prdctnametxtbox.Text.Trim());
//        //            cmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(purchasepricetxtbox1.Text));
//        //            cmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(salepricetxtbox2.Text));
//        //            cmd.Parameters.AddWithValue("@RetailPrice", Convert.ToDecimal(retailpricetxtbox.Text));
//        //            cmd.Parameters.AddWithValue("@PurPackingId", packingId ?? (object)DBNull.Value);
//        //            cmd.Parameters.AddWithValue("@SalPackingId", salePackingId ?? (object)DBNull.Value);
//        //            cmd.Parameters.AddWithValue("@OrderStockLevel", Convert.ToInt32(orderstocktxtbox.Text));
//        //            cmd.Parameters.AddWithValue("@BarCode", barcodetxtbox.Text.Trim());
//        //            cmd.Parameters.AddWithValue("@ProductWeight", Convert.ToDecimal(packinloseweighttxtbox.Text));
//        //            cmd.Parameters.AddWithValue("@PurchasePrice", SafeParseDecimal(purchasepricetxtbox1.Text));
//        //            cmd.Parameters.AddWithValue("@SalePrice", SafeParseDecimal(salepricetxtbox2.Text));
//        //            cmd.Parameters.AddWithValue("@RetailPrice", SafeParseDecimal(retailpricetxtbox.Text));
//        //            cmd.Parameters.AddWithValue("@OrderStockLevel", SafeParseInt(orderstocktxtbox.Text));
//        //            cmd.Parameters.AddWithValue("@ProductWeight", SafeParseDecimal(packinloseweighttxtbox.Text));

//        //            cmd.ExecuteNonQuery();

//        //            MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        //            LoadProductsGrid();
//        //            ClearForm();
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//        //    }
//        //}

//        private void SaveProduct()
//        {
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string productId = ptdcidttxtbox1.Text.Trim();
//                    string companyId = cmpnynamecombobox1.SelectedValue?.ToString();
//                    string groupId = grpnamecomboBox.SelectedValue?.ToString();
//                    string subGroupId = subgrpnamecomboBox.SelectedValue?.ToString();
//                    string packingId = purchasepackcombobox.SelectedValue?.ToString();
//                    string salePackingId = salepackcombobox.SelectedValue?.ToString();

//                    // Check if product exists
//                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
//                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
//                    checkCmd.Parameters.AddWithValue("@ProductID", productId);
//                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

//                    string query;
//                    if (exists > 0)
//                    {
//                        // Update existing product
//                        query = @"UPDATE Products SET 
//                        CompanyID = @CompanyID,
//                        GroupId = @GroupId,
//                        SubGroupId = @SubGroupId,
//                        ProductName = @ProductName,
//                        PurchasePrice = @PurchasePrice,
//                        SalePrice = @SalePrice,
//                        RetailPrice = @RetailPrice,
//                        PurPackingId = @PurPackingId,
//                        SalPackingId = @SalPackingId,
//                        OrderStockLevel = @OrderStockLevel,
//                        BarCode = @BarCode,
//                        ProductWeight = @ProductWeight
//                        WHERE ProductID = @ProductID";
//                    }
//                    else
//                    {
//                        // Insert new product
//                        query = @"INSERT INTO Products 
//                        (ProductID, CompanyID, GroupId, SubGroupId, ProductName, 
//                        PurchasePrice, SalePrice, RetailPrice, PurPackingId, 
//                        SalPackingId, OrderStockLevel, BarCode, ProductWeight,
//                        PurDiscRatio, SaleDiscRatio, PurDiscValue, SaleDiscValue,
//                        PurSTRatio, SaleSTRatio, MinStockLevel, IsFinishProduct)
//                        VALUES 
//                        (@ProductID, @CompanyID, @GroupId, @SubGroupId, @ProductName,
//                        @PurchasePrice, @SalePrice, @RetailPrice, @PurPackingId,
//                        @SalPackingId, @OrderStockLevel, @BarCode, @ProductWeight,
//                        0, 0, 0, 0, 0, 0, 0, 0)";
//                    }

//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@ProductID", productId);
//                    cmd.Parameters.AddWithValue("@CompanyID", companyId ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@GroupId", groupId ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@ProductName", prdctnametxtbox.Text.Trim());
//                    cmd.Parameters.AddWithValue("@PurPackingId", packingId ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@SalPackingId", salePackingId ?? (object)DBNull.Value);
//                    cmd.Parameters.AddWithValue("@BarCode", barcodetxtbox.Text.Trim());

//                    // SIRF YEH LINES RAKHEIN (SafeParse wali):
//                    cmd.Parameters.AddWithValue("@PurchasePrice", SafeParseDecimal(purchasepricetxtbox1.Text));
//                    cmd.Parameters.AddWithValue("@SalePrice", SafeParseDecimal(salepricetxtbox2.Text));
//                    cmd.Parameters.AddWithValue("@RetailPrice", SafeParseDecimal(retailpricetxtbox.Text));
//                    cmd.Parameters.AddWithValue("@OrderStockLevel", SafeParseInt(orderstocktxtbox.Text));
//                    cmd.Parameters.AddWithValue("@ProductWeight", SafeParseDecimal(packinloseweighttxtbox.Text));

//                    cmd.ExecuteNonQuery();

//                    MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    LoadProductsGrid();
//                    ClearForm();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }


//        private void DeleteProduct()
//        {
//            try
//            {
//                string productId = dataGridView2.SelectedRows[0].Cells["ProductID"].Value.ToString();

//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@ProductID", productId);
//                    cmd.ExecuteNonQuery();

//                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    LoadProductsGrid();
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        //private bool ValidateForm()
//        //{
//        //    if (string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
//        //    {
//        //        MessageBox.Show("Please enter Product ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        ptdcidttxtbox1.Focus();
//        //        return false;
//        //    }

//        //    if (string.IsNullOrWhiteSpace(prdctnametxtbox.Text))
//        //    {
//        //        MessageBox.Show("Please enter Product Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        prdctnametxtbox.Focus();
//        //        return false;
//        //    }

//        //    if (cmpnynamecombobox1.SelectedIndex == -1)
//        //    {
//        //        MessageBox.Show("Please select a Company", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        cmpnynamecombobox1.Focus();
//        //        return false;
//        //    }

//        //    // Validate numeric fields
//        //    if (!decimal.TryParse(purchasepricetxtbox1.Text, out decimal purchasePrice) || purchasePrice < 0)
//        //    {
//        //        MessageBox.Show("Please enter a valid Purchase Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        purchasepricetxtbox1.Focus();
//        //        return false;
//        //    }

//        //    if (!decimal.TryParse(salepricetxtbox2.Text, out decimal salePrice) || salePrice < 0)
//        //    {
//        //        MessageBox.Show("Please enter a valid Sale Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//        //        salepricetxtbox2.Focus();
//        //        return false;
//        //    }

//        //    return true;
//        //}

//        private bool ValidateForm()
//        {
//            // ... existing validations ...

//            // Validate ALL numeric fields
//            if (!decimal.TryParse(purchasepricetxtbox1.Text, out decimal purchasePrice) || purchasePrice < 0)
//            {
//                MessageBox.Show("Please enter a valid Purchase Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                purchasepricetxtbox1.Focus();
//                return false;
//            }

//            if (!decimal.TryParse(salepricetxtbox2.Text, out decimal salePrice) || salePrice < 0)
//            {
//                MessageBox.Show("Please enter a valid Sale Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                salepricetxtbox2.Focus();
//                return false;
//            }

//            // Add these validations:
//            if (!decimal.TryParse(retailpricetxtbox.Text, out decimal retailPrice) || retailPrice < 0)
//            {
//                MessageBox.Show("Please enter a valid Retail Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                retailpricetxtbox.Focus();
//                return false;
//            }

//            if (!int.TryParse(orderstocktxtbox.Text, out int orderStock) || orderStock < 0)
//            {
//                MessageBox.Show("Please enter a valid Order Stock Level", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                orderstocktxtbox.Focus();
//                return false;
//            }

//            if (!decimal.TryParse(packinloseweighttxtbox.Text, out decimal productWeight) || productWeight < 0)
//            {
//                MessageBox.Show("Please enter a valid Product Weight", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                packinloseweighttxtbox.Focus();
//                return false;
//            }

//            return true;
//        }

//        private decimal SafeParseDecimal(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text))
//                return 0;

//            // Remove any spaces and handle culture-specific formats
//            text = text.Trim();
//            // Replace comma with dot if needed
//            text = text.Replace(",", ".");

//            if (decimal.TryParse(text, out decimal result))
//                return result;

//            return 0; // or throw exception
//        }

//        private void ClearForm()
//        {
//            ptdcidttxtbox1.Clear();
//            prdctnametxtbox.Clear();
//            cmpnynamecombobox1.SelectedIndex = -1;
//            grpnamecomboBox.DataSource = null;
//            grpnamecomboBox.Items.Clear();
//            grpnamecomboBox.Text = "";
//            subgrpnamecomboBox.DataSource = null;
//            subgrpnamecomboBox.Items.Clear();
//            subgrpnamecomboBox.Text = "";
//            purchasepackcombobox.SelectedIndex = -1;
//            salepackcombobox.SelectedIndex = -1;
//            packinloseweighttxtbox.Clear();
//            retailpricetxtbox.Clear();
//            purchasepricetxtbox1.Clear();
//            salepricetxtbox2.Clear();
//            purchasedisctxtbox1.Clear();
//            saledisctxtbox2.Clear();
//            purchasediscvaltxtbox1.Clear();
//            putchasediscvaltxtbox2.Clear();
//            sttxtbox.Clear();
//            stvaltxtbox.Clear();
//            orderstocktxtbox.Clear();
//            barcodetxtbox.Clear();
//            genbarcodetxtbox.Clear();
//        }

//        #endregion

//        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle grid cell clicks if needed
//        }

//        private void dataGridViewavlblepkng_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Handle available packings grid clicks if needed
//        }

//        // Other event handlers remain as they were
//        private void ptdcidttxtbox1_TextChanged(object sender, EventArgs e) { }
//        private void prdctnametxtbox_TextChanged(object sender, EventArgs e) { }
//        private void notxtbox_TextChanged(object sender, EventArgs e) { }
//        private void subgrpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e) { }
//        private void slctcmpnycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
//        private void slctprdctgrpcomboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
//        private void purchasepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
//        private void salepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
//        private void packinloseweighttxtbox_TextChanged(object sender, EventArgs e) { }
//        private void retailpricetxtbox_TextChanged(object sender, EventArgs e) { }
//        private void purchasepricetxtbox1_TextChanged(object sender, EventArgs e) { }
//        private void salepricetxtbox2_TextChanged(object sender, EventArgs e) { }
//        private void purchasedisctxtbox1_TextChanged(object sender, EventArgs e) { }
//        private void saledisctxtbox2_TextChanged(object sender, EventArgs e) { }
//        private void purchasediscvaltxtbox1_TextChanged(object sender, EventArgs e) { }
//        private void putchasediscvaltxtbox2_TextChanged(object sender, EventArgs e) { }
//        private void sttxtbox_TextChanged(object sender, EventArgs e) { }
//        private void stvaltxtbox_TextChanged(object sender, EventArgs e) { }
//        private void orderstocktxtbox_TextChanged(object sender, EventArgs e) { }
//        private void barcodetxtbox_TextChanged(object sender, EventArgs e) { }
//        private void genbarcodetxtbox_TextChanged(object sender, EventArgs e) { }
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

namespace FO_2022
{
    public partial class stockalertlvltxtbox : Form
    {
        // Connection string
        private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";

        // DataTables for storing data
        private DataTable dtCompanies;
        private DataTable dtProductGroups;
        private DataTable dtSubGroups;
        private DataTable dtPackings;
        private DataTable dtProducts;

        public stockalertlvltxtbox()
        {
            InitializeComponent();
        }

        private int SafeParseInt(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = text.Trim();

            if (int.TryParse(text, out int result))
                return result;

            return 0;
        }

        private void Products_Load(object sender, EventArgs e)
        {
            LoadAllData();

            // Event handlers attach karein
            srchprdctnametxtbox.KeyPress += srchprdctnametxtbox_KeyPress;
            slctprdctgrpcomboBox1.Click += slctprdctgrpcomboBox1_Click;
            dataGridViewavlblepkng.SelectionChanged += dataGridViewavlblepkng_SelectionChanged;
        }

        private void LoadAllData()
        {
            try
            {
                // Load Companies
                LoadCompanies();

                // Load Product Groups
                LoadProductGroups();

                // Load Sub Groups
                LoadSubGroups();

                // Load Packings
                LoadPackings();

                // Load Products Grid
                LoadProductsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Data Loading Methods

        private void LoadCompanies()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtCompanies = new DataTable();
                    da.Fill(dtCompanies);

                    // Bind to main company combobox
                    cmpnynamecombobox1.DataSource = dtCompanies.Copy();
                    cmpnynamecombobox1.DisplayMember = "CompanyName";
                    cmpnynamecombobox1.ValueMember = "CompanyID";

                    // REQUIREMENT 1: slctcmpnycombobox ko alag se bind karna
                    if (slctcmpnycombobox != null)
                    {
                        // Alag DataTable copy banayein independent binding ke liye
                        DataTable dtForSearchCombo = dtCompanies.Copy();
                        slctcmpnycombobox.DataSource = dtForSearchCombo;
                        slctcmpnycombobox.DisplayMember = "CompanyName";
                        slctcmpnycombobox.ValueMember = "CompanyID";
                        slctcmpnycombobox.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading companies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyId, GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtProductGroups = new DataTable();
                    da.Fill(dtProductGroups);

                    // Ensure columns exist
                    if (!dtProductGroups.Columns.Contains("CompanyId"))
                        dtProductGroups.Columns.Add("CompanyId", typeof(string));
                    if (!dtProductGroups.Columns.Contains("GroupId"))
                        dtProductGroups.Columns.Add("GroupId", typeof(int));
                    if (!dtProductGroups.Columns.Contains("GroupName"))
                        dtProductGroups.Columns.Add("GroupName", typeof(string));

                    // Bind to main product group combobox
                    grpnamecomboBox.DataSource = dtProductGroups.Copy();
                    grpnamecomboBox.DisplayMember = "GroupName";
                    grpnamecomboBox.ValueMember = "GroupId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // REQUIREMENT 2: slctprdctgrpcomboBox1 click par ProductGroups load karne ka method
        private void LoadProductGroupsForSearchCombo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT GroupId, GroupName FROM ProductGroups ORDER BY GroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dtProductGroupsForSearch = new DataTable();
                    da.Fill(dtProductGroupsForSearch);

                    // Bind to search product group combobox
                    slctprdctgrpcomboBox1.DataSource = dtProductGroupsForSearch;
                    slctprdctgrpcomboBox1.DisplayMember = "GroupName";
                    slctprdctgrpcomboBox1.ValueMember = "GroupId";
                    slctprdctgrpcomboBox1.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubGroups()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT sg.GroupID, sg.SubGroupId, sg.CompanyId, sg.SubGroupName 
                                   FROM ProductSubGroups sg
                                   INNER JOIN ProductGroups pg ON sg.CompanyId = pg.CompanyId AND sg.GroupID = pg.GroupId
                                   ORDER BY sg.SubGroupName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtSubGroups = new DataTable();
                    da.Fill(dtSubGroups);

                    // Ensure columns exist
                    if (!dtSubGroups.Columns.Contains("CompanyId"))
                        dtSubGroups.Columns.Add("CompanyId", typeof(string));
                    if (!dtSubGroups.Columns.Contains("GroupID"))
                        dtSubGroups.Columns.Add("GroupID", typeof(int));
                    if (!dtSubGroups.Columns.Contains("SubGroupId"))
                        dtSubGroups.Columns.Add("SubGroupId", typeof(int));
                    if (!dtSubGroups.Columns.Contains("SubGroupName"))
                        dtSubGroups.Columns.Add("SubGroupName", typeof(string));

                    // Bind to sub group combobox
                    subgrpnamecomboBox.DataSource = dtSubGroups.Copy();
                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
                    subgrpnamecomboBox.ValueMember = "SubGroupId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPackings()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT PackingId, PackingName FROM Packings ORDER BY PackingName";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtPackings = new DataTable();
                    da.Fill(dtPackings);

                    // Bind to purchase pack combobox
                    purchasepackcombobox.DataSource = dtPackings.Copy();
                    purchasepackcombobox.DisplayMember = "PackingName";
                    purchasepackcombobox.ValueMember = "PackingId";

                    // Bind to sale pack combobox
                    salepackcombobox.DataSource = dtPackings.Copy();
                    salepackcombobox.DisplayMember = "PackingName";
                    salepackcombobox.ValueMember = "PackingId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading packings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductsGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                    p.ProductID,
                                    c.CompanyName,
                                    pg.GroupName,
                                    psg.SubGroupName,
                                    p.ProductName,
                                    p.PurchasePrice,
                                    p.SalePrice,
                                    p.RetailPrice,
                                    p.MinStockLevel,
                                    p.OrderStockLevel,
                                    p.BarCode,
                                    p.ProductWeight,
                                    p.IsFinishProduct
                                FROM Products p
                                LEFT JOIN Companies c ON p.CompanyID = c.CompanyID
                                LEFT JOIN ProductGroups pg ON p.CompanyID = pg.CompanyId AND p.GroupId = pg.GroupId
                                LEFT JOIN ProductSubGroups psg ON p.CompanyID = psg.CompanyId AND p.GroupId = psg.GroupID AND p.SubGroupId = psg.SubGroupId
                                ORDER BY p.ProductID";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    dtProducts = new DataTable();
                    da.Fill(dtProducts);

                    // Bind to DataGridView
                    dataGridView2.DataSource = dtProducts;
                    FormatProductsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatProductsGrid()
        {
            if (dataGridView2.Columns.Count > 0)
            {
                dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView2.Columns["ProductID"].HeaderText = "Product ID";
                dataGridView2.Columns["CompanyName"].HeaderText = "Company";
                dataGridView2.Columns["GroupName"].HeaderText = "Group";
                dataGridView2.Columns["SubGroupName"].HeaderText = "Sub Group";
                dataGridView2.Columns["ProductName"].HeaderText = "Product Name";
                dataGridView2.Columns["PurchasePrice"].HeaderText = "Purchase Price";
                dataGridView2.Columns["SalePrice"].HeaderText = "Sale Price";
                dataGridView2.Columns["RetailPrice"].HeaderText = "Retail Price";
                dataGridView2.Columns["MinStockLevel"].HeaderText = "Min Stock";
                dataGridView2.Columns["OrderStockLevel"].HeaderText = "Order Stock";
                dataGridView2.Columns["BarCode"].HeaderText = "Barcode";
                dataGridView2.Columns["ProductWeight"].HeaderText = "Weight";
                dataGridView2.Columns["IsFinishProduct"].HeaderText = "Finish Product";
            }
        }

        #endregion

        #region REQUIREMENT 2: slctprdctgrpcomboBox1 Click Event

        private void slctprdctgrpcomboBox1_Click(object sender, EventArgs e)
        {
            // REQUIREMENT 2: Click karne par ProductGroups table se data fetch ho
            LoadProductGroupsForSearchCombo();
        }

        #endregion

        #region REQUIREMENT 3: Search Product and Show in GridView

        private void srchprdctnametxtbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // REQUIREMENT 3: Enter press karne par search ho
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchProductByName();
            }
        }

        private void SearchProductByName()
        {
            string productName = srchprdctnametxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Please enter product name to search", "Information",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                srchprdctnametxtbox.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT ProductID, ProductName 
                                   FROM Products 
                                   WHERE ProductName LIKE @ProductName 
                                   ORDER BY ProductName";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@ProductName", "%" + productName + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        // REQUIREMENT 3: dataGridViewavlblepkng mein data show ho
                        dataGridViewavlblepkng.DataSource = dt;

                        // Grid formatting
                        dataGridViewavlblepkng.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Header text set karein
                        if (dataGridViewavlblepkng.Columns.Count > 0)
                        {
                            dataGridViewavlblepkng.Columns["ProductID"].HeaderText = "Product ID";
                            dataGridViewavlblepkng.Columns["ProductName"].HeaderText = "Product Name";
                        }

                        // Show message kitne products milay
                        MessageBox.Show($"{dt.Rows.Count} product(s) found", "Search Results",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No products found with this name", "Search Results",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridViewavlblepkng.DataSource = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching products: " + ex.Message, "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region DataGridView Selection Event

        private void dataGridViewavlblepkng_SelectionChanged(object sender, EventArgs e)
        {
            // REQUIREMENT 3: Grid se select karne par textboxes mein data show ho
            if (dataGridViewavlblepkng.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewavlblepkng.SelectedRows[0];

                // ptdcidttxtbox1 mein ProductID show ho
                if (row.Cells["ProductID"].Value != null)
                {
                    ptdcidttxtbox1.Text = row.Cells["ProductID"].Value.ToString();
                }

                // prdctnametxtbox mein ProductName show ho
                if (row.Cells["ProductName"].Value != null)
                {
                    prdctnametxtbox.Text = row.Cells["ProductName"].Value.ToString();
                }
            }
        }

        #endregion

        #region Event Handlers

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Mainform1 mainForm = new Mainform1();
            mainForm.Show();
            this.Hide();
        }

        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When company changes, filter product groups
            if (cmpnynamecombobox1.SelectedValue != null && dtProductGroups != null && dtProductGroups.Rows.Count > 0)
            {
                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
                FilterProductGroupsByCompany(selectedCompanyId);
            }
            else
            {
                ResetGroupComboBoxes();
            }
        }

        private void FilterProductGroupsByCompany(string companyId)
        {
            try
            {
                if (dtProductGroups == null || dtProductGroups.Rows.Count == 0)
                {
                    ResetGroupComboBoxes();
                    return;
                }

                DataView dv = new DataView(dtProductGroups);
                dv.RowFilter = $"CompanyId = '{companyId}'";

                // Create a new DataTable from the filtered DataView
                DataTable filteredTable = dv.ToTable();

                // Check if the filtered table has rows
                if (filteredTable.Rows.Count > 0)
                {
                    grpnamecomboBox.DataSource = filteredTable;
                    grpnamecomboBox.DisplayMember = "GroupName";
                    grpnamecomboBox.ValueMember = "GroupId";
                    grpnamecomboBox.Enabled = true;
                }
                else
                {
                    // If no groups for this company, reset the combobox
                    grpnamecomboBox.DataSource = null;
                    grpnamecomboBox.Items.Clear();
                    grpnamecomboBox.Text = "";
                    grpnamecomboBox.Enabled = false;

                    // Also reset sub group combobox
                    subgrpnamecomboBox.DataSource = null;
                    subgrpnamecomboBox.Items.Clear();
                    subgrpnamecomboBox.Text = "";
                    subgrpnamecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetGroupComboBoxes();
            }
        }

        private void grpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When group changes, filter sub groups
            if (grpnamecomboBox.SelectedValue != null && cmpnynamecombobox1.SelectedValue != null
                && dtSubGroups != null && dtSubGroups.Rows.Count > 0)
            {
                string selectedCompanyId = cmpnynamecombobox1.SelectedValue.ToString();
                string selectedGroupId = grpnamecomboBox.SelectedValue.ToString();
                FilterSubGroups(selectedCompanyId, selectedGroupId);
            }
            else
            {
                ResetSubGroupComboBox();
            }
        }

        private void FilterSubGroups(string companyId, string groupId)
        {
            try
            {
                if (dtSubGroups == null || dtSubGroups.Rows.Count == 0)
                {
                    ResetSubGroupComboBox();
                    return;
                }

                DataView dv = new DataView(dtSubGroups);
                dv.RowFilter = $"CompanyId = '{companyId}' AND GroupID = {groupId}";

                // Create a new DataTable from the filtered DataView
                DataTable filteredTable = dv.ToTable();

                // Check if the filtered table has rows
                if (filteredTable.Rows.Count > 0)
                {
                    subgrpnamecomboBox.DataSource = filteredTable;
                    subgrpnamecomboBox.DisplayMember = "SubGroupName";
                    subgrpnamecomboBox.ValueMember = "SubGroupId";
                    subgrpnamecomboBox.Enabled = true;
                }
                else
                {
                    // If no sub-groups for this group, reset the combobox
                    subgrpnamecomboBox.DataSource = null;
                    subgrpnamecomboBox.Items.Clear();
                    subgrpnamecomboBox.Text = "";
                    subgrpnamecomboBox.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering sub groups: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetSubGroupComboBox();
            }
        }

        private void ResetGroupComboBoxes()
        {
            grpnamecomboBox.DataSource = null;
            grpnamecomboBox.Items.Clear();
            grpnamecomboBox.Text = "";
            grpnamecomboBox.Enabled = false;

            ResetSubGroupComboBox();
        }

        private void ResetSubGroupComboBox()
        {
            subgrpnamecomboBox.DataSource = null;
            subgrpnamecomboBox.Items.Clear();
            subgrpnamecomboBox.Text = "";
            subgrpnamecomboBox.Enabled = false;
        }

        #endregion

        #region Button Click Events

        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            ptdcidttxtbox1.Focus();
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                LoadProductForEdit();
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteProduct();
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveProduct();
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region CRUD Operations

        private void LoadProductForEdit()
        {
            try
            {
                DataGridViewRow row = dataGridView2.SelectedRows[0];

                ptdcidttxtbox1.Text = row.Cells["ProductID"].Value.ToString();
                prdctnametxtbox.Text = row.Cells["ProductName"].Value.ToString();

                // Set company
                string companyName = row.Cells["CompanyName"].Value.ToString();
                foreach (DataRowView item in cmpnynamecombobox1.Items)
                {
                    if (item["CompanyName"].ToString() == companyName)
                    {
                        cmpnynamecombobox1.SelectedItem = item;
                        break;
                    }
                }

                // Set group
                string groupName = row.Cells["GroupName"].Value.ToString();
                foreach (DataRowView item in grpnamecomboBox.Items)
                {
                    if (item["GroupName"].ToString() == groupName)
                    {
                        grpnamecomboBox.SelectedItem = item;
                        break;
                    }
                }

                // Set sub group
                string subGroupName = row.Cells["SubGroupName"].Value.ToString();
                foreach (DataRowView item in subgrpnamecomboBox.Items)
                {
                    if (item["SubGroupName"].ToString() == subGroupName)
                    {
                        subgrpnamecomboBox.SelectedItem = item;
                        break;
                    }
                }

                purchasepricetxtbox1.Text = row.Cells["PurchasePrice"].Value.ToString();
                salepricetxtbox2.Text = row.Cells["SalePrice"].Value.ToString();
                retailpricetxtbox.Text = row.Cells["RetailPrice"].Value.ToString();
                orderstocktxtbox.Text = row.Cells["OrderStockLevel"].Value.ToString();
                barcodetxtbox.Text = row.Cells["BarCode"]?.Value?.ToString() ?? "";

                // Load other fields if they exist in the grid
                if (row.Cells["ProductWeight"].Value != null)
                    packinloseweighttxtbox.Text = row.Cells["ProductWeight"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveProduct()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string productId = ptdcidttxtbox1.Text.Trim();
                    string companyId = cmpnynamecombobox1.SelectedValue?.ToString();
                    string groupId = grpnamecomboBox.SelectedValue?.ToString();
                    string subGroupId = subgrpnamecomboBox.SelectedValue?.ToString();
                    string packingId = purchasepackcombobox.SelectedValue?.ToString();
                    string salePackingId = salepackcombobox.SelectedValue?.ToString();

                    // Check if product exists
                    string checkQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@ProductID", productId);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    string query;
                    if (exists > 0)
                    {
                        // Update existing product
                        query = @"UPDATE Products SET 
                        CompanyID = @CompanyID,
                        GroupId = @GroupId,
                        SubGroupId = @SubGroupId,
                        ProductName = @ProductName,
                        PurchasePrice = @PurchasePrice,
                        SalePrice = @SalePrice,
                        RetailPrice = @RetailPrice,
                        PurPackingId = @PurPackingId,
                        SalPackingId = @SalPackingId,
                        OrderStockLevel = @OrderStockLevel,
                        BarCode = @BarCode,
                        ProductWeight = @ProductWeight
                        WHERE ProductID = @ProductID";
                    }
                    else
                    {
                        // Insert new product
                        query = @"INSERT INTO Products 
                        (ProductID, CompanyID, GroupId, SubGroupId, ProductName, 
                        PurchasePrice, SalePrice, RetailPrice, PurPackingId, 
                        SalPackingId, OrderStockLevel, BarCode, ProductWeight,
                        PurDiscRatio, SaleDiscRatio, PurDiscValue, SaleDiscValue,
                        PurSTRatio, SaleSTRatio, MinStockLevel, IsFinishProduct)
                        VALUES 
                        (@ProductID, @CompanyID, @GroupId, @SubGroupId, @ProductName,
                        @PurchasePrice, @SalePrice, @RetailPrice, @PurPackingId,
                        @SalPackingId, @OrderStockLevel, @BarCode, @ProductWeight,
                        0, 0, 0, 0, 0, 0, 0, 0)";
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@CompanyID", companyId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@GroupId", groupId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SubGroupId", subGroupId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ProductName", prdctnametxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@PurPackingId", packingId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SalPackingId", salePackingId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@BarCode", barcodetxtbox.Text.Trim());

                    // Use SafeParse methods
                    cmd.Parameters.AddWithValue("@PurchasePrice", SafeParseDecimal(purchasepricetxtbox1.Text));
                    cmd.Parameters.AddWithValue("@SalePrice", SafeParseDecimal(salepricetxtbox2.Text));
                    cmd.Parameters.AddWithValue("@RetailPrice", SafeParseDecimal(retailpricetxtbox.Text));
                    cmd.Parameters.AddWithValue("@OrderStockLevel", SafeParseInt(orderstocktxtbox.Text));
                    cmd.Parameters.AddWithValue("@ProductWeight", SafeParseDecimal(packinloseweighttxtbox.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsGrid();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteProduct()
        {
            try
            {
                string productId = dataGridView2.SelectedRows[0].Cells["ProductID"].Value.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(ptdcidttxtbox1.Text))
            {
                MessageBox.Show("Please enter Product ID", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ptdcidttxtbox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(prdctnametxtbox.Text))
            {
                MessageBox.Show("Please enter Product Name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                prdctnametxtbox.Focus();
                return false;
            }

            if (cmpnynamecombobox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Company", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmpnynamecombobox1.Focus();
                return false;
            }

            // Validate numeric fields
            if (!decimal.TryParse(purchasepricetxtbox1.Text, out decimal purchasePrice) || purchasePrice < 0)
            {
                MessageBox.Show("Please enter a valid Purchase Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                purchasepricetxtbox1.Focus();
                return false;
            }

            if (!decimal.TryParse(salepricetxtbox2.Text, out decimal salePrice) || salePrice < 0)
            {
                MessageBox.Show("Please enter a valid Sale Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                salepricetxtbox2.Focus();
                return false;
            }

            if (!decimal.TryParse(retailpricetxtbox.Text, out decimal retailPrice) || retailPrice < 0)
            {
                MessageBox.Show("Please enter a valid Retail Price", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                retailpricetxtbox.Focus();
                return false;
            }

            if (!int.TryParse(orderstocktxtbox.Text, out int orderStock) || orderStock < 0)
            {
                MessageBox.Show("Please enter a valid Order Stock Level", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                orderstocktxtbox.Focus();
                return false;
            }

            if (!decimal.TryParse(packinloseweighttxtbox.Text, out decimal productWeight) || productWeight < 0)
            {
                MessageBox.Show("Please enter a valid Product Weight", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                packinloseweighttxtbox.Focus();
                return false;
            }

            return true;
        }

        private decimal SafeParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            text = text.Trim();
            text = text.Replace(",", ".");

            if (decimal.TryParse(text, out decimal result))
                return result;

            return 0;
        }

        private void ClearForm()
        {
            ptdcidttxtbox1.Clear();
            prdctnametxtbox.Clear();
            cmpnynamecombobox1.SelectedIndex = -1;
            grpnamecomboBox.DataSource = null;
            grpnamecomboBox.Items.Clear();
            grpnamecomboBox.Text = "";
            subgrpnamecomboBox.DataSource = null;
            subgrpnamecomboBox.Items.Clear();
            subgrpnamecomboBox.Text = "";
            purchasepackcombobox.SelectedIndex = -1;
            salepackcombobox.SelectedIndex = -1;
            packinloseweighttxtbox.Clear();
            retailpricetxtbox.Clear();
            purchasepricetxtbox1.Clear();
            salepricetxtbox2.Clear();
            purchasedisctxtbox1.Clear();
            saledisctxtbox2.Clear();
            purchasediscvaltxtbox1.Clear();
            putchasediscvaltxtbox2.Clear();
            sttxtbox.Clear();
            stvaltxtbox.Clear();
            orderstocktxtbox.Clear();
            barcodetxtbox.Clear();
            genbarcodetxtbox.Clear();

            // Also clear search textbox and grid
            srchprdctnametxtbox.Clear();
            dataGridViewavlblepkng.DataSource = null;
        }

        #endregion

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle grid cell clicks if needed
        }

        private void dataGridViewavlblepkng_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle available packings grid clicks if needed
        }

        // Other event handlers
        private void ptdcidttxtbox1_TextChanged(object sender, EventArgs e) { }
        private void prdctnametxtbox_TextChanged(object sender, EventArgs e) { }
        private void notxtbox_TextChanged(object sender, EventArgs e) { }
        private void subgrpnamecomboBox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void slctcmpnycombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void slctprdctgrpcomboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void purchasepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void salepackcombobox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void packinloseweighttxtbox_TextChanged(object sender, EventArgs e) { }
        private void retailpricetxtbox_TextChanged(object sender, EventArgs e) { }
        private void purchasepricetxtbox1_TextChanged(object sender, EventArgs e) { }
        private void salepricetxtbox2_TextChanged(object sender, EventArgs e) { }
        private void purchasedisctxtbox1_TextChanged(object sender, EventArgs e) { }
        private void saledisctxtbox2_TextChanged(object sender, EventArgs e) { }
        private void purchasediscvaltxtbox1_TextChanged(object sender, EventArgs e) { }
        private void putchasediscvaltxtbox2_TextChanged(object sender, EventArgs e) { }
        private void sttxtbox_TextChanged(object sender, EventArgs e) { }
        private void stvaltxtbox_TextChanged(object sender, EventArgs e) { }
        private void orderstocktxtbox_TextChanged(object sender, EventArgs e) { }
        private void barcodetxtbox_TextChanged(object sender, EventArgs e) { }
        private void genbarcodetxtbox_TextChanged(object sender, EventArgs e) { }
    }
}