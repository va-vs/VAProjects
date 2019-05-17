using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;
namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    public class Works
    {

        public static long GetWorksIDByPriod(long periodID)
        {
            string account = DAL.Common.GetLoginAccount;
            DataSet ds = DAL.Works.GetWorksByPeriodAndAccount(periodID, account);
            long worksID = 0;
            if (ds.Tables[0].Rows.Count > 0)
                worksID = (long)ds.Tables[0].Rows[0]["WorksID"];
            return worksID;
        }
        /// <summary>
        /// 老师给出的样例分real single 
        /// 样例作品评价训练，3-不通过，4-通过
        /// </summary>
        /// <param name="worksID"></param>
        /// <returns></returns>
        public static long GetScoreResult(long worksID, Single studentScore)
        {
            DataSet ds = DAL.Works.GetWorksByWorksID(worksID);
            Single standScore = 0;
            if (ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Rows[0].IsNull("Score"))
                standScore = (Single)ds.Tables[0].Rows[0]["Score"];
            if (studentScore >= standScore * 0.95 && studentScore <= standScore * 1.05)
                return 4;
            else
                return 3;
        }
        /// <summary>
        /// 样例评分
        /// </summary>
        /// <returns></returns>
        public static long SampleScore(long worksExpertID, long worksID, long expertID, SortedList<int, int> scoreDeltails, string txtPingYu)
        {
            using (SqlTransaction trans = DAL.DataProvider.CurrentTransactionEx)
                try
                {
                    float scores = 0;
                    for (int i = 0; i < scoreDeltails.Count; i++)
                    {
                        int standardID = scoreDeltails.Keys[i];
                        scores += scoreDeltails[standardID];
                    }
                    long flag = GetScoreResult(worksID, scores);
                    DataSet ds = DAL.Works.GetWorksExpertByID(worksExpertID);

                    DataRow drWorksExpert;
                    if (ds.Tables[0].Rows.Count > 0)
                        drWorksExpert = ds.Tables[0].Rows[0];
                    else
                    {
                        drWorksExpert = ds.Tables[0].NewRow();
                        drWorksExpert["WorksID"] = worksID;
                        drWorksExpert["ExpertID"] = expertID;
                    }
                    drWorksExpert["Comments"] = txtPingYu;
                    drWorksExpert["Score"] = scores;
                    drWorksExpert["Flag"] = flag;//样式作品评价训练通过
                    if (worksExpertID == 0)
                    {
                        drWorksExpert["Created"] = DateTime.Now;
                        worksExpertID = DAL.User.InsertWorksExpert(drWorksExpert);

                    }
                    else
                    {
                        drWorksExpert["Modified"] = DateTime.Now;
                        DAL.User.UpdateWorksExpert(drWorksExpert);
                    }

                    DataTable dtDetail = DAL.Works.GetRatingDetailsByWorksExpertID(worksExpertID).Tables[0];
                    for (int i = 0; i < scoreDeltails.Count; i++)
                    {
                        int standardID = scoreDeltails.Keys[i];
                        DataRow dr;
                        DataRow[] drs;
                        drs = dtDetail.Select("StandardID=" + standardID);
                        if (drs.Length > 0)
                            dr = drs[0];
                        else
                        {
                            dr = dtDetail.NewRow();
                            dr["WorksExpertID"] = worksExpertID;
                            dr["StandardID"] = standardID;
                            dr["Flag"] = 1;
                        }
                        dr["Score"] = scoreDeltails[standardID];
                        if (drs.Length == 0)
                            DAL.Works.InsertRatingDetails(trans, dr);
                        else
                            DAL.Works.UpdateRatingDetails(trans, dr);
                    }
                    trans.Commit();
                    return flag;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return 0;
                }
        }
        /// <summary>
        /// 作品评分
        /// </summary>
        /// <returns></returns>
        public static int WorksScore(long worksExpertID, long worksID, long expertID, SortedList<int, int> scoreDeltails, string txtPingYu)
        {
            DataTable dtDetail = DAL.Works.GetRatingDetailsByWorksExpertID(worksExpertID).Tables[0];

            using (SqlTransaction trans = DAL.DataProvider.CurrentTransactionEx)
                try
                {
                    int scores = 0;
                    for (int i = 0; i < scoreDeltails.Count; i++)
                    {
                        int standardID = scoreDeltails.Keys[i];
                        DataRow dr;
                        DataRow[] drs;
                        drs = dtDetail.Select("StandardID=" + standardID);
                        if (drs.Length > 0)
                            dr = drs[0];
                        else
                        {
                            dr = dtDetail.NewRow();
                            dr["WorksExpertID"] = worksExpertID;
                            dr["StandardID"] = standardID;
                            dr["Flag"] = 1;
                        }
                        dr["Score"] = scoreDeltails[standardID];
                        if (drs.Length == 0)
                            DAL.Works.InsertRatingDetails(trans, dr);
                        else
                            DAL.Works.UpdateRatingDetails(trans, dr);

                        scores += scoreDeltails[standardID];
                    }
                    DataRow drWorksExpert = DAL.Works.GetWorksExpertByID(worksExpertID).Tables[0].Rows[0];
                    drWorksExpert["Score"] = scores;
                    drWorksExpert["Comments"] = txtPingYu;
                    drWorksExpert["Modified"] = DateTime.Now;

                    DAL.Works.UpdateWorksExpert(trans, drWorksExpert);
                    trans.Commit();
                    return 1;
                }
                catch
                {
                    trans.Rollback();
                    return 0;
                }


        }

        public static long GetPeriodID()
        {
            long courseID = 0;
            string name = DAL.Common.SPWeb.Name;
            DataSet ds = DAL.Course.GetCourseByName(name);
            long periodID = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                courseID = (long)ds.Tables[0].Rows[0]["CourseID"];
                ds = DAL.Periods.GetPeriodByCourseID(courseID);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    periodID = (long)ds.Tables[0].Rows[0][0];
                }
            }
            return periodID;
        }

        /// <summary>
        /// 创建列表
        /// </summary>
        public static SPList CreateList(string listName)
        {

            //  "workVideo" "workPic",  "workFile"

            SPListCollection col = DAL.Common.SPWeb.Lists;
            Guid guid;
            if (listName == "workPic")
            {
                guid = col.Add(listName, "", SPListTemplateType.PictureLibrary);

            }
            else
            {
                guid = col.Add(listName, "", SPListTemplateType.DocumentLibrary);

            }
            SPList list = col.GetList(guid, false);
            list.Hidden = true;
            list.Update();
            return list;
            
        }
    }
}
