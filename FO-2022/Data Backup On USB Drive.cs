using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Data_Backup_On_USB_Drive : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Data_Backup_On_USB_Drive()
        {
            InitializeComponent();
        }

        private void Data_Backup_On_USB_Drive_Load(object sender, EventArgs e)
        {
            try
            {
                // Set current date and time
                backupdatetxtbox.Text = DateTime.Now.ToString("dd/MM/yyyy");
                backuptimetxtbox.Text = DateTime.Now.ToString("HH:mm:ss");

                // Auto-detect USB drives and set default path
                SetDefaultUSBPath();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetDefaultUSBPath()
        {
            try
            {
                // Find USB drives
                DriveInfo[] drives = DriveInfo.GetDrives();
                var usbDrives = drives.Where(d => d.DriveType == DriveType.Removable && d.IsReady).ToList();

                if (usbDrives.Count > 0)
                {
                    // Use first available USB drive
                    DriveInfo usbDrive = usbDrives.First();
                    string defaultFileName = $"UltraBiz_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                    usbbackupfiletxtbox.Text = Path.Combine(usbDrive.RootDirectory.FullName, defaultFileName);

                    // Show USB info
                    MessageBox.Show($"USB drive detected: {usbDrive.VolumeLabel} ({usbDrive.Name})\n" +
                                  $"Free space: {usbDrive.AvailableFreeSpace / (1024 * 1024 * 1024):F2} GB",
                                  "USB Drive Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // No USB found, use Desktop as fallback
                    string defaultFileName = $"Dello_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                    usbbackupfiletxtbox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), defaultFileName);

                    MessageBox.Show("No USB drive detected. Please insert a USB drive and click Browse.",
                                  "USB Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string defaultFileName = $"Dello_Backup_{DateTime.Now:yyyyMMdd_HHmm}.bak";
                usbbackupfiletxtbox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), defaultFileName);
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

                    // Set initial directory to USB drives if available
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    var usbDrives = drives.Where(d => d.DriveType == DriveType.Removable && d.IsReady).ToList();

                    if (usbDrives.Count > 0)
                    {
                        saveFileDialog.InitialDirectory = usbDrives.First().RootDirectory.FullName;
                    }
                    else
                    {
                        saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    }

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        usbbackupfiletxtbox.Text = saveFileDialog.FileName;

                        // Check if selected path is on USB drive
                        string selectedDrive = Path.GetPathRoot(saveFileDialog.FileName);
                        DriveInfo driveInfo = new DriveInfo(selectedDrive);

                        if (driveInfo.DriveType != DriveType.Removable)
                        {
                            DialogResult result = MessageBox.Show(
                                "Selected location is not on a USB drive.\n" +
                                "Do you want to continue with non-USB location?",
                                "Not USB Drive",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (result == DialogResult.No)
                            {
                                // Re-open dialog by calling the method again
                                filesearch_Click(sender, e);
                            }
                        }
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
                string backupPath = usbbackupfiletxtbox.Text.Trim();

                // Check if file extension is .bak, if not add it
                if (!backupPath.EndsWith(".bak", StringComparison.OrdinalIgnoreCase))
                {
                    backupPath += ".bak";
                    usbbackupfiletxtbox.Text = backupPath;
                }

                // Check USB drive availability
                string driveRoot = Path.GetPathRoot(backupPath);
                DriveInfo driveInfo = new DriveInfo(driveRoot);

                if (driveInfo.DriveType != DriveType.Removable)
                {
                    DialogResult result = MessageBox.Show(
                        "Warning: Selected location is not on a USB drive.\n" +
                        "This is USB Backup form. Do you want to continue anyway?",
                        "Not USB Drive",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                        return;
                }

                // Check if USB drive is ready
                if (!driveInfo.IsReady)
                {
                    MessageBox.Show("USB drive is not ready. Please check connection.",
                        "USB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if directory exists, create if not
                string directory = Path.GetDirectoryName(backupPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Perform backup
                PerformDatabaseBackup(backupPath);

                MessageBox.Show($"Backup completed successfully!\nFile saved on USB at: {backupPath}",
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
            if (string.IsNullOrWhiteSpace(usbbackupfiletxtbox.Text))
            {
                MessageBox.Show("Please select a backup file location.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                filesearch.Focus();
                return false;
            }

            string backupPath = usbbackupfiletxtbox.Text.Trim();

            // Check if path is valid
            try
            {
                string fullPath = Path.GetFullPath(backupPath);
                string directory = Path.GetDirectoryName(fullPath);

                // Check if drive exists
                if (!Directory.Exists(Path.GetPathRoot(fullPath)))
                {
                    MessageBox.Show("Selected drive does not exist.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Invalid file path.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Check disk space (minimum 500MB for USB)
            try
            {
                string drive = Path.GetPathRoot(backupPath);
                DriveInfo driveInfo = new DriveInfo(drive);

                if (driveInfo.AvailableFreeSpace < 500 * 1024 * 1024) // 500 MB minimum
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
                     MEDIANAME = 'UltraBizUSBBackup', 
                     NAME = 'USB Backup of UltraBiz20232',
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

        private void usbbackupfiletxtbox_TextChanged(object sender, EventArgs e)
        {
            // No action needed
        }

        private void backuppasswordtxtbox_TextChanged(object sender, EventArgs e)
        {
            // Not used - password removed
        }

        private void backupdatetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
        }

        private void backuptimetxtbox_TextChanged(object sender, EventArgs e)
        {
            // Read-only field
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