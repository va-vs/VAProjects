using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    public class Course
    {
        /// <summary>
        /// 根据课程名称获取课程信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DataSet GetCourseByName(string name)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetCourseByName", name);
            return ds;
        }
        /// <summary>
        /// 获取所有课程信息
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCourses()
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetCourses");
            return ds;
        }
    }
}
