using System.Collections.Generic;
using System.Data;

namespace iLibrary
{
    public class FunctionStepNameProvider
    {
        #region "Shared member"

        private static Dictionary<string, Dictionary<string, Dictionary<int, string>>> m_FunctionStepNameDictionary;

        public static string m_Language;

        static FunctionStepNameProvider()
        {
            m_FunctionStepNameDictionary = new Dictionary<string, Dictionary<string, Dictionary<int, string>>>();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable functionStepNameData = db.SelectData(dbObject, "select lang,functionName,step,stepName from dbo.functionStepName inner join dbo.functionName on functionStepName.functionNameId = functionName.id order by lang,functionName,step");
            dbObject.ConnectionClose();
            string language = "";
            string functionName = "";
            Dictionary<int, string> StepNameDictionnary = new Dictionary<int, string>();
            Dictionary<string, Dictionary<int, string>> functionNameDictionnary = new Dictionary<string, Dictionary<int, string>>();
            foreach (DataRow row in functionStepNameData.Rows)
            {
                if (language == "")
                {
                    language = row.Field<string>("lang");
                }
                if (language != row.Field<string>("lang"))
                {
                    functionNameDictionnary.Add(functionName, StepNameDictionnary);
                    functionName = "";
                    m_FunctionStepNameDictionary.Add(language, functionNameDictionnary);
                    language = row.Field<string>("lang");
                    functionNameDictionnary = new Dictionary<string, Dictionary<int, string>>();
                    StepNameDictionnary = new Dictionary<int, string>();
                }
                if (functionName == "")
                {
                    functionName = row.Field<string>("functionName");
                }
                if (functionName != row.Field<string>("functionName"))
                {
                    functionNameDictionnary.Add(functionName, StepNameDictionnary);
                    functionName = row.Field<string>("functionName");
                    StepNameDictionnary = new Dictionary<int, string>();
                }
                StepNameDictionnary.Add(row.Field<int>("step"), row.Field<string>("stepName"));
            }
            functionNameDictionnary.Add(functionName, StepNameDictionnary);
            m_FunctionStepNameDictionary.Add(language, functionNameDictionnary);
            m_Language = "Eng";
        }

        public static Dictionary<int,string> GetStepName(string functionName)
        {
            Dictionary<int, string> StepNameDictionary = m_FunctionStepNameDictionary[m_Language][functionName];
            if (StepNameDictionary.Count == 0)
            {
                if (m_Language == "Tha")
                {
                    return new Dictionary<int, string> { { 1, "ไม่มีข้อมูลเสต็ป" } };
                }
                else if (m_Language == "Jpn")
                {
                    return new Dictionary<int, string> { { 1, "" } };
                }
                else
                {
                    return new Dictionary<int, string> { { 1, "Step data not found" } };
                }
            }
            else
            {
                return StepNameDictionary;
            }
        }

        public static string Language
        {
            get { return m_Language; }
            set { m_Language = value; }
        }
        private FunctionStepNameProvider() { }

        #endregion
    }
}
