using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSCDLL.Common
{
    public class Users
    {
        //获取SharePoint网站当前用户的UID，若未登录，则为0
        public static int UserID
        {
            get
            {
                SPUser user = SPContext.Current.Web.CurrentUser;
                if (user != null)
                {
                    return user.ID;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取用户登录账号
        /// </summary>
        /// <returns></returns>
        public static string GetLoginAccount
        {
            get
            {
                SPUser us = SPContext.Current.Web.CurrentUser;
                string accountName = "";
                if (us != null)
                {
                    accountName = SPContext.Current.Web.CurrentUser.LoginName;
                    accountName = accountName.Substring(accountName.LastIndexOf("\\") + 1);
                }
                return accountName;
            }
        }
    }
}
