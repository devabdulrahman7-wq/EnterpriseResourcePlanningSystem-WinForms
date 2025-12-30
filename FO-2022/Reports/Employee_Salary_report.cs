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
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class Employee_Salary_report : Form
    {
        // Properties to receive data from main form
        public DataTable EmployeeData { get; set; }
        public string ReportTitle { get; set; }

        public Employee_Salary_report()
        {
            InitializeComponent();
        }
        private void Employee_Salary_report_Load(object sender, EventArgs e)
        {
            LoadReport();
        }
        private void LoadReport()
        {
            try
            {
                if (EmployeeData == null || EmployeeData.Rows.Count == 0)
                {
                    MessageBox.Show("No employee data available.", "Information");
                    this.Close();
                    return;
                }
                // Find report file
                string reportPath = FindReportFile("Employee_Salary_rv.rdlc");

                if (string.IsNullOrEmpty(reportPath))
                {
                    MessageBox.Show("Report file not found.", "Error");
                    this.Close();
                    return;
                }
                // Setup report viewer
                reportViewer1.LocalReport.ReportPath = reportPath;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet1", EmployeeData));

                // Add report title if parameter exists
                try
                {
                    Microsoft.Reporting.WinForms.ReportParameter titleParam =
                        new Microsoft.Reporting.WinForms.ReportParameter("ReportTitle",
                            string.IsNullOrEmpty(ReportTitle) ? "Employee Salary Sheet" : ReportTitle);
                    reportViewer1.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter[] { titleParam });
                }
                catch
                {
                    // Parameter doesn't exist in RDLC, that's OK
                }

                // Setup layout
                SetupReportLayout();

                // Refresh report
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message, "Error");
                this.Close();
            }
        }

        private string FindReportFile(string fileName)
        {
            string[] possiblePaths =
            {
                Path.Combine(Application.StartupPath, fileName),
                Path.Combine(Application.StartupPath, "Reports", fileName),
                Path.Combine(Application.StartupPath, "..", "..", "Reports", fileName),
                Path.Combine(Application.StartupPath, "..", "..", fileName)
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }

            return null;
        }

        private void SetupReportLayout()
        {
            try
            {
                // Fix margins and layout
                System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
                pg.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
                pg.PaperSize = new System.Drawing.Printing.PaperSize("A4", 827, 1169);

                reportViewer1.SetPageSettings(pg);
                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ZoomMode = ZoomMode.PageWidth;
            }
            catch
            {
                // Silent fail
            }
        }
    }
}