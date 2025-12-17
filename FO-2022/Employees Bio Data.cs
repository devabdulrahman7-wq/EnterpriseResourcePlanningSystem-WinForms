//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace FO_2022
//{
//    public partial class Employees_Bio_Data : Form
//    {
//        private SqlConnection connection;
//        private DataTable employeesData;

//        //private string connectionString = "Data Source=SQL5110.site4now.net;Initial Catalog=db_ac0035_pos;User Id=db_ac0035_pos_admin;Password=PTCL0614581188!";
//        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//        private bool isEditMode = false;
//        private string currentEmployeeId = "";
//        private byte[] employeePicture = null;

//        public Employees_Bio_Data()
//        {
//            InitializeComponent();
//            InitializeConnection();
//            LoadDesignations();
//            LoadJobTypes();
//            LoadEmployeesGrid();
//        }

//        private void InitializeConnection()
//        {
//            connection = new SqlConnection(connectionString);
//        }

//        private void LoadDesignations()
//        {
//            try
//            {
//                string query = "SELECT DesignationID, DesignationName FROM PayRolDesignations";
//                SqlCommand cmd = new SqlCommand(query, connection);

//                connection.Open();
//                SqlDataReader reader = cmd.ExecuteReader();

//                designationcombobox.Items.Clear();
//                while (reader.Read())
//                {
//                    designationcombobox.Items.Add(reader["DesignationName"].ToString());
//                }
//                reader.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading designations: " + ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }
//        private void LoadJobTypes()
//        {
//            jobtypecpmbobox.Items.Clear();
//            jobtypecpmbobox.Items.Add("Regular");
//            jobtypecpmbobox.Items.Add("Contract");
//            jobtypecpmbobox.Items.Add("Daily wages");
//        }

//        private void LoadEmployeesGrid()
//        {
//            try
//            {
//                string query = @"
//                    SELECT 
//                        e.EmployeeID,
//                        e.EmployeeName,
//                        e.FatherName,
//                        e.SalaryWages as Salary,
//                        e.IsDisabled
//                    FROM PayRolEmployees e
//                    ORDER BY e.EmployeeID";

//                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
//                employeesData = new DataTable();
//                adapter.Fill(employeesData);

//                dataGridView1.DataSource = employeesData;
//                dataGridView1.Columns["IsDisabled"].Visible = false;

//                // Update group box with status
//                UpdateEmployeeStatus();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading employees: " + ex.Message);
//            }
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

//        private void activebtn_Click(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(currentEmployeeId))
//            {
//                UpdateEmployeeStatusInDB(false);
//                UpdateEmployeeStatus();
//            }
//        }

//        private void deactivebtn_Click(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(currentEmployeeId))
//            {
//                UpdateEmployeeStatusInDB(true);
//                UpdateEmployeeStatus();
//            }
//        }

//        private void UpdateEmployeeStatusInDB(bool isDisabled)
//        {
//            try
//            {
//                string query = "UPDATE PayRolEmployees SET IsDisabled = @IsDisabled WHERE EmployeeID = @EmployeeID";

//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@IsDisabled", isDisabled);
//                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

//                connection.Open();
//                cmd.ExecuteNonQuery();

//                LoadEmployeesGrid();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error updating status: " + ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void UpdateEmployeeStatus()
//        {
//            if (!string.IsNullOrEmpty(currentEmployeeId))
//            {
//                try
//                {
//                    string query = "SELECT IsDisabled FROM PayRolEmployees WHERE EmployeeID = @EmployeeID";
//                    SqlCommand cmd = new SqlCommand(query, connection);
//                    cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

//                    connection.Open();
//                    object result = cmd.ExecuteScalar();

//                    if (result != null && result != DBNull.Value)
//                    {
//                        bool isDisabled = Convert.ToBoolean(result);
//                        label16.Text = isDisabled ? "Deactive" : "Active";
//                    }
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("Error getting status: " + ex.Message);
//                }
//                finally
//                {
//                    if (connection.State == ConnectionState.Open)
//                        connection.Close();
//                }
//            }
//        }

//        private void removebtn_Click(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(currentEmployeeId))
//            {
//                if (MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Delete",
//                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
//                {
//                    DeleteEmployee();
//                }
//            }
//        }

//        private void DeleteEmployee()
//        {
//            try
//            {
//                // Start transaction to delete from both tables
//                connection.Open();
//                SqlTransaction transaction = connection.BeginTransaction();

//                try
//                {
//                    // Delete from PayRolEmployees
//                    string deleteEmployeeQuery = "DELETE FROM PayRolEmployees WHERE EmployeeID = @EmployeeID";
//                    SqlCommand cmd1 = new SqlCommand(deleteEmployeeQuery, connection, transaction);
//                    cmd1.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
//                    cmd1.ExecuteNonQuery();

//                    // Delete from ChartOfAccounts (linked via AccountNo)
//                    string deleteAccountQuery = "DELETE FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
//                    SqlCommand cmd2 = new SqlCommand(deleteAccountQuery, connection, transaction);
//                    cmd2.Parameters.AddWithValue("@AccountNo", currentEmployeeId);
//                    cmd2.ExecuteNonQuery();

//                    transaction.Commit();

//                    MessageBox.Show("Employee deleted successfully!");
//                    ClearForm();
//                    LoadEmployeesGrid();
//                }
//                catch
//                {
//                    transaction.Rollback();
//                    throw;
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error deleting employee: " + ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void editbtn_Click(object sender, EventArgs e)
//        {
//            isEditMode = true;
//            savebtn.Text = "Update";
//        }

//        private void newbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//            isEditMode = false;
//            savebtn.Text = "Save";
//        }

//        private void clearbtn_Click(object sender, EventArgs e)
//        {
//            ClearForm();
//        }

//        private void ClearForm()
//        {
//            // Clear all textboxes
//            nametxtbox.Clear();
//            fathernametxtbox.Clear();
//            addresstxtbox.Clear();
//            citytxtbox.Clear();
//            cnictxtbox.Clear();
//            eduacationtxtbox.Clear();
//            phonenotxtbox.Clear();
//            mblnotxtbox.Clear();
//            employeeidtxtbox.Clear();
//            employeeidtxtbox1.Clear();
//            urdunametxtbox.Clear();
//            fatherurdunametxtbox.Clear();
//            urduaddresstxtbox.Clear();
//            referencetxtbox.Clear();
//            salarytxtbox.Clear();
//            yearlyleavesallowedtxtbox.Clear();

//            // Clear comboboxes
//            designationcombobox.SelectedIndex = -1;
//            jobtypecpmbobox.SelectedIndex = -1;

//            // Clear picture
//            employeepictxtbox.Image = null;
//            employeePicture = null;

//            // Reset date pickers
//            joiningdatetimepcker.Value = DateTime.Now;
//            terminationpckr.Value = DateTime.Now;

//            // Clear status
//            label16.Text = "";
//            currentEmployeeId = "";
//            empchkbar.Checked = false;

//            isEditMode = false;
//            savebtn.Text = "Save";
//        }

//        private void savebtn_Click(object sender, EventArgs e)
//        {
//            if (ValidateForm())
//            {
//                if (isEditMode)
//                {
//                    UpdateEmployee();
//                }
//                else
//                {
//                    InsertEmployee();
//                }
//            }
//        }

//        private bool ValidateForm()
//        {
//            if (string.IsNullOrEmpty(nametxtbox.Text))
//            {
//                MessageBox.Show("Please enter employee name");
//                return false;
//            }

//            if (string.IsNullOrEmpty(fathernametxtbox.Text))
//            {
//                MessageBox.Show("Please enter father name");
//                return false;
//            }

//            if (string.IsNullOrEmpty(employeeidtxtbox1.Text))
//            {
//                MessageBox.Show("Please enter employee ID");
//                return false;
//            }

//            if (string.IsNullOrEmpty(salarytxtbox.Text))
//            {
//                MessageBox.Show("Please enter salary");
//                return false;
//            }

//            if (!decimal.TryParse(salarytxtbox.Text, out decimal salary))
//            {
//                MessageBox.Show("Please enter valid salary");
//                return false;
//            }

//            if (designationcombobox.SelectedIndex == -1)
//            {
//                MessageBox.Show("Please select designation");
//                return false;
//            }

//            if (jobtypecpmbobox.SelectedIndex == -1)
//            {
//                MessageBox.Show("Please select job type");
//                return false;
//            }

//            return true;
//        }

//        private void InsertEmployee()
//        {
//            try
//            {
//                connection.Open();
//                SqlTransaction transaction = connection.BeginTransaction();

//                try
//                {
//                    string employeeId = employeeidtxtbox1.Text.Trim();
//                    currentEmployeeId = employeeId;

//                    // Insert into ChartOfAccounts first
//                    string accountQuery = @"
//                        INSERT INTO ChartOfAccounts 
//                        (AccountNo, UserNo, AccountName, AccountType, AccountDepth, 
//                         ParentAccountNo, OpeningDebit, OpeningCredit, AdjustedDebit, 
//                         AdjustedCredit, IsDetailed, IsLocked, IsPosted, IsEditable, 
//                         BalFlag, PLFlag, ExpFlag)
//                        VALUES 
//                        (@AccountNo, 1, @AccountName, 'Parties', 3, 
//                         '0201010000', 0, 0, 0, 0, 0, 0, 1, 1, 0, '0', 0)";

//                    SqlCommand accountCmd = new SqlCommand(accountQuery, connection, transaction);
//                    accountCmd.Parameters.AddWithValue("@AccountNo", employeeId);
//                    accountCmd.Parameters.AddWithValue("@AccountName", nametxtbox.Text.Trim());
//                    accountCmd.ExecuteNonQuery();

//                    // Insert into PayRolEmployees
//                    string employeeQuery = @"
//                        INSERT INTO PayRolEmployees 
//                        (EmployeeID, DepartmentId, EmployeeName, FatherName, NICNo, 
//                         DesignationID, Education, Address, City, Phone, Reference, 
//                         JobType, JoiningDate, TerminationDate, IsOverTimeApplicable, 
//                         SalaryWages, YearlyLeavesAllowed, IsDisabled, AccountNo, 
//                         EmployeeNameUrdu, FatherNameUrdu, AddressUrdu)
//                        VALUES 
//                        (@EmployeeID, '01', @EmployeeName, @FatherName, @NICNo,
//                         @DesignationID, @Education, @Address, @City, @Phone, @Reference,
//                         @JobType, @JoiningDate, @TerminationDate, 0,
//                         @SalaryWages, @YearlyLeavesAllowed, 0, @AccountNo,
//                         @EmployeeNameUrdu, @FatherNameUrdu, @AddressUrdu)";

//                    SqlCommand employeeCmd = new SqlCommand(employeeQuery, connection, transaction);

//                    // Add parameters
//                    employeeCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
//                    employeeCmd.Parameters.AddWithValue("@EmployeeName", nametxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@FatherName", fathernametxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@NICNo", cnictxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@DesignationID", GetDesignationId(designationcombobox.Text));
//                    employeeCmd.Parameters.AddWithValue("@Education", eduacationtxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@Reference", referencetxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@JobType", jobtypecpmbobox.Text);
//                    employeeCmd.Parameters.AddWithValue("@JoiningDate", joiningdatetimepcker.Value);
//                    employeeCmd.Parameters.AddWithValue("@TerminationDate",
//                        terminationpckr.Value < joiningdatetimepcker.Value ?
//                        (object)DBNull.Value : terminationpckr.Value);
//                    employeeCmd.Parameters.AddWithValue("@SalaryWages", decimal.Parse(salarytxtbox.Text));
//                    employeeCmd.Parameters.AddWithValue("@YearlyLeavesAllowed",
//                        string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) ? 0 : int.Parse(yearlyleavesallowedtxtbox.Text));
//                    employeeCmd.Parameters.AddWithValue("@AccountNo", employeeId);
//                    employeeCmd.Parameters.AddWithValue("@EmployeeNameUrdu", urdunametxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@FatherNameUrdu", fatherurdunametxtbox.Text.Trim());
//                    employeeCmd.Parameters.AddWithValue("@AddressUrdu", urduaddresstxtbox.Text.Trim());

//                    employeeCmd.ExecuteNonQuery();

//                    // Insert picture if available
//                    if (employeePicture != null)
//                    {
//                        string pictureQuery = @"
//                            INSERT INTO EmployeesPicture (EmployeeID, Picture)
//                            VALUES (@EmployeeID, @Picture)";

//                        SqlCommand pictureCmd = new SqlCommand(pictureQuery, connection, transaction);
//                        pictureCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
//                        pictureCmd.Parameters.Add("@Picture", SqlDbType.Image).Value = employeePicture;
//                        pictureCmd.ExecuteNonQuery();
//                    }

//                    transaction.Commit();

//                    MessageBox.Show("Employee saved successfully!");
//                    LoadEmployeesGrid();
//                    ClearForm();
//                }
//                catch (Exception ex)
//                {
//                    transaction.Rollback();
//                    throw new Exception("Error saving employee: " + ex.Message);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void UpdateEmployee()
//        {
//            try
//            {
//                string query = @"
//                    UPDATE PayRolEmployees 
//                    SET EmployeeName = @EmployeeName,
//                        FatherName = @FatherName,
//                        NICNo = @NICNo,
//                        DesignationID = @DesignationID,
//                        Education = @Education,
//                        Address = @Address,
//                        City = @City,
//                        Phone = @Phone,
//                        Reference = @Reference,
//                        JobType = @JobType,
//                        JoiningDate = @JoiningDate,
//                        TerminationDate = @TerminationDate,
//                        SalaryWages = @SalaryWages,
//                        YearlyLeavesAllowed = @YearlyLeavesAllowed,
//                        EmployeeNameUrdu = @EmployeeNameUrdu,
//                        FatherNameUrdu = @FatherNameUrdu,
//                        AddressUrdu = @AddressUrdu
//                    WHERE EmployeeID = @EmployeeID";

//                SqlCommand cmd = new SqlCommand(query, connection);

//                // Add parameters
//                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
//                cmd.Parameters.AddWithValue("@EmployeeName", nametxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@FatherName", fathernametxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@NICNo", cnictxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@DesignationID", GetDesignationId(designationcombobox.Text));
//                cmd.Parameters.AddWithValue("@Education", eduacationtxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@Reference", referencetxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@JobType", jobtypecpmbobox.Text);
//                cmd.Parameters.AddWithValue("@JoiningDate", joiningdatetimepcker.Value);
//                cmd.Parameters.AddWithValue("@TerminationDate",
//                    terminationpckr.Value < joiningdatetimepcker.Value ?
//                    (object)DBNull.Value : terminationpckr.Value);
//                cmd.Parameters.AddWithValue("@SalaryWages", decimal.Parse(salarytxtbox.Text));
//                cmd.Parameters.AddWithValue("@YearlyLeavesAllowed",
//                    string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) ? 0 : int.Parse(yearlyleavesallowedtxtbox.Text));
//                cmd.Parameters.AddWithValue("@EmployeeNameUrdu", urdunametxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@FatherNameUrdu", fatherurdunametxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@AddressUrdu", urduaddresstxtbox.Text.Trim());

//                connection.Open();
//                cmd.ExecuteNonQuery();

//                // Update picture if available
//                if (employeePicture != null)
//                {
//                    UpdateEmployeePicture();
//                }

//                // Update ChartOfAccounts
//                UpdateChartOfAccounts();

//                MessageBox.Show("Employee updated successfully!");
//                LoadEmployeesGrid();
//                ClearForm();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error updating employee: " + ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void UpdateEmployeePicture()
//        {
//            try
//            {
//                string checkQuery = "SELECT COUNT(*) FROM EmployeesPicture WHERE EmployeeID = @EmployeeID";
//                SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
//                checkCmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

//                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

//                string pictureQuery;
//                if (count > 0)
//                {
//                    pictureQuery = "UPDATE EmployeesPicture SET Picture = @Picture WHERE EmployeeID = @EmployeeID";
//                }
//                else
//                {
//                    pictureQuery = "INSERT INTO EmployeesPicture (EmployeeID, Picture) VALUES (@EmployeeID, @Picture)";
//                }

//                SqlCommand pictureCmd = new SqlCommand(pictureQuery, connection);
//                pictureCmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
//                pictureCmd.Parameters.Add("@Picture", SqlDbType.Image).Value = employeePicture;
//                pictureCmd.ExecuteNonQuery();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error updating picture: " + ex.Message);
//            }
//        }

//        private void UpdateChartOfAccounts()
//        {
//            try
//            {
//                string query = "UPDATE ChartOfAccounts SET AccountName = @AccountName WHERE AccountNo = @AccountNo";
//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@AccountName", nametxtbox.Text.Trim());
//                cmd.Parameters.AddWithValue("@AccountNo", currentEmployeeId);
//                cmd.ExecuteNonQuery();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error updating chart of accounts: " + ex.Message);
//            }
//        }

//        private int GetDesignationId(string designationName)
//        {
//            try
//            {
//                string query = "SELECT DesignationID FROM PayRolDesignations WHERE DesignationName = @DesignationName";
//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@DesignationName", designationName);

//                if (connection.State != ConnectionState.Open)
//                    connection.Open();

//                object result = cmd.ExecuteScalar();
//                return result != null ? Convert.ToInt32(result) : 1;
//            }
//            catch
//            {
//                return 1; // Default designation ID
//            }
//        }

//        private void employeepictxtbox_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog openFileDialog = new OpenFileDialog())
//            {
//                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    try
//                    {
//                        Image image = Image.FromFile(openFileDialog.FileName);
//                        employeepictxtbox.Image = image;

//                        // Convert image to byte array
//                        using (MemoryStream ms = new MemoryStream())
//                        {
//                            image.Save(ms, image.RawFormat);
//                            employeePicture = ms.ToArray();
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show("Error loading image: " + ex.Message);
//                    }
//                }
//            }
//        }

//        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
//            {
//                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

//                currentEmployeeId = row.Cells["EmployeeID"].Value.ToString();
//                employeeidtxtbox.Text = currentEmployeeId;
//                employeeidtxtbox1.Text = currentEmployeeId;
//                nametxtbox.Text = row.Cells["EmployeeName"].Value.ToString();
//                fathernametxtbox.Text = row.Cells["FatherName"].Value.ToString();
//                salarytxtbox.Text = row.Cells["Salary"].Value.ToString();

//                // Load additional details from database
//                LoadEmployeeDetails(currentEmployeeId);

//                isEditMode = true;
//                savebtn.Text = "Update";
//            }
//        }

//        private void LoadEmployeeDetails(string employeeId)
//        {
//            try
//            {
//                string query = @"
//                    SELECT 
//                        e.NICNo, e.Education, e.Address, e.City, e.Phone, e.Mobile,
//                        e.Reference, e.JobType, e.JoiningDate, e.TerminationDate,
//                        e.YearlyLeavesAllowed, e.EmployeeNameUrdu, e.FatherNameUrdu,
//                        e.AddressUrdu, d.DesignationName,
//                        ep.Picture
//                    FROM PayRolEmployees e
//                    LEFT JOIN PayRolDesignations d ON e.DesignationID = d.DesignationID
//                    LEFT JOIN EmployeesPicture ep ON e.EmployeeID = ep.EmployeeID
//                    WHERE e.EmployeeID = @EmployeeID";

//                SqlCommand cmd = new SqlCommand(query, connection);
//                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

//                connection.Open();
//                SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    cnictxtbox.Text = reader["NICNo"].ToString();
//                    eduacationtxtbox.Text = reader["Education"].ToString();
//                    addresstxtbox.Text = reader["Address"].ToString();
//                    citytxtbox.Text = reader["City"].ToString();
//                    phonenotxtbox.Text = reader["Phone"].ToString();
//                    mblnotxtbox.Text = reader["Mobile"].ToString();
//                    referencetxtbox.Text = reader["Reference"].ToString();

//                    // Set job type
//                    string jobType = reader["JobType"].ToString();
//                    jobtypecpmbobox.SelectedItem = jobType;

//                    // Set designation
//                    string designation = reader["DesignationName"].ToString();
//                    designationcombobox.SelectedItem = designation;

//                    // Set dates
//                    if (reader["JoiningDate"] != DBNull.Value)
//                        joiningdatetimepcker.Value = Convert.ToDateTime(reader["JoiningDate"]);

//                    if (reader["TerminationDate"] != DBNull.Value)
//                        terminationpckr.Value = Convert.ToDateTime(reader["TerminationDate"]);

//                    yearlyleavesallowedtxtbox.Text = reader["YearlyLeavesAllowed"].ToString();
//                    urdunametxtbox.Text = reader["EmployeeNameUrdu"].ToString();
//                    fatherurdunametxtbox.Text = reader["FatherNameUrdu"].ToString();
//                    urduaddresstxtbox.Text = reader["AddressUrdu"].ToString();

//                    // Load picture
//                    if (reader["Picture"] != DBNull.Value)
//                    {
//                        employeePicture = (byte[])reader["Picture"];
//                        using (MemoryStream ms = new MemoryStream(employeePicture))
//                        {
//                            employeepictxtbox.Image = Image.FromStream(ms);
//                        }
//                    }
//                    else
//                    {
//                        employeepictxtbox.Image = null;
//                        employeePicture = null;
//                    }
//                }

//                reader.Close();
//                UpdateEmployeeStatus();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error loading employee details: " + ex.Message);
//            }
//            finally
//            {
//                if (connection.State == ConnectionState.Open)
//                    connection.Close();
//            }
//        }

//        private void jobtypecpmbobox_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            // Optional: Add any logic when job type changes
//        }

//        private void salarytxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Validate numeric input
//            if (!string.IsNullOrEmpty(salarytxtbox.Text) && !decimal.TryParse(salarytxtbox.Text, out _))
//            {
//                MessageBox.Show("Please enter a valid salary amount");
//                salarytxtbox.Text = "";
//            }
//        }

//        private void yearlyleavesallowedtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Validate numeric input
//            if (!string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) && !int.TryParse(yearlyleavesallowedtxtbox.Text, out _))
//            {
//                MessageBox.Show("Please enter a valid number for yearly leaves");
//                yearlyleavesallowedtxtbox.Text = "";
//            }
//        }

//        private void referencetxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add any logic
//        }

//        private void cnictxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add CNIC validation logic
//        }

//        private void eduacationtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add any logic
//        }

//        private void citytxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add any logic
//        }

//        private void mblnotxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add mobile number validation
//        }

//        private void phonenotxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // Optional: Add phone number validation
//        }

//        private void employeeidtxtbox_TextChanged(object sender, EventArgs e)
//        {
//            // This is the AccountNo from ChartOfAccounts - read-only in edit mode
//        }

//        // Add this method to handle form load
//        private void Employees_Bio_Data_Load(object sender, EventArgs e)
//        {
//            // Add event handler for grid cell click
//            dataGridView1.CellClick += dataGridView1_CellClick;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Employees_Bio_Data : Form
    {
        private DataTable employeesData;
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private bool isEditMode = false;
        private string currentEmployeeId = "";
        private byte[] employeePicture = null;

        public Employees_Bio_Data()
        {
            InitializeComponent();
            LoadDesignations();
            LoadJobTypes();
            LoadEmployeesGrid();
        }

        private void LoadDesignations()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT DesignationID, DesignationName FROM PayRolDesignations";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    designationcombobox.Items.Clear();
                    while (reader.Read())
                    {
                        designationcombobox.Items.Add(reader["DesignationName"].ToString());
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading designations: " + ex.Message);
                }
            }
        }

        private void LoadJobTypes()
        {
            jobtypecpmbobox.Items.Clear();
            jobtypecpmbobox.Items.Add("Regular");
            jobtypecpmbobox.Items.Add("Contract");
            jobtypecpmbobox.Items.Add("Daily wages");
        }

        private void LoadEmployeesGrid()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            e.EmployeeID,
                            e.EmployeeName,
                            e.FatherName,
                            e.SalaryWages as Salary,
                            e.IsDisabled
                        FROM PayRolEmployees e
                        ORDER BY e.EmployeeID";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    employeesData = new DataTable();
                    adapter.Fill(employeesData);

                    dataGridView1.DataSource = employeesData;
                    dataGridView1.Columns["IsDisabled"].Visible = false;

                    // Update group box with status
                    UpdateEmployeeStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading employees: " + ex.Message);
                }
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

        private void activebtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentEmployeeId))
            {
                UpdateEmployeeStatusInDB(false);
                UpdateEmployeeStatus();
            }
        }

        private void deactivebtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentEmployeeId))
            {
                UpdateEmployeeStatusInDB(true);
                UpdateEmployeeStatus();
            }
        }

        private void UpdateEmployeeStatusInDB(bool isDisabled)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "UPDATE PayRolEmployees SET IsDisabled = @IsDisabled WHERE EmployeeID = @EmployeeID";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@IsDisabled", isDisabled);
                    cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    LoadEmployeesGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating status: " + ex.Message);
                }
            }
        }

        private void UpdateEmployeeStatus()
        {
            if (!string.IsNullOrEmpty(currentEmployeeId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        string query = "SELECT IsDisabled FROM PayRolEmployees WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

                        connection.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            bool isDisabled = Convert.ToBoolean(result);
                            label16.Text = isDisabled ? "Deactive" : "Active";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error getting status: " + ex.Message);
                    }
                }
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentEmployeeId))
            {
                if (MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteEmployee();
                }
            }
        }

        private void DeleteEmployee()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Start transaction to delete from both tables
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Delete from PayRolEmployees
                        string deleteEmployeeQuery = "DELETE FROM PayRolEmployees WHERE EmployeeID = @EmployeeID";
                        SqlCommand cmd1 = new SqlCommand(deleteEmployeeQuery, connection, transaction);
                        cmd1.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
                        cmd1.ExecuteNonQuery();

                        // Delete from ChartOfAccounts (linked via AccountNo)
                        string deleteAccountQuery = "DELETE FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                        SqlCommand cmd2 = new SqlCommand(deleteAccountQuery, connection, transaction);
                        cmd2.Parameters.AddWithValue("@AccountNo", currentEmployeeId);
                        cmd2.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show("Employee deleted successfully!");
                        ClearForm();
                        LoadEmployeesGrid();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting employee: " + ex.Message);
                }
            }
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            isEditMode = true;
            savebtn.Text = "Update";
        }

        private void newbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            isEditMode = false;
            savebtn.Text = "Save";
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            // Clear all textboxes
            nametxtbox.Clear();
            fathernametxtbox.Clear();
            addresstxtbox.Clear();
            citytxtbox.Clear();
            cnictxtbox.Clear();
            eduacationtxtbox.Clear();
            phonenotxtbox.Clear();
            mblnotxtbox.Clear();
            employeeidtxtbox.Clear();
            employeeidtxtbox1.Clear();
            urdunametxtbox.Clear();
            fatherurdunametxtbox.Clear();
            urduaddresstxtbox.Clear();
            referencetxtbox.Clear();
            salarytxtbox.Clear();
            yearlyleavesallowedtxtbox.Clear();

            // Clear comboboxes
            designationcombobox.SelectedIndex = -1;
            jobtypecpmbobox.SelectedIndex = -1;

            // Clear picture
            employeepictxtbox.Image = null;
            employeePicture = null;

            // Reset date pickers
            joiningdatetimepcker.Value = DateTime.Now;
            terminationpckr.Value = DateTime.Now;

            // Clear status
            label16.Text = "";
            currentEmployeeId = "";
            empchkbar.Checked = false;

            isEditMode = false;
            savebtn.Text = "Save";
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                if (isEditMode)
                {
                    UpdateEmployee();
                }
                else
                {
                    InsertEmployee();
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(nametxtbox.Text))
            {
                MessageBox.Show("Please enter employee name");
                return false;
            }

            if (string.IsNullOrEmpty(fathernametxtbox.Text))
            {
                MessageBox.Show("Please enter father name");
                return false;
            }

            if (string.IsNullOrEmpty(employeeidtxtbox1.Text))
            {
                MessageBox.Show("Please enter employee ID");
                return false;
            }

            if (string.IsNullOrEmpty(salarytxtbox.Text))
            {
                MessageBox.Show("Please enter salary");
                return false;
            }

            if (!decimal.TryParse(salarytxtbox.Text, out decimal salary))
            {
                MessageBox.Show("Please enter valid salary");
                return false;
            }

            if (designationcombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select designation");
                return false;
            }

            if (jobtypecpmbobox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select job type");
                return false;
            }

            return true;
        }

        private void InsertEmployee()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        string employeeId = employeeidtxtbox1.Text.Trim();
                        currentEmployeeId = employeeId;

                        // Insert into ChartOfAccounts first
                        string accountQuery = @"
                            INSERT INTO ChartOfAccounts 
                            (AccountNo, UserNo, AccountName, AccountType, AccountDepth, 
                             ParentAccountNo, OpeningDebit, OpeningCredit, AdjustedDebit, 
                             AdjustedCredit, IsDetailed, IsLocked, IsPosted, IsEditable, 
                             BalFlag, PLFlag, ExpFlag)
                            VALUES 
                            (@AccountNo, 1, @AccountName, 'Parties', 3, 
                             '0201010000', 0, 0, 0, 0, 0, 0, 1, 1, 0, '0', 0)";

                        SqlCommand accountCmd = new SqlCommand(accountQuery, connection, transaction);
                        accountCmd.Parameters.AddWithValue("@AccountNo", employeeId);
                        accountCmd.Parameters.AddWithValue("@AccountName", nametxtbox.Text.Trim());
                        accountCmd.ExecuteNonQuery();

                        // Insert into PayRolEmployees
                        string employeeQuery = @"
                            INSERT INTO PayRolEmployees 
                            (EmployeeID, DepartmentId, EmployeeName, FatherName, NICNo, 
                             DesignationID, Education, Address, City, Phone, Reference, 
                             JobType, JoiningDate, TerminationDate, IsOverTimeApplicable, 
                             SalaryWages, YearlyLeavesAllowed, IsDisabled, AccountNo, 
                             EmployeeNameUrdu, FatherNameUrdu, AddressUrdu)
                            VALUES 
                            (@EmployeeID, '01', @EmployeeName, @FatherName, @NICNo,
                             @DesignationID, @Education, @Address, @City, @Phone, @Reference,
                             @JobType, @JoiningDate, @TerminationDate, 0,
                             @SalaryWages, @YearlyLeavesAllowed, 0, @AccountNo,
                             @EmployeeNameUrdu, @FatherNameUrdu, @AddressUrdu)";

                        SqlCommand employeeCmd = new SqlCommand(employeeQuery, connection, transaction);

                        // Add parameters
                        employeeCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        employeeCmd.Parameters.AddWithValue("@EmployeeName", nametxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@FatherName", fathernametxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@NICNo", cnictxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@DesignationID", GetDesignationId(designationcombobox.Text));
                        employeeCmd.Parameters.AddWithValue("@Education", eduacationtxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@Reference", referencetxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@JobType", jobtypecpmbobox.Text);
                        employeeCmd.Parameters.AddWithValue("@JoiningDate", joiningdatetimepcker.Value);
                        employeeCmd.Parameters.AddWithValue("@TerminationDate",
                            terminationpckr.Value < joiningdatetimepcker.Value ?
                            (object)DBNull.Value : terminationpckr.Value);
                        employeeCmd.Parameters.AddWithValue("@SalaryWages", decimal.Parse(salarytxtbox.Text));
                        employeeCmd.Parameters.AddWithValue("@YearlyLeavesAllowed",
                            string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) ? 0 : int.Parse(yearlyleavesallowedtxtbox.Text));
                        employeeCmd.Parameters.AddWithValue("@AccountNo", employeeId);
                        employeeCmd.Parameters.AddWithValue("@EmployeeNameUrdu", urdunametxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@FatherNameUrdu", fatherurdunametxtbox.Text.Trim());
                        employeeCmd.Parameters.AddWithValue("@AddressUrdu", urduaddresstxtbox.Text.Trim());

                        employeeCmd.ExecuteNonQuery();

                        // Insert picture if available
                        if (employeePicture != null)
                        {
                            string pictureQuery = @"
                                INSERT INTO EmployeesPicture (EmployeeID, Picture)
                                VALUES (@EmployeeID, @Picture)";

                            SqlCommand pictureCmd = new SqlCommand(pictureQuery, connection, transaction);
                            pictureCmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                            pictureCmd.Parameters.Add("@Picture", SqlDbType.Image).Value = employeePicture;
                            pictureCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show("Employee saved successfully!");
                        LoadEmployeesGrid();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error saving employee: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdateEmployee()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        UPDATE PayRolEmployees 
                        SET EmployeeName = @EmployeeName,
                            FatherName = @FatherName,
                            NICNo = @NICNo,
                            DesignationID = @DesignationID,
                            Education = @Education,
                            Address = @Address,
                            City = @City,
                            Phone = @Phone,
                            Reference = @Reference,
                            JobType = @JobType,
                            JoiningDate = @JoiningDate,
                            TerminationDate = @TerminationDate,
                            SalaryWages = @SalaryWages,
                            YearlyLeavesAllowed = @YearlyLeavesAllowed,
                            EmployeeNameUrdu = @EmployeeNameUrdu,
                            FatherNameUrdu = @FatherNameUrdu,
                            AddressUrdu = @AddressUrdu
                        WHERE EmployeeID = @EmployeeID";

                    SqlCommand cmd = new SqlCommand(query, connection);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
                    cmd.Parameters.AddWithValue("@EmployeeName", nametxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@FatherName", fathernametxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@NICNo", cnictxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@DesignationID", GetDesignationId(designationcombobox.Text));
                    cmd.Parameters.AddWithValue("@Education", eduacationtxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", addresstxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@City", citytxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", phonenotxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Reference", referencetxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@JobType", jobtypecpmbobox.Text);
                    cmd.Parameters.AddWithValue("@JoiningDate", joiningdatetimepcker.Value);
                    cmd.Parameters.AddWithValue("@TerminationDate",
                        terminationpckr.Value < joiningdatetimepcker.Value ?
                        (object)DBNull.Value : terminationpckr.Value);
                    cmd.Parameters.AddWithValue("@SalaryWages", decimal.Parse(salarytxtbox.Text));
                    cmd.Parameters.AddWithValue("@YearlyLeavesAllowed",
                        string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) ? 0 : int.Parse(yearlyleavesallowedtxtbox.Text));
                    cmd.Parameters.AddWithValue("@EmployeeNameUrdu", urdunametxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@FatherNameUrdu", fatherurdunametxtbox.Text.Trim());
                    cmd.Parameters.AddWithValue("@AddressUrdu", urduaddresstxtbox.Text.Trim());

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    // Update picture if available
                    if (employeePicture != null)
                    {
                        UpdateEmployeePicture(connection);
                    }

                    // Update ChartOfAccounts
                    UpdateChartOfAccounts(connection);

                    MessageBox.Show("Employee updated successfully!");
                    LoadEmployeesGrid();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating employee: " + ex.Message);
                }
            }
        }

        private void UpdateEmployeePicture(SqlConnection connection)
        {
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM EmployeesPicture WHERE EmployeeID = @EmployeeID";
                SqlCommand checkCmd = new SqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                string pictureQuery;
                if (count > 0)
                {
                    pictureQuery = "UPDATE EmployeesPicture SET Picture = @Picture WHERE EmployeeID = @EmployeeID";
                }
                else
                {
                    pictureQuery = "INSERT INTO EmployeesPicture (EmployeeID, Picture) VALUES (@EmployeeID, @Picture)";
                }

                SqlCommand pictureCmd = new SqlCommand(pictureQuery, connection);
                pictureCmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeId);
                pictureCmd.Parameters.Add("@Picture", SqlDbType.Image).Value = employeePicture;
                pictureCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating picture: " + ex.Message);
            }
        }

        private void UpdateChartOfAccounts(SqlConnection connection)
        {
            try
            {
                string query = "UPDATE ChartOfAccounts SET AccountName = @AccountName WHERE AccountNo = @AccountNo";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AccountName", nametxtbox.Text.Trim());
                cmd.Parameters.AddWithValue("@AccountNo", currentEmployeeId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating chart of accounts: " + ex.Message);
            }
        }

        private int GetDesignationId(string designationName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "SELECT DesignationID FROM PayRolDesignations WHERE DesignationName = @DesignationName";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@DesignationName", designationName);

                    connection.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 1;
                }
                catch
                {
                    return 1; // Default designation ID
                }
            }
        }

        private void employeepictxtbox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Image image = Image.FromFile(openFileDialog.FileName);
                        employeepictxtbox.Image = image;

                        // Convert image to byte array
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, image.RawFormat);
                            employeePicture = ms.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message);
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                currentEmployeeId = row.Cells["EmployeeID"].Value.ToString();
                employeeidtxtbox.Text = currentEmployeeId;
                employeeidtxtbox1.Text = currentEmployeeId;
                nametxtbox.Text = row.Cells["EmployeeName"].Value.ToString();
                fathernametxtbox.Text = row.Cells["FatherName"].Value.ToString();
                salarytxtbox.Text = row.Cells["Salary"].Value.ToString();

                // Load additional details from database
                LoadEmployeeDetails(currentEmployeeId);

                isEditMode = true;
                savebtn.Text = "Update";
            }
        }

        private void LoadEmployeeDetails(string employeeId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"
                        SELECT 
                            e.NICNo, e.Education, e.Address, e.City, e.Phone, e.Mobile,
                            e.Reference, e.JobType, e.JoiningDate, e.TerminationDate,
                            e.YearlyLeavesAllowed, e.EmployeeNameUrdu, e.FatherNameUrdu,
                            e.AddressUrdu, d.DesignationName,
                            ep.Picture
                        FROM PayRolEmployees e
                        LEFT JOIN PayRolDesignations d ON e.DesignationID = d.DesignationID
                        LEFT JOIN EmployeesPicture ep ON e.EmployeeID = ep.EmployeeID
                        WHERE e.EmployeeID = @EmployeeID";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        cnictxtbox.Text = reader["NICNo"].ToString();
                        eduacationtxtbox.Text = reader["Education"].ToString();
                        addresstxtbox.Text = reader["Address"].ToString();
                        citytxtbox.Text = reader["City"].ToString();
                        phonenotxtbox.Text = reader["Phone"].ToString();
                        mblnotxtbox.Text = reader["Mobile"].ToString();
                        referencetxtbox.Text = reader["Reference"].ToString();

                        // Set job type
                        string jobType = reader["JobType"].ToString();
                        jobtypecpmbobox.SelectedItem = jobType;

                        // Set designation
                        string designation = reader["DesignationName"].ToString();
                        designationcombobox.SelectedItem = designation;

                        // Set dates
                        if (reader["JoiningDate"] != DBNull.Value)
                            joiningdatetimepcker.Value = Convert.ToDateTime(reader["JoiningDate"]);

                        if (reader["TerminationDate"] != DBNull.Value)
                            terminationpckr.Value = Convert.ToDateTime(reader["TerminationDate"]);

                        yearlyleavesallowedtxtbox.Text = reader["YearlyLeavesAllowed"].ToString();
                        urdunametxtbox.Text = reader["EmployeeNameUrdu"].ToString();
                        fatherurdunametxtbox.Text = reader["FatherNameUrdu"].ToString();
                        urduaddresstxtbox.Text = reader["AddressUrdu"].ToString();

                        // Load picture
                        if (reader["Picture"] != DBNull.Value)
                        {
                            employeePicture = (byte[])reader["Picture"];
                            using (MemoryStream ms = new MemoryStream(employeePicture))
                            {
                                employeepictxtbox.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            employeepictxtbox.Image = null;
                            employeePicture = null;
                        }
                    }

                    reader.Close();
                    UpdateEmployeeStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading employee details: " + ex.Message);
                }
            }
        }

        private void jobtypecpmbobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Add any logic when job type changes
        }

        private void salarytxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input
            if (!string.IsNullOrEmpty(salarytxtbox.Text) && !decimal.TryParse(salarytxtbox.Text, out _))
            {
                MessageBox.Show("Please enter a valid salary amount");
                salarytxtbox.Text = "";
            }
        }

        private void yearlyleavesallowedtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Validate numeric input
            if (!string.IsNullOrEmpty(yearlyleavesallowedtxtbox.Text) && !int.TryParse(yearlyleavesallowedtxtbox.Text, out _))
            {
                MessageBox.Show("Please enter a valid number for yearly leaves");
                yearlyleavesallowedtxtbox.Text = "";
            }
        }

        private void referencetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add any logic
        }

        private void cnictxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add CNIC validation logic
        }

        private void eduacationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add any logic
        }

        private void citytxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add any logic
        }

        private void mblnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add mobile number validation
        }

        private void phonenotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add phone number validation
        }

        private void employeeidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // This is the AccountNo from ChartOfAccounts - read-only in edit mode
        }

        // Add this method to handle form load
        private void Employees_Bio_Data_Load(object sender, EventArgs e)
        {
            // Add event handler for grid cell click
            dataGridView1.CellClick += dataGridView1_CellClick;
        }
    }
}