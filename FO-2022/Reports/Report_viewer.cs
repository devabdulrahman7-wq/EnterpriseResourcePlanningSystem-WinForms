//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Windows.Forms;
//using Microsoft.Reporting.WinForms;

//namespace FO_2022.Reports
//{
//    public partial class Report_viewer : Form
//    {
//        private string _reportName;
//        private Dictionary<string, DataTable> _dataSources;

//        public Report_viewer()
//        {
//            InitializeComponent();
//        }

//        public void ShowReport(string reportName, Dictionary<string, DataTable> dataSources)
//        {
//            _reportName = reportName;
//            _dataSources = dataSources;
//            this.ShowDialog();
//        }

//        private void Report_viewer_Load(object sender, EventArgs e)
//        {
//            try
//            {
//                // 📌 Get RDLC path
//                string rdlcPath = GetReportPath();

//                if (!File.Exists(rdlcPath))
//                {
//                    throw new FileNotFoundException($"Report file not found: {rdlcPath}");
//                }

//                // 📌 Set report path
//                reportViewer1.LocalReport.ReportPath = rdlcPath;

//                // 📌 Clear old data
//                reportViewer1.LocalReport.DataSources.Clear();

//                // 📌 Add new data sources
//                foreach (var dataSource in _dataSources)
//                {
//                    ReportDataSource rds = new ReportDataSource(
//                        dataSource.Key,
//                        dataSource.Value
//                    );
//                    reportViewer1.LocalReport.DataSources.Add(rds);
//                }

//                // 📌 Refresh report
//                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
//                reportViewer1.ZoomMode = ZoomMode.PageWidth;
//                reportViewer1.RefreshReport();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading report: {ex.Message}\n\n" +
//                    $"Report Name: {_reportName}",
//                    "Report Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//                this.Close();
//            }
//        }

//        private string GetReportPath()
//        {
//            string[] possibleExtensions = { ".rdlc", "" };
//            string[] possibleLocations =
//            {
//                // 1. Application startup path with Reports folder
//                Path.Combine(Application.StartupPath, "Reports"),

//                // 2. Application startup path
//                Application.StartupPath,

//                // 3. Current directory with Reports folder
//                Path.Combine(Directory.GetCurrentDirectory(), "Reports"),

//                // 4. Project folder (for development)
//                GetProjectFolderPath()
//            };

//            foreach (var location in possibleLocations)
//            {
//                if (!string.IsNullOrEmpty(location))
//                {
//                    foreach (var extension in possibleExtensions)
//                    {
//                        string fileName = _reportName.EndsWith(".rdlc", StringComparison.OrdinalIgnoreCase)
//                            ? _reportName
//                            : _reportName + ".rdlc";

//                        string fullPath = Path.Combine(location, fileName);

//                        if (File.Exists(fullPath))
//                        {
//                            return fullPath;
//                        }
//                    }
//                }
//            }

//            throw new FileNotFoundException($"Could not find report file '{_reportName}.rdlc'. " +
//                $"Searched in: {string.Join(", ", possibleLocations)}");
//        }

//        private string GetProjectFolderPath()
//        {
//            try
//            {
//                // Try to find project folder (for development environment)
//                string startupPath = Application.StartupPath;
//                DirectoryInfo dir = new DirectoryInfo(startupPath);

//                // Go up until we find a folder with .csproj file or Reports folder
//                while (dir != null)
//                {
//                    // Check for Reports folder
//                    string reportsPath = Path.Combine(dir.FullName, "Reports");
//                    if (Directory.Exists(reportsPath))
//                    {
//                        return reportsPath;
//                    }

//                    // Check for .csproj file
//                    string[] csprojFiles = Directory.GetFiles(dir.FullName, "*.csproj");
//                    if (csprojFiles.Length > 0)
//                    {
//                        return Path.Combine(dir.FullName, "Reports");
//                    }

//                    dir = dir.Parent;
//                }
//            }
//            catch { }

//            return null;
//        }

//        private void btnClose_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class Report_viewer : Form
    {
        private string _reportName;
        private Dictionary<string, DataTable> _dataSources;
        private Dictionary<string, string> _parameters;
        private List<ReportParameter> _reportParameters;

        public Report_viewer()
        {
            InitializeComponent();
        }

        // 📌 MAIN METHOD TO SHOW REPORT
        public void ShowReport(string reportName, Dictionary<string, DataTable> dataSources,
                              Dictionary<string, string> parameters = null,
                              List<ReportParameter> reportParameters = null)
        {
            _reportName = reportName;
            _dataSources = dataSources;
            _parameters = parameters;
            _reportParameters = reportParameters;
            this.ShowDialog();
        }

        private void Report_viewer_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                LoadReport();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // 📌 LOAD REPORT METHOD
        private void LoadReport()
        {
            // Get RDLC path
            string rdlcPath = GetReportPath(_reportName);
            ValidateFileExists(rdlcPath);

            // Setup report
            reportViewer1.LocalReport.ReportPath = rdlcPath;
            reportViewer1.LocalReport.DataSources.Clear();

            // Add data sources
            foreach (var dataSource in _dataSources)
            {
                reportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource(dataSource.Key, dataSource.Value));
            }

            // Handle parameters
            SetReportParameters();

            // Configure viewer
            ConfigureViewerSettings();

            // Refresh report
            reportViewer1.RefreshReport();
        }

        // 📌 GET REPORT PATH (Optimized for multiple locations)
        private string GetReportPath(string reportName)
        {
            // Ensure .rdlc extension
            if (!reportName.EndsWith(".rdlc", StringComparison.OrdinalIgnoreCase))
                reportName += ".rdlc";

            // List of possible locations (priority order)
            var searchLocations = new List<string>
            {
                // 1. Current directory with Reports folder
                Path.Combine(Application.StartupPath, "Reports"),
                
                // 2. Application startup path
                Application.StartupPath,
                
                // 3. Project folder for development
                GetProjectFolder(),
                
                // 4. Common report locations
                Path.Combine(Directory.GetCurrentDirectory(), "Reports"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports"),
                Path.Combine(Environment.CurrentDirectory, "Reports"),
                
                // 5. Relative paths
                Path.Combine(Application.StartupPath, @"..\..\Reports"),
                Path.Combine(Application.StartupPath, @"..\..\..\Reports")
            };

            // Remove duplicates and nulls
            searchLocations = searchLocations
                .Where(loc => !string.IsNullOrEmpty(loc))
                .Distinct()
                .ToList();

            // Search in locations
            foreach (var location in searchLocations)
            {
                string fullPath = Path.Combine(location, reportName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            // If not found, throw detailed error
            throw new FileNotFoundException(
                $"Report '{reportName}' not found. Searched in:\n" +
                string.Join("\n", searchLocations.Select(l => $"• {l}")));
        }

        // 📌 VALIDATE FILE EXISTS
        private void ValidateFileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Report file not found: {filePath}");
            }
        }

        // 📌 SET REPORT PARAMETERS
        private void SetReportParameters()
        {
            List<ReportParameter> parameters = new List<ReportParameter>();

            // Add simple string parameters
            if (_parameters != null && _parameters.Count > 0)
            {
                foreach (var param in _parameters)
                {
                    parameters.Add(new ReportParameter(param.Key, param.Value));
                }
            }

            // Add complex parameters if provided
            if (_reportParameters != null && _reportParameters.Count > 0)
            {
                parameters.AddRange(_reportParameters);
            }

            // Set parameters if any
            if (parameters.Count > 0)
            {
                reportViewer1.LocalReport.SetParameters(parameters);
            }
        }

        // 📌 CONFIGURE VIEWER SETTINGS
        private void ConfigureViewerSettings()
        {
            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.ZoomPercent = 100;
            reportViewer1.ShowToolBar = true;
            reportViewer1.ShowZoomControl = true;
            reportViewer1.ShowPrintButton = true;
            reportViewer1.ShowExportButton = true;
            reportViewer1.ShowRefreshButton = true;
        }

        // 📌 GET PROJECT FOLDER
        private string GetProjectFolder()
        {
            try
            {
                string currentPath = Application.StartupPath;
                DirectoryInfo dir = new DirectoryInfo(currentPath);

                // Go up to find project folder
                while (dir != null)
                {
                    // Check for Reports folder
                    string reportsPath = Path.Combine(dir.FullName, "Reports");
                    if (Directory.Exists(reportsPath))
                    {
                        return reportsPath;
                    }

                    // Check for .csproj file (project root)
                    if (Directory.GetFiles(dir.FullName, "*.csproj").Length > 0)
                    {
                        return Path.Combine(dir.FullName, "Reports");
                    }

                    dir = dir.Parent;
                }
            }
            catch
            {
                // Silent fail - return null
            }

            return null;
        }

        // 📌 SHOW ERROR
        private void ShowError(Exception ex)
        {
            MessageBox.Show(
                $"Error loading report '{_reportName}':\n\n{ex.Message}\n\n" +
                $"Stack Trace:\n{ex.StackTrace}",
                "Report Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            this.Close();
        }

        // 📌 CLOSE BUTTON
        //private void btnClose_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}

        //// 📌 FORM CLOSING
        //private void Report_viewer_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    // Clean up resources
        //    if (reportViewer1.LocalReport != null)
        //    {
        //        reportViewer1.LocalReport.DataSources.Clear();
        //        reportViewer1.LocalReport.ReleaseSandboxAppDomain();
        //    }
        //}

        //// 📌 KEYBOARD SHORTCUTS
        //private void Report_viewer_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Escape)
        //        this.Close();
        //    else if (e.Control && e.KeyCode == Keys.P)
        //        reportViewer1.PrintDialog();
        //    else if (e.Control && e.KeyCode == Keys.S)
        //        SaveReport();
        //}

        // 📌 SAVE REPORT (Export functionality)
        //private void SaveReport()
        //{
        //    try
        //    {
        //        using (SaveFileDialog saveDialog = new SaveFileDialog())
        //        {
        //            saveDialog.Filter = "PDF files (*.pdf)|*.pdf|Excel files (*.xlsx)|*.xlsx|Word files (*.docx)|*.docx";
        //            saveDialog.FilterIndex = 1;
        //            saveDialog.RestoreDirectory = true;

        //            if (saveDialog.ShowDialog() == DialogResult.OK)
        //            {
        //                string fileName = saveDialog.FileName;
        //                string fileType = Path.GetExtension(fileName).ToLower();

        //                byte[] bytes = reportViewer1.LocalReport.Render(
        //                    GetRenderFormat(fileType),
        //                    deviceInfo: null,
        //                    out string mimeType,
        //                    out string encoding,
        //                    out string fileNameExtension,
        //                    out string[] streams,
        //                    out Warning[] warnings);

        //                File.WriteAllBytes(fileName, bytes);
        //                MessageBox.Show($"Report saved successfully to:\n{fileName}", "Success",
        //                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving report: {ex.Message}", "Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // 📌 GET RENDER FORMAT
        private string GetRenderFormat(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".pdf": return "PDF";
                case ".xlsx": return "EXCELOPENXML";
                case ".docx": return "WORDOPENXML";
                default: return "PDF";
            }
        }
    }
}