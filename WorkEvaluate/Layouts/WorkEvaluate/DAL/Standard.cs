using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    public  class Standard
    {
        public static long UpdatePeriodStandard(DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(DAL.DataProvider.ConnectionString, "UpdatePeriodStandard", dr));
        }
        /// <summary>
        /// 作品评分明细
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static long InsertPeriodStandard(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "InsertPeriodStandard", dr));
        }
    }
}
