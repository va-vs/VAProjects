using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
   public class Periods
    {
        
       public static long UpdatePeriodsByID(DataRow dr)
       {
           return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdatePeriodsByID", dr));
       }
       public static long DelPeriodsByID(DataRow dr)
       {
           return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "DelPeriodsByID", dr));
       }

       public static DataSet GetPeriodsByID(long PeriodID)
       {
           DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetPeriodsByID ", PeriodID);
           return ds;
       }
       public static long UpdatePeriods(DataRow dr)
       {
           return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdatePeriods", dr));
       }
       public static long InsertPeriods(DataRow dr)
       {
           return ((long)SqlHelper.ExecuteNonQueryTypedParamsOutput(DataProvider.ConnectionString, "InsertPeriods", dr)[0].Value);
       }
       public static DataSet GetPeriodByCourseID(long CourseID)
       {
           DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetPeriodByCourseID", CourseID);
           return ds;
       }
       /// <summary>
       /// 获取所有期次
       /// </summary>
       /// <returns></returns>
       public static DataSet GetPeriods()
       {
           DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetPeriods");
           return ds;
       }
       /// <summary>
       /// 根据用户ID获取该用户创建的所有期次
       /// </summary>
       /// <param name="userId">用户ID</param>
       /// <returns></returns>
       public static DataSet GetPeriodByUserId(long userId)
       {
           DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetPeriodByUserID", userId);
           return ds;
       }
    }
}
