using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace VATaskDocment.TaskDoc
{
    /// <summary>
    /// 列表项事件
    /// </summary>
    public class TaskDoc : SPItemEventReceiver
    {
        #region 事件
        /// <summary>
        /// 正在添加项.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            base.ItemAdding(properties);
        }
        /// <summary>
        /// 正在更新项.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
        }
        /// <summary>
        /// 正在删除项.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);
        }
        /// <summary>
        /// 已添加项，
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            base.ItemAdded(properties);
            WriteDocumentNew(properties);
            //WriteSubTask(properties);
        }
        /// <summary>
        /// 已更新项.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            WriteDocumentNew(properties);
            //WriteSubTask(properties);
        }
        /// <summary>
        /// 已删除项.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);
        }
        #endregion
        #region 创建活动时，关联例时的任务
        private SPFieldUrlValue WriteRoutineTaskDoc(SPItemEventProperties properties,string accountName,string typeName,string taskDocLibName)
        {
            //string 
            //properties.ListItem["Title"] = documentName;
            SPSite site = properties.Site;
            SPWeb web = properties.Web;
            SPDocumentLibrary library = web.Lists[taskDocLibName] as SPDocumentLibrary;
            string docType = "Word文档.docx";

            string templatePath = "";
            string documentName = properties.ListItem.Title;//.Replace(properties.UserDisplayName,"") ;// 去掉用户的显示名称
            string fileName = documentName + docType.Substring(docType.IndexOf("."));
            string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
            string libraryPath = site.MakeFullUrl(libraryRelativePath);//带网站集的url
            string documentPath = libraryPath + "/" + fileName;//创建的文档名称
            templatePath = libraryPath + "/Forms/" + docType;// +fileType;// web.Url + "/" + library.DocumentTemplateUrl;  
            SPFile file = web.GetFile(templatePath);//获取模板文件
            Stream readStream = file.OpenBinaryStream();

            documentName = GetRoutineDocName(accountName, typeName);
            fileName = documentName + docType.Substring(docType.IndexOf("."));
            SPFieldUrlValue urlValue = UpdateDocumentToOnedrive(readStream, fileName, accountName);
            return urlValue;
        }
        #endregion
        #region 任务文档(项目和例行20181127)
        //获取任务文档名，文档名称加上项目的名称
        private string GetDocName(List<string> account)
        {
            string docName="";
            if (account.Count >1)//项目为根节点，从叶子到根保存
            {
                account.Reverse();
                for (int i = 0; i < account.Count;i++)
                    docName = docName + account[i]; 
            }
            return docName;
        }
        //创建任务时创建文档对象，如果文档对象不为空，则不创建,Routine部分不进行处理
        //2019-1-19日更新，项目文档中去年项目，不用判断
        public void WriteDocumentNew(SPItemEventProperties properties)
        {
            if (!properties.List.Fields.ContainsFieldWithStaticName("TaskDoc")|| properties.ListItem["TaskDoc"] != null)//条件判断
                return;
            int level = 0;
            List<string> account = new List<string>(); ;
            string projectName = GetRootType(properties.ListItemId, properties.List, ref level,ref account );
            //if (taskType == "Routine"  ||level==1 )//Routine/用户/类别/规则文档,项目顶级不处理&& level < 4
            //    return;
            web = properties.Web;
            string taskDocLibName =projectName+"任务文档" ;// 二级任务所对应的文档库
            string blogName =projectName+ "项目任务";//三级及以上对应的博客
            try
            {
                SPSite site = properties.Site;
                SPWeb web = properties.Web;
                string documentName = properties.ListItem.Title;//.Replace(properties.UserDisplayName,"") ;// 去掉用户的显示名称
                SPFieldUrlValue urlValue = new SPFieldUrlValue();
                web.AllowUnsafeUpdates = true;
                if (level < 2)
                {
                    SPDocumentLibrary library = web.Lists[taskDocLibName] as SPDocumentLibrary;
                    string docType = "Word文档.docx";
                    string templatePath = "";
                    string fileName = documentName + docType.Substring(docType.IndexOf("."));
                    string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
                    string libraryPath = site.MakeFullUrl(libraryRelativePath);//带网站集的url
                    string documentPath = libraryPath + "/" + fileName;//创建的文档名称
                    templatePath = libraryPath + "/Forms/" + docType;// +fileType;// web.Url + "/" + library.DocumentTemplateUrl;  
                    SPFile file = web.GetFile(templatePath);//获取模板文件
                    #region  fileNotNull
                    if (file != null)
                    {
                        try
                        {
                            //更新作务主表文档对象字段
                            Stream readStream = file.OpenBinaryStream(); //file is SPFile type文件不存在会报错
                            SPFile uploadedFile = null;// web.GetFile(documentPath);//
                            #region 上传文件
                            try
                            {
                                uploadedFile = web.Files.Add(documentPath, readStream, false);
                                uploadedFile.Item["Title"] = documentName;
                                uploadedFile.Item.Update();
                                uploadedFile.CheckIn("", SPCheckinType.MajorCheckIn);
                                uploadedFile.Update();
                            }
                            catch (Exception ex)//文档存在，原来的任务无链接则继续执行或者是原来的地址不统一，则不更改返回
                            {
                                uploadedFile = web.GetFile(documentPath);
                            }
                            urlValue = new SPFieldUrlValue();
                            urlValue.Description = uploadedFile.Title;
                            urlValue.Url = library.DefaultViewUrl;// documentPath;
                            #endregion
                        }
                        catch (Exception ex)//源文件不存在或目标文档已经存在，会报错
                        {
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                    #endregion
                }
                else//三级以上直接打开编辑
                {
                    int blogCateID = 0;
                    SPWeb blogWeb = GetBlogWeb(web,blogName ) ;//列表名为任务文章 或Posts
                    if (blogWeb == null) return;
                    string cateListName = "类别";//类别网站
                    SPList cateList = blogWeb.Lists.TryGetList(cateListName);
                    if (level == 2)//按类别打开博客
                    {
                        blogCateID = GetBlogCategoryID(cateList, documentName);
                        urlValue = new SPFieldUrlValue();
                        urlValue.Description = documentName;
                        string itemUrl = cateList.DefaultViewUrl.Replace("AllCategories.aspx", "Category.aspx");//由显示改为编辑
                        urlValue.Url = itemUrl + "?CategoryId=" + blogCateID;
                    }
                    else
                    {
                        SPList blogList = blogWeb.Lists.TryGetList(blogName.Replace(projectName, ""));

                        if (blogList == null)
                            blogList = blogWeb.Lists.TryGetList("Posts");

                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + documentName + "</Value></Eq></Where>";
                        SPListItemCollection items = blogList.GetItems(qry);
                        SPListItem item;
                        if (items.Count > 0)
                        {
                            item = items[0];
                        }
                        else
                        {
                            item = blogList.AddItem();
                            item["Title"] = documentName;
                            item["PublishedDate"] = DateTime.Now;
                            string category = account[1];//当前任务的上级任务 
                            blogCateID = GetBlogCategoryID(cateList, category);
                            if (blogCateID > 0)
                            {
                                SPFieldLookupValue cateValue = new SPFieldLookupValue();
                                cateValue.LookupId = items[0].ID;
                                item["PostCategory"] = cateValue;

                            }
                            item.Update();
                        }

                        urlValue = new SPFieldUrlValue();
                        urlValue.Description = documentName;
                        string itemUrl = blogList.DefaultViewUrl.Replace("AllPosts.aspx", "EditPost.aspx");//由显示改为编辑
                        urlValue.Url = itemUrl + "?ID=" + item.ID;
                    }
                }
                if (urlValue == null) return;
                this.EventFiringEnabled = false;//否则会死循环
                properties.ListItem["TaskDoc"] = urlValue;
                properties.ListItem.Update();
                this.EventFiringEnabled = true;
                web.AllowUnsafeUpdates = false;
            }
            catch (Exception ex)
            {
                //w.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 根据博客类别名称返回类别ID
        /// </summary>
        /// <param name="blogWeb"></param>
        /// <param name="cateName"></param>
        /// <returns></returns>
        private int GetBlogCategoryID(SPList  cateList, string cateName)
        {
            int cateID = 0;
            SPQuery qry = new SPQuery();
            qry.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + cateName + "</Value></Eq></Where>";
            SPListItemCollection items = cateList.GetItems(qry);
            if (items.Count >0)
                cateID = items[0].ID;
            return cateID;

        }
        private SPWeb GetBlogWeb(SPWeb web, string title)
        {
            foreach (SPWeb subWeb in web.Webs)
                if (subWeb.Title == title)
                    return subWeb;
            return null;
        }
        SPWeb web;
        /// <summary>
        /// 返回文档名称，没有扩展名
        /// </summary>
        /// <param name="account"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string GetRoutineDocName(string account, string typeName)
        {
            GregorianCalendar gc = new GregorianCalendar();
            string titleName = account + DateTime.Today.ToString("yyyy") + "年";
            if (typeName == "生活类")
            {
                titleName += DateTime.Now.Month + "月"; 
            }
            else
            {
                int weekOfYear = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
                titleName += "第" + weekOfYear + "周";
            }
            return titleName+typeName ;
        }
        /// <summary>
        /// 获取任务根节点的文档，项目和例行
        /// </summary>
        /// <returns></returns>
        private SPUser GetSPUser(string sLoginName)
        {
            SPUser oUser = null;
            try
            {
                if (!string.IsNullOrEmpty(sLoginName))
                {
                    if (web.AllUsers.Count > 0)
                    {
                        string domainName = web.AllUsers[0].LoginName;
                        domainName = domainName.Substring(0, domainName.IndexOf(@"\") + 1);
                        sLoginName = domainName + sLoginName;
                        oUser = web.AllUsers[sLoginName];
                    }
                    
                }
            }
            catch (Exception ex)
            {
            }
            return oUser;
        }
        /// <summary>
        /// 根结节为项目名称
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="taskList"></param>
        /// <param name="level"></param>
        /// <param name="account">向当前列表项而上遍历的标题</param>
        /// <returns></returns>
        private string GetRootType(int itemID, SPList taskList, ref int level, ref List<string> account)
        {
            SPListItem pItem = taskList.GetItemById(itemID);
            level = 1;
            if (pItem["ParentID"] == null)
                return pItem["Title"].ToString();
            else
            {
                while (pItem["ParentID"] != null)
                {
                    SPFieldLookupValue pFld = new SPFieldLookupValue(pItem["ParentID"].ToString());
                    account.Add(pItem["Title"].ToString());
                    pItem = taskList.GetItemById(pFld.LookupId);
                    level += 1;
                }
                account.Add(pItem["Title"].ToString());
                return pItem["Title"].ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filedataStream"></param>
        /// <param name="fileName"></param>
        /// <param name="lgUser">分配用户，也就是每二级的标题</param>
        /// <returns></returns>
        private SPFieldUrlValue UpdateDocumentToOnedrive(Stream filedataStream,string fileName,string lngAccount)
        {
            SPFieldUrlValue urlValue=null;
            try
            {
                string personalUrl = "http://localhost/personal/" + lngAccount;//进入个人网站
                
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite spSite = new SPSite(personalUrl))
                    {
                        using (SPWeb sWeb = spSite.OpenWeb())
                        {
                            sWeb.AllowUnsafeUpdates = true;
                            SPDocumentLibrary library = sWeb.Lists["文档"] as SPDocumentLibrary;
                            string libraryRelativePath = library.Folders[0].Url;
                            string libraryPath = spSite.MakeFullUrl(libraryRelativePath);//带网站集的url
                            string documentPath = libraryPath + "/" + fileName;// documentName + fileType;//创建的文档名称
                            SPFile sFile;
                            try
                            {
                                SPUser user = GetSPUser(lngAccount);
                                sFile = sWeb.Files.Add(documentPath, filedataStream, true);//
                                if (user != null)
                                {
                                    sFile.Properties["Author"] = user.ID;
                                    sFile.Properties["Editor"] = user.ID;
                                }
                                sFile.Update();
                                sFile.CheckIn("", SPCheckinType.MajorCheckIn);
                                sFile.Update();
                            }
                            catch (Exception ex)
                            {
                                sFile = sWeb.GetFile(documentPath);
                            }
                            if (sFile.Exists)
                            {
                                 urlValue = new SPFieldUrlValue();
                                int index = sFile.Name.IndexOf(".");//去掉扩展名
                                string docName=sFile.Name.Remove(index, sFile.Name.Substring(index).Length);
                                docName = docName.Replace(lngAccount, "");
                                urlValue.Description = docName;
                                urlValue.Url = documentPath;
                                 
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // "此文档已经存在或者文件类型被限制，请重新选择！";
            }
            return urlValue ;
        }
        //i:0#.w|ccc\20142781,
        public string GetAccount(SPUser lgUser)
        {
            string loginName = lgUser.LoginName;
            loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
            string account = loginName.Replace(@"i:0#.w|", "");
            return account;
        }
        #endregion
        #region 方法
        //检查子任务是否存在，通过快速编辑创建的时候，需要触发编辑的事件才能创建
        private bool CheckSubTask(string title, long parentID, SPList list)
        {
            bool exists = false;

            SPQuery qry = new SPQuery();
            qry.Query = @"<Where><And><Eq><FieldRef Name='Title' /><Value Type='Text'>" + title + "</Value></Eq><Eq><FieldRef Name='ParentID' LookupId='True' /><Value Type='Lookup'>" + parentID + "</Value></Eq></And></Where>";
            SPListItemCollection items = list.GetItems(qry);
            if (items.Count > 0)
            {
                exists =true;
            }


            return exists;
        }

        //根据操作个数创建子任务，满足创建子任务的三个条件，
        //http://xqx2012/blog/Lists/List8/AllItems.aspx（本地测试）
        public void WriteSubTask(SPItemEventProperties properties)
        {
            if (properties.ListItem["操作"] == null) return;
            if (properties.ListItem["TaskPType"] != null && properties.ListItem["操作"].ToString () == "所有")
            {
                string taskType = properties.ListItem["TaskPType"].ToString();//对象类型
                taskType = taskType.Substring(taskType.IndexOf(";#") + 2);
                string[] taskActions=GetTaskPActions (properties,"项目类型",taskType);
                string docType = "Word文档.docx";//获取模板
                SPListItem newItem = null;
                string action = "";
                bool tempExits = false;
                for (int i = 0; i < taskActions.Length; i++)
                {

                    action = taskActions[i];
                    string title = action + (properties.ListItem["Title"] == null ? "" : properties.ListItem["Title"].ToString());
                    if (i == 0)
                    {
                        tempExits = CheckSubTask(title, properties.ListItemId, properties.List);
                        if (tempExits) break;
                    }
                    docType = taskType + action+".docx";//项目类型加上操作
                    tempExits= this.CheckTemplateFile(properties, docType);
                    if (!tempExits)
                        docType = "Word文档.docx";
                    newItem = properties.List.AddItem();
                    newItem["DocType"] = docType;
                    newItem["操作"] = action;
                    newItem["对象"] = properties.ListItem["Title"];
                    newItem["ParentID"] = properties.ListItemId;
                    newItem["Title"] = title;// newItem["操作"].ToString() + (newItem["对象"] == null ? "" : newItem["对象"].ToString());
                    newItem["Priority"] = taskActions.Length - i;
                    newItem["TaskPType"] = properties.ListItem["TaskPType"];
                    if (i == 0)
                        newItem["Status"] = 1;
                    else
                        newItem["Status"] = 0;
                    if (properties.ListItem["预估工作量"]!=null)
                    {
                        float hours = float.Parse(properties.ListItem["预估工作量"].ToString());
                        newItem["预估工作量"] = hours / taskActions.Length;
                    }
                    newItem["PercentComplete"] = 0;
                    newItem["AssignedTo"] = properties.ListItem["AssignedTo"];
                    newItem["开始日期"] = properties.ListItem["开始日期"];
                    newItem["截止日期"] = properties.ListItem["截止日期"];
                    newItem.Update();
                }

            }

        }
        //获取指定项目类型下面的所有操作，在子表中
        private string[] GetTaskPActions(SPItemEventProperties properties, string taskTypeList, string taskType)
        {
            string[] actions = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            { using (SPSite site=new SPSite (properties.Site.ID ))
            {
                using (SPWeb web=site.OpenWeb (properties.Web.ID ))
                {
                    SPList list = web.Lists.TryGetList(taskTypeList);
                    if (list != null)
                    {
                        SPQuery qry = new SPQuery();
                        qry.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + taskType + "</Value></Eq></Where>";
                        SPListItemCollection items= list.GetItems(qry);
                        if (items.Count >0)
                        {
                            string  taskActions= items[0]["Actions"].ToString ().Replace ("；", ";").Trim();
                            actions = Regex.Split(taskActions, ";");
                             
                        }
                    }
                }
            }

            });
            return actions;
        }
        //创建任务时创建文档对象，如果文档对象不为空，则不创建
        public void WriteDocument(SPItemEventProperties properties)
        {
            if (properties.ListItem["Document"] != null)
                return;
            string taskDocName = "项目文档";
            SPUser lgUser = properties.Web.CurrentUser;
            //StreamWriter w = new StreamWriter("c:\\lg.txt");
            try
            {
                SPSite site = properties.Site;
                SPWeb web = properties.Web;
                SPDocumentLibrary library = web.Lists[taskDocName] as SPDocumentLibrary;
                string docType = "Word文档.docx";
                if (properties.ListItem["DocType"] != null)
                    docType = properties.ListItem["DocType"].ToString();
                string fileType = docType.Substring(docType.LastIndexOf("."));
                string templatePath = "";
                string documentName = properties.ListItem.Title;// +".xlsx";//".docx" ;
                string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
                string libraryPath = site.MakeFullUrl(libraryRelativePath);//带网站集的url
                string documentPath = libraryPath + "/" + documentName + fileType;//创建的文档名称
                templatePath = libraryPath + "/Forms/" + docType;// +fileType;// web.Url + "/" + library.DocumentTemplateUrl;  
                SPFile file = web.GetFile(templatePath);
                #region  fileNotNull
                if (file != null)
                {
                    try
                    {
                        web.AllowUnsafeUpdates = true;
                        Stream readStream = file.OpenBinaryStream(); //file is SPFile type文件不存在会报错
                        SPFile uploadedFile = null;// web.GetFile(documentPath);//
                        #region 上传文件
                        try
                        {
                            uploadedFile = web.Files.Add(documentPath, readStream, false);

                            uploadedFile.Item["Title"] = documentName;
                            uploadedFile.Item.Update();

                            uploadedFile.CheckIn("", SPCheckinType.MajorCheckIn);
                            uploadedFile.Update();
                        }
                        catch (Exception ex)//文档存在，原来的任务无链接则继续执行或者是原来的地址不统一，则不更改返回
                        {
                            //w.WriteLine(ex.ToString());
                            if (properties.ListItem["Document"] == null || !properties.ListItem["Document"].ToString().Contains(documentName + fileType))
                            {
                                uploadedFile = web.GetFile(documentPath);
                            }
                            else
                            {
                                //w.Close();
                                //w.Dispose();
                                return;
                            }
                        }
                        #endregion
                        //更新作务主表文档对象字段
                        SPFieldUrlValue urlValue = new SPFieldUrlValue();
                        urlValue.Description = documentName;
                        urlValue.Url = documentPath;
                        this.EventFiringEnabled = false;//否则会死循环
                        properties.ListItem["Document"] = urlValue;
                        properties.ListItem.Update();
                        this.EventFiringEnabled = true;
                        web.AllowUnsafeUpdates = false;
                    }
                    catch (Exception ex)//源文件不存在或目标文档已经存在，会报错
                    {
                        //w.WriteLine(ex.ToString());
                        web.AllowUnsafeUpdates = false;
                    }
                }
                #endregion
                //    }
                //}
            }
            catch (Exception ex)
            {
                //w.WriteLine(ex.ToString());
            }
            //});
            //w.Close();
            //w.Dispose();
        }
        //检查模板文件是否存在，创建子任务时根据项目类型和操作找模板文件
        private bool CheckTemplateFile(SPItemEventProperties properties, string docType)
        {
            string taskDocName = "项目文档";
            SPSite site = properties.Site;
            SPWeb web = properties.Web;
            SPDocumentLibrary library = web.Lists[taskDocName] as SPDocumentLibrary;
            string templatePath = "";
            string documentName = properties.ListItem.Title;// +".xlsx";//".docx" ;
            string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
            string libraryPath = site.MakeFullUrl(libraryRelativePath);//带网站集的url
            templatePath = libraryPath + "/Forms/" + docType;// +fileType;// web.Url + "/" + library.DocumentTemplateUrl;  
            SPFile file = web.GetFile(templatePath);
            #region  fileNotNull
            if (file != null)
            {
                try
                {
                    web.AllowUnsafeUpdates = true;
                    Stream readStream = file.OpenBinaryStream(); //file is SPFile type文件不存在会报错
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            #endregion
            return false;
        }
        private void WriteDocument11(SPItemEventProperties properties)
        {
            string taskDocName = "项目文档";
            SPSite siteCollection = properties.Site;
            SPWeb subWeb = properties.Web;
            SPDocumentLibrary library = subWeb.Lists[taskDocName] as SPDocumentLibrary;
            string docType = properties.ListItem["DocType"].ToString();
            string temType = "";
            string fileType = "";
            if (docType == "Word文档")
            { temType = ".dotx"; fileType = ".docx"; }
            else if (docType == "PPT演示文稿")
              {  temType = ".potx"; fileType = ".pptx"; }
            else if (docType == "Excel工作簿")
            { temType = ".xltx"; fileType = ".xlsx"; }

            foreach (SPContentType cType in library.ContentTypes)
            {
                if (cType.DocumentTemplateUrl.EndsWith(temType))
                {
                    library.DocumentTemplateUrl = cType.DocumentTemplateUrl;
                    break;
                }
            }
            string documentName = properties.ListItem.Title;// +".xlsx";//".docx" ;
            string libraryRelativePath = library.RootFolder.ServerRelativeUrl;
            string libraryPath = siteCollection.MakeFullUrl(libraryRelativePath);
            string documentPath = libraryPath + "/" + documentName + fileType;

            Stream documentStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(documentStream);
            writer.Write("");
            writer.Flush();

            Hashtable docProperties = new Hashtable();
            docProperties["vti_title"] = documentName;
            //docProperties["Color"] = "Green";

            try
            {
                subWeb.Files.Add(documentPath, documentStream, docProperties, false);
                SPFieldUrlValue urlValue = new SPFieldUrlValue();
                urlValue.Description = documentName;
                urlValue.Url = documentPath;

                properties.ListItem["Document"] = urlValue;
                properties.ListItem.Update();
                ////Response.Redirect(libraryPath);
            }
            catch//文档已经存在，会报错
            {

            }

        }
        #endregion
    }
}