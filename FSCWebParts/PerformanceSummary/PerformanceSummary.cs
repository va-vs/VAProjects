using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FSCWebParts.PerformanceSummary
{
    [ToolboxItemAttribute(false)]
    public class PerformanceSummary : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/FSCWebParts/PerformanceSummary/PerformanceSummaryUserControl.ascx";

        protected override void CreateChildControls()
        {
            PerformanceSummaryUserControl control = Page.LoadControl(_ascxPath) as PerformanceSummaryUserControl;
            if (control!=null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        #region 自定义设置
        /// <summary>
        /// 列表名称
        /// </summary>
        private string _listName = "业绩汇总";
        [Personalizable, WebDisplayName("列表名称"), WebDescription("列表名称，如：业绩汇总"), WebBrowsable, Category("自定义设置")]
        public string ListName
        {
            get
            {
                return this._listName;
            }
            set
            {
                this._listName = value;
            }
        }

        private string _webUrl = "";
        [Personalizable, WebDisplayName("列表所在网站地址"), WebDescription("列表名称，如：http://www.fsc.neu.edu.cn/Performance"), WebBrowsable, Category("自定义设置")]
        public string WebUrl
        {
            get
            {
                return this._webUrl;
            }
            set
            {
                this._webUrl = value;
            }
        }

        private string _byYear = DateTime.Now.Year.ToString();
        [Personalizable, WebDisplayName("年度"), WebDescription("指定年度，如：2018"), WebBrowsable, Category("自定义设置")]
        public string ByYear
        {
            get
            {
                return this._byYear;
            }
            set
            {
                this._byYear = value;
            }
        }

        public string _CollateDate = "绩效日程";
        [Personalizable, WebDisplayName("绩效日程列表名称"), WebDescription("绩效日程保存的列表"), WebBrowsable, Category("自定义设置")]
        public string CollateDate
        {
            get
            {
                return this._CollateDate;
            }
            set
            {
                this._CollateDate = value;
            }
        }


        private string _collate="校对管理员";
        [Personalizable, WebDisplayName("校对管理员"), WebDescription("校对管理员账户列表"), WebBrowsable, Category("自定义设置")]
        public string Collater
        {
            get
            {
                return this._collate;
            }
            set
            {
                this._collate = value;
            }
        }
        #endregion

    }
}
