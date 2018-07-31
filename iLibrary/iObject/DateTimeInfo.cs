using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLibrary
{
    public class DateTimeInfo
    {
        private int c_DayId;
        private DateTime c_Datetime;
        private bool c_Errorflag;
        private String c_Errormessage;

        public DateTimeInfo()
        {
            c_DayId = -1;
            c_Datetime = new DateTime(0);
            c_Errorflag = true;
            c_Errormessage = null;
    }

        public int DayId
        {
            get { return c_DayId; }
            set { c_DayId = value; }
        }
        public DateTime Datetime
        {
            get { return c_Datetime; }
            set { c_Datetime = value; }
        }
        public bool Errorflag
        {
            get { return c_Errorflag; }
            set { c_Errorflag = value; }
        }
        public String Errormessage
        {
            get { return c_Errormessage; }
            set { c_Errormessage = value; }
        }
    }
}
