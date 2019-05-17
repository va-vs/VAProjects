using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Collections.Generic ;

namespace PerformanceList.PerformList
{
    /// <summary>
    /// 列表项事件
    /// </summary>
    public class PerformList : SPItemEventReceiver
    {
#region 事件
        /// <summary>
        /// 正在添加项.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
            if (!hasChildList(properties))
            {
                return;
            }
            //论文是否重复
            bool result = CheckAdUser(properties);
            if (!result)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = GetDispNameByInternalName(properties.List, "AuthorName") + "个数超限，请重新输入";
            }
        }

        /// <summary>
        /// 正在更新项.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
            if (!hasChildList(properties))
            {
                return;
            }
            SPUser loginUser = properties.Web.CurrentUser;
            SPListItemCollection  item = GetAppraiseRecord("审核", properties,0,"通过") ;

            bool hasRight = UserHaveApproveRight(properties.SiteId, properties.Web.Name, properties.List.Title, loginUser);//具有网站级审批权限的用户保留删除的权限
            if (item != null && !hasRight)//审批通过
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "该记录已通过审核,您无权修改!";
            }
            flag = 0;
           if (properties.List.Fields.ContainsField ("Flag"))
           {
               if (properties.ListItem["Flag"] == null && properties.AfterProperties["Flag"] != null || properties.AfterProperties["Flag"]!=null && properties.AfterProperties["Flag"].ToString() != properties.ListItem["Flag"].ToString())//审核事件
               {
                   //properties.Status = SPEventReceiverStatus.CancelNoError;
                   flag = 1;
                    return;
               }
           }
            //检查论文是否重复

            bool result = CheckAdUser(properties);
            if (!result)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = GetDispNameByInternalName(properties.List, "AuthorName") + "个数超限，请重新输入";
            }
        }

        /// <summary>
        /// 正在删除项.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);
            if (!hasChildList(properties))
            {
                return;
            }
            SPUser loginUser = properties.Web.CurrentUser;
            SPListItemCollection item = GetAppraiseRecord("审核",properties,0,"通过");

            bool hasRight = UserHaveApproveRight(properties.SiteId, properties.Web.Name, properties.List.Title, loginUser);//具有网站级审批权限的用户保留删除的权限
            if (item != null && !hasRight)//审批通过
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "该记录已通过审核,您无权删除!";
            }
        }

        /// <summary>
        /// 已添加项.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            if (!hasChildList(properties))
            {
                return;
            }
            //UpdateADUser(properties);
            this.EventFiringEnabled = false;//否则会死循环
            SavePerform(properties,0);
            this.EventFiringEnabled = true;
        }
        //根据AAD用户写入业绩子表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="opetype">0=new 1-edit</param>
        private void SavePerform(SPItemEventProperties properties,int opetype=1)
        {
            string adUserDispName = GetDispNameByInternalName(properties.List, "AuthorName");
             SPListItem splItem = properties.ListItem;
             List<int> athors = new List<int>();
             if (splItem[adUserDispName] != null)
             {
                 SPFieldUserValueCollection adUsers = splItem[adUserDispName] as SPFieldUserValueCollection;
                 foreach (SPFieldUserValue adUser in adUsers)
                 {
                     SPListItemCollection item = GetAppraiseRecord("录入", properties, adUser.LookupId, "待定");
                     if (item == null)
                     {
                         AddPerformRecord(properties, "录入", "", adUser.User);
                     }
                     athors.Add(adUser.User.ID);
                 }

             }
             if (opetype == 1)
             {
                 SPListItemCollection items = GetAppraiseRecord("录入", properties,0,"待定");//获取所有录入子表
                 if (items != null)
                 {
                     for (int i = items.Count - 1; i >= 0; i--)
                     {
                         SPFieldUserValue author = new SPFieldUserValue(properties.Web, items[i]["AuthorName"].ToString());
                         if (!athors.Contains(author.LookupId))
                             items[i].Delete();

                     }

                 }
                 if (splItem["Flag"].ToString() == "2")//审核不通过的修改后重新待审
                 {
                     splItem["Flag"] = 0;
                     splItem.Update();
                 }
             }
         
        }
        private static  int flag;
        public bool AddPerformRecord(SPItemEventProperties properties, string strAction, string strPerformance, SPUser author)
        {
            string siteUrl = properties.Site.Url;
            SPUser lgUser = properties.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                    {
                        string childListName = properties.List.Title + "业绩";
                        SPList spList = spWeb.Lists.TryGetList(childListName);
                        if (spList != null)
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            SPListItem listItem = spList.AddItem();
                            string lookupInternalName = GetSubTitleInterlname(properties).Trim();
                            listItem[lookupInternalName] = properties.ListItemId;// +";#" + strPerformance;
                            listItem[GetDispNameByInternalName(spList, "Author")] = lgUser.ID + ";#" + lgUser.Name;//"创建者"
                            listItem[GetDispNameByInternalName(spList, "AuthorName")] = author.ID + ";#" + author.Name;//"创建者"
                            listItem[GetDispNameByInternalName(spList, "Action") ]= strAction;
                            listItem[GetDispNameByInternalName(spList, "State")] = "待定";
                            this.EventFiringEnabled = false;//否则会死循环
                            listItem.Update();
                            this.EventFiringEnabled = true;
                            spWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
            return true;
        }
        //判断是否存在，不能重复添加，标题和作者作为论文的唯一性,=-1 作者为空 =-0重复 =1允许添加
        private int CheckAllowNew(SPItemEventProperties properties)
        {
            string authorsDispName = GetDispNameByInternalName(properties.List, "Authors");
            SPListItem splItem = properties.ListItem;
            int ret = 1;
            if (splItem[authorsDispName] != null && splItem[authorsDispName].ToString() != "" || splItem[authorsDispName].ToString() == "")
            {
                if (splItem[authorsDispName].ToString() == "")
                {
                    ret=-1;
                }
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = new SPSite(properties.Site.ID  )) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                        {
                            SPList spList = spWeb.Lists.TryGetList(properties.ListTitle);
                            string lookupInternalName = GetSubTitleInterlname(properties).Trim();
                            string strPerformanceID = properties.ListItemId.ToString();
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = @"<Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + properties.ListItem.Title + "</Value></Eq><Eq><FieldRef Name='Authors' /><Value Type='Text'>" + strPerformanceID + "</Value></Eq></And></Where>";
                                SPListItemCollection listItems = spList.GetItems(qry);
                                if (listItems.Count > 0)
                                   ret =0;
                            }
                        }
                    }
                });
            }
            return ret ;
        }
       
        //ad用户个数 不能超过字符串中用户数
        private bool CheckAdUser(SPItemEventProperties properties)
        {
            string adUserDispName = "AuthorName";// GetDispNameByInternalName(properties.List, "AuthorName");
            SPListItem splItem = properties.ListItem;
            SPItemEventDataCollection afterData = properties.AfterProperties;
            if (afterData[adUserDispName] != null && afterData[adUserDispName].ToString()!="")
            {
                //SPFieldUserValueCollection adUsers = afterData[adUserDispName] as SPFieldUserValueCollection;//此处为空值
                int adCount = GetAdUserCount (afterData[adUserDispName] .ToString ().ToLower());
                string authorsDispName = "Authors";// GetDispNameByInternalName(properties.List, "Authors");
                if (afterData[authorsDispName] != null && afterData[authorsDispName].ToString() != "" || afterData[authorsDispName].ToString() == "")
                {
                    if (afterData[authorsDispName].ToString() == "")
                    {
                        return false;
                    }
                    string txtAuthors = afterData[authorsDispName].ToString().Trim().Replace("；", ";");
                    txtAuthors = txtAuthors.TrimEnd(';');
                    string[] users = Regex.Split(txtAuthors, ";");
                    if (users.Length < adCount)//用户个数小于Ad个数
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;
        }
        //在adding和Updating中，取新值时，用户值为字符串，需要解析
//        "31;#i:0#.w|ccc\\zhaowen;#11;#i:0#.w|ccc\\wangboran"
//SHAREPOINT\system
        private int GetAdUserCount(string users)
        {
            int i = 0;
            int c=0;
            while  (i>-1)
            {
                i = users.IndexOf("ccc\\",i>-1?i+5:i) ;
                if (i > -1)
                    c = c + 1;
            }
            if (users.IndexOf("\\system",0) > -1)//系统用户
                c = c + 1;
            return c;
        }
        //字符串字段另保持到用户组
        private void UpdateADUser(SPItemEventProperties properties)
        {
            string authorsDispName = GetDispNameByInternalName(properties.List, "Authors");
            SPListItem splItem = properties.ListItem;
            if (splItem[authorsDispName] != null && splItem[authorsDispName].ToString() != "")
            {
                string authors = splItem[authorsDispName].ToString().Trim() .Replace("；", ";").TrimEnd(';');
                SPFieldUserValueCollection users = GetAuthorsFromString(authors, properties.Web);
                splItem["AD用户"] = users;
                splItem.SystemUpdate();
            }
        }
        /// <summary>
        /// 已更新项.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            if (!hasChildList(properties))
            {
                return;
            }
            if (flag ==1 )
            {
                return;
            }
            this.EventFiringEnabled = false;//否则会死循环
            //UpdateADUser(properties);
            SavePerform(properties,1);
            this.EventFiringEnabled = true;
        }

        /// <summary>
        /// 已删除项.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);
            if (!hasChildList(properties))
            {
                return;
            }
        }
#endregion 
        #region 方法
        private SPFieldUserValueCollection GetAuthorsFromString(string txtAuthors,SPWeb myWeb)
        {
            txtAuthors = txtAuthors.TrimEnd(';');
            string[] authors = Regex.Split(txtAuthors, ";");
            SPFieldUserValueCollection author1 = new SPFieldUserValueCollection();//创建者为第一作者
            for (int i = 0; i < authors.Length; i++)
            {
                DirectoryEntry adUser = GetDirectoryEntryByName(authors[i].Trim());
                if (adUser != null)
                {
                    string account = adUser.Properties["sAMAccountName"].Value.ToString();
                    SPUser user = myWeb.EnsureUser("ccc\\" + account);
                    SPFieldUserValue fUser = new SPFieldUserValue(myWeb, user.ID, user.LoginName);
                    author1.Add(fUser);
                }
            }
            return author1;
        }
        /// <summary>
        /// 判断当前用户是否有审批级别的权限
        /// </summary>
        /// <param name="siteID"></param>
        /// <param name="webName"></param>
        /// <param name="lstName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
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
        /// 当前数据是否已经评审,需要提升权限，否则得不到数据
        /// </summary>
        /// <param name="strAction">操作</param>
        /// <param name="properties">operType =0是否审核，>0查找用户业绩</param>
        /// <returns></returns>
        public SPListItemCollection GetAppraiseRecord(string strAction,SPItemEventProperties properties, int userID = 0, string strState="")
        {
            string siteUrl = properties.Site.Url;// SPContext.Current.Site.Url;
            SPListItemCollection retItem = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                    {
                        string childListName = properties.List.Title + "业绩";
                        SPList spList = spWeb.Lists.TryGetList(childListName);
                        string lookupInternalName = GetSubTitleInterlname(properties).Trim();
                        string strPerformanceID = properties.ListItemId.ToString();
                        if (spList != null)
                        {
                            SPQuery qry = new SPQuery();
                            if (userID  == 0)
                                qry.Query = @"<Where><And><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq><And><Eq><FieldRef Name='State' /><Value Type='Choice'>" + strState + "</Value></Eq><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + strPerformanceID + "</Value></Eq></And></And></Where>";
                            else//是否已经录入业绩
                                qry.Query = @"<Where><And><And><Eq><FieldRef Name='AuthorName' LookupId='True' /><Value Type='Integer'>"+userID +"</Value></Eq><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq></And><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + strPerformanceID + "</Value></Eq></And></Where>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                                retItem = listItems;// listItems[0];
                        }
                    }
                }
            });
            return retItem;
        }
        //根据当前时间判断当前进行到什么审批阶段，如果为空则当前没有进行审批
        public string GetAppraiseAction(SPItemEventProperties properties)
        {
            string siteUrl = properties.Site.Url;// SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                {
                    string childListName = properties.List.Title + "业绩";
                    SPList spList = spWeb.Lists.TryGetList(childListName);
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><And>
                                         <Leq>
                                            <FieldRef Name='StartDate' />
                                            <Value Type='DateTime'>
                                               <Today />
                                            </Value>
                                         </Leq>
                                         <Gt>
                                            <FieldRef Name='EndDate' />
                                            <Value Type='DateTime'>
                                               <Today />
                                            </Value>
                                         </Gt>
                                      </And></Where>";
                        SPListItemCollection listItems = spList.GetItems(qry);
                        if (listItems.Count > 0)
                        {
                            return listItems[0]["标题"].ToString();
                        }
                    }
                }
            }
            return "";
        }
        private bool hasChildList(SPItemEventProperties properties)
        {
            string siteUrl = properties.Site.Url;// SPContext.Current.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                {
                    string childListName = properties.List.Title + "业绩";
                    SPList spList = spWeb.Lists.TryGetList(childListName);
                    if (spList != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 添加子列表项
        /// </summary>
        /// <param name="childItem"></param>
        /// <param name="properties"></param>
        private void AddChild(ref SPListItem childItem, SPItemEventProperties properties)
        {
            childItem["ID"] = properties.ListItemId;//ID查阅项
            childItem.Update();//添加子列表项
        }

        #endregion
        #region ADHelp
        ///
        ///根据用户帐号称取得用户的 对象
        ///
        ///用户帐号名 

        ///如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName)
        {
            DirectoryEntry de = new DirectoryEntry(ADPath);//GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + sAMAccountName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch
            {
                return null;
            }
        }
        public static DirectoryEntry GetDirectoryEntryByName(string displayName)
        {
            DirectoryEntry de = new DirectoryEntry(ADPath);//GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(name=" + displayName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResultCollection results = deSearch.FindAll();
                if (results.Count == 1)
                    return (new DirectoryEntry(results[0].Path));
                foreach (SearchResult result in results)
                {
                    if (result.Path.Contains("教师"))
                        return (new DirectoryEntry(result.Path));
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        private static string ADPath
        {
            get
            {
                DirectoryEntry root = new DirectoryEntry("LDAP://rootDSE");
                string path = "LDAP://" + root.Properties["defaultNamingContext"].Value;
                return path;

            }
        }
        public static DirectoryEntry GetDirectoryEntryOfOU(string entryPath, string ouName)
        {
            if (entryPath == "") entryPath = ADPath;
            DirectoryEntry de = new DirectoryEntry(entryPath);//GetDirectoryObject();

            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(objectClass=organizationalUnit)(OU=" + ouName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    
#region 属性
        //获取业绩子表的关联字段内部名，显示名称同主表标题，查阅项
        private string GetSubTitleInterlname(SPItemEventProperties properties)
        {
            string siteUrl = properties.Site.Url;
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb(properties.Web.ID))
                {
                    string childListName = properties.List.Title + "业绩";
                    SPList list = spWeb.Lists.TryGetList(childListName);
                    if (list != null)
                    {
                        string displayName = GetTitleDispName(properties);//获取主表的标题显示名称
                        SPField field = list.Fields.GetField(displayName);
                        return field.InternalName;
                    }
                    else
                        return "";
                }
            }
        }
        //获取当前表的显示名称
        private string GetDispNameByInternalName(SPList list, string internalName)
        {
            SPField field = list.Fields.GetFieldByInternalName(internalName );
            return field.Title;
        }
        //获到主表标题的中文显示名称，如论文标题，内部名称为Title
        private string GetTitleDispName(SPItemEventProperties properties)
        {
            SPList list = properties.List;
            SPField field = list.Fields.GetFieldByInternalName("Title");
            return field.Title;
        }
        #endregion

    }
}