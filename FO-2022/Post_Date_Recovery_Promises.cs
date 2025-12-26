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
    public partial class Post_Date_Recovery_Promises : Form
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private SqlConnection connection;
        private int currentPromiseId = 0;
        private bool isEditing = false;

        public Post_Date_Recovery_Promises()
        {
            InitializeComponent();
            InitializeDatabaseConnection();
            LoadNextPromiseId();
        }

        private void InitializeDatabaseConnection()
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadNextPromiseId()
        {
            try
            {
                string query = "SELECT ISNULL(MAX(PromiseNo), 0) + 1 FROM Promises";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        promisesidtxtbox.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading next ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    if (isEditing)
                    {
                        UpdatePromise();
                    }
                    else
                    {
                        InsertPromise();
                    }
                    ClearForm();
                    LoadNextPromiseId();
                    isEditing = false;
                    MessageBox.Show("Promise saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving promise: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(accountnotxtbox.Text))
            {
                MessageBox.Show("Please enter account number", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(amounttxtbox.Text) || !decimal.TryParse(amounttxtbox.Text, out decimal amount))
            {
                MessageBox.Show("Please enter a valid amount", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (promisesdatepcker.Value <= entrydatepcker.Value)
            {
                MessageBox.Show("Promise date must be after entry date", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void InsertPromise()
        {
            string query = @"INSERT INTO Promises 
                            (PromiseNo, CurrentDate, AccountNo, PromiseDate, IsPayment, Narration, Amount, UserNo, IsPosted) 
                            VALUES 
                            (@PromiseNo, @CurrentDate, @AccountNo, @PromiseDate, @IsPayment, @Narration, @Amount, @UserNo, 0)";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PromiseNo", int.Parse(promisesidtxtbox.Text));
                cmd.Parameters.AddWithValue("@CurrentDate", entrydatepcker.Value);
                cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text);
                cmd.Parameters.AddWithValue("@PromiseDate", promisesdatepcker.Value);
                cmd.Parameters.AddWithValue("@IsPayment", 0); // Assuming default is not paid
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));
                cmd.Parameters.AddWithValue("@UserNo", GetCurrentUserNo()); // You need to implement this

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdatePromise()
        {
            string query = @"UPDATE Promises SET 
                            AccountNo = @AccountNo, 
                            PromiseDate = @PromiseDate, 
                            Narration = @Narration, 
                            Amount = @Amount 
                            WHERE PromiseNo = @PromiseNo";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@PromiseNo", currentPromiseId);
                cmd.Parameters.AddWithValue("@AccountNo", accountnotxtbox.Text);
                cmd.Parameters.AddWithValue("@PromiseDate", promisesdatepcker.Value);
                cmd.Parameters.AddWithValue("@Narration", narrationtxtbox.Text);
                cmd.Parameters.AddWithValue("@Amount", decimal.Parse(amounttxtbox.Text));

                cmd.ExecuteNonQuery();
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearForm();
            LoadNextPromiseId();
            isEditing = false;
        }

        private void ClearForm()
        {
            accountnotxtbox.Clear();
            accountnametxtbox.Clear();
            amounttxtbox.Clear();
            wordstxtbox.Clear();
            narrationtxtbox.Clear();
            entrydatepcker.Value = DateTime.Now;
            promisesdatepcker.Value = DateTime.Now.AddDays(1);
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (isEditing && currentPromiseId > 0)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to delete this promise?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "DELETE FROM Promises WHERE PromiseNo = @PromiseNo";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@PromiseNo", currentPromiseId);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Promise deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadNextPromiseId();
                        isEditing = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting promise: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No promise selected to delete", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create custom input dialog
                Form inputForm = new Form();
                inputForm.Text = "Open Promise";
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.StartPosition = FormStartPosition.CenterParent;

                Label label = new Label();
                label.Text = "Enter Promise ID to open:";
                label.SetBounds(10, 20, 280, 20);

                TextBox textBox = new TextBox();
                textBox.SetBounds(10, 45, 260, 20);

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.SetBounds(60, 80, 75, 25);

                Button cancelButton = new Button();
                cancelButton.Text = "Cancel";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.SetBounds(140, 80, 75, 25);

                inputForm.Controls.Add(label);
                inputForm.Controls.Add(textBox);
                inputForm.Controls.Add(okButton);
                inputForm.Controls.Add(cancelButton);
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string input = textBox.Text;

                    if (!string.IsNullOrEmpty(input))
                    {
                        if (int.TryParse(input, out int promiseId))
                        {
                            LoadPromiseData(promiseId);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid numeric Promise ID!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening promise: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPromiseData(int promiseId)
        {
            try
            {
                string query = "SELECT * FROM Promises WHERE PromiseNo = @PromiseNo";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PromiseNo", promiseId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentPromiseId = promiseId;
                            isEditing = true;

                            promisesidtxtbox.Text = reader["PromiseNo"].ToString();
                            entrydatepcker.Value = Convert.ToDateTime(reader["CurrentDate"]);
                            accountnotxtbox.Text = reader["AccountNo"].ToString();
                            promisesdatepcker.Value = Convert.ToDateTime(reader["PromiseDate"]);
                            narrationtxtbox.Text = reader["Narration"].ToString();
                            amounttxtbox.Text = reader["Amount"].ToString();

                            // Fetch account name
                            LoadAccountName(accountnotxtbox.Text);
                            // Convert amount to words
                            ConvertAmountToWords(amounttxtbox.Text);

                            MessageBox.Show("Promise loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Promise not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading promise: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void accountnosearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(accountnotxtbox.Text))
            {
                LoadAccountName(accountnotxtbox.Text);
            }
            else
            {
                MessageBox.Show("Please enter account number", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadAccountName(string accountNo)
        {
            try
            {
                string query = "SELECT AccountName FROM ChartOfAccounts WHERE AccountNo = @AccountNo";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@AccountNo", accountNo);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        accountnametxtbox.Text = result.ToString();
                    }
                    else
                    {
                        accountnametxtbox.Text = "Account not found";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching account name: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                accountnametxtbox.Text = "Error loading";
            }
        }

        private void amountsearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(amounttxtbox.Text) && decimal.TryParse(amounttxtbox.Text, out decimal amount))
            {
                ConvertAmountToWords(amounttxtbox.Text);
            }
            else
            {
                MessageBox.Show("Please enter a valid amount", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ConvertAmountToWords(string amountText)
        {
            try
            {
                decimal amount = decimal.Parse(amountText);
                wordstxtbox.Text = NumberToWords(amount);
            }
            catch
            {
                wordstxtbox.Text = "Invalid amount";
            }
        }

        private string NumberToWords(decimal number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            long integerPart = (long)Math.Floor(number);
            decimal fractionalPart = number - integerPart;

            if (integerPart > 0)
            {
                words = ConvertIntegerToWords(integerPart);
            }

            if (fractionalPart > 0)
            {
                int paise = (int)(fractionalPart * 100);
                if (!string.IsNullOrEmpty(words))
                    words += " and ";
                words += ConvertIntegerToWords(paise) + " Paise";
            }

            return words + " Only";
        }

        private string ConvertIntegerToWords(long number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + ConvertIntegerToWords(Math.Abs(number));

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ConvertIntegerToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertIntegerToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertIntegerToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        private int GetCurrentUserNo()
        {
            // You need to implement this based on your authentication system
            // For now, returning 1 as default
            return 1;
        }

        private void Post_Date_Recovery_Promises_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void promisesidtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: You can add validation here if needed
        }

        private void entrydatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Set minimum promise date to be after entry date
            promisesdatepcker.MinDate = entrydatepcker.Value.AddDays(1);
        }

        private void promisesdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add any validation if needed
        }

        private void accountnotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Clear account name when account number changes
            if (!string.IsNullOrEmpty(accountnametxtbox.Text) && accountnametxtbox.Text != "Account not found" && accountnametxtbox.Text != "Error loading")
            {
                accountnametxtbox.Clear();
            }
        }

        private void narrationtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Optional: Add character limit validation if needed
        }

        private void Post_Date_Recovery_Promises_Load(object sender, EventArgs e)
        {
            // Set initial date values
            entrydatepcker.Value = DateTime.Now;
            promisesdatepcker.Value = DateTime.Now.AddDays(1);
            promisesdatepcker.MinDate = entrydatepcker.Value.AddDays(1);
        }
    }
}