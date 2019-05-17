using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace LeaveApproval.Appraval
{
    [ToolboxItemAttribute(false)]
    public class Appraval : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/LeaveApproval/Appraval/AppravalUserControl.ascx";

        protected override void CreateChildControls()
        {
            AppravalUserControl control = Page.LoadControl(_ascxPath) as AppravalUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "请假申请";
        [WebDisplayName("请假")]
        [WebDescription("要审批的请假名单")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
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
        private string _action = "审批";
        [Personalizable, WebDisplayName("对请假申请进行的操作"), WebDescription("对请假申请时行的操作"), WebBrowsable, Category("设置")]
        public string CurrentAction
        {
            get
            {
                return this._action;
            }
            set
            {
                this._action = value;
            }
        }
        private string _result = "批准";
        [Personalizable, WebDisplayName("审批的结果"), WebDescription("审批返回的结果"), WebBrowsable, Category("设置")]
        public string ReturnResult
        {
            get
            {
                return this._result;
            }
            set
            {
                this._result = value;
            }
        }
        private string _retUrl = "/blog/Lists/Posts/AllPosts.aspx";

        [Personalizable, WebDisplayName("返回的网址"), WebDescription(""), WebBrowsable, Category("设置")]
        public string RetUrl
        {
            get
            {
                return this._retUrl;
            }
            set
            {
                this._retUrl = value;
            }
        }
        private string teacherListWebName = "";
        [WebDisplayName("教师花名册所在的子网站")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string TeacherListWebName
        {
            set { teacherListWebName = value; }
            get { return teacherListWebName; }
        }
        private string teacherList = "教师花名册";
        [WebDisplayName("教师列表名称")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string TeacherList
        {
            set { teacherList = value; }
            get { return teacherList; }
        }
        private string biaoXiaoOptionTitle = "报销选项：";
        [WebDisplayName("标题")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string BiaoXiaoOptionTitle
        {
            set { biaoXiaoOptionTitle = value; }
            get { return biaoXiaoOptionTitle; }
        }
        #endregion
    }

}
