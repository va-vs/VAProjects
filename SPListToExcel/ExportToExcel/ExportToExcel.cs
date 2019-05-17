using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SPListToExcel.ExportToExcel
{
    [ToolboxItemAttribute(false)]
    public class ExportToExcel : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/SPListToExcel/ExportToExcel/ExportToExcelUserControl.ascx";

        protected override void CreateChildControls()
        {
            ExportToExcelUserControl control = Page.LoadControl(_ascxPath) as ExportToExcelUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"

        private string _TempfileName = "业绩导出模版.xlsm";
        [WebDisplayName("模版文件名")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ExcelTempfile
        {
            set { _TempfileName = value; }
            get { return _TempfileName; }
        }

        private string _fileName = "教师业绩.xlsm";
        [WebDisplayName("导出到Excel默认的文件名")]
        [WebDescription("默认的文件名")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ExcelFilename
        {
            set { _fileName = value; }
            get { return _fileName; }
        }

        private string exportLists = "本科教学理论课时；本科教学实践课时；指导毕业论文；新开课；研究生教学理论课时；指导研究生；研究生批卷；科研立项；教学立项；科研成果；论文；专著；译著；教材；教学奖励；教学竞赛；管理工作；职称；学科建设；人才引进；学术兼职；加减分项";
        [WebDisplayName("导出的列表名称")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ExportLists
        {
            set { exportLists = value; }
            get { return exportLists; }
        }
        private string _ratioCaption = "点数";
        [WebDisplayName("导出的业绩点的列名称")]
        [WebDescription("Excel文件中的列名")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string RatioCaption
        {
            set { _ratioCaption = value; }
            get { return _ratioCaption; }
        }

        private string _subWebUrl = "";
        [WebDisplayName("系部，教师花名册所在的子网站，不填表示根网站")]
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
        [WebDescription("指定系部列表的名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string DepartList
        {
            set { departList = value; }
            get { return departList; }
        }

        private string teacherList = "教师花名册";
        [WebDisplayName("教师列表名称")]
        [WebDescription("教师花名册或列表的名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string TeacherList
        {
            set { teacherList = value; }
            get { return teacherList; }
        }

        private int repeartColumn =3;
        [WebDisplayName("列数")]
        [WebDescription("部件中多选控件数据项的列数")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int RepeatCol
        {
            set { repeartColumn = value; }
            get { return repeartColumn; }
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

        private short fontsize = 10;
        [WebDisplayName("导出的字体大小")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public short  FontSize
        {
            set { fontsize = value; }
            get { return fontsize; }
        }

        private string noDataMsg = "没有要导出的数据！";
        [WebDisplayName("没有数据提示文本")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string NoDataMsg
        {
            set { noDataMsg = value; }
            get { return noDataMsg; }
        }
        #endregion
    }
}
