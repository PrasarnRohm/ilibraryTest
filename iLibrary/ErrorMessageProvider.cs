using System.Collections.Generic;
using System.Data;

namespace iLibrary
{
    public class ErrorMessageProvider
    {

        #region "Shared member"
        private static Dictionary<string, Dictionary<int, string>> m_ErrorMessageDictionary;
        public static string m_Language;
        static ErrorMessageProvider()
        {
            m_ErrorMessageDictionary = new Dictionary<string, Dictionary<int, string>>();
            DatabaseAccess db = new DatabaseAccess();
            DatabaseAccessObject dbObject = new DatabaseAccessObject();
            dbObject.ConnectionOpen();
            DataTable errorData = db.SelectData(dbObject, "select lang,code,message from mdm.errors where app_name = 'iLibrary' order by lang,code");
            dbObject.ConnectionClose();
            string language = "";
            Dictionary<int, string> errorMassageDictionnary = new Dictionary<int, string>();
            foreach (DataRow row in errorData.Rows)
            {
                if (language == "")
                {
                    language = row.Field<string>("lang");
                }
                if (language != row.Field<string>("lang"))
                {
                    m_ErrorMessageDictionary.Add(language, errorMassageDictionnary);
                    language = row.Field<string>("lang");
                    errorMassageDictionnary = new Dictionary<int, string>();
                }
                errorMassageDictionnary.Add(row.Field<int>("code"), row.Field<string>("message"));
            }
            m_ErrorMessageDictionary.Add(language, errorMassageDictionnary);
            m_Language = "Eng";
        }

        public static string GetErrorMessage(int errorNo)
        {            
            Dictionary<int, string> errorMessageDictionary = m_ErrorMessageDictionary[m_Language];
            string errorMessage = null;
            
            try
            {

                errorMessage = errorMessageDictionary[errorNo];
            }
            catch (System.Exception)
            {

            }
            finally 
            {

            }
            if (errorMessage == null)
            {
                if (m_Language == "Tha")
                {
                 
                   
                    return "ไม่มีข้อมูล Err ของ ภาษาไทย ในระบบ (" + errorNo.ToString ()  + ")";
                
                }
                else if (m_Language == "Jpn")
                {
                    return "システム内の日本語のデータがない（" + errorNo.ToString() + "）"; 
                }
                else
                {
                    return "No Err data of english language in system(" + errorNo.ToString ()  + ")";
                }
            }
            else
            {
                return errorMessage;
            }
        }
        public static string Language
        {
            get { return m_Language; }
            set { m_Language = value; }
        }
        private ErrorMessageProvider() { }
        #endregion
    }
}
