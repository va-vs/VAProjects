using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace NewsPhotoShowWithText.NewsPhotoShowWithText
{
    [ToolboxItemAttribute(false)]
    public class NewsPhotoShowWithText : WebPart
    {
        protected override void CreateChildControls()
        {
            //<a href="..." target="_blank">
        }
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    ArrayList newsList = getDataFromSPList(SiteUrl, ListName,NewsNum,StrLength);
                    StringBuilder controlHtml = new StringBuilder();
                    if (newsList!=null)
                    {
                        controlHtml = getControlHtml(newsList);
                    }
                    else
                    {
                        controlHtml.Append("你设置的网站中的列表不存在数据,请重新设置!");
                    }
                    
                    writer.Write(controlHtml.ToString());
                });
                
            }
            catch (System.Exception ex)
            {
                this.Controls.Add(new LiteralControl(ex.ToString()));
            }
        }
         public static string _jsPath = "_layouts/15/NewsPhotoShowWithText/";
        
        #region 属性
        /// <summary>
        /// 用户指定新闻列表名称
        /// </summary>
        string _ListName = "新闻";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻列表名称")]
        [WebDescription("新闻列表的名称 (例如：新闻)")]
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }

        string _SiteUrl = SPContext.Current.Site.Url;//缺省为当前网站的地址
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("网站地址")]
        [WebDescription("新闻列表所在的网站地址，注意：只写到网站的根路径，不需要写到网站首页")]
        public string SiteUrl
        {
            get { return _SiteUrl; }
            set { _SiteUrl = value; }
        }

        uint _NewsNum = 5;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("新闻的个数")]
        [WebDescription("最多在图片滚动中显示的新闻条数，缺省为5个")]
        public uint NewsNum
        {
            get { return _NewsNum; }
            set { _NewsNum = value; }
        }

        int _StrLength = 120;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("正文摘要字数")]
        [WebDescription("在摘要部分显示的字数,可以为1-255整数")]
        public int StrLength
        {
            get { return _StrLength; }
            set { _StrLength = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 读取指定网站的制定列表的数据,生成图片新闻列表数据集
        /// </summary>
        /// <param name="siteURL">网站地址</param>
        /// <param name="listName">列表名称</param>
        /// <param name="newsNum">数据集长度</param>
        /// <returns>图片新闻列表ArrayList</returns>
        public static ArrayList getDataFromSPList(string siteURL, string listName,uint newsNum,int strLength)
        {
            ArrayList arr = new ArrayList();
            SPWeb spweb = new SPSite(siteURL).OpenWeb();  //Open website
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
                          <FieldRef Name='Created' Ascending='FALSE' />
                       </OrderBy>";
                //qry.ViewFields = @"<FieldRef Name='Title' /><FieldRef Name='Body' /><FieldRef Name='Author0' /><FieldRef Name='Modified' /><FieldRef Name='Editor' /><FieldRef Name='FileRef' />";
                qry.RowLimit = newsNum;
                SPListItemCollection listItems = spList.GetItems(qry);
                //将SP列表数据集转化为ArrayList，ArrayList内存储图片新闻的每一条记录,以数组形式存储
                if (listItems.Count>0)
                {
                    string tempStr = "";
                    foreach (SPListItem listItem in listItems)
                    {
                        string[] news = new string[6];
                        string bodyStr = listItem["正文"].ToString();
                        news[0] = listItem["标题"].ToString();//0 新闻标题
                        news[1]=string.Format("{0:d}", listItem["创建时间"]);//1 新闻发布时间
                        tempStr = listItem["创建者"].ToString();
                        news[2] = tempStr.Substring(tempStr.IndexOf("#")+1); //2 新闻作者
                        if (listItem["撰稿人"]!=null)
                        {
                            news[2] = listItem["撰稿人"].ToString();
                        }

                        news[3] = StripHT(bodyStr,strLength);//3 新闻摘要,获取新闻正文中前200个字

                        ArrayList imgurlLists = getimgurl(bodyStr);//获取新闻中的所有图片
                        if (imgurlLists.Count>0)//新闻中不止一张图片
                        {
                            news[4] = imgurlLists[0].ToString();//4 正文中有图片，无论多少张，均记录第一张图片地址
                        }
                        else
                        {
                            news[4] = _jsPath+"images/DefaultImg.png";//4 正文中若没有图片，则显示为缺省图片
                        }

                        tempStr = spList.DefaultDisplayFormUrl;                        
                        news[5] = tempStr + "?ID=" + listItem["ID"];//5 新闻地址
                        arr.Add(news);
                    }
                }
                else
                {
                    arr = null;
                }
            }
            else
            {
                arr = null;
            }
            return arr;
        }

        /// <summary>
        /// 将列表数据格式化后输出成控件的html
        /// </summary>
        /// <param name="photoNewsList"></param>
        /// <returns></returns>
        public StringBuilder getControlHtml(ArrayList photoNewsList)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<head>");
            //sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.AppendLine("<link rel='stylesheet' type='text/css' href='" + _jsPath + "css/style.css'>");
            sb.AppendLine("<script type='text/javascript' src='" + _jsPath + "js/prototype.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='" + _jsPath + "js/ImageSlide.js'></script>");
            //sb.AppendLine("</head>");
            //sb.AppendLine("<body>");
            sb.AppendLine("<div id='focusImage' class='slide'>");
            sb.AppendLine("<ul class='contents'>");
            foreach (string[] photoNews in photoNewsList)
            {
                sb.AppendLine("<li class='current'>");
                sb.AppendLine("<div class='image'><a href='" + photoNews[5] + "' target='_blank'><img src='" + photoNews[4] + "' width='480px' height='270px'></a></div>");
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

            //sb.AppendLine("</body>");
            return sb;
        }

        /// <summary>
        /// 获取HTML文本串中的图片地址：本方法返回的是一个ArrayList 集合，包含了文本里面所有的Img的src；
        /// </summary>
        /// <param name="html">html文本串</param>
        /// <returns></returns>
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

        /// <summary>
        /// 提取Html中的文本,本根据需要输出固定的文字数目
        /// </summary>
        /// <param name="strHtml">html内容</param>
        /// <param name="stringLength">输出字符串长度</param>
        /// <returns></returns>
        private static string StripHT(string strHtml,int stringLength)  //从html中提取纯文本
        {
            Regex regex = new Regex("<.+?>", RegexOptions.IgnoreCase);
            string strOutput = regex.Replace(strHtml, "");//替换掉"<"和">"之间的内容
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("&nbsp;", "");
            if (strOutput.Length> stringLength)
            {
                strOutput = strOutput.Substring(0, stringLength) + " ……";
            }
            return strOutput;
        }


        #endregion

        #region 暂时废弃的属性和方法

        #region 获取新闻内容摘要

        /// <summary>
        /// 新闻内容摘要：方法中的bool StripHTML参数表示是否以HTMl文本方式输出，如果为True的话表示去除HTML标签与样式，截取到的是纯文本，反之就是以HTMl文本输出。
        /// </summary>
        /// <param name="sString"></param>
        /// <param name="nLeng"></param>
        /// <returns></returns>
        public static string GetContentSummary(string content, int length, bool StripHTML)
        {
            if (string.IsNullOrEmpty(content) || length == 0)
                return "";
            if (StripHTML)//选择以Html方式输出新闻内容摘要
            {
                Regex re = new Regex("<[^>]*>");
                content = re.Replace(content, "");
                content = content.Replace("　", "").Replace(" ", "");
                if (content.Length <= length)
                    return content;
                else
                    return content.Substring(0, length) + "……";
            }
            else//选择以纯文本方式输出新闻内容摘要
            {
                if (content.Length <= length)
                    return content;

                int pos = 0, npos = 0, size = 0;
                bool firststop = false, notr = false, noli = false;
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    if (pos >= content.Length)
                        break;
                    string cur = content.Substring(pos, 1);
                    if (cur == "<")
                    {
                        string next = content.Substring(pos + 1, 3).ToLower();
                        if (next.IndexOf("p") == 0 && next.IndexOf("pre") != 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                        }
                        else if (next.IndexOf("/p") == 0 && next.IndexOf("/pr") != 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                                sb.Append("<br/>");
                        }
                        else if (next.IndexOf("br") == 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                                sb.Append("<br/>");
                        }
                        else if (next.IndexOf("img") == 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                            {
                                sb.Append(content.Substring(pos, npos - pos));
                                size += npos - pos + 1;
                            }
                        }
                        else if (next.IndexOf("li") == 0 || next.IndexOf("/li") == 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                            {
                                sb.Append(content.Substring(pos, npos - pos));
                            }
                            else
                            {
                                if (!noli && next.IndexOf("/li") == 0)
                                {
                                    sb.Append(content.Substring(pos, npos - pos));
                                    noli = true;
                                }
                            }
                        }
                        else if (next.IndexOf("tr") == 0 || next.IndexOf("/tr") == 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                            {
                                sb.Append(content.Substring(pos, npos - pos));
                            }
                            else
                            {
                                if (!notr && next.IndexOf("/tr") == 0)
                                {
                                    sb.Append(content.Substring(pos, npos - pos));
                                    notr = true;
                                }
                            }
                        }
                        else if (next.IndexOf("td") == 0 || next.IndexOf("/td") == 0)
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            if (size < length)
                            {
                                sb.Append(content.Substring(pos, npos - pos));
                            }
                            else
                            {
                                if (!notr)
                                {
                                    sb.Append(content.Substring(pos, npos - pos));
                                }
                            }
                        }
                        else
                        {
                            npos = content.IndexOf(">", pos) + 1;
                            sb.Append(content.Substring(pos, npos - pos));
                        }
                        if (npos <= pos)
                            npos = pos + 1;
                        pos = npos;
                    }
                    else
                    {
                        if (size < length)
                        {
                            sb.Append(cur);
                            size++;
                        }
                        else
                        {
                            if (!firststop)
                            {
                                sb.Append("……");
                                firststop = true;
                            }
                        }
                        pos++;
                    }

                }
                return sb.ToString();
            }
        }
        #endregion

        //解析新闻字符串
        private ArrayList GetTitleAndTextOFList(string listUrl)
        {
            ArrayList txtTitle = new ArrayList();

            if (listUrl.StartsWith("http://"))
                listUrl = listUrl.Substring(listUrl.IndexOf("/", 7) + 1);
            if (listUrl.IndexOf("&") > 0)
                listUrl = listUrl.Substring(0, listUrl.IndexOf("&"));
            string num = listUrl.Substring(listUrl.IndexOf("=") + 1);//当前新闻的序号
            string listURl = listUrl.Substring(0, listUrl.LastIndexOf("/"));//列表url
            SPWeb web = SPContext.Current.Web;
            string txt;
            string listName = "";
            foreach (SPList myList in web.Lists)
            {
                if (myList.BaseTemplate == SPListTemplateType.Announcements)
                {
                    txt = myList.DefaultDisplayFormUrl;
                    if (txt.Contains(listURl))
                    {
                        listName = myList.Title;
                        break;
                    }
                }
            }
            SPList list = web.Lists.TryGetList(listName);
            SPQuery query = new SPQuery();
            query.ViewAttributes = "Scope='RecursiveAll'";
            query.Query = "<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + num + "</Value></Eq></Where>";
            SPListItemCollection items = list.GetItems(query);
            if (items.Count > 0)
            {
                txtTitle.Add(items[0]["标题"].ToString());
                txtTitle.Add(items[0]["正文"].ToString());
            }
            return txtTitle;

        }

        private string GetWriteString(SPListItemCollection items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
            sb.AppendLine("<link rel='stylesheet' type='text/css' href='" + _jsPath + "css/style.css'>");
            sb.AppendLine("<script type='text/javascript' src='" + _jsPath + "js/prototype.js'></script>");
            sb.AppendLine("<script type='text/javascript' src='" + _jsPath + "js/ImageSlide.js'></script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div id='focusImage' class='slide'>");
            sb.AppendLine("<ul class='contents'>");
            ArrayList txtNews = new ArrayList();
            foreach (SPListItem item in items)
            {
                sb.AppendLine("<li class='current'>");
                sb.AppendLine("<div class='image'><a href='" + item["说明"] + "' target='_blank'><img src='" + item["URL 路径"] + "' width='480px' height='270px'></a></div>");
                sb.AppendLine("<div class='text'>");
                txtNews = GetTitleAndTextOFList(item["说明"].ToString());
                sb.AppendLine("<span class='title'><a href='" + item["说明"] + "' target='_blank'>" + txtNews[0].ToString() + "</a></span>");
                string txtText = txtNews[1].ToString();
                sb.AppendLine("<div class='newsBody'>" + txtText + "</div>");
                sb.AppendLine("</div>");
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("<div class='triggers'>");
            int i = 0;
            foreach (SPListItem item in items)
            {
                if (i == 0)
                    sb.AppendLine("<a href='javascript:;' class='cur current'><img src='" + item["URL 路径"] + "' width='58' height='38'></a>");
                else
                    sb.AppendLine("<a href='javascript:;'><img src='" + item["URL 路径"] + "' width='58' height='38'></a>");
                i = i + 1;
            }
            //sb.AppendLine("<a href='javascript:;'><img src='images/2.jpg' width="58" height="38"></a>");
            //sb.AppendLine("<a href='javascript:;'><img src='images/3.jpg' width="58" height="38"></a>");
            //sb.AppendLine("<a href='javascript:;'><img src='images/4.jpg' width="58" height="38"></a>");
            //sb.AppendLine("<a href='javascript:;'><img src='images/5.jpg' width="58" height="38"></a>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class='icon-dot'>");
            i = 0;
            foreach (SPListItem item in items)
            {
                if (i == 0)
                    sb.AppendLine("<a href='javascript:;' class='cur current'></a>");
                else
                    sb.AppendLine("<a href='javascript:;'></a>");

                i = i + 1;
            }
            //sb.AppendLine("<a href='javascript:;'></a>");
            //sb.AppendLine("<a href='javascript:;'></a>");
            //sb.AppendLine("<a href='javascript:;'></a>");
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

            sb.AppendLine("</body>");
            return sb.ToString();
        }

        //string _PhotoListName = "首页轮显";
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("图片库名称")]
        //[WebDescription("图片库名称 (例如：图片新闻)")]
        //public string PhotoListName
        //{
        //    get { return _PhotoListName; }
        //    set { _PhotoListName = value; }
        //}


        //int _PhotoWidth = 662;
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("图片宽度")]
        //[WebDescription("要显示的图片宽度")]
        //public int PhotoWidth
        //{
        //    get { return _PhotoWidth; }
        //    set { _PhotoWidth = value; }
        //}
        //int _ImagHeight = 340;
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("图片库高度")]
        //[WebDescription("要显示的图片高度")]
        //public int PhotoHeight
        //{
        //    get { return _ImagHeight; }
        //    set { _ImagHeight = value; }
        //}
        //int _TitleHeight = 20;
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("图片标题的高度")]
        //[WebDescription("要显示的图片标题高度")]
        //public int TitleHeight
        //{
        //    get { return _TitleHeight; }
        //    set { _TitleHeight = value; }
        //}
        //uint _PhotoNum = 5;
        //[Personalizable]
        //[WebBrowsable]
        //[WebDisplayName("新闻的个数")]
        //[WebDescription("最多在图片滚动中显示的新闻条数，缺省为5个")]
        //public uint PhotoNum
        //{
        //    get { return _PhotoNum; }
        //    set { _PhotoNum = value; }
        //}
        #endregion
    }
}
