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
//using System.Configuration;

//namespace FO_2022
//{
//    public partial class Change_Product_Prices : Form
//    {
//        private SqlConnection connection;
//        private DataTable dataTable;
//        private bool isLoading = false;
//        private DataTable originalDataTable; // To track changes

//        public Change_Product_Prices()
//        {
//            InitializeComponent();
//            InitializeDatabaseConnection();
//        }

//        private void InitializeDatabaseConnection()
//        {
//            try
//            {
//                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//                connection = new SqlConnection(connStr);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Connection Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void Change_Product_Prices_Load(object sender, EventArgs e)
//        {
//            LoadCompanies();
//            LoadDataGrid();
//            SetupDataGridView();
//        }

//        private void LoadCompanies()
//        {
//            try
//            {
//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
//                SqlCommand cmd = new SqlCommand(query, connection);
//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                DataTable dt = new DataTable();
//                adapter.Fill(dt);

//                cmpnynamecombobox1.DataSource = dt;
//                cmpnynamecombobox1.DisplayMember = "CompanyName";
//                cmpnynamecombobox1.ValueMember = "CompanyID";
//                cmpnynamecombobox1.SelectedIndex = -1;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading companies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void LoadDataGrid(string companyId = null)
//        {
//            try
//            {
//                isLoading = true;

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                string query = @"
//                    SELECT 
//                        p.ProductID,
//                        p.ProductName,
//                        pp.PackingName AS PurchasePacking,
//                        p.PurchasePrice,
//                        p.PurDiscRatio AS PurchaseDiscPercent,
//                        p.PurDiscValue AS PurchaseDiscValue,
//                        p.PurSTRatio AS PurchaseSTPercent,
//                        sp.PackingName AS SalePacking,
//                        p.SalePrice,
//                        p.SaleDiscRatio AS SaleDiscPercent,
//                        p.SaleDiscValue AS SaleDiscValue,
//                        p.SaleSTRatio AS SaleSTPercent,
//                        p.PurPackingId,
//                        p.SalPackingId
//                    FROM 
//                        [dbo].[Products] p
//                    LEFT JOIN 
//                        [dbo].[Packings] pp ON p.PurPackingId = pp.PackingId
//                    LEFT JOIN 
//                        [dbo].[Packings] sp ON p.SalPackingId = sp.PackingId
//                    WHERE 
//                        (@CompanyID IS NULL OR p.CompanyID = @CompanyID)
//                    ORDER BY 
//                        p.ProductID";

//                SqlCommand cmd = new SqlCommand(query, connection);

//                if (!string.IsNullOrEmpty(companyId) && cmpnynamecombobox1.SelectedIndex != -1)
//                    cmd.Parameters.AddWithValue("@CompanyID", companyId);
//                else
//                    cmd.Parameters.AddWithValue("@CompanyID", DBNull.Value);

//                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
//                dataTable = new DataTable();
//                adapter.Fill(dataTable);

//                // Create a copy for tracking changes
//                originalDataTable = dataTable.Copy();

//                dataGridView1.DataSource = dataTable;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                isLoading = false;
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void SetupDataGridView()
//        {
//            dataGridView1.AutoGenerateColumns = false;
//            dataGridView1.Columns.Clear();

//            // Pro ID (Column 1)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "ProductID",
//                HeaderText = "Pro ID",
//                DataPropertyName = "ProductID",
//                ReadOnly = true,
//                Width = 70,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Product Name (Column 2)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "ProductName",
//                HeaderText = "Product Name",
//                DataPropertyName = "ProductName",
//                ReadOnly = true,
//                Width = 150,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // PURCHASE SECTION HEADER (Column 3-7)
//            // Purchase Packing (Column 3)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurchasePacking",
//                HeaderText = "Packing",
//                DataPropertyName = "PurchasePacking",
//                ReadOnly = true,
//                Width = 70,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Purchase Price (Column 4)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurchasePrice",
//                HeaderText = "Price",
//                DataPropertyName = "PurchasePrice",
//                Width = 70,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Purchase Disc % (Column 5)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurchaseDiscPercent",
//                HeaderText = "Disc%",
//                DataPropertyName = "PurchaseDiscPercent",
//                Width = 60,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Purchase Disc Value (Column 6)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurchaseDiscValue",
//                HeaderText = "Disc Value",
//                DataPropertyName = "PurchaseDiscValue",
//                Width = 80,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Purchase S.T % (Column 7)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurchaseSTPercent",
//                HeaderText = "S.T%",
//                DataPropertyName = "PurchaseSTPercent",
//                Width = 60,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // SALE SECTION HEADER (Column 8-12)
//            // Sale Packing (Column 8)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SalePacking",
//                HeaderText = "Packing",
//                DataPropertyName = "SalePacking",
//                ReadOnly = true,
//                Width = 70,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Sale Price (Column 9)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SalePrice",
//                HeaderText = "Price",
//                DataPropertyName = "SalePrice",
//                Width = 70,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Sale Disc % (Column 10)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SaleDiscPercent",
//                HeaderText = "Disc%",
//                DataPropertyName = "SaleDiscPercent",
//                Width = 60,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Sale Disc Value (Column 11)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SaleDiscValue",
//                HeaderText = "Disc Value",
//                DataPropertyName = "SaleDiscValue",
//                Width = 80,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Sale S.T % (Column 12)
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SaleSTPercent",
//                HeaderText = "S.T%",
//                DataPropertyName = "SaleSTPercent",
//                Width = 60,
//                SortMode = DataGridViewColumnSortMode.NotSortable
//            });

//            // Hidden columns for IDs
//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "PurPackingId",
//                DataPropertyName = "PurPackingId",
//                Visible = false
//            });

//            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
//            {
//                Name = "SalPackingId",
//                DataPropertyName = "SalPackingId",
//                Visible = false
//            });

//            // Enable cell editing
//            dataGridView1.ReadOnly = false;
//            dataGridView1.AllowUserToAddRows = false;
//            dataGridView1.AllowUserToDeleteRows = false;
//            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

//            // Disable sorting for entire grid
//            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
//        }
//        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
//        {
//            if (isLoading) return;

//            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
//            string stringValue = e.FormattedValue.ToString();

//            // Validate numeric fields
//            if (columnName.Contains("Price") || columnName.Contains("Disc") || columnName.Contains("Percent"))
//            {
//                if (!decimal.TryParse(stringValue, out decimal result))
//                {
//                    dataGridView1.Rows[e.RowIndex].ErrorText = "Please enter a valid number";
//                    e.Cancel = true;
//                }
//                else if (result < 0)
//                {
//                    dataGridView1.Rows[e.RowIndex].ErrorText = "Value cannot be negative";
//                    e.Cancel = true;
//                }
//                else
//                {
//                    dataGridView1.Rows[e.RowIndex].ErrorText = "";
//                }
//            }
//        }

//        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
//        {
//            dataGridView1.Rows[e.RowIndex].ErrorText = "";
//        }

//        private bool HasRowChanged(DataRow currentRow, DataRow originalRow)
//        {
//            if (originalRow == null) return true;

//            string[] editableColumns = { "PurchasePrice", "PurchaseDiscPercent", "PurchaseDiscValue",
//                                         "PurchaseSTPercent", "SalePrice", "SaleDiscPercent",
//                                         "SaleDiscValue", "SaleSTPercent" };

//            foreach (string col in editableColumns)
//            {
//                object currentValue = currentRow[col];
//                object originalValue = originalRow[col];

//                if (!currentValue.Equals(originalValue))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (dataTable == null || dataTable.Rows.Count == 0)
//            {
//                MessageBox.Show("No data to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            try
//            {
//                int successCount = 0;
//                int errorCount = 0;

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                for (int i = 0; i < dataTable.Rows.Count; i++)
//                {
//                    try
//                    {
//                        DataRow currentRow = dataTable.Rows[i];
//                        DataRow originalRow = originalDataTable.Rows[i];

//                        // Check if any editable field has changed
//                        if (HasRowChanged(currentRow, originalRow))
//                        {
//                            string productId = currentRow["ProductID"].ToString();

//                            // First check if product exists
//                            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID", connection);
//                            checkCmd.Parameters.AddWithValue("@ProductID", productId);
//                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

//                            if (exists > 0)
//                            {
//                                // Update product
//                                SqlCommand updateCmd = new SqlCommand(@"
//                                    UPDATE Products 
//                                    SET PurchasePrice = @PurchasePrice,
//                                        PurDiscRatio = @PurDiscRatio,
//                                        PurDiscValue = @PurDiscValue,
//                                        PurSTRatio = @PurSTRatio,
//                                        SalePrice = @SalePrice,
//                                        SaleDiscRatio = @SaleDiscRatio,
//                                        SaleDiscValue = @SaleDiscValue,
//                                        SaleSTRatio = @SaleSTRatio
//                                    WHERE ProductID = @ProductID", connection);

//                                updateCmd.Parameters.AddWithValue("@ProductID", productId);
//                                updateCmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(currentRow["PurchasePrice"]));
//                                updateCmd.Parameters.AddWithValue("@PurDiscRatio", Convert.ToDecimal(currentRow["PurchaseDiscPercent"]));
//                                updateCmd.Parameters.AddWithValue("@PurDiscValue", Convert.ToDecimal(currentRow["PurchaseDiscValue"]));
//                                updateCmd.Parameters.AddWithValue("@PurSTRatio", Convert.ToDecimal(currentRow["PurchaseSTPercent"]));
//                                updateCmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(currentRow["SalePrice"]));
//                                updateCmd.Parameters.AddWithValue("@SaleDiscRatio", Convert.ToDecimal(currentRow["SaleDiscPercent"]));
//                                updateCmd.Parameters.AddWithValue("@SaleDiscValue", Convert.ToDecimal(currentRow["SaleDiscValue"]));
//                                updateCmd.Parameters.AddWithValue("@SaleSTRatio", Convert.ToDecimal(currentRow["SaleSTPercent"]));

//                                updateCmd.ExecuteNonQuery();
//                                successCount++;

//                                // Update original row to match current
//                                originalRow.ItemArray = currentRow.ItemArray;
//                            }
//                            else
//                            {
//                                errorCount++;
//                                Console.WriteLine($"Product {productId} not found in database");
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        errorCount++;
//                        Console.WriteLine($"Error updating row {i}: {ex.Message}");
//                    }
//                }

//                MessageBox.Show($"Save completed.\nSuccessfully updated: {successCount} products\nErrors: {errorCount}",
//                    "Save Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

//                // Refresh data after save
//                LoadDataGrid(cmpnynamecombobox1.SelectedValue?.ToString());
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void refreshbtn_Click(object sender, EventArgs e)
//        {
//            LoadDataGrid(cmpnynamecombobox1.SelectedValue?.ToString());
//            MessageBox.Show("Data refreshed successfully.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            cmpnynamecombobox1.SelectedIndex = -1;
//            LoadDataGrid(); // Load all products
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            ReturnToMainForm();
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            ReturnToMainForm();
//        }

//        private void ReturnToMainForm()
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Optional: Add cell click logic if needed
//        }

//        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            // Optional: Add content click logic if needed
//        }

//        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            // If you have another combo box, add logic here
//            if (cmpnynamecombobox1.SelectedIndex != -1)
//            {
//                string companyId = cmpnynamecombobox1.SelectedValue.ToString();
//                LoadDataGrid(companyId);
//            }
//            else
//            {
//                LoadDataGrid();
//            }
//        }

//        private void label3_Click(object sender, EventArgs e)
//        {
//            // Label click event
//        }

//        private void label4_Click(object sender, EventArgs e)
//        {
//            // Label click event
//        }

//        private void cmpnynamecombobox2_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }
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
using System.Configuration;

namespace FO_2022
{
    public partial class Change_Product_Prices : Form
    {
        private SqlConnection connection;
        private DataTable dataTable;
        private bool isLoading = false;
        private DataTable originalDataTable; // To track changes

        public Change_Product_Prices()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Change_Product_Prices_Load(object sender, EventArgs e)
        {
            LoadCompanies();
            LoadGroups();
            LoadDataGrid();
            SetupDataGridView();
        }

        private void LoadCompanies()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                string query = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Add "----All Companies----" as first option
                DataRow newRow = dt.NewRow();
                newRow["CompanyID"] = "0";
                newRow["CompanyName"] = "----All Companies----";
                dt.Rows.InsertAt(newRow, 0);

                cmpnynamecombobox1.DataSource = dt;
                cmpnynamecombobox1.DisplayMember = "CompanyName";
                cmpnynamecombobox1.ValueMember = "CompanyID";
                cmpnynamecombobox1.SelectedIndex = 0; // Select "All Companies" by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading companies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void LoadGroups()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                // First get all unique group IDs and names
                string query = @"
                    SELECT DISTINCT 
                        g.GroupId, 
                        g.GroupName,
                        c.CompanyName
                    FROM 
                        [dbo].[ProductGroups] g
                    LEFT JOIN 
                        [dbo].[Companies] c ON g.CompanyId = c.CompanyID
                    ORDER BY 
                        g.GroupName";

                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Add "----All Groups----" as first option
                DataRow newRow = dt.NewRow();
                newRow["GroupId"] = 0;
                newRow["GroupName"] = "----All Groups----";
                newRow["CompanyName"] = "";
                dt.Rows.InsertAt(newRow, 0);

                cmpnynamecombobox2.DataSource = dt;
                cmpnynamecombobox2.DisplayMember = "GroupName";
                cmpnynamecombobox2.ValueMember = "GroupId";
                cmpnynamecombobox2.SelectedIndex = 0; // Select "All Groups" by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading groups: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private string GetSelectedCompanyId()
        {
            try
            {
                if (cmpnynamecombobox1.SelectedIndex == -1 || cmpnynamecombobox1.SelectedIndex == 0)
                    return "0";

                if (cmpnynamecombobox1.SelectedValue != null)
                {
                    return cmpnynamecombobox1.SelectedValue.ToString();
                }
                return "0";
            }
            catch
            {
                return "0";
            }
        }

        private int GetSelectedGroupId()
        {
            try
            {
                if (cmpnynamecombobox2.SelectedIndex == -1 || cmpnynamecombobox2.SelectedIndex == 0)
                    return 0;

                if (cmpnynamecombobox2.SelectedValue != null)
                {
                    if (int.TryParse(cmpnynamecombobox2.SelectedValue.ToString(), out int groupId))
                    {
                        return groupId;
                    }
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private void LoadDataGrid(string companyId = null, int groupId = 0)
        {
            try
            {
                isLoading = true;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                string query = @"
                    SELECT 
                        p.ProductID,
                        p.ProductName,
                        pp.PackingName AS PurchasePacking,
                        p.PurchasePrice,
                        p.PurDiscRatio AS PurchaseDiscPercent,
                        p.PurDiscValue AS PurchaseDiscValue,
                        p.PurSTRatio AS PurchaseSTPercent,
                        sp.PackingName AS SalePacking,
                        p.SalePrice,
                        p.SaleDiscRatio AS SaleDiscPercent,
                        p.SaleDiscValue AS SaleDiscValue,
                        p.SaleSTRatio AS SaleSTPercent,
                        p.PurPackingId,
                        p.SalPackingId
                    FROM 
                        [dbo].[Products] p
                    LEFT JOIN 
                        [dbo].[Packings] pp ON p.PurPackingId = pp.PackingId
                    LEFT JOIN 
                        [dbo].[Packings] sp ON p.SalPackingId = sp.PackingId
                    WHERE 
                        1=1"; // Always true for easy WHERE clause building

                List<SqlParameter> parameters = new List<SqlParameter>();

                // Add company filter if not "All Companies"
                if (!string.IsNullOrEmpty(companyId) && companyId != "0")
                {
                    query += " AND p.CompanyID = @CompanyID";
                    parameters.Add(new SqlParameter("@CompanyID", companyId));
                }

                // Add group filter if not "All Groups"
                if (groupId != 0)
                {
                    query += " AND p.GroupId = @GroupId";
                    parameters.Add(new SqlParameter("@GroupId", groupId));
                }

                query += " ORDER BY p.ProductID";

                SqlCommand cmd = new SqlCommand(query, connection);
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Create a copy for tracking changes
                if (originalDataTable != null)
                    originalDataTable.Dispose();
                originalDataTable = dataTable.Copy();

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            // Pro ID (Column 1)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "ProductID",
                HeaderText = "Pro ID",
                DataPropertyName = "ProductID",
                ReadOnly = true,
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Product Name (Column 2)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "ProductName",
                HeaderText = "Product Name",
                DataPropertyName = "ProductName",
                ReadOnly = true,
                Width = 150,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // PURCHASE SECTION HEADER (Column 3-7)
            // Purchase Packing (Column 3)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurchasePacking",
                HeaderText = "Packing",
                DataPropertyName = "PurchasePacking",
                ReadOnly = true,
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Purchase Price (Column 4)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurchasePrice",
                HeaderText = "Price",
                DataPropertyName = "PurchasePrice",
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Purchase Disc % (Column 5)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurchaseDiscPercent",
                HeaderText = "Disc%",
                DataPropertyName = "PurchaseDiscPercent",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Purchase Disc Value (Column 6)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurchaseDiscValue",
                HeaderText = "Disc Value",
                DataPropertyName = "PurchaseDiscValue",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Purchase S.T % (Column 7)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurchaseSTPercent",
                HeaderText = "S.T%",
                DataPropertyName = "PurchaseSTPercent",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // SALE SECTION HEADER (Column 8-12)
            // Sale Packing (Column 8)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SalePacking",
                HeaderText = "Packing",
                DataPropertyName = "SalePacking",
                ReadOnly = true,
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Sale Price (Column 9)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SalePrice",
                HeaderText = "Price",
                DataPropertyName = "SalePrice",
                Width = 70,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Sale Disc % (Column 10)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SaleDiscPercent",
                HeaderText = "Disc%",
                DataPropertyName = "SaleDiscPercent",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Sale Disc Value (Column 11)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SaleDiscValue",
                HeaderText = "Disc Value",
                DataPropertyName = "SaleDiscValue",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Sale S.T % (Column 12)
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SaleSTPercent",
                HeaderText = "S.T%",
                DataPropertyName = "SaleSTPercent",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // Hidden columns for IDs
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "PurPackingId",
                DataPropertyName = "PurPackingId",
                Visible = false
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                Name = "SalPackingId",
                DataPropertyName = "SalPackingId",
                Visible = false
            });

            // Enable cell editing
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

            // Disable sorting for entire grid
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        // First combobox - Company filter
        private void cmpnynamecombobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmpnynamecombobox1.SelectedIndex != -1)
            {
                string companyId = GetSelectedCompanyId();
                int groupId = GetSelectedGroupId();

                LoadDataGrid(companyId, groupId);
            }
        }

        // Second combobox - Group filter
        private void cmpnynamecombobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmpnynamecombobox2.SelectedIndex != -1)
            {
                string companyId = GetSelectedCompanyId();
                int groupId = GetSelectedGroupId();

                LoadDataGrid(companyId, groupId);
            }
        }

        // Third combobox (if exists)
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If you have another combobox, add logic here
            // Currently not used
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (isLoading) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            string stringValue = e.FormattedValue.ToString();

            // Validate numeric fields
            if (columnName.Contains("Price") || columnName.Contains("Disc") || columnName.Contains("Percent"))
            {
                if (!decimal.TryParse(stringValue, out decimal result))
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "Please enter a valid number";
                    e.Cancel = true;
                }
                else if (result < 0)
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "Value cannot be negative";
                    e.Cancel = true;
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].ErrorText = "";
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].ErrorText = "";
        }

        private bool HasRowChanged(DataRow currentRow, DataRow originalRow)
        {
            if (originalRow == null) return true;

            string[] editableColumns = { "PurchasePrice", "PurchaseDiscPercent", "PurchaseDiscValue",
                                         "PurchaseSTPercent", "SalePrice", "SaleDiscPercent",
                                         "SaleDiscValue", "SaleSTPercent" };

            foreach (string col in editableColumns)
            {
                object currentValue = currentRow[col];
                object originalValue = originalRow[col];

                if (currentValue == null && originalValue == null)
                    continue;

                if (currentValue == null || originalValue == null)
                    return true;

                if (!currentValue.Equals(originalValue))
                {
                    return true;
                }
            }
            return false;
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No data to save.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int successCount = 0;
                int errorCount = 0;

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    try
                    {
                        DataRow currentRow = dataTable.Rows[i];
                        DataRow originalRow = originalDataTable.Rows[i];

                        // Check if any editable field has changed
                        if (HasRowChanged(currentRow, originalRow))
                        {
                            string productId = currentRow["ProductID"].ToString();

                            // First check if product exists
                            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE ProductID = @ProductID", connection);
                            checkCmd.Parameters.AddWithValue("@ProductID", productId);
                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (exists > 0)
                            {
                                // Update product
                                SqlCommand updateCmd = new SqlCommand(@"
                                    UPDATE Products 
                                    SET PurchasePrice = @PurchasePrice,
                                        PurDiscRatio = @PurDiscRatio,
                                        PurDiscValue = @PurDiscValue,
                                        PurSTRatio = @PurSTRatio,
                                        SalePrice = @SalePrice,
                                        SaleDiscRatio = @SaleDiscRatio,
                                        SaleDiscValue = @SaleDiscValue,
                                        SaleSTRatio = @SaleSTRatio
                                    WHERE ProductID = @ProductID", connection);

                                updateCmd.Parameters.AddWithValue("@ProductID", productId);
                                updateCmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(currentRow["PurchasePrice"]));
                                updateCmd.Parameters.AddWithValue("@PurDiscRatio", Convert.ToDecimal(currentRow["PurchaseDiscPercent"]));
                                updateCmd.Parameters.AddWithValue("@PurDiscValue", Convert.ToDecimal(currentRow["PurchaseDiscValue"]));
                                updateCmd.Parameters.AddWithValue("@PurSTRatio", Convert.ToDecimal(currentRow["PurchaseSTPercent"]));
                                updateCmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(currentRow["SalePrice"]));
                                updateCmd.Parameters.AddWithValue("@SaleDiscRatio", Convert.ToDecimal(currentRow["SaleDiscPercent"]));
                                updateCmd.Parameters.AddWithValue("@SaleDiscValue", Convert.ToDecimal(currentRow["SaleDiscValue"]));
                                updateCmd.Parameters.AddWithValue("@SaleSTRatio", Convert.ToDecimal(currentRow["SaleSTPercent"]));

                                updateCmd.ExecuteNonQuery();
                                successCount++;

                                // Update original row to match current
                                originalRow.ItemArray = currentRow.ItemArray;
                            }
                            else
                            {
                                errorCount++;
                                Console.WriteLine($"Product {productId} not found in database");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Console.WriteLine($"Error updating row {i}: {ex.Message}");
                    }
                }

                MessageBox.Show($"Save completed.\nSuccessfully updated: {successCount} ",
                    "Save Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh data after save
                string companyId = GetSelectedCompanyId();
                int groupId = GetSelectedGroupId();
                LoadDataGrid(companyId, groupId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            string companyId = GetSelectedCompanyId();
            int groupId = GetSelectedGroupId();

            LoadDataGrid(companyId, groupId);
            MessageBox.Show("Data refreshed successfully.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            // Reset both comboboxes to "All"
            cmpnynamecombobox1.SelectedIndex = 0; // "All Companies"
            cmpnynamecombobox2.SelectedIndex = 0; // "All Groups"

            // Load all products
            LoadDataGrid();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //ReturnToMainForm();
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();

        }
        private void ReturnToMainForm()
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: Add cell click logic if needed
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: Add content click logic if needed
        }
        private void label3_Click(object sender, EventArgs e)
        {
            // Label click event
        }
        private void label4_Click(object sender, EventArgs e)
        {
            // Label click event
        }
    }
}