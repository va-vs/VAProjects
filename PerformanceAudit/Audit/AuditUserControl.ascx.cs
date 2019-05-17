using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
namespace PerformanceAudit.Audit
{
    public partial class AuditUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SPContext.Current.Web.CurrentUser != null)
                {
                    bool strStep = GetAppraiseAction(webObj.CurrentAction);
                    if (!strStep  )
                    {
                        AppraiseDiv.Visible = false;//什么也不显示
                        //lblMsg.Text = strStep+"         "+ webObj.CurrentAction ;
                    }
                    else
                    {
                        SPUser user = SPContext.Current.Web.CurrentUser;
                        bool hasRight = UserHaveApproveRight(SPContext.Current.Site.ID, SPContext.Current.Web.Name, webObj.ListName, user);
                        if (!hasRight)
                        {
                            AppraiseDiv.Visible = false;//什么也不显示
                            //lblMsg.Text = "no has right";
                            return;
                        }
                        action = webObj.CurrentAction;  
                        AppAction.Visible = true;
                        lbAppraise.Text = action;
                        string ID = Page.Request.QueryString["ID"];
                        try
                        {
                            int flag = 0;
                            SPListItem item = GetAppraiseRecord(action, ID, webObj.ReturnResult, ref flag);
                            if (flag ==1)
                            {
                                btnAppraise.Enabled = false;
                                lblMsg.Text = "此" + webObj.ListName + "已经" + webObj.CurrentAction;
                                btnNoPass.Enabled = false;
                                //ddlResult.SelectedItem.Text = item["结果"].ToString(); 
                                return;
                            }
                            else
                            {
                                item = GetAppraiseRecord(action, ID, "不" + webObj.ReturnResult, ref flag);
                                if (flag ==2)
                                {
                                    btnAppraise.Enabled = false;
                                    btnNoPass.Enabled = false;
                                    lblMsg.Text = "此" + webObj.ListName + webObj.CurrentAction + "不" + webObj.ReturnResult;
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            lblMsg.Text = ex.ToString();
                        }
                    }
                }
                else
                {
                    AppraiseDiv.Visible = false;
                }
            }
            //WriteChkItems();
            btnAppraise.Click += btnAppraise_Click;
            btnCancle.Click += btnCancle_Click;
            btnNoPass.Click += btnNoPass_Click;
        }

        void btnNoPass_Click(object sender, EventArgs e)
        {
          bool   result= AddAppraiseRecord(action,2,"不"+webObj.ReturnResult );
          if (!result)
              lblMsg.Text = webObj.CurrentAction + " 失败";
          else
              RedirectNext();
        }
        void btnCancle_Click(object sender, EventArgs e)
        {
            ReturnLast();
        }
        private void ReturnLast()
        {
            string returnUrl = webObj.RetUrl;
            if (Page.Request.QueryString["Source"] != null)
                returnUrl = Page.Request.QueryString["Source"];

            Response.Redirect(returnUrl);

        }

        private void RedirectNext()//审核完成后进入下一条
        {
            string lstName = webObj.RetUrl;
            string returnUrl = "";
            SPWeb web = SPContext.Current.Web;
            SPList spList = web.Lists.TryGetList(webObj.ListName);
            if (spList != null)
            {
                SPQuery qry = new SPQuery();
                qry.Query = @"<Where><And><Neq><FieldRef Name='Flag' /><Value Type='Number'>1</Value></Neq><Neq><FieldRef Name='Flag' /><Value Type='Number'>2</Value></Neq></And></Where><OrderBy><FieldRef Name='Author'/></OrderBy>";
                SPListItemCollection listItems = spList.GetItems(qry);
                if (listItems.Count>0)
                {
                    returnUrl = spList.DefaultDisplayFormUrl+"?ID="+ listItems[0].ID.ToString();
                }              
            }
            if (returnUrl!="")
            {
                Response.Redirect(returnUrl);
            }
            else
            {
                ReturnLast();
            }
            
        }

        /// <summary>
        /// 通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAppraise_Click(object sender, EventArgs e)
        {
            //bool result = ValidChkItems();
            //if (!result)
            //    return;
            bool result = AddAppraiseRecord(action, 1, webObj.ReturnResult);
            if (!result)
                lblMsg.Text = webObj.CurrentAction + " 失败";
            else
                RedirectNext();

        }
        #endregion
        #region 属性
        //获取业绩子表的关联字段内部名，显示名称同主表标题，查阅项
        private string GetSubTitleInterlname
        {
            get
            {
                if (ViewState["subFieldInterlName"] != null)
                    return ViewState["subFieldInterlName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        string displayName = GetTitleDispName;//获取主表的标题显示名称
                        SPField field = list.Fields.GetField(displayName);
                        ViewState["subFieldInterlName"] = field.InternalName ;
                        return field.InternalName;
                    }
                    else
                        return "";
                }
            }
        }
        //创建者
        private string GetAuthorDispName
        {
            get
            {
                if (ViewState["AuthorShowName"] != null)
                    return ViewState["AuthorShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ResultList);
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Author");
                        ViewState["AuthorShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        //获到主表标题的中文显示名称，如论文标题，内部名称为Title
        private string GetTitleDispName
        {
            get
            {
                if (ViewState["TitleShowName"] != null)
                    return ViewState["TitleShowName"].ToString();
                else
                {
                    SPWeb web = SPContext.Current.Web;
                    SPList list = web.Lists.TryGetList(webObj.ListName );
                    if (list != null)
                    {
                        SPField field = list.Fields.GetFieldByInternalName("Title");
                        ViewState["TitleShowName"] = field.Title;
                        return field.Title;
                    }
                    else
                        return "";
                }
            }
        }
        public Audit webObj { get; set; }
        string _action;
        public string action
        {
            get
            {
                if (_action == null)
                    _action = webObj.CurrentAction;// GetAppraiseAction();
                return _action;
            }
            set { _action = value; }
        }
        string _creativty;
        public string creatity
        {
            get
            {
                if (_creativty == null)
                    _creativty = GetCreativityName( );
                return _creativty;
            }
            set { _creativty = value; }
        }
        #endregion
        #region 方法
        //填写要评审的项目
        private void WriteChkItems()
        {
            //填充内容
            string items = webObj.ChkItems.Replace("；", ";").Trim ();
            string des = webObj.ChkDesc.Replace("；", ";").Trim();
            string[] chkItems = Regex.Split(items, ";");
            string[] chkDes = Regex.Split(des, ";");
            tbContent.Rows.Clear();

            CheckBox chk;
            TableRow  tRow;
            TableCell tCell;
            for (int i = 0; i < chkItems.Length;i++ )
            {
                tRow = new TableRow();
                tCell = new TableCell();
                chk = new CheckBox();
                chk.Text = chkItems[i];
                tCell.Controls.Add(chk);
                tRow.Cells.Add(tCell);//加审核项
                tbContent.Rows.Add(tRow);
                tRow = new  TableRow();
                tCell = new  TableCell();
                tCell.Controls.Add(new LiteralControl("<div style=\"color:#a0a0a0\">" + chkDes[i] + "</div>"));
                tRow.Cells.Add(tCell);//加审核说明
                tbContent.Rows.Add(tRow);
            }
                //chkAppraise.Items.Add(new ListItem(chkItem));
        }
        //验证审核的各项内容是否正确
        private bool ValidChkItems()
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList  myList= web.Lists.TryGetList(webObj.ListName);
            SPListItem myItem = myList.GetItemById(int.Parse(ID));
            bool result = true;
            int i = 0;
            CheckBox item;
           while (i < tbContent.Rows.Count)
            {
                item = (CheckBox)tbContent.Rows[i].Cells[0].Controls[0];    
                if (item.Checked  == false)
                {
                    result = false;
                    lblMsg.Text = "您还没有" + webObj.CurrentAction + item.Text;
                    break;
                }
                else
                {
                    if (myItem[item.Text] == null || myItem[item.Text].ToString() == "")
                    {
                        if (item.Text == webObj.AttachmentField)//检查附件
                        {
                            result = myItem.Attachments .Count >0?true:false ; // CheckAttachment(ID);
                            if (!result )
                            {
                                lblMsg.Text = item.Text + "不能为空！";
                                break;
                            }
                        }
                        else
                        {
                            result = false;
                            lblMsg.Text = item.Text + "不能为空！";
                            break;
                        }
                    }
                    else
                        lblMsg.Text = "";
                }
                i = i + 2;
            }
            return result;
        }
        //如果审核地址相关的，地址为空时，查找附件库
        protected bool  CheckAttachment(string id)
        {
            bool result=false;
            SPListItemCollection docITems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                try
                {
                    using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.Name])
                        {
                            SPList sList = sWeb.Lists[webObj.ListName + "附件库"];
                            SPQuery qry = new SPQuery();
                            qry.Query = @"<Where><Eq><FieldRef Name='ParentTitle' LookupId='True' /><Value Type='Lookup'>" + id + "</Value></Eq></Where>";
                            docITems = sList.GetItems(qry);
                            if (docITems.Count > 0)
                                result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            });
            return result;
        }

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
        /// <summary>
        /// 获取当前要的绩效项，如论文标题
        /// </summary>
        /// <returns></returns>
        private string GetCreativityName( )
        {
            string ID = Page.Request.QueryString["ID"];
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists.TryGetList(webObj.ListName);
            if (list != null)
            {
                SPListItem itme = list.GetItemById(int.Parse(ID));
                if (itme != null)
                {
                     return itme[GetTitleDispName ].ToString();
                }

            }
            return "";
        }
        //审核
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAction"></param>
        /// <param name="flag">1=通过，2=不通过</param>
        /// <returns></returns>
        public bool AddAppraiseRecord(string strAction, int flag ,string state)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);//子表
                        SPList spTList = spWeb.Lists.TryGetList(webObj.ListName);//主表
                        if (spList != null && spTList !=null)
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            string ID = Page.Request.QueryString["ID"];
                            SPListItem listItem;
                            if (ViewState["id"] == null)
                            {
                                listItem = spList.AddItem();
                                listItem[GetTitleDispName] = ID;// +";#" + strPerformance;
                                listItem[GetAuthorDispName] = appraiseUser.ID + ";#" + appraiseUser.Name;//"创建者"
                                listItem[GetDispNameByInternalName(spList, "AuthorName")] = appraiseUser.ID;// +";#" + appraiseUser.Name;//"作者"
                            }

                            else
                            {
                                listItem = spList.Items.GetItemById((int)ViewState["id"]);

                            }

                            listItem[GetDispNameByInternalName(spList, "Action")] = action ;
                            listItem[GetDispNameByInternalName(spList, "State")] = state ;//webObj.ReturnResult;
                            listItem.Update();
                            if (spTList.Fields.ContainsFieldWithStaticName("Flag"))//审核通过，主表加标记
                            {
                                SPListItem parentItem = spTList.GetItemById(int.Parse(ID));
                                parentItem["Flag"] = flag ;
                                parentItem.Update();
                            }

                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
            return true;
        }
        private string GetDispNameByInternalName(SPList list, string internalName)
        {
            SPField field = list.Fields.GetFieldByInternalName(internalName);
            return field.Title;
        }
        //是否已经评审
        /// <summary>
        /// 当前登录者是否已经审批过，只能审批一次
        /// </summary>
        /// <param name="strAction">操作名称</param>
        /// <param name="strCreativity">创意名称</param>
        /// <returns></returns>
        public SPListItem GetAppraiseRecord(string strAction, string strPerformanceID, string strState, ref int flag)
        {

            string siteUrl = SPContext.Current.Site.Url;
            int f = flag;
            SPListItem retItem = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
         {
             using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
             {
                 using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                 {
                     SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);
                     SPList spTList = spWeb.Lists.TryGetList(webObj.ListName);
                     string lookupInternalName = GetSubTitleInterlname.Trim();
                     if (spList != null)
                     {
                         SPQuery qry = new SPQuery();
                         //<And><Eq><FieldRef Name='Author' /> <Value Type='Integer'> <UserID /> </Value></Eq></And>
                         qry.Query = @"<Where><And><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq><And><Eq><FieldRef Name='State' /><Value Type='Choice'>" + strState + "</Value></Eq><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + strPerformanceID + "</Value></Eq></And></And></Where>";

                         SPListItemCollection listItems = spList.GetItems(qry);
                         if (listItems.Count > 0)
                         {
                             //ViewState["id"] = listItems[0]["ID"];
                             SPListItem parentItem = spTList.GetItemById(int.Parse(strPerformanceID));
                             if (strState == webObj.ReturnResult)
                             {
                                 if (spTList != null && spTList.Fields.ContainsFieldWithStaticName("Flag"))//审核通过，主表加标记
                                 {
                                     if (parentItem["Flag"] == null || parentItem["Flag"].ToString() != "1")
                                     {
                                         spWeb.AllowUnsafeUpdates = true;
                                         parentItem["Flag"] = 1;
                                         parentItem.Update();
                                         spWeb.AllowUnsafeUpdates = false;
                                     }
                                 }
                             }
                             f = int.Parse(parentItem["Flag"].ToString());
                             retItem = listItems[0];
                         }
                     }
                 }
             }
         });
            flag = f;
            return retItem;
        }
     
        //根据当前时间判断当前进行到什么审批阶段，如果为空则当前没有进行审批
        //根据操作判断时间
        public bool  GetAppraiseAction(string action)
        {
            string siteUrl = SPContext.Current.Site.Url;
            bool retAction =false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                        {
                            using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                            {
                                SPList spList = spWeb.Lists.TryGetList(webObj.TimeList);
                                if (spList != null)
                                {
                                    SPQuery qry = new SPQuery();
                                    qry.Query = @"<Where> <Eq><FieldRef Name='Title' /><Value Type='Text'>" + action + "</Value></Eq></Where>";
                                    SPListItemCollection listItems = spList.GetItems(qry);
                                    if (listItems.Count > 0)
                                    {
                                        if (DateTime.Now.Date >= (DateTime)listItems[0]["StartDate"] && DateTime.Now.Date <= (DateTime)listItems[0]["EndDate"])
                                            retAction = true;
                                    }

                                    //<Leq>
                                    //   <FieldRef Name='StartDate' />
                                    //   <Value Type='DateTime'>
                                    //      <Today />
                                    //   </Value>
                                    //</Leq>
                                    //<Gt>
                                    //   <FieldRef Name='EndDate' />
                                    //   <Value Type='DateTime'>
                                    //      <Today />
                                    //   </Value>
                                    //</Gt>

                                }
                            }
                        }
                        
                    });
            return retAction;
        }
        #endregion
    }
}
