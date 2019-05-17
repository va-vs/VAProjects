using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Data;
using System.Text;



using System.Collections;


namespace WorkEvaluate.Layouts.WorkEvaluate
{
    public partial class OnlineEnroll : LayoutsPageBase
    {
        #region 变量
        private string worksID; //作品Id
        private DataTable worksFileDt;//作品附件
        private DataTable imageFileDt;
        private DataTable worksDt; //作品
        private SPWeb web; //当前网站
        private SPSite site;
        protected Button btnDel; //删除按钮
        protected Button btnDown; //下载按钮
        //
        private DataTable WorksTypeDt; //作品类别
        private DataTable userWorksDt;//用户作品关系
        public DataTable UserWorksDt
        {
            get
            {
                if (userWorksDt == null)
                {
                    userWorksDt = new DataTable();
                    userWorksDt.Columns.Add("UserWorksID", typeof(long));
                    userWorksDt.Columns.Add("WorksID", typeof(long));
                    userWorksDt.Columns.Add("UserID", typeof(long));
                    userWorksDt.Columns.Add("Relationship", typeof(int));
                    userWorksDt.Columns.Add("Flag", typeof(int));
                }

                return userWorksDt;
            }
        }
        #endregion
        #region 属性
        /// <summary>
        /// 当前网站
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
        public SPSite mySite
        {

            get
            {
                if (site == null)
                {

                    site = SPContext.Current.Site;
                }
                return site;

            }

        }
        /// <summary>
        /// 作品
        /// </summary>
        public DataTable GetWorksDt
        {
            get
            {
                if (worksDt == null)
                {
                    worksDt = new DataTable();

                    worksDt.Columns.Add("PeriodID", typeof(long));
                    worksDt.Columns.Add("WorksID", typeof(long));
                    worksDt.Columns.Add("WorksTypeID", typeof(string));

                    worksDt.Columns.Add("WorksCode", typeof(string));
                    worksDt.Columns.Add("WorksName", typeof(string));

                    worksDt.Columns.Add("IsSample", typeof(int));
                    worksDt.Columns.Add("SubmitProfile", typeof(string));
                    worksDt.Columns.Add("DesignIdeas", typeof(string));
                    worksDt.Columns.Add("KeyPoints", typeof(string));
                    worksDt.Columns.Add("DemoURL", typeof(string));

                    worksDt.Columns.Add("Author", typeof(long));
                    worksDt.Columns.Add("Members", typeof(long));

                    worksDt.Columns.Add("Flag", typeof(long));
                    worksDt.Columns.Add("WorksState", typeof(int));
                    //
                    worksDt.Columns.Add("Score", typeof(float));
                    //创建时间和人
                    worksDt.Columns.Add("CreatedBy", typeof(long));
                    worksDt.Columns.Add("Created", typeof(DateTime));

                    //worksDt.Columns.Add("CreatedBy", typeof(float));
                    //worksDt.Columns.Add("Created", typeof(float));



                }
                return worksDt;
            }
        }
        /// <summary>
        /// 附件
        /// </summary>
        public DataTable GetWorksFileDt
        {
            get
            {
                if (worksFileDt == null)
                {
                    worksFileDt = new DataTable();
                    worksFileDt.Columns.Add("WorksFileID", typeof(long));
                    worksFileDt.Columns.Add("Type", typeof(int));
                    worksFileDt.Columns.Add("WorksID", typeof(long));
                    worksFileDt.Columns.Add("FileName", typeof(string));
                    worksFileDt.Columns.Add("FilePath", typeof(string));
                    worksFileDt.Columns.Add("FileSize", typeof(int));
                    worksFileDt.Columns.Add("Flag", typeof(long));

                }
                return worksFileDt;
            }
        }
        /// <summary>
        /// 删除作品
        /// </summary>
        private DataTable delWorksDt;
        public DataTable GetDelImageDt
        {
            get
            {
                if (delWorksDt == null)
                {
                    delWorksDt = new DataTable();
                    delWorksDt.Columns.Add("WorksFileID", typeof(long));
                    delWorksDt.Columns.Add("Type", typeof(int));
                    delWorksDt.Columns.Add("FileName", typeof(string));
                    delWorksDt.Columns.Add("FilePath", typeof(string));
                    delWorksDt.Columns.Add("ModifiedBy", typeof(long));
                    delWorksDt.Columns.Add("Modified", typeof(DateTime));
                    delWorksDt.Columns.Add("FileSize", typeof(int));
                    delWorksDt.Columns.Add("Flag", typeof(long));
                }
                return delWorksDt;
            }
        }
        /// <summary>
        /// 效果图
        /// </summary>
        public DataTable GetImageFileDt
        {
            get
            {
                if (imageFileDt == null)
                {
                    imageFileDt = new DataTable();
                    imageFileDt.Columns.Add("WorksFileID", typeof(long));
                    imageFileDt.Columns.Add("Type", typeof(int));
                    imageFileDt.Columns.Add("WorksID", typeof(long));
                    imageFileDt.Columns.Add("FileName", typeof(string));
                    imageFileDt.Columns.Add("FilePath", typeof(string));
                    imageFileDt.Columns.Add("FileSize", typeof(int));
                    imageFileDt.Columns.Add("Flag", typeof(long));

                }
                return imageFileDt;
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
            btnSave.Click += new EventHandler(btnSave_Click);
            btnUpFile.Click += btnUpFile_Click;
            gvWorks.RowCommand += gvWorks_RowCommand;
            //
            btnImageUpload.Click += btnImageUpload_Click;
            gvImage.RowCommand += gvImage_RowCommand;
            //
            ddlOneWorksType.SelectedIndexChanged += ddlOneWorksType_SelectedIndexChanged;
            ddlPeriod.SelectedIndexChanged += ddlPeriod_SelectedIndexChanged;
            btnSubmit.Click += btnSubmit_Click;
            //btnIsSample.Click += btnIsSample_Click;



        }
        //void btnIsSample_Click(object sender, EventArgs e)
        //{

        //    BLL.User.GetUserID(SPContext.Current.Web.CurrentUser);

        //    DAL.Common.OpenWindow(this.Page, myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?PeriodID=" + ddlPeriod.SelectedValue + "&&IsSample=1");
        //    //  Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?PeriodID=" + ddlPeriod.SelectedValue + "&&IsSample=1");

        //    //Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?IsSample=1&&PeriodID=36");
        //}
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                long courseID = BLL.Course.GetCourseID();
                ViewState["CourseID"] = courseID;
                WorksTypeDt = DAL.Works.GetWorksType().Tables[0];
                ViewState["WorksType"] = WorksTypeDt;

                //BindOneWorksType(WorksTypeDt);
                //BindTwoWorksType(WorksTypeDt, ddlOneWorksType.SelectedValue);
                //加载期次
                LoadPeriods();
                //加载页面信息
                LoadPeriodPage();
                JudgeIsSampleButton();
            }
        }
        /// <summary>
        /// 期次更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddlPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPeriodPage();
        }
        /// <summary>
        /// 效果图命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gvImage_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent));
                DataTable dt = (DataTable)ViewState["imageFile"];
                //获取删除dataview数据
                if (long.Parse(drv.Cells[0].Text) != 0)
                {
                    //if不是刚传的，删除数据库，更新viewstate
                    DataRow delDr = GetDelImageDt.NewRow();
                    delDr["WorksFileID"] = long.Parse(drv.Cells[0].Text);
                    delDr["Type"] = int.Parse(drv.Cells[4].Text);
                    delDr["FileName"] = drv.Cells[1].Text;

                    delDr["FilePath"] = drv.Cells[2].Text;
                    delDr["ModifiedBy"] = BLL.User.GetUserID(SPContext.Current.Web.CurrentUser);
                    delDr["Modified"] = DateTime.Now;
                    if (drv.Cells[3].Text != "")
                    {
                        delDr["FileSize"] = int.Parse(drv.Cells[3].Text);
                    }
                    else
                    {
                        delDr["FileSize"] = 0;
                    }
                    delDr["Flag"] = 0;

                    if (ViewState["delImageDt"] == null)
                    {
                        ViewState["delImageDt"] = GetDelImageDt;

                    }
                    //  DataRow delDr = ((DataTable)ViewState["delImageDt"]).NewRow();
                    ((DataTable)ViewState["delImageDt"]).Rows.Add(delDr.ItemArray);
                }
                //删除当前viewstate
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j]["FileName"].ToString() == drv.Cells[1].Text.Replace("amp;", ""))
                    {
                        //if (long.Parse(drv.Cells[0].Text) == 0)
                        //{
                        //    DelList(dt.Rows[j]["FileName"].ToString(), dt.Rows[j]["Type"].ToString());
                        //}
                        dt.Rows[j].Delete();
                        dt.AcceptChanges();
                    }
                }
                ViewState["imageFile"] = dt;
                BindGridView((DataTable)ViewState["imageFile"], gvImage);
                //btnSubmit.Focus();

                if (ddlPeriod.SelectedValue != "")
                {
                    LoadUI(long.Parse(ddlPeriod.SelectedValue));
                    JudgeScore();
                }
            }
            if (e.CommandName == "Down")
            {
                GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent));
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    DAL.Common.DownLoadFileByStream(myWeb.Url + "/" + drv.Cells[2].Text, drv.Cells[1].Text, Response);
                });
                //btnSubmit.Focus();
            }

        }
        /// <summary>
        /// 行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gvWorks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent));
                DataTable dt = (DataTable)ViewState["workFile"];

                //获取删除dataview数据
                if (long.Parse(drv.Cells[0].Text) != 0)
                {
                    //if不是刚传的，删除数据库，更新viewstate

                    DataRow delDr = GetDelImageDt.NewRow();

                    delDr["WorksFileID"] = long.Parse(drv.Cells[0].Text);
                    delDr["Type"] = int.Parse(drv.Cells[4].Text);
                    delDr["FileName"] = drv.Cells[1].Text;

                    delDr["FilePath"] = drv.Cells[2].Text;
                    delDr["ModifiedBy"] = BLL.User.GetUserID(SPContext.Current.Web.CurrentUser);
                    delDr["Modified"] = DateTime.Now;
                    if (drv.Cells[3].Text != "")
                    {
                        delDr["FileSize"] = int.Parse(drv.Cells[3].Text);
                    }
                    else
                    {
                        delDr["FileSize"] = 0;
                    }
                    delDr["Flag"] = 0;


                    if (ViewState["delWorksDt"] == null)
                    {
                        ViewState["delWorksDt"] = GetDelImageDt;

                    }
                    // DataRow delDr = ((DataTable)ViewState["delWorksDt"]).NewRow();
                    ((DataTable)ViewState["delWorksDt"]).Rows.Add(delDr.ItemArray);
                }
                //删除当前viewstate
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Rows[j]["FileName"].ToString() == drv.Cells[1].Text.Replace("amp;", ""))
                    {
                        //if (long.Parse(drv.Cells[0].Text) == 0)
                        //{
                        ////    DelList(dt.Rows[j]["FileName"].ToString(), dt.Rows[j]["Type"].ToString());
                        //}
                        dt.Rows[j].Delete();
                        dt.AcceptChanges();
                    }
                }

                ViewState["workFile"] = dt;
                //gvWork
                BindGridView((DataTable)ViewState["workFile"], gvWorks);


                //btnSubmit.Focus();
                if (ddlPeriod.SelectedValue != "")
                {
                    LoadUI(long.Parse(ddlPeriod.SelectedValue));
                    JudgeScore();
                }
            }
            if (e.CommandName == "Down")
            {
                GridViewRow drv = ((GridViewRow)(((Button)(e.CommandSource)).Parent.Parent));
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    DAL.Common.DownLoadFileByStream(myWeb.Url + "/" + drv.Cells[2].Text, drv.Cells[1].Text, Response);
                });
                //btnSubmit.Focus();
            }
        }
        /// <summary>
        /// 组别改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ddlOneWorksType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "")
            {
                LoadUI(long.Parse(ddlPeriod.SelectedValue));
                JudgeScore();
            }
            ////if (JudgeWordCount())
            ////{
            ////    return;
            ////}
           
            DataRow dr = GetWorksDt.NewRow();
            //dr["WorksState"] = 0;
            Save(dr, 0);
            //  GoToPage();
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ddlPeriod.SelectedValue != "")
            {
                LoadUI(long.Parse(ddlPeriod.SelectedValue));
                JudgeScore();
            }
            if (JudgeWordCount())
            {
                return;
            }
            //2014-11-12
            //提交时判断有没有上传文档，没有不让提交

            if (((DataTable)ViewState["workFile"]).Rows.Count == 0 || ((DataTable)ViewState["imageFile"]).Rows.Count == 0)
            {
                error.Text = "请注意，提交作品时请确保已上传作品文件和讲解视频";
            }
            else
            {

                DataRow dr = GetWorksDt.NewRow();
                //dr["WorksState"] = 1;
                Save(dr, 1);
                // DAL.Common.ShowMessage(this.Page, this.GetType(), "提交成功！");
                // GoToPage();

                SetControls(false);
            }
        }
        /// 视频讲解上传控件
        /// </summary>                                  
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        void btnUpFile_Click(object sender, EventArgs e)
        {
            bool allowUpload = false;
            string[] extensionType = { ".mp4", ".mp3" };
            try
            {
                if (fileUpload.HasFile == false)
                {
                    DAL.Common.ShowMessage(this.Page, this.GetType(), "请选择文件");
                }
                else
                {
                    string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                    //判断文件类型是否符合要求
                    foreach (string var in extensionType)
                    {
                        if (fileExtension == var)
                        {
                            allowUpload = true;
                            break;
                        }
                    }
                    if (allowUpload)
                    {
                        //通过后缀判断存放在哪个文档库
                        if (fileExtension == ".mp4" || fileExtension == ".mp3")
                        {
                            //  //文档1  图片2  文档视频3  讲解视频4 
                            this.AddWorkFile(myWeb, "workVideo", fileUpload, "workFile", gvWorks, 4);

                        }
                        //else
                        //{
                        //    this.AddWorkFile(myWeb, "workFile", fileUpload, "workFile", gvWorks, 2);
                        //}
                    }
                    else
                    {
                        DAL.Common.ShowMessage(this.Page, this.GetType(), "只能上传指定格式的文件！");
                    }
                }
            }
            catch
            {
            }
            //
            if (ddlPeriod.SelectedValue != "")
            {
                LoadUI(long.Parse(ddlPeriod.SelectedValue));
                JudgeScore();
            }
        }
        private void JudgeIsSampleButton()
        {
            if (ddlPeriod.SelectedValue != "")
            {
                HyperLink1.Visible = true;
                //btnIsSample.Visible = true;
            }
            else
            {
                HyperLink1.Visible = false;
                //  btnIsSample.Visible = false;
            }
        }
        /// <summary>
        /// 效果图上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnImageUpload_Click(object sender, EventArgs e)
        {
            //文档1  图片2  文档视频3  讲解视频4 
            bool allowUpload = false;
            string[] extensionType = { ".docx", ".doc", ".pdf", ".ppt", ".pptx", ".xls", ".xlsx", ".mp4", ".accdb", ".png", ".jpg", ".gif", ".bmp", ".mp3" };
            try
            {
                if (imageFileUpload.HasFile == false)
                {
                    DAL.Common.ShowMessage(this.Page, this.GetType(), "请选择文件");
                }
                else
                {
                    string fileExtension = Path.GetExtension(imageFileUpload.FileName).ToLower();
                    //判断文件类型是否符合要求
                    foreach (string var in extensionType)
                    {
                        if (fileExtension == var)
                        {
                            allowUpload = true;
                            break;
                        }
                    }
                    if (allowUpload)
                    {
                        //文档1  图片2  文档视频3  讲解视频4   
                        //通过后缀判断存放在哪个文档库
                        if (fileExtension == ".mp4" || fileExtension == ".mp3")
                        {
                            this.AddWorkFile(myWeb, "workVideo", imageFileUpload, "imageFile", gvImage, 3);

                        }
                        //".png", ".jpg", ".gif", ".bmp",
                        else if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".gif" || fileExtension == ".bmp")
                        {
                            this.AddWorkFile(myWeb, "workPic", imageFileUpload, "imageFile", gvImage, 2);
                        }
                        else
                        {
                            this.AddWorkFile(myWeb, "workFile", imageFileUpload, "imageFile", gvImage, 1);
                        }

                    }
                    else
                    {
                        DAL.Common.ShowMessage(this.Page, this.GetType(), "只能上传指定格式的文件！");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            if (ddlPeriod.SelectedValue != "")
            {
                LoadUI(long.Parse(ddlPeriod.SelectedValue));
                JudgeScore();
            }
        }
        #endregion
        #region 方法
        private void Save(DataRow dr, int WorksState)
        {
            //

            //重新判断
            //  ViewState["WoksID"] = BLL.Works.GetWorksIDByPriod(long.Parse(ddlPeriod.SelectedValue)).ToString();
            long workID = BLL.Works.GetWorksIDByPriod(long.Parse(ddlPeriod.SelectedValue));
            //if (ViewState["WoksID"] == null)
            //{
            //    ViewState["WoksID"] = 0;
            //}   
            //判断是否提交


            DataTable dt = DAL.Works.GetWorksByWorksID(workID).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //判断
                if (!JudgeSubmit(dt.Rows[0]))
                {
                    SetControls(false);
                    error.Text = "请注意，该作品已提交不可更改！";
                    return;
                }
            }
            //



            dr["PeriodID"] = long.Parse(ddlPeriod.SelectedValue);
            //
            dr["WorksID"] = workID;
            dr["WorksName"] = txtWorksName.Text;
            if (Request.QueryString["IsSample"] != null)
            {
                //再次判断当前用户登陆角色

                if (BLL.User.JudgeUserRight())
                {
                    dr["IsSample"] = 0;
                }
                else
                {
                    dr["IsSample"] = 1;
                }

                if (txtScore.Text != "")
                {
                    dr["Score"] = txtScore.Text;
                }
            }
            else
            {
                dr["IsSample"] = 0;

            }
            //dr["WorksCode"] = GetWorksCode();
            if (ddlTwoWorksType.SelectedValue == "0")
            {
                dr["WorksTypeID"] = int.Parse(ddlOneWorksType.SelectedValue);
            }
            else
            {
                dr["WorksTypeID"] = int.Parse(ddlTwoWorksType.SelectedValue);
            }
            // dr["ContestID"]=ContestID;

            dr["SubmitProfile"] = txtSubmitProfile.Text;
            dr["DesignIdeas"] = txtDesignIdeas.Text;
            dr["KeyPoints"] = txtKeyPoints.Text;
            dr["DemoURL"] = txtDemo1URL.Text;
            dr["Flag"] = 1;

            dr["WorksState"] = WorksState;



            //时间和人
            dr["CreatedBy"] = BLL.User.GetUserID(SPContext.Current.Web.CurrentUser);
            dr["Created"] = DateTime.Now;


            //插入work
            if (workID != 0)
            {
                // workID = long.Parse(ViewState["WorksID"].ToString());
                DAL.Works.UpdateWorksInfo(dr);

            }
            else
            {
                workID = DAL.Works.InsertWorks(dr);
                // 
                dr["WorksID"] = workID;
            }
            dr["WorksCode"] = GetWorksCode(workID);
            DAL.Works.UpdateWorksCode(dr);
            //本次作品的人员
            DataTable UserWorkDt = DAL.User.GetUserByWorksID(workID).Tables[0];
            //插入User
            ArrayList list = user.ResolvedEntities;
            long UserID;
            DataRow userWorkDr = UserWorksDt.NewRow();
            userWorkDr["WorksID"] = workID;
            //
            if (GetUserCount() == 0)
            {
                userWorkDr["Relationship"] = 0;
            }
            else
            {
                userWorkDr["Relationship"] = 1;
            }
            //添加登陆用户
            SPUser member = SPContext.Current.Web.CurrentUser;
            UserID = BLL.User.GetUserID(member);
            userWorkDr["UserID"] = UserID;
            userWorkDr["Flag"] = 1;
            DataRow[] drs = UserWorkDt.Select("UserID=" + UserID.ToString());
            if (drs.Length == 0)
                DAL.User.InsertUserWorks(userWorkDr);
            else
                DAL.User.UpdateUserWorks(userWorkDr);
            //hxhxhxhxhxhxhxxhhx
            //  string sql = "UserID!=" + UserID.ToString();
            //处理队员
            string sql = "UserID<>" + UserID.ToString();
            //获取id和显示名称
            foreach (Microsoft.SharePoint.WebControls.PickerEntity p in list)
            {
                //排除当前用户


                userWorkDr = UserWorksDt.NewRow();
                member = myWeb.EnsureUser(p.Key);
                if (member.LoginName != SPContext.Current.Web.CurrentUser.LoginName)
                {
                    //roleID 1教师 2学生
                    UserID = BLL.User.GetUserID(member);

                    userWorkDr["Relationship"] = 2;
                    userWorkDr["UserID"] = UserID;
                    userWorkDr["Flag"] = 1;
                    userWorkDr["WorksID"] = workID;
                    //xqx
                    sql += " and UserID<>" + UserID.ToString();
                    drs = UserWorkDt.Select("UserID=" + UserID.ToString());
                    if (drs.Length == 0)
                    {

                        DAL.User.InsertUserWorks(userWorkDr);
                    }

                    else
                        DAL.User.UpdateUserWorks(userWorkDr);
                }
            }
            //删除
            drs = UserWorkDt.Select(sql);
            foreach (DataRow dr1 in drs)
            {
                userWorkDr["UserID"] = dr1["UserID"];
                userWorkDr["Flag"] = 0;
                DAL.User.UpdateUserWorks(userWorkDr);
            }

            //插入效果图
            if (ViewState["imageFile"] == null)
            {
                ViewState["imageFile"] = GetImageFileDt;
            }
            foreach (DataRow drr in ((DataTable)ViewState["imageFile"]).Rows)
            {
                if (drr["WorksFileID"].ToString() == "0")
                {
                    drr["WorksID"] = workID;
                    DAL.Works.InsertWorksImages(drr);

                }
            }

            //插入视频讲解
            if (ViewState["workFile"] == null)
            {
                ViewState["workFile"] = GetImageFileDt;
            }
            foreach (DataRow drr in ((DataTable)ViewState["workFile"]).Rows)
            {
                if (drr["WorksFileID"].ToString() == "0")
                {
                    drr["WorksID"] = workID;
                    DAL.Works.InsertWorksImages(drr);
                }
            }

            //提交已删除的数据           
            if (ViewState["delWorksDt"] != null)
            {
                foreach (DataRow ddr in ((DataTable)ViewState["delWorksDt"]).Rows)
                {
                    DAL.Works.UpdateWorksFile(ddr);
                    // DelList(ddr["FileName"].ToString(), ddr["Type"].ToString());
                }
            }
            //提交已删除的图片数据
            if (ViewState["delImageDt"] != null)
            {
                foreach (DataRow idr in ((DataTable)ViewState["delImageDt"]).Rows)
                {
                    DAL.Works.UpdateWorksFile(idr);
                    // DelList(idr["FileName"].ToString(), idr["Type"].ToString());
                }
            }
            GoToPage(workID);

        }
        /// <summary>
        /// 绑定组别第一级
        /// </summary>
        /// <param name="dt"></param>
        public void BindOneWorksType(DataTable dt)
        {
            ddlOneWorksType.Items.Clear();
            //ddlOneWorksType.Items.Add(new ListItem("全部", "0"));
            DataRow[] drs = dt.Select("LevelID=0");
            foreach (DataRow dr in drs)
            {
                ddlOneWorksType.Items.Add(new ListItem(dr["WorksTypeName"].ToString(), dr["WorksTypeID"].ToString()));
            }
        }
        /// <summary>
        /// 绑定组别第二级
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="WorksTypeID"></param>
        public void BindTwoWorksType(DataTable dt, string WorksTypeID)
        {
            ddlTwoWorksType.Items.Clear();
            ddlTwoWorksType.Items.Add(new ListItem("全部", "0"));
            DataRow[] drs = dt.Select("LevelID=1");
            foreach (DataRow dr in drs)
            {
                if (WorksTypeID == "0")
                {
                    ddlTwoWorksType.Items.Add(new ListItem(dr["WorksTypeName"].ToString(), dr["WorksTypeID"].ToString()));
                }
                else
                {
                    if (dr["ParentID"].ToString() == WorksTypeID)
                    {
                        ddlTwoWorksType.Items.Add(new ListItem(dr["WorksTypeName"].ToString(), dr["WorksTypeID"].ToString()));
                    }
                }
            }
        }
        /// <summary>
        /// 上传附件
        /// </summary>
        public void AddWorkFile(SPWeb web, string docLibName, FileUpload fUpload, String ViewStateName, GridView gv, int type)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite ElevatedsiteColl = new SPSite(mySite.ID))
                    {
                        using (SPWeb ElevatedSite = ElevatedsiteColl.OpenWeb(myWeb.ID))
                        {
                            ElevatedSite.AllowUnsafeUpdates = true;
                            SPList list = ElevatedSite.Lists.TryGetList(docLibName);
                            //如果没有文档库，生成文档库
                            if (list == null)
                            {

                                list = BLL.Works.CreateList(docLibName);
                            }
                            SPDocumentLibrary docLib = (SPDocumentLibrary)list;
                            if (fUpload.HasFile)
                            {
                                //数据库
                                if (ViewState[ViewStateName] == null)
                                {
                                    ViewState[ViewStateName] = GetWorksFileDt;
                                }
                                //判断是否存在 
                                if (JudgeSize(fUpload.PostedFile.ContentLength))
                                {
                                    error.Text = "超出上传文档大小限制！";
                                    // DAL.Common.ShowMessage(this.Page, this.GetType(), "超出上传文档大小限制！");
                                }
                                else
                                {
                                    string fn = type.ToString().PadLeft(2, '0') + DateTime.Now.Year + ddlPeriod.SelectedValue.PadLeft(4, '0') + "-" + DAL.Common.GetLoginAccount + "-" + Path.GetFileName(fUpload.PostedFile.FileName);
                                    Stream stm = fUpload.PostedFile.InputStream;
                                    int iLength = (int)stm.Length;
                                    if (iLength > 0)
                                    {
                                        SPFolder rootFolder = docLib.RootFolder;
                                        Byte[] filecontent = new byte[iLength];
                                        stm.Read(filecontent, 0, iLength);
                                        try
                                        {
                                            SPFile f;
                                            //如果在文档库中有,删除文档库中同名文档后，上传文档
                                            bool isDocHave = false;
                                            bool isDBHave = false;
                                            isDocHave = JudgeIsExistInDoc(docLib, fn, type);
                                            isDBHave = JudgeIsExist(fn, ViewStateName, iLength);
                                            //判断文档库
                                            if (isDocHave)
                                            {
                                                error.Text = "请注意，该文件已存在，上传后自动覆盖已有文件！";

                                                DelList(fn, type.ToString());
                                                f = rootFolder.Files.Add(fn, filecontent, true);

                                            }
                                            else
                                            {
                                                f = rootFolder.Files.Add(fn, filecontent, true);
                                            }
                                            //判断数据库
                                            if (isDBHave)
                                            {
                                                //如果数据库中有，更新数据库
                                            }
                                            else
                                            {
                                                //如果没有插入数据库
                                                error.Text = "";
                                                //数据库
                                                DataTable dt = (DataTable)ViewState[ViewStateName];
                                                DataRow dr = dt.NewRow();
                                                dr["WorksFileID"] = 0;
                                                dr["WorksID"] = 0;
                                                dr["Type"] = type;
                                                dr["FileName"] = fn;
                                                //dr["FilePath"] = web.ServerRelativeUrl + "/" + docLibName + "/" + fn;
                                                dr["FilePath"] = "/" + docLibName + "/" + fn;
                                                dr["FileSize"] = iLength;
                                                dr["Flag"] = 1;

                                                ((DataTable)ViewState[ViewStateName]).Rows.Add(dr.ItemArray);
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        finally
                                        {
                                            stm.Close();
                                        }

                                    }
                                }
                            }
                            ElevatedSite.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                error.Text = "出错了，请报告给管理员！";
                // DAL.Common.ShowMessage(this.Page, this.GetType(), "出错了，请报告给管理员！");
            }
            finally
            {

            }
            BindGridView((DataTable)ViewState[ViewStateName], gv);
        }
        /// <summary>
        /// 判断当前作品是否存在
        /// </summary>
        /// <param name="drr"></param>
        /// <param name="viewstateName"></param>
        /// <returns></returns>
        public bool JudgeIsExist(string fileName, string viewstateName, int length)
        {

            bool isExist = false;
            DataTable dt = (DataTable)ViewState[viewstateName];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["FileName"].ToString() == fileName)
                {
                    isExist = true;
                    //更新
                    DataRow delDr = GetDelImageDt.NewRow();
                    if (dt.Rows[i]["WorksFileID"].ToString() != "0")
                    {
                        delDr["WorksFileID"] = dt.Rows[i]["WorksFileID"];
                        delDr["Type"] = dt.Rows[i]["Type"];
                        delDr["FilePath"] = dt.Rows[i]["FilePath"];
                        delDr["ModifiedBy"] = BLL.User.GetUserID(SPContext.Current.Web.CurrentUser);
                        delDr["Modified"] = DateTime.Now;
                        delDr["FileSize"] = length;
                        delDr["Flag"] = 1;
                        DAL.Works.UpdateWorksFileForSize(delDr);
                    }
                }
            }
            return isExist;
        }

        public bool JudgeIsExistInDoc(SPList docLib, string fileName, int type)
        {
            //得判断资产库

            bool isExist = false;
            if (type == 3 || type == 4)
            {

                for (int i = 0; i < docLib.RootFolder.SubFolders.Count; i++)
                {
                    if (docLib.RootFolder.SubFolders[i].Files[0].Name == fileName)
                    {
                        isExist = true;
                        break;

                    }
                }
                //不开启资产库
                for (int i = 0; i < docLib.RootFolder.Files.Count; i++)
                {
                    if (docLib.RootFolder.Files[i].Name == fileName)
                    {
                        isExist = true;
                        break;
                    }
                }

            }
            else
            {
                for (int i = 0; i < docLib.RootFolder.Files.Count; i++)
                {
                    if (docLib.RootFolder.Files[i].Name == fileName)
                    {
                        isExist = true;
                        break;
                    }
                }
            }
            return isExist;
        }
        /// <summary>
        /// 判断是否超出大小
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool JudgeSize(long i)
        {
            bool isSize = false;
            //更改大小
            int size = int.Parse(System.Configuration.ConfigurationManager.AppSettings["uploadSize"]);
            // 52428801
            if (i > size)
            {
                isSize = true;
            }
            return isSize;
        }
        /// <summary>
        /// 绑定works
        /// </summary>
        private void BindGridView(DataTable dt, GridView gv)
        {
            gv.DataSource = dt;
            gv.DataBind();
        }
        /// <summary>
        /// 获取work表中的作品数据
        /// </summary>
        private void LoadInfo()
        {
            long workID = long.Parse(ViewState["WoksID"].ToString());

            DataTable dt = DAL.Works.GetWorksByWorksID(workID).Tables[0];
            txtWorksName.Text = dt.Rows[0]["WorksName"].ToString();

            txtSubmitProfile.Text = dt.Rows[0]["SubmitProfile"].ToString();
            txtDesignIdeas.Text = dt.Rows[0]["DesignIdeas"].ToString();
            txtKeyPoints.Text = dt.Rows[0]["KeyPoints"].ToString();
            txtDemo1URL.Text = dt.Rows[0]["DemoURL"].ToString();

            if (Request.QueryString["IsSample"] != null)
            {
                txtScore.Text = dt.Rows[0]["Score"].ToString();
            }

            //判断
            if (!JudgeSubmit(dt.Rows[0]))
            {
                SetControls(false);
                error.Text = "请注意，该作品已提交不可更改！";
            }
            else
            {

                SetControls(true);
                error.Text = "";
            }
            //加载队员


            DataTable userDt = BLL.User.GetGroupMemberByWorksID(workID).Tables[0];
            StringBuilder sb = new StringBuilder();
            foreach (DataRow dr in userDt.Rows)
            {
                sb.Append("ccc\\" + dr["Account"].ToString() + ",");
            }
            user.CommaSeparatedAccounts = sb.ToString();

            JudgePeriodWorkTypeIsEnable();
            //LoadPeriodWorkType();
            //当存的不是不限时
            if (dt.Rows[0]["LevelID"].ToString() != "")
            {
                //有小类
                if (dt.Rows[0]["LevelID"].ToString() != "0")
                {
                    BindOneWorksType((DataTable)ViewState["WorksType"]);
                    //加载作品类别
                    ddlOneWorksType.SelectedIndex = -1;
                    ddlOneWorksType.Items.FindByValue(dt.Rows[0]["ParentID"].ToString()).Selected = true;


                    BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
                    ddlTwoWorksType.SelectedIndex = -1;
                    ddlTwoWorksType.Items.FindByValue(dt.Rows[0]["WorksTypeID"].ToString()).Selected = true;
                    //
                    //ddlTwoWorksType.Visible = true;
                    //ddlOneWorksType.Enabled = false;
                    //ddlTwoWorksType.Enabled = false;
                }
                else
                {
                    BindOneWorksType((DataTable)ViewState["WorksType"]);
                    //加载作品类别

                    ddlOneWorksType.SelectedIndex = -1;
                    ddlOneWorksType.Items.FindByValue(dt.Rows[0]["WorksTypeID"].ToString()).Selected = true;

                    BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
                    //ddlTwoWorksType.SelectedIndex = -1;
                    //ddlTwoWorksType.Items.FindByValue("0").Selected = true;
                    // ddlTwoWorksType.Visible=false;
                    //2014-11-13
                    //ddlOneWorksType.Enabled = false;
                    //ddlTwoWorksType.Enabled = true;
                }
            }

        }
        private void LoadPeriodWorkType()
        {

            //当是新数据时
            //判断期次的类别
            DataRow[] PWorkTypeDr = GetCurrPeriodsData();
            if (PWorkTypeDr[0]["LevelID"].ToString() != "")
            {
                if (PWorkTypeDr[0]["LevelID"].ToString() != "0")
                {
                    //期次里有小类
                    BindOneWorksType((DataTable)ViewState["WorksType"]);
                    //加载作品类别
                    ddlOneWorksType.SelectedIndex = -1;
                    ddlOneWorksType.Items.FindByValue(PWorkTypeDr[0]["ParentID"].ToString()).Selected = true;


                    BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
                    ddlTwoWorksType.SelectedIndex = -1;
                    ddlTwoWorksType.Items.FindByValue(PWorkTypeDr[0]["WorksTypeID"].ToString()).Selected = true;
                    //
                    ddlTwoWorksType.Visible = true;
                    ddlOneWorksType.Enabled = false;
                    ddlTwoWorksType.Enabled = false;
                }
                else
                {

                    //期次里有小类里有大类
                    BindOneWorksType((DataTable)ViewState["WorksType"]);
                    //加载作品类别

                    ddlOneWorksType.SelectedIndex = -1;
                    ddlOneWorksType.Items.FindByValue(PWorkTypeDr[0]["WorksTypeID"].ToString()).Selected = true;

                    BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
                    //ddlTwoWorksType.SelectedIndex = -1;
                    //ddlTwoWorksType.Items.FindByValue("0").Selected = true;

                    //ddlTwoWorksType.Visible = false;
                    //
                    ddlOneWorksType.Enabled = false;
                    ddlTwoWorksType.Enabled = true;
                }


            }
            else
            {
                //期次里没设置的
                BindOneWorksType((DataTable)ViewState["WorksType"]);
                //加载作品类别
                BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);
                ddlOneWorksType.Enabled = true;
                ddlTwoWorksType.Enabled = true;
            }
        }
        private void JudgePeriodWorkTypeIsEnable()
        {

            //先判断期次的 控制控件可操作性

            DataRow[] PWorkTypeDr = GetCurrPeriodsData();
            if (PWorkTypeDr[0]["LevelID"].ToString() != "")
            {
                if (PWorkTypeDr[0]["LevelID"].ToString() != "0")
                {
                    ddlTwoWorksType.Visible = true;
                    ddlOneWorksType.Enabled = false;
                    ddlTwoWorksType.Enabled = false;
                }
                else
                {
                    ddlOneWorksType.Enabled = false;
                    ddlTwoWorksType.Enabled = true;
                }
            }
            else
            {
                ddlOneWorksType.Enabled = true;
                ddlTwoWorksType.Enabled = true;
            }

        }
        /// <summary>
        /// 加载期次
        /// </summary>
        private void LoadPeriods()
        {
            //加载期次
            DataTable dt = DAL.Periods.GetPeriodByCourseID(long.Parse(ViewState["CourseID"].ToString())).Tables[0];
            ViewState["Periods"] = dt;
            ddlPeriod.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                if (DateTime.Now > DateTime.Parse(dr["StartSubmit"].ToString()) && DateTime.Now < DateTime.Parse(dr["EndSubmit"].ToString()).AddDays(1))
                {
                    if (Request.QueryString["PeriodID"] != null)
                    {
                        //判断是否是样例上传
                        string pID = Request.QueryString["PeriodID"].ToString();
                        if (dr["PeriodID"].ToString() == pID)
                        {
                            ddlPeriod.Enabled = false;
                            ddlPeriod.Items.Add(new ListItem(dr["PeriodTitle"].ToString(), dr["PeriodID"].ToString()));
                        }
                    }
                    else
                    {
                        ddlPeriod.Items.Add(new ListItem(dr["PeriodTitle"].ToString(), dr["PeriodID"].ToString()));
                    }
                }
            }

        }
        private void LoadGVInfo(string viewStateName, GridView gv, int type)
        {
            if (ViewState["WoksID"] == null)
            {
                ViewState["WoksID"] = 0;
            }
            if (ViewState[viewStateName] == null)
            {
                ViewState[viewStateName] = GetWorksFileDt;
            }
            ViewState[viewStateName] = DAL.Works.GetWorksFile(long.Parse(ViewState["WoksID"].ToString()), type).Tables[0];
            BindGridView((DataTable)ViewState[viewStateName], gv);
        }
        /// <summary>
        /// 判断当前是否可以修改报名
        /// </summary>
        /// <returns></returns>
        private bool JudgeTime(long PeriodID)
        {
            bool isLoad = true;
            DataTable dt = DAL.Periods.GetPeriodsByID(PeriodID).Tables[0];
            ddlPeriod.Items.Clear();

            if (DateTime.Now > DateTime.Parse(dt.Rows[0]["StartSubmit"].ToString()) && DateTime.Now < DateTime.Parse(dt.Rows[0]["EndSubmit"].ToString()))
            {
                return false;
            }
            return isLoad;

        }
        private bool JudgeSubmit(DataRow dr)
        {
            bool isSubmit = true;
            if (dr["WorksState"].ToString() == "1")
            {
                isSubmit = false;
            }
            return isSubmit;

        }
        /// <summary>
        /// 设置控件不可用
        /// </summary>
        private void SetControls(bool istrue)
        {

            txtWorksName.Enabled = istrue;
            //
            //ddlOneWorksType.Enabled = istrue;
            //ddlTwoWorksType.Enabled = istrue;

            txtSubmitProfile.Enabled = istrue;
            txtDesignIdeas.Enabled = istrue;
            txtKeyPoints.Enabled = istrue;
            btnSave.Enabled = istrue;
            btnSubmit.Enabled = istrue;
            txtDemo1URL.Enabled = istrue;
            //
            btnUpFile.Enabled = istrue;
            btnImageUpload.Enabled = istrue;
            gvImage.Enabled = istrue;
            gvWorks.Enabled = istrue;
            user.Enabled = istrue;



        }
        /// <summary>
        /// 获取作品编码
        /// </summary>
        /// <returns></returns>
        public string GetWorksCode(long worksID)
        {
            StringBuilder sCode = new StringBuilder();
            //contestID(2)+worksID(6) 共8位作品编码
            long pID = long.Parse(ddlPeriod.SelectedValue);
            if (pID < 10000)
            {
                sCode.Append(DateTime.Now.Year.ToString().Substring(2) + pID.ToString().PadLeft(4, '0') + worksID.ToString().PadLeft(6, '0'));
            }
            else
            {
                sCode.Append(DateTime.Now.Year.ToString().Substring(2) + pID.ToString() + worksID.ToString().PadLeft(6, '0'));
            }
            return sCode.ToString();

        }
        /// <summary>
        /// 控制队员个数
        /// </summary>
        /// <param name="PeriodID"></param>
        public void LoadUI(long PeriodID)
        {

            int i = 0;
            string WorksTypeID = "0";
            if (ddlPeriod.SelectedValue != "")
            {
                DataTable dt = DAL.Periods.GetPeriodsByID(PeriodID).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    i = int.Parse(dt.Rows[0]["Number"].ToString());

                    WorksTypeID = dt.Rows[0]["WorksTypeID"].ToString();
                }
                user.MaximumEntities = i - 1;

                if ((i - 1) < 1)
                {
                    user.Enabled = false;
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "showUser", "<script>document.getElementById('showUser').style.display='none'</script>");

                }
                else
                {

                    user.Enabled = true;
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "hideUser", "<script>document.getElementById('showUser').style.display='block'</script>");
                }

            }
        }
        /// <summary>
        /// 判断控件
        /// </summary>
        private void Judge()
        {

            if (JudgeTime(long.Parse(ddlPeriod.SelectedValue)))
            {
                SetControls(true);
            }
            else
            {
                SetControls(false);
            }
        }
        /// <summary>
        /// 作品显示
        /// </summary>
        private void LoadPeriodPage()
        {
            error.Text = "";
            long worksID = 0;
            //加载work
            if (ddlPeriod.SelectedValue != "")
            {
                worksID = BLL.Works.GetWorksIDByPriod(long.Parse(ddlPeriod.SelectedValue));
                //判断团队人
                LoadUI(long.Parse(ddlPeriod.SelectedValue));
                //显示基准分数
                JudgeScore();
                //加载样例
                HyperLink1.NavigateUrl = myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?PeriodID=" + ddlPeriod.SelectedValue + "&&IsSample=1";
                if (worksID != 0)
                {
                    // ViewState["WoksID"] = DESEncrypt.DesDecrypt(Request.QueryString["WorksID"].ToString());
                    ViewState["WoksID"] = worksID;
                    LoadInfo();
                    LoadGVInfo("workFile", gvWorks, 4);
                    LoadGVInfo("imageFile", gvImage, 1);

                    //2014-12-6
                    ViewState["delImageDt"] = null;
                    ViewState["delWorksDt"] = null;
                    //判断加载
                    //CreatePeopleEditor();
                    //LoadUI(long.Parse(ddlPeriod.SelectedValue));

                }
                else
                {
                    //BindOneWorksType((DataTable)ViewState["WorksType"]);
                    ////加载作品类别
                    //BindTwoWorksType((DataTable)ViewState["WorksType"], ddlOneWorksType.SelectedValue);

                    //判断期次的workType
                    LoadPeriodWorkType();
                    //重置数据
                    ResetControls();
                    ViewState["WoksID"] = 0;
                    ViewState["workFile"] = null;
                    ViewState["imageFile"] = null;
                    //2014-12-6

                    ViewState["delImageDt"] = null;
                    ViewState["delWorksDt"] = null;
                    LoadGVInfo("workFile", gvWorks, 4);
                    LoadGVInfo("imageFile", gvImage, 1);
                    SetControls(true);
                    //LoadUI(long.Parse(ddlPeriod.SelectedValue));

                }
                //加载时间和作品要求
                LoadPeriodsRequire();
                //加载人数和倒计时


            }
            else
            {
                SetControls(false);
                error.Text = "当前阶段没有作品的需求！";
            }
            ViewState["WorksID"] = worksID;

        }
        /// <summary>
        /// 重置控件
        /// </summary>
        private void ResetControls()
        {

            txtWorksName.Text = "";
            txtSubmitProfile.Text = "";
            txtDesignIdeas.Text = "";
            txtKeyPoints.Text = "";
            txtDemo1URL.Text = "";
            user.Entities.Clear();

        }
        /// <summary>
        /// 显示期次需求
        /// </summary>
        private void LoadPeriodsRequire()
        {
            lblSubmit.Text = "";
            lblScorelbl.Text = "";
            lblPublic.Text = "";
            lilRequire.Text = "";
            lblTeacher.Text = "";
            //
            DataRow[] dr = GetCurrPeriodsData();
            //
            lblSubmit.Text = transFormat(dr[0]["StartSubmit"].ToString()) + "-" + transFormat(dr[0]["EndSubmit"].ToString());
            lblScorelbl.Text = transFormat(dr[0]["StartScore"].ToString()) + "-" + transFormat(dr[0]["EndScore"].ToString());
            lblPublic.Text = transFormat(dr[0]["StartPublic"].ToString()) + "-" + transFormat(dr[0]["EndPublic"].ToString());

            lilRequire.Text = "<div style='width:700px'>" + dr[0]["Require"].ToString() + "</div>";
            lblNum.Text = dr[0]["Number"].ToString();

            if (dr[0]["CreatedBy"].ToString() != "")
            {

                DataTable userDt = DAL.User.GetUserByUserID(long.Parse(dr[0]["CreatedBy"].ToString())).Tables[0];
                if (userDt.Rows.Count > 0)
                {
                    lblTeacher.Text = userDt.Rows[0]["Name"].ToString();

                }
            }
            //加载上传人数
            DataTable worksNum = DAL.Works.GetWorksNumByPeriodID(long.Parse(ddlPeriod.SelectedValue)).Tables[0];
            if (worksNum.Rows.Count > 0)
            {
                lblWorksNum.Text = worksNum.Rows[0]["Num"].ToString();
            }
            //设置倒计时
            if (dr[0]["EndSubmit"].ToString() != "")
            {
                hfEndTime.Value = DateTime.Parse(dr[0]["EndSubmit"].ToString()).AddDays(1).ToString().Replace("-", "/");

            }
            else
            {
                hfEndTime.Value = DateTime.Now.ToString().Replace("-", "/");
            }

        }
        /// <summary>
        /// 获取当前期次数据
        /// </summary>
        /// <returns></returns>
        private DataRow[] GetCurrPeriodsData()
        {

            DataTable dt = (DataTable)ViewState["Periods"];
            DataRow[] dr = dt.Select("PeriodID=" + ddlPeriod.SelectedValue);
            return dr;

        }
        /// <summary>
        /// 转换格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string transFormat(string time)
        {
            return DateTime.Parse(time).ToString("d").Replace('-', '.');
        }
        /// <summary>
        /// 判断显示分数
        /// </summary>
        private void JudgeScore()
        {

            if (Request.QueryString["IsSample"] != null)
            {
                ddlPeriod.Enabled = false;

                rFScore.Enabled = true;
                rVScore.Enabled = true;
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "showScore", "<script>document.getElementById('showScore').style.display='block'</script>");
            }
            else
            {
                rFScore.Enabled = false;
                rVScore.Enabled = false;

                ddlPeriod.Enabled = true;
                Page.ClientScript.RegisterStartupScript(Page.GetType(), "showScore", "<script>document.getElementById('showScore').style.display='none'</script>");
            }


        }
        private void GoToPage(long worksID)
        {
            if (Request.QueryString["IsSample"] != null || Request.QueryString["PeriodID"] != null)
            {
                //  Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/OnlineEnroll.aspx?IsSample=1&&PeriodID=" + Request.QueryString["PeriodID"].ToString());
                Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?View=1&&WorksID=" + worksID);
            }
            else
            {
                // Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/OnlineEnroll.aspx");
                Response.Redirect(myWeb.Url + "/_layouts/15/WorkEvaluate/Comments.aspx?View=1&&WorksID=" + worksID);
            }
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        private void DelList(string fileName, string type)
        {
            if (type == "4" || type == "3")
            {
                DelListItem(fileName, "workVideo");
            }
            else if (type == "2")
            {
                DelListItem(fileName, "workPic");
            }
            else
            {
                DelListItem(fileName, "workFile");
            }
        }
        /// <summary>
        /// 删除附件项
        /// </summary>
        /// <param name="path"></param>
        /// <param name="listName"></param>
        private void DelListItem(string fileName, string listName)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite ElevatedsiteColl = new SPSite(mySite.ID))
                {
                    using (SPWeb ElevatedSite = ElevatedsiteColl.OpenWeb(myWeb.ID))
                    {
                        ElevatedSite.AllowUnsafeUpdates = true;
                        SPList list = ElevatedSite.Lists.TryGetList(listName);
                        SPDocumentLibrary docLib = (SPDocumentLibrary)list;
                        SPFolder rootFolder = docLib.RootFolder;
                        if (listName == "workVideo")
                        {

                            for (int i = 0; i < rootFolder.SubFolders.Count; i++)
                            {
                                if (rootFolder.SubFolders[i].Files[0].Name == fileName)
                                {
                                    rootFolder.SubFolders[i].Delete();

                                }
                            }
                            //不开启资产库
                            for (int i = 0; i < rootFolder.Files.Count; i++)
                            {
                                if (rootFolder.Files[i].Name == fileName)
                                {
                                    rootFolder.Files[i].Delete();
                                }
                            }

                        }
                        else
                        {
                            for (int i = 0; i < rootFolder.Files.Count; i++)
                            {
                                if (rootFolder.Files[i].Name == fileName)
                                {
                                    rootFolder.Files[i].Delete();
                                }
                            }
                        }
                        ElevatedSite.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        private bool JudgeWordCount()
        {
            bool isSave = false;
            if (txtKeyPoints.Text.Length < 200 || txtKeyPoints.Text.Length > 800)
            {
                isSave = true;
                errorKeyPoints.Text = "请注意字数限制";
                errorKeyPoints.Focus();
            }
            else
            {
                errorKeyPoints.Text = "";

            }
            if (txtDesignIdeas.Text.Length < 500 || txtDesignIdeas.Text.Length > 2000)
            {
                isSave = true;
                errorDesignIdeas.Text = "请注意字数限制";
                errorDesignIdeas.Focus();
            }
            else
            {
                errorDesignIdeas.Text = "";

            }

            if (txtSubmitProfile.Text.Length < 50 || txtSubmitProfile.Text.Length > 500)
            {
                isSave = true;
                errorSubmitProfile.Text = "请注意字数限制";
                errorSubmitProfile.Focus();
            }
            else
            {
                errorSubmitProfile.Text = "";
            }


            //团队上传
            if (((DataTable)ViewState["workFile"]).Rows.Count != GetUserCount() + 1)
            {
                isSave = true;
                jiangJieError.Text = "请注意，上传讲解视频的数量应与团队人数相符";
            }
            return isSave;
        }

        private int GetUserCount()
        {
            ArrayList list = user.ResolvedEntities;
            SPUser member;
            int num = 0;
            foreach (Microsoft.SharePoint.WebControls.PickerEntity p in list)
            {
                member = myWeb.EnsureUser(p.Key);
                if (member.LoginName != SPContext.Current.Web.CurrentUser.LoginName)
                {
                    num++;
                }

            }
            return num;

        }

        #endregion

    }
}
