using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UploadAttachment.PerformAttachment
{
    [ToolboxItemAttribute(false)]
    public class PerformAttachment : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/UploadAttachment/PerformAttachment/PerformAttachmentUserControl.ascx";

        protected override void CreateChildControls()
        {
            PerformAttachmentUserControl control = Page.LoadControl(_ascxPath) as PerformAttachmentUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "论文";
        [WebDisplayName("附件库对应的列表")]
        [WebDescription("附件库对应的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _title= "附件库";
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
        #endregion
    }
}
