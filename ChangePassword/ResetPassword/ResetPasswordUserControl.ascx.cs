﻿using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ChangePassword.ResetPassword
{
    public partial class ResetPasswordUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SPUser user = SPContext.Current.Web.CurrentUser;
                if ( user== null)
                {
                    Response.Redirect(SPContext.Current.Web.Url);
                    return;
                }
                else
                {
                    string[] accouts = webObj.AllowAccounts.Split(',') ;
                    string lngAccount = user.LoginName;
                    bool chk=false ;
                    foreach (string acc in accouts )
                    {
                        if (lngAccount.EndsWith (acc))
                        {
                            chk = true;
                            break;
                        }
                    }
                    if (!chk )
                    {
                        lblMsg.Text = "您无权进行此项操作！";
                        //txtAccount.Enabled = false;
                        UserID.Enabled = false;
                        txtPwd.Enabled = false;
                        txtPwd1.Enabled = false;
                        btnSubmit.Enabled = false;
                        return;
                    }
                    txtPwd.Text = webObj.DefaultPassword ;
                    txtPwd1.Text = txtPwd.Text;
                }
            }
            btnSubmit.Click += btnSubmit_Click;
            txtPwd.Attributes["value"] = txtPwd.Text;
            txtPwd1.Attributes["value"] = txtPwd1.Text;
        }
        public ResetPassword webObj { get; set; }
        void btnSubmit_Click(object sender, EventArgs e)
        {
            bool isSuccess = false;
            string errMsg = "";
            
            if (UserID.ResolvedEntities.Count ==0)
            {
                lblMsg.Text = "帐号不能为空！";
                return;
            }
            SPWeb web = SPContext.Current.Web;
            PickerEntity picker = UserID.ResolvedEntities[0] as PickerEntity;
            SPUser user = web.EnsureUser(picker.Key);
            string account = user.LoginName;
            account = account.Substring(account.IndexOf("\\")+1);
            string strConst = HiddenField1.Value;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                string domain = ADHelper.Domain;
                if (impersonateValidUser("administrator", domain, strConst))//.Substring(strConst.IndexOf(" ") + 1)))
                {
                    isSuccess = ADHelper.ChangePassword(account, txtPwd.Text, ref errMsg);
                    undoImpersonation();
                }
                else
                {
                    //Your impersonation failed. Therefore, include a fail-safe mechanism here.
                }
                if (isSuccess)
                {
                    StreamWriter w = new StreamWriter("c:\\restPwd.txt",true );
                    w.WriteLine(SPContext.Current.Web.CurrentUser.Name + "   " + DateTime.Now.ToString() + "重置了帐号：" + account  + "；"+user.Name +"    新密码为：" + txtPwd.Text);
                    w.Close();
                    lblMsg.Text = "重置密码成功！";
                }

                else
                    lblMsg.Text = "重置密码失败：" + errMsg;


            });

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "message", "<script defer>alert('密码重置成功，请重新登录本站！');top.location.href='" + SPContext.Current.Site.Url + "/_layouts/15/Authenticate.aspx'</script>");

        }
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        WindowsImpersonationContext impersonationContext;

        [DllImport("advapi32.dll")]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        private bool impersonateValidUser(String userName, String domain, String password)
        {
            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;

            if (RevertToSelf())
            {
                if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                    LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                {
                    if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                    {
                        tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                        impersonationContext = tempWindowsIdentity.Impersonate();
                        if (impersonationContext != null)
                        {
                            CloseHandle(token);
                            CloseHandle(tokenDuplicate);
                            return true;
                        }
                    }
                }
            }
            if (token != IntPtr.Zero)
                CloseHandle(token);
            if (tokenDuplicate != IntPtr.Zero)
                CloseHandle(tokenDuplicate);
            return false;
        }

        private void undoImpersonation()
        {
            impersonationContext.Undo();
        }
    }
    public class ADHelper
    {
        #region 属性
        /// <summary>
        /// 返回域
        /// </summary>
        public static string Domain
        {
            get
            {
                DirectoryEntry root = new DirectoryEntry("LDAP://rootDSE");

                string domain = (string)root.Properties["ldapServiceName"].Value;
                domain = domain.Substring(0, domain.IndexOf("."));
                return domain;

            }
        }
        /// <summary>
        /// 获取如下形式的域全名@CCC.NEU.EDU.CN
        /// </summary>
        private static string DomainName
        {
            get
            {
                DirectoryEntry root = new DirectoryEntry("LDAP://rootDSE");
                string domain = (string)root.Properties["ldapServiceName"].Value;
                domain = domain.Substring(domain.IndexOf("@"));
                return domain;
            }
        }
        ///
        /// LDAP绑定路径
        ///
        private static string ADPath
        {
            get
            {
                DirectoryEntry root = new DirectoryEntry("LDAP://rootDSE");
                string path = "LDAP://" + root.Properties["defaultNamingContext"].Value;
                return path;

            }
        }
        #endregion
        #region 方法
        //启用账户
        public static bool EnableAdUser(string account)
        {
            try
            {
                DirectoryEntry NewUser = GetDirectoryEntryByAccount(account);
                if (NewUser != null)
                {
                    try
                    {
                        ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;

                        user.AccountDisabled = false;

                        user.SetInfo();

                        NewUser.CommitChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch
            {

            }
            return false;
        }
        //删除用户
        public static bool DeleteAdUser(string account)
        {
            try
            {
                DirectoryEntry lgUser = GetDirectoryEntryByAccount(account);
                if (lgUser != null)
                {
                    lgUser.DeleteTree();
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
        /// <summary>
        /// 添加安全组，先判断是否存在
        /// </summary>
        /// <param name="AD"></param>
        /// <param name="groupName"></param>
        public static DirectoryEntry AddGroup(DirectoryEntry AD, string groupName)
        {
            DirectoryEntry grp;
            try
            {
                grp = AD.Children.Find("cn=" + groupName, "group");
            }
            catch
            {
                grp = AD.Children.Add("cn=" + groupName, "group");
                grp.Properties["sAMAccountName"].Add(groupName);//windows2000 以前的组名
                grp.CommitChanges();
            }
            return grp;

        }
        //活动目录加组织单元,并返回新组织单元的路径
        public static string AddOU(string path, string ouName)
        {
            if (path == null || path.Length == 0) return "";
            string ouPath = GetDirectoryEntryOfOU(path, ouName);
            if (ouPath == "")
            {
                using (DirectoryEntry AD = new DirectoryEntry(path))
                {
                    using (DirectoryEntry OU = AD.Children.Add("OU=" + ouName, "organizationalUnit"))
                    {
                        OU.CommitChanges();
                        return OU.Path;
                        //OU.Properties["distinguishedName"].Value.ToString();  
                    }
                }
            }
            else
                return ouPath;
        }
        //编写用户的单位名称
        public static bool EditUserDepartment(string userDep, string loginName)
        {
            DirectoryEntry currentUser = ADHelper.GetDirectoryEntryByAccount(loginName);//当前被编辑的用户
            //currentUser.Properties["name"][0] = displayName;//执行此句会出现错误
            if (userDep != "")
            {
                if (currentUser.Properties.Contains("department"))
                    currentUser.Properties["department"][0] = userDep;
                else
                    currentUser.Properties["department"].Add(userDep);//家庭电话otherTelephone
                try
                {
                    currentUser.CommitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 创建域用户,"administrator","Ccc2008neu","administrator","Ccc2008neu"
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        /// <param name="pwd"></param>
        public static bool AddUser(string loginName, string displayName, string email, string phone, string pwd, string topPath, string groupName,  bool enabled, string depart="")
        {
            string ouPath = topPath  ;// AddOU(topPath, schoolName);
            bool result;
            string content = "";
            //先加安全组，帐号重复会出错；否则会出现错误
            DirectoryEntry grp = AddGroup(new DirectoryEntry(topPath), groupName);
            using (DirectoryEntry AD = new DirectoryEntry(ouPath))
            {
                try
                {
                    using (DirectoryEntry NewUser = AD.Children.Add("CN=" + loginName, "user"))
                    {
                        NewUser.Properties["displayName"].Add(displayName);
                        NewUser.Properties["name"].Add(displayName);
                        NewUser.Properties["sAMAccountName"].Add(loginName);
                        NewUser.Properties["userPrincipalName"].Add(loginName + DomainName);
                        if (phone != "")
                            NewUser.Properties["telephoneNumber"].Add(phone);
                        if (email != "")
                            NewUser.Properties["mail"].Add(email);
                        if (depart !="")//单位名称
                            NewUser.Properties["department"].Add(depart);
                        NewUser.CommitChanges();
                        try
                        {
                            ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;
                            user.AccountDisabled = !enabled;
                            user.SetPassword(pwd);
                            //密码永不过期
                            dynamic flag = user.Get("userAccountControl");

                            int newFlag = 0X10000;
                            user.Put("userAccountControl", newFlag);
                            user.SetInfo();

                            NewUser.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            content += ex.ToString() + "\r\f";
                        }
                        if (groupName != "")
                            AddUserToGroup(grp, NewUser);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    content += ex.ToString();
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 将指定登录名的用户添加到安全组
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool AddUserToSafeGroup(string loginName, string groupName)
        {
            try
            {
                DirectoryEntry user = GetDirectoryEntryByAccount(loginName);
                DirectoryEntry group = GetDirectoryEntryOfGroup("", groupName);
                AddUserToGroup(group, user);
                return true;
            }
            catch
            {
                return false;
            }


        }
        /// <summary>
        /// 编辑AD中已经注册的当前用户信息
        /// </summary>
        /// <param name="loginName">当前用户的登录名</param>
        /// <param name="displayName">当前用户的显示名</param>
        /// <param name="email">当前用户的电子邮件</param>
        /// <param name="mobile">当前用户的手机号码</param>
        /// <returns>T/F</returns>
        public static bool EditUser(string loginName, string displayName, string email, string mobile, string groupName, string userDep = "")
        {
            DirectoryEntry currentUser = ADHelper.GetDirectoryEntryByAccount(loginName);//当前被编辑的用户
            currentUser.Properties["displayName"][0] = displayName;
            //currentUser.Properties["name"][0] = displayName;//执行此句会出现错误
            if (mobile != "")
            {
                if (currentUser.Properties.Contains("telephoneNumber"))
                    currentUser.Properties["telephoneNumber"][0] = mobile;
                else
                    currentUser.Properties["telephoneNumber"].Add(mobile);//家庭电话otherTelephone
            }
            else
                if (currentUser.Properties.Contains("telephoneNumber"))
                    currentUser.Properties["telephoneNumber"].RemoveAt(0);

            if (email != "")
            {
                if (currentUser.Properties.Contains("mail"))
                    currentUser.Properties["mail"][0] = email;
                else
                    currentUser.Properties["mail"].Add(email);
            }
            else
                if (currentUser.Properties.Contains("mail"))
                    currentUser.Properties["mail"].RemoveAt(0);

            if (userDep != "")
            {
                if (currentUser.Properties.Contains("department"))
                    currentUser.Properties["department"][0] = mobile;
                else
                    currentUser.Properties["department"].Add(mobile);//手机号码
            }
            else
                if (currentUser.Properties.Contains("department"))
                    currentUser.Properties["department"].RemoveAt(0);

            if (groupName != "")
            {
                AddUserToSafeGroup(loginName, groupName);
            }
            try
            {
                currentUser.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //更改密码
        public static bool ChangePassword(string loginName, string newPassword,ref string errMsg)
        {
            DirectoryEntry NewUser = ADHelper.GetDirectoryEntryByAccount(loginName);
            try
            {
                ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;

                user.SetPassword(newPassword);
                NewUser.CommitChanges();
                return true;
            }
            catch  (Exception ex)
            {
                errMsg = ex.ToString();
                return false;
            }

        }
        /// <summary>
        /// 修改AD中的用户信息,启动帐号（帐号禁用帐号）
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static bool EnabledUser(string loginName, bool userEnabled)
        {
            using (DirectoryEntry NewUser = ADHelper.GetDirectoryEntryByAccount(loginName))
            {
                try
                {
                    ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;
                    user.AccountDisabled = !userEnabled;
                    user.SetInfo();
                    NewUser.CommitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }
        /// <summary>
        /// 用户添加到安全组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        private static void AddUserToGroup(DirectoryEntry group, DirectoryEntry newUser)
        {
            if (group != null)
            {
                try
                {
                    group.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                    group.CommitChanges();
                }
                catch
                {

                }
            }

        }
        /// <summary>
        /// 用户添加到组
        /// </summary>
        /// <param name="AD"></param>
        /// <param name="groupName"></param>
        /// <param name="newUser"></param>
        private static void AddUserToGroup(DirectoryEntry AD, string groupName, DirectoryEntry newUser)
        {
            DirectoryEntry grp;
            try
            {
                grp = AD.Children.Find("cn=" + groupName, "group");
            }
            catch
            {
                grp = AddGroup(AD, groupName);
            }
            if (grp != null)
            {
                //从安全组中移除成员
                //grp.Properties["member"].Remove(de.Properties["distinguishedName"].Value);
                //grp.CommitChanges();
                //向安全组中添加用户
                //grp.Invoke("Add", new object[] { de.Path.ToString() });

                grp.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                grp.CommitChanges();

            }

        }
        private static DirectoryEntry GetDirectoryEntryOfGroup(string entryPath, string groupName)
        {
            if (entryPath == "") entryPath = ADPath;
            DirectoryEntry de = new DirectoryEntry(entryPath);//GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(objectClass=group)(CN=" + groupName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                return result.GetDirectoryEntry();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取OU所在的LDAP路径
        /// </summary>
        /// <param name="ouName"></param>
        /// <returns></returns>
        public static string GetDirectoryEntryOfOU(string entryPath, string ouName)
        {
            if (entryPath == "") entryPath = ADPath;
            DirectoryEntry de = new DirectoryEntry(entryPath);//GetDirectoryObject();

            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(objectClass=organizationalUnit)(OU=" + ouName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                return result.Path;
            }
            catch
            {
                return "";
            }
        }

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

        #endregion
    }

}
