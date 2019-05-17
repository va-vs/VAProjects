using System;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text;

namespace VADocumentsDownload.VADocumentsDownload
{
    public partial class VADocumentsDownloadUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                ReadDocLib();
        }
        public VADocumentsDownload webObj { get; set; }
        /// <summary>
        /// 读取文档库的文档
        /// </summary>
        private void ReadDocLib()
        {
            List<SPListItem> docFiles = new List<SPListItem>();
            List<SPListItem> picFiles = new List<SPListItem>();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList splist = web.Lists.TryGetList(webObj.ListName);
                        if (splist != null)
                        {
                            foreach (SPListItem lstDoc in splist.Items)
                            {
                                string url = lstDoc["FileRef"].ToString();
                                if (AllowPic(url))
                                    picFiles.Add(lstDoc);
                                else
                                    docFiles.Add(lstDoc);
                            }
                        }
                        else
                        {
                            lblMsg.Text = webObj.ListName + " 库不存在！";
                            return;
                        }
                    }
                }
            });
            if (picFiles.Count > 0)
                FillAlbum(picFiles, webObj.ListName);
            if (docFiles.Count > 0)
                FillDocs(docFiles, webObj.ListName);
        }

        /// <summary>
        /// 填充图片集
        /// </summary>
        /// <param name="picFiles"></param>
        /// <param name="listName"></param>
        private void FillAlbum(List<SPListItem> picFiles,string listName)
        {
            divImgs.InnerHtml = "";
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id='albumcontent'><div class='mygallery clearfix'><div class='tn3 album'><ol>");
            for (int i = 0; i < picFiles.Count; i++)
            {
                string thumbnail = picFiles[i]["FileRef"].ToString();
                //string thumbTitle = picFiles[i]["FileLeafRef"].ToString();
                //thumbTitle = thumbTitle.Substring(0, thumbTitle.LastIndexOf("."));//获取不含扩展名的标题
                sb.Append("<li><h4>"+ picFiles[i].DisplayName + "</h4><div class='tn3 description'><a href='" + thumbnail + "' download='" + picFiles[i].DisplayName + "'>下载图片</a></div><a href='" + thumbnail + "'><img src='" + thumbnail + "'/></a></li>");
            }
            sb.Append("</ol></div></div></div>");
            divImgs.InnerHtml = "<div class='grouptitle'>" + listName + "图片集 <span>(计 "+picFiles.Count+" 项)</span></div><br/>" + sb.ToString();
        }


        private void FillDocs(List<SPListItem> docFiles, string listName)
        {
            divContent.InnerHtml = "";
            string webUrl = SPContext.Current.Web.Url;
            webUrl = webUrl + "/_layouts/15/WopiFrame.aspx?sourcedoc=";
            string localUrl = Page.Request.Url.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<table id='mytab' border='0' class='mytable'>");
            for (int i = 0; i < docFiles.Count; i++)
            {
                if (i%2==0)
                {
                    sb.Append("<tr class='a1'>");
                }
                else
                {
                    sb.Append("<tr>");
                }
                sb.Append("<td>◇ <a target =\"_blank\" title='" + docFiles[i].DisplayName + "' href='" + webUrl + docFiles[i]["FileRef"] + "&action=view'>" + docFiles[i].DisplayName + "</a></td>");
                sb.Append("<td><a href=\"/_layouts/15/VADocumentsDownload/download.aspx?docUrl=" + docFiles[i]["FileRef"] + "\">点击下载 </a></td>");
                sb.Append("</tr>");
            //string thumbTitle = picFiles[i]["FileLeafRef"].ToString();
            //thumbTitle = thumbTitle.Substring(0, thumbTitle.LastIndexOf("."));//获取不含扩展名的标题

            }
            sb.Append("</table>");
            divContent.InnerHtml = "<div class='grouptitle'>" + listName + "文档集 <span>(计 " + docFiles.Count + " 项)</span></div><br/>" + sb.ToString();
        }
        /// <summary>
        /// 填充图片
        /// </summary>
        /// <param name="PicFiles"></param>
        private void FillPic(List<SPListItem> PicFiles)
        {
            Table tbl = new Table();
            tbl.CellPadding = 10;
            TableRow trow = new TableRow();
            TableCell tcell = new TableCell();

            int i = 0;
            while (i < PicFiles.Count)
            {
                trow = new TableRow();
                for (int col = 0; col < webObj.PicCols; col++)
                {
                    tcell = new TableCell();
                    string thumbnail = PicFiles[i]["FileRef"].ToString ();
                    string extName = thumbnail.Substring(thumbnail.LastIndexOf("."));//获取扩展名
                    string fileName = thumbnail.Substring(thumbnail.LastIndexOf("/")).Replace(extName, "");
                    string fName = thumbnail.Substring(0, thumbnail.LastIndexOf("/")) + "/_t";
                    fName = fName + fileName + extName.Replace(".", "_") + extName;
                    tcell.Controls.Add(new LiteralControl("<a target=\"_blank\" href =\"" + PicFiles[i]["FileRef"]+"\"><img src=\"" + fName   + "\" height=\"" + webObj.PicHeigh  + "\" width=\"" + webObj.PicWidth  + "\" title=\""+ PicFiles[i].DisplayName  + "\" />" + "</a>"));
                    trow.Cells.Add(tcell);
                    i = i + 1;
                    if (i == PicFiles.Count  )
                        break;
                }

                tbl.Rows.Add(trow);
            }
            divContent.Visible = true;
            divContent.Controls.Add(tbl);
        }
        private void FillDoc(List<SPListItem> docFiles, string listName)
        {
            Table tbl = new Table();
            tbl.CellPadding = 5;
            TableRow trow = new TableRow();
            TableCell tcell = new TableCell();
            string webUrl = SPContext.Current.Web.Url;
            webUrl = webUrl + "/_layouts/15/WopiFrame.aspx?sourcedoc=";
            string localUrl = Page.Request.Url.ToString();
            int i = 0;
            while (i<docFiles.Count )
            {
                trow = new TableRow();
                tcell = new TableCell();

                tcell.Controls.Add(new LiteralControl("<a target=\"_blank\" title='" + docFiles[i].DisplayName + "' href='"+webUrl + docFiles[i]["FileRef"] + "&action=view'>" + docFiles[i].DisplayName +"</a>"));
                trow.Cells.Add(tcell);
                tcell = new TableCell();
                tcell.Controls.Add(new LiteralControl("<a href=\"/_layouts/15/VADocumentsDownload/download.aspx?docUrl=" + docFiles[i]["FileRef"] + "\">点击下载 </a>"));
                trow.Cells.Add(tcell);
                tbl.Rows.Add(trow);
                i = i + 1;
            }
            divContent.Visible = true;
            divContent.Controls.Add(tbl);
        }
        /// <summary>
        /// 判断是否是图片类型
        /// </summary>
        /// <param name="fileUrl">文档的地址链接</param>
        /// <returns>是否是图片文档</returns>
       private bool AllowPic(string fileUrl)
        {
            fileUrl = fileUrl.ToLower();
            string picType = webObj.PicType.ToLower();
            picType = picType.Replace("；", ";") .TrimEnd(';');
            string[] fTypes = Regex.Split(picType, ";");
            foreach (string ftype in fTypes )
            {
                if (fileUrl.EndsWith(ftype))
                    return true;
            }
            return false ;
        }
    }
}
