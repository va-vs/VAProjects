using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AssistantResults.AddAttachments
{
    [ToolboxItemAttribute(false)]
    public class AddAttachments : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/AssistantResults/AddAttachments/AddAttachmentsUserControl.ascx";

        protected override void CreateChildControls()
        {
            AddAttachmentsUserControl control = Page.LoadControl(_ascxPath) as AddAttachmentsUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        # region 属性
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
        #endregion
    }
}
