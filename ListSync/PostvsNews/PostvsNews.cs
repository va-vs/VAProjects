using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace ListSync.PostvsNews
{
    /// <summary>
    /// 列表项事件,在博客中输入新闻，同时同步到列表。
    /// </summary>
    public class PostvsNews : SPItemEventReceiver
    {
        #region 事件
        /// <summary>
        /// 已添加项.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            string newsID = "";
            if (properties.List.Fields.ContainsField("新闻ID"))
            {
                SPList listPost = properties.List;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                          {
                              using (SPSite mySite = new SPSite(properties.Site.ID))
                              {
                                  using (SPWeb spWeb = mySite.RootWeb)
                                  {
                                      SPList listNews = spWeb.Lists.TryGetList("新闻");
                                      if (listNews != null)
                                      {
                                          SPListItemCollection itemsNews = listNews.Items;

                                          SPListItem itemNews = itemsNews.Add();//将新建的博文添加到新闻列表
                                          UpdateNews(ref itemNews, properties);
                                          newsID = itemNews.ID.ToString();
                                      }
                                  }
                              }
                          });
            }
            if (newsID != "")
            {
                properties.ListItem["新闻ID"] = newsID;//新闻添加时返回新建新闻的ID
                properties.ListItem.Update();//将新闻ID信息更新到对应博客项中

            }
        }
        private void UpdateNews(ref SPListItem itemNews, SPItemEventProperties properties)
        {
            itemNews["标题"] = properties.ListItem["标题"];//标题
            itemNews["正文"] = properties.ListItem["正文"];//正文
            itemNews["创建者"] = properties.ListItem["创建者"];//创建者
            if (properties.ListItem["类别"].ToString()!="" )
            {
                string newsType = properties.ListItem["类别"].ToString();
                itemNews["新闻类别"] = newsType.Substring(newsType.IndexOf(";#") + 2);//类别
            }
            else
            {

            }
            itemNews.Update();//添加新闻
        }
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
            SPUser loginUser = properties.Web.CurrentUser;
            string loginInfo = loginUser.ID + ";#" + loginUser.Name;
            SPList lstNews = properties.List;//
            bool hasRight = UserHaveApproveRight(properties.SiteId, properties.Web.Name, properties.List.Title, loginUser);
            string modeState = properties.ListItem["审批状态"].ToString();
            if (modeState == "0" && !hasRight)//审批通过
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "这不是您发布的新闻，您无权修改！";

            }
            else if (!hasRight && !properties.ListItem["创建者"].ToString().Contains(loginInfo))
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "这不是您发布的新闻，您无权修改！";
            }

        }
        /// <summary>
        /// 已更新项.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            if (properties.List.Fields.ContainsField("新闻ID"))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                          {
                              using (SPSite mySite = new SPSite(properties.Site.ID))
                              {
                                  using (SPWeb spWeb = mySite.RootWeb)
                                  {
                                      SPList listNews = spWeb.Lists.TryGetList("新闻");
                                      if (listNews != null)
                                      {
                                          SPListItemCollection itemsNews = listNews.Items;
                                          int Newsid = int.Parse(properties.ListItem["新闻ID"].ToString());
                                          SPListItem itemNews = itemsNews.GetItemById(Newsid);//查找指定同步标记ID的这条新闻
                                          if (itemNews != null)//该条新闻存在
                                          {
                                              UpdateNews(ref itemNews, properties);
                                          }
                                      }
                                  }

                              }
                          });
            }

        }
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);

            SPUser loginUser = properties.Web.CurrentUser;
            string loginInfo = loginUser.ID + ";#" + loginUser.Name;
            SPList lstNews = properties.List;//
            bool hasRight = UserHaveApproveRight(properties.SiteId, properties.Web.Name, properties.List.Title, loginUser);
            string modeState = properties.ListItem["审批状态"].ToString();
            if (modeState == "0" && !hasRight)//审批通过
            {
                properties.Status = SPEventReceiverStatus.CancelNoError;
            }
            else if (properties.Status == SPEventReceiverStatus.Continue)
            {
                if (properties.List.Fields.ContainsField("新闻ID"))
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite mySite = new SPSite(properties.Site.ID))
                        {
                            using (SPWeb spWeb = mySite.RootWeb)
                            {
                                SPList listNews = spWeb.Lists.TryGetList("新闻");
                                if (listNews != null)
                                {
                                    SPListItemCollection itemsNews = listNews.Items;
                                    int Newsid = int.Parse(properties.ListItem["新闻ID"].ToString());
                                    SPListItem itemNews = itemsNews.GetItemById(Newsid);//查找指定同步标记ID的这条新闻
                                    if (itemNews != null)//该条新闻存在
                                    {
                                        itemNews.Delete();
                                        listNews.Update();
                                    }
                                }
                            }

                        }
                    });
                }
            }

        }
     
        #endregion

        #region 方法
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
        //获取登陆用户的账号
        public static string GetAccount(SPUser currentUser)
        {
            string loginName = currentUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        #endregion
    }
}