using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.SharePoint;
using System.Data;
using WorkEvaluate.Layouts.WorkEvaluate.DAL;
namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    public class Course
    {
        /// <summary>
        /// 1、评价训练 2、评分 3、公示点评
        /// </summary>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public static int JudgeDate(long periodID)
        {
            DataSet ds = DAL.Periods.GetPeriodsByID(periodID);
            if (ds.Tables[0].Rows.Count == 0)
                return 0;
            DateTime dtStart = (DateTime)ds.Tables[0].Rows[0]["StartScore"];
            DateTime dtEnd = (DateTime)ds.Tables[0].Rows[0]["EndScore"];
            if (DateTime.Today < dtStart)
                return 1;
            else if (DateTime.Today >= dtStart && DateTime.Today < dtEnd.AddDays(1))
                return 2;
            dtStart = (DateTime)ds.Tables[0].Rows[0]["StartPublic"];
            dtEnd = (DateTime)ds.Tables[0].Rows[0]["EndPublic"];
            if (DateTime.Today >= dtStart && DateTime.Today < dtEnd.AddDays(1))
                return 3;
            return 0;
        }
        public static long GetCourseID()
        {
            DataTable CourseDt = DAL.Course.GetCourseByName(Common.SPWeb.Title).Tables[0];
            if (CourseDt.Rows.Count > 0)
            {

                return long.Parse(CourseDt.Rows[0]["CourseID"].ToString());
            }
            else
            {
                return 0;
            }
        }
    }
}
