using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLibrary
{
    /// <summary>
    /// This class is used when application don't have app.config file.
    /// </summary>
    public class AppConfig
    {
        private static Dictionary<string, string> m_ConnectionString;
        private static Dictionary<string, string> m_AppSetting;
        static AppConfig()
        {
            m_ConnectionString = new Dictionary<string, string>();
            m_AppSetting = new Dictionary<string, string>();
        }

        public static string GetConnectionString(string key)
        {
            if (m_ConnectionString.ContainsKey(key))
            {
                return m_ConnectionString[key];
            }
            else
            {
                return null;
            }
        }
        public static void AddConnectionString(string name, string connectionString)
        {
            if (name == null || name == "" || connectionString == null || connectionString == "")
            {
                throw new Exception("ConnectionString's name and ConnectionString can't be null or \"\".When adding new ConnectionString");
            }
            m_ConnectionString.Add(name, connectionString);
        }
        public static void RemoveConnectionString(string name)
        {
            if (name == null || name == "")
            {
                throw new Exception("ConnectionString's name can't be null or \"\".When removing ConnectionString");
            }
            m_ConnectionString.Remove(name);
        }
        public static string GetAppSetting(string key)
        {
            if (m_AppSetting.ContainsKey(key))
            {
                return m_AppSetting[key];
            }
            else
            {
                return null;
            }
        }
        public static void AddAppSetting(string name, string value)
        {
            if (name == null || name == "" || value == null || value == "")
            {
                throw new Exception("AppSetting's name and value can't be null or \"\".When adding new AppSetting");
            }
            m_AppSetting.Add(name, value);
        }
        public static void RemoveAppSetting(string name)
        {
            if (name == null || name == "")
            {
                throw new Exception("AppSetting's name can't be null or \"\".When removing AppSetting");
            }
            m_AppSetting.Remove(name);
        }
        private AppConfig() { }
    }
}
