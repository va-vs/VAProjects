using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;

namespace NewsPhotoShow.NewsPhotoShow
{
    [ToolboxItemAttribute(false)]
    public class NewsPhotoShow : WebPart
    {
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            SPWeb site = SPContext.Current.Web;
            try
            {
                SPList list = site.Lists[_PhotoListName];
                SPQuery query = new SPQuery();
                query.ViewAttributes = "Scope='RecursiveAll'";
                query.ViewFields = "<FieldRef Name='Title'/><FieldRef Name='FileDirRef'/><FieldRef Name='Modified'/><FieldRef Name='NewsUrl'/><FieldRef Name='FileRef'/><FieldRef Name='Description'/>";
                query.Query = "<OrderBy><FieldRef Name='Modified' Ascending='false'/></OrderBy>";
                query.RowLimit = PhotoNum;
                SPListItemCollection items = list.GetItems(query);
                this.Controls.Add(new LiteralControl(items[0]["URL 路径"].ToString()));
                string txtContent = GetWriteString(items);
                writer.Write(txtContent);
            }
            catch (Exception ex)
            {
                this.Controls.Add(new LiteralControl(ex.ToString()));
            }
        }
        string _jsPath = "_layouts/15/NewsPhotoShow/";

        private string GetWriteString(SPListItemCollection items)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + _jsPath + "css/tFocus.css\">");
            sb.AppendLine("<script type=\"text/javascript\" src=\"" + _jsPath + "js/tFocus.js\"></script>");
            sb.AppendLine("<!--[if IE 6]>");
            sb.AppendLine("<script src=\"js/ie6PNG.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script type=\"text/javascript\">DD_belatedPNG.fix('*');</script>");
            sb.AppendLine("<![endif]-->");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<!--效果开始-->");
            sb.AppendLine("<div id=\"tFocus\">");
            sb.AppendLine("<div class=\"prev\" id=\"prev\"></div>");
            sb.AppendLine("<div class=\"next\" id=\"next\"></div>");
            sb.AppendLine(" <a class=\"mark_left\" href=\"javascript:;\" id=\"markLeft\"></a>");
            sb.AppendLine("<a class=\"mark_right\" href=\"javascript:;\" id=\"markRight\"></a>");
            sb.AppendLine("<ul id=\"tFocus-pic\" alt=\"多图相册轮播特效\" >");
            for (int i = 0; i < items.Count; i++)
                sb.AppendLine(" <li><img src=\"" + items[i]["URL 路径"] + "\" width=\"" + PhotoWidth + "\" height=\"" + PhotoHeight + "\" alt=\"" + items[i]["标题"] + "\" value='" + items[i]["说明"] + "' /></li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("<!-- 下面划块隐藏的 -->");
            sb.AppendLine("<div id=\"tFocusBtn\">");
            sb.AppendLine("<a href=\"javascript:void(0);\" id=\"tFocus-leftbtn\"></a>");
            sb.AppendLine("<div id=\"tFocus-btn\">");
            sb.AppendLine("<ul>");
            sb.AppendLine("<li class=\"active\"><img src=\"images/02.jpg\" width=\"90\" height=\"48\" alt=\"热烈庆祝iSmart正式上线\" /></li>");
            sb.AppendLine("<li><img src=\"images/01.jpg\" width=\"90\" height=\"48\" alt=\"iSmart研发团队\" /></li>");
            sb.AppendLine("<li><img src=\"images/03.jpg\" width=\"90\" height=\"48\" alt=\"2015辽宁省大学英语教师教学能力提升培训班在东北大学举行\" /></li>");
            sb.AppendLine("<li><img src=\"images/04.jpg\" width=\"90\" height=\"48\" alt=\"2015辽宁省大学英语教师教学能力提升培训班成员合影\" /></li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");
            sb.AppendLine("<a href=\"javascript:void(0);\" id=\"tFocus-rightbtn\"></a>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id=\"tFocus-bg\" style=\"color:#fff\"><input type=\"button\" value=\"\" id=\"ismart_v\" style=\"border:0;color:#fff;font-family:微软雅黑;font-size:16px;height:35px;padding-left:10px;width:660px;text-align:left;background:none; cursor:pointer;\"/></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<script type=\"text/javascript\">addLoadEvent(Focus());</script>");
            sb.AppendLine("<!--End-->");

            sb.AppendLine("</body>");
            return sb.ToString();
        }
        #region 属性
        string _PhotoListName = "首页轮显";
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片库名称")]
        [WebDescription("图片库名称 (例如：图片新闻)")]
        public string PhotoListName
        {
            get { return _PhotoListName; }
            set { _PhotoListName = value; }
        }
        int _PhotoWidth = 662;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片宽度")]
        [WebDescription("要显示的图片宽度")]
        public int PhotoWidth
        {
            get { return _PhotoWidth; }
            set { _PhotoWidth = value; }
        }
        int _ImagHeight = 340;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片库高度")]
        [WebDescription("要显示的图片高度")]
        public int PhotoHeight
        {
            get { return _ImagHeight; }
            set { _ImagHeight = value; }
        }
        int _TitleHeight = 20;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片标题的高度")]
        [WebDescription("要显示的图片标题高度")]
        public int TitleHeight
        {
            get { return _TitleHeight; }
            set { _TitleHeight = value; }
        }
        uint _PhotoNum = 4;
        [Personalizable]
        [WebBrowsable]
        [WebDisplayName("图片的个数")]
        [WebDescription("")]
        public uint PhotoNum
        {
            get { return _PhotoNum; }
            set { _PhotoNum = value; }
        }
        #endregion
    }
}
