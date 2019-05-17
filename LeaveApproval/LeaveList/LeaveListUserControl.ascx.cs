using Microsoft.SharePoint;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace LeaveApproval.LeaveList
{
    public partial class LeaveListUserControl : UserControl
    {
        public LeaveList WebPartObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (SPContext.Current.Web.CurrentUser != null)
                {
                    lvList.Visible = true;
                    SPUser user = SPContext.Current.Web.CurrentUser;
                    ReadItems(WebPartObj.WPFeature);
                }
                else
                {
                    lvList.Visible = false;
                }
            }
        }

        /// <summary>
        /// 获取当前用户账户（不包含AD）
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public string GetAccount()
        {
            string loginName = SPContext.Current.Web.CurrentUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        /// <summary>
        /// 在教师花名册中查找当前用户的信息
        /// </summary>
        /// 角色与部门
        /// <returns></returns>
        private string[] GetUserInfo()
        {
            string[] userInfo = new string[2];
            string listTeachers = WebPartObj.UserList;
            string siteUrl = SPContext.Current.Site.Url;
            string userListWeb = WebPartObj.UserSite != "" ? WebPartObj.UserSite : "";
            if (WebPartObj.UserSite!="")
            {
                siteUrl = WebPartObj.UserSite;
            }
            //SPUser currentUser = SPContext.Current.Web.CurrentUser;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb (userListWeb))//花名册所在网站
                    {
                        SPQuery qry = new SPQuery();
                        SPListItemCollection listItems;

                        #region teachers
                        SPList spTeacherList = spWeb.Lists.TryGetList(listTeachers);

                        qry = new SPQuery();
                        qry.Query = @"<Where><Eq><FieldRef Name='EmpNO' /><Value Type='Text'>" + GetAccount() + "</Value></Eq></Where>";
                        listItems = spTeacherList.GetItems(qry);
                        if (listItems.Count > 0)//获取指定工号的教师信息
                        {
                            retItems = listItems;
                        }
                        #endregion
                    }
                }
            });
            if (retItems != null)
            {
                userInfo[0] = retItems[0]["Duty"] != null ? retItems[0]["Duty"].ToString() : "教师";
                string dep = retItems[0]["Department"] != null ? retItems[0]["Department"].ToString() : "";
                userInfo[1] = dep != "" ? dep.Substring(dep.IndexOf(";#") + 2) : "";
            }
            return userInfo;
        }
        /// <summary>
        /// 获取当前用户的身份
        /// 1:普通教师;
        /// 2:系部负责人;
        /// 3:主管教学领导
        /// 4:主管科研领导
        /// 5:主管人事领导
        /// </summary>
        protected string GetQueryStr()
        {
            string userList =WebPartObj.UserList!=""? WebPartObj.UserList : "教师花名册";
            string userListWeb = WebPartObj.UserSite != "" ? WebPartObj.UserSite : "";
            string queryStr = "";
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb spWeb = site.OpenWeb(userListWeb))
                    {
                        SPList spList = spWeb.Lists.TryGetList(userList);
                        string userName = GetAccount();
                        if (spList != null)
                        {
                            string[] userInfo = GetUserInfo();
                            switch (userInfo[0])//教师身份
                            {
                                case "系部领导":
                                    string dept = userInfo[1];
                                    //queryStr = "<Where><And><Eq><FieldRef Name='Dept'/><Value Type='Text'>" + dept + "</Value></Eq><Eq><FieldRef Name ='State'/><Value Type='Text'>系部负责人待审批</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    //queryStr = "<Where><And><And><Or><Eq><FieldRef Name='Flag'/><Value Type='Number'>0</Value></Eq><IsNull><FieldRef Name='Flag'/></IsNull></Or><Lt><FieldRef Name='ApproveType'/><Value Type='Number'>2</Value></Lt></And>";
                                    //queryStr += "<Eq>< FieldRef Name = 'Dept'/><Value Type = 'Text'>" + dept + "</Value></Eq></And></Where>";
                                    //queryStr += "<OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    queryStr=@"<Where><And><And><Or><Eq><FieldRef Name='Flag' /><Value Type='Number'>0</Value></Eq><IsNull><FieldRef Name='Flag' /></IsNull></Or><Lt><FieldRef Name='ApproveType' /><Value Type='Number'>2</Value></Lt></And><Eq><FieldRef Name='Dept' /><Value Type = 'Text'>" + dept + "</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    break;
                                case "分管教学领导":
                                    queryStr = "<Where><And><Or><Eq><FieldRef Name='Reimburse' /><Value Type='Boolean'>0</Value></Eq><Eq><FieldRef Name='ReimburseType' /><Value Type='Choice'>教学</Value></Eq></Or><Eq><FieldRef Name='Flag' /><Value Type='Number'>11</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    break;
                                case "分管科研领导":
                                    queryStr = "<Where><And><And><Or><Eq><FieldRef Name='ApproveType'/><Value Type='Number'>1</Value></Eq><Eq><FieldRef Name='ApproveType'/><Value Type='Number'>2</Value></Eq></Or><Neq><FieldRef Name='ReimburseType'/><Value Type='Choice'>科研</Value></Neq></And><Eq><FieldRef Name='Flag'/><Value Type='Number'>11</Value></Eq></And></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    break;
                                case "人事领导":
                                    queryStr = "<Where><Or><And><Eq><FieldRef Name='Flag'/><Value Type='Number'>11</Value></Eq><Eq><FieldRef Name='ApproveType'/><Value Type='Number'>0</Value></Eq></And><Eq><FieldRef Name='Flag'/><Value Type='Number'>21</Value></Eq></Or></Where><OrderBy><FieldRef Name='StartDate'/></OrderBy>";
                                    break;
                                default://教师
                                    queryStr = "";
                                    break;
                            }

                        }
                    }
                }
            });
            return queryStr;
        }

        /// <summary>
        /// 查询当前用户待审的请假记录，按部件功能显示数据
        /// </summary>
        /// <param name="userList">教师花名册</param>
        /// <param name="wpFeature">部件功能：待审提醒or待审列表</param>
        public void ReadItems(string wpFeature)
        {
            string webUrl = WebPartObj.SiteUrl!=""? WebPartObj.SiteUrl:"";
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.Url)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(webUrl))
                    {
                        string lvList = WebPartObj.ListName;
                        SPList spList = spWeb.Lists.TryGetList(lvList);
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();

                            string queryStr = GetQueryStr();//根据当前用户的身份选择不同的查询语句SPQuery
                            if (queryStr == "")//没有请假审批权的用户,隐藏Web部件显示
                            {
                                divData.Visible = false;
                                pagediv.Visible = false;
                                wbtitle.Visible = false;
                            }
                            else
                            {
                                qry.Query = queryStr;
                                SPListItemCollection listItems = spList.GetItems(qry);

                                if (listItems.Count > 0)//具有待审的请假记录
                                {
                                    if (wpFeature == "待办")//部件作为待办提醒使用
                                    {
                                        wbtitle.InnerHtml = "待批复请假提醒";
                                        wbtitle.Visible = true;
                                        string approveUrl = WebPartObj.ApproveUrl;
                                        divData.InnerHtml = "<span style='color:red;'>您有 <b><a href='" + approveUrl + "'>" + listItems.Count + "</a></b> 个请假申请批复!</span>";
                                        pagediv.Visible = false;
                                    }
                                    else//部件作为待审列表使用
                                    {
                                        pagediv.Visible = true;
                                        wbtitle.Visible = true;
                                        StringBuilder sb = new StringBuilder();
                                        sb.Append("<table  width='100 % ' id='ListArea' border='0' class='t1' cellpadding='0' cellspacing='0'>");
                                        sb.Append("<tr style='font-weight:600;font-size:14px;'><th>标题</th><th>请假人</th><th>申请时间</th></tr>");
                                        foreach (SPListItem item in listItems)
                                        {
                                            string displayUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/15/CopyUtil.aspx?Use=id&Action=dispform";
                                            //SPSite currentSite = SPContext.Current.Site;
                                            //SPWeb currentWeb = currentSite.OpenWeb();
                                            displayUrl = displayUrl + "&ItemId=" + item.ID + "&ListId=" + spList.ID + "&WebId=" + spWeb.ID + "&SiteId=" + spSite.ID + "&Source=" + UrlEncode(Request.Url.ToString());

                                            sb.Append("<tr>");
                                            sb.Append("<td><a href='" + displayUrl + "'>" + item.Title + "</a></td>");
                                            string temp = item["请假人"].ToString();
                                            temp = temp.Substring(temp.IndexOf("#") + 1);
                                            sb.Append("<td>" + temp + "</td>");
                                            sb.Append("<td>" + string.Format("{0:yyyy/MM/dd}", item["创建时间"]) + "</td>");

                                            sb.Append("</tr>");

                                        }
                                        sb.Append("</table>");
                                        divData.InnerHtml = sb.ToString();
                                    }
                                }
                                else//没有待审的请假记录
                                {
                                    divData.InnerHtml = "没有新的请假待审批！<br/>";
                                    pagediv.Visible = false;
                                    wbtitle.Visible = true;
                                }
                            }
                        }
                    }
                }
            });
        }

        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }
}
