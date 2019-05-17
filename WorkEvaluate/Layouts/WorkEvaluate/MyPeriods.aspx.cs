using System;
using System.Data;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WorkEvaluate.Layouts.WorkEvaluate.BLL;
using WorkEvaluateForMy.Layouts.WorkEvaluateForMy.DAL;
using System.Web.UI.HtmlControls;

namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class MyPeriods : LayoutsPageBase
    {
        #region 变量
        private string Useraccount;
        private static DataTable dtWorksToScore;
        SPUser currentUser = SPContext.Current.Web.CurrentUser;

        /// <summary>
        /// 作品
        /// </summary>
        private DataTable periodsDt;
        public DataTable PeriodsDt
        {
            get
            {
                if (periodsDt == null)
                {
                    periodsDt = new DataTable();
                    periodsDt.Columns.Add("PeriodID", typeof(long));
                    //periodsDt.Columns.Add("CourseName", typeof(string));
                    periodsDt.Columns.Add("PeriodTitle", typeof(string));
                    periodsDt.Columns.Add("WorksCount", typeof(string));

                }
                return periodsDt;
            }

        }
        #endregion
        #region 事件
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitializeComponent()
        {
            gvPeriod.RowCommand += gvPeriod_RowCommand;
            gvPeriod.RowDataBound += gvPeriod_RowDataBound;
        }
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //workscountlb.Text = workscount.ToString();
                if (!BLL.User.JudgeUserRight())
                {
                    error.Text = "";
                    long userId = BLL.User.GetUserID(currentUser);
                    ViewState["CreatedBy"] = userId;
                    DataTable dt = DAL.Periods.GetPeriodByUserId(userId).Tables[0];
                    BindGvDate(dt);
                    BindCourses(DAL.Course.GetCourses().Tables[0]);
                }
                else
                {
                    gvPeriod.Visible = false;
                    ddlCourses.Visible = false;
                    error.Text = "当前用户不可操作";
                }
                //AllotNumTB.Text = "";
            }
        }
        /// <summary>
        /// 重新选择筛选条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearSets_OnClick(object sender, EventArgs e)
        {
            long userId = BLL.User.GetUserID(currentUser);
            //我的作品gvMyWorks事件
            gvPeriod.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            gvPeriod.PagerSettings.FirstPageText = "1";
            gvPeriod.DataKeyNames = new string[] { "PeriodID" };

            //ddl课程和期次绑定
            BindCourses(DAL.Course.GetCourses().Tables[0]);
            ViewState["CreatedBy"] = userId;
            DataTable dtMyPeriods = DAL.Periods.GetPeriodByUserId(userId).Tables[0];
            BindGvDate(dtMyPeriods);
            gvPeriod.Visible = true;
            clearSets.Visible = false;
            error.Visible = false;
        }
        /// <summary>
        /// 行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gvPeriod_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long periodId = long.Parse(((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent)).Cells[0].Text);//根据点击行，获取对应期次ID
            if (e.CommandName == "Del")//删除期次，即将期次的Flag置为0
            {
                DataRow dr = PeriodsDt.NewRow();
                dr["PeriodID"] = periodId;
                DAL.Periods.DelPeriodsByID(dr);
                DataTable dt = DAL.Periods.GetPeriodByUserId(long.Parse(ViewState["CreatedBy"].ToString())).Tables[0];
                BindGvDate(dt);
            }
            if (e.CommandName == "Standard")//评价指标，为该期次制定评价指标
            {
                DAL.Common.OpenWindow(this.Page, DAL.Common.SPWeb.Url + "/_layouts/15/WorkEvaluate/" + "ScoreStandard.aspx?PeriodID=" + periodId.ToString());
            }
        
            if (e.CommandName == "Upload")//为该期次上传样例作品，以供学生评分训练
            {
                DAL.Common.OpenWindow(this.Page, DAL.Common.SPWeb.Url + "/_layouts/15/WorkEvaluate/" + "OnlineEnroll.aspx?IsSample=1&&PeriodID=" + periodId.ToString());
            }

            if (e.CommandName == "Alloting") //评分分配，按照作品总数，确定互评每组人数，然后随机分配作品给每个用户
            {
                long workscount = Convert.ToInt64(DAL.Works.GetWorksNumByPeriodID(periodId).Tables[0].Rows[0][0].ToString());
                if (workscount <= 2)
                {

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message",
                        "<script defer>alert('作品数仅有" + workscount + "件，总数太少,请催促学生尽快提交作品!');</script>");
                }
                else
                {
                    BLL.Period.WorksAlloting(periodId,Allotdt);
                }
            }
            if (e.CommandName == "ComputingScore")
            {
                DateTime[] dateArr = BLL.Period.GetPeridTimeSets(periodId);//获取当前期次的时间设置
                DateTime nowDateTime = DateTime.Now;
                if (nowDateTime > dateArr[3].AddDays(1))//期次评分阶段结束
                {
                    BLL.WorksScoring.ComputerAllScoresByPeriod(periodId);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('本期成绩已公布!');</script>");
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('作品还未到成绩公布时间,或已过期,请确认所处做出时间阶段后再试!');</script>");
                }

            }
        }
        
        #endregion
        /// <summary>
        /// 绑定期次列表
        /// </summary>
        private void BindGvDate(DataTable dt)
        {

            BindGridView(dt, gvPeriod);
        }
        /// <summary>
        /// 绑定列表
        /// </summary>
        private void BindGridView(DataTable dt, GridView gv)
        {
            gv.DataSource = dt;
            gv.DataBind();
        }
        /// <summary>
        /// 绑定课程下拉列表级
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="CourseID"></param>
        private void BindCourses(DataTable dt)
        {
            ddlCourses.Items.Clear();
            ddlCourses.Items.Add(new ListItem("请选择所属课程", "0"));
            //DataRow[] drs = dt.Select("LevelID=0");
            foreach (DataRow dr in dt.Rows)
            {
                ddlCourses.Items.Add(new ListItem(dr["CourseName"].ToString(), dr["CourseID"].ToString()));
            }
        }
        protected void ddlCourses_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            string userAccount = currentUser.LoginName.ToString();
            userAccount = userAccount.Substring(userAccount.IndexOf('\\') + 1);
            if (userAccount == "")
            {
                error.Text = "请先登录！";
            }
            else
            {
                long userId = BLL.User.GetUserID(currentUser);
                long courseId = long.Parse(ddlCourses.SelectedValue);
                if (courseId > 0)
                {

                    DataTable dt = DAL.Periods.GetPeriodByUserId(userId).Tables[0];
                    int nn = dt.Rows.Count;
                    DataRow[] drbyCourse = dt.Select("CourseID=" + courseId + "");
                    nn = drbyCourse.Length;
                    if (nn>0)
                    {
                        DataTable dtMyPeriods = DAL.Common.ToDataTable(drbyCourse);
                        ViewState["CreatedBy"] = dtMyPeriods;
                        BindGvDate(dtMyPeriods);

                        gvPeriod.Visible = true;
                        error.Text = "";
                    }
                    else
                    {
                        gvPeriod.Visible = false;
                        error.Text = "该课程下你没有创建期次！";
                    }
                    clearSets.Visible = true;
                }
                else
                {
                    clearSets.Visible = false;
                    gvPeriod.Visible = true;
                }
            }

        }
        /// <summary>
        /// 初始化评分分配数据表
        /// </summary>
        private static DataTable Allotdt
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
    
        /// <summary>
        /// 期次列表行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvPeriod_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow) //判断当前行是否是数据行
            {
                //当鼠标停留时更改背景色
                e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#e4e3f7'");
                //当鼠标移开时还原背景色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
                e.Row.Attributes.Add("style", "height:35px;");

                Label lbpState = (Label)e.Row.FindControl("lbPeriodState");//用FindControl方法找到模板中的Label控件
                Button btnUpload = (Button)e.Row.FindControl("btnUpload");
                Button btnStandard = (Button)e.Row.FindControl("btnStandard");
                Button btnAllot = (Button)e.Row.FindControl("btnAllot");
                Button btnScore = (Button)e.Row.FindControl("btnScore");
                Button btnDel = (Button)e.Row.FindControl("btnDel");
                DateTime[] dtArr =BLL.Period.GetPeridTimeSets(long.Parse(e.Row.Cells[0].Text));
                DateTime nowDateTime = DateTime.Now;
                if (nowDateTime < dtArr[0])
                {
                    lbpState.Text = "尚未开始";
                    lbpState.ForeColor = System.Drawing.Color.Red;
                    btnStandard.Enabled = false;
                    btnStandard.ToolTip = "当前状态下不可操作";
                    btnUpload.Enabled = false;
                    btnUpload.ToolTip = "当前状态下不可操作";
                    btnAllot.Enabled = false;
                    btnAllot.ToolTip = "当前状态下不可操作";
                    btnScore.Enabled = false;
                    btnScore.ToolTip = "当前状态下不可操作";
                    btnDel.Enabled = true;
                }
                if (dtArr[0] <= nowDateTime && nowDateTime <= dtArr[1].AddDays(1))//上传作品时期
                {
                    lbpState.Text = "作品提交期";
                    lbpState.ForeColor = System.Drawing.Color.Green;
                    btnStandard.Enabled = true;
                    btnUpload.Enabled = true;
                    btnAllot.Enabled=false;
                    btnAllot.ToolTip = "当前状态下不可操作";
                    btnScore.Enabled = false;
                    btnScore.ToolTip = "当前状态下不可操作";
                    btnDel.Enabled = false;
                    btnDel.ToolTip = "当前状态下不可操作";
                }
                else if (dtArr[1].AddDays(1) < nowDateTime && nowDateTime <= dtArr[3].AddDays(1))
                {
                    lbpState.Text = "作品评分期";
                    lbpState.ForeColor = System.Drawing.Color.Blue;
                    btnStandard.Enabled = false;
                    btnStandard.ToolTip = "当前状态下不可操作";
                    btnUpload.Enabled = false;
                    btnUpload.ToolTip = "当前状态下不可操作";
                    btnAllot.Enabled = true;
                    btnScore.Enabled = false;
                    btnScore.ToolTip = "当前状态下不可操作";
                    btnDel.Enabled = false;
                    btnDel.ToolTip = "当前状态下不可操作";
                }
                else if (dtArr[3].AddDays(1) < nowDateTime && nowDateTime <= dtArr[5].AddDays(1))
                {
                    lbpState.Text = "作品公示期";
                    lbpState.ForeColor = System.Drawing.Color.DarkSlateGray;
                    btnStandard.Enabled = false;
                    btnStandard.ToolTip = "当前状态下不可操作";
                    btnUpload.Enabled = false;
                    btnUpload.ToolTip = "当前状态下不可操作";
                    btnAllot.Enabled = false;
                    btnAllot.ToolTip = "当前状态下不可操作";
                    btnScore.Enabled = true;
                    btnDel.Enabled = false;
                    btnDel.ToolTip = "当前状态下不可操作";
                }
                else if (nowDateTime > dtArr[5])
                {
                    lbpState.Text = "期次已结束";
                    lbpState.ForeColor = System.Drawing.Color.DarkRed;
                    btnStandard.Enabled = false;
                    btnStandard.ToolTip = "当前状态下不可操作";
                    btnUpload.Enabled = false;
                    btnUpload.ToolTip = "当前状态下不可操作";
                    btnAllot.Enabled = false;
                    btnAllot.ToolTip = "当前状态下不可操作";
                    btnScore.Enabled = false;
                    btnScore.ToolTip = "当前状态下不可操作";
                    btnDel.Enabled = true;
                    //btnDel.ToolTip = "当前状态下不可操作";
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                //方法一： 
                e.Row.Cells[i].Text = "&nbsp;" + e.Row.Cells[i].Text + "&nbsp;";
                e.Row.Cells[i].Wrap = false;
                //方法二：
                //e.Row.Cells[i].Text = "<nobr>" + e.Row.Cells[i].Text + " </nobr>"; 
            }
        }
     
    }
}
