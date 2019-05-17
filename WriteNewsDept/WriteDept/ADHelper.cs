using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteNewsDept.WriteDept
{
    public class ADHelper
    {
        #region 属性
        /// <summary>
        /// ccc
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
        public static bool AddUser(string loginName, string displayName, string email, string phone, string pwd, string topPath, string groupName, string schoolName, bool enabled)
        {
            string ouPath = "";// AddOU(topPath, schoolName);
            bool result = true;
            string content = "";
            //先加安全组，帐号重复会出错；否则会出现错误
            //DirectoryEntry grp = AddGroup(new DirectoryEntry(topPath), groupName);
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
                            //ActiveDs.IADsUser user = (ActiveDs.IADsUser)NewUser.NativeObject;
                            //user.AccountDisabled = !enabled;
                            //user.SetPassword(pwd);
                            //密码永不过期
                            //dynamic flag = user.Get("userAccountControl");

                            //int newFlag = 0X10000;
                            //user.Put("userAccountControl", newFlag);
                            //user.SetInfo();

                            NewUser.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            content += ex.ToString() + "\r\f";
                        }
                        if (groupName != "")
                            //AddUserToGroup(grp, NewUser);
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
        /// 根据用户帐号取得用户的对象。如果找到该用户，则返回用户的对象；否则返回null
        /// </summary>
        /// <param name="sAMAccountName"></param>
        /// <returns></returns>
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

        public static string GetClassNameByAccount(string AccountName)
        {
            DirectoryEntry de = new DirectoryEntry(ADPath);
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + AccountName + "))";
            deSearch.SearchScope = SearchScope.Subtree;
            string member = null;
            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                string name = null;
                if (de.Properties["memberOf"].Count > 1)
                {
                    name = de.Properties["memberOf"][de.Properties["memberOf"].Count - 1].ToString();
                }
                else
                {

                    name = de.Properties["memberOf"].Value.ToString();
                }



                member = name.Substring(3, name.IndexOf(",") - 3);
                return member;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户所属组
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public string[] GetGroupsForUser(string username, string _path)
        {
            DirectorySearcher ds = new DirectorySearcher(_path);
            ds.Filter = "(&(sAMAccountName=" + username + "))";
            ds.PropertiesToLoad.Add("memberof");
            SearchResult r = ds.FindOne();

            if (r.Properties["memberof"].Count == 0)
            {
                return (null);
            }

            string[] results = new string[r.Properties["memberof"].Count];
            for (int i = 0; i < r.Properties["memberof"].Count; i++)
            {
                string theGroupPath = r.Properties["memberof"][i].ToString();
                results[i] = theGroupPath.Substring(3, theGroupPath.IndexOf(",") - 3);
            }
            return (results);
        }
        #endregion
    }
}
