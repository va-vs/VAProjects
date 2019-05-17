using System;
using System.Web;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.DirectoryServices;
using System.Text;

namespace WriteNewsDept.WriteDept
{
    /// <summary>
    /// 列表项事件
    /// </summary>
    public class WriteDept : SPItemEventReceiver
    {
        /// <summary>
        /// 已添加项.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {

                if (properties.List.Fields.ContainsField("所属系部"))
                {
                    string loginName =  GetAccount(properties.Web.CurrentUser);
                    properties.ListItem["所属系部"] = GetUserDept(loginName);
                    properties.ListItem.Update();
                }

            });
            
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
            SPUser loginUser = properties.Web.CurrentUser;
            string loginInfo=  loginUser.ID+";#"+ loginUser.Name ;
            SPList lstNews = properties.List;//
            bool hasRight = UserHaveApproveRight(properties.SiteId, properties.Web.Name, properties.List.Title , loginUser);
            string modeState=properties.ListItem["审批状态"].ToString();
            if (modeState =="0" &&!hasRight)//审批通过
            {
                     properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = "这不是您发布的新闻，您无权修改！";
 
            }
            else if  (!hasRight && !properties.ListItem["创建者"].ToString().Contains(loginInfo))
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "这不是您发布的新闻，您无权修改！";
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
                //properties.Status = SPEventReceiverStatus.CancelWithError;
                // properties.ErrorMessage = "此新闻已经审批，您无权删除！";
                properties.Status = SPEventReceiverStatus.CancelNoError;
                //properties.Status = SPEventReceiverStatus.CancelWithRedirectUrl;
                //properties.RedirectUrl = "/_layouts/15/CustError/error.aspx?ErrMsg='文件不存在'";

            }
        }
        #region 方法
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
        //public static string UrlDecoder(string str, Encoding encoding)
        //{
        //    if (encoding == null)
        //    {
        //        Encoding utf8 = Encoding.UTF8;
        //        //首先用utf-8进行解码                    
        //        string code = HttpUtility.UrlDecode(str.ToUpper(), utf8);
        //        //将已经解码的字符再次进行编码.
        //        string encode = HttpUtility.UrlEncode(code, utf8).ToUpper();
        //        if (str == encode)
        //            encoding = Encoding.UTF8;
        //        else
        //            encoding = Encoding.GetEncoding("gb2312");
        //    }
        //    return HttpUtility.UrlDecode(str, encoding);
        //}
        private bool UserHaveApproveRight(Guid  siteID,string webName, string lstName,SPUser user)
        {
            bool isRight = true;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                  try
                {
                    using (SPSite spSite = new SPSite(siteID ))
                    {
                        using (SPWeb sWeb = spSite.AllWebs[webName])
                        {
                            SPList sList = sWeb.Lists[lstName];
                            sList.DoesUserHavePermissions(user, SPBasePermissions.ApproveItems);
                            isRight =  sList.DoesUserHavePermissions (user,SPBasePermissions.ApproveItems );
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
        public static string GetUserDept(string loginName)
        {
            string dept = "";
            DirectoryEntry de = ADHelper.GetDirectoryEntryByAccount(loginName);
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.Filter = ("(SAMAccountName=" + loginName + ")");
            SearchResult dss = ds.FindOne();
            if (dss != null)
            {
                string dpath = dss.Path;
                dpath = dpath.Substring(dpath.IndexOf("OU=") + 3);
                dept = dpath.Substring(0, dpath.IndexOf(","));
            }
            else
                dept = "管理员";
            return dept;
        }
        #endregion
    }
}