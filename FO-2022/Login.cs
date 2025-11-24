using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Login : Form
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public Login()
        {
            InitializeComponent();
        }
        private void loginbtn_Click(object sender, EventArgs e)
        {
          
        }
        private void signupbtn_Click(object sender, EventArgs e)
        {
            // Open signup form
            SignUp su = new SignUp();
            su.Show();
            this.Hide();
        }
        private void Login_Load(object sender, EventArgs e)
        {
        }
        private void loginbtn_Click_1(object sender, EventArgs e)
        {
       
            string username = usertxtbox.Text.Trim();
            string password = passwordtxtbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both username and password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT IsAdministrator FROM dbo.Users WHERE UserName=@UserName AND Password=@Password";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    con.Open();

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int isAdmin = Convert.ToInt32(result); // 1 = Admin, 0 = Normal user

                        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Mainform1 mf = new Mainform1();
                        mf.isAdmin = (isAdmin == 1);

                        mf.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void signoutbtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
