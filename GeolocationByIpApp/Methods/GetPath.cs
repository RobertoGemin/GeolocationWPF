using System;
using System.Configuration;
using System.IO;

namespace GeolocationApp.Methods
{
    public class GetPath
    {
        public static string GetFullPath()
        {
            bool useBasePath;
            var basePath = ConfigurationManager.AppSettings["BasePath"];
            var specificPath = ConfigurationManager.AppSettings["SpecificPath"] ??
                               Path.Combine("GeolocationApp", "data");
            string fullPath;

            if (bool.TryParse(ConfigurationManager.AppSettings["UseBasePath"], out useBasePath))
            {
                if (useBasePath && !string.IsNullOrEmpty(basePath))
                    fullPath = Path.Combine(basePath, specificPath);
                else
                    fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        specificPath);
            }
            else
            {
                fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), specificPath);
            }

            return fullPath;
        }
    }
}