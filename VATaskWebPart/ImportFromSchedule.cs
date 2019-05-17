using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VATaskWebPart.ImportFromSchedule
{
    [ToolboxItemAttribute(false)]
    public class ImportFromSchedule : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/VATaskWebPart/ImportFromSchedule/ImportFromScheduleUserControl.ascx";

        protected override void CreateChildControls()
        {
            ImportFromScheduleUserControl control = Page.LoadControl(_ascxPath) as ImportFromScheduleUserControl ;
            if (control != null)
                control.objWeb = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "个人学习助手";
        [WebDisplayName("列表名称")]
        [WebDescription("要导入到的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _schedule = "项目日程";
        [WebDisplayName("列表名称")]
        [WebDescription("要导入到列表的源列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string SchedulelList
        {
            set { _schedule = value; }
            get { return _schedule; }
        }
        private string _task = "任务";
        [WebDisplayName("列表名称")]
        [WebDescription("日程所对应的任务列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string TaskList
        {
            set { _task = value; }
            get { return _task; }
        }
        #endregion
    }
}
