using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ActivityStastics.ActivityStastics
{
    [ToolboxItemAttribute(false)]
    public class ActivityStastics : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/ActivityStastics/ActivityStastics/ActivityStasticsUserControl.ascx";

        protected override void CreateChildControls()
        {
            ActivityStasticsUserControl control = Page.LoadControl(_ascxPath) as  ActivityStasticsUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "个人学习助手";
        [WebDisplayName("统计的表")]
        [WebDescription("统计的列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _RelListName = "操作";
        [WebDisplayName("关联的表")]
        [WebDescription("关联的列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string RelationListName
        {
            set { _RelListName = value; }
            get { return _RelListName; }
        }
        #endregion

        #region 自定义设置

        private string _routinePlanList = "例行计划";
        [Personalizable, WebDisplayName("例行计划列表名称"), WebDescription("例行计划列表名称，如：例行计划"), WebBrowsable, Category("自定义设置")]
        public string RoutinePlanList
        {
            get
            {
                return this._routinePlanList;
            }
            set
            {
                this._routinePlanList = value;
            }
        }

        private string _planList = "个人学习助手";
        [Personalizable, WebDisplayName("助手列表名称"), WebDescription("个人学习助手列表名称，如：个人学习助手"), WebBrowsable, Category("自定义设置")]
        public string PlanList
        {
            get
            {
                return this._planList;
            }
            set
            {
                this._planList = value;
            }
        }

        private string _resultList = "个人学习助手结果";
        [Personalizable, WebDisplayName("结果列表名称"), WebDescription("结果列表名称，如：个人学习助手结果"), WebBrowsable, Category("自定义设置")]
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


        private string _actionList = "操作";
        [Personalizable, WebDisplayName("操作列表名称"), WebDescription("操作列表名称，如：操作"), WebBrowsable, Category("自定义设置")]
        public string ActionList
        {
            get
            {
                return this._actionList;
            }
            set
            {
                this._actionList = value;
            }
        }

        private string _actionTypeList = "操作类别";
        [Personalizable, WebDisplayName("操作类别列表名称"), WebDescription("操作类别列表名称，如：操作类别"), WebBrowsable, Category("自定义设置")]
        public string ActionTypeList
        {
            get
            {
                return this._actionTypeList;
            }
            set
            {
                this._actionTypeList = value;
            }
        }

        private string _logList = "例行计划生成历史";
        [Personalizable, WebDisplayName("例行计划生成历史"), WebDescription("例行计划生成历史列表名称，如：个人学习助手"), WebBrowsable, Category("自定义设置")]
        public string LogList
        {
            get
            {
                return this._logList;
            }
            set
            {
                this._logList = value;
            }
        }

        private string _siteUrl = "http://ws2018:19568/";
        [Personalizable, WebDisplayName("网址"), WebDescription("网站地址，如：http://va.neu.edu.cn"), WebBrowsable, Category("自定义设置")]
        public string SiteUrl
        {
            get
            {
                return this._siteUrl;
            }
            set
            {
                this._siteUrl = value;
            }
        }

       
        

        #endregion
    }
}
