using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Whidsoft.WebControls;

namespace GenOrg.Orgs
{
    public partial class OrgsUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string style = Request.QueryString["style"];

            if (style == null || style == "")
            {
                style = "0";
            }

            if (!IsPostBack)
            {
                //每一个组织图中的节点都是一个OrgNode，各属性分别为显示名称，tips
                OrgNode orgNode = new OrgNode();

                orgNode.Text = "罗宝线";
                orgNode.Description = "这是组织图的塔尖";
                orgNode.NavigateUrl = "http://www.whidsoft.cn";
                //orgNode.Description = "累计电度：1000万度,占线路用电 100%";

                //建立各节点
                OrgNode OrgNode1 = new OrgNode();
                OrgNode1.Text = "车站照明动力";
                OrgNode1.Description = "这是";
                //OrgNode1.Description = "累计电度：55万度,占线路用电 65%";

                OrgNode OrgNode11 = new OrgNode();
                OrgNode11.Text = "车站1";
                OrgNode11.Description = "这是";
                //OrgNode11.Description = "累计电度：33万度,占线路用电 13%";

                OrgNode OrgNode12 = new OrgNode();
                OrgNode12.Text = "车站2";
                OrgNode12.Description = "这是";
                //OrgNode12.Description = "累计电度：133万度,占线路用电 33%";

                OrgNode OrgNode13 = new OrgNode();
                OrgNode13.Text = "车站3";
                OrgNode13.Description = "这是";
                //OrgNode13.Description = "累计电度：13万度,占线路用电 33%";

                OrgNode1.Nodes.Add(OrgNode11);
                OrgNode1.Nodes.Add(OrgNode12);
                OrgNode1.Nodes.Add(OrgNode13);
                orgNode.Nodes.Add(OrgNode1);

                OrgNode OrgNode2 = new OrgNode();
                OrgNode2.Text = "牵引动力";
                OrgNode2.Description = "这是";
                //OrgNode2.Description = "累计电度：13万度,占线路用电 11%";

                OrgNode OrgNode21 = new OrgNode();
                OrgNode21.Text = "1段牵引";
                //OrgNode21.Description = "这是";

                OrgNode OrgNode22 = new OrgNode();
                OrgNode22.Text = "2段牵引";
                OrgNode22.Description = "这是";

                OrgNode2.Nodes.Add(OrgNode21);
                OrgNode2.Nodes.Add(OrgNode22);
                //orgNode.Nodes.Add(OrgNode2);

                OrgNode OrgNode3 = new OrgNode();
                OrgNode3.Text = "维修段";
                OrgNode3.Description = "这是3";
                //OrgNode3.Description = "累计电度：345万度,占线路用电 54%";

                OrgNode OrgNode31 = new OrgNode();
                OrgNode31.Text = "检修";
                OrgNode31.Description = "这是3";

                OrgNode OrgNode32 = new OrgNode();
                OrgNode32.Text = "洗车及污水";
                OrgNode32.Description = "这是3";

                OrgNode OrgNode33 = new OrgNode();
                OrgNode33.Text = "综合办公楼";
                OrgNode33.Description = "这是3";

                OrgNode3.Nodes.Add(OrgNode31);
                OrgNode3.Nodes.Add(OrgNode32);
                OrgNode3.Nodes.Add(OrgNode33);
                orgNode.Nodes.Add(OrgNode3);

                OrgNode OrgNode4 = new OrgNode();
                OrgNode4.Text = "管理中心";
                OrgNode4.Description = "这是3";
                //OrgNode4.Description = "累计电度：234万度,占线路用电 34%";

                OrgNode OrgNode41 = new OrgNode();
                OrgNode41.Text = "办公";
                OrgNode41.Description = "这是3";

                OrgNode OrgNode42 = new OrgNode();
                OrgNode42.Text = "商业";
                OrgNode42.Description = "这是3";

                OrgNode OrgNode43 = new OrgNode();
                OrgNode43.Text = "机房";
                OrgNode43.Description = "这是3";

                OrgNode4.Nodes.Add(OrgNode41);
                OrgNode4.Nodes.Add(OrgNode42);
                OrgNode4.Nodes.Add(OrgNode43);
                orgNode.Nodes.Add(OrgNode4);

                OrgChart1.Node = orgNode;

                OrgChart1.ChartStyle = (style == "0") ? Whidsoft.WebControls.Orientation.Vertical : Whidsoft.WebControls.Orientation.Horizontal;

            }
        }
    }
}
