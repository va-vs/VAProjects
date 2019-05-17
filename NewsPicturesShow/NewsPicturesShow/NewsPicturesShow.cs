using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;

namespace NewsPicturesShow.NewsPicturesShow
{
    [ToolboxItemAttribute(false)]
    public class NewsPicturesShow : WebPart
    {
        protected override void CreateChildControls()
        {

        }
        string title;
        //string title1;
        string topUrl;
        string eimg;
        string[] img1;
        string url1;
        string url2;
        protected override void Render(HtmlTextWriter writer)
        {
            SPWeb site = SPContext.Current.Web;
            try
            {
                SPList list = site.Lists[_PhotoListName];
                string queryStr = "SELECT [URL 路径],标题,名称,说明 FROM " + _PhotoListName + " order by 修改时间 desc";//查询列表名为list1中图片不是空的所有数据项
                int count = 1;
                string url = site.Url + "/";
                topUrl = site.Site.Url + "/_layouts";
                foreach (SPListItem item in list.Items)
                {
                    img1 = item["URL 路径"].ToString().Split(',');
                    eimg = eimg + site.Site.Url + img1[0].ToString() + "|";
                    url2 = item["说明"] != null ? item["说明"].ToString() : "";// url + list.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url + "?
                    url1 = url1 + url2 + "|";//把得到的URL进行累加，并用|进行区分
                    if (item["标题"] != null)
                        title = title + item["标题"].ToString() + "|";
                    else
                        title = title + " " + "|";
                    if (count == 5) break;
                    count = count + 1;
                }
                eimg = eimg.TrimEnd(new char[] { '|' });
                url1 = url1.TrimEnd(new char[] { '|' });
                title  = title.TrimEnd(new char[] { '|' });
                string txt = GetHtmlText(eimg, url1,title);
                writer.Write(txt);
            }
            catch (Exception e)
            { writer.Write(e.ToString()); }
        }
        private string GetHtmlText(string eimg, string url,string title)
        {
            string _jsPath = "_layouts/15/NewsPicturesShow/";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\"/></head>");
            sb.AppendLine("<link href=\"" + _jsPath + "css/css.css\" rel=\"stylesheet\" type=\"text/css\">");
            sb.AppendLine("<body>");
            sb.AppendLine("<script src=\"" + _jsPath + "js/yu.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"" + _jsPath + "js/tb.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<div align=\"center\">");
            sb.AppendLine("<div id=\"MainPromotionBanner\">");
            sb.AppendLine("<div id=\"SlidePlayer\">");
            sb.AppendLine("<ul class=\"Slides\">");
            string[] imgUrls = eimg.Split(new char[] { '|' });
            string[] linkUrls = url.Split(new char[] { '|' });
            string[] ingTitle = title.Split(new char[] { '|' });
            for (int i = 0; i < imgUrls.Length; i++)
            {
                sb.AppendLine("<li><a target=\"_blank\" href=\"" + linkUrls[i] + "\"><img src=\"" + imgUrls[i] + "\"+ alt=\"" + ingTitle[i] + "\"></a></li>");
            }
            sb.AppendLine("</ul></div><script type=\"text/javascript\">");
            sb.AppendLine("TB.widget.SimpleSlide.decoration('SlidePlayer', {eventType:'mouse', effect:'scroll'});");
            sb.AppendLine("</script></div></body></html>");
            return sb.ToString();
        }
        #region 属性
        string _PhotoListName = "首页轮显";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片库名称")]
        [WebDescription("图片库名称 (例如：首页轮显)")]
        public string PhotoListName
        {
            get { return _PhotoListName; }
            set { _PhotoListName = value; }
        }
        #endregion
    }
}
