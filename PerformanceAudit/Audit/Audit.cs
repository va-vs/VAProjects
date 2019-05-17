using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace PerformanceAudit.Audit
{
    [ToolboxItemAttribute(false)]
    public class Audit : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/PerformanceAudit/Audit/AuditUserControl.ascx";

        protected override void CreateChildControls()
        {
            AuditUserControl control = Page.LoadControl(_ascxPath) as AuditUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "论文";
        [WebDisplayName("业绩主表")]
        [WebDescription("要评审的列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _resultList = "论文业绩";
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
        private string _timeList = "绩效日程";
        [Personalizable, WebDisplayName("绩效日程日程时间"), WebDescription("记录绩效日程日程时间"), WebBrowsable, Category("设置")]
        public string TimeList
        {
            get
            {
                return this._timeList;
            }
            set
            {
                this._timeList = value;
            }
        }
       private string _action = "审核";
        [Personalizable, WebDisplayName("当前时间段要进行的操作"), WebDescription("要进行什么操作通用性"), WebBrowsable, Category("设置")]
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
        private string _result = "通过";
        [Personalizable, WebDisplayName("操作返回的结果"), WebDescription("返回的结果"), WebBrowsable, Category("设置")]
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
        private string _chkItems = "论文地址；作者；所属类别";

        [Personalizable, WebDisplayName("需要审核的内容"), WebDescription(""), WebBrowsable, Category("设置")]
        public string ChkItems
        {
            get
            {
                return this._chkItems;
            }
            set
            {
                this._chkItems = value;
            }
        }

        private string _chkDesc = "论文地址是否真实有效;作者是否有效；类别是否正确";

        [Personalizable, WebDisplayName("审核项目说明"), WebDescription("对应审核的项目编写每项的说明,多项用英文分号';'隔开"), WebBrowsable, Category("设置")]
        public string ChkDesc
        {
            get
            {
                return this._chkDesc;
            }
            set
            {
                this._chkDesc = value;
            }
        }

        private string _attachmentField = "论文地址";

        [Personalizable, WebDisplayName("与附件相关的字段名称，在审核时，如果空，则找对应的附件库"), WebDescription(""), WebBrowsable, Category("设置")]
        public string AttachmentField
        {
            get
            {
                return this._attachmentField;
            }
            set
            {
                this._attachmentField = value;
            }
        }
        #endregion
    }
}