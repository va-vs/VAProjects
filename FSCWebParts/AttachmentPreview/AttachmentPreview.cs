using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FSCWebParts.AttachmentPreview
{
    [ToolboxItemAttribute(false)]
    public class AttachmentPreview : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/FSCWebParts/AttachmentPreview/AttachmentPreviewUserControl.ascx";

        protected override void CreateChildControls()
        {
            AttachmentPreviewUserControl control = Page.LoadControl(_ascxPath) as AttachmentPreviewUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }

        #region 自定义设置
        /// <summary>
        /// 列表名称
        /// </summary>
        private string _listName = "作品";
        [Personalizable, WebDisplayName("列表名称"), WebDescription("列表名称，如:作品"), WebBrowsable, Category("自定义设置")]
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

        /// <summary>
        /// Office Online Server服务器地址
        /// </summary>
        private string _ooServer = "http://ismart.neu.edu.cn/";
        [Personalizable, WebDisplayName("Office Online Server服务器地址"), WebDescription("Office Online Server服务器地址"), WebBrowsable, Category("自定义设置")]
        public string OOServer
        {
            get
            {
                return this._ooServer;
            }
            set
            {
                this._ooServer = value;
            }
        }

        /// <summary>
        /// 列表名称
        /// </summary>
        private int _width = 600;
        [Personalizable, WebDisplayName("显示宽度"), WebDescription("列表名称，如:600"), WebBrowsable, Category("自定义设置")]
        public int DispWidth
        {
            get
            {
                return this._width;
            }
            set
            {
                this._width = value;
            }
        }

        /// <summary>
        /// 列表名称
        /// </summary>
        private int _height = 450;
        [Personalizable, WebDisplayName("显示宽度"), WebDescription("列表名称，如:450"), WebBrowsable, Category("自定义设置")]
        public int DispHeight
        {
            get
            {
                return this._height;
            }
            set
            {
                this._height = value;
            }
        }
        #endregion
    }
}
