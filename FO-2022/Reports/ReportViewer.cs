using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace FO_2022.Reports
{
    public partial class ReportViewer : Form
    {
        // Store report data passed from calling form
        private string _reportName;
        private Dictionary<string, DataTable> _dataSources;

        public ReportViewer()
        {
            InitializeComponent();
        }

        // 📌 MAIN METHOD: CALL THIS FROM ANY FORM TO SHOW REPORT
        public void ShowReport(string reportName, Dictionary<string, DataTable> dataSources)
        {
            _reportName = reportName;
            _dataSources = dataSources;
            this.ShowDialog();
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            try
            {
                // 📌 STEP 1: Set RDLC file path
                string rdlcPath = GetReportPath(_reportName);
                reportViewer1.LocalReport.ReportPath = rdlcPath;

                // 📌 STEP 2: Clear old data
                reportViewer1.LocalReport.DataSources.Clear();

                // 📌 STEP 3: Add all DataTables as DataSources
                foreach (var dataSource in _dataSources)
                {
                    // DataSource name must match RDLC DataSet name
                    ReportDataSource rds = new ReportDataSource(
                        dataSource.Key,  // e.g., "dsSalesman" (RDLC Dataset name)
                        dataSource.Value // DataTable
                    );
                    reportViewer1.LocalReport.DataSources.Add(rds);
                }

                // 📌 STEP 4: Refresh to display report
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report: {ex.Message}\n\n" +
                    $"Report: {_reportName}\n" +
                    $"Make sure RDLC file exists and DataSet names match.",
                    "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        // 📌 HELPER: Get RDLC file path
        private string GetReportPath(string reportName)
        {
            // 📌 CHANGE HERE: Add your RDLC file names
            // Format: { "ReportKey", "FileName.rdlc" }
            var reportFiles = new Dictionary<string, string>
            {
                // EXAMPLE REPORTS - ADD ALL 90 HERE
                { "SalesmanSectorList", "SalesmanSectorList.rdlc" },
                { "SalesReport", "SalesReport.rdlc" },
                { "PurchaseReport", "PurchaseReport.rdlc" },
                { "StockReport", "StockReport.rdlc" },
                // ... Add all 90 report names
            };

            if (reportFiles.ContainsKey(reportName))
            {
                return $@"{Application.StartupPath}\Reports\{reportFiles[reportName]}";
            }
            else
            {
                throw new Exception($"Report '{reportName}' not found in configuration.");
            }
        }

        // 📌 BUTTON: Print Report
        private void btnPrint_Click(object sender, EventArgs e)
        {
            reportViewer1.PrintDialog();
        }

        // 📌 BUTTON: Export to PDF
        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"{_reportName}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (save.ShowDialog() == DialogResult.OK)
            {
                byte[] pdfBytes = reportViewer1.LocalReport.Render("PDF");
                System.IO.File.WriteAllBytes(save.FileName, pdfBytes);
                MessageBox.Show("PDF exported successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 📌 BUTTON: Close
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}