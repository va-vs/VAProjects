using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;


namespace ShowNewsScroll.ShowNewsScroll
{
    [ToolboxItemAttribute(false)]
    public class ShowNewsScroll : WebPart
    {
        #region 定义
        // Fields
        private int beforeDays = 2;
        private string direction = "up";
        private int listCount =10;
        private int listheight = 160;
        private string webUrl = "News-Notices";
        private string webpartUrl = "/";
        private string listName = "通知";
        private int Listwidth = 220;
        private bool showIcon = true;
        private string orderbyField = "创建时间;降序";

        //private string webName = "";
        #endregion
        #region 事件
        protected override void OnPreRender(EventArgs e)
        {
            ///注册CSS文件
            RegisterCommonCSS("style.css");
            //注册JS文件
            //RegisterCommonJS("script.js");
            base.OnPreRender(e);
        }
        protected override void RenderContents(HtmlTextWriter writer)
        {
            SPWeb web = SPContext.Current.Web;
            if (WebUrl != null)
            {
                    web = web.Webs[webUrl];
            }

            try
            {
                SPList list = web.Lists[ListName];
                string lstUrl = list.DefaultViewUrl;
                //if (list.get_ParentWebUrl() != "/")
                //{
                //    lstUrl = lstUrl.Replace(list.get_ParentWebUrl(), "");
                //}
                string urlall = web.Site.Url + lstUrl;
                DataTable sourceData = new DataTable();
                sourceData.Columns.Add("通知ID", Type.GetType("System.Int32"));
                sourceData.Columns.Add("通知标题", Type.GetType("System.String"));
                sourceData.Columns.Add("通知创建时间", Type.GetType("System.DateTime"));
                sourceData.Columns.Add("置顶时间", Type.GetType("System.DateTime"));
                string queryStr= @"<Where>
                              <Eq>
                                 <FieldRef Name='_ModerationStatus' />
                                 <Value Type='ModStat'>已批准</Value>
                              </Eq>
                           </Where>
                           <OrderBy>
                              <FieldRef Name='EndTopDate' Ascending='FALSE' />
                              <FieldRef Name='EndTopDate' Ascending='FALSE' />
                              <FieldRef Name='Created' Ascending='FALSE' />
                              <FieldRef Name='Created' Ascending='FALSE' />
                           </OrderBy>";//仅显示已批准的通知，首先按照指定截至时间排序，再按照创建时间排序
                SPListItemCollection items = GetListItems(web, ListName, queryStr, (uint)listCount);
                if (WebPartUrl=="")
                {
                    WebPartUrl = web.Url;
                }
                writer.Write("<div class='ScrollNote'>");
                writer.Write("<div class='title'><a href='"+ WebPartUrl + "'>"+ WebPartTitle + "</a><a href='" + WebPartUrl + "' class='more'>更多>></a></div>");
                if (items.Count>0)
                {
                    FillListItem(writer, items, sourceData.Clone(), urlall);
                }
                else
                {
                    writer.Write("<span>尚未发布任何通知！</span>");
                }
                writer.Write("</div>");
            }
            catch
            {
                writer.Write("“" + ListName + "”列表不存在或子网站不存在");
            }
        }
        private void WriteScrollNews(HtmlTextWriter writer)
        {
            writer.Write("<div id='box' style='width: ", this.Listwidth, "px; height: ", this.listheight, "px;'>");
            writer.Write("<ul id='cont1' >");
            writer.Write("<li><a target='_blank' href='#'>111111111111</a></li>");
            writer.Write(" </ul>");
            writer.Write("<ul id='cont2'></ul>");
            writer.Write("</div>");
        }
        #endregion
        #region 方法
        private void FillListItem(HtmlTextWriter writer, SPListItemCollection items, DataTable sourceData,string txtViewUrl)
        {
            foreach (SPListItem item in items)
            {
                DataRow row = sourceData.NewRow();
                row["通知标题"] = item["标题"];
                row["通知ID"] = item.ID;
                row["通知创建时间"] = string.Format("{0:yyyy-MM-dd}", item["创建时间"].ToString()); //item["创建时间"].ToString("d");
                if (item["置顶截止时间"]!=null)
                {
                    row["置顶时间"] = string.Format("{0:yyyy-MM-dd}", item["置顶截止时间"].ToString());
                }
                else
                {
                    row["置顶时间"] = string.Format("{0:yyyy-MM-dd}", item["创建时间"].ToString());
                }
                sourceData.Rows.Add(row);
            }
            int num = sourceData.Rows.Count;
            if (num > listCount)
            {
                num = listCount;
            }
            string lstUrl = items.List.DefaultViewUrl;
            string url = txtViewUrl.Replace("AllPosts.aspx", "Post.aspx?ID=");
            writer.Write("<ul class='ScrollNote-list'>");
            for (int i = 0; i < num; i++)
            {
                string lUrl = url;
                lUrl = lUrl + sourceData.Rows[i]["通知ID"];
                DateTime dtCreated = (DateTime)sourceData.Rows[i]["通知创建时间"];
                DateTime dtTop = (DateTime)sourceData.Rows[i]["置顶时间"];
                string dtStr = dtCreated.ToString("yyyy-MM-dd");
                string strName = sourceData.Rows[i]["通知标题"].ToString();
                if (strName.Length > 20)
                {
                    strName = strName.Substring(0, 20) + "…";
                }
                if (dtCreated < dtTop)//当前为置顶通知
                {
                    writer.Write("<li title='【重要通知】' style='background-image: url(/_layouts/15/2052/images/top.png);background-position:left top;background-size:20px 20px;background-repeat: no-repeat;padding-left:5px;'>");
                }
                else
                {
                    writer.Write("<li>");
                }
                if (ShowIcon)
                {
                    if (DateTime.Now.Subtract(dtCreated).Days <= this.BeforeDays)
                    {
                        writer.Write(string.Concat(new object[] { "<a title='", sourceData.Rows[i]["通知标题"], "' href='", lUrl, "'>", strName, "<img title='新' class='ms-newgif' alt='新' src='/_layouts/15/2052/images/star.png' height='20px' width='20px'><span>&nbsp;&nbsp;", dtStr, "</span></a>" }) + "</li>");
                    }
                    else
                    {
                        writer.Write(string.Concat(new object[] { "<a title='", sourceData.Rows[i]["通知标题"], "' href='", lUrl, "'>",strName , "&nbsp;&nbsp;<span>", dtStr, "</span></a>" }) + "</li>");
                    }
                }
                else
                {
                    writer.Write(string.Concat(new object[] { "<a title='", sourceData.Rows[i]["通知标题"], "' href='", lUrl, "'>",strName , "&nbsp;&nbsp;<span>", dtStr, "</span></a>" }) + "</li>");
                }
            }
            writer.Write("</ul>");
        }
        private SPListItemCollection GetListItems(SPWeb web, string lstName,string queryStr,uint rowLimit)
        {
            SPList list = web.Lists[lstName];
            //string lstUrl = list.DefaultViewUrl;
            //string url = (web.Site.Url + lstUrl).Replace("AllPosts.aspx", "Post.aspx?ID=");
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope='RecursiveAll'";
            //string orderbystr = OrderByField;
            //SPField spf = web.Fields.GetField(orderbystr);
            query.Query = queryStr;
            query.RowLimit = rowLimit;
            SPListItemCollection items = list.GetItems(query);
            return items;
        }

        #endregion
        #region 注册
        //声明页面等资源文件存放的目录

        public const string RESOURCE_PATH = "/_layouts/15/ShowNewsScroll/";
        /// <summary>
        /// 注册CSS文件
        /// </summary>
        /// <param name="filename">CSS文件名</param>
        public void RegisterCommonCSS(string cssFilename)
        {
            string id = cssFilename.ToLower().Replace(".", "_");
            //判断CSS文件是否被添加过
            foreach (Control ctl in this.Page.Header.Controls)
            {
                if (ctl.ID == id)
                {
                    return;
                }
            }
            //创建CSS文件链接控件

            HtmlLink link1 = new HtmlLink();
            link1.ID = id;
            link1.Attributes["type"] = "text/css";
            link1.Attributes["rel"] = "stylesheet";
            link1.Attributes["href"] = base.ResolveUrl(RESOURCE_PATH) + cssFilename;
            this.Page.Header.Controls.Add(link1);
        }

        /// <summary>
        /// 注册脚本
        /// </summary>
        /// <param name="jsFilename">脚本文件名称</param>
        public void RegisterCommonJS(string jsFilename)
        {
            //脚本路径
            string jsPath = base.ResolveUrl(RESOURCE_PATH) + jsFilename;
            //注册脚本
            Page.ClientScript.RegisterClientScriptInclude(typeof(ShowNewsScroll), jsFilename.ToLower(), jsPath);

        }
        #endregion
        #region 设置项
        // Properties
        [WebBrowsable, WebDisplayName("提前的天数"), WebDescription(""), Personalizable]
        public int BeforeDays
        {
            get
            {
                return this.beforeDays;
            }
            set
            {
                this.beforeDays = value;
            }
        }

        string webpartTitle = "通知公告";
        [WebBrowsable, WebDisplayName("部件标题"), WebDescription("自定义部件标题"), Personalizable]
        public string WebPartTitle
        {
            get
            {
                return this.webpartTitle;
            }
            set
            {
                this.webpartTitle = value;
            }
        }

        [WebDisplayName("列表条数"), Personalizable, WebDescription("每个列表显示的条数"), WebBrowsable]
        public int ListCount
        {
            get
            {
                return this.listCount;
            }
            set
            {
                this.listCount = value;
            }
        }
        int _listCountScroll=6;
        [WebDisplayName("列表条数超过多少时开始滚动"), Personalizable, WebDescription(""), WebBrowsable]
        public int ListCountScroll
        {
            get
            {
                return this._listCountScroll;
            }
            set
            {
                this._listCountScroll = value;
            }
        }

        [WebDisplayName("列表高度"), WebDescription("设置列表"), WebBrowsable, Personalizable]
        public int listHeight
        {
            get
            {
                return this.listheight;
            }
            set
            {
                this.listheight = value;
            }
        }



        [Personalizable, WebBrowsable, WebDisplayName("列表宽度"), WebDescription("设置列表宽度")]
        public int ListWidth
        {
            get
            {
                return this.Listwidth;
            }
            set
            {
                this.Listwidth = value;
            }
        }

        [WebDescription("显示的默认列表名称"), Personalizable, WebBrowsable, WebDisplayName("显示的默认列表名称")]
        public string ListName
        {
            get
            {
                return this.listName;
            }
            set
            {
                this.listName = value;
            }
        }

        [WebDisplayName("滚动方向"), WebBrowsable, Personalizable, WebDescription("设置滚动方向")]
        public string ScrollDirection
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }

        [WebBrowsable, WebDisplayName("新建图标"), Personalizable, WebDescription("是否显示新建图标")]
        public bool ShowIcon
        {
            get
            {
                return this.showIcon;
            }
            set
            {
                this.showIcon = value;
            }
        }

        //[Personalizable, WebDisplayName("子网站的相对Url，多个网站用全角分号（；）分开"), WebDescription("显示子网站的所有列表"), WebBrowsable]
        //public string WebName
        //{
        //    get
        //    {
        //        return this.webName;
        //    }
        //    set
        //    {
        //        this.webName = value;
        //    }
        //}
        [Personalizable, WebDisplayName("网站地址"), WebDescription("通知列表所在的网站地址"), WebBrowsable]
        public string WebUrl
        {
            get
            {
                return this.webUrl;
            }
            set
            {
                this.webUrl = value;
            }
        }

        [Personalizable, WebDisplayName("Web部件标题Url"), WebDescription("Web部件标题指向的Url地址"), WebBrowsable]
        public string WebPartUrl
        {
            get
            {
                return this.webpartUrl;
            }
            set
            {
                this.webpartUrl = value;
            }
        }

        [Personalizable, WebDisplayName("排序字段"), WebDescription("指定数据排序的字段和排序方法,比如:创建时间;降序"), WebBrowsable]
        public string OrderByField
        {
            get
            {
                return this.orderbyField;
            }
            set
            {
                this.orderbyField = value;
            }
        }
        #endregion
    }
}
