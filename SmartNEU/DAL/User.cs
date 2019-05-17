using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SmartNEU.DAL
{
    class User
    {
          public static DataSet GetSchool(int AreaID, int schoolId)
        {
            DataSet ds =DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetSchool", AreaID, schoolId);
            return ds;
        }
        public static DataSet GetSchoolById(int schoolId)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetSchoolByID", schoolId);
            return ds;
        }
        public static DataSet GetOrg(int parentID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetOrg", parentID);
            return ds;
        }
        public static DataSet GetCity(int parentID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetCity", parentID);
            return ds;
        }
        public static DataSet GetRole()
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetRole");
            return ds;
        }
        public static DataSet GetUserByAccount(string account)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUserByAccount", account);
            return ds;
        }
        public static long InsertUser(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertUser", dr)[0].Value);
        }

        public static int UpdateUser(DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(DAL.DataProvider.ConnectionString, "UpdateUser", dr));
        }
    }
}
