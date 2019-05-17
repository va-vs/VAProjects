using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ChangePassword.ResetPassword
{
    [ToolboxItemAttribute(false)]
    public class ResetPassword : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/ChangePassword/ResetPassword/ResetPasswordUserControl.ascx";

        protected override void CreateChildControls()
        {
            ResetPasswordUserControl control = Page.LoadControl(_ascxPath) as ResetPasswordUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region 属性
        private  string  accounts= "xueqingxia,muyousheng"  ;
        /// <summary>
        /// 允许操作的帐号
        /// </summary>
        [WebDisplayName("允许操作的帐号")]
        [WebDescription("允许操作的帐号")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string AllowAccounts
        {
            get
            {
                return accounts;
            }
            set
            {
                accounts = value;
            }
        }
        private string pwd = "fsc.12345";
        /// <summary>
        /// 默认的密码
        /// </summary>
        [WebDisplayName("默认设置的密码")]
        [WebDescription("默认设置的密码")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string DefaultPassword
        {
            get
            {
                return pwd;
            }
            set
            {
                pwd = value;
            }
        }
        #endregion
    }
}
