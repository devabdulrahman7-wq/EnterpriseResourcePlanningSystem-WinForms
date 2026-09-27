using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace FO_2022.Reports
{
    public partial class FrmReportViewer : Form
    {
        private string _embeddedReportResource;
        private List<ReportDataSource> _dataSources;
        private List<ReportParameter> _parameters;

        public FrmReportViewer()
        {
            InitializeComponent();
        }

        public void LoadReport(
            string embeddedReportResource,
            IEnumerable<ReportDataSource> dataSources,
            IEnumerable<ReportParameter> parameters)
        {
            _embeddedReportResource = embeddedReportResource;
            _dataSources = dataSources == null ? new List<ReportDataSource>() : dataSources.ToList();
            _parameters = parameters == null ? new List<ReportParameter>() : parameters.ToList();
        }

        private void FrmReportViewer_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_embeddedReportResource))
            {
                BindReport();
            }
        }

        private void BindReport()
        {
            reportViewer1.Reset();
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.ReportEmbeddedResource = _embeddedReportResource;
            reportViewer1.LocalReport.DataSources.Clear();

            foreach (ReportDataSource dataSource in _dataSources)
            {
                reportViewer1.LocalReport.DataSources.Add(dataSource);
            }

            if (_parameters.Count > 0)
            {
                reportViewer1.LocalReport.SetParameters(_parameters);
            }

            ConfigureViewer();
            reportViewer1.RefreshReport();
        }

        private void ConfigureViewer()
        {
            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer1.ZoomMode = ZoomMode.PageWidth;
            reportViewer1.ShowBackButton = false;
            reportViewer1.ShowDocumentMapButton = false;
            reportViewer1.ShowFindControls = false;
            reportViewer1.ShowPageNavigationControls = true;
            reportViewer1.ShowParameterPrompts = false;
            reportViewer1.ShowStopButton = false;

            PageSettings pageSettings = new PageSettings();
            pageSettings.Margins = new Margins(25, 25, 25, 25);
            reportViewer1.SetPageSettings(pageSettings);
        }
    }
}
