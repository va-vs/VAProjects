using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace UploadAttachment.PerformAttachment
{
    public partial class PerformAttachmentUserControl : UserControl
    {
        #region 事件
        protected void Page_Load(object sender, EventArgs e)
        {
            SPUser user = SPContext.Current.Web.CurrentUser;
            bool isExits = ListExits();
            if (!isExits)
            {
                lblMsg.Text = webObj.ListName + "  列表不存在！";
                divUpload.Visible = false;
                return;
            }
            if (!Page.IsPostBack)
            {
                if (user != null)
                {
                    string ID = Page.Request.QueryString["ID"];
                    bool hasAudit = GetAppraiseRecord(ID);//是否已经审核
                    if (hasAudit)//已经审核，只能浏览
                    {
                        ViewState["hasRight"] = 0;
                    }
                    else
                    {
                        bool hasRight = UserHaveApproveRight(SPContext.Current.Site.ID, SPContext.Current.Web.Name, webObj.ListName, user);
                        if (hasRight)
                            ViewState["hasRight"] = 1;
                        else
                        {
                            bool hasAttachRight = UserHasAttachRight(user, int.Parse(ID));
                            if (!hasAttachRight)
                            {
                                ViewState["hasRight"] = 0;
                            }
                            else
                                ViewState["hasRight"] = 1;

                        }
                    }

                    lbAppraise.Text = webObj.ShowTitle;
                    if (ViewState["hasRight"].ToString() == "0")
                        divUpload.Visible = false;

                }
                else//匿名用户不可谏
                {
                    AppraiseDiv.Visible = false;
                }
            }
         
            WriteAttachments();
            btnAppraise.Click += btnAppraise_Click;
        }
        //上传附件
        void btnAppraise_Click(object sender, EventArgs e)
        {
            if (FileUpload1.FileName != "")
                UpdateDocument();
           
        }
        #endregion
        #region 属性
         public PerformAttachment webObj { get ;set  ; }
        #endregion
        #region 方法
        private void UpdateDocument()
         {
             string filename = FileUpload1.FileName;
             filename = filename.Substring(filename.LastIndexOf("/") + 1);
            string  titleName = filename.Substring(0, filename.IndexOf("."));//去掉扩展名
            string modifyFileName = DateTime.Today.ToString ("yyyyMMdd")+ DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
            filename = titleName + modifyFileName + filename.Substring(filename.IndexOf("."));
             Stream filedataStream = FileUpload1.PostedFile.InputStream;

             int dataLen = FileUpload1.PostedFile.ContentLength;

             string fileType = FileUpload1.PostedFile.ContentType;

             byte[] fileData = new byte[dataLen];

             filedataStream.Read(fileData, 0, dataLen);

             string id = Page.Request.QueryString["ID"];
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 try
                 {
                     using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                     {
                         using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.Name])
                         {
                             sWeb.AllowUnsafeUpdates = true;
                             SPList sList = sWeb.Lists[webObj.ListName + "附件库"];

                             SPFile sFile=sList.RootFolder.Files.Add (filename, fileData);
                             SPListItem item = sFile.Item;
                             item["ParentTitle"] = id;
                             item["Editor"] = SPContext.Current.Web.CurrentUser.ID;
                             item["Author"] = SPContext.Current.Web.CurrentUser.ID;
                             item["Title"] = titleName;
                             item.Update();
                             sWeb.AllowUnsafeUpdates = false;
                             WriteAttachments();
                         }
                     }
                 }
                 catch
                 {

                 }
             });
         }
        //读取列表对应的附件库
         private void WriteAttachments()
         {
             string id = Page.Request.QueryString["ID"];
             SPListItemCollection docITems = null;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 try
                 {
                     using (SPSite spSite = new SPSite(SPContext.Current.Site.ID ))
                     {
                         using (SPWeb sWeb = spSite.AllWebs[SPContext.Current.Web.Name ])
                         {
                             SPList sList = sWeb.Lists[webObj.ListName +"附件库"];
                             SPQuery qry = new SPQuery();
                             qry.Query = @"<Where><Eq><FieldRef Name='ParentTitle' LookupId='True' /><Value Type='Lookup'>" + id + "</Value></Eq></Where>";
                             docITems = sList.GetItems(qry);
                         }  
                     }
                 }
                 catch
                 {
                   
                 }
             });
             //填充内容
             tbContent.Rows.Clear();
             TableRow tRow;
             TableCell tCell;
             LinkButton lnkBtn;
             for (int i = 0; i < docITems.Count; i++)
             {
                 tRow = new TableRow();
                 tCell = new TableCell();
                 tCell.Controls.Add(new LiteralControl("<a href='" + docITems[i]["FileRef"] + "'>" + docITems[i]["Title"] + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));
                 tRow.Cells.Add(tCell);//加审核项
                 if (ViewState["hasRight"].ToString() == "1")
                 {
                     tCell = new TableCell();
                     lnkBtn = new LinkButton();
                     lnkBtn.Text = "删除";
                     lnkBtn.OnClientClick = "if(!confirm('确定要删除操作吗？'))return false;";
                     lnkBtn.Click += lnkBtn_Click;
                     lnkBtn.ID = "btn_" + docITems[i].ID;
                     tCell.Controls.Add(lnkBtn);
                     tRow.Cells.Add(tCell);//加审核说明
                 }
                 tbContent.Rows.Add(tRow);
             }
         }

         void lnkBtn_Click(object sender, EventArgs e)
         {
             LinkButton lnkBtn=sender as LinkButton ;
             string id = lnkBtn.ID.Substring(lnkBtn.ID.IndexOf("_") + 1);
             SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList sList = sWeb.Lists[webObj.ListName + "附件库"];
                        SPListItem item = sList.GetItemById(int.Parse(id));
                        sWeb.AllowUnsafeUpdates = true;
                        item.Delete();
                        sWeb.AllowUnsafeUpdates = false;
                        WriteAttachments();
                    }
                }
            });
         }
         private bool ListExits()
         {
             bool isExits = false;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite spSite = new SPSite(SPContext.Current.Site.ID)) //找到网站集
                 {
                     using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         SPList parentList = spWeb.Lists.TryGetList(webObj.ListName);
                         if (parentList == null)
                             isExits = false;
                         else
                             isExits = true;
                     }
                 }
             });
             return isExits;
         }
         /// <summary>
         /// 当前数据是否已经评审,需要提升权限，否则得不到数据
         /// </summary>
         /// <param name="strAction">操作</param>
         /// <param name="properties">operType =0是否审核，>0查找用户业绩</param>
         /// <returns></returns>
         public bool  GetAppraiseRecord(string strPerformanceID )
         {
             string strAction = "审核";
             bool retItem = false ;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite spSite = new SPSite(SPContext.Current.Site.ID )) //找到网站集
                 {
                     using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                     {
                         string childListName = webObj.ListName + "业绩";
                         SPList spList = spWeb.Lists.TryGetList(childListName);
                         SPList parentList = spWeb.Lists.TryGetList(webObj.ListName);
                         if (parentList != null)
                         {
                             string lookupInternalName = spList.Fields.GetField(parentList.Fields.GetFieldByInternalName("Title").Title).InternalName;
                             if (spList != null)
                             {
                                 SPQuery qry = new SPQuery();
                                 qry.Query = @"<Where><And><Eq><FieldRef Name='Action' /><Value Type='Choice'>" + strAction + "</Value></Eq><Eq><FieldRef Name='" + lookupInternalName + "' LookupId='True' /><Value Type='Lookup'>" + strPerformanceID + "</Value></Eq></And></Where>";
                                 SPListItemCollection listItems = spList.GetItems(qry);
                                 if (listItems.Count > 0)
                                     retItem = true;
                             }
                         }
                     }
                 }
             });
             return retItem;
         }
         //根据当前时间判断当前进行到什么审批阶段，如果为空则当前没有进行审批
        //用户是否有权限
         private bool UserHasAttachRight(SPUser user,int itemID)
         {
             bool isRight = false ;
             using (SPSite spSite = new SPSite(SPContext.Current.Site.ID ))
             {
                 using (SPWeb sWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                 {
                     SPList sList = sWeb.Lists.TryGetList(webObj.ListName);
                     if (sList != null)
                     {
                         SPListItem item = sList.GetItemById(itemID);
                         SPFieldUserValue author = new SPFieldUserValue(sWeb, (item["创建者"].ToString()));
                         if (author.LookupId == user.ID)
                             isRight = true;
                         else
                         {
                             SPFieldUserValueCollection adUsers = item["AuthorName"] as SPFieldUserValueCollection;
                             foreach (SPFieldUserValue adUser in adUsers)
                                 if (adUser.LookupId == user.ID)
                                 {
                                     isRight = true;
                                     break;
                                 }
                         }
                     }
                 }
             }
             return isRight;
         }
        //用户有审核权限
         private bool UserHaveApproveRight(Guid siteID, string webName, string lstName, SPUser user)
         {
             bool isRight = true;
             SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 try
                 {
                     using (SPSite spSite = new SPSite(siteID))
                     {
                         using (SPWeb sWeb = spSite.AllWebs[webName])
                         {
                             SPList sList = sWeb.Lists[lstName];
                             sList.DoesUserHavePermissions(user, SPBasePermissions.ApproveItems);
                             isRight = sList.DoesUserHavePermissions(user, SPBasePermissions.ApproveItems);
                         }  //return DoesUserHavePermssionsToWeb(ref user, ref sWeb);
                     }
                 }
                 catch
                 {
                     isRight = false;
                 }
             });
             return isRight;
         }
         #endregion
    }
}
