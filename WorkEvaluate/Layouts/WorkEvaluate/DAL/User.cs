using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    public class User
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public static DataSet GetUserByAccount(string account)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUserByAccount", account);
            return ds;
        }
        /// <summary>
        /// 添加用户信息 
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns></returns>
        public static long InsertUser(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertUser", dr)[0].Value);
        }
        public static DataSet GetUserByWorksID(long WorksID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUserByWorksID", WorksID);
            return ds;
        }
        public static long UpdateUserWorks(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateUserWorks", dr));
        }

        /// <summary>
        /// 将作品分配给用户进行评分，无返回行数据 
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns></returns>
        public static long InsertWorksExpert(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertWorksExpert", dr)[0].Value);
        }
        /// <summary>
        /// 获取本期次所有已经上传了作品的用户且可参与评价的用户
        /// </summary>
        /// <param name="periodId">本期次ID</param>
        /// <returns></returns>
        public static long UpdateWorksExpert(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksExpert", dr));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodId"></param>
        /// <returns></returns>
        public static DataSet GetUserIdByPeriodId(long periodId)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUsersIDByPeriodID", periodId);
            return ds;
        }

        /// <summary>
        /// 添加用户作品关系表，无返回行数据 
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <returns></returns>
        public static long InsertUserWorks(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertUserWorks", dr)[0].Value);
        }

        public static DataSet GetUserByUserID(long UserID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUserByUserID", UserID);
            return ds;
        }
    }
}
