using System;
using System.ServiceModel;
using System.Data;
using Rohm.Common.Logging;
using System.Collections.Generic;

namespace iLibrary
{
    [ServiceContract()]
    public interface IApcsProService
    {
        //[OperationContract()]
        //UserInfo GetUserInfo(string userCode);

        [OperationContract()]
        UserInfo GetUserInfo(string userCode, int LimitDates = 30);

        [OperationContract()]
        UserInfo GetUserInfoWithPassword(string userCode,string password);

        [OperationContract()]
        CheckUserPermissionResult CheckUserPermission(UserInfo userInfo, string appName, string functionName);

        [OperationContract()]
        MachineInfo GetMachineInfo(int machineId);
        
        [OperationContract()]
        MachineInfo GetMachineInfo(string machineNo);
        
        [OperationContract()]
        MachineInfo[] GetMachineInfoArrayByCellConIp(string cellConIp);

        [OperationContract()]
        LotInfo GetLotInfo(string lotNo);

        [OperationContract()]
        LotUpdateInfo LotSetup(int lot_id, int machine_id, int user_id, int DBx, string XmlData, int Online_state, DateTime time, Logger Log);

        //[OperationContract()]
        //LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, string XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, String Recipe, Logger Log, int Location_Num = 1, int act_pass_qty = -1);

        [OperationContract()]
        LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, String Recipe, DateTime time, Logger Log, int Location_Num = 1, int act_pass_qty = -1);

        //[OperationContract()]
        //LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, Logger Log);

        LotUpdateInfo OnlineStart(int lot_id, int machine_id, int user_id, int DBx, string XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo OnlineEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo Update_Firstinspection(int lot_id, int Ins_Result, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo Update_Finalinspection(int lot_id, int Ins_Result, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        MachineUpdateInfo Update_ErrorHappenRecord(int[] lot_id, MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log);

        [OperationContract()]
        MachineUpdateInfo Update_ErrorResetRecord(MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log);

        [OperationContract()]
        MachineUpdateInfo Update_ErrorRecoveryRecord(MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log);

        [OperationContract()]
        DataRow Get_LotDBData(int id, Logger Log);

        //[OperationContract()]
        //LotInfo Get_LotDBData(int lotId);

        [OperationContract()]
        DataTable Get_LotDBTable(int[] ids, Logger Log);

        [OperationContract()]
        DataTable Get_LotInfos(int[] ids, Logger Log);

        [OperationContract()]
        int CountUp_Numbers(DatabaseAccess db, DatabaseAccessObject dbObject, string name, int num, LoggerObject Log);

        [OperationContract()]
        DateTimeInfo Get_DateTimeInfo(Logger Log);

        [OperationContract()]
        CheckMachineConditionResult CheckMachineCondition(MachineInfo machineInfo);

        [OperationContract()]
        CheckLotConditionResult CheckLotCondition(LotInfo lotInfo);

        [OperationContract()]
        CheckLotAndMachineCompatibilityResult CheckLotAndMachineCompatibility(LotInfo lotInfo, MachineInfo machineInfo);

        [OperationContract()]
        CheckProcessFlowResult CheckProcessFlow(int state, int lot_id, int machine_id, Logger Log);

        [OperationContract()]
        LotUpdateInfo LotCancel(int lot_id, int machine_id, int user_id, int online_state, Logger Log);

        [OperationContract()]
        LotUpdateInfo ReInput(int lot_id, int machine_id, int user_id, int qty_pass, int qty_fail, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo LotEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        int Update_MachineState(int machine_id, int run_state, int user_id, Logger Log);

        [OperationContract()]
        int Update_MachineState(ref LotUpdateInfo lotinfo, int machine_id, int run_state, int user_id, Logger Log);

        [OperationContract()]
        int Update_MachineQualityState(int machine_id, int run_state, int user_id, Logger Log);

        [OperationContract()]
        int Update_MachineQualityState(ref LotUpdateInfo lotinfo, int machine_id, int run_state, int user_id, Logger Log);

        [OperationContract()]
        int Update_MachineOnlineState(int machine_id, int online_state, int user_id, Logger Log);

        [OperationContract()]
        int Update_MachineOnlineState(ref LotUpdateInfo lotinfo, int machine_id, int online_state, int user_id, Logger Log);

        [OperationContract()]
        Info[] Get_EnabledPackageList(int division_id, Logger Log);

        [OperationContract()]
        DataRow Get_CheckFlowData(int lot_id, int machine_id, Logger Log);

        [OperationContract()]
        DataRow Get_SpLotDBData(int id, Logger Log);

        [OperationContract()]
        DataTable Get_SpLotDBTable(int[] ids, Logger Log);

        //[OperationContract()]
        //int LotSetup_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotSetup_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotCancel_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotStart_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int record_id, int pj_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotStart_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotCancel_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int record_id, int pj_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotEnd_NormalFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_faill, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int record_id, int pj_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        //[OperationContract()]
        //int LotEnd_SpecialFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_faill, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String FunctionName);

        [OperationContract()]
        JigInfo[] Get_JigsbyMachine(int machine_id, int lot_id, Logger Log);

        //[OperationContract()]
        //int[] Get_JigIdsbyMachine(int machine_id, int lot_id);

        [OperationContract()]
        JigInfo[] Get_JigsbyMachine(int machine_id, int lot_id, string item_name, int division_id, Logger Log);

        [OperationContract()]
        CheckJigResult[] Check_Jigs(int[] jig_id, bool Is_WarnError, Logger Log);

        [OperationContract()]
        int Update_JigValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int jig_id, int countunit, int control, Logger Log);

        [OperationContract()]
        int Update_JigState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int jig_id, Logger Log);

        [OperationContract()]
        CheckJigResult[] Check_Jig(int jig_id, bool Is_WarnError, Logger Log);

        [OperationContract()]
        DateTime[] TimeUp_Calculation(DatabaseAccess db, DatabaseAccessObject dbObject, int lot_id, int next_step_no, Logger Log);

        [OperationContract()]
        JigInfo Get_JigsbyID(string barcode, string item_name, int division_id, Logger Log);

        [OperationContract()]
        MaterialInfo[] Get_MaterialsbyMachine(int machine_id, int lot_id, string item_name, int division_id, Logger Log);

        [OperationContract()]
        CheckMaterialResult[] Check_Materials(int[] Material_id, bool Is_WarnError, Logger Log);

        [OperationContract()]
        int Update_MaterialValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int Material_id, int countunit, int control, Logger Log);

        [OperationContract()]
        int Update_MaterialState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int Material_id, Logger Log);

        [OperationContract()]
        CheckMaterialResult[] Check_Material(int Material_id, bool Is_WarnError, Logger Log);

        //[OperationContract()]
        //MaterialInfo Get_MaterialsbyID(string barcode, string item_name, int division_id);

        [OperationContract()]
        EqpInfo[] Get_EqpsbyMachine(int machine_id, int lot_id, string item_name, int division_id, Logger Log);

        [OperationContract()]
        CheckEQPResult[] Check_Eqps(int[] EQP_id, bool Is_WarnError, Logger Log);

        [OperationContract()]
        int Update_EqpValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int EQP_id, int countunit, int control, Logger Log);

        [OperationContract()]
        int Update_EqpState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int EQP_id, Logger Log);

        [OperationContract()]
        CheckEQPResult[] Check_Eqp(int EQP_id, bool Is_WarnError, Logger Log);

        [OperationContract()]
        Boolean CheckPackageEnable(String packageName, Logger Log);

        [OperationContract()]
        GetRecipeResult Get_Recipe(int lot_id, int machine_id, Logger Log);

        [OperationContract()]
        List<String> Get_PermissionMachinesByLMS(int user_id, Logger Log);

        [OperationContract()]
        Boolean Check_PermissionMachinesByLMS(int user_id, string machine_name, Logger Log);

        [OperationContract()]
        LotUpdateInfo AbnormalLotHold(int lot_id, int machine_id, int user_id, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        LotUpdateInfo AbnormalLotEnd_BackToThe_BeforeProcess(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log);

        [OperationContract()]
        Boolean Get_LotAbnormalOrNot(int lot_id, Logger Log);

        [OperationContract()]
        Boolean Check_UserLotAutoMotive(UserInfo user, LotInfo lot, Logger Log);

        [OperationContract()]
        List<APCSMaterialInfo> MaterialCheckByAPCS(String Process, List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log);

        [OperationContract()]
        int GetReelCount(LotInfo Lots, Logger Log);

        [OperationContract()]
        Boolean CheckLotisExist(String LotName, Logger Log);

        [OperationContract()]
        List<AlarmInfoObject> Get_MachineAlarmCounter(int lot_id, MachineInfo machine_info, DateTime time, Logger Log);
        //[OperationContract()]
        //EqpInfo Get_EqpsbyID(string barcode, string item_name, int division_id);
    }
}
