using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace iLibrary
{
    #region JIG_INFO
    public class EqpInfo : Info
    {
        private int c_Type_ID;
        private String c_Type_Name;
        private int c_Count_id;
        private String c_Count_name;    
        private String c_Value;
        private int c_Is_ValueExsist;
        private int c_CountUnit;
        private String c_CountWarning;
        private String c_CountLimit;
        private int c_Location;
        private String c_LocationType;
        private int c_Product_family_id;
        private bool c_Errorflag;
        private String c_ErrorMessage;

        public EqpInfo()
        {
            c_Type_ID = -1;
            c_Type_Name = null;
            c_Count_id = -1;
            c_Count_name = null;
            c_Value = null;
            c_Is_ValueExsist = -1;
            c_CountUnit = -1;
            c_CountWarning = null;
            c_CountLimit = null;
            c_Location = -1;
            c_LocationType = null;
            c_Product_family_id = -1;
            c_Errorflag = true;
            c_ErrorMessage = null;
        }
        public EqpInfo(DataRow EqpInfo) : base(EqpInfo)
        {
            try
            {
                c_Type_ID = DBCONVERT.GetInt32(EqpInfo,"categorie_id");
                c_Type_Name = DBCONVERT.GetString(EqpInfo,"name");
                c_Count_id = DBCONVERT.GetInt32(EqpInfo,"control_no");
                c_Count_name = DBCONVERT.GetString(EqpInfo,"control_name");
                c_Value = DBCONVERT.GetString(EqpInfo,"value");
                c_Is_ValueExsist = DBCONVERT.GetInt32(EqpInfo,"is_value_null");
                c_CountUnit = DBCONVERT.GetInt32(EqpInfo,"integration_unit");
                c_CountWarning = DBCONVERT.GetString(EqpInfo,"warn_value");
                c_CountLimit = DBCONVERT.GetString(EqpInfo,"alarm_value");
                c_Location = DBCONVERT.GetInt32(EqpInfo,"location_id");
                c_LocationType = DBCONVERT.GetString(EqpInfo,"location_name");
                c_Product_family_id = DBCONVERT.GetInt32(EqpInfo,"division_id");
                c_Errorflag = true;
                c_ErrorMessage = null;
            }
            catch (Exception e)
            {
                c_Type_ID = -1;
                c_Type_Name = null;
                c_Count_id = -1;
                c_Count_name = null;
                c_Value = null;
                c_Is_ValueExsist = -1;
                c_CountUnit = -1;
                c_CountWarning = null;
                c_CountLimit = null;
                c_Location = -1;
                c_LocationType = null;
                c_Product_family_id = -1;
                c_Errorflag = true;
                c_ErrorMessage = e.ToString();
            }
        }

        public int Type_ID
        {
            get { return c_Type_ID; }
            set { c_Type_ID = value; }
        }
        public string Type_Name
        {
            get { return c_Type_Name; }
            set { c_Type_Name = value; }
        }
        public int Count_id
        {
            get { return c_Count_id; }
            set { c_Count_id = value; }
        }
        public string Count_name
        {
            get { return c_Count_name; }
            set { c_Count_name = value; }
        }
        public string Value
        {
            get { return c_Value; }
            set { c_Value = value; }
        }
        public int Is_ValueExsist
        {
            get { return c_Is_ValueExsist; }
            set { c_Is_ValueExsist = value; }
        }
        public int CountUnit
        {
            get { return c_CountUnit; }
            set { c_CountUnit = value; }
        }
        public String CountWarning
        {
            get { return c_CountWarning; }
            set { c_CountWarning = value; }
        }
        public String CountLimit
        {
            get { return c_CountLimit; }
            set { c_CountLimit = value; }
        }
        public int Location_id
        {
            get { return c_Location; }
            set { c_Location = value; }
        }
        public string LocationType
        {
            get { return c_LocationType; }
            set { c_LocationType = value; }
        }
        public int Product_family_id
        {
            get { return c_Product_family_id; }
            set { c_Product_family_id = value; }
        }
        public bool Errorflag
        {
            get { return c_Errorflag; }
            set { c_Errorflag = value; }
        }
        public String ErrorMessage
        {
            get { return c_ErrorMessage; }
            set { c_ErrorMessage = value; }
        }

    }
    #endregion
}