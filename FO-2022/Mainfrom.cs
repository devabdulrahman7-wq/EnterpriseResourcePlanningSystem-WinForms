using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Mainfrom : Form
    {
        public Mainfrom()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void customerdCategrToolStripMenuItem_Click(object sender, EventArgs e)
        {
                
        }

        private void assignCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolStripSplitButton2_ButtonClick(object sender, EventArgs e)
        {

        }

        private void postShortToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Mainfrom_Load(object sender, EventArgs e)
        {
        
        }
        private void Mainfrom_Paint(object sender, PaintEventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 15; // roundness
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(panel1.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(panel1.Width - radius, panel1.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, panel1.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            panel1.Region = new Region(path);
        }




        private void button1_Click(object sender, EventArgs e)
        {
            this.WindowState = (this.WindowState == FormWindowState.Maximized)
                   ? FormWindowState.Normal
                   : FormWindowState.Maximized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
