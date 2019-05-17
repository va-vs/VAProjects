using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPListToExcel.ImportFromExcel
{
    [ToolboxItemAttribute(false)]
    public class ImportFromExcel : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/SPListToExcel/ImportFromExcel/ImportFromExcelUserControl.ascx";

        protected override void CreateChildControls()
        {
            ImportFromExcelUserControl control = Page.LoadControl(_ascxPath) as ImportFromExcelUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region 属性
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
        private string _ListName = "业绩汇总";
        [WebDisplayName("列表名称")]
        [WebDescription("要导入的到的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _subWebUrl = "";
        [WebDisplayName("系部，教师常量表所在的子网站")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string SubWebUrl
        {
            set { _subWebUrl = value; }
            get { return _subWebUrl; }
        }
        private string departList = "系部";
        [WebDisplayName("系部列表名称")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string DepartList
        {
            set { departList = value; }
            get { return departList; }
        }
        private int beforeYears = 3;
        [WebDisplayName("几年前的业绩")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int BeforeYears
        {
            set { beforeYears = value; }
            get { return beforeYears; }
        }
        #endregion
    }
}
