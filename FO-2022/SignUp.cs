using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;


namespace FO_2022
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
        }

        private void signupbtn_Click(object sender, EventArgs e)
        {
            // 🔹 Get connection string from App.config
            string connectionString = ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString;

            // 🔹 Get input
            string username = usertxtbox.Text.Trim();
            string password = passwordtxtbox.Text.Trim();

            // 🔹 Validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Optional fields (you can leave them null)
            string email = null;
            bool? isAdmin = null; // null means default
            DateTime? createdDate = null; // will default to GETDATE()

            // 🔹 Insert query
            string query = @"INSERT INTO dbo.Users 
                            (UserName, Password, Email, IsAdministrator, CreatedDate)
                            VALUES (@UserName, @Password, @Email, @IsAdministrator, @CreatedDate)";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsAdministrator", (object)isAdmin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedDate", (object)createdDate ?? DBNull.Value);

                    con.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Signup successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Mainform1 mf = new Mainform1();
                        mf.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Signup failed. Try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loginbtn_Click(object sender, EventArgs e)
        {
            Login lo = new Login();
            lo.Show();
            this.Hide();
        }
    }
}
