using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AssistantResults.AssistantResults
{
    [ToolboxItemAttribute(false)]
    public class AssistantResults : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/AssistantResults/AssistantResults/AssistantResultsUserControl.ascx";

        protected override void CreateChildControls()
        {
            AssistantResultsUserControl control = Page.LoadControl(_ascxPath) as AssistantResultsUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "活动";
        [WebDisplayName("活动对应的列表")]
        [WebDescription("活动对应的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _ListMediaRel = "活动媒体";
        [WebDisplayName("附件库与列表的对应关系")]
        [WebDescription("附件库与列表的对应关系")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListMediaRel
        {
            set { _ListMediaRel = value; }
            get { return _ListMediaRel; }
        }
        private string _ListMediaLib = "活动媒体库";
        [WebDisplayName("附件库所在的列表")]
        [WebDescription("附件库所在的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListMediaLib
        {
            set { _ListMediaLib = value; }
            get { return _ListMediaLib; }
        }
        private string _title = "结果";
        [WebDisplayName("显示的标题")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ShowTitle
        {
            set { _title = value; }
            get { return _title; }
        }
        private string _action = "Edit";
        [WebDisplayName("是编辑(Edit)还是显示（Disp）")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string Action
        {
            set { _action = value; }
            get { return _action; }
        }
        private string _webRelativeUrl = "blog";
        [WebDisplayName("文档库所在的子网站")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string WebRelativeUrl
        {
            set { _webRelativeUrl = value; }
            get { return _webRelativeUrl; }
        }
        private string lnkDescription = @"JPEG,JPG,BMP,GIF,PNG,mp3,mp4,wmv,wma,wav";
        [WebDisplayName("支持上传的文件类型")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string LnkDescription
        {
            set { lnkDescription = value; }
            get { return lnkDescription; }
        }
        #endregion
    }
}
