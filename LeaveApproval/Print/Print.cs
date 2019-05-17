using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace LeaveApproval.Print
{
    [ToolboxItemAttribute(false)]
    public class Print : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/LeaveApproval/Print/PrintUserControl.ascx";

        protected override void CreateChildControls()
        {
            PrintUserControl control = Page.LoadControl(_ascxPath) as PrintUserControl;
            if (control != null)
                control.WebPartObj = this;
            Controls.Add(control);
        }
        #region 设置属性
        private string _ListName = "请假申请";
        [WebDisplayName("请假申请表")]
        [WebDescription("请假申请的列表名称 (例如：请假申请)")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }

        private string _resultList = "请假审批";
        [Personalizable, WebDisplayName("请假审批表"), WebDescription("审批记录列表的名称"), WebBrowsable, Category("设置")]
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

        private string _WebUrl = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("网站地址")]
        [WebDescription("列表所在的网站绝对地址")]
        public string WebUrl
        {
            get { return _WebUrl; }
            set { _WebUrl = value; }
        }


        private string _SignImgLib = "签字图片库";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("签字图片库")]
        [WebDescription("签字图片所在的图片库名称")]
        public string SignImgLib
        {
            get { return _SignImgLib; }
            set { _SignImgLib = value; }
        }
        
        #endregion
    }
}
