using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Data_Backup_On_Hard_Drive : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Data_Backup_On_Hard_Drive()
        {
            InitializeComponent();
        }

        private void Data_Backup_On_Hard_Drive_Load(object sender, EventArgs e)
        {
            try
            {
                // Set current date and time
                backupdatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
                backuptimetxtbox.Text = DateTime.Now.ToString("HH:mm:ss");

                // Set default backup file name
                string defaultFileName = $"Dello_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                harddrivebackupfiletxtbox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), defaultFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void filesearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Backup Files (*.bak)|*.bak|All Files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;

                    // Set default file name
                    string defaultFileName = $"Dello_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                    saveFileDialog.FileName = defaultFileName;

                    // Set default directory to Desktop
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        harddrivebackupfiletxtbox.Text = saveFileDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backupbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Validate inputs
                if (!ValidateBackupInputs()) return;

                // Get backup path
                string backupPath = harddrivebackupfiletxtbox.Text.Trim();

                // Check if file extension is .bak, if not add it
                if (!backupPath.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
                {
                    backupPath += ".bak";
                    harddrivebackupfiletxtbox.Text = backupPath;
                }

                // Check if directory exists, create if not
                string directory = Path.GetDirectoryName(backupPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Perform backup
                PerformDatabaseBackup(backupPath);

                MessageBox.Show($"Backup completed successfully!\nFile saved at: {backupPath}",
                    "Backup Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Update time
                backuptimetxtbox.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}", "Backup Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private bool ValidateBackupInputs()
        {
            // Validate backup file path
            if (string.IsNullOrWhiteSpace(harddrivebackupfiletxtbox.Text))
            {
                MessageBox.Show("Please select a backup file location.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                filesearch.Focus();
                return false;
            }

            string backupPath = harddrivebackupfiletxtbox.Text.Trim();

            // Check if path is valid
            try
            {
                string fullPath = Path.GetFullPath(backupPath);
                string directory = Path.GetDirectoryName(fullPath);

                // Check if drive exists
                if (!Directory.Exists(Path.GetPathRoot(fullPath)))
                {
                    MessageBox.Show("Selected drive does not exist. Please select a valid hard drive.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Invalid file path. Please select a valid location.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check disk space
            try
            {
                string drive = Path.GetPathRoot(backupPath);
                DriveInfo driveInfo = new DriveInfo(drive);

                if (driveInfo.AvailableFreeSpace < 100 * 1024 * 1024) // 100 MB minimum
                {
                    DialogResult result = MessageBox.Show(
                        $"Low disk space on drive {drive}. Available: {driveInfo.AvailableFreeSpace / (1024 * 1024)} MB\n" +
                        "Do you want to continue anyway?",
                        "Low Disk Space Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return false;
                }
            }
            catch
            {
                // Silent fail for disk space check
            }

            return true;
        }

        private void PerformDatabaseBackup(string backupPath)
        {
            string backupQuery = @"
                BACKUP DATABASE [UltraBiz20232] 
                TO DISK = @BackupPath 
                WITH FORMAT, 
                     MEDIANAME = 'UltraBizBackup', 
                     NAME = 'Full Backup of UltraBiz20232',
                     STATS = 10,
                     CHECKSUM";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(backupQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@BackupPath", backupPath);

                    // Execute backup command
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void backupdatetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }
        private void backuptimetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }
        private void backuppasswordtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Not used - password removed
        }
        private void harddrivebackupfiletxtbox_TextChanged(object sender, EventArgs e)
        {
            // No action needed
        }
        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}