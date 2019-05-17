using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FSCWebParts.ListItemNav
{
    [ToolboxItemAttribute(false)]
    public class ListItemNav : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/FSCWebParts/ListItemNav/ListItemNavUserControl.ascx";

        protected override void CreateChildControls()
        {
            ListItemNavUserControl control = Page.LoadControl(_ascxPath) as ListItemNavUserControl;
            if (control != null)
                control.webpartObj = this;
            Controls.Add(control);
        }

        #region 属性
        /// <summary>
        /// 用户指定列表名称
        /// </summary>
        string _ListName = "";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("列表名称")]
        [WebDescription("指定要实现上下文导航的列表的名称")]
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }

        /// <summary>
        /// 用户指定排序字段
        /// </summary>
        string _SortField = "";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("排序字段")]
        [WebDescription("指定上下文导航的排序字段显示名称")]
        public string SortField
        {
            get { return _SortField; }
            set { _SortField = value; }
        }

        /// <summary>
        /// 用户指定排序方式
        /// </summary>
        string _SortDirections = "0";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("排序方式")]
        [WebDescription("指定上下文导航的排序方式:0是升序,1是降序")]
        public string SortDirections
        {
            get { return _SortDirections; }
            set { _SortDirections = value; }
        }

        /// <summary>
        /// 是否只显示待审核
        /// </summary>
        string _OnlyAudit = "否";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("是否只显示待审核")]
        [WebDescription("指定上下文导航中是否只显示待审核的数据记录")]
        public string OnlyAudit
        {
            get { return _OnlyAudit; }
            set { _OnlyAudit = value; }
        }

        /// <summary>
        /// 指定显示数据年份
        /// </summary>
        string _ByYear = "";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("指定显示数据年份")]
        [WebDescription("指定筛选要显示的数据年份,不填表示当前年份")]
        public string ByYear
        {
            get { return _ByYear; }
            set { _ByYear = value; }
        }

        ///// <summary>
        ///// 是否只显示当前用户记录
        ///// </summary>
        //string _OnlyMy = "否";
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("是否只显示当前用户记录")]
        //[WebDescription("指定上下文导航中是否只显示当前用户的数据记录")]
        //public string OnlyMy
        //{
        //    get { return _OnlyMy; }
        //    set { _OnlyMy = value; }
        //}




        ///// <summary>
        ///// 用户指定排序方式
        ///// </summary>
        //string _MyField = "";
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("用户归属栏名")]
        //[WebDescription("指定哪个栏可以确认记录归属本人所有")]
        //public string MyField
        //{
        //    get { return _MyField; }
        //    set { _MyField = value; }
        //}
        #endregion
    }
}
