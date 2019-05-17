using System;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class Comments : System.Web.UI.Page
    {
        #region 显示控件定义
        protected Label lblWorksName;
        protected Label lblWorksCode;
        protected Label lblWorksType;
        protected Label lblSubmitProfile;

        protected GridView gvComments;
        protected HtmlGenericControl divWorksFile;
        protected Label lblPersons;
        protected HtmlGenericControl divScore;
        //点评
        protected HtmlGenericControl divPublicComments;//公式期点评
        protected HiddenField HiddenField2;
        protected Button btnSubmitShow;
        protected TextBox txtComments;
        protected HiddenField HiddenField1Show;
        protected Label lblDemoURL;
        protected HtmlGenericControl divDesignIdeas;
        protected HtmlGenericControl divViewShow;
        protected HtmlGenericControl divDocView;
        protected HtmlGenericControl divKeyPoints;
        protected HtmlGenericControl divDovVideo;
        #endregion
        #region 评分控件定义
        protected HtmlGenericControl divWorksScore;
        protected Button btnSubmit;
        protected Button btnSave;
        protected Label lblTotalScore;
        protected HtmlGenericControl divContent;
        protected Button btnClose;
        protected TextBox txtScorePingYu;
        protected HiddenField HiddenField1;
        protected UpdatePanel UpdatePanel1;
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DAL.Common.SPWeb.CurrentUser == null)
            {
                ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "message", "alert('请先登录');document.location.replace('"+DAL.Common.SPWeb.Url +"');", true);
                return;
            }
            if (WorksID == 0) return;
            btnSubmit.Click += new EventHandler(btnSubmint_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
            if (HiddenField1.Value != "-1")
                lblTotalScore.Text = HiddenField1.Value;
            InitControl();
            //查看比较评过的分数
            if (!Page.IsPostBack)
            {
                if (IsViewScore)
                {
                    ViewComments(false);
                }
                else if (PeriodTime == 2)
                {
                    ViewComments(true);
                }
            }
            //作品相关的

            btnSubmitShow.Click += new EventHandler(btnSubmitShow_Click);
            if (!Page.IsPostBack)
            {
                InitControlWorksShow();
            }
            FillDocView();//doc
            ShowControlDisplay();

        }
        #region 评分相关的
        #region 属性
        /// <summary>
        /// 返回当前作品其次的时间段1、评价训练2、评分、3、公示点评
        /// </summary>
        private int PeriodTime
        {
            get
            {
                if (ViewState["periodTime"]==null )
                {
                    ViewState["periodTime"] = BLL.Course.JudgeDate(PeriodID);
                }
                return (int)ViewState["periodTime"];
            }
        }
        private DataSet DSWorksType
        {
            get
            {
                if (ViewState["dsWorksType"] == null)
                {
                    DataSet ds = DAL.Works.GetWorksType();
                    ViewState["dsWorksType"] = ds;
                }
                return (DataSet)ViewState["dsWorksType"];
            }
        }
        private long PeriodID
        {
            get
            {
                if (Request.QueryString["PeriodID"] != null && Request.QueryString["PeriodID"]!="")
                    return long.Parse(Request.QueryString["PeriodID"]);
                else
                    return 0;
            }
        }
        private int IsSample
        {
            get
            {
                if (Request.QueryString["IsSample"] != null)
                    return int.Parse(Request.QueryString["IsSample"]);
                else
                    return 0;
            }
        }
        /// <summary>
        /// 评分和点评控件何时显示
        /// </summary>
        private void ShowControlDisplay()
        {
            if (IsViewWorks)
            {
                divPublicComments.Attributes.Add("style", "display:none");
                divWorksScore.Attributes.Add("style", "display:none");
                return;
            }
            if (IsSample == 0)
            {
                if (PeriodTime == 3)
                {
                    divPublicComments.Attributes.Add("style", "display:");
                    divWorksScore.Attributes.Add("style", "display:none");
                }
                else if (PeriodTime == 2)
                {
                    divPublicComments.Attributes.Add("style", "display:none");
                    divWorksScore.Attributes.Add("style", "display:");
                }
                else
                {
                    divPublicComments.Attributes.Add("style", "display:none");
                    divWorksScore.Attributes.Add("style", "display:none");
                }
            }
            else
            {
                if (PeriodTime != 1)
                {
                    divPublicComments.Attributes.Add("style", "display:none");
                    divWorksScore.Attributes.Add("style", "display:none");
                }


            }
        }
        private DataSet DsCurrentWorks
        {
            get
            {

                if (ViewState["DsWorks"] == null)
                {
                    DataSet dsTmp;
                    if (IsViewWorks)
                    { 
                        dsTmp = DAL.Works.GetWorksByWorksID(WorksID);
                        ViewState["DsWorks"] = dsTmp;
                        return (DataSet)ViewState["DsWorks"];
                    }

                    DataSet dsSearch;
                    DataRow[] drs;
                    if (PeriodTime == 3)
                        dsSearch = DAL.Works.GetWorksByPeriodID(PeriodID);
                    else
                        dsSearch = DAL.Works.GetWorksToEvaluate(DAL.Common.LoginID, PeriodID, IsSample);

                    dsTmp = dsSearch.Clone();
                    if (IsSample == 1 && PeriodTime==1 )
                    {
                        SamplePassed = 0;
                        if (dsSearch.Tables[0].Rows.Count == 0)//样例作品
                        {
                            dsTmp = DAL.Works.GetSampleWorksByPeriodID(PeriodID);
                        }
                        else
                        {//未通过的
                            drs = dsSearch.Tables[0].Select("Flag=4");
                            if (drs.Length > 0)
                            {
                                SamplePassed = 1;
                                ShowMessage (UpdatePanel1, "您已经通过评价训练！");
                                dsTmp.Merge(drs);
                                btnSubmit.Enabled = false;
                            }
                            else
                            {
                                //sui ji sample
                                dsTmp = DAL.Works.GetSampleWorksByPeriodID(PeriodID);
                                if (dsTmp.Tables[0].Rows.Count == 0)
                                {
                                    drs = dsSearch.Tables[0].Select("Flag=3");
                                    dsTmp.Merge(drs);
                                }
                                else
                                {
                                    foreach (DataRow dr in dsTmp.Tables[0].Rows  )
                                    {
                                        drs = dsSearch.Tables[0].Select("WorksID="+dr["WorksID"] +" and ExpertID="+DAL.Common.LoginID); //是否已经点评过
                                        if (drs.Length ==0)
                                        {
                                            DataSet dsSamples = dsTmp.Clone();
                                            dsSamples.Merge(new DataRow[]{  dr});
                                            dsTmp = dsSamples.Copy();
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else if (IsSample ==0)
                    {
                        drs = dsSearch.Tables[0].Select("WorksID=" +WorksID.ToString() );
                        dsTmp = dsSearch.Clone();
                        dsTmp.Merge(drs);
                    }
                    ViewState["DsWorks"] = dsTmp;
                }
                return (DataSet)ViewState["DsWorks"];
            }
        }
        /// <summary>
        /// 状态、0为保存，1为提交
        /// </summary>
        private int ScoreState
        {
            get
            {
            DataSet ds=DsCurrentWorks;
            if (ds.Tables[0].Rows.Count > 0)
            {

            }
            return 0;
            }
        }
        private int samplePassed = 0;
        private int SamplePassed
        {
            get
            {
                return samplePassed;
            }
            set
            {
                samplePassed = value; 

            }
        }
        //查看作品，不查看分数
        private bool IsViewWorks
        {
            get
            {

                if (Request.QueryString["View"] == "1")
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 样例通过可以查看分数
        /// </summary>
        private bool IsViewScore
        {
            get
            {

                if ( SamplePassed ==1)
                    return true;
                else
                    return false;
            }
        }
        private DataSet DSScoreDetails
        {
            get
            {
                if (ViewState["dsScoreDetails"] == null)
                {
                    DataSet ds = DsCurrentWorks;
                    long worksExpertID = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                        if (ds.Tables[0].Columns.Contains("WorksExpertID"))
                            worksExpertID = (long)ds.Tables[0].Rows[0]["WorksExpertID"];
                    ds = DAL.Works.GetRatingDetailsByWorksExpertID(worksExpertID);
                    ViewState["dsScoreDetails"] = ds;
                }
                return (DataSet)ViewState["dsScoreDetails"];
            }
        }
        //获取所有类别
        private DataSet DSScoreStandardSubLevel
        {
            get
            {
                if (ViewState["dsSublevel"] == null)
                {
                    DataSet ds = DAL.Works.GetScoreStandardSubLevel();
                    ViewState["dsSublevel"] = ds;
                }
                return (DataSet)ViewState["dsSublevel"];
            }
        }
      
        #endregion
        #region 事件
   
        void btnClose_Click(object sender, EventArgs e)
        {
        }
        void btnSubmint_Click(object sender, EventArgs e)
        {
            string txtPingYu = txtScorePingYu.Text;
            if ( txtPingYu.Length <30)
            {
                ShowMessage (UpdatePanel1 ,"评语不能少于30字" );
                return;
            }
            SortedList<int, int> scores1 = new SortedList<int, int>();//按添加时的顺序
            foreach (Control ctr in this.divContent.Controls)
            {
                if (ctr.GetType() == typeof(Panel))
                {
                    string id = ctr.ID;
                    string standardID = id.Substring(3);
                    Panel panel = (Panel)ctr;
                    foreach (Control subCtr in panel.Controls)
                    {
                        if (subCtr.GetType() == typeof(TextBox))
                        {
                            TextBox txtBox = (TextBox)subCtr;
                            scores1.Add(int.Parse(standardID), int.Parse(txtBox.Text));
                        }
                    }
                }
            }
            if (DsCurrentWorks.Tables[0].Rows.Count >0 )
            {
                if (IsSample == 1)
                    SaveSampleScore(scores1);
                else
                    SaveWorksScore(scores1);
            }
           
        }
        /// <summary>
        /// 保存样例成绩
        /// </summary>
        private void SaveSampleScore(SortedList<int, int> scores1)
        {
            DataSet ds = DsCurrentWorks;
            long result = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow DRCurrentWorks = ds.Tables[0].Rows[0];
                long worksExpertID = 0;
                long userID = DAL.Common.LoginID;
                if (ds.Tables[0].Columns.Contains("WorksExpertID"))
                {
                    worksExpertID = (long)DRCurrentWorks["WorksExpertID"];
                    userID = (long)DRCurrentWorks["ExpertID"];
                }
                else if (userID==0 )
                {
                    userID = BLL.User.GetUserID(DAL.Common.SPWeb.CurrentUser);
                }

                result = BLL.Works.SampleScore(worksExpertID, (long)DRCurrentWorks["WorksID"], userID, scores1, txtScorePingYu.Text);
                if (result == 4)
                {
                    btnSubmit.Enabled = false;
                    ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "message", "alert('评价练习已经通过');window.opener ='';window.open('','_self');window.close()", true);
                }
                else if (result == 3)
                {
                    ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "message", "alert('评价练习没有通过，请继续评价');document.location.replace('Comments.aspx?PeriodID=" + PeriodID + "&&IsSample=1')", true);
                }
                else if (result == 0)
                {
                    ShowMessage(UpdatePanel1, "提交失败");
                }
            }
        }
        /// <summary>
        /// 保存作品评分
        /// </summary>
        private void SaveWorksScore(SortedList<int, int> scores1)
        {
            DataSet ds = DsCurrentWorks;
            int result = 0;
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow DRCurrentWorks = ds.Tables[0].Rows[0];
                result = BLL.Works.WorksScore((long)DRCurrentWorks["WorksExpertID"], (long)DRCurrentWorks["WorksID"], (long)DRCurrentWorks["ExpertID"], scores1, txtScorePingYu.Text);
                if (result == 1)
                {
                    ViewState["DsWorks"] = null;
                    ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "message", "alert('提交成功');window.opener ='';window.open('','_self');window.close()", true);
                }
                else if (result == 0)
                {
                    ShowMessage(UpdatePanel1, "提交失败");
                }
            }
        }
        /// <summary>
        /// 保存输入错误的分的ID值
        /// </summary>
        /// <param name="errorID"></param>
        /// <param name="rightID"></param>
        /// <returns></returns>
        private void GetErrorInput(string errorID, string rightID)
        {
            List<string> lsts = (List<string>)ViewState["ErrorIds"];
            if (lsts == null)
                lsts = new List<string>();
            if (errorID != "")
            {
                if (!lsts.Contains(errorID))
                    lsts.Add(errorID);
            }
            else
            {
                if (lsts.Contains(rightID))
                    lsts.Remove(rightID);
            }
            ViewState["ErrorIds"] = lsts;
        }

        //分数更改事件
        void txtBox_TextChanged(object sender, EventArgs e)
        {
            string id = ((TextBox)sender).ClientID;
            string indx = id.Substring(6, id.IndexOf("_") - 6);
            int max = int.Parse(id.Substring(id.IndexOf("_") + 1));
            int def = 0;
            string err = "只能输入0和" + max.ToString() + "之间的分数";
            Panel panel = (Panel)Page.Form.FindControl("div" + indx);
            Image img = (Image)panel.FindControl("img" + indx);
            img.Style.Remove("visibility");
            img.ImageUrl = "images/cuo.png";
            try
            {
                def = int.Parse(((TextBox)sender).Text);
                if (def >= 0 && def <= max)
                {
                    img.ImageUrl = "images/dui.png";
                    GetErrorInput("", indx);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script defer>alert('" + err + "');document.getElementById('" + id + "').select();</script>");
                    GetErrorInput(indx, "");
                    return;
                }
            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script defer>alert('" + err + "');document.getElementById('" + id + "').select();</script>");
                GetErrorInput(indx, "");
                return;
            }

            int total = ViewState["total"] == null ? 0 : int.Parse(ViewState["total"].ToString());
            int oleScore = ViewState[id] == null ? 0 : int.Parse(ViewState[id].ToString());
            lblTotalScore.Text = (total - oleScore + def).ToString();
            ViewState["total"] = lblTotalScore.Text;
            ViewState[id] = ((TextBox)sender).Text;
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "", "<script defer>document.getElementById('" + id + "').focus();</script>");

        }

        #endregion
        #region 方法
        ///// <summary>
        ///// 1为可以操作，0不在时间范围内
        ///// </summary>
        ///// <param name="operate">Score\Public</param>
        ///// <returns></returns>
        //private int JudgeDate()
        //{
        //    DataSet ds = DAL.Periods.GetPeriodsByID(PeriodID);
        //    if (ds.Tables[0].Rows.Count == 0)
        //        return 0;
        //    DateTime dtStart = (DateTime)ds.Tables[0].Rows[0]["StartScore"];
        //    DateTime dtEnd = (DateTime)ds.Tables[0].Rows[0]["EndScore"];
        //    if (DateTime.Today < dtStart)
        //        return 1;
        //    else  if (DateTime.Today >= dtStart && DateTime.Today < dtEnd.AddDays(1))
        //        return 2;
        //    dtStart = (DateTime)ds.Tables[0].Rows[0]["StartPublic"];
        //    dtEnd = (DateTime)ds.Tables[0].Rows[0]["EndPublic"];
        //    if (DateTime.Today >= dtStart && DateTime.Today < dtEnd.AddDays(1))
        //        return 3;
        //    return 0;
        //}
        private int GetTopWorksTypeID (int worksTypeID)
        {
            DataSet dsWorksType = DSWorksType;
            DataRow[] drs=dsWorksType.Tables[0].Select("WorksTypeID="+worksTypeID )  ;
            int parentID=(int)drs[0]["ParentID"];
            while (parentID >0)
            {
                drs = dsWorksType.Tables[0].Select("WorksTypeID="+parentID); 
                if (drs.Length >0)
                    parentID = (int)drs[0]["ParentID"];
            }

            return (int)drs[0]["WorksTypeID"];
        }
        /// <summary>
        /// 初始化控件pingfen
        /// </summary>
        private void InitControl()
        {
            if (IsViewWorks)
                return;
            divContent.Controls.Clear();
            int worksTypeID = 0;
            DataSet ds = DsCurrentWorks;
            if (ds.Tables[0].Rows.Count > 0)
            {

                worksTypeID = (int)ds.Tables[0].Rows[0]["WorksTypeID"];
            }
            else
            {
                btnSubmit.Enabled = false;
                return;
            }
            DataSet dsWorksType = DSWorksType;
            List<string> itmes = new List<string>();
            DataView dv = dsWorksType.Tables[0].DefaultView;
            dv.RowFilter = "WorksTypeID=" + worksTypeID;
            if (dv.Count == 0) return;
            itmes.Add(dv[0]["WorksTypeName"].ToString());
            GetWorksTypeTopLevel(dsWorksType, (int)dv[0]["ParentID"], ref itmes);
            //作品类别、期次ID
            int topWorksTypeID = GetTopWorksTypeID(worksTypeID);
            DataSet dsScoreStandard = DAL.Works.GetScoreStandardByWorksType(topWorksTypeID, PeriodID);
            int i = 1;
            if (dsScoreStandard.Tables[0].Rows.Count == 0)
                dsScoreStandard = DAL.WorksType.GetWorksTypeScoreStandardByTypeID(topWorksTypeID);
            foreach (DataRow dr in dsScoreStandard.Tables[0].Rows)
            {
                FillScoreStandard(dr,i.ToString ());
                i += 1;
            }
        }
        //浏览的分数界面
        private void ViewComments(bool allowEdit)
        {

            btnSubmit.Visible = allowEdit;
            DataSet ds = DsCurrentWorks;

            if (!ds.Tables[0].Rows[0].IsNull("Score"))
                lblTotalScore.Text = ds.Tables[0].Rows[0]["Score"].ToString();
            //if (ds.Tables[0].Columns.Contains("Comments"))
            txtScorePingYu.Text = ds.Tables[0].Rows[0]["Comments"].ToString();
            txtScorePingYu.ReadOnly = !allowEdit;
            ds = DSScoreDetails;
            DataView dv = ds.Tables[0].DefaultView;
            try
            {
                Single totalScore = 0;
                foreach (Control ctr in this.divContent.Controls)
                {
                    if (ctr.GetType() == typeof(Panel))
                    {
                        string id = ctr.ID;
                        string standardID = id.Substring(3);
                        Panel panel = (Panel)ctr;
                        foreach (Control subCtr in panel.Controls)
                        {
                            if (subCtr.GetType() == typeof(TextBox))
                            {
                                TextBox txtBox = (TextBox)subCtr;
                                dv.RowFilter = "StandardID=" + standardID;
                                txtBox.Text = dv[0]["Score"].ToString();
                                totalScore += (Single)dv[0]["Score"];
                                txtBox.ReadOnly = !allowEdit;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
        //获取大类别
        private void GetWorksTypeTopLevel(DataSet dsWorksType, int parentID, ref List<string> itmes)
        {
            DataView dv = dsWorksType.Tables[0].DefaultView;
            dv.RowFilter = "WorksTypeID=" + parentID;
            if (dv.Count > 0)
            {
                itmes.Add(dv[0]["WorksTypeName"].ToString());
                GetWorksTypeTopLevel(dsWorksType, (int)dv[0]["ParentID"], ref itmes);
            }

        }
  //评分标准
        private void FillScoreStandard(DataRow dr,string i)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<div class=\"comments-scores-model\">");
                        //评分
            Panel div = new Panel();
            div.ID = "div" + dr["StandardID"];
            div.CssClass = "comments-scores-modelTitle";
            div.Controls.Add(new LiteralControl(dr["StandardName"] + "（" + dr["Score"] + "分）"));

            TextBox txtBox = new TextBox();
            txtBox.ID = "txtFen" + dr["StandardID"] + "_" + dr["Score"];
            txtBox.Text = "0";
            txtBox.CssClass = "txtcss";
            txtBox.Attributes.Add("onfocus", "onft"+i+"(this)");
            txtBox.Attributes.Add("onblur", "onbt" + i + "(this)");
            txtBox.Attributes.Add("onchange", "checkInput(this)");
            div.Controls.Add(txtBox);
            Image img = new Image();
            img.Style.Add("visibility", "hidden");
            img.ID = "img" + dr["StandardID"];
            img.ImageUrl = "images/dui.png";
            div.Controls.Add(img);
            str.Append("<div class=\"comments-scores-divHidden\" id=\"HiddenDiv" + i + "\">");
            str.AppendLine("<ul>");
            string subStandard = dr["StandardDescription"].ToString ();
            string[] subsValue = subStandard.Split('\n');
            foreach (string strValue in subsValue )
            {
                str.AppendLine("<li>" + strValue + "</li>");
            }
            str.AppendLine("</ul>");
            str.AppendLine("</div>");
            divContent.Controls.Add(div);
            divContent.Controls.Add(new LiteralControl(str.ToString()));
            divContent.Controls.Add(new LiteralControl("</div>"));
        }        
        #endregion
        #region 引用的方法
        public bool ShowMessage(UpdatePanel UpdatePanel1, string msg)
        {
            ScriptManager.RegisterStartupScript(UpdatePanel1, this.GetType(), "updateScript", "alert('" + msg + "')", true);
            return true;
        }
        #endregion
        #endregion
        #region 作品相关的
        #region 属性
        private DataSet GetDSWorksComments
        {
            get
            {
                if (ViewState["dsWorksComments"] == null)
                {

                    DataSet ds = DAL.Works.GetWorksCommentsByWorksID(WorksID);
                    ViewState["dsWorksComments"] = ds;
                }
                return (DataSet)ViewState["dsWorksComments"];
            }
        }
        private DataSet GetDSWorksSubmit
        {
            get
            {
                if (ViewState["dsWorksSubmit"] == null)
                {

                    DataSet ds = DAL.Works.GetWorksSubmitByID(WorksID);
                    ViewState["dsWorksSubmit"] = ds;
                }
                return (DataSet)ViewState["dsWorksSubmit"];
            }
        }
        /// <summary>
        /// get doc
        /// </summary>
        private DataSet GetWorksDoc
        {
            get
            {
                if (ViewState["dsWorksDoc"] == null)
                {
                    DataSet ds = BLL.WorksType.GetWorksFileByTypeID(WorksID, 1);
                    ViewState["dsWorksDoc"] = ds;
                }
                return (DataSet)ViewState["dsWorksDoc"];
            }
        }  
        //获取文档视频
        private DataSet GetWorksDocVideo
        {
            get
            {
                if (ViewState["dsWorks3"] == null)
                {
                    DataSet ds = BLL.WorksType.GetWorksFileByTypeID(WorksID, 3);
                    ViewState["dsWorks3"] = ds;
                }
                return (DataSet)ViewState["dsWorks3"];
            }
        }   
        //获取演示视频
        private DataSet GetWorksViewVideo
        {
            get
            {
                if (ViewState["dsWorks4"] == null)
                {
                    DataSet ds = BLL.WorksType.GetWorksFileByTypeID(WorksID, 4);
                    ViewState["dsWorks4"] = ds;
                }
                return (DataSet)ViewState["dsWorks4"];
            }
        }
        //类型2为图片
        private DataSet GetDSWorksFile
        {
            get
            {
                if (ViewState["dsWorksFile"] == null)
                {
                    DataSet ds = BLL.WorksType.GetWorksFileByTypeID(WorksID, 2);
                    ViewState["dsWorksFile"] = ds;
                }
                return (DataSet)ViewState["dsWorksFile"];
            }
        }
        private long WorksID
        {
            get
            {
                if (ViewState["worksID"] == null)
                {
                    long worksID = 0;
                    if (Request.QueryString["WorksID"]!=null)
                        worksID = long.Parse(Request.QueryString["WorksID"]);
                    else
                        if (IsSample == 1 && PeriodTime == 1 && DsCurrentWorks.Tables[0].Rows.Count > 0)
                            worksID = (long)DsCurrentWorks.Tables[0].Rows[0]["WorksID"];
                    ViewState["worksID"] = worksID;
                }
                return (long)ViewState["worksID"];
            }
        }
        #endregion
        #region 事件
        void btnSubmitShow_Click(object sender, EventArgs e)
        {
            Single score = Single.Parse(HiddenField2.Value);
            DataTable dt = GetDSWorksComments.Tables[0].Clone();
            dt.Columns.Remove("Name");
            dt.AcceptChanges();
            DataRow dr = dt.NewRow();
            dr["WorksID"] = WorksID;
            dr["ExpertID"] = DAL.Common.LoginID;
            dr["Flag"] = 2;
            dr["Score"] = score;
            dr["Comments"] = txtComments.Text;
            dr["Created"] = DateTime.Now;
            try
            {
                long resultID = DAL.Works.InsertWorksComments(dr);
                if (resultID > 0)
                {
                    ViewState["dsWorksComments"] = null;
                    FillComments();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('点评成功');document.location.replace('Comments.aspx?PeriodID=" + PeriodID + "&&WoksID=" + WorksID +")</script>");
                    //btnSubmitShow.Enabled = false;
                    //Session["IsSubmit"] = true;
                    //Response.Redirect("Comments.aspx");
                }
            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('点评失败');</script>");

            }
        }
        #endregion
        #region 方法
        private void InitControlWorksShow()
        {
            if (DsCurrentWorks.Tables[0].Rows.Count ==0) return;
            DataSet dsWorks = GetDSWorksSubmit;
            DataRow DRCurrentWorks = DsCurrentWorks.Tables[0].Rows[0];  
            if (dsWorks.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dsWorks.Tables[0].Rows[0];
                lblWorksName.Text = DRCurrentWorks["WorksName"].ToString();
                lblWorksCode.Text = dr.IsNull("WorksCode")?"": dr["WorksCode"].ToString();
                lblWorksType.Text = DRCurrentWorks["WorksTypeName"].ToString();
                try
                {
                    lblSubmitProfile.Text = dr.IsNull("SubmitProfile") ? "" : dr["SubmitProfile"].ToString();
                    divKeyPoints.Controls.Clear();
                    if (!dr.IsNull("KeyPoints"))
                        divKeyPoints.Controls.Add(new LiteralControl(dr["KeyPoints"].ToString()));
                    lblDemoURL.Text = dr.IsNull("DemoURL") ? "" : dr["DemoURL"].ToString();
                    divDesignIdeas.Controls.Clear();
                    if (!dr.IsNull("DesignIdeas"))
                        divDesignIdeas.Controls.Add(new LiteralControl(dr["DesignIdeas"].ToString()));
                }
                catch
                {
                }
                FillVideoShow();//video
                FillWorksFile();//picture
                //FillDocView();//doc
                FillComments();//dianping
            }
        }
        private void FillDocView()
        {
            string txtDocView = "<iframe src='"+DAL.Common.SPWeb.Url + "/_layouts/15/WopiFrame.aspx?sourcedoc=qqqqqq1111&action=embedview'  width='600px' height='470px'></iframe>";

            divDocView.Controls.Clear();
            DataSet ds = GetWorksDoc;
            string txtUrl;
            string subWebUrl = DAL.Common.SPWeb.Url;
            subWebUrl = subWebUrl.Substring(subWebUrl.IndexOf("/", 8));
            LinkButton btn = new LinkButton();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                txtUrl =   (string)dr["FilePath"] ;
                string docUrl=subWebUrl+txtUrl;
                txtDocView = txtDocView.Replace("qqqqqq1111",docUrl  );//加上子网站的链接
                divDocView.Controls.Add(new LiteralControl(txtDocView));
                btn = new LinkButton();
                btn.ID = "btn" + dr["WorksFileID"];
                btn.CommandArgument = docUrl;
                btn.Click += btn_Click;
                btn.Text = "下载到本地";
                divDocView.Controls.Add(btn ); 
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string fileName = btn.CommandArgument;
            string docName = fileName.Substring(fileName.LastIndexOf("/") + 1);
            string saveFileName = DealWorkfileName(docName);
            DownloadFile(fileName, saveFileName);
        }
        public void DownloadFile(string url,string fileName)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb web = site.OpenWeb(DAL.Common.SPWeb.ID ))
                        {
                            //SPList lst=web.Lists["workFile"];
                            //SPDocumentLibrary lstDoc = (SPDocumentLibrary)lst;
                            //SPFile file= lstDoc.RootFolder.Files[url];
                            SPFile file = web.GetFile(url);
                            DAL.Common.DownLoadFileByStream(file, Response,fileName );
                            //Response.Write(file.Name );
                            //SPFolder folder = web.Folders[file.ParentFolder.Name];
                            //file = folder.Files[file.Name];

                            //var context = WebOperationContext.Current.OutgoingResponse;
                            //context.ContentType = "application/octet-stream";
                            //context.Headers.Add("Content-Disposition", "attachment; filename=" + file.Name  );
                            //Response.Write("header");
                           
                        }
                    }
                });
            }
            catch (Exception ex)
            {}
        }
        private void FillVideoShow()
        {
            DataSet ds = GetWorksDocVideo;//3
            string txtVideo1 = "";
            int i = 1;
            divDovVideo.Controls.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                txtVideo1 = FillVideoControl("文档视频："+(string)dr["FileName"], (string)dr["FilePath"], i.ToString(),"true");
                divDovVideo.Controls.Add(new LiteralControl(txtVideo1));
                i += 1;
            }
            ds = GetWorksViewVideo;//4
            divViewShow.Controls.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                txtVideo1 = FillVideoControl("讲解视频：" + (string)dr["FileName"], (string)dr["FilePath"], i.ToString(),"false");
                divViewShow.Controls.Add(new LiteralControl(txtVideo1));
                i += 1;
            }
        }
        private string FillVideoControl(string mediaTitle, string mediaUrl,string i,string autoPlay)
        {
            StringBuilder txtContent = new StringBuilder();
            txtContent.AppendLine("<div id=\"MediaPlayerHost"+i+"\">");
            txtContent.AppendLine("<script type=\"text/javascript\" >");
            txtContent.AppendLine("{");
            txtContent.AppendLine("mediaPlayer.createMediaPlayer(");
            txtContent.AppendLine("document.getElementById('MediaPlayerHost"+i +"'),");
            txtContent.AppendLine("'MediaPlayerHost',");
            txtContent.AppendLine("'600px', '470px',");
            txtContent.AppendLine("{");
            txtContent.AppendLine("displayMode: 'Inline',");
            txtContent.AppendLine("mediaTitle: '" + DealWorkfileName( mediaTitle) + "',");
            txtContent.AppendLine("mediaSource: '" +DAL.Common.SPWeb.Url + mediaUrl + "',");
            string txtExtend = mediaTitle.Substring(mediaTitle.LastIndexOf(".") + 1);
            string previewFile = DAL.Common.SPWeb.Url + "/_layouts/15/WorkEvaluate/images/";
            if (txtExtend.ToLower() == "mp3")
                previewFile += "yinpin.jpg";
            else
                previewFile += "shipin.jpg";
            txtContent.AppendLine("previewImageSource: '"+previewFile+"',");
            txtContent.AppendLine("autoPlay: "+autoPlay+",");
            txtContent.AppendLine("loop: false,");
            txtContent.AppendLine("mediaFileExtensions: 'wmv;wma;avi;mpg;mp3;',");
            txtContent.AppendLine(" silverlightMediaExtensions: 'wmv;wma;mp3;'");
            txtContent.AppendLine("});");
            txtContent.AppendLine("}");
            txtContent.AppendLine("</script>");
            txtContent.AppendLine("</div>");
            return txtContent.ToString();
        }
        private string DealWorkfileName(string fileName)
        {
            string txtFile= fileName.Substring(0,fileName.IndexOf("-"))+fileName.Substring(fileName.LastIndexOf("-")+1 );
            return txtFile;
        }
        //作品展示图
        private void FillWorksFile()
        {
            DataSet ds = GetDSWorksFile;
            StringBuilder txtContent = new StringBuilder();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                txtContent.AppendLine("<img src='"+DAL.Common.SPWeb.Url + dr["FilePath"].ToString() + "' width='500px'/><br />" +DealWorkfileName( dr["FileName"].ToString()));
                txtContent.AppendLine("<br/>");
            }
            if (txtContent.ToString().Length > 0)
                txtContent.Remove(txtContent.Length - 5, 5);
            divWorksFile.Controls.Clear();
            divWorksFile.Controls.Add(new LiteralControl(txtContent.ToString()));
        }
        //师生点评
        private void FillComments()
        {
            divScore.Controls.Clear();
            DataSet ds = GetDSWorksComments;
            DataView dv = ds.Tables[0].DefaultView;
            dv.RowFilter = "ExpertID=" + DAL.Common.LoginID ;
            if (dv.Count > 0)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>YiComments();</script>");

            dv.RowFilter = "";
            lblPersons.Text = ds.Tables[0].Rows.Count.ToString();
            gvComments.DataSource = dv;
            gvComments.DataBind();
            double avg = 0;
            string sScore = ds.Tables[0].Compute("AVG(Score)", "").ToString();
            if (!string.IsNullOrEmpty(sScore))
                avg = double.Parse(sScore);
            int score = (int)avg;
            string txt = "";
            for (int i = 0; i < score; i++)
            {
                txt += "<img src='images/star_red.gif'/>";
            }
            int j = score;
            if (avg > score)
            {
                j = j + 1;
                txt += "<img src='images/star_red2.gif'/>";

            }
            for (int i = j; i < 5; i++)
            {
                txt += "<img src='images/star_gray.gif'/>";

            }
            divScore.Controls.Add(new LiteralControl(txt));
        }
        #endregion
        #endregion
 
    }
}
