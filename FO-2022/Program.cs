using Delloweb.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!LicenseEngine.IsLicenseValid())
            {
                new ActivationForm().ShowDialog();
                if (!LicenseEngine.IsLicenseValid())
                    Application.Exit();
            }
            else
            {

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form());
            }
        }
    }
    
}
