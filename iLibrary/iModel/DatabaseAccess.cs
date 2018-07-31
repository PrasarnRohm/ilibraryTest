using Rohm.Common.Logging;
using System;
using System.Data;
using System.Diagnostics;
using Rohm.Common.Logging;

namespace iLibrary
{
    public class DatabaseAccess
    {
        public DataTable SelectData(DatabaseAccessObject databaseAccessObject, string cmdText, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {            
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                Stopwatch sqlTimer = new Stopwatch();
                sqlTimer.Start();
                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                sqlTimer.Stop();
                SaveSqlLog(cmdText, parameterText, sqlTimer.ElapsedMilliseconds, DateTime.Now);

                DataTable dt = CreateSchemaDataTable(reader);

                // 結果を表示します。
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }
                    dt.Rows.Add(row);
                }
                reader.Close();
                return dt;
            }
        }

        public DataTable SelectData(string FunctionName, DatabaseAccessObject databaseAccessObject, string cmdText, LoggerObject Log, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                Stopwatch sqlTimer = new Stopwatch();

                Log.SaveFunctionLog_Start(DateTime.Now, FunctionName, "SQLIN", "", "iLibrary", "", cmd.CommandText, parameterText);
                sqlTimer.Start();
                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                sqlTimer.Stop();
                //Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "SQLOUT", "", "iLibrary", sqlTimer.ElapsedMilliseconds, "", Log.ReaderToCSV(reader), "");

                DataTable dt = CreateSchemaDataTable(reader);

                // 結果を表示します。
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader.GetValue(i);
                    }
                    dt.Rows.Add(row);
                }
                Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "SQLOUT", "", "iLibrary", sqlTimer.ElapsedMilliseconds, "", Log.ReaderToCSV(dt), "");
                reader.Close();
                return dt;
            }
        }

        public System.Data.SqlClient.SqlDataReader SelectDataReader(DatabaseAccessObject databaseAccessObject, string cmdText, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                            
                    
                Stopwatch sqlTimer = new Stopwatch();
                sqlTimer.Start();

                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();

                sqlTimer.Stop();
                SaveSqlLog(cmdText, parameterText, sqlTimer.ElapsedMilliseconds, DateTime.Now);
                return reader;
            }
        }

        public System.Data.SqlClient.SqlDataReader SelectDataReader(string FunctionName, DatabaseAccessObject databaseAccessObject, string cmdText, LoggerObject Log, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }

                Stopwatch sqlTimer = new Stopwatch();
                
                Log.SaveFunctionLog_Start(DateTime.Now, FunctionName, "SQLIN", "", "iLibrary", "", cmd.CommandText, parameterText);
                sqlTimer.Start();
                System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                sqlTimer.Stop();
                //Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "SQLOUT", "", "iLibrary", sqlTimer.ElapsedMilliseconds, "", Log.ReaderToCSV(reader), "");
                return reader;
            }
        }

        public int SelectExist(DatabaseAccessObject databaseAccessObject, string cmdText, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                using (System.Data.SqlClient.SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter())
                {
                    DataTable data = new DataTable();
                    Stopwatch sqlTimer = new Stopwatch();
                    sqlTimer.Start();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(data);
                    sqlTimer.Stop();
                    SaveSqlLog(cmdText, parameterText, sqlTimer.ElapsedMilliseconds, DateTime.Now);
                    return data.Rows.Count;
                }
            }
        }

        public int SelectExist(string FunctionName, DatabaseAccessObject databaseAccessObject, string cmdText, LoggerObject Log, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            string parameterText = "";
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                using (System.Data.SqlClient.SqlDataAdapter dataAdapter = new System.Data.SqlClient.SqlDataAdapter())
                {
                    DataTable data = new DataTable();
                    Stopwatch sqlTimer = new Stopwatch();
                    Log.SaveFunctionLog_Start(DateTime.Now, FunctionName, "SQLIN", "", "iLibrary", "", cmd.CommandText, parameterText);
                    sqlTimer.Start();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(data);
                    sqlTimer.Stop();
                    Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "SQLOUT", "", "iLibrary", sqlTimer.ElapsedMilliseconds, "", Log.DataTableToCSV(data), "");
                    return data.Rows.Count;
                }
            }
        }

        public int OperateData(DatabaseAccessObject databaseAccessObject, string cmdText, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                string parameterText = "";
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }                
                Stopwatch sqlTimer = new Stopwatch();
                sqlTimer.Start();
                int affectedRow = cmd.ExecuteNonQuery();
                sqlTimer.Stop();
                SaveSqlLog(cmdText, parameterText, sqlTimer.ElapsedMilliseconds, DateTime.Now);
                return affectedRow;
            }
        }

        public int OperateData(string FunctionName, DatabaseAccessObject databaseAccessObject, string cmdText, LoggerObject Log, params System.Data.SqlClient.SqlParameter[] parameterArray)
        {
            using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(cmdText, databaseAccessObject.Connection))
            {
                string parameterText = "";
                cmd.Transaction = databaseAccessObject.Transaction;
                if (parameterArray.Length != 0)
                {
                    int parameterCount = 0;
                    foreach (System.Data.SqlClient.SqlParameter parameter in parameterArray)
                    {
                        cmd.Parameters.Add(parameter);
                        if (parameterCount != 0)
                        {
                            parameterText += ",";
                        }
                        parameterText += parameter.ParameterName + "(" + parameter.SqlDbType + ") = " + parameter.Value;
                        parameterCount += 1;
                    }
                }
                Stopwatch sqlTimer = new Stopwatch();
                Log.SaveFunctionLog_Start(DateTime.Now, FunctionName, "SQLIN", "", "iLibrary", "", cmd.CommandText, parameterText);
                sqlTimer.Start();
                int affectedRow = cmd.ExecuteNonQuery();
                sqlTimer.Stop();
                Log.SaveFunctionLog_End(DateTime.Now, 0, FunctionName, "SQLOUT", "", "iLibrary", sqlTimer.ElapsedMilliseconds, "", affectedRow.ToString() + " Records." , "");
                return affectedRow;
            }
        }

        private DataTable CreateSchemaDataTable(System.Data.SqlClient.SqlDataReader reader)
        {
            if (reader == null) { return null; }
            if (reader.IsClosed) { return null; }

            DataTable schema = reader.GetSchemaTable();
            DataTable dt = new DataTable();

            foreach (DataRow row in schema.Rows)
            {
                // Column情報を追加してください。
                DataColumn col = new DataColumn();
                col.ColumnName = row["ColumnName"].ToString();
                col.DataType = Type.GetType(row["DataType"].ToString());

                if (col.DataType.Equals(typeof(string)))
                {
                    col.MaxLength = (int)row["ColumnSize"];
                }

                dt.Columns.Add(col);
            }
            return dt;
        }

        private bool SaveSqlLog(int ErrorCode, string FunctionName, string TypeStatus, string ProcessTime, string FunctionNumber, string Text1, string Text2)
        {
            Logger Log = new Logger();
            //savelogcode
            Log.SqlLogger.Write(ErrorCode, FunctionName, TypeStatus, "iLibrary", "Cellcon", double.Parse(ProcessTime), FunctionNumber, Text1, Text2);
            return true;
        }
        private bool SaveSqlLog(string cmdText,string parameterText, long runtime, DateTime now )
        {
            //savelogcode
            return true;
        }
        private bool SaveSqlLog(string cmdText, long runtime, DateTime now)
        {
            //savelogcode
            return true;
        }
    }
}
