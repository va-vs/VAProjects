using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DeptNews.LeaveList
{
    [ToolboxItemAttribute(false)]
    public class LeaveList : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/DeptNews/LeaveList/LeaveListUserControl.ascx";

        protected override void CreateChildControls()
        {
            LeaveListUserControl control = Page.LoadControl(_ascxPath) as LeaveListUserControl;
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

        private string _wpFeature = "审批";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("审批或待办")]
        [WebDescription("本部件是用于审批筛选还是待办提醒")]
        public string WPFeature
        {
            get { return _wpFeature; }
            set { _wpFeature = value; }
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
        [WebDisplayName("每页条数")]
        [WebDescription("当前部件中每页显示的数据条数")]
        public uint NewsNum
        {
            get { return _NewsNum; }
            set { _NewsNum = value; }
        }


        private string webUrl = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("人事网站地址")]
        [WebDescription("人事子网站地址,只输入英文标题即可,如'Personnel'")]        
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


        private string _userList = "教师花名册";
        [Personalizable]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("教师花名册")]
        [WebDescription("教师信息存储列表名称")]
        public string UserList
        {
            get
            {
                return this._userList;
            }
            set
            {
                this._userList = value;
            }
        }

        private string approveUrl = "";
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        [WebDisplayName("人事网站地址")]
        [WebDescription("人事子网站地址,只输入英文标题即可,如'Personnel'")]
        public string ApproveUrl
        {
            get
            {
                return this.approveUrl;
            }
            set
            {
                this.approveUrl = value;
            }
        }
        #endregion
    }
}
