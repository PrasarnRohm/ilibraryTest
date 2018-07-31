using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace iLibrary
{
    internal class AppSettingHelper
    {
        private AppSettingHelper() { }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/exception

        /// <summary>
        /// get value from appSettings section in app.config file
        /// </summary>
        /// <param name="key">key name</param>
        /// <returns>value</returns>
        /// <exception cref="System.Exception">throw when Key was not found</exception>
        /// <example>see example <see cref="GetAppSettingsValue"/>
        /// class TestClass {
        ///     static int Main()
        ///     {
        ///         string val = AppSettingHelper.GetAppSettingsValue("X-Key");
        ///     }
        /// }
        /// </example>
        internal static string GetAppSettingsValue(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? AppConfig.GetAppSetting(key) ?? throw new Exception("Application Setting Key was not found [Key=" + key + "]");
            //string val = ConfigurationManager.AppSettings[key];
            //return val ?? throw new Exception("Application Setting Key was not found [Key=" + key + "]");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string GetConnectionStringValue(string key)
        {            
            if (ConfigurationManager.ConnectionStrings[key] == null)
            {
                return AppConfig.GetConnectionString(key) ?? throw new Exception("ConnectionString Setting Key was not found [Key=" + key + "]");
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[key].ConnectionString;
            }
            //string val = ConfigurationManager.ConnectionStrings[key].ConnectionString;
            //return val ?? throw new Exception("ConectionString Setting Key was not found [Key=" + key + "]");
        }
    }
}
