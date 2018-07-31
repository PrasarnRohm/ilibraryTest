using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace iLibrary
{
    
    public class DatabaseAccessObject
    {
        private SqlConnection m_Connection;
        private SqlTransaction m_Transaction;
        private string m_ConnectionString;
        
        public SqlConnection Connection
        {
            get { return m_Connection; }
        }
        
        public SqlTransaction Transaction
        {
            get { return m_Transaction; }
        }
        
        public string ConnectionString
        {
            get { return m_ConnectionString; }
        }
        public DatabaseAccessObject()
        {
            m_ConnectionString = AppSettingHelper.GetConnectionStringValue("ApcsProConnectionString");
            m_Connection = new SqlConnection(m_ConnectionString);
        }
        public DatabaseAccessObject(string connectionStringName)
        {
            m_ConnectionString = AppSettingHelper.GetConnectionStringValue(connectionStringName);
            m_Connection = new SqlConnection(m_ConnectionString);
        }
        public void BeginTransaction()
        {
            m_Transaction = m_Connection.BeginTransaction();
        }
        public void BeginTransaction(IsolationLevel isolationLevel)
        {
            m_Transaction = m_Connection.BeginTransaction(isolationLevel);
        }
        public void ConnectionOpen()
        {
            m_Connection.Open();
        }
        public void TransactionCommit()
        {
            m_Transaction.Commit();
            m_Transaction = null;
        }
        public int ConnectionClose()
        {
            if (m_Transaction != null)
            {
                m_Transaction.Rollback();
                m_Transaction = null;
                m_Connection.Close();
                return 1; // Not Commit -> Rollback
            }
            m_Connection.Close();
            return 0; // OK
        }
        public void TransactionCancel()
        {
            m_Transaction.Rollback();
            m_Transaction = null;
        }
    }
}
