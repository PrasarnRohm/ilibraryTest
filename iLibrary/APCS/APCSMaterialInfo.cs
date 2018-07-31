using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLibrary
{
    public class APCSMaterialInfo : Info
    {
        private String c_Type;
        private String c_ProductionName;
        private String c_QRCode;
        private bool c_Pass;
        private DateTime c_InputTime;
        private DateTime c_ExpiredTime;
        private int c_UseCount;
        private int c_ExpiredCount;
        private Boolean c_IsChanged;
        private float c_Pcs;
        private Boolean c_IsDummy;
        private float c_TotalCount;
        private int c_ErrorNo;
        private String c_ErrorMessage;
        private String c_Message;
        

        public APCSMaterialInfo() : base()
        {
            c_Type = "";
            c_ProductionName = "";
            c_QRCode = "";
            c_Pass = false;
            c_InputTime = new DateTime(0);
            c_ExpiredTime = new DateTime(0);
            c_UseCount = 0;
            c_ExpiredCount = 0;
            c_Pcs = 0;
            c_IsDummy = false;
            c_TotalCount = 0;
            c_ErrorNo = -1;
            c_ErrorMessage = "";
            c_Message = "";
            c_IsChanged = false;
        }

        public APCSMaterialInfo(APCSMaterialInfo clone)
        {
            this.Set_Id = clone.Id;
            this.Name = clone.Name;
            c_Type = clone.Type;
            c_ProductionName = clone.ProductionName;
            c_QRCode = clone.QRCode;
            c_Pass = clone.Pass;
            c_InputTime = clone.InputTime;
            c_ExpiredTime = clone.ExpiredTime;
            c_UseCount = clone.UseCount;
            c_ExpiredCount = clone.ExpiredCount;
            c_IsChanged = clone.IsChanged;
            c_Pcs = clone.Pcs;
            c_IsDummy = clone.IsDummy;
            c_TotalCount = clone.TotalCount;
            c_ErrorNo = clone.ErrorNo;
            c_ErrorMessage = clone.ErrorMessage;
            c_Message = clone.Message;
        }

        public APCSMaterialInfo(String type, String ProductName, String QRCode, float o_Pcs = 0, bool o_IsDummy = false, float o_TotalCount = 0) : base()
        {
            c_Type = type;
            c_ProductionName = ProductName;
            c_QRCode = QRCode;
            c_Pass = false;
            c_InputTime = new DateTime(0);
            c_ExpiredTime = new DateTime(0);
            c_UseCount = 0;
            c_ExpiredCount = 0;
            c_IsChanged = false;
            c_Pcs = o_Pcs;
            c_IsDummy = o_IsDummy;
            c_TotalCount = o_TotalCount;
            c_ErrorNo = -1;
            c_ErrorMessage = "";
            c_Message = "";
        }

        public String Type
        {
            get { return c_Type; }
            set { c_Type = value; }
        }
        public String ProductionName
        {
            get { return c_ProductionName; }
            set { c_ProductionName = value; }
        }
        public String QRCode
        {
            get { return c_QRCode; }
            set { c_QRCode = value; }
        }
        public Boolean IsChanged
        {
            get { return c_IsChanged; }
            set { c_IsChanged = value; }
        }
        public Boolean Pass
        {
            get { return c_Pass; }
            set { c_Pass = value; }
        }
        public DateTime ExpiredTime
        {
            get { return c_ExpiredTime; }
            set { c_ExpiredTime = value; }
        }
        public DateTime InputTime
        {
            get { return c_InputTime; }
            set { c_InputTime = value; }
        }
        public int ExpiredCount
        {
            get { return c_ExpiredCount; }
            set { c_ExpiredCount = value; }
        }
        public int UseCount
        {
            get { return c_UseCount; }
            set { c_UseCount = value; }
        }
        public float Pcs
        { 
            get { return c_Pcs; }
            set { c_Pcs = value; }
        }
        public Boolean IsDummy
        {
            get { return c_IsDummy; }
            set { c_IsDummy = value; }
        }
        public float TotalCount
        {
            get { return c_TotalCount; }
            set { c_TotalCount = value; }
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
        public String Message
        {
            get { return c_Message; }
            set { c_Message = value; }
        }
    }
}
