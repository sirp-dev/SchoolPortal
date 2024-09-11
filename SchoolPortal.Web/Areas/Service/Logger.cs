using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Areas.Service
{
    public class Logger
    {
        private const string IDENTITY = @"HKEY_LOCAL_MACHINE\Ident";
        private const string NAME = "Name";


        public static void log(string name, string path)
        {
            string DeviceName = (string)Registry.GetValue(IDENTITY, NAME, "NAME");
            string pcName = System.Environment.MachineName;
            var fileContents = HttpContext.Current.Server.MapPath("~/App_Data/Logger.txt");
            if (File.Exists(fileContents))
            {
                using (TextWriter sw = new StreamWriter(fileContents, true))
                {
                    sw.Write(Environment.NewLine + "**username**"+ name + " *****path** " + path + " *****date** " + DateTime.UtcNow + " ****device name** " + DeviceName + " ****pc name*** " + pcName + "<br/>");
                    sw.Close();
                }
            }
        }
    }
}