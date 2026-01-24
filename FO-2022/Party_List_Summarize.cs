using System;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Party_List_Summarize : Form
    {
        public Party_List_Summarize()
        {
            InitializeComponent();
        }

        private void previewbtn_Click(object sender, EventArgs e)
        {
            Reports.Part_List_rv reportViewerForm = new Reports.Part_List_rv();
            reportViewerForm.Show();
        }

        private void printbtn_Click(object sender, EventArgs e)
        {
            Reports.Part_List_rv reportViewerForm = new Reports.Part_List_rv();
            reportViewerForm.PrintReport();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Party_List_Summarize_Load(object sender, EventArgs e)
        {
            // Nothing needed here
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}