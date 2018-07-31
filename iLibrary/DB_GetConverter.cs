using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace iLibrary
{
    public class DB_GetConverter
    {
        public int GetInt32(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return -1; }
            else { return row.Field<int>(Name); }
        }
        public int GetInt16(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return -1; }
            else { return Convert.ToInt32(row.Field<Int16>(Name)); }
        }
        public bool GetBool(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return false; }
            else { return row.Field<bool>(Name); }
        }
        public String GetString(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return null; }
            else { return row.Field<String>(Name); }
        }
        public DateTime GetDateTime(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return new DateTime(0); }
            else { return row.Field<DateTime>(Name); }
        }
        public int GetByte(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return 0; }
            else { return row.Field<Byte>(Name); }
        }
    }
}
