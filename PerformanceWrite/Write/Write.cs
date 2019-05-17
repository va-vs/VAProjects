using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace PerformanceWrite.Write
{
    [ToolboxItemAttribute(false)]
    public class Write : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/PerformanceWrite/Write/WriteUserControl.ascx";

        protected override void CreateChildControls()
        {
            WriteUserControl control = Page.LoadControl(_ascxPath) as WriteUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }

       
        #region "参数设置"
        private string _FromList = "论文";
        [WebDisplayName("业绩主表")]
        [WebDescription("父列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string FromList
        {
            set { _FromList = value; }
            get { return _FromList; }
        }
        private string _resultList = "论文业绩";
        [Personalizable, WebDisplayName("业绩子表"), WebDescription(""), WebBrowsable, Category("设置")]
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
        private int _totalRows=10;
        [Personalizable, WebDisplayName("默认的业绩分配总行数"), WebDescription("去掉了新建功能"), WebBrowsable, Category("设置")]
        public int TotolRows
        {
            get
            {
                return _totalRows;
            }
            set
            {
                _totalRows = value;
            }
        }
        private string _action = "录入";
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
        private string _writedListTitle = "分配记录：";
        [Personalizable, WebDisplayName("显示的名称"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string AuthorListTitle 
        {
            get
            {
                return this._writedListTitle;
            }
            set
            {
                this._writedListTitle = value;
            }
        }
        private string  fromRatioField = "当量";
        [Personalizable, WebDisplayName("主表中当前的字段名,系数之和不能超过这个字段的值"), WebDescription("这个现在不可"), WebBrowsable, Category("设置")]
        public string FromRatioField
        {
            get
            {
                return this.fromRatioField;
            }
            set
            {
                this.fromRatioField = value;
            }
        }
        private string _showMsgAuthor = " 干系人不能重复，请重新分配！";
        [Personalizable, WebDisplayName("业绩干系人重复提示信息"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string ShowMsgAuthor
        {
            get
            {
                return this._showMsgAuthor;
            }
            set
            {
                this._showMsgAuthor = value;
            }
        }
        private string _showMsg = " 认领超限(最大可分N)";
        [Personalizable, WebDisplayName("业绩超限提示信息"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string ShowMsg
        {
            get
            {
                return this._showMsg;
            }
            set
            {
                this._showMsg = value;
            }
        }
        private string _showMsgRepeat = "该教师已分配业绩，不能重复分配";
        [Personalizable, WebDisplayName("业绩分配重复提示信息"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string  ShowMsgRepeat
        {
            get
            {
                return this._showMsgRepeat;
            }
            set
            {
                this._showMsgRepeat = value;
            }
        }
        private string _title = "业绩系数分配";
        [Personalizable, WebDisplayName("标题信息"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string ShowTitle
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
            }
        }
        private string _userDesp = "点击保存，保存修改或新建的业绩分配；业绩分配按人数进行分配。";
        [Personalizable, WebDisplayName("使用说明信息"), WebDescription(" "), WebBrowsable, Category("设置")]
        public string UserDesp
        {
            get
            {
                return this._userDesp;
            }
            set
            {
                this._userDesp = value;
            }
        }
        #endregion
    }
}
