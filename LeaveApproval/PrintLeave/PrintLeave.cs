using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace LeaveApproval.PrintLeave
{
    [ToolboxItemAttribute(false)]
    public class PrintLeave : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/LeaveApproval/PrintLeave/PrintLeaveUserControl.ascx";

        protected override void CreateChildControls()
        {
            PrintLeaveUserControl  control = Page.LoadControl(_ascxPath) as PrintLeaveUserControl ;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "请假申请";
        [WebDisplayName("请假")]
        [WebDescription("要审批的请假名单")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private string _wordfileName = "请假条.docx";
        [WebDisplayName("生成的文件名")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string WordSavefile
        {
            set { _wordfileName = value; }
            get { return _wordfileName; }
        }

        private string _TempfileName = "请假条模板.docx";
        [WebDisplayName("模版文件名")]
        [WebDescription(" ")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string WordTempfile
        {
            set { _TempfileName = value; }
            get { return _TempfileName; }
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
