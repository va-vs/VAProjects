using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace OnlineVote.OnlineVote
{
    [ToolboxItemAttribute(false)]
    public class OnlineVote : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/OnlineVote/OnlineVote/OnlineVoteUserControl.ascx";

        protected override void CreateChildControls()
        {
            OnlineVoteUserControl control = Page.LoadControl(_ascxPath) as OnlineVoteUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _ListName = "创意";
        [WebDisplayName("列表名称")]
        [WebDescription("要投票的列表名称")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }


        private string _resultList = "投票明细";

        [Personalizable, WebDisplayName("投票明细"), WebDescription(""), WebBrowsable, Category("设置")]
        public string VoteDetail
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
        #endregion
    }
}
