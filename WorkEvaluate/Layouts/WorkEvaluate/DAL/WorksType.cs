using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    public class WorksType
    {
        public static DataSet GetWorksTypeScoreStandardByTypeID(int worksTypeID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksTypeScoreStandardByTypeID", worksTypeID);
            return ds;

        }
        public static int InsertWorksType(DataRow dr)
        {
            return ((int)SqlHelper.ExecuteNonQueryTypedParamsOutput(DataProvider.ConnectionString, "InsertWorksType", dr)[0].Value);
        }
        /// <summary>
        /// 无返回ID
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static int InsertWorksTypeScoreStandard(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "InsertWorksTypeScoreStandard", dr));
        }
        public static long UpdateWorksType(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksType", dr));
        }
        public static long UpdateWorksTypeScoreStandard(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksTypeScoreStandard", dr));
        }
    }
}
