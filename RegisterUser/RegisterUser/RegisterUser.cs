using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RegisterUser.RegisterUser
{
    [ToolboxItemAttribute(false)]
    public class RegisterUser : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/RegisterUser/RegisterUser/RegisterUserUserControl.ascx";

        protected override void CreateChildControls()
        {
            //Control control = Page.LoadControl(_ascxPath);
            RegisterUserUserControl control = Page.LoadControl(_ascxPath) as RegisterUserUserControl;
            if (control != null)
                control.WebPartObj = this;

            Controls.Add(control);
        }
        #region 属性
        /// <summary>
        /// 列表标题
        /// </summary>
        private string _adPath = "外国语学院";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [WebDisplayName("注册用户所在AD的根目录")]
        [WebDescription("注册用户所在的AD根目录")]
        public string ADPath
        {
            set { _adPath = value; }
            get { return _adPath; }
        }
        /// <summary>
        /// 注册教师的路径
        /// </summary>
        private string _teacherPath = "LDAP://OU=东北大学教师,DC=CCC,DC=NEU,DC=EDU,DC=CN";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [WebDisplayName("注册用户所在AD的根目录")]
        [WebDescription("注册用户所在的AD根目录")]
        public string TeacherPath
        {
            set { _teacherPath = value; }
            get { return _teacherPath; }
        }
        /// <summary>
        /// 注册学生的路径
        /// </summary>
        private string _studentPath = "LDAP://OU=东北大学本科生,DC=CCC,DC=NEU,DC=EDU,DC=CN";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [WebDisplayName("注册用户所在AD的根目录")]
        [WebDescription("注册用户所在的AD根目录")]
        public string StudentPath
        {
            set { _studentPath = value; }
            get { return _studentPath; }
        }
        #endregion
    }
}
