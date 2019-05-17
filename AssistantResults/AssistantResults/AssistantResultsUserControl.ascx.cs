using Microsoft.SharePoint;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using System.Globalization;

namespace AssistantResults.AssistantResults
{
    //个人学习助手对应的结果，编辑，显示与当前数据相关联的附件；如果为新建，则显示附件库中所有附件
    public partial class AssistantResultsUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            SPUser user = SPContext.Current.Web.CurrentUser;
            bool isExits = ListExits();
            if (!isExits)
            {
                lblMsg.Text = webObj.ListName + "  列表不存在！";
                AppAction.Visible = false;
                lbAppraise.Text = "";
                return;
            }
            //isExits = SubwebExits();
            //if (!isExits)
            //{
            //    lblMsg.Text = webObj.WebRelativeUrl + "  子网站不存在！";
            //    AppAction.Visible = false;
            //      lbAppraise.Text = "";
            //    return;
            //}
            if (!Page.IsPostBack)
            {
                if (user != null)
                {
                    string ID = Page.Request.QueryString["ID"] == null ? "0" : Page.Request.QueryString["ID"];

                    //bool hasRight = UserHaveApproveRight(SPContext.Current.Site.ID, SPContext.Current.Web.Name, webObj.ListName, user);
                    //if (hasRight)
                    //    ViewState["hasRight"] = 1;
                    //else
                    //{
                    bool hasAttachRight = UserHasAttachRight(user, int.Parse(ID));
                    if (!hasAttachRight)
                    {
                        ViewState["hasRight"] = 0;
                    }
                    else
                        ViewState["hasRight"] = 1;

                    //}

                    lbAppraise.Text = webObj.ShowTitle;
                    if (ViewState["hasRight"].ToString() == "0")
                    {
                        if (webObj.Action != "Edit")
                            divUpload.Visible = false;
                        else
                            divTotal.Visible = false;

                    }
                }
                else//匿名用户不可见
                {
                    divTotal.Visible = false;
                }
                Lnk1_Click(null, null);

                NewReulst();
            }
            InitResults();
            //显示上传的控件和链接
            btnAppraise.Click += btnAppraise_Click;
            btnAdd.Click += BtnAdd_Click;
            lnk1.Click += Lnk1_Click;
            lnk2.Click += Lnk2_Click;
            lnk3.Click += Lnk3_Click;
        }

        private void Lnk3_Click(object sender, EventArgs e)
        {
            divLinks.Visible = true;
            divUpload.Visible = false;
            divDocs.Visible = false;

        }

        private void Lnk2_Click(object sender, EventArgs e)
        {
            divUpload.Visible = true;
            divDocs.Visible = false;
            divLinks.Visible = false;
        }

        private void Lnk1_Click(object sender, EventArgs e)
        {
            divDocs.Visible = true;
            divUpload.Visible = false;
            divLinks.Visible = false;
        }

        //添加链接
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (txtUrl.Text == "")
            {
                lblMsgLnks.Text = "网址不能为空！";
                txtUrl.Focus();
                return;
            }
            else
                lblMsgLnks.Text = "";
            string title=txtDescrip.Text ;
            if (title == "")
            {
                if (txtUrl.Text.IndexOf("/") > 7)
                    title = txtUrl.Text.Substring(0, txtUrl.Text.IndexOf("/"));//7为http://后面的第一个字符
                else
                    title = txtUrl.Text;

            }
            WriteResultToList("add", new string[] {title  ,txtUrl.Text  },0, -1);
            txtDescrip.Text = "";
            txtUrl.Text = "";
        }

        //上传附件
        void btnAppraise_Click(object sender, EventArgs e)
        {
            if (FileUpload1.FileName != "")
                UpdateDocument();
            //UpdateDocumentToOnedrive();

        }
        #endregion
        #region 属性
        public AssistantResults webObj { get; set; }
        #endregion
        #region 方法
        /// <summary>
        /// 新建时设置标记
        /// </summary>
        private void NewReulst()
        {
            string url = Request.Url.ToString();
            if (url.IndexOf("NewForm.aspx") > 0)
                SetFlag(0);
        }
        private void InitResults()
        {
             
            lblMsg.Text = "";
            tbContent.Rows.Clear();
            tbResult.Rows.Clear();
            lblMsgNoResults.Text = "";
            lblMsgNoResults.Visible = false;
            DataTable dt = GetLinkResults();
            if (webObj.Action != "Edit")
            {
                divDocs.Visible = false;
                tblNavi.Visible = false;
                //divLine.Visible = false;

            }
            if (ViewState["hasRight"] == null)
                return;

            if (webObj.Action == "Edit" && ViewState["hasRight"]!=null && ViewState["hasRight"].ToString() == "1" || webObj.Action != "Edit")
            {

                if (dt == null || dt.Rows.Count ==0)
                {
                    if (webObj.Action == "Edit")
                    {
                        lblMsgNoResults.Text = webObj.ShowTitle + "为空！";
                        lblMsgNoResults.Visible = true;
                    }
                    else
                        divTotal.Visible = false;

                }
                else
                    FillResults(dt.Copy(), SPBaseType.GenericList, false, ref tbResult);
            }

            else
            {
                if (dt == null && (webObj.Action != "Edit" || ViewState["hasRight"].ToString() == "0"))
                    divTotal.Visible = false;
            }
            DataTable dt1;

            //  webObj.Action == "Edit"
            if (webObj.Action == "Edit" && ViewState["hasRight"].ToString() == "1")
            {
                dt1 = GetDocLinks;
                if (dt1 != null)
                {
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string[] lnkUrl = dr["Result"].ToString().Split(',');
                            string url = lnkUrl[0];
                            if (url.StartsWith("http://"))
                            {
                                url = url.Replace("http://", "");
                                if (url.IndexOf("/") > 0)
                                    url = url.Substring(url.IndexOf("/"));
                                else
                                    url = "";
                            }
                            if (url != "")
                            {
                                DataRow[] drs = dt1.Select("FileRef='" + url + "'");
                                if (drs.Length > 0)
                                    dt1.Rows.Remove(drs[0]);
                            }
                        }
                        dt1.AcceptChanges();
                    }
                    //if (dt1.Rows.Count > 10)
                    //    tbContent.Width = 400;
                    FillResults(dt1.Copy(), SPBaseType.DocumentLibrary, true, ref tbContent);
                }
            }
        }
        public string GetAccount(SPUser lgUser )
        {
            string loginName = lgUser .LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        private void UpdateDocumentToOnedrive()
        {
            SPUser lgUser = SPContext.Current.Web.CurrentUser;
            Stream filedataStream = FileUpload1.PostedFile.InputStream;
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            string filename = FileUpload1.FileName;
            string titleName = "Rooting" + DateTime.Today.ToString("yyyy") + "第" + weekOfYear + "周";
            filename = titleName + filename.Substring(filename.IndexOf("."));

            int dataLen = FileUpload1.PostedFile.ContentLength;

            string fileType = FileUpload1.PostedFile.ContentType;

            byte[] fileData = new byte[dataLen];

            filedataStream.Read(fileData, 0, dataLen);

            string id = Page.Request.QueryString["ID"];
            string lngAccount = GetAccount(lgUser);
            if (lngAccount == "system") lngAccount = "xueqingxia";

            try
            {
                string personalUrl = "http://localhost/personal/" + lngAccount;//进入个人网站
                using (SPSite spSite = new SPSite(personalUrl))
                {
                    using (SPWeb sWeb = spSite.OpenWeb())
                    {
                        sWeb.AllowUnsafeUpdates = true;
                        SPDocumentLibrary library = sWeb.Lists["文档"] as SPDocumentLibrary;
                        string libraryRelativePath = library.Folders[0].Url;
                        string libraryPath = spSite.MakeFullUrl(libraryRelativePath);//带网站集的url
                        string documentPath = libraryPath + "/" + filename;// documentName + fileType;//创建的文档名称
                        SPFile sFile = sWeb.Files.Add(documentPath, filedataStream, true);// sList.RootFolder.Files.Add (filename, fileData);

                        sWeb.AllowUnsafeUpdates = true;
                        SPListItem item = sFile.Item;
                        //item["ParentID"] = id;//查阅项改为数字，去掉了附件表中的关联
                        item["Editor"] = SPContext.Current.Web.CurrentUser.ID;
                        item["Author"] = SPContext.Current.Web.CurrentUser.ID;
                        item["标题"] = titleName;
                        item.Update();
                        sWeb.AllowUnsafeUpdates = false;
                        WriteResultToList("add", new string[] { titleName, documentPath }, item.ID, -1);
                        //WriteAttachments();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();// "此文档已经存在或者文件类型被限制，请重新选择！";
            }
        }
       
        //上传附件后缀为年月日时分秒豪秒
        private void UpdateDocument()
        {
            SPUser lgUser = SPContext.Current.Web.CurrentUser;
            string filename = FileUpload1.FileName;
            filename = filename.Substring(filename.LastIndexOf("/") + 1);
            string titleName = filename.Substring(0, filename.IndexOf("."));//去掉扩展名
            string fileExt = filename.Substring(filename.IndexOf(".") + 1);
            if (!webObj.LnkDescription.ToLower () .Contains (fileExt.ToLower () ))
                {
                lblMsg.Text = "只能上传指定格式的文件！";
                return;
            }
            string modifyFileName =lgUser.ID.ToString ();// DateTime.Today.ToString("yyyyMMdd") + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            filename = titleName + modifyFileName + filename.Substring(filename.IndexOf("."));
            Stream filedataStream = FileUpload1.PostedFile.InputStream;

            int dataLen = FileUpload1.PostedFile.ContentLength;

            string fileType = FileUpload1.PostedFile.ContentType;

            byte[] fileData = new byte[dataLen];

            filedataStream.Read(fileData, 0, dataLen);

            string id = Page.Request.QueryString["ID"];
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))//.AllWebs[SPContext.Current.Web.ServerRelativeUrl])
                    {
                        sWeb.AllowUnsafeUpdates = true;
                        SPDocumentLibrary library = sWeb.Lists[webObj.ListMediaLib] as SPDocumentLibrary;
                        string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
                        string libraryPath = spSite.MakeFullUrl(libraryRelativePath);//带网站集的url
                        string documentPath = libraryPath + "/" + filename;// documentName + fileType;//创建的文档名称
                        SPFile sFile;
                        try
                        {
                            sFile = sWeb.Files.Add(documentPath, filedataStream, true);// sList.RootFolder.Files.Add (filename, fileData);
                            sWeb.AllowUnsafeUpdates = true;
                            SPListItem item = sFile.Item;
                            //item["ParentID"] = id;//查阅项改为数字，去掉了附件表中的关联
                            item["Editor"] = SPContext.Current.Web.CurrentUser.ID;
                            item["Author"] = SPContext.Current.Web.CurrentUser.ID;
                            item["标题"] = titleName;
                            item.Update();
                            sWeb.AllowUnsafeUpdates = false;
                            WriteResultToList("add", new string[] { titleName, documentPath }, item.ID, -1);
                        }
                        catch
                        {
                            //sFile = sWeb.GetFile(documentPath);
                        }
                       
                        //WriteAttachments();
                    }
                }
            });
        }

        /// <summary>
        /// 从当前网站的下面获取自定义列表
        /// </summary>
        /// <param name="sWeb"></param>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private DataTable GetLinkResults()
        {
            string id = Page.Request.QueryString["ID"] == null ? "" : Page.Request.QueryString["ID"];
            SPUser user = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
    {

        using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
        {
            using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
            {
                SPList sList = sWeb.Lists.TryGetList(webObj.ListMediaRel);
                if (sList == null)
                {
                    lblMsg.Text = webObj.ListMediaRel + " 列表不存在！";
                }
                else
                {
                    lblMsg.Text = "";
                    SPQuery qry = new SPQuery();
                    SPListItemCollection docITems = null;
                    //当前个人学习记录的文档
                    qry.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Author' /><FieldRef Name='Result' /><FieldRef Name='AssistantID' /><FieldRef Name='Title' /><FieldRef Name='Created' />";
                    if (id != "")
                        qry.Query = @"<Where><Eq><FieldRef Name='AssistantID'/><Value Type='Number'>" + id + "</Value></Eq></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                    else//新建
                    {
                        //根据时间获取新建项目的结果，根据用户和时间
                        string txtDate = SPUtility.CreateISO8601DateTimeFromSystemDateTime(GetNewResultTime);
                        qry.Query = @"<Where><And><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + user.ID + "</Value></Eq><Eq><FieldRef Name='Created'/><Value Type='DateTime' IncludeTimeValue='TRUE'>" + txtDate + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";
                    }
                    docITems = sList.GetItems(qry);
                    DataTable dt = docITems.GetDataTable();
                    if (dt == null && sList.ItemCount >0)
                        dt=sList.Items.GetDataTable().Clone();
                    ViewState["LinkResults"] = dt;
                }
            }
        }
    });
            return ViewState["LinkResults"] as DataTable;
        }
            //读取指定网站下的所有文档库
        DataTable allLinks = null;
        private DataTable GetDocLinks
        {
            get
            {
                if (allLinks != null)
                    return allLinks;
                string id = Page.Request.QueryString["ID"] == null ? "" : Page.Request.QueryString["ID"];
                SPUser user = SPContext.Current.Web.CurrentUser;
                DataTable allDocLinks = null;
                SPListItemCollection docITems = null;
                SPQuery qry;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    try
                    {
                        using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                        {
                            //SPWeb sWeb = spSite.AllWebs[webObj.WebRelativeUrl];

                            //List<string> webs = new List<string>();
                            //webs.Add(webObj.WebRelativeUrl);
                            //foreach (SPWeb subWeb in sWeb.Webs)
                            //    webs.Add(subWeb.ServerRelativeUrl);
                            //foreach (string webUrl in webs)
                            //{
                            using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                            {
                                //foreach (SPList sList in spWeb.Lists)//网站下的所有列表
                                //{
                                //    if (sList.BaseType == SPBaseType.DocumentLibrary && sList.Hidden == false)
                                //    {
                                SPList sList = spWeb.Lists.TryGetList(webObj.ListMediaLib);
                                qry = new SPQuery();
                                //当前用户创建的文档
                                qry.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Author' /><FieldRef Name='FileRef' /><FieldRef Name='Title' />";
                                qry.Query = @"<Where><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + user.ID + "</Value></Eq></Where>";
                                qry.Folder = sList.RootFolder; ;
                                qry.ViewAttributes = "Scope=\"RecursiveAll\"";
                                docITems = sList.GetItems(qry);
                                if (docITems.Count > 0)
                                {
                                    DataTable dt = docITems.GetDataTable();
                                    if (allDocLinks == null)
                                        allDocLinks = dt.Clone();
                                    DataRow[] flds;//从表格中移除文件夹
                                    foreach (SPListItem fld in sList.Folders)
                                    {
                                        flds = dt.Select("FileRef='" + fld["FileRef"] + "'");
                                        if (flds.Length > 0)
                                            dt.Rows.Remove(flds[0]);


                                    }
                                    //DataRow dr;
                                    //for  (int i= dt.Rows.Count - 1;i>0;i-- )
                                    //{
                                    //    dr = dt.Rows[i]; 
                                    //    if (dr["FileRef"].ToString().ToLower() .EndsWith(".aspx"))
                                    //        dt.Rows.Remove(dr);
                                    //}
                                    allDocLinks.Merge(dt.Copy());
                                }
                            }
                                    }
                            //    }
                            //}
                        //}
                    }
                    catch (Exception ex)
                    {
                        lblMsg.Text = "SPQuery:" + ex.ToString();
                    }
                });
                if (allDocLinks != null)
                    allLinks = allDocLinks.Copy();
                return allDocLinks;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="docITems"></param>
        /// <param name="lstType"></param>
        /// <param name="lstID">控件不重复命名</param>
        /// <param name="isNew"></param>
        private void FillResults(DataTable  docITems, SPBaseType lstType, bool isNew ,ref Table tbContent)
        { //填充内容
            //tbContent.Rows.Clear();
            TableRow tRow;
            TableCell tCell;
            LinkButton lnkBtn;
            try
            {
                
                string title="";
                string[] lnkUrl=new string[] { };
                for (int i = 0; i < docITems.Rows.Count ; i++)
                {
                    tRow = new TableRow();
                    tCell = new TableCell();
                    if (lstType == SPBaseType.DocumentLibrary && ViewState["hasRight"].ToString() == "1" && webObj.Action == "Edit")
                    {
                        title = docITems.Rows[i]["Title"].ToString ();
                        if (title =="")
                        {
                            title = docITems.Rows[i]["FileRef"].ToString ();
                            title = title.Substring(title.LastIndexOf("/") + 1);
                        }
                        tCell.Controls.Add(new LiteralControl("<a href='" + docITems.Rows[i]["FileRef"] + "'>" + title + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));

                    }
                    else if (lstType == SPBaseType.GenericList)
                    {
                        lnkUrl = docITems.Rows[i]["Result"].ToString().Split(',');
                        tCell.Controls.Add(new LiteralControl("<a href='" + lnkUrl[0] + "'>" + docITems.Rows[i]["Title"] + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));
                        //"http://xqx2012/blog/blog1/DocLib/%E5%91%BD%E5%90%8D%E6%96%87%E6%A1%A3%E8%A7%84%E5%88%99201807301426633.docx, 命名文档"
                    }

                    tRow.Cells.Add(tCell);//加审核项


                    if (ViewState["hasRight"].ToString() == "1" && webObj.Action == "Edit")
                    {
                        tCell = new TableCell();
                        tCell.Width = 50;
                        lnkBtn = new LinkButton();
                        
                        if (isNew)
                        {
                            lnkBtn.CommandName = "add";
                            lnkBtn.ID = "btn" +tbContent.Rows.Count .ToString ();
                            lnkBtn.CommandArgument =title +";"+ docITems.Rows[i]["FileRef"].ToString ();//添加文档库的链接
                            lnkBtn.Text = "选择";
                            lnkBtn.OnClientClick = "if(!confirm('确定要选择“"+title+"”作为结果吗？'))return false;";
                        }
                        else
                        {
                            lnkBtn.CommandName = "edit";
                            lnkBtn.ID = "btn" +tbContent.Rows.Count .ToString ()+"_" + docITems.Rows[i]["ID"];//不同列表的ID会出现重复的情况
                            lnkBtn.CommandArgument =docITems.Rows[i]["Title"] +";"+ lnkUrl[0];//添加文档库的链接 
                            lnkBtn.Text = "删除";
                            lnkBtn.OnClientClick = "if(!confirm('确定要从结果中删除“"+docITems.Rows[i]["Title"]+"” 吗？'))return false;";
                        }
                        lnkBtn.Click += lnkBtn_Click;
                        tCell.Controls.Add(lnkBtn);
                        tRow.Cells.Add(tCell);// 
                        
                        
                    }
                    tbContent.Rows.Add(tRow);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = "docITems.Count" + ex.ToString();
            }
        }
        //添加表格结果行
        private void AddRowToTbContent(string url, string title, int resultID,int rowIndex= 0)
        {
            TableRow tRow = new TableRow();
            TableCell tCell = new TableCell();
            tCell.Controls.Add(new LiteralControl("<a href='" + url + "'>" + title + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = 50;
            LinkButton lnkBtn = new LinkButton();
            lnkBtn.CommandName = "edit";
            lnkBtn.ID = "btn0" + "_" + resultID;//不同列表的ID会出现重复的情况

            lnkBtn.Text = "删除";
            lnkBtn.OnClientClick = "if(!confirm('确定要结果中删除“"+title+"”吗？'))return false;";

            lnkBtn.Click += lnkBtn_Click;
            tCell.Controls.Add(lnkBtn);
            tRow.Cells.Add(tCell);// 
            tbResult .Rows.AddAt(0,tRow);

        }
        private DateTime GetNewResultTime
        {
            get
            {
                if (ViewState["dtAddNew"] == null)
                    ViewState["dtAddNew"] = DateTime.Now;
                return (DateTime)ViewState["dtAddNew"];
            }
            

        }
        //添加结果链接到结果库中
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">添加链接；删除链接</param>
        /// <param name="lnkUrl">链接的地址1和说明0，添加时使用</param>
        /// <param name="id">要删除的结果ID</param>
        private void WriteResultToList(string action, string[] lnkUrl, int id,int rowIndex)
        {
            lblMsgNoResults.Visible = false;
            string assistantID = Page.Request.QueryString["ID"] == null ? "0" : Page.Request.QueryString["ID"];
            SPSecurity.RunWithElevatedPrivileges(delegate ()
        {
            using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPList sList = sWeb.Lists[webObj.ListMediaRel];
                    SPListItem item;
                    sWeb.AllowUnsafeUpdates = true;

                    if (action == "add" )
                    {
                        item = sList.Items.Add();
                        item["AssistantID"] = assistantID;
                        SPFieldUrlValue urlValue = new SPFieldUrlValue();

                        item["标题"] = lnkUrl[0];
                        urlValue.Description = lnkUrl[0];
                        urlValue.Url = lnkUrl[1];
                        item["Result"] = urlValue;
                        item["修改者"] = SPContext.Current.Web.CurrentUser.ID;
                        item["创建者"] = SPContext.Current.Web.CurrentUser.ID;
                        if (assistantID == "0")//新建
                            item["创建时间"] = GetNewResultTime ;
                        item.Update();
                        if (rowIndex >= 0)//直接上传附件不需要此操作
                            tbContent.Rows.RemoveAt(rowIndex);

                        AddRowToTbContent(lnkUrl[1], lnkUrl[0], item.ID);

                    }
                    else
                    {
                        item = sList.GetItemById(id);
                        item.Delete();
                        if (rowIndex >= 0)//直接上传附件不需要此操作
                        {
                            tbResult.Rows.RemoveAt(rowIndex);//表格拆分两个
                        }
                            
                    }
                    sWeb.AllowUnsafeUpdates = false;
                    
                    //WriteAttachments();
                }
            }
        });
            SetFlag(tbResult.Rows.Count );//传递参数，在事件中对新建的助手结果进行处理
        }
        private void SetFlag(int num)
        {
            SPWeb web = SPContext.Current.Web;
            SPUser user = web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(web.Site.ID))
                {
                    using (SPWeb sWeb = spSite.OpenWeb(web.ID))
                    {
                        SPList sList = sWeb.Lists[webObj.ListName + "临时文件"];
                        SPQuery qry = new SPQuery();
                        SPListItemCollection docITems = null;
                        //当前个人学习记录的文档
                        qry.ViewFields = "<FieldRef Name='ID' /><FieldRef Name='Author' /><FieldRef Name='Title' /><FieldRef Name='Created' />";

                        //根据时间获取新建项目的结果，根据用户和时间
                        qry.Query = @"<Where><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Lookup'>" + user.ID + "</Value></Eq></Where>";

                        docITems = sList.GetItems(qry);
                        SPListItem imte;
                        if (docITems.Count > 0)
                        {
                            imte = docITems[0];

                        }
                        else
                        {
                            imte = sList.Items.Add();
                            imte["创建者"] = user;
                        }
                        imte["修改者"] = user;
                        imte["标题"] = num > 0 ? "1" : "0"; //1为有附件，0为无
                        imte["创建时间"] = GetNewResultTime;
                        sWeb.AllowUnsafeUpdates = true;
                        imte.Update ();
                        sWeb.AllowUnsafeUpdates = false;
                    }
                }
            });



        }
        //更改为：结果只保存链接，包括上传的文档库地址
        void lnkBtn_Click(object sender, EventArgs e)
        {
            LinkButton lnkBtn = sender as LinkButton;
            string id = "0";// 
            string action = lnkBtn.CommandName;
            int rowIndex;
            if (action == "edit")
            {
                id = lnkBtn.ID.Substring(lnkBtn.ID.IndexOf("_") + 1);
                rowIndex = int.Parse ( lnkBtn.ID.Substring(3,lnkBtn.ID.IndexOf("_")-3));
            }
            else
                rowIndex =  int.Parse ( lnkBtn.ID.Substring(3));
            string[] lnkUrl = lnkBtn.CommandArgument==""?null:lnkBtn.CommandArgument.Split(';');
            
            WriteResultToList(action, lnkUrl, int.Parse (id),rowIndex);

        }
        private bool ListExits()
         {
             bool isExits = false;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                 {
                     using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         SPList parentList = spWeb.Lists.TryGetList(webObj.ListName);
                         if (parentList == null)
                             isExits = false;
                         else
                             isExits = true;
                     }
                 }
             });
             return isExits;
         }
       private bool SubwebExits()
        {
            bool isExits = false;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                 {
                   
                    using (SPWeb spWeb = spSite.AllWebs[webObj.WebRelativeUrl ])
                    {


                        isExits = spWeb.Exists; 
                    }
                }
            });
            return isExits;
        }
        //用户是否有权限,编辑的时候，创建者等于当前用户
         private bool UserHasAttachRight(SPUser user,int itemID)
         {
             bool isRight = false ;
            if (itemID == 0) return true;//新建
             using (SPSite spSite = new SPSite(SPContext.Current.Site.ID ))
             {
                 using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                 {
                     SPList sList = sWeb.Lists.TryGetList(webObj.ListName);
                     if (sList != null)
                     {
                         SPListItem item = sList.GetItemById(itemID);
                         SPFieldUserValue author = new SPFieldUserValue(sWeb, (item["创建者"].ToString()));
                         if (author.LookupId == user.ID)
                             isRight = true;
                         //else
                         //{
                         //    SPFieldUserValueCollection adUsers = item["AuthorName"] as SPFieldUserValueCollection;
                         //    foreach (SPFieldUserValue adUser in adUsers)
                         //        if (adUser.LookupId == user.ID)
                         //        {
                         //            isRight = true;
                         //            break;
                         //        }
                         //}
                     }
                 }
             }
             return isRight;
         }
        //用户有审核权限
         private bool UserHaveApproveRight(Guid siteID, string webName, string lstName, SPUser user)
         {
             bool isRight = true;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 try
                 {
                     using (SPSite spSite = new SPSite(siteID))
                     {
                         using (SPWeb sWeb = spSite.AllWebs[webName])
                         {
                             SPList sList = sWeb.Lists[lstName];
                             sList.DoesUserHavePermissions(user, SPBasePermissions.ApproveItems);
                             isRight = sList.DoesUserHavePermissions(user, SPBasePermissions.ApproveItems);
                         }  //return DoesUserHavePermssionsToWeb(ref user, ref sWeb);
                     }
                 }
                 catch
                 {
                     isRight = false;
                 }
             });
             return isRight;
         }
         #endregion
    }
}
