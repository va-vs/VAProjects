using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Text.RegularExpressions;

namespace SmartNEUEntrollEvent.SmartNEUEntrollEventReceiver
{
    /// <summary>
    /// 作品提交时添加默认数据（博客网站-301）
    /// </summary>
    public class SmartNEUEntrollEventReceiver : SPItemEventReceiver
    {
        #region 事件
        /// <summary>
        /// 正在添加项.，同一个登录用户只能提交一个作品
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
            if (!properties.List.Fields.ContainsField("学号/工号"))
                return ;
            string loginName = GetAccount(properties.UserLoginName );
            int isEntroll = 0;
            isEntroll = CheckEntroll(properties, loginName);
            this.EventFiringEnabled = false;
            if (isEntroll ==1 )
            {
                properties.ErrorMessage = "您填写的学号/工号与登录帐号不符！";
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
            else if (isEntroll == 2)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "您的帐号不是学号/工号，不能申报！";

            }
            else if (isEntroll ==3 )
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = "您已申报创意，不能重复申报！";

            }
            this.EventFiringEnabled = true;

        }

        /// <summary>
        /// 已添加项，发布日期保存为当前日期
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            properties.ListItem["发布日期"] = DateTime.Now;
            properties.ListItem.Update();
        }
        #endregion
        #region 方法
        //同一个人只能进行一次报名
        private int  CheckEntroll(SPItemEventProperties properties,string loginName)
        {
            string internalName = properties.List.Fields["学号/工号"].InternalName;
            SPItemEventDataCollection afterData = properties.AfterProperties;
            if (afterData[internalName].ToString().Trim() != loginName)
                return 1;
            if (!Common.IsMatching (loginName ))
                return 2;
            SPList spList = properties.List;
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope='RecursiveAll'";
            query.Query = "<Where><Eq><FieldRef Name='" + internalName + "' /><Value Type='Text'>" + loginName + "</Value></Eq></Where>";
            SPListItemCollection listItems = spList.GetItems(query);
            if (listItems.Count > 0)
                return 3;
            else
                return 0;
        }
        public static string GetAccount(string loginName)
        {
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        #endregion
      
    }
    public class Common
    {
        public static bool IsMatching(string accString)
        {
            bool ismatch = true;
            ismatch = Common.isNumberic(accString);
            if (ismatch)
            {
                if (accString.Length >= 5 && accString.Length <= 8)
                    ismatch = true;
                else
                    ismatch = false;
            }

            return ismatch;
        }
        //判断字符串是否是数字
        public static bool isNumberic(string message)
        {
            //if (message != "" && Regex.IsMatch(message, @"^\d{5}$"))
            Regex rex = new Regex(@"^\d+$");

            if (rex.IsMatch(message))
            {
                return true;
            }
            else
                return false;
        }
    }
}