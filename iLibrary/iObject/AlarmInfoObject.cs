using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iLibrary
{
    #region ALARM_INFO
    public class AlarmInfoObject
    {
        private int c_machine_id;
        private int c_model_alarm_id;
        private int c_alarm_count;
        private bool c_pass;
        private int c_ErrorNo;
        private String c_ErrorMessage;

        public AlarmInfoObject()
        {
            c_machine_id = -1;
            c_model_alarm_id = -1;
            c_alarm_count = -1;
            c_pass = false;
            c_ErrorNo = 0;
            c_ErrorMessage = "";
        }

        public AlarmInfoObject(int a_machine_id, int a_model_alarm_id, int a_count)
        {
            c_machine_id = a_machine_id;
            c_model_alarm_id = a_model_alarm_id;
            c_alarm_count = a_count;
            c_pass = false;
            c_ErrorNo = 0;
            c_ErrorMessage = "";
        }
        public int Machine_id
        {
            get { return c_machine_id; }
            set { c_machine_id = value; }
        }
        public int Model_alarm_id
        {
            get { return c_model_alarm_id; }
            set { c_model_alarm_id = value; }
        }
        public int Alarm_count
        {
            get { return c_alarm_count; }
            set { c_alarm_count = value; }
        }
        public Boolean Pass
        {
            get { return c_pass; }
            set { c_pass = value; }
        }
        public int ErrorNo
        {
            get { return c_ErrorNo; }
            set { c_ErrorNo = value; }
        }
        public String ErrorMessage
        {
            get { return c_ErrorMessage; }
            set { c_ErrorMessage = value; }
        }
    }
    #endregion ALARM_INFO
}