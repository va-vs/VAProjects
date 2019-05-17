using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkEvaluate.Layouts.WorkEvaluate.DAL;
using System.Data;
using Microsoft.SharePoint;

namespace WorkEvaluate.Layouts.WorkEvaluate.BLL
{
    public class User
    {
        /// <summary>
        /// 获取团队成员
        /// </summary>
        /// <param name="worksID"></param>
        /// <returns></returns>
        public static DataSet GetGroupMemberByWorksID(long worksID)
        {
            DataSet ds = DAL.User.GetUserByWorksID(worksID);
            DataRow[] drs = ds.Tables[0].Select("Relationship=2");
            DataSet dsTmp = ds.Clone();
            dsTmp.Merge(drs);
            return dsTmp;
        }
        public static long GetUserID(SPUser user)
        {
            string name=user.LoginName; 
            //检查用户
            DataTable dt = DAL.User.GetUserByAccount(name.Substring(name.LastIndexOf("\\") + 1)).Tables[0];
            if (dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                dr["Account"] = user.LoginName.Split('\\')[1];
                dr["Name"] = user.Name;
                dr["Flag"] = 1;
                return DAL.User.InsertUser(dr);
            }
            else {
                return long.Parse(dt.Rows[0]["UserID"].ToString());
            
            }
        }


        public static SPUser GetSPUser(string sLoginName)
        {
            SPUser oUser = null;
            try
            {
                if (!string.IsNullOrEmpty(sLoginName))
                {
                    oUser = SPContext.Current.Web.EnsureUser(sLoginName);
                }
            }
            catch (Exception ex)
            {
                string sMessage = ex.Message;
            }
            return oUser;
        }
        public static bool JudgeUserRight()
        {
            bool isStudent;

            string UserAccount = DAL.Common.GetLoginAccount;
            if (UserAccount.Length == 8 && UserAccount.Substring(0, 2) == "20")
            {
                isStudent = true;
            }
            else
            {
                isStudent = false;
            }
            return isStudent;
        }
       
    }
}
