using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DeptNews.DeptNews
{
    [ToolboxItemAttribute(false)]
    public class DeptNews : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/DeptNews/DeptNews/DeptNewsUserControl.ascx";

        protected override void CreateChildControls()
        {
            //Control control = Page.LoadControl(_ascxPath);
            DeptNewsUserControl control = Page.LoadControl(_ascxPath) as DeptNewsUserControl;
            if (control != null)
                control.WebPartObj = this;
            Controls.Add(control);
        }
        #region 设置属性
        private string _ListName = "新闻公告";
        [WebDisplayName("列表名称")]
        [WebDescription("要查阅的列表名称 (例如：新闻)")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }            
        }

        private string _DeptName = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("系部名称")]
        [WebDescription("要筛选的系别名称，由系别网站名称确定 (例如：英语系)")]
        public string DeptName
        {
            get { return _DeptName; }
            set { _DeptName = value; }
        }

        private string _SortString = "系部";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("筛选条件")]
        [WebDescription("筛选列表的条件，比如按系部筛选，则输入“系部”")]
        public string SortString
        {
            get { return _SortString; }
            set { _SortString = value; }
        }

        private string _ModerationUrl = SPContext.Current.Site.RootWeb.Url+ "/pages/Moderation.aspx";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("审批页面URL")]
        [WebDescription("审批页面的地址")]
        public string ModerationUrl
        {
            get { return _ModerationUrl; }
            set { _ModerationUrl = value; }
        }

        private string _SiteUrl = SPContext.Current.Site.RootWeb.Url;
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

        private uint _NewsNum = 5;
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("每页新闻条数")]
        [WebDescription("当前部件中每页显示的新闻条数")]
        public uint NewsNum
        {
            get { return _NewsNum; }
            set { _NewsNum = value; }
        }
        private string webUrl = "NewsEdit";
        [Personalizable, WebDisplayName("子网站地址"), WebDescription("创建新闻列表所在的子网站地址"), WebBrowsable]
        public string WebUrl
        {
            get
            {
                return this.webUrl;
            }
            set
            {
                this.webUrl = value;
            }
        }
        private string newsList = "新闻";      
            
       [Personalizable, WebDisplayName("子网站的新闻名称"), WebDescription("创建新闻列表所在的子网站地址"), WebBrowsable]
        public string NewsList
        {
            get
            {
                return this.newsList;
            }
            set
            {
                this.newsList = value;
            }
        }
        #endregion
    }
}
