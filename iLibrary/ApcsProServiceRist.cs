using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Rohm.Common.Logging;

namespace iLibrary
{
    public partial class ApcsProService
    : IApcsProService
    {
        #region IApcsProService Members

        #region FUNCTION

        #region GET
        #region MAN
        public UserInfo GetUserInfo(string userCode, int LimitDates = 30)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable userInfo = db.SelectData(dbObject, "select id as Id,name as Name,full_name as FullName,english_name as EnglishName,emp_num as Code,default_language from man.users where users.emp_num = @userCode", new System.Data.SqlClient.SqlParameter("@userCode", SqlDbType.VarChar) { Value = userCode });

            if (userInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetUserInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }

            string userDefaultLanguage = userInfo.Rows[0].Field<string>("default_language");
            ErrorMessageProvider.Language = userDefaultLanguage;
            FunctionStepNameProvider.Language = userDefaultLanguage;
            functionTimer.Stop();
            SaveFunctionLog("GetUserInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
            UserInfo info = new UserInfo(userInfo.Rows[0]);

            DataTable UserLicense = db.SelectData(dbObject, "select LI.lic_id as id, LI.lic_name as name, UI.stop_date as expire from ctrlic.user_lic as UI with(NOLOCK) inner join ctrlic.license as LI with(NOLOCK) on LI.lic_id = UI.lic_id where UI.user_id = @user_id", new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = info.Id });
            dbObject.ConnectionClose();

            License[] Licenses = null;
            for (int i = 0; i < UserLicense.Rows.Count; i++)
            {
                if (i == 0) { Licenses = new License[UserLicense.Rows.Count]; }
                License Licenseinfo = new License(UserLicense.Rows[i], LimitDates);
                Licenses[i] = Licenseinfo;
            }

            info.Is_PD_Automotive = Check_UserAutomotiveByLMS(db, dbObject, info.Id, null);
            info.License = Licenses;
            return info;
        }
        public UserInfo GetUserInfoWithPassword(string userCode, string password)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable userInfo = db.SelectData(dbObject, "select id as Id,name as Name,full_name as FullName,english_name as EnglishName,emp_num as Code,default_language from man.users where users.emp_num = @userCode and password = @password", new System.Data.SqlClient.SqlParameter("@userCode", SqlDbType.VarChar) { Value = userCode }, new System.Data.SqlClient.SqlParameter("@password", SqlDbType.VarChar) { Value = password });
            dbObject.ConnectionClose();
            if (userInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetUserInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }
            string userDefaultLanguage = userInfo.Rows[0].Field<string>("default_language");
            ErrorMessageProvider.Language = userDefaultLanguage;
            FunctionStepNameProvider.Language = userDefaultLanguage;
            functionTimer.Stop();
            SaveFunctionLog("GetUserInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
            UserInfo info = new UserInfo(userInfo.Rows[0]);

            info.Is_PD_Automotive = Check_UserAutomotiveByLMS(db, dbObject, info.Id, null);
            return info;
        }
        #endregion
        #region MACHINE
        public MachineInfo GetMachineInfo(int machineId)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable machineInfo = db.SelectData(dbObject, "select MC.id as Id, MCS.machine_type_name as MachineType, MC.name as Name, MCM.id as MachineModelId, MCM.name as MachineModel, is_automotive as IsAutomotive, is_disabled as IsDisable from mc.machines as MC with(nolock) inner join mc.models as MCM with(nolock) on MCM.id = MC.machine_model_id inner join trans.machine_states as MCS with(nolock) on MCS.machine_id = MC.id where MC.id = @machineId", new System.Data.SqlClient.SqlParameter("@machineId", SqlDbType.VarChar) { Value = machineId });
            dbObject.ConnectionClose();
            if (machineInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }
            dbObject.ConnectionOpen();
            DataTable jobInfo = db.SelectData(dbObject, "select JB.id as Id, JB.name as Name from[method].jobs as JB with(nolock) inner join[mc].group_models as GM with(nolock) on GM.machine_group_id = JB.machine_group_id inner join[mc].machines as MC with(nolock) on MC.machine_model_id = GM.machine_model_id where MC.id = @machineId", new System.Data.SqlClient.SqlParameter("@machineId", SqlDbType.VarChar) { Value = machineId });
            dbObject.ConnectionClose();
            if (jobInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new MachineInfo(machineInfo.Rows[0], null,null);
            }
            dbObject.ConnectionOpen();
            DataTable machineStateInfo = db.SelectData(dbObject, "select online_state as OnlineState, run_state as RunState, qc_state as QcState from trans.machine_states where machine_states.machine_id = @machineId", new System.Data.SqlClient.SqlParameter("@machineId", SqlDbType.VarChar) { Value = machineInfo.Rows[0].Field<int>("Id") });
            dbObject.ConnectionClose();
            if (machineStateInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new MachineInfo(machineInfo.Rows[0], jobInfo, null);
            }
            functionTimer.Stop();
            SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new MachineInfo(machineInfo.Rows[0], jobInfo, machineStateInfo.Rows[0]);
        }
        #region Requested by Kyoto
        public MachineInfo GetMachineInfo(string machineName)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable machineInfo = db.SelectData(dbObject, "select MC.id as Id , MCS.machine_type_name as MachineType, MC.name as Name, MCM.id as MachineModelId, MCM.name as MachineModel, is_automotive as IsAutomotive, is_disabled as IsDisable from mc.machines as MC with(nolock) inner join mc.models as MCM with(nolock) on MCM.id = MC.machine_model_id inner join trans.machine_states as MCS with(nolock) on MCS.machine_id = MC.id where MC.name = @machineName", new System.Data.SqlClient.SqlParameter("@machineName", SqlDbType.VarChar) { Value = machineName });
            dbObject.ConnectionClose();
            if (machineInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }
            dbObject.ConnectionOpen();
            DataTable jobInfo = db.SelectData(dbObject, "select JB.id as Id, JB.name as Name from[method].jobs as JB with(nolock) inner join[mc].group_models as GM with(nolock) on GM.machine_group_id = JB.machine_group_id inner join[mc].machines as MC with(nolock) on MC.machine_model_id = GM.machine_model_id where MC.name = @machineName", new System.Data.SqlClient.SqlParameter("@machineName", SqlDbType.VarChar) { Value = machineName });
            dbObject.ConnectionClose();
            if (jobInfo == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new MachineInfo(machineInfo.Rows[0], null, null);
            }
            if (jobInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new MachineInfo(machineInfo.Rows[0], null, null);
            }
            dbObject.ConnectionOpen();
            DataTable machineStateInfo = db.SelectData(dbObject, "select online_state as OnlineState, run_state as RunState, qc_state as QcState from trans.machine_states where machine_states.machine_id = @machineId", new System.Data.SqlClient.SqlParameter("@machineId", SqlDbType.VarChar) { Value = machineInfo.Rows[0].Field<int>("Id") });
            dbObject.ConnectionClose();
            if (machineStateInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new MachineInfo(machineInfo.Rows[0], jobInfo, null);
            }
            functionTimer.Stop();
            SaveFunctionLog("GetMachineInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new MachineInfo(machineInfo.Rows[0], jobInfo, machineStateInfo.Rows[0]);
        }
        #endregion
        public MachineInfo[] GetMachineInfoArrayByCellConIp(string cellConIp)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable machineInfo = db.SelectData(dbObject, "select machines.id as Id from mc.machines where machines.cell_ip = @cellConIp", new System.Data.SqlClient.SqlParameter("@cellConIp", SqlDbType.VarChar) { Value = cellConIp });
            dbObject.ConnectionClose();
            if (machineInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfoByCellConId", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }
            else
            {
                int MachineInfoRowCount = machineInfo.Rows.Count;
                MachineInfo[] machineInfoArray = new MachineInfo[MachineInfoRowCount];
                for (int i = 0; i < MachineInfoRowCount; i++)
                {
                    machineInfoArray[i] = GetMachineInfo(machineInfo.Rows[i].Field<int>("Id"));
                }
                functionTimer.Stop();
                SaveFunctionLog("GetMachineInfoByCellConId", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return machineInfoArray;
            }
        }
        #endregion
        #region UNKNOWN
        public LotInfo GetLotInfo(string lotNo)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable lotInfo = db.SelectData(dbObject, "select lots.id as Id, lot_no as [Name], is_automotive as IsAutomotive, is_special_flow as IsSpecialFlow, device_names.id as DeviceId, process_state as ProcessState, device_names.[name] as DeviceName,packages.id as PackageId, packages.[name] as PackageName, qty_in as QuantityIn, qty_pass as QuantityPass, qty_last_pass as QuantityLastPass, qty_pass_step_sum as QuantityPassStepSum, qty_fail as QuantityFail, qty_last_fail as QuantityLastFail, qty_fail_step_sum as QuantityFailStepSum, job_id as JobId, method.packages.pcs_per_work as PcsPerWork from trans.lots inner join method.device_slips on lots.device_slip_id = device_slips.device_slip_id inner join method.device_versions on device_slips.device_id = device_versions.device_id and device_slips.version_num = device_versions.version_num inner join method.device_names on device_versions.device_name_id = device_names.id inner join method.device_flows on lots.device_slip_id = device_flows.device_slip_id and lots.step_no = device_flows.step_no inner join method.packages on package_id = packages.id where lot_no = @lotNo", new System.Data.SqlClient.SqlParameter("@lotNo", SqlDbType.VarChar) { Value = lotNo });
            dbObject.ConnectionClose();
            if (lotInfo.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("GetLotInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return null;
            }

            dbObject.ConnectionOpen();
            DataTable jobInfo = db.SelectData(dbObject, "select id as Id,name as Name from method.jobs where id = @jobId", new System.Data.SqlClient.SqlParameter("@jobId", SqlDbType.VarChar) { Value = lotInfo.Rows[0].Field<int>("JobId") });
            dbObject.ConnectionClose();
            DataRow jobInfoRow = null;
            if (jobInfo.Rows.Count != 0)
            {
                jobInfoRow = jobInfo.Rows[0];
            }

            dbObject.ConnectionOpen();
            DataTable recipeInfo = db.SelectData(dbObject, "select id as Id,name as Name from method.jobs where id = @recipeId", new System.Data.SqlClient.SqlParameter("@recipeId", SqlDbType.VarChar) { Value = lotInfo.Rows[0].Field<int>("JobId") });
            dbObject.ConnectionClose();
            DataRow recipeInfoRow = null;
            if (recipeInfo.Rows.Count != 0)
            {
                recipeInfoRow = recipeInfo.Rows[0];
            }

            dbObject.ConnectionOpen();
            DataTable lotStateInfo = db.SelectData(dbObject, "select process_state as ProcessState,quality_state as QualityState from trans.lots where id = @lotId", new System.Data.SqlClient.SqlParameter("@lotId", SqlDbType.VarChar) { Value = lotInfo.Rows[0].Field<int>("Id") });
            dbObject.ConnectionClose();
            DataRow lotStateInfoRow = null;
            if (lotStateInfo.Rows.Count != 0)
            {
                lotStateInfoRow = lotStateInfo.Rows[0];
            }
            functionTimer.Stop();
            SaveFunctionLog("GetLotInfo", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new LotInfo(lotInfo.Rows[0], jobInfoRow, recipeInfoRow, lotStateInfoRow, Get_LotAbnormalOrNot(lotInfo.Rows[0].Field<int>("Id"), null));
        }
        #endregion
        #endregion

        #region CHECK
        #region MAN
        public CheckUserPermissionResult CheckUserPermission(UserInfo userInfo, string appName, string functionName)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable userData;
            int stepCount = 1;
            Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
            if (userInfo == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckUserPermissionResult(1, WarningMessage, "www.google.com", stepCount);
            }
            dbObject.ConnectionOpen();
            userData = db.SelectData(dbObject, "select lockout,is_admin,case when getdate() > expired_on then 1 end as UserExpired from man.users where users.id = @userId", new System.Data.SqlClient.SqlParameter("@userId", SqlDbType.VarChar) { Value = userInfo.Id.ToString() });
            dbObject.ConnectionClose();
            stepCount += 1;
            if (!(userData.Rows[0].ItemArray[0].Equals(DBNull.Value)))
            {
                if (userData.Rows[0].Field<byte>("lockout") == 1)
                {
                    functionTimer.Stop();
                    SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                    return new CheckUserPermissionResult(2, WarningMessage, "www.google.com", stepCount);
                }
            }
            if (!(userData.Rows[0].ItemArray[1].Equals(DBNull.Value)))
            {
                if (userData.Rows[0].Field<byte>("is_admin") == 1)
                {
                    functionTimer.Stop();
                    SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                    return new CheckUserPermissionResult(WarningMessage);
                }
            }
            if (!(userData.Rows[0].ItemArray[2].Equals(DBNull.Value)))
            {
                if (userData.Rows[0].Field<int>("UserExpired") == 1)
                {
                    functionTimer.Stop();
                    SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                    return new CheckUserPermissionResult(3, WarningMessage, "www.google.com", stepCount);
                }
            }
            dbObject.ConnectionOpen();
            DataTable licenseData = db.SelectData(dbObject, "select case when getdate() > user_roles.expired_on then 1 end as LicenseExpired from man.user_roles inner join man.role_permissions on user_roles.role_id = role_permissions.role_id inner join man.permission_operations on role_permissions.permission_id = permission_operations.permission_id inner join man.operations on permission_operations.operation_id = operations.id where [user_id] = @userID and [app_name] = @appName and function_name = @functionName", new System.Data.SqlClient.SqlParameter("@userID", SqlDbType.VarChar) { Value = userInfo.Id.ToString() }, new System.Data.SqlClient.SqlParameter("@functionName", SqlDbType.VarChar) { Value = functionName }, new System.Data.SqlClient.SqlParameter("@appName", SqlDbType.VarChar) { Value = appName });
            dbObject.ConnectionClose();
            stepCount += 1;
            if (licenseData.Rows.Count == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckUserPermissionResult(4, WarningMessage, "www.google.com", stepCount);
            }
            else
            {
                if (!(licenseData.Rows[0].ItemArray[0].Equals(DBNull.Value)))
                {

                    if (licenseData.Rows[0].Field<string>("LicenseExpired") == "1")
                    {
                        functionTimer.Stop();
                        SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                        return new CheckUserPermissionResult(5, WarningMessage, "www.google.com", stepCount);
                    }
                }
                functionTimer.Stop();
                SaveFunctionLog("CheckUserPermission", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckUserPermissionResult(WarningMessage);
            }
        }
        #endregion
        #region MACHINE
        public CheckMachineConditionResult CheckMachineCondition(MachineInfo machineInfo)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            int stepCount = 1;
            Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
            if (machineInfo == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(6, WarningMessage, "www.google.com", stepCount);
            }         
            stepCount += 1;            
            if (machineInfo.IsDisable)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(7, WarningMessage, "www.google.com", stepCount);
            }
            if (machineInfo.MachineState == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(25, WarningMessage, "www.google.com", stepCount);
            }
            if (machineInfo.MachineState.OnlineState == 0)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(23, WarningMessage, "www.google.com", stepCount);
            }
            if (machineInfo.MachineState.OnlineState == 2)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(24, WarningMessage, "www.google.com", stepCount);
            }
            if (machineInfo.MachineState.RunState == 3)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(21, WarningMessage, "www.google.com", stepCount);
            }
            stepCount += 1;
            if (machineInfo.MachineState.QcState == 1)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckMachineConditionResult(8, WarningMessage, "www.google.com", stepCount);
            }
            if (machineInfo.MachineState.QcState == 2)
            {
                WarningMessage.Add(stepCount, ErrorMessageProvider.GetErrorMessage(9));
            }
            functionTimer.Stop();
            SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new CheckMachineConditionResult(WarningMessage);
        }
        #endregion
        #region MATERIAL
        #endregion
        #region UNKNOWN
        public CheckLotConditionResult CheckLotCondition(LotInfo lotInfo)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            int stepCount = 1;
            Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
            if (lotInfo == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(10, WarningMessage, "www.google.com", stepCount);
            }
            stepCount += 1;
            if (lotInfo.LotState == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(26, WarningMessage, "www.google.com", stepCount);
            }
            if (lotInfo.LotState.ProcessState == 1)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(13, WarningMessage, "www.google.com", stepCount);
            }
            if (lotInfo.LotState.ProcessState == 2)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(14, WarningMessage, "www.google.com", stepCount);
            }
            if (lotInfo.LotState.ProcessState == 3)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(15, WarningMessage, "www.google.com", stepCount);
            }
            stepCount += 1;
            if (lotInfo.LotState.QualityState == 1)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(16, WarningMessage, "www.google.com", stepCount);
            }
            if (lotInfo.LotState.QualityState == 2)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(17, WarningMessage, "www.google.com", stepCount);
            }
            if (lotInfo.LotState.QualityState == 3)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotConditionResult(18, WarningMessage, "www.google.com", stepCount);
            }
            functionTimer.Stop();
            SaveFunctionLog("CheckMachineCondition", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new CheckLotConditionResult(WarningMessage);
        }

        public CheckLotAndMachineCompatibilityResult CheckLotAndMachineCompatibility(LotInfo lotInfo, MachineInfo machineInfo)
        {
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            int stepCount = 1;
            Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
            //1.) machine must be set as high reliability if lot is auto motive
            if (lotInfo.IsAutomotive && !machineInfo.IsAutomotive)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotAndMachineCompatibility", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotAndMachineCompatibilityResult(11, WarningMessage, "www.google.com", stepCount);
            }
            stepCount += 1;
            //2.) check machine ...
            //  2.1) process step must be equal
            if (machineInfo.Job == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotAndMachineCompatibility", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotAndMachineCompatibilityResult(27, WarningMessage, "www.google.com", stepCount);

            }
            if (lotInfo.Job == null)
            {
                functionTimer.Stop();
                SaveFunctionLog("CheckLotAndMachineCompatibility", functionTimer.ElapsedMilliseconds, DateTime.Now);
                return new CheckLotAndMachineCompatibilityResult(28, WarningMessage, "www.google.com", stepCount);
            }
            for (int i = 0; i < machineInfo.Job.Length; i++) {
                if (lotInfo.Job.Id == machineInfo.Job[i].Id)
                {
                    break;
                }
                if (i == machineInfo.Job.Length - 1)
                {
                    functionTimer.Stop();
                    SaveFunctionLog("CheckLotAndMachineCompatibility", functionTimer.ElapsedMilliseconds, DateTime.Now);
                    return new CheckLotAndMachineCompatibilityResult(12, WarningMessage, "www.google.com", stepCount);
                }
            }
            //  2.2) machine type must equal to specific as the lot needed

            //3.) machine's material check
            //  3.1) material type check
            //  3.2) material life time check
            //  3.3) material usagable date check

            //4.) machine's jig check
            //  4.1) jig type check
            //  4.2) jig life time check

            //5.) machine's equipment check
            //  5.1) equipment type check
            //  5.2) equipment setting check
            //  5.3) equipment quantity check

            functionTimer.Stop();
            SaveFunctionLog("CheckLotAndMachineCompatibility", functionTimer.ElapsedMilliseconds, DateTime.Now);
            return new CheckLotAndMachineCompatibilityResult(WarningMessage);
        }
        #endregion
        #endregion

        #region UNKNOWN
        #endregion
        #endregion
        #endregion
        #region OTHER

        public Boolean Check_UserAutomotiveByLMS(DatabaseAccess db, DatabaseAccessObject dbObject, int user_id, Logger Log)
        {
            List<String> Machines = new List<string>();

            String FunctionName = "Check_UserAutomotiveByLMS";
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            dbObject.ConnectionOpen();
            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select US.id, LC.lic_name From[man].users as US with(NOLOCK) inner join[ctrlic].user_lic as UL with(NOLOCK) on UL.user_id = US.id inner join[ctrlic].license as LC with(NOLOCK) on LC.lic_id = UL.lic_id and LC.lic_name = 'PD AUTOMOTIVE' where US.id = @user_id", Log.SqlLogger, sqlparameters);
            dbObject.ConnectionClose();

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            try
            {
                if (tmp1 == null)
                {
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "User don't have Automotive License.", "");
                    return false;
                }
                if (tmp1.Rows.Count == 0)
                {
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "User don't have Automotive License.", "");
                    return false;
                }
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.ConnectionClose();
                throw new Exception(e.Message, e);
            }

            Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "User have Automotive License.", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return true;// Machines.Exists(x => x == machine_name);
        }

        public static bool SaveFunctionLog(string functionName, long runtime, DateTime now)
        {
            return true;
        }
        #endregion
    }
}
