using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DeptNews.DeptNews
{
    public partial class DeptNewsUserControl : UserControl
    {
        public DeptNews WebPartObj { get; set; }
        //public uint newsperpage = WebPartObj.NewsNum;
        protected void Page_Load(object sender, EventArgs e)
        {

            ReadNewsByDept(WebPartObj.ListName, WebPartObj.SiteUrl, WebPartObj.DeptName, WebPartObj.SortString, WebPartObj.ModerationUrl);
        }     
         
        public void ReadNewsByDept(string listName,string siteUrl,string deptName,string sortString, string moderationUrl)
        {
            if (siteUrl=="")
            {
                siteUrl = SPContext.Current.Site.RootWeb.Url;
            }
            using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    SPList spList = spWeb.Lists.TryGetList(listName);
                    if (spList != null)
                    {
                        SPQuery qry = new SPQuery();
                        if (string.IsNullOrEmpty(sortString) || sortString == "系部")//此部件用作系部新闻筛选
                        {
                            
                            if (string.IsNullOrEmpty(deptName))//未填写系部名称
                            {
                                qry.Query = @"<OrderBy><FieldRef Name='Modified' Ascending='FALSE'/><FieldRef Name='Modified' Ascending='FALSE'/></OrderBy>";
                                deptName = "所有系部";
                            }
                            else
                            {
                                qry.Query = @"<Where><Eq><FieldRef Name='Dept'/><Value Type='Text'>" + deptName + "</Value></Eq></Where><OrderBy><FieldRef Name='Modified' Ascending='FALSE'/><FieldRef Name='Modified' Ascending='FALSE'/></OrderBy>";
                            }
                            
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<table  width='100 % ' id='ListArea' border='0' class='t1' cellpadding='0' cellspacing='0'>");
                                sb.Append("<tr style='font-weight:600;font-size:14px;'><th>标题</th><th>作者</th><th>时间</th></tr>");
                                foreach (SPListItem item in listItems)
                                {
                                    string displayUrl = SPContext.Current.Site.RootWeb.Url + "/_layouts/15/CopyUtil.aspx?Use=id&Action=dispform";
                                    //SPSite currentSite = SPContext.Current.Site;
                                    //SPWeb currentWeb = currentSite.OpenWeb();
                                    displayUrl = displayUrl + "&ItemId=" + item.ID + "&ListId=" + spList.ID + "&WebId=" + spWeb.ID + "&SiteId=" + spSite.ID + "&Source=" + UrlEncode(Request.Url.ToString());

                                    sb.Append("<tr>");
                                    sb.Append("<td><a href='" + displayUrl + "'>" + item.Title + "</a></td>");
                                    string temp = item["创建者"].ToString();
                                    temp = temp.Substring(temp.IndexOf("#") + 1);
                                    sb.Append("<td>" + temp + "</td>");
                                    sb.Append("<td>" + string.Format("{0:yyyy/MM/dd}", item["修改时间"]) + "</td>");
                                    //sb.Append("<td>"+item["新闻类别"]+"</td>");

                                    //sb.Append("<td><a href='" + spList.DefaultEditFormUrl + "?ID=" + item.ID + "'>编辑</a></td>");
                                    sb.Append("</tr>");
                                    
                                }
                                sb.Append("</table>");
                                divData.InnerHtml = sb.ToString();

                                string path = GetAddNewsUrl();
                                chtml.InnerHtml = "<a href='" + path  + "?Source=" + UrlEncode(Request.Url.ToString()) + "'>添加新闻</a>";
                            }
                            else
                            {
                                chtml.InnerHtml = "<b>" + deptName + "</b>尚无新闻发布！<br/><a href='" + spList.DefaultNewFormUrl + "?Source="+ UrlEncode(Request.Url.ToString()) + "'>添加新闻</a>";
                            }
                        }
                        else//此部件用做审批
                        {                            
                            qry.Query = @"<Where>
                                            <Eq>
                                                <FieldRef Name='_ModerationStatus' />
                                                <Value Type='ModStat'>2</Value>
                                            </Eq>
                                        </Where>
                                        <OrderBy>
                                            <FieldRef Name='Modified' Ascending='FALSE' />
                                            <FieldRef Name='Modified' Ascending='FALSE' />
                                        </OrderBy>";
                            SPListItemCollection listItems = spList.GetItems(qry);
                            if (listItems.Count > 0)
                            {
                                if (listItems.Count <= 8)
                                {
                                    pagediv.Visible = false;
                                }
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<table  width='100 % ' id='ListArea' border='0' class='t1' cellpadding='0' cellspacing='0'>");
                                sb.Append("<tr style='font-weight:600;font-size:14px;' align='left'><th>标题</th><th>作者</th><th>发布时间</th><th>作者系部</th><th>新闻类别</th></tr>");
                                foreach (SPListItem item in listItems)
                                {
                                    string mUrl = moderationUrl + "?ItemId=" + item.ID + "&Source=" + Request.Url.ToString();

                                    sb.Append("<tr>");
                                    sb.Append("<td><a href='" + mUrl + "' title='点击查看并审批'>" + item.Title + " </a></td>");//标题
                                    string temp = item["创建者"].ToString();                                   
                                    temp = temp.Substring(temp.IndexOf("#") + 1);
                                    sb.Append("<td>" + temp + "</td>");//作者
                                    sb.Append("<td>" + string.Format("{0:yyyy/MM/dd}", item["修改时间"]) + "</td>");//发布时间
                                    
                                    sb.Append("<td>" + item["所属系部"] + "</td>");//系部
                                    temp = item["新闻类别"].ToString();
                                    temp = temp.Substring(temp.IndexOf("#") + 1);
                                    sb.Append("<td>" + temp + "</td>");//新闻类别
                                    sb.Append("</tr>");
                                }
                                sb.Append("</table>");
                                divData.InnerHtml = sb.ToString();
                            }
                            else
                            {
                                chtml.InnerHtml = "无新闻待审批！";
                            }
                        }
                    }
                }
            }
        }
        //获取用博客网站创建新闻的链接地址
        private string GetAddNewsUrl()
        {
              SPWeb   web = SPContext.Current.Site.RootWeb ;
              string webUrl = WebPartObj.WebUrl;
              if (webUrl != null)
            {
                    web = web.Webs[webUrl];
            }

            try
            {
                SPList list = web.Lists.TryGetList(WebPartObj.NewsList);
                string lstUrl = list.DefaultNewFormUrl ;
                return lstUrl;
            }
            catch
            {
                return "";
            }
        }
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        public static void BindData(DataTable dt,GridView gv)
        {
            
        }
        protected DataTable MakeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("标题", System.Type.GetType("System.String"));
            dt.Columns.Add("发布者", System.Type.GetType("System.String"));
            dt.Columns.Add("发布时间", System.Type.GetType("System.String"));
            dt.Columns.Add("新闻类别", System.Type.GetType("System.String"));
            dt.Columns.Add("编辑", System.Type.GetType("System.String"));
            return dt;
        }
        protected DataTable DeptNewsdt(DataTable AllNewsdt)
        {
            DataTable dpNewsdt = MakeTable();
            for (int i=0;i<AllNewsdt.Rows.Count;i++)
            {
                DataRow dr0 = AllNewsdt.Rows[i];
                DataRow dr1 = dpNewsdt.NewRow();
                dr1["标题"] = "<a href='" + dr0["ID"] + "'>" + dr0["Title"] + "</a>";
            }            
            return dpNewsdt;

        }
        
        protected string ExportDatatableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags. 
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;

        }
    }
}
