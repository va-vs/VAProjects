using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    public class WorksType
    {
        /// <summary>
        /// 获取顶级作品类型
        /// </summary>
        /// <returns></returns>
        public static DataSet GetWorksTypeTopLevel()
        {
            DataSet ds = DAL.Works.GetWorksType();
            DataRow[] drs = ds.Tables[0].Select("ParentID=0");
            DataSet dsTmp = ds.Clone();
            dsTmp.Merge(drs);
            return dsTmp;
        }
        public static DataSet GetWorksFileByTypeID(long worksID, int typeID)
        {
            DataSet ds = DAL.Works.GetWorksFile(worksID, typeID);
            if (typeID < 4)
            {
                DataSet dsTmp = ds.Clone();
                DataRow[] drs = ds.Tables[0].Select("Type=" + typeID);
                dsTmp.Merge(drs);
                return dsTmp;
            }
            return ds;
        }
    
      
    }
}
