using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace CreativeAppraise.Appraise
{
    [ToolboxItemAttribute(false)]
    public class Appraise : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/CreativeAppraise/Appraise/AppraiseUserControl.ascx";

        protected override void CreateChildControls()
        {
            AppraiseUserControl  control = Page.LoadControl(_ascxPath) as AppraiseUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "创意";//主表
        [WebDisplayName("列表名称")]
        [WebDescription("要评审的列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }

       
        private string _resultList = "创意评审";//子表

        [Personalizable, WebDisplayName("评审结果"), WebDescription(""), WebBrowsable, Category("设置")]
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
        private string _timeList = "比赛日程";//日程表

        [Personalizable, WebDisplayName("比赛日程时间"), WebDescription("记录比赛"), WebBrowsable,Category("设置")]
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
        //统计字数的字段
        private string _body = "正文";//

        [Personalizable, WebDisplayName("统计字数的字段"), WebDescription("字段名称"), WebBrowsable, Category("设置")]
        public string BodyField
        {
            get
            {
                return this._body;
            }
            set
            {
                this._body = value;
            }
        }
        private string _Attention = "注意事项";

        [Personalizable, WebDisplayName("注意事项"), WebDescription("表单填写注意事项,包含各项内容规定等"), WebBrowsable, Category("设置")]
        public string Attention
        {
            get
            {
                return this._Attention;
            }
            set
            {
                this._Attention = value;
            }
        }
        private int _score = 100;
        [Personalizable, WebDisplayName("最高分值"), WebDescription("评分制"), WebBrowsable, Category("设置")]
        public int Score
        {
            get
            {
                return this._score;
            }
            set
            {
                this._score = value;
            }
        }
        private int _appraiseNum = 100;
        [Personalizable, WebDisplayName("评语字数"), WebDescription(""), WebBrowsable, Category("设置")]
        public int AppraiseNum
        {
            get
            {
                return this._appraiseNum;
            }
            set
            {
                this._appraiseNum = value;
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
        #endregion
    }
}
