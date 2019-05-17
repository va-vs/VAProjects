using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LeaveApproval.Appraval
{
    public partial class AppravalUserControl : UserControl
    {
        public Appraval  webObj { get; set; }
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SPContext.Current.Web.CurrentUser != null)
                {
                  
                        SPUser user = SPContext.Current.Web.CurrentUser;
                        bool hasRight = UserHaveApproveRight(SPContext.Current.Site.ID, SPContext.Current.Web.Name, webObj.ListName, user);
                        if (!hasRight)
                        {
                            AppraiseDiv.Visible = false;//什么也不显示
                            //lblMsg.Text = "no has right";
                            return;
                        }
                        AppAction.Visible = true;
                        string ID = Page.Request.QueryString["ID"];
                        try
                        {
                            int flag = -1;
                            string action = webObj.CurrentAction;
                            SPListItem item= GetAppraiseApply( ID,  ref flag);
                            SPListItem subItem=GetLeaveApprove (ID.ToString (),item["报销类别"].ToString ());
                            if (flag == 1)
                            {
                                if (subItem != null)//
                                {
                                    btnAppraise.Enabled = false;
                                    txtAppraise.Text = subItem["Opinion"].ToString();
                                    txtAppraise.ReadOnly = true;
                                    lblMsg.Text = "此" + webObj.ListName + "已经" + webObj.CurrentAction;
                                    btnNoPass.Enabled = false;
                                    //ddlResult.SelectedItem.Text = item["结果"].ToString(); 
                                }
                                else
                                {
                                    this.Visible = false;
                                    AppraiseDiv.Visible = false;//什么也不显示
                                }
                                return;

                            }
                            else if (flag == 0)
                            {

                                btnAppraise.Enabled = false;
                                btnNoPass.Enabled = false;
                                txtAppraise.Text = subItem["Opinion"].ToString();
                                txtAppraise.ReadOnly = true;

                                lblMsg.Text = "此" + webObj.ListName + webObj.CurrentAction + "已拒绝";
                                return;
                            }
                            else if (flag == -1)
                            {
                                if (subItem != null)
                                {
                                    btnAppraise.Enabled = false;
                                    btnNoPass.Enabled = false;
                                    txtAppraise.Text = subItem["Opinion"].ToString();
                                    txtAppraise.ReadOnly = true;

                                    lblMsg.Text = "此" + webObj.ListName + webObj.CurrentAction + subItem["Result"].ToString();
                                }
                                else
                                {
                                    this.Visible = false;
                                    AppraiseDiv.Visible = false;//什么也不显示

                                }
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            lblMsg.Text = ex.ToString();
                        }
                        this.lblOptionTitle.Text = webObj.BiaoXiaoOptionTitle;
                }
                else
                {
                    AppraiseDiv.Visible = false;
                }
            }
            btnAppraise.Click += btnAppraise_Click;
            btnCancle.Click += btnCancle_Click;
            btnNoPass.Click += btnNoPass_Click;
        }
        /// <summary>
        /// 通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAppraise_Click(object sender, EventArgs e)
        {
            string action = "审批";
            bool result = AddAppraiseRecord(action, 1, webObj.ReturnResult);
            if (!result)
                lblMsg.Text = webObj.CurrentAction + " 失败";
            else
                ReturnLast();

        }
        void btnNoPass_Click(object sender, EventArgs e)
        {
            string action = "审批";
            bool result = AddAppraiseRecord(action,0, "拒绝");
            if (!result)
                lblMsg.Text = webObj.CurrentAction + " 失败";
            else
                ReturnLast();
        }
        void btnCancle_Click(object sender, EventArgs e)
        {
            ReturnLast();
        }
        #endregion
        #region 方法
        //获取审批子表的数据
        private SPListItem GetLeaveApprove(string leaveID, string bxType)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPListItem item = null;
            string duty = GetUserDuty;
            int f = -1;
            switch (duty)
            {
                case "系部领导":
                    {
                        f = 1;
                        break;
                    }
                case "分管教学领导":
                    if (bxType == "教学")
                        f = 2;
                    break;
                case "分管科研领导":
                    {
                        if (bxType == "科研")
                            f = 2;
                        break;
                    }
                case "人事领导":
                    {
                        f = 3;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            if (f == -1) return null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(webObj.ResultList);//子表

                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            qry.Query = @"<Where><And> <Or><Eq>
               <FieldRef Name='Flag' />
               <Value Type='Number'>" + (f * 10).ToString() + "</Value></Eq> <Eq> <FieldRef Name='Flag' /><Value Type='Number'>" + (f * 10 + 1).ToString() + "</Value></Eq></Or><Eq><FieldRef Name='LeaveID' LookupId='True' /><Value Type='Lookup'>" + leaveID + "</Value></Eq></And></Where><OrderBy><FieldRef Name='Created' Ascending='FALSE' /></OrderBy>";

                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                item = listItems[0];
                        }
                    }
                }
            });
            return item;
        }
        //是否已经评审
        /// <summary>
        /// ret=1审核通过，=0审核不通过 =-1不进行任何操作 2进行审核操作
        /// </summary>
        /// <param name="strAction">操作名称</param>
        /// <param name="strCreativity">创意名称</param>
        /// <returns></returns>
        public SPListItem GetAppraiseApply( string strPerformanceID, ref int ret)
        {

            string siteUrl = SPContext.Current.Site.Url;
            int flag = -1;
            int approveType = -1;
            string bxType="";
            bool bx=false ;
            SPListItem retItem = null;
            string dept="";
            SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                 {
                     using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         SPList spTList = spWeb.Lists.TryGetList(webObj.ListName);

                         retItem = spTList.GetItemById(int.Parse(strPerformanceID));
                         if (retItem["Flag"] != null)
                             flag = int.Parse(retItem["Flag"].ToString());
                         else
                             flag = 0;
                         approveType =int.Parse (retItem["ApproveType"].ToString ());
                         bxType = retItem["报销类别"].ToString();
                         dept =retItem["Dept"].ToString();
                         bx = retItem["Reimburse"] != null ? (bool)retItem["Reimburse"] : false; 
                     }
                 }
             });
             string duty = GetUserDuty;
             switch (duty)
             {
                 case "系部领导":
                     {
                         List<string> userInfo = GetUserInfo(SPContext.Current.Web.CurrentUser.LoginName);
                         if (approveType < 2 && flag == 0 && userInfo[0] == dept)//可以审核,只审当前部门下的请假申请
                             ret = 2;
                         else if (flag == 11)//pass
                             ret = 1;
                         else if (flag == 10)//reject
                             ret = 0;
                         else //no
                             ret = -1;

                         break;
                     }
                 case "分管教学领导":
                     if (flag == 21)
                         ret = 1;
                     else if (flag == 20)
                         ret = 0;
                     else if (flag == 11 && approveType != 0 && (!bx || bxType == "教学"))
                         ret = 2;
                     else
                         ret = -1;
                     break;
                 case "分管科研领导":
                     {
                         if (flag == 21)
                             ret = 1;
                         else if (flag == 20)
                             ret = 0;
                         else if (flag == 11 && approveType != 0 && bxType == "科研" && bx)
                             ret = 2;
                         else
                             ret = -1;
                         break;
                     }
                 case "人事领导":
                     {
                         if (flag == 31)
                             ret = 1;
                         else if (flag == 30)
                             ret = 0;
                         else if (flag == 21 || flag == 11 && approveType == 0)
                             ret = 2;
                         else
                             ret = -1;
                         break;
                     }

                 default:
                     {
                         break;
                     }
             }
            return retItem;
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
        public bool AddAppraiseRecord(string strAction, int flag, string state)
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
                        if (spList != null && spTList != null)
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            string ID = Page.Request.QueryString["ID"];
                            SPListItem listItem;
                            listItem = spList.AddItem();
                            listItem["LeaveID"] = ID;// +";#" + strPerformance;
                            listItem["Opinion"] = txtAppraise.Text;
                            listItem["Action"] = strAction;
                            listItem["Result"] = state;
                            int f = 0;
                            if (spTList.Fields.ContainsFieldWithStaticName("Flag"))//审核通过，主表加标记
                            {
                                SPListItem parentItem = spTList.GetItemById(int.Parse(ID));
                                int appType = int.Parse(parentItem["ApproveType"].ToString());
                                if (parentItem["Flag"] != null && parentItem["Flag"].ToString().Length == 2)
                                    f = int.Parse(parentItem["Flag"].ToString().Substring(0, 1)) + 1;
                                else
                                    f = 1;
                                parentItem["Flag"] = f * 10 + flag;//十位表示审批类型，个位表示审批结果
                                parentItem.Update();
                            }
                            listItem["Flag"] = f * 10 + flag;//第几级审

                            listItem.Update();
                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
            return true;
        }
        //获取当前用户的帐号
        public string GetAccount(string loginName)
        {
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        /// <summary>
        /// 获取具有审核权限用户的信息
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="lgName"></param>
        /// <returns></returns>
        private string GetUserDuty
        {
            get
            {
                if (ViewState["userDuty"] == null)
                {
                    string lgName = SPContext.Current.Web.CurrentUser.LoginName;
                    lgName = GetAccount(lgName);
                    string listTeachers = webObj.TeacherList;
                    string siteUrl = SPContext.Current.Site.Url;
                    SPUser appraiseUser = SPContext.Current.Web.CurrentUser;
                    SPListItemCollection retItems = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                        {
                            using (SPWeb spWeb = spSite.AllWebs [webObj.TeacherListWebName ])
                            {
                                SPQuery qry = new SPQuery();
                                SPListItemCollection listItems;

                                #region teachers
                                SPList spTeacherList = spWeb.Lists.TryGetList(listTeachers);

                                qry = new SPQuery();
                                qry.Query = @"<Where><Eq><FieldRef Name='EmpNO' /><Value Type='Text'>" + lgName + "</Value></Eq></Where>";
                                listItems = spTeacherList.GetItems(qry);
                                if (listItems.Count > 0)//获取系部下的教师
                                {
                                    retItems = listItems;
                                }
                                #endregion
                            }
                        }
                    });
                    if (retItems != null)
                    {
                        ViewState["userDuty"] = retItems[0]["Duty"] != null ? retItems[0]["Duty"].ToString() : "";
                    }
                }
                return ViewState["userDuty"]==null?"":ViewState["userDuty"].ToString ();
            }
        }
        
        private List<string> GetUserInfo(  string lgName)
        {
            lgName = GetAccount(lgName);
            string listTeachers = "教师花名册";
            string siteUrl = SPContext.Current.Site.Url ;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webObj.TeacherListWebName))
                    {
                        SPQuery qry = new SPQuery();
                        SPListItemCollection listItems;

                        #region teachers
                        SPList spTeacherList = spWeb.Lists.TryGetList(listTeachers);

                        qry = new SPQuery();
                        qry.Query = @"<Where><Eq><FieldRef Name='EmpNO' /><Value Type='Text'>" + lgName + "</Value></Eq></Where>";
                        listItems = spTeacherList.GetItems(qry);
                        if (listItems.Count > 0)//获取系部下的教师
                        {
                            retItems = listItems;
                        }
                        #endregion
                    }
                }
            });
            List<string> userIDs = new List<string>();
            if (retItems != null)
            {
                foreach (SPListItem item in retItems)
                {
                    string dep = item["Department"].ToString();
                    userIDs.Add(dep.Substring(dep.IndexOf(";#") + 2));//查阅项需要解析
                    userIDs.Add(item["性别"] != null ? item["性别"].ToString() : "");
                    userIDs.Add(item["Duty"] != null ? item["Duty"].ToString() : "");
                }

            }
            return userIDs;
        }
        //返回的页面
        private void ReturnLast()
        {
            string returnUrl = webObj.RetUrl;
            if (Page.Request.QueryString["Source"] != null)
                returnUrl = Page.Request.QueryString["Source"];

            Response.Redirect(returnUrl);

        }
        #endregion
    }
}
