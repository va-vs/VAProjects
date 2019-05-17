using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;

namespace DepartmentPhotoList.DepartmentPhotoList
{
    [ToolboxItemAttribute(false)]
    public class DepartmentPhotoList : WebPart
    {
        // Fields
        private string listName = "系部直通车";

        // Methods
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            SPList list = SPContext.Current.Web.Lists.TryGetList(ListName);
            if (list != null)
                this.Controls.Add(new LiteralControl(this.GetString()));
        }

        private string GetString()
        {
            StringBuilder strText = new StringBuilder();
            strText.AppendLine("<div align='center' style='text-align:center'><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" >");
            strText.AppendLine("<tr>");
            for (int i = 0; i < PhotoCols; i++)
                strText.AppendLine("<td style=\"height: 15px;\"></td>");
            strText.AppendLine("</tr>");
            //strText.AppendLine(this.GetWebLink(new string[] { "BGS", "jxkyb" }, new string[] { "Office.jpg", "LearnOffice.jpg" }));
            //strText.AppendLine(this.GetWebLink(new string[] { "JYS", "JXSJB" }, new string[] { "Computer.jpg", "Practice.jpg" }));
            //strText.AppendLine(this.GetWebLink(new string[] { "XXFWB", "WLYXB" }, new string[] { "Network.jpg", "Service.jpg" }));
            strText.AppendLine(GetWebLink());
            strText.AppendLine("</table></div>");
            return strText.ToString();
        }

        private string GetWebLink()
        {
            StringBuilder strText = new StringBuilder();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SPWeb site = SPContext.Current.Web;
                SPList list = site.Lists[listName];
                SPQuery query = new SPQuery();
                //query.ViewAttributes = "Scope='RecursiveAll'";
                //query.ViewFields = "<FieldRef Name='Title'/><FieldRef Name='FileDirRef'/><FieldRef Name='Modified'/><FieldRef Name='ShowNum'/><FieldRef Name='FileRef'/><FieldRef Name='Description'/>";
                query.Query = "<OrderBy><FieldRef Name='ShowNum' Ascending='true'/></OrderBy>";
                SPListItemCollection items = list.GetItems(query);

                string link;
                int itemIndex = 0;
                string imgUrl;
                string subWeb="";
                for (int r = 0; r < PhotoRows; r++)
                {
                    strText.AppendLine("<tr>");
                    for (int j = 0; j < PhotoCols; j++)
                    {
                        if (itemIndex == items.Count) break;
                        if (items[itemIndex].Fields.ContainsField("Url") && items[itemIndex]["Url"] != null)
                            subWeb = items[itemIndex]["Url"].ToString();
                        else if (items[itemIndex]["说明"] != null)
                            subWeb = items[itemIndex]["说明"].ToString();
                        else
                            subWeb = "";
                        imgUrl = items[itemIndex]["URL 路径"].ToString();

                        if (imgUrl.StartsWith("/"))
                            imgUrl = imgUrl.Substring(1);
                        if (IfRight ==0 || subWeb!="" && this.UserHaveRight(subWeb))//webName[0]))
                        {
                            link = "<a target='_blank' href=\"" + subWeb + "\"><img style=\"border=0\" alt=\"\" src=\"/" + imgUrl + "\" width=\"" + PhotoWidth + "\" height=\"" + PhotoHeight + "\" /></a>";
                        }
                        else
                        {
                            link = "<img style=\"border=0\" alt=\"\" src=\"/" + imgUrl + "\" width=\"" + PhotoWidth + "\"" + PhotoHeight + "\" />";
                        }
                        strText.AppendLine("<td style=\"padding-right: 3px; padding-bottom: 2px; text-align: left;\">" + link + "</td>");
                        itemIndex = itemIndex + 1;
                    }
                    strText.AppendLine("</tr>");
                    if (itemIndex == items.Count) break;
                }
            }
                );


            return strText.ToString();
        }

        private bool UserHaveRight(string subWebUrl)
        {
            SPUser user = SPContext.Current.Web.CurrentUser;
            bool isRight=false ;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {

                string webName = subWebUrl.Substring(subWebUrl.LastIndexOf("/") + 1);
                try
                {
                    using(SPSite spSite=new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb sWeb = spSite.AllWebs[webName])
                        {
                            isRight = sWeb.DoesUserHavePermissions(user.LoginName, SPBasePermissions.Open);
                        }  //return DoesUserHavePermssionsToWeb(ref user, ref sWeb);
                    }
                }
                catch
                {
                    isRight= false;
                }
            });
            return isRight;
        }
        #region 后加的判断用户权限
        private bool DoesUserHavePermssionsToWeb(ref SPUser user, ref SPWeb web)
        {
            bool hasPermission = false;
            SPBasePermissions perms = this.GetPermissionsForUser(ref user, ref web);
            if (perms.ToString().Contains(SPBasePermissions.Open.ToString()) || perms.ToString().Contains(SPBasePermissions.FullMask.ToString()))
                hasPermission = true;

            if (!hasPermission)
            {

                // Check the users groups - this is for indirect membership;        
                foreach (string groupLoginName in this.GetCurrentUserADGroups())
                {
                    try
                    {
                        SPUser groupUser = web.SiteUsers[groupLoginName];
                        perms = this.GetPermissionsForUser(ref groupUser, ref web);
                        if (perms.ToString().Contains(SPBasePermissions.Open.ToString()) || perms.ToString().Contains(SPBasePermissions.FullMask.ToString()))
                        {
                            hasPermission = true;
                            break;
                        }
                    }
                    catch { }
                }
            }
            return hasPermission;
        }

        private SPBasePermissions GetPermissionsForUser(ref SPUser user, ref SPWeb web)
        {
            SPBasePermissions perms = SPBasePermissions.EmptyMask;
            try
            {
                SPUserToken userToken = user.UserToken;
                System.Reflection.MethodInfo getPermissions = typeof(Microsoft.SharePoint.Utilities.SPUtility).GetMethod("GetPermissions",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.InvokeMethod |
                System.Reflection.BindingFlags.Static);
                perms = (SPBasePermissions)getPermissions.Invoke(null, new object[] { userToken, web });
            }
            catch { }
            return perms;
        }

        private System.Collections.ArrayList GetCurrentUserADGroups()
        {
            // Get the current groups for the logged in user;    
            System.Collections.ArrayList groups = new System.Collections.ArrayList();
            foreach (System.Security.Principal.IdentityReference group in System.Web.HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                groups.Add(group.Translate(typeof(System.Security.Principal.NTAccount)).ToString());
            }
            return groups;
        }
        #endregion
        // Properties
        private string CurrentSelectedUser
        {
            get
            {
                object currentSelectedUser = this.ViewState["_CurrentSelectedUser"];
                if (currentSelectedUser == null)
                {
                    this.ViewState["_CurrentSelectedUser"] = this.Context.User.Identity.Name;
                    return this.Context.User.Identity.Name;
                }
                return (string)currentSelectedUser;
            }
            set
            {
                this.ViewState["_CurrentSelectedUser"] = value;
            }
        }

        #region 属性
        [WebBrowsable, WebDisplayName("部门网站图片"), Personalizable, WebDescription("部门网站图片")]
        public string ListName
        {
            get
            {
                return this.listName;
            }
            set
            {
                this.listName = value;
            }
        }
        int _PhotoRows = 4;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片行数")]
        [WebDescription("要显示的图片行")]
        public int PhotoRows
        {
            get { return _PhotoRows; }
            set { _PhotoRows = value; }
        }
        int _photoCols = 2;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片列")]
        [WebDescription("要显示的图片列")]
        public int PhotoCols
        {
            get { return _photoCols; }
            set { _photoCols = value; }
        }
        int _PhotoWidth = 89;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片宽度")]
        [WebDescription("要显示的图片宽度")]
        public int PhotoWidth
        {
            get { return _PhotoWidth; }
            set { _PhotoWidth = value; }
        }
        int _ImagHeight = 68;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片库高度")]
        [WebDescription("要显示的图片高度")]
        public int PhotoHeight
        {
            get { return _ImagHeight; }
            set { _ImagHeight = value; }
        }
        int _ifRight = 1;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("是否判断权限")]
        [WebDescription("对链接地址是否需要判断权限")]
        public int IfRight
        {
            get { return _ifRight; }
            set { _ifRight = value; }
        }
        #endregion
    }
}
