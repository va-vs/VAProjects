using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VAStateIndicator.StateIndicator
{
    [ToolboxItemAttribute(false)]
    public class StateIndicator : WebPart, IWebPartTable
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/VAStateIndicator/StateIndicator/StateIndicatorUserControl.ascx";

        protected override void CreateChildControls()
        {
            StateIndicatorUserControl control = Page.LoadControl(_ascxPath) as StateIndicatorUserControl;
            if (control != null)
                control.webObj = this;
            Controls.Add(control);
        }
        #region "参数"
        private string _IndicatorListName = "指标";
        [WebDisplayName("指标列表名称")]
        [WebDescription("记录指标的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string IndicatorListName
        {
            set { _IndicatorListName = value; }
            get { return _IndicatorListName; }
        }
        //private string _IndicatorRuleListName = "指标变化规则";
        //[WebDisplayName("指标列表名称")]
        //[WebDescription("记录指标规则的列表")]
        //[Personalizable(true)]
        //[WebBrowsable(true)]
        //[Category("设置")]
        //public string IndicatorRuleListName
        //{
        //    set { _IndicatorRuleListName = value; }
        //    get { return _IndicatorRuleListName; }
        //}
        private string _ListName = "个人状态指标";
        [WebDisplayName("列表名称")]
        [WebDescription("记录用户指标的列表")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public string ListName
        {
            set { _ListName = value; }
            get { return _ListName; }
        }
        private int  _calDays = 7;
        [WebDisplayName("统计指标趋势的天数")]
        [WebDescription("统计指标趋势的天数，默认为7天")]
        [Personalizable(true)]
        [WebBrowsable(true)]
        [Category("设置")]
        public int CalDays
        {
            set { _calDays = value; }
            get { return _calDays; }
        }

        #endregion
        #region 获取指标变化趋势曲线
        /// <summary>
        /// 获取我的指标（从个人状态指标列表中读取）
        /// </summary>
        /// <returns></returns>
        private void ChartTableData()
        {
            DataTable dtIndex = StateIndicatorUserControl.GetListData(IndicatorListName);//指标列表
            //趋势表格
            _table = new DataTable();
            _table.Columns.Add("Title", typeof(string));
            foreach (DataRow dr in dtIndex.Rows)//找下面的指标规则
            {
                string indexTitle = dr["Title"].ToString();
                _table.Columns.Add(new DataColumn(indexTitle, typeof(int)));

            }
            long userID = StateIndicatorUserControl.GetAuthorID();

            DataTable dtActivity = StateIndicatorUserControl.GetActivityByUser(ListName, userID);//获当前用户的所有状态指标
            if (userID == 0 || dtActivity == null) return;
            //获取n天之前的数据，天数作为参数

            DateTime dtFrom = DateTime.Today .AddDays(-CalDays);
            DateTime dtTo = DateTime.Today .AddDays(1);
            //按每天进行统计
            while (dtFrom < dtTo)
            {
                DataRow[] drsActivty = dtActivity.Select("JudgeDate>='" + dtFrom.ToShortDateString() + "' and JudgeDate<'" + dtFrom.AddDays(1).ToShortDateString() + "'");
                DataRow drNew = _table.NewRow();
                drNew["Title"] = dtFrom.ToString("M-d");//按标题进行统计
                if (drsActivty.Length > 0)
                {
                    foreach (DataRow dr in drsActivty)
                    {
                        string title = dr["Index"].ToString();
                        drNew[title] = dr["Value"];
                    }
                }
                else
                {
                    foreach (DataRow dr in dtIndex.Rows)//找下面的指标规则
                    {
                        string indexTitle = dr["Title"].ToString();
                        drNew[indexTitle] = 1;
                    }
                }
                _table.Rows.Add(drNew);
                dtFrom = dtFrom.AddDays(1);
            }
        }
        #endregion
        #region 传值
        DataTable _table;
        public PropertyDescriptorCollection Schema
        {
            get
            {
                if (_table.DefaultView.Count > 0)
                    return TypeDescriptor.GetProperties(_table.DefaultView[0]);
                else
                    return TypeDescriptor.GetProperties(_table.DefaultView);
            }
        }


        public void GetTableData(TableCallback callback)
        {
            ChartTableData();
            callback(_table.Rows);
        }

        public bool ConnectionPointEnabled
        {
            get
            {
                object o = ViewState["ConnectionPointEnabled"];
                return (o != null) ? (bool)o : true;
            }
            set
            {
                ViewState["ConnectionPointEnabled"] = value;
            }
        }

        [ConnectionProvider("Table", typeof(TableProviderConnectionPoint),
      AllowsMultipleConnections = true)]
        public IWebPartTable GetConnectionInterface()
        {
            return this;// new PersonalStastics();
        }

        public class TableProviderConnectionPoint : ProviderConnectionPoint
        {
            public TableProviderConnectionPoint(MethodInfo callbackMethod,
        Type interfaceType, Type controlType, string name, string id,
        bool allowsMultipleConnections)
                : base(callbackMethod, interfaceType, controlType, name, id,
                  allowsMultipleConnections)
            {
            }

            public override bool GetEnabled(Control control)
            {
                return ((StateIndicator)control).ConnectionPointEnabled;
            }

        }
        #endregion
    }
}
