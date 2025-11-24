using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class CreateUserForm : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public CreateUserForm()
        {
            InitializeComponent();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            string username = usernametxtbox.Text.Trim();
            string password = passwordtxtbox.Text.Trim();
            string retypePassword = retypepasswordtxtbox.Text.Trim();
            int userType = standarduserbtn.Checked ? 0 : 1; // 0 = Standard, 1 = Admin

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(retypePassword))
            {
                MessageBox.Show("All fields are required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != retypePassword)
            {
                MessageBox.Show("Password and Re-Type Password do not match!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "INSERT INTO dbo.Users (UserName, Password, IsAdministrator) VALUES (@UserName, @Password, @IsAdmin)";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@IsAdmin", userType);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadUserList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void ClearFields()
        {
            usernametxtbox.Clear();
            passwordtxtbox.Clear();
            retypepasswordtxtbox.Clear();
            standarduserbtn.Checked = true;
        }
        private void LoadUserList()
        {
            string query = "SELECT UserNo, UserName FROM dbo.Users";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                {
                    System.Data.DataTable dt = new System.Data.DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void removeBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int userNo = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UserNo"].Value);

                string query = "DELETE FROM dbo.Users WHERE UserNo=@UserNo";

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserNo", userNo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void editBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int userNo = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UserNo"].Value);
                string username = usernametxtbox.Text.Trim();
                string password = passwordtxtbox.Text.Trim();
                int userType = standarduserbtn.Checked ? 0 : 1;

                string query = "UPDATE dbo.Users SET UserName=@UserName, Password=@Password, IsAdministrator=@IsAdmin WHERE UserNo=@UserNo";

                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@IsAdmin", userType);
                        cmd.Parameters.AddWithValue("@UserNo", userNo);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void newBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void CreateUserForm_Load(object sender, EventArgs e)
        {
            LoadUserList();
        }
    }
}
