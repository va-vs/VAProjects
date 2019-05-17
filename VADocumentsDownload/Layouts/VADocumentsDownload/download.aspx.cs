using System;
using System.IO;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace VADocumentsDownload.Layouts.VADocumentsDownload
{
    public partial class download  : UnsecuredLayoutsPageBase
    {
        //下载网站上文档库中的文档
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Request["docUrl"] != null)
                GetDownloadFile(Server.UrlDecode(Page.Request["docUrl"].ToString()));

        }
        #region 方法
  
        private void GetDownloadFile(string docUrl)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPFile file = web.GetFile(docUrl);
                        Stream readStream = file.OpenBinaryStream();
                        byte[] fileContent = file.OpenBinary();
                        DownLoad(fileContent, file.Name);
                    }
                }
            });
        }
        //以流的形式下载文档
        private void DownLoad(byte[] fileContent, string fileName)
        {
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition",
              "attachment;filename=" + Server.UrlEncode(fileName));
            //Response.ContentType = "text/plain";
            Response.BinaryWrite(fileContent);
            Response.End();
        }
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }
        #endregion
    }
}
