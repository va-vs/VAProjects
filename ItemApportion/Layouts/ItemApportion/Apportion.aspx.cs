using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Data;
using System.Collections;

namespace ItemApportion.Layouts.ItemApportion
{
    public partial class Apportion : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string itemId = Page.Request.QueryString["itemID"];
            string listId= Page.Request.QueryString["listID"];
            Guid listGuid = new Guid(listId);
            //string SourceUrl = Page.Request.QueryString["Source"];
            //string siteUrl = SPContext.Current.Site.Url;
           
            getDataFromparentlist(int.Parse(itemId), listGuid);
            btnAppraise.Click += BtnAppraise_Click;
            btnCancle.Click += BtnCancle_Click;
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            string SourceUrl = Page.Request.QueryString["Source"];
            Response.Redirect(SourceUrl);
        }

        private void BtnAppraise_Click(object sender, EventArgs e)
        {
            
            if (tbName.CommaSeparatedAccounts == "" || tbDurings.Text.Trim() == "")
            {
                lblMsg.Text = "分配对象或计划时长尚未填写!";
            }
            else
            {
                string durings = tbDurings.Text.Trim();
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    try
                    {                    
                        using (SPSite spSite = SPContext.Current.Site) //找到网站集
                        {
                            using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                            {
                                SPList childlist = spWeb.Lists.TryGetList("日程");
                                if (childlist != null)
                                {
                                    spWeb.AllowUnsafeUpdates = true;
                                    SPListItem listItem;
                                    string itemID = Page.Request.QueryString["itemID"];
                                    listItem = childlist.AddItem();
                                    listItem["任务名称"] = itemID + ";#" + lbItemTitle.Text;
                                    listItem["分配对象"] = GetUserValue(spWeb, (PickerEntity)tbName.ResolvedEntities[0]);
                                    listItem["计划时长"] = durings;
                                    listItem.Update();
                                    string SourceUrl = Page.Request.QueryString["Source"];
                                    if (string.IsNullOrEmpty(SourceUrl))
                                    {
                                        SourceUrl = SPContext.Current.Site.Url;
                                    }
                                    Response.Redirect(SourceUrl);
                                    //string myurl = childlist.DefaultViewUrl;
                                    //Response.Redirect(myurl);
                                }
                                else
                                {
                                    lblMsg.Text = "日程创建失败";
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        lblMsg.Text = "日程创建失败,可能原因是:"+ex.ToString();
                    }
                });
            }
            
        }

        //获取控件中的值
        private SPFieldUserValue GetUserValue(SPWeb web, PickerEntity picker)
        {
            SPUser user = web.EnsureUser(picker.Key);
            SPFieldUserValue fieldvalue = new SPFieldUserValue(web, user.ID, user.LoginName);
            return fieldvalue;
        }

        public static string GetPeopleEditorValue(PeopleEditor objPeopleEditor)
        {
            string strResult = string.Empty;
            ArrayList list = objPeopleEditor.ResolvedEntities;

            foreach (PickerEntity p in list)
            {
                string userId = p.EntityData["SPUserID"].ToString();
                string DisplayName = p.EntityData["DisplayName"].ToString();
                strResult += userId + ";#" + DisplayName;
                strResult += ",";
            }
            return strResult;
        }
        /// <summary>
        /// 根据列表栏的内部名查找显示名
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns></returns>
        public static string getDNameByIName(SPList list,string internalName)
        {
            string dispName = "";
            SPField field = list.Fields.GetFieldByInternalName(internalName);
            dispName = field.Title;
            return dispName;
        }


        public void getRecords(string siteUrl, Guid listGuid, int itemId)
        {
            using (SPSite spsite = new SPSite(siteUrl))
            {
                using (SPWeb spweb = spsite.OpenWeb(siteUrl))
                {
                    SPListCollection colllists = spweb.Lists;
                    SPList parentlist = colllists.GetList(listGuid, true);
                    string parentlistTitle = parentlist.Title.ToString();//父列表标题
                    string childlistTitle = parentlistTitle + "业绩";//子列表标题
                    SPList childlist = colllists.TryGetList(childlistTitle);

                    if (childlist != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query ="<Where><Eq><FieldRef Name='Dept' LookupId='True' /><Value Type='Lookup'>"+ itemId + "</Value></Eq></Where>";
                        SPListItemCollection listItems = childlist.GetItems(qry);
                        if (listItems.Count>0)
                        {
                            DataTable dt = listItems.GetDataTable();                            
                        }
                    }
                }
            }
        }

        public void getDataFromparentlist(int itemId,Guid listguid)
        {
            using (SPSite spSite = SPContext.Current.Site) //找到网站集
            {
                using (SPWeb spweb = spSite.OpenWeb(SPContext.Current.Web.ID))
                {
                    SPUser user = spweb.CurrentUser;
                    if (user!=null)//已登录
                    {
                        SPListCollection colllists = spweb.Lists;
                        SPList spList = colllists[listguid];
                        //SPList spList = colllists.TryGetList("任务");
                        if (spList!=null)
                        {
                            SPListItem item = spList.GetItemById(itemId);
                            string action = item["操作"].ToString();
                            if (action=="所有")
                            {
                                AppAction.InnerHtml = "该任务不是原子任务,无法进行日程分配!";
                            }
                            else
                            {
                                lbItemTitle.Text = item.Title;
                                tbName.CommaSeparatedAccounts = user.LoginName;
                            }
                            
                        }
                     
                        else
                        {
                            AppAction.InnerHtml = "你尚未指定分配日程的任务!";
                        }
                    }
                    else
                    {
                        AppAction.InnerHtml = "你尚未登录!";
                    }                    
                }
            }
        }
        public DataTable newDatatable(string tabletitle)
        {
            DataTable dt = new DataTable(tabletitle);
            dt.Columns.Add(tabletitle + "标题", Type.GetType("string"));//列1:父项标题(ParentTitle)
            dt.Columns.Add("姓名", Type.GetType("string"));//列2:姓名(AuthorName)
            dt.Columns.Add("系数", Type.GetType("int"));//列3:系数(Ratio)
            dt.Columns.Add("动作", Type.GetType("string"));//列4:动作(Action)
            dt.Columns.Add("状态", Type.GetType("string"));//列5:状态(Status)
            dt.Columns.Add("备注", Type.GetType("string"));//列6:备注(Title)
            return dt;
        }
        public List<SPUser> GetUsersFromSPFieldUser(SPListItem item,string filedName)
        {
            List<SPUser> fUsers = new List<SPUser>();
            SPWeb web = item.ParentList.ParentWeb;
            SPFieldUser uField = item.Fields[" 作者 "] as SPFieldUser;
            if (!uField.AllowMultipleValues)  //未允许多重选择
            {
                SPFieldUserValue userValue = new SPFieldUserValue(web, item[filedName].ToString());
                fUsers.Add(userValue.User);
            }
            else
            {
                SPFieldUserValueCollection userValues = item[filedName] as SPFieldUserValueCollection;
                foreach (SPFieldUserValue userValue  in  userValues)
                {
                    if ( userValue.User!=null)
                    {
                        fUsers.Add(userValue.User);
                    }
                    //else
                    //{
                    //    SPGroup userGroup = web.SiteGroups.GetByID(userValue.LookupID);
                    //    fUsers.AddRange(userGroup.Users);
                    //}
                }
            }
            return fUsers;
        }
    }
}
