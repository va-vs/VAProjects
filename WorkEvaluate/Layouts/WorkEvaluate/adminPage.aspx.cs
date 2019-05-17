using System;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class adminPage : LayoutsPageBase
    {
        private static long periodId =10;
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(DAL.Common.GetLoginAccount);
        }
        private static int AllotNum = 9;
        private static DataTable dtWorksToScore;
        /// <summary>
        /// 为该期次的所有作品分配评分对象
        /// </summary>
        /// <param name="periodId">期次ID</param>
        /// <param name="e"></param>
        protected void setgroup_OnClick(object sender, EventArgs e)
        {
            DataTable dtWaitedUser = DAL.User.GetUserIdByPeriodId(periodId).Tables[0];//初始化数据表以保存待分配的用户ID
            long userId = 0;
            if (dtWaitedUser.Rows.Count > 0)
            {
                for (int i = 0; i < dtWaitedUser.Rows.Count; i++)
                {
                    userId = Convert.ToInt64(dtWaitedUser.Rows[i]["UserID"]);
                    DataTable dtWaitedWorks = DAL.Works.GetWorksToAllot(userId, periodId,AllotNum).Tables[0];
                    if (dtWaitedWorks.Rows.Count > 0)
                    {
                        int allottimes = 0;
                        string[] arrayWaitedWorks = DAL.Common.TableTostrArray(dtWaitedWorks, "WorksID");
                        string[] arrayToAllot = DAL.Common.GetRandomsArray(AllotNum, arrayWaitedWorks);
                        for (int j = 0; j < arrayToAllot.Length; j++)
                        {
                            //插入评分分配新纪录
                            DataRow dr = Allotdt.NewRow();
                            dr["WorksID"] = Convert.ToInt64(arrayToAllot[j]);
                            dr["ExpertID"] = userId;
                            dr["Flag"] = 1;
                            DAL.Works.InsertWorksComments(dr);
                            //为作品分配计数+1
                            DataTable dtAllotTimes =
                                DAL.Works.GetWorksAllotTimesByWorsID(Convert.ToInt64(arrayToAllot[j])).Tables[0];
                            allottimes = Convert.ToInt32(dtAllotTimes.Rows[0]["AllotTimes"]);
                            DataRow dr2 = dtAllotTimes.NewRow();
                            dr2["AllotTimes"] = allottimes + 1;//评分次数+1
                            if (allottimes==8)
                            {
                                dr2["WorksState"] = 2;//最后一次分配,将作品状态置为2:作品评分中
                            }
                            else
                            {
                                dr2["WorksState"] = dr2["WorksState"];//分配未完成,保持状态不变
                            }
                            DAL.Works.UpdateWorksAllotTimes(dr2);
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('该期次没有作品待评分');</script>");
                    }
                }
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('分配成功');</script>");
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('该期次下没有用户参与评分');</script>");
            }
        }
        
        public static void InsertWorksExpert(long periodId)
        {
            
        }

        /// <summary>
        /// 初始化评分分配数据表
        /// </summary>
        public static DataTable Allotdt
        {
            get
            {
                if (dtWorksToScore == null)
                {
                    dtWorksToScore = new DataTable();
                    dtWorksToScore.Columns.Add("WorksExpertID", typeof(long));
                    dtWorksToScore.Columns.Add("WorksID", typeof(string));
                    dtWorksToScore.Columns.Add("ExpertID", typeof(string));
                    dtWorksToScore.Columns.Add("Flag", typeof(int));
                }
                return dtWorksToScore;
            }
        }

        public void MakeDataView(long periodId)
        {
            DataSet workstoallotDs = BLL.WorksScoring.PeriodWorksDataSetExtend(periodId);
            if (workstoallotDs.Tables[0].Rows.Count <= 1) //没有作品
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('作品数太少,等待或催促学生尽快上传作品!');</script>");
            }
            else
            {
                int n = 0;
                if (workstoallotDs.Tables[0].Rows.Count <= 9)//小于9个作品
                {
                    n = workstoallotDs.Tables[0].Rows.Count - 1;
                }
                else
                {
                    n = 9;
                }
                for (int i = 0; i < workstoallotDs.Tables[0].Rows.Count; i++)
                {
                    long eid = Convert.ToInt64(workstoallotDs.Tables[0].Rows[i]["UserID"]);
                    DataView dv = new DataView();
                    dv.Table = workstoallotDs.Tables[0];
                    dv.AllowDelete = true;
                    dv.AllowEdit = true;
                    dv.AllowNew = true;
                    dv.RowFilter = "UserID !="+ eid+"";
                }
                DataSet allotedDs = new DataSet();
                DataRow dr = allotedDs.Tables[0].NewRow();
            }
        }
        // <summary>
        /// 获取对固定列制定规则的新DataTable
        /// </summary>
        /// <param name="dt">含有重复数据的DataTable</param>
        /// <param name="colName">需要验证重复的列名</param>
        /// <returns>新的DataTable，colName列不重复，表格式保持不变</returns>
        private DataTable GetDistinctTable(DataTable dt, long userid)
        {
            DataView dv = dt.DefaultView;
            DataTable dtuserTable = dv.ToTable();
            DataTable dtFilter = new DataTable();//筛选后的数据表
            dtFilter = dv.ToTable();
            dtFilter.Clear();
            for (int i = 0; i < dtuserTable.Rows.Count; i++)
            {
                DataRow dr = dt.Select(userid + "!='" + Convert.ToInt64(dtuserTable.Rows[i]["UserID"]) + "'")[0];
                dtFilter.Rows.Add(dr.ItemArray);
            }
            return dtFilter;
        }

    }
}
