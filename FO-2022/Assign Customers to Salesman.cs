//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Assign_Customers_to_Salesman : Form
//    {
//        public Assign_Customers_to_Salesman()
//        {
//            InitializeComponent();
//        }

//        private void pictureBox2_Click(object sender, EventArgs e)
//        {
//            this.Close();   
//        }

//        private void closebtn_Click(object sender, EventArgs e)
//        {
//            Mainform1 mf = new Mainform1();
//            mf.Show();
//            this.Hide();
//        }

//        private void actualsectorcombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void actualtowncombobox_SelectedIndexChanged(object sender, EventArgs e)
//        {

//        }

//        private void multiplesalesmancheckbox_CheckedChanged(object sender, EventArgs e)
//        {

//        }

//        private void refreshbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void salesmanidtxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void salesmanidsearch_Click(object sender, EventArgs e)
//        {

//        }

//        private void salesmannametxtbox_TextChanged(object sender, EventArgs e)
//        {

//        }

//        private void UnAssignedCustomersdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void AssignedCustomerdataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void nextbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void backbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void dblnextbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void bdlbackbtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {

//        }

//        private void clearbtn_Click(object sender, EventArgs e)
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
    public partial class Assign_Customers_to_Salesman : Form
    {
        private SqlConnection connection;
        private string salesmanId = "";
        private DataTable unassignedCustomers;
        private DataTable assignedCustomers;
        private bool multipleSalesmanMode = false;

        public Assign_Customers_to_Salesman()
        {
            InitializeComponent();
            InitializeConnection();
            LoadTownsAndSectors();

            // Add Enter key event handler
            salesmanidtxtbox.KeyDown += Salesmanidtxtbox_KeyDown;
        }

        // Handle Enter key press in salesmanidtxtbox
        private void Salesmanidtxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Perform the same action as search button
                salesmanidsearch_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true; // Prevent the beep sound
            }
        }

        private void InitializeConnection()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTownsAndSectors()
        {
            try
            {
                // Load towns
                string townQuery = "SELECT TownID, TownName FROM Towns ORDER BY TownName";
                SqlCommand townCmd = new SqlCommand(townQuery, connection);
                SqlDataReader townReader = townCmd.ExecuteReader();

                actualtowncombobox.Items.Clear();
                actualtowncombobox.Items.Add("All Towns");

                while (townReader.Read())
                {
                    actualtowncombobox.Items.Add(new ComboboxItem
                    {
                        Text = townReader["TownName"].ToString(),
                        Value = townReader["TownID"].ToString()
                    });
                }
                townReader.Close();
                actualtowncombobox.SelectedIndex = 0;

                // Load all sectors for "All Towns" option
                LoadSectorsByTown("ALL");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading towns and sectors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSectorsByTown(string townId)
        {
            try
            {
                actualsectorcombobox.Items.Clear();
                actualsectorcombobox.Items.Add("All Sectors");

                string sectorQuery;
                SqlCommand sectorCmd;

                if (townId == "ALL")
                {
                    // Load all sectors
                    sectorQuery = "SELECT SectorID, SectorName FROM Sectors ORDER BY SectorName";
                    sectorCmd = new SqlCommand(sectorQuery, connection);
                }
                else
                {
                    // Load sectors for specific town
                    sectorQuery = "SELECT SectorID, SectorName FROM Sectors WHERE TownID = @TownID ORDER BY SectorName";
                    sectorCmd = new SqlCommand(sectorQuery, connection);
                    sectorCmd.Parameters.AddWithValue("@TownID", Convert.ToInt32(townId));
                }

                SqlDataReader sectorReader = sectorCmd.ExecuteReader();

                while (sectorReader.Read())
                {
                    actualsectorcombobox.Items.Add(new ComboboxItem
                    {
                        Text = sectorReader["SectorName"].ToString(),
                        Value = sectorReader["SectorID"].ToString()
                    });
                }
                sectorReader.Close();

                actualsectorcombobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sectors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void actualtowncombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentSectorId = "";
            // Save current sector selection if exists
            if (actualsectorcombobox.SelectedIndex > 0)
            {
                ComboboxItem currentSector = (ComboboxItem)actualsectorcombobox.SelectedItem;
                currentSectorId = currentSector.Value;
            }

            if (actualtowncombobox.SelectedIndex == 0) // "All Towns"
            {
                LoadSectorsByTown("ALL");
            }
            else if (actualtowncombobox.SelectedIndex > 0)
            {
                ComboboxItem selectedItem = (ComboboxItem)actualtowncombobox.SelectedItem;
                LoadSectorsByTown(selectedItem.Value);
            }

            // Restore previous sector selection if still valid
            if (!string.IsNullOrEmpty(currentSectorId))
            {
                for (int i = 0; i < actualsectorcombobox.Items.Count; i++)
                {
                    if (actualsectorcombobox.Items[i] is ComboboxItem item && item.Value == currentSectorId)
                    {
                        actualsectorcombobox.SelectedIndex = i;
                        break;
                    }
                }
            }

            LoadCustomers();
        }

        private void actualsectorcombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void multiplesalesmancheckbox_CheckedChanged(object sender, EventArgs e)
        {
            multipleSalesmanMode = multiplesalesmancheckbox.Checked;
            LoadCustomers();
        }

        private void refreshbtn_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void salesmanidsearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(salesmanidtxtbox.Text))
            {
                MessageBox.Show("Please enter Salesman ID", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT SalesmanName FROM Salesmen WHERE SalesmanID = @SalesmanID";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SalesmanID", salesmanidtxtbox.Text.Trim());

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    salesmanId = salesmanidtxtbox.Text.Trim();
                    salesmannametxtbox.Text = result.ToString();

                    // Load assigned customers specifically for this salesman
                    LoadAssignedCustomersForSalesman();

                    // Also refresh the unassigned customers
                    LoadCustomers();
                }
                else
                {
                    MessageBox.Show("Salesman not found!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    salesmannametxtbox.Clear();
                    salesmanId = "";
                    AssignedCustomerdataGridView.DataSource = null;
                    AssignedCustomerdataGridView.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching salesman: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // New method to load only assigned customers for the salesman
        //private void LoadAssignedCustomersForSalesman()
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(salesmanId))
        //        {
        //            AssignedCustomerdataGridView.DataSource = null;
        //            AssignedCustomerdataGridView.Rows.Clear();
        //            return;
        //        }

        //        string assignedQuery = @"
        //            SELECT 
        //                P.PartyName as CustomerName, 
        //                T.TownName
        //            FROM Parties P
        //            INNER JOIN SalesmenParties SP ON P.PartyID = SP.AccountNo
        //            INNER JOIN Sectors S ON P.SectorID = S.SectorID
        //            INNER JOIN Towns T ON S.TownID = T.TownID
        //            WHERE SP.SalesmanID = @SalesmanID
        //            AND P.PartyType = 'C'
        //            ORDER BY P.PartyName";

        //        SqlCommand assignedCmd = new SqlCommand(assignedQuery, connection);
        //        assignedCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

        //        SqlDataAdapter assignedAdapter = new SqlDataAdapter(assignedCmd);
        //        DataTable assignedCustomersTable = new DataTable();
        //        assignedAdapter.Fill(assignedCustomersTable);

        //        // Bind to grid with only 2 columns
        //        AssignedCustomerdataGridView.DataSource = assignedCustomersTable;

        //        // Format the grid with only 2 columns
        //        FormatAssignedGrid();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error loading assigned customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void LoadAssignedCustomersForSalesman()
        {
            try
            {
                if (string.IsNullOrEmpty(salesmanId))
                {
                    AssignedCustomerdataGridView.DataSource = null;
                    AssignedCustomerdataGridView.Rows.Clear();
                    return;
                }

                // Fixed query - Use correct table and column names
                string assignedQuery = @"
            SELECT 
                P.PartyName as CustomerName, 
                T.TownName
            FROM Parties P
            INNER JOIN SalesmenParties SP ON P.PartyID = SP.AccountNo
            LEFT JOIN Sectors S ON P.SectorID = S.SectorID
            LEFT JOIN Towns T ON S.TownID = T.TownID
            WHERE SP.SalesmanID = @SalesmanID
            AND P.PartyType = 'C'
            ORDER BY P.PartyName";

                SqlCommand assignedCmd = new SqlCommand(assignedQuery, connection);
                assignedCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);

                SqlDataAdapter assignedAdapter = new SqlDataAdapter(assignedCmd);
                DataTable assignedCustomersTable = new DataTable();
                assignedAdapter.Fill(assignedCustomersTable);

                // Check if data was retrieved
                if (assignedCustomersTable.Rows.Count == 0)
                {
                    MessageBox.Show("No customers assigned to this salesman.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Bind to grid
                AssignedCustomerdataGridView.DataSource = assignedCustomersTable;
                FormatAssignedGrid();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}\nTable SalesmenParties exists? {DoesTableExist("SalesmenParties")}",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading assigned customers: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to check if table exists
        private bool DoesTableExist(string tableName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TableName", tableName);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch
            {
                return false;
            }
        }
        // New method to format assigned grid with only 2 columns
        private void FormatAssignedGrid()
        {
            if (AssignedCustomerdataGridView.DataSource != null)
            {
                AssignedCustomerdataGridView.AutoGenerateColumns = false;
                AssignedCustomerdataGridView.Columns.Clear();

                // Add only 2 columns
                DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                col1.DataPropertyName = "CustomerName";
                col1.HeaderText = "Customer Name";
                col1.Width = 300;
                AssignedCustomerdataGridView.Columns.Add(col1);

                DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
                col2.DataPropertyName = "TownName";
                col2.HeaderText = "Town";
                col2.Width = 200;
                AssignedCustomerdataGridView.Columns.Add(col2);

                AssignedCustomerdataGridView.AllowUserToAddRows = false;
                AssignedCustomerdataGridView.AllowUserToDeleteRows = false;
                AssignedCustomerdataGridView.ReadOnly = true;
                AssignedCustomerdataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                AssignedCustomerdataGridView.MultiSelect = true;
            }
        }

        private void LoadCustomers()
        {
            try
            {
                // If no salesman selected and not in multiple salesman mode, show nothing
                if (string.IsNullOrEmpty(salesmanId) && !multipleSalesmanMode)
                {
                    ClearGrids();
                    return;
                }

                // Build filter conditions
                List<string> whereConditions = new List<string>();
                List<SqlParameter> parameters = new List<SqlParameter>();

                // Always filter by customer type
                whereConditions.Add("P.PartyType = 'C'");

                // Town filter
                if (actualtowncombobox.SelectedIndex > 0)
                {
                    ComboboxItem townItem = (ComboboxItem)actualtowncombobox.SelectedItem;
                    whereConditions.Add("P.SectorID IN (SELECT SectorID FROM Sectors WHERE TownID = @TownID)");
                    parameters.Add(new SqlParameter("@TownID", Convert.ToInt32(townItem.Value)));
                }

                // Sector filter
                if (actualsectorcombobox.SelectedIndex > 0)
                {
                    ComboboxItem sectorItem = (ComboboxItem)actualsectorcombobox.SelectedItem;
                    whereConditions.Add("P.SectorID = @SectorID");
                    parameters.Add(new SqlParameter("@SectorID", Convert.ToInt32(sectorItem.Value)));
                }

                string whereClause = whereConditions.Count > 0 ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                // Load unassigned customers
                string unassignedQuery = $@"
                    SELECT 
                        P.PartyID, 
                        P.PartyName as CustomerName, 
                        ISNULL(S.SectorName, '') as SectorName, 
                        ISNULL(T.TownName, '') as TownName
                    FROM Parties P
                    LEFT JOIN Sectors S ON P.SectorID = S.SectorID
                    LEFT JOIN Towns T ON S.TownID = T.TownID
                    {whereClause}";

                if (!multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId))
                {
                    // Add condition for unassigned customers
                    unassignedQuery += whereClause.Contains("WHERE") ? " AND " : " WHERE ";
                    unassignedQuery += "P.PartyID NOT IN (SELECT AccountNo FROM SalesmenParties WHERE SalesmanID = @SalesmanID)";
                    parameters.Add(new SqlParameter("@SalesmanID", salesmanId));
                }

                unassignedQuery += " ORDER BY P.PartyName";

                SqlCommand unassignedCmd = new SqlCommand(unassignedQuery, connection);
                foreach (var param in parameters)
                {
                    unassignedCmd.Parameters.Add(param);
                }

                SqlDataAdapter unassignedAdapter = new SqlDataAdapter(unassignedCmd);
                unassignedCustomers = new DataTable();
                unassignedCustomers.TableName = "UnassignedCustomers";
                unassignedAdapter.Fill(unassignedCustomers);

                UnAssignedCustomersdataGridView.DataSource = unassignedCustomers;
                FormatGrid(UnAssignedCustomersdataGridView);

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Modify the existing FormatGrid method (for unassigned customers)
        private void FormatGrid(DataGridView grid)
        {
            if (grid.DataSource != null && grid == UnAssignedCustomersdataGridView)
            {
                grid.AutoGenerateColumns = false;
                grid.Columns.Clear();

                // Add all 4 columns for unassigned grid
                DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
                col1.DataPropertyName = "PartyID";
                col1.HeaderText = "Customer ID";
                col1.Width = 120;
                grid.Columns.Add(col1);

                DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
                col2.DataPropertyName = "CustomerName";
                col2.HeaderText = "Customer Name";
                col2.Width = 200;
                grid.Columns.Add(col2);

                DataGridViewTextBoxColumn col3 = new DataGridViewTextBoxColumn();
                col3.DataPropertyName = "SectorName";
                col3.HeaderText = "Sector";
                col3.Width = 150;
                grid.Columns.Add(col3);

                DataGridViewTextBoxColumn col4 = new DataGridViewTextBoxColumn();
                col4.DataPropertyName = "TownName";
                col4.HeaderText = "Town";
                col4.Width = 150;
                grid.Columns.Add(col4);

                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.ReadOnly = true;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = true;
            }
        }

        private void ClearGrids()
        {
            UnAssignedCustomersdataGridView.DataSource = null;
            UnAssignedCustomersdataGridView.Rows.Clear();
            AssignedCustomerdataGridView.DataSource = null;
            AssignedCustomerdataGridView.Rows.Clear();
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasUnassignedRows = unassignedCustomers != null && unassignedCustomers.Rows.Count > 0;
            bool hasAssignedRows = assignedCustomers != null && assignedCustomers.Rows.Count > 0;
            bool hasUnassignedSelection = UnAssignedCustomersdataGridView.SelectedRows.Count > 0;
            bool hasAssignedSelection = AssignedCustomerdataGridView.SelectedRows.Count > 0;

            nextbtn.Enabled = hasUnassignedSelection && !multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId);
            dblnextbtn.Enabled = hasUnassignedRows && !multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId);
            backbtn.Enabled = hasAssignedSelection && !multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId);
            bdlbackbtn.Enabled = hasAssignedRows && !multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId);
            savebtn.Enabled = !multipleSalesmanMode && !string.IsNullOrEmpty(salesmanId);
        }

        // Option A - Buttons se (Move selected rows)
        private void nextbtn_Click(object sender, EventArgs e)
        {
            MoveSelectedRows(UnAssignedCustomersdataGridView, AssignedCustomerdataGridView, true);
        }

        private void backbtn_Click(object sender, EventArgs e)
        {
            MoveSelectedRows(AssignedCustomerdataGridView, UnAssignedCustomersdataGridView, false);
        }

        private void dblnextbtn_Click(object sender, EventArgs e)
        {
            MoveAllRows(UnAssignedCustomersdataGridView, AssignedCustomerdataGridView, true);
        }

        private void bdlbackbtn_Click(object sender, EventArgs e)
        {
            MoveAllRows(AssignedCustomerdataGridView, UnAssignedCustomersdataGridView, false);
        }

        private void MoveSelectedRows(DataGridView sourceGrid, DataGridView targetGrid, bool isToAssigned)
        {
            if (sourceGrid.SelectedRows.Count == 0) return;

            try
            {
                // Get the DataTables
                DataTable sourceTable = (DataTable)sourceGrid.DataSource;
                DataTable targetTable = (DataTable)targetGrid.DataSource;

                if (sourceTable == null || targetTable == null) return;

                // Get PartyID from selected rows
                List<string> partyIdsToMove = new List<string>();
                foreach (DataGridViewRow gridRow in sourceGrid.SelectedRows)
                {
                    if (gridRow.Cells["PartyID"].Value != null)
                    {
                        partyIdsToMove.Add(gridRow.Cells["PartyID"].Value.ToString());
                    }
                }

                // For each PartyID, get full customer details and add to target table
                foreach (string partyId in partyIdsToMove)
                {
                    // Get customer details from database
                    string customerQuery = @"
                        SELECT 
                            P.PartyName as CustomerName, 
                            T.TownName
                        FROM Parties P
                        INNER JOIN Sectors S ON P.SectorID = S.SectorID
                        INNER JOIN Towns T ON S.TownID = T.TownID
                        WHERE P.PartyID = @PartyID";

                    SqlCommand customerCmd = new SqlCommand(customerQuery, connection);
                    customerCmd.Parameters.AddWithValue("@PartyID", Convert.ToInt32(partyId));

                    SqlDataReader reader = customerCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Add to target table
                        DataRow newRow = targetTable.NewRow();
                        newRow["CustomerName"] = reader["CustomerName"].ToString();
                        newRow["TownName"] = reader["TownName"].ToString();
                        targetTable.Rows.Add(newRow);
                    }
                    reader.Close();

                    // Remove from source table
                    DataRow[] rowsToRemove = sourceTable.Select($"PartyID = '{partyId}'");
                    foreach (DataRow row in rowsToRemove)
                    {
                        sourceTable.Rows.Remove(row);
                    }
                }

                // Refresh grids
                sourceGrid.DataSource = null;
                sourceGrid.DataSource = sourceTable;
                FormatGrid(sourceGrid);

                targetGrid.DataSource = null;
                targetGrid.DataSource = targetTable;
                FormatAssignedGrid();

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving rows: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MoveAllRows(DataGridView sourceGrid, DataGridView targetGrid, bool isToAssigned)
        {
            try
            {
                DataTable sourceTable = (DataTable)sourceGrid.DataSource;
                DataTable targetTable = (DataTable)targetGrid.DataSource;

                if (sourceTable == null || targetTable == null) return;

                // Get all PartyIDs from source
                List<string> allPartyIds = new List<string>();
                foreach (DataRow row in sourceTable.Rows)
                {
                    if (row["PartyID"] != null)
                    {
                        allPartyIds.Add(row["PartyID"].ToString());
                    }
                }

                // For each PartyID, get customer details and add to target
                foreach (string partyId in allPartyIds)
                {
                    // Get customer details from database
                    string customerQuery = @"
                        SELECT 
                            P.PartyName as CustomerName, 
                            T.TownName
                        FROM Parties P
                        INNER JOIN Sectors S ON P.SectorID = S.SectorID
                        INNER JOIN Towns T ON S.TownID = T.TownID
                        WHERE P.PartyID = @PartyID";

                    SqlCommand customerCmd = new SqlCommand(customerQuery, connection);
                    customerCmd.Parameters.AddWithValue("@PartyID", Convert.ToInt32(partyId));

                    SqlDataReader reader = customerCmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Add to target table
                        DataRow newRow = targetTable.NewRow();
                        newRow["CustomerName"] = reader["CustomerName"].ToString();
                        newRow["TownName"] = reader["TownName"].ToString();
                        targetTable.Rows.Add(newRow);
                    }
                    reader.Close();
                }

                // Clear source table
                sourceTable.Rows.Clear();

                // Refresh grids
                sourceGrid.DataSource = null;
                sourceGrid.DataSource = sourceTable;
                FormatGrid(sourceGrid);

                targetGrid.DataSource = null;
                targetGrid.DataSource = targetTable;
                FormatAssignedGrid();

                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving all rows: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void savebtn_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(salesmanId))
        //    {
        //        MessageBox.Show("Please select a salesman first", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    // Get current assigned customers from the grid
        //    DataTable assignedTable = (DataTable)AssignedCustomerdataGridView.DataSource;
        //    if (assignedTable == null || assignedTable.Rows.Count == 0)
        //    {
        //        MessageBox.Show("No customers assigned to save!", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    DialogResult result = MessageBox.Show("Are you sure you want to save these assignments?", "Confirm Save",
        //        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //    if (result != DialogResult.Yes) return;

        //    try
        //    {
        //        // Begin transaction
        //        SqlTransaction transaction = connection.BeginTransaction();

        //        try
        //        {
        //            // First, remove all existing assignments for this salesman
        //            string deleteQuery = "DELETE FROM SalesmenParties WHERE SalesmanID = @SalesmanID";
        //            SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction);
        //            deleteCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
        //            deleteCmd.ExecuteNonQuery();

        //            // Add new assignments from assigned grid
        //            // We need to get PartyID from PartyName and TownName
        //            string insertQuery = @"
        //                INSERT INTO SalesmenParties (SalesmanID, AccountNo) 
        //                SELECT @SalesmanID, P.PartyID 
        //                FROM Parties P
        //                INNER JOIN Sectors S ON P.SectorID = S.SectorID
        //                INNER JOIN Towns T ON S.TownID = T.TownID
        //                WHERE P.PartyName = @PartyName AND T.TownName = @TownName";

        //            foreach (DataRow row in assignedTable.Rows)
        //            {
        //                string partyName = row["CustomerName"].ToString();
        //                string townName = row["TownName"].ToString();

        //                SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction);
        //                insertCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
        //                insertCmd.Parameters.AddWithValue("@PartyName", partyName);
        //                insertCmd.Parameters.AddWithValue("@TownName", townName);

        //                int rowsAffected = insertCmd.ExecuteNonQuery();

        //                if (rowsAffected == 0)
        //                {
        //                    MessageBox.Show($"Could not find customer: {partyName} in town: {townName}", "Warning",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                }
        //            }

        //            // Commit transaction
        //            transaction.Commit();

        //            MessageBox.Show("Customer assignments saved successfully!", "Success",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);

        //            // Refresh to show current state
        //            LoadAssignedCustomersForSalesman();
        //            LoadCustomers();
        //        }
        //        catch (Exception ex)
        //        {
        //            transaction.Rollback();
        //            MessageBox.Show("Error saving assignments: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error starting transaction: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(salesmanId))
            {
                MessageBox.Show("Please select a salesman first", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable assignedTable = (DataTable)AssignedCustomerdataGridView.DataSource;
            if (assignedTable == null || assignedTable.Rows.Count == 0)
            {
                MessageBox.Show("No customers assigned to save!", "No Data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to save these assignments?",
                "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // First, remove all existing assignments for this salesman
                    string deleteQuery = "DELETE FROM SalesmenParties WHERE SalesmanID = @SalesmanID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, connection, transaction);
                    deleteCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                    deleteCmd.ExecuteNonQuery();

                    // Add new assignments
                    string insertQuery = @"
                INSERT INTO SalesmenParties (SalesmanID, AccountNo) 
                VALUES (@SalesmanID, 
                    (SELECT PartyID FROM Parties 
                     WHERE PartyName = @PartyName AND PartyType = 'C'))";

                    int successCount = 0;
                    int failCount = 0;

                    foreach (DataRow row in assignedTable.Rows)
                    {
                        string partyName = row["CustomerName"].ToString();

                        SqlCommand insertCmd = new SqlCommand(insertQuery, connection, transaction);
                        insertCmd.Parameters.AddWithValue("@SalesmanID", salesmanId);
                        insertCmd.Parameters.AddWithValue("@PartyName", partyName);

                        try
                        {
                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                                successCount++;
                            else
                                failCount++;
                        }
                        catch (SqlException ex)
                        {
                            // Log error but continue with other records
                            failCount++;
                        }
                    }

                    transaction.Commit();

                    MessageBox.Show($"Assignments saved successfully!\nSuccess: {successCount}\nFailed: {failCount}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh data
                    LoadAssignedCustomersForSalesman();
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving assignments: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting transaction: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            salesmanidtxtbox.Clear();
            salesmannametxtbox.Clear();
            salesmanId = "";
            multiplesalesmancheckbox.Checked = false;
            actualtowncombobox.SelectedIndex = 0;
            ClearGrids();
        }


        private void Assign_Customers_to_Salesman_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        // Helper class for combobox items with Text and Value
        private class ComboboxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public override string ToString() => Text;
        }
    }
}