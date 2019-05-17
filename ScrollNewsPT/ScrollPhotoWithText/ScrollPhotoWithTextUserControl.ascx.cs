using Microsoft.SharePoint;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace ScrollNewsPT.ScrollPhotoWithText
{
    public partial class ScrollPhotoWithTextUserControl : UserControl
    {
        public ScrollPhotoWithText wpObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList newsList = getDataFromSPList(wpObj.SiteUrl, wpObj.ListName, wpObj.NewsNum);
            if (newsList!=null)
            {
                StringBuilder controlHtml = getControlHtml(newsList);
                myScroll.InnerHtml = controlHtml.ToString();
            }
            else
            {
                myScroll.InnerHtml = "<span style='color:red'>你所设置的列表不存在或无数据</span>";
            }
                    
        }
        public static string _jsPath = "_layouts/15/NewsPhotoShowWithText/";
        #region 读取新闻列表生成图片新闻控件的html

        /// <summary>
        /// 读取指定网站的制定列表的数据,生成图片新闻列表数据集
        /// </summary>
        /// <param name="siteURL">网站地址</param>
        /// <param name="listName">列表名称</param>
        /// <param name="newsNum">数据集长度</param>
        /// <returns>图片新闻列表ArrayList</returns>
        public static ArrayList getDataFromSPList(string siteURL, string listName, uint newsNum)
        {
            ArrayList arr = new ArrayList();
            SPWeb spweb = new SPSite(siteURL).OpenWeb();  //Open website
            if (spweb!=null)
            {
                spweb.AllowUnsafeUpdates = true;
                SPQuery query = new SPQuery();
                SPList spList = spweb.Lists.TryGetList(listName);
                if (spList != null)
                {
                    SPQuery qry = new SPQuery();
                    //IsHighlights表示是否图片新闻的字段的内部名称，查询条件是否还要加上已审批通过这个条件
                    qry.Query =
                        @"<Where>
                         <And>
                                 <Eq>
                                    <FieldRef Name='IsHighlights' />
                                    <Value Type='Boolean'>1</Value>
                                 </Eq>
                                 <Eq>
                                    <FieldRef Name='_ModerationStatus' />
                                    <Value Type='ModStat'>0</Value>
                                 </Eq>
                              </And>                       
                        </Where>
                       <OrderBy>
                          <FieldRef Name='Modified' Ascending='FALSE' />
                       </OrderBy>";
                    //qry.ViewFields = @"<FieldRef Name='Title' /><FieldRef Name='Body' /><FieldRef Name='Author0' /><FieldRef Name='Modified' /><FieldRef Name='Editor' /><FieldRef Name='FileRef' />";
                    qry.RowLimit = newsNum;
                    SPListItemCollection listItems = spList.GetItems(qry);
                    //将SP列表数据集转化为ArrayList，ArrayList内存储图片新闻的每一条记录,以数组形式存储
                    if (listItems.Count > 0)
                    {
                        string tempStr = "";
                        foreach (SPListItem listItem in listItems)
                        {
                            string[] news = new string[6];
                            string bodyStr = listItem["正文"].ToString();
                            news[0] = listItem["标题"].ToString();//0 新闻标题
                            news[1] = string.Format("{0:d}", listItem["创建时间"]);//1 新闻发布时间
                            tempStr = listItem["创建者"].ToString();
                            news[2] = tempStr.Substring(tempStr.IndexOf("#") + 1); //2 新闻作者
                            if (listItem["撰稿人"] != null)
                            {
                                news[2] = listItem["撰稿人"].ToString();
                            }

                            news[3] = StripHT(bodyStr);//3 新闻摘要,获取新闻正文中前200个字

                            ArrayList imgurlLists = getimgurl(bodyStr);
                            if (imgurlLists.Count > 0)
                            {
                                news[4] = imgurlLists[0].ToString();//4 正文中有图片，无论多少张，均记录第一张图片地址
                            }
                            else
                            {
                                news[4] = _jsPath + "images/DefaultImg.png";//4 正文中若没有图片，则显示为缺省图片
                            }

                            tempStr = spList.DefaultDisplayFormUrl;
                            news[5] = tempStr + "?ID=" + listItem["ID"];//5 新闻地址

                            arr.Add(news);
                        }
                    }
                }
                else
                {
                    arr= null;
                }
            }
            else
            {
                arr = null;
            }
            return arr;

        }
        public StringBuilder getControlHtml(ArrayList photoNewsList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div id='focusImage' class='slide'>");
            sb.AppendLine("<ul class='contents'>");
            foreach (string[] photoNews in photoNewsList)
            {
                sb.AppendLine("<li class='current'>");
                sb.AppendLine("<div class='image'><a href='" + photoNews[5] + "' target='_blank'><img src='" + photoNews[4] + "'></a></div>");
                sb.AppendLine("<div class='text'>");
                sb.AppendLine("<span class='title'><a href='" + photoNews[5] + "' target='_blank'>" + photoNews[0] + "</a></span>");
                sb.AppendLine("<div style='text-align:right;'><hr/><b>" + photoNews[2] + "</b> 发布于 <b>" + photoNews[1] + "</b></div>");
                sb.AppendLine("<div class='newsBody'>" + photoNews[3] + "</div>");

                sb.AppendLine("</div>");
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("<div class='triggers'>");
            int i = 0;
            foreach (string[] photoNews in photoNewsList)
            {
                if (i == 0)
                    sb.AppendLine("<a href='javascript:;' class='cur current'><img src='" + photoNews[4] + "' width='58' height='38'></a>");
                else
                    sb.AppendLine("<a href='javascript:;'><img src='" + photoNews[4] + "' width='58' height='38'></a>");
                i = i + 1;
            }

            sb.AppendLine("</div>");

            sb.AppendLine("<div class='icon-dot'>");
            i = 0;
            foreach (string[] photoNews in photoNewsList)
            {
                if (i == 0)
                    sb.AppendLine("<a href='javascript:;' class='cur current'></a>");
                else
                    sb.AppendLine("<a href='javascript:;'></a>");

                i = i + 1;
            }
            sb.AppendLine("</div>");

            sb.AppendLine("<span class='link-watch pre down'></span>");
            sb.AppendLine("<span class='link-watch next down'></span>");

            sb.AppendLine("</div>");

            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("new ImageSlide({");
            sb.AppendLine("project: '#focusImage',");
            sb.AppendLine("content: '.contents li',");
            sb.AppendLine("tigger: '.triggers a',");
            sb.AppendLine("dot: '.icon-dot a',");
            sb.AppendLine("watch: '.link-watch',");
            sb.AppendLine("auto: !0,");
            sb.AppendLine("hide: !0");
            sb.AppendLine("})");
            sb.AppendLine("</script>");
            return sb;
        }


        private static string StripHT(string strHtml)  //从html中提取纯文本
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            if (strOutput.Length > 200)
            {
                strOutput = strOutput.Substring(0, 200) + " …";
            }
            return strOutput;
        }
        #endregion

        #region 获取图片新闻中第一张图片

        /// <summary>
        /// 获取HTML文本的图片地址：本方法返回的是一个ArrayList 集合，包含了文本里面所有的Img的src；
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>/
        /// 
        public static ArrayList getimgurl(string html)
        {
            ArrayList resultStrList = new ArrayList();
            Regex r = new Regex(@"<IMG[^>]+src=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.IgnoreCase);//忽视大小写
            MatchCollection mc = r.Matches(html);

            foreach (Match m in mc)
            {
                resultStrList.Add(m.Groups["src"].Value.ToLower());
            }
            if (resultStrList.Count > 0)
            {
                return resultStrList;
            }
            else
            {
                resultStrList.Clear();
                return resultStrList;
            }
        }
        #endregion
    }
}
