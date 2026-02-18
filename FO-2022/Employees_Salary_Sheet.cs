using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Employees_Salary_Sheet : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public Employees_Salary_Sheet()
        {
            InitializeComponent();
        }

        private void Employees_Salary_Sheet_Load(object sender, EventArgs e)
        {
            try
            {
                // Set default dates (first day of current month to today)
                fromdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                todatepcker.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateReport()
        {
            try
            {
                // Validate date range
                if (fromdatepcker.Value > todatepcker.Value)
                {
                    MessageBox.Show("From date cannot be later than To date.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Get data for report
                DataTable dt = GetEmployeeSalaryData(
                    fromdatepcker.Checked ? fromdatepcker.Value : (DateTime?)null,
                    todatepcker.Checked ? todatepcker.Value : (DateTime?)null
                );

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No employee data found for the selected criteria.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Prepare report data sources
                Dictionary<string, DataTable> dataSources = new Dictionary<string, DataTable>();
                dataSources.Add("DataSet1", dt);

                // Prepare report parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    //{ "ReportTitle", "Employee Salary Sheet" },
                    //{ "GeneratedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                // Add date range parameters if filters are applied
                if (fromdatepcker.Checked)
                {
                    //parameters.Add("FromDate", fromdatepcker.Value.ToString("dd/MM/yyyy"));
                }

                if (todatepcker.Checked)
                {
                    //parameters.Add("ToDate", todatepcker.Value.ToString("dd/MM/yyyy"));
                }

                // Show report viewer (universal one)
                Report_viewer reportViewer = new Report_viewer();
                reportViewer.ShowReport("Employee_Salary_rv", dataSources, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetEmployeeSalaryData(DateTime? fromDate, DateTime? toDate)
        {
            DataTable dt = new DataTable();

            // Define columns matching RDLC dataset
            dt.Columns.Add("EmployeeID", typeof(string));
            dt.Columns.Add("EmployeeName", typeof(string));
            dt.Columns.Add("FatherName", typeof(string));
            dt.Columns.Add("JobType", typeof(string));
            dt.Columns.Add("Designation", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("JoiningDate", typeof(DateTime));
            dt.Columns.Add("SalaryWages", typeof(decimal));
            dt.Columns.Add("NICNo", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("Phone", typeof(string));

            try
            {
                string query = @"
                    SELECT 
                        pe.EmployeeID,
                        pe.EmployeeName,
                        pe.FatherName,
                        pe.JobType,
                        pd.DesignationName as Designation,
                        '' as Department, -- Add if you have Department table
                        pe.JoiningDate,
                        pe.SalaryWages,
                        ISNULL(pe.NICNo, '') as NICNo,
                        pe.Address,
                        pe.City,
                        ISNULL(pe.Phone, '') as Phone
                    FROM PayRolEmployees pe
                    LEFT JOIN PayRolDesignations pd ON pe.DesignationID = pd.DesignationID
                    WHERE pe.IsDisabled = 0"; // Active employees only

                // Add date filters if provided
                if (fromDate.HasValue)
                {
                    query += " AND pe.JoiningDate >= @FromDate";
                }

                if (toDate.HasValue)
                {
                    query += " AND pe.JoiningDate <= @ToDate";
                }

                query += " ORDER BY pe.EmployeeName";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (fromDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@FromDate", fromDate.Value);
                        }

                        if (toDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ToDate", toDate.Value);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = dt.NewRow();
                                row["EmployeeID"] = reader["EmployeeID"];
                                row["EmployeeName"] = reader["EmployeeName"];
                                row["FatherName"] = reader["FatherName"];
                                row["JobType"] = reader["JobType"];
                                row["Designation"] = reader["Designation"];
                                row["Department"] = reader["Department"];
                                row["JoiningDate"] = reader["JoiningDate"];
                                row["SalaryWages"] = reader["SalaryWages"];
                                row["NICNo"] = reader["NICNo"];
                                row["Address"] = reader["Address"];
                                row["City"] = reader["City"];
                                row["Phone"] = reader["Phone"];
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching employee data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void previewbtn_Click(object sender, EventArgs e) => GenerateReport();
        private void printbtn_Click(object sender, EventArgs e) => GenerateReport();

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation
            if (fromdatepcker.Checked && todatepcker.Checked && fromdatepcker.Value > todatepcker.Value)
            {
                MessageBox.Show("From date cannot be later than To date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                fromdatepcker.Value = todatepcker.Value;
            }
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Validation
            if (fromdatepcker.Checked && todatepcker.Checked && todatepcker.Value < fromdatepcker.Value)
            {
                MessageBox.Show("To date cannot be earlier than From date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                todatepcker.Value = fromdatepcker.Value;
            }
        }
    }
}