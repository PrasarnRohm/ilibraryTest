using System.Runtime.Serialization;
using System.Collections.Generic;

namespace iLibrary
{

    [DataContract()]
    public class CheckResult
    {
        #region FIELD
        private bool c_IsPass;
        private int c_ErrorNo;
        private string c_ErrorMessage;
        private string c_GotoUrl;
        private Dictionary<int, string> c_StepName;
        private int c_ErrorAtStep;
        private Dictionary<int, string> c_WarningMessage;
        #endregion
        #region CONSTRUCTOR/DESTRUCTOR
        internal CheckResult()
        {
            c_IsPass = false;
        }
        internal CheckResult(Dictionary<int, string> warningMessage, string functionName)
        {
            c_IsPass = true;
            c_StepName = FunctionStepNameProvider.GetStepName(functionName);
            c_WarningMessage = warningMessage;
        }
        internal CheckResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, string functionName, int errorAtStep)
        {
            c_IsPass = false;
            c_ErrorNo = errorNo;
            c_ErrorMessage = ErrorMessageProvider.GetErrorMessage(errorNo);
            c_WarningMessage = warningMessage;
            c_GotoUrl = gotoUrl;
            c_StepName = FunctionStepNameProvider.GetStepName(functionName);
            c_ErrorAtStep = errorAtStep;
        }
        #endregion
        #region PROPERTY
        [DataMember()]
        public bool IsPass
        {
            get { return c_IsPass; }
            private set { c_IsPass = value; }
        }
        [DataMember()]
        public int ErrorNo
        {
            get { return c_ErrorNo; }
            private set { c_ErrorNo = value; }
        }
        [DataMember()]
        public string ErrorMessage
        {
            get { return c_ErrorMessage; }
            private set { c_ErrorMessage = value; }
        }
        [DataMember()]
        public Dictionary<int, string> WarningMessage
        {
            get { return c_WarningMessage; }
            private set { c_WarningMessage = value; }
        }
        [DataMember()]
        public string GotoUrl
        {
            get { return c_GotoUrl; }
            private set { c_GotoUrl = value; }
        }
        [DataMember()]
        public Dictionary<int, string> StepName
        {
            get { return c_StepName; }
            private set { c_StepName = value; }
        }
        [DataMember()]
        public int ErrorAtStep
        {
            get { return c_ErrorAtStep; }
            private set { c_ErrorAtStep = value; }
        }
        [DataMember()]
        internal bool SetIsPass
        {
            set { c_IsPass = value; }
        }
        [DataMember()]
        internal int SetErrorNo
        {
            set { c_ErrorNo = value; }
        }
        [DataMember()]
        internal string SetErrorMessage
        {
            set { c_ErrorMessage = value; }
        }
        #endregion
    }

    public class GetRecipeResult : CheckResult
    {
        private string c_Recipe1 = "";
        private string c_Recipe2 = "";

        #region CONSTRUCTOR/DESTRUCTOR

        internal GetRecipeResult() : base() { }
        internal GetRecipeResult(Dictionary<int, string> warningMessage) : base(warningMessage, "CheckMachineCondition") { }
        internal GetRecipeResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckMachineCondition", errorAtStep) { }
        #endregion
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

    public class CheckMachineConditionResult : CheckResult
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal CheckMachineConditionResult() : base() { }
        internal CheckMachineConditionResult(Dictionary<int, string> warningMessage) : base(warningMessage, "CheckMachineCondition") { }
        internal CheckMachineConditionResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckMachineCondition", errorAtStep) { }
        #endregion
    }
    public class CheckLotConditionResult : CheckResult
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal CheckLotConditionResult() : base() { }
        internal CheckLotConditionResult(Dictionary<int, string> warningMessage) : base(warningMessage, "CheckLotCondition") { }
        internal CheckLotConditionResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckLotCondition", errorAtStep) { }
        #endregion
    }
    public class CheckLotAndMachineCompatibilityResult : CheckResult
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal CheckLotAndMachineCompatibilityResult() : base() { }
        internal CheckLotAndMachineCompatibilityResult(Dictionary<int, string> warningMessage)
            : base(warningMessage, "CheckLotAndMachineCompatibility") { }
        internal CheckLotAndMachineCompatibilityResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckLotAndMachineCompatibility", errorAtStep) { }
        #endregion
    }
    public class CheckUserPermissionResult : CheckResult
    {
        #region CONSTRUCTOR/DESTRUCTOR
        internal CheckUserPermissionResult() : base() { }
        internal CheckUserPermissionResult(Dictionary<int, string> warningMessage) : base(warningMessage, "CheckUserPermission") { }
        internal CheckUserPermissionResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckUserPermission", errorAtStep) { }
        #endregion
    }
    public class CheckProcessFlowResult : CheckResult
    {
        private int c_Lot_Id;
        private int c_Division_Id;
        private bool c_Is_SpFlow;
        #region CONSTRUCTOR/DESTRUCTOR
        public CheckProcessFlowResult() : base() { c_Is_SpFlow = false; c_Division_Id = -1; c_Lot_Id = -1; }
        public CheckProcessFlowResult(Dictionary<int, string> warningMessage, bool Is_SpFlow, int Division_Id, int Lot_Id) : base(warningMessage, "CheckProcessFlowResult") { c_Is_SpFlow = Is_SpFlow; c_Division_Id = Division_Id; c_Lot_Id = Lot_Id; }
        public CheckProcessFlowResult(int errorNo, Dictionary<int, string> warningMessage, bool Is_SpFlow, string gotoUrl, int errorAtStep)
            : base(errorNo, warningMessage, gotoUrl, "CheckProcessFlowResult", errorAtStep) { c_Is_SpFlow = Is_SpFlow; }
        #endregion
        public int Lot_Id
        {
            get { return c_Lot_Id; }
            set { c_Lot_Id = value; }
        }
        public int Division_Id
        {
            get { return c_Division_Id; }
            set { c_Division_Id = value; }
        }
        public bool Is_SpFlow
        {
            get { return c_Is_SpFlow; }
            set { c_Is_SpFlow = value; }
        }
    }
    public class CheckJigResult : CheckResult
    {
        private int c_id;
        private string c_barcode;
        private int c_Type_ID;
        private string c_Type_Name;

        #region CONSTRUCTOR/DESTRUCTOR
        public CheckJigResult() : base() { c_id = -1; c_barcode = null; c_Type_ID = -1; c_Type_Name = null; }
        public CheckJigResult(Dictionary<int, string> warningMessage, int jig_id, string barcode, int type_id, string type_name) : base(warningMessage, "CheckJigResult") { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        public CheckJigResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep, int jig_id, string barcode, int type_id, string type_name)
            : base(errorNo, warningMessage, gotoUrl, "CheckJigResult", errorAtStep) { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        #endregion

        public int Id
        {
            get { return c_id; }
            set { c_id = value; }
        }
        public string Barcode
        {
            get { return c_barcode; }
            set { c_barcode = value; }
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
        
    }

    public class CheckMaterialResult : CheckResult
    {
        private int c_id;
        private string c_barcode;
        private int c_Type_ID;
        private string c_Type_Name;

        #region CONSTRUCTOR/DESTRUCTOR
        public CheckMaterialResult() : base() { c_id = -1; c_barcode = null; }
        public CheckMaterialResult(Dictionary<int, string> warningMessage, int jig_id, string barcode, int type_id, string type_name) : base(warningMessage, "CheckMaterialResult") { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        public CheckMaterialResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep, int jig_id, string barcode, int type_id, string type_name)
            : base(errorNo, warningMessage, gotoUrl, "CheckMaterialResult", errorAtStep) { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        #endregion

        public int Id
        {
            get { return c_id; }
            set { c_id = value; }
        }
        public string Barcode
        {
            get { return c_barcode; }
            set { c_barcode = value; }
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
    }

    public class CheckEQPResult : CheckResult
    {
        private int c_id;
        private string c_barcode;
        private int c_Type_ID;
        private string c_Type_Name;

        #region CONSTRUCTOR/DESTRUCTOR
        public CheckEQPResult() : base() { c_id = -1; c_barcode = null; }
        public CheckEQPResult(Dictionary<int, string> warningMessage, int jig_id, string barcode, int type_id, string type_name) : base(warningMessage, "CheckEQPResult") { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        public CheckEQPResult(int errorNo, Dictionary<int, string> warningMessage, string gotoUrl, int errorAtStep, int jig_id, string barcode, int type_id, string type_name)
            : base(errorNo, warningMessage, gotoUrl, "CheckEQPResult", errorAtStep) { c_id = jig_id; c_barcode = barcode; c_Type_ID = type_id; c_Type_Name = type_name; }
        #endregion

        public int Id
        {
            get { return c_id; }
            set { c_id = value; }
        }
        public string Barcode
        {
            get { return c_barcode; }
            set { c_barcode = value; }
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
    }
}