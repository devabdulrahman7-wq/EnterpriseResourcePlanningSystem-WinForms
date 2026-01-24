using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace FO_2022
{
    public partial class System_Settings : Form
    {
        private string connectionString;
        private Dictionary<string, string> registrySettings;
        private Image companyLogo;
        private bool isImageChanged = false;
        public System_Settings()
        {
            InitializeComponent();
           
        }
        private void System_Settings_Load(object sender, EventArgs e)
        {
            // Set focus to company name textbox
            //ClearForm();
            InitializeConnection();
            LoadRegistrySettings();
            LoadCompanyLogo();
            cmpnynametxtbox.Focus();
        }


        private void InitializeConnection()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading connection string: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadRegistrySettings()
        {
            registrySettings = new Dictionary<string, string>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT RegistryKey, RegistryValue FROM Registry";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string key = reader["RegistryKey"].ToString();
                                string value = reader["RegistryValue"].ToString();
                                registrySettings[key] = value;
                            }
                        }
                    }
                }

                // Populate form fields from registry settings
                cmpnynametxtbox.Text = GetRegistryValue("CompanyName", "");
                addresstxtbox.Text = GetRegistryValue("CompanyAddress", "");
                phonetxtbox.Text = GetRegistryValue("CompanyPhone", "");
                phonenotxtbox.Text = GetRegistryValue("CompanyPhone2", "");
                mbltxtbox.Text = GetRegistryValue("CompanyMobile", "");
                faxtxtbox.Text = GetRegistryValue("CompanyFax", "");
                emailtxtbox.Text = GetRegistryValue("CompanyEmail", "");
                wwwtxtbox.Text = GetRegistryValue("CompanyWebsite", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetRegistryValue(string key, string defaultValue)
        {
            return registrySettings.ContainsKey(key) ? registrySettings[key] : defaultValue;
        }

        private void LoadCompanyLogo()
        {
            try
            {
                string logoPath = GetRegistryValue("CompanyLogoPath", "");
                if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                {
                    companyLogo = Image.FromFile(logoPath);
                    pictureBox1.Image = companyLogo;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading company logo: {ex.Message}", "Logo Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

      
        private void cmpnyinformationbtn_Click(object sender, EventArgs e)
        {
            // Show company information section (already visible)
            // Reset tab order to company name
            cmpnynametxtbox.Focus();
        }

        private void dellosoftsetting_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("DelloSoft Setting feature is not implemented yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void browseimgbtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load and display the image
                        companyLogo = Image.FromFile(openFileDialog.FileName);
                        pictureBox1.Image = companyLogo;
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        isImageChanged = true;

                        // Save the file path to registry
                        SaveRegistrySetting("CompanyLogoPath", openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void clearimgbtn_Click(object sender, EventArgs e)
        {
            // Clear the picture box
            pictureBox1.Image = null;
            companyLogo = null;
            isImageChanged = true;

            // Remove logo path from registry
            SaveRegistrySetting("CompanyLogoPath", "");
        }
        private bool ValidateForm()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(cmpnynametxtbox.Text))
            {
                MessageBox.Show("Company name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmpnynametxtbox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(addresstxtbox.Text))
            {
                MessageBox.Show("Company address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                addresstxtbox.Focus();
                return false;
            }

            // Validate email format if provided
            if (!string.IsNullOrWhiteSpace(emailtxtbox.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(emailtxtbox.Text);
                    if (addr.Address != emailtxtbox.Text.Trim())
                    {
                        MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        emailtxtbox.Focus();
                        return false;
                    }
                }
                catch
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    emailtxtbox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void SaveAllSettings()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Save company information
                SaveRegistrySetting("CompanyName", cmpnynametxtbox.Text.Trim());
                SaveRegistrySetting("CompanyAddress", addresstxtbox.Text.Trim());
                SaveRegistrySetting("CompanyPhone", phonetxtbox.Text.Trim());
                SaveRegistrySetting("CompanyPhone2", phonenotxtbox.Text.Trim());
                SaveRegistrySetting("CompanyMobile", mbltxtbox.Text.Trim());
                SaveRegistrySetting("CompanyFax", faxtxtbox.Text.Trim());
                SaveRegistrySetting("CompanyEmail", emailtxtbox.Text.Trim());
                SaveRegistrySetting("CompanyWebsite", wwwtxtbox.Text.Trim());

                // Save logo image to database if changed
                //if (isImageChanged && companyLogo != null)
                //{
                //    //SaveLogoToDatabase(conn);
                //}
                //else if (isImageChanged && companyLogo == null)
                //{
                //    // Clear logo from database
                //    //ClearLogoFromDatabase(conn);
                //}

                // Update local dictionary
                registrySettings["CompanyName"] = cmpnynametxtbox.Text.Trim();
                registrySettings["CompanyAddress"] = addresstxtbox.Text.Trim();
                registrySettings["CompanyPhone"] = phonetxtbox.Text.Trim();
                registrySettings["CompanyPhone2"] = phonenotxtbox.Text.Trim();
                registrySettings["CompanyMobile"] = mbltxtbox.Text.Trim();
                registrySettings["CompanyFax"] = faxtxtbox.Text.Trim();
                registrySettings["CompanyEmail"] = emailtxtbox.Text.Trim();
                registrySettings["CompanyWebsite"] = wwwtxtbox.Text.Trim();
            }

            isImageChanged = false;
        }

        private void SaveRegistrySetting(string key, string value)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use MERGE (UPSERT) operation
                    string query = @"
                    MERGE Registry AS target
                    USING (SELECT @Key AS RegistryKey, @Value AS RegistryValue) AS source
                    ON target.RegistryKey = source.RegistryKey
                    WHEN MATCHED THEN
                        UPDATE SET RegistryValue = source.RegistryValue
                    WHEN NOT MATCHED THEN
                        INSERT (RegistryKey, RegistryValue)
                        VALUES (source.RegistryKey, source.RegistryValue);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Key", key);
                        cmd.Parameters.AddWithValue("@Value", value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving registry setting '{key}': {ex.Message}", ex);
            }
        }

        //private void SaveLogoToDatabase(SqlConnection conn)
        //{
        //    try
        //    {
        //        // Convert image to byte array
        //        byte[] imageBytes;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            companyLogo.Save(ms, companyLogo.RawFormat);
        //            imageBytes = ms.ToArray();
        //        }

        //        // Check if registry entry for logo exists
        //        string checkQuery = "SELECT COUNT(*) FROM Registry WHERE RegistryKey = 'CompanyLogo'";
        //        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
        //        {
        //            int count = (int)checkCmd.ExecuteScalar();

        //            if (count > 0)
        //            {
        //                // Update existing logo
        //                string updateQuery = "UPDATE Registry SET RegistryValue = @LogoData WHERE RegistryKey = 'CompanyLogo'";
        //                using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
        //                {
        //                    updateCmd.Parameters.Add("@LogoData", System.Data.SqlDbType.Image).Value = imageBytes;
        //                    updateCmd.ExecuteNonQuery();
        //                }
        //            }
        //            else
        //            {
        //                // Insert new logo
        //                string insertQuery = "INSERT INTO Registry (RegistryKey, RegistryValue) VALUES ('CompanyLogo', @LogoData)";
        //                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
        //                {
        //                    insertCmd.Parameters.Add("@LogoData", System.Data.SqlDbType.Image).Value = imageBytes;
        //                    insertCmd.ExecuteNonQuery();
        //                }
        //            }
        //        }

        //        // Also save the file path
        //        SaveRegistrySetting("CompanyLogoPath", GetRegistryValue("CompanyLogoPath", ""));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error saving logo to database: {ex.Message}", ex);
        //    }
        //}

        //private void ClearLogoFromDatabase(SqlConnection conn)
        //{
        //    try
        //    {
        //        string deleteQuery = "DELETE FROM Registry WHERE RegistryKey = 'CompanyLogo'";
        //        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
        //        {
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error clearing logo from database: {ex.Message}", ex);
        //    }
        //}
        //closebtn_Click
        //
        //if (isImageChanged || HasUnsavedChanges())
        //{
        //    DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?",
        //        "Unsaved Changes",
        //        MessageBoxButtons.YesNoCancel,
        //        MessageBoxIcon.Question);

        //    if (result == DialogResult.Yes)
        //    {
        //        if (ValidateForm())
        //        {
        //            SaveAllSettings();
        //            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        }
        //        else
        //        {
        //            return; // Don't close if validation fails
        //        }
        //    }
        //    else if (result == DialogResult.Cancel)
        //    {
        //        return; // Don't close
        //    }
        //}



        private bool HasUnsavedChanges()
        {
            return cmpnynametxtbox.Text != GetRegistryValue("CompanyName", "") ||
                   addresstxtbox.Text != GetRegistryValue("CompanyAddress", "") ||
                   phonetxtbox.Text != GetRegistryValue("CompanyPhone", "") ||
                   phonenotxtbox.Text != GetRegistryValue("CompanyPhone2", "") ||
                   mbltxtbox.Text != GetRegistryValue("CompanyMobile", "") ||
                   faxtxtbox.Text != GetRegistryValue("CompanyFax", "") ||
                   emailtxtbox.Text != GetRegistryValue("CompanyEmail", "") ||
                   wwwtxtbox.Text != GetRegistryValue("CompanyWebsite", "");
        }

        private void phonenotxtbox_KeyDown(object sender, KeyEventArgs e)
        {
            HandleNavigationKeys(sender, e);
        }

        private void HandleNavigationKeys(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SelectNextControl((Control)sender, true, true, true, true);
            }
            //else if (e.KeyCode == Keys.Escape)
            //{
            //    closebtn_Click(sender, e);
            //}
            //else if (e.Control && e.KeyCode == Keys.S)
            //{
            //    savebtn_Click(sender, e);
            //}
        }

        // Event handlers for text changes to track unsaved changes
        private void cmpnynametxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void phonenotxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void phonetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void faxtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void wwwtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void emailtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void mbltxtbox_TextChanged(object sender, EventArgs e)
        {
            // Text changed, can be used to track changes if needed
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Optional: Click to browse image
            browseimgbtn_Click(sender, e);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //closebtn_Click(sender, e);
            this.Close();
            //Mainform1 mf = new Mainform1();
            //mf.Show();
        }

        private void savebtn_Click_1(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveAllSettings();
                    MessageBox.Show("Settings saved successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset the flag after saving
                    isImageChanged = false;
                    //ClearForm();
                    //LoadRegistrySettings(); // Reload settings to confirm save
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving settings: {ex.Message}",
                                  "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //private void ClearForm()
        //{
        //    // Clear all textboxes
        //    cmpnynametxtbox.Clear();
        //    addresstxtbox.Clear();
        //    phonetxtbox.Clear();
        //    phonenotxtbox.Clear();
        //    mbltxtbox.Clear();
        //    faxtxtbox.Clear();
        //    emailtxtbox.Clear();
        //    wwwtxtbox.Clear();

        //    // Clear the picture box
        //    pictureBox1.Image = null;
        //    companyLogo = null;

        //    // Clear registry settings dictionary
        //    registrySettings.Clear();

        //    // Set focus to first field
        //    cmpnynametxtbox.Focus();
        //}

        private void closebtn_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Mainform1 mf = new Mainform1();
            mf.Show();
        }

        //private void System_Settings_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (isImageChanged || HasUnsavedChanges())
        //    {
        //        DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?",
        //            "Unsaved Changes",
        //            MessageBoxButtons.YesNoCancel,
        //            MessageBoxIcon.Question);

        //        if (result == DialogResult.Yes)
        //        {
        //            if (ValidateForm())
        //            {
        //                SaveAllSettings();
        //                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //            else
        //            {
        //                e.Cancel = true; // Cancel closing if validation fails
        //            }
        //        }
        //        else if (result == DialogResult.Cancel)
        //        {
        //            e.Cancel = true; // Cancel closing
        //        }
        //    }
        //}
    }
}