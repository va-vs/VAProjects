using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VATaskWebPart.ImportFromExcel
{
    [ToolboxItemAttribute(false)]
    public class ImportFromExcel : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/VATaskWebPart/ImportFromExcel/ImportFromExcelUserControl.ascx";

        protected override void CreateChildControls()
        {
            ImportFromExcelUserControl control = Page.LoadControl(_ascxPath) as ImportFromExcelUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "个人计划助手";
        [WebDisplayName("列表名称")]
        [WebDescription("要导入的请假名单")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string subWebName = "";
        [WebDisplayName("导入哪个子网站的列表")]
        [WebDescription("子网站Url,空为要网站")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string WebUrl
        {
            set { subWebName = value; }
            get { return subWebName; }
        }

        #endregion
    }
}
