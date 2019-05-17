using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FSCAppPages.Layouts.FSCAppPages
{
    public class GetConfig
    {
        //private string siteUrl = ConfigurationManager.AppSettings["SiteURL"];
        // <summary>
        /// <param name="ConnectionName">todo: describe ConnectionName parameter on GetConnectionString</param>
        public static string GetConnectionString(string ConnectionName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string CurrentPath = path + @"\Common Files\Microsoft Shared\web server extensions\15\TEMPLATE\LAYOUTS\web.config";
            XmlDocument doc = new XmlDocument();
            doc.Load(CurrentPath);
            XmlNode node = doc.SelectSingleNode("/configuration/connectionStrings/add[@name=\"" + ConnectionName + "\"]");
            XmlElement element = (XmlElement)node;
            string _connectionString = element.GetAttribute("connectionString");
            return _connectionString;
        }
        // <summary>
        /// <param name="AppSettingsKey">todo: describe AppSettingsKey parameter on GetAppSettings</param>

        public static string GetAppSetting(string AppSettingsKey)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string CurrentPath = path + @"\Common Files\Microsoft Shared\Web Server Extensions\15\TEMPLATE\LAYOUTS\web.config";
            XmlDocument doc = new XmlDocument();
            doc.Load(CurrentPath);
            XmlNode node = doc.SelectSingleNode("/configuration/appSettings/add[@key=\"" + AppSettingsKey + "\"]");
            XmlElement element = (XmlElement)node;
            string _connectionString = element.GetAttribute("value");
            return _connectionString;
        }



    }
}
