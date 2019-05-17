using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    public class Period
    {
        /// <summary>
        /// 根据期次ID获取期次的时间设置,并存入数组dateArr
        /// </summary>
        /// <param name="periodId">期次ID</param>
        /// <returns></returns>
        public static DateTime[] GetPeridTimeSets(long periodId)
        {
            DataTable dtPeriods = DAL.Periods.GetPeriodsByID(periodId).Tables[0];
            DateTime[] dateArr = new DateTime[6];//设置数组保存每个时期的六个时间值(上传开始、结束；评分开始、结束；公示开始、结束。)
            if (dtPeriods.Rows.Count > 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    dateArr[i] = DateTime.Parse(dtPeriods.Rows[0][i + 6].ToString());
                }
            }
            return dateArr;
        }
        /// <summary>
        /// 根据作品总数确定每个评分人可分配的作品个数
        /// </summary>
        /// <param name="worksCount">作品总数</param>
        /// <returns></returns>
        public static long SetallotNum(long worksCount)
        {
            long allotNum = 0;
            if (worksCount > 2 & worksCount <= 9)//作品总数在2-9之间，个数=总数-1
            {
                allotNum = worksCount - 1;
            }
            else if (worksCount > 9)        //作品总数大于9，个数等于9
            {
                allotNum = 9;
            }
            return allotNum;
        }
        /// <summary>
        /// 期次作品互评分配
        /// </summary>
        /// <param name="periodId"></param>
        /// <param name="resulDt"></param>
        public static void WorksAlloting(long periodId, DataTable resulDt)
        {
            //long workscount = long.Parse(DAL.Works.GetWorksNumByPeriodID(periodId).Tables[0].Rows[0][0].ToString());//获取当前期次上传的作品数量,不包含样例作品

            long allotNum = BLL.Period.SetallotNum(long.Parse(DAL.Works.GetWorksNumByPeriodID(periodId).Tables[0].Rows[0][0].ToString()));
            DataTable dtWaitedUser = DAL.User.GetUserIdByPeriodId(periodId).Tables[0]; //初始化数据表以保存待分配的用户ID
            if (dtWaitedUser.Rows.Count > 0)//确定本期次可以参与互评的人数
            {
                for (int i = 0; i < dtWaitedUser.Rows.Count; i++)
                {
                    long expertId = long.Parse(dtWaitedUser.Rows[i]["UserID"].ToString());
                    long hasAlloted = DAL.Works.GetWorksForMeByPeriodId(periodId, expertId).Tables[0].Rows.Count;
                    if (hasAlloted < allotNum)   //获取该用户已经分配的作品数，若未满则继续分配，否则则跳过
                    {
                        DataTable dtWaitedWorks = DAL.Works.GetWorksToAllot(expertId, periodId, allotNum).Tables[0];//
                        if (dtWaitedWorks.Rows.Count > 0)
                        {
                            string[] arrayWaitedWorks = DAL.Common.TableTostrArray(dtWaitedWorks, "WorksID");//待分配作品表转化为数组

                            long shortOf = allotNum - hasAlloted;//获取该用户还差需要分配的作品数
                            string[] arrayToAllot = DAL.Common.GetRandomsArray(shortOf, arrayWaitedWorks);

                            for (int j = 0; j < arrayToAllot.Length; j++)//插入评分分配新纪录
                            {
                                long worksId = long.Parse(arrayToAllot[j].ToString());
                                DataRow dr = resulDt.NewRow();
                                dr["WorksID"] = worksId; //作品ID
                                dr["ExpertID"] = expertId; //评分用户ID
                                //dr["Created"] = DateTime.Now;
                                dr["Flag"] = 1;
                                long aaa = DAL.Works.InsertWorksComments(dr);
                                //为作品分配计数+1,最后一次重置作品状态为2:已分配,评分中
                                DataTable dtAllotTimes = DAL.Works.GetWorksAllotTimesByWorsID(worksId).Tables[0];
                                long allottimes = long.Parse(dtAllotTimes.Rows[0]["AllotTimes"].ToString());
                                DataRow drAllotTimes = dtAllotTimes.Rows[0];
                                drAllotTimes["WorksID"] = worksId;
                                drAllotTimes["AllotTimes"] = allottimes + 1;
                                if (allottimes == allotNum - 1)
                                {
                                    drAllotTimes["WorksState"] = 2; //最后一次分配,将作品状态置为2:作品评分中
                                }
                                else
                                {
                                    drAllotTimes["WorksState"] = Convert.ToInt32(drAllotTimes["WorksState"].ToString()); //分配未完成,保持状态不变
                                }
                                //Response.Write("WorksID:"+drAllotTimes["WorksID"] + "AllotTimes:" + drAllotTimes["AllotTimes"] + "WorksState:" + drAllotTimes["WorksState"]+"<br/>");
                                DAL.Works.UpdateWorksAllotTimes(drAllotTimes);
                            }
                        }
                    }
                }
            }
        }
    }
}
