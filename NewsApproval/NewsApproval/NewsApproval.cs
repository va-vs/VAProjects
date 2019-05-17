using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace NewsApproval.NewsApproval
{
    [ToolboxItemAttribute(false)]
    public class NewsApproval : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/NewsApproval/NewsApproval/NewsApprovalUserControl.ascx";

        protected override void CreateChildControls()
        {
            //Control control = Page.LoadControl(_ascxPath);
            NewsApprovalUserControl control = Page.LoadControl(_ascxPath) as NewsApprovalUserControl;
            if (control != null)
                control.webpartObj  = this;

            Controls.Add(control);
        }
        #region 方法

        #endregion
        #region 属性
        /// <summary>
        /// 用户指定新闻列表名称
        /// </summary>
        string _ListName = "新闻公告";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻列表名称")]
        [WebDescription("新闻列表的名称 (例如：新闻)")]
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }
        /// <summary>
        /// 用户指定新闻列表类别列表
        /// </summary>
        string _typeName = "新闻类别";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻类别名称")]
        [WebDescription("新闻类别列表的名称 ")]
        public string TypeListName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }
        #endregion
    }
}
