using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    /// <summary>
    /// 提供数据操作的基本方法，如取得数据集，链接数据库，
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// 链接字符串
        /// </summary>
        private static string connectionString;
        /// <summary>
        /// 连接对象
        /// </summary>
        private static SqlConnection currentConnection;

        /// <summary>
        /// 公用事务
        /// </summary>
        public SqlTransaction CurrentTransaction
        {
            get
            {
                if (currentConnection != null && currentConnection.State != ConnectionState.Open)
                    currentConnection.Open();
                return currentConnection.BeginTransaction(IsolationLevel.ReadCommitted);

            }
        }

       

        public static SqlTransaction CurrentTransactionEx
        {
            get
            {
                SqlConnection connection = new SqlConnection(ConnectionString);
                connection.Open();
                return connection.BeginTransaction(IsolationLevel.ReadCommitted);
            }
        }

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (connectionString == null || connectionString.Length == 0)
                {
                    string connString = WebConfigurationManager.ConnectionStrings["SqlProvider"].ConnectionString; ;
                    SqlConnection conn = new SqlConnection(connString);
                    try
                    {
                        conn.Open();
                        currentConnection = conn;
                        connectionString = connString;
                    }
                    catch (Exception ex)
                    {
                        connectionString = string.Empty;
                        throw ex;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                return connectionString;

            }

            set
            {
                SqlConnection conn = new SqlConnection(value);
                try
                {
                    conn.Open();
                    currentConnection = conn;
                    connectionString = value;
                }
                catch (Exception ex)
                {
                    connectionString = string.Empty;
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

    }
}
