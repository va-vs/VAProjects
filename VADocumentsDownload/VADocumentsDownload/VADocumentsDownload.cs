using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VADocumentsDownload.VADocumentsDownload
{
    [ToolboxItemAttribute(false)]
    public class VADocumentsDownload : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/VADocumentsDownload/VADocumentsDownload/VADocumentsDownloadUserControl.ascx";

        protected override void CreateChildControls()
        {
            VADocumentsDownloadUserControl  control = Page.LoadControl(_ascxPath) as VADocumentsDownloadUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "论文";
        [WebDisplayName("文档库名称")]
        [WebDescription("读取文档库中的文档")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _picType = ".jpg;.png;.bmp;";
        [WebDisplayName("图片库扩展名")]
        [WebDescription("以图片文件单独显示")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string PicType
        {
            set { _picType = value; }
            get { return _picType; }
        }
        private int _picCols = 5;
        [WebDisplayName("图片每行显示几列")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int PicCols
        {
            set { _picCols = value; }
            get { return _picCols; }
        }
        private int _picWidth = 80;
        [WebDisplayName("图片的宽")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int PicWidth
        {
            set { _picWidth = value; }
            get { return _picWidth; }
        }
        private int _picHeigh = 80;
        [WebDisplayName("图片的高")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int PicHeigh
        {
            set { _picHeigh = value; }
            get { return _picHeigh; }
        }
        #endregion
    }
}
