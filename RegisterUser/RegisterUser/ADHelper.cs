using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using ActiveDs;
using System.Runtime.InteropServices;
using System.Collections;

namespace RegisterUser.RegisterUser
{
    public class ADHelper
    {
        // Fields
        private static string domainPath;
        private DirectoryEntry root;

        // Methods
        public ADHelper()
        {
            this.root = new DirectoryEntry("LDAP://rootDSE");
            try
            {
                domainPath = "LDAP://" + this.root.Properties["defaultNamingContext"].Value;
            }
            catch
            {
                domainPath = "";
            }
        }

        public ADHelper(string path)
        {
            domainPath = path;
        }
        /// <summary>
        /// 创建域用户,"administrator","Ccc2008neu","administrator","Ccc2008neu"
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="displayName"></param>
        /// <param name="description"></param>
        /// <param name="pwd"></param>
        public static bool AddUser(string loginName, string displayName, string email, string phone, string pwd, string topPath, string groupName, string schoolName, bool enabled)
        {
            string ouPath = AddOU(topPath, schoolName);
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
        /// 用户添加到安全组
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        private static void AddUserToGroup(DirectoryEntry group, DirectoryEntry newUser)
        {
            if (group != null)
            {
                group.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                group.CommitChanges();
            }

        }
        /// <summary>
        /// 先查找，没有就创建
        /// </summary>
        /// <param name="AD"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static DirectoryEntry AddGroup(DirectoryEntry AD, string groupName)
        {
            DirectoryEntry grp=null;
            try
            {
                grp = AD.Children.Find("cn=" + groupName, "group");
            }
            catch
            {
                if (grp == null)
                {
                    grp = AD.Children.Add("cn=" + groupName, "group");
                    grp.Properties["sAMAccountName"].Add(groupName);//windows2000 以前的组名
                    grp.CommitChanges();
                }
            }
            return grp;
        }

        public static string AddOU(string path, string ouName)
        {
            if ((path == null) || (path.Length == 0))
            {
                return "";
            }
            using (DirectoryEntry AD = new DirectoryEntry(path))
            {
                DirectoryEntry OU = AD.Children.Add("OU=" + ouName, "organizationalUnit");
                try
                {
                    OU.CommitChanges();
                    return OU.Path;
                }
                catch
                {
                    return GetDirectoryEntryOfOU(ouName).Path;
                }
                finally
                {
                    if (OU != null)
                    {
                        OU.Dispose();
                    }
                }
            }
        }

        public bool AddUser(string path, UserInfo userInfo)
        {
            bool isResult = false;
            using (DirectoryEntry AD = new DirectoryEntry(path))
            {
                using (DirectoryEntry NewUser = AD.Children.Add("CN=" + userInfo.name, "user"))
                {
                    NewUser.Properties["displayName"].Add(userInfo.displayName);
                    NewUser.Properties["name"].Add(userInfo.name);
                    NewUser.Properties["sAMAccountName"].Add(userInfo.sAMAccountName);
                    NewUser.Properties["userPrincipalName"].Add(userInfo.userPrincipalName);
                    NewUser.Properties["description"].Add(userInfo.description);
                    NewUser.CommitChanges();

                    ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;
                    user.AccountDisabled = !userInfo.userEnabled;
                    user.SetPassword(userInfo.userPassword);
                    //密码永不过期
                    dynamic flag = user.Get("userAccountControl");

                    int newFlag = 0X10000;
                    user.Put("userAccountControl", newFlag);
                    user.SetInfo();

                    NewUser.CommitChanges();

                    isResult = true;
                }
            }
            return isResult;
        }

        public static void AddUserToGroup(DirectoryEntry AD, string groupName, DirectoryEntry newUser)
        {
            if (groupName != "")
            {
                DirectoryEntry grp = null;
                try
                {
                    grp = AD.Children.Find("cn=" + groupName, "group");
                }
                catch
                {
                    if (grp == null)
                    {
                        grp = AddGroup(AD, groupName);
                    }
                }
                if (grp != null)
                {
                    try
                    {
                        grp.Properties["member"].Add(newUser.Properties["distinguishedName"].Value);
                        grp.CommitChanges();
                    }
                    catch
                    {
                    }
                }
            }
        }
        /// <summary>
        /// 获取OU下的子结点，用于添加外语学院下的教师系部
        /// </summary>
        /// <param name="entryPath"></param>
        /// <param name="ouName"></param>
        /// <returns></returns>
        public static ArrayList  GetDeptsOfOU(string entryPath, string ouName)
        {
            ArrayList depts = new ArrayList();
            if (entryPath == "") entryPath = ADPath;
            string ouPath = GetDirectoryEntryOfOU(entryPath, ouName);
            DirectoryEntry de = new DirectoryEntry(ouPath);// GetDirectoryEntryOfouName(entryPath, ouName);
            //DirectoryEntry de = GetDirectoryEntryOfOU(ouName);
            if (de != null && ouPath!="")
            {
                foreach (DirectoryEntry subDepts in de.Children)
                {
                    if (subDepts.SchemaClassName == "organizationalUnit")
                        depts.Add(subDepts.Name.Substring (3).Trim ());

                }
            }
            return depts;

            //DirectorySea rcher deSearch = new DirectorySearcher(de);
            //deSearch.Filter = "(&(objectClass=organizationalUnit)(OU=" + ouName + "))";
            //deSearch.SearchScope = SearchScope.Subtree;

            //try
            //{
            //    SearchResult result = deSearch.FindOne();
            //    return depts;
            //}
            //catch
            //{
            //    return depts;
            //}
        }
        public static DirectoryEntry GetDirectoryEntryOfouName(string entryPath, string ouName)
        {
            if (entryPath == "") entryPath = ADPath;
            DirectoryEntry de = new DirectoryEntry(entryPath);//GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(objectClass=organizationalUnit)(OU=" + ouName + "))";
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
            //de.Children.Find("OU=" + ouName, "organizationalUnit");

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
        public static DirectoryEntry GetDirectoryEntryOfOU(string ouName)
        {
            DirectoryEntry de = new DirectoryEntry(DomainPath);
            DirectorySearcher deSearch = new DirectorySearcher(de)
            {
                Filter = "(&(objectClass=organizationalUnit)(OU=" + ouName + "))",
                SearchScope = SearchScope.Subtree
            };
            try
            {
                SearchResult result = deSearch.FindOne();
                return deSearch.FindOne().GetDirectoryEntry();
            }
            catch
            {
                return null;
            }
        }
        #region 属性
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
        public static string DomainPath
        {
            get
            {
                if (string.IsNullOrEmpty(domainPath))
                {
                    DirectoryEntry broot = new DirectoryEntry("LDAP://rootDSE");
                    try
                    {
                        domainPath = "LDAP://" + broot.Properties["defaultNamingContext"].Value;
                    }
                    catch
                    {
                        domainPath = "";
                    }
                }
                return domainPath;
            }
        }
        #endregion
        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct UserInfo
        {
            public string name;
            public string displayName;
            public string sAMAccountName;
            public string description;
            public string userPassword;
            public string userPrincipalName;
            public bool passwordExpire;
            public bool userEnabled;
        }
    }
}
