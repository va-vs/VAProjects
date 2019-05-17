using Microsoft.SharePoint;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace FSCWebParts.AttachmentPreview
{
    public partial class AttachmentPreviewUserControl : UserControl
    {
        public AttachmentPreview webObj;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int itemId = GetItemID();
                ShowAttachment(itemId);
            }
        }

        private void ShowAttachment(int itemId)
        {
            string query = @"<Where>
                              <Eq>
                                 <FieldRef Name='ID' />
                                 <Value Type='Counter'>" + itemId + @"</Value>
                              </Eq>
                           </Where>";
            SPListItemCollection items = GetDataFromList(webObj.ListName, query);
            if (items!=null)
            {
                if (items.Count>0)
                {
                    SPListItem item = items[0];
                    SPAttachmentCollection attach= item.Attachments;
                    if (attach.Count>0)
                    {
                        lbAttach.Text = "附件浏览（共"+attach.Count+"个）";
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<dl>");
                        for(int i=0; i<attach.Count; i++)
                        {
                            string title = attach[i];
                            string url = attach.UrlPrefix+ attach[i];
                            int k = url.IndexOf('/', 7);
                            string hosturl = url.Substring(0,k);
                            string weburl = SPContext.Current.Site.Url;
                            url = url.Replace(hosturl, weburl);
                            sb.Append(GenHtml(title,url,i));

                        }
                        sb.Append("</dl>");
                        divAttach.Visible=true;
                        lbAttach.Visible = true;
                        divAttach.Style.Value = "width:"+(webObj.DispWidth+20).ToString()+"px;height:"+webObj.DispHeight+"px;overflow-y:auto;border:1px solid #ebebeb;padding:10px;";
                        divAttach.InnerHtml = sb.ToString();
                    }
                    else
                    {
                        lbAttach.Visible = false;
                        divAttach.Visible=false;
                    }
                }
            }

        }

        private bool CheckAttchType(string[] fileTypes,string title)
        {
            bool fileOk = false;
            title = title.ToLower();
            for (int i = 0; i < fileTypes.Length; i++)
            {
                if (title.Contains(fileTypes[i]))
                {
                    fileOk = true;
                    break;
                }
            }
            return fileOk;
        }

        private string GenHtml(string title,string url,int i)
        {
            string[] imgfiles = new string[] { "jpg", "png", "bmp", "jpeg", "gif", "tiff" };
            string[] docfiles = new string[7] { "doc", "docx", "pdf", "ppt", "pptx", "xls", "xlsx" };
            StringBuilder sb = new StringBuilder();
            sb.Append("<dt>");
            sb.Append("<span style='line-height:25px;font-size:14px;'>附件"+(i+1).ToString()+"："+title+"</span><br/>");
            if (CheckAttchType(imgfiles,title))
            {

                sb.Append("<img src='" + url + "' width='"+webObj.DispWidth+"'/>");
            }
            else if(CheckAttchType(docfiles,title))
            {
                string ooserver = webObj.OOServer + "op/embed.aspx?src=";
                sb.Append("<iframe src='"+ooserver+url+"' width='"+webObj.DispWidth+"' height='"+webObj.DispHeight+"' frameborder='0'></iframe>");
            }
            else
            {
                sb.Append("<span>本附件无法在线浏览，请 <a href='"+url+"' title='"+title+"'><b>点此下载</b></a> </span>");
            }
            sb.Append("</dt>");
            return sb.ToString();
        }

        /// <summary>
        /// 从SharePoint列表获取数据
        /// </summary>
        /// <param name="listName">列表名称</param>
        /// <param name="query">Caml查询语句</param>
        /// <returns></returns>
        private SPListItemCollection GetDataFromList(string listName, string query)
        {
            SPListItemCollection items = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                        {
                            SPList spList = spWeb.Lists.TryGetList(listName);

                            if (spList != null)
                            {
                                if (query != "")
                                {
                                    SPQuery qry = new SPQuery();
                                    qry.Query = query;
                                    items = spList.GetItems(qry);
                                }
                                else
                                {
                                    items = spList.GetItems();
                                }
                            }
                            else
                            {
                                lbErr.Text = "指定的列表“" + listName + "”不存在！";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    lbErr.Text = ex.ToString();
                }
            });
            return items;
        }

        /// <summary>
        /// 获取URL中传递的项ID
        /// </summary>
        /// <returns></returns>
        private int GetItemID()
        {

            if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                return int.Parse(Request.QueryString["ID"]);
            }
            else
            {
                return 0;
            }
        }
    }
}
