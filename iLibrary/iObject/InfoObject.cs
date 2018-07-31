using System;
using System.Data;
using System.Runtime.Serialization;

namespace iLibrary
{
    [DataContract()]
    public class Info
    {
        #region FIELD
        private int c_Id;
        private string c_Name;
        private DB_GetConverter c_DBCONVERT;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        internal Info() { }
        /// <summary>
        /// Class Info's constructor with datarow parameter
        /// </summary>
        /// <param name="info">Data row with specific column name [<int>Id,<string>Name]</param>
        internal Info(DataRow info)
        {
            c_DBCONVERT = new DB_GetConverter();
            c_Id = DBCONVERT.GetInt32(info, "Id");
            c_Name = DBCONVERT.GetString(info, "Name").Trim();
        }
        internal Info(int id, string name)
        {
            c_Id = id;
            if (name == null) { c_Name = null; }
            else { c_Name = name.Trim(); }
            c_DBCONVERT = new DB_GetConverter();
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public int Id
        {
            get { return c_Id; }
            set { c_Id = value; }
        }
        [DataMember()]
        public string Name
        {
            get { return c_Name; }
            set { c_Name = value; }
        }
        [DataMember()]
        public DB_GetConverter DBCONVERT
        {
            get { return c_DBCONVERT; }
            set { c_DBCONVERT = value; }
        }

        [DataMember()]
        public int Set_Id
        {
             set { c_Id = value; }
        }
        [DataMember()]
        public string Set_Name
        {
            set { c_Name = value; }
        }
        //-----------------------------------------------Delete later
        //[DataMember()]
        //internal int SetId
        //{
        //    set { c_Id = value; }
        //}
        //[DataMember()]
        //internal string SetName
        //{
        //    set { c_Name = value; }
        //}
        #endregion
    }
    public class UserInfo : Info
    {
        #region FIELD
        private string c_FullName;
        private string c_EnglishName;
        private string c_Code;
        private Boolean c_Is_PD_Automotive;
        private Boolean c_Is_PM_Automotive;
        private License[] c_License;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        /// <summary>
        /// Class UserInfo's constructor with datarow parameter
        /// </summary>
        /// <param name="userInfo">Data row with specific column name [<int>Id,<string>Name,<string>FullName,<string>EnglishName,<string>Code,<string>DefaultLanguage]</param>
        internal UserInfo(DataRow userInfo) : base(userInfo)
        {
            c_FullName = DBCONVERT.GetString(userInfo,"FullName").Trim();
            c_EnglishName = DBCONVERT.GetString(userInfo,"EnglishName").Trim();
            c_Code = DBCONVERT.GetString(userInfo,"Code").Trim();
            c_Is_PD_Automotive = false;
            c_Is_PM_Automotive = false;
            c_License = null;
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public string FullName
        {
            get { return c_FullName; }
            private set { c_FullName = value; }
        }
        [DataMember()]
        public string EnglishName
        {
            get { return c_EnglishName; }
            private set { c_EnglishName = value; }
        }
        [DataMember()]
        public string Code
        {
            get { return c_Code; }
            private set { c_Code = value; }
        }
        [DataMember()]
        public Boolean Is_PD_Automotive
        {
            get { return c_Is_PD_Automotive; }
            set { c_Is_PD_Automotive = value; }
        }
        [DataMember()]
        public Boolean Is_PM_Automotive
        {
            get { return c_Is_PM_Automotive; }
            set { c_Is_PM_Automotive = value; }
        }
        public License[] License
        {
            get { return c_License; }
            set { c_License = value; }
        }

        #endregion
    }

    public class License : Info
    {
        #region FIELD
        private DateTime c_ExpireDate;
        private Boolean c_Is_Warning;
        private Boolean c_Is_Expired;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR

        internal License(DataRow userInfo, int LimitDates = 30) : base(userInfo)
        {
            c_ExpireDate = DBCONVERT.GetDateTime(userInfo, "expire");
            if (c_ExpireDate < DateTime.Now) { c_Is_Expired = true; }
            else { c_Is_Expired = false; }
            if ((c_ExpireDate - DateTime.Now).TotalDays < LimitDates) { c_Is_Warning = true; }
            else { c_Is_Warning = false; }
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public DateTime ExpireDate
        {
            get { return c_ExpireDate; }
            private set { c_ExpireDate = value; }
        }
        [DataMember()]
        public Boolean Is_Expired
        {
            get { return c_Is_Expired; }
            private set { c_Is_Expired = value; }
        }
        public Boolean Is_Warning
        {
            get { return c_Is_Warning; }
            private set { c_Is_Warning = value; }
        }
        #endregion
    }

    public class MachineInfo : Info
    {
        #region FIELD
        private MachineModelInfo c_MachineModels;
        private String c_MachineType;
        private bool c_IsAutomotive;
        private bool c_IsDisable;
        private JobInfo[] c_Jobs;
        private MachineState c_MachineState;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        /// <summary>
        /// Class MachineInfo's constructor with datarow parameter
        /// </summary>
        /// <param name="machineInfo">Data row with specific column name [<int>Id,<string>Name,<int>MachineTypeId,<string>MachineType,<bool>IsAutomotive,<bool>IsDisable]</param>
        /// <param name="jobInfo">Data row with specific column name [<int>Id,<string>Name]</param>
        /// <param name="machineStateInfo">Data row with specific column name [<int>OnlineState,<int>RunState,<int>QcState]</param>
        internal MachineInfo(DataRow machineInfo, DataTable jobInfo, DataRow machineStateInfo) : base(machineInfo)
        {
            c_MachineModels = new MachineModelInfo(DBCONVERT.GetInt32(machineInfo,"MachineModelId"), DBCONVERT.GetString(machineInfo, "MachineModel"));
            c_MachineType = DBCONVERT.GetString(machineInfo, "MachineType");
            c_IsAutomotive = Convert.ToBoolean(DBCONVERT.GetByte(machineInfo, "IsAutomotive"));
            c_IsDisable = Convert.ToBoolean(DBCONVERT.GetByte(machineInfo, "IsDisable"));
            c_Jobs = new JobInfo[jobInfo.Rows.Count];

            if (jobInfo != null)
            {
                for (int i = 0; i < jobInfo.Rows.Count; i++)
                {

                    JobInfo c_tmps = new JobInfo(jobInfo.Rows[i]);
                    c_Jobs[i] = c_tmps;
                }
            }
            if (machineStateInfo != null)
            {
                c_MachineState = new MachineState(DBCONVERT.GetByte(machineStateInfo, "OnlineState"), DBCONVERT.GetByte(machineStateInfo, "RunState"), DBCONVERT.GetByte(machineStateInfo, "QcState"));
            }
        }

        internal MachineInfo(int id, string name) : base(id, name)
        {
            c_MachineModels = null;
            c_MachineType = null;
            c_IsAutomotive = true;
            c_IsDisable = false;
            c_Jobs = null;
            c_MachineState = null;
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public MachineModelInfo MachineModel
        {
            get { return c_MachineModels; }
            private set { c_MachineModels = value; }
        }
        [DataMember()]
        public String MachineType
        {
            get { return c_MachineType; }
            private set { c_MachineType = value; }
        }
        [DataMember()]
        public bool IsAutomotive
        {
            get { return c_IsAutomotive; }
            private set { c_IsAutomotive = value; }
        }
        [DataMember()]
        public bool IsDisable
        {
            get { return c_IsDisable; }
            private set { c_IsDisable = value; }
        }
        [DataMember()]
        public JobInfo[] Job
        {
            get { return c_Jobs; }
            private set { c_Jobs = value; }
        }
        [DataMember()]
        public MachineState MachineState
        {
            get { return c_MachineState; }
            private set { c_MachineState = value; }
        }
        #endregion
    }
    public class ProcessInfo : Info
    {
        #region FIELD
        private int c_IsMount;
        private int c_IsSkipped;
        private int c_IsFirstInsChecking;
        private int c_IsFinalInsChecking;
        
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        /// <summary>
        /// Class MachineInfo's constructor with datarow parameter
        /// </summary>
        /// <param name="machineInfo">Data row with specific column name [<int>Id,<string>Name,<int>MachineTypeId,<string>MachineType,<bool>IsAutomotive,<bool>IsDisable]</param>
        /// <param name="jobInfo">Data row with specific column name [<int>Id,<string>Name]</param>
        /// <param name="machineStateInfo">Data row with specific column name [<int>OnlineState,<int>RunState,<int>QcState]</param>
        internal ProcessInfo() : base()
        {
            c_IsMount = 0;
            c_IsSkipped = 1;
            c_IsFirstInsChecking =1;
            c_IsFinalInsChecking = 1;
        }

        internal ProcessInfo(int id, string name) : base(id, name)
        {
            c_IsMount = 0;
            c_IsSkipped = 1;
            c_IsFirstInsChecking = 1;
            c_IsFinalInsChecking = 1;
        }
        internal ProcessInfo(DataRow LotInfo) : base(LotInfo)
        {
            c_IsMount = DBCONVERT.GetByte(LotInfo,"is_mount");
            c_IsSkipped = DBCONVERT.GetByte(LotInfo,"is_skipped");
            c_IsFirstInsChecking = DBCONVERT.GetByte(LotInfo,"is_first_ins");
            c_IsFinalInsChecking = DBCONVERT.GetByte(LotInfo,"is_final_ins");
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public int IsMount
        {
            get { return c_IsMount; }
            private set { c_IsMount = value; }
        }
        [DataMember()]
        public int IsSkipped
        {
            get { return c_IsSkipped; }
            private set { c_IsSkipped = value; }
        }
        [DataMember()]
        public int IsFirstInsChecking
        {
            get { return c_IsFirstInsChecking; }
            private set { c_IsFirstInsChecking = value; }
        }
        [DataMember()]
        public int IsFinalInsChecking
        {
            get { return c_IsFinalInsChecking; }
            private set { c_IsFinalInsChecking = value; }
        }
        #endregion
    }
    public class LotInfo : Info
    {
        #region FIELD
        private bool c_IsAutomotive;
        private bool c_IsSpecialFlow;
        private DeviceInfo c_Device;
        private PackageInfo c_Package;
        private JobInfo c_Job;
        private RecipeInfo c_DeviceFlows_Recipe;
        private LotState c_LotState;
        private Quantity c_Quantity;
        private int c_PcsParWork;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        #region Create By Rohm Kyoto
        internal LotInfo()
        {
            c_IsAutomotive = true;
            c_IsSpecialFlow = false;
            c_Device = null;
            c_Package = null;
            c_Job = null;
            c_LotState = null;
            c_Quantity = null;
            c_PcsParWork = 0;
        }

        internal LotInfo(int id, string name, int IsAutomotive, int JobId) : base(id, name)
        {
            if (IsAutomotive == 1) { c_IsAutomotive = true; }
            else { c_IsAutomotive = false; }
            c_IsSpecialFlow = false;
            c_Device = null;
            c_Package = null;
            c_Job = new JobInfo(JobId, null);
            c_LotState = null;
            c_Quantity = null;
            c_PcsParWork = 0;
        }

        internal LotInfo(int id, string name) : base(id, name)
        {
            c_IsAutomotive = true;
            c_IsSpecialFlow = false;
            c_Device = null;
            c_Package = null;
            c_Job = null;
            c_LotState = null;
            c_Quantity = null;
            c_PcsParWork = 0;
        }
        #endregion
        /// <summary>
        /// Class LotInfo's constructor with datarow parameter
        /// </summary>
        /// <param name="lotInfo">Data row with specific column name [<int>Id,<string>Name,<bool>IsAutomotive,<bool>IsSpecialFlow,<int>DeviceId,<string>DeviceName,<int>PackageId,<string>PackageName,<int>QuantityIn,<int>QuantityPass,<int>QuantityFail]</param>
        /// <param name="jobInfo">Data row with specific column name [<int>Id,<string>Name]</param>
        /// <param name="RecipeInfo">Data row with specific column name [<int>Id,<string>Name]</param>
        /// <param name="LotStateInfo">Data row with specific column name [<int>ProcessState,<int>QualityState]</param>
        internal LotInfo(DataRow lotInfo,DataRow jobInfo,DataRow recipeInfo,DataRow lotStateInfo, Boolean Is_Abnormal) : base(lotInfo)
        {
            c_IsAutomotive = Convert.ToBoolean(DBCONVERT.GetByte(lotInfo,"IsAutomotive"));
            c_IsSpecialFlow = Convert.ToBoolean(DBCONVERT.GetByte(lotInfo,"IsSpecialFlow"));
            c_Device = new DeviceInfo(DBCONVERT.GetInt32(lotInfo,"DeviceId"), DBCONVERT.GetString(lotInfo,"DeviceName"));
            c_Package = new PackageInfo(DBCONVERT.GetInt32(lotInfo,"PackageId"), DBCONVERT.GetString(lotInfo,"PackageName"));
            if (jobInfo != null)
            {
                c_Job = new JobInfo(DBCONVERT.GetInt32(jobInfo,"Id"), DBCONVERT.GetString(jobInfo,"Name"));
            }
            if (recipeInfo != null)
            {
                c_DeviceFlows_Recipe = new RecipeInfo(DBCONVERT.GetInt32(jobInfo,"Id"), DBCONVERT.GetString(jobInfo,"Name"));
            }
            if (lotStateInfo != null)
            {
                c_LotState = new LotState(DBCONVERT.GetByte(lotStateInfo,"ProcessState"), DBCONVERT.GetByte(lotStateInfo,"QualityState"));
            }

            c_Quantity = new Quantity(lotInfo, Is_Abnormal);
            c_PcsParWork = DBCONVERT.GetInt16(lotInfo,"PcsPerWork");
        }

        #endregion
        #region PROPERTY
        [DataMember()]
        public bool IsAutomotive
        {
            get { return c_IsAutomotive; }
            private set { c_IsAutomotive = value; }
        }
        [DataMember()]
        public bool IsSpecialFlow
        {
            get { return c_IsSpecialFlow; }
            private set { c_IsSpecialFlow = value; }
        }
        [DataMember()]
        public PackageInfo Package
        {
            get { return c_Package; }
            private set { c_Package = value; }
        }
        [DataMember()]
        public JobInfo Job
        {
            get { return c_Job; }
            private set { c_Job = value; }
        }
        [DataMember()]
        public RecipeInfo DeviceFlows_Recipe
        {
            get { return c_DeviceFlows_Recipe; }
            private set { c_DeviceFlows_Recipe = value; }
        }
        [DataMember()]
        public LotState LotState
        {
            get { return c_LotState; }
            private set { c_LotState = value; }
        }
        [DataMember()]
        public Quantity Quantity
        {
            get { return c_Quantity; }
            set { c_Quantity = value; }
        }
        [DataMember()]
        public int PcsPerWork
        {
            get { return c_PcsParWork; }
            set { c_PcsParWork = value; }
        }
        //-----------------------------------------------Delete later
        //[DataMember()]
        //public bool SetIsAutomotive
        //{
        //    set { c_IsAutomotive = value; }
        //}
        //[DataMember()]
        //public JobInfo SetJob
        //{
        //    set { c_Job = value; }
        //}
        #endregion
    }
    #region COMPONENT

    public class MachineModelInfo : Info
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal MachineModelInfo(int id, string name) : base(id, name) { }
        #endregion
    }
    public class JobInfo : Info
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal JobInfo(int id, string name) : base(id, name) { }
        internal JobInfo(DataRow Info) : base(Info) { }
        #endregion
    }
    public class DeviceInfo : Info
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal DeviceInfo(int id, string name) : base(id, name) { }
        internal DeviceInfo(DataRow Info) : base(Info) { }
        #endregion
    }
    public class PackageInfo : Info
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal PackageInfo(int id, string name) : base(id, name) { }
        internal PackageInfo(DataRow Info) : base(Info) { }
        #endregion
    }
    public class RecipeInfo : Info
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal RecipeInfo(int id, string name) : base(id, name) { }
        internal RecipeInfo(DataRow Info) : base(Info) { }
        #endregion
    }
    [DataContract()]
    public class MachineState
    {
        #region FIELD
        private int c_OnlineState;
        private int c_RunState;
        private int c_QcState;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        internal MachineState(int onlineState, int runState, int qcState)
        {
            c_OnlineState = onlineState;
            c_RunState = runState;
            c_QcState = qcState;
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public int OnlineState
        {
            get { return c_OnlineState; }
            private set { c_OnlineState = value; }
        }
        [DataMember()]
        public int RunState
        {
            get { return c_RunState; }
            private set { c_RunState = value; }
        }
        [DataMember()]
        public int QcState
        {
            get { return c_QcState; }
            private set { c_QcState = value; }
        }
        #endregion
    }
    [DataContract()]
    public class LotState
    {
        #region FIELD
        private int c_ProcessState;
        private int c_QualityState;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        internal LotState(int processState, int qualityState)
        {
            c_ProcessState = processState;
            c_QualityState = qualityState;
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public int ProcessState
        {
            get { return c_ProcessState; }
            private set { c_ProcessState = value; }
        }
        [DataMember()]
        public int QualityState
        {
            get { return c_QualityState; }
            private set { c_QualityState = value; }
        }
        #endregion
    }
    [DataContract()]
    public class Quantity
    {
        #region FIELD
        private int c_In;
        private int c_Pass;
        private int c_Fail;
        private int c_LastPass;
        private int c_LastFail;
        private int c_PassStepSum;
        private int c_FailStepSum;

        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        internal Quantity(int quantityIn,int quantityPass,int quantityFail,int qty_LastPass,int qty_LastFail, int qty_PassStepSum,int qty_FailStepSum)
        {
            In = quantityIn;
            Pass = quantityPass;
            Fail = quantityFail;
            LastPass = qty_LastPass;
            LastFail = qty_LastFail;
            PassStepSum = qty_PassStepSum;
            FailStepSum = qty_FailStepSum;
        }
        internal Quantity(DataRow lotInfo, bool Is_Abnormal)
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            In = DBCONVERT.GetInt32(lotInfo, "QuantityIn");
            Pass = DBCONVERT.GetInt32(lotInfo, "QuantityPass");
            Fail = DBCONVERT.GetInt32(lotInfo, "QuantityFail");
            LastPass = DBCONVERT.GetInt32(lotInfo, "QuantityLastPass");
            LastFail = DBCONVERT.GetInt32(lotInfo, "QuantityLastFail");
            PassStepSum = DBCONVERT.GetInt32(lotInfo, "QuantityPassStepSum");
            FailStepSum = DBCONVERT.GetInt32(lotInfo, "QuantityFailStepSum");
            if (Is_Abnormal) { Pass = In - (PassStepSum + FailStepSum); }
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public int In
        {
            get { return c_In; }
            private set { c_In = value; }
        }
        [DataMember()]
        public int Pass
        {
            get { return c_Pass; }
            set { c_Pass = value; }
        }
        [DataMember()]
        public int Fail
        {
            get { return c_Fail; }
            set { c_Fail = value; }
        }
        [DataMember()]
        public int LastPass
        {
            get { return c_LastPass; }
            set { c_LastPass = value; }
        }
        [DataMember()]
        public int LastFail
        {
            get { return c_LastFail; }
            set { c_LastFail = value; }
        }
        [DataMember()]
        public int PassStepSum
        {
            get { return c_PassStepSum; }
            set { c_PassStepSum = value; }
        }
        [DataMember()]
        public int FailStepSum
        {
            get { return c_FailStepSum; }
            set { c_FailStepSum = value; }
        }
        #endregion
    }
    #endregion
}
