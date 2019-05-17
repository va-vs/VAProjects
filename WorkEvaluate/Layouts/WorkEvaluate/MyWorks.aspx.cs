using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using WorkEvaluate.Layouts.WorkEvaluate.DAL;
using WorkEvaluateForMy.Layouts.WorkEvaluateForMy.DAL;


namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class MyWorks : LayoutsPageBase
    {
        #region 变量
        /// <summary>
        /// 变量与控件定义
        /// </summary>
        private string Useraccount;
        private string UAccout = "userb";
        private SPWeb web; //当前网站
        //protected GridView gvMyWorks;
        private Label error;
        SPUser currentUser = SPContext.Current.Web.CurrentUser;
        
        
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
            //初始化"我的作品列表" gvMyWorks组件事件
            gvMyWorks.PageIndexChanging += new GridViewPageEventHandler(gvMyWorks_PageIndexChanging);
            gvMyWorks.RowDataBound += new GridViewRowEventHandler(gvMyWorks_RowDataBound);
            gvMyWorks.RowCommand += new GridViewCommandEventHandler(gvMyWorks_RowCommand);

        }

        void gvMyWorks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Useraccount = currentUser.LoginName.ToString();
            Useraccount = Useraccount.Substring(Useraccount.IndexOf('\\') + 1);

            GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent));
            string worksId = drv.Cells[0].Text.Trim();
            DataTable dt = DAL.Works.GetMyWorks(Useraccount).Tables[0];
            int nn = dt.Rows.Count;
            DataRow[] drbyWorksId = dt.Select("WorksID=" + worksId + "");
            nn = drbyWorksId.Length;
            DataTable dtMyWorks = DAL.Common.ToDataTable(drbyWorksId);
            long courseId = Convert.ToInt64(dtMyWorks.Rows[0]["CourseID"].ToString());
            DataTable dt2 = DAL.Course.GetCourses().Tables[0];
            DataRow[] drbyCourseId = dt2.Select("CourseID=" + courseId + "");
            DataTable dtthisCourse = DAL.Common.ToDataTable(drbyCourseId);
            string courseUrl = dtthisCourse.Rows[0]["Url"].ToString();
            if (e.CommandName == "ViewWorks")
            {
                Response.Redirect(myWeb.Url +courseUrl+ "_layouts/15/WorkEvaluate/Comments.aspx?View=1&&WorksID=" +worksId);
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>window.open('OnlineEnroll.aspx?WorksID=" + Server.UrlEncode(DESEncrypt.DesEncrypt(drv.Cells[0].Text)) + "&&CreatedBy=" + Server.UrlEncode(DESEncrypt.DesEncrypt(currentUser.LoginName.Substring(currentUser.LoginName.IndexOf("\\") + 1) ))+ "')</script>");
            }
            if (e.CommandName=="EditWorks")
            {
                Response.Redirect(myWeb.Url + courseUrl + "_layouts/15/WorkEvaluate/OnlineEnroll.aspx");
                //ScriptManager.RegisterStartupScript(Page, this.GetType(), "message", "window.opener ='"+myWeb.Url+"" + courseUrl + "/_layouts/15/WorkEvaluate/OnlineEnroll.aspx';window.open('','_blank');", true);
            }
        }
        void gvMyWorks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)// || e.Row.RowType == DataControlRowType.Header)
            {
                //当鼠标停留时更改背景色
                e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#e4e3f7'");
                //当鼠标移开时还原背景色
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
                e.Row.Attributes.Add("style", "height:35px;");
                string worksState = DataBinder.Eval(e.Row.DataItem, "StateName").ToString();
                Button btnEditWorks = (Button)e.Row.FindControl("btnEdit");
                if (worksState == "已保存")
                {
                    //e.Row.BackColor = System.Drawing.Color.Azure;
                    e.Row.Cells[7].ForeColor = System.Drawing.Color.CornflowerBlue;
                }
                else if (worksState == "已提交")
                {
                    //e.Row.BackColor = System.Drawing.Color.PapayaWhip;
                    e.Row.Cells[7].ForeColor = System.Drawing.Color.DarkBlue;
                    btnEditWorks.Enabled = false;
                    btnEditWorks.ToolTip = "作品已提交成功，禁止修改";
                }
                else if (worksState == "评分中")
                {
                    //e.Row.BackColor = System.Drawing.Color.LightPink;
                    e.Row.Cells[7].ForeColor = System.Drawing.Color.CadetBlue;
                    btnEditWorks.Enabled = false;
                    btnEditWorks.ToolTip = "作品已在评分中，禁止修改";
                }
                else if (worksState == "评分完成")
                {
                    //e.Row.BackColor = System.Drawing.Color.LightSlateGray;
                    e.Row.Cells[7].ForeColor = System.Drawing.Color.DeepPink;
                    btnEditWorks.Enabled = false;
                    btnEditWorks.ToolTip = "作品评分已完成，禁止修改";
                }
                else if (worksState == "公示中")
                {
                    //e.Row.BackColor = System.Drawing.Color.LightGray;
                    e.Row.Cells[7].ForeColor = System.Drawing.Color.DarkOrange;
                    btnEditWorks.Enabled = false;
                    btnEditWorks.ToolTip = "作品已在公示中，禁止修改";
                }
                for (int i = 0; i <=7; i++)
                {
                    //方法一： 
                    e.Row.Cells[i].Text = " " + e.Row.Cells[i].Text + " ";
                    e.Row.Cells[i].Wrap = false;
                    
                    //方法二：
                    //e.Row.Cells[i].Text = "<nobr>#nbsp; " + e.Row.Cells[i].Text + " </nobr>"; 
                }
                if (e.Row.Cells[5].Text.Trim() == "0" || string.IsNullOrEmpty(e.Row.Cells[5].Text.Trim()))
                {
                    e.Row.Cells[5].Text = "尚未公布";
                }
                string relationship = DataBinder.Eval(e.Row.DataItem, "Relationship").ToString();
                //int relationship = Convert.ToInt32(e.Row.Cells[6].Text);
                switch (relationship)
                {
                    case "0":
                        e.Row.Cells[6].Text = "★独创作者★";
                        e.Row.Cells[6].ForeColor = System.Drawing.Color.Red;
                        break;
                    case "1":
                        e.Row.Cells[6].Text = "☆团队队长☆";
                        e.Row.Cells[6].ForeColor = System.Drawing.Color.SeaGreen;
                        break;
                    case "2":
                        e.Row.Cells[6].Text = "团队成员";
                        e.Row.Cells[6].ForeColor = System.Drawing.Color.DarkSlateBlue;
                        break;
                }
                
            }
            

        }

        void gvMyWorks_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMyWorks.PageIndex = e.NewPageIndex;
            BindWorks((DataTable)ViewState["worksinfo"]);
        }
        protected void gvMyWorks_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sPage = e.SortExpression;
            if (ViewState["SortOrder"].ToString() == sPage)
            {
                if (ViewState["OrderDire"].ToString() == "Desc")
                    ViewState["OrderDire"] = "ASC";
                else
                    ViewState["OrderDire"] = "Desc";
            }
            else
            {
                ViewState["SortOrder"] = e.SortExpression;
            }
            BindWorks((DataTable)ViewState["worksinfo"]);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Useraccount = currentUser.LoginName.ToString();
                Useraccount=Useraccount.Substring(Useraccount.IndexOf('\\') + 1);

                //我的作品gvMyWorks事件
                gvMyWorks.PagerSettings.Mode = PagerButtons.NumericFirstLast;
                gvMyWorks.PagerSettings.FirstPageText = "1";
                gvMyWorks.DataKeyNames = new string[] { "WorksID" };
                ViewState["SortOrder"] = "Score";
                ViewState["OrderDire"] = "ASC";
                //ddl课程和期次绑定
                BindCourses(DAL.Course.GetCourses().Tables[0]);
                ddlPeriods.Visible = false;
                //BindPeriods(Convert.ToInt64(ddlCourses.SelectedValue));
                if (Useraccount == null)
                {
                    ScriptManager.RegisterStartupScript(Page, this.GetType(), "message", "alert('请先登录');window.opener ='';window.open('','_self');window.close()", true);
                    //return;
                }
                else
                {
                    DataTable dtMyWorks = Works.GetMyWorks(Useraccount).Tables[0];
                    ViewState["worksinfo"] = dtMyWorks;
                    BindWorks(dtMyWorks);
                }
            }
        }
      
        #endregion


        #region 方法
        /// <summary>
        /// 获取当站网站Url
        /// </summary>
         public SPWeb myWeb
        {
            get
            {
                if (web == null)
                {
                    web = SPContext.Current.Web;
                }
                return web;
            }
        }
        /// <summary>
        /// 我的作品列表
        /// </summary>
        /// <param name="dt"></param>
        public void BindWorks(DataTable dt)
        {
            gvMyWorks.DataSource = dt;
            gvMyWorks.DataBind();
        }

        /// <summary>
        /// 绑定课程下拉列表级
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="CourseID"></param>
        public void BindCourses(DataTable dt)
        {
            ddlCourses.Items.Clear();
            ddlCourses.Items.Add(new ListItem("请选择所属课程", "0"));
            //DataRow[] drs = dt.Select("LevelID=0");
            foreach (DataRow dr in dt.Rows)
            {
                ddlCourses.Items.Add(new ListItem(dr["CourseName"].ToString(), dr["CourseID"].ToString()));
            }
        }
        /// <summary>
        /// 绑定期次下拉列表级
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="courseId"></param>
        public void BindPeriods(long courseId)
        {
            DataTable dt = DAL.Periods.GetPeriods().Tables[0];
            //string courseId = ddlCourses.SelectedValue;
            ddlPeriods.Items.Clear();
            ddlPeriods.Items.Add(new ListItem("请选择所属期次", "0"));
            if (courseId >0)
            {
                ddlPeriods.Visible = true;
                clearSets.Visible = true;
                DataRow[] drs = dt.Select("courseId=" + courseId + "");
                long nnn = drs.Length;
                if (drs.Length <= 0)
                {
                    //ddlPeriods.Items.Clear();
                    //ddlPeriods.Items.Add(new ListItem("没有期次", "0"));
                    ddlPeriods.Visible=false;
                    showperiod.Visible = true;
                }
                else
                {
                    showperiod.Visible = false;
                    foreach (DataRow dr in drs)
                    {
                        ddlPeriods.Items.Add(new ListItem(dr["PeriodTitle"].ToString(), dr["PeriodID"].ToString()));
                    }
                }
            }
        }
        #endregion

        protected void ddlCourses_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Useraccount = currentUser.LoginName.ToString();
            Useraccount = Useraccount.Substring(Useraccount.IndexOf('\\') + 1);
            if (Useraccount=="")
            {
                error.Text = "请先登录！";
            }
            else
            {
                long courseId = Convert.ToInt64(ddlCourses.SelectedValue);
                BindPeriods(courseId);
                if (courseId > 0)
                {
                    DataTable dt = DAL.Works.GetMyWorks(Useraccount).Tables[0];
                    int nn = dt.Rows.Count;
                    DataRow[] drbyCourse = dt.Select("CourseID=" + courseId + "");
                    nn = drbyCourse.Length;
                    DataTable dtMyWorks = DAL.Common.ToDataTable(drbyCourse);
                
                    ViewState["worksinfo"] = dtMyWorks;
                    BindWorks(dtMyWorks);
                }
            }
           
        }
        protected void ddlPeriods_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Useraccount = currentUser.LoginName.ToString();
            Useraccount = Useraccount.Substring(Useraccount.IndexOf('\\') + 1);
            if (Useraccount == "")
            {
                error.Text = "请先登录！";
            }
            else
            {
                long periodId = Convert.ToInt64(ddlPeriods.SelectedValue);
                if (periodId > 0)
                {
                    DataTable dt = DAL.Works.GetMyWorks(Useraccount).Tables[0];
                    int nn = dt.Rows.Count;
                    DataRow[] drbyPeriod = dt.Select("PeriodID=" + periodId + "");
                    nn = drbyPeriod.Length;
                    DataTable dtMyWorks = DAL.Common.ToDataTable(drbyPeriod);
                    ViewState["worksinfo"] = dtMyWorks;
                    BindWorks(dtMyWorks);
                }
            }
        }
        /// <summary>
        /// 重新选择筛选条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void clearSets_OnClick(object sender, EventArgs e)
        {
            Useraccount = currentUser.LoginName.ToString();
            Useraccount = Useraccount.Substring(Useraccount.IndexOf('\\') + 1);
            
            //我的作品gvMyWorks事件
            gvMyWorks.PagerSettings.Mode = PagerButtons.NumericFirstLast;
            gvMyWorks.PagerSettings.FirstPageText = "1";
            gvMyWorks.DataKeyNames = new string[] { "WorksID" };

            //ddl课程和期次绑定
            BindCourses(DAL.Course.GetCourses().Tables[0]);
            ddlPeriods.Visible = false;
            DataTable dtMyWorks = Works.GetMyWorks(Useraccount).Tables[0];
            ViewState["worksinfo"] = dtMyWorks;
            BindWorks(dtMyWorks);
            showperiod.Visible = false;
            clearSets.Visible = false;
        }
    }

}
