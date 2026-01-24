using System;
using System.IO;
using System.Text;
using System.Management;
using System.Security.Cryptography;

namespace Delloweb.Licensing
{
    public static class LicenseEngine
    {
        public const string APP_ID = "DELOWEB_ERP";
        public const string SECRET = "DWB-ERP-PRIVATE-SECRET";
        private static string LicensePath = @"C:\ProgramData\delloweb_erp.lic";

        public static string GetMachineID()
        {
            string id = "";
            var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
                id = obj["ProcessorId"].ToString();
            return id;
        }

        public static string CreateHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").Substring(0, 12);
            }
        }

        public static string Decode(string key)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(key));
        }

        public static bool ValidateLicense(string key, out int licensedYear)
        {
            licensedYear = 0;
            try
            {
                string decoded = Decode(key);
                var parts = decoded.Split('|');
                if (parts.Length != 4) return false;

                string app = parts[0];
                int year = int.Parse(parts[1]);
                string machine = parts[2];
                string sign = parts[3];

                if (app != APP_ID) return false;
                if (machine != GetMachineID()) return false;

                string raw = $"{app}|{year}|{machine}|{SECRET}";
                string expected = CreateHash(raw);

                if (expected != sign) return false;

                licensedYear = year;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SaveLicense(int year)
        {
            File.WriteAllText(LicensePath, Encrypt(year.ToString()));
        }

        public static int LoadLicense()
        {
            if (!File.Exists(LicensePath)) return 0;
            return int.Parse(Decrypt(File.ReadAllText(LicensePath)));
        }

        public static bool IsLicenseValid()
        {
            int year = LoadLicense();
            if (year == 0) return false;

            DateTime now = DateTime.Now;
            if (now < new DateTime(year, 1, 1)) return false;
            if (now >= new DateTime(year + 1, 1, 1)) return false;

            return true;
        }

        private static string Encrypt(string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        private static string Decrypt(string data)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(data));
        }
    }
}
