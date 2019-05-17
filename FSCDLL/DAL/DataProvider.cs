using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace FSCDLL.DAL
{
    /// <summary>
    /// 与Sql数据库的连接信息
    /// </summary>
    public class DataProvider
    {
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
        /// 获得连接字符串，连接字符串写在web.config中
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (connectionString == null || connectionString.Length == 0)
                {
                    string connString = WebConfigurationManager.ConnectionStrings["SqlProviderFSC"].ConnectionString;
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
