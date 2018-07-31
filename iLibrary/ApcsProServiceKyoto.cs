using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Rohm.Common.Logging;
using DB_APCSMaterialControl;
using WB_APCSMaterialControl;
using MP_APCSMaterialControl;
using TC_APCSMaterialControl;

namespace iLibrary
{
    public partial class ApcsProService : IApcsProService
    {
        
        //State : 0.Setup, 1.Start, 2.End, 3.Cancel, 4.OnlineStart, 5.OnlineEnd
        public CheckProcessFlowResult CheckProcessFlow(int State, int lot_id, int machine_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            CheckProcessFlowResult result = new CheckProcessFlowResult();
            LotProcessCheckInfo CheckFlowInfo = null;
            Log = Log ?? new Logger();
            //if(Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_CheckFlowData(lot_id, machine_id,Log);
                if (tmp == null)
                {
                    //Datas Nothing
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                }
                CheckFlowInfo = new LotProcessCheckInfo(tmp);
                if (CheckFlowInfo == null)
                {
                    //Datas Nothing
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);

                }
                if (CheckFlowInfo.Is_special_flow == 1)
                {
                    CheckFlowInfo = new LotProcessCheckInfo(Get_CheckSpFlowData(lot_id, machine_id,Log));
                    if (CheckFlowInfo == null)
                    {
                        //Datas Nothing
                        iLibraryErrorAction(Errors_CheckProcessFlow.SPLotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.SPLotDoNotWip);
                    }
                }

                if (CheckFlowInfo.Wip_state != 20 )
                {
                    //Lot_State is not 0 (Can't Setup)
                    iLibraryErrorAction(Errors_CheckProcessFlow.WipStateIsNotInput, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.WipStateIsNotInput);
                }

                switch (State)
                {
                    case 0: //Setup
                        if (!(CheckFlowInfo.Process_state == 0 || CheckFlowInfo.Process_state == 100))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotSetup, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotSetup);
                        }
                        break;
                    case 1: //Start
                        if (!(CheckFlowInfo.Process_state == 1 || CheckFlowInfo.Process_state == 101))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotStart, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotStart);
                        }
                        break;
                    case 2: //End
                        if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotEnd, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotEnd);
                        }
                        break;
                    case 3: //Cancel
                        if (!(CheckFlowInfo.Process_state == 1 || CheckFlowInfo.Process_state == 101))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotCancel, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotCancel);
                        }
                        break;
                    case 4: //OnlineStart
                        if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotOnlineStart, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotOnlineStart);
                        }
                        break;
                    case 5: //OnlineEnd
                        if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                        {
                            //Lot_State is not 0 (Can't Setup)
                            iLibraryErrorAction(Errors_CheckProcessFlow.CanNotOnlineEnd, FunctionName, functionTimer, Log, dbObject);
                            return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotOnlineEnd);
                        }
                        break;
                    case 6:
                        {
                            break;
                        }
                    default:
                        iLibraryErrorAction(Errors_CheckProcessFlow.StrangeStates, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.StrangeStates);
                }
                switch (CheckFlowInfo.Quality_state)
                {
                    case 0: break;
                    case 1:
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Abnormal, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Abnormal);
                    case 2:
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Stop, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Stop);
                    case 3:
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Hold, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Hold);
                    case 4:
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_SPFlow, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_SPFlow);
                    default:
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotQCNG, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQCNG);
                }
                if (CheckFlowInfo.Package_IsEnable != 1)
                {
                    //Package_IsEnable is not 0 (Can't Setup)
                    iLibraryErrorAction(Errors_CheckProcessFlow.PackageNotEnable, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.PackageNotEnable);
                }
                //if (CheckFlowInfo.MachineRunState != 0)
                //{
                //    //Machine_AutoMotive is not AutoMotive (Can't Setup)
                //    ErrorNo = 1;
                //    result.SetIsPass = false;
                //    result.SetErrorNo = ErrorNo;
                //    result.SetErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //    functionTimer.Stop();
                //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //    return result;
                //}
                switch (CheckFlowInfo.MachineQCState)
                {
                    case 0: break;
                    case 1:
                        iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCAbnormalStop, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCAbnormalStop);
                    case 2:
                        iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCReserveStop, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCReserveStop);
                    default:
                        iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCNG, FunctionName, functionTimer, Log, dbObject);
                        return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCNG);
                }
                if (CheckFlowInfo.MachineOnlineState != 1)
                {
                    //Machine_AutoMotive is not AutoMotive (Can't Setup)
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, Errors_CheckProcessFlow.MachineOnlineStateIsWorng, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineOnlineStateIsWorng);
                }
                if (CheckFlowInfo.IsAutomotive == true && CheckFlowInfo.Machine_AutoMotive != 1)
                {
                    //Machine_AutoMotive is not AutoMotive (Can't Setup)
                    iLibraryErrorAction(Errors_CheckProcessFlow.NotAutoMotive, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.NotAutoMotive);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            result.SetIsPass = true;
            result.Lot_Id = CheckFlowInfo.Id;
            result.Division_Id = CheckFlowInfo.Product_family_id;
            if (CheckFlowInfo.Is_special_flow != 0) { result.Is_SpFlow = true; }
            else { result.Is_SpFlow = false; }
            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            //SaveFunctionLog(FunctionName, , DateTime.Now);
            return result;
        }
        public CheckProcessFlowResult[] CheckProcessFlow(int State, int[] lot_id, int machine_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name; 
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            CheckProcessFlowResult[] results = null;
            LotProcessCheckInfo[] CheckFlowInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            int count = 0;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();
            try
            {
                /*-------------------Get_MachineInfo about Recipe ----------------*/

                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

                DataTable tmp0 = db.SelectData(FunctionName, dbObject, "select MD.enable_lot_max from [mc].machines as MC with(NOLOCK) inner join [mc].models as MD with(NOLOCK) on MD.id = MC.machine_model_id where MC.id = @machine_id", Log.SqlLogger, sqlparameters);

                if (tmp0 == null)
                {
                    //Datas Nothing
                    results = new CheckProcessFlowResult[1];
                    int ErrorNo = Errors_CheckProcessFlow.MachineIsNothing;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);

                    iLibraryErrorAction(Errors_CheckProcessFlow.MachineIsNothing, FunctionName, functionTimer, Log, dbObject);
                    results[0] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineIsNothing );
                    return results;
                }
                int enablelot_num = 1;
                try { enablelot_num = DBCONVERT.GetByte(tmp0.Rows[0],"enable_lot_max"); }
                catch { enablelot_num = 1; }

                if (enablelot_num < lot_id.Length)
                {
                    //Datas Nothing
                    results = new CheckProcessFlowResult[1];
                    int ErrorNo = Errors_CheckProcessFlow.ToMutchLot;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);

                    iLibraryErrorAction(Errors_CheckProcessFlow.MachineIsNothing, FunctionName, functionTimer, Log, dbObject);
                    results[0] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineIsNothing);
                    return results;
                }
                /*-------------------Get_MachineInfo about Recipe ----------------*/

                /*-------------------Get_LotInfo----------------*/
                DataTable tmp = Get_CheckFlowData(lot_id, machine_id,Log);
                if (tmp == null)
                {
                    results = new CheckProcessFlowResult[1];
                    //Datas Nothing
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                    results[0] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                    return results;
                }
                results = new CheckProcessFlowResult[lot_id.Length];
                CheckFlowInfos = new LotProcessCheckInfo[lot_id.Length];
                CheckProcessFlowResult result = new CheckProcessFlowResult();
                for (int i = 0; i < lot_id.Length; i++)
                {
                    count++;
                    LotProcessCheckInfo CheckFlowInfo = new LotProcessCheckInfo(tmp.Rows[i]);
                    CheckFlowInfos[i] = CheckFlowInfo;
                    if (CheckFlowInfo == null)
                    {
                        //Datas Nothing
                        iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                        results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                        return results;
                    }
                    if (CheckFlowInfo.Is_special_flow == 0)
                    {
                        CheckFlowInfo = new LotProcessCheckInfo(Get_CheckSpFlowData(lot_id[i], machine_id, Log));
                        if (CheckFlowInfo == null)
                        {
                            //Datas Nothing
                            iLibraryErrorAction(Errors_CheckProcessFlow.SPLotDoNotWip, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.SPLotDoNotWip);
                            return results;
                        }
                    }

                    if (CheckFlowInfo.Wip_state != 20)
                    {
                        //Lot_State is not 0 (Can't Setup)
                        iLibraryErrorAction(Errors_CheckProcessFlow.WipStateIsNotInput, FunctionName, functionTimer, Log, dbObject);
                        results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.WipStateIsNotInput);
                        return results;
                    }

                    switch (State)
                    {
                        case 0: //Setup
                            if (!(CheckFlowInfo.Process_state == 0 || CheckFlowInfo.Process_state == 100))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotSetup, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotSetup);
                                return results;
                            }
                            break;
                        case 1: //Start
                            if (!(CheckFlowInfo.Process_state == 1 || CheckFlowInfo.Process_state == 101))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotStart, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotStart);
                                return results;
                            }
                            break;
                        case 2: //End
                            if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotEnd, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotEnd);
                                return results;
                            }
                            break;
                        case 3: //Cancel
                            if (!(CheckFlowInfo.Process_state == 1 || CheckFlowInfo.Process_state == 101))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotCancel, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotCancel);
                                return results;
                            }
                            break;
                        case 4: //OnlineStart
                            if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotOnlineStart, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotOnlineStart);
                                return results;
                            }
                            break;
                        case 5: //OnlineEnd
                            if (!(CheckFlowInfo.Process_state == 2 || CheckFlowInfo.Process_state == 102))
                            {
                                //Lot_State is not 0 (Can't Setup)
                                iLibraryErrorAction(Errors_CheckProcessFlow.CanNotOnlineEnd, FunctionName, functionTimer, Log, dbObject);
                                results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.CanNotOnlineEnd);
                                return results;
                            }
                            break;
                        default:
                            iLibraryErrorAction(Errors_CheckProcessFlow.StrangeStates, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.StrangeStates);
                            return results;
                    }

                    switch (CheckFlowInfo.Quality_state)
                    {
                        case 0: break;
                        case 1:
                            iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Abnormal, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Abnormal);
                            return results;
                        case 2:
                            iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Stop, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Stop);
                            return results;
                        case 3:
                            iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_Hold, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_Hold);
                            return results;
                        case 4:
                            iLibraryErrorAction(Errors_CheckProcessFlow.LotQC_SPFlow, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQC_SPFlow);
                            return results;
                        default:
                            iLibraryErrorAction(Errors_CheckProcessFlow.LotQCNG, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotQCNG);
                            return results;
                    }
                    if (CheckFlowInfo.Package_IsEnable != 1)
                    {
                        //Package_IsEnable is not 0 (Can't Setup)
                        iLibraryErrorAction(Errors_CheckProcessFlow.PackageNotEnable, FunctionName, functionTimer, Log, dbObject);
                        results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.PackageNotEnable);
                        return results;
                    }
                    //if (CheckFlowInfo.MachineRunState != 0)
                    //{
                    //    //Machine_AutoMotive is not AutoMotive (Can't Setup)
                    //    ErrorNo = 1;
                    //    result.SetIsPass = false;
                    //    result.SetErrorNo = ErrorNo;
                    //    result.SetErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    //    functionTimer.Stop();
                    //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    //    results[i] = result;
                    //    return results;
                    //}
                    switch (CheckFlowInfo.MachineQCState)
                    {
                        case 0: break;
                        case 1:
                            iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCAbnormalStop, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCAbnormalStop);
                            return results;
                        case 2:
                            iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCReserveStop, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCReserveStop);
                            return results;
                        default:
                            iLibraryErrorAction(Errors_CheckProcessFlow.MachineQCNG, FunctionName, functionTimer, Log, dbObject);
                            results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineQCNG);
                            return results;
                    }
                    //if (CheckFlowInfo.MachineOnlineState != 1)
                    //{
                    //    //Machine_AutoMotive is not AutoMotive (Can't Setup)
                    //    functionTimer.Stop();
                    //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, Errors_CheckProcessFlow.MachineOnlineStateIsWorng, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    //    results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.MachineOnlineStateIsWorng);
                    //    return results;
                    //}
                    if (CheckFlowInfo.IsAutomotive == true && CheckFlowInfo.Machine_AutoMotive != 1)
                    {
                        //Machine_AutoMotive is not AutoMotive (Can't Setup)
                        iLibraryErrorAction(Errors_CheckProcessFlow.NotAutoMotive, FunctionName, functionTimer, Log, dbObject);
                        results[i] = ResultMaker_CheckProcessFlowResult(FunctionName, functionTimer, Errors_CheckProcessFlow.NotAutoMotive);
                        return results;
                    }
                    result.SetIsPass = true;
                    result.Lot_Id = CheckFlowInfo.Id;
                    result.Division_Id = CheckFlowInfo.Product_family_id;
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message,e);
            }
            return results;
        }

        public LotUpdateInfo LotSetup(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(0, lot_id, machine_id,Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";
            String numberName2 = "lot_pjs.process_job_id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Insert about Process_JobID----------------*/
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@product_family_id", SqlDbType.Int) { Value = ProcessFlowChecker.Division_Id };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_pjs( process_job_id, product_family_id, machine_id, online_state, created_at, started_at, started_by, finished_at, finished_by ) select @pjid, @product_family_id, MS.machine_id, MS.online_state, @date1, null, null, null, null from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't insert lot_pjs
                    dbObject.TransactionCancel();
                    dbObject.ConnectionClose();
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_LotSetup.CanNotInsertLotPjs;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@idx", SqlDbType.Int) { Value = 0 };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = ProcessFlowChecker.Lot_Id };
                okng = db.OperateData(FunctionName, dbObject, "insert [trans].pj_lots(process_job_id, idx, lot_id) select @pjid, @idx, @lot_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't insert pj_lots
                    dbObject.TransactionCancel();
                    dbObject.ConnectionClose();
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_LotSetup.CanNotInsertPjLots;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }

                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                lotinfo.Set_Name = LotDBInfos.Name;
                /*-------------------Get_LotInfo----------------*/

                /*-------------------Insert about Process_JobID----------------*/
                if (ProcessFlowChecker.Is_SpFlow == true)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotSetup_SpecialFlows(ref lotinfo, db, dbObject, latestnum2, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        dbObject.TransactionCancel();
                        dbObject.ConnectionClose();
                        lotinfo.IsOk = false;
                        int ErrorNo = Errors_LotSetup.CanNotUpdateSpecialFlowState;//ProcessFlowChecker.ErrorNo;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotSetup_NormalFlows(ref lotinfo, db, dbObject, latestnum2, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        dbObject.TransactionCancel();
                        dbObject.ConnectionClose();
                        lotinfo.IsOk = false;
                        int ErrorNo = Errors_LotSetup.CanNotUpdateLotState;//ProcessFlowChecker.ErrorNo;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            //LotStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, Log);
            //OnlineStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, Log);
            //Update_Firstinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, Log);
            //OnlineEnd(lot_id, machine_id, user_id, true, 6000, 20, DBx, XmlData, Online_state, Log);
            //Update_Lastinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, Log);
            //LotEnd(lot_id, machine_id, user_id, true, 6000, 20, DBx, XmlData, Online_state, Log);
            return lotinfo;
        }
        public LotUpdateInfo LotSetup(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            LotDBInfo LotDBInfos = null;
            if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(0, lot_id, machine_id, Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";
            String numberName2 = "lot_pjs.process_job_id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            datetime.Datetime = time; 

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Insert about Process_JobID----------------*/
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@product_family_id", SqlDbType.Int) { Value = ProcessFlowChecker.Division_Id };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_pjs( process_job_id, product_family_id, machine_id, online_state, created_at, started_at, started_by, finished_at, finished_by ) select @pjid, @product_family_id, MS.machine_id, MS.online_state, @date1, null, null, null, null from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't insert lot_pjs
                    dbObject.TransactionCancel();
                    dbObject.ConnectionClose();
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_LotSetup.CanNotInsertLotPjs;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@idx", SqlDbType.Int) { Value = 0 };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = ProcessFlowChecker.Lot_Id };
                okng = db.OperateData(FunctionName, dbObject, "insert [trans].pj_lots(process_job_id, idx, lot_id) select @pjid, @idx, @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't insert pj_lots
                    dbObject.TransactionCancel();
                    dbObject.ConnectionClose();
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_LotSetup.CanNotInsertPjLots;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }

                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                lotinfo.Set_Name = LotDBInfos.Name;
                /*-------------------Get_LotInfo----------------*/

                /*-------------------Insert about Process_JobID----------------*/
                if (ProcessFlowChecker.Is_SpFlow == true)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotSetup_SpecialFlows(ref lotinfo, db, dbObject, latestnum2, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        dbObject.TransactionCancel();
                        dbObject.ConnectionClose();
                        lotinfo.IsOk = false;
                        int Error_No = Errors_LotSetup.CanNotUpdateSpecialFlowState;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(time, Error_No, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotSetup_NormalFlows(ref lotinfo, db, dbObject, latestnum2, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        dbObject.TransactionCancel();
                        dbObject.ConnectionClose();
                        lotinfo.IsOk = false;
                        int Error_No = Errors_LotSetup.CanNotUpdateLotState;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(time, Error_No, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;
            //int pass = 6000;
            //int fail = 100;
            //int[] a = new int[1]; a[0] = 2;
            ////Update_ErrorHappenRecord(a, a[0], a[0], a[0], Log);
            //LotStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, "RecipeByCellcon", time, Log);
            //OnlineStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, time, Log);
            //ReInput(lot_id, machine_id, user_id, pass, fail, Online_state, time, Log);
            //Update_Firstinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, time, Log);
            //OnlineEnd(lot_id, machine_id, user_id, true, pass, fail, DBx, XmlData, Online_state, time, Log);
            //Update_Finalinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, time, Log);

            //AbnormalLotEnd_BackToThe_BeforeProcess(lot_id, machine_id, user_id, false, pass, fail, DBx, XmlData, Online_state, time, Log);
            //LotStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, "RecipeByCellcon", time, Log);
            //OnlineStart(lot_id, machine_id, user_id, DBx, XmlData, Online_state, time, Log);
            //Update_Firstinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, time, Log);
            //OnlineEnd(lot_id, machine_id, user_id, true, pass, fail, DBx, XmlData, Online_state, time, Log);
            //Update_Finalinspection(lot_id, 1, user_id, DBx, XmlData, Online_state, time, Log);

            ////LotEnd_FirstProcess(lot_id, machine_id, user_id, false, pass, fail, DBx, XmlData, Online_state, time, Log);
            //LotEnd(lot_id, machine_id, user_id, false, pass, fail, DBx, XmlData, Online_state, time, Log);
            return lotinfo;
        }
        public LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, String Recipe, Logger Log, int Location_Num = 1, int act_pass_qty = -1)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id,Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotStart_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log, act_pass_qty, Location_Num);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = LotStart_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Recipe, Log, act_pass_qty, Location_Num);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo LotStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, String Recipe, DateTime time, Logger Log, int Location_Num = 1, int act_pass_qty = -1)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id, Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotStart_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log, act_pass_qty, Location_Num);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = LotStart_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Recipe, Log, act_pass_qty, Location_Num);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo OnlineStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);


            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber----------------*/

            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = OnlineStart_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = OnlineStart_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo OnlineStart(int lot_id, int machine_id, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);


            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber----------------*/

            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = OnlineStart_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = OnlineStart_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo LotCancel(int lot_id, int machine_id, int user_id, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            if (LotDBInfos.Errorflag == true) { throw new Exception(LotDBInfos.ErrorMessage); }
            /*-------------------Get_LotInfo----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotCancel_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotCancel_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            functionTimer = null;
            dbObject = null;
            db = null;

            return lotinfo;
        }
        public LotUpdateInfo LotCancel(int lot_id, int machine_id, int user_id, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(3, lot_id, machine_id, Log);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            if (LotDBInfos.Errorflag == true) { throw new Exception(LotDBInfos.ErrorMessage); }
            /*-------------------Get_LotInfo----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotCancel_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotCancel_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            functionTimer = null;
            dbObject = null;
            db = null;

            return lotinfo;
        }

        public LotUpdateInfo ReInput(int lot_id, int machine_id, int user_id, int qty_pass, int qty_fail, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger(); 
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id, Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            if (LotDBInfos.Errorflag == true) { throw new Exception(LotDBInfos.ErrorMessage); }
            /*-------------------Get_LotInfo----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Reinput_SpecialFlows(ref lotinfo, db, dbObject, qty_pass, qty_fail, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Reinput_NormalFlows(ref lotinfo, db, dbObject, qty_pass, qty_fail, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            functionTimer = null;
            dbObject = null;
            db = null;

            return lotinfo;
        }

        public LotUpdateInfo LotEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(2, lot_id, machine_id, Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            /*-------------------Get_LotInfo----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";
            int latestnum1 = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log)) { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 2, Log.SqlLogger); }
            else { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger); }
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            dbObject.TransactionCommit();
            dbObject.BeginTransaction();

            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}

            /*-------------------Jig_Check/Update----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }

                    okng = LotEnd_SpecialFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotEnd_NormalFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                //int ErrorNo = 1;
                //lotinfo.ErrorNo = ErrorNo;
                //lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //return lotinfo;
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo LotEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(2, lot_id, machine_id, Log);
            if (ProcessFlowChecker.IsPass != true)
            {
                lotinfo.IsOk = false;
                int ErrorNo = ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                return lotinfo;
            }
            /*-------------------Check_processFlow----------------*/

            /*-------------------Get_DateTime----------------*/
            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            /*-------------------Get_DateTime----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            if (Check_InspectionState(db, dbObject, ref LotDBInfos, time, Log) == false)
            {
                dbObject.TransactionCancel();
                int ErrorNo = Errors_LotEnd.LotDidNotInspection;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                return lotinfo;
            }
            /*-------------------Get_LotInfo----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";
            int latestnum1 = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log)) { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 2, Log.SqlLogger); }
            else { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger); }
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            dbObject.TransactionCommit();
            dbObject.BeginTransaction();

            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}

            /*-------------------Jig_Check/Update----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }

                    okng = LotEnd_SpecialFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }
                    /*-------------------Update SpecilLotState----------------*/
                    okng = LotEnd_NormalFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                //int ErrorNo = 1;
                //lotinfo.ErrorNo = ErrorNo;
                //lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //functionTimer.Stop();
                //iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                //return lotinfo;
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }

        //public LotUpdateInfo LotEnd_QtyAdding(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int Hasuu, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        //{
        //    String FunctionName = MethodBase.GetCurrentMethod().Name;
        //    Stopwatch functionTimer = new Stopwatch();
        //    DatabaseAccess db = new DatabaseAccess();
        //    DatabaseAccessObject dbObject = new DatabaseAccessObject();
        //    LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
        //    if (Log == null) { Log = new Logger(); }
        //    int okng = -1;

        //    Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
        //    functionTimer.Start();

        //    /*-------------------Check_processFlow----------------*/
        //    CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(2, lot_id, machine_id, Log);
        //    if (ProcessFlowChecker.IsPass != true)
        //    {
        //        lotinfo.IsOk = false;
        //        int ErrorNo = ProcessFlowChecker.ErrorNo;
        //        lotinfo.ErrorNo = ErrorNo;
        //        lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
        //        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //        return lotinfo;
        //    }
        //    /*-------------------Check_processFlow----------------*/

        //    /*-------------------Get_DateTime----------------*/
        //    dbObject.ConnectionOpen();
        //    dbObject.BeginTransaction();

        //    DateTimeInfo datetime = Get_DateTimeInfo(Log);
        //    datetime.Datetime = time;
        //    /*-------------------Get_DateTime----------------*/

        //    /*-------------------Get_LotInfo----------------*/
        //    DataRow tmp = Get_LotDBData(lot_id, Log);
        //    if (tmp == null)
        //    {
        //        lotinfo.IsOk = false;
        //        int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
        //        lotinfo.ErrorNo = ErrorNo;
        //        lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
        //        functionTimer.Stop();
        //        Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
        //        return lotinfo;
        //    }
        //    LotDBInfo LotDBInfos = new LotDBInfo(tmp);
        //    if (Check_InspectionState(db, dbObject, LotDBInfos, time, Log) == false)
        //    {
        //        dbObject.TransactionCancel();
        //        int ErrorNo = 00;
        //        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //        return lotinfo;
        //    }
        //    /*-------------------Get_LotInfo----------------*/

        //    /*-------------------Increment RecordNumber and PJ_Number----------------*/
        //    String numberName1 = "lot_process_records.id";
        //    int latestnum1 = -1;
        //    if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log)) { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 2, Log.SqlLogger); }
        //    else { latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger); }
        //    /*-------------------Increment RecordNumber and PJ_Number----------------*/

        //    /*-------------------Jig_Check/Update----------------*/
        //    //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
        //    /*-------------------Increment RecordNumber and PJ_Number----------------*/
        //    //String numberName2 = "jig_condition_records.device_slip_id";

        //    //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());
        //    /*-------------------Increment RecordNumber and PJ_Number----------------*/
        //    dbObject.TransactionCommit();
        //    dbObject.BeginTransaction();

        //    //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
        //    //bool checkflag = false;
        //    //for (int i = 0; i < JigResult.Count(); i++)
        //    //{
        //    //    if (JigResult[i].IsPass == false) { checkflag = true; }
        //    //}
        //    //if (checkflag == true)
        //    //{
        //    //    //OverLimit/Worning Jig Exist(Can't Setup)
        //    //    int ErrorNo = 1;
        //    //    lotinfo.ErrorNo = ErrorNo;
        //    //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
        //    //    functionTimer.Stop();
        //    //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //    //    return lotinfo;
        //    //}

        //    /*-------------------Jig_Check/Update----------------*/

        //    try
        //    {
        //        if (LotDBInfos.Is_special_flow != 0)
        //        {
        //            /*-------------------Update SpecilLotState----------------*/
        //            int end_mode = -1;
        //            if (isAbnormal == false) { end_mode = 0; }
        //            else { end_mode = 3; }

        //            okng = LotEnd_QtyAdding_SpecialFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, Hasuu, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
        //            if (okng == -1)
        //            {
        //                int ErrorNo = 00;
        //                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //                return lotinfo;
        //            }
        //            /*-------------------Update LotState----------------*/
        //        }
        //        else
        //        {
        //            int end_mode = -1;
        //            if (isAbnormal == false) { end_mode = 0; }
        //            else { end_mode = 3; }
        //            /*-------------------Update SpecilLotState----------------*/
        //            okng = LotEnd_QtyAdding_NormalFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, Hasuu, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
        //            if (okng == -1)
        //            {
        //                int ErrorNo = 00;
        //                iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //                return lotinfo;
        //            }
        //            /*-------------------Update LotState----------------*/
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Can't insert pj_lots
        //        //int ErrorNo = 1;
        //        //lotinfo.ErrorNo = ErrorNo;
        //        //lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
        //        //functionTimer.Stop();
        //        //iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
        //        //return lotinfo;
        //        dbObject.TransactionCancel();
        //        dbObject.ConnectionClose();
        //        throw new Exception(e.Message,e);
        //    }
        //    dbObject.TransactionCommit();
        //    dbObject.ConnectionClose();

        //    functionTimer.Stop();
        //    Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

        //    functionTimer = null;
        //    dbObject = null;
        //    db = null;
        //    lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
        //    lotinfo.IsOk = true;
        //    lotinfo.ErrorNo = -1;
        //    lotinfo.ErrorMessage = null;
        //    lotinfo.Step_No = LotDBInfos.Step_no;
        //    lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
        //    lotinfo.Start_Time = datetime.Datetime;
        //    lotinfo.Machine_id = machine_id;
        //    lotinfo.Input_Qty = LotDBInfos.Qty_in;
        //    lotinfo.User = user_id;

        //    return lotinfo;
        //}

        public LotUpdateInfo OnlineEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(2, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            /*-------------------Get_LotInfo----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());
            /*-------------------Increment RecordNumber and PJ_Number----------------*/


            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}

            /*-------------------Jig_Check/Update----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }

                    okng = OnlineEnd_SpecialFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }
                    /*-------------------Update SpecilLotState----------------*/
                    okng = OnlineEnd_NormalFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                //int ErrorNo = 1;
                //lotinfo.ErrorNo = ErrorNo;
                //lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //return lotinfo;
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo OnlineEnd(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(2, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id,Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            /*-------------------Get_LotInfo----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());
            /*-------------------Increment RecordNumber and PJ_Number----------------*/


            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}

            /*-------------------Jig_Check/Update----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }

                    okng = OnlineEnd_SpecialFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    int end_mode = -1;
                    if (isAbnormal == false) { end_mode = 0; }
                    else { end_mode = 3; }
                    /*-------------------Update SpecilLotState----------------*/
                    okng = OnlineEnd_NormalFlows(ref lotinfo, end_mode, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                //int ErrorNo = 1;
                //lotinfo.ErrorNo = ErrorNo;
                //lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //functionTimer.Stop();
                //iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                //return lotinfo;
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo Update_Firstinspection(int lot_id, int Ins_result, int user_id, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Update_FirstInspectionSpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = Update_FirstInspectionNormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            //lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo Update_Firstinspection(int lot_id, int Ins_result, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Update_FirstInspectionSpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = Update_FirstInspectionNormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            //lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo Update_Finalinspection(int lot_id, int Ins_result, int user_id, int DBx, String XmlData, int Online_state, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Update_FinalInspectionSpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = Update_FinalInspectionNormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        functionTimer.Stop();
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            //lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public LotUpdateInfo Update_Finalinspection(int lot_id, int Ins_result, int user_id, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            LotDBInfo LotDBInfos = null;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Check_processFlow----------------*/
            //CheckProcessFlowResult ProcessFlowChecker = CheckProcessFlow(1, lot_id, machine_id);
            //if (ProcessFlowChecker.IsPass != true)
            //{
            //    lotinfo.IsOk = false;
            //    int ErrorNo = ProcessFlowChecker.ErrorNo;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ProcessFlowChecker.ErrorMessage;
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Check_processFlow----------------*/

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Jig_Check/Update----------------*/
            //int[] Jigids = Get_JigIdsbyMachine(machine_id, lot_id);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //String numberName2 = "jig_condition_records.device_slip_id";

            //int latestnum2 = CountUp_Numbers(numberName2, Jigids.Count());

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            //CheckJigResult[] JigResult = Check_Jigs(Jigids, false);
            //bool checkflag = false;
            //for (int i = 0; i < JigResult.Count(); i++)
            //{
            //    if (JigResult[i].IsPass == false) { checkflag = true; }
            //}
            //if (checkflag == true)
            //{
            //    //OverLimit/Worning Jig Exist(Can't Setup)
            //    int ErrorNo = 1;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    functionTimer.Stop();
            //    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
            //    return lotinfo;
            //}
            /*-------------------Jig_Check/Update----------------*/
            try
            {
                /*-------------------Get_LotInfo----------------*/
                DataRow tmp = Get_LotDBData(lot_id,Log);
                if (tmp == null)
                {
                    lotinfo.IsOk = false;
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return lotinfo;
                }
                LotDBInfos = new LotDBInfo(tmp);
                /*-------------------Get_LotInfo----------------*/

                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = Update_FinalInspectionSpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update NormalLotState----------------*/
                    okng = Update_FinalInspectionNormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, Ins_result, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            //lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }
        public MachineUpdateInfo Update_ErrorHappenRecord(int[] lot_id, MachineInfo machine, int user_id, String ErrorCode, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
   
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber----------------*/
            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber----------------*/

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();

            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 2, MS.updated_at = @date from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state) from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_cod", SqlDbType.Int) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id, updated_at, machine_id, model_alarm_id, alarm_on_at, alarm_off_at, started_at)select @record_id, @updae_date, @machine_id, MA.id, @date, '', '' from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }
                if (lot_id != null)
                {
                    for (int i = 0; i < lot_id.Length; i++)
                    {
                        sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                        sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                        sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id[i] };

                        okng = -1;

                        okng = db.OperateData(FunctionName, dbObject, "insert [trans].alarm_lot_records(id, lot_id)select @record_id, @lot_id ", Log.SqlLogger, sqlparameters);

                        if (okng < 1)
                        {
                            //Can't Insert MachineRecords
                            int ErrorNo = 204;
                            info.ErrorNo = ErrorNo;
                            info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                            functionTimer.Stop();
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                            return info;
                        }
                    }
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }
        public MachineUpdateInfo Update_ErrorHappenRecord(int[] lot_id, MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
           
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();

            try
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 2, MS.updated_at = @date from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_code", SqlDbType.VarChar) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id, updated_at, machine_id, model_alarm_id, alarm_on_at, alarm_off_at, started_at)select @record_id, @updae_date, @machine_id, MA.id, @date, '', '' from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                if (lot_id != null)
                {
                    for (int i = 0; i < lot_id.Length; i++)
                    {
                        sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                        sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                        sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id[i] };

                        okng = -1;

                        okng = db.OperateData(FunctionName, dbObject, "insert [trans].alarm_lot_records(id, lot_id)select @record_id, @lot_id ", Log.SqlLogger, sqlparameters);

                        if (okng < 1)
                        {
                            //Can't Insert MachineRecords
                            int ErrorNo = 204;
                            info.ErrorNo = ErrorNo;
                            info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                            iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                            return info;
                        }
                    }
                }
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }
        public MachineUpdateInfo Update_ErrorResetRecord(MachineInfo machine, int user_id, String ErrorCode, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
              
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber----------------*/
            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber----------------*/

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            
            try
            {
                
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 0, MS.updated_at = @date from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state ) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_cod", SqlDbType.VarChar) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id,updated_at,machine_id,model_alarm_id,alarm_on_at,alarm_off_at,started_at)select @record_id, @updae_date, @machine_id,  MA.id, '', @date, '' from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }
        public MachineUpdateInfo Update_ErrorResetRecord(MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
       
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber----------------*/
            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber----------------*/

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            
            try
            {
                
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 0 from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state ) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_code", SqlDbType.VarChar) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id,updated_at,machine_id,model_alarm_id,alarm_on_at,alarm_off_at,started_at)select @record_id, @updae_date, @machine_id,  MA.id, '', @date, '' from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }
        public MachineUpdateInfo Update_ErrorRecoveryRecord(MachineInfo machine, int user_id, String ErrorCode, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
           
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber----------------*/
            String numberName1 = "lot_process_records.id";
            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber----------------*/

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            
            try
            {
                
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 0 from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state ) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_code", SqlDbType.VarChar) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id,updated_at,machine_id,model_alarm_id,alarm_on_at,alarm_off_at,started_at)select @record_id, @updae_date, @machine_id,  MA.id, '', '', @date from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters );

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return info;
                }

                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }
        public MachineUpdateInfo Update_ErrorRecoveryRecord(MachineInfo machine, int user_id, String ErrorCode, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            MachineUpdateInfo info = new MachineUpdateInfo(machine.Id, "");
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
          
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber----------------*/
            String numberName2 = "machine_state_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum2 = CountUp_Numbers(db, dbObject, numberName2, 1, Log.SqlLogger);
            
            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            
            try
            {
               
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = 0 from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state) from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[6];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum2 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@updae_date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine.Id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@error_cod", SqlDbType.VarChar) { Value = ErrorCode };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine.MachineModel.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_alarm_records(id,updated_at,machine_id,model_alarm_id,alarm_on_at,alarm_off_at,started_at)select @record_id, @updae_date, @machine_id, @error_cod, '', '', @date from mc.model_alarms as MA with(nolock) where MA.alarm_code = @error_code and MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert MachineRecords
                    int ErrorNo = 204;
                    info.ErrorNo = ErrorNo;
                    info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                    return info;
                }

                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            info = new MachineUpdateInfo(machine.Id, "");
            info.IsOk = true;
            info.ErrorNo = -1;
            info.ErrorMessage = null;
            info.User = user_id;

            return info;
        }

        public List<AlarmInfoObject> Get_MachineAlarmCounter(int lot_id, MachineInfo machine_info, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Log = Log ?? new Logger();
            DB_GetConverter c_DBCONVERT = new DB_GetConverter();

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;
            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");

            List<AlarmInfoObject> Results = new List<AlarmInfoObject>();
            functionTimer.Start();

            /*-------------------Function Start----------------*/
            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            try
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@model_id", SqlDbType.Int) { Value = machine_info.MachineModel.Id };

                okng = -1;

                DataTable Tmp1 = db.SelectData(FunctionName, dbObject, "select MA.id, MA.alarm_code from mc.model_alarms as MA with(nolock)where MA.machine_model_id = @model_id", Log.SqlLogger, sqlparameters);

                if (Tmp1 == null || Tmp1.Rows.Count < 1)
                {
                    //Can't Insert MachineRecords
                    Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Results;
                }
                foreach(DataRow Tmp1_1 in Tmp1.Rows)
                {
                    AlarmInfoObject tmp = new AlarmInfoObject(machine_info.Id, c_DBCONVERT.GetInt32(Tmp1_1, "id"), 0);
                    tmp.Pass = true;
                    Results.Add(tmp);
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_info.Id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

                DataTable Tmp2 = db.SelectData(FunctionName, dbObject, "select MA.id, AT.alarm_text from trans.alarm_lot_records as ALR with(nolock) inner join trans.machine_alarm_records as MAR with(nolock) on MAR.id = ALR.id inner join mc.model_alarms as MA with(nolock) on MA.id = MAR.model_alarm_id inner join mc.alarm_texts as AT with(nolock) on AT.alarm_text_id = MA.alarm_text_id where MAR.machine_id = @machine_id and ALR.lot_id = @lot_id", Log.SqlLogger, sqlparameters);

                if (Tmp2 == null || Tmp2.Rows.Count < 1)
                {
                    //Can't Insert MachineRecords
                    Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Results;
                }

                foreach (DataRow Tmp2_1 in Tmp2.Rows)
                {
                    AlarmCountUpper(ref Results, c_DBCONVERT.GetInt32(Tmp2_1, "id"));
                }
                
                dbObject.TransactionCommit();
                dbObject.ConnectionClose();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message, e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;

            return Results;
        }

        private void AlarmCountUpper(ref List<AlarmInfoObject> Results, int id )
        {
            for (int i = 0; i < Results.Count; i++)
            {
                if(Results[i].Model_alarm_id == id) { Results[i].Alarm_count += 1; continue; }
            }
        }

        public LotUpdateInfo AbnormalLotHold(int lot_id, int machine_id, int user_id, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id, Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);
            if (LotDBInfos.Errorflag == true) { throw new Exception(LotDBInfos.ErrorMessage); }
            /*-------------------Get_LotInfo----------------*/

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = AbnormalLotHold_SpecialFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = AbnormalLotHold_NormalFlows(ref lotinfo, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, 0, "", Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 00;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                dbObject.TransactionCommit();
            }
            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            functionTimer = null;
            dbObject = null;
            db = null;

            return lotinfo;
        }

        public LotUpdateInfo AbnormalLotEnd_BackToThe_BeforeProcess(int lot_id, int machine_id, int user_id, bool isAbnormal, int pass_qty, int fail_qty, int DBx, String XmlData, int Online_state, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            LotUpdateInfo lotinfo = new LotUpdateInfo(lot_id, "");
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            int okng = -1;

            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "lot_process_records.id";

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            DateTimeInfo datetime = Get_DateTimeInfo(Log);
            datetime.Datetime = time;
            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            /*-------------------Get_LotInfo----------------*/
            DataRow tmp = Get_LotDBData(lot_id, Log);
            if (tmp == null)
            {
                lotinfo.IsOk = false;
                int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;//ProcessFlowChecker.ErrorNo;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return lotinfo;
            }
            LotDBInfo LotDBInfos = new LotDBInfo(tmp);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();

            try
            {
                if (LotDBInfos.Is_special_flow != 0)
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = AbnormalLotEnd_SpecialFlows(ref lotinfo, 100, isAbnormal, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 1101;
                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
                else
                {
                    /*-------------------Update SpecilLotState----------------*/
                    okng = AbnormalLotEnd_NormalFlows(ref lotinfo, 100, isAbnormal, pass_qty, fail_qty, LotDBInfos.Qty_fail, db, dbObject, LotDBInfos.Pj_id, latestnum1, Online_state, machine_id, LotDBInfos, datetime, user_id, DBx, XmlData, Log);
                    if (okng == -1)
                    {
                        int ErrorNo = 1104;

                        iLibraryErrorAction(ErrorNo, FunctionName, functionTimer, Log, dbObject);
                        return lotinfo;
                    }
                    /*-------------------Update LotState----------------*/
                }
            }
            catch (Exception e)
            {
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(time, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            lotinfo = new LotUpdateInfo(LotDBInfos.Id, LotDBInfos.Name);
            lotinfo.IsOk = true;
            lotinfo.ErrorNo = -1;
            lotinfo.ErrorMessage = null;
            lotinfo.Step_No = LotDBInfos.Step_no;
            lotinfo.Ticket_ID = LotDBInfos.Ticket_id;
            lotinfo.Start_Time = datetime.Datetime;
            lotinfo.Machine_id = machine_id;
            lotinfo.Input_Qty = LotDBInfos.Qty_in;
            lotinfo.User = user_id;

            return lotinfo;
        }

        public DateTime Get_SQLServerTime(Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            try
            {
                Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
                functionTimer.Start();

                dbObject.ConnectionOpen();
                DataTable tabledatetimeinfo = db.SelectData(FunctionName, dbObject, "select DAYS.id ,getdate() from[trans].days as DAYS with(nolock) where DAYS.date_value = convert(date, getdate())" , Log.SqlLogger);
                dbObject.ConnectionClose();

                datetime_info.DayId = int.Parse(tabledatetimeinfo.Rows[0].ItemArray[0].ToString());
                datetime_info.Datetime = DateTime.Parse(tabledatetimeinfo.Rows[0].ItemArray[1].ToString());
                datetime_info.Errorflag = false;
            }
            catch (Exception e)
            {
                //datetime_info.Errorflag = true;
                //datetime_info.Errormessage = e.ToString();
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            functionTimer = null;
            dbObject = null;
            db = null;
            datetime_info = null;

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return datetime_info.Datetime;
        }

        #region OTHER

        public int Update_MachineState(int machine_id, int run_state, int user_id, Logger Log)
        {
            //Run_State → 0:
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();
            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = run_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = @state, MS.updated_at = @date from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public int Update_MachineState(ref LotUpdateInfo lotinfo, int machine_id, int run_state, int user_id, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = run_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.run_state = @state, MS.updated_at = @date from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Update MachineRecords
                int ErrorNo = 1;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", lotinfo.ErrorMessage, "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public int Update_MachineOnlineState(int machine_id, int online_state, int user_id, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", ""); Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = online_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            string txt = "update MS set MS.online_state = @state, MS.updated_at = @date, MS.onlined_at = @date";

            if (online_state == 1) { txt = txt + ", MS.run_state = 0"; }

            okng = db.OperateData(FunctionName, dbObject, txt + " from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );
            if (okng < 1)
            {
                //Can't Update MachineRecords
                int ErrorNo = 1;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public int Update_MachineOnlineState(ref LotUpdateInfo lotinfo, int machine_id, int online_state, int user_id, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = online_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.online_state = @state, MS.updated_at = @date, MS.onlined_at = @date from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Update MachineRecords
                int ErrorNo = 1;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", lotinfo.ErrorMessage, "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public int Update_MachineQualityState(int machine_id, int quality_state, int user_id, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", ""); Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = quality_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.qc_state = @state, MS.updated_at = @date from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);
            if (okng < 1)
            {
                //Can't Update MachineRecords
                int ErrorNo = 1;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                //info.ErrorNo = ErrorNo;
                //info.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public int Update_MachineQualityState(ref LotUpdateInfo lotinfo, int machine_id, int quality_state, int user_id, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------Increment RecordNumber and PJ_Number----------------*/
            String numberName1 = "machine_state_records.id";

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            dbObject.BeginTransaction();

            int latestnum1 = CountUp_Numbers(db, dbObject, numberName1, 1, Log.SqlLogger);

            dbObject.TransactionCommit();
            dbObject.BeginTransaction();
            /*-------------------Increment RecordNumber and PJ_Number----------------*/

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = quality_state };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.qc_state = @state from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Update MachineRecords
                int ErrorNo = 1;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", lotinfo.ErrorMessage, "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = latestnum1 };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].machine_state_records(id ,day_id ,updated_at ,operated_by ,machine_id ,online_state ,run_state ,qc_state ,check_state) select @record_id ,@day_id ,@date ,@user_id ,MS.machine_id ,MS.online_state ,MS.run_state ,MS.qc_state ,MS.check_state from [trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", lotinfo.ErrorMessage, "");
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                functionTimer = null;
                return okng;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            dbObject.TransactionCommit();
            dbObject.ConnectionClose();
            functionTimer = null;
            return okng;
        }
        public DateTimeInfo Get_DateTimeInfo(Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DateTimeInfo datetime_info = new DateTimeInfo();
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            try
            {
                Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
                functionTimer.Start();

                dbObject.ConnectionOpen();
                DataTable tabledatetimeinfo = db.SelectData(FunctionName, dbObject, "select DAYS.id ,getdate() from[trans].days as DAYS with(nolock) where DAYS.date_value = convert(date, getdate())", Log.SqlLogger);
                dbObject.ConnectionClose();

                datetime_info.DayId = int.Parse(tabledatetimeinfo.Rows[0].ItemArray[0].ToString());
                datetime_info.Datetime = DateTime.Parse(tabledatetimeinfo.Rows[0].ItemArray[1].ToString());
                datetime_info.Errorflag = false;
            }
            catch (Exception e)
            {
                //datetime_info.Errorflag = true;
                //datetime_info.Errormessage = e.ToString();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            return datetime_info;
        }
        public DateTimeInfo Get_DateTimeInfo(DatabaseAccess db, DatabaseAccessObject dbObject, Logger Log)
        {
            string FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DateTimeInfo datetime_info = new DateTimeInfo();
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            try
            {
                Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
                functionTimer.Start();

                dbObject.ConnectionOpen();
                DataTable tabledatetimeinfo = db.SelectData(FunctionName, dbObject, "select DAYS.id ,getdate() from[trans].days as DAYS with(nolock) where DAYS.date_value = convert(date, getdate())", Log.SqlLogger);
                dbObject.ConnectionClose();

                datetime_info.DayId = int.Parse(tabledatetimeinfo.Rows[0].ItemArray[0].ToString());
                datetime_info.Datetime = DateTime.Parse(tabledatetimeinfo.Rows[0].ItemArray[1].ToString());
                datetime_info.Errorflag = false;
            }
            catch (Exception e)
            {
                //datetime_info.Errorflag = true;
                //datetime_info.Errormessage = e.ToString();
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");

            functionTimer = null;
            dbObject = null;
            db = null;
            return datetime_info;
        }
        public Info[] Get_EnabledPackageList(int product_family_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            Info[] Package_infos;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@product_family_id", SqlDbType.Int) { Value = product_family_id };

                SaveFunctionLog(FunctionName, 0, DateTime.Now);
                functionTimer.Start();

                dbObject.ConnectionOpen();
                DataTable PackageInfoTable = db.SelectData(FunctionName, dbObject, "select PK.id, PK.name from[method].packages as PK with(nolock) where PK.product_family_id = @product_family_id and PK.is_enabled = 1", Log.SqlLogger, sqlparameters );
                dbObject.ConnectionClose();

                Package_infos = new Info[PackageInfoTable.Rows.Count];
                for (int i = 0; i < PackageInfoTable.Rows.Count; i++)
                {
                    Info Package_info = new Info(int.Parse(PackageInfoTable.Rows[0].ItemArray[0].ToString()), PackageInfoTable.Rows[0].ItemArray[1].ToString());
                    Package_infos[i] = Package_info;
                }
            }
            catch (Exception e)
            {
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return Package_infos;
        }
        public Boolean CheckPackageEnable(String packageName,Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            try
            {
                SaveFunctionLog(FunctionName, 0, DateTime.Now);
                functionTimer.Start();

                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_name", SqlDbType.VarChar) { Value = packageName };

                dbObject.ConnectionOpen();
                int flag = db.SelectExist(FunctionName, dbObject, "select PK.id, PK.name from[method].packages as PK with(nolock) where PK.name = @package_name and PK.is_enabled = 1", Log.SqlLogger, sqlparameters);
                dbObject.ConnectionClose();

               if(flag < 1)
                {
                    int ErrorNo = 00;
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return false;
                }
            }
            catch (Exception e)
            {
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return true;
        }

        public Boolean CheckLotisExist(String LotName, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            try
            {
                SaveFunctionLog(FunctionName, 0, DateTime.Now);
                functionTimer.Start();

                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_no", SqlDbType.VarChar) { Value = LotName };

                dbObject.ConnectionOpen();
                int flag = db.SelectExist(FunctionName, dbObject, "select * from [trans].lots as LO with(nolock) where LO.lot_no = @lot_no", Log.SqlLogger, sqlparameters);
                dbObject.ConnectionClose();

                if (flag < 1)
                {
                    int ErrorNo = 00;
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return false;
                }
            }
            catch (Exception e)
            {
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message, e);
            }

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return true;
        }

        public int CountUp_Numbers(DatabaseAccess db, DatabaseAccessObject dbObject, string name, int num, LoggerObject Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new LoggerObject();
            //if (Log == null) { Log = new LoggerObject(); }

            SaveFunctionLog("GetServerTime", 0, DateTime.Now);
            functionTimer.Start();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
            DataTable tablenumber = db.SelectData(FunctionName, dbObject, "select NUM.id from[trans].numbers as NUM with(xlock)where NUM.name = @name ", Log, sqlparameters );

            try
            {
                if (tablenumber == null || tablenumber.Rows.Count < 1)
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@num", SqlDbType.Int) { Value = num };
                    int okng = db.OperateData(FunctionName, dbObject, "insert [trans].numbers(name, id) select @name, @num", Log, sqlparameters );

                    dbObject.TransactionCommit();
                    dbObject.ConnectionClose();
                    functionTimer.Stop();
                    Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "0 (It is new one)", "");
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    return num;
                }
                else
                {
                    int  firstnum = int.Parse(tablenumber.Rows[0].ItemArray[0].ToString());
                    sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@num", SqlDbType.Int) { Value = num };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
                    int okng = db.OperateData(FunctionName, dbObject, "update NUM set NUM.id = NUM.id + @num from[trans].numbers as NUM where NUM.name = @name", Log, sqlparameters );

                    functionTimer.Stop();
                    Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", (firstnum + num).ToString(), "");
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;


                    return int.Parse(tablenumber.Rows[0].ItemArray[0].ToString());
                }
            }
            catch (Exception e)
            {
                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;


                //return -1;
                throw new Exception(e.Message,e);
            }
        }

        public int Update_Lot_ProcessJobs(DatabaseAccess db, DatabaseAccessObject dbObject, int pjid, int user_id, DateTime date, bool Is_Start, Logger Log )
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog(FunctionName, 0, DateTime.Now);
            functionTimer.Start();

            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pjid };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = date };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                int okng = -1;
                if (Is_Start)
                {
                    okng = db.OperateData(FunctionName, dbObject, "update [trans].lot_pjs Set started_at = @date, started_by = @user_id where[trans].lot_pjs.process_job_id = @pjid", Log.SqlLogger, sqlparameters);
                }
                else
                {
                    okng = db.OperateData(FunctionName, dbObject, "update [trans].lot_pjs Set finished_at = @date, finished_by = @user_id where[trans].lot_pjs.process_job_id = @pjid", Log.SqlLogger, sqlparameters);
                }

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 505;

                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                    return okng;
                }
                functionTimer.Stop();
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                functionTimer = null;
                sqlparameters = null;

                return okng;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message,e);
            }
        }

        //public int CountUp_Numbers(DatabaseAccess db, DatabaseAccessObject dbObject, string name, int num, LoggerObject Log)
        //{
        //    String FunctionName = MethodBase.GetCurrentMethod().Name;
        //    Stopwatch functionTimer = new Stopwatch();
        //    System.Data.SqlClient.SqlParameter[] sqlparameters;

        //    Log.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", name + "-> + " + num.ToString(), "");
        //    functionTimer.Start();

        //    sqlparameters = new System.Data.SqlClient.SqlParameter[1];
        //    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
        //    DataTable tablenumber = db.SelectData(FunctionName, dbObject, "select NUM.id from[trans].numbers as NUM with(ROWLOCK,XLOCK) where NUM.name = @name ", Log, sqlparameters );

        //    try
        //    {
        //        if (tablenumber == null || tablenumber.Rows.Count == 0)
        //        {
        //            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
        //            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
        //            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@num", SqlDbType.Int) { Value = num };
        //            int okng = db.OperateData(FunctionName, dbObject, "insert [trans].numbers(name, id) select @name, @num",Log , sqlparameters );

        //            functionTimer.Stop();
        //            Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "0 (It is new one)", "");
        //            functionTimer = null;
        //            dbObject = null;
        //            db = null;
        //            sqlparameters = null;

        //            return num;
        //        }
        //        else
        //        {
        //            tablenumber.Read();
        //            int firstnum = int.Parse(tablenumber.GetValue(0).ToString());
        //            tablenumber.Close();

        //            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
        //            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@num", SqlDbType.Int) { Value = num };
        //            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@name", SqlDbType.VarChar) { Value = name };
        //            int okng = db.OperateData(FunctionName, dbObject, "update NUM set NUM.id = NUM.id + @num from[trans].numbers as NUM where NUM.name = @name",Log , sqlparameters );

        //            functionTimer.Stop();
        //            Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", (firstnum + num).ToString(), "");
        //            functionTimer = null;
        //            dbObject = null;
        //            db = null;
        //            sqlparameters = null;

        //            return firstnum + num;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //functionTimer.Stop();
        //        //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
        //        dbObject.TransactionCancel();
        //        dbObject = null;
        //        db = null;
        //        //functionTimer = null;
        //        //dbObject = null;
        //        //db = null;
        //        //sqlparameters = null;
        //        //return -1;
        //        throw new Exception(e.Message,e);
        //    }
        //}

        public DataTable Get_LotInfos(int[] ids, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_LotInfo", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String in_id = null;
            for (int i = 0; i < ids.Length; i++)
            {
                in_id = in_id + ids.ToString();
                if (i < ids.Length - 1)
                {
                    in_id = in_id + ",";
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = in_id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, DN.is_automotive, DF.job_id, LO.process_state, LO.device_slip_id, LO.step_no, LO.act_package_id, LO.product_family_id, LO.quality_state, LO.is_special_flow, LO.location_id, LO.qty_in, LO.qty_pass, LO.qty_fail from[trans].lots as LO with(NOLOCK) inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id left outer join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id  IN(@lots_id)", Log.SqlLogger, sqlparameters );

            try
            {
                if (LotDataTable == null)
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    functionTimer.Stop();
                    int ErrorNo = 00;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return LotDataTable;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;

                //functionTimer.Stop();
                //Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //return null;
                throw new Exception(e.Message,e);
            }
        }
        public DataRow Get_LotDBData(int id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_LotDBData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, DN.is_automotive, DF.job_id, LO.process_state, LO.device_slip_id, LO.step_no, LO.act_package_id, LO.product_family_id, LO.quality_state, LO.wip_state, LO.is_special_flow, LO.location_id, LO.process_job_id, LO.qty_in, LO.qty_pass, LO.qty_fail, LO.first_ins_state, LO.final_ins_state from[trans].lots as LO with(NOLOCK) inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id left outer join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id = @lots_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (LotDataTable == null)
                {
                    int ErrorNo = 00;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (LotDataTable.Rows.Count == 0)
                {
                    int ErrorNo = 00;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (LotDataTable.Rows.Count > 1)
                {
                    int ErrorNo = 00;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    return LotDataTable.Rows[0];
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }
        public DataRow Get_CheckFlowData(int lot_id, int machine_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            functionTimer.Start();
            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select * from(select LO.id, LO.lot_no, LO.is_special_flow, LO.location_id, DF.job_id, LO.step_no, LO.act_package_id, LO.product_family_id, LO.process_state, DF.device_slip_id, LO.process_job_id, LO.quality_state, LO.wip_state, LO.qty_in, LO.qty_pass, LO.qty_fail, PK.id as package_id, PK.name, PK.is_Enabled, DN.is_automotive, case when PMM.permitted_machine_id is null then JMC.id else PMC.id end as machine_id, case when PMM.permitted_machine_id is null then JMC.name else PMC.name end as machine_name, MS.run_state as run_state, MS.online_state as online_state, MS.qc_state as qc_state, case when PMM.permitted_machine_id is null then JMC.is_automotive else PMC.is_automotive end as machine_is_automotive, LO.first_ins_state, LO.final_ins_state from[trans].lots as LO inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join [method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id left outer join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join method.jobs as JB with(NOLOCK) on JB.id = DF.job_id inner join mc.group_models as GM with(NOLOCK) on GM.machine_group_id = JB.machine_group_id inner join mc.machines as JMC with(NOLOCK) on JMC.machine_model_id = GM.machine_model_id left outer join[trans].machine_states as MS with(NOLOCK) on MS.machine_id = JMC.id left outer join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id left outer join[mc].machines as PMC with(NOLOCK) on PMC.id = PMM.machine_id and PMC.id = JMC.id where LO.id = @lot_id and JMC.id = @machine_id) as T1 where T1.machine_id is not null", Log.SqlLogger, sqlparameters );

            try
            {
                if (LotDataTable == null)
                {
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject, sqlparameters);
                    return null;
                }
                else if (LotDataTable.Rows.Count == 0)
                {
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject, sqlparameters);
                    return null;
                }
                else if (LotDataTable.Rows.Count > 1)
                {
                    iLibraryErrorAction(Errors_CheckProcessFlow.ToMutchLot, FunctionName, functionTimer, Log, dbObject, sqlparameters);

                    return null;
                }
                else
                {
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    return LotDataTable.Rows[0];
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }
        public DataTable Get_CheckFlowData(int[] lot_id, int machine_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_CheckFlowData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select * from(select LO.id, LO.lot_no, LO.is_special_flow, LO.location_id, DF.job_id, LO.step_no, LO.act_package_id, LO.product_family_id, LO.process_state, DF.device_slip_id, LO.process_job_id, LO.quality_state, LO.qty_in, LO.qty_pass, LO.qty_fail, PK.id as package_id, PK.name, PK.is_Enabled, DN.is_automotive, case when PMM.permitted_machine_id is null then JMC.id else PMC.id end as machine_id, case when PMM.permitted_machine_id is null then JMC.name else PMC.name end as machine_name, MS.run_state as run_state, MS.online_state as online_state, MS.qc_state as qc_state, case when PMM.permitted_machine_id is null then JMC.is_automotive else PMC.is_automotive end as machine_is_automotive, LO.first_ins_state, LO.final_ins_state from[trans].lots as LO inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join [method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id left outer join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join method.jobs as JB with(NOLOCK) on JB.id = DF.job_id inner join mc.group_models as GM with(NOLOCK) on GM.machine_group_id = JB.machine_group_id inner join mc.machines as JMC with(NOLOCK) on JMC.machine_model_id = GM.machine_model_id left outer join[trans].machine_states as MS with(NOLOCK) on MS.machine_id = JMC.id left outer join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id left outer join[mc].machines as PMC with(NOLOCK) on PMC.id = PMM.machine_id and PMC.id = JMC.id where LO.id in ( @lot_id ) and JMC.id = @machine_id) as T1 where T1.machine_id is not null", Log.SqlLogger, sqlparameters );

            try
            {
                if (LotDataTable == null)
                {
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject, sqlparameters);

                    return null;
                }
                else if (LotDataTable.Rows.Count == 0)
                {
                    iLibraryErrorAction(Errors_CheckProcessFlow.LotDoNotWip, FunctionName, functionTimer, Log, dbObject, sqlparameters);

                    return null;
                }
                else
                {
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    return LotDataTable;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }
        public DataRow Get_CheckSpFlowData(int lot_id, int machine_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_CheckFlowData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, LO.is_special_flow, LO.location_id, LOS.job_id, SF.step_no, LO.act_package_id, LO.product_family_id, SF.process_state, LOS.id as device_slip_id, LO.process_job_id, SF.quality_state, SF.qty_in, SF.qty_pass, SF.qty_fail, PK.id as package_id, PK.name, PK.is_Enabled, DN.is_automotive, MC.id as machine_id, MC.name as machine_name, MS.run_state as run_state, MS.online_state as online_state, MS.qc_state as qc_state, MC.is_automotive as machine_is_automotive, SF.first_ins_state, SF.final_ins_state from[trans].lots as LO inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[trans].special_flows as SF on SF.lot_id = LO.id and SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LOS with(NOLOCK) on LOS.special_flow_id = SF.id and LOS.step_no = SF.step_no inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id left outer join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = LOS.permitted_machine_id left outer join[mc].machines as MC with(NOLOCK) on MC.id = PMM.machine_id   left outer join[trans].machine_states as MS with(NOLOCK) on MS.machine_id = MC.id where LO.id = @lot_id and MC.id = @machine_id", Log.SqlLogger, sqlparameters );

            if (LotDataTable == null)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return null;
            }
            else if (LotDataTable.Rows.Count == 0)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return null;
            }
            else if (LotDataTable.Rows.Count > 1)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                throw new Exception("Resualt is more than 1 Record");
            }
            else
            {
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return LotDataTable.Rows[0];
            }

        }
        public DataRow Get_SpLotDBData(int id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_LotDBData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = id };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, SF.id as special_flow_id, SF.process_state, SF.step_no, SF.quality_state, SF.qty_in, SF.qty_pass, SF.qty_fail from[trans].lots as LO with(nolock) inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id where LO.id IN(@lots_id)", Log.SqlLogger, sqlparameters );

            db = null;
            if (LotDataTable == null)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return null;
            }
            else if (LotDataTable.Rows.Count == 0)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return null;
            }
            else if (LotDataTable.Rows.Count > 1)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                throw new Exception("Resualt is more than 1 Record");
            }
            else
            {
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return LotDataTable.Rows[0];
            }

        }
        public DataTable Get_LotDBTable(int[] ids, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_LotDBData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String id_string = null;

            for (int i = 0; i < ids.Length; i++)
            {
                id_string = id_string + ids[i].ToString();
                if (i < ids.Length - 1) { id_string = id_string + ','; }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_ids", SqlDbType.VarChar) { Value = id_string };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, DN.is_automotive, DF.job_id, LO.process_state, LO.device_slip_id, LO.step_no, LO.act_package_id, LO.product_family_id, LO.quality_state, LO.is_special_flow, LO.location_id, LO.qty_in, LO.qty_pass, LO.qty_fail, LO.first_ins_state, LO.final_ins_state from[trans].lots as LO with(nolock) inner join[method].packages as PK with(nolock) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(nolock) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(nolock) on DV.device_id = DS.device_id inner join[method].device_names as DN with(nolock) on  DN.id = DV.device_name_id left outer join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id in ( @lots_ids )", Log.SqlLogger, sqlparameters );

            if (LotDataTable == null)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return null;
            }
            else if (LotDataTable.Rows.Count == 0)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return null;
            }
            else if (LotDataTable.Rows.Count > 1)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                throw new Exception("Resualt is more than 1 Record");
            }
            else
            {
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                return LotDataTable;
            }

        }
        public DataTable Get_SpLotDBTable(int[] ids, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable LotDataTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            SaveFunctionLog("Get_LotDBData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String id_string = null;

            for (int i = 0; i < ids.Length; i++)
            {
                id_string = id_string + ids[i].ToString();
                if (i < ids.Length - 1) { id_string = id_string + ','; }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_ids", SqlDbType.VarChar) { Value = id_string };
            LotDataTable = db.SelectData(FunctionName, dbObject, "select LO.id, LO.lot_no, SF.id as special_flow_id, SF.process_state, SF.step_no, SF.quality_state, SF.qty_in, SF.qty_pass, SF.qty_fail from[trans].lots as LO with(nolock) inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id where LO.id IN(@lots_id)", Log.SqlLogger, sqlparameters );

            if (LotDataTable == null)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return null;
            }
            else if (LotDataTable.Rows.Count == 0)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return null;
            }
            else if (LotDataTable.Rows.Count > 1)
            {
                int ErrorNo = 00;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                throw new Exception("Resualt is more than 1 Record");
            }
            else
            {
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                dbObject.ConnectionClose();
                functionTimer = null;
                dbObject = null;
                sqlparameters = null;
                db = null;
                return LotDataTable;
            }
        }

        public int LotSetup_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            GetRecipeResult temp = Get_Recipe(LotDBInfos.Id, machine_id, Log);
            lotinfo.Recipe1 = temp.Recipe1;
            lotinfo.Recipe2 = temp.Recipe2;

            //if ( temp.IsPass != true)
            //{
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, temp.ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", temp.ErrorMessage, "");
            //    return -1;
            //}

            int okng = -1;
            try
            {
                if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    functionTimer.Start();

                    okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 101, LO.qty_last_pass = 0, LO.qty_last_fail = 0, LO.updated_by = @user_id, LO.updated_at = @date, LO.machine_id = @machine_id, LO.process_job_id = @pjid, LO.first_ins_state = 0, LO.final_ins_state = 0 from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                    if (okng < 1)
                    {
                        //Can't update LotState
                        int ErrorNo = 505;
                        lotinfo.ErrorNo = ErrorNo;
                        lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return -1;
                    }
                }
                else
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    functionTimer.Start();

                    okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 1, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.qty_last_pass = 0, LO.qty_fail_step_sum = 0, LO.qty_pass_step_sum = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid, LO.first_ins_state = 0, LO.final_ins_state = 0, LO.std_time_sum = DF.sum_process_minutes from[trans].lots as LO inner join [method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                    if (okng < 1)
                    {
                        //Can't update LotState
                        int ErrorNo = 505;
                        lotinfo.ErrorNo = ErrorNo;
                        lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                        Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                        return -1;
                    }
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[12];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 5 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@Recipe", SqlDbType.VarChar) { Value = temp.Recipe1 };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, @Recipe, DV.version_num ,LO.machine_id, @pjid, @online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up, LO.created_at, LO.created_by, LO.updated_at, LO.updated_by, LO.updated_by, LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with(nolock) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(nolock) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 506;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    return -1;
                }

                //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
                //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
                //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

                //okng = -1;

                //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 4 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

                //if (okng < 1)
                //{
                //    //Can't Insert MachineRecords
                //    int ErrorNo = 504;
                //    lotinfo.ErrorNo = ErrorNo;
                //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                //    return -1;
                //}
            }
            catch (Exception e)
            {
                throw new Exception(e.Message,e);
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }
        public int LotSetup_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            System.Data.SqlClient.SqlParameter[] sqlparameters;

            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            functionTimer.Start();

            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 500;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 0)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 501;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            int okng = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 11, SF.qty_last_pass = 0, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id, SF.process_job_id = @pjid, SF.first_ins_state = 0, SF.first_ins_state = 0 from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id ", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 502;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 1, SF.updated_by = @user_id, SF.updated_at = @date, SF.qty_last_pass = 0, SF.qty_pass_step_sum = 0, SF.qty_last_fail = 0, SF.qty_fail_step_sum = 0, SF.machine_id = @machine_id, SF.process_job_id = @pjid, SF.first_ins_state = 0, SF.first_ins_state = 0 from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id ", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 502;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 5 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class, lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, @pjid, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with(nolock) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(nolock) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 503;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 4 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 504;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }
        public int LotStart_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, String Recipe, Logger Log, int act_pass_qty, int Location_Num = 1)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;

            int next_step = Get_NextStepNo(LotDBInfos.Id, db, dbObject, datetime, Log);
            DateTime[] timeups = TimeUp_Calculation_BeforeStart(db, dbObject, LotDBInfos.Id, next_step, Log);
            Update_Lot_ProcessJobs(db, dbObject, pj_id, user_id, datetime.Datetime, true, Log);
            int okng = -1;

            if (act_pass_qty != -1)
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@pass_qty", SqlDbType.Int) { Value = act_pass_qty };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.qty_pass = @pass_qty from[trans].lots as LO where LO.id IN(@lot_id) ", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 205;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[16];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 1 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@position_id", SqlDbType.Int) { Value = Location_Num };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[14] = new System.Data.SqlClient.SqlParameter("@recipe", SqlDbType.VarChar) { Value = Recipe };
                sqlparameters[15] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, position_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, LO.qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, @recipe, DV.version_num ,LO.machine_id ,@position_id, @pjid ,@online_state, @DBx, LO.wip_state, 102, LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, @day_at, @user_id , @user_id ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 206;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[7];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 102, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_pass = 0, LO.qty_last_fail = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 205;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[16];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 1 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@position_id", SqlDbType.Int) { Value = Location_Num };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[14] = new System.Data.SqlClient.SqlParameter("@recipe", SqlDbType.VarChar) { Value = Recipe };
                sqlparameters[15] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, position_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id, wait_time) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, LO.qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, @recipe, DV.version_num ,LO.machine_id ,@position_id, @pjid ,@online_state, @DBx, LO.wip_state, 2, LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, @user_id ,@user_id ,LO.first_ins_state, LO.final_ins_state, DF.job_id, case when LO.finished_at IS NULL then DATEDIFF ( MINUTE , LO.created_at , @day_at ) else DATEDIFF(MINUTE, LO.finished_at, @day_at) end from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 206;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[7];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 2, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.qty_last_pass = 0, LO.qty_pass_step_sum = 0, LO.qty_fail_step_sum = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 205;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = LO.act_package_id, MS.last_device_id = LO.act_device_name_id from [trans].machine_states as MS cross join[trans].lots as LO where MS.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int LotStart_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log, int act_pass_qty, int Location_Num = 1)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            System.Data.SqlClient.SqlParameter[] sqlparameters;

            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 200;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 201;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            int okng = -1;

            if (act_pass_qty != -1)
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@pass_qty", SqlDbType.Int) { Value = act_pass_qty };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.qty_pass = @pass_qty from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id ", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 202;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 102, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 202;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {         
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 2, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 202;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 1 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@position_id", SqlDbType.Int) { Value = Location_Num };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, position_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by,LO.updated_by , first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id ,@position_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id,aa LO.is_temp_devided ,LO.temp_devided_count	,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by,SF.first_ins_state, SF.final_ins_state from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 203;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 204;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int OnlineStart_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            int okng = -1;

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 11 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 206;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int OnlineStart_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 200;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 201;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 11 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in,@qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 203;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int LotCancel_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            int okng = -1;

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 100, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 405;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 0, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 405;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 6 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 406;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 0 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 404;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int LotCancel_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 400;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 401;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 100, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 402;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 0, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 402;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 6 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id , SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 403;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 0 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters );

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 404;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int Reinput_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pass_qty, int fail_qty, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            int okng = -1;

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 100, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 405;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[5];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = 0, LO.finish_date_id = null, LO.finished_at = null, LO.updated_by = @user_id, LO.updated_at = @date, LO.qty_last_fail = 0, LO.machine_id = @machine_id, LO.process_job_id = @pjid from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 405;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 8 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = pass_qty };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = fail_qty };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 406;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int Reinput_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pass_qty, int fail_qty, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 400;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 401;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 100, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 402;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = 0, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 402;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 8 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = pass_qty };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = fail_qty };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id , SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 403;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int AbnormalLotHold_NormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[4];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update LO set LO.quality_state = 1, LO.updated_by = @user_id, LO.updated_at = @date, LO.machine_id = @machine_id from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 405;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 22 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 406;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 0 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 404;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }
        public int AbnormalLotHold_SpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 400;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 401;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[5];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update SF set SF.quality_state = 1, SF.finish_date_id = null, SF.finished_at = null, SF.updated_by = @user_id, SF.updated_at = @date, SF.machine_id = @machine_id from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 402;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 22 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 403;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@package_id", SqlDbType.Int) { Value = LotDBInfos.Package_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@device_id", SqlDbType.Int) { Value = LotDBInfos.Product_family_id };
            //sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = @package_id, MS.last_device_id = @device_id, MS.run_state = 0 from[trans].machine_states as MS where MS.machine_id = @machine_id", Log.SqlLogger, sqlparameters);

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 404;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int AbnormalLotEnd_NormalFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, bool is_hold, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            //TimeUp cluclate
            int next_step = Get_NextStepNo(LotDBInfos.Id, db, dbObject, datetime, Log);
            DateTime[] timeups = TimeUp_Calculation(db, dbObject, LotDBInfos.Id, next_step, Log);
            Update_Lot_ProcessJobs(db, dbObject, pj_id, user_id, datetime.Datetime, false, Log);

            int okng = -1;

            if (LotDBInfos.Quality_state != 0) { okng = Update_LotQCState(LotDBInfos.Id, db, dbObject, datetime, Log); }
            if (Is_Abnormal != 0) { okng = Update_LotProcessState(LotDBInfos.Id, Is_Abnormal, db, dbObject, datetime, Log); }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[14];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 23 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
            if (is_hold) { sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 1 }; }
            else { sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 0 }; }
            sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
            sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, quality_state, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, @hold, LO.qty_in, LO.qty_pass, @qty_pass, LO.qty_pass_step_sum + @qty_pass, LO.qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state, LO.process_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 306;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = DN.package_id, MS.last_device_id = DN.id from [trans].machine_states as MS inner join[trans].lots as LO on LO.machine_id = MS.machine_id inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_names as DN on DN.id = DS.device_id where MS.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 304;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            okng = -1;

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
            if (is_hold) { sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 1 }; }
            else { sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 0 }; }

            okng = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = @end_mode, LO.act_process_id = DF.act_process_id, LO.act_job_id = DF.job_id, LO.qty_last_pass = @qty_pass, LO.qty_pass_step_sum = LO.qty_pass_step_sum + @qty_pass, LO.qty_fail_step_sum = LO.qty_fail_step_sum + @qty_fail, LO.qty_last_fail = @qty_fail, LO.std_time_sum = DF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1, LO.first_ins_state = 0, LO.final_ins_state = 0, LO.quality_state = @hold from[trans].lots as LO with(nolock) inner join[method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 305;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int AbnormalLotEnd_SpecialFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, bool is_hold, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 300;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 2)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 301;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
            if (is_hold) { sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 1 }; }
            else { sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 0 }; }

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, quality_state, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, @hold, SF.qty_in, SF.qty_pass, @qty_pass, SF.qty_pass_step_sum + @qty_pass, SF.qty_fail , @qty_fail, SF.qty_fail_step_sum + @qty_fail, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state, SF.process_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 303;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            //sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = DN.package_id, MS.last_device_id = DN.id, MS.run_state = 0 from [trans].machine_states as MS inner join[trans].special_flows as SLO on SLO.machine_id = MS.machine_id inner join[trans].lots as LO on LO.special_flow_id = SLO.id inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_names as DN on DN.id = DS.device_id where MS.machine_id = @machine_id and SLO.id = @lot_id", Log.SqlLogger, sqlparameters);

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 304;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}

            sqlparameters = new System.Data.SqlClient.SqlParameter[9];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
            if (is_hold) { sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 1 }; }
            else { sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@hold", SqlDbType.Int) { Value = 0 }; }

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update SF set SF.process_state = @end_mode, SF.qty_last_pass = @qty_pass, SF.qty_pass_step_sum = SF.qty_pass_step_sum + @qty_pass, SF.qty_fail_step_sum = SF.qty_fail_step_sum + @qty_fail, SF.qty_last_fail = @qty_fail, SF.finish_date_id = @day_id, SF.finished_at = @date1, SF.updated_by = @user_id, SF.updated_at = @date2, SF.machine_id = -1, SF.first_ins_state = 0, SF.final_ins_state = 0, SF.quality_state = @ hold from[trans].special_flows as SF inner join[trans].lot_special_flows as LSF on LSF.id = SF.id inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id  where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 302;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int LotEnd_NormalFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            //TimeUp cluclate
            int next_step = Get_NextStepNo(LotDBInfos.Id, db, dbObject, datetime, Log);
            DateTime[] timeups = TimeUp_Calculation(db, dbObject, LotDBInfos.Id, next_step, Log);
            Update_Lot_ProcessJobs(db, dbObject, pj_id, user_id, datetime.Datetime, false, Log);

            int okng = -1;

            if (LotDBInfos.Quality_state != 0) { okng = Update_LotQCState(LotDBInfos.Id, db, dbObject, datetime, Log); }
            if (Is_Abnormal != 0) { okng = Update_LotProcessState(LotDBInfos.Id, Is_Abnormal, db, dbObject, datetime, Log); }

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 23 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, quality_state, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.quality_state, LO.qty_in, LO.qty_pass, @qty_pass, LO.qty_pass_step_sum + @qty_pass, LO.qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state, LO.process_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, DF.sum_process_minutes, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by, LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }

                if (next_step != 0)
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id + 1 };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                    sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                    sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                    sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                    sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                    sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                    sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                    sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                    okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass + LO.qty_pass_step_sum, @qty_pass, @qty_pass + LO.qty_pass_step_sum, LO.qty_fail + LO.qty_fail_step_sum + @qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, DF.sum_process_minutes, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by , LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);
                }

                else
                {

                    sqlparameters = new System.Data.SqlClient.SqlParameter[14];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id + 1 };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                    sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                    sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                    sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                    sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                    sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                    sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                    sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                    sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@wip_state", SqlDbType.Int) { Value = 100 };

                    okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass + LO.qty_pass_step_sum, @qty_pass, @qty_pass + LO.qty_pass_step_sum, LO.qty_fail + LO.qty_fail_step_sum + @qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, @wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, DF.sum_process_minutes, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by , LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);
                }
                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                if (next_step != 0)
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                    sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                    sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                    sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                    sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                    sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                    sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                    sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                    string tmp = "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in,";

                    if (LotDBInfos.Step_no == 100) { tmp = tmp + " @qty_pass, @qty_pass, @qty_pass, "; }
                    else { tmp = tmp + " LO.qty_pass - @qty_fail, LO.qty_pass - @qty_fail, LO.qty_pass - @qty_fail, "; }

                    okng = db.OperateData(FunctionName, dbObject, tmp + "LO.qty_fail + @qty_fail, @qty_fail, @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, DF.sum_process_minutes, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);
                }
                else
                {
                    sqlparameters = new System.Data.SqlClient.SqlParameter[14];
                    sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                    sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                    sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                    sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                    sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                    sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                    sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                    sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                    sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                    sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                    sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                    sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                    sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                    sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@wip_state", SqlDbType.Int) { Value = 100 };

                    string tmp = "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in,";

                    if (LotDBInfos.Step_no == 100) { tmp = tmp + " @qty_pass, @qty_pass, @qty_pass, "; }
                    else { tmp = tmp + " LO.qty_pass - @qty_fail, LO.qty_pass - @qty_fail, LO.qty_pass - @qty_fail, "; }

                    okng = db.OperateData(FunctionName, dbObject, tmp + "LO.qty_fail + @qty_fail, @qty_fail, @qty_fail, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, @wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, DF.sum_process_minutes, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);
                }

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = LO.act_package_id, MS.last_device_id = LO.act_device_name_id from [trans].machine_states as MS cross join[trans].lots as LO where MS.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 304;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            okng = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[12];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };

                string end_txt = "update LO set LO.process_state = @end_mode, LO.act_process_id = NDF.act_process_id, LO.act_job_id = NDF.job_id, LO.qty_pass = LO.qty_pass_step_sum + @qty_pass, LO.qty_last_pass = @qty_pass, LO.qty_pass_step_sum = LO.qty_pass_step_sum + @qty_pass, LO.qty_fail = LO.qty_fail + LO.qty_fail_step_sum + @qty_fail, LO.qty_last_fail = @qty_last_fail, LO.qty_fail_step_sum = LO.qty_fail_step_sum + @qty_fail, LO.std_time_sum = NDF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1";

                if (next_step == 0) { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.wip_state = 100"; }
                else if (next_step != 0 && LotDBInfos.Quality_state != 0) { ; }
                else { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.step_no = @next_step_no"; }

                okng = db.OperateData(FunctionName, dbObject, end_txt + " from[trans].lots as LO with(nolock) inner join[method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[method].device_flows as NDF on NDF.device_slip_id = LO.device_slip_id and NDF.step_no = DF.next_step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 305;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[12];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                //sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@time_up", SqlDbType.DateTime) { Value = timeup };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };
                string end_txt = null;

                if (LotDBInfos.Step_no != 100)
                {
                    end_txt = "update LO set LO.process_state = @end_mode, LO.act_process_id = NDF.act_process_id, LO.act_job_id = NDF.job_id, LO.qty_pass = LO.qty_pass - @qty_fail, LO.qty_last_pass = LO.qty_pass - @qty_fail, LO.qty_pass_step_sum = LO.qty_pass - @qty_fail, LO.qty_fail = LO.qty_fail + @qty_fail, LO.qty_last_fail = @qty_fail, LO.qty_fail_step_sum = @qty_fail, LO.std_time_sum = DF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1";
                }
                else
                {
                    end_txt = "update LO set LO.process_state = @end_mode, LO.act_process_id = NDF.act_process_id, LO.act_job_id = NDF.job_id, LO.qty_pass = @qty_pass, LO.qty_last_pass = @qty_pass, LO.qty_pass_step_sum = @qty_pass, LO.qty_fail = LO.qty_fail + @qty_fail, LO.qty_last_fail = @qty_fail, LO.qty_fail_step_sum = @qty_fail, LO.std_time_sum = DF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1";
                }
                if (next_step == 0) { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.wip_state = 100"; }
                else if (next_step != 0 && LotDBInfos.Quality_state != 0) {; }
                else { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.step_no = @next_step_no"; }

                okng = db.OperateData(FunctionName, dbObject, end_txt + " from[trans].lots as LO with(nolock) inner join[method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[method].device_flows as NDF on NDF.device_slip_id = LO.device_slip_id and NDF.step_no = DF.next_step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 305;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int LotEnd_SpecialFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 300;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 2)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 301;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            int okng = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[10];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class, lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, SF.qty_pass_step_sum + @qty_pass, @qty_pass, SF.qty_pass_step_sum + @qty_pass, SF.qty_fail_step_sum + @qty_fail, @qty_fail, SF.qty_fail_step_sum + @qty_fail, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, LO.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 303;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            { 
                sqlparameters = new System.Data.SqlClient.SqlParameter[10];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, LO.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 303;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            //sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = DN.package_id, MS.last_device_id = DN.id, MS.run_state = 0 from [trans].machine_states as MS inner join[trans].special_flows as SLO on SLO.machine_id = MS.machine_id inner join[trans].lots as LO on LO.special_flow_id = SLO.id inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_names as DN on DN.id = DS.device_id where MS.machine_id = @machine_id and SLO.id = @lot_id", Log.SqlLogger, sqlparameters );

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 304;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[9];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.step_no = LSF.next_step_no, SF.process_state = @end_mode, SF.qty_pass = SF.qty_pass_step_sum + @qty_pass, SF.qty_last_pass = @qty_pass, SF.qty_pass_step_sum = SF.qty_pass_step_sum + @qty_pass, SF.qty_fail = SF.qty_fail_step_sum + @qty_fail, SF.qty_last_fail = @qty_last_fail, SF.qty_fail_step_sum = SF.qty_fail_step_sum + @qty_fail, SF.finish_date_id = @day_id, SF.finished_at = @date1, SF.updated_by = @user_id, SF.updated_at = @date2, SF.machine_id = -1, SF.first_ins_state = 0, SF.final_ins_state = 0 from[trans].special_flows as SF inner join[trans].lot_special_flows as LSF on LSF.id = SF.id inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id  where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 302;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[9];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.step_no = LSF.next_step_no, SF.process_state = @end_mode, SF.qty_pass = SF.qty_pass - @qty_fail, SF.qty_last_pass = @qty_pass, SF.qty_pass_step_sum = @qty_pass, SF.qty_fail = SF.qty_fail + @qty_fail, SF.qty_last_fail = @qty_last_fail, SF.qty_fail_step_sum = @qty_fail, SF.finish_date_id = @day_id, SF.finished_at = @date1, SF.updated_by = @user_id, SF.updated_at = @date2, SF.machine_id = -1, SF.first_ins_state = 0, SF.final_ins_state = 0 from[trans].special_flows as SF inner join[trans].lot_special_flows as LSF on LSF.id = SF.id inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id  where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 302;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int LotEnd_QtyAdding_NormalFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, int hasuu, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            //TimeUp cluclate
            int next_step = Get_NextStepNo(LotDBInfos.Id, db, dbObject, datetime, Log);
            DateTime[] timeups = TimeUp_Calculation(db, dbObject, LotDBInfos.Id, next_step, Log);
            Update_Lot_ProcessJobs(db, dbObject, pj_id, user_id, datetime.Datetime, false, Log);

            int okng = -1;

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {

                sqlparameters = new System.Data.SqlClient.SqlParameter[14];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 23 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@hasuu", SqlDbType.Int) { Value = hasuu };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[13] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, quality_state, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.quality_state, LO.qty_in, LO.qty_pass, @qty_pass, LO.qty_pass_step_sum + @qty_pass, LO.qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu + @hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state, LO.process_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by , LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }

                sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id + 1 };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@hasuu", SqlDbType.Int) { Value = hasuu};
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass + LO.qty_pass_step_sum, @qty_pass, @qty_pass + LO.qty_pass_step_sum, LO.qty_fail + LO.qty_fail_step_sum + @qty_fail , @qty_fail, LO.qty_fail_step_sum + @qty_fail, LO.qty_divided, LO.qty_hasuu + @hasuu, LO.qty_out, DV.version_num ,LO.machine_id, LO.process_job_id ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@hasuu", SqlDbType.Int) { Value = hasuu };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, @qty_pass, @qty_pass, LO.qty_fail + @qty_fail, @qty_fail, @qty_fail, LO.qty_divided, LO.qty_hasuu + @hasuu , LO.qty_out, DV.version_num ,LO.machine_id, LO.process_job_id ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, @pass_plan_time, @pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 306;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = DN.package_id, MS.last_device_id = DN.id from [trans].machine_states as MS inner join[trans].lots as LO on LO.machine_id = MS.machine_id inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_names as DN on DN.id = DS.device_id where MS.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters);

            if (okng < 1)
            {
                //Can't Insert MachineRecords
                int ErrorNo = 304;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            okng = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@hasuu", SqlDbType.Int) { Value = hasuu };

                string end_txt = "update LO set LO.process_state = @end_mode, LO.act_process_id = NDF.act_process_id, LO.act_job_id = NDF.job_id, LO.qty_pass = LO.qty_pass_step_sum + @qty_pass, LO.qty_last_pass = @qty_pass, LO.qty_pass_step_sum = LO.qty_pass_step_sum + @qty_pass, LO.qty_fail = LO.qty_fail + LO.qty_fail_step_sum + @qty_fail, LO.qty_last_fail = @qty_last_fail, LO.qty_fail_step_sum = LO.qty_fail_step_sum + @qty_fail, LO.qty_hasuu = LO.qty_hasuu + @hasuu, LO.std_time_sum = NDF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1";

                if (next_step == 0) { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.wip_state = 100"; }
                else if (next_step != 0 && LotDBInfos.Quality_state != 0) {; }
                else { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.step_no = @next_step_no"; }

                okng = db.OperateData(FunctionName, dbObject, end_txt + " from[trans].lots as LO with(nolock) inner join[method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[method].device_flows as NDF on NDF.device_slip_id = LO.device_slip_id and NDF.step_no = DF.next_step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 305;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[13];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                //sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@time_up", SqlDbType.DateTime) { Value = timeup };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@pass_plan_time", SqlDbType.DateTime) { Value = timeups[0] };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@pass_plan_time_up", SqlDbType.DateTime) { Value = timeups[1] };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };
                sqlparameters[11] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };
                sqlparameters[12] = new System.Data.SqlClient.SqlParameter("@hasuu", SqlDbType.Int) { Value = hasuu };

                string end_txt = "update LO set LO.process_state = @end_mode, LO.act_process_id = DF.act_process_id, LO.act_job_id = DF.job_id, LO.qty_pass = @qty_pass, LO.qty_last_pass = @qty_pass, LO.qty_pass_step_sum = @qty_pass, LO.qty_fail = LO.qty_fail + @qty_fail, LO.qty_last_fail = @qty_fail, LO.qty_fail_step_sum = @qty_fail, LO.qty_hasuu = LO.qty_hasuu + @hasuu, LO.std_time_sum = DF.sum_process_minutes, LO.pass_plan_time = @pass_plan_time, LO.pass_plan_time_up = @pass_plan_time_up, LO.finish_date_id = @day_id, LO.finished_at = @date1, LO.updated_by = @user_id, LO.updated_at = @date2, LO.machine_id = -1";

                if (next_step == 0) { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.wip_state = 100"; }
                else if (next_step != 0 && LotDBInfos.Quality_state != 0) {; }
                else { end_txt = end_txt + ", LO.first_ins_state = 0, LO.final_ins_state = 0, LO.step_no = @next_step_no"; }

                okng = db.OperateData(FunctionName, dbObject, end_txt + " from[trans].lots as LO with(nolock) inner join[method].device_flows as DF on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 305;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int LotEnd_QtyAdding_SpecialFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, int hasuu, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id, Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 300;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 2)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 301;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            int okng = -1;
            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[11];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class, lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, SF.qty_pass_step_sum + @qty_pass, @qty_pass, SF.qty_pass_step_sum + @qty_pass, SF.qty_fail_step_sum + @qty_fail, @qty_fail, SF.qty_fail_step_sum + @qty_fail, SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 303;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[10];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 2 };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
                sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass + SF.qty_pass_step_sum, @qty_pass, @qty_pass + SF.qty_pass_step_sum, @qty_fail + SF.qty_fail, @qty_fail, @qty_fail + SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't Insert LotRecords
                    int ErrorNo = 303;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            //sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            //sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            //sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            //okng = -1;

            //okng = db.OperateData(FunctionName, dbObject, "update MS set MS.last_package_id = DN.package_id, MS.last_device_id = DN.id, MS.run_state = 0 from [trans].machine_states as MS inner join[trans].special_flows as SLO on SLO.machine_id = MS.machine_id inner join[trans].lots as LO on LO.special_flow_id = SLO.id inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_names as DN on DN.id = DS.device_id where MS.machine_id = @machine_id and SLO.id = @lot_id", Log.SqlLogger, sqlparameters);

            //if (okng < 1)
            //{
            //    //Can't Insert MachineRecords
            //    int ErrorNo = 304;
            //    lotinfo.ErrorNo = ErrorNo;
            //    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            //    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
            //    functionTimer = null;
            //    return -1;
            //}

            if (Get_LotAbnormalOrNot(LotDBInfos.Id, Log))
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[9];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.step_no = LSF.next_step_no, SF.process_state = @end_mode, SF.qty_pass = SF.qty_pass_step_sum + @qty_pass, SF.qty_last_pass = @qty_pass, SF.qty_pass_step_sum = SF.qty_pass_step_sum + @qty_pass, SF.qty_fail = SF.qty_fail_step_sum + @qty_fail, SF.qty_last_fail = @qty_last_fail, SF.qty_fail_step_sum = SF.qty_fail_step_sum + @qty_fail, SF.finish_date_id = @day_id, SF.finished_at = @date1, SF.updated_by = @user_id, SF.updated_at = @date2, SF.machine_id = -1, SF.first_ins_state = 0, SF.final_ins_state = 0 from[trans].special_flows as SF inner join[trans].lot_special_flows as LSF on LSF.id = SF.id inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id  where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 302;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[9];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@end_mode", SqlDbType.Int) { Value = Is_Abnormal };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = qty_pass };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@qty_last_fail", SqlDbType.Int) { Value = qty_fail };
                sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
                sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@date1", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
                sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@date2", SqlDbType.DateTime) { Value = datetime.Datetime };
                sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

                okng = -1;

                okng = db.OperateData(FunctionName, dbObject, "update SF set SF.step_no = LSF.next_step_no, SF.process_state = @end_mode, SF.qty_pass = @qty_pass, SF.qty_last_pass = @qty_pass, SF.qty_pass_step_sum = @qty_pass, SF.qty_fail = SF.qty_fail + @qty_fail, SF.qty_last_fail = @qty_last_fail, SF.qty_fail_step_sum = @qty_fail, SF.finish_date_id = @day_id, SF.finished_at = @date1, SF.updated_by = @user_id, SF.updated_at = @date2, SF.machine_id = -1, SF.first_ins_state = 0, SF.final_ins_state = 0 from[trans].special_flows as SF inner join[trans].lot_special_flows as LSF on LSF.id = SF.id inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id  where LO.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (okng < 1)
                {
                    //Can't update LotState
                    int ErrorNo = 302;
                    lotinfo.ErrorNo = ErrorNo;
                    lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    functionTimer = null;
                    return -1;
                }
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int OnlineEnd_NormalFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 12 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 306;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int OnlineEnd_SpecialFlows(ref LotUpdateInfo lotinfo, int Is_Abnormal, int qty_pass, int qty_fail, int qty_last_fail, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int machine_id, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 300;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 2)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 301;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int okng = -1;

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 12 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 303;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int Update_FirstInspectionNormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int ins_result, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[4];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@ins_result", SqlDbType.Int) { Value = ins_result };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update LO set LO.updated_by = @user_id, LO.updated_at = @date, LO.first_ins_state = @ins_result from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 205;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 13 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count	,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 206;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int Update_FirstInspectionSpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int ins_result, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 200;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 201;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[4];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@ins_result", SqlDbType.Int) { Value = ins_result };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update SF set SF.updated_by = @user_id, SF.updated_at = @date, SF.first_ins_state = @ins_result from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 202;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 13 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count ,SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 203;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int Update_FinalInspectionNormalFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int ins_result, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[4];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@ins_result", SqlDbType.Int) { Value = ins_result };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update LO set LO.updated_by = @user_id, LO.updated_at = @date, LO.final_ins_state = @ins_result from[trans].lots as LO where LO.id IN(@lot_id)", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 205;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[11];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 14 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = LotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = LotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@pjid", SqlDbType.Int) { Value = LotDBInfos.Pj_id };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[10] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, LO.step_no, LO.qty_in, @qty_pass, LO.qty_last_pass, LO.qty_pass_step_sum, @qty_fail, LO.qty_last_fail, LO.qty_fail_step_sum, LO.qty_divided, LO.qty_hasuu, LO.qty_out, DV.version_num ,LO.machine_id, @pjid ,@online_state, @DBx, LO.wip_state,LO.process_state,LO.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count	,LO.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,LO.created_at, LO.created_by, LO.updated_at, LO.updated_by ,LO.updated_by ,LO.first_ins_state, LO.final_ins_state, DF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 206;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public int Update_FinalInspectionSpecialFlows(ref LotUpdateInfo lotinfo, DatabaseAccess db, DatabaseAccessObject dbObject, int pj_id, int record_id, int Online_state, int ins_result, LotDBInfo LotDBInfos, DateTimeInfo datetime, int user_id, int DBx, String XmlData, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            SpLotDBInfo SpLotDBInfos = new SpLotDBInfo(Get_SpLotDBData(LotDBInfos.Id,Log));
            if (SpLotDBInfos == null)
            {
                //Datas Nothing
                int ErrorNo = 200;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            if (SpLotDBInfos.Pocess_state != 1)
            {
                //Lot_State is not 0 (Can't Setup)
                int ErrorNo = 201;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            System.Data.SqlClient.SqlParameter[] sqlparameters;
            sqlparameters = new System.Data.SqlClient.SqlParameter[4];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@date", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@ins_result", SqlDbType.Int) { Value = ins_result };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            int okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "update SF set SF.updated_by = @user_id, SF.updated_at = @date, SF.final_ins_state = @ins_result from[trans].special_flows as SF inner join[trans].lots as LO on LO.special_flow_id = SF.id and LO.id = SF.lot_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't update LotState
                int ErrorNo = 202;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[10];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@record_id", SqlDbType.Int) { Value = record_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@day_id", SqlDbType.Int) { Value = datetime.DayId };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@day_at", SqlDbType.DateTime) { Value = datetime.Datetime };
            sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@record_class", SqlDbType.Int) { Value = 14 };
            sqlparameters[4] = new System.Data.SqlClient.SqlParameter("@qty_pass", SqlDbType.Int) { Value = SpLotDBInfos.Qty_pass };
            sqlparameters[5] = new System.Data.SqlClient.SqlParameter("@qty_fail", SqlDbType.Int) { Value = SpLotDBInfos.Qty_fail };
            sqlparameters[6] = new System.Data.SqlClient.SqlParameter("@DBx", SqlDbType.Int) { Value = DBx };
            sqlparameters[7] = new System.Data.SqlClient.SqlParameter("@online_state", SqlDbType.Int) { Value = Online_state };
            sqlparameters[8] = new System.Data.SqlClient.SqlParameter("@XmlData", SqlDbType.Xml) { Value = XmlData };
            sqlparameters[9] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = LotDBInfos.Id };

            okng = -1;

            okng = db.OperateData(FunctionName, dbObject, "insert [trans].lot_process_records(id, day_id, recorded_at, record_class,  lot_id, process_id, step_no, qty_in, qty_pass, qty_last_pass, qty_pass_step_sum, qty_fail, qty_last_fail, qty_fail_step_sum, qty_divided, qty_hasuu, qty_out, recipe, recipe_version, machine_id, process_job_id, is_onlined, dbx_id, wip_state, process_state, quality_state, is_special_flow, special_flow_id, is_temp_devided, temp_devided_count, container_no, extend_data, std_time_sum, pass_plan_time, pass_plan_time_up, created_at, created_by, updated_at, updated_by, operated_by, first_ins_state, final_ins_state, job_id) select @record_id, @day_id, @day_at, @record_class,LO.id, DF.act_process_id, SF.step_no, SF.qty_in, @qty_pass, SF.qty_last_pass, SF.qty_pass_step_sum, @qty_fail, SF.qty_last_fail, SF.qty_fail_step_sum,  SF.qty_divided, SF.qty_hasuu, SF.qty_out, LSF.recipe, DV.version_num ,SF.machine_id, SF.process_job_id, @online_state, @DBx, SF.wip_state,SF.process_state, SF.quality_state, LO.is_special_flow ,LO.special_flow_id, LO.is_temp_devided ,LO.temp_devided_count, SF.container_no ,@XmlData, LO.std_time_sum, LO.pass_plan_time, LO.pass_plan_time_up ,SF.created_at, SF.created_by, SF.updated_at, SF.updated_by, SF.first_ins_state, SF.final_ins_state, LSF.job_id from[trans].lots as LO with(nolock) inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [method].device_slips as DS with ( NOLOCK ) on DS.device_slip_id = DF.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[trans].special_flows as SF with(nolock) on SF.id = LO.special_flow_id inner join[trans].lot_special_flows as LSF with(nolock) on LSF.special_flow_id = SF.id and LSF.step_no = SF.step_no where LO.id IN(@lots_id )", Log.SqlLogger, sqlparameters );

            if (okng < 1)
            {
                //Can't Insert LotRecords
                int ErrorNo = 203;
                lotinfo.ErrorNo = ErrorNo;
                lotinfo.ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                functionTimer = null;
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            return okng;
        }

        public JigInfo[] Get_JigsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            JigInfo[] jigs;
            SaveFunctionLog("Get_JigData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            JigTable = db.SelectData(FunctionName, dbObject, "select MCJ.jig_id from[trans].lots as LO inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [mc].permitted_machine_machines as PMM with(nolock) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_jigs as MCJ with(nolock) on PMM.permitted_machine_id = MCJ.machine_id inner join [method].jig_set_list as JSL with(nolock) on JSL.id = DF.jig_set_id and JSL.jig_group_id = MCJ.jig_group_id where MCJ.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    jigs = new JigInfo[JigTable.Rows.Count];

                    for (int i = 0; i < JigTable.Rows.Count; i++)
                    {
                        JigInfo jig = new JigInfo(JigTable.Rows[i]);
                        jigs[i] = jig;
                    }

                    return jigs;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int[] Get_JigIdsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int[] jigids;
            SaveFunctionLog("Get_JigIdsbyMachine", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            JigTable = db.SelectData(FunctionName, dbObject, "select MCJ.jig_id from[trans].lots as LO inner join[method].device_flows as DF with(nolock) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [mc].permitted_machine_machines as PMM with(nolock) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_jigs as MCJ with(nolock) on PMM.permitted_machine_id = MCJ.machine_id inner join [method].jig_set_list as JSL with(nolock) on JSL.id = DF.jig_set_id and JSL.jig_group_id = MCJ.jig_group_id where MCJ.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    jigids = new int[JigTable.Rows.Count];

                    for (int i = 0; i < JigTable.Rows.Count; i++)
                    {
                        JigInfo jig = new JigInfo(JigTable.Rows[i]);
                        jigids[i] = jig.Id;
                    }

                    return jigids;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public JigInfo[] Get_JigsbyMachine(int machine_id, int lot_id, string item_name, int product_family_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            JigInfo[] jigs;
            SaveFunctionLog("Get_JigData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            JigTable = db.SelectData(FunctionName, dbObject, "select JG.id, JG.barcode, CTG.id as categorie_id, CTG.name, JCT.control_no, JCT.control_name, case when JCT.integration_unit = null then 0 when JCT.integration_unit = 4 then case when JCD.value = null then convert(varchar(20), getdate()) else JCD.value  end else case when JCD.value = null then 0 else JCD.value end end as value, case when JCD.value = null then 1 else 0 end as is_value_null, JCT.integration_unit, JCT.warn_value, JCT.alarm_value, LC.id as location_id, LC.name as location_name, product_family_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join [mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_jigs as MCJ with(NOLOCK) on PMM.permitted_machine_id = MCJ.machine_id inner join[method].jig_set_list as JSL with(NOLOCK) on JSL.id = DF.jig_set_id and JSL.jig_group_id  MCJ.jig_group_id inner join[trans].jigs as JG with(NOLOCK) on JG.id = MCJ.jig_id inner join[material].jig_productions as PRD with(NOLOCK) on PRD.id = JG.jig_production_id left outer join[material].jig_categories as CTG with(NOLOCK) on CTG.id = PRD.category_id left outer join[material].jig_controls as JCT with(NOLOCK) on JCT.jig_production_id = JG.jig_production_idleft outer join[trans].jig_conditions as JCD with(NOLOCK) on JCD.id = JG.id and JCD.control_no = JCT.control_noleft outer join[material].locations as LC with(NOLOCK) on LC.id = JG.location_id left outer join[material].item_labels as IL with(NOLOCK) on IL.name = @labelname where MCJ.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    jigs = new JigInfo[JigTable.Rows.Count];
                    for (int i = 0; i < JigTable.Rows.Count; i++)
                    {
                        jigs[i] = new JigInfo(JigTable.Rows[i]);
                        if (jigs[i].Is_ValueExsist != 0)
                        {
                            dbObject.BeginTransaction();

                            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@jig_id", SqlDbType.VarChar) { Value = jigs[i].Id };
                            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@control_no", SqlDbType.VarChar) { Value = item_name };
                            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.Int) { Value = 0 };
                            JigTable = db.SelectData(FunctionName, dbObject, "insert [trans].jig_conditions(id, control_no, value, control_state, reseted_at, reseted_by) select @jig_id, @control_no, @value,0,null,null", Log.SqlLogger, sqlparameters );
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    return jigs;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public CheckJigResult[] Check_Jigs(int[] jig_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            JigInfo[] jigs = null;
            CheckJigResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Jigs", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String in_jigs = null;
            for (int i = 0; i < jig_id.Count(); i++)
            {
                in_jigs = in_jigs + jig_id[i].ToString();
                if (i != jig_id.Count() - 1) { in_jigs = in_jigs + ","; }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "jig_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = in_jigs };
            JigTable = db.SelectData(FunctionName, dbObject, "select JG.id , JG.barcode, CTG.id, CTG.name,case when JCT.integration_unit = null then 0 when JCT.integration_unit = 4 then	case when JCD.value = null then convert(varchar(20), getdate()) else JCD.value end else case when JCD.value = null then 0 else JCD.value end end as value,case when JCD.value = null then 1 else 0 end as is_value_null, JCT.integration_unit, IL.label_eng, JCT.warn_value, JCT.alarm_value, LC.id, LC.headquarter_id from[trans].jigs as JG with(nolock) inner join[material].jig_productions as PRD with(nolock) on PRD.id = JG.jig_production_id left outer join[material].jig_categories as CTG with(nolock) on CTG.id = PRD.category_id left outer join[material].jig_controls as JCT with(nolock) on JCT.jig_production_id = JG.jig_production_id left outer join[trans].jig_conditions as JCD with(nolock) on JCD.id = JG.id and JCD.control_no = JCT.control_no left outer join[material].locations as LC with(nolock) on LC.id = JG.location_id left outer join[material].item_labels as IL with(nolock) on IL.name = @label where JG.id IN( @id )", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    jigs = new JigInfo[JigTable.Rows.Count];
                    int Limit = 2;
                    int Warning = 1;

                    for (int i = 0; i < JigTable.Rows.Count; i++)
                    {
                        results = new CheckJigResult[JigTable.Rows.Count];
                        JigInfo jig = new JigInfo(JigTable.Rows[i]);
                        if (jig.CountUnit == 4)
                        {
                            if (DateTime.Parse(jig.CountLimit) < DateTime.Parse(jig.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                Update_JigState(db, dbObject, Limit, jig.Id, Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(jig.CountWarning) < DateTime.Parse(jig.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                        Update_JigState(db, dbObject, Warning, jig.Id, Log);
                                    }
                                    else
                                    {
                                        CheckJigResult result = new CheckJigResult();
                                        result.SetIsPass = true;
                                        result.Id = jig.Id;
                                        result.Barcode = jig.Name;
                                        result.Type_ID = jig.Type_ID;
                                        result.Type_Name = jig.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckJigResult result = new CheckJigResult();
                                    result.SetIsPass = true;
                                    result.Id = jig.Id;
                                    result.Barcode = jig.Name;
                                    result.Type_ID = jig.Type_ID;
                                    result.Type_Name = jig.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(jig.CountLimit) < int.Parse(jig.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                Update_JigState(db, dbObject, Limit, jig.Id, Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(jig.CountWarning) < int.Parse(jig.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                        Update_JigState(db, dbObject, Warning, jig.Id, Log);
                                    }
                                    else
                                    {
                                        CheckJigResult result = new CheckJigResult();
                                        result.SetIsPass = true;
                                        result.Id = jig.Id;
                                        result.Barcode = jig.Name;
                                        result.Type_ID = jig.Type_ID;
                                        result.Type_Name = jig.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckJigResult result = new CheckJigResult();
                                    result.SetIsPass = true;
                                    result.Id = jig.Id;
                                    result.Barcode = jig.Name;
                                    result.Type_ID = jig.Type_ID;
                                    result.Type_Name = jig.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int Update_JigValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int jig_id, int countunit, int control, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_JigValue", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.VarChar) { Value = value };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@jig_id", SqlDbType.Int) { Value = jig_id };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@countunit", SqlDbType.Int) { Value = countunit };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@control", SqlDbType.Int) { Value = control };
                okng = db.OperateData(FunctionName, dbObject, "update JCD SET JCD.value = @value from[trans].jig_conditions as JCD inner join[material].jig_controls as JCT on JCT.control_no = JCD.control_no where JCD.id = @jig_id and JCT.integration_unit = @countunit and JCD.control_no = @control", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public int Update_JigState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int jig_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_JigState", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = State };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@jig_id", SqlDbType.Int) { Value = jig_id };
                okng = db.OperateData(FunctionName, dbObject, "update JG SET JG.limit_state = @state from[trans].jigs as JG where JG.id = @jig_id", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public CheckJigResult[] Check_Jig(int jig_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            JigInfo[] jigs = null;
            CheckJigResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Jig", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "jig_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = jig_id };
            JigTable = db.SelectData(FunctionName, dbObject, "select JG.id , JG.barcode, CTG.id, CTG.name,case when JCT.integration_unit = null then 0 when JCT.integration_unit = 4 then	case when JCD.value = null then convert(varchar(20), getdate()) else JCD.value end else case when JCD.value = null then 0 else JCD.value end end as value,case when JCD.value = null then 1 else 0 end as is_value_null, JCT.integration_unit, IL.label_eng, JCT.warn_value, JCT.alarm_value, LC.id, LC.headquarter_id from[trans].jigs as JG with(nolock) inner join[material].jig_productions as PRD with(nolock) on PRD.id = JG.jig_production_id left outer join[material].jig_categories as CTG with(nolock) on CTG.id = PRD.category_id left outer join[material].jig_controls as JCT with(nolock) on JCT.jig_production_id = JG.jig_production_id left outer join[trans].jig_conditions as JCD with(nolock) on JCD.id = JG.id and JCD.control_no = JCT.control_no left outer join[material].locations as LC with(nolock) on LC.id = JG.location_id left outer join[material].item_labels as IL with(nolock) on IL.name = @label where JG.id = @id", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    jigs = new JigInfo[JigTable.Rows.Count];

                    for (int i = 0; i < JigTable.Rows.Count; i++)
                    {
                        results = new CheckJigResult[JigTable.Rows.Count];
                        JigInfo jig = new JigInfo(JigTable.Rows[i]);
                        if (jig.CountUnit == 4)
                        {
                            if (DateTime.Parse(jig.CountLimit) < DateTime.Parse(jig.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(jig.CountWarning) < DateTime.Parse(jig.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                    }
                                    else
                                    {
                                        CheckJigResult result = new CheckJigResult();
                                        result.SetIsPass = true;
                                        result.Id = jig.Id;
                                        result.Barcode = jig.Name;
                                        result.Type_ID = jig.Type_ID;
                                        result.Type_Name = jig.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckJigResult result = new CheckJigResult();
                                    result.SetIsPass = true;
                                    result.Id = jig.Id;
                                    result.Barcode = jig.Name;
                                    result.Type_ID = jig.Type_ID;
                                    result.Type_Name = jig.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(jig.CountLimit) < int.Parse(jig.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(jig.CountWarning) < int.Parse(jig.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckJigResult(ErrorNo, WarningMessage, "www.google.com", StepNum, jig.Id, jig.Name, jig.Type_ID, jig.Type_Name);
                                    }
                                    else
                                    {
                                        CheckJigResult result = new CheckJigResult();
                                        result.SetIsPass = true;
                                        result.Id = jig.Id;
                                        result.Barcode = jig.Name;
                                        result.Type_ID = jig.Type_ID;
                                        result.Type_Name = jig.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckJigResult result = new CheckJigResult();
                                    result.SetIsPass = true;
                                    result.Id = jig.Id;
                                    result.Barcode = jig.Name;
                                    result.Type_ID = jig.Type_ID;
                                    result.Type_Name = jig.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public MaterialInfo[] Get_MaterialsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable MaterialTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            MaterialInfo[] Materials;
            SaveFunctionLog("Get_MaterialData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            MaterialTable = db.SelectData(FunctionName, dbObject, "select MCM.material_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_materials as MCM with(NOLOCK) on PMM.permitted_machine_id = MCM.machine_id inner join[method].material_set_list as MSL with(NOLOCK) on MSL.id = DF.material_set_id and MSL.material_group_id = MCM.material_group_id where MCM.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (MaterialTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (MaterialTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    Materials = new MaterialInfo[MaterialTable.Rows.Count];

                    for (int i = 0; i < MaterialTable.Rows.Count; i++)
                    {
                        MaterialInfo Material = new MaterialInfo(MaterialTable.Rows[i]);
                        Materials[i] = Material;
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Materials;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int[] Get_MaterialIdsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable MaterialTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int[] Materialids;
            SaveFunctionLog("Get_MaterialIdsbyMachine", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            MaterialTable = db.SelectData(FunctionName, dbObject, "select MCM.material_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_materials as MCM with(NOLOCK) on PMM.permitted_machine_id = MCM.machine_id inner join[method].material_set_list as MSL with(NOLOCK) on MSL.id = DF.material_set_id and MSL.material_group_id = MCM.material_group_id where MCM.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (MaterialTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (MaterialTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    Materialids = new int[MaterialTable.Rows.Count];

                    for (int i = 0; i < MaterialTable.Rows.Count; i++)
                    {
                        MaterialInfo Material = new MaterialInfo(MaterialTable.Rows[i]);
                        Materialids[i] = Material.Id;
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Materialids;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public MaterialInfo[] Get_MaterialsbyMachine(int machine_id, int lot_id, string item_name, int product_family_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable MaterialTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            MaterialInfo[] materials;
            SaveFunctionLog("Get_MaterialData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            MaterialTable = db.SelectData(FunctionName, dbObject, "select MT.id, MT.barcode, CTG.id as categorie_id, CTG.name, MCT.control_no, MCT.control_name, case when MCT.integration_unit = null then 0 when MCT.integration_unit = 4 then case when MCD.value = null then convert(varchar(20), getdate()) else MCD.value end else case when MCD.value = null then 0 else MCD.value end end as value, case when MCD.value = null then 1 else 0 end as is_value_null, MCT.integration_unit, MCT.warn_value, MCT.alarm_value, LC.id as location_id, LC.name as location_name, vision_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_materials as MCM with(NOLOCK) on PMM.permitted_machine_id = MCM.machine_id inner join[method].material_set_list as MSL with(NOLOCK) on MSL.id = DF.material_set_id and MSL.material_group_id = MCM.material_group_id    inner join[trans].materials as MT with(NOLOCK) on MT.id = MCM.material_id inner join[material].material_productions as PRD with(NOLOCK) on PRD.id = MT.material_production_id left outer join[material].material_categories as CTG with(NOLOCK) on CTG.id = PRD.category_id left outer join[material].material_controls as MCT with(NOLOCK) on MCT.material_production_id = MT.material_production_id left outer join[trans].material_conditions as MCD with(NOLOCK) on MCD.id = MT.id and MCD.control_no = MCT.control_no left outer join[material].locations as LC with(NOLOCK) on LC.id = MT.location_id left outer join[material].item_labels as IL with(NOLOCK) on IL.name = @labelname where MCM.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (MaterialTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (MaterialTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    materials = new MaterialInfo[MaterialTable.Rows.Count];
                    for (int i = 0; i < MaterialTable.Rows.Count; i++)
                    {
                        materials[i] = new MaterialInfo(MaterialTable.Rows[i]);
                        if (materials[i].Is_ValueExsist != 0)
                        {
                            dbObject.BeginTransaction();

                            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@material_id", SqlDbType.VarChar) { Value = materials[i].Id };
                            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@control_no", SqlDbType.VarChar) { Value = item_name };
                            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.Int) { Value = 0 };
                            MaterialTable = db.SelectData(FunctionName, dbObject, "insert [trans].material_conditions(id, control_no, value, control_state, reseted_at, reseted_by) select @material_id, @control_no, @value,0,null,null", Log.SqlLogger, sqlparameters );
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    return materials;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public CheckMaterialResult[] Check_Materials(int[] material_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable MaterialTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            MaterialInfo[] materials = null;
            CheckMaterialResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Materials", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String in_materials = null;
            for (int i = 0; i < material_id.Count(); i++)
            {
                in_materials = in_materials + material_id[i].ToString();
                if (i != material_id.Count() - 1) { in_materials = in_materials + ","; }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "material_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = in_materials };
            MaterialTable = db.SelectData(FunctionName, dbObject, "select MT.id, MT.barcode, CTG.id, CTG.name, case when MCT.integration_unit = null then 0 when MCT.integration_unit = 4 then case when MCD.value = null then convert(varchar(20), getdate()) else MCD.value end else case when MCD.value = null then 0 else MCD.value end end as value, case when MCD.value = null then 1 else 0 end as is_value_null, MCT.integration_unit, IL.label_eng, MCT.warn_value, MCT.alarm_value, LC.id, LC.product_family_id from[trans].materials as MT with(NOLOCK) inner join[material].material_productions as PRD with(NOLOCK) on PRD.id = MT.material_production_id left outer join[material].material_categories as CTG with(NOLOCK) on CTG.id = RD.category_id left outer join[material].material_controls as MCT with(NOLOCK) on MCT.material_production_id = MT.material_production_id left outer join[trans].material_conditions as MCD with(NOLOCK) on MCD.id = MT.id and MCD.control_no = MCT.control_no left outer join[material].locations as LC with(NOLOCK) on LC.id = MT.location_id left outer join[material].item_labels as IL with(NOLOCK) on IL.name = @label where MT.id IN(@id) ", Log.SqlLogger, sqlparameters );

            try
            {
                if (MaterialTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (MaterialTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    materials = new MaterialInfo[MaterialTable.Rows.Count];
                    int Limit = 2;
                    int Warning = 1;

                    for (int i = 0; i < MaterialTable.Rows.Count; i++)
                    {
                        results = new CheckMaterialResult[MaterialTable.Rows.Count];
                        MaterialInfo material = new MaterialInfo(MaterialTable.Rows[i]);
                        if (material.CountUnit == 4)
                        {
                            if (DateTime.Parse(material.CountLimit) < DateTime.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                Update_MaterialState(db, dbObject, Limit, material.Id,Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(material.CountWarning) < DateTime.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                        Update_MaterialState(db, dbObject, Warning, material.Id,Log);
                                    }
                                    else
                                    {
                                        CheckMaterialResult result = new CheckMaterialResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckMaterialResult result = new CheckMaterialResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(material.CountLimit) < int.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                Update_MaterialState(db, dbObject, Limit, material.Id,Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(material.CountWarning) < int.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                        Update_MaterialState(db, dbObject, Warning, material.Id,Log);
                                    }
                                    else
                                    {
                                        CheckMaterialResult result = new CheckMaterialResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckMaterialResult result = new CheckMaterialResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int Update_MaterialValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int material_id, int countunit, int control, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_MaterialValue", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.VarChar) { Value = value };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@material_id", SqlDbType.Int) { Value = material_id };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@countunit", SqlDbType.Int) { Value = countunit };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@control", SqlDbType.Int) { Value = control };
                okng = db.OperateData(FunctionName, dbObject, "update MCD SET MCD.value = @value from[trans].material_conditions as MCD inner join[material].material_controls as MCT on MCT.control_no = MCD.control_no where MCD.id = @material_id and MCT.integration_unit = @countunit and MCD.control_no = @control", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public int Update_MaterialState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int material_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_MaterialState", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = State };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@material_id", SqlDbType.Int) { Value = material_id };
                okng = db.OperateData(FunctionName, dbObject, "update MT SET MT.limit_state = @state from[trans].materials as MT where MT.id = @material_id", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public CheckMaterialResult[] Check_Material(int material_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable MaterialTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            MaterialInfo[] materials = null;
            CheckMaterialResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Material", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "material_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = material_id };
            MaterialTable = db.SelectData(FunctionName, dbObject, "select MT.id, MT.barcode, CTG.id, CTG.name, case when MCT.integration_unit = null then 0 when MCT.integration_unit = 4 then case when MCD.value = null then convert(varchar(20), getdate()) else MCD.value end else case when MCD.value = null then 0 else MCD.value end end as value, case when MCD.value = null then 1 else 0 end as is_value_null, MCT.integration_unit, IL.label_eng, MCT.warn_value, MCT.alarm_value, LC.id, LC.product_family_id from[trans].materials as MT with(NOLOCK) inner join[material].material_productions as PRD with(NOLOCK) on PRD.id = MT.material_production_id left outer join[material].material_categories as CTG with(NOLOCK) on CTG.id = PRD.category_id left outer join[material].material_controls as MCT with(NOLOCK) on MCT.material_production_id = MT.material_production_id left outer join[trans].material_conditions as MCD with(NOLOCK) on MCD.id = MT.id and MCD.control_no = MCT.control_no left outer join[material].locations as LC with(NOLOCK) on LC.id = MT.location_id left outer join[material].item_labels as IL with(NOLOCK) on IL.name = @label where MT.id = @id", Log.SqlLogger, sqlparameters );

            try
            {
                if (MaterialTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (MaterialTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    materials = new MaterialInfo[MaterialTable.Rows.Count];

                    for (int i = 0; i < MaterialTable.Rows.Count; i++)
                    {
                        results = new CheckMaterialResult[MaterialTable.Rows.Count];
                        MaterialInfo material = new MaterialInfo(MaterialTable.Rows[i]);
                        if (material.CountUnit == 4)
                        {
                            if (DateTime.Parse(material.CountLimit) < DateTime.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(material.CountWarning) < DateTime.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                    }
                                    else
                                    {
                                        CheckMaterialResult result = new CheckMaterialResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckMaterialResult result = new CheckMaterialResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(material.CountLimit) < int.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(material.CountWarning) < int.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckMaterialResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                    }
                                    else
                                    {
                                        CheckMaterialResult result = new CheckMaterialResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckMaterialResult result = new CheckMaterialResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public EqpInfo[] Get_EqpsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable EqpTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            EqpInfo[] Eqps;
            SaveFunctionLog("Get_EqpData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            EqpTable = db.SelectData(FunctionName, dbObject, "select ECT.eqp_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_eqps as ECT with(NOLOCK) on PMM.permitted_machine_id = ECT.machine_id inner join[method].eqp_set_list as ESL with(NOLOCK) on ESL.id = DF.eqp_set_id and ESL.eqp_group_id = ECT.eqp_group_id where ECT.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (EqpTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (EqpTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    Eqps = new EqpInfo[EqpTable.Rows.Count];

                    for (int i = 0; i < EqpTable.Rows.Count; i++)
                    {
                        EqpInfo Eqp = new EqpInfo(EqpTable.Rows[i]);
                        Eqps[i] = Eqp;
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Eqps;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int[] Get_EqpIdsbyMachine(int machine_id, int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable EqpTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            int[] Eqpids;
            SaveFunctionLog("Get_EqpIdsbyMachine", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            EqpTable = db.SelectData(FunctionName, dbObject, "select MCE.eqp_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_eqps as MCE with(NOLOCK) on PMM.permitted_machine_id = MCE.machine_id inner join[method].eqp_set_list as ESL with(NOLOCK) on ESL.id = DF.eqp_set_id and ESL.eqp_group_id = MCE.eqp_group_id where MCE.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (EqpTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (EqpTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                     dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    Eqpids = new int[EqpTable.Rows.Count];

                    for (int i = 0; i < EqpTable.Rows.Count; i++)
                    {
                        EqpInfo Eqp = new EqpInfo(EqpTable.Rows[i]);
                        Eqpids[i] = Eqp.Id;
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return Eqpids;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public EqpInfo[] Get_EqpsbyMachine(int machine_id, int lot_id, string item_name, int product_family_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable EqpTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            EqpInfo[] eqps;
            SaveFunctionLog("Get_EqpData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = lot_id };
            EqpTable = db.SelectData(FunctionName, dbObject, "select ET.id, ET.barcode, CTG.id as categorie_id, CTG.name, ECT.control_no, ECT.control_name, case when ECT.integration_unit = null then 0 when ECT.integration_unit = 4 then case when ECD.value = null then convert(varchar(20), getdate()) else ECD.value end else case when ECD.value = null then 0 else ECD.value end end as value, case when ECD.value = null then 1 else 0 end as is_value_null, ECT.integration_unit, ECT.warn_value, ECT.alarm_value, LC.id as location_id, LC.name as location_name, vision_id from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[mc].permitted_machine_machines as PMM with(NOLOCK) on PMM.permitted_machine_id = DF.permitted_machine_id inner join[trans].machine_eqps as MCE with(NOLOCK) on PMM.permitted_machine_id = MCE.machine_id inner join[method].eqp_set_list as ESL with(NOLOCK) on ESL.id = DF.eqp_set_id and ESL.eqp_group_id = MCE.eqp_group_id    inner join[trans].eqps as ET with(NOLOCK) on ET.id = MCE.eqp_id inner join[eqp].eqp_productions as PRD with(NOLOCK) on PRD.id = ET.eqp_production_id left outer join[eqp].eqp_categories as CTG with(NOLOCK) on CTG.id = PRD.category_id left outer join[eqp].eqp_controls as ECT with(NOLOCK) on ECT.eqp_production_id = ET.eqp_production_id left outer join[trans].eqp_conditions as ECD with(NOLOCK) on ECD.id = ET.id and ECD.control_no = ECT.control_no left outer join[eqp].locations as LC with(NOLOCK) on LC.id = ET.location_id left outer join[eqp].item_labels as IL with(NOLOCK) on IL.name = @labelname where MCE.machine_id = @machine_id and LO.id = @lot_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (EqpTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (EqpTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    eqps = new EqpInfo[EqpTable.Rows.Count];
                    for (int i = 0; i < EqpTable.Rows.Count; i++)
                    {
                        eqps[i] = new EqpInfo(EqpTable.Rows[i]);
                        if (eqps[i].Is_ValueExsist != 0)
                        {
                            dbObject.BeginTransaction();

                            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@eqp_id", SqlDbType.VarChar) { Value = eqps[i].Id };
                            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@control_no", SqlDbType.VarChar) { Value = item_name };
                            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.Int) { Value = 0 };
                            EqpTable = db.SelectData(FunctionName, dbObject, "insert [trans].eqp_conditions(id, control_no, value, control_state, reseted_at, reseted_by) select @eqp_id, @control_no, @value,0,null,null", Log.SqlLogger, sqlparameters );
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    return eqps;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public CheckEQPResult[] Check_Eqps(int[] material_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable EqpTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            EqpInfo[] materials = null;
            CheckEQPResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Eqps", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();
            String in_materials = null;
            for (int i = 0; i < material_id.Count(); i++)
            {
                in_materials = in_materials + material_id[i].ToString();
                if (i != material_id.Count() - 1) { in_materials = in_materials + ","; }
            }

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "material_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = in_materials };
            EqpTable = db.SelectData(FunctionName, dbObject, "select ET.id, ET.barcode, CTG.id, CTG.name, case when ECT.integration_unit = null then 0 when ECT.integration_unit = 4 then case when ECD.value = null then convert(varchar(20), getdate()) else ECD.value end else case when ECD.value = null then 0 else ECD.value end end as value, case when ECD.value = null then 1 else 0 end as is_value_null, ECT.integration_unit, IL.label_eng, ECT.warn_value, ECT.alarm_value, LC.id, LC.product_family_id from[trans].materials as ET with(NOLOCK) inner join[material].material_productions as PRD with(NOLOCK) on PRD.id = ET.material_production_id left outer join[material].material_categories as CTG with(NOLOCK) on CTG.id = RD.category_id left outer join[material].material_controls as ECT with(NOLOCK) on ECT.material_production_id = ET.material_production_id left outer join[trans].material_conditions as ECD with(NOLOCK) on ECD.id = ET.id and ECD.control_no = ECT.control_no left outer join[material].locations as LC with(NOLOCK) on LC.id = ET.location_id left outer join[material].item_labels as IL with(NOLOCK) on IL.name = @label where ET.id IN(@id) ", Log.SqlLogger, sqlparameters );

            try
            {
                if (EqpTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (EqpTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    materials = new EqpInfo[EqpTable.Rows.Count];
                    int Limit = 2;
                    int Warning = 1;

                    for (int i = 0; i < EqpTable.Rows.Count; i++)
                    {
                        results = new CheckEQPResult[EqpTable.Rows.Count];
                        EqpInfo material = new EqpInfo(EqpTable.Rows[i]);
                        if (material.CountUnit == 4)
                        {
                            if (DateTime.Parse(material.CountLimit) < DateTime.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                Update_EqpState(db, dbObject, Limit, material.Id, Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(material.CountWarning) < DateTime.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                        Update_EqpState(db, dbObject, Warning, material.Id, Log);
                                    }
                                    else
                                    {
                                        CheckEQPResult result = new CheckEQPResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckEQPResult result = new CheckEQPResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(material.CountLimit) < int.Parse(material.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                Update_EqpState(db, dbObject, Limit, material.Id, Log);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(material.CountWarning) < int.Parse(material.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, material.Id, material.Name, material.Type_ID, material.Type_Name);
                                        Update_EqpState(db, dbObject, Warning, material.Id, Log);
                                    }
                                    else
                                    {
                                        CheckEQPResult result = new CheckEQPResult();
                                        result.SetIsPass = true;
                                        result.Id = material.Id;
                                        result.Barcode = material.Name;
                                        result.Type_ID = material.Type_ID;
                                        result.Type_Name = material.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckEQPResult result = new CheckEQPResult();
                                    result.SetIsPass = true;
                                    result.Id = material.Id;
                                    result.Barcode = material.Name;
                                    result.Type_ID = material.Type_ID;
                                    result.Type_Name = material.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public int Update_EqpValue(DatabaseAccess db, DatabaseAccessObject dbObject, string value, int eqp_id, int countunit, int control, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_EqpValue", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[4];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.VarChar) { Value = value };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@eqp_id", SqlDbType.Int) { Value = eqp_id };
                sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@countunit", SqlDbType.Int) { Value = countunit };
                sqlparameters[3] = new System.Data.SqlClient.SqlParameter("@control", SqlDbType.Int) { Value = control };
                okng = db.OperateData(FunctionName, dbObject, "update ECD SET ECD.value = @value from[trans].eqp_conditions as ECD inner join[eqp].eqp_controls as ECT on ECT.control_no = ECD.control_no where ECD.id = @eqp_id and ECT.integration_unit = @countunit and ECD.control_no = @control", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public int Update_EqpState(DatabaseAccess db, DatabaseAccessObject dbObject, int State, int eqp_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Update_EqpState", 0, DateTime.Now);
            int okng = -1;
            try
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@state", SqlDbType.Int) { Value = State };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@eqp_id", SqlDbType.Int) { Value = eqp_id };
                okng = db.OperateData(FunctionName, dbObject, "update ET SET ET.limit_state = @state from[trans].eqps as ET where ET.id = @eqp_id", Log.SqlLogger, sqlparameters );
            }
            catch (Exception e)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                throw new Exception(e.Message,e);
            }
            if (okng < 1)
            {
                int ErrorNo = 999;
                Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                return -1;
            }
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            return okng;
        }

        public CheckEQPResult[] Check_Eqp(int eqp_id, bool Is_WarnError, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable EqpTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            EqpInfo[] eqps = null;
            CheckEQPResult[] results = null;
            int StepNum = 0;
            SaveFunctionLog("Check_Eqp", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[2];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@label", SqlDbType.VarChar) { Value = "eqp_contors.integration_unit" };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@id", SqlDbType.Int) { Value = eqp_id };
            EqpTable = db.SelectData(FunctionName, dbObject, "select ET.id, ET.barcode, CTG.id, CTG.name, case when ECT.integration_unit = null then 0 when ECT.integration_unit = 4 then case when ECD.value = null then convert(varchar(20), getdate()) else ECD.value end else case when ECD.value = null then 0 else ECD.value end end as value, case when ECD.value = null then 1 else 0 end as is_value_null, ECT.integration_unit, IL.label_eng, ECT.warn_value, ECT.alarm_value, LC.id, LC.product_family_id from[trans].eqps as ET with(NOLOCK) inner join[eqp].eqp_productions as PRD with(NOLOCK) on PRD.id = ET.eqp_production_id left outer join[eqp].eqp_categories as CTG with(NOLOCK) on CTG.id = PRD.category_id left outer join[eqp].eqp_controls as ECT with(NOLOCK) on ECT.eqp_production_id = ET.eqp_production_id left outer join[trans].eqp_conditions as ECD with(NOLOCK) on ECD.id = ET.id and ECD.control_no = ECT.control_no left outer join[eqp].locations as LC with(NOLOCK) on LC.id = ET.location_id left outer join[eqp].item_labels as IL with(NOLOCK) on IL.name = @label where ET.id = @id", Log.SqlLogger, sqlparameters );

            try
            {
                if (EqpTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (EqpTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;
                    eqps = new EqpInfo[EqpTable.Rows.Count];

                    for (int i = 0; i < EqpTable.Rows.Count; i++)
                    {
                        results = new CheckEQPResult[EqpTable.Rows.Count];
                        EqpInfo eqp = new EqpInfo(EqpTable.Rows[i]);
                        if (eqp.CountUnit == 4)
                        {
                            if (DateTime.Parse(eqp.CountLimit) < DateTime.Parse(eqp.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, eqp.Id, eqp.Name, eqp.Type_ID, eqp.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (DateTime.Parse(eqp.CountWarning) < DateTime.Parse(eqp.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, eqp.Id, eqp.Name, eqp.Type_ID, eqp.Type_Name);
                                    }
                                    else
                                    {
                                        CheckEQPResult result = new CheckEQPResult();
                                        result.SetIsPass = true;
                                        result.Id = eqp.Id;
                                        result.Barcode = eqp.Name;
                                        result.Type_ID = eqp.Type_ID;
                                        result.Type_Name = eqp.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckEQPResult result = new CheckEQPResult();
                                    result.SetIsPass = true;
                                    result.Id = eqp.Id;
                                    result.Barcode = eqp.Name;
                                    result.Type_ID = eqp.Type_ID;
                                    result.Type_Name = eqp.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                        else
                        {
                            if (int.Parse(eqp.CountLimit) < int.Parse(eqp.Value))
                            {
                                Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                int ErrorNo = 1;
                                results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, eqp.Id, eqp.Name, eqp.Type_ID, eqp.Type_Name);
                            }
                            else
                            {
                                if (Is_WarnError == true)
                                {
                                    if (int.Parse(eqp.CountWarning) < int.Parse(eqp.Value))
                                    {
                                        Dictionary<int, string> WarningMessage = new Dictionary<int, string>();
                                        int ErrorNo = 1;
                                        results[i] = new CheckEQPResult(ErrorNo, WarningMessage, "www.google.com", StepNum, eqp.Id, eqp.Name, eqp.Type_ID, eqp.Type_Name);
                                    }
                                    else
                                    {
                                        CheckEQPResult result = new CheckEQPResult();
                                        result.SetIsPass = true;
                                        result.Id = eqp.Id;
                                        result.Barcode = eqp.Name;
                                        result.Type_ID = eqp.Type_ID;
                                        result.Type_Name = eqp.Type_Name;
                                        results[i] = result;
                                    }
                                }
                                else
                                {
                                    CheckEQPResult result = new CheckEQPResult();
                                    result.SetIsPass = true;
                                    result.Id = eqp.Id;
                                    result.Barcode = eqp.Name;
                                    result.Type_ID = eqp.Type_ID;
                                    result.Type_Name = eqp.Type_Name;
                                    results[i] = result;
                                }
                            }
                        }
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    return results;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public DateTime[] TimeUp_Calculation(DatabaseAccess db, DatabaseAccessObject dbObject, int lot_id, int next_step, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("TimeUp_Calculation", 0, DateTime.Now);
            DateTime[] Timeups = new DateTime[2];

            if (next_step != 0)
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };

                times = db.SelectData(FunctionName, dbObject, "select DS.normal_leadtime_minutes, NEXT_DF.process_minutes, NEXT_DF.sum_process_minutes, INPLAN.date_value as in_date_value, OUTPLAN.date_value as out_date_value, FIXED_OUTPLAN.date_value as fixed_out_date_value, ACTUALLY_IN.date_value as act_in_date_value from[trans].lots as LO with(NOLOCK) inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_flows as NEXT_DF on NEXT_DF.device_slip_id = DS.device_slip_id and NEXT_DF.step_no = @next_step_no inner join[trans].days as INPLAN on INPLAN.id = LO.in_plan_date_id inner join[trans].days as OUTPLAN on OUTPLAN.id = LO.out_plan_date_id inner join[trans].days as FIXED_OUTPLAN on FIXED_OUTPLAN.id = LO.modify_out_plan_date_id inner join[trans].days as ACTUALLY_IN on ACTUALLY_IN.id = LO.in_date_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

                times = db.SelectData(FunctionName, dbObject, "select DS.normal_leadtime_minutes, NEXT_DF.process_minutes, NEXT_DF.sum_process_minutes, INPLAN.date_value as in_date_value, OUTPLAN.date_value as out_date_value, FIXED_OUTPLAN.date_value as fixed_out_date_value, ACTUALLY_IN.date_value as act_in_date_value from[trans].lots as LO with(NOLOCK) inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_flows as NEXT_DF on NEXT_DF.device_slip_id = DS.device_slip_id and NEXT_DF.step_no = LO.step_no inner join[trans].days as INPLAN on INPLAN.id = LO.in_plan_date_id inner join[trans].days as OUTPLAN on OUTPLAN.id = LO.out_plan_date_id inner join[trans].days as FIXED_OUTPLAN on FIXED_OUTPLAN.id = LO.modify_out_plan_date_id inner join[trans].days as ACTUALLY_IN on ACTUALLY_IN.id = LO.in_date_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            }
            double rate = double.Parse(times.Rows[0].ItemArray[0].ToString());
            int process_minutes = int.Parse(times.Rows[0].ItemArray[1].ToString());
            int sum_process_minutes = int.Parse(times.Rows[0].ItemArray[2].ToString());
            DateTime In_date_value = DateTime.Parse(times.Rows[0].ItemArray[3].ToString());
            DateTime Out_date_value = DateTime.Parse(times.Rows[0].ItemArray[4].ToString());
            DateTime Fixed_Out_date_value = DateTime.Parse(times.Rows[0].ItemArray[5].ToString());
            DateTime Act_In_date_value = DateTime.Parse(times.Rows[0].ItemArray[6].ToString());

            if (next_step == 0) { Timeups[0] = Out_date_value; Timeups[1] = Fixed_Out_date_value; return Timeups; }

            DateTime Timeup1 = In_date_value + TimeSpan.FromMinutes(sum_process_minutes * rate);
            DateTime Timeup2 = Act_In_date_value + TimeSpan.FromMinutes(sum_process_minutes * rate);
            Timeups[0] = Timeup1;
            Timeups[1] = Timeup2;

            return Timeups;
        }

        public DateTime[] TimeUp_Calculation_BeforeStart(DatabaseAccess db, DatabaseAccessObject dbObject, int lot_id, int next_step, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DataTable times = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("TimeUp_Calculation", 0, DateTime.Now);
            DateTime[] Timeups = new DateTime[2];

            if (next_step != 0)
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@next_step_no", SqlDbType.Int) { Value = next_step };

                times = db.SelectData(FunctionName, dbObject, "select DS.normal_leadtime_minutes, NEXT_DF.process_minutes, NEXT_DF.sum_process_minutes, INPLAN.date_value as in_date_value, OUTPLAN.date_value as out_date_value, FIXED_OUTPLAN.date_value as fixed_out_date_value, ACTUALLY_IN.date_value as act_in_date_value from[trans].lots as LO with(NOLOCK) inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_flows as NEXT_DF on NEXT_DF.device_slip_id = DS.device_slip_id and NEXT_DF.step_no = @next_step_no inner join[trans].days as INPLAN on INPLAN.id = LO.in_plan_date_id inner join[trans].days as OUTPLAN on OUTPLAN.id = LO.out_plan_date_id inner join[trans].days as FIXED_OUTPLAN on FIXED_OUTPLAN.id = LO.modify_out_plan_date_id inner join[trans].days as ACTUALLY_IN on ACTUALLY_IN.id = LO.in_date_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            }
            else
            {
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

                times = db.SelectData(FunctionName, dbObject, "select DS.normal_leadtime_minutes, NEXT_DF.process_minutes, NEXT_DF.sum_process_minutes, INPLAN.date_value as in_date_value, OUTPLAN.date_value as out_date_value, FIXED_OUTPLAN.date_value as fixed_out_date_value, ACTUALLY_IN.date_value as act_in_date_value from[trans].lots as LO with(NOLOCK) inner join[method].device_slips as DS on DS.device_slip_id = LO.device_slip_id inner join[method].device_flows as NEXT_DF on NEXT_DF.device_slip_id = DS.device_slip_id and NEXT_DF.step_no = LO.step_no inner join[trans].days as INPLAN on INPLAN.id = LO.in_plan_date_id inner join[trans].days as OUTPLAN on OUTPLAN.id = LO.out_plan_date_id inner join[trans].days as FIXED_OUTPLAN on FIXED_OUTPLAN.id = LO.modify_out_plan_date_id inner join[trans].days as ACTUALLY_IN on ACTUALLY_IN.id = LO.in_date_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            }
            double rate = double.Parse(times.Rows[0].ItemArray[0].ToString());
            int process_minutes = int.Parse(times.Rows[0].ItemArray[1].ToString());
            int sum_process_minutes = int.Parse(times.Rows[0].ItemArray[2].ToString());
            DateTime In_date_value = DateTime.Parse(times.Rows[0].ItemArray[3].ToString());
            DateTime Out_date_value = DateTime.Parse(times.Rows[0].ItemArray[4].ToString());
            DateTime Fixed_Out_date_value = DateTime.Parse(times.Rows[0].ItemArray[5].ToString());
            DateTime Act_In_date_value = DateTime.Parse(times.Rows[0].ItemArray[6].ToString());

            if (next_step == 0) { Timeups[0] = Out_date_value; Timeups[1] = Fixed_Out_date_value; return Timeups; }

            DateTime Timeup1 = In_date_value + TimeSpan.FromMinutes(sum_process_minutes * rate - process_minutes);
            DateTime Timeup2 = Act_In_date_value + TimeSpan.FromMinutes(sum_process_minutes * rate - process_minutes);
            Timeups[0] = Timeup1;
            Timeups[1] = Timeup2;

            return Timeups;
        }

        public JigInfo Get_JigsbyID(string barcode, string item_name, int product_family_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            DataTable JigTable = new DataTable();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            SaveFunctionLog("Get_JigData", 0, DateTime.Now);
            functionTimer.Start();

            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[3];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@labelname", SqlDbType.VarChar) { Value = item_name };
            sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@barcode", SqlDbType.VarChar) { Value = barcode };
            sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@product_family_id", SqlDbType.Int) { Value = product_family_id };
            JigTable = db.SelectData(FunctionName, dbObject, "select JG.id, JG.barcode, CTG.id as categorie_id, CTG.name, JCT.control_no, JCT.control_name,case when JCT.integration_unit = null then 0 when JCT.integration_unit = 4 then case when JCD.value = null then convert(varchar(20), getdate()) else JCD.value end else case when JCD.value = null then 0 else JCD.value end end as value, case when JCD.value = null then 1 else 0 end as is_value_null, JCT.integration_unit, JCT.warn_value, JCT.alarm_value, LC.id as location_id, LC.name as location_name, LC.product_family_id from[trans].jigs as JG with(nolock) inner join[material].jig_productions as PRD with(nolock) on PRD.id = JG.jig_production_id left outer join[material].jig_categories as CTG with(nolock) on CTG.id = PRD.category_id left outer join[material].jig_controls as JCT with(nolock) on JCT.jig_production_id = JG.jig_production_id　left outer join[trans].jig_conditions as JCD with(nolock) on JCD.id = JG.id and JCD.control_no = JCT.control_no　left outer join[material].locations as LC with(nolock) on LC.id = JG.location_id left outer join[material].item_labels as IL with(nolock) on IL.name = @labelname where JG.barcode = @barcode and PRD.product_family_id = @product_family_id", Log.SqlLogger, sqlparameters );

            try
            {
                if (JigTable == null)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else if (JigTable.Rows.Count == 0)
                {
                    int ErrorNo = 999;
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Faile", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    sqlparameters = null;

                    return null;
                }
                else
                {
                    //for (int i = 0; i < JigTable.Rows.Count; i++)
                    //{
                    //    JigInfo jig = new JigInfo(JigTable.Rows[i]);
                    //    if (jig.Is_ValueExsist != 0)
                    //    {
                    //        dbObject.BeginTransaction();

                    //        sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                    //        sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@labelname", SqlDbType.Int) { Value = item_name };
                    //        sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@barcode", SqlDbType.Int) { Value = barcode };
                    //        sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@product_family_id", SqlDbType.Int) { Value = product_family_id };
                    //        JigTable = db.SelectData(FunctionName, dbObject, "insert [trans].jig_conditions(id, control_no, value, control_state, reseted_at, reseted_by) select @jig_id, @control_no, @value,0,null,null",Log , sqlparameters );
                    //    }
                    //}
                    JigInfo jig = new JigInfo(JigTable.Rows[0]);
                    if (jig.Is_ValueExsist != 0)
                    {
                        dbObject.BeginTransaction();

                        sqlparameters = new System.Data.SqlClient.SqlParameter[3];
                        sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@jig_id", SqlDbType.Int) { Value = jig.Id };
                        sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@control_no", SqlDbType.Int) { Value = item_name };
                        sqlparameters[2] = new System.Data.SqlClient.SqlParameter("@value", SqlDbType.Int) { Value = jig.Value };
                        JigTable = db.SelectData(FunctionName, dbObject, "insert [trans].jig_conditions(id, control_no, value, control_state, reseted_at, reseted_by) select @jig_id, @control_no, @value,0,null,null", Log.SqlLogger, sqlparameters );
                    }
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
                    dbObject.ConnectionClose();
                    functionTimer = null;
                    dbObject = null;
                    db = null;
                    sqlparameters = null;

                    return jig;
                }
            }
            catch (Exception e)
            {
                //dbObject.ConnectionClose();
                //functionTimer = null;
                //dbObject = null;
                //db = null;
                //sqlparameters = null;
                //return null;
                throw new Exception(e.Message,e);
            }
        }

        public CheckProcessFlowResult ResultMaker_CheckProcessFlowResult(string FunctionName, Stopwatch FunctionTimer, int ErrorNo)
        {
            CheckProcessFlowResult result = new CheckProcessFlowResult();
            result.SetIsPass = false;
            result.SetErrorNo = ErrorNo;
            result.SetErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            SaveFunctionLog(FunctionName, FunctionTimer.ElapsedMilliseconds, DateTime.Now);

            FunctionTimer = null;
            return result;
        }

        public GetRecipeResult ResultMaker_GetRecipeResult(string FunctionName, Stopwatch FunctionTimer, int ErrorNo)
        {
            GetRecipeResult result = new GetRecipeResult();
            result.SetIsPass = false;
            result.SetErrorNo = ErrorNo;
            result.SetErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
            SaveFunctionLog(FunctionName, FunctionTimer.ElapsedMilliseconds, DateTime.Now);

            FunctionTimer = null;
            return result;
        }

        public bool Check_InspectionState(DatabaseAccess db, DatabaseAccessObject dbObject, ref LotDBInfo info, DateTime time, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log.SqlLogger.SaveFunctionLog_Start(time, FunctionName, "IN", "", "iLibrary", "", "", "");

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lots_id", SqlDbType.Int) { Value = info.Id };
            DataTable tmp_table = db.SelectData(FunctionName, dbObject, "select JB.id, JB.name, JB.is_mount, JB.is_skipped, JB.is_first_ins, JB.is_final_ins from[trans].lots as LO inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no inner join[method].jobs as JB with(NOLOCK) on JB.id = DF.job_id where LO.id = @lots_id", Log.SqlLogger, sqlparameters);

            if(tmp_table == null)
            {
                return false;
            }
            if(tmp_table.Rows.Count < 1)
            {
                return false;
            }
            if(tmp_table.Rows.Count > 1)
            {
                return false;
            }
            ProcessInfo process = new ProcessInfo(tmp_table.Rows[0]);
            if (process.IsFirstInsChecking == 1 && info.First_inspection < 1)
            {
                return false;
            }
            if (process.IsFinalInsChecking == 1 && info.Final_inspection < 1)
            {
                return false;
            }

            if (info.First_inspection >= 2 || info.Final_inspection >= 2)
            {
                info.Quality_state = 1;
            }
            return true;
        }

        public GetRecipeResult Get_Recipe(int lot_id, int machine_id, Logger Log)
        {
            GetRecipeResult result = new GetRecipeResult();
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetRecipesList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select DS.tp_code, DS.os_program_name, DN.assy_name, DN.ft_name, DN.name as device_name, DC.chip_name, PK.name from[trans].lots as LO with(NOLOCK) inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no left outer join[method].device_chips as DC with(NOLOCK) on DC.device_slip_id = DF.device_slip_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            try
            {
                if (tmp1 == null)
                {
                    iLibraryErrorAction(Errors_GetRecipe.LotRecipeDataIsNothig, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_GetRecipeResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                }
                DB_GetConverter DBCONVERT = new DB_GetConverter();
                String Tp_Code = DBCONVERT.GetString(tmp1.Rows[0],"tp_code");
                if (!String.IsNullOrEmpty(Tp_Code)) { Tp_Code.Trim(); }
                String OS_ProgramName = DBCONVERT.GetString(tmp1.Rows[0],"os_program_name");
                if (!String.IsNullOrEmpty(OS_ProgramName)) { OS_ProgramName.Trim(); }
                String AssyName = DBCONVERT.GetString(tmp1.Rows[0],"assy_name");
                if (!String.IsNullOrEmpty(AssyName)) { AssyName.Trim(); }
                String FTName = DBCONVERT.GetString(tmp1.Rows[0],"ft_name");
                if (!String.IsNullOrEmpty(FTName)) { FTName.Trim(); }
                String DeviceName = DBCONVERT.GetString(tmp1.Rows[0],"device_name");
                if (!String.IsNullOrEmpty(DeviceName)) { DeviceName.Trim(); }
                String ChipName = DBCONVERT.GetString(tmp1.Rows[0],"chip_name");
                if (!String.IsNullOrEmpty(ChipName)) { ChipName.Trim(); }
                String PackageName = DBCONVERT.GetString(tmp1.Rows[0],"name");
                if (!String.IsNullOrEmpty(PackageName)) { PackageName.Trim(); }
                /*-------------------GetRecipesList----------------*/

                /*-------------------Get_MachineInfo about Recipe ----------------*/
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = machine_id };

                DataTable tmp2 = db.SelectData(FunctionName, dbObject, "Select MD.ppid_type1, MD.ppid_type2 from [mc].machines as MC with(NOLOCK) inner join[mc].models as MD with(NOLOCK) on MD.id = MC.machine_model_id where MC.id = @machine_id", Log.SqlLogger, sqlparameters);

                if (tmp2 == null)
                {
                    iLibraryErrorAction(Errors_GetRecipe.MachinePPIDTypeIsNothing, FunctionName, functionTimer, Log, dbObject);
                    result.Recipe1 = "";
                    result.Recipe2 = "";
                    result.SetIsPass = true;
                    result.SetErrorNo = 0;
                    result.SetErrorMessage = null;
                    return ResultMaker_GetRecipeResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                }
                int PpidType1 = -1;
                int PpidType2 = -1;
                try { PpidType1 = DBCONVERT.GetByte(tmp2.Rows[0],"ppid_type1"); }
                catch { PpidType1 = -1; }
                try { PpidType2 = DBCONVERT.GetByte(tmp2.Rows[0],"ppid_type2"); }
                catch { PpidType2 = -1; }
                if (PpidType1 == -1 && PpidType2 == -1) { return null; }
                /*-------------------Get_MachineInfo about Recipe ----------------*/

                if (PpidType1 <= 0)
                {
                    iLibraryErrorAction(Errors_GetRecipe.MachinePPIDTypeIsNothing, FunctionName, functionTimer, Log, dbObject);
                    return ResultMaker_GetRecipeResult(FunctionName, functionTimer, Errors_CheckProcessFlow.LotDoNotWip);
                }

                switch (PpidType1)
                {
                    case 3: result.Recipe1 = Tp_Code; break;
                    case 4: result.Recipe1 = OS_ProgramName; break;
                    case 5: result.Recipe1 = AssyName; break;
                    case 6: result.Recipe1 = FTName; break;
                    case 7: result.Recipe1 = DeviceName; break;
                    case 8: result.Recipe1 = ChipName; break;
                    case 9: result.Recipe1 = PackageName; break;
                    default: result.Recipe1 = null; break;
                }

                switch (PpidType2)
                {
                    case 3: result.Recipe2 = Tp_Code; break;
                    case 4: result.Recipe2 = OS_ProgramName; break;
                    case 5: result.Recipe2 = AssyName; break;
                    case 6: result.Recipe2 = FTName; break;
                    case 7: result.Recipe2 = DeviceName; break;
                    case 8: result.Recipe2 = ChipName; break;
                    case 9: result.Recipe2 = PackageName; break;
                    default: result.Recipe2 = null; break;
                }
                if (result.Recipe1 == null) { result.Recipe1 = ""; }
                if (result.Recipe2 == null) { result.Recipe2 = ""; }
            }

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            result.SetIsPass = true;
            result.SetErrorNo = 0;
            result.SetErrorMessage = null;
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return result;
        }

        public List<String> Get_PermissionMachinesByLMS(int user_id, Logger Log)
        {
            List<String> Machines = new List<string>();

            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();
            
            /*-------------------GetMachineNameList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select MC.name From[ctrlic].user_lic as UL with(nolock) inner join[ctrlic].model_lic as ML with(nolock) on ML.lic_id = UL.lic_id inner join[mc].machines as MC with(nolock) on MC.machine_model_id = ML.model_ref_id where UL.user_id = @user_id", Log.SqlLogger, sqlparameters);

            dbObject.ConnectionClose();
            try
            {
                if (tmp1 == null)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return null;
                }

                for (int i = 0; i < tmp1.Rows.Count; i++)
                {
                    Machines.Add(DBCONVERT.GetString(tmp1.Rows[i],"name"));
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return Machines;
        }

        public List<String> Get_PermissionMachinesByLMS(DatabaseAccess db, DatabaseAccessObject dbObject, int user_id, DateTimeInfo datetime, Logger Log)
        {
            List<String> Machines = new List<string>();

            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetMachineNameList----------------*/
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select MC.name From[ctrlic].user_lic as UL with(nolock) inner join[ctrlic].model_lic as ML with(nolock) on ML.lic_id = UL.lic_id inner join[mc].machines as MC with(nolock) on MC.machine_model_id = ML.model_ref_id where UL.user_id = @user_id", Log.SqlLogger, sqlparameters);

            try
            {
                if (tmp1 == null)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return null;
                }

                for (int i = 0; i < tmp1.Rows.Count; i++)
                {
                    Machines.Add(DBCONVERT.GetString(tmp1.Rows[i],"name"));
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return Machines;
        }

        public Boolean Check_PermissionMachinesByLMS(int user_id, string machine_name, Logger Log)
        {
            List<String> Machines = new List<string>();

            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetMachineNameList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);

            dbObject.ConnectionOpen();
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@user_id", SqlDbType.Int) { Value = user_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select MC.name From[ctrlic].user_lic as UL with(nolock) inner join[ctrlic].model_lic as ML with(nolock) on ML.lic_id = UL.lic_id inner join[mc].machines as MC with(nolock) on MC.machine_model_id = ML.model_ref_id where UL.user_id = @user_id", Log.SqlLogger, sqlparameters);

            dbObject.ConnectionClose();
            try
            {
                if (tmp1 == null)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return false;
                }

                for (int i = 0; i < tmp1.Rows.Count; i++)
                {
                    Machines.Add(DBCONVERT.GetString(tmp1.Rows[i],"name"));
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            return Machines.Exists(x => x==machine_name);
        }

        public Boolean Check_UserLotAutoMotive(UserInfo user, LotInfo lot, Logger Log)
        {
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            Stopwatch functionTimer = new Stopwatch();
            functionTimer.Start();

            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            try
            {
                if (lot.IsAutomotive == false){ return true; }
                else
                {
                    if(user.Is_PD_Automotive == true) { return true; }
                    else
                    {
                        int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                        Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessageProvider.GetErrorMessage(ErrorNo), "");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                //int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", e.Message, "");
                throw new Exception(e.Message,e);
            }
        }

        public Boolean Get_LotAbnormalOrNot(int lot_id, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetMachineNameList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            dbObject.ConnectionOpen();
            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select LO.process_state From[trans].lots as LO with(nolock) where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
            int state_num = -1;
            try
            {
                if (tmp1 == null)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return true;
                }

                state_num = DBCONVERT.GetByte(tmp1.Rows[0],"process_state");
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }

            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "OK", "");
            functionTimer = null;
            dbObject = null;
            db = null;

            if ((state_num / 100) != 1) { return false; }
            else { return true; }
        }
        public int Get_NextStepNo(int lot_id, DatabaseAccess db, DatabaseAccessObject dbObject, DateTimeInfo datetime, Logger Log )
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();
            try
            {
                /*-------------------GetNextStepInfo----------------*/
                //dbObject.ConnectionOpen();
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };

                DataTable tmp1 = db.SelectData(FunctionName, dbObject, "select LO.id, LO.device_slip_id, LO.step_no, DF.next_step_no ,NEXT.next_step_no as next_second_step_no from[trans].lots as LO inner join[method].packages as PK with(NOLOCK) on PK.id = LO.act_package_id inner join[method].device_slips as DS with(NOLOCK) on DS.device_slip_id = LO.device_slip_id inner join[method].device_versions as DV with(NOLOCK) on DV.device_id = DS.device_id inner join[method].device_names as DN with(NOLOCK) on DN.id = DV.device_name_id inner join[method].device_flows as DF with(NOLOCK) on DF.device_slip_id = LO.device_slip_id and DF.step_no = LO.step_no left outer join[method].device_flows as NEXT with ( NOLOCK ) on NEXT.device_slip_id = DF.device_slip_id and NEXT.step_no = DF.next_step_no and NEXT.is_skipped = 1 where LO.id = @lot_id", Log.SqlLogger, sqlparameters);


                if (tmp1 == null)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return -1;
                }
                int slip_id = -1;
                int step = -1;
                int next_step = -1;
                int next_second_step = -1;

                slip_id = DBCONVERT.GetInt32(tmp1.Rows[0], "device_slip_id");
                step = DBCONVERT.GetInt32(tmp1.Rows[0], "step_no");
                next_step = DBCONVERT.GetInt32(tmp1.Rows[0], "next_step_no");
                next_second_step = DBCONVERT.GetInt32(tmp1.Rows[0], "next_second_step_no");

                if(step == next_step) { return 0; }

                if (next_second_step == -1)
                {
                    Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next_step.ToString(), "");
                    functionTimer = null;
                    //dbObject.ConnectionClose();
                    return next_step;
                }
                else
                {
                    while (true)
                    {
                        sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                        sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@slip_id", SqlDbType.Int) { Value = slip_id };
                        sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@step_no", SqlDbType.Int) { Value = next_second_step };

                        DataTable tmp2 = db.SelectData(FunctionName, dbObject, "select NEXT0.next_step_no as NEXT0_StepNum, NEXT1.next_step_no as NEXT1_StepNum, NEXT2.next_step_no as NEXT2_StepNum, NEXT3.next_step_no as NEXT3_StepNum, NEXT4.next_step_no as NEXT4_StepNum, NEXT5.next_step_no as NEXT5_StepNum from[method].device_flows as NEXT0 with(NOLOCK) left outer join[method].device_flows as NEXT1 with(NOLOCK) on NEXT1.device_slip_id = NEXT0.device_slip_id and NEXT1.step_no = NEXT0.next_step_no and NEXT1.is_skipped = 1 left outer join[method].device_flows as NEXT2 with(NOLOCK) on NEXT2.device_slip_id = NEXT1.device_slip_id and NEXT2.step_no = NEXT1.next_step_no and NEXT2.is_skipped = 1 left outer join[method].device_flows as NEXT3 with(NOLOCK) on NEXT3.device_slip_id = NEXT2.device_slip_id and NEXT3.step_no = NEXT2.next_step_no and NEXT3.is_skipped = 1 left outer join[method].device_flows as NEXT4 with(NOLOCK) on NEXT4.device_slip_id = NEXT3.device_slip_id and NEXT4.step_no = NEXT3.next_step_no and NEXT4.is_skipped = 1 left outer join[method].device_flows as NEXT5 with(NOLOCK) on NEXT5.device_slip_id = NEXT4.device_slip_id and NEXT5.step_no = NEXT4.next_step_no and NEXT5.is_skipped = 1 where NEXT0.device_slip_id = @slip_id and NEXT0.step_no = @step_no and NEXT0.is_skipped = 1", Log.SqlLogger, sqlparameters);

                        if (tmp2 == null || tmp2.Rows.Count == 0)
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next_second_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next_second_step;
                        }

                        int next1_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT0_StepNum");
                        int next2_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT1_StepNum");
                        int next3_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT2_StepNum");
                        int next4_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT3_StepNum");
                        int next5_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT4_StepNum");
                        next_second_step = DBCONVERT.GetInt32(tmp2.Rows[0], "NEXT5_StepNum");
                        if (next_second_step != -1) { continue; }
                        else if(next5_step != -1)
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next5_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next5_step;
                        }
                        else if (next4_step != -1)
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next4_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next4_step;
                        }
                        else if (next3_step != -1)
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next3_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next3_step;
                        }
                        else if (next2_step != -1)
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next2_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next2_step;
                        }
                        else
                        {
                            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", "Next Step =" + next1_step.ToString(), "");
                            functionTimer = null;
                            //dbObject.ConnectionClose();
                            return next1_step;
                        }
                    }
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
        }

        public int Update_LotQCState(int lot_id, DatabaseAccess db, DatabaseAccessObject dbObject, DateTimeInfo datetime, Logger Log )
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            int tmp = -1;
            try
            {
                /*-------------------GetNextStepInfo----------------*/
                //dbObject.ConnectionOpen();
                sqlparameters = new System.Data.SqlClient.SqlParameter[1];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
                
                tmp = db.OperateData(FunctionName, dbObject, "update LO set LO.quality_state = 1 from [trans].lots as LO where Lo.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (tmp <= 0)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return -1;
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message,e);
            }
            return tmp;
        }

        public int Update_LotProcessState(int lot_id, int Process_state,DatabaseAccess db, DatabaseAccessObject dbObject, DateTimeInfo datetime, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }
            DB_GetConverter DBCONVERT = new DB_GetConverter();

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            int tmp = -1;
            try
            {
                /*-------------------GetNextStepInfo----------------*/
                //dbObject.ConnectionOpen();
                sqlparameters = new System.Data.SqlClient.SqlParameter[2];
                sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = lot_id };
                sqlparameters[1] = new System.Data.SqlClient.SqlParameter("@process_state", SqlDbType.Int) { Value = Process_state };

                tmp = db.OperateData(FunctionName, dbObject, "update LO set LO.process_state = @process_state from [trans].lots as LO where Lo.id = @lot_id", Log.SqlLogger, sqlparameters);

                if (tmp <= 0)
                {
                    int ErrorNo = Errors_CheckProcessFlow.LotDoNotWip;
                    String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);
                    functionTimer.Stop();
                    Log.SqlLogger.SaveFunctionLog_End(datetime.Datetime, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");
                    return -1;
                }
            }
            /*-------------------GetMachineNameList----------------*/

            catch (Exception e)
            {
                //Can't insert pj_lots
                dbObject.TransactionCancel();
                dbObject.ConnectionClose();
                throw new Exception(e.Message, e);
            }
            return tmp;
        }

        public int GetInt32(DataRow row, String Name)
        {
            if (row.IsNull(Name)) { return -1; }
            else { return row.Field<int>(Name); }
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

        public void iLibraryErrorAction( int ErrorNo, String FunctionName, Stopwatch functionTimer, Logger Log, DatabaseAccessObject dbObject, System.Data.SqlClient.SqlParameter[] sqlparameters)
        {
            String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");

            dbObject.ConnectionClose();
            functionTimer = null;
            dbObject = null;
            sqlparameters = null;
        }
        public void iLibraryErrorAction(int ErrorNo, String FunctionName, Stopwatch functionTimer, Logger Log, DatabaseAccessObject dbObject)
        {
            String ErrorMessage = ErrorMessageProvider.GetErrorMessage(ErrorNo);

            functionTimer.Stop();
            Log.SqlLogger.SaveFunctionLog_End(DateTime.Now, ErrorNo, FunctionName, "OUT", "", "iLibrary", functionTimer.ElapsedMilliseconds, "", ErrorMessage, "");

            dbObject.ConnectionClose();
            dbObject = null;
        }
        public List<APCSMaterialInfo> MaterialCheckByAPCS(String Process, List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log)
        {
            List<APCSMaterialInfo> ReturnList = new List<APCSMaterialInfo>();
            switch (Process)
            {
                case "DB":
                    ReturnList = MaterialCheckByAPCS_DB(MaterialInfos, datetime, Lots, Machines, Users, Log);
                    break;
                case "WB":
                    ReturnList = MaterialCheckByAPCS_WB(MaterialInfos, datetime, Lots, Machines, Users, Log);
                    break;
                case "MP":
                    ReturnList = MaterialCheckByAPCS_MP(MaterialInfos, datetime, Lots, Machines, Users, Log);
                    break;
                case "TC":
                    ReturnList = MaterialCheckByAPCS_TC(MaterialInfos, datetime, Lots, Machines, Users, Log);
                    break;
                default:
                    break;
            }
            return ReturnList;
        }

        private List<APCSMaterialInfo> MaterialCheckByAPCS_DB(List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log)
        {
            DB_InputMaterial Temp = new DB_InputMaterial();
            List<APCSMaterialInfo> ReturnList = new List<APCSMaterialInfo>();
            String QRLotNo = "";
            if (Lots != null) { QRLotNo = Lots.Name; }
                
            //---------SetPart-----------
            for (int i = 0; i < MaterialInfos.Count; i++)
            {
                switch (MaterialInfos[i].ProductionName)
                {
                    case "Preform":
                        MaterialInfos[i].Pass = Temp.PreformCheck(MaterialInfos[i].QRCode);
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.PreformAlarm; }
                        else
                        {
                            MaterialInfos[i].Pass = Temp.PackageDeviceComparePreform(Temp.m_PreformType, QRLotNo);
                            if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.m_QRReadAlarm; }
                        }
                        MaterialInfos[i].Set_Name = Temp.m_PreformLotNo;
                        MaterialInfos[i].ExpiredTime = Temp.m_PreformEXP;
                        MaterialInfos[i].InputTime = Temp.m_PreformInput;
                        MaterialInfos[i].Type = Temp.m_PreformType;
                        if (!string.IsNullOrEmpty(Temp.PreformQR))
                        {
                            MaterialInfos[i].Id = int.Parse(Temp.PreformQR);
                        }
                      ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "Frame":
                        DB_APCSMaterialControl.MaterialClass mat = Temp.FrameCheck(MaterialInfos[i].QRCode);
                        MaterialInfos[i].Pass = mat.Pass;
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = mat.ErrMessage; }
                        else
                        {
                            MaterialInfos[i].Pass = Temp.CheckFrameCondition(ref mat, QRLotNo);
                            if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = mat.ErrMessage; }
                        }
                        MaterialInfos[i].Set_Name = mat.FrameLotNo;
                        MaterialInfos[i].Type = mat.FrameType;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "Rubber Collet":
                        DB_APCSMaterialControl.ServiceReference2.CheckResult result = Temp.CheckCollet(Lots.Name, MaterialInfos[i].QRCode, Machines.Name, Machines.MachineType, Users.Code);
                        MaterialInfos[i].Pass = result.IsPass;
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = result.ErrorMessage; }
                        MaterialInfos[i].Set_Name = result.ItemCode;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                }
            }
            return ReturnList;
        }

        private List<APCSMaterialInfo> MaterialCheckByAPCS_WB(List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log)
        {
            WB_InputMaterial Temp = new WB_InputMaterial();
            List<APCSMaterialInfo> ReturnList = new List<APCSMaterialInfo>();
            String QRLotNo = "";
            if (Lots != null) { QRLotNo = Lots.Name; }

            ////---------SetPart-----------
            for (int i = 0; i < MaterialInfos.Count; i++)
            {
                switch (MaterialInfos[i].ProductionName)
                {
                    case "Cappillary":
                        MaterialInfos[i].Pass = Temp.CheckCapillary(MaterialInfos[i].IsChanged, QRLotNo, MaterialInfos[i].QRCode, Machines.Name);
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.ErrorMessage; }
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "Bolt":
                        MaterialInfos[i].Pass = Temp.BoltCheck(MaterialInfos[i].IsChanged, Machines.Name);
                        MaterialInfos[i].ErrorMessage = Temp.ErrorMessage;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "Wire":
                        MaterialInfos[i].Pass = Temp.WireCheck(MaterialInfos[i].IsChanged, QRLotNo, MaterialInfos[i].QRCode, Machines.Name);
                        MaterialInfos[i].ErrorMessage = Temp.ErrorMessage;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "HP":
                        MaterialInfos[i].Pass = Temp.JigCheck(MaterialInfos[i].IsChanged, QRLotNo, MaterialInfos[i].QRCode, Machines.Name, Users.Code);
                        MaterialInfos[i].ErrorMessage = Temp.ErrorMessage;
                        MaterialInfos[i].Message = Temp.Message;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "PP":
                        MaterialInfos[i].Pass = Temp.JigCheck(MaterialInfos[i].IsChanged, QRLotNo, MaterialInfos[i].QRCode, Machines.Name, Users.Code);
                        MaterialInfos[i].ErrorMessage = Temp.ErrorMessage;
                        MaterialInfos[i].Message = Temp.Message;
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                }
            }
            return ReturnList;
        }

        private List<APCSMaterialInfo> MaterialCheckByAPCS_MP(List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log)
        {
            MP_InputMaterial Temp = new MP_InputMaterial();
            List<APCSMaterialInfo> ReturnList = new List<APCSMaterialInfo>();
            String QRLotNo = "";
            if (Lots != null) { QRLotNo = Lots.Name; }

            ////---------SetPart-----------
            for (int i = 0; i < MaterialInfos.Count; i++)
            {
                switch (MaterialInfos[i].ProductionName)
                {
                    case "Resin":
                        int PcsInt1 = -1;
                        if(!int.TryParse(MaterialInfos[i].Pcs.ToString(),out PcsInt1))
                        {
                            MaterialInfos[i].ErrorMessage = "Pcs is Strange Parameter.";
                            MaterialInfos[i].Pass = false;
                            ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                            break;
                        }
                        MaterialInfos[i].Pass = Temp.ResinExpiredWarining(MaterialInfos[i].QRCode, PcsInt1, QRLotNo, MaterialInfos[i].IsDummy, MaterialInfos[i].IsChanged, Users.Code);
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.ErrorMessage; }
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                    case "CleanShot":
                        int PcsInt2 = -1;
                        if (!int.TryParse(MaterialInfos[i].Pcs.ToString(), out PcsInt2))
                        {
                            MaterialInfos[i].ErrorMessage = "Pcs is Strange Parameter.";
                            MaterialInfos[i].Pass = false;
                            ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                            break;
                        }
                        int Total = -1;
                        if (!int.TryParse(MaterialInfos[i].TotalCount.ToString(), out Total))
                        {
                            MaterialInfos[i].ErrorMessage = "TotalCount is Strange Parameter.";
                            MaterialInfos[i].Pass = false;
                            ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                            break;
                        }
                        MaterialInfos[i].Pass = Temp.CheckCleanShot(PcsInt2, MaterialInfos[i].UseCount, QRLotNo, Total);
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.ErrorMessage; }
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                }
            }
            return ReturnList;
        }

        private List<APCSMaterialInfo> MaterialCheckByAPCS_TC(List<APCSMaterialInfo> MaterialInfos, DateTime datetime, LotInfo Lots, MachineInfo Machines, UserInfo Users, Logger Log)
        {
            TC_InputMaterial Temp = new TC_InputMaterial();
            //DB_APCSMaterialControl.DB_InputMaterial Temp = new DB_APCSMaterialControl.DB_InputMaterial();
            List<APCSMaterialInfo> ReturnList = new List<APCSMaterialInfo>();

            ////---------SetPart-----------
            for (int i = 0; i < MaterialInfos.Count; i++)
            {
                switch (MaterialInfos[i].ProductionName)
                {
                    case "Kanagata":
                        KanagataInfo KanaInfo = new KanagataInfo();
                        KanaInfo = Temp.LoadKanagataInfo(Machines.Name, MaterialInfos[i].QRCode);
                        MaterialInfos[i].Pass = KanaInfo.IsPass;
                        Temp.ErrorMessage = KanaInfo.ErrorMessage;
                        if (!MaterialInfos[i].Pass) { MaterialInfos[i].ErrorMessage = Temp.ErrorMessage; }
                        ReturnList.Add(new APCSMaterialInfo(MaterialInfos[i]));
                        break;
                }
            }
            return ReturnList;
        }
        private MachineComInfo GetMachineComInfos(MachineInfo Machines, Logger Log)
        {
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetMachineNameList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@machine_id", SqlDbType.Int) { Value = Machines.Id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "Select * From mc.machines as MC with(nolock) inner join mc.model_comms as MDC with(nolock) on MDC.machine_model_id = MC.machine_model_id where MC.id = @machine_id", Log.SqlLogger, sqlparameters);
            MachineComInfo McComm = new MachineComInfo(tmp1);

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@com_id", SqlDbType.Int) { Value = McComm.Id };

            DataTable tmp2 = db.SelectData(FunctionName, dbObject, "SELECT comm_id, ceid, sort_num, rptid, descriptions FROM mc.comm_ceid_rptids as COM WHERE COM.comm_id = @com_id", Log.SqlLogger, sqlparameters);
            McComm.Set_CEIDS(tmp2);

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@com_id", SqlDbType.Int) { Value = McComm.Id };

            DataTable tmp3 = db.SelectData(FunctionName, dbObject, "SELECT comm_id, rptid, sort_num, vid, descriptions FROM mc.comm_rptid_vids AS RPT WHERE RPT.comm_id = @com_id", Log.SqlLogger, sqlparameters);
            McComm.Set_RPTIDS(tmp3);

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@com_id", SqlDbType.Int) { Value = McComm.Id };

            DataTable tmp4 = db.SelectData(FunctionName, dbObject, "SELECT machine_id, comm_id, rs232c_com_no, local_port_no, secs_device_id, remote_ip, remote_port FROM mc.machine_comm_settings AS COMS WHERE COMS.comm_id = @com_id", Log.SqlLogger, sqlparameters);
            McComm.Set_Comms(tmp4);

            dbObject.ConnectionClose();
            tmp1 = null;
            tmp2 = null;
            tmp3 = null;
            tmp4 = null;

            return McComm;
        }



        public int GetReelCount(LotInfo Lots, Logger Log)
        {
            DB_GetConverter DBCONVERT = new DB_GetConverter();
            String FunctionName = MethodBase.GetCurrentMethod().Name;
            Stopwatch functionTimer = new Stopwatch();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            System.Data.SqlClient.SqlParameter[] sqlparameters;
            Log = Log ?? new Logger();
            //if (Log == null) { Log = new Logger(); }

            Log.SqlLogger.SaveFunctionLog_Start(DateTime.Now, FunctionName, "IN", "", "iLibrary", "", "", "");
            functionTimer.Start();

            /*-------------------GetMachineNameList----------------*/
            DateTimeInfo datetime = Get_DateTimeInfo(db, dbObject, Log);
            dbObject.ConnectionOpen();

            sqlparameters = new System.Data.SqlClient.SqlParameter[1];
            sqlparameters[0] = new System.Data.SqlClient.SqlParameter("@lot_id", SqlDbType.Int) { Value = Lots.Id };

            DataTable tmp1 = db.SelectData(FunctionName, dbObject, "select DN.pcs_per_pack from trans.lots as LO with(nolock) inner join method.device_names as DN with(nolock) on DN.id = LO.act_device_name_id where LO.id = @lot_id", Log.SqlLogger, sqlparameters);
           
            if ( tmp1.Rows.Count < 1 ) { return 0; }
            int count = DBCONVERT.GetInt32(tmp1.Rows[0], "pcs_per_pack");
            return count;
        }
        #endregion
    }
}

