using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VATaskWebPart.CreateScheduleByTask
{
    [ToolboxItemAttribute(false)]
    public class CreateScheduleByTask : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/VATaskWebPart/CreateScheduleByTask/CreateScheduleByTaskUserControl.ascx";

        protected override void CreateChildControls()
        {
            CreateScheduleByTaskUserControl  control = Page.LoadControl(_ascxPath) as CreateScheduleByTaskUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region parameters
        private string _assignedTo = "xueqingxia;muyousheng;huangrenjie";
        [WebDisplayName("分配的人员列表")]
        [WebDescription("按人员进行分配")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string AssignedTo
        {
            set { _assignedTo = value; }
            get { return _assignedTo; }
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
        private DateTime startDate = DateTime.Today;
        [WebDisplayName("分配日程的任务开始日期")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        //滚动的天数
        private int _days = 7;
        [WebDisplayName("创建日程从开始日期开始滚动的天数")]
        [WebDescription("")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]

        public int days
        {
            get { return _days ; }
            set { _days = value; }
        }
        #endregion
    }
}
