using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iLibrary
{
    #region Kyoto
    public static class Errors_Common
    {
        public static int UserIsNotRegisted            = 1;
        public static int UserResigned                 = 2;
        public static int UserExpired                  = 3;
        public static int UserDoNotHaveLicense         = 4;
        public static int LicenseExpired               = 5;
        public static int MachineIsNotRegisted         = 6;
        public static int MachineDisable               = 7;
        public static int MachineDisableByQC           = 8;
        public static int MachineReservationDisable    = 9;
        public static int LotDoesNotExist              = 10;
        public static int MachineIsNotAutomotive       = 11;
        public static int DoNotMatchLotandMachine      = 12;
        public static int AlreadySetup                 = 13;
        public static int AlreadyStart                 = 14;
        public static int LotWasAbnormalEnd            = 15;
        public static int LotWasAbnormal               = 16;
        public static int LotWasHoldByQC               = 17;
        public static int LotWasHold                   = 18;

        public static int MachineIsMaintenancing       = 21;

        public static int MachineOffline               = 23;
        public static int MachineAMHS                  = 24;
        public static int MachineDoNotHaveStateData    = 25;
        public static int LotDoNotHaveStateData        = 26;
        public static int MachineDoNotHaveJobData      = 27;
        public static int LotDoNotHaveJobData          = 28;
        public static int DoNotExistData               = 29;
    }

    public static class Errors_CheckProcessFlow
    {
        public static int CanNotSetup                  = 100;
        public static int CanNotStart                  = 101;
        public static int CanNotEnd                    = 102;
        public static int CanNotCancel                 = 103;
        public static int StrangeStates                = 104;
        public static int LotQCNG                      = 105;
        public static int PackageNotEnable             = 106;
        public static int MachineQCNG                  = 107;
        public static int MachineOnlineStateIsWorng    = 108;
        public static int NotAutoMotive                = 109;

        public static int LotDoNotWip                  = 112;
        public static int SPLotDoNotWip                = 113;
        public static int CanNotOnlineStart            = 114;
        public static int CanNotOnlineEnd              = 115;

        public static int MachineIsNothing             = 116;
        public static int ToMutchLot                   = 117;

        public static int LotQC_Abnormal               = 118;
        public static int LotQC_Stop                   = 119;
        public static int LotQC_Hold                   = 120;
        public static int LotQC_SPFlow                 = 121;

        public static int MachineQCAbnormalStop        = 122;
        public static int MachineQCReserveStop         = 123;
        public static int WipStateIsNotInput           = 124;
    }
    public static class Errors_LotSetup
    {
        public static int SpecialFlowIsNothing         = 500;
        public static int SpecialFlowStateIsNotIdle    = 501;
        public static int CanNotUpdateSpecialFlowState = 502;
        public static int CanNotInsertSpecialFlowState = 503;
        public static int CanNotUpdateMachineState     = 504;
        public static int CanNotUpdateLotState         = 505;
        public static int CanNotInsertLotRecord        = 506;
        public static int CanNotInsertLotPjs           = 507;
        public static int CanNotInsertPjLots           = 508;
    }
    public static class Errors_GetRecipe
    {
        public static int LotRecipeDataIsNothig = 1200;
        public static int MachinePPIDTypeIsNothing = 1201;
    }
    class Errors_LotStart
    {

    }
    class Errors_OnlineStart
    {

    }
    class Errors_LotCancel
    {

    }
    class Errors_OnlineLotEnd
    {

    }
    class Errors_LotEnd
    {
        public static int SpecialFlowIsNothing = 300;
        public static int SpecialFlowStateIsNotStart = 301;
        public static int CanNotUpdateSpecialFlowState = 302;
        public static int CanNotInsertSpecialFlowState = 303;
        public static int CanNotUpdateMachineState = 304;
        public static int CanNotUpdateLotState = 305;
        public static int CanNotInsertLotRecord = 306;
        public static int LotDidNotInspection = 307;
    }
    class Errors_Firstinspection
    {

    }
    class Errors_Finalinspection
    {

    }
    class Errors_ErrorHappenRecord
    {

    }
    class Errors_ErrorResetRecord
    {

    }
    class Errors_ErrorRecoveryRecord
    {

    }
    class Errors_GetSQLServerTime
    {

    }
    //class Errors_GetSQLServerTime
    //{

    //}
    #endregion Kyoto   
}
