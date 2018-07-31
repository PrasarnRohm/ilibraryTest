using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace iLibrary
{
    #region LOT_DB_INFO
    public class LotDBInfo : LotInfo
    {
        private int c_Process_state;
        private int c_Ticket_id;
        private int c_Step_no;
        private int c_Package_id;
        private int c_Product_family_id;
        private int c_Quality_state;
        private int c_Is_special_flow;
        private int c_Location_id;
        private int c_Pj_id;
        private int c_Qty_in;
        private int c_Qty_pass;
        private int c_Qty_fail;
        private int c_Wip_state;
        private int c_First_inspection;
        private int c_Final_inspection;
        private bool c_Errorflag;
        private String c_ErrorMessage;

        internal LotDBInfo() :base()
        {
            c_Process_state = -1;
            c_Ticket_id = 1;
            c_Step_no = -1;
            c_Package_id = -1;
            c_Product_family_id = -1;
            c_Quality_state = -1;
            c_Is_special_flow = -1;
            c_Location_id = -1;
            c_Pj_id = -1;
            c_Qty_in = -1;
            c_Qty_pass = -1;
            c_Qty_fail = -1;
            c_First_inspection = -1;
            c_Final_inspection = -1;
            c_Wip_state = -1;
            c_Errorflag = true;
            c_ErrorMessage = null;
        }
        internal LotDBInfo(DataRow LotInfo) : base(LotInfo.Field<int>("id"), LotInfo.Field<string>("lot_no"), LotInfo.Field<byte>("is_automotive"), LotInfo.Field<int>("job_id"))
        {
            if(LotInfo == null)
            {
                throw new Exception("Data is Null");
            }
            c_Process_state = DBCONVERT.GetByte(LotInfo,"process_state");
            c_Ticket_id = DBCONVERT.GetInt32(LotInfo,"device_slip_id");
            c_Step_no = DBCONVERT.GetInt32(LotInfo,"step_no");
            c_Package_id = DBCONVERT.GetInt32(LotInfo,"act_package_id");
            c_Product_family_id = DBCONVERT.GetInt32(LotInfo,"product_family_id");
            c_Quality_state = DBCONVERT.GetByte(LotInfo,"quality_state");
            c_Is_special_flow = DBCONVERT.GetByte(LotInfo, "is_special_flow");
            c_Location_id = DBCONVERT.GetInt32(LotInfo,"location_id");
            c_Pj_id = DBCONVERT.GetInt32(LotInfo,"process_job_id");
            c_Qty_in = DBCONVERT.GetInt32(LotInfo,"qty_in");
            c_Qty_pass = DBCONVERT.GetInt32(LotInfo,"qty_pass");
            c_Qty_fail = DBCONVERT.GetInt32(LotInfo,"qty_fail");
            c_Wip_state = DBCONVERT.GetByte(LotInfo, "wip_state");
            c_First_inspection = DBCONVERT.GetByte(LotInfo, "first_ins_state");
            c_Final_inspection = DBCONVERT.GetByte(LotInfo, "final_ins_state");

            c_ErrorMessage = null;
        }

        public int Process_state
        {
            get { return c_Process_state; }
            set { c_Process_state = value; }
        }
        public int Ticket_id
        {
            get { return c_Ticket_id; }
            set { c_Ticket_id = value; }
        }
        public int Step_no
        {
            get { return c_Step_no; }
            set { c_Step_no = value; }
        }
        public int Package_id
        {
            get { return c_Package_id; }
            set { c_Package_id = value; }
        }
        public int Pj_id
        {
            get { return c_Pj_id; }
            set { c_Pj_id = value; }
        }
        public int Product_family_id
        {
            get { return c_Product_family_id; }
            set { c_Product_family_id = value; }
        }
        public int Quality_state
        {
            get { return c_Quality_state; }
            set { c_Quality_state = value; }
        }
   
        public int Wip_state
        {
            get { return c_Wip_state; }
            set { c_Wip_state = value; }
        }

        public int Is_special_flow
        {
            get { return c_Is_special_flow; }
            set { c_Is_special_flow = value; }
        }
        public int Location_id
        {
            get { return c_Location_id; }
            set { c_Location_id = value; }
        }
        public int Qty_in
        {
            get { return c_Qty_in; }
            set { c_Qty_in = value; }
        }
        public int Qty_pass
        {
            get { return c_Qty_pass; }
            set { c_Qty_pass = value; }
        }
        public int Qty_fail
        {
            get { return c_Qty_fail; }
            set { c_Qty_fail = value; }
        }
        public int First_inspection
        {
            get { return c_First_inspection; }
            set { c_First_inspection = value; }
        }
        public int Final_inspection
        {
            get { return c_Final_inspection; }
            set { c_Final_inspection = value; }
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
    #region FOR_CHECKFLOW
    public class LotProcessCheckInfo : LotDBInfo
    {
        private String c_Package_name;
        private int c_Package_IsEnable;
        private int c_Package_AutoMotive;
        private int c_MachineId;
        private String c_MachineName;
        private int c_MachineRunState;
        private int c_MachineOnlineState;
        private int c_MachineQCState;
        private int c_Machine_AutoMotive;

        public LotProcessCheckInfo() : base()
        {
            Process_state = -1;
            Ticket_id = 1;
            Step_no = -1;
            Package_id = -1;
            Product_family_id = -1;
            Quality_state = -1;
            Wip_state = -1;
            Is_special_flow = -1;
            Location_id = -1;
            Pj_id = -1;
            Qty_in = -1;
            Qty_pass = -1;
            Qty_fail = -1;
            c_Package_name = null;
            c_Package_IsEnable = 0;
            c_Package_AutoMotive = 0;
            c_MachineId = -1;
            c_MachineName = null;
            c_MachineRunState = -1;
            c_MachineOnlineState = -1;
            c_MachineQCState = -1;
            c_Machine_AutoMotive = 0;
            Errorflag = true;
            ErrorMessage = null;
        }
        public LotProcessCheckInfo(DataRow LotInfo) : base(LotInfo)
        {
            if(LotInfo == null)
            {
                throw new Exception("Data is Null");
            }
            Process_state = DBCONVERT.GetByte(LotInfo,"process_state");
            Ticket_id = DBCONVERT.GetInt32(LotInfo,"device_slip_id");
            Step_no = DBCONVERT.GetInt32(LotInfo,"step_no");
            Package_id = DBCONVERT.GetInt32(LotInfo,"package_id");
            Product_family_id = DBCONVERT.GetInt32(LotInfo,"product_family_id");
            Quality_state = DBCONVERT.GetByte(LotInfo,"quality_state");
            Wip_state = DBCONVERT.GetByte(LotInfo, "wip_state");
            Is_special_flow = DBCONVERT.GetByte(LotInfo,"is_special_flow");
            Location_id = DBCONVERT.GetInt32(LotInfo,"location_id");
            Pj_id = DBCONVERT.GetInt32(LotInfo,"process_job_id");
            Qty_in = DBCONVERT.GetInt32(LotInfo,"qty_in");
            Qty_pass = DBCONVERT.GetInt32(LotInfo,"qty_pass");
            Qty_fail = DBCONVERT.GetInt32(LotInfo,"qty_fail");
            c_Package_name = DBCONVERT.GetString(LotInfo,"name");
            c_Package_IsEnable = DBCONVERT.GetByte(LotInfo,"is_Enabled");
            c_Package_AutoMotive = DBCONVERT.GetByte(LotInfo,"is_automotive");
            c_MachineId = DBCONVERT.GetInt32(LotInfo,"machine_id");
            c_MachineName = DBCONVERT.GetString(LotInfo,"machine_name");
            c_MachineRunState = DBCONVERT.GetByte(LotInfo,"run_state");
            c_MachineOnlineState = DBCONVERT.GetByte(LotInfo,"online_state");
            c_MachineQCState = DBCONVERT.GetByte(LotInfo,"qc_state");
            c_Machine_AutoMotive = DBCONVERT.GetByte(LotInfo,"machine_is_automotive");
            Errorflag = false;
            ErrorMessage = null;
            
        }
        
        public string Package_name
        {
            get { return c_Package_name; }
            set { c_Package_name = value; }
        }
        public int Package_IsEnable
        {
            get { return c_Package_IsEnable; }
            set { c_Package_IsEnable = value; }
        }
        public int Package_AutoMotive
        {
            get { return c_Package_AutoMotive; }
            set { c_Package_AutoMotive = value; }
        }
        public int MachineId
        {
            get { return c_MachineId; }
            set { c_MachineId = value; }
        }
        public string MachineName
        {
            get { return c_MachineName; }
            set { c_MachineName = value; }
        }
        public int MachineRunState
        {
            get { return c_MachineRunState; }
            set { c_MachineRunState = value; }
        }
        public int MachineOnlineState
        {
            get { return c_MachineOnlineState; }
            set { c_MachineOnlineState = value; }
        }
        public int MachineQCState
        {
            get { return c_MachineQCState; }
            set { c_MachineQCState = value; }
        }
        public int Machine_AutoMotive
        {
            get { return c_Machine_AutoMotive; }
            set { c_Machine_AutoMotive = value; }
        }
    }
    #endregion


    #region SPLOT_DB_INFO
    public class SpLotDBInfo : LotInfo
    {
        private int c_Specialflow_id;
        private int c_Pocess_state;
        private int c_Step_no;
        private int c_Quality_state;
        private int c_Qty_in;
        private int c_Qty_pass;
        private int c_Qty_fail;
        private bool c_Errorflag;
        private String c_ErrorMessage;

        public SpLotDBInfo() : base()
        {

            c_Specialflow_id = -1;
            c_Pocess_state = -1;
            c_Step_no = -1;
            c_Quality_state = -1;
            c_Qty_in = -1;
            c_Qty_pass = -1;
            c_Qty_fail = -1;
            c_Errorflag = true;
            c_ErrorMessage = null;
        }
        public SpLotDBInfo(DataRow LotInfo) : base(LotInfo.Field<int>("id"), LotInfo.Field<string>("lot_no"))
        {
            if(LotInfo == null)
            {
                throw new Exception("Data is Null");
            }
            
            c_Specialflow_id = DBCONVERT.GetInt32(LotInfo,"special_flow_id");
            c_Pocess_state = DBCONVERT.GetByte(LotInfo,"process_state");
            c_Step_no = DBCONVERT.GetInt32(LotInfo,"step_no");
            c_Quality_state = DBCONVERT.GetByte(LotInfo,"quality_state");
            c_Qty_in = DBCONVERT.GetInt32(LotInfo,"qty_in");
            c_Qty_pass = DBCONVERT.GetInt32(LotInfo,"qty_pass");
            c_Qty_fail = DBCONVERT.GetInt32(LotInfo,"qty_fail");
            c_Errorflag = false;
            c_ErrorMessage = null;
           
        }

        public int Pocess_state
        {
            get { return c_Pocess_state; }
            set { c_Pocess_state = value; }
        }
        public int Specialflow_id
        {
            get { return c_Specialflow_id; }
            set { c_Specialflow_id = value; }
        }
        public int Step_no
        {
            get { return c_Step_no; }
            set { c_Step_no = value; }
        }
        public int Quality_state
        {
            get { return c_Quality_state; }
            set { c_Quality_state = value; }
        }
        public int Qty_in
        {
            get { return c_Qty_in; }
            set { c_Qty_in = value; }
        }
        public int Qty_pass
        {
            get { return c_Qty_pass; }
            set { c_Qty_pass = value; }
        }
        public int Qty_fail
        {
            get { return c_Qty_fail; }
            set { c_Qty_fail = value; }
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
    #region LOT_UPDATE_INFO
    public class LotUpdateInfo : LotInfo
    {
        private bool c_IsOk;
        private int c_ErrorNo;
        private String c_ErrorMessage;
        private int c_Step_No;
        private int c_Ticket_ID;
        private DateTime c_Start_Time;
        private int c_Machine_id;
        private int c_Input_Qty;
        private int c_User;
        private string c_Recipe1;
        private string c_Recipe2;

        public LotUpdateInfo() : base()
        {
            c_IsOk = false;
            c_ErrorNo = -1;
            c_ErrorMessage = null;
            c_Step_No = -1;
            c_Ticket_ID = -1;
            c_Start_Time = new DateTime(0);
            c_Machine_id = -1;
            c_Input_Qty = -1;
            c_User = -1;
            c_Recipe1 = null;
            c_Recipe2 = null;
        }

        public LotUpdateInfo(int id, string lot_no) : base(id, lot_no)
        {
            c_IsOk = false;
            c_ErrorNo = -1;
            c_ErrorMessage = null;
            c_Step_No = -1;
            c_Ticket_ID = -1;
            c_Start_Time = new DateTime(0);
            c_Machine_id = -1;
            c_Input_Qty = -1;
            c_User = -1;
            c_Recipe1 = null;
            c_Recipe2 = null;
        }

        public bool IsOk
        {
            get { return c_IsOk; }
            set { c_IsOk = value; }
        }
        public int ErrorNo
        {
            get { return c_ErrorNo; }
            set { c_ErrorNo = value; }
        }
        public string ErrorMessage
        {
            get { return c_ErrorMessage; }
            set { c_ErrorMessage = value; }
        }
        public int Step_No
        {
            get { return c_Step_No; }
            set { c_Step_No = value; }
        }
        public DateTime Start_Time
        {
            get { return c_Start_Time; }
            set { c_Start_Time = value; }
        }
        public int Ticket_ID
        {
            get { return c_Ticket_ID; }
            set { c_Ticket_ID = value; }
        }
        public int Machine_id
        {
            get { return c_Machine_id; }
            set { c_Machine_id = value; }
        }
        public int Input_Qty
        {
            get { return c_Input_Qty; }
            set { c_Input_Qty = value; }
        }
        public int User
        {
            get { return c_User; }
            set { c_User = value; }
        }
        public string Recipe1
        {
            get { return c_Recipe1; }
            set { c_Recipe1 = value; }
        }
        public string Recipe2
        {
            get { return c_Recipe2; }
            set { c_Recipe2 = value; }
        }
    }
    #endregion

    #region LOT_SETUP_RESULT
    public class LotSetupResult : LotUpdateInfo
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal LotSetupResult(int id, string name) : base(id, name) { }
        #endregion
    }
    #endregion
}