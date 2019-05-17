using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace LeaveApproval.PrintTable
{
    [ToolboxItemAttribute(false)]
    public class PrintTable : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/LeaveApproval/PrintTable/PrintTableUserControl.ascx";

        protected override void CreateChildControls()
        {
            PrintTableUserControl control = Page.LoadControl(_ascxPath) as PrintTableUserControl;
            if (control != null)
                control.WebPartObj = this;
            Controls.Add(control);
        }

        #region 设置属性
        private string _ListName = "请假申请";
        [WebDisplayName("列表名称")]
        [WebDescription("要查阅的列表名称 (例如：请假申请)")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }

        private string _resultList = "请假审批";
        [Personalizable, WebDisplayName("审核结果"), WebDescription(""), WebBrowsable, Category("设置")]
        public string ResultList
        {
            get
            {
                return this._resultList;
            }
            set
            {
                this._resultList = value;
            }
        }

        private string _SiteUrl = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("网站地址")]
        [WebDescription("列表所在的网站绝对地址")]
        public string SiteUrl
        {
            get { return _SiteUrl; }
            set { _SiteUrl = value; }
        }

        private string _ImgUrl = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("网站地址")]
        [WebDescription("列表所在的网站绝对地址")]
        public string ImgUrl
        {
            get { return _ImgUrl; }
            set { _ImgUrl = value; }
        }
        #endregion

    }
}
