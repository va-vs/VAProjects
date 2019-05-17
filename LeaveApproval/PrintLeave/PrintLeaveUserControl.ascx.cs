using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;  

namespace LeaveApproval.PrintLeave
{
    public partial class PrintLeaveUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnExport.Click += btnExport_Click;

        }
        public PrintLeave webObj { get; set; }
        #region 事件及方法
        private const string _exportPath = @"/_layouts/15/excel/";
        private const string _imagePath = @"/_layouts/15/images/";

        private SPListItem GetLeaveInfo()
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPListItem retItem = null;
                        string ID = Page.Request.QueryString["ID"];

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spTList = spWeb.Lists.TryGetList(webObj.ListName);

                         retItem = spTList.GetItemById(int.Parse(ID));
                       
                    }
                }
            });
            return retItem;
        }
        void btnExport_Click(object sender, EventArgs e)
        {
            string fileName = webObj.WordSavefile;
            string tempName = webObj.WordTempfile;
            int flag = 0;
            SPListItem retItem = GetLeaveInfo();
            if (retItem["Flag"] != null)
                flag = int.Parse(retItem["Flag"].ToString());
            else
                flag = 0;
            if (flag==31)
            {
                lblMsg.Text = "正在导出，请稍等……";
                btnExport.Enabled = false;
                WriteToWord(tempName, fileName );
                btnExport.Enabled = true;
                lblMsg.Text = "";
            }
            else
            {
                divSaveAs.InnerHtml = "";
                lblMsg.Text = webObj.NoDataMsg;
            }
        }
        private void WriteToWord(string tempName,string fileName )
        {
            string _filePath = Server.MapPath(_exportPath) + tempName;
            XWPFDocument m_Docx;
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                m_Docx = new XWPFDocument(fs);
                //获得书签  
                XWPFTable tb=  m_Docx.Tables[0];
                int i1=0;
                int j=0;
                foreach (XWPFTableRow row in tb.Rows)
                {
                    i1 += 1;
                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {
                        j = j + 1;
                        if (cell.GetText().IndexOf("姓名")>=0)
                            cell.Paragraphs[0].ReplaceText("姓名", "name");
                        
                        //cell.SetText(cell.Paragraphs.Count.ToString ());
                    }
                    j = 0;
                }
                XWPFTableCell ce=tb.Rows[0].GetCell(1);
                //插入图片  
                var gfs = new FileStream("1.png", FileMode.Open, FileAccess.Read);
                var gp = m_Docx.CreateParagraph();
                gp.Alignment = ParagraphAlignment.CENTER; //居中  
                var gr = gp.CreateRun();
                gr.AddPicture(gfs, (int)PictureType.JPEG, "1.png", 1000000, 1000000); //1000000 差不多100像素多一点  
                gfs.Close();

                List < CT_Bookmark > bkList = new List<CT_Bookmark>();
                int pcount = ce.GetXWPFDocument().Document.body.ItemsElementName.Count;
                for (int i = 0; i < pcount; i++)
                {
                    var ctp = ce.GetXWPFDocument().Document.body.GetPArray(i);
                    if (ctp != null)
                    {
                        var tempBookMarkList = ctp.GetBookmarkStartList();
                        bkList.AddRange(tempBookMarkList);
                    }
                 
                }
                //int pcount = m_Docx.Document.body.ItemsElementName.Count ;
                //for (int i = 0; i < pcount; i++)
                //{
                //    var ctp = m_Docx.Document.body.GetPArray(i);
                //    if (ctp != null)
                //    {
                //        var tempBookMarkList = ctp.GetBookmarkStartList();
                //        bkList.AddRange(tempBookMarkList);
                // 
                //}

                foreach (var bookMark in bkList)
                {//替换书签内容?????????????????找不到示例代码还是不支持此功能?  
                    
                    bookMark.colFirst = "1";
                    bookMark.colLast = "2";
                    bookMark.displacedByCustomXmlSpecified = true;




                }


            }
            //保存到本地文件
            _filePath = Server.MapPath(_exportPath) + fileName;
            using (FileStream fs1 = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            {
                m_Docx.Write(fs1);
            }
            m_Docx = null;
            divSaveAs.InnerHtml = "<a href='" + _exportPath + fileName + "'>单击下载</a>";
        }
        #endregion
    }
}
