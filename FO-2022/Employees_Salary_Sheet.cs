using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void closebtn_Click(object sender, EventArgs e)
        {
            Mainform1 mf = new Mainform1();
            mf.Show();
            this.Hide();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            OpenReportViewer();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            OpenReportViewer();
        }

        private void OpenReportViewer()
        {
            try
            {
                DataTable dt = GetEmployeeSalaryData();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No employee data found.", "Information");
                    return;
                }

                Reports.Employee_Salary_report reportForm = new Reports.Employee_Salary_report();
                reportForm.EmployeeData = dt;
                reportForm.ReportTitle = $"Employee Salary Sheet ({DateTime.Now:dd-MMM-yyyy})";
                reportForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
        }

        private DataTable GetEmployeeSalaryData()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                        EmployeeID,
                                        EmployeeName,
                                        FatherName,
                                        JobType,
                                        SalaryWages
                                     FROM PayRolEmployees
                                     WHERE IsDisabled = 0  -- Active employees only
                                     ORDER BY EmployeeID";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add date filters if checked
                    if (fromdatepcker.Checked)
                    {
                        query = @"SELECT 
                                    EmployeeID,
                                    EmployeeName,
                                    FatherName,
                                    JobType,
                                    SalaryWages
                                  FROM PayRolEmployees
                                  WHERE IsDisabled = 0 
                                  AND JoiningDate >= @FromDate
                                  AND (@ToDate IS NULL OR JoiningDate <= @ToDate)
                                  ORDER BY EmployeeID";

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@FromDate", fromdatepcker.Value);

                        if (todatepcker.Checked)
                            cmd.Parameters.AddWithValue("@ToDate", todatepcker.Value);
                        else
                            cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Database error: " + ex.Message);
            }

            return dt;
        }

        private void Employees_Salary_Sheet_Load(object sender, EventArgs e)
        {
            // Set default dates
            fromdatepcker.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            todatepcker.Value = DateTime.Now;
        }

        private void fromdatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }

        private void todatepcker_ValueChanged(object sender, EventArgs e)
        {
            // Optional: Add validation if needed
        }
    }
}