using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Collections.Generic;

namespace LeaveEvent.WriteInfo
{
    /// <summary>
    /// 列表项事件
    /// </summary>
    public class WriteInfo : SPItemEventReceiver
    {
        /// <summary>
        /// 正在添加项.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
        }

        /// <summary>
        /// 正在更新项.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
        }

        /// <summary>
        /// 正在删除项.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);
        }

        /// <summary>
        /// 已添加项.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            if (properties.List.Title == "请假申请")
                WriteLeaveInfo(properties);
        }

        /// <summary>
        /// 已更新项.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            if (properties.List.Title == "请假申请")
                WriteLeaveInfo(properties);
        }

        /// <summary>
        /// 已删除项.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);
        }
        #region 方法
        #region 获取系部教师
        //获取当前用户的帐号
        public  string GetAccount(string loginName)
        {
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        private void WriteLeaveInfo(SPItemEventProperties properties  )
        {
            string lgName = properties.Web.CurrentUser.LoginName;
            if (properties.ListItem["Author"]!=null )
            {
                SPFieldUserValue fUser = new SPFieldUserValue(properties.Web, properties.ListItem["Author"].ToString ());
                lgName = fUser.User.LoginName;
            }
                
            lgName = GetAccount(lgName);
            List<string> userInfo = GetUserInfo(properties,lgName );
            if (userInfo.Count >0)
            {
                SPListItem item = properties.ListItem;
                item["Dept"] = userInfo[0];
                item["Sex"] = userInfo[1];
                
                bool opt= item["是否报销"]!=null?(bool)item["是否报销"]:false ;
                string duty = userInfo[2];
                string during = item["During"].ToString();
                int start = during.IndexOf(";#") + 2;
                int end = during.IndexOf(".");
                int days = int.Parse(during.Substring(start,end-start ));//计算列需要解析
                switch (duty)
                {
                    case "系部领导":
                        {
                            item["ApproveType"] = 2;
                            if (item["Flag"] == null || item["Flag"].ToString() == "0")//系部负责人请假，分管领导和人事领导审批
                                item["Flag"] = 11;
                            break;
                        }
                    case "分管教学领导":
                    case "分管科研领导":
                    case "人事领导":
                        {
                            item["ApproveType"] = 3;
                            break;
                        }

                    default:
                        {
                            if (days <= 3 && !opt )
                                item["ApproveType"] = 0;
                            else if (days <= 15)
                                item["ApproveType"] = 1;
                            else
                                item["ApproveType"] = 2;
                            break;
                        }
                }
                //item["State"] = GetApprovalStateByFlag(properties.ListItem["Flag"]);
                item.Update();
                    
            }

        }

        /// <summary>
        /// 根据Flag判断审批状态
        /// </summary>
        /// <param name="Flag">审批标记</param>
        /// <returns></returns>
        private string GetApprovalStateByFlag(object Flag)
        {
            string appState = "等待审批中";
            if (Flag!=null)
            {
                int flag = (int)Flag;
                switch (flag)
                {
                    case 10:
                        appState = "系部负责人已拒绝";
                        break;
                    case 11:
                        appState = "系部负责人已同意，等待下一级审批";
                        break;
                    case 20:
                        appState = "分管领导已拒绝";
                        break;
                    case 21:
                        appState = "分管领导已同意，等待下一级审批";
                        break;
                    case 30:
                        appState = "人事领导已拒绝";
                        break;
                    case 31:
                        appState = "人事领导已同意，可以打印请假审批表";
                        break;
                    default:
                        appState = "等待审批中";
                        break;
                }
            }            
            return appState;
        }

        /// <summary>
        /// 获取当前用户的信息
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="lgName"></param>
        /// <returns></returns>
        private List<string> GetUserInfo(SPItemEventProperties properties,string lgName)
        {
            //string listName = properties.ListTitle;
            string listTeachers ="教师花名册";
            string siteUrl = properties.Site.Url ;
            SPListItemCollection retItems = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(""))
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
                    userIDs.Add(dep.Substring (dep.IndexOf (";#")+2));//查阅项需要解析
                    userIDs.Add(item["性别"] != null ? item["性别"].ToString() : "");
                    userIDs.Add(item["Duty"] != null ? item["Duty"].ToString() : "");
                }

            }
            return userIDs;
        }
        #endregion
        #endregion

    }
}