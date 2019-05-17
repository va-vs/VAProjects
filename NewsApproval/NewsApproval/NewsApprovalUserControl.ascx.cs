using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Collections;
using System.Text;
using System.Xml;

namespace NewsApproval.NewsApproval
{
    public partial class NewsApprovalUserControl : UserControl
    {
       /// <summary>
       /// 用来调用参数
       /// </summary>
        public  NewsApproval webpartObj { get; set; }
        string srcUrl;
        int itemID;
        protected void Page_Load(object sender, EventArgs e)
        {
            //string  txt = testc();
            // this.Controls.Add(new LiteralControl(txt));

            if (Request.QueryString["Source"] != null)
            {
                srcUrl = Request.QueryString["Source"];
                itemID = int.Parse(Request.QueryString["ItemID"]);
            }
            else
            {
                srcUrl = SPContext.Current.Web.Url;
                itemID = 494;
            }
            if (!Page.IsPostBack)
                ReadNews();
        }
        #region 方法
        private void ReadNews()
        {
            using (SPSite mysite = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb myweb = mysite.AllWebs[SPContext.Current.Web.ID])
                {
                    SPList typeList = myweb.Lists.TryGetList(webpartObj.TypeListName);
                    if (typeList == null)
                    {
                        errShow.InnerText = webpartObj.TypeListName + "不存在";
                        Table1.Visible = false;
                        return;
                    }
                    ddlNewsType.Items.Clear();
                    foreach (SPListItem typeItem in typeList.Items)
                        ddlNewsType.Items.Add(new ListItem(typeItem["标题"].ToString(), typeItem["ID"].ToString()));

                    SPList list = myweb.Lists.TryGetList(webpartObj.ListName);
                    if (list==null)
                    {
                        errShow.InnerText =webpartObj.ListName + "不存在";
                        Table1.Visible = false;
                        return;
                    }
                    SPQuery query = new SPQuery();
                    query.ViewAttributes = "Scope='RecursiveAll'";
                    query.Query = "<Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>" + itemID + "</Value></Eq></Where>";
                    SPListItemCollection items = list.GetItems(query);
                    if (items.Count > 0)
                    {
                        SPListItem item = items[0];
                        txtTitle.Text = item["标题"].ToString();
                        txt1Body.Text = item["正文"].ToString();
                        if (item["是否主页显示"] != null)
                            chkIsPage.Checked = (bool)item["是否主页显示"]==true   ? true : false;
                        if (item["是否图片要闻"] != null)
                            chkIsPic.Checked = (bool)item["是否图片要闻"] == true ? true : false;
                        if (item["审批者评论"] != null)
                            txtModerationComments.Text = item["审批者评论"].ToString();
                        if (item["新闻类别"] != null)
                        {
                            string txt = item["新闻类别"].ToString();//5;#科技要闻
                            txt = txt.Substring(txt.IndexOf(";#") + 2);//类别名称
                            ddlNewsType.SelectedItem.Text = txt;
                        }

                    }
                }
            }
        }
        private void SaveNews(SPModerationStatusType  moderationStatus)
        {
            //SPSecurity.RunWithElevatedPrivileges(delegate()
            //{
                using (SPSite mysite = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb myweb = mysite.AllWebs[SPContext.Current.Web.ID])
                    {
                        myweb.AllowUnsafeUpdates = true;
                        SPList list = myweb.Lists.TryGetList(webpartObj.ListName);
                        SPListItem item = list.GetItemById(itemID);
                        if (moderationStatus == 0)//审批通过保存新闻内容，否则只保存审批相关内容
                        {
                            item["标题"] = txtTitle.Text.ToString();
                            item["正文"] = txt1Body.Text.ToString();
                            item["是否主页显示"] = chkIsPage.Checked == true ? true : false;
                            item["是否图片要闻"] = chkIsPic.Checked == true ? true : false;

                            item["新闻类别"] = ddlNewsType.SelectedItem.Value + ";#" + ddlNewsType.SelectedItem.Text;
                        }
                        //item["审批者评论"] = txtModerationComments.Text.ToString();
                        item.ModerationInformation.Comment = txtModerationComments.Text.ToString();
                        item.ModerationInformation.Status = moderationStatus ;
                        item.Update();
                        //item["审批状态"] = moderationStatus.ToString ();
                        ////item.SystemUpdate();
                        //item.mo
                        myweb.AllowUnsafeUpdates = false;
                    }
                }
            //});
        }
        //public  XmlNode UpdateListItemApprove(int moderationStatus)
        //{
            
        //    //listservice.Lists listProxy = new listservice.Lists();
        //    //string xml = "<Batch OnError='Continue'><Method ID='1' Cmd='Moderate'><Field Name='ID'>"+itemID +"</Field><Field Name=\"_ModerationStatus\" >"+moderationStatus.ToString ()+"</Field></Method></Batch>";
        //    //XmlDocument doc = new XmlDocument();
        //    //doc.LoadXml(xml);
        //    //XmlNode batchNode = doc.SelectSingleNode("//Batch");
        //    //listProxy.Url =  SPContext.Current.Web.Url + "/_vti_bin/lists.asmx";
        //    //listProxy.UseDefaultCredentials = true;
            
        //    XmlNode resultNode = listProxy.UpdateListItems(webpartObj.ListName , batchNode);
        //    return resultNode;
        //}
        private  string  testc()
        {
            StringBuilder txt = new StringBuilder();
            SPSecurity.RunWithElevatedPrivileges(delegate() {
            using (SPSite mysite =new SPSite (SPContext.Current.Site.ID ))
            {
                using (SPWeb myweb =mysite.AllWebs[SPContext.Current.Web.ID ] )
                {
                    SPList list = myweb.Lists.TryGetList(webpartObj.ListName);
                    foreach (SPField myfied in list.Fields)
                        txt.AppendLine(myfied.Title + ", " + myfied.InternalName+";<br>");
                }
            }
            
            });
            return txt.ToString ();
        }
        #endregion
        #region 按钮事件
        private void returnLastPage()
        {
            Response.Redirect(srcUrl );
        }
        //审批不通过
        protected void btnReject_Click(object sender, EventArgs e)
        {
            SaveNews(SPModerationStatusType.Denied );
            returnLastPage();
        }
        //审批通过
        protected void btnPiZhu_Click(object sender, EventArgs e)
        {
            SaveNews(SPModerationStatusType.Approved);
            returnLastPage();
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            returnLastPage();
        }
        #endregion
    }
}
