using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace PersonalRemind.remind
{
    /// <summary>
    /// 提醒
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class remind : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/PersonalRemind/remind/remindUserControl.ascx";

        protected override void CreateChildControls()
        {
            remindUserControl control = Page.LoadControl(_ascxPath) as remindUserControl;
            if (control!=null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        #region 自定义设置
        private string _chkItem = "3H值;积分;作品数;知识量;互动值;专注度";

        [Personalizable, WebDisplayName("检查提醒项目"), WebDescription("需要检查提醒个人状态指标项目，多项用;隔开"), WebBrowsable, Category("自定义设置")]
        public string ChkItem
        {
            get
            {
                return this._chkItem;
            }
            set
            {
                this._chkItem = value;
            }
        }

        private string _listName = "个人状态指标";
        [Personalizable, WebDisplayName("列表名称"), WebDescription("存储个人状态指标的列表名称"), WebBrowsable, Category("自定义设置")]
        public string ListName
        {
            get
            {
                return this._listName;
            }
            set
            {
                this._listName = value;
            }
        }

        private string _webUrl = "http://va.neu.edu.cn/Projects/VAExtension/";
        [Personalizable, WebDisplayName("网站路径"), WebDescription("包含个人状态指标的网站路径"), WebBrowsable, Category("自定义设置")]
        public string WebUrl
        {
            get
            {
                return this._webUrl;
            }
            set
            {
                this._webUrl = value;
            }
        }
        #endregion
    }

}
