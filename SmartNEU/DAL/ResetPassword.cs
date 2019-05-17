using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SmartNEU.DAL
{
    public class ResetPassword
    {
        public static DataSet GetResetPassword(long operateID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetResetPasswordByID", operateID);
            return ds;
        }
        public static long InsertResetPassword(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertResetPassword", dr)[0].Value);
        }
        public static int UpdateResetPassword(DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(DAL.DataProvider.ConnectionString, "UpdateResetPassword", dr));
        }
    }
}
